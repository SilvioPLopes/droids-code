import fs from 'node:fs/promises';
const base='C:/Users/silve/DroidsCode/output/apresentacao-telas/build/';
let src=await fs.readFile(base+'create.mjs','utf8');
src=src.replace("import { Presentation, PresentationFile }", "import { Presentation, PresentationFile }");
src=src.replace("const build=path.join(work,'build');", "const build=path.join(work,'build/v2');");
src=src.replace('entrega/DroidsCode-telas-e-menus.pptx','entrega/DroidsCode-telas-mapas-e-npcs.pptx');
const extra=String.raw`
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
`;
src=src.replace("const candidate=",extra+"\nconst candidate=");
await fs.writeFile(base+'create-v2.mjs',src,'utf8');
