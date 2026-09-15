using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Coloque este script em um GameObject com um Collider2D marcado como
/// "Is Trigger", posicionado sobre a entrada/portão que deve levar o
/// player para outra cena (ex: a entrada da Cidade no mapa principal, ou
/// a saída da Cidade de volta pro mapa principal).
///
/// Diferente do EncounterZone, aqui a troca de cena é sempre determinística
/// (sem sorteio): o player encosta no trigger e é levado direto para a
/// cena de destino, aparecendo na posição marcada por "pontoDeChegada".
///
/// IMPORTANTE (revisado 13/09/2026): o destino é um Vector2 digitado no
/// Inspector, NÃO um Transform arrastado. O Unity não permite salvar uma
/// referência de Transform que aponte para um objeto de OUTRA cena
/// ("cross-scene reference") — mesmo com as duas cenas abertas juntas no
/// Editor, essa referência nunca é salva de verdade e some ao fechar uma
/// das cenas. Por isso o campo aqui é só coordenadas (X, Y): você descobre
/// esse número posicionando um objeto marcador na cena de destino, olhando
/// o Position dele no Inspector, e digitando os mesmos números aqui.
///
/// Reaproveita o mesmo mecanismo de EncounterZone/RestaurarPosicao:
/// GerenciadorDeEstado.SalvarPosicao(posicao, cena) guarda ONDE o player
/// deve aparecer e EM QUAL CENA essa posição é válida. RestaurarPosicao,
/// anexado ao Player, já sabe ler isso sozinho ao carregar a cena nova —
/// este script não precisa (e não deve) mexer em RestaurarPosicao.cs.
///
/// TRAVA/BLOQUEIO (14/09/2026): quem precisar impedir a passagem até uma
/// condição ser cumprida (ex: puzzle da casa do Lip) NÃO deve mais
/// desabilitar este componente (transicaoDeCena.enabled = false). Um
/// componente desabilitado nunca recebe OnTriggerEnter2D, então não tem
/// como reagir ao toque do player pra mostrar um aviso -- e se esse
/// desabilitar acontecer no Awake() de outro script (ex: um checker que
/// mora dentro de um painel de UI escondido por padrão), esse Awake pode
/// nem ter rodado ainda quando o player já encosta na porta, já que Awake
/// só dispara quando o GameObject é ativado pela primeira vez.
/// Use o campo "trancada" em vez disso: o trigger continua sempre ativo,
/// então ele consegue mostrar "textoDeBloqueio"/"mensagemDeBloqueio"
/// quando o player esbarra travado, e destravar é só marcar
/// trancada = false em código (ex: PuzzleAguaCasaChecker.VerificarVitoria).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TransicaoDeCena : MonoBehaviour
{
    [Header("Destino")]
    [Tooltip("Nome exato da cena para carregar (tem que estar em File > Build Settings > Scenes In Build). Ex: \"City\" ou \"Game\".")]
    public string nomeCenaDestino;

    [Tooltip("Posição (X, Y) onde o player deve aparecer ao chegar na cena de destino. Para descobrir esses números: abra a cena de destino, veja a posição de um objeto marcador colocado no lugar certo, e digite os mesmos valores aqui.")]
    public Vector2 pontoDeChegada;

    [Header("Bloqueio (opcional)")]
    [Tooltip("Enquanto marcado, o trigger NÃO leva pra outra cena -- só mostra a mensagem de bloqueio (se atribuída) e ignora o toque. Deixe desmarcado em portas que nunca travam (ex: Cidade<->mundo). Controlado em código por quem tranca a porta (ex: PuzzleAguaCasaChecker).")]
    public bool trancada;

    [Tooltip("Texto (TMP) usado pra avisar o player que a porta está trancada. Fica ativado/desativado por este script -- pode deixar o próprio GameObject dele desativado na cena, não precisa mexer nisso manualmente. Deixe vazio se não quiser nenhum aviso.")]
    public TMP_Text textoDeBloqueio;

    [Tooltip("Mensagem mostrada em textoDeBloqueio quando o player esbarra na porta travada.")]
    public string mensagemDeBloqueio = "Preciso terminar o terminal antes de sair.";

    // Evita disparar a transição várias vezes enquanto o player ainda
    // está sobrepondo o trigger no frame da troca de cena.
    private bool transicaoDisparada;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (transicaoDisparada) return;
        if (!other.CompareTag("Player")) return;

        if (trancada)
        {
            MostrarMensagemDeBloqueio();
            return;
        }

        if (string.IsNullOrEmpty(nomeCenaDestino))
        {
            Debug.LogWarning($"TransicaoDeCena em '{gameObject.name}': nomeCenaDestino não foi configurado no Inspector.");
            return;
        }

        transicaoDisparada = true;
        IniciarTransicao();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Some com o aviso quando o player se afasta da porta, senão o
        // texto fica preso na tela depois que ele já saiu de perto.
        if (!other.CompareTag("Player")) return;
        EsconderMensagemDeBloqueio();
    }

    void IniciarTransicao()
    {
        // Mesma lógica de EncounterZone.IniciarBatalha, mas guardando a
        // posição de CHEGADA (pontoDeChegada) na cena de DESTINO — não a
        // posição atual do player na cena de origem. RestaurarPosicao só
        // teleporta se CenaDeOrigemDaPosicao bater com o nome da cena que
        // acabou de carregar, então o segundo argumento tem que ser
        // nomeCenaDestino, não a cena atual.
        GerenciadorDeEstado.Instancia.SalvarPosicao(pontoDeChegada, nomeCenaDestino);

        SceneManager.LoadScene(nomeCenaDestino);
    }

    private void MostrarMensagemDeBloqueio()
    {
        if (textoDeBloqueio == null) return;
        textoDeBloqueio.text = mensagemDeBloqueio;
        textoDeBloqueio.gameObject.SetActive(true);
    }

    private void EsconderMensagemDeBloqueio()
    {
        if (textoDeBloqueio == null) return;
        textoDeBloqueio.gameObject.SetActive(false);
    }
}
