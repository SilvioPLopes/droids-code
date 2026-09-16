using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Diario de missoes. Acessivel pelo Menu do Mundo (botao "Missoes").
/// Mostra as ativas em cima, as concluidas embaixo, e o detalhe da
/// selecionada com objetivos marcados, recompensas e -- quando ja concluida
/// -- a curiosidade liberada.
///
/// Segue EXATAMENTE o mesmo contrato dos paineis que ja existem
/// (TelaDeStatusManager/BagUIManager/LojaUIManager): painel raiz desativado
/// em Start, callback AoFechar, botao Voltar, e
/// LayoutRebuilder.ForceRebuildLayoutImmediate com o container ja ativo
/// antes de popular -- a mesma correcao de "painel ligado mas invisivel"
/// documentada no BagUIManager.
/// </summary>
public class QuestUIManager : MonoBehaviour
{
    [Header("Painel raiz")]
    public GameObject painel;

    [Header("Lista de missões")]
    [Tooltip("Container com Vertical Layout Group + Content Size Fitter.")]
    public Transform painelListaDeQuests;
    [Tooltip("Prefab de botão com TextMeshProUGUI filho. Pode reusar o da Bag/Loja.")]
    public Button prefabBotaoQuest;
    public Button botaoVoltar;

    [Header("Detalhe da missão selecionada")]
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoResumo;
    public TextMeshProUGUI textoObjetivos;
    public TextMeshProUGUI textoRecompensas;
    [Tooltip("Curiosidade/lore, só aparece depois de concluir a missão.")]
    public TextMeshProUGUI textoCuriosidade;

    public System.Action AoFechar;

    private string _selecionada;

    void Start()
    {
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);
        if (painel != null) painel.SetActive(false);
    }

    void OnEnable()
    {
        GerenciadorDeQuests.AoMudarQuests += AtualizarSeAberto;
    }

    void OnDisable()
    {
        GerenciadorDeQuests.AoMudarQuests -= AtualizarSeAberto;
    }

    void AtualizarSeAberto()
    {
        if (painel != null && painel.activeSelf) AtualizarTudo();
    }

    public void Mostrar()
    {
        // Ativar ANTES de popular -- senão o ContentSizeFitter trava em 0x0
        // (o Unity só recalcula layout em objetos ativos). Mesma correção
        // documentada em BagUIManager.Mostrar.
        if (painel != null) painel.SetActive(true);

        GerenciadorDeQuests.AtualizarQuestsAutomaticas();
        AtualizarTudo();
    }

    public void Fechar()
    {
        if (painel != null) painel.SetActive(false);
        AoFechar?.Invoke();
    }

    void AtualizarTudo()
    {
        AtualizarLista();
        AtualizarDetalhe();

        if (painelListaDeQuests != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeQuests.GetComponent<RectTransform>());
        }
        if (painel != null)
        {
            var rect = painel.GetComponent<RectTransform>();
            if (rect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    void AtualizarLista()
    {
        if (painelListaDeQuests == null || prefabBotaoQuest == null)
        {
            Debug.LogWarning("QuestUIManager: painelListaDeQuests/prefabBotaoQuest não configurados no Inspector.");
            return;
        }

        foreach (Transform filho in painelListaDeQuests) Destroy(filho.gameObject);

        List<DefinicaoDeQuest> ativas = GerenciadorDeQuests.Ativas();
        List<DefinicaoDeQuest> concluidas = GerenciadorDeQuests.Concluidas();

        if (ativas.Count == 0 && concluidas.Count == 0)
        {
            CriarCabecalho("Nenhuma missão ainda. Converse com as pessoas.");
            return;
        }

        // Ordem: principal, guilda, secundária. O jogador perdido precisa
        // achar o fio da história no topo, não no meio da lista.
        if (ativas.Count > 0)
        {
            CriarCabecalho("— EM ANDAMENTO —");
            foreach (DefinicaoDeQuest q in Ordenar(ativas)) CriarBotao(q, ativa: true);
        }

        if (concluidas.Count > 0)
        {
            CriarCabecalho("— CONCLUÍDAS —");
            foreach (DefinicaoDeQuest q in Ordenar(concluidas)) CriarBotao(q, ativa: false);
        }

        if (string.IsNullOrEmpty(_selecionada) && ativas.Count > 0)
        {
            _selecionada = Ordenar(ativas)[0].Id;
        }
    }

    List<DefinicaoDeQuest> Ordenar(List<DefinicaoDeQuest> lista)
    {
        var ordenada = new List<DefinicaoDeQuest>();
        foreach (DefinicaoDeQuest q in lista) { if (q.EhPrincipal) ordenada.Add(q); }
        foreach (DefinicaoDeQuest q in lista) { if (q.EhDeGuilda) ordenada.Add(q); }
        foreach (DefinicaoDeQuest q in lista) { if (!q.EhPrincipal && !q.EhDeGuilda) ordenada.Add(q); }
        return ordenada;
    }

    void CriarCabecalho(string texto)
    {
        Button botao = Instantiate(prefabBotaoQuest, painelListaDeQuests);
        botao.gameObject.SetActive(true);
        botao.interactable = false;

        var tmp = botao.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = texto;
    }

    void CriarBotao(DefinicaoDeQuest q, bool ativa)
    {
        Button botao = Instantiate(prefabBotaoQuest, painelListaDeQuests);
        botao.gameObject.SetActive(true);

        string marcador = q.EhPrincipal ? "◆ " : (q.EhDeGuilda ? "⚙ " : "• ");
        string estado = ativa
            ? (GerenciadorDeQuests.PodeEntregar(q.Id) ? "  (pronta para entregar)" : "")
            : "  (concluída)";

        var tmp = botao.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = $"{marcador}{q.Titulo}{estado}";

        string idCapturado = q.Id;
        botao.onClick.AddListener(() =>
        {
            _selecionada = idCapturado;
            AtualizarDetalhe();
        });
    }

    void AtualizarDetalhe()
    {
        DefinicaoDeQuest q = CatalogoDeQuests.Obter(_selecionada);

        if (q == null)
        {
            DefinirTexto(textoTitulo, "");
            DefinirTexto(textoResumo, "Selecione uma missão.");
            DefinirTexto(textoObjetivos, "");
            DefinirTexto(textoRecompensas, "");
            DefinirTexto(textoCuriosidade, "");
            if (textoCuriosidade != null) textoCuriosidade.gameObject.SetActive(false);
            return;
        }

        DefinirTexto(textoTitulo, q.Titulo);

        string quem = CatalogoDeNpcs.NomeDe(q.NpcQueOferece);
        DefinirTexto(textoResumo, $"{q.Resumo}\n\n<i>Pedido por: {quem}</i>");

        var objetivos = new System.Text.StringBuilder();
        for (int i = 0; i < q.Objetivos.Count; i++)
        {
            objetivos.AppendLine(GerenciadorDeQuests.DescreverObjetivo(q, i));
        }
        DefinirTexto(textoObjetivos, objetivos.ToString());

        DefinirTexto(textoRecompensas, "Recompensa: " + GerenciadorDeQuests.DescreverRecompensas(q));

        // Curiosidade é recompensa de informação: só depois de concluir.
        bool concluida = GerenciadorDeQuests.EstaConcluida(q.Id);
        bool temCuriosidade = concluida && !string.IsNullOrWhiteSpace(q.Curiosidade);

        if (textoCuriosidade != null)
        {
            textoCuriosidade.gameObject.SetActive(temCuriosidade);
            if (temCuriosidade) textoCuriosidade.text = $"<i>{q.Curiosidade}</i>";
        }
    }

    static void DefinirTexto(TextMeshProUGUI campo, string valor)
    {
        if (campo != null) campo.text = valor;
    }
}
