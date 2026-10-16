# Montagem do Ato 1 no Unity

**Revisão:** 16/10/2026. Este guia usa os scripts existentes e descreve a montagem pretendida. Não indica que o Ato 1 inteiro já esteja configurado ou testado. Comece pelo [contexto](../contexto-projeto.md) e pelas dependências no [plano](../plano-desenvolvimento.md).

Os nomes em código abaixo são os campos serializados; o Inspector pode exibi-los com espaços. Alterações de Editor descritas aqui serão executadas em uma tarefa própria, fora da refatoração documental.

## 1. Preparar a sessão

1. Abrir o projeto em Unity `6000.5.8f1`, aguardar pacotes/compilação e registrar erros de scripts antes da montagem.
2. Conferir MainMenu, CasaLip, City, Game e Battle nas cenas habilitadas do build.
3. Trabalhar fora de Play mode ao alterar objetos e salvar cada cena após uma etapa. Usar uma branch de implementação e preservar `.meta` dos assets movidos.
4. Usar os catálogos de [Assets/Scripts/Catalogos](../../Assets/Scripts/Catalogos) como fonte de Ids e falas. Os itens de facção já existem; não há etapa atual de colar pacotes antigos.
5. Executar primeiro o ciclo básico de validação MainMenu → CasaLip → City → CasaLip. A montagem posterior depende de INT-001/HIS-001 e do controle consistente de UI.

## 2. Canvas e painel de diálogo

O Canvas do MenuMundoManager persiste. Uma instância nova em uma cena pode ser descartada como duplicata. Todo componente local que depende da UI deve resolver a instância válida conforme o fluxo de integração; uma referência para a UI nova da cena pode se perder.

1. Inspecionar o Canvas existente e reaproveitar o painel de diálogo quando já estiver presente.
2. Se o painel não existir, criar `PainelDialogo` dentro do Canvas persistente, com RectTransform ancorado na parte inferior. Criar os filhos `TextoFalante`, `TextoFala`, `BotaoAvancar`, `PainelDeEscolhas` e, se houver retratos, `ImagemRetrato`.
3. Adicionar DialogoUIManager ao PainelDialogo e associar:

| Campo | Alvo |
|---|---|
| `painel` | PainelDialogo |
| `textoFalante` | TMP de TextoFalante |
| `textoFala` | TMP de TextoFala |
| `botaoAvancar` | Button de BotaoAvancar |
| `painelDeEscolhas` | RectTransform de PainelDeEscolhas |
| `prefabBotaoEscolha` | Button do asset BotaoItemTemplate |
| `imagemRetrato` | Image de ImagemRetrato, se usado |

4. Configurar TextoFala com Overflow = Page. Em PainelDeEscolhas, usar Vertical Layout Group e Content Size Fitter, Vertical Fit = Preferred Size.
5. Conferir a inicialização: DialogoUIManager.Start desativa o painel e liga o botão. Manter o componente responsável pela inicialização ativo na carga da cena; deixá-lo inicialmente inativo no mesmo objeto pode adiar Start até a primeira abertura e interferir nela. Separar controlador e painel visual requer uma revisão própria, não uma mudança implícita neste guia.
6. Retratos são procurados em `Assets/Resources/Retratos/<Falante>`; o nome e a capitalização precisam corresponder às falas para o build. A lista `retratosConhecidos` é uma exceção manual.
7. Validar diálogo curto e longo: primeira fala, páginas, escolhas, fechamento e desbloqueio do Player. Consultar VAL-05.

## 3. CasaLip e conclusão persistente

**Pré-requisito: INT-001 e HIS-001.** A montagem atual usa o checker antigo dentro do terminal persistente. Uma troca de componentes sozinha não resolve automaticamente a referência do terminal local nem sua busca quando inativo.

Após corrigir e validar o mecanismo de associação:

1. Manter `Actors` ativo, posição/rotação zero e escala um. Preservar Player, seus componentes, tag Player, layer, transform e prefab. Revisar separadamente a escala Z zero existente.
2. Manter PortaDeSaidaCasa com Collider2D de trigger e TransicaoDeCena ativo. Campo `trancada` marcado no estado inicial não resolvido; destino City e coordenadas de chegada conferidas no mapa.
3. Em objeto local ativo, por exemplo `ControlePuzzleAgua`, configurar o VerificadorDePuzzle conforme a integração corrigida:

| Campo | Valor |
|---|---|
| `idDoPuzzle` | `p_agua_casa` |
| `portaParaDestrancar` | TransicaoDeCena da PortaDeSaidaCasa |
| `flagAoResolver` | `ato1_casa_resolvida` |
| `flagQueJaResolve` | `ato1_casa_resolvida` |
| `terminal` | Associação com a instância persistente válida; seguir a correção de INT-001 |
| `textoDeApollo` | TMP válido, ou usar o log após TER-002 |

4. Remover PuzzleAguaCasaChecker da instância da casa durante a migração e conferir instâncias de prefab/overrides. Garantir que só um checker controla a conclusão.
5. Associar InteragirComTerminalCasa ao terminal válido pelo mecanismo corrigido. BoxCollider2D deve ter Is Trigger marcado.
6. Apollo pode usar `idDoDialogo: casa_intro_apollo`, com `falas` vazio para consumir o catálogo. O diálogo introdutório deve ter uma flag de conversa distinta da conclusão do puzzle, definida em HIS-001.
7. Apresentar ao jogador o código inicial do catálogo; criar um verificador por Id não preenche automaticamente o campo do terminal. O método TextoDoEnunciado também precisa de um consumidor para aparecer na UI.
8. Executar VAL-02/03/04 antes de prosseguir. Essa migração é necessária para o percurso pretendido; seu comportamento de revisita precisa ser validado.

## 4. City: conversas e portão

Existem Dara, homem da fila, Tácio e comentário do Apollo. Conferir os componentes existentes antes de adicionar duplicatas.

1. Para cada NPC, conferir sprite, Collider2D de trigger, tag esperada do Player e posição de interação.
2. Escolher uma única origem de entrada: GatilhoDeDialogo em AoInteragir sozinho, ou NpcInterativo tipo Dialogo chamando um gatilho em ApenasPorScript. Para Tácio, usar NpcInterativo tipo Loja e gatilho ApenasPorScript, mantendo lojaUIManager associado à Loja.
3. Configurar os gatilhos:

| Personagem/evento | `idDoDialogo` | `flagsNecessarias` | `flagsQueImpedem` | `flagAoTerminar` |
|---|---|---|---|---|
| Dara | `city_s2_dara` | `ato1_casa_resolvida` | `ato1_sessao2` | `ato1_falou_dara` |
| Homem da fila | `city_s2_homem_fila` | `ato1_casa_resolvida` | `ato1_sessao2` | `ato1_falou_fila` |
| Tácio | `city_s2_tacio` | Conforme roteiro | Conforme roteiro | Conforme consequência aprovada |
| Comentário do Apollo | `city_s2_apollo_conclusao` | `ato1_falou_dara`, `ato1_falou_fila` | `ato1_sessao2` | `ato1_sessao2` |

4. No comentário, usar AoEntrarNoTrigger, `apenasUmaVez` e `desativarAposConcluir` marcados.
5. Em SaidaCidade, conferir TransicaoDeCena para Game e TrancaPorHistoria com `flagsNecessarias: ato1_sessao2`, `exigirTecnicaConfigurada: true`, `minimoDeAcoes: 2` e `podeTrancarDeNovo: false`.
6. Conferir a fonte de pontos anterior a esse portão. O Droid inicial possui zero pontos, e o checker antigo da casa não concede pontos. A rota normal precisa permitir aprender a técnica mínima; `testeAdicionarPontos` é ferramenta de teste, não a solução de progressão. Registrar HIS-003 antes de aprovar o onboarding.
7. Manter ControleDeHistoria ativo e configurar suas regras somente quando houver alvos e condições definidos. Ele está vazio na cena atual; não pode desativar a si mesmo ou um ancestral que o impeça de reavaliar.

## 5. Game: encontro roteirizado, facção e energia

**Pré-requisitos:** primeiro ciclo validado, HIS-002 e revisão do roteiro. Os blocos abaixo são configuração da sequência técnica herdada; a narrativa v2 precisa de conferência de ordem e consequências antes de declarar o Ato completo.

### Primeiro combate

1. Usar um gatilho de fala com Id `game_s3_apollo_golpes`, aposentando o conteúdo de teste `casa_intro_apollo` em `falaqualquer` quando a nova montagem estiver pronta.
2. Criar um trigger com GatilhoDeBatalha: `idDoInimigo: sucata_captura`, `nomeCenaBatalha: Battle`, `flagsNecessarias: ato1_sessao2`, `flagDeVitoria: ato1_sessao3` e `exigirTecla: false`.
3. Validar vitória, fuga e retorno à posição. A flag de vitória já bloqueia repetição no gatilho; o catálogo sozinho não implementa todas as restrições de encontro.

### Escolha de facção

1. Montar o grupo de Rasha e Teodoro com gatilho Id `game_s4_escolha`, condicionado a `ato1_sessao3` e impedido por `ato1_faccao_escolhida`.
2. No ControleDeHistoria local ativo, usar uma regra que ativa o grupo após a vitória e o desativa após escolher. Alvos devem ser externos ao controlador.
3. Definir `flagAoTerminar: ato1_faccao_escolhida`. Configurar três escolhas no Inspector:

| Escolha | `flagAoEscolher` | `idItemConcedido` | Quantidade |
|---|---|---|---:|
| Flor de Ferro | `ato1_faccao_ferro` | `nucleo_sobrecarga` | 1 |
| Lótus Branca | `ato1_faccao_lotus` | `modulo_ressonancia` | 1 |
| Desgarrado | `ato1_faccao_desgarrado` | Vazio | 0 |

4. Validar uma escolha por partida, sem nova recompensa após revisita/load. As flags são livres; exclusividade depende da montagem e do fluxo.

### Cratera, energia e encontro do Ivo

1. EncounterZone da região: `idDoInimigo: droid_selvagem`, `nomeCenaBatalha: Battle`, `flagsNecessarias: ato1_faccao_escolhida`. Chance/distância exigem playtesting; não há valor final aprovado.
2. Em controlador local ativo, VerificadorDePuzzle: `idDoPuzzle: p_painel_energia`, porta local, e ambas as flags de conclusão com `ato1_puzzle_energia`. A definição exige `energia >= 50` e `porta_aberta == 1`.
3. Apresentar enunciado/código inicial do catálogo pelo diálogo/UI escolhido. O verificador lê resultado final, sem comprovar se o jogador escreveu um if.
4. Configurar as falas do Ivo usando os Ids do catálogo e a ordem narrativa revisada. `ato1_ivo_confessou` deve representar o ponto aprovado de conclusão da conversa.
5. GatilhoDeBatalha técnico do Ivo: `idDoInimigo: droid_do_ivo`, `nomeCenaBatalha: Battle`, `flagDeVitoria: ato1_sessao6`, `exigirTecla: true`. Condicionar a puzzle resolvido e conversa concluída conforme a sequência aprovada, para impedir que E inicie luta antes do diálogo.
6. COM-002 precisa garantir a regra de fuga do chefe; preencher `PermiteFugir` no catálogo ainda não muda o fluxo atual de BattleManager.

## 6. Retorno à City e conclusão narrativa

1. Revisar as falas de retorno do catálogo v2, incluindo consequências por facção e os eventos posteriores. O guia antigo não cobre toda essa versão.
2. Condicionar os gatilhos de retorno a `ato1_sessao6` e à facção correspondente; usar impedimentos persistentes para falas de uma única vez.
3. Definir em HIS-002 qual evento grava `ato1_concluido`. Não considerar automaticamente a antiga revelação do Apollo como encerramento da versão atual.
4. Criar a apresentação de fim de Ato ligada à conclusão aprovada, se fizer parte da entrega.
5. Validar VAL-10 em todas as rotas. O término não pode depender de uma missão opcional, salvo decisão explícita de design.

## 7. Diário e NPCs adicionais

**Pré-requisito: QUEST-001.** O motor e QuestUIManager existem, mas a abertura pelo menu, oferta/entrega e eventos de progresso precisam de código. Não há GatilhoDeQuest disponível para adicionar no Inspector.

O painel existente pode ser montado com os campos reais de QuestUIManager: `painel`, `painelListaDeQuests`, `prefabBotaoQuest`, `botaoVoltar`, `textoTitulo`, `textoResumo`, `textoObjetivos`, `textoRecompensas` e `textoCuriosidade`. Usar container de lista com layout e prefab de Button/TMP; a ligação ao Menu do Mundo deve acompanhar a implementação de QUEST-001, com controle de fechamento/bloqueio.

Para NPCs adicionais, associar identidade, Ids de diálogo, condições e local real. CatalogoDeNpcs.LocalDe/QuemEstaEm só consulta dados: movimentação, relógio e destinos de navegação dependem de NPC-002. Nenhum componente de rotina inexistente deve ser prometido como etapa de Inspector.

## 8. Diagnóstico durante a montagem

| Sintoma | Verificar |
|---|---|
| Terminal indisponível após revisita | Referência local destruída, instância persistente, inicialização e INT-001. |
| Porta volta trancada | Fonte persistente da conclusão, reaplicação na nova porta e autoria da flag. |
| Diálogo não abre ou fecha imediatamente | Referências, inicialização de Start, instância do painel, condições e duas origens lendo E. |
| Escolhas/listas não aparecem | Prefab Button/TMP, container ativo, layout e referências. |
| Personagem não anda | Registrar input, contador, painéis, Rigidbody2D, colisões e foco; sintoma permanece não reproduzido. |
| Puzzle não resolve | Id, variáveis, sessão Lua, associação/evento e flags. Usar logDeDepuracao temporariamente. |
| Sem pontos para técnica no portão | Fonte de progressão anterior à saída; não mascarar aprovação do tutorial com comando de teste. |

Registrar o resultado no [roteiro de validação](validacao-jogo.md), com estado antes/depois e evidência, em vez de marcar a etapa como funcional apenas por salvar a cena.
