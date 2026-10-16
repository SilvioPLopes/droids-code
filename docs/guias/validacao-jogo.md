# Validação do jogo

**Revisão:** 16/10/2026. Este roteiro foi criado na consolidação documental. Os casos abaixo estão **pendentes de execução nesta revisão**, salvo as observações anteriores identificadas explicitamente. Consulte o [plano](../plano-desenvolvimento.md) para dependências e o [contexto](../contexto-projeto.md) para comportamento existente.

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

## VAL-02 — Puzzle da água e autoria da conclusão

- **Ações:** antes de resolver, conversar com Apollo e tentar sair. Abrir terminal, executar entrada abaixo do alvo e depois a solução de referência finita do catálogo.
- **Esperado:** conversa não conclui puzzle; porta permanece trancada abaixo do alvo; solução grava conclusão e destranca uma vez. Dica/fala funciona e não concede recompensa duplicada.
- **Dependência:** INT-001/HIS-001. A implementação atual mistura a flag da conversa com a conclusão.

## VAL-03 — Revisita e load da CasaLip

- **Ações:** resolver, ir para City, retornar à CasaLip, abrir/fechar terminal e sair de novo. Repetir duas vezes. Salvar, encerrar a sessão, reabrir e carregar; repetir o percurso.
- **Esperado:** nova porta recebe estado resolvido; interação usa terminal válido; checker local não mantém porta destruída; não há Canvas/UI concorrentes ou novos erros.
- **Evidência anterior:** referências perdidas e porta trancada observadas por MCP; [INT-001](../problemas/casalip-retorno-porta-trancada.md) permanece aberto até esse percurso passar após correção.

## VAL-04 — UI, terminal e hipótese de movimento

- **Ações:** abrir terminal por E no mundo e por Esc → Terminal; digitar comando finito contendo `e`; reabrir; executar; fechar com botão e Esc. Repetir com Status, Bag, Droid e Loja, observando a origem da abertura.
- **Esperado:** uma abertura lógica por painel; VM não reinicia durante digitação; contador volta ao estado correto; menus não surgem por trás de outro painel; nenhuma coroutine é iniciada no painel inativo; Player retoma movimento ao fechar o último bloqueio.
- **Se houver travamento:** capturar direcaoInput, velocidade, constraints, simulated, componente habilitado, Time.timeScale, contador, painéis, contato de colisão, foco e Console antes de reiniciar.
- **Limite:** a hipótese de E acumular bloqueios vem da leitura; o responsável não reproduziu o travamento. Não atribuir causa sem capturar a falha.

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
| Console/MCP em 15–16/10: erro de log do terminal e referências perdidas na revisita | Confirmados nos relatos vinculados; correções pendentes. |
| Player habilitado, contador zero e Time.timeScale 1 no instante observado | Não demonstrou o travamento relatado; responsável também não o reproduziu. |
| Consulta MCP de testes | Nenhum teste retornado; nenhum teste executado nesta consolidação. |
| VAL-01 a VAL-11 após esta refatoração | Pendentes; nenhuma alteração de gameplay foi feita. |
