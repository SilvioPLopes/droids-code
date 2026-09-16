using System.Collections.Generic;
using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Servico de quests. Estatico e sem estado proprio de propósito: TODO o
/// progresso mora no GerenciadorDeEstado (flags + contadores), que ja e
/// persistido pelo SalvamentoJson. Isso significa que uma quest sobrevive a
/// save/load sem nenhum codigo de serializacao novo.
///
/// Convencao de chaves (nao mudar sem migrar saves):
///   flag     "quest_{id}_ativa"
///   flag     "quest_{id}_concluida"
///   contador "quest_{id}_obj_{indice}"
///
/// Quem CHAMA RegistrarProgresso:
///   - BattleManager, ao vencer                 -> Derrotar
///   - GatilhoDeDialogo, ao terminar uma fala   -> Conversar
///   - VerificadorDePuzzle, ao resolver         -> ResolverPuzzle
///   - qualquer um que grave flag               -> AlcancarFlag (avaliado na hora)
///   - Coletar nao registra progresso: e lido direto do inventario, senao
///     o contador e o inventario poderiam divergir (vender o item deixaria
///     o objetivo "completo" com a bag vazia).
/// </summary>
public static class GerenciadorDeQuests
{
    public const string PrefixoFlagAtiva = "quest_";
    public const string SufixoAtiva = "_ativa";
    public const string SufixoConcluida = "_concluida";

    private static readonly ISistemaDeSalvamento Salvamento = new SalvamentoJson();

    // Invocado quando qualquer coisa muda (aceitar, progredir, entregar).
    // A UI do diario e o HUD de objetivo se inscrevem pra se redesenhar.
    public static System.Action AoMudarQuests;

    // ------------------------------------------------------------------
    // CONSULTA
    // ------------------------------------------------------------------

    public static string ChaveAtiva(string idQuest) => PrefixoFlagAtiva + idQuest + SufixoAtiva;
    public static string ChaveConcluida(string idQuest) => PrefixoFlagAtiva + idQuest + SufixoConcluida;
    public static string ChaveObjetivo(string idQuest, int indice) => PrefixoFlagAtiva + idQuest + "_obj_" + indice;

    public static bool EstaAtiva(string idQuest) =>
        GerenciadorDeEstado.Instancia.ObterFlag(ChaveAtiva(idQuest));

    public static bool EstaConcluida(string idQuest) =>
        GerenciadorDeEstado.Instancia.ObterFlag(ChaveConcluida(idQuest));

    /// <summary>
    /// A quest pode ser OFERECIDA agora? (pre-requisito cumprido, nao ativa,
    /// e nao concluida -- salvo se for repetivel).
    /// </summary>
    public static bool PodeSerOferecida(string idQuest)
    {
        DefinicaoDeQuest q = CatalogoDeQuests.Obter(idQuest);
        if (q == null) return false;
        if (EstaAtiva(idQuest)) return false;
        if (EstaConcluida(idQuest) && !q.Repetivel) return false;

        if (!string.IsNullOrWhiteSpace(q.FlagNecessaria) &&
            !GerenciadorDeEstado.Instancia.ObterFlag(q.FlagNecessaria))
        {
            return false;
        }

        return true;
    }

    /// <summary>Progresso atual de um objetivo (ja limitado a Quantidade).</summary>
    public static int ProgressoDoObjetivo(DefinicaoDeQuest q, int indice)
    {
        if (q == null || indice < 0 || indice >= q.Objetivos.Count) return 0;

        ObjetivoDeQuest obj = q.Objetivos[indice];
        var gerenciador = GerenciadorDeEstado.Instancia;

        switch (obj.Tipo)
        {
            case TipoDeObjetivo.Coletar:
                // Lido do inventario, nao de contador -- ver comentario da classe.
                return Mathf.Min(gerenciador.ObterQuantidadeDeItem(obj.Alvo), obj.Quantidade);

            case TipoDeObjetivo.AlcancarFlag:
                return gerenciador.ObterFlag(obj.Alvo) ? obj.Quantidade : 0;

            default:
                return Mathf.Min(gerenciador.ObterContador(ChaveObjetivo(q.Id, indice)), obj.Quantidade);
        }
    }

    public static bool ObjetivoCompleto(DefinicaoDeQuest q, int indice)
    {
        if (q == null || indice < 0 || indice >= q.Objetivos.Count) return false;
        return ProgressoDoObjetivo(q, indice) >= q.Objetivos[indice].Quantidade;
    }

    public static bool TodosOsObjetivosCompletos(string idQuest)
    {
        DefinicaoDeQuest q = CatalogoDeQuests.Obter(idQuest);
        if (q == null) return false;

        for (int i = 0; i < q.Objetivos.Count; i++)
        {
            if (!ObjetivoCompleto(q, i)) return false;
        }
        return true;
    }

    public static bool PodeEntregar(string idQuest) =>
        EstaAtiva(idQuest) && TodosOsObjetivosCompletos(idQuest);

    public static List<DefinicaoDeQuest> Ativas()
    {
        var lista = new List<DefinicaoDeQuest>();
        foreach (DefinicaoDeQuest q in CatalogoDeQuests.Todos.Values)
        {
            if (EstaAtiva(q.Id)) lista.Add(q);
        }
        return lista;
    }

    public static List<DefinicaoDeQuest> Concluidas()
    {
        var lista = new List<DefinicaoDeQuest>();
        foreach (DefinicaoDeQuest q in CatalogoDeQuests.Todos.Values)
        {
            if (EstaConcluida(q.Id)) lista.Add(q);
        }
        return lista;
    }

    // ------------------------------------------------------------------
    // ACOES
    // ------------------------------------------------------------------

    public static bool Aceitar(string idQuest)
    {
        if (!PodeSerOferecida(idQuest)) return false;

        DefinicaoDeQuest q = CatalogoDeQuests.Obter(idQuest);
        var gerenciador = GerenciadorDeEstado.Instancia;

        // Repetivel re-aceita: limpa a marca de concluida e zera contadores.
        if (q.Repetivel && EstaConcluida(idQuest))
        {
            gerenciador.DefinirFlag(ChaveConcluida(idQuest), false);
        }
        ZerarContadores(q);

        gerenciador.DefinirFlag(ChaveAtiva(idQuest), true);
        Salvamento.Salvar();
        AoMudarQuests?.Invoke();
        return true;
    }

    /// <summary>
    /// Aceita sozinha toda quest marcada como AceitaAutomaticamente cujo
    /// pre-requisito ja esteja cumprido. Chamado pelo ControladorDeRoteiro
    /// a cada troca de cena e sempre que uma flag de historia muda -- a
    /// linha principal nao pode depender de o jogador lembrar de aceitar.
    /// </summary>
    public static void AtualizarQuestsAutomaticas()
    {
        bool mudou = false;
        foreach (DefinicaoDeQuest q in CatalogoDeQuests.Todos.Values)
        {
            if (!q.AceitaAutomaticamente) continue;
            if (!PodeSerOferecida(q.Id)) continue;

            GerenciadorDeEstado.Instancia.DefinirFlag(ChaveAtiva(q.Id), true);
            ZerarContadores(q);
            mudou = true;
        }

        if (mudou)
        {
            Salvamento.Salvar();
            AoMudarQuests?.Invoke();
        }
    }

    /// <summary>
    /// Registra um evento de jogo em TODAS as quests ativas que esperam por
    /// ele. Seguro chamar com alvo desconhecido -- simplesmente nao casa
    /// com nada.
    /// </summary>
    public static void RegistrarProgresso(TipoDeObjetivo tipo, string alvo, int quantidade = 1)
    {
        if (string.IsNullOrEmpty(alvo) || quantidade <= 0) return;

        var gerenciador = GerenciadorDeEstado.Instancia;
        bool mudou = false;

        foreach (DefinicaoDeQuest q in CatalogoDeQuests.Todos.Values)
        {
            if (!EstaAtiva(q.Id)) continue;

            for (int i = 0; i < q.Objetivos.Count; i++)
            {
                ObjetivoDeQuest obj = q.Objetivos[i];
                if (obj.Tipo != tipo || obj.Alvo != alvo) continue;

                // Coletar/AlcancarFlag sao derivados (inventario/flag), nao
                // acumulados -- nao gravam contador.
                if (obj.Tipo == TipoDeObjetivo.Coletar || obj.Tipo == TipoDeObjetivo.AlcancarFlag) continue;

                string chave = ChaveObjetivo(q.Id, i);
                int atual = gerenciador.ObterContador(chave);
                if (atual >= obj.Quantidade) continue; // ja completo, nao inflaciona

                gerenciador.DefinirContador(chave, Mathf.Min(atual + quantidade, obj.Quantidade));
                mudou = true;
            }
        }

        if (mudou)
        {
            Salvamento.Salvar();
            AoMudarQuests?.Invoke();
        }
    }

    /// <summary>
    /// Entrega a quest: consome itens de coleta, paga XP/Gold/itens, grava
    /// a flag de conclusao. Devolve false se ainda nao pode entregar.
    /// </summary>
    public static bool Entregar(string idQuest)
    {
        if (!PodeEntregar(idQuest)) return false;

        DefinicaoDeQuest q = CatalogoDeQuests.Obter(idQuest);
        var gerenciador = GerenciadorDeEstado.Instancia;
        Droid droid = gerenciador.DroidDoJogador;

        // 1. Consome os itens de coleta. Feito ANTES de pagar, e com
        //    verificacao, pra nao existir o caso "pagou e o item sumiu".
        foreach (ObjetivoDeQuest obj in q.Objetivos)
        {
            if (obj.Tipo != TipoDeObjetivo.Coletar) continue;
            if (!gerenciador.TentarRemoverItem(obj.Alvo, obj.Quantidade))
            {
                Debug.LogWarning($"GerenciadorDeQuests: entrega de '{idQuest}' abortada -- faltou {obj.Alvo}.");
                return false;
            }
        }

        // 2. Recompensas.
        if (q.XpRecompensa > 0 && droid != null)
        {
            SistemaDeProgressao.GanharExperiencia(droid, q.XpRecompensa);
        }
        if (q.GoldRecompensa > 0)
        {
            gerenciador.AdicionarGold(q.GoldRecompensa);
        }
        foreach (RecompensaDeItem r in q.ItensRecompensa)
        {
            if (!string.IsNullOrEmpty(r.IdItem) && r.Quantidade > 0)
            {
                gerenciador.AdicionarItem(r.IdItem, r.Quantidade);
            }
        }

        // 3. Estado.
        gerenciador.DefinirFlag(ChaveAtiva(idQuest), false);
        gerenciador.DefinirFlag(ChaveConcluida(idQuest), true);
        if (!string.IsNullOrWhiteSpace(q.FlagAoConcluir))
        {
            gerenciador.DefinirFlag(q.FlagAoConcluir, true);
        }
        ZerarContadores(q);

        Salvamento.Salvar();
        AtualizarQuestsAutomaticas(); // a flag nova pode liberar a proxima
        AoMudarQuests?.Invoke();
        return true;
    }

    private static void ZerarContadores(DefinicaoDeQuest q)
    {
        if (q == null) return;
        for (int i = 0; i < q.Objetivos.Count; i++)
        {
            GerenciadorDeEstado.Instancia.DefinirContador(ChaveObjetivo(q.Id, i), 0);
        }
    }

    // ------------------------------------------------------------------
    // TEXTO (usado pelo diario e pelo HUD)
    // ------------------------------------------------------------------

    public static string DescreverObjetivo(DefinicaoDeQuest q, int indice)
    {
        ObjetivoDeQuest obj = q.Objetivos[indice];
        int progresso = ProgressoDoObjetivo(q, indice);
        bool completo = progresso >= obj.Quantidade;

        string marca = completo ? "[x]" : "[ ]";
        string contagem = obj.Quantidade > 1 ? $" ({progresso}/{obj.Quantidade})" : "";
        return $"{marca} {obj.Descricao}{contagem}";
    }

    public static string DescreverRecompensas(DefinicaoDeQuest q)
    {
        var partes = new List<string>();
        if (q.XpRecompensa > 0) partes.Add($"{q.XpRecompensa} XP");
        if (q.GoldRecompensa > 0) partes.Add($"{q.GoldRecompensa} Gold");

        foreach (RecompensaDeItem r in q.ItensRecompensa)
        {
            DefinicaoDeItem def = CatalogoDeItens.Obter(r.IdItem);
            string nome = def != null ? def.Nome : r.IdItem;
            partes.Add(r.Quantidade > 1 ? $"{nome} x{r.Quantidade}" : nome);
        }

        return partes.Count == 0 ? "—" : string.Join(", ", partes);
    }

    /// <summary>
    /// Objetivo atual pro HUD: a primeira quest principal ativa com objetivo
    /// incompleto. Devolve string vazia se nao houver nada pendente.
    /// </summary>
    public static string ObjetivoAtualParaHud()
    {
        foreach (DefinicaoDeQuest q in CatalogoDeQuests.Todos.Values)
        {
            if (!q.EhPrincipal || !EstaAtiva(q.Id)) continue;

            for (int i = 0; i < q.Objetivos.Count; i++)
            {
                if (!ObjetivoCompleto(q, i))
                {
                    return $"{q.Titulo}: {q.Objetivos[i].Descricao}";
                }
            }
            return $"{q.Titulo}: pronto para concluir";
        }
        return "";
    }
}
