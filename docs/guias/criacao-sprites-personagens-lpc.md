# Criação e revisão de sprites de personagens com LPC

Guia do acervo real do Droids Code. Para moradores e funções por edifício, seguir o [plano de Ferrovale](../planos-implementacao/2026-10-28-moradores-e-rotinas-ferrovale.md). Este guia prepara produções futuras; não significa que o compositor histórico já execute no Windows ou que uma nova sprite tenha sido gerada.

## Fontes e estrutura vigente

- [Catálogo visual](../../Assets/sprites/Entities/Characters/Documentation/character-catalog.md), [mapeamento narrativo](../../Assets/sprites/Entities/Characters/Documentation/sprite-mapping.md) e [cobertura de animações](../../Assets/sprites/Entities/Characters/Documentation/animation-coverage.md).
- Elenco específico em `Assets/sprites/Entities/Characters/{Protagonist,Companion,Antagonists,NPCs}/`; figurantes em `NPCs/Generic/`. Os caminhos antigos `Assets/Characters/` escritos em parte da documentação são históricos, não o destino atual dos assets.
- [Instrução original fornecida pelo usuário](../../output/moradores-ferrovale/2026-10-28/instrucao-original-sprites.txt), preservada integralmente. Aplicar suas regras de fidelidade e rastreabilidade; não executar uma regeneração completa quando a tarefa pede um personagem.
- [Gerador oficial](https://liberatedpixelcup.github.io/Universal-LPC-Spritesheet-Character-Generator/) e [repositório LPC](https://github.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator). O repositório oferece definições, spritesheets e créditos por peça; não assumir que a interface atual mantém todos os IDs de uma receita antiga.

## O que já existe e o que falta para automatizar

**Atualização da montagem de 29/10:** Benta/Zeca e duas variantes infantis agora existem como derivações dos modelos LPC, com receitas/créditos próprios. A execução específica está em output/montagem-ferrovale/2026-10-29; dependências locais em output/ferramentas/lpc-portatil. Não foi executado nem portado integralmente o build.py histórico. O diagnóstico de dependências ausentes abaixo registra a análise anterior; não significa que as variantes atuais continuem faltando. O jogo não depende dessas ferramentas Python para rodar.

O inventário de 28/10 registra **18 montagens específicas** (inclui Lip com mochila, Apollo provisório e Célia para referência) e **12 genéricas**. Todas têm imagem de referência 64×64 e walk 576×256, nove quadros por direção. As 12 genéricas têm idle 128×256; dez montagens específicas não têm idle nativo. Nesse caso, usar a pose parada do walk na montagem de animação, sem anunciar uma animação idle nova.

Ferramentas históricas preservadas em [Documentation/tools](../../Assets/sprites/Entities/Characters/Documentation/tools/):

| Arquivo | Responsabilidade existente | Limite atual |
|---|---|---|
| `lpc.py` | Compor camadas por zPos, aplicar paletas, consultar cobertura e recortar quadros | Importa metadados de `/home/claude/meta`; sprites em `/home/claude/lpc`; depende de Pillow/NumPy |
| `cast.py` | Receitas do elenco específico | Não é o cadastro dos 12 genéricos nem prova de que uma peça segue disponível upstream |
| `spec.py` | Características e justificativas narrativas | Não resolve divergências novas entre diálogo e biografia |
| `build.py` | Produção histórica em lote, documentação e créditos | Caminhos `/mnt/user-data/...`, import Linux e remoção recursiva da pasta de saída: não executar como comando de produção neste workspace |

Consulta do Python padrão nesta análise: Pillow e NumPy ausentes; metadados do compositor não encontrados nos diretórios de assets/output inspecionados. Nenhum pacote foi instalado, repositório baixado ou script de montagem executado. Não há um comando de geração Windows validado nesta entrega.

Automatização futura deve adaptar entradas/saída para caminhos explícitos, validar que a saída permanece no diretório novo da tarefa e processar um ID por vez. Deve recusar sobrescrita e qualquer limpeza ampla. O teste mínimo antes de usar um personagem novo é reproduzir uma receita existente em output separado e comparar quadros, dimensões, transparência e créditos. Isso verifica a compatibilidade do adaptador; não é necessário regenerar o elenco inteiro.

## Receitas: preservar os dois formatos

| Acervo | Receita real | Campos principais |
|---|---|---|
| Personagem específico | `character.json` | `personagem`, `body_type`, `skin`, `itens`, `animacoes_cobertas`, `url_gerador_nao_testada`, `repositorio_lpc_commit` |
| Genérico | `character-config.json` | `id`, `body_type`, `body_color`, `items` com `id`, `color`/`eye_color` |

Não passar uma receita genérica diretamente ao compositor de específicos: os esquemas diferem. O futuro adaptador deve manter a receita original e produzir uma representação normalizada, registrando qualquer conversão. Não renomear arquivos existentes para simular compatibilidade. O commit declarado nas receitas específicas é `9bf06be193b053db256b2536824af426de046aa2`; sua disponibilidade remota não foi revalidada nesta análise. Links antigos marcados como não testados continuam com esse estado.

## Ficha mínima de uma produção

Antes de gerar, registrar na tarefa:

1. ID de personagem e ID de catálogo, quando houver; morador, visitante, inimigo ou referência de foto. Uma receita visual não cria automaticamente um NPC em `CatalogoDeNpcs`.
2. Fonte de cada característica: diálogo/quest, enredo/adendo, catálogo ou proposta. Ler o enredo completo antes de iniciar uma nova montagem narrativa, conforme a instrução fornecida; uma consulta parcial para distribuir casas não substitui essa etapa.
3. Corpo/cabeça/pele, cabelo, roupa, acessórios e silhueta. Reutilizar IDs reais de peças conferidos nos metadados do gerador, sem inventar opções. Profissão não determina tom de pele, riqueza ou personalidade.
4. Receita original, revisão/commit da fonte, ordem das camadas, paletas, animações pedidas e cobertura real de todas as peças. Não pedir run/combat_idle apenas porque walk existe.
5. Destino novo em `output/`, relação de créditos e diferenças intencionais. Para revisar alguém existente, manter a versão anterior e apresentar comparação.

A posição da casa pertence ao planejamento do mundo; não deve ser gravada como se fosse parte da aparência canônica da pessoa.

## Sequência de produção e aceite

1. **Auditar a receita e os arquivos de origem.** Conferir suporte de cada peça/corpo por animação e tamanho dos quadros. Não usar uma categoria visual humana para simular um robô final sem identificar a aproximação.
2. **Compor em área de trabalho separada.** Manter estilo, proporção e transparência; variar roupas/cabelo/acessórios para diferenciar personagens, preservando características já estabelecidas.
3. **Verificar a imagem.** PNG RGBA, dimensões reais, alinhamento dos pés/pivot, quatro direções de walk quando previstas, camadas sem cortes e ausência de magenta de máscara. Conferir todos os quadros, não só a miniatura frontal. Não esticar um NPC 64×64 para 4K: a resolução dos mapas não define a grade das sprites.
4. **Guardar exportação e evidência.** `sprite.png`, sheets/animações realmente produzidos, receita, descrição de aproximações/pendências, créditos e hash. Aprovação visual da referência não comprova que Animator e collider funcionam no jogo.
5. **Integrar só o personagem aprovado.** Preservar GUIDs/metas de assets usados; configurar importação de pixel art e slicing conforme o consumidor real. Não copiar PPU/pivot entre humano, criança e robô sem comparar a altura visível e o ponto dos pés. Usar controladores e parâmetros existentes (`MoveX`, `MoveY`, `Speed`) quando compatíveis. Walk aqui usa linhas cima/esquerda/baixo/direita; não aplicar flip extra quando já existem lados distintos.
6. **Verificar no Unity.** Alinhamento com piso/casas, colisão nos pés, orientação ao conversar, diálogos/quests por ID e persistência de presença. Separar revisão do arquivo, montagem salva, teste por API, input real e aprovação visual.

Os assets ativos da City podem estar diferentes do catálogo visual: na leitura desta rodada, Tácio usava idle de Ivo e Homem da Fila usava o modelo genérico morador_01. Há sprites específicos disponíveis para ambos. É uma divergência de montagem a resolver no recorte de NPCs, não prova de que faltam imagens; nada foi trocado aqui.

## Créditos e proveniência

Preservar os créditos específicos de [elenco](../../Assets/sprites/Entities/Characters/Documentation/CREDITS.md) e [genéricos](../../Assets/sprites/Entities/Characters/NPCs/Generic/CREDITS.md). Para nova combinação, guardar autores, arquivos de origem, URLs e licenças efetivamente selecionadas por peça. O [README oficial, seção de atribuição](https://github.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator#licensing-and-attribution-credits) explica a exportação de créditos; o [CREDITS.csv](https://github.com/LiberatedPixelCup/Universal-LPC-Spritesheet-Character-Generator/blob/master/CREDITS.csv) é a relação de origem. Não atribuir uma licença única ao acervo inteiro nem copiar a lista de um personagem diferente como se cobrisse uma nova receita.

## Reutilização para inimigos

O procedimento de ficha, variantes, arquivos de origem, transparência, créditos e validação também serve aos moldes de inimigos. A técnica visual depende do tipo:

| Tipo | Caminho de produção | Contrato a registrar |
|---|---|---|
| Humanoide compatível com LPC | Receita modular conferida | Corpo, silhueta, direção de combate, idle/ataques realmente existentes |
| Droid/robô, sucata, drone, chefe grande | Arte/folha própria compatível com o projeto | Dimensão, pivot, enquadramento na Battle, poses e animações necessárias; sem impor a grade humana |
| Variação de inimigo existente | Derivação versionada, preservando identidade e créditos | Diferença visível além do nome, tamanho/efeitos e associação ao ID correto |

IDs já encontrados em `CatalogoDeInimigos`: `sucata_captura`, `sucata_reforcada`, `droid_selvagem`, `droid_selvagem_alfa`, `carcaca_errante`, `drone_reconhecimento`, `droid_do_ivo`, `esqueleto_teste`. Esta lista não comprova arte final, script de batalha ou balanceamento concluídos. Apollo LPC continua explicitamente provisório; Artemis não recebe corpo humanoide por conveniência do gerador. Não produzir inimigos novos ou modificar combate como efeito desta documentação.
