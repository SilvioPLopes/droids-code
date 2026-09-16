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

## 0.1 Regras obrigatórias de comportamento e entrega (13/09/2026)

> Adicionado depois de retrabalho real causado por agentes que: (a) não leram
> todos os arquivos enviados antes de responder, (b) questionaram código que
> o responsável do projeto já havia afirmado ser o estado real (transplante),
> (c) fizeram perguntas de esclarecimento em rodadas sucessivas em vez de uma
> única vez, e (d) entregaram código solto em vez de zip com estrutura de
> pasta. Estas regras existem pra fechar esse vácuo — elas são tão
> vinculantes quanto a seção 6 (decisões fechadas).

**a) Leitura é obrigatória e completa, não amostral.**
Se o responsável do projeto enviar N arquivos (código ou documentação) e
disser "leia tudo antes de responder", o agente lê o conteúdo de **todos os
N arquivos**, um por um, antes de formular qualquer resposta — mesmo que
sejam 40 arquivos, mesmo que pareçam repetitivos. Se algum arquivo listado
como enviado não aparecer com conteúdo visível na conversa, o agente busca
ativamente (ex: lendo do disco/upload) antes de dizer que não recebeu —
"não recebi esse arquivo" só é uma resposta válida depois dessa checagem.
Não é aceitável responder com base em título/nome de arquivo, resumo prévio,
ou suposição do que o arquivo "provavelmente" contém.

**b) Código informado como "estado real do projeto" não é questionado.**
Quando o responsável do projeto reenvia um `.cs` afirmando que é o código
atualmente no projeto (transplante), o agente trata isso como fato dado —
não pergunta "isso já está assim?", não pede confirmação, não sugere que
pode haver divergência. A dúvida documentada na seção 0 (item 4, sobre não
afirmar "consistente" sem o arquivo em mãos) é sobre o agente **verificar
antes de declarar consistência por conta própria** — nunca sobre
desconfiar de uma afirmação direta do responsável do projeto sobre o que
está no projeto dele.

**c) Um único alinhamento, no início — não perguntas em série.**
Se o agente precisa de mais contexto pra executar uma tarefa, ele levanta
**todas** as dúvidas de uma vez, num único momento de alinhamento, antes de
começar o trabalho. Depois desse alinhamento, o agente prossegue com as
informações que tem, assumindo a interpretação mais razoável e registrando
a suposição explicitamente na entrega — não interrompe de novo pra
perguntar mais uma coisa, e mais outra, em rodadas separadas. Se o agente
sente que vai precisar perguntar de novo depois, o momento de perguntar era
antes, não depois.

**d) Toda entrega de código vem como ZIP com a estrutura de pastas real.**
Sempre que a tarefa envolver criar/alterar `.cs` (um arquivo ou vários), a
entrega final é um `.zip` reproduzindo a árvore de pastas da seção 3 (ex:
`Assets/Scripts/Droid/Droid.cs`, não só `Droid.cs` solto) — mesmo que seja
uma alteração em um único arquivo. Isso evita o responsável do projeto ter
que adivinhar em qual pasta cada arquivo entra ao importar de volta no
Unity.

**e) Passo a passo de Unity é completo e sem hedging.**
Quando a tarefa exige configuração no Editor (arrastar referência, criar
Prefab, configurar Inspector), o agente descreve o passo a passo completo e
definitivo — nome exato de campo, ordem exata de ação — em vez de "você
provavelmente vai precisar configurar algo como X" ou perguntas do tipo
"você já tem um Prefab pra isso?". Se a resposta a essa pergunta mudasse o
passo a passo, o agente apresenta as duas variações dentro da mesma
resposta (ex: "se já existe o Prefab X, faça A; se não existe, crie assim:
B"), em vez de parar e perguntar.

**f) Respostas afirmativas, não hedged.**
Evitar linguagem de dúvida desnecessária ("acho que", "pode ser que",
"acredito que") quando a informação já está nos arquivos lidos. Se algo
realmente não está determinável pelos arquivos, o agente diz isso
explicitamente e de forma direta ("isso não está definido em nenhum arquivo
recebido — decisão pendente"), em vez de especular com qualificadores
vagos.

---

## 0.2 Os três documentos são UMA entidade, não três documentos separados (13/09/2026)

`readme.md`, `DOCUMENTACAO_TECNICA.md` e `CHECKLIST_DE_DESENVOLVIMENTO.md` são
**partes de um único sistema de contexto**, separados por conveniência de
leitura (o que o jogo é / como o código está / o que fazer agora) — não são
independentes e nunca devem ser tratados como se um pudesse mudar sem checar
os outros dois.

**Regra obrigatória:** sempre que uma sessão alterar o estado do projeto —
classe nova, comportamento mudado, bug corrigido, item de roadmap concluído,
decisão de design fechada — o agente verifica, **antes de encerrar a
resposta**, se essa mudança exige atualização correspondente **nos três
documentos**, não só naquele que estava "no assunto" da tarefa. Corrigir só
um e deixar o outro desatualizado é o mesmo erro que a seção 0 já pede pra
evitar dentro de um único documento — só que espalhado entre três arquivos,
o que o torna mais fácil de passar despercebido, não menos grave.

**Correspondências que precisam ficar sempre batendo:**

| Se isto mudar em... | Isto tem que refletir em... |
|---|---|
| `CHECKLIST` marca algo como "✅ Concluído" envolvendo classe nova/alterada | `DOCUMENTACAO_TECNICA` §3 (árvore de pastas, se houver arquivo novo) e §4 (mapa de classes) |
| Um item do roadmap (`DOCUMENTACAO_TECNICA` §8) é concluído | Remover/mover o mesmo item no roadmap do `readme.md` — nunca só num dos dois |
| `readme.md` diz que algo está "✅ Implementado e funcional" | `DOCUMENTACAO_TECNICA` §4 tem que descrever exatamente esse comportamento — nunca "funcional" num lugar e "planejado" no outro para a mesma feature |
| Uma pendência é resolvida (`DOCUMENTACAO_TECNICA` §9) | Se o `CHECKLIST` também listava o mesmo item em "🔴 Bugs abertos", removê-lo de lá também |

**Verificação de saída:** ao final de qualquer resposta que altere um destes
três documentos, o agente relê os outros dois e **relata explicitamente**
(pro responsável do projeto) quais divergências encontrou e corrigiu — não
corrige silenciosamente e não presume que "só esse documento estava
desatualizado" sem checar.

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

> **Regra (12/09/2026):** cada arquivo abaixo tem o **caminho completo**
> listado explicitamente, um por linha — não apenas agrupado por pasta.
> Motivo: em refatorações grandes é comum aparecerem arquivos com o mesmo
> nome em lugares diferentes (ex: um `TecnicaSalva.cs` histórico vs. o atual
> em `Salvamento/`), e uma lista solta de nomes dentro da pasta não deixa
> isso claro. Sempre que este projeto for exportado/zipado para análise
> externa, a árvore de pastas real deve corresponder a esta lista — se um
> arquivo mudar de pasta, atualize a linha correspondente aqui no mesmo
> commit/sessão (mesma regra de manutenção da seção 0).
>
> **✅ Correção (13/09/2026):** a árvore abaixo estava desatualizada —
> descrevia pastas (`World/`, `DroidCore/`, `Salvamento/`, `UI/`) que **não
> correspondem** à estrutura real do projeto. Substituída pela árvore real,
> confirmada via print do Windows Explorer em 13/09/2026 (exatamente o tipo
> de divergência silenciosa que a seção 0 pede pra evitar). Principais
> diferenças: os arquivos de `UI/` (`BattleManager`, `MenuMundoManager`,
> `TerminalUIManager`, `TelaDeStatusManager`, `MainMenuManager`) na verdade
> ficam soltos na raiz de `Scripts/`, junto com `PlayerMovement`,
> `EncounterZone`, `RestaurarPosicao` (que a versão antiga agrupava em
> `World/`); `DroidCore/` chama-se `Droid/` na pasta real; `Salvamento/` e
> `GerenciadorDeEstado.cs` (que estava em `World/`) foram unificados numa
> única pasta `Estado/`. `GerenciadorDeEstado.cs` também está bem mais
> recente (12/09 02:09) do que o resto de `Estado/`, mas isso é só data de
> modificação — não indica arquivo desatualizado.
>
> **✅ Correção (14/09/2026):** confirmada via print do Windows Explorer.
> Três pastas novas em `Scripts/`: `Casa/` (cena `CasaLip`, puzzle de
> introdução — ver §4.7), `Camera/` (`CameraFollow.cs`, ver §4.6) e
> `Mapas/` (existe na raiz de `Scripts/`, conteúdo ainda não visto nesta
> sessão — não documentado abaixo até alguém confirmar o que tem dentro).
> `CatalogoDeItens.cs` (pasta `Droid/`) foi confirmado como o nome real —
> o "l" minúsculo no Explorer só parecia um "I" maiúsculo, não é um
> arquivo/typo diferente.

```
Assets/Scripts/BattleManager.cs
Assets/Scripts/MainMenuManager.cs
Assets/Scripts/MenuMundoManager.cs
Assets/Scripts/TelaDeStatusManager.cs
Assets/Scripts/TelaDeDroidManager.cs
Assets/Scripts/TerminalUIManager.cs
Assets/Scripts/BagUIManager.cs
Assets/Scripts/EncounterZone.cs
Assets/Scripts/PlayerMovement.cs
Assets/Scripts/RestaurarPosicao.cs
Assets/Scripts/TransicaoDeCena.cs
Assets/Scripts/LojaUIManager.cs
Assets/Scripts/NpcInterativo.cs
Assets/Scripts/readme.md
Assets/Scripts/DOCUMENTACAO_TECNICA.md
Assets/Scripts/CHECKLIST_DE_DESENVOLVIMENTO.md

Assets/Scripts/Camera/CameraFollow.cs — novo (14/09/2026), ver §4.6

Assets/Scripts/Historia/DialogoUIManager.cs — novo (15/09/2026), ver §4.8
Assets/Scripts/Historia/GatilhoDeDialogo.cs — novo (15/09/2026), ver §4.8 (contém também FalaDeDialogo, EscolhaDeDialogo e a classe estática CondicoesDeHistoria)
Assets/Scripts/Historia/CondicaoDeHistoria.cs — novo (15/09/2026), ver §4.8
Assets/Scripts/Historia/GatilhoDeBatalha.cs — novo (15/09/2026), ver §4.8
Assets/Scripts/Historia/VerificadorDePuzzle.cs — v2 (15/09/2026), confirmado por .cs real: lê CatalogoDePuzzles por Id, mantém modo manual como fallback. Ver §8.1
Assets/Scripts/Historia/TrancaPorHistoria.cs — novo (15/09/2026), ver §4.8
Assets/Scripts/Historia/ApolloDica.cs — novo (15/09/2026), ver §4.8

Assets/Scripts/Historia/GerenciadorDeQuests.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/QuestUIManager.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/CatalogoDeQuests.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/CatalogoDeNpcs.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/CatalogoDeDialogos.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/CatalogoDePuzzles.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/CatalogoDeLicoes.cs — novo (15/09/2026), confirmado por .cs real, ver §8.1
Assets/Scripts/Historia/GatilhoDeQuest.cs — ⚠️ citado no LEIA_ISTO, .cs NÃO anexado nesta sessão, ver §8.1

Assets/Scripts/Casa/InteragirComTerminalCasa.cs — novo (14/09/2026), ver §4.7
Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs — novo (14/09/2026), ver §4.7

Assets/Scripts/Mapas/ — pasta existe na raiz de Scripts/ (confirmada via Explorer, 14/09/2026), conteúdo ainda não visto nesta sessão — não documentar até confirmar

Assets/Scripts/Combat/CombatEngine.cs
Assets/Scripts/Combat/IParticipanteDeCombate.cs
Assets/Scripts/Combat/ResultadoAcao.cs
Assets/Scripts/Combat/InimigoFixo.cs
Assets/Scripts/Combat/CatalogoDeInimigos.cs — novo (15/09/2026), ver §4.2 (contém DefinicaoDeInimigo e DropDeInimigo)

Assets/Scripts/Droid/Droid.cs
Assets/Scripts/Droid/DroidPart.cs
Assets/Scripts/Droid/Braco.cs
Assets/Scripts/Droid/Perna.cs
Assets/Scripts/Droid/Tronco.cs
Assets/Scripts/Droid/Cabeca.cs
Assets/Scripts/Droid/DroidStats.cs
Assets/Scripts/Droid/TipoAtributo.cs
Assets/Scripts/Droid/PontosDeProgressao.cs
Assets/Scripts/Droid/ProgressaoDeNivel.cs
Assets/Scripts/Droid/SistemaDeProgressao.cs
Assets/Scripts/Droid/TabelaDeCustos.cs
Assets/Scripts/Droid/TabelaDeCombate.cs
Assets/Scripts/Droid/TecnicaComposta.cs
Assets/Scripts/Droid/EfeitosTemporariosUtil.cs
Assets/Scripts/Droid/CatalogoDeItens.cs — reescrito (15/09/2026), confirmado por .cs real: 6 categorias (+ Chave), Descricao/Raridade, VendavelNaLoja, spread de preço. Ver §8.1
Assets/Scripts/Droid/ItemConsumivel.cs — DEPRECATED (13/09/2026): substituído por CatalogoDeItens.cs/Inventario real. Mantido no projeto sem uso ativo; candidato a remoção, não apagar sem confirmar que nada mais referencia ItensDeBatalha.Disponiveis.

Assets/Scripts/Estado/GerenciadorDeEstado.cs
Assets/Scripts/Estado/ISistemaDeSalvamento.cs
Assets/Scripts/Estado/SalvamentoJson.cs
Assets/Scripts/Estado/DadosDoJogo.cs

Assets/Scripts/Scripting/DroidScriptRunner.cs
Assets/Scripts/Scripting/TerminalDroidApi.cs

Assets/Scripts/_Teste/InimigoTeste.cs
```

> **Nota:** os namespaces (`DroidsCode.DroidCore`, `DroidsCode.Combat`,
> `DroidsCode.Scripting` — seção 1) continuam com esses nomes mesmo a pasta
> física sendo `Droid/` em vez de `DroidCore/`; isso é comum em C# (pasta e
> namespace não precisam bater) e não é um erro a corrigir.

---

## 4. Mapa de classes

### 4.1 `DroidCore` — Droid e progressão

**`Droid`** (implementa `IParticipanteDeCombate`) — estado de um combatente configurável.
- `Nome`, `Hp`, `HpMax`, `Braco`/`Perna`/`Tronco`/`Cabeca`, `StatsBase` (`DroidStats`)
- `TecnicasConfiguradas`: **`Dictionary<string, TecnicaComposta>`** (chave = nome da técnica)
- `Pontos`: `PontosDeProgressao` (orçamento gasto no terminal)
- `Progressao`: `ProgressaoDeNivel` (nível/XP ganhos em combate — separado de `Pontos` por SRP)
- `Defesa => ObterTotal(TipoAtributo.Vit)` — VIT total, **já incluindo bônus de peça** (ver `ObterTotal` abaixo)
- `HpMaxBase` (persistido) + `HpMax` **calculado** a partir de `HpMaxBase` + %VIT + bônus de peça. `RecalcularHpAposMudancaDeVit(hpMaxAntes)` mantém o HP atual coerente quando o teto sobe (chamado por `TerminalDroidApi.AplicarUpgrade`)
- `BonusPercentualDeCura` — bônus de cura derivado de INT (`TabelaDeCombate.PercentualBonusCuraPorInt`), usado por `BattleManager.UsarItemDeCura` e `BagUIManager.UsarCuraForaDeBatalha`
- `RolarResistencia(chance)` (estático) — rolagem de resistência a efeito, usada por `BattleManager.AplicarEfeitoDeItem` para Debuff
- `EquiparPeca(TipoDePeca, DroidPart)` — instala a peça no slot, **substituindo** a anterior (confirmado em teste: não duplica bônus)
- `ObterBonusDePecas(TipoAtributo)` — soma os bônus das 4 peças equipadas
- `EfeitosAtivos`: `List<EfeitoDeAtributo>` — só o Droid do jogador tem, decrementado 1x por turno via `DecrementarEfeitosAtivos()`
- `ExecutarAcao(nomeAcao, alvo)`: lê a técnica configurada, calcula dano (ver fórmula na seção 5) — nunca chama Lua
- `ObterTotal(TipoAtributo)`: base (`StatsBase`) + bônus de peça
> ✅ **Corrigido nesta seção (15/09/2026):** a versão anterior desta linha dizia
> que o bônus de peça era "hoje sempre 0 — peças não têm bônus nomeado por
> atributo ainda". Isso **estava desatualizado** e contradizia a própria §4.3
> deste documento, além do `readme.md` e do `CHECKLIST`. Confirmado com os
> arquivos reais em mãos (`BagUIManager.cs` constrói
> `new Braco(nome, 0, Raridade.Comum, item.AtributoBonificado, item.ValorDoBonus)`
> e `TelaDeDroidManager.cs` lê `peca.AtributoBonificado`/`peca.ValorDoBonus`):
> **peças dão bônus real de atributo.** Esta era a divergência nº 2 do trio.

**`DroidPart`** (abstrata) + `Braco`/`Perna`/`Tronco`/`Cabeca` — só dado (`Nome`, `AtributoPrincipal`, `Raridade`, **`AtributoBonificado`** (`TipoAtributo?`, nulo = peça sem bônus) e **`ValorDoBonus`**), sem comportamento próprio. Construtor confirmado em uso: `new Braco(nome, valorPrincipal, Raridade, atributoBonificado, valorDoBonus)`. Enums auxiliares confirmados em uso: `TipoDePeca` (`Braco`/`Perna`/`Tronco`/`Cabeca`) e `Raridade` (`Comum`, ...). Isso é intencional na fase atual (Fase 1 do design); a herança de verdade com métodos sobrescritos é Fase 2 (roadmap).

**`PontosDeProgressao`** — orçamento gasto no terminal. `GanharPontosPorNivel(niveis)`, `TentarGastar(quantidade)`, `DefinirPontos(quantidade)` (uso exclusivo do sistema de salvamento, para restaurar valor exato — não usar em fluxo normal).

**`ProgressaoDeNivel`** — nível e XP ganhos em combate. `GanharExperiencia(xpGanho)` retorna quantos níveis subiram; `ExperienciaNecessariaProximoNivel = Nivel * 100` (placeholder). `Definir(nivel, experiencia)` é uso exclusivo do sistema de salvamento.

**`SistemaDeProgressao`** (estático) — serviço de domínio que liga as duas classes acima: recebe XP, decide se sobe nível, concede pontos se sim.

**`TipoAtributo`** — enum `{ For, Agi, Vit, Int, Dex, Luk }`. Não expandir (DEF/FLEE/ATK% etc. como valores derivados) sem decisão explícita — ver roadmap, seção 8.

**`TabelaDeCustos`** (placeholder, não balanceado) — `CustoUpgradeAtributo = 1`; `CustoDanoPorNivel = 1`; `CustoResistenciaPorNivel = 1` (**declarada mas não usada em nenhum cálculo hoje** — verificar se ainda é necessária); `CustoPorTurnoDeEfeito`: Stun = 3/turno, Envenenamento = 2 (não confirmado se escala por turno como Stun).

**`TabelaDeCombate`** (placeholder) — `DanoPorNivelDeTecnica = 5`. Separada de `TabelaDeCustos` de propósito: uma é custo em pontos, outra é output de dano.

**`CatalogoDeItens`** (Estágio 2 em 13/09/2026, expandido nas sessões seguintes — substitui `ItemConsumivel.cs`/`ItensDeBatalha`) — catálogo estático (`Dictionary<string, DefinicaoDeItem>`) dos itens que existem no jogo. `DefinicaoDeItem`: `Id` (string estável — **nunca renomear**), `Nome`, `Tipo` (`TipoDeItem`: `Cura`, `Buff`, `Debuff`, `Equipavel`, `ForaDeBatalha` — as 5 categorias já implementadas), `CuraHp`, `UsavelEmBatalha`/`UsavelForaDeBatalha`, e **`Preco`** (novo, sessão da Loja — todo item vale `PrecoPadrao` = 1 Gold, decisão deliberada não balanceada). **`ItensDaLoja`** (nova propriedade, sessão da Loja) expõe `Todos.Values` inteiro como itens vendáveis, sem filtro por tipo. Buff/Debuff reaproveitam `EfeitoDeAtributo`/`EfeitoDeDanoPorTurno` (mesmo motor de técnica); Equipável carrega `TipoDePeca` + o bônus de atributo da peça (ver `Droid.EquiparPeca`/`ObterBonusDePecas`, §4.3). O **catálogo** (o que existe) fica aqui; a **quantidade que o jogador possui** fica em `GerenciadorDeEstado.Inventario` (ver §4.5).
> ⚠️ `ItemConsumivel.cs`/`ItensDeBatalha` (lista fixa sem quantidade) ainda existem no projeto mas estão **deprecated**, sem nenhum código ativo os referenciando desde 13/09/2026 — ver §3.

**`TecnicaComposta`** — `Nome`, `NivelDeDano`, `EfeitosDeAtributo` (`List<EfeitoDeAtributo>`), `EfeitosDeDanoPorTurno` (`List<EfeitoDeDanoPorTurno>`), `CustoTotal()`. `EfeitoDeAtributo` liga-se a um `TipoAtributo` + valor + duração (`-1` = permanente, convenção não usada ainda). `EfeitoDeDanoPorTurno` é dano contínuo (ex: Envenenamento), modelado como tipo próprio por não caber no formato de "alterar atributo".
> ✅ **Implementado (12/09/2026)** — os efeitos de `TecnicaComposta` (Stun, Envenenamento) agora são aplicados de verdade: `Droid.ExecutarAcao` chama `AplicarEfeitosDaTecnica(tecnica, alvo)` (método privado estático), que copia cada `EfeitoDeAtributo`/`EfeitoDeDanoPorTurno` da técnica para `alvo.EfeitosAtivos`/`alvo.EfeitosDeDanoAtivos` (cópia, não referência — cada uso da técnica gera instâncias novas, com sua própria `DuracaoEmTurnos`). Quem realmente processa os efeitos durante o turno é o `CombatEngine` (ver 4.2).
> ⚠️ **Risco não resolvido:** nada limpa `EfeitosAtivos`/`EfeitosDeDanoAtivos` do `Droid` entre batalhas (`FinalizarBatalha`/`BattleManager.Start()` não zeram essas listas). Hoje isso não tem efeito visível porque só o `Droid` aplica efeitos — `InimigoFixo` nunca tem `TecnicaComposta` — mas o dia em que um inimigo puder envenenar/atordoar o jogador, efeitos podem vazar de uma batalha pra outra. Ver seção 9.
> ⚠️ **Sem dedupe/cap de empilhamento:** cada acerto de uma técnica com Veneno/Stun adiciona uma nova instância na lista do alvo. Envenenar o mesmo alvo várias vezes soma o dano por turno de todas as instâncias ativas; Stun repetido não estende a duração de fato (todas decrementam juntas a cada turno do alvo — o efeito prático nunca passa da maior duração individual). Não confirmado se é intencional.

### 4.2 `Combat` — combate

**`IParticipanteDeCombate`** — contrato: `Nome`, `Hp` (get/set), `HpMax` (get), `Defesa` (get), `ObterAcoesDisponiveis()`, `ExecutarAcao(nomeAcao, alvo)`, **`EfeitosAtivos`**, **`EfeitosDeDanoAtivos`** e **`ChanceDeResistirEfeito`**.
> ✅ **Corrigido (15/09/2026):** os três últimos membros não constavam nesta
> linha. Confirmado com `BattleManager.cs` em mãos, que faz
> `alvo.EfeitosAtivos.Add(...)`, `alvo.EfeitosDeDanoAtivos.Add(...)` e
> `Droid.RolarResistencia(alvo.ChanceDeResistirEfeito)` tratando `inimigo` e
> `droid` pela mesma interface — ou seja, `InimigoFixo` **também** carrega
> efeitos hoje. Consequência importante: o risco descrito em §4.1 ("o dia em
> que um inimigo puder envenenar/atordoar") deixou de ser hipotético para o
> lado do inimigo — um Debuff de item já aplica efeito nele.

**`CatalogoDeInimigos`** (novo, 15/09/2026) — catálogo estático `Dictionary<string, DefinicaoDeInimigo>`, mesmo padrão de `CatalogoDeItens` (Id string estável, `Obter(id)` devolvendo `null` em vez de lançar). Guarda `Nome`/`HpMax`/`Ataque`/`Defesa`/`RecompensaXp`/`GoldMinimo`/`GoldMaximo`/`Drops` de cada inimigo. Ids atuais: `sucata_captura`, `droid_selvagem`, `droid_do_ivo`, `esqueleto_teste`. `DropDeInimigo` é o espelho em C# puro de `BattleManager.DropDeItem` (que é serializável e depende de `UnityEngine`) — a conversão acontece em `BattleManager.AplicarInimigoDoCatalogo`. **Motivo de existir:** sem ele, cada inimigo novo exigia uma cena `Battle` duplicada e mais um lugar para esquecer de preencher `tabelaDeDrops` — pendência que o `CHECKLIST` já listava.

**`ResultadoAcao`** — `Sucesso`, `DanoCausado`, `Mensagem`. Mínima de propósito — não adicionar campos (efeito visual, combo, status) sem necessidade concreta.

**`CombatEngine`** — sem estado. `ExecutarTurno(atacante, alvo, nomeAcao)`:
1. Aplica dano por turno (`EfeitosDeDanoAtivos`, ex: Envenenamento) no **próprio `atacante`**, no início do turno dele — antes de decidir se ele age.
2. Se isso zerar o HP do atacante, retorna direto ("sucumbiu a um efeito ativo antes de agir"), sem executar a ação.
3. Se o atacante estiver atordoado (`EstaAtordoado` — checa `EfeitosAtivos` por `NomeExibicao == "Stun"` com duração > 0), perde o turno.
4. Caso contrário, delega para `atacante.ExecutarAcao`, aplica o dano resultante ao HP do alvo.
5. Ao final, chama `atacante.DecrementarEfeitosAtivos()` — **só do atacante desta chamada**, nunca do alvo.

`VerificarDerrota(participante)` = `Hp <= 0` (nenhuma outra condição implementada).
> ⚠️ **Bug conhecido:** `BattleManager` só chama `VerificarDerrota` no participante "óbvio" de cada método (`VerificarDerrota(inimigo)` depois do ataque do jogador; `VerificarDerrota(droid)` depois do turno do inimigo). Se o dano por turno do passo 1 acima matar o atacante *antes* de agir, esse participante nunca é checado por `VerificarDerrota` naquele exato método — a batalha só percebe a derrota/vitória um turno depois. Ver seção 9.

**`InimigoFixo`** (implementa `IParticipanteDeCombate`) — comportamento 100% fixo em C#, uma única ação ("Atacar"). Tem `RecompensaXp`, propositalmente fora da interface genérica (só o `BattleManager` referencia `InimigoFixo` diretamente).

### 4.3 `Scripting` — terminal

**`DroidScriptRunner`** — cria a VM Lua (`Script(CoreModules.Preset_SoftSandbox)`), registra `TerminalDroidApi` como único tipo exposto (`droid` global). `ExecutarCodigo(codigoLua)` roda o script e captura exceções de sintaxe/execução.
> Hoje só existe **um modo** de execução (equivalente ao "Modo Configuração" do design original). O **Modo Puzzle** (exercícios isolados, nunca tocando o Droid real) está no roadmap — ver seção 8.

**`TerminalDroidApi`** — única fachada exposta ao Lua; `Droid` nunca é registrado diretamente no MoonSharp.
- `SubirAtributo(nomeAtributo, quantidade)`: valida quantidade > 0, nome de atributo válido, pontos suficientes; aplica e loga.
- `AprenderTecnica(nome, nivelDeDano)`: valida nome/nível, checa `TecnicaComposta.CustoTotal()` contra pontos; aplica e loga.
- `AprenderTecnicaComVeneno(nome, nivelDeDano, danoPorTurno, duracaoEmTurnos)`: mesma validação de `AprenderTecnica`, mais `danoPorTurno > 0` e `duracaoEmTurnos > 0`; monta a técnica com um `EfeitoDeDanoPorTurno` ("Envenenamento") e delega para `TentarAprender`.
- `AprenderTecnicaComStun(nome, nivelDeDano, duracaoEmTurnos)`: mesma ideia, monta a técnica com um `EfeitoDeAtributo` ("Stun", `Atributo`/`Valor` no default de propósito — é uma flag checada por nome em `CombatEngine.EstaAtordoado`, não um buff real) e delega para `TentarAprender`.
- `ObterPontosDisponiveis()`, `ObterLogDaSessao()`, `LimparLog()`.
- `TesteAdicionarPontos(quantidade)`: **[TESTE]** soma pontos direto via `Pontos.DefinirPontos(...)`, ignorando XP/nível. Existe pra testar builds de técnica/atributo sem farmar batalha. Comentário no próprio código já sinaliza "considere remover ou esconder antes de qualquer build final/demo pra terceiros" — ainda não removido/protegido.
> ✅ **Corrigido (12/09/2026)** — proteção do Ataque Básico replicada nas 3
> variantes de aprender técnica (`AprenderTecnica`, `AprenderTecnicaComVeneno`,
> `AprenderTecnicaComStun`): todas recusam `nome == Droid.NomeAtaqueBasico`,
> igual `EsquecerTecnica` já fazia. Decisão registrada: não foi criado um
> campo `EhProtegida` separado em `TecnicaComposta` (ficaria pra quando
> houver mais de uma técnica "protegida" — não é o caso hoje).
>
> ✅ **Corrigido (12/09/2026)** — reaprender uma técnica existente agora é
> **bloqueado** em `TentarAprender` (as 3 variantes de aprender compartilham
> esse método): se `nome` já está em `TecnicasConfiguradas`, a chamada falha
> e o log de sessão já indica o comando correto (`droid.melhorarTecnica(...)`
> ou `droid.esquecerTecnica(...)` antes de reaprender do zero) — funciona como
> dica de uso dentro do próprio terminal, sem precisar de documentação externa.
> Foi adicionado `MelhorarTecnica(nome, nivelDeDanoAdicional)`: soma ao
> `NivelDeDano` já configurado e cobra **só a diferença** entre
> `CustoTotal()` novo e antigo (nunca o custo cheio de novo). Se os pontos
> disponíveis não cobrirem a diferença, o aumento de nível é revertido antes
> de retornar falha — a técnica nunca fica num estado "melhorada mas não
> paga". `MelhorarTecnica` só altera `NivelDeDano` (dano direto); não mexe
> em `EfeitosDeAtributo`/`EfeitosDeDanoPorTurno` da técnica — evoluir um
> efeito (ex: aumentar duração do Veneno) não foi pedido e fica fora deste
> escopo. Texto de ajuda do terminal (`TerminalUIManager.TextoAjudaPadrao`)
> atualizado com o novo comando.
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

**`GerenciadorDeEstado`** (Singleton lazy, `DontDestroyOnLoad`) — guarda `DroidDoJogador` (mesma instância entre cenas), posição salva para transição Game↔Battle, flags de história, um contador de menus abertos (`MenuAberto`, incrementado/decrementado por `RegistrarMenuAberto`/`RegistrarMenuFechado`) usado para congelar o movimento do player enquanto um menu/terminal está aberto, o **`Inventario`** (`Dictionary<string, int>`, Id de `CatalogoDeItens` → quantidade: `AdicionarItem`, `TentarRemoverItem`, `ObterQuantidadeDeItem`, `CarregarInventario`) e o **`Gold`** (`AdicionarGold`/`TentarGastarGold`/`CarregarGold`, persistido em save v4). Droid novo começa com kit inicial fixo (3 Poção Pequena + 1 Poção Média) e Gold = 0 — fontes reais de Gold em jogo normal: drop configurável por `BattleManager` (ver §8) e venda de item na Loja.

> ✅ **Novo (15/09/2026 — Ato 1):** `GerenciadorDeEstado` ganhou dois campos
> de **trânsito** mundo → cena `Battle`, deliberadamente **não persistidos**
> (mesma natureza de `PosicaoSalva`/`CenaDeOrigemDaPosicao`): `ProximoInimigoId`
> (Id no `CatalogoDeInimigos`; vazio = `BattleManager` usa o Inspector da cena,
> comportamento antigo intacto) e `FlagDeVitoriaPendente` (flag de história
> gravada se o jogador vencer). `LimparBatalhaPendente()` zera os dois e é
> chamado por `BattleManager.FinalizarBatalha`, para que um encontro aleatório
> seguinte não herde o inimigo/flag de uma batalha roteirizada.

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

**`BattleManager`** — não calcula nada, só orquestra: botões fixos (Atacar/Item/Fugir/Status), lista dinâmica de técnicas se o Droid tiver mais de uma configurada, corrotinas de turno, integração com `TelaDeStatusManager`.
- ✅ **Corrigido/substituído (Estágio 2, 13/09/2026)** — a descrição antiga deste item (botão "Item" abrindo `ItensDeBatalha.Disponiveis`, lista fixa) está **obsoleta**. `AbrirListaDeItens` agora lê `GerenciadorDeEstado.Inventario` de verdade via `CatalogoDeItens.Obter`, filtra só itens com `UsavelEmBatalha` e estoque > 0, e mostra quantidade real no botão (`"Poção Pequena (+20 HP) x3"`). `UsarItem` decrementa o estoque (`TentarRemoverItem`) e **salva o jogo na hora** (`new SalvamentoJson().Salvar()`) — decisão: o efeito de cura já foi aplicado ao Droid, então o consumo do item precisa ficar gravado imediatamente, mesmo que o jogo feche antes do próximo save manual.
- **Log de batalha:** `MostrarMensagem` mantém as últimas 4 linhas (`_logDeBatalha`) em vez de sobrescrever a mensagem toda vez — mesmo padrão do log do Terminal (`TerminalUIManager.linhasDeLog`). Cada chamada separa a mensagem recebida por `\n` em várias entradas do log, porque `CombatEngine.ExecutarTurno` pode devolver mais de um evento no mesmo turno (ex: dano de veneno + resultado do ataque).
- ✅ **Corrigido (13/09/2026)** — `painelListaDeAtaques`/`painelListaDeItens` ganharam um botão "Voltar" (reaproveita `prefabBotaoAtaque`/`prefabBotaoItem`, só chama `SetActive(false)`, nunca executa ação) e um `Update()` que fecha o painel aberto com ESC. Corrigido no mesmo commit: `AbrirListaDeAtaques`/`AbrirListaDeItens` agora fecham o outro painel antes de abrir o seu — antes disso, abrir Ataque e depois Item (sem fechar o primeiro) deixava os dois painéis ativos ao mesmo tempo.

> ✅ **Novo (15/09/2026 — Ato 1):** `BattleManager` ganhou
> `AplicarInimigoDoCatalogo()` (chamado no início de `Start()`, monta o
> inimigo a partir de `GerenciadorDeEstado.ProximoInimigoId` → `idDoInimigoPadrao`
> → Inspector, nessa ordem de fallback) e `GravarFlagDeVitoria()` (chamado nos
> **dois** pontos onde a derrota do inimigo é detectada, junto de
> `AplicarRecompensas`). `FinalizarBatalha` chama `LimparBatalhaPendente()`.
> ⚠️ **Bug encontrado e corrigido na mesma sessão:** o `Update()` do
> `BattleManager` dizia, em comentário, "assume que esta cena não convive com
> o Menu do Mundo — confirmar se algum dia as duas rodarem sobrepostas". Desde
> que `MenuMundoManager` ganhou `DontDestroyOnLoad`, **elas convivem**: o
> Canvas do menu viaja para dentro da cena `Battle`, e um único ESC era
> consumido pelos dois scripts (fechava a lista de ataques *e* abria o menu de
> pausa por cima da batalha). Corrigido dos dois lados — guarda
> `if (GerenciadorDeEstado.Instancia.MenuAberto) return;` no `BattleManager`,
> e o campo `cenasBloqueadas` no `MenuMundoManager` (abaixo).

**`MenuMundoManager`** — menu de pausa (tecla Esc): Status/Terminal/Bag/Droid/Opções/Salvar/Carregar/Voltar. Opções ainda é placeholder ("ainda não foi implementado").
- ✅ **Novo (15/09/2026):** campo `cenasBloqueadas` (default `Battle`, `MainMenu`) — o menu ignora a tecla nessas cenas. A checagem roda **antes** do bloco de `_outroPainelAberto`, de propósito: se um painel já estiver aberto, ESC ainda consegue fechá-lo.
- ✅ **Correção já presente no código (não documentada antes):** ESC deixou de ser ignorado enquanto Status/Terminal/Bag/Droid estão abertos — o campo `_fecharPainelAtual` guarda qual painel fechar, e `Update()` chama ele. A descrição anterior desta seção dizia que ESC era ignorado nesse período.
- ✅ **Corrigido (13/09/2026)** — exclusividade de painéis: ao abrir Status ou Terminal, `painelMenu` é escondido (`SetActive(false)`) via `AoClicarStatus`/`AoClicarTerminal`, e reexibido pelo callback `AoFecharOutroPainel` quando o Status/Terminal fecha (ver `TelaDeStatusManager`/`TerminalUIManager` abaixo). O contador de `GerenciadorDeEstado` (`RegistrarMenuAberto`/`Fechado`) não foi tocado por essa mudança. Nova flag `_outroPainelAberto` faz `Update()` ignorar ESC enquanto Status/Terminal estiverem abertos — sem ela, ESC com o Terminal aberto reabria `painelMenu` por baixo dele e incrementava o contador de novo (bug encontrado e corrigido no mesmo commit).
- ✅ **Corrigido (Estágio 2, 13/09/2026)** — Bag deixou de ser placeholder: `AoClicarBag` abre `BagUIManager` real, mesmo padrão de exclusividade de `AoClicarStatus`.
- ✅ **Corrigido (sessão do Painel do Droid)** — botão "Droid" deixou de chamar `MostrarAviso("Equipamentos")`; agora `AoClicarDroid` abre `TelaDeDroidManager` (novo campo `telaDeDroid`), mesmo padrão de exclusividade dos demais painéis. `AoFechar` do novo painel ligado em `Start()` junto dos outros.

**`TelaDeDroidManager`** (novo, sessão do Painel do Droid) — painel somente-leitura das 4 peças do Droid (`Braco`/`Perna`/`Tronco`/`Cabeca`), busca o Droid direto do `GerenciadorDeEstado` (mesmo padrão de `TelaDeStatusManager`). Mostra nome + `AtributoBonificado`/`ValorDoBonus` de cada peça, ou "Vazio" se o slot não tiver peça (nunca esconde o slot). Segue o mesmo contrato de painel + callback `AoFechar` de `TelaDeStatusManager`/`TerminalUIManager`/`BagUIManager`. Escopo desta leva é só visualização — equipar continua exclusivamente pela Bag (`BagUIManager.EquiparItem`); não há "desequipar".

**`TransicaoDeCena`** (genérico, usado em `City` e `CasaLip`) — leva o jogador entre cenas nos dois sentidos.
> ✅ **Pergunta de §4.7/§9 respondida (15/09/2026):** `trancada` (bool),
> `textoDeBloqueio` (`TMP_Text`) e `mensagemDeBloqueio` (string) são campos
> **genéricos da própria classe `TransicaoDeCena`**, não algo específico da
> porta da Casa — confirmado com o arquivo real em mãos. Qualquer porta do
> jogo pode usá-los. `OnTriggerEnter2D` continua ativo quando trancada (só
> mostra o aviso e retorna), e `OnTriggerExit2D` esconde o aviso. O novo
> `TrancaPorHistoria` (§4.8) é quem controla esse campo por condição de jogo. **Corrigido nesta sessão:** o campo de destino era `Transform`; trocado pra `Vector2` (coordenadas digitadas no Inspector), porque Unity não permite salvar referência de `Transform` entre cenas diferentes (cross-scene reference).

**`NpcInterativo`** (cena `City`) — enum `TipoDeNpc` (`Generico`/`Loja`) + campo `lojaUIManager`. `Interagir()` despacha por tipo: `Generico` mantém placeholder (`Debug.Log`, diálogo simples foi pulado por decisão do responsável do projeto, não descartado); `Loja` chama `RegistrarMenuAberto`/(`AoFechar` → `RegistrarMenuFechado`) e abre `LojaUIManager`, mesmo padrão de exclusividade de `TelaDeStatusManager`/`TerminalUIManager`.

**`LojaUIManager`** (novo, cena `City`) — painel com abas Comprar/Vender, mesmo contrato estrutural de `BagUIManager` (`AoFechar`, `LayoutRebuilder.ForceRebuildLayoutImmediate` ativando o painel antes de popular a lista — mesma correção de bug já usada na Bag). Comprar lê `CatalogoDeItens.ItensDaLoja`, gasta Gold via `TentarGastarGold` e soma item via `AdicionarItem`; Vender lê o inventário do jogador, remove item via `TentarRemoverItem` e credita Gold via `AdicionarGold`.

**`CameraFollow`** (novo, 14/09/2026, pasta `Camera/`) — segue suavemente um `target` (Transform do Lip) via `Vector3.Lerp` em `LateUpdate`, com `offset` configurável (Z negativo, exigido pra câmera 2D enxergar sprites) e `smoothSpeed`. Opcionalmente (`usarLimites`) trava a câmera dentro de um retângulo (`limiteMin`/`limiteMax`, coordenadas de mundo) calculado a partir de `Camera.orthographicSize`/`aspect`, pra nunca mostrar área vazia fora do mapa; se o mapa for menor que a tela num eixo, trava no centro daquele eixo em vez de inverter os limites. Puro apresentação — não referencia `GerenciadorDeEstado`/Domínio, então não tem acoplamento com save/estado do jogo. Não documentado ainda em qual(is) cena(s) está de fato anexado (a doc no próprio arquivo sugere `City`, mas não está confirmado se também está em `Game`/`CasaLip`/`Battle`).

**`TerminalUIManager`** — UI do terminal: campo de código, histórico de comandos (↑/↓), log de sessão, atalho Ctrl+Enter.
> ✅ **Novo (14/09/2026 — puzzle da casa):** ganhou `public DroidScriptRunner Runner => runner` (só leitura — exposto pra permitir que um checker de puzzle específico de cena, como `PuzzleAguaCasaChecker`, leia o estado da VM Lua depois de cada execução, sem duplicar toda a UI do terminal) e `public System.Action AoExecutarCodigo`, invocado ao final de `AoClicarExecutar()` — sempre, sucesso ou erro. O terminal geral não sabe (nem precisa saber) o que os ouvintes fazem com isso.
> ✅ **Confirmado (14/09/2026):** puzzle da água (`CasaLip`) testado ponta a ponta pelo responsável do projeto — funciona.
> ✅ **Débito quitado (15/09/2026):** o `Debug.Log` de depuração de
> `DroidScriptRunner.ObterVariavelNumerica` foi **removido**, junto do
> `using System.Linq` e do `using UnityEngine` que só existiam por causa
> dele. Quem precisar depurar puzzle agora usa o campo `logDeDepuracao` do
> `VerificadorDePuzzle` (§4.8), que é ligável por instância no Inspector em
> vez de global e sempre ligado. Item 7 dos "Próximos passos imediatos" do
> `CHECKLIST` — concluído.

### 4.7 `Casa` — cena `CasaLip` (puzzle de introdução, Sessão 1 do Enredo)

> Cena nova (`CasaLip`, confirmada via print do Windows Explorer em
> `Assets/Scenes`, 14/09/2026, junto com `Battle`/`City`/`Game`/`MainMenu`),
> não descrita em nenhuma versão anterior deste documento. Cobre o puzzle
> de água citado no Enredo ("Sessão 1":
> `int nivel_agua = 0; while (nivel_agua < 100) { ... }`) como primeiro
> contato do jogador com o terminal Lua, antes mesmo da cena `Game`.

**`InteragirComTerminalCasa`** — trigger de interação simples (`Collider2D` como trigger, via `[RequireComponent]` + `Reset()` que já marca `isTrigger = true`). Jogador entra na área (`OnTriggerEnter2D`, checa `tagDoPlayer` = "Player"), aperta uma tecla (`teclaDeInteracao`, default `E`), e chama `terminal.Abrir()` — o mesmo `TerminalUIManager` geral do jogo, reaproveitado aqui, não uma classe própria de terminal da casa. **Não depende de `NpcInterativo`** — decisão deliberada de não travar a entrega numa dependência de padrão de interação que não estava confirmada em mãos na sessão; pode ser substituído pelo padrão de `NpcInterativo` da Cidade depois, sem tocar em `PuzzleAguaCasaChecker` (a única exigência entre os dois é que `Abrir()` seja chamado em algum momento).

**`PuzzleAguaCasaChecker`** — checagem de vitória do puzzle de água. `[RequireComponent(typeof(TerminalUIManager))]`: mora no **mesmo GameObject** que o `TerminalUIManager` da casa (não existe uma classe `TerminalCasaManager` separada — é o terminal geral reaproveitado, confirmado). Assina `terminal.AoExecutarCodigo` em `OnEnable`/desassina em `OnDisable` (padrão seguro de evento em MonoBehaviour, evita assinatura duplicada ou vazamento). A cada execução de código, lê `terminal.Runner.ObterVariavelNumerica(nomeDaVariavel)` (default `"nivel_agua"`) — se `null` (variável ainda não existe na sessão Lua, ex: jogador digitou outro nome), loga e não faz nada; se `< valorDeVitoria` (default 100), não faz nada; se atingir o valor, marca `_puzzleResolvido` (idempotente — não reexecuta), ativa uma fala do Apollo (`textoDeApollo`, TMP) e destranca a porta de saída via `transicaoDaPortaDeSaida.trancada = false`.
> ⚠️ **Decisão de arquitetura registrada (14/09/2026) — como a porta é trancada:** a versão anterior desativava o componente `TransicaoDeCena` da porta inteiro (`.enabled = false`) no `Awake()`. Dois problemas, ambos corrigidos: (1) componente desabilitado não recebe `OnTriggerEnter2D`, então a porta não conseguia nem avisar o jogador que precisava resolver o terminal primeiro; (2) `PuzzleAguaCasaChecker` mora no painel do terminal, que nasce **desativado** por padrão (só ativa em `Abrir()`) — `Awake()` de objeto inativo não roda, então a porta ficava destrancada até o jogador abrir o terminal pela 1ª vez. **Correção:** a porta usa um campo público `trancada` (`bool`) em `TransicaoDeCena`, marcado no Inspector da própria porta (ativa desde o início da cena); o trigger continua funcionando pra mostrar o aviso de bloqueio, e este script só desmarca a flag quando o puzzle é resolvido — nunca desabilita o componente.
> **Nota:** este é o primeiro lugar no projeto onde `TransicaoDeCena` tem um campo `trancada`/bloqueio condicional — a §4.6 (bloco `TransicaoDeCena`, cena `City`) ainda descreve só a correção de `Transform`→`Vector2`; **verificar se esse campo `trancada`/`textoDeBloqueio` é genérico em `TransicaoDeCena` (reaproveitável em outras portas) ou específico desta instância da porta da Casa** antes de generalizar essa descrição na §4.6.
- ✅ **Corrigido (13/09/2026)** — ganhou o campo público `AoFechar` (`System.Action`, opcional), invocado dentro do `if` de `Fechar()` (só quando o painel de fato estava aberto). Usado por `MenuMundoManager` pra saber quando reexibir seu próprio painel.

**`TelaDeStatusManager`** — painel de status, busca o Droid direto do `GerenciadorDeEstado` (reutilizável entre batalha e mundo).
- ✅ **Corrigido (13/09/2026)** — mesmo padrão do Terminal: ganhou `AoFechar` (`System.Action`, opcional), invocado ao final de `Fechar()`. Como o uso dentro da Batalha não atribui esse callback, fica `null` ali e não é chamado (`?.Invoke()`) — nenhuma mudança de comportamento na Batalha.

**`BagUIManager`** (novo, Estágio 2, 13/09/2026) — painel da Bag, acessível pelo `MenuMundoManager` (substitui o placeholder "Bag ainda não foi implementado"). Cura fora de batalha, sem gastar turno (não existe turno fora de combate), filtra itens do `Inventario` por `UsavelForaDeBatalha`, decrementa estoque e salva na hora — mesmo padrão de `UsarItem` do `BattleManager`. Segue o mesmo contrato de painel + callback `AoFechar` de `TelaDeStatusManager`/`TerminalUIManager` (usado por `MenuMundoManager` pra reexibir seu próprio painel).
> ⚠️ **Bug corrigido (13/09/2026):** `Mostrar()` ativa o painel (`SetActive(true)`) **antes** de popular a lista de botões e força `LayoutRebuilder.ForceRebuildLayoutImmediate` — a ordem antiga (popular primeiro, ativar depois) deixava o painel "ligado" mas com `ContentSizeFitter` travado em 0x0, porque o Unity só recalcula layout em objetos ativos.

**`MainMenuManager`** — navegação do menu principal (Novo Jogo/Configurações/Sair). Sem mudanças estruturais previstas.

---

### 4.8 `Historia` — diálogo, flags e progressão do Ato 1 (15/09/2026)

> Pasta nova (`Assets/Scripts/Historia/`). Sete arquivos que, juntos, tornam
> jogável o Ato 1 do `TCC_-_ENREDO.md` (Sessões 2 a 7). Nenhum sistema novo de
> domínio foi criado: tudo se apoia em estruturas que já existiam e estavam
> ociosas — `GerenciadorDeEstado` flags de história (existiam, persistiam, e
> **nada as usava**), `TransicaoDeCena.trancada`, o evento
> `TerminalUIManager.AoExecutarCodigo` e `DroidScriptRunner.ObterVariavelNumerica`.

**`DialogoUIManager`** — painel único de diálogo. Mesmo contrato dos painéis
existentes (`painel` raiz desativado em `Start()`, `AoFechar`,
`RegistrarMenuAberto`/`RegistrarMenuFechado`, `ForceRebuildLayoutImmediate`
com o container **ativo antes** de popular). `Mostrar(falas, escolhas, aoConcluir)`
exibe as falas em sequência e, no fim, botões de escolha opcionais.
**Diferença deliberada em relação aos outros painéis:** o acesso é por
`DialogoUIManager.Instancia` (estático, resolvido em runtime), não por
referência de Inspector — gatilhos ficam espalhados por várias cenas e o Unity
não permite referência cross-scene (mesmo motivo que fez `TransicaoDeCena`
trocar `Transform` por `Vector2`). Deve morar dentro do Canvas persistente do
`MenuMundoManager`; não chama `DontDestroyOnLoad` próprio, para não brigar com
a proteção de duplicata do `MenuMundoManager.Awake`.

**`FalaDeDialogo`** / **`EscolhaDeDialogo`** (`[Serializable]`, no mesmo
arquivo) — DTOs de Inspector. `EscolhaDeDialogo` carrega a consequência de
forma declarativa (`flagAoEscolher`, `idItemConcedido`, `quantidadeDoItem`,
`respostaAposEscolher`), e é por isso que a escolha de facção da Sessão 4 não
precisa de script próprio.

**`GatilhoDeDialogo`** — dispara um diálogo por proximidade+tecla
(`AoInteragir`), ao encostar (`AoEntrarNoTrigger`) ou só por chamada externa
(`ApenasPorScript`). Carrega as falas, `flagsNecessarias`/`flagsQueImpedem`,
`flagAoTerminar`, e o callback `AoConcluirDialogoExterno` (atribuído por código,
não pelo Inspector). É o componente que contém **todo o conteúdo narrativo** do
Ato 1 — o texto está no Inspector, nunca em `.cs`.

**`CondicoesDeHistoria`** (estática, no mesmo arquivo) — regra única
compartilhada: "todas as `flagsNecessarias` ativas E nenhuma das
`flagsQueImpedem`". Usada por `GatilhoDeDialogo`, `CondicaoDeHistoria`,
`TrancaPorHistoria`, `GatilhoDeBatalha` e `EncounterZone`.

**`CondicaoDeHistoria`** — liga/desliga GameObjects conforme flags, por regras.
**Armadilha registrada:** o componente **não** pode ter como alvo o próprio
GameObject — objeto desativado não roda `Update()`, então nunca se religaria
quando a flag mudasse. O código detecta e recusa esse caso com um aviso, e a
lista de alvos é externa de propósito.

**`GatilhoDeBatalha`** — batalha roteirizada (não aleatória). Irmão de
`EncounterZone` sem o sorteio: salva a posição (mesmo mecanismo de
`SalvarPosicao` + `RestaurarPosicao`), escreve `ProximoInimigoId` e
`FlagDeVitoriaPendente` no `GerenciadorDeEstado` e carrega a cena de batalha.
A própria `flagDeVitoria` serve de trava contra repetição — sem isso o jogador
voltaria da luta, encostaria no mesmo trigger e lutaria em loop.

**`VerificadorDePuzzle`** — generalização do `PuzzleAguaCasaChecker`. Lê uma
variável global Lua após cada execução (`terminal.Runner.ObterVariavelNumerica`),
compara (`MaiorOuIgual`/`IgualA`/`Diferente`), e ao resolver: destranca porta
(`trancada = false`, **nunca** desabilita o componente — ver §4.7), mostra fala,
liga/desliga objetos e grava `flagAoResolver`. `flagQueJaResolve` faz o puzzle
nascer resolvido depois de um load.
> **Mudança estrutural em relação ao checker original:** não usa
> `[RequireComponent(typeof(TerminalUIManager))]` e **não mora no GameObject do
> terminal**. Com o `MenuMundoManager` persistindo entre cenas, o
> `TerminalUIManager` persiste junto — um checker preso a ele viajaria para
> todas as cenas seguintes continuando a escutar execuções que não têm nada a
> ver com o puzzle dele. Aqui o checker mora na própria cena e assina/desassina
> em `OnEnable`/`OnDisable`. O `PuzzleAguaCasaChecker` original **não foi
> apagado** — os dois coexistem até a `CasaLip` ser migrada no Editor.

**`TrancaPorHistoria`** (`[RequireComponent(typeof(TransicaoDeCena))]`) —
controla `trancada` a partir de duas classes de exigência combináveis: flags de
história, e **ter técnica configurada no terminal** (`exigirTecnicaConfigurada`
+ `minimoDeAcoes`, contando por `Droid.ObterAcoesDisponiveis()` para não
depender de o Ataque Básico morar ou não em `TecnicasConfiguradas`). A segunda
é a mais importante do projeto: é o que torna o terminal caminho crítico em vez
de painel opcional. Reavalia em `Update()` porque o jogador pode configurar uma
técnica sem trocar de cena. Troca `mensagemDeBloqueio` conforme o que falta.

**`ApolloDica`** (estática) — traduz a mensagem crua do MoonSharp numa
explicação conceitual na voz do Apollo. `Traduzir(mensagem)` varre uma lista
ordenada de pares (trecho em minúsculo → fala) e sempre devolve algo, mesmo
para erro desconhecido. `ExtrairLinha(mensagem)` tira o "(linha N)" do formato
`chunk_0:(3,12)`. Ligada em `TerminalUIManager.AoClicarExecutar`, no ramo de
erro, atrás do campo `mostrarDicasDoApollo` (default ligado).
> ⚠️ **Escopo honesto:** isto **não** é o "Apollo Debugger" do `readme.md`. Não
> destaca linha dentro do editor, não é reativo enquanto o jogador digita e não
> analisa o código — só o texto do erro já ocorrido. É o primeiro degrau. A
> lacuna continua registrada no `CHECKLIST` → Amadurecimento, agora reduzida.

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

**Defesa** = `ObterTotal(TipoAtributo.Vit)` — **com** bônus de peça (ver §4.1).

**HP máximo** = `HpMaxBase` + percentual de VIT + bônus de peça, calculado em
`Droid.HpMax` (não é campo). O save persiste `hpMaxBase`, nunca o total — ver
`DadosDoJogo.DroidSalvo.hpMaxBase` e a nota de compatibilidade em
`SalvamentoJson.Carregar`.

**Bônus de cura por INT** = `Droid.BonusPercentualDeCura` (%), aplicado sobre
`item.CuraHp` em `BattleManager.UsarItemDeCura` e `BagUIManager.UsarCuraForaDeBatalha`.
Constante em `TabelaDeCombate.PercentualBonusCuraPorInt`.

**Resistência a efeito** = `Droid.RolarResistencia(alvo.ChanceDeResistirEfeito)`,
rolada apenas para Debuff (Buff no próprio Droid não rola).

> ⚠️ **Não confirmado nesta sessão:** a fórmula de dano acima, o roll de
> acerto (HIT×FLEE) e o crítico por LUK **não puderam ser verificados** — os
> arquivos `Droid.cs`, `CombatEngine.cs`, `TabelaDeCombate.cs` e
> `InimigoFixo.cs` não estavam entre os enviados. O `readme.md` lista
> HIT/FLEE/crítico/INT como implementados; a §8 deste documento ainda lista
> HIT/FLEE como "planejado". A parte de **INT/resistência está confirmada**
> (visível em `BattleManager.cs`); HIT/FLEE e crítico **continuam em aberto**
> até alguém colar `Droid.cs`/`CombatEngine.cs`. Esta linha existe em
> obediência à §0 item 4: não declarar consistência sem o arquivo em mãos.

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
| Modo Puzzle do terminal | Exercícios de lógica isolados (`DefinicaoDePuzzle`/`ResultadoDePuzzle`), nunca tocando o Droid real | **Reavaliado (15/09/2026): provavelmente desnecessário.** O `VerificadorDePuzzle` (§4.8) já é genérico e cobre todos os puzzles previstos do Ato 1 com configuração de Inspector, zero código por puzzle. Manter fora de escopo até aparecer um puzzle que ele comprovadamente não resolva |
| Sistema de história (diálogo, flags, portões, batalha roteirizada) | Falas por Inspector, condição por flag persistida, portão de cena e batalha com inimigo de catálogo | **Concluído em código (15/09/2026)** — ver §4.8. Falta só o trabalho de Editor (montagem de cena), ver `GUIA_DE_MONTAGEM_ATO1.md` |
| Apollo Debugger | Destacar a linha da falha no editor + dica conceitual reativa a erro | **Parcial (15/09/2026):** `ApolloDica` entrega a dica conceitual por erro (§4.8). Falta o destaque de linha dentro do campo de código e a reatividade em tempo real |
| `DroidDataSO` / `ItemDataSO` / Factory | Migrar criação de Droid/itens de código direto para ScriptableObjects configuráveis no Inspector | Planejado, não implementado |
| Fase 2 de peças (`DroidPart`) | Herança real via Cartuchos de Código sobrescrevendo `DroidBase` | Planejado (design), não implementado |
| Inventário e equipamento completos | Todas as 6 categorias (`Cura`/`Buff`/`Debuff`/`Equipavel`/`ForaDeBatalha`/`Chave`) implementadas, com quantidade real e persistência (ver `CatalogoDeItens` — reescrito e confirmado 15/09/2026, ver §8.1 —, `Inventario`, `BagUIManager`). Obtenção via kit inicial, drop configurável por `BattleManager` e Loja (compra/venda). Descrição longa + Raridade + spread de preço confirmados; UI ainda mínima (sem ícone/filtro por raridade). | **Concluído** — falta só UI mais rica (não bloqueante), ver "Amadurecimento" em `CHECKLIST_DE_DESENVOLVIMENTO.md` |
| Sistema de missões (catálogo + motor + diário) | `CatalogoDeQuests` (10 quests), `GerenciadorDeQuests` (motor estático), `QuestUIManager` (diário) | **Concluído em código (15/09/2026)**, confirmado por `.cs` real — ver §8.1. Falta `GatilhoDeQuest.cs` (componente de NPC para oferecer/entregar quest — citado no `LEIA_ISTO`, `.cs` não anexado) e montagem de Editor (Prefab do diário, ligação nos NPCs) |
| Ajuda pedagógica ativa (`droid.explicar`) | `CatalogoDeLicoes` (9 conceitos) + ponta no `TerminalDroidApi` (`Explicar`/`Ajuda`/`Resumo`) | `CatalogoDeLicoes.cs` **confirmado por `.cs` real** (15/09/2026, ver §8.1). A ponta em `TerminalDroidApi` **não confirmada** — o arquivo `TerminalDroidApi.cs` em si nunca foi anexado nesta sessão, só é referenciado por outros arquivos |

---

## 8.1 Pacote "Ato 1 — catálogos de conteúdo" (✅ confirmado em 15/09/2026, sessão de reconciliação)

> **Atualização:** dos 10 arquivos descritos em `LEIA_ISTO_OS_10_ARQUIVOS_
> NOVOS.md`, **9 tiveram o `.cs` real anexado e lido nesta sessão**:
> `CatalogoDeItens.cs` (reescrito), `CatalogoDeInimigos.cs`,
> `CatalogoDeQuests.cs`, `CatalogoDeNpcs.cs`, `CatalogoDeDialogos.cs`,
> `CatalogoDePuzzles.cs`, `CatalogoDeLicoes.cs`, `GerenciadorDeQuests.cs`,
> `QuestUIManager.cs`, e a versão nova de `VerificadorDePuzzle.cs`. Confirmado
> por leitura direta do arquivo (não por inferência do `LEIA_ISTO`):
>
> - `CatalogoDeItens.cs`: 6 categorias (`Cura`/`Buff`/`Debuff`/`Equipavel`/
>   `ForaDeBatalha`/`Chave`), `Descricao` + `Raridade` em todo item,
>   `VendavelNaLoja` + `PrecoDeVenda`/`PrecoDeVendaReal` (spread 50%,
>   mínimo 1), preços com escala real (8–150), os 2 itens de recompensa de
>   facção (`Núcleo de Sobrecarga`, `Módulo de Ressonância`, ambos
>   `VendavelNaLoja = false`), 6 itens de quest (`Chave`, todos preço 0 e
>   não usáveis). `ItensDaLoja` filtra por `VendavelNaLoja && Preco > 0`.
> - `CatalogoDeQuests.cs`: 10 quests confirmadas por Id — 2 principais
>   (`q_main_agua`, `q_main_estacao`), 4 de Guilda (`q_guilda_1..4`), 4
>   secundárias (`q_agua_dara`, `q_cobre_tacio`, `q_perna_apollo`,
>   `q_limpeza_borda`). Objetivos tipados (`Conversar`/`Derrotar`/`Coletar`/
>   `ResolverPuzzle`/`AlcancarFlag`), referenciando `CatalogoDeNpcs`,
>   `CatalogoDeInimigos` e `CatalogoDePuzzles` por Id (integração cruzada
>   real, não só descrita).
> - `CatalogoDeNpcs.cs`: 13 NPCs confirmados (7 do Enredo + 6 novos:
>   Mestra da Guilda, estalajadeira, ferreiro, criança de rua, informante
>   do viaduto, vendedora concorrente), cada um com `Papel`
>   (`Generico`/`Loja`/`Estalagem`/`Guilda`/`Ferreiro`/`Informante`), local
>   e descrição.
> - `CatalogoDeDialogos.cs`: roteiro confirmado cobrindo `Casa` (Sessão 1) e
>   `City`/`Game` das Sessões 2, 3, 4, 5, 6 e 7 — bate com "Sessões 1 a 7".
> - `CatalogoDePuzzles.cs`: 10 puzzles confirmados por Id
>   (`p_agua_casa`, `p_painel_energia`, `p_contador_sucata`,
>   `p_filtro_agua`, `p_sequencia_porta`, `p_calibragem_braco`,
>   `p_rota_patrulha`, `p_bomba_reserva`, `p_triagem_pecas`,
>   `p_antena_guilda`) — os 2 da matriz curricular (§ correspondente no
>   `readme.md`) mais os 8 extras.
> - `CatalogoDeLicoes.cs`: 9 conceitos confirmados (`variavel`, `while`,
>   `if`, `comparacao`, `contador`, `funcao`, `erro`, `atributo`,
>   `tecnica`), com apelidos normalizados (acento/maiúscula/sinônimo caem
>   no mesmo tópico). `Explicar(topico)` e `ListarTopicos()` confirmados.
> - `GerenciadorDeQuests.cs`: motor estático confirmado — `Aceitar`,
>   `AtualizarQuestsAutomaticas`, `RegistrarProgresso`, `Entregar`,
>   `PodeEntregar`, `Ativas()`/`Concluidas()`. Progresso 100% em flags/
>   contadores do `GerenciadorDeEstado` (nenhuma estrutura de save nova).
> - `QuestUIManager.cs`: diário confirmado — painel único, lista
>   ativas/concluídas, detalhe com objetivos/recompensas/curiosidade
>   (curiosidade só visível após `EstaConcluida`). Mesmo contrato dos
>   outros painéis (`painel`/`AoFechar`/`LayoutRebuilder` antes de popular).
>
> **1 dos 10 segue sem arquivo — `GatilhoDeQuest.cs`.** Não foi anexado
> nesta sessão, e nenhum dos 9 arquivos confirmados o referencia como
> dependência direta (o diário e o motor funcionam sem ele). Tratar como
> **⚠️ não confirmado** — o `LEIA_ISTO` descreve um componente de NPC que
> oferece/lembra/entrega quest ao interagir; sem o `.cs`, não é possível
> confirmar se esse fluxo já está ligado a `NpcInterativo`.
>
> **Divergência de contrato — `VerificadorDePuzzle.cs` (2 versões, ambas já
> vistas nesta reconciliação):**
> - **v1** (lida em sessão anterior, ainda documentada no corpo da §4.8):
>   lê `nomeDaVariavel`/`valorDeVitoria` direto do Inspector, sem `Id`.
> - **v2** (lida nesta sessão, substitui a v1): lê `idDoPuzzle` do
>   `CatalogoDePuzzles`, suporta múltiplas `VerificacaoDeVariavel`
>   simultâneas, conta tentativas e mostra dica do catálogo após N erros,
>   chama `GerenciadorDeQuests.RegistrarProgresso(ResolverPuzzle, ...)`, e
>   **mantém o modo manual da v1 como fallback** se `idDoPuzzle` ficar
>   vazio — não é uma reescrita destrutiva, é estritamente maior.
>   **A v2 é a versão vigente.** A descrição em §4.8 (corpo do documento)
>   ainda reflete a v1 e precisa ser atualizada numa próxima sessão de
>   edição da §4 — registrado aqui para não perder o rastro.
>
> **🔴 Bug de integração confirmado por leitura cruzada dos dois arquivos:**
> `VerificadorDePuzzle.cs` v2 chama `terminal.EscreverNoLog(...)` (método
> público) quando não há `textoDeApollo` configurado. O `TerminalUIManager.cs`
> desta mesma entrega **não tem esse método** — só existe
> `AdicionarLinhaDeLog`, que é `private`. Isso quebra a compilação sempre
> que um `VerificadorDePuzzle` v2 sem `textoDeApollo` tentar mostrar uma
> dica ou fala de vitória pelo log do terminal. Ver §9 para o registro
> formal da pendência.

---

## 9. Pendências técnicas conhecidas

> Itens abaixo marcados com data foram levantados numa varredura dedicada de bugs (não é lista de "coisas que faltam fazer" — isso é papel do `CHECKLIST_DE_DESENVOLVIMENTO.md`, que também lista os mesmos itens em "🔴 Bugs abertos").

- `TabelaDeCustos.CustoResistenciaPorNivel` está declarada mas não é referenciada em nenhum cálculo — confirmar se ainda é necessária ou remover.
- MoonSharp não tem proteção contra loop infinito (`while true do end` travaria o jogo) — sem solução ainda, fora de escopo imediato.
- ✅ **Resolvido (15/09/2026):** o `Debug.Log` de depuração de `DroidScriptRunner.ObterVariavelNumerica` foi removido. Ver §4.3.
- ✅ **Resolvido (15/09/2026):** `TransicaoDeCena.trancada`/`textoDeBloqueio`/`mensagemDeBloqueio` **são genéricos da classe**, confirmado com o arquivo real. §4.6 atualizada.
- **Pacote de 10 arquivos do Ato 1 (catálogos de conteúdo) — 9/10 confirmados nesta sessão.** Ver §8.1 para o detalhamento. Falta só `GatilhoDeQuest.cs` (não anexado) e `TerminalDroidApi.cs` com os métodos `Explicar`/`Ajuda`/`Resumo` (o arquivo em si nunca foi anexado nesta sessão nem na anterior — só é referenciado por `TerminalUIManager`/`DroidScriptRunner`, nunca com seu próprio conteúdo visível). **Bug de integração encontrado:** `VerificadorDePuzzle.cs` v2 chama `terminal.EscreverNoLog(...)`, método que não existe em `TerminalUIManager.cs` (só há `AdicionarLinhaDeLog`, privado) — quebra a compilação para qualquer `VerificadorDePuzzle` sem `textoDeApollo` configurado. Corrigir expondo `AdicionarLinhaDeLog` como público (ou criando `EscreverNoLog` como alias público) antes de montar cena com puzzle sem TMP de fala dedicado.
- **Divergência ainda aberta — HIT/FLEE e crítico (LUK).** `readme.md` diz implementado; §8 deste documento diz planejado. Resistência por INT está confirmada; o resto depende de `Droid.cs`/`CombatEngine.cs` serem colados numa próxima sessão. Ver a nota no fim da §5.
- **`InimigoFixo` já carrega efeitos ativos** (`EfeitosAtivos`/`EfeitosDeDanoAtivos` estão na interface e `BattleManager` aplica Debuff nele). Isso torna o bug "efeitos nunca zerados entre batalhas" **menos hipotético** do que a §4.1 sugere — mas o `InimigoFixo` é recriado a cada `BattleManager.Start()`, então o vazamento real continua sendo só do lado do `Droid` do jogador, que é persistente. Não corrigido nesta sessão (fora de escopo por instrução).

**Levantados em 12/09/2026 (varredura de bugs pós motor de efeitos) — status atualizado:**

- ✅ Corrigido: detecção de vitória/derrota atrasada (`BattleManager` agora checa os dois lados após todo `ExecutarTurno`).
- ✅ Corrigido: Save/Load descartava Veneno/Stun (`TecnicaSalva` agora grava `EfeitosDeAtributo`/`EfeitosDeDanoPorTurno`).
- ✅ Corrigido: Ataque Básico podia ser sobrescrito (proteção replicada nas 3 variantes de aprender).
- ✅ Corrigido: reaprender uma técnica não reembolsava — decisão tomada: **bloquear reaprendizado do mesmo nome**, introduzir `MelhorarTecnica(nome, nivelAdicional)` cobrando a diferença de custo. Ver §4.3.
- ✅ Corrigido: seta ↓ do terminal sem proteção contra edição manual, e sem posição "livre" pra voltar ao campo vazio.

**Levantados/corrigidos em 13/09/2026 (botão Voltar + exclusividade de painéis):**

- ✅ Corrigido: falta de botão "Voltar"/"Cancelar" nos menus de Ataque e Item da batalha — ver §4.6 (`BattleManager`).
- ✅ Corrigido: bug encontrado durante a correção acima — abrir o painel de Ataque e, sem fechar, abrir o de Item (ou vice-versa) deixava os dois ativos ao mesmo tempo. Ver §4.6.
- ✅ Corrigido: painéis de Status/Terminal não tinham exclusividade com o Menu do Mundo — ficavam sobrepostos visualmente. Ver §4.6 (`MenuMundoManager`/`TerminalUIManager`/`TelaDeStatusManager`).
- ✅ Corrigido: bug encontrado durante a correção acima — ESC com o Terminal aberto reabria o Menu do Mundo por baixo dele e incrementava o contador de menus de novo. Ver §4.6 (`MenuMundoManager`).
- Testado e confirmado pelo responsável do projeto.

**Ainda em aberto (não implementado nesta sessão, por decisão explícita):**

- **Efeitos ativos do Droid nunca são zerados entre batalhas** (`EfeitosAtivos`/`EfeitosDeDanoAtivos`). Sem efeito visível hoje (só `InimigoFixo` poderia causar isso e não tem `TecnicaComposta` ainda), mas é risco pro dia em que houver inimigo com efeito.
- **Empilhamento sem limite de Veneno/Stun** (`TecnicaComposta`/`Droid.EfeitosAtivos`/`EfeitosDeDanoAtivos`). Cada acerto soma uma nova instância — Veneno acumula dano por turno de todas as instâncias; Stun repetido não estende a duração de fato (`EfeitosTemporariosUtil.Decrementar` decrementa cada instância independentemente). **Decisão explicitamente adiada** — não implementar cap/renovação sem definição de comportamento desejado primeiro.
- **Painel de lista de itens/botão Item (`BattleManager.painelListaDeItens`/`prefabBotaoItem`)** ainda está "mal otimizado" — trabalho de configuração/revisão de UI pendente, fora do escopo desta sessão. Ver seção "Trabalho de Editor pendente" no `CHECKLIST_DE_DESENVOLVIMENTO.md`.