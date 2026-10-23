using System.Text;
using DroidsCode.DroidCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Ficha visual do Apollo. Não gasta pontos, itens, energia nem grava progresso.</summary>
public sealed class ApresentacaoStatusIlustrado : MonoBehaviour
{
    static readonly TipoAtributo[] Tipos = { TipoAtributo.For, TipoAtributo.Agi, TipoAtributo.Vit, TipoAtributo.Int, TipoAtributo.Dex, TipoAtributo.Luk };
    static readonly string[] Nomes = { "FOR · Força", "AGI · Agilidade", "VIT · Vitalidade", "INT · Inteligência", "DEX · Destreza", "LUK · Sorte" };
    static readonly string[] Explicacoes = {
        "Força aumenta o ataque. O dano final também depende da técnica, da defesa do alvo e de golpes críticos.",
        "Agilidade ajuda Apollo a esquivar dos ataques. O resultado também depende da precisão do oponente.",
        "Vitalidade aumenta a defesa e contribui para a vida máxima de Apollo.",
        "Inteligência ajuda a resistir a efeitos e aumenta a cura recebida por itens.",
        "Destreza aumenta a precisão. A chance de acertar também depende da esquiva do alvo.",
        "Sorte aumenta a chance de um golpe crítico depois que o ataque acerta." };
    TelaDeStatusManager dono;
    TMP_FontAsset fonte;
    RectTransform foco;
    TextMeshProUGUI nome, nivel, vidaTexto, energiaTexto, xp, recursos, tecnicas, ajuda;
    readonly TextMeshProUGUI[] atributos = new TextMeshProUGUI[6];
    Image vida, energia;
    ScrollRect rolagemTecnicas, rolagemAjuda;
    Droid observado;
    public float FracaoVida => vida == null ? 0 : vida.fillAmount;
    public float FracaoEnergia => energia == null ? 0 : energia.fillAmount;
    public string EstadoEnergia => energiaTexto == null ? "" : energiaTexto.text;

    public static ApresentacaoStatusIlustrado Criar(TelaDeStatusManager controlador)
    {
        var raiz = controlador.painel.transform;
        var modelo = controlador.textoNome;
        foreach (Transform filho in raiz) filho.gameObject.SetActive(false);
        foreach (var b in raiz.GetComponents<Behaviour>())
            if (b is LayoutGroup || b is ContentSizeFitter || b is ScrollRect || b is Mask || b is RectMask2D || b is Image) b.enabled = false;
        var go = new GameObject("StatusIlustrado", typeof(RectTransform), typeof(ApresentacaoStatusIlustrado));
        go.transform.SetParent(raiz, false);
        var a = go.GetComponent<ApresentacaoStatusIlustrado>(); a.dono = controlador;
        a.fonte = modelo != null ? modelo.font : TMP_Settings.defaultFontAsset;
        a.Montar(); a.AtualizarEscala(); return a;
    }
    RectTransform Caixa(Transform pai, string nomeObjeto, float x, float y, float w, float h)
    {
        var r = new GameObject(nomeObjeto, typeof(RectTransform)).GetComponent<RectTransform>();
        r.SetParent(pai, false); r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
    }
    Image Imagem(Transform pai, string nomeObjeto, float x, float y, float w, float h, Sprite sprite = null)
    {
        var i = Caixa(pai, nomeObjeto, x, y, w, h).gameObject.AddComponent<Image>();
        i.sprite = sprite; i.preserveAspect = sprite != null; i.raycastTarget = false; return i;
    }
    TextMeshProUGUI Texto(Transform pai, string nomeObjeto, float x, float y, float w, float h, string texto, float tamanho, bool papel = false)
    {
        var t = Caixa(pai, nomeObjeto, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>();
        t.font = fonte; t.text = texto; t.fontSize = tamanho; t.enableAutoSizing = false;
        t.color = papel ? new Color(.18f, .14f, .09f) : new Color(.96f, .92f, .77f);
        t.textWrappingMode = TextWrappingModes.Normal; t.alignment = TextAlignmentOptions.MidlineLeft;
        t.raycastTarget = false; return t;
    }
    Button Botao(Transform pai, string nomeObjeto, float x, float y, float w, float h)
    {
        var i = Imagem(pai, nomeObjeto, x, y, w, h); i.raycastTarget = true;
        i.canvasRenderer.cullTransparentMesh = false;
        var b = i.gameObject.AddComponent<Button>(); b.targetGraphic = i;
        var c = b.colors; c.normalColor = Color.clear; c.highlightedColor = new Color(1, .8f, .4f, .15f);
        c.selectedColor = new Color(1, .8f, .4f, .12f); c.pressedColor = new Color(1, .8f, .4f, .25f); b.colors = c;
        return b;
    }
    TextMeshProUGUI TextoRolavel(string nomeObjeto, float x, float y, float w, float h, bool papel, out ScrollRect scroll)
    {
        var viewport = Caixa(transform, nomeObjeto, x, y, w, h);
        var i = viewport.gameObject.AddComponent<Image>(); i.color = new Color(0, 0, 0, .01f);
        viewport.gameObject.AddComponent<RectMask2D>();
        scroll = viewport.gameObject.AddComponent<ScrollRect>(); scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped; scroll.scrollSensitivity = 35;
        var t = Texto(viewport, "Conteudo", 0, 0, w - 22, h, "", 34, papel);
        t.alignment = TextAlignmentOptions.TopLeft; scroll.viewport = viewport; scroll.content = t.rectTransform;
        var trilho = Imagem(transform, nomeObjeto + "Barra", x + w - 13, y, 11, h);
        trilho.color = new Color(.2f, .18f, .12f, .7f); trilho.raycastTarget = true;
        var barra = trilho.gameObject.AddComponent<Scrollbar>();
        var pega = Imagem(trilho.transform, "Pega", 0, 0, 11, 60);
        pega.color = new Color(.63f, .46f, .22f); pega.raycastTarget = true;
        barra.handleRect = pega.rectTransform; barra.targetGraphic = pega; barra.direction = Scrollbar.Direction.BottomToTop;
        scroll.verticalScrollbar = barra; scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        return t;
    }
    Image Tubo(string nomeObjeto, float y, Color cor)
    {
        // Região interna do vidro; não cobre as conexões ou bordas de cobre da arte.
        var fundo = Imagem(transform, nomeObjeto + "Fundo", 546, y, 695, 31);
        fundo.color = new Color(.055f, .075f, .065f, .96f);
        var preenchimento = Imagem(transform, nomeObjeto, 546, y, 695, 31);
        preenchimento.color = cor;
        // Image.Filled precisa de sprite: textura branca nativa do Unity, sem alterar a arte.
        preenchimento.sprite = SpriteBranco(); preenchimento.type = Image.Type.Filled;
        preenchimento.fillMethod = Image.FillMethod.Horizontal; preenchimento.fillOrigin = 0;
        preenchimento.fillAmount = 0; return preenchimento;
    }
    Sprite branco;
    Sprite SpriteBranco()
    {
        if (branco == null) branco = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), Vector2.one * .5f);
        return branco;
    }
    void OnDestroy()
    {
        if (branco == null) return;
        if (Application.isPlaying) Destroy(branco); else DestroyImmediate(branco);
    }
    void Montar()
    {
        foco = Imagem(transform, "FundoFoco", 0, 0, 1536, 1024).rectTransform;
        foco.GetComponent<Image>().color = new Color(.035f, .04f, .035f, .98f); foco.GetComponent<Image>().raycastTarget = true;
        Imagem(transform, "ArteV2", 0, 0, 1536, 1024, dono.arteStatus);
        Texto(transform, "Titulo", 607, 23, 320, 58, "STATUS", 48).alignment = TextAlignmentOptions.Center;
        Imagem(transform, "RetratoApollo", 171, 105, 190, 184, dono.retratoApollo);
        nome = Texto(transform, "Nome", 526, 117, 550, 48, "", 42);
        nivel = Texto(transform, "Nivel", 1162, 110, 110, 49, "", 36);
        vida = Tubo("Vida", 203, new Color(.27f, .84f, .28f));
        energia = Tubo("Energia", 276, dono.corEnergia);
        vidaTexto = Texto(transform, "ValorVida", 552, 200, 680, 37, "", 32);
        energiaTexto = Texto(transform, "ValorEnergia", 552, 273, 680, 37, "Energia · a definir", 32);
        vidaTexto.alignment = energiaTexto.alignment = TextAlignmentOptions.Center;
        vidaTexto.outlineColor = energiaTexto.outlineColor = Color.black;
        vidaTexto.outlineWidth = energiaTexto.outlineWidth = .22f;
        for (int n = 0; n < 6; n++)
        {
            int indice = n; float y = 363 + n * 88;
            var b = Botao(transform, "Atributo" + n, 116, y, 780, 68);
            atributos[n] = Texto(b.transform, "Valor", 12, 0, 750, 68, "", 36, true);
            b.onClick.AddListener(() => Explicar(indice));
        }
        Texto(transform, "TituloTecnicas", 1053, 351, 295, 36, "TÉCNICAS", 32, true).alignment = TextAlignmentOptions.Center;
        tecnicas = TextoRolavel("ListaTecnicas", 978, 410, 431, 195, false, out rolagemTecnicas);
        Texto(transform, "TituloAjuda", 1053, 657, 295, 34, "ATRIBUTOS", 30, true).alignment = TextAlignmentOptions.Center;
        ajuda = TextoRolavel("Explicacao", 982, 711, 427, 143, true, out rolagemAjuda);
        xp = Texto(transform, "Experiencia", 125, 923, 520, 48, "", 32);
        recursos = Texto(transform, "Recursos", 721, 915, 405, 68, "", 30);
        var voltar = Botao(transform, "Voltar", 1210, 929, 203, 49);
        Texto(voltar.transform, "Rotulo", 0, 0, 203, 49, "Voltar", 40).alignment = TextAlignmentOptions.Center;
        voltar.onClick.AddListener(dono.Fechar); Explicar(0);
    }
    static void AjustarTextoRolavel(TextMeshProUGUI texto, ScrollRect scroll)
    {
        float largura = texto.rectTransform.sizeDelta.x;
        texto.rectTransform.sizeDelta = new Vector2(largura, Mathf.Max(scroll.viewport.rect.height,
            texto.GetPreferredValues(texto.text, largura, Mathf.Infinity).y));
        scroll.verticalNormalizedPosition = 1;
    }
    void Explicar(int indice)
    {
        ajuda.text = Explicacoes[indice]; AjustarTextoRolavel(ajuda, rolagemAjuda);
    }
    public void Atualizar(Droid droid)
    {
        observado = droid;
        nome.text = droid.Nome == "Heroi" ? "Apollo" : droid.Nome;
        nivel.text = "Nv. " + droid.Progressao.Nivel;
        for (int n = 0; n < 6; n++) atributos[n].text = $"{Nomes[n]}       {droid.ObterTotal(Tipos[n])}";
        var lista = new StringBuilder();
        foreach (var tecnica in droid.ObterAcoesDisponiveis())
            lista.AppendLine(tecnica == Droid.NomeAtaqueBasico
                ? $"{tecnica} · +{TabelaDeEnergia.RecuperacaoAtaqueBasico} energia"
                : $"{tecnica} · {droid.ObterCustoDeEnergia(tecnica)} energia");
        tecnicas.text = lista.Length == 0 ? "Nenhuma técnica" : lista.ToString(); AjustarTextoRolavel(tecnicas, rolagemTecnicas);
        xp.text = $"XP: {droid.Progressao.ExperienciaAtual}/{droid.Progressao.ExperienciaNecessariaProximoNivel}";
        recursos.text = $"Gold: {GerenciadorDeEstado.Instancia.Gold}\nPontos de evolução: {droid.Pontos.PontosDisponiveis}";
        AtualizarVida();
    }
    void AtualizarVida()
    {
        if (observado == null) return;
        vida.fillAmount = observado.HpMax > 0 ? Mathf.Clamp01((float)observado.Hp / observado.HpMax) : 0;
        vidaTexto.text = $"Vida: {observado.Hp}/{observado.HpMax}";
        energia.fillAmount = observado.EnergiaMaxima > 0 ? Mathf.Clamp01((float)observado.EnergiaAtual / observado.EnergiaMaxima) : 0;
        energiaTexto.text = $"Energia: {observado.EnergiaAtual}/{observado.EnergiaMaxima}";
    }
    void LateUpdate() { AtualizarEscala(); AtualizarVida(); }
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
