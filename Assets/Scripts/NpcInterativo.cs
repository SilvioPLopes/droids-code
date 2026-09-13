using UnityEngine;

/// <summary>
/// Componente genérico para qualquer NPC com o qual o player possa
/// interagir (falar, comprar, etc). Nesta primeira fatia, só resolve
/// a DETECÇÃO — o player chega perto, um indicador visual liga, e ao
/// apertar a tecla de interação o NPC reage (por enquanto só um Debug.Log
/// de placeholder). O que exatamente acontece na interação — abrir um
/// diálogo (Fatia 2) ou abrir uma loja (Fatia 3) — é decidido depois,
/// dentro do método Interagir() abaixo, sem precisar mexer na detecção.
///
/// Colocar este script no GameObject do NPC, junto com um Collider2D
/// marcado como "Is Trigger" (raio de detecção — não precisa ser do
/// tamanho do sprite do NPC, pode ser um pouco maior, pra dar espaço
/// de aproximação).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NpcInterativo : MonoBehaviour
{
    [Header("Identificação")]
    [Tooltip("Nome do NPC, usado futuramente no painel de diálogo/loja (Fatia 2/3). Não afeta a detecção.")]
    public string nomeDoNpc = "NPC";

    [Header("Indicador visual (opcional)")]
    [Tooltip("GameObject filho (ex: um balão \"!\" ou ícone de interação) que liga quando o player está perto e desliga quando se afasta. Pode deixar vazio por enquanto.")]
    public GameObject indicadorDeInteracao;

    [Header("Interação")]
    [Tooltip("Tecla que o player aperta para interagir, quando está dentro do raio.")]
    public KeyCode teclaDeInteracao = KeyCode.E;

    private bool playerPerto;
    private GameObject playerAtual;

    void Awake()
    {
        if (indicadorDeInteracao != null)
        {
            indicadorDeInteracao.SetActive(false);
        }
    }

    void Update()
    {
        // Mesmo cuidado do PlayerMovement: não interagir com um
        // menu/terminal/diálogo já aberto por cima.
        if (GerenciadorDeEstado.Instancia.MenuAberto) return;

        if (playerPerto && Input.GetKeyDown(teclaDeInteracao))
        {
            Interagir();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerPerto = true;
        playerAtual = other.gameObject;

        if (indicadorDeInteracao != null)
        {
            indicadorDeInteracao.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerPerto = false;
        playerAtual = null;

        if (indicadorDeInteracao != null)
        {
            indicadorDeInteracao.SetActive(false);
        }
    }

    /// <summary>
    /// Placeholder da Fatia 1. Nas próximas fatias, este método passa a
    /// abrir o painel de Diálogo (Fatia 2) ou de Loja (Fatia 3), em vez
    /// de só logar — a detecção acima (proximidade + tecla) não muda.
    /// </summary>
    void Interagir()
    {
        Debug.Log($"Interagiu com {nomeDoNpc} (placeholder — Fatia 2/3 vai abrir Diálogo/Loja aqui).");
    }
}
