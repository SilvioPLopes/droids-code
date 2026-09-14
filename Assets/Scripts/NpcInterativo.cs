using UnityEngine;

/// <summary>
/// Componente genérico para qualquer NPC com o qual o player possa
/// interagir (falar, comprar, etc). Detecção de proximidade + tecla de
/// interação (Fatia 1). Interagir() agora sabe abrir a Loja (Fatia Loja,
/// 13/09/2026) — a variante de Diálogo simples ainda não foi feita
/// (pulada de propósito, ver conversa) e pode ser adicionada depois sem
/// mexer na detecção abaixo.
///
/// Colocar este script no GameObject do NPC, junto com um Collider2D
/// marcado como "Is Trigger" (raio de detecção — não precisa ser do
/// tamanho do sprite do NPC, pode ser um pouco maior, pra dar espaço
/// de aproximação).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NpcInterativo : MonoBehaviour
{
    public enum TipoDeNpc
    {
        Generico, // placeholder (Debug.Log) — comportamento original da Fatia 1
        Loja
    }

    [Header("Identificação")]
    [Tooltip("Nome do NPC. Usado futuramente no painel de diálogo. Não afeta a detecção.")]
    public string nomeDoNpc = "NPC";

    [Header("Comportamento")]
    [Tooltip("O que acontece ao interagir. 'Loja' abre o painel configurado em Loja UI Manager abaixo.")]
    public TipoDeNpc tipo = TipoDeNpc.Generico;

    [Tooltip("Obrigatório se Tipo = Loja. Arraste aqui o GameObject que tem o componente LojaUIManager (geralmente um Canvas da cena).")]
    public LojaUIManager lojaUIManager;

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
        // menu/terminal/diálogo/loja já aberto por cima.
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

    void Interagir()
    {
        switch (tipo)
        {
            case TipoDeNpc.Loja:
                AbrirLoja();
                break;

            default:
                Debug.Log($"Interagiu com {nomeDoNpc} (placeholder — Diálogo ainda não implementado).");
                break;
        }
    }

    void AbrirLoja()
    {
        if (lojaUIManager == null)
        {
            Debug.LogWarning($"NpcInterativo '{gameObject.name}': Tipo = Loja mas lojaUIManager não foi configurado no Inspector.");
            return;
        }

        // Mesmo padrão de contador de menus que o resto do jogo já usa
        // (TelaDeStatusManager/TerminalUIManager) — RegistrarMenuAberto
        // impede o PlayerMovement de mover o player e este próprio script
        // de reagir à tecla de interação enquanto a Loja está na tela.
        GerenciadorDeEstado.Instancia.RegistrarMenuAberto();
        lojaUIManager.AoFechar = () => GerenciadorDeEstado.Instancia.RegistrarMenuFechado();
        lojaUIManager.Mostrar();
    }
}
