using System;
using System.IO;
using DroidsCode.DroidCore;
using DroidsCode.Combat;
using UnityEditor;
using UnityEngine;

/// <summary>Testes isolados de energia e DTO/JSON em memória. Não escreve save nem troca cena.</summary>
[InitializeOnLoad]
public static class ValidacaoEnergiaApollo
{
    static ValidacaoEnergiaApollo()
    {
        if (File.Exists("output/status-ilustrado/energia.pendente")) EditorApplication.delayCall += Pendente;
    }
    static void Pendente()
    {
        const string pasta = "output/status-ilustrado/";
        if (!File.Exists(pasta + "energia.pendente")) return;
        File.Delete(pasta + "energia.pendente");
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { File.WriteAllText(pasta + "energia-validacao.txt", "Pendente: sair de Play para testar isoladamente."); return; }
        try { File.WriteAllText(pasta + "energia-validacao.txt", Executar()); }
        catch (Exception e) { File.WriteAllText(pasta + "energia-validacao.txt", e.ToString()); }
    }
    static void Exigir(bool ok, string erro) { if (!ok) throw new InvalidOperationException(erro); }
    [MenuItem("DroidsCode/Validação/Energia Apollo (sem saves)")]
    public static void Menu() => Debug.Log(Executar());
    public static string Executar()
    {
        Exigir(!EditorApplication.isPlayingOrWillChangePlaymode, "Exige fora de Play.");
        var randomAnterior = UnityEngine.Random.state;
        try
        {
            var d = new Droid("TesteEnergia", 100); var alvo = new Droid("Alvo", 100);
            d.TecnicasConfiguradas["Fraca"] = new TecnicaComposta { Nome = "Fraca", NivelDeDano = 1 };
            d.TecnicasConfiguradas["Forte"] = new TecnicaComposta { Nome = "Forte", NivelDeDano = 3 };
            Exigir(d.EnergiaAtual == 100 && d.EnergiaMaxima == 100, "Reserva inicial incorreta.");
            Exigir(d.ObterCustoDeEnergia("Forte") > d.ObterCustoDeEnergia("Fraca"), "Força não aumenta custo.");
            var efeito = d.TecnicasConfiguradas["Forte"];
            int custoAntes = d.ObterCustoDeEnergia("Forte");
            efeito.EfeitosDeAtributo.Add(new EfeitoDeAtributo { NomeExibicao = "Stun", DuracaoEmTurnos = 2 });
            Exigir(d.ObterCustoDeEnergia("Forte") > custoAntes, "Efeito não aumenta custo.");
            int custo = d.ObterCustoDeEnergia("Fraca");
            d.ExecutarAcao("Fraca", alvo); Exigir(d.EnergiaAtual == 100 - custo, "Tentativa não cobrou exatamente uma vez.");
            // Escolher uma semente de erro sem depender de sorte, preservando Random.state no finally.
            alvo.StatsBase.Agi = 1000;
            int sementeErro = -1;
            for (int n = 0; n < 20; n++)
            {
                UnityEngine.Random.InitState(n);
                if (UnityEngine.Random.Range(0, 100) >= TabelaDeCombate.ChanceDeAcertoMinima) { sementeErro = n; break; }
            }
            Exigir(sementeErro >= 0, "Fixture não encontrou semente de erro.");
            UnityEngine.Random.InitState(sementeErro);
            var errou = d.ExecutarAcao("Fraca", alvo);
            Exigir(!errou.Sucesso && d.EnergiaAtual == 100 - 2 * custo, "Erro não cobrou energia.");
            d.RestaurarEnergia(0, 100); int hpAntes = alvo.Hp;
            var insuficiente = d.ExecutarAcao("Forte", alvo);
            Exigir(!insuficiente.Sucesso && insuficiente.DanoCausado == 0 && alvo.Hp == hpAntes && alvo.EfeitosAtivos.Count == 0 && d.EnergiaAtual == 0,
                "Sem energia aplicou dano/efeito ou alterou saldo.");
            Exigir(insuficiente.Mensagem.Contains("energia"), "Falta de energia sem explicação.");
            d.ExecutarAcao(Droid.NomeAtaqueBasico, alvo); Exigir(d.EnergiaAtual == 10, "Básico não recuperou energia.");
            d.RestaurarEnergia(99, 100); d.ExecutarAcao(Droid.NomeAtaqueBasico, alvo); Exigir(d.EnergiaAtual == 100, "Recarga ultrapassou teto.");
            d.RestaurarEnergia(60, 100);
            d.EfeitosAtivos.Add(new EfeitoDeAtributo { NomeExibicao = "Stun", DuracaoEmTurnos = 2 });
            new CombatEngine().ExecutarTurno(d, alvo, "Forte"); Exigir(d.EnergiaAtual == 60, "Atordoamento gastou energia.");
            d.EfeitosAtivos.Clear(); d.Hp = 0; new CombatEngine().ExecutarTurno(d, alvo, "Forte");
            Exigir(d.EnergiaAtual == 60, "Morte gastou energia.");
            d.RestaurarEnergia(0, 100); d.ExecutarAcao("Inexistente", alvo); Exigir(d.EnergiaAtual == 0, "Técnica inválida alterou energia.");
            var legado = new DroidSalvo { nome = "Legado", hp = 30, hpMaxBase = 30 };
            var dto = new DadosDoJogo { versao = 6, droid = legado };
            SalvamentoJson.MigrarDados(dto); Exigir(legado.energiaAtual == 100 && legado.energiaMaxima == 100, "Save legado não inicia cheio.");
            var novo = new DadosDoJogo { droid = new DroidSalvo { energiaAtual = 0, energiaMaxima = 100 } };
            var volta = JsonUtility.FromJson<DadosDoJogo>(JsonUtility.ToJson(novo));
            SalvamentoJson.MigrarDados(volta); Exigir(volta.droid.energiaAtual == 0 && volta.droid.energiaMaxima == 100, "Zero perdido na migração/JSON.");
            return "OK: inicial100, custo por força/efeitos, cobrança única inclusive erro, saldo insuficiente sem dano/efeitos, básico+10/teto, stun/morte/inválida sem gasto, legado cheio e zero preservado no JSON. Sem saves/cenas reais.";
        }
        finally { UnityEngine.Random.state = randomAnterior; }
    }
}
