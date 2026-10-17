using System.Collections;
using UnityEngine;

namespace DroidsCode.Casa
{
    /// <summary>Coordena a UI da casa sem carregar suas regras para outras cenas.</summary>
    [RequireComponent(typeof(VerificadorDePuzzle))]
    public class ControladorPuzzleAguaCasa : MonoBehaviour
    {
        public VerificadorDePuzzle verificador;

        private const string IdDoPuzzle = "p_agua_casa";
        private VerificadorDePuzzle _verificadorInscrito;
        private TerminalUIManager _terminal;
        private Coroutine _fluxoDeSucesso;

        void OnEnable()
        {
            if (verificador == null) verificador = GetComponent<VerificadorDePuzzle>();
            if (verificador != null)
            {
                _verificadorInscrito = verificador;
                _verificadorInscrito.AoResolverPelaPrimeiraVez += AoResolver;
            }
            AtualizarTerminal();
        }

        void Update() { AtualizarTerminal(); }

        private void AtualizarTerminal()
        {
            if (verificador == null) return;
            if (verificador.terminal == null)
                verificador.AssociarTerminal(LocalizadorDeTerminal.Encontrar());

            var atual = verificador.terminal;
            if (ReferenceEquals(_terminal, atual)) return;
            if (_terminal != null) _terminal.LimparContextoPuzzle(IdDoPuzzle);
            _terminal = atual;
            if (_terminal != null) _terminal.DefinirContextoPuzzle(IdDoPuzzle);
        }

        private void AoResolver()
        {
            if (_fluxoDeSucesso == null)
                _fluxoDeSucesso = StartCoroutine(ApresentarSucesso());
        }

        private IEnumerator ApresentarSucesso()
        {
            AtualizarTerminal();
            if (_terminal != null)
            {
                // O menu esquece primeiro a origem da abertura; Fechar não
                // deve fazê-lo reaparecer por trás da conversa.
                foreach (var menu in _terminal.transform.root.GetComponentsInChildren<MenuMundoManager>(true))
                    menu.FecharParaDialogo();
                _terminal.Fechar();
            }

            var falas = CatalogoDeDialogos.Obter(CatalogoDeDialogos.CasaPortaApollo);
            while (isActiveAndEnabled)
            {
                var dialogo = DialogoUIManager.Instancia;
                if (dialogo != null && !dialogo.EstaAberto && dialogo.painel != null &&
                    dialogo.textoFala != null && dialogo.Mostrar(falas))
                    break;

                // Não substituir callbacks de uma conversa em andamento.
                // O controlador é local; OnDisable cancela esta espera.
                yield return null;
            }
            _fluxoDeSucesso = null;
        }

        void OnDisable()
        {
            if (!ReferenceEquals(_verificadorInscrito, null))
                _verificadorInscrito.AoResolverPelaPrimeiraVez -= AoResolver;
            _verificadorInscrito = null;
            if (_fluxoDeSucesso != null) StopCoroutine(_fluxoDeSucesso);
            _fluxoDeSucesso = null;
            if (_terminal != null) _terminal.LimparContextoPuzzle(IdDoPuzzle);
            _terminal = null;
        }
    }
}
