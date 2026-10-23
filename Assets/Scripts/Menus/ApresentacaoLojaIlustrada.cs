using DroidsCode.DroidCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Layout local e representação de produtos. Não altera preços nem inventário.</summary>
[ExecuteAlways, DisallowMultipleComponent]
public sealed class ApresentacaoLojaIlustrada : MonoBehaviour
{
    public RectTransform fundoFoco;
    public TextMeshProUGUI nomeItem, descricao, preco, estoque, rotuloAcao, vazio;
    public Image iconeItem;
    [HideInInspector] public Sprite[] icones; // Legado serializado; não define mais a aparência por categoria.
    public LojaUIManager loja;
    bool ajustando;
    static readonly Color Claro = new Color(.94f,.87f,.68f);
    static readonly Color Selecionado = new Color(.34f,.28f,.15f,.98f);

    void OnEnable() => AtualizarEscala();
    void LateUpdate() => AtualizarEscala();
    void OnRectTransformDimensionsChange() => AtualizarEscala();

    public void AtualizarEscala()
    {
        if (ajustando) return;
        var area = transform as RectTransform;
        var canvas = GetComponentInParent<Canvas>();
        if (area == null || canvas == null || transform.parent == null) return;
        var raiz = (RectTransform)canvas.rootCanvas.transform;
        var herdada = transform.parent.lossyScale;
        if (Mathf.Abs(herdada.x)<.00001f || Mathf.Abs(herdada.y)<.00001f) return;
        ajustando = true;
        try
        {
            float fator = Mathf.Min(raiz.rect.width*Mathf.Abs(raiz.lossyScale.x)/1536f,
                raiz.rect.height*Mathf.Abs(raiz.lossyScale.y)/1024f)*.98f;
            area.anchorMin=area.anchorMax=area.pivot=Vector2.one*.5f;
            area.sizeDelta=new Vector2(1536,1024);
            area.localScale=new Vector3(fator/Mathf.Abs(herdada.x),fator/Mathf.Abs(herdada.y),1);
            area.position=raiz.TransformPoint(raiz.rect.center);
            if (fundoFoco != null)
            {
                fundoFoco.sizeDelta=new Vector2(raiz.rect.width*Mathf.Abs(raiz.lossyScale.x)/Mathf.Abs(area.lossyScale.x),
                    raiz.rect.height*Mathf.Abs(raiz.lossyScale.y)/Mathf.Abs(area.lossyScale.y));
                fundoFoco.position=area.position;
            }
        }
        finally { ajustando=false; }
    }

    Sprite Icone(DefinicaoDeItem item)
    {
        return CatalogoVisualDeItens.Obter(item.Id);
    }

    public void PrepararProduto(Button botao, DefinicaoDeItem item, int valor, int quantidade, bool vendendo)
    {
        botao.GetComponent<Image>().color=new Color(.13f,.14f,.13f,.96f);
        var nome=botao.transform.Find("Nome").GetComponent<TextMeshProUGUI>();
        nome.text=item.Nome;
        botao.transform.Find("Preco").GetComponent<TextMeshProUGUI>().text=vendendo?$"{valor} Gold  ·  ×{quantidade}":$"{valor} Gold";
        botao.transform.Find("Icone").GetComponent<Image>().sprite=Icone(item);
        botao.name="Produto_"+item.Id;
        if (loja.ItemSelecionado != null && loja.ItemSelecionado.Id==item.Id) botao.GetComponent<Image>().color=Selecionado;
    }

    public void MostrarVazio(string texto) { if(vazio!=null) vazio.text=texto; }

    /// <summary>Reutiliza o rodapé existente; confirmação local não abre conversa.</summary>
    public TextMeshProUGUI PrepararMensagem()
    {
        var alvo = transform.Find("AjudaSelecao");
        if (alvo == null) return null;
        var texto = alvo.GetComponent<TextMeshProUGUI>();
        if (texto == null) return null;
        texto.fontSize=32;texto.enableAutoSizing=false;
        texto.textWrappingMode=TextWrappingModes.Normal;
        texto.rectTransform.anchoredPosition=new Vector2(85,-905);
        texto.rectTransform.sizeDelta=new Vector2(730,76);
        return texto;
    }

    public void MostrarSelecao(DefinicaoDeItem item, bool vendendo)
    {
        bool tem=item!=null;
        nomeItem.text=tem?item.Nome:"Selecione um item";
        descricao.text=tem?item.Descricao:"Escolha um produto para ver os detalhes antes de confirmar a operação.";
        preco.text=tem?$"{item.Preco} Gold":"";
        estoque.text=tem?$"Na mochila: {GerenciadorDeEstado.Instancia.ObterQuantidadeDeItem(item.Id)}":"";
        iconeItem.gameObject.SetActive(tem);
        if(tem) iconeItem.sprite=Icone(item);
        rotuloAcao.text=vendendo?"VENDER 1":"COMPRAR 1";
        if(vazio!=null) vazio.gameObject.SetActive(loja.painelListaDeItens.GetComponentsInChildren<Button>().Length==0);
        foreach(var b in loja.painelListaDeItens.GetComponentsInChildren<Button>())
            b.GetComponent<Image>().color=tem&&b.name=="Produto_"+item.Id?Selecionado:new Color(.13f,.14f,.13f,.96f);
        loja.botaoAbaComprar.GetComponent<Image>().color=vendendo?new Color(.18f,.18f,.16f):Selecionado;
        loja.botaoAbaVender.GetComponent<Image>().color=vendendo?Selecionado:new Color(.18f,.18f,.16f);
    }
}
