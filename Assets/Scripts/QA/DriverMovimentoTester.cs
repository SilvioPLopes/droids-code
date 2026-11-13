#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

// Somente Editor/Play: fornece intencao ao FixedUpdate REAL do PlayerMovement.
// Nao move Transform/Rigidbody, nao desabilita controlador nem muda fisica.
[DefaultExecutionOrder(-10000)]
public sealed class DriverMovimentoTester : MonoBehaviour
{
    [Serializable] public class PassoFisico
    {
        public string cena;
        public double tempo;
        public float x,y,entradaX,entradaY,vx,vy,penetracao;
        public bool menu;
        public string[] contatos;
    }
    public Vector2 DirecaoSolicitada {get;set;}
    public string Erro {get;private set;}="";
    public Func<bool> DeveCancelar;
    public Func<Vector2,Vector2> EscolherDirecao;
    public Action<PassoFisico> AoPasso;
    PlayerMovement movimento;
    Rigidbody2D corpo;
    Collider2D col;
    GerenciadorDeEstado estado;
    FieldInfo entrada,ultima;
    MethodInfo animacao;
    readonly ContactPoint2D[] contatos=new ContactPoint2D[16];

    void Awake()
    {
        movimento=GetComponent<PlayerMovement>(); corpo=GetComponent<Rigidbody2D>(); col=GetComponent<Collider2D>();
        estado=UnityEngine.Object.FindAnyObjectByType<GerenciadorDeEstado>();
        const BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
        entrada=typeof(PlayerMovement).GetField("direcaoInput",flags);
        ultima=typeof(PlayerMovement).GetField("ultimaDirecao",flags);
        animacao=typeof(PlayerMovement).GetMethod("AtualizarAnimacao",flags);
        if(!Application.isPlaying || movimento==null || corpo==null || col==null || estado==null ||
            entrada?.FieldType!=typeof(Vector2) || ultima?.FieldType!=typeof(Vector2) || animacao==null)
        { Erro="Contrato do controlador real indisponivel; driver nao pode operar."; enabled=false; }
    }
    Vector2 Injetar()
    {
        if(movimento==null || corpo==null)return Vector2.zero;
        if(!movimento.enabled)Erro="PlayerMovement foi desabilitado durante o teste.";
        bool parar=!string.IsNullOrEmpty(Erro) || estado==null || estado.MenuAberto ||
            (DeveCancelar?.Invoke()==true);
        var direcao=parar?Vector2.zero:new Vector2(Mathf.Abs(DirecaoSolicitada.x)<.001f?0:Mathf.Sign(DirecaoSolicitada.x),
            Mathf.Abs(DirecaoSolicitada.y)<.001f?0:Mathf.Sign(DirecaoSolicitada.y)).normalized;
        entrada?.SetValue(movimento,direcao);
        if(direcao.sqrMagnitude>.01f)ultima?.SetValue(movimento,direcao);
        return direcao;
    }
    void FixedUpdate()
    {
        if(EscolherDirecao!=null && corpo!=null)
        {
            try{DirecaoSolicitada=EscolherDirecao(corpo.position);}
            catch(Exception e){Erro="Falha ao escolher direcao: "+e.Message;Parar();}
        }
        var direcao=Injetar();
        if(AoPasso==null || corpo==null || col==null)return;
        try
        {
            int quantidade=col.GetContacts(contatos);
            float penetracao=0;
            var nomes=new string[quantidade];
            for(int i=0;i<quantidade;i++)
            {
                var outro=contatos[i].collider==col?contatos[i].otherCollider:contatos[i].collider;
                nomes[i]=outro==null?"":Caminho(outro.transform);
                if(outro!=null && !outro.isTrigger)penetracao=Mathf.Min(penetracao,col.Distance(outro).distance);
            }
            AoPasso(new PassoFisico {cena=SceneManager.GetActiveScene().name,tempo=Time.fixedTimeAsDouble,
                x=corpo.position.x,y=corpo.position.y,entradaX=direcao.x,entradaY=direcao.y,
                vx=corpo.linearVelocity.x,vy=corpo.linearVelocity.y,menu=estado!=null&&estado.MenuAberto,
                contatos=nomes.Where(n=>!string.IsNullOrEmpty(n)).Distinct().ToArray(),penetracao=penetracao});
        }
        catch(Exception e){Erro="Falha ao registrar passo: "+e.Message;Parar();}
    }
    void LateUpdate()
    {
        Injetar();
        if(movimento!=null && animacao!=null)animacao.Invoke(movimento,null);
    }
    public void Parar()
    {
        DirecaoSolicitada=Vector2.zero;
        if(movimento!=null)entrada?.SetValue(movimento,Vector2.zero);
        if(corpo!=null)corpo.linearVelocity=Vector2.zero;
    }
    void OnDisable()=>Parar();
    void OnDestroy()=>Parar();
    public static string Caminho(Transform alvo)
    {
        string nome=alvo.name;
        while(alvo.parent!=null){alvo=alvo.parent;nome=alvo.name+"/"+nome;}
        return nome;
    }
}
#endif
