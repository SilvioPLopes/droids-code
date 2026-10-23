using System.Collections.Generic;
using DroidsCode.DroidCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Montagem local da Bag sobre a arte; regras e saves pertencem ao controlador.</summary>
public sealed class ApresentacaoBagIlustrada : MonoBehaviour
{
    const int PorPagina = 12;
    BagUIManager bag;
    TMP_FontAsset fonte;
    RectTransform foco;
    readonly List<DefinicaoDeItem> itens = new List<DefinicaoDeItem>();
    readonly Button[] slots = new Button[PorPagina];
    readonly Image[] icones = new Image[PorPagina];
    readonly TextMeshProUGUI[] quantidades = new TextMeshProUGUI[PorPagina];
    TextMeshProUGUI nome, descricao, rotuloAcao, paginaTexto;
    Image previa;
    Button acao, anterior, proxima;
    ScrollRect rolagemDescricao;
    int pagina;
    string selecionado;
    public TextMeshProUGUI Mensagem { get; private set; }
    public string ItemSelecionado => selecionado;
    public int Pagina => pagina;

    public static ApresentacaoBagIlustrada Criar(BagUIManager dono, Sprite arte)
    {
        // Não altera o asset dos controles antigos nem componentes de outros painéis.
        var raiz = dono.painel.transform;
        var fonteAntiga = dono.botaoVoltar.GetComponentInChildren<TextMeshProUGUI>(true);
        var fonte = fonteAntiga != null ? fonteAntiga.font : TMP_Settings.defaultFontAsset;
        foreach (Transform filho in raiz) filho.gameObject.SetActive(false);
        foreach (var componente in raiz.GetComponents<Behaviour>())
            if (componente is LayoutGroup || componente is ContentSizeFitter || componente is ScrollRect ||
                componente is Mask || componente is RectMask2D || componente is Image) componente.enabled = false;
        var go = new GameObject("BagIlustrada", typeof(RectTransform), typeof(ApresentacaoBagIlustrada));
        go.transform.SetParent(raiz, false);
        var a = go.GetComponent<ApresentacaoBagIlustrada>();
        a.bag = dono; a.fonte = fonte; a.Montar(arte); a.AtualizarEscala();
        return a;
    }

    static RectTransform Caixa(Transform pai, string nome, float x, float y, float w, float h)
    {
        var r = new GameObject(nome, typeof(RectTransform)).GetComponent<RectTransform>();
        r.SetParent(pai, false); r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h);
        return r;
    }
    Image Imagem(Transform pai, string nome, float x, float y, float w, float h, Sprite sprite)
    {
        var i = Caixa(pai, nome, x, y, w, h).gameObject.AddComponent<Image>();
        i.sprite = sprite; i.raycastTarget = false; i.preserveAspect = sprite != null;
        return i;
    }
    TextMeshProUGUI Texto(Transform pai, string nome, float x, float y, float w, float h, string valor, float tamanho)
    {
        var t = Caixa(pai, nome, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>();
        t.font = fonte; t.text = valor; t.fontSize = tamanho; t.enableAutoSizing = false;
        t.color = new Color(.96f, .91f, .76f); t.alignment = TextAlignmentOptions.Center;
        t.textWrappingMode = TextWrappingModes.Normal; t.raycastTarget = false;
        return t;
    }
    Button Botao(string nome, float x, float y, float w, float h, string valor)
    {
        var i = Imagem(transform, nome, x, y, w, h, null);
        i.raycastTarget = true; i.canvasRenderer.cullTransparentMesh = false;
        var b = i.gameObject.AddComponent<Button>(); b.targetGraphic = i;
        var c = b.colors; c.normalColor = Color.clear;
        c.highlightedColor = new Color(1, .8f, .45f, .22f);
        c.selectedColor = new Color(1, .8f, .45f, .12f);
        c.pressedColor = new Color(1, .8f, .45f, .35f); c.disabledColor = Color.clear; b.colors = c;
        Texto(b.transform, "Rotulo", 0, 0, w, h, valor, 40);
        return b;
    }
    void Montar(Sprite arte)
    {
        foco = Imagem(transform, "FundoFoco", 0, 0, 1536, 1024, null).rectTransform;
        foco.GetComponent<Image>().color = new Color(.035f, .04f, .035f, .97f);
        foco.GetComponent<Image>().raycastTarget = true;
        Imagem(transform, "ArteAprovada", 0, 0, 1536, 1024, arte);
        Texto(transform, "Titulo", 350, 37, 440, 62, "BAG", 54);
        anterior = Botao("PaginaAnterior", 850, 92, 60, 60, "‹");
        proxima = Botao("ProximaPagina", 978, 92, 60, 60, "›");
        paginaTexto = Texto(transform, "Pagina", 904, 92, 80, 60, "", 32);
        anterior.onClick.AddListener(() => MudarPagina(-1)); proxima.onClick.AddListener(() => MudarPagina(1));
        for (int n = 0; n < PorPagina; n++)
        {
            int indice = n;
            slots[n] = Botao("Slot" + n, 99 + n % 4 * 230, 195 + n / 4 * 215, 185, 167, "");
            icones[n] = Imagem(slots[n].transform, "Icone", 22, 4, 140, 128, null);
            quantidades[n] = Texto(slots[n].transform, "Quantidade", 5, 128, 175, 36, "", 32);
            quantidades[n].alignment = TextAlignmentOptions.Right;
            slots[n].onClick.AddListener(() => Selecionar(indice));
        }
        previa = Imagem(transform, "Previa", 1110, 140, 205, 185, null);
        nome = Texto(transform, "Nome", 1080, 362, 375, 44, "", 34);
        var viewport = Caixa(transform, "LeituraDescricao", 1074, 449, 380, 277);
        viewport.gameObject.AddComponent<RectMask2D>();
        rolagemDescricao = viewport.gameObject.AddComponent<ScrollRect>();
        rolagemDescricao.horizontal = false; rolagemDescricao.scrollSensitivity = 35;
        rolagemDescricao.movementType = ScrollRect.MovementType.Clamped;
        var captura = viewport.gameObject.AddComponent<Image>(); captura.color = new Color(0, 0, 0, .01f);
        descricao = Texto(viewport, "Descricao", 0, 0, 380, 277, "", 34);
        rolagemDescricao.viewport = viewport; rolagemDescricao.content = descricao.rectTransform;
        var trilho = Imagem(transform, "RolagemDescricao", 1457, 449, 12, 277, null);
        trilho.color = new Color(.12f, .11f, .08f, .9f); trilho.raycastTarget = true;
        var barra = trilho.gameObject.AddComponent<Scrollbar>();
        var pega = Imagem(trilho.transform, "Pega", 0, 0, 12, 60, null);
        pega.color = new Color(.64f, .48f, .24f); pega.raycastTarget = true;
        barra.handleRect = pega.rectTransform; barra.targetGraphic = pega;
        barra.direction = Scrollbar.Direction.BottomToTop;
        rolagemDescricao.verticalScrollbar = barra;
        rolagemDescricao.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        descricao.alignment = TextAlignmentOptions.TopLeft;
        acao = Botao("Acao", 1075, 779, 380, 52, "USAR");
        rotuloAcao = acao.GetComponentInChildren<TextMeshProUGUI>();
        acao.onClick.AddListener(() => { if (selecionado != null) bag.ConfirmarItemDaBag(selecionado); });
        Mensagem = Texto(transform, "Mensagem", 173, 889, 978, 75, "", 32);
        Botao("Voltar", 1210, 897, 190, 60, "Voltar").onClick.AddListener(bag.Fechar);
    }

    public void Atualizar()
    {
        itens.Clear();
        foreach (var kv in GerenciadorDeEstado.Instancia.Inventario)
        {
            var item = CatalogoDeItens.Obter(kv.Key);
            if (kv.Value > 0 && item != null && item.UsavelForaDeBatalha) itens.Add(item);
        }
        pagina = Mathf.Clamp(pagina, 0, Mathf.Max(0, (itens.Count - 1) / PorPagina));
        if (!itens.Exists(i => i.Id == selecionado)) selecionado = null;
        Desenhar();
        if (itens.Count == 0) Mensagem.text = "Sua bag está vazia.";
    }
    public void MudarPagina(int deslocamento)
    {
        pagina = Mathf.Clamp(pagina + deslocamento, 0, Mathf.Max(0, (itens.Count - 1) / PorPagina));
        selecionado = null; Desenhar();
    }
    public void Selecionar(int slot)
    {
        int indice = pagina * PorPagina + slot;
        if (slot < 0 || slot >= PorPagina || indice >= itens.Count) return;
        selecionado = itens[indice].Id; Desenhar();
    }
    void Desenhar()
    {
        for (int n = 0; n < PorPagina; n++)
        {
            int indice = pagina * PorPagina + n; bool existe = indice < itens.Count;
            slots[n].gameObject.SetActive(existe);
            if (!existe) continue;
            var item = itens[indice];
            icones[n].sprite = CatalogoVisualDeItens.Obter(item.Id);
            icones[n].enabled = icones[n].sprite != null;
            quantidades[n].text = "×" + GerenciadorDeEstado.Instancia.ObterQuantidadeDeItem(item.Id);
            var c = slots[n].colors; c.normalColor = item.Id == selecionado ? new Color(1, .78f, .3f, .19f) : Color.clear; slots[n].colors = c;
        }
        var atual = itens.Find(i => i.Id == selecionado);
        nome.text = atual == null ? "Selecione um item" : atual.Nome;
        descricao.text = atual == null ? "Escolha um item para consultar a descrição. Use o botão abaixo para confirmar a ação." : atual.Descricao;
        descricao.rectTransform.sizeDelta = new Vector2(380, Mathf.Max(277, descricao.GetPreferredValues(descricao.text, 380, Mathf.Infinity).y));
        rolagemDescricao.verticalNormalizedPosition = 1;
        previa.sprite = atual == null ? null : CatalogoVisualDeItens.Obter(atual.Id);
        previa.enabled = previa.sprite != null;
        bool equipamento = atual != null && atual.Tipo == TipoDeItem.Equipavel;
        acao.interactable = atual != null && !equipamento;
        rotuloAcao.text = equipamento ? "NO TERMINAL" : "USAR";
        if (equipamento)
        {
            descricao.text += $"\n\nEquipe pelo terminal:\ndroid.equipar(\"{atual.Id}\")";
            descricao.rectTransform.sizeDelta = new Vector2(380, Mathf.Max(277, descricao.GetPreferredValues(descricao.text, 380, Mathf.Infinity).y));
        }
        anterior.interactable = pagina > 0; proxima.interactable = (pagina + 1) * PorPagina < itens.Count;
        paginaTexto.text = $"{pagina + 1}/{Mathf.Max(1, (itens.Count + PorPagina - 1) / PorPagina)}";
    }
    void LateUpdate() => AtualizarEscala();
    public void AtualizarEscala()
    {
        var canvas = GetComponentInParent<Canvas>(); if (canvas == null || foco == null) return;
        var raiz = (RectTransform)canvas.rootCanvas.transform; var herdada = transform.parent.lossyScale;
        if (Mathf.Abs(herdada.x) < .00001f || Mathf.Abs(herdada.y) < .00001f) return;
        float fator = Mathf.Min(raiz.rect.width * Mathf.Abs(raiz.lossyScale.x) / 1536f,
            raiz.rect.height * Mathf.Abs(raiz.lossyScale.y) / 1024f) * .98f;
        if (fator <= 0) return;
        var r = (RectTransform)transform; r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f;
        r.sizeDelta = new Vector2(1536, 1024); r.localScale = new Vector3(fator / Mathf.Abs(herdada.x), fator / Mathf.Abs(herdada.y), 1);
        r.position = raiz.TransformPoint(raiz.rect.center);
        foco.anchorMin = foco.anchorMax = foco.pivot = Vector2.one * .5f; foco.anchoredPosition = Vector2.zero;
        foco.sizeDelta = new Vector2(raiz.rect.width * Mathf.Abs(raiz.lossyScale.x) / fator,
            raiz.rect.height * Mathf.Abs(raiz.lossyScale.y) / fator);
    }
}
