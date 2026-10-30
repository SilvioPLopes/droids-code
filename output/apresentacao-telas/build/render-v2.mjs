import fs from 'node:fs/promises';
import { FileBlob, PresentationFile } from '@oai/artifact-tool';
const root='C:/Users/silve/DroidsCode/output/apresentacao-telas/';
const p=await PresentationFile.importPptx(await FileBlob.load(root+'entrega/DroidsCode-telas-mapas-e-npcs.pptx'));
for(let i=6;i<p.slides.items.length;i++){
 const png=await p.export({slide:p.slides.items[i],format:'png',scale:1});
 await fs.writeFile(root+'build/v2/previews/slide-'+(i+1)+'.png',new Uint8Array(await png.arrayBuffer()));
 console.log('Renderizado slide '+(i+1));
}
