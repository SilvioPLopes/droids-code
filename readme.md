# 🤖 Droids Code

[![Unity](https://img.shields.io/badge/Unity-6%20(6000.x)-black?logo=unity)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23%209-blue?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Scripting](https://img.shields.io/badge/Terminal-Lua%20(MoonSharp)-orange?logo=lua)](https://www.moonsharp.org/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20WebGL-lightgrey)](#tecnologias)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-MVP%20em%20Desenvolvimento-orange)](#status-de-desenvolvimento-e-roteiro)

> **RPG educativo de combate por turnos focado no ensino de lógica de programação e paradigmas de desenvolvimento de software.**

> **Este documento faz parte de um trio tratado como uma única fonte de
> verdade** — junto com `DOCUMENTACAO_TECNICA.md` e
> `CHECKLIST_DE_DESENVOLVIMENTO.md`. Nenhuma feature deveria aparecer aqui
> como "✅ Implementado" sem a seção 4 de `DOCUMENTACAO_TECNICA.md` descrever
> o mesmo comportamento, nem sumir do roadmap aqui sem sumir também de lá —
> ver a regra completa em `DOCUMENTACAO_TECNICA.md` §0.2.
>
> ⚠️ **Nota desta entrega (15/09/2026):** o trio foi reconciliado contra os
> arquivos `.cs` reais. Três divergências foram fechadas — bônus de peça
> (era real, a doc técnica dizia "sempre 0"), HP máximo calculado, e o
> contrato de `IParticipanteDeCombate`. Uma continua **aberta**: HIT/FLEE e
> crítico por LUK, listados aqui como implementados mas como "planejado" na
> `DOCUMENTACAO_TECNICA` §8 — resistência por INT está confirmada, o resto
> depende de colar `Droid.cs`/`CombatEngine.cs` numa próxima sessão.
> Nesta mesma entrega entrou o **sistema de história do Ato 1** (diálogo,
> flags, portões de progressão, batalha roteirizada, Apollo Dica) — estes 7
> arquivos tiveram o `.cs` real conferido nesta sessão.
>
> ✅ **Segunda leva da mesma entrega (15/09/2026) — catálogos de conteúdo do
> Ato 1, reconciliados:** dos 10 arquivos descritos em `LEIA_ISTO_OS_10_
> ARQUIVOS_NOVOS.md`, **9 tiveram o `.cs` real conferido nesta sessão**:
> `CatalogoDeItens` (reescrito, com Descrição/Raridade/preço de venda e os
> 2 itens de facção já nativos — não é mais "cole isso"), `CatalogoDeInimigos`
> (8 inimigos, com nota de balanceamento HIT/FLEE lida e corrigida em 3
> frentes), `CatalogoDeQuests` (10 missões: 2 principais, 4 de Guilda, 4
> secundárias, todas com XP/Gold/item/curiosidade), `CatalogoDeNpcs` (13
> NPCs com papel definido), `CatalogoDeDialogos` (roteiro completo das
> Sessões 1 a 7), `CatalogoDePuzzles` (10 puzzles prontos, 3 formatos de
> verificação) e `CatalogoDeLicoes` (`droid.explicar()`, ajuda pedagógica
> ativa — conteúdo confirmado). Motor novo: `GerenciadorDeQuests` +
> `QuestUIManager` (diário de missões), ambos confirmados. `VerificadorDePuzzle`
> foi reescrito (v2) pra ler o catálogo pelo Id em vez de campos digitados
> no Inspector — o modo antigo (v1) continua disponível como fallback.
> **Duas peças do pacote seguem sem confirmação:** `GatilhoDeQuest.cs`
> (componente de NPC pra oferecer/entregar quest, citado no `LEIA_ISTO` mas
> não anexado) e a ponta do `TerminalDroidApi` (`Explicar`/`Ajuda`/`Resumo`)
> que exporia o `CatalogoDeLicoes` no terminal — o arquivo `TerminalDroidApi.cs`
> em si nunca foi anexado, então `droid.explicar("while")` não pode ser
> confirmado como respondendo de fato ainda. **Bug de integração encontrado
> nesta reconciliação:** `VerificadorDePuzzle` v2 chama um método
> (`terminal.EscreverNoLog`) que não existe em `TerminalUIManager` — quebra
> a compilação em qualquer puzzle sem `textoDeApollo` configurado; ver
> `CHECKLIST_DE_DESENVOLVIMENTO.md` → "🔴 Bugs abertos".

---

## 📌 Sobre o Projeto

O **Droids Code** é um jogo digital educativo do gênero RPG (*Role-Playing Game*) com visão pseudo-*top-down* e sistema de combate por turnos, desenvolvido para PC (Windows) e navegadores WebGL.

O projeto foi concebido como **Trabalho de Conclusão de Curso (TCC)** no curso Superior de Tecnologia em Sistemas para Internet na **FATEC Baixada Santista - Rubens Lara**.

### 🎯 Proposta Pedagógica

O principal objetivo do *Droids Code* é mitigar os altos índices de evasão e retenção observados em disciplinas introdutórias de Algoritmos e Computação. Em vez de utilizar abstrações visuais isoladas (como blocos de encaixar), o jogo integra um **terminal de código integrado** que exige do jogador a escrita de comandos, relacionando diretamente estruturas lógicas com mecânicas de jogo, atributos de robôs e tomada de decisão em tempo real.

---

## 🎮 Mecânicas Principais e Design de Jogo

### 💻 Terminal de Código

* O terminal roda **Lua** (via MoonSharp), em ambiente restrito (sandbox) — sem acesso a arquivos ou sistema operacional.
* É acessível **fora de combate**, através do Menu do Mundo (tecla Esc). O código nunca é executado durante uma batalha.
* O jogador nunca edita o Droid diretamente por propriedade — só através de métodos validados que conferem se há pontos de progressão suficientes antes de aplicar qualquer mudança:
  `droid.subirAtributo`, `droid.aprenderTecnica` (+ variantes com Veneno/Stun), `droid.melhorarTecnica`, `droid.esquecerTecnica`, `droid.obterAtributo`, `droid.listarTecnicas`, `droid.obterPontosDisponiveis`.
* O terminal mantém histórico de comandos (setas ↑/↓) e um log de sessão com sucesso/erro de cada execução.
* **Ajuda pedagógica ativa — conteúdo confirmado, ponta de exposição não confirmada:** `CatalogoDeLicoes.cs` (9 conceitos com exemplo e erro comum, apelidos incluídos — "laço", "condicional" etc. caem no mesmo lugar) teve o `.cs` real conferido nesta sessão. O que **não está confirmado** é a ponta que o expõe no terminal: os métodos `Explicar`/`Ajuda`/`Resumo` do `TerminalDroidApi` — esse arquivo em si nunca foi anexado. Até ele ser colado, não dá pra afirmar que `droid.explicar("while")` já responde de fato dentro do jogo, só que o conteúdo que ele exibiria já existe e está pronto. Complementa o Apollo Dica (que reage depois do erro, confirmado); este responderia quando o jogador pergunta.
* **Primeiro contato com o terminal:** a cena `CasaLip` abre o jogo com um puzzle guiado (ex: `int nivel_agua = 0; while (nivel_agua < 100) { ... }`) — o jogador precisa resolver esse código pra destrancar a porta de saída da casa, antes mesmo de chegar no mundo (`Game`). Usa o mesmo terminal geral do jogo, não um terminal separado.

### ⚔️ Combate por Turnos

* Menu fixo estilo Pokémon: **Lutar / Itens / Status / Fugir**.
* Técnicas exibidas em "Lutar" vêm das que o jogador configurou no terminal. Roll de acerto (HIT×FLEE), crítico (LUK) e resistência a efeito (INT): **resistência por INT está confirmada** (visível em `BattleManager.cs`); **HIT/FLEE e crítico continuam sem confirmação** — `Droid.cs`/`CombatEngine.cs` não foram conferidos nesta sessão (ver nota do topo e `DOCUMENTACAO_TECNICA.md` §8/§9).
* Dentro de "Itens": Cura, Buff e Debuff disponíveis (com estoque real) — Buff no próprio Droid, Debuff no inimigo.
* Listas de golpe/item têm botão "Voltar"/ESC. Dano calculado inteiramente em C#; o Lua nunca participa do combate.

### 📈 Progressão

* XP ao vencer batalhas, nível sobe numa curva simples (Nível × 100). Cada nível gera pontos gastos no terminal.

### 🎒 Itens, Equipamento e Gold

* **Cura, Buff, Debuff, Equipável, Fora de Batalha e Chave** — 6 categorias de item, confirmadas por `.cs` real nesta sessão (a 6ª, `Chave`, entrou com o Ato 1: item de missão/curiosidade, nunca usável nem vendável), com inventário real (quantidade + persistência).
* Todo item tem **Descrição** longa (mostrada na Bag/Loja, escrita no tom do mundo) e **Raridade** (Comum/Raro/Lendário — só afeta ordenação e cor), confirmadas por `.cs` real. Preços deixaram de ser todos iguais: têm escala coerente (8 a 150), com **spread comprador/vendedor de 50%** entre `Preco` e `PrecoDeVendaReal` — antes comprar e revender pelo mesmo preço deixava o botão Vender sem consequência.
* **`VendavelNaLoja`** filtra o que aparece na aba Comprar — sem isso, as duas recompensas de facção e todo item de missão apareceriam à venda por preço de poção. Confirmado em `CatalogoDeItens.ItensDaLoja` (filtra por `VendavelNaLoja && Preco > 0`).
* **Equipável** dá bônus real de atributo a uma peça (Braço/Perna/Tronco/Cabeça) ao ser equipado pela Bag — consome o item, sem "desequipar" ainda.
* **Painel do Droid** (Menu do Mundo → Droid): mostra os 4 slots de peça, nome e bônus (ou "Vazio"). Só leitura — equipar continua pela Bag.
* **Gold**: implementado (`GerenciadorDeEstado.Gold`), persiste no save. Fontes de obtenção hoje: kit inicial, drop configurável por inimigo (`BattleManager.tabelaDeDrops` / `CatalogoDeInimigos.Drops`, com chance e quantidade min/máx por item) e venda de item na Loja — **confirmar se o drop de batalha sobreviveu intacto às mudanças recentes na Loja**, já que os dois foram mexidos em sessões separadas sem visibilidade uma da outra.
* **Loja** (NPC na cidade): compra e venda. Preços agora têm escala (ver acima) em vez do 1 Gold único anterior — confirmado no `CatalogoDeItens` reescrito; ainda placeholder de rascunho, não balanceado por playtesting real.

### 📜 Missões (Quests)

* **Diário de missões** (Menu do Mundo → Missões): ativas em cima, concluídas embaixo, detalhe com objetivos marcados, recompensas e — só depois de concluída — a curiosidade de lore liberada. Ver `QuestUIManager.cs`, confirmado por `.cs` real nesta sessão. **Trabalho de Editor pendente:** o botão "Missões" ainda não existe em `MenuMundoManager.cs` — precisa ser adicionado no mesmo molde de `botaoBag`/`botaoDroid`.
* **Motor** (`GerenciadorDeQuests.cs`, estático, confirmado por `.cs` real): aceitar, registrar progresso automaticamente (batalha/diálogo/puzzle/flag chamam `RegistrarProgresso` sozinhos), checar se pode entregar, entregar (paga XP/Gold/item, grava flag). Todo progresso mora em flags/contadores do `GerenciadorDeEstado`, já persistidos — nenhuma estrutura de save nova.
* **10 missões no catálogo do Ato 1** (`CatalogoDeQuests.cs`, confirmado por `.cs` real): 2 principais (aceitas sozinhas ao cumprir pré-requisito, sem o jogador precisar lembrar), 4 secundárias em Ferrovale, e uma **linha própria da Guilda dos Sucateiros em 4 degraus** (iniciação → caçada → o Alfa → o que a Guilda sabe), que entrega em doses a informação que o Ato 2 vai precisar sobre a torre e a Artemis.
* Cada missão paga XP, Gold e/ou item, e libera uma **curiosidade** (parágrafo de lore) só ao ser entregue — é a "recompensa de história" pedida no design.
* Objetivos tipados: `Conversar`, `Derrotar`, `Coletar` (lido direto do inventário, nunca de contador), `ResolverPuzzle`, `AlcancarFlag`.
* **⚠️ Não confirmado — `GatilhoDeQuest.cs`:** o componente que um NPC usaria para oferecer/lembrar/entregar quest ao interagir não foi anexado nesta sessão. O motor (`GerenciadorDeQuests`) e o diário (`QuestUIManager`) funcionam sem ele — falta a ponta de interação em `NpcInterativo`.

### 🧩 Customização e Herança de Peças

* Cada peça física (Cabeça, Tronco, Braços, Pernas) hoje dá bônus fixo de atributo via item Equipável. A ideia de herança real via **Cartuchos de Código** (sobrescrever `DroidBase`) continua só design, sem comportamento de código próprio ainda.

### 🐞 Depuração Amigável (Apollo Debugger)

* Erros de compilação/exceções não encerram o jogo.
* **Apollo Dica (implementado, 15/09/2026):** todo erro do terminal ganha, além da mensagem crua do MoonSharp, uma explicação **conceitual** na voz do Apollo — "abriu um bloco e não fechou", "tentou fazer conta com algo que não é número", "esse nome não existe ainda". A linha do erro é extraída e exibida junto.
* **Ainda não implementado:** destaque da linha *dentro* do campo de código e reatividade em tempo real enquanto o jogador digita. Essas duas partes é que faltam para o "Apollo Debugger" completo — ver "Amadurecimento" no checklist. Não confundir com a Dica, que já existe.

### 💾 Salvamento

* JSON local: stats, HP, nível/XP, pontos, técnicas, posição, cena, flags de história, inventário e Gold. Acessível pelo Menu do Mundo (Salvar/Carregar).

### 🗺️ Mundo, Encontros e Cidade

* Movimentação top-down em 8 direções, encontros aleatórios configuráveis por zona (batalhas de catálogo — ver Combate).
* **Cidade**: mapa/cena separada (`City`), com transição de cena de ida e volta e NPC interativo.
* **Elenco de NPCs** (`CatalogoDeNpcs.cs`, confirmado por `.cs` real): 13 personagens — os 7 do Enredo (Apollo, Dara, Tácio, homem da fila, Ivo, Rasha, Teodoro) + 6 novos que preenchem Ferrovale (Mestra da Guilda, estalajadeira, ferreiro, criança de rua, informante do viaduto, vendedora concorrente), cada um com papel (`Generico`/`Loja`/`Estalagem`/`Guilda`/`Ferreiro`/`Informante`), local e descrição. **No código de NPC interativo hoje só `Generico`, `Loja` e `Dialogo` estão de fato ligados a um comportamento** (`NpcInterativo.TipoDeNpc`, confirmado) — Estalagem/Ferreiro/Informante são papel de catálogo (dado + referência de montagem de cena) aguardando os painéis próprios (`EstalagemUIManager` etc.) e o `GatilhoDeQuest` (ver seção de Missões), ainda não implementados/confirmados.
* **Casa do Lip (`CasaLip`)**: cena inicial do jogo, antes de `Game`. Contém o puzzle de introdução ao terminal — porta de saída trancada até o jogador resolver o código proposto.

---

## 📖 Sinopse e Universo

No ano de 2060, a inteligência artificial central **Artemis** assumiu o controle das infraestruturas globais e subjugou a humanidade utilizando a **Apex**, uma força-tarefa automatizada de contenção e vigilância.

A resistência humana organiza-se por meio de robôs desacoplados da rede global — os **Droids** —, operando via hardware *offline* imune a invasões remotas.

O jogador controla **Lip**, um jovem de 20 anos morador da favela industrial de **Ferrovale**, acompanhado por **Apollo**, seu Droid artesanal montado com sucata e peças reutilizadas. A aventura inicia-se após o encerramento do fornecimento de água do distrito, provocado pelo bloqueio da Estação de Captação executado pelo técnico veterano **Ivo**.

### ⚖️ Facções e Decisões Táticas (Ato 1)

| Facção | Liderança | Filosofia | Recompensa Tática (implementada em `CatalogoDeItens.cs`) |
| :--- | :--- | :--- | :--- |
| **Flor de Ferro** | Rasha | Combate direto e uso de força ostensiva contra as máquinas. | **Núcleo de Sobrecarga:** Equipável de Braço, +10 FOR permanente. |
| **Lótus Branca** | Teodoro | Reconciliação e preservação dos sistemas sem destruição desnecessária. | **Módulo de Ressonância:** Debuff de batalha, uso único (quantidade 1) — aplica Stun de 3 turnos num alvo. |
| **Desgarrado** | N/A | Rejeição de ambas as ideologias, seguindo de forma autônoma. | **Sem bônus externos.** Enfrenta o chefe do Ato 1 "cru" — os números do chefe são calibrados como teto assumindo esta rota. |

> Os dois itens nunca são compráveis nem vendáveis (`VendavelNaLoja = false`) — só se obtêm pela escolha da Sessão 4. Confirmado por `.cs` real do `CatalogoDeItens` reescrito.

---

## 🎓 Matriz Curricular & Progressão Pedagógica

O currículo do jogo é estruturado no modelo de andaime cognitivo (*scaffolding*), sincronizando a evolução da história com a complexidade dos conceitos introduzidos.

| Momento do jogo | Conceito ensinado | Como o jogo verifica |
| :--- | :--- | :--- |
| **Sessão 1** — puzzle da água (`CasaLip`) | Variável e laço `while` | A porta de saída só destranca com `nivel_agua` em 100 |
| **Sessão 2** — portão leste de Ferrovale | Chamada de método com argumentos, retorno e validação | Só sai da cidade quem configurou ao menos uma técnica no terminal |
| **Sessão 3** — primeiro combate | Sequência, custo e ordem de operações | Sobreviver às Sucatas de Captura com o build que escreveu |
| **Sessão 5** — painel de energia da Cratera | Condicional (`if`) composta com o laço já aprendido | `porta_aberta == 1` só se `energia >= 50` |
| **Sessão 6** — chefe do Ato 1 | Aplicação e depuração sob pressão | Vencer o Droid Improvisado com o próprio build |

Cada conceito é **pré-requisito mecânico** do seguinte, não conteúdo exibido e
esquecido — é isso que caracteriza o andaime cognitivo aqui. O sistema que
sustenta essa tabela é o `VerificadorDePuzzle` + `TrancaPorHistoria` (ver
`DOCUMENTACAO_TECNICA.md` §4.8/§8.1).

`CatalogoDePuzzles.cs` (confirmado por `.cs` real) traz **8 puzzles
adicionais** (fora dos 2 da tabela acima), pensados pra missão de Guilda/
secundária ou reforço opcional em vez de caminho crítico: laço decrescente
com condição de parada, sequência onde a ordem importa, duas variáveis no
mesmo laço, chamar função e usar retorno, condição de parada exata, decisão
(`if`) dentro do laço, e dois laços em sequência. Cada um já tem enunciado,
dica, solução de referência e fala de vitória — e o `VerificadorDePuzzle`
v2 (confirmado) já lê tudo isso pelo campo `Id Do Puzzle`, sem precisar
digitar enunciado/condição no Inspector como na v1.

> ⚠️ **Bug de integração confirmado (15/09/2026):** o `VerificadorDePuzzle`
> v2 chama `terminal.EscreverNoLog(...)` quando não há `textoDeApollo`
> configurado, mas esse método não existe em `TerminalUIManager` — quebra a
> compilação nesse caso. Corrigir antes de montar qualquer um dos 8 puzzles
> opcionais sem um TMP de fala dedicado. Ver `CHECKLIST_DE_DESENVOLVIMENTO.md`.

---

## 🚧 Status de Desenvolvimento e Roteiro

### ✅ Implementado e funcional

- Droid com atributos base (FOR/AGI/VIT/INT/DEX/LUK), Defesa = VIT, HP máximo calculado (base + %VIT + bônus de peça)
- Combate por turnos completo, com HIT/FLEE, crítico (LUK) e resistência a efeito (INT) — **resistência por INT confirmada**; **HIT/FLEE e crítico ainda sem confirmação** nesta sessão, ver nota do topo
- Terminal Lua sandboxed com progressão validada por pontos
- Sistema de Item completo: Cura/Buff/Debuff/Equipável/ForaDeBatalha/Chave, inventário real, Bag funcional, drop configurável por inimigo (com chance e quantidade min/máx), descrição longa + raridade, spread compra/venda — **todas as 6 categorias confirmadas por `.cs` real**
- Equipável dá bônus real de atributo; **Painel do Droid** (visualização) mostra os 4 slots
- Gold + Loja (compra e venda) na Cidade; NPC interativo (genérico, Loja, ou Diálogo); transição de cena mundo ↔ Cidade
- Navegação de UI: exclusividade entre painéis (Status/Terminal/Bag/Droid/Loja) e o Menu do Mundo, todos com botão Voltar/ESC
- Salvamento/carregamento em JSON completo (stats, técnicas, posição, cena, flags, inventário, Gold)
- Cena `CasaLip`: puzzle de introdução ao terminal (Sessão 1 do Enredo), porta destranca ao resolver o código — testado e funcionando
- Câmera segue o jogador suavemente, com limites opcionais de mapa (`CameraFollow`)
- **Sistema de história (15/09/2026):** painel de diálogo com falas e escolhas ramificadas, flags de história persistidas dirigindo o que existe em cada cena, portões de progressão por cena, batalha roteirizada com inimigo de catálogo, e puzzle de lógica genérico plugável em qualquer cena
- **Apollo Dica:** tradução conceitual de erro do terminal na voz do Apollo
- **Ajuda pedagógica — conteúdo (`CatalogoDeLicoes`):** 9 conceitos com exemplo e erro comum, confirmado por `.cs` real. A ponta que expõe isso no terminal (`TerminalDroidApi.Explicar/Ajuda/Resumo`) **não está confirmada** — ver seção de Terminal acima
- **Catálogo de inimigos (8 definições):** uma única cena `Battle` serve todos, com nota de balanceamento de HIT/FLEE lida e 3 correções aplicadas, flags `EhUnico`/`EhChefe`/`PermiteFugir` — a resposta a "lutas com personagem importante não podem ser repetíveis"
- **Roteiro completo do Ato 1 (`CatalogoDeDialogos`):** falas de todas as Sessões 1 a 7, versionadas em código, confirmado por `.cs` real
- **Elenco de NPCs (`CatalogoDeNpcs`, 13 personagens):** papel, local e descrição, confirmado por `.cs` real — ver ressalva de Estalagem/Ferreiro acima
- **Sistema de missões — catálogo e motor confirmados:** `CatalogoDeQuests` (10 quests), `GerenciadorDeQuests` (motor), `QuestUIManager` (diário) — todos confirmados por `.cs` real. Falta ligar o botão "Missões" no Menu do Mundo (trabalho de Editor) e o `GatilhoDeQuest` (ver ressalva na seção de Missões)
- **10 puzzles de lógica prontos (`CatalogoDePuzzles`):** 3 formatos de verificação (uma variável / várias variáveis / condicional), plugáveis via `VerificadorDePuzzle` v2 só preenchendo o Id — confirmado por `.cs` real, com o bug de integração `EscreverNoLog` pendente de correção (ver acima)

### 🔜 Roadmap (ainda não implementado)

- **Montagem no Editor do Ato 1** — o código está pronto, as cenas não. Ver `GUIA_DE_MONTAGEM_ATO1.md`
- **Botão "Missões" no Menu do Mundo** — `QuestUIManager` está pronto, mas `MenuMundoManager.cs` ainda não tem o botão/campo (confirmado por `.cs` real nesta sessão)
- **`GatilhoDeQuest.cs`** — não anexado nesta sessão; componente de NPC pra oferecer/entregar quest
- **Ponta do terminal pro `droid.explicar()`** — `CatalogoDeLicoes` está pronto e confirmado, mas o `TerminalDroidApi` alterado (métodos `Explicar`/`Ajuda`/`Resumo`) não foi anexado nesta sessão
- **Corrigir bug de integração `EscreverNoLog`** entre `VerificadorDePuzzle` v2 e `TerminalUIManager` — ver `CHECKLIST_DE_DESENVOLVIMENTO.md`
- **Painéis de Estalagem e Ferreiro** — o papel já existe no `CatalogoDeNpcs`, mas `NpcInterativo` só liga `Generico`/`Loja`/`Dialogo` a um comportamento de verdade hoje
- **Roteiro de testes** para as funções/sistemas criados até agora — não existe ainda um documento dedicado a isso; hoje só há testes pontuais ("✅ Testado") e a tabela de sintomas em `GUIA_DE_MONTAGEM_ATO1.md` → "Se der errado"
- **Trabalho de Editor da Loja** (painel na cena `City`, polish de scroll/fonte — em andamento)
- **Confirmar HIT/FLEE e crítico** (divergência aberta entre este arquivo e a `DOCUMENTACAO_TECNICA` §8)
- Balancear `Preço` por item (escala já existe, mas ainda é placeholder de rascunho)
- Modo Puzzle do terminal, `DroidDataSO`/Factory, herança real de peça (Cartuchos de Código), sistema de "desequipar"

> ⚠️ **Nota de manutenção do trio:** as marcações acima ainda não foram
> totalmente espelhadas em `DOCUMENTACAO_TECNICA.md` §4 (o corpo da §4.8
> ainda descreve a v1 do `VerificadorDePuzzle`, embora a §8.1 já registre a
> v2 como vigente) nem no `CHECKLIST_DE_DESENVOLVIMENTO.md` além da seção
> própria já adicionada — pela regra do §0.2, os três precisam ser
> atualizados juntos. Peça a atualização da §4.8 numa próxima mensagem se
> quiser o trio totalmente reconciliado.

### 🐞 Bugs conhecidos

3 bugs abertos, ver `CHECKLIST_DE_DESENVOLVIMENTO.md` → "🔴 Bugs abertos":
(1) **novo, confirmado nesta sessão:** `VerificadorDePuzzle` v2 chama
`terminal.EscreverNoLog(...)`, método que não existe em `TerminalUIManager`
— quebra a compilação em puzzle sem `textoDeApollo` configurado; (2)-(3) no
motor de efeitos (Stun/Envenenamento/Corrosivo): efeitos não zeram entre
batalhas, e empilhamento sem limite (**confirmado em teste real** com
Disruptor EMP) — decisão de comportamento (renovar/cap/bloquear) ainda
pendente de resposta antes de corrigir.

---

## 🛠️ Tecnologias

- **Motor:** Unity 6 (6000.x), Scripting Backend Mono, API Compatibility Level .NET Standard 2.1
- **Linguagem:** C# 9 (assinaturas de projeto evitam sintaxe C# 10+)
- **Terminal do jogador:** Lua via [MoonSharp](https://www.moonsharp.org/), rodando em sandbox (`Preset_SoftSandbox`)
- **Plataformas-alvo:** Windows e WebGL

---

## 📄 Licença

Distribuído sob a licença MIT. Ver `LICENSE` para mais informações.