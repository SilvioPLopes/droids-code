# Desafios de programação

O jogo cadastra **dez puzzles** de terminal. Cada um vincula uma situação do mundo a um estado que o programa precisa produzir. Esta página descreve os desafios; o tutorial da linguagem ficará em construção, conforme o recorte atual da wiki.

**Disponibilidade:** cadastro é diferente de montagem na cena. A casa inicial tem contrato próprio para o puzzle da água. Os demais registros podem exigir integração ou percurso ainda em desenvolvimento.

## Catálogo

| Desafio | Conceito trabalhado |
|---|---|
| O gerador de água | Variável e laço (while) |
| Painel de energia da Cratera | Condicional (if) + laço |
| Balança da Guilda | Duas variáveis no mesmo laço (acumulador e contador) |
| Purga do filtro | Laço decrescente e condição de parada |
| Tranca de três estágios | Sequência: a ordem das operações muda o resultado |
| Calibragem do braço | Chamar função e usar o valor que ela devolve |
| Janela da patrulha | Condição composta (and) e decisão |
| Bomba de reserva | Condição de parada: por que um laço termina |
| Triagem do ferro-velho | Decisão dentro do laço (classificar enquanto percorre) |
| Sintonia da antena | Dois laços em sequência (subir e depois afinar) |


## O gerador de água

**ID:** `p_agua_casa` · **Aprendizagem:** Variável e laço (while).

O valor não muda sozinho. Faz ele subir de dez em dez até encher — cem.

**Condições cadastradas:**

- `nivel_agua`: maior ou igual a `100`


## Painel de energia da Cratera

**ID:** `p_painel_energia` · **Aprendizagem:** Condicional (if) + laço.

Carrega a energia até cinquenta e SÓ ENTÃO marca porta_aberta = 1. Se marcar antes, o painel rejeita.

**Condições cadastradas:**

- `energia`: maior ou igual a `50`
- `porta_aberta`: igual a `1`


## Balança da Guilda

**ID:** `p_contador_sucata` · **Aprendizagem:** Duas variáveis no mesmo laço (acumulador e contador).

Cada peça pesa 7. Empilha até o peso total passar de 70, e me diz em quantas peças deu.

**Condições cadastradas:**

- `total`: maior que `70`
- `quantas`: igual a `11`


## Purga do filtro

**ID:** `p_filtro_agua` · **Aprendizagem:** Laço decrescente e condição de parada.

O filtro tem 60 de resíduo. Cada purga tira 8. Purga até sobrar menos que 8 e marca pronto = 1.

**Condições cadastradas:**

- `residuo`: Menor `8`
- `pronto`: igual a `1`


## Tranca de três estágios

**ID:** `p_sequencia_porta` · **Aprendizagem:** Sequência: a ordem das operações muda o resultado.

A tranca tem três estágios. Cada um multiplica o código por 2, depois soma 3. Começa em 1 e roda os três. O resultado tem que ser 29.

**Condições cadastradas:**

- `codigo`: igual a `29`
- `etapas`: igual a `3`


## Calibragem do braço

**ID:** `p_calibragem_braco` · **Aprendizagem:** Chamar função e usar o valor que ela devolve.

Lê minha força com droid.obterAtributo("For") e guarda em forca_lida. Depois guarda o dobro dela em previsao.

**Condições cadastradas:**

- `forca_lida`: maior que `0`
- `previsao`: maior que `0`


## Janela da patrulha

**ID:** `p_rota_patrulha` · **Aprendizagem:** Condição composta (and) e decisão.

A patrulha passa a cada 12 minutos e leva 4 pra sumir de vista. Avança o relógio até dar 24 e, se o relógio for múltiplo de 12 E o tempo de espera for pelo menos 4, marca seguro = 1.

**Condições cadastradas:**

- `relogio`: igual a `24`
- `seguro`: igual a `1`


## Bomba de reserva

**ID:** `p_bomba_reserva` · **Aprendizagem:** Condição de parada: por que um laço termina.

A bomba enche o tanque de 15 em 15 até 90. Mas ela só liga se pressao for exatamente 90 — nem mais, nem menos.

**Condições cadastradas:**

- `pressao`: igual a `90`


## Triagem do ferro-velho

**ID:** `p_triagem_pecas` · **Aprendizagem:** Decisão dentro do laço (classificar enquanto percorre).

Vinte peças numeradas de 1 a 20. Peça de número par vai pra reaproveitar, ímpar vai pra fundir. Conta quantas de cada.

**Condições cadastradas:**

- `reaproveitar`: igual a `10`
- `fundir`: igual a `10`
- `peca`: maior que `20`


## Sintonia da antena

**ID:** `p_antena_guilda` · **Aprendizagem:** Dois laços em sequência (subir e depois afinar).

Sobe a frequência de 100 em 100 até passar de 800. Depois desce de 25 em 25 até ficar em 800 ou menos. E conta quantos ajustes ao todo.

**Condições cadastradas:**

- `freq`: igual a `800`
- `ajustes`: igual a `13`


## O que o verificador observa

O verificador consulta os valores finais das variáveis do programa. Não analisa a intenção do jogador nem prova o uso de um comando específico. Isso permite mais de uma solução, mas também significa que alcançar os números esperados não comprova domínio da estrutura proposta. A avaliação do professor pode explorar essa diferença.

Para executar a primeira solução e acompanhar a liberação da porta, use o [Guia do professor](https://github.com/SilvioPLopes/droids-code/wiki/Guia-do-professor).

Fonte: [arquivo do projeto](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDePuzzles.cs). Consulte também [Fontes e critérios de leitura](https://github.com/SilvioPLopes/droids-code/wiki/Fontes-e-criterios).
