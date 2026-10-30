# Plano — interação dos NPCs e propostas de City viva

**Estado documental:** consolidado em 29/10/2026. Registra entregas e propostas anteriores; não comprova a montagem atual por inspeção nova do Unity.
**Objetivo:** manter a retomada de orientação, nomes e diálogo dos NPCs, com propostas de expansão vinculadas ao contrato City.

## Fontes e precedência

- [Contrato City](../contextos/cenas/city.md): valores e configuração de referência de nomes, escala, retratos e input.
- [Matriz de identidade e nomenclatura](2026-10-29-refatoracao-nomenclatura-ferrovale.md): casas, pessoas, representações e presença única.
- [Montagem de 29/10](../historico/2026-10-29-montagem-cidade-interiores-npcs.md): lote posterior; não repetir a montagem antiga deste plano.
- [Revisão dos interiores](2026-10-29-revisao-interiores-dialogos-colisoes.md): regressões de retrato, escolhas e circulação relatadas depois.
- [Registro integral anterior à consolidação](../../output/backups/2026-10-29-refatoracao-documentacao/lote-02/antes/docs/planos-implementacao/2026-10-22-orientacao-npcs-city.md): cronologia, valores anteriores, verificações e restrições por rodada.

A revisão de 29/10 prevalece sobre conclusões estruturais anteriores quando há regressão relatada. Teste por API não é input real nem aprovação visual.

## Entregas registradas e validação restante

| Recorte | Resultado registrado em 22–23/10 | Limite / pendência |
|---|---|---|
| Orientação | OrientacaoNpc configurado em Dara, Tácio e Homem da fila; poses 0/2/4/6. Homem da fila passou à arte Morador após escolha do usuário | Quatro direções e alvo nulo passaram por API em temporários; confirmar comportamento por interação real |
| Nomes e proporção | Montagem de nomes WorldSpace sob os pés; X/Y 1.33 conforme pedido, Z preservado | Prévia de Tácio inspecionada em 1920×1080; Dara/fila conferidos por propriedades. Aceite dos três no jogo não foi presumido |
| Aviso antigo | SaidaCidade/Canvas/desativado desativado e referência textoDeBloqueio desvinculada | Aviso de saída pelo diálogo continuou como tarefa separada; não reativar o texto antigo |
| Fala genérica | Principal disponível tem prioridade; alternativa repetível sem progresso/flags/save; loja de Tácio abre depois; proteção da tecla no fechamento | Harness validou fluxo; input e aparência continuam separados. IDs/configuração estão no contrato City |
| Retratos e apresentação | Retratos frontais exportados para Resources e cache defensivo de DialogoUIManager; adaptação local do diálogo | Houve reprovação visual. Não usar a fixture anterior como aceite; regressões posteriores seguem a revisão dos interiores |

Preservar a decisão de não redimensionar o protagonista para compensar NPCs. A hipótese antiga de normalizar PPU do protagonista não foi aceita como solução. Não copiar Canvas/Scaler da CasaLip para a raiz da City: isso afeta os menus relacionados.

Evidências e backups anteriores permanecem em `output/npcs-city/`: `City-antes-orientacao.unity`, `City-antes-morador.unity`, `nomes-escala/City-antes.unity`, `backup-falas-genericas/` e `correcao-dialogo/`. A prévia de nomes está em `nomes-escala/preview-tacio-1920.png`. Valores específicos e resultados de harness estão no registro integral, sem instrução para reaplicar etapas.

## City viva — rastreabilidade CV-01 a CV-06

As seis etapas abaixo foram concluídas **como análise**, não como implementação ou aprovação de todas as propostas.

| Etapa | Fonte responsável | Situação / próxima decisão |
|---|---|---|
| CV-01 — elenco, locais e falas | [Base e limites](../contextos/cenas/city.md#cv-01--base-encontrada-e-limites) | Catálogo e pasta de sprite não comprovam ator integrado |
| CV-02 — ocupação e função | [Núcleos de ocupação](../contextos/cenas/city.md#cv-02--proposta-de-ocupação-por-lugar) | Proposta inicial externa; reconciliar com distribuição e montagem posteriores |
| CV-03 — rotina e disponibilidade | [Rotina](../contextos/cenas/city.md#cv-03--rotina-sem-perder-acesso-aos-personagens) | Presença única primeiro; relógio e deslocamento exigem contrato próprio |
| CV-04 — quests e descobertas | [Sidequests e segredos](../contextos/cenas/city.md#cv-04--sidequests-segredos-e-pequenas-descobertas) | Preservar pré-requisitos, conteúdo opcional e cânone |
| CV-05 — cobertura de falas | [Falas e rascunhos](../contextos/cenas/city.md#cv-05--revisão-de-falas-e-primeiro-rascunho) | 14 entradas de revisão/rascunho; IDs propostos precisam de unicidade antes de catálogo |
| CV-06 — inserção em lotes | Seção abaixo | Ordem candidata; não é autorização para recolocar atores já montados |

### CV-06 — ponto de inserção de mais NPCs

A sequência L0–L4 era uma proposta de expansão. A montagem posterior já incluiu parte dos personagens: inventariar antes de escolher o próximo alvo, evitando duplicatas.

| Lote candidato | Conteúdo | Condição para executar |
|---|---|---|
| L0 — localização | Associar núcleos ao mapa | Confirmar locais representados, portão leste/norte, portas e circulação; não inventar XY |
| L1 — cotidiano | Moradora do poço, carregador e triadora | Conferir presença existente, variantes, retratos, fala e densidade |
| L2a — mercado/oficina | Sula, Onofre e Quirino | Revisar montagem e diálogos/serviços reais antes de acrescentar atores |
| L2b — beco/Guilda/estalagem | Pipo, Bruna e Nilce | Locais alcançáveis e texto sem spoilers; quarto3 continua reservado |
| L2c — cuidado/cerca | Benta e Zeca | Conferir sprites já produzidas e pontos externos; sem cura ou regra de saída nova |
| L3 — sidequest completa | Candidata q_agua_dara | Verificar oferta, itens, entrega, recompensa, persistência e reação pelo percurso normal |
| L4 — memória e retorno | Uma pista, um easter egg e reações | Estado narrativo validado; Nadir só no evento correspondente |

Ficha no plano do lote: ID/nome/falante → local e presença → atividade/orientação → sprite/poses/retrato → falas e condições → quest/serviço → objetos/propriedades permitidos → antes/backup/reversão → validação e resultado.

Aceite: atores distintos, nomes legíveis sem sobreposição, portas/passagens livres, uma resposta por E, fala/retrato corretos e retorno sem menus indevidos. Registrar resolução, zoom e Canvas/Scaler antes/depois para alterações visuais. Preservar protagonista e componentes compartilhados fora do escopo.

## Decisões abertas e retomada

- Definir postos e exclusão de representações pelo plano de identidade; não implantar relógio para simular chegada ou duplicar pessoa dentro/fora.
- Reconciliar Geiger de Tácio, quarto de Nilce, terceira entrega do cobre e portão leste/norte antes da fatia correspondente; não alterar catálogos por suposição.
- Avaliar a arbitragem ampliada de história, quest, assunto opcional e serviço. Não inferir implementação a partir da proposta CV-05.
- Registrar input real e aceite dos nomes, proporção, orientação e conversas; tratar retratos/escolhas com a revisão vigente.
- Próximo passo: inventário dirigido de identidade/presença, sem reexecutar os lotes anteriores.
- Nesta consolidação: somente documentação editada e salva; Unity não consultado, Play/cena atuais desconhecidos, nenhum save acessado. Cópia anterior no backup vinculado; sem limpeza de gameplay pendente.
