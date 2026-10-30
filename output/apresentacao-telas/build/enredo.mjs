import fs from 'node:fs/promises';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
import sharp from 'sharp';
import {Presentation,PresentationFile} from '@oai/artifact-tool';
const root='C:/Users/silve/DroidsCode';
const work=path.join(root,'output/apresentacao-telas');
const build=path.join(work,'build/enredo');
const skill='C:/Users/silve/.codex/plugins/cache/openai-primary-runtime/presentations/26.1022.11436/skills/presentations';
const runtime='C:/Users/silve/.cache/codex-runtimes/codex-primary-runtime/dependencies';
process.env.RUNTIME_NODE_MODULES=path.join(runtime,'node/node_modules');
await fs.mkdir(path.join(build,'previews'),{recursive:true});
const p=Presentation.create({slideSize:{width:1280,height:720}});
const c={bg:'#101318',cream:'#F5E8CE',copper:'#D5A263',muted:'#B8B4AC'};
const enredo='docs/enredo/TCC_-_ENREDO_v2.md', adendo='docs/enredo/TCC_-_ENREDO_v2_ADENDO.md';
function t(s,text,x,y,w,h,size=27,color=c.cream,bold=false){
 const z=s.shapes.add({geometry:'textbox',name:text.split('\n')[0],position:{left:x,top:y,width:w,height:h},fill:'none',line:{fill:'none',width:0,style:'solid'}});
 z.text=text;z.text.style={typeface:'Segoe UI',fontSize:size,color,bold};return z;
}
function slide(title,section,notes,assets=[]){
 const s=p.slides.add();s.background.fill=c.bg;
 if(title)t(s,title,64,30,1140,77,46,c.cream,true);
 s.speakerNotes.text=`Fontes narrativas: ${enredo}, ${section}; ${adendo}, que prevalece nas divergências. ${notes}\nFontes visuais: ${assets.join('; ')}. Artes e prévias existentes, sem nova execução do jogo. Esta apresentação resume o roteiro e não atesta a implementação de cada acontecimento.`;
 return s;
}
async function pic(s,file,box,{crop,pixel=false}={}){
 let q=sharp(path.join(root,file));if(crop)q=q.extract(crop);
 if(pixel)q=q.trim().resize({height:Math.round(box.height),kernel:'nearest'});
 const bytes=await q.png().toBuffer();
 s.images.add({blob:bytes.buffer.slice(bytes.byteOffset,bytes.byteOffset+bytes.byteLength),contentType:'image/png',alt:path.basename(file),fit:'contain',position:box});
}
const char='Assets/sprites/Entities/Characters/';
const main='Assets/sprites/Mapas/mainmenu.jpg';
{
 const s=slide(null,'Partes III–V e Ato 1','Apresentação narrativa de Droids Code, focada no Ato 1: A Cota Cortada. O robô na arte do menu é uma referência de atmosfera do projeto, não uma identificação canônica de Apollo.',[main]);
 await pic(s,main,{left:574,top:20,width:680,height:680},{crop:{left:0,top:0,width:870,height:768}});
 t(s,'DROIDS CODE',64,182,510,100,57,c.cream,true);
 t(s,'A Cota Cortada',68,306,470,62,36,c.copper);
 t(s,'Uma história sobre aprender\na consertar — e escolher\no que deve mudar.',68,413,480,150,28,c.muted);
}
{
 const file='Assets/sprites/Mapas/metavalle.jpg';
 const s=slide('2079 · Ferrovale','III.1 e III.5','Cidade de sucata sobre ruínas de concreto. A vida comunitária gira em torno da fila do poço. A Apex controla o abastecimento regional. A imagem é o fundo da City e ilustra a cidade, sem pretender mapear literalmente cada local do roteiro.',[file]);
 t(s,'Uma cidade de sucata. A sobrevivência passa pela água.',66,100,1140,38,26,c.muted);
 await pic(s,file,{left:70,top:155,width:1140,height:540});
}
{
 const lip=char+'Protagonist/sprite.png', apollo=char+'Companion/Apollo/sprite.png';
 const s=slide('Lip e Apollo','IV.1, IV.2; adendo A.2','Lip tem 20 anos. Montou Apollo aos 13 e convive com ele há sete anos. Lip trata o erro como culpa; Apollo quer que ele sobreviva e aprenda. Conforme o adendo, a importância de Apollo nasce da convivência, não de uma origem única. Sprites atuais do projeto servem como referência, sem afirmar equivalência integral à descrição literária.',[lip,apollo]);
 t(s,'Sete anos de convivência\ntransformaram sucata\nem companhia.',66,235,480,150,34);
 await pic(s,lip,{left:610,top:180,width:210,height:310},{pixel:true});
 await pic(s,apollo,{left:930,top:180,width:230,height:310},{pixel:true});
 t(s,'Lip',650,535,180,55,31,c.copper,true);t(s,'Apollo',947,535,250,55,31,c.copper,true);
 t(s,'Conserta máquinas.',600,600,270,45,24,c.muted);t(s,'Ensina a tentar.',920,600,290,45,24,c.muted);
}
{
 const file='Assets/sprites/Mapas/LipCasa.jpg';
 const s=slide(null,'Sessão 1, 1.1–1.3','A primeira tarefa é fazer o gerador funcionar por meio de um laço em Lua. Um fio de água escura volta à torneira; ainda não é potável. A casa guarda o filtro construído por Lip e lembranças de Inês. O enredo liga uma ação técnica pequena à história pessoal do protagonista.',[file]);
 t(s,'Tudo começa\nem casa',64,157,510,165,53,c.cream,true);
 t(s,'Um gerador parado.\nUm laço de código.\nA primeira água volta.',68,380,470,155,29,c.muted);
 await pic(s,file,{left:585,top:25,width:620,height:670});
}
{
 const dara='Assets/Resources/Retratos/Dara.png',fila='Assets/Resources/Retratos/Homem da fila.png';
 const s=slide('A cota cortada','III.5; Sessão 2, 2.1–2.2; IV.6','A cota diária cai de doze para seis litros por pessoa. Dara continua reservando uma terceira parte para o irmão desaparecido, Emílio/Milo. Os moradores trazem versões diferentes dos acontecimentos; investigar exige ouvi-los. Retratos existentes da City.',[dara,fila]);
 t(s,'12 → 6 litros',66,198,550,110,66,c.copper,true);
 t(s,'Por pessoa, por dia.',70,321,460,48,29,c.muted);
 t(s,'Dara espera na fila.\nLip começa a fazer perguntas.',70,439,500,115,31);
 await pic(s,dara,{left:665,top:215,width:190,height:300},{pixel:true});
 await pic(s,fila,{left:965,top:215,width:190,height:300},{pixel:true});
 t(s,'Dara',700,560,170,55,28,c.copper,true);t(s,'Homem da fila',903,560,330,55,28,c.copper,true);
}
{
 const file='output/npcs-city/nomes-escala/preview-tacio-1920.png';
 const s=slide('Uma cidade, várias versões','Sessão 2, 2.2–2.4; IV.9','Tácio vende água e traz seu ponto de vista sobre a crise. Os depoimentos, incluindo os de outros moradores, permitem entender a pressão da Apex e a negociação de Ivo. A imagem mostra uma prévia anterior de Tácio no cenário, sem encenar a conversa do roteiro.',[file]);
 t(s,'Ouvir os moradores é parte da investigação.',66,100,1140,40,26,c.muted);
 await pic(s,file,{left:70,top:155,width:1140,height:540});
}
{
 const file='Assets/sprites/Mapas/ferrovale.jpg';
 const s=slide('A travessia da Cratera','III.6; Sessões 3–5','Lip e Apollo deixam a segurança relativa da cidade e seguem para a Estação 7. A Cratera não é apenas cenário de guerra: o roteiro a revela como vestígio de povoados realocados à força. O asset do vale ilustra a travessia; não é um registro de todos os locais narrativos já montados.',[file]);
 t(s,'Ruínas de cidades. Máquinas hostis. O caminho até a Estação 7.',66,100,1140,40,26,c.muted);
 await pic(s,file,{left:70,top:155,width:1140,height:540});
}
{
 const rasha=char+'NPCs/Rasha/sprite.png',teodoro=char+'NPCs/Teodoro_Sampaio/sprite.png';
 const s=slide('Três caminhos','III.8; Sessão 4 e variações 5–7','Rasha representa a Flor de Ferro: ação direta e interrupção do sistema. Teodoro representa a Lótus Branca: negociação e contenção. Lip também pode seguir como Desgarrado. No roteiro, os módulos vêm após prova de conduta, não como presente automático; o efeito principal da escolha aparece nas consequências da assembleia.',[rasha,teodoro]);
 await pic(s,rasha,{left:180,top:145,width:200,height:240},{pixel:true});
 await pic(s,teodoro,{left:790,top:145,width:200,height:240},{pixel:true});
 t(s,'Flor de Ferro',100,420,500,50,35,c.copper,true);
 t(s,'Rasha propõe enfrentar.',100,485,510,50,27,c.muted);
 t(s,'Lótus Branca',710,420,500,50,35,c.copper,true);
 t(s,'Teodoro propõe negociar.',710,485,510,50,27,c.muted);
 t(s,'Desgarrado · seguir sem facção, assumindo o próprio caminho.',100,615,1110,60,27);
}
{
 const ivo=char+'Antagonists/Ivo_Rangel/sprite.png',battle='Assets/sprites/Mapas/battle.jpg';
 const s=slide('Ivo Rangel','IV.3; Sessão 6, 6.1–6.6; adendo A','O técnico mantém a Estação 7 há dezenove anos e negocia por desespero. A ligação com Célia revela um erro herdado; a crise não nasce de um vilão simples. O combate expõe instruções escritas sem limites. Conforme o adendo, não reproduzir a afirmação antiga de que a Apex busca exclusivamente Apollo. Fundo de batalha é referência visual, não ilustração literal da Estação 7.',[ivo,battle]);
 t(s,'O técnico da estação\ntenta negociar com a Apex.\nSeu desespero vira conflito.',68,130,460,145,29,c.muted);
 await pic(s,ivo,{left:180,top:325,width:210,height:265},{pixel:true});
 await pic(s,battle,{left:560,top:205,width:660,height:430});
}
{
 const quirino=char+'NPCs/Quirino/sprite.png';
 const s=slide('Uma vitória que deixa marcas','Sessão 6, 6.7; Sessão 7, 7a','Spoilers do Ato 1. A água volta. Para impedir a leitura de setores que desconhece, Apollo sobrescreve um arquivo e perde as gravações da voz de Inês, mãe de Lip. Essa perda não é reparada pela vitória. Quirino instala um novo servo na perna de Apollo, em uma cena de alegria que contrasta com o custo pessoal. Ilustração de Quirino disponível no projeto, sem afirmar montagem da cena.',[quirino]);
 t(s,'A água volta.',68,180,630,74,50,c.copper,true);
 t(s,'Apollo perde as gravações\nda voz da mãe de Lip.',70,298,650,112,35);
 t(s,'Quirino conserta a perna do Apollo.\nHá alegria — e há perda.',70,480,680,110,29,c.muted);
 await pic(s,quirino,{left:885,top:220,width:240,height:330},{pixel:true});
}
{
 const nadir=char+'Antagonists/Nadir_Quintela/sprite.png';
 const s=slide('A oferta da Apex','IV.5; Sessão 7, 7b; adendo A.2–A.4','Nadir oferece dezoito litros por pessoa por dia e nova linha de bombeamento. Em troca, pede censo e entrega de unidades autônomas não registradas. Ela é cordial, cumpre as condições e acredita reduzir sofrimento. A busca é burocrática e ampla, ligada aos 431 núcleos; não é perseguição pessoal a um escolhido. A assembleia varia conforme a rota.',[nadir]);
 t(s,'18 litros por pessoa.',68,176,780,88,49,c.copper,true);
 t(s,'Em troca: o censo e a entrega\ndos droids não registrados.',70,306,770,122,35);
 t(s,'Nadir cumpre o que promete.\nO conflito está no preço.',70,506,720,110,29,c.muted);
 await pic(s,nadir,{left:940,top:245,width:200,height:290},{pixel:true});
}
{
 const s=slide('Os três atos','Adendo B.1–B.3 e C.1–C.4','Arquitetura narrativa planejada, não comprovação de implementação. No Ato 2, Apollo é levado pela Apex e Lip enfrenta a ausência da orientação; a revelação do histórico de versões preserva a tese de que a convivência o distingue. No Ato 3, a Torre é armazenamento e o confronto final com Artemis é programação e responsabilidade por regras, sem corpo físico de chefe. Não declarar finais mutuamente exclusivos de sobrevivência: o próprio adendo também prevê Apollo sobrevivendo em Ramificar.',[]);
 const cols=[['ATO 1','Posso consertar?','A água de uma cidade.','O erro é informação.'],['ATO 2','Quem me escreveu?','A origem de Apollo.','O histórico de versões.'],['ATO 3','Quem eu escrevo?','A regra de um sistema.','Leia o meu erro.']];
 cols.forEach((a,i)=>{const x=70+i*408;t(s,a[0],x,193,365,50,27,c.copper,true);t(s,a[1],x,290,365,110,34,c.cream,true);t(s,a[2],x,443,365,90,26,c.muted);t(s,a[3],x,555,365,85,25);});
 t(s,'Arco narrativo planejado',70,657,1100,36,21,c.copper);
}
{
 const file='output/terminal-ilustrado/recorte-2026-10-23/preview-1280x720.png';
 const s=slide(null,'V.2; Sessão 1; adendo B e C','O terminal é a ligação entre ficção e aprendizagem. No Ato 1, Apollo orienta; o jogador testa, recebe retorno e revisa. A progressão posterior pretende retirar o apoio e culminar em uma decisão escrita no terminal. A captura é uma fixture anterior com texto e saída de teste; não encena o desfecho nem comprova sua implementação.',[file]);
 t(s,'Aprender\ntambém é jogar',64,155,510,170,51,c.cream,true);
 t(s,'Escrever. Testar. Rever.',68,387,470,60,31,c.copper);
 t(s,'Apollo ajuda a entender o erro.\nO jogador tenta outra vez.',68,486,470,115,28,c.muted);
 await pic(s,file,{left:555,top:30,width:670,height:660},{crop:{left:286,top:6,width:710,height:708}});
}
{
 const file='output/equipamentos-apollo/preview-1280x720.png';
 const s=slide('Preparar Apollo para seguir','Sessão 7, A perna; sistema atual conforme docs/guias/equipamentos-apollo.md','A relação com Apollo também aparece na inspeção de peças e técnicas. No sistema atual, instalar e remover peças é tarefa do terminal; o menu apenas mostra e consulta. O roteiro da perna é uma cena planejada distinta desse sistema já mostrado. A captura contém dados de fixture, não um save do usuário.',[file]);
 t(s,'Peças e técnicas dão forma ao companheiro de viagem.',66,100,1140,40,26,c.muted);
 await pic(s,file,{left:70,top:155,width:1140,height:540},{crop:{left:133,top:10,width:1018,height:687}});
}
{
 const s=slide(null,'V.1–V.2; adendo A.2 e C.4','Síntese temática: a identidade é construída por convivência e escolhas. Lip começa tratando erro como condenação e a saga pretende levá-lo a assumir a responsabilidade por regras que escreve. O fechamento resume o enredo, sem alegar que a saga inteira já está implementada.',[]);
 t(s,'QUEM ESCREVE QUEM',70,97,1120,70,28,c.copper,true);
 t(s,'O erro é informação',68,223,1140,100,67,c.cream,true);
 t(s,'O que fazemos depois dele\né uma escolha.',72,374,1120,145,49,c.muted);
 t(s,'Droids Code · Enredo e universo',74,624,1100,50,25,c.copper);
}
for(const [i,s] of p.slides.items.entries()){
 if(![9,11,14].includes(i))continue;
 const png=await p.export({slide:s,format:'png',scale:1});
 await fs.writeFile(path.join(build,'previews',`slide-${i+1}.png`),new Uint8Array(await png.arrayBuffer()));
 console.log('Slide '+(i+1)+' renderizado');
}
await(await PresentationFile.exportPptx(p)).save(path.join(build,'candidate.pptx'));
const {finalizePresentation}=await import(pathToFileURL(path.join(skill,'container_tools/artifact_tool_utils.mjs')).href);
const result=await finalizePresentation({workspaceDir:work,candidatePath:path.join(build,'candidate.pptx'),finalPath:path.join(work,'entrega/DroidsCode-enredo-ilustrado.pptx'),pythonExecutable:path.join(runtime,'python/python.exe'),integrityValidatorPath:path.join(skill,'container_tools/inspect_presentation_package_integrity.py'),layoutValidatorPath:path.join(skill,'container_tools/inspect_presentation_layout_geometry.py'),layoutArgs:['--expected-slide-size-emu','12192000,6858000','--validate-heading-fit'],requiredNativeTableOwnerSlides:[],requiredNativeChartOwnerSlides:[],fontPolicy:{basis:'design',families:['Segoe UI']},verifyArtifactToolImport:true,receiptPath:path.join(build,'validation-final.json')});
console.log(JSON.stringify({file:result.finalPath,slides:result.packageIntegrity.slide_count,integrity:result.packageIntegrity.status,layoutFindings:result.presentationLayout.finding_count,importPassed:result.firstPartyImport.passed}));
