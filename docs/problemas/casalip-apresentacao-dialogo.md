# CasaLip: sprites, nome e apresentação do diálogo

## Estado em 17/10/2026

**Implementado; validação parcial registrada.** A montagem local refez o diálogo com Canvas escalável, âncoras coerentes, texto/avanço em escala um, nome habilitado e retrato encaixado com proporção preservada. Apollo recebeu idle e retrato LPC provisórios; Lip recebeu a arte disponível de protagonista, com animações e opção de espelhamento configurada para as direções. A etiqueta de Apollo passou ao mundo. A captura final `Logs/CorrecaoCasaLip/depois-1280.png` foi inspecionada pelo agente principal e está legível; C09 conferiu por API quatro direções/quatro diagonais e flip LPC/legado. Ainda faltam 1920, narração/fala longa e input real/física.

O sucesso usa `ControladorPuzzleAguaCasa` e as quatro falas de `casa_porta_apollo`; a origem do menu é limpa antes de fechar o terminal. Pela origem mundo/API, 100 concluiu e percorreu as quatro falas, com contador um durante diálogo e zero ao terminar; reexecutar não repetiu evento/diálogo/save. Duas revisitas por API mantiveram porta/terminal e não repetiram a introdução/sucesso, com save inalterado. O harness de UI/contexto/origem/migração passou novamente. Sucesso integrado pelo menu, laço finito, diálogo ocupado e reinício/load ainda não foram demonstrados. Resultados estão na [validação de 17/10](../revisoes/casalip-validacao-2026-10-17.md); não há encerramento global de UI-003/VIS-001/HIS-002.

## Registro histórico — coleta de 16/10/2026

As causas, referências de assets e recomendações abaixo descrevem o estado anterior à correção. As capturas originais continuam válidas como evidência desse diagnóstico.

**Coleta:** 16/10/2026, capturas de 22h57 a 23h01, America/Sao_Paulo. **Base:** `027952041d40e7957875a66641727684586455c1`, branch `docs/consolidar-contexto`.

**Escopo:** analisar os dados coletados pelo responsável e confrontá-los com código, cena salva, histórico Git e observação MCP. As ações de gameplay foram executadas pelo responsável; o agente inspecionou o estado resultante, inclusive objetos em DontDestroyOnLoad. Não houve alteração de scripts, cenas, prefabs ou saves nesta análise.

Tarefas: `UI-003` (layout), `VIS-001` (associações visuais), `HIS-002` (conversa de sucesso), `INT-001/HIS-001` (conclusão e revisita). Consulte o [plano](../plano-desenvolvimento.md) e a [revisão da casa](../revisoes/casalip.md).

## Dados preservados

As seis cópias têm SHA-256 igual ao arquivo original fornecido. A numeração corresponde à ordem dos anexos na conversa, não à ordem cronológica.

| Imagem | Horário e origem | O que mostra |
|---|---|---|
| [01 — diálogo no retorno](../revisoes/evidencias/casalip-2026-10-16/01-dialogo-retorno.jpeg) | WhatsApp Image 2026-10-16 at 23.01.01.jpeg | Quadro estreito/deformado, retrato do mago separado da moldura e nome distante |
| [02 — City](../revisoes/evidencias/casalip-2026-10-16/02-city.jpeg) | WhatsApp Image 2026-10-16 at 23.00.16.jpeg | O jogador chegou à cidade durante a coleta |
| [03 — sucesso](../revisoes/evidencias/casalip-2026-10-16/03-sucesso-terminal.jpeg) | WhatsApp Image 2026-10-16 at 22.58.50.jpeg | Frase de vitória acima do terminal, fora do sistema de diálogo |
| [04 — comando](../revisoes/evidencias/casalip-2026-10-16/04-comando-agua.jpeg) | WhatsApp Image 2026-10-16 at 22.58.16.jpeg | `nivel_agua=100` no campo; nome Apollo sobreposto à UI |
| [05 — personagens](../revisoes/evidencias/casalip-2026-10-16/05-personagens.jpeg) | WhatsApp Image 2026-10-16 at 22.57.50.jpeg | Apollo representado por mago; Lip usa corpo-base sem roupas |
| [06 — Editor](../revisoes/evidencias/casalip-2026-10-16/06-visao-editor.jpeg) | WhatsApp Image 2026-10-16 at 22.57.18.jpeg | Play Mode, nome deslocado e avisos de conexão no Console |

Essas capturas sustentam o funcionamento da primeira execução e da saída para City naquela coleta. Elas não comprovam sozinhas salvamento após reinício. O relato do responsável e a observação MCP sustentam a falha de retorno descrita a seguir.

## 1. Apollo usa duas artes de mago

O SpriteRenderer do objeto Apollo aponta para `Assets/sprites/Entities/Npc's/Wizzard/Death/Death-Sheet.png`. O retrato usa outro arquivo, [Resources/Retratos/Apollo.png](../../Assets/Resources/Retratos/Apollo.png), inspecionado visualmente: também é um mago.

[DialogoUIManager.ObterRetrato](../../Assets/Scripts/Historia/DialogoUIManager.cs) procura o nome do falante em Resources. A lista manual da cena está vazia; por isso a fala de Apollo encontra exatamente esse retrato. Trocar somente a sprite do mapa deixa o retrato errado.

O [Apollo novo](../../Assets/sprites/Entities/Npc's/Characters/Companion/Apollo/sprite.png) existe, mas sua [configuração](../../Assets/sprites/Entities/Npc's/Characters/Companion/Apollo/character-config.md) o identifica como placeholder LPC. Ele é uma figura de armadura usada como aproximação; alto-falante, assimetria e gestos do robô ainda estão pendentes. O visual definitivo precisa respeitar essa limitação. As sprites novas do protagonista também ainda não substituíram o corpo-base de Lip nesta cena.

**Direção de correção:** associar os assets escolhidos aos personagens e aos retratos separadamente; preservar componentes, tag, colisores e referências do Player. Registrar explicitamente qualquer placeholder utilizado.

## 2. O nome distante vem de um Canvas de tela

O objeto `Apollo/Canvas` está em `ScreenSpaceOverlay`, com `CanvasScaler.ConstantPixelSize`. Ser filho do personagem não o transforma em etiqueta no mundo.

O filho `Text (TMP)` contém "Apollo", fonte 36, `anchoredPosition = (0, 0)`. Na observação, sua posição de tela foi `(751, 236)`, centro de um Canvas de `1502 × 472`. Isso explica o nome no centro da imagem, sobre a mesa ou sobre o terminal, independentemente da posição do droid.

Há ainda um nome próprio do diálogo, `Canvas/PainelDialogo/TextoFalante`, que está com o componente TMP **desabilitado** na cena salva. São dois objetos diferentes.

**Direção de correção:** usar uma etiqueta em World Space posicionada acima de Apollo, e configurar o nome do falante dentro da janela de diálogo. Ambos devem acompanhar seus respectivos elementos sem sobreposição.

## 3. A janela pequena/deformada tem causa e commit identificados

O commit `ae69544`, de **15/10/2026 às 18h28**, mensagem **backoup**, alterou estes campos de [CasaLip.unity](../../Assets/Scenes/CasaLip.unity):

| Elemento | Alteração registrada |
|---|---|
| PainelDialogo | Âncoras normais `(0,0) → (1,1)` passaram a **invertidas**: `anchorMin=(1,1)`, `anchorMax=(0,0)` |
| PainelDialogo | Escala passou a `(1.25,1.25,1)` e SizeDelta a `(1675.4065,739.8648)` |
| TextoFala | Escala passou de `(1,1,1)` para **`(0.25,0.325,1)`**; margens também foram alteradas |
| TextoFalante | Foi desabilitado, deslocado e reduzido de fonte 36 para 12 |
| ImagemRetrato | Foi adicionada em `(-251,0)`, tamanho `100 × 100`, escala `1.1`, fora do encaixe do quadro observado |
| BotaoAvancar | Escala passou de um para aproximadamente `(0.08,0.16,1)` |

Na consulta MCP, o painel tinha largura efetiva de **173.41** e altura de **267.86**, apesar do SizeDelta grande. Com âncoras invertidas, sua largura é `1675.4065 - larguraDoCanvas`: na janela de 1502 pixels, sobra 173.41. A largura diminui quando a janela cresce. O retângulo do TextoFalante chegou a largura negativa.

O Canvas principal usa ConstantPixelSize; o Image da moldura não preserva proporção. A [arte do quadro](../../Assets/sprites/Atualizados/Tela_de_dialogo-removebg-preview.png) tem margens transparentes e uma silhueta desenhada na janela de retrato. O retrato dinâmico está afastado desse encaixe, o que deixa a silhueta aparecendo e o mago separado.

O commit posterior `0279520`, de 16/10, criou Actors, mudou o parent do Player e ajustou um arredondamento de posição do terminal (`217.00002 → 217`). Ele **não introduziu** as alterações de layout acima. As mudanças feitas nesta conversa antes da análise foram documentais. O Git identifica commits e valores; não permite atribuir sozinho a alteração a uma conversa ou agente específico.

**Direção de correção:** reconstruir os RectTransforms do diálogo com âncoras coerentes, texto e botão em escala um, nome habilitado, retrato encaixado e layout que se adapte à resolução. Conferir a proporção e as margens da moldura ao dimensioná-la. Aumentar somente a escala não resolve a causa.

## 4. O sucesso já tem conversa no catálogo

[PuzzleAguaCasaChecker.VerificarVitoria](../../Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs) usa `mensagemDeVitoria`, associada a `TextoApollo`, e atribui `.text` diretamente. Isso produz a frase isolada acima do terminal mostrada na imagem 03.

O [catálogo](../../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs) já contém `CasaPortaApollo`, Id **`casa_porta_apollo`**, com quatro falas de Apollo e Lip. Não é necessário inventar outra mensagem de sucesso.

**Pedido do responsável:** apresentar a vitória como diálogo de Apollo usando o catálogo.

**Recomendação de fluxo:** resolver → registrar a conclusão e liberar a porta → fechar o terminal de forma consistente → abrir `casa_porta_apollo` pelo DialogoUIManager. A transição de UI deve impedir que o Menu do Mundo apareça por trás ou que o bloqueio de movimento fique acumulado. Reentrar na casa/carregar deve restaurar a passagem sem repetir a conversa de vitória.

## 5. A porta não precisa sobreviver como objeto entre cenas

Durante a inspeção em Play Mode, o MCP encontrou:

```text
PuzzleAguaCasaChecker: cena=DontDestroyOnLoad, _puzzleResolvido=True
PuzzleAguaCasaChecker.transicaoDaPortaDeSaida: sem referência viva
TerminalCasa: cena=CasaLip, terminal destruído
PortaDeSaidaCasa.trancada: True
DialogoUIManager._aberto: True
GerenciadorDeEstado._contadorMenusAbertos: 1
```

O contador um é compatível com o diálogo aberto; não demonstra travamento permanente de Lip.

O problema é de duração dos objetos: a porta da casa é recriada, enquanto o checker e o terminal antigos persistem. O checker resolvido não reaplica a vitória à porta nova. O [relato INT-001](casalip-retorno-porta-trancada.md) descreve a mesma estrutura e agora tem confirmação adicional da coleta.

**Recomendação:** persistir `ato1_casa_resolvida` como conclusão do puzzle, separar a flag da introdução (`HIS-001`) e usar um controlador local para reaplicar o estado à porta nova e localizar o terminal persistente válido, inclusive inativo. O [VerificadorDePuzzle](../../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs) já oferece parte desse contrato, mas sua associação precisa de revisão.

Manter a própria porta com DontDestroyOnLoad exigiria controlar sua presença em City, evitar duplicatas ao voltar e resolver posição/colisores. Isso ainda não persistiria seu estado em disco após reiniciar a aplicação. A flag salva atende à necessidade com o ciclo de vida local da porta.

## Próximo trecho de trabalho e critérios

| Trabalho local | Evidência de conclusão |
|---|---|
| `UI-003`: janela de diálogo | Moldura ampla e legível, proporção correta, retrato encaixado, nome e avanço visíveis em pelo menos duas resoluções; texto paginado sem corte |
| `VIS-001`: mundo, retratos e etiqueta | Apollo deixa de usar o mago no mapa/retrato; Lip usa a arte escolhida; nome acompanha Apollo ao mover personagem/câmera |
| `HIS-002`: sucesso pelo catálogo | Resolver abre as falas de `casa_porta_apollo` uma vez; terminal/menu/diálogo não se sobrepõem |
| `INT-001/HIS-001`: estado e referências | Conversar não conclui o puzzle; resolver → City → CasaLip duas vezes mantém porta e terminal; reinício com load preserva a conclusão |

O fechamento da CasaLip também inclui apresentar o código inicial de água e o feedback visual/sonoro do gerador. A avaliação atual continua lendo apenas o valor final: o comando direto mostrado na captura não comprova uso de laço. Não foi decidido mudar essa avaliação nesta análise.

**Limites:** o agente não executou o percurso por input nem o teste de reinício/load. Uma consulta auxiliar de diagnóstico foi rejeitada por usar `GetInstanceID`, obsoleto nesta versão do Unity; foi reexecutada sem essa chamada. Isso foi uma falha do diagnóstico temporário, sem alteração de fontes do projeto.

## Pesquisa

`UI-003`, `VIS-001`, `casa_porta_apollo`, `ae69544`, `ScreenSpaceOverlay`, `ConstantPixelSize`, `anchorMin`, `anchorMax`, `TextoFalante`, `ImagemRetrato`, `Death-Sheet`, `INT-001`.
