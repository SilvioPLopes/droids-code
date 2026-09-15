using UnityEngine;
using TMPro;

namespace DroidsCode.Casa
{
    /// <summary>
    /// Checagem de vitória do puzzle de água da cena da casa (Sessão 1 do
    /// Enredo: "int nivel_agua = 0; while (nivel_agua &lt; 100) { ... }").
    ///
    /// NÃO é um terminal -- reaproveita 100% o TerminalUIManager que já está
    /// no painel duplicado (campo, botões, log, histórico de comandos, tudo
    /// dele). Este script só escuta "um código acabou de ser executado"
    /// (TerminalUIManager.AoExecutarCodigo) e olha se a variável Lua
    /// "nivel_agua" chegou no valor-alvo; se sim, DESTRANCA a porta e troca
    /// a fala do Apollo.
    ///
    /// MUDANÇA IMPORTANTE (14/09/2026) -- como a porta é trancada:
    /// a versão anterior fazia transicaoDaPortaDeSaida.enabled = false no
    /// Awake(). Dois problemas com isso:
    ///   1) Componente desabilitado não recebe OnTriggerEnter2D. Com isso a
    ///      porta ficava "muda": não dava pra avisar o player que ele
    ///      precisa resolver o terminal primeiro, porque o trigger nem
    ///      disparava.
    ///   2) Este script mora no painel do terminal, que normalmente nasce
    ///      DESATIVADO (só aparece no Abrir()). Awake() de objeto inativo
    ///      não roda -- ou seja, a porta ficava destrancada até o jogador
    ///      abrir o terminal pela primeira vez.
    /// Agora quem manda é o campo público "trancada" do TransicaoDeCena:
    /// ele nasce marcado no Inspector da PRÓPRIA porta (que está ativa
    /// desde o começo da cena), o trigger continua vivo pra mostrar o
    /// aviso, e este script só desmarca a flag quando o puzzle é resolvido.
    ///
    /// Montagem na cena:
    ///   1) Este script fica no MESMO GameObject que tem o
    ///      TerminalUIManager (o PainelTerminal da casa).
    ///   2) Na porta (PortaDeSaidaCasa), no componente TransicaoDeCena:
    ///      marcar "Trancada" e preencher "Texto De Bloqueio".
    ///   3) Preencher aqui: nomeDaVariavel / valorDeVitoria /
    ///      transicaoDaPortaDeSaida / textoDeApollo.
    /// </summary>
    [RequireComponent(typeof(TerminalUIManager))]
    public class PuzzleAguaCasaChecker : MonoBehaviour
    {
        [Header("Regras do puzzle")]
        [Tooltip("Nome da variável Lua que o jogador precisa controlar. TEM que ser exatamente o nome que o jogador digita no terminal -- 'agua' e 'nivel_agua' são variáveis diferentes pro Lua.")]
        [SerializeField] private string nomeDaVariavel = "nivel_agua";
        [Tooltip("Valor mínimo da variável para considerar o puzzle resolvido.")]
        [SerializeField] private double valorDeVitoria = 100;

        [Header("Vitória")]
        [Tooltip("Componente TransicaoDeCena da porta de saída. Fica com 'Trancada' marcado até o puzzle ser resolvido -- este script só desmarca a flag, nunca desabilita o componente (componente desabilitado não recebe trigger e não consegue avisar o player).")]
        [SerializeField] private TransicaoDeCena transicaoDaPortaDeSaida;
        [Tooltip("Texto (TMP) da fala do Apollo dentro do painel do terminal. Pode estar desativado na cena -- este script ativa sozinho na hora da vitória.")]
        [SerializeField] private TMP_Text textoDeApollo;
        [SerializeField] private string mensagemDeVitoria =
            "Apollo: Viu? Não foi tão difícil quanto você fingiu que ia ser.";

        private TerminalUIManager _terminal;
        private bool _puzzleResolvido;

        void Awake()
        {
            _terminal = GetComponent<TerminalUIManager>();
        }

        // Assina/desassina no (Dis)Enable, não no Awake/OnDestroy -- padrão
        // seguro de evento em MonoBehaviour (evita assinar duas vezes se o
        // objeto for desativado/reativado, e evita vazar assinatura se o
        // objeto for destruído sem passar por OnDisable).
        void OnEnable()
        {
            if (_terminal != null) _terminal.AoExecutarCodigo += VerificarVitoria;
        }

        void OnDisable()
        {
            if (_terminal != null) _terminal.AoExecutarCodigo -= VerificarVitoria;
        }

        private void VerificarVitoria()
        {
            if (_puzzleResolvido || _terminal == null || _terminal.Runner == null) return;

            double? valorAtual = _terminal.Runner.ObterVariavelNumerica(nomeDaVariavel);

            if (valorAtual == null)
            {
                // Caso clássico: o jogador digitou outro nome de variável
                // (ex: "agua = 100" quando o alvo é "nivel_agua"). Sem este
                // aviso o puzzle parece "quebrado" sendo que só não existe
                // variável com esse nome na sessão Lua.
                Debug.Log($"[PuzzleAguaCasaChecker] Variável '{nomeDaVariavel}' ainda não existe na sessão Lua.");
                return;
            }

            if (valorAtual.Value < valorDeVitoria) return;

            _puzzleResolvido = true;

            if (textoDeApollo != null)
            {
                textoDeApollo.text = mensagemDeVitoria;
                textoDeApollo.gameObject.SetActive(true);
            }

            if (transicaoDaPortaDeSaida != null) transicaoDaPortaDeSaida.trancada = false;
        }
    }
}
