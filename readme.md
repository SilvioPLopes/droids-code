# 🤖 Droids Code

[![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23-blue?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20WebGL-lightgrey)](#especificações-técnicas)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-MVP%20em%20Desenvolvimento-orange)](#status-de-desenvolvimento-e-roteiro)

> **RPG educativo de combate por turnos focado no ensino de lógica de programação e paradigmas de desenvolvimento de software.**

---

## 📌 Sobre o Projeto

O **Droids Code** é um jogo digital educativo do gênero RPG (*Role-Playing Game*) com visão pseudo-*top-down* e sistema de combate por turnos, desenvolvido para PC (Windows) e navegadores WebGL.

O projeto foi concebido como **Trabalho de Conclusão de Curso (TCC)** no curso Superior de Tecnologia em Sistemas para Internet na **FATEC Baixada Santista - Rubens Lara**.

### 🎯 Proposta Pedagógica
O principal objetivo do *Droids Code* é mitigar os altos índices de evasão e retenção observados em disciplinas introdutórias de Algoritmos e Computação. Em vez de utilizar abstrações visuais isoladas (como blocos de encaixar), o jogo integra um **terminal de código integrado** que exige do jogador a escrita de comandos com sintaxe próxima ao **C#**, relacionando diretamente estruturas lógicas com mecânicas de jogo, atributos de robôs e tomada de decisão em tempo real.

---

## 🎮 Mecânicas Principais e Design de Jogo

### 💻 Terminal de Código e Batalha Integrada
* As opções de ações disponíveis no menu de combate do jogador são diretamente mapeadas a partir dos **métodos públicos** implementados ou equipados no objeto do Droid.
* Caso uma função de ataque, defesa ou diagnóstico contenha um erro sintático ou de lógica, a ação não estará disponível para uso durante o turno ou resultará em falha operacional.

### 🧩 Customização e Herança de Peças
* Cada componente físico do robô (Cabeça, Tronco, Braços e Pernas) funciona como um módulo de código.
* O jogador insere **Cartuchos de Código** nos conectores do Droid para declarar herança e sobrescrever ou estender os métodos nativos da classe base `DroidBase.cs`.

### 🐞 Depuração Amigável (Apollo Debugger)
* Erros de compilação ou exceções em tempo de execução (*Runtime Exceptions*) não encerram o jogo nem penalizam punitivamente o jogador.
* O companheiro robótico **Apollo** atua como depurador interativo, destacando a linha exata da falha no terminal e apresentando dicas conceituais para auxílio na resolução do bug.

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

O currículo do jogo é estruturado no modelo de andaime cognitivo (*scaffolding*), sincronizando a evolução da história com a complexidade dos conceitos introduzidos: