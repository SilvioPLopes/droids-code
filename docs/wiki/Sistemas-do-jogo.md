# Sistemas do jogo

Droids Code combina exploração, leitura de pessoas e preparação de uma máquina. O terminal é parte do mundo: aprender a descrever uma mudança de estado permite resolver um problema e preparar Apollo para o que vem depois.

## O ciclo de jogo

1. **Explorar:** visitar lugares e encontrar personagens ou objetos.
2. **Observar:** reunir informações, ouvir versões e reconhecer condições.
3. **Preparar:** usar o terminal, administrar recursos e escolher equipamentos.
4. **Agir:** resolver um desafio, interagir com um evento ou lutar.
5. **Ler o resultado:** conferir mensagens, recompensas e mudanças do mundo.

Esse ciclo é a proposta do projeto. A extensão disponível depende da montagem de cada cena e do percurso da versão entregue.

## Exploração e conversas

Lip se move com WASD ou setas. A tecla E aciona interações próximas. Diálogos e menus bloqueiam temporariamente o movimento para concentrar a ação na interface. O Menu do Mundo, aberto por Esc na exploração, oferece acesso aos painéis do jogador.

Conversar pode reunir informação ou alterar um estado da história, mas não deve ser confundido com resolver qualquer puzzle associado ao local. Na CasaLip, introdução e conclusão do desafio da água são estados separados.

## Terminal e preparação de Apollo

O terminal permite executar o código que configura atributos e técnicas e produz os valores verificados pelos desafios. A Wiki reserva a explicação da linguagem para [Lua — em construção](https://github.com/SilvioPLopes/droids-code/wiki/Lua); aqui o foco é o papel desse sistema dentro do jogo.

Preparar uma técnica envolve custo em pontos. Usá-la em batalha envolve energia. A existência de uma assinatura de comando na ajuda não significa que qualquer jogador possa executar a alteração sem atender seus requisitos.

## Os seis atributos

| Atributo | Papel na implementação consultada |
|---|---|
| FOR | Contribui para o ataque de Apollo |
| VIT | Define defesa e acrescenta bônus ao HP máximo |
| AGI | Define a esquiva, chamada Flee |
| DEX | Define o acerto, chamado Hit |
| INT | Contribui para resistência a efeitos e bônus de cura |
| LUK | Contribui para a chance de crítico |

O total considera a construção do Droid, equipamentos e efeitos aplicáveis. Subir dano sem cuidar de acerto pode produzir ataques fortes que falham. A defesa do adversário também altera o resultado: o valor bruto de uma técnica não é dano garantido.

### Como acerto e defesa aparecem na estratégia

A chance de acerto usa uma base de 50 somada à diferença entre Hit do atacante e Flee do alvo, limitada entre 5% e 95%. Isso mantém possibilidade de erro mesmo em condições favoráveis. Os números são regras atuais, sujeitos a balanceamento.

O crítico é verificado depois do acerto e usa multiplicador próprio. HP, dano, probabilidade e efeitos são dimensões diferentes; interpretar uma falha como se fosse falta de ataque pode levar a uma preparação inadequada.

## Energia de combate

Apollo possui energia atual e máxima, com valor inicial 100/100 na configuração desse sistema. Técnicas consomem energia de acordo com sua composição; custos maiores acompanham ataques e efeitos mais fortes.

O **Ataque Básico custa zero e recupera 10 de energia**, até o máximo, mesmo quando erra. Abrir um painel, mudar de cena ou terminar uma batalha não é uma fonte automática de recuperação. Um turno bloqueado por atordoamento não executa a ação do participante.

Pontos do terminal, energia de combate e HP não são o mesmo recurso. Essa distinção é importante ao preparar Apollo e interpretar a interface.

## Combate por turnos

A batalha usa uma cena própria. O jogador escolhe uma ação, o sistema valida seu contexto e resolve acerto, dano e efeitos. O adversário pode ter uma ação simples ou comportamento específico, como o ciclo anunciado do [Droid do Ivo](https://github.com/SilvioPLopes/droids-code/wiki/Boss-Droid-do-Ivo).

Itens permitidos em batalha podem ocupar o turno. Itens restritos devem informar o impedimento sem produzir consumo ou efeito válido. Técnicas, buffs, debuffs e efeitos por turno ampliam o planejamento para além de dano imediato.

Vitória pode conceder XP, Gold e drops; fuga não concede a recompensa de vitória. Regras especiais de inimigos únicos ou chefes precisam ser observadas no fluxo implementado, sem presumir que um campo de catálogo valide todas as restrições.

## Inventário, equipamentos e loja

A Bag organiza os itens e seus detalhes. Equipamentos acrescentam bônus por slot e podem ser substituídos; consumíveis têm contexto e efeito próprios. O catálogo visual relaciona cada ID à sua aparência, sem alterar a regra do objeto.

O inventário também guarda coisas que não existem para serem usadas em um turno: materiais de entrega, emblemas e registros. A função de missão não é equivalente a um consumível que cura ou causa dano. Consulte o [catálogo de itens](https://github.com/SilvioPLopes/droids-code/wiki/Itens).

O preço cadastrado não certifica o estoque de uma loja. Recompensas de facção e objetos de história podem estar fora da vitrine comum mesmo tendo uma entrada no catálogo.

## Missões e estados de história

O catálogo aceita objetivos de conversa, derrota de inimigos, coleta, resolução de puzzle e alcance de uma condição narrativa. Esses objetivos usam IDs estáveis para relacionar sistemas.

Os estados da história também condicionam diálogos e passagens. Um catálogo pode conter uma missão pronta em dados e ainda precisar de um personagem, gatilho ou entrega montados para completar o percurso. Veja [Missões](https://github.com/SilvioPLopes/droids-code/wiki/Missoes) e [Estado do projeto](https://github.com/SilvioPLopes/droids-code/wiki/Estado-do-projeto).

## Salvar e continuar

O estado persistente inclui progresso, inventário, atributos e dados de equipamentos conforme o formato atual. A entrada de combate usa um checkpoint do mundo anterior à luta; não é um salvamento de turno para retomar uma batalha exatamente no mesmo instante.

Os efeitos de itens utilitários precisam ser lidos antes da confirmação: o Sinalizador de Retorno, por exemplo, está descrito como retorno ao ponto salvo, com implicações para o progresso desde esse ponto. Não trate todos os consumíveis como mudanças apenas visuais.

Fontes: [Droid.cs](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Droid/Droid.cs), [TabelaDeCombate](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Scripting/TabelaDeCombate.cs), [contrato de combate](https://github.com/SilvioPLopes/droids-code/blob/main/docs/contextos/sistemas/combate.md) e [estado/save](https://github.com/SilvioPLopes/droids-code/blob/main/docs/contextos/sistemas/estado-e-save.md).
