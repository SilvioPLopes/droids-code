using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>Render isolado, fora de Play. Não executa turnos, transições ou saves.</summary>
public static class ValidacaoBattleIlustrada
{
    const string Pasta = "output/battle-ilustrada/2026-10-29/";
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Exigir(bool ok, string mensagem) { if (!ok) throw new InvalidOperationException(mensagem); }
    public static string VerificarMontagem()
    {
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Battle.unity");
        try
        {
            bool montada = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<MonoBehaviour>(true))
                .Any(c => c != null && c.GetType().Name == "ApresentacaoBattleIlustrada");
            if (!montada) throw new InvalidOperationException("Battle ainda não tem apresentação ilustrada.");
            return "OK: apresentação serializada na Battle.";
        }
        finally { EditorSceneManager.ClosePreviewScene(cena); }
    }

    public static string Capturar(int largura, int altura, string nome)
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Exige fora de Play.");
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Battle.unity");
        RenderTexture rt = null;
        try
        {
            var raizes = cena.GetRootGameObjects();
            var batalha = raizes.SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
            batalha.telaDeStatus.painel.SetActive(false); // Estado inicial de Start, sem mostrar placeholders da ficha.
            var cam = raizes.SelectMany(g => g.GetComponentsInChildren<Camera>(true)).First();
            cam.scene = cena; cam.enabled = false; cam.orthographic = true;
            cam.orthographicSize = altura / 2f; cam.aspect = (float)largura / altura;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(.025f, .03f, .025f);
            rt = new RenderTexture(largura, altura, 24); cam.targetTexture = rt;
            foreach (var canvas in raizes.SelectMany(g => g.GetComponentsInChildren<Canvas>(true)).Where(c => c.isRootCanvas))
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = cam; canvas.planeDistance = 10;
            }
            Canvas.ForceUpdateCanvases();
            foreach (var c in raizes.SelectMany(g => g.GetComponentsInChildren<MonoBehaviour>(true)))
                if (c != null && c.GetType().Name == "ApresentacaoBattleIlustrada")
                    c.GetType().GetMethod("AtualizarLayout").Invoke(c, null);
            Canvas.ForceUpdateCanvases(); cam.Render();
            Salvar(rt, Pasta + nome + $"-{largura}x{altura}.png");
            return "Render salvo: " + Pasta + nome + $"-{largura}x{altura}.png";
        }
        finally { if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); } EditorSceneManager.ClosePreviewScene(cena); }
    }

    public static string Executar(int largura, int altura)
    {
        Exigir(!EditorApplication.isPlaying, "Exige fora de Play.");
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = singleton.GetValue(null);
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Battle.unity");
        RenderTexture rt = null;
        try
        {
            var raizes = cena.GetRootGameObjects();
            var b = raizes.SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
            var a = raizes.SelectMany(g => g.GetComponentsInChildren<ApresentacaoBattleIlustrada>(true)).Single();
            var bot = new[] { b.botaoAtacar, b.botaoItem, b.botaoStatus, b.botaoFugir };
            Exigir(bot.Distinct().Count() == 4 && a.batalha == b, "Referências perdidas.");
            var estadoGo = new GameObject("EstadoFixtureBattle", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo, cena);
            var estado = estadoGo.GetComponent<GerenciadorDeEstado>();
            var d = new Droid("Heroi", 30); d.Hp = 23; d.RestaurarEnergia(25, 100);
            for (int i = 0; i < 16; i++)
            {
                string nome = "Técnica de sobrecarga com nome comprido " + i;
                d.TecnicasConfiguradas[nome] = new TecnicaComposta { Nome = nome, NivelDeDano = 1 + i % 4 };
            }
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", Privado).SetValue(estado, d);
            singleton.SetValue(null, estado);
            estado.AdicionarItem("pocao_pequena", 3); estado.AdicionarItem("nucleo_de_energia", 2);
            estado.AdicionarItem("disruptor_emp", 1);
            typeof(BattleManager).GetMethod("Start", Privado).Invoke(b, null);
            var cam = raizes.SelectMany(g => g.GetComponentsInChildren<Camera>(true)).First();
            cam.scene = cena; cam.enabled = false; cam.orthographic = true; cam.orthographicSize = altura / 2f;
            cam.aspect = (float)largura / altura; cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(.025f, .03f, .025f);
            rt = new RenderTexture(largura, altura, 24); cam.targetTexture = rt;
            var canvas = b.botaoAtacar.GetComponentInParent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = cam; canvas.planeDistance = 10;
            var scaler = canvas.GetComponent<CanvasScaler>();
            Exigir(scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize && Mathf.Approximately(scaler.scaleFactor, 3.3f), "Canvas/Scaler alterado.");
            Canvas.ForceUpdateCanvases(); a.AtualizarLayout(); Canvas.ForceUpdateCanvases();
            b.sliderInimigo.value = 12f / b.hpMaxInimigo;
            a.AtualizarHud(d, b.nomeInimigo, 12, b.hpMaxInimigo);
            Exigir(a.nomePlayer.text == "APOLLO" && a.textoEnergia.text.Contains("25/100") &&
                Mathf.Approximately(a.energia.rectTransform.anchorMax.x, .25f), "HUD não representa dados reais.");
            // Mensagens extensas precisam continuar acessíveis sem reduzir a fonte.
            b.textoMensagem.text = string.Join("\n", Enumerable.Range(1, 4).Select(i =>
                "Mensagem " + i + ": Apollo utilizou uma técnica de sobrecarga; o inimigo resistiu ao efeito, mas recebeu o dano calculado. A batalha continua."));
            a.AtualizarConteudo(); Canvas.ForceUpdateCanvases();
            Exigir(a.log.content.rect.height > a.log.viewport.rect.height, "Log extenso não pode rolar.");
            a.log.verticalNormalizedPosition = 1; Exigir(a.log.verticalNormalizedPosition > .99f, "Início do log inacessível.");
            a.log.verticalNormalizedPosition = 0; Exigir(a.log.verticalNormalizedPosition < .01f, "Final do log inacessível.");
            ValidarPegas(a);
            var eventosGo = new GameObject("EventosFixtureBattle", typeof(EventSystem));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(eventosGo, cena);
            var eventos = eventosGo.GetComponent<EventSystem>(); var raycaster = canvas.GetComponent<GraphicRaycaster>();
            cam.Render();
            foreach (var button in bot)
            {
                var t = button.GetComponentInChildren<TextMeshProUGUI>(); t.ForceMeshUpdate();
                Exigir(!t.isTextTruncated && t.preferredWidth <= t.rectTransform.rect.width + 1, "Comando cortado: " + t.text);
                Exigir(t.fontSize * Mathf.Abs(t.transform.lossyScale.y) >= 20, "Comando pequeno: " + t.text);
                var r = (RectTransform)button.transform; var cantos = new Vector3[4]; r.GetWorldCorners(cantos);
                foreach (var c in cantos)
                {
                    var p = cam.WorldToScreenPoint(c);
                    Exigir(p.x >= 0 && p.y >= 0 && p.x <= largura && p.y <= altura, "Comando fora da tela.");
                }
                var hits = new System.Collections.Generic.List<RaycastResult>();
                raycaster.Raycast(new PointerEventData(eventos) { position = cam.WorldToScreenPoint(r.TransformPoint(r.rect.center)) }, hits);
                Exigir(hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == button, "Clique bloqueado: " + button.name);
            }
            b.botaoAtacar.onClick.Invoke(); a.AtualizarConteudo(); Canvas.ForceUpdateCanvases();
            Exigir(a.quadroAtaques.activeSelf && a.ataques.content.childCount == 18, "Lista perdeu técnicas/Voltar.");
            Exigir(a.ataques.content.rect.height > a.ataques.viewport.rect.height, "Lista extensa não pode rolar.");
            Exigir(a.ataques.content.GetComponentsInChildren<Button>().Any(x => !x.interactable), "Custo de energia não preservado.");
            a.ataques.verticalNormalizedPosition = 0; Canvas.ForceUpdateCanvases(); cam.Render();
            var voltar = a.ataques.content.GetComponentsInChildren<Button>().Last();
            Exigir(voltar.GetComponentInChildren<TextMeshProUGUI>().text == "Voltar", "Voltar perdido.");
            Salvar(rt, Pasta + $"golpes-{largura}x{altura}.png");
            voltar.onClick.Invoke(); a.AtualizarConteudo(); Exigir(!a.quadroAtaques.activeSelf, "Voltar não fechou golpes.");
            b.botaoItem.onClick.Invoke(); a.AtualizarConteudo(); Canvas.ForceUpdateCanvases();
            if (b.bagDeBatalha != null)
            {
                Exigir(b.bagDeBatalha.EstaAberta && b.bagDeBatalha.EmBatalha && !a.quadroAtaques.activeSelf &&
                    !a.quadroItens.activeSelf, "Bag não abriu exclusiva.");
                var bag = b.bagDeBatalha.painel.GetComponentInChildren<ApresentacaoBagIlustrada>();
                bag.transform.Find("Voltar").GetComponent<Button>().onClick.Invoke();
                Exigir(!b.bagDeBatalha.EstaAberta, "Voltar não fechou Bag.");
            }
            else
            {
                Exigir(a.quadroItens.activeSelf && !a.quadroAtaques.activeSelf && a.itens.content.childCount > 1, "Itens não abriram exclusivos.");
                a.itens.content.GetComponentsInChildren<Button>().Last().onClick.Invoke();
            }
            a.AtualizarConteudo();
            Exigir(!a.quadroItens.activeSelf && estado.ObterQuantidadeDeItem("pocao_pequena") == 3, "Voltar consumiu item ou deixou quadro aberto.");
            b.botaoStatus.onClick.Invoke(); Exigir(b.telaDeStatus.painel.activeSelf, "Status não abriu.");
            b.telaDeStatus.botaoVoltar.onClick.Invoke(); Exigir(!b.telaDeStatus.painel.activeSelf, "Status não fechou.");
            b.textoMensagem.text = "Um " + b.nomeInimigo + " apareceu!\nEscolha uma ação.\nAtaque básico recupera energia.\nSua vez, Apollo.";
            a.AtualizarConteudo(); Canvas.ForceUpdateCanvases(); cam.Render();
            Salvar(rt, Pasta + $"depois-{largura}x{altura}.png");
            return $"OK {largura}x{altura}: quatro comandos/enquadramento/raycast; HUD real; log extenso rolável;18 opções de golpes/Voltar/energia; itens/Voltar sem consumo; Status/retorno. Sem turnos, Play, transição ou saves.";
        }
        finally
        {
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena); singleton.SetValue(null, anterior);
        }
    }

    static void ValidarPegas(ApresentacaoBattleIlustrada a)
    {
        foreach (var scroll in new[] { a.log, a.ataques, a.itens })
        {
            var pega = scroll.verticalScrollbar.handleRect;
            var trilha = (RectTransform)pega.parent;
            Exigir(pega.sizeDelta.sqrMagnitude < .01f, "Pega conserva tamanho fixo fora da trilha.");
            if (!pega.gameObject.activeInHierarchy) continue;
            var cantos = new Vector3[4]; pega.GetWorldCorners(cantos);
            foreach (var canto in cantos)
            {
                var p = trilha.InverseTransformPoint(canto);
                Exigir(p.x >= trilha.rect.xMin - .1f && p.x <= trilha.rect.xMax + .1f &&
                    p.y >= trilha.rect.yMin - .1f && p.y <= trilha.rect.yMax + .1f, "Pega fora da trilha.");
            }
        }
    }

    static void Salvar(RenderTexture rt, string caminho)
    {
        var anterior = RenderTexture.active; Texture2D imagem = null;
        try
        {
            RenderTexture.active = rt; imagem = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            imagem.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); imagem.Apply();
            Directory.CreateDirectory(Pasta); File.WriteAllBytes(caminho, imagem.EncodeToPNG());
        }
        finally { RenderTexture.active = anterior; if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem); }
    }
}
