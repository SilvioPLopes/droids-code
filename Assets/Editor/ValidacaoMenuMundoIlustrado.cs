using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Fixture isolada do menu. Não clica Salvar/Carregar nem inicia Play.</summary>
public static class ValidacaoMenuMundoIlustrado
{
    const BindingFlags Privado=BindingFlags.Instance|BindingFlags.NonPublic;
    static void Exigir(bool condicao,string erro){if(!condicao)throw new InvalidOperationException(erro);}
    public static string Executar(int largura,int altura)
    {
        Exigir(!EditorApplication.isPlaying,"Exige fora de Play.");
        var campo=typeof(GerenciadorDeEstado).GetField("instancia",BindingFlags.Static|BindingFlags.NonPublic);var anterior=campo.GetValue(null);
        var cena=EditorSceneManager.NewPreviewScene();Camera cam=null;RenderTexture rt=null;
        try
        {
            var estadoGo=new GameObject("EstadoTesteMenu",typeof(GerenciadorDeEstado));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo,cena);
            var estado=estadoGo.GetComponent<GerenciadorDeEstado>();campo.SetValue(null,estado);
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField",Privado).SetValue(estado,new Droid("TesteMenu",30));
            var cameraGo=new GameObject("CameraTesteMenu",typeof(Camera));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraGo,cena);
            cam=cameraGo.GetComponent<Camera>();cam.scene=cena;cam.orthographic=true;cam.orthographicSize=altura/2f;cam.aspect=(float)largura/altura;
            cam.transform.position=new Vector3(0,0,-10);cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.12f,.15f,.12f);cam.cullingMask=1<<30;
            rt=new RenderTexture(largura,altura,24);cam.targetTexture=rt;
            var origem=AssetDatabase.LoadAssetAtPath<GameObject>(MontagemMenuMundoIlustrado.Prefab);
            var raiz=UnityEngine.Object.Instantiate(origem);raiz.name="CanvasTesteMenu";UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(raiz,cena);
            var canvas=raiz.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=10;
            var menu=raiz.GetComponentInChildren<MenuMundoManager>(true);
            foreach(Transform filho in raiz.transform)if(filho!=menu.transform&&filho!=menu.painelMenu.transform)filho.gameObject.SetActive(false);
            var a=menu.painelMenu.GetComponent<ApresentacaoMenuMundoIlustrado>();Exigir(a!=null,"Apresentação não montada.");
            foreach(var tr in raiz.GetComponentsInChildren<Transform>(true))tr.gameObject.layer=30;
            typeof(MenuMundoManager).GetMethod("Alternar",Privado).Invoke(menu,null);
            Exigir(menu.painelMenu.activeSelf&&estado.MenuAberto,"Abertura não ativou menu/contador.");
            var botoes=a.Botoes;var rotulos=new[]{"Carregar","Salvar","Terminal","Status","Bag","Droid","Opções","Voltar"};
            Exigir(botoes.Length==8&&botoes.Distinct().Count()==8&&menu.painelMenu.GetComponentsInChildren<Button>(true).Length==8,"Alguma opção perdida, duplicada ou criada.");
            for(int i=0;i<8;i++)
            {
                Exigir(botoes[i].gameObject.activeSelf&&botoes[i].interactable,"Opção removida/desabilitada: "+rotulos[i]);
                Exigir(botoes[i].GetComponentInChildren<TextMeshProUGUI>(true).text==rotulos[i],"Rótulo perdido: "+rotulos[i]);
                if(i%2==0)Exigir(Mathf.Abs(((RectTransform)botoes[i].transform).anchoredPosition.y-((RectTransform)botoes[i+1].transform).anchoredPosition.y)<1,"Botões não estão em duas colunas alinhadas.");
            }
            // Exercitar destinos existentes sem gravar progresso nem executar código Lua.
            menu.botaoStatus.onClick.Invoke();Exigir(!menu.painelMenu.activeSelf&&menu.telaDeStatus.painel.activeSelf,"Status não abriu.");
            menu.telaDeStatus.Fechar();Exigir(menu.painelMenu.activeSelf,"Status não retornou ao menu.");
            menu.botaoBag.onClick.Invoke();Exigir(!menu.painelMenu.activeSelf&&menu.bag.painel.activeSelf,"Bag não abriu.");
            menu.bag.Fechar();Exigir(menu.painelMenu.activeSelf,"Bag não retornou ao menu.");
            menu.botaoDroid.onClick.Invoke();Exigir(!menu.painelMenu.activeSelf&&menu.telaDeDroid.painel.activeSelf,"Droid não abriu.");
            menu.telaDeDroid.Fechar();Exigir(menu.painelMenu.activeSelf,"Droid não retornou ao menu.");
            menu.botaoTerminal.onClick.Invoke();Exigir(!menu.painelMenu.activeSelf&&menu.terminal.EstaAberto,"Terminal não abriu.");
            menu.terminal.Fechar();Exigir(menu.painelMenu.activeSelf,"Terminal não retornou ao menu.");
            menu.botaoOpcoes.onClick.Invoke();Exigir(menu.painelMenu.activeSelf&&menu.textoAviso.text.Contains("ainda não foi implementado"),"Aviso de Opções se perdeu.");
            a.AtualizarEscala();Canvas.ForceUpdateCanvases();a.AtualizarEscala();Canvas.ForceUpdateCanvases();
            foreach(var texto in menu.painelMenu.GetComponentsInChildren<TextMeshProUGUI>())texto.ForceMeshUpdate();
            Exigir(!menu.textoAviso.isTextTruncated,"Aviso cortado na moldura.");
            var cantos=new Vector3[4];
            foreach(var b in botoes)
            {
                var texto=b.GetComponentInChildren<TextMeshProUGUI>();Exigir(!texto.isTextTruncated,"Rótulo cortado: "+texto.text);
                if(altura<=720)Exigir(texto.fontSize*Mathf.Abs(texto.transform.lossyScale.y)>=18,"Rótulo pequeno: "+texto.text);
                ((RectTransform)b.transform).GetWorldCorners(cantos);
                foreach(var canto in cantos){var pixel=cam.WorldToScreenPoint(canto);Exigir(pixel.x>=0&&pixel.y>=0&&pixel.x<=largura&&pixel.y<=altura,"Opção fora da tela: "+texto.text);}
            }
            cam.Render();
            var eventosGo=new GameObject("EventosTesteMenu",typeof(EventSystem));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(eventosGo,cena);
            var raycaster=raiz.GetComponent<GraphicRaycaster>();
            foreach(var b in botoes)
            {
                var hits=new System.Collections.Generic.List<RaycastResult>();var r=(RectTransform)b.transform;
                raycaster.Raycast(new PointerEventData(eventosGo.GetComponent<EventSystem>()){position=cam.WorldToScreenPoint(r.TransformPoint(r.rect.center))},hits);
                Exigir(hits.Count>0&&hits[0].gameObject.GetComponentInParent<Button>()==b,"Clique bloqueado: "+b.name);
            }
            var rtAnterior=RenderTexture.active;Texture2D imagem=null;
            try{RenderTexture.active=rt;imagem=new Texture2D(largura,altura,TextureFormat.RGB24,false);imagem.ReadPixels(new Rect(0,0,largura,altura),0,0);imagem.Apply();Directory.CreateDirectory("output/menu-mundo-ilustrado");File.WriteAllBytes($"output/menu-mundo-ilustrado/preview-{largura}x{altura}.png",imagem.EncodeToPNG());}
            finally{RenderTexture.active=rtAnterior;if(imagem!=null)UnityEngine.Object.DestroyImmediate(imagem);}
            menu.botaoVoltar.onClick.Invoke();Exigir(!menu.painelMenu.activeSelf&&!estado.MenuAberto,"Voltar deixou painel/contador aberto.");
            return $"OK {largura}x{altura}: oito opções/áreas clicáveis, duas colunas, fontes/enquadramento, Status/Bag/Droid/Terminal e retornos, aviso Opções e Voltar. Salvar/Carregar não invocados; sem saves.";
        }
        finally{if(cam!=null)cam.targetTexture=null;if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}EditorSceneManager.ClosePreviewScene(cena);campo.SetValue(null,anterior);}
    }
}
