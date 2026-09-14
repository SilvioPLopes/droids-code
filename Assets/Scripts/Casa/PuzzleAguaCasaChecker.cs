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
    /// "nivel_agua" chegou no valor-alvo; se sim, destrava a porta e troca a
    /// fala do Apollo.
    ///
    /// Substitui TerminalCasaManager por completo. Aquele script duplicava
    /// toda a UI que o TerminalUIManager do painel já resolvia -- era
    /// exatamente o motivo dos dois brigando pelo mesmo painel. Passo a
    /// passo pra corrigir a cena:
    ///   1) No PainelTerminal (o duplicado), remova o componente
    ///      "Terminal Casa Manager (Script)" -- Add Component não precisa
    ///      dele, o TerminalUIManager que já está lá continua igual.
    ///   2) Adicione este script (PuzzleAguaCasaChecker) no MESMO
    ///      GameObject que tem o TerminalUIManager.
    ///   3) Preencha nomeDaVariavel/valorDeVitoria/transicaoDaPortaDeSaida/
    ///      textoDeApollo no Inspector (mesmos valores que estavam no
    ///      TerminalCasaManager antigo).
    ///   4) Em InteragirComTerminalCasa (o script do trigger de entrada),
    ///      arraste o TerminalUIManager do painel no campo "terminal"
    ///      (o tipo mudou de TerminalCasaManager pra TerminalUIManager).
    ///
    /// Dica: o TerminalUIManager também tem campos "Texto de exemplo",
    /// "Texto pontos" e "Texto ajuda" que fazem sentido pro terminal geral
    /// (droid.subirAtributo etc.) mas não pro puzzle da casa. Pra essa
    /// instância específica do painel: troque "Código de exemplo" pra algo
    /// como "nivel_agua = 0" e deixe "Texto pontos"/"Texto ajuda" vazios no
    /// Inspector -- ambos já são tratados como opcionais no código
    /// (checagem de null), não precisa mexer em nada do TerminalUIManager
    /// pra isso.
    /// </summary>
    [RequireComponent(typeof(TerminalUIManager))]
    public class PuzzleAguaCasaChecker : MonoBehaviour
    {
        [Header("Regras do puzzle")]
        [Tooltip("Nome da variável Lua que o jogador precisa controlar.")]
        [SerializeField] private string nomeDaVariavel = "nivel_agua";
        [Tooltip("Valor mínimo da variável para considerar o puzzle resolvido.")]
        [SerializeField] private double valorDeVitoria = 100;

        [Header("Vitória")]
        [Tooltip("Componente TransicaoDeCena da porta de saída. Fica desativado até o puzzle ser resolvido (desativar o componente também desliga o OnTriggerEnter2D dele).")]
        [SerializeField] private TransicaoDeCena transicaoDaPortaDeSaida;
        [SerializeField] private TMP_Text textoDeApollo;
        [SerializeField] private string mensagemDeVitoria =
            "Apollo: Viu? Não foi tão difícil quanto você fingiu que ia ser.";

        private TerminalUIManager _terminal;
        private bool _puzzleResolvido;

        void Awake()
        {
            _terminal = GetComponent<TerminalUIManager>();

            // Porta trancada até resolver o puzzle -- garantido em código
            // pra não depender de lembrar de desmarcar "enabled" no
            // Inspector toda vez que a cena for mexida.
            if (transicaoDaPortaDeSaida != null) transicaoDaPortaDeSaida.enabled = false;
        }

        // Assina/desassina no (Dis)Enable, não no Awake/OnDestroy -- padrão
        // seguro de evento em MonoBehaviour (evita assinar duas vezes se o
        // objeto for desativado/reativado, e evita vazar assinatura se o
        // objeto for destruído sem passar por OnDisable).
        void OnEnable()
        {
            if (_terminal != null) _terminal.AoExecutarCodigo += VerificarVitoria;

            // DEBUG TEMPORÁRIO (remover depois de confirmar que funciona):
            Debug.Log($"[PuzzleAguaCasaChecker] OnEnable -- terminal encontrado: {_terminal != null}");
        }

        void OnDisable()
        {
            if (_terminal != null) _terminal.AoExecutarCodigo -= VerificarVitoria;
        }

        private void VerificarVitoria()
        {
            // DEBUG TEMPORÁRIO (remover depois de confirmar que funciona):
            double? debugValor = _terminal != null && _terminal.Runner != null
                ? _terminal.Runner.ObterVariavelNumerica(nomeDaVariavel)
                : null;
            Debug.Log($"[PuzzleAguaCasaChecker] VerificarVitoria -- valor lido de '{nomeDaVariavel}': {debugValor} (alvo: {valorDeVitoria}) -- porta atribuída: {transicaoDaPortaDeSaida != null} -- texto Apollo atribuído: {textoDeApollo != null}");

            if (_puzzleResolvido || _terminal == null || _terminal.Runner == null) return;

            double? valorAtual = _terminal.Runner.ObterVariavelNumerica(nomeDaVariavel);
            if (valorAtual == null || valorAtual.Value < valorDeVitoria) return;

            _puzzleResolvido = true;
            if (textoDeApollo != null) textoDeApollo.text = mensagemDeVitoria;
            if (transicaoDaPortaDeSaida != null) transicaoDaPortaDeSaida.enabled = true;
        }
    }
}
