using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>Fixtures artificiais do serviço/catálogo. Não são percurso, input ou oferta por NPC real.</summary>
public static class ValidacaoContratosQuestsDemo
{
    const BindingFlags EstaticoPrivado = BindingFlags.Static | BindingFlags.NonPublic;

    sealed class Esperado
    {
        public readonly string Id;
        public readonly int Xp, Gold;
        public readonly bool Repetivel;
        public readonly Dictionary<string, int> Itens = new Dictionary<string, int>();
        public Esperado(string id, int xp, int gold, bool repetivel = false, string itens = "")
        {
            Id = id; Xp = xp; Gold = gold; Repetivel = repetivel;
            foreach (string item in itens.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] partes = item.Split('=');
                Itens.Add(partes[0], int.Parse(partes[1]));
            }
        }
    }

    // Valores esperados independentes da lógica de entrega; não calculados pelo produtor de recompensas.
    static readonly Esperado[] Casos =
    {
        new Esperado("q_main_agua", 120, 15),
        new Esperado("q_main_estacao", 400, 60),
        new Esperado("q_main_partida", 500, 0, itens: "kit_de_acampamento=1"),
        new Esperado("q_agua_dara", 90, 0, itens: "pocao_media=2"),
        new Esperado("q_dara_terceira", 160, 0, itens: "reparo_completo=1"),
        new Esperado("q_cobre_tacio", 80, 70, true),
        new Esperado("q_tacio_linha", 220, 40, itens: "sensor_optico=1"),
        new Esperado("q_perna_apollo", 200, 0, itens: "servo_motor=1"),
        new Esperado("q_limpeza_borda", 150, 45, true),
        new Esperado("q_pipo_peca", 60, 0, itens: "sinalizador_de_retorno=1"),
        new Esperado("q_onofre_provas", 180, 0, itens: "pocao_grande=1"),
        new Esperado("q_nilce_quarto", 140, 25, itens: "kit_de_acampamento=1"),
        new Esperado("q_ivo_terca", 130, 35),
        new Esperado("q_sula_divida", 170, 0, itens: "disruptor_emp=2;oleo_de_precisao=1"),
        new Esperado("q_guilda_1_iniciacao", 120, 30, itens: "emblema_da_guilda=1"),
        new Esperado("q_guilda_2_cacada", 250, 80, itens: "granada_de_acido=2"),
        new Esperado("q_guilda_3_alfa", 350, 120, itens: "pocao_grande=2;giroscopio=1"),
        new Esperado("q_guilda_4_verdade", 200, 0, itens: "chave_de_acesso=1")
    };

    sealed class SaveTemporario : ISistemaDeSalvamento
    {
        public string Caminho;
        public void Salvar() => new SalvamentoJson(Caminho).SalvarCheckpoint(
            GerenciadorDeEstado.Instancia, "City", new Vector3(3, 4, 0));
        public bool ExisteSave() => File.Exists(Caminho);
        public void Carregar() { throw new InvalidOperationException("Fixture não navega cenas."); }
    }

    static void Exigir(bool condicao, string mensagem)
    {
        if (!condicao) throw new InvalidOperationException(mensagem);
    }

    static int XpTotal(Droid droid) =>
        100 * (droid.Progressao.Nivel - 1) * droid.Progressao.Nivel / 2 + droid.Progressao.ExperienciaAtual;

    static string Resumo(GerenciadorDeEstado estado) =>
        estado.Gold + ":" + XpTotal(estado.DroidDoJogador) + ":" + estado.DroidDoJogador.Pontos.PontosDisponiveis
        + "|" + string.Join(";", estado.Inventario.OrderBy(p => p.Key).Select(p => p.Key + "=" + p.Value))
        + "|" + string.Join(";", estado.TodasAsFlags.OrderBy(p => p.Key).Select(p => p.Key + "=" + p.Value))
        + "|" + string.Join(";", estado.TodosOsContadores.OrderBy(p => p.Key).Select(p => p.Key + "=" + p.Value));

    static void PreencherObjetivo(DefinicaoDeQuest quest, int indice, bool completo)
    {
        var objetivo = quest.Objetivos[indice];
        var estado = GerenciadorDeEstado.Instancia;
        if (objetivo.Tipo == TipoDeObjetivo.AlcancarFlag)
            estado.DefinirFlag(objetivo.Alvo, completo);
        else if (objetivo.Tipo == TipoDeObjetivo.Coletar)
        {
            int desejado = completo ? objetivo.Quantidade + 2 : objetivo.Quantidade - 1;
            int faltante = desejado - estado.ObterQuantidadeDeItem(objetivo.Alvo);
            if (faltante > 0) estado.AdicionarItem(objetivo.Alvo, faltante);
        }
        else
        {
            int desejado = completo ? objetivo.Quantidade + 7 : objetivo.Quantidade - 1;
            int faltante = desejado - GerenciadorDeQuests.ProgressoDoObjetivo(quest, indice);
            if (faltante > 0) GerenciadorDeQuests.RegistrarProgresso(objetivo.Tipo, objetivo.Alvo, faltante);
        }
    }

    static void VerificarRecompensa(Esperado esperado, DefinicaoDeQuest quest, int entregas)
    {
        var estado = GerenciadorDeEstado.Instancia;
        Exigir(XpTotal(estado.DroidDoJogador) == esperado.Xp * entregas, "XP diferente da recompensa esperada.");
        Exigir(estado.Gold == esperado.Gold * entregas, "Gold diferente da recompensa esperada.");
        Exigir(estado.DroidDoJogador.Pontos.PontosDisponiveis == (estado.DroidDoJogador.Progressao.Nivel - 1) * 5,
            "Pontos por nível ausentes ou duplicados.");
        var inventarioEsperado = quest.Objetivos.Where(o => o.Tipo == TipoDeObjetivo.Coletar)
            .Select(o => o.Alvo).Distinct().ToDictionary(id => id, _ => 2);
        foreach (var item in esperado.Itens)
        {
            inventarioEsperado.TryGetValue(item.Key, out int anterior);
            inventarioEsperado[item.Key] = anterior + item.Value * entregas;
        }
        Exigir(inventarioEsperado.OrderBy(p => p.Key).SequenceEqual(estado.Inventario.OrderBy(p => p.Key)),
            "Consumo/recompensa de itens diferente do esperado; cada coleta deve deixar duas unidades excedentes.");
        Exigir(GerenciadorDeQuests.EstaConcluida(quest.Id) && !GerenciadorDeQuests.EstaAtiva(quest.Id),
            "Entrega não gravou conclusão ou manteve missão ativa.");
        if (!string.IsNullOrWhiteSpace(quest.FlagAoConcluir))
            Exigir(estado.ObterFlag(quest.FlagAoConcluir), "Entrega não aplicou a consequência da missão.");
    }

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", EstaticoPrivado);
        var campoSave = typeof(GerenciadorDeQuests).GetField("Salvamento", EstaticoPrivado);
        object estadoAnterior = singleton.GetValue(null), saveAnterior = campoSave.GetValue(null);
        var eventoAnterior = GerenciadorDeQuests.AoMudarQuests;
        var randomAnterior = UnityEngine.Random.state;
        var cena = EditorSceneManager.NewPreviewScene();
        string pasta = Path.Combine(Path.GetTempPath(), "DroidsCode-Contratos-Quests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pasta);
        var save = new SaveTemporario();
        var falhas = new List<string>();
        var aprovados = new List<string>();
        void Caso(string nome, Action acao)
        {
            try { acao(); aprovados.Add(nome); }
            catch (Exception e) { falhas.Add(nome + ": " + (e.InnerException ?? e).Message); }
        }
        try
        {
            campoSave.SetValue(null, save);
            Exigir(ReferenceEquals(campoSave.GetValue(null), save), "Não foi possível isolar a persistência.");
            GerenciadorDeQuests.AoMudarQuests = null;
            GerenciadorDeEstado EstadoNovo()
            {
                var objeto = new GameObject("Estado artificial de quest");
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(objeto, cena);
                var estado = objeto.AddComponent<GerenciadorDeEstado>();
                typeof(GerenciadorDeEstado).GetProperty("DroidDoJogador").SetValue(estado, new Droid("Fixture", 40));
                estado.CarregarFlags(new Dictionary<string, bool>());
                estado.CarregarContadores(new Dictionary<string, int>());
                estado.CarregarInventario(new Dictionary<string, int>());
                estado.CarregarGold(0);
                estado.ZerarMenusAbertos();
                singleton.SetValue(null, estado);
                return estado;
            }
            GerenciadorDeEstado Reabrir(GerenciadorDeEstado estado)
            {
                string anterior = Resumo(estado);
                byte[] arquivoAnterior = File.ReadAllBytes(save.Caminho);
                var dados = new SalvamentoJson(save.Caminho).LerDados();
                var novo = EstadoNovo();
                SalvamentoJson.AplicarDadosSemTrocarCena(dados, novo);
                Exigir(Resumo(novo) == anterior, "Nova instância perdeu estado persistido da missão/recompensas.");
                Exigir(arquivoAnterior.SequenceEqual(File.ReadAllBytes(save.Caminho)), "Leitura/restauração regravou o arquivo.");
                return novo;
            }
            Caso("cobertura do catálogo", () => Exigir(
                CatalogoDeQuests.Todos.Keys.OrderBy(id => id).SequenceEqual(Casos.Select(c => c.Id).OrderBy(id => id)),
                "A tabela de fixtures não cobre exatamente as 18 quests atuais."));

            foreach (var esperado in Casos)
            {
                Caso(esperado.Id, () =>
                {
                    var quest = CatalogoDeQuests.Obter(esperado.Id);
                    Exigir(quest != null && quest.Objetivos.Count > 0, "Missão inexistente ou sem objetivos.");
                    save.Caminho = Path.Combine(pasta, esperado.Id + ".json");
                    var estado = EstadoNovo();
                    Exigir(!string.IsNullOrWhiteSpace(quest.FlagNecessaria), "Fixture exige pré-requisito conhecido.");
                    string inicial = Resumo(estado);
                    Exigir(!GerenciadorDeQuests.PodeSerOferecida(quest.Id) && !GerenciadorDeQuests.Aceitar(quest.Id),
                        "Missão foi oferecida/aceita sem seu pré-requisito.");
                    Exigir(Resumo(estado) == inicial && !File.Exists(save.Caminho), "Aceite recusado alterou/grava estado.");
                    estado.DefinirFlag(quest.FlagNecessaria, true);
                    Exigir(GerenciadorDeQuests.PodeSerOferecida(quest.Id), "Pré-requisito não liberou a oferta.");
                    if (quest.AceitaAutomaticamente) GerenciadorDeQuests.AtualizarQuestsAutomaticas();
                    else Exigir(GerenciadorDeQuests.Aceitar(quest.Id), "Aceite válido foi recusado.");
                    Exigir(GerenciadorDeQuests.EstaAtiva(quest.Id), "Missão não ficou ativa.");
                    string aceita = Resumo(estado);
                    byte[] arquivoAceito = File.ReadAllBytes(save.Caminho);
                    Exigir(!GerenciadorDeQuests.Aceitar(quest.Id), "Aceite duplicado foi aceito.");
                    Exigir(!GerenciadorDeQuests.PodeEntregar(quest.Id) && !GerenciadorDeQuests.Entregar(quest.Id),
                        "Missão sem objetivos completos foi entregue.");
                    Exigir(Resumo(estado) == aceita && arquivoAceito.SequenceEqual(File.ReadAllBytes(save.Caminho)),
                        "Aceite/entrega recusados mudaram progresso, recompensas ou save.");

                    for (int i = 0; i < quest.Objetivos.Count; i++)
                        PreencherObjetivo(quest, i, i < quest.Objetivos.Count - 1);
                    string parcial = Resumo(estado);
                    Exigir(!GerenciadorDeQuests.PodeEntregar(quest.Id) && !GerenciadorDeQuests.Entregar(quest.Id),
                        "Objetivo final incompleto não impediu a entrega.");
                    Exigir(Resumo(estado) == parcial, "Entrega incompleta consumiu materiais ou pagou recompensa.");
                    save.Salvar();
                    estado = Reabrir(estado);
                    Exigir(GerenciadorDeQuests.EstaAtiva(quest.Id) && !GerenciadorDeQuests.PodeEntregar(quest.Id),
                        "Load alterou indevidamente a disponibilidade da entrega parcial.");
                    for (int i = 0; i < quest.Objetivos.Count; i++) PreencherObjetivo(quest, i, true);
                    for (int i = 0; i < quest.Objetivos.Count; i++)
                        Exigir(GerenciadorDeQuests.ProgressoDoObjetivo(quest, i) == quest.Objetivos[i].Quantidade,
                            "Progresso ausente ou inflado pelo excesso de eventos/itens.");
                    Exigir(GerenciadorDeQuests.PodeEntregar(quest.Id) && GerenciadorDeQuests.Entregar(quest.Id),
                        "Objetivos completos não permitiram a entrega.");
                    VerificarRecompensa(esperado, quest, 1);
                    string entregue = Resumo(estado);
                    byte[] arquivoEntregue = File.ReadAllBytes(save.Caminho);
                    Exigir(!GerenciadorDeQuests.Entregar(quest.Id), "Segunda entrega sem novo aceite foi aceita.");
                    Exigir(Resumo(estado) == entregue && arquivoEntregue.SequenceEqual(File.ReadAllBytes(save.Caminho)),
                        "Segunda entrega alterou estado ou regravou o save.");
                    estado = Reabrir(estado);
                    VerificarRecompensa(esperado, quest, 1);
                    Exigir(GerenciadorDeQuests.Concluidas().Any(q => q.Id == quest.Id)
                        && !GerenciadorDeQuests.Ativas().Any(q => q.Id == quest.Id), "Revisita perdeu a conclusão.");
                    Exigir(GerenciadorDeQuests.PodeSerOferecida(quest.Id) == esperado.Repetivel,
                        "Reoferta incompatível com as rodadas previstas na demo.");
                    if (!esperado.Repetivel)
                    {
                        Exigir(!GerenciadorDeQuests.Aceitar(quest.Id), "Missão única foi reaceita após load.");
                        return;
                    }
                    Exigir(GerenciadorDeQuests.Aceitar(quest.Id), "Nova rodada repetível não foi aceita.");
                    Exigir(!GerenciadorDeQuests.PodeEntregar(quest.Id) && !GerenciadorDeQuests.Entregar(quest.Id),
                        "Nova rodada reutilizou objetivos/itens da anterior sem novo trabalho.");
                    for (int i = 0; i < quest.Objetivos.Count; i++) PreencherObjetivo(quest, i, true);
                    Exigir(GerenciadorDeQuests.Entregar(quest.Id), "Nova rodada completa não foi entregue.");
                    VerificarRecompensa(esperado, quest, 2);
                    estado = Reabrir(estado);
                    VerificarRecompensa(esperado, quest, 2);
                    Exigir(!GerenciadorDeQuests.Entregar(quest.Id), "Nova rodada permitiu entrega duplicada após load.");
                });
            }
            Caso("assembleia ainda não encerra a demo", () =>
            {
                save.Caminho = Path.Combine(pasta, "partida-antecipada.json");
                var estado = EstadoNovo();
                estado.DefinirFlag("ato1_sessao6", true);
                GerenciadorDeQuests.AtualizarQuestsAutomaticas();
                estado.DefinirFlag("ato1_agua_voltou", true);
                GerenciadorDeQuests.RegistrarProgresso(TipoDeObjetivo.Conversar, "nadir");
                estado.DefinirFlag("ato1_cidade_respondeu", true);
                Exigir(!GerenciadorDeQuests.PodeEntregar("q_main_partida") && !GerenciadorDeQuests.Entregar("q_main_partida"),
                    "Água/Nadir/assembleia concluíram a missão antes da partida ao amanhecer.");
                Exigir(!estado.ObterFlag("ato1_concluido") && XpTotal(estado.DroidDoJogador) == 0
                    && estado.ObterQuantidadeDeItem("kit_de_acampamento") == 0, "Entrega antecipada deu consequência ou prêmio.");
            });
        }
        finally
        {
            campoSave.SetValue(null, saveAnterior);
            singleton.SetValue(null, estadoAnterior);
            GerenciadorDeQuests.AoMudarQuests = eventoAnterior;
            UnityEngine.Random.state = randomAnterior;
            EditorSceneManager.ClosePreviewScene(cena);
            Directory.Delete(pasta, true);
        }
        Exigir(falhas.Count == 0, "Contratos de quests — " + string.Join(" | ", falhas));
        return "18 quests: pré-requisito/oferta pelo serviço, aceite manual/automático e duplicado, entrega incompleta/completa, consumo, XP/Gold/itens únicos, revisita e carga em nova instância. "
            + "Duas repetíveis exigiram novos objetivos/itens; q_ivo_terca permaneceu única. Assembleia não encerrou a demo. "
            + "Fixtures com estado artificial e JSON temporário, sem input, NPCs reais, circulação ou acesso ao save pessoal. Casos: " + string.Join(", ", aprovados);
    }
}
