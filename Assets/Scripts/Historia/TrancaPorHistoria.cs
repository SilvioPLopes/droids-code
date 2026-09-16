using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Controla o campo "trancada" de um TransicaoDeCena a partir do estado do
/// jogo. E o portao de progressao do Ato 1: a saida leste de Ferrovale e a
/// entrada da Cratera.
///
/// Duas classes de exigencia, combinaveis:
///
///  1. FLAGS DE HISTORIA — "voce ainda nao descobriu o que aconteceu com a
///     agua" (portao leste antes da Sessao 2 estar concluida).
///
///  2. TECNICAS CONFIGURADAS — "voce ainda nao programou o Apollo".
///     Esta e a exigencia mais importante do TCC inteiro: e o que transforma
///     o terminal de painel opcional em caminho critico. Sem escrever codigo
///     no terminal, o jogador literalmente nao sai da cidade.
///
/// Conta tecnicas via Droid.ObterAcoesDisponiveis() (que inclui o Ataque
/// Basico) em vez de TecnicasConfiguradas.Count — assim a regra continua
/// certa independente de o Ataque Basico morar ou nao no dicionario, que e
/// detalhe interno do Droid.
///
/// Reavalia em Update() de proposito: o jogador pode abrir o Menu do Mundo,
/// configurar uma tecnica no terminal e voltar pra porta sem trocar de cena.
/// A porta precisa perceber isso na hora.
/// </summary>
[RequireComponent(typeof(TransicaoDeCena))]
public class TrancaPorHistoria : MonoBehaviour
{
    [Header("Exigencia 1 — flags de historia")]
    [Tooltip("A porta so abre se TODAS estiverem ativas. Vazio = sem exigencia de flag.")]
    public string[] flagsNecessarias;

    [Tooltip("A porta trava se QUALQUER uma destas estiver ativa (ex: fechar a volta depois que o Ato acabou).")]
    public string[] flagsQueImpedem;

    [TextArea(2, 4)]
    [Tooltip("Aviso mostrado quando falta flag. Deixe vazio pra usar a mensagem padrao do proprio TransicaoDeCena.")]
    public string mensagemQuandoFaltaFlag = "Ainda nao sei o que esta acontecendo. Preciso falar com as pessoas da praca primeiro.";

    [Header("Exigencia 2 — o jogador precisa ter programado o Droid")]
    [Tooltip("Se marcado, a porta so abre quando o Droid tiver ao menos uma tecnica ALEM do Ataque Basico configurada no terminal.")]
    public bool exigirTecnicaConfigurada = false;

    [Tooltip("Quantas acoes o Droid precisa ter no total (Ataque Basico conta como 1). 2 = pelo menos uma tecnica escrita no terminal.")]
    public int minimoDeAcoes = 2;

    [TextArea(2, 4)]
    public string mensagemQuandoFaltaTecnica = "Voce quer sair daqui com um braco que so sabe bater? Esc. Terminal. Me programa primeiro.";

    [Header("Comportamento")]
    [Tooltip("Se marcado, a porta volta a trancar caso a condicao deixe de valer. Normalmente deixe DESMARCADO em portas de historia (uma vez aberta, fica aberta).")]
    public bool podeTrancarDeNovo = false;

    private TransicaoDeCena _transicao;
    private string _mensagemOriginal;
    private bool _jaDestrancou;

    void Awake()
    {
        _transicao = GetComponent<TransicaoDeCena>();
        _mensagemOriginal = _transicao.mensagemDeBloqueio;
    }

    void Start()
    {
        Avaliar();
    }

    void Update()
    {
        Avaliar();
    }

    void Avaliar()
    {
        if (_jaDestrancou && !podeTrancarDeNovo) return;

        bool flagsOk = CondicoesDeHistoria.Satisfeitas(flagsNecessarias, flagsQueImpedem);
        bool tecnicaOk = !exigirTecnicaConfigurada || TemTecnicaSuficiente();

        if (flagsOk && tecnicaOk)
        {
            _transicao.trancada = false;
            _transicao.mensagemDeBloqueio = _mensagemOriginal;
            _jaDestrancou = true;
            return;
        }

        _transicao.trancada = true;

        // A mensagem explica QUAL das duas exigencias esta faltando —
        // sem isso o jogador bate numa porta muda e nao sabe o que fazer.
        if (!flagsOk && !string.IsNullOrWhiteSpace(mensagemQuandoFaltaFlag))
        {
            _transicao.mensagemDeBloqueio = mensagemQuandoFaltaFlag;
        }
        else if (!tecnicaOk && !string.IsNullOrWhiteSpace(mensagemQuandoFaltaTecnica))
        {
            _transicao.mensagemDeBloqueio = mensagemQuandoFaltaTecnica;
        }
        else
        {
            _transicao.mensagemDeBloqueio = _mensagemOriginal;
        }
    }

    bool TemTecnicaSuficiente()
    {
        Droid droid = GerenciadorDeEstado.Instancia.DroidDoJogador;
        if (droid == null) return false;

        int total = 0;
        foreach (string acao in droid.ObterAcoesDisponiveis())
        {
            if (!string.IsNullOrEmpty(acao)) total++;
        }

        return total >= minimoDeAcoes;
    }
}
