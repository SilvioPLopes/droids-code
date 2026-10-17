# Índice de trabalho e retomada

**Atualizado:** 17/10/2026. **Direção acordada:** desenvolver uma demo por partes, começando pela CasaLip e ampliando o percurso conforme cada trecho for concluído. O ponto final da demo ainda não foi definido.

Este índice guarda caminhos e chaves de pesquisa. O [contexto](contexto-projeto.md) descreve os sistemas; o [plano existente](plano-desenvolvimento.md) mantém tarefas e critérios; cada revisão registra o estado verificado da cena.

## Onde estamos

- **Cena em revisão:** [CasaLip](revisoes/casalip.md).
- **Plano de implementação deste trecho:** [Correções da CasaLip — 16/10/2026](planos-implementacao/2026-10-16-correcao-casalip.md). Código/montagem implementados; resultados parciais na [revisão de 17/10](revisoes/casalip-validacao-2026-10-17.md).
- **Trabalho restante:** reinício de Play/load (C06), sucesso integrado pelo menu/laço finito/diálogo ocupado, 1920/narração/fala longa e input real/física. C05 já passou em duas revisitas por API.
- **Tarefas relacionadas:** `INT-001`, `HIS-001`, `UI-001`, `TER-002`; execução Lua limitada em `TER-001`.
- **Última evidência:** `Logs/CorrecaoCasaLip/green-harness-final.txt` tem seis OK recentes. `fluxo-validacao.txt` confirma C01/C02, C04 mundo/API sem repetição, duas revisitas C05 com save inalterado e oito direções C09 por API. O agente principal inspecionou `depois-1280.png` como legível. Revisão estática em `runtime-review.md` sem defeito novo no pacote. As capturas/diagnósticos de 16/10 são históricos.
- **Sessão encerrada parcialmente:** Computer Use foi interrompido pelo usuário com Escape; input real não foi aprovado. O save original foi restaurado via MCP, SHA-256 `F9018A19EFE54652D201B02BF161E7C808D3B4DC1F2FCFAF025B29F74322ABC7`, igual ao backup.
- **Compatibilidade do save:** v6 preserva conclusão legada verdadeira e acrescenta introdução. A ambiguidade antiga permanece; sessões novas separam `ato1_falou_apollo_casa` e `ato1_casa_resolvida`.

## Documentos por assunto

| Assunto | Consultar | Chaves de pesquisa |
|---|---|---|
| Estado técnico, cenas e responsabilidades | [Contexto do projeto](contexto-projeto.md) | `Sistemas e responsabilidades`, `Estado persistente`, `Salvamento` |
| Tarefas, dependências e encerramento | [Plano de desenvolvimento](plano-desenvolvimento.md) | `INT-001`, `HIS-001`, `UI-001`, `SAVE-001`, `SAVE-002`, `TER-001`, `HIS-002`, `QUEST-001` |
| Configuração no Unity | [Montagem do Ato 1](guias/montagem-ato-1.md) | `CasaLip`, `Canvas`, `City`, `Primeiro combate`, `Escolha de facção` |
| Percursos de verificação | [Validação do jogo](guias/validacao-jogo.md) | `VAL-01` a `VAL-11`, `Execução limitada de Lua` |
| História, personagens e sete sessões do Ato 1 | [Enredo v2](enredo/TCC_-_ENREDO_v2.md) | `SESSÃO 1`, `1.2`, `1.3`, `Notas de montagem`, `PARTE VII`, `X.1` |
| Cânone corrigido e direção dos atos posteriores | [Adendo](enredo/TCC_-_ENREDO_v2_ADENDO.md) | `A.2`, `A.4`, `431`, `quatrocentos`, `B.2`, `C.4`, `D.2` |
| Porta e terminal após retornar à casa | [Relato INT-001](problemas/casalip-retorno-porta-trancada.md) | `_puzzleResolvido`, `DontDestroyOnLoad`, `doorAlive`, `terminalAlive` |
| Correções locais e resultados de execução | [Validação CasaLip de 17/10](revisoes/casalip-validacao-2026-10-17.md) | `C01` a `C10`, `harness`, `migração`, `input`, `revisita`, `load` |
| Capturas, diálogo pequeno, mago e nome distante | [Análise da apresentação da CasaLip](problemas/casalip-apresentacao-dialogo.md) | `UI-003`, `VIS-001`, `ae69544`, `anchorMin`, `ScreenSpaceOverlay`, `casa_porta_apollo` |
| Rolagem do terminal ao abrir | [Relato TER-002](problemas/terminal-log-painel-inativo.md) | `AdicionarLinhaDeLog`, `RolarParaFimNoProximoFrame` |
| Fundo de batalha | [Relato do fundo](problemas/battle-fundo-nao-renderiza.md) | `SpriteRenderer`, `Image`, `contorno` |
| Entregas anteriores e originais preservados | [Índice histórico](historico/README.md) | `Registros preservados`, `Divergências reconciliadas` |
| Validações antigas da casa | [Bugs de 14/09](historico/2026-09-14-bugs-casalip.md) | `RESOLVIDO`, `Is Trigger`, `o evento dispara normalmente` |
| Arquitetura e planejamento anteriores | [Análise de 14/10](historico/2026-10-14-analise-projeto-unity.md) e [originais de contexto](historico/2026-10-16-contexto-anterior/checklist-desenvolvimento.md) | Usar o histórico para recuperar decisões; conferir o estado atual antes de executá-las |

O **adendo prevalece** sobre o enredo principal. O enredo orienta a experiência desejada; comentários que dizem "implementado" ou "testado" precisam ser confrontados com código, montagem e evidências. Os guias ainda contêm etapas herdadas: por exemplo, o enredo vigente transfere a entrega dos módulos de facção da Sessão 4 para a Sessão 5.

## Código e assets por assunto

| Assunto | Ponto de entrada | Chaves |
|---|---|---|
| Puzzle atual da casa | [ControladorPuzzleAguaCasa](../Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs), [VerificadorDePuzzle](../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs) | `ControlePuzzleAgua`, `p_agua_casa`, `AoResolverPelaPrimeiraVez`, `Resolvido` |
| Interação do terminal | [InteragirComTerminalCasa](../Assets/Scripts/Casa/InteragirComTerminalCasa.cs) | `terminal`, `_playerPorPerto`, `KeyCode.E` |
| Porta e chegada | [TransicaoDeCena](../Assets/Scripts/Scripting/TransicaoDeCena.cs), [RestaurarPosicao](../Assets/Scripts/Scripting/RestaurarPosicao.cs) | `trancada`, `pontoDeChegada`, `TemPosicaoSalva` |
| Terminal e UI persistente | [TerminalUIManager](../Assets/Scripts/Menus/TerminalUIManager.cs), [MenuMundoManager](../Assets/Scripts/Menus/MenuMundoManager.cs), [LocalizadorDeTerminal](../Assets/Scripts/Scripting/LocalizadorDeTerminal.cs) | `EstaAberto`, `DefinirContextoPuzzle`, `FecharParaDialogo`, `Encontrar`, `DontDestroyOnLoad` |
| Flags, inventário e arquivo salvo | [GerenciadorDeEstado](../Assets/Scripts/Estado/GerenciadorDeEstado.cs), [SalvamentoJson](../Assets/Scripts/Estado/SalvamentoJson.cs) | `DefinirFlag`, `MigrarDados`, `versao = 6`, `flagsDeHistoria`, `save.json` |
| Diálogos e condições | [GatilhoDeDialogo](../Assets/Scripts/Historia/GatilhoDeDialogo.cs), [CondicaoDeHistoria](../Assets/Scripts/Historia/CondicaoDeHistoria.cs) | `flagAoTerminar`, `flagsQueImpedem`, `CondicoesDeHistoria` |
| Verificador genérico e regras dos puzzles | [VerificadorDePuzzle](../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs), [CatalogoDePuzzles](../Assets/Scripts/Catalogos/CatalogoDePuzzles.cs) | `p_agua_casa`, `flagAoResolver`, `flagQueJaResolve`, `Inscrever` |
| Execução e ajuda Lua | [DroidScriptRunner](../Assets/Scripts/Scripting/DroidScriptRunner.cs), [ApolloDica](../Assets/Scripts/Historia/ApolloDica.cs) | `ExecutarCodigo`, `ObterVariavelNumerica`, `Traduzir` |
| Falas da casa | [CatalogoDeDialogos](../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs) | `CasaIntroApollo`, `CasaFiltro`, `CasaCaneca`, `CasaCaderno`, `CasaNoiteApollo` |
| Regressões CasaLip por API/inspeção | [ValidacaoCasaLip](../Assets/Editor/ValidacaoCasaLip.cs) | `ValidarEstadoDaCasa`, `ValidarMenus`, `ValidarMigracaoCasaLip`, `ValidarOrigemDoTerminal` |
| Personagens e sprites disponíveis | [Catálogo visual](../Assets/sprites/Entities/Npc's/Characters/Documentation/character-catalog.md), [mapeamento](../Assets/sprites/Entities/Npc's/Characters/Documentation/sprite-mapping.md), [animações](../Assets/sprites/Entities/Npc's/Characters/Documentation/animation-coverage.md) | `placeholder`, `PENDENTE`, `walk`, `idle`, `character-config.md` |

## Pesquisas rápidas

Executar na raiz do projeto:

```powershell
rg -n --glob '*.md' 'INT-001|HIS-001|VAL-02|VAL-03' docs
rg -n 'ato1_casa_resolvida|nivel_agua|_puzzleResolvido' Assets/Scripts Assets/Scenes/CasaLip.unity
rg -n 'CasaFiltro|CasaCaneca|CasaCaderno|CasaNoiteApollo' Assets/Scripts/Catalogos/CatalogoDeDialogos.cs
```

Ao retomar, ler a revisão da cena, consultar somente os documentos vinculados à tarefa e verificar o estado local/MCP. Registrar o resultado observado antes de marcar uma tarefa como concluída. O desenvolvimento pode ser acompanhado pelo chat no celular; referências e resultados ficam no repositório para não depender da memória da conversa.
