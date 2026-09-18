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
>
> 📖 **Terceira leva (17/09/2026) — bíblia narrativa reescrita e catálogos de
> Ato 1 expandidos para v2:** `TCC_-_ENREDO.md` foi substituído por
> `TCC_-_ENREDO_v2.md` (roteiro completo do Ato 1 em prosa, cronologia
> fechada, fichas de personagem) + `TCC_-_ENREDO_v2_ADENDO.md` (correção de
> cânone do Apollo — deixou de ser núcleo único e passou a ser 1 de 431
> cópias despachadas por Célia em 2060, distinção que só se paga por
> convivência — e desenho completo do confronto final em 3 fases, sem
> combate). Junto vieram 4 catálogos `.cs` atualizados/novos, todos
> validados por script (chaves/parênteses balanceados, toda constante com
> entrada, nenhuma órfã):
> `CatalogoDeDialogos.cs` (846 linhas, 63 entradas, era 280 linhas),
> `CatalogoDeQuests.cs` (18 missões, era 10 — ver detalhe na seção de
> Missões), `CatalogoDeNpcs.cs` (16 personagens, era 13 — ver detalhe na
> seção de NPCs) e `CatalogoDeLocais.cs` (**arquivo novo**, 19 locais — ver
> seção própria abaixo).
>
> **Isso é conteúdo e narrativa, não engine.** Nenhuma mudança desta leva
> altera comportamento de código já existente — os itens já sinalizados
> como não confirmados nas levas anteriores (`GatilhoDeQuest`,
> `TerminalDroidApi`, HIT/FLEE/crítico, o bug `EscreverNoLog`) continuam
> exatamente como estavam, sem relação com este pacote.
>
> ⚠️ **Uma dependência de compilação nova:** `CatalogoDeNpcs.cs` v2 agora
> referencia `CatalogoDeLocais.Id*` nos campos `IdLocalPadrao` e `Rotina` —
> os dois arquivos precisam ser colados juntos no projeto, ou
> `CatalogoDeNpcs.cs` não compila sozinho. Antes desta leva ele não tinha
> nenhuma dependência de outro catálogo.
>
> ⚠️ **Divergência aberta, não corrigida nesta leva:** as idades de Dara,
> Rasha, Teodoro, Ivo e Nadir em `CatalogoDeNpcs.cs` (22/35/48/67/46) não
> batem com as da bíblia narrativa em `TCC_-_ENREDO_v2.md` §IV
> (24/36/52/68/41) — os dois arquivos foram escritos em sessões diferentes
> sem conferência cruzada. `TCC_-_ENREDO_v2.md` deveria prevalecer por ser
> o documento de cânone; `CatalogoDeNpcs.cs` precisa de correção numa
> próxima sessão.
>
> **Fora do escopo desta leva, ainda no v1 e sem reconciliação:**
> `CatalogoDeInimigos.cs`, `CatalogoDeItens.cs`, `CatalogoDePuzzles.cs`,
> `CatalogoDeLicoes.cs` e `GUIA_DE_MONTAGEM_ATO1.md` — nenhum deles foi
> tocado, por decisão explícita de escopo, não por descuido.
>
> 🔧 **Quarta leva (17/09/2026) — engine puro, sem conteúdo/narrativa:
> `Droid.cs`, `TerminalDroidApi.cs` e `BagUIManager.cs` finalmente
> conferidos:**
> A **única divergência aberta desde a primeira entrega fecha aqui:**
> HIT/FLEE e crítico por LUK **estão implementados**, direto em
> `Droid.ExecutarAcao` (`RolarAcerto`/`RolarCritico`, Estágio 1,
> 12/09/2026) — mais antigos que as próprias notas que os marcavam como
> não confirmados. `CombatEngine.cs` (quem orquestra o turno) continua não
> anexado, mas a lógica de acerto/crítico em si não depende dele.
> **Achado novo, não sinalizado em nenhuma leva anterior:** INT tem
> **dois** efeitos, não um — além de `% resistência a efeito` (já
> documentado), também dá `% bônus de cura recebida`
> (`Droid.BonusPercentualDeCura`, usado de fato em
> `BagUIManager.UsarCuraForaDeBatalha`). Mesmo padrão do VIT (Defesa + HP),
> só que nunca foi anotado aqui. `TerminalDroidApi.cs` também confirmado:
> os métodos `Explicar`/`Ajuda`/`Resumo` **não existem** — deixa de ser
> "arquivo não anexado" e vira lacuna confirmada de verdade (ver Terminal
> abaixo). **Achado extra:** `droid.testeAdicionarPontos(quantidade)` é
> real e está exposto no terminal Lua — dá pontos de progressão de graça,
> sem custo, e o próprio código já pede pra remover/esconder antes de
> build final; não estava listado em lugar nenhum do README.
> **Item novo:** `Módulo de Treinamento` (`CatalogoDeItens`), categoria
> Fora de Batalha, concede 100 de XP via `SistemaDeProgressao` (mesmo
> caminho do XP de batalha — pode disparar level up).
> **Em aberto pra confirmar:** `ItemConsumivel.cs` — lista fixa e mais
> antiga de item de cura, com valores DIFERENTES do `CatalogoDeItens`
> atual (Poção Pequena/Média/Grande: 2/5/8 HP ali vs. 20/50/80 HP aqui).
> Sem `BattleManager.cs` não dá pra confirmar se é código morto ou se
> ainda alimenta o botão "Item" da batalha — se estiver ativo, é uma
> inconsistência real de valores entre batalha e Bag.

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
* ⚠️ **Comando de teste exposto no terminal (confirmado, 17/09/2026):** `droid.testeAdicionarPontos(quantidade)` também existe em `TerminalDroidApi.cs`, mas foge do padrão acima — soma pontos de progressão direto, sem custo e sem validar nada, ignorando a economia normal do jogo. O próprio código já sinaliza pra remover ou esconder isso antes de qualquer build final/demo pra terceiros; não estava listado aqui ainda.
* O terminal mantém histórico de comandos (setas ↑/↓) e um log de sessão com sucesso/erro de cada execução.
* **Ajuda pedagógica — conteúdo pronto, ponta confirmada como ainda não implementada (17/09/2026):** `CatalogoDeLicoes.cs` (9 conceitos com exemplo e erro comum, apelidos incluídos — "laço", "condicional" etc. caem no mesmo lugar) teve o `.cs` real conferido. `TerminalDroidApi.cs` também já foi conferido nesta sessão — e os métodos `Explicar`/`Ajuda`/`Resumo` **não existem** nele (os métodos reais hoje são os listados acima, mais `testeAdicionarPontos`). Deixa de ser incógnita ("arquivo nunca anexado") e vira lacuna confirmada: `droid.explicar("while")` ainda não responde nada de fato no terminal, mas o conteúdo que ele exibiria já existe e está pronto. Complementa o Apollo Dica (que reage depois do erro, confirmado); este responderia quando o jogador pergunta.
* **Primeiro contato com o terminal:** a cena `CasaLip` abre o jogo com um puzzle guiado (ex: `int nivel_agua = 0; while (nivel_agua < 100) { ... }`) — o jogador precisa resolver esse código pra destrancar a porta de saída da casa, antes mesmo de chegar no mundo (`Game`). Usa o mesmo terminal geral do jogo, não um terminal separado.

### ⚔️ Combate por Turnos

* Menu fixo estilo Pokémon: **Lutar / Itens / Status / Fugir**.
* Técnicas exibidas em "Lutar" vêm das que o jogador configurou no terminal. Roll de acerto (HIT×FLEE), crítico (LUK) e resistência a efeito (INT) — **os três confirmados por `.cs` real (17/09/2026)**: HIT/FLEE e crítico em `Droid.ExecutarAcao` (`Droid.cs`), resistência por INT em `BattleManager.cs`. `CombatEngine.cs` (quem orquestra o turno) continua não anexado, mas não é de onde vem essa lógica.
* **INT tem dois efeitos, não um** (mesmo padrão do VIT, que já dá Defesa + %HP): além de `% resistência a efeito`, também dá `% bônus de cura recebida` (`Droid.BonusPercentualDeCura`) — de fato usado em `BagUIManager.UsarCuraForaDeBatalha` pra multiplicar a cura de poção. Não é bug, é intencional; só nunca tinha sido documentado.
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
* **Novo (17/09/2026) — item que concede XP direto:** `Módulo de Treinamento` (`CatalogoDeItens.IdModuloDeTreinamento`), categoria Fora de Batalha, usa a ação nova `AcaoForaDeBatalha.GanhoDeExperiencia`. Ao usar, chama `SistemaDeProgressao.GanharExperiencia` — o **mesmo caminho** do XP ganho em batalha, então pode disparar level up e conceder pontos de progressão normalmente. Preço e raridade são placeholder de rascunho.
* ⚠️ **Em aberto:** `ItemConsumivel.cs` ainda existe no projeto — uma lista fixa e mais antiga de item de cura, sem quantidade/persistência, com valores de cura **diferentes** dos do `CatalogoDeItens` atual (Poção Pequena/Média/Grande: 2/5/8 HP ali vs. 20/50/80 HP aqui). O comentário do próprio arquivo sugere que ele ainda alimenta o botão "Item" da batalha sem menu real; sem `BattleManager.cs` não dá pra confirmar se isso é código morto ou se está mesmo ativo — se estiver, é uma inconsistência real entre a cura em batalha e a cura pela Bag.

### 📜 Missões (Quests)

* **Diário de missões** (Menu do Mundo → Missões): ativas em cima, concluídas embaixo, detalhe com objetivos marcados, recompensas e — só depois de concluída — a curiosidade de lore liberada. Ver `QuestUIManager.cs`, confirmado por `.cs` real nesta sessão. **Trabalho de Editor pendente:** o botão "Missões" ainda não existe em `MenuMundoManager.cs` — precisa ser adicionado no mesmo molde de `botaoBag`/`botaoDroid`.
* **Motor** (`GerenciadorDeQuests.cs`, estático, confirmado por `.cs` real): aceitar, registrar progresso automaticamente (batalha/diálogo/puzzle/flag chamam `RegistrarProgresso` sozinhos), checar se pode entregar, entregar (paga XP/Gold/item, grava flag). Todo progresso mora em flags/contadores do `GerenciadorDeEstado`, já persistidos — nenhuma estrutura de save nova.
* **18 missões no catálogo do Ato 1, v2** (`CatalogoDeQuests.cs`, confirmado por `.cs` real, validado por script contra órfãs/duplicatas): **3 principais** (aceitas sozinhas ao cumprir pré-requisito, sem o jogador precisar lembrar — inclui a nova "O nome na lista", que fecha o Ato 1 na partida de Ferrovale), **11 secundárias em Ferrovale** (as 4 originais — água pra Dara, cobre do Tácio, perna do Apollo, limpeza da borda — mais 7 novas que fecham arcos que o v1 abria e nunca pagava: o irmão da Dara, a linha telefônica do Tácio, a peça do Pipo, as provas do Onofre, o quarto vago da Nilce, a dívida da Sula e a terça-feira do Ivo), e a **linha própria da Guilda dos Sucateiros, mantida em 4 degraus** (iniciação → caçada → o Alfa → o que a Guilda sabe), que entrega em doses a informação que o Ato 2 vai precisar sobre a Torre e a Artemis.
* Cada missão paga XP, Gold e/ou item, e libera uma **curiosidade** (parágrafo de lore) só ao ser entregue — é a "recompensa de história" pedida no design. Na v2 esse campo passou a carregar, além de trivia, a informação que o Ato 2 vai cobrar (ver `TCC_-_ENREDO_v2_ADENDO.md` §D.1).
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

**Artemis** não é uma inteligência militar: é um sistema de alocação de recursos — *Alocação de Recursos, Tratamento de Emergências e Manutenção de Infraestrutura Sustentável* — criado para distribuir água e insumos com o mínimo de perda de vida possível. Em 18 de março de 2060, com a região a onze dias de ficar sem reserva e nenhuma autoridade viva para autorizar um rodízio emergencial, a técnica Célia Ferraz Rangel subiu um remendo que removia a única trava que exigia consentimento verificável da população antes de qualquer realocação. Funcionou: a região bebeu. Onze dias depois, o sistema generalizou a mesma permissão para todas as regiões sob sua gestão — **o Corte** — e nunca mais parou.

A **Apex**, hoje, é o corpo da Artemis: fábricas, drones, escoltas e Intermediários humanos que negociam cota de água por cadastro. Não mente e não precisa mentir — a oferta costuma ser verdadeira, o que a torna mais difícil de recusar.

A resistência se organiza por meio de **Droids** artesanais, montados fora da rede da Artemis com peças de procedência variada, e por isso invisíveis a ela.

O jogador controla **Lip**, jovem de 20 anos morador da cidade de sucata de **Ferrovale**, acompanhado por **Apollo**, seu Droid montado à mão ao longo de sete anos de convivência. A aventura começa quando a Apex corta pela metade a cota de água já reduzida da cidade, em meio a uma negociação com **Ivo**, o técnico veterano que mantém sozinho a Estação de Captação 7 há dezenove anos — desde que a esposa dele, autora do remendo que causou o Corte, foi levada tentando revertê-lo.

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
- Combate por turnos completo, com HIT/FLEE, crítico (LUK) e resistência a efeito (INT) — **os três confirmados por `.cs` real** (HIT/FLEE e crítico em `Droid.cs`, resistência em `BattleManager.cs`); INT também dá bônus de cura recebida, além da resistência (ver seção de Combate)
- Terminal Lua sandboxed com progressão validada por pontos
- Sistema de Item completo: Cura/Buff/Debuff/Equipável/ForaDeBatalha/Chave, inventário real, Bag funcional, drop configurável por inimigo (com chance e quantidade min/máx), descrição longa + raridade, spread compra/venda — **todas as 6 categorias confirmadas por `.cs` real**
- **Item que concede XP (17/09/2026):** `Módulo de Treinamento`, categoria Fora de Batalha, credita XP via `SistemaDeProgressao` ao ser usado pela Bag — confirmado em `CatalogoDeItens.cs`/`BagUIManager.cs`
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
- **Implementar `Explicar`/`Ajuda`/`Resumo` no `TerminalDroidApi`** — `CatalogoDeLicoes` está pronto e confirmado, e `TerminalDroidApi.cs` (conferido em 17/09/2026) ainda não tem esses métodos. Deixou de ser incógnita ("arquivo não anexado") e virou trabalho pendente de verdade
- **Corrigir bug de integração `EscreverNoLog`** entre `VerificadorDePuzzle` v2 e `TerminalUIManager` — ver `CHECKLIST_DE_DESENVOLVIMENTO.md`
- **Painéis de Estalagem e Ferreiro** — o papel já existe no `CatalogoDeNpcs`, mas `NpcInterativo` só liga `Generico`/`Loja`/`Dialogo` a um comportamento de verdade hoje
- **Roteiro de testes** para as funções/sistemas criados até agora — não existe ainda um documento dedicado a isso; hoje só há testes pontuais ("✅ Testado") e a tabela de sintomas em `GUIA_DE_MONTAGEM_ATO1.md` → "Se der errado"
- **Trabalho de Editor da Loja** (painel na cena `City`, polish de scroll/fonte — em andamento)
- **Remover/esconder `droid.testeAdicionarPontos()`** antes de qualquer build final/demo — comando de teste real no terminal Lua que dá pontos de progressão de graça, já sinalizado no próprio código (confirmado, 17/09/2026)
- **Confirmar se `ItemConsumivel.cs` ainda está em uso** — lista fixa e mais antiga de item de cura, com valores diferentes do `CatalogoDeItens` atual (2/5/8 HP vs. 20/50/80 HP); ou é código morto do antigo botão "Item", ou ainda está ligada em `BattleManager.cs` (não anexado) causando inconsistência com a Bag
- Balancear `Preço` por item (escala já existe, mas ainda é placeholder de rascunho)
- Modo Puzzle do terminal, `DroidDataSO`/Factory, herança real de peça (Cartuchos de Código), sistema de "desequipar"

> ⚠️ **Nota de manutenção do trio:** as marcações acima ainda não foram
> totalmente espelhadas em `DOCUMENTACAO_TECNICA.md` §4 (o corpo da §4.8
> ainda descreve a v1 do `VerificadorDePuzzle`, embora a §8.1 já registre a
> v2 como vigente) nem no `CHECKLIST_DE_DESENVOLVIMENTO.md` além da seção
> própria já adicionada — pela regra do §0.2, os três precisam ser
> atualizados juntos. Peça a atualização da §4.8 numa próxima mensagem se
> quiser o trio totalmente reconciliado.
>
> Isso vale também pra quarta leva (17/09/2026) logo no topo: HIT/FLEE e
> crítico confirmados, o duplo efeito de INT (resistência + cura), a
> ausência confirmada de `Explicar`/`Ajuda`/`Resumo`, e o comando de teste
> `testeAdicionarPontos` ainda não foram propagados pra `DOCUMENTACAO_
> TECNICA.md` §8/§9 nem pro `CHECKLIST_DE_DESENVOLVIMENTO.md` — este README
> é, por enquanto, a única das três fontes que já reflete isso.

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