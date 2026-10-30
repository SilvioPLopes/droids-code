# Plano de implementação — Validação e entrega

**Objetivo:** comprovar cada mudança no nível necessário, preservar progresso e tornar a retomada verificável. Esta sessão valida documentos; os testes de jogo abaixo são planejamento para futuras entregas autorizadas.

**Fontes:** [AGENTS.md](../../../AGENTS.md), [plano principal](README.md), [retomada vigente da CasaLip](../2026-10-16-correcao-casalip.md#retomada-vigente), [auditoria de cenas](../2026-10-17-auditoria-cenas.md) e contratos por sistema vinculados no índice principal.

**Regra de evidência:** leitura de código, teste de regra/API, input real e aprovação visual são evidências distintas. Um resultado antigo só dispensa repetição se o caso, a configuração e o código relevante permanecerem equivalentes. Não reabrir uma investigação encerrada sem evidência nova; não encerrar pendência antiga apenas porque foi citada aqui.

## VAL-01 — Preparar baseline e isolamento por entrega

**Prioridade:** P1. **Dependências:** escolher uma entrega e seu escopo; antecede qualquer mutação futura. **Arquivos candidatos:** plano da entrega e harness correspondente; não criar uma infraestrutura geral de testes como condição para começar.

- [ ] Registrar arquivo/objeto/propriedade permitido, consumidores compartilhados, comportamento anterior, critério de aceite e menor verificação capaz de demonstrá-lo.
- [ ] Conferir `git status` e diff dos arquivos afetados; identificar alterações alheias. Não limpar a árvore, trocar de branch ou sobrescrever arquivos para produzir um baseline artificial.
- [ ] Capturar valores e referências anteriores para reversão seletiva. Para assets Unity, preservar `.meta`, GUIDs e componentes não abrangidos.
- [ ] Antes de qualquer operação no Editor, confirmar projeto, cena, modo Play e alvo exato. Ler o procedimento MCP apenas se o conector for realmente utilizado.
- [ ] Preferir fixtures em memória/arquivo temporário e harness Editor para lógica pura. Declarar restauração de singletons, inscrições, aleatoriedade e recursos em `finally`.
- [ ] Se o teste precisar de save pessoal, obter o recorte autorizado, copiar arquivo/backup existente para destino fora do caminho ativo e registrar SHA-256, data e local. Sem backup verificado, não iniciar o teste que grava.
- [ ] Ao restaurar, conferir se o arquivo ativo mudou por progresso novo do usuário. Havendo mudança não atribuível ao teste, preservar ambos e não sobrescrever; registrar a pendência. Depois da restauração autorizada, verificar hash e integridade.
- [ ] Registrar o estado final do Unity e o destino de cada backup. Não deixar Play, processo ou painel de teste ativo sem informar.

**Aceite:** é possível apontar o que pode ser alterado, como provar o resultado e como voltar sem destruir trabalho alheio. Para documentação desta sessão, o baseline limita-se aos arquivos novos; não é necessário abrir o Unity ou copiar saves.

**Reversão:** restaurar somente o recorte identificado a partir da cópia/diff registrado. Reversão não comprova correção do sintoma nem aprovação visual.

## VAL-02 — Validar riscos e regressões necessárias

**Prioridade:** P2. **Dependências:** implementação do item correspondente e VAL-01. **Arquivos:** harnesses existentes ou propostos nos planos de cada sistema; assets apenas quando forem alvo da entrega.

Os casos detalhados permanecem em cada plano. Esta matriz define o mínimo de integração e evita executar uma bateria inteira a cada edição de texto.

| Eixo / entregas | Verificação mínima futura | Resultado exigido |
|---|---|---|
| TER-01/TER-02 | Programa finito que excede orçamento reduzido; cancelamento; erro após comando; comando retornando `false`; segunda execução válida | UI recupera; execução anterior não confirma efeitos; nova tentativa funciona |
| PUZ-01 | Sucesso, falha, contexto diferente, evento repetido e fechamento do terminal | Só o sucesso do contexto ativo conclui uma vez |
| TER-03 | Digitar várias linhas, mover cursor, executar, pedir ajuda, reabrir e consultar histórico | Código preservado; setas não substituem edição; ajuda e log legíveis |
| PUZ-02/MEC-01/MEC-02 | Código constante, código inicial defeituoso, solução geral, erro/cancelamento e valores extremos | Todos os casos exigidos passam; falha mostra divergência e não dá prêmio |
| CAD-01 | Primeira solução, solução posterior, rascunho, save anterior sem caderno e carga | Histórico limitado mantém primeira solução e última edição sem inventar dados antigos |
| EST-01 | Fixtures v6 e versão nova, quatro slots, raridade/bônus, slot vazio, item desconhecido e versão futura | Migração definida; HP restaurado após peças; incompatível não aplica metade nem sobrescreve original |
| EST-02 | Carregar sobre sessão suja; novo jogo; load inválido; falha de gravação; snapshot da Casa e autosave em City | Estado completo coerente; Casa não é substituída por autosave; snapshot não cresce recursivamente |
| INV-01/INV-02 | Troca e desequipar, mesma peça, HP acima de novo máximo, item sem efeito, sinalizador | Peça anterior retorna; HP não cresce por troca; consumo/retorno segue contrato |
| ECO-01 | Compra/venda válida, item protegido, saldo insuficiente, quantidade inválida, overflow e save falho | Nenhuma mutação parcial; preço correto; repetir gravação não repete transação |
| COM-01 | Atributos/resistências do catálogo, único derrotado, fuga proibida e ID inválido | Regras valem por UI e chamada de domínio |
| COM-02 | Ataque/item/fuga sob DOT e stun; efeito aplicado no turno; reaplicação; morte por DOT | Mesmo ciclo de efeitos, sem ações extras nem consumo incorreto |
| COM-03/COM-04 | B1/R1, custo 8 com saldo 7/8, fronteiras dos tetos, legado 6 versus preço 8, recarga e cópias renomeadas | Não gerar pontos; básico disponível; famílias bloqueiam bypass; prévia coincide com confirmação |
| COM-05/MEC-03 | Sequência, limiar de HP, anúncio, stun e análise quando implementada | Anúncio corresponde à ação ou à interrupção explicada |
| COM-06 | Registrar Casa, progredir/salvar em City, perder batalha, retornar e reiniciar; snapshot ausente/corrompido e falha de IO | Volta à Casa; desfaz progresso posterior de forma integral; falha não aplica estado parcial |
| MIS-01/MIS-02 | Conversa concluída/cancelada, progresso anterior ao aceite, vitória duplicada, entrega duplicada, IO falho | Progresso e prêmio uma vez; nenhum save intermediário em Battle |
| PRO-01 | Campanha nova, 120 XP, gasto 2+3, ordem alternativa de conversas, saldo gasto fora da sequência | Preparação alcançável sem comando de depuração e com recuperação explicada |
| PRO-02 | Redistribuir técnica 2 + DEX comprada 3; repetir; peça com bônus; cancelar; IO falho; recarregar/derrotar | Saldo final 5 uma vez; não devolver bônus de peça; checkpoint e ledger permanecem coerentes |
| COM-07 | Simular cinco turnos em cópias, fechar/reabrir e comparar estado/save | Relatório usa regras reais; zero XP/gold/itens/quest e nenhuma mutação persistente |
| UI-01/MUN-01/NAR-01 | Diário fechado, objetivo atual, NPC correto, consequência após carga e IDs cruzados | Progresso independente da UI; apresentação e conteúdo coerentes |
| MEC-04/MEC-05/MEC-06/MUN-02 | Apenas quando selecionados: requisito de peça, duas escolhas válidas, rodada atual, dois destinos de NPC | Sem bloqueio obrigatório, repetição indevida ou estado divergente após load |

**Harnesses existentes, sem executar nesta sessão:**

- `Assets/Editor/ValidacaoCombate.cs`: `ValidacaoCombate.Executar()` e menu `Droids Code/Validacao/Checkpoint e encerramento de combate`. Conferir precondições no momento de uso; verificar casos `OK` e ausência de `FALHOU`, não apenas ausência de exceção.
- `Assets/Editor/ValidacaoCasaLip.cs`: métodos de migração, contexto/origem do terminal, abertura repetida, primeiro diálogo e leitura Lua finita. Alguns casos requerem Play e montagem específica; escolher somente os atingidos e respeitar a retomada vigente.
- Harnesses de terminal, puzzles, inventário/economia, progressão, regras de combate e quests descritos nos planos são **novos propostos**. Não executar nomes presumindo que já existam. O implementador deve registrar entrada/menu e resultado de cada caso no plano da entrega.

**UI e input real:**

1. Registrar resolução efetivamente renderizada, escala de visualização da Game View, Canvas/Scaler e painel exato. Cobertura inicial: 1280×720 e 1920×1080, quando suportadas pelo alvo. Não confundir zoom do Editor com tamanho do Canvas.
2. Capturar referência anterior e posterior nas mesmas condições. Conferir texto longo, narração sem retrato, lista cheia, descrição de item e foco do editor quando atingidos pela mudança.
3. Verificar movimento e interação por teclado real, abertura/fechamento de cada painel envolvido e devolução de foco. Teste por API não substitui esse percurso.
4. Exigir legibilidade, enquadramento, ausência de cortes/sobreposição e preservação dos painéis relacionados. Não usar um valor de escala como prova de sucesso.
5. Se o usuário reservar a avaliação visual, marcar explicitamente “visual pendente do usuário”; não tentar encerrar o aceite com uma inspeção numérica.

**Segurança de execução:** nunca executar intencionalmente Lua infinito. Para demonstrar limitação, usar código finito com orçamento propositalmente menor e número de passos máximo conhecido; timeout do cliente não prova interrupção no Editor. Após uma falha de ferramenta, no máximo duas recuperações fundamentadas e consulta de estado antes de repetir mutação.

**Casos acrescentados à proposta de combate:** após perder oportunidade por stun, dois nomes diferentes de técnica não retiram a oportunidade protegida seguinte; conferir imunidade de alvo e limpeza ao sair. Para cooldown 2, uso não desconta a recarga nova, duas oportunidades próprias seguintes ficam bloqueadas e a terceira libera; item/fuga frustrada avançam a espera pelo mesmo ciclo. Relatório de treino de COM-07 é funcionalidade futura para o jogador, não autorização para executar experimentos de balanceamento nesta sessão.

**Aceite:** cada risco do recorte possui resultado observado ou pendência explícita. Compilação não comprova correção de comportamento; teste de regra não comprova usabilidade.

**Reversão:** harness não deixa save pessoal, eventos, singleton ou aleatoriedade alterados. Interromper e registrar se a limpeza falhar; não esconder limpeza pendente com nova execução.

## VAL-03 — Conferir execução e persistência nos alvos

**Prioridade:** P2. **Dependências:** TER-01, EST-02 e um percurso integrado; confirmação dos alvos de distribuição. **Arquivos:** nenhum pacote/configuração é alterado automaticamente por esta etapa. Código específico de plataforma, se necessário, requer recorte próprio.

**D-VAL-01:** confirmar se a entrega pretendida inclui Windows e WebGL. São alvos de validação propostos; a existência de código para uma plataforma não confirma que ela será publicada.

- [ ] Usar a versão Unity e os pacotes do projeto; registrar ambiente e plataforma. Não atualizar MoonSharp, Unity ou framework de teste para contornar uma incompatibilidade sem tarefa específica.
- [ ] Verificar comportamento cooperativo e responsividade do terminal no alvo real, inclusive cancelar/reabrir. Perfil de desempenho só após execução autorizada; os limites propostos não são garantia de tempo em milissegundos.
- [ ] Executar salvar → encerrar aplicação → reabrir → carregar no Windows, com fixture autorizada e estado representativo.
- [ ] Se WebGL fizer parte da entrega, verificar persistência após recarregar página e reabrir o mesmo contexto de armazenamento. Diferenciar gravação lógica de sincronização durável do armazenamento do navegador.
- [ ] Verificar erro de armazenamento indisponível/insuficiente com substituto controlado ou fixture, sem manipular dados pessoais do navegador. Mensagem deve permitir tentar gravar novamente, sem duplicar transação.
- [ ] Checar tratamento de recursos que dependem de threads, sistema de arquivos ou APIs específicas; não prometer que a compilação do Editor demonstra compatibilidade WebGL.
- [ ] Registrar cada build/caso uma vez. Repetir somente por mudança relevante, falha ou incerteza concreta.

**Aceite:** cada plataforma aprovada executa o percurso e conserva o estado após encerramento real. Se um alvo não for validado, informá-lo como pendente, sem atribuir equivalência ao teste do Editor.

**Reversão:** builds ficam em diretório de saída definido no recorte, sem substituir artefatos distribuídos. Não publicar nem alterar hospedagem como consequência implícita de validar.

## VAL-04 — Encerrar a entrega e avaliar a demo

**Prioridade:** P2. **Dependências:** DEM-01 e casos pertinentes de VAL-02/VAL-03. **Arquivos:** plano da entrega; contextos existentes somente em futura tarefa que autorize atualizar contratos implementados.

- [ ] Percorrer campanha nova e retomada intermediária com os recursos normais do jogador. Não usar pontos/itens de depuração para satisfazer o aceite de progressão.
- [ ] Validar missão de Dara, fonte do filtro, efeito visível local, puzzle de depuração, caderno e padrão de inimigo conforme DEM-01.
- [ ] Exercitar ao menos um caminho alternativo: puzzle antes do aceite, conversa em ordem diferente ou derrota com retomada. Escolher os demais conforme risco e falhas observadas.
- [ ] Registrar medidas simples localmente: tentativas por puzzle, dicas consultadas, erro mais comum, turnos/itens na luta, bloqueios e explicação do jogador sobre uma decisão. Não criar telemetria externa ou coleta de dados pessoais por padrão.
- [ ] Não usar metas arbitrárias para declarar o jogo divertido. Confrontar observações com as hipóteses: feedback ajuda a depurar, morador reage à ação, intenção muda a decisão de combate.
- [ ] Separar defeito demonstrado, ajuste de balanceamento e preferência. Novos experimentos ou mudanças de escopo exigem definição própria; relato de piora suspende ajustes por tentativa.
- [ ] Conferir diff, links/IDs afetados, arquivo salvo e ausência de alterações fora do recorte. Não incluir ajustes alheios em commit ou reversão.
- [ ] Atualizar o plano com último resultado, pendências, próximo passo, estado conhecido do Unity, destino dos backups e necessidade de aprovação visual.

**Critério de saída da demo:** nenhum bloqueio P1 aberto no percurso escolhido; requisitos P2 selecionados observados; saves compatíveis e equipamentos/progresso preservados; pendências visuais/input claramente resolvidas ou entrega explicitamente parcial. Recursos P3 não são exigidos para essa conclusão.

**Registro mínimo por entrega:**

| Campo | Preencher com evidência real |
|---|---|
| ID / data / base | Entrega, data e identificação da base, incluindo alterações locais relevantes |
| Editado e salvo | Arquivos/objetos/propriedades; onde conferir o diff |
| Regra/API | Caso, preparação isolada, resultado esperado/observado e limpeza |
| Input real | Sequência executada e resultado; “não executado” quando aplicável |
| Visual | Condições de renderização, comparação e quem aprovou; não presumir aprovação |
| Persistência | Fixture/backup, antes/depois, restauração verificada ou bloqueio |
| Pendência | Efeito no aceite, evidência que falta e próximo passo exato |

**Reversão:** documentar componentes que podem voltar juntos e incompatibilidades de save antes de aplicá-la. Preservar arquivo no formato mais novo; retornar código antigo não autoriza descartar dados novos.

## Verificação documental desta sessão

A validação local verifica os sete arquivos esperados, links Markdown relativos, IDs únicos de entregas, referências de dependência sem ciclo, cobertura dos 62 scripts e limitação das alterações desta tarefa à pasta solicitada. Não executa harnesses, build, Play ou scripts do jogo. O resultado consolidado e as decisões ainda abertas ficam no [README](README.md), evitando duplicar o registro da sessão.

**Próximo passo:** roteiro entregue para revisão, com resultado da conferência documental no README. Na execução futura, selecionar primeiro um recorte P1 e aplicar VAL-01; confirmar as decisões de design apenas dos itens selecionados, sem transformar todo o backlog em pré-requisito.
