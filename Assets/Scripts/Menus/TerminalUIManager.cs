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
///
/// CORRECAO (13/09/2026 — exclusividade de painéis): ganhou o callback
/// opcional AoFechar, invocado dentro do "if" de Fechar() (ou seja, só
/// quando o painel de fato estava aberto e foi fechado agora). O
/// MenuMundoManager usa isso pra saber quando reexibir o painel do Menu do
/// Mundo depois que o Terminal é fechado.
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

    // NOVO (15/09/2026 — Apollo Dica): quando marcado, todo erro de execução
    // ganha, além da mensagem crua do MoonSharp, uma explicação conceitual na
    // voz do Apollo (ver ApolloDica.cs). Deixe LIGADO — é o pilar pedagógico
    // do projeto. O campo existe pra poder desligar em teste de balanceamento
    // ou gravação de vídeo, não pro uso normal.
    [Tooltip("Mostra a explicação do Apollo junto de cada erro do terminal. Manter ligado.")]
    public bool mostrarDicasDoApollo = true;

    // NOVO (13/09/2026): callback opcional, atribuído externamente (ver
    // MenuMundoManager.Start()). Invocado dentro de Fechar().
    public System.Action AoFechar;

    // NOVO (14/09/2026 — puzzle da casa/Sessão 1): exposto pra permitir que
    // um script de puzzle específico de uma cena (ex: PuzzleAguaCasaChecker)
    // leia o estado do Lua depois de cada execução, sem duplicar toda essa
    // UI de novo (foi isso que o TerminalCasaManager fazia errado). Só
    // leitura -- ninguém de fora deveria trocar o runner do terminal.
    public DroidScriptRunner Runner => runner;

    // Invocado ao final de AoClicarExecutar(), sempre (sucesso ou erro).
    // O terminal geral não sabe nem precisa saber o que os ouvintes fazem
    // com isso -- ex.: checar se uma variável Lua chegou num valor-alvo.
    public System.Action AoExecutarCodigo;

    private DroidScriptRunner runner;
    private TerminalDroidApi api;
    private Droid droid;

    private readonly List<string> historicoDeComandos = new List<string>();
    // indiceHistorico == historicoDeComandos.Count representa a posicao
    // "livre" (campo vazio, depois do ultimo comando) -- igual um terminal
    // de verdade (bash). Comeca aqui, nao em -1.
    private int indiceHistorico = 0;
    // O que o campo deveria conter, segundo a navegacao do historico. Se o
    // jogador editar o texto manualmente (sem usar seta), isso diverge do
    // campoDeCodigo.text real -- e usamos essa divergencia pra bloquear a
    // proxima navegacao em vez de apagar a edicao dele (mesma logica
    // protetora que o ↑ ja tinha, so que generalizada pros dois lados).
    private string textoEsperadoPeloHistorico = "";

    private readonly List<string> linhasDeLog = new List<string>();

    private static readonly string TextoAjudaPadrao =
        "COMANDOS DISPONÍVEIS:\n" +
        "  droid.subirAtributo(\"For\"|\"Agi\"|\"Vit\"|\"Int\"|\"Dex\"|\"Luk\", quantidade)\n" +
        "  droid.aprenderTecnica(\"nome\", nivelDeDano)\n" +
        "  droid.aprenderTecnicaComVeneno(\"nome\", nivelDeDano, danoPorTurno, duracaoEmTurnos)\n" +
        "  droid.aprenderTecnicaComStun(\"nome\", nivelDeDano, duracaoEmTurnos)\n" +
        "  droid.melhorarTecnica(\"nome\", nivelDeDanoAdicional) -- upgrade, cobra só a diferença\n" +
        "  droid.esquecerTecnica(\"nome\")\n" +
        "  droid.obterAtributo(\"nome\")        -- retorna número\n" +
        "  droid.listarTecnicas()              -- retorna texto\n" +
        "  droid.obterPontosDisponiveis()       -- retorna número\n" +
        "  droid.testeAdicionarPontos(quantidade) -- [TESTE] ignora XP/nível";

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

        // CORRECAO (12/09/2026): ↑ e ↓ agora usam a MESMA regra protetora --
        // so navegam se o texto atual do campo bate com o que o historico
        // "acha" que colocou la (ou seja, o jogador nao editou manualmente
        // desde a ultima navegacao). Antes o ↑ so checava campo vazio (o que
        // por acaso protegia a digitacao inicial, mas nao uma edicao em cima
        // de um item de historico ja carregado) e o ↓ nao checava nada.
        bool textoNaoFoiEditadoManualmente = campoDeCodigo.text == textoEsperadoPeloHistorico;

        if (Input.GetKeyDown(KeyCode.UpArrow) && textoNaoFoiEditadoManualmente)
        {
            NavegarHistorico(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && textoNaoFoiEditadoManualmente)
        {
            NavegarHistorico(1);
        }
        else if (!textoNaoFoiEditadoManualmente)
        {
            // O jogador editou o texto manualmente (fora de uma navegacao
            // de seta) -- essa edicao vira o novo baseline. Isso e o que
            // permite digitar livremente depois de uma edicao, em vez de
            // ficar bloqueado contra o texto antigo do historico pra
            // sempre; a protecao so precisa valer pro PROXIMO toque de
            // seta, nao pra digitacao normal.
            textoEsperadoPeloHistorico = campoDeCodigo.text;
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

        // Sincroniza o baseline com o que de fato esta no campo ao abrir
        // (vazio ou o exemplo pre-preenchido), senao a primeira seta
        // pressionada seria bloqueada por uma falsa "edicao manual".
        indiceHistorico = historicoDeComandos.Count;
        textoEsperadoPeloHistorico = campoDeCodigo != null ? campoDeCodigo.text : "";

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

            // NOVO (13/09/2026): avisa quem estiver ouvindo (ex: MenuMundoManager)
            // que o Terminal foi fechado.
            AoFechar?.Invoke();
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

            // NOVO (15/09/2026 — Apollo Dica): alem da mensagem crua do
            // MoonSharp, o Apollo traduz o erro numa explicacao conceitual.
            // Este e o primeiro degrau do "Apollo Debugger" prometido no
            // readme -- NAO destaca linha no editor nem e reativo enquanto o
            // jogador digita; so explica o erro que ja aconteceu. Ver
            // ApolloDica.cs pro escopo exato e como adicionar novas dicas.
            if (mostrarDicasDoApollo)
            {
                string linhaDoErro = ApolloDica.ExtrairLinha(resultado.MensagemErro);
                string dica = ApolloDica.Traduzir(resultado.MensagemErro);
                AdicionarLinhaDeLog(string.IsNullOrEmpty(linhaDoErro) ? dica : $"{dica} {linhaDoErro}");
            }
        }

        AdicionarAoHistorico(codigo);
        AtualizarPontos();

        AoExecutarCodigo?.Invoke();
    }

    void AoClicarLimpar()
    {
        if (campoDeCodigo != null) campoDeCodigo.text = "";
    }

    void AdicionarAoHistorico(string codigo)
    {
        historicoDeComandos.Add(codigo);
        indiceHistorico = historicoDeComandos.Count; // volta pra posicao "livre"
        textoEsperadoPeloHistorico = "";
        if (campoDeCodigo != null) campoDeCodigo.text = "";
    }

    // CORRECAO (12/09/2026): duas mudancas em relacao a versao antiga:
    // 1) o range de Clamp agora vai ate historicoDeComandos.Count (nao
    //    Count - 1) -- essa posicao extra representa o campo vazio "depois
    //    do ultimo comando", entao dando ↓ repetidas vezes o jogador chega
    //    de volta nele, em vez de travar no ultimo item pra sempre.
    // 2) depois de mover o indice, guardamos o texto resultante em
    //    textoEsperadoPeloHistorico -- e essa variavel que o Update() usa
    //    pra saber se o jogador editou manualmente antes da proxima seta.
    void NavegarHistorico(int direcao)
    {
        if (historicoDeComandos.Count == 0) return;

        indiceHistorico = Mathf.Clamp(indiceHistorico + direcao, 0, historicoDeComandos.Count);

        string novoTexto = indiceHistorico < historicoDeComandos.Count
            ? historicoDeComandos[indiceHistorico]
            : ""; // posicao "livre": campo vazio, igual um terminal de verdade

        campoDeCodigo.text = novoTexto;
        campoDeCodigo.caretPosition = campoDeCodigo.text.Length;
        textoEsperadoPeloHistorico = novoTexto;
    }

    void AtualizarPontos()
    {
        if (textoPontos == null || droid == null) return;
        textoPontos.text = $"Pontos disponíveis: {droid.Pontos.PontosDisponiveis}";
    }

    // NOVO: ponto de entrada publico pra scripts de FORA do terminal (ex:
    // VerificadorDePuzzle, que usa isso como fallback quando nao ha TMP de
    // fala do Apollo configurado no Inspector) escreverem uma linha no log
    // sem duplicar a logica de buffer/scroll. So repassa pro metodo interno
    // -- faltava exatamente este metodo, causava o CS1061.
    public void EscreverNoLog(string linha) => AdicionarLinhaDeLog(linha);

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

        // CORRECAO (12/09/2026): scrollRectSaida.verticalNormalizedPosition = 0f
        // direto aqui ia pro fim de um tamanho de conteudo DESATUALIZADO --
        // o Unity so recalcula a altura real do texto (Content) um frame
        // depois da mudanca, entao a régua do scroll mirava no fim antigo, nao
        // no fim de verdade. Sintoma: precisar arrastar manualmente pra achar
        // a mensagem nova. Corrigido rodando a rolagem numa coroutine que
        // espera o fim do frame antes de mover o scroll (ver
        // RolarParaFimNoProximoFrame abaixo).
        if (scrollRectSaida != null)
        {
            StartCoroutine(RolarParaFimNoProximoFrame());
        }
    }

    private System.Collections.IEnumerator RolarParaFimNoProximoFrame()
    {
        // Espera o layout (tamanho real do Content) ser recalculado antes de
        // mover o scroll -- e' esse atraso de um frame que causava o "vai pro
        // fim errado" quando a rolagem era feita na mesma chamada que mudava
        // o texto.
        Canvas.ForceUpdateCanvases();
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        scrollRectSaida.verticalNormalizedPosition = 0f;
    }
}
