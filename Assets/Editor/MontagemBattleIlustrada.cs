using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Montagem única na Battle aberta e limpa. Não altera prefabs compartilhados.</summary>
public static class MontagemBattleIlustrada
{
    const string Pasta = "Assets/UI/BattleIlustrada/";
    static TMP_FontAsset fonte;
    static readonly Color Creme = new Color(.96f, .91f, .76f);

    public static string IntegrarBag()
    {
        var cena = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (Application.dataPath.Replace('\\', '/') != "C:/Users/silve/DroidsCode/Assets" || EditorApplication.isPlaying ||
            cena.path != "Assets/Scenes/Battle.unity" || cena.isDirty) throw new InvalidOperationException("Exige Battle limpa fora de Play.");
        var b = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
        if (b.bagDeBatalha != null) throw new InvalidOperationException("Bag já integrada; não repetir montagem.");
        var origem = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PainelBag.prefab");
        var arte = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/BagIlustrada/bag-conceito.png");
        if (origem == null || arte == null) throw new InvalidOperationException("Prefab/arte existentes ausentes.");
        var canvas = b.botaoItem.GetComponentInParent<Canvas>();
        var go = (GameObject)PrefabUtility.InstantiatePrefab(origem, canvas.transform);
        go.name = "BagBatalha"; go.SetActive(false); go.transform.SetAsLastSibling();
        var bag = go.GetComponent<BagUIManager>();
        bag.arteBag = arte; b.bagDeBatalha = bag;
        PrefabUtility.RecordPrefabInstancePropertyModifications(bag);
        EditorSceneManager.MarkSceneDirty(cena); EditorSceneManager.SaveScene(cena);
        return "Uma instância local de PainelBag configurada e vinculada à Battle; prefab compartilhado preservado.";
    }

    static RectTransform Caixa(Transform pai, string nome, float x, float y, float w, float h)
    {
        var r = new GameObject(nome, typeof(RectTransform)).GetComponent<RectTransform>();
        r.gameObject.layer = 5; r.SetParent(pai, false); Posicionar(r, x, y, w, h); return r;
    }
    static void Posicionar(RectTransform r, float x, float y, float w, float h)
    {
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); r.localScale = Vector3.one;
    }
    static Image Imagem(Transform pai, string nome, float x, float y, float w, float h, string arte = null)
    {
        var i = Caixa(pai, nome, x, y, w, h).gameObject.AddComponent<Image>();
        if (arte != null) i.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Pasta + arte + ".asset");
        i.raycastTarget = false; return i;
    }
    static TextMeshProUGUI Texto(Transform pai, string nome, float x, float y, float w, float h, string texto, float tamanho)
    {
        var t = Caixa(pai, nome, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>();
        t.font = fonte; t.text = texto; Estilo(t, tamanho); return t;
    }
    static void Estilo(TextMeshProUGUI t, float tamanho)
    {
        t.fontSize = tamanho; t.enableAutoSizing = false; t.fontStyle = FontStyles.Normal;
        t.color = Creme; t.alignment = TextAlignmentOptions.MidlineLeft; t.raycastTarget = false;
        t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Overflow;
    }
    static void Alongar(RectTransform r)
    {
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero;
    }
    static ScrollRect Rolagem(RectTransform quadro, RectTransform conteudo, float y, float h)
    {
        var view = Imagem(quadro, "Viewport", 38, y, quadro.sizeDelta.x - 80, h);
        view.color = new Color(0, 0, 0, .001f); view.raycastTarget = true; view.gameObject.AddComponent<RectMask2D>();
        conteudo.SetParent(view.transform, false);
        conteudo.anchorMin = new Vector2(0, 1); conteudo.anchorMax = Vector2.one;
        conteudo.pivot = new Vector2(.5f, 1); conteudo.anchoredPosition = Vector2.zero;
        conteudo.sizeDelta = new Vector2(0, h); conteudo.localScale = Vector3.one;
        var scroll = quadro.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = view.rectTransform; scroll.content = conteudo;
        scroll.horizontal = false; scroll.vertical = true; scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 55;
        var track = Imagem(quadro, "Trilha", quadro.sizeDelta.x - 28, y, 12, h);
        track.color = new Color(.09f, .08f, .06f); track.raycastTarget = true;
        var barra = track.gameObject.AddComponent<Scrollbar>(); barra.direction = Scrollbar.Direction.BottomToTop;
        var pega = Imagem(track.transform, "Pega", 0, 0, 12, h);
        pega.color = new Color(.63f, .43f, .19f); pega.raycastTarget = true;
        pega.rectTransform.pivot = Vector2.one * .5f;
        pega.rectTransform.sizeDelta = Vector2.zero; pega.rectTransform.anchoredPosition = Vector2.zero;
        barra.handleRect = pega.rectTransform; barra.targetGraphic = pega;
        scroll.verticalScrollbar = barra; scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        return scroll;
    }
    static void Barra(Slider slider, Transform raiz, float x, float y, float w, float h)
    {
        slider.transform.SetParent(raiz, false); Posicionar((RectTransform)slider.transform, x, y, w, h);
        foreach (Transform filho in slider.transform) filho.gameObject.SetActive(false);
        var fill = Imagem(slider.transform, "VidaIlustrada", 0, 0, w, h); fill.color = new Color(.78f, .13f, .1f);
        Alongar(fill.rectTransform); slider.fillRect = fill.rectTransform; slider.handleRect = null;
        slider.direction = Slider.Direction.LeftToRight; slider.interactable = false; slider.transition = Selectable.Transition.None;
    }
    static ScrollRect Lista(Transform raiz, Transform conteudo, string titulo, out GameObject quadro)
    {
        var img = Imagem(raiz, "Quadro" + titulo, 350, 195, 972, 402, "moldura-log");
        Texto(img.transform, "Titulo", 48, 25, 850, 50, titulo + "  ·  role para ver todas as opções", 30);
        var scroll = Rolagem(img.rectTransform, (RectTransform)conteudo, 88, 276);
        var fundoAntigo = conteudo.GetComponent<Image>(); if (fundoAntigo != null) fundoAntigo.enabled = false;
        var v = conteudo.GetComponent<VerticalLayoutGroup>();
        v.padding = new RectOffset(10, 10, 4, 4); v.spacing = 8;
        v.childControlWidth = v.childControlHeight = true; v.childForceExpandWidth = true; v.childForceExpandHeight = false;
        var fit = conteudo.GetComponent<ContentSizeFitter>();
        fit.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        quadro = img.gameObject; quadro.SetActive(false); return scroll;
    }

    public static string Aplicar()
    {
        var cena = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (Application.dataPath.Replace('\\', '/') != "C:/Users/silve/DroidsCode/Assets" || EditorApplication.isPlaying ||
            cena.path != "Assets/Scenes/Battle.unity" || cena.isDirty) throw new InvalidOperationException("Exige Battle limpa, projeto correto e fora de Play.");
        var b = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
        var canvas = b.botaoAtacar.GetComponentInParent<Canvas>();
        if (canvas.GetComponentInChildren<ApresentacaoBattleIlustrada>(true) != null) throw new InvalidOperationException("Já montada; não repetir.");
        var botoes = new[] { b.botaoAtacar, b.botaoItem, b.botaoStatus, b.botaoFugir };
        if (botoes.Any(x => x == null) || botoes.Distinct().Count() != 4) throw new InvalidOperationException("Exige quatro botões existentes.");
        fonte = b.botaoAtacar.GetComponentInChildren<TextMeshProUGUI>(true).font;
        var raiz = Caixa(canvas.transform, "BattleIlustrada", 0, 0, 1672, 941);
        raiz.SetAsFirstSibling();
        var a = raiz.gameObject.AddComponent<ApresentacaoBattleIlustrada>(); a.batalha = b;
        foreach (var nome in new[] { "Mapa", "SpritePlayer", "SpriteInimigo", "Inimigo" })
        {
            var r = canvas.GetComponentsInChildren<RectTransform>(true).Single(t => t.name == nome);
            r.SetParent(raiz, false);
            if (nome == "Mapa") Posicionar(r, 0, 0, 1672, 775);
            else if (nome == "SpritePlayer") Posicionar(r, 200, 367, 280, 240);
            else Posicionar(r, 1120, 239, 230, 230);
            r.GetComponent<Image>().raycastTarget = false; r.GetComponent<Image>().preserveAspect = nome != "Mapa";
        }
        Imagem(raiz, "HudApollo", 20, 18, 450, 169, "hud-apollo");
        Imagem(raiz, "HudInimigo", 1220, 18, 432, 131, "hud-inimigo");
        a.nomePlayer = Texto(raiz, "NomeApollo", 85, 26, 215, 43, "APOLLO", 36);
        a.nomeInimigo = Texto(raiz, "NomeInimigo", 1410, 26, 195, 43, b.nomeInimigo, 34);
        foreach (var t in new[] { a.nomePlayer, a.nomeInimigo })
        { t.enableAutoSizing = true; t.fontSizeMin = 22; t.fontSizeMax = 36; t.overflowMode = TextOverflowModes.Ellipsis; }
        Barra(b.sliderPlayer, raiz, 62, 93, 368, 23);
        Barra(b.sliderInimigo, raiz, 1260, 93, 348, 23);
        var trilhaEnergia = Caixa(raiz, "EnergiaApollo", 62, 138, 368, 23);
        a.energia = Imagem(trilhaEnergia, "Saldo", 0, 0, 368, 23); a.energia.color = new Color(.1f, .64f, .87f);
        Alongar(a.energia.rectTransform);
        a.hpPlayer = Texto(raiz, "HpApollo", 68, 89, 355, 32, "HP", 26);
        a.textoEnergia = Texto(raiz, "TextoEnergia", 68, 133, 355, 32, "ENERGIA", 26);
        a.hpInimigo = Texto(raiz, "HpInimigo", 1266, 89, 330, 32, "HP", 26);
        var quadroLog = Imagem(raiz, "QuadroLog", 280, 612, 1112, 202, "moldura-log");
        a.log = Rolagem(quadroLog.rectTransform, b.textoMensagem.rectTransform, 26, 148);
        Estilo(b.textoMensagem, 32); b.textoMensagem.alignment = TextAlignmentOptions.TopLeft;
        b.textoMensagem.text = "";
        var artes = new[] { "botao-atacar", "botao-item", "botao-status", "botao-fugir" };
        var nomes = new[] { "Atacar", "Item", "Status", "Fugir" };
        for (int i = 0; i < 4; i++)
        {
            var botao = botoes[i]; botao.transform.SetParent(raiz, false);
            Posicionar((RectTransform)botao.transform, 20 + i * 410, 824, 402, 100);
            var img = botao.GetComponent<Image>(); img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Pasta + artes[i] + ".asset");
            img.type = Image.Type.Simple; img.color = Color.white;
            botao.targetGraphic = img; botao.transition = Selectable.Transition.ColorTint;
            var cores = botao.colors; cores.normalColor = Color.white; cores.highlightedColor = new Color(1, .85f, .6f);
            cores.pressedColor = new Color(.65f, .55f, .38f); cores.selectedColor = new Color(1, .85f, .6f);
            cores.disabledColor = new Color(.45f, .45f, .45f); cores.colorMultiplier = 1; botao.colors = cores;
            var t = botao.GetComponentInChildren<TextMeshProUGUI>(true); Estilo(t, 44); t.text = nomes[i];
            Posicionar(t.rectTransform, 180, 7, 205, 84);
        }
        a.molduraLista = AssetDatabase.LoadAssetAtPath<Sprite>(Pasta + "moldura-log.asset");
        a.ataques = Lista(raiz, b.painelListaDeAtaques, "GOLPES", out a.quadroAtaques);
        a.itens = Lista(raiz, b.painelListaDeItens, "ITENS", out a.quadroItens);
        b.painelListaDeAtaques.gameObject.SetActive(false); b.painelListaDeItens.gameObject.SetActive(false);
        b.telaDeStatus.painel.SetActive(false);
        a.AtualizarLayout(); a.AtualizarConteudo();
        EditorSceneManager.MarkSceneDirty(cena); EditorSceneManager.SaveScene(cena);
        AssetDatabase.SaveAssets();
        return "Battle salva: quatro botões/sliders/log/pais das listas originais preservados; apresentação local adicionada.";
    }
}
