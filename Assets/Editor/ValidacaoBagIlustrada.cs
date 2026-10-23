using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fixture em cena de prévia, sem uso de itens, saves ou alteração da cena aberta.</summary>
[InitializeOnLoad]
public static class ValidacaoBagIlustrada
{
    const string Pasta = "output/bag-ilustrada/";
    static ValidacaoBagIlustrada()
    {
        if (File.Exists(Pasta + "validar.pendente")) EditorApplication.delayCall += ValidarPendente;
    }
    static void Exigir(bool ok, string erro) { if (!ok) throw new InvalidOperationException(erro); }
    static void ValidarPendente()
    {
        if (!File.Exists(Pasta + "validar.pendente")) return;
        File.Delete(Pasta + "validar.pendente");
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            File.WriteAllText(Pasta + "validacao.txt", "PENDENTE: Editor em Play. Nenhuma cena ou save alterado."); return;
        }
        try { File.WriteAllText(Pasta + "validacao.txt", Executar(844, 475) + "\n" + Executar(1280, 720)); }
        catch (Exception e) { File.WriteAllText(Pasta + "validacao.txt", e.ToString()); }
    }
    [MenuItem("DroidsCode/Validação/Bag ilustrada (sem saves)")]
    public static void Menu() => Debug.Log(Executar(844, 475));
    public static string Executar(int largura, int altura)
    {
        Exigir(!EditorApplication.isPlayingOrWillChangePlaymode, "Exige fora de Play.");
        Exigir(Application.dataPath.Replace('\\', '/').EndsWith("/DroidsCode/Assets"), "Projeto inesperado.");
        var campo = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = campo.GetValue(null); var cena = EditorSceneManager.NewPreviewScene();
        Camera camera = null; RenderTexture rt = null;
        try
        {
            var estadoGo = new GameObject("EstadoTesteBag", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo, cena);
            var estado = estadoGo.GetComponent<GerenciadorDeEstado>(); campo.SetValue(null, estado);
            var elegiveis = CatalogoDeItens.Todos.Values.Where(i => i.UsavelForaDeBatalha).ToList();
            Exigir(elegiveis.Count > 12, "Fixture exige mais de uma página de itens reais.");
            estado.CarregarInventario(elegiveis.ToDictionary(i => i.Id, i => 3));
            var snapshot = estado.Inventario.ToDictionary(kv => kv.Key, kv => kv.Value);
            var cgo = new GameObject("CameraTesteBag", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cgo, cena);
            camera = cgo.GetComponent<Camera>(); camera.scene = cena; camera.orthographic = true;
            camera.orthographicSize = altura / 2f; camera.aspect = (float)largura / altura;
            camera.transform.position = new Vector3(0, 0, -10); camera.cullingMask = 1 << 30;
            rt = new RenderTexture(largura, altura, 24); camera.targetTexture = rt;
            var raiz = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Canvas.prefab"));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(raiz, cena);
            var canvas = raiz.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera; canvas.planeDistance = 10;
            var bag = raiz.GetComponentInChildren<BagUIManager>(true);
            Exigir(bag != null && bag.arteBag != null, "Bag/arte não vinculada no prefab.");
            foreach (Transform filho in raiz.transform) filho.gameObject.SetActive(false);
            bag.Mostrar();
            var a = bag.painel.GetComponentInChildren<ApresentacaoBagIlustrada>();
            Exigir(a != null && bag.painel.activeSelf, "Montagem não abriu.");
            Exigir(a.ItemSelecionado == null, "Abertura selecionou item implicitamente.");
            for (int pagina = 0; pagina < (elegiveis.Count + 11) / 12; pagina++)
            {
                if (pagina > 0) a.MudarPagina(1);
                Exigir(a.Pagina == pagina, "Página inacessível.");
                for (int slot = 0; slot < Math.Min(12, elegiveis.Count - pagina * 12); slot++)
                {
                    a.Selecionar(slot);
                    Exigir(a.ItemSelecionado == elegiveis[pagina * 12 + slot].Id, "Item não acessível por seleção.");
                }
            }
            Exigir(snapshot.All(kv => estado.ObterQuantidadeDeItem(kv.Key) == kv.Value), "Seleção consumiu inventário.");
            a.MudarPagina(-999); a.Selecionar(0);
            foreach (var tr in raiz.GetComponentsInChildren<Transform>(true)) tr.gameObject.layer = 30;
            Canvas.ForceUpdateCanvases(); a.AtualizarEscala(); Canvas.ForceUpdateCanvases();
            foreach (var t in a.GetComponentsInChildren<TextMeshProUGUI>()) t.ForceMeshUpdate();
            var cantos = new Vector3[4]; ((RectTransform)a.transform).GetWorldCorners(cantos);
            foreach (var canto in cantos)
            {
                var p = camera.WorldToScreenPoint(canto);
                Exigir(p.x >= -1 && p.y >= -1 && p.x <= largura + 1 && p.y <= altura + 1, "Arte fora da tela.");
            }
            camera.Render(); var antes = RenderTexture.active;
            Texture2D imagem = null;
            try
            {
                RenderTexture.active = rt; imagem = new Texture2D(largura, altura, TextureFormat.RGB24, false);
                imagem.ReadPixels(new Rect(0, 0, largura, altura), 0, 0); imagem.Apply();
                Directory.CreateDirectory(Pasta); File.WriteAllBytes($"{Pasta}preview-{largura}x{altura}.png", imagem.EncodeToPNG());
            }
            finally { RenderTexture.active = antes; if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem); }
            bool fechou = false; bag.AoFechar += () => fechou = true;
            a.transform.Find("Voltar").GetComponent<Button>().onClick.Invoke();
            Exigir(fechou && !bag.painel.activeSelf, "Voltar não fechou/comunicou retorno.");
            bag.Mostrar(); Exigir(bag.painel.GetComponentsInChildren<ApresentacaoBagIlustrada>(true).Length == 1, "Montagem duplicada ao reabrir.");
            estado.CarregarInventario(new Dictionary<string, int>()); a.Atualizar();
            Exigir(a.Mensagem.text.Contains("vazia") && a.ItemSelecionado == null, "Estado vazio inconsistente.");
            return $"OK {largura}×{altura}: arte vinculada, {elegiveis.Count} itens acessíveis, seleção sem consumo, retorno/reabertura e bag vazia. Sem saves. Canvas local; zoom Game View não envolvido.";
        }
        finally
        {
            if (camera != null) camera.targetTexture = null;
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena); campo.SetValue(null, anterior);
        }
    }
}
