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
    static T Unico<T>(System.Collections.Generic.IEnumerable<T> candidatos,Func<T,bool> criterio,string etapa,Func<T,string> nome)
    {
        var todos=candidatos.ToArray();var encontrados=todos.Where(criterio).ToArray();
        Exigir(encontrados.Length==1,$"Fixture loja [{etapa}]: esperava1 alvo, encontrou {encontrados.Length}. Disponíveis: {string.Join(", ",todos.Select(nome))}");
        return encontrados[0];
    }
    public static string Executar(int largura,int altura)
    {
        Exigir(!EditorApplication.isPlaying,"Exige fora de Play.");
        var singleton=typeof(GerenciadorDeEstado).GetField("instancia",BindingFlags.Static|BindingFlags.NonPublic);
        var singletonDialogo=typeof(DialogoUIManager).GetField("_instancia",BindingFlags.Static|BindingFlags.NonPublic);
        var anterior=singleton.GetValue(null);var dialogoAnterior=singletonDialogo.GetValue(null);
        var fonte=Unico(Resources.FindObjectsOfTypeAll<LojaUIManager>(),x=>x.gameObject.scene.name=="City","fonte LojaUIManager na City",x=>x.name+"@"+x.gameObject.scene.name);
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
            var origemDialogo=Unico(Resources.FindObjectsOfTypeAll<DialogoUIManager>(),x=>x.gameObject.scene.name=="City","fonte DialogoUIManager na City",x=>x.name+"@"+x.gameObject.scene.name);
            var dialogo=UnityEngine.Object.Instantiate(origemDialogo.gameObject,canvasGo.transform,false).GetComponent<DialogoUIManager>();singletonDialogo.SetValue(null,dialogo);
            Exigir(dialogo.painel!=null&&dialogo.painel.transform.IsChildOf(canvasGo.transform)&&dialogo.textoFala!=null&&dialogo.textoFala.transform.IsChildOf(canvasGo.transform),"Referências do diálogo clonado escapam do Canvas de preview.");
            // Em Edit Mode Start não roda; inicializar o clone oculta o painel serializado antes de qualquer captura.
            typeof(DialogoUIManager).GetMethod("Inicializar",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(dialogo,null);
            foreach(var tr in canvasGo.GetComponentsInChildren<Transform>(true))tr.gameObject.layer=30;
            void MensagemLocal(string trecho){Exigir(l.painel.activeSelf&&!dialogo.EstaAberto&&l.textoMensagem!=null&&l.textoMensagem.text.Contains(trecho),"Retorno não ficou dentro da loja: "+trecho);}
            int fechamentos=0;estado.RegistrarMenuAberto();l.AoFechar=()=>{fechamentos++;estado.RegistrarMenuFechado();};
            l.Mostrar();
            var botoes=l.painelListaDeItens.GetComponentsInChildren<Button>();
            Exigir(botoes.Length==CatalogoDeItens.ItensDaLoja.Count(),"Produtos não correspondem ao catálogo.");
            OrganizacaoVisualDeItens.Validar();
            foreach(var produto in CatalogoDeItens.ItensDaLoja)
            {
                var botao=Unico(botoes,x=>x.name=="Produto_"+produto.Id,"produto de compra "+produto.Id,x=>x.name);
                Exigir(botao.transform.Find("Icone").GetComponent<Image>().sprite==CatalogoVisualDeItens.Obter(produto.Id),"Ícone incorreto na lista: "+produto.Id);
            }
            botoes.Last().onClick.Invoke();var item=l.ItemSelecionado;
            Exigir(l.apresentacao.iconeItem.sprite==CatalogoVisualDeItens.Obter(item.Id),"Detalhe e lista usam sprites diferentes.");
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
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==1&&estado.Gold==item.PrecoDeVendaReal,"Venda mudou a regra econômica.");
            l.botaoAcao.onClick.Invoke();MensagemLocal("Vendeu");
            Exigir(estado.ObterQuantidadeDeItem(item.Id)==0&&estado.Gold==2*item.PrecoDeVendaReal,"Venda repetida não atualizou quantidade/saldo.");
            Exigir(!l.botaoAcao.interactable&&l.apresentacao.vazio.gameObject.activeSelf,"Venda do último item não mostrou estado vazio.");
            // Regressão: a confirmação não pode consumir nem pagar um item protegido.
            estado.CarregarInventario(new System.Collections.Generic.Dictionary<string,int>{{CatalogoDeItens.IdChaveDeAcesso,1}});
            estado.CarregarGold(73);l.botaoAbaVender.onClick.Invoke();
            var protegido=Unico(l.painelListaDeItens.GetComponentsInChildren<Button>(),x=>x.name=="Produto_"+CatalogoDeItens.IdChaveDeAcesso,"protegido visível "+CatalogoDeItens.IdChaveDeAcesso,x=>x.name);
            Exigir(protegido.gameObject.activeInHierarchy&&protegido.interactable,"Item protegido não ficou visível/selecionável.");
            protegido.onClick.Invoke();
            Exigir(l.ItemSelecionado.Id==CatalogoDeItens.IdChaveDeAcesso&&l.botaoAcao.interactable,"Item protegido não pôde ser selecionado para tentativa.");
            l.botaoAcao.onClick.Invoke();
            Exigir(estado.ObterQuantidadeDeItem(CatalogoDeItens.IdChaveDeAcesso)==1&&estado.Gold==73,"Venda protegida consumiu item ou alterou Gold.");
            Exigir(l.painel.activeSelf&&l.Vendendo&&l.ItemSelecionado.Id==CatalogoDeItens.IdChaveDeAcesso&&!dialogo.EstaAberto,"Recusa mudou seleção/aba ou abriu diálogo.");
            var falas=l.apresentacao.GetComponentsInChildren<TextMeshProUGUI>();
            Exigir(falas.Any(x=>x.text=="Lojista")&&falas.Any(x=>x.text=="Não posso comprar esse item."),"Recusa não mostrou fala local exata da Lojista.");
            var balao=l.apresentacao.transform.Find("RecusaVenda");
            Exigir(balao!=null&&balao.gameObject.activeSelf&&!l.textoMensagem.gameObject.activeSelf,"Balão não substituiu apenas o rodapé.");
            var instanciaBalao=balao;
            var scrollVenda=l.painelListaDeItens.GetComponentInParent<ScrollRect>();
            var posicaoVenda=scrollVenda.content.anchoredPosition;
            l.botaoAcao.onClick.Invoke();
            Exigir(l.apresentacao.transform.Find("RecusaVenda")==instanciaBalao&&l.apresentacao.transform.Cast<Transform>().Count(x=>x.name=="RecusaVenda")==1,"Recusa repetida duplicou balão.");
            Exigir(estado.ObterQuantidadeDeItem(CatalogoDeItens.IdChaveDeAcesso)==1&&estado.Gold==73&&scrollVenda.content.anchoredPosition==posicaoVenda,"Recusa repetida alterou estoque/Gold/rolagem.");
            Exigir(balao.GetComponentsInChildren<Graphic>(true).All(x=>!x.raycastTarget),"Balão decorativo intercepta cliques.");
            foreach(var id in new[]{CatalogoDeItens.IdFioDeCobre,CatalogoDeItens.IdNucleoDeSobrecarga,CatalogoDeItens.IdModuloDeRessonancia})
            {
                estado.CarregarInventario(new System.Collections.Generic.Dictionary<string,int>{{id,1}});
                l.botaoAbaVender.onClick.Invoke();
                Exigir(!balao.gameObject.activeSelf&&l.textoMensagem.gameObject.activeSelf,"Troca de aba não limpou recusa.");
                l.painelListaDeItens.GetComponentsInChildren<Button>().Single().onClick.Invoke();
                l.botaoAcao.onClick.Invoke();
                Exigir(estado.ObterQuantidadeDeItem(id)==1&&estado.Gold==73&&l.ItemSelecionado.Id==id&&balao.gameObject.activeSelf,"Proteção falhou para "+id);
            }
            Canvas.ForceUpdateCanvases();l.apresentacao.AtualizarEscala();Canvas.ForceUpdateCanvases();
            foreach(var t in balao.GetComponentsInChildren<TextMeshProUGUI>()){t.ForceMeshUpdate();Exigir(!t.isTextTruncated,"Fala de recusa cortada: "+t.name);}
            Exigir(!dialogo.EstaAberto&&!dialogo.painel.activeInHierarchy,"Captura de recusa contaminada por painel de conversa.");
            DiagnosticarBalao(balao.GetComponent<BalaoDaLoja>(),largura,altura);
            Exportar(cam,rt,$"output/loja-ilustrada/recusa-{largura}x{altura}.png");
            // Uma seleção capturada não pode ignorar o catálogo vigente nem pagar estoque já removido.
            estado.CarregarInventario(new System.Collections.Generic.Dictionary<string,int>{{CatalogoDeItens.IdPocaoPequena,1}});
            l.botaoAbaVender.onClick.Invoke();l.painelListaDeItens.GetComponentsInChildren<Button>().Single().onClick.Invoke();
            Exigir(!balao.gameObject.activeSelf,"Mudar seleção não limpou recusa.");
            estado.TentarRemoverItem(CatalogoDeItens.IdPocaoPequena);
            l.botaoAcao.onClick.Invoke();MensagemLocal("não tem mais");
            Exigir(estado.Gold==73&&!l.botaoAcao.interactable&&l.ItemSelecionado==null,"Estoque alterado pagou Gold ou preservou seleção inválida.");
            l.botaoAcao.onClick.Invoke();Exigir(estado.Gold==73,"Confirmação sem estoque pagou novamente.");
            var campoSelecao=typeof(LojaUIManager).GetField("selecionado",BindingFlags.Instance|BindingFlags.NonPublic);
            campoSelecao.SetValue(l,new DefinicaoDeItem{Id="id_invalido_fixture",Nome="Inválido",Preco=9999});
            l.ConfirmarSelecao();Exigir(estado.Gold==73&&estado.Inventario.Count==0&&balao.gameObject.activeSelf,"Definição inválida não foi recusada com segurança.");
            estado.AdicionarItem(CatalogoDeItens.IdPocaoPequena,1);
            campoSelecao.SetValue(l,new DefinicaoDeItem{Id=CatalogoDeItens.IdPocaoPequena,Nome="Seleção antiga",Preco=9999});
            l.ConfirmarSelecao();
            Exigir(estado.Gold==73+CatalogoDeItens.Obter(CatalogoDeItens.IdPocaoPequena).Preco&&estado.Inventario.Count==0,"Venda usou definição/preço capturado em vez do catálogo vigente.");
            estado.CarregarInventario(new System.Collections.Generic.Dictionary<string,int>());
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
            // Também preservar conversa ocupada durante recusa protegida, sem alterar contador.
            estado.AdicionarItem(CatalogoDeItens.IdChaveDeAcesso,1);l.botaoAbaVender.onClick.Invoke();
            Unico(l.painelListaDeItens.GetComponentsInChildren<Button>(),x=>x.name=="Produto_"+CatalogoDeItens.IdChaveDeAcesso,"protegido durante conversa ocupada",x=>x.name).onClick.Invoke();
            bool callbackRecusa=false;Exigir(dialogo.Mostrar(new[]{new FalaDeDialogo{texto="Conversa durante recusa",falante=""}},()=>callbackRecusa=true),"Fixture não abriu conversa antes da recusa.");
            l.botaoAcao.onClick.Invoke();Exigir(dialogo.textoFala.text=="Conversa durante recusa"&&fechamentos==0,"Recusa alterou conversa ou fechou loja.");
            // Avancar percorre páginas TMP antes de concluir a fala e invocar seu callback.
            int paginasRecusa=Mathf.Max(1,dialogo.textoFala.textInfo.pageCount);
            for(int pagina=0;pagina<paginasRecusa;pagina++)dialogo.Avancar();
            Exigir(callbackRecusa&&!dialogo.EstaAberto,$"Recusa perdeu callback da conversa existente após {paginasRecusa} avanços; página {dialogo.textoFala.pageToDisplay}/{dialogo.textoFala.textInfo.pageCount}.");
            l.botaoVoltar.onClick.Invoke();Exigir(fechamentos==1&&!l.painel.activeSelf&&!estado.MenuAberto&&!balao.gameObject.activeSelf,"Fechamento deixou contador/callback/balão residual.");
            estado.RegistrarMenuAberto();l.Mostrar();Exigir(!balao.gameObject.activeSelf&&l.textoMensagem.gameObject.activeSelf,"Reabrir loja manteve recusa.");l.Fechar();
            return $"OK {largura}x{altura}: seleção, compra/venda repetida, insuficiente, feedback local sem diálogo, vazio, conversa preservada ({paginasRecusa} páginas na recusa), rolagem, clique, enquadramento, callback/contador. Estado isolado; sem saves.";
        }
        finally {if(rt!=null){camCleanup(rt);};EditorSceneManager.ClosePreviewScene(cena);singleton.SetValue(null,anterior);singletonDialogo.SetValue(null,dialogoAnterior);}
    }
    static void DiagnosticarBalao(BalaoDaLoja balao,int largura,int altura)
    {
        var renderer=balao.canvasRenderer;var malha=renderer.GetMesh();
        var geracaoLegacy=typeof(Graphic).GetProperty("useLegacyMeshGeneration",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(balao);
        string malhaDireta;
        using(var vertices=new VertexHelper())
        {
            typeof(BalaoDaLoja).GetMethod("OnPopulateMesh",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(VertexHelper)},null).Invoke(balao,new object[]{vertices});
            malhaDireta=$"vertices={vertices.currentVertCount}, indices={vertices.currentIndexCount}";
        }
        var cores=malha!=null?malha.colors32:new Color32[0];
        string material=renderer.materialCount>0&&renderer.GetMaterial(0)!=null?renderer.GetMaterial(0).name:"ausente";
        var diagnostico=new[]{
            $"activeSelf={balao.gameObject.activeSelf}; activeHierarchy={balao.gameObject.activeInHierarchy}; enabled={balao.enabled}; isActive={balao.IsActive()}; layer={balao.gameObject.layer}",
            $"canvas={balao.canvas?.name}; registradoCanvas={balao.canvas!=null&&GraphicRegistry.GetGraphicsForCanvas(balao.canvas).Contains(balao)}; graphicDepth={balao.depth}; rendererDepth={renderer.absoluteDepth}; cull={renderer.cull}; alphaRenderer={renderer.GetAlpha()}; corGraphic={balao.color}",
            $"legacy={geracaoLegacy}; materialCount={renderer.materialCount}; material={material}; rect={balao.rectTransform.rect}; escala={balao.transform.lossyScale}",
            $"malhaRenderer: vertices={(malha!=null?malha.vertexCount:0)}; indices={(malha!=null?malha.triangles.Length:0)}; bounds={(malha!=null?malha.bounds.ToString():"ausente")}; alphaMin={(cores.Length>0?cores.Min(x=>x.a):0)}; alphaMax={(cores.Length>0?cores.Max(x=>x.a):0)}",
            $"malhaDiretaOverride: {malhaDireta}; outlineAtivo={balao.GetComponent<Outline>()?.isActiveAndEnabled}"
        };
        Directory.CreateDirectory("output/loja-ilustrada");
        File.WriteAllLines($"output/loja-ilustrada/diagnostico-balao-{largura}x{altura}.txt",diagnostico);
    }
    static void Exportar(Camera cam,RenderTexture rt,string caminho)
    {
        cam.Render();var anterior=RenderTexture.active;Texture2D imagem=null;
        try {RenderTexture.active=rt;imagem=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);imagem.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);imagem.Apply();Directory.CreateDirectory(Path.GetDirectoryName(caminho));File.WriteAllBytes(caminho,imagem.EncodeToPNG());}
        finally {RenderTexture.active=anterior;if(imagem!=null)UnityEngine.Object.DestroyImmediate(imagem);}
    }
    static void camCleanup(RenderTexture rt){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
}
