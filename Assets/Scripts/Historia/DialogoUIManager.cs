using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Painel unico de dialogo do jogo (Ato 1). Mostra uma sequencia de falas
/// (falante + texto) e, opcionalmente, um conjunto de escolhas no final.
///
/// Segue o MESMO contrato dos paineis que ja existem no projeto
/// (TelaDeStatusManager/TerminalUIManager/BagUIManager/LojaUIManager):
///   - campo "painel" raiz, desativado em Start();
///   - RegistrarMenuAberto/RegistrarMenuFechado no GerenciadorDeEstado, pra
///     congelar PlayerMovement e impedir NpcInterativo de reagir por baixo;
///   - callback opcional AoFechar.
///
/// DIFERENCA IMPORTANTE em relacao aos outros paineis: este NAO e arrastado
/// no Inspector por quem o usa. Gatilhos de dialogo ficam espalhados por
/// varias cenas, e o Unity NAO permite referencia de objeto entre cenas
/// diferentes (mesmo motivo que fez TransicaoDeCena trocar Transform por
/// Vector2 — ver comentario naquele arquivo). Por isso o acesso e via
/// DialogoUIManager.Instancia (estatico), resolvido em runtime.
///
/// MONTAGEM: coloque este painel DENTRO do mesmo Canvas raiz que ja tem o
/// MenuMundoManager (o Canvas que recebe DontDestroyOnLoad). Assim ele
/// sobrevive a troca de cena junto com o resto do menu, sem precisar de um
/// DontDestroyOnLoad proprio (que brigaria com a protecao de duplicata do
/// MenuMundoManager.Awake).
///
/// PAGINACAO DE TEXTO LONGO: textoFala deve estar com Overflow = Page no
/// Inspector (TextMeshPro - Text (UI) > Extra Settings > Overflow). Com
/// isso, uma fala com texto maior do que cabe na caixa e dividida em
/// "paginas" automaticamente pelo proprio TMP -- Avancar() avanca de
/// pagina antes de avancar de fala, entao uma Fala pode ter qualquer
/// tamanho de texto sem precisar ser quebrada em varias Falas na mao.
///
/// DEBUG TEMPORARIO: o Debug.Log no inicio de Mostrar() (marcado com
/// [DialogoUIManager]) e so pra rastrear por que o dialogo nao esta
/// disparando. Remova depois que o bug for resolvido.
/// </summary>
public class DialogoUIManager : MonoBehaviour
{
    // ------------------------------------------------------------------
    // ACESSO ESTATICO
    // ------------------------------------------------------------------

    private static DialogoUIManager _instancia;

    /// <summary>
    /// Instancia viva do painel de dialogo, ou null se nao houver nenhuma
    /// na cena (ex: voce deu Play direto numa cena sem o Canvas do menu).
    /// Quem chama SEMPRE deve checar null — um gatilho de dialogo nao pode
    /// derrubar o jogo so porque o painel nao foi montado ainda.
    /// </summary>
    public static DialogoUIManager Instancia
    {
        get
        {
            if (_instancia == null)
            {
                // FindAnyObjectByType inclui objetos vindos de DontDestroyOnLoad.
                // CORRECAO (Unity 6): FindObjectOfType estava obsoleto
                // (CS0618) -- FindAnyObjectByType e o substituto direto
                // quando so importa achar UM (nao precisa de ordenacao).
                _instancia = FindAnyObjectByType<DialogoUIManager>();
            }
            return _instancia;
        }
    }

    // ------------------------------------------------------------------
    // INSPECTOR
    // ------------------------------------------------------------------

    [Header("Painel raiz")]
    public GameObject painel;

    [Header("Texto")]
    [Tooltip("Nome de quem esta falando (ex: \"Apollo\"). Opcional — se ficar vazio, o campo e escondido.")]
    public TextMeshProUGUI textoFalante;
    [Tooltip("IMPORTANTE: deixe Overflow = Page no Inspector deste componente (Extra Settings > Overflow), pra textos longos paginarem em vez de vazar da caixa.")]
    public TextMeshProUGUI textoFala;

    [Header("Avancar")]
    [Tooltip("Botao \"continuar\". Pode ser o proprio painel inteiro com um Button em cima.")]
    public Button botaoAvancar;
    [Tooltip("Teclas que tambem avancam a fala.")]
    public KeyCode teclaAvancar = KeyCode.E;
    public KeyCode teclaAvancarAlternativa = KeyCode.Space;

    [Header("Escolhas (opcional — so usado por dialogos com ramificacao)")]
    [Tooltip("Container onde os botoes de escolha sao instanciados. Deixe desativado por padrao.")]
    public Transform painelDeEscolhas;
    [Tooltip("Prefab de botao com um TextMeshProUGUI filho. Pode reusar o mesmo prefab da Bag/Loja.")]
    public Button prefabBotaoEscolha;

    /// <summary>Mesmo contrato dos outros paineis. Opcional.</summary>
    public Action AoFechar;

    // ------------------------------------------------------------------
    // ESTADO INTERNO
    // ------------------------------------------------------------------

    private readonly List<FalaDeDialogo> _falas = new List<FalaDeDialogo>();
    private readonly List<EscolhaDeDialogo> _escolhas = new List<EscolhaDeDialogo>();
    private Action<EscolhaDeDialogo> _aoConcluir;

    private int _indiceDaFala;
    private bool _mostrandoEscolhas;
    private bool _aberto;

    // Sem isso, a MESMA tecla E que abriu o dialogo (em NpcInterativo ou
    // GatilhoDeDialogo) seria lida de novo por este Update no mesmo frame,
    // pulando a primeira fala instantaneamente.
    private bool _ignorarInputEsteFrame;

    private readonly ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    void Awake()
    {
        if (_instancia != null && _instancia != this)
        {
            // Duplicata (ex: Canvas do menu colocado manualmente numa segunda
            // cena). Nao destroi o GameObject inteiro aqui -- isso e papel do
            // MenuMundoManager.Awake, que ja cuida do Canvas raiz. So ignora.
            return;
        }
        _instancia = this;
    }

    void Start()
    {
        if (botaoAvancar != null) botaoAvancar.onClick.AddListener(Avancar);
        if (painel != null) painel.SetActive(false);
        if (painelDeEscolhas != null) painelDeEscolhas.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (_instancia == this) _instancia = null;
    }

    void Update()
    {
        if (!_aberto) return;

        if (_ignorarInputEsteFrame)
        {
            _ignorarInputEsteFrame = false;
            return;
        }

        // Enquanto as escolhas estao na tela, so clique resolve -- tecla
        // nenhuma avanca, senao o jogador "escolheria" sem querer.
        if (_mostrandoEscolhas) return;

        if (Input.GetKeyDown(teclaAvancar) || Input.GetKeyDown(teclaAvancarAlternativa) || Input.GetKeyDown(KeyCode.Return))
        {
            Avancar();
        }
    }

    // ------------------------------------------------------------------
    // API PUBLICA
    // ------------------------------------------------------------------

    /// <summary>Dialogo simples, sem ramificacao. Devolve false se nao abriu.</summary>
    public bool Mostrar(IList<FalaDeDialogo> falas, Action aoConcluir = null)
    {
        return Mostrar(falas, null, aoConcluir == null ? (Action<EscolhaDeDialogo>)null : _ => aoConcluir());
    }

    /// <summary>
    /// Dialogo com escolhas opcionais no final. O callback recebe a escolha
    /// feita, ou null se o dialogo nao tinha escolhas.
    /// </summary>
    public bool Mostrar(IList<FalaDeDialogo> falas, IList<EscolhaDeDialogo> escolhas, Action<EscolhaDeDialogo> aoConcluir)
    {
        // DEBUG: confirma se este metodo esta sendo chamado, e se o painel
        // ja estava preso como "_aberto = true" de uma tentativa anterior.
        Debug.Log($"[DialogoUIManager] Mostrar chamado em '{gameObject.name}'. _aberto = {_aberto}, painel null? {painel == null}");

        if (painel == null || textoFala == null)
        {
            Debug.LogWarning("DialogoUIManager: painel/textoFala nao configurados no Inspector. Dialogo ignorado.");
            aoConcluir?.Invoke(null);
            return false;
        }

        if (_aberto)
        {
            // Ja tem um dialogo rodando. Ignorar e mais seguro do que
            // sobrescrever no meio (o callback do primeiro se perderia).
            // Devolve false pra quem chamou poder tentar de novo depois, em
            // vez de marcar o gatilho como "ja usado" por engano.
            return false;
        }

        _falas.Clear();
        if (falas != null) _falas.AddRange(falas);

        _escolhas.Clear();
        if (escolhas != null) _escolhas.AddRange(escolhas);

        _aoConcluir = aoConcluir;
        _indiceDaFala = 0;
        _mostrandoEscolhas = false;
        _aberto = true;
        _ignorarInputEsteFrame = true;

        GerenciadorDeEstado.Instancia.RegistrarMenuAberto();
        painel.SetActive(true);

        if (_falas.Count == 0)
        {
            // Dialogo so de escolha (raro, mas valido).
            MostrarEscolhas();
        }
        else
        {
            MostrarFalaAtual();
        }

        return true;
    }

    // ------------------------------------------------------------------
    // FLUXO
    // ------------------------------------------------------------------

    void MostrarFalaAtual()
    {
        FalaDeDialogo fala = _falas[_indiceDaFala];

        if (textoFalante != null)
        {
            bool temFalante = !string.IsNullOrWhiteSpace(fala.falante);
            textoFalante.gameObject.SetActive(temFalante);
            if (temFalante) textoFalante.text = fala.falante;
        }

        textoFala.text = fala.texto;

        // NOVO: toda fala nova comeca na primeira "pagina" do texto (so faz
        // diferenca quando o Overflow do TMP esta configurado como Page).
        // ForceMeshUpdate garante que textInfo.pageCount ja esta correto no
        // mesmo frame, sem depender do proximo recalculo automatico do TMP.
        textoFala.pageToDisplay = 1;
        textoFala.ForceMeshUpdate();

        if (botaoAvancar != null) botaoAvancar.gameObject.SetActive(true);
        if (painelDeEscolhas != null) painelDeEscolhas.gameObject.SetActive(false);
    }

    public void Avancar()
    {
        if (!_aberto || _mostrandoEscolhas) return;

        // NOVO: se o texto atual tem mais "paginas" (nao coube inteiro na
        // caixa -- Overflow = Page no TextoFala), avanca a pagina em vez de
        // pular pra proxima Fala. So passa pra proxima Fala quando todas as
        // paginas da atual ja foram mostradas. Isso e o que permite uma
        // unica Fala ter um texto de qualquer tamanho, sem quebrar em
        // varias Falas manualmente no Inspector.
        if (textoFala != null && textoFala.textInfo != null
            && textoFala.pageToDisplay < textoFala.textInfo.pageCount)
        {
            textoFala.pageToDisplay++;
            return;
        }

        _indiceDaFala++;

        if (_indiceDaFala < _falas.Count)
        {
            MostrarFalaAtual();
            return;
        }

        if (_escolhas.Count > 0)
        {
            MostrarEscolhas();
            return;
        }

        Concluir(null);
    }

    void MostrarEscolhas()
    {
        if (painelDeEscolhas == null || prefabBotaoEscolha == null)
        {
            Debug.LogWarning("DialogoUIManager: dialogo tem escolhas mas painelDeEscolhas/prefabBotaoEscolha nao foram configurados. Concluindo sem escolha.");
            Concluir(null);
            return;
        }

        _mostrandoEscolhas = true;
        if (botaoAvancar != null) botaoAvancar.gameObject.SetActive(false);

        // Mesma correcao de layout ja documentada em BagUIManager.Mostrar:
        // ativar o container ANTES de instanciar os filhos, senao o
        // ContentSizeFitter trava em 0x0 (Unity so recalcula layout em
        // objetos ativos).
        painelDeEscolhas.gameObject.SetActive(true);

        foreach (Transform filho in painelDeEscolhas)
        {
            Destroy(filho.gameObject);
        }

        foreach (EscolhaDeDialogo escolha in _escolhas)
        {
            Button botao = Instantiate(prefabBotaoEscolha, painelDeEscolhas);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = escolha.texto;

            EscolhaDeDialogo capturada = escolha; // evita captura errada da variavel de loop
            botao.onClick.AddListener(() => AoClicarEscolha(capturada));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(painelDeEscolhas.GetComponent<RectTransform>());
    }

    void AoClicarEscolha(EscolhaDeDialogo escolha)
    {
        if (!_mostrandoEscolhas) return;

        AplicarConsequencias(escolha);

        // Se a escolha tem uma fala de resposta, ela vira a proxima (e
        // ultima) fala do dialogo, em vez de fechar de imediato.
        if (!string.IsNullOrWhiteSpace(escolha.respostaAposEscolher))
        {
            _mostrandoEscolhas = false;
            _ignorarInputEsteFrame = true; // Enter no botao nao pode avancar a resposta no mesmo frame
            _escolhas.Clear();

            _falas.Clear();
            _falas.Add(new FalaDeDialogo
            {
                falante = escolha.falanteDaResposta,
                texto = escolha.respostaAposEscolher
            });
            _indiceDaFala = 0;

            if (painelDeEscolhas != null) painelDeEscolhas.gameObject.SetActive(false);
            MostrarFalaAtual();

            // Guarda a escolha pra entregar no Concluir la na frente.
            _escolhaPendente = escolha;
            return;
        }

        Concluir(escolha);
    }

    private EscolhaDeDialogo _escolhaPendente;

    void AplicarConsequencias(EscolhaDeDialogo escolha)
    {
        bool mudouAlgo = false;
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (!string.IsNullOrWhiteSpace(escolha.flagAoEscolher))
        {
            gerenciador.DefinirFlag(escolha.flagAoEscolher, true);
            mudouAlgo = true;
        }

        if (!string.IsNullOrWhiteSpace(escolha.idItemConcedido) && escolha.quantidadeDoItem > 0)
        {
            gerenciador.AdicionarItem(escolha.idItemConcedido, escolha.quantidadeDoItem);
            mudouAlgo = true;
        }

        if (mudouAlgo)
        {
            // Mesmo padrao de "persistir imediatamente" que BattleManager.UsarItem
            // e BagUIManager ja usam: a consequencia ja foi aplicada ao estado,
            // entao nao pode se perder se o jogo fechar antes do proximo save.
            _salvamento.Salvar();
        }
    }

    void Concluir(EscolhaDeDialogo escolha)
    {
        if (escolha == null && _escolhaPendente != null)
        {
            escolha = _escolhaPendente;
        }
        _escolhaPendente = null;

        Action<EscolhaDeDialogo> callback = _aoConcluir;
        _aoConcluir = null;

        Fechar();
        callback?.Invoke(escolha);
    }

    public void Fechar()
    {
        if (!_aberto) return;

        _aberto = false;
        _mostrandoEscolhas = false;

        if (painelDeEscolhas != null)
        {
            foreach (Transform filho in painelDeEscolhas)
            {
                Destroy(filho.gameObject);
            }
            painelDeEscolhas.gameObject.SetActive(false);
        }

        if (painel != null) painel.SetActive(false);

        GerenciadorDeEstado.Instancia.RegistrarMenuFechado();
        AoFechar?.Invoke();
    }
}

/// <summary>Uma linha de fala. Serializavel pra aparecer no Inspector.</summary>
[Serializable]
public class FalaDeDialogo
{
    [Tooltip("Quem fala. Deixe vazio pra narracao (o campo de nome some).")]
    public string falante = "Apollo";

    [TextArea(2, 6)]
    public string texto;
}

/// <summary>
/// Uma opcao no fim de um dialogo. A consequencia e declarativa (flag + item)
/// pra que a ramificacao de faccao do Ato 1 nao exija script proprio.
/// </summary>
[Serializable]
public class EscolhaDeDialogo
{
    [Tooltip("Texto do botao (ex: \"Flor de Ferro\").")]
    public string texto;

    [Tooltip("Flag gravada no GerenciadorDeEstado ao escolher (ex: \"ato1_faccao_ferro\"). Persiste no save.")]
    public string flagAoEscolher;

    [Tooltip("Id de item do CatalogoDeItens entregue ao escolher (ex: \"nucleo_sobrecarga\"). Deixe vazio pra nenhuma recompensa.")]
    public string idItemConcedido;

    public int quantidadeDoItem = 1;

    [Tooltip("Nome de quem responde depois da escolha. Opcional.")]
    public string falanteDaResposta = "Apollo";

    [TextArea(2, 5)]
    [Tooltip("Fala mostrada logo apos a escolha. Deixe vazio pra fechar o dialogo direto.")]
    public string respostaAposEscolher;
}
