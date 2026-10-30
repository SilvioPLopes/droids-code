using System;
using System.Linq;
using System.IO;
using System.Reflection;
using DroidsCode.Combat;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ArteInimigosBattle
{
    const string Pasta = "Assets/sprites/Entities/Enemies/Battle/";
    const string Saida = "output/inimigos-battle/2026-10-29/";
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Exigir(bool ok, string erro) { if (!ok) throw new InvalidOperationException(erro); }

    static Sprite Importar(string arquivo)
    {
        string caminho = Pasta + arquivo;
        AssetDatabase.ImportAsset(caminho);
        var imp = AssetImporter.GetAtPath(caminho) as TextureImporter;
        Exigir(imp != null, "Imagem não encontrada: " + caminho);
        imp.textureType = TextureImporterType.Sprite; imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = 100; imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false; imp.filterMode = FilterMode.Point;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.maxTextureSize = 2048; imp.npotScale = TextureImporterNPOTScale.None;
        var settings = new TextureImporterSettings(); imp.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect; imp.SetTextureSettings(settings);
        imp.SaveAndReimport();
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(caminho);
        Exigir(sprite != null, "Sprite não importada: " + caminho); return sprite;
    }

    public static string Aplicar()
    {
        var cena = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        Exigir(Application.dataPath.Replace('\\', '/') == "C:/Users/silve/DroidsCode/Assets" &&
            !EditorApplication.isPlaying && cena.path == "Assets/Scenes/Battle.unity" && !cena.isDirty,
            "Exige projeto correto, Battle limpa e fora de Play.");
        var images = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Image>(true)).ToArray();
        var alvo = images.Single(i => i.name == "SpriteInimigo");
        var duplicado = images.Single(i => i.name == "Inimigo");
        Exigir(alvo.GetComponent<ApresentacaoInimigoBattle>() == null, "Já montado; não repetir.");
        var a = Importar("SucataCaptura-v1.png"); var b = Importar("SucataReforcada-v1.png");
        var visual = alvo.gameObject.AddComponent<ApresentacaoInimigoBattle>();
        visual.batalha = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
        visual.droidA = a; visual.droidB = b; visual.legado = duplicado.sprite;
        visual.quadroLegado = new Rect(alvo.rectTransform.anchoredPosition.x, -alvo.rectTransform.anchoredPosition.y,
            alvo.rectTransform.sizeDelta.x, alvo.rectTransform.sizeDelta.y);
        visual.AplicarPorId(""); duplicado.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(cena); EditorSceneManager.SaveScene(cena);
        return VerificarMontagem() + " A/B vinculadas por ID; Battle salva sem alterar BattleManager.";
    }
    public static string VerificarMontagem()
    {
        var cena = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var visiveis = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Image>(true))
            .Where(i => (i.name == "SpriteInimigo" || i.name == "Inimigo") && i.isActiveAndEnabled).ToArray();
        if (visiveis.Length != 1 || !AssetDatabase.GetAssetPath(visiveis[0].sprite).StartsWith("Assets/sprites/Entities/Enemies/Battle/"))
            throw new InvalidOperationException("Exige uma única arte nova visível; montagem legada ainda presente.");
        return "OK: uma arte nova visível, sem o duplicado legado.";
    }

    public static string Validar(int largura, int altura, string id, string nome)
    {
        Exigir(!EditorApplication.isPlaying, "Exige fora de Play.");
        var campo = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = campo.GetValue(null);
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Battle.unity");
        RenderTexture rt = null;
        try
        {
            var raizes = cena.GetRootGameObjects();
            var b = raizes.SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
            var visual = raizes.SelectMany(g => g.GetComponentsInChildren<ApresentacaoInimigoBattle>(true)).Single();
            var imagem = visual.GetComponent<Image>();
            var go = new GameObject("EstadoFixtureArtes", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena);
            var estado = go.GetComponent<GerenciadorDeEstado>();
            var d = new Droid("Heroi", 30); d.Hp = 23; d.RestaurarEnergia(75, 100);
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", Privado).SetValue(estado, d);
            campo.SetValue(null, estado);
            estado.ProximoInimigoId = id;
            typeof(BattleManager).GetMethod("Start", Privado).Invoke(b, null);
            string nomeReal = b.nomeInimigo; int hpReal = b.hpMaxInimigo;
            var definicao = CatalogoDeInimigos.Obter(id);
            Exigir(definicao != null && nomeReal == definicao.Nome && hpReal == definicao.HpMax,
                "Dados do encontro não correspondem ao catálogo.");
            visual.AplicarPorId(CatalogoDeInimigos.IdSucataDeCaptura);
            Exigir(imagem.sprite == visual.droidA, "ID A não selecionou A.");
            visual.AplicarPorId(CatalogoDeInimigos.IdSucataReforcada);
            Exigir(imagem.sprite == visual.droidB && visual.droidA != visual.droidB, "ID B não diferenciou B.");
            visual.AplicarPorId("id_sem_arte");
            Exigir(imagem.sprite == visual.legado && imagem.rectTransform.sizeDelta == visual.quadroLegado.size,
                "Fallback legado não preservado.");
            visual.AplicarPorId(""); Exigir(imagem.sprite == visual.droidA, "Battle direta não exibe A.");
            estado.ProximoInimigoId = ""; b.idDoInimigoPadrao = "";
            visual.AplicarContexto(); Exigir(imagem.sprite == visual.droidA, "Contexto vazio não exibe A.");
            b.idDoInimigoPadrao = CatalogoDeInimigos.IdSucataReforcada;
            estado.ProximoInimigoId = CatalogoDeInimigos.IdSucataDeCaptura;
            visual.AplicarContexto(); Exigir(imagem.sprite == visual.droidA, "Contexto não tem precedência.");
            estado.ProximoInimigoId = "";
            visual.AplicarContexto(); Exigir(imagem.sprite == visual.droidB, "ID padrão não selecionou B.");
            Exigir(b.nomeInimigo == nomeReal && b.hpMaxInimigo == hpReal, "Arte alterou atributos do gestor.");
            estado.ProximoInimigoId = id; visual.AplicarContexto();
            var alvoEsperado = id == CatalogoDeInimigos.IdSucataReforcada ? visual.droidB : visual.droidA;
            Exigir(imagem.sprite == alvoEsperado, "Arte não corresponde ao encontro.");
            var antigos = raizes.SelectMany(g => g.GetComponentsInChildren<Image>(true))
                .Where(i => i.name == "SpriteInimigo" || i.name == "Inimigo").ToArray();
            Exigir(antigos.Count(i => i.isActiveAndEnabled) == 1, "Duplicado permanece visível.");
            Exigir(imagem.sprite.texture.filterMode == FilterMode.Point, "Filtro da sprite não é Point.");
            var cam = raizes.SelectMany(g => g.GetComponentsInChildren<Camera>(true)).First();
            cam.scene = cena; cam.enabled = false; cam.orthographic = true; cam.orthographicSize = altura / 2f;
            cam.aspect = (float)largura / altura; cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(.025f, .03f, .025f);
            rt = new RenderTexture(largura, altura, 24); cam.targetTexture = rt;
            var canvas = b.botaoAtacar.GetComponentInParent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = cam; canvas.planeDistance = 10;
            Canvas.ForceUpdateCanvases();
            var hud = raizes.SelectMany(g => g.GetComponentsInChildren<ApresentacaoBattleIlustrada>(true)).Single();
            hud.AtualizarLayout(); hud.AtualizarHud(d, b.nomeInimigo, b.hpMaxInimigo, b.hpMaxInimigo);
            hud.AtualizarConteudo(); Canvas.ForceUpdateCanvases();
            var cantos = new Vector3[4]; imagem.rectTransform.GetWorldCorners(cantos);
            foreach (var canto in cantos)
            {
                var p = cam.WorldToScreenPoint(canto);
                Exigir(p.x >= 0 && p.y >= 0 && p.x <= largura && p.y <= altura, "Figura fora da tela.");
            }
            cam.Render(); Salvar(rt, Saida + $"battle-{nome}-{largura}x{altura}.png");
            return $"OK {nome} {largura}x{altura}: A/B por ID, contexto/padrão/fallback, uma imagem visível, enquadramento/filtroPoint, nome/HP corretos, sem alteração de stats ou saves.";
        }
        finally
        {
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena); campo.SetValue(null, anterior);
        }
    }

    static void Salvar(RenderTexture rt, string caminho)
    {
        var antes = RenderTexture.active; Texture2D t = null;
        try
        {
            RenderTexture.active = rt; t = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            t.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); t.Apply();
            File.WriteAllBytes(caminho, t.EncodeToPNG());
        }
        finally { RenderTexture.active = antes; if (t != null) UnityEngine.Object.DestroyImmediate(t); }
    }
}
