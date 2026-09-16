using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla a navegação do Menu Principal: abrir o jogo, mostrar
/// configurações e sair. Arraste este script para um GameObject vazio
/// na cena MainMenu e conecte os campos abaixo no Inspector.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis (arraste os objetos da Hierarchy aqui)")]
    public GameObject painelPrincipal;
    public GameObject painelConfiguracoes;

    [Header("Configuração de cena")]
    [Tooltip("Nome exato da cena de gameplay, igual está em Build Settings.")]
    public string nomeCenaJogo = "Game";

    // Chamado pelo botão "Novo Jogo"
    public void NovoJogo()
    {
        SceneManager.LoadScene(nomeCenaJogo);
    }

    // Chamado pelo botão "Configurações"
    public void AbrirConfiguracoes()
    {
        painelPrincipal.SetActive(false);
        painelConfiguracoes.SetActive(true);
    }

    // Chamado pelo botão "Voltar" dentro do painel de configurações
    public void FecharConfiguracoes()
    {
        painelConfiguracoes.SetActive(false);
        painelPrincipal.SetActive(true);
    }

    // Chamado pelo Slider de volume (evento "On Value Changed")
    public void MudarVolume(float valor)
    {
        AudioListener.volume = valor;
    }

    // Chamado pelo botão "Sair"
    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        // Application.Quit() não funciona dentro do Editor,
        // então isso simula a saída ao testar com o Play.
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
