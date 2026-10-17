# Plano de desenvolvimento

**Revisão:** 17/10/2026. Consulte primeiro o [contexto](contexto-projeto.md). Este arquivo registra trabalho e critérios de conclusão; descrições completas de comportamento permanecem no contexto ou no relato vinculado.

**Foco acordado para a retomada:** finalizar a demo por partes, começando pela CasaLip e aproveitando o trabalho existente. As correções locais estão implementadas, com validação parcial na [revisão de 17/10](revisoes/casalip-validacao-2026-10-17.md): introdução, casos inválidos, sucesso por 100/mundo e duas revisitas passaram por API; seis fixtures passaram novamente, com captura 1280 legível e oito direções por API. Ainda faltam reinício/load, sucesso integrado por menu/laço/diálogo ocupado, 1920/narração/fala longa e input real. A [revisão da casa](revisoes/casalip.md) separa esses resultados do diagnóstico anterior. Caminhos e chaves ficam no [índice de trabalho](indice-trabalho.md).

**Coleta histórica de 16/10, 22h57–23h01:** [capturas e diagnóstico](problemas/casalip-apresentacao-dialogo.md) confirmaram a falha de revisita e detalharam `UI-003/VIS-001`. Em 17/10, controlador local, flags separadas, associação persistente, UI/contexto, sucesso `casa_porta_apollo`, layout e artes disponíveis foram implementados. A matriz tem resultados parciais, sem encerramento global. O usuário interrompeu Computer Use com Escape; input real não foi aprovado. O save original foi restaurado via MCP com SHA-256 igual ao backup.

## Prioridades e estados

- **P1:** integridade de progresso, continuidade do jogo ou risco de travamento.
- **P2:** integração funcional e cobertura de validação.
- **P3:** organização e apresentação sem bloqueio atual comprovado.
- **Confirmado:** evidência de código/configuração ou execução especificada na tarefa.
- **Em investigação:** existe hipótese, mas a causa ou o comportamento precisa de teste.
- **Não reproduzido:** o sintoma relatado não foi confirmado; não há correção atribuída.
- **Proposto:** melhoria ainda sem implementação; não equivale a decisão de design aprovada.
- **Implementado; validação em andamento:** código/montagem alterados; faltam critérios de execução identificados no registro.
- **Concluído:** critérios atendidos, com resultado e evidência registrados.

## Primeiro ciclo: estado, UI e persistência

| ID | Prioridade e estado | Trabalho | Dependência | Critério de conclusão |
|---|---|---|---|---|
| INT-001 | P1 — C05 passou em duas revisitas por API; C06 pendente | [Retorno à CasaLip](problemas/casalip-retorno-porta-trancada.md): nova porta/terminal/referências válidos e save inalterado nas duas revisitas, sem repetição. | Nenhuma. | Resolver → City → CasaLip duas vezes; porta e terminal válidos; salvar, reiniciar e carregar mantém a conclusão. Ver VAL-02/03. |
| HIS-001 | P1 — separação confirmada por API; migração fixture OK; validação de City pendente | Introdução sem conclusão e resolução por 100 confirmadas. Legado verdadeiro preservado com ambiguidade registrada. | Coordenar com INT-001. | Em sessão nova, conversar não conclui puzzle; resolver grava conclusão; progressão de City continua acessível. |
| HIS-003 | P1 — risco de progressão por leitura; percurso não validado | Confirmar e integrar a fonte de pontos anterior ao portão que exige técnica extra. O orçamento inicial é zero; a correção de persistência da casa não adicionou pontos para técnicas. | HIS-001 e desenho de onboarding. | Uma sessão nova aprende técnica mínima antes de sair de City usando a rota normal, sem comando de teste. Ver VAL-06. |
| UI-001 | P1 — recorte CasaLip implementado, fixtures OK; validação global pendente | Abertura/fechamento de terminal/diálogo e origem do menu corrigidos. Conferir input real e demais painéis/caminhos de load. | Testar junto de INT-001. | Cada painel adquire/libera seu bloqueio uma vez; contador zero sem painéis; sessão Lua não reinicia durante digitação. Ver VAL-04. |
| SAVE-001 | P1 — lacuna de serialização confirmada | Persistir peças equipadas e sua identidade/atributos. | Definir compatibilidade com saves anteriores. | Equipar, salvar, encerrar sessão, reabrir e carregar preserva slots, bônus e quantidade consumida. Ver VAL-07. |
| SAVE-002 | P1 — fluxo de código confirmado; consequência precisa de teste | Definir checkpoint de batalha, cena/posição de retorno e restauração após autosave. | Definir política de retomada de combate. | Autosave por item/vitória seguido de reinício não produz batalha sem contexto, retorno incorreto ou recompensas duplicadas. Ver VAL-08/09. |
| TER-001 | P1 — limitação de código confirmada | Limitar execução de Lua para que loops sem fim não bloqueiem a aplicação. | Escolher mecanismo compatível com MoonSharp e WebGL. | Código que excede o limite é interrompido com feedback, preservando a responsividade. Não testar loop infinito antes de existir proteção. |
| TER-002 | P2 — correção implementada; validação final em andamento | [Diagnóstico anterior do log inativo](problemas/terminal-log-painel-inativo.md): painel ativa antes do log e coroutine exige componente ativo. | Pode acompanhar UI-001. | Abrir pelo mundo e menu não gera o erro; primeira mensagem e rolagem ficam corretas. Ver VAL-04. |
| OBS-001 | P3 — não reproduzido | Observar eventual travamento permanente do Player. | Reabrir investigação somente com reprodução/evidência. | Registrar sequência, input, MenuAberto, contador, Rigidbody2D, colisões e Console antes de atribuir causa. Sem correção definida. |

Próximo passo: executar C06 e as pendências de integração/input/visual da CasaLip registradas acima; C05 já passou por API. Depois tratar HIS-003, SAVE-001/SAVE-002 e TER-001 conforme o recorte acordado. A entrega parcial não encerra o primeiro ciclo inteiro.

## Segundo ciclo: completar as integrações do Ato 1

| ID | Prioridade e estado | Trabalho | Dependência | Critério de conclusão |
|---|---|---|---|---|
| HIS-002 | P2 — parcial; quatro falas CasaLip passaram pela origem mundo/API | Sucesso e reexecução sem repetição confirmados; integração menu/laço/diálogo ocupado pendente. Demais triggers seguem o [guia](guias/montagem-ato-1.md). | Primeiro ciclo; conferir roteiro vigente. | Percurso principal é alcançável, flags são coerentes e revisitas não repetem recompensas. Ver VAL-05/06/10. |
| QUEST-001 | P2 — integração ausente | Ligar diário ao menu, oferta/entrega de NPC e eventos de conversa/vitória. Definir se precisa de GatilhoDeQuest. | HIS-001 e eventos de gameplay. | Aceitar → progredir → entregar funciona e persiste; missões automáticas não dependem de abrir o diário. |
| NPC-001 | P2 — proposto | Organizar NPCs por identidade, diálogo, condições e prefab; mapear dados do catálogo para objetos reais. | HIS-002/QUEST-001. | Cada NPC montado tem referência de identidade e uma origem de interação; indicadores e falas correspondem ao estado. |
| NPC-002 | P2 — proposto | Implementar deslocamento/rotina somente após definir horários, pontos de destino e tratamento durante diálogo. | NPC-001 e decisão de design. | NPC chega a destinos válidos, pausa interação/movimento conforme regra e retoma sem atravessar colisões. |
| COM-001 | P2 — pendência de design e código | Definir empilhamento/renovação/bloqueio de efeitos e limpeza ao terminar batalha. | Decisão explícita de comportamento. | Aplicações repetidas seguem a regra escolhida; efeitos não vazam indevidamente para outra luta. |
| COM-002 | P2 — integração parcial | Consumir PermiteFugir/EhUnico/EhChefe, limpar contexto também na fuga e retornar à cena correta. | SAVE-002 e HIS-002. | Restrições de chefe/único são respeitadas e o próximo encontro não herda flag/inimigo indevido. |
| LOJA-001 | P2 — divergência entre catálogo e UI | Aplicar preço de venda e restrições de itens; definir autosave de transações. | Política de persistência. | Itens de missão/facção protegidos; preço da UI corresponde ao recebido; reinício conserva a transação. Ver VAL-07. |
| TER-003 | P2 — integração ausente | Expor ajuda pedagógica do CatalogoDeLicoes na API do terminal. | Definir comandos e mensagens. | Comando documentado retorna conteúdo válido, lida com tópico desconhecido e aparece na ajuda da UI. |
| TEST-001 | P2 — parcial: harness CasaLip implementado, primeiros checks OK | Harness Editor manual cobre UI, contexto, migração e estrutura da casa; ampliar cobertura conforme fluxos corrigidos. | Interfaces testáveis; isolamento de aleatoriedade quando necessário. | Testes verificam regras e regressões relevantes; resultados são registrados com versão/commit. |

## Organização e acabamento

| ID | Prioridade e estado | Trabalho | Critério de conclusão |
|---|---|---|---|
| EST-001 | P3 — proposto | Redistribuir Scripting por responsabilidade, preservando `.meta` e referências. | Estrutura aprovada, compilação e cenas verificadas; contexto atualizado com os caminhos reais. |
| EST-002 | P3 — proposto | Extrair itens/recompensas de BattleManager; reduzir dependências diretas de UI/estado global; revisar invariantes de domínio. | Regras cobertas por testes; comportamento existente preservado. |
| EST-003 | P3 — proposto | Confirmar referências e remover ItemConsumivel legado; decidir uso de CustoResistenciaPorNivel. | Busca e validação sem consumidores restantes antes de remoção. |
| UI-002 | P3 — pendente de revisão | Revisar scroll, layout, fontes, ícones e descrição de itens em Bag/Loja/Battle. | Listas legíveis em resoluções-alvo, sem sobreposição ou conteúdo inacessível. |
| UI-003 | P2 — captura 1280 legível; 1920/narração/fala longa pendentes | [Janela de diálogo da CasaLip](problemas/casalip-apresentacao-dialogo.md): âncoras, escala unitária, nome/retrato e Canvas escalável ajustados. | Janela ampla e legível, texto paginado sem corte e moldura/retrato proporcionais em pelo menos duas resoluções. |
| VIS-001 | P2 — assets/oito direções por API OK; input real/física pendentes | [Sprites/retratos CasaLip](problemas/casalip-apresentacao-dialogo.md): Lip com arte disponível e Apollo idle/retrato LPC provisório, etiqueta no mundo. | Assets escolhidos coerentes no mapa e retrato; nome acompanha Apollo sem sobrepor a UI; componentes e referências do Player preservados. |
| UX-001 | P2 — pendente | Validar onboarding e adequação pedagógica com jogador iniciante. | Registrar entendimento, bloqueios e evidência de aprendizado; considerar que valor final de variável não comprova uso de while/if. |
| BAL-001 | P3 — pendente | Playtesting de custos, preços, XP e dificuldade. | Alterações de balanceamento justificadas por resultados, preservando todas as rotas de facção. |
| LIC-001 | P3 — pendente de confirmação | Confirmar licença e adicionar arquivo correspondente se aprovado pelo responsável. | README e arquivo de licença concordam; licenças dos assets/dependências são consideradas. |
| DOC-001 | Concluído — 16/10/2026 | Consolidar documentação e preservar originais. | 98 links relativos e nomes conferidos; inventário de 59 scripts reconciliado; oito originais preservados byte a byte; 183 arquivos protegidos idênticos à linha de base desta tarefa. |

O [problema de fundo de Battle](problemas/battle-fundo-nao-renderiza.md) permanece **contornado** conforme relato histórico; uma correção de causa raiz ainda não foi demonstrada. Reavaliar sua prioridade se voltar a afetar o jogo.

## Propostas de longo prazo

Som ambiente, feedback de combate, transições, múltiplos slots de save, Estalagem/Ferreiro, Apollo Debugger com destaque de linha, ScriptableObjects/Factory, Cartuchos de Código e desequipar continuam propostas. Modo Puzzle separado requer demonstrar uma necessidade não coberta pelo verificador atual. Nenhum desses itens deve ser apresentado como implementado só por constar no roteiro.

## Pendências antigas reconciliadas

| Registro antigo | Estado que substitui a pendência |
|---|---|
| EscreverNoLog ausente | Método público presente em TerminalUIManager; não há mais essa divergência de contrato. Não equivale a uma compilação nova do projeto. |
| HIT/FLEE e crítico sem confirmação | Implementados em Droid; constantes lidas em TabelaDeCombate. |
| Catálogos com 10 quests/13 NPCs | Código atual contém 18 quests/16 definições de personagens. |
| Colar itens de facção manualmente | Entradas já existem no CatalogoDeItens; conferir obtenção e proteção na integração. |
| Verificador descrito apenas como v1 | v2 por Id é vigente, com fallback manual. |
| ItemConsumivel possivelmente alimentando batalha | BattleManager atual lê inventário/CatalogoDeItens; legado é candidato de EST-003. |
| Drop possivelmente perdido após Loja | AplicarRecompensas existe e salva; ids e valores de conteúdo ainda exigem validação. |
| Vitória/derrota detectada só no participante óbvio | BattleManager atual verifica ambos nos dois fluxos de ataque. |

## Como encerrar uma tarefa

Registrar ID, alteração, arquivos envolvidos, data, commit/base com alterações locais e testes executados. Atualizar a seção correspondente do contexto e o guia afetado; o detalhe de um bug fica no relato, sem duplicação completa aqui. Teste planejado continua pendente até existir resultado observado.
