# Itens e equipamentos

O inventário de Droids Code reúne reparos, componentes de combate, peças instaláveis e objetos ligados à vida de Ferrovale. O catálogo atual contém **27 itens**. As chamadas poções representam gel, solda e manutenção de máquinas dentro da ficção.

**Como ler:** valores abaixo são os cadastros atuais, sujeitos a balanceamento. Um item cadastrado não está necessariamente disponível em toda loja ou acessível no percurso da demo. Gold é a unidade econômica do sistema; no mundo narrativo, representa lastro de sucata.

Veja também [Sistemas do jogo](https://github.com/SilvioPLopes/droids-code/wiki/Sistemas-do-jogo), [Missões](https://github.com/SilvioPLopes/droids-code/wiki/Missoes) e [Galeria de sprites](https://github.com/SilvioPLopes/droids-code/wiki/Galeria).

## Consulta rápida

| Item | Categoria | Preço cadastrado | Disponibilidade |
|---|---|---:|---|
| Poção Pequena | Reparo / cura | 8 | Consultar estoque |
| Poção Média | Reparo / cura | 22 | Consultar estoque |
| Poção Grande | Reparo / cura | 55 | Consultar estoque |
| Reparo Completo | Reparo / cura | 150 | Consultar estoque |
| Escudo de Emergência | Melhoria temporária | 30 | Consultar estoque |
| Óleo de Precisão | Melhoria temporária | 18 | Consultar estoque |
| Injetor de Sobrecarga | Melhoria temporária | 35 | Consultar estoque |
| Granada de Ácido | Efeito no alvo | 25 | Consultar estoque |
| Disruptor EMP | Efeito no alvo | 40 | Consultar estoque |
| Corrosivo | Efeito no alvo | 28 | Consultar estoque |
| Núcleo de Energia | Equipamento | 60 | Consultar estoque |
| Servo-Motor | Equipamento | 60 | Consultar estoque |
| Sensor Óptico | Equipamento | 70 | Consultar estoque |
| Placa de Blindagem | Equipamento | 65 | Consultar estoque |
| Giroscópio | Equipamento | 75 | Consultar estoque |
| Sinalizador de Retorno | Utilitário | 45 | Consultar estoque |
| Kit de Acampamento | Utilitário | 50 | Consultar estoque |
| Chave de Acesso | Utilitário | 90 | Fora da vitrine comum |
| Módulo de Treinamento | Utilitário | 120 | Consultar estoque |
| Núcleo de Sobrecarga | Equipamento | 0 | Fora da vitrine comum |
| Módulo de Ressonância | Efeito no alvo | 0 | Fora da vitrine comum |
| Fio de Cobre | Missão / chave | 0 | Fora da vitrine comum |
| Filtro de Água | Missão / chave | 0 | Fora da vitrine comum |
| Sucata Limpa | Missão / chave | 0 | Fora da vitrine comum |
| Emblema da Guilda | Missão / chave | 0 | Fora da vitrine comum |
| Bilhete do Ivo | Missão / chave | 0 | Fora da vitrine comum |
| Servo da Perna do Apollo | Missão / chave | 0 | Fora da vitrine comum |



## Catálogo detalhado


### Poção Pequena

**ID:** `pocao_pequena` · **Categoria:** Reparo / cura · **Preço cadastrado:** 8 Gold.

Um tubo de gel reparador raspado de kits médicos pré-2060. Sabe a metal, mas fecha junta.

**Reparo:** 20 HP, limitado ao HP máximo.


### Poção Média

**ID:** `pocao_media` · **Categoria:** Reparo / cura · **Preço cadastrado:** 22 Gold.

Dose dupla, selada a quente. O Tácio jura que é do estoque antigo do hospital. Ninguém checa.

**Reparo:** 50 HP, limitado ao HP máximo.


### Poção Grande

**ID:** `pocao_grande` · **Categoria:** Reparo / cura · **Preço cadastrado:** 55 Gold.

Cartucho industrial. Pesado demais pra carregar muitos, bom demais pra deixar pra trás.

**Reparo:** 80 HP, limitado ao HP máximo.


### Reparo Completo

**ID:** `reparo_completo` · **Categoria:** Reparo / cura · **Preço cadastrado:** 150 Gold.

Nanorreparo de verdade, dos que a Apex usa nas próprias unidades. Ninguém em Ferrovale sabe como um foi parar aqui.

**Reparo:** 9999 HP, limitado ao HP máximo. O valor alto representa recuperação integral na prática.


### Escudo de Emergência

**ID:** `escudo_de_emergencia` · **Categoria:** Melhoria temporária · **Preço cadastrado:** 30 Gold.

Campo de contenção improvisado. Aguenta três turnos antes de derreter o próprio emissor. +10 VIT.

**Contexto:** uso de combate; não é utilizável na exploração.


### Óleo de Precisão

**ID:** `oleo_de_precisao` · **Categoria:** Melhoria temporária · **Preço cadastrado:** 18 Gold.

Lubrifica os servos de mira do braço. +15 DEX por 4 turnos — na prática, você para de errar tanto.

**Contexto:** uso de combate; não é utilizável na exploração.


### Injetor de Sobrecarga

**ID:** `sobrecarga_temp` · **Categoria:** Melhoria temporária · **Preço cadastrado:** 35 Gold.

Despeja energia direto no atuador. +8 FOR por 3 turnos. O Apollo reclama depois.

**Contexto:** uso de combate; não é utilizável na exploração.


### Granada de Ácido

**ID:** `granada_de_acido` · **Categoria:** Efeito no alvo · **Preço cadastrado:** 25 Gold.

Come a blindagem do alvo. -5 VIT por 3 turnos, o que significa mais dano em todo golpe seu.

**Contexto:** uso de combate; não é utilizável na exploração.


### Disruptor EMP

**ID:** `disruptor_emp` · **Categoria:** Efeito no alvo · **Preço cadastrado:** 40 Gold.

Pulso curto. O alvo perde um turno inteiro se reorganizando.

**Contexto:** uso de combate; não é utilizável na exploração.


### Corrosivo

**ID:** `corrosivo` · **Categoria:** Efeito no alvo · **Preço cadastrado:** 28 Gold.

Frasco de resíduo da Cratera. 5 de dano por turno, por 3 turnos.

**Contexto:** uso de combate; não é utilizável na exploração.


### Núcleo de Energia

**ID:** `nucleo_de_energia` · **Categoria:** Equipamento · **Preço cadastrado:** 60 Gold.

Célula de carga reaproveitada. Instala no tronco e alimenta os atuadores com mais folga. +5 FOR.

**Peça:** Tronco · **Bônus:** +5 FOR.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Servo-Motor

**ID:** `servo_motor` · **Categoria:** Equipamento · **Preço cadastrado:** 60 Gold.

Resolve o mancar do Apollo — pelo menos por um tempo. +5 AGI, ou seja, esquiva mais.

**Peça:** Perna · **Bônus:** +5 AGI.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Sensor Óptico

**ID:** `sensor_optico` · **Categoria:** Equipamento · **Preço cadastrado:** 70 Gold.

Lente de drone de reconhecimento, com a marca da Apex lixada por cima. +6 DEX — você erra muito menos.

**Peça:** Cabeca · **Bônus:** +6 DEX.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Placa de Blindagem

**ID:** `placa_de_blindagem` · **Categoria:** Equipamento · **Preço cadastrado:** 65 Gold.

Chapa de container cortada na medida. Feia, pesada, funciona. +6 VIT (defesa e HP máximo).

**Peça:** Tronco · **Bônus:** +6 VIT.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Giroscópio

**ID:** `giroscopio` · **Categoria:** Equipamento · **Preço cadastrado:** 75 Gold.

Estabiliza o braço no instante do impacto. +6 LUK — mais chance de crítico.

**Peça:** Braco · **Bônus:** +6 LUK.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Sinalizador de Retorno

**ID:** `sinalizador_de_retorno` · **Categoria:** Utilitário · **Preço cadastrado:** 45 Gold.

Queima e chama um carrinho de sucata. Na prática: volta pro último ponto salvo — inclusive desfazendo o que você fez desde então.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Kit de Acampamento

**ID:** `kit_de_acampamento` · **Categoria:** Utilitário · **Preço cadastrado:** 50 Gold.

Lona, soldador e um pouco de paciência. Recupera todo o HP e salva o jogo na hora.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Chave de Acesso

**ID:** `chave_de_acesso` · **Categoria:** Utilitário · **Preço cadastrado:** 90 Gold.

Cartão de manutenção antigo. Abre portas que não deveriam mais abrir.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Módulo de Treinamento

**ID:** `modulo_de_treinamento` · **Categoria:** Utilitário · **Preço cadastrado:** 120 Gold.

Simulador de combate remontado com sucata da Apex. Roda sozinho, sem risco -- só cansa. Concede experiência como se fosse uma batalha de verdade.

**Experiência concedida:** 100 XP.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Núcleo de Sobrecarga

**ID:** `nucleo_sobrecarga` · **Categoria:** Equipamento · **Preço cadastrado:** 0 Gold.

Presente associado à Rasha e à Flor de Ferro. Seu núcleo vermelho e acabamento áspero expressam a aposta da facção em força e intervenção direta.

**Peça:** Braco · **Bônus:** +10 FOR.

**Contexto:** uso fora de batalha. Verifique a ação e a confirmação mostradas pelo jogo.


### Módulo de Ressonância

**ID:** `modulo_ressonancia` · **Categoria:** Efeito no alvo · **Preço cadastrado:** 0 Gold.

Do Teodoro. Disco liso, luz branca pulsando devagar. Reescreve o protocolo de combate do alvo em vez de danificar o hardware — ele para por 3 turnos. Você só tem um.

**Contexto:** uso de combate; não é utilizável na exploração.


### Fio de Cobre

**ID:** `fio_de_cobre` · **Categoria:** Missão / chave · **Preço cadastrado:** 0 Gold.

Em Ferrovale, cobre é moeda. Antes de virar moeda, era o que levava energia pros lugares.

**Uso:** objeto de missão/identificação, sem uso direto como consumível.


### Filtro de Água

**ID:** `filtro_de_agua` · **Categoria:** Missão / chave · **Preço cadastrado:** 0 Gold.

Cartucho de carvão prensado. Não deixa a água boa, só deixa ela possível.

**Uso:** objeto de missão/identificação, sem uso direto como consumível.


### Sucata Limpa

**ID:** `sucata_limpa` · **Categoria:** Missão / chave · **Preço cadastrado:** 0 Gold.

Metal sem corrosão, raro depois da Cratera. Vale mais que comida em certos dias.

**Uso:** objeto de missão/identificação, sem uso direto como consumível.


### Emblema da Guilda

**ID:** `emblema_da_guilda` · **Categoria:** Missão / chave · **Preço cadastrado:** 0 Gold.

Uma engrenagem torta com três dentes limados. Quem carrega isso em Ferrovale é reconhecido — pra bem e pra mal.

**Uso:** objeto de missão/identificação, sem uso direto como consumível.


### Bilhete do Ivo

**ID:** `bilhete_do_ivo` · **Categoria:** Missão / chave · **Preço cadastrado:** 0 Gold.

Registro escrito por Ivo com instruções de contingência para manter o abastecimento caso ele não retorne. Liga um objeto de missão à responsabilidade do técnico pela cidade.

**Uso:** objeto de missão/identificação, sem uso direto como consumível.


### Servo da Perna do Apollo

**ID:** `peca_perna_apollo` · **Categoria:** Missão / chave · **Preço cadastrado:** 0 Gold.

O servo emperrado que faz o Apollo mancar desde sempre. Sete anos esperando uma peça de reposição.

**Uso:** objeto de missão/identificação, sem uso direto como consumível.


## Aparência dos itens

<img src="https://raw.githubusercontent.com/wiki/SilvioPLopes/droids-code/images/itens-consumo.png" alt="Folha de itens de consumo" width="420">

<img src="https://raw.githubusercontent.com/wiki/SilvioPLopes/droids-code/images/itens-tecnologia.png" alt="Folha de itens tecnológicos" width="420">

<img src="https://raw.githubusercontent.com/wiki/SilvioPLopes/droids-code/images/itens-especiais.png" alt="Folha de itens especiais e de missão" width="420">

As folhas acima são artes existentes do projeto. As entradas do catálogo visual definem o recorte individual de cada item; a folha completa não é o ícone mostrado na interface.

Fonte: [arquivo do projeto](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeItens.cs). Consulte também [Fontes e critérios de leitura](https://github.com/SilvioPLopes/droids-code/wiki/Fontes-e-criterios).
