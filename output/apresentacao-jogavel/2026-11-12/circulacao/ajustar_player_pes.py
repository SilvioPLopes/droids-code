"""Ajuste aos pes do sprite observado, somente CircleCollider do GO PlayerMovement."""
import hashlib,json,re,shutil,sys
from pathlib import Path
BASE=Path(__file__).resolve().parent;ROOT=BASE.parents[3]
guid='f4dec413241413f418f0f5606601cd49'
header=re.compile(rb'(?m)^--- !u!(\d+) &(-?\d+)(?: stripped)?\r?\n')
prepared=[]
for path in (ROOT/'Assets/Scenes').rglob('*.unity'):
    before=path.read_bytes()
    if guid.encode() not in before:continue
    hs=list(header.finditer(before));blocks=[(int(h[1]),int(h[2]),h.start(),hs[n+1].start() if n+1<len(hs) else len(before)) for n,h in enumerate(hs)]
    movement=[before[a:b] for kind,_,a,b in blocks if kind==114 and guid.encode() in before[a:b]]
    assert len(movement)==1
    go=re.search(rb'm_GameObject: \{fileID: (-?\d+)\}',movement[0])[1]
    target=[(fid,a,b) for kind,fid,a,b in blocks if kind==58 and re.search(rb'm_GameObject: \{fileID: '+go+rb'\}',before[a:b])]
    assert len(target)==1
    fid,a,b=target[0];old=before[a:b]
    assert re.search(rb'^  m_Radius: 0\.15\r?$',old,re.M)
    assert re.search(rb'^  m_Offset: \{x: 0, y: 0\.31\}\r?$',old,re.M)
    assert re.search(rb'^  m_Enabled: 1\r?$',old,re.M)
    new=old.replace(b'm_Radius: 0.15',b'm_Radius: 0.065').replace(b'm_Offset: {x: 0, y: 0.31}',b'm_Offset: {x: 0, y: 0.215}')
    assert new.replace(b'm_Radius: 0.065',b'm_Radius: 0.15').replace(b'm_Offset: {x: 0, y: 0.215}',b'm_Offset: {x: 0, y: 0.31}')==old
    prepared.append((path,before,before[:a]+new+before[b:],fid,go.decode()))
assert len(prepared)==34
print('PREVIA:34CircleColliderPlayer; radius.15->.065,offsetY.31->.215; NPCs intactos')
if '--apply' not in sys.argv:sys.exit(0)
receipt=BASE/'aplicacao-player-pes.json';assert not receipt.exists()
for path,before,after,_,_ in prepared:
    dst=BASE/'baseline/player-pes'/path.relative_to(ROOT);assert not dst.exists()
    dst.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(path,dst);shutil.copy2(str(path)+'.meta',str(dst)+'.meta')
records=[]
for path,before,after,fid,go in prepared:
    assert path.read_bytes()==before;path.write_bytes(after);assert path.read_bytes()==after
    records.append({'path':str(path.relative_to(ROOT)),'colliderFileID':fid,'playerGameObjectFileID':go,'antesSHA256':hashlib.sha256(before).hexdigest(),'depoisSHA256':hashlib.sha256(after).hexdigest()})
receipt.write_text(json.dumps({'antes':{'radius':.15,'offsetY':.31},'depois':{'radius':.065,'offsetY':.215},'arquivos':records},indent=2))
print('APLICADO:34players ajustados aos pes, colisores habilitados, nenhum NPC alterado')
