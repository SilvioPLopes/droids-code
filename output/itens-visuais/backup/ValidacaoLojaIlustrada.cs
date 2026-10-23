using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DroidsCode.DroidCore;

/// <summary>Render e comportamento em preview isolada; nunca grava save.</summary>
public static class ValidacaoLojaIlustrada
{
    static void Exigir(bool condicao,string erro) {if(!condicao)throw new InvalidOperationException(erro);}
    public static string Executar(int largura,int altura)
    {
        Exigir(!EditorApplication.isPlaying,"Exige fora de Play.");
        var singleton=typeof(GerenciadorDeEstado).GetField("instancia",BindingFlags.Static|BindingFlags.NonPublic);
        var singletonDialogo=typeof(DialogoUIManager).GetField("_instancia",BindingFlags.Static|BindingFlags.NonPublic);
        var anterior=singleton.GetValue(null);var dialogoAnterior=singletonDialogo.GetValue(null);
        var fonte=Resources.FindObjectsOfTypeAll<LojaUIManager>().Single(x=>x.gameObject.scene.name=="City");
        var cena=EditorSceneManager.NewPreviewScene();RenderTexture rt=null;
        try
        {
            var cameraGo=new GameObject("CameraTesteLoja",typeof(Camera));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraGo,cena);
            var cam=cameraGo.GetComponent<Camera>();cam.scene=cena;cam.transform.position=new Vector3(0,0,-10);cam.orthographic=true;cam.orthographicSize=altura/2f;cam.aspect=(float)largura/altura;cam.cullingMask=1<<30;
            rt=new RenderTexture(largura,altura,24);cam.targetTexture=rt;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Color.black;
            var canvasGo=new GameObject("CanvasTesteLoja",typeof(RectTransform),typeof(Canvas),typeof(GraphicRaycaster));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(canvasGo,cena);
            var canvas=canvasGo.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=10;
            var estadoGo=new GameObject("EstadoTesteLoja",typeof(GerenciadorDeEstado));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo,cena);
            var estado=estadoGo.GetComponent<GerenciadorDeEstado>();singleton.SetValue(null,estado);estado.CarregarInventario(new System.Collections.Generic.Dictionary<string,int>());estado.CarregarGold(1000);
            var l=UnityEngine.Object.Instantiate(fonte.gameObject,canvasGo.transform,false).GetComponent<LojaUIManager>();
            var origemDialogo=Resources.FindObjectsOfTypeAll<DialogoUIManager>().Single(x=>x.gameObject.scene.name=="City");
            var dialogo=UnityEngine.Object.Instantiate(origemDialogo.gameObject,canvasGo.transform,false).GetComponent<DialogoUIManager>();singletonDialogo.SetValue(null,dialogo);
            foreach(var tr in canvasGo.GetComponentsInChildren<Transform>(true))tr.gameObject.layer=30;
            void MensagemLocal(string trecho){Exigir(l.painel.activeSelf&&!dialogo.EstaAberto&&l.textoMensagem!=null&&l.textoMensagem.text.Contains(trecho),"Retorno não ficou dentro da loja: "+trecho);}
            int fechamentos=0;estado.RegistrarMenuAberto();l.AoFechar=()=>{fechamentos++;estado.RegistrarMenuFechado();};
            l.Mostrar();
            var botoes=l.painelListaDeItens.GetComponentsInChildren<Button>();
            Exigir(botoes.Length==CatalogoDeItens.ItensDaLoja.Count(),"Produtos não correspondem ao catálogo.");
            botoes.Last().onClick.Invoke();var item=l.ItemSelecionado;
            Exigir(item!=null&&estado.Gold==1000&&estado.Inventario.Count==0,"Selecionar executou operação.");
            l.botaoAcao.onClick.Invoke();
            Exigir(estado.Gold==1000-item.Preco&&estado.ObterQuantidadeDeItem(item.Id)==1,"Compra não atualizou saldo/estoque uma vez.");
            Exigir(l.painel.activeSelf&&!dialogo.EstaAberto&&l.textoMensagem!=null&&l.textoMensagem.text.Contains("Comprou"),"Compra precisa confirmar dentro da loja, sem abrir diálogo.");
            Exigir(fechamentos==0&&l.botaoAcao.interactable,"Confirmação bloqueou/fechou a loja.");
            l.botaoAcao.onClick.Invoke();MensagemLocal("Comprou");
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==2&&estado.Gold==1000-2*item.Preco,"Compra repetida não preservou quantidade/saldo.");
            estado.CarregarGold(0);l.botaoAcao.onClick.Invoke();
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==2&&estado.Gold==0,"Saldo insuficiente alterou estoque.");MensagemLocal("insuficiente");
            l.botaoAcao.onClick.Invoke();MensagemLocal("insuficiente");
            l.botaoAbaVender.onClick.Invoke();l.painelListaDeItens.GetComponentsInChildren<Button>().Single().onClick.Invoke();
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==2,"Selecionar venda removeu item.");l.botaoAcao.onClick.Invoke();MensagemLocal("Vendeu");
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==1&&estado.Gold==item.Preco,"Venda mudou a regra econômica.");
            l.botaoAcao.onClick.Invoke();MensagemLocal("Vendeu");
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==0&&estado.Gold==2*item.Preco,"Venda repetida não atualizou quantidade/saldo.");
            Exigir(!l.botaoAcao.interactable&&l.apresentacao.vazio.gameObject.activeSelf,"Venda do último item não mostrou estado vazio.");
            // A loja não deve tocar na conversa nem em seu callback.
            estado.CarregarGold(0);l.botaoAbaComprar.onClick.Invoke();l.painelListaDeItens.GetComponentsInChildren<Button>().Last().onClick.Invoke();
            bool callbackOriginal=false;dialogo.Mostrar(new[]{new FalaDeDialogo{texto="Conversa existente",falante=""}},()=>callbackOriginal=true);
            l.botaoAcao.onClick.Invoke();Exigir(dialogo.textoFala.text=="Conversa existente","Loja sobrescreveu diálogo ocupado.");
            Exigir(l.painel.activeSelf&&l.textoMensagem.text.Contains("insuficiente"),"Loja dependeu do diálogo ocupado para informar.");dialogo.Avancar();
            Exigir(callbackOriginal,"Callback do diálogo existente se perdeu.");
            estado.CarregarGold(350);l.botaoAbaComprar.onClick.Invoke();l.painelListaDeItens.GetComponentsInChildren<Button>().Last().onClick.Invoke();
            l.botaoAcao.onClick.Invoke();MensagemLocal("Comprou");
            var a=l.apresentacao;Canvas.ForceUpdateCanvases();a.AtualizarEscala();Canvas.ForceUpdateCanvases();
            foreach(var t in canvasGo.GetComponentsInChildren<TextMeshProUGUI>())t.ForceMeshUpdate();Canvas.ForceUpdateCanvases();
            var lista=l.painelListaDeItens.GetComponentInParent<ScrollRect>();lista.Rebuild(CanvasUpdate.PostLayout);
            Exigir(lista.content.rect.height>lista.viewport.rect.height,"Lista longa sem rolagem.");lista.verticalNormalizedPosition=1;var topo=lista.content.anchoredPosition;lista.verticalNormalizedPosition=0;
            Exigir(Vector2.Distance(topo,lista.content.anchoredPosition)>1,"Rolagem não alcançou produtos finais.");lista.verticalNormalizedPosition=1;
            var detalhes=a.descricao.GetComponentInParent<ScrollRect>();detalhes.Rebuild(CanvasUpdate.PostLayout);
            Exigir(detalhes.content.rect.height>detalhes.viewport.rect.height,"Descrição longa não criou rolagem.");
            detalhes.verticalNormalizedPosition=1;var inicioDescricao=detalhes.content.anchoredPosition;detalhes.verticalNormalizedPosition=0;
            Exigir(Vector2.Distance(inicioDescricao,detalhes.content.anchoredPosition)>1,"Descrição longa não alcançou o final.");detalhes.verticalNormalizedPosition=1;
            var cantos=new Vector3[4];
            foreach(var r in new[]{(RectTransform)l.botaoAcao.transform,(RectTransform)l.botaoVoltar.transform,a.nomeItem.rectTransform,a.descricao.GetComponentInParent<ScrollRect>().GetComponent<RectTransform>(),lista.GetComponent<RectTransform>()})
            {r.GetWorldCorners(cantos);foreach(var canto in cantos){var p=cam.WorldToScreenPoint(canto);Exigir(p.x>=-1&&p.y>=-1&&p.x<=largura+1&&p.y<=altura+1,"Controle fora da tela: "+r.name);}}
            if(altura<=500)Exigir(a.descricao.fontSize*Mathf.Abs(a.descricao.transform.lossyScale.y)>=15,"Descrição pequena na janela de referência.");
            Exigir(-a.estoque.rectTransform.anchoredPosition.y+a.estoque.rectTransform.rect.height<=825,"Estoque invade moldura inferior dos detalhes.");
            Exigir(!l.textoMensagem.isTextTruncated&&l.textoMensagem.textInfo.lineCount<=2,"Confirmação local cortada.");
            var mensagemRect=l.textoMensagem.rectTransform;
            Exigir(mensagemRect.anchoredPosition.x+mensagemRect.rect.width<((RectTransform)l.botaoVoltar.transform).anchoredPosition.x,"Mensagem sobrepõe Voltar.");
            cam.Render();
            var eventosGo=new GameObject("EventosTesteLoja",typeof(EventSystem));UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(eventosGo,cena);
            foreach(var b in new[]{l.botaoAcao,l.botaoAbaComprar,l.botaoAbaVender,l.botaoVoltar,l.painelListaDeItens.GetComponentsInChildren<Button>().First()})
            {
                var hits=new System.Collections.Generic.List<RaycastResult>();var r=(RectTransform)b.transform;
                canvasGo.GetComponent<GraphicRaycaster>().Raycast(new PointerEventData(eventosGo.GetComponent<EventSystem>()){position=cam.WorldToScreenPoint(r.TransformPoint(r.rect.center))},hits);
                Exigir(hits.Count>0&&hits[0].gameObject.GetComponentInParent<Button>()==b,"Clique bloqueado: "+b.name);
            }
            var rtAnterior=RenderTexture.active;Texture2D imagem=null;
            try {RenderTexture.active=rt;imagem=new Texture2D(largura,altura,TextureFormat.RGB24,false);imagem.ReadPixels(new Rect(0,0,largura,altura),0,0);imagem.Apply();Directory.CreateDirectory("output/loja-ilustrada");File.WriteAllBytes($"output/loja-ilustrada/preview-{largura}x{altura}.png",imagem.EncodeToPNG());}
            finally {RenderTexture.active=rtAnterior;if(imagem!=null)UnityEngine.Object.DestroyImmediate(imagem);}
            l.botaoVoltar.onClick.Invoke();Exigir(fechamentos==1&&!l.painel.activeSelf&&!estado.MenuAberto,"Fechamento deixou contador/callback residual.");
            return $"OK {largura}x{altura}: seleção, compra/venda repetida, insuficiente, feedback local sem diálogo, vazio, conversa preservada, rolagem, clique, enquadramento, callback/contador. Estado isolado; sem saves.";
        }
        finally {if(rt!=null){camCleanup(rt);};EditorSceneManager.ClosePreviewScene(cena);singleton.SetValue(null,anterior);singletonDialogo.SetValue(null,dialogoAnterior);}
    }
    static void camCleanup(RenderTexture rt){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
}
