using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Prévia e teste isolados; não carrega/grava saves nem inicia Play.</summary>
public static class ValidacaoTerminalIlustrado
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;

    static void Exigir(bool valor, string erro) { if(!valor) throw new InvalidOperationException(erro); }

    public static string Executar(int largura, int altura)
    {
        Exigir(!EditorApplication.isPlaying, "Teste isolado exige fora de Play.");
        var cena = EditorSceneManager.NewPreviewScene();
        var campoSingleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = campoSingleton.GetValue(null);
        RenderTexture rt = null;
        try
        {
            var camGo = new GameObject("CameraPreviewTerminal", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(camGo, cena);
            var cam = camGo.GetComponent<Camera>(); cam.scene = cena; cam.transform.position = new Vector3(0,0,-10);
            cam.orthographic = true; cam.orthographicSize = altura/2f; cam.aspect = (float)largura/altura;
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.025f,0.03f,0.028f);
            cam.cullingMask = 1<<30;
            rt = new RenderTexture(largura, altura, 24); cam.targetTexture = rt;
            var canvasGo = new GameObject("CanvasPreviewTerminal", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(canvasGo, cena);
            var canvas = canvasGo.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam; canvas.planeDistance = 10;
            var origem = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Canvas.prefab").GetComponentInChildren<TerminalUIManager>(true);
            var painel = UnityEngine.Object.Instantiate(origem.gameObject, canvasGo.transform, false);
            var t = painel.GetComponent<TerminalUIManager>();
            foreach(var tr in canvasGo.GetComponentsInChildren<Transform>(true)) tr.gameObject.layer = 30;
            var estadoGo = new GameObject("EstadoIsoladoTerminal", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo, cena);
            var estado = estadoGo.GetComponent<GerenciadorDeEstado>();
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField",Privado).SetValue(estado,new Droid("Teste UI",30));
            campoSingleton.SetValue(null, estado);
            // O clone contém só a UI: nenhum verificador/quest do mundo assina este evento.
            int execucoes = 0; t.AoExecutarCodigo += () => execucoes++;
            int fechamentos = 0; t.AoFechar += () => fechamentos++;
            var menuGo = new GameObject("MenuIsoladoTerminal", typeof(MenuMundoManager));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(menuGo,cena);
            var menu = menuGo.GetComponent<MenuMundoManager>();
            menu.painelMenu = new GameObject("PainelMenuIsolado"); menu.painelMenu.transform.SetParent(menuGo.transform,false);
            var botaoMenu = new GameObject("BotaoTerminalIsolado",typeof(RectTransform),typeof(Button)); botaoMenu.transform.SetParent(menuGo.transform,false);
            menu.botaoTerminal = botaoMenu.GetComponent<Button>(); menu.terminal = t;
            typeof(MenuMundoManager).GetMethod("Alternar",Privado).Invoke(menu,null);
            menu.botaoTerminal.onClick.Invoke();
            Exigir(t.EstaAberto && !menu.painelMenu.activeSelf && t.textoAjuda.text.Contains("COMANDOS DISPONÍVEIS"), "Abertura pelo menu/ajuda geral falhou.");
            t.campoDeCodigo.text = "";
            t.campoDeCodigo.ProcessEvent(new Event {type=EventType.KeyDown,character='v',keyCode=KeyCode.V});
            Exigir(t.campoDeCodigo.text=="v", "Campo não recebeu evento de digitação.");
            t.botaoFechar.onClick.Invoke();
            Exigir(menu.painelMenu.activeSelf && fechamentos==1, "Fechar não devolveu controle ao menu de origem.");
            menu.FecharParaDialogo();
            Exigir(!estado.MenuAberto, "Menu de origem deixou bloqueio residual.");
            t.DefinirContextoPuzzle(CatalogoDePuzzles.IdAguaDaCasa);
            t.Abrir();
            Exigir(t.EstaAberto && painel.activeSelf, "Abrir não mostrou o painel ilustrado.");
            var scroll = t.scrollRectSaida;
            // Não iniciar coroutine de fim de frame fora de Play; layout/rolagem são conferidos abaixo.
            t.scrollRectSaida = null;
            t.campoDeCodigo.text = "nivel_agua = 0\nwhile nivel_agua < 100 do\n    nivel_agua = nivel_agua + 10\nend";
            t.botaoExecutar.onClick.Invoke();
            Exigir(t.Runner.ObterVariavelNumerica("nivel_agua") == 100, "Botão Executar não enviou o campo real ao Lua.");
            Exigir(execucoes == 1 && t.EstadoVisual == TerminalUIManager.EstadoDoTerminal.Pronto, "Callback/status de sucesso incorreto.");
            t.campoDeCodigo.text = "if then";
            t.botaoExecutar.onClick.Invoke();
            Exigir(t.EstadoVisual == TerminalUIManager.EstadoDoTerminal.Erro && t.textoSaida.text.Contains("ERRO:"), "Erro Lua não chegou à saída/status.");
            var a = painel.GetComponent<ApresentacaoTerminalIlustrado>();
            a.depuracao.isOn = true;
            Exigir(t.depuracaoAtiva, "Checkbox de depuração sem ligação.");
            t.campoDeCodigo.text = "resultado = 6 * 7";
            t.botaoExecutar.onClick.Invoke();
            Exigir(t.Runner.ObterVariavelNumerica("resultado") == 42 && t.textoSaida.text.Contains("[Depuração]"),
                "Depuração/execução finita falhou: resultado="+t.Runner.ObterVariavelNumerica("resultado")+" ativo="+t.depuracaoAtiva+" saída="+t.textoSaida.text);
            t.botaoLimpar.onClick.Invoke();
            Exigir(string.IsNullOrEmpty(t.campoDeCodigo.text), "Limpar não apagou o campo.");
            t.campoDeCodigo.text = "nivel_agua = 0\nwhile nivel_agua < 100 do\n    nivel_agua = nivel_agua + 10\nend";
            a.AtualizarDados();
            Exigir(a.numerosDeLinha.text.StartsWith("1\n2\n3\n4\n"), "Numeração não acompanha o código.");
            t.scrollRectSaida = scroll;
            Canvas.ForceUpdateCanvases();
            a.AtualizarEscala(); Canvas.ForceUpdateCanvases();
            foreach(var texto in painel.GetComponentsInChildren<TMPro.TextMeshProUGUI>()) texto.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
            scroll.Rebuild(CanvasUpdate.PostLayout);
            var objetivoRolavel=t.textoAjuda.GetComponentInParent<ScrollRect>();
            objetivoRolavel.Rebuild(CanvasUpdate.PostLayout);
            scroll.verticalNormalizedPosition = 1;
            var posicaoTopo = scroll.content.anchoredPosition;
            scroll.verticalNormalizedPosition = 0;
            scroll.Rebuild(CanvasUpdate.PostLayout);
            Exigir(Vector2.Distance(posicaoTopo, scroll.content.anchoredPosition)>1, "Saída não se move entre topo/fim.");
            Exigir(scroll.verticalScrollbar.size>0 && scroll.verticalScrollbar.size<1, "Indicador de rolagem não representa conteúdo longo.");
            var cantos = new Vector3[4]; ((RectTransform)painel.transform).GetWorldCorners(cantos);
            var minimo = cam.WorldToScreenPoint(cantos[0]); var maximo = cam.WorldToScreenPoint(cantos[2]);
            var areaTerminal = (RectTransform)painel.transform;
            var janelaMin = cam.WorldToScreenPoint(areaTerminal.TransformPoint(new Vector3(areaTerminal.rect.xMin,areaTerminal.rect.yMin+a.referencia.y*a.corteInferior,0f)));
            var janelaMax = cam.WorldToScreenPoint(areaTerminal.TransformPoint(new Vector3(areaTerminal.rect.xMax,areaTerminal.rect.yMax-a.referencia.y*a.corteSuperior,0f)));
            Exigir(janelaMin.x >= -1 && janelaMin.y >= -1 && janelaMax.x <= largura+1 && janelaMax.y <= altura+1, "Janela útil do terminal cortada.");
            foreach(var controle in new RectTransform[]{t.campoDeCodigo.GetComponent<RectTransform>(),t.textoAjuda.GetComponentInParent<ScrollRect>().GetComponent<RectTransform>(),t.scrollRectSaida.GetComponent<RectTransform>(),t.botaoLimpar.GetComponent<RectTransform>(),t.botaoExecutar.GetComponent<RectTransform>(),t.botaoFechar.GetComponent<RectTransform>(),a.depuracao.GetComponent<RectTransform>()}) {
                controle.GetWorldCorners(cantos);
                foreach(var c in cantos) {var pixel=cam.WorldToScreenPoint(c);Exigir(pixel.x>=-1&&pixel.y>=-1&&pixel.x<=largura+1&&pixel.y<=altura+1,"Controle fora da tela: "+controle.name);}
            }
            Exigir(Mathf.Abs((maximo.x-minimo.x)/(maximo.y-minimo.y)-a.referencia.x/a.referencia.y) < 0.01f, "Arte deformada.");
            if(altura<=500) {
                Exigir(t.campoDeCodigo.textComponent.fontSize*Mathf.Abs(t.campoDeCodigo.transform.lossyScale.y)>=16f,"Texto de código pequeno na janela de referência.");
                Exigir(t.textoSaida.fontSize*Mathf.Abs(t.textoSaida.transform.lossyScale.y)>=14f && t.textoAjuda.fontSize*Mathf.Abs(t.textoAjuda.transform.lossyScale.y)>=14f,"Texto lateral pequeno na janela de referência.");
            }
            Exigir(scroll.content.rect.height > scroll.viewport.rect.height, "Saída longa não criou conteúdo rolável.");
            var eventGo = new GameObject("EventosPreviewTerminal",typeof(EventSystem));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(eventGo,cena);
            var eventos = eventGo.GetComponent<EventSystem>();
            // Depth fica -1 até o Canvas da cena de preview ser renderizado.
            cam.Render();
            var anteriorRt = RenderTexture.active;
            Texture2D imagem = null;
            try {
                RenderTexture.active = rt; imagem = new Texture2D(largura,altura,TextureFormat.RGB24,false);
                imagem.ReadPixels(new Rect(0,0,largura,altura),0,0); imagem.Apply();
                Directory.CreateDirectory("output/terminal-ilustrado");
                File.WriteAllBytes("output/terminal-ilustrado/preview-"+largura+"x"+altura+".png",imagem.EncodeToPNG());
            } finally {RenderTexture.active=anteriorRt;if(imagem!=null)UnityEngine.Object.DestroyImmediate(imagem);}
            foreach(var b in new[]{t.botaoLimpar,t.botaoExecutar,t.botaoFechar}) {
                var hits = new System.Collections.Generic.List<RaycastResult>();
                var pointer = new PointerEventData(eventos) { position = cam.WorldToScreenPoint(((RectTransform)b.transform).TransformPoint(((RectTransform)b.transform).rect.center)) };
                canvasGo.GetComponent<GraphicRaycaster>().Raycast(pointer,hits);
                Exigir(hits.Count>0 && hits[0].gameObject.GetComponentInParent<Button>() == b,
                    "Região clicável não alcança "+b.name+" posição="+pointer.position+" hits="+string.Join(",",hits.Select(x=>x.gameObject.name))+" depth="+b.GetComponent<Image>().depth+" cull="+b.GetComponent<Image>().canvasRenderer.cull);
            }
            var hitsCampo = new System.Collections.Generic.List<RaycastResult>();
            var rectCampo = (RectTransform)t.campoDeCodigo.transform;
            canvasGo.GetComponent<GraphicRaycaster>().Raycast(new PointerEventData(eventos) { position=cam.WorldToScreenPoint(rectCampo.TransformPoint(rectCampo.rect.center)) },hitsCampo);
            Exigir(hitsCampo.Count>0 && hitsCampo[0].gameObject.GetComponentInParent<TMPro.TMP_InputField>()==t.campoDeCodigo, "Arte bloqueou o clique no campo de código.");
            t.botaoFechar.onClick.Invoke();
            Exigir(!t.EstaAberto && !painel.activeSelf && fechamentos == 2 && !estado.MenuAberto, "Fechar não liberou painel/contador/callback.");
            return "OK "+largura+"x"+altura+": abrir pelo menu/contexto puzzle, código/Lua finito, sucesso/erro, depuração, limpar, numeração, saída rolável, regiões clicáveis, fechar e enquadramento. Sem saves.";
        }
        finally
        {
            campoSingleton.SetValue(null,anterior);
            if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
            EditorSceneManager.ClosePreviewScene(cena);
        }
    }
}
