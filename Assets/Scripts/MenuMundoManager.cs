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
/// </summary>
public class MenuMundoManager : MonoBehaviour
{
    [Header("Painel raiz do menu")]
    public GameObject painelMenu;

    [Header("Tecla que abre/fecha o menu")]
    public KeyCode teclaDeAbertura = KeyCode.Escape;

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

    [Header("Texto de aviso pros botões ainda não implementados / feedback de save")]
    public TextMeshProUGUI textoAviso;

    private ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    // NOVO (13/09/2026): true enquanto Status ou Terminal estiverem abertos
    // por cima do Menu do Mundo. Update() ignora Escape nesse período, pra
    // não reabrir o painelMenu por baixo do painel que está em foco.
    private bool _outroPainelAberto = false;

    void Start()
    {
        if (painelMenu != null) painelMenu.SetActive(false);

        if (botaoStatus != null) botaoStatus.onClick.AddListener(AoClicarStatus);
        if (botaoTerminal != null) botaoTerminal.onClick.AddListener(AoClicarTerminal);
        if (botaoBag != null) botaoBag.onClick.AddListener(AoClicarBag);
        if (botaoDroid != null) botaoDroid.onClick.AddListener(() => MostrarAviso("Equipamentos"));
        if (botaoOpcoes != null) botaoOpcoes.onClick.AddListener(() => MostrarAviso("Opções"));
        if (botaoSalvar != null) botaoSalvar.onClick.AddListener(AoClicarSalvar);
        if (botaoCarregar != null) botaoCarregar.onClick.AddListener(AoClicarCarregar);
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);

        // NOVO (13/09/2026): liga os callbacks de fechamento do Status e do
        // Terminal — é assim que o Menu do Mundo sabe quando reaparecer.
        if (telaDeStatus != null) telaDeStatus.AoFechar = AoFecharOutroPainel;
        if (terminal != null) terminal.AoFechar = AoFecharOutroPainel;
        if (bag != null) bag.AoFechar = AoFecharOutroPainel;
    }

    void Update()
    {
        // NOVO (13/09/2026): enquanto Status/Terminal estiverem abertos,
        // Escape não deve mexer no painel do Menu do Mundo.
        if (_outroPainelAberto) return;

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
            if (painelMenu != null) painelMenu.SetActive(false);
            bag.Mostrar();
        }
        else
        {
            MostrarAviso("Bag");
        }
    }

    void AoClicarTerminal()
    {
        if (terminal != null)
        {
            // NOVO (13/09/2026): esconde o Menu do Mundo antes de abrir o Terminal.
            _outroPainelAberto = true;
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
