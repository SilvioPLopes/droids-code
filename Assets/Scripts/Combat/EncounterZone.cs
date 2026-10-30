using DroidsCode.Combat;
using UnityEngine;

/// <summary>
/// Coloque este script em um GameObject com um Collider2D marcado como
/// "Is Trigger", cobrindo a área de areia (ou qualquer zona de encontro).
/// A cada certa distância percorrida pelo player dentro da zona, sorteia
/// uma chance de iniciar uma batalha.
///
/// ATO 1 (15/09/2026): ganhou duas coisas, ambas opcionais e sem efeito se
/// deixadas vazias (nenhuma zona existente muda de comportamento):
///   - idDoInimigo: qual inimigo do CatalogoDeInimigos esta zona gera. É o
///     que faz a Cratera ter "Droid Selvagem" e a borda da cidade ter
///     "Sucata de Captura", sem precisar de uma cena Battle por zona.
///   - flagsNecessarias/flagsQueImpedem: a zona só sorteia encontro se a
///     história permitir. É o que impede o jogador de farmar a Cratera
///     antes de ter escolhido facção — sem precisar de parede física.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncounterZone : MonoBehaviour
{
    [Header("Configuração do encontro")]
    [Tooltip("Chance por passo quando a tabela de encontros está vazia. Ex: 0.15 = 15%.")]
    [Range(0f, 1f)]
    public float chanceDeEncontro = 0.15f;

    [Tooltip("Distância (em unidades) que o player precisa andar dentro da zona para contar como um 'passo' e sortear a chance.")]
    public float distanciaPorPasso = 1f;

    [Tooltip("Nome da cena de batalha para carregar. Deixe vazio se ainda não tiver essa cena.")]
    public string nomeCenaBatalha = "Battle";

    [Header("Inimigo (Ato 1 — opcional)")]
    [Tooltip("Id no CatalogoDeInimigos gerado por esta zona (ex: \"droid_selvagem\"). Vazio = a cena Battle usa o que estiver no Inspector dela.")]
    public string idDoInimigo = "";

    [System.Serializable]
    public class EntradaEncontro
    {
        public string idDoInimigo;
        [Tooltip("Chance absoluta por tentativa. A soma da tabela não pode passar de 1; o restante não gera batalha.")]
        [Range(0f, 1f)] public float chance;
    }

    [Header("Variação de inimigos (opcional)")]
    [Tooltip("Se preenchida, substitui a chance e o ID únicos acima. Ex: A=0.15, B=0.05, sem encontro=0.80.")]
    public EntradaEncontro[] encontros;

    [Header("Condições de história (Ato 1 — opcional)")]
    [Tooltip("A zona só sorteia encontro se TODAS estiverem ativas.")]
    public string[] flagsNecessarias;

    [Tooltip("A zona para de sortear encontro se QUALQUER uma estiver ativa.")]
    public string[] flagsQueImpedem;

    private Vector3 ultimaPosicao;
    private float distanciaAcumulada;
    private bool rastreandoPosicao;
    private bool batalhaDisparada;

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
        // NOVO (Ato 1): zona desligada pela história não sorteia nada.
        if (!CondicoesDeHistoria.Satisfeitas(flagsNecessarias, flagsQueImpedem)) return;

        // Não puxar o jogador pra uma batalha com um painel aberto na frente
        // dele (Menu do Mundo, terminal, diálogo). Sem isso, um diálogo longo
        // enquanto o player desliza pelo trigger podia virar batalha no meio
        // da fala.
        if (GerenciadorDeEstado.Instancia.MenuAberto) return;

        float sorteio = Random.value; // valor entre 0.0 e 1.0

        if (TentarSelecionarInimigo(sorteio, out string idSelecionado))
        {
            IniciarBatalha(player, idSelecionado);
        }
    }

    /// <summary>Seleciona sem consumir RNG nem alterar estado. Amostra entre 0 e 1.</summary>
    public bool TentarSelecionarInimigo(float amostra, out string idSelecionado)
    {
        idSelecionado = null;
        if (float.IsNaN(amostra) || amostra < 0f || amostra > 1f) return false;

        if (encontros == null || encontros.Length == 0)
        {
            if (float.IsNaN(chanceDeEncontro) || chanceDeEncontro <= 0f || chanceDeEncontro > 1f || amostra > chanceDeEncontro) return false;
            idSelecionado = idDoInimigo;
            return true;
        }

        float total = 0f;
        foreach (var entrada in encontros)
        {
            if (entrada == null || float.IsNaN(entrada.chance) || entrada.chance < 0f || entrada.chance > 1f) return false;
            total += entrada.chance;
        }
        // Configuração inválida não é normalizada: evita modificar as chances silenciosamente.
        if (total > 1f) return false;

        float limite = 0f;
        foreach (var entrada in encontros)
        {
            limite += entrada.chance;
            if (entrada.chance <= 0f || !(amostra < limite || (amostra == 1f && limite == 1f))) continue;
            var definicao = CatalogoDeInimigos.Obter(entrada.idDoInimigo);
            // Uma entrada indisponível mantém sua faixa sem encontro; não aumenta as demais.
            if (definicao == null || definicao.EhChefe || definicao.EhUnico) return false;
            idSelecionado = definicao.Id;
            return true;
        }
        return false;
    }

    void IniciarBatalha(GameObject player, string idSelecionado)
    {
        if (batalhaDisparada || player == null) return;
        if (string.IsNullOrWhiteSpace(nomeCenaBatalha) || !Application.CanStreamedLevelBeLoaded(nomeCenaBatalha))
        {
            Debug.LogWarning("EncounterZone: cena de batalha indisponível.");
            return;
        }
        string cenaAtual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!GerenciadorDeEstado.Instancia.TentarPrepararBatalha(cenaAtual, player.transform.position,
            idSelecionado, null, new SalvamentoJson(), out string erro))
        {
            Debug.LogWarning(erro);
            return;
        }
        batalhaDisparada = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nomeCenaBatalha);
    }
}
