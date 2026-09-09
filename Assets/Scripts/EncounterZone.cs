using UnityEngine;

/// <summary>
/// Coloque este script em um GameObject com um Collider2D marcado como
/// "Is Trigger", cobrindo a área de areia (ou qualquer zona de encontro).
/// A cada certa distância percorrida pelo player dentro da zona, sorteia
/// uma chance de iniciar uma batalha.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncounterZone : MonoBehaviour
{
    [Header("Configuração do encontro")]
    [Tooltip("Chance de batalha a cada 'passo' (0 a 1). Ex: 0.15 = 15%.")]
    [Range(0f, 1f)]
    public float chanceDeEncontro = 0.15f;

    [Tooltip("Distância (em unidades) que o player precisa andar dentro da zona para contar como um 'passo' e sortear a chance.")]
    public float distanciaPorPasso = 1f;

    [Tooltip("Nome da cena de batalha para carregar. Deixe vazio se ainda não tiver essa cena.")]
    public string nomeCenaBatalha = "Battle";

    private Vector3 ultimaPosicao;
    private float distanciaAcumulada;
    private bool rastreandoPosicao;

    // OnTriggerStay2D dispara a cada frame físico enquanto o Player estiver
    // dentro da zona — diferente de OnTriggerEnter2D, funciona mesmo se o
    // Player já começar a cena dentro da área (sem precisar "entrar" nela).
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 posicaoAtual = other.transform.position;

        // Primeira vez que detectamos o player nesta zona: só inicializa a referência.
        if (!rastreandoPosicao)
        {
            ultimaPosicao = posicaoAtual;
            distanciaAcumulada = 0f;
            rastreandoPosicao = true;
            return;
        }

        // Acumula a distância percorrida desde a última checagem
        float distanciaMovida = Vector3.Distance(posicaoAtual, ultimaPosicao);
        distanciaAcumulada += distanciaMovida;
        ultimaPosicao = posicaoAtual;

        // A cada "passo" completo, sorteia a chance de batalha
        if (distanciaAcumulada >= distanciaPorPasso)
        {
            distanciaAcumulada = 0f;
            SortearEncontro(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        rastreandoPosicao = false;
    }

    void SortearEncontro(GameObject player)
    {
        float sorteio = Random.value; // valor entre 0.0 e 1.0

        if (sorteio <= chanceDeEncontro)
        {
            IniciarBatalha(player);
        }
    }

    void IniciarBatalha(GameObject player)
    {
        Debug.Log("Encontro aleatório disparado! Iniciando batalha...");

        // Salva onde o player estava antes de entrar na batalha, pra ele
        // voltar pro lugar certo depois (ver GerenciadorDeEstado.cs e
        // RestaurarPosicao.cs).
        string cenaAtual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GerenciadorDeEstado.Instancia.SalvarPosicao(player.transform.position, cenaAtual);

        if (!string.IsNullOrEmpty(nomeCenaBatalha))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nomeCenaBatalha);
        }
    }
}
