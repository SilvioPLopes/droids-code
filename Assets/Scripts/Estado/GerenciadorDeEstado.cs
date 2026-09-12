using System.Collections.Generic;
using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Guarda o estado que precisa sobreviver a troca de cena (Game <-> Battle):
/// o Droid do jogador (mesma instancia, nunca recriada), a posicao onde ele
/// estava no mapa antes de entrar em batalha, e agora flags de historia
/// abertas para o sistema de salvamento (ver SalvamentoJson.cs).
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

    private int _contadorMenusAbertos;
    public bool MenuAberto => _contadorMenusAbertos > 0;

    public void RegistrarMenuAberto() => _contadorMenusAbertos++;
    public void RegistrarMenuFechado() => _contadorMenusAbertos = Mathf.Max(0, _contadorMenusAbertos - 1);

    // Flags de historia: chave livre (ex: "derrotou_chefe_1"), aberto pra
    // uso futuro. Nao ha nenhuma flag definida ainda -- so a estrutura.
    private readonly Dictionary<string, bool> _flagsDeHistoria = new Dictionary<string, bool>();

    public bool ObterFlag(string chave) =>
        _flagsDeHistoria.TryGetValue(chave, out bool valor) && valor;

    public void DefinirFlag(string chave, bool valor) => _flagsDeHistoria[chave] = valor;

    public IReadOnlyDictionary<string, bool> TodasAsFlags => _flagsDeHistoria;

    public void CarregarFlags(Dictionary<string, bool> flags)
    {
        _flagsDeHistoria.Clear();
        foreach (var kv in flags)
        {
            _flagsDeHistoria[kv.Key] = kv.Value;
        }
    }

    void Awake()
    {
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
            // houver tela de criacao de personagem real.
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
