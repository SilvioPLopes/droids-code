using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Anexar no MESMO GameObject do Player, na cena Game. Nao mexe em
/// PlayerMovement.cs de proposito — e um componente a mais, nao uma edicao
/// do que ja funciona.
///
/// Ao carregar a cena, se o GerenciadorDeEstado tiver uma posicao salva
/// PARA ESTA cena especificamente, teleporta o player pra la e limpa a
/// posicao salva (pra nao teleportar de novo achando errado da proxima vez
/// que a cena Game carregar por outro motivo).
/// </summary>
public class RestaurarPosicao : MonoBehaviour
{
    void Start()
    {
        GerenciadorDeEstado gerenciador = GerenciadorDeEstado.Instancia;
        string cenaAtual = SceneManager.GetActiveScene().name;

        if (gerenciador.TemPosicaoSalva && gerenciador.CenaDeOrigemDaPosicao == cenaAtual)
        {
            transform.position = gerenciador.PosicaoSalva;
            gerenciador.LimparPosicaoSalva();
        }
    }
}
