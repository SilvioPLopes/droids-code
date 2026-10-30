# Sucata Reforçada — Droid B, v1

- ID existente: `sucata_reforcada`; variante da família `sucata_captura` (Droid A).
- PNG de origem: `sprite-conceito-v1.png`,1263×1246, RGBA, uma pose estática voltada à esquerda; fundo transparente. Cópia para integração: `Assets/sprites/Entities/Enemies/Battle/SucataReforcada-v1.png`.
- Ferramenta: geração nativa ImageGen, edição/variação da arte A aprovada. Prompt exato em `prompt.txt`. Sem compositor/peças LPC, sem folha de animações ou direções adicionais.
- Fonte canônica: CatalogoDeInimigos descreve uma sucata de captura sobrevivente, remendada pela Apex. Nome/estatísticas do catálogo permanecem intactos. “Droid B” é rótulo de produção, não renomeação do inimigo.

## Decisões visuais propostas

Mesma cabeça/sensor âmbar, pinças abertas, pés e pose da A. Peitoral/ombros em aço cinza com rebites maiores, colares e atuadores dos braços reforçados, proteção das canelas e faixas amarelas de advertência. Detalhamento/cor/formato dos reforços são decisões de design, não descrições físicas explícitas do enredo. Não transformar em chefe ou espécie diferente.

## Procedência e uso

Referência direta: `../sucata-captura/sprite-conceito-v1.png`, geração anterior aprovada pelo usuário. Produção por ImageGen; não foi exportada do gerador LPC. Cópias preservam os pixels e a transparência; sem redimensionamento/recorte por código. Importação Unity Sprite/Single, Point, sem mipmap/compressão e sem redimensionamento NPOT; limite2048. A imagem de alta resolução é renderizada em um quadro UI menor; isso não a converte em folha LPC nativa64×64.

Integração usa o mesmo componente visual local da A, selecionando por ID do encontro; IDs sem arte própria preservam a arte legada. Não introduz níveis, novos encontros ou balanceamento. Avaliação visual da B/resultado integrado continua com o usuário; não confundir aprovação da arte A com aprovação automática de B.
