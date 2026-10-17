# CasaLip: porta e terminal após retornar da City

**ID:** INT-001.  
**Status em 17/10/2026:** correção implementada; duas revisitas por API passaram; reinício/load pendente.

**Evidência:** sessão de investigação de 15–16/10/2026.  
**Cena:** [CasaLip.unity](../../Assets/Scenes/CasaLip.unity).  
**Prioridade e dependências:** [plano](../plano-desenvolvimento.md).

## Correção implementada em 17/10/2026

O objeto local ativo `ControlePuzzleAgua` usa `VerificadorDePuzzle` e `ControladorPuzzleAguaCasa`; a cena deixou de anexar `PuzzleAguaCasaChecker` ao terminal persistente. A primeira resolução grava `ato1_casa_resolvida`, progresso e save antes do evento de sucesso. Em outra entrada, a flag reaplica os efeitos na porta nova sem repetir save, progresso, evento ou fala. A introdução usa `ato1_falou_apollo_casa`.

`LocalizadorDeTerminal.Encontrar` inclui inativos e prefere a UI persistente válida. Interação e verificador recuperam a referência; a associação remove a inscrição anterior mesmo quando o objeto Unity foi destruído. A porta continua local, com `TransicaoDeCena` habilitado e bloqueio pelo campo `trancada`.

Save v6 migra dados anteriores preservando conclusão legada verdadeira e marcando introdução; essa política conserva progresso, mas também conserva a conclusão ambígua de um legado que só representava conversa. Não existe evidência adicional no save v5 para separar os percursos.

`Logs/CorrecaoCasaLip/fluxo-validacao.txt` registra duas revisitas CasaLip → City → CasaLip por API em 17/10. Em ambas, o harness confirmou porta aberta, terminal/referências vivos e um verificador local; introdução/sucesso não repetiram e o hash do save permaneceu igual. C05 está demonstrado nesse escopo. O mesmo percurso registrou introdução sem conclusão, resolução por 100 e reexecução sem novo save/evento/diálogo. A migração em memória passou novamente em `green-harness-final.txt`.

O critério de reiniciar Play e carregar (`C06`) permanece pendente; não foi substituído pelas revisitas. O save original foi restaurado via MCP com SHA-256 igual ao backup. Resultados e limites completos estão na [validação de 17/10](../revisoes/casalip-validacao-2026-10-17.md); INT-001 não está encerrado em todos os critérios.

## Registro histórico — investigação de 15–16/10/2026

O restante deste relato descreve o problema anterior e a solução proposta naquela coleta; não afirma que o checker legado continue montado na cena atual.

**Confirmação adicional:** [coleta de capturas e observação MCP em Play](casalip-apresentacao-dialogo.md), 16/10/2026; `_puzzleResolvido = true`, porta anterior sem referência viva, interação com terminal destruído e porta nova trancada. O responsável executou o percurso e o agente inspecionou o resultado.

## Comportamento esperado

Depois de resolver o puzzle, sair para City e voltar, a conclusão permanece, a porta está destrancada e o terminal continua utilizável.

## Reprodução relatada

1. Entrar na casa e resolver `nivel_agua >= 100`.
2. Sair pela porta para City.
3. Voltar por Entrada_CasaLip.
4. Tentar sair novamente e interagir com o terminal.

A sequência foi relatada pelo responsável. O MCP inspecionou o estado resultante; não executou automaticamente o percurso completo.

## Evidência observada

```text
WaterChecker scene=DontDestroyOnLoad resolved=True doorAlive=False
TerminalInteraction scene=CasaLip terminalAlive=False
PortaDeSaidaCasa trancada=True
GerenciadorDeEstado flags=0
```

O registro de flags é o valor daquele instante, não uma afirmação sobre todas as partidas.

## Causa sustentada por código e estado

[PuzzleAguaCasaChecker](../../Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs) mantém `_puzzleResolvido` e uma referência para a porta local. Ele está no painel do terminal, dentro do Canvas que [MenuMundoManager](../../Assets/Scripts/Menus/MenuMundoManager.cs) mantém entre cenas. O checker resolvido não reaplica vitória e sua porta anterior foi destruída.

O novo Canvas é descartado pelo controle de duplicatas. [InteragirComTerminalCasa](../../Assets/Scripts/Casa/InteragirComTerminalCasa.cs) não reassocia o terminal persistente quando sua referência deixa de existir. A porta nova nasce com `trancada: 1`.

Além disso, o diálogo introdutório grava `ato1_casa_resolvida`, enquanto o checker antigo não grava essa flag. A conversa e a conclusão do puzzle precisam de consequências distintas.

## Solução proposta

Usar conclusão persistente como fonte de estado; manter o controlador do puzzle na própria cena; resolver a referência ao terminal persistente, inclusive inativo; aplicar o estado à nova porta em cada entrada. O VerificadorDePuzzle v2 oferece parte desse fluxo, mas sua inicialização/busca precisa ser revisada antes da migração. Evitar deixar os dois checkers controlando a mesma porta.

## Validação para encerrar

Executar VAL-02/03/05 do [roteiro](../guias/validacao-jogo.md), incluindo duas revisitas e reinício com load. Conversar sem resolver não deve gravar conclusão. Registrar resultado e estado das referências.

O travamento permanente do Player permanece não reproduzido e não é consequência confirmada deste relato.
