using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DroidsCode.DroidCore;

/// <summary>
/// Painel do Droid — mostra as 4 peças equipadas (Braço/Perna/Tronco/Cabeça)
/// em modo somente-leitura. Nao recebe o Droid por parametro — busca direto
/// do GerenciadorDeEstado toda vez que é aberto, mesmo padrão de
/// TelaDeStatusManager (permite reusar o script sem duplicar nada).
///
/// Decisões fechadas para esta leva (ver PLANO_PAINEL_DROID.md, seção 3):
/// - Só visualização (Opção A). Equipar continua acontecendo pela Bag
///   (BagUIManager.EquiparItem) — este painel nunca escreve em Droid.
/// - Sem "desequipar" nesta leva.
/// - Slot vazio sempre aparece, com o texto "Vazio" (nunca esconde o slot).
///
/// Segue o mesmo contrato de painel + callback AoFechar já usado por
/// TelaDeStatusManager/TerminalUIManager/BagUIManager (usado pelo
/// MenuMundoManager pra reexibir seu próprio painel quando este fecha).
/// </summary>
public class TelaDeDroidManager : MonoBehaviour
{
    [Header("Referências de UI (arraste os objetos da Hierarchy aqui)")]
    public GameObject painel;
    public TextMeshProUGUI textoBraco;
    public TextMeshProUGUI textoPerna;
    public TextMeshProUGUI textoTronco;
    public TextMeshProUGUI textoCabeca;
    public Button botaoVoltar;

    // Mesmo padrão de TelaDeStatusManager/TerminalUIManager: callback
    // opcional, atribuído externamente pelo MenuMundoManager em Start().
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

        textoBraco.text = DescreverPeca(droid.Braco, "Braço");
        textoPerna.text = DescreverPeca(droid.Perna, "Perna");
        textoTronco.text = DescreverPeca(droid.Tronco, "Tronco");
        textoCabeca.text = DescreverPeca(droid.Cabeca, "Cabeça");

        painel.SetActive(true);
    }

    // Slot vazio (peca == null) sempre mostra "Vazio" -- decisao fechada,
    // nao esconder o slot. Peca sem bonus definido (AtributoBonificado ==
    // null, ex: peca cosmetica/pre-Estagio 3) mostra so o nome, sem bonus.
    private string DescreverPeca(DroidPart peca, string nomeDoSlot)
    {
        if (peca == null)
        {
            return $"{nomeDoSlot}: Vazio";
        }

        if (peca.AtributoBonificado == null)
        {
            return $"{nomeDoSlot}: {peca.Nome}";
        }

        return $"{nomeDoSlot}: {peca.Nome} (+{peca.ValorDoBonus} {peca.AtributoBonificado})";
    }

    public void Fechar()
    {
        painel.SetActive(false);

        // Mesmo padrão de TelaDeStatusManager: avisa quem estiver ouvindo
        // (MenuMundoManager) que o painel foi fechado. Seguro mesmo se
        // ninguém tiver se inscrito.
        AoFechar?.Invoke();
    }
}
