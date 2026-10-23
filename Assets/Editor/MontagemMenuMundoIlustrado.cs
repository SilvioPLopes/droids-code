using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Montagem única e delimitada ao PainelMenuMundo do prefab compartilhado.</summary>
public static class MontagemMenuMundoIlustrado
{
    public const string Prefab="Assets/Prefabs/Canvas.prefab";
    const string Arte="Assets/UI/MenuMundoIlustrado/menu-fundo.png";
    static GameObject Objeto(Transform pai,string nome,params Type[] tipos)
    {
        var go=new GameObject(nome,tipos);go.layer=5;go.transform.SetParent(pai,false);return go;
    }
    public static string Aplicar()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Aplicar fora de Play.");
        var arquivo=AssetDatabase.LoadAssetAtPath<GameObject>(Prefab);
        var original=arquivo.GetComponentInChildren<MenuMundoManager>(true);
        if(original.painelMenu.GetComponent<ApresentacaoMenuMundoIlustrado>()!=null)throw new InvalidOperationException("Menu já montado; não repetir para obter prévia.");
        var imp=AssetImporter.GetAtPath(Arte) as TextureImporter;if(imp==null)throw new InvalidOperationException("Arte não importada.");
        imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.mipmapEnabled=false;
        imp.alphaIsTransparency=true;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.maxTextureSize=2048;imp.filterMode=FilterMode.Bilinear;imp.SaveAndReimport();
        var raiz=PrefabUtility.LoadPrefabContents(Prefab);
        try
        {
            var menu=raiz.GetComponentInChildren<MenuMundoManager>(true);var painel=menu.painelMenu.transform;
            var a=menu.painelMenu.AddComponent<ApresentacaoMenuMundoIlustrado>();a.menu=menu;
            var botoes=a.Botoes;if(botoes.Length!=8||Array.Exists(botoes,b=>b==null))throw new InvalidOperationException("Exige oito referências originais.");
            var foco=Objeto(painel,"FundoMenuIlustrado",typeof(RectTransform),typeof(Image));foco.transform.SetAsFirstSibling();
            foco.GetComponent<Image>().color=new Color(.025f,.03f,.025f,.8f);a.fundoFoco=(RectTransform)foco.transform;
            a.fundoFoco.anchorMin=a.fundoFoco.anchorMax=a.fundoFoco.pivot=Vector2.one*.5f;
            var arte=Objeto(painel,"ArteMenuMundo",typeof(RectTransform),typeof(Image));arte.transform.SetSiblingIndex(1);
            var imagem=arte.GetComponent<Image>();imagem.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Arte);imagem.raycastTarget=false;
            ApresentacaoMenuMundoIlustrado.Caixa(imagem.rectTransform,0,0,1254,1254);
            var titulo=Objeto(painel,"TituloMenuIlustrado",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            titulo.font=menu.botaoCarregar.GetComponentInChildren<TextMeshProUGUI>(true).font;titulo.text="MENU";titulo.fontSize=66;
            titulo.enableAutoSizing=false;titulo.color=new Color(.12f,.085f,.035f);titulo.alignment=TextAlignmentOptions.Center;titulo.raycastTarget=false;
            ApresentacaoMenuMundoIlustrado.Caixa(titulo.rectTransform,370,115,500,80);
            a.AplicarApresentacao();a.AtualizarEscala();PrefabUtility.SaveAsPrefabAsset(raiz,Prefab);
            return "Uma apresentação adicionada ao menu; oito botões originais reaproveitados. Nenhuma cena salva.";
        }
        finally{PrefabUtility.UnloadPrefabContents(raiz);}
    }
}
