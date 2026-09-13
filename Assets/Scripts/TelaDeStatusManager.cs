using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DroidsCode.DroidCore;

/// <summary>
/// Painel de status do Droid. Nao recebe o Droid por parametro — busca
/// direto do GerenciadorDeEstado toda vez que é aberto. Isso permite usar
/// o MESMO script tanto no botão Status de dentro da batalha quanto, mais
/// pra frente, no menu fora de batalha, sem duplicar nada.
///
/// CORRECAO (13/09/2026 — exclusividade de painéis): ganhou o callback
/// opcional AoFechar, invocado em Fechar(). O MenuMundoManager usa isso
/// pra saber quando reexibir o painel do Menu do Mundo depois que o
/// Status é fechado. Na Batalha (BattleManager), AoFechar simplesmente
/// fica null e não é chamado — nenhuma mudança de comportamento lá.
/// </summary>
public class TelaDeStatusManager : MonoBehaviour
{
    [Header("Referências de UI (arraste os objetos da Hierarchy aqui)")]
    public GameObject painel;
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoHp;
    public TextMeshProUGUI textoAtributos;
    public TextMeshProUGUI textoTecnicas;
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoExp;
    public Button botaoVoltar;

    // NOVO (13/09/2026): callback opcional, atribuído externamente (ver
    // MenuMundoManager.Start()). Invocado ao final de Fechar().
    public System.Action AoFechar;

    void Start()
    {
        if (botaoVoltar != null)
        {
            botaoVoltar.onClick.AddListener(Fechar);
        }

        if (painel != null)
        {
            painel.SetActive(false);
        }
    }

    public void Mostrar()
    {
        Droid droid = GerenciadorDeEstado.Instancia.DroidDoJogador;

        textoNome.text = droid.Nome;
        textoHp.text = $"HP: {droid.Hp}/{droid.HpMax}";

        textoAtributos.text =
            $"FOR: {droid.ObterTotal(TipoAtributo.For)}\n" +
            $"AGI: {droid.ObterTotal(TipoAtributo.Agi)}\n" +
            $"VIT: {droid.ObterTotal(TipoAtributo.Vit)}\n" +
            $"INT: {droid.ObterTotal(TipoAtributo.Int)}\n" +
            $"DEX: {droid.ObterTotal(TipoAtributo.Dex)}\n" +
            $"LUK: {droid.ObterTotal(TipoAtributo.Luk)}";

        var tecnicas = new StringBuilder();
        foreach (string nomeAcao in droid.ObterAcoesDisponiveis())
        {
            tecnicas.AppendLine(nomeAcao);
        }
        textoTecnicas.text = tecnicas.Length > 0 ? tecnicas.ToString() : "Nenhuma";

        textoPontos.text = $"Pontos disponíveis: {droid.Pontos.PontosDisponiveis}";

        // Campo opcional: se você ainda não criou o TextMeshProUGUI de XP no
        // Inspector, isso simplesmente não mostra nada, sem quebrar o resto.
        if (textoExp != null)
        {
            textoExp.text =
                $"Nível: {droid.Progressao.Nivel}\n" +
                $"XP: {droid.Progressao.ExperienciaAtual}/{droid.Progressao.ExperienciaNecessariaProximoNivel}";
        }

        painel.SetActive(true);
    }

    public void Fechar()
    {
        painel.SetActive(false);

        // NOVO (13/09/2026): avisa quem estiver ouvindo (ex: MenuMundoManager)
        // que o painel de Status foi fechado. Seguro mesmo se ninguém tiver
        // se inscrito (ex: uso dentro da Batalha).
        AoFechar?.Invoke();
    }
}
