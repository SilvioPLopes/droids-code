using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using DroidsCode.Casa;

// Fixture manual, arena temporaria longe do mapa, sem IO de save.
public static class ValidacaoRotasTester
{
    public static async Task<string> VerificarManobraCurta()
    {
        if(!EditorApplication.isPlaying)throw new Exception("Fixture exige Play sem auditor ativo.");
        var anterior=UnityEngine.Object.FindAnyObjectByType<GerenciadorDeEstado>();
        var g=GerenciadorDeEstado.Instancia;
        var player=new GameObject("QA_Player_Manobra_Curta");
        var portal=new GameObject("QA_Portal_Manobra_Curta");
        try
        {
            if(g.MenuAberto)throw new Exception("Fixture exige menu fechado.");
            var basePos=new Vector2(1000,1000);
            player.transform.position=basePos+new Vector2(-2.58850026f,.3735531f);
            var rb=player.AddComponent<Rigidbody2D>();rb.gravityScale=0;rb.constraints=RigidbodyConstraints2D.FreezeRotation;
            var col=player.AddComponent<CircleCollider2D>();col.radius=.3f;col.offset=new Vector2(0,.62f);
            var anim=player.AddComponent<Animator>();
            anim.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AssetDatabase.GUIDToAssetPath("b41d4dbbce30e7f459929eeb6bd56b9d"));
            player.AddComponent<SpriteRenderer>();var movimento=player.AddComponent<PlayerMovement>();
            portal.transform.position=basePos+new Vector2(-1.87f,1.015f);
            var trigger=portal.AddComponent<BoxCollider2D>();trigger.size=new Vector2(.75f,.12f);trigger.isTrigger=true;
            portal.AddComponent<TransicaoDeCena>();Physics2D.SyncTransforms();
            bool Objetivo(Vector2 p) {p-=basePos;return p.x>=-2.57f && p.x< -2.32f && p.y>=.215f && p.y<.465f;}
            var metodo=typeof(TesterPlayer).GetMethod("PlanejarManobraCurtaRotas",BindingFlags.Static|BindingFlags.NonPublic);
            var comandos=(Vector2[])metodo.Invoke(null,new object[]{col,rb.position,col.offset,movimento.velocidade*Time.fixedDeltaTime,new Func<Vector2,bool>(Objetivo)});
            if(comandos.Length!=2)throw new Exception("Caso estreito nao encontrou manobra de dois comandos.");
            var fila=new System.Collections.Generic.Queue<Vector2>(comandos);
            var driver=player.AddComponent<DriverMovimentoTester>();bool terminou=false;string erro="";
            driver.EscolherDirecao=real=>
            {
                if(Objetivo(real)){terminou=true;return Vector2.zero;}
                if(fila.Count==0){erro="Comandos terminaram fora do setor real.";return Vector2.zero;}
                return fila.Dequeue();
            };
            driver.AoPasso=passo=> {if(col.IsTouching(trigger))erro="Manobra tocou portal nao escolhido.";};
            var limite=DateTime.UtcNow.AddSeconds(2);
            while(!terminou && erro.Length==0 && DateTime.UtcNow<limite)await Task.Delay(20);
            driver.Parar();
            if(!terminou || erro.Length>0 || driver.Erro.Length>0 || !Objetivo(rb.position) || !movimento.enabled)
                throw new Exception("Manobra fisica reprovou: "+erro+"; "+driver.Erro+"; pose="+(rb.position-basePos));
            string resultado="PASSOU: dois comandos reais, velocidade5/corpo.30 preservados; setor alcancado sem tocar portal. Pose="+(rb.position-basePos);
            System.IO.File.WriteAllText(System.IO.Path.GetFullPath("output/apresentacao-jogavel/2026-11-12/circulacao/fixture-manobra-curta.txt"),resultado);
            return resultado;
        }
        finally
        {
            UnityEngine.Object.Destroy(player);UnityEngine.Object.Destroy(portal);
            if(anterior==null)UnityEngine.Object.Destroy(g.gameObject);
        }
    }

    public static string VerificarPortalAlemDoWaypoint()
    {
        if (!EditorApplication.isPlaying) throw new Exception("Fixture exige Play.");
        var player = new GameObject("QA_Corpo_Passo_Integral");
        var portal = new GameObject("QA_Portal_Passo_Integral");
        try
        {
            player.transform.position = new Vector3(1000,1000);
            var rb = player.AddComponent<Rigidbody2D>(); rb.bodyType = RigidbodyType2D.Kinematic;
            var corpo = player.AddComponent<CircleCollider2D>(); corpo.radius = .30f;
            portal.transform.position = new Vector3(1000,1000.40944f);
            var trigger = portal.AddComponent<BoxCollider2D>(); trigger.size = new Vector2(.75f,.12f); trigger.isTrigger = true;
            portal.AddComponent<TransicaoDeCena>();
            Physics2D.SyncTransforms();
            var metodo = typeof(TesterPlayer).GetMethod("SegmentoBloqueadoRotas", BindingFlags.Static | BindingFlags.NonPublic);
            var direcao = new Vector2(1,1).normalized;
            string Verificar(float tick, Collider2D escolhido) => (string)metodo.Invoke(null,
                new object[] { corpo, new Vector2(1000,1000), direcao, tick, escolhido, .04164f });
            if (Verificar(.04164f,null).Length != 0) throw new Exception("Fixture nao isola portal alem do waypoint.");
            if (!Verificar(.10f,null).StartsWith("PORTAL_NAO_ESCOLHIDO")) throw new Exception("Tick completo pode acionar portal nao escolhido.");
            if (Verificar(.10f,trigger).Length != 0) throw new Exception("Portal escolhido foi recusado.");
            portal.transform.position = new Vector3(1000,1000.3658f); Physics2D.SyncTransforms();
            var filtro = new ContactFilter2D(); filtro.NoFilter(); filtro.useTriggers = true;
            var sobrepostos = new System.Collections.Generic.List<Collider2D>();
            Physics2D.OverlapCircle(new Vector2(1000,1000),.30f,filtro,sobrepostos);
            if (!sobrepostos.Contains(trigger)) throw new Exception("Fixture lateral nao reproduziu sobreposicao fisica.");
            if (!Verificar(.001f,null).StartsWith("PORTAL_NAO_ESCOLHIDO")) throw new Exception("Contato lateral escapou do guard.");
            if (Verificar(.001f,trigger).Length != 0) throw new Exception("Contato lateral escolhido foi recusado.");
            return "PASSOU: tick integral e sobreposicao lateral recusam portal nao escolhido; escolhido permitido.";
        }
        finally { UnityEngine.Object.Destroy(player); UnityEngine.Object.Destroy(portal); }
    }

    public static string VerificarMapa()
    {
        if(!EditorApplication.isPlaying)throw new Exception("Fixture de mapa exige Play para objetos temporarios.");
        var player=new GameObject("QA_Corpo_Grade");
        var parede=new GameObject("QA_Parede_Fina");
        var alvo=new GameObject("QA_Contato_Atras_Da_Parede");
        try
        {
            player.transform.position=new Vector3(1000,1000);
            var rb=player.AddComponent<Rigidbody2D>(); rb.gravityScale=0;
            player.AddComponent<CircleCollider2D>().radius=.03f;
            parede.transform.position=new Vector3(1001.0925f,1000);
            parede.AddComponent<BoxCollider2D>().size=new Vector2(.005f,8);
            alvo.transform.position=new Vector3(1002,1000);
            var contato=alvo.AddComponent<CircleCollider2D>(); contato.radius=.1f; contato.isTrigger=true;
            Physics2D.SyncTransforms();
            var mapa=AnaliseCirculacaoTester.Criar(player,new Bounds(new Vector3(1001,1000),new Vector3(4,4,0)));
            var r=mapa.Planejar(new Vector2(1000,1000),new Vector2(1002,1000));
            if(r.sucesso || !r.bloqueadores.Any(x=>x.Contains("QA_Parede_Fina")))
                throw new Exception("Grade aprovou segmento que cruza parede entre os nos: "+r.erro);
            var ocupado=new Vector2(1001.0925f,1000);
            if(!mapa.Ocupado(ocupado) || mapa.Planejar(ocupado,new Vector2(1000,1000)).erro!="ORIGEM_OCUPADA")
                throw new Exception("Spawn em solido nao foi recusado.");
            if(mapa.Planejar(new Vector2(1000,1000),new Vector2(1002,1000),contato,0).sucesso)
                throw new Exception("Contato do outro lado da parede virou falso verde.");
            return "PASSOU: parede entre nos, spawn ocupado e contato inacessivel recusados.";
        }
        finally
        {
            UnityEngine.Object.Destroy(player);UnityEngine.Object.Destroy(parede);UnityEngine.Object.Destroy(alvo);
        }
    }
    [Serializable] class CasoGeometrico
    {
        public string cena,origem,alvo,erro;
        public float x,y;
        public bool chegadaLivre,sucesso;
        public int visitados,pontos;
        public string[] bloqueadores;
    }
    [Serializable] class RelatorioGeometrico
    {
        public string data,limite="Conectividade geometrica fora de Play; nao comprova input nem movimento.";
        public int cenas,chegadas,aprovados,reprovados;
        public System.Collections.Generic.List<CasoGeometrico> casos=new System.Collections.Generic.List<CasoGeometrico>();
    }
    public static string AuditarGeometria(string inventario="inventario-pos-mobiliario",string nomeSaida="geometria-pos-mobiliario.json")
    {
        if(EditorApplication.isPlaying)throw new Exception("Geometria salva exige fora de Play.");
        var setup=EditorSceneManager.GetSceneManagerSetup();
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new Exception("Cena suja.");
        var inv=JsonUtility.FromJson<InventarioCirculacaoTester.InventarioDto>(System.IO.File.ReadAllText(
            System.IO.Path.GetFullPath("output/apresentacao-jogavel/2026-11-12/circulacao/"+inventario+"/inventario.json")));
        var rel=new RelatorioGeometrico {data=DateTime.UtcNow.ToString("O")};
        string saida=System.IO.Path.GetFullPath("output/apresentacao-jogavel/2026-11-12/circulacao/"+nomeSaida);
        try
        {
            foreach(var c in inv.cenas.Where(c=>c.nome!="City"))
            {
                var cena=EditorSceneManager.OpenScene(c.path);
                var player=GameObject.FindGameObjectWithTag("Player");
                var sprites=cena.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<SpriteRenderer>(true));
                Bounds bounds=sprites.Where(s=>s.sprite!=null && s.GetComponentInParent<PlayerMovement>()==null)
                    .OrderByDescending(s=>s.bounds.size.x*s.bounds.size.y).First().bounds;
                if(c.nome=="CasaLip")bounds=UnityEngine.Object.FindObjectsByType<EdgeCollider2D>(FindObjectsSortMode.None)
                    .OrderByDescending(e=>e.bounds.size.x*e.bounds.size.y).First().bounds;
                var mapa=AnaliseCirculacaoTester.Criar(player,bounds);
                var chegadas=new System.Collections.Generic.List<Tuple<string,Vector2>>();
                foreach(var origem in inv.cenas)
                {
                    foreach(var porta in origem.portais.Where(p=>p.destino==c.nome && p.enabled && p.active))
                        chegadas.Add(Tuple.Create(origem.nome+"/"+porta.nome,new Vector2(porta.pontoChegadaX,porta.pontoChegadaY)));
                    foreach(var npc in origem.npcs.Where(n=>n.destinoDeVisita==c.nome && n.enabled && n.active))
                        chegadas.Add(Tuple.Create(origem.nome+"/visita/"+npc.nome,new Vector2(npc.chegadaDaVisitaX,npc.chegadaDaVisitaY)));
                }
                var alvos=UnityEngine.Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None).Where(a=>a.isTrigger && a.enabled &&
                    (a.GetComponent<TransicaoDeCena>()!=null || a.GetComponent<MoradorInterativo>()!=null ||
                    a.GetComponent<TerminalDoInterior>()!=null || a.GetComponent<InteragirComTerminalCasa>()!=null ||
                    a.GetComponent<GatilhoDeDialogo>()!=null)).OrderBy(a=>DriverMovimentoTester.Caminho(a.transform)).ToArray();
                foreach(var chegada in chegadas)
                {
                    rel.chegadas++;
                    bool livre=!mapa.Ocupado(chegada.Item2);
                    foreach(var alvo in alvos)
                    {
                        var r=mapa.Planejar(chegada.Item2,alvo.bounds.center,alvo,0);
                        bool aprovado=livre&&r.sucesso;
                        rel.casos.Add(new CasoGeometrico {cena=c.nome,origem=chegada.Item1,x=chegada.Item2.x,y=chegada.Item2.y,
                            chegadaLivre=livre,alvo=DriverMovimentoTester.Caminho(alvo.transform),sucesso=aprovado,
                            erro=livre?r.erro:"CHEGADA_OCUPADA: "+r.erro,visitados=r.visitados,pontos=r.pontos.Length,bloqueadores=r.bloqueadores});
                        if(aprovado)rel.aprovados++;else rel.reprovados++;
                    }
                }
                rel.cenas++;
                System.IO.File.WriteAllText(saida,JsonUtility.ToJson(rel,true));
            }
        }
        finally
        {
            EditorSceneManager.RestoreSceneManagerSetup(setup);
            System.IO.File.WriteAllText(saida,JsonUtility.ToJson(rel,true));
        }
        return "GEOMETRIA: cenas="+rel.cenas+", chegadas="+rel.chegadas+", trechos="+rel.casos.Count+", aprovados="+rel.aprovados+", reprovados="+rel.reprovados+" | "+saida;
    }
    public static async Task<string> Executar()
    {
        var tipo=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("DriverMovimentoTester")).FirstOrDefault(t=>t!=null);
        if(tipo==null)throw new InvalidOperationException("Driver do controlador real ainda nao implementado.");
        if(!EditorApplication.isPlaying)throw new InvalidOperationException("Fixture fisica exige Play sem tester ativo.");
        var anterior=UnityEngine.Object.FindAnyObjectByType<GerenciadorDeEstado>();
        var g=GerenciadorDeEstado.Instancia;
        bool menuRegistrado=false,cancelamento=ValidacaoPercursoApresentacao.CancelamentoSolicitado;
        var player=new GameObject("QA_Player_Circulacao");
        var parede=new GameObject("QA_Parede_Circulacao");
        try
        {
            if(g.MenuAberto)throw new Exception("Fixture exige inicio sem menu aberto.");
            ValidacaoPercursoApresentacao.CancelamentoSolicitado=false;
            player.transform.position=new Vector3(1000,1000);
            var rb=player.AddComponent<Rigidbody2D>(); rb.gravityScale=0; rb.constraints=RigidbodyConstraints2D.FreezeRotation;
            var col=player.AddComponent<CircleCollider2D>(); col.radius=.3f; col.offset=new Vector2(0,.62f);
            var anim=player.AddComponent<Animator>();
            anim.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AssetDatabase.GUIDToAssetPath("b41d4dbbce30e7f459929eeb6bd56b9d"));
            player.AddComponent<SpriteRenderer>();
            var movimento=player.AddComponent<PlayerMovement>();
            parede.transform.position=new Vector3(1002,1000.6f);
            parede.AddComponent<BoxCollider2D>().size=new Vector2(.4f,2);
            Physics2D.SyncTransforms();
            var driver=player.AddComponent(tipo);
            if(driver==null)throw new Exception("Unity nao anexou o driver ao player temporario.");
            tipo.GetField("DeveCancelar").SetValue(driver,new Func<bool>(()=>ValidacaoPercursoApresentacao.CancelamentoSolicitado));
            var direcao=tipo.GetProperty("DirecaoSolicitada");
            if(direcao==null)throw new Exception("Driver sem API de intencao.");
            direcao.SetValue(driver,Vector2.right);
            await Task.Delay(350);
            if(rb.position.x<1000.5f || !movimento.enabled)throw new Exception("Controlador real nao movimentou o player habilitado.");
            g.RegistrarMenuAberto(); menuRegistrado=true; await Task.Delay(80); var parado=rb.position;
            direcao.SetValue(driver,Vector2.left); await Task.Delay(220);
            if(Vector2.Distance(parado,rb.position)>.02f)throw new Exception("Driver ignorou trava do menu.");
            g.RegistrarMenuFechado(); menuRegistrado=false; direcao.SetValue(driver,Vector2.right); await Task.Delay(400);
            if(rb.position.x>1001.53f || rb.position.x<1001.4f)throw new Exception("Parede nao reteve o volume integral do player: "+rb.position);
            var distancia=col.Distance(parede.GetComponent<Collider2D>());
            if(distancia.distance<-.02f)throw new Exception("Penetracao excessiva no obstaculo: "+distancia.distance);
            ValidacaoPercursoApresentacao.CancelamentoSolicitado=true;
            direcao.SetValue(driver,Vector2.left); await Task.Delay(80); parado=rb.position; await Task.Delay(180);
            if(Vector2.Distance(parado,rb.position)>.02f)throw new Exception("Cancelamento deixou movimento ativo.");
            string erro=(string)tipo.GetProperty("Erro").GetValue(driver);
            if(!string.IsNullOrEmpty(erro))throw new Exception(erro);
            UnityEngine.Object.Destroy(driver); await Task.Delay(80);
            var input=(Vector2)typeof(PlayerMovement).GetField("direcaoInput",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(movimento);
            if(!movimento.enabled || input.sqrMagnitude>.001f || rb.linearVelocity.sqrMagnitude>.001f)
                throw new Exception("Encerramento nao devolveu controlador parado e habilitado.");
            ValidacaoPercursoApresentacao.CancelamentoSolicitado=false;
            var mapa=AnaliseCirculacaoTester.Criar(player,new Bounds(new Vector3(1001,1000.6f),new Vector3(6,6,0)));
            var tester=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("TesterPlayer")).Single(t=>t!=null);
            var separar=(Task)tester.GetMethod("SepararContatoRasoRotas",BindingFlags.Static|BindingFlags.NonPublic)
                .Invoke(null,new object[]{player,mapa,null});
            await separar;
            if(mapa.Ocupado(player.transform.position) || col.radius!=.3f || !movimento.enabled)
                throw new Exception("Saida de contato nao preservou corpo e controlador.");
            return "PASSOU: controlador real, parede, menu, cancelamento, liberacao e saida fisica de contato raso.";
        }
        catch(Exception e)
        {
            System.IO.File.WriteAllText(System.IO.Path.GetFullPath("output/apresentacao-jogavel/2026-11-12/circulacao/erro-fixture-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")+".txt"),e.ToString());
            throw;
        }
        finally
        {
            if(menuRegistrado)g.RegistrarMenuFechado();
            ValidacaoPercursoApresentacao.CancelamentoSolicitado=cancelamento;
            UnityEngine.Object.Destroy(player); UnityEngine.Object.Destroy(parede);
            if(anterior==null)UnityEngine.Object.Destroy(g.gameObject);
        }
    }
}
