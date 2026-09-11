using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Guarda o estado que precisa sobreviver a troca de cena (Game <-> Battle):
/// o Droid do jogador (mesma instancia, nunca recriada) e a posicao onde ele
/// estava no mapa antes de entrar em batalha.
///
/// NAO e sistema de save/load — isso continua fora de escopo (ver
/// LEIA_PRIMEIRO.md). E so memoria de sessao: fecha o jogo, perde tudo.
///
/// Padrao Singleton COM criacao preguicosa (lazy): se voce der Play direto
/// na cena Battle (sem passar por MainMenu/Game antes), a primeira chamada
/// a Instancia cria um Droid novo com valores padrao — nao quebra o teste
/// isolado de uma cena.
/// </summary>
public class GerenciadorDeEstado : MonoBehaviour
{
    private static GerenciadorDeEstado instancia;

    public static GerenciadorDeEstado Instancia
    {
        get
        {
            if (instancia == null)
            {
                var objeto = new GameObject("GerenciadorDeEstado");
                instancia = objeto.AddComponent<GerenciadorDeEstado>();
                DontDestroyOnLoad(objeto);
            }
            return instancia;
        }
    }

    public Droid DroidDoJogador { get; private set; }

    public bool TemPosicaoSalva { get; private set; }
    public Vector3 PosicaoSalva { get; private set; }
    public string CenaDeOrigemDaPosicao { get; private set; }

    // Contador, nao bool simples: Terminal abre DE DENTRO do Menu, entao
    // fechar o Terminal nao pode reativar o movimento se o Menu ainda
    // estiver aberto por tras dele. MenuAberto so fica false quando todos
    // os paineis abertos foram fechados.
    private int _contadorMenusAbertos;
    public bool MenuAberto => _contadorMenusAbertos > 0;

    public void RegistrarMenuAberto() => _contadorMenusAbertos++;
    public void RegistrarMenuFechado() => _contadorMenusAbertos = Mathf.Max(0, _contadorMenusAbertos - 1);

    void Awake()
    {
        // Protege contra 2 instancias (ex: se por engano existir uma na cena
        // alem da criada via Instancia).
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        if (DroidDoJogador == null)
        {
            // [DEFAULT] valores iniciais de um Droid novo — ajustar quando
            // houver tela de criacao de personagem real. Mesmos valores que
            // ja estavam no BattleManager antes desta mudanca.
            DroidDoJogador = new Droid("Heroi", hpMax: 30);
            DroidDoJogador.StatsBase.For = 5;
            DroidDoJogador.StatsBase.Vit = 3;
        }
    }

    public void SalvarPosicao(Vector3 posicao, string nomeDaCena)
    {
        PosicaoSalva = posicao;
        CenaDeOrigemDaPosicao = nomeDaCena;
        TemPosicaoSalva = true;
    }

    public void LimparPosicaoSalva()
    {
        TemPosicaoSalva = false;
    }
}
