from pathlib import Path
import re, json, sys, hashlib, shutil
ROOT = Path(__file__).resolve().parents[3]
OUT = Path(__file__).parent
BACK = ROOT / 'output/backups/2026-10-29-montagem-ferrovale'
PATTERN = rb'(?m)^--- !u!(\d+) &(\d+)[^\n]*\n[\s\S]*?(?=^--- !u!|\Z)'
apply = '--apply' in sys.argv
reports = []
for path, receipt in [('Assets/Scenes/City.unity','city-edits.json'),('Assets/Scenes/CasaLip.unity','casalip-edits.json'),('Assets/Scenes/Interiores/Interior_E10_Casa03.unity',None)]:
    src = ROOT/path; before = (BACK/path).read_bytes(); after = src.read_bytes()
    old = {m.group(2).decode():m for m in re.finditer(PATTERN,before)}
    new = {m.group(2).decode():m for m in re.finditer(PATTERN,after)}
    allowed = set()
    data = json.loads((OUT/receipt).read_text()) if receipt else {}
    allowed.update(data.get('allowedOldIds',[]))
    roots = {i for i,m in old.items() if m.group(1)==b'1660057539'}
    allowed.update(roots)
    if receipt == 'city-edits.json':
        names = [d['name'].encode() for d in data['doors']]
        instances = {i for i,m in old.items() if m.group(1)==b'1001' and any(b'value: '+n+b'\n' in m.group(0).replace(b'\r\n',b'\n') for n in names)}
        assert len(instances)==21,len(instances)
        allowed.update(instances)
        for i,m in old.items():
            if any(('m_PrefabInstance: {fileID: '+p+'}').encode() in m.group(0) for p in instances):allowed.add(i)
    missing = allowed & (set(old)-set(new))
    assert not missing,('allowed object removed',missing)
    first = min(m.start() for m in old.values()); chunks=[before[:first]]
    for i,m in old.items():
        if i in roots:continue
        chunks.append(new[i].group(0) if i in allowed else m.group(0))
    for i,m in new.items():
        if i not in old:chunks.append(m.group(0))
    for i in roots:chunks.append(new[i].group(0))
    result=b''.join(chunks)
    final={m.group(2).decode():m.group(0) for m in re.finditer(PATTERN,result)}
    assert all(final[i]==m.group(0) for i,m in old.items() if i not in allowed)
    assert len(final)==len(old)+len(set(new)-set(old))
    report={'scene':path,'existingBlocksChanged':sum(final[i]!=m.group(0) for i,m in old.items()),'newBlocks':len(set(new)-set(old)),'unrelatedExistingBlocksBytePreserved':sum(i not in allowed for i in old),'sha256':hashlib.sha256(result).hexdigest()}
    if apply:
        diagnostic=OUT/(src.stem+'-serializacao-completa.unity')
        if not diagnostic.exists():shutil.copy2(src,diagnostic)
        src.write_bytes(result)
    reports.append(report)
if apply:(OUT/'diff-cenas.json').write_text(json.dumps(reports,indent=2)+'\n',encoding='utf-8')
print(json.dumps({'applied':apply,'scenes':reports}))
