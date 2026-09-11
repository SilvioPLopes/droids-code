using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menu fora de batalha, estilo Pokémon: Status / Terminal / Bag / Droid /
/// Opções / Voltar. Status e Terminal estão funcionais agora — os outros
/// mostram um aviso "ainda não implementado" (menu completo, funcionalidade
/// parcial, combinado assim de propósito).
///
/// Abre e fecha com a tecla em teclaDeAbertura (padrão: Esc). Troque no
/// Inspector se quiser outra tecla.
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
    public Button botaoVoltar;

    [Header("Tela de Status (arraste a instância do Prefab PainelStatus)")]
    public TelaDeStatusManager telaDeStatus;

    [Header("Terminal de código (arraste a instância do Prefab PainelTerminal)")]
    public TerminalUIManager terminal;

    [Header("Texto de aviso pros botões ainda não implementados")]
    public TextMeshProUGUI textoAviso;

    void Start()
    {
        if (painelMenu != null) painelMenu.SetActive(false);

        if (botaoStatus != null) botaoStatus.onClick.AddListener(() => telaDeStatus.Mostrar());
        if (botaoTerminal != null) botaoTerminal.onClick.AddListener(AoClicarTerminal);
        if (botaoBag != null) botaoBag.onClick.AddListener(() => MostrarAviso("Bag"));
        if (botaoDroid != null) botaoDroid.onClick.AddListener(() => MostrarAviso("Equipamentos"));
        if (botaoOpcoes != null) botaoOpcoes.onClick.AddListener(() => MostrarAviso("Opções"));
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

        if (vaiAbrir && textoAviso != null)
        {
            textoAviso.text = "";
        }
    }

    void Fechar()
    {
        if (painelMenu != null) painelMenu.SetActive(false);
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

    void MostrarAviso(string nomeDaTela)
    {
        if (textoAviso == null) return;
        textoAviso.text = $"{nomeDaTela} ainda não foi implementado.";
    }
}