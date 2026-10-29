from pathlib import Path
import json, sys, shutil, hashlib, math
ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / 'output/ferramentas/lpc-portatil'))
from PIL import Image
OUT = Path(__file__).parent
base = json.loads((OUT / 'ambientes-base.json').read_text(encoding='utf-8'))
inventory = json.loads((ROOT / 'output/moradores-ferrovale/2026-10-28/inventario-elenco.json').read_text(encoding='utf-8'))
proposal = json.loads((ROOT / 'output/moradores-ferrovale/2026-10-28/proposta-ocupacao.json').read_text(encoding='utf-8'))
named = {n['id']: n for n in inventory['npcs']}
folders = {'dara':'NPCs/Dara','tacio':'NPCs/Tacio','homem_fila':'NPCs/Homem_da_Fila','mestra_guilda':'NPCs/Mestra_Bruna','estalajadeira':'NPCs/Nilce','ferreiro':'NPCs/Quirino','vendedora_pecas':'NPCs/Sula','crianca_rua':'NPCs/Pipo','velho_viaduto':'NPCs/Seu_Onofre','nadir':'Antagonists/Nadir_Quintela','benta':'NPCs/Benta','zeca_cerca':'NPCs/Zeca','fv_crianca_sula_02':'NPCs/Crianca_Sula_A','fv_crianca_sula_03':'NPCs/Crianca_Sula_B'}
for g in proposal['genericResidents']: folders[g['id']] = 'NPCs/Generic/' + g['recipe']
actor_list = []
for id, folder in folders.items():
    source = ROOT / 'Assets/sprites/Entities/Characters' / folder
    walk = source / 'spritesheet_walk.png'
    if not walk.exists(): walk = source / 'animations/walk.png'
    image = Image.open(walk).convert('RGBA'); assert image.size == (576,256)
    target = ROOT / 'Assets/sprites/Entities/Characters/RuntimeFerrovale' / id
    target.mkdir(parents=True, exist_ok=True)
    for row, direction in enumerate(['cima','esquerda','baixo','direita']): image.crop((0,row*64,64,row*64+64)).save(target/(direction+'.png'))
    origin = {'source': walk.relative_to(ROOT).as_posix(), 'frame':0, 'frameSize':[64,64], 'method':'crop_existing_LPC_walk_no_original_overwrite'}
    (target/'origem.json').write_text(json.dumps(origin,indent=2)+'\n',encoding='utf-8')
    name = named.get(id,{}).get('Nome') or ('Crianca' if 'crianca' in id else id.replace('fv_','').replace('_01','').replace('_',' ').capitalize())
    actor_list.append({'id':id,'name':name,'poses':target.relative_to(ROOT).as_posix(),'prefab':'Assets/Prefabs/NPCs/Ferrovale/'+id+'.prefab','shop':id in ['tacio','vendedora_pecas','ferreiro','benta'],'greeting':named.get(id,{}).get('TextoDeBoasVindas') or 'A cidade continua de pe. Cada um cuida de um pedaco.'})

# Pegadas conservadoras dos moveis; paredes sao limites separados do fundo.
boxes = {
'E06_Terreo':[[130,300,400,790],[370,430,660,660],[680,240,940,470]],
'E06_Superior':[[110,220,270,570],[450,190,820,400],[350,500,820,740],[1180,390,1340,810]],
'E06_Anexo':[[150,190,1190,375],[110,420,300,780],[1200,430,1350,850]],
'E05_Principal':[[650,230,1040,420],[200,420,405,660],[430,430,690,620],[1050,210,1300,400]],
'E10_Casa04':[[1050,360,1270,800],[775,420,1030,650],[220,260,500,440],[550,190,950,325]],
'E11_Casa05':[[155,320,390,780],[385,440,670,690],[160,180,930,340],[1010,240,1270,420]],
'E11_Casa06':[[260,530,760,790],[400,360,650,560],[690,210,980,440],[1040,210,1280,375]],
'E09_Terreo':[[210,320,700,640],[270,720,600,895],[680,190,1030,350]],
'E09_Superior':[[180,260,650,650],[240,740,670,890],[1050,260,1270,500]],
'E08_Principal':[[700,210,1050,390],[210,400,680,730],[680,460,1040,700]],
'E08_Superior':[[370,180,740,400],[850,190,1090,375],[320,500,1020,750]],
'E00_Hall':[[220,210,600,370],[120,400,310,740]],
'E01_Hall':[[230,200,760,370],[110,460,310,710]],
'E13_Hall':[[110,410,600,680],[110,200,470,390]],
'E02_Principal':[[220,260,800,630],[1050,200,1300,390],[870,620,1250,855]],
'E02_Garagem':[[200,180,1170,380],[110,400,350,800],[1200,370,1350,850]],
'E03_Terreo':[[500,260,940,480],[190,520,720,830],[780,520,1260,825]],
'E03_Superior':[[450,190,970,350],[450,400,1030,630],[220,690,1040,890]],
'E14_UnidadeA':[[350,270,970,700],[100,250,280,760],[1080,310,1320,540]],
'E14_Porao':[[110,200,330,710],[230,170,1170,340],[1160,250,1330,850],[1030,350,1240,520]],
'E14_UnidadeB':[[200,360,710,720],[620,200,970,400],[1040,210,1300,400]],
}
for key in ['E00_Apartamento01']:boxes[key]=[[650,220,1100,430],[410,440,780,690],[1070,680,1300,890],[110,340,260,700]]
for key in ['E04_UnidadeA','E01_Apartamento01']:boxes[key]=[[1040,360,1270,780],[770,420,1030,660],[210,220,510,445],[600,185,1000,340]]
for key in ['E04_UnidadeB','E13_Apartamento01']:boxes[key]=[[530,230,970,480],[1050,195,1290,380],[220,740,690,880],[290,510,590,700]]
for key in ['E00_Patamar','E01_Patamar','E13_Patamar']:boxes[key]=[[110,650,290,880],[675,85,965,400]]
boxes['E16_Civico']=[[580,465,860,685],[530,195,930,330]]

actors_by_room = {
'E06_Terreo':[('fv_mecanico_01',1000,730)],'E06_Anexo':[('fv_mecanico_01',850,650)],
'E05_Principal':[('vendedora_pecas',1030,650),('crianca_rua',800,740),('fv_crianca_sula_02',1040,800),('fv_crianca_sula_03',450,800)],
'E10_Casa03':[('fv_operario_01',945,790)],'E10_Casa04':[('homem_fila',480,700)],
'E11_Casa05':[('tacio',1000,700)],'E11_Casa06':[('fv_idoso_01',1000,680)],
'E09_Terreo':[('mestra_guilda',720,650),('fv_operario_01',450,690)],'E08_Principal':[('estalajadeira',1120,740)],'E08_Superior':[('zeca_cerca',1100,750)],
'E00_Apartamento01':[('fv_vendedor_01',1050,520)],'E01_Apartamento01':[('fv_hacker_01',450,680)],
'E02_Principal':[('ferreiro',850,450)],'E02_Garagem':[('ferreiro',930,650)],
'E04_UnidadeA':[('dara',510,700)],'E04_UnidadeB':[('fv_vizinha_dara_01',1030,680)],
'E13_Apartamento01':[('fv_cientista_01',1030,690)],'E14_UnidadeA':[('benta',1080,730)],'E14_UnidadeB':[('fv_guarda_01',1080,700)]}

rooms=[]; copied={}
for r in base['rooms']:
    if r['id']=='E12_CasaLip':continue
    src=ROOT/r['art']; sha=hashlib.sha256(src.read_bytes()).hexdigest()
    if r['id']=='E10_Casa03':asset='Assets/sprites/Mapas/Interiores/Casa03.png'
    elif sha in copied:asset=copied[sha]
    else:
        asset='Assets/sprites/Mapas/Interiores/'+src.name
        dst=ROOT/asset
        if not dst.exists():shutil.copy2(src,dst)
    copied[sha]=asset
    rr={'id':r['id'],'scene':'Assets/Scenes/Interiores/'+r['sceneNameProposal']+'.unity','sceneName':r['sceneNameProposal'],'prefab':'Assets/Prefabs/Interiores/'+r['id']+'.prefab','art':asset,'sourceArt':r['art'],'boxes':boxes.get(r['id'],[]),'actors':[{'id':id,'x':x,'y':y} for id,x,y in actors_by_room.get(r['id'],[])],'portals':[]}
    rr['outline']=[[95,230],[1350,230],[1350,920],[95,920]]
    if r['id']=='E09_Terreo':rr['outline']=[[95,230],[1350,230],[1350,450],[825,450],[825,920],[95,920]]
    if r['id']=='E09_Superior':rr['outline']=[[95,230],[1350,230],[1350,600],[825,600],[825,920],[95,920]]
    if r['id']=='E16_Civico':rr['outline']=[[724+610*math.cos(a*math.pi/16),555+400*math.sin(a*math.pi/16)] for a in range(32)]
    rooms.append(rr)
byid={r['id']:r for r in rooms}
def connect(a,pa,b,pb,kind='escada'):
    byid[a]['portals'].append({'key':kind+'_'+b,'px':pa[0],'py':pa[1],'targetRoom':b,'targetFootPixel':[pb[0],pb[1]+145]})
    byid[b]['portals'].append({'key':kind+'_'+a,'px':pb[0],'py':pb[1],'targetRoom':a,'targetFootPixel':[pa[0],pa[1]+145]})
connect('E06_Terreo',(1250,350),'E06_Superior',(1250,350))
connect('E09_Terreo',(1250,310),'E09_Superior',(780,300))
connect('E08_Principal',(1250,350),'E08_Superior',(1250,350))
connect('E03_Terreo',(1250,315),'E03_Superior',(1250,315))
for b in ['E00','E01','E13']:
    connect(b+'_Hall',(1250,355),b+'_Patamar',(1135,390))
    byid[b+'_Patamar']['portals'].append({'key':'apartamento','px':350,'py':340,'targetRoom':b+'_Apartamento01','targetFootPixel':[724,760]})
    byid[b+'_Apartamento01']['portals'].append({'key':'retorno_patamar','px':724,'py':895,'targetRoom':b+'_Patamar','targetFootPixel':[350,485]})

city_before=json.loads((OUT/'portas-city-anterior.json').read_text());old={d['name']:d for d in city_before['doors']};city=[]
for d in base['cityDoors']:
    if d['cityDoor']=='Entrada_CasaLip':continue
    room=byid[d['room']];before=old[d['cityDoor']];x=before['x'];y=before['y']+.62
    if d['cityDoor']=='Porta_Casa03_Provisoria':x=2.56479287;y=-8.663213+.62
    # A prefab atual guarda referencias coletadas do Transform do Player; agora centralizar nos pes.
    entryx=724
    if d['cityDoor']=='Porta_PredioPlaca_A':entryx=510
    if d['cityDoor']=='Porta_PredioPlaca_B':entryx=875
    py=895 if d['room']!='E14_Porao' else 795
    spawn=[entryx,py-145]
    target={'key':d['cityDoor'],'x':x,'y':y,'targetRoom':room['id'],'targetFootPixel':spawn,'previousX':before['x'],'previousY':before['y'],'returnX':x,'returnY':y-1.32}
    city.append(target)
    room['portals'].append({'key':'saida_'+d['cityDoor'],'px':entryx,'py':py,'targetScene':'City','targetX':x,'targetY':y-1.32})

# Segundo acesso reconhecido da Casa2, que compartilha o mesmo interior.
e05=next(d for d in city if d['key']=='Porta_Casa02')
left={**e05,'key':'Porta_Casa02_Esquerda','x':e05['x']-1.9,'returnX':e05['x']-1.9,'targetFootPixel':[380,750],'previousX':None,'previousY':None}
city.append(left)
byid['E05_Principal']['portals'].append({'key':'saida_Porta_Casa02_Esquerda','px':380,'py':895,'targetScene':'City','targetX':left['x'],'targetY':left['returnY']})
byid['E06_Anexo']['portals'].append({'key':'voltar_casa','px':724,'py':895,'targetRoom':'E06_Terreo','targetFootPixel':[1000,800]})
data={'rooms':rooms,'actors':actor_list,'cityDoors':city,'approvedArtworkOnly':True,'heldCandidate':'output/montagem-ferrovale/2026-10-29/estalagem-corredor-seis-portas.png','playerFootOffset':.62,'actorScale':1.4}
(OUT/'config-montagem.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(json.dumps({'newRoomTargets':len(rooms),'actorPrefabs':len(actor_list),'cityTargets':len(city),'originalCasaLipPreserved':True,'newCandidateImported':False}))
