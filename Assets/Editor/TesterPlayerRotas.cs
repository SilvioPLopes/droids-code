// Auditoria Editor explicitamente armada. Nao grava saves nem altera a geometria/controlador.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DroidsCode.Casa;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class TesterPlayer
{
    const string ArmadoRotas = "DroidsCode.TesterPlayer.Rotas.InicioSolicitado";
    const string RetomarRotas = ArmadoRotas + ".Retomar";
    const string PendenteRotas = "PENDENTE", AprovadoRotas = "PASSOU";
    static readonly string PastaRotas = Path.GetFullPath("output/apresentacao-jogavel/2026-11-12/circulacao");
    static string InventarioRotas => Path.Combine(PastaRotas, "inventario-colisores-ativos/inventario.json");
    static string CheckpointRotas => Path.Combine(Pasta, "save-todas-18-quests.json");
    static string PonteiroRotas => Path.Combine(PastaRotas, "resultado-atual.json");
    static string HistoricoRotas => Path.Combine(PastaRotas, "execucoes", relatorioRotas.idExecucao);
    static Task execucaoRotas;
    static DriverMovimentoTester driverRotas;
    static int versaoInicialRotas;
    static InventarioCirculacaoTester.InventarioDto inventarioRotas;
    static RelatorioRotas relatorioRotas = new RelatorioRotas();
    static readonly MethodInfo centroVisualRotas = typeof(MoradorInterativo).GetMethod("CentroVisual", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo jogadoresRotas = typeof(MoradorInterativo).GetField("jogadores", BindingFlags.Instance | BindingFlags.NonPublic);

    [Serializable] sealed class RelatorioRotas
    {
        public int versao = 1;
        public bool auditando;
        public string resultado = "NAO_INICIADO", idExecucao, retomadaDe, inicio, fim, cena, trechoAtual, inventarioSha256;
        public string checkpoint, metodo = "PlayerMovement real; grade .125m; setores .25m; sem input nativo";
        public int rotasPrevistas, rotasExecutadas, rotasAprovadas, rotasRestantes, transicoesExecutadas;
        public int ambientesPrevistos, ambientesVisitados, ambientesAprovados, alvosPrevistos, alvosAprovados, setoresPrevistos, setoresAprovados;
        public string[] restantes = Array.Empty<string>();
        public List<LigacaoRotas> ligacoes = new List<LigacaoRotas>();
        public List<AmbienteRotas> ambientes = new List<AmbienteRotas>();
        public List<TrechoRotas> trechos = new List<TrechoRotas>();
        public List<ErroRotas> erros = new List<ErroRotas>();
    }
    [Serializable] sealed class LigacaoRotas
    {
        public string id, origem, caminho, destino, tipo, estado = PendenteRotas;
        public float chegadaX, chegadaY;
        public int tentativas;
        public bool chegadaValidada;
    }
    [Serializable] sealed class AmbienteRotas
    {
        public string cena, estado = PendenteRotas, geometriaSha256, referenciaVisual;
        public bool visitado, alvosDescobertos, coberturaDescoberta;
        public int chegadasValidadas;
        public float minX, minY, maxX, maxY, raio, offsetX, offsetY;
        public List<AlvoRotas> alvos = new List<AlvoRotas>();
        public List<SetorRotas> setores = new List<SetorRotas>();
    }
    [Serializable] sealed class AlvoRotas
    {
        public string id, caminho, tipo, estado = PendenteRotas, resultado;
        public bool contatoReal, selecionavelPorE, interacaoAberta;
    }
    [Serializable] sealed class SetorRotas { public string id, estado = PendenteRotas; public float x, y; }
    [Serializable] sealed class TrechoRotas
    {
        public string id, execucao, cena, alvo, origem, resultado = "EM_ANDAMENTO", inicio, fim, bloqueador, precisao;
        public float inicioX, inicioY, destinoX, destinoY, finalX, finalY, chegadaEsperadaX, chegadaEsperadaY, deltaChegada;
        public bool contatoReal, poseEsperadaLivre, spawnOcupado;
        public string[] penetracoes;
        public int passos;
    }
    [Serializable] sealed class ErroRotas { public string execucao, instante, cena, alvo, bloqueador, origem, resultado; }
    [Serializable] sealed class LinhaMovimentoRotas { public string routeID; public DriverMovimentoTester.PassoFisico passo; }
    sealed class BloqueioRotas : Exception { public BloqueioRotas(string texto) : base(texto) { } }

    public static string EstadoRotas() => JsonUtility.ToJson(relatorioRotas, true);

    [InitializeOnLoadMethod] static void RegistrarRotas()
    {
        EditorApplication.playModeStateChanged -= MudouPlayRotas;
        EditorApplication.playModeStateChanged += MudouPlayRotas;
    }
    static async void MudouPlayRotas(PlayModeStateChange estado)
    {
        if (estado == PlayModeStateChange.ExitingPlayMode)
        {
            SessionState.SetBool(ArmadoRotas, false);
            if (driverRotas != null) driverRotas.Parar();
        }
        if (estado != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(ArmadoRotas, false)) return;
        bool retomar = SessionState.GetBool(RetomarRotas, false);
        SessionState.SetBool(ArmadoRotas, false);
        int versaoSolicitada = versaoCancelamento;
        await Task.Delay(250);
        if (!EditorApplication.isPlaying || versaoSolicitada != versaoCancelamento) return;
        execucaoRotas = ExecutarAuditoriaRotas(retomar);
    }

    [MenuItem("Droids Code/Testes/TesterPlayer/Auditar circulacao integral")]
    public static void PrepararAuditoriaRotas() { SessionState.SetBool(ChaveVisualRotas,false); PrepararRotas(false); }
    [MenuItem("Droids Code/Testes/TesterPlayer/Retomar auditoria de circulacao")]
    public static void PrepararRetomadaRotas() => PrepararRotas(true);
    static void PrepararRotas(bool retomar)
    {
        Exigir(!relatorio.executando && !relatorioRotas.auditando, "Outro TesterPlayer esta executando.");
        Exigir(File.Exists(InventarioRotas) && File.Exists(CheckpointRotas), "Inventario simplificado/checkpoint legitimo ausente.");
        if (retomar)
        {
            Exigir(File.Exists(PonteiroRotas), "Nao ha auditoria para retomar.");
            var anterior = JsonUtility.FromJson<RelatorioRotas>(File.ReadAllText(PonteiroRotas));
            Exigir(anterior.inventarioSha256 == HashRotas(File.ReadAllBytes(InventarioRotas)), "Inventario mudou: retomada recusada; inicie nova auditoria explicita.");
        }
        SessionState.SetBool(Armado, false); // O percurso principal/opcionais nunca e armado por esta ferramenta.
        ValidacaoPercursoApresentacao.CancelamentoSolicitado = false;
        if (EditorApplication.isPlaying)
        {
            Exigir(!EditorApplication.isPaused, "Retire a pausa antes de iniciar explicitamente.");
            execucaoRotas = ExecutarAuditoriaRotas(retomar);
            return;
        }
        for (int i = 0; i < SceneManager.sceneCount; i++) Exigir(!SceneManager.GetSceneAt(i).isDirty, "Cena com alteracoes nao salvas.");
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        SessionState.SetBool(RetomarRotas, retomar);
        SessionState.SetBool(ArmadoRotas, true);
        EditorApplication.isPlaying = true;
    }

    static async Task ExecutarAuditoriaRotas(bool retomar)
    {
        try { await ExecutarAuditoriaRotasInterna(retomar); }
        catch (Exception e) // Inclui falhas de IO/schema durante a preparacao, antes do primeiro trecho.
        {
            if (string.IsNullOrEmpty(relatorioRotas.idExecucao)) relatorioRotas.idExecucao = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + "-preparo";
            relatorioRotas.auditando = false; relatorioRotas.resultado = "PAUSADO_POR_ERRO"; relatorioRotas.fim = DateTime.UtcNow.ToString("O");
            LiberarDriverRotas();
            ErroAuditoriaRotas(SceneManager.GetActiveScene().name, relatorioRotas.trechoAtual, e.GetType().Name, "preparo/persistencia", e.ToString());
            if (EditorApplication.isPlaying) EditorApplication.isPaused = true;
            try { GravarRotas(); } catch (Exception io) { Debug.LogError("[TesterPlayerRotas] Falha ao persistir; evidencia em memoria/Console: " + io); }
            Debug.LogError("[TesterPlayerRotas] Estado preservado: " + e);
        }
    }
    static async Task ExecutarAuditoriaRotasInterna(bool retomar)
    {
        string id = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6);
        string hash = HashRotas(File.ReadAllBytes(InventarioRotas));
        inventarioRotas = JsonUtility.FromJson<InventarioCirculacaoTester.InventarioDto>(File.ReadAllText(InventarioRotas));
        if (retomar)
        {
            string snapshot = File.ReadAllText(PonteiroRotas);
            relatorioRotas = JsonUtility.FromJson<RelatorioRotas>(snapshot);
            Exigir(relatorioRotas.inventarioSha256 == hash, "Inventario mudou entre preparo e inicio.");
            string anterior = relatorioRotas.idExecucao;
            File.WriteAllText(Path.Combine(PastaRotas, "execucoes", anterior, "antes-retomada-" + id + ".json"), snapshot);
            relatorioRotas.retomadaDe = anterior;
            relatorioRotas.idExecucao = id;
            Directory.CreateDirectory(HistoricoRotas);
            File.WriteAllText(Path.Combine(HistoricoRotas, "retomado-de.json"), snapshot);
            foreach (var l in relatorioRotas.ligacoes.Where(l => l.estado != AprovadoRotas)) l.estado = PendenteRotas;
            foreach (var a in relatorioRotas.ambientes)
            {
                foreach (var t in a.alvos.Where(t => t.estado != AprovadoRotas)) t.estado = PendenteRotas;
                foreach (var s in a.setores.Where(s => s.estado != AprovadoRotas)) s.estado = PendenteRotas;
            }
        }
        else
        {
            relatorioRotas = new RelatorioRotas { idExecucao = id, inventarioSha256 = hash, checkpoint = CheckpointRotas };
            CriarGrafoRotas();
            Directory.CreateDirectory(HistoricoRotas);
        }
        relatorioRotas.auditando = true; relatorioRotas.resultado = "EXECUTANDO";
        if(FuncionalVisualRotas)relatorioRotas.metodo="PlayerMovement real; 68 ligacoes e todos os alvos; captura visual de salas corrigidas e prova de colisao; sem varredura de setores";
        relatorioRotas.inicio = DateTime.UtcNow.ToString("O"); relatorioRotas.fim = "";
        ValidacaoPercursoApresentacao.CancelamentoSolicitado = false; versaoInicialRotas = versaoCancelamento;
        GravarRotas();
        try
        {
            Exigir(centroVisualRotas != null && jogadoresRotas != null, "Contrato readonly de arbitragem Morador ausente.");
            await RecuperarCityRotas("inicio explicito pelo checkpoint legitimo");
            var cenasAuditadasNestaExecucao = new HashSet<string>();
            while (relatorioRotas.ligacoes.Any(l => l.estado == PendenteRotas))
            {
                CancelamentoRotas();
                Exigir(relatorioRotas.transicoesExecutadas < 200, "TETO_TRANSICOES: 200 transicoes no passeio.");
                string atual = SceneManager.GetActiveScene().name;
                if (atual != "City" && cenasAuditadasNestaExecucao.Add(atual)) await AuditarAmbienteRotas();
                var alvo = relatorioRotas.ligacoes.FirstOrDefault(l => l.estado == PendenteRotas && l.origem == atual)
                    ?? relatorioRotas.ligacoes.First(l => l.estado == PendenteRotas);
                if (alvo.origem != atual)
                {
                    var caminho = CaminhoGrafoRotas(atual, alvo.origem);
                    if (caminho == null)
                    {
                        await RecuperarCityRotas("recuperacao explicita para origem " + alvo.origem);
                        caminho = CaminhoGrafoRotas("City", alvo.origem);
                    }
                    if (caminho == null)
                    {
                        alvo.estado = "BLOQUEADA_POR_LIGACAO";
                        ErroAuditoriaRotas(alvo.origem, alvo.id, "grafo sem caminho fisicamente aprovado ou ainda verificavel", "BFS+recuperacao checkpoint", alvo.estado);
                        GravarRotas(); continue;
                    }
                    bool chegou = true;
                    foreach (var elo in caminho)
                    {
                        if (!await AtravessarLigacaoRotas(elo)) { chegou = false; break; }
                        string nome = SceneManager.GetActiveScene().name;
                        if (nome != "City") { cenasAuditadasNestaExecucao.Add(nome); await AuditarAmbienteRotas(); }
                    }
                    if (!chegou) { await RecuperarCityRotas("ligacao intermediaria bloqueada"); continue; }
                }
                if (!await AtravessarLigacaoRotas(alvo)) await RecuperarCityRotas("recuperacao apos ligacao bloqueada " + alvo.id);
                else if (SceneManager.GetActiveScene().name != "City")
                { cenasAuditadasNestaExecucao.Add(SceneManager.GetActiveScene().name); await AuditarAmbienteRotas(); }
            }
            if (SceneManager.GetActiveScene().name != "City" && cenasAuditadasNestaExecucao.Add(SceneManager.GetActiveScene().name)) await AuditarAmbienteRotas();
            // Retomar cobertura incompleta tambem quando seus portais ja passaram.
            foreach (var ambientePendente in relatorioRotas.ambientes.Where(a => a.estado != AprovadoRotas).ToArray())
            {
                CancelamentoRotas();
                var caminho = CaminhoGrafoRotas(SceneManager.GetActiveScene().name, ambientePendente.cena);
                if (caminho == null) continue; // Ligacao bloqueada continua pendencia explicita.
                bool chegou = true;
                foreach (var elo in caminho)
                    if (!await AtravessarLigacaoRotas(elo)) { chegou = false; break; }
                if (chegou) await AuditarAmbienteRotas();
            }
            GravarRotas();
            relatorioRotas.resultado = relatorioRotas.rotasRestantes == 0 && relatorioRotas.ambientesAprovados == relatorioRotas.ambientesPrevistos
                && !relatorioRotas.erros.Any(e => e.execucao == id) ? AprovadoRotas : "REPROVOU_COM_PENDENCIAS";
        }
        catch (OperationCanceledException e) { relatorioRotas.resultado = "CANCELADO"; ErroAuditoriaRotas(SceneManager.GetActiveScene().name, relatorioRotas.trechoAtual, "cancelamento", "orquestrador", e.Message); }
        catch (Exception e)
        {
            relatorioRotas.resultado = "PAUSADO_POR_ERRO";
            ErroAuditoriaRotas(SceneManager.GetActiveScene().name, relatorioRotas.trechoAtual, e.GetType().Name, "orquestrador", e.ToString());
            if (EditorApplication.isPlaying) EditorApplication.isPaused = true;
            Debug.LogError("[TesterPlayerRotas] Auditoria interrompida; Play e estado preservados. " + e);
        }
        finally
        {
            LiberarDriverRotas();
            foreach (var trecho in relatorioRotas.trechos.Where(t => t.resultado == "EM_ANDAMENTO"))
            { trecho.resultado = relatorioRotas.resultado; trecho.fim = DateTime.UtcNow.ToString("O"); trecho.bloqueador = "Auditoria interrompida; consultar erros preservados."; }
            relatorioRotas.auditando = false; relatorioRotas.fim = DateTime.UtcNow.ToString("O");
            GravarRotas();
        }
    }

    static void CriarGrafoRotas()
    {
        var ambientes = inventarioRotas.cenas.Where(c => c.nome == "CasaLip" || c.nome.StartsWith("Interior_", StringComparison.Ordinal)).ToArray();
        Exigir(ambientes.Length == 32, "Inventario deve conter 31 interiores + CasaLip; observado " + ambientes.Length);
        var nomes = new HashSet<string>(ambientes.Select(c => c.nome)) { "City" };
        foreach (var c in inventarioRotas.cenas.Where(c => nomes.Contains(c.nome)))
        {
            if (c.nome != "City")
            {
                var ambiente = new AmbienteRotas { cena = c.nome, geometriaSha256 = HashRotas(Encoding.UTF8.GetBytes(JsonUtility.ToJson(c))) };
                foreach (var n in c.npcs.Where(n => n.active && n.enabled && n.tipo == "MoradorInterativo"))
                    ambiente.alvos.Add(new AlvoRotas { id = n.caminho + "::MoradorInterativo", caminho = n.caminho, tipo = "MoradorInterativo" });
                foreach (var n in c.terminais.Where(n => n.active && n.enabled))
                    ambiente.alvos.Add(new AlvoRotas { id = n.caminho + "::TerminalDoInterior", caminho = n.caminho, tipo = "TerminalDoInterior" });
                foreach (var n in c.terminaisCasa.Where(n => n.active && n.enabled))
                    ambiente.alvos.Add(new AlvoRotas { id = n.caminho + "::InteragirComTerminalCasa", caminho = n.caminho, tipo = "InteragirComTerminalCasa" });
                relatorioRotas.ambientes.Add(ambiente);
            }
            foreach (var p in c.portais.Where(p => p.active && p.enabled && nomes.Contains(p.destino)))
                relatorioRotas.ligacoes.Add(new LigacaoRotas { id = c.nome + "::" + p.caminho, origem = c.nome, caminho = p.caminho,
                    destino = p.destino, chegadaX = p.pontoChegadaX, chegadaY = p.pontoChegadaY, tipo = "TRIGGER_REAL" });
            foreach (var n in c.npcs.Where(n => n.tipo == "MoradorInterativo" && n.active && n.enabled && nomes.Contains(n.destinoDeVisita ?? "")))
                relatorioRotas.ligacoes.Add(new LigacaoRotas { id = c.nome + "::" + n.caminho + "::visita", origem = c.nome, caminho = n.caminho,
                    destino = n.destinoDeVisita, chegadaX = n.chegadaDaVisitaX, chegadaY = n.chegadaDaVisitaY, tipo = "DIALOGO_REAL" });
        }
        Exigir(relatorioRotas.ligacoes.Select(l => l.id).Distinct().Count() == relatorioRotas.ligacoes.Count, "Identidade de portal duplicada no inventario.");
    }
    static List<LigacaoRotas> CaminhoGrafoRotas(string origem, string destino)
    {
        var fila = new Queue<string>(); fila.Enqueue(origem);
        var pais = new Dictionary<string, LigacaoRotas> { [origem] = null };
        while (fila.Count > 0)
        {
            string atual = fila.Dequeue(); if (atual == destino) break;
            foreach (var l in relatorioRotas.ligacoes.Where(l => l.origem == atual && (l.estado == PendenteRotas || l.estado == AprovadoRotas)))
                if (!pais.ContainsKey(l.destino)) { pais[l.destino] = l; fila.Enqueue(l.destino); }
        }
        if (!pais.ContainsKey(destino)) return null;
        var lista = new List<LigacaoRotas>();
        for (string c = destino; c != origem; c = pais[c].origem) lista.Add(pais[c]);
        lista.Reverse(); return lista;
    }
    static async Task RecuperarCityRotas(string motivo)
    {
        CancelamentoRotas(); LiberarDriverRotas();
        var t = NovoTrechoRotas("CHECKPOINT_RECUPERACAO", motivo, Vector2.zero);
        new SalvamentoJson(CheckpointRotas).Carregar();
        await AguardarRotas(() => SceneManager.GetActiveScene().name == "City" && PlayerRotas() != null && G != null, "Checkpoint nao abriu City/Player.");
        await Task.Delay(140); CancelamentoRotas();
        Exigir(!G.MenuAberto && PlayerRotas().GetComponent<PlayerMovement>().enabled, "Checkpoint abriu sem controle livre.");
        t.resultado = "RECUPERADO_EXPLICITAMENTE_NAO_E_PORTAL"; FinalizarTrechoRotas(t);
    }

    static async Task<bool> AtravessarLigacaoRotas(LigacaoRotas l)
    {
        CancelamentoRotas(); Exigir(SceneManager.GetActiveScene().name == l.origem, "Origem real diverge do grafo: " + l.id);
        Exigir(relatorioRotas.transicoesExecutadas < 200, "TETO_TRANSICOES: 200 transicoes no passeio.");
        l.tentativas++; var t = NovoTrechoRotas(l.id, l.origem, new Vector2(l.chegadaX, l.chegadaY));
        t.chegadaEsperadaX = l.chegadaX; t.chegadaEsperadaY = l.chegadaY; GravarRotas();
        try
        {
            if (l.tipo == "DIALOGO_REAL")
            {
                var npc = ComponentesRotas<MoradorInterativo>().SingleOrDefault(n => CaminhoRotas(n.transform) == l.caminho);
                if (npc == null || !npc.isActiveAndEnabled) throw new BloqueioRotas("Morador da visita ausente/inativo: " + l.caminho);
                await AlcancarNpcRotas(npc, t);
                if (npc.batalha != null && npc.batalha.PodeDisparar()) throw new BloqueioRotas("VISITA_COM_BATALHA_ELEGIVEL_NAO_ACIONADA");
                npc.Interagir(); await Dialogo("Conhecer a oficina anexa");
            }
            else
            {
                var portal = ComponentesRotas<TransicaoDeCena>().SingleOrDefault(p => CaminhoRotas(p.transform) == l.caminho);
                if (portal == null || !portal.isActiveAndEnabled || portal.nomeCenaDestino != l.destino) throw new BloqueioRotas("Portal diverge do inventario: " + l.caminho);
                if (portal.trancada) throw new BloqueioRotas("PORTAL_TRANCADO: " + l.caminho);
                var contato = TriggersRotas(portal).FirstOrDefault();
                if (contato == null) throw new BloqueioRotas("PORTAL_SEM_TRIGGER: " + l.caminho);
                await IrAteRotas(contato.bounds.center, contato, t, l.destino);
            }
            await AguardarRotas(() => SceneManager.GetActiveScene().name != l.origem, "Portal nao produziu transicao real.");
            Exigir(SceneManager.GetActiveScene().name == l.destino, "TRANSICAO_INESPERADA: " + SceneManager.GetActiveScene().name + " esperado " + l.destino);
            // A transicao observada e o callback real; o Player/trigger anterior pode ter sido destruido no mesmo tick.
            t.contatoReal = true;
            relatorioRotas.transicoesExecutadas++;
            await AguardarRotas(() => PlayerRotas() != null && PlayerRotas().GetComponent<PlayerMovement>() != null, "Chegada sem Player.");
            await Task.Delay(140); CancelamentoRotas(); // RestaurarPosicao executa em Start, depois do callback de cena.
            var p = PlayerRotas(); var mapa = MapaRotas(p, p.transform.position);
            Vector2 esperado = new Vector2(l.chegadaX, l.chegadaY);
            t.poseEsperadaLivre = !mapa.Ocupado(esperado); t.spawnOcupado = mapa.Ocupado(p.transform.position);
            t.penetracoes = mapa.Penetracoes(.005f); t.deltaChegada = Vector2.Distance(p.transform.position, esperado);
            if (!t.poseEsperadaLivre || t.spawnOcupado || t.penetracoes.Length > 0 || t.deltaChegada > .15f)
                throw new BloqueioRotas("CHEGADA_INVALIDA: delta=" + t.deltaChegada + "; esperadoLivre=" + t.poseEsperadaLivre + "; spawnOcupado=" + t.spawnOcupado + "; " + string.Join(";", t.penetracoes));
            l.chegadaValidada = true; l.estado = AprovadoRotas; t.resultado = AprovadoRotas;
            var ambiente = relatorioRotas.ambientes.FirstOrDefault(a => a.cena == l.destino);
            if (ambiente != null) { ambiente.visitado = true; ambiente.chegadasValidadas++; }
            FinalizarTrechoRotas(t); return true;
        }
        catch (BloqueioRotas e)
        {
            l.estado = "BLOQUEADA"; t.resultado = "BLOQUEADA"; t.bloqueador = e.Message;
            ErroAuditoriaRotas(l.origem, l.id, e.Message, l.caminho, "BLOQUEADA"); FinalizarTrechoRotas(t); return false;
        }
        catch (TimeoutException e)
        {
            l.estado = "BLOQUEADA"; t.resultado = "BLOQUEADA"; t.bloqueador = e.Message;
            ErroAuditoriaRotas(l.origem, l.id, e.Message, l.caminho, "BLOQUEADA_TIMEOUT"); FinalizarTrechoRotas(t); return false;
        }
    }

    static async Task AuditarAmbienteRotas()
    {
        string cena = SceneManager.GetActiveScene().name;
        var a = relatorioRotas.ambientes.Single(x => x.cena == cena); a.visitado = true;
        var player = PlayerRotas(); var mapa = MapaRotas(player, player.transform.position);
        // Aprovacoes de cobertura podem ser reutilizadas somente com inventario identico.
        if (!a.coberturaDescoberta && !FuncionalVisualRotas)
        {
            var representantes = mapa.Cobertura(player.transform.position, .25f);
            a.setores = representantes.Select(p => new SetorRotas { id = ChaveSetorRotas(mapa, p), x = p.x, y = p.y }).ToList();
            a.coberturaDescoberta = true;
        }
        var alvos = ComponentesRotas<MoradorInterativo>().Where(n => n.isActiveAndEnabled).Cast<Behaviour>()
            .Concat(ComponentesRotas<TerminalDoInterior>().Where(n => n.isActiveAndEnabled))
            .Concat(ComponentesRotas<InteragirComTerminalCasa>().Where(n => n.isActiveAndEnabled))
            .Concat(ComponentesRotas<GatilhoDeDialogo>().Where(n => n.isActiveAndEnabled)).ToArray();
        foreach (var alvo in alvos)
        {
            string id = CaminhoRotas(alvo.transform) + "::" + alvo.GetType().Name;
            if (!a.alvos.Any(x => x.id == id)) a.alvos.Add(new AlvoRotas { id = id, caminho = CaminhoRotas(alvo.transform), tipo = alvo.GetType().Name });
        }
        a.alvosDescobertos = true; GravarRotas();
        foreach (var alvo in alvos)
        {
            CancelamentoRotas(); string id = CaminhoRotas(alvo.transform) + "::" + alvo.GetType().Name;
            var registro = a.alvos.Single(x => x.id == id);
            // Reentrada confirma novamente alvos; uma cobertura aprovada nao e refeita.
            var t = NovoTrechoRotas(id, cena, alvo.transform.position);
            try
            {
                var npc = alvo as MoradorInterativo;
                if (npc != null)
                {
                    await AlcancarNpcRotas(npc, t); registro.selecionavelPorE = true;
                    if (npc.batalha != null && npc.batalha.PodeDisparar()) throw new BloqueioRotas("NPC_COM_BATALHA_ELEGIVEL_NAO_ACIONADA");
                    npc.Interagir(); await Task.Delay(80); CancelamentoRotas();
                    registro.interacaoAberta = DialogoUIManager.Instancia != null && DialogoUIManager.Instancia.EstaAberto;
                    if (!registro.interacaoAberta) throw new BloqueioRotas("NPC_NAO_ABRIU_INTERACAO");
                    await FecharDialogoRotas();
                }
                else
                {
                    var contato = TriggersRotas(alvo).FirstOrDefault();
                    if (contato == null) throw new BloqueioRotas("ALVO_SEM_TRIGGER_ATIVO");
                    await IrAteRotas(contato.bounds.center, contato, t);
                    if (!TocandoRotas(contato)) throw new BloqueioRotas("CONTATO_REAL_NAO_CONFIRMADO");
                    if (alvo is TerminalDoInterior interior)
                    {
                        if (!string.IsNullOrWhiteSpace(interior.flagNecessaria) && !G.ObterFlag(interior.flagNecessaria)) registro.resultado = "CONTATO_ALCANCADO_REQUISITO_INATIVO";
                        else
                        {
                            registro.interacaoAberta = interior.TentarInteragir();
                            if (!registro.interacaoAberta) throw new BloqueioRotas("TERMINAL_NAO_ABRIU_APOS_CONTATO");
                            UnityEngine.Object.FindAnyObjectByType<TerminalUIManager>(FindObjectsInactive.Include).Fechar();
                        }
                    }
                    else if (alvo is InteragirComTerminalCasa)
                    {
                        var perto = alvo.GetType().GetField("_playerPorPerto", BindingFlags.Instance | BindingFlags.NonPublic);
                        Exigir(perto != null && (bool)perto.GetValue(alvo), "TerminalCasa nao recebeu callback real de proximidade.");
                        var terminal = UnityEngine.Object.FindAnyObjectByType<TerminalUIManager>(FindObjectsInactive.Include);
                        Exigir(terminal != null, "TerminalCasa sem UI."); terminal.Abrir(); registro.interacaoAberta = terminal.EstaAberto; terminal.Fechar();
                        if (!registro.interacaoAberta) throw new BloqueioRotas("TERMINAL_CASA_NAO_ABRIU");
                    }
                    else if (alvo is GatilhoDeDialogo gatilho)
                    {
                        if (gatilho.PodeDisparar() || gatilho.PodeDispararGenerico())
                        { gatilho.TentarInteragir(); registro.interacaoAberta = DialogoUIManager.Instancia != null && DialogoUIManager.Instancia.EstaAberto; await FecharDialogoRotas(); }
                        else registro.resultado = "CONTATO_ALCANCADO_DIALOGO_INELEGIVEL_NO_CHECKPOINT";
                    }
                }
                registro.contatoReal = true; registro.estado = AprovadoRotas; t.resultado = AprovadoRotas;
                if (string.IsNullOrEmpty(registro.resultado)) registro.resultado = "CONTATO_E_INTERACAO_CONFIRMADOS";
            }
            catch (BloqueioRotas e)
            {
                registro.estado = "BLOQUEADO"; registro.resultado = e.Message; t.resultado = "BLOQUEADO"; t.bloqueador = e.Message;
                ErroAuditoriaRotas(cena, id, e.Message, "alvo fisico/arbitragem real", "BLOQUEADO");
            }
            finally { FinalizarTrechoRotas(t); }
        }
        if(FuncionalVisualRotas)
        {
            await CapturarVisualRotas(cena);
            if(cena=="Interior_E03_Terreo" && !provaMovelVisualRotas)
            { await ProvarMovelVisualRotas(mapa); provaMovelVisualRotas=true; }
            a.estado=a.alvos.All(x=>x.estado==AprovadoRotas)?AprovadoRotas:"PENDENCIAS";
            GravarRotas();return;
        }
        // Um passeio por representantes de TODOS os setores da componente, incluindo cantos.
        var tentados = new HashSet<string>();
        while (a.setores.Any(s => s.estado != AprovadoRotas && !tentados.Contains(s.id)))
        {
            CancelamentoRotas();
            var s = a.setores.Where(x => x.estado != AprovadoRotas && !tentados.Contains(x.id))
                .OrderBy(x => ((Vector2)PlayerRotas().transform.position - new Vector2(x.x, x.y)).sqrMagnitude).First();
            tentados.Add(s.id); var t = NovoTrechoRotas("setor:" + s.id, cena, new Vector2(s.x, s.y));
            try
            {
                await IrAteRotas(new Vector2(s.x, s.y), null, t, null, a, mapa);
                MarcarSetorRotas(a, mapa, PlayerRotas().transform.position);
                if (s.estado != AprovadoRotas) throw new BloqueioRotas("SETOR_NAO_VISITADO_FISICAMENTE");
                t.resultado = AprovadoRotas;
            }
            catch (BloqueioRotas e)
            {
                s.estado = "BLOQUEADO"; t.resultado = "BLOQUEADO"; t.bloqueador = e.Message;
                ErroAuditoriaRotas(cena, "setor:" + s.id, e.Message, "passeio fisico .25m", "BLOQUEADO");
                if(mapa.Ocupado(PlayerRotas().transform.position))
                {
                    try { await SepararContatoRasoRotas(PlayerRotas(),mapa,t); }
                    catch(BloqueioRotas contatoRaso)
                    { t.precisao += "; cobertura interrompida: " + contatoRaso.Message; break; }
                    if(mapa.Ocupado(PlayerRotas().transform.position))break;
                }
            }
            finally { FinalizarTrechoRotas(t); }
        }
        a.estado = a.alvos.All(x => x.estado == AprovadoRotas) && a.setores.Count > 0 && a.setores.All(x => x.estado == AprovadoRotas) ? AprovadoRotas : "PENDENCIAS";
        GravarRotas();
    }

    static async Task AlcancarNpcRotas(MoradorInterativo npc, TrechoRotas t)
    {
        var contato = TriggersRotas(npc).FirstOrDefault();
        if (contato == null) throw new BloqueioRotas("NPC_SEM_TRIGGER_ATIVO");
        await IrAteRotas(contato.bounds.center, contato, t);
        if (NpcEscolhidoRotas() == npc) return;
        var mapa = MapaRotas(PlayerRotas(), npc.transform.position);
        Vector2 centro = (Vector3)centroVisualRotas.Invoke(npc, null);
        for (int i = 0; i < 8; i++)
        {
            Vector2 ponto = centro + new Vector2(Mathf.Cos(i * Mathf.PI / 4), Mathf.Sin(i * Mathf.PI / 4)) * .7f;
            var rota = mapa.Planejar(PlayerRotas().transform.position, ponto);
            if (!rota.sucesso) continue;
            var alternativa = NovoTrechoRotas(t.alvo + "::selecao-E-" + i, t.cena, ponto);
            try { await MoverRotas(mapa, rota, ponto, null, alternativa, null); alternativa.resultado = AprovadoRotas; }
            catch (BloqueioRotas e) { alternativa.resultado = "BLOQUEADO"; alternativa.bloqueador = e.Message; }
            finally { FinalizarTrechoRotas(alternativa); }
            if (TocandoRotas(contato) && NpcEscolhidoRotas() == npc) return;
        }
        throw new BloqueioRotas("NPC_NAO_SELECIONAVEL_POR_E: " + npc.idNpc);
    }
    static MoradorInterativo NpcEscolhidoRotas()
    {
        var corpo = CorpoRotas();
        return ComponentesRotas<MoradorInterativo>().Where(n => n.isActiveAndEnabled
                && ((HashSet<Collider2D>)jogadoresRotas.GetValue(n)).Any(c => c != null))
            .OrderBy(n => ((Vector3)centroVisualRotas.Invoke(n, null) - corpo.bounds.center).sqrMagnitude)
            .ThenBy(n => n.idNpc, StringComparer.Ordinal).FirstOrDefault();
    }
    static async Task FecharDialogoRotas()
    {
        if (DialogoUIManager.Instancia == null || !DialogoUIManager.Instancia.EstaAberto) return;
        await Dialogo();
        if (Escolhas().Length > 0)
        {
            if (!Escolhas().Any(b => Texto(b) == "Até mais")) throw new BloqueioRotas("DIALOGO_SEM_SAIDA_SEGURA: " + string.Join(";", Escolhas().Select(Texto)));
            await Dialogo("Até mais");
        }
        Exigir(!DialogoUIManager.Instancia.EstaAberto && !G.MenuAberto, "Dialogo deixou menu aberto; estado preservado.");
    }

    static async Task IrAteRotas(Vector2 destino, Collider2D contato, TrechoRotas t, string cenaPermitida = null,
        AmbienteRotas ambiente = null, AnaliseCirculacaoTester.Mapa mapaExistente = null)
    {
        string origem = SceneManager.GetActiveScene().name;
        // City e grande demais para uma grade global .125m: janelas fisicas locais, nunca teleport.
        for (int janela = 0; janela < 16; janela++)
        {
            CancelamentoRotas(); var player = PlayerRotas(); Vector2 pos = player.transform.position;
            bool local = origem == "City" && Vector2.Distance(pos, destino) > 7f;
            Vector2 meta = local ? pos + (destino - pos).normalized * 6f : destino;
            var mapa = mapaExistente ?? MapaRotas(player, meta);
            await SepararContatoRasoRotas(player, mapa, t);
            pos = player.transform.position;
            var rota = mapa.Planejar(pos, meta, local ? null : contato, margemContato: .1f);
            if (!rota.sucesso && local)
            {
                // Uma busca local alternativa declarada, ainda com corpo integral e todas as arestas varridas.
                foreach (float angulo in new[] { 45f, -45f, 90f, -90f })
                {
                    Vector2 direcao = Quaternion.Euler(0, 0, angulo) * (Vector3)(destino - pos).normalized;
                    Vector2 candidato = pos + direcao * 4f;
                    var alternativo = MapaRotas(player, candidato);
                    var caminho = alternativo.Planejar(pos, candidato);
                    if (!caminho.sucesso) continue;
                    mapa = alternativo; rota = caminho; meta = candidato; t.precisao = "City: desvio local fisico " + angulo; break;
                }
            }
            if (!rota.sucesso && !local && contato != null && rota.erro == "CONTATO_SEM_MARGEM")
            {
                t.precisao = "alternativa unica declarada: margem 0m; contato REAL obrigatorio";
                rota = mapa.Planejar(pos, meta, contato, margemContato: 0f);
            }
            if (!rota.sucesso) throw new BloqueioRotas(rota.erro + ": " + string.Join(";", rota.bloqueadores));
            try { await MoverRotas(mapa, rota, meta, local ? null : contato, t, cenaPermitida, ambiente); }
            catch(BloqueioRotas localBloqueado) when(origem=="City" && SceneManager.GetActiveScene().name==origem)
            {
                // Uma alternativa apos a falha local demonstrada; nao refaz a cidade a cada janela.
                var fundo = ComponentesRotas<SpriteRenderer>().Where(s=>s.enabled && s.sprite!=null && !s.transform.IsChildOf(player.transform))
                    .OrderByDescending(s=>s.bounds.size.x*s.bounds.size.y).First();
                var global = AnaliseCirculacaoTester.Criar(player,fundo.bounds,.25f);
                await SepararContatoRasoRotas(player,global,t);
                var caminhoGlobal = global.Planejar(player.transform.position,destino,contato,.1f);
                if(!caminhoGlobal.sucesso && contato!=null && caminhoGlobal.erro=="CONTATO_SEM_MARGEM")
                    caminhoGlobal=global.Planejar(player.transform.position,destino,contato,0f);
                if(!caminhoGlobal.sucesso)throw new BloqueioRotas(localBloqueado.Message+"; CITY_GLOBAL: "+caminhoGlobal.erro);
                t.precisao += "; alternativa City global .25 apos " + localBloqueado.Message;
                await MoverRotas(global,caminhoGlobal,destino,contato,t,cenaPermitida,ambiente);
                return;
            }
            if (!local || SceneManager.GetActiveScene().name != origem) return;
        }
        throw new BloqueioRotas("TETO_JANELAS_CITY: destino nao alcancado em 16 janelas.");
    }

    // Um contato raso do solver nao e um spawn aprovado nem muda a grade estrita.
    // Saia dele pelo controlador real antes de pedir o proximo caminho.
    static async Task SepararContatoRasoRotas(GameObject player, AnaliseCirculacaoTester.Mapa mapa, TrechoRotas trecho)
    {
        if(!mapa.Ocupado(player.transform.position))return;
        var corpo=player.GetComponent<CircleCollider2D>();
        float raio=corpo.bounds.extents.x, limite=Mathf.Min(.01f,Physics2D.defaultContactOffset);
        Collider2D[] Sobreposicoes()=>Physics2D.OverlapCircleAll(corpo.bounds.center,raio,Physics2D.AllLayers)
            .Where(c=>c!=corpo && !c.isTrigger && c.enabled && c.gameObject.activeInHierarchy &&
                c.attachedRigidbody!=corpo.attachedRigidbody && !Physics2D.GetIgnoreLayerCollision(corpo.gameObject.layer,c.gameObject.layer) &&
                !Physics2D.GetIgnoreCollision(corpo,c) && corpo.Distance(c).distance<-.0005f).ToArray();
        var iniciais=Sobreposicoes();
        if(iniciais.Length==0 || iniciais.Any(c=>!corpo.IsTouching(c) || -corpo.Distance(c).distance>limite))
            throw new BloqueioRotas("ORIGEM_OCUPADA_SEM_CONTATO_RASO_REGISTRADO");
        var profundidades=iniciais.ToDictionary(c=>c,c=>-corpo.Distance(c).distance);
        var comandos=new[]{Vector2.right,Vector2.up,Vector2.left,Vector2.down,
            new Vector2(1,1).normalized,new Vector2(-1,1).normalized,new Vector2(-1,-1).normalized,new Vector2(1,-1).normalized};
        string erro=""; bool terminou=false; int ticks=0; var buffer=new StringBuilder();
        LiberarDriverRotas(); driverRotas=player.AddComponent<DriverMovimentoTester>(); var driver=driverRotas;
        driver.DeveCancelar=()=>ValidacaoPercursoApresentacao.CancelamentoSolicitado || versaoInicialRotas!=versaoCancelamento;
        driver.EscolherDirecao=real=>
        {
            if(terminou || erro.Length>0)return Vector2.zero;
            if(++ticks>8){erro="CONTATO_RASO_SEM_SAIDA_EM_8_TICKS";return Vector2.zero;}
            if(G.MenuAberto){erro="MENU_DURANTE_SEPARACAO";return Vector2.zero;}
            var atuais=Sobreposicoes();
            if(atuais.Any(c=>!profundidades.ContainsKey(c) || -corpo.Distance(c).distance>profundidades[c]+.001f))
            {erro="PENETRACAO_CRESCEU_DURANTE_SEPARACAO";return Vector2.zero;}
            if(!mapa.Ocupado(real)){terminou=true;return Vector2.zero;}
            var centro=(Vector2)corpo.bounds.center;
            var normais=atuais.Select(c=>(centro-c.ClosestPoint(centro)).normalized).ToArray();
            float passo=player.GetComponent<PlayerMovement>().velocidade*Time.fixedDeltaTime;
            foreach(var comando in comandos.OrderByDescending(d=>normais.Sum(n=>Vector2.Dot(d,n))))
            {
                if(normais.Length==0 || normais.Any(n=>Vector2.Dot(comando,n)<-.0001f) || !normais.Any(n=>Vector2.Dot(comando,n)>.01f))continue;
                bool bloqueado=false;
                foreach(var hit in Physics2D.CircleCastAll(centro,raio,comando,passo,Physics2D.AllLayers))
                {
                    var c=hit.collider;
                    if(c==null || c==corpo || !c.enabled || !c.gameObject.activeInHierarchy || c.attachedRigidbody==corpo.attachedRigidbody ||
                        Physics2D.GetIgnoreLayerCollision(corpo.gameObject.layer,c.gameObject.layer) || Physics2D.GetIgnoreCollision(corpo,c))continue;
                    if(c.isTrigger){if(c.GetComponent<TransicaoDeCena>()!=null)bloqueado=true;continue;}
                    var normal=(centro-c.ClosestPoint(centro)).normalized;
                    if(iniciais.Contains(c) && Vector2.Dot(comando,normal)>=-.0001f)continue;
                    if(hit.distance<=.001f && corpo.Distance(c).distance>=-.0005f && Vector2.Dot(comando,normal)>=-.0001f)continue;
                    bloqueado=true;
                }
                if(!bloqueado)return comando;
            }
            erro="CONTATO_RASO_SEM_DIRECAO_WASD_SEGURA";return Vector2.zero;
        };
        if(trecho!=null)driver.AoPasso=passo=>
        {
            trecho.passos++;
            buffer.AppendLine(JsonUtility.ToJson(new LinhaMovimentoRotas {routeID=trecho.id+"/separacao-contato",passo=passo}));
        };
        try
        {
            var prazo=DateTime.UtcNow.AddSeconds(1);
            while(!terminou && erro.Length==0)
            {
                CancelamentoRotas(); Exigir(driver!=null && string.IsNullOrEmpty(driver.Erro),"Driver indisponivel na separacao.");
                if(DateTime.UtcNow>prazo)throw new BloqueioRotas("TETO_SEPARACAO_CONTATO");
                await Task.Delay(20);
            }
            if(erro.Length>0)throw new BloqueioRotas(erro);
            if(mapa.Ocupado(player.transform.position))throw new BloqueioRotas("SEPARACAO_NAO_CONFIRMADA");
            if(trecho!=null)trecho.precisao+="; contato fisico raso separado por WASD; grade integral inalterada";
        }
        finally{LiberarDriverRotas();if(trecho!=null)FlushMovimentosRotas(buffer);}
    }

    static async Task MoverRotas(AnaliseCirculacaoTester.Mapa mapa, AnaliseCirculacaoTester.Resultado rota, Vector2 destino,
        Collider2D contato, TrechoRotas t, string cenaPermitida, AmbienteRotas ambiente = null)
    {
        Exigir(rota.sucesso, "Caminho diagnostico nao pode ser executado como aprovado.");
        var player = PlayerRotas(); var corpo = CorpoRotas(); string cena = SceneManager.GetActiveScene().name;
        if (mapa.Ocupado(player.transform.position)) throw new BloqueioRotas("SPAWN_OCUPADO_ANTES_DO_TRECHO");
        Exigir(mapa.Penetracoes(.02f).Length == 0, "PENETRACAO_ANTES_DO_TRECHO");
        var buffer = new StringBuilder(); int indice = 0, parado = 0; Vector2 anterior = player.transform.position;
        bool terminou = false, tentouManobra = false; string bloqueio = "", fatal = "";
        var manobra = new Queue<Vector2>();
        string setorDesejado=ambiente!=null && t.alvo.StartsWith("setor:",StringComparison.Ordinal)?t.alvo.Substring(6):null;
        bool NoSetor(Vector2 real)=>setorDesejado!=null && ChaveSetorRotas(mapa,real)==setorDesejado;
        LiberarDriverRotas(); driverRotas = player.AddComponent<DriverMovimentoTester>();
        var driver = driverRotas;
        driver.DeveCancelar = () => ValidacaoPercursoApresentacao.CancelamentoSolicitado || versaoInicialRotas != versaoCancelamento;
        driver.EscolherDirecao = real =>
        {
            if (terminou || !string.IsNullOrEmpty(bloqueio) || !string.IsNullOrEmpty(fatal)) return Vector2.zero;
            string atual = SceneManager.GetActiveScene().name;
            if (atual != cena)
            {
                if (atual == cenaPermitida) terminou = true; else fatal = "CENA_INESPERADA: " + atual;
                return Vector2.zero;
            }
            if (G.MenuAberto) { fatal = "MENU_INESPERADO_DURANTE_MOVIMENTO"; return Vector2.zero; }
            if (contato != null && TocandoRotas(contato)) { t.contatoReal = true; terminou = true; return Vector2.zero; }
            if (contato == null && (setorDesejado!=null?NoSetor(real):Vector2.Distance(real, destino)<=.075f))
            { terminou = true; return Vector2.zero; }
            while (indice < rota.pontos.Length && Vector2.Distance(real, rota.pontos[indice]) <= .08f) indice++;
            Vector2 proximo = indice < rota.pontos.Length ? rota.pontos[indice] : destino;
            float passo = player.GetComponent<PlayerMovement>().velocidade * Time.fixedDeltaTime;
            if(manobra.Count>0)
            {
                var comando=manobra.Dequeue();
                string obstaculo=SegmentoBloqueadoRotas(corpo,real+mapa.offset,comando,passo,contato);
                if(obstaculo.Length==0)return comando;
                bloqueio="MANOBRA_REAL_BLOQUEADA: "+obstaculo;return Vector2.zero;
            }
            var comandos = new[] { Vector2.right, Vector2.up, Vector2.left, Vector2.down,
                new Vector2(1,1).normalized, new Vector2(-1,1).normalized, new Vector2(-1,-1).normalized, new Vector2(1,-1).normalized };
            string primeiroBloqueador = "";
            foreach (var comando in comandos.OrderBy(d=>NoSetor(real+d*passo)?0:1).ThenBy(d => (real + d * passo - proximo).sqrMagnitude))
            {
                if ((real + comando * passo - proximo).sqrMagnitude > (real - proximo).sqrMagnitude + .0001f && !NoSetor(real+comando*passo)) continue;
                // A fisica pode clipar o restante do tick num contato seguro DEPOIS do waypoint.
                // O segmento necessario continua varrido com raio integral; nunca antecipa contato.
                float necessario = Mathf.Min(passo, Vector2.Distance(real, proximo));
                string obstaculo = SegmentoBloqueadoRotas(corpo, real + mapa.offset, comando, passo, contato, necessario);
                if (obstaculo.Length == 0) return comando;
                if (primeiroBloqueador.Length == 0) primeiroBloqueador = obstaculo;
            }
            if(setorDesejado!=null && !tentouManobra)
            {
                tentouManobra=true;
                var comandosCurtos=PlanejarManobraCurtaRotas(corpo,real,mapa.offset,passo,NoSetor);
                if(comandosCurtos.Length>0)
                {
                    foreach(var comando in comandosCurtos.Skip(1))manobra.Enqueue(comando);
                    t.precisao += "; manobra limitada de dois comandos; aprovacao apenas por pose real";
                    return comandosCurtos[0];
                }
            }
            bloqueio = primeiroBloqueador.Length > 0 ? "SEGMENTO_REAL_BLOQUEADO: " + primeiroBloqueador : "SEM_COMANDO_WASD_QUE_APROXIME_WAYPOINT";
            return Vector2.zero;
        };
        driver.AoPasso = passo =>
        {
            t.passos++; buffer.AppendLine(JsonUtility.ToJson(new LinhaMovimentoRotas { routeID = t.id, passo = passo }));
            if (t.passos % 50 == 0) FlushMovimentosRotas(buffer);
            if (passo.penetracao < -.02f && !terminou) fatal = "PENETRACAO_REAL: " + passo.penetracao + " " + string.Join(";", passo.contatos);
            if (passo.menu) fatal = "MENU_INESPERADO_DURANTE_MOVIMENTO";
            Vector2 real = new Vector2(passo.x, passo.y);
            parado = (real - anterior).sqrMagnitude < .000004f && !terminou ? parado + 1 : 0; anterior = real;
            if (parado > 50) bloqueio = "SEM_PROGRESSO_50_TICKS: " + string.Join(";", passo.contatos);
            if (ambiente != null && passo.cena == cena && passo.penetracao >= -.01f) MarcarSetorRotas(ambiente, mapa, real);
            if (fatal.Length > 0 || bloqueio.Length > 0) driver.Parar();
        };
        try
        {
            DateTime limite = DateTime.UtcNow.AddSeconds(40);
            while (!terminou && bloqueio.Length == 0 && fatal.Length == 0)
            {
                CancelamentoRotas();
                if (driver == null)
                {
                    if (SceneManager.GetActiveScene().name == cenaPermitida) { terminou = true; break; }
                    throw new InvalidOperationException("Driver destruido antes da conclusao do movimento.");
                }
                Exigir(string.IsNullOrEmpty(driver.Erro), driver.Erro);
                if (DateTime.UtcNow > limite) { bloqueio = "TETO_TRAJETO_40S"; break; }
                await Task.Delay(30); // Somente monitora; comando e decidido antes de cada FixedUpdate real.
            }
            Exigir(fatal.Length == 0, fatal);
            if (bloqueio.Length > 0) throw new BloqueioRotas(bloqueio);
            if(SceneManager.GetActiveScene().name==cena && mapa.Ocupado(player.transform.position))
            {
                driver.Parar();
                await Task.Delay(250); CancelamentoRotas();
                var persistentes=mapa.Penetracoes(.01f);
                if(persistentes.Length>0)throw new BloqueioRotas("PENETRACAO_PERSISTENTE_APOS_REPOUSO: "+string.Join(";",persistentes));
                if(ambiente!=null)MarcarSetorRotas(ambiente,mapa,player.transform.position);
                t.precisao+="; repouso do solver sem comando antes da validacao final";
            }
            t.contatoReal |= contato != null && TocandoRotas(contato);
            if (SceneManager.GetActiveScene().name == cena && contato != null && !t.contatoReal) throw new BloqueioRotas("CONTATO_REAL_NAO_CONFIRMADO");
        }
        finally { LiberarDriverRotas(); FlushMovimentosRotas(buffer); }
    }

    static Vector2[] PlanejarManobraCurtaRotas(Collider2D corpo, Vector2 raiz, Vector2 offset, float passo, Func<Vector2,bool> objetivo)
    {
        var comandos=new[] {Vector2.right,Vector2.up,Vector2.left,Vector2.down,
            new Vector2(1,1).normalized,new Vector2(-1,1).normalized,new Vector2(-1,-1).normalized,new Vector2(1,-1).normalized};
        foreach(var primeiro in comandos)
        {
            var meio=raiz+primeiro*passo;
            if(SegmentoBloqueadoRotas(corpo,raiz+offset,primeiro,passo,null).Length>0)continue;
            foreach(var segundo in comandos)
                if(objetivo(meio+segundo*passo) && SegmentoBloqueadoRotas(corpo,meio+offset,segundo,passo,null).Length==0)
                    return new[]{primeiro,segundo};
        }
        return Array.Empty<Vector2>();
    }

    static string SegmentoBloqueadoRotas(Collider2D corpo, Vector2 centro, Vector2 direcao, float distancia, Collider2D contato, float distanciaSolidos = -1f)
    {
        if (distanciaSolidos < 0f) distanciaSolidos = distancia;
        float raio = Mathf.Max(corpo.bounds.extents.x, corpo.bounds.extents.y);
        // Cast sozinho pode omitir contato lateral/inicial que a fisica reconhece.
        // Mesma consulta integral de sobreposicao usada pela grade, sem alterar raio.
        var filtro = new ContactFilter2D(); filtro.NoFilter(); filtro.useTriggers = true;
        var sobrepostos = new List<Collider2D>();
        foreach (var ponto in new[] { centro, centro + direcao * distancia })
        {
            sobrepostos.Clear(); Physics2D.OverlapCircle(ponto, raio, filtro, sobrepostos);
            foreach (var c in sobrepostos)
                if (c != contato && c.isTrigger && c.GetComponent<TransicaoDeCena>() != null
                    && !Physics2D.GetIgnoreLayerCollision(corpo.gameObject.layer,c.gameObject.layer)
                    && !Physics2D.GetIgnoreCollision(corpo,c))
                    return "PORTAL_NAO_ESCOLHIDO " + CaminhoRotas(c.transform);
        }
        foreach (var hit in Physics2D.CircleCastAll(centro, raio, direcao, distancia, Physics2D.AllLayers))
        {
            var c = hit.collider;
            if (c == null || c == corpo || c.transform.IsChildOf(corpo.transform) || c.attachedRigidbody == corpo.attachedRigidbody
                || !c.enabled || !c.gameObject.activeInHierarchy || c.attachedRigidbody != null && !c.attachedRigidbody.simulated
                || Physics2D.GetIgnoreLayerCollision(corpo.gameObject.layer, c.gameObject.layer) || Physics2D.GetIgnoreCollision(corpo, c)) continue;
            if (c.isTrigger)
            {
                // O controlador executa o tick inteiro, inclusive alem do waypoint.
                // Trigger nao interrompe movimento como uma parede: nunca recortar sua varredura.
                if (c != contato && c.GetComponent<TransicaoDeCena>() != null) return "PORTAL_NAO_ESCOLHIDO " + CaminhoRotas(c.transform);
                continue;
            }
            if (hit.distance > distanciaSolidos + .0005f) continue;
            Vector2 normal = centro - c.ClosestPoint(centro);
            float profundidade = raio - normal.magnitude;
            // Contato inicial apenas rasante/afastando-se nao e uma parede no sentido do comando.
            if (hit.distance <= .001f && profundidade <= .001f && Vector2.Dot(direcao, normal.normalized) >= -.0001f) continue;
            if (hit.distance >= distanciaSolidos - .0005f && raio - Vector2.Distance(centro + direcao * distanciaSolidos, c.ClosestPoint(centro + direcao * distanciaSolidos)) <= .0005f) continue;
            return CaminhoRotas(c.transform);
        }
        return "";
    }
    static AnaliseCirculacaoTester.Mapa MapaRotas(GameObject player, Vector2 destino)
    {
        string cena = SceneManager.GetActiveScene().name; Bounds bounds;
        var fundo = ComponentesRotas<SpriteRenderer>().Where(s => s.enabled && s.sprite != null && !s.transform.IsChildOf(player.transform))
            .OrderByDescending(s => s.bounds.size.x * s.bounds.size.y).FirstOrDefault();
        Exigir(fundo != null, "Fundo visual ausente: " + cena);
        if (cena == "City")
        {
            Vector2 origem = player.transform.position;
            bounds = new Bounds((origem + destino) / 2, new Vector3(Mathf.Abs(origem.x - destino.x) + 5f, Mathf.Abs(origem.y - destino.y) + 5f, 1));
        }
        else
        {
            var limite = cena == "CasaLip" ? ComponentesRotas<EdgeCollider2D>().FirstOrDefault(c => c.enabled && !c.isTrigger && c.name == "GameObject (3)") : null;
            bounds = limite != null ? limite.bounds : fundo.bounds;
        }
        var mapa = AnaliseCirculacaoTester.Criar(player, bounds, .125f);
        var ambiente = relatorioRotas.ambientes.FirstOrDefault(a => a.cena == cena);
        if (ambiente != null)
        {
            ambiente.referenciaVisual = CaminhoRotas(fundo.transform) + " / " + AssetDatabase.GetAssetPath(fundo.sprite);
            ambiente.minX = bounds.min.x; ambiente.minY = bounds.min.y; ambiente.maxX = bounds.max.x; ambiente.maxY = bounds.max.y;
            ambiente.raio = mapa.raio; ambiente.offsetX = mapa.offset.x; ambiente.offsetY = mapa.offset.y;
        }
        return mapa;
    }
    static string ChaveSetorRotas(AnaliseCirculacaoTester.Mapa mapa, Vector2 raiz) => Mathf.FloorToInt((raiz.x + mapa.offset.x - mapa.min.x) / .25f) + ":" + Mathf.FloorToInt((raiz.y + mapa.offset.y - mapa.min.y) / .25f);
    static void MarcarSetorRotas(AmbienteRotas a, AnaliseCirculacaoTester.Mapa mapa, Vector2 raiz)
    {
        string id = ChaveSetorRotas(mapa, raiz); var s = a.setores.FirstOrDefault(x => x.id == id);
        if (s != null) s.estado = AprovadoRotas;
    }
    static T[] ComponentesRotas<T>() where T : Component => SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<T>(false)).ToArray();
    static Collider2D[] TriggersRotas(Component c) => c.GetComponents<Collider2D>().Where(x => x.enabled && x.isTrigger && x.gameObject.activeInHierarchy).ToArray();
    static GameObject PlayerRotas() => GameObject.FindGameObjectWithTag("Player");
    static Collider2D CorpoRotas() => PlayerRotas().GetComponentsInChildren<Collider2D>().Single(c => c.enabled && !c.isTrigger);
    static bool TocandoRotas(Collider2D contato)
    {
        if (contato == null || PlayerRotas() == null) return false;
        var corpo = CorpoRotas(); var distancia = corpo.Distance(contato);
        return corpo.IsTouching(contato) && distancia.isValid && distancia.distance <= .001f;
    }
    static string CaminhoRotas(Transform t) => DriverMovimentoTester.Caminho(t);
    static string HashRotas(byte[] bytes) { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", ""); }
    static void CancelamentoRotas()
    {
        if (!EditorApplication.isPlaying || ValidacaoPercursoApresentacao.CancelamentoSolicitado || versaoInicialRotas != versaoCancelamento)
            throw new OperationCanceledException("Auditoria de circulacao interrompida.");
    }
    static async Task AguardarRotas(Func<bool> pronto, string descricao)
    {
        DateTime fim = DateTime.UtcNow.AddSeconds(12);
        while (!pronto()) { CancelamentoRotas(); if (DateTime.UtcNow > fim) throw new TimeoutException(descricao); await Task.Delay(50); }
    }
    static void LiberarDriverRotas()
    {
        if (driverRotas == null) return;
        driverRotas.EscolherDirecao=null; driverRotas.AoPasso=null; driverRotas.DeveCancelar=null;
        driverRotas.Parar(); driverRotas.enabled=false; UnityEngine.Object.Destroy(driverRotas); driverRotas = null;
    }
    static TrechoRotas NovoTrechoRotas(string alvo, string origem, Vector2 destino)
    {
        var player = PlayerRotas(); Vector2 pos = player != null ? (Vector2)player.transform.position : Vector2.zero;
        var t = new TrechoRotas { id = relatorioRotas.idExecucao + "/" + (relatorioRotas.trechos.Count + 1).ToString("D6"),
            execucao = relatorioRotas.idExecucao, cena = SceneManager.GetActiveScene().name, alvo = alvo, origem = origem,
            inicio = DateTime.UtcNow.ToString("O"), inicioX = pos.x, inicioY = pos.y, destinoX = destino.x, destinoY = destino.y };
        relatorioRotas.trechos.Add(t); relatorioRotas.trechoAtual = t.id; GravarRotas(); return t;
    }
    static void FinalizarTrechoRotas(TrechoRotas t)
    {
        t.fim = DateTime.UtcNow.ToString("O"); var player = PlayerRotas();
        if (player != null) { t.finalX = player.transform.position.x; t.finalY = player.transform.position.y; }
        GravarRotas();
    }
    static void ErroAuditoriaRotas(string cena, string alvo, string bloqueador, string origem, string resultado)
    {
        relatorioRotas.erros.Add(new ErroRotas { execucao = relatorioRotas.idExecucao, instante = DateTime.UtcNow.ToString("O"),
            cena = cena, alvo = alvo, bloqueador = bloqueador, origem = origem, resultado = resultado });
    }
    static void FlushMovimentosRotas(StringBuilder buffer)
    {
        if (buffer.Length == 0) return;
        File.AppendAllText(Path.Combine(HistoricoRotas, "movimentos.jsonl"), buffer.ToString()); buffer.Clear();
    }
    static void GravarRotas()
    {
        relatorioRotas.cena = SceneManager.GetActiveScene().name;
        relatorioRotas.rotasPrevistas = relatorioRotas.ligacoes.Count; relatorioRotas.rotasExecutadas = relatorioRotas.ligacoes.Count(l => l.tentativas > 0);
        relatorioRotas.rotasAprovadas = relatorioRotas.ligacoes.Count(l => l.estado == AprovadoRotas); relatorioRotas.rotasRestantes = relatorioRotas.rotasPrevistas - relatorioRotas.rotasAprovadas;
        relatorioRotas.ambientesPrevistos = relatorioRotas.ambientes.Count; relatorioRotas.ambientesVisitados = relatorioRotas.ambientes.Count(a => a.visitado);
        relatorioRotas.ambientesAprovados = relatorioRotas.ambientes.Count(a => a.estado == AprovadoRotas);
        relatorioRotas.alvosPrevistos = relatorioRotas.ambientes.Sum(a => a.alvos.Count); relatorioRotas.alvosAprovados = relatorioRotas.ambientes.Sum(a => a.alvos.Count(t => t.estado == AprovadoRotas));
        relatorioRotas.setoresPrevistos = relatorioRotas.ambientes.Sum(a => a.setores.Count); relatorioRotas.setoresAprovados = relatorioRotas.ambientes.Sum(a => a.setores.Count(s => s.estado == AprovadoRotas));
        relatorioRotas.restantes = relatorioRotas.ligacoes.Where(l => l.estado != AprovadoRotas).Select(l => l.id + "=" + l.estado)
            .Concat(relatorioRotas.ambientes.Where(a => a.estado != AprovadoRotas).Select(a => a.cena + "=" + a.estado + "; alvosDescobertos=" + a.alvosDescobertos + "; coberturaDescoberta=" + a.coberturaDescoberta)).ToArray();
        Directory.CreateDirectory(HistoricoRotas); string json = EstadoRotas();
        File.WriteAllText(Path.Combine(HistoricoRotas, "resultado.json"), json);
        File.WriteAllText(PonteiroRotas, json);
    }
}
