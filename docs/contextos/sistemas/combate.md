# Contexto do combate e checkpoint

## Energia do Apollo

Definida pelo usuário em23/10/2026: técnicas de combate gastam energia; técnicas mais fortes custam mais. `Droid` mantém saldo atual/máximo, inicialmente100/100. `TabelaDeEnergia` é balanceamento inicial separado de `TabelaDeCustos` (pontos de programação).

- Custo não básico:10×máximo(1, nível de dano), mais5 por turno e por faixa de5 pontos de cada efeito de atributo (mínimo uma faixa), mais dano×turnos de efeitos de dano contínuo; máximo100 por técnica. Durações negativas não diminuem custo. Valores iniciais para playtest, não fórmulas de dano novas.
- Ataque Básico custa zero e recupera10 até o máximo, acertando ou errando. Nenhuma recarga automática por abrir Status, trocar cena ou terminar batalha. Não adicionar novos consumíveis nesta alteração.
- `Droid.ExecutarAcao` verifica saldo e cobra antes do acerto; erro também paga. Saldo insuficiente não aplica dano/efeitos nem modifica energia. `CombatEngine` impede ação de morto/atordoado antes da chamada, portanto esses casos não gastam nem recarregam. Uma chamada recusada segue o fluxo de turno existente; UI desabilita técnicas sem saldo e mantém o básico disponível.
- Lista de ataques e ficha Status mostram custos/recuperação. Energia não altera pontos do terminal, HP, valores de atributos nem comportamento dos inimigos. Checkpoints existentes persistem o saldo conforme [estado e save](estado-e-save.md).

Implementação, testes e limites na [etapa de energia do Status](../../planos-implementacao/2026-10-23-status-ilustrado.md#energia-de-combate--etapa-vigente). Entrada real por controles/aceite visual continuam distintos dos testes isolados.

Contrato implementado em `BattleManager`, `EncounterZone`, `GatilhoDeBatalha`, `GerenciadorDeEstado` e `SalvamentoJson`. Estado de entrega e limites na [retomada da auditoria](../../planos-implementacao/2026-10-17-auditoria-cenas.md).

- Entradas por zona e gatilho validam o destino e gravam checkpoint do estado anterior à luta na cena/posição de exploração. Se a gravação falhar, a batalha não inicia. O contexto só é atualizado após gravar.
- Cena/posição de retorno do combate têm campos próprios; não disputam a posição temporária das portas. Battle captura a origem no Start e a reaplica antes de carregar o mundo.
- Durante a luta, consumo e HP ficam em memória. Interromper a sessão e carregar retoma os dados serializados no checkpoint de entrada. Não há restauração de turno/inimigo em andamento.
- Encerrar bloqueia comandos e fecha listas antes da espera. Vitória aplica XP, flag e recompensas uma única vez; vitória/fuga gravam estado final para a origem. Fuga não concede prêmio. As duas saídas limpam inimigo/flag/contexto pendentes.
- Falha de gravação final informa o jogador e mantém o resultado em memória para salvar no mundo; não repete recompensas. Derrota não sobrescreve o checkpoint anterior nem implementa uma política nova de cura/penalidade.
- Battle aberta diretamente, sem contexto válido, não grava checkpoint de encerramento; usa o destino de fallback do Inspector para sair.
- Save legado cuja cena é Battle é recusado antes de aplicar dados; arquivo preservado. Não é possível reconstruir origem/turno ausentes. O menu mostra o erro e permanece aberto.

SAVE-001 foi implementado em23/10: save v7 inclui equipamentos, com testes isolados de restauração; [entrega e limites](../../planos-implementacao/2026-10-23-droid-ilustrado.md). Isso não valida o percurso real de batalha/checkpoint. Restrições de chefes/únicos, limpeza de efeitos e política completa de derrota continuam pendências próprias. Testes finais do recorte de17/10 foram suspensos a pedido; builds Windows/WebGL não executados.

## Próxima apresentação de Battle e inimigos

Direção registrada em23/10, ainda sem implementação: refazer menus de Battle mantendo ações/listas/Status/log/energia/retorno e considerar padrões de inimigos em entrega separada. `InimigoFixo` disponibiliza hoje uma única ação; evoluir isso exige regra explícita e playtest, não apenas arte nova. UI-004/COM-003 no [plano de desenvolvimento](../../plano-desenvolvimento.md) e COM-05 no [plano de melhorias](../../planos-implemnentação/melhorias/03-combate-e-progressao.md) concentram a sequência. Prefabs disponíveis e limites na [auditoria atualizada](../../planos-implementacao/2026-10-17-auditoria-cenas.md#atualização-documental-e-reutilização--23102026).
