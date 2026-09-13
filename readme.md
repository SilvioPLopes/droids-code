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
* O jogador nunca edita o Droid diretamente por propriedade (ex: `droid.stats.for = 99` não existe) — só através de métodos validados que conferem se há pontos de progressão suficientes antes de aplicar qualquer mudança:
  - `droid.subirAtributo("For", quantidade)` — investe pontos em FOR/AGI/VIT/INT/DEX/LUK
  - `droid.aprenderTecnica("nome", nivelDeDano)` — cria uma nova técnica de combate
  - `droid.obterPontosDisponiveis()` — consulta o saldo de pontos
* O terminal mantém histórico de comandos (setas ↑/↓) e um log de sessão com sucesso/erro de cada execução.

### ⚔️ Combate por Turnos

* Menu fixo estilo Pokémon: **Lutar / Itens / Status / Fugir**.
* Dentro de "Lutar", a lista de técnicas exibidas é dinâmica — vem das técnicas que o próprio jogador configurou no terminal.
* Dentro de "Itens", a lista mostra Cura, Buff e Debuff disponíveis (com estoque real) — Buff aplica no próprio Droid, Debuff aplica no inimigo.
* As listas de golpes e de itens têm um botão "Voltar" (ou ESC) pra fechar o painel sem agir.
* O dano é calculado inteiramente em C#, lendo dados já configurados pelo jogador — o Lua nunca participa do cálculo de combate.

### 📈 Progressão

* O Droid ganha XP ao vencer batalhas e sobe de nível segundo uma curva simples (Nível × 100 de XP necessário).
* Cada nível concedido gera pontos de progressão, gastos no terminal para investir em atributos ou aprender técnicas.

### 🎒 Itens e Equipamento

* **Cura**: consumível clássico, com bônus percentual de INT na cura recebida. Usável em batalha e na Bag (fora de batalha).
* **Buff/Debuff**: itens que aplicam um efeito temporário (mesmo motor de Stun/Envenenamento das técnicas) — Buff no próprio Droid, Debuff no inimigo (com chance de resistência por INT). Só usáveis em batalha.
* **Equipável**: dá um bônus permanente de atributo a uma peça do Droid (Braço/Perna/Tronco/Cabeça) ao ser equipado pela Bag — equipar consome o item.
* **Uso fora de batalha**: Sinalizador de Retorno (volta ao último save), Kit de Acampamento (cura total + salva) e Chave de Acesso (destrava flags de história), todos usáveis pela Bag.
* **Obtenção**: kit inicial fixo + drop configurável por inimigo (item e/ou Gold) ao vencer uma batalha. Ainda não existe loja/compra.

### 🧩 Customização e Herança de Peças

* Cada componente físico do robô (Cabeça, Tronco, Braços e Pernas) funciona como um módulo de código.
* O jogador insere **Cartuchos de Código** nos conectores do Droid para declarar herança e sobrescrever ou estender os métodos nativos da classe base `DroidBase.cs`.
* *(Fase de herança real — ver [Status de Desenvolvimento](#status-de-desenvolvimento-e-roteiro): hoje as peças têm bônus fixo de atributo via itens Equipáveis, mas ainda não têm comportamento de código próprio.)*

### 🐞 Depuração Amigável (Apollo Debugger)

* Erros de compilação ou exceções em tempo de execução (*Runtime Exceptions*) não encerram o jogo nem penalizam punitivamente o jogador.
* O companheiro robótico **Apollo** atua como depurador interativo, destacando a linha exata da falha no terminal e apresentando dicas conceituais para auxílio na resolução do bug.

### 💾 Salvamento

* Progresso salvo localmente em JSON: stats do Droid, HP, nível/XP, pontos disponíveis, técnicas configuradas, posição no mapa, cena atual, flags de história, inventário e Gold.
* Acessível pelo Menu do Mundo (Salvar / Carregar).

### 🗺️ Mundo e Encontros

* Movimentação top-down em 8 direções.
* Zonas de encontro aleatório: a cada certa distância percorrida numa área marcada, há uma chance configurável de iniciar uma batalha.

---

## 📖 Sinopse e Universo

No ano de 2060, a inteligência artificial central **Artemis** assumiu o controle das infraestruturas globais e subjugou a humanidade utilizando a **Apex**, uma força-tarefa automatizada de contenção e vigilância.

A resistência humana organiza-se por meio de robôs desacoplados da rede global — os **Droids** —, operando via hardware *offline* imune a invasões remotas.

O jogador controla **Lip**, um jovem de 20 anos morador da favela industrial de **Ferrovale**, acompanhado por **Apollo**, seu Droid artesanal montado com sucata e peças reutilizadas. A aventura inicia-se após o encerramento do fornecimento de água do distrito, provocado pelo bloqueio da Estação de Captação executado pelo técnico veterano **Ivo**.

### ⚖️ Facções e Decisões Táticas (Ato 1)

Durante a travessia pela região da Cratera, o jogador interage com as duas vertentes ideológicas da resistência humana:

| Facção | Liderança | Filosofia | Recompensa Tática |
| :--- | :--- | :--- | :--- |
| **Flor de Ferro** | Rasha | Combate direto e uso de força ostensiva contra as máquinas. | **Núcleo de Sobrecarga:** Dobra o dano dos ataques, mas aumenta o consumo de energia por turno. |
| **Lótus Branca** | Teodoro | Reconciliação e preservação dos sistemas sem destruição desnecessária. | **Módulo de Ressonância:** Permite desativar temporariamente a agressividade das unidades Apex. |
| **Desgarrado** | N/A | Rejeição de ambas as ideologias, seguindo de forma autônoma. | **Sem bônus externos:** Eleva o nível de desafio nas batalhas, sem módulos táticos adicionais. |

---

## 🎓 Matriz Curricular & Progressão Pedagógica

O currículo do jogo é estruturado no modelo de andaime cognitivo (*scaffolding*), sincronizando a evolução da história com a complexidade dos conceitos introduzidos.

> ⚠️ **Seção incompleta:** a versão anterior deste README também cortava aqui, sem listar a tabela de fases/conceitos. Se você tiver esse conteúdo (talvez no GDD), me envie para eu completar esta seção.

---

## 🚧 Status de Desenvolvimento e Roteiro

### ✅ Implementado e funcional

- Droid com atributos base (FOR/AGI/VIT/INT/DEX/LUK) e defesa = VIT total
- Combate por turnos completo (jogador vs. inimigo fixo), com menu Lutar/Itens/Status/Fugir
- Navegação de UI: botão "Voltar"/ESC nos painéis de Ataque e Item da batalha; exclusividade entre os painéis de Status/Terminal/Bag e o Menu do Mundo
- Terminal Lua sandboxed, com métodos validados de progressão (`subirAtributo`, `aprenderTecnica`, `aprenderTecnicaComVeneno`, `aprenderTecnicaComStun`, `melhorarTecnica`, `esquecerTecnica`, `obterAtributo`, `listarTecnicas`, `obterPontosDisponiveis`)
- Sistema de pontos de progressão e nível/XP
- Técnicas compostas com efeitos de atributo (ex: Stun) e dano por turno (ex: Envenenamento), aplicação real em combate
- **Atributos derivados (12/09/2026):** VIT (HP% e Defesa), AGI→FLEE, DEX→HIT, LUK→crítico, INT→resistência a efeito e bônus de cura — ver `CHECKLIST_DE_DESENVOLVIMENTO.md` para os números exatos
- **Sistema de Item completo (fase 1 em 13/09/2026, fase 2 concluída na sessão seguinte):**
  - Inventário real (quantidade + persistência), Bag funcional fora de batalha
  - **Cura, Buff, Debuff, Equipável e uso Fora de Batalha** — todas as 5 categorias de `TipoDeItem` implementadas
  - Drop de item/Gold configurável por inimigo ao vencer uma batalha (ver `BattleManager`)
  - Equipar peça (Braço/Perna/Tronco/Cabeça) dá bônus real de atributo — consome o item, sem desequipar nesta fase; **duplicação de bônus ao equipar 2x ainda não foi testada** (ver checklist, seção "Pendência de teste")
  - Layout da lista de itens da Bag corrigido (Vertical Layout Group + Content Size Fitter) — mesmo ajuste ainda pendente nos painéis de Ataque/Item da cena Battle
  - **Painel do Droid (novo):** botão "Droid" do Menu do Mundo deixou de ser placeholder — abre um painel somente-leitura mostrando os 4 slots (Braço/Perna/Tronco/Cabeça), nome da peça e bônus de atributo (ou "Vazio" se o slot não tiver peça). Equipar continua acontecendo só pela Bag; sem "desequipar" nesta leva. Código pronto (`TelaDeDroidManager.cs` + patch em `MenuMundoManager.cs`); falta só criar o Prefab `PainelDroid` no Editor.
  - Ainda falta: loja/compra (só drop + kit inicial por ora)
- Salvamento/carregamento em JSON (stats, técnicas, posição, cena, flags de história, inventário, Gold)
- Encontros aleatórios no mapa e menu principal

### 🔜 Roadmap (ainda não implementado)

- **Modo Puzzle** do terminal (exercícios de lógica isolados, fora da configuração real do Droid)
- **`DroidDataSO`/Factory** — hoje o Droid é criado direto em código; a ideia é migrar para ScriptableObjects configuráveis no Inspector
- **Fase de herança real de peças** — Cartuchos de Código sobrescrevendo `DroidBase` (peças hoje só têm bônus fixo de atributo via item Equipável, não comportamento de código)
- **Loja/compra de item** — hoje só existe kit inicial + drop de inimigo
- **Sistema de "desequipar"** — equipar consome o item permanentemente nesta fase, sem devolver/trocar

### 🐞 Bugs conhecidos

Ver `CHECKLIST_DE_DESENVOLVIMENTO.md` → seção "🔴 Bugs abertos": 2 itens confirmados no código, ambos sobre o motor de efeitos (Stun/Envenenamento/Corrosivo): efeitos ativos do Droid não são zerados entre batalhas, e não há limite/renovação para instâncias empilhadas do mesmo efeito. O item Debuff "Corrosivo" (novo) herda esses mesmos bugs, por usar o mesmo motor.

---

## 🛠️ Tecnologias

- **Motor:** Unity 6 (6000.x), Scripting Backend Mono, API Compatibility Level .NET Standard 2.1
- **Linguagem:** C# 9 (assinaturas de projeto evitam sintaxe C# 10+)
- **Terminal do jogador:** Lua via [MoonSharp](https://www.moonsharp.org/), rodando em sandbox (`Preset_SoftSandbox`)
- **Plataformas-alvo:** Windows e WebGL

---

## 📄 Licença

Distribuído sob a licença MIT. Ver `LICENSE` para mais informações.
