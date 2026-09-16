using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dispara um dialogo (ver DialogoUIManager) por proximidade + tecla, ou ao
/// entrar num trigger.
///
/// CONTEUDO: as falas podem vir de duas formas --
///   1) Digitadas direto no campo "Falas" do Inspector (jeito antigo, ainda
///      funciona, tem PRIORIDADE se preenchido);
///   2) Por "Id Do Dialogo", buscando no CatalogoDeDialogos.cs (jeito novo,
///      recomendado -- usado quando "Falas" estiver vazio). E o formato que
///      escala pro roteiro inteiro do jogo: o texto fica centralizado num
///      unico arquivo de codigo, versionado, em vez de espalhado por
///      dezenas de componentes na cena.
///
/// Condicoes de historia sao declarativas: o gatilho so dispara se TODAS as
/// flags de "flagsNecessarias" estiverem ativas e NENHUMA de
/// "flagsQueImpedem" estiver. Isso e o que faz a mesma cena (City) contar
/// coisas diferentes na Sessao 2 e na Sessao 7 sem script novo.
///
/// Colocar no GameObject do NPC/gatilho, junto com um Collider2D marcado
/// como "Is Trigger" (e um Rigidbody2D nao-Static, se o collider for
/// Composite -- Static+Static ou Static+Kinematic nao gera trigger no
/// Unity 2D).
///
/// DEBUG TEMPORARIO: os Debug.Log abaixo (marcados com [GatilhoDeDialogo])
/// sao so pra rastrear por que o dialogo nao esta disparando. Remova depois
/// que o bug for resolvido.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GatilhoDeDialogo : MonoBehaviour
{
    public enum ModoDeDisparo
    {
        AoInteragir,        // player entra no raio e aperta a tecla (NPCs)
        AoEntrarNoTrigger,  // dispara sozinho ao encostar (cutscenes de passagem)
        ApenasPorScript     // nao reage a nada sozinho -- quem dispara e outro componente
    }

    /// <summary>
    /// Invocado quando o dialogo termina, DEPOIS de gravar flagAoTerminar.
    /// Atribuido por codigo (nao aparece no Inspector). E o que permite o
    /// NpcInterativo do Tacio contar a fala dele e so entao abrir a Loja.
    /// </summary>
    public System.Action AoConcluirDialogoExterno;

    [Header("Disparo")]
    public ModoDeDisparo modo = ModoDeDisparo.AoInteragir;

    [Tooltip("So usado quando Modo = Ao Interagir. Se este GameObject tambem tem um NpcInterativo com Tipo = Dialogo, use Modo = Apenas Por Script aqui, senao os dois reagem a mesma tecla.")]
    public KeyCode teclaDeInteracao = KeyCode.E;

    [Tooltip("Tag do player. Igual ao resto do projeto: \"Player\".")]
    public string tagDoPlayer = "Player";

    [Header("Condicoes de historia")]
    [Tooltip("O dialogo so dispara se TODAS estas flags estiverem ativas. Vazio = sem exigencia.")]
    public string[] flagsNecessarias;

    [Tooltip("O dialogo NAO dispara se QUALQUER uma destas flags estiver ativa. Use pra aposentar um dialogo depois que a historia avancou.")]
    public string[] flagsQueImpedem;

    [Header("Conteudo")]
    [Tooltip("Id do dialogo no CatalogoDeDialogos.cs (ex: \"city_s2_dara\"). So e usado se o campo \"Falas\" abaixo estiver vazio -- e assim que a maioria dos NPCs deve ser configurada.")]
    public string idDoDialogo;

    [Tooltip("Falas digitadas direto aqui. Se preenchido, tem PRIORIDADE sobre Id Do Dialogo (compatibilidade com o que ja foi montado no Inspector). Deixe vazio e use Id Do Dialogo pra dialogos novos.")]
    public List<FalaDeDialogo> falas = new List<FalaDeDialogo>();

    [Tooltip("Opcional. Se preenchido, aparece como botoes no fim do dialogo (ex: escolha de faccao). Nao vem do CatalogoDeDialogos -- e configurado aqui mesmo.")]
    public List<EscolhaDeDialogo> escolhas = new List<EscolhaDeDialogo>();

    [Header("Consequencia")]
    [Tooltip("Flag gravada quando o dialogo termina (ex: \"ato1_falou_dara\"). Persiste no save.")]
    public string flagAoTerminar;

    [Tooltip("Se marcado, o dialogo nao repete depois de concluido nesta sessao. Se voce tambem preencheu flagAoTerminar, a repeticao fica bloqueada entre sessoes tambem (adicione a mesma flag em flagsQueImpedem pra isso).")]
    public bool apenasUmaVez = true;

    [Tooltip("Desativa este GameObject depois que o dialogo termina. Util pra gatilhos de passagem que nao devem existir mais.")]
    public bool desativarAposConcluir = false;

    [Header("Indicador visual (opcional)")]
    [Tooltip("Filho (ex: balao \"!\") ligado quando o player esta perto e o dialogo esta disponivel.")]
    public GameObject indicadorDeInteracao;

    private bool _playerPerto;
    private bool _jaDisparouNestaSessao;

    private readonly ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    void Reset()
    {
        // Conveniencia de Editor: ja deixa o Collider2D como trigger, mesmo
        // padrao de InteragirComTerminalCasa.
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Awake()
    {
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
    }

    void Update()
    {
        if (modo != ModoDeDisparo.AoInteragir) return;

        // Mesmo cuidado de NpcInterativo/PlayerMovement: nao reagir com um
        // painel ja aberto por cima (inclusive o proprio dialogo).
        if (GerenciadorDeEstado.Instancia.MenuAberto)
        {
            if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
            return;
        }

        bool disponivel = _playerPerto && PodeDisparar();
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(disponivel);

        if (disponivel && Input.GetKeyDown(teclaDeInteracao))
        {
            Disparar();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // DEBUG: confirma se a fisica esta detectando QUALQUER coisa aqui,
        // mesmo que a tag esteja errada.
        Debug.Log($"[GatilhoDeDialogo] OnTriggerEnter2D em '{gameObject.name}'. Objeto: {other.gameObject.name}, tag: {other.tag}");

        if (!other.CompareTag(tagDoPlayer)) return;

        _playerPerto = true;

        if (modo == ModoDeDisparo.AoEntrarNoTrigger && PodeDisparar() && !GerenciadorDeEstado.Instancia.MenuAberto)
        {
            Disparar();
        }
        else if (modo == ModoDeDisparo.AoEntrarNoTrigger)
        {
            // DEBUG: mostra qual condicao bloqueou o disparo.
            Debug.Log($"[GatilhoDeDialogo] Nao disparou. PodeDisparar={PodeDisparar()} MenuAberto={GerenciadorDeEstado.Instancia.MenuAberto}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoPlayer)) return;

        _playerPerto = false;
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
    }

    /// <summary>
    /// Falas efetivamente usadas ao disparar: o campo "Falas" do Inspector
    /// tem prioridade (compatibilidade com dialogos ja montados na mao); se
    /// estiver vazio, busca no CatalogoDeDialogos pelo Id Do Dialogo. Pode
    /// devolver null se nenhum dos dois estiver preenchido/existir.
    /// </summary>
    private List<FalaDeDialogo> ObterFalas()
    {
        if (falas != null && falas.Count > 0) return falas;
        return CatalogoDeDialogos.Obter(idDoDialogo);
    }

    /// <summary>
    /// Publico porque NpcInterativo (Tipo = Dialogo) delega pra ca, em vez de
    /// duplicar deteccao de proximidade.
    /// </summary>
    public bool PodeDisparar()
    {
        if (apenasUmaVez && _jaDisparouNestaSessao) return false;

        List<FalaDeDialogo> falasResolvidas = ObterFalas();
        bool semFalas = falasResolvidas == null || falasResolvidas.Count == 0;
        if (semFalas && escolhas.Count == 0) return false;

        return CondicoesDeHistoria.Satisfeitas(flagsNecessarias, flagsQueImpedem);
    }

    /// <summary>Publico pra permitir disparo por script (ex: NpcInterativo).</summary>
    public void Disparar()
    {
        DialogoUIManager painel = DialogoUIManager.Instancia;

        // DEBUG: confirma se a instancia estatica do painel foi achada.
        Debug.Log($"[GatilhoDeDialogo] Disparar em '{gameObject.name}'. Painel encontrado? {painel != null}");

        if (painel == null)
        {
            Debug.LogWarning($"GatilhoDeDialogo '{gameObject.name}': nenhum DialogoUIManager encontrado na cena. Coloque o painel de dialogo dentro do Canvas persistente do MenuMundoManager.");
            return;
        }

        List<FalaDeDialogo> falasResolvidas = ObterFalas();

        if (falasResolvidas == null && escolhas.Count == 0)
        {
            Debug.LogWarning($"GatilhoDeDialogo '{gameObject.name}': Id Do Dialogo \"{idDoDialogo}\" nao existe no CatalogoDeDialogos, e o campo Falas esta vazio. Nada pra mostrar.");
            return;
        }

        // So marca como usado se o painel REALMENTE abriu. Se ele recusar
        // (outro dialogo em andamento, painel mal configurado), o gatilho
        // continua disponivel em vez de morrer em silencio.
        bool abriu = painel.Mostrar(falasResolvidas, escolhas, AoConcluirDialogo);

        // DEBUG: confirma o retorno de Mostrar().
        Debug.Log($"[GatilhoDeDialogo] painel.Mostrar retornou: {abriu}");

        if (!abriu) return;

        _jaDisparouNestaSessao = true;
        if (indicadorDeInteracao != null) indicadorDeInteracao.SetActive(false);
    }

    void AoConcluirDialogo(EscolhaDeDialogo escolhaFeita)
    {
        if (!string.IsNullOrWhiteSpace(flagAoTerminar))
        {
            GerenciadorDeEstado.Instancia.DefinirFlag(flagAoTerminar, true);
            // A escolha (se houve) ja foi persistida pelo proprio
            // DialogoUIManager; aqui salvamos a flag de conclusao.
            _salvamento.Salvar();
        }

        AoConcluirDialogoExterno?.Invoke();

        if (desativarAposConcluir)
        {
            gameObject.SetActive(false);
        }
    }
}

/// <summary>
/// Avaliacao de condicao de historia, compartilhada por GatilhoDeDialogo,
/// CondicaoDeHistoria, TrancaPorHistoria, GatilhoDeBatalha e EncounterZone.
/// Centralizada aqui pra que a regra ("todas as necessarias E nenhuma das
/// que impedem") exista num lugar so.
/// </summary>
public static class CondicoesDeHistoria
{
    public static bool Satisfeitas(string[] flagsNecessarias, string[] flagsQueImpedem)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (flagsNecessarias != null)
        {
            foreach (string flag in flagsNecessarias)
            {
                if (string.IsNullOrWhiteSpace(flag)) continue;
                if (!gerenciador.ObterFlag(flag)) return false;
            }
        }

        if (flagsQueImpedem != null)
        {
            foreach (string flag in flagsQueImpedem)
            {
                if (string.IsNullOrWhiteSpace(flag)) continue;
                if (gerenciador.ObterFlag(flag)) return false;
            }
        }

        return true;
    }
}
