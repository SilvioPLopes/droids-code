# Menu do mundo ilustrado

**Pedido:** melhorar o menu com a referência `Assets/sprites/Atualizados/novos/12 - Copia.jfif`, sem perder nenhuma opção, inclusive as incompletas. Liberdade visual já autorizada. Duas colunas/quatro linhas na ordem de leitura: Carregar/Salvar, Terminal/Status, Bag/Droid, Opções/Voltar. Tipografia real sobre arte de metal, cobre, engrenagens e medidores.

**Escopo permitido:** somente ramo `PainelMenuMundo` e descendentes no prefab `Assets/Prefabs/Canvas.prefab`; novo componente de apresentação e helper Editor de montagem/validação; nova arte irmã em `Assets/UI/MenuMundoIlustrado`; documentação deste plano/índice/contrato. Não mudar `MenuMundoManager.cs`, suas referências, listeners ou funções. Não editar cenas, outros painéis, Canvas/Scaler global, terminal, loja, preços, saves, flags ou funções incompletas.

**Consumidores:** Canvas é prefab compartilhado, com instâncias em Game/City/CasaLip. O gestor persiste entre cenas e bloqueia abertura em cenas configuradas; preservar essas propriedades e callbacks. Botões de Salvar/Carregar podem gravar ou trocar cena: conferir referências/código preservados sem invocá-los no teste visual. Rotas Status/Terminal/Bag/Droid podem ser exercitadas em fixture isolada sem uso de itens ou código Lua.

**Evidência inicial:** DroidsCode, City limpa e fora de Play; render3840×2160, Canvas ScreenSpaceOverlay, escala efetiva1, sem CanvasScaler na instância consultada. Painel com escala local4×4 e oito botões160×30 em coluna; campo de aviso200×50. Captura do usuário mostra janela503×619, escala de visualização/origem não determinada. Registrar condições controladas nas prévias, não alterar zoom do Editor para alegar legibilidade.

**Aceite:** oito botões originais presentes, legíveis e clicáveis em duas colunas; sem corte/sobreposição com moldura, ícones e aviso; foco visual semelhante à referência. Texto mínimo nominal de rótulos aproximadamente18px em844×475 e layout proporcional também em1280×720 e503×619. Mesmos destinos/retornos e aviso de Opções preservados; placeholders não removidos nem implementados nesta tarefa. Campo de aviso existente continua disponível em região própria.

**Backup e reversão:** prefab anterior em `output/menu-mundo-ilustrado/backup/Canvas.prefab`; hashes de gestores/cenas preservados em `backup/preservados.json`. Registrar IDs de gestor/botões antes de aplicar. Reverter somente registros do ramo alterado e novos assets próprios, preservando trabalho posterior. Mutação de prefab fora de Play, guarda de aplicação única; nenhuma cena salva. Conferir/retirar eventual serialização automática fora do ramo comparando registros com backup.

## Etapas

- [x] Confirmar preservação das oito referências e teste inicial da disposição em duas colunas.
- [x] Gerar fundo novo baseado em12, sem palavras impressas; inspecionar posições reais de oito espaços.
- [x] Montar apresentação local reaproveitando botões/labels/aviso e listeners; encaixe proporcional sem Canvas global.
- [x] Testar oito regiões clicáveis, rotas sem gravação, aviso de opção incompleta e fechar/retornar; renders pequenos/retangular/vertical.
- [x] Conferir escopo do diff, hashes, estado Unity e documentar resultado/limites.

## Resultado e retomada

**Editado/salvo:** prefab com uma apresentação nova exclusivamente no ramo do menu; componentes/IDs de todos os botões mantidos. Rótulos reais Carregar, Salvar, Terminal, Status, Bag, Droid, Opções e Voltar. Retorno existente de Opções permanece no rodapé; nenhum listener substituído. Escala proporcional do painel com perfil1254×1254/margem0.95, sem Canvas global. Instâncias antigas com escala4 são corrigidas localmente ao abrir; cenas não foram editadas.

**Geração e origem:** image_gen integrado; referência local12 - Copia.jfif; saída copiada para `Assets/UI/MenuMundoIlustrado/menu-fundo.png`,1254×1254 com alpha confirmado. Original do gerador: `.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-7517e1f9-20b4-42fd-8dda-2a627edb5f2c.png`. Prompt completo em `output/menu-mundo-ilustrado/prompt.txt`: moldura de ferro/cobre, tubulação, engrenagens, oito espaços escuros em2×4 com pictogramas de pasta/disquete/monitor/perfil/mochila/robô/engrenagem/porta, placa superior e aviso inferior vazios, sem letras/números/magenta, alpha fora do painel. Coordenadas finais foram medidas na imagem efetiva, em vez de assumir que o prompt seria seguido exatamente.

**Teste inicial:** oito referências distintas presentes; teste de pares alinhados em duas colunas retornou falso antes da montagem. IDs globais dos botões guardados em `backup/botoes.json`; após montagem, comparação confirmou as mesmas oito referências originais.

**Testes após:** `ValidacaoMenuMundoIlustrado.Executar` retornou OK em844×475,1280×720 e503×619: oito rótulos/áreas clicáveis, duas colunas, fontes/enquadramento, Status/Bag/Droid/Terminal com retorno ao menu, aviso de Opções incompleta, Voltar e contador sem resíduo. Nenhum código Lua executado, uso de item ou save gravado. Salvar/Carregar foram conferidos quanto a referência original, raycast e preservação do código do gestor; **não foram acionados nem tiveram funcionamento de persistência revalidado** nesta tarefa. Escape/input físico não testados; código de abertura/fechamento permaneceu intacto.

**Inspeção visual:** renders `output/menu-mundo-ilustrado/preview-844x475.png`, `preview-1280x720.png` e `preview-503x619.png`. Pequeno e vertical inspecionados: oito nomes legíveis e dentro das molduras, pictogramas desobstruídos, aviso em duas linhas dentro da placa inferior. Aprovação visual do usuário pendente; renders de fixture não substituem teste no jogo.

**Consumidores:** abertos City/CasaLip/Game em preview e fechados sem salvar. Cada cena resolveu um gestor, uma apresentação e oito referências, com pares alinhados após aplicação local do estilo. Nenhuma montagem repetida para testar.

**Preservação:** serialização automática havia acrescentado `cenasBloqueadas` ao registro do MenuMundoManager fora do ramo; o registro foi reposto do backup. Auditoria final em `output/menu-mundo-ilustrado/diff-final.json`:45 registros alterados dentro do menu, zero alterados/novos fora, nenhum removido. Hashes de MenuMundoManager, City/CasaLip/Game, LojaUIManager e TerminalUIManager iguais ao início. Diff sem erros de whitespace. Demais painéis, arte original e saves preservados; sem commit/push.

**Estado final:** DroidsCode, City limpa e ativa, fora de Play, compilação parada/sem falha, zero objetos temporários da fixture. Backups em `output/menu-mundo-ilustrado/backup/`. Próximo passo: usuário avaliar apresentação e input real; preservar captura/reavaliar causa se rejeitar. Não executar migração novamente para gerar prévia.

Método: [interfaces sobre imagens](../guias/interfaces-sobre-imagens.md); referência/análise: [artes disponíveis](2026-10-23-aproveitamento-artes-menus.md).
