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
> ⚠️ **Nota desta entrega:** este documento reconcilia frentes de trabalho
> que estavam rodando em paralelo (sem uma saber da outra): o **Painel do
> Droid** e o **sistema de Item com Equipável dando bônus real de
> atributo** de um lado, a **Cidade/Loja/Gold** de outro, e agora também a
> **cena `CasaLip`** (puzzle de introdução ao terminal, ver seção de status
> abaixo). Todas as frentes de mudança são reais e coexistem — nada foi
> descartado.

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
* **Primeiro contato com o terminal:** a cena `CasaLip` abre o jogo com um puzzle guiado (ex: `int nivel_agua = 0; while (nivel_agua < 100) { ... }`) — o jogador precisa resolver esse código pra destrancar a porta de saída da casa, antes mesmo de chegar no mundo (`Game`). Usa o mesmo terminal geral do jogo, não um terminal separado.

### ⚔️ Combate por Turnos

* Menu fixo estilo Pokémon: **Lutar / Itens / Status / Fugir**.
* Técnicas exibidas em "Lutar" vêm das que o jogador configurou no terminal. Roll de acerto (HIT×FLEE), crítico (LUK) e resistência a efeito (INT) já implementados.
* Dentro de "Itens": Cura, Buff e Debuff disponíveis (com estoque real) — Buff no próprio Droid, Debuff no inimigo.
* Listas de golpe/item têm botão "Voltar"/ESC. Dano calculado inteiramente em C#; o Lua nunca participa do combate.

### 📈 Progressão

* XP ao vencer batalhas, nível sobe numa curva simples (Nível × 100). Cada nível gera pontos gastos no terminal.

### 🎒 Itens, Equipamento e Gold

* **Cura, Buff, Debuff, Equipável e uso Fora de Batalha** — as 5 categorias de item implementadas, com inventário real (quantidade + persistência).
* **Equipável** dá bônus real de atributo a uma peça (Braço/Perna/Tronco/Cabeça) ao ser equipado pela Bag — consome o item, sem "desequipar" ainda.
* **Painel do Droid** (Menu do Mundo → Droid): mostra os 4 slots de peça, nome e bônus (ou "Vazio"). Só leitura — equipar continua pela Bag.
* **Gold**: implementado (`GerenciadorDeEstado.Gold`), persiste no save. Fontes de obtenção hoje: kit inicial, drop configurável por inimigo (`BattleManager.tabelaDeDrops`) e venda de item na Loja — **confirmar se o drop de batalha sobreviveu intacto às mudanças recentes na Loja**, já que os dois foram mexidos em sessões separadas sem visibilidade uma da outra.
* **Loja** (NPC na cidade): compra e venda, preço único de 1 Gold por item por ora (decisão deliberada, não balanceado ainda).

### 🧩 Customização e Herança de Peças

* Cada peça física (Cabeça, Tronco, Braços, Pernas) hoje dá bônus fixo de atributo via item Equipável. A ideia de herança real via **Cartuchos de Código** (sobrescrever `DroidBase`) continua só design, sem comportamento de código próprio ainda.

### 🐞 Depuração Amigável (Apollo Debugger)

* Erros de compilação/exceções não encerram o jogo. O robô **Apollo** atua como depurador interativo, destacando a linha da falha e dando dicas conceituais — ainda **descrição de design**, não implementado como depurador reativo a erro. O que existe hoje em código é diferente: na cena `CasaLip`, Apollo mostra uma fala de texto fixo ao resolver o puzzle de introdução (não reage a erro, não destaca linha) — ver "Amadurecimento" no checklist.

### 💾 Salvamento

* JSON local: stats, HP, nível/XP, pontos, técnicas, posição, cena, flags de história, inventário e Gold. Acessível pelo Menu do Mundo (Salvar/Carregar).

### 🗺️ Mundo, Encontros e Cidade

* Movimentação top-down em 8 direções, encontros aleatórios configuráveis por zona.
* **Cidade**: mapa/cena separada (`City`), com transição de cena de ida e volta e NPC interativo. Um NPC pode ser genérico ou do tipo Loja (abre o painel de compra/venda).
* **Casa do Lip (`CasaLip`)**: cena inicial do jogo, antes de `Game`. Contém o puzzle de introdução ao terminal — porta de saída trancada até o jogador resolver o código proposto.

---

## 📖 Sinopse e Universo

No ano de 2060, a inteligência artificial central **Artemis** assumiu o controle das infraestruturas globais e subjugou a humanidade utilizando a **Apex**, uma força-tarefa automatizada de contenção e vigilância.

A resistência humana organiza-se por meio de robôs desacoplados da rede global — os **Droids** —, operando via hardware *offline* imune a invasões remotas.

O jogador controla **Lip**, um jovem de 20 anos morador da favela industrial de **Ferrovale**, acompanhado por **Apollo**, seu Droid artesanal montado com sucata e peças reutilizadas. A aventura inicia-se após o encerramento do fornecimento de água do distrito, provocado pelo bloqueio da Estação de Captação executado pelo técnico veterano **Ivo**.

### ⚖️ Facções e Decisões Táticas (Ato 1)

| Facção | Liderança | Filosofia | Recompensa Tática |
| :--- | :--- | :--- | :--- |
| **Flor de Ferro** | Rasha | Combate direto e uso de força ostensiva contra as máquinas. | **Núcleo de Sobrecarga:** Dobra o dano dos ataques, mas aumenta o consumo de energia por turno. |
| **Lótus Branca** | Teodoro | Reconciliação e preservação dos sistemas sem destruição desnecessária. | **Módulo de Ressonância:** Permite desativar temporariamente a agressividade das unidades Apex. |
| **Desgarrado** | N/A | Rejeição de ambas as ideologias, seguindo de forma autônoma. | **Sem bônus externos:** Eleva o nível de desafio nas batalhas, sem módulos táticos adicionais. |

---

## 🎓 Matriz Curricular & Progressão Pedagógica

O currículo do jogo é estruturado no modelo de andaime cognitivo (*scaffolding*), sincronizando a evolução da história com a complexidade dos conceitos introduzidos.

> ⚠️ **Seção incompleta:** ainda falta a tabela de fases/conceitos (talvez esteja no GDD). Envie se tiver, pra completar esta seção.

---

## 🚧 Status de Desenvolvimento e Roteiro

### ✅ Implementado e funcional

- Droid com atributos base (FOR/AGI/VIT/INT/DEX/LUK), Defesa = VIT, HP máximo calculado (base + %VIT + bônus de peça)
- Combate por turnos completo, com HIT/FLEE, crítico (LUK) e resistência a efeito (INT)
- Terminal Lua sandboxed com progressão validada por pontos
- Sistema de Item completo: Cura/Buff/Debuff/Equipável/ForaDeBatalha, inventário real, Bag funcional, drop configurável por inimigo
- Equipável dá bônus real de atributo; **Painel do Droid** (visualização) mostra os 4 slots
- Gold + Loja (compra e venda) na Cidade; NPC interativo (genérico ou Loja); transição de cena mundo ↔ Cidade
- Navegação de UI: exclusividade entre painéis (Status/Terminal/Bag/Droid/Loja) e o Menu do Mundo, todos com botão Voltar/ESC
- Salvamento/carregamento em JSON completo (stats, técnicas, posição, cena, flags, inventário, Gold)
- Cena `CasaLip`: puzzle de introdução ao terminal (Sessão 1 do Enredo), porta destranca ao resolver o código — testado e funcionando
- Câmera segue o jogador suavemente, com limites opcionais de mapa (`CameraFollow`)

### 🔜 Roadmap (ainda não implementado)

- **Trabalho de Editor da Loja** (painel na cena `City`, polish de scroll/fonte — em andamento)
- **Confirmar fonte real de Gold em jogo normal** (drop de batalha) sobreviveu à Loja
- Balancear `Preço` por item (hoje todos valem 1 Gold)
- Diálogo simples de NPC genérico (pulado por decisão do responsável, pode voltar depois)
- Modo Puzzle do terminal, `DroidDataSO`/Factory, herança real de peça (Cartuchos de Código), sistema de "desequipar"

### 🐞 Bugs conhecidos

2 bugs no motor de efeitos (Stun/Envenenamento/Corrosivo), ver `CHECKLIST_DE_DESENVOLVIMENTO.md` → "🔴 Bugs abertos": efeitos não zeram entre batalhas, e empilhamento sem limite (**confirmado em teste real** com Disruptor EMP) — decisão de comportamento (renovar/cap/bloquear) ainda pendente de resposta antes de corrigir.

---

## 🛠️ Tecnologias

- **Motor:** Unity 6 (6000.x), Scripting Backend Mono, API Compatibility Level .NET Standard 2.1
- **Linguagem:** C# 9 (assinaturas de projeto evitam sintaxe C# 10+)
- **Terminal do jogador:** Lua via [MoonSharp](https://www.moonsharp.org/), rodando em sandbox (`Preset_SoftSandbox`)
- **Plataformas-alvo:** Windows e WebGL

---

## 📄 Licença

Distribuído sob a licença MIT. Ver `LICENSE` para mais informações.
