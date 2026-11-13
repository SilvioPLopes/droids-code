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

/// <summary>Fixture artificial de API, sem Play, navegação ou acesso ao save pessoal.</summary>
public static class ValidacaoLimpezaEfeitosCombateDemo
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    const BindingFlags Estatico = BindingFlags.Static | BindingFlags.NonPublic;

    sealed class ProibirSaveDeQuest : ISistemaDeSalvamento
    {
        public void Salvar() { throw new InvalidOperationException("Gravação de quest fora do checkpoint isolado."); }
        public void Carregar() { throw new InvalidOperationException("Fixture não navega cenas."); }
        public bool ExisteSave() => false;
    }

    static void Exigir(bool ok, string mensagem)
    {
        if (!ok) throw new InvalidOperationException(mensagem);
    }

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", Estatico);
        var campoSaveQuest = typeof(GerenciadorDeQuests).GetField("Salvamento", Estatico);
        var estadoAnterior = singleton.GetValue(null);
        var saveAnterior = campoSaveQuest.GetValue(null);
        var eventoAnterior = GerenciadorDeQuests.AoMudarQuests;
        var randomAnterior = UnityEngine.Random.state;
        var cena = EditorSceneManager.NewPreviewScene();
        string pasta = Path.Combine(Path.GetTempPath(), "DroidsCode-Limpeza-Efeitos-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pasta);
        var falhas = new List<string>();
        var aprovados = new List<string>();
        void Caso(string nome, Action acao)
        {
            try { acao(); aprovados.Add(nome); }
            catch (Exception e) { falhas.Add(nome + ": " + (e.InnerException ?? e).Message); }
        }
        try
        {
            var sentinela = new ProibirSaveDeQuest();
            campoSaveQuest.SetValue(null, sentinela);
            Exigir(ReferenceEquals(campoSaveQuest.GetValue(null), sentinela), "Persistência de quests não foi isolada.");
            GerenciadorDeQuests.AoMudarQuests = null;
            GameObject Objeto(string nome, params Type[] tipos)
            {
                var go = new GameObject(nome, tipos);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena);
                return go;
            }
            foreach (var caso in new[]
            {
                (nome: "vitória com HP do buff", vitoria: true, fuga: false, ferido: false),
                (nome: "fuga com HP do buff", vitoria: false, fuga: true, ferido: false),
                (nome: "derrota preserva checkpoint", vitoria: false, fuga: false, ferido: false),
                (nome: "vitória não cura o ferido", vitoria: true, fuga: false, ferido: true),
                (nome: "fuga não cura o ferido", vitoria: false, fuga: true, ferido: true)
            })
            {
                Caso(caso.nome, () =>
                {
                    var estado = Objeto("Estado artificial " + caso.nome, typeof(GerenciadorDeEstado)).GetComponent<GerenciadorDeEstado>();
                    singleton.SetValue(null, estado);
                    var droid = new Droid("Fixture", 100);
                    typeof(GerenciadorDeEstado).GetProperty("DroidDoJogador").SetValue(estado, droid);
                    estado.CarregarFlags(new Dictionary<string, bool>());
                    estado.CarregarContadores(new Dictionary<string, int>());
                    estado.CarregarInventario(new Dictionary<string, int>());
                    estado.CarregarGold(11);
                    estado.ZerarMenusAbertos();
                    estado.AdicionarItem(CatalogoDeItens.IdPocaoPequena, 2);
                    estado.AdicionarItem(CatalogoDeItens.IdServoMotor, 3);
                    estado.AdicionarItem(CatalogoDeItens.IdFiltroDeAgua, 1);
                    droid.StatsBase.For = 5; droid.StatsBase.Agi = 4; droid.StatsBase.Vit = 10;
                    droid.StatsBase.Int = 3; droid.StatsBase.Dex = 8; droid.StatsBase.Luk = 2;
                    droid.RestaurarEnergia(37, 100);
                    droid.Pontos.DefinirPontos(4);
                    var placa = CatalogoDeItens.Obter(CatalogoDeItens.IdPlacaDeBlindagem);
                    var peca = ServicoDeEquipamento.CriarPeca(TipoDePeca.Tronco, placa.Id, placa.Nome, placa.Raridade,
                        placa.AtributoBonificado, placa.ValorDoBonus);
                    droid.EquiparPeca(TipoDePeca.Tronco, peca);
                    var tecnica = new TecnicaComposta { Nome = "Técnica de fixture", NivelDeDano = 1 };
                    tecnica.EfeitosDeDanoPorTurno.Add(new EfeitoDeDanoPorTurno
                        { NomeExibicao = "Veneno da técnica", DanoPorTurno = 1, DuracaoEmTurnos = 2 });
                    droid.TecnicasConfiguradas[tecnica.Nome] = tecnica;
                    int maximoSemTemporarios = droid.HpMax;
                    droid.Hp = maximoSemTemporarios;
                    string arquivo = Path.Combine(pasta, Guid.NewGuid().ToString("N") + ".json");
                    var salvamento = new SalvamentoJson(arquivo);
                    salvamento.SalvarCheckpoint(estado, "Game", new Vector3(3, 4, 0));
                    byte[] checkpointAnterior = File.ReadAllBytes(arquivo);

                    droid.EfeitosAtivos.Add(new EfeitoDeAtributo
                        { NomeExibicao = "Buff VIT", Atributo = TipoAtributo.Vit, Valor = 50, DuracaoEmTurnos = 3 });
                    droid.EfeitosAtivos.Add(new EfeitoDeAtributo
                        { NomeExibicao = "Debuff FOR", Atributo = TipoAtributo.For, Valor = -2, DuracaoEmTurnos = 2 });
                    droid.EfeitosDeDanoAtivos.Add(new EfeitoDeDanoPorTurno
                        { NomeExibicao = "Vazamento", DanoPorTurno = 2, DuracaoEmTurnos = 1 });
                    Exigir(droid.HpMax > maximoSemTemporarios, "Fixture não criou o bônus temporário de HP.");
                    droid.Hp = !caso.vitoria && !caso.fuga ? 0 : caso.ferido ? 25 : droid.HpMax;
                    int hpEsperado = !caso.vitoria && !caso.fuga ? 0 : caso.ferido ? 25 : maximoSemTemporarios;

                    var battle = Objeto("Batalha artificial " + caso.nome, typeof(BattleManager)).GetComponent<BattleManager>();
                    battle.botaoAtacar = Objeto("Atacar", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.botaoItem = Objeto("Item", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.botaoFugir = Objeto("Fugir", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
                    battle.textoMensagem = Objeto("Mensagem", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                    battle.flagDeVitoria = "fixture_vitoria";
                    battle.goldMinimo = 7; battle.goldMaximo = 7;
                    battle.tabelaDeDrops.Add(new BattleManager.DropDeItem
                        { idItem = CatalogoDeItens.IdPocaoPequena, chance = 1, quantidadeMinima = 3, quantidadeMaxima = 3 });
                    typeof(BattleManager).GetField("droid", Privado).SetValue(battle, droid);
                    typeof(BattleManager).GetField("inimigo", Privado).SetValue(battle, new InimigoFixo("Fixture", 10, 1, 1, 80));
                    typeof(BattleManager).GetField("_salvamento", Privado).SetValue(battle, salvamento);
                    typeof(BattleManager).GetField("_cenaRetorno", Privado).SetValue(battle, "Game");
                    typeof(BattleManager).GetField("_posicaoRetorno", Privado).SetValue(battle, new Vector3(3, 4, 0));
                    var rotina = (IEnumerator)typeof(BattleManager).GetMethod("FinalizarBatalha", Privado)
                        .Invoke(battle, new object[] { caso.vitoria, caso.fuga });
                    Exigir(rotina.MoveNext(), "Finalização não iniciou sua saída.");
                    // Não avançar depois do primeiro yield: navegação e esperas pertencem ao percurso Play.
                    Exigir(droid.EfeitosAtivos.Count == 0 && droid.EfeitosDeDanoAtivos.Count == 0,
                        "Efeitos temporários sobreviveram ao fim da batalha.");
                    Exigir(droid.HpMax == maximoSemTemporarios && droid.Hp == hpEsperado,
                        "Remover temporários deixou HP acima do máximo, curou o ferido ou ressuscitou a derrota.");
                    Exigir(droid.HpMaxBase == 100 && droid.StatsBase.For == 5 && droid.StatsBase.Agi == 4
                        && droid.StatsBase.Vit == 10 && droid.StatsBase.Int == 3 && droid.StatsBase.Dex == 8 && droid.StatsBase.Luk == 2,
                        "Limpeza alterou atributos/base permanentes.");
                    Exigir(ReferenceEquals(droid.ObterPeca(TipoDePeca.Tronco), peca)
                        && droid.ObterTotal(TipoAtributo.Vit) == 16, "Limpeza removeu a peça ou seu bônus.");
                    Exigir(droid.EnergiaAtual == 37 && droid.EnergiaMaxima == 100 && droid.Pontos.PontosDisponiveis == 4,
                        "Fim alterou energia ou pontos não gastos.");
                    Exigir(ReferenceEquals(droid.TecnicasConfiguradas[tecnica.Nome], tecnica)
                        && tecnica.EfeitosDeDanoPorTurno.Count == 1, "Limpeza apagou efeitos configurados na técnica.");
                    Exigir(estado.Gold == (caso.vitoria ? 18 : 11)
                        && estado.ObterQuantidadeDeItem(CatalogoDeItens.IdPocaoPequena) == (caso.vitoria ? 5 : 2)
                        && estado.ObterQuantidadeDeItem(CatalogoDeItens.IdServoMotor) == 3
                        && estado.ObterQuantidadeDeItem(CatalogoDeItens.IdFiltroDeAgua) == 1
                        && estado.Inventario.Count == 3 && droid.Progressao.ExperienciaAtual == (caso.vitoria ? 80 : 0),
                        "Limpeza alterou inventário/recompensas ou pagou fuga/derrota.");
                    if (caso.vitoria || caso.fuga)
                    {
                        var dados = salvamento.LerDados();
                        Exigir(dados.cena == "Game" && dados.droid.hp == hpEsperado && dados.droid.energiaAtual == 37
                            && dados.droid.pecas.Count == 1 && dados.droid.pecas[0].idItem == placa.Id,
                            "Checkpoint foi gravado antes da limpeza ou perdeu dados permanentes.");
                        int hpAntesDoProximoTurno = droid.Hp;
                        var resultado = new CombatEngine().ExecutarTurno(droid, new InimigoFixo("Próximo", 50, 1, 0, 0), Droid.NomeAtaqueBasico);
                        Exigir(droid.Hp == hpAntesDoProximoTurno && !resultado.Mensagem.Contains("Vazamento"),
                            "O primeiro turno seguinte herdou dano da batalha encerrada.");
                    }
                    else Exigir(checkpointAnterior.SequenceEqual(File.ReadAllBytes(arquivo)), "Derrota sobrescreveu o checkpoint anterior.");
                });
            }
        }
        finally
        {
            campoSaveQuest.SetValue(null, saveAnterior);
            singleton.SetValue(null, estadoAnterior);
            GerenciadorDeQuests.AoMudarQuests = eventoAnterior;
            UnityEngine.Random.state = randomAnterior;
            EditorSceneManager.ClosePreviewScene(cena);
            Directory.Delete(pasta, true);
        }
        Exigir(falhas.Count == 0, "Limpeza de efeitos — " + string.Join(" | ", falhas));
        return "Limpeza: " + string.Join("; ", aprovados)
            + ". Temporários removidos antes do checkpoint; HP limitado sem cura; base/peça/técnica/energia/itens/recompensas preservados; primeiro turno seguinte sem Vazamento. Fixture artificial de API, sem navegação ou save pessoal.";
    }
}
