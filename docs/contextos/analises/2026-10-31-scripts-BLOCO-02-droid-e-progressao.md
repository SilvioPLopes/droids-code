# Auditoria de scripts C# — BLOCO 02: Droid e progressão

**Escopo:** leitura estática integral dos scripts em `Assets/Scripts/Droid/` e `Assets/Scripts/Scripting/`. Comparei os fluxos pertinentes com o relatório geral anterior, o BLOCO 01 e trechos relacionados da documentação. Não executei testes, compilei, abri Unity, rodei o jogo ou usei MCP. Nenhum arquivo existente foi alterado.

**Baseline:** consultei [Auditoria geral do projeto — 30/10](2026-10-30-auditoria-geral-projeto.md) e [BLOCO 01 — Combate](2026-10-31-scripts-BLOCO-01-combate.md) antes da análise. Achados já registrados nesses documentos estão listados como prévios, sem repetição como achados novos.

## 1. Resumo por arquivo

| Arquivo | Leitura integral | Função |
|---|---:|---|
| `Assets/Scripts/Droid/Braco.cs` | Sim | Peça tipada que delega os dados comuns a `DroidPart`. |
| `Assets/Scripts/Droid/Cabeca.cs` | Sim | Peça tipada que delega os dados comuns a `DroidPart`. |
| `Assets/Scripts/Droid/Droid.cs` | Sim | Estado e atributos do jogador, ações de combate, energia, efeitos e equipamento. |
| `Assets/Scripts/Droid/DroidPart.cs` | Sim | Tipos de slot, raridade e dados comuns das peças. |
| `Assets/Scripts/Droid/DroidStats.cs` | Sim | Atributos-base do Droid. |
| `Assets/Scripts/Droid/Perna.cs` | Sim | Peça tipada que delega os dados comuns a `DroidPart`. |
| `Assets/Scripts/Droid/ServicoDeEquipamento.cs` | Sim | Valida e executa equipar/devolver peças no estado em memória. |
| `Assets/Scripts/Droid/Tronco.cs` | Sim | Peça tipada que delega os dados comuns a `DroidPart`. |
| `Assets/Scripts/Scripting/DroidScriptRunner.cs` | Sim | Gerencia VM Lua, retomadas limitadas, resultado e cancelamento. |
| `Assets/Scripts/Scripting/EfeitosTemporariosUtil.cs` | Sim | Decrementa e remove efeitos temporários. |
| `Assets/Scripts/Scripting/ItemConsumivel.cs` | Sim | Define itens de cura de batalha em uma lista fixa declarada como provisória. |
| `Assets/Scripts/Scripting/LimitesDeExecucao.cs` | Sim | Define limites configuráveis para fonte, retomadas e saída Lua. |
| `Assets/Scripts/Scripting/LocalizadorDeTerminal.cs` | Sim | Localiza e desempata terminais carregados. |
| `Assets/Scripts/Scripting/PlayerMovement.cs` | Sim | Lê movimento, atualiza animação e consulta estado para bloquear movimento em menus. |
| `Assets/Scripts/Scripting/PontosDeProgressao.cs` | Sim | Concede, gasta e restaura pontos de progressão. |
| `Assets/Scripts/Scripting/ProgressaoDeNivel.cs` | Sim | Acumula XP, calcula níveis pela curva atual e restaura progresso. |
| `Assets/Scripts/Scripting/RestaurarPosicao.cs` | Sim | Restaura a posição ao carregar a cena correspondente. |
| `Assets/Scripts/Scripting/SistemaDeProgressao.cs` | Sim | Converte níveis ganhos em pontos de progressão. |
| `Assets/Scripts/Scripting/TabelaDeCombate.cs` | Sim | Contém constantes de combate e cálculo de atributos derivados; identificado como balanceamento inicial. |
| `Assets/Scripts/Scripting/TabelaDeCustos.cs` | Sim | Define custos de programação, com valores provisórios. |
| `Assets/Scripts/Scripting/TabelaDeEnergia.cs` | Sim | Calcula custo de energia usando aritmética `long` e teto. |
| `Assets/Scripts/Scripting/TecnicaComposta.cs` | Sim | Representa técnicas e efeitos e calcula custos. |
| `Assets/Scripts/Scripting/TerminalDroidApi.cs` | Sim | Expõe ao Lua atributos, técnicas, pontos, equipamento e log. |
| `Assets/Scripts/Scripting/TipoAtributo.cs` | Sim | Enumera os seis atributos. |
| `Assets/Scripts/Scripting/TransicaoDeCena.cs` | Sim | Trata passagem por trigger, bloqueio opcional e posição de chegada. |

## 2. Estado por sistema

### Droid, combate e equipamento

`Droid` é simultaneamente estado do jogador, participante de combate e consumidor das tabelas/utilitários de `Scripting`. O equipamento é mediado por `ServicoDeEquipamento`, que coordena as peças e o inventário em memória. Os scripts de peças tipadas reutilizam `DroidPart` para dados comuns. A documentação consultada descreve as fórmulas e os atributos como atuais/iniciais; não propus rebalanceamento de constantes que o próprio código ou os documentos chamam provisórias.

### Técnicas, pontos e execução Lua

`TerminalDroidApi` expõe operações Lua que consultam ou alteram o Droid, inclusive aprender técnica e gastar pontos. `TecnicaComposta` calcula custos; `PontosDeProgressao` efetua o gasto. Um caminho permite que a entrada Lua aceite durações positivas sem limite, embora custo de veneno seja multiplicado em `int`, criando o achado novo abaixo.

O runner limita fonte, retomadas e saída por `LimitesDeExecucao`. Uma coroutine que termina normalmente pode ser reportada como execução concluída mesmo se uma chamada Lua retornou `false`; o plano TER-02 trata a semântica de recusas como decisão ainda proposta. A reversão após erro/cancelamento é parcial para atributos, pontos e técnicas e essa limitação está explicitamente avisada, não sendo registrada como nova falha.

### XP, energia, efeitos e transição

`ProgressaoDeNivel` usa a curva atual e `SistemaDeProgressao` traduz níveis em pontos. A curva, pontos por nível e as tabelas de custos/balanceamento são provisórios conforme os próprios arquivos e documentação. `TabelaDeEnergia` usa `long` e teto para o custo, diferentemente da operação de custo de técnica envolvida no overflow. A utilidade de efeitos decrementa e remove efeitos; a persistência/duração entre batalhas depende do fluxo de combate, fora da conclusão isolada deste bloco. Movimento, restauração de posição e transição dependem de `GerenciadorDeEstado.Instancia` e de identificadores de cena.

## 3. Achados

### 🟡 Importante — overflow no custo de técnica pode conceder quase todos os pontos

- **Arquivos/linhas:** [TerminalDroidApi.cs:166–169](../../Assets/Scripts/Scripting/TerminalDroidApi.cs); [TabelaDeCustos.cs:16–17](../../Assets/Scripts/Scripting/TabelaDeCustos.cs); [TecnicaComposta.cs:66–74](../../Assets/Scripts/Scripting/TecnicaComposta.cs); [TerminalDroidApi.cs:246–247](../../Assets/Scripts/Scripting/TerminalDroidApi.cs); [PontosDeProgressao.cs:15–23](../../Assets/Scripts/Scripting/PontosDeProgressao.cs).
- **Evidência:** a API aceita duração positiva sem teto; o custo de veneno é 2 por turno; a técnica multiplica esse valor pela duração em `int`; o custo resultante é passado ao gasto de pontos; e `TentarGastar` não rejeita gasto negativo.
- **Reprodução estática:** uma técnica com dano 1 e veneno de dano 1 por `1.073.741.824` turnos faz `2 × duração` transbordar para `int.MinValue`. Somado ao custo do dano, o custo total torna-se negativo. Subtrair esse gasto negativo de saldo zero pode resultar em `2.147.483.647` pontos.
- **Causa provável:** parâmetros sem limites e operações de custo em `int` sem verificação, combinados com uma rotina de gasto que aceita quantidade negativa.
- **Sugestão (não aplicada):** limitar dano/duração/nível; calcular custos em aritmética verificada e rejeitar custo inválido antes de mutar a técnica ou o saldo; fazer `TentarGastar` recusar valores não positivos. A validação deve garantir que overflow/falha não cause mutação parcial.
- **Contexto documental:** coincide com requisitos ainda propostos em `docs/planos-implementacao/melhorias/03-combate-e-progressao.md:168–180` (overflow deve falhar sem mutação; gasto negativo não pode conceder saldo), não com implementação aprovada já concluída.

## 4. Achados prévios, divergências e decisões

### Achados prévios não repetidos como novos

- A auditoria geral já registrava como pendência o acesso de produção a `TesteAdicionarPontos` e a discrepância documental do TER-001. A cobertura deste bloco confirma o caminho em [TerminalDroidApi.cs:315–323](../../Assets/Scripts/Scripting/TerminalDroidApi.cs) e a configuração da VM em [DroidScriptRunner.cs:20–22](../../Assets/Scripts/Scripting/DroidScriptRunner.cs); não reclassifico como achado novo.
- O baseline registra incerteza sobre efeitos temporários entre batalhas. [EfeitosTemporariosUtil.cs](../../Assets/Scripts/Scripting/EfeitosTemporariosUtil.cs) confirma decremento/remoção, mas este bloco não auditou o fluxo completo de fim de batalha.
- O problema de progresso de quest por derrota, confirmado no BLOCO 01, pertence ao fluxo de batalha e não é reclassificado aqui.

### Limitações ou intenções documentadas (⚪)

- **Comando recusado dentro de Lua:** os métodos da API retornam `false` para recusas; o runner considera concluída uma coroutine que termina normalmente (`DroidScriptRunner.cs:76,96`), e a UI publica `AoExecutarCodigo` após sucesso do runner (`Assets/Scripts/Menus/TerminalUIManager.cs:312`). O plano TER-02 propõe tratar recusa como falha transacional (`docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md:73–84`), ainda como proposta. Não classificado como defeito independente até decisão do contrato.
- **Mutação parcial em erro/cancelamento Lua:** o runner reverte equipamento/inventário, mas informa que atributos, pontos e técnicas podem permanecer alterados. Isso está explicitamente documentado no código; D-TER-02 continua proposto. É limitação conhecida.
- **Placeholders:** lista de itens de batalha, curva de XP, pontos por nível e tabelas de custo/balanceamento são apresentados como provisórios no código/documentação. Não sugeri rebalanceamento.
- **Lua “Em construção”:** `docs/wiki/Lua.md` descreve a página explicativa; esse rótulo não contradiz, por si só, a existência de runner/API implementados.
- As fórmulas consultadas em `docs/wiki/Sistemas-do-jogo.md` e `docs/contextos/sistemas/combate.md` são, em geral, coerentes com o código. Textos antigos sobre Lua sem limites permanecem divergência já anotada no baseline.

### Acoplamentos e fragilidades

- `Droid` implementa o contrato de combate e depende de tabelas/utilitários em `Scripting`.
- `DroidScriptRunner` delega à `TerminalDroidApi`; a UI avança o runner e publica o evento de execução. Alterar a semântica de erro/recusa exige compatibilidade entre API, runner e UI.
- `ServicoDeEquipamento` coordena inventário e estado do Droid.
- Movimento, restauração de posição e transições dependem de `GerenciadorDeEstado.Instancia` e nomes/IDs de cena.

## 5. Documentação consultada e divergências

Consulta dirigida, não integral, de:

- `docs/contextos/sistemas/combate.md`
- `docs/contextos/sistemas/estado-e-save.md`
- `docs/wiki/Sistemas-do-jogo.md`
- `docs/wiki/Lua.md`
- Trechos pertinentes de `docs/planos-implementacao/melhorias/03-combate-e-progressao.md`
- Trechos pertinentes de `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md`
- Trechos pertinentes de `docs/planos-implementacao/2026-10-30-estabilidade-lua-loja-game-over.md`

No material consultado, as fórmulas correntes e os placeholders estão em geral alinhados. Permanecem as divergências antigas sobre exposição de comandos de teste e limites Lua, já descritas no relatório geral. O requisito de overflow sem mutação está no plano de melhoria, mas ainda não está satisfeito pelo caminho de custo em `int`.

## 6. Limitações da auditoria

- Cenas, prefabs, valores efetivos do Inspector, entradas efetivas de catálogos, persistência em execução e validação visual não foram inspecionados.
- A leitura dos documentos relacionados foi dirigida aos trechos pertinentes; não se declara leitura integral deles.
- A passagem de efeitos entre batalhas e o fluxo integral de quest não foram reabertos neste bloco por já estarem registrados como itens de investigação em outros relatórios.

## 7. Próximo passo

BLOCO 02 concluído e registrado. Próximo na sequência: BLOCO 03 — `Assets/Scripts/Estado/` inteiro e `SalvamentoJson.cs`, mantendo as restrições de auditoria somente leitura e sem repetir achados anteriores como novos.
