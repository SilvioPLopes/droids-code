import fs from 'node:fs/promises';
import path from 'node:path';
import { pathToFileURL } from 'node:url';
import sharp from 'sharp';
import { Presentation, PresentationFile } from '@oai/artifact-tool';

const root='C:/Users/silve/DroidsCode';
const work=path.join(root,'output/apresentacao-telas');
const build=path.join(work,'build');
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
const candidate=path.join(build,'candidate.pptx');
await(await PresentationFile.exportPptx(p)).save(candidate);
const {finalizePresentation}=await import(pathToFileURL(path.join(skill,'container_tools/artifact_tool_utils.mjs')).href);
const result=await finalizePresentation({workspaceDir:work,candidatePath:candidate,finalPath:path.join(work,'entrega/DroidsCode-telas-e-menus.pptx'),pythonExecutable:path.join(runtime,'python/python.exe'),integrityValidatorPath:path.join(skill,'container_tools/inspect_presentation_package_integrity.py'),layoutValidatorPath:path.join(skill,'container_tools/inspect_presentation_layout_geometry.py'),layoutArgs:['--expected-slide-size-emu','12192000,6858000','--validate-bullet-geometry','--validate-heading-fit'],requiredNativeTableOwnerSlides:[],requiredNativeChartOwnerSlides:[],fontPolicy:{basis:'design',families:[font]},verifyArtifactToolImport:true,receiptPath:path.join(build,'validation.json')});
console.log(JSON.stringify(result));
