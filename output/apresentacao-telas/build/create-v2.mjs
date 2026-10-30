import fs from 'node:fs/promises';
import path from 'node:path';
import { pathToFileURL } from 'node:url';
import sharp from 'sharp';
import { Presentation, PresentationFile } from '@oai/artifact-tool';

const root='C:/Users/silve/DroidsCode';
const work=path.join(root,'output/apresentacao-telas');
const build=path.join(work,'build/v2');
const skill='C:/Users/silve/.codex/plugins/cache/openai-primary-runtime/presentations/26.1022.11436/skills/presentations';
const runtime='C:/Users/silve/.cache/codex-runtimes/codex-primary-runtime/dependencies';
process.env.RUNTIME_NODE_MODULES=path.join(runtime,'node/node_modules');
const p=Presentation.create({slideSize:{width:1280,height:720}});
const font='Segoe UI';
const bg='#101318', ivory='#F5E8CE', copper='#D5A263', secondary='#B8B4AC';
function txt(s,text,x,y,w,h,size,color=ivory,bold=false){
  const o=s.shapes.add({geometry:'textbox',name:text.split('\n')[0],position:{left:x,top:y,width:w,height:h},fill:'none',line:{fill:'none',width:0,style:'solid'}});
  o.text=text;o.text.style={typeface:font,fontSize:size,color,bold};return o;
}
const screens=[
 {title:'Menu do mundo',caption:'Todos os painéis a um clique.',file:'output/menu-mundo-ilustrado/preview-1280x720.png',crop:{left:305,top:42,width:682,height:623},portrait:true,notes:'Oito opções visíveis. A mensagem da captura informa que Opções ainda não foi implementado. Salvar/Carregar não foram executados para esta apresentação.'},
 {title:'Bag',caption:'Itens, quantidades e detalhes de uso.',file:'output/bag-ilustrada/preview-1280x720.png',crop:{left:118,top:7,width:1047,height:699},notes:'Inventário com grade paginada e detalhe do item selecionado. A captura existente contém dados de fixture, não o progresso atual do jogador.'},
 {title:'Loja',caption:'Comprar e vender com os detalhes à vista.',file:'output/loja-ilustrada/preview-1280x720.png',crop:{left:106,top:4,width:1067,height:711},notes:'Prévia existente com preço, saldo, produtos e mensagem de compra. Não foi feita nenhuma transação durante a criação da apresentação.'},
 {title:'Status',caption:'Vida, energia, atributos e técnicas.',file:'output/status-ilustrado/preview-1280x720.png',crop:{left:118,top:4,width:1047,height:700},notes:'Valores e técnica de teste pertencem à fixture da captura. Não representam balanceamento final. Input real e aprovação visual continuam separados das verificações por API documentadas no projeto.'},
 {title:'Apollo · Droid',caption:'Peças instaladas e técnicas do Droid.',file:'output/equipamentos-apollo/preview-1280x720.png',crop:{left:133,top:10,width:1018,height:687},notes:'Quatro slots de equipamentos e consulta de estoque/técnicas. Instalação e remoção são feitas pelo terminal, como informa o rodapé da própria tela. Dados de fixture; não retratar como estado do jogador.'},
 {title:'Terminal',caption:'Escreva comandos.\nExecute e acompanhe a saída.',file:'output/terminal-ilustrado/recorte-2026-10-23/preview-1280x720.png',crop:{left:286,top:6,width:710,height:708},portrait:true,notes:'Prévia existente do terminal ilustrado, com editor de código, objetivo e saída. A saída contém conteúdo de teste já presente na imagem; nenhum comando foi executado para produzir o deck.'},
];
await fs.mkdir(path.join(build,'previews'),{recursive:true});
for (const [i,d] of screens.entries()){
 const s=p.slides.add();s.background.fill=bg;
 const bytes=await sharp(path.join(root,d.file)).extract(d.crop).png().toBuffer();
 const blob=bytes.buffer.slice(bytes.byteOffset,bytes.byteOffset+bytes.byteLength);
 if(d.portrait){
  txt(s,i===0?'DroidsCode':'DroidsCode / Terminal',64,68,420,40,23,copper,true);
  txt(s,i===0?'Telas e\nmenus':'Terminal',64,190,430,174,i===0?64:58,ivory,true);
  txt(s,i===0?'Menu do mundo':d.caption,66,i===0?400:390,380,100,i===0?32:27,i===0?copper:secondary);
  if(i===0)txt(s,d.caption,66,462,375,80,25,secondary);
  s.images.add({blob,contentType:'image/png',alt:d.title,fit:'contain',position:{left:502,top:40,width:718,height:640}});
 }else{
  txt(s,d.title,64,24,1000,64,46,ivory,true);
  txt(s,d.caption,66,88,1100,37,25,secondary);
  s.images.add({blob,contentType:'image/png',alt:d.title,fit:'contain',position:{left:80,top:137,width:1120,height:558}});
 }
 s.speakerNotes.text=`Fonte local: ${d.file}. Prévia de montagem existente no projeto, coletada antes desta apresentação; não é uma nova captura de uma sessão de jogo. Foram recortadas apenas margens externas sem controles, mantendo proporção. ${d.notes}`;
 const png=await p.export({slide:s,format:'png',scale:1});
 await fs.writeFile(path.join(build,'previews',`slide-${i+1}.png`),new Uint8Array(await png.arrayBuffer()));
 console.log(`Renderizado ${i+1}: ${d.title}`);
}

const imageDir=path.join(work,'imagens-mapas-npcs');
await fs.mkdir(imageDir,{recursive:true});
const maps=[
 {title:'Casa de Lip',caption:'O interior da casa e seu terminal.',file:'Assets/sprites/Mapas/LipCasa.jpg',out:'01-casa-de-lip.jpg',portrait:true},
 {title:'Cidade',caption:'Ruas e construções vistas de cima.',file:'Assets/sprites/Mapas/metavalle.jpg',out:'02-cidade.jpg'},
 {title:'Vale',caption:'Ruínas, sucata e caminhos de exploração.',file:'Assets/sprites/Mapas/ferrovale.jpg',out:'03-vale.jpg'},
 {title:'Cenário de batalha',caption:'O ambiente visual dos combates.',file:'Assets/sprites/Mapas/battle.jpg',out:'04-cenario-de-batalha.jpg'},
];
async function pic(s,file,position){
 const bytes=await fs.readFile(file);const blob=bytes.buffer.slice(bytes.byteOffset,bytes.byteOffset+bytes.byteLength);
 s.images.add({blob,contentType:file.endsWith('.jpg')?'image/jpeg':'image/png',alt:path.basename(file),fit:'contain',position});
}
for(const d of maps){
 const s=p.slides.add();s.background.fill=bg;
 await fs.copyFile(path.join(root,d.file),path.join(imageDir,d.out));
 if(d.portrait){
  txt(s,'Mapas / Interiores',64,68,420,40,23,copper,true);
  txt(s,d.title,64,190,450,90,54,ivory,true);
  txt(s,d.caption,66,330,400,100,27,secondary);
  await pic(s,path.join(root,d.file),{left:568,top:25,width:620,height:670});
 }else{
  txt(s,d.title,64,24,1000,64,46,ivory,true);txt(s,d.caption,66,88,1100,37,25,secondary);
  await pic(s,path.join(root,d.file),{left:60,top:139,width:1160,height:558});
 }
 s.speakerNotes.text='Fonte: '+d.file+'. Arte de fundo original usada na cena correspondente. Não é captura de gameplay e não inclui atores, colisões ou interface. Imagem inteira, proporção preservada. Referências verificadas em Assets/Scenes e no contrato da City.';
}
{
 const s=p.slides.add();s.background.fill=bg;
 txt(s,'NPCs da cidade',64,24,1100,64,46,ivory,true);
 txt(s,'Dara, Homem da fila e Senhor Tácio.',66,88,1100,40,25,secondary);
 const npcs=[['Dara','05-dara.png',155],['Homem da fila','06-homem-da-fila.png',535],['Senhor Tácio','07-senhor-tacio.png',915]];
 for(const [name,out,x] of npcs){
  const source=path.join(root,'Assets/Resources/Retratos',name+'.png');
  const dest=path.join(imageDir,out);
  await sharp(source).resize({height:352,kernel:'nearest'}).png().toFile(dest);
  await pic(s,dest,{left:x,top:190,width:210,height:352});
  txt(s,name,x-35,585,310,56,30,copper,true);
 }
 s.speakerNotes.text='Fontes: Assets/Resources/Retratos/Dara.png, Homem da fila.png, Senhor Tácio.png. São retratos originais de diálogo ampliados por vizinho mais próximo para preservar os pixels, sem redesenho. Contrato docs/contextos/cenas/city.md identifica os três NPCs montados; Tácio usa a folha de Ivo na cena. Não há novos personagens implementados nesta apresentação.';
}
{
 const s=p.slides.add();s.background.fill=bg;
 txt(s,'Tácio no mapa',64,24,1100,64,46,ivory,true);
 txt(s,'Personagem e identificação no cenário.',66,88,1100,40,25,secondary);
 const source=path.join(root,'output/npcs-city/nomes-escala/preview-tacio-1920.png');
 await pic(s,source,{left:60,top:139,width:1160,height:558});
 await fs.copyFile(source,path.join(imageDir,'08-tacio-no-mapa-previa.png'));
 s.speakerNotes.text='Fonte: output/npcs-city/nomes-escala/preview-tacio-1920.png. Prévia existente do enquadramento de Tácio na City, criada em tarefa anterior. Não é uma nova captura de jogo e não comprova o estado atual de uma sessão do usuário.';
}
await fs.writeFile(path.join(imageDir,'LEIA-ME.txt'),'DroidsCode - imagens do projeto\n\n01 a 04: artes de fundo dos mapas (sem atores ou interface).\n05 a 07: retratos originais dos NPCs ampliados por vizinho mais próximo.\n08: prévia existente de Tácio no mapa, sem nova captura do Unity.\n\nArquivos originais do projeto preservados.\n','utf8');

const candidate=path.join(build,'candidate.pptx');
await(await PresentationFile.exportPptx(p)).save(candidate);
const {finalizePresentation}=await import(pathToFileURL(path.join(skill,'container_tools/artifact_tool_utils.mjs')).href);
const result=await finalizePresentation({workspaceDir:work,candidatePath:candidate,finalPath:path.join(work,'entrega/DroidsCode-telas-mapas-e-npcs.pptx'),pythonExecutable:path.join(runtime,'python/python.exe'),integrityValidatorPath:path.join(skill,'container_tools/inspect_presentation_package_integrity.py'),layoutValidatorPath:path.join(skill,'container_tools/inspect_presentation_layout_geometry.py'),layoutArgs:['--expected-slide-size-emu','12192000,6858000','--validate-bullet-geometry','--validate-heading-fit'],requiredNativeTableOwnerSlides:[],requiredNativeChartOwnerSlides:[],fontPolicy:{basis:'design',families:[font]},verifyArtifactToolImport:true,receiptPath:path.join(build,'validation.json')});
console.log(JSON.stringify(result));
