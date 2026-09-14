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

    public System.Action AoFechar;

    private enum Aba { Comprar, Vender }
    private Aba _abaAtual = Aba.Comprar;

    void Start()
    {
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);
        if (botaoAbaComprar != null) botaoAbaComprar.onClick.AddListener(() => MudarAba(Aba.Comprar));
        if (botaoAbaVender != null) botaoAbaVender.onClick.AddListener(() => MudarAba(Aba.Vender));
        if (painel != null) painel.SetActive(false);
    }

    // Mesma correção documentada em BagUIManager.Mostrar: ativar o painel
    // ANTES de popular a lista, senão o Content Size Fitter trava em 0x0
    // porque o Unity só recalcula layout em objetos ativos.
    public void Mostrar()
    {
        if (painel != null) painel.SetActive(true);
        _abaAtual = Aba.Comprar;
        AtualizarListaComRebuild();
    }

    public void Fechar()
    {
        if (painel != null) painel.SetActive(false);
        AoFechar?.Invoke();
    }

    void MudarAba(Aba aba)
    {
        _abaAtual = aba;
        AtualizarListaComRebuild();
    }

    void AtualizarLista()
    {
        if (painelListaDeItens == null || prefabBotaoItem == null)
        {
            Debug.LogWarning("LojaUIManager: painelListaDeItens/prefabBotaoItem não configurados no Inspector.");
            return;
        }

        foreach (Transform filho in painelListaDeItens)
            Destroy(filho.gameObject);

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
            MostrarMensagem("A loja está sem itens no momento.");
            return;
        }

        foreach (DefinicaoDeItem item in itens)
        {
            Button botao = Instantiate(prefabBotaoItem, painelListaDeItens);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = $"{item.Nome} — {item.Preco} Gold";

            DefinicaoDeItem itemCapturado = item;
            botao.onClick.AddListener(() => Comprar(itemCapturado));
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
            MostrarMensagem("Você não tem itens para vender.");
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
            botao.onClick.AddListener(() => Vender(itemCapturado));
        }
    }

    void Comprar(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (!gerenciador.TentarGastarGold(item.Preco))
        {
            MostrarMensagem($"Gold insuficiente para comprar {item.Nome}.");
            return;
        }

        gerenciador.AdicionarItem(item.Id, 1);
        MostrarMensagem($"Comprou {item.Nome} por {item.Preco} Gold.");
        AtualizarListaComRebuild();
    }

    void Vender(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            MostrarMensagem($"Você não tem mais {item.Nome} para vender.");
            return;
        }

        gerenciador.AdicionarGold(item.Preco);
        MostrarMensagem($"Vendeu {item.Nome} por {item.Preco} Gold.");
        AtualizarListaComRebuild();
    }

    void AtualizarListaComRebuild()
    {
        AtualizarLista();
        if (painelListaDeItens != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeItens.GetComponent<RectTransform>());
        if (painel != null)
        {
            var painelRect = painel.GetComponent<RectTransform>();
            if (painelRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(painelRect);
        }
    }

    void MostrarMensagem(string mensagem)
    {
        if (textoMensagem != null) textoMensagem.text = mensagem;
        Debug.Log($"[Loja] {mensagem}");
    }
}
