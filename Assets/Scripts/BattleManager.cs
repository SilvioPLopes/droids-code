using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Lógica de turnos da batalha. Este script NÃO cria nenhuma interface —
/// ele espera que você já tenha montado os elementos no Editor e apenas
/// arraste as referências nos campos abaixo, no Inspector.
///
/// COMO USAR:
/// 1. Crie um GameObject vazio na cena Battle, chame de "BattleManager".
/// 2. Arraste este script para ele.
/// 3. No Inspector, arraste cada elemento de UI que você já criou para o
///    campo correspondente (Slider Player, Slider Inimigo, Texto Mensagem,
///    Botao Atacar, Botao Item, Botao Fugir).
/// 4. Dê Play e clique nos botões.
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Referências de UI (arraste os objetos da Hierarchy aqui)")]
    public Slider sliderPlayer;
    public Slider sliderInimigo;
    public Text textoMensagem;
    public Button botaoAtacar;
    public Button botaoItem;
    public Button botaoFugir;

    [Header("Status do Player")]
    public string nomePlayer = "Herói";
    public int hpMaxPlayer = 30;
    public int atkPlayer = 8;
    public int defPlayer = 2;

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

    private int hpAtualPlayer;
    private int hpAtualInimigo;
    private bool turnoDoJogador = true;
    private bool batalhaEncerrada = false;

    void Start()
    {
        hpAtualPlayer = hpMaxPlayer;
        hpAtualInimigo = hpMaxInimigo;

        // Conecta cada botão à sua função correspondente
        botaoAtacar.onClick.AddListener(AoClicarAtacar);
        botaoItem.onClick.AddListener(AoClicarItem);
        botaoFugir.onClick.AddListener(AoClicarFugir);

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

        int dano = Mathf.Max(1, atkPlayer - defInimigo);
        hpAtualInimigo = Mathf.Max(0, hpAtualInimigo - dano);
        AtualizarBarras();
        MostrarMensagem($"{nomePlayer} atacou! {nomeInimigo} perdeu {dano} de HP.");

        if (hpAtualInimigo <= 0)
            StartCoroutine(FinalizarBatalha(true));
        else
            StartCoroutine(TurnoDoInimigo());
    }

    void AoClicarItem()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        int cura = 5;
        hpAtualPlayer = Mathf.Min(hpMaxPlayer, hpAtualPlayer + cura);
        AtualizarBarras();
        MostrarMensagem($"{nomePlayer} usou um item e recuperou {cura} de HP!");

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

        int dano = Mathf.Max(1, atkInimigo - defPlayer);
        hpAtualPlayer = Mathf.Max(0, hpAtualPlayer - dano);
        AtualizarBarras();
        MostrarMensagem($"{nomeInimigo} atacou! {nomePlayer} perdeu {dano} de HP.");
        yield return new WaitForSeconds(1f);

        if (hpAtualPlayer <= 0)
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
        sliderPlayer.value = (float)hpAtualPlayer / hpMaxPlayer;
        sliderInimigo.value = (float)hpAtualInimigo / hpMaxInimigo;
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