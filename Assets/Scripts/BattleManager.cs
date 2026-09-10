using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DroidsCode.Combat;
using DroidsCode.DroidCore;

/// <summary>
/// Lógica de turnos da batalha. Este script NÃO cria nenhuma interface —
/// ele espera que você já tenha montado os elementos no Editor e apenas
/// arraste as referências nos campos abaixo, no Inspector.
///
/// Refatorado para consumir Droid/CombatEngine em vez de calcular dano
/// aqui dentro (ver MATRIZ_DESENVOLVIMENTO.md e CORRECAO_ARQUITETURA_TERMINAL_MENU.md).
/// "Atacar" agora abre uma lista de golpes se o Droid tiver mais de um
/// configurado; se só tiver o Ataque Básico, ataca direto.
///
/// NOVO NO INSPECTOR — precisa criar 2 objetos na cena (ver instruções
/// enviadas junto com este arquivo):
///   - painelListaDeAtaques: painel vazio, inativo por padrão
///   - prefabBotaoAtaque: prefab de botão com um texto (TMP) filho
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Referências de UI (arraste os objetos da Hierarchy aqui)")]
    public Slider sliderPlayer;
    public Slider sliderInimigo;
    public TextMeshProUGUI textoMensagem;
    public Button botaoAtacar;
    public Button botaoItem;
    public Button botaoFugir;
    public Button botaoStatus;
    public TelaDeStatusManager telaDeStatus;

    [Header("Lista de golpes (novo)")]
    [Tooltip("Painel vazio, desativado por padrão, que recebe os botões de cada golpe.")]
    public Transform painelListaDeAtaques;
    [Tooltip("Prefab de um botão simples (com TextMeshProUGUI filho), um por golpe.")]
    public Button prefabBotaoAtaque;

    [Header("Status do Player")]
    // nome/hp/atributos agora vêm do GerenciadorDeEstado.DroidDoJogador —
    // esses campos foram removidos daqui de propósito (ver GerenciadorDeEstado.cs).

    [Header("Status do Inimigo")]
    public string nomeInimigo = "Esqueleto";
    public int hpMaxInimigo = 20;
    public int atkInimigo = 5;
    public int defInimigo = 1;

    [Header("Configuração")]
    [Tooltip("Cena para onde voltar depois da batalha.")]
    public string nomeCenaMundo = "Game";
    [Range(0f, 1f)]
    public float chanceDeFugir = 0.5f;

    private Droid droid;
    private InimigoFixo inimigo;
    private CombatEngine engine;
    private bool turnoDoJogador = true;
    private bool batalhaEncerrada = false;

    void Start()
    {
        droid = GerenciadorDeEstado.Instancia.DroidDoJogador;

        inimigo = new InimigoFixo(nomeInimigo, hpMaxInimigo, atkInimigo, defInimigo);
        engine = new CombatEngine();

        botaoAtacar.onClick.AddListener(AoClicarAtacar);
        botaoItem.onClick.AddListener(AoClicarItem);
        botaoFugir.onClick.AddListener(AoClicarFugir);
        if (botaoStatus != null && telaDeStatus != null)
        {
            botaoStatus.onClick.AddListener(() => telaDeStatus.Mostrar());
        }

        if (painelListaDeAtaques != null)
            painelListaDeAtaques.gameObject.SetActive(false);

        sliderPlayer.minValue = 0;
        sliderPlayer.maxValue = 1;
        sliderInimigo.minValue = 0;
        sliderInimigo.maxValue = 1;

        AtualizarBarras();
        MostrarMensagem($"Um {nomeInimigo} selvagem apareceu!");
    }

    // ------------------------------------------------------------------
    // AÇÕES DOS BOTÕES
    // ------------------------------------------------------------------

    void AoClicarAtacar()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        var acoes = new List<string>(droid.ObterAcoesDisponiveis());

        if (acoes.Count <= 1)
        {
            string unica = acoes.Count == 1 ? acoes[0] : Droid.NomeAtaqueBasico;
            ExecutarAtaqueDoJogador(unica);
        }
        else
        {
            AbrirListaDeAtaques(acoes);
        }
    }

    void AbrirListaDeAtaques(List<string> acoes)
    {
        if (painelListaDeAtaques == null || prefabBotaoAtaque == null)
        {
            Debug.LogWarning("painelListaDeAtaques/prefabBotaoAtaque não configurados no Inspector — atacando com o básico.");
            ExecutarAtaqueDoJogador(Droid.NomeAtaqueBasico);
            return;
        }

        foreach (Transform filho in painelListaDeAtaques)
            Destroy(filho.gameObject);

        foreach (string nomeAcao in acoes)
        {
            Button botao = Instantiate(prefabBotaoAtaque, painelListaDeAtaques);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = nomeAcao;

            string nomeCapturado = nomeAcao; // evita captura errada da variavel de loop
            botao.onClick.AddListener(() =>
            {
                painelListaDeAtaques.gameObject.SetActive(false);
                ExecutarAtaqueDoJogador(nomeCapturado);
            });
        }

        painelListaDeAtaques.gameObject.SetActive(true);
    }

    void ExecutarAtaqueDoJogador(string nomeAcao)
    {
        ResultadoAcao resultado = engine.ExecutarTurno(droid, inimigo, nomeAcao);
        AtualizarBarras();
        MostrarMensagem(resultado.Mensagem);

        if (engine.VerificarDerrota(inimigo))
            StartCoroutine(FinalizarBatalha(true));
        else
            StartCoroutine(TurnoDoInimigo());
    }

    void AoClicarItem()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        // TODO: sistema de inventario real fica pra Fase 2 (fora de escopo
        // do Ato 1, ver LEIA_PRIMEIRO.md). Mantido como cura fixa por ora.
        int cura = 5;
        droid.Hp = Mathf.Min(droid.HpMax, droid.Hp + cura);
        AtualizarBarras();
        MostrarMensagem($"{droid.Nome} usou um item e recuperou {cura} de HP!");

        StartCoroutine(TurnoDoInimigo());
    }

    void AoClicarFugir()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        if (Random.value <= chanceDeFugir)
        {
            MostrarMensagem("Você fugiu com segurança!");
            StartCoroutine(VoltarParaOMundo(1.5f));
        }
        else
        {
            MostrarMensagem("Não foi possível fugir!");
            StartCoroutine(TurnoDoInimigo());
        }
    }

    // ------------------------------------------------------------------
    // FLUXO DE TURNOS
    // ------------------------------------------------------------------

    IEnumerator TurnoDoInimigo()
    {
        turnoDoJogador = false;
        DefinirBotoesInterativos(false);
        yield return new WaitForSeconds(1f);

        string acaoInimigo = new List<string>(inimigo.ObterAcoesDisponiveis())[0];
        ResultadoAcao resultado = engine.ExecutarTurno(inimigo, droid, acaoInimigo);
        AtualizarBarras();
        MostrarMensagem(resultado.Mensagem);
        yield return new WaitForSeconds(1f);

        if (engine.VerificarDerrota(droid))
        {
            StartCoroutine(FinalizarBatalha(false));
        }
        else
        {
            turnoDoJogador = true;
            DefinirBotoesInterativos(true);
            MostrarMensagem("O que você vai fazer?");
        }
    }

    IEnumerator FinalizarBatalha(bool vitoria)
    {
        batalhaEncerrada = true;
        DefinirBotoesInterativos(false);
        MostrarMensagem(vitoria ? $"Você derrotou o {nomeInimigo}!" : "Você foi derrotado...");
        yield return VoltarParaOMundo(2f);
    }

    IEnumerator VoltarParaOMundo(float espera)
    {
        yield return new WaitForSeconds(espera);
        if (!string.IsNullOrEmpty(nomeCenaMundo))
            SceneManager.LoadScene(nomeCenaMundo);
    }

    // ------------------------------------------------------------------
    // AUXILIARES
    // ------------------------------------------------------------------

    void AtualizarBarras()
    {
        sliderPlayer.value = (float)droid.Hp / droid.HpMax;
        sliderInimigo.value = (float)inimigo.Hp / inimigo.HpMax;
    }

    void MostrarMensagem(string mensagem)
    {
        textoMensagem.text = mensagem;
    }

    void DefinirBotoesInterativos(bool ativo)
    {
        botaoAtacar.interactable = ativo;
        botaoItem.interactable = ativo;
        botaoFugir.interactable = ativo;
    }
}
