using System.Collections.Generic;
using DroidsCode.DroidCore;
using DroidsCode.Combat;

/// <summary>
/// Catalogo de missoes. Mesmo padrao dos outros catalogos do projeto
/// (Dictionary com Id string estavel, Obter devolvendo null).
///
/// TUDO aqui e DADO, nao comportamento -- quem executa e
/// GerenciadorDeQuests. Isso permite adicionar quest nova sem tocar em
/// nenhum script de cena, que e a mesma logica que ja vale pra
/// CatalogoDeItens/CatalogoDeInimigos.
///
/// PROGRESSO E PERSISTENCIA: cada quest usa flags e contadores do
/// GerenciadorDeEstado, ambos ja persistidos no save (contadores entraram
/// na v5). Nenhuma estrutura de save nova foi inventada pra isso.
///   flag  "quest_{id}_ativa"      -> aceita
///   flag  "quest_{id}_concluida"  -> entregue
///   contador "quest_{id}_obj_{n}" -> progresso do objetivo n
/// </summary>
public static class CatalogoDeQuests
{
    // --- Ferrovale / secundarias ---
    public const string IdAguaParaDara = "q_agua_dara";
    public const string IdCobreDoTacio = "q_cobre_tacio";
    public const string IdAPernaDoApollo = "q_perna_apollo";
    public const string IdLimpezaDaBorda = "q_limpeza_borda";

    // --- Guilda dos Sucateiros (linha propria, ver EhDeGuilda) ---
    public const string IdGuildaIniciacao = "q_guilda_1_iniciacao";
    public const string IdGuildaCacada = "q_guilda_2_cacada";
    public const string IdGuildaOAlfa = "q_guilda_3_alfa";
    public const string IdGuildaOQueAGuildaSabe = "q_guilda_4_verdade";

    // --- Linha principal (espelham o roteiro, dao XP de acerto) ---
    public const string IdPrincipalDescobrirAgua = "q_main_agua";
    public const string IdPrincipalAEstacao = "q_main_estacao";

    public static readonly Dictionary<string, DefinicaoDeQuest> Todos =
        new Dictionary<string, DefinicaoDeQuest>
        {
            // =============================================================
            // PRINCIPAIS — existem pra que o objetivo da história apareça no
            // diário junto das secundárias, em vez de o jogador ter que
            // adivinhar pra onde ir.
            // =============================================================
            [IdPrincipalDescobrirAgua] = new DefinicaoDeQuest
            {
                Id = IdPrincipalDescobrirAgua,
                Titulo = "A cota cortada",
                NpcQueOferece = CatalogoDeNpcs.IdApollo,
                EhPrincipal = true,
                AceitaAutomaticamente = true,
                FlagNecessaria = "ato1_casa_resolvida",
                Resumo = "A fila do poço está maior que o normal e ninguém quer dizer por quê. Descubra o que aconteceu com a água de Ferrovale.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdDara, Quantidade = 1, Descricao = "Falar com a Dara na fila do poço" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdHomemDaFila, Quantidade = 1, Descricao = "Falar com o homem da fila" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdTacio, Quantidade = 1, Descricao = "Falar com o Senhor Tácio" }
                },
                XpRecompensa = 120, GoldRecompensa = 15,
                FlagAoConcluir = "ato1_sessao2",
                Curiosidade = "Ferrovale nunca teve encanamento próprio. A estação de captação foi construída pela prefeitura antiga em 2041 e nunca foi atualizada — o Ivo mantém aquilo funcionando com peça de sucata há dezenove anos."
            },

            [IdPrincipalAEstacao] = new DefinicaoDeQuest
            {
                Id = IdPrincipalAEstacao,
                Titulo = "A estação de captação",
                NpcQueOferece = CatalogoDeNpcs.IdApollo,
                EhPrincipal = true,
                AceitaAutomaticamente = true,
                FlagNecessaria = "ato1_sessao3",
                Resumo = "O Ivo fechou a estação e chamou a Apex até aqui. Atravesse a Cratera e resolva isso — de um jeito ou de outro.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.AlcancarFlag, Alvo = "ato1_faccao_escolhida", Quantidade = 1, Descricao = "Decidir sobre Rasha e Teodoro" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.ResolverPuzzle, Alvo = CatalogoDePuzzles.IdPainelDeEnergia, Quantidade = 1, Descricao = "Religar o painel de energia da Cratera" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Derrotar, Alvo = CatalogoDeInimigos.IdDroidDoIvo, Quantidade = 1, Descricao = "Enfrentar o droid do Ivo" }
                },
                XpRecompensa = 400, GoldRecompensa = 60,
                FlagAoConcluir = "ato1_sessao6",
                Curiosidade = "O droid do Ivo não tinha nenhuma salvaguarda de combate. Isso não é descuido: salvaguarda exige testar, e testar exige tempo que ele não tinha."
            },

            // =============================================================
            // SECUNDÁRIAS DE FERROVALE
            // =============================================================
            [IdAguaParaDara] = new DefinicaoDeQuest
            {
                Id = IdAguaParaDara,
                Titulo = "Um filtro para a Dara",
                NpcQueOferece = CatalogoDeNpcs.IdDara,
                FlagNecessaria = "ato1_falou_dara",
                Resumo = "A Dara divide a cota com duas famílias do beco. Com a metade do que era, ela precisa filtrar o que sobrou — e o filtro dela rachou no inverno.",
                TextoDeOferta = "Você é o garoto que conserta coisa, né? Eu não tenho o que te pagar. Mas se aparecer um filtro nessa cidade, eu preciso de um.",
                TextoDeEntrega = "Ela segura o filtro com as duas mãos, do jeito que segurava o balde vazio. — Isso aqui vale três dias. Obrigada.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdFiltroDeAgua, Quantidade = 1, Descricao = "Conseguir um Filtro de Água" }
                },
                XpRecompensa = 90, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdPocaoMedia, Quantidade = 2 }
                },
                FlagAoConcluir = "quest_dara_filtro_ok",
                Curiosidade = "O irmão da Dara foi levado numa captura da Apex dois invernos atrás. Ela ainda divide a cota em três partes por hábito."
            },

            [IdCobreDoTacio] = new DefinicaoDeQuest
            {
                Id = IdCobreDoTacio,
                Titulo = "Cobre é moeda",
                NpcQueOferece = CatalogoDeNpcs.IdTacio,
                FlagNecessaria = "ato1_falou_tacio",
                Resumo = "O Tácio compra fio de cobre por um preço melhor que o de qualquer outro na praça. Ele não explica por quê.",
                TextoDeOferta = "Traz cobre. Fio, cabo, bobina, o que for. Eu pago melhor que a praça inteira e não faço pergunta — e nem você.",
                TextoDeEntrega = "Ele pesa o cobre na mão, sem balança nenhuma, e assente. — Tá bom. Tá bom mesmo.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdFioDeCobre, Quantidade = 5, Descricao = "Entregar 5 Fios de Cobre" }
                },
                XpRecompensa = 80, GoldRecompensa = 70,
                Repetivel = true,
                FlagAoConcluir = "quest_tacio_cobre_ok",
                Curiosidade = "O contador Geiger quebrado no pescoço do Tácio não mede nada há oito anos. Ele mantém porque as pessoas compram mais rápido de quem parece saber o que está fazendo."
            },

            [IdAPernaDoApollo] = new DefinicaoDeQuest
            {
                Id = IdAPernaDoApollo,
                Titulo = "A perna do Apollo",
                NpcQueOferece = CatalogoDeNpcs.IdApollo,
                FlagNecessaria = "ato1_sessao6",
                Resumo = "Sete anos mancando. O servo que caiu do droid do Ivo é o primeiro sobressalente compatível que aparece.",
                TextoDeOferta = "— Não é urgente. Nunca foi urgente. — Ele diz isso do mesmo jeito que diz tudo. — Mas se você quiser, agora dá.",
                TextoDeEntrega = "O Apollo dá três passos. Depois mais três. Não diz nada por um tempo longo demais pra um droid.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdPecaDaPernaDoApollo, Quantidade = 1, Descricao = "Recuperar o servo da perna" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdSucataLimpa, Quantidade = 3, Descricao = "Juntar 3 de Sucata Limpa pro reparo" }
                },
                XpRecompensa = 200, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdServoMotor, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_apollo_perna_ok",
                Curiosidade = "O Apollo foi montado quando o Lip tinha treze anos, com peças de quatro droids diferentes. A perna sempre foi a parte pior — era o que sobrou."
            },

            [IdLimpezaDaBorda] = new DefinicaoDeQuest
            {
                Id = IdLimpezaDaBorda,
                Titulo = "Limpeza da borda",
                NpcQueOferece = CatalogoDeNpcs.IdMestraDaGuilda,
                FlagNecessaria = "ato1_sessao3",
                Resumo = "Sucatas de captura continuam aparecendo perto da cerca. Enquanto estiverem ali, ninguém sai pra catar.",
                TextoDeOferta = "Três delas, no mínimo. Não quero saber de bravura — quero a borda limpa até o fim do dia.",
                TextoDeEntrega = "— Feito é feito. — Ela empurra as moedas pela mesa sem contar.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Derrotar, Alvo = CatalogoDeInimigos.IdSucataDeCaptura, Quantidade = 3, Descricao = "Derrotar 3 Sucatas de Captura" }
                },
                XpRecompensa = 150, GoldRecompensa = 45,
                Repetivel = true,
                FlagAoConcluir = "quest_limpeza_borda_ok",
                Curiosidade = "Sucatas de captura não se afastam mais que dois quilômetros do ponto de lançamento. Se tem três na borda, alguma coisa as soltou perto dali."
            },

            // =============================================================
            // GUILDA DOS SUCATEIROS — linha própria, em 4 degraus.
            // Task 9: "quest de guilda com mais ênfase na história".
            // A linha existe pra entregar, em doses, a informação que o
            // Enredo do Ato 2 vai precisar: a torre, a Artemis, e por que
            // droids offline valem tanto.
            // =============================================================
            [IdGuildaIniciacao] = new DefinicaoDeQuest
            {
                Id = IdGuildaIniciacao,
                Titulo = "Guilda I — Ferro velho e gente velha",
                NpcQueOferece = CatalogoDeNpcs.IdMestraDaGuilda,
                EhDeGuilda = true,
                FlagNecessaria = "ato1_sessao2",
                Resumo = "A Guilda dos Sucateiros não aceita quem não trouxe nada. Traga metal que preste.",
                TextoDeOferta = "Guilda não é clube. É lista de quem a cidade procura quando precisa de alguma coisa achada. Traz cinco de sucata limpa e eu escrevo teu nome.",
                TextoDeEntrega = "Ela pega um pedaço de metal, dobra com a mão, e devolve torto. — Teu nome tá na lista. Não me faz apagar.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdSucataLimpa, Quantidade = 5, Descricao = "Entregar 5 de Sucata Limpa" }
                },
                XpRecompensa = 120, GoldRecompensa = 30,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdEmblemaDaGuilda, Quantidade = 1 }
                },
                FlagAoConcluir = "guilda_membro",
                Curiosidade = "A Guilda dos Sucateiros existe desde antes da Artemis. Na época, era sindicato de coleta seletiva."
            },

            [IdGuildaCacada] = new DefinicaoDeQuest
            {
                Id = IdGuildaCacada,
                Titulo = "Guilda II — O que anda na Cratera",
                NpcQueOferece = CatalogoDeNpcs.IdMestraDaGuilda,
                EhDeGuilda = true,
                FlagNecessaria = "guilda_membro",
                Resumo = "A Guilda quer saber por que os droids selvagens da Cratera estão descendo mais perto da cidade do que deveriam.",
                TextoDeOferta = "Selvagem não desce. Nunca desceu. Agora desce. Mata uns cinco e me traz o que eles tiverem dentro — quero ver o que eles andaram comendo.",
                TextoDeEntrega = "— Cobre. Cabo de rede velho, do tipo que não se usa mais desde 2050. — Ela larga o punhado na mesa. — Eles estão desenterrando alguma coisa lá.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Derrotar, Alvo = CatalogoDeInimigos.IdDroidSelvagem, Quantidade = 5, Descricao = "Derrotar 5 Droids Selvagens" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdFioDeCobre, Quantidade = 3, Descricao = "Trazer 3 Fios de Cobre da Cratera" }
                },
                XpRecompensa = 250, GoldRecompensa = 80,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdGranadaDeAcido, Quantidade = 2 }
                },
                FlagAoConcluir = "guilda_cacada_ok",
                Curiosidade = "Cabo de rede de 2050 não conduz energia — conduz dados. Se os selvagens estão desenterrando cabo de dados, alguma coisa embaixo da Cratera ainda está transmitindo."
            },

            [IdGuildaOAlfa] = new DefinicaoDeQuest
            {
                Id = IdGuildaOAlfa,
                Titulo = "Guilda III — O Alfa",
                NpcQueOferece = CatalogoDeNpcs.IdMestraDaGuilda,
                EhDeGuilda = true,
                FlagNecessaria = "guilda_cacada_ok",
                Resumo = "Alguma coisa está organizando os selvagens da Cratera. A Guilda quer aquilo desmontado.",
                TextoDeOferta = "Selvagem não se organiza. Tem um maior que os outros lá, e os outros andam atrás dele. Isso não é fauna. Isso é comando.",
                TextoDeEntrega = "Ela examina o sensor óptico que você trouxe por muito tempo, virando na luz. — Isso aqui é peça da Apex. Lixada por cima, mas é. Alguém montou aquele bicho.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Derrotar, Alvo = CatalogoDeInimigos.IdDroidSelvagemAlfa, Quantidade = 1, Descricao = "Derrotar o Selvagem Alfa" }
                },
                XpRecompensa = 350, GoldRecompensa = 120,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdPocaoGrande, Quantidade = 2 },
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdGiroscopio, Quantidade = 1 }
                },
                FlagAoConcluir = "guilda_alfa_ok",
                Curiosidade = "Um droid selvagem que espera antes de atacar não é um droid selvagem. É um droid com instrução."
            },

            [IdGuildaOQueAGuildaSabe] = new DefinicaoDeQuest
            {
                Id = IdGuildaOQueAGuildaSabe,
                Titulo = "Guilda IV — O que a Guilda sabe",
                NpcQueOferece = CatalogoDeNpcs.IdMestraDaGuilda,
                EhDeGuilda = true,
                FlagNecessaria = "guilda_alfa_ok",
                Resumo = "A Mestra vai contar o que a Guilda sabe sobre a torre. Ela quer o Emblema de volta na mesa antes de falar.",
                TextoDeOferta = "Senta. Põe o emblema na mesa. O que eu vou te dizer, eu digo pra membro — e membro pode devolver o emblema depois, se não quiser mais carregar.",
                TextoDeEntrega = "— A torre não ficou mais alta. A gente é que foi ficando mais perto de entender o que ela é. Ela cresce porque alguém continua construindo. E quem constrói, constrói pra guardar alguma coisa lá dentro.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdMestraDaGuilda, Quantidade = 1, Descricao = "Ouvir o que a Mestra tem a dizer" }
                },
                XpRecompensa = 200, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdChaveDeAcesso, Quantidade = 1 }
                },
                FlagAoConcluir = "guilda_verdade_ok",
                Curiosidade = "Antes da Artemis, a Guilda tinha 400 membros registrados. Hoje tem onze. A Mestra sabe o nome de todos os 400."
            }
        };

    public static DefinicaoDeQuest Obter(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return Todos.TryGetValue(id, out DefinicaoDeQuest q) ? q : null;
    }

    /// <summary>Todas as quests oferecidas por um NPC específico.</summary>
    public static List<DefinicaoDeQuest> DoNpc(string idNpc)
    {
        var lista = new List<DefinicaoDeQuest>();
        if (string.IsNullOrEmpty(idNpc)) return lista;

        foreach (DefinicaoDeQuest q in Todos.Values)
        {
            if (q.NpcQueOferece == idNpc) lista.Add(q);
        }
        return lista;
    }
}

public enum TipoDeObjetivo
{
    /// <summary>Falar com um NPC (Alvo = Id do NPC no CatalogoDeNpcs).</summary>
    Conversar,
    /// <summary>Vencer batalhas (Alvo = Id do inimigo no CatalogoDeInimigos).</summary>
    Derrotar,
    /// <summary>Ter N unidades no inventario (Alvo = Id do item). Consumido na entrega.</summary>
    Coletar,
    /// <summary>Resolver um puzzle de terminal (Alvo = Id no CatalogoDePuzzles).</summary>
    ResolverPuzzle,
    /// <summary>Uma flag de historia ficar ativa (Alvo = nome da flag).</summary>
    AlcancarFlag
}

public class ObjetivoDeQuest
{
    public TipoDeObjetivo Tipo;
    public string Alvo;
    public int Quantidade = 1;
    public string Descricao = "";
}

public class RecompensaDeItem
{
    public string IdItem;
    public int Quantidade = 1;
}

public class DefinicaoDeQuest
{
    public string Id;
    public string Titulo;
    public string Resumo = "";

    /// <summary>Id do NPC que oferece e recebe a entrega (CatalogoDeNpcs).</summary>
    public string NpcQueOferece;

    public string TextoDeOferta = "";
    public string TextoDeEntrega = "";

    /// <summary>Flag que precisa estar ativa pra quest sequer aparecer.</summary>
    public string FlagNecessaria = "";

    /// <summary>Flag gravada quando a quest e ENTREGUE.</summary>
    public string FlagAoConcluir = "";

    public List<ObjetivoDeQuest> Objetivos = new List<ObjetivoDeQuest>();

    public int XpRecompensa;
    public int GoldRecompensa;
    public List<RecompensaDeItem> ItensRecompensa = new List<RecompensaDeItem>();

    /// <summary>Linha principal: aparece no topo do diario, em destaque.</summary>
    public bool EhPrincipal = false;

    /// <summary>Linha da Guilda dos Sucateiros: agrupada separadamente.</summary>
    public bool EhDeGuilda = false;

    /// <summary>
    /// Aceita sozinha assim que FlagNecessaria fica ativa, sem precisar
    /// falar com ninguem. Usado so pelas principais -- a historia nao
    /// deveria depender do jogador lembrar de aceitar uma missao.
    /// </summary>
    public bool AceitaAutomaticamente = false;

    /// <summary>
    /// Pode ser refeita depois de entregue (quest de coleta/caça). Ao
    /// entregar, o progresso zera e a quest volta a ficar disponivel.
    /// </summary>
    public bool Repetivel = false;

    /// <summary>
    /// Curiosidade / lore liberada ao concluir. Fica guardada no diario --
    /// e a "informacao da historia" como recompensa, alem de XP/item/Gold.
    /// </summary>
    public string Curiosidade = "";
}
