import os, json, shutil, sys
sys.path.insert(0,'/home/claude/work')
import lpc, cast, spec
from PIL import Image, ImageDraw
import numpy as np
OUT='/mnt/user-data/outputs/DroidsCode/Assets/Characters'
shutil.rmtree('/mnt/user-data/outputs/DroidsCode',ignore_errors=True)
os.makedirs(OUT+'/Documentation',exist_ok=True)
CORE=['walk','idle','hurt','slash','thrust','spellcast','shoot','run','combat_idle']
ROWS={'hurt':1,'climb':1}
def rel_credits(ch):
    seen={}
    for s in ch.items:
        iid=s['id']; layer=lpc.LAY[iid]['layer_1']; base=layer.get(ch.body_type,'')
        base=base.replace('${head}','').rstrip('/')
        ents=lpc.CRED.get(iid,[])
        sel=[e for e in ents if base.startswith(e['file']) or e['file'].startswith(base[:base.rfind('/')] if '/' in base else base)] or ents
        for e in sel:
            key=(e['file'],tuple(e['authors']),tuple(e['licenses']))
            seen[key]=(iid,e)
    return list(seen.values())
report=[]; sprites={}; allcred={}
for k,c in cast.CAST.items():
    sp=spec.SPEC[k]; ch=lpc.Char(k,c['body'],c['skin'],c['items'])
    covx=ch.coverage(); sheet,miss=ch.render(covx)
    if miss:
        bad={m[1] for m in miss}; print(k,'arquivo ausente em',bad,[m[0] for m in miss][:2]); covx=[a for a in covx if a not in bad]; sheet,miss=ch.render(covx)
    assert not miss
    d=f"{OUT}/{c['folder']}"; os.makedirs(d+'/animations',exist_ok=True)
    sheet.save(d+'/spritesheet.png')
    cov=covx
    bodyfolder={'child':'child'}.get(c['body'],c['body'])
    for a in cov:
        folder={'combat_idle':'combat_idle'}.get(a,a)
        bp=f"{lpc.ROOT}/spritesheets/body/bodies/{bodyfolder}/{folder}.png"
        if not os.path.exists(bp): continue
        w,h=Image.open(bp).size
        sheet.crop((0,lpc.OFFSETS[a],w,lpc.OFFSETS[a]+h)).save(f"{d}/animations/{a}.png")
    if 'idle' in cov: sf,src=lpc.frame(sheet,'idle','down',0),'idle (baixo, quadro 0)'
    else: sf,src=lpc.frame(sheet,'walk','down',0),'walk (baixo, quadro 0 = pose parada)'
    sf.save(d+'/sprite.png'); sprites[k]=sf
    bb=sf.getbbox(); stand=(bb[3]-bb[1]) if bb else 0
    # checks
    arr=np.array(sheet); mag=((arr[...,0]==255)&(arr[...,1]==44)&(arr[...,2]==230)&(arr[...,3]>0)).sum()
    empty=[a for a in cov if lpc.frame(sheet,a,'down',0).getbbox() is None]
    report.append((k,c,sheet.size,sf.size,stand,cov,int(mag),empty,src,ch))
    json.dump({'personagem':c['display'],'body_type':c['body'],'skin':c['skin'],'itens':c['items'],'animacoes_cobertas':cov,'url_gerador_nao_testada':ch.url(),'repositorio_lpc_commit':'9bf06be193b053db256b2536824af426de046aa2'},open(d+'/character.json','w'),ensure_ascii=False,indent=1)
    L=[f"# {c['display']}",'',f"- **Categoria:** {sp['tier']}",f"- **Idade:** {sp['idade']}",f"- **Função narrativa:** {sp['funcao']}",f"- **Onde aparece:** {sp['local']}",f"- **Tipo de corpo / tom de pele (LPC):** {c['body']} / {c['skin']}",f"- **sprite.png:** {src}",'','## Correspondência com o enredo','','| Característica | Origem | Peça selecionada | Status | Justificativa |','|---|---|---|---|---|']
    L+=[f"| {a} | {b} | {p} | {s_} | {j} |" for a,b,p,s_,j in sp['rows']]
    L+=['','## Peças LPC usadas (ordem de montagem)','','| Item (id) | Nome | Cor/variante |','|---|---|---|']
    for s in c['items']:
        m=lpc.IT[s['id']]; col=s.get('variant') or ', '.join(f"{kk}={vv}" for kk,vv in s.get('colors',{}).items()) or ('pele '+c['skin'] if m['matchBodyColor'] else '—')
        L.append(f"| `{s['id']}` | {m['name']} | {col} |")
    L+=['','## Animações disponíveis (todas as peças têm arte)','',', '.join(cov),'','Animações ausentes não foram renderizadas: '+(', '.join(a for a in lpc.OFFSETS if a not in cov) or 'nenhuma')+'.','']
    if sp['pend']: L+=['## Pendências','']+[f'- {p}' for p in sp['pend']]+['']
    L+=['## Reabrir no gerador','',f"Link montado a partir do código do repositório (**não testado no navegador**): {ch.url()}",'','## Créditos e licenças','']
    cr=rel_credits(ch)
    for iid,e in cr:
        L.append(f"- `{e['file']}` — {', '.join(e['authors'])} — {', '.join(e['licenses'])}")
        allcred.setdefault(c['display'],[]).append((e['file'],e['authors'],e['licenses'],e['urls']))
    open(d+'/character-config.md','w').write('\n'.join(L)+'\n')
# visão geral
names=list(sprites); cols=6; rows=(len(names)+cols-1)//cols
W=Image.new('RGBA',(192*cols,208*rows),(58,60,66,255)); dr=ImageDraw.Draw(W)
for i,n in enumerate(names):
    x,y=(i%cols)*192,(i//cols)*208
    W.alpha_composite(sprites[n].resize((192,192),0),(x,y+14)); dr.text((x+4,y+2),n,fill=(230,230,230,255))
W.save(OUT+'/Documentation/cast-overview.png')
# créditos
CR=['# Créditos e licenças','','Fonte: Universal LPC Spritesheet Character Generator (repositório LiberatedPixelCup, commit 9bf06be, 14/10/2026). Todo autor listado deve ser creditado no jogo.','','**Atenção:** as peças misturam OGA-BY 3.0, CC-BY-SA 3.0, CC-BY 3.0/4.0 e GPL 2.0/3.0. A licença mais restritiva (CC-BY-SA) exige compartilhamento sob a mesma licença para obras derivadas. O README do repositório recomenda só OGA-BY/CC0 para lojas com DRM (Steam, App Store). Confirme a compatibilidade antes de distribuir; isto não é aconselhamento jurídico.','']
for n,ents in allcred.items():
    CR+=[f'## {n}','']
    for f,a,l,u in dict.fromkeys((f,tuple(a),tuple(l),tuple(u)) for f,a,l,u in ents):
        CR.append(f"- `{f}` — {', '.join(a)} — {', '.join(l)} — {'; '.join(u)}")
    CR.append('')
open(OUT+'/Documentation/CREDITS.md','w').write('\n'.join(CR))
# mapeamento
M=['# Mapeamento enredo → peças do gerador','','Status: CANÔNICO (está no enredo), APROXIMAÇÃO (peça mais próxima), SUGESTÃO (decisão de design, não canônica), PENDENTE (sem peça).','','| Personagem | Característica | Origem | Peça selecionada | Status | Justificativa |','|---|---|---|---|---|---|']
for k,c in cast.CAST.items():
    for a,b,p,s_,j in spec.SPEC[k]['rows']: M.append(f"| {c['display']} | {a} | {b} | {p} | {s_} | {j} |")
open(OUT+'/Documentation/sprite-mapping.md','w').write('\n'.join(M)+'\n')
# cobertura
V=['# Cobertura de animações e verificações','',f"Sheet padrão LPC: 832×3456 px, quadros 64×64, linhas up/left/down/right. Antes de tudo: nada foi comparado pixel a pixel com a exportação do site (inacessível aqui); a montagem usa um compositor próprio que replica o renderer do repositório.",'','| Personagem | '+' | '.join(CORE)+' | extras | sheet | sprite | altura parado (px) | magenta restante | quadros vazios |','|'+'---|'*(len(CORE)+8)]
for k,c,ss,sz,st,cov,mag,emp,src,ch in report:
    ex=[a for a in cov if a not in CORE]
    V.append(f"| {c['display']} | "+' | '.join('✔' if a in cov else '—' for a in CORE)+f" | {', '.join(ex) or '—'} | {ss[0]}×{ss[1]} | {sz[0]}×{sz[1]} | {st} | {mag} | {', '.join(emp) or 'nenhum'} |")
V+=['','Não foi criada nenhuma animação nova; animações marcadas com — não existem para a combinação de peças escolhida. Onde falta `idle`, a pose parada é o quadro 0 de `walk`. Para combate por turnos use `combat_idle` quando houver, senão `idle`/`walk`.','','Direções: apenas `hurt` e `climb` têm uma única direção.']
open(OUT+'/Documentation/animation-coverage.md','w').write('\n'.join(V)+'\n')
# catálogo
G=[('Protagonista',['lip','lip_mochila']),('Companheiro',['apollo']),('Antagonistas',['ivo','nadir','carregador']),('Facções e cidade',['rasha','teodoro','dara','tacio','bruna','nilce','quirino','sula','pipo','onofre','homem_fila']),('Referência',['celia'])]
K=['# Catálogo de personagens — Droids Code (Ato 1)','','Fonte: TCC_-_ENREDO_v2.md e Adendo. Gerador: Universal LPC (repositório local, commit 9bf06be).','','![visão geral](cast-overview.png)','','## A. Lista de personagens','']
for g,ks in G:
    K.append(f'**{g}**'); 
    for k in ks: K.append(f"- {cast.CAST[k]['display']} — {spec.SPEC[k]['funcao']}")
    K.append('')
K+=['**Não montados**']+[f'- {a}: {b}' for a,b in spec.NOT_BUILT]
K+=['','## B. Correspondência visual','','Tabela completa em `sprite-mapping.md`; cada personagem tem seu `character-config.md`.','','## C. Arquivos gerados','','Por personagem: `sprite.png`, `spritesheet.png`, `animations/*.png`, `character.json`, `character-config.md`.','']
for k,c in cast.CAST.items(): K.append(f"- `Assets/Characters/{c['folder']}/`")
K+=['- `Documentation/`: `character-catalog.md`, `sprite-mapping.md`, `animation-coverage.md`, `CREDITS.md`, `cast-overview.png`','','## D. Pendências','','- Props e emblemas sem peça: balde, caderneta, martelo, chave inglesa, contador Geiger, flor da Flor de Ferro, broche da Lótus.','- Apollo é placeholder (sem alto-falante, perna assimétrica, braço de carga).','- Inimigos e droids não foram montados (fora do escopo do gerador).','- Pele, cabelo e roupa da maioria dos personagens são sugestões; revise-as.','- Links do gerador não foram testados.','','## E. Próximas etapas','','1. Revisar as sugestões de design.','2. Importar `spritesheet.png` (grade 64×64) no motor, sem filtro.','3. Decidir o uso de `combat_idle`/`idle` nos combates.','4. Produzir camadas custom (emblemas, props) e as sprites finais do Apollo e dos inimigos.','5. Revisar licenças antes de distribuir.']
open(OUT+'/Documentation/character-catalog.md','w').write('\n'.join(K)+'\n')
os.makedirs(OUT+'/Documentation/tools',exist_ok=True)
for f in ['lpc.py','cast.py','spec.py','build.py']: shutil.copy('/home/claude/work/'+f,OUT+'/Documentation/tools/'+f)
print('\n'.join(V[4:]))
