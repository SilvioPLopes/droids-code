"""Altera uma chegada calculada por chamada; preserva bytes e campos alheios."""
import hashlib, json, re, shutil, sys
from pathlib import Path

ROOT = Path.cwd().resolve()
OUT = ROOT / 'output/apresentacao-jogavel/2026-11-12/circulacao'
SPECS = {
    'E03': ('Assets/Scenes/City.unity', '770223059', 'Interior_E03_Terreo', (-1.495, -.07999998)),
    'E05': ('Assets/Scenes/City.unity', '129796394', 'Interior_E05_Principal', (2.005, -1.5799999)),
    'E09': ('Assets/Prefabs/Interiores/E09_Superior.prefab', '4832129328522173717', 'Interior_E09_Terreo', (2.38, .17000002)),
}

def sha(data):
    return hashlib.sha256(data).hexdigest().upper()

def main():
    key = sys.argv[1]
    source, file_id, scene, expected = SPECS[key]
    proposal = next(x for x in json.loads((OUT / 'previa-chegadas.json').read_text()) if x['cena'] == scene)
    assert proposal['livre'] and proposal['aprovados'] == proposal['total']
    path = (ROOT / source).resolve()
    assert path.is_relative_to(ROOT) and path.suffix in ('.unity', '.prefab')
    old = path.read_bytes()
    pattern = rb'(?ms)^--- !u!1001 &' + file_id.encode() + rb'\r?\n.*?(?=^--- !u!|\Z)'
    blocks = list(re.finditer(pattern, old))
    assert len(blocks) == 1, 'PrefabInstance nao unica'
    match = blocks[0]
    block = match.group()
    modified = block
    changes = []
    for axis, before, after in zip(('x', 'y'), expected, (proposal['novaX'], proposal['novaY'])):
        prop = re.compile(rb'(propertyPath: pontoDeChegada\.' + axis.encode() + rb'\r?\n[ \t]+value: )([^\r\n]*)')
        values = list(prop.finditer(modified))
        assert len(values) == 1, 'Campo ausente ou duplicado'
        current = float(values[0].group(2))
        assert abs(current - before) < 1e-6, 'Valor atual divergiu; preservar trabalho novo'
        if abs(current - after) < 1e-6:
            continue
        modified = prop.sub(lambda m: m.group(1) + format(after, '.9g').encode(), modified, count=1)
        changes.append({'campo': 'pontoDeChegada.' + axis, 'antes': current, 'depois': after})
    assert changes, 'Nenhuma alteracao prevista'
    new = old[:match.start()] + modified + old[match.end():]
    backup = OUT / 'baseline/chegadas' / key / source
    backup.parent.mkdir(parents=True, exist_ok=True)
    assert not backup.exists(), 'Backup desta etapa ja existe; consultar estado antes de repetir'
    backup.write_bytes(old)
    shutil.copyfile(str(path) + '.meta', str(backup) + '.meta')
    assert sha(path.read_bytes()) == sha(old), 'Arquivo mudou antes da escrita'
    path.write_bytes(new)
    assert sha(path.read_bytes()) == sha(new)
    assert Path(str(path) + '.meta').read_bytes() == Path(str(backup) + '.meta').read_bytes()
    report = {'caso': key, 'arquivo': source, 'prefabInstance': file_id, 'antesSHA256': sha(old),
              'depoisSHA256': sha(new), 'mudancas': changes, 'backup': str(backup.relative_to(ROOT)),
              'estado': 'EDITADO_SALVO_SEM_TESTE_DE_MOVIMENTO'}
    destination = OUT / ('aplicacao-chegada-' + key + '.json')
    assert not destination.exists()
    destination.write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding='utf-8')
    print(json.dumps(report, ensure_ascii=False))

if __name__ == '__main__':
    main()
