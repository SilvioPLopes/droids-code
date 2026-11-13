using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ferramenta manual de QA. Nunca altera flags, itens, XP ou regras do jogo.
public static class ValidacaoPercursoApresentacao
{
    public static bool CancelamentoSolicitado { get; set; }
    public static bool PreferirAtaqueBasico { get; set; }
    const string Pasta = "C:/Users/silve/DroidsCode/output/apresentacao-jogavel/2026-11-12/";
    public static string Estado()
    {
        var g = GerenciadorDeEstado.Instancia;
        var p = GameObject.FindGameObjectWithTag("Player");
        var d = DialogoUIManager.Instancia;
        var t = UnityEngine.Object.FindAnyObjectByType<TerminalUIManager>(FindObjectsInactive.Include);
        return "scene=" + SceneManager.GetActiveScene().name + " player=" + (p ? p.transform.position.ToString() : "-") +
            " menu=" + g.MenuAberto + " hp=" + g.DroidDoJogador.Hp + " gold=" + g.Gold + " pontos=" + g.DroidDoJogador.Pontos.PontosDisponiveis +
            "\nflags=" + string.Join(",", g.TodasAsFlags.Where(x => x.Value).Select(x => x.Key)) +
            "\ninventario=" + string.Join(",", g.Inventario.Select(x => x.Key + ":" + x.Value)) +
            "\ndialog=" + (d && d.EstaAberto ? d.textoFala.text : "fechado") +
            "\nescolhas=" + (d && d.painelDeEscolhas && d.painelDeEscolhas.gameObject.activeInHierarchy ?
                string.Join("|", d.painelDeEscolhas.GetComponentsInChildren<Button>().Select(Label)) : "-") +
            "\nterminal=" + (t && t.EstaAberto ? t.textoSaida.text : "fechado");
    }
    static string Label(Button b) => b.GetComponentInChildren<TMP_Text>()?.text ?? b.name;
    public static void Capturar(string nome) => ScreenCapture.CaptureScreenshot(Pasta + nome + ".png");
    public static string Interagir(string nome)
    {
        var m = UnityEngine.Object.FindObjectsByType<MoradorInterativo>(FindObjectsSortMode.None)
            .Single(x => x.gameObject.name == nome);
        var p = GameObject.FindGameObjectWithTag("Player");
        if (!m.GetComponents<Collider2D>().Any(c => c.isTrigger && c.IsTouching(p.GetComponent<Collider2D>())))
            return "FORA_DO_ALCANCE: " + nome + " @" + m.transform.position;
        m.Interagir();
        return Estado();
    }
    public static async Task<string> Dialogo(string escolha = "")
    {
        var d = DialogoUIManager.Instancia;
        bool escolhida = false;
        for (int i = 0; i < 120 && d != null && d.EstaAberto; i++)
        {
            if (CancelamentoSolicitado) return "CANCELADO";
            if (d.painelDeEscolhas && d.painelDeEscolhas.gameObject.activeInHierarchy)
            {
                if (string.IsNullOrEmpty(escolha) || escolhida) return Estado();
                var b = d.painelDeEscolhas.GetComponentsInChildren<Button>().SingleOrDefault(x => Label(x).Contains(escolha));
                if (b == null) return "ESCOLHA_NAO_ENCONTRADA\n" + Estado();
                b.onClick.Invoke(); escolhida = true;
            }
            else { d.textoFala.ForceMeshUpdate(); d.Avancar(); }
            await Task.Delay(30);
        }
        return Estado();
    }
    public static string Codigo(string codigo)
    {
        if (CancelamentoSolicitado) return "CANCELADO";
        var t = LocalizadorDeTerminal.Encontrar();
        if (t == null) throw new InvalidOperationException("Terminal utilizavel nao encontrado.");
        if (!t.EstaAberto) t.Abrir();
        t.campoDeCodigo.text = codigo;
        t.botaoExecutar.onClick.Invoke();
        return "CODIGO_ENVIADO";
    }
    // BFS de passabilidade usa o footprint real do player, colisores e layers efetivos.
    // Movimento via Rigidbody2D, sem teleporte. O controlador normal é restaurado ao final.
    public static Task<string> Caminhar(float x, float y) => CaminharCore(x, y, null);
    public static Task<string> CaminharAteTrigger(Collider2D alvo) => CaminharCore(alvo.bounds.center.x, alvo.bounds.center.y, alvo);
    static async Task<string> CaminharCore(float x, float y, Collider2D alvoDeContato, float passo = .25f)
    {
        if (CancelamentoSolicitado) return "CANCELADO";
        if (!EditorApplication.isPlaying) return "FORA_DE_PLAY";
        var p = GameObject.FindGameObjectWithTag("Player");
        var mov = p.GetComponent<PlayerMovement>();
        var rb = p.GetComponent<Rigidbody2D>();
        var col = p.GetComponent<Collider2D>();
        var origem = rb.position;
        var destino = new Vector2(x, y);
        var offset = (Vector2)col.bounds.center - origem;
        var size = (Vector2)col.bounds.size * .97f;
        var min = Vector2.Min(origem, destino) - Vector2.one * 8;
        var max = Vector2.Max(origem, destino) + Vector2.one * 8;
        int w = Mathf.CeilToInt((max.x - min.x) / passo) + 1;
        int h = Mathf.CeilToInt((max.y - min.y) / passo) + 1;
        Vector2 Pos(int k) => min + new Vector2(k % w, k / w) * passo;
        int Key(Vector2 v) => Mathf.Clamp(Mathf.RoundToInt((v.y-min.y)/passo),0,h-1)*w + Mathf.Clamp(Mathf.RoundToInt((v.x-min.x)/passo),0,w-1);
        var cache = new Dictionary<int,bool>();
        Collider2D[] Sobrepostos(Vector2 centro)
        {
            if(col is CircleCollider2D)return Physics2D.OverlapCircleAll(centro,size.x*.5f);
            if(col is CapsuleCollider2D capsula)return Physics2D.OverlapCapsuleAll(centro,size,capsula.direction,0);
            return Physics2D.OverlapBoxAll(centro,size,0);
        }
        bool Livre(int k)
        {
            if (cache.TryGetValue(k,out bool l)) return l;
            l = !Sobrepostos(Pos(k)+offset).Any(c => c.gameObject != p &&
                !Physics2D.GetIgnoreLayerCollision(p.layer,c.gameObject.layer) &&
                (!c.isTrigger || (c.GetComponent<TransicaoDeCena>() != null &&
                    (alvoDeContato != null ? c != alvoDeContato : Vector2.Distance(c.bounds.center,destino)>1.5f))));
            cache[k]=l; return l;
        }
        bool Chegou(int k) => Livre(k) && (alvoDeContato == null
            ? Vector2.Distance(Pos(k),destino)<.20f
            : Sobrepostos(Pos(k)+offset).Contains(alvoDeContato));
        int start=Key(origem), goal=-1;
        var fila=new Queue<int>(); var anterior=new Dictionary<int,int>{{start,-1}}; fila.Enqueue(start);
        int[] dx={1,-1,0,0}, dy={0,0,1,-1};
        while(fila.Count>0 && anterior.Count<40000)
        {
            int k=fila.Dequeue();
            if(Chegou(k)){goal=k;break;}
            for(int j=0;j<4;j++)
            {
                int nx=k%w+dx[j], ny=k/w+dy[j];
                if(nx<0||ny<0||nx>=w||ny>=h)continue;
                int n=ny*w+nx;
                if(anterior.ContainsKey(n)||!Livre(n))continue;
                anterior[n]=k; fila.Enqueue(n);
            }
        }
        if(goal<0 && passo>.126f)return await CaminharCore(x,y,alvoDeContato,.125f);
        if(goal<0)return "SEM_CAMINHO: "+origem+" -> "+destino+" footprint="+size;
        var rota=new List<Vector2>(); for(int k=goal;k!=start;k=anterior[k])rota.Add(Pos(k)); rota.Reverse();
        string cena=SceneManager.GetActiveScene().name; bool antes=mov.enabled; mov.enabled=false;
        int frames=0; Vector2 ultimo=rb.position; int parado=0;
        try
        {
            foreach(var alvo in rota)
            {
                while(rb != null && Vector2.Distance(rb.position,alvo)>(alvoDeContato!=null?.01f:.08f))
                {
                    if(CancelamentoSolicitado)return "CANCELADO";
                    if(SceneManager.GetActiveScene().name!=cena)return "TRANSICAO_FISICA: "+SceneManager.GetActiveScene().name;
                    if(GerenciadorDeEstado.Instancia.MenuAberto)return "INTERROMPIDO_POR_MENU\n"+Estado();
                    if(++frames>1800)return "LIMITE_DE_MOVIMENTO\n"+Estado();
                    rb.MovePosition(Vector2.MoveTowards(rb.position,alvo,mov.velocidade*.02f));
                    var anim=p.GetComponent<Animator>();
                    var dir=(alvo-rb.position).normalized;
                    if(anim!=null){anim.SetFloat("MoveX",dir.x);anim.SetFloat("MoveY",dir.y);anim.SetFloat("Speed",1);}
                    await Task.Delay(20);
                    if(rb==null)return "TRANSICAO_FISICA: "+SceneManager.GetActiveScene().name;
                    if(Vector2.Distance(ultimo,rb.position)<.005f)parado++;else parado=0;
                    ultimo=rb.position;
                    if(parado>35)return "COLISAO_BLOQUEOU @"+rb.position+" alvo="+alvo;
                }
            }
            return "CAMINHO_FISICO_OK "+origem+" -> "+rb.position+"\n"+Estado();
        }
        finally
        {
            if(mov!=null)mov.enabled=antes;
            if(rb!=null)rb.linearVelocity=Vector2.zero;
            if(p!=null && p.TryGetComponent<Animator>(out var anim))anim.SetFloat("Speed",0);
        }
    }
    public static string Atores() => string.Join("\n",UnityEngine.Object.FindObjectsByType<MoradorInterativo>(FindObjectsSortMode.None)
        .Select(m=>m.gameObject.name+"|"+m.idNpc+"|"+m.transform.position));
    public static int VidaSeguraParaBoss(int ataque, int defesa, int danoPorTurno, string proximaAcao)
    {
        if(proximaAcao==DroidsCode.Combat.DroidDoIvo.Carga || proximaAcao==DroidsCode.Combat.DroidDoIvo.Descanso)
            return Math.Max(0,danoPorTurno);
        int extra=proximaAcao==DroidsCode.Combat.DroidDoIvo.Prensa?0:12;
        return (int)Math.Round(Math.Max(1,ataque+extra-defesa)*DroidsCode.DroidCore.TabelaDeCombate.MultiplicadorDano_Critico)+Math.Max(0,danoPorTurno);
    }
    public static string EscolherCura()
    {
        var g=GerenciadorDeEstado.Instancia;
        int falta=g.DroidDoJogador.HpMax-g.DroidDoJogador.Hp;
        var itens=new[]{"pocao_pequena","pocao_media","pocao_grande"}
            .Where(id=>g.ObterQuantidadeDeItem(id)>0).Select(DroidsCode.DroidCore.CatalogoDeItens.Obter)
            .OrderBy(i=>i.CuraHp).ToArray();
        return (itens.FirstOrDefault(i=>i.CuraHp>=falta)??itens.LastOrDefault())?.Id;
    }
    [Serializable] class AcaoDeCombate
    {
        public string instante, acao, mensagem;
        public int hp, hpMax, defesa, energia, inimigoHp;
    }
    static void RegistrarCombate(BattleManager b,string acao)
    {
        var d=GerenciadorDeEstado.Instancia.DroidDoJogador;
        var linha=new AcaoDeCombate { instante=DateTime.UtcNow.ToString("O"),acao=acao,hp=d.Hp,hpMax=d.HpMax,
            defesa=d.Defesa,energia=d.EnergiaAtual,inimigoHp=Mathf.RoundToInt(b.sliderInimigo.normalizedValue*b.hpMaxInimigo),mensagem=b.textoMensagem.text };
        string pasta=Pasta+"tester-player"; System.IO.Directory.CreateDirectory(pasta);
        System.IO.File.AppendAllText(pasta+"/combate.jsonl",JsonUtility.ToJson(linha)+Environment.NewLine);
    }
    public static async Task<string> Combater(int limite=60)
    {
        var b=UnityEngine.Object.FindAnyObjectByType<BattleManager>();
        if(b==null)return "SEM_BATALHA";
        bool usouOleo=false;
        int acoes=0, consultas=0;
        var prazo=DateTime.UtcNow.AddMinutes(4);
        while(b!=null && acoes<limite && consultas++<limite*4 && DateTime.UtcNow<prazo)
        {
            if(CancelamentoSolicitado)return "CANCELADO";
            if(b.botaoAtacar.interactable)
            {
                var d=GerenciadorDeEstado.Instancia.DroidDoJogador;
                int limiar=Mathf.FloorToInt(d.HpMax*.45f);
                if(b.hpMaxInimigo>=150)
                {
                    const string prefixo="Próxima ação: ";
                    int pos=b.textoMensagem.text.LastIndexOf(prefixo,StringComparison.Ordinal);
                    string proxima=pos<0?"":b.textoMensagem.text.Substring(pos+prefixo.Length)
                        .Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim().TrimEnd('.');
                    limiar=Math.Max(limiar,VidaSeguraParaBoss(b.atkInimigo,d.Defesa,d.EfeitosDeDanoAtivos.Sum(e=>e.DanoPorTurno),proxima));
                }
                if(d.Hp<=limiar && d.Hp<d.HpMax)
                {
                    string cura=EscolherCura();
                    if(cura!=null)
                    {
                        RegistrarCombate(b,"curar:"+cura);
                        b.botaoItem.onClick.Invoke();
                        await Task.Delay(50);
                        if(!b.TentarUsarItemDaBag(cura))throw new InvalidOperationException("Cura pela Bag foi recusada.");
                        acoes++; await Task.Delay(1800);continue;
                    }
                }
                if(b.hpMaxInimigo>=150 && !usouOleo && GerenciadorDeEstado.Instancia.ObterQuantidadeDeItem(DroidsCode.DroidCore.CatalogoDeItens.IdOleoDePrecisao)>0)
                {
                    RegistrarCombate(b,"oleo");
                    b.botaoItem.onClick.Invoke(); await Task.Delay(50);
                    if(!b.TentarUsarItemDaBag(DroidsCode.DroidCore.CatalogoDeItens.IdOleoDePrecisao))throw new InvalidOperationException("Oleo pela Bag foi recusado.");
                    usouOleo=true; acoes++; await Task.Delay(1800); continue;
                }
                b.botaoAtacar.onClick.Invoke(); await Task.Delay(50);
                if(b!=null && b.painelListaDeAtaques.gameObject.activeInHierarchy)
                {
                    var golpes=b.painelListaDeAtaques.GetComponentsInChildren<Button>().Where(x=>x.interactable && Label(x)!="Voltar").ToArray();
                    var golpe=(PreferirAtaqueBasico?golpes.FirstOrDefault(x=>Label(x).Contains("Basico")):golpes.FirstOrDefault(x=>Label(x).Contains("Sobrecarga"))) ?? golpes.FirstOrDefault();
                    if(golpe==null)throw new InvalidOperationException("SEM_GOLPE_DISPONIVEL\n"+Estado());
                    RegistrarCombate(b,"golpe:"+Label(golpe));
                    golpe.onClick.Invoke();
                }
                acoes++;
            }
            await Task.Delay(1800);
        }
        for(int espera=0;b!=null && !b.botaoAtacar.interactable && espera<50;espera++)
        {
            if(CancelamentoSolicitado)return "CANCELADO";
            await Task.Delay(100);
        }
        if(b!=null)throw new InvalidOperationException("Limite de batalha do tester: acoes="+acoes+", consultas="+consultas);
        Debug.Log("[TesterPlayer] Combate encerrado: acoes="+acoes+", consultas="+consultas);
        return Estado();
    }
}
