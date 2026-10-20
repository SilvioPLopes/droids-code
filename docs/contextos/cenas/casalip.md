# Contexto da CasaLip

Contrato da cena; o [plano da tarefa](../../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente) mantém aceite, pendências e próximos passos.

- Cena: [CasaLip.unity](../../../Assets/Scenes/CasaLip.unity). Player, Apollo, porta e controlador do puzzle pertencem à cena. A UI pode sobreviver a transições.
- [ControladorPuzzleAguaCasa](../../../Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs) coordena `ControlePuzzleAgua`, contexto do terminal e conversa de sucesso.
- [VerificadorDePuzzle](../../../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs) usa `p_agua_casa`: `nivel_agua >= 100` conclui. Atribuição direta continua aceita; não há comprovação de uso de `while`.
- A introdução usa `ato1_falou_apollo_casa`; conclusão usa `ato1_casa_resolvida`. Restaurar conclusão reaplica a porta sem repetir evento, recompensa ou conversa.
- Após a introdução, outro GatilhoDeDialogo no Apollo usa `casa_apollo_conversa`: “Por que não vamos lá fora?”. Exige a flag da introdução, permite repetição e não grava flag ao terminar. O NpcInterativo genérico do Apollo fica desabilitado; os gatilhos atendem a tecla E com condições complementares.
- O sucesso usa `CatalogoDeDialogos.CasaPortaApollo` / `casa_porta_apollo`; o controlador aguarda diálogo ocupado sem substituir o callback.
- Apollo usa `Assets/sprites/Entities/Npc's/Characters/Companion/Apollo/animations/idle.png`, quadro `idle_4`, estático. O componente reutilizável [OrientacaoNpc](../../../Assets/Scripts/Historia/OrientacaoNpc.cs) permite trocar a pose na tentativa de interação com E, mesmo sem fala nova disponível. GatilhoDeDialogo e NpcInterativo respeitam o bloqueio de menus e mantêm as condições narrativas; chamadas diretas de Disparar também orientam após abrir uma conversa. Para NPCs sem Animator: anexar ao mesmo objeto do gatilho/interação e preencher renderer e poses cima/esquerda/baixo/direita. No atlas do Apollo correspondem a `idle_0/2/4/6`. A orientação usa o eixo predominante e mantém a pose depois da conversa; NPCs sem componente mantêm o comportamento anterior. Montagem e validação do Apollo estão no plano.
- Lip usa `Assets/Animations/CasaLip/Lip/`; parâmetros `MoveX`, `MoveY`, `Speed`. As instâncias LPC em CasaLip/City desativam `usarEspelhamentoHorizontal` para não duplicar o flip.

Dependências: [interface](../sistemas/interface.md), [estado/save](../sistemas/estado-e-save.md). Evidências datadas: [revisão de 17/10](../../revisoes/casalip-validacao-2026-10-17.md). Diagnósticos anteriores: [porta](../../problemas/casalip-retorno-porta-trancada.md), [apresentação](../../problemas/casalip-apresentacao-dialogo.md).

Pesquisa restrita: `rg -n 'ato1_casa_resolvida|ato1_falou_apollo_casa|ControlePuzzleAgua' Assets/Scripts Assets/Scenes/CasaLip.unity`.
