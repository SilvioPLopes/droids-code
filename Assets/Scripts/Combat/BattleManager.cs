using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DroidsCode.Combat;
using DroidsCode.DroidCore;

/// <summary>
/// Lógica de turnos da batalha. Este script NÃO cria nenhuma interface —
/// ele espera que você já tenha montado os elementos no Editor e apenas
/// arraste as referências nos campos abaixo, no Inspector.
///
/// Refatorado para consumir Droid/CombatEngine em vez de calcular dano
/// aqui dentro (ver MATRIZ_DESENVOLVIMENTO.md e CORRECAO_ARQUITETURA_TERMINAL_MENU.md).
/// "Atacar" agora abre uma lista de golpes se o Droid tiver mais de um
/// configurado; se só tiver o Ataque Básico, ataca direto.
///
/// NOVO NO INSPECTOR — precisa criar 2 objetos na cena (ver instruções
/// enviadas junto com este arquivo):
///   - painelListaDeAtaques: painel vazio, inativo por padrão
///   - prefabBotaoAtaque: prefab de botão com um texto (TMP) filho
///
/// CORRECAO (13/09/2026 — botão Voltar / ESC / double-painel):
///   - Ambos os painéis (Ataque e Item) agora ganham um botão extra
///     "Voltar" no final da lista, reaproveitando o mesmo prefab — ele só
///     fecha o painel, sem chamar ExecutarAtaqueDoJogador/UsarItem.
///   - ESC fecha o painel que estiver aberto (Update() novo).
///   - Bug encontrado durante a correção: nada impedia abrir Ataque e,
///     sem fechar, clicar Item (botaoItem continuava interactable) —
///     os dois painéis ficavam abertos ao mesmo tempo. Corrigido: cada
///     Abrir...() agora fecha o outro painel antes de abrir o seu.
///
/// ATO 1 / HISTORIA (15/09/2026):
///   - O inimigo pode vir do CatalogoDeInimigos em vez do Inspector. Quem
///     inicia a batalha (GatilhoDeBatalha/EncounterZone) escreve o Id em
///     GerenciadorDeEstado.ProximoInimigoId e este script se monta a partir
///     dele — nome, HP, ATK, DEF, XP, Gold e tabela de drops. Se o Id vier
///     vazio ou desconhecido, NADA muda: continua valendo exatamente o que
///     esta digitado no Inspector desta cena (compatibilidade total com a
///     cena Battle que ja existe).
///   - Ao VENCER, grava a flag de historia que veio em
///     GerenciadorDeEstado.FlagDeVitoriaPendente (ex: "ato1_sessao3"), ou a
///     flagDeVitoria digitada no Inspector, o que existir.
///   - Update() agora ignora ESC quando ha um painel do Menu do Mundo aberto.
///     Motivo: desde que o MenuMundoManager ganhou DontDestroyOnLoad, o menu
///     VIAJA pra dentro da cena Battle — a suposicao antiga ("esta cena nao
///     convive com o Menu do Mundo") deixou de valer.
///
/// ESTAGIO 3 (fase 2 do sistema de item, sem alinhamento extra):
///   - UsarItem agora trata Cura/Buff/Debuff (Equipavel/ForaDeBatalha nao
///     sao UsavelEmBatalha, ver CatalogoDeItens — nao aparecem na lista).
///   - Nova tabela de drop (tabelaDeDrops) + Gold (goldMinimo/goldMaximo),
///     configuraveis no Inspector por inimigo. Rolados em AplicarRecompensas,
///     chamado nos dois pontos onde a vitoria contra o inimigo é detectada.
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Referências de UI (arraste os objetos da Hierarchy aqui)")]
    public Slider sliderPlayer;
    public Slider sliderInimigo;
    public TextMeshProUGUI textoMensagem;
    public Button botaoAtacar;
    public Button botaoItem;
    public Button botaoFugir;
    public Button botaoStatus;
    public TelaDeStatusManager telaDeStatus;

    [Header("Lista de golpes (novo)")]
    [Tooltip("Painel vazio, desativado por padrão, que recebe os botões de cada golpe.")]
    public Transform painelListaDeAtaques;
    [Tooltip("Prefab de um botão simples (com TextMeshProUGUI filho), um por golpe.")]
    public Button prefabBotaoAtaque;

    [Header("Lista de itens (novo)")]
    [Tooltip("Painel vazio, desativado por padrão, que recebe os botões de cada item. Pode ser uma cópia do painelListaDeAtaques.")]
    public Transform painelListaDeItens;
    [Tooltip("Prefab de botão pros itens. Pode reutilizar o mesmo prefabBotaoAtaque.")]
    public Button prefabBotaoItem;

    [Header("Status do Player")]
    // nome/hp/atributos agora vêm do GerenciadorDeEstado.DroidDoJogador —
    // esses campos foram removidos daqui de propósito (ver GerenciadorDeEstado.cs).

    [Header("Status do Inimigo")]
    public string nomeInimigo = "Esqueleto";
    public int hpMaxInimigo = 20;
    public int atkInimigo = 5;
    public int defInimigo = 1;
    public int recompensaXpInimigo = 10;

    // Estagio 3 (fase 2 do sistema de item): recompensa de Gold/item ao
    // vencer este inimigo especificamente. Cada entrada de tabelaDeDrops
    // e rolada de forma INDEPENDENTE (varios itens podem dropar juntos).
    [System.Serializable]
    public class DropDeItem
    {
        [Tooltip("Id do item no CatalogoDeItens (ex: \"pocao_pequena\"). Ids invalidos sao ignorados silenciosamente.")]
        public string idItem;
        [Range(0f, 1f)]
        [Tooltip("Chance deste drop especifico (0 a 1), independente dos outros.")]
        public float chance = 1f;
        public int quantidadeMinima = 1;
        public int quantidadeMaxima = 1;
    }

    [Header("Recompensas de vitória (Estágio 3 — fase 2 do sistema de item)")]
    [Tooltip("Gold minimo dropado ao vencer (0 = sem Gold garantido).")]
    public int goldMinimo = 0;
    [Tooltip("Gold maximo dropado ao vencer.")]
    public int goldMaximo = 0;
    [Tooltip("Cada entrada e um item que PODE dropar, com sua propria chance.")]
    public List<DropDeItem> tabelaDeDrops = new List<DropDeItem>();

    [Header("História (Ato 1)")]
    [Tooltip("Id no CatalogoDeInimigos usado quando NINGUÉM definiu ProximoInimigoId (ex: você deu Play direto na cena Battle). Vazio = usa os campos de 'Status do Inimigo' acima.")]
    public string idDoInimigoPadrao = "";

    [Tooltip("Flag de história gravada ao vencer, quando nenhuma flag pendente veio do gatilho que iniciou a batalha. Normalmente deixe vazio — quem manda é o GatilhoDeBatalha.")]
    public string flagDeVitoria = "";

    [Header("Configuração")]
    [Tooltip("Cena para onde voltar depois da batalha.")]
    public string nomeCenaMundo = "Game";
    [Range(0f, 1f)]
    public float chanceDeFugir = 0.5f;

    private Droid droid;
    private InimigoFixo inimigo;
    private CombatEngine engine;
    private bool turnoDoJogador = true;
    private bool batalhaEncerrada = false;

    // Log de batalha: mesmo padrao do Terminal (TerminalUIManager.linhasDeLog)
    // -- mantem as ultimas N linhas em vez de sobrescrever a mensagem toda vez.
    private readonly List<string> _logDeBatalha = new List<string>();
    private const int MaximoDeLinhasDeLogDeBatalha = 4;

    void Start()
    {
        droid = GerenciadorDeEstado.Instancia.DroidDoJogador;

        // NOVO (Ato 1): se algum gatilho no mundo escolheu um inimigo do
        // catalogo, ele sobrescreve os campos do Inspector ANTES de montar o
        // InimigoFixo. Chamada sem efeito nenhum quando ProximoInimigoId
        // esta vazio -- a cena Battle antiga continua funcionando igual.
        AplicarInimigoDoCatalogo();

        inimigo = new InimigoFixo(nomeInimigo, hpMaxInimigo, atkInimigo, defInimigo, recompensaXpInimigo);
        engine = new CombatEngine();

        botaoAtacar.onClick.AddListener(AoClicarAtacar);
        botaoItem.onClick.AddListener(AoClicarItem);
        botaoFugir.onClick.AddListener(AoClicarFugir);
        if (botaoStatus != null && telaDeStatus != null)
        {
            botaoStatus.onClick.AddListener(() => telaDeStatus.Mostrar());
        }

        if (painelListaDeAtaques != null)
            painelListaDeAtaques.gameObject.SetActive(false);

        if (painelListaDeItens != null)
            painelListaDeItens.gameObject.SetActive(false);

        sliderPlayer.minValue = 0;
        sliderPlayer.maxValue = 1;
        sliderInimigo.minValue = 0;
        sliderInimigo.maxValue = 1;

        AtualizarBarras();
        MostrarMensagem($"Um {nomeInimigo} selvagem apareceu!");
    }

    // NOVO (13/09/2026): ESC fecha o painel de Ataque ou Item que estiver
    // aberto no momento. Assume que esta cena (Battle) não convive junto
    // com o Menu do Mundo (que também usa Escape) — confirmar se algum dia
    // as duas rodarem sobrepostas.
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        // CORRECAO (15/09/2026): o comentario acima dizia "assume que esta
        // cena nao convive com o Menu do Mundo -- confirmar se algum dia as
        // duas rodarem sobrepostas". Esse dia chegou: MenuMundoManager ganhou
        // DontDestroyOnLoad pra funcionar na Cidade, entao ele agora existe
        // TAMBEM dentro da cena Battle. Sem esta guarda, um unico ESC seria
        // consumido pelos dois scripts ao mesmo tempo (fecharia a lista de
        // ataques E abriria o menu de pausa por cima da batalha).
        if (GerenciadorDeEstado.Instancia.MenuAberto) return;

        if (painelListaDeAtaques != null && painelListaDeAtaques.gameObject.activeSelf)
        {
            painelListaDeAtaques.gameObject.SetActive(false);
        }
        else if (painelListaDeItens != null && painelListaDeItens.gameObject.activeSelf)
        {
            painelListaDeItens.gameObject.SetActive(false);
        }
    }

    // ------------------------------------------------------------------
    // AÇÕES DOS BOTÕES
    // ------------------------------------------------------------------

    void AoClicarAtacar()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        var acoes = new List<string>(droid.ObterAcoesDisponiveis());

        if (acoes.Count <= 1)
        {
            string unica = acoes.Count == 1 ? acoes[0] : Droid.NomeAtaqueBasico;
            ExecutarAtaqueDoJogador(unica);
        }
        else
        {
            AbrirListaDeAtaques(acoes);
        }
    }

    void AbrirListaDeAtaques(List<string> acoes)
    {
        if (painelListaDeAtaques == null || prefabBotaoAtaque == null)
        {
            Debug.LogWarning("painelListaDeAtaques/prefabBotaoAtaque não configurados no Inspector — atacando com o básico.");
            ExecutarAtaqueDoJogador(Droid.NomeAtaqueBasico);
            return;
        }

        // CORRECAO (13/09/2026 — double-painel): garante que o painel de
        // itens não fique aberto por baixo do painel de ataques.
        if (painelListaDeItens != null)
            painelListaDeItens.gameObject.SetActive(false);

        foreach (Transform filho in painelListaDeAtaques)
            Destroy(filho.gameObject);

        foreach (string nomeAcao in acoes)
        {
            Button botao = Instantiate(prefabBotaoAtaque, painelListaDeAtaques);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = nomeAcao;

            string nomeCapturado = nomeAcao; // evita captura errada da variavel de loop
            botao.onClick.AddListener(() =>
            {
                painelListaDeAtaques.gameObject.SetActive(false);
                ExecutarAtaqueDoJogador(nomeCapturado);
            });
        }

        // NOVO (13/09/2026): botão "Voltar" no final da lista — só fecha o
        // painel, sem executar nenhuma ação.
        AdicionarBotaoVoltar(painelListaDeAtaques, prefabBotaoAtaque);

        painelListaDeAtaques.gameObject.SetActive(true);
    }

    void ExecutarAtaqueDoJogador(string nomeAcao)
    {
        ResultadoAcao resultado = engine.ExecutarTurno(droid, inimigo, nomeAcao);
        AtualizarBarras();
        MostrarMensagem(resultado.Mensagem);

        // CORRECAO (12/09/2026): checar os DOIS lados sempre, nao so o
        // "obvio" deste metodo (inimigo). Dano por turno (veneno) pode
        // zerar o HP do proprio atacante (droid) no inicio do turno dele,
        // antes de agir -- se so checarmos o inimigo aqui, essa derrota do
        // droid so seria percebida no proximo metodo, um turno depois.
        // Prioridade: se ambos cairem no mesmo turno, tratamos como derrota
        // do jogador (efeito de veneno nele mesmo nao deveria contar como
        // vitoria contra o inimigo que ainda esta de pe).
        if (engine.VerificarDerrota(droid))
        {
            StartCoroutine(FinalizarBatalha(false));
        }
        else if (engine.VerificarDerrota(inimigo))
        {
            SistemaDeProgressao.GanharExperiencia(droid, inimigo.RecompensaXp);
            MostrarMensagem($"Você ganhou {inimigo.RecompensaXp} de XP!");
            GravarFlagDeVitoria();
            AplicarRecompensas();
            StartCoroutine(FinalizarBatalha(true));
        }
        else
            StartCoroutine(TurnoDoInimigo());
    }

    void AoClicarItem()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        AbrirListaDeItens();
    }

    // REFATORACAO (Estagio 2 — 13/09/2026): antes lia ItensDeBatalha.Disponiveis
    // (lista fixa, sem quantidade). Agora le o inventario real do jogador
    // (GerenciadorDeEstado) -- so mostra itens com quantidade > 0, e o
    // botao exibe "(xN)" com o estoque atual.
    void AbrirListaDeItens()
    {
        var gerenciador = GerenciadorDeEstado.Instancia;
        var itensComEstoque = new List<DefinicaoDeItem>();

        foreach (var kv in gerenciador.Inventario)
        {
            if (kv.Value <= 0) continue;
            var def = CatalogoDeItens.Obter(kv.Key);
            // Item desconhecido no catalogo (ex: save de versao futura) --
            // ignora em vez de quebrar a lista inteira.
            if (def == null || !def.UsavelEmBatalha) continue;
            itensComEstoque.Add(def);
        }

        if (itensComEstoque.Count == 0)
        {
            MostrarMensagem("Nenhum item disponível.");
            return;
        }

        if (painelListaDeItens == null || prefabBotaoItem == null)
        {
            Debug.LogWarning("painelListaDeItens/prefabBotaoItem não configurados no Inspector — usando o primeiro item direto.");
            UsarItem(itensComEstoque[0]);
            return;
        }

        // CORRECAO (13/09/2026 — double-painel): garante que o painel de
        // ataques não fique aberto por baixo do painel de itens.
        if (painelListaDeAtaques != null)
            painelListaDeAtaques.gameObject.SetActive(false);

        foreach (Transform filho in painelListaDeItens)
            Destroy(filho.gameObject);

        foreach (DefinicaoDeItem item in itensComEstoque)
        {
            int quantidade = gerenciador.ObterQuantidadeDeItem(item.Id);

            Button botao = Instantiate(prefabBotaoItem, painelListaDeItens);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            // Estagio 3: descricao generica, ja que agora existem tipos
            // alem de Cura (Buff/Debuff) na lista de batalha.
            if (texto != null) texto.text = DescreverItem(item, quantidade);

            DefinicaoDeItem itemCapturado = item; // evita captura errada da variavel de loop
            botao.onClick.AddListener(() =>
            {
                painelListaDeItens.gameObject.SetActive(false);
                UsarItem(itemCapturado);
            });
        }

        // NOVO (13/09/2026): botão "Voltar" no final da lista — só fecha o
        // painel, sem usar nenhum item.
        AdicionarBotaoVoltar(painelListaDeItens, prefabBotaoItem);

        painelListaDeItens.gameObject.SetActive(true);
    }

    // NOVO (Estagio 3): texto do botao de item, generico por Tipo -- antes
    // era hard-coded pra Cura ("(+X HP)").
    string DescreverItem(DefinicaoDeItem item, int quantidade)
    {
        string detalhe = item.Tipo switch
        {
            TipoDeItem.Cura => $"(+{item.CuraHp} HP)",
            TipoDeItem.Buff => item.EfeitoDeAtributoAplicado != null ? $"({item.EfeitoDeAtributoAplicado.NomeExibicao})" : "",
            TipoDeItem.Debuff => $"({item.EfeitoDeAtributoAplicado?.NomeExibicao ?? item.EfeitoDeDanoAplicado?.NomeExibicao})",
            _ => ""
        };
        return string.IsNullOrEmpty(detalhe) ? $"{item.Nome} x{quantidade}" : $"{item.Nome} {detalhe} x{quantidade}";
    }

    // NOVO (13/09/2026): reaproveitado pelos dois painéis (Ataque e Item).
    // Instancia mais um botão do mesmo prefab, com texto "Voltar", que só
    // desativa o painel — nunca chama ExecutarAtaqueDoJogador/UsarItem.
    void AdicionarBotaoVoltar(Transform painel, Button prefabBotao)
    {
        Button botaoVoltar = Instantiate(prefabBotao, painel);
        botaoVoltar.gameObject.SetActive(true);

        var texto = botaoVoltar.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null) texto.text = "Voltar";

        botaoVoltar.onClick.AddListener(() => painel.gameObject.SetActive(false));
    }

    // REFATORACAO (Estagio 2 — 13/09/2026): recebe DefinicaoDeItem (catalogo)
    // em vez de ItemConsumivel. Decrementa o estoque real via
    // GerenciadorDeEstado.TentarRemoverItem e SALVA na hora (SalvamentoJson)
    // -- decisao: persistir o consumo imediatamente evita perder o item "de
    // graca" se o jogo fechar no meio da batalha (o jogador ja recebeu o
    // efeito, entao o consumo tem que ficar gravado).
    // REFATORACAO (Estagio 3 — fase 2 do sistema de item): generalizado pra
    // tambem tratar Buff (aplica no proprio Droid) e Debuff (aplica no
    // inimigo, rolando resistencia). Equipavel/ForaDeBatalha nao chegam
    // aqui (UsavelEmBatalha=false no catalogo), mas o "default" abaixo
    // devolve o item em vez de sumir com ele, por seguranca.
    void UsarItem(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            // Guarda de seguranca: nao deveria acontecer (a lista so mostra
            // itens com estoque > 0), mas evita usar de graca se acontecer.
            MostrarMensagem($"Você não tem mais {item.Nome}.");
            return;
        }

        switch (item.Tipo)
        {
            case TipoDeItem.Cura:
                UsarItemDeCura(item);
                break;

            case TipoDeItem.Buff:
                AplicarEfeitoDeItem(item, droid);
                MostrarMensagem($"{droid.Nome} usou {item.Nome}!");
                break;

            case TipoDeItem.Debuff:
                AplicarEfeitoDeItem(item, inimigo);
                MostrarMensagem($"{droid.Nome} usou {item.Nome} em {inimigo.Nome}!");
                break;

            default:
                // Nao deveria chegar aqui (Equipavel/ForaDeBatalha tem
                // UsavelEmBatalha=false), mas devolve o item em vez de
                // consumir silenciosamente se acontecer.
                gerenciador.AdicionarItem(item.Id, 1);
                MostrarMensagem($"{item.Nome} não pode ser usado em batalha.");
                return;
        }

        AtualizarBarras();
        new SalvamentoJson().Salvar();
        StartCoroutine(TurnoDoInimigo());
    }

    // NOVO (Estagio 3): cura de item, extraido de UsarItem sem mudar a
    // formula (bonus de INT, ver TabelaDeCombate.PercentualBonusCuraPorInt).
    void UsarItemDeCura(DefinicaoDeItem item)
    {
        float multiplicador = 1f + (droid.BonusPercentualDeCura / 100f);
        int curaBase = Mathf.FloorToInt(item.CuraHp * multiplicador);

        int curaAplicada = Mathf.Min(curaBase, droid.HpMax - droid.Hp);
        droid.Hp = Mathf.Min(droid.HpMax, droid.Hp + curaBase);
        MostrarMensagem($"{droid.Nome} usou {item.Nome} e recuperou {curaAplicada} de HP!");
    }

    // NOVO (Estagio 3): aplica o(s) efeito(s) de Buff/Debuff do item no
    // alvo dado. Debuff rola resistencia (INT do alvo, mesma formula de
    // efeito de tecnica — ver Droid.RolarResistencia); Buff nao rola
    // resistencia (o proprio Droid nao "resiste" o proprio item).
    void AplicarEfeitoDeItem(DefinicaoDeItem item, IParticipanteDeCombate alvo)
    {
        bool ehDebuff = item.Tipo == TipoDeItem.Debuff;

        if (item.EfeitoDeAtributoAplicado != null)
        {
            if (ehDebuff && Droid.RolarResistencia(alvo.ChanceDeResistirEfeito))
            {
                MostrarMensagem($"{alvo.Nome} resistiu ao efeito!");
            }
            else
            {
                alvo.EfeitosAtivos.Add(new EfeitoDeAtributo
                {
                    NomeExibicao = item.EfeitoDeAtributoAplicado.NomeExibicao,
                    Atributo = item.EfeitoDeAtributoAplicado.Atributo,
                    Valor = item.EfeitoDeAtributoAplicado.Valor,
                    DuracaoEmTurnos = item.EfeitoDeAtributoAplicado.DuracaoEmTurnos
                });
            }
        }

        if (item.EfeitoDeDanoAplicado != null)
        {
            if (ehDebuff && Droid.RolarResistencia(alvo.ChanceDeResistirEfeito))
            {
                MostrarMensagem($"{alvo.Nome} resistiu ao efeito!");
            }
            else
            {
                alvo.EfeitosDeDanoAtivos.Add(new EfeitoDeDanoPorTurno
                {
                    NomeExibicao = item.EfeitoDeDanoAplicado.NomeExibicao,
                    DanoPorTurno = item.EfeitoDeDanoAplicado.DanoPorTurno,
                    DuracaoEmTurnos = item.EfeitoDeDanoAplicado.DuracaoEmTurnos
                });
            }
        }
    }

    // NOVO (Estagio 3): roda uma vez, na vitoria (chamado nos dois pontos
    // onde a derrota do inimigo e detectada — ver ExecutarAtaqueDoJogador e
    // TurnoDoInimigo). Rola Gold (goldMinimo..goldMaximo) e cada entrada de
    // tabelaDeDrops de forma independente, credita no GerenciadorDeEstado e
    // salva na hora -- mesmo padrao de "persistir imediatamente" que
    // UsarItem/BagUIManager ja usam.
    void AplicarRecompensas()
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        if (goldMaximo > 0)
        {
            int goldGanho = Random.Range(goldMinimo, goldMaximo + 1);
            if (goldGanho > 0)
            {
                gerenciador.AdicionarGold(goldGanho);
                MostrarMensagem($"Você encontrou {goldGanho} de Gold!");
            }
        }

        foreach (DropDeItem drop in tabelaDeDrops)
        {
            if (string.IsNullOrEmpty(drop.idItem)) continue;
            if (Random.value > drop.chance) continue;

            var definicao = CatalogoDeItens.Obter(drop.idItem);
            if (definicao == null) continue; // id invalido no Inspector -- ignora em vez de quebrar

            int quantidade = Random.Range(drop.quantidadeMinima, drop.quantidadeMaxima + 1);
            if (quantidade <= 0) continue;

            gerenciador.AdicionarItem(drop.idItem, quantidade);
            MostrarMensagem($"Você obteve {definicao.Nome} x{quantidade}!");
        }

        new SalvamentoJson().Salvar();
    }

    // NOVO (Ato 1): le GerenciadorDeEstado.ProximoInimigoId (escrito por
    // GatilhoDeBatalha ou EncounterZone) e sobrescreve os campos de inimigo
    // e de recompensa deste BattleManager. Fallback em cascata:
    //   1. ProximoInimigoId (batalha roteirizada ou zona de encontro)
    //   2. idDoInimigoPadrao (Inspector desta cena)
    //   3. nada -- valem os campos "Status do Inimigo" digitados a mao
    // Id desconhecido cai no passo 3 com um aviso, nunca quebra a cena.
    void AplicarInimigoDoCatalogo()
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        string id = !string.IsNullOrEmpty(gerenciador.ProximoInimigoId)
            ? gerenciador.ProximoInimigoId
            : idDoInimigoPadrao;

        if (string.IsNullOrEmpty(id)) return;

        DefinicaoDeInimigo definicao = CatalogoDeInimigos.Obter(id);
        if (definicao == null)
        {
            Debug.LogWarning($"BattleManager: inimigo '{id}' nao existe no CatalogoDeInimigos. Usando os valores do Inspector.");
            return;
        }

        nomeInimigo = definicao.Nome;
        hpMaxInimigo = definicao.HpMax;
        atkInimigo = definicao.Ataque;
        defInimigo = definicao.Defesa;
        recompensaXpInimigo = definicao.RecompensaXp;
        goldMinimo = definicao.GoldMinimo;
        goldMaximo = definicao.GoldMaximo;

        // Converte DropDeInimigo (C# puro, dominio) em DropDeItem (classe
        // serializavel do MonoBehaviour). Ver comentario em CatalogoDeInimigos.
        tabelaDeDrops = new List<DropDeItem>();
        if (definicao.Drops != null)
        {
            foreach (DropDeInimigo drop in definicao.Drops)
            {
                tabelaDeDrops.Add(new DropDeItem
                {
                    idItem = drop.IdItem,
                    chance = drop.Chance,
                    quantidadeMinima = drop.QuantidadeMinima,
                    quantidadeMaxima = drop.QuantidadeMaxima
                });
            }
        }
    }

    // NOVO (Ato 1): grava a flag de historia da vitoria. Chamado nos DOIS
    // pontos onde a derrota do inimigo e detectada, junto de AplicarRecompensas.
    void GravarFlagDeVitoria()
    {
        var gerenciador = GerenciadorDeEstado.Instancia;

        string flag = !string.IsNullOrEmpty(gerenciador.FlagDeVitoriaPendente)
            ? gerenciador.FlagDeVitoriaPendente
            : flagDeVitoria;

        if (string.IsNullOrEmpty(flag)) return;

        gerenciador.DefinirFlag(flag, true);
        // AplicarRecompensas() ja chama Salvar() logo em seguida, entao a
        // flag entra no mesmo save -- nao precisa de um segundo Salvar aqui.
    }

    void AoClicarFugir()
    {
        if (!turnoDoJogador || batalhaEncerrada) return;

        if (Random.value <= chanceDeFugir)
        {
            MostrarMensagem("Você fugiu com segurança!");
            StartCoroutine(VoltarParaOMundo(1.5f));
        }
        else
        {
            MostrarMensagem("Não foi possível fugir!");
            StartCoroutine(TurnoDoInimigo());
        }
    }

    // ------------------------------------------------------------------
    // FLUXO DE TURNOS
    // ------------------------------------------------------------------

    IEnumerator TurnoDoInimigo()
    {
        turnoDoJogador = false;
        DefinirBotoesInterativos(false);
        yield return new WaitForSeconds(1f);

        string acaoInimigo = new List<string>(inimigo.ObterAcoesDisponiveis())[0];
        ResultadoAcao resultado = engine.ExecutarTurno(inimigo, droid, acaoInimigo);
        AtualizarBarras();
        MostrarMensagem(resultado.Mensagem);
        yield return new WaitForSeconds(1f);

        // CORRECAO (12/09/2026): mesma ideia do outro metodo -- checar os
        // dois lados, nao so o droid. Se o inimigo tiver algum dano por
        // turno no futuro (efeito nele mesmo) e morrer antes de agir, a
        // vitoria precisa ser detectada aqui tambem.
        if (engine.VerificarDerrota(droid))
        {
            StartCoroutine(FinalizarBatalha(false));
        }
        else if (engine.VerificarDerrota(inimigo))
        {
            SistemaDeProgressao.GanharExperiencia(droid, inimigo.RecompensaXp);
            MostrarMensagem($"Você ganhou {inimigo.RecompensaXp} de XP!");
            GravarFlagDeVitoria();
            AplicarRecompensas();
            StartCoroutine(FinalizarBatalha(true));
        }
        else
        {
            turnoDoJogador = true;
            DefinirBotoesInterativos(true);
            MostrarMensagem("O que você vai fazer?");
        }
    }

    IEnumerator FinalizarBatalha(bool vitoria)
    {
        batalhaEncerrada = true;

        // NOVO (Ato 1): zera o inimigo/flag de transito pra que o PROXIMO
        // encontro aleatorio nao herde o inimigo de uma batalha roteirizada
        // (ex: voltar da luta contra o chefe e encontrar outro chefe no mato).
        GerenciadorDeEstado.Instancia.LimparBatalhaPendente();

        DefinirBotoesInterativos(false);
        MostrarMensagem(vitoria ? $"Você derrotou o {nomeInimigo}!" : "Você foi derrotado...");
        yield return VoltarParaOMundo(2f);
    }

    IEnumerator VoltarParaOMundo(float espera)
    {
        yield return new WaitForSeconds(espera);
        if (!string.IsNullOrEmpty(nomeCenaMundo))
            SceneManager.LoadScene(nomeCenaMundo);
    }

    // ------------------------------------------------------------------
    // AUXILIARES
    // ------------------------------------------------------------------

    void AtualizarBarras()
    {
        sliderPlayer.value = (float)droid.Hp / droid.HpMax;
        sliderInimigo.value = (float)inimigo.Hp / inimigo.HpMax;
    }

    void MostrarMensagem(string mensagem)
    {
        // Uma chamada pode trazer mais de um evento junto (ex: dano de veneno
        // + resultado da ação, separados por \n pelo CombatEngine) -- cada
        // linha vira uma entrada própria no log, igual o Terminal já faz.
        foreach (string linha in mensagem.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(linha)) continue;
            _logDeBatalha.Add(linha);
            Debug.Log($"[Batalha] {linha}"); // histórico fica no Console também
        }

        while (_logDeBatalha.Count > MaximoDeLinhasDeLogDeBatalha)
        {
            _logDeBatalha.RemoveAt(0);
        }

        textoMensagem.text = string.Join("\n", _logDeBatalha);
    }

    void DefinirBotoesInterativos(bool ativo)
    {
        botaoAtacar.interactable = ativo;
        botaoItem.interactable = ativo;
        botaoFugir.interactable = ativo;
    }
}
