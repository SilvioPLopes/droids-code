from pathlib import Path
import sys, json, hashlib, shutil

ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / 'output/ferramentas/lpc-portatil'))
from PIL import Image

BASE = ROOT / 'Assets/sprites/Entities/Characters'
ORIGENS = Path(__file__).parent / 'lpc-origens'
PALETAS = json.loads((ORIGENS / 'palette_definitions/cloth/cloth_ulpc.json').read_text())

def cor(h):
    return tuple(bytes.fromhex(h.lstrip('#')))

def recolorir(imagem, origem, destino, torso=False):
    resultado = imagem.convert('RGBA').copy()
    pares = dict(zip(map(cor, PALETAS[origem]), map(cor, PALETAS[destino])))
    pixels = resultado.load()
    for y in range(resultado.height):
        if torso and not 25 <= y % 64 <= 52:
            continue
        for x in range(resultado.width):
            r, g, b, a = pixels[x, y]
            if a and (r, g, b) in pares:
                pixels[x, y] = (*pares[r, g, b], a)
    return resultado

especificacoes = [
    ('Benta', 'medico_01', 'white', 'tan', 'spritesheets/torso/aprons/apron_full/female/walk/white.png'),
    ('Zeca', 'guarda_01', None, None, 'spritesheets/hat/cloth/hood/adult/walk.png'),
    ('Crianca_Sula_A', 'crianca_01', 'red', 'navy', None),
    ('Crianca_Sula_B', 'crianca_01', 'red', 'forest', None),
]
registro = []
for nome, modelo, origem, destino, camada in especificacoes:
    pasta = BASE / 'NPCs' / nome
    if pasta.exists():
        raise RuntimeError(f'Destino existente; inspecionar antes de repetir: {pasta}')
    fonte = BASE / 'NPCs/Generic' / modelo / 'spritesheet_walk.png'
    imagem = Image.open(fonte).convert('RGBA')
    assert imagem.size == (576, 256)
    if origem:
        imagem = recolorir(imagem, origem, destino, torso=nome == 'Benta')
    if camada:
        extra = Image.open(ORIGENS / camada).convert('RGBA')
        assert extra.size == imagem.size
        if nome == 'Zeca':
            extra = recolorir(extra, 'white', 'brown')
        imagem = Image.alpha_composite(imagem, extra)
    # LPC mascara magenta, se presente na camada de origem, nao e uma cor visivel.
    dados = list(imagem.getdata())
    imagem.putdata([(0, 0, 0, 0) if p[:3] == (255, 44, 230) else p for p in dados])
    pasta.mkdir(parents=True)
    imagem.save(pasta / 'spritesheet_walk.png')
    imagem.crop((0, 128, 64, 192)).save(pasta / 'sprite.png')
    receita = {'id': nome, 'method': 'derived_LPC_composite', 'baseRecipe': str(fonte.parent.relative_to(ROOT) / 'character-config.json'), 'baseWalk': str(fonte.relative_to(ROOT)), 'recolor': {'from': origem, 'to': destino}, 'additionalLayer': camada, 'animationsProduced': ['walk'], 'directions': ['up', 'left', 'down', 'right'], 'frameSize': [64, 64], 'approximations': ['Benta: presilhas e caixa cirurgica nao representadas; avental sobre modelo medico existente.' if nome == 'Benta' else 'Zeca: binoculo e cano nao representados; capuz sobre modelo guarda existente.' if nome == 'Zeca' else 'Variante visual proposta; nome, parentesco individual e genero narrativo nao definidos.']}
    (pasta / 'character-config.json').write_text(json.dumps(receita, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    shutil.copy2(BASE / 'NPCs/Generic/CREDITS.md', pasta / 'CREDITS-base.md')
    creditos = ['# Origem da variante', '', 'Base LPC previamente existente no projeto: ' + modelo + '.', 'Creditos completos da base em CREDITS-base.md.', 'Nao e desenho original independente nem copia declarada como geracao do zero.', '']
    if camada:
        definicao = 'sheet_definitions/torso/aprons/torso_aprons_apron_full.json' if nome == 'Benta' else 'sheet_definitions/headwear/coverings/hoods/hat_hood_cloth.json'
        d = json.loads((ORIGENS / definicao).read_text(encoding='utf-8'))
        creditos.extend(['Camada adicional: ' + camada, 'Fonte: https://github.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator', 'SHA256 camada: ' + hashlib.sha256((ORIGENS / camada).read_bytes()).hexdigest(), '', json.dumps(d['credits'], ensure_ascii=False, indent=2)])
    (pasta / 'character-config.md').write_text('# ' + nome + '\n\nVariante modular LPC para montagem de Ferrovale.\n\n' + '\n'.join(receita['approximations']) + '\n\nWalk: 576x256, quatro direcoes; referencia:64x64. Nao foi criada animacao idle nova.\n', encoding='utf-8')
    (pasta / 'CREDITS.md').write_text('\n'.join(creditos) + '\n', encoding='utf-8')
    registro.append({'id': nome, 'folder': pasta.relative_to(ROOT).as_posix(), 'width': 576, 'height': 256, 'spriteSHA256': hashlib.sha256((pasta / 'sprite.png').read_bytes()).hexdigest()})
(Path(__file__).parent / 'sprites-geradas.json').write_text(json.dumps(registro, indent=2) + '\n', encoding='utf-8')
print(json.dumps(registro))
