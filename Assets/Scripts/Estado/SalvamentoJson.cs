using System.IO;
using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Salva/carrega em Application.persistentDataPath/save.json.
/// Cobre: Droid (stats, HP, pontos, tecnicas), posicao no mapa, cena, flags
/// de historia, inventario e (Estagio 3) Gold.
/// </summary>
public class SalvamentoJson : ISistemaDeSalvamento
{
    private static string CaminhoDoArquivo => Path.Combine(Application.persistentDataPath, "save.json");

    public bool ExisteSave() => File.Exists(CaminhoDoArquivo);

    public void Salvar()
    {
        GerenciadorDeEstado gerenciador = GerenciadorDeEstado.Instancia;
        Droid droid = gerenciador.DroidDoJogador;

        // Posicao REAL e atual do Player agora — nao gerenciador.PosicaoSalva,
        // que e so uma variavel de transito Game->Battle->Game (fica vazia
        // quando o jogador so esta andando livre e clica em Salvar).
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Vector3 posicaoAtual = playerObj != null ? playerObj.transform.position : Vector3.zero;
        string cenaAtual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        var dados = new DadosDoJogo
        {
            droid = new DroidSalvo
            {
                nome = droid.Nome,
                hp = droid.Hp,
                hpMaxBase = droid.HpMaxBase,
                statsFor = droid.StatsBase.For,
                statsAgi = droid.StatsBase.Agi,
                statsVit = droid.StatsBase.Vit,
                statsInt = droid.StatsBase.Int,
                statsDex = droid.StatsBase.Dex,
                statsLuk = droid.StatsBase.Luk,
                pontosDisponiveis = droid.Pontos.PontosDisponiveis,
                nivel = droid.Progressao.Nivel,
                experiencia = droid.Progressao.ExperienciaAtual
            },
            posicaoX = posicaoAtual.x,
            posicaoY = posicaoAtual.y,
            posicaoZ = posicaoAtual.z,
            cena = cenaAtual,
            // Estagio 3 (fase 2 do sistema de item): persiste Gold junto.
            gold = gerenciador.Gold
        };

        // CORRECAO (12/09/2026): gravar tambem os efeitos da tecnica (antes
        // so nome/nivelDeDano eram salvos -- Veneno/Stun sumiam ao carregar).
        foreach (var tecnica in droid.TecnicasConfiguradas.Values)
        {
            var tecnicaSalva = new TecnicaSalva { nome = tecnica.Nome, nivelDeDano = tecnica.NivelDeDano };

            foreach (var efeito in tecnica.EfeitosDeAtributo)
            {
                tecnicaSalva.efeitosDeAtributo.Add(new EfeitoDeAtributoSalvo
                {
                    nomeExibicao = efeito.NomeExibicao,
                    atributo = (int)efeito.Atributo,
                    valor = efeito.Valor,
                    duracaoEmTurnos = efeito.DuracaoEmTurnos
                });
            }

            foreach (var efeito in tecnica.EfeitosDeDanoPorTurno)
            {
                tecnicaSalva.efeitosDeDanoPorTurno.Add(new EfeitoDeDanoPorTurnoSalvo
                {
                    nomeExibicao = efeito.NomeExibicao,
                    danoPorTurno = efeito.DanoPorTurno,
                    duracaoEmTurnos = efeito.DuracaoEmTurnos
                });
            }

            dados.droid.tecnicas.Add(tecnicaSalva);
        }

        foreach (var kv in gerenciador.TodasAsFlags)
        {
            dados.flagsDeHistoria.Add(new FlagDeHistoria { chave = kv.Key, valor = kv.Value });
        }

        // Estagio 2 (13/09/2026): persistir o inventario junto.
        foreach (var kv in gerenciador.Inventario)
        {
            dados.itens.Add(new ItemSalvo { id = kv.Key, quantidade = kv.Value });
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

        // COMPATIBILIDADE (Estagio 1 — 12/09/2026): saves v1 nao tinham
        // hpMaxBase (so hpMax, campo que nem existe mais em DroidSalvo).
        // JsonUtility preenche hpMaxBase com 0 nesse caso -- se vier 0,
        // assume que e um save antigo e mantem o HpMaxBase atual do Droid
        // (o valor default do construtor) em vez de zerar o teto de HP.
        if (dados.droid.hpMaxBase > 0)
        {
            droid.HpMaxBase = dados.droid.hpMaxBase;
        }

        droid.StatsBase.For = dados.droid.statsFor;
        droid.StatsBase.Agi = dados.droid.statsAgi;
        droid.StatsBase.Vit = dados.droid.statsVit;
        droid.StatsBase.Int = dados.droid.statsInt;
        droid.StatsBase.Dex = dados.droid.statsDex;
        droid.StatsBase.Luk = dados.droid.statsLuk;
        // Hp precisa ser restaurado DEPOIS de StatsBase.Vit e HpMaxBase,
        // porque droid.HpMax agora depende de VIT (calculado) -- restaurar
        // antes poderia truncar Hp contra um HpMax desatualizado.
        droid.Hp = System.Math.Min(dados.droid.hp, droid.HpMax);
        droid.Pontos.DefinirPontos(dados.droid.pontosDisponiveis);
        droid.Progressao.Definir(dados.droid.nivel, dados.droid.experiencia);

        // CORRECAO (12/09/2026): reconstruir tambem os efeitos salvos (antes
        // a tecnica voltava do save como ataque comum, sem Veneno/Stun).
        droid.TecnicasConfiguradas.Clear();
        foreach (var t in dados.droid.tecnicas)
        {
            var tecnica = new TecnicaComposta { Nome = t.nome, NivelDeDano = t.nivelDeDano };

            foreach (var efeitoSalvo in t.efeitosDeAtributo)
            {
                tecnica.EfeitosDeAtributo.Add(new EfeitoDeAtributo
                {
                    NomeExibicao = efeitoSalvo.nomeExibicao,
                    Atributo = (TipoAtributo)efeitoSalvo.atributo,
                    Valor = efeitoSalvo.valor,
                    DuracaoEmTurnos = efeitoSalvo.duracaoEmTurnos
                });
            }

            foreach (var efeitoSalvo in t.efeitosDeDanoPorTurno)
            {
                tecnica.EfeitosDeDanoPorTurno.Add(new EfeitoDeDanoPorTurno
                {
                    NomeExibicao = efeitoSalvo.nomeExibicao,
                    DanoPorTurno = efeitoSalvo.danoPorTurno,
                    DuracaoEmTurnos = efeitoSalvo.duracaoEmTurnos
                });
            }

            droid.TecnicasConfiguradas[t.nome] = tecnica;
        }

        var flags = new System.Collections.Generic.Dictionary<string, bool>();
        foreach (var f in dados.flagsDeHistoria)
        {
            flags[f.chave] = f.valor;
        }
        gerenciador.CarregarFlags(flags);

        // COMPATIBILIDADE (Estagio 2 — 13/09/2026): saves v1/v2 nao tem o
        // campo "itens" -- JsonUtility preenche como lista vazia nesse caso,
        // entao o inventario so fica vazio (nao quebra o load).
        var inventario = new System.Collections.Generic.Dictionary<string, int>();
        foreach (var itemSalvo in dados.itens)
        {
            inventario[itemSalvo.id] = itemSalvo.quantidade;
        }
        gerenciador.CarregarInventario(inventario);

        // COMPATIBILIDADE (Estagio 3 — fase 2 do sistema de item): saves
        // v1/v2/v3 nao tem "gold" -- JsonUtility preenche com 0, carrega
        // Gold zerado (aceitavel, mesma logica do inventario vazio acima).
        gerenciador.CarregarGold(dados.gold);

        gerenciador.SalvarPosicao(new Vector3(dados.posicaoX, dados.posicaoY, dados.posicaoZ), dados.cena);

        // Correcao do bug "personagem trava apos salvar/carregar": o
        // LoadScene abaixo destroi o MenuMundoManager da cena antiga sem
        // passar por RegistrarMenuFechado(), entao o contador de menus
        // abertos precisa ser zerado explicitamente aqui, antes da troca de
        // cena, ou MenuAberto fica true para sempre.
        gerenciador.ZerarMenusAbertos();

        // Recarrega a cena salva — isso faz o RestaurarPosicao.cs (que ja
        // existe no Player) pegar TemPosicaoSalva e teleportar sozinho,
        // reaproveitando o mecanismo do Game<->Battle em vez de duplicar
        // logica de teleporte aqui.
        UnityEngine.SceneManagement.SceneManager.LoadScene(dados.cena);

        Debug.Log("Jogo carregado.");
    }
}
