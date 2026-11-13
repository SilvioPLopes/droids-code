import hashlib, json, re, shutil
from pathlib import Path

root = Path.cwd()
folder = root / 'output/apresentacao-jogavel/2026-11-12/circulacao'
source = root / 'Assets/Scenes/City.unity'
old = source.read_bytes()
sha = lambda data: hashlib.sha256(data).hexdigest().upper()
assert sha(old) == 'C250200237629709295E391272CB098250A22E0AA0F2ED90F75C73B980AFEECE', 'City mudou; reavaliar'
pattern = rb'(?ms)^--- !u!4 &1292165768[^\r\n]*\r?\n.*?(?=^--- !u!|\Z)'
matches = list(re.finditer(pattern, old))
assert len(matches) == 1
block = matches[0]
before = b'm_LocalPosition: {x: 8.89, y: -7.65, z: 0}'
after = b'm_LocalPosition: {x: 8.89, y: -7.88653814, z: 0}'
assert block.group().count(before) == 1
modified = block.group().replace(before, after, 1)
new = old[:block.start()] + modified + old[block.end():]
backup = folder / 'baseline/soleira-casalip/City.unity'
assert not backup.exists(), 'Consultar aplicacao anterior antes de repetir'
backup.parent.mkdir(parents=True, exist_ok=True)
backup.write_bytes(old)
shutil.copyfile(str(source) + '.meta', str(backup) + '.meta')
assert source.read_bytes() == old
source.write_bytes(new)
assert source.read_bytes() == new
assert Path(str(source) + '.meta').read_bytes() == Path(str(backup) + '.meta').read_bytes()
report = {'arquivo': 'Assets/Scenes/City.unity', 'objeto': 'Entrada_CasaLip',
          'transformFileID': 1292165768, 'campo': 'm_LocalPosition.y',
          'antes': -7.65, 'depois': -7.88653814, 'shaAntes': sha(old), 'shaDepois': sha(new),
          'estado': 'EDITADO_SALVO_COMPORTAMENTO_PENDENTE'}
(folder / 'aplicacao-soleira-casalip.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
print(json.dumps(report))
