using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public static class MontagemFerrovale
{
    const string Output = "output/montagem-ferrovale/2026-10-29/";
    static JObject Config() => JObject.Parse(File.ReadAllText(Output + "config-montagem.json"));
    static string S(JToken t, string key) => (string)t[key];
    static float F(JToken t, string key) => (float)t[key];
    static Vector2 Pixel(float x, float y) => new Vector2((x - 724) / 200f, (543 - y) / 200f);
    static Vector3 PlayerAt(Vector2 pixel) { var p = Pixel(pixel.x, pixel.y); return new Vector3(p.x, p.y - .62f, 0); }
    static GameObject Child(string name, Transform parent) { var g = new GameObject(name); g.transform.SetParent(parent, false); return g; }
    static void Guard()
    {
        if (Application.dataPath.Replace('\\', '/') != "C:/Users/silve/DroidsCode/Assets" || EditorApplication.isPlaying || EditorApplication.isCompiling) throw new Exception("Unexpected project, Play or compilation.");
        for (int i = 0; i < SceneManager.sceneCount; i++) if (SceneManager.GetSceneAt(i).isDirty) throw new Exception("Unsaved scene: " + SceneManager.GetSceneAt(i).path);
    }
    static void Folder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = path.Substring(0, path.LastIndexOf('/')); Folder(parent); AssetDatabase.CreateFolder(parent, path.Substring(path.LastIndexOf('/') + 1));
    }
    static Sprite SpriteAt(string path, float ppu, bool actor)
    {
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) throw new Exception("Missing importer: " + path);
        ti.textureType = TextureImporterType.Sprite; ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = ppu; ti.mipmapEnabled = false; ti.alphaIsTransparency = true;
        ti.textureCompression = TextureImporterCompression.Uncompressed; ti.maxTextureSize = actor ? 64 : 2048;
        ti.filterMode = actor ? FilterMode.Point : FilterMode.Bilinear; ti.wrapMode = TextureWrapMode.Clamp; ti.npotScale = TextureImporterNPOTScale.None;
        var settings = new TextureImporterSettings(); ti.ReadTextureSettings(settings); settings.spriteAlignment = (int)SpriteAlignment.Custom;
        settings.spritePivot = new Vector2(.5f, actor ? .125f : .5f); settings.spriteMeshType = SpriteMeshType.FullRect; ti.SetTextureSettings(settings);
        ti.SaveAndReimport(); return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    static Scene Open(string path, out bool opened)
    {
        var scene = SceneManager.GetSceneByPath(path); opened = !scene.IsValid() || !scene.isLoaded;
        return opened ? EditorSceneManager.OpenScene(path, OpenSceneMode.Additive) : scene;
    }
    static IEnumerable<T> Components<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true));
    static void AddStory(GameObject go, string id, string flag, string[] required = null)
    {
        var g = go.AddComponent<GatilhoDeDialogo>(); g.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript;
        g.idDoDialogo = id; g.apenasUmaVez = true; g.flagAoTerminar = flag;
        g.flagsNecessarias = required ?? Array.Empty<string>(); g.flagsQueImpedem = string.IsNullOrEmpty(flag) ? Array.Empty<string>() : new[] { flag };
    }
    static void Stories(GameObject go, string id, Scene city)
    {
        string sourceName = id == "dara" ? "Dara" : id == "tacio" ? "Tácio" : id == "homem_fila" ? "homem da fila" : null;
        if (sourceName != null)
        {
            foreach (var source in Components<GatilhoDeDialogo>(city).Where(g => g.name == sourceName))
            {
                var g = go.AddComponent<GatilhoDeDialogo>(); EditorUtility.CopySerialized(source, g);
                g.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript; g.indicadorDeInteracao = null; g.AoConcluirDialogoExterno = null;
            }
        }
        else if (id == "mestra_guilda") AddStory(go, CatalogoDeDialogos.CityS2Bruna, "ato1_falou_bruna");
        else if (id == "estalajadeira") AddStory(go, CatalogoDeDialogos.CityS2Nilce, "ato1_falou_nilce");
        else if (id == "ferreiro") AddStory(go, CatalogoDeDialogos.CityS2Quirino, "ato1_falou_quirino");
        else if (id == "vendedora_pecas") AddStory(go, CatalogoDeDialogos.CityS2Sula, "ato1_falou_sula");
        else if (id == "crianca_rua") AddStory(go, CatalogoDeDialogos.CityS2Pipo, "ato1_falou_pipo");
        else if (id == "velho_viaduto") AddStory(go, CatalogoDeDialogos.CityS2Onofre, "ato1_falou_onofre");
        else if (id == "nadir") AddStory(go, CatalogoDeDialogos.CityS7NadirOferta, "ato1_ouviu_nadir", new[] { "ato1_nadir" });
    }
    public static object BuildNpcPrefabs()
    {
        Guard(); var cfg = Config(); var previous = SceneManager.GetActiveScene(); bool opened;
        var city = Open("Assets/Scenes/City.unity", out opened);
        var scratch = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive); SceneManager.SetActiveScene(scratch);
        Folder("Assets/Prefabs/NPCs/Ferrovale"); var created = new List<string>();
        try
        {
            foreach (var actor in cfg["actors"])
            {
                string path = S(actor, "prefab"); if (File.Exists(path)) continue;
                var go = new GameObject(S(actor, "name")); go.transform.localScale = new Vector3(1.4f, 1.4f, 1);
                var sr = go.AddComponent<SpriteRenderer>(); sr.sortingOrder = 1;
                var poses = new[] { "cima", "esquerda", "baixo", "direita" }.Select(d => SpriteAt(S(actor, "poses") + "/" + d + ".png", 100, true)).ToArray(); sr.sprite = poses[2];
                var range = go.AddComponent<CircleCollider2D>(); range.isTrigger = true; range.radius = .65f;
                var body = go.AddComponent<CircleCollider2D>(); body.radius = .13f;
                var orient = go.AddComponent<OrientacaoNpc>(); orient.renderizador = sr; orient.cima = poses[0]; orient.esquerda = poses[1]; orient.baixo = poses[2]; orient.direita = poses[3];
                Stories(go, S(actor, "id"), city); range.isTrigger = true; body.isTrigger = false;
                var npc = go.AddComponent<MoradorInterativo>(); npc.idNpc = S(actor, "id"); npc.nome = CatalogoDeNpcs.Obter(S(actor, "id"))?.Nome ?? S(actor, "name"); npc.saudacao = S(actor, "greeting"); npc.negociar = (bool)actor["shop"]; npc.historia = go.GetComponents<GatilhoDeDialogo>();
                PrefabUtility.SaveAsPrefabAsset(go, path); Object.DestroyImmediate(go); created.Add(path);
            }
            AssetDatabase.SaveAssets(); File.WriteAllText(Output + "npc-prefabs.json", Newtonsoft.Json.JsonConvert.SerializeObject(created));
        }
        finally { EditorSceneManager.CloseScene(scratch, true); if (opened) EditorSceneManager.CloseScene(city, true); SceneManager.SetActiveScene(previous); }
        return new { created = created.Count, expected = cfg["actors"].Count(), prefabs = created };
    }

    static bool Inside(JToken room, Vector2 p, float radius, bool actors)
    {
        var poly = room["outline"].Select(v => new Vector2((float)v[0], (float)v[1])).ToArray(); bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            var a = poly[i]; var b = poly[j];
            if (((a.y > p.y) != (b.y > p.y)) && p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x) inside = !inside;
            var ab = b - a; float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
            if (Vector2.Distance(p, a + ab * t) < radius + 3) return false;
        }
        if (!inside) return false;
        foreach (var r in room["boxes"])
        {
            float x = Mathf.Clamp(p.x, (float)r[0], (float)r[2]); float y = Mathf.Clamp(p.y, (float)r[1], (float)r[3]);
            if (Vector2.Distance(p, new Vector2(x, y)) < radius + 3) return false;
        }
        if (actors) foreach (var a in room["actors"]) if (Vector2.Distance(p, new Vector2(F(a, "x"), F(a, "y"))) < radius + 42) return false;
        foreach (var portal in room["portals"]) if (Vector2.Distance(p, new Vector2(F(portal, "px"), F(portal, "py"))) < radius + 30) return false;
        return true;
    }
    static Vector2 Free(JToken room, Vector2 wanted, float radius = 60, bool actors = true)
    {
        if (Inside(room, wanted, radius, actors)) return wanted;
        var candidates = new List<Vector2>();
        for (float y = 310; y <= 840; y += 25) for (float x = 175; x <= 1275; x += 25) candidates.Add(new Vector2(x, y));
        foreach (var p in candidates.OrderBy(v => (v - wanted).sqrMagnitude)) if (Inside(room, p, radius, actors)) return p;
        throw new Exception("No free arrival in " + S(room, "id"));
    }
    static JObject Room(JObject cfg, string id) => (JObject)cfg["rooms"].Single(r => S(r, "id") == id);
    static void Box(Transform parent, string name, JToken r)
    {
        var go = Child(name, parent); go.transform.localPosition = Pixel(((float)r[0] + (float)r[2]) / 2, ((float)r[1] + (float)r[3]) / 2);
        go.AddComponent<BoxCollider2D>().size = new Vector2(((float)r[2] - (float)r[0]) / 200, ((float)r[3] - (float)r[1]) / 200);
    }
    static void PointStory(Scene scene, string name, Vector2 pixel, string dialogue, string text = null)
    {
        var go = new GameObject(name); SceneManager.MoveGameObjectToScene(go, scene); go.transform.position = Pixel(pixel.x, pixel.y);
        var c = go.AddComponent<CircleCollider2D>(); c.isTrigger = true; c.radius = .55f;
        var g = go.AddComponent<GatilhoDeDialogo>(); g.modo = GatilhoDeDialogo.ModoDeDisparo.AoInteragir; g.idDoDialogo = dialogue; g.apenasUmaVez = false;
        if (!string.IsNullOrEmpty(text)) g.falas.Add(new FalaDeDialogo { falante = "", texto = text });
        g.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript;
        var input = go.AddComponent<MoradorInterativo>(); input.idNpc = "fv_obj_" + name; input.nome = ""; input.historia = new[] { g };
    }
    static void Terminal(Scene scene, string name, Vector2 pixel, string puzzle, string prerequisite)
    {
        var go = new GameObject(name); SceneManager.MoveGameObjectToScene(go, scene); go.transform.position = Pixel(pixel.x, pixel.y);
        var range = go.AddComponent<CircleCollider2D>(); range.isTrigger = true; range.radius = .55f;
        var check = go.AddComponent<VerificadorDePuzzle>(); check.idDoPuzzle = puzzle; check.flagAoResolver = "fv_" + puzzle + "_ok"; check.flagQueJaResolve = check.flagAoResolver; check.mostrarFalaAoResolver = false; check.enabled = false;
        var terminal = go.AddComponent<TerminalDoInterior>(); terminal.idPuzzle = puzzle; terminal.flagNecessaria = prerequisite; terminal.verificador = check;
    }
    public static object BuildRooms(int start, int count)
    {
        Guard(); var cfg = Config(); var previous = SceneManager.GetActiveScene(); bool opened;
        var pilot = Open("Assets/Scenes/Interiores/Interior_E10_Casa03.unity", out opened);
        var sourcePlayer = pilot.GetRootGameObjects().Single(g => g.name == "Player");
        var sourceCamera = pilot.GetRootGameObjects().Single(g => g.name == "Main Camera");
        var sourceLight = Components<Light2D>(pilot).Single();
        var sourceEvent = pilot.GetRootGameObjects().Single(g => g.name == "EventSystem");
        var done = new List<string>();
        try
        {
            foreach (JObject room in cfg["rooms"].Skip(start).Take(count))
            {
                string path = S(room, "scene"); bool existingPilot = S(room, "id") == "E10_Casa03";
                if (File.Exists(path) && !existingPilot) { done.Add(S(room, "id") + ":existing"); continue; }
                Scene scene;
                if (existingPilot) scene = pilot;
                else { scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive); }
                SceneManager.SetActiveScene(scene);
                if (!existingPilot)
                {
                    var root = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Interiores/InteriorBase.prefab"), scene); root.name = S(room, "id");
                    var bg = root.transform.Find("Fundo").GetComponent<SpriteRenderer>(); bg.sprite = SpriteAt(S(room, "art"), 200, false); PrefabUtility.RecordPrefabInstancePropertyModifications(bg);
                    var edge = Child("LimitesDoPiso", root.transform.Find("Colisoes")).AddComponent<EdgeCollider2D>();
                    var outline = room["outline"].Select(v => Pixel((float)v[0], (float)v[1])).ToList(); outline.Add(outline[0]); edge.points = outline.ToArray();
                    int bi = 0; foreach (var b in room["boxes"]) Box(root.transform.Find("Colisoes"), "Obstaculo_" + (++bi), b);
                    var baseExit = root.transform.Find("Portas/Saida").gameObject; bool first = true;
                    foreach (var p in room["portals"])
                    {
                        var door = first ? baseExit : (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PortaTransicao.prefab"), scene); first = false;
                        door.name = S(p, "key"); door.transform.SetParent(root.transform.Find("Portas"), false); door.transform.localPosition = Pixel(F(p, "px"), F(p, "py"));
                        var transition = door.GetComponent<TransicaoDeCena>();
                        if (p["targetRoom"] != null)
                        {
                            var destination = Room(cfg, S(p, "targetRoom")); var wanted = new Vector2((float)p["targetFootPixel"][0], (float)p["targetFootPixel"][1]); var spawn = PlayerAt(Free(destination, wanted));
                            transition.nomeCenaDestino = S(destination, "sceneName"); transition.pontoDeChegada = spawn;
                        }
                        else { transition.nomeCenaDestino = S(p, "targetScene"); transition.pontoDeChegada = new Vector2(F(p, "targetX"), F(p, "targetY")); }
                        transition.trancada = false; var col = door.GetComponent<BoxCollider2D>(); col.size = new Vector2(.75f, .12f); col.isTrigger = true;
                        PrefabUtility.RecordPrefabInstancePropertyModifications(door.transform); PrefabUtility.RecordPrefabInstancePropertyModifications(transition); PrefabUtility.RecordPrefabInstancePropertyModifications(col);
                    }
                    PrefabUtility.SaveAsPrefabAsset(root, S(room, "prefab")); Object.DestroyImmediate(root);
                    PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(S(room, "prefab")), scene);
                    var player = Object.Instantiate(sourcePlayer); player.name = "Player"; player.transform.SetParent(null); player.transform.position = PlayerAt(Free(room, new Vector2(724, 750))); player.transform.localScale = new Vector3(2, 2, 1);
                    var camera = Object.Instantiate(sourceCamera); camera.name = "Main Camera"; camera.transform.position = new Vector3(0, 0, -10); var follow = camera.GetComponent<CameraFollow>(); follow.target = player.transform; follow.usarLimites = true; follow.limiteMin = new Vector2(-3.62f, -2.715f); follow.limiteMax = new Vector2(3.62f, 2.715f);
                    var light = new GameObject("Global Light 2D").AddComponent<Light2D>(); EditorUtility.CopySerialized(sourceLight, light);
                    PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Canvas.prefab"), scene);
                    var events = Object.Instantiate(sourceEvent); events.name = "EventSystem"; RenderSettings.skybox = null;
                }
                var mobRoot = scene.GetRootGameObjects().FirstOrDefault(g => g.name == "MoradoresFerrovale"); if (mobRoot == null) mobRoot = new GameObject("MoradoresFerrovale");
                foreach (var a in room["actors"])
                {
                    if (mobRoot.transform.Cast<Transform>().Any(t => t.name == S(a, "id"))) continue;
                    var actor = cfg["actors"].Single(t => S(t, "id") == S(a, "id")); var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(S(actor, "prefab")), scene);
                    go.name = S(a, "id"); go.transform.SetParent(mobRoot.transform, false); var foot = Free(room, new Vector2(F(a, "x"), F(a, "y")), 35, false); go.transform.position = Pixel(foot.x, foot.y);
                    if (S(a, "id") == "fv_mecanico_01" && S(room, "id") == "E06_Terreo")
                    { var npc = go.GetComponent<MoradorInterativo>(); var dest = Room(cfg, "E06_Anexo"); npc.destinoDeVisita = S(dest, "sceneName"); npc.chegadaDaVisita = PlayerAt(Free(dest, new Vector2(724, 750))); PrefabUtility.RecordPrefabInstancePropertyModifications(npc); }
                    PrefabUtility.RecordPrefabInstancePropertyModifications(go.transform);
                }
                if (!existingPilot)
                {
                    string id = S(room, "id");
                    if (id == "E03_Terreo") PointStory(scene, "CadernosDoIvo", new Vector2(1050, 400), CatalogoDeDialogos.CityS2CasaIvoCadernos);
                    if (id == "E03_Superior") { PointStory(scene, "FotoDeCelia", new Vector2(460, 345), CatalogoDeDialogos.CityS2CasaIvoFoto); PointStory(scene, "BilheteDoIvo", new Vector2(1000, 380), CatalogoDeDialogos.CityS2CasaIvoBilhete); }
                    if (id.EndsWith("_Patamar")) PointStory(scene, "EscadaSuperiorReservada", new Vector2(820, 440), "", "Os degraus acima estão instáveis. Melhor ficar neste andar.");
                    if (id == "E08_Superior") PointStory(scene, "QuartoTresReservado", new Vector2(550, 360), "", "O quarto três está reservado. Preciso conversar com a Nilce.");
                    if (id == "E08_Principal") Terminal(scene, "BombaDeReserva", new Vector2(1040, 380), CatalogoDePuzzles.IdBombaDeReserva, "quest_q_nilce_quarto_ativa");
                    if (id == "E11_Casa05") Terminal(scene, "RoloDeCobre", new Vector2(1100, 440), CatalogoDePuzzles.IdContadorDeSucata, "quest_q_tacio_linha_ativa");
                    if (id == "E09_Terreo") Terminal(scene, "TriagemDaGuilda", new Vector2(1000, 400), CatalogoDePuzzles.IdTriagemDePecas, "guilda_membro");
                }
                EditorSceneManager.MarkSceneDirty(scene); if (!EditorSceneManager.SaveScene(scene, path)) throw new Exception("Save failed: " + path);
                done.Add(S(room, "id")); File.WriteAllText(Output + "ultimo-lote.json", Newtonsoft.Json.JsonConvert.SerializeObject(new { start, count, done }));
                if (!existingPilot) EditorSceneManager.CloseScene(scene, true);
            }
            AssetDatabase.SaveAssets();
        }
        finally { if (opened) EditorSceneManager.CloseScene(pilot, true); if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous); }
        return new { start, count, done };
    }

    static void Allow(Object obj, HashSet<string> ids)
    {
        var id = GlobalObjectId.GetGlobalObjectIdSlow(obj).targetObjectId;
        if (id != 0) ids.Add(id.ToString());
    }
    static MoradorInterativo Wrap(GameObject go, string id, HashSet<string> ids)
    {
        Allow(go, ids);
        foreach (var old in go.GetComponents<NpcInterativo>()) { Allow(old, ids); old.enabled = false; }
        var input = go.GetComponent<MoradorInterativo>(); if (input == null) input = go.AddComponent<MoradorInterativo>();
        input.idNpc = id; input.nome = CatalogoDeNpcs.NomeDe(id); input.saudacao = CatalogoDeNpcs.Obter(id)?.TextoDeBoasVindas;
        input.negociar = id == CatalogoDeNpcs.IdTacio;
        var manual = new List<GatilhoDeDialogo>();
        foreach (var g in go.GetComponents<GatilhoDeDialogo>())
        {
            if (g.modo == GatilhoDeDialogo.ModoDeDisparo.AoEntrarNoTrigger) continue;
            Allow(g, ids); g.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript; manual.Add(g);
        }
        input.historia = manual.ToArray();
        if (id == CatalogoDeNpcs.IdApollo) input.flagParaMissoes = "ato1_casa_resolvida";
        if (id == CatalogoDeNpcs.IdTacio) foreach (var g in manual.Where(g => g.idDoDialogo == CatalogoDeDialogos.CityS2Tacio)) { g.flagAoTerminar = "ato1_falou_tacio"; g.flagsQueImpedem = (g.flagsQueImpedem ?? Array.Empty<string>()).Concat(new[] { "ato1_falou_tacio" }).Distinct().ToArray(); }
        EditorUtility.SetDirty(input); return input;
    }
    public static object WireCityAndCasa()
    {
        Guard(); var cfg = Config(); var previous = SceneManager.GetActiveScene(); bool opened;
        var city = Open("Assets/Scenes/City.unity", out opened); SceneManager.SetActiveScene(city);
        var allowed = new HashSet<string>(); var root = city.GetRootGameObjects().Single(g => g.name == "PortasMapeadas"); Allow(root, allowed); Allow(root.transform, allowed);
        var receipt = new List<object>();
        foreach (var c in cfg["cityDoors"])
        {
            string name = S(c, "key"); var transition = Components<TransicaoDeCena>(city).SingleOrDefault(t => t.name == name);
            if (transition == null)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PortaTransicao.prefab"), city); go.name = name; go.transform.SetParent(root.transform, false); transition = go.GetComponent<TransicaoDeCena>();
            }
            var before = new { x = transition.transform.position.x, y = transition.transform.position.y, target = transition.nomeCenaDestino };
            Allow(transition.gameObject, allowed); Allow(transition.transform, allowed); Allow(transition, allowed); Allow(transition.GetComponent<BoxCollider2D>(), allowed);
            // The prefab-instance override record is an existing YAML object distinct from its stripped Transform.
            var room = Room(cfg, S(c, "targetRoom")); var foot = Free(room, new Vector2((float)c["targetFootPixel"][0], (float)c["targetFootPixel"][1])); var spawn = PlayerAt(foot);
            transition.transform.position = new Vector3(F(c, "x"), F(c, "y"), 0); transition.nomeCenaDestino = S(room, "sceneName"); transition.pontoDeChegada = spawn; transition.trancada = false;
            var col = transition.GetComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(.55f, .12f);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transition.transform); PrefabUtility.RecordPrefabInstancePropertyModifications(transition); PrefabUtility.RecordPrefabInstancePropertyModifications(col);
            receipt.Add(new { name, before, x = transition.transform.position.x, y = transition.transform.position.y, scene = transition.nomeCenaDestino, spawnX = spawn.x, spawnY = spawn.y, returnX = F(c, "returnX"), returnY = F(c, "returnY") });
        }
        foreach (var pair in new[] { ("Dara", "dara"), ("Tácio", "tacio"), ("homem da fila", "homem_fila") })
        {
            var go = Components<GatilhoDeDialogo>(city).Where(g => g.name == pair.Item1).Select(g => g.gameObject).FirstOrDefault();
            if (go != null) Wrap(go, pair.Item2, allowed);
        }
        var outside = city.GetRootGameObjects().FirstOrDefault(g => g.name == "MoradoresFerrovale_Externos"); if (outside == null) outside = new GameObject("MoradoresFerrovale_Externos");
        foreach (var placement in new[] { ("velho_viaduto", -13f, -4.8f), ("benta", 6.2f, -4f), ("zeca_cerca", 0f, 9.2f), ("nadir", -.4f, -4.9f) })
        {
            if (outside.transform.Cast<Transform>().Any(t => t.name == placement.Item1)) continue;
            var actor = cfg["actors"].Single(a => S(a, "id") == placement.Item1);
            var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(S(actor, "prefab")), city); go.name = placement.Item1; go.transform.SetParent(outside.transform, false); go.transform.position = new Vector3(placement.Item2, placement.Item3, 0); PrefabUtility.RecordPrefabInstancePropertyModifications(go.transform);
            if (placement.Item1 == "nadir")
            {
                var controller = Child("PresencaDaNadir", outside.transform).AddComponent<CondicaoDeHistoria>();
                controller.regras = new[] { new CondicaoDeHistoria.Regra { descricao = "Nadir so apos a chegada da historia", alvos = new[] { go }, flagsNecessarias = new[] { "ato1_nadir" }, flagsQueImpedem = Array.Empty<string>() } };
            }
        }
        EditorSceneManager.MarkSceneDirty(city); EditorSceneManager.SaveScene(city);
        File.WriteAllText(Output + "city-edits.json", Newtonsoft.Json.JsonConvert.SerializeObject(new { allowedOldIds = allowed.ToArray(), doors = receipt, newRoot = "MoradoresFerrovale_Externos" }));
        var casa = Open("Assets/Scenes/CasaLip.unity", out bool casaOpened); var casaAllowed = new HashSet<string>();
        var apollo = Components<GatilhoDeDialogo>(casa).FirstOrDefault(g => g.name == "Apollo");
        if (apollo != null) { Wrap(apollo.gameObject, "apollo", casaAllowed); EditorSceneManager.MarkSceneDirty(casa); EditorSceneManager.SaveScene(casa); }
        File.WriteAllText(Output + "casalip-edits.json", Newtonsoft.Json.JsonConvert.SerializeObject(new { allowedOldIds = casaAllowed.ToArray(), apolloFound = apollo != null }));
        bool apolloIntegrated = apollo != null;
        if (casaOpened) EditorSceneManager.CloseScene(casa, true);
        var scenes = EditorBuildSettings.scenes.ToList();
        foreach (var r in cfg["rooms"]) if (!scenes.Any(s => s.path == S(r, "scene"))) scenes.Add(new EditorBuildSettingsScene(S(r, "scene"), true));
        EditorBuildSettings.scenes = scenes.ToArray(); AssetDatabase.SaveAssets();
        if (opened) EditorSceneManager.CloseScene(city, true); if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
        return new { buildingDoorsWired = receipt.Count, apolloIntegrated, buildSceneCount = scenes.Count, playing = EditorApplication.isPlaying };
    }
}
