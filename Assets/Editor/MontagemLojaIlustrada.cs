using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Migração única do ramo Loja da City. Não altera o Canvas nem os demais painéis.</summary>
public static class MontagemLojaIlustrada
{
    static readonly Color Creme=new Color(.94f,.87f,.68f);
    public static void Caixa(RectTransform r,float x,float y,float w,float h)
    {
        r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);
        r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;
    }
    static GameObject Objeto(Transform pai,string nome,params Type[] tipos)
    {
        var go=new GameObject(nome,new[]{typeof(RectTransform)}.Concat(tipos).ToArray());
        go.layer=5;go.transform.SetParent(pai,false);return go;
    }
    static Image Imagem(Transform pai,string nome,Color cor,float x,float y,float w,float h)
    {
        var i=Objeto(pai,nome,typeof(Image)).GetComponent<Image>();i.color=cor;
        Caixa(i.rectTransform,x,y,w,h);return i;
    }
    public static TextMeshProUGUI Texto(Transform pai,string nome,string texto,TMP_FontAsset fonte,float x,float y,float w,float h,float tamanho)
    {
        var t=Objeto(pai,nome,typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        t.font=fonte;t.text=texto;t.fontSize=tamanho;t.enableAutoSizing=false;t.color=Creme;
        t.alignment=TextAlignmentOptions.MidlineLeft;t.raycastTarget=false;
        t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;
        Caixa(t.rectTransform,x,y,w,h);return t;
    }
    static Button Botao(Transform pai,string nome,string texto,TMP_FontAsset fonte,float x,float y,float w,float h)
    {
        var i=Imagem(pai,nome,new Color(.25f,.23f,.17f),x,y,w,h);
        var contorno=i.gameObject.AddComponent<Outline>();contorno.effectColor=new Color(.65f,.48f,.24f);contorno.effectDistance=new Vector2(2,-2);
        var b=i.gameObject.AddComponent<Button>();b.targetGraphic=i;
        var t=Texto(i.transform,"Legenda",texto,fonte,10,4,w-20,h-8,36);t.alignment=TextAlignmentOptions.Center;
        var cores=b.colors;cores.highlightedColor=new Color(1.25f,1.2f,1.05f);cores.selectedColor=Color.white;cores.disabledColor=new Color(.45f,.45f,.45f);b.colors=cores;
        return b;
    }
    static ScrollRect Rolagem(Transform pai,string nome,float x,float y,float w,float h,out RectTransform conteudo)
    {
        var area=Imagem(pai,nome,new Color(.05f,.055f,.05f,.5f),x,y,w,h);
        var scroll=area.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=55;
        var view=Objeto(area.transform,"Viewport",typeof(RectMask2D)).GetComponent<RectTransform>();Caixa(view,0,0,w-24,h);scroll.viewport=view;
        conteudo=Objeto(view,"Conteudo").GetComponent<RectTransform>();Caixa(conteudo,0,0,w-24,h);scroll.content=conteudo;
        var bar=Imagem(area.transform,"Barra",new Color(.12f,.12f,.10f),w-18,0,14,h);
        var handle=Imagem(bar.transform,"Handle",new Color(.68f,.51f,.26f),0,0,14,60);
        var sr=bar.gameObject.AddComponent<Scrollbar>();sr.handleRect=handle.rectTransform;sr.targetGraphic=handle;sr.direction=Scrollbar.Direction.BottomToTop;
        scroll.verticalScrollbar=sr;scroll.verticalScrollbarVisibility=ScrollRect.ScrollbarVisibility.AutoHide;
        return scroll;
    }
    static Sprite Importar(string path)
    {
        var imp=(TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;
        imp.mipmapEnabled=false;imp.maxTextureSize=2048;imp.textureCompression=TextureImporterCompression.Uncompressed;
        imp.filterMode=FilterMode.Bilinear;imp.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    public static string Aplicar()
    {
        if(EditorApplication.isPlaying) throw new InvalidOperationException("Montagem exige fora de Play.");
        var cena=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(cena.path!="Assets/Scenes/City.unity"||cena.isDirty)throw new InvalidOperationException("Exige City limpa e ativa.");
        var lojas=cena.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<LojaUIManager>(true)).ToArray();
        if(lojas.Length!=1)throw new InvalidOperationException("Esperada exatamente uma loja.");
        var loja=lojas[0];if(loja.apresentacao!=null)throw new InvalidOperationException("Loja já montada; não repetir.");
        var painel=loja.transform;var fonte=loja.textoGold.font;
        var fundo=Importar("Assets/UI/LojaIlustrada/loja-fundo.png");
        Importar("Assets/UI/LojaIlustrada/loja-icones.png");
        var textura=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UI/LojaIlustrada/loja-icones.png");
        var icones=new Sprite[6];
        for(int i=0;i<6;i++)
        {
            string path=$"Assets/UI/LojaIlustrada/icone-{i}.asset";
            if(AssetDatabase.LoadAssetAtPath<Sprite>(path)!=null)throw new InvalidOperationException("Ícone já existe: "+path);
            var s=Sprite.Create(textura,new Rect((i%3)*512,(1-i/3)*512,512,512),Vector2.one*.5f,100);
            s.name="LojaIcone"+i;AssetDatabase.CreateAsset(s,path);icones[i]=s;
        }
        // Os objetos anteriores ficam inativos para preservar IDs e referências; o prefab compartilhado de item não é editado.
        foreach(Transform filho in painel)filho.gameObject.SetActive(false);
        var imagemRaiz=loja.GetComponent<Image>();if(imagemRaiz!=null)imagemRaiz.enabled=false;
        var a=loja.gameObject.AddComponent<ApresentacaoLojaIlustrada>();a.loja=loja;a.icones=icones;loja.apresentacao=a;
        a.fundoFoco=Imagem(painel,"FundoLoja",new Color(.025f,.027f,.025f),0,0,1536,1024).rectTransform;
        a.fundoFoco.anchorMin=a.fundoFoco.anchorMax=a.fundoFoco.pivot=Vector2.one*.5f;
        var arte=Imagem(painel,"ArteLoja",Color.white,0,0,1536,1024);arte.sprite=fundo;arte.raycastTarget=false;
        Texto(painel,"TituloLoja","LOJA",fonte,100,55,300,65,54);
        loja.botaoAbaComprar=Botao(painel,"AbaComprar","COMPRAR",fonte,460,66,270,66);
        loja.botaoAbaVender=Botao(painel,"AbaVender","VENDER",fonte,750,66,250,66);
        loja.textoGold=Texto(painel,"Saldo","Gold: 0",fonte,1060,65,365,65,42);
        Texto(painel,"TituloProdutos","PRODUTOS",fonte,85,207,650,53,38);
        var lista=Rolagem(painel,"ProdutosRolaveis",73,277,852,530,out var conteudo);
        loja.painelListaDeItens=conteudo;
        var grade=conteudo.gameObject.AddComponent<GridLayoutGroup>();grade.cellSize=new Vector2(400,154);grade.spacing=new Vector2(12,14);
        grade.constraint=GridLayoutGroup.Constraint.FixedColumnCount;grade.constraintCount=2;grade.padding=new RectOffset(3,3,3,3);
        var fit=conteudo.gameObject.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;
        var item=Imagem(painel,"ProdutoTemplate",new Color(.13f,.14f,.13f),0,0,400,154);
        loja.prefabBotaoItem=item.gameObject.AddComponent<Button>();loja.prefabBotaoItem.targetGraphic=item;
        var borda=item.gameObject.AddComponent<Outline>();borda.effectColor=new Color(.48f,.39f,.23f);borda.effectDistance=new Vector2(1,-1);
        Imagem(item.transform,"Icone",Color.white,10,27,90,90).preserveAspect=true;
        Texto(item.transform,"Nome","Produto",fonte,115,10,275,88,36);
        Texto(item.transform,"Preco","0 Gold",fonte,115,105,275,38,32);
        item.gameObject.SetActive(false);
        a.vazio=Texto(painel,"ListaVazia","Nenhum produto disponível.",fonte,100,330,760,200,38);a.vazio.gameObject.SetActive(false);
        a.iconeItem=Imagem(painel,"IconeSelecionado",Color.white,1050,230,120,120);a.iconeItem.preserveAspect=true;
        Texto(painel,"RotuloDetalhes","DETALHES",fonte,1200,260,260,50,36);
        a.nomeItem=Texto(painel,"NomeSelecionado","Selecione um item",fonte,1040,367,400,100,42);
        var detalhes=Rolagem(painel,"DescricaoRolavel",1040,490,405,205,out var textoConteudo);
        a.descricao=Texto(textoConteudo,"Descricao","Escolha um produto.",fonte,0,0,380,205,34);
        a.descricao.rectTransform.anchorMin=new Vector2(0,1);a.descricao.rectTransform.anchorMax=new Vector2(1,1);a.descricao.rectTransform.sizeDelta=new Vector2(0,205);
        var vertical=textoConteudo.gameObject.AddComponent<VerticalLayoutGroup>();vertical.childControlHeight=true;vertical.childControlWidth=true;vertical.childForceExpandHeight=false;
        textoConteudo.gameObject.AddComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;
        a.descricao.overflowMode=TextOverflowModes.Overflow;
        a.preco=Texto(painel,"PrecoSelecionado","",fonte,1040,710,400,55,46);
        a.estoque=Texto(painel,"EstoqueSelecionado","",fonte,1040,775,400,45,34);
        loja.textoMensagem=Texto(painel,"AjudaSelecao","Selecione um produto para consultar.",fonte,85,905,730,76,32);
        loja.botaoAcao=Botao(painel,"ConfirmarOperacao","COMPRAR 1",fonte,1025,910,420,65);
        a.rotuloAcao=loja.botaoAcao.GetComponentInChildren<TextMeshProUGUI>();loja.botaoAcao.interactable=false;
        loja.botaoVoltar=Botao(painel,"FecharLoja","VOLTAR",fonte,835,910,170,65);
        a.MostrarSelecao(null,false);a.AtualizarEscala();loja.painel.SetActive(false);
        EditorSceneManager.MarkSceneDirty(cena);EditorSceneManager.SaveScene(cena);AssetDatabase.SaveAssets();
        return "Uma loja montada e City salva; Canvas/Scaler e demais ramos preservados.";
    }
}
