# Arte de Battle — proposta 3

Ferramenta: geração de imagens nativa. Referência aprovada: `C:/Users/silve/.codex/generated_images/01a18701-f786-7273-9858-6ef99d7ed7d4/exec-879bd1a9-0973-479c-8263-1c1f326220be.png`.

## Limpeza para integração

Use case: precise-object-edit. Production game UI asset derived from approved concept. Keep ONLY the industrial UI frames from the reference, in exactly the same relative positions and shapes on a landscape 16:9 transparent canvas. Remove the entire arena/scenery to REAL ALPHA TRANSPARENCY. Keep two top HUD metal panels, the lower central log metal panel, and the four bottom metal button panels. Preserve aged steel/brass/copper pixel-art aesthetic and small existing button icons (sword, bag, profile, running person). Remove ALL text, words, names, labels, letters, digits. Remove all colorful red/cyan health and energy fills: leave dark empty horizontal meter troughs; keep their thin brass frames. Name plates empty. Button faces empty except their icons. Log interior dark opaque, plain, no printed horizontal lines; enlarge its height UPWARD by around 40% for four readable lines of live text, without moving bottom buttons. The four bottom button frames must be separate with transparent narrow gutters, not joined into a single solid bar. No additional panels, no backdrop, no shadows filling the arena, no checkerboard baked into pixels. Keep interiors of retained UI panels opaque dark metal so live cream text can be overlaid later. Do not redesign, do not add portraits or decoration. Output is only these seven reusable UI panels on genuinely transparent canvas, no text anywhere.

## Ajuste único — HUD do inimigo

Referência: primeira saída limpa `exec-94560909-e225-452e-8807-cc0d6b517142.png` no mesmo diretório de geração.

Edit ONLY the top RIGHT HUD panel. It is for an enemy with only ONE health meter, so remove its lower empty meter trough completely, shorten that right panel vertically to contain just the name plate and ONE horizontal empty trough, and close the bottom with matching brass edging. Keep it aligned at the same top/right position and width. Preserve absolutely everything else: left HUD has two empty troughs, lower central blank log panel, four lower separate button panels and their existing icons, exact placement, transparent canvas, pixel-art metal texture. Keep all words/numbers absent. Background must remain REAL ALPHA TRANSPARENCY, interiors of panels dark opaque. No second bar in right HUD. No other changes.

## Saída e importação

Saída final original: `C:/Users/silve/.codex/generated_images/01a18701-f786-7273-9858-6ef99d7ed7d4/exec-d9731b65-2983-42f2-a6f7-a5579928d0a7.png`.

Cópia idêntica no projeto: `Assets/UI/BattleIlustrada/battle-ui-atlas.png`,1671×941, RGBA. Amostra na arena alpha0; interiores do log/botões alpha253. Sete sprites criadas pelo Unity referenciam regiões da mesma textura, sem reamostrar/editar pixels. Importação Sprite/Single, sem mipmap, sem compressão, Bilinear, limite2048, transparência preservada.

Regiões em pixels a partir do canto superior esquerdo (x,y,largura,altura):

| Sprite | Região |
|---|---|
| hud-apollo |14,14,535,201|
| hud-inimigo |1122,14,535,162|
| moldura-log |307,585,1056,207|
| botao-atacar |14,797,418,130|
| botao-item |432,797,404,130|
| botao-status |835,797,405,130|
| botao-fugir |1240,797,417,130|

Preparado/importado, sem integração na cena. Fixture inicial arquivada em `preparo-validacao/` porque erros existentes do terminal impedem compilação e teste; não foi executada com sucesso nem comprova o comportamento futuro.
