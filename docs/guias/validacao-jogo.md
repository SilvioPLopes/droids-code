# Validação do jogo

**Revisão:** 17/10/2026. A correção da CasaLip está implementada, com resultados parciais e pendências na [validação de 17/10](../revisoes/casalip-validacao-2026-10-17.md). Os casos do jogo continuam pendentes fora dos resultados explicitamente registrados. Consulte o [plano](../plano-desenvolvimento.md) e o [contexto](../contexto-projeto.md).

`Logs/CorrecaoCasaLip/green-harness-final.txt` registra seis OK recentes de fixtures: abertura repetida, primeiro diálogo inativo, leitura Lua finita, contexto, origem do terminal e migração. `fluxo-validacao.txt` registra separadamente integração por API e duas revisitas aprovadas (C05); a captura final 1280 foi inspecionada pelo agente principal e está legível. Permanecem pendentes reinício/load, sucesso integrado pelo menu/laço finito/diálogo ocupado, 1920 com narração/fala longa e input real. Computer Use foi interrompido pelo usuário com Escape, sem resultado de aprovação por input. O save original foi restaurado via MCP com SHA-256 igual ao backup.

## Preparar e registrar

1. Registrar versão Unity, branch, commit/base, alterações locais, data, plataforma, cena inicial e origem do save. Builds exigem resultados próprios.
2. Antes de reproduzir, preservar logs e estado relevantes. Registrar a última sequência do Console para distinguir erros antigos de novos; não apagar evidências necessárias.
3. Usar uma sessão de teste identificada. Testes de transação, puzzle e batalha podem autosalvar; preservar o save que se deseja manter antes de executá-los. Não concluir persistência após reinício apenas por recarregar a cena.
4. Conferir compilação, cenas de build, referências e uma instância ativa de Player/EventSystem nas cenas que os usam. Objetos persistentes também precisam ser incluídos na inspeção.
5. O MCP pode ler flags, referências, componentes, contador, colisões e Console. Registrar quem executou as ações de gameplay; inspeção posterior não equivale a reprodução automática.

Modelo de resultado:

| Campo | Preencher |
|---|---|
| Caso / tarefa | VAL-xx / ID do plano |
| Ambiente | Unity, plataforma, branch e commit/base com alterações locais |
| Estado inicial | Cena, posição, menus, flags e save relevantes |
| Ações | Sequência realmente executada |
| Esperado / observado | Descrição separada |
| Resultado | Passou, falhou, bloqueado ou não reproduzido |
| Evidência | Log, leitura MCP, captura ou relato identificado |
| Data / responsável | Data local e executor |

## VAL-01 — Novo Jogo e abertura direta de cenas

- **Ações:** em sessão nova, iniciar pelo MainMenu e escolher Novo Jogo; conferir CasaLip. Em sessões separadas, abrir CasaLip, City, Game e Battle diretamente para avaliar inicialização isolada.
- **Esperado:** fluxo principal inicia na casa; estado é criado corretamente; as dependências de teste isolado são conhecidas e não produzem null silencioso. Repetir Novo Jogo a partir de uma sessão existente, se houver acesso a essa opção, deve seguir uma política de reset definida.
- **Evidência:** cena, instâncias persistentes, Console e estado inicial. O default Game no script não substitui o campo CasaLip da cena.
- **Resultado parcial em 17/10:** MainMenu → CasaLip chamado por API; introdução terminou verdadeira, conclusão falsa e porta trancada (C01). Não equivale a selecionar Novo Jogo por input nem valida todas as cenas isoladas.

## VAL-02 — Puzzle da água e autoria da conclusão

- **Ações:** antes de resolver, conversar com Apollo e tentar sair. Executar variável ausente, texto e 90; depois solução de referência finita até 100. Em sessão separada, concluir por atribuição direta. Conferir as quatro falas de `casa_porta_apollo` pelas duas origens de abertura.
- **Esperado:** conversa não conclui puzzle; porta permanece trancada abaixo do alvo; solução grava conclusão e destranca uma vez. Dica/fala funciona e não concede recompensa duplicada.
- **Implementação:** INT-001/HIS-001 separaram introdução (`ato1_falou_apollo_casa`) de conclusão (`ato1_casa_resolvida`) e montaram verificador/controlador locais. Validar o fluxo completo antes de encerrar os critérios.
- **Resultado parcial em 17/10:** ausente/texto/90 rejeitados e código inicial correto (C02). Origem mundo/API com atribuição direta 100 concluiu, salvou, abriu a porta e as quatro falas; um evento/um menu durante diálogo, contador zero ao finalizar e reexecução sem novo save/evento/diálogo (C04). Laço finito integrado, origem menu e conversa de sucesso enquanto outro diálogo está ocupado seguem pendentes; o laço só passou no Runner isolado.

## VAL-03 — Revisita e load da CasaLip

- **Ações:** resolver, ir para City, retornar à CasaLip, abrir/fechar terminal e sair de novo. Repetir duas vezes. Salvar, encerrar a sessão, reabrir e carregar; repetir o percurso.
- **Esperado:** nova porta recebe estado resolvido; interação usa terminal válido; checker local não mantém porta destruída; não há Canvas/UI concorrentes ou novos erros.
- **Resultado em 17/10:** duas revisitas CasaLip → City → CasaLip passaram por API (C05), com harness confirmando porta aberta, terminal/referências vivos e verificador local único. Introdução/sucesso não repetiram e hash do save permaneceu igual. Evidência: `fluxo-validacao.txt`. Reinício de Play seguido de load (C06) permanece pendente; INT-001 ainda não está encerrado em todos os critérios.
- **Migração:** conferir DTO v5 com conclusão verdadeira preservada + introdução verdadeira, v5 sem a flag, idempotência e v6 intacta. A política conserva também conclusão ambígua do legado que só conversou; registrar esse limite. A fixture em memória passou; carregar save v6 real exige sessão própria.

## VAL-04 — UI, terminal e hipótese de movimento

- **Ações:** abrir terminal por E no mundo e por Esc → Terminal; digitar comando finito contendo `e`; reabrir; executar; fechar com botão e Esc. Repetir com Status, Bag, Droid e Loja, observando a origem da abertura.
- **Esperado:** uma abertura lógica por painel; VM não reinicia durante digitação; contador volta ao estado correto; menus não surgem por trás de outro painel; nenhuma coroutine é iniciada no painel inativo; Player retoma movimento ao fechar o último bloqueio.
- **Se houver travamento:** capturar direcaoInput, velocidade, constraints, simulated, componente habilitado, Time.timeScale, contador, painéis, contato de colisão, foco e Console antes de reiniciar.
- **Resultado parcial:** chamadas diretas antes da correção reproduziram Runner substituído e contador `1 → 2 → 1` em duas aberturas e um fechamento. O harness passou após a correção para abertura repetida e origem do terminal. Isso não comprova que esse defeito causava o travamento permanente relatado, que segue não reproduzido. Input real, tecla E na fala final, load por botão e os demais painéis exigem execução própria.

## VAL-05 — Diálogos e condições de história

- **Ações:** testar NPC com uma origem de interação, fala com múltiplas páginas, escolhas, fechamento, revisita e load. Conferir Dara/fila antes e depois da conclusão da casa e do comentário do Apollo.
- **Esperado:** primeira fala não é pulada, nomes/retratos correspondem, Player bloqueia somente durante interação, flags mudam na conclusão correta e falas de uma única vez não voltam após load.
- **Evidência:** Id do diálogo, gatilho, flags antes/depois e instância do painel. Falas manuais podem ter prioridade sobre o catálogo.

## VAL-06 — Portão e primeira técnica

- **Ações:** chegar ao portão sem flags; completar conversas; tentar sair sem técnica extra; aprender técnica com pontos obtidos pela rota normal; testar de novo.
- **Esperado:** mensagem distingue falta de história e de técnica; a rota fornece orçamento suficiente antes do portão; técnica libera a saída. Não usar pontos de teste para aprovar o percurso normal.
- **Dependências:** HIS-003 e montagem de City. Sem fonte de pontos confirmada, registrar bloqueado.

## VAL-07 — Inventário, equipamento e Loja

- **Ações:** usar cura fora/dentro de batalha, equipar peça, substituir slot, comprar/vender item comum e tentar vender item protegido. Salvar, encerrar sessão, reabrir e carregar.
- **Esperado após correções:** estoque, Gold e slots persistem; bônus não duplica; itens protegidos não são vendáveis; preço de revenda corresponde ao catálogo e à UI.
- **Lacunas atuais:** SAVE-001 e LOJA-001. Testar load na mesma instância de Droid não comprova persistência dos equipamentos em disco.

## VAL-08 — Batalha, efeitos e retorno

- **Ações:** entrar por zona e gatilho; atacar, usar itens, abrir/fechar listas, vencer, perder e fugir em sessões adequadas. Repetir com efeito ativo e com dano por turno fatal. Conferir retorno e próximo encontro.
- **Esperado:** ações/estoque/recompensas corretos, vitória/derrota imediata, regra de efeitos definida, origem/posição restauradas, contexto pendente limpo e restrições de chefe respeitadas.
- **Dependências:** COM-001/002 e SAVE-002. Balanceamento e restrições não são aprovados apenas porque o catálogo contém os campos.

## VAL-09 — Save durante combate

- **Ações:** numa sessão de teste preservada, gerar autosave por consumo de item e por vitória. Encerrar a sessão, reabrir e carregar cada caso; conferir cena, posição, inimigo, recompensas e contexto de retorno.
- **Esperado após política de checkpoint:** estado retomado é consistente, sem duplicar recompensas ou carregar uma batalha incompleta sem contexto.
- **Situação atual:** código grava cena ativa e usa posição zero sem Player; consequência precisa de reprodução controlada.

## VAL-10 — Ato 1 e missões

- **Ações:** percorrer a sequência montada, escolha de cada facção em sessões separadas, puzzle de energia, evento do Ivo e retorno narrativo. Aceitar/progredir/entregar missão de conversa, combate, coleta, puzzle e flag; salvar e carregar.
- **Esperado:** percurso principal alcançável, progressão contabilizada, uma recompensa por conclusão, missões opcionais não bloqueiam indevidamente o final e conclusão do Ato ocorre no evento aprovado.
- **Dependências:** HIS-002 e QUEST-001; conteúdo em catálogo não significa montagem concluída. Registrar trechos bloqueados.

## VAL-11 — Apresentação e builds

- **Ações:** verificar fundo/inimigo de Battle, listas, textos longos, scroll, diferentes resoluções e retratos. Depois dos fluxos principais, executar builds Windows e WebGL com resultados separados.
- **Esperado:** UI legível e ordenada, recursos carregados corretamente, controles/terminal/save funcionam conforme limitações da plataforma.
- **Referência:** [contorno do fundo](../problemas/battle-fundo-nao-renderiza.md). Aparência correta com UI.Image não comprova resolução da causa histórica de SpriteRenderer.

## Execução limitada de Lua

Depois de TER-001, criar um caso específico para exceder o limite de execução e conferir interrupção/feedback. O código atual não tem proteção: executar intencionalmente um laço infinito pode bloquear o Editor e não faz parte desta revisão documental.

## Observações anteriores e resultado atual

| Evidência | Estado |
|---|---|
| Relatos de setembro sobre puzzle, peças e painéis | Preservados no [histórico](../historico/README.md); não substituem regressão após mudanças. |
| Console/MCP em 15–16/10: erro de log do terminal e referências perdidas na revisita | Diagnóstico histórico; correções implementadas em 17/10. Revisitas por API passaram; mensagem/rolagem nas duas origens e load ainda exigem critérios próprios. |
| API em 17/10: C01/C02, C04 pela origem mundo e duas revisitas C05 | Passaram no escopo registrado em `fluxo-validacao.txt`; C06, menu/laço integrados, 1920/textos longos e input real permanecem pendentes. |
| Player habilitado, contador zero e Time.timeScale 1 no instante observado | Não demonstrou o travamento relatado; responsável também não o reproduziu. |
| Consulta MCP de testes | Nenhum teste retornado; nenhum teste executado nesta consolidação. |
| VAL-01 a VAL-11 após esta refatoração | Pendentes; nenhuma alteração de gameplay foi feita. |
