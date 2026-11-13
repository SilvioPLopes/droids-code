using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.Combat;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fixture de API, fora de Play. Estado artificial isolado; não aprova percurso da demo.</summary>
public static class ValidacaoProgressoQuestsDemo
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    const BindingFlags Estatico = BindingFlags.Static | BindingFlags.NonPublic;

    // A persistência é a fronteira externa: uma gravação antecipada de quest em Battle é erro.
    sealed class SalvamentoDeQuestIsolado : ISistemaDeSalvamento
    {
        public bool Proibir = true;
        public string Caminho;
        public void Salvar()
        {
            if (Proibir) throw new InvalidOperationException("Quest tentou gravar antes do checkpoint da batalha.");
            new SalvamentoJson(Caminho).Salvar();
        }
        public void Carregar() { throw new InvalidOperationException("Fixture não navega cenas."); }
        public bool ExisteSave() => File.Exists(Caminho);
    }

    static void Exigir(bool ok, string mensagem)
    {
        if (!ok) throw new InvalidOperationException(mensagem);
    }

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", Estatico);
        var servico = typeof(GerenciadorDeQuests).GetField("Salvamento", Estatico);
        var estadoAnterior = singleton.GetValue(null);
        var servicoAnterior = servico.GetValue(null);
        var eventoAnterior = GerenciadorDeQuests.AoMudarQuests;
        var randomAnterior = UnityEngine.Random.state;
        var cena = EditorSceneManager.NewPreviewScene();
        string pasta = Path.Combine(Path.GetTempPath(), "DroidsCode-Quests-Demo-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pasta);
        var falhas = new List<string>();
        var resultados = new List<string>();
        void Caso(string nome, Action acao)
        {
            try { acao(); resultados.Add(nome); }
            catch (Exception e) { falhas.Add(nome + ": " + (e.InnerException ?? e).Message); }
        }
        try
        {
            var salvamentoQuest = new SalvamentoDeQuestIsolado { Caminho = Path.Combine(pasta, "quest.json") };
            // Se o runtime não permitir substituir o readonly, falha ANTES de executar qualquer produtor.
            servico.SetValue(null, salvamentoQuest);
            Exigir(ReferenceEquals(servico.GetValue(null), salvamentoQuest), "Serviço de save da fixture não foi isolado.");
            GerenciadorDeQuests.AoMudarQuests = null;
            GameObject Objeto(string nome, params Type[] tipos)
            {
                var go = new GameObject(nome, tipos);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena);
                return go;
            }
            var estado = Objeto("Estado de quest isolado", typeof(GerenciadorDeEstado)).GetComponent<GerenciadorDeEstado>();
            singleton.SetValue(null, estado);
            void PrepararEstado(string idQuest)
            {
                typeof(GerenciadorDeEstado).GetProperty("DroidDoJogador").SetValue(estado, new Droid("Fixture", 40));
                estado.CarregarFlags(new Dictionary<string, bool>());
                estado.CarregarContadores(new Dictionary<string, int>());
                estado.CarregarInventario(new Dictionary<string, int>());
                estado.CarregarGold(0);
                estado.LimparBatalhaPendente();
                estado.DefinirFlag(GerenciadorDeQuests.ChaveAtiva(idQuest), true);
            }
            BattleManager PrepararBatalha(string id, string arquivo)
            {
                var battle = Objeto("Batalha isolada " + id, typeof(BattleManager)).GetComponent<BattleManager>();
                battle.idDoInimigoPadrao = id;
                battle.botaoAtacar = Objeto("Atacar", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                battle.botaoItem = Objeto("Item", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                battle.botaoFugir = Objeto("Fugir", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                battle.textoMensagem = Objeto("Mensagem", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                typeof(BattleManager).GetField("droid", Privado).SetValue(battle, estado.DroidDoJogador);
                typeof(BattleManager).GetMethod("AplicarInimigoDoCatalogo", Privado).Invoke(battle, null);
                var definicao = CatalogoDeInimigos.Obter(id);
                typeof(BattleManager).GetField("inimigo", Privado).SetValue(battle,
                    new InimigoFixo(definicao.Nome, definicao.HpMax, definicao.Ataque, definicao.Defesa, definicao.RecompensaXp));
                typeof(BattleManager).GetField("_salvamento", Privado).SetValue(battle, new SalvamentoJson(arquivo));
                typeof(BattleManager).GetField("_cenaRetorno", Privado).SetValue(battle, "Game");
                typeof(BattleManager).GetField("_posicaoRetorno", Privado).SetValue(battle, new Vector3(3, 4, 0));
                return battle;
            }
            IEnumerator Finalizar(BattleManager battle, bool vitoria, bool fuga = false) =>
                (IEnumerator)typeof(BattleManager).GetMethod("FinalizarBatalha", Privado).Invoke(battle, new object[] { vitoria, fuga });

            foreach (string id in new[] { "droid_do_ivo", "sucata_captura" })
            {
                Caso("vitória conta e persiste uma vez: " + id, () =>
                {
                    bool boss = id == "droid_do_ivo";
                    string quest = boss ? "q_main_estacao" : "q_limpeza_borda";
                    string chave = GerenciadorDeQuests.ChaveObjetivo(quest, boss ? 2 : 0);
                    PrepararEstado(quest);
                    string arquivo = Path.Combine(pasta, id + ".json");
                    var battle = PrepararBatalha(id, arquivo);
                    Exigir(Finalizar(battle, true).MoveNext(), "Vitória não iniciou a saída.");
                    Exigir(estado.ObterContador(chave) == 1, "Vitória não avançou o objetivo de derrotar.");
                    var dados = new SalvamentoJson(arquivo).LerDados();
                    Exigir(dados.cena == "Game" && dados.contadores.Any(c => c.chave == chave && c.valor == 1),
                        "Checkpoint da origem não contém o progresso da vitória.");
                    var bytes = File.ReadAllBytes(arquivo);
                    int gold = estado.Gold;
                    var itens = estado.Inventario.OrderBy(kv => kv.Key).ToArray();
                    Exigir(!Finalizar(battle, true).MoveNext(), "Finalização repetida foi aceita.");
                    Exigir(estado.ObterContador(chave) == 1 && estado.Gold == gold && itens.SequenceEqual(estado.Inventario.OrderBy(kv => kv.Key)),
                        "Finalização repetida duplicou progresso ou recompensas.");
                    Exigir(bytes.SequenceEqual(File.ReadAllBytes(arquivo)), "Finalização repetida regravou o checkpoint.");
                });
            }
            foreach (bool fuga in new[] { false, true })
            {
                Caso(fuga ? "fuga não conta vitória" : "derrota não conta vitória", () =>
                {
                    PrepararEstado("q_limpeza_borda");
                    string arquivo = Path.Combine(pasta, fuga ? "fuga.json" : "derrota.json");
                    var battle = PrepararBatalha("sucata_captura", arquivo);
                    Exigir(Finalizar(battle, false, fuga).MoveNext(), "Saída não iniciou.");
                    Exigir(estado.ObterContador("quest_q_limpeza_borda_obj_0") == 0, "Saída sem vitória contou inimigo derrotado.");
                    Exigir(estado.Gold == 0 && estado.Inventario.Count == 0, "Saída sem vitória pagou recompensa.");
                    Exigir(File.Exists(arquivo) == fuga, "Derrota gravou ou fuga perdeu o checkpoint.");
                });
            }
            Caso("consumidor existente mantém persistência padrão", () =>
            {
                PrepararEstado("q_main_agua");
                salvamentoQuest.Proibir = false;
                GerenciadorDeQuests.RegistrarProgresso(TipoDeObjetivo.Conversar, "dara");
                var dados = new SalvamentoJson(salvamentoQuest.Caminho).LerDados();
                Exigir(dados.contadores.Any(c => c.chave == "quest_q_main_agua_obj_0" && c.valor == 1),
                    "Consumidor sem opção explícita deixou de salvar seu progresso.");
            });
        }
        finally
        {
            servico.SetValue(null, servicoAnterior);
            singleton.SetValue(null, estadoAnterior);
            GerenciadorDeQuests.AoMudarQuests = eventoAnterior;
            UnityEngine.Random.state = randomAnterior;
            EditorSceneManager.ClosePreviewScene(cena);
            Directory.Delete(pasta, true);
        }
        Exigir(falhas.Count == 0, "Quests de combate — " + string.Join(" | ", falhas));
        return "Quests de combate: " + string.Join("; ", resultados) + ". Fixture/API isolada; não navega cenas nem acessa save pessoal.";
    }
}
