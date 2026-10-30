using System;
using System.Linq;
using System.Reflection;
using DroidsCode.Combat;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>Fixtures determinísticas: não chamam entrada/saída de batalha nem salvamento.</summary>
public static class ValidacaoEncontrosAB
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Exigir(bool ok, string mensagem) { if (!ok) throw new InvalidOperationException(mensagem); }

    [MenuItem("DroidsCode/Battle/Validar encontros A-B (sem saves)")]
    public static void Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        Debug.Log(Selecao());
        Debug.Log(Atributos());
        Debug.Log(Prefabs());
    }

    public static string Selecao()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var metodo = typeof(EncounterZone).GetMethod("TentarSelecionarInimigo");
        Exigir(metodo != null, "FALHA: zona não possui seleção de espécies por amostra.");
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Game.unity");
        try
        {
            var zona = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<EncounterZone>(true)).Single();
            Func<float, string> selecionar = amostra => {
                object[] args = { amostra, null };
                return (bool)metodo.Invoke(zona, args) ? (string)args[1] : null;
            };
            Exigir(selecionar(0) == CatalogoDeInimigos.IdSucataDeCaptura && selecionar(.14999f) == CatalogoDeInimigos.IdSucataDeCaptura, "Faixa A incorreta.");
            Exigir(selecionar(.15f) == CatalogoDeInimigos.IdSucataReforcada && selecionar(.19999f) == CatalogoDeInimigos.IdSucataReforcada, "Faixa B incorreta.");
            Exigir(selecionar(.20f) == null && selecionar(1) == null && selecionar(-.01f) == null && selecionar(float.NaN) == null, "Faixa sem encontro/amostra inválida incorreta.");
            int a = 0, b = 0, vazio = 0;
            for (int i = 0; i < 10000; i++)
            {
                var id = selecionar((i + .5f) / 10000f);
                if (id == CatalogoDeInimigos.IdSucataDeCaptura) a++;
                else if (id == CatalogoDeInimigos.IdSucataReforcada) b++;
                else vazio++;
            }
            Exigir(a == 1500 && b == 500 && vazio == 8000, $"Distribuição {a}/{b}/{vazio}.");
            var campo = typeof(EncounterZone).GetField("encontros");
            var entradas = (Array)campo.GetValue(zona);
            var tipo = entradas.GetValue(0).GetType();
            var primeiro = entradas.GetValue(0);
            tipo.GetField("idDoInimigo").SetValue(primeiro, CatalogoDeInimigos.IdDroidDoIvo);
            Exigir(selecionar(.01f) == null, "Chefe entrou na tabela comum.");
            tipo.GetField("idDoInimigo").SetValue(primeiro, CatalogoDeInimigos.IdDroidSelvagemAlfa);
            Exigir(selecionar(.01f) == null, "Único entrou na tabela comum.");
            tipo.GetField("idDoInimigo").SetValue(primeiro, "id_inexistente");
            Exigir(selecionar(.01f) == null, "ID desconhecido entrou na tabela.");
            tipo.GetField("chance").SetValue(primeiro, 1f);
            Exigir(selecionar(.17f) == null, "Tabela acima de100% não foi recusada.");
            campo.SetValue(zona, null); zona.idDoInimigo = ""; zona.chanceDeEncontro = .15f;
            Exigir(selecionar(.1f) == "" && selecionar(.16f) == null, "Fallback legado alterado.");
            zona.chanceDeEncontro = 0;
            Exigir(selecionar(0) == null, "Chance zero gerou encontro.");
            return "OK: 10.000 amostras determinísticas = A1500/B500/nenhum8000; fronteiras, inválidos, chefe/único e fallback.";
        }
        finally { EditorSceneManager.ClosePreviewScene(cena); }
    }

    public static string Atributos()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = singleton.GetValue(null);
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Battle.unity");
        try
        {
            var b = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
            var go = new GameObject("EstadoFixtureAB", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena);
            var estado = go.GetComponent<GerenciadorDeEstado>();
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", Privado).SetValue(estado, new Droid("Fixture", 30));
            singleton.SetValue(null, estado);
            foreach (var id in new[] { CatalogoDeInimigos.IdSucataDeCaptura, CatalogoDeInimigos.IdSucataReforcada, CatalogoDeInimigos.IdDroidDoIvo })
            {
                estado.ProximoInimigoId = id;
                typeof(BattleManager).GetMethod("Start", Privado).Invoke(b, null);
                var inimigo = (InimigoFixo)typeof(BattleManager).GetField("inimigo", Privado).GetValue(b);
                var def = CatalogoDeInimigos.Obter(id);
                Exigir(inimigo.HpMax == def.HpMax && inimigo.Hit == def.Hit && inimigo.Flee == def.Flee && inimigo.ChanceCritica == def.ChanceCritica && inimigo.ChanceDeResistirEfeito == def.ChanceDeResistirEfeito, "FALHA: atributos derivados não chegaram ao combate: " + id);
                if (!def.PermiteFugir) Exigir(!b.botaoFugir.interactable, "Chefe permite fuga na UI.");
            }
            return "OK: HP/acerto/esquiva/crítico/resistência reais do catálogo em A/B/chefe e bloqueio de fuga na UI.";
        }
        finally { singleton.SetValue(null, anterior); EditorSceneManager.ClosePreviewScene(cena); }
    }

    public static string Prefabs()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var visual = PrefabUtility.LoadPrefabContents("Assets/Prefabs/VisualInimigoBattle.prefab");
        try
        {
            var v = visual.GetComponent<ApresentacaoInimigoBattle>();
            Exigir(v != null && v.batalha == null && v.visuais.Length == 2, "Prefab visual depende de objeto externo ou não tem A/B.");
            Exigir(AssetDatabase.GetAssetPath(v.droidB).EndsWith("SucataReforcada-v2.png"), "Prefab visual não usa B-v2.");
            Exigir(v.droidB.texture.filterMode == FilterMode.Point, "B-v2 não usa Point.");
        }
        finally { PrefabUtility.UnloadPrefabContents(visual); }
        var zona = PrefabUtility.LoadPrefabContents("Assets/Prefabs/ZonaEncontro.prefab");
        try
        {
            var z = zona.GetComponent<EncounterZone>();
            Exigir(z != null && z.encontros.Length == 2 && zona.GetComponent<Collider2D>().isTrigger, "Prefab zona incompleto.");
            Exigir(z.TentarSelecionarInimigo(.17f, out var id) && id == CatalogoDeInimigos.IdSucataReforcada, "Prefab zona não seleciona B.");
        }
        finally { PrefabUtility.UnloadPrefabContents(zona); }
        return "OK: prefabs carregam, visual sem gestor externo/B-v2 Point e zona trigger com A/B.";
    }
}
