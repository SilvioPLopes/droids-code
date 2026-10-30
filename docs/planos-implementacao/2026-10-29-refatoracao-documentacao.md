# Plano — refatoração documental de docs, exceto enredo

**Data:** 29/10/2026. **Estado:** triagem global preliminar concluída; riscos de consulta verificados e orientação de pesquisa aplicada ao índice. Consolidação global de conteúdo pendente.
**Objetivo:** reduzir sobreposição e carga de leitura, definir a fonte responsável por cada informação e retirar da navegação ativa somente o que tiver substituto comprovado.
**Autorização vigente:** validar utilidade/risco de consulta e preparar orientação para a IA pesquisar com menos desperdício. Permitidos nesta rodada: este plano e `docs/indice-trabalho.md`, já indicado por AGENTS.md. Sem migração/exclusão de fontes, edição de enredo, código, cenas, configurações ou plugins.

**Antes/reversão deste recorte:** índice ainda sem roteiro explícito de busca/semáforo; plano com classificação global preliminar. Cópias exatas dos dois documentos e hashes dos 65 arquivos fora de enredo em `output/backups/2026-10-29-refatoracao-documentacao/guia-consulta/`. Reverter apenas os trechos deste recorte, conferindo alterações posteriores. Consumidores: AGENTS.md encaminha ao índice; a IA usa o índice para escolher plano/contrato. Aceite: consulta com caminho claro, riscos sustentados por trechos concretos, nenhuma leitura integral obrigatória e nenhum documento marcado inútil sem prova.

## Escopo completo e limite da validação

Foram inventariados **67 arquivos Markdown** em docs: **65 no escopo** e **dois de enredo excluídos expressamente pelo usuário**. Há ainda dez imagens de evidência, a preservar com seus vínculos; não foram avaliadas visualmente nem são candidatas automáticas a descarte.

| Área | Markdown no escopo | Papel e atenção principal |
|---|---:|---|
| Raiz | 3 | Entrada, arquitetura e prioridades; evitar três fontes de andamento |
| contextos | 7 | Contratos misturados com propostas e uma auditoria datada |
| guias | 8 | Procedimentos úteis; montagem/validação/City precisam de reconciliação temporal |
| planos-implementacao, incluindo melhorias | 27 | Tarefas, referências e propostas; maior concentração de cronologia e sobreposição |
| problemas | 4 | Reprodução/causa; verificar absorção pelas tarefas e revisões |
| revisoes | 3 | Evidência datada, distinta de contrato ou tarefa vigente |
| historico | 13 | Proveniência já fora da leitura diária; idade não autoriza exclusão |
| **Total** | **65** | **Todos listados individualmente abaixo** |

**Método econômico:** inventário de caminhos/tamanho, títulos, seções e estados declarados; uso dos recortes já lidos nesta conversa e leitura dirigida de entradas/finais onde a classificação dependia de função/época. Não houve releitura integral dos 65 documentos, consulta de código para provar implementação, execução de testes ou inspeção do Editor. Assim, a tabela valida a cobertura do escopo e propõe o tratamento; **não comprova que todas as informações de cada arquivo sejam atuais, redundantes ou dispensáveis**.

Confiança alta na contagem, localização e papel declarado; indicação preliminar nas consolidações; exclusão de conteúdo ainda sem validação suficiente. Alegações antigas de implementação/teste devem manter data, método e pendências. A pasta enredo não será revisada, conciliada, movida nem reescrita por este plano.

## Correção do escopo anterior

Os lotes anteriores unificaram as pastas de planos, arquivaram três registros, reduziram o índice e reescreveram **apenas quatro documentos**. A redução anunciada de aproximadamente 75% era do texto desses quatro, não de docs inteira. O total permaneceu 67 arquivos. Aquela execução é parcial e não encerra esta refatoração global.

Preservar esse trabalho, sem reexecutá-lo por falta de cobertura anterior. Os registros estão nos manifestos de [organização](../../output/backups/2026-10-29-refatoracao-documentacao/manifesto.json) e [reescrita parcial](../../output/backups/2026-10-29-refatoracao-documentacao/lote-02/manifesto.json). A versão anterior deste plano está em [backup da revisão de escopo](../../output/backups/2026-10-29-refatoracao-documentacao/revisao-escopo-global/2026-10-29-refatoracao-documentacao.md).

## Critérios de classificação

- **Manter:** função própria identificada; não significa conteúdo inteiro validado como atual.
- **Revisar:** função útil, mas mistura épocas, propostas, execução ou duplicação. Reduzir depois de conferir as seções afetadas.
- **Consolidar:** sobreposição identificada; candidato a transferência de informação para fonte definida, sem retirada automática.
- **Reclassificar:** função não combina com a pasta; movimento proposto, preservando conteúdo e referências.
- **Preservar histórico:** manter evidência original fora da retomada diária. Histórico não comanda execução atual.

**Exclusão aprovada nesta triagem: nenhuma.** O candidato antigo City HD não tem redundância integral demonstrada. Um documento só pode sair de docs depois de registrar conteúdo único, destino, pendências transferidas e consumidores; preservar cópia anterior. Não arquivar plano com input/aceite aberto como se o produto estivesse concluído.

## Matriz global — 65 documentos

Os caminhos são relativos a docs. As observações são encaminhamentos preliminares, não autorizações para alterar os arquivos.

### Raiz

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [contexto-projeto.md](../contexto-projeto.md) | Manter | Arquitetura e mapa dos sistemas; não é diário de execução. |
| [indice-trabalho.md](../indice-trabalho.md) | Manter | Entrada única; atualizar depois de consolidar fontes. |
| [plano-desenvolvimento.md](../plano-desenvolvimento.md) | Revisar | Concentrar prioridades/estado por ID; substituir repetição de tarefas por links. |

### Contextos

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [contextos/analises/2026-10-18-auditoria-city-casalip-ddol.md](../contextos/analises/2026-10-18-auditoria-city-casalip-ddol.md) | Reclassificar | Auditoria datada, não contrato: candidata a revisoes/ ou historico/, com links corrigidos; conservar hipóteses como hipóteses. |
| [contextos/cenas/_modelo-cena.md](../contextos/cenas/_modelo-cena.md) | Manter | Modelo reutilizável; não é pendência nem legado. |
| [contextos/cenas/casalip.md](../contextos/cenas/casalip.md) | Manter | Contrato compacto; resultado/aceite pertencem ao plano e revisão. |
| [contextos/cenas/city.md](../contextos/cenas/city.md) | Revisar | Separar contrato das propostas City viva; preservar CV-01…CV-05 e corrigir estados temporais sem presumir montagem atual. |
| [contextos/sistemas/combate.md](../contextos/sistemas/combate.md) | Manter | Contrato em atualização recente por outra tarefa; não reduzir regras sem validar consumidores. |
| [contextos/sistemas/estado-e-save.md](../contextos/sistemas/estado-e-save.md) | Manter | Contrato de persistência; referência para evitar propostas que reabram entregas. |
| [contextos/sistemas/interface.md](../contextos/sistemas/interface.md) | Revisar | Manter contrato por painel e exceções de feedback; retirar cronologia/testes duplicados após localizar evidência. |

### Guias

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [guias/criacao-sprites-personagens-lpc.md](../guias/criacao-sprites-personagens-lpc.md) | Manter | Procedimento e proveniência; distinguir automação existente e trabalho proposto. |
| [guias/equipamentos-apollo.md](../guias/equipamentos-apollo.md) | Manter | Uso/comandos/IDs; fonte de consulta operacional. |
| [guias/icones-de-itens.md](../guias/icones-de-itens.md) | Manter | Troca e vínculo visual por ID; não confundir com aceite dos ícones. |
| [guias/implementacao-city.md](../guias/implementacao-city.md) | Consolidar | Matriz de diálogos/eventos de City: cruzar com contexto City, moradores e nomenclatura; preservar IDs, divergências e produtores de flags únicos. |
| [guias/interfaces-sobre-imagens.md](../guias/interfaces-sobre-imagens.md) | Manter | Método reutilizável; retirar cópias desse procedimento nos planos de cada menu. |
| [guias/montagem-ato-1.md](../guias/montagem-ato-1.md) | Revisar | Roteiro mistura montagem pretendida e parcial; separar procedimento de estado datado; não declarar Ato1 pronto. |
| [guias/operacao-mcp.md](../guias/operacao-mcp.md) | Manter | Procedimento específico; não operado nesta revisão. |
| [guias/validacao-jogo.md](../guias/validacao-jogo.md) | Revisar | Matriz VAL-01…VAL-11 continua útil; remeter resultados às revisões e reconciliar com eixo06 sem eliminar cenários. |

### Planos — tarefas e referências

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [planos-implementacao/_modelo-tarefa.md](_modelo-tarefa.md) | Manter | Modelo exigido pelo fluxo; não arquivar como tarefa antiga. |
| [planos-implementacao/2026-10-16-correcao-casalip.md](2026-10-16-correcao-casalip.md) | Revisar | Resumir retomada/pendências atuais; separar checklists históricos de trabalho restante; candidato a receber síntese dos relatos da casa. |
| [planos-implementacao/2026-10-17-auditoria-cenas.md](2026-10-17-auditoria-cenas.md) | Revisar | Mistura auditoria, coleta de coordenadas e frentes posteriores; preservar coordenadas e IDs, encaminhar evidências datadas e tarefas próprias. |
| [planos-implementacao/2026-10-22-diagnostico-avisos-e-carregamento.md](2026-10-22-diagnostico-avisos-e-carregamento.md) | Revisar | Várias retomadas em ordem temporal ambígua; conservar a última decisão por fluxo e suas validações pendentes. |
| [planos-implementacao/2026-10-22-orientacao-npcs-city.md](2026-10-22-orientacao-npcs-city.md) | Manter | Consolidado parcialmente nesta conversa; reavaliar encaixe após revisão global de City, sem repetir a reescrita. |
| [planos-implementacao/2026-10-22-terminal-ilustrado.md](2026-10-22-terminal-ilustrado.md) | Consolidar | Grupo de interfaces: método no guia, contrato na interface, pendências na matriz de aceite; preservar foco/legibilidade e evidência de reprovação. |
| [planos-implementacao/2026-10-23-bag-ilustrada.md](2026-10-23-bag-ilustrada.md) | Consolidar | Grupo de interfaces: preservar input/aceite e decisão de equipamentos pelo terminal; retirar proposta artística superada da leitura ativa. |
| [planos-implementacao/2026-10-23-droid-ilustrado.md](2026-10-23-droid-ilustrado.md) | Consolidar | Grupo de interfaces: separar mecânica/save entregue de arte/gabarito ainda propostos; não perder testes de equipamentos. |
| [planos-implementacao/2026-10-23-loja-ilustrada.md](2026-10-23-loja-ilustrada.md) | Consolidar | Grupo de interfaces: preservar feedback local, comportamento econômico e pendência de input/aceite. |
| [planos-implementacao/2026-10-23-menu-mundo-ilustrado.md](2026-10-23-menu-mundo-ilustrado.md) | Consolidar | Grupo de interfaces: conservar opções/origem/retorno e avaliação manual; encaminhar montagem já documentada. |
| [planos-implementacao/2026-10-23-sprites-unicas-itens.md](2026-10-23-sprites-unicas-itens.md) | Consolidar | Grupo de interfaces: mapa operacional no guia de ícones, evidência datada preservada e aceite visual na matriz. |
| [planos-implementacao/2026-10-23-status-ilustrado.md](2026-10-23-status-ilustrado.md) | Consolidar | Grupo de interfaces: preservar atributos/energia, listas longas, fixtures e aceite/input pendentes. |
| [planos-implementacao/2026-10-24-apresentacao-telas.md](2026-10-24-apresentacao-telas.md) | Manter | Entrega distinta com avaliação/publicação; não arquivar somente por data nem alterar trabalho externo. |
| [planos-implementacao/2026-10-28-concepcao-interiores-ferrovale.md](2026-10-28-concepcao-interiores-ferrovale.md) | Manter | Referência arquitetônica já abreviada; contém preferências e decisões que a matriz de pessoas não substitui. |
| [planos-implementacao/2026-10-28-moradores-e-rotinas-ferrovale.md](2026-10-28-moradores-e-rotinas-ferrovale.md) | Manter | Justificativas/rotinas/divergências já abreviadas; não confundir com presença ou caminhada implementadas. |
| [planos-implementacao/2026-10-28-portas-interiores-e-mapas.md](2026-10-28-portas-interiores-e-mapas.md) | Revisar | Maior concentração de frentes e cronologia; distinguir City/vale/Battle, interiores e candidatos de arte; preservar coordenadas e decisões de instalação. |
| [planos-implementacao/2026-10-29-battle-artes-menus.md](2026-10-29-battle-artes-menus.md) | Manter | Trabalho recente em andamento; há registro de divergência disco/Editor. Não consolidar sem checkpoint do responsável. |
| [planos-implementacao/2026-10-29-refatoracao-documentacao.md](2026-10-29-refatoracao-documentacao.md) | Manter | Fonte desta triagem global; não tratar o lote anterior como conclusão de docs inteira. |
| [planos-implementacao/2026-10-29-refatoracao-nomenclatura-ferrovale.md](2026-10-29-refatoracao-nomenclatura-ferrovale.md) | Manter | Planejamento independente de código/cenas; não executar como consequência da organização documental. |
| [planos-implementacao/2026-10-29-revisao-interiores-dialogos-colisoes.md](2026-10-29-revisao-interiores-dialogos-colisoes.md) | Revisar | Diagnóstico/regressões úteis; separar relato/evidência/commit de ações ainda abertas, preservando causas e restrições do usuário. |

### Planos — backlog de melhorias

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [planos-implementacao/melhorias/README.md](melhorias/README.md) | Manter | Índice já abreviado; precedência das entregas posteriores explicitada. |
| [planos-implementacao/melhorias/01-terminal-e-puzzles.md](melhorias/01-terminal-e-puzzles.md) | Revisar | Conferir propostas TER/PUZ/CAD contra contratos e entregas; checkboxes antigos não comprovam ausência no código. |
| [planos-implementacao/melhorias/02-estado-inventario-e-economia.md](melhorias/02-estado-inventario-e-economia.md) | Revisar | EST-01/INV-01 têm entrega posterior parcial; preservar EST-02/ECO/INV-02 e testes não demonstrados. |
| [planos-implementacao/melhorias/03-combate-e-progressao.md](melhorias/03-combate-e-progressao.md) | Revisar | B1/R1 e decisões de retorno Casa precisam de precedência explícita frente a energia/checkpoint posteriores; não apagar propostas ainda válidas. |
| [planos-implementacao/melhorias/04-missoes-mundo-e-narrativa.md](melhorias/04-missoes-mundo-e-narrativa.md) | Revisar | Conciliar MIS com montagem de moradores/quests, sem declarar todas as cadeias integradas; narrativa continua adiada. |
| [planos-implementacao/melhorias/05-mecanicas-e-demo.md](melhorias/05-mecanicas-e-demo.md) | Manter | Propostas de produto, não legado comprovado; manter decisão/proposta separadas e ligação ao eixo03. |
| [planos-implementacao/melhorias/06-validacao-e-entrega.md](melhorias/06-validacao-e-entrega.md) | Consolidar | Definir divisão com validacao-jogo: processo/encerramento aqui, casos de jogo no guia, resultados nas revisões. |

### Problemas

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [problemas/battle-fundo-nao-renderiza.md](../problemas/battle-fundo-nao-renderiza.md) | Revisar | Contorno antigo não comprova causa resolvida; confrontar relato com evidência do plano Battle antes de arquivar. |
| [problemas/casalip-apresentacao-dialogo.md](../problemas/casalip-apresentacao-dialogo.md) | Consolidar | Família CasaLip: síntese/pendências no plano, evidência datada na revisão; contém referências antigas ausentes e aceites parciais. |
| [problemas/casalip-retorno-porta-trancada.md](../problemas/casalip-retorno-porta-trancada.md) | Consolidar | Família CasaLip: preservar reprodução/causa e reinício/load pendente; API de revisita não encerra o caso. |
| [problemas/terminal-log-painel-inativo.md](../problemas/terminal-log-painel-inativo.md) | Consolidar | Família CasaLip/terminal: cruzar TER-002 com plano/validação; preservar reprodução se ainda única. |

### Revisões

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [revisoes/auditoria-cenas-2026-10-17.md](../revisoes/auditoria-cenas-2026-10-17.md) | Manter | Evidência datada; tarefas atuais apontam para ela, sem reproduzir o diagnóstico integral. |
| [revisoes/casalip-validacao-2026-10-17.md](../revisoes/casalip-validacao-2026-10-17.md) | Manter | Resultados C01…C10 e limites de teste; não excluir porque há plano com resumo. |
| [revisoes/casalip.md](../revisoes/casalip.md) | Reclassificar | Diagnóstico histórico de 16/10 com atualizações; candidato a histórico após resolver links, sem perder evidência ou chaves de reprodução. |

### Histórico

| Documento | Encaminhamento | Motivo / condição |
|---|---|---|
| [historico/README.md](../historico/README.md) | Revisar | Navegação histórica: separar arquivo, origem, sucessor e pendências atuais; sem copiar estado antigo como atual. |
| [historico/2026-09-14-bugs-casalip.md](../historico/2026-09-14-bugs-casalip.md) | Preservar histórico | Relato original; não usar como validação atual de erro semelhante. |
| [historico/2026-09-15-entrega-ato-1.md](../historico/2026-09-15-entrega-ato-1.md) | Preservar histórico | Fotografia de entrega; fora da leitura diária. |
| [historico/2026-10-14-analise-projeto-unity.md](../historico/2026-10-14-analise-projeto-unity.md) | Preservar histórico | Inventário estático datado; não é arquitetura atual. |
| [historico/2026-10-16-contexto-anterior/bug-battle-sprite-background.md](../historico/2026-10-16-contexto-anterior/bug-battle-sprite-background.md) | Preservar histórico | Investigação original do contorno; referência de reprodução. |
| [historico/2026-10-16-contexto-anterior/checklist-desenvolvimento.md](../historico/2026-10-16-contexto-anterior/checklist-desenvolvimento.md) | Preservar histórico | Checklist anterior; suas caixas abertas não reabrem trabalho automaticamente. |
| [historico/2026-10-16-contexto-anterior/documentacao-tecnica.md](../historico/2026-10-16-contexto-anterior/documentacao-tecnica.md) | Preservar histórico | Contrato/cronologia antigos; instruções históricas não substituem AGENTS/índice atuais. |
| [historico/2026-10-16-contexto-anterior/guia-montagem-ato-1.md](../historico/2026-10-16-contexto-anterior/guia-montagem-ato-1.md) | Preservar histórico | Versão original do guia, sem execução automática. |
| [historico/2026-10-16-contexto-anterior/readme.md](../historico/2026-10-16-contexto-anterior/readme.md) | Preservar histórico | Entrada antiga, mantida por proveniência. |
| [historico/2026-10-17-contexto-antes-hierarquia.md](../historico/2026-10-17-contexto-antes-hierarquia.md) | Preservar histórico | Contexto anterior à hierarquia; consulta sob demanda. |
| [historico/2026-10-22-cidade-hd.md](../historico/2026-10-22-cidade-hd.md) | Preservar histórico | Candidato anterior a descarte, sem prova de redundância integral; suspender exclusão automática. |
| [historico/2026-10-23-aproveitamento-artes-menus.md](../historico/2026-10-23-aproveitamento-artes-menus.md) | Preservar histórico | Avaliação original de imagens; referência, não tarefa de escolher próximo painel. |
| [historico/2026-10-29-montagem-cidade-interiores-npcs.md](../historico/2026-10-29-montagem-cidade-interiores-npcs.md) | Preservar histórico | Baseline da montagem; não comprova circulação/aceite e não encerra regressões. |

## Riscos de consulta confirmados

Validação dirigida em 29/10/2026, usando os trechos abaixo e as leituras já realizadas. Confirma riscos documentais; não audita a implementação atual do jogo. **Nenhum arquivo inteiro foi comprovado inútil.** A classificação de cada arquivo permanece na matriz acima; vermelho indica risco de uso como instrução vigente, não autorização de exclusão.

| Fonte / trecho identificador | Evidência e utilidade preservada | Sinal vermelho / encaminhamento |
|---|---|---|
| [Backlog de estado/inventário](melhorias/02-estado-inventario-e-economia.md), “Estado atualizado” de 23/10 | O próprio texto registra entregas EST-01 e parte de INV-01; checkboxes conservam o planejamento original. Outros itens continuam propostas úteis. | Não tratar todos os checkboxes abertos como trabalho pendente. Conciliar por ID com a entrega correspondente antes de executar. |
| [Backlog de combate](melhorias/03-combate-e-progressao.md), atualização de 23/10 | Registra energia Apollo implementada e identifica “sem sistema paralelo de energia” como premissa antiga; recarga R1 ainda proposta. | Não executar COM-04 a partir da premissa antiga. Preservar a decisão em aberto sobre complementar/substituir a energia. |
| [Guia City](../guias/implementacao-city.md), premissa inicial e matriz de diálogos | “A reposicionar” aparece em premissa explicitamente não auditada. A matriz de 35 diálogos contém conteúdo que precisa de destino antes de eventual retirada. | Não reposicionar NPCs por essa frase. Consultar matriz de identidade/plano da tarefa e evidência atual. Não descartar o guia inteiro. |
| [Contrato City](../contextos/cenas/city.md), “Escolha proposta, ainda não aprovada” | Há propostas de postos fixos e afirmações limitadas à época da análise, misturadas com contrato. | Não confundir proposta com decisão vigente nem “não implementado nesta análise” com ausência atual no jogo. Separar esses papéis na consolidação City. |
| [Plano de portas/mapas](2026-10-28-portas-interiores-e-mapas.md), retomadas e piloto | A cronologia contém etapas com condições distintas; a montagem posterior altera parte do enquadramento inicial. | Não escolher a primeira ocorrência de “retomada vigente” como resumo definitivo. Localizar a etapa e o escopo que respondem à tarefa. |
| [Plano Battle](2026-10-29-battle-artes-menus.md), estado conhecido do Editor | Registra divergência possível entre disco/Editor e restrição operacional daquela tarefa. É uma entrega em andamento, não documento descartável. | Não inferir Editor sincronizado a partir dos arquivos; respeitar a restrição e conferir o checkpoint antes de qualquer atuação. Esta revisão não operou o Unity. |
| Histórico, revisões e auditorias datadas | Conservam causas, condições de teste, decisões e proveniência. | Fora da leitura inicial; consulta dirigida quando a dúvida exigir evidência histórica. Teste antigo não invalida regressão relatada agora. |

**Útil no cotidiano:** entrada, plano da tarefa, contrato pertinente e procedimento necessário. **Útil sob demanda:** propostas, relatos, auditorias e histórico. **Candidato a retirar:** somente trecho duplicado ou instrução superada cujo conteúdo único, pendências e substituto estejam identificados. A lista de arquivos a excluir permanece vazia por falta dessa prova; a primeira limpeza aplicada é de navegação e carga de leitura.

## Estrutura de destino e fontes responsáveis

Manter a estrutura existente antes de criar novas pastas:

| Informação | Fonte responsável |
|---|---|
| Onde retomar | indice-trabalho.md |
| Prioridade e estado resumido por ID | plano-desenvolvimento.md |
| Arquitetura | contexto-projeto.md |
| Contrato vigente de cena/sistema | contextos/cenas e contextos/sistemas |
| Procedimento reutilizável e casos de teste | guias |
| Escopo, decisões abertas, aceite e próximo passo de uma entrega | plano da tarefa |
| Reprodução/causa de problema ainda distinto | problemas |
| Evidência datada e limite do método | revisoes |
| Versão substituída e proveniência | historico ou backup explícito, sem participação na leitura diária |

Não transformar referências em novos documentos concorrentes. Um resumo deve apontar para a fonte completa; não copiar todas as falas, regras, testes e resultados para cada plano.

## Consolidações propostas e ganho esperado

| Frente | Proposta para implementação futura | Ganho / risco a resolver |
|---|---|---|
| Interfaces de exploração | Conferir os sete planos Terminal, Bag, Droid, Loja, Mundo, Status e Ícones; extrair uma matriz curta de pendências/aceite, usando contratos/guias existentes para funcionamento | Candidata a substituir sete entradas por uma. Droid inclui mecânica/save e Terminal inclui foco: preservar esses conteúdos e testes antes de retirar originais |
| CasaLip | Cruzar plano da casa, três relatos de problema, revisão original e validação17/10; manter síntese das ações no plano e evidências datadas nas revisões | Possível retirar relatos absorvidos; não perder reinício/load, input/visual, rejeições, causas e reprodução |
| City | Conferir guia implementacao-city, contrato City, orientação, moradores e matriz de nomenclatura | Um destino para cada ID/vínculo/condição, sem cinco descrições concorrentes. Guia de 35 diálogos pode conter informação única; não descartá-lo sem mapa |
| Mapas e auditorias | Separar coordenação atual de portas/mapas, coordenadas estáveis e registros datados; reclassificar auditoria DDOL | Retomada curta; preservar coleta e divergência entre arquivos e Editor. Não atribuir estado atual por relatório antigo |
| Backlog e validação | Reconciliar os seis eixos com entregas posteriores; processo no eixo06, casos no guia, resultados nas revisões | Evitar reabrir tarefas entregues e manter propostas ainda úteis; preservar 37 IDs e diferenças de namespace entre IDs VAL dos documentos |
| Histórico | Conferir somente navegação, sucessores e consumo ativo; manter originais enquanto tiverem função probatória | Menos leitura de legado sem inventar ganho de quantidade. Redução de arquivos históricos exige decisão sustentada por conteúdo, não por data |

Esses ganhos são **hipóteses de consolidação**, sem percentual global prometido ou número final de arquivos definido. Mover para histórico diminui a fila ativa, mas não o total. Uma eventual redução física depende da absorção completa e de como serão guardados os originais.

## Sequência de implementação a avaliar

- [x] **G0 — revisar o plano global:** 65 documentos nominalmente cobertos; enredo excluído; candidatos e incertezas registrados.
- [x] **G0.1 — orientar a consulta:** índice com busca delimitada, semáforo e critério de parada; riscos concretos registrados aqui. Não exige carregar esta matriz em toda tarefa.
- [ ] **G1 — fechar fontes e inventário de sobreposição:** para a frente escolhida, registrar seção atual → informação única → destino → pendência/consumidor. Não reler toda docs; aprofundar apenas os candidatos daquela frente. Aceite: nada retirado sem destino ou justificativa.
- [ ] **G2 — piloto de consolidação de interfaces:** revisar os sete planos citados, contratos e guias pertinentes. Produzir a matriz de aceite; comparar com originais antes de aposentar qualquer arquivo. Se a informação não couber sem ambiguidade, manter a fonte própria e registrar o motivo.
- [ ] **G3 — CasaLip e evidências:** reconciliar ações atuais e relatos; transferir somente duplicação comprovada, com datas e IDs preservados. Aceite: nenhum teste pendente desaparece e todo diagnóstico tem fonte.
- [ ] **G4 — City e mapas:** resolver primeiro sobreposições documentais; não corrigir narrativa, geometria, personagens ou código. Trabalho recente de Battle só entra após checkpoint documental e conferência de alterações concorrentes.
- [ ] **G5 — backlog, validação e navegação:** atualizar precedência por links, preservar IDs, reclassificar fontes datadas e retirar entradas substituídas do índice.
- [ ] **G6 — decidir retirada de arquivos absorvidos:** conferir conteúdo único, links/âncoras, backups e diff; registrar origem→destino, quantidade ativa e total antes/depois. Não excluir anexos nem enredo.

G1 é a condição para converter a triagem em plano executável por arquivo/seção; a tabela global sozinha não autoriza os movimentos. O roteiro de consulta já foi aplicado no índice; a consolidação de fontes continua pendente.

## Preservação e validação mínima

**Revisão global anterior:** cópia do plano e hashes dos 65 documentos em `output/backups/2026-10-29-refatoracao-documentacao/revisao-escopo-global/`; cobertura nominal 65/65 verificada naquela rodada.

**Rodada de orientação de consulta:** cópias do índice e deste plano, com hashes anteriores, em `output/backups/2026-10-29-refatoracao-documentacao/guia-consulta/`. Conferir diff desses dois arquivos, destinos dos links e preservação dos demais documentos. Reversão conforme registro de escopo no início deste plano. Não repetir a auditoria integral para validar dois arquivos Markdown.

**Em cada lote futuro:** declarar arquivos/trechos permitidos, dependências compartilhadas e consumidores; guardar cópias/hashes anteriores; preservar GUIDs, evidências, referências e alterações de outras tarefas. Validar conteúdo único/pendências, links/âncoras e diff delimitado. Não executar Unity ou testes de gameplay para validar edição Markdown.

Não confundir implementação, API, input real e aprovação visual. Documento antigo sem relato novo de problema também não comprova que o problema foi resolvido.

## Resultado e retomada

- **Entregue agora:** orientação de pesquisa e semáforo no índice existente, com riscos concretos registrados neste plano. A matriz mantém cobertura nominal dos 65 documentos fora de enredo. Nenhuma fonte foi movida ou apagada nesta rodada.
- **Validação documental:** comparação com hashes anteriores confirmou somente índice e plano alterados entre os 65 arquivos; os outros 63 permaneceram idênticos, sem adições/remoções. Destinos locais dos links desses dois documentos existem; a âncora nova de riscos corresponde à seção criada. Não houve teste de comportamento ou aprovação visual.
- **Limites:** as classificações globais ainda exigem reconciliação por seção nos candidatos; não há lista validada de arquivos descartáveis. Orientação salva não comprova economia de tokens medida nem garante cumprimento pela IA. Enredo e jogo não foram reavaliados.
- **Próximo passo:** executar G1 em uma única frente; começar pela reconciliação do backlog com entregas posteriores, onde já há risco comprovado de reabrir trabalho. Registrar IDs e destinos antes de retirar trechos. Depois avaliar o piloto de interfaces.
- **Unity/save:** não consultados nem operados; cena/Play atuais desconhecidos, nenhum save acessado. Estado de outros trabalhos permanece fora desta análise.
- **Backups:** versões anteriores e manifestos preservados, incluindo `guia-consulta/`; nenhuma limpeza pendente criada por esta revisão.
