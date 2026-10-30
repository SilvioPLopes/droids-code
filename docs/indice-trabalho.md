# Índice de trabalho

Leia [AGENTS.md](../AGENTS.md) → este índice → plano da tarefa → apenas os subcontextos necessários. Andamento e evidências ficam nas fontes vinculadas; histórico é consulta sob demanda.

## Como a IA deve pesquisar

1. Identifique o alvo, o resultado pedido e as restrições. Escolha a entrada correspondente na tabela abaixo; os links são alternativas de consulta, não uma lista obrigatória de leitura.
2. Abra o plano da tarefa e localize escopo, estado, aceite e próximo passo. Leia apenas o contrato de cena/sistema necessário. Instruções já disponíveis na sessão não precisam ser relidas sem mudança.
3. Se faltar informação, formule a dúvida e procure o termo/ID em caminhos delimitados, por exemplo: `rg -n 'presença|rotina' docs/planos-implementacao/2026-10-28-moradores-e-rotinas-ferrovale.md`. Abra a seção pertinente, sem despejar toda a pasta docs no contexto.
4. Abra o trecho encontrado e seu contexto suficiente para distinguir proposta, execução e teste. Não siga todos os links em cadeia. Amplie a busca somente quando a fonte escolhida não responder à dúvida, registrando o que falta.
5. Se houver conflito, a instrução atual do usuário delimita a tarefa; compare datas e escopos dos registros. Uma seção chamada “retomada vigente” ou um checkbox isolado não prova o estado atual. Para corrigir comportamento, confira a evidência necessária no alvo real antes de editar.
6. Pare a pesquisa documental quando souber o escopo, contrato aplicável, pendência e validação mínima; correções também exigem causa sustentada. Ao terminar, atualize o plano existente com resultado, limite da validação e próximo passo. Não replique a narrativa em vários documentos.

## Semáforo de consulta

| Sinal | Fontes / situação | Conduta |
|---|---|---|
| **VERDE — entrada da tarefa** | Este índice, plano específico e contrato pertinente | Consultar o necessário para executar o pedido; verde não certifica que todas as seções estejam atualizadas. |
| **AMARELO — referência dirigida** | Guias, melhorias/backlog, outros planos, problemas | Abrir para uma dúvida concreta. Checklists e propostas precisam ser conciliados com entregas posteriores. |
| **VERMELHO — fora da carga inicial** | `historico/`, `revisoes/`, `contextos/analises/`, backups e versões anteriores | Consultar somente para evidência, regressão, proveniência ou reversão. Não repetir ações antigas como tarefas novas. |
| **VERMELHO — instrução possivelmente superada** | Premissas antigas, propostas não aprovadas, validações de outra data/condição, estados contraditórios | Suspender o uso daquele trecho como ordem; localizar a decisão/evidência que resolve o conflito. Não concluir que o arquivo inteiro é inútil. |

Alertas concretos e classificação dos **65 documentos fora de enredo** estão no [plano documental](planos-implementacao/2026-10-29-refatoracao-documentacao.md#riscos-de-consulta-confirmados). Essa matriz é ferramenta de limpeza, não leitura obrigatória em tarefas de jogo. Enredo ficou fora desta revisão; consulte-o apenas quando a tarefa narrativa exigir.

## Retomada

Organização documental: [inventário completo, riscos e consolidações propostas](planos-implementacao/2026-10-29-refatoracao-documentacao.md). Para casas e NPCs, começar pela [matriz de identidade e nomenclatura](planos-implementacao/2026-10-29-refatoracao-nomenclatura-ferrovale.md).

Interiores: [revisão de diálogos, colisões e circulação](planos-implementacao/2026-10-29-revisao-interiores-dialogos-colisoes.md). O registro da montagem fica vinculado no histórico; não repetir o lote nem considerar circulação aprovada.

## Navegação por necessidade

| Necessidade | Fonte de entrada |
|---|---|
| Prioridades e backlog | [Plano de desenvolvimento](plano-desenvolvimento.md), [melhorias por tema](planos-implementacao/melhorias/README.md) |
| Arquitetura e relações entre sistemas | [Contexto do projeto](contexto-projeto.md) |
| Casas, NPCs, identidade e presença única | [Matriz e plano de Ferrovale](planos-implementacao/2026-10-29-refatoracao-nomenclatura-ferrovale.md) |
| Portas e qualidade dos mapas City/vale/Battle | [Plano de mapas e interiores](planos-implementacao/2026-10-28-portas-interiores-e-mapas.md) |
| Fachadas, materiais e ocupação | [Concepção](planos-implementacao/2026-10-28-concepcao-interiores-ferrovale.md), [moradores e vínculos](planos-implementacao/2026-10-28-moradores-e-rotinas-ferrovale.md) |
| Colisões, escadas e diálogos dos interiores | [Revisão vigente](planos-implementacao/2026-10-29-revisao-interiores-dialogos-colisoes.md) |
| CasaLip: contrato e correções | [Contexto](contextos/cenas/casalip.md), [retomada da tarefa](planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente) |
| City: montagem e interação | [Contrato City](contextos/cenas/city.md) |
| Auditoria de cenas e reutilização de prefabs | [Auditoria e pendências](planos-implementacao/2026-10-17-auditoria-cenas.md) |
| UI compartilhada e método de composição | [Contrato](contextos/sistemas/interface.md), [interfaces sobre imagens](guias/interfaces-sobre-imagens.md) |
| Menus de exploração: escolher só o painel envolvido | [Terminal](planos-implementacao/2026-10-22-terminal-ilustrado.md), [mundo](planos-implementacao/2026-10-23-menu-mundo-ilustrado.md), [loja](planos-implementacao/2026-10-23-loja-ilustrada.md), [Bag](planos-implementacao/2026-10-23-bag-ilustrada.md), [Status](planos-implementacao/2026-10-23-status-ilustrado.md), [Droid](planos-implementacao/2026-10-23-droid-ilustrado.md) |
| Equipamentos e ícones | [Equipamentos Apollo](guias/equipamentos-apollo.md), [ícones de itens](guias/icones-de-itens.md), [aceite dos ícones](planos-implementacao/2026-10-23-sprites-unicas-itens.md) |
| Salvar/carregar e mensagens | [Estado e save](contextos/sistemas/estado-e-save.md), [diagnóstico/retomada](planos-implementacao/2026-10-22-diagnostico-avisos-e-carregamento.md) |
| Batalha e sua interface | [Contrato de combate](contextos/sistemas/combate.md), [artes e menus Battle](planos-implementacao/2026-10-29-battle-artes-menus.md) |
| Evento do boss no vale | [Lógica, prefab e técnicas](planos-implementacao/2026-10-29-evento-boss-vale.md) |
| Sprites de personagens | [Guia LPC](guias/criacao-sprites-personagens-lpc.md), [catálogo visual](../Assets/sprites/Entities/Characters/Documentation/character-catalog.md) |
| Direção visual de robôs, chefe e inimigos | [Guia de arte e prompts](guias/direcao-arte-robos-e-inimigos.md), [revisão e proposta do Ato 1](planos-implementacao/2026-10-29-direcao-visual-inimigos-ato1.md) |
| Narrativa | [Enredo](enredo/TCC_-_ENREDO_v2.md), [adendo](enredo/TCC_-_ENREDO_v2_ADENDO.md); o adendo prevalece |
| Montagem e validação por percurso | [Guia de montagem](guias/montagem-ato-1.md), [validação](guias/validacao-jogo.md); conferir a época de cada etapa |
| Apresentação das telas | [Plano da apresentação](planos-implementacao/2026-10-24-apresentacao-telas.md) |
| Operar o conector | [Procedimento MCP](guias/operacao-mcp.md) |
| Iniciar tarefa ou contexto de cena | [Modelo de tarefa](planos-implementacao/_modelo-tarefa.md), [modelo de cena](contextos/cenas/_modelo-cena.md) |
| Evidências e histórico | [Índice histórico](historico/README.md), [validação CasaLip](revisoes/casalip-validacao-2026-10-17.md) |

Planos em `planos-implementacao/` podem conter propostas ou testes pendentes; estar nessa pasta não comprova implementação nem aprovação visual. Rotinas de NPCs dependem do escopo do plano correspondente.
