using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Aplica uma única migração visual ao ramo terminal; não toca cenas ou regras.</summary>
public static class RefocoTerminalIlustrado
{
    const string Caminho = "Assets/Prefabs/Canvas.prefab";
    const string Arte = "Assets/UI/TerminalIlustrado/terminal-foco-horizontal.png";
    static readonly Color Verde = new Color(0.68f, 1f, 0.73f);

    static void Caixa(RectTransform r,float x,float y,float w,float h)
    {
        r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);
        r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;
    }

    static void Fonte(TMP_Text t,float tamanho,Color cor)
    {
        t.fontSize=tamanho;t.enableAutoSizing=false;t.color=cor;t.margin=Vector4.zero;
        t.raycastTarget=false;t.fontStyle=FontStyles.Normal;
    }

    static TextMeshProUGUI Rotulo(Transform pai,string nome,string texto,TMP_FontAsset fonte,float x,float y,float w,float h,float tamanho,Color cor)
    {
        var go=new GameObject(nome,typeof(RectTransform),typeof(TextMeshProUGUI));go.layer=5;go.transform.SetParent(pai,false);
        var t=go.GetComponent<TextMeshProUGUI>();t.font=fonte;t.text=texto;t.alignment=TextAlignmentOptions.MidlineLeft;
        t.textWrappingMode=TextWrappingModes.NoWrap;Fonte(t,tamanho,cor);Caixa(t.rectTransform,x,y,w,h);return t;
    }

    static void Botao(Button b,string texto,float x,float largura)
    {
        Caixa((RectTransform)b.transform,x,858,largura,91);
        var t=b.GetComponentsInChildren<TextMeshProUGUI>(true).First();t.gameObject.SetActive(true);t.text=texto;
        Fonte(t,42,texto=="EXECUTAR"?new Color(0.045f,0.03f,0.012f):Verde);t.alignment=TextAlignmentOptions.Center;t.textWrappingMode=TextWrappingModes.NoWrap;
        var r=t.rectTransform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
    }

    public static string Aplicar()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Aplicar fora de Play.");
        var imp=(TextureImporter)AssetImporter.GetAtPath(Arte);
        imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;
        imp.mipmapEnabled=false;imp.maxTextureSize=2048;imp.textureCompression=TextureImporterCompression.Uncompressed;
        imp.filterMode=FilterMode.Bilinear;imp.SaveAndReimport();
        var raiz=PrefabUtility.LoadPrefabContents(Caminho);
        try
        {
            var t=raiz.GetComponentInChildren<TerminalUIManager>(true);var painel=(RectTransform)t.transform;
            if(painel.Find("FundoFoco")!=null)throw new InvalidOperationException("Refoco já aplicado; consultar antes de repetir.");
            var a=t.GetComponent<ApresentacaoTerminalIlustrado>();
            a.referencia=new Vector2(1536,1024);a.corteSuperior=a.corteInferior=0;a.margemDaTela=0.98f;
            var fundo=new GameObject("FundoFoco",typeof(RectTransform),typeof(Image));fundo.layer=5;fundo.transform.SetParent(painel,false);fundo.transform.SetAsFirstSibling();
            fundo.GetComponent<Image>().color=new Color(0.01f,0.013f,0.011f,1f);a.fundoFoco=(RectTransform)fundo.transform;
            painel.Find("ArteTerminal").GetComponent<Image>().sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Arte);
            Caixa(t.campoDeCodigo.GetComponent<RectTransform>(),136,166,824,642);
            Fonte(t.campoDeCodigo.textComponent,40,Verde);
            if(t.campoDeCodigo.placeholder is TMP_Text ph)Fonte(ph,40,new Color(0.55f,0.75f,0.6f,0.65f));
            Caixa((RectTransform)a.numerosDeLinha.transform.parent,80,166,47,642);Fonte(a.numerosDeLinha,40,new Color(0.4f,0.72f,0.48f));
            var fonte=t.campoDeCodigo.textComponent.font;
            Rotulo(painel,"TituloTerminal","TERMINAL",fonte,90,77,560,45,40,Verde);
            Rotulo(painel,"RotuloPronto","PRONTO",fonte,815,78,190,42,32,Verde);
            Rotulo(painel,"RotuloExecutando","EXECUTANDO",fonte,1060,78,225,42,32,new Color(1f,0.79f,0.35f));
            Rotulo(painel,"RotuloErro","ERRO",fonte,1320,78,120,42,32,new Color(1f,0.48f,0.38f));
            Caixa(a.luzPronto.rectTransform,782,88,24,24);Caixa(a.luzExecutando.rectTransform,1027,88,24,24);Caixa(a.luzErro.rectTransform,1287,88,24,24);
            Rotulo(painel,"TituloObjetivo","OBJETIVO",fonte,1036,163,380,44,38,new Color(0.11f,0.08f,0.025f));
            Caixa(t.textoAjuda.GetComponentInParent<ScrollRect>().GetComponent<RectTransform>(),1036,220,402,134);
            Fonte(t.textoAjuda,32,new Color(0.09f,0.065f,0.02f));
            ConfigurarIndicadores(t);
            Rotulo(painel,"TituloSaida","SAÍDA",fonte,1030,413,380,44,38,Verde);
            Caixa(t.scrollRectSaida.GetComponent<RectTransform>(),1030,473,397,178);Fonte(t.textoSaida,34,Verde);
            Caixa(t.scrollRectSaida.verticalScrollbar.GetComponent<RectTransform>(),410,0,14,178);
            Rotulo(painel,"TituloApollo","APOLLO",fonte,1030,700,240,44,36,Verde);
            Caixa(t.textoPontos.rectTransform,1030,754,265,71);Fonte(t.textoPontos,32,Verde);
            Botao(t.botaoLimpar,"LIMPAR",80,302);Botao(t.botaoExecutar,"EXECUTAR",412,320);Botao(t.botaoFechar,"FECHAR",758,294);
            Rotulo(painel,"RotuloDepuracao","Depuração",fonte,1082,885,175,44,30,Verde);
            Caixa(a.depuracao.GetComponent<RectTransform>(),1262,886,40,40);
            foreach(var marca in a.depuracao.GetComponentsInChildren<TextMeshProUGUI>(true))Fonte(marca,38,Verde);
            Caixa(a.estadoDaDepuracao.rectTransform,1316,887,142,42);Fonte(a.estadoDaDepuracao,30,Verde);
            a.AtualizarDados();a.AtualizarEscala();PrefabUtility.SaveAsPrefabAsset(raiz,Caminho);
            return "Terminal 1536x1024, fontes 40/34/32 e fundo exclusivo de foco aplicados.";
        }
        finally {PrefabUtility.UnloadPrefabContents(raiz);}
    }

    public static void ConfigurarIndicadores(TerminalUIManager t)
    {
        var objetivo=t.textoAjuda.GetComponentInParent<ScrollRect>();
        if(objetivo.verticalScrollbar==null) {
            var go=UnityEngine.Object.Instantiate(t.scrollRectSaida.verticalScrollbar.gameObject,objetivo.transform,false);go.name="RolagemObjetivo";
            var bar=go.GetComponent<Scrollbar>();Caixa((RectTransform)go.transform,390,0,12,134);
            bar.GetComponent<Image>().color=new Color(0.4f,0.27f,0.1f,0.35f);
            bar.handleRect.GetComponent<Image>().color=new Color(0.23f,0.15f,0.035f);
            objetivo.verticalScrollbar=bar;objetivo.verticalScrollbarVisibility=ScrollRect.ScrollbarVisibility.AutoHide;
        }
        var margem=objetivo.viewport.offsetMax;margem.x=-20;objetivo.viewport.offsetMax=margem;
        var a=t.GetComponent<ApresentacaoTerminalIlustrado>();
        a.depuracao.GetComponent<Image>().color=new Color(0.015f,0.055f,0.025f);
        var borda=a.depuracao.GetComponent<Outline>()??a.depuracao.gameObject.AddComponent<Outline>();
        borda.effectColor=new Color(0.35f,0.75f,0.45f);borda.effectDistance=new Vector2(2,-2);
    }
}
