# Contexto da interface

- [MenuMundoManager](../../../Assets/Scripts/Menus/MenuMundoManager.cs) mantém o Canvas entre cenas e controla a origem de painéis. Terminal aberto no mundo não deve revelar o menu ao fechar.
- [TerminalUIManager](../../../Assets/Scripts/Menus/TerminalUIManager.cs) inicializa uma vez; abertura repetida preserva Runner e contador. `DefinirContextoPuzzle`/`LimparContextoPuzzle` controlam ajuda e código inicial sem apagar o rascunho global.
- [DialogoUIManager](../../../Assets/Scripts/Historia/DialogoUIManager.cs) localiza UI inativa e mantém a primeira fala aberta. `FecharParaDialogo` coordena o fechamento do menu/terminal.
- [LocalizadorDeTerminal](../../../Assets/Scripts/Scripting/LocalizadorDeTerminal.cs) inclui inativos e prefere a UI persistente válida. Referências a objetos locais podem morrer nas transições.
- Terminal e diálogo registram a própria abertura; Status/Bag/Droid dependem do menu; Loja depende de NpcInterativo. Alterações no contador exigem considerar essa diferença.
- O Canvas compartilhado teve referência alterada para 1280×720, Match 0.5. O usuário rejeitou o tamanho resultante dos menus/terminal. Isso não é uma configuração visual aprovada. A correção e seu aceite estão no [plano da CasaLip](../../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente).

Antes de mudar Canvas, Scaler ou gestores persistentes, pesquise os consumidores em cenas/prefabs. Compare o painel afetado nas resoluções do aceite; confirme que os outros consumidores não regrediram. Teste por API não comprova teclas físicas nem aparência final.

Pesquisa inicial: `rg -n 'CanvasScaler|m_ReferenceResolution|MenuMundoManager' Assets/Scenes Assets/Prefabs Assets/Scripts/Menus`.

## Diretriz de mensagens ao jogador — definida em 22/10/2026

**Determinação do usuário:** mensagens de sucesso, confirmação de salvamento/carregamento, falhas de operação, dicas e explicações de bloqueio (casa, portão da cidade e outras passagens) devem usar a **caixa de diálogo existente**, com a apresentação aprovada. Não criar texto avulso, toast, nova janela ou reativar a etiqueta “desativado”. Isso vale desde a primeira tentativa; ausência de retorno não é funcionamento aceitável.

- Conteúdo narrativo/orientação de Apollo usa o falante e miniatura correspondentes; aviso de sistema usa apresentação de narração/sem personagem, sem atribuir ficcionalmente uma operação de arquivo ao NPC. Textos fixos devem ter origem identificada no catálogo; condições e dados dinâmicos ficam no controlador da ação.
- Sucesso só pode ser anunciado depois de a operação terminar com sucesso. Ausência de save, erro de leitura/gravação e bloqueio por requisito devem gerar explicação compreensível no mesmo painel, sem depender de Console/Debug.Log para informar o jogador.
- Diálogo já aberto não pode ser sobrescrito nem perder callback. A futura integração deve preservar a mensagem pendente ou tratar a indisponibilidade explicitamente, coordenar fechamento/retorno do menu/terminal e impedir reabertura no mesmo input. Não presumir que `Mostrar` foi aceito: verificar seu retorno.
- Aviso de bloqueio deve explicar o requisito realmente ausente, continuar disponível nas novas tentativas e não alterar flags nem liberar passagem por ter sido lido. Conclusão de puzzle deve separar efeito persistente, primeira conversa e eventual feedback de repetição; a política de repetição não justifica ausência da primeira fala.
- Esta diretriz trata mensagens e orientação ao jogador, sem redesenhar HUD, rótulos de controles ou editor de código. Saída técnica de depuração não substitui a mensagem no diálogo. Dicas atualmente escritas no log do terminal também precisarão ser revisadas conforme esta diretriz; não foram migradas nesta rodada.
- Preservar moldura, escala local, nomes/retratos e Canvas/Scaler dos demais painéis. Confirmar mensagem visível por input real desde a primeira tentativa e nas tentativas posteriores pertinentes; chamada por API/compilação não comprova isso.

**Estado:** diretriz registrada, implementação ainda divergente. O [diagnóstico de avisos/carregamento](../../planos-implementacao/2026-10-22-diagnostico-avisos-e-carregamento.md) concentra causas, evidências, lacunas e próxima etapa; não interpretar esta seção como correção aplicada.
