using UnityEngine;

/// <summary>
/// Liga/desliga GameObjects conforme o estado das flags de historia.
/// E o que faz Rasha e Teodoro so existirem depois da Sessao 3, o drone
/// aparecer no ceu de Ferrovale so na Sessao 3, e a Dara trocar de dialogo
/// no retorno (Sessao 7) — tudo sem script proprio por personagem.
///
/// IMPORTANTE (armadilha de Unity): este componente NAO deve ficar no
/// proprio objeto que ele desliga. Um GameObject desativado nao roda
/// Update() nem Start(), entao ele nunca conseguiria se religar quando a
/// flag mudasse. Por isso o alvo e uma LISTA de objetos externos, e este
/// componente mora num objeto que fica sempre ativo (ex: um GameObject
/// vazio chamado "ControleDeHistoria" na raiz da cena).
/// </summary>
public class CondicaoDeHistoria : MonoBehaviour
{
    [System.Serializable]
    public class Regra
    {
        [Tooltip("Apelido pra voce se achar no Inspector. Nao afeta nada.")]
        public string descricao = "";

        [Tooltip("Objetos ligados quando a condicao e satisfeita, e desligados quando nao e.")]
        public GameObject[] alvos;

        [Tooltip("TODAS precisam estar ativas. Vazio = sem exigencia.")]
        public string[] flagsNecessarias;

        [Tooltip("NENHUMA pode estar ativa.")]
        public string[] flagsQueImpedem;

        [Tooltip("Inverte o resultado: liga quando a condicao NAO e satisfeita.")]
        public bool inverter = false;
    }

    [Header("Regras")]
    public Regra[] regras;

    [Header("Quando reavaliar")]
    [Tooltip("Se marcado, reavalia todo frame. Deixe ligado em cenas onde uma flag pode mudar sem troca de cena (ex: City, onde um dialogo grava flag e outro objeto precisa reagir na hora).")]
    public bool reavaliarContinuamente = true;

    [Tooltip("Intervalo em segundos entre reavaliacoes, quando 'reavaliar continuamente' esta ligado. 0 = todo frame.")]
    public float intervaloDeReavaliacao = 0.2f;

    private float _tempoAteProximaChecagem;

    void Start()
    {
        Aplicar();
    }

    void Update()
    {
        if (!reavaliarContinuamente) return;

        if (intervaloDeReavaliacao > 0f)
        {
            _tempoAteProximaChecagem -= Time.deltaTime;
            if (_tempoAteProximaChecagem > 0f) return;
            _tempoAteProximaChecagem = intervaloDeReavaliacao;
        }

        Aplicar();
    }

    /// <summary>Publico pra permitir forcar reavaliacao depois de um evento.</summary>
    public void Aplicar()
    {
        if (regras == null) return;

        foreach (Regra regra in regras)
        {
            if (regra == null || regra.alvos == null) continue;

            bool satisfeita = CondicoesDeHistoria.Satisfeitas(regra.flagsNecessarias, regra.flagsQueImpedem);
            bool ligar = regra.inverter ? !satisfeita : satisfeita;

            foreach (GameObject alvo in regra.alvos)
            {
                if (alvo == null) continue;
                if (alvo == gameObject)
                {
                    // Protecao contra o erro descrito no cabecalho: desligar
                    // a si mesmo mataria o Update que religaria depois.
                    Debug.LogWarning($"CondicaoDeHistoria em '{gameObject.name}': o alvo e o proprio objeto do componente. Isso o impediria de reavaliar depois. Alvo ignorado.");
                    continue;
                }
                if (alvo.activeSelf != ligar) alvo.SetActive(ligar);
            }
        }
    }
}
