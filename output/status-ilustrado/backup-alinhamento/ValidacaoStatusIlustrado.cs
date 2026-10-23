using System;
using System.IO;
using System.Reflection;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Prévia isolada e verificações de apresentação. Nunca chama salvamento ou combate.</summary>
[InitializeOnLoad]
public static class ValidacaoStatusIlustrado
{
    const string Pasta = "output/status-ilustrado/";
    static ValidacaoStatusIlustrado()
    {
        if (File.Exists(Pasta + "validar.pendente")) EditorApplication.delayCall += Pendente;
    }
    static void Exigir(bool ok, string erro) { if (!ok) throw new InvalidOperationException(erro); }
    static void Pendente()
    {
        if (!File.Exists(Pasta + "validar.pendente")) return;
        File.Delete(Pasta + "validar.pendente");
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { File.WriteAllText(Pasta + "validacao.txt", "Pendente: Editor em Play. Nenhuma cena/save alterado."); return; }
        try { File.WriteAllText(Pasta + "validacao.txt", Executar(844, 475) + "\n" + Executar(1280, 720)); }
        catch (Exception e) { File.WriteAllText(Pasta + "validacao.txt", e.ToString()); }
    }
    [MenuItem("DroidsCode/Validação/Status ilustrado (sem saves)")]
    public static void Menu() => Debug.Log(Executar(844, 475));
    public static string Executar(int largura, int altura)
    {
        Exigir(!EditorApplication.isPlayingOrWillChangePlaymode, "Exige fora de Play.");
        Exigir(Application.dataPath.Replace('\\', '/') == "C:/Users/silve/DroidsCode/Assets", "Projeto inesperado.");
        var campo = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var estadoAnterior = campo.GetValue(null); var cena = EditorSceneManager.NewPreviewScene();
        Camera camera = null; RenderTexture rt = null;
        try
        {
            var estadoGo = new GameObject("EstadoTesteStatus", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo, cena);
            var estado = estadoGo.GetComponent<GerenciadorDeEstado>(); campo.SetValue(null, estado);
            var droid = new Droid("Heroi", 100); droid.Hp = 50;
            for (int n = 0; n < 12; n++) droid.TecnicasConfiguradas["Técnica de teste " + n] = new TecnicaComposta();
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(estado, droid);
            var cgo = new GameObject("CameraTesteStatus", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cgo, cena);
            camera = cgo.GetComponent<Camera>(); camera.scene = cena; camera.orthographic = true;
            camera.orthographicSize = altura / 2f; camera.aspect = (float)largura / altura;
            camera.transform.position = new Vector3(0, 0, -10); camera.cullingMask = 1 << 30;
            rt = new RenderTexture(largura, altura, 24); camera.targetTexture = rt;
            var raiz = new GameObject("CanvasTesteStatus", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(raiz, cena);
            var canvas = raiz.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera; canvas.planeDistance = 10;
            var painel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PainelStatus.prefab"), raiz.transform);
            var controlador = painel.GetComponent<TelaDeStatusManager>();
            Exigir(controlador.arteStatus != null && controlador.retratoApollo != null, "Arte/retrato não vinculados.");
            painel.transform.localScale = new Vector3(4, 4, 1);
            controlador.Mostrar();
            var a = painel.GetComponentInChildren<ApresentacaoStatusIlustrado>();
            Exigir(a != null && Mathf.Abs(a.FracaoVida - .5f) < .001f, "Vida não representa HP real.");
            Exigir(a.EstadoEnergia.Contains("100/100") && a.FracaoEnergia == 1, "Energia não representa saldo real.");
            droid.RestaurarEnergia(25, 100); a.Atualizar(droid);
            Exigir(Mathf.Abs(a.FracaoEnergia - .25f) < .001f && a.EstadoEnergia.Contains("25/100"), "Energia parcial incorreta.");
            droid.Hp = 0; a.Atualizar(droid); Exigir(a.FracaoVida == 0, "Vida zero não esvaziou.");
            droid.Hp = 100; a.Atualizar(droid); Exigir(a.FracaoVida == 1, "Vida cheia não preencheu.");
            droid.Hp = 50; a.Atualizar(droid);
            for (int n = 0; n < 6; n++)
                Exigir(a.transform.Find("Atributo" + n) != null, "Atributo perdido.");
            var rolagem = a.transform.Find("ListaTecnicas").GetComponent<ScrollRect>();
            Exigir(rolagem.content.rect.height > rolagem.viewport.rect.height, "Lista longa sem rolagem.");
            foreach (var tr in raiz.GetComponentsInChildren<Transform>(true)) tr.gameObject.layer = 30;
            Canvas.ForceUpdateCanvases(); a.AtualizarEscala(); Canvas.ForceUpdateCanvases();
            foreach (var t in a.GetComponentsInChildren<TextMeshProUGUI>()) t.ForceMeshUpdate();
            var cantos = new Vector3[4]; ((RectTransform)a.transform).GetWorldCorners(cantos);
            foreach (var canto in cantos)
            {
                var p = camera.WorldToScreenPoint(canto);
                Exigir(p.x >= -1 && p.y >= -1 && p.x <= largura + 1 && p.y <= altura + 1, "Painel fora da tela.");
            }
            var eventos = new GameObject("EventosTesteStatus", typeof(EventSystem));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(eventos, cena);
            var voltar = a.transform.Find("Voltar").GetComponent<Button>();
            var r = (RectTransform)voltar.transform;
            int profundidadeAntes = voltar.GetComponent<Image>().depth;
            camera.Render(); // Estabelece a profundidade dos Graphics antes do primeiro raycast da prévia.
            int profundidadeDepois = voltar.GetComponent<Image>().depth;
            var hits = new System.Collections.Generic.List<RaycastResult>();
            raiz.GetComponent<GraphicRaycaster>().Raycast(new PointerEventData(eventos.GetComponent<EventSystem>())
                { position = camera.WorldToScreenPoint(r.TransformPoint(r.rect.center)) }, hits);
            Exigir(hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == voltar,
                $"Clique Voltar bloqueado: depth {profundidadeAntes}/{profundidadeDepois}, hits {hits.Count}, primeiro {(hits.Count > 0 ? hits[0].gameObject.name : "nenhum")}.");
            camera.Render(); var ativo = RenderTexture.active; Texture2D imagem = null;
            try
            {
                RenderTexture.active = rt; imagem = new Texture2D(largura, altura, TextureFormat.RGB24, false);
                imagem.ReadPixels(new Rect(0, 0, largura, altura), 0, 0); imagem.Apply();
                File.WriteAllBytes($"{Pasta}preview-{largura}x{altura}.png", imagem.EncodeToPNG());
            }
            finally { RenderTexture.active = ativo; if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem); }
            bool fechou = false; controlador.AoFechar += () => fechou = true; voltar.onClick.Invoke();
            Exigir(fechou && !painel.activeSelf, "Retorno não preservado.");
            controlador.Mostrar(); Exigir(painel.GetComponentsInChildren<ApresentacaoStatusIlustrado>(true).Length == 1, "Reabertura duplicou montagem.");
            Exigir(droid.Nome == "Heroi" && droid.Hp == 50, "Apresentação alterou domínio.");
            return $"OK {largura}×{altura}: HP0/50/100%, energia real100/25%, seis atributos, técnicas roláveis, enquadramento com escala herdada4, raycast/retorno/reabertura. Depth antes/depois do primeiro render: {profundidadeAntes}/{profundidadeDepois}. Sem saves. Preview isolada; sem input real.";
        }
        finally
        {
            if (camera != null) camera.targetTexture = null;
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena); campo.SetValue(null, estadoAnterior);
        }
    }
}
