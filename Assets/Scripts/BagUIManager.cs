using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DroidsCode.DroidCore;

public class BagUIManager : MonoBehaviour
{
    [Header("Painel raiz")]
    public GameObject painel;

    [Header("Lista de itens")]
    public Transform painelListaDeItens;
    public Button prefabBotaoItem;
    public Button botaoVoltar;

    [Header("Feedback")]
    public TextMeshProUGUI textoMensagem;

    public System.Action AoFechar;

    private readonly ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    void Start()
    {
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);
        if (painel != null) painel.SetActive(false);
    }

    // CORRECAO (13/09/2026 — Bag nao aparecia ao clicar): a ordem antiga era
    // AtualizarLista() (que instancia botoes DENTRO de um painel ainda
    // INATIVO) e só depois painel.SetActive(true). Instanciar filhos num
    // GameObject inativo, com VerticalLayoutGroup + ContentSizeFitter no
    // pai, faz o Unity nao recalcular a altura/largura do painel (ele so
    // recalcula layout em objetos ATIVOS) -- o painel ativa com Content
    // Size Fitter travado em 0x0, entao fica "ligado" mas invisivel.
    // Fix: ativar o painel PRIMEIRO, so DEPOIS popular a lista, e forcar um
    // rebuild de layout no frame seguinte.
    public void Mostrar()
    {
        if (painel != null) painel.SetActive(true);
        AtualizarLista();

        if (painelListaDeItens != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeItens.GetComponent<RectTransform>());
        }
        if (painel != null)
        {
            var painelRect = painel.GetComponent<RectTransform>();
            if (painelRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(painelRect);
        }
    }

    public void Fechar()
    {
        if (painel != null) painel.SetActive(false);
        AoFechar?.Invoke();
    }

    void AtualizarLista()
    {
        if (painelListaDeItens == null || prefabBotaoItem == null)
        {
            Debug.LogWarning("BagUIManager: painelListaDeItens/prefabBotaoItem não configurados no Inspector.");
            return;
        }

        foreach (Transform filho in painelListaDeItens)
            Destroy(filho.gameObject);

        var gerenciador = GerenciadorDeEstado.Instancia;
        var itensComEstoque = new List<DefinicaoDeItem>();

        foreach (var kv in gerenciador.Inventario)
        {
            if (kv.Value <= 0) continue;
            var def = CatalogoDeItens.Obter(kv.Key);
            if (def == null || !def.UsavelForaDeBatalha) continue;
            itensComEstoque.Add(def);
        }

        if (itensComEstoque.Count == 0)
        {
            MostrarMensagem("Sua bag está vazia.");
        }

        foreach (DefinicaoDeItem item in itensComEstoque)
        {
            int quantidade = gerenciador.ObterQuantidadeDeItem(item.Id);

            Button botao = Instantiate(prefabBotaoItem, painelListaDeItens);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = $"{item.Nome} (+{item.CuraHp} HP) x{quantidade}";

            DefinicaoDeItem itemCapturado = item;
            botao.onClick.AddListener(() => UsarItemForaDeBatalha(itemCapturado));
        }
    }

    void UsarItemForaDeBatalha(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;
        Droid droid = gerenciador.DroidDoJogador;

        if (droid.Hp >= droid.HpMax)
        {
            MostrarMensagem($"{droid.Nome} já está com HP cheio.");
            return;
        }

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            MostrarMensagem($"Você não tem mais {item.Nome}.");
            return;
        }

        float multiplicador = 1f + (droid.BonusPercentualDeCura / 100f);
        int curaBase = Mathf.FloorToInt(item.CuraHp * multiplicador);
        int curaAplicada = Mathf.Min(curaBase, droid.HpMax - droid.Hp);
        droid.Hp = Mathf.Min(droid.HpMax, droid.Hp + curaBase);

        MostrarMensagem($"{droid.Nome} usou {item.Nome} e recuperou {curaAplicada} de HP!");
        _salvamento.Salvar();

        AtualizarLista();
        if (painelListaDeItens != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeItens.GetComponent<RectTransform>());
    }

    void MostrarMensagem(string mensagem)
    {
        if (textoMensagem != null) textoMensagem.text = mensagem;
        Debug.Log($"[Bag] {mensagem}");
    }
}
