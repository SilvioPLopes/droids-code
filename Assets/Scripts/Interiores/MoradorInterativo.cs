using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Input único dos moradores: narrativa existente, conversa e missões do catálogo.</summary>
[DisallowMultipleComponent]
public class MoradorInterativo : MonoBehaviour
{
    public string idNpc;
    public string nome;
    [TextArea] public string saudacao = "Entre. Podemos conversar um pouco.";
    public GatilhoDeDialogo[] historia = Array.Empty<GatilhoDeDialogo>();
    public bool negociar;
    public string flagParaMissoes;
    public string destinoDeVisita;
    public Vector2 chegadaDaVisita;
    public KeyCode tecla = KeyCode.E;
    private readonly HashSet<Collider2D> jogadores = new HashSet<Collider2D>();
    private int frameFechamento = -1;

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) jogadores.Add(other); }
    void OnTriggerExit2D(Collider2D other) { jogadores.Remove(other); }
    void OnDisable() { jogadores.Clear(); }
    void Update()
    {
        jogadores.RemoveWhere(p => p == null);
        if (jogadores.Count == 0 || !Input.GetKeyDown(tecla)) return;
        // Nas casas compartilhadas os raios se sobrepoem: somente o morador mais proximo recebe E.
        var jogador = jogadores.FirstOrDefault();
        if (jogador == null) return;
        var centro = jogador.bounds.center;
        var escolhido = FindObjectsByType<MoradorInterativo>(FindObjectsSortMode.None)
            .Where(n => n.isActiveAndEnabled && n.jogadores.Any(c => c != null))
            .OrderBy(n => (n.CentroVisual() - centro).sqrMagnitude)
            .ThenBy(n => n.idNpc, StringComparer.Ordinal)
            .FirstOrDefault();
        if (escolhido == this) Interagir();
    }

    Vector3 CentroVisual()
    {
        var sr = GetComponent<SpriteRenderer>();
        return sr != null ? sr.bounds.center : transform.position;
    }

    public void Interagir()
    {
        if (frameFechamento == Time.frameCount || GerenciadorDeEstado.Instancia.MenuAberto) return;
        var painel = DialogoUIManager.Instancia;
        if (painel == null) return;
        var jogador = GameObject.FindGameObjectWithTag("Player");
        GetComponent<OrientacaoNpc>()?.OlharPara(jogador != null ? jogador.transform : null);
        foreach (var gatilho in historia ?? Array.Empty<GatilhoDeDialogo>())
        {
            if (gatilho == null || !gatilho.enabled || !gatilho.PodeDisparar()) continue;
            bool missoesLiberadas = string.IsNullOrWhiteSpace(flagParaMissoes) || GerenciadorDeEstado.Instancia.ObterFlag(flagParaMissoes);
            if (!gatilho.apenasUmaVez && string.IsNullOrWhiteSpace(gatilho.flagAoTerminar) && missoesLiberadas && MissoesDe(idNpc).Any(q => GerenciadorDeQuests.EstaAtiva(q.Id) || GerenciadorDeQuests.PodeSerOferecida(q.Id))) continue;
            var anterior = gatilho.AoConcluirDialogoExterno;
            gatilho.AoConcluirDialogoExterno = () =>
            {
                gatilho.AoConcluirDialogoExterno = anterior;
                anterior?.Invoke();
                ConcluiuConversa();
            };
            gatilho.Disparar();
            if (!painel.EstaAberto) gatilho.AoConcluirDialogoExterno = anterior;
            return;
        }
        MostrarOpcoes(painel);
    }

    void ConcluiuConversa()
    {
        frameFechamento = Time.frameCount;
        if (string.IsNullOrWhiteSpace(idNpc) || CatalogoDeNpcs.Obter(idNpc) == null) return;
        GerenciadorDeQuests.AtualizarQuestsAutomaticas();
        GerenciadorDeQuests.RegistrarProgresso(TipoDeObjetivo.Conversar, idNpc);
    }

    public static List<DefinicaoDeQuest> MissoesDe(string npc)
    {
        return CatalogoDeQuests.Todos.Values.Where(q => q.NpcQueOferece == npc).ToList();
    }

    void MostrarOpcoes(DialogoUIManager painel)
    {
        var escolhas = new List<EscolhaDeDialogo>();
        var acoes = new Dictionary<EscolhaDeDialogo, Action>();
        void Opcao(string titulo, Action acao)
        {
            var escolha = new EscolhaDeDialogo { texto = titulo, falanteDaResposta = nome };
            escolhas.Add(escolha); acoes.Add(escolha, acao);
        }
        bool liberado = string.IsNullOrWhiteSpace(flagParaMissoes) || GerenciadorDeEstado.Instancia.ObterFlag(flagParaMissoes);
        if (liberado)
        {
            foreach (var q in MissoesDe(idNpc))
            {
                var quest = q;
                if (GerenciadorDeQuests.PodeEntregar(q.Id))
                    Opcao("Entregar: " + q.Titulo, () => MostrarEntrega(quest));
                else if (GerenciadorDeQuests.EstaAtiva(q.Id))
                    Opcao("Missão: " + q.Titulo, () => MostrarAndamento(quest));
                else if (!q.AceitaAutomaticamente && GerenciadorDeQuests.PodeSerOferecida(q.Id))
                    Opcao("Ajuda: " + q.Titulo, () => MostrarOferta(quest));
            }
        }
        if (negociar) Opcao("Ver mercadorias", AbrirLoja);
        if (!string.IsNullOrWhiteSpace(destinoDeVisita)) Opcao("Conhecer a oficina anexa", () =>
        {
            GerenciadorDeEstado.Instancia.SalvarPosicao(chegadaDaVisita, destinoDeVisita);
            UnityEngine.SceneManagement.SceneManager.LoadScene(destinoDeVisita);
        });
        Opcao("Conversar", () =>
        {
            string texto = saudacao;
            var def = CatalogoDeNpcs.Obter(idNpc);
            if (def != null && !string.IsNullOrWhiteSpace(def.TextoDeBoasVindas)) texto = def.TextoDeBoasVindas;
            painel.Mostrar(Fala(texto), ConcluiuConversa);
        });
        Opcao("Até mais", () => { frameFechamento = Time.frameCount; });
        painel.Mostrar(Fala("O que você precisa?"), escolhas, escolha =>
        {
            frameFechamento = Time.frameCount;
            if (escolha != null && acoes.TryGetValue(escolha, out var acao)) acao();
        });
    }

    List<FalaDeDialogo> Fala(string texto) => new List<FalaDeDialogo> { new FalaDeDialogo { falante = nome, texto = texto } };

    void MostrarOferta(DefinicaoDeQuest q)
    {
        var aceitar = new EscolhaDeDialogo { texto = "Vou ajudar." };
        var voltar = new EscolhaDeDialogo { texto = "Agora não." };
        DialogoUIManager.Instancia?.Mostrar(Fala(q.TextoDeOferta), new[] { aceitar, voltar }, escolha =>
        {
            frameFechamento = Time.frameCount;
            if (escolha == aceitar && GerenciadorDeQuests.Aceitar(q.Id)) ConcluiuConversa();
        });
    }

    void MostrarEntrega(DefinicaoDeQuest q)
    {
        // Reavaliar no callback: só o gerenciador pode consumir itens e pagar recompensas.
        var entregar = new EscolhaDeDialogo { texto = "Concluir entrega" };
        var voltar = new EscolhaDeDialogo { texto = "Voltar" };
        DialogoUIManager.Instancia?.Mostrar(Fala(q.Titulo + "\n" + GerenciadorDeQuests.DescreverRecompensas(q)), new[] { entregar, voltar }, escolha =>
        {
            frameFechamento = Time.frameCount;
            if (escolha == entregar && GerenciadorDeQuests.Entregar(q.Id))
                DialogoUIManager.Instancia?.Mostrar(Fala(q.TextoDeEntrega), () => { frameFechamento = Time.frameCount; });
        });
    }

    void MostrarAndamento(DefinicaoDeQuest q)
    {
        string texto = q.Resumo + "\n" + string.Join("\n", q.Objetivos.Select((o, i) => GerenciadorDeQuests.DescreverObjetivo(q, i)));
        DialogoUIManager.Instancia?.Mostrar(Fala(texto), () => { frameFechamento = Time.frameCount; });
    }

    void AbrirLoja()
    {
        var loja = FindAnyObjectByType<LojaUIManager>(FindObjectsInactive.Include);
        if (loja == null) return;
        GerenciadorDeEstado.Instancia.RegistrarMenuAberto();
        loja.AoFechar = () => { GerenciadorDeEstado.Instancia.RegistrarMenuFechado(); frameFechamento = Time.frameCount; };
        loja.Mostrar();
    }
}
