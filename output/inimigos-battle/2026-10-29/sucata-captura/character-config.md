# Sucata de Captura — piloto visual v1

- ID de catálogo correspondente: `sucata_captura`.
- Arquivo: `sprite-conceito-v1.png`, uma pose estática de corpo inteiro, três quartos voltada à esquerda, transparência real; dimensões/amostras de alpha em `verificacao.json`.
- Estado: proposta visual, não importada/integrada ao Unity e não aprovada pelo usuário. Não é spritesheet animado.
- Atualização posterior: usuário aprovou a proposta e autorizou integração na Battle, com variante B. Cópia para integração em `Assets/sprites/Entities/Enemies/Battle/SucataCaptura-v1.png`; detalhes do resultado e limites no plano de Battle. A descrição de estado acima é o checkpoint do piloto, não o estado final da integração.
- Método: geração de imagem nativa (ImageGen), sem recortar ou compor peças LPC. Prompt integral em `prompt.txt`. Nenhuma configuração LPC ou animação foi produzida.

## Característica → origem → representação

| Característica | Origem | Representação nesta proposta |
|---|---|---|
| Magra, rápida, frágil | Enredo v2,Sessões3.4/4.1; CatalogoDeInimigos | Corpo estreito, espaço negativo entre membros, juntas/pistões expostos |
| Garras no lugar das mãos | Mesmas fontes | Duas pinças mecânicas abertas |
| Imobilizar, não matar | Enredo v2,Sessão3.4 | Garras de preensão sem lâminas/armas de fogo |
| Placas oxidadas, metal cinza/latão | Proposta de design; coerência com battle.jpg | Chapas remendadas, ferrugem contida |
| Uma lente âmbar/vermelha e cabeça mecânica | Proposta de design, não descrição canônica específica | Sensor único, sem rosto humano |
| Forma dos pés, antena, cabos e assimetria | Proposta de design | Estrutura industrial leve e reconhecível |

## Fontes, autoria e limites

Referências visuais do workspace: `Assets/sprites/Mapas/battle.jpg` (paleta/cenário, não incluído no fundo) e `Assets/sprites/Entities/Characters/Companion/Apollo/sprite.png` (linguagem de sprite/escala, sem reutilizar peças). Arte final gerada por ImageGen. Nenhum arquivo de imagem LPC foi incorporado como camada por compositor; esta entrega não é uma exportação licenciada do gerador LPC e não deve ser catalogada assim.

Fontes oficiais consultadas para avaliar o método modular:

- https://github.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator
- https://raw.githubusercontent.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator/master/sheet_definitions/body/body.json
- https://github.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator/tree/master/sheet_definitions/body/special

As bases padrão consultadas são humanoides; especiais listadas incluem skeleton/zombie. Consulta não é inventário exaustivo de todos os acessórios. A ficha Apollo local já registra armadura humana como placeholder robótico. O método de rastreabilidade/organização serve para inimigos; o corpo final pode exigir arte própria.

Este é um piloto orientado aos trechos do inimigo, não uma execução completa do prompt de elenco. Não foi feita leitura integral/revisão de todo o enredo nesta rodada, nem levantamento de todos os personagens.

## Pendências antes de produção

O prompt pediu lógica de64×64 ampliada, mas a geração entrega imagem maior: não tratar automaticamente como sprite nativa64×64 nem como grade pixel-perfect verificada. Definir tamanho final e pivot após aprovação; conferir nitidez no tamanho efetivo da Battle e harmonia com o Apollo. Nenhuma operação de redução/edição por código foi aplicada nesta proposta. Faltam poses/animações idle/ataque/dano e demais direções, se necessárias ao consumidor. Preservar a candidata anterior caso nova versão seja solicitada. Não substituir os dois objetos atuais de inimigo ou seus sprites sem escopo de integração próprio.
