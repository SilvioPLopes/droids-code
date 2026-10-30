# Bag ilustrada — análise e integração

**Atualização documental23/10:** relatório existente `output/bag-ilustrada/validacao.txt` registra OK844×475/1280×720:14itens acessíveis, seleção sem consumo, retorno/reabertura e vazio. Não houve novo teste nesta revisão. Input real/aceite visual permanecem distintos. Alteração posterior de equipamentos: ação da Bag orienta para terminal, instalação/devolução pelo serviço; [entrega e testes posteriores](2026-10-23-droid-ilustrado.md). Isso substitui descrições antigas de Equipar por botão abaixo. Arte vinculada no Canvas; reutilização isolada do prefab base requer conferir os campos, conforme [inventário](2026-10-17-auditoria-cenas.md#inventário-de-reutilização-conferido-no-disco).

## Integração autorizada em 23/10/2026

Usuário aprovou a arte e pediu inserção imediata. Escopo: `BagUIManager.cs`, novo `ApresentacaoBagIlustrada.cs` e meta, referência da arte somente no componente Bag do `Canvas.prefab`, importação da arte aprovada e documentação vinculada. Dependências compartilhadas: Canvas atende City/CasaLip/Game; catálogo visual por ID e operações de itens/saves existentes serão reutilizados sem alterações. Nenhuma cena, Canvas/Scaler global, loja, terminal ou menu principal será redimensionado.

Montagem local criada ao abrir a Bag: grade de 12 itens por página, seleção antes de Usar/Equipar, ícone e descrição reais, quantidades, retorno e feedback local já existente. Paginação mantém acesso a todo o inventário elegível; filtro fora de batalha preservado. Arte aprovada, sem nova geração. Controles antigos preservados no prefab e ocultos apenas na instância durante a montagem.

Backup anterior: `output/bag-ilustrada/backup/` contém Canvas.prefab, BagUIManager.cs e meta da imagem. Reversão: restaurar apenas o registro Bag/alterações próprias do script/meta e retirar o novo apresentador, conferindo diff e preservando edições posteriores.

Aceite: ícones/quantidades dentro dos slots, detalhe e ação legíveis, retorno funcional, navegação acima de 12 itens e ausência de consumo ao selecionar. Perfil virtual 1536×1024, encaixe proporcional local em 98% do Canvas; resolução de referência 844×475 e 1280×720. Game View/estado atual não confirmados: conector Unity indisponível nesta sessão. Validação mínima: compilação com referências Unity, inspeção de vínculos e teste isolado de seleção/paginação quando possível, sem gravar save. Validação visual em jogo permanece distinta da arte já aprovada.

### Resultado e retomada da integração

**Editado e salvo:** arte vinculada no registro Bag do Canvas.prefab (uma linha; demais registros intactos), novo apresentador e ramificação local no controlador. Montagem dinâmica ao abrir: ícones exclusivos por ID, quantidades, seleção, descrição com rolagem/barra, botão Usar/Equipar, páginas e Voltar. Reabertura reutiliza a montagem. Inicialização do controlador é idempotente para não fechar uma Bag já aberta antes de Start. Métodos de cura/equipamento/ações e chamadas de save não foram alterados. Importação preservou GUID/fileID da imagem e ampliou o retângulo do sprite de x12/largura1513 para x0/largura1536, mantendo todo o PNG e o mapeamento da arte. PNG original intacto.

**Verificado:** compilação de Assembly-CSharp e Assembly-CSharp-Editor com referências Unity e inclusão explícita dos arquivos novos: zero erros, dois avisos preexistentes de API obsoleta em MenuMundoManager. A primeira execução sem restore encontrou ausência de project.assets.json; restore com feed local vazio resolveu, sem atualização de pacotes. Diff do prefab restrito ao campo arteBag. Cenas e Canvas/Scaler globais não editados.

**Ainda não testado no Editor:** comportamento, raycast/input real e render nas resoluções de aceite. Fixture `ValidacaoBagIlustrada` verifica vínculo, acesso a mais de 12 itens reais, seleção sem consumo, Voltar/reabertura e vazio, em cena de prévia com singleton restaurado no finally. Não chama uso de itens nem saves. Marcador `output/bag-ilustrada/validar.pendente` solicita uma execução após importação/recompilação, somente fora de Play e no projeto DroidsCode; relatório esperado em `validacao.txt`, capturas `preview-844x475.png`/`preview-1280x720.png`. Nenhum resultado dessa fixture foi observado nesta sessão. Não interpretar compilação como teste de comportamento ou aceite visual.

**Estado Unity e próximo passo:** conector indisponível; Play/cena/zoom atuais não confirmados nem controlados. Nenhuma cena foi salva ou Play interrompido por esta rodada. Após importação, abrir Bag pelo menu em City/CasaLip, conferir legibilidade/ícones/ação e retorno; consultar relatório da fixture antes de repeti-la. Arte aprovada pelo usuário; integração visual ainda sem aprovação. Backups em `output/bag-ilustrada/backup/`; sem saves alterados, commit ou push.

## Histórico da proposta artística (etapa anterior)

**Pedido e estado da etapa anterior:** usuário quer começar pela Bag, analisar referências9/4/8/14 e criar uma imagem coerente com a loja/menu novos. A etapa abaixo produziu a arte de proposta, antes da aprovação e integração registradas acima.

**Escopo antes de editar:** criar somente imagem irmã em `Assets/UI/BagIlustrada/bag-conceito.png`, este plano e link no índice. Preservar originais/metas, cenas, prefab Canvas, scripts, demais menus e saves. Sem teste de uso de itens ou operação do Unity. Reversão: desreferenciar/remover somente os novos arquivos próprios se solicitado; nenhuma substituição de imagem aprovada.

**Fontes necessárias:** [análise das artes](../historico/2026-10-23-aproveitamento-artes-menus.md), [método visual](../guias/interfaces-sobre-imagens.md), `Assets/Scripts/Menus/BagUIManager.cs` para identificar conteúdo existente; referência de acabamento em `Assets/UI/LojaIlustrada/loja-fundo.png` e menu novo. Não usar a imagem da loja como composição final da Bag.

## Análise

| Referência | Aproveitamento proposto |
|---|---|
| `14.jpeg`, painel superior direito | Melhor estrutura de inventário: grade de itens e área lateral de detalhe. Reduzir densidade de slots para caber ícone/quantidade legíveis. |
| `8-removebg-preview.png` | Superfícies limpas para receber dados reais e molduras sem palavras/números de demonstração. |
| `9.jfif` | Kit modular de placas, molduras e slots; combinar materiais com os menus novos. Fundo magenta é referência de recorte, não cor da interface final. |
| `4-removebg-preview.png` | Referência de hierarquia de informações. Não copiar textos/números embutidos nem o painel de atributos para a Bag. |
| Loja/menu atuais | Continuidade de ferro escuro, cobre/latão, rebites e engrenagens discretas. Bag pode ter pequenos detalhes de couro para sugerir armazenamento, mantendo a família artística. |

**Conteúdo existente observado no código:** inventário com quantidade, filtro de itens usáveis fora de batalha, cura, equipamento e ações fora de batalha, botão Voltar e mensagem de retorno. Hoje o clique já usa/equipa o item; algumas ações gravam save. A arte não altera esse fluxo nem aprova, por si só, um futuro fluxo de selecionar/confirmar. Esse comportamento deverá ser delimitado antes da integração.

## Composição escolhida para a proposta

- Painel horizontal1536×1024 como intenção de geração, com moldura estreita e espaço útil maior que a decoração. Conferir dimensões efetivamente entregues.
- Área principal esquerda com4×3 espaços grandes e vazios. Doze posições visíveis são proposta de enquadramento, **não capacidade máxima do inventário**; a futura montagem deve manter acesso aos demais itens com navegação apropriada, definida naquela etapa.
- Lateral direita com janela para sprite ampliada, espaço de nome, descrição e uma superfície de ação variável. Uso/equipamento usam os dados existentes; não inventar ações extras.
- Rodapé com área de retorno/mensagem e botão Voltar. Sem inventar peso, capacidade, dinheiro ou contadores que não pertençam ao painel atual.
- Sem títulos, nomes, ícones de produtos, quantidades ou estados pintados no fundo. Na futura montagem, usar as referências por ID de CatalogoVisualDeItens e TMP/componentes reais. Espaços para ações são somente arte nesta rodada.

**Aceite desta etapa:** imagem com identidade coerente, grade menos densa que a14, áreas claramente separadas e sem textos/dados falsos; original intacto e proposta salva separadamente. Inspecionar a imagem em tamanho grande e numa redução de prévia. Isso comprova composição, não legibilidade de fontes ainda não montadas nem funcionamento da Bag.

**Validação mínima e retomada:** inspeção visual da arte e metadados do PNG; conferir docs/links. Não compilar/testar jogo para tarefa exclusivamente visual. Próximo passo após reação do usuário: definir montagem/escopo e preservar funções existentes, sem interpretar aprovação da arte como aprovação de mudança econômica/uso de itens. Estado Unity não consultado nem modificado nesta rodada.

## Resultado da proposta

**Criado e salvo:** [bag-conceito.png](../../Assets/UI/BagIlustrada/bag-conceito.png),1536×1024,PNG RGBA com transparência confirmada no canto externo. Arquivo novo, sem substituir imagem anterior. Geração pela ferramenta image_gen integrada; original em `.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-1ee9e8dc-6949-46d8-aa49-12bb3a784438.png`.

**Inspeção:** exatamente12 slots em4×3, molduras maiores que a grade densa da14; prévia quadrada/nome/descrição/ação separados à direita, placa de título e rodapé de mensagem/Voltar. Paleta e materiais coerentes com loja/menu novos; detalhes de couro distinguem o painel como armazenamento. Sem palavras, números, sprites de itens ou estados falsos pintados. Áreas inspecionadas na arte gerada; coordenadas do prompt são intenção e deverão ser medidas na imagem para montar os componentes reais.

**Limites e próximo passo:** não existe montagem nova no Unity nesta rodada; quantidade de slots pintada não altera limite do inventário. Não foi testada tipografia em jogo nem redução controlada com dados reais, pois a arte não contém textos. Uso/equipamento, callbacks, filtro de itens e saves continuam inalterados. Opinião/aprovação visual do usuário pendente. Após sua reação, definir escopo da integração e comparar legibilidade no tamanho de uso. Não gerar outra arte nem modificar a Bag só para verificar o arquivo entregue.

**Estado e verificação:** scripts, prefab Canvas e cenas não editados; estado Unity não consultado. Sem testes de comportamento, gravação de save, commit ou push. Diff documental sem erros de whitespace; referências locais conferidas. Apenas arte nova e documentação nesta rodada.

## Prompt da proposta

Ferramenta image_gen integrada, com referências14,8,loja nova,menu novo e9; imagem nova baseada nessas referências, sem substituí-las.

```text
Create ONE production-ready game UI background for a BAG / INVENTORY, landscape 1536x1024 composition, not a collage. Reference image 1 (14.jpeg): use ONLY its upper-right inventory layout as structural inspiration. Reference image 2 (8-removebg-preview.png): blank functional panels and clean modular spaces. Reference image 3 (existing loja-fundo.png): match the game's existing dark iron, weathered brass/copper, restrained gears and pixel-art craftsmanship. Reference image 4 (menu-fundo.png): same copper/brass family, but do NOT copy its eight-button menu. Reference image 5 (9.jfif): modular bevels and slot edges. This bag should visibly belong to the same RPG interface set as the existing shop and pause menu. One cohesive frontal flat inventory board, tight framing and narrow aged metal border, dark matte charcoal fields for pale readable native UI text, small corner gears/rivets, tiny leather straps at upper corners to suggest a carried storage case. No huge pipes or decoration wasting reading area. Blank title plaque at upper left/center, header strip otherwise clear. MAIN LEFT region around x65-895,y210-825: EXACTLY TWELVE generous blank square inventory slots in FOUR columns and THREE rows, evenly spaced, approximately170px square, narrow copper/brass bevel frames, interiors flat dark and completely empty, with room for real item sprites and quantity overlays later. RIGHT region around x950-1470,y210-825: one blank framed preview square about150px at top, a wide blank name strip underneath, a large blank low-texture description rectangle below, and ONE blank wide action-button surface at the bottom. Keep all these distinct and non-overlapping. Footer y900-990: broad EMPTY message area on the left and ONE EMPTY back-button surface on the far right. This is the background asset, real Unity UI will supply all labels, item pictures, quantities, actions, scrollbar and selection states. Do not paint text, numbers, letters, item icons, potions, currency, HP/XP bars, fake capacities, weight meters, category labels, character portraits, cursors, checked states or navigation arrows. No magenta, no watermark, no scenery. Transparent alpha outside the board silhouette, no transparency inside the dark fields. Match artistic identity of supplied references while giving this screen a clearly organized inventory purpose.
```
