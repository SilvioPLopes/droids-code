"""Análise de cenas salvas: geometria estática e rotas de retorno, sem Play/save.
Grade 0.05 mundo. Inclui corpos de NPCs, dimensões reais de triggers e instâncias.
Não substitui input real, física em movimento, condições narrativas ou aceite visual.
"""
import json, math
from collections import deque
from pathlib import Path

BASE=Path(__file__).resolve().parent
load=lambda p:json.loads(p.read_text(encoding='utf-8-sig'))
rooms={j['scene']:j for p in list(BASE.glob('Interior_*.json'))+[BASE/'CasaLip.json'] for j in [load(p)]}
city=load(BASE/'city-build.json')
build={Path(s['path']).stem for s in city['build'] if s['enabled']}
xy=lambda p:(p['x'],p['y'])
STEP=.05

def segment(p,a,b):
    x,y=b[0]-a[0],b[1]-a[1];d=x*x+y*y
    t=max(0,min(1,((p[0]-a[0])*x+(p[1]-a[1])*y)/d)) if d else 0
    return math.hypot(p[0]-a[0]-t*x,p[1]-a[1]-t*y)

def contains(p,poly):
    inside=False
    for a,b in zip(poly,poly[1:]+poly[:1]):
        if (a[1]>p[1])!=(b[1]>p[1]) and p[0]<(b[0]-a[0])*(p[1]-a[1])/(b[1]-a[1])+a[0]:inside=not inside
    return inside

state={}
for name,r in rooms.items():
    # CasaLip: topologia inspecionada; sua física/puzzle antigos fora da grade nova.
    if name=='CasaLip':continue
    radius=next(p['radius'] for p in r['player']['foot'] if not p['isTrigger'])
    offset=tuple(a-b for a,b in zip(xy(r['player']['foot'][0]['position']),xy(r['player']['position'])))
    boxes=[]; circles=[]; outlines=[]; unsupported=[]
    for c in r['colliders']:
        if not c['enabled'] or not c['active']:continue
        if c['type']=='BoxCollider2D' and abs(c['rotation'])<.01:
            x,y=xy(c['center']); w=abs(c['size']['x']*c['scale']['x'])/2;h=abs(c['size']['y']*c['scale']['y'])/2
            boxes.append((x-w,y-h,x+w,y+h))
        elif c['type']=='CircleCollider2D':circles.append((*xy(c['center']),c['radius']*max(abs(c['scale']['x']),abs(c['scale']['y']))))
        elif c['type']=='EdgeCollider2D' and c['points'] and math.dist(xy(c['points'][0]),xy(c['points'][-1]))<.001:
            outlines.append(([xy(p) for p in c['points']],c['edgeRadius']))
        else:unsupported.append(c['path'])
    def free(p):
        for poly,edge in outlines:
            if not contains(p,poly):return False
            if any(segment(p,a,b)<radius+edge-1e-6 for a,b in zip(poly,poly[1:])):return False
        for x0,y0,x1,y1 in boxes:
            if math.hypot(p[0]-max(x0,min(x1,p[0])),p[1]-max(y0,min(y1,p[1])))<radius-1e-6:return False
        if any(math.hypot(p[0]-x,p[1]-y)<radius+cr-1e-6 for x,y,cr in circles):return False
        return True
    b=r['background'];cx,cy=xy(b['position']);hw=b['width']/b['ppu']*abs(b['scale']['x'])/2;hh=b['height']/b['ppu']*abs(b['scale']['y'])/2
    coords={(i,k):(i*STEP,k*STEP) for i in range(math.ceil((cx-hw)/STEP),math.floor((cx+hw)/STEP)+1) for k in range(math.ceil((cy-hh)/STEP),math.floor((cy+hh)/STEP)+1) if free((i*STEP,k*STEP))}
    labels={};members=[]
    for start in sorted(coords):
        if start in labels:continue
        ident=len(members);queue=deque([start]);labels[start]=ident;points=[]
        while queue:
            n=queue.popleft();points.append(coords[n])
            for q in ((n[0]-1,n[1]),(n[0]+1,n[1]),(n[0],n[1]-1),(n[0],n[1]+1)):
                if q in coords and q not in labels and free(((coords[n][0]+coords[q][0])/2,(coords[n][1]+coords[q][1])/2)):
                    labels[q]=ident;queue.append(q)
        members.append(points)
    def locate(p):
        if not free(p):return None
        cand=[n for n in coords if math.dist(coords[n],p)<STEP*1.5]
        return labels[min(cand,key=lambda n:math.dist(coords[n],p))] if cand else None
    portal_components={}
    for p in r['portals']:
        found=[]
        for i,points in enumerate(members):
            for c in p['colliders']:
                if not c['enabled'] or not c['isTrigger']:continue
                x,y=xy(c['center']);w,h=c['size']['x']/2,c['size']['y']/2
                if any(math.hypot(max(abs(q[0]-x)-w,0),max(abs(q[1]-y)-h,0))<radius for q in points):found.append(i);break
        portal_components[p['path']]=found
    incoming=[]
    for p in city['doors']:
        if p['target']==name:incoming.append(('City/'+p['name'],p['arrival']))
    for s in rooms.values():
        for p in s['portals']+s['visits']:
            if p['target']==name:incoming.append((s['scene']+'/'+p['path'],p['arrival']))
    entries=[]
    for origin,p in incoming:
        foot=(p['x']+offset[0],p['y']+offset[1]);entries.append({'source':origin,'foot':foot,'component':locate(foot)})
    default=locate(xy(r['player']['foot'][0]['position']))
    visits={}
    for v in r['visits']:
        body=next((c for c in r['colliders'] if c['path']==v['path'] and c['type']=='CircleCollider2D'),None)
        if body:visits[v['path']]=[i for i,points in enumerate(members) if any(math.dist(p,xy(body['center']))<.91+radius for p in points)]
    state[name]={'radius':radius,'components':len(members),'sizes':[len(x) for x in members],'portalComponents':portal_components,'arrivals':entries,'defaultSpawnComponent':default,'visitComponents':visits,'unsupported':unsupported}

# Grafo de componentes de piso: atravessar portal chega à componente efetiva.
graph={};invalid=[];dead=[]
for name,r in rooms.items():
    for p in r['portals']+r['visits']:
        if p['target'] not in build:invalid.append({'scene':name,'path':p['path'],'target':p['target']})
    if name not in state:continue
    for p in r['portals']+r['visits']:
        is_visit=p in r['visits'];components=state[name]['visitComponents' if is_visit else 'portalComponents'].get(p['path'],[])
        if not components:dead.append({'scene':name,'portal':p['path'],'reason':'nenhum contato no piso livre da grade'})
        if p['target']=='City':dest='City'
        elif p['target'] in state:
            arrival=next(a for a in state[p['target']]['arrivals'] if a['source']==name+'/'+p['path'])
            dest=(p['target'],arrival['component']) if arrival['component'] is not None else None
        else:dest=None
        for comp in components:
            if not is_visit and (p['trancada'] or not p['enabled'] or not p['active']):continue
            if dest is not None:graph.setdefault((name,comp),set()).add(dest)

def reaches_city(start):
    todo=[start];seen=set()
    while todo:
        n=todo.pop()
        if n=='City':return True
        if n in seen:continue
        seen.add(n);todo.extend(graph.get(n,[]))
    return False
risks=[]
for name,s in state.items():
    for a in s['arrivals']:
        a['routeToCity']=a['component'] is not None and reaches_city((name,a['component']))
        if not a['routeToCity']:risks.append({'scene':name,**a})
    s['defaultSpawnRouteToCity']=s['defaultSpawnComponent'] is not None and reaches_city((name,s['defaultSpawnComponent']))

summary={'sceneCount':len(rooms),'physicalTransitions':sum(len(r['portals']) for r in rooms.values()),'dialogueVisits':sum(len(r['visits']) for r in rooms.values()),'invalidInteriorTargets':invalid,'cityEmptyTargets':[p['name'] for p in city['doors'] if not p['target']], 'unreachablePortalContacts':dead,'arrivalsWithoutRouteToCity':risks,'rooms':state,
 'limits':'Geometria estática de instâncias de cena; grade0.05; inclui NPCs imóveis e dimensões reais de triggers. CasaLip só topologia, pois puzzle/colisões antigos fora desta grade. Rotas de visita dependem de interação. Não simula movimento nem aprovação visual.'}
logical={name:{p['target'] for p in r['portals']+r['visits'] if p['target']} for name,r in rooms.items()}
def logical_return(name):
    pending=[name];seen=set()
    while pending:
        n=pending.pop()
        if n=='City':return True
        if n in seen:continue
        seen.add(n);pending.extend(logical.get(n,[]))
    return False
summary['logicalRoomsWithoutReturn']=[n for n in rooms if not logical_return(n)]
summary['interiorConnectionsWithoutReverse']=[{'scene':n,'portal':p['path'],'target':p['target']} for n,r in rooms.items() for p in r['portals'] if p['target'] in rooms and n not in logical[p['target']]]
overlaps=[]
for name,s in state.items():
    for a in s['arrivals']:
        for p in rooms[name]['portals']:
            for c in p['colliders']:
                if not c['enabled'] or not c['isTrigger']:continue
                x,y=xy(c['center']);w,h=c['size']['x']/2,c['size']['y']/2
                if math.hypot(max(abs(a['foot'][0]-x)-w,0),max(abs(a['foot'][1]-y)-h,0))<s['radius']:
                    overlaps.append({'scene':name,'source':a['source'],'portal':p['path']})
summary['arrivalsOverlappingTriggers']=overlaps
(BASE/'resultado-fluxo.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in summary.items() if k not in ('rooms','limits')},ensure_ascii=False,indent=2))
