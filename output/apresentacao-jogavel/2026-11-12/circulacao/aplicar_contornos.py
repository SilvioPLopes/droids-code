"""Aplica retangulos revisados em pixels a IDs existentes; nunca desliga colisores."""
import hashlib
import json
from pathlib import Path
import re
import shutil
import sys

BASE=Path(__file__).resolve().parent
ROOT=BASE.parents[3]
manifest=json.loads((BASE/'mobiliario-preview.json').read_text(encoding='utf-8-sig'))
inventory=json.loads((BASE/'inventario-inicial/inventario.json').read_text(encoding='utf-8-sig'))
recipe=json.loads((BASE/'contornos-corrigidos.json').read_text())
changes={}; records=[]
for a in recipe['alvos']:
    scene=next(s for s in inventory['cenas'] if s['nome']==a['cena'])
    f=scene['fundo']; col=next(c for c in scene['solidos'] if c['nome']==a['nome'])
    entry=next(e for e in manifest['arquivos'] if e['path']==col['prefabSourcePath'])
    component=next(c for c in entry['componentes'] if c['nome']==a['nome'])
    path=ROOT/entry['path']
    assert path.resolve().is_relative_to(ROOT/'Assets/Prefabs/Interiores')
    if path not in changes:changes[path]=[path.read_bytes(),path.read_bytes()]
    before,data=changes[path]
    pattern=rb'^--- !u!61 &'+component['componentFileID'].encode()+rb'\r?\n(?:(?!^---).)*'
    m=re.search(pattern,data,re.M|re.S);assert m
    block=m[0]
    assert re.search(rb'^  m_Enabled: 1\r?$',block,re.M)
    def field(key):
        z=re.search(rb'^  '+key.encode()+rb': ([^\r\n]+)',block,re.M)
        assert z;return z
    old_size=field('m_Size');old_offset=field('m_Offset')
    assert old_size[1].decode()==component['geometriaOriginalYaml']['m_Size']
    assert old_offset[1].decode()==component['geometriaOriginalYaml']['m_Offset']
    assert not f['flipX'] and not f['flipY'] and f['rectXPixels']==0 and f['rectYPixels']==0
    assert f['rectWidthPixels']==f['textureWidth'] and f['rectHeightPixels']==f['textureHeight']
    x0,y0,x1,y1=a['pixels'];assert 0<x0<x1<f['textureWidth'] and 0<y0<y1<f['textureHeight']
    sx=(f['maxX']-f['minX'])/f['textureWidth'];sy=(f['maxY']-f['minY'])/f['textureHeight']
    world_x=f['minX']+(x0+x1)/2*sx;world_y=f['maxY']-(y0+y1)/2*sy
    def vec(v):return [float(x) for x in re.findall(rb'[xy]: ([\d.eE+\-]+)',v)]
    size=vec(old_size[1]);offset=vec(old_offset[1])
    scale_x=col['width']/size[0];scale_y=col['height']/size[1]
    assert abs(scale_x-1)<1e-5 and abs(scale_y-1)<1e-5
    new_offset=[offset[0]+(world_x-col['x'])/scale_x,offset[1]+(world_y-col['y'])/scale_y]
    new_size=[(x1-x0)*sx/scale_x,(y1-y0)*sy/scale_y]
    def yaml(v):return ('{x: %.8g, y: %.8g}'%tuple(v)).encode()
    newblock=block.replace(old_size[0],b'  m_Size: '+yaml(new_size),1).replace(old_offset[0],b'  m_Offset: '+yaml(new_offset),1)
    assert newblock.replace(b'  m_Size: '+yaml(new_size),old_size[0],1).replace(b'  m_Offset: '+yaml(new_offset),old_offset[0],1)==block
    changes[path][1]=data[:m.start()]+newblock+data[m.end():]
    records.append(dict(a,path=entry['path'],componentFileID=component['componentFileID'],
                        antes={'size':size,'offset':offset},depois={'size':new_size,'offset':new_offset},enabled=1))
assert len(records)==12 and len(changes)==5
if '--apply' not in sys.argv:
    print(json.dumps(records,indent=2));sys.exit(0)
receipt=BASE/'aplicacao-contornos.json';assert not receipt.exists()
for path,(before,after) in changes.items():
    backup=BASE/'baseline/contornos'/path.relative_to(ROOT)
    assert not backup.exists();backup.parent.mkdir(parents=True,exist_ok=True)
    shutil.copy2(path,backup);shutil.copy2(str(path)+'.meta',str(backup)+'.meta')
for path,(before,after) in changes.items():
    assert path.read_bytes()==before;path.write_bytes(after);assert path.read_bytes()==after
receipt.write_text(json.dumps({'alvos':records,'hashes':[{'path':str(p.relative_to(ROOT)),
    'antes':hashlib.sha256(v[0]).hexdigest(),'depois':hashlib.sha256(v[1]).hexdigest()} for p,v in changes.items()]},indent=2))
print('APLICADO:12 formas em5prefabs;98colisores continuam ativos; somente offset/size alterados')
