# Plano de implementação — Lua recuperável, venda protegida e Game Over

> **Para os agentes executores:** usar `superpowers:subagent-driven-development`, adaptado ao pedido explícito de **três implementadores simultâneos**, com propriedade exclusiva abaixo. Não criar outros agentes nem cadeias de revisores. O coordenador verifica integração e opera o Unity sequencialmente. Etapas são acompanhadas por checkboxes neste plano.

**Estado e data:** **PAUSADO por pedido explícito do usuário em 2026-10-30 — alerta de fim de tokens.** Implementação salva; não executar mais testes, mutações ou retomada automática. Próxima sessão somente após solicitação do usuário. Todas as derrotas geram Game Over; Continuar usa último save/checkpoint; preços/autosave da loja preservados.
**Objetivo:** tornar a execução Lua recuperável, impedir venda de itens protegidos com balão local e oferecer Game Over em toda derrota, com retorno ao último save/checkpoint ou à tela inicial.
**Arquitetura:** manter MoonSharp encapsulado no runner existente, a guarda comercial no controlador da loja e o fluxo de derrota em uma cena própria. Reutilizar persistência e controles reais; não substituir Canvas compartilhado nem alterar o schema do save.
**Tecnologias:** Unity/C# existentes, MoonSharp instalado, uGUI/TMP e ImageGen integrado para o novo fundo. Nenhuma atualização de pacote, serviço ou infraestrutura.
**Especificação:** pedido do usuário resumido na seção seguinte; diagnóstico de origem na [revisão geral](2026-10-30-revisao-geral-projeto-e-repositorio.md). Este plano delimita a execução futura, sem substituir contratos não envolvidos.

## Especificação e decisões para revisão

### RETOMADA PRIORITÁRIA — parada segura solicitada

- Três implementadores encerraram a escrita. Código, arte, cena e documentos estão salvos; nenhum commit/push. Nenhum save pessoal foi lido ou alterado; não há teste Lua infinito nem Play em andamento.
- **Aplicado:** Lua cooperativo e limitado; guarda de venda protegida; GameOver em toda derrota, Continuação a partir do arquivo e limpeza de efeitos. Arte `Assets/UI/GameOverIlustrado/game-over-fundo-v1.png` e cena `Assets/Scenes/GameOver.unity` existem.
- **Build persistido:** adicionadas somente três linhas de GameOver em `ProjectSettings/EditorBuildSettings.asset`, com GUID `7a0e08f4b2fdb514986735592a975049`; as36 entradas anteriores preservadas. Em memória havia37 entradas. **NÃO executar novamente MontagemGameOver.Aplicar nem acrescentar a cena outra vez.**
- **Verificado:** compilação sem falhas antes da última edição exclusiva de diagnóstico da fixture; `ValidacaoLimiteLua.Executar()` e `ExecutarUI()` passaram; `ValidacaoGameOver.Executar()` passou; `ValidarTela(1280,720)` e `(844,475)` passaram; loja passou em844×475/1280×720, inclusive estoque/Gold/callbacks/paginação. API/render não substituem input físico.
- **Pendência decisiva:** captura limpa da recusa da loja mostra falante/texto, mas corpo/moldura/rabicho do balão não ficaram visíveis. Não declarar aprovação visual nem tentar novos valores. Agente acrescentou SOMENTE diagnóstico na fixture para investigar malha/renderer/Canvas, sem mudar produção. Fonte atual `ValidacaoLojaIlustrada.cs` gera `output/loja-ilustrada/diagnostico-balao-{largura}x{altura}.txt` na próxima execução. Esse diagnóstico ainda NÃO foi coletado. Consultar nome exato do arquivo na fixture se necessário.
- A última importação foi somente dessa fixture diagnóstica; na consulta seguinte o conector desconectou durante recarga. **Último estado confirmado antes dela:** DroidsCode/Battle, fora de Play, limpa. Estado de compilação após essa última importação ainda não confirmado. Nenhuma operação de montagem pendente/repetível foi enviada.
- **Próximo passo quando o usuário pedir retomada:** conferir diff/arquivos e estado Unity; compilar/importar somente se necessário; executar uma vez a fixture loja com City aditiva como fonte e coletar diagnóstico da malha; fechar City sem salvar e restaurar Battle em finally. Corrigir somente após causa demonstrada. Depois inspeção visual, input real e aceite do usuário. Não repetir os testes já verdes sem mudança relevante.
- Backups anteriores em `output/backups/2026-10-30-estabilidade/{game-over,lua,loja,integracao-documental}/`. Snapshot de parada com cópias/hashes em `output/backups/2026-10-30-estabilidade/checkpoint-integrado/2026-10-30-parada-v1/` (manifesto confere as cópias). Nada disso será adicionado ao Git automaticamente.

- A próxima demo **termina no boss do vale**. A ordem narrativa até ele, side quests, recompensas e gatilho definitivo ainda serão definidos em outra tarefa.
- **Toda derrota**, contra boss ou droid comum, abre **Game Over**. Vitória e fuga conservam o fluxo atual.
- Checkpoints: preservar o checkpoint pré-batalha existente e examinar gravações após marcos resolvidos (puzzle inicial/conclusão narrativa já implementada). Acrescentar somente checkpoint ausente de marco comprovado, sem inventar nova sequência narrativa; registrar alvos/valores antes da edição. Sem save/checkpoint recuperável, Continuar informa a ausência e mantém Sair disponível, sem fabricar progresso.
- **Continuar:** carregar o último save/checkpoint efetivamente gravado, restaurando seus dados e a posição de exploração. Hoje a entrada em batalha grava checkpoint; portanto o último save pode ser o instante anterior à luta, e não o último salvamento manual. Nunca salvar o estado derrotado para depois carregá-lo.
- **Sair:** voltar para `MainMenu`, sem encerrar a aplicação e sem salvar, apagar ou substituir o arquivo de progresso.
- Itens protegidos continuam selecionáveis na aba Vender. A tentativa mostra um balão da **Lojista** com **“Não posso comprar esse item.”**, sem consumir item nem alterar Gold e sem fechar a loja.
- Lua recebe execução cooperativa com orçamento, cancelamento ao fechar o terminal e feedback de interrupção. Não executar deliberadamente programas infinitos nem usar `Task.Run`/`Thread.Abort`.
- **Decisão técnica aprovada no lote:** limitar bibliotecas Lua ao conjunto `Basic | GlobalConsts | TableIterators | Math | Bit32`, preservando sintaxe de funções, tabelas, `if`, `while`, `for` e comandos `droid` usados nos exercícios atuais. Bibliotecas avançadas retiradas não serão anunciadas como disponíveis. Uma quota somente na coroutine externa deixa caminhos de contorno demonstrados no pacote instalado.
- **Proposta visual nossa:** droid desativado em ambiente industrial, com área limpa para título e ações. O usuário pediu uma arte de Game Over; personagem/composição são proposta, sem definir a aparência definitiva de Apollo.

## Escopo permitido, preservação e fontes

**Nesta etapa documental:** criar somente este plano e acrescentar sua entrada no [índice](../indice-trabalho.md). Este arquivo não existia; o índice ainda não tinha a entrada. Reversão: retirar somente o arquivo/linha acrescentados, preservando mudanças posteriores.

**Na implementação futura:** apenas arquivos/objetos listados por frente e respectivos `.meta` novos. Antes de editar, conferir diff e salvar baseline/hashes dos arquivos permitidos em `output/backups/2026-10-30-estabilidade/{game-over,lua,loja}/`; registrar inventário aqui. Backups são locais e não devem ser adicionados ao Git automaticamente.

**Preservar:** GUIDs/metas existentes, cenas de exploração e Battle, `Assets/Prefabs/Canvas.prefab`, Canvas/Scaler compartilhados, preços, autosave da loja, schema/versão do save, pacotes, regras de turno, quests/NPCs e histórico Git. A única nova cena é `GameOver`; seu registro não autoriza reserializar as demais.

**Fontes necessárias por frente:** [combate](../contextos/sistemas/combate.md), [estado/save](../contextos/sistemas/estado-e-save.md) e [UI](../contextos/sistemas/interface.md) para Game Over; [TER-01](melhorias/01-terminal-e-puzzles.md#ter-01--execução-limitada-e-recuperável) para Lua; [contrato da loja](2026-10-23-loja-ilustrada.md#retomada-vigente--retorno-de-transações-dentro-da-loja-23102026) e [método visual](../guias/interfaces-sobre-imagens.md) para balão/cena. Operação MCP somente após ler seu procedimento e confirmar projeto/cena/Play/alvo.

## Três agentes simultâneos e propriedade

| Agente | Arquivos exclusivos de implementação | Entrega |
|---|---|---|
| **1 — Game Over e arte** | Alterar `Assets/Scripts/Combat/BattleManager.cs`, `Assets/Scripts/Menus/MenuMundoManager.cs`, `Assets/Scripts/Estado/SalvamentoJson.cs` e `ProjectSettings/EditorBuildSettings.asset`. Criar `Assets/Scripts/Menus/GameOverManager.cs`, `Assets/Scenes/GameOver.unity`, `Assets/UI/GameOverIlustrado/game-over-fundo-v1.png`, `Assets/Editor/ValidacaoGameOver.cs` e respectivos `.meta`. Se necessário à montagem, criar somente `Assets/Editor/MontagemGameOver.cs` e seu `.meta`. | Rota de derrota do boss, cena ilustrada, Continuar/Sair e verificações isoladas. Registro/montagem pelo Editor somente na janela exclusiva coordenada. |
| **2 — Lua** | Alterar `Assets/Scripts/Scripting/DroidScriptRunner.cs`, `Assets/Scripts/Scripting/TerminalDroidApi.cs`, `Assets/Scripts/Menus/TerminalUIManager.cs`. Criar `Assets/Scripts/Scripting/LimitesDeExecucao.cs`, `Assets/Editor/ValidacaoLimiteLua.cs` e `.meta`. | Motor limitado, UI responsiva, cancelamento e feedback, preservando adaptador síncrono também limitado. |
| **3 — Loja** | Alterar `Assets/Scripts/Menus/LojaUIManager.cs`, `Assets/Scripts/Menus/ApresentacaoLojaIlustrada.cs`, `Assets/Scripts/Catalogos/CatalogoDeDialogos.cs` apenas para a fala nova, e `Assets/Editor/ValidacaoLojaIlustrada.cs`. Se indispensável ao rabicho, criar `Assets/Scripts/Menus/BalaoDaLoja.cs` e `.meta`. | Guarda de venda e balão local, sem mudanças de cena/economia. |

O coordenador é dono deste plano e da integração documental. Outros agentes leem os arquivos compartilhados, mas não os editam. Todos recebem a instrução de que não estão sozinhos e devem preservar alterações alheias. Não há dependência de código novo entre as três frentes.

Na implementação, a propriedade documental do coordenador abrange, apenas para contrato/estado efetivamente alterado: `docs/contextos/sistemas/combate.md`, `docs/contextos/sistemas/estado-e-save.md`, `docs/contextos/sistemas/interface.md`, `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md` (TER-01 e referência à entrega) e `docs/planos-implementacao/2026-10-23-loja-ilustrada.md` (exceção de recusa por balão e link). Não copiar a narrativa deste plano para essas fontes.

Baseline desses cinco documentos será preservado em `output/backups/2026-10-30-estabilidade/integracao-documental/`, com hashes, antes da sincronização de contratos. Reversão documental limita-se aos hunks próprios.

## Evidência que sustenta a alteração

| Frente | Causa demonstrada e consumidores | Limite do diagnóstico |
|---|---|---|
| Game Over | `BattleManager.cs:724–768` salva somente vitória/fuga, mas retorna ao mundo inclusive na derrota. `GatilhoDeBatalha.cs:120–141` / `GerenciadorDeEstado.cs:191–209` preparam checkpoint antes da luta. `SalvamentoJson.cs:265–379` restaura o mesmo Droid e limpa contexto/menus, mas não suas listas de efeitos temporários. `MenuMundoManager` persiste seu Canvas e bloqueia Battle/MainMenu; `ValidarDestino` só recusa Battle. | Não houve Play. Nenhum fluxo novo foi testado; o arquivo pessoal não foi lido. |
| Lua | `DroidScriptRunner.cs:25–32` usa `DoString` síncrono. MoonSharp local `3.0.0-beta.1`, commit resolvido `0fb8ba9106c44b140b8f56cb44cb1b50b358897c`, fornece `LoadString`, `CreateCoroutine`, `Resume` e `AutoYieldCounter`. Fontes locais mostram coroutine filha sem herdar quota e chamadas Lua por bibliotecas que desabilitam yield. Consumidores: TerminalUIManager, verificadores de puzzle e fixtures CasaLip/equipamentos. | Explica laço sem término; não comprova `StackOverflowException` CLR. Callback C#, parsing e memória não ganham preempção universal. |
| Loja | `LojaUIManager.cs:163–224` aceita estoque conhecido e remove/paga sem `CatalogoDeItens.PodeSerVendido` (`:443–445`). O feedback atual usa rodapé, não balão. A abertura global de diálogo altera contador/foco; por isso será evitada. Catálogo de itens tem muitos consumidores e não precisa mudar. | Guarda ausente demonstrada no código; não foi reproduzida perda de item em Play nem validada aparência. |

## Riscos que orientam os testes

1. Continuar não pode aplicar o Droid morto, herdar Vazamento/stun ou alterar bytes do último save.
2. Save ausente/inválido, destino Battle/GameOver, HP não recuperável ou clique repetido precisam manter uma saída compreensível, sem gravar progresso.
3. Lua interrompido não pode completar puzzle com variáveis parciais, continuar após fechar ou iniciar dois jobs por duplo clique.
4. Item protegido, definição inválida ou estoque alterado entre seleção/confirmação não podem remover item ou pagar Gold.
5. Balão e Game Over precisam permanecer legíveis, clicáveis e sem sobreposição nas resoluções de aceite; mudança de Canvas global não é solução autorizada.

## Tarefa 1 — Game Over de toda derrota e arte

**Interfaces:** `GameOverManager.Continuar()` e `GameOverManager.Sair()` são callbacks reais dos botões. Usar `SalvamentoJson(string caminho)`, `ExisteSave()`, `DadosDoJogo LerDados()` e `AplicarDadosSemTrocarCena(DadosDoJogo, GerenciadorDeEstado)`. Separar restauração sem troca de cena da navegação, para testar com arquivo temporário. Não chamar `Carregar()` silencioso nem `MainMenuManager.SairDoJogo()`.

- [x] Criar fixture manual `ValidacaoGameOver.Executar()` sem execução ao importar. Confirmar falha do contrato atual com casos isolados de rota do boss e restauração; o harness deve compilar sem depender de tipos novos ainda ausentes, usando inspeção/reflexão quando necessário.
- [x] Em `FinalizarBatalha`, depois de bloquear ações/fechar painéis, rotear toda ocorrência de `!vitoria && !fuga` para GameOver, independentemente do inimigo. Limpar batalha pendente, respeitar a espera e a guarda de finalização única; não conceder recompensa, salvar ou passar pelo retorno ao mundo nesse ramo. Preservar vitória/fuga.
- [x] Implementar Continuar: existência → leitura/validação vigente do DTO → recusa de HP não recuperável/destino inadequado → aplicar dados sem trocar cena → limpar `EfeitosAtivos` e `EfeitosDeDanoAtivos` do Droid restaurado → carregar cena/posição do save. Não curar artificialmente nem gerar save. Preservar migração/leitura de saves antigos válidos.
- [x] Mostrar aviso real na própria tela se o save estiver ausente, inválido ou não recuperável, preservando arquivo e permitindo Sair. Guardar transição contra clique duplo; reabilitar ações quando a tentativa falhar.
- [x] Implementar Sair como retorno a MainMenu sem gravação. Bloquear menus persistentes em GameOver em `MenuBloqueadoNestaCena()`, independentemente do array já serializado. Recusar GameOver em `SalvamentoJson.ValidarDestino`, tanto para salvar quanto para carregar.
- [x] Gerar o fundo com **ImageGen integrado**, inspecionando previamente as referências locais com `view_image`. Copiar o resultado selecionado para o asset novo no projeto; não substituir originais e não usar fallback CLI/API sem autorização específica. Registrar prompt final e origem neste plano.
- [x] Montar exclusivamente GameOver, fora de Play, com Canvas local, EventSystem funcional sem duplicação, título TMP **Game Over**, botões **Continuar** e **Sair**, aviso e foco visível. Acrescentar a cena ao build preservando entradas/ordem atuais. Se necessário o helper `Assets/Editor/MontagemGameOver.cs`, declarar seu efeito antes de executar; ele não pode regravar outras cenas.
- [x] Executar fixture com save temporário: derrota comum e boss → GameOver; vitória/fuga preservadas; Continue restaura HP/energia, equipamentos, inventário, Gold, flags e posição, limpa efeitos e não altera hash do arquivo. Cobrir save ausente, JSON inválido, versão futura, destino indisponível/Battle/GameOver, HP0 e ações repetidas.
- [ ] Após verdes isolados, testar percurso real com backup/restauração de save e avaliar arte/UI. Registrar API, input físico e aprovação visual separadamente.

**Arte/layout propostos:** referências `Assets/sprites/Mapas/mainmenu.jpg`, `Assets/UI/DroidIlustrado/droid-conceito-v2.png` e `Assets/UI/StatusIlustrado/status-conceito-v2.png`. Fundo 16:9 opaco; Canvas novo Overlay/Scale With Screen Size, referência 1920×1080, sem mudar outros Scalers. Proposta de composição: droid à esquerda e região tranquila para título/ações à direita. Textos e controles ficam no Unity.

**Prompt-base:** “Fundo ilustrado horizontal para a tela Game Over de DroidsCode, alinhado às referências de menu e painéis fornecidas. Atmosfera industrial sombria, aço gasto, carvão, ferrugem contida, cobre e latão. Um droid de manutenção desativado, apoiado entre peças de oficina, com silhueta clara e sensor apagado; derrota com possibilidade de reparo. Não copiar o chefe nem definir o desenho definitivo de Apollo. Reservar área escura tranquila para título e duas ações reais adicionadas depois. Sem palavras, letras, números, logotipos, botões, HUD, sangue ou partes orgânicas. Composição 16:9 e fundo opaco.”

**Limite conhecido:** `MainMenuManager.NovoJogo()` atualmente apenas troca de cena e não reinicia o singleton. O retorno solicitado é MainMenu; refazer Novo Jogo está fora deste lote. Registrar essa lacuna antes de avaliar uma entrega pública: chegar à tela inicial não comprova começar uma sessão nova limpa.

## Tarefa 2 — Execução Lua limitada e recuperável

**Interfaces propostas, baseadas em TER-01:** `bool IniciarExecucao(string codigo, LimitesDeExecucao limites)`, `EstadoExecucao AvancarExecucao()`, `void CancelarExecucao()` e `ResultadoExecucao UltimoResultado`, válido ao terminar. Estados: `Executando`, `Concluida`, `Falhou`, `Cancelada`, `LimiteExcedido`. Manter `ResultadoExecucao.Sucesso`/`MensagemErro` e `ObterVariavelNumerica(string)` compatíveis; preservar `ExecutarCodigo(string)` como adaptador síncrono limitado pelo mesmo motor, sem entrada pública com `DoString` desprotegido.

- [x] Criar `ValidacaoLimiteLua.Executar()` manual, sem execução ao importar. Usar somente programas finitos, inclusive na falha anterior à correção: 10.000 iterações com orçamento pequeno devem terminar por limite; programa curto deve retornar resultado correto.
- [x] Criar `LimitesDeExecucao` configurável com valores iniciais já propostos em TER-01: 16.384 caracteres, 1.000 instruções por retomada, 200 retomadas, 100 linhas de saída de até 512 caracteres. Valores serão qualificados, não apresentados como performance medida ou número exato de instruções úteis.
- [x] Montar VM com o conjunto explícito de módulos aprovado acima; recusar fonte maior e prefixo de bytecode `MoonSharp_dump_b64::` antes de compilar. Usar `LoadString` → `CreateCoroutine` → `AutoYieldCounter` → `Resume`, tratando `ForceSuspended` e `Dead` conforme fonte instalada. Não editar PackageCache nem trocar pacote.
- [x] Fazer `TerminalUIManager.Update` avançar no máximo uma retomada por frame; desabilitar Executar enquanto ativo. Fechar, `OnDisable` e troca de contexto cancelam e descartam a execução. Nenhum novo botão/prefab é necessário: fechar continua sendo a ação de cancelamento.
- [x] Encerrar o ciclo `IniciarExecucaoEquipamentos`/`ConcluirExecucaoEquipamentos` exatamente uma vez. Em sucesso, preservar a VM para a próxima execução; em erro, limite ou cancelamento, descartar VM parcial e iniciar sessão limpa na próxima tentativa. Recusar concorrência sem interromper/substituir o job válido.
- [x] Publicar `AoExecutarCodigo` somente no sucesso e impedir leitura de vitória enquanto executando. Preservar verificadores/puzzles atuais; não implementar a reformulação pedagógica PUZ-01/PUZ-02 inteira.
- [x] Limitar a saída da fachada e `print` pelo mesmo coletor, com aviso único de truncamento. Mostrar limite como interrupção conhecida, sem passar a mensagem por ApolloDica como erro desconhecido. Manter o código disponível para correção/reexecução.
- [x] Exibir: **“Execução interrompida: limite de instruções atingido. Confira a condição de parada e o incremento do laço. As variáveis desta sessão foram reiniciadas. Você pode editar e executar novamente.”** Informar efeitos parciais quando houver comandos que alterem o Droid; não prometer rollback integral.
- [x] Validar: cálculo e variáveis entre sucessos; mesmo laço finito com orçamento pequeno/suficiente; exemplos atuais de `while`; recursão finita; erro de sintaxe/runtime; atribuição de puzzle antes de exceder quota sem vitória nem valor residual; cancelamento/reabertura; duplo clique; módulos retirados indisponíveis; saída limitada e próxima execução curta funcionando. Não executar Lua infinito para provar proteção.
- [ ] Conferir UI responsiva por input real e compatibilidade dos consumidores síncronos sem rodar toda fixture de equipamentos que grava saves. Qualificar o alvo de build escolhido antes de declarar compatibilidade Windows/WebGL.

**Limites explícitos:** o rollback atual da fachada cobre inventário/peças/HP, mas atributos, pontos e técnicas podem permanecer alterados. Atomicidade TER-02 fica fora do escopo. Quota não interrompe callback C# arbitrário nem resolve universalmente parsing profundo, memória ou `StackOverflowException`; falta stack trace para atribuir esse erro ao relato. Ajuda do terminal deve informar o conjunto disponível sem redesenhar o painel.

## Tarefa 3 — Venda protegida com balão local

**Interfaces:** manter `CatalogoDeItens.PodeSerVendido(DefinicaoDeItem)` como regra única. Acrescentar `CatalogoDeDialogos.RecusaDeCompraDaLoja()` retornando `FalaDeDialogo` com falante **Lojista** e texto **“Não posso comprar esse item.”**. Acrescentar `ApresentacaoLojaIlustrada.MostrarRecusaVenda(string falante, string texto)` e `LimparRecusaVenda()`.

- [x] Estender `ValidacaoLojaIlustrada` antes da guarda: item protegido selecionável deve recusar confirmação com mensagem exata e inventário/Gold inalterados. Confirmar a falha do comportamento atual em estado sintético, sem save.
- [x] Manter o item protegido visível/selecionável. Em `Vender`, resolver novamente a definição pelo ID e conferir elegibilidade imediatamente antes de remover. ID inválido recusa sem exceção; item protegido retorna antes de transacionar ou reconstruir lista, preservando seleção, aba e rolagem.
- [x] Reutilizar `TentarRemoverItem` como guarda de estoque: falha não paga Gold e atualiza seleção/lista inválida. Uma confirmação vende no máximo uma unidade; confirmação após zerar não cria crédito extra nem estoque negativo.
- [x] Criar balão local, uma única vez, com caixa/moldura, rabicho reconhecível, nome e fala TMP. Sem `DialogoUIManager`, modal, alteração do contador de menus ou fechamento da loja. Elementos decorativos não interceptam raycasts. Ocultar somente o texto de rodapé enquanto a recusa estiver visível.
- [x] Limpar balão ao mudar seleção/aba, abrir ou fechar. Recusa repetida reutiliza o mesmo objeto, sem duplicar listeners. Preservar compra normal, falta de Gold, feedback comum e callback de retorno.
- [x] Executar `ValidacaoLojaIlustrada.Executar(844,475)` e `(1280,720)` com casos protegidos de chave/acesso/recompensa de facção, vendável, última unidade, estoque alterado, definição inválida, tentativas repetidas, callback/menu e conversa já ocupada preservados. Sem escrita de save.
- [ ] Comparar renders antes/depois e testar clique real para seleção, tentativa, repetição e saída. Separar resultado de API, input físico e aceite visual.

**Proposta visual para validar:** referência local 1536×1024; balão em x85/y840, 730×145, abaixo da lista e antes da região Voltar. A proposta decorre da montagem em disco, não de captura atual aprovada. Registrar resolução renderizada, zoom/escala de exibição e Canvas/Scaler efetivos antes de editar; não alterar City/prefab compartilhado como atalho.

**Preço/persistência preservados:** o contrato da loja de 23/10 e sua fixture mantiveram `Preco`, apesar de existir `PrecoDeVendaReal`. A divergência econômica permanece registrada para decisão separada; não mudar valores nesta correção. Não adicionar autosave. A recusa é a exceção nova de apresentação autorizada pelo pedido atual; demais mensagens continuam no rodapé local.

## Execução e integração após revisão do plano

1. Coordenador confirma diff/baseline, arquivos permitidos, isolamento aplicável e estado Unity (projeto correto, cena, Play, compilação e alterações não salvas). Não mudar permissões, autenticação ou pacotes. Registrar backups antes das mutações.
2. Despachar **os três implementadores simultaneamente**, com seus recortes e evidências. Primeiro produzir os casos de regressão seguros; o coordenador executa as verificações de falha no Editor de forma sequencial. Arte pode ser gerada em paralelo. Depois os três implementam seus arquivos exclusivos em paralelo.
3. Nenhum agente inicia Play, salva cena ou executa fixture no Editor enquanto outro opera o conector. Antes da montagem da cena/validação, coordenador confirma scripts estabilizados e compilação sem erros; reserva uma única janela de operação Unity. Timeout exige consultar estado antes de repetir mutação; máximo de duas recuperações fundamentadas da mesma falha.
4. Rodar apenas os verdes das três frentes e a verificação mínima de integração: terminal → fechar sem job; loja → recusa/retorno; derrota boss → GameOver → Continue/Sair. Não repetir todas as auditorias nem corrigir infraestrutura ou bugs novos fora do escopo.
5. Validar GameOver e balão em 1280×720 e 844×475; registrar condições efetivas de render/zoom/Canvas. Aprovação visual é do usuário, separada de propriedades, compilação e testes por API. Se houver piora/rejeição, suspender novos ajustes, preservar evidência e reavaliar antes de alterar novamente.
6. Atualizar somente o estado deste plano e os contratos de sistema efetivamente mudados pelo coordenador. Sem commit/push/limpeza do repositório automáticos. Relatar editado, salvo, testado no comportamento e aprovado visualmente separadamente.

**Save real:** fixtures usam `SalvamentoJson(caminhoTemporario)` e estados isolados, restaurando singletons/contadores. Percurso real da batalha só depois de backup/hash do save e registros associados; registrar o arquivo produzido pelo teste. Restaurar apenas se ele ainda for o produzido pela sessão de teste, nunca sobrescrevendo progresso novo do usuário. Caso não seja seguro restaurar, registrar pendência e não afirmar limpeza concluída.

**Reversão:** cada frente registra seus hunks/arquivos novos. Reverter somente a frente solicitada/autorizada, sem reset/checkout amplo e sem sobrescrever outras alterações. Na frente Lua, motor e UI voltam juntos; em GameOver, remover registro/cena e rota juntos; na loja, remover guarda/balão apenas conforme a autorização. Conferir diff e estado após reversão; isso não comprova recuperação visual.

## Retomada e assuntos posteriores

### Execução autorizada — evidências iniciais

### Freio de segurança e retomada — pedido do usuário em 30/10

**Ponto de retomada durável:** implementação das três frentes salva nos arquivos do projeto, sem commit/push. Arte em `Assets/UI/GameOverIlustrado/game-over-fundo-v1.png`; backups/hashes nas três subpastas já delimitadas. Nenhum save pessoal foi acessado/alterado. Não depender do histórico da conversa ou da memória dos agentes para retomar.

**Freio:** não entrar em Play com rota GameOver sem a cena criada/registrada; não executar percurso que grave saves sem backup/hash; não repetir montagem ou mutação após queda/timeout sem verificar arquivos, Build Settings, cenas carregadas e resultado anterior. Ao reconectar, começar por este trecho, diff e estado do Editor. Não reverter nem descartar progresso por falha de conexão/contexto. Não houve migração/alteração do schema do save.

Snapshot adicional permitido dos arquivos deste lote e seus hashes em `output/backups/2026-10-30-estabilidade/checkpoint-integrado/`, sem saves pessoais, caches ou credenciais. É cópia local do código/arte/cena/documentos, sem commit/push automático; registrar a data/estado do snapshot e não sobrescrever backups anteriores.

**Estado exato no checkpoint:**
- RED das três frentes confirmado (ver abaixo). Produção Lua e loja pronta/salva, GREEN pendente. Lua também fornece `ValidacaoLimiteLua.ExecutarUI()` (API isolada, sem save).
- Produção GameOver e helper salvos; **`MontagemGameOver.Aplicar()` ainda NÃO executado** e cena GameOver ainda não criada neste checkpoint. Não declarar fluxo jogável concluído.
- Revisão do coordenador identificou 17 linhas de validação estrutural global preventiva adicionadas sem evidência. Agente GameOver foi instruído a removê-las, preservando apenas recusa do destino GameOver e limpeza comprovada dos efeitos antes do cálculo de HP. Conferir esse ajuste no diff ao retomar. Ampliação limitada autorizada pelo integrador: limpeza na restauração compartilhada, pois efeitos temporários não estão no snapshot e debuff de VIT pode truncar HP antes da limpeza local posterior; RED demonstrou herança indevida. Não ampliar validações globais nem rejeitar saves legados válidos.
- Ajuste acima concluído e salvo pelo agente: validações preventivas globais removidas; normalização de coleções opcionais/recusa estrutural só em `GameOverManager.PrepararContinuacao`, antes de aplicar no estado real. Os três implementadores encerraram escrita; integração fica exclusivamente com o coordenador.
- Último estado Unity confirmado: projeto DroidsCode, Battle fora de Play e limpa; compilação após import estabilizou sem falha antes dos REDs. Estado após os últimos arquivos de produção ainda precisa ser consultado. City usada como fonte de fixture foi fechada sem salvar; nenhuma montagem de cena existente executada.

**Próximas operações em ordem, sem repetir investigação:**
1. Conferir ajuste do GameOver e fontes salvas; consultar projeto/Play/cena/dirty/compiling/scriptCompilationFailed. Importar somente alterações necessárias e aguardar compilação estabilizar; diante de erro, corrigir a causa delimitada.
2. Executar sequencialmente `ValidacaoLimiteLua.Executar()`, `ValidacaoLimiteLua.ExecutarUI()` e `ValidacaoGameOver.Executar()` (somente estados/saves temporários). Registrar saída completa pertinente, sem afirmar GREEN antecipado.
3. Para loja, abrir City ADITIVAMENTE como fonte, executar `ValidacaoLojaIlustrada.Executar(844,475)` e `(1280,720)`, fechar City SEM salvar e restaurar cena ativa original em finally.
4. Consultar existência de `Assets/Scenes/GameOver.unity` e entrada de build; **somente se ausentes**, executar uma vez `MontagemGameOver.Aplicar()`. Helper é aditivo e salva exclusivamente cena nova; não remontar se já existir.
5. Executar `ValidacaoGameOver.ValidarTela(1280,720)` e `(844,475)` e inspecionar previews; comparar balão normal/recusa da loja. API/render não são input físico nem aprovação visual do usuário.
6. Conferir diff/escopo, atualizar contratos efetivamente mudados e este plano. Testes reais de save/input exigem proteção descrita acima; se não realizados, entregar com essa pendência explícita.

**Última atualização do freio:** primeiro import de produção encontrou CS0619 apenas na fixture da loja (`GetInstanceID`, Unity 6.5). As duas verificações foram substituídas por comparação direta de referência `Transform`; arquivo salvo e import solicitado. Na consulta seguinte o conector desconectou durante recarga/compilação; a chamada era somente leitura e não deve ser tratada como execução de montagem. `MontagemGameOver.Aplicar()` continua NÃO executado. Conferir compilação/estado antes de qualquer nova ação Editor. Código, arte e backups permanecem em disco; GREEN ainda não comprovado.

**Checkpoint posterior, prevalece sobre o anterior:** conexão recuperada; projeto correto, Battle limpa fora de Play, compilação estabilizada sem falha. `ValidacaoLimiteLua.Executar()`, `ExecutarUI()` e `ValidacaoGameOver.Executar()` passaram. `MontagemGameOver.Aplicar()` **EXECUTADO COM SUCESSO**: cena GameOver criada/salva e registrada no build; não executar helper novamente. Loja falhou em fixture com “Sequence contains more than one element”; diagnóstico delimitado em andamento, sem ajuste de produção por tentativa. Nenhum save pessoal tocado.

**Persistência do registro Unity6:** conferência adicional encontrou GameOver apenas em `EditorBuildSettings.scenes` (37), mas ausente no arquivo global `ProjectSettings/EditorBuildSettings.asset` (36). API instalada expõe `globalScenes`; o registro permitido será completado acrescentando somente GameOver ao global se ausente, preservando as36 entradas e o perfil ativo. Não repetir a montagem. Baseline do arquivo já está no backup GameOver. Verificar arquivo e GUID após o setter.

Diagnóstico refinado antes de nova mutação: `globalScenes` também já tinha37 entradas; há exatamente um objeto `EditorBuildSettings` com caminho `ProjectSettings/EditorBuildSettings.asset` e dirty=true. Portanto falta salvar esse objeto, não acrescentar novamente. API instalada confirmou `InternalEditorUtility.SaveToSerializedFileAndForget(Object[],string,bool)`; salvar exclusivamente esse objeto no mesmo caminho, depois conferir diff preservando36 entradas/configurações. Nenhum SaveAssets global autorizado/necessário.

A gravação nativa delimitada não materializou a entrada no arquivo (conferência imediata false; diff vazio). Para encerrar sem ampliar infraestrutura, aplicado append textual **somente** dos três campos da nova entrada já presente no Editor, com GUID conferido do `.meta` (`7a0e08f4b2fdb514986735592a975049`), antes de `m_configObjects`. Não alterou as36 entradas nem configurações existentes. Conferir diff de três linhas e registro em memória na validação final; não repetir montagem nem append.

Nenhuma garantia sobre queda do serviço externo é feita: o mecanismo de recuperação é código/arte salvos, backups, testes e este estado documentado. Não há tarefa em memória cuja perda autorize repetir gravações.

- Backups por frente em `output/backups/2026-10-30-estabilidade/{game-over,lua,loja}/`, com hashes dos arquivos preexistentes, preparados antes das edições de produção.
- RED Lua confirmado por `ValidacaoLimiteLua.Executar()`: “motor ainda não oferece limites configuráveis”.
- RED GameOver confirmado por `ValidacaoGameOver.Executar()`: derrota comum/boss ainda retornava ao mundo; carga herdava efeitos; callbacks ausentes.
- RED loja confirmado por `ValidacaoLojaIlustrada.Executar(844,475)`: “Venda protegida consumiu item ou alterou Gold.” A fixture requer City carregada como fonte; foi aberta aditivamente, fechada sem salvar e a cena Battle restaurada. A tentativa anterior sem fonte falhou na preparação, não contou como regressão comportamental.
- Checkpoints existentes confirmados: `CasaLip.unity` usa `p_agua_casa`/`ato1_casa_resolvida`; `VerificadorDePuzzle.cs:269–278` grava após a resolução. `GatilhoDeDialogo.cs:285–291` grava `flagAoTerminar`; City usa `city_s2_apollo_conclusao`/`ato1_sessao2` após Dara/fila. A entrada em batalha também exige checkpoint gravado. Nenhuma lacuna demonstrada nesses marcos; não acrescentar gravações redundantes nem inventar sequência narrativa.
- Três agentes liberados para produção após suas falhas comportamentais. Sem operação Editor pelos implementadores. GREEN, montagem e aceites ainda pendentes.

- **Planejamento realizado:** três agentes de leitura investigaram independentemente Game Over, MoonSharp e loja; coordenador reuniu causas, contratos, propriedade, aceite e riscos. Código, cenas, assets e saves não foram alterados; nenhuma arte foi gerada ainda.
- **Decisões do usuário:** plano aprovado para execução; todas as derrotas geram GameOver com Continuar/Sair; último save/checkpoints em marcos; Lua estrutural básico; recusa por balão; preços/autosave da loja preservados. Composição e proteção seguem o plano aprovado.
- **Próximo passo:** executar as três frentes simultâneas com a integração descrita. Nenhuma implementação está marcada como concluída.
- **Workspace:** checkout normal `main`, Git dir/common `.git`; trabalho no projeto atual conectado ao Unity, conforme escopo compartilhado do plano aprovado. Não criar outro projeto/worktree ou alterar Git/ignore como efeito secundário.
- **Fora deste lote, para examinar em seguida:** sequência de acontecimentos e gatilho do boss; side quests/recompensas; identidades/modelos dos NPCs versus roteiro diário; presença única e rotinas; estado do script de movimentação; funções já convertidas em prefabs para reutilização em outras sessões. Não presumir que esses sistemas estão implementados nem criar novos gatilhos por inferência.
- **Estado inicial Unity na execução:** confirmado projeto `C:/Users/silve/DroidsCode/Assets`, cena `Assets/Scenes/Battle.unity`, fora de Play, limpa e sem compilação em andamento. **Backups:** cada implementador registra seus arquivos no diretório delimitado antes da edição. **Validação:** fixtures de regressão em preparação; ainda sem alegação de correção ou aceite visual.

## Relato posterior — revisão visual do Game Over e tela cheia (30/10/2026)

O usuário solicitou somente registrar os problemas nesta documentação; isso **não retoma** a execução pausada nem autoriza alterações em código, cena, prefab, arte, configuração ou save. O registro geral dos relatos e dos demais tópicos está no [plano de revisão geral do projeto](2026-10-30-revisao-geral-projeto-e-repositorio.md).

- **Game Over:** relato de que as telas/botões não usam a arte do jogo. Revisar a tela inteira e os controles como conjunto visual, sem assumir qual imagem, botão ou montagem está incorreto. A existência da arte de fundo e da cena registradas acima não demonstra que o resultado visual atende à identidade desejada.
- **Tela cheia/redimensionamento:** ao ativar tela cheia, o redimensionamento deixa visível uma área azul atribuída ao Unity. Sintoma relatado; cena, resolução, modo de tela, área exposta e causa técnica ainda não foram identificados. Capturar condições reais de reprodução e inspecionar o fundo/câmera/UI antes de editar. Não alterar Canvas/Scaler compartilhado nem ajustar cor por tentativa.
- **Aceite antes de qualquer futura correção:** comparar tela Game Over e telas afetadas em janela/tela cheia e nas resoluções reproduzidas; registrar resolução renderizada, zoom da Game View quando aplicável e configuração efetiva do Canvas/Scaler. Confirmar controles legíveis e sem cortes/sobreposições e ausência do azul inesperado, preservando os painéis relacionados. Teste por API ou render isolado não substitui input real nem aprovação visual do usuário.
- **Estado desta atualização:** apenas documentação foi editada. Diagnóstico, reprodução, mudança visual, teste de comportamento e aprovação visual permanecem pendentes. Unity e saves não foram consultados nem operados; estado ao vivo desconhecido. Nenhum backup adicional foi criado.

## Retomada delimitada — três correções da auditoria estática (31/10/2026)

Esta autorização do usuário retoma **somente** os três bugs abaixo; não retoma o restante do plano pausado nem autoriza operação do Unity, edição de cenas/prefabs/GUIDs/CanvasScaler/saves, ou alteração de pacotes.

**Arquivos autorizados:** `Assets/Scripts/Scripting/TerminalDroidApi.cs`, `TabelaDeCustos.cs`, `TecnicaComposta.cs`, `PontosDeProgressao.cs`, `Assets/Scripts/Menus/LojaUIManager.cs`, `ApresentacaoLojaIlustrada.cs`, `Assets/Scripts/Estado/GerenciadorDeEstado.cs`, `Assets/Editor/ValidacaoLojaIlustrada.cs` (autorização posterior explícita para atualizar as expectativas afetadas) e este plano. Os três arquivos de API/loja e a fixture já tinham alterações locais não relacionadas; preservá-las e restringir o diff aos fluxos indicados.

**Dependências compartilhadas:** `TecnicaComposta` calcula custos consumidos pela fachada Lua; pontos são debitados por `PontosDeProgressao`; os valores de preço vêm de `CatalogoDeItens.PrecoDeVendaReal`; a guarda de inventário é compartilhada com loja, equipamento, batalha, Bag e entrega de quest. Não alterar contratos externos nem os consumidores fora dos trechos necessários.

**Estado anterior registrado antes da edição:**

- `TabelaDeCustos` definia coeficientes atuais de dano 1, Stun 3/turno e Envenenamento 2/turno, sem constantes de teto.
- `TecnicaComposta.CustoTotal()` somava/multiplicava em `int`; as entradas Lua aceitavam inteiros positivos sem teto; `TentarAprender` cobrava o resultado e `MelhorarTecnica` alterava primeiro a técnica e tentava reverter se o gasto falhasse.
- `PontosDeProgressao.TentarGastar` não rejeitava quantidade zero/negativa.
- Venda listava, apresentava, mostrava seleção e creditava `item.Preco`, apesar da propriedade `PrecoDeVendaReal`.
- `GerenciadorDeEstado.TentarRemoverItem` consultava estoque antes de rejeitar quantidade não positiva; não havia essa rejeição.
- `ValidacaoLojaIlustrada.cs` já tinha alterações locais; suas duas asserções de crédito da venda esperavam `item.Preco` (`:72,74`), o que deixa a fixture incompatível com o preço de venda solicitado.
- Reversão autorizada: restaurar manualmente apenas as linhas adicionadas/alteradas nesses alvos para esses comportamentos anteriores, sem `git checkout`/reset e sem desfazer alterações locais preexistentes.

**Critério de aceite:** limites B1 aplicados na API de aprendizagem/melhoria; custo verificado e recusa sem mutação de técnica/saldo; gasto não positivo recusado; lista, detalhe, confirmação e crédito da venda usam `PrecoDeVendaReal`; remoção não positiva retorna `false` sem consultar/mutar o inventário.

**Validação mínima:** revisão do diff focada nos sete scripts, verificação de consumidores de `CustoTotal` e, se existir runner C# isolado já configurado, validação dirigida sem Unity nem escrita em saves. Manual no Unity (não executado nesta tarefa): custo/limite e saldo inválido; preço exibido/pago com preço padrão e específico; remoção com quantidades `0`, negativa e `1`.

**Estado desta retomada:** alterações aplicadas e salvas nos sete scripts, mais duas expectativas da fixture da loja atualizadas após autorização do usuário. A mensagem pós-venda também recebe `PrecoDeVendaReal`; mensagens de compra continuam usando `Preco`. `git diff --check` passou; `dotnet build Assembly-CSharp-Editor.csproj --no-restore` compilou as assemblies de produção e Editor com 0 erros e 4 avisos CS0618 de APIs obsoletas em `MoradorInterativo.cs`/`MenuMundoManager.cs` (fora do escopo). A fixture não foi executada e Unity não foi aberto; testes manuais de comportamento continuam pendentes para o usuário. Nenhum backup novo foi criado; valores anteriores e procedimento de reversão estão registrados acima. Unity/saves não foram consultados, estado ao vivo desconhecido. Próximo passo: validar manualmente os três fluxos no Unity, sem alterar cenas/prefabs.
