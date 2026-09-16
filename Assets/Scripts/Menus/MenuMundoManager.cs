using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// CORRECAO (13/09/2026 — exclusividade de painéis Status/Terminal/Menu):
///   - Ao abrir Status ou Terminal a partir daqui, o painel do Menu do
///     Mundo agora é escondido (SetActive(false)) — não fica mais visível
///     por baixo. Ele volta a aparecer quando Status/Terminal fecham,
///     via callback AoFechar (ver TelaDeStatusManager/TerminalUIManager).
///   - O contador de GerenciadorDeEstado (RegistrarMenuAberto/Fechado)
///     continua intocado — cada painel já cuida do seu próprio registro
///     (Terminal já chamava isso; Status não chama, e não precisa).
///   - Bug encontrado durante a correção: como Update() escutava Escape o
///     tempo todo, apertar Esc com o Terminal aberto reabria o painel do
///     Menu do Mundo por baixo dele (Alternar() via painelMenu.activeSelf
///     == false) e incrementava o contador de novo, "vazando" um open.
///     Corrigido com a flag _outroPainelAberto, que o próprio
///     MenuMundoManager controla e o Update() respeita.
///
/// NOVO (Painel do Droid, sessão atual): botão "Droid" deixou de ser
/// placeholder — segue exatamente o mesmo molde de AoClicarStatus/AoClicarBag.
/// </summary>
public class MenuMundoManager : MonoBehaviour
{
    [Header("Painel raiz do menu")]
    public GameObject painelMenu;

    [Header("Tecla que abre/fecha o menu")]
    public KeyCode teclaDeAbertura = KeyCode.Escape;

    // NOVO (15/09/2026 — Ato 1). Desde que este Canvas ganhou
    // DontDestroyOnLoad, ele viaja pra TODAS as cenas, inclusive a de
    // batalha, onde o BattleManager também escuta Escape (pra fechar as
    // listas de Ataque/Item). Um único ESC era consumido pelos dois. Aqui
    // o menu simplesmente não abre nas cenas listadas.
    [Header("Cenas onde o menu NÃO deve abrir")]
    [Tooltip("Nomes exatos de cena (ex: \"Battle\", \"MainMenu\"). O Menu do Mundo ignora a tecla nessas cenas.")]
    public string[] cenasBloqueadas = new string[] { "Battle", "MainMenu" };

    [Header("Botões")]
    public Button botaoStatus;
    public Button botaoTerminal;
    public Button botaoBag;
    public Button botaoDroid;
    public Button botaoOpcoes;
    public Button botaoSalvar;
    public Button botaoCarregar;
    public Button botaoVoltar;

    [Header("Tela de Status (arraste a instância do Prefab PainelStatus)")]
    public TelaDeStatusManager telaDeStatus;

    [Header("Terminal de código (arraste a instância do Prefab PainelTerminal)")]
    public TerminalUIManager terminal;

    // Estagio 2 (13/09/2026): Bag real, substitui o aviso "não implementado".
    [Header("Bag (arraste a instância do Prefab PainelBag)")]
    public BagUIManager bag;

    // NOVO (Painel do Droid, sessão atual): mesmo padrão de telaDeStatus/bag.
    [Header("Painel do Droid (arraste a instância do Prefab PainelDroid)")]
    public TelaDeDroidManager telaDeDroid;

    [Header("Texto de aviso pros botões ainda não implementados / feedback de save")]
    public TextMeshProUGUI textoAviso;

    private ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    // NOVO (13/09/2026): true enquanto Status ou Terminal estiverem abertos
    // por cima do Menu do Mundo. Update() ignora Escape nesse período, pra
    // não reabrir o painelMenu por baixo do painel que está em foco.
    private bool _outroPainelAberto = false;

    // NOVO: guarda qual painel fechar quando ESC for apertado com um painel
    // aberto (Status/Terminal/Bag/Droid). Setado no mesmo AoClicarX que liga
    // _outroPainelAberto = true; zerado em AoFecharOutroPainel.
    private System.Action _fecharPainelAtual;

    // NOVO: mesmo padrão de GerenciadorDeEstado — só uma instância do Menu
    // do Mundo sobrevive entre cenas (DontDestroyOnLoad). Sem isso, o menu
    // (e todos os painéis Status/Terminal/Bag/Droid) só existia na cena
    // onde o Canvas foi colocado (Game) — abrir ESC na Cidade não achava
    // nada, porque o objeto inteiro nunca chegava lá.
    //
    // IMPORTANTE (trabalho de Editor que isso exige):
    // 1. O GameObject raiz do Canvas que contém o MenuMundoManager (e todos
    //    os painéis filhos) deve ser o alvo do DontDestroyOnLoad — aqui uso
    //    transform.root.gameObject, que é esse Canvas raiz.
    // 2. Remova qualquer Canvas/Menu duplicado que você já tenha colocado
    //    manualmente na cena City — só deve existir UM, criado na primeira
    //    cena carregada (normalmente Game), que persiste daí em diante.
    void Awake()
    {
        // CORRECAO (Unity 6): FindObjectsOfType estava obsoleto (CS0618).
        // FindObjectsByType exige escolher ordenacao -- aqui a ordem nao
        // importa (so contamos quantas instancias existem), entao
        // FindObjectsSortMode.None e o substituto direto e mais rapido.
        var instanciasExistentes = FindObjectsByType<MenuMundoManager>(FindObjectsSortMode.None);
        if (instanciasExistentes.Length > 1)
        {
            // Já existe um Menu do Mundo persistente de uma cena anterior —
            // este aqui é duplicado (ex: você entrou de novo na cena onde
            // o objeto foi originalmente criado). Destrói o novo, mantém o
            // que já estava sobrevivendo.
            Destroy(transform.root.gameObject);
            return;
        }

        DontDestroyOnLoad(transform.root.gameObject);
    }

    void Start()
    {
        if (painelMenu != null) painelMenu.SetActive(false);

        if (botaoStatus != null) botaoStatus.onClick.AddListener(AoClicarStatus);
        if (botaoTerminal != null) botaoTerminal.onClick.AddListener(AoClicarTerminal);
        if (botaoBag != null) botaoBag.onClick.AddListener(AoClicarBag);
        if (botaoDroid != null) botaoDroid.onClick.AddListener(AoClicarDroid);
        if (botaoOpcoes != null) botaoOpcoes.onClick.AddListener(() => MostrarAviso("Opções"));
        if (botaoSalvar != null) botaoSalvar.onClick.AddListener(AoClicarSalvar);
        if (botaoCarregar != null) botaoCarregar.onClick.AddListener(AoClicarCarregar);
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);

        // NOVO (13/09/2026): liga os callbacks de fechamento do Status e do
        // Terminal — é assim que o Menu do Mundo sabe quando reaparecer.
        if (telaDeStatus != null) telaDeStatus.AoFechar = AoFecharOutroPainel;
        if (terminal != null) terminal.AoFechar = AoFecharOutroPainel;
        if (bag != null) bag.AoFechar = AoFecharOutroPainel;
        if (telaDeDroid != null) telaDeDroid.AoFechar = AoFecharOutroPainel;
    }

    bool MenuBloqueadoNestaCena()
    {
        if (cenasBloqueadas == null || cenasBloqueadas.Length == 0) return false;

        string cenaAtual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        foreach (string cena in cenasBloqueadas)
        {
            if (!string.IsNullOrEmpty(cena) && cena == cenaAtual) return true;
        }
        return false;
    }

    void Update()
    {
        // NOVO (15/09/2026): não abre o menu de pausa em cenas bloqueadas
        // (Battle/MainMenu). Se um painel já estiver aberto, a checagem fica
        // abaixo, pra que ESC ainda consiga FECHAR o que estiver na tela.
        if (!_outroPainelAberto && MenuBloqueadoNestaCena()) return;

        // CORRIGIDO: antes, Escape era ignorado enquanto Status/Terminal/
        // Bag/Droid estivessem abertos (só dava pra fechar clicando em
        // Voltar). Agora ESC fecha o painel que estiver aberto, chamando o
        // mesmo Fechar() que o botão Voltar de cada um já chama.
        if (_outroPainelAberto)
        {
            if (Input.GetKeyDown(teclaDeAbertura))
            {
                _fecharPainelAtual?.Invoke();
            }
            return;
        }

        if (Input.GetKeyDown(teclaDeAbertura))
        {
            Alternar();
        }
    }

    void Alternar()
    {
        if (painelMenu == null) return;

        bool vaiAbrir = !painelMenu.activeSelf;
        painelMenu.SetActive(vaiAbrir);

        if (vaiAbrir)
        {
            GerenciadorDeEstado.Instancia.RegistrarMenuAberto();
            if (textoAviso != null) textoAviso.text = "";
        }
        else
        {
            GerenciadorDeEstado.Instancia.RegistrarMenuFechado();
        }
    }

    void Fechar()
    {
        if (painelMenu != null && painelMenu.activeSelf)
        {
            painelMenu.SetActive(false);
            GerenciadorDeEstado.Instancia.RegistrarMenuFechado();
        }
    }

    // NOVO (13/09/2026): substitui a lambda antiga do botaoStatus — agora
    // esconde o painel do Menu do Mundo antes de mostrar o Status.
    void AoClicarStatus()
    {
        if (telaDeStatus == null) return;

        _outroPainelAberto = true;
        _fecharPainelAtual = telaDeStatus.Fechar;
        if (painelMenu != null) painelMenu.SetActive(false);
        telaDeStatus.Mostrar();
    }

    // NOVO (13/09/2026): substitui o aviso "não implementado" — abre a Bag
    // real (BagUIManager), mesmo padrão de exclusividade de AoClicarStatus.
    void AoClicarBag()
    {
        if (bag != null)
        {
            _outroPainelAberto = true;
            _fecharPainelAtual = bag.Fechar;
            if (painelMenu != null) painelMenu.SetActive(false);
            bag.Mostrar();
        }
        else
        {
            MostrarAviso("Bag");
        }
    }

    // NOVO (Painel do Droid, sessão atual): substitui o aviso "não
    // implementado" (MostrarAviso("Equipamentos")) — abre o Painel do Droid
    // real (TelaDeDroidManager, só visualização), mesmo padrão de
    // exclusividade de AoClicarStatus/AoClicarBag.
    void AoClicarDroid()
    {
        if (telaDeDroid != null)
        {
            _outroPainelAberto = true;
            _fecharPainelAtual = telaDeDroid.Fechar;
            if (painelMenu != null) painelMenu.SetActive(false);
            telaDeDroid.Mostrar();
        }
        else
        {
            MostrarAviso("Droid");
        }
    }

    void AoClicarTerminal()
    {
        if (terminal != null)
        {
            // NOVO (13/09/2026): esconde o Menu do Mundo antes de abrir o Terminal.
            _outroPainelAberto = true;
            _fecharPainelAtual = terminal.Fechar;
            if (painelMenu != null) painelMenu.SetActive(false);
            terminal.Abrir();
        }
        else
        {
            MostrarAviso("Terminal");
        }
    }

    // NOVO (13/09/2026): chamado pelos callbacks AoFechar do Status e do
    // Terminal — reexibe o Menu do Mundo e libera o Update() de novo.
    void AoFecharOutroPainel()
    {
        _outroPainelAberto = false;
        _fecharPainelAtual = null;
        if (painelMenu != null) painelMenu.SetActive(true);
    }

    void AoClicarSalvar()
    {
        _salvamento.Salvar();
        if (textoAviso != null) textoAviso.text = "Jogo salvo.";
    }

    void AoClicarCarregar()
    {
        if (!_salvamento.ExisteSave())
        {
            if (textoAviso != null) textoAviso.text = "Nenhum save encontrado.";
            return;
        }

        _salvamento.Carregar();
        if (textoAviso != null) textoAviso.text = "Jogo carregado.";
    }

    void MostrarAviso(string nomeDaTela)
    {
        if (textoAviso == null) return;
        textoAviso.text = $"{nomeDaTela} ainda não foi implementado.";
    }
}
