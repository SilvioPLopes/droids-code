# Estado do projeto e leitura da entrega

Esta wiki reúne o universo narrativo completo disponível nas fontes e um recorte técnico do projeto. As duas coisas têm extensões diferentes. A tabela abaixo serve para orientar a leitura, não para substituir os planos de desenvolvimento.

## Quatro estados que não devem ser confundidos

| Estado | O que significa | Exemplo |
|---|---|---|
| Narrativa definida | Acontecimento ou personagem descrito no roteiro | Atos 2/3 e os finais de Artemis |
| Conteúdo cadastrado | Dados existem em um catálogo | Missão com objetivos, requisitos e recompensas |
| Lógica implementada | Há código que atende um comportamento | Verificador de puzzle, checkpoint de combate, ciclo do boss |
| Percurso validado | A sequência foi executada nas condições indicadas | Resolver, sair, voltar e carregar numa sessão identificada |

Uma ficha pode reunir os três primeiros estados sem comprovar o quarto. Da mesma maneira, a aprovação visual de uma tela precisa observar a tela; números corretos no arquivo não garantem enquadramento e legibilidade.

## O recorte apresentado

- O README orienta a execução local pelo **Unity Editor 6000.5.8f1**, começando em MainMenu.
- A CasaLip possui contrato de introdução, puzzle da água e liberação da porta.
- City reúne exploração, conversas e acesso aos menus e interiores montados.
- O projeto contém cena e sistemas de combate, inimigos, itens, equipamentos e salvamento.
- Os catálogos reúnem 16 NPCs, 19 locais, 18 missões, 27 itens, 8 inimigos e 10 puzzles no estado consultado.
- A campanha futura é descrita no enredo/adendo; a wiki não anuncia os Atos 2 e 3 como jogáveis completos.

## Limites que afetam a apresentação

| Tema | Limite da evidência |
|---|---|
| Saída de City | Exige história e técnica extra; disponibilidade de pontos antes da passagem precisa de confirmação pelo percurso normal |
| Missões secundárias | Um registro de missão não confirma todos os gatilhos, itens e entregas montados |
| Boss | Lógica, atributos, prefab e arte não certificam posicionamento, acesso normal ou aceite visual |
| NPCs | Aparição, serviço e rotina precisam de montagem além da biografia |
| Builds | Funcionamento no Editor não aprova Windows/WebGL |
| Campanha e finais | Desenho narrativo, não entrega integral certificada |
| Lua na wiki | Página deliberadamente em construção |

## Referências de execução

O [índice de trabalho](https://github.com/SilvioPLopes/droids-code/blob/main/docs/indice-trabalho.md) aponta para planos e contratos por assunto. O [roteiro técnico de validação](https://github.com/SilvioPLopes/droids-code/blob/main/docs/guias/validacao-jogo.md) oferece condições de teste, mas registros antigos não certificam automaticamente a versão atual.

Esta expansão da wiki foi feita por leitura de arquivos e conferência documental, sem executar Unity, gravar saves ou percorrer novamente o jogo. Seus resultados verificáveis são cobertura de dados, coerência de referências e publicação dos documentos.
