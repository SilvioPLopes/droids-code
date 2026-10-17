# Contexto da interface

- [MenuMundoManager](../../../Assets/Scripts/Menus/MenuMundoManager.cs) mantém o Canvas entre cenas e controla a origem de painéis. Terminal aberto no mundo não deve revelar o menu ao fechar.
- [TerminalUIManager](../../../Assets/Scripts/Menus/TerminalUIManager.cs) inicializa uma vez; abertura repetida preserva Runner e contador. `DefinirContextoPuzzle`/`LimparContextoPuzzle` controlam ajuda e código inicial sem apagar o rascunho global.
- [DialogoUIManager](../../../Assets/Scripts/Historia/DialogoUIManager.cs) localiza UI inativa e mantém a primeira fala aberta. `FecharParaDialogo` coordena o fechamento do menu/terminal.
- [LocalizadorDeTerminal](../../../Assets/Scripts/Scripting/LocalizadorDeTerminal.cs) inclui inativos e prefere a UI persistente válida. Referências a objetos locais podem morrer nas transições.
- Terminal e diálogo registram a própria abertura; Status/Bag/Droid dependem do menu; Loja depende de NpcInterativo. Alterações no contador exigem considerar essa diferença.
- O Canvas compartilhado teve referência alterada para 1280×720, Match 0.5. O usuário rejeitou o tamanho resultante dos menus/terminal. Isso não é uma configuração visual aprovada. A correção e seu aceite estão no [plano da CasaLip](../../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente).

Antes de mudar Canvas, Scaler ou gestores persistentes, pesquise os consumidores em cenas/prefabs. Compare o painel afetado nas resoluções do aceite; confirme que os outros consumidores não regrediram. Teste por API não comprova teclas físicas nem aparência final.

Pesquisa inicial: `rg -n 'CanvasScaler|m_ReferenceResolution|MenuMundoManager' Assets/Scenes Assets/Prefabs Assets/Scripts/Menus`.
