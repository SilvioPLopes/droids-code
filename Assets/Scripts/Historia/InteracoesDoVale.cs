using UnityEngine;

/// <summary>Apresenta os encontros do vale e entrega o E ao ponto disponível mais próximo.</summary>
[DefaultExecutionOrder(-30)]
public class InteracoesDoVale : MonoBehaviour
{
    // Os encontros de história vêm antes das caçadas repetíveis na lista da cena.
    public MoradorInterativo[] encontros;
    public MoradorInterativo apolloDaEntrada;
    public MoradorInterativo sucataDaBorda;
    public GatilhoDeBatalha segundaLuta;
    private GatilhoDeBatalha cacaDaBorda;
    private CircleCollider2D corpoJogador;
    private CircleCollider2D[] raios;
    private bool[] estavaFora;
    private MoradorInterativo[] moradores;
    private TerminalDoInterior[] terminais;
    private GameObject[] avisosTerminal;
    private Transform jogador;
    private GameObject avisoAtual;

    void Awake()
    {
        moradores = GetComponentsInChildren<MoradorInterativo>(true);
        terminais = GetComponentsInChildren<TerminalDoInterior>(true);
        avisosTerminal = new GameObject[terminais.Length];
        foreach (var morador in moradores)
        {
            morador.entradaPeloVale = true;
            morador.indicadorDeProximidade?.SetActive(false);
        }
        for (int i = 0; i < terminais.Length; i++)
        {
            terminais[i].entradaPeloVale = true;
            avisosTerminal[i] = terminais[i].transform.Find("Interação").gameObject;
            avisosTerminal[i].SetActive(false);
        }
        jogador = GameObject.FindGameObjectWithTag("Player")?.transform;
        corpoJogador = jogador?.GetComponent<CircleCollider2D>();
        cacaDaBorda = sucataDaBorda.batalha;
        raios = System.Array.ConvertAll(encontros, e => e.GetComponent<CircleCollider2D>());
        estavaFora = new bool[encontros.Length];
    }

    public void AtualizarEncontros()
    {
        var estado = GerenciadorDeEstado.Instancia;
        bool passouPelaEntrada = estado.ObterFlag("ato1_apos_primeiro_combate") ||
            estado.ObterFlag("ato1_faccao_escolhida") || estado.ObterFlag("ato1_modulo_recebido");
        bool mostrarApollo = !passouPelaEntrada &&
            System.Array.Exists(apolloDaEntrada.historia, fala => fala.PodeDisparar());
        if (apolloDaEntrada.gameObject.activeSelf != mostrarApollo)
            apolloDaEntrada.gameObject.SetActive(mostrarApollo);
        sucataDaBorda.batalha = passouPelaEntrada || estado.ObterFlag(segundaLuta.flagDeVitoria)
            ? cacaDaBorda : segundaLuta;
        for (int i = 0; i < encontros.Length; i++)
        {
            var ponto = encontros[i];
            bool mostrar = ponto.batalha.PodeDisparar();
            for (int j = 0; mostrar && j < i; j++)
                if (encontros[j].gameObject.activeSelf &&
                    encontros[j].batalha.idDoInimigo == ponto.batalha.idDoInimigo)
                    mostrar = false;
            if (ponto.gameObject.activeSelf != mostrar)
            {
                estavaFora[i] = false;
                ponto.gameObject.SetActive(mostrar);
            }
        }
    }

    // Na volta de Battle, o jogador pode nascer dentro do mesmo encontro.
    // Só arma depois de observar o corpo fora do círculo desta aparição.
    bool IniciarAoAproximar()
    {
        if (corpoJogador == null) return false;
        Vector2 centro = corpoJogador.transform.TransformPoint(corpoJogador.offset);
        for (int i = 0; i < encontros.Length; i++)
        {
            if (!encontros[i].gameObject.activeInHierarchy) continue;
            var area = raios[i];
            Vector2 alvo = area.transform.TransformPoint(area.offset);
            float alcance = RaioNoMundo(area) + RaioNoMundo(corpoJogador);
            if ((centro - alvo).sqrMagnitude > alcance * alcance)
            {
                estavaFora[i] = true;
                continue;
            }
            if (!estavaFora[i]) continue;
            estavaFora[i] = false;
            encontros[i].batalha.Disparar();
            return true;
        }
        return false;
    }

    static float RaioNoMundo(CircleCollider2D circulo)
    {
        var escala = circulo.transform.lossyScale;
        return circulo.radius * Mathf.Max(Mathf.Abs(escala.x), Mathf.Abs(escala.y));
    }

    void Update()
    {
        if (GerenciadorDeEstado.Instancia.MenuAberto || jogador == null)
        {
            MostrarAviso(null);
            return;
        }
        AtualizarEncontros();
        if (IniciarAoAproximar()) return;
        MoradorInterativo ator = null;
        TerminalDoInterior terminal = null;
        GameObject aviso = null;
        float menor = float.PositiveInfinity;
        foreach (var m in moradores)
        {
            if (!m.isActiveAndEnabled || !m.JogadorPerto) continue;
            if (System.Array.IndexOf(encontros, m) >= 0) continue;
            if (string.IsNullOrEmpty(m.idNpc) && m.batalha == null &&
                !System.Array.Exists(m.historia, g => g != null && g.enabled && g.PodeDisparar())) continue;
            float distancia = (m.transform.position - jogador.position).sqrMagnitude;
            if (distancia >= menor) continue;
            menor = distancia;
            ator = m;
            aviso = m.indicadorDeProximidade;
        }
        for (int i = 0; i < terminais.Length; i++)
        {
            var t = terminais[i];
            if (!t.isActiveAndEnabled || !t.JogadorPerto ||
                GerenciadorDeEstado.Instancia.ObterFlag(t.verificador.flagQueJaResolve)) continue;
            float distancia = (t.transform.position - jogador.position).sqrMagnitude;
            if (distancia >= menor) continue;
            menor = distancia;
            terminal = t;
            aviso = avisosTerminal[i];
        }
        MostrarAviso(aviso);
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (terminal != null) terminal.TentarInteragir();
        else ator?.Interagir();
    }

    void MostrarAviso(GameObject aviso)
    {
        if (avisoAtual != aviso) avisoAtual?.SetActive(false);
        if (aviso != null && !aviso.activeSelf) aviso.SetActive(true);
        avisoAtual = aviso;
    }
}
