# Moradores, vínculos e rotinas de Ferrovale

**Estado documental:** referência de distribuição/narrativa consolidada em 29/10/2026. A distribuição foi autorizada e usada na montagem; relógio, deslocamento e seleção de presença por identidade continuam pendentes.
**Objetivo:** conservar o motivo das ocupações, relações e exceções sem manter uma segunda descrição do estado instalado.

## Fontes responsáveis

- [Matriz de edifícios, ambientes e identidades](2026-10-29-refatoracao-nomenclatura-ferrovale.md): entrada para localizar quem é quem e preparar nomenclatura/presença única.
- [Montagem de 29/10](../historico/2026-10-29-montagem-cidade-interiores-npcs.md): representações e interações instaladas; [revisão vigente](2026-10-29-revisao-interiores-dialogos-colisoes.md): defeitos e validações pendentes.
- [Concepção dos interiores](2026-10-28-concepcao-interiores-ferrovale.md): fachadas, materiais e arquitetura; [contrato City](../contextos/cenas/city.md): interação e propostas de fala.
- [Proposta de ocupação por ambiente](../../output/moradores-ferrovale/2026-10-28/proposta-ocupacao.json), [inventário de elenco](../../output/moradores-ferrovale/2026-10-28/inventario-elenco.json) e [mapa visual de ocupação](../../output/moradores-ferrovale/2026-10-28/ocupacao-proposta.html): evidência datada do estudo; o JSON de proposta não é consumido pelo jogo.
- [Guia LPC](../guias/criacao-sprites-personagens-lpc.md): criação, receitas e limites; [cópia integral anterior](../../output/backups/2026-10-29-refatoracao-documentacao/lote-02/antes/docs/planos-implementacao/2026-10-28-moradores-e-rotinas-ferrovale.md): medições e cronologia.

## Distribuição: origem das escolhas

A tabela completa casa→ambiente→pessoa fica na matriz de identidade. As fachadas atribuídas são escolhas de design, exceto o vínculo existente Lip/Apollo–CasaLip; biografia e quest não determinam automaticamente o número da casa. Condição econômica não foi deduzida da aparência dos moradores.

| Núcleo | Justificativa e limite a preservar |
|---|---|
| E06 — mecânico | Torre e anexo de metal favorecem manutenção; figurante sem parentesco inventado |
| E05 — Sula/Pipo/crianças | Catálogo estabelece quatro pessoas, três crianças; duas portas servem à mesma família, sem duplicá-la |
| E10 — operário/Homem da fila | Casas3/4 independentes; operário ligado ao trabalho na Guilda, Homem da fila mantém encontro necessário no poço |
| E12 — Lip/Apollo | Vínculo pré-existente; não redesenhar o puzzle por causa da distribuição |
| E11 — Tácio/idoso | Casas5/6 independentes; Tácio tem comércio e visita Quirino; idoso genérico não é Onofre |
| E09 — Guilda | Espaço de triagem/tarefas/oficina para Bruna e operário; superior não vira residência canônica de Bruna |
| E08 — estalagem | Nilce e Zeca hóspede de manhã têm apoio nas fontes; quarto de Zeca indefinido e diferente do3 |
| E16 — edifício cívico | Usuário identifica prefeitura; enredo não define prefeito. Não inventar governante nem transferir assembleia da praça |
| E00/E01/E13 — torres | Reservadas a vendedor e moradores técnicos genéricos; modelos hacker/cientista não criam quest ou profissão funcional |
| E02 — Quirino | Casa/oficina com acessos externos independentes; relação espacial com o mercado ainda precisa de correspondência no mapa |
| E03 — casa vazia de Ivo | Foto, cadernos e bilhete são exigidos pelos diálogos; não colocar Ivo/Célia vivos para preencher a casa |
| E04 — Dara/vizinha | Duas moradias; terceira parte da cota é divisão de água, não terceiro apartamento |
| E14 — Benta/vigia | Duas unidades e porão comum; posto de Benta é junto à estalagem, não consultório subterrâneo. Vigia não é Zeca |

### Fontes e implicações para os interiores

- **Ivo:** `CityS2CasaIvoCadernos/Foto/Bilhete` no [catálogo de diálogos](../../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs) sustentam os pontos de história. Ivo permanece vinculado à Estação7; Célia é memória. Menção a cama não obriga equipar todas as casas como quartos.
- **Nilce:** [local estalagem](../../Assets/Scripts/Catalogos/CatalogoDeLocais.cs) descreve dois andares/seis portas; [q_nilce_quarto](../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs) exige quarto3 e bomba. Reservar o ponto não comprova seis quartos funcionais.
- **Bruna:** [catálogo de NPCs](../../Assets/Scripts/Catalogos/CatalogoDeNpcs.cs) e quests sustentam Guilda/triagem/forja. Decoração e moradia privada continuam questões distintas.
- **Dara:** terceira parte, cisterna e Emílio pertencem à quest/enredo; não ampliar unidades ou reescrever a ausência por conveniência.
- **Quirino:** oficina é descrita nos fundos do mercado; viaduto/mercado não foram inteiramente reconhecidos na arte. Não resolver essa lacuna com coordenadas inventadas.
- **Prefeitura:** praça da assembleia é externa; domo comunitário não transfere automaticamente a Sessão7.
- **Benta:** morar em E14 não desloca o toldo junto à E08. Modelo médico não a transforma em médica nem cria cura.

### Quem permanece fora dessas moradias

Rasha/Teodoro pertencem à Cratera/acampamento. Nadir e carregadores são visitantes conforme evento. Onofre dorme sob o viaduto; não lhe atribuir apartamento por obrigação. Célia/Emílio são memória/ausência, sem ator residente vivo. Artemis não recebe corpo/casa para ocupar espaço. A filha de Tácio não é transferida de Prumo para preencher unidade.

## Preparação das rotinas

O catálogo declara períodos e locais, sem demonstrar relógio, coordenadas, pathfinding ou travessia de cenas. Moradia, trabalho e visita são vínculos distintos. A mesma pessoa não pode permanecer representada em casa e no trabalho sem uma decisão de presença coerente.

| Pessoa | Base proposta | Percurso a preparar, sem XY inventado |
|---|---|---|
| Dara | E04-A | Casa → poço → mercado/cisterna do beco → casa; disponibilidade de quest tem prioridade |
| Tácio | E11-Casa5 | Casa → banca/filtro → dominó na oficina de Quirino → casa |
| Sula | E05 | Casa → banca leste → beco → casa; quest continua ligada à banca |
| Pipo | E05 | Casa/banca da Sula → carcaças → mercado; manter região de encontro |
| Homem da Fila | E10-Casa4 | Casa → poço → praça; não removê-lo da conversa necessária |
| Nilce | E08 | Balcão → corredor → porta3; mercado conforme catálogo |
| Zeca | E08, hóspede | Portão norte → descanso de manhã → portão; quarto ainda indefinido |
| Quirino | E02 | Casa → bancada/oficina → encontro com Tácio |
| Bruna | Guilda E09 | Porta/nomes → triagem/forja → ferro-velho → Guilda; moradia privada não definida |
| Benta | E14-A | Casa → posto junto à E08 → ferro-velho → posto/casa; retorno doméstico é proposta |
| Operário genérico | Casa3 | Sequência curta própria dentro da casa; depois, porta → Guilda → casa |
| Outros genéricos | Tabela | Pequenos ciclos diferentes, com pausas/orientação; roupa não cria loja, profissão funcional ou puzzle |

Primeira fatia candidata: operário genérico na Casa3, pontos explícitos, pausas e orientação; depois percurso para Guilda. Isso continua proposta, não rotina pronta. Pontos próprios oferecem controle; variação de tempo pode vir depois de validar passagem e interação.

Antes de movimento externo, conferir acessos e postos reais e a disponibilidade de quest/loja. Mudança de estado não deve remover interlocutor durante conversa, compra ou entrega. Relógio/navmesh/controlador global precisam de escopo próprio; seleção inicial de presença está no plano de identidade.

## Divergências preservadas

| Tema | Fontes divergentes | Tratamento nesta proposta |
|---|---|---|
| Quarto3 de Nilce | Background do NPC: filho Aurélio; q_nilce_quarto: abrigo de pessoas em fuga | Reservar quarto3 sem atribuir ocupante; conservar as falas da quest até revisão narrativa |
| Filha de Tácio | Background: Lúcia viva em Prumo; quest diz que ele sabe que ela não está lá; adendo B.2 menciona a cidade onde ela mora | Não criar a filha nem reescrever o desfecho; manter banca/cobre e casa de Tácio |
| Idades | Dara22/24, Ivo67/68, Nadir46/41, Rasha35/36, Teodoro48/52 entre catálogo/enredo | Não regenerar rosto por uma idade ainda divergente |
| Primeiro avistamento de Nadir | Catálogo menciona Zeca; enredo/diálogos dão o aviso a Pipo | Zeca pode vigiar; não substituir o evento de Pipo |
| Arte ativa | Tácio/Ivo e Homem da Fila/genérico diferem do elenco específico disponível | Corrigir apenas no recorte de montagem, preservando preferências do usuário |

A divergência de arte registra o estudo de 28/10; confirmar o vínculo persistente atual no inventário de identidade antes de trocar sprite. O adendo prevalece onde altera expressamente o cânone; divergência com falas existentes exige revisão do recorte, sem reescrita automática de história/quests/datas.

## Sprites e identidades

O inventário inicial encontrou 16 definições no catálogo e 30 conjuntos LPC (18 específicos e 12 genéricos). Não são 30 moradores: incluem variantes de Lip, Célia de referência, Apollo provisório e pessoas de outras regiões. Dimensões conferidas não comprovam Animator ou todos os quadros.

A lacuna inicial de Benta/Zeca foi sucedida pela produção registrada na montagem, junto de duas variantes infantis de Sula. As crianças são identidades distintas; não criar três Pipos idênticos. São derivações LPC com origem/receita, sem declarar desenhos originais do zero; aproximações de acessórios ficam no registro da montagem.

Genéricos selecionados no estudo: mecanico_01, operario_01, idoso_01, idosa_01 (vizinha), vendedor_01, hacker_01, cientista_01 e guarda_01. Reserva: morador_01, moradora_01, medico_01 e crianca_01. Disponibilidade de pasta não autoriza usar todos ou substituir pessoa nomeada por genérico.

Receita visual, nome exibido, ID técnico e moradia são registros distintos. Não apagar walk/idle/poses como “NPC duplicado”; investigar representações ativas por identidade. Conservar originais, créditos e proveniência do RuntimeFerrovale.

## Retomada

- Próximo passo: completar inventário da matriz de identidade, incluindo pontos externos/internos e escolha de presença; não redistribuir moradores por esta consolidação.
- Pendentes: rotina/horário, circulação e disponibilidade por estado; correspondência mercado/oficina/viaduto; decisões narrativas da tabela de divergências.
- Consolidação salva somente em documentação; não houve input, teste por API, inspeção nova ou aprovação visual de cenas.
- Unity não operado, cena/Play atuais desconhecidos; nenhum save ou backup de gameplay alterado. Registro anterior no backup vinculado; evidências existentes preservadas.
