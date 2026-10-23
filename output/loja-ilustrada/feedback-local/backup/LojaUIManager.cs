using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DroidsCode.DroidCore;

/// <summary>
/// NOVO (Cidade, Fatia Loja — 13/09/2026). Painel de Loja com duas abas:
/// Comprar (lista TODOS os itens do catálogo — CatalogoDeItens.ItensDaLoja
/// — e gasta Gold do jogador) e Vender (lista o inventário do jogador,
/// mesmo padrão de BagUIManager.AtualizarLista, e credita Gold).
///
/// Segue exatamente o mesmo contrato estrutural de BagUIManager: painel
/// raiz, AoFechar (callback pra MenuMundoManager/NpcInterativo
/// esconder/reexibir o que estava por baixo), texto de mensagem, e o mesmo
/// cuidado de LayoutRebuilder.ForceRebuildLayoutImmediate ativando o painel
/// ANTES de popular a lista (ver comentário de correção em BagUIManager —
/// o mesmo bug de painel "ligado mas invisível" se aplica aqui).
///
/// Decisão de preço (13/09/2026, sem alinhamento extra): todo item vale o
/// mesmo hoje (DefinicaoDeItem.Preco = PrecoPadrao). Venda usa o MESMO
/// valor de Preco (sem "spread" comprador/vendedor) — balanceamento real
/// fica para quando houver playtesting, mesma ressalva já registrada para
/// TabelaDeCombate/TabelaDeCustos no checklist.
/// </summary>
public class LojaUIManager : MonoBehaviour
{
    [Header("Painel raiz")]
    public GameObject painel;

    [Header("Abas")]
    public Button botaoAbaComprar;
    public Button botaoAbaVender;

    [Header("Lista de itens (reaproveitada pelas duas abas)")]
    public Transform painelListaDeItens;
    public Button prefabBotaoItem;
    public Button botaoVoltar;

    [Header("Feedback")]
    public TextMeshProUGUI textoMensagem;
    public TextMeshProUGUI textoGold;

    [Header("Apresentação ilustrada (local à loja)")]
    public ApresentacaoLojaIlustrada apresentacao;
    public Button botaoAcao;
    DefinicaoDeItem selecionado;
    bool inicializado;
    List<FalaDeDialogo> avisoPendente;
    public DefinicaoDeItem ItemSelecionado => selecionado;
    public bool Vendendo => _abaAtual == Aba.Vender;

    public System.Action AoFechar;

    private enum Aba { Comprar, Vender }
    private Aba _abaAtual = Aba.Comprar;

    void Start()
    {
        Inicializar();
    }

    void Inicializar()
    {
        if (inicializado) return;
        inicializado = true;
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);
        if (botaoAbaComprar != null) botaoAbaComprar.onClick.AddListener(() => MudarAba(Aba.Comprar));
        if (botaoAbaVender != null) botaoAbaVender.onClick.AddListener(() => MudarAba(Aba.Vender));
        if (botaoAcao != null) botaoAcao.onClick.AddListener(ConfirmarSelecao);
        if (painel != null) painel.SetActive(false);
    }

    // Mesma correção documentada em BagUIManager.Mostrar: ativar o painel
    // ANTES de popular a lista, senão o Content Size Fitter trava em 0x0
    // porque o Unity só recalcula layout em objetos ativos.
    public void Mostrar()
    {
        Inicializar();
        selecionado = null;
        if (painel != null) painel.SetActive(true);
        _abaAtual = Aba.Comprar;
        AtualizarListaComRebuild();
    }

    public void Fechar()
    {
        avisoPendente = null;
        if (painel != null) painel.SetActive(false);
        AoFechar?.Invoke();
    }

    void MudarAba(Aba aba)
    {
        if (avisoPendente != null) return;
        selecionado = null;
        _abaAtual = aba;
        AtualizarListaComRebuild();
        var scroll = painelListaDeItens.GetComponentInParent<ScrollRect>();
        if (scroll != null) scroll.verticalNormalizedPosition = 1;
    }

    void AtualizarLista()
    {
        if (painelListaDeItens == null || prefabBotaoItem == null)
        {
            Debug.LogWarning("LojaUIManager: painelListaDeItens/prefabBotaoItem não configurados no Inspector.");
            return;
        }

        for (int i = painelListaDeItens.childCount - 1; i >= 0; i--)
        {
            var filho = painelListaDeItens.GetChild(i).gameObject;
            filho.SetActive(false);
            if (Application.isPlaying) Destroy(filho); else DestroyImmediate(filho);
        }

        var gerenciador = GerenciadorDeEstado.Instancia;

        if (textoGold != null)
        {
            textoGold.text = $"Gold: {gerenciador.Gold}";
        }

        if (_abaAtual == Aba.Comprar)
        {
            PopularComprar(gerenciador);
        }
        else
        {
            PopularVender(gerenciador);
        }
    }

    // "A loja vai ter todos os itens" — sem filtro por tipo, ver
    // CatalogoDeItens.ItensDaLoja.
    void PopularComprar(GerenciadorDeEstado gerenciador)
    {
        var itens = new List<DefinicaoDeItem>(CatalogoDeItens.ItensDaLoja);

        if (itens.Count == 0)
        {
            if (apresentacao != null) apresentacao.MostrarVazio("A loja está sem itens no momento.");
            return;
        }

        foreach (DefinicaoDeItem item in itens)
        {
            Button botao = Instantiate(prefabBotaoItem, painelListaDeItens);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = $"{item.Nome} — {item.Preco} Gold";

            DefinicaoDeItem itemCapturado = item;
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => Selecionar(itemCapturado));
            if (apresentacao != null) apresentacao.PrepararProduto(botao, item, item.Preco, gerenciador.ObterQuantidadeDeItem(item.Id), false);
        }
    }

    // Mesmo filtro de "tem estoque > 0" que BagUIManager.AtualizarLista já
    // usa — aqui não filtramos por UsavelForaDeBatalha (é venda, não uso).
    void PopularVender(GerenciadorDeEstado gerenciador)
    {
        var itensComEstoque = new List<DefinicaoDeItem>();
        foreach (var kv in gerenciador.Inventario)
        {
            if (kv.Value <= 0) continue;
            var def = CatalogoDeItens.Obter(kv.Key);
            if (def == null) continue;
            itensComEstoque.Add(def);
        }

        if (itensComEstoque.Count == 0)
        {
            if (apresentacao != null) apresentacao.MostrarVazio("Você não tem itens para vender.");
            return;
        }

        foreach (DefinicaoDeItem item in itensComEstoque)
        {
            int quantidade = gerenciador.ObterQuantidadeDeItem(item.Id);

            Button botao = Instantiate(prefabBotaoItem, painelListaDeItens);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = $"{item.Nome} x{quantidade} — {item.Preco} Gold cada";

            DefinicaoDeItem itemCapturado = item;
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => Selecionar(itemCapturado));
            if (apresentacao != null) apresentacao.PrepararProduto(botao, item, item.Preco, quantidade, true);
        }
    }

    void Comprar(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (!gerenciador.TentarGastarGold(item.Preco))
        {
            MostrarMensagem("sem_gold", item);
            return;
        }

        gerenciador.AdicionarItem(item.Id, 1);
        MostrarMensagem("compra", item);
        AtualizarListaComRebuild();
    }

    void Vender(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            MostrarMensagem("sem_item", item);
            return;
        }

        gerenciador.AdicionarGold(item.Preco);
        MostrarMensagem("venda", item);
        AtualizarListaComRebuild();
    }

    void AtualizarListaComRebuild()
    {
        AtualizarLista();
        if (selecionado != null && Vendendo && GerenciadorDeEstado.Instancia.ObterQuantidadeDeItem(selecionado.Id) <= 0)
            selecionado = null;
        if (botaoAcao != null) botaoAcao.interactable = selecionado != null && avisoPendente == null;
        if (apresentacao != null) apresentacao.MostrarSelecao(selecionado, Vendendo);
        if (painelListaDeItens != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeItens.GetComponent<RectTransform>());
        if (painel != null)
        {
            var painelRect = painel.GetComponent<RectTransform>();
            if (painelRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(painelRect);
        }
    }

    void Selecionar(DefinicaoDeItem item)
    {
        if (avisoPendente != null) return;
        selecionado = item;
        if (botaoAcao != null) botaoAcao.interactable = true;
        if (apresentacao != null) apresentacao.MostrarSelecao(item, Vendendo);
    }

    public void ConfirmarSelecao()
    {
        if (selecionado == null || avisoPendente != null) return;
        if (Vendendo) Vender(selecionado); else Comprar(selecionado);
    }

    void MostrarMensagem(string motivo, DefinicaoDeItem item)
    {
        avisoPendente = CatalogoDeDialogos.AvisoDaLoja(motivo, item.Nome, item.Preco);
        if (botaoAcao != null) botaoAcao.interactable = false;
        TentarMostrarAviso();
    }

    void Update() { if (avisoPendente != null) TentarMostrarAviso(); }

    void TentarMostrarAviso()
    {
        var dialogo = DialogoUIManager.Instancia;
        if (dialogo == null || dialogo.EstaAberto) return;
        // Manter a loja aberta se Mostrar recusar; nunca perder o aviso nem o callback existente.
        bool aceito = dialogo.Mostrar(avisoPendente, () => {
            if (painel == null) return;
            painel.SetActive(true);
            AtualizarListaComRebuild();
        });
        if (!aceito) return;
        avisoPendente = null;
        if (painel != null) painel.SetActive(false);
    }
}
