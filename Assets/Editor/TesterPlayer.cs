using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DroidsCode.Casa;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Piloto de QA somente Editor. Ativado explicitamente; joga pelos componentes reais.</summary>
public static partial class TesterPlayer
{
    const string Armado = "DroidsCode.TesterPlayer.InicioSolicitado";
    const string Modo = Armado + ".Modo";
    static readonly string Pasta = Path.GetFullPath("output/apresentacao-jogavel/2026-11-12/tester-player");
    [Serializable] class Relatorio
    {
        public bool executando;
        public string resultado = "Nao iniciado";
        public string etapa = "";
        public string cena = "";
        public string erro = "";
        public string inicio = "";
        public string fim = "";
        public int etapasConcluidas;
        public int etapasExecutadas;
        public int retomadaDeEtapa;
        public string idExecucao;
        public string escopo;
        public string[] questsNoCheckpoint;
        public ResumoMissao[] missoes;
    }
    [Serializable] class ResumoMissao { public string id, titulo, estado; public string[] objetivos; }
    static Relatorio relatorio = new Relatorio();
    static Task execucao;
    static int numeroEtapa, retomarDe, versaoCancelamento, repeticoesCheckpoint;
    public static string Estado() => JsonUtility.ToJson(relatorio, true);

    [InitializeOnLoadMethod]
    static void Registrar()
    {
        EditorApplication.playModeStateChanged -= MudouPlay;
        EditorApplication.playModeStateChanged += MudouPlay;
    }
    static async void MudouPlay(PlayModeStateChange estado)
    {
        if (estado == PlayModeStateChange.ExitingPlayMode) Cancelar();
        if (estado != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Armado, false)) return;
        int versaoInicial=versaoCancelamento;
        await Task.Delay(250);
        if (EditorApplication.isPlaying && SessionState.GetBool(Armado, false))
        {
            SessionState.SetBool(Armado, false);
            string modo=SessionState.GetString(Modo,"novo");
            if (modo == "save" || modo == "opcionais")
            {
                try
                {
                    new SalvamentoJson().Carregar();
                    await Task.Delay(180);
                    if(versaoInicial!=versaoCancelamento || !EditorApplication.isPlaying)return;
                    if(modo=="opcionais")
                    {
                        Exigir(Flag("ato1_concluido"),"Conclua a historia antes da rodada de todas as opcionais.");
                        IniciarExecucao(0,true);
                    }
                    else
                    {
                        int inicio = DeterminarEtapaDeRetomada(Flag);
                        Exigir(inicio >= 0, "Checkpoint ainda nao chegou ao boss; use partida nova.");
                        IniciarExecucao(inicio);
                    }
                }
                catch (Exception e) { Debug.LogError("[TesterPlayer] Retomada recusada: " + e); }
            }
            else Iniciar();
        }
    }

    [MenuItem("Droids Code/Testes/TesterPlayer/Iniciar Ato 1 completo")]
    public static void PrepararEIniciar()
    {
        if (EditorApplication.isPlaying) { Debug.Log(Iniciar()); return; }
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Salve a cena antes de iniciar o tester.");
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        SessionState.SetString(Modo, "novo");
        SessionState.SetBool(Armado, true);
        EditorApplication.isPlaying = true;
    }

    public static string Iniciar()
    {
        if (!EditorApplication.isPlaying) return "RECUSADO: entre em Play pela MainMenu ou use o menu TesterPlayer.";
        if (relatorio.executando) return "RECUSADO: TesterPlayer ja esta em execucao.";
        if (SceneManager.GetActiveScene().name != "MainMenu") return "RECUSADO: o percurso completo comeca na MainMenu.";
        var existente = UnityEngine.Object.FindAnyObjectByType<GerenciadorDeEstado>();
        if (existente != null && existente.TodasAsFlags.Count != 0)
            return "RECUSADO: reinicie Play para obter uma partida nova.";
        return IniciarExecucao(0);
    }
    public static int DeterminarEtapaDeRetomada(Func<string, bool> flag)
    {
        if (flag("quest_q_main_partida_concluida")) return 13;
        if (flag("ato1_concluido")) return 13;
        if (flag("ato1_sessao6")) return 9;
        if (flag("ato1_puzzle_energia") && flag("ato1_ivo_confessou")) return 8;
        return -1;
    }
    [MenuItem("Droids Code/Testes/TesterPlayer/Retomar checkpoint do boss ou epilogo")]
    public static void PrepararContinuacao()
    {
        Exigir(!EditorApplication.isPlaying, "Saia de Play antes de carregar o checkpoint.");
        for (int i=0;i<SceneManager.sceneCount;i++) Exigir(!SceneManager.GetSceneAt(i).isDirty,"Cena com alteracoes nao salvas.");
        Exigir(new SalvamentoJson().ExisteSave(), "Checkpoint nao encontrado.");
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        SessionState.SetString(Modo,"save"); SessionState.SetBool(Armado,true);
        EditorApplication.isPlaying=true;
    }
    static string IniciarExecucao(int inicio, bool opcionais=false)
    {
        Exigir(!relatorio.executando,"Tester ja esta executando.");
        relatorio = new Relatorio { executando = true, resultado = "Executando", inicio = DateTime.UtcNow.ToString("O"),
            idExecucao=DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff"),retomadaDeEtapa=inicio,
            escopo=opcionais?"todas as opcionais":"Ato 1",questsNoCheckpoint=opcionais||inicio>0?GerenciadorDeQuests.Concluidas().Select(q=>q.Id).ToArray():Array.Empty<string>() };
        retomarDe=inicio; numeroEtapa=0; repeticoesCheckpoint=0;
        ValidacaoPercursoApresentacao.CancelamentoSolicitado = false;
        ValidacaoPercursoApresentacao.PreferirAtaqueBasico=false;
        Directory.CreateDirectory(Pasta);
        if(File.Exists(Path.Combine(Pasta,"resultado.json")))
            File.Copy(Path.Combine(Pasta,"resultado.json"),Path.Combine(Pasta,"anterior-"+relatorio.idExecucao+".json"));
        if(File.Exists(Path.Combine(Pasta,"etapas.jsonl")))
            File.Copy(Path.Combine(Pasta,"etapas.jsonl"),Path.Combine(Pasta,"etapas-anteriores-"+relatorio.idExecucao+".jsonl"));
        File.WriteAllText(Path.Combine(Pasta, "etapas.jsonl"), "");
        execucao = opcionais?ExecutarOpcionais():Executar();
        return "INICIADO: " + Path.Combine(Pasta, "resultado.json");
    }

    [MenuItem("Droids Code/Testes/TesterPlayer/Cancelar")]
    public static void Cancelar()
    {
        versaoCancelamento++;
        SessionState.SetBool(Armado, false);
        ValidacaoPercursoApresentacao.CancelamentoSolicitado = true;
    }

    [MenuItem("Droids Code/Testes/TesterPlayer/Mostrar estado no Console")]
    public static void MostrarEstado() => Debug.Log(Estado());
    static void ConferirCancelamento()
    {
        if (!EditorApplication.isPlaying || ValidacaoPercursoApresentacao.CancelamentoSolicitado)
            throw new OperationCanceledException("Teste interrompido.");
    }
    static void Gravar()
    {
        relatorio.cena = SceneManager.GetActiveScene().name;
        if(EditorApplication.isPlaying && UnityEngine.Object.FindAnyObjectByType<GerenciadorDeEstado>()!=null)
            relatorio.missoes=CatalogoDeQuests.Todos.Values.Select(q=>
            {
                bool ativa=GerenciadorDeQuests.EstaAtiva(q.Id), entregue=GerenciadorDeQuests.EstaConcluida(q.Id) && !ativa;
                return new ResumoMissao {id=q.Id,titulo=q.Titulo,estado=ativa?"ativa":entregue?"entregue":GerenciadorDeQuests.PodeSerOferecida(q.Id)?"disponivel":"bloqueada",
                    objetivos=entregue?q.Objetivos.Select(o=>"entregue: "+o.Descricao).ToArray():q.Objetivos.Select((o,i)=>GerenciadorDeQuests.DescreverObjetivo(q,i)).ToArray()};
            }).ToArray();
        File.WriteAllText(Path.Combine(Pasta, "resultado.json"), Estado());
        string historico=Path.Combine(Pasta,"execucoes",relatorio.idExecucao);
        Directory.CreateDirectory(historico);
        File.WriteAllText(Path.Combine(historico,"resultado.json"),Estado());
    }
    static async Task Etapa(string nome, Func<Task> acao)
    {
        ConferirCancelamento();
        if(numeroEtapa++<retomarDe){relatorio.etapasConcluidas=numeroEtapa;return;}
        relatorio.etapa = nome; Gravar();
        Debug.Log("[TesterPlayer] " + nome);
        await acao(); ConferirCancelamento();
        relatorio.etapasConcluidas++;
        relatorio.etapasExecutadas++;
        Exigir(!G.MenuAberto,"Etapa terminou com menu bloqueando movimento.");
        var player=GameObject.FindGameObjectWithTag("Player");
        Exigir(player!=null,"Checkpoint de etapa sem player.");
        var dados=SalvamentoJson.CriarDados(G,SceneManager.GetActiveScene().name,player.transform.position);
        File.WriteAllText(Path.Combine(Pasta,"execucoes",relatorio.idExecucao,"save-etapa-"+relatorio.etapasConcluidas.ToString("D2")+".json"),JsonUtility.ToJson(dados,true));
        File.AppendAllText(Path.Combine(Pasta, "etapas.jsonl"), JsonUtility.ToJson(relatorio) + Environment.NewLine);
        File.AppendAllText(Path.Combine(Pasta,"execucoes",relatorio.idExecucao,"etapas.jsonl"),JsonUtility.ToJson(relatorio)+Environment.NewLine);
        Gravar();
    }
    static async Task Esperar(Func<bool> condicao, string descricao, int limiteMs = 12000)
    {
        var fim = DateTime.UtcNow.AddMilliseconds(limiteMs);
        while (!condicao())
        {
            ConferirCancelamento();
            if (DateTime.UtcNow > fim) throw new TimeoutException(descricao);
            await Task.Delay(60);
        }
    }
    static Task Cena(string nome) => Esperar(() => SceneManager.GetActiveScene().name == nome, "Cena nao abriu: " + nome);
    static GerenciadorDeEstado G => GerenciadorDeEstado.Instancia;
    static bool Flag(string chave) => G.ObterFlag(chave);
    static void Exigir(bool condicao, string mensagem) { if (!condicao) throw new InvalidOperationException(mensagem); }
    static string Texto(Button b) => b.GetComponentInChildren<TMP_Text>()?.text ?? b.name;
    static Button[] Escolhas()
    {
        var d = DialogoUIManager.Instancia;
        return d != null && d.EstaAberto && d.painelDeEscolhas != null && d.painelDeEscolhas.gameObject.activeInHierarchy
            ? d.painelDeEscolhas.GetComponentsInChildren<Button>().Where(b => b.interactable).ToArray() : Array.Empty<Button>();
    }
    static async Task Dialogo(string escolha = "")
    {
        ConferirCancelamento();
        string r = await ValidacaoPercursoApresentacao.Dialogo(escolha);
        if (r.StartsWith("ESCOLHA_NAO_ENCONTRADA")) throw new InvalidOperationException(r);
        ConferirCancelamento();
    }
    static MoradorInterativo Morador(string chave) => UnityEngine.Object.FindObjectsByType<MoradorInterativo>()
        .Single(m => m.name == chave || m.idNpc == chave);
    static Collider2D ColPlayer => GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    static Vector2 Alvo(Collider2D col)
    {
        var p = ColPlayer;
        return (Vector2)col.bounds.center - ((Vector2)p.bounds.center - (Vector2)p.transform.position);
    }
    static async Task Mover(Vector2 destino, string cenaPermitida = "", bool permitirCombateAleatorio = true)
    {
        for (int tentativa = 0; tentativa < 5; tentativa++)
        {
            ConferirCancelamento();
            string cena = SceneManager.GetActiveScene().name;
            string r = await ValidacaoPercursoApresentacao.Caminhar(destino.x, destino.y);
            if (r.StartsWith("CAMINHO_FISICO_OK")) return;
            if (r.StartsWith("CANCELADO")) throw new OperationCanceledException();
            if (SceneManager.GetActiveScene().name == cenaPermitida && !string.IsNullOrEmpty(cenaPermitida)) return;
            if (SceneManager.GetActiveScene().name == "Battle")
            {
                Exigir(permitirCombateAleatorio, "Batalha disparou ao aproximar, antes da interacao explicita.");
                await Lutar(); await Cena(cena); continue;
            }
            if (r.StartsWith("INTERROMPIDO_POR_MENU") && DialogoUIManager.Instancia.EstaAberto)
            {
                await Dialogo(); Exigir(Escolhas().Length == 0, "Dialogo automatico exige escolha inesperada."); continue;
            }
            throw new InvalidOperationException(r);
        }
        throw new InvalidOperationException("Movimento interrompido cinco vezes: " + destino);
    }
    static async Task Aproximar(string chave)
    {
        var m = Morador(chave);
        var trigger = m.GetComponents<Collider2D>().First(c => c.isTrigger);
        await AproximarTrigger(trigger, m.batalha != null);
    }
    static async Task AproximarTrigger(Collider2D trigger, bool impedirCombateAutomatico = false, string cenaDestino = "")
    {
        if (trigger.IsTouching(ColPlayer)) return;
        for (int i = 0; i < 4; i++)
        {
            ConferirCancelamento();
            string r = await ValidacaoPercursoApresentacao.CaminharAteTrigger(trigger);
            if (!string.IsNullOrEmpty(cenaDestino) && SceneManager.GetActiveScene().name == cenaDestino) return;
            Exigir(SceneManager.GetActiveScene().name != "Battle", impedirCombateAutomatico
                ? "Batalha disparou ao aproximar, antes da interacao explicita." : "Encontro inesperado durante aproximacao.");
            if (r.StartsWith("CANCELADO")) throw new OperationCanceledException();
            if (r.StartsWith("INTERROMPIDO_POR_MENU") && DialogoUIManager.Instancia.EstaAberto)
            { await Dialogo(); Exigir(Escolhas().Length == 0, "Dialogo automatico pede escolha."); continue; }
            Exigir(r.StartsWith("CAMINHO_FISICO_OK"), r);
            await Task.Delay(100);
            if (!string.IsNullOrEmpty(cenaDestino) && SceneManager.GetActiveScene().name == cenaDestino) return;
            if (trigger != null && trigger.IsTouching(ColPlayer)) return;
        }
        throw new InvalidOperationException("Nenhum ponto acessivel de interacao: " + trigger.name);
    }
    static async Task Interagir(string chave)
    {
        await Aproximar(chave);
        string r = ValidacaoPercursoApresentacao.Interagir(Morador(chave).name);
        Exigir(!r.StartsWith("FORA_DO_ALCANCE"), r);
        await Task.Delay(80);
    }
    static async Task FalarAte(string chave, string flag, string escolha = "")
    {
        for (int i = 0; i < 6 && !Flag(flag); i++)
        {
            await Interagir(chave); await Dialogo(escolha);
            if (Escolhas().Length > 0) throw new InvalidOperationException("Conversa inesperada antes de " + flag + ": " + string.Join(",", Escolhas().Select(Texto)));
        }
        Exigir(Flag(flag), "Conversa nao produziu marco: " + flag);
    }
    static async Task Transitar(string destino, string porta="")
    {
        var candidatos=UnityEngine.Object.FindObjectsByType<TransicaoDeCena>().Where(x=>x.nomeCenaDestino==destino);
        var t=string.IsNullOrEmpty(porta)
            ?candidatos.OrderBy(x=>Vector2.Distance(x.transform.position,ColPlayer.transform.position)).ThenBy(x=>x.name).FirstOrDefault()
            :candidatos.SingleOrDefault(x=>x.name==porta);
        Exigir(t!=null,"Porta nao encontrada: "+destino+"/"+porta);
        Exigir(!t.trancada, "Porta trancada para " + destino + ": " + t.mensagemDeBloqueio);
        await AproximarTrigger(t.GetComponent<Collider2D>(), false, destino);
        await Cena(destino); await Task.Delay(160);
    }
    static async Task Missao(string npc, string id, bool entregar = false, bool reaceitar = false)
    {
        var q = CatalogoDeQuests.Obter(id);
        Exigir(q != null, "Quest ausente: " + id);
        Exigir(!reaceitar || q.Repetivel,"Reaceite exige missao repetivel: "+id);
        if (entregar && GerenciadorDeQuests.EstaConcluida(id) && !GerenciadorDeQuests.EstaAtiva(id)) return;
        if (!entregar && (GerenciadorDeQuests.EstaAtiva(id) || (!reaceitar && Flag("quest_" + id + "_concluida")))) return;
        for (int i = 0; i < 6; i++)
        {
            await Interagir(npc); await Dialogo();
            string prefixo = (entregar ? "Entregar: " : "Ajuda: ") + q.Titulo;
            if (Escolhas().Any(b => Texto(b) == prefixo))
            {
                await Dialogo(prefixo); await Dialogo(entregar ? "Concluir entrega" : "Vou ajudar.");
                Exigir(!DialogoUIManager.Instancia.EstaAberto, "Missao deixou dialogo pendente: " + id);
                Exigir(entregar ? Flag("quest_" + id + "_concluida") : GerenciadorDeQuests.EstaAtiva(id), "Operacao de quest recusada: " + id);
                return;
            }
            Exigir(Escolhas().Length == 0, "Opcao nao disponivel: " + prefixo);
        }
        throw new InvalidOperationException("Missao nao acessivel: " + id);
    }
    static async Task Codigo(string codigo)
    {
        Exigir(!DialogoUIManager.Instancia.EstaAberto, "Terminal durante dialogo.");
        ValidacaoPercursoApresentacao.Codigo(codigo);
        var t = LocalizadorDeTerminal.Encontrar();
        await Esperar(() => t.Runner != null && t.Runner.EstadoAtual.ToString() != "Executando", "Lua nao terminou.");
        Exigir(t.Runner.EstadoAtual.ToString() == "Concluida", "Lua recusado: " + t.textoSaida.text);
        Exigir(!t.textoSaida.text.Contains("Erro:") && !t.textoSaida.text.Contains("Falha:"), "Comando recusado: " + t.textoSaida.text);
        t.Fechar(); await Task.Delay(100);
        if (DialogoUIManager.Instancia.EstaAberto) await Dialogo();
    }
    static async Task Puzzle(string nome, string codigo, string flag)
    {
        var terminal = UnityEngine.Object.FindObjectsByType<TerminalDoInterior>().Single(t => t.name == nome);
        await AproximarTrigger(terminal.GetComponent<Collider2D>());
        Exigir(terminal.TentarInteragir(), "Terminal nao abriu por proximidade: " + nome);
        await Codigo(codigo); Exigir(Flag(flag), "Puzzle nao concluiu: " + nome);
    }
    static async Task Lutar()
    {
        await Cena("Battle");
        await Esperar(() => UnityEngine.Object.FindAnyObjectByType<BattleManager>()?.botaoAtacar != null, "Batalha sem controles.");
        await ValidacaoPercursoApresentacao.Combater(60);
        Exigir(SceneManager.GetActiveScene().name != "GameOver", "Derrota: o preparo/estrategia do tester precisa ser revisto.");
        await Cena("Game");
        await Task.Delay(100);
        if (DialogoUIManager.Instancia.EstaAberto) await Dialogo();
    }
    static async Task Encontro(string nome)
    {
        string aviso="",origem=SceneManager.GetActiveScene().name;
        Application.LogCallback observar=(mensagem,pilha,tipo)=>
        {
            if(mensagem.StartsWith("Não foi possível salvar o checkpoint. A batalha não foi iniciada.",StringComparison.Ordinal))aviso=mensagem;
        };
        Application.logMessageReceived+=observar;
        try
        {
            await AbordarEncontro(nome);
            if(!string.IsNullOrEmpty(aviso))
            {
                Exigir(SceneManager.GetActiveScene().name==origem && !G.MenuAberto && Morador(nome).batalha.PodeDisparar(),"Estado ambiguo apos recusa de checkpoint; nao repetir.");
                Exigir(repeticoesCheckpoint++==0,"Nova falha de checkpoint apos recuperacao; interrompendo. "+aviso);
                Debug.LogWarning("[TesterPlayer] Checkpoint recusado antes da batalha; estado consultado, uma repeticao autorizada: "+aviso);
                await Task.Delay(300); ConferirCancelamento(); await AbordarEncontro(nome);
                if(SceneManager.GetActiveScene().name==origem && Morador(nome).batalha.PodeDisparar())
                    throw new InvalidOperationException("Checkpoint continuou recusado; sem terceira tentativa. "+aviso);
            }
            await Lutar();
        }
        finally { Application.logMessageReceived-=observar; }
    }
    static async Task AbordarEncontro(string nome)
    {
        var ponto = Morador(nome);
        var vale = ponto.GetComponentInParent<InteracoesDoVale>();
        if (vale == null || Array.IndexOf(vale.encontros, ponto) < 0)
        {
            await Interagir(nome);
            return;
        }
        var area = ponto.GetComponent<CircleCollider2D>();
        if (area.Distance(ColPlayer).isOverlapped)
        {
            Vector2 direcao = (Vector2)(ColPlayer.bounds.center - area.bounds.center);
            if (direcao.sqrMagnitude < .0001f) direcao = Vector2.down;
            await Mover(Alvo(area) + direcao.normalized *
                (area.bounds.extents.x + ColPlayer.bounds.extents.x + .35f));
        }
        await AproximarTrigger(area, cenaDestino: "Battle");
    }
    static async Task CurarFora(bool completo=false)
    {
        var d = G.DroidDoJogador;
        for (int i = 0; i < 5 && d.Hp < (completo?d.HpMax:d.HpMax*.6f); i++)
        {
            string id = ValidacaoPercursoApresentacao.EscolherCura();
            if (id == null) break;
            var bag = UnityEngine.Object.FindAnyObjectByType<BagUIManager>(FindObjectsInactive.Include);
            bag.Mostrar(); bag.ConfirmarItemDaBag(id); bag.Fechar(); await Task.Delay(80);
        }
    }
    static async Task Comprar(string id)
    {
        var loja = UnityEngine.Object.FindAnyObjectByType<LojaUIManager>(FindObjectsInactive.Include);
        var item = CatalogoDeItens.Obter(id);
        Exigir(item != null && G.Gold >= item.Preco, "Gold insuficiente para " + id);
        var b = loja.painelListaDeItens.GetComponentsInChildren<Button>(true).SingleOrDefault(x => Texto(x).Contains(item.Nome));
        Exigir(b != null, "Produto nao encontrado: " + item.Nome);
        int quantidade = G.ObterQuantidadeDeItem(id);
        b.onClick.Invoke(); loja.ConfirmarSelecao(); await Task.Delay(80);
        Exigir(G.ObterQuantidadeDeItem(id) == quantidade + 1, "Compra nao confirmada: " + id);
    }
    static async Task GarantirCity()
    {
        string cena=SceneManager.GetActiveScene().name;
        if(cena=="City")return;
        Exigir(cena=="Game" || cena.StartsWith("Interior_"),"Retomada em cena inesperada: "+cena);
        await Transitar("City");
    }
    static async Task GarantirGame()
    {
        if(SceneManager.GetActiveScene().name=="Game")return;
        await GarantirCity(); await Transitar("Game");
    }
    // Teto calculado para60acoes: HP35/DEF19, criticos e vazamento, pequenas20.
    // A reserva e conferida depois do topup; medias/grandes tambem contam.
    const int ReservaCurasBoss=22;
    static int CurasDisponiveisBoss()=>new[]{CatalogoDeItens.IdPocaoPequena,CatalogoDeItens.IdPocaoMedia,CatalogoDeItens.IdPocaoGrande}
        .Sum(id=>G.ObterQuantidadeDeItem(id));
    static int CurasAComprarBoss()
    {
        var d=G.DroidDoJogador;
        int cura=CatalogoDeItens.Obter(CatalogoDeItens.IdPocaoPequena).CuraHp;
        int topup=(d.HpMax-d.Hp+cura-1)/cura;
        return Math.Max(0,ReservaCurasBoss+topup-CurasDisponiveisBoss());
    }
    static async Task PrepararBoss()
    {
        await GarantirGame();
        var d=G.DroidDoJogador;
        int vitNecessaria=Math.Max(0,13-d.StatsBase.Vit);
        try
        {
            ValidacaoPercursoApresentacao.PreferirAtaqueBasico=true;
            for(int i=0;(d.Pontos.PontosDisponiveis<vitNecessaria || d.EnergiaAtual<d.EnergiaMaxima) && i<10;i++)
            { await CurarFora(); await Encontro("Sucata da borda"); }
            Exigir(d.Pontos.PontosDisponiveis>=vitNecessaria,"Faltam pontos normais para preparo defensivo.");
            if(vitNecessaria>0)await Codigo("droid.subirAtributo(\"Vit\","+vitNecessaria+")");
            int preco=CatalogoDeItens.Obter(CatalogoDeItens.IdPocaoPequena).Preco;
            var limpeza=CatalogoDeQuests.Obter("q_limpeza_borda");
            for(int rodada=0;G.Gold<CurasAComprarBoss()*preco && rodada<6;rodada++)
            {
                await Bruna(); await Missao("mestra_guilda",limpeza.Id,reaceitar:true);
                await GarantirGame();
                for(int luta=0;!GerenciadorDeQuests.ObjetivoCompleto(limpeza,0) && luta<3;luta++)
                { await CurarFora(); await Encontro("Sucata da borda"); }
                Exigir(GerenciadorDeQuests.ObjetivoCompleto(limpeza,0),"Contrato de preparo nao contou tres vitorias.");
                await Bruna(); await Missao("mestra_guilda",limpeza.Id,true);
                Exigir(!GerenciadorDeQuests.EstaAtiva(limpeza.Id),"Contrato de preparo ficou ativo.");
                await GarantirGame();
                Debug.Log("[TesterPlayer] Preparo boss: curas="+CurasDisponiveisBoss()+", comprar="+CurasAComprarBoss()+", gold="+G.Gold);
            }
            int pequenasNecessarias=CurasAComprarBoss();
            Exigir(G.Gold>=pequenasNecessarias*preco,"Limite de contratos sem ouro para reserva do boss.");
            if(pequenasNecessarias>0)
            {
                await Transitar("City"); await Interagir("Tácio"); await Dialogo("Ver mercadorias");
                for(int i=0;i<pequenasNecessarias;i++)await Comprar(CatalogoDeItens.IdPocaoPequena);
                UnityEngine.Object.FindAnyObjectByType<LojaUIManager>(FindObjectsInactive.Include).Fechar();
                await Transitar("Game");
            }
            await CurarFora(true);
            Exigir(d.Hp==d.HpMax && d.HpMax>=35 && d.Defesa>=19,"Defesa/vida fora das condicoes do preparo calculado.");
            Exigir(d.EnergiaAtual==d.EnergiaMaxima,"Energia nao recarregou por ataques basicos.");
            Exigir(CurasDisponiveisBoss()>=ReservaCurasBoss,"Cura fora consumiu reserva necessaria para o boss.");
        }
        finally { ValidacaoPercursoApresentacao.PreferirAtaqueBasico=false; }
    }

    static async Task Executar()
    {
        try
        {
            await Etapa("Nova partida e CasaLip", async () =>
            {
                UnityEngine.Object.FindAnyObjectByType<MainMenuManager>().NovoJogo(); await Cena("CasaLip"); await Task.Delay(150);
                await FalarAte("Apollo", "ato1_falou_apollo_casa");
                var terminal = UnityEngine.Object.FindAnyObjectByType<InteragirComTerminalCasa>();
                await AproximarTrigger(terminal.GetComponent<Collider2D>());
                await Codigo("nivel_agua=0\nwhile nivel_agua<100 do nivel_agua=nivel_agua+10 end");
                Exigir(Flag("ato1_casa_resolvida"), "Puzzle da casa nao resolveu.");
                await Transitar("City");
            });
            await Etapa("City: ouvir, entregar e aprender", async () =>
            {
                await FalarAte("Dara", "ato1_falou_dara"); await FalarAte("homem da fila", "ato1_falou_fila"); await FalarAte("Tácio", "ato1_falou_tacio");
                await FalarAte("Apollo — portão", "ato1_apollo_conclusao");
                await Missao("Apollo — portão", "q_main_agua", true);
                await Codigo("droid.aprenderTecnica(\"Soco de Sobrecarga\",1)\ndroid.subirAtributo(\"Dex\",3)");
                await FalarAte("Apollo — portão", "ato1_sino"); await FalarAte("Apollo — portão", "ato1_apollo_recusou");
            });
            await Etapa("Filtro da Dara e curas", async () =>
            {
                await Missao("dara", "q_agua_dara"); await Transitar("Interior_E02_Principal");
                await Puzzle("Bancada do filtro", "residuo=60\npronto=0\nwhile residuo>=8 do residuo=residuo-8 end\nif residuo<8 then pronto=1 end", "ato1_filtro_pronto");
                await FalarAte("ferreiro", "ato1_filtro_recolhido", "Levar o filtro para Dara");
                await Transitar("City"); await Missao("dara", "q_agua_dara", true);
                await Transitar("Game");
            });
            await Etapa("Primeiro combate e rota Desgarrado", async () =>
            {
                await FalarAte("Apollo", "ato1_apollo_explicou"); await Encontro("Sucata de Captura");
                await CurarFora(); await Encontro("Sucata da borda");
                await FalarAte("Apollo", "ato1_apos_primeiro_combate");
                await FalarAte("Rasha e Teodoro", "ato1_faccao_escolhida", "Seguir sem facção");
                await FalarAte("Rasha e Teodoro", "ato1_chegou_cratera"); await FalarAte("Rasha e Teodoro", "ato1_modulo_recebido");
                await Transitar("City");
            });
            await Etapa("Aceitar preparo de Onofre e Bruna", async () =>
            {
                await Missao("velho_viaduto", "q_onofre_provas"); await Transitar("Interior_E09_Terreo");
                await Missao("mestra_guilda", "q_limpeza_borda"); await Transitar("City"); await Transitar("Game");
            });
            await Etapa("Investigar marcas e vencer tres Sucatas", async () =>
            {
                await FalarAte("Placa da rua", "ato1_marca_placa"); await FalarAte("Degrau antigo", "ato1_marca_degrau"); await FalarAte("Canalização", "ato1_cratera_verdade");
                for (int i = 0; i < 3; i++) { await CurarFora(); await Encontro("Sucata da borda"); }
                await Transitar("City"); await Missao("velho_viaduto", "q_onofre_provas", true);
                await Transitar("Interior_E09_Terreo"); await Missao("mestra_guilda", "q_limpeza_borda", true); await Transitar("City");
            });
            await Etapa("Preparar tecnica, atributos e blindagem", async () =>
            {
                for (int i = 0; (G.Gold < 83 || G.DroidDoJogador.Pontos.PontosDisponiveis < 11) && i < 8; i++)
                { await Transitar("Game"); await CurarFora(); await Encontro("Sucata da borda"); await Transitar("City"); }
                Exigir(G.DroidDoJogador.Pontos.PontosDisponiveis >= 11, "Preparo exige onze pontos ganhos normalmente.");
                await Codigo("droid.melhorarTecnica(\"Soco de Sobrecarga\",2)\ndroid.subirAtributo(\"For\",2)\ndroid.subirAtributo(\"Dex\",2)\ndroid.subirAtributo(\"Vit\",5)");
                await Interagir("Tácio"); await Dialogo("Ver mercadorias");
                await Comprar("placa_de_blindagem");
                await Comprar(CatalogoDeItens.IdOleoDePrecisao);
                UnityEngine.Object.FindAnyObjectByType<LojaUIManager>(FindObjectsInactive.Include).Fechar();
                await Codigo("droid.equipar(\"placa_de_blindagem\")"); await CurarFora(); await Transitar("Game");
            });
            await Etapa("Energia, unidade e revelacao de Ivo", async () =>
            {
                await FalarAte("Apollo — painel de energia", "ato1_leu_painel");
                await Puzzle("Painel de energia", "energia=0\nporta_aberta=0\nwhile energia<50 do energia=energia+10 end\nif energia>=50 then porta_aberta=1 end", "ato1_puzzle_energia");
                await FalarAte("Unidade de manutenção", "ato1_viu_unidade");
                await FalarAte("Ivo", "ato1_ivo_reconheceu"); await FalarAte("Ivo", "ato1_ivo_celia"); await FalarAte("Ivo", "ato1_ivo_confessou");
            });
            await Etapa("Vencer boss com progresso do percurso", async () =>
            {
                await PrepararBoss(); ValidacaoPercursoApresentacao.Capturar("tester-antes-boss");
                await Encontro("Ivo"); Exigir(Flag("ato1_sessao6"), "Vitoria do boss nao persistiu.");
                ValidacaoPercursoApresentacao.Capturar("tester-vitoria-boss");
            });
            await Etapa("Ivo: preco, resolucao e caderno", async () =>
            {
                await GarantirGame();
                if(DialogoUIManager.Instancia.EstaAberto)await Dialogo();
                await FalarAte("Ivo","ato1_preco_pago"); await FalarAte("Ivo","ato1_ivo_resolvido");
                await FalarAte("Ivo","ato1_caderno_2060");
            });
            await Etapa("Agua, entrega da estacao e reparo de Apollo", async () =>
            {
                await GarantirCity(); await FalarAte("Apollo — portão","ato1_agua_voltou");
                await Missao("Apollo — portão","q_main_estacao",true);
                if(!Flag("ato1_perna_reparada"))
                {
                    if(!GerenciadorDeQuests.EstaConcluida("q_perna_apollo"))
                    {
                        for(int i=0;G.ObterQuantidadeDeItem(CatalogoDeItens.IdSucataLimpa)<3 && i<5;i++)
                        { await Transitar("Game"); await Encontro("Sucata da borda"); await Transitar("City"); }
                        Exigir(G.ObterQuantidadeDeItem(CatalogoDeItens.IdSucataLimpa)>=3,"Falta sucata para reparo.");
                    }
                    await Transitar("Interior_E02_Principal"); await Missao("ferreiro","q_perna_apollo");
                    await Missao("ferreiro","q_perna_apollo",true); await FalarAte("ferreiro","ato1_perna_reparada");
                    await Transitar("City");
                }
            });
            await Etapa("Nadir: chegada, oferta, assembleia e resposta", async () =>
            {
                await GarantirCity(); await FalarAte("Apollo — portão","ato1_nadir");
                await Esperar(()=>UnityEngine.Object.FindObjectsByType<MoradorInterativo>().Any(m=>m.idNpc=="nadir"),"Nadir nao apareceu.");
                await FalarAte("nadir","ato1_ouviu_nadir"); await FalarAte("nadir","ato1_assembleia");
                await FalarAte("nadir","ato1_dara_falou_assembleia"); await FalarAte("nadir","ato1_cidade_respondeu");
            });
            await Etapa("Noite, mochila, cantil e encerramento do Ato 1", async () =>
            {
                await FalarAte("Apollo — portão","ato1_conversa_noturna"); await FalarAte("Apollo — portão","ato1_mochila_pronta");
                await FalarAte("Dara","ato1_cantil_recebido"); await FalarAte("Apollo — portão","ato1_concluido","Encerrar a demo");
                ValidacaoPercursoApresentacao.Capturar("tester-ato1-concluido");
            });
            await Etapa("Entregar O nome na lista e conferir todas as quests", async () =>
            {
                await GarantirCity(); await Missao("Apollo — portão","q_main_partida",true);
                if(GerenciadorDeQuests.EstaAtiva("q_ivo_terca"))
                {
                    await Transitar("Game");
                    await Puzzle("Calibragem da bomba dois","forca_lida=droid.obterAtributo(\"For\")\nprevisao=forca_lida*2","ato1_calibragem_feita");
                    await Missao("Ivo","q_ivo_terca",true); await Transitar("City");
                }
                Exigir(Flag("ato1_concluido"),"Final narrativo nao concluiu.");
                foreach(string id in new[]{"q_main_agua","q_agua_dara","q_onofre_provas","q_limpeza_borda","q_main_estacao","q_perna_apollo","q_main_partida"})
                    Exigir(GerenciadorDeQuests.EstaConcluida(id),"Quest do percurso nao entregue: "+id);
                Exigir(GerenciadorDeQuests.Ativas().Count==0,"Quests ativas restantes: "+string.Join(",",GerenciadorDeQuests.Ativas().Select(q=>q.Id)));
            });
            relatorio.resultado = "PASSOU: Ato 1 encerrado e quests do percurso entregues";
        }
        catch (OperationCanceledException) { relatorio.resultado = "CANCELADO"; }
        catch (Exception e)
        {
            relatorio.resultado = "BLOQUEADO"; relatorio.erro = e.ToString();
            if (EditorApplication.isPlaying) ValidacaoPercursoApresentacao.Capturar("tester-bloqueio");
            Debug.LogError("[TesterPlayer] " + relatorio.etapa + "\n" + e);
        }
        finally { relatorio.executando = false; relatorio.fim = DateTime.UtcNow.ToString("O"); Gravar(); }
    }
}
