using System.IO;
using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Salva/carrega em Application.persistentDataPath/save.json.
/// Cobre: Droid (stats, HP, pontos, tecnicas), posicao no mapa, cena, e uma
/// lista aberta de flags de historia (vazia por enquanto — a estrutura ja
/// existe pra quando houver o que salvar ali).
/// </summary>
public class SalvamentoJson : ISistemaDeSalvamento
{
    private static string CaminhoDoArquivo => Path.Combine(Application.persistentDataPath, "save.json");

    public bool ExisteSave() => File.Exists(CaminhoDoArquivo);

    public void Salvar()
    {
        GerenciadorDeEstado gerenciador = GerenciadorDeEstado.Instancia;
        Droid droid = gerenciador.DroidDoJogador;

        var dados = new DadosDoJogo
        {
            droid = new DroidSalvo
            {
                nome = droid.Nome,
                hp = droid.Hp,
                hpMax = droid.HpMax,
                statsFor = droid.StatsBase.For,
                statsAgi = droid.StatsBase.Agi,
                statsVit = droid.StatsBase.Vit,
                statsInt = droid.StatsBase.Int,
                statsDex = droid.StatsBase.Dex,
                statsLuk = droid.StatsBase.Luk,
                pontosDisponiveis = droid.Pontos.PontosDisponiveis
            },
            posicaoX = gerenciador.TemPosicaoSalva ? gerenciador.PosicaoSalva.x : 0,
            posicaoY = gerenciador.TemPosicaoSalva ? gerenciador.PosicaoSalva.y : 0,
            posicaoZ = 0,
            cena = gerenciador.TemPosicaoSalva
                ? gerenciador.CenaDeOrigemDaPosicao
                : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        };

        foreach (var tecnica in droid.TecnicasConfiguradas.Values)
        {
            dados.droid.tecnicas.Add(new TecnicaSalva { nome = tecnica.Nome, nivelDeDano = tecnica.NivelDeDano });
        }

        foreach (var kv in gerenciador.TodasAsFlags)
        {
            dados.flagsDeHistoria.Add(new FlagDeHistoria { chave = kv.Key, valor = kv.Value });
        }

        string json = JsonUtility.ToJson(dados, prettyPrint: true);
        File.WriteAllText(CaminhoDoArquivo, json);

        Debug.Log($"Jogo salvo em: {CaminhoDoArquivo}");
    }

    public void Carregar()
    {
        if (!ExisteSave())
        {
            Debug.LogWarning("Nenhum save encontrado em " + CaminhoDoArquivo);
            return;
        }

        string json = File.ReadAllText(CaminhoDoArquivo);
        DadosDoJogo dados = JsonUtility.FromJson<DadosDoJogo>(json);

        GerenciadorDeEstado gerenciador = GerenciadorDeEstado.Instancia;
        Droid droid = gerenciador.DroidDoJogador;

        droid.Nome = dados.droid.nome;
        droid.HpMax = dados.droid.hpMax;
        droid.Hp = dados.droid.hp;
        droid.StatsBase.For = dados.droid.statsFor;
        droid.StatsBase.Agi = dados.droid.statsAgi;
        droid.StatsBase.Vit = dados.droid.statsVit;
        droid.StatsBase.Int = dados.droid.statsInt;
        droid.StatsBase.Dex = dados.droid.statsDex;
        droid.StatsBase.Luk = dados.droid.statsLuk;
        droid.Pontos.DefinirPontos(dados.droid.pontosDisponiveis);

        droid.TecnicasConfiguradas.Clear();
        foreach (var t in dados.droid.tecnicas)
        {
            droid.TecnicasConfiguradas[t.nome] = new TecnicaComposta { Nome = t.nome, NivelDeDano = t.nivelDeDano };
        }

        var flags = new System.Collections.Generic.Dictionary<string, bool>();
        foreach (var f in dados.flagsDeHistoria)
        {
            flags[f.chave] = f.valor;
        }
        gerenciador.CarregarFlags(flags);

        gerenciador.SalvarPosicao(new Vector3(dados.posicaoX, dados.posicaoY, dados.posicaoZ), dados.cena);

        Debug.Log("Jogo carregado.");
    }
}
