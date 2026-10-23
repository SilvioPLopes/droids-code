# Loja ilustrada — City

## Retomada vigente — retorno de transações dentro da loja (23/10/2026)

**Pedido novo e prioridade:** usuário rejeitou a confirmação abrindo a conversa; compra/venda e falhas de transação devem informar no próprio menu, mantendo a loja aberta. Exceção explícita à diretriz anterior de diálogo, que permanece para mensagens fora da loja. Captura mostra “Vendeu Poção Pequena por 8 Gold.” no diálogo; não é falha econômica.

**Causa sustentada:** `LojaUIManager.MostrarMensagem` chama `DialogoUIManager.Mostrar` e desativa `painel` quando aceito. Campo `textoMensagem` da loja atual está nulo. Há um texto local `AjudaSelecao` no rodapé, reutilizável sem nova janela nem alteração da arte.

**Escopo desta correção:** LojaUIManager.cs, ApresentacaoLojaIlustrada.cs, helpers MontagemLojaIlustrada.cs/ValidacaoLojaIlustrada.cs e documentos deste plano/contrato. Reutilizar AjudaSelecao como retorno local, ajustar somente seu espaço de texto ao inicializar o painel. Não editar City, Canvas/Scaler, diálogo compartilhado, catálogo, arte, economia ou saves. Consumidor conhecido: única loja da City; NpcInterativo conserva abertura/fechamento e contador.

**Aceite:** comprar/vender e tentar sem Gold mostram texto legível no rodapé; loja permanece ativa, não abre diálogo, não exige avançar conversa e permite repetir a operação. Preservar saldo, quantidade, abas e callback. Mensagem longa em até duas linhas sem sobrepor Voltar/Comprar.

**Backup/reversão:** copiar quatro scripts anteriores em `output/loja-ilustrada/feedback-local/backup/`; rollback apenas dos hunks desta correção. Estado confirmado antes de editar: DroidsCode/City limpa, fora de Play, compilação parada. Sem edição persistente de cena nem teste que grave saves.

**Validação mínima:** adaptar fixture isolada para exigir feedback local e ausência de abertura de diálogo; confirmar falha antes de mudar controlador; testar compra/venda repetida e Gold insuficiente, renderizar feedback longo em 844×475 e 1280×720. Separar API/render de input real e aceite visual. Registros abaixo são históricos e não definem o aceite vigente de mensagens.

**Resultado desta correção:** removidos fila de avisos, chamadas a DialogoUIManager e ocultação/reabertura do menu para transações. Inicialização associa `textoMensagem` ao `AjudaSelecao` existente via `PrepararMensagem`, ajustando somente esse texto para x85/y905, 730×76, fonte32 e quebra de linha. Conteúdo mantém origem no catálogo; ação permanece disponível após aviso, conforme seleção/estoque. Helper de montagem atualizado para refletir o contrato, sem reexecutá-lo.

**Evidência:** teste novo falhou antes da correção com “Compra precisa confirmar dentro da loja, sem abrir diálogo.” Após alteração, `ValidacaoLojaIlustrada.Executar` retornou OK em 844×475 e 1280×720 para seleção, compra/venda repetida, insuficiente, retorno local sem abrir conversa, vazio, conversa/callback existente preservados, rolagem, clique, enquadramento e contador. Testes por API, não input físico. Render pequeno inspecionado: confirmação no rodapé sem corte/sobreposição; prévias atualizadas em `output/loja-ilustrada/preview-*.png`. Aceite visual do usuário ainda pendente.

**Estado final e preservação:** City limpa, fora de Play, compilação parada e sem falha; zero objetos temporários. Hashes de City e DialogoUIManager iguais aos anteriores desta rodada, armazenados em `output/loja-ilustrada/feedback-local/preservados.json`; nenhuma cena salva e nenhum save gravado. Backup dos quatro scripts em `feedback-local/backup/`. Diff sem erros de whitespace. Sem commit/push. Próximo passo: avaliar a transação no menu durante o jogo, sem remontar nem alterar outros painéis.

## Escopo e aceite

Usuário escolheu executar a ideia da loja e forneceu capturas: lista e botões pequenos, nenhum fundo ilustrado, aviso de Gold sobrepondo os demais textos. Autorização anterior permite adaptar/fundir artes. Proposta: composição horizontal de metal/latão baseada nas imagens 8/9, produtos com ícones e rolagem à esquerda, detalhes à direita, saldo e abas no topo, ação explícita de uma unidade e voltar. Selecionar um produto não deve comprar/vender. Avisos na caixa de diálogo existente.

**Arquivos permitidos:** ramo `Canvas/Loja` e seus descendentes em `Assets/Scenes/City.unity`; `Assets/Scripts/Menus/LojaUIManager.cs`; novos componentes locais de apresentação, helper de montagem/validação em Editor; novos assets em `Assets/UI/LojaIlustrada`; acréscimos de falas da loja no catálogo de diálogos se necessários; este plano, índice e contrato de interface.

**Preservar:** outros ramos/cenas/prefabs, Canvas/Scaler global, NPCs, terminal, preços, inventário, saves, regras econômicas atuais. Não corrigir discrepância antiga entre preço de venda do catálogo e uso de Preco no controlador nesta tarefa visual.

**Consumidores:** busca por GUID encontra LojaUIManager somente em City; NpcInterativo abre loja após diálogo e controla contador/callback. Componentes TMP/Button e prefab de item podem ser compartilhados: não editar o asset de botão original. Diálogo é compartilhado: usar sua API, sem mudar sua apresentação ou contador.

**Evidência inicial:** Unity DroidsCode, City em Play, loja aberta; tela renderizada 3840×2160; Canvas local ScreenSpaceOverlay com escala 1, sem CanvasScaler; lista 500×300, linhas 450×30 e abas 160×30. Capturas do usuário têm 321×165 e 505×486; zoom/editor e resolução de origem dessas capturas não determinados. Não confundir essa redução com tamanho real do render. Mensagem tem caixa 200×50 e texto cresce sobre campos vizinhos. Código compra/vende diretamente no clique do item.

**Aceite:** painel ilustrado proporcional com texto legível em 844×475 e 1280×720; saldo, seleção, descrição, preço, abas, ação e voltar separados e sem sobreposição. Todos os produtos acessíveis por rolagem. Falta de Gold, compra/venda e inventário vazio compreensíveis; avisos de operação no diálogo. Preservar retorno à loja e callback de fechamento.

**Backup/reversão:** antes da mutação persistente copiar City, controlador e catálogo alterado para `output/loja-ilustrada/backup/`; registrar dados da loja aberta antes de sair de Play. Restaurar somente mudanças próprias, sem sobrescrever trabalho posterior. Nenhum teste deve gravar save; usar estado isolado, restaurar singleton/counters ao final.

## Etapas

- [x] Criar fundo separado com image_gen, inspecionar composição; conservar originais.
- [x] Confirmar regressão pertinente antes da correção: selecionar Módulo de Treinamento gastou 120 Gold (1000 → 880) na fixture antiga. Primeiro teste havia escolhido um botão estático sem ação; teste ajustado para o produto gerado, não usado aquele resultado como aprovação.
- [x] Adaptar controlador e montagem local fora de Play; preservar economia e referências externas.
- [x] Compilar, testar comportamento isolado, renderizar pequeno/grande e comparar; conferir diff restrito à loja.
- [x] Atualizar resultado, backups, estado Unity e limites do aceite.

## Resultado e retomada da execução

**Editado/salvo:** City recebeu uma montagem em `Canvas/Loja`, referência 1536×1024, encaixe local proporcional, fundo opaco exclusivo, produtos em duas colunas roláveis, detalhes com rolagem, saldo, abas, ação explícita e voltar. Objetos antigos ficaram inativos para preservar IDs; o prefab compartilhado de botão não foi alterado. `LojaUIManager` seleciona antes de comprar/vender e encaminha avisos ao diálogo com retorno à loja; espera se outro diálogo estiver aberto. Textos de transação têm origem em `CatalogoDeDialogos.AvisoDaLoja`. Preços e regra de venda preexistentes preservados.

**Assets:** `Assets/UI/LojaIlustrada/loja-fundo.png` e `loja-icones.png`, gerados com a ferramenta image_gen integrada, mais seis sprites por regiões iguais do atlas. Fundo 1536×1024, dois painéis vazios e moldura de ferro/latão inspirados em 8/9; nenhum texto impresso. Atlas 1536×1024, grade 3×2: reparo, frasco de óleo, EMP, núcleo de energia, servo/engrenagem e mochila, inspirados em 3/17; transparência conferida por canal alpha no arquivo (canto alpha 0). Ícones representam categorias e núcleo de energia; não são ilustrações individuais de cada produto. Originais não editados.

**Prompts registrados:** fundo: “production game UI background landscape 1536×1024, weathered iron/aged brass from 8/9, blank header, large blank left tray for dynamic products, blank right detail panel, footer, no slots/text/items/buttons drawn”; atlas: “transparent pixel-art RPG spritesheet 1536×1024, exactly six standalone icons in equal 512×512 cells, repair kit/oil flask/EMP grenade/energy core/servo motor/field bag, references 3/17, no captions/magenta/frame”. Saídas originais em `.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-5df0487e-869f-46f1-8878-6132e8d54da4.png` e `exec-74993e0b-905e-4a7a-9e9d-8f149a38bed7.png`; cópias finais já em Assets.

**Preservação:** backup anterior em `output/loja-ilustrada/backup/`; dados visíveis da sessão em `output/loja-ilustrada/sessao-antes.json`. Play encerrado para aplicar montagem persistente. Comparação por registros YAML identificou serialização automática fora da loja: dois registros e uma referência stripped foram repostos do backup. Comparação posterior: zero registros alterados/novos fora da loja e nenhum removido. Ordem original dos registros preservada para reduzir o diff. Nenhum save gravado.

**Verificação e correções de fixture:** scripts de apresentação/controlador/montagem compilados sem falha antes da aplicação; montagem retornou sucesso e salvou uma loja. Primeira execução do helper revelou que o diálogo está local na City, não no prefab Canvas; fixture corrigida para clonar o diálogo real. A câmera da fixture também foi ajustada a uma unidade por pixel antes de usar escala mundial para medir tamanho nominal de fonte. Essas correções não modificaram layout por suposição.

**Interrupções resolvidas:** após atualizações delimitadas da City, confirmações de recarga bloquearam o conector; usuário confirmou Reload nas duas ocasiões. Respostas pendentes foram recolhidas antes de novas chamadas. Uma desconexão posterior foi seguida de consulta ao estado/temporários antes de repetir o teste; nenhuma montagem foi repetida.

**Resultado final testado:** `ValidacaoLojaIlustrada.Executar(844,475)` e `(1280,720)` retornaram OK após a última alteração. Conferidos: seleção sem transação; compra de uma unidade; Gold insuficiente sem alteração de inventário; venda e inventário vazio; aviso no diálogo sem sobrescrever conversa/callback existente; retorno à loja; contador/callback de fechamento; rolagem de produtos e descrição longa; regiões de clique por raycast; enquadramento e fonte nominal mínima. Testes isolados por API, sem input físico nem gravação de saves. O saldo de 350 mostrado nas prévias é da fixture, não do progresso do usuário.

**Inspeção visual:** renders em `output/loja-ilustrada/preview-844x475.png` e `preview-1280x720.png`. A primeira inspeção identificou estoque junto à moldura inferior; somente preço/estoque foram movidos para y710/y775, respectivamente, com backup da montagem anterior em `output/loja-ilustrada/antes-espacamento.unity`. Helper de montagem atualizado, mas não reexecutado. Teste novo assegura limite inferior do estoque; renders finais inspecionados mostram os textos dentro da moldura, sem sobreposição. Legibilidade visual avaliada pelo agente; aceite do usuário pendente.

**Estado final:** DroidsCode, City limpa e ativa, fora de Play, sem compilação em andamento/falha; nenhum EstadoTesteLoja/CanvasTesteLoja remanescente. Diff documental/C# e cena sem erros de whitespace; registros externos restaurados continuam idênticos ao backup. Originais e demais cenas/prefabs preservados. Não houve commit/push.

**Próximo passo:** usuário abrir a loja no jogo para avaliar aparência e input real. Se rejeitar, preservar a evidência e reavaliar a causa antes de ajustes. Não salvar novamente a City de modo amplo sem conferir serializações externas, nem reexecutar a montagem para obter prévia.

Método: [guia](../guias/interfaces-sobre-imagens.md); referências: [análise](2026-10-23-aproveitamento-artes-menus.md). Autorização de liberdade criativa recebida; execução local por um agente.
