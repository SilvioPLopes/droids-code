# Montagem dos interiores e NPCs de Ferrovale

## Objetivo e autorização

Montar o conjunto da cidade para teste manual do usuário, seguindo a distribuição de [moradores](2026-10-28-moradores-e-rotinas-ferrovale.md) e o [mapa de ambientes](../../output/interiores-ferrovale/2026-10-28/mapa-integracao.json). Pedido de 29/10: ligar todas as portas, escolher interiores conforme as fachadas, colocar sprites dos moradores, conectar falas/quests e criar sprites ausentes; organizar descartes reversíveis. Usuário reserva locomoção e pede verificações curtas. Usar planejamento/execução em sequência neste projeto Unity aberto, preservando alterações anteriores e trabalho paralelo.

## Escopo permitido e dependências

- Novas cenas em `Assets/Scenes/Interiores/`, variantes em `Assets/Prefabs/Interiores/` e imagens copiadas em `Assets/sprites/Mapas/Interiores/`; reutilizar InteriorBase/PortaTransicao, Player, câmera e Canvas existentes. Casa03 pode ser atualizada preservando seus GUIDs e a edição visual atual. CasaLip preserva imagem/puzzle/montagem; integração pontual do Apollo às quests, se necessária, sem alterar o puzzle.
- City: destinos/chegadas das entradas de edifícios; remarcação sobre a textura atual com registro antes/depois; acesso candidato somente se a fachada tiver entrada reconhecida. NPCs existentes: preservar os encontros externos; ligar a mesma identidade ao fluxo de quests, sem duplicar input. Novos pontos de serviço/personagens que faltam podem ser adicionados como raízes identificadas. Saídas externas da cidade e seu bloqueio de história não entram na remarcação de casas.
- Código novo em `Assets/Scripts/Interiores/`: interação de morador que coordena os gatilhos existentes, falas e oferta/entrega pelo GerenciadorDeQuests; objetos de quest/interior por composição, sem alterar o contrato do save. Nada de refatoração de UI, troca de namespaces ou novo sistema de quests.
- Novas sprites/configurações de Benta, Zeca e crianças/variantes necessárias em `Assets/sprites/Entities/Characters/`; composição conforme a técnica LPC autorizada pelo usuário, registrando origem e aproximações. Catálogos de história/quests e scripts compartilhados ficam preservados, salvo necessidade registrada antes de ultrapassar esse limite.
- Acrescentar cenas ao EditorBuildSettings; atualizar mapa de integração/inventário/retomadas. Evidências e ferramentas desta execução em `output/montagem-ferrovale/2026-10-29/`; backups exatos em `output/backups/2026-10-29-montagem-ferrovale/`.
- Quarentena em `output/quarentena/2026-10-29/`, somente para versões descartadas sem consumo atual comprovado, com origem/hash e referências atualizadas. Não apagar nada, não mover saves/backups, não limpar dependências/plugins, não mover assets referenciados ou arquivos de outra tarefa por aparência de lixo.

## Estado inicial e reversão

Projeto MCP confirmado `C:/Users/silve/DroidsCode/Assets`, fora de Play. Primeira consulta City limpa; consulta seguinte Game limpa: outro trabalho pode trocar a cena aberta. Confirmar modo/cenas antes de cada mutação e não salvar cenas alheias incidentalmente. Manter snapshots de City, CasaLip, build, prefab/arte Casa03 e JSONs antes da montagem. Registrar diffs por blocos/arquivos; reversão aplica apenas o alvo desta tarefa se não houver edição posterior. Novos assets têm metas próprios; GUIDs existentes preservados.

Referência anterior da Casa3: Player (2.56479287,-8.663213,0), indicada pelo usuário. Isso é posição do Player, não centro automático do trigger. Conferir o sprite/collider do Player e a abertura da fachada, sem deduzir o offset do Editor pelo zoom. Campo coletado será usado como base, com diferenças justificadas em tabela de remarcação.

## Aceite e validação mínima

- Cada acesso de edifício tem destino válido, chegada livre e saída recíproca; escadas/corredores mantêm identidade da casa. Imagem, NPC e casa correspondem aos IDs do plano. Ausência de referências de cena cruzadas.
- NPC fala pelo painel existente com E; um único consumidor de input por ator; gatilhos narrativos respeitam flags e quests usam os IDs/requisitos/recompensas existentes. Oferta não completa quest por atalho; entrega exige objetivos completos e ocorre somente por escolha do jogador.
- Personagens ausentes recebem arquivo real/configuração. Não chamar cópia simples de geração original; variantes derivadas têm fonte declarada. Personagens da Cratera continuam fora das residências da City.
- Verificação: compilação, consulta de referências/IDs/montagem persistente, no máximo um teste curto da lógica nova sem gravar saves. Não iniciar Play para testar caminhada, não fazer percurso automático de 32 cenas, não acessar progresso do usuário. Usuário avalia locomoção, escala, alinhamento e acabamento visual.
- Canvas/Scaler compartilhados preservados; arte original aprovada preservada. Quantidade de cenas/atores/portas e limitações entregues com evidência, sem declarar aprovação visual presumida.

## Etapas

### Ajuste do fluxo durante a execução

Usuário questionou a geração do corredor novo. Foi esclarecido que não era uma imagem aprovada e não tinha sido aplicada. Decisão de execução comunicada: montar usando as imagens aprovadas; manter `estalagem-corredor-seis-portas.png` e prompt somente como candidata em output, fora de Assets e de todas as cenas. A matriz superior aprovada da estalagem será mantida; as seis portas/quartos adicionais ficam como limitação explícita, sem gerar ou instalar novos interiores por suposição. Preservar o quarto3 como ponto narrativo do interior aprovado, sem anunciar seis quartos funcionais montados.

- [x] 1. Snapshot, configuração de montagem e posições por imagem/fachada.
- [x] 2. Sprites ausentes e componente de interação/quest.
- [x] 3. Interiores, conexões internas, moradores e pontos de história/quest.
- [x] 4. Portas da City, serviços/encontros preservados e build.
- [x] 5. Checagem curta de referências/compilação, quarentena reversível e entrega para teste manual.

## Resultado salvo para teste manual

- **32 ambientes:** 30 cenas novas, Casa03 atualizada e CasaLip existente preservada. Há 31 cenas na pasta Interiores e 36 cenas habilitadas no build ao todo. As cenas novas reutilizam variantes de InteriorBase, Player/câmera/iluminação e o Canvas existente. CasaLip recebeu apenas o controlador de interação do Apollo e ajuste do modo dos gatilhos manuais; introdução automática, puzzle, arte, porta e montagem foram preservados.
- **23 acessos de edifícios na City:** as 21 portas coletadas foram ligadas aos ambientes, a entrada original da CasaLip permanece funcional e o segundo acesso visível da Casa2 foi acrescentado. Dois acessos da Casa2 chegam ao mesmo interior de Sula. Há 25 TransicaoDeCena na City incluindo as duas saídas externas. A saída superior que já estava pendente continua fora deste recorte; não foi criado desvio do bloqueio narrativo da saída existente.
- **43 transições em prefabs de interiores**, além da visita ao anexo oferecida pelo mecânico. Dez ligações previstas de escada/corredor/apartamento foram montadas. O anexo retorna à casa; não foi inventada uma segunda porta na fachada ambígua da Casa1. Pontos de chegada efetivos estão nas coordenadas de TransicaoDeCena e no mapa de integração, sem referências entre Transforms de cenas diferentes.
- **22 prefabs de NPC em Assets/Prefabs/NPCs/Ferrovale**, com quatro poses recortadas dos arquivos LPC, orientação e interação por E. Foram colocados 23 pontos de NPC nos interiores, preservando os encontros da rua; City recebeu Onofre, Benta, Zeca e presença condicional de Nadir. Um mesmo personagem pode ter representação na casa e no local de trabalho, em cenas carregadas separadamente. Não foi implantado relógio, percurso diário ou seleção de presença por horário.
- **Quatro variantes visuais criadas:** Benta e Zeca, mais duas crianças sem nome da casa de Sula. São derivações LPC reais: modelo médico + avental, guarda + capuz e variantes de roupa infantil; não foram declaradas desenhos inéditos do zero. Sprite64×64 e walk576×256, receitas, origem e créditos guardados nas pastas NPCs/Benta, Zeca, Crianca_Sula_A e Crianca_Sula_B. Presilhas/caixa de Benta e binóculo/cano de Zeca continuam aproximações não desenhadas. Recortes de produção ficam em RuntimeFerrovale; originais anteriores preservados.
- **Conversas e quests:** MoradorInterativo coordena um único input, prioriza gatilhos narrativos, mostra opções de oferta/andamento/entrega e chama GerenciadorDeQuests para validar, consumir itens e pagar recompensas. Requisitos existentes não foram liberados artificialmente. Apollo oferece o fluxo após a casa resolvida. Lojistas usam LojaUIManager existente; descanso pago da estalagem não foi inventado. Gatilhos de NPC originais na rua continuam com suas flags, sob o controlador único.
- **Pontos adicionais:** cadernos/foto/bilhete de Ivo usam diálogos do catálogo; são pontos de interação, não novas ilustrações desses objetos. Terminais da bomba da estalagem, rolo de cobre do Tácio e triagem da Guilda usam TerminalUIManager/VerificadorDePuzzle existentes e pré-requisitos. Apenas o verificador do terminal usado fica ativo. Andares sem ambiente definido e quarto3 possuem interações de reserva, sem fingir seis quartos novos funcionais.

## Verificação e limites da evidência

**Retorno do teste do usuário, 29/10:** retratos ausentes, escolhas inadequadas, colisores/portais incorretos, soft lock e circulação vertical incompleta. A checagem estrutural abaixo não equivale a circulação aprovada. Diagnóstico e prioridades passam à [revisão dos interiores](2026-10-29-revisao-interiores-dialogos-colisoes.md); checkpoint da montagem feito antes da análise, sem aplicar nova correção por suposição.

[Verificação estrutural](../../output/montagem-ferrovale/2026-10-29/verificacao-estrutural.json): 31 cenas-alvo, 22 prefabs de NPC, 43 transições de interiores e 36 destinos de build conferidos, sem referências inválidas apontadas e sem falha de compilação. [Ligações efetivamente salvas](../../output/montagem-ferrovale/2026-10-29/ligacoes-salvas.json) e [recibo das portas](../../output/montagem-ferrovale/2026-10-29/city-edits.json) registram os destinos/chegadas. Nova consulta confirmou PlayerMovement habilitado e CameraFollow da City já vinculado ao Player, sem editar a câmera.

Revisão independente, somente leitura, identificou proteção de frame ausente no aviso de pré-requisito do terminal e nome de Tácio com codificação incorreta. Aplicados: callback de fechamento no terminal; nome canônico no prefab, conferido contra CatalogoDeNpcs. O nome foi tratado como relevante por também afetar a busca do retrato. Conferência dirigida encontrou a flag vazia na fala original de Tácio: City e prefab agora gravam ato1_falou_tacio, requisito já existente de q_cobre_tacio, sem mudar o catálogo. Moradores próximos disputam E por proximidade; sete pontos repetíveis usam o mesmo controlador para não reabrir no frame de fechamento. Esses ajustes foram compilados/inspecionados, não testados por teclado nesta rodada.

**Não houve Play, caminhada automática, Lua executado ou gravação de saves por testes do agente.** Usuário reservou input/locomoção: alinhamento das portas, escala dos NPCs, colisões dos móveis, aproximação para E, opções de conversa/quest e retorno de cenas aguardam seu teste. Colisões e posições foram montadas como estrutura de teste; não declarar acabamento visual ou circulação perfeitos. Pré-requisitos de história/itens permanecem necessários; este lote não conclui a campanha nem os encontros da Cratera.

O corredor gerado durante a execução permanece somente em output, fora dos assets/cenas. A montagem usa as imagens anteriores. Estalagem mantém a matriz superior aprovada; seis portas/quartos reais dependem de decisão visual futura. Bruna/Quirino e demais serviços usam as bases aprovadas; decoração específica de Guilda/oficina/objetos narrativos pode ser refinada após o teste, sem presumir que um nome de objeto altera a ilustração.

Pontos externos novos de Onofre/Benta/Zeca são posições de montagem para teste; mercado, viaduto e toldo ainda não ganharam ambientação própria na arte da City. Lojistas compartilham a loja/estoque já existentes, sem estoques individuais por profissão. Essas limitações não foram ocultadas criando imagens ou serviços novos fora do fluxo aprovado.

## Preservação, organização e retomada

O SaveScene da City normalizou centenas de registros existentes. Um merge por IDs preservou byte a byte os 415 blocos antigos fora dos alvos permitidos; só portas/controle dos NPCs e novas raízes entraram no resultado. CasaLip teve três blocos antigos alterados e um controlador acrescentado. Casa03 manteve os blocos anteriores, acrescentando moradores; seu retorno foi alinhado à nova porta, sem mudar imagem/colisões. Comparação de hashes confirmou Canvas.prefab, InteriorBase.prefab, Game, Battle e mapa8K iguais ao baseline desta execução.

Seis arquivos v1 de superiores, substituídos pelas v2 selecionadas e sem consumo ativo na montagem, foram movidos para [quarentena reversível](../../output/quarentena/2026-10-29/interiores-v1-substituidos/manifesto.json), com origem/destino/SHA256. Nenhuma exclusão realizada; arquivos ativos, candidatos ainda em avaliação, ferramentas e backups não foram classificados como lixo. Manifesto de produção guarda os novos caminhos de arquivo. Backups exatos em output/backups/2026-10-29-montagem-ferrovale; os backups anteriores permanecem preservados. Bibliotecas LPC locais ficam somente em output/ferramentas/lpc-portatil, sem alteração de pacotes Unity ou Python global.

Estado entregue: City limpa, fora de Play, Player habilitado e câmera vinculada. Próximo passo: usuário iniciar Play e testar as entradas e E nos moradores. Registrar os locais de colisão/desalinhamento encontrados; corrigir por alvo, sem remarcar tudo por tentativa. Somente depois de validar esta montagem, preparar rotinas de movimento/horário e eventual corredor alternativo. Nenhum save foi acessado ou precisa ser restaurado; arquivos de teste do piloto anterior não foram reutilizados.
