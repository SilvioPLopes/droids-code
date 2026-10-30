# Fontes e critérios da wiki

Esta página explica como o material de desenvolvimento foi transformado em uma wiki de jogo. As fontes continuam no repositório principal; a wiki as organiza para jogadores, professores e apresentação do projeto.

## Qual fonte responde a qual pergunta

| Pergunta | Fonte principal |
|---|---|
| Quem são os personagens e por que agem? | Enredo v2 e adendo |
| Qual é a origem vigente de Apollo? | Adendo, Parte A |
| Como se organizam os atos finais? | Adendo, Partes B/C |
| Quais NPCs, lugares, objetos e missões estão cadastrados? | Catálogos em C# |
| Quais são os atributos atuais de um inimigo? | Catálogo de inimigos vigente |
| Como funciona o comportamento especial do boss? | `DroidDoIvo.cs` |
| Uma mecânica foi montada e testada? | Contrato, plano e evidência específica, com data e condições |
| Qual sprite está disponível? | Asset existente e catálogo visual, quando aplicável |

## Precedência narrativa

O adendo declara que prevalece sobre o enredo principal nas divergências. Isso altera especialmente a origem de Apollo: são **431 cópias em núcleos**, não um único backup. A Apex procura cópias sobreviventes, sem uma perseguição exclusiva ao protagonista desde o início.

Uma descrição antiga do catálogo não é usada para desfazer essa decisão. A wiki também não modifica os scripts para fazê-los concordar: reconciliação de código e roteiro é uma tarefa diferente.

## Diferenças encontradas e tratamento editorial

| Tema | Diferença nas fontes | Como a wiki trata |
|---|---|---|
| Idades dos NPCs | Alguns números divergem entre ficha narrativa e catálogo | Omitir idades divergentes; manter apenas as sustentadas e necessárias |
| Célia | Datas e circunstâncias de recolhimento não são uniformes | Explicar o papel e a ausência, sem inventar uma data definitiva de morte |
| Tácio | Situação da filha e funcionamento do contador têm versões diferentes | Preservar motivação e missões sem afirmar uma situação definitiva não reconciliada |
| Páginas do caderno | Versões anteriores mudam autoria e destino das páginas | Seguir o adendo para o inventário de despacho e a revelação na Torre |
| Torre/Prumo/Bastião 4 | Nomes aparecem aproximados no texto principal | Usar a distinção espacial desenvolvida pelo adendo, sem declarar equivalência definitiva |
| Portão da cidade | Leste no roteiro, norte em registros de local/vigia | Preservar os nomes na origem e sinalizar a divergência |
| Apollo no final Ramificar | Uma frase chama Apagar de única sobrevivência; a seção específica de Ramificar afirma sobrevivência também | Usar a descrição detalhada de cada operação e registrar o conflito |
| Vida do boss | Há proposta anterior de 210; cadastro atual traz 200 | Publicar **200 HP**, conforme o arquivo atual |

## Catálogos utilizados

- [NPCs](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeNpcs.cs).
- [Locais](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeLocais.cs).
- [Missões](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeQuests.cs).
- [Itens](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeItens.cs).
- [Inimigos](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeInimigos.cs).
- [Puzzles](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDePuzzles.cs).
- [Diálogos](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeDialogos.cs): referência de organização e progressão, sem transcrição das falas na wiki.

As entradas de itens, missões, locais, inimigos e puzzles foram conferidas por seus IDs. Os textos da wiki condensam e reorganizam essas fontes; não substituem seus contratos técnicos.

## Narrativa e imagem

Um personagem pode ter biografia sem possuir uma sprite selecionada nesta galeria. Não foi criada arte para preencher essas ausências. As imagens incluídas são cópias de assets existentes, com procedência na [Galeria](https://github.com/SilvioPLopes/droids-code/wiki/Galeria).

Enredo e diálogos contêm falas com funções dramáticas específicas. A wiki descreve ações, conflitos e motivações sem reproduzir roteiros de conversa. As seções com revelações e finais são sinalizadas como spoilers.

## Fontes narrativas e técnicas

- [Enredo v2](https://github.com/SilvioPLopes/droids-code/blob/main/docs/enredo/TCC_-_ENREDO_v2.md).
- [Adendo vigente](https://github.com/SilvioPLopes/droids-code/blob/main/docs/enredo/TCC_-_ENREDO_v2_ADENDO.md).
- [Contexto do projeto](https://github.com/SilvioPLopes/droids-code/blob/main/docs/contexto-projeto.md).
- [Contrato de combate](https://github.com/SilvioPLopes/droids-code/blob/main/docs/contextos/sistemas/combate.md).
- [Contrato de estado e save](https://github.com/SilvioPLopes/droids-code/blob/main/docs/contextos/sistemas/estado-e-save.md).
- [Matriz de nomenclatura de Ferrovale](https://github.com/SilvioPLopes/droids-code/blob/main/docs/planos-implementacao/2026-10-29-refatoracao-nomenclatura-ferrovale.md).
- [Plano de produção e publicação desta wiki](https://github.com/SilvioPLopes/droids-code/blob/main/docs/planos-implementacao/2026-10-29-readme-e-wiki.md).

## Manter uma documentação viva

Mudanças nas regras devem atualizar a página correspondente. Uma mudança de cânone precisa registrar qual decisão substituiu a anterior. Quando um conteúdo passar de catálogo para percurso validado, atualizar sua situação com a evidência adequada.

O objetivo é permitir consulta confiável sem copiar todos os planos de implementação. Os planos preservam o trabalho e as evidências; a wiki apresenta o jogo e seu estado compreensível ao leitor.
