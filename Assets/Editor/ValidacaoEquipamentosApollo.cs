using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DroidsCode.DroidCore;
using DroidsCode.Scripting;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Fixtures exclusivas em output; não toca save pessoal nem carrega cenas de jogo.</summary>
public static class ValidacaoEquipamentosApollo
{
    const string Pasta = "output/equipamentos-apollo/";
    static readonly FieldInfo Singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.NonPublic | BindingFlags.Static);
    static void Exigir(bool ok, string erro) { if (!ok) throw new InvalidOperationException(erro); }
    static GameObject Objeto(UnityEngine.SceneManagement.Scene cena, string nome, params Type[] tipos)
    {
        var go = new GameObject(nome, tipos); UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena); return go;
    }
    static GerenciadorDeEstado Estado(UnityEngine.SceneManagement.Scene cena)
    {
        var e = Objeto(cena, "EstadoFixture", typeof(GerenciadorDeEstado)).GetComponent<GerenciadorDeEstado>();
        typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(e, new Droid("Fixture", 30));
        return e;
    }
    static void Precondicoes()
    {
        Exigir(!EditorApplication.isPlayingOrWillChangePlaymode, "Exige fora de Play.");
        Exigir(Application.dataPath.Replace('\\', '/') == "C:/Users/silve/DroidsCode/Assets", "Projeto inesperado.");
        Directory.CreateDirectory(Pasta);
    }
    static void Equipar(GerenciadorDeEstado e, string id)
    {
        Exigir(ServicoDeEquipamento.TentarEquipar(e, id, out var m), m);
    }
    [MenuItem("DroidsCode/Validação/Equipamentos Apollo (arquivo isolado)")]
    public static void Menu() => Debug.Log(Executar());
    public static string Executar()
    {
        Precondicoes(); var anterior = Singleton.GetValue(null); var cena = EditorSceneManager.NewPreviewScene();
        string arquivo = Path.GetFullPath(Pasta + "fixture-" + Guid.NewGuid().ToString("N") + ".json");
        try
        {
            var e = Estado(cena); Singleton.SetValue(null, e); var d = e.DroidDoJogador;
            var sprites = new HashSet<Sprite>(); int itens = 0, pecas = 0;
            foreach (var kv in CatalogoDeItens.Todos)
            {
                Exigir(kv.Key == kv.Value.Id, "ID divergente: " + kv.Key);
                var sprite = CatalogoVisualDeItens.Obter(kv.Key);
                Exigir(sprite != null && sprites.Add(sprite), "Ícone ausente/repetido: " + kv.Key); itens++;
                if (kv.Value.Tipo != TipoDeItem.Equipavel) continue;
                pecas++; e.AdicionarItem(kv.Key, 2); Equipar(e, kv.Key);
                Exigir(d.ObterPeca(kv.Value.SlotDePeca.Value).IdItem == kv.Key, "Identidade perdida.");
                Exigir(d.ObterPeca(kv.Value.SlotDePeca.Value).Raridade == kv.Value.Raridade, "Raridade perdida.");
            }
            Exigir(itens == 27 && pecas == 6, "Catálogo mudou; revisar cobertura.");
            Equipar(e, "nucleo_de_energia"); int q = e.ObterQuantidadeDeItem("nucleo_de_energia");
            Equipar(e, "placa_de_blindagem");
            Exigir(e.ObterQuantidadeDeItem("nucleo_de_energia") == q + 1, "Troca não devolveu peça anterior.");
            q = e.ObterQuantidadeDeItem("placa_de_blindagem"); Equipar(e, "placa_de_blindagem");
            Exigir(q == e.ObterQuantidadeDeItem("placa_de_blindagem"), "Mesmo ID consumiu unidade.");
            Exigir(!ServicoDeEquipamento.TentarEquipar(e, "nao_existe", out _) && !ServicoDeEquipamento.TentarEquipar(e, "pocao_pequena", out _), "ID inválido aceito.");
            d.Hp = d.HpMax; int tetoComPeca = d.HpMax;
            Exigir(ServicoDeEquipamento.TentarDesequipar(e, TipoDePeca.Tronco, out _), "Desequipar falhou.");
            Exigir(d.Tronco == null && d.Hp == d.HpMax && d.HpMax < tetoComPeca, "Remoção de VIT não limitou HP.");
            int hp = d.Hp; Equipar(e, "placa_de_blindagem"); Exigir(d.Hp == hp, "Troca curou HP.");
            var vazio = Estado(cena);
            Exigir(!ServicoDeEquipamento.TentarEquipar(vazio, "sensor_optico", out _) && vazio.DroidDoJogador.Cabeca == null, "Sem estoque instalou peça.");
            var api = new TerminalDroidApi(d, e); var runner = new DroidScriptRunner(api);
            Exigir(runner.ExecutarCodigo("droid.desequipar('perna'); assert(droid.equipar('servo_motor')); droid.listarPecas()").Sucesso, "Comandos Lua reais falharam.");
            q = e.ObterQuantidadeDeItem("servo_motor"); var pernaAntes = d.Perna;
            Exigir(!runner.ExecutarCodigo("droid.desequipar('perna'); error('fixture')").Sucesso, "Erro Lua não detectado.");
            Exigir(ReferenceEquals(pernaAntes, d.Perna) && q == e.ObterQuantidadeDeItem("servo_motor"), "Erro Lua não reverteu inventário/peça.");
            Equipar(e, "sensor_optico"); Equipar(e, CatalogoDeItens.IdNucleoDeSobrecarga);
            d.Hp = d.HpMax - 1; d.RestaurarEnergia(25, 100);
            var save = new SalvamentoJson(arquivo); save.SalvarCheckpoint(e, "Game", new Vector3(2, 3, 0));
            var dados = save.LerDados(); Exigir(dados.versao == 7 && dados.droid.pecas.Count == 4, "Save não gravou quatro peças/v7.");
            var novo = Estado(cena); SalvamentoJson.AplicarDadosSemTrocarCena(dados, novo);
            foreach (TipoDePeca slot in Enum.GetValues(typeof(TipoDePeca)))
            {
                var a = d.ObterPeca(slot); var b = novo.DroidDoJogador.ObterPeca(slot);
                Exigir(a.IdItem == b.IdItem && a.Raridade == b.Raridade && a.AtributoBonificado == b.AtributoBonificado && a.ValorDoBonus == b.ValorDoBonus, "Carga perdeu payload: " + slot);
            }
            Exigir(novo.DroidDoJogador.Hp == d.Hp && novo.DroidDoJogador.EnergiaAtual == 25, "Carga truncou HP equipado/energia.");
            foreach (var kv in e.Inventario) Exigir(novo.ObterQuantidadeDeItem(kv.Key) == kv.Value, "Inventário divergente após carga.");
            int antesDaRemocao = novo.ObterQuantidadeDeItem("sensor_optico");
            Exigir(ServicoDeEquipamento.TentarDesequipar(novo, TipoDePeca.Cabeca, out _) && novo.DroidDoJogador.Cabeca == null
                && novo.ObterQuantidadeDeItem("sensor_optico") == antesDaRemocao + 1, "Peça carregada não pôde ser devolvida.");
            Equipar(novo, "sensor_optico");
            var invalido = JsonUtility.FromJson<DadosDoJogo>(JsonUtility.ToJson(dados));
            invalido.droid.pecas.Add(invalido.droid.pecas[0]);
            hp = novo.DroidDoJogador.Hp; bool rejeitou = false;
            try { SalvamentoJson.AplicarDadosSemTrocarCena(invalido, novo); } catch (InvalidOperationException) { rejeitou = true; }
            Exigir(rejeitou && hp == novo.DroidDoJogador.Hp, "Inválido aplicou estado parcial.");
            foreach (var caso in new[] { "duplicado", "id", "slot", "versao" })
            {
                invalido = JsonUtility.FromJson<DadosDoJogo>(JsonUtility.ToJson(dados));
                if (caso == "duplicado") invalido.droid.pecas[1].slot = invalido.droid.pecas[0].slot;
                if (caso == "id") invalido.droid.pecas[0].idItem = "id_invalido";
                if (caso == "slot") invalido.droid.pecas[0].slot = 999;
                if (caso == "versao") invalido.versao = 8;
                rejeitou = false; try { SalvamentoJson.AplicarDadosSemTrocarCena(invalido, novo); } catch (InvalidOperationException) { rejeitou = true; }
                Exigir(rejeitou && novo.DroidDoJogador.Cabeca.IdItem == d.Cabeca.IdItem, "Falha na validação: " + caso);
            }
            dados.versao = 6; dados.droid.pecas = null;
            SalvamentoJson.AplicarDadosSemTrocarCena(dados, novo);
            foreach (TipoDePeca slot in Enum.GetValues(typeof(TipoDePeca))) Exigir(novo.DroidDoJogador.ObterPeca(slot) == null, "Legado preservou slot da sessão anterior.");
            Exigir(dados.versao == 7, "Migração legada falhou.");
            return "OK: 27 IDs/ícones únicos, 6 equipamentos; troca/devolução/idempotência/raridade/estoque/HP; Lua real equipar/desequipar/listar e rollback em erro; grava/lê/restaura quatro slots em instância nova, inventário/HP/energia; legado e rejeição de ID/slot/duplicata/versão futura. Sem cena/save pessoal.";
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(cena); Singleton.SetValue(null, anterior);
            if (File.Exists(arquivo)) File.Delete(arquivo);
            if (File.Exists(arquivo + ".tmp")) File.Delete(arquivo + ".tmp");
        }
    }
    public static string Visual(int largura, int altura)
    {
        Precondicoes(); var anterior = Singleton.GetValue(null); var cena = EditorSceneManager.NewPreviewScene();
        Camera camera = null; RenderTexture rt = null;
        try
        {
            var e = Estado(cena); Singleton.SetValue(null, e);
            foreach (var kv in CatalogoDeItens.Todos) if (kv.Value.Tipo == TipoDeItem.Equipavel) e.AdicionarItem(kv.Key, 2);
            foreach (var id in new[] { "servo_motor", "sensor_optico", "placa_de_blindagem", CatalogoDeItens.IdNucleoDeSobrecarga }) Equipar(e, id);
            for (int n = 0; n < 12; n++) e.DroidDoJogador.TecnicasConfiguradas["Técnica " + n] = new TecnicaComposta { Nome = "Técnica " + n, NivelDeDano = n + 1 };
            var raiz = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Canvas.prefab"));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(raiz, cena);
            foreach (var c in raiz.GetComponentsInChildren<Canvas>(true)) c.enabled = false;
            var controller = raiz.GetComponentInChildren<TelaDeDroidManager>(true);
            Exigir(controller != null && controller.arteDroid != null && controller.corpoApollo != null, "Vínculos da arte/corpo ausentes.");
            // Isola somente o painel real; mantém sua escala herdada como desafio de encaixe local.
            var canvasGo = Objeto(cena, "CanvasFixture", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            controller.transform.SetParent(canvasGo.transform, true); UnityEngine.Object.DestroyImmediate(raiz);
            camera = Objeto(cena, "CameraFixture", typeof(Camera)).GetComponent<Camera>(); camera.scene = cena;
            camera.orthographic = true; camera.orthographicSize = altura / 2f; camera.aspect = (float)largura / altura;
            camera.transform.position = new Vector3(0, 0, -10); camera.cullingMask = 1 << 30;
            rt = new RenderTexture(largura, altura, 24); camera.targetTexture = rt;
            var canvas = canvasGo.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            controller.Mostrar(); var a = controller.painel.GetComponentInChildren<ApresentacaoDroidIlustrado>();
            Exigir(a != null, "Apresentação não foi criada.");
            foreach (TipoDePeca slot in Enum.GetValues(typeof(TipoDePeca)))
                Exigir(a.transform.Find("Equipado" + slot + "/Icone").GetComponent<Image>().sprite == CatalogoVisualDeItens.Obter(e.DroidDoJogador.ObterPeca(slot).IdItem), "Ícone não corresponde à peça instalada.");
            foreach (var tr in canvasGo.GetComponentsInChildren<Transform>(true)) tr.gameObject.layer = 30;
            Canvas.ForceUpdateCanvases(); a.AtualizarEscala(); Canvas.ForceUpdateCanvases(); camera.Render();
            var cantos = new Vector3[4]; ((RectTransform)a.transform).GetWorldCorners(cantos);
            foreach (var canto in cantos) { var p = camera.WorldToScreenPoint(canto); Exigir(p.x >= -1 && p.y >= -1 && p.x <= largura + 1 && p.y <= altura + 1, "Painel fora da tela."); }
            a.MudarPaginaTecnicas(1); Exigir(a.transform.Find("PaginaTecnicas").GetComponent<TextMeshProUGUI>().text.Contains("2/2"), "Técnicas inacessíveis."); a.MudarPaginaTecnicas(-1);
            int quantidade = e.ObterQuantidadeDeItem("servo_motor"); a.transform.Find("PecaDisponivel0").GetComponent<Button>().onClick.Invoke();
            Exigir(e.ObterQuantidadeDeItem("servo_motor") == quantidade, "Inspeção consumiu item."); a.Atualizar();
            foreach (var t in a.GetComponentsInChildren<TextMeshProUGUI>()) t.ForceMeshUpdate();
            camera.Render(); var ativo = RenderTexture.active; Texture2D imagem = null;
            try { RenderTexture.active = rt; imagem = new Texture2D(largura, altura, TextureFormat.RGB24, false); imagem.ReadPixels(new Rect(0, 0, largura, altura), 0, 0); imagem.Apply(); File.WriteAllBytes(Pasta + $"preview-{largura}x{altura}.png", imagem.EncodeToPNG()); }
            finally { RenderTexture.active = ativo; if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem); }
            var eventos = Objeto(cena, "EventosFixture", typeof(EventSystem)).GetComponent<EventSystem>(); var voltar = a.transform.Find("Voltar").GetComponent<Button>();
            var r = (RectTransform)voltar.transform; var hits = new List<RaycastResult>();
            canvasGo.GetComponent<GraphicRaycaster>().Raycast(new PointerEventData(eventos) { position = camera.WorldToScreenPoint(r.TransformPoint(r.rect.center)) }, hits);
            Exigir(hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == voltar, "Retorno bloqueado no raycast.");
            bool fechou = false; controller.AoFechar = () => fechou = true; voltar.onClick.Invoke(); Exigir(fechou && !controller.painel.activeSelf, "Retorno falhou.");
            controller.Mostrar(); Exigir(controller.painel.GetComponentsInChildren<ApresentacaoDroidIlustrado>(true).Length == 1, "Reabertura duplicou montagem.");
            return $"OK {largura}x{altura}: prefab real, quatro ícones por ID, seis atributos, estoque inspecionável sem mutação, paginação de 13 técnicas, enquadramento, raycast/retorno/reabertura. Preview ScreenSpaceCamera sem Scaler, fit local .98; não representa input real/aprovação visual.";
        }
        finally
        {
            if (camera != null) camera.targetTexture = null;
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena); Singleton.SetValue(null, anterior);
        }
    }
}
