# Missões de Ferrovale

**Spoilers de objetivos e recompensas.** O catálogo reúne **18 missões**: três principais, onze secundárias e quatro da Guilda. As fichas registram o conteúdo cadastrado; não afirmam que todas as condições, entregas e recompensas já tenham sido percorridas no jogo.

A linha de investigação acompanha [História](https://github.com/SilvioPLopes/droids-code/wiki/Historia); a Guilda possui um [guia próprio](https://github.com/SilvioPLopes/droids-code/wiki/Guilda-dos-Sucateiros). Não são reproduzidos diálogos de oferta ou entrega.

## Visão geral

| Missão | Linha | Personagem |
|---|---|---|
| A cota cortada | Principal | Apollo |
| A estação de captação | Principal | Apollo |
| O nome na lista | Principal | Apollo |
| Um filtro para a Dara | Secundária | Dara |
| A terceira parte | Secundária | Dara |
| Cobre é moeda | Secundária | Senhor Tácio |
| Dezoito quilômetros | Secundária | Senhor Tácio |
| A perna do Apollo | Secundária | Apollo |
| Limpeza da borda | Secundária | Mestra Bruna |
| A peça que o Pipo viu | Secundária | Pipo |
| O que tinha antes da Cratera | Secundária | Seu Onofre |
| O quarto que não está quebrado | Secundária | Nilce |
| Toda terça | Secundária | Ivo |
| O que a Sula vendeu | Secundária | Sula |
| Guilda I — Ferro velho e gente velha | Guilda | Mestra Bruna |
| Guilda II — O que anda na Cratera | Guilda | Mestra Bruna |
| Guilda III — O Alfa | Guilda | Mestra Bruna |
| Guilda IV — O que a Guilda sabe | Guilda | Mestra Bruna |


## Linha principal


### A cota cortada

**ID:** `q_main_agua` · **Personagem:** Apollo.

A fila do poço está maior que o normal e ninguém quer dizer por quê. Três pessoas vão te dar três versões incompatíveis. A verdade é o que sobra depois de ouvir as três.

**Objetivos cadastrados:**

- Falar com a Dara na fila do poço
- Falar com o homem da fila
- Falar com o Senhor Tácio

**Recompensas cadastradas:** 120 XP; 15 Gold.

**Disponibilidade técnica:** depende de `ato1_casa_resolvida`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### A estação de captação

**ID:** `q_main_estacao` · **Personagem:** Apollo.

O Ivo fechou a estação e trouxe a Apex até aqui. Atravesse a Cratera e resolva isso — de um jeito ou de outro.

**Objetivos cadastrados:**

- Decidir sobre Rasha e Teodoro
- Religar o painel de energia da Cratera
- Enfrentar o droid do Ivo

**Recompensas cadastradas:** 400 XP; 60 Gold.

**Disponibilidade técnica:** depende de `ato1_sessao3`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### O nome na lista

**ID:** `q_main_partida` · **Personagem:** Apollo.

A água voltou. Uma mulher de caderneta está no portão com uma oferta boa demais, e o preço dela tem nome e sobrenome.

**Objetivos cadastrados:**

- Ver a água voltar na praça
- Ouvir a proposta da Intermediária
- Ver como Ferrovale responde

**Recompensas cadastradas:** 500 XP; 1 × Kit de Acampamento.

**Disponibilidade técnica:** depende de `ato1_sessao6`. Essa chave representa um estado narrativo; não é um comando para o jogador.


## Linha secundária


### Um filtro para a Dara

**ID:** `q_agua_dara` · **Personagem:** Dara.

A Dara divide a cota com duas famílias do beco. Com a metade do que era, ela precisa filtrar o que sobrou — e o filtro dela rachou no inverno.

**Objetivos cadastrados:**

- Conseguir um Filtro de Água

**Recompensas cadastradas:** 90 XP; 2 × Poção Média.

**Disponibilidade técnica:** depende de `ato1_falou_dara`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### A terceira parte

**ID:** `q_dara_terceira` · **Personagem:** Dara.

A Dara guarda um terço da cota todo dia, há dois anos, e nunca disse pra quem. Talvez esteja na hora de alguém perguntar.

**Objetivos cadastrados:**

- Juntar 4 de Sucata Limpa pra consertar a cisterna do beco
- Perguntar pra quem é a terceira parte

**Recompensas cadastradas:** 160 XP; 1 × Reparo Completo.

**Disponibilidade técnica:** depende de `quest_dara_filtro_ok`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### Cobre é moeda

**ID:** `q_cobre_tacio` · **Personagem:** Senhor Tácio.

O Tácio compra fio de cobre por um preço melhor que o de qualquer outro na praça. Ele não explica por quê.

**Objetivos cadastrados:**

- Entregar 5 Fios de Cobre

**Recompensas cadastradas:** 80 XP; 70 Gold.

**Disponibilidade técnica:** depende de `ato1_falou_tacio`. Essa chave representa um estado narrativo; não é um comando para o jogador.

**Repetível no cadastro.** A repetição efetiva depende dos gatilhos e do controle da missão na versão entregue.


### Dezoito quilômetros

**ID:** `q_tacio_linha` · **Personagem:** Senhor Tácio.

Depois da terceira entrega, o Tácio para de fingir que é negócio e mostra o que está fazendo com o cobre.

**Objetivos cadastrados:**

- Trazer mais 10 Fios de Cobre
- Contar o rolo emendado com o terminal

**Recompensas cadastradas:** 220 XP; 40 Gold; 1 × Sensor Óptico.

**Disponibilidade técnica:** depende de `quest_tacio_cobre_ok`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### A perna do Apollo

**ID:** `q_perna_apollo` · **Personagem:** Apollo.

Sete anos mancando. O servo que caiu do droid do Ivo é o primeiro sobressalente compatível que aparece.

**Objetivos cadastrados:**

- Recuperar o servo da perna
- Juntar 3 de Sucata Limpa pro reparo
- Levar tudo pro Quirino

**Recompensas cadastradas:** 200 XP; 1 × Servo-Motor.

**Disponibilidade técnica:** depende de `ato1_sessao6`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### Limpeza da borda

**ID:** `q_limpeza_borda` · **Personagem:** Mestra Bruna.

Sucatas de captura continuam aparecendo perto da cerca. Enquanto estiverem ali, ninguém sai pra catar.

**Objetivos cadastrados:**

- Derrotar 3 Sucatas de Captura

**Recompensas cadastradas:** 150 XP; 45 Gold.

**Disponibilidade técnica:** depende de `ato1_sessao3`. Essa chave representa um estado narrativo; não é um comando para o jogador.

**Repetível no cadastro.** A repetição efetiva depende dos gatilhos e do controle da missão na versão entregue.


### A peça que o Pipo viu

**ID:** `q_pipo_peca` · **Personagem:** Pipo.

O Pipo jura que sabe onde tem uma peça boa. A peça não existe. O que ele sabe de verdade vale mais.

**Objetivos cadastrados:**

- Ir com o Pipo até a "peça boa"

**Recompensas cadastradas:** 60 XP; 1 × Sinalizador de Retorno.

**Disponibilidade técnica:** depende de `ato1_sessao2`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### O que tinha antes da Cratera

**ID:** `q_onofre_provas` · **Personagem:** Seu Onofre.

Seu Onofre conta a mesma história de jeitos diferentes, e ninguém acredita nele. Traga três provas e ele para de precisar ser acreditado.

**Objetivos cadastrados:**

- Encontrar as três marcas na Cratera

**Recompensas cadastradas:** 180 XP; 1 × Poção Grande.

**Disponibilidade técnica:** depende de `ato1_faccao_escolhida`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### O quarto que não está quebrado

**ID:** `q_nilce_quarto` · **Personagem:** Nilce.

A Nilce mantém um quarto sempre vago e diz que está quebrado. Não está.

**Objetivos cadastrados:**

- Religar a bomba de reserva da estalagem

**Recompensas cadastradas:** 140 XP; 25 Gold; 1 × Kit de Acampamento.

**Disponibilidade técnica:** depende de `ato1_sessao3`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### Toda terça

**ID:** `q_ivo_terca` · **Personagem:** Ivo.

A bomba dois pede ajuste toda terça. O Ivo não vai conseguir fazer isso pra sempre, e os dois sabem.

**Objetivos cadastrados:**

- Calibrar a bomba dois pelo terminal

**Recompensas cadastradas:** 130 XP; 35 Gold.

**Disponibilidade técnica:** depende de `ato1_sessao6`. Essa chave representa um estado narrativo; não é um comando para o jogador.

**Repetível no cadastro.** A repetição efetiva depende dos gatilhos e do controle da missão na versão entregue.


### O que a Sula vendeu

**ID:** `q_sula_divida` · **Personagem:** Sula.

A Intermediária sabia o número antes de ler em voz alta. Alguém contou. A Sula não vai negar se você perguntar direito.

**Objetivos cadastrados:**

- Perguntar pra Sula

**Recompensas cadastradas:** 170 XP; 2 × Disruptor EMP; 1 × Óleo de Precisão.

**Disponibilidade técnica:** depende de `ato1_nadir`. Essa chave representa um estado narrativo; não é um comando para o jogador.


## Linha guilda


### Guilda I — Ferro velho e gente velha

**ID:** `q_guilda_1_iniciacao` · **Personagem:** Mestra Bruna.

A Guilda dos Sucateiros não aceita quem não trouxe nada. Traga metal que preste.

**Objetivos cadastrados:**

- Entregar 5 de Sucata Limpa

**Recompensas cadastradas:** 120 XP; 30 Gold; 1 × Emblema da Guilda.

**Disponibilidade técnica:** depende de `ato1_sessao2`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### Guilda II — O que anda na Cratera

**ID:** `q_guilda_2_cacada` · **Personagem:** Mestra Bruna.

A Guilda quer saber por que os droids selvagens da Cratera estão descendo mais perto da cidade do que deveriam.

**Objetivos cadastrados:**

- Derrotar 5 Droids Selvagens
- Trazer 3 Fios de Cobre da Cratera

**Recompensas cadastradas:** 250 XP; 80 Gold; 2 × Granada de Ácido.

**Disponibilidade técnica:** depende de `guilda_membro`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### Guilda III — O Alfa

**ID:** `q_guilda_3_alfa` · **Personagem:** Mestra Bruna.

Alguma coisa está organizando os selvagens da Cratera. A Guilda quer aquilo desmontado.

**Objetivos cadastrados:**

- Derrotar o Selvagem Alfa
- Trazer o sensor óptico dele

**Recompensas cadastradas:** 350 XP; 120 Gold; 2 × Poção Grande; 1 × Giroscópio.

**Disponibilidade técnica:** depende de `guilda_cacada_ok`. Essa chave representa um estado narrativo; não é um comando para o jogador.


### Guilda IV — O que a Guilda sabe

**ID:** `q_guilda_4_verdade` · **Personagem:** Mestra Bruna.

A Mestra vai contar o que a Guilda sabe sobre a torre. Ela quer o Emblema de volta na mesa antes de falar.

**Objetivos cadastrados:**

- Ouvir o que a Mestra tem a dizer

**Recompensas cadastradas:** 200 XP; 1 × Chave de Acesso.

**Disponibilidade técnica:** depende de `guilda_alfa_ok`. Essa chave representa um estado narrativo; não é um comando para o jogador.


## Missões como construção do mundo

A água da Dara mostra que uma cota é dividida entre pessoas. O cobre do Tácio transforma material de coleta em vínculo afetivo. A perna do Apollo faz uma recompensa técnica ter significado para a relação dos protagonistas. A linha da Guilda liga combate e coleta à investigação da Cratera. Essas relações fazem parte da proposta narrativa; entregar uma missão exige também a montagem correspondente nas cenas.

Fonte: [arquivo do projeto](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeQuests.cs). Consulte também [Fontes e critérios de leitura](https://github.com/SilvioPLopes/droids-code/wiki/Fontes-e-criterios).
