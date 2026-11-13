"""Reativa somente os componentes do lote documentado, sem restaurar prefabs inteiros."""
import hashlib
import json
from pathlib import Path
import re
import shutil
import sys

BASE = Path(__file__).resolve().parent
ROOT = BASE.parents[3]
manifest = json.loads((BASE / 'mobiliario-preview.json').read_text(encoding='utf-8-sig'))
prepared = []
for entry in manifest['arquivos']:
    path = ROOT / entry['path']
    assert path.resolve().is_relative_to(ROOT / 'Assets/Prefabs/Interiores')
    before = path.read_bytes()
    after = bytearray(before)
    for component in entry['componentes']:
        pattern = (rb'^--- !u!61 &' + component['componentFileID'].encode()
                   + rb'\r?\n(?:(?!^---).)*?^  m_Enabled: ([01])')
        match = re.search(pattern, before, re.M | re.S)
        assert match and match[1] == b'0', (path, component['componentFileID'])
        after[match.start(1)] = ord('1')
    assert sum(a != b for a, b in zip(before, after)) == len(entry['componentes'])
    prepared.append((path, before, bytes(after), entry))
assert len(prepared) == 30 and sum(len(x[3]['componentes']) for x in prepared) == 98
if '--apply' not in sys.argv:
    print('PREVIA: 98 componentes, 30 prefabs, somente enabled0->1')
    sys.exit(0)
receipt = BASE / 'reativacao-mobiliario.json'
assert not receipt.exists(), 'Aplicacao ja registrada; conferir estado antes de repetir'
for path, before, after, entry in prepared:
    backup = BASE / 'baseline/reativacao' / entry['path']
    assert not backup.exists()
    backup.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(path, backup)
    shutil.copy2(str(path) + '.meta', str(backup) + '.meta')
records = []
for path, before, after, entry in prepared:
    assert path.read_bytes() == before
    path.write_bytes(after)
    assert path.read_bytes() == after
    records.append({'path': entry['path'], 'componentes': len(entry['componentes']),
                    'antesSHA256': hashlib.sha256(before).hexdigest(),
                    'depoisSHA256': hashlib.sha256(after).hexdigest()})
receipt.write_text(json.dumps({'alteracao': 'enabled0->1; geometria preservada',
                               'componentes': 98, 'arquivos': records}, indent=2), encoding='utf-8')
print('APLICADO: 98 colisores reativados; 30 prefabs; backups e recibo preservados')
