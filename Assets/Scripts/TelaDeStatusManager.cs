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
    public Button botaoVoltar;

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

        painel.SetActive(true);
    }

    public void Fechar()
    {
        painel.SetActive(false);
    }
}
