using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Apresentação do menu existente. Não troca referências, eventos nem ações.</summary>
[ExecuteAlways,DisallowMultipleComponent,RequireComponent(typeof(RectTransform))]
public sealed class ApresentacaoMenuMundoIlustrado : MonoBehaviour
{
    public MenuMundoManager menu;
    public RectTransform fundoFoco;
    public Vector2 referencia=new Vector2(1254,1254);
    public float margem=.95f;
    public Rect[] regioes={
        new Rect(222,289,350,99),new Rect(643,289,350,99),
        new Rect(222,447,350,99),new Rect(643,447,350,99),
        new Rect(222,605,350,99),new Rect(643,605,350,99),
        new Rect(222,765,350,99),new Rect(643,765,350,99)
    };
    bool ajustando;
    static readonly string[] Rotulos={"Carregar","Salvar","Terminal","Status","Bag","Droid","Opções","Voltar"};

    public Button[] Botoes => menu==null?new Button[0]:new[]{menu.botaoCarregar,menu.botaoSalvar,menu.botaoTerminal,
        menu.botaoStatus,menu.botaoBag,menu.botaoDroid,menu.botaoOpcoes,menu.botaoVoltar};

    void OnEnable(){AplicarApresentacao();AtualizarEscala();}
    void LateUpdate()=>AtualizarEscala();
    void OnRectTransformDimensionsChange()=>AtualizarEscala();

    public static void Caixa(RectTransform r,float x,float y,float largura,float altura)
    {
        r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);
        r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(largura,altura);r.localScale=Vector3.one;
    }

    public void AplicarApresentacao()
    {
        if(menu==null||regioes==null||regioes.Length!=8)return;
        var raiz=GetComponent<Image>();if(raiz!=null)raiz.enabled=false;
        var botoes=Botoes;
        for(int i=0;i<8;i++)
        {
            var b=botoes[i];if(b==null)continue;var rect=regioes[i];
            Caixa((RectTransform)b.transform,rect.x,rect.y,rect.width,rect.height);
            var imagem=b.GetComponent<Image>();
            if(imagem!=null){imagem.color=Color.white;imagem.raycastTarget=true;imagem.canvasRenderer.cullTransparentMesh=false;}
            b.transition=Selectable.Transition.ColorTint;
            var cores=b.colors;cores.normalColor=new Color(1,1,1,0);cores.highlightedColor=new Color(1,.8f,.45f,.16f);
            cores.selectedColor=new Color(1,.8f,.45f,.1f);cores.pressedColor=new Color(.4f,.3f,.12f,.32f);
            cores.disabledColor=new Color(.2f,.2f,.2f,.15f);cores.colorMultiplier=1;b.colors=cores;
            var t=b.GetComponentInChildren<TextMeshProUGUI>(true);if(t==null)continue;
            t.text=Rotulos[i];t.fontSize=52;t.enableAutoSizing=false;t.fontStyle=FontStyles.Normal;
            t.color=new Color(.96f,.91f,.76f);t.alignment=TextAlignmentOptions.MidlineLeft;
            t.textWrappingMode=TextWrappingModes.NoWrap;t.overflowMode=TextOverflowModes.Truncate;t.raycastTarget=false;
            Caixa(t.rectTransform,115,0,rect.width-125,rect.height);
        }
        if(menu.textoAviso!=null)
        {
            Caixa(menu.textoAviso.rectTransform,300,985,615,115);
            menu.textoAviso.fontSize=42;menu.textoAviso.enableAutoSizing=false;menu.textoAviso.fontStyle=FontStyles.Normal;
            menu.textoAviso.color=new Color(.96f,.91f,.76f);menu.textoAviso.alignment=TextAlignmentOptions.Center;
            menu.textoAviso.textWrappingMode=TextWrappingModes.Normal;menu.textoAviso.raycastTarget=false;
        }
    }

    public void AtualizarEscala()
    {
        if(ajustando||referencia.x<=0||referencia.y<=0||transform.parent==null)return;
        var area=transform as RectTransform;var canvas=GetComponentInParent<Canvas>();if(area==null||canvas==null)return;
        var raiz=canvas.rootCanvas.transform as RectTransform;if(raiz==null||raiz.rect.width<=0||raiz.rect.height<=0)return;
        var herdada=transform.parent.lossyScale;if(Mathf.Abs(herdada.x)<.00001f||Mathf.Abs(herdada.y)<.00001f)return;
        ajustando=true;
        try
        {
            float fator=Mathf.Min(raiz.rect.width*Mathf.Abs(raiz.lossyScale.x)/referencia.x,
                raiz.rect.height*Mathf.Abs(raiz.lossyScale.y)/referencia.y)*margem;
            area.anchorMin=area.anchorMax=area.pivot=Vector2.one*.5f;area.sizeDelta=referencia;
            area.localScale=new Vector3(fator/Mathf.Abs(herdada.x),fator/Mathf.Abs(herdada.y),1);
            area.position=raiz.TransformPoint(raiz.rect.center);
            if(fundoFoco!=null)
            {
                fundoFoco.sizeDelta=new Vector2(raiz.rect.width*Mathf.Abs(raiz.lossyScale.x)/Mathf.Abs(area.lossyScale.x),
                    raiz.rect.height*Mathf.Abs(raiz.lossyScale.y)/Mathf.Abs(area.lossyScale.y));
                fundoFoco.position=area.position;
            }
        }
        finally{ajustando=false;}
    }
}
