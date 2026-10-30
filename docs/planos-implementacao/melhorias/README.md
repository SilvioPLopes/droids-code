# Melhorias — índice do backlog

**Consolidado em 29/10/2026.** Entrada de navegação para os seis planos temáticos. Prioridades ficam no [plano de desenvolvimento](../../plano-desenvolvimento.md); implementação, testes e aceite ficam no plano da entrega selecionada.

## Navegação e IDs preservados

| Documento | Entregas propostas |
|---|---|
| [01 — Terminal e puzzles](01-terminal-e-puzzles.md) | TER-01, TER-02, TER-03, PUZ-01, PUZ-02, CAD-01 |
| [02 — Estado, inventário e economia](02-estado-inventario-e-economia.md) | EST-01, EST-02, INV-01, INV-02, ECO-01 |
| [03 — Combate e progressão](03-combate-e-progressao.md) | COM-01, COM-02, COM-03, COM-04, COM-05, COM-06, COM-07, PRO-01, PRO-02 |
| [04 — Missões, mundo e narrativa](04-missoes-mundo-e-narrativa.md) | MIS-01, MIS-02, UI-01, MUN-01, MUN-02, NAR-01 |
| [05 — Mecânicas e demo](05-mecanicas-e-demo.md) | MEC-01, MEC-02, MEC-03, MEC-04, MEC-05, MEC-06, DEM-01 |
| [06 — Validação e entrega](06-validacao-e-entrega.md) | VAL-01, VAL-02, VAL-03, VAL-04 |

São **37 IDs de entrega**, preservados. Dependências, contratos, arquivos candidatos e aceite são consultados no item correspondente, sem segunda lista de execução neste README. Não implementar as 37 entregas como um lote nem interpretar checkboxes antigos como defeitos atuais.

## Precedência sobre o roteiro antigo

Os planos nasceram de análise estática de 20–21/10. A expressão antiga “implementação não iniciada” se referia àquela tarefa documental; não descreve o projeto inteiro em 29/10. Não reabrir trabalho já entregue nem encerrar validação real por causa de fixture.

| Tema | Fonte para reconciliar o backlog com entregas posteriores |
|---|---|
| Equipamentos, troca/devolução, comandos e save v7 | [Entrega Droid](../2026-10-23-droid-ilustrado.md), [guia de equipamentos](../../guias/equipamentos-apollo.md) |
| Estado, carga, nova sessão e retomada | [Contrato de save](../../contextos/sistemas/estado-e-save.md), [diagnóstico de carregar](../2026-10-22-diagnostico-avisos-e-carregamento.md) |
| Energia, efeitos, checkpoint e UI Battle | [Contrato de combate](../../contextos/sistemas/combate.md), [plano Battle](../2026-10-29-battle-artes-menus.md) |
| Menus e ícones | [Índice de trabalho](../../indice-trabalho.md), [guia de ícones](../../guias/icones-de-itens.md); aceite é por painel |
| Quests, moradores e presença | [Revisão de interiores](../2026-10-29-revisao-interiores-dialogos-colisoes.md), [matriz de identidade](../2026-10-29-refatoracao-nomenclatura-ferrovale.md) |

## Decisões e propostas a respeitar

- **Recarga e retorno à Casa:** escolhas expressas do usuário no roteiro. R1 define proposta de intervalos/famílias/imunidade; deve ser reconciliada com o sistema de energia posteriormente implementado antes de somar limitadores.
- **B1 e PRO-02:** custos, tetos, redistribuição gratuita e reembolso por investimento comprovado são propostas, sem playtest presumido. Tabelas e exemplos pertencem ao plano03; não reprecificar investimento legado pelo preço novo.
- **Casa/estado:** snapshot completo separado, novo jogo e legado não reconstruível precisam de política explícita; implementação parcial de equipamentos não encerra EST-02.
- **Terminal:** limites cooperativos, cancelamento, configuração em cópia e rollback global precisam de recorte próprio; preservar o contrato do puzzle da CasaLip.
- **Missões:** fatos únicos retroativos, vitórias após aceite, entrega atômica e rodadas explícitas são contratos propostos no plano04; não anunciar integração de todas as quests pela existência do catálogo.
- **Narrativa/NAR-01:** revisão editorial foi adiada pelo usuário; não bloqueia estudos de custos/checkpoint e não é reaberta por esta organização.
- **Demo/expansões:** um marco visível antes de rotina limitada; depuração/dicas, scanner, capacidades de peças e repetição dependem da fatia escolhida. COM-07 propõe treino finito de cinco turnos, sem XP, itens, progresso ou save.
- **Plataformas:** escolher Windows, WebGL ou ambos no recorte de validação, sem prometer teste em alvo não executado.

## Como retomar uma entrega

1. Consultar prioridade e estado atual nas fontes acima; escolher um único recorte.
2. Conferir no plano temático seus pré-requisitos e decisões abertas. Arquivos compartilhados exigem consumidores identificados e um dono por alteração.
3. Registrar no plano da tarefa arquivos/objetos/propriedades, estado anterior, backup/reversão, aceite e validação mínima.
4. Implementar somente o escopo autorizado; registrar separadamente salvo, API, input real e aprovação visual.

O inventário datado de 62 scripts, grafo anterior, evidências de 21/10 e cronologia permanecem na [cópia integral anterior](../../../output/backups/2026-10-29-refatoracao-documentacao/lote-02/antes/docs/planos-implementacao/melhorias/README.md). Esse inventário não é apresentado como retrato atual do código.

**Resultado desta consolidação:** índice reduzido, IDs e seis planos preservados; nenhuma alteração dos contratos temáticos ou da narrativa. Unity não consultado, nenhum teste de jogo ou save acessado. Próximo passo: selecionar a entrega pelas prioridades vigentes, sem reexecutar análises/testes aprovados por ausência de marcação no roteiro antigo.
