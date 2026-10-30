# Bestiário

**Spoilers de inimigos e recompensas.** O catálogo possui oito registros, incluindo um inimigo de teste. Um registro descreve atributos e recompensas; não garante encontro disponível em qualquer região.

Para estratégia geral, consulte [Sistemas do jogo](https://github.com/SilvioPLopes/droids-code/wiki/Sistemas-do-jogo). O [Droid Improvisado de Ivo](https://github.com/SilvioPLopes/droids-code/wiki/Boss-Droid-do-Ivo) possui uma página de chefe.

## Atributos cadastrados

| Inimigo | HP | ATK | DEF | Acerto (Hit) | Esquiva (Flee) | XP | Gold |
|---|---:|---:|---:|---:|---:|---:|---|
| Sucata de Captura | 24 | 4 | 1 | 6 | 2 | 45 | 3–7 |
| Sucata Reforçada | 40 | 7 | 3 | 10 | 5 | 80 | 6–12 |
| Droid Selvagem | 38 | 8 | 2 | 12 | 4 | 75 | 5–11 |
| Selvagem Alfa | 60 | 12 | 4 | 16 | 8 | 160 | 15–28 |
| Carcaça Errante | 55 | 6 | 8 | 8 | 1 | 90 | 8–14 |
| Drone de Reconhecimento | 26 | 9 | 1 | 18 | 14 | 110 | 10–18 |
| Droid Improvisado | 200 | 18 | 10 | 22 | 4 | 300 | 20–40 |
| Esqueleto | 20 | 5 | 1 | padrão | padrão | 10 | 0–2 |


## Sucata de Captura

**ID:** `sucata_captura`.

Droid magro, rápido, com garras em vez de mãos. Projetado pra imobilizar, não pra matar. Isso, de alguma forma, é pior.

Unidade de contenção rápida e frágil. No cadastro, oferece uma introdução de menor resistência ao combate.

**Drops cadastrados:**

- Poção Pequena: 50%, quantidade 1
- Sucata Limpa: 60%, quantidade 1


## Sucata Reforçada

**ID:** `sucata_reforcada`.

Uma sucata de captura que sobreviveu tempo suficiente pra ser remendada pela própria Apex.

Versão remendada e mais resistente; possui mais vida, ataque e defesa que a Sucata de Captura.

**Drops cadastrados:**

- Sucata Limpa: 80%, quantidade 1–2
- Fio de Cobre: 40%, quantidade 1


## Droid Selvagem

**ID:** `droid_selvagem`.

Máquina desconectada de qualquer rede, que aprendeu sozinha a caçar energia pela Cratera. Os olhos brilham um azul instável, nada parecido com a calma dos do Apollo.

Máquina desconectada que busca energia. Ataque elevado em relação à defesa torna importante a preparação do Apollo.

**Drops cadastrados:**

- Poção Pequena: 45%, quantidade 1–2
- Fio de Cobre: 50%, quantidade 1
- Poção Média: 12%, quantidade 1


## Selvagem Alfa

**ID:** `droid_selvagem_alfa`.

Maior que os outros, montado com pedaços de três droids diferentes. Alguma coisa nele aprendeu a esperar antes de atacar.

A organização dos selvagens deixa de parecer comportamento espontâneo. A linha da Guilda investiga o significado desse comando.

**Drops cadastrados:**

- Sensor Óptico: 100%, quantidade 1
- Poção Média: 100%, quantidade 2


## Carcaça Errante

**ID:** `carcaca_errante`.

Um droid industrial que nunca parou de andar. Blindagem grossa, reflexos nenhum. Bater de frente é o jeito lento de vencer.

Blindagem alta e esquiva baixa. A descrição convida o jogador a pensar em redução de defesa, em vez de repetir ataques fracos.

**Drops cadastrados:**

- Sucata Limpa: 100%, quantidade 2–3
- Placa de Blindagem: 15%, quantidade 1


## Drone de Reconhecimento

**ID:** `drone_reconhecimento`.

Não vem sozinho, nunca. Se você está vendo um, alguém já sabe onde você está.

Acerto e esquiva altos no cadastro. Sua presença também é sinal de vigilância e circulação de informação para a Apex.

**Drops cadastrados:**

- Sensor Óptico: 25%, quantidade 1
- Fio de Cobre: 70%, quantidade 1–2


## Droid Improvisado

**ID:** `droid_do_ivo`.

Partes de um droid industrial antigo, soldadas com placas da própria estação. Não é um droid de guerra de verdade. É pior — é algo feito por desespero, sem nenhuma das salvaguardas que um projeto de combate teria.

Chefe associado a Ivo. Sua construção improvisada expressa contenção e desespero; o ciclo de ações é detalhado na página própria.

**Drops cadastrados:**

- Poção Média: 100%, quantidade 2
- Servo da Perna do Apollo: 100%, quantidade 1


## Esqueleto

**ID:** `esqueleto_teste`.

Inimigo de teste, anterior ao Ato 1.

Registro técnico anterior ao recorte do Ato 1; não é uma criatura canônica do mundo de Ferrovale.


## Encontros no vale

A configuração em disco do vale usa uma zona de encontro por deslocamento. A tabela atual atribui 15% por tentativa à Sucata de Captura, 5% à Sucata Reforçada e 80% a nenhum encontro. Entre as batalhas geradas por essa tabela, a proporção esperada é 3:1. São valores de configuração, sem certificação de distribuição por uma partida completa. Não há, nessa mecânica, um inimigo visível caminhando pelo mapa.

Fonte: [arquivo do projeto](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeInimigos.cs). Consulte também [Fontes e critérios de leitura](https://github.com/SilvioPLopes/droids-code/wiki/Fontes-e-criterios).
