# 🤖 Droids Code

[![Unity](https://img.shields.io/badge/Unity-6%20(6000.x)-black?logo=unity)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23%209-blue?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Scripting](https://img.shields.io/badge/Terminal-Lua%20(MoonSharp)-orange?logo=lua)](https://www.moonsharp.org/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20WebGL-lightgrey)](#tecnologias)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-MVP%20em%20Desenvolvimento-orange)](#status-de-desenvolvimento-e-roteiro)

> **RPG educativo de combate por turnos focado no ensino de lógica de programação e paradigmas de desenvolvimento de software.**

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
* O dano é calculado inteiramente em C#, lendo dados já configurados pelo jogador — o Lua nunca participa do cálculo de combate.

### 📈 Progressão

* O Droid ganha XP ao vencer batalhas e sobe de nível segundo uma curva simples (Nível × 100 de XP necessário).
* Cada nível concedido gera pontos de progressão, gastos no terminal para investir em atributos ou aprender técnicas.

### 🧩 Customização e Herança de Peças

* Cada componente físico do robô (Cabeça, Tronco, Braços e Pernas) funciona como um módulo de código.
* O jogador insere **Cartuchos de Código** nos conectores do Droid para declarar herança e sobrescrever ou estender os métodos nativos da classe base `DroidBase.cs`.
* *(Fase 2 do design — ver [Status de Desenvolvimento](#status-de-desenvolvimento-e-roteiro): hoje as peças ainda não têm comportamento próprio.)*

### 🐞 Depuração Amigável (Apollo Debugger)

* Erros de compilação ou exceções em tempo de execução (*Runtime Exceptions*) não encerram o jogo nem penalizam punitivamente o jogador.
* O companheiro robótico **Apollo** atua como depurador interativo, destacando a linha exata da falha no terminal e apresentando dicas conceituais para auxílio na resolução do bug.

### 💾 Salvamento

* Progresso salvo localmente em JSON: stats do Droid, HP, nível/XP, pontos disponíveis, técnicas configuradas, posição no mapa, cena atual e flags de história.
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

- Droid com atributos base (FOR/AGI/VIT/INT/DEX/LUK), peças (sem comportamento próprio ainda) e defesa = VIT total
- Combate por turnos completo (jogador vs. inimigo fixo), com menu Lutar/Itens/Status/Fugir
- Botão "Item" na batalha abre uma lista fixa de itens (`ItensDeBatalha`: Poção Pequena +2, Poção Média +5, Poção Grande +8, Reparo Completo) em vez de curar sozinho
- Motor de efeitos por turno: Stun (pula o turno) e Envenenamento (dano por turno) aplicados de verdade em combate, tanto no Droid quanto no inimigo; buffs/debuffs de atributo entram no cálculo de `ObterTotal`
- Terminal Lua sandboxed, com métodos validados de progressão (`subirAtributo`, `aprenderTecnica`, `esquecerTecnica`, `obterAtributo`, `listarTecnicas`, `obterPontosDisponiveis`)
- Sistema de pontos de progressão e nível/XP
- Técnicas compostas com efeitos de atributo (ex: Stun) e dano por turno (ex: Envenenamento) — custo calculado, aplicação em combate ainda pendente
- Salvamento/carregamento em JSON (stats, técnicas, posição, cena, flags de história)
- Encontros aleatórios no mapa e menu principal

### 🔜 Roadmap (ainda não implementado)

- **Sistema de acerto/erro (HIT/FLEE)** e atributos derivados completos (ATK/DEF/HIT/FLEE) — hoje todo ataque sempre acerta
- **Modo Puzzle** do terminal (exercícios de lógica isolados, fora da configuração real do Droid)
- **`DroidDataSO`/Factory** — hoje o Droid é criado direto em código; a ideia é migrar para ScriptableObjects configuráveis no Inspector
- **Fase 2 de customização de peças** — herança real com Cartuchos de Código sobrescrevendo `DroidBase`
- Inventário e equipamento completos (a lista fixa de itens da batalha é um placeholder mínimo, não um inventário real)
- Bag/Droid/Opções no Menu do Mundo ainda são placeholders ("ainda não foi implementado")

### 🐞 Bugs conhecidos

Nenhum no momento.

---

## 🛠️ Tecnologias

- **Motor:** Unity 6 (6000.x), Scripting Backend Mono, API Compatibility Level .NET Standard 2.1
- **Linguagem:** C# 9 (assinaturas de projeto evitam sintaxe C# 10+)
- **Terminal do jogador:** Lua via [MoonSharp](https://www.moonsharp.org/), rodando em sandbox (`Preset_SoftSandbox`)
- **Plataformas-alvo:** Windows e WebGL

---

## 📄 Licença

Distribuído sob a licença MIT. Ver `LICENSE` para mais informações.
