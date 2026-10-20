# Contexto do combate e checkpoint

Contrato implementado em `BattleManager`, `EncounterZone`, `GatilhoDeBatalha`, `GerenciadorDeEstado` e `SalvamentoJson`. Estado de entrega e limites na [retomada da auditoria](../../planos-implementacao/2026-10-17-auditoria-cenas.md).

- Entradas por zona e gatilho validam o destino e gravam checkpoint do estado anterior à luta na cena/posição de exploração. Se a gravação falhar, a batalha não inicia. O contexto só é atualizado após gravar.
- Cena/posição de retorno do combate têm campos próprios; não disputam a posição temporária das portas. Battle captura a origem no Start e a reaplica antes de carregar o mundo.
- Durante a luta, consumo e HP ficam em memória. Interromper a sessão e carregar retoma os dados serializados no checkpoint de entrada. Não há restauração de turno/inimigo em andamento.
- Encerrar bloqueia comandos e fecha listas antes da espera. Vitória aplica XP, flag e recompensas uma única vez; vitória/fuga gravam estado final para a origem. Fuga não concede prêmio. As duas saídas limpam inimigo/flag/contexto pendentes.
- Falha de gravação final informa o jogador e mantém o resultado em memória para salvar no mundo; não repete recompensas. Derrota não sobrescreve o checkpoint anterior nem implementa uma política nova de cura/penalidade.
- Battle aberta diretamente, sem contexto válido, não grava checkpoint de encerramento; usa o destino de fallback do Inspector para sair.
- Save legado cuja cena é Battle é recusado antes de aplicar dados; arquivo preservado. Não é possível reconstruir origem/turno ausentes. O menu mostra o erro e permanece aberto.

Equipamentos ainda não são serializados por SAVE-001. Restrições de chefes/únicos, limpeza de efeitos e política completa de derrota continuam pendências próprias. Implementação gravada não equivale a comportamento validado: testes finais foram suspensos a pedido do usuário; builds Windows/WebGL não foram executados.
