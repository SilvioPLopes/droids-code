# Diagnóstico — carregamento e avisos no diálogo

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
