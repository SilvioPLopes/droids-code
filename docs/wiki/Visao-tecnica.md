# Visão técnica

Esta página apresenta a organização do programa para quem quer compreender o projeto sem começar pelos planos de implementação. A enciclopédia de personagens e mundo começa na [Home](https://github.com/SilvioPLopes/droids-code/wiki).

## Tecnologias

Unity 6000.5.8f1 e C# compõem o jogo. O terminal executa Lua por meio de MoonSharp. A apresentação usa recursos 2D do Unity, URP, uGUI e TextMeshPro. Os pacotes estão declarados em `Packages/manifest.json`.

A página dedicada à linguagem permanece [em construção](https://github.com/SilvioPLopes/droids-code/wiki/Lua). Esta visão técnica trata da relação entre os componentes do programa.

## Organização das cenas

| Cena | Papel |
|---|---|
| `MainMenu` | Entrada do jogo; Novo Jogo carrega CasaLip |
| `CasaLip` | Introdução com Apollo e desafio do gerador de água |
| `City` | Exploração de Ferrovale, conversas, comércio e acesso a interiores |
| `Game` | Mapa externo/vale |
| `Battle` | Interface e execução dos combates por turnos |
| `Interiores/` | Ambientes internos de Ferrovale |

Essa organização descreve os arquivos existentes; não certifica que todos os percursos narrativos estejam completos.

## Camadas de responsabilidade

| Camada | Responsabilidade | Exemplos |
|---|---|---|
| Catálogos | Definir conteúdo por identificadores estáveis | Itens, inimigos, quests, NPCs, locais, puzzles |
| Domínio | Representar regras e dados do Droid/combate | Atributos, técnicas, energia, dano e efeitos |
| História e mundo | Condicionar eventos e passagens | Gatilhos de conversa, flags, portas e verificadores |
| Interface | Apresentar opções e resultados | Terminal, Bag, Status, Droid, Loja e Battle |
| Persistência | Guardar e restaurar progresso | Estado, inventário, equipamentos e checkpoints |
| Cenas/prefabs | Montar instâncias e referências concretas | Player, colliders, painéis, portas e eventos |

Um catálogo fornece a definição; a cena fornece a presença. Por isso adicionar uma missão em dados não cria automaticamente todos os NPCs e gatilhos necessários para jogá-la.

## O caminho de um desafio

1. A interação abre o terminal e, quando aplicável, seu contexto de puzzle.
2. O jogador executa o programa no interpretador.
3. O verificador consulta os valores finais exigidos pelo catálogo.
4. A primeira conclusão altera estado e pode disparar eventos e persistência conforme o controlador.
5. A montagem local, como a porta da CasaLip, responde à conclusão.

Na casa, o sistema distingue introdução de conclusão. Restaurar uma flag de puzzle já resolvido deve reaplicar a porta sem conceder novamente recompensa ou conversa de primeira resolução.

## Conteúdo desacoplado da aparência

Um item tem ID, categoria, preço e efeito no catálogo lógico. Seu ícone fica no catálogo visual. Essa separação permite substituir uma imagem sem mudar o comportamento ou o ID usado por inventário e missão.

Inimigos usam suas definições para atributos e recompensas. O boss associado a Ivo acrescenta comportamento próprio por uma classe específica, sem fazer da arte o lugar onde as regras são definidas.

## Persistência e combate

O estado do jogador atravessa cenas e precisa ser restaurado de forma coerente. O save guarda equipamentos por slot/ID e dados do Droid conforme a versão do formato. Entrar em Battle registra um checkpoint anterior ao confronto; o sistema não reconstrói um turno intermediário apenas por carregar um save.

Essas regras de persistência e a navegação da interface têm contratos específicos. Consultá-los é mais seguro que inferir o funcionamento somente pelo nome de um arquivo ou por uma captura de tela.

## Catálogo, código e montagem

A wiki usa esses termos com precisão. **Catálogo** significa dados existentes; **código** significa uma regra implementada; **montagem** significa referência e instância na cena; **validação** significa execução observada nas condições registradas. A [página de estado](https://github.com/SilvioPLopes/droids-code/wiki/Estado-do-projeto) aplica essa distinção ao recorte atual.

## Fontes de referência

Os documentos abaixo pertencem ao repositório principal. Decisões e pendências permanecem nas fontes técnicas, evitando cópias divergentes na wiki.

- [Contexto e arquitetura](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contexto-projeto.md).
- [Índice por assunto](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/indice-trabalho.md).
- [Interface](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contextos/sistemas/interface.md).
- [Estado e salvamento](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contextos/sistemas/estado-e-save.md).
- [Combate](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contextos/sistemas/combate.md).
- [Enredo](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/enredo/TCC_-_ENREDO_v2.md) e [adendo](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/enredo/TCC_-_ENREDO_v2_ADENDO.md).

Links para arquivos locais novos ou modificados só refletirão esta entrega no GitHub depois do envio da versão correspondente.
