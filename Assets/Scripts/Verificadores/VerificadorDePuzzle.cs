using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Avalia um puzzle de logica lendo o valor final de UMA OU MAIS variaveis
/// Lua depois de cada execucao do terminal.
///
/// v2 (15/09/2026) -- o que mudou em relacao a v1 desta entrega:
///   - Le a definicao do CatalogoDePuzzles pelo Id, em vez de ter o enunciado
///     e a condicao digitados no Inspector. Isso deixa os 10 puzzles prontos
///     utilizaveis sem redigitar nada.
///   - Suporta VARIAS verificacoes ao mesmo tempo (ex: "energia >= 50 E
///     porta_aberta == 1"), que e o que torna possivel exigir condicional.
///   - Conta tentativas e mostra a Dica do Apollo depois de N erros --
///     o jogador nao fica preso sem saber o que fazer.
///   - Registra progresso de quest (TipoDeObjetivo.ResolverPuzzle).
///   - Mantem o modo manual: se voce nao preencher idDoPuzzle, os campos do
///     Inspector continuam valendo, exatamente como na v1.
///
/// POR QUE NAO MORA NO GAMEOBJECT DO TERMINAL (mudanca estrutural em relacao
/// ao PuzzleAguaCasaChecker original): com o MenuMundoManager persistindo
/// entre cenas, o TerminalUIManager persiste junto. Um checker preso a ele
/// por [RequireComponent] viajaria pra todas as cenas seguintes continuando a
/// escutar execucoes que nao tem nada a ver com o puzzle dele. Aqui o checker
/// mora num objeto da PROPRIA cena e assina/desassina em OnEnable/OnDisable.
///
/// LIMITE CONHECIDO (design do terminal, nao bug daqui): TerminalUIManager.
/// Abrir() cria um DroidScriptRunner novo a cada abertura, entao as variaveis
/// Lua zeram quando o jogador fecha e reabre o terminal. O puzzle precisa ser
/// resolvido dentro de uma mesma sessao. E coerente com terminal de verdade.
/// </summary>
public class VerificadorDePuzzle : MonoBehaviour
{
    [Header("Puzzle do catálogo (recomendado)")]
    [Tooltip("Id no CatalogoDePuzzles (ex: \"p_painel_energia\"). Preenchendo isto, o enunciado, as condições, a dica e a fala de vitória vêm de lá. Deixe vazio pra usar os campos manuais abaixo.")]
    public string idDoPuzzle = "";

    [Header("Terminal")]
    [Tooltip("Opcional. Vazio = procura um TerminalUIManager na cena (inclusive o persistente do Canvas do Menu do Mundo).")]
    public TerminalUIManager terminal;

    [Header("Modo manual (só usado se Id Do Puzzle ficar vazio)")]
    [Tooltip("Nome EXATO da variável global Lua. 'agua' e 'nivel_agua' são variáveis diferentes pro Lua.")]
    public string nomeDaVariavel = "nivel_agua";
    public TipoDeComparacao comparacao = TipoDeComparacao.MaiorOuIgual;
    public double valorDeVitoria = 100;

    [Header("Efeitos ao resolver")]
    [Tooltip("Porta que destranca. Marca trancada = false — nunca desabilita o componente (ver TransicaoDeCena).")]
    public TransicaoDeCena portaParaDestrancar;

    [Tooltip("Texto (TMP) da fala do Apollo. Pode estar desativado na cena — este script ativa sozinho.")]
    public TMP_Text textoDeApollo;

    [TextArea(2, 5)]
    [Tooltip("Usado só no modo manual. Com Id Do Puzzle preenchido, a fala vem do catálogo.")]
    public string falaAoResolverManual = "Viu? Não foi tão difícil quanto você fingiu que ia ser.";

    public GameObject[] objetosParaAtivar;
    public GameObject[] objetosParaDesativar;

    [Header("Ajuda progressiva")]
    [Tooltip("Depois de quantas execuções sem sucesso o Apollo solta a dica do catálogo. 0 desliga.")]
    public int tentativasAteDica = 3;

    [Header("História")]
    [Tooltip("Flag gravada ao resolver (ex: \"ato1_puzzle_energia\"). Persiste no save.")]
    public string flagAoResolver;

    [Tooltip("Se já estiver ativa ao carregar a cena, o puzzle nasce resolvido (porta destrancada). Normalmente é a MESMA de Flag Ao Resolver.")]
    public string flagQueJaResolve;

    [Header("Depuração")]
    [Tooltip("Escreve no Console cada leitura de variável. Útil enquanto você monta; desligue depois.")]
    public bool logDeDepuracao = false;

    private bool _resolvido;
    private bool _inscrito;
    private int _tentativas;
    private DefinicaoDePuzzle _definicao;
    private readonly ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    void Awake()
    {
        _definicao = CatalogoDePuzzles.Obter(idDoPuzzle);

        if (!string.IsNullOrWhiteSpace(idDoPuzzle) && _definicao == null)
        {
            Debug.LogWarning($"VerificadorDePuzzle em '{gameObject.name}': puzzle '{idDoPuzzle}' não existe no CatalogoDePuzzles. Caindo no modo manual.");
        }
    }

    void Start()
    {
        if (!string.IsNullOrWhiteSpace(flagQueJaResolve) &&
            GerenciadorDeEstado.Instancia.ObterFlag(flagQueJaResolve))
        {
            AplicarEfeitosDeVitoria(persistir: false, mostrarFala: false);
        }
    }

    void OnEnable() { Inscrever(); }

    void OnDisable()
    {
        // Obrigatorio: o terminal pode ser persistente (DontDestroyOnLoad).
        // Sem desinscrever, o Action dele guardaria referencia pra um
        // MonoBehaviour ja destruido e estouraria na proxima execucao de
        // codigo, numa cena totalmente diferente.
        if (_inscrito && terminal != null)
        {
            terminal.AoExecutarCodigo -= VerificarVitoria;
            _inscrito = false;
        }
    }

    void Inscrever()
    {
        if (_inscrito) return;
        // CORRECAO (Unity 6): FindObjectOfType estava obsoleto (CS0618).
        // FindAnyObjectByType e o substituto direto quando so importa achar
        // UM (nao precisa de ordenacao).
        if (terminal == null) terminal = FindAnyObjectByType<TerminalUIManager>();

        if (terminal == null)
        {
            Debug.LogWarning($"VerificadorDePuzzle em '{gameObject.name}': nenhum TerminalUIManager encontrado. O puzzle nunca será avaliado.");
            return;
        }

        terminal.AoExecutarCodigo += VerificarVitoria;
        _inscrito = true;
    }

    /// <summary>
    /// Enunciado completo pro Apollo apresentar (usado por GatilhoDeDialogo
    /// ou por um texto de cena). Vazio se o puzzle for manual.
    /// </summary>
    public string TextoDoEnunciado()
    {
        if (_definicao == null) return "";
        return $"{_definicao.Enunciado}\n\n{_definicao.CodigoInicial}";
    }

    /// <summary>
    /// Chamado pelo terminal ao final de CADA execução (sucesso ou erro).
    /// Idempotente: depois de resolvido, não reexecuta os efeitos.
    /// </summary>
    public void VerificarVitoria()
    {
        if (_resolvido) return;
        if (terminal == null || terminal.Runner == null) return;

        List<VerificacaoDeVariavel> condicoes = ObterCondicoes();
        if (condicoes.Count == 0) return;

        bool todasSatisfeitas = true;

        foreach (VerificacaoDeVariavel cond in condicoes)
        {
            double? lido = terminal.Runner.ObterVariavelNumerica(cond.Variavel);

            if (logDeDepuracao)
            {
                string valorTexto = lido.HasValue ? lido.Value.ToString() : "(não existe / não é número)";
                Debug.Log($"[VerificadorDePuzzle] {cond.Variavel} = {valorTexto} | esperado: {cond.Descrever()}");
            }

            // Variavel ainda nao existe na sessao Lua (o jogador usou outro
            // nome, ou ainda nao rodou nada). Nao e erro -- so nao resolveu.
            if (!lido.HasValue || !cond.Satisfeita(lido.Value))
            {
                todasSatisfeitas = false;
            }
        }

        if (todasSatisfeitas)
        {
            AplicarEfeitosDeVitoria(persistir: true, mostrarFala: true);
            return;
        }

        // Ajuda progressiva: o jogador tentou e nao chegou la.
        _tentativas++;
        if (tentativasAteDica > 0 && _tentativas == tentativasAteDica && _definicao != null &&
            !string.IsNullOrWhiteSpace(_definicao.Dica))
        {
            MostrarFala($"{_definicao.Dica}");
        }
    }

    private List<VerificacaoDeVariavel> ObterCondicoes()
    {
        if (_definicao != null && _definicao.Verificacoes.Count > 0)
        {
            return _definicao.Verificacoes;
        }

        // Modo manual (v1): uma variável só, direto do Inspector.
        return new List<VerificacaoDeVariavel>
        {
            new VerificacaoDeVariavel
            {
                Variavel = nomeDaVariavel,
                Comparacao = comparacao,
                Valor = valorDeVitoria
            }
        };
    }

    void AplicarEfeitosDeVitoria(bool persistir, bool mostrarFala)
    {
        _resolvido = true;

        if (portaParaDestrancar != null)
        {
            // Nunca desabilitar o componente -- ver a explicação longa no
            // cabeçalho de TransicaoDeCena.cs. Só desmarca a flag.
            portaParaDestrancar.trancada = false;
        }

        if (mostrarFala)
        {
            string fala = _definicao != null && !string.IsNullOrWhiteSpace(_definicao.FalaDeVitoria)
                ? _definicao.FalaDeVitoria
                : falaAoResolverManual;
            MostrarFala(fala);
        }

        if (objetosParaAtivar != null)
        {
            foreach (GameObject obj in objetosParaAtivar) { if (obj != null) obj.SetActive(true); }
        }
        if (objetosParaDesativar != null)
        {
            foreach (GameObject obj in objetosParaDesativar) { if (obj != null) obj.SetActive(false); }
        }

        if (persistir)
        {
            if (!string.IsNullOrWhiteSpace(flagAoResolver))
            {
                GerenciadorDeEstado.Instancia.DefinirFlag(flagAoResolver, true);
            }

            // Progresso de quest (ex: objetivo "religar o painel de energia").
            if (!string.IsNullOrWhiteSpace(idDoPuzzle))
            {
                GerenciadorDeQuests.RegistrarProgresso(TipoDeObjetivo.ResolverPuzzle, idDoPuzzle);
            }

            _salvamento.Salvar();
        }
    }

    void MostrarFala(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        if (textoDeApollo != null)
        {
            textoDeApollo.text = $"Apollo: {texto}";
            textoDeApollo.gameObject.SetActive(true);
        }
        else if (terminal != null)
        {
            // Sem TMP de fala configurado, a voz do Apollo vai pro log do
            // terminal -- que o jogador já está olhando de qualquer jeito.
            terminal.EscreverNoLog($"{ApolloDica.Prefixo} {texto}");
        }
    }
}
