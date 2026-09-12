# Droids Code — Documentação Técnica

> Documento único de referência para desenvolvimento. Substitui e unifica
> `MATRIZ_DESENVOLVIMENTO.md`, `ESPECIFICACAO_TECNICA.md`,
> `PLANO_IMPLEMENTACAO.md`, `LEIA_PRIMEIRO.md`, `CORRECAO_FORMULA_DANO.md`,
> `CORRECAO_ARQUITETURA_TERMINAL_MENU.md` e `briefing_terminal_lua_completo.md`.
> Esses arquivos podem ser arquivados (não apagados — têm valor histórico de
> "por que uma decisão foi tomada"), mas deixam de ser a fonte de verdade.
>
> **Regra de manutenção:** este documento descreve o **código como ele é
> hoje**. Sempre que uma classe mudar, atualize a seção correspondente aqui
> no mesmo commit/sessão — não deixe o documento "descolar" do código de
> novo (foi exatamente isso que gerou a necessidade desta unificação).
>
> **Regra de verificação (mantida do LEIA_PRIMEIRO original):** nenhuma
> afirmação de "está consistente" ou "confirmado" entre este documento e o
> código vale sem o arquivo `.cs` real em mãos no momento da checagem. Já
> aconteceram divergências silenciosas neste projeto (fórmula de dano
> documentada 4x menor que a real, tipo de coleção documentado como `List`
> quando era `Dictionary`) — a causa nas duas vezes foi alguém confirmar de
> memória em vez de olhar o arquivo real.

---

## 0. Como um agente de IA deve usar este documento

Este projeto é trabalhado por sessões de chat zeradas (sem IDE, sem acesso ao repositório) — só este documento, o `readme.md`, e os arquivos `.cs` que o responsável do projeto colar na conversa. **Isso muda como você deve se comportar:**

1. **Este documento é um mapa, não o território.** Ele te diz o que cada classe faz, por que existe, e onde fica — o suficiente para você formular uma hipótese e saber **qual arquivo pedir**. Ele não substitui o arquivo real quando a tarefa exige precisão (corrigir um bug, alterar uma assinatura, confirmar um valor exato).
2. **Diagnóstico de bug — fluxo esperado:**
   - Ouça o sintoma.
   - Cruze o sintoma com as responsabilidades descritas na seção 4 (Mapa de classes) para levantar 1–3 arquivos candidatos.
   - Se a seção 9 (Pendências técnicas conhecidas) já tem uma hipótese registrada para esse sintoma, mencione-a — mas trate como hipótese, não como diagnóstico fechado.
   - **Peça o(s) arquivo(s) candidato(s) antes de afirmar a causa ou propor a correção.** Uma hipótese bem fundamentada no mapa ainda é uma hipótese.
3. **Pedido de código novo — fluxo esperado:** identifique, pela seção 4, se a funcionalidade se encaixa em uma classe existente ou exige uma nova; verifique a seção 6 (decisões fechadas) antes de propor qualquer coisa que pareça contradizê-las; verifique a seção 8 (roadmap) para não reintroduzir algo já decidido como "não agora".
4. **Nunca diga "confirmado" ou "está consistente"** entre este documento e o código sem ter o arquivo `.cs` real na conversa no momento da afirmação — nem que pareça óbvio que não mudou. Já houve 2 divergências silenciosas neste projeto por esse motivo exato (ver histórico arquivado em `docs/historico/LEIA_PRIMEIRO.md`).
5. **Sempre que algo relevante mudar nesta sessão** (nova classe, campo renomeado, decisão de arquitetura, bug corrigido, item de roadmap concluído), **edite este documento (e o `readme.md`, se for status/roadmap) diretamente, na mesma sessão** — não só avise que precisa atualizar, aplique a atualização. Edite pontualmente (mover um item de "Roadmap" para "Implementado", ajustar uma linha da fórmula, remover um bug corrigido) em vez de reescrever seções inteiras — isso mantém o documento pequeno e evita reintroduzir o mesmo problema de "documentação que descola do código".
6. **`TCC_-_GDD` e `TCC_-_ENREDO` (história/design de longo prazo) não fazem parte do contexto de desenvolvimento técnico.** Não pedir, não ler, não misturar com este documento — mesmo que existam e estejam desatualizados. Eles só entram na conversa quando a tarefa explicitamente for sobre narrativa/diálogo/conteúdo de fase, nunca para decidir arquitetura, corrigir bug ou priorizar o que implementar a seguir (isso é papel do `CHECKLIST_DE_DESENVOLVIMENTO.md`).

---

## 1. Ambiente confirmado

| Item | Valor |
|---|---|
| Unity | 6 (6000.x) |
| Scripting Backend | Mono |
| API Compatibility Level | .NET Standard 2.1 |
| C# Language Version | 9 — evitar sintaxe C# 10+ |
| Terminal do jogador | Lua via MoonSharp, `CoreModules.Preset_SoftSandbox` (sem `io`, `os`, `load`, `require`, `dofile`) |
| Namespace raiz | `DroidsCode` (sub-namespaces: `DroidsCode.DroidCore`, `DroidsCode.Combat`, `DroidsCode.Scripting`) |
| Convenção de nomes | Classes/métodos/campos públicos em português, PascalCase para tipos/métodos públicos, camelCase para campos privados |

---

## 2. Arquitetura

**Estilo:** Layered Architecture (Arquitetura em Camadas) com Domain Model. Sem Hexagonal/Clean/Onion — o projeto não tem infraestrutura externa trocável (banco, filas, APIs) que justifique o overhead de portas/adaptadores.

```
UI            (BattleManager, MenuMundoManager, TerminalUIManager, TelaDeStatusManager, MainMenuManager)
   ↓
Domínio       (Droid, DroidPart, CombatEngine, IParticipanteDeCombate, ProgressaoDeNivel, SistemaDeProgressao)
   ↑ (consumido como serviço, não como camada abaixo do domínio)
Scripting     (DroidScriptRunner / TerminalDroidApi / MoonSharp)
   ·
Salvamento    (ISistemaDeSalvamento / SalvamentoJson / DadosDoJogo) — cruza Domínio e persistência
```

**Padrões aplicados:**

| Padrão | Onde | Por quê |
|---|---|---|
| Composição sobre herança | `Droid` possui `Braco`/`Perna`/`Tronco`/`Cabeca` | Peças trocáveis em runtime |
| Strategy | `IParticipanteDeCombate` implementado por `Droid` e `InimigoFixo` | `CombatEngine` trata jogador e inimigo sem `if/switch` por tipo |
| SRP | `PontosDeProgressao` (orçamento de pontos) separada de `ProgressaoDeNivel` (nível/XP) separada de `SistemaDeProgressao` (serviço que liga as duas) | Cada uma tem um único motivo para mudar |
| Open/Closed + Dependency Inversion | `CombatEngine` depende de `IParticipanteDeCombate`, nunca de `Droid`/`InimigoFixo` diretamente | Novos tipos de combatente sem alterar `CombatEngine` |
| Singleton (lazy) | `GerenciadorDeEstado` | Precisa sobreviver a troca de cena (Game ↔ Battle) |

**Descartados conscientemente:** Hexagonal/Clean (sem infraestrutura trocável a proteger), Singleton para `DroidScriptRunner`/`TerminalDroidApi` (instanciados por sessão de terminal, sem necessidade de estado global), geração de UI de combate via código do jogador (ver seção 6 — decisão revertida).

---

## 3. Estrutura de pastas (scripts)

> **Corrigido (12/09/2026), confirmado por print do Explorer.** A estrutura
> abaixo divergia da real (dizia `World/`, `DroidCore/`, `Salvamento/`, `UI/`
> — nomes que não existem no projeto). As camadas lógicas da seção 2
> continuam valendo como conceito; só os nomes de pasta físicos mudam.

```
Assets/Scripts/
├── (raiz)      MainMenuManager.cs, MenuMundoManager.cs, PlayerMovement.cs, RestaurarPosicao.cs,
│               EncounterZone.cs, TelaDeStatusManager.cs, TerminalUIManager.cs, BattleManager.cs,
│               readme.md, DOCUMENTACAO_TECNICA.md, CHECKLIST_DE_DESENVOLVIMENTO.md
├── Combat/     CombatEngine.cs, IParticipanteDeCombate.cs, ResultadoAcao.cs, InimigoFixo.cs
├── Droid/      Droid.cs, DroidPart.cs, Braco.cs, Perna.cs, Tronco.cs, Cabeca.cs, DroidStats.cs,
│               TipoAtributo.cs, PontosDeProgressao.cs, ProgressaoDeNivel.cs, SistemaDeProgressao.cs,
│               TabelaDeCustos.cs, TabelaDeCombate.cs, TecnicaComposta.cs, ItemConsumivel.cs,
│               EfeitosTemporariosUtil.cs
├── Estado/     GerenciadorDeEstado.cs, ISistemaDeSalvamento.cs, SalvamentoJson.cs, DadosDoJogo.cs
├── Scripting/  DroidScriptRunner.cs, TerminalDroidApi.cs
└── _Teste/     (conteúdo não verificado ainda — nenhum arquivo de lá foi enviado)
```

---

## 4. Mapa de classes

### 4.1 `DroidCore` — Droid e progressão

**`Droid`** (implementa `IParticipanteDeCombate`) — estado de um combatente configurável.
- `Nome`, `Hp`, `HpMax`, `Braco`/`Perna`/`Tronco`/`Cabeca`, `StatsBase` (`DroidStats`)
- `TecnicasConfiguradas`: **`Dictionary<string, TecnicaComposta>`** (chave = nome da técnica)
- `Pontos`: `PontosDeProgressao` (orçamento gasto no terminal)
- `Progressao`: `ProgressaoDeNivel` (nível/XP ganhos em combate — separado de `Pontos` por SRP)
- `Defesa => ObterTotal(TipoAtributo.Vit)` — hoje é só VIT total; peças ainda não contribuem para defesa
- `EfeitosAtivos`: `List<EfeitoDeAtributo>` (buffs/debuffs de atributo + Stun) e `EfeitosDeDanoAtivos`: `List<EfeitoDeDanoPorTurno>` (Envenenamento) — decrementados 1x por turno via `DecrementarEfeitosAtivos()` (usa `EfeitosTemporariosUtil.Decrementar<T>`, compartilhado com `InimigoFixo`)
- `ExecutarAcao(nomeAcao, alvo)`: lê a técnica configurada, calcula dano (ver fórmula na seção 5), **copia** os efeitos da técnica (`EfeitosDeAtributo`/`EfeitosDeDanoPorTurno`) pro `alvo.EfeitosAtivos`/`alvo.EfeitosDeDanoAtivos` (cópia, não referência — cada uso da técnica gera uma instância própria, sem compartilhar `DuracaoEmTurnos`) — nunca chama Lua
- `ObterTotal(TipoAtributo)`: base (`StatsBase`) + bônus de peça (hoje sempre 0) + **bônus de efeitos ativos** (soma `Valor` de cada `EfeitoDeAtributo` ativo que bate com o atributo consultado — corrigido em 12/09/2026, antes retornava sempre 0 de bônus de efeito)

**`DroidPart`** (abstrata) + `Braco`/`Perna`/`Tronco`/`Cabeca` — só dado (`Nome`, `AtributoPrincipal`, `Raridade`), sem comportamento próprio. Isso é intencional na fase atual (Fase 1 do design); a herança de verdade com métodos sobrescritos é Fase 2 (roadmap).

**`PontosDeProgressao`** — orçamento gasto no terminal. `GanharPontosPorNivel(niveis)`, `TentarGastar(quantidade)`, `DefinirPontos(quantidade)` (uso exclusivo do sistema de salvamento, para restaurar valor exato — não usar em fluxo normal).

**`ProgressaoDeNivel`** — nível e XP ganhos em combate. `GanharExperiencia(xpGanho)` retorna quantos níveis subiram; `ExperienciaNecessariaProximoNivel = Nivel * 100` (placeholder). `Definir(nivel, experiencia)` é uso exclusivo do sistema de salvamento.

**`SistemaDeProgressao`** (estático) — serviço de domínio que liga as duas classes acima: recebe XP, decide se sobe nível, concede pontos se sim.

**`TipoAtributo`** — enum `{ For, Agi, Vit, Int, Dex, Luk }`. Não expandir (DEF/FLEE/ATK% etc. como valores derivados) sem decisão explícita — ver roadmap, seção 8.

**`TabelaDeCustos`** (placeholder, não balanceado) — `CustoUpgradeAtributo = 1`; `CustoDanoPorNivel = 1`; `CustoResistenciaPorNivel = 1` (**declarada mas não usada em nenhum cálculo hoje** — verificar se ainda é necessária); `CustoPorTurnoDeEfeito`: Stun = 3/turno, Envenenamento = 2 (não confirmado se escala por turno como Stun).

**`TabelaDeCombate`** (placeholder) — `DanoPorNivelDeTecnica = 5`. Separada de `TabelaDeCustos` de propósito: uma é custo em pontos, outra é output de dano.

**`TecnicaComposta`** — `Nome`, `NivelDeDano`, `EfeitosDeAtributo` (`List<EfeitoDeAtributo>`), `EfeitosDeDanoPorTurno` (`List<EfeitoDeDanoPorTurno>`), `CustoTotal()`. `EfeitoDeAtributo` liga-se a um `TipoAtributo` + valor + duração (`-1` = permanente, convenção não usada ainda). `EfeitoDeDanoPorTurno` é dano contínuo (ex: Envenenamento), modelado como tipo próprio por não caber no formato de "alterar atributo".
> ✅ **Corrigido (12/09/2026)** — motor de efeitos por turno implementado: `Droid.ExecutarAcao` agora copia `EfeitosDeAtributo`/`EfeitosDeDanoPorTurno` da técnica pro alvo; `CombatEngine.ExecutarTurno` aplica dano por turno no início do turno de quem está afetado e checa Stun (flag `NomeExibicao == "Stun"`) antes de deixar agir; `IParticipanteDeCombate` ganhou `EfeitosAtivos`/`EfeitosDeDanoAtivos`/`DecrementarEfeitosAtivos()` no contrato, então `InimigoFixo` também sofre Stun/Envenenamento (sem buff de atributo — ele não tem stats configuráveis como o Droid). Nova classe `EfeitosTemporariosUtil` (em `Droid/`) compartilha a lógica de decremento/expiração entre `Droid` e `InimigoFixo`.

### 4.2 `Combat` — combate

**`IParticipanteDeCombate`** — contrato: `Nome`, `Hp` (get/set), `HpMax` (get), `Defesa` (get), `EfeitosAtivos`/`EfeitosDeDanoAtivos`/`DecrementarEfeitosAtivos()` (motor de efeitos, adicionado 12/09/2026), `ObterAcoesDisponiveis()`, `ExecutarAcao(nomeAcao, alvo)`.

**`ResultadoAcao`** — `Sucesso`, `DanoCausado`, `Mensagem`. Mínima de propósito — não adicionar campos (efeito visual, combo, status) sem necessidade concreta.

**`CombatEngine`** — sem estado. `ExecutarTurno(atacante, alvo, nomeAcao)`: aplica dano por turno em `atacante` (Envenenamento), checa Stun (pula a ação se `atacante` tiver `EfeitoDeAtributo` com `NomeExibicao == "Stun"` e duração > 0), senão delega para `atacante.ExecutarAcao` e aplica dano ao HP do alvo; decrementa os efeitos de `atacante` no fim. `VerificarDerrota(participante)` = `Hp <= 0` (nenhuma outra condição implementada).

**`InimigoFixo`** (implementa `IParticipanteDeCombate`) — comportamento 100% fixo em C#, uma única ação ("Atacar"). Tem `RecompensaXp`, propositalmente fora da interface genérica (só o `BattleManager` referencia `InimigoFixo` diretamente). Sofre Stun/Envenenamento como o Droid, mas sem buff de atributo (não tem `ObterTotal`/stats configuráveis).

### 4.3 `Scripting` — terminal

**`DroidScriptRunner`** — cria a VM Lua (`Script(CoreModules.Preset_SoftSandbox)`), registra `TerminalDroidApi` como único tipo exposto (`droid` global). `ExecutarCodigo(codigoLua)` roda o script e captura exceções de sintaxe/execução.
> Hoje só existe **um modo** de execução (equivalente ao "Modo Configuração" do design original). O **Modo Puzzle** (exercícios isolados, nunca tocando o Droid real) está no roadmap — ver seção 8.

**`TerminalDroidApi`** — única fachada exposta ao Lua; `Droid` nunca é registrado diretamente no MoonSharp.
- `SubirAtributo(nomeAtributo, quantidade)`: valida quantidade > 0, nome de atributo válido, pontos suficientes; aplica e loga.
- `AprenderTecnica(nome, nivelDeDano)`: valida nome/nível, checa `TecnicaComposta.CustoTotal()` contra pontos; aplica e loga.
- `ObterPontosDisponiveis()`, `ObterLogDaSessao()`, `LimparLog()`.
> ✅ **Corrigido (12/09/2026)** — os três métodos que o texto de ajuda já citava foram implementados em `TerminalDroidApi`:
> - `ObterAtributo(nomeAtributo)`: leitura pura (não gasta pontos), retorna `Droid.ObterTotal(atributo)`; nome inválido retorna 0 e loga falha.
> - `ListarTecnicas()`: retorna os nomes de `TecnicasConfiguradas` separados por vírgula, ou "Nenhuma técnica configurada.".
> - `EsquecerTecnica(nome)`: remove a técnica e devolve os pontos gastos (`tecnica.CustoTotal()`) via `Pontos.DefinirPontos(...)`; recusa remover o Ataque Básico.

### 4.4 `Salvamento` — persistência

**`ISistemaDeSalvamento`** — `Salvar()`, `Carregar()`, `ExisteSave()`.

**`SalvamentoJson`** (implementação) — salva/carrega `Application.persistentDataPath/save.json`. Cobre: stats/HP/pontos/nível/XP/técnicas do Droid, posição real do Player (via `GameObject.FindGameObjectWithTag("Player")`), cena atual, e flags de história (`Dictionary<string, bool>` em `GerenciadorDeEstado`, convertido para lista plana porque `JsonUtility` não serializa `Dictionary`).
Ao carregar, recarrega a cena salva (`SceneManager.LoadScene`) — depende de `RestaurarPosicao` (World) para de fato teleportar o player, reaproveitando o mecanismo Game↔Battle em vez de duplicar lógica de teleporte.

**`DadosDoJogo`** / `DroidSalvo` / `TecnicaSalva` / `FlagDeHistoria` — DTOs planos (`[Serializable]`), sem `Dictionary`, separados dos objetos de domínio de propósito.

### 4.5 `World` — mundo e estado entre cenas

**`GerenciadorDeEstado`** (Singleton lazy, `DontDestroyOnLoad`) — guarda `DroidDoJogador` (mesma instância entre cenas), posição salva para transição Game↔Battle, flags de história, e um contador de menus abertos (`MenuAberto`, incrementado/decrementado por `RegistrarMenuAberto`/`RegistrarMenuFechado`) usado para congelar o movimento do player enquanto um menu/terminal está aberto.

**`PlayerMovement`** — move via `Rigidbody2D`, alimenta `Animator` (MoveX/MoveY/Speed), não se move se `GerenciadorDeEstado.MenuAberto`.

**`EncounterZone`** — sorteia encontro aleatório a cada "passo" dentro de uma zona de trigger; salva a posição do player antes de trocar para a cena de batalha.

**`RestaurarPosicao`** — componente anexado ao Player na cena Game; teleporta para a posição salva se `GerenciadorDeEstado` tiver uma posição pendente **para aquela cena especificamente**, e limpa a posição salva em seguida.

> ✅ **Corrigido (12/09/2026)** — bug "personagem para de andar após salvar/carregar": `GerenciadorDeEstado` persiste entre cenas via `DontDestroyOnLoad`. Ao abrir o Menu do Mundo, `_contadorMenusAbertos` sobe para 1. Ao clicar em "Carregar", `SalvamentoJson.Carregar()` chamava `SceneManager.LoadScene`, que destruía o `MenuMundoManager` da cena antiga **sem que `RegistrarMenuFechado()` fosse chamado** — o contador nunca voltava a 0. A posição sempre foi restaurada corretamente porque usa outro mecanismo (`PosicaoSalva`, não o contador de menus).
>
> **Correção aplicada:**
> - `GerenciadorDeEstado.cs`: adicionado `public void ZerarMenusAbertos() => _contadorMenusAbertos = 0;`
> - `SalvamentoJson.cs`, em `Carregar()`: chamado `gerenciador.ZerarMenusAbertos();` imediatamente antes do `SceneManager.LoadScene(dados.cena);`
> - Roteiro de validação: `ROTEIRO_DE_TESTES_BUG_MENU_ABERTO.md`

### 4.6 `UI` — apresentação

**`BattleManager`** — não calcula nada, só orquestra: botões fixos (Atacar/Item/Fugir/Status), lista dinâmica de técnicas se o Droid tiver mais de uma configurada, corrotinas de turno, integração com `TelaDeStatusManager`. Botão "Item" (corrigido em 12/09/2026) abre `painelListaDeItens` com os itens de `ItensDeBatalha.Disponiveis` (lista fixa, ver `ItemConsumivel.cs`) — não cura mais sozinho automaticamente.

**`MenuMundoManager`** — menu de pausa (tecla Esc): Status/Terminal/Bag/Droid/Opções/Salvar/Carregar/Voltar. Bag/Droid/Opções ainda são placeholders ("ainda não foi implementado").

**`TerminalUIManager`** — UI do terminal: campo de código, histórico de comandos (↑/↓), log de sessão, atalho Ctrl+Enter.

**`TelaDeStatusManager`** — painel de status, busca o Droid direto do `GerenciadorDeEstado` (reutilizável entre batalha e mundo).

**`MainMenuManager`** — navegação do menu principal (Novo Jogo/Configurações/Sair). Sem mudanças estruturais previstas.

---

## 5. Fórmulas e valores atuais

**Dano de combate** (vigente, testado em batalha real):
```
statusAtk    = FOR_total × 2
bonusTecnica = NivelDeDano × TabelaDeCombate.DanoPorNivelDeTecnica   (= 5 hoje)
totalAtk     = statusAtk + bonusTecnica
dano         = max(1, totalAtk − Defesa_do_alvo)
```
Exemplo: FOR 5, técnica NivelDeDano 3, alvo com Defesa 3 → `10 + 15 − 3 = 22`.

**Defesa** = `ObterTotal(TipoAtributo.Vit)` — sem bônus de peça/equipamento ainda.

**Derrota** = `Hp <= 0`. Nenhuma outra condição implementada.

**Progressão:** 5 pontos por nível (`PontosDeProgressao.PontosPorNivel`), XP necessário por nível = `Nivel × 100` (`ProgressaoDeNivel`). Ambos placeholders de rascunho, não balanceados.

**Custo de upgrade de atributo:** 1 ponto por unidade de FOR/AGI/VIT/INT/DEX/LUK.

**Todos os valores acima são placeholders isolados em classes `static` centralizadas, para ajuste fácil quando houver playtesting real.** Não são decisão final de game design.

---

## 6. Decisões estruturais fechadas (não reabrir sem necessidade real)

- **Lua nunca roda durante uma batalha.** O terminal só é acessível fora de combate. O dano é 100% calculado em C#, lendo dados já configurados.
- **Lua nunca mexe direto numa propriedade do Droid.** Só métodos validados (`SubirAtributo`, `AprenderTecnica`), que conferem pontos antes de aplicar.
- **O jogador não cria botões de combate via código.** O menu de batalha é fixo (Lutar/Itens/Status/Fugir); dentro de "Lutar", a lista de técnicas é dado, não geração de UI por código. (Essa era a premissa original do projeto — foi tentada, revertida, e não deve ser retomada.)
- **`CombatEngine` nunca ramifica por tipo concreto de participante** — sempre via `IParticipanteDeCombate.ExecutarAcao` (Strategy).
- **Inimigos não usam Lua.** Comportamento 100% fixo em C#.

---

## 7. Convenções de código

- Nomes de classes/métodos/campos públicos em português.
- Propriedades automáticas (`{ get; set; }`) em vez de campos públicos crus.
- Classes de domínio (`Droid`, `DroidPart`, `CombatEngine`, etc.) não referenciam `UnityEngine.MonoBehaviour` — são C# puro, testáveis sem a Unity rodando.
- Valores de balanceamento sempre centralizados em uma classe `static` própria (`TabelaDeCustos`, `TabelaDeCombate`), nunca espalhados como números mágicos.

---

## 8. Roadmap (documentado, ainda não implementado)

| Item | Descrição | Status |
|---|---|---|
| Sistema de acerto/erro (HIT/FLEE) | Atributos derivados ATK/DEF/HIT/FLEE/HpMax completos, com chance de acerto baseada em HIT do atacante vs. FLEE do alvo | Planejado, não implementado — hoje todo ataque sempre acerta |
| Modo Puzzle do terminal | Exercícios de lógica isolados (`DefinicaoDePuzzle`/`ResultadoDePuzzle`), nunca tocando o Droid real, avaliados por valor final de variável | Planejado, não implementado |
| `DroidDataSO` / `ItemDataSO` / Factory | Migrar criação de Droid/itens de código direto para ScriptableObjects configuráveis no Inspector | Planejado, não implementado |
| Fase 2 de peças (`DroidPart`) | Herança real via Cartuchos de Código sobrescrevendo `DroidBase` | Planejado (design), não implementado |
| Inventário e equipamento completos | Hoje existe só `ItensDeBatalha.Disponiveis` (lista fixa de 4 itens de cura, sem quantidade/persistência/drop) — inventário real (biblioteca maior, quantidades, itens de status/situacionais) ainda não existe | Planejado, não implementado |

---

## 9. Pendências técnicas conhecidas

- `TabelaDeCustos.CustoResistenciaPorNivel` está declarada mas não é referenciada em nenhum cálculo — confirmar se ainda é necessária ou remover.
- MoonSharp não tem proteção contra loop infinito (`while true do end` travaria o jogo) — sem solução ainda, fora de escopo imediato.
