using System.Collections.Generic;
using DroidsCode.DroidCore;
using DroidsCode.Combat;

/// <summary>
/// Catalogo de missoes. VERSAO 2 -- alinhada ao TCC_-_ENREDO_v2.md.
///
/// Mesmo padrao dos outros catalogos do projeto (Dictionary com Id string
/// estavel, Obter devolvendo null). TUDO aqui e DADO, nao comportamento --
/// quem executa e o GerenciadorDeQuests. Nenhuma classe mudou nesta versao,
/// entao nenhum script de cena precisa ser tocado: so entraram entradas
/// novas e textos melhores.
///
/// PROGRESSO E PERSISTENCIA: cada quest usa flags e contadores do
/// GerenciadorDeEstado, ambos ja persistidos no save.
///   flag  "quest_{id}_ativa"      -> aceita
///   flag  "quest_{id}_concluida"  -> entregue
///   contador "quest_{id}_obj_{n}" -> progresso do objetivo n
///
/// O QUE MUDOU NA V2
/// -----------------
/// 1. Toda quest secundaria agora paga alguma coisa da HISTORIA, e nao so
///    XP. O campo Curiosidade deixou de ser trivia e virou o canal por onde
///    o Ato 1 entrega a informacao que o Ato 2 vai cobrar.
/// 2. Entraram as quests que fecham os arcos que o enredo v1 abria e nunca
///    pagava: o irmao da Dara, o cobre do Tacio, o quarto vago da Nilce, as
///    historias do Onofre, e a divida que a Sula contrai na Sessao 7.
/// 3. As quests secundarias usam estrutura de quatro tempos (introducao,
///    desenvolvimento, virada, conclusao) em vez de jornada do heroi. A
///    virada dispensa conflito -- ver Parte II.4 do enredo.
/// 4. Regra de escopo: nenhuma quest secundaria e obrigatoria para terminar
///    o Ato 1. Quem corre termina; quem explora entende.
/// </summary>
public static class CatalogoDeQuests
{
    // --- Ferrovale / secundarias ---
    public const string IdAguaParaDara = "q_agua_dara";
    public const string IdCobreDoTacio = "q_cobre_tacio";
    public const string IdAPernaDoApollo = "q_perna_apollo";
    public const string IdLimpezaDaBorda = "q_limpeza_borda";

    // --- Ferrovale / NOVAS na v2 ---
    public const string IdATerceiraParte = "q_dara_terceira";
    public const string IdDezoitoQuilometros = "q_tacio_linha";
    public const string IdAPecaDoPipo = "q_pipo_peca";
    public const string IdOQueTinhaAntes = "q_onofre_provas";
    public const string IdOQuartoVago = "q_nilce_quarto";
    public const string IdOQueASulaVendeu = "q_sula_divida";
    public const string IdTodaTerca = "q_ivo_terca";

    // --- Guilda dos Sucateiros (linha propria, ver EhDeGuilda) ---
    public const string IdGuildaIniciacao = "q_guilda_1_iniciacao";
    public const string IdGuildaCacada = "q_guilda_2_cacada";
    public const string IdGuildaOAlfa = "q_guilda_3_alfa";
    public const string IdGuildaOQueAGuildaSabe = "q_guilda_4_verdade";

    // --- Linha principal (espelham o roteiro, dao XP de acerto) ---
    public const string IdPrincipalDescobrirAgua = "q_main_agua";
    public const string IdPrincipalAEstacao = "q_main_estacao";
    public const string IdPrincipalONomeNaLista = "q_main_partida";

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
                Resumo = "A fila do poço está maior que o normal e ninguém quer dizer por quê. Três pessoas vão te dar três versões incompatíveis. A verdade é o que sobra depois de ouvir as três.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdDara, Quantidade = 1, Descricao = "Falar com a Dara na fila do poço" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdHomemDaFila, Quantidade = 1, Descricao = "Falar com o homem da fila" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdTacio, Quantidade = 1, Descricao = "Falar com o Senhor Tácio" }
                },
                XpRecompensa = 120, GoldRecompensa = 15,
                FlagAoConcluir = "ato1_sessao2",
                Curiosidade = "As três versões não se contradizem por acaso. O homem da fila acha que foi traição, o Tácio sabe que cota não é botão, e a Mestra Bruna reparou que o drone passou quarenta minutos mais cedo dois dias seguidos. Ninguém está mentindo. Cada um viu um pedaço."
            },

            [IdPrincipalAEstacao] = new DefinicaoDeQuest
            {
                Id = IdPrincipalAEstacao,
                Titulo = "A estação de captação",
                NpcQueOferece = CatalogoDeNpcs.IdApollo,
                EhPrincipal = true,
                AceitaAutomaticamente = true,
                FlagNecessaria = "ato1_sessao3",
                Resumo = "O Ivo fechou a estação e trouxe a Apex até aqui. Atravesse a Cratera e resolva isso — de um jeito ou de outro.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.AlcancarFlag, Alvo = "ato1_faccao_escolhida", Quantidade = 1, Descricao = "Decidir sobre Rasha e Teodoro" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.ResolverPuzzle, Alvo = CatalogoDePuzzles.IdPainelDeEnergia, Quantidade = 1, Descricao = "Religar o painel de energia da Cratera" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Derrotar, Alvo = CatalogoDeInimigos.IdDroidDoIvo, Quantidade = 1, Descricao = "Enfrentar o droid do Ivo" }
                },
                XpRecompensa = 400, GoldRecompensa = 60,
                FlagAoConcluir = "ato1_sessao6",
                Curiosidade = "O droid do Ivo não tinha nenhuma salvaguarda de combate — mas tinha três lentes de leitura profunda alinhadas com precisão de micrômetro. Ninguém gasta isso pra brigar. Ele montou aquilo pra procurar uma coisa específica, e procurou por dezenove anos."
            },

            [IdPrincipalONomeNaLista] = new DefinicaoDeQuest
            {
                Id = IdPrincipalONomeNaLista,
                Titulo = "O nome na lista",
                NpcQueOferece = CatalogoDeNpcs.IdApollo,
                EhPrincipal = true,
                AceitaAutomaticamente = true,
                FlagNecessaria = "ato1_sessao6",
                Resumo = "A água voltou. Uma mulher de caderneta está no portão com uma oferta boa demais, e o preço dela tem nome e sobrenome.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.AlcancarFlag, Alvo = "ato1_agua_voltou", Quantidade = 1, Descricao = "Ver a água voltar na praça" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdNadir, Quantidade = 1, Descricao = "Ouvir a proposta da Intermediária" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.AlcancarFlag, Alvo = "ato1_cidade_respondeu", Quantidade = 1, Descricao = "Ver como Ferrovale responde" }
                },
                XpRecompensa = 500, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdKitDeAcampamento, Quantidade = 1 }
                },
                FlagAoConcluir = "ato1_concluido",
                Curiosidade = "A Nadir não mentiu em nenhum momento da assembleia. O número era verdadeiro, o prazo era verdadeiro, e ela teria cumprido. É exatamente por isso que funciona."
            },

            // =============================================================
            // SECUNDÁRIAS DE FERROVALE
            // Estrutura de quatro tempos: o jogador junta uma coisa, junta
            // mais, descobre PRA QUE era, e decide o que fazer com isso.
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
                Curiosidade = "Ela diz \"duas famílias do beco\", mas divide em três partes. Ninguém nunca perguntou pra quem é a terceira."
            },

            [IdATerceiraParte] = new DefinicaoDeQuest
            {
                Id = IdATerceiraParte,
                Titulo = "A terceira parte",
                NpcQueOferece = CatalogoDeNpcs.IdDara,
                FlagNecessaria = "quest_dara_filtro_ok",
                Resumo = "A Dara guarda um terço da cota todo dia, há dois anos, e nunca disse pra quem. Talvez esteja na hora de alguém perguntar.",
                TextoDeOferta = "Você perguntou. Ninguém pergunta. — Ela demora. — Meu irmão. Emílio. Levaram em novembro de setenta e sete e eu continuo separando a parte dele.",
                TextoDeEntrega = "— Eu sei que ele não volta. Eu sei desde o primeiro inverno. — Ela olha o cantil. — Mas eu queria que ficasse cheio de alguma coisa que não fosse espera.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdSucataLimpa, Quantidade = 4, Descricao = "Juntar 4 de Sucata Limpa pra consertar a cisterna do beco" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdDara, Quantidade = 1, Descricao = "Perguntar pra quem é a terceira parte" }
                },
                XpRecompensa = 160, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdReparoCompleto, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_dara_terceira_ok",
                Curiosidade = "Captura da Apex não mata. Registra e realoca. Isso significa que, em algum lugar, existe uma linha num sistema com o nome do Emílio — e é isso que torna a espera da Dara racional, e por isso pior."
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

            [IdDezoitoQuilometros] = new DefinicaoDeQuest
            {
                Id = IdDezoitoQuilometros,
                Titulo = "Dezoito quilômetros",
                NpcQueOferece = CatalogoDeNpcs.IdTacio,
                FlagNecessaria = "quest_tacio_cobre_ok",
                Resumo = "Depois da terceira entrega, o Tácio para de fingir que é negócio e mostra o que está fazendo com o cobre.",
                TextoDeOferta = "Vem cá atrás. Você já trouxe cobre demais pra eu continuar mentindo que é pra revender.",
                TextoDeEntrega = "— Falta seis quilômetro. Eu sei que ela não tá lá. Sei desde sessenta e um. — Ele enrola o fio no carretel. — Mas o dia que a linha fechar, eu vou poder discar. E aí eu paro.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdFioDeCobre, Quantidade = 10, Descricao = "Trazer mais 10 Fios de Cobre" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.ResolverPuzzle, Alvo = CatalogoDePuzzles.IdContadorDeSucata, Quantidade = 1, Descricao = "Contar o rolo emendado com o terminal" }
                },
                XpRecompensa = 220, GoldRecompensa = 40,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdSensorOptico, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_tacio_linha_ok",
                Curiosidade = "Ele está refazendo, fio por fio, a linha telefônica de dezoito quilômetros até o povoado onde a filha dele morava em 2060. Doze quilômetros já estão emendados. Ele trabalha nisso todas as noites desde então."
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
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdSucataLimpa, Quantidade = 3, Descricao = "Juntar 3 de Sucata Limpa pro reparo" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdFerreiro, Quantidade = 1, Descricao = "Levar tudo pro Quirino" }
                },
                XpRecompensa = 200, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdServoMotor, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_apollo_perna_ok",
                Curiosidade = "O Apollo foi montado quando o Lip tinha treze anos, com peças de quatro droids diferentes. A perna sempre foi a parte pior — era o que sobrou. O Quirino não cobrou nada, não aceitou ajuda e não deixou ninguém olhar enquanto trabalhava."
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
                Curiosidade = "Sucatas de captura não se afastam mais que dois quilômetros do ponto de lançamento. Se tem três na borda, alguma coisa as soltou perto dali — e não foi a estação, que fica a nove."
            },

            [IdAPecaDoPipo] = new DefinicaoDeQuest
            {
                Id = IdAPecaDoPipo,
                Titulo = "A peça que o Pipo viu",
                NpcQueOferece = CatalogoDeNpcs.IdCriancaDaRua,
                FlagNecessaria = "ato1_sessao2",
                Resumo = "O Pipo jura que sabe onde tem uma peça boa. A peça não existe. O que ele sabe de verdade vale mais.",
                TextoDeOferta = "Uma bateria pequena e eu te levo lá. Palavra de sucateiro. Eu ia ser da Guilda se eles deixassem criança.",
                TextoDeEntrega = "— Tá, não tinha peça. — Ele chuta a poeira. — Mas eu sei onde passa o drone. Eu conto de graça, porque você não gritou comigo.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdCriancaDaRua, Quantidade = 1, Descricao = "Ir com o Pipo até a \"peça boa\"" }
                },
                XpRecompensa = 60, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdSinalizadorDeRetorno, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_pipo_ok",
                Curiosidade = "O horário de passagem do drone que o Pipo decorou está certo ao minuto. Ninguém repara numa criança — e é exatamente por isso que ela vê tudo."
            },

            [IdOQueTinhaAntes] = new DefinicaoDeQuest
            {
                Id = IdOQueTinhaAntes,
                Titulo = "O que tinha antes da Cratera",
                NpcQueOferece = CatalogoDeNpcs.IdVelhoDoViaduto,
                FlagNecessaria = "ato1_faccao_escolhida",
                Resumo = "Seu Onofre conta a mesma história de jeitos diferentes, e ninguém acredita nele. Traga três provas e ele para de precisar ser acreditado.",
                TextoDeOferta = "Tinha rua lá. Rua com nome. Ninguém acredita em velho, e eu não culpo. Me traz alguma coisa que fale por mim.",
                TextoDeEntrega = "Ele passa o dedo na placa derretida até achar o L de Luzia. Não diz nada por um tempo. — Pronto. Agora tem prova. Agora é história, não é caso de velho.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.AlcancarFlag, Alvo = "ato1_cratera_verdade", Quantidade = 1, Descricao = "Encontrar as três marcas na Cratera" }
                },
                XpRecompensa = 180, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdPocaoGrande, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_onofre_ok",
                Curiosidade = "Não foi teste de arma. Quatro povoados foram consolidados num ponto único de distribuição em 2061, e a infraestrutura dos quatro foi derretida no lugar pra que ninguém voltasse. O que parece campo de batalha é aterro de cidade."
            },

            [IdOQuartoVago] = new DefinicaoDeQuest
            {
                Id = IdOQuartoVago,
                Titulo = "O quarto que não está quebrado",
                NpcQueOferece = CatalogoDeNpcs.IdEstalajadeira,
                FlagNecessaria = "ato1_sessao3",
                Resumo = "A Nilce mantém um quarto sempre vago e diz que está quebrado. Não está.",
                TextoDeOferta = "O três tá quebrado. Sempre esteve. — Ela não olha na cara de quem pergunta. — Se você quiser ajudar em alguma coisa, ajuda na bomba do fundo, essa aí quebrou de verdade.",
                TextoDeEntrega = "— Obrigada. — E depois, muito depois, quando você já estava saindo: — O três não tá quebrado, não. É pra quem chegar correndo de noite. Sempre tem alguém chegando correndo de noite.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.ResolverPuzzle, Alvo = CatalogoDePuzzles.IdBombaDeReserva, Quantidade = 1, Descricao = "Religar a bomba de reserva da estalagem" }
                },
                XpRecompensa = 140, GoldRecompensa = 25,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdKitDeAcampamento, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_nilce_ok",
                Curiosidade = "Nos últimos quatro anos, sete pessoas dormiram no quarto três. Nenhuma pagou. A Nilce não sabe o nome de cinco delas e nunca perguntou — é esse o serviço."
            },

            [IdTodaTerca] = new DefinicaoDeQuest
            {
                Id = IdTodaTerca,
                Titulo = "Toda terça",
                NpcQueOferece = CatalogoDeNpcs.IdIvo,
                FlagNecessaria = "ato1_sessao6",
                Resumo = "A bomba dois pede ajuste toda terça. O Ivo não vai conseguir fazer isso pra sempre, e os dois sabem.",
                TextoDeOferta = "Terça. Anota. Não confia no manômetro da direita, ele mente — sempre mentiu, desde antes de você nascer.",
                TextoDeEntrega = "Ele confere o teu ajuste sem falar. Mexe dois graus pra trás. — Tá bom. Tá quase igual ao jeito que ela fazia.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.ResolverPuzzle, Alvo = CatalogoDePuzzles.IdCalibragemDoBraco, Quantidade = 1, Descricao = "Calibrar a bomba dois pelo terminal" }
                },
                XpRecompensa = 130, GoldRecompensa = 35,
                Repetivel = true,
                FlagAoConcluir = "quest_ivo_terca_ok",
                Curiosidade = "Manutenção não é heroísmo e por isso ninguém escreve música sobre. A cidade bebeu por dezenove anos porque um velho foi até lá toda terça-feira, inclusive nas terças em que ele não queria ir."
            },

            [IdOQueASulaVendeu] = new DefinicaoDeQuest
            {
                Id = IdOQueASulaVendeu,
                Titulo = "O que a Sula vendeu",
                NpcQueOferece = CatalogoDeNpcs.IdVendedoraDePecas,
                FlagNecessaria = "ato1_nadir",
                Resumo = "A Intermediária sabia o número antes de ler em voz alta. Alguém contou. A Sula não vai negar se você perguntar direito.",
                TextoDeOferta = "Foi eu. — Ela não para de arrumar as peças na banca enquanto fala. — Três semanas atrás. Perguntaram se tinha droid offline na cidade e ofereceram um preço, e eu falei.",
                TextoDeEntrega = "— Eu não vou pedir desculpa porque desculpa não devolve nada. — Ela empurra a caixa pela banca. — Leva. E se um dia precisar sumir dessa cidade rápido, eu sei três saídas que não têm cerca.",
                Objetivos = new List<ObjetivoDeQuest>
                {
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Conversar, Alvo = CatalogoDeNpcs.IdVendedoraDePecas, Quantidade = 1, Descricao = "Perguntar pra Sula" }
                },
                XpRecompensa = 170, GoldRecompensa = 0,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdDisruptorEmp, Quantidade = 2 },
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdOleoDePrecisao, Quantidade = 1 }
                },
                FlagAoConcluir = "quest_sula_ok",
                Curiosidade = "Ela recebeu quatro semanas de água por pessoa da família. Fez a conta e aceitou. Vai continuar morando aqui, vendendo na mesma esquina, do lado de gente que sabe. Esse é o preço, e é maior que o pagamento."
            },

            // =============================================================
            // GUILDA DOS SUCATEIROS — linha própria, em 4 degraus.
            // Task 9: "quest de guilda com mais ênfase na história".
            // Na v2 essa linha deixou de ser conteúdo lateral: é por aqui
            // que entra, em doses, a informação que o Ato 2 vai cobrar —
            // o que tem embaixo da Cratera e o que a Torre é.
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
                Curiosidade = "A Guilda existe desde antes do Corte. Na época era sindicato de coleta seletiva, com quatrocentos nomes registrados. Os onze que sobraram estão riscados a ponta de prego na porta, e a Mestra recita todos eles toda manhã."
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
                Curiosidade = "Cabo de rede de 2050 não conduz energia — conduz dados. Se os selvagens estão desenterrando cabo de dados, alguma coisa embaixo da Cratera ainda está transmitindo. E máquina faminta não cava atrás de dado: cava atrás de sinal."
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
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Derrotar, Alvo = CatalogoDeInimigos.IdDroidSelvagemAlfa, Quantidade = 1, Descricao = "Derrotar o Selvagem Alfa" },
                    new ObjetivoDeQuest { Tipo = TipoDeObjetivo.Coletar, Alvo = CatalogoDeItens.IdSensorOptico, Quantidade = 1, Descricao = "Trazer o sensor óptico dele" }
                },
                XpRecompensa = 350, GoldRecompensa = 120,
                ItensRecompensa = new List<RecompensaDeItem>
                {
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdPocaoGrande, Quantidade = 2 },
                    new RecompensaDeItem { IdItem = CatalogoDeItens.IdGiroscopio, Quantidade = 1 }
                },
                FlagAoConcluir = "guilda_alfa_ok",
                Curiosidade = "Um droid selvagem que espera antes de atacar não é um droid selvagem. É um droid com instrução. O Alfa não foi montado pra caçar gente: foi montado pra manter os outros cavando no mesmo lugar."
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
                TextoDeEntrega = "— A torre não ficou mais alta. A gente é que foi ficando perto o bastante pra entender o que ela é. Ela cresce porque ninguém nunca tirou nada de dentro. Cada camada em cima da anterior, porque tirar exigia ter certeza, e ter certeza exigia alguém que lesse o que estava embaixo.",
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
                Curiosidade = "Antes do Corte, a Guilda tinha 400 membros registrados. Hoje tem onze. A Mestra sabe o nome de todos os 400 — e o que ela não diz em voz alta é que a lista de 400 nomes é, hoje, o registro mais completo de quem morava aqui que existe fora dos sistemas da Apex."
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

    /// <summary>Linha principal, pro topo do diário.</summary>
    public static List<DefinicaoDeQuest> Principais()
    {
        var lista = new List<DefinicaoDeQuest>();
        foreach (DefinicaoDeQuest q in Todos.Values)
        {
            if (q.EhPrincipal) lista.Add(q);
        }
        return lista;
    }

    /// <summary>Linha da Guilda, agrupada separadamente no quadro.</summary>
    public static List<DefinicaoDeQuest> DaGuilda()
    {
        var lista = new List<DefinicaoDeQuest>();
        foreach (DefinicaoDeQuest q in Todos.Values)
        {
            if (q.EhDeGuilda) lista.Add(q);
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
    /// Na v2 este campo carrega a informacao que o Ato 2 vai cobrar: quem
    /// faz as secundarias entra no Ato 2 sabendo o que a Torre e.
    /// </summary>
    public string Curiosidade = "";
}
