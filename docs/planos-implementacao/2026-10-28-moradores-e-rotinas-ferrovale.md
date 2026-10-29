# Moradores, serviços e preparação de rotinas de Ferrovale

## Objetivo e escopo

**Atualização de 29/10:** a distribuição foi autorizada para montagem. Seguir o [plano de execução vigente](2026-10-29-montagem-cidade-interiores-npcs.md) para cenas/portas/NPCs salvos e limites do teste. Este documento conserva a origem das escolhas; as declarações abaixo de “não implantado” descrevem a análise anterior. Benta/Zeca e variantes infantis foram produzidos no lote seguinte; relógio e rotinas de caminhada continuam pendentes.

Antes de remarcar as portas sobre a imagem editada da City, cruzar diálogos de história/quests, catálogo de NPCs/locais, enredo pertinente e sprites existentes para propor quem ocupa cada edifício. A atribuição a um prédio é proposta de design quando a fonte só define um lugar abstrato. Diferenciar moradia, trabalho/serviço, visita e personagem de outra região; não transformar todo NPC em morador da cidade.

Arquivos permitidos nesta rodada: este plano; novo `docs/guias/criacao-sprites-personagens-lpc.md`; apontadores em `docs/indice-trabalho.md` e no plano de portas/interiores; inventário e evidência de leitura em `output/moradores-ferrovale/2026-10-28/`. As fontes consultadas, os catálogos C#, sprites/receitas/metas, cenas, prefabs, mapas e saves ficam em leitura. Não mover portas, atribuir componentes/rotinas no Unity, gerar ou substituir personagens nesta análise. Se faltar arte, registrar a necessidade e a receita candidata antes de uma produção específica.

Dependências: tabela de edifícios E00…E16 e seus acessos na [concepção de interiores](2026-10-28-concepcao-interiores-ferrovale.md); diálogos, quests e locais em `Assets/Scripts/Catalogos/`; elenco LPC em `Assets/sprites/Entities/Characters/`; modelos anteriores em `Assets/sprites/Entities/Npc's/`. O texto anexado pelo usuário é referência do método de criação, não autorização para apagar ou regenerar o acervo inteiro.

Aceite: cobertura dos 15 edifícios com atribuições justificadas, residentes/não residentes separados, relação real de sprites e lacunas, coerência entre família/quests/localização, preferências do usuário preservadas e guia de criação com comandos/limitações reais. Validação mínima: referências locais e contagens, cruzamento dos IDs existentes e conferência visual do mapa ativo. Aprovação das novas atribuições e implementação das rotinas ficam pendentes; uma rotina declarada no catálogo não comprova movimentação no jogo.

Estado anterior e reversão: somente novos documentos/inventários e links, sem mutação do Editor. Remover apenas os arquivos novos e apontadores desta rodada se solicitado; nunca substituir as fontes ou restaurar um mapa antigo por causa deste planejamento. Assets e instruções existentes devem permanecer preservados.

## Referência capturada antes da remarcação

Consulta MCP em leitura em 28/10/2026, 23:53:51 de São Paulo (UTC 29/10, 02:53:51): projeto correto, City limpa em Play. Player em **(2.56479287, -8.663213, 0)**, indicado pelo usuário como referência visual atual da entrada da Casa 3. Porta_Casa03_Provisoria permanece em **(2.55934167, -8.62464, 0)**, destino Interior_E10_Casa03 e chegada (0,-1.92).

Mapa ativo consultado: `Assets/sprites/Mapas/metavalle-original-upscale-8k.png`, textura 7680×4320. Isso substitui a imagem ativa vista no piloto anterior; não reutilizar uma marcação de pixels da versão 4K sem reconciliar Transform/rect/PPU. A referência do Player não é automaticamente o centro do trigger: o collider do personagem possui offset e deve entrar no cálculo quando a remarcação for executada. Nenhuma porta foi movida nesta rodada.

## Sequência de trabalho

1. Registrar posição sem alterar Play/Player e inspecionar o mapa vigente.
2. Extrair elenco, serviços, relações, horários e gatilhos dos catálogos de diálogo/quest/NPC/local.
3. Conferir sprites específicos e genéricos, receitas, cobertura de animações e ferramenta LPC existente.
4. Propor ocupação dos 15 edifícios, incluindo serviços e espaços reservados; identificar mudanças de interior necessárias para quests.
5. Documentar criação modular reutilizável para NPCs e limites para inimigos/droids; verificar arquivos/links e entregar decisões para avaliação.

## Resultado do cruzamento

[Inventário verificável](../../output/moradores-ferrovale/2026-10-28/inventario-elenco.json): 16 definições em CatalogoDeNpcs, 18 montagens específicas e 12 genéricas. As 18 não representam 18 moradores: incluem Lip com mochila, Célia para referência, Apollo provisório e pessoas de outras regiões. Todos os 30 conjuntos têm sprite64×64 e walk576×256; dez específicos não têm idle nativo. Conferência de dimensões e arquivos não é teste de todos os quadros ou Animator.

Faltam sprites próprias de **Benta** e **Zeca** no acervo inspecionado. medico_01 e guarda_01 são modelos genéricos, não identidade aprovada dos dois. As duas outras crianças da família de Sula podem permanecer fora de quadro; se aparecerem, preparar variantes individuais, sem criar três Pipos idênticos. Não falta figurante para iniciar o piloto.

Leitura atual da City: Tácio usa idle de Ivo; Homem da Fila usa idle de morador_01; Dara usa arte própria. Há modelos específicos de Tácio/Homem da Fila disponíveis. Essa divergência de montagem foi registrada, sem trocar sprites.

## Distribuição proposta por edifício

**Estado: proposta para avaliação; nenhum morador ou rotina implantado.** Todos os vínculos de fachada abaixo são escolhas de design do agente, exceto Lip/Apollo na CasaLip existente. A história sustenta relações e locais de trabalho, mas não a numeração das casas. A condição econômica não foi inferida da aparência física dos moradores.

A proposta aproveita edifícios grandes para funções já presentes nas fontes e preserva casas comuns para figurantes. As artes aprovadas continuam bases: nomear um serviço não transforma automaticamente uma sala de estar em oficina ou pousada.

| Edifício / coleta | Ocupação proposta | Justificativa |
|---|---|---|
| E06 — Casa 1 e anexo | Mecânico genérico | Torre compacta e anexo de metal combinam com manutenção; distribui um figurante sem inventar parentesco. |
| E05 — Casa 2 | Sula, Pipo e duas crianças sem nome | Catálogo afirma quatro pessoas na casa de Sula e Pipo morando com ela. Duas entradas podem servir a uma só moradia. |
| E10 — Casas 3 e 4 | Casa 3: operário; Casa 4: Homem da Fila | Mantém duas moradias independentes. O piloto da Casa 3 serve a um morador sem quest; Homem da Fila continua encontrado no poço. |
| E12 — Casa do protagonista | Lip e Apollo (vínculo existente) | CasaLip tem ligação existente na City e contrato de história próprio. |
| E11 — Casas 5 e 6 | Casa 5: Tácio; Casa 6: idoso genérico | Duas moradias independentes; Tácio trabalha no mercado e visita a oficina à noite. Não confundir idoso genérico com Onofre. |
| E09 — Casa maior em L | Guilda: Bruna e operário da Casa 3 | Maior área para triagem, quadro de tarefas e oficina; cria percurso de trabalho com destino real. |
| E08 — Prédio com placa e duas entradas | Estalagem: Nilce; Zeca como hóspede de manhã | Placa, dois volumes e proposta de piso superior favorecem recepção e quartos. Fonte já liga Zeca à estalagem. |
| E16 — Domo / prefeitura | Uso comunitário; sem prefeito ou morador fixo | Usuário identifica o domo como prefeitura; enredo diz que Ferrovale não tem prefeito. Conservar prédio cívico sem criar governante. |
| E00 — Casa 7 / torre alaranjada | Vendedor genérico | Reserva residência urbana ao figurante sem duplicar a identidade de Tácio ou Sula. |
| E01 — Casa 8 / torre azul | Pessoa técnica genérica, modelo hacker | Metal e apartamento compacto combinam com bancada/eletrônica; profissão visual não cria quest automaticamente. |
| E02 — Casa 9 e garagem | Quirino: casa e oficina/garagem | Garagem e ferramentas favorecem oficina; catálogo coloca Quirino nos fundos do mercado. |
| E03 — Casa grande / possível mansão | Antiga casa de Ivo/Célia, vazia | Diálogos da Sessão 2 exigem casa vazia com foto, registros e bilhete. Isso dá função à casa-chave sem inventar novo personagem rico. |
| E04 — Geminado azul | Dara e vizinha em unidades separadas | Vizinhança física sustenta a terceira parte da cota; duas entradas justificam duas moradias. |
| E13 — Casa 10 / torre azul e ocre | Morador técnico genérico, modelo cientista | Preenche uma torre sem deslocar personagem que pertence à Cratera ou Prumo. |
| E14 — Duas unidades e porão | Benta e vigia genérico; porão comum | Permite ocupações distintas preservando as três entradas; Benta trabalha junto à estalagem, não neste porão. |

[Detalhamento por ambiente](../../output/moradores-ferrovale/2026-10-28/proposta-ocupacao.json) guarda IDs, vínculos e pendências. IDs fv_... ainda não existem no catálogo C#; o JSON não é consumido pelo jogo. O mapa-integracao.json do piloto não foi alterado.

### Fontes e implicações para os interiores

- **E03 / casa vazia de Ivo:** diálogos CityS2CasaIvoCadernos/Foto/Bilhete, em [CatalogoDeDialogos](../../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs), exigem foto, registros/cadernos e bilhete. Ivo continua na Estação 7. A fachada grande é uma proposta de localização, não prova de riqueza. Célia é foto/memória, nunca NPC vivo. O hall aprovado precisa de ambientação própria; a cama citada em uma narração não justifica colocar camas em todas as casas.
- **E08 / estalagem:** [CatalogoDeLocais](../../Assets/Scripts/Catalogos/CatalogoDeLocais.cs), IdEstalagem, descreve prédio de dois andares com seis portas no superior. [Quest q_nilce_quarto](../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs) exige quarto3 e bomba de reserva. Adaptar recepção/corredor e preservar a porta3; não gerar seis apartamentos completos só para representar seis portas. Zeca dorme ali de manhã, segundo o catálogo, em quarto ainda não escolhido e diferente do3.
- **E09 / Guilda:** Bruna e tarefas têm suporte em [CatalogoDeNpcs](../../Assets/Scripts/Catalogos/CatalogoDeNpcs.cs) e nas quests da Guilda. A arte de sala precisa de adaptação para quadro, triagem e forja. O superior não se torna residência canônica de Bruna; alojamento anexo é só possibilidade.
- **E04 / Dara e vizinha:** o enredo IV.6 descreve divisão da cota entre Dara, vizinha e uma parte guardada. q_dara_terceira fala de água, cisterna e Emílio. Não criar um terceiro apartamento por causa da expressão “terceira parte”. IdCasaDaDara descreve três áreas internas; isso não substitui o significado da quest. Duas unidades geminadas são a proposta física.
- **E05 / Sula e crianças:** o catálogo afirma quatro pessoas na casa de Sula, três delas crianças, e que Pipo mora com ela. Esse vínculo vem da biografia, não de falas já implantadas. Duas portas podem servir à mesma moradia; não criar duas famílias automaticamente.
- **E02 / Quirino:** IdOficinaDoQuirino fica nos fundos do mercado. Posicionar o setor de mercado de modo coerente ainda é pendência: não foi identificado um viaduto pronto na imagem atual. Casa e garagem continuam com acessos externos independentes, sem inventar porta interna.
- **E16 / domo:** usuário identifica como prefeitura; enredo III.5 afirma que Ferrovale não tem prefeito. Usar prédio comunitário sem inventar governante. A praça da assembleia, IdPracaDaAssembleia, é externa; não transferir a Sessão7 para o domo por conveniência. Interior metálico/radial já concebido preservado.
- **E14 / Benta:** a esquerda seria sua residência; IdPostoDaBenta é um toldo ao lado da estalagem E08. O porão não vira consultório subterrâneo. Vigia genérico da direita não substitui Zeca.

### Quem permanece fora dessas moradias

Rasha e Teodoro: Cratera/acampamento. Nadir e carregadores: visitantes conforme a história. Ivo: Estação7, mantendo casa vazia em Ferrovale. Onofre: dorme sob o viaduto, conforme catálogo; não lhe atribuir apartamento por obrigação. Célia/Emílio: memória e ausência, sem ator residente vivo. Artemis não recebe corpo/casa porque há espaço disponível. A filha de Tácio não é deslocada de Prumo para preencher apartamento.

Genéricos usados: mecanico_01, operario_01, idoso_01, idosa_01 (vizinha de Dara), vendedor_01, hacker_01, cientista_01, guarda_01. Reserva: morador_01, moradora_01, medico_01, crianca_01. Não é necessário usar todos os arquivos.

## Preparação das rotinas

O catálogo declara Madrugada/Manhã/Tarde/Noite e locais; isso é dado de design, não evidência de deslocamento ou relógio funcional. A atividade escrita não contém coordenadas/pathfinding.

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

Primeira implementação recomendada: **um genérico na Casa3**, com pontos próprios e pausas. Pontos explícitos por NPC dão mais controle que caminhada aleatória irrestrita. Pequenas variações de tempo podem vir depois. Relógio, presença única por ID e travessia de cenas exigem contrato próprio; não clonar o mesmo morador permanentemente dentro e fora.

Antes das rotinas externas, remarcar os acessos afetados pela imagem e definir pontos de trabalho/espaços comuns ainda ausentes. Um horário não deve tornar a progressão de quest inacessível. Não implantar relógio/navmesh/controlador global como efeito desta análise.

## Divergências preservadas

| Tema | Fontes divergentes | Tratamento nesta proposta |
|---|---|---|
| Quarto3 de Nilce | Background do NPC: filho Aurélio; q_nilce_quarto: abrigo de pessoas em fuga | Reservar quarto3 sem atribuir ocupante; conservar as falas da quest até revisão narrativa |
| Filha de Tácio | Background: Lúcia viva em Prumo; quest diz que ele sabe que ela não está lá; adendo B.2 menciona a cidade onde ela mora | Não criar a filha nem reescrever o desfecho; manter banca/cobre e casa de Tácio |
| Idades | Dara22/24, Ivo67/68, Nadir46/41, Rasha35/36, Teodoro48/52 entre catálogo/enredo | Não regenerar rosto por uma idade ainda divergente |
| Primeiro avistamento de Nadir | Catálogo menciona Zeca; enredo/diálogos dão o aviso a Pipo | Zeca pode vigiar; não substituir o evento de Pipo |
| Arte ativa | Tácio/Ivo e Homem da Fila/genérico diferem do elenco específico disponível | Corrigir apenas no recorte de montagem, preservando preferências do usuário |

O adendo prevalece onde altera expressamente o cânone. Onde ele diverge de falas existentes, registrar incerteza; distribuir moradias não autoriza reescrever quests, história ou datas. Foram consultados recortes narrativos pertinentes, não o roteiro integral. Nenhuma nova montagem visual foi iniciada; a instrução fornecida exige leitura integral antes dessa produção.

## Criação de sprites e retomada

[Mapa visual com a proposta de moradores](../../output/moradores-ferrovale/2026-10-28/ocupacao-proposta.html): imagem atual, 15 fachadas identificadas e miniaturas dos personagens existentes. Áreas desenhadas servem para reconhecer edifícios; não são nova coleta de portas ou prova de alinhamento geométrico.

Verificação documental: 30 conjuntos com dimensões de referência/walk conferidas, 15 IDs de edifício únicos, 41 referências locais/âncoras do catálogo válidas e nove fontes com hashes preservados após a análise. Guia e plano com links locais existentes. Revisão independente de coerência: sem bloqueadores; relação espacial do mercado/oficina de Quirino permanece pendência explícita. Nada foi testado em comportamento ou aprovado visualmente como consequência dessa revisão documental.

Método, receitas e situação real da automação no [guia LPC](../guias/criacao-sprites-personagens-lpc.md). Identidade de NPC, receita visual e moradia são registros distintos. Benta/Zeca são lacunas; mais genéricos não são necessários para começar. O compositor histórico precisa de adaptação de caminhos, dependências e metadados, sem executar sua limpeza em lote.

Decisões a avaliar: E03 como casa vazia de Ivo, E09 como Guilda, E08 como estalagem e a distribuição doméstica. Depois: remarcação, morador piloto, orientação/colisão/diálogo e expansão das rotinas. Nada disso é declarado implantado por este documento.

Estado: documentos/inventários salvos; nenhum NPC, sprite, diálogo, cena, prefab, porta ou save alterado. Referência inicial do Player preservada mesmo que ele se mova depois. A visualização direta da imagem8K excedeu o limite de transporte; foi criada uma prévia JPEG de leitura em output, sem mudar o original. Última consulta do Unity: City em Play, sem mutação de modo. Saves não foram acessados; nenhum backup novo de save ou limpeza pendente. Backups do piloto anterior preservados.

