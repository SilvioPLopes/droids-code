using UnityEngine;

namespace DroidsCode.Casa
{
    /// <summary>
    /// Trigger simples de interação com o terminal da casa: o player entra
    /// na área do objeto, aperta a tecla de interação, o painel do terminal
    /// abre (TerminalUIManager.Abrir() -- o mesmo painel/terminal geral do
    /// jogo, reaproveitado aqui pra cena da casa).
    ///
    /// NÃO depende de NpcInterativo (arquivo não visto nesta sessão) -- se o
    /// projeto já tem um padrão de interação genérico usado pelos NPCs da
    /// Cidade, este script pode ser substituído por ele sem tocar em
    /// TerminalCasaManager (a única exigência é chamar Abrir() em algum
    /// momento). Ficou como trigger + tecla própria de propósito, pra não
    /// travar esta entrega numa dependência que ainda não temos em mãos.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteragirComTerminalCasa : MonoBehaviour
    {
        [SerializeField] private TerminalUIManager terminal;
        [SerializeField] private KeyCode teclaDeInteracao = KeyCode.E;

        [Tooltip("Tag usada pelo GameObject do Player na cena. Confirmar se já é 'Player' no projeto.")]
        [SerializeField] private string tagDoPlayer = "Player";

        private bool _playerPorPerto;

        // Garante que o Collider2D adicionado por [RequireComponent] já
        // nasce como trigger, sem precisar lembrar de marcar no Inspector.
        void Reset()
        {
            var colisor = GetComponent<Collider2D>();
            if (colisor != null) colisor.isTrigger = true;
        }

        void Update()
        {
            if (_playerPorPerto && terminal != null && Input.GetKeyDown(teclaDeInteracao))
            {
                terminal.Abrir();
            }
        }

        void OnTriggerEnter2D(Collider2D outro)
        {
            if (outro.CompareTag(tagDoPlayer)) _playerPorPerto = true;
        }

        void OnTriggerExit2D(Collider2D outro)
        {
            if (outro.CompareTag(tagDoPlayer)) _playerPorPerto = false;
        }
    }
}
