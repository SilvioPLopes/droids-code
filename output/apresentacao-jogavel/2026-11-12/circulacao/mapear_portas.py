"""Mapa offline de referencias. Nao controla Unity nem altera assets/saves."""
import argparse
import hashlib
import json
from pathlib import Path

BASE = Path('output/apresentacao-jogavel/2026-11-12/circulacao')
SOURCE = BASE / 'inventario-simplificado/inventario.json'
OUT = BASE / 'portas-referencia/mapa-portas.json'


def pixel(point, fundo):
    if not fundo:
        return None
    a, b, _, d = fundo['cornersWorld']
    ux, uy = b['x'] - a['x'], b['y'] - a['y']
    vx, vy = d['x'] - a['x'], d['y'] - a['y']
    determinant = ux * vy - uy * vx
    if abs(determinant) < 1e-9:
        raise ValueError('Base world/pixels singular')
    x, y = point['x'] - a['x'], point['y'] - a['y']
    u, v = (x * vy - y * vx) / determinant, (ux * y - uy * x) / determinant
    if fundo['flipX']:
        u = 1 - u
    if fundo['flipY']:
        v = 1 - v
    return {'x': fundo['rectXPixels'] + u * fundo['rectWidthPixels'],
            'y': fundo['textureHeight'] - fundo['rectYPixels'] - v * fundo['rectHeightPixels']}


def imagem(scene):
    return (scene.get('fundo') or {}).get('textureAssetPath')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source', type=Path, default=SOURCE)
    parser.add_argument('--out', type=Path, default=OUT)
    args = parser.parse_args()
    source, out = args.source, args.out
    original = source.read_bytes()
    inv = json.loads(original)
    cenas = {c['nome']: c for c in inv['cenas']}
    ambientes = set(cenas) - {'City'}
    rows = []
    for scene in inv['cenas']:
        collider = {c['colliderPath']: c for c in scene['solidos'] + scene['triggers']}
        links = [dict(p, tipo='escada' if p['nome'].startswith('escada_') else 'porta') for p in scene['portais']]
        links += [dict(nome=n['nome'], caminho=n['caminho'], destino=n['destinoDeVisita'],
                       pontoChegadaX=n['chegadaDaVisitaX'], pontoChegadaY=n['chegadaDaVisitaY'],
                       enabled=n['enabled'], active=n['active'], colliderPaths=n['triggerColliderPaths'],
                       tipo='visita_por_dialogo') for n in scene['npcs'] if n['destinoDeVisita']]
        for p in links:
            target = cenas.get(p['destino'])
            retorno = [] if target is None else [x for x in target['portais'] if x['destino'] == scene['nome']]
            nome_par = p['nome'][6:] if p['nome'].startswith('saida_') else 'saida_' + p['nome']
            exatos = [x for x in retorno if x['nome'] == nome_par]
            escolhido = exatos[0] if len(exatos) == 1 else retorno[0] if len(retorno) == 1 else None
            player = (target['players'] or [None])[0] if target else None
            raiz = {'x': p['pontoChegadaX'], 'y': p['pontoChegadaY']}
            corpo = None if player is None else {'x': raiz['x'] + player['colliderCenterOffsetX'],
                                               'y': raiz['y'] + player['colliderCenterOffsetY']}
            formas = []
            for key in p['colliderPaths']:
                c = collider[key]
                cantos = c['cornersWorld'] or [dict(x=x, y=y) for x, y in
                    ((c['minX'], c['minY']), (c['maxX'], c['minY']), (c['maxX'], c['maxY']), (c['minX'], c['maxY']))]
                formas.append({'id': key, 'tipo': c['tipo'], 'ativo': c['active'] and c['enabled'],
                               'world': cantos, 'pixelsTextura': [pixel(x, scene['fundo']) for x in cantos]})
            recorte = p['active'] and p['enabled'] and (
                scene['nome'] == 'City' and p['destino'] in ambientes or
                scene['nome'] in ambientes and p['destino'] in ambientes | {'City'})
            rows.append({'id': scene['nome'] + '::' + p['caminho'] + '::' + p['tipo'],
                'origem': scene['nome'], 'caminho': p['caminho'], 'nome': p['nome'], 'tipo': p['tipo'],
                'destino': p['destino'], 'noRecorteCirculacao': bool(recorte), 'imagemOrigem': imagem(scene),
                'gatilhos': formas, 'imagemDestino': imagem(target) if target else None,
                'chegadaRaizWorld': raiz, 'chegadaCorpoWorld': corpo,
                'raioCorpoDestino': player['raioWorld'] if player else None,
                'chegadaPixelsTextura': pixel(raiz, target['fundo']) if target else None,
                'corpoPixelsTextura': pixel(corpo, target['fundo']) if corpo else None,
                'retornosPossiveis': [x['caminho'] for x in retorno],
                'retornoIdentificado': escolhido['caminho'] if escolhido else None,
                'reciprocidade': 'pareamento_exato' if len(exatos) == 1 else 'retorno_unico' if escolhido else
                    'ambiguo' if len(retorno) > 1 else 'sem_portal_reciproco_no_snapshot',
                'soleiraVisual': None,
                'estadoVisual': 'requer_confirmacao_na_imagem; visita_dialogo_nao_implica_porta_desenhada',
                'estadoComportamento': 'nao_testado_por_este_gerador'})
    assert len(rows) == 70 and len({x['id'] for x in rows}) == 70, 'Ligacao perdida ou duplicada'
    assert sum(x['noRecorteCirculacao'] for x in rows) == 68, 'Recorte divergiu da referencia'
    for nome in ('Porta_Casa02', 'Porta_Casa02_Esquerda', 'Porta_PredioPlaca_A', 'Porta_PredioPlaca_B'):
        row = next(x for x in rows if x['origem'] == 'City' and x['nome'] == nome)
        assert row['retornoIdentificado'].endswith('saida_' + nome), 'Portas duplas confundidas'
    for c in inv['cenas']:
        f = c.get('fundo')
        if f:
            p = pixel(f['cornersWorld'][0], f)
            ex = f['rectXPixels'] + (f['rectWidthPixels'] if f['flipX'] else 0)
            ey = f['textureHeight'] - f['rectYPixels'] - (f['rectHeightPixels'] if f['flipY'] else 0)
            assert abs(p['x'] - ex) < .001 and abs(p['y'] - ey) < .001
    report = {'versao': 1, 'inventario': str(source), 'inventarioSHA256': hashlib.sha256(original).hexdigest().upper(),
              'exportadoEmUtc': inv['exportadoEmUtc'],
              'limite': 'Referencia datada do snapshot; nao comprova estado posterior, soleira visual nem percurso fisico.',
              'metodo': ['Confirmar soleira visivel na imagem e registrar ancora/referencia visual.',
                         'Cruzar ancora com trigger; checar solidos e corpo integral.',
                         'Checar chegada junto ao acesso correto, livre e fora dos triggers.',
                         'Percorrer ida/volta pelo controlador real e registrar resultado.'],
              'total': len(rows), 'noRecorte': 68, 'ligacoes': rows}
    out.parent.mkdir(parents=True, exist_ok=True)
    assert not out.exists(), 'Saida ja existe; conservar referencia anterior'
    out.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding='utf-8')
    print(json.dumps({'arquivo': str(out), 'registros': len(rows), 'noRecorte': 68,
                      'verificacoes': 'IDs, portas duplas e projecao de imagens passaram',
                      'Unity': 'nao_acessado'}, ensure_ascii=False))


if __name__ == '__main__':
    main()
