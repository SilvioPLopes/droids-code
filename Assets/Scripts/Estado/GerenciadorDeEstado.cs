using System.Collections.Generic;
using UnityEngine;
using DroidsCode.DroidCore;

/// <summary>
/// Guarda o estado que precisa sobreviver a troca de cena (Game <-> Battle):
/// o Droid do jogador (mesma instancia, nunca recriada), a posicao onde ele
/// estava no mapa antes de entrar em batalha, e agora flags de historia
/// abertas para o sistema de salvamento (ver SalvamentoJson.cs).
///
/// Padrao Singleton COM criacao preguicosa (lazy): se voce der Play direto
/// na cena Battle (sem passar por MainMenu/Game antes), a primeira chamada
/// a Instancia cria um Droid novo com valores padrao — nao quebra o teste
/// isolado de uma cena.
/// </summary>
public class GerenciadorDeEstado : MonoBehaviour
{
    private static GerenciadorDeEstado instancia;

    public static GerenciadorDeEstado Instancia
    {
        get
        {
            if (instancia == null)
            {
                var objeto = new GameObject("GerenciadorDeEstado");
                instancia = objeto.AddComponent<GerenciadorDeEstado>();
                DontDestroyOnLoad(objeto);
            }
            return instancia;
        }
    }

    public Droid DroidDoJogador { get; private set; }

    public bool TemPosicaoSalva { get; private set; }
    public Vector3 PosicaoSalva { get; private set; }
    public string CenaDeOrigemDaPosicao { get; private set; }

    private int _contadorMenusAbertos;
    public bool MenuAberto => _contadorMenusAbertos > 0;

    public void RegistrarMenuAberto() => _contadorMenusAbertos++;
    public void RegistrarMenuFechado() => _contadorMenusAbertos = Mathf.Max(0, _contadorMenusAbertos - 1);

    // Correcao do bug "personagem trava apos salvar/carregar": o
    // SceneManager.LoadScene chamado por SalvamentoJson.Carregar() destroi o
    // MenuMundoManager da cena antiga sem passar por RegistrarMenuFechado(),
    // entao o contador precisa ser zerado explicitamente antes da troca de
    // cena (ver SalvamentoJson.Carregar()).
    public void ZerarMenusAbertos() => _contadorMenusAbertos = 0;

    // Flags de historia: chave livre (ex: "derrotou_chefe_1"), aberto pra
    // uso futuro. Nao ha nenhuma flag definida ainda -- so a estrutura.
    private readonly Dictionary<string, bool> _flagsDeHistoria = new Dictionary<string, bool>();

    public bool ObterFlag(string chave) =>
        _flagsDeHistoria.TryGetValue(chave, out bool valor) && valor;

    public void DefinirFlag(string chave, bool valor) => _flagsDeHistoria[chave] = valor;

    public IReadOnlyDictionary<string, bool> TodasAsFlags => _flagsDeHistoria;

    public void CarregarFlags(Dictionary<string, bool> flags)
    {
        _flagsDeHistoria.Clear();
        foreach (var kv in flags)
        {
            _flagsDeHistoria[kv.Key] = kv.Value;
        }
    }

    // --- Historia/Quests: contadores de progresso de objetivo -----------
    // Chave livre (ver GerenciadorDeQuests.ChaveObjetivo, ex:
    // "quest_entregarPacote_obj_0"). Mesmo padrao das flags acima: dicionario
    // em memoria + persistido pelo SalvamentoJson (ver
    // DadosDoJogo.contadores). Faltava esta classe inteira -- por isso o
    // erro CS1061 em GerenciadorDeQuests (ObterContador/DefinirContador
    // nunca existiram).
    private readonly Dictionary<string, int> _contadores = new Dictionary<string, int>();

    public int ObterContador(string chave) =>
        _contadores.TryGetValue(chave, out int valor) ? valor : 0;

    public void DefinirContador(string chave, int valor) => _contadores[chave] = valor;

    public IReadOnlyDictionary<string, int> TodosOsContadores => _contadores;

    public void CarregarContadores(Dictionary<string, int> contadores)
    {
        _contadores.Clear();
        foreach (var kv in contadores)
        {
            _contadores[kv.Key] = kv.Value;
        }
    }

    // --- Estagio 2 (13/09/2026): Inventario real (substitui ItensDeBatalha,
    // lista fixa sem quantidade). Chave = DefinicaoDeItem.Id (ver
    // CatalogoDeItens.cs). Persistido via SalvamentoJson como List<ItemSalvo>
    // (JsonUtility nao serializa Dictionary).
    private readonly Dictionary<string, int> _inventario = new Dictionary<string, int>();

    public IReadOnlyDictionary<string, int> Inventario => _inventario;

    public int ObterQuantidadeDeItem(string idItem) =>
        _inventario.TryGetValue(idItem, out int qtd) ? qtd : 0;

    public void AdicionarItem(string idItem, int quantidade)
    {
        if (quantidade <= 0) return;
        _inventario[idItem] = ObterQuantidadeDeItem(idItem) + quantidade;
    }

    // Retorna false se nao houver estoque suficiente (nao deixa ficar negativo).
    public bool TentarRemoverItem(string idItem, int quantidade = 1)
    {
        int atual = ObterQuantidadeDeItem(idItem);
        if (atual < quantidade) return false;

        int restante = atual - quantidade;
        if (restante <= 0)
        {
            _inventario.Remove(idItem); // nao deixa lixo de "0 unidades" no dicionario
        }
        else
        {
            _inventario[idItem] = restante;
        }
        return true;
    }

    public void CarregarInventario(Dictionary<string, int> inventario)
    {
        _inventario.Clear();
        foreach (var kv in inventario)
        {
            if (kv.Value > 0) _inventario[kv.Key] = kv.Value;
        }
    }

    // --- Estagio 3 (13/09/2026, fase 2 do sistema de item): Gold. Usado
    // pela recompensa de drop de batalha (ver BattleManager.AplicarRecompensas).
    // Sem loja/gasto real ainda -- TentarGastarGold fica pronto pro proximo
    // passo do checklist ("Sem forma de obter item alem do kit inicial" ja
    // deixa de ser verdade com drop, mas loja continua fora desta entrega).
    public int Gold { get; private set; }

    public void AdicionarGold(int quantidade)
    {
        if (quantidade <= 0) return;
        Gold += quantidade;
    }

    public bool TentarGastarGold(int quantidade)
    {
        if (quantidade <= 0 || Gold < quantidade) return false;
        Gold -= quantidade;
        return true;
    }

    public void CarregarGold(int quantidade)
    {
        Gold = Mathf.Max(0, quantidade);
    }

    // --- Ato 1 (historia): passagem de dados mundo -> cena Battle ------
    //
    // Estes dois NAO sao persistidos de proposito. Sao variaveis de transito
    // do mesmo tipo que PosicaoSalva/CenaDeOrigemDaPosicao: valem entre "o
    // gatilho disparou" e "a batalha comecou", e nada alem disso. Salvar
    // significaria que carregar um save feito no mundo poderia reinjetar um
    // inimigo de uma batalha que nunca aconteceu.
    //
    // ProximoInimigoId: Id no CatalogoDeInimigos. Vazio = BattleManager usa
    // o inimigo digitado no Inspector da cena (comportamento antigo, intacto).
    //
    // FlagDeVitoriaPendente: flag de historia gravada se o jogador VENCER
    // esta batalha (ex: "ato1_sessao3"). Quem sabe o que a luta significa e
    // o gatilho no mundo, nao a cena Battle, que e generica e compartilhada.

    public string ProximoInimigoId { get; set; }

    public string FlagDeVitoriaPendente { get; set; }

    /// <summary>
    /// Chamado pelo BattleManager ao terminar a batalha (vitoria ou derrota),
    /// pra que um encontro aleatorio seguinte nao herde o inimigo/flag de uma
    /// batalha roteirizada anterior.
    /// </summary>
    public void LimparBatalhaPendente()
    {
        ProximoInimigoId = null;
        FlagDeVitoriaPendente = null;
    }

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        if (DroidDoJogador == null)
        {
            // [DEFAULT] valores iniciais de um Droid novo — ajustar quando
            // houver tela de criacao de personagem real.
            // NOTA (Estagio 1): "hpMax: 30" aqui e o HpMaxBase (ver Droid.cs) —
            // o HpMax REAL final ja sai maior por causa do VIT=3 abaixo
            // (30 + 3% = +0 arredondado pra baixo neste caso especifico, mas
            // sobe conforme StatsBase.Vit crescer no terminal).
            DroidDoJogador = new Droid("Heroi", hpMax: 30);
            DroidDoJogador.StatsBase.For = 5;
            DroidDoJogador.StatsBase.Vit = 3;

            // [DEFAULT] Estagio 2: kit inicial de itens, pra Bag nao comecar
            // vazia sem NENHUMA forma de obter item ainda. Estagio 3
            // adiciona drop de batalha (ver BattleManager) -- kit inicial
            // continua existindo, agora so como "ponto de partida".
            AdicionarItem(CatalogoDeItens.IdPocaoPequena, 3);
            AdicionarItem(CatalogoDeItens.IdPocaoMedia, 1);
        }
    }

    public void SalvarPosicao(Vector3 posicao, string nomeDaCena)
    {
        PosicaoSalva = posicao;
        CenaDeOrigemDaPosicao = nomeDaCena;
        TemPosicaoSalva = true;
    }

    public void LimparPosicaoSalva()
    {
        TemPosicaoSalva = false;
    }
}
