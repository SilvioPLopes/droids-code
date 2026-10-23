# Contexto da interface

- [MenuMundoManager](../../../Assets/Scripts/Menus/MenuMundoManager.cs) mantém o Canvas entre cenas e controla a origem de painéis. Terminal aberto no mundo não deve revelar o menu ao fechar.
- [TerminalUIManager](../../../Assets/Scripts/Menus/TerminalUIManager.cs) inicializa uma vez; abertura repetida preserva Runner e contador. `DefinirContextoPuzzle`/`LimparContextoPuzzle` controlam ajuda e código inicial sem apagar o rascunho global.
- [DialogoUIManager](../../../Assets/Scripts/Historia/DialogoUIManager.cs) localiza UI inativa e mantém a primeira fala aberta. `FecharParaDialogo` coordena o fechamento do menu/terminal.
- [LocalizadorDeTerminal](../../../Assets/Scripts/Scripting/LocalizadorDeTerminal.cs) inclui inativos e prefere a UI persistente válida. Referências a objetos locais podem morrer nas transições.
- Terminal e diálogo registram a própria abertura; Status/Bag/Droid dependem do menu; Loja depende de NpcInterativo. Alterações no contador exigem considerar essa diferença.
- O Canvas compartilhado teve referência alterada para 1280×720, Match 0.5. O usuário rejeitou o tamanho resultante dos menus/terminal. Isso não é uma configuração visual aprovada. A correção e seu aceite estão no [plano da CasaLip](../../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente).

Antes de mudar Canvas, Scaler ou gestores persistentes, pesquise os consumidores em cenas/prefabs. Compare o painel afetado nas resoluções do aceite; confirme que os outros consumidores não regrediram. Teste por API não comprova teclas físicas nem aparência final.

Pesquisa inicial: `rg -n 'CanvasScaler|m_ReferenceResolution|MenuMundoManager' Assets/Scenes Assets/Prefabs Assets/Scripts/Menus`.

## Terminal ilustrado

O [método para interfaces sobre imagens](../../guias/interfaces-sobre-imagens.md) documenta as decisões do terminal e o procedimento reutilizável para outras interfaces, com ficha de escopo, arte, controles e validação. Os parâmetros abaixo pertencem ao terminal, não são valores a copiar automaticamente.

- `Assets/Prefabs/Canvas.prefab`, ramo `PainelTerminal`, usa **`Assets/UI/TerminalIlustrado/terminal-foco-horizontal.png`**, perfil 1536×1024. Arte sem letras, só monitor, sem base/pés; títulos/legendas são TMP reais. A imagem anterior `terminal-fundo.png` foi preservada; o recorte quadrado não teve aceite e foi substituído por esta composição após o relato de ilegibilidade.
- [ApresentacaoTerminalIlustrado](../../../Assets/Scripts/Menus/ApresentacaoTerminalIlustrado.cs) usa `referencia` para ajustar o painel preservando proporção e compensando escala herdada. Perfil vigente: 1536×1024, cortes zero, margem 0.98. `FundoFoco`, filho exclusivo do terminal, cobre o Canvas com fundo opaco enquanto aberto; fecha junto com o painel. Não mudar Canvas/Scaler global nem depender do zoom da Game View.
- Fontes nominais virtuais: código/números 40, saída 34, objetivo/pontos 32; objetivo longo possui scrollbar visível, saída rolável preservada. A legibilidade deve ser inspecionada também em janela pequena (referência 844×475), não apenas em captura 1920×1080. Os cortes antigos 0.05/0.05 continuam disponíveis para perfil antigo, não são o ajuste vigente.
- Campo real de código sobre área esquerda; objetivo/ajuda rolável no papel; saída real rolável à direita; pontos do Droid ao lado de Apollo. Componentes e referências de TerminalUIManager reutilizados; fundos/legendas duplicadas dos controles Unity ficam transparentes/ocultos. Botões Limpar/Executar/Fechar ocupam os botões desenhados.
- Numeração acompanha código; luzes mostram estado de execução/erro; checkbox de depuração ativa linhas técnicas adicionais no log, sem alterar Lua/pontos/progresso. Não confundir essa opção com a habilitação das dicas de Apollo.
- Contrato atende CasaLip/City/Game e o Canvas persistente. Overrides antigos de apresentação do terminal nessas cenas foram retirados de forma delimitada; não repor escalas locais 2/4 sobre o painel ajustado pelo componente.
- Montagem única: [MontagemTerminalIlustrado](../../../Assets/Editor/MontagemTerminalIlustrado.cs), que recusa executar de novo se o componente já existe. Verificação isolada: [ValidacaoTerminalIlustrado](../../../Assets/Editor/ValidacaoTerminalIlustrado.cs). Estado/aceite, evidências e backups no [plano do terminal](../../planos-implementacao/2026-10-22-terminal-ilustrado.md). Não executar montagem novamente para testar.
- Migração para foco horizontal: [RefocoTerminalIlustrado](../../../Assets/Editor/RefocoTerminalIlustrado.cs), aplicação única já realizada, com guarda contra repetição. Não executá-la outra vez para obter prévia.

## Loja ilustrada da City

- Loja local em `Assets/Scenes/City.unity`, ramo `Canvas/Loja`; não pertence ao prefab Canvas compartilhado. Apresentação em `ApresentacaoLojaIlustrada`, referência virtual 1536×1024 e encaixe proporcional local, com fundo de foco exclusivo. Canvas/Scaler e os outros painéis não foram redimensionados.
- `LojaUIManager` seleciona produtos antes da operação; `botaoAcao` confirma uma unidade. Produtos e descrição têm rolagem; saldo, preço e quantidade na mochila são dados reais. Assets próprios em `Assets/UI/LojaIlustrada`; seis ícones representam categorias, com destaque para núcleo de energia. Originais em sprites/Atualizados/novos preservados.
- Avisos de compra, venda e saldo insuficiente vêm de `CatalogoDeDialogos.AvisoDaLoja` e usam `DialogoUIManager.Mostrar`. Loja aguarda se outro diálogo estiver aberto; oculta seu painel somente quando o aviso é aceito e retorna ao terminar, sem chamar o callback externo de fechamento. NpcInterativo continua responsável pelo contador de abertura da loja.
- Economia anterior preservada, inclusive uso de `item.Preco` na venda; a diferença antiga em relação a `PrecoDeVendaReal` não foi corrigida nesta alteração visual. A montagem não cria produtos nem modifica inventário/preços do catálogo.
- `MontagemLojaIlustrada.Aplicar` é migração única, já aplicada; não repetir para testar. `ValidacaoLojaIlustrada.Executar` produz fixtures/renders isolados sem salvar progresso. Aceite, testes, backups e limitações no [plano da loja](../../planos-implementacao/2026-10-23-loja-ilustrada.md).

## Diretriz de mensagens ao jogador — definida em 22/10/2026

**Determinação do usuário:** mensagens de sucesso, confirmação de salvamento/carregamento, falhas de operação, dicas e explicações de bloqueio (casa, portão da cidade e outras passagens) devem usar a **caixa de diálogo existente**, com a apresentação aprovada. Não criar texto avulso, toast, nova janela ou reativar a etiqueta “desativado”. Isso vale desde a primeira tentativa; ausência de retorno não é funcionamento aceitável.

- Conteúdo narrativo/orientação de Apollo usa o falante e miniatura correspondentes; aviso de sistema usa apresentação de narração/sem personagem, sem atribuir ficcionalmente uma operação de arquivo ao NPC. Textos fixos devem ter origem identificada no catálogo; condições e dados dinâmicos ficam no controlador da ação.
- Sucesso só pode ser anunciado depois de a operação terminar com sucesso. Ausência de save, erro de leitura/gravação e bloqueio por requisito devem gerar explicação compreensível no mesmo painel, sem depender de Console/Debug.Log para informar o jogador.
- Diálogo já aberto não pode ser sobrescrito nem perder callback. A futura integração deve preservar a mensagem pendente ou tratar a indisponibilidade explicitamente, coordenar fechamento/retorno do menu/terminal e impedir reabertura no mesmo input. Não presumir que `Mostrar` foi aceito: verificar seu retorno.
- Aviso de bloqueio deve explicar o requisito realmente ausente, continuar disponível nas novas tentativas e não alterar flags nem liberar passagem por ter sido lido. Conclusão de puzzle deve separar efeito persistente, primeira conversa e eventual feedback de repetição; a política de repetição não justifica ausência da primeira fala.
- Esta diretriz trata mensagens e orientação ao jogador, sem redesenhar HUD, rótulos de controles ou editor de código. Saída técnica de depuração não substitui a mensagem no diálogo. Dicas atualmente escritas no log do terminal também precisarão ser revisadas conforme esta diretriz; não foram migradas nesta rodada.
- Preservar moldura, escala local, nomes/retratos e Canvas/Scaler dos demais painéis. Confirmar mensagem visível por input real desde a primeira tentativa e nas tentativas posteriores pertinentes; chamada por API/compilação não comprova isso.

**Estado:** diretriz registrada, implementação ainda divergente. O [diagnóstico de avisos/carregamento](../../planos-implementacao/2026-10-22-diagnostico-avisos-e-carregamento.md) concentra causas, evidências, lacunas e próxima etapa; não interpretar esta seção como correção aplicada.
