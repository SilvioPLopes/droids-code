# Bug: puzzle de água da CasaLip não destrava a porta / não mostra fala do Apollo [RESOLVIDO]

**Aberto em:** 14/09/2026
**Resolvido em:** 14/09/2026 — confirmado pelo usuário: "o evento dispara normalmente" após o log de diagnóstico (`Scripts/Scripting/DroidScriptRunner.cs`). Causa exata não detalhada pelo usuário na confirmação; o log temporário pode ser removido quando conveniente.
**Cena:** `CasaLip.unity`
**Scripts envolvidos:** `PuzzleAguaCasaChecker.cs`, `TerminalUIManager.cs`, `DroidScriptRunner.cs` (ainda não visto nesta sessão), `TerminalDroidApi.cs` (ainda não visto nesta sessão)

## Sintoma

1. Jogador abre o terminal da casa, digita `nivel_agua = 100` e clica Executar.
2. Terminal mostra `(código executado, nenhuma ação de droid chamada)` — ou seja, o Lua rodou sem erro.
3. A porta (`PortaDeSaidaCasa`) continua trancada.
4. A fala de vitória do Apollo não aparece.
5. O Console mostra, a cada execução:
   ```
   [PuzzleAguaCasaChecker] Variável 'nivel_agua' ainda não existe na sessão Lua.
   ```
   (log de `PuzzleAguaCasaChecker.VerificarVitoria()`, linha 92)

## Hipóteses já refutadas (confirmado por teste/inspeção real)

| # | Hipótese | Por que foi descartada |
|---|----------|--------------------------|
| 1 | `textoDeApollo` sem referência no Inspector | Corrigido e confirmado por print do Inspector: campo aponta pra `TextoApollo (Text Mesh Pro UGUI)`. |
| 2 | `transicaoDaPortaDeSaida` sem referência | Confirmado por print do Inspector: aponta pra `PortaDeSaidaCasa (Transicao De Cena) (CasaLip)`. |
| 3 | `nomeDaVariavel` não bate com o que o jogador digita | Confirmado igual nos dois lados: campo no Inspector é `nivel_agua`, comando digitado no terminal é `nivel_agua = 100`. |
| 4 | Componente `PuzzleAguaCasaChecker` desativado (checkbox desmarcado) | Confirmado por print: checkbox marcado. |
| 5 | Componente está num GameObject diferente do `TerminalUIManager` (painel errado) | Confirmado por print: os dois componentes aparecem empilhados no mesmo Inspector, mesmo GameObject `PainelTerminal`. |
| 6 | Checker nunca inscrito no evento `AoExecutarCodigo` (`OnEnable` nunca roda) | Refutado pelo próprio Console: a linha de log de `VerificarVitoria()` aparece a cada clique em Executar — o método está sendo chamado normalmente. |
| 7 | Terminal não abre com a tecla E (`InteragirComTerminalCasa` / campo `Terminal` vazio / Tag do Player / collider sem Is Trigger) | Refutado: o terminal abriu, o campo aceitou o comando e o botão Executar funcionou normalmente nas duas tentativas do log. |
| 8 | `GerenciadorDeEstado.Instancia` nulo por testar a cena direto (sem passar pelo Main Menu) | Refutado pelo mesmo motivo do item 7 — `Abrir()` completou (só esbarrou no bug cosmético do item "Bug secundário" abaixo, que é depois da linha que usa `GerenciadorDeEstado.Instancia`). |
| 9 | `ExecutarCodigo` cria um novo `Script`/ambiente Lua a cada execução, descartando o estado anterior antes de `ObterVariavelNumerica` ler | **Refutado por leitura do código** (`DroidScriptRunner.cs` recebido em 14/09/2026): o `Script _vm` só é criado uma vez, no construtor de `DroidScriptRunner` — e esse construtor só roda dentro de `TerminalUIManager.Abrir()`. Ou seja, `ExecutarCodigo` e `ObterVariavelNumerica` usam o **mesmo** `_vm.Globals` durante toda a sessão em que o terminal fica aberto. As duas tentativas do log (22:17:15 e 22:17:41) não tiveram um "Terminal aberto" no meio, então foi a mesma sessão/mesmo `_vm` nos dois casos — essa hipótese não explica a falha. |
| 10 | `ObterVariavelNumerica` lê de uma tabela diferente de onde o Lua escreveu (`_G` vs. environment customizado) | **Não encontrado nenhum environment customizado no código.** `_vm.DoString(codigoLua)` roda no ambiente padrão do MoonSharp, cujo `_ENV` é exatamente `_vm.Globals` — é o mesmo objeto que `ObterVariavelNumerica` consulta via `_vm.Globals.Get(...)`. Esse é o idioma padrão e documentado do MoonSharp pra ler globals setadas por Lua, sem nenhum wrapper estranho no meio. Não refutado com 100% de certeza (não dá pra ver o valor real em runtime só lendo o código), mas nada no código aponta pra essa causa. |

## Validação de 14/09/2026 (após receber DroidScriptRunner.cs e TerminalDroidApi.cs)

Os dois arquivos foram revisados e **o padrão de uso do MoonSharp está correto**: `_vm.Globals["droid"] = api` no construtor, `_vm.DoString(codigoLua)` em `ExecutarCodigo`, e `_vm.Globals.Get(nomeDaVariavel)` em `ObterVariavelNumerica` são exatamente a forma padrão de setar/ler uma variável global Lua via MoonSharp. Não foi encontrado nenhum bug óbvio de código nesses dois arquivos que explique `nivel_agua` "não existir" depois de `nivel_agua = 100` ser executado sem erro no mesmo `_vm`.

Ou seja: a leitura de código, sozinha, não é suficiente pra achar a causa raiz — precisa de um dado que só existe em runtime (o que de fato está dentro de `_vm.Globals` logo depois da execução).

**Achado à parte (não é a causa deste bug, mas é comportamento real do sistema):** um `DroidScriptRunner` novo (com `_vm` zerado) é criado toda vez que `Abrir()` roda — ou seja, fechar e reabrir o terminal apaga `nivel_agua` e qualquer outra variável Lua da sessão anterior. Não foi o que causou esta falha específica (não houve reabertura entre as duas tentativas), mas é uma decisão de design que vale confirmar: o puzzle deveria permitir fechar/reabrir o terminal sem perder o progresso da variável, ou resolver tudo numa sessão só é intencional?

## Resolução

Como a leitura de código não achou a causa, foi instrumentado `ObterVariavelNumerica` com um log temporário que lista TODAS as chaves que existem de fato em `_vm.Globals` no momento da checagem (patch em `Scripts/Scripting/DroidScriptRunner.cs`, marcado como DEBUG TEMPORÁRIO). Usuário rodou de novo com esse log e confirmou: **"o evento dispara normalmente"** — bug resolvido. Causa raiz exata (o que exatamente estava faltando em `_vm.Globals` antes) não foi detalhada na confirmação. O log temporário de diagnóstico continua no arquivo e pode ser removido quando conveniente.

## Bug secundário encontrado (cosmético, não é a causa do bug principal)

```
Coroutine couldn't be started because the game object 'PainelTerminal' is inactive!
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
```

Em `TerminalUIManager.Abrir()`, a chamada `AdicionarLinhaDeLog("=== Terminal aberto...")` acontece **antes** de `painelTerminal.SetActive(true)`. Nesse instante o GameObject ainda está inativo, então o `StartCoroutine(RolarParaFimNoProximoFrame())` chamado de dentro de `AdicionarLinhaDeLog` falha. Só afeta a rolagem automática da primeira linha de log ao abrir o terminal — não trava nada e não tem relação com o puzzle. Ainda não corrigido (baixa prioridade).

## Pendência de arquitetura (separada do bug, levantada pelo usuário nesta sessão)

O jogo precisa poder ser testado dando Play direto em qualquer cena (ex: `CasaLip`), sem depender de passar pelo Main Menu antes. Isso ainda não foi validado a fundo porque a hipótese nº8 acima não chegou a se confirmar como causa real neste caso — mas fica registrado como necessidade de projeto pra revisar como `GerenciadorDeEstado.Instancia` é inicializado (hoje, aparentemente, só no fluxo normal a partir do Main Menu).

---

# Bug 2: tecla E não abria o terminal da casa [RESOLVIDO]

**Aberto em:** 14/09/2026
**Resolvido em:** 14/09/2026 — confirmado pelo usuário.
**Script envolvido:** `InteragirComTerminalCasa.cs`

## Sintoma

Jogador anda até a frente do terminal (o colisor do gatilho de interação) e aperta E. Nada acontece — sem erro no Console, sem o painel abrir.

## Investigação

Print do Inspector do GameObject com `InteragirComTerminalCasa` mostrou todos os campos do script corretos:
- **Terminal**: `PainelTerminal (Terminal UI Manager)` — referenciado corretamente.
- **Tecla De Interacao**: `E`.
- **Tag Do Player**: `Player`.

O problema não estava no script `InteragirComTerminalCasa` em si, e sim no **Box Collider 2D** do mesmo GameObject: o checkbox **"Is Trigger" estava desmarcado**. Sem `Is Trigger` marcado, o Unity trata o collider como sólido normal — `OnTriggerEnter2D`/`OnTriggerExit2D` nunca disparam, então `_playerPorPerto` nunca vira `true`, e o `if` dentro de `Update()` nunca chama `terminal.Abrir()`. Isso explica o silêncio total (nenhum erro, porque tecnicamente nada de errado acontece do ponto de vista do C#, o evento simplesmente nunca é gerado).

`InteragirComTerminalCasa.Reset()` normalmente marca `isTrigger = true` sozinho, mas esse método só roda automaticamente na primeira vez que o componente é adicionado pelo Editor — se o Collider2D já existia antes (ou foi reconfigurado depois), o `Reset()` não é chamado de novo, e o valor pode ficar desmarcado sem nenhum aviso.

## Correção

Só configuração no Editor, sem mudança de código: marcar **Is Trigger** no Box Collider 2D do GameObject do gatilho de interação do terminal.

## Observação adicional (mesma sessão de testes)

Como parte de deixar a cena funcionando de ponta a ponta, o usuário também conferiu/marcou o campo **Trancada** (bool público adicionado em `TransicaoDeCena.cs`, ver correção anterior do bug da porta) no componente `Transicao De Cena` da `PortaDeSaidaCasa`, garantindo que a porta nasce travada. Essa configuração já fazia parte do guia de Editor entregue na correção do bug da porta (passo 4) — não é uma causa nova, só o fechamento desse passo pendente.
