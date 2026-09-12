using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Texto de aviso pros botões ainda não implementados / feedback de save")]
    public TextMeshProUGUI textoAviso;

    private ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    void Start()
    {
        if (painelMenu != null) painelMenu.SetActive(false);

        if (botaoStatus != null) botaoStatus.onClick.AddListener(() => telaDeStatus.Mostrar());
        if (botaoTerminal != null) botaoTerminal.onClick.AddListener(AoClicarTerminal);
        if (botaoBag != null) botaoBag.onClick.AddListener(() => MostrarAviso("Bag"));
        if (botaoDroid != null) botaoDroid.onClick.AddListener(() => MostrarAviso("Equipamentos"));
        if (botaoOpcoes != null) botaoOpcoes.onClick.AddListener(() => MostrarAviso("Opções"));
        if (botaoSalvar != null) botaoSalvar.onClick.AddListener(AoClicarSalvar);
        if (botaoCarregar != null) botaoCarregar.onClick.AddListener(AoClicarCarregar);
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);
    }

    void Update()
    {
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

    void AoClicarTerminal()
    {
        if (terminal != null)
        {
            terminal.Abrir();
        }
        else
        {
            MostrarAviso("Terminal");
        }
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
