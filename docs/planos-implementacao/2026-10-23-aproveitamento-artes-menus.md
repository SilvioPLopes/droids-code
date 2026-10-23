# Análise — aproveitamento de artes para menus e itens

**Estado e data:** análise visual registrada em 23/10/2026; nenhuma montagem de menu iniciada.

**Objetivo:** preservar a avaliação das imagens fornecidas para loja, status, menu do Droid e navegação, como ponto de partida para escolher o próximo painel.

**Escopo documental desta rodada:** criar este registro e vinculá-lo no índice de trabalho. Preservar imagens, `.meta`, scripts, prefabs, cenas, Canvas/Scaler e saves. Reversão: remover apenas os acréscimos documentais desta rodada.

**Dependências e aceite:** nenhuma dependência de execução alterada; consumidores dos painéis deverão ser identificados antes de uma montagem futura. O registro deve distinguir observações, sugestões e autorização do usuário. Validação mínima: conferir os caminhos citados e o diff documental; não exige teste Unity.

## Fonte e liberdade de adaptação

Pasta informada pelo usuário: `C:\Users\silve\DroidsCode\Assets\sprites\Atualizados\novos` ([abrir pasta](../../Assets/sprites/Atualizados/novos)). Esta análise cobre os arquivos abaixo, não um inventário completo da pasta.

O usuário autorizou alterar e fundir imagens e informou que pode usar um programa para remover fundos. Essa liberdade permite preparar composições futuras; não representa aprovação de uma montagem específica nem autorização para modificar sistemas de jogo fora do painel escolhido. Nesta rodada, pediu somente documentar a análise.

Aplicar o [método de interfaces sobre imagens](../guias/interfaces-sobre-imagens.md): partir do conteúdo e da condição real de leitura, separar arte dos controles e dados, preservar originais em versões irmãs e validar cada painel no tamanho de uso. Os parâmetros do terminal não são medidas universais para estes menus.

## Avaliação das imagens

| Arquivo | Uso sugerido | O que aproveitar e o que avaliar |
|---|---|---|
| `2-removebg-preview.png` | Status e equipamento do protagonista | Distribuição entre personagem, atributos e inventário. Substituir o personagem ilustrado pelo nosso; conferir legibilidade dos campos e slots pequenos. |
| `3-removebg-preview.png` | Ícones de loja, inventário e recompensas | Baterias, medicamentos, ferramentas e componentes. Selecionar os que correspondam a itens reais; a existência de um ícone não implica criar uma mecânica. |
| `4-removebg-preview.png` | Referência de status, navegação e compra | Mostra a intenção dos espaços, mas contém textos, números e símbolos de demonstração. Usar a composição como referência, retirando conteúdo variável impresso. |
| `8-removebg-preview.png` | Base para status e loja | Áreas vazias favorecem nomes, preços, descrições e botões reais. Uma das bases mais flexíveis e com menos limpeza de conteúdo demonstrativo. |
| `9-removebg-preview.png` | Kit compartilhado de interface | Molduras, botões, slots e detalhes para dar consistência aos menus. Permite compor dimensões adequadas ao conteúdo; avaliar recortes e bordas antes de montar. |
| `10.jfif` | Referência de informações do status | Hierarquia clara de personagem, vida, experiência e atributos. Composição vertical precisa ser adaptada à tela horizontal. Não reproduzir os textos técnicos de exemplo nem valores impressos. |
| `12.jfif` | Menu geral do jogo | Moldura e oito botões com identidade visual clara. Ajustar quantidade, ícones e disposição às opções existentes; remover o fundo magenta para uso como painel. |
| `15.jpeg` | Menu do Droid | Linguagem de manutenção, equipamentos e melhorias. Simplificar decoração e quantidade de slots para destacar o Droid e dados relevantes. A silhueta central não deve substituir a representação real do personagem. |
| `17-removebg-preview.png` | Itens especiais, missões e loja | Chaves, circuitos, ferramentas e objetos narrativos. Selecionar pela função no jogo; palavras impressas em alguns ícones podem ficar ilegíveis quando reduzidas. |

As cópias `2-removebg-preview - Copia.png` e `3-removebg-preview - Copia.png` têm conteúdo idêntico aos respectivos arquivos sem “Copia”, confirmado por SHA-256 durante a análise. A imagem `15.jpeg` foi apresentada duas vezes na conversa; isso não representa duas alternativas distintas. Nenhum duplicado foi apagado.

## Proposta inicial de combinação

Estas são recomendações de análise, ainda sem escolha final do usuário:

| Destino | Referências propostas | Direção |
|---|---|---|
| Menu geral | 12 | Aproveitar navegação e identidade visual, ajustando às opções reais. |
| Status do protagonista | 10 + 8 + 9 | Manter a hierarquia de informações da 10 em composição horizontal, com painéis e peças das outras duas. |
| Menu do Droid | 15 | Reduzir decoração e espaços sem função; ampliar personagem, peças e informações relevantes. |
| Loja | 8 + 9 + 3/17 | Grade de produtos, descrição, preço e ações reais, usando ícones selecionados. |

A recomendação inicial foi começar pela loja, porque o conjunto 8 + 9 + 3/17 oferece uma base flexível com menos reconstrução de arte. A ordem não foi aprovada pelo usuário.

## Cuidados antes de produzir

- Há magenta visível nos contornos de várias prévias e fundo magenta na 12. Conferir os arquivos, canal alpha e bordas sobre fundos claros e escuros antes de escolher a limpeza. Não pressupor transparência perfeita pelo nome “removebg”.
- Remover fundo e corrigir halos são verificações diferentes. O usuário pode fornecer uma versão sem fundo; avaliar o resultado antes da montagem.
- Manter estilo coerente entre metal, ferrugem, engrenagens e cores; não fundir todas as peças apenas porque estão disponíveis. Evitar diferenças excessivas de detalhe e escala entre ícones.
- Usar fontes, nomes, valores, preços, barras, legendas, estados e regiões clicáveis reais no Unity. Não deixar exemplos da imagem parecerem dados do jogo.
- Não aumentar resolução e declarar melhora de leitura sem verificar composição, contraste e tamanho exibido. A experiência do terminal mostrou que recorte sozinho pode não resolver.
- Definir conteúdo real, escopo, consumidores, resolução, backup e aceite visível no plano do painel antes de editar sua montagem. Preservar funções e referências existentes.

## Retomada

**Atualização posterior:** o usuário escolheu começar pela loja e enviou capturas da interface atual. Implementação e validação passam a ser acompanhadas no [plano da loja ilustrada](2026-10-23-loja-ilustrada.md). A avaliação acima permanece como referência inicial; a escolha da loja já foi feita.

**Resultado:** avaliação e sugestões preservadas neste documento; liberdade de adaptação registrada. Somente documentação editada nesta rodada.

**Limites da evidência:** inspeção visual das imagens enviadas e abertura local de 10/12; comparação de hash dos dois pares duplicados. Não foram testados recortes, transparência, importação Unity, interação ou legibilidade dentro do jogo. As recomendações não comprovam integração pronta.

**Próximo passo:** escolher com o usuário um painel e listar seu conteúdo real; preencher a ficha do guia antes de criar/adaptar a arte. Nenhuma escolha de painel está pendente para concluir esta tarefa documental.

**Estado Unity/save/backups:** Unity não operado nesta rodada; estado atual não consultado. Nenhum save ou asset alterado, nenhum backup novo necessário, nenhum commit/push solicitado.
