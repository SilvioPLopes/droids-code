# Plano — nomenclatura, casas e identidade dos NPCs de Ferrovale

**Estado e data:** planejamento inicial, 29/10/2026; nenhuma alteração de código, cena ou asset autorizada por este documento.
**Objetivo:** localizar cada edifício, ambiente, acesso, limitador e pessoa por uma matriz coerente; impedir que uma mesma pessoa seja apresentada em locais incompatíveis.
**Execução:** um agente por padrão, em lotes pequenos; usar execução de planos ao implementar. Este plano contém a proposta inicial e os pontos que precisam de definição antes de fechar contratos de código.
**Tecnologia existente:** Unity/C#, cenas e prefabs serializados, catálogos C# e inventários JSON; sem novos pacotes.

## Escopo desta rodada e dependências

Permitidos agora: este documento, o [plano documental](2026-10-29-refatoracao-documentacao.md) e apontadores no índice. Leitura estática dos arquivos necessários. Preservar cenas, prefabs, GUIDs, `.meta`, sprites, diálogos, quests, flags, saves e trabalho existente.

Fontes: [montagem salva](../historico/2026-10-29-montagem-cidade-interiores-npcs.md), [revisão de regressões](2026-10-29-revisao-interiores-dialogos-colisoes.md), [distribuição de moradores](2026-10-28-moradores-e-rotinas-ferrovale.md), [contrato City](../contextos/cenas/city.md), [mapa de integração](../../output/interiores-ferrovale/2026-10-28/mapa-integracao.json), [configuração do lote](../../output/montagem-ferrovale/2026-10-29/config-montagem.json). A matriz abaixo é uma visão de navegação inicial, não um novo catálogo executável.

Escopo futuro candidato: City, CasaLip apenas nos vínculos de identidade necessários, cenas/prefabs de Interiores, prefabs de NPCs/Ferrovale e referências afetadas no build, scripts, inventários e documentação. Antes de cada lote, registrar lista exata de arquivos, objetos, propriedades, valores anteriores e reversão; esta lista de candidatos não autoriza renomeação global.

Fora do escopo: resolver colisões, redesenhar mapas/UI, criar andares, alterar puzzle da CasaLip, reescrever narrativa, namespaces, pacotes ou infraestrutura. Os bloqueios de circulação continuam na revisão existente; nomes melhores não os corrigem.

## Diagnóstico sustentado e incertezas

| Observação | Implicação | Evidência / limite |
|---|---|---|
| City possui `GameObject`, `GameObject (1)`…`GameObject (16)` | Não é possível reconhecer a função pelo nome | Nomes conferidos no YAML; ainda falta associar componentes, posição e função a cada objeto. Não afirmar que todos são limitadores |
| `Porta_Casa03_Provisoria`, `Porta_Mansao_Provisoria`, `Porta_CasaMaior` | Mistura nome provisório, aparência e numeração | Overrides salvos na City; portas têm destino, mas o nome não revela morador/função |
| E03_Terreo tem `Obstaculo_1`, `_2`, `_3` e `LimitesDoPiso` | Obstáculo e limite precisam de vocabulário por função | Prefab conferido; o objeto desenhado sob cada collider exige comparação visual |
| `MoradorInterativo.idNpc` é separado de `nome`; Nilce=`estalajadeira`, Quirino=`ferreiro`, Sula=`vendedora_pecas` | Nome visível não pode substituir ID técnico sem mapear consumidores | Campos atuais conferidos; IDs também ligam quests e história |
| Mecânico consta em térreo/anexo; operário em casa/Guilda; Quirino em casa/garagem | Há múltiplos pontos de representação da mesma pessoa | Configuração do lote, não teste de todas as instâncias atuais |
| Montagem preserva encontros externos e coloca moradores nos interiores, sem seleção por horário | Carregar uma cena por vez não garante coerência de localização da pessoa | Estado documentado na montagem; não há comprovação de duplicação simultânea em memória nesta rodada |
| Transição carrega por nome; save grava `cena` e carrega esse nome | Renomear arquivo de cena pode quebrar destinos e saves antigos | `TransicaoDeCena`, `MoradorInterativo.destinoDeVisita` e `SalvamentoJson`; migração precisa ser definida antes do primeiro rename |

Contagens da montagem anterior: 32 ambientes incluindo CasaLip, 31 cenas em Interiores, 22 prefabs de NPC e 23 pontos de NPC internos. Não equivalem ao número de pessoas únicas ou à população canônica. A pasta Interiores e os caminhos das cenas foram consultados; não foi repetida a auditoria estrutural nem observado o Editor ao vivo.

## Matriz inicial: edifício → ambientes → pessoas

E00…E16 são IDs existentes de edifícios, com lacunas na sequência; preservar esses IDs para cruzar evidências. Sufixos abaixo abreviam `Interior_<ID>_<sufixo>`; CasaLip mantém seu nome atual. Ocupações vêm da distribuição/montagem, com conferência da configuração do lote; não comprovam presença visual atual ou rotina funcional.

| ID | Nome legível proposto | Ambientes atuais | Pessoas vinculadas / ressalva |
|---|---|---|---|
| E00 | Torre do vendedor | Hall, Patamar, Apartamento01 | `fv_vendedor_01` |
| E01 | Torre da pessoa técnica | Hall, Patamar, Apartamento01 | `fv_hacker_01`; sem nome pessoal definido |
| E02 | Casa e oficina de Quirino | Principal, Garagem | `ferreiro`; escolher onde fica em cada estado |
| E03 | Antiga casa de Ivo | Terreo, Superior | Casa vazia; Ivo está ligado à Estação 7; Célia é memória |
| E04 | Geminado de Dara e vizinha | UnidadeA, UnidadeB | `dara`, `fv_vizinha_dara_01`; unidades distintas |
| E05 | Casa de Sula | Principal | `vendedora_pecas`, `crianca_rua` (Pipo), `fv_crianca_sula_02`, `fv_crianca_sula_03`; duas entradas, mesma moradia |
| E06 | Casa do mecânico e anexo | Terreo, Superior, Anexo | `fv_mecanico_01`; térreo/anexo precisam de exclusão de presença |
| E08 | Estalagem de Nilce | Principal, Superior | `estalajadeira`, `zeca_cerca`; quarto de Zeca indefinido, não atribuir quarto3 |
| E09 | Guilda | Terreo, Superior | `mestra_guilda` (Bruna), `fv_operario_01`; moradia de Bruna não definida |
| E10 | Geminado do operário e Homem da fila | Casa03, Casa04 | `fv_operario_01`, `homem_fila`; não fundir as casas |
| E11 | Geminado de Tácio e idoso | Casa05, Casa06 | `tacio`, `fv_idoso_01`; o idoso não é Onofre |
| E12 | Casa de Lip | CasaLip | Lip e Apollo; preservar contrato do puzzle |
| E13 | Torre do morador técnico | Hall, Patamar, Apartamento01 | `fv_cientista_01`; sem nome pessoal definido |
| E14 | Moradias de Benta e vigia | UnidadeA, UnidadeB, Porao | `benta`, `fv_guarda_01`; posto de Benta é junto à estalagem; vigia não é Zeca |
| E16 | Edifício cívico / prefeitura | Civico | Sem prefeito/NPC residente inventado |

Fora da tabela doméstica: Onofre (`velho_viaduto`) junto ao viaduto; Nadir como presença de evento; personagens de outras regiões não recebem casa para preencher espaço. A matriz final deve contar separadamente identidades, representações em cenas, receitas visuais e arquivos derivados.

## Padrão proposto para avaliação

Separar **ID estável**, **nome legível**, **local físico** e **representação visual**. Um NPC mantém identidade quando muda de casa, trabalho, roupa ou nome exibido. Não trocar os IDs atuais de quests por apelidos novos nesta refatoração.

| Elemento | Formato proposto | Exemplo de destino, ainda não aplicado |
|---|---|---|
| Cena de interior | `Interior_<edificio>_<funcao>_<ambiente>` | `Interior_E05_CasaSula_Principal` |
| Raiz de edifício na City | `Edificio_<id>_<funcao>` | `Edificio_E05_CasaSula` |
| Portal | `Portal_<origem>_<destino>_<acesso>` | `Portal_City_E05_Principal_A` e `_B` |
| Chegada | `Chegada_<ambiente>_De_<origem>_<acesso>` | `Chegada_E05_Principal_De_City_A` |
| Limite externo | `Limite_<local>_<trecho>` | `Limite_City_Norte_01`, somente após identificar o trecho |
| Obstáculo interno | `Obstaculo_<ambiente>_<elemento>_<indice>` | `Obstaculo_E03_Terreo_Sofa_01`, somente com correspondência visual |
| Representação de NPC | `NPC_<idAtual>_<ponto>` | `NPC_ferreiro_E02_Garagem` |
| Visual | Identidade → receita/origem → poses/retrato | Quirino → origem LPC → recortes runtime; caminhos existentes antes de propor migração |

Usar identificadores técnicos sem espaços/acentos; manter português correto nos nomes apresentados ao jogador. Preservar City e CasaLip na primeira etapa para reduzir alcance. Evitar `Provisoria`, `CasaMaior` ou numeração sem vínculo de edifício nos nomes finais. Nomear pela função comprovada; não atribuir morador a um corredor compartilhado.

Agrupamento sugerido da hierarquia City: Edificios, Portais, Limites, NPCs e Servicos, usando raízes existentes quando possível. Reparentear pode alterar Transform e comportamento: primeiro renomear; agrupar só em lote próprio com preservação do Transform mundial e revisão dos consumidores de hierarquia.

## Contrato proposto de presença única

Regra exigida pelo usuário: **uma identidade tem um único local autorizado em cada estado do mundo**. Ao sair de casa para trabalhar, a casa não mostra uma cópia. Verificar apenas “uma cena carregada” é insuficiente: visitar casa e trabalho em sequência também precisa contar a mesma história.

Proposta para a primeira entrega: postos fixos escolhidos explicitamente por identidade, com os demais pontos desativados pela mesma decisão de presença. Isso resolve a consistência inicial sem pressupor relógio. Rotinas futuras mudam o local autorizado; caminho e animação de deslocamento são etapa separada. A escolha de cada posto ainda precisa ser avaliada para não retirar diálogos/serviços necessários.

Representações alternativas podem continuar nos assets; apenas a representação permitida fica visível/interativa, incluindo sprite, collider de interação e gatilhos do ator. Desativação não pode apagar quests, trocar identidade ou deixar collider invisível bloqueando passagem. As duas crianças de Sula são duas identidades e duas variantes; compartilhar nome visível não as funde.

Durante diálogo, loja ou entrega, uma mudança futura de estado não remove o interlocutor. Ausências narrativas e eventos, como Nadir, respeitam seus gatilhos existentes. Não criar sistema global de horário ou novo formato de save por consequência de renomear objetos.

## Etapas e validação por entrega

- [ ] **N1 — fechar matriz de identidade e localização, em leitura.** Completar nesta fonte as relações NPC→ID atual→nome→residência→trabalho→pontos de cena→prefab→origem visual→poses/retrato→diálogos/quests→regra de presença. Usar configuração como pista e confirmar no estado persistente, incluindo overrides e GUIDs; diferenciar duplicata de ator, mesma arte em pessoas distintas e derivados legítimos. Inventariar os objetos anônimos da City por caminho/fileID, componentes, posição e função; comparar limitadores com mapa. Aceite: todos os alvos têm identidade e evidência, dúvidas marcadas sem nome inventado.
- [ ] **N2 — avaliar nomes e presença.** Apresentar matriz antes/depois e postos propostos, mantendo IDs atuais. Definir com o usuário nomes pessoais que faltam e locais iniciais de Quirino, mecânico, operário e demais pessoas com casa/trabalho. Aceite: nenhum nome altera cânone e nenhum serviço obrigatório perde local acessível. O mecanismo de presença será especificado após esse levantamento, antes de escrever código.
- [ ] **N3 — renomear objetos locais.** Registrar nomes anteriores e caminho/fileID; localizar consumidores por `Find`, caminhos de Transform, nomes e referências serializadas. Alterar apenas nomes aprovados de limitadores/obstáculos/portais/NPCs; preservar componentes, posição, tamanho, layers e referências. Fora de Play, salvar e conferir diff. Aceite: hierarquia reconhecível e comparação visual sem deslocamentos; nomes não são usados como prova de circulação corrigida.
- [ ] **N4 — renomear cenas/prefabs/assets em lote piloto.** Começar por E05, pois duas entradas chegam à mesma moradia. Antes da edição, mapear `nomeCenaDestino`, `destinoDeVisita`, build, loaders, save `cena`, ferramentas geradoras, JSONs e links. Definir compatibilidade de saves existentes: preferir tradução dos nomes antigos no carregamento, com escopo adicional delimitado e testado, ou adiar rename físico até resolver compatibilidade. Renomear via Unity preservando `.meta`/GUID, atualizar todas as referências do piloto e conferir que geradores não recriam os nomes antigos. Aceite: duas entradas/retornos válidos, save com nome antigo resolvido e nenhuma referência ausente; só então expandir por edifício. Não editar saves reais do usuário.
- [ ] **N5 — organizar o vínculo visual, sem apagar sprites.** Após N1, escolher uma origem visual por identidade e registrar derivados usados por Animator, SpriteRenderer, retrato e prefabs. Corrigir vínculo duplicado entre pessoas diferentes somente com evidência e escolha visual; mover pastas apenas se melhorar navegação e todos os consumidores estiverem mapeados. Aceite: miniatura, corpo e fala pertencem ao interlocutor; crianças distintas; receita/origem preservadas. Arquivos walk/idle/poses não são duplicação de pessoa por si só.
- [ ] **N6 — implementar presença por identidade.** Fechar arquivos e interface da solução mínima após N1/N2, identificando consumidores de `MoradorInterativo`, `NpcInterativo`, gatilhos e controlador de presença de Nadir antes de mudar componente compartilhado. Testes de comportamento: dois pontos do mesmo ID autorizam somente um local; casa↔trabalho preserva escolha; IDs diferentes permanecem independentes; ausente não fala nem bloqueia; conversa aberta preserva interlocutor; Nadir e pré-requisitos continuam condicionais. Aceite por input real: visitar os dois locais não mostra a mesma pessoa em ambos e quests/lojas permanecem acessíveis. Esta etapa não pode ser declarada concluída apenas por compilação.
- [ ] **N7 — retomada e expansão.** Atualizar contratos somente onde mudaram, registrar mapa de renomes e resultados nesta fonte, vincular evidências sem copiá-las. Expandir lotes aprovados; caminhada/relógio só após contrato próprio de rotinas e save. Conferir contagens de pessoas e representações separadamente.

## Aceite geral, riscos e reversão

O usuário identifica uma fachada e encontra interior, moradores, falas e limites correspondentes; os nomes distinguem unidades, acessos e obstáculos. Uma mesma pessoa não aparece em casa e no trabalho sem uma mudança de estado que justifique a viagem. Pessoas diferentes não usam acidentalmente a mesma aparência. As cenas continuam acessíveis e saves antigos permanecem carregáveis.

Riscos prioritários: renomear destinos por string, invalidar saves, perder overrides/GUIDs, confundir IDs de função com nomes, desativar interlocutor necessário e interpretar arquivo derivado como pessoa duplicada. Cada risco pertence às validações N1/N3/N4/N5/N6 acima; compilação não comprova comportamento nem aparência.

Antes de mutações futuras: snapshot dos arquivos exatos e tabela nome/caminho/valor anterior→novo em `output/backups/<data>-nomenclatura-ferrovale/`, com destino registrado aqui. Reverter somente o lote solicitado e sem sobrescrever edição posterior; conferir estado e diff. Teste que grave save exige backup e restauração verificada; preferir fixtures isoladas. Nenhum backup de assets é necessário para esta rodada documental, que não os altera.

Validação visual futura: registrar resolução renderizada, zoom da Game View, Canvas/Scaler efetivos e referência antes/depois. Preservar Canvas/Scaler; comparação deve verificar enquadramento, ausência de deslocamentos e identidade visual, sem tratar zoom do Editor como causa. Respeitar a reserva anterior de input/locomoção pelo usuário; testes por API ficam identificados como tal.

## Execução e retomada

- **Editado/salvo nesta rodada:** somente planos e apontadores; sem rename de código, objetos ou assets.
- **Último resultado:** análise estática em 29/10/2026, matriz inicial e exemplos reais de nomes/IDs conferidos nos arquivos indicados. Configuração de lote é evidência histórica, não substitui N1.
- **Testado em comportamento:** nada nesta rodada. **Aprovado visualmente:** nada nesta rodada.
- **Pendências:** completar N1; avaliar padrão e postos; fechar interfaces de presença e compatibilidade antes de implementar. Não atribuir a esta tarefa os bloqueios de circulação já diagnosticados.
- **Próximo passo concreto:** revisar esta matriz com o usuário e completar o inventário direcionado de limitadores e representações, sem alterar a montagem.
- **Unity:** não operado; cena, Play e alterações não salvas atuais desconhecidos. Nenhum save acessado, backup criado ou limpeza pendente por esta rodada. Backups anteriores permanecem nos destinos das tarefas de origem.
