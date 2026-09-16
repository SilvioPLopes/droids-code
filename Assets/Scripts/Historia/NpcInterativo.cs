using UnityEngine;

/// <summary>
/// Componente genérico para qualquer NPC com o qual o player possa
/// interagir (falar, comprar, etc). Detecção de proximidade + tecla de
/// interação (Fatia 1). Interagir() sabe abrir a Loja (Fatia Loja,
/// 13/09/2026) e, desde 15/09/2026 (Ato 1), também abrir Diálogo.
///
/// DIÁLOGO (15/09/2026): o tipo 'Dialogo' NÃO duplica nada — ele delega
/// para um GatilhoDeDialogo no MESMO GameObject, que é quem carrega as
/// falas, as condições de flag e as escolhas. Este script continua sendo
/// só detecção de proximidade + despacho por tipo. Isso fecha o item
/// "Diálogo simples de NPC genérico (pulado por decisão do responsável)"
/// do CHECKLIST sem reabrir a discussão de design que foi adiada: o
/// conteúdo é todo Inspector, não código.
///
/// O tipo 'Generico' foi mantido (ainda Debug.Log) de propósito, pra não
/// quebrar nenhum NPC já configurado na cena City.
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
        Loja,
        Dialogo   // NOVO (15/09/2026): delega pro GatilhoDeDialogo deste mesmo GameObject
    }

    [Header("Identificação")]
    [Tooltip("Nome do NPC. Usado futuramente no painel de diálogo. Não afeta a detecção.")]
    public string nomeDoNpc = "NPC";

    [Header("Comportamento")]
    [Tooltip("O que acontece ao interagir. 'Loja' abre o painel configurado em Loja UI Manager abaixo.")]
    public TipoDeNpc tipo = TipoDeNpc.Generico;

    [Tooltip("Obrigatório se Tipo = Loja. Arraste aqui o GameObject que tem o componente LojaUIManager (geralmente um Canvas da cena).")]
    public LojaUIManager lojaUIManager;

    [Tooltip("Opcional. Se Tipo = Dialogo e este campo ficar vazio, o script procura um GatilhoDeDialogo neste mesmo GameObject automaticamente.")]
    public GatilhoDeDialogo gatilhoDeDialogo;

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

        // NOVO (15/09/2026): resolve o gatilho de diálogo do próprio objeto,
        // pra você não precisar arrastar um componente pra dentro dele mesmo.
        if (tipo == TipoDeNpc.Dialogo && gatilhoDeDialogo == null)
        {
            gatilhoDeDialogo = GetComponent<GatilhoDeDialogo>();
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

            case TipoDeNpc.Dialogo:
                AbrirDialogo();
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

        // NOVO (15/09/2026 — Ato 1): se este NPC lojista também tem uma fala
        // disponível (ex: o Senhor Tácio contando da fila do poço na Sessão 2),
        // ela vem ANTES da vitrine. O painel de compra só abre quando o
        // diálogo terminar. Sem isso, a Loja seria um sistema solto colado
        // num personagem mudo.
        if (gatilhoDeDialogo == null) gatilhoDeDialogo = GetComponent<GatilhoDeDialogo>();

        if (gatilhoDeDialogo != null && gatilhoDeDialogo.PodeDisparar())
        {
            gatilhoDeDialogo.AoConcluirDialogoExterno = MostrarPainelDaLoja;
            gatilhoDeDialogo.Disparar();
            return;
        }

        MostrarPainelDaLoja();
    }

    void MostrarPainelDaLoja()
    {
        if (lojaUIManager == null) return;

        // Mesmo padrão de contador de menus que o resto do jogo já usa
        // (TelaDeStatusManager/TerminalUIManager) — RegistrarMenuAberto
        // impede o PlayerMovement de mover o player e este próprio script
        // de reagir à tecla de interação enquanto a Loja está na tela.
        GerenciadorDeEstado.Instancia.RegistrarMenuAberto();
        lojaUIManager.AoFechar = () => GerenciadorDeEstado.Instancia.RegistrarMenuFechado();
        lojaUIManager.Mostrar();
    }

    // NOVO (15/09/2026 — Ato 1). Diferente de AbrirLoja, aqui NÃO chamamos
    // RegistrarMenuAberto: o DialogoUIManager já cuida do próprio registro
    // (mesmo padrão do TerminalUIManager, que também se registra sozinho).
    // Registrar duas vezes deixaria o contador preso em 1 e o player
    // congelado pra sempre depois do diálogo — exatamente o bug de
    // "personagem para de andar" que já foi caçado uma vez neste projeto.
    void AbrirDialogo()
    {
        if (gatilhoDeDialogo == null)
        {
            Debug.LogWarning($"NpcInterativo '{gameObject.name}': Tipo = Dialogo mas não há GatilhoDeDialogo neste GameObject.");
            return;
        }

        if (!gatilhoDeDialogo.PodeDisparar())
        {
            // Sem falas disponíveis para o estado atual da história (ex: as
            // condições de flag não batem, ou o diálogo já foi visto).
            // Silêncio é o comportamento certo: o indicador de interação do
            // próprio GatilhoDeDialogo já não deveria estar aceso.
            return;
        }

        gatilhoDeDialogo.Disparar();
    }
}
