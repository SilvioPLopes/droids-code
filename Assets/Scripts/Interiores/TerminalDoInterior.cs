using UnityEngine;

/// <summary>Reaproveita o terminal e verificador existentes num ponto do interior.</summary>
public class TerminalDoInterior : MonoBehaviour
{
    public string idPuzzle;
    public string flagNecessaria;
    public VerificadorDePuzzle verificador;
    private bool perto;
    private TerminalUIManager terminal;
    private static VerificadorDePuzzle verificadorAtivo;
    private int frameFechamento = -1;
    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) perto = true; }
    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) perto = false; }
    void Update()
    {
        if (frameFechamento == Time.frameCount || !perto || !Input.GetKeyDown(KeyCode.E) || GerenciadorDeEstado.Instancia.MenuAberto) return;
        if (!string.IsNullOrWhiteSpace(flagNecessaria) && !GerenciadorDeEstado.Instancia.ObterFlag(flagNecessaria))
        {
            DialogoUIManager.Instancia?.Mostrar(new[] { new FalaDeDialogo { falante = "", texto = "Antes de usar este equipamento, preciso conversar com a pessoa responsável." } }, () => { frameFechamento = Time.frameCount; });
            return;
        }
        terminal = FindAnyObjectByType<TerminalUIManager>(FindObjectsInactive.Include);
        if (terminal == null) return;
        terminal.DefinirContextoPuzzle(idPuzzle);
        if (verificadorAtivo != null && verificadorAtivo != verificador) verificadorAtivo.enabled = false;
        if (verificador != null)
        {
            verificador.enabled = true;
            verificador.AssociarTerminal(terminal);
            verificadorAtivo = verificador;
        }
        terminal.Abrir();
    }
    void OnDisable()
    {
        if (terminal != null) terminal.LimparContextoPuzzle(idPuzzle);
        if (verificador != null) verificador.enabled = false;
        if (verificadorAtivo == verificador) verificadorAtivo = null;
        perto = false;
    }
}
