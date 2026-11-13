// QA Editor: captura e fisica real, sem gravar geometria nem save.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class TesterPlayer
{
    const string ChaveVisualRotas="DroidsCode.TesterPlayer.Rotas.FuncionalVisual";
    static bool FuncionalVisualRotas=>SessionState.GetBool(ChaveVisualRotas,false);
    static readonly HashSet<string> capturasVisualRotas=new HashSet<string>();
    static bool provaMovelVisualRotas;
    [MenuItem("Droids Code/Testes/TesterPlayer/Validar funcional com imagens e colisores ativos")]
    public static void PrepararAuditoriaVisual()
    {
        SessionState.SetBool(ChaveVisualRotas,true);
        capturasVisualRotas.Clear();provaMovelVisualRotas=false;
        PrepararRotas(false);
    }
    [Serializable] sealed class ObjetoVisualRotas
    {
        public string caminho,tipo;
        public bool enabled,trigger;
        public Vector2 centroPixel;
        public float raioPixel;
        public Vector2[] contornoPixel;
    }
    [Serializable] sealed class FrameVisualRotas
    {
        public string cena,instante,imagem,execucao;
        public int largura,altura;
        public float cameraOrtho;
        public Vector3 cameraPosicao,playerRoot;
        public ObjetoVisualRotas[] colisores;
    }
    static async Task CapturarVisualRotas(string nome,bool forcar=false)
    {
        string cena=SceneManager.GetActiveScene().name;
        var salas=new[]{"CasaLip","Interior_E03_Terreo","Interior_E03_Superior","Interior_E09_Terreo","Interior_E08_Superior","Interior_E05_Principal"};
        if(!forcar && !salas.Contains(cena) || !capturasVisualRotas.Add(nome))return;
        var camera=Camera.main;Exigir(camera!=null,"Captura sem camera.");
        string dir=Path.Combine(HistoricoRotas,"visual");Directory.CreateDirectory(dir);
        string imagem=Path.Combine(dir,nome+".png");
        float pixelPorWorld=Screen.height/(2*camera.orthographicSize);
        var objetos=ComponentesRotas<Collider2D>().Where(c=>c.enabled && c.gameObject.activeInHierarchy).Select(c=>
        {
            var o=new ObjetoVisualRotas {caminho=CaminhoRotas(c.transform),tipo=c.GetType().Name,enabled=c.enabled,trigger=c.isTrigger,
                centroPixel=camera.WorldToScreenPoint(c.bounds.center)};
            if(c is CircleCollider2D)o.raioPixel=Mathf.Max(c.bounds.extents.x,c.bounds.extents.y)*pixelPorWorld;
            else if(c is BoxCollider2D b)
            {
                var h=b.size*.5f;
                o.contornoPixel=new[]{new Vector2(-h.x,-h.y),new Vector2(h.x,-h.y),new Vector2(h.x,h.y),new Vector2(-h.x,h.y)}
                    .Select(v=>(Vector2)camera.WorldToScreenPoint(b.transform.TransformPoint(v+b.offset))).ToArray();
            }
            else if(c is PolygonCollider2D p)o.contornoPixel=p.GetPath(0).Select(v=>(Vector2)camera.WorldToScreenPoint(p.transform.TransformPoint(v+p.offset))).ToArray();
            else if(c is EdgeCollider2D e)o.contornoPixel=e.points.Select(v=>(Vector2)camera.WorldToScreenPoint(e.transform.TransformPoint(v+e.offset))).ToArray();
            return o;
        }).ToArray();
        var frame=new FrameVisualRotas {cena=cena,instante=DateTime.UtcNow.ToString("O"),imagem=imagem,execucao=relatorioRotas.idExecucao,
            largura=Screen.width,altura=Screen.height,cameraOrtho=camera.orthographicSize,cameraPosicao=camera.transform.position,
            playerRoot=PlayerRotas().transform.position,colisores=objetos};
        File.WriteAllText(Path.Combine(dir,nome+".json"),JsonUtility.ToJson(frame,true));
        ScreenCapture.CaptureScreenshot(imagem);
        await Task.Delay(150);CancelamentoRotas();
        Exigir(SceneManager.GetActiveScene().name==cena,"Cena mudou durante captura visual.");
        Exigir(File.Exists(imagem),"Imagem nao foi gravada; metadados nao substituem captura.");
    }
    static async Task ProvarMovelVisualRotas(AnaliseCirculacaoTester.Mapa mapa)
    {
        var box=ComponentesRotas<BoxCollider2D>().Single(c=>CaminhoRotas(c.transform)=="E03_Terreo/Colisoes/Obstaculo_3");
        Exigir(box.enabled && !box.isTrigger,"Movel deve permanecer solido e ativo.");
        var raiz=new Vector2(box.bounds.min.x-mapa.raio-.25f,box.bounds.center.y-mapa.offset.y);
        var trecho=NovoTrechoRotas("PROVA_COLISAO_MOVEL_ATIVO",SceneManager.GetActiveScene().name,raiz);
        try
        {
            await IrAteRotas(raiz,null,trecho,null,null,mapa);
            var player=PlayerRotas();var corpo=CorpoRotas();
            LiberarDriverRotas();driverRotas=player.AddComponent<DriverMovimentoTester>();
            driverRotas.DirecaoSolicitada=Vector2.right;
            await Task.Delay(450);CancelamentoRotas();driverRotas.Parar();
            await Task.Delay(120);CancelamentoRotas();
            float distancia=corpo.Distance(box).distance;
            Exigir(corpo.IsTouching(box),"Movel nao reteve o player por contato fisico.");
            Exigir(distancia>=-.01f && corpo.bounds.center.x<=box.bounds.min.x-mapa.raio+.02f,"Player atravessou o movel ativo.");
            trecho.precisao="450ms de intencao contra movel; distancia real="+distancia+"; colisor ativo reteve player";
            await CapturarVisualRotas("colisao-real-movel",true);
            LiberarDriverRotas();
            await SepararContatoRasoRotas(player,mapa,trecho);
            trecho.resultado=AprovadoRotas;
        }
        finally {LiberarDriverRotas();FinalizarTrechoRotas(trecho);}
    }
}
