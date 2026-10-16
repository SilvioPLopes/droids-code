# Terminal: coroutine do log com painel inativo

**ID:** TER-002.  
**Status:** confirmado no histórico do Console e por leitura; correção pendente.  
**Evidência:** registros de 15/10/2026, consultados pelo MCP em 15–16/10.  
**Prioridade:** [plano](../plano-desenvolvimento.md).

## Reprodução e resultado

Abrir o terminal quando PainelTerminal está inativo, pelo objeto da casa ou pelo menu. O Console contém nove ocorrências deste erro na consulta realizada:

```text
Coroutine couldn't be started because the the game object 'PainelTerminal' is inactive!
TerminalUIManager:AdicionarLinhaDeLog (...) — linha 329
TerminalUIManager:Abrir () — linha 169
DroidsCode.Casa.InteragirComTerminalCasa:Update () — linha 41
```

As linhas correspondem à versão inspecionada; o método é a referência estável para futuras revisões.

## Causa

Em [TerminalUIManager.Abrir](../../Assets/Scripts/Menus/TerminalUIManager.cs), AdicionarLinhaDeLog é chamado antes de ativar o painel. O método inicia a coroutine de rolagem enquanto seu GameObject está inativo.

## Impacto e limites

Há erro de execução e falha na rolagem inicial do log. O histórico de setembro já descrevia esse erro como secundário. Não há evidência de que ele provoque o travamento permanente do Player ou explique, sozinho, a falha de retorno à casa.

## Solução proposta e validação

Garantir que a coroutine rode em um objeto ativo e que o painel esteja pronto antes da rolagem. Revisar a ordem de abertura e inicialização sem acumular registros de menu. Executar VAL-04 no [roteiro](../guias/validacao-jogo.md): primeira abertura, reabertura, menu, interação do mundo, mensagem visível e ausência do erro novo no Console.

Fonte preservada: [investigação de setembro](../historico/2026-09-14-bugs-casalip.md). Esse relato antigo não representa uma correção deste erro.
