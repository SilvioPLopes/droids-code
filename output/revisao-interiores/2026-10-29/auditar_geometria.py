"""Diagnóstico estático; não altera Assets, não roda Unity/Play nem acessa saves.

Usa colliders dos prefabs efetivamente salvos; ignora corpos de NPCs e física
dinâmica. Grade 10px, círculo dos pés raio 60px (0.3 mundo), arte 200PPU.
Resultado é indício geométrico, não teste de locomoção/aprovação visual.
"""
import json
import math
from collections import deque
from pathlib import Path

BASE = Path(__file__).resolve().parent
ROOT = BASE.parents[2]
load = lambda p: json.loads(p.read_text(encoding="utf-8-sig"))
rooms = load(BASE / "geometria-prefabs.json")
city = load(BASE / "chegadas-city.json")
RADIUS, STEP = 60, 10

def pixel(x, y):
    return (724 + 200*x, 543 - 200*y)

def distance_segment(p, a, b):
    vx, vy = b[0]-a[0], b[1]-a[1]
    t = max(0, min(1, ((p[0]-a[0])*vx+(p[1]-a[1])*vy)/(vx*vx+vy*vy)))
    return math.hypot(p[0]-a[0]-t*vx, p[1]-a[1]-t*vy)

results = []
for room in rooms:
    polygons, boxes = [], []
    for c in room["colliders"]:
        if not c["enabled"] or not c["active"]:
            continue
        if c["type"] == "EdgeCollider2D":
            polygons.append([pixel(c["x"]+(p["x"]+c["offsetX"])*c["scaleX"],
                                   c["y"]+(p["y"]+c["offsetY"])*c["scaleY"])
                             for p in c["points"]])
        elif c["type"] == "BoxCollider2D":
            x, y = pixel(c["x"]+c["offsetX"]*c["scaleX"], c["y"]+c["offsetY"]*c["scaleY"])
            w, h = 100*c["boxW"]*c["scaleX"], 100*c["boxH"]*c["scaleY"]
            boxes.append((x-w, y-h, x+w, y+h))
    if not polygons:
        results.append({"room":room["id"], "status":"sem contorno Edge; revisar manualmente"})
        continue
    def free(p):
        for poly in polygons:
            inside = False
            for a, b in zip(poly, poly[1:]):
                if (a[1]>p[1]) != (b[1]>p[1]) and p[0] < (b[0]-a[0])*(p[1]-a[1])/(b[1]-a[1])+a[0]:
                    inside = not inside
                if distance_segment(p,a,b) < RADIUS:
                    return False
            if not inside:
                return False
        for x0,y0,x1,y1 in boxes:
            dx = p[0]-max(x0,min(x1,p[0])); dy = p[1]-max(y0,min(y1,p[1]))
            if math.hypot(dx,dy) < RADIUS:
                return False
        return True
    available = {(x,y) for x in range(0,1450,STEP) for y in range(0,1090,STEP) if free((x,y))}
    components, labels = [], {}
    for start in sorted(available):
        if start in labels:
            continue
        n = len(components); labels[start]=n; queue=deque([start]); members=[]
        while queue:
            p=queue.popleft(); members.append(p)
            for q in ((p[0]-STEP,p[1]),(p[0]+STEP,p[1]),(p[0],p[1]-STEP),(p[0],p[1]+STEP)):
                if q in available and q not in labels:
                    labels[q]=n; queue.append(q)
        components.append(members)
    def component_for(p):
        near = [q for q in available if math.dist(q,p) <= 15]
        return labels[min(near,key=lambda q:math.dist(q,p))] if free(p) and near else None
    portals_by_component = [[] for _ in components]
    for portal in room["portals"]:
        if not portal["enabled"]:
            continue
        x,y = pixel(portal["x"],portal["y"])
        # Limite dos templates novos: 150x24px. Casa03 requer revisão própria.
        for i,members in enumerate(components):
            if any(math.hypot(max(abs(p[0]-x)-75,0),max(abs(p[1]-y)-12,0)) <= RADIUS for p in members):
                portals_by_component[i].append(portal["name"])
    arrivals=[]
    target="Interior_"+room["id"]
    sources=[{"source":"City/"+p["door"],"x":p["x"],"y":p["y"]} for p in city if p["target"]==target]
    sources += [{"source":r["id"]+"/"+p["name"],"x":p["targetX"],"y":p["targetY"]}
                for r in rooms for p in r["portals"] if p["target"]==target]
    for p in sources:
        foot=pixel(p["x"],p["y"]+.62); comp=component_for(foot)
        arrivals.append({"source":p["source"],"footPixel":[round(v,2) for v in foot],
                         "component":comp,"portalsReachable":portals_by_component[comp] if comp is not None else [],
                         "status":"livre com portal" if comp is not None and portals_by_component[comp] else "RISCO: chegada sem portal na componente"})
    results.append({"room":room["id"],"components":[{"id":i,"gridPoints":len(c),"portals":portals_by_component[i]} for i,c in enumerate(components)],"arrivals":arrivals})

report={"method":"geometria estática dos prefabs; grade10px; pés raio60px; sem corpos de NPCs/Play",
        "limits":"não comprova percurso físico, alinhamento com arte ou input; Casa03 sem Edge exige inspeção própria",
        "results":results}
(BASE/"analise-conectividade.json").write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding="utf-8")
for r in results:
    risks=[a for a in r.get("arrivals",[]) if a["status"].startswith("RISCO")]
    if risks or len(r.get("components",[]))>1 or r["room"]=="E03_Terreo":
        print(r["room"],"componentes",len(r.get("components",[])),"riscos",len(risks))
        for a in risks: print(" ",a["source"],a["footPixel"])
