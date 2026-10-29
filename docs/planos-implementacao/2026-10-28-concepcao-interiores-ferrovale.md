# Concepção — Interiores de Ferrovale por edifício

**Estado:** concepção e auditoria em execução, 28/10/2026. Pedido do usuário: documentar a ideia geral antes da produção e começar os interiores da cidade inteira usando fachadas, entradas e matrizes aprovadas. Este documento é subordinado ao [plano de portas, interiores e mapas](2026-10-28-portas-interiores-e-mapas.md); coordenadas coletadas continuam somente na [coleta](2026-10-17-auditoria-cenas.md#coleta-de-entradas-da-city--28102026).

## Escopo antes de produzir

Permitidos: este documento; links no índice e plano principal/geral; novas artes, prompts, inventário e recortes de inspeção em `output/interiores-ferrovale/2026-10-28/`. Examinar o mapa `Assets/sprites/Mapas/metavalle.jpg`, matrizes aprovadas e campos restritos da City por MCP somente leitura. Não alterar Assets, cenas, contornos, portas, câmera, scripts, importers, saves ou atribuir moradores/quests. Criar artes é diferente de instalar interiores jogáveis. Preservar todas as matrizes anteriores; nenhuma substituição automática.

Reversão: retirar apenas novos arquivos sem consumidores e diffs documentais desta rodada. Nenhum backup novo de cena/save necessário. Último estado consultado: projeto correto, City limpa e fora de Play. Validar cada imagem visualmente, registrar dimensões reais e referências; não apresentar geração como aprovação do usuário ou entrega final em 4K/8K.

## Preferências duráveis do usuário

- Pequenas alterações devem criar identidade entre ambientes: distribuição dos móveis, materiais, textura, reparos, conservação e detalhes de uso. Evitar apartamentos integralmente idênticos.
- Variar pisos de azulejo/cerâmica, cimento, metal/chapas e madeira/tacos. Madeira é uma possibilidade, não o padrão obrigatório. Aumentar presença de metal em paredes, estruturas, móveis e pisos conforme fachada.
- Há diferenças de recursos mesmo no pós-apocalipse: moradias mais pobres podem ter remendos coerentes e materiais reaproveitados; as de mais recursos podem ter acabamentos mais completos e conservados, sem luxo deslocado do mundo. Pobreza não significa piso intransitável, lixo gratuito ou ruína sem função.
- Tacos, juntas, retângulos e direções de textura precisam de alinhamento consistente; transições de material devem fechar com intenção. Evitar remendos cuja geometria pareça erro de geração. O usuário cogitou descartar o primeiro apartamento por desalinhamento: A v2 fica fora da seleção de produção, preservada como histórico; B/C/D continuam bases utilizáveis. Não apagar o arquivo A.
- Materiais/acabamento das bases aprovados; câmera top-down de RPG e orientação dos móveis seguindo seu uso. Sem cama/cozinha de kitnet como fórmula, sem pendente de teto, sem personagem ou UI incorporados. Porta única em apartamentos, entrada curta e discreta. Exceções com múltiplas entradas exigem fachada/acessos compatíveis.
- Casa, prédio, mansão, porão e terraço têm necessidades próprias. Usar apenas espaços úteis, sem simular residência funcional completa. Escadas precisam de patamares e leitura de subida/descida; terraços apenas quando propostos explicitamente, sem fingir que fachada comprova um acesso não observado.
- Prefeitura é um DOMO METÁLICO especial: sem madeira, planta ampla curva/radial, piso contínuo de metal/cimento com juntas radiais ou grandes placas curvas, sem grade de quadradinhos ou reaproveitamento literal da sala doméstica retangular. Criar arte exclusiva.

## Método de localização e contagem

Mapa ativo inspecionado, 1920×1080. MCP projetou bounds dos colliders e posições de portas para pixels da textura usando Transform inverso, rect, pivot e PPU reais. O percurso em cobra descrito pelo usuário prevalece sobre a instrução inicial de direita/esquerda: a numeração segue a coleta; não renumerar pelas coordenadas. IDs E00…E16 abaixo referem-se aos objetos de contorno, sem renomeá-los no Unity.

Resultado inicial: 17 colliders de objetos `GameObject...`; 16 PolygonCollider2D e 1 EdgeCollider2D. Destes, 15 polígonos coincidem visualmente com volumes de edifícios. E07 está sobre mancha/resíduo no lote inferior esquerdo, sem prédio desenhado; E15 é EdgeCollider2D do perímetro. Conferência por faixas: 4 volumes no alto, 6 no meio, 5 embaixo. Isso é contagem de volumes/lotes construídos, não de moradias ou pavimentos. As geminadas podem conter mais de uma unidade.

Há 24 componentes TransicaoDeCena na City: 21 acessos novos de prédios + entrada existente da CasaLip = 22 entradas de edifícios; saída superior nova + saída inferior existente = 2 saídas de cidade. Entradas não equivalem a 22 casas ou 22 interiores distintos. A posição coletada do Player é referência em frente à porta, não centro final do vão.

Antes de fechar o lote de produção: fazer recortes de leitura das fachadas, conferir material/volumes e registrar acessos adicionais somente como candidatos, sem criar coordenadas exatas a partir de pixels. Recortes de inspeção são cópias para análise, não novas artes nem alterações da textura ativa.

## Organização preliminar

Separar três frentes: (1) tabela por edifício e acessos; (2) produção de artes identificadas pelo ID exterior, preservando bases; (3) integração futura de cenas/portas, após conferência e aceite. Cada arte terá filename estável, prompt e referência. Arte já aprovada pode ser reutilizada quando combina com a fachada; novas variantes devem mudar detalhes com intenção, sem reproduzir erros de alinhamento.

Ainda não há moradores atribuídos. Prefeitura, casa-chave e mansão recebem linguagem própria antes de qualquer uso de catálogo narrativo. Não criar recompensa, pista de roteiro, loja funcional ou puzzle como efeito de desenhar uma bancada.

## Tabela por fachada — conferência em cobra

Coluna de entradas descreve referências coletadas; acessos candidatos da arte não viram posições confirmadas. ID corresponde ao sufixo de `GameObject`, E00 ao objeto sem número. Alto/baixo são faixas da textura; coordenadas de mundo foram reconciliadas por projeção MCP, não por ordenação do nome.

| Ordem da faixa | ID / contorno | Identidade da coleta | Leitura da fachada | Entradas conhecidas / incerteza | Proposta de interiores |
|---|---|---|---|---|---|
| Inferior, esquerda→direita | E06 / (6) | Casa 1 | Torre compacta à esquerda e anexo vermelho; metal e painéis quentes | 1 coletada; possível acesso do anexo a conferir | Casa compacta térreo/superior + anexo de garagem como proposta, sem vínculo de porta definitivo |
| Inferior | E05 / (5) | Casa 2 | Casa baixa de cobertura alaranjada/azul e duas molduras de acesso | 1 coletada à direita; acesso frontal esquerdo visível, não coletado | Interior de mesma casa com dois acessos ao sul, posição externa esquerda ainda pendente |
| Inferior | E10 / (10) | Casas 3/4 | Conjunto geminado, módulos laranja/azul | 2 coletadas | Duas moradias, já decidido; pisos/arranjos distintos |
| Inferior | E12 / (12) | Entrada_CasaLip existente | Torre estreita metálica azul/cinza | 1 existente, destino CasaLip | Proposta visual de atualização compatível com casa do protagonista; não substituir cena/artes atuais |
| Inferior | E11 / (11) | Casas 5/6 | Geminadas de cobertura vermelha e painéis distintos | 2 coletadas | Duas moradias, já decidido; variar acabamento/recursos |
| Meio, direita→esquerda | E09 / (9) | Casa maior | Grande construção em L com tábuas/painéis quentes e estruturas de metal; acesso lateral elevado aparente | 1 coletada; acesso elevado à direita é candidato, sem XY coletado | Térreo próprio + proposta de piso superior com terraço, sem afirmar que é conteúdo narrativo definido |
| Meio | E08 / (8) | Prédio de duas portas/com placa | Dois volumes unidos, um maior/alto e um anexo | 2 coletadas | Proposta do agente: casa ampla compartilhada com duas entradas e acesso superior; escolha anterior estava aberta |
| Meio | E16 / (16) | Prefeitura | Domo cinza, nervuras e anexos metálicos; nenhuma madeira | 1 coletada | Salão cívico amplo e circular/radial próprio, metálico e sem piso quadriculado |
| Meio | E00 / sem número | Casa 7 | Torre estreita alaranjada/metálica, fachadas em faixas | 1 coletada | Hall de prédio compacto, patamar reutilizável e apartamento; altura real/pavimentos adicionais não presumidos |
| Meio | E01 / (1) | Casa 8 | Torre estreita de cobertura azul e metal | 1 coletada | Hall próprio, patamar reutilizável e apartamento com mais metal |
| Meio | E02 / (2) | Casa 9 + garagem | Casa com volume anexo, cobertura azul/oxidada | 2 coletadas (casa/garagem) | Casa e garagem próprias, mesmo contorno; conexão interna a decidir |
| Superior, esquerda→direita | E03 / (3) | Possível mansão/casa-chave | Volume largo, duas faixas de janelas e cobertura metálica plana | 1 coletada | Hall amplo e andar superior próprios, maior conservação e metal; sem atribuir dono |
| Superior | E04 / (4) | Geminado azul/visual distinto | Dois módulos de cobertura azul, divisão estrutural e dois acessos | 2 coletadas | Proposta do agente: duas unidades independentes com layouts aprovados C/D; função especial ainda em aberto |
| Superior | E13 / (13) | Casa 10 | Volume azul + volume laranja alto unidos | 1 coletada; faixa frontal esquerda não comprovada como porta | Hall próprio, patamar reutilizável e apartamento; não transformar janela em passagem |
| Superior | E14 / (14) | Casa com porão | Dois volumes de aspecto/material diferente e acesso baixo central | 3 coletadas (esquerda/porão/direita) | Duas unidades superiores independentes e porão comum propostos, relação funcional ainda por aprovar |

**Conferência de número:** 15 conjuntos construídos (14 de uso potencialmente residencial/serviço + 1 prefeitura), 22 entradas de edifícios registradas (21 novas e 1 existente). E07 e E15 excluídos da contagem de casas. Duas saídas da cidade não entram na contagem de edifícios. Há um acesso adicional claro em E05 ainda sem coleta e candidatos em E06/E09; não fechar um total definitivo de portas da arte sem conferência desses pontos. A numeração Casas 1–10 é nomenclatura da coleta, não total de residências. As alturas e a quantidade de apartamentos permanecem design, não contagem comprovada por collider.

## Lote de produção autorizado — 24 novas artes e reutilização

Produzir 24 artes por edifício, listadas abaixo; reutilizar explicitamente seis matrizes aprovadas em oito usos (patamar compartilhado entre torres e piso superior E08, unidades E04, apartamento E00 e unidade direita E14). Arte reutilizada não será contada como geração nova. Todos os destinos são propostas de arte, não conexões ativas de cena. Novos arquivos/proveniência ficam em `output/interiores-ferrovale/2026-10-28/`; não sobrescrever as bases.

| Chave | Nova arte prevista | Família / material predominante |
|---|---|---|
| E16-civico | e16-prefeitura-domo.png | Grande planta circular metálica; piso contínuo radial, sem madeira/grade |
| E06-terreo | e06-casa01-terreo.png | Compacta vertical, madeira reparada + chapa; escada |
| E06-superior | e06-casa01-superior.png | Sala superior compacta, retorno pela escada, piso de metal |
| E06-anexo | e06-casa01-anexo.png | Garagem/anexo proposto, concreto e metal |
| E05-casa | e05-casa02.png | Casa baixa com dois acessos; cerâmica recuperada e chapas |
| E10-a | e10-casa03.png | Moradia simples de metal e cimento |
| E10-b | e10-casa04.png | Moradia de manutenção maior, azulejo e painéis reaproveitados |
| E11-a | e11-casa05.png | Painéis vermelhos, concreto e reparos discretos |
| E11-b | e11-casa06.png | Cerâmica terrosa e metal, arranjo distinto |
| E12-casa | e12-casalip-proposta.png | Torre metálica; referência da CasaLip com escada/terminal, sem pendente |
| E09-terreo | e09-casa-maior-terreo.png | Planta em L, madeira + metal, escada |
| E09-superior | e09-superior-terraco.png | Piso superior com área externa de terraço, aço/cimento |
| E08-casa | e08-casa-duas-entradas.png | Casa compartilhada com duas portas ao sul e escada; piso cerâmico |
| E00-hall | e00-casa07-hall.png | Hall estreito, metal alaranjado e piso de chapa |
| E01-hall | e01-casa08-hall.png | Hall estreito azul, cimento/metal |
| E01-unidade | e01-apartamento-metal.png | Layout C aprovado, piso metálico com juntas alinhadas |
| E02-casa | e02-casa09.png | Casa de metal/painéis terrosos, sala compacta e piso cerâmico |
| E02-garagem | e02-garagem.png | Garagem própria, placa/concreto, ferramentas sem função de jogo nova |
| E03-terreo | e03-mansao-terreo.png | Hall largo próprio, piso de grandes peças alinhadas e metal conservado |
| E03-superior | e03-mansao-superior.png | Sala de leitura própria com escada descendente |
| E13-hall | e13-casa10-hall.png | Hall de dois volumes, metal azul/ocre e escada |
| E13-unidade | e13-apartamento-ceramica.png | Layout D aprovado, piso cerâmico/coerente |
| E14-esquerda | e14-unidade-esquerda.png | Unidade compacta, madeira em painéis e concreto/metal no chão |
| E14-porao | e14-porao.png | Porão de metal/cimento, depósito e escada de saída |

Reutilizações declaradas (sem cópia de arte para simular criação): `matriz-predio-corredor-v1.png` como patamar genérico de E00/E01/E13; `matriz-casa-grande-superior-v1.png` como matriz do piso superior de E08; `apartamento-b-estar-norte-v2.png` para E00; `apartamento-c-estar-leste-v2.png` e `apartamento-d-mesa-norte-v2.png` para as duas unidades de E04; `interior-sala-piso-cuidado-v3.png` para unidade direita E14. São seis assets distintos em oito usos. Quando a montagem pedir adaptação de footprint/material, derivar nova versão com escopo próprio em vez de prometer encaixe perfeito da matriz. Não inclui apartamento A desalinhado.

**Critérios de produção:** preservar proporção e escala visual, uma entrada em apartamentos, manter exatamente entradas justificadas em casas compartilhadas, nenhum pendente/cama/cozinha de kitnet, piso com juntas/retângulos bem alinhados. Especializar prefeitura e mansão; não apenas trocar cor da mesma sala. Inspecionar cada geração e registrar limites. Não fabricar resolução: medir pixels reais. Aprovação destas novas artes permanece do usuário; se rejeitar, suspender nova tentativa daquele alvo e reavaliar.

**Checagem única de concepção:** revisor independente em leitura restrita confirmou a cobertura dos 15 edifícios e a conta 24 novas + seis assets em oito usos. Apontou somente a ligação casa/garagem E02, que afeta portas internas. Decisão do agente antes dos prompts: E02 terá dois interiores independentes, cada qual com seu acesso externo, sem porta de ligação interna nesta proposta. É uma escolha arquitetônica explícita para avaliação, não fato já decidido na história; não adicionar acesso interno por suposição.

## Retomada

### Escopo da associação e estudo de integração — 28/10/2026

Pedido seguinte do usuário: associar imagens às casas/portas e entender cenas versus prefabs. Arquivos permitidos nesta rodada: este plano e `output/interiores-ferrovale/2026-10-28/mapa-integracao.json`. Estado anterior: catálogo/manifesto associam artes a fachadas, sem tabela completa de ligações de ambientes; cenas e destinos não serão editados durante esta definição. Dependências consultadas somente em leitura: TransicaoDeCena, RestaurarPosicao, DadosDoJogo/SalvamentoJson, CameraFollow e persistência do menu do mundo.

Aceite documental: 32 ambientes com IDs distintos nos 15 edifícios, cobertura das 22 entradas externas registradas, escadas internas separadas das portas da City, arquivos de arte existentes, ligações recíprocas propostas e nenhuma coordenada de chegada inventada. Validação mínima: contagens, unicidade, existência dos arquivos e integridade das referências. Reversão: retirar apenas esta seção/resultado de integração e o JSON novo; preservar catálogo, manifesto e arte existentes. Não há alteração de Unity, assets ativos, GUIDs, save ou scripts nesta rodada.

### Resultado entregue nesta rodada

- [Catálogo visual por fachada](../../output/interiores-ferrovale/2026-10-28/catalogo-interiores.html): mapa identificado, tabela dos 15 edifícios, recorte exterior e interiores associados. Cada nova proposta tem imagem completa e link do prompt. As matrizes reaproveitadas estão explicitamente identificadas.
- [Manifesto final](../../output/interiores-ferrovale/2026-10-28/manifesto-producao.json): 24 alvos novos concluídos, seis bases aprovadas em oito usos, 32 usos de ambientes para cobrir os 15 conjuntos. São 30 assets distintos selecionados, sem contar versões anteriores. Há 27 PNGs novos no lote por causa de três revisões pontuais; somente 24 são selecionados no catálogo. Não confundir esses totais com casas, apartamentos ou portas.
- Dimensões reais das 24 novas propostas selecionadas: **1448×1086**, obtidas na saída nativa da ferramenta. Não são entrega final em 4K/8K; não foi feito upscale para alegar qualidade inexistente. Qualificação de detalhe no zoom do jogo/alta resolução permanece na etapa de artes do plano principal.
- Preferências do usuário guardadas na seção própria: diferenças pequenas, pisos variados/alinhados, mais metal, condições econômicas distintas e projetos especiais. A excluída A v2 não foi selecionada. Nenhum novo interior recebeu morador/quest/serviço, nem aprovação visual presumida.
- Checagens: os 24 hashes SHA-256 selecionados foram conferidos contra os arquivos; são 24 imagens distintas. Todos os prompts existem. Catálogo: 185 referências locais verificadas, nenhum destino ausente; IDs e âncoras internos únicos/válidos. As contagens de 15 edifícios/22 entradas/32 usos conferem. Verificação estrutural do catálogo não é aprovação visual ou execução no jogo.
- QA visual: leitura dos materiais e acessos examinada nas gerações; E03/E06 superiores tiveram travessas frontais retiradas; E03/E09 superiores tiveram saída sul indevida fechada. Seleções são v2 nesses três alvos. Escadas continuam sujeitas à leitura do usuário e alinhamento em montagem; E09 tem diferença de posição entre pisos explicitamente registrada. Não foram desenhados cenários de banho/quartos/cozinhas funcionais como preenchimento.
- Estado: somente documentação, recortes de análise e novos arquivos em output foram produzidos. Unity teve consulta inicial somente leitura, City limpa fora de Play; não foi reconsultado ao final e não foi operado para montagem. Backup anterior de City permanece em `output/backups/2026-10-28-portas-city/City.unity`; nenhum backup/save novo foi criado ou acessado nesta rodada, nenhuma limpeza de save pendente. Crops são referências de inspeção mantidas no catálogo, sem alterar o mapa original.
- Próximo passo: usuário avaliar o catálogo por edifício; conferir acessos candidatos E05/E06/E09 e decisões propostas E04/E08/E14 antes da instalação. Depois definir interiores/cenas/retornos em recorte próprio, com escala real do Player e colisões; nenhuma integração funcional é declarada pronta por esta entrega de arte.

### Histórico de execução deste lote

**Checkpoint de produção:** 12 dos 24 alvos gerados e salvos; manifesto em [manifesto-producao.json](../../output/interiores-ferrovale/2026-10-28/manifesto-producao.json). Originais e revisões dos superiores preservados; E03/E06 superiores v2 são as versões selecionadas após correção de acessos. Houve erro apenas na escrita inicial de três prompts (quebra de linha no patch); imagens não foram regeneradas por isso, prompts recuperados e gravados. Nenhuma operação de cena/save. Próximo lote: E09 térreo, E08 duas entradas, E00 hall.

**QA inicial dos superiores, antes da correção:** E03-superior herdou uma abertura/tapete ao sul, apesar de o prompt pedir perímetro fechado; E03/E06 superiores mostram travessa/corrimão fechando visualmente a aproximação do vão. Causa sustentada para a edição: arte contradiz os próprios acessos definidos, com geometria copiada do térreo de referência. Autorização do lote permite acabamento; corrigir somente fechamento da parede sul E03 e retirar travessa frontal nos dois vãos, mantendo os demais pixels/composição como alvo de preservação. Gerar versões `e03-mansao-superior-v2.png` e `e06-casa01-superior-v2.png`, preservando v1. Nenhum ajuste numérico de cena ou segundo diagnóstico especulativo. Atualizar prompts seguintes para vedar corrimão transversal no patamar.

**QA E09 superior:** imagem preservou terraço e planta em L, mas herdou entrada/tapete ao sul do térreo e escada clara com leitura ascendente. Correção delimitada autorizada pelo lote: fechar somente a parede sul e substituir leitura da escada por vão descendente, mantendo área de terraço/móveis. Gerar `e09-superior-terraco-v2.png`, preservar v1. A posição do vão no superior difere da projeção exata do térreo; alinhamento final entre pisos continua pendência de projeto/montagem e não será declarado resolvido por uma imagem.

O registro inicial de contagem e os checkpoints acima conservam sua sequência histórica. A situação vigente é o resultado entregue nesta seção: lote completo de arte/associações, com aprovação e integração ainda pendentes. Imagens aprovadas e arquivos existentes preservados.


### Resultado do estudo: cenas leves e montagem reaproveitada

Recomendacao para a arquitetura atual: uma cena leve por ambiente visitavel, com prefab base e template de cena compartilhados. Nao e regra uma cena por arquivo de imagem: um mesmo patamar ilustrado pode ser usado em tres cenas diferentes, preservando a identidade de cada predio. Portas/escadas ligam ambientes; nem toda imagem exige uma nova porta na rua. Duas portas da mesma casa podem chegar ao mesmo interior, com pontos de chegada/retorno distintos.

Fundamento no codigo consultado: `TransicaoDeCena` salva posicao e nome de cena antes de `LoadScene`; `RestaurarPosicao` restaura pelo nome da cena; `DadosDoJogo` persiste cena e XYZ, sem ID de casa/andar. Uma cena generica com troca de imagem exigiria tambem persistir/restaurar esse ID e preparar arte, colisoes, Player e camera na ordem correta, incluindo retorno de batalha. Portanto nao representa apenas substituir uma textura. Nenhuma dessas alteracoes foi implementada.

Prefab e cena sao complementares. O prefab proposto concentra estrutura comum: fundo via SpriteRenderer, raiz de colisoes, pontos de chegada e instancias de PortaTransicao. Cada ambiente configura seus limites, passagens e obstaculos. O template de cena facilita Player/camera e referencias iniciais; nao duplicar gerenciadores/Canvas persistentes. CameraFollow ja permite limites; a configuracao por sala continua necessaria. Nao criar PolygonCollider do contorno inteiro da imagem como substituto das paredes e moveis.

O numero de arquivos de cena nao significa carregar todas as casas juntas. A proposta mantem uma cena de exploracao por vez, conforme o fluxo atual; economia vem de referencias aos mesmos assets/prefabs e montagem comum. Nao ha medicao de desempenho desta proposta. Prefab unico que troca toda a casa pode ser reavaliado futuramente, se o volume de ambientes justificar a ampliacao do contrato de save.

Fontes Unity: [Prefabs](https://docs.unity.com/en-us/engine/6000.0/manual/working-with-gameobjects/prefabs/introduction), [Scene templates](https://docs.unity3d.com/6000.0/Documentation/Manual/scene-templates.html), [SceneManager.LoadScene](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager.LoadScene.html).

#### Associacao explicita por porta e ambiente

[Mapa de integracao](../../output/interiores-ferrovale/2026-10-28/mapa-integracao.json): 32 IDs de ambiente, 30 artes distintas, 15 edificios, 22 portas externas registradas e 10 ligacoes internas bidirecionais propostas. Cada ambiente tem seu edificio e imagem; cada acesso tem sua identidade. As 22 portas nao incluem as duas saidas da cidade. Coordenadas de chegada/retorno novas permanecem null ate montagem; nao substituir por zero. Os nomes de cena no JSON sao propostas, exceto CasaLip ja existente.

| Edificio | Portas externas registradas -> ambiente | Continuacao interna proposta |
|---|---|---|
| E06 - Casa 1 e anexo | `Porta_Casa01 -> Terreo` | Terreo <-> Superior; Anexo separado; acesso ainda nao confirmado |
| E05 - Casa 2 | `Porta_Casa02 -> Principal` | Sem ligacao interna definida |
| E10 - Casas 3 e 4 | `Porta_Casa03_Provisoria -> Casa03`; `Porta_Casa04 -> Casa04` | Sem ligacao interna definida |
| E12 - CasaLip — torre existente | `Entrada_CasaLip -> CasaLip` | Sem ligacao interna definida |
| E11 - Casas 5 e 6 | `Porta_Casa05 -> Casa05`; `Porta_Casa06 -> Casa06` | Sem ligacao interna definida |
| E09 - Casa maior em L | `Porta_CasaMaior -> Terreo` | Terreo <-> Superior |
| E08 - Casa com placa / duas entradas | `Porta_PredioPlaca_A -> Principal`; `Porta_PredioPlaca_B -> Principal` | Principal <-> Superior |
| E16 - Prefeitura — domo | `Porta_Prefeitura -> Civico` | Sem ligacao interna definida |
| E00 - Casa 7 — torre alaranjada | `Porta_Casa07 -> Hall` | Hall <-> Patamar; Patamar <-> Apartamento01 |
| E01 - Casa 8 — torre azul | `Porta_Casa08 -> Hall` | Hall <-> Patamar; Patamar <-> Apartamento01 |
| E02 - Casa 9 e garagem | `Porta_Casa09 -> Principal`; `Porta_Garagem -> Garagem` | Sem ligacao interna definida |
| E03 - Possível mansão / casa-chave | `Porta_Mansao_Provisoria -> Terreo` | Terreo <-> Superior |
| E04 - Geminado de telhado azul | `Porta_GeminadoAzul_A -> UnidadeA`; `Porta_GeminadoAzul_B -> UnidadeB` | Sem ligacao interna definida |
| E13 - Casa 10 — volumes azul e ocre | `Porta_Casa10 -> Hall` | Hall <-> Patamar; Patamar <-> Apartamento01 |
| E14 - Casa com porão | `Porta_CasaPorao_Esquerda -> UnidadeA`; `Porta_Porao -> Porao`; `Porta_CasaPorao_Direita -> UnidadeB` | Sem ligacao interna definida |

As geminadas Casas 3/4 e 5/6 mantem moradias independentes, conforme decisao do usuario. As demais propostas arquitetonicas e acessos incertos estao em `pending` no JSON; nao criar passagem inexistente para completar a tabela. Piso superior nao recebe porta de rua artificial: sua entrada e a escada da propria casa. E06-anexo permanece sem conexao confirmada; E02-casa/garagem possuem acessos externos independentes.

Proximo recorte indicado: piloto da Casa 3 (E10), uma porta e um ambiente. Importar uma copia da arte selecionada em Assets preservando o output; definir escala com Player real, enquadramento/camera, colisoes e dois pontos seguros de transicao. Testar City -> Casa03 -> City, salvamento/carregamento dentro da casa e preservacao do menu. Validar visualmente antes de replicar. Depois testar uma casa de dois pisos e um predio, incluindo retorno por escadas/porta de apartamento. Nao criar em lote 31 novas cenas antes desse piloto; 32 usos de ambientes sao inventario proposto, nao 32 cenas prontas.

Resultado desta rodada: associacoes salvas e verificadas por arquivo/referencia; nenhuma imagem foi movida ou sobrescrita, nenhuma cena/prefab/script foi montado ou conectado. Checagens confirmaram 32 IDs unicos, 15 edificios, 22 portas unicas, 30 arquivos de arte distintos existentes e 10 ligacoes internas com destinos validos. Teste comportamental e aprovacao visual no jogo pendentes. Unity nao foi operado nesta rodada; ultimo estado conhecido continua City fora de Play, sem confirmacao em tempo real. Backup anterior permanece em `output/backups/2026-10-28-portas-city/City.unity`; nenhum save foi acessado.


### Piloto autorizado da Casa 3 ? execu??o

Usu?rio autorizou a montagem proposta. Recorte: uma casa simples E10/Casa03, sem moradores, quests, escadas ou altera??o da CasaLip. Projeto confirmado por MCP: C:/Users/silve/DroidsCode/Assets, cena City limpa, fora de Play. Nova consulta mostrou mapa ativo metavalle-unificada-4k.png, que difere do estado da gera??o das artes; preservar integralmente esse trabalho atual.

Arquivos/objetos permitidos: nova imagem copiada para Assets/sprites/Mapas/Interiores/Casa03.png e respectivos metas/pastas; novos Assets/Prefabs/Interiores/InteriorBase.prefab e Casa03Interior.prefab; nova Assets/Scenes/Interiores/Interior_E10_Casa03.unity; adicionar somente essa cena ao EditorBuildSettings; somente destino/chegada e, se a confer?ncia geom?trica justificar, trigger da City/PortasMapeadas/Porta_Casa03_Provisoria. Documenta??o e estado do ambiente E10_Casa03 em mapa-integracao.json; evid?ncias/scripts/capturas em output/interiores-ferrovale/2026-10-28/piloto-casa03/. Demais objetos/cenas/prefabs compartilhados, scripts de gameplay, Canvas/Scaler, artes aprovadas e imagem 4K ficam preservados.

Depend?ncias: PlayerMovement/RestaurarPosicao e anima??o do Player da City ser?o reaproveitados por c?pia local; CameraFollow e ilumina??o compat?vel com URP 2D; PortaTransicao existente por inst?ncia; Canvas.prefab e EventSystem existentes conforme bootstrap das cenas atuais. O Canvas comum n?o ser? editado. Prefab base define estrutura; variante Casa03 define imagem/obst?culos/retorno. Player/c?mera ficam na cena.

Estado anterior e revers?o: snapshot de City, EditorBuildSettings e mapa-integracao.json em output/backups/2026-10-28-piloto-casa03/, com hashes no estado-anterior.json. Porta Casa03 tem destino vazio, chegada (0,0), destrancada, posi??o (2.55934167,-8.62464), trigger (0.5,0.1). Reverter somente o override dessa porta, retirar apenas a cena adicionada do build e remover os assets novos se solicitado; n?o substituir City inteira se houver progresso posterior. Originais novos poder?o ser removidos somente ap?s conferir refer?ncias. Nenhuma revers?o est? sendo executada.

Aceite: City -> Casa03 -> City sem repeti??o imediata; Player e m?veis em escala coerente; paredes/m?veis bloqueiam, acesso livre; c?mera sem cortes do mapa, menu ?nico e funcional; save isolado conserva cena/posi??o e recarrega o interior. Valida??o m?nima: verifica??o de montagem antes/depois, geometria de chegada/retorno, transi??es por trigger/f?sica e menu por API, render comparativo a 3840x2160; validar tamb?m enquadramento 1280x720. Teste por API n?o comprova input real nem aprova??o visual. Canvas atual confirmado ScreenSpaceOverlay/ConstantPixelSize/fator1, sem edi??o. Escala de visualiza??o da Game View n?o foi consultada; capturas de render independem dela.

Passos: (1) registrar backup e falha de aceite por aus?ncia da cena; (2) criar base/variante, arte importada e cena; (3) conectar apenas a porta Casa03 e build; (4) testar entrada/colis?es/retorno/menu/save isolado, registrar evid?ncias e deixar fora de Play. As escolhas locais ficam neste plano, sem processo paralelo de commits/worktrees: o MCP opera o projeto Unity j? aberto e o trabalho anterior n?o est? commitado. A valida??o usa c?digo de diagn?stico fora de Assets, sem adicionar c?digo de gameplay.


#### Resultado vigente do piloto Casa03

- **Editado e salvo fora de Play:** InteriorBase.prefab (estrutura neutra, sem copiar casa funcional), Casa03Interior.prefab (variante real do base, inst?ncia aninhada de PortaTransicao), cena Interior_E10_Casa03, c?pia Casa03.png, respectivos metas. Imagem original e c?pia importada t?m bytes id?nticos; importa??o Single Sprite, 1448?1086, PPU200, sem compress?o/mipmaps, Bilinear/Clamp. N?o ? uma nova arte 4K.
- Cena: variante do interior, Player com controles/anima??o/restaura??o j? existentes, c?mera CameraFollow com alvo correto, uma ilumina??o Global 2D, inst?ncia do Canvas.prefab compartilhado e EventSystem. Nenhum script/Canvas/Scaler compartilhado foi alterado. O singleton existente do menu preserva um Canvas em runtime, inclusive ao carregar o interior a partir do save.
- F?sica da variante: 13 colliders s?lidos para paredes, m?veis e limite ap?s a sa?da; 1 trigger de sa?da. Fundo n?o recebeu collider de toda a imagem. Chegada do Player (0,-1.92,0), sa?da para City com chegada (2.55934167,-9.87464,0). Porta_Casa03_Provisoria mant?m nome/Transform/trigger originais e recebeu somente tr?s overrides: nomeCenaDestino, pontoDeChegada.x/y. Nome ainda provis?rio ? preservado para n?o renumerar a coleta.
- City: compara??o byte a byte contra snapshot desta execu??o confirmou altera??o somente no bloco PrefabInstance 708079542 dessa porta; demais blocos e mapa 4K preservados. EditorBuildSettings recebeu somente a sexta cena habilitada; lista/configura??es anteriores intactas. JSON de integra??o alterado somente no ambiente/porta Casa03 e status global; outras 31 associa??es permanecem propostas.
- **Testado por API em Play:** entrada por deslocamento f?sico do Rigidbody atrav?s do trigger da City; chegada exata ao interior; parede leste e sof? bloqueiam; passagem lateral livre; sa?da f?sica para City e perman?ncia nela sem reentrada autom?tica; save v7 em arquivo isolado, leitura validada e recarga real do interior com restaura??o da posi??o; menu abre/fecha, libera controle e mant?m uma inst?ncia. Evid?ncias JSON em output/interiores-ferrovale/2026-10-28/piloto-casa03/.
- **Input real:** usu?rio informou inicialmente que n?o conseguia andar enquanto a rotina desabilitava temporariamente PlayerMovement. Consulta posterior confirmou componente habilitado, corpo simulado, Play sem pausa, timeScale1 e menu fechado. Usu?rio ent?o confirmou que conseguiu andar e testou. Rotina final tem finally para reabilitar movimento mesmo em falha. Isso confirma movimento por teclado; n?o presume teste manual de todas as portas/menu/save.
- **Valida??o visual:** captura do render real 3840?2160 e render somente da c?mera 1280?720 (mesmo aspecto16:9), sem mudar Game View/Canvas; mapa inteiro enquadrado e Player vis?vel. A imagem enviada pelo usu?rio mostra Game View UHD3840?2160, Scale0.24x. Ele observou a porta um pouco abaixo e reservou o ajuste visual para si: N?O alterar automaticamente arte, trigger, posi??o de porta ou enquadramento em resposta. Aprova??o visual final da montagem permanece com o usu?rio; a confirma??o de movimento n?o aprova todos os alinhamentos. Capturas: interior-3840x2160.png e interior-camera-1280x720.png.
- Diagn?stico de teste: primeira sonda de parede foi inv?lida porque posicionou o corpo dentro da cadeira de jantar, gerando contato simult?neo em passagem estreita. Nenhum valor de cena foi corrigido por essa falha; a sonda foi refeita a partir de posi??o livre, com guarda de overlap. Resultado: parede parou x2.7751, sof? parou x-1.6244 e corredor permitiu deslocamento. Registro em colisoes.json. Aviso de duas luzes globais ocorreu durante inspe??o aditiva de cenas no Editor; runtime consultado tem uma luz.
- **Saves e backups:** save real e eventual tempor?rio foram inventariados antes do Play; save.json existente copiado para output/backups/2026-10-28-piloto-casa03/save/. Hash do save real conferido inalterado ap?s testes, sem restaura??o por sobrescrita. save-teste.json e metadados permanecem isolados no output do piloto para reprodu??o; n?o s?o progresso do usu?rio nem limpeza pendente do save real. Backup de City/build/JSON anterior no mesmo diret?rio de backup, sem substituir os backups hist?ricos.
- **Estado entregue:** Unity deixado em Play no interior, Player habilitado, sem pausa e menu fechado, para continuar o teste iniciado pelo usu?rio. Nenhuma edi??o persistente foi salva em Play. A mudan?a em rela??o ao passo original de encerrar Play preserva o teste manual que o usu?rio iniciou. Ajustes permanentes da porta devem ser feitos fora de Play. ?ltima evid?ncia runtime em estado-final-runtime.json; n?o tratar isso como garantia de que o usu?rio n?o mudou o estado depois.
- Pr?ximo passo: usu?rio ajustar/avaliar a porta; aprovar escala/enquadramento/circula??o do piloto. S? depois replicar o padr?o nas outras casas e preparar piloto de escadas/pr?dio. N?o conectar automaticamente as demais 21 entradas. A c?mera da City tinha CameraFollow.target vazio na consulta inicial; foi preservada, n?o ? causa atribu?da ao movimento no interior nem corre??o inclu?da neste recorte.

Registro de navega??o autorizado neste mesmo recorte: atualizar somente um apontador no plano principal de portas/interiores para este resultado, preservando o planejamento independente do mapa externo e seus registros mais recentes.

Revis?o final independente, somente leitura: sem bloqueadores. Conferidos tr?s overrides exclusivos da City, ?nica cena adicionada no build, resolu??o de GUIDs/IDs e liga??o rec?proca, sem mudan?as rastreadas em Canvas/scripts compartilhados. Nenhuma nova revis?o em cadeia ou opera??o do Editor.
