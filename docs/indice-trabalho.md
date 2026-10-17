# Índice de trabalho

Leia [AGENTS.md](../AGENTS.md) → este índice → plano da tarefa → somente os subcontextos necessários. Use os links para pesquisar; não leia todos os documentos em sequência.

## Retomada

A reorganização documental foi aplicada em 17/10/2026. O trabalho de gameplay da CasaLip continua no [plano de correção — retomada vigente](planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente). Esse bloco concentra aceite do usuário, resultados posteriores, pendências, próximo passo e cuidado com o save. Consultar antes de executar qualquer correção.

## Navegação por necessidade

| Necessidade | Fonte | Chaves de busca |
|---|---|---|
| Arquitetura e relações entre sistemas | [Contexto do projeto](contexto-projeto.md) | `Ambiente e relações`, `Subcontextos` |
| Contrato da casa | [CasaLip](contextos/cenas/casalip.md) | `ControlePuzzleAgua`, `p_agua_casa` |
| UI compartilhada | [Interface](contextos/sistemas/interface.md) | `Canvas`, `Runner`, `origem` |
| Persistência | [Estado e save](contextos/sistemas/estado-e-save.md) | `MigrarDados`, `ato1_casa_resolvida` |
| Operar o conector | [MCP](guias/operacao-mcp.md) | `timeout`, `dry-run`, `Segurança` |
| Prioridades | [Plano de desenvolvimento](plano-desenvolvimento.md) | `INT-001`, `UI-003`, `TER-001` |
| Iniciar uma tarefa | [Modelo](planos-implementacao/_modelo-tarefa.md) | `escopo`, `aceite`, `retomada` |
| Configuração e percursos | [Montagem](guias/montagem-ato-1.md), [validação](guias/validacao-jogo.md) | Pesquisar apenas cena/sistema envolvido; há etapas herdadas |
| Evidências da correção | [Revisão de 17/10](revisoes/casalip-validacao-2026-10-17.md) | `C01` a `C10`, `Atualização` |
| Diagnóstico anterior | [Revisão da casa](revisoes/casalip.md), [porta](problemas/casalip-retorno-porta-trancada.md), [apresentação](problemas/casalip-apresentacao-dialogo.md) | Consultar para reprodução ou decisão anterior |
| Narrativa | [Enredo](enredo/TCC_-_ENREDO_v2.md), [adendo](enredo/TCC_-_ENREDO_v2_ADENDO.md) | O adendo prevalece |
| Assets de personagens | [Catálogo visual](../Assets/sprites/Entities/Npc's/Characters/Documentation/character-catalog.md) | `idle`, `walk`, `placeholder` |
| Histórico e inventários antigos | [Índice histórico](historico/README.md), [contexto anterior](historico/2026-10-17-contexto-antes-hierarquia.md) | Consulta sob demanda; não usar como estado atual |

Andamento fica no plano da tarefa. Evidências detalhadas ficam em revisões; este índice mantém caminhos, sem copiar resultados. City receberá contexto próprio quando sua análise for iniciada; ideias de NPCs andando ainda não são implementação autorizada.
