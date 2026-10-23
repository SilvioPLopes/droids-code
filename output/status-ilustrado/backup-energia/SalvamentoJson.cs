using System;
using System.IO;
using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Salva/carrega em Application.persistentDataPath/save.json.
/// Cobre: Droid (stats, HP, pontos, tecnicas), posicao no mapa, cena, flags
/// de historia, inventario, Gold (Estagio 3) e (Historia/Quests) contadores
/// de progresso de objetivo.
/// </summary>
public class SalvamentoJson : ISistemaDeSalvamento
{
    private readonly string CaminhoDoArquivo;

    public SalvamentoJson() : this(Path.Combine(Application.persistentDataPath, "save.json")) { }

    public SalvamentoJson(string caminhoDoArquivo)
    {
        if (string.IsNullOrWhiteSpace(caminhoDoArquivo)) throw new ArgumentException("Informe o arquivo de save.");
        CaminhoDoArquivo = caminhoDoArquivo;
    }

    public bool ExisteSave() => File.Exists(CaminhoDoArquivo);

    /// <summary>
    /// Migra somente o DTO em memória. A flag v5 era compartilhada pela
    /// introdução e pela casa resolvida; o save não permite distingui-las.
    /// Preservamos o progresso legado e marcamos a introdução como vista.
    /// </summary>
    public static void MigrarDados(DadosDoJogo dados)
    {
        if (dados == null) throw new System.ArgumentNullException(nameof(dados));
        if (dados.versao >= 6) return;

        bool casaLegadaMarcada = false;
        if (dados.flagsDeHistoria != null)
        {
            foreach (var flag in dados.flagsDeHistoria)
            {
                if (flag != null && flag.chave == "ato1_casa_resolvida" && flag.valor)
                    casaLegadaMarcada = true;
            }

            if (casaLegadaMarcada)
            {
                bool encontrouIntroducao = false;
                foreach (var flag in dados.flagsDeHistoria)
                {
                    if (flag == null || flag.chave != "ato1_falou_apollo_casa") continue;
                    flag.valor = true;
                    encontrouIntroducao = true;
                }
                if (!encontrouIntroducao)
                    dados.flagsDeHistoria.Add(new FlagDeHistoria
                    {
                        chave = "ato1_falou_apollo_casa", valor = true
                    });
            }
        }
        dados.versao = 6;
    }

    public void Salvar()
    {
        string cenaAtual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        ValidarDestino(cenaAtual, Vector3.zero);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Vector3 posicaoAtual = playerObj != null ? playerObj.transform.position : Vector3.zero;
        SalvarCheckpoint(GerenciadorDeEstado.Instancia, cenaAtual, posicaoAtual);
    }

    /// <summary>Grava estado atual com destino explícito de exploração, inclusive ao encerrar Battle.</summary>
    public void SalvarCheckpoint(GerenciadorDeEstado gerenciador, string cena, Vector3 posicao)
    {
        DadosDoJogo dados = CriarDados(gerenciador, cena, posicao);
        string temporario = CaminhoDoArquivo + ".tmp";
        try
        {
            File.WriteAllText(temporario, JsonUtility.ToJson(dados, prettyPrint: true));
            if (File.Exists(CaminhoDoArquivo)) File.Replace(temporario, CaminhoDoArquivo, null);
            else File.Move(temporario, CaminhoDoArquivo);
        }
        finally
        {
            if (File.Exists(temporario)) File.Delete(temporario);
        }
        Debug.Log("Checkpoint salvo: " + cena);
    }

    public static DadosDoJogo CriarDados(GerenciadorDeEstado gerenciador, string cenaAtual, Vector3 posicaoAtual)
    {
        ValidarDestino(cenaAtual, posicaoAtual);
        if (gerenciador == null || gerenciador.DroidDoJogador == null)
            throw new InvalidOperationException("Estado do jogador indisponível para salvar.");
        Droid droid = gerenciador.DroidDoJogador;

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

        // NOVO (Historia/Quests): persistir os contadores de progresso de
        // objetivo junto -- sem isso, o progresso de uma quest em andamento
        // (ex: "3/5 parafusos coletados" via contador) se perdia a cada
        // save/load, mesmo com a quest continuando ativa (flag "_ativa").
        foreach (var kv in gerenciador.TodosOsContadores)
        {
            dados.contadores.Add(new ContadorDeQuest { chave = kv.Key, valor = kv.Value });
        }

        return dados;
    }

    static void ValidarDestino(string cena, Vector3 posicao)
    {
        if (cena == "Battle")
            throw new InvalidOperationException("Este save de batalha não possui um checkpoint de exploração. O arquivo foi preservado; use um backup de exploração para recuperar o progresso.");
        if (string.IsNullOrWhiteSpace(cena) || float.IsNaN(posicao.x) || float.IsInfinity(posicao.x)
            || float.IsNaN(posicao.y) || float.IsInfinity(posicao.y) || float.IsNaN(posicao.z) || float.IsInfinity(posicao.z))
            throw new InvalidOperationException("O save não contém uma cena e posição válidas.");
    }

    /// <summary>Lê/valida sem alterar singleton, cena ou arquivo; permite mostrar erro antes de fechar o menu.</summary>
    public DadosDoJogo LerDados()
    {
        var dados = JsonUtility.FromJson<DadosDoJogo>(File.ReadAllText(CaminhoDoArquivo));
        ValidarDados(dados);
        return dados;
    }

    static void ValidarDados(DadosDoJogo dados)
    {
        if (dados == null || dados.droid == null) throw new InvalidOperationException("Save incompleto: dados do jogador ausentes.");
        ValidarDestino(dados.cena, new Vector3(dados.posicaoX, dados.posicaoY, dados.posicaoZ));
        // CanStreamedLevelBeLoaded retorna false fora de Play no Editor.
        // A lista do build permite validar o arquivo sem uma partida ativa.
        bool cenaDisponivel = false;
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string caminho = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            if (Path.GetFileNameWithoutExtension(caminho) == dados.cena)
            {
                cenaDisponivel = true;
                break;
            }
        }
        if (!cenaDisponivel)
            throw new InvalidOperationException("A cena do save não está disponível: " + dados.cena);
    }

    public void Carregar()
    {
        if (!ExisteSave())
        {
            Debug.LogWarning("Nenhum save encontrado em " + CaminhoDoArquivo);
            return;
        }

        CarregarDados(LerDados());
    }

    public void CarregarDados(DadosDoJogo dados)
    {
        ValidarDados(dados);
        MigrarDados(dados);

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

        // COMPATIBILIDADE (v5 — Historia/Quests): saves anteriores nao tem
        // "contadores" -- JsonUtility preenche como lista vazia, entao o
        // progresso numerico de quests em andamento reseta (mas nao quebra
        // o load, e a flag "_ativa"/"_concluida" da quest continua valendo).
        var contadores = new System.Collections.Generic.Dictionary<string, int>();
        foreach (var contadorSalvo in dados.contadores)
        {
            contadores[contadorSalvo.chave] = contadorSalvo.valor;
        }
        gerenciador.CarregarContadores(contadores);

        gerenciador.SalvarPosicao(new Vector3(dados.posicaoX, dados.posicaoY, dados.posicaoZ), dados.cena);
        gerenciador.LimparBatalhaPendente();

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
