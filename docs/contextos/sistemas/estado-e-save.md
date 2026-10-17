# Contexto de estado e save

- [GerenciadorDeEstado](../../../Assets/Scripts/Estado/GerenciadorDeEstado.cs) mantém estado semântico da sessão; portas, NPCs e controladores locais o reaplicam ao carregar a cena.
- [DadosDoJogo](../../../Assets/Scripts/Estado/DadosDoJogo.cs) usa versão 6; [SalvamentoJson](../../../Assets/Scripts/Estado/SalvamentoJson.cs) grava `Application.persistentDataPath/save.json` com JsonUtility.
- `MigrarDados` roda em memória antes de restaurar flags. Legados anteriores à v6 com `ato1_casa_resolvida=true` conservam a conclusão e recebem `ato1_falou_apollo_casa=true`. O formato antigo não distingue introdução de resolução; preservar progresso também libera quem só conversou. Não apagar essa flag para tentar inferir o passado.
- Variáveis da VM Lua pertencem à abertura do terminal e não substituem flags persistentes. Restaurar uma conclusão não deve repetir gravação, recompensa ou evento de primeira resolução.
- Carregar restaura dados, agenda posição, zera contador de menus e muda a cena. Isso não garante sincronização de todas as flags internas da UI persistente.

Lacunas registradas na análise anterior: peças equipadas não serializadas, autosave de Battle sem contexto completo de retorno e compra/venda sem save nesses métodos. Confirmar o código ao tratar uma dessas tarefas; não incluí-las automaticamente numa correção da casa.

Antes de testar gravação, copiar o arquivo ativo e registrar seu hash. Ao terminar, restaurar somente se o arquivo ainda corresponder à sessão de teste; verificar o resultado. O estado operacional do save da última sessão está no [plano da tarefa](../../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente), não neste contrato.
