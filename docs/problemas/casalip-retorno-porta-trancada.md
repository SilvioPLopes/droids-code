# CasaLip: porta e terminal após retornar da City

**ID:** INT-001.  
**Status:** confirmado por observação MCP; correção pendente.  
**Evidência:** sessão de investigação de 15–16/10/2026.  
**Cena:** [CasaLip.unity](../../Assets/Scenes/CasaLip.unity).  
**Prioridade e dependências:** [plano](../plano-desenvolvimento.md).

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
