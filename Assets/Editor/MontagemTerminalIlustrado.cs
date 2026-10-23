using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MontagemTerminalIlustrado
{
    const string Prefab = "Assets/Prefabs/Canvas.prefab";
    const string Fundo = "Assets/UI/TerminalIlustrado/terminal-fundo.png";
    static readonly Color Verde = new Color(0.55f, 0.93f, 0.62f);

    static void Caixa(RectTransform r, float x, float y, float w, float h)
    {
        r.anchorMin = r.anchorMax = new Vector2(0, 1);
        r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(x, -y);
        r.sizeDelta = new Vector2(w, h);
        r.localScale = Vector3.one;
        r.localRotation = Quaternion.identity;
    }

    static RectTransform Criar(string nome, Transform pai, params Type[] tipos)
    {
        var go = new GameObject(nome, new[] { typeof(RectTransform) }.Concat(tipos).ToArray());
        go.layer = 5;
        go.transform.SetParent(pai, false);
        return (RectTransform)go.transform;
    }

    static void Esticar(RectTransform r)
    {
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.pivot = new Vector2(0.5f, 0.5f);
        r.offsetMin = r.offsetMax = Vector2.zero; r.localScale = Vector3.one;
    }

    static TextMeshProUGUI Texto(string nome, Transform pai, TMP_FontAsset fonte, float tamanho, Color cor)
    {
        var r = Criar(nome, pai, typeof(TextMeshProUGUI));
        var t = r.GetComponent<TextMeshProUGUI>();
        Formatar(t, fonte, tamanho, cor);
        return t;
    }

    static void Formatar(TMP_Text t, TMP_FontAsset fonte, float tamanho, Color cor)
    {
        t.font = fonte; t.fontSize = tamanho; t.enableAutoSizing = false;
        t.color = cor; t.fontStyle = FontStyles.Normal; t.alignment = TextAlignmentOptions.TopLeft;
        t.margin = Vector4.zero; t.raycastTarget = false; t.richText = false;
        t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Overflow;
        t.lineSpacing = 0;
    }

    static void Botao(Button b, float x, float y, float w, float h)
    {
        Caixa((RectTransform)b.transform, x, y, w, h);
        foreach(var t in b.GetComponentsInChildren<TextMeshProUGUI>(true)) t.gameObject.SetActive(false);
        var i = b.GetComponent<Image>(); i.sprite = null; i.color = Color.clear; i.raycastTarget = true;
        b.targetGraphic = i; b.transition = Selectable.Transition.ColorTint;
        var c = b.colors; c.normalColor = Color.clear;
        c.highlightedColor = new Color(1f, 0.85f, 0.35f, 0.12f);
        c.selectedColor = new Color(1f, 0.85f, 0.35f, 0.08f);
        c.pressedColor = new Color(1f, 0.85f, 0.35f, 0.3f);
        c.disabledColor = new Color(0, 0, 0, 0.2f); c.fadeDuration = 0.08f;
        b.colors = c; b.interactable = true;
        var nav = b.navigation; nav.mode = Navigation.Mode.None; b.navigation = nav;
    }

    static ScrollRect Rolagem(string nome, Transform pai, TextMeshProUGUI texto, float x, float y, float w, float h)
    {
        var r = Criar(nome, pai, typeof(Image), typeof(ScrollRect)); Caixa(r, x, y, w, h);
        r.GetComponent<Image>().color = Color.clear;
        var viewport = Criar("Viewport", r, typeof(RectMask2D)); Esticar(viewport);
        texto.transform.SetParent(viewport, false);
        var tr = texto.rectTransform;
        tr.anchorMin = new Vector2(0, 1); tr.anchorMax = Vector2.one; tr.pivot = new Vector2(0, 1);
        tr.anchoredPosition = Vector2.zero; tr.sizeDelta = Vector2.zero; tr.localScale = Vector3.one;
        var fitter = texto.GetComponent<ContentSizeFitter>() ?? texto.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var s = r.GetComponent<ScrollRect>(); s.viewport = viewport; s.content = tr;
        s.horizontal = false; s.vertical = true; s.movementType = ScrollRect.MovementType.Clamped;
        s.scrollSensitivity = 30; return s;
    }

    static Image Luz(Transform pai, string nome, float x)
    {
        var r = Criar(nome, pai, typeof(Image)); Caixa(r, x - 11, 166, 22, 22);
        var i = r.GetComponent<Image>(); i.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        i.raycastTarget = false; return i;
    }

    public static string Montar()
    {
        if(EditorApplication.isPlaying) throw new InvalidOperationException("Montagem persistente exige fora de Play.");
        var imp = (TextureImporter)AssetImporter.GetAtPath(Fundo);
        imp.textureType = TextureImporterType.Sprite; imp.spriteImportMode = SpriteImportMode.Single;
        imp.mipmapEnabled = false; imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.maxTextureSize = 2048; imp.filterMode = FilterMode.Bilinear; imp.SaveAndReimport();
        var raiz = PrefabUtility.LoadPrefabContents(Prefab);
        try
        {
            var t = raiz.GetComponentInChildren<TerminalUIManager>(true);
            var painel = (RectTransform)t.painelTerminal.transform;
            if(painel.GetComponent<ApresentacaoTerminalIlustrado>() != null)
                throw new InvalidOperationException("Montagem já existe; inspecione antes de repetir.");
            painel.sizeDelta = Vector2.one * 1254; painel.localScale = Vector3.one;
            var fonte = t.campoDeCodigo.textComponent.font;
            var fundo = Criar("ArteTerminal", painel, typeof(Image)); Esticar(fundo); fundo.SetAsFirstSibling();
            var img = fundo.GetComponent<Image>(); img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Fundo);
            img.preserveAspect = true; img.raycastTarget = true;
            // Mesmos componentes e IDs do campo; só apresentação e geometria.
            var campo = t.campoDeCodigo;
            Caixa(campo.GetComponent<RectTransform>(), 190, 240, 565, 660);
            campo.GetComponent<Image>().color = Color.clear; campo.transition = Selectable.Transition.None;
            campo.lineType = TMP_InputField.LineType.MultiLineNewline;
            campo.customCaretColor = true; campo.caretColor = Verde; campo.selectionColor = new Color(0.3f, 0.8f, 0.4f, 0.3f);
            campo.caretWidth = 2; campo.richText = false;
            Esticar(campo.textViewport); Esticar(campo.textComponent.rectTransform);
            Formatar(campo.textComponent, fonte, 24, Verde);
            campo.textComponent.textWrappingMode = TextWrappingModes.NoWrap;
            if(campo.placeholder is TextMeshProUGUI ph) {
                Esticar(ph.rectTransform); Formatar(ph, fonte, 24, new Color(0.4f, 0.65f, 0.45f, 0.6f));
                ph.text = "Escreva seu código aqui...";
            }
            var gutter = Criar("MargemNumeros", painel, typeof(RectMask2D)); Caixa(gutter, 118, 240, 49, 660);
            var numeros = Texto("NumerosDeLinha", gutter, fonte, 24, new Color(0.32f, 0.65f, 0.39f));
            Esticar(numeros.rectTransform); numeros.alignment = TextAlignmentOptions.TopRight;
            numeros.textWrappingMode = TextWrappingModes.NoWrap; numeros.text = "1";
            Botao(t.botaoLimpar, 113, 946, 211, 91);
            Botao(t.botaoExecutar, 350, 946, 240, 91);
            Botao(t.botaoFechar, 620, 946, 199, 91);
            Formatar(t.textoAjuda, fonte, 20, new Color(0.14f, 0.11f, 0.045f));
            t.textoAjuda.gameObject.SetActive(true);
            Rolagem("ObjetivoRolavel", painel, t.textoAjuda, 816, 296, 305, 112);
            // Reutiliza saída/ScrollRect, eliminando fundos e máscara opaca antigos.
            var scroll = t.scrollRectSaida; Caixa((RectTransform)scroll.transform, 813, 511, 330, 166);
            scroll.GetComponent<Image>().color = Color.clear;
            var view = scroll.viewport; Esticar(view);
            var mask = view.GetComponent<Mask>(); if(mask != null) UnityEngine.Object.DestroyImmediate(mask);
            var vi = view.GetComponent<Image>(); if(vi != null) vi.enabled = false;
            if(view.GetComponent<RectMask2D>() == null) view.gameObject.AddComponent<RectMask2D>();
            t.textoSaida.transform.SetParent(view, false);
            var oldContent = scroll.content; scroll.content = t.textoSaida.rectTransform;
            if(oldContent != null && oldContent.gameObject != t.textoSaida.gameObject) oldContent.gameObject.SetActive(false);
            var rt = t.textoSaida.rectTransform;
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = Vector2.zero; rt.sizeDelta = Vector2.zero; rt.localScale = Vector3.one;
            Formatar(t.textoSaida, fonte, 20, Verde);
            var fit = t.textoSaida.GetComponent<ContentSizeFitter>() ?? t.textoSaida.gameObject.AddComponent<ContentSizeFitter>();
            fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; fit.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            scroll.horizontal = false; scroll.vertical = true; scroll.movementType = ScrollRect.MovementType.Clamped; scroll.scrollSensitivity = 30;
            scroll.horizontalScrollbar = null;
            foreach(var bar in scroll.GetComponentsInChildren<Scrollbar>(true)) bar.gameObject.SetActive(bar == scroll.verticalScrollbar);
            var vertical = scroll.verticalScrollbar;
            if(vertical != null) {
                Caixa((RectTransform)vertical.transform, 341, -31, 14, 190);
                vertical.GetComponent<Image>().sprite = null;
                vertical.GetComponent<Image>().color = new Color(0.003f, 0.035f, 0.012f);
                var handle = vertical.handleRect; Esticar((RectTransform)handle.parent); Esticar(handle);
                handle.GetComponent<Image>().sprite = null; handle.GetComponent<Image>().color = new Color(0.17f, 0.48f, 0.24f);
                vertical.transition = Selectable.Transition.None;
                scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            }
            t.textoPontos.gameObject.SetActive(true);
            Caixa(t.textoPontos.rectTransform, 981, 789, 172, 105);
            Formatar(t.textoPontos, fonte, 23, Verde);
            var antigo = painel.Find("TextoApollo"); if(antigo != null) antigo.gameObject.SetActive(false);
            var toggleRect = Criar("Depuracao", painel, typeof(Image), typeof(Toggle)); Caixa(toggleRect, 1016, 974, 33, 33);
            var toggle = toggleRect.GetComponent<Toggle>();
            toggleRect.GetComponent<Image>().color = Color.clear; toggle.targetGraphic = toggleRect.GetComponent<Image>();
            var marca = Texto("Marca", toggleRect, fonte, 30, Verde); Esticar(marca.rectTransform);
            marca.alignment = TextAlignmentOptions.Center; marca.text = "×";
            toggle.graphic = marca; toggle.isOn = false;
            var estado = Texto("EstadoDepuracao", painel, fonte, 22, Verde); Caixa(estado.rectTransform, 1065, 980, 108, 30);
            var a = painel.gameObject.AddComponent<ApresentacaoTerminalIlustrado>();
            a.terminal = t; a.numerosDeLinha = numeros; a.depuracao = toggle; a.estadoDaDepuracao = estado;
            a.luzPronto = Luz(painel,"LuzPronto",741); a.luzExecutando = Luz(painel,"LuzExecutando",892); a.luzErro = Luz(painel,"LuzErro",1078);
            a.AtualizarDados(); a.AtualizarEscala();
            PrefabUtility.SaveAsPrefabAsset(raiz, Prefab);
            return "PainelTerminal montado; referências de campo/botões/saída/ajuda/pontos preservadas.";
        }
        finally { PrefabUtility.UnloadPrefabContents(raiz); }
    }
}
