# Plano de implementação — Terminal, puzzles e caderno de programas

> Execução futura: seguir [AGENTS.md](../../../AGENTS.md) e a skill `superpowers:executing-plans`, tarefa por tarefa. Delegar somente parcelas independentes e delimitadas. Este plano não autoriza iniciar implementação, Play ou experimentos.

**Objetivo:** permitir aprender Lua com execução recuperável, feedback compreensível e puzzles que avaliem resultados em situações diferentes.
**Arquitetura:** manter MoonSharp encapsulado em `DroidScriptRunner`, a fachada em `TerminalDroidApi` e a UI em `TerminalUIManager`; separar execução, avaliação pedagógica e aplicação de consequências persistentes.
**Tecnologias:** Unity 6000.5.8f1, C#, MoonSharp já instalado, uGUI/TMP, salvamento JSON existente. Não atualizar pacotes.
**Especificação:** [plano principal](README.md), decisões D-TER-01 a D-TER-03 deste documento e [adendo narrativo, parte D](../../enredo/TCC_-_ENREDO_v2_ADENDO.md).
**Estado:** proposta, não implementada. Responsável futuro por cada tarefa deve registrar seu nome e recorte antes de editar.

## Escopo, evidência e consumidores

| Evidência da análise de 20/10 | Consequência | Entrega |
|---|---|---|
| `DroidScriptRunner.ExecutarCodigo` usa `DoString` síncrono | Lua longo bloqueia a aplicação; não existe interrupção cooperativa no fluxo atual | TER-01 |
| API contém `TesteAdicionarPontos` público e ajuda o anuncia | Progressão de teste está disponível ao jogador | TER-02 |
| `AoExecutarCodigo` ocorre mesmo após erro, sem resultado/contexto no evento | Puzzle pode concluir com estado parcial e não distingue a origem da execução | PUZ-01 |
| Campo de código é limpo após executar; ajuda de lições não é consumida | Edição de programas maiores é trabalhosa | TER-03 |
| Verificações observam apenas números finais | Não comprovam ordem, relação entre variáveis ou generalização | PUZ-02 |
| Save não possui fonte das soluções nem rascunhos | Caderno e reaproveitamento narrativo ainda precisam de persistência | CAD-01 |

Consumidores atuais: `VerificadorDePuzzle`, os três scripts de `Casa`, `MenuMundoManager`, `LocalizadorDeTerminal`, `ApolloDica` e `ValidacaoCasaLip`. Mudanças na abertura/fechamento devem preservar origem mundo/menu e o bloqueio de movimento. O Runner atual é preservado em chamadas repetidas de `Abrir` enquanto aberto, mas é recriado numa nova abertura após fechar; não confundir esses dois casos.

Arquivos permitidos por tarefa estão abaixo. Cenas/prefabs só entram após identificação por GUID e autorização do recorte; Canvas/Scaler compartilhados não entram automaticamente.

## Decisões de design propostas

- **D-TER-01 — política de execução:** execução cooperativa em lotes, orçamento total e cancelamento. Sem executar a VM em `Task.Run` nem prometer interrupção de qualquer chamada C# arbitrária.
- **D-TER-02 — efeitos parciais:** comandos de configuração do Droid operam sobre uma cópia durante cada execução; confirmar a cópia somente em término bem-sucedido. Erro, cancelamento ou limite descartam a execução e invalidam sua VM, sem retirar pontos do jogador. Falha de um comando retornando `false` deve marcar a transação como recusada, mesmo sem exceção Lua. Reembolso não pode usar o preço atual para desfazer comandos antigos.
- **D-TER-03 — escopo pedagógico:** preservar conclusões antigas e o contrato atual da CasaLip inicialmente; introduzir avaliação com múltiplos casos em um puzzle novo/optativo. Mudar a exigência da casa requer tarefa própria. Não exigir a presença textual de `while`/`if` como prova de aprendizado.

Parâmetros iniciais propostos para TER-01: fonte de até 16.384 caracteres, até 100 linhas de saída de 512 caracteres por execução, 1.000 instruções por lote e no máximo 200 retomadas. São limites iniciais configuráveis para qualificação, não valores medidos de performance nem orçamento exato de instruções. Registrar o valor efetivo usado em Windows e WebGL.

## TER-01 — Execução limitada e recuperável

**Dependências:** nenhuma implementação anterior; definição D-TER-01. **Prioridade:** P1.

**Arquivos candidatos:**

- Alterar `Assets/Scripts/Scripting/DroidScriptRunner.cs`.
- Alterar `Assets/Scripts/Menus/TerminalUIManager.cs` somente ciclo de execução, cancelamento e feedback.
- Criar `Assets/Scripts/Scripting/LimitesDeExecucao.cs`.
- Criar `Assets/Editor/ValidacaoTerminal.cs`, harness manual sem execução ao importar.

**Viabilidade consultada:** pacote local `Library/PackageCache/org.moonsharp.moonsharp@8d260bd192ea`, `Runtime/DataTypes/Coroutine.cs` contém `AutoYieldCounter` e `Resume`; `Runtime/Script.cs` contém `CreateCoroutine`; `ScriptOptions` contém `DebugPrint`. Isso confirma APIs disponíveis, não sua integração nem compatibilidade do fluxo com WebGL. Nunca editar o cache do pacote.

**Contrato proposto:**

- `bool DroidScriptRunner.IniciarExecucao(string codigo, LimitesDeExecucao limites)`: recusa execução concorrente e valida tamanho antes da compilação.
- `EstadoExecucao DroidScriptRunner.AvancarExecucao()`: realiza no máximo uma retomada; estados `Executando`, `Concluida`, `Falhou`, `Cancelada`, `LimiteExcedido`.
- `void DroidScriptRunner.CancelarExecucao()`: impede novas retomadas; resultado terminal publicado uma vez.
- `ResultadoExecucao DroidScriptRunner.UltimoResultado`: só válido em estado terminal.
- Preservar `ObterVariavelNumerica(string)` para consumidores; proibir leituras de vitória enquanto executando.
- O legado `ExecutarCodigo(string)` deve ser migrado nos consumidores/fixtures da pasta; não manter uma entrada pública que contorne os limites.

**Passos:**

- [ ] Capturar diff anterior dos arquivos e mapear consumidores de `ExecutarCodigo`/`ResultadoExecucao`.
- [ ] Criar testes com programa finito curto e outro finito que exceda um orçamento muito pequeno; confirmar falha do contrato atual sem executar Lua infinito.
- [ ] Usar `LoadString`/corrotina da versão instalada e distribuir retomadas entre frames da UI; confirmar os estados reais retornados pelo pacote antes de escrever o adaptador.
- [ ] Auditar módulos expostos: impedir criação de corrotinas Lua sem orçamento, carregamento dinâmico e funções C#/biblioteca que gerem trabalho descontrolado. Expor apenas capacidades necessárias aos exercícios. Não confiar no nome “sandbox” como garantia de tempo/memória.
- [ ] Aplicar limites de fonte, saída e chamadas da fachada; truncamento de log deve informar o ocorrido.
- [ ] Desabilitar Executar durante execução; Cancelar continua disponível. Fechar, desabilitar ou trocar contexto cancela e desvincula o job.
- [ ] Descartar referências da corrotina cancelada/expirada; não retomá-la depois de fechar ou carregar save.
- [ ] Executar os casos finitos do harness, sem mutar save pessoal; qualificar em build somente na etapa VAL-03.

**Casos mínimos:** `x=2+3` termina com 5; laço finito com 10.000 iterações e orçamento de uma retomada produz `LimiteExcedido`; erro de sintaxe produz linha/mensagem; dois cliques não duplicam trabalho; fechar no meio cancela; saída longa é limitada; próxima execução finita funciona.

**Aceite:** interface responde durante execução distribuída; cancelamento/limite não deixam job ativo; nenhuma conclusão de puzzle é disparada nesses resultados. Funções nativas longas e alocação de memória são limites a documentar: a quota da VM não é preempção universal.

**Reversão:** reverter apenas o diff deste pacote e de seus consumidores em conjunto; preservar código do jogador antes de trocar versões. Não recolocar uma UI que anuncie Cancelar sobre um Runner síncrono.

## TER-02 — Fachada de produção e configuração transacional

**Dependências:** TER-01; coordenar validações de custo com COM-03 e cópia de estado com EST-02. **Prioridade:** P1.

**Arquivos:** alterar `TerminalDroidApi.cs`, `DroidScriptRunner.cs`, `TerminalUIManager.cs`, `PontosDeProgressao.cs` e `ValidacaoTerminal.cs`; criar `Assets/Scripts/Scripting/SessaoDeConfiguracao.cs`. Não expor `GerenciadorDeEstado` ao Lua.

**Contrato:** `SessaoDeConfiguracao(Droid original)` cria cópia profunda dos atributos, pontos e técnicas; `Droid DroidDeTrabalho` é a instância entregue à API; `bool Confirmar(out string erro)` copia somente os campos de configuração aprovados ao original; `void Descartar()` encerra sem alterações. Usar a regra existente de ajuste de HP por VIT na confirmação, sem copiar HP antigo por cima de mudanças externas. O jogo permanece bloqueado para ações concorrentes enquanto o terminal configura.

- [ ] Provar que uma alteração seguida de erro atualmente deixa mudança parcial; usar fixture sem singleton/save real.
- [ ] Validar quantidade positiva, enum definido, nome normalizado, limites e aritmética `checked` antes de qualquer gasto. Rejeitar enum numérico desconhecido e overflow com mensagem.
- [ ] Retirar `TesteAdicionarPontos` da fachada registrada no Lua e da ajuda; disponibilizar depuração só em harness Editor, fora do objeto `droid`.
- [ ] Aplicar D-TER-02: comando recusado marca a transação; terminar Lua sem exceção não basta para confirmá-la.
- [ ] Confirmar uma vez no sucesso; descartar em erro/limite/cancelamento/recusa. Criar VM limpa após descarte para que variáveis não façam parecer que uma operação recusada foi efetivada.
- [ ] Manter permissões diferentes: terminal de configuração admite gasto; avaliação de puzzle admite apenas leitura/simulação e nunca grava no Droid real.
- [ ] Atualizar a mensagem para distinguir “código executado”, “configuração aplicada” e “comando recusado”.

**Testes:** aprender técnica seguido de `error(...)` não muda pontos/técnicas; duas aprendizagens sendo a segunda inválida não aplicam a primeira; sucesso aplica uma vez; tentar técnica básica protegida não cobra; quantidades negativas e overflow não aumentam saldo; cancelamento não dá upgrade; função de teste indisponível no ambiente de produção.

**Aceite:** erros de aprendizagem não custam progresso; o terminal informa por que a configuração não foi aplicada.

**Persistência:** o sucesso altera a sessão. Salvamento explícito/autosave segue EST-02, sem gravar a cada chamada Lua nem em ambiente de avaliação.

## PUZ-01 — Evento de execução com contexto e conclusão única

**Dependências:** TER-01 e TER-02. **Prioridade:** P1.

**Arquivos:** alterar `TerminalUIManager.cs`, `DroidScriptRunner.cs`, `VerificadorDePuzzle.cs`, `ControladorPuzzleAguaCasa.cs`, `PuzzleAguaCasaChecker.cs` somente se houver consumidor ativo; atualizar `ValidacaoCasaLip.cs`; criar `Assets/Editor/ValidacaoPuzzles.cs`.

**Contrato proposto em `ResultadoExecucao`:** `string IdExecucao`, `string IdContexto`, `string CodigoFonte`, `EstadoExecucao Estado`, `bool Sucesso`, `string MensagemErro`, `int? LinhaErro`. O ID identifica uma execução, não uma tentativa por frame.

**Evento:** `event Action<ResultadoExecucao> TerminalUIManager.AoConcluirExecucao`. Migrar os consumidores juntos; não disparar eventos velho e novo para o mesmo verificador.

- [ ] Testar atribuição correta seguida de erro; esperado: porta/flag/recompensa inalteradas.
- [ ] Incluir contexto explícito da execução; verificador ignora contexto diferente, execução em andamento e resultados não bem-sucedidos.
- [ ] Capturar a fonte antes de qualquer mudança da UI, para CAD-01.
- [ ] Contar uma tentativa por execução iniciada no puzzle; erro pode gerar dica, mas não vitória.
- [ ] Separar avaliar condições de aplicar efeitos persistentes; restaurar flag existente não repete evento, XP nem fala de primeira resolução.
- [ ] Preservar a apresentação específica da casa e sua coordenação com diálogo já aberto.
- [ ] Conferir usos por GUID do checker legado antes de propor remoção. Enquanto coexistir, não permitir duas concessões para o mesmo puzzle.

**Testes:** contexto A não conclui B; execução falha com variável-alvo não conclui; sucesso duplicado do mesmo ID aplica uma vez; revisita reaplica porta sem prêmio; ausência de UI não apaga conclusão; fluxo com diálogo ocupado não substitui callback.

**Aceite:** resolver afeta somente o desafio ativo; falhas não liberam portas; comportamento atual aprovado da casa permanece protegido.

## TER-03 — Editor de código e ajuda do Apollo

**Dependências:** TER-01; PUZ-01 para feedback por tentativa. **Prioridade:** P2.

**Arquivos:** alterar `TerminalUIManager.cs`, `TerminalDroidApi.cs`, `ApolloDica.cs`, `CatalogoDeLicoes.cs`; ampliar `ValidacaoTerminal.cs`. Prefab/objeto do painel somente após mapeamento, sem alteração global de escala.

**Interfaces:** `string TerminalDroidApi.Explicar(string topico)` e `string ListarTopicos()` consomem catálogo; as chamadas devem também registrar o texto no log, pois retornar texto sozinho não o exibe. Associar `Script.Options.DebugPrint` à saída limitada da execução para `print(...)`.

- [ ] Preservar conteúdo e caret após executar; histórico deve ser acionado por comando explícito e não capturar setas usadas para editar múltiplas linhas.
- [ ] Distinguir Executar, Limpar campo e Reiniciar exercício; reiniciar recria VM/variáveis e apresenta código inicial, sem apagar solução salva ou progresso.
- [ ] Exibir erro com linha e dica; não traduzir todos os problemas como erro de sintaxe.
- [ ] Conectar lições existentes, tópicos desconhecidos e aliases; mostrar apenas comandos disponíveis naquele contexto.
- [ ] Oferecer consulta de variáveis conhecidas do exercício, com valor e tipo, sem despejar todas as globals internas da VM.
- [ ] Aplicar limites ao histórico e ao log; manter rascunho por contexto quando CAD-01 entrar.
- [ ] Fazer comparação visual e input real nas condições de VAL-02; verificar leitura, multiline, seleção, teclado e scroll.

**Testes:** executar não limpa código; seta move caret; histórico recupera programa por controle próprio; `print(5)` aparece; `explicar("laço")` retorna tópico while; tópico inválido lista ajuda; reiniciar restaura variáveis iniciais e não revoga vitória antiga.

**Aceite visual:** fonte legível, erro localizável, botões acessíveis e ausência de cortes em 1280×720 e 1920×1080. Registrar zoom da Game View e Canvas/Scaler efetivos, sem modificá-los por presunção.

## PUZ-02 — Casos de avaliação e depuração

**Dependências:** PUZ-01, TER-03; contrato de simulação sem mutações de TER-02. **Prioridade:** P2, primeiro em desafio optativo.

**Arquivos:** alterar `CatalogoDePuzzles.cs`, `VerificadorDePuzzle.cs`; criar `Assets/Scripts/Verificadores/AvaliadorDePuzzle.cs` e `Assets/Scripts/Verificadores/CasoDePuzzle.cs`; ampliar `ValidacaoPuzzles.cs`.

**Contrato:** `AvaliadorDePuzzle.Iniciar(string idPuzzle, string codigo)` cria uma avaliação; `Avancar()` consome no máximo um lote do executor; `ResultadoAvaliacao Resultado` contém casos aprovados, caso falho e esperado/obtido. Cada `CasoDePuzzle` define entradas numéricas, saídas esperadas e descrição; nova VM por caso. Não criar `Avaliar` síncrono que trave enquanto roda todos os casos.

- [ ] Manter modo legado de comparação para puzzles existentes; selecionar modo por definição, sem deduzir pelo texto.
- [ ] Criar desafio optativo “calibragem verificável”: entrada `forca_lida` fornecida por leitura controlada; jogador produz `previsao`.
- [ ] Validar forças 0, 5 e 9 com previsões 0, 10 e 18; uma constante positiva deve falhar nos casos que não atende.
- [ ] Separar entradas do simulador de variáveis que o aluno pode escrever. Sobrescrever a variável local da entrada não pode alterar a referência esperada pelo avaliador.
- [ ] Garantir ausência de `SubirAtributo`, aprendizagem, inventário e save na fachada de avaliação.
- [ ] Parar avaliação em erro/cancelamento/limite; mostrar um caso útil por vez e permitir repetir.
- [ ] Para desafios de ordem, usar operações controladas que registrem eventos, como carregar/abrir; validar o histórico de ações. Não alegar prova de ordem por estado final.
- [ ] Revisar a fala do puzzle de sequência: inverter somar/multiplicar três vezes a partir de 1 resulta em 50, não 32. Confirmar a aritmética em fixture antes da edição do texto.

**Casos adicionais:** função/expressão alternativa correta aceita; atribuição constante falha; `local` em vez de global recebe orientação clara sobre o contrato da saída; entradas negativas só quando fizerem parte do exercício; resultados não finitos rejeitados; todos casos de uma tentativa contam uma tentativa.

**Aceite:** feedback permite explicar o erro; soluções equivalentes são aceitas; fontes iniciais não sobrescrevem as entradas dos casos; nenhuma consequência de mundo é aplicada antes de concluir a avaliação inteira.

## CAD-01 — Caderno persistente de programas

**Dependências:** PUZ-01, EST-01 e EST-02; independente de lançar Ato 3. **Prioridade:** P2.

**Arquivos:** alterar `DadosDoJogo.cs`, `GerenciadorDeEstado.cs`, `SalvamentoJson.cs`, `TerminalUIManager.cs`; criar `Assets/Scripts/Scripting/RegistroDePrograma.cs`; ampliar `ValidacaoTerminal.cs` e `ValidacaoPersistencia.cs`.

**Registro proposto:** `idPuzzle`, `rascunho`, `primeiraSolucao`, `ultimaSolucao`, `tentativas`, `tentativasAtePrimeiroSucesso`, `concluidoEmUtc`. Sem usar flags booleanas para armazenar texto; lista serializável no DTO, mapa de acesso na sessão. Limitar fonte conforme TER-01 e guardar primeira/última solução, sem histórico ilimitado.

- [ ] Evoluir schema na próxima versão disponível, com lista vazia para saves antigos; não fabricar fonte ou tentativas anteriores.
- [ ] Capturar o código que passou e a contagem real; primeira solução não muda quando o jogador melhora a última.
- [ ] Separar rascunho de solução validada; carregar rascunho não executa código automaticamente.
- [ ] Persistir registro junto da conclusão e da flag, em uma unidade de salvamento; fechar terminal pode manter rascunho em memória até o próximo salvamento válido.
- [ ] Mostrar exercícios visitados, código guardado e ação “carregar no editor”; impedir perda silenciosa de rascunho ao trocar.
- [ ] Testar carregar em nova instância, migração sem registros, Unicode/newlines e falha de gravação sem apagar arquivo anterior.

**Aceite:** jogador revisita seu programa depois de reiniciar o jogo; programa guardado corresponde à tentativa validada; save antigo abre normalmente e informa quando não há fonte histórica.

## Retomada e encerramento deste eixo

- **Editado/salvo:** somente este plano documental.
- **Testado no comportamento/aprovado visualmente:** não executado/não solicitado.
- **Próximo passo:** TER-01, após delimitar execução autorizada; TER-02 não deve ser considerado resolvido apenas por adicionar corrotina.
- **Backups/Unity:** nenhum save acessado, nenhuma cena consultada ou alterada. Em implementação futura, registrar pré-estado e backup conforme [validação](06-validacao-e-entrega.md).
