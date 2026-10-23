# Índice de trabalho

Leia [AGENTS.md](../AGENTS.md) → este índice → plano da tarefa → somente os subcontextos necessários. Use os links para pesquisar; não leia todos os documentos em sequência.

## Retomada

A reorganização documental foi aplicada em17/10; revisão leve de retomada em23/10. Para menus/equipamentos, seguir o plano específico abaixo; para prefabs e próximos trabalhos de Game/vale e Battle, seguir a [atualização da auditoria](planos-implementacao/2026-10-17-auditoria-cenas.md#atualização-documental-e-reutilização--23102026) e o [plano de desenvolvimento](plano-desenvolvimento.md). Gameplay da CasaLip continua no [plano da casa](planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente). Ler somente o recorte necessário; entregas novas não encerram automaticamente as pendências históricas.

## Navegação por necessidade

| Necessidade | Fonte | Chaves de busca |
|---|---|---|
| Auditoria das cinco cenas e objetos | [Plano da auditoria](planos-implementacao/2026-10-17-auditoria-cenas.md) | `montagem`, `referências`, `integração` |
| Prefabs disponíveis; próximos trabalhos do vale e Battle | [Atualização de reutilização](planos-implementacao/2026-10-17-auditoria-cenas.md#atualização-documental-e-reutilização--23102026) | `PREFAB-001`, `Game`, `MetaValle`, `UI-004`, `COM-003` |
| Arquitetura e relações entre sistemas | [Contexto do projeto](contexto-projeto.md) | `Ambiente e relações`, `Subcontextos` |
| Contrato da casa | [CasaLip](contextos/cenas/casalip.md) | `ControlePuzzleAgua`, `p_agua_casa` |
| Configuração específica da cidade | [City](contextos/cenas/city.md) | `1.33`, `NomeNPC`, `miniatura`, `AreaDialogo` |
| Documentar configuração de outra cena | [Modelo de contexto de cena](contextos/cenas/_modelo-cena.md) | `atores`, `interface`, `integração` |
| UI compartilhada | [Interface](contextos/sistemas/interface.md) | `Canvas`, `Runner`, `origem` |
| Terminal sobre a arte do usuário | [Plano do terminal](planos-implementacao/2026-10-22-terminal-ilustrado.md), [contrato](contextos/sistemas/interface.md#terminal-ilustrado) | `ApresentacaoTerminalIlustrado`, `1536`, `PainelTerminal` |
| Repetir o método visual em outras interfaces | [Interfaces sobre imagens](guias/interfaces-sobre-imagens.md) | `legibilidade`, `arte`, `controles`, `ficha reutilizável` |
| Artes disponíveis para loja, status e Droid | [Análise de aproveitamento](planos-implementacao/2026-10-23-aproveitamento-artes-menus.md) | `novos`, `ícones`, `molduras`, `combinações` |
| Loja ilustrada na City | [Plano e retomada](planos-implementacao/2026-10-23-loja-ilustrada.md) | `LojaUIManager`, `seleção`, `Gold`, `compra`, `venda` |
| Menu do mundo ilustrado com oito opções | [Plano e validação](planos-implementacao/2026-10-23-menu-mundo-ilustrado.md) | `PainelMenuMundo`, `duas colunas`, `Carregar`, `Opções` |
| Bag ilustrada: montagem e referências | [Plano e estado da integração](planos-implementacao/2026-10-23-bag-ilustrada.md) | `grade`, `paginação`, `ApresentacaoBagIlustrada`, `bag-conceito` |
| Status: atributos e proposta visual | [Análise do Status](planos-implementacao/2026-10-23-status-ilustrado.md) | `Droid`, `seis atributos`, `10.jfif`, `técnicas`, `pontos` |
| Menu do Droid: peças e aparência em camadas | [Análise e estrutura proposta](planos-implementacao/2026-10-23-droid-ilustrado.md) | `equipamentos`, `sobreposição`, `gabarito`, `perfil de corpo`, `persistência` |
| Equipar/desequipar Apollo, IDs e localização das artes | [Guia de equipamentos](guias/equipamentos-apollo.md) | `droid.equipar`, `droid.desequipar`, `v7`, `Assets/UI` |
| Localizar ou trocar a sprite de um item | [Guia de ícones](guias/icones-de-itens.md), [plano](planos-implementacao/2026-10-23-sprites-unicas-itens.md) | `ID`, `CatalogoVisualDeItens`, `VisuaisDeItens`, `Recortes` |
| Mensagens na caixa de diálogo e falha de carregar | [Diagnóstico vigente](planos-implementacao/2026-10-22-diagnostico-avisos-e-carregamento.md), [diretriz de interface](contextos/sistemas/interface.md#diretriz-de-mensagens-ao-jogador--definida-em-22102026) | `Salvar`, `Carregar`, `primeira tentativa`, `bloqueio` |
| Persistência | [Estado e save](contextos/sistemas/estado-e-save.md) | `MigrarDados`, `ato1_casa_resolvida` |
| Batalha e checkpoint de exploração | [Combate](contextos/sistemas/combate.md) | `TentarPrepararBatalha`, `FinalizarBatalha` |
| Operar o conector | [MCP](guias/operacao-mcp.md) | `timeout`, `dry-run`, `Segurança` |
| Prioridades | [Plano de desenvolvimento](plano-desenvolvimento.md) | `INT-001`, `UI-003`, `TER-001` |
| Backlog detalhado de melhorias | [Plano de melhorias e atualizações](planos-implemnentação/melhorias/README.md) | `EST-01`, `INV-01`, `COM-04`, `COM-05`, `energia`, `recarga` |
| Iniciar uma tarefa | [Modelo](planos-implementacao/_modelo-tarefa.md) | `escopo`, `aceite`, `retomada` |
| Configuração e percursos | [Montagem](guias/montagem-ato-1.md), [validação](guias/validacao-jogo.md) | Pesquisar apenas cena/sistema envolvido; há etapas herdadas |
| Evidências da correção | [Revisão de 17/10](revisoes/casalip-validacao-2026-10-17.md) | `C01` a `C10`, `Atualização` |
| Diagnóstico anterior | [Revisão da casa](revisoes/casalip.md), [porta](problemas/casalip-retorno-porta-trancada.md), [apresentação](problemas/casalip-apresentacao-dialogo.md) | Consultar para reprodução ou decisão anterior |
| Narrativa | [Enredo](enredo/TCC_-_ENREDO_v2.md), [adendo](enredo/TCC_-_ENREDO_v2_ADENDO.md) | O adendo prevalece |
| Assets de personagens | [Catálogo visual](../Assets/sprites/Entities/Characters/Documentation/character-catalog.md) | `idle`, `walk`, `placeholder` |
| Histórico e inventários antigos | [Índice histórico](historico/README.md), [contexto anterior](historico/2026-10-17-contexto-antes-hierarquia.md) | Consulta sob demanda; não usar como estado atual |

Andamento fica no plano da tarefa. Evidências detalhadas ficam em revisões; este índice mantém caminhos, sem copiar resultados. A City tem contrato próprio; ideias de NPCs andando ainda não são implementação autorizada.
