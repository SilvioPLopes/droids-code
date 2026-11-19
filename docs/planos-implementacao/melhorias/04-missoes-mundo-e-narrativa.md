# Plano de implementação — Missões, mundo e narrativa

> Execução futura: seguir `superpowers:executing-plans` por entrega, adaptado ao [AGENTS.md](../../../AGENTS.md). Este documento especifica trabalho; não autoriza implementação, Play, montagem de cenas ou migração de saves nesta sessão.

**Estado:** planejamento, 20/10/2026; nenhuma tarefa abaixo implementada ou testada por este plano.

**Objetivo:** tornar as missões acessíveis e persistentes, conectar suas consequências ao mundo e apresentar uma narrativa coerente com as mecânicas realmente disponíveis.

**Arquitetura proposta:** `GerenciadorDeQuests` concentra regras e mutação em memória; os produtores de eventos controlam a persistência no checkpoint apropriado. NPCs oferecem interações, diário/HUD consultam o estado, e componentes existentes de condição de história apresentam as consequências.

**Tecnologias:** C#, Unity, UI/TMP existentes e persistência JSON atual; sem pacote novo, relógio global, servidor ou framework de quests.

**Fontes:** [plano geral](README.md), parecer estático dos 62 scripts realizado nesta conversa, [interface](../../contextos/sistemas/interface.md), [estado e save](../../contextos/sistemas/estado-e-save.md), [combate](../../contextos/sistemas/combate.md), [enredo](../../enredo/TCC_-_ENREDO_v2.md) e [adendo prevalente](../../enredo/TCC_-_ENREDO_v2_ADENDO.md).

## Limites, evidências e decisões

- Escopo atual: somente este documento. As listas de arquivos abaixo delimitam candidatos para execução futura.
- Antes de cada edição futura, registrar no plano os arquivos/objetos exatos, valores anteriores, dependências, aceite e procedimento de reversão.
- Cenas/prefabs serão localizados por GUID dos scripts e componentes no início da entrega; nomes de objetos ainda não confirmados não são instruções de montagem.
- Não modificar Canvas/Scaler compartilhado, geografia, arte, namespaces, GUIDs ou referências alheias para integrar um painel.
- Preservar o checkpoint de exploração e a garantia de recompensa única já existentes; `SalvamentoJson.Salvar()` não pode ser chamado em `Battle`.
- Save pessoal exige backup anterior e restauração verificada; preferir fixtures isoladas. Migração pertence a EST-01; restauração integral pertence a EST-02.
- Entrega de missão tem uma transação local no gerenciador de quests. ECO-01 cuida de compra/venda; compartilhar invariantes de saldo/inventário não cria dependência da loja para progredir.
- Nenhuma descrição em catálogo comprova objeto montado, caminho navegável, input real ou resultado visual aprovado.

| Sintoma sustentado pela análise | Causa ou limite conhecido | Entrega |
|---|---|---|
| Aceitar/entregar quests não forma um percurso de gameplay | APIs sem consumidores de gameplay nos scripts analisados | MIS-02 |
| Conversas e vitórias não avançam objetivos | `RegistrarProgresso` é chamado pelos puzzles; comentários descrevem integrações ausentes | MIS-01 |
| Missão automática depende da abertura do diário ou de uma entrega | Atualização não integrada ao ciclo de estado | MIS-01 |
| Puzzle resolvido antes do aceite perde contribuição | Contadores só recebem eventos para quests ativas | MIS-01 |
| Puzzle repetível não tem rodada executável garantida | Catálogo repetível encontra verificador de uso único | MIS-01 protege o aceite; MEC-06 introduz rodadas depois da demo |
| Integrar vitória diretamente tentaria salvar em Battle | `RegistrarProgresso` chama `Salvar()` | MIS-01 + COM |
| Diário/HUD não acessíveis pelo menu do mundo | Consulta e painel existem sem integração completa | UI-01 |
| NPCs/locais parecem mais completos nos dados que no jogo | Identidade, oferta e ações dos catálogos não comprovam montagem | MIS-02 + MUN-01 |
| Biografias, quests e promessas de mecânica divergem | Versões de conteúdo não reconciliadas | NAR-01 |
| Rotinas não acontecem | `PontoDeRotina` descreve dados; não existe execução desses percursos | MUN-02 |

**D-MIS-01 — proposta de progresso retroativo:** fatos únicos concluídos de conversa e puzzle persistem e podem satisfazer missões aceitas depois. Vitórias repetíveis contam somente após aceite. Puzzle repetível exige rodada identificada e executável; resolver uma vez não autoriza recompensas infinitas.

**D-NAR-01 — decisão editorial pendente:** reconciliar versões seguindo o adendo. A filha de Tácio vive em Prumo no adendo; a história de Nilce exige escolha explícita entre memória do filho, abrigo ou uma sequência que concilie ambos. Não inventar essa sequência silenciosamente.

**D-MUN-01 — proposta de escopo:** demonstrar resposta do mundo por marcos de história primeiro. Rotinas por períodos são opcionais após a demo; relógio contínuo, rotas completas e simulação fora de cena ficam adiados.

**Dependência de progressão:** `q_main_agua` oferece 120 XP, mas isso só ajuda a rota inicial depois de aceitar, registrar três conversas e entregar. PRO-01 valida se essa rota realmente fornece os cinco pontos exigidos por técnica 2 + DEX 3; este plano não presume que 120 XP bastem.

## Cobertura dos scripts e consumidores

| Script existente | Responsabilidade planejada | Consumidores a preservar |
|---|---|---|
| [GerenciadorDeQuests](../../../Assets/Scripts/Historia/GerenciadorDeQuests.cs) | MIS-01/MIS-02: estado, progresso, elegibilidade e entrega | Puzzles, NPCs, combate, diário/HUD |
| [QuestUIManager](../../../Assets/Scripts/Historia/QuestUIManager.cs) | UI-01: consulta, seleção e detalhes | Menu persistente, callbacks de fechamento |
| [NpcInterativo](../../../Assets/Scripts/Historia/NpcInterativo.cs) | MIS-02: identidade e opções de serviço/quest | Loja, diálogo, orientação, proximidade |
| [GatilhoDeDialogo](../../../Assets/Scripts/Historia/GatilhoDeDialogo.cs) | MIS-01/MIS-02: evento de conversa concluída | `CondicoesDeHistoria`, gatilhos, escolhas |
| [DialogoUIManager](../../../Assets/Scripts/Historia/DialogoUIManager.cs) | MIS-02: escolhas e encerramento | Contador de menus, loja após fala, terminal |
| [CondicaoDeHistoria](../../../Assets/Scripts/Historia/CondicaoDeHistoria.cs) | MUN-01: presença e resposta por flags | Alvos serializados das regras |
| [TrancaPorHistoria](../../../Assets/Scripts/Historia/TrancaPorHistoria.cs) | MUN-01: acesso por progresso | Transição de cena, técnica, flags |
| [OrientacaoNpc](../../../Assets/Scripts/Historia/OrientacaoNpc.cs) | Preservar pose nas novas opções | Interação e diálogo; MUN-02 se aprovado |
| [CatalogoDeQuests](../../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs) | NAR-01/MIS-01: objetivos e identidade de rodada | Serviço, diário, NPC, progresso |
| [CatalogoDeNpcs](../../../Assets/Scripts/Catalogos/CatalogoDeNpcs.cs) | NAR-01/MUN: identidade, voz, presença e rotina | NPC, quests, nomes, retratos |
| [CatalogoDeLocais](../../../Assets/Scripts/Catalogos/CatalogoDeLocais.cs) | NAR-01/MUN: conexões e ações documentadas | Montagem futura; não substituir cenas |
| [CatalogoDeDialogos](../../../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs) | NAR-01: conteúdo consistente | Gatilhos; falas no Inspector têm prioridade |
| [MenuMundoManager](../../../Assets/Scripts/Menus/MenuMundoManager.cs) | UI-01: acesso e origem do diário | Todos os painéis persistentes |
| [TelaDeStatusManager](../../../Assets/Scripts/Menus/TelaDeStatusManager.cs) | UI-01: informações completas e legíveis | Mundo e batalha |

Dependências compartilhadas, cobertas também em seus planos: `GerenciadorDeEstado`, `DadosDoJogo`, `SalvamentoJson`, `VerificadorDePuzzle`, `BattleManager`, `SistemaDeProgressao`, `CatalogoDeItens`, `CatalogoDePuzzles` e `CatalogoDeInimigos`.

`ApolloDica` integra ajuda em TER-03; PRO-01 consome essa orientação no tutorial. Este plano exige coerência entre dica, objetivo visível e mecânica disponível.

## Contratos propostos de integração

As assinaturas abaixo são propostas, não APIs já implementadas. Confirmá-las entre entregas antes da primeira edição.

```csharp
// GerenciadorDeQuests: mutação em memória; bool indica mudança efetiva.
bool RegistrarProgresso(TipoDeObjetivo tipo, string alvo,
    int quantidade = 1, string idOcorrencia = null);
bool AtualizarQuestsAutomaticas();
bool Aceitar(string idQuest);   // mantém elegibilidade pública existente
bool Entregar(string idQuest);  // transação local de quest; nenhuma gravação aqui
```

- `RegistrarProgresso` rejeita alvo vazio, quantidade não positiva e eventos derivados de coleta/flag; progresso nunca supera a meta.
- `idOcorrencia` identifica a ocorrência concluída. Vitória usa ciclo monotônico do produtor de batalha, não uma lista ilimitada de GUIDs. Persistir contador emitido no checkpoint de entrada e último ciclo aplicado no checkpoint de encerramento, sob coordenação de EST-01 e COM-01.
- Conversa única usa identidade estável do diálogo/objetivo, associada ao ID do NPC; abrir ou cancelar diálogo não produz fato concluído.
- Puzzle único usa seu ID persistente; rodada repetível inclui ID persistido da rodada fornecido pela integração de puzzles.
- Fatos únicos usam o conjunto finito de IDs de conteúdo; cada produtor sequencial mantém um marcador do último ciclo aplicado. Esses campos entram por migração coordenada com EST-01 quando a capacidade for introduzida; não reservar um número de versão concorrente.
- Aplicar o lote de objetivos de uma vitória antes de avançar seu marcador; nunca descartar o segundo objetivo porque o primeiro já marcou o ciclo. Ciclo menor ou igual ao marcador é ignorado inteiro. Battle mantém também a guarda transitória de encerramento único.
- Esse contrato pressupõe produtores locais, sequenciais e sem eventos atrasados válidos. Não promete processamento global exatamente uma vez perante corrupção, ordenação arbitrária ou restauração de um arquivo externo. Rollback para checkpoint restaura contadores e fatos juntos; novo jogo reinicia ambos. IDs ausentes só são permitidos em caminhos legados convertidos em fatos únicos identificáveis.
- `Aceitar` aplica fatos retroativos elegíveis depois de inicializar os contadores; não reutiliza vitórias anteriores de contratos repetíveis.
- `AoMudarQuests` continua a notificação existente, emitida uma vez após a mutação consistente; assinantes não salvam nem concedem recompensas.
- `Entregar` agrega todos os consumos por ID, valida objetivos, recursos, recompensas e overflow antes de mutar. Captura snapshot local de inventário, gold, progressão, flags e contadores envolvidos; aplica consumo e prêmio juntos; em erro interno, restaura esse snapshot antes de notificar. Marca conclusão antes de liberar reentrada. Exceção de assinante após confirmação não reaplica recompensa nem desfaz a transação confirmada.
- Dono da interação: aplica consequências, reavalia automáticas, confirma a transação e persiste uma vez no checkpoint permitido.
- Vitória: aplicar recompensa e progresso de quest antes da captura do checkpoint final já protegido pelo combate; preservar deduplicação e não criar um autosave paralelo.
- Load/new game: reavaliar automáticas depois da restauração completa; nunca durante desserialização parcial ou pela simples abertura do diário.
- Falha de persistência usa a semântica de EST-02: manter estado consistente em memória, informar que falta gravar e repetir apenas I/O. Não reaplicar recompensas para “tentar salvar novamente”.

## MIS-01 — Ativação, eventos e persistência coordenada

**Prioridade/dependências:** P1; EST-01 e EST-02 para novos dados/restauração; COM-01/COM-02 para o contrato de vitória; PUZ-01 para sucesso comprovado. A loja não bloqueia esta entrega.

**Arquivos permitidos na futura entrega:** `Assets/Scripts/Historia/GerenciadorDeQuests.cs`, `GatilhoDeDialogo.cs` e pontos mínimos nos produtores de puzzle/batalha e no ciclo de estado, acordados com os donos desses planos.

**Criar somente se necessário:** `Assets/Scripts/Historia/CoordenadorDeQuests.cs` para inscrição no ciclo de load/new game/cena; não concentrar roteiro nele. `ControladorDeRoteiro` citado em comentário não foi encontrado como script existente.

**Validação candidata:** `Assets/Editor/ValidacaoQuests.cs`, com entrada `ValidarProgresso()` e menu `DroidsCode/Validacao/Quests/Progresso`; aproveitar o padrão de harness do projeto, sem instalar infraestrutura.

**Passos:**

- [ ] Registrar chamadas existentes de mutação/save e as chaves atuais `quest_{id}_ativa`, `quest_{id}_concluida`, `quest_{id}_obj_{indice}`; preservar compatibilidade.
- [ ] Preparar fixtures de missão automática, conversa única, puzzle único e contrato de três vitórias; registrar as falhas abaixo antes da alteração.
- [ ] Implementar mutações sem I/O e notificação após estado consistente, incluindo entrega e ativação automática.
- [ ] Persistir fatos únicos e deduplicação pelo contrato de EST-01; recuperar apenas fatos demonstráveis em flags/estado antigo, sem inventar conversas já realizadas.
- [ ] Integrar atualização automática ao load/new game completo e às mudanças relevantes de história, sem depender de polling do diário.
- [ ] No encerramento válido de diálogo, registrar o NPC/objetivo correto; encadear os callbacks existentes sem substituir abertura de loja ou escolha.
- [ ] Integrar evento de puzzle somente após sucesso de PUZ-01; evento de vitória no ponto combinado com combate, antes do checkpoint final.
- [ ] Manter `q_ivo_terca` indisponível para segundo aceite enquanto não houver nova rodada executável. PUZ-01 não fornece repetição; MEC-06 é a expansão futura, fora da demo. Exibir motivo sem apagar a primeira conclusão.
- [ ] Executar o harness com fixtures isoladas; depois percorrer uma missão curta por input real quando a integração de NPC estiver pronta.

**Casos e resultados exigidos:**

| Caso do harness | Preparação e asserção |
|---|---|
| `AutomaticaSemDiario` | Ativar `ato1_casa_resolvida`; `q_main_agua` fica ativa antes de abrir UI |
| `ConversaConcluidaUmaVez` | Abrir/cancelar não conta; concluir e reenviar o mesmo ID conta exatamente 1 |
| `PuzzleAntesDoAceite` | Registrar sucesso único sem quest ativa; aceitar depois produz objetivo completo |
| `VitoriaDesdeAceite` | Vitória antes do aceite não conta; três IDs novos após aceite completam meta 3 |
| `VitoriaDuplicadaAposLoad` | Persistir e restaurar ocorrência; reenviá-la mantém contador e recompensa |
| `PuzzleRepetivelExigeRodada` | Segundo aceite sem nova rodada é recusado sem reset; cenário de rodada nova só passa a ser exigido em MEC-06 |
| `DerivadosNaoAcumulam` | Coleta lê inventário atual; vender item reduz disponibilidade; flag lê estado atual |
| `EventoEmBattleNaoSalva` | Aplicar vitória altera quest e gera zero chamadas a `Salvar`; checkpoint final contém ambos |
| `CargaSemEfeitosParciais` | Restaurar completo e reavaliar não concede recompensa nem duplica assinantes |

**Aceite observável:** missão progride após a ação correspondente e sobrevive à recarga; diário fechado não impede avanço; replay do mesmo evento não rende vantagem.

**Reversão:** registrar diff dos produtores e snapshot anterior das fixtures; retirar integração e restaurar os arquivos autorizados em conjunto. Não apagar campos de saves do usuário para reverter código.

## MIS-02 — Oferta, aceite e entrega por NPC

**Prioridade/dependências:** P2, necessária à demo; MIS-01 e decisões de NAR-01 para o conteúdo selecionado. Coordenar invariantes com ECO-01 sem depender do serviço de compra/venda.

**Arquivos:** `NpcInterativo.cs`, `GatilhoDeDialogo.cs`, `DialogoUIManager.cs`, `GerenciadorDeQuests.cs`; prefabs/cenas apenas dos NPCs da fatia aprovada, após inventário exato.

**Contrato:** adicionar `idDoNpc` estável validado por `CatalogoDeNpcs.Obter`; usar `CatalogoDeQuests.DoNpc(idDoNpc)` e consultas existentes. Nome de exibição não é chave de progresso.

**Escolha de interação proposta:** uma lista contextual reúne “Conversar”, “Loja”, entregas prontas e ofertas disponíveis; “Voltar” fecha sem efeitos. NPC com serviço continua oferecendo esse serviço.

**Passos:**

- [ ] Enumerar os NPCs concretos da demo, seus IDs e quem recebe cada entrega, incluindo Apollo para a missão principal; existência no catálogo não dispensa montagem.
- [ ] Registrar valores atuais de tipo, gatilho, callback, tecla, raio e indicadores; identificar se `NpcInterativo` e gatilho escutam a mesma tecla.
- [ ] Criar casos de escolha cancelada, aceite duplicado, entrega sem recursos e entrega duplicada em `ValidacaoQuests.ValidarInteracoes()`.
- [ ] Conectar oferta ao texto e recompensa da quest; aceitar somente por opção explícita, exceto missões marcadas automáticas.
- [ ] Expor entrega apenas ao NPC correto e com objetivos completos; revalidar no clique e consumir/recompensar pela transação local de `Entregar`. Validar consumos agregados: dois objetivos exigindo o mesmo item não podem gastar mais que o inventário.
- [ ] Configurar um único dono do input de interação; quando NPC despacha, usar o gatilho apenas por script.
- [ ] Preservar orientação, diálogo anterior à loja e contador de menus; manter retorno definido após cada ação.
- [ ] Montar primeiro os três interlocutores de `q_main_agua` e o ponto de entrega, depois uma secundária pequena; não montar todo o catálogo na mesma entrega.
- [ ] Percorrer por tecla real: aproximar, conversar, aceitar, cancelar, concluir, entregar e afastar; registrar separadamente o que foi observado em Play.

**Verificação mínima:** executar menu futuro `DroidsCode/Validacao/Quests/Interacoes`; resultado esperado: cancelamento sem mutação, aceite duplicado sem reset, entrega inválida sem perda parcial, entrega repetida sem XP/gold/itens adicionais.

**Aceite observável:** jogador encontra a missão, entende o pedido e recebe recompensa uma vez; a loja continua acessível, e fechar qualquer opção devolve movimento.

**Reversão:** restaurar configurações dos NPCs e callbacks registrados; restaurar fixture de save e confirmar integridade. Não substituir cena inteira se houver mudanças alheias.

## UI-01 — Diário, objetivo atual e status completo

**Prioridade/dependências:** P2; MIS-01 e MIS-02 para dados reais; contratos de interface persistente existentes.

**Arquivos:** `MenuMundoManager.cs`, `QuestUIManager.cs`, `TelaDeStatusManager.cs`; criar `Assets/Scripts/Historia/ObjetivoQuestHud.cs` se não houver componente equivalente. Montagem limitada aos objetos do diário, HUD e campos de status aprovados.

**Contrato:** diário usa `Ativas`, `Concluidas`, `DescreverObjetivo` e `DescreverRecompensas`; HUD usa `ObjetivoAtualParaHud`; abertura/fechamento respeita origem e contador do menu.

**Passos:**

- [ ] Registrar resolução renderizada, escala de visualização da Game View, Canvas/Scaler efetivos e captura anterior dos painéis envolvidos.
- [ ] Identificar todos os consumidores do Canvas e do status; manter Canvas/Scaler fora do escopo e não usar zoom do Editor como explicação da UI.
- [ ] Conectar botão “Missões”, painel e callback; retirar da abertura do diário a responsabilidade de ativar quests.
- [ ] Montar lista rolável com ativas/concluídas, detalhe, recompensas, próximo passo e destinatário da entrega; curiosidade só após conclusão.
- [ ] Definir seleção estável após concluir uma quest e mensagem útil quando listas estiverem vazias; não deixar detalhes de seleção inválida.
- [ ] Montar HUD compacto com objetivo principal; atualização por evento e reconstrução ao load, sem polling de catálogo a cada frame.
- [ ] Ocultar HUD quando disputar espaço com diálogo/terminal/batalha; sem registrar abertura de menu por um texto informativo.
- [ ] Garantir nível, XP atual/próximo nível, gold, HP, atributos, técnicas e pontos no status; campos opcionais existentes devem estar ligados na montagem escolhida.
- [ ] Comparar antes/depois nas resoluções do aceite registradas na execução, com missão de texto longo e lista maior que a área visível.
- [ ] Testar por input real: abrir/fechar diário, alternar status e retornar de cena; verificar terminal, bag, loja e diálogo afetados pelo Canvas compartilhado.

**Validação mínima:** observação de textos com fixture conhecida + comparação visual; não criar teste que apenas espelhe âncoras/valores de layout.

**Aceite observável:** objetivo legível, nenhuma sobreposição/corte, navegação e rolagem disponíveis, painéis relacionados preservados. Se usuário reservar avaliação visual, registrar “edição aplicada; aprovação visual pendente”.

**Reversão:** capturar valores/componentes alterados e diff da montagem antes de editar; restaurar só objetos autorizados e comparar novamente. Reversão não comprova aparência aprovada.

## MUN-01 — Consequências visíveis por marcos

**Prioridade/dependências:** P2 para uma consequência na demo; MIS-01/MIS-02, NAR-01 e flags do percurso escolhido.

**Arquivos:** `CondicaoDeHistoria.cs` e `TrancaPorHistoria.cs` apenas se as regras existentes forem insuficientes; `CatalogoDeNpcs.cs`, `CatalogoDeLocais.cs` e montagem dos alvos delimitados.

**Contrato:** usar `CondicoesDeHistoria.Satisfeitas`; flags permanecem fonte de verdade. `FlagQueFazAparecer`, `FlagQueFazSumir` e `FlagQueDesbloqueia` só têm efeito quando conectadas a objetos concretos.

**Passos:**

- [ ] Escolher um marco demonstrável, como conclusão de uma missão local, com flag produtora, NPC/objeto consumidor e mudança visível descritos no plano.
- [ ] Mapear referência real de cena/prefab e preservar a condição anterior; não converter automaticamente todas as `AcoesPossiveis` em funcionalidades.
- [ ] Configurar regras existentes para antes/depois, mantendo o objeto que reavalia ativo quando ele controla filhos desativados.
- [ ] Se necessário, integrar reavaliação ao evento relevante e ao load; não substituir globalmente o polling sem evidência de necessidade.
- [ ] Atualizar fala/indicador/acesso do marco e informar ao jogador por objetivo ou fala o que mudou.
- [ ] Validar estados antes, depois e após recarga; testar acesso ao local sem cumprir o marco e retorno de outra cena.

**Casos mínimos:** flag ausente mantém estado inicial; flag presente aplica consequência uma vez; load reproduz o mesmo mundo sem recompensar de novo; regra invertida não oculta o controlador necessário.

**Aceite observável:** jogador reconhece uma consequência concreta de sua ação, sem atravessar porta ainda bloqueada nem perder NPC indispensável.

**Limites/reversão:** somente marco e objetos selecionados; sem montar regiões completas. Registrar valores de flags de fixture e regras anteriores; reverter configuração e código juntos, preservando save real.

**Diagnóstico dirigido do vale — 19/11/2026 (sem edição de jogo):** escopo desta leitura: `Assets/Scenes/Game.unity`, `MoradorInterativo`, `GatilhoDeDialogo`, `GatilhoDeBatalha`, `CondicaoDeHistoria` e falas/quests da Cratera; único arquivo editado é este plano. A cena serializa duas instâncias ativas com `idNpc: apollo`: `Apollo` em (-18.75, 6) e `Apollo — painel de energia` em (-13.5, 4). Não há componente `CondicaoDeHistoria` serializado em `Game.unity` para alternar a presença delas; as flags dos gatilhos limitam falas, não a visibilidade dos corpos. `MoradorInterativo` mostra o indicador por proximidade e, sem história/batalha disponível, abre opções com a saudação genérica; isso também se aplica aos droids de batalha e às interações de cenário montadas como moradores. Vários inimigos roteirizados estão ativos na cena com batalhas condicionadas, então a aparência estática e as interações sem efeito narrativo têm causa plausível no contrato atual. Capturas do usuário mostram a sobreposição de rótulos e a duplicação percebida de Apollo; esta leitura não contou instâncias em runtime nem recebeu aprovação visual. Próximo passo: decidir a sequência de presença de Apollo e quais droids são encontros, carcaças ou pistas; antes de editar a cena, registrar objetos/campos, valores anteriores, reversão e validação de estados antes/depois/load. Aceite visível proposto: apenas um Apollo em cada momento, transição compreensível entre suas falas, inimigos distinguíveis de objetos examináveis e indicadores legíveis sem sobreposição. Reversão desta anotação documental: remover somente este parágrafo, preservando alterações posteriores.

**Aplicação delimitada autorizada — 19/11/2026, antes da edição:** o usuário aprovou presença por marcos e pediu escala 0.5 para o Apollo do painel; teste visual será feito por ele. Arquivos permitidos: somente `Assets/Scenes/Game.unity` e este plano; backup da cena em `output/backups/2026-11-19-apollo-vale/`. Objetos permitidos: `DemoAto1` (adicionar um `CondicaoDeHistoria` sempre ativo), `Apollo` fileID `1460676833` (alvo de regra, sem mudança no Transform) e `Apollo — painel de energia` fileID `234058870` (alvo e escala local). Valores anteriores: ambos `m_IsActive: 1`; primeiro em (-18.75, 6, 0), escala (0.5, 0.5, 1); segundo em (-13.5, 4, 0), escala (1, 1, 1). `DemoAto1` fileID `509054203` tem somente Transform `509054204`, sem controlador. Adicionar duas regras: primeiro presente com `ato1_apollo_recusou` e ausente após `ato1_apos_primeiro_combate` ou `ato1_modulo_recebido`; segundo presente com `ato1_modulo_recebido` e ausente após `ato1_leu_painel`. A trava adicional por módulo impede duas aparições até em save com flags incompletas. Usar `CondicaoDeHistoria` existente, sem alterar script, flags, diálogos, puzzles, outros NPCs/droids, Canvas ou Scaler. Dependência compartilhada: o componente usa `CondicoesDeHistoria.Satisfeitas` e `GerenciadorDeEstado` para ler flags; consumidores de `DemoAto1` são apenas seus filhos na cena, mas as regras apontam somente os dois Apollo. Reversão: retirar apenas o componente novo e a referência correspondente em `DemoAto1`, restaurar só a escala local do segundo para (1, 1, 1), conferindo diff e eventual trabalho posterior; backup integral só se ainda corresponder ao estado de origem. Aceite mínimo: zero ou um Apollo visível por marco testado, escala 0.5 nos dois, interação narrativa alcançável no primeiro e no painel, retorno à cena/load reproduzindo presença sem apagar falas; droids e demais objetos preservados. Validação mínima do agente: conferir YAML, referências/IDs, tabela das flags para antes/depois e diff restrito; avaliação visual e input real ficam com o usuário. Estado Unity pré-edição consultado por MCP: projeto conectado com `MainMenu` limpa, fora de Play.

**Resultado da aplicação Apollo — 19/11/2026:** `Game.unity` editada e salva em disco. `DemoAto1` recebeu exatamente um `CondicaoDeHistoria` com duas regras e alvos exclusivos; Apollo da entrada usa recusa/primeiro combate/módulo, Apollo do painel usa módulo/leitura do painel. Escala local do segundo mudou de (1, 1, 1) para (0.5, 0.5, 1); o primeiro já era 0.5. Backup anterior SHA256 `FFD445931BF259ABC38B46D833E651503E48596C24815511CACC1CBDA11D6AAC` em `output/backups/2026-11-19-apollo-vale/Game.unity`; `.meta` preservado. Unity importou e abriu a cena `Game` limpa, fora de Play, sem compilar; consulta confirmou o controlador, as duas regras/flags/alvos e as escalas. Diff restrito e `git diff --check` sem erro; não houve Play, input real, gravação de save nem aprovação visual. Próximo passo: usuário testar antes/depois das flags, retorno ao mapa e enquadramento a 3840×2160 na escala habitual; se houver piora, preservar captura/estado e reavaliar antes de outro ajuste. Droids e outros NPCs não foram alterados; a proposta de distinguir encontro de carcaça segue pendente.

**Reavaliação após teste visual rejeitado — 19/11/2026, antes da nova edição:** o usuário mostrou que a correção anterior não resolveu a duplicação percebida e apontou o boneco grande. Causa sustentada por consulta do Unity fora de Play: há um terceiro `Visual` com o sprite exato `Companion/Apollo/sprite.png` sob `DemoAto1/Unidade de manutenção`, além das duas instâncias nomeadas Apollo. O boneco vermelho grande da captura é `DemoAto1/Rasha e Teodoro`, escala raiz (1, 1, 1), `Visual` (2.2, 2.2, 2.2); o Apollo inicial tem raiz (0.5, 0.5, 1). As duas regras anteriores continuam importadas, mas não controlam o terceiro sprite. Nesta rodada, escopo permitido: somente `Assets/Scenes/Game.unity` e este plano; `Unidade de manutenção/Visual` (desativar o filho visual que falseia a identidade, mantendo root, gatilhos, indicador e falas) e Transform de `Rasha e Teodoro` (escala raiz 1→0.5 em X/Y, Z preservado). A unidade continua como ponto examinável; representação visual própria fica pendente, pois nenhuma arte de carcaça foi identificada no recorte. Dependências: desativar o filho visual afeta apenas o SpriteRenderer da unidade; reduzir a raiz de Rasha também reduz raio de interação e deslocamento dos filhos, a conferir por inspeção e teste do usuário. Antes de editar, criar backup atual em `output/backups/2026-11-19-apollo-vale-rodada2/`; reversão somente dos dois campos, preservando a regra de Apollo e trabalho posterior. Aceite mínimo desta rodada: no máximo um sprite de Apollo visível nos marcos da história, Rasha visualmente próxima da escala do protagonista, unidade ainda interagível quando disponível, sem alterar inimigos nem quests. Validação do agente: diff e importação da cena, sprites/escala/alvos; input e aparência final pelo usuário. Se piorar, suspender novos ajustes e reavaliar a captura.

**Resultado da reavaliação — 19/11/2026:** backup imediatamente anterior SHA256 `2D4D85A971259077A1FD27EAB3D5E1BB5D025981806183B18D31123D36529C25` em `output/backups/2026-11-19-apollo-vale-rodada2/Game.unity`. Prévia MCP apontou exatamente dois alvos; mutação e releitura confirmaram `Unidade de manutenção/Visual` ativo→inativo e `Rasha e Teodoro` escala raiz (1,1,1)→(0.5,0.5,1). A cena foi salva/importada e ficou `Game` limpa, fora de Play, sem compilação; consulta listou as três referências ao sprite de Apollo: dois filhos ativos sob os atores controlados por flags e o terceiro filho inativo sob a unidade. As duas regras de presença anteriores permaneceram. `git diff --check` passou; nenhuma outra cena, script, flag, inimigo ou save foi editado nesta rodada. Não houve teste de input nem aprovação visual; a unidade agora é um ponto examinável sem sprite próprio, pendência visual explícita. Próximo passo: usuário testar em jogo a unicidade de Apollo, escala de Rasha e acesso à unidade; se houver piora, preservar captura/estado e diagnosticar antes de nova mutação. Backups das duas rodadas preservados, sem limpeza pendente; droids estáticos ainda requerem decisão própria.

**Revisão integral autorizada do vale, 19/11.** O inventário no Editor encontrou 15 pontos em DemoAto1, 12 renderizadores na cena, cinco batalhas fixas e uma zona aleatória independente. A montagem de Editor cria os pontos em operações explícitas e recusa reconstruir uma raiz existente. Nas cinco batalhas as flags limitam a luta, mas o corpo e o menu genérico continuam disponíveis. Painel e bomba mantêm o texto ligado fora do alcance. A unidade ainda referencia a arte de Apollo e Ivo ainda usa o dobro da altura de Rasha. O roteiro requer caçadas repetíveis, três escolhas de facção e a volta a Ivo para a manutenção.

Escopo antes da edição: Game.unity (15 pontos de DemoAto1, controlador, visual da unidade e escala de Ivo), novo componente local para selecionar encontros e interação, MoradorInterativo e TerminalDoInterior apenas para delegar entrada quando ligados ao controlador local. Os componentes compartilhados aparecem em 35 cenas/prefabs. Consumidores sem esse controlador seguem pelo input existente. Preservar a zona aleatória, posições, diálogos, recompensas e saves. Reaproveitar o sprite existente de sucata para a unidade. Estado inicial e reversão dos três arquivos existentes em output/backups/2026-11-19-vale-presenca/. Retirar somente os deltas e componente novo se necessário. Aceite: até um Apollo visível, encontro de história prioritário sobre caça com o mesmo modelo, repetíveis acessíveis após a prova, um indicador correspondente ao E, Ivo disponível depois do chefe. Conferir no Editor os objetos realmente ativos em estados de história sintéticos, a altura dos personagens e a disponibilidade dos puzzles. Aprovação visual final e input manual pelo usuário.

**Resultado em Play, 19/11.** Cena e scripts salvos e importados. Os 15 pontos começam inativos e a história ou a disponibilidade do combate os apresenta. A luta da prova tem precedência sobre a caça com o mesmo droid. Ivo, Rasha e as duas aparições de Apollo têm escala 0,5 no plano XY. A unidade usa o recorte de sucata existente com altura 0,55. Em Play, a verificação por API comparou a lista inteira dos pontos ativos com a esperada em 13 estados de entrada, combate, três facções, painel, exame, Alfa e manutenção. Todos coincidiram, com zero ou um Apollo renderizável em cada caso. Dois contatos simulados simultaneamente alternaram um único indicador entre bomba e droid conforme a distância. Foram inspecionadas duas imagens da câmera do jogo com posições e estados preparados, em `output/vale-presenca-2026-11-19/`. A prova cobre disponibilidade e foco, sem simular teclado nem repetir a campanha completa. Save idêntico ao backup depois de sair de Play. Unity fora de Play, sem compilação, Game marcada como modificada na memória ao retornar ao Editor. A origem dessa marca não foi identificada. O arquivo persistente já estava salvo antes do teste. Backups em `output/backups/2026-11-19-vale-presenca/`. Próximo passo de aceite visual reservado ao usuário na campanha.

**Dois combates por aproximação, 19/11.** O pedido substitui o E nos cinco droids físicos do vale. “Tasha” corresponde a Rasha no encontro com Teodoro. Antes desta edição o primeiro combate gravava `ato1_sessao3`, Apollo permanecia na entrada e a sucata da borda virava caça repetível imediatamente. Escopo: Game.unity (Apollo da entrada, encontros, zona aleatória e condição de retorno), InteracoesDoVale, caminho do TesterPlayer e roteiro jogável. O gatilho de batalha compartilhado e o mecanismo de checkpoint já iniciam a luta e registram a vitória. Usar a mesma posição da borda para a segunda luta única, guardar seu marco e manter a caça posterior. A zona aleatória volta após a escolha de facção. Aceite: contato sem E inicia Battle, saída ou derrota não conta como vitória, retorno dentro de encontro não dispara em laço, Apollo sai e volta nas etapas certas e progresso anterior à mudança continua utilizável. Backup prévio dos arquivos e save em `output/backups/2026-11-19-vale-dois-combates/`. Reversão somente pelos deltas desta rodada. Unity estava em MainMenu limpa fora de Play.

**Resultado do percurso de dois combates.** Cena, controlador, tester e roteiro salvos. Em Play, o teste começou com a recusa da City e usou as conversas e os combates reais. O teste posicionou o jogador por API fora e dentro dos círculos, avançou as falas pelos controles de diálogo e usou atributos elevados para encurtar as batalhas. Confirmadas as três entradas em Battle sem E, as duas vitórias com os respectivos checkpoints, a ausência de Apollo entre as lutas, seu retorno sozinho, a aparição de Rasha após a fala e o Selvagem após escolher seguir sem facção. A volta da terceira vitória permaneceu em Game dentro do alcance, sem reinício. Outras facções e balanceamento não foram percorridos nesta rodada. Save anterior restaurado com verificação SHA256 antes e depois da cópia. Backups no diretório registrado acima. Game limpa, fora de Play após reimportação. Próximo passo de aceite visual pelo usuário.

## NAR-01 — Consistência de conteúdo e referências

**Prioridade/dependências:** P2; pode ser preparada independentemente dos contratos de estado. Escolhas editoriais precisam ser registradas antes da alteração de conteúdo.

**Arquivos:** quatro catálogos desta frente e falas serializadas apenas quando localizadas nos objetos aprovados; `Assets/Editor/ValidacaoCatalogos.cs` para referências verificáveis.

**Contrato:** adendo prevalece sobre enredo; IDs persistidos não mudam para corrigir texto. Intenção narrativa, mecânica implementada e montagem disponível permanecem campos distintos na avaliação.

**Passos:**

- [ ] Montar tabela curta de conflitos: origem, trechos envolvidos, versão prevalente e texto final proposto; registrar decisões aqui antes de editar.
- [ ] Reconciliar Tácio/Lúcia com Prumo e filha viva; conferir oferta, entrega e curiosidade de `q_tacio_linha`, não somente biografia.
- [ ] Resolver D-NAR-01 para Nilce e revisar cronologia do Corte, varreduras, filho e uso do quarto sem misturar versões incompatíveis.
- [ ] Aplicar o cânone do Apollo comum no Ato 1: pistas ambíguas, sem confirmar excepcionalidade ou antecipar revelação reservada pelo adendo.
- [ ] Cruzar falas que prometem escaneamento, sobrecarga e adaptação de chefe com as entregas de combate; ajustar promessa ao que existir ou marcar conteúdo indisponível fora da demo.
- [ ] Verificar IDs de NPC, item, puzzle, inimigo e local referenciados por quests; objetivos positivos, recompensas válidas e pré-requisitos alcançáveis.
- [ ] Verificar diálogos por ID e registrar falas de Inspector que sobrescrevem catálogo; não declarar conteúdo corrigido no jogo sem conferir esses consumidores.
- [ ] Implementar `ValidacaoCatalogos.ValidarReferencias()` e menu `DroidsCode/Validacao/Catalogos/Referencias`, retornando localização e ID de cada inconsistência.
- [ ] Executar validação estática; revisar editorialmente os textos alterados e percorrer apenas os diálogos afetados quando houver execução de jogo autorizada.

**Asserções:** ID inexistente falha com origem; objetivo de quantidade zero falha; dependência de missão inexistente ou ciclo impossível falha; referência válida passa. Referência a flag externa exige produtor inventariado, não rejeição automática.

**Aceite observável:** conteúdo da fatia não contradiz o cânone escolhido nem instrui uma mecânica inacessível; nomes e destinos ajudam a encontrar a ação.

**Reversão:** restaurar somente campos de conteúdo/objetos alterados; preservar IDs e migrações. Teste de referências não comprova qualidade literária, cronologia ou aprovação do usuário.

## MUN-02 — Rotinas limitadas após a demo

**Prioridade:** P3, opcional e fora do caminho crítico. Depende de MUN-01 e decisão explícita de incluir ambientação além dos marcos.

**Arquivos candidatos:** `CatalogoDeNpcs.cs`, `CatalogoDeLocais.cs`; criar `Assets/Scripts/Historia/RotinaNpcPorPeriodo.cs` somente se aprovado. `OrientacaoNpc` deve continuar responsável pela pose ao conversar.

**Contrato proposto:** `void AplicarPeriodo(PeriodoDoDia periodo)` seleciona ponto configurado do NPC na cena atual; sem relógio contínuo, teleporte entre cenas ou inventário de simulação global.

**Passos:**

- [ ] Escolher um NPC secundário e dois pontos alcançáveis existentes; definir avanço por marco controlado e fallback para local padrão.
- [ ] Registrar posições/orientação anteriores; impedir mudança enquanto diálogo, loja ou interação estiverem abertos.
- [ ] Aplicar período determinístico após marco e ao carregar, sem deslocar NPC essencial para local inacessível.
- [ ] Validar duas mudanças, recarga e interação durante transição; caso exija navegação nova, registrar ampliação antes de editar.

**Aceite/reversão:** NPC permanece encontrável e interagível; nenhuma missão fica impossível pelo período. Reverter componente e posições registradas; rotas completas e pathfinding exigem outro escopo.

## Refinamentos de interiores solicitados em 13/11/2026

Estas propostas complementam o backlog original; não estão implementadas. O trabalho atual continua em circulação, conforme o [plano de apresentação jogável](../2026-11-12-apresentacao-jogavel.md). Não executar junto com o agente responsável pelas identidades visuais.

### Prédios com 3–4 pavimentos

Pedido: expandir um ou dois prédios hoje pouco exploráveis para uma experiência vertical semelhante à loja de vários andares citada pelo usuário. Referência de escopo: 3–4 pavimentos realmente acessíveis; proposta inicial de quatro no total (térreo + três), com seleção das fachadas antes da montagem.

Candidatos mapeados: E00, E01 e E13 possuem Hall → Patamar → Apartamento01; a escada superior reservada não representa outro andar implementado. Isso é um conjunto candidato, não a decisão de expandir os três. Confirmar fachada, função e o estado atual quando esta entrega for selecionada.

- [ ] Selecionar um ou dois prédios e registrar imagem da fachada, função e quantidade final de pavimentos.
- [ ] Definir cenas/IDs próprios por andar, conexões recíprocas e chegadas seguras. Reutilizar uma matriz visual com variações de uso; não duplicar a mesma cena como se fossem níveis distintos.
- [ ] Definir conteúdo mínimo por andar e sinalização de piso, evitando pavimentos vazios criados apenas para contar andares.
- [ ] Testar rua → todos os andares → rua, ida e volta, com volume integral do jogador, sem laços de escada ou chegadas sobrepostas a portais.

Aceite: os 3–4 pavimentos escolhidos existem no grafo, podem ser percorridos e reconhecidos pelo jogador. Mensagem de escada reservada não comprova expansão.

### Enquadramento do diálogo e das escolhas

Relato atual: diálogo e opções estão mal enquadrados e visualmente inadequados. Usuário pediu **registrar agora e corrigir em outra etapa**, mantendo o foco atual nas movimentações. O diagnóstico anterior está na [revisão de interiores](../2026-10-29-revisao-interiores-dialogos-colisoes.md); não é prova de aparência atual.

- [ ] Coordenar propriedade com o agente de identidade visual antes de editar painéis, templates ou cenas compartilhadas.
- [ ] Registrar captura atual, resolução renderizada, zoom da Game View e configuração efetiva de Canvas/Scaler.
- [ ] Trabalhar no painel/template de escolhas correspondente, com consumidores identificados; evitar ajuste global de Canvas/Scaler para resolver um painel.
- [ ] Validar textos longos, várias missões, retrato, escolhas curtas/longas e fechamento sem prender movimento.

Aceite visível: texto e opções legíveis, enquadrados, sem cortes/sobreposições, com hierarquia coerente e preservação dos outros painéis. Sem alteração visual nesta retomada de circulação.

## Ordem, evidência e retomada

1. Fechar contratos de EST/PUZ/combate e preparar NAR-01 para a fatia selecionada; alinhar invariantes com ECO sem colocar a loja no caminho obrigatório das missões.
2. Executar MIS-01; depois MIS-02 e UI-01 com montagem delimitada.
3. Aplicar um marco de MUN-01 e validar a rota com PRO-01.
4. Avaliar MUN-02 somente após o aceite da demo.

Riscos prioritários cobertos: evento anterior ao aceite (MIS-01), duplicação após load (MIS-01/MIS-02), callback interrompendo loja (MIS-02), painel ilegível ou movimento preso (UI-01), NPC inacessível após marco (MUN-01/MUN-02).

**Resultado desta sessão:** plano documental produzido; código e montagem não editados. Os menus e harnesses descritos são propostas de implementação futura.

**Estado conhecido do Unity:** não consultado; nenhum Play, teste por API, input real ou aprovação visual realizado nesta tarefa.

**Backups:** nenhum save acessado e nenhum backup de runtime necessário nesta sessão. Em execução futura, registrar caminho, checksum/restauração e eventual limpeza pendente na entrega correspondente.

**Pendências:** decisão editorial de Nilce; revisão do contrato de ciclos sequenciais e escolha futura da disponibilidade de rodadas; objetos reais da demo; resoluções e referências visuais para UI-01; evidência de percurso da recompensa inicial em PRO-01.

**Próximo passo:** selecionar o recorte futuro e fechar suas decisões; revisão de IDs, dependências e cobertura consolidada no [plano geral](README.md). Implementação depende de tarefa futura autorizada.
