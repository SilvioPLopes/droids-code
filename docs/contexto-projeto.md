# Contexto do projeto

Referência de arquitetura. Para retomar trabalho, comece pelo [índice](indice-trabalho.md) e pelo plano da tarefa; resultados e pendências pertencem ao plano.

Droids Code é um RPG educativo em 2D com exploração em oito direções, combate por turnos e programação Lua. Lip e Apollo atravessam Ferrovale. A demo avança por partes; seu ponto final ainda não está definido.

## Ambiente e relações

- Unity: versão em [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt). Código C#, URP 2D, uGUI/TMP e MoonSharp; dependências em [manifest.json](../Packages/manifest.json).
- Navegação configurada na análise de 17/10: MainMenu → CasaLip ↔ City ↔ Game → Battle → Game. Cenas habilitadas em [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset). Mudanças de retorno da batalha exigem contexto próprio.
- Estado semântico persiste em GerenciadorDeEstado; portas, NPCs, Player e controladores são locais à cena e reaplicam esse estado. O Canvas do MenuMundoManager pode persistir entre cenas.
- O terminal executa Lua por DroidScriptRunner/TerminalDroidApi. VerificadorDePuzzle lê condições dos catálogos e coordena conclusão persistente. Lua síncrono sem limite de execução é risco registrado em `TER-001`; não testar laços infinitos.
- Catálogos mantêm conteúdo; gatilhos e cenas fazem a integração. Conteúdo no catálogo não comprova montagem ou funcionamento.
- Windows e WebGL são plataformas pretendidas; funcionamento no Editor não comprova builds.

## Subcontextos

| Assunto | Fonte e quando consultar |
|---|---|
| CasaLip: atores, puzzle e integração | [CasaLip](contextos/cenas/casalip.md), ao alterar a casa |
| City: escala, nomes, miniaturas e montagem dos NPCs | [City](contextos/cenas/city.md), ao alterar a cidade; usar o [modelo de cena](contextos/cenas/_modelo-cena.md) para novos contratos |
| Canvas, painéis e origem de abertura | [Interface](contextos/sistemas/interface.md), ao tocar UI compartilhada |
| Reutilizar menus/portas ou preparar Game/vale e Battle | [Inventário e próximos recortes](planos-implementacao/2026-10-17-auditoria-cenas.md#atualização-documental-e-reutilização--23102026), conferência estática23/10; consultar antes de criar duplicatas |
| Flags, duração de objetos e migração | [Estado e save](contextos/sistemas/estado-e-save.md), ao tocar persistência |
| Checkpoint, encerramento e retorno de batalha | [Combate](contextos/sistemas/combate.md), ao alterar entrada/saída ou save durante luta |
| Uso e segurança do conector | [Operação MCP](guias/operacao-mcp.md), ao operar/diagnosticar MCP |
| Inventário amplo de scripts, combate, fórmulas e lacunas | [Referência de 17/10](historico/2026-10-17-contexto-antes-hierarquia.md), somente se a tarefa exigir; confirmar código atual |

Novos subcontextos são extraídos quando necessários a uma tarefa, sem duplicar a mesma informação. A referência anterior fica preservada para consulta; não é uma segunda fonte de andamento.

## Autoridade das informações

[AGENTS.md](../AGENTS.md) orienta a execução; o [plano de desenvolvimento](plano-desenvolvimento.md) prioriza trabalho. Planos de implementação concentram escopo, decisões e retomada. Revisões guardam evidências datadas. Guias descrevem procedimentos reutilizáveis.

O [adendo narrativo](enredo/TCC_-_ENREDO_v2_ADENDO.md) prevalece sobre o [enredo v2](enredo/TCC_-_ENREDO_v2.md). Conflitos com montagem devem ser registrados antes de alterar comportamento.

Código lido, configuração serializada, execução por API, input real e avaliação visual são evidências diferentes. A reprovação visual do usuário permanece pendente mesmo com testes de lógica aprovados.
