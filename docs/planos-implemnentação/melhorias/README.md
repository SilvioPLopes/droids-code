# Plano de implementação — Melhorias do Droids Code

## Atualização de estado — 23/10/2026

Esta atualização documental, solicitada após as entregas de menus, prevalece sobre o estado inicial “implementação não iniciada” abaixo. Preservar as propostas antigas como roteiro, sem tratá-las como descrição do código atual. Navegação/ordem de trabalho no [plano de desenvolvimento](../../plano-desenvolvimento.md); evidências ficam nos planos específicos, sem reproduzir toda a análise aqui.

| Entrega | Estado e fonte atual | Restante |
|---|---|---|
| EST-01 / SAVE-001 | Identidade/payload de quatro peças no save v7 implementados; fixtures de IO/restauração passaram. [Entrega do Droid](../../planos-implementacao/2026-10-23-droid-ilustrado.md) | Percurso real encerrar/reabrir pelo jogo; não recuperar peça que save antigo nunca gravou. |
| INV-01 | Serviço valida estoque, troca/devolve, desequipa e limita HP sem cura; terminal é a origem, Bag/Droid inspecionam. [Comandos/IDs](../../guias/equipamentos-apollo.md) | Input real/aceite visual; não refazer a instalação como botão de Bag. |
| EST-02 | Correção local Salvar/Carregar e feedback no menu entregue; restauração de equipamentos testada | Novo jogo, snapshot da Casa, restauração integral/transação de todos os sistemas continuam propostas. Não marcar o eixo inteiro concluído. |
| TER-02 | Rollback somente de equipamentos/inventário em erro Lua implementado | Transação global de atributos/técnicas/pontos ainda não implementada. |
| UI / artes / inventário | Mundo, terminal, loja, Bag, Status e Droid ilustrados;27IDs/ícones exclusivos | Input/aceite visual por painel; camada vestível precisa de arte/gabarito compatível. |
| COM-04 | Energia100, custo crescente e básico gratuito que recupera10 implementados por decisão posterior; [contrato](../../contextos/sistemas/combate.md) | R1 de cooldown não implementada. Reconciliar com energia antes de somar dois limitadores; proposta antiga “sem sistema paralelo de energia” não descreve mais o jogo. |

**Próximos recortes para a nova janela:** conferir reutilização (PREFAB-001), melhorar somente imagem de Game/vale (VIS-002), refazer menus Battle (UI-004) e estudar padrões de inimigos (COM-003/COM-05) em etapas separadas. [Inventário de prefabs, limites e aceite](../../planos-implementacao/2026-10-17-auditoria-cenas.md#atualização-documental-e-reutilização--23102026). Não criar outro plano concorrente nem renumerar os37itens históricos; os IDs do plano principal mapeiam os recortes novos. Nenhuma implementação/teste/save nesta revisão documental.

**Estado:** planejamento entregue para revisão; conferência documental concluída em 21/10/2026. Implementação não iniciada. A análise dos scripts foi realizada em 20/10/2026.

**Objetivo:** transformar o parecer sobre os 62 scripts de `Assets/Scripts` em entregas pequenas, verificáveis e priorizadas, preservando a identidade de RPG educativo.

**Autorização desta tarefa:** criar somente documentação nesta pasta. Não editar código, cenas, prefabs, pacotes, configurações, saves ou documentos anteriores. Não iniciar Play, testes de gameplay, builds ou operações MCP.

**Arquivos permitidos nesta tarefa:** `README.md`, `01-terminal-e-puzzles.md`, `02-estado-inventario-e-economia.md`, `03-combate-e-progressao.md`, `04-missoes-mundo-e-narrativa.md`, `05-mecanicas-e-demo.md` e `06-validacao-e-entrega.md`, todos nesta pasta.

**Dependências compartilhadas:** os planos futuros abrangem estado/save, terminal, combate, catálogos e UI persistente. Cada plano delimitará consumidores e pontos de coordenação; sua descrição não autoriza alterações nesses sistemas agora.

**Aceite documental:** cobertura dos achados e sugestões do parecer; sequência sem dependências circulares; distinção entre evidência, proposta e decisão pendente; arquivos candidatos, contratos, testes mínimos, critérios visíveis, reversão e retomada por entrega.

**Validação mínima desta tarefa:** conferir links locais, IDs e dependências, consistência dos contratos propostos, cobertura dos 62 scripts e diff restrito à pasta solicitada.

**Fontes iniciais:** [índice de trabalho](../../indice-trabalho.md), [modelo de tarefa](../../planos-implementacao/_modelo-tarefa.md), análise estática realizada nesta conversa e [AGENTS.md](../../../AGENTS.md).

## Atualização solicitada — pontos, recarga e retorno à Casa

**Escopo registrado antes da edição:** atualizar somente este README e os planos 02, 03, 05 e 06 desta pasta. Dependências compartilhadas: orçamento de progressão, técnicas, persistência/restauração e encerramento de combate. Código, cenas, prefabs, saves e narrativa ficam fora desta atualização.

**Direções escolhidas pelo usuário:** rebalancear pontos; adotar recarga; acrescentar tabela de custos; retornar ao último checkpoint na Casa após derrota. Narrativa adiada, sem revisão editorial nesta rodada. Os números, a redistribuição e as proteções complementares serão propostas do plano, sem alegar balanceamento testado ou concordância individual com cada valor.

**Estado anterior e reversão documental:** havia 35 entregas; cooldown ainda era alternativa a energia; não havia tabela fechada de tetos/intervalos; o exemplo de veneno custava 14; derrota restaurava o checkpoint imediatamente anterior à luta. Preservar este registro ao substituir essas propostas. Reversão futura desta documentação deve ser seletiva, pelo diff da rodada, conferindo alterações posteriores; não altera progresso do jogo.

**Aceite da atualização:** tabela de custos/limites/recargas com exemplos calculáveis, redistribuição sem geração de pontos, Casa distinguida do autosave de exploração, narrativa adiada e lista das adições. **Validação mínima:** links, IDs, dependências sem ciclos, consistência dos exemplos e escopo documental. Implementação e playtest permanecem futuros.

## Como usar este plano

| Documento | Conteúdo e entregas |
|---|---|
| [01 — Terminal e puzzles](01-terminal-e-puzzles.md) | Execução limitada, transação da API, contexto de conclusão, editor/ajuda, avaliação por casos e caderno: TER-01/02/03, PUZ-01/02, CAD-01 |
| [02 — Estado, inventário e economia](02-estado-inventario-e-economia.md) | Equipamento no save, restauração integral, troca de peças, compra/venda e consumíveis: EST-01/02, INV-01/02, ECO-01 |
| [03 — Combate e progressão](03-combate-e-progressao.md) | Catálogo, efeitos, custos B1, recarga R1, padrões, Casa, treino e pontos: COM-01 a COM-07, PRO-01/02 |
| [04 — Missões, mundo e narrativa](04-missoes-mundo-e-narrativa.md) | Eventos e entrega, interação com NPC, diário/status, consequências, consistência editorial e rotinas limitadas: MIS-01/02, UI-01, MUN-01/02, NAR-01 |
| [05 — Mecânicas e demo](05-mecanicas-e-demo.md) | Visualização de programa, depuração, scanner, capacidades de peças, decisões de recursos, rodadas e percurso integrado: MEC-01 a MEC-06, DEM-01 |
| [06 — Validação e entrega](06-validacao-e-entrega.md) | Baseline, isolamento, matriz por risco, input/visual, plataformas e encerramento: VAL-01 a VAL-04 |

São **37 entregas propostas**. Cada item especifica propósito, dependências, arquivos candidatos, contrato/passos, aceite e verificação. Regras de reversão e proteção de dados aparecem no eixo e em VAL-01/VAL-04; não se deve executar os 37 itens como um único lote.

Escolher uma entrega, fechar somente suas decisões pendentes, registrar o recorte exato e então implementar em tarefa autorizada. Checkboxes desmarcados indicam trabalho futuro; não são defeitos introduzidos por esta sessão. Nomes de classes/métodos novos estão explicitamente propostos e não devem ser tratados como APIs existentes.

## Parecer e direção recomendada

A base já contém os sistemas centrais de um RPG educativo: atributos e técnicas configuráveis, programação Lua, puzzles, inventário, loja, combate, missões e narrativa catalogada. O maior ganho inicial está em completar as ligações entre esses sistemas e preservar suas consequências entre sessões. Novas mecânicas terão mais valor quando o jogador puder perceber, utilizar e recuperar o resultado das ações que já existem.

O ciclo desejado é: conversar e identificar um problema → programar e observar o resultado → receber uma consequência coerente → preparar o Droid → enfrentar uma decisão legível → salvar e retomar. A demo proposta exercita esse ciclo com uma missão local, um programa defeituoso e um inimigo com intenção visível.

| Achado sustentado pela leitura | Impacto possível / limite da evidência | Resposta planejada |
|---|---|---|
| Lua executa sincronamente e a fachada expõe comando de depuração | Código longo pode bloquear; esta sessão não executou casos perigosos | TER-01/TER-02 |
| Verificadores dependem de valores finais/contexto; evento não distingue adequadamente falhas | Resultado coincidente pode parecer solução; falha não pode concluir puzzle | PUZ-01/PUZ-02 |
| Peças equipadas não compõem o save atual | Identidade/bônus podem se perder na retomada | EST-01/EST-02/INV-01 |
| Loja diverge de preços/restrições do catálogo e não consolida transação | Itens protegidos e progresso econômico ficam expostos a inconsistência | ECO-01 |
| Efeitos passam pelo caminho de ataque, enquanto item/fuga têm fluxo próprio | Regras de turno podem ser contornadas; ácido não afeta a defesa fixa como esperado | COM-02 |
| Inimigo não recebe todos os atributos/restrições do catálogo | Conteúdo descrito não garante comportamento no encontro | COM-01 |
| Custo de veneno ignora magnitude e não há limites completos de domínio | Técnicas desproporcionais e risco aritmético | COM-03 |
| Falta ligação jogável completa de aceite, conversa/vitória e entrega | Missões e pontos iniciais não estão garantidos pelo catálogo | MIS-01/MIS-02/PRO-01 |
| Retorno após derrota não define restauração integral do estado em memória | Preservar o arquivo anterior não basta para uma retomada utilizável | EST-02/COM-06 |
| Catálogos de aulas, mundo e rotina excedem a integração disponível | Texto pode prometer uma mecânica que o jogador não encontra | TER-03/NAR-01/MUN-01/MUN-02 |

Esses achados são de análise estática. A correção de checkpoint e os ajustes da CasaLip já registrados em outras tarefas **não são tratados como ausentes**; suas validações finais continuam no plano de origem. Uma limitação de código não prova sua frequência no jogo real, e um teste de regras futuro não prova diversão ou qualidade visual.

## Prioridades e relação com o trabalho existente

Manter a classificação do [plano de desenvolvimento](../../plano-desenvolvimento.md): P1 para integridade/continuidade/travamento; P2 para integração e validação funcional; P3 para expansão/apresentação sem bloqueio comprovado. A prioridade mede risco/valor, não substitui dependências. Uma integração P2 pode ser pré-requisito de uma correção P1, como a fonte dos pontos iniciais.

| Trabalho vigente | Desdobramento aqui | Cuidado de continuidade |
|---|---|---|
| TER-001 | TER-01/TER-02 | Não executar Lua infinito para comprovar o risco |
| SAVE-001 | EST-01/INV-01 | Manter migração vigente da casa e identidade dos assets |
| SAVE-002 / COM-002 | EST-02/COM-01/COM-06 | Preservar autosave de exploração; acrescentar snapshot da Casa para derrota sem confundi-los. Validação final existente não encerrada aqui |
| HIS-003 | PRO-01 com MIS-01/MIS-02 | Integrar a fonte de XP existente, sem mascarar com pontos de teste |
| QUEST-001 / NPC-001 | MIS-01/MIS-02/UI-01 | Catálogo não comprova montagem dos NPCs |
| COM-001 | COM-02/COM-04 | Recarga escolhida, R1 proposta; proteção contra stun contínuo e ciclo uniforme dos efeitos |
| LOJA-001 | ECO-01 | Proteger itens e consolidar transação uma vez |
| TER-003 / UX-001 | TER-03/PUZ-02/MEC-02 | Ajuda disponível, feedback e avaliação pedagógica observável |
| UI-001/UI-003/VIS-001 / INT-001/HIS-001 | VAL-02/DEM-01 | Respeitar a retomada vigente e a avaliação visual pendente; não declarar aceites antigos satisfeitos |
| NPC-002 / BAL-001 | MUN-02 e playtests de COM/DEM | Adiados até haver fluxo integrado e recorte autorizado |

Não reorganizar namespaces/pastas, migrar catálogos para ScriptableObjects, atualizar pacotes ou remover legado como trabalho incidental. `ItemConsumivel` e constantes possivelmente ociosas só podem ser removidos após confirmar referências e necessidade em tarefa própria.

## Contratos e responsabilidade compartilhada

| Responsável | Contrato com os demais e limite |
|---|---|
| Terminal / TER-01 e TER-02 | Uma execução limitada, um resultado e uma transação de configuração; falha/cancelamento não confirmam gastos. Não muta mundo na simulação de casos |
| Puzzle / PUZ-01 e PUZ-02 | Sucesso validado com contexto e identidade; conclusão única antiga preservada. Casos de avaliação não concedem prêmios individualmente |
| Estado / EST-01 e EST-02 | Migrações sequenciais, ledger, snapshot da Casa não recursivo e restauração completa. Autosave corrente não redefine o ponto de retorno; cada capacidade usa a próxima versão disponível |
| Inventário / INV-01 | Identidade e troca de peças consistentes; HP limitado ao máximo sem cura por alternância. Estado restaurado recalcula atributos antes do HP |
| Economia / ECO-01 | Compra/venda atômicas em memória; nenhuma API genérica de transação de quests é presumida |
| Quest / MIS-01 e MIS-02 | Progresso sem IO; entrega agrega consumo/prêmio e notifica após confirmação. Dono da interação persiste uma vez |
| Combate / COM-01 e COM-02 | Um fluxo de ação e encerramento; emitir lote de progresso antes do checkpoint final. Preservar guarda de encerramento e marcador persistido de ciclo |
| Progressão / PRO-01/PRO-02 e COM-03 | XP pelo caminho existente; B1 e ledger de compras; prévia/reembolso pelo investimento histórico, sem devolver bônus de peça ou conceder nível novamente |
| UI / TER-03, UI-01 e integrações | Apresentar estado; não conceder recompensa por abrir painel nem ativar quests somente ao abrir diário. Um dono por input e por bloqueio |
| Conteúdo / NAR-01 e DEM-01 | IDs estáveis, cânone do adendo e fontes alcançáveis de itens/missões. Objeto real inventariado antes de editar cena |

Arquivos compartilhados de maior conflito: `GerenciadorDeEstado.cs`, `DadosDoJogo.cs`, `SalvamentoJson.cs`, `TerminalUIManager.cs`, `BattleManager.cs`, `TerminalDroidApi.cs`, `GerenciadorDeQuests.cs` e catálogos. Executar suas alterações sequencialmente ou atribuir um único dono de integração; não delegar mudanças concorrentes no mesmo arquivo apenas porque estão em capítulos diferentes.

## Sequência de implementação proposta

VAL-01 é precondição de cada execução autorizada, não uma grande fase de infraestrutura. A tabela abaixo registra dependências de implementação; decisões de design e critérios de validação ficam nas seções próprias. Coordenação entre contratos não significa dependência circular de entrega.

| Entrega | Prioridade | Pré-requisitos de implementação | Resultado do recorte |
|---|---|---|---|
| VAL-01 | P1 | — | Baseline, alvo, evidência e reversão definidos |
| TER-01 | P1 | — | Execução cooperativa limitada e cancelável |
| EST-01 | P1 | — | Equipamento com identidade persistente e migração |
| COM-01 | P1 | — | Atributos/restrições do catálogo efetivos |
| NAR-01 | P2 | — | Conteúdo/referências da fatia coerentes |
| EST-02 | P1 | EST-01 | Carga e nova sessão consistentes |
| COM-02 | P1 | COM-01 | Turno uniforme para ataque/item/fuga |
| COM-03 | P1 | EST-01 para persistir investimento | Custos/limites/reembolso coerentes; regra pura pode ser preparada antes |
| INV-01 | P1 | EST-01, EST-02 | Troca/desequipar sem perda |
| TER-02 | P1 | TER-01, EST-02, COM-03 | Fachada de produção e confirmação atômica |
| ECO-01 | P1 | EST-02, INV-01 | Compra/venda protegidas e persistência coordenada |
| COM-06 | P1 | COM-02, EST-01, EST-02 | Retorno ao último checkpoint registrado na Casa |
| PUZ-01 | P1 | TER-01, TER-02 | Conclusão correta pelo contexto |
| MIS-01 | P1 | EST-01, EST-02, COM-01, COM-02, PUZ-01 | Eventos e progresso sem save intermediário |
| TER-03 | P2 | TER-01, PUZ-01 | Edição recuperável e ajuda disponível |
| CAD-01 | P2 | PUZ-01, EST-01, EST-02 | Programas persistidos com limites |
| COM-04 | P2 | COM-02, COM-03 | Recarga por técnica/família conforme R1 |
| INV-02 | P2 | EST-02, INV-01 | Consumíveis/retorno com efeito explicado |
| MIS-02 | P2 | MIS-01, NAR-01 para o conteúdo usado | Aceite e entrega jogáveis |
| PRO-01 | P1 | MIS-01, MIS-02, COM-03 | Cinco pontos acessíveis pela missão inicial |
| PRO-02 | P2 | COM-03, TER-02, EST-01, EST-02 | Prévia e redistribuição de pontos na Casa, sem duplicação |
| UI-01 | P2 | MIS-01, MIS-02 | Diário, objetivo e status legíveis |
| MUN-01 | P2 | MIS-01, MIS-02, NAR-01 | Uma consequência visível e persistente |
| PUZ-02 | P2 | PUZ-01, TER-03, TER-02 | Avaliação por múltiplos casos |
| COM-05 | P2 | COM-01, COM-02; COM-04 se o padrão usar recarga | Padrões de ação anunciados |
| MEC-01 | P2 | PUZ-02, TER-03 | Saída do programa compreensível visualmente |
| MEC-02 | P2 | PUZ-02, TER-03, MEC-01 na demo | Um desafio de depuração e dicas |
| DEM-01 | P2 | PRO-01, MIS-02, UI-01, MUN-01, NAR-01, COM-05, COM-06, MEC-01, MEC-02, CAD-01, INV-01, ECO-01 | Percurso completo da fatia |
| VAL-02 | P2 | VAL-01 e cada item selecionado, por recorte | Casos afetados observados; não bloqueia início de todos os demais |
| VAL-03 | P2 | TER-01, EST-02 e percurso integrado | Plataforma aprovada conserva comportamento/progresso |
| VAL-04 | P2 | DEM-01, VAL-02, VAL-03 nos alvos escolhidos | Aceite final com limites de evidência claros |
| MEC-03 | P3 | COM-02, COM-04, COM-05 | Scanner com utilidade tática |
| COM-07 | P3 | COM-01, COM-02, COM-03, COM-04 | Treino finito na Casa, sem alterar progresso ou conceder prêmios |
| MEC-04 | P3 | EST-01, INV-01, PUZ-02; MIS-02 se obtido por quest | Peça habilita capacidade optativa |
| MEC-05 | P3 | PUZ-02, MIS-02, MUN-01, NAR-01 | Escolhas técnicas com consequências |
| MEC-06 | P3 | PUZ-02, CAD-01, MIS-01, MIS-02, EST-02 | Rodadas repetíveis sem duplicação |
| MUN-02 | P3 | MUN-01; iniciar após VAL-04 | Um NPC secundário e dois destinos por marco |

**Ondas de entrega:**

1. **Base confiável:** selecionar os primeiros recortes TER-01, EST-01 e COM-01. Validar cada recorte, sem disparar a implementação inteira. NAR-01 está adiada por solicitação do usuário; não abrir revisão editorial nesta rodada.
2. **Estado e regras:** EST-02, COM-02/COM-03, INV-01, TER-02, PUZ-01, ECO-01 e COM-06. Reconciliar contratos compartilhados antes de integrá-los.
3. **Progressão jogável:** MIS-01/MIS-02, PRO-01/PRO-02, TER-03, UI-01, CAD-01 e MUN-01. O jogador deve conseguir avançar, redistribuir pontos e retomar pelo percurso normal; conteúdo dependente de decisões editoriais aguarda retomada própria de NAR-01.
4. **Interesse e demonstração:** PUZ-02, MEC-01/MEC-02, COM-04/COM-05, DEM-01 e validação de saída. INV-02 entra quando seu item for exposto no percurso.
5. **Expansões por evidência:** treino COM-07, MEC-03 a MEC-06 e MUN-02 depois de avaliar a demo. Priorizar conforme bloqueios e interesse observados, sem estimativa artificial de prazo/tokens.

## Decisões escolhidas, propostas numéricas e temas adiados

O roteiro contém recomendações concretas, mas não transforma preferência de design em aprovação do usuário. Estas decisões são fechadas no recorte correspondente; não impedem analisar ou revisar os demais documentos.

| IDs | Recomendação para revisão | O que falta fechar antes de executar |
|---|---|---|
| D-TER-01/02/03 | Execução cooperativa; configuração em cópia; contrato da casa preservado e novos casos optativos | Limites iniciais e política de cancelamento/rollback confirmados no recorte |
| D-EST-01/02 | Identidade/atributos de peças; restauração completa; novo jogo sem apagar silenciosamente o slot anterior | Política visível do primeiro save da nova campanha e tratamento de legado não reconstruível |
| D-INV-01/02 | Troca devolve peça, HP não cura por alternância; sinalizador explicitamente retorna ao checkpoint | Aceite do comportamento do sinalizador e sua apresentação |
| D-COM-01/02 | Recarga escolhida pelo usuário; R1 com espera 1/2/3, famílias e proteção contra stun contínuo | Intervalos e imunidade são propostas concretas para playtest futuro; manter ciclo uniforme de efeitos |
| D-COM-03/04 | Casa escolhida como retorno; B1 define custos e tetos iniciais | Snapshot separado, descanso/rollback integral e valores B1 detalhados como proposta; validar migração e IO na implementação |
| D-PRO-01 | Manter 120 XP e cinco pontos iniciais; acrescentar redistribuição na Casa | PRO-02 propõe reembolso integral de investimentos comprovados, com prévia e checkpoint atualizado na confirmação |
| D-MIS-01 | Fatos únicos retroativos; vitórias depois do aceite; rodadas explícitas | Aceite do contrato de eventos sequenciais e limites de deduplicação |
| D-NAR-01 | Tema adiado pelo usuário nesta rodada | Preservar o registro anterior sem revisar texto ou solicitar decisões narrativas agora |
| D-MUN-01 | Um marco visível primeiro; rotina limitada posterior | Objeto/NPC e mudança local exata da fatia |
| D-MEC-01/02/03/04 | Depuração com resposta visual, fatia pequena, dicas graduais, rodadas controladas | Conteúdo do exercício, fonte do filtro, montagem da demo e disponibilidade/recompensas de repetição futura |
| D-VAL-01 | Validar nos alvos de distribuição efetivamente escolhidos | Confirmar Windows, WebGL ou ambos; disponibilidade do ambiente |

Outros dados necessários à execução: nomes/hierarquia exatos de objetos, valores anteriores e imagens de referência dos painéis. São inventário de montagem a registrar no início da tarefa correspondente; não justificam abrir ou modificar cenas durante este planejamento. B1/R1 são propostas numéricas completas para o primeiro recorte, ainda sem playtest. Narrativa permanece adiada; não bloqueia elaborar custos, recarga ou checkpoint.

## Adições desta atualização

1. **Rebalanceamento B1:** cinco pontos por nível como orçamento inicial, custo por atributo/dano e veneno proporcional à magnitude×duração com coeficiente 1; stun custa 3 por turno. Exemplo anteriormente proposto de 14 passa a 8 pontos, sem reprecificar reembolso legado.
2. **Limites explícitos:** dano direto até nível 10; veneno até 3 de dano por 3 turnos; stun por 1 turno; até dois efeitos distintos por técnica; quatro técnicas customizadas; até dez incrementos comprados por atributo.
3. **Tabela R1 de recarga:** básico livre, dano direto com 1/2/3 oportunidades de espera conforme faixa, veneno no mínimo 2 e stun no mínimo 3.
4. **Proteções de controle:** família de recarga impede bypass por cópias renomeadas; imunidade temporária do alvo impede stun contínuo por técnicas alternadas.
5. **Prévia de custo e saldo:** mostrar gasto total, diferença, saldo final, recarga e efeito antes de confirmar, com cancelamento sem mutação.
6. **PRO-02 — redistribuição na Casa:** devolução integral do investimento comprovado em técnicas/atributos comprados, sem devolver bônus de equipamento nem gerar pontos. Ledger e checkpoint atualizados juntos.
7. **Retorno à Casa:** snapshot completo separado do autosave corrente; proposta de descanso para registrar e de rollback integral ao último ponto, sem penalidade extra de XP/gold. Tratar ausência, corrupção e falha de gravação.
8. **COM-07 — treino seguro:** simulação finita de cinco turnos, com relatório de dano/efeitos/recarga em cópias, sem XP, itens, progresso ou save.
9. **Validações novas:** fronteiras B1/R1, orçamento 2+3, legado 6 versus preço 8, duplicação de reembolso, cópias de stun, snapshot não recursivo e retorno durável à Casa.

Recarga e destino Casa são decisões expressas do usuário. Custos específicos, limites, redistribuição gratuita, famílias, imunidade e treino são propostas acrescentadas, sem confundir sua inclusão no plano com implementação ou balanceamento aprovado em jogo.

## Cobertura dos 62 scripts

O inventário abaixo relaciona cada script analisado ao plano ou à preservação exigida. “Preservar” significa consumidor conhecido/regressão de integração, não uma sugestão de editar o arquivo. O escopo real de cada entrega será menor que este inventário.

| Pasta em `Assets/Scripts` | Script | Destino da análise |
|---|---|---|
| Camera | `CameraFollow.cs` | Preservar enquadramento; VAL-02 em transição/layout afetados |
| Casa | `ControladorPuzzleAguaCasa.cs` | PUZ-01; manter contrato da casa e flags separadas |
| Casa | `InteragirComTerminalCasa.cs` | TER-03/PUZ-01; origem e bloqueio de interação |
| Casa | `PuzzleAguaCasaChecker.cs` | PUZ-01; migração do evento sem mudar solução aceita |
| Catalogos | `CatalogoDeDialogos.cs` | NAR-01/PRO-01; coerência e tutorial alcançável |
| Catalogos | `CatalogoDeInimigos.cs` | COM-01/COM-05; atributos, restrições e padrões |
| Catalogos | `CatalogoDeItens.cs` | ECO-01/INV-01/INV-02; MEC-04 opcional |
| Catalogos | `CatalogoDeLicoes.cs` | TER-03/MEC-02; ajuda conectada ao terminal |
| Catalogos | `CatalogoDeLocais.cs` | NAR-01/MUN-01/MUN-02; dados versus montagem |
| Catalogos | `CatalogoDeNpcs.cs` | MIS-02/NAR-01; MUN-02 opcional |
| Catalogos | `CatalogoDePuzzles.cs` | PUZ-02/MEC-02/MEC-06; casos e rodadas |
| Catalogos | `CatalogoDeQuests.cs` | MIS-01/MIS-02/PRO-01/NAR-01; fontes/recompensas |
| Combat | `BattleManager.cs` | COM-01 a COM-06; produtor de MIS-01 |
| Combat | `CombatEngine.cs` | COM-02/COM-04; ação e ciclo de efeitos |
| Combat | `EncounterZone.cs` | COM-01/COM-06; entrada, único e rearme |
| Combat | `InimigoFixo.cs` | COM-01/COM-02/COM-05; atributos efetivos e ações |
| Combat | `IParticipanteDeCombate.cs` | COM-02; preservar consumidores ao mudar contrato |
| Combat | `ResultadoAcao.cs` | COM-02; resultado explícito de turno/encerramento |
| Droid | `Braco.cs` | EST-01/INV-01; slot, identidade e atributos |
| Droid | `Cabeca.cs` | EST-01/INV-01; slot, identidade e atributos |
| Droid | `Droid.cs` | TER-02/EST-02/COM-02/COM-03; estado e invariantes |
| Droid | `DroidPart.cs` | EST-01/INV-01; MEC-04 opcional |
| Droid | `DroidStats.cs` | COM-02/EST-01; recomposição de atributos e HP |
| Droid | `Perna.cs` | EST-01/INV-01; slot, identidade e atributos |
| Droid | `Tronco.cs` | EST-01/INV-01; slot, identidade e atributos |
| Estado | `DadosDoJogo.cs` | EST-01; capacidades de CAD-01/MIS-01 em migrações sequenciais |
| Estado | `GerenciadorDeEstado.cs` | EST-02/ECO-01/MIS-01; aplicação e persistência coordenadas |
| Estado | `ISistemaDeSalvamento.cs` | EST-02/VAL-03; preservar contrato e substituir IO em fixture |
| Estado | `SalvamentoJson.cs` | EST-01/EST-02; validação, migração e falhas |
| Historia | `ApolloDica.cs` | TER-03/MEC-02; orientação consumida por PRO-01 |
| Historia | `CondicaoDeHistoria.cs` | MUN-01/NAR-01; condições e produtores de flags |
| Historia | `DialogoUIManager.cs` | MIS-02/TER-03; callbacks, foco e visual pendente |
| Historia | `GatilhoDeBatalha.cs` | COM-01/COM-06; regras e retorno sem nova luta imediata |
| Historia | `GatilhoDeDialogo.cs` | MIS-01/MIS-02; conversa concluída e produtor único |
| Historia | `GerenciadorDeQuests.cs` | MIS-01/MIS-02; eventos, atomicidade e entrega |
| Historia | `NpcInterativo.cs` | MIS-02; identidade, serviços e input único |
| Historia | `OrientacaoNpc.cs` | Preservar em MIS-02/MUN-02; pose não é rotina |
| Historia | `QuestUIManager.cs` | UI-01; diário sem responsabilidade de ativação |
| Historia | `TrancaPorHistoria.cs` | PRO-01/MUN-01; requisitos acessíveis e retorno |
| Menus | `BagUIManager.cs` | INV-01/INV-02; peças, itens e descrição |
| Menus | `LojaUIManager.cs` | ECO-01; filtro, preço e transação |
| Menus | `MainMenuManager.cs` | EST-02; novo jogo/carga e tratamento de falha |
| Menus | `MenuMundoManager.cs` | UI-01/EST-02; diário, origem e bloqueios |
| Menus | `TelaDeDroidManager.cs` | INV-01/TER-03; consumidor de peças e configuração |
| Menus | `TelaDeStatusManager.cs` | UI-01/PRO-01; informações reais da progressão |
| Menus | `TerminalUIManager.cs` | TER-01/TER-03/PUZ-01/CAD-01; ciclo e edição |
| Scripting | `DroidScriptRunner.cs` | TER-01/TER-02/PUZ-01; VM, transação e resultado |
| Scripting | `EfeitosTemporariosUtil.cs` | COM-02; aplicação, duração e limpeza |
| Scripting | `ItemConsumivel.cs` | Preservar legado; remoção depende de prova de ausência de consumidores |
| Scripting | `LocalizadorDeTerminal.cs` | TER-03/PUZ-01; preservar contexto e origem |
| Scripting | `PlayerMovement.cs` | Preservar; VAL-02 confere input e liberação após menus |
| Scripting | `PontosDeProgressao.cs` | COM-03/PRO-01; gastos e saldo válidos |
| Scripting | `ProgressaoDeNivel.cs` | PRO-01; limiares e pontos por nível |
| Scripting | `RestaurarPosicao.cs` | EST-02/COM-06; restauração coordenada e retorno |
| Scripting | `SistemaDeProgressao.cs` | PRO-01/MIS-02; XP concedido uma vez |
| Scripting | `TabelaDeCombate.cs` | COM-01/COM-02; preservar fórmulas antes de balancear |
| Scripting | `TabelaDeCustos.cs` | COM-03; custos/limites e política legada |
| Scripting | `TecnicaComposta.cs` | COM-03/COM-04; investimento e uso |
| Scripting | `TerminalDroidApi.cs` | TER-02/TER-03/COM-03; fachada, ajuda e validação |
| Scripting | `TipoAtributo.cs` | TER-02/COM-03; validar enum sem reorganizar namespace |
| Scripting | `TransicaoDeCena.cs` | EST-02/COM-06; preservar transições e contexto |
| Verificadores | `VerificadorDePuzzle.cs` | PUZ-01/PUZ-02; MEC-06 opcional |

Contagem por pasta: Camera 1; Casa 3; Catalogos 8; Combat 6; Droid 7; Estado 4; Historia 10; Menus 7; Scripting 15; Verificadores 1. Não foram incluídos scripts de pacotes/terceiros como se fossem código do jogo.

## Critério de completude e sessão atual

O planejamento está completo **como roteiro para revisão**: há cobertura integral do inventário, entregas com contratos/aceite/validação, ordem sem ciclo, propostas de mecânicas com recorte de demo e decisões abertas identificadas. Isso não significa que todos os detalhes de balanceamento, textos e objetos já estejam fechados para implementar. A execução integral ainda depende das decisões listadas; cada recorte poderá avançar quando seus próprios pré-requisitos forem resolvidos.

| Critério | Situação nesta revisão |
|---|---|
| Análise e rastreabilidade dos scripts | 62 scripts relacionados acima; análise estática, sem teste de gameplay |
| Planos de implementação e mecânicas | Sete documentos; 37 entregas, incluindo PRO-02 e COM-07 |
| Dependências e contratos | 37 entregas e linhas de dependência conferidas, sem ciclos, IDs duplicados ou referências desconhecidas |
| Arquivos, aceite, validação e reversão | Definidos por eixo/entrega; montagem exata será inventariada no recorte autorizado |
| Decisões de produto | Recarga e retorno Casa escolhidos; B1/R1 e melhorias complementares propostas; narrativa adiada |
| Implementação e runtime | Não iniciados; nenhuma alegação de compilação, input real ou aprovação visual |

**Ajustes feitos nesta validação:** retirar loja do caminho obrigatório de missões/progressão; tornar entrega de quest local e atômica; limitar deduplicação a produtores sequenciais com marcadores; separar puzzle único de rodadas futuras; explicitar fonte proposta do filtro de Dara e dono de cada recompensa; preservar reembolso histórico, migrações sequenciais e checkpoint vigente.

**Evidência da conferência anterior, em 21/10/2026:** sete arquivos Markdown; 68 links locais com destinos existentes; âncora da retomada vigente conferida; 35 definições de entregas únicas, sem referências desconhecidas; 35 entradas no grafo de dependências, sem ciclos; inventário de 62 scripts comparado aos 62 arquivos em disco, sem falta ou sobra. Esse resultado antecede a adição de PRO-02/COM-07 e não substitui a nova conferência.

**Resultado desta atualização:** propostas inseridas nos cinco documentos delimitados, sem alterações narrativas. Conferência concluída: sete documentos, 68 destinos de links locais válidos, 37 entregas únicas e 37 linhas de dependência sem ciclo, cobertura dos 62 scripts e cinco exemplos de custo B1 recalculados sem divergência. Blocos Markdown balanceados, final de linha presente e sem espaços finais; `git diff --check` sem diagnóstico. Status Git restrito aos sete documentos novos da tarefa original, sem mudança de código/assets; aviso de leitura do ignore global não impediu a conferência local. Testes do jogo não executados; números continuam propostas sem playtest.

**Editado e salvo nesta tarefa:** somente os sete documentos desta pasta. O escopo documental foi registrado antes da redação. Código, cenas, prefabs, pacotes, saves e documentação anterior permanecem fora das alterações desta tarefa.

**Estado conhecido do Unity:** não consultado. Não foram iniciados Play, testes por API, input real, builds ou operações MCP. Pendências da CasaLip e da auditoria continuam nos respectivos planos, sem nova confirmação nesta sessão.

**Backups:** nenhum save pessoal acessado e nenhum backup de gameplay necessário. Para reverter apenas esta documentação, remover seletivamente os arquivos criados por esta tarefa após conferir alterações posteriores; não aplicar limpeza recursiva da pasta do projeto.

**Próximo passo:** atualização documental entregue. Na futura implementação, selecionar um recorte P1, usar B1/R1 como propostas registradas e executar VAL-01; inventariar o ponto seguro da Casa e a migração. Narrativa permanece adiada. Nenhum desses comportamentos foi implementado por esta tarefa.
