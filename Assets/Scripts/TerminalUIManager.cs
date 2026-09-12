using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DroidsCode.Scripting;
using DroidsCode.DroidCore;

/// <summary>
/// UI do terminal de código — Modo Configuração (fora de batalha).
/// Conecta o campo de texto onde o jogador digita Lua ao DroidScriptRunner.
/// Não roda Lua em combate (ver CORRECAO_ARQUITETURA_TERMINAL_MENU.md).
/// </summary>
public class TerminalUIManager : MonoBehaviour
{
    [Header("Painel raiz")]
    public GameObject painelTerminal;

    [Header("Referências de UI (arraste da Hierarchy)")]
    public TMP_InputField campoDeCodigo;
    public Button botaoExecutar;
    public Button botaoFechar;
    public Button botaoLimpar;
    public TextMeshProUGUI textoSaida;
    public ScrollRect scrollRectSaida;
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoAjuda;

    [Header("Texto de exemplo (pré-preenche o campo ao abrir pela 1ª vez)")]
    [TextArea(3, 6)]
    public string codigoDeExemplo =
        "-- Escreva seu código aqui e clique Executar (ou Ctrl+Enter)\n" +
        "droid.subirAtributo(\"For\", 1)\n";

    [Header("Configuração")]
    [Tooltip("Quantas linhas de log manter visíveis antes de descartar as mais antigas.")]
    public int maximoDeLinhasDeLog = 40;

    private DroidScriptRunner runner;
    private TerminalDroidApi api;
    private Droid droid;

    private readonly List<string> historicoDeComandos = new List<string>();
    private int indiceHistorico = -1;

    private readonly List<string> linhasDeLog = new List<string>();

    private static readonly string TextoAjudaPadrao =
        "COMANDOS DISPONÍVEIS:\n" +
        "  droid.subirAtributo(\"For\"|\"Agi\"|\"Vit\"|\"Int\"|\"Dex\"|\"Luk\", quantidade)\n" +
        "  droid.aprenderTecnica(\"nome\", nivelDeDano)\n" +
        "  droid.aprenderTecnicaComVeneno(\"nome\", nivelDeDano, danoPorTurno, duracaoEmTurnos)\n" +
        "  droid.aprenderTecnicaComStun(\"nome\", nivelDeDano, duracaoEmTurnos)\n" +
        "  droid.esquecerTecnica(\"nome\")\n" +
        "  droid.obterAtributo(\"nome\")        -- retorna número\n" +
        "  droid.listarTecnicas()              -- retorna texto\n" +
        "  droid.obterPontosDisponiveis()       -- retorna número";

    void Start()
    {
        if (painelTerminal != null) painelTerminal.SetActive(false);

        if (botaoExecutar != null) botaoExecutar.onClick.AddListener(AoClicarExecutar);
        if (botaoFechar != null) botaoFechar.onClick.AddListener(Fechar);
        if (botaoLimpar != null) botaoLimpar.onClick.AddListener(AoClicarLimpar);

        if (textoAjuda != null) textoAjuda.text = TextoAjudaPadrao;
    }

    void Update()
    {
        if (painelTerminal == null || !painelTerminal.activeSelf) return;
        if (campoDeCodigo == null || !campoDeCodigo.isFocused) return;

        bool ctrlSegurado = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (ctrlSegurado && Input.GetKeyDown(KeyCode.Return))
        {
            AoClicarExecutar();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && string.IsNullOrEmpty(campoDeCodigo.text))
        {
            NavegarHistorico(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && indiceHistorico >= 0)
        {
            NavegarHistorico(1);
        }
    }

    public void Abrir()
    {
        droid = GerenciadorDeEstado.Instancia.DroidDoJogador;

        api = new TerminalDroidApi(droid);
        runner = new DroidScriptRunner(api);

        if (campoDeCodigo != null && string.IsNullOrEmpty(campoDeCodigo.text) && historicoDeComandos.Count == 0)
        {
            campoDeCodigo.text = codigoDeExemplo;
        }

        AtualizarPontos();
        AdicionarLinhaDeLog("=== Terminal aberto. Digite um comando e clique Executar. ===");
        GerenciadorDeEstado.Instancia.RegistrarMenuAberto();

        if (painelTerminal != null) painelTerminal.SetActive(true);

        if (campoDeCodigo != null)
        {
            campoDeCodigo.Select();
            campoDeCodigo.ActivateInputField();
        }
    }

    public void Fechar()
    {
        if (painelTerminal != null && painelTerminal.activeSelf)
        {
            painelTerminal.SetActive(false);
            GerenciadorDeEstado.Instancia.RegistrarMenuFechado();
        }
    }

    void AoClicarExecutar()
    {
        if (runner == null || campoDeCodigo == null) return;

        string codigo = campoDeCodigo.text;
        if (string.IsNullOrWhiteSpace(codigo)) return;

        AdicionarLinhaDeLog($"> {codigo.Replace("\n", " / ")}");

        api.LimparLog();
        ResultadoExecucao resultado = runner.ExecutarCodigo(codigo);

        if (resultado.Sucesso)
        {
            foreach (string linha in api.ObterLogDaSessao())
            {
                AdicionarLinhaDeLog(linha);
            }

            if (api.ObterLogDaSessao().Count == 0)
            {
                AdicionarLinhaDeLog("(código executado, nenhuma ação de droid chamada)");
            }
        }
        else
        {
            // Mesmo em falha, o Lua pode ter executado alguns comandos com
            // sucesso ANTES do erro. Mostra o log acumulado antes do "ERRO".
            foreach (string linha in api.ObterLogDaSessao())
            {
                AdicionarLinhaDeLog(linha);
            }

            AdicionarLinhaDeLog($"ERRO: {resultado.MensagemErro}");
        }

        AdicionarAoHistorico(codigo);
        AtualizarPontos();
    }

    void AoClicarLimpar()
    {
        if (campoDeCodigo != null) campoDeCodigo.text = "";
    }

    void AdicionarAoHistorico(string codigo)
    {
        historicoDeComandos.Add(codigo);
        indiceHistorico = historicoDeComandos.Count;
        if (campoDeCodigo != null) campoDeCodigo.text = "";
    }

    void NavegarHistorico(int direcao)
    {
        if (historicoDeComandos.Count == 0) return;

        indiceHistorico = Mathf.Clamp(indiceHistorico + direcao, 0, historicoDeComandos.Count - 1);
        campoDeCodigo.text = historicoDeComandos[indiceHistorico];
        campoDeCodigo.caretPosition = campoDeCodigo.text.Length;
    }

    void AtualizarPontos()
    {
        if (textoPontos == null || droid == null) return;
        textoPontos.text = $"Pontos disponíveis: {droid.Pontos.PontosDisponiveis}";
    }

    void AdicionarLinhaDeLog(string linha)
    {
        Debug.Log($"[Terminal] {linha}"); // TEMPORARIO — remover depois que a UI estiver legivel

        linhasDeLog.Add(linha);

        while (linhasDeLog.Count > maximoDeLinhasDeLog)
        {
            linhasDeLog.RemoveAt(0);
        }

        if (textoSaida != null)
        {
            var sb = new StringBuilder();
            foreach (string l in linhasDeLog)
            {
                sb.AppendLine(l);
            }
            textoSaida.text = sb.ToString();
        }

        if (scrollRectSaida != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRectSaida.verticalNormalizedPosition = 0f;
        }
    }
}