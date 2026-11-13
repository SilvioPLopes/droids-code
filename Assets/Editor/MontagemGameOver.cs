using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Montagem única aditiva. Salva somente GameOver; não fecha nem salva cenas do usuário.</summary>
public static class MontagemGameOver
{
    const string Cena = "Assets/Scenes/GameOver.unity";
    const string Arte = "Assets/UI/GameOverIlustrado/game-over-fundo-v1.png";

    public static string Aplicar()
    {
        if (Application.dataPath.Replace('\\', '/') != "C:/Users/silve/DroidsCode/Assets" || EditorApplication.isPlaying)
            throw new InvalidOperationException("Exige projeto DroidsCode fora de Play.");
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Cena) != null || System.IO.File.Exists(Cena))
            throw new InvalidOperationException("GameOver já existe. Montagem não repetida.");
        var textura = AssetDatabase.LoadAssetAtPath<Texture2D>(Arte);
        if (textura == null) throw new InvalidOperationException("Arte GameOver ausente; importe o PNG antes.");
        var importador = (TextureImporter)AssetImporter.GetAtPath(Arte);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.mipmapEnabled = false;
        importador.filterMode = FilterMode.Point;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
        importador.npotScale = TextureImporterNPOTScale.None;
        importador.maxTextureSize = 2048;
        importador.SaveAndReimport();
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Arte);
        var fonte = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (sprite == null || fonte == null) throw new InvalidOperationException("Arte/fonte ausentes; nenhuma cena criada.");

        var anterior = SceneManager.GetActiveScene();
        var nova = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        try
        {
            SceneManager.SetActiveScene(nova);
            var camera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.black;
            camera.orthographic = true; camera.transform.position = new Vector3(0, 0, -10);
            var canvas = new GameObject("CanvasGameOver", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 100;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            var fundo = new GameObject("Fundo", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            fundo.transform.SetParent(canvas.transform, false); fundo.sprite = sprite; fundo.raycastTarget = false;
            fundo.rectTransform.anchorMin = Vector2.zero; fundo.rectTransform.anchorMax = Vector2.one;
            fundo.rectTransform.offsetMin = fundo.rectTransform.offsetMax = Vector2.zero;

            RectTransform Caixa(string nome, Transform pai, float x, float y, float w, float h)
            {
                var r = new GameObject(nome, typeof(RectTransform)).GetComponent<RectTransform>();
                r.SetParent(pai, false); r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
                r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
            }
            TextMeshProUGUI Texto(string nome, Transform pai, float x, float y, float w, float h, string valor, float tamanho)
            {
                var t = Caixa(nome, pai, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>();
                t.font = fonte; t.text = valor; t.fontSize = tamanho; t.color = new Color(.96f, .91f, .76f);
                t.alignment = TextAlignmentOptions.Center; t.raycastTarget = false;
                t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Truncate;
                return t;
            }
            Button Botao(string rotulo, float y)
            {
                var caixa = Caixa(rotulo, canvas.transform, 1110, y, 580, 112);
                var imagem = caixa.gameObject.AddComponent<Image>(); imagem.color = Color.white;
                var borda = caixa.gameObject.AddComponent<Outline>(); borda.effectColor = new Color(.67f, .45f, .23f); borda.effectDistance = new Vector2(3, -3);
                var b = caixa.gameObject.AddComponent<Button>(); b.targetGraphic = imagem;
                var cores = b.colors; cores.normalColor = new Color(.16f, .18f, .19f, .96f);
                cores.highlightedColor = new Color(.48f, .36f, .19f); cores.selectedColor = new Color(.48f, .36f, .19f);
                cores.pressedColor = new Color(.28f, .22f, .12f); cores.disabledColor = new Color(.1f, .1f, .1f); b.colors = cores;
                Texto("Rótulo", caixa, 10, 4, 560, 104, rotulo, 52);
                return b;
            }
            Texto("Título", canvas.transform, 1000, 220, 800, 160, "Game Over", 105);
            var manager = new GameObject("GameOverManager", typeof(GameOverManager)).GetComponent<GameOverManager>();
            manager.botaoContinuar = Botao("Continuar", 470); manager.botaoSair = Botao("Sair", 620);
            manager.textoAviso = Texto("Aviso", canvas.transform, 970, 790, 850, 230, "", 38);
            UnityEventTools.AddPersistentListener(manager.botaoContinuar.onClick, manager.Continuar);
            UnityEventTools.AddPersistentListener(manager.botaoSair.onClick, manager.Sair);
            var nav = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = manager.botaoSair, selectOnUp = manager.botaoSair };
            manager.botaoContinuar.navigation = nav;
            nav.selectOnDown = manager.botaoContinuar; nav.selectOnUp = manager.botaoContinuar;
            manager.botaoSair.navigation = nav;
            var eventos = new GameObject("EventSystemGameOver"); eventos.SetActive(false);
            eventos.AddComponent<EventSystem>(); eventos.AddComponent<StandaloneInputModule>(); manager.eventosLocais = eventos;
            if (!EditorSceneManager.SaveScene(nova, Cena)) throw new InvalidOperationException("Não foi possível salvar GameOver.");
            if (!EditorBuildSettings.scenes.Any(s => s.path == Cena))
                EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(Cena, true) }).ToArray();
            // Unity 6 pode expor scenes pelo perfil ativo; persistir também a lista global do projeto.
            if (!EditorBuildSettings.globalScenes.Any(s => s.path == Cena))
                EditorBuildSettings.globalScenes = EditorBuildSettings.globalScenes.Concat(new[] { new EditorBuildSettingsScene(Cena, true) }).ToArray();
            return "GameOver criada/salva e acrescentada ao BuildSettings. Cenas previamente abertas preservadas, sem salvar. Canvas local 1920x1080/Match0.5; ações TMP e EventSystem condicional.";
        }
        finally
        {
            if (anterior.IsValid() && anterior.isLoaded) SceneManager.SetActiveScene(anterior);
            if (nova.IsValid() && nova.isLoaded) EditorSceneManager.CloseScene(nova, true);
        }
    }
}
