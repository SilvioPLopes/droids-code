using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Batalha ROTEIRIZADA (nao aleatoria). Irmao do EncounterZone, sem o
/// sorteio: o player encosta no trigger e a batalha acontece, contra um
/// inimigo especifico do CatalogoDeInimigos.
///
/// E o que entrega as duas lutas obrigatorias do Ato 1: as Sucatas de
/// Captura da Sessao 3 e o Droid Improvisado do Ivo da Sessao 6.
///
/// Reaproveita o mecanismo que ja existe e ja funciona
/// (GerenciadorDeEstado.SalvarPosicao + RestaurarPosicao no Player), igual
/// EncounterZone e TransicaoDeCena fazem — nada de logica de teleporte nova.
///
/// A flag de vitoria NAO e lida pelo Inspector da cena Battle: ela viaja em
/// GerenciadorDeEstado.FlagDeVitoriaPendente, porque quem sabe o que esta
/// luta significa pra historia e este gatilho aqui, na cena do mundo — nao
/// a cena de batalha, que e generica e compartilhada.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GatilhoDeBatalha : MonoBehaviour
{
    [Header("Inimigo")]
    [Tooltip("Id no CatalogoDeInimigos (ex: \"sucata_captura\", \"droid_do_ivo\"). Id invalido cai no inimigo configurado no Inspector da cena Battle.")]
    public string idDoInimigo = "sucata_captura";

    [Header("Cena")]
    [Tooltip("Nome da cena de batalha. Igual ao EncounterZone.")]
    public string nomeCenaBatalha = "Battle";

    [Header("Condicoes de historia")]
    [Tooltip("So dispara se TODAS estiverem ativas.")]
    public string[] flagsNecessarias;

    [Tooltip("NAO dispara se QUALQUER uma estiver ativa. A flag de vitoria entra aqui automaticamente (ver flagDeVitoria) — nao precisa repetir.")]
    public string[] flagsQueImpedem;

    [Header("Consequencia")]
    [Tooltip("Flag gravada ao VENCER esta batalha (ex: \"ato1_sessao3\"). Tambem serve de trava: se ja estiver ativa, o gatilho nao dispara de novo.")]
    public string flagDeVitoria;

    [Header("Interacao")]
    public string tagDoPlayer = "Player";

    [Tooltip("Se marcado, o player precisa apertar a tecla abaixo dentro do trigger. Se desmarcado, a batalha comeca ao encostar.")]
    public bool exigirTecla = false;
    public KeyCode teclaDeInteracao = KeyCode.E;

    [Tooltip("Filho (ex: balao \"!\") ligado quando o player esta dentro do trigger e a batalha esta disponivel.")]
    public GameObject indicadorDeInteracao;

    private bool _playerPerto;
    private bool _disparada;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Awake()
    {
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
    }

    void Update()
    {
        if (!exigirTecla) return;
        if (GerenciadorDeEstado.Instancia.MenuAberto)
        {
            if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
            return;
        }

        bool disponivel = _playerPerto && PodeDisparar();
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(disponivel);

        if (disponivel && Input.GetKeyDown(teclaDeInteracao))
        {
            Disparar();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoPlayer)) return;

        _playerPerto = true;

        if (!exigirTecla && PodeDisparar() && !GerenciadorDeEstado.Instancia.MenuAberto)
        {
            Disparar();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoPlayer)) return;

        _playerPerto = false;
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
    }

    bool PodeDisparar()
    {
        if (_disparada) return false;

        // A propria flag de vitoria funciona como "ja fiz essa luta" — sem
        // isso, o player voltaria da batalha, encostaria no mesmo trigger e
        // lutaria de novo em loop.
        if (!string.IsNullOrWhiteSpace(flagDeVitoria) && GerenciadorDeEstado.Instancia.ObterFlag(flagDeVitoria))
        {
            return false;
        }

        return CondicoesDeHistoria.Satisfeitas(flagsNecessarias, flagsQueImpedem);
    }

    public void Disparar()
    {
        if (_disparada) return;
        _disparada = true;

        GameObject player = GameObject.FindGameObjectWithTag(tagDoPlayer);
        var gerenciador = GerenciadorDeEstado.Instancia;

        // Mesma logica de EncounterZone.IniciarBatalha: guarda ONDE voltar.
        string cenaAtual = SceneManager.GetActiveScene().name;
        Vector3 posicao = player != null ? player.transform.position : transform.position;
        gerenciador.SalvarPosicao(posicao, cenaAtual);

        gerenciador.ProximoInimigoId = idDoInimigo;
        gerenciador.FlagDeVitoriaPendente = flagDeVitoria;

        if (string.IsNullOrEmpty(nomeCenaBatalha))
        {
            Debug.LogWarning($"GatilhoDeBatalha '{gameObject.name}': nomeCenaBatalha vazio no Inspector.");
            _disparada = false;
            return;
        }

        SceneManager.LoadScene(nomeCenaBatalha);
    }
}
