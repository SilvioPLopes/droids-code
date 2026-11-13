using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DroidsCode.DroidCore;
using DroidsCode.Combat;

/// <summary>Fixture manual e finita. Só usa save temporário e preview isolada; não navega cenas.</summary>
public static class ValidacaoGameOver
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Exigir(bool ok, string mensagem) { if (!ok) throw new InvalidOperationException(mensagem); }

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var falhas = new List<string>();
        void Caso(string nome, Action acao)
        {
            try { acao(); }
            catch (Exception e) { falhas.Add(nome + ": " + (e.InnerException ?? e).Message); }
        }
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = singleton.GetValue(null);
        var cena = EditorSceneManager.NewPreviewScene();
        string pasta = Path.Combine(Path.GetTempPath(), "DroidsCode-GameOver-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pasta);
        try
        {
            GameObject Objeto(string nome, params Type[] tipos)
            {
                var go = new GameObject(nome, tipos);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena);
                return go;
            }
            var estado = Objeto("Estado isolado", typeof(GerenciadorDeEstado)).GetComponent<GerenciadorDeEstado>();
            singleton.SetValue(null, estado);
            typeof(GerenciadorDeEstado).GetProperty("DroidDoJogador").SetValue(estado, new Droid("Fixture", 40));
            foreach (string id in new[] { "droid_do_ivo", "sucata_captura" })
            {
                Caso("derrota " + id, () =>
                {
                    var battle = Objeto("Battle isolada", typeof(BattleManager)).GetComponent<BattleManager>();
                    battle.idDoInimigoPadrao = id;
                    battle.botaoAtacar = Objeto("Atacar", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.botaoItem = Objeto("Item", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.botaoFugir = Objeto("Fugir", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.textoMensagem = Objeto("Mensagem", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                    var rotina = (IEnumerator)typeof(BattleManager).GetMethod("FinalizarBatalha", Privado).Invoke(battle, new object[] { false, false });
                    Exigir(rotina.MoveNext(), "Derrota não aguardou a mensagem.");
                    // Não avançar depois da espera: a próxima etapa navega e pertence ao teste Play.
                    Exigir(rotina.Current is WaitForSeconds, "Derrota ainda usa a rotina de retorno ao mundo em vez de aguardar GameOver.");
                    Exigir(!battle.botaoAtacar.interactable && !battle.botaoItem.interactable, "Ações continuam abertas.");
                    var repetida = (IEnumerator)typeof(BattleManager).GetMethod("FinalizarBatalha", Privado).Invoke(battle, new object[] { false, false });
                    Exigir(!repetida.MoveNext(), "Finalização repetida não foi bloqueada.");
                });
            }
            foreach (bool vitoria in new[] { true, false })
            {
                Caso(vitoria ? "vitória mantém retorno ao mundo" : "fuga mantém retorno ao mundo", () =>
                {
                    var battle = Objeto("Saída sem derrota", typeof(BattleManager)).GetComponent<BattleManager>();
                    battle.botaoAtacar = Objeto("Atacar", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.botaoItem = Objeto("Item", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.botaoFugir = Objeto("Fugir", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.textoMensagem = Objeto("Mensagem", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                    typeof(BattleManager).GetField("droid", Privado).SetValue(battle, estado.DroidDoJogador);
                    typeof(BattleManager).GetField("inimigo", Privado).SetValue(battle, new InimigoFixo("Fixture", 10, 1, 1, 0));
                    var rotina = (IEnumerator)typeof(BattleManager).GetMethod("FinalizarBatalha", Privado).Invoke(battle, new object[] { vitoria, !vitoria });
                    Exigir(rotina.MoveNext() && rotina.Current is IEnumerator, "Vitória/fuga desviou do retorno ao mundo.");
                    var retorno = (IEnumerator)rotina.Current;
                    Exigir(retorno.MoveNext() && retorno.Current is WaitForSeconds, "Retorno não respeita a espera.");
                    // Não executar LoadScene nem escrever checkpoint real.
                });
            }
            Caso("restauração limpa sem regravar", () =>
            {
                string arquivo = Path.Combine(pasta, "save.json");
                var save = new SalvamentoJson(arquivo);
                estado.DroidDoJogador.Hp = 23;
                estado.DroidDoJogador.RestaurarEnergia(31, 100);
                estado.CarregarGold(187);
                estado.DefinirFlag("fixture_marco", true);
                estado.AdicionarItem(CatalogoDeItens.IdPocaoPequena, 2);
                estado.DroidDoJogador.EquiparPeca(TipoDePeca.Braco,
                    ServicoDeEquipamento.CriarPeca(TipoDePeca.Braco, null, "Braço fixture", Raridade.Raro, TipoAtributo.For, 3, 4));
                save.SalvarCheckpoint(estado, "City", new Vector3(3, 4, 0));
                var bytes = File.ReadAllBytes(arquivo);
                estado.DroidDoJogador.Hp = 0;
                estado.DroidDoJogador.EfeitosAtivos.Add(new EfeitoDeAtributo { NomeExibicao = "Stun", DuracaoEmTurnos = 2 });
                estado.DroidDoJogador.EfeitosDeDanoAtivos.Add(new EfeitoDeDanoPorTurno { NomeExibicao = "Vazamento", DanoPorTurno = 2, DuracaoEmTurnos = 2 });
                SalvamentoJson.AplicarDadosSemTrocarCena(save.LerDados(), estado);
                Exigir(estado.DroidDoJogador.Hp == 23 && estado.DroidDoJogador.EnergiaAtual == 31, "HP/energia não vieram do checkpoint.");
                Exigir(estado.Gold == 187 && estado.ObterFlag("fixture_marco") && estado.ObterQuantidadeDeItem(CatalogoDeItens.IdPocaoPequena) == 2, "Progresso não restaurado.");
                Exigir(estado.PosicaoSalva == new Vector3(3, 4, 0), "Posição não restaurada.");
                Exigir(estado.DroidDoJogador.ObterPeca(TipoDePeca.Braco)?.Nome == "Braço fixture", "Equipamento não restaurado.");
                Exigir(Convert.ToBase64String(bytes) == Convert.ToBase64String(File.ReadAllBytes(arquivo)), "Carga modificou o arquivo.");
                Exigir(estado.DroidDoJogador.EfeitosAtivos.Count == 0 && estado.DroidDoJogador.EfeitosDeDanoAtivos.Count == 0, "Carga herdou efeitos temporários da derrota.");
            });
            Caso("controlador disponível", () =>
            {
                var tipo = typeof(BattleManager).Assembly.GetType("GameOverManager");
                Exigir(tipo != null && tipo.GetMethod("Continuar") != null && tipo.GetMethod("Sair") != null, "Callbacks Continuar/Sair ainda ausentes.");
            });
            Caso("continuação recusa arquivos não recuperáveis", () =>
            {
                var tipo = typeof(BattleManager).Assembly.GetType("GameOverManager");
                var preparar = tipo?.GetMethod("PrepararContinuacao", BindingFlags.Public | BindingFlags.Static);
                Exigir(preparar != null, "Preparação isolada da continuação ainda ausente.");
                string arquivo = Path.Combine(pasta, "invalido.json");
                var save = new SalvamentoJson(arquivo);
                void Recusar(string nome)
                {
                    int hpAnterior = estado.DroidDoJogador.Hp;
                    string antes = File.Exists(arquivo) ? File.ReadAllText(arquivo) : null;
                    bool recusou = false;
                    try { preparar.Invoke(null, new object[] { save, estado }); }
                    catch (TargetInvocationException) { recusou = true; }
                    Exigir(recusou, "Aceitou " + nome);
                    Exigir(estado.DroidDoJogador.Hp == hpAnterior, "Arquivo inválido alterou HP: " + nome);
                    Exigir(antes == (File.Exists(arquivo) ? File.ReadAllText(arquivo) : null), "Alterou arquivo: " + nome);
                }
                Recusar("save ausente");
                File.WriteAllText(arquivo, "{invalido"); Recusar("JSON inválido");
                var dados = SalvamentoJson.CriarDados(estado, "City", Vector3.one);
                dados.versao = 999; File.WriteAllText(arquivo, JsonUtility.ToJson(dados)); Recusar("versão futura");
                dados.versao = 7;
                foreach (string destino in new[] { "Battle", "GameOver", "CenaInexistenteDaFixture" })
                {
                    dados.cena = destino; File.WriteAllText(arquivo, JsonUtility.ToJson(dados)); Recusar("destino " + destino);
                }
                dados.cena = "City"; dados.droid.hp = 0;
                File.WriteAllText(arquivo, JsonUtility.ToJson(dados)); Recusar("HP zero"); Recusar("HP zero repetido");
                dados.droid.hp = 17; dados.droid.energiaAtual = 29;
                File.WriteAllText(arquivo, JsonUtility.ToJson(dados));
                string texto = File.ReadAllText(arquivo);
                var restaurado = (DadosDoJogo)preparar.Invoke(null, new object[] { save, estado });
                Exigir(restaurado.cena == "City" && estado.DroidDoJogador.Hp == 17 && estado.DroidDoJogador.EnergiaAtual == 29,
                    "Continuação não aplicou o último checkpoint.");
                Exigir(File.ReadAllText(arquivo) == texto, "Continuação gravou progresso.");
            });
            Caso("GameOver não pode ser destino de gravação", () =>
            {
                string arquivo = Path.Combine(pasta, "nao-gravar.json");
                bool recusou = false;
                try { new SalvamentoJson(arquivo).SalvarCheckpoint(estado, "GameOver", Vector3.zero); }
                catch (InvalidOperationException) { recusou = true; }
                Exigir(recusou && !File.Exists(arquivo), "Gravou checkpoint dentro de GameOver.");
            });
        }
        finally
        {
            singleton.SetValue(null, anterior);
            EditorSceneManager.ClosePreviewScene(cena);
            Directory.Delete(pasta, true);
        }
        Exigir(falhas.Count == 0, "GameOver — " + string.Join(" | ", falhas));
        return "GameOver: derrotas comum/boss aguardam rota própria; carga limpa restaura progresso sem gravar; callbacks disponíveis. Save pessoal intocado.";
    }

    public static string ValidarTela(int largura, int altura)
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/GameOver.unity");
        RenderTexture rt = null; Texture2D captura = null;
        var ativoAnterior = RenderTexture.active;
        try
        {
            var manager = Array.Find(cena.GetRootGameObjects(), go => go.GetComponent<GameOverManager>() != null).GetComponent<GameOverManager>();
            var canvas = Array.Find(cena.GetRootGameObjects(), go => go.GetComponent<Canvas>() != null).GetComponent<Canvas>();
            var camera = Array.Find(cena.GetRootGameObjects(), go => go.GetComponent<Camera>() != null).GetComponent<Camera>();
            camera.scene = cena; camera.orthographicSize = altura / 2f; camera.aspect = (float)largura / altura;
            rt = new RenderTexture(largura, altura, 24); camera.targetTexture = rt;
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            Canvas.ForceUpdateCanvases();
            foreach (var texto in canvas.GetComponentsInChildren<TextMeshProUGUI>()) texto.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
            // GraphicRaycaster exige profundidades de desenho já processadas pelo Canvas.
            camera.Render();
            var cantos = new Vector3[4];
            var eventos = manager.eventosLocais.GetComponent<UnityEngine.EventSystems.EventSystem>();
            foreach (var b in new[] { manager.botaoContinuar, manager.botaoSair })
            {
                Exigir(b != null && b.onClick.GetPersistentEventCount() == 1 && b.onClick.GetPersistentTarget(0) == manager, "Callback real ausente: " + b?.name);
                ((RectTransform)b.transform).GetWorldCorners(cantos);
                foreach (var canto in cantos)
                {
                    var p = camera.WorldToScreenPoint(canto);
                    Exigir(p.x >= -1 && p.x <= largura + 1 && p.y >= -1 && p.y <= altura + 1, "Controle cortado: " + b.name);
                }
                var t = b.GetComponentInChildren<TextMeshProUGUI>();
                Exigir(!t.isTextTruncated && t.fontSize * canvas.scaleFactor >= 20, "Rótulo pequeno/cortado: " + b.name);
                var hits = new List<UnityEngine.EventSystems.RaycastResult>();
                var r = (RectTransform)b.transform;
                canvas.GetComponent<GraphicRaycaster>().Raycast(new UnityEngine.EventSystems.PointerEventData(eventos)
                    { position = camera.WorldToScreenPoint(r.TransformPoint(r.rect.center)) }, hits);
                Exigir(hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == b,
                    "Clique obstruído: " + b.name + "; depth=" + b.targetGraphic.depth + "; cull=" + b.targetGraphic.canvasRenderer.cull + "; hits=" + hits.Count);
            }
            Exigir(manager.botaoContinuar.onClick.GetPersistentMethodName(0) == "Continuar" && manager.botaoSair.onClick.GetPersistentMethodName(0) == "Sair", "Botões apontam para ações incorretas.");
            RenderTexture.active = rt;
            captura = new Texture2D(largura, altura, TextureFormat.RGB24, false);
            captura.ReadPixels(new Rect(0, 0, largura, altura), 0, 0); captura.Apply();
            string pasta = Path.GetFullPath("output/game-over"); Directory.CreateDirectory(pasta);
            string destino = Path.Combine(pasta, "game-over-" + largura + "x" + altura + ".png");
            File.WriteAllBytes(destino, captura.EncodeToPNG());
            return "GameOver render " + largura + "x" + altura + "; Canvas local ref1920x1080/Match0.5; rótulos e callbacks verificados; " + destino;
        }
        finally
        {
            RenderTexture.active = ativoAnterior;
            if (captura != null) UnityEngine.Object.DestroyImmediate(captura);
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena);
        }
    }
}
