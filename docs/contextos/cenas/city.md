# Contexto da City

Contrato específico da cena. Andamento, testes e aceite ficam no [plano da tarefa](../../planos-implementacao/2026-10-22-orientacao-npcs-city.md). Para outras cenas, consultar o contexto correspondente; não transferir os valores da City automaticamente.

## Identidade e limites

- Cena: [City.unity](../../../Assets/Scenes/City.unity). Atores e nomes no mundo pertencem à cena; a UI do MenuMundoManager pode persistir nas transições.
- Escala local padrão definida pelo usuário para os NPCs da City: **X = 1.33, Y = 1.33**. Z dos três atores existentes permanece 2. Não redimensionar o protagonista para compensar diferenças visuais.
- Preservar GUIDs, sprites, posições e configuração local dos colliders/gatilhos ao ajustar apresentação. A escala do ator também afeta os filhos e o alcance físico dos colliders; não alterar os valores locais destes como efeito secundário.

## Atores e conteúdo

| Objeto na cena | Nome no mundo | Folha de poses | Fala principal / genérica | Retrato em Resources/Retratos |
|---|---|---|---|---|
| Dara | Dara | Characters/NPCs/Dara/animations/idle.png | city_s2_dara / city_generico_dara | Dara.png |
| homem da fila | Homem da fila | Characters/NPCs/Generic/morador_01/spritesheet_idle.png | city_s2_homem_fila / city_generico_homem_fila | Homem da fila.png |
| Tácio | Tácio | Characters/Antagonists/Ivo_Rangel/animations/idle.png | city_s2_tacio / city_generico_tacio | Senhor Tácio.png |

As folhas estão em `Assets/sprites/Entities/`. A arte de Tácio continua usando a folha de Ivo; não trocar a identidade visual sem escopo próprio. Os três têm OrientacaoNpc no mesmo objeto do gatilho: poses cima/esquerda/baixo/direita são os quadros 0/2/4/6. A pose frontal é a usada no retrato.

## Nomes no mundo

- Hierarquia padrão: `NPC/NomeNPC/Nome`. NomeNPC é um Canvas **WorldSpace**, filho do NPC, e não um Canvas Overlay preso à tela. O TMP existente é preservado para manter as referências de interação.
- Canvas local: área 250×32, escala uniforme 0.004, rotação identidade, centralizado em X. Sorting layer do SpriteRenderer, ordem do sprite + 10.
- TMP: fonte existente, tamanho 24, centralizado, uma linha sem quebra, sem raycast e com área esticada no Canvas. Não editar o material compartilhado da fonte para estilizar um único nome.
- Posicionamento: abaixo da maior altura das quatro poses, com folga local 0.04 entre os pés e o topo da área de nome. Centro Y local do contêiner: Dara -0.354; homem da fila -0.374; Tácio -0.349.
- Visibilidade por proximidade/disponibilidade preservada: Dara/fila usam `GatilhoDeDialogo.indicadorDeInteracao`; Tácio usa `NpcInterativo.indicadorDeInteracao`. A referência aponta ao TMP Nome, não ao Canvas dos menus.
- **Loja de Tácio:** continua em `Tácio/Canvas/Loja`, com Canvas ScreenSpaceOverlay separado. O nome fica no novo `Tácio/NomeNPC`. Não converter o Canvas da loja para WorldSpace nem herdar a configuração do nome.

## Diálogo e interface

- Montagem de referência: diálogo da CasaLip, com moldura `Assets/sprites/Atualizados/Tela_de_dialogo-removebg-preview.png`, miniatura frontal do falante, nome e fala ao lado. Não alterar a CasaLip para ajustar a City.
- Ramo local: `Canvas/AreaDialogo/PainelDialogo`. EscalaLocalDeDialogo mantém coordenadas virtuais 1920×1080, Match 0.5; a adaptação é exclusiva do diálogo. Painel com escala local 1, fonte 48, retrato com preserveAspect e AspectRatioFitter da referência.
- Canvas compartilhado da City permanece ConstantPixelSize. Não mudar seu Scaler como forma de corrigir diálogo ou nome de NPC; isso afeta os outros painéis.
- Retratos seguem o nome exato do **falante do catálogo**, que pode diferir do nome do objeto (Tácio fala como “Senhor Tácio”). DialogoUIManager consulta a lista manual e depois `Resources/Retratos/<Falante>`. O cache deve se inicializar sob demanda; ausência de cache não pode interromper miniatura/fala.
- Prioridade: fala principal disponível → fala genérica repetível quando principal indisponível/já vista. A genérica não grava flags/save nem consome a principal. Em Tácio, a loja abre depois da fala. O frame de fechamento não reutiliza a mesma tecla para abrir outra conversa.
- Dara/fila usam GatilhoDeDialogo AoInteragir e NpcInterativo desabilitado. Tácio usa NpcInterativo Loja e gatilho ApenasPorScript. Não criar duas origens para a mesma tecla.

## Mapa e transições

- Acessos novos ficam no grupo local `PortasMapeadas`, como instâncias de `Assets/Prefabs/PortaTransicao.prefab`, separadas dos contornos dos prédios. Destino vazio representa montagem ainda sem interior conectado; não equivale a porta utilizável nem a trava narrativa. Identidades/coordenadas permanecem na [coleta](../../planos-implementacao/2026-10-17-auditoria-cenas.md#coleta-de-entradas-da-city--28102026); decisões, validação e melhoria das artes ficam no [plano de portas, interiores e mapas](../../planos-implementacao/2026-10-28-portas-interiores-e-mapas.md).
- Fundo: `Assets/sprites/Mapas/metavalle-original-upscale-8k.png`, teste de super-resolução 4× da fonte inteira, 7680×4320. Sprite metavalle_0: rect (60,0,7560,4320), PPU 562.5, preservando área 13.44×7.68 em unidades locais e transformação do mapa (escala 3; 40.32×23.04 no mundo). Original `metavalle.jpg`, montagem anterior `metavalle-unificada-4k.png` e seus `.meta` preservados. Importação técnica conferida em 28/10; nitidez/acabamento no jogo aguardam validação do usuário. Fumaça já pintada na fonte continua pendente, conforme plano de portas, interiores e mapas.
- Fluxo: CasaLip ↔ City ↔ Game. Condições narrativas e configuração dos gatilhos/portão estão na [montagem do Ato 1, seção City](../../guias/montagem-ato-1.md#4-city-conversas-e-portão).
- `SaidaCidade/Canvas/desativado` fica inativo e desvinculado de TransicaoDeCena.textoDeBloqueio. Não reativar o aviso antigo como etiqueta do personagem.

## City viva — análise em construção (22/10/2026)

### CV-01 — base encontrada e limites

**Status:** levantamento documental; propostas abaixo não são montagem aprovada ou conteúdo já integrado. Preservar os contratos técnicos anteriores.

- Fontes: [NPCs](../../../Assets/Scripts/Catalogos/CatalogoDeNpcs.cs), [locais](../../../Assets/Scripts/Catalogos/CatalogoDeLocais.cs), [quests](../../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs), [falas](../../../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs), [Ferrovale no enredo](../../enredo/TCC_-_ENREDO_v2.md#iii5--ferrovale) e [adendo](../../enredo/TCC_-_ENREDO_v2_ADENDO.md). O adendo prevalece.
- Montagem conhecida: Dara, homem da fila e Tácio. Elenco disponível para expansão de Ferrovale: Mestra Bruna, Nilce, Quirino, Sula, Pipo, Seu Onofre, Benta e Zeca; Nadir depende da chegada narrativa. Ivo participa do arco da estação; Rasha/Teodoro pertencem ao arco externo, sem presumir residência na City.
- Existem locais catalogados para poço, mercado, barraca, oficina, estalagem, posto, guilda, ferro-velho, beco, praça, portão, casa da Dara e mirante. Um local narrativo não comprova prédio acessível ou área montada. Ainda falta associar esses IDs ao mapa aprovado, sem renomear visualmente construções por suposição.
- `Rotina` registra quatro períodos e atividades por personagem. Busca por `PeriodoDoDia`, `PontoDeRotina`, `PresentesEm` e `LocalDe(` em Assets/Scripts não encontrou consumidor fora do próprio catálogo de NPCs. Isso sustenta planejar rotina como design, sem prometer relógio, deslocamento ou troca automática de atores.
- Falas de apresentação já existem para Bruna, Pipo, Onofre, Nilce, Quirino e Sula (`city_s2_*`); genéricas repetíveis da City atualmente cobrem apenas os três atores montados. Ter fala no catálogo não comprova gatilho, retrato, serviço ou quest ligados à cena.
- Reflexão: dar a cada pessoa um trabalho observável e uma relação com alguém cria mais identidade que preencher ruas com cópias. Genérico significa participação opcional, não fala sem personalidade; nem todo figurante precisa oferecer uma quest.

**Conflitos a resolver antes de implementar conteúdo:**

1. O adendo estabelece Apollo como um entre 431 núcleos e censo como busca burocrática ampla. Não escrever rumores que confirmem um escolhido ou perseguição exclusiva a ele, mesmo que descrições antigas sugiram isso.
2. `q_tacio_linha` descreve oferta depois da terceira entrega, mas declara somente `quest_tacio_cobre_ok` como pré-requisito. Conferir contador/consumidor antes de prometer três entregas na experiência.
3. A história do Geiger varia entre o segredo do perfil de Tácio e a curiosidade de `q_cobre_tacio`; não transformar nenhuma versão em nova fala factual antes de conciliar a cronologia.
4. O quarto três de Nilce tem explicações diferentes entre perfil e curiosidade de `q_nilce_quarto`. Preservar o mistério; não inventar solução conciliadora.
5. Enredo chama a saída para a Cratera de portão leste; catálogo traz `loc_portao_norte` e Zeca nesse posto. Não concluir que são o mesmo portão nem mudar a saída atual. Decidir a correspondência no mapa antes de posicionar o vigia.
6. `PapelDeNpc` descreve profissões mais amplas que os tipos implementados em `NpcInterativo` (Generico/Loja/Dialogo). O tipo Generico ainda é placeholder com log; não basta escolhê-lo para ter conversa. Estalagem/guilda/ferreiro catalogados não comprovam serviço funcional ao interagir.

### CV-02 — proposta de ocupação por lugar

**Proposta do agente para discussão:** construir encontros pequenos com propósito; preencher progressivamente, sem tentar representar literalmente os cerca de 700 moradores do enredo. Coordenadas e quantidade visível simultânea dependem de leitura posterior do mapa/Game View. Os locais abaixo são âncoras narrativas, não novos interiores autorizados.

| Núcleo / IDs de local | História e serviço | Genéricos propostos | O que faz o lugar parecer habitado |
|---|---|---|---|
| Poço e praça: `loc_poco`, `loc_praca` | Dara + homem da fila; Nadir só no evento próprio | Uma moradora com recipientes; o homem da fila já conta como figurante, sem duplicá-lo | Esperar, contar litros, guardar vez; praça pode receber assembleia depois |
| Mercado: `loc_mercado`, `loc_barraca_sula` | Tácio, Sula, Onofre no pilar | Um carregador de peças | Compra, trabalho e conversa curta; rivalidade comercial percebida sem explicação longa |
| Oficina: `loc_oficina` | Quirino | Nenhum inicialmente; carregador pode mencionar o conserto | Bancada, peça pendente, encomenda coberta; preservar passagem e interação |
| Guilda/pátio: `loc_guilda`, `loc_ferro_velho` | Bruna | Uma triadora de sucata | Separar material e conferir peso; trabalho da cidade continua sem depender de Lip |
| Estalagem/posto: `loc_estalagem`, `loc_posto_benta` | Nilce e Benta | Nenhum obrigatório no primeiro lote | Porta reconhecível, atendimento e sinais de limpeza; serviços só depois de integração comprovada |
| Beco: `loc_beco_carcacas` | Pipo | Nenhuma criança extra no primeiro lote | Brincadeira com carcaça e observação do céu; criança não existe só para vender missão |
| Cerca e saída: `loc_portao_norte` (orientação pendente) | Zeca | Nenhum vigia duplicado | Observar estrada, sino e mudança de tensão; não bloquear caminho do jogador |
| Residências/mirante: `loc_casa_lip`, `loc_casa_dara`, `loc_mirante` | Referências pessoais e cenas reservadas | Sem aglomeração inicial | Respiro, silêncio, retorno e contraste com mercado; não povoar cada espaço vazio |

**Elenco permanente candidato:** os três atuais mais Bruna, Quirino, Nilce, Sula, Pipo, Onofre, Benta e Zeca (11 papéis, não 11 atores necessariamente ativos no mesmo enquadramento). **Figurantes novos iniciais:** moradora do poço, carregador e triadora (3). Nadir e acompanhantes são evento posterior, não população inicial. Essa é uma hipótese de composição, sem meta de encher a tela.

Há pastas visuais genéricas `moradora_01`, `operario_01`, `mecanico_01`, `medico_01`, `guarda_01` e outras. Isso é disponibilidade de arquivos, não aprovação de aparência/importação. Candidatos: moradora do poço → `moradora_01`; carregador → `operario_01`; triadora → avaliar variante existente sem confundir silhuetas. Não transformar `medico_01` automaticamente em Benta: ela explicitamente não se apresenta como médica. Benta/Zeca precisam de correspondência visual conferida; catálogo visual também contém caminhos antigos e não é prova de integração.

**Relações para aparecerem no espaço/falas:** Tácio/Sula disputam clientela; Tácio/Quirino jogam dominó; Nilce acolhe Zeca; Benta atende gente ferida no trabalho; Pipo observa o que adultos ignoram. Revelar relações em detalhes pequenos, sem todos os NPCs saberem tudo sobre o enredo.

### CV-03 — rotina sem perder acesso aos personagens

**Escolha proposta, ainda não aprovada:** primeira montagem com postos estáveis e atividade sugerida por pose/objeto/fala. Segunda etapa pode variar estado por momento narrativo. Relógio contínuo e deslocamento entre locais ficam para tarefa própria, caso o usuário queira. Nenhum desses mecanismos foi implementado nesta análise.

| Personagem/grupo | Rotina catalogada a aproveitar | Primeira representação viável / dependência |
|---|---|---|
| Dara | Poço madrugada/manhã; mercado à tarde; casa à noite | Manter encontro do poço no arco da cota. Mudança de endereço exige garantir acesso ao diálogo obrigatório |
| Tácio | Banca manhã/tarde; dominó com Quirino à noite | Banca estável primeiro; versão noturna não pode desaparecer com a única loja disponível |
| Bruna | Guilda manhã/noite; pátio à tarde | Um posto inicial com triagem; não criar duas Brunas ativas para simular rotina |
| Nilce | Estalagem manhã/noite; mercado à tarde; corredor de madrugada | Permanecer acessível na entrada até interior/descanso estarem comprovados |
| Quirino | Oficina manhã/tarde/noite | Solda e atendimento no mesmo lugar; dominó futuro é variação de atividade, não quest nova |
| Sula | Banca manhã/tarde; beco à noite | Posto de venda inicial; retirada noturna depende da política de disponibilidade da loja |
| Pipo | Beco manhã; mercado tarde; banca da Sula noite | Um ponto inicial no beco; não prometer escolta andando porque objetivo de quest diz “ir com” |
| Onofre | Segue sombra entre pilares; dorme ali | Posto fixo já comunica hábito; deslocamento curto pode ser posterior |
| Benta | Posto manhã/noite; ferro-velho tarde | Atendimento sugerido; ida ao acidente exige evento/posição alternativa antes de simulação |
| Zeca | Portão tarde/noite/madrugada; estalagem manhã | Não mostrar dormindo e de guarda ao mesmo tempo; portão exato ainda pendente |
| Três genéricos novos | Ainda sem rotina no catálogo | Moradora: fila/retorno; carregador: espera por carga/entrega; triadora: separação/fechamento. Iniciar em posto fixo, sem trajetos ficticiamente funcionais |

**Estados narrativos candidatos:** antes da saída (escassez e suspeita), retorno da estação (água/ausências/consequências), chegada da Nadir (assembleia/censo). Associar a flags efetivamente validadas numa implementação futura; período do catálogo e avanço da história não são sinônimos. Não inferir horário pela quantidade de minutos jogados.

**Contrato futuro de disponibilidade:** conversa aberta congela orientação/atividade; troca de estado não remove interlocutor durante diálogo, loja ou entrega; retorno restaura pose adequada. Cada NPC obrigatório tem um ponto alcançável ou substituto explícito. Implementação de rotina precisará validar passagem, colisões, save/load e retorno de cena, com backup de saves se houver gravação. Não registrar esses itens como capacidades atuais.

### CV-04 — sidequests, segredos e pequenas descobertas

**Direção proposta:** aproveitar as quests já escritas antes de criar mais tarefas de coleta. O jogador vê uma necessidade, pode perguntar e decide ajudar. Fala ambiente não aceita missão automaticamente nem transforma todo morador em marcador de tarefa. Conteúdo lateral permanece opcional; não usar novas sidequests para resolver implicitamente o gate pedagógico da saída.

| Núcleo | Conteúdo já catalogado e condição declarada | Preparação na cidade / cuidado |
|---|---|---|
| Dara | `q_agua_dara` após `ato1_falou_dara`; `q_dara_terceira` após `quest_dara_filtro_ok` | Balde/filtro e preocupação prática primeiro. Irmão e terceira parte são aprofundamento, não frase genérica de boas-vindas |
| Tácio | `q_cobre_tacio` após `ato1_falou_tacio`; `q_tacio_linha` após `quest_tacio_cobre_ok` | Pedido por cobre introduz hábito; linha de 18 km é descoberta. Resolver divergência das três entregas antes de integrar |
| Pipo | `q_pipo_peca` após `ato1_sessao2` | Brincadeira e observação tornam informação crível. Texto pede bateria e sugere acompanhamento, mas objetivo declarado é conversar: não prometer consumo de item/escolta sem conferir integração |
| Bruna | Cadeia `q_guilda_1_iniciacao` → `_2_cacada` → `_3_alfa` → `_4_verdade`; inicia após `ato1_sessao2` | Porta/nomes/pesagem preparam vínculo; missões externas não precisam caber fisicamente na City. Não despejar quatro ofertas no primeiro E |
| Nilce | `q_nilce_quarto` após `ato1_sessao3` | Bomba e hospitalidade podem introduzir necessidade; conteúdo do quarto depende da decisão de cânone pendente |
| Onofre | `q_onofre_provas` após `ato1_faccao_escolhida` | História da torneira primeiro; provas da Cratera aprofundam memória e geografia, sem substituir investigação principal |
| Quirino | `q_perna_apollo` após `ato1_sessao6` | Conserto anunciado no início ganha retorno posterior; não executar reparo antecipadamente por oferta genérica |
| Sula | `q_sula_divida` após `ato1_nadir` | Comércio e discrição primeiro; delação não deve aparecer numa repetição antes da chegada de Nadir |
| Ivo | `q_ivo_terca` após `ato1_sessao6` | Bilhete da bomba prepara cuidado/manutenção; presença na City depende da resolução narrativa, não de população genérica |

Essas condições são leitura do catálogo, não teste de oferta/entrega no jogo. `GerenciadorDeQuests` possui operações de progresso, mas não foi demonstrada a ligação delas a cada interlocutor. Inventário, consumo de itens, contagem de conversas, recompensas, repetibilidade e preservação do save precisam de validação específica na futura fatia de quest.

**Três tipos de descoberta, com tratamentos diferentes:**

| Tipo | Conteúdo / fonte | Como apresentar sem antecipar resposta |
|---|---|---|
| Pista canônica | Vulto coberto na oficina — adendo A.4, pista 7 | Examinável opcional, descrição cotidiana de encomenda/projeto. Não confirmar outro núcleo nem resolver no Ato 1 |
| Pista canônica | Cadernos, foto e bilhete da casa de Ivo — `city_s2_casa_ivo_cadernos`, `_foto`, `_bilhete` | Já há texto; falta demonstrar acesso/objetos. Não inventar interior para encaixá-los nesta rodada |
| Segredo pessoal | Terceira parte de Dara; linha de Tácio; venda de Sula | Revelação por relação/quest e momento narrativo. Figurante não comenta informação que não teria como conhecer |
| Memória coletiva | Onze nomes da Guilda; relato de Onofre; torneira seca | Pequena observação em local reconhecível; texto é recompensa de explorar, sem exigir flag nova para cada detalhe |
| Easter egg proposto, não canônico | Placa de oficina: “Funciona. Não encoste.” | Humor sobre reparos de Ferrovale, sem pista falsa da trama ou recompensa obrigatória |
| Easter egg proposto, não canônico | Carcaça de Pipo com riscos “versão 1”, “versão 2”, “agora vai” | Eco do tema aprender com erro; só inspeção, sem minigame obrigatório |
| Easter egg proposto, não canônico | Placar do dominó com Tácio contestando a conta | Referência à relação já catalogada; não alterar preço da loja como efeito oculto |

**Reflexão:** segredo não é sinônimo de loot. Uma cidade viva recompensa atenção com relações, memória e mudança percebida. Evitar transformar trauma de Dara/Nilce em piada ou esconder informação essencial atrás de um easter egg. Inicialmente escolher apenas uma pista e uma brincadeira para avaliar tom e densidade.

### CV-05 — revisão de falas e primeiro rascunho

**Diagnóstico de cobertura:** apresentações existem para seis moradores ainda não comprovados na cena; faltam genéricas próprias para eles e para os novos figurantes. Benta/Zeca não têm apresentação `city_s2_*` identificada na revisão da City. As três genéricas atuais funcionam como fallback, mas Dara (“Se precisar de mim...”) pouco caracteriza sua preocupação; Tácio já fala como vendedor. Não substituir nenhuma fala implementada sem revisão do texto e da condição de uso.

**Ordem atual que deve ser preservada:** principal disponível → genérica configurada; Tácio abre loja depois. **Ordem futura proposta:** evento obrigatório disponível → entrega/oferta/continuação de quest escolhida conscientemente → assunto opcional de história → genérica compatível com estado → serviço, quando aplicável. Essa arbitragem de múltiplos assuntos ainda não existe como capacidade demonstrada. Deve haver uma única origem de interação, uma resposta por ação e serviço acessível; não abrir uma sequência automática de monólogos a cada E.

Oferta/entrega deve reaproveitar `TextoDeOferta`/`TextoDeEntrega` do catálogo de quests quando cabível, evitando cópias divergentes. Definir adaptador e prioridade é uma tarefa de implementação futura. Não confundir nova flag narrativa com ID de diálogo: o catálogo armazena texto, não garante condição de disparo. Genérica não libera flags, não aceita quests e não consome a principal.

**Textos abaixo são propostas editoriais, ainda fora do catálogo de código.** IDs novos precisam de confirmação de unicidade antes da futura edição. Voz segue o perfil de cada personagem. Novas falas usam miniatura frontal e nome exato do falante; retrato ausente é impedimento da montagem, sem quadrado branco como resultado aceitável.

| ID existente ou proposto | Falante | Rascunho / uso |
|---|---|---|
| `city_generico_dara` — existe; revisão proposta | Dara | “Põe o balde na sombra. O pouco que tem precisa durar.” Apenas estado de escassez; após retorno da água precisa de variante |
| `city_generico_homem_fila` — existe; alternativa proposta | Homem da fila | “Pode passar. Só não perde a conta de quem chegou primeiro.” Não repete acusação a Ivo fora de contexto |
| `city_generico_tacio` — existe; manter por enquanto | Senhor Tácio | “Dê uma olhada. Talvez eu tenha o que você precisa.” Já é fallback neutro compatível com acesso à loja |
| `city_generico_bruna` — proposto | Mestra Bruna | “Separa o cobre do ferro. Teu trabalho começa aí.” Sem aceitar missão ou conceder filiação |
| `city_generico_quirino` — proposto | Quirino | “Faz barulho quando esquenta? Então não tá consertado.” Diagnóstico cotidiano, sem revelar procedência de Apollo |
| `city_generico_nilce` — proposto | Nilce | “Veio descansar, meu filho? Veio sim. Até tua sombra tá cansada.” Fala de hospitalidade; não promete mecânica de descanso pronta |
| `city_generico_sula` — proposto | Sula | “O preço tá na peça. Conversa não entra no troco.” Comércio sem expor delação |
| `city_generico_pipo` — proposto | Pipo | “Vrum tchk essa aqui só vira pra esquerda por isso eu estaciono em círculo.” Brincadeira/voz rápida; no catálogo pode ser dividida para legibilidade |
| `city_generico_onofre` — proposto | Seu Onofre | “Essa sombra? Já tava aqui antes da banca, moço. Eu só acompanho.” Hábito cotidiano sem outra exposição longa sobre a Cratera |
| `city_generico_benta` — proposto | Benta | “Senta. Mostra a mão. Ferrugem pequena também corta.” Apenas atendimento sugerido; sem curar/consumir item automaticamente |
| `city_generico_zeca` — proposto | Zeca | “Viu alguma coisa vindo? ...Espera. Não. É poeira.” Só no posto de vigia, depois de definir o portão |
| `city_generico_moradora_poco` — proposto | Moradora do poço | “Esse risco no galão é pra ninguém precisar adivinhar.” Sem números incompatíveis entre fases |
| `city_generico_carregador` — proposto | Carregador | “Parece leve até a terceira viagem. Abre um espaço aí?” Não obriga jogador a empurrar ou abrir caminho |
| `city_generico_triadora` — proposto | Triadora | “Parafuso bom vai nessa lata. O torto vira gancho.” Material comunica economia de reaproveitamento |

**Reação de retorno, rascunho sem ID/flag definidos:** Dara: “Agora espera a água assentar. Não enche tudo de uma vez.”; moradora: “Dá pra ouvir no cano antes de chegar.” Ativar somente quando houver estado confirmado de abastecimento/restabelecimento; não usar conclusão genérica de qualquer quest como substituto. Para Nadir/assembleia, priorizar falas existentes `city_s7_*` e evitar improvisar confissão pública de Sula.

**Cobertura a completar por NPC antes de montagem:** apresentação ou principal; genérica antes/depois do principal; bloqueio de quest sem spoiler; oferta; progresso incompleto; entrega; pós-conclusão; reação ao estado da cidade; retrato pelo nome do falante. Nem todos precisam de todos os estados, mas ausência deve ser consciente. Para figurantes simples, uma genérica coerente basta no primeiro lote; variação aleatória é opcional e não foi implementada.

**Reflexão:** a mudança mais importante não é volume de linhas. É o jogador voltar e perceber que alguém lembra do problema resolvido enquanto outras pessoas continuam suas vidas. Começar com duas reações bem condicionadas vale mais que muitas falas que contradizem o momento atual.

## Fontes e validação

- Contratos compartilhados: [interface](../sistemas/interface.md), [estado e save](../sistemas/estado-e-save.md).
- Alterações visuais exigem registrar resolução/zoom e observar enquadramento, legibilidade e ausência de sobreposição. Propriedade salva não equivale a input real nem à aprovação visual do usuário; resultados pertencem ao plano.
- Pesquisa restrita: objetos Dara, homem da fila, Tácio e seus filhos em City.unity; GatilhoDeDialogo, NpcInterativo, OrientacaoNpc, DialogoUIManager e EscalaLocalDeDialogo somente se a tarefa envolver esses contratos.
