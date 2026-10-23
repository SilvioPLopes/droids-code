# Diagnóstico — carregamento e avisos no diálogo

## Correção de apresentação solicitada —23/10/2026

Usuário rejeitou a confirmação de Salvar na caixa de conversa: retorno de Salvar/Carregar deve aparecer no próprio menu, sem conversa ou mudança de painel pela confirmação. Esta instrução específica substitui a diretriz geral de mensagens no diálogo para esse fluxo, como já ocorre na loja.

Escopo antes de editar: somente MenuMundoManager, fixture ValidacaoSalvarCarregar e documentação vinculada. Reutilizar textoAviso do menu ilustrado, cuja área já existe; não alterar arte/prefab/Canvas ou o serviço corrigido. Remover fila e chamada do diálogo; salvar/erros mantêm menu aberto. Carregar pode trocar a cena pela operação real, mas reapresenta o mesmo menu com confirmação local, recompondo seu contador após CarregarDados zerá-lo. Backup anterior em `output/correcao-save/backup-feedback-local/`; reversão somente dos trechos próprios. Aceite: mensagem local, nenhum diálogo aberto/substituído, menu permanece ativo no salvar/erros, contador consistente; IO em fixture e hash pessoal preservado. Nova aprovação visual ainda necessária; captura do usuário comprova rejeição do fluxo anterior.

## Correção autorizada —23/10/2026

### Resultado aplicado e retomada

Corrigido o construtor padrão: deixou de chamar Application.persistentDataPath; serviço resolve caminho ao executar operações, preservando injeção de caminho explícito. Corrige a causa compartilhada sem editar os outros MonoBehaviours. Menu recupera `_salvamento` nulo, inclui ExisteSave/ler/carregar no tratamento de falhas e não anuncia sucesso antes da operação. Resultados vêm de CatalogoDeDialogos.AvisoDePersistencia e aguardam o diálogo existente estar livre; não substituem conversa/callback. Após apresentação aceita, menu fecha coordenando contador. Escape continua disponível enquanto um aviso aguarda dependência ausente; a operação não se repete por clique duplo com resultado pendente.

Antes/depois: teste .NET contra DLL real falhava no construtor por depender de UnityEngine.Application; depois passou construindo em Task/thread secundário e usando caminho explícito. Compilação runtime/Editor passou sem erros (dois avisos preexistentes de APIs obsoletas do Menu). Fixture `ValidacaoSalvarCarregar.Executar()` executada no Unity com IO somente em diretório exclusivo de output: criar/regravar/ler dados, HP/energia/posição, limpeza.tmp, menu com referência nula, ausência/JSON inválido/falha de escrita, confirmação no diálogo e conversa/callback preservados. Resultado em `output/correcao-save/validacao-unity.txt`.

Uma primeira execução da fixture usou DLL Editor anterior à correção de reflexão de Avancar (método público), interrompendo o teste. Verificada defasagem entre fonte e DLL; importação atualizou a fixture e execução passou. Não houve nova mutação do produto baseada nesse erro de teste. Recompilação derrubou o conector uma vez; consulta de estado posterior confirmou retorno antes de nova execução. Arquivos da fixture foram removidos no finally, inclusive na execução interrompida.

Save pessoal copiado para `output/correcao-save/backup/save-pessoal.json`, hash em arquivo.sha256. Hash final comparado e idêntico; nenhuma restauração necessária ou realizada. Código anterior dos três arquivos em backup/. Não houve teste usando Salvar/Carregar no arquivo pessoal nem CarregarDados/troca de cena. Portanto gravação/leitura e feedback foram testados isoladamente; clique real e retomada completa continuam validação distinta.

Conferência final somente leitura pelo serviço real no Editor: `new SalvamentoJson().LerDados()` passou para o save pessoal v6/destinoGame, sem restaurar estado ou carregar cena. Compilação final passou; diff dos arquivos da correção sem erro de whitespace. `git diff --check` geral também aponta espaços em metas do Droid alterados fora desta tarefa; preservados, sem limpeza incidental.

Estado Unity confirmado pelo conector: projeto DroidsCode, City, fora de Play antes de importar; não foi preciso encerrar jogo. Nenhuma cena/prefab/Canvas salvo ou alterado. Próximo passo: conferir Salvar/Carregar por input real no jogo; persistência de peças permanece tarefa separada EST-01 do plano Droid. Não ampliar esta entrega para migração de equipamentos. Sem commit/push nesta rodada; preservar mudanças alheias nos metas do DroidIlustrado.

Usuário autorizou corrigir Salvar/Carregar e avaliar se peças cabem na mesma sessão. Decisão: **separar persistência das peças**. Há sobreposição em SalvamentoJson, mas EST-01 altera DTO/migração e construtores de quatro peças/Bag/Droid; INV-01 acrescenta transação de equipamento. Não é necessário para recuperar os botões e ampliaria o risco desta correção. Plano do Droid permanece como próxima tarefa, sem mudanças de equipamento agora.

**Escopo antes de editar:** `Assets/Scripts/Estado/SalvamentoJson.cs` (resolução tardia do caminho padrão), `Assets/Scripts/Menus/MenuMundoManager.cs` (recuperação do serviço/erros/feedback), `CatalogoDeDialogos.cs` (textos de resultado), fixture nova `Assets/Editor/ValidacaoSalvarCarregar.cs`/meta e este plano/contrato de save. Consumidores compartilhados do construtor: Menu, Bag, diálogo, gatilhos, verificador de puzzle, combate e quests; evitar edições nesses consumidores usando correção no serviço. Preservar construtor com caminho explícito e política dos checkpoints. Sem editar cenas, prefabs, Canvas/Scaler, peças ou save pessoal.

**Backup/reversão:** copiar fontes anteriores para `output/correcao-save/backup/`. Somente diffs próprios em eventual reversão, conferindo mudanças posteriores. Registrar hash e copiar save pessoal antes de qualquer fixture que grave; testes usam diretório exclusivo temporário, nunca caminho padrão. Confirmar hash pessoal ao final; não restaurar por cima de progresso novo.

**Aceite mínimo:** construção padrão segura fora do ciclo de inicialização Unity, referências nulas do menu recuperadas, IO explícito em fixture grava/regrava/lê sem mudar cena, arquivo ausente/inválido/falha comunicados sem falso sucesso, feedback pelo diálogo existente e sem sobrescrever conversa/callback. Serviço não consulta Application.persistentDataPath durante construção. Input real e retomada de cena serão reportados separadamente.

Conector Unity indisponível. Fixture Editor pode importar e executar uma vez mediante marcador: confirmar projeto exato e estado/cena antes de qualquer ação; usuário autorizou sair de Play se necessário. Se sair, guardar rascunho/saída do terminal em output antes, sem gravar save pessoal. Não salvar cena nem repetir mutação após timeout. Não alegar que fixtures equivalem a clique real ou persistência completa de equipamentos.

## Retomada vigente — novo relato em23/10/2026

Usuário confirmou que Salvar e Carregar não funcionam e pediu investigar novamente, autorizando encerrar Play se necessário. Escopo antes de editar: leitura de handlers, construtor do serviço, logs atuais, metadados/JSON do save e configuração de cenas; registrar somente neste plano. Nenhum código, prefab, cena, save ou modo Play alterado nesta rodada. Reversão documental seletiva por diff; nenhum backup/restauração de progresso necessário para consulta somente leitura. Preservar os planos do menu Droid; esta investigação não implementa EST-01/INV-01.

### Evidências novas e causa sustentada

- `MenuMundoManager.cs:68` continua declarando `_salvamento = new SalvamentoJson()` como inicializador de campo de MonoBehaviour. `SalvamentoJson.cs:16` consulta `Application.persistentDataPath` imediatamente no construtor padrão.
- Log local `Logs/Editor.log:6964–6971` contém UnityException explícita: `get_persistentDataPath is not allowed to be called from a MonoBehaviour constructor (or instance field initializer)`. Stack: Application.get_persistentDataPath → SalvamentoJson..ctor linha16 → MenuMundoManager..ctor linha68. Última escrita do arquivo de log observada em23/10/2026 às16:21:51; isso data o arquivo, não o instante individual de cada exceção. Não alegar que esse stack corresponde ao último clique do usuário.
- Esse stack demonstra a origem da falha de inicialização antes não fechada no diagnóstico de22/10. Naquela inspeção viva, o serviço do menu já havia sido observado nulo; essa observação é histórica e não substitui leitura da instância atual. A cadeia fonte/log é compatível com a perda de inicialização que impede os handlers.
- `AoClicarSalvar` chama `_salvamento.Salvar()` diretamente. `AoClicarCarregar` chama `_salvamento.ExisteSave()` antes do try. Nenhum deles garante serviço válido; salvar não trata a exceção e o catch de carregar só cobre a leitura posterior, não a primeira chamada. Assim, inicialização interrompida pode impedir ambas as ações antes de qualquer gravação/carga ou feedback.
- Mesmo log contém `NullReferenceException` nos handlers reais: AoClicarSalvar linha307 (log2931–2932) e AoClicarCarregar linha313 (log2952–2953 e6352–6353). Essas são precisamente as primeiras chamadas ao serviço. Portanto não é apenas hipótese de falha futura: há registro de interrupção efetiva dos dois cliques por referência nula, além da falha de construção. Game também consta habilitada no EditorBuildSettings; ausência desse destino não explica essas exceções anteriores à leitura.
- A mesma consulta indevida aparece no log para BagUIManager, DialogoUIManager e GatilhoDeDialogo, todos com serviço criado em inicializador de campo. Isso identifica consumidores compartilhados da mesma causa; não autoriza refatorá-los nesta análise.
- Save pessoal encontrado em `Application.persistentDataPath` esperado pelo projeto DefaultCompany/DroidsCode: JSON legível, versão6, destinoGame, Droid presente, uma técnica; último horário de alteração21/10/2026 às15:53:27,911bytes. Somente leitura de metadados/parse; não imprimir progresso completo, não executar LerDados/CarregarDados/Salvar ou restaurar sessão. JSON sintaticamente legível não comprova que toda validação/retomada em jogo passou.
- A energia foi acrescentada ao payload e sua migração, mas o stack está na construção do serviço, antes de ler ou gravar esses campos. Não há evidência de a energia ou a nova arte de menu serem a causa imediata. Não foi feita auditoria histórica para atribuir quando o erro começou.

### Distinção necessária e correção recomendada

Falta de persistência das peças (SAVE-001/EST-01) faz equipamentos não serem reconstruídos ao carregar; ela é independente da exceção de inicialização que impede Salvar/Carregar. Corrigir só o DTO das peças não recupera os handlers.

Correção sustentada: retirar a consulta a API Unity dos inicializadores/construtores executados na serialização. Opções delimitadas: resolver o caminho sob demanda quando o serviço é usado no thread principal, preservando construtor com caminho explícito dos testes; ou criar/recuperar serviços no ciclo apropriado do MonoBehaviour. Na implementação considerar os consumidores compartilhados e proteção de instâncias já existentes, não apenas trocar uma referência no Inspector. Tratar falhas e apresentar resultado real no diálogo conforme diretriz vigente; hoje o fonte ainda escreve sucesso/ausência/erro em textoAviso do menu.

### Limites e próximo passo

Diagnóstico de fonte/log concluído para a falha de inicialização. Não houve novo teste por clique, reprodução isolada ou consulta à instância viva atual: conector Unity não está disponível entre as ferramentas desta sessão. Não foi necessário encerrar Play para obter essa evidência, nem houve tentativa de controle do Editor. A autorização para parar o teste foi preservada, mas não utilizada.

Próxima etapa: correção local com escopo/backup próprio; teste isolado de construção/importação, serviço previamente nulo e handlers; IO somente em arquivo de fixture, sem alterar save pessoal; depois input real Salvar/Carregar, feedback no diálogo e retomada de cena/estado. Nesta rodada foi editado somente este plano; produto e progresso permanecem intactos. Não declarar o problema corrigido.

## Retomada vigente — 22/10/2026

**Pedido:** investigar botão Carregar sem efeito, Salvar sem confirmação, ausência de fala de Apollo após puzzle e ausência de orientação na saída bloqueada da City. Usuário determinou que mensagens ao jogador devem usar a caixa de diálogo existente.

**Escopo desta rodada:** leitura delimitada dos handlers, persistência, conclusão do puzzle, transição e referências de cena/prefab; consultas MCP somente leitura quando disponíveis. Editar este plano, o contrato de interface e a navegação do índice. Não alterar scripts, cenas, assets, saves, modo Play ou estado do jogador. Planejamento City viva fica preservado e suspenso durante este diagnóstico.

**Alvos/dependências:** MenuMundoManager e botões Salvar/Carregar, SalvamentoJson/GerenciadorDeEstado, ControladorPuzzleAguaCasa/VerificadorDePuzzle, TransicaoDeCena da saída City, DialogoUIManager persistente. Consumidores compartilhados: CasaLip, City e Game; verificar o prefab do Canvas e seus overrides somente nos campos envolvidos.

**Aceite:** separar sintoma, caminho comprovado, causa demonstrada e incerteza de cada relato; documentar diretriz e próxima evidência necessária. Não inferir correção nem regressão apenas de documentação/teste antigo. Não iniciar experimentos de gameplay ou carregar/salvar progresso pessoal.

**Valores anteriores/reversão:** sem mutação de produto prevista. Documentação será aditiva; retirar somente os acréscimos próprios em eventual reversão, preservando trabalho anterior. Não produzir backup/restauração de save para consulta que não o modifica.

**Validação mínima:** ler handlers e referências atuais, conferir condições que descartam mensagens/carregamento, comparar integração efetiva quando consultável; links e diff documental. Nenhum teste que execute Salvar/Carregar/puzzle/transição nesta fase.

**Estado inicial:** relatos do usuário são evidência de falha no jogo; cena/Play atuais ainda não consultados. Consulta de ferramentas MCP respondeu. Save não acessado nem modificado.

**Relato confirmado pelo usuário:** puzzle afetado é o da água na CasaLip. Mensagens de sucesso e de porta da casa/saída principal da City não aparecem na caixa de diálogo desde a primeira tentativa, nem nas posteriores. Isso é evidência de falha, não pergunta em aberto. Não explicar o relato apenas pela regra de não repetir uma resolução já concluída.

## Evidências atuais — consultas somente leitura

- Projeto confirmado: `C:/Users/silve/DroidsCode/Assets`; City em Play, não pausada, cena limpa. Uma instância de MenuMundoManager e uma de DialogoUIManager encontradas em DontDestroyOnLoad. Não sair de Play ou mudar cena para testar sem combinar uma rodada própria.
- Menu inicializado, botões Salvar/Carregar atribuídos, interagíveis, cada um com um listener de runtime. Zero listeners persistentes não significa falta de ligação: o script registra em Inicializar. Menu estava fechado no instante observado; botões inativos na hierarquia nesse estado são esperados e não demonstram causa.
- Campo privado `MenuMundoManager._salvamento` **nulo na instância viva**. O fonte o inicializa na declaração, mas os handlers não garantem recuperação nem checam null. O momento/origem em que essa referência se perdeu não foi demonstrado; não atribuir automaticamente à última alteração de City.
- `new SalvamentoJson().ExisteSave()` retornou true. `LerDados()` passou sem restaurar nada: save v6, destino Game. Estado/droid da sessão presentes. Não foi necessário expor o save inteiro nem executá-lo; o arquivo não foi gravado, migrado ou carregado no estado do jogador.
- DialogoUIManager tem painel e textoFala atribuídos, fechado no momento consultado; listas de falas/escolhas e cache de retrato não nulos. Seu próprio `_salvamento` também está nulo: risco adicional confirmado para escolhas com consequências que tentem salvar, sem comprovar que isso impediu a primeira fala do puzzle.
- SaidaCidade tem TransicaoDeCena ativo, destino Game e textoDeBloqueio nulo. Na cena persistida: trancada=true inicialmente; na sessão consultada: trancada=false, com `ato1_sessao2=true`. Essa diferença é compatível com TrancaPorHistoria; não reproduz uma tentativa bloqueada e não invalida o relato.
- Flags runtime `ato1_casa_resolvida=true` e `ato1_sessao2=true`. Nenhum VerificadorDePuzzle ativo/inativo encontrado na City; o controlador do puzzle pertence à CasaLip, portanto não foi possível ler seu evento/corrotina no instante da falha relatada.
- Consulta filtrada do Console atual não encontrou registros dos handlers/evento investigados; trecho final do Editor.log também não trouxe stack correspondente. Ausência desses registros não comprova ausência de exceção na tentativa anterior. Não repetir buscas amplas de logs sem evidência nova.

## Diagnóstico por caminho

### 1. Carregar sem efeito — causa imediata demonstrada na sessão

[AoClicarCarregar](../../Assets/Scripts/Menus/MenuMundoManager.cs:311) começa acessando `_salvamento.ExisteSave()`. Como o campo está nulo, o caminho falha antes da leitura e antes de `CarregarDados`. O try/catch só envolve LerDados e não cobre esse acesso inicial. Botão ligado e save válido descartam, nesta sessão, as hipóteses simples de botão sem referência e arquivo ausente/inválido.

**Limite:** não foi provocado clique ou exceção de propósito. Causa sustentada por estado vivo + desreferência no fonte; origem da perda do campo ainda incerta. Também existem falhas de leitura previstas no código que só escrevem em textoAviso, sem diálogo; precisam do mesmo contrato de mensagens.

### 2. Salvar sem confirmação — operação e apresentação têm falhas distintas

[AoClicarSalvar](../../Assets/Scripts/Menus/MenuMundoManager.cs:305) chama `_salvamento.Salvar()` antes de escrever “Jogo salvo.”. Com o campo nulo, não chega nem à gravação nem ao aviso. Não afirmar que o clique salvou silenciosamente: nesta instância o caminho está impedido.

Mesmo com um serviço válido, a confirmação atual vai somente para `textoAviso` do menu, nunca para DialogoUIManager. Portanto recuperar a referência resolveria apenas a primeira falha, não atenderia à diretriz do usuário. Não há tratamento de falha de gravação nesse handler. A existência do save não prova sucesso do último clique; autosaves e sessões anteriores também o produzem.

### 3. Casa e saída da City bloqueadas — caminho antigo não migrou para o diálogo

[TransicaoDeCena](../../Assets/Scripts/Scripting/TransicaoDeCena.cs:68) recebe trigger bloqueado e chama MostrarMensagemDeBloqueio. Esse método só ativa/escreve TMP, retornando silenciosamente se textoDeBloqueio é nulo. Não há chamada à caixa de diálogo nesse caminho.

- City: referência foi removida com a etiqueta “desativado”, mas não foi implementado substituto no diálogo. Isso explica diretamente o silêncio quando a passagem está bloqueada.
- CasaLip: a porta ainda tem referência serializada ao TMP antigo (`textoDeBloqueio` fileID 988698170), mas usa o mesmo script: não apresenta diálogo. Estar atribuído não comprova que o TMP avulso tenha ficado visível; o relato se refere à ausência na caixa aprovada.
- TrancaPorHistoria calcula motivo (flag narrativa ou técnica insuficiente) e atualiza mensagemDeBloqueio; não a exibe por si. O texto existe, mas falta encaminhamento ao painel correto. Não reativar o aviso antigo nem liberar portão como solução.

**Causa demonstrada:** os dois caminhos de porta ainda usam o mecanismo de TMP separado; no da City seu destino foi explicitamente retirado. O portão já destrancado na consulta atual não permite testar bloqueio sem alterar progresso/estado, o que não foi feito.

### 4. Sucesso da água / Apollo — integração existe, ponto de interrupção ainda não demonstrado

- `CasaPortaApollo` contém quatro falas, incluindo Apollo/Lip. Não é conteúdo ausente do catálogo.
- CasaLip persistida contém ControlePuzzleAgua ativo com VerificadorDePuzzle e ControladorPuzzleAguaCasa habilitados, verificador associado, `idDoPuzzle=p_agua_casa`, flags de resolução corretas. `mostrarFalaAoResolver=false` desliga o aviso legado; não deveria desligar o controlador que apresenta o diálogo.
- [ControladorPuzzleAguaCasa](../../Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs:17) assina AoResolverPelaPrimeiraVez, fecha o terminal e tenta Mostrar(CasaPortaApollo). Se painel estiver ocupado/ausente, espera; desativar o controlador cancela a corrotina.
- [VerificadorDePuzzle](../../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs:278) marca resolvido, aplica porta/flag/progresso e **salva antes de invocar o evento da primeira fala**. Exceção nesse salvamento interrompe o evento depois de já alterar parte do estado. Há uma vulnerabilidade concreta na ordem, mas o campo/stack desse verificador no instante da falha ainda não foi capturado; não declarar exceção confirmada nele a partir do null de outro objeto.
- Ao restaurar a flag, Start reaplica conclusão sem evento/fala e VerificarVitoria retorna cedo se já resolvido. Isso explica o silêncio esperado numa restauração/repetição, mas **não explica sozinho a primeira tentativa reportada**. O estado atual marcado como concluído não distingue sucesso anterior íntegro de conclusão parcial interrompida.

**Causa ainda não fechada para o puzzle:** falta observar verificador, inscrição do evento, conclusão do salvamento e corrotina/UI no momento da primeira execução que deveria falar. Não foi aberto outro jogo, resetada flag ou repetido puzzle sobre o progresso do usuário. Testes históricos por API não comprovam o comportamento atual relatado.

## Diretriz registrada e recorte de correção futura

Contrato adicionado em [interface — mensagens ao jogador](../contextos/sistemas/interface.md#diretriz-de-mensagens-ao-jogador--definida-em-22102026), com navegação no índice. Sucesso, falha, dica e bloqueio devem chegar ao diálogo existente desde a primeira tentativa; preservar apresentação e coordenar painel ocupado/menu/terminal, sem substituir callback ou criar texto solto.

1. Garantir serviço de salvamento disponível nos handlers e informar resultado real no diálogo; tratar leitura/gravação sem consumir silenciosamente falhas. A origem do null deve ser distinguida da recuperação defensiva necessária.
2. Encaminhar bloqueios da casa/City ao diálogo com o motivo atual; repetição ao tentar novamente não deve duplicar conversa no mesmo contato nem consumir requisito de progressão.
3. Capturar o ponto de interrupção da primeira vitória da CasaLip antes de editar o fluxo; proteger consistência entre concluir, persistir e notificar conforme a causa demonstrada. Não ativar TMP legado como remendo.
4. Não ampliar para todo subsistema de save/quests ou alterar Canvas/Scaler. `_salvamento` nulo nas escolhas do diálogo é pendência compartilhada identificada; avaliar seu recorte antes de alterar esse consumidor.

**Validação necessária na correção:** service nulo/recuperação, save válido/ausente/falha sem tocar arquivo pessoal; primeira vitória, diálogo ocupado e restauração sem recompensar de novo; portas bloqueadas por cada requisito e nova tentativa; uma única caixa legível, nome/miniatura adequados e movimento/menus liberados ao fechar. Testes que gravem exigem backup e restauração verificada; input real e aceite visual continuam distintos.

**Último resultado:** quatro relatos analisados; causas imediatas de Salvar/Carregar e destino dos avisos de porta identificados. Puzzle tem cadeia e vulnerabilidade delimitadas, mas causa do primeiro disparo ainda não comprovada. Alterados somente este plano, interface e índice; scripts/cenas/progresso preservados.

**Conferência documental:** links novos do plano/interface e entrada nova do índice com destinos presentes; diff dos documentos rastreados sem erro de whitespace. Verificador de links apontou somente caminho antigo preexistente do catálogo visual no índice (`Entities/Npc's/...`), fora deste diagnóstico e preservado. Aviso Git de LF→CRLF não é falha funcional. Nenhum teste de gameplay realizado.

**Estado final/backups:** última consulta City em Play, não pausada e limpa; nenhum modo/flag/cena alterado pelo agente. Sem backup novo de save, pois só houve leitura validada. Destinos antigos de backup permanecem inalterados; nenhuma restauração/limpeza pendente criada nesta rodada.

**Próximo passo:** capturar somente o fluxo da CasaLip no estado de reprodução combinado com o usuário, sem resetar progresso por conta própria. Para Salvar/Carregar e portas, já há evidência para delimitar uma correção própria; registrar arquivos/objetos e rollback antes de implementar. A investigação não representa correção aplicada.
