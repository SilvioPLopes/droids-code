using System;
using System.Linq;
using System.Reflection;
using DroidsCode.Combat;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEngine;

/// <summary>Combate em memória: não dispara gatilhos, não entra em Play e não grava saves.</summary>
public static class ValidacaoBossVale
{
    static void Exigir(bool ok, string mensagem)
    {
        if (!ok) throw new InvalidOperationException(mensagem);
    }

    static InimigoFixo CriarBoss()
    {
        var tipo = typeof(InimigoFixo).Assembly.GetType("DroidsCode.Combat.DroidDoIvo");
        Exigir(tipo != null, "FALHA: boss ainda não possui comportamento próprio DroidDoIvo.");
        return (InimigoFixo)Activator.CreateInstance(tipo, CatalogoDeInimigos.Obter(CatalogoDeInimigos.IdDroidDoIvo));
    }

    static string Proxima(InimigoFixo boss) => boss.ObterAcoesDisponiveis().First();
    static Droid Alvo() => new Droid("Fixture", 500) { StatsBase = new DroidStats { Vit = 5 } };

    [MenuItem("DroidsCode/Battle/Validar boss do vale (sem saves)")]
    public static void Menu() => Debug.Log(Executar());

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var randomAnterior = UnityEngine.Random.state;
        try
        {
            Ciclo();
            Efeitos();
            var taxas = Dificuldade();
            PrefabEArte();
            return "OK: ciclo, carga/stun, ações inválidas, dano/efeitos/resistência, prefab e arte; " + taxas;
        }
        finally { UnityEngine.Random.state = randomAnterior; }
    }

    static void Ciclo()
    {
        var boss = CriarBoss();
        var alvo = Alvo();
        var motor = new CombatEngine();
        var ciclo = new[] { "Prensa Hidráulica", "Pressurizar", "Corte de Contenção", "Descompressão" };
        Exigir(Proxima(boss) == ciclo[0], "Boss não inicia com prensa.");
        var invalido = boss.ExecutarAcao("inexistente", alvo);
        Exigir(!invalido.Sucesso && Proxima(boss) == ciclo[0], "Ação inválida avançou o ciclo.");
        boss.EfeitosAtivos.Add(new EfeitoDeAtributo { NomeExibicao = "Stun", DuracaoEmTurnos = 1 });
        var bloqueada = motor.ExecutarTurno(boss, alvo, Proxima(boss));
        Exigir(!bloqueada.Sucesso && Proxima(boss) == ciclo[0], "Stun não preservou intenção do boss.");
        for (int i = 0; i < 8; i++)
        {
            Exigir(Proxima(boss) == ciclo[i % 4], "Ciclo incorreto no passo " + i);
            var r = motor.ExecutarTurno(boss, alvo, Proxima(boss));
            Exigir(r.Mensagem.Contains(ciclo[(i + 1) % 4]), "Intenção seguinte ausente do log.");
            if (i % 4 == 1 || i % 4 == 3)
                Exigir(r.Sucesso && r.DanoCausado == 0, "Carga/descompressão causou dano.");
        }
        var carregado = CriarBoss();
        motor.ExecutarTurno(carregado, alvo, Proxima(carregado));
        motor.ExecutarTurno(carregado, alvo, Proxima(carregado));
        carregado.EfeitosAtivos.Add(new EfeitoDeAtributo { NomeExibicao = "Stun", DuracaoEmTurnos = 1 });
        motor.ExecutarTurno(carregado, alvo, Proxima(carregado));
        Exigir(Proxima(carregado) == ciclo[2], "Stun consumiu carga sem executar o corte.");
    }

    static void Efeitos()
    {
        bool acerto = false, erro = false, resistencia = false;
        for (int seed = 0; seed < 100 && !(acerto && erro && resistencia); seed++)
        {
            foreach (int resistir in new[] { 0, 100 })
            {
                var boss = CriarBoss();
                var alvo = Alvo(); alvo.StatsBase.Int = resistir;
                boss.ExecutarAcao(Proxima(boss), alvo);
                boss.ExecutarAcao(Proxima(boss), alvo);
                boss.ChanceCritica = 0;
                UnityEngine.Random.InitState(seed);
                var corte = boss.ExecutarAcao(Proxima(boss), alvo);
                if (!corte.Sucesso)
                {
                    erro = true;
                    Exigir(alvo.EfeitosDeDanoAtivos.Count == 0 && corte.DanoCausado == 0, "Corte errado aplicou dano/efeito.");
                }
                else
                {
                    Exigir(corte.DanoCausado == 25, "Corte não respeitou ATK30 - DEF5.");
                    if (resistir == 100)
                    {
                        resistencia = true;
                        Exigir(alvo.EfeitosDeDanoAtivos.Count == 0, "Resistência100 não bloqueou vazamento.");
                    }
                    else
                    {
                        acerto = true;
                        var efeito = alvo.EfeitosDeDanoAtivos.Single();
                        Exigir(efeito.DanoPorTurno == 2 && efeito.DuracaoEmTurnos == 2, "Vazamento incorreto.");
                        int hpAntes = alvo.Hp;
                        var motor = new CombatEngine();
                        motor.ExecutarTurno(alvo, boss, Droid.NomeAtaqueBasico);
                        motor.ExecutarTurno(alvo, boss, Droid.NomeAtaqueBasico);
                        Exigir(alvo.Hp == hpAntes - 4 && alvo.EfeitosDeDanoAtivos.Count == 0, "Vazamento não expirou após dois turnos.");
                    }
                }
            }
        }
        Exigir(acerto && erro && resistencia, "Não foram cobertos acerto, erro e resistência.");
    }

    static string Dificuldade()
    {
        int inicial = 0, preparado = 0;
        for (int seed = 0; seed < 200; seed++)
        {
            if (Lutar(seed, false)) inicial++;
            if (Lutar(seed, true)) preparado++;
        }
        Exigir(inicial <= 10, "Inicial venceu mais de5%: " + inicial);
        Exigir(preparado >= 140, "Preparado venceu menos de70%: " + preparado);
        return $"200 seeds: inicial={inicial}/200, preparado={preparado}/200 (sem itens).";
    }

    static bool Lutar(int seed, bool preparado)
    {
        UnityEngine.Random.InitState(seed);
        var jogador = new Droid("ApolloFixture", 30);
        jogador.StatsBase = preparado
            ? new DroidStats { For = 20, Vit = 15, Dex = 20, Agi = 10 }
            : new DroidStats { For = 5, Vit = 3 };
        jogador.Hp = jogador.HpMax;
        jogador.TecnicasConfiguradas["Corte Programado"] = new TecnicaComposta { Nome = "Corte Programado", NivelDeDano = 3 };
        var boss = CriarBoss();
        var motor = new CombatEngine();
        for (int turno = 0; turno < 300 && jogador.Hp > 0 && boss.Hp > 0; turno++)
        {
            string tecnica = preparado && jogador.EnergiaAtual >= jogador.ObterCustoDeEnergia("Corte Programado")
                ? "Corte Programado" : Droid.NomeAtaqueBasico;
            motor.ExecutarTurno(jogador, boss, tecnica);
            if (jogador.Hp <= 0 || boss.Hp <= 0) break;
            motor.ExecutarTurno(boss, jogador, Proxima(boss));
        }
        return jogador.Hp > 0 && boss.Hp <= 0;
    }

    static void PrefabEArte()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EventoBossVale.prefab");
        Exigir(prefab != null, "Prefab do evento ausente.");
        var g = prefab.GetComponent<GatilhoDeBatalha>();
        Exigir(g != null && g.idDoInimigo == CatalogoDeInimigos.IdDroidDoIvo && g.nomeCenaBatalha == "Battle", "Destino/ID do evento incorreto.");
        Exigir(g.exigirTecla && g.teclaDeInteracao == KeyCode.E && prefab.GetComponent<Collider2D>().isTrigger, "Evento não usa trigger/E.");
        Exigir(string.IsNullOrEmpty(g.flagDeVitoria) && g.flagsNecessarias.Length == 0 && g.flagsQueImpedem.Length == 0, "Evento criou flags narrativas.");
        Exigir(prefab.transform.position == Vector3.zero, "Prefab foi posicionado no mapa.");
        Exigir(Resources.Load<Sprite>("Inimigos/BossIvo-v2") != null, "Arte v2 do boss não carrega.");
    }
}
