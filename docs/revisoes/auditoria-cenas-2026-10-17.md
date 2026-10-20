# Auditoria das cenas e objetos — 17/10/2026

**Atualização:** correções implementadas após autorização do usuário; ver a entrega abaixo. O diagnóstico original permanece como evidência do estado anterior. [Retomada vigente](../planos-implementacao/2026-10-17-auditoria-cenas.md).

Escopo e retomada: [plano da auditoria](../planos-implementacao/2026-10-17-auditoria-cenas.md).

## Método e limites

Inspeção dos arquivos atuais das cinco cenas em `Assets/Scenes`, dos cinco prefabs em `Assets/Prefabs` e dos scripts diretamente ligados aos achados. Consultas MCP somente de leitura na CasaLip carregada e no AssetDatabase. Não foram abertas outras cenas, iniciado Play, executados diálogos, gravados saves ou aplicadas correções.

O Unity estava em DroidsCode/CasaLip, fora de Play, sem compilação em andamento e com a cena suja. Portanto, arquivo salvo e objeto carregado são fontes diferentes. Os achados das demais cenas referem-se ao disco, não a uma sessão de jogo observada.

Verificações: 104 GUIDs externos distintos consultados no AssetDatabase; dois sem resolução, detalhados abaixo. Não foram encontrados fileIDs locais não nulos apontando para registros ausentes nos dez arquivos examinados. Isso não valida todos os subassets, eventos, colisões ou caminhos de execução. CasaLip carregada: 92 GameObjects, 14 raízes, zero componentes de script ausentes. Canvas.prefab carregado como asset: 66 objetos e zero scripts ausentes; PainelBag: 5 e zero; PainelStatus: 10 e zero. Contagens de prefab incluem filhos; não somar às cenas sem considerar overrides.

O Console consultado continha três entradas referentes à mesma consulta MCP antiga que não compilou por usar `GetInstanceID`. Não são evidência de três bugs do jogo nem de falha atual de compilação dos fontes. Nenhum build ou teste de comportamento foi executado nesta auditoria.

## Inventário funcional

As cinco cenas estão habilitadas em `ProjectSettings/EditorBuildSettings.asset`.

| Cena | Objetos e sistemas relevantes | Conexões atuais |
|---|---|---|
| MainMenu | Main Camera, Canvas, PainelPrincipal, PainelConfiguracoes, Novo Jogo, Configurações, Sair, Voltar, Slider, GerenciadorMenu e EventSystem | Novo Jogo carrega CasaLip |
| CasaLip | Actors/Player, Apollo (dois GatilhoDeDialogo + OrientacaoNpc; NpcInterativo desabilitado), TerminalCasa, ControlePuzzleAgua, PortaDeSaidaCasa, LipCasa_0, quatro objetos GameObject de montagem, Canvas com menus/terminal/status/bag/droid/diálogo e EventSystem | Porta leva a City; conclusão local reaplicada pelo controlador do puzzle |
| City | Player sob Global Light 2D, Main Camera/CameraFollow, mapa/Grid/Tilemap, Dara, homem da fila, Tácio com Loja, comentário_Apollo, ControleDeHistoria, SaidaCidade, Entrada_CasaLip, marcadores, Canvas compartilhado/diálogo e EventSystem | SaidaCidade → Game; Entrada_CasaLip → CasaLip; NPCs controlam flags da sessão 2 |
| Game | Player sob Global Light 2D, Main Camera/CameraFollow, mapa/Grid/Vale, ZonaBatalha, EntradaCidade, falaqualquer, GameObject com referência TesteBloco1, marcador, Canvas compartilhado/diálogo e EventSystem | EntradaCidade → City; ZonaBatalha → Battle |
| Battle | BattleManager, SpritePlayer, SpriteInimigo, Main Camera, Canvas, barras de HP, log, Atacar/Item/Fugir/Status, listas de ataques/itens, PainelStatus e EventSystem | Volta para Game pelo campo nomeCenaMundo |

Os players de CasaLip, City e Game derivam do asset importado `Assets/sprites/Entities/Characters/Body_A/Animations/Walk_Base/Walk_Down.aseprite`, com componentes e overrides por cena. A hierarquia sob Global Light 2D e nomes genéricos dificultam manutenção, mas, isoladamente, não comprovam um defeito de execução. Não foi feita reorganização.

## Achados prioritários sustentados pela montagem e pelo código

### A01 — Alta: autosave de Battle não representa um ponto de retomada completo

- Evidência: `BattleManager.cs:445` e `:540` chamam `SalvamentoJson.Salvar` ao consumir item e conceder recompensas. `SalvamentoJson.cs:63`–`:87` usa a cena ativa e a posição do objeto com tag Player, ou zero se ausente. Battle não possui objeto com essa tag. O DTO em `DadosDoJogo.cs` não contém o inimigo/turno/contexto de retorno; `Carregar`, linha 265, carrega diretamente `dados.cena`.
- Consequência deduzida: salvar durante Battle grava `cena=Battle` e posição zero; carregar esse arquivo recria uma batalha pelo fluxo de Start, em vez de restaurar o combate ou o ponto do mundo. Um save após vitória também pode voltar a uma batalha nova. A posição de exploração guardada em memória não é preservada nesse arquivo.
- Estado: defeito de contrato identificado por leitura; nenhuma gravação/reprodução foi feita. Lacuna já mencionada no contexto de estado, agora reconfirmada no código atual.
- Validação mínima futura: save de teste isolado, com backup e restauração verificados, cobrindo consumo de item, vitória, encerramento da sessão e carregamento. Definir antes se o produto retoma no mundo ou no combate.

### A02 — Média: Voltar das configurações está sem ação

- Objeto: MainMenu / Canvas / PainelConfiguracoes / Voltar.
- Evidência: `MainMenu.unity:1567`, componente Button, contém `m_OnClick.m_PersistentCalls.m_Calls: []` (linha 1600). `MainMenuManager.FecharConfiguracoes`, linha 33, existe, mas não há associação serializada nem registro por código em `Assets/Scripts`.
- Consequência: a montagem atual não oferece uma ação nesse botão para retornar ao painel principal.
- Estado: falha de ligação comprovada no arquivo; clique real não executado.
- Validação mínima futura: abrir Configurações e usar Voltar com mouse/teclado após ligar o evento.

### A03 — Média: componente de script perdido em Game

- Objeto: Game / GameObject, fileID do componente `475653968`.
- Evidência: `Game.unity:695` referencia GUID `494d1517f11112544a58ae995bad129c`, com identificador `DroidsCode._Teste.TesteBloco1`. O AssetDatabase não resolve esse GUID; não foi encontrado fonte TesteBloco1 na listagem de scripts atual.
- Consequência: esse componente não pode executar. O nome sugere um teste antigo, mas não comprova que seja seguro removê-lo; sua função precisa ser confirmada.
- Estado: referência quebrada comprovada. Game não foi aberta para observar aviso em execução.
- Validação mínima futura: determinar se o teste foi aposentado; remover somente o componente obsoleto ou restaurar sua dependência conforme a decisão.

### A04 — Média: fuga permite novas ações antes da troca de cena

- Objeto: Battle / BattleManager e botões de ação.
- Evidência: `BattleManager.cs:610`–`:625`: no sucesso da fuga, inicia `VoltarParaOMundo(1.5f)` sem encerrar a batalha, mudar `turnoDoJogador` ou desabilitar botões. Atacar e Item verificam justamente essas flags. A coroutine de retorno apenas espera e carrega Game.
- Consequência deduzida: durante 1,5 segundo após o sucesso, novas ações/fugas ainda são aceitas; podem iniciar turnos ou retornos adicionais antes de a cena ser descarregada.
- Estado: caminho lógico identificado; não foi exercitado em Play.
- Validação mínima futura: fugir com sucesso e tentar Atacar/Item/Fugir durante a espera; nenhuma ação adicional deve ser aceita.

### A05 — Média: diálogo introdutório da casa montado no mapa externo sem condições

- Objeto: Game / falaqualquer, posição `(-1, 4)`, ativo, gatilho habilitado.
- Evidência: `Game.unity:235`–`:259`: modo AoEntrarNoTrigger, `idDoDialogo=casa_intro_apollo`, flags necessárias/impeditivas vazias, nenhuma flag ao terminar, apenasUmaVez=true. O bloqueio de repetição do GatilhoDeDialogo é local à instância; sem flag, desaparece ao recriar a cena.
- Consequência deduzida: atravessar esse gatilho pode apresentar novamente a introdução da casa durante a exploração, inclusive em nova visita ao mapa.
- Estado: conteúdo e condições comprovados; intenção narrativa e alcance real pelo jogador ainda precisam ser confirmados. Pode ser um objeto de teste remanescente.
- Validação mínima futura: confirmar a finalidade desse objeto e atravessar sua área numa partida apropriada.

## Inconsistências e riscos que exigem validação específica

### A06 — UI diferente conforme a cena inicial

`Canvas.prefab:2946` usa ConstantPixelSize, scaleFactor=1, referência 800×600. CasaLip carregada confirma essa configuração. Game tem override para ScaleWithScreenSize/Match 0.5 (`Game.unity:4819`), enquanto CasaLip e City herdam a configuração do prefab. Além disso, os PainelDialogo locais têm escalas distintas: CasaLip `(1,1,1)`, Game `(1,0.5,1)`, City aproximadamente `(1.036,0.518,1.036)`.

`MenuMundoManager.Awake` mantém o primeiro Canvas com DontDestroyOnLoad e destrói o novo quando já existe outro. Logo, iniciar Play em Game e chegar a Game pelo fluxo MainMenu → CasaLip → City não equivale a usar a mesma configuração de UI. O painel local de uma cena posterior pode nem ser o painel que aparece em jogo.

Isso comprova divergência de configuração e dependência do percurso, mas não encerra o diagnóstico de legibilidade rejeitada pelo usuário. O Canvas carregado informou pixelRect 3840×2160; a resolução efetiva de renderização da Game View e seu zoom não foram medidos independentemente. Nenhuma comparação visual antes/depois foi feita. Não houve ajuste de Canvas/Scaler. A referência a 1280×720 no contexto da interface não descreve o prefab atual.

### A07 — PainelBag isolado tem referência quebrada, mascarada no Canvas

`PainelBag.prefab:528` aponta prefabBotaoItem para GUID inexistente `22ffa17152be789408e0f022bd9ca831`. Consulta do asset pelo Editor confirma o campo nulo. Entretanto, `Canvas.prefab:5155` sobrescreve o campo com BotaoItemTemplate válido; o BagUIManager da CasaLip carregada também aponta para ele.

Portanto, não é evidência de que a Bag atual da CasaLip esteja quebrada. É um defeito de reutilização do prefab base: instanciá-lo sozinho mantém a referência inválida. Validação futura deve distinguir prefab base e instância sobrescrita.

### A08 — Orientação do Apollo usa uma origem diferente do centro visível do jogador

Consulta atual da CasaLip: Player transform `(-0.568,-0.029)`, escala `(4,4,0)`; collider com offset local `(0,0.31)`, centro mundial aproximadamente `(-0.568,1.211)` e raio 0.6; sprite com centro aproximadamente `(-0.568,1.351)`. Apollo transform `(3.39,0.31)`, sprite com centro `(3.39,0.42)`.

`OrientacaoNpc.OlharPara` compara `interlocutor.position` e `transform.position`, não os centros visuais. A diferença entre pivô e desenho pode tornar a direção escolhida pouco intuitiva em certas posições, mesmo quando a troca de sprite é executada. Os quatro sprites e o renderer do Apollo estão associados; não há Animator nesse NPC.

Isso é uma hipótese geométrica para parte da percepção visual, não a causa comprovada do relato de que Apollo nunca gira. Não houve input E, movimento, interação ou alteração de pose nesta auditoria. Continua válida a pendência de observar o estado na sessão real preparada pelo usuário, registrada no plano da CasaLip.

### A09 — Indicadores de NPCs não acompanham a disponibilidade das falas

Em City, Dara e homem da fila mantêm NpcInterativo do tipo genérico junto com GatilhoDeDialogo/AoInteragir. O primeiro mostra o indicador ao entrar no collider, independentemente das flags narrativas, e sua interação genérica só escreve um log; o segundo decide se há diálogo. Seus indicadores não estão ligados ao campo indicadorDeInteracao do GatilhoDeDialogo.

Risco: mostrar indicação de interação mesmo depois de a fala estar bloqueada pela história. Não afirmar que sempre há duas conversas: o componente genérico não abre uma conversa. Confirmar com input real e estado narrativo antes de alterar a montagem.

## Suspeitas não promovidas a defeitos

- `VerificadorDePuzzle.terminal=null` na CasaLip não é, sozinho, uma referência obrigatória faltante: o componente localiza e associa o terminal por código. A referência do TerminalCasa carregado está preenchida.
- O template da Loja de Tácio é uma instância local, mas está sob Loja, fora do container `lista` que o código limpa. Não foi demonstrada destruição do template por essa rotina.
- Os pontos de chegada não devem ser julgados só pela posição do Transform: City usa Player com escala 2 e CasaLip usa 4, ambos com offset no collider. Não foi comprovado um ciclo imediato de portas nesta análise.
- Retorno de Battle é fixo para Game; a zona montada atualmente também parte de Game. Isso limita batalhas futuras em outras cenas, mas não prova destino errado no percurso atual.
- Escala zero serializada em Canvas de overlay de City não foi tratada como prova de invisibilidade; requer observar o estado efetivo e a renderização.

## Ordem sugerida para o próximo trabalho

Priorizar o contrato do save de Battle; depois a janela de ações após fuga, o botão Voltar e a dependência perdida de Game. Confirmar a intenção de falaqualquer antes de mexer nele. Fazer diagnóstico visual separado para os percursos de UI e para a orientação do Apollo, preservando a montagem e as restrições existentes.

Esta auditoria entrega diagnóstico, não correções. Nenhum achado foi validado com input real nesta sessão e não há nova aprovação visual do usuário.

## Entrega posterior — correções editadas e gravadas

O usuário confirmou que o script de teste não tinha utilidade e autorizou implementar a proposta. Depois solicitou terminar sem novos testes. O registro desta seção substitui o estado de diagnóstico dos achados implementados, sem apagar as evidências anteriores.

| Achado | Alteração aplicada | Limite da entrega |
|---|---|---|
| A01 | Checkpoint antes do combate; estado intermediário em memória; vitória/fuga salvam na origem. GerenciadorDeEstado mantém retorno próprio. Save de Battle recusado antes de aplicar dados; menu exibe erro. | Validação final de retomada e builds pendente. Não recupera automaticamente saves antigos incompletos. |
| A02 | Evento de Voltar em MainMenu aponta a MainMenuManager.FecharConfiguracoes. | Clique real pendente. |
| A03 | Componente órfão TesteBloco1 removido de Game após confirmação do usuário; GameObject e Transform preservados. | Cena não aberta pelo agente para teste. |
| A04 | Encerramento único bloqueia turno/botões/callbacks e fecha listas; fuga limpa contexto e consolida sem prêmio; vitória concede prêmio uma única vez. | Input e tempo de retorno em Play pendentes. |
| A05 | Gatilho e indicador de falaqualquer desativados; conteúdo mantido para a montagem narrativa futura. | A próxima conversa/primeiro combate roteirizado não foram implementados por esta limpeza. |
| A07 | PainelBag base agora aponta a BotaoItemTemplate válido; override do Canvas preservado. | Uso real da lista pendente. |
| A09 | Dara/fila passam o indicador ao GatilhoDeDialogo e desabilitam o NpcInterativo genérico. Tácio preservado. | Input/condições narrativas em City pendentes. |
| A06/A08 | Nenhum ajuste visual aplicado. | UI e orientação do Apollo continuam aguardando a evidência específica descrita no plano. |

Save grava primeiro em arquivo temporário e substitui o destino; falha na entrada impede iniciar a luta. Falha no save final informa o jogador e mantém o resultado em memória para tentar salvar pelo menu do mundo, sem repetir prêmio. Derrota preserva o checkpoint anterior; nenhuma política nova de cura/penalidade foi criada. Battle isolada, sem origem, não grava resultado no save normal.

### Evidência anterior ao pedido de parar testes

- O harness `Assets/Editor/ValidacaoCombate.cs` foi adicionado como comando manual, sem execução na importação. Usa preview descartável, singleton anterior restaurado e arquivos em diretórios temporários próprios, com limpeza em finally; não acessa o save pessoal.
- A versão inicial registrou a ausência das interfaces de checkpoint. Na rodada intermediária da implementação, passaram três casos: recusa de gravar Battle, recusa do legado de Battle e falha de gravação sem preparar entrada. Os outros seis falharam; não há aprovação global da suíte.
- A leitura de checkpoint City foi recusada porque `Application.CanStreamedLevelBeLoaded` retornou false fora de Play. Consulta daquele momento mostrou cinco cenas habilitadas. O código final passou a validar a lista do build.
- Os demais caminhos de encerramento esbarraram na lista de log nula da fixture inativa de preview; isso foi observado por consulta direta ao campo. A fixture final passou a inicializar explicitamente esse campo. Não atribuir essa falha ao comportamento da Battle em partida.
- Esses ajustes finais foram gravados, mas não houve nova execução do harness, teste de compilação dirigido, Play ou operação MCP após a instrução do usuário. A validação final permanece pendente; os resultados intermediários não a substituem.

### Roteiro básico para o usuário executar depois, se desejar

1. Antes de trocar de cena, preservar no Editor as alterações não salvas da CasaLip. O agente não salvou nem descartou essa cena. Aguardar a compilação automática habitual e observar se há erro novo no Console.
2. Abrir MainMenu, entrar em Play, abrir Configurações e clicar Voltar duas vezes em ciclos separados. Esperado: voltar ao painel principal a cada vez. Sair de Play antes de abrir outra cena.
3. Abrir Game e conferir que o objeto do antigo teste não mostra Missing Script. Em Play, atravessar a área de falaqualquer; a introdução da casa não deve abrir.
4. Em uma sessão de teste com o save que se deseja manter previamente copiado, entrar numa batalha pelo mapa. Usar um item e vencer ou fugir. Após sucesso da fuga, tentar clicar outra ação durante a espera; não deve executar. Conferir retorno ao ponto de origem.
5. Para validar persistência, usar uma cópia de teste: interromper uma luta e carregar deve voltar ao estado anterior à entrada; vencer/fugir e carregar depois deve conservar o resultado consolidado. Não usar Battle aberta diretamente para aprovar esse fluxo, pois ela não tem contexto de origem.
6. Na City, observar Dara/fila com uma fala disponível e depois bloqueada pelas flags: o indicador deve acompanhar a disponibilidade. Conferir Bag abrindo sua lista. Esses casos exigem estado narrativo apropriado; não resetar flags para simular aprovação.

Backup de fontes/montagem: `Logs/CorrecaoAuditoria/antes-implementacao.zip`, com manifesto SHA-256. Não é backup do save pessoal. Estado final conhecido do Unity e próximo passo ficam somente no plano vinculado.
