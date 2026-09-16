using System.Collections.Generic;

/// <summary>
/// Falas do jogo, escritas em codigo em vez de digitadas no Inspector.
/// Task 4: "estrutura, fala de NPCs".
///
/// POR QUE SAIU DO INSPECTOR: na entrega anterior, cada GatilhoDeDialogo
/// carregava o proprio texto. Isso tem tres problemas praticos num TCC:
///   1. Revisar o roteiro exige abrir a Unity e clicar em cada NPC.
///   2. Texto em cena nao aparece em diff de commit -- e impossivel saber o
///      que mudou no roteiro entre duas versoes.
///   3. Perder a cena perde o roteiro.
/// Aqui o texto vive num .cs, versionado, revisavel num editor de texto.
///
/// COMO USAR: no GatilhoDeDialogo, preencha "Id Do Dialogo" em vez de
/// digitar as falas. O campo "falas" continua existindo e continua tendo
/// PRIORIDADE -- ou seja, dialogo escrito a mao no Inspector ainda funciona,
/// e nada do que voce ja montou quebra.
///
/// CONVENCAO DE ID: "{cena}_{momento}_{quem}". Ex: "city_s2_dara".
/// </summary>
public static class CatalogoDeDialogos
{
    // --- CasaLip (Sessao 1) ---
    public const string CasaIntroApollo = "casa_intro_apollo";
    public const string CasaPuzzleDica = "casa_puzzle_dica";
    public const string CasaPortaApollo = "casa_porta_apollo";

    // --- Ferrovale / Sessao 2 ---
    public const string CityS2Dara = "city_s2_dara";
    public const string CityS2Tacio = "city_s2_tacio";
    public const string CityS2HomemDaFila = "city_s2_homem_fila";
    public const string CityS2ApolloConclusao = "city_s2_apollo_conclusao";
    public const string CityS2Pipo = "city_s2_pipo";
    public const string CityS2Onofre = "city_s2_onofre";
    public const string CityS2Nilce = "city_s2_nilce";
    public const string CityS2Quirino = "city_s2_quirino";
    public const string CityS2Bruna = "city_s2_bruna";

    // --- Sessao 3 ---
    public const string GameS3ApolloGolpes = "game_s3_apollo_golpes";
    public const string GameS3AposCombate = "game_s3_apos_combate";

    // --- Sessao 4 (escolha) ---
    public const string GameS4Escolha = "game_s4_escolha";

    // --- Sessao 5 (Cratera) ---
    public const string GameS5ApolloCratera = "game_s5_apollo_cratera";
    public const string GameS5PainelEnergia = "game_s5_painel_energia";

    // --- Sessao 6 (estacao / Ivo) ---
    public const string GameS6IvoConfissao = "game_s6_ivo_confissao";
    public const string GameS6AposChefeFerro = "game_s6_pos_ferro";
    public const string GameS6AposChefeLotus = "game_s6_pos_lotus";
    public const string GameS6AposChefeDesgarrado = "game_s6_pos_desgarrado";

    // --- Sessao 7 (retorno) ---
    public const string CityS7DaraFerro = "city_s7_dara_ferro";
    public const string CityS7DaraLotus = "city_s7_dara_lotus";
    public const string CityS7DaraDesgarrado = "city_s7_dara_desgarrado";
    public const string CityS7ApolloRevelacao = "city_s7_apollo_revelacao";
    public const string CityS7Onofre = "city_s7_onofre";

    public static readonly Dictionary<string, List<FalaDeDialogo>> Todos =
        new Dictionary<string, List<FalaDeDialogo>>
        {
            // =============================================================
            // CASA DO LIP — Sessão 1
            // =============================================================
            [CasaIntroApollo] = Falas(
                A("Você errou a sequência de novo. Olha o valor que você guardou. Ele não muda sozinho."),
                A("Essa linha só roda uma vez. Você quer que o valor suba até encher, não só uma vez. Precisa repetir."),
                A("Abre o terminal — Esc, Terminal. Ou aperta E aqui no painel. Escreve o laço e me mostra.")
            ),
            [CasaPuzzleDica] = Falas(
                A("Começa a variável em zero. Depois usa while pra somar de dez em dez até chegar em cem."),
                A("Em Lua não precisa de ponto e vírgula. E todo while pede um 'do' e termina com 'end'.")
            ),
            [CasaPortaApollo] = Falas(
                A("Viu? Não foi tão difícil quanto você fingiu que ia ser."),
                A("O que você acabou de fazer comigo, você vai fazer de novo lá fora. Esc, Terminal. Não esquece.")
            ),

            // =============================================================
            // FERROVALE — Sessão 2
            // =============================================================
            [CityS2Dara] = Falas(
                N("Ela segura um balde vazio com as duas mãos, como se o peso dele ainda importasse."),
                F("Dara", "Cortaram a cota. Metade do que era ontem."),
                F("Dara", "E ninguém aqui quer ser o primeiro a dizer por quê.")
            ),
            [CityS2Tacio] = Falas(
                F("Senhor Tácio", "Água filtrada, mesmo preço de ontem. Por enquanto."),
                F("Senhor Tácio", "Se o poço secar de vez, nem esse preço vai existir. Compra o que der hoje.")
            ),
            [CityS2HomemDaFila] = Falas(
                N("Ele não se vira de imediato. Quando vira, há mais medo que raiva na voz."),
                F("Homem da fila", "Não falhou nada. Foi o Ivo que mexeu."),
                F("Homem da fila", "Fechou a estação. Ninguém entra, ninguém sai. Disse que vai \"renegociar\" com quem quiser ouvir.")
            ),
            [CityS2ApolloConclusao] = Falas(
                A("Renegociar com quem, Lip?"),
                A("Com a Apex."),
                A("A estação fica a leste, perto da Cratera. E a única coisa que a Apex realmente quer nessa cidade..."),
                A("...são Droids como eu. Os que ainda funcionam offline."),
                A("Se a gente vai até lá, não é conversa. É combate. Me programa antes de sair — Esc, Terminal.")
            ),
            [CityS2Pipo] = Falas(
                F("Pipo", "Eu sei onde tem peça boa. Sei mesmo. Ninguém repara na gente."),
                F("Pipo", "Tem uma coisa piscando lá longe, na torre. Vermelho. Meu pai disse pra não olhar."),
                F("Pipo", "Eu olho todo dia.")
            ),
            [CityS2Onofre] = Falas(
                F("Seu Onofre", "Antes tinha água na torneira. Torneira, garoto. Você gira e sai água."),
                F("Seu Onofre", "Ninguém acredita quando eu conto. Mas era assim."),
                F("Seu Onofre", "A estação foi feita em 2041. O Ivo mantém aquilo de pé com sucata há dezenove anos. Dezenove.")
            ),
            [CityS2Nilce] = Falas(
                F("Nilce", "Colchão limpo, porta que tranca, e eu não vi você entrar."),
                F("Nilce", "Descansa que teu droid conserta melhor descansado. E você também.")
            ),
            [CityS2Quirino] = Falas(
                F("Quirino", "Peça eu tenho. Garantia eu não dou."),
                F("Quirino", "Esse teu droid aí — a perna tá emperrada faz tempo. Servo travado. Precisa de um sobressalente compatível e eu não tenho."),
                F("Quirino", "Se achar um, traz. Eu instalo de graça, só pra ver funcionar.")
            ),
            [CityS2Bruna] = Falas(
                F("Mestra Bruna", "Guilda não é clube. É a lista de quem essa cidade procura quando precisa de alguma coisa achada."),
                F("Mestra Bruna", "Teu nome não tá na lista. Traz metal que preste e a gente conversa.")
            ),

            // =============================================================
            // SESSÃO 3
            // =============================================================
            [GameS3ApolloGolpes] = Falas(
                A("Espera. Se for confirmação de contato, em breve vem escolta. Coisas pequenas, mas armadas."),
                A("Então escuta com atenção, porque não vou ter tempo de explicar de novo lá fora."),
                A("Golpe de impacto: simples, direto, gasta pouco. É o que você aprende com droid.aprenderTecnica(\"Impacto\", 2)."),
                A("Golpe de sobrecarga: mais forte, mas me deixa vulnerável no turno seguinte. Nível de dano maior, custo maior."),
                A("E uma coisa que você esqueceu: eu não tenho mira nenhuma. DEX zero. Eu erro quase metade dos golpes."),
                A("Sobe isso antes de sair: droid.subirAtributo(\"Dex\", 3). Confia em mim."),
                A("Depois me mostra: droid.listarTecnicas().")
            ),
            [GameS3AposCombate] = Falas(
                N("Quando a poeira assenta, restam só carcaças desmontadas na terra batida."),
                A("Funcionou. O que você escreveu no terminal virou isso aqui."),
                A("Guarda essa sensação. Vai ser assim o resto do caminho.")
            ),

            // =============================================================
            // SESSÃO 4 — A ESCOLHA
            // =============================================================
            [GameS4Escolha] = Falas(
                N("Duas figuras observam de uma elevação de sucata próxima. Longe o bastante pra não terem se envolvido, perto o bastante pra terem visto tudo."),
                F("Rasha", "Bom trabalho. Raramente vejo um Droid civil aguentar isso sozinho. Alguém programou bem esse aí."),
                F("Rasha", "Meu nome é Rasha. Flor de Ferro. Estamos recrutando quem já provou que não vai desmoronar na primeira patrulha."),
                F("Teodoro", "Eu sou Teodoro. Lótus Branca. E antes que ela continue vendendo a proposta dela, eu vou te pedir uma coisa que ela não vai: pensa direito antes de escolher qualquer lado."),
                F("Teodoro", "A Flor de Ferro luta pra virar o jogo. Supremacia. Mas o preço é que, no fim, vocês também vão precisar se tornar aquilo que estão combatendo."),
                F("Teodoro", "A Lótus acredita que existe diferença entre a Artemis e as máquinas que ela comanda. É mais lento. É mais frustrante."),
                F("Rasha", "É ingênuo."),
                A("E se ele não quiser nenhum dos dois?"),
                F("Teodoro", "Então ele segue sozinho. Mais difícil. Mais livre, talvez, dependendo de como você olha."),
                F("Rasha", "Então? O que vai ser?")
            ),

            // =============================================================
            // SESSÃO 5 — CRATERA
            // =============================================================
            [GameS5ApolloCratera] = Falas(
                N("A terra batida vira um mosaico de metal derretido e cristalizado. O ar tem gosto metálico, quase elétrico."),
                A("Dizem que foi aqui que a Artemis testou as primeiras armas de contenção. A terra nunca mais cresceu nada além de ferrugem."),
                A("O que anda aqui não é Apex. É pior: são máquinas que aprenderam sozinhas a caçar energia. Imprevisíveis."),
                A("Contato às nove horas. Recomendo cautela — não sabemos como esse reage.")
            ),
            [GameS5PainelEnergia] = Falas(
                A("Painel de energia. A passagem só libera com carga suficiente."),
                A("Carrega até cinquenta e só então marca porta_aberta = 1. Se marcar antes, o painel rejeita."),
                A("Duas coisas juntas agora: o laço que você já sabe, e uma condição. if energia >= 50 then ... end.")
            ),

            // =============================================================
            // SESSÃO 6 — ESTAÇÃO
            // =============================================================
            [GameS6IvoConfissao] = Falas(
                N("A estação está escura, iluminada só pelo brilho intermitente de painéis antigos. Ivo está sentado no chão, entre cabos arrancados."),
                F("Ivo", "Não devia ter vindo. Nenhum de vocês devia ter vindo."),
                A("Ivo, o que você fez?"),
                F("Ivo", "O que eu fiz? Eu sobrevivi. É o que eu fiz."),
                F("Ivo", "Eu ofereci dados. Rotas de patrulha antigas, mapas de sucata, coisa que a Apex já sabia de qualquer jeito. Em troca, prometeram deixar a estação funcionando."),
                F("Ivo", "Mas eles sabem que existem Droids offline aqui. Droids como o seu."),
                F("Ivo", "Eu não ia entregar ele. Juro que não ia. Mas quando perceberam que eu estava enrolando... mandaram a escolta."),
                N("O droid improvisado ao lado dele se move, um gesto quase protetor, quase desesperado."),
                F("Ivo", "Sinto muito. Mas eu não posso deixar vocês saírem daqui sem nada. Não depois de tudo."),
                A("Esse tem alguém pensando do outro lado. Vai reagir aos seus padrões. Não repita a mesma sequência de golpes duas vezes seguidas.")
            ),
            [GameS6AposChefeFerro] = Falas(
                N("O droid improvisado cai de joelhos. Ivo não se move do lugar."),
                F("Ivo", "Acabou. Vocês podem me entregar pra Apex agora, se quiserem. Provavelmente é o que eu mereço."),
                N("— Ninguém vai entregar ninguém. Você errou. Mas você não é eles."),
                F("Ivo", "Leva o que restou de equipamento daqui. E avisa a cidade. Digam que fui eu.")
            ),
            [GameS6AposChefeLotus] = Falas(
                N("A Ressonância dispara — um pulso branco que percorre o droid improvisado. As lâminas param a centímetros do Apollo."),
                F("Ivo", "Ele... ele parou. Vocês pararam ele sem quebrar nada."),
                A("Temporário. Mas é tempo suficiente pra você desligar ele direito, se quiser."),
                F("Ivo", "Vocês podiam ter me matado. Ou pior, matado ele. Por que não fizeram?"),
                N("— Porque destruir você não ia trazer água de volta pra cidade."),
                F("Ivo", "Vou consertar isso. Toda a estação. E vou contar pra cidade a verdade, do meu jeito.")
            ),
            [GameS6AposChefeDesgarrado] = Falas(
                N("O combate se estendeu mais do que devia. Sem módulo, sem núcleo, sem atalho."),
                F("Ivo", "Vocês vieram sozinhos. Sem a Ferro. Sem a Lótus."),
                N("— Viemos porque essa cidade é nossa também."),
                A("Sem atalho dessa vez. Fizemos do nosso jeito."),
                F("Ivo", "Vocês não precisaram de ninguém."),
                N("— A gente nunca precisou. Só precisava de tempo pra aprender a fazer sozinho.")
            ),

            // =============================================================
            // SESSÃO 7 — RETORNO
            // =============================================================
            [CityS7DaraFerro] = Falas(
                N("Dara está na entrada da praça, o balde ainda na mão."),
                F("Dara", "Dizem que teve luta. Que o droid do Ivo não anda mais."),
                F("Dara", "A água vai voltar. É o que importa, eu acho. É o que eu vou repetir pra mim mesma.")
            ),
            [CityS7DaraLotus] = Falas(
                N("Dara olha pra você com uma expressão que ele nunca tinha visto nela antes."),
                F("Dara", "O Ivo mesmo veio contar. Ele mesmo."),
                F("Dara", "Faz muito tempo que ninguém nessa cidade conserta uma coisa sem quebrar outra.")
            ),
            [CityS7DaraDesgarrado] = Falas(
                F("Dara", "O boato que corre não fala da Ferro nem da Lótus."),
                F("Dara", "Fala do garoto e do droid dele. Só isso."),
                F("Dara", "Acho que é assim que começa.")
            ),
            [CityS7ApolloRevelacao] = Falas(
                N("Na soleira de casa, o gerador zunindo baixo ao fundo, como sempre."),
                A("Antes da luta, quando escaneei o droid do Ivo..."),
                A("Encontrei um resquício de assinatura de rede. Fraco, quase apagado, mas reconhecível."),
                A("Não era protocolo comum da Apex. Era rastreamento direto. Do tipo que a Artemis usa pra localizar Droids específicos."),
                A("Alguém, em algum lugar da rede da Artemis, estava procurando por mim. Não por \"um Droid offline qualquer\". Por mim."),
                N("Ao longe, além da cerca de sucata, a torre pisca sua luz vermelha fraca — como sempre. Só que agora, pela primeira vez, ela parece estar olhando de volta.")
            ),
            [CityS7Onofre] = Falas(
                F("Seu Onofre", "Você foi lá. Na Cratera."),
                F("Seu Onofre", "Sabe o que tinha lá antes da Cratera ser Cratera? Nada. Terra. Mato até."),
                F("Seu Onofre", "Aí um dia veio a luz, e depois disso a terra não segura mais nada. Dizem que foi teste."),
                F("Seu Onofre", "Teste de quê, garoto? Teste pra usar onde depois?")
            )
        };

    /// <summary>
    /// Devolve as falas de um Id, ou null se não existir. Quem chama decide
    /// o que fazer (o GatilhoDeDialogo loga um aviso e não dispara).
    /// </summary>
    public static List<FalaDeDialogo> Obter(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return Todos.TryGetValue(id, out List<FalaDeDialogo> falas) ? falas : null;
    }

    // --- Açúcar sintático pra deixar o roteiro legível acima ---

    private static List<FalaDeDialogo> Falas(params FalaDeDialogo[] falas) =>
        new List<FalaDeDialogo>(falas);

    /// <summary>Fala do Apollo.</summary>
    private static FalaDeDialogo A(string texto) =>
        new FalaDeDialogo { falante = "Apollo", texto = texto };

    /// <summary>Narração (sem nome de falante).</summary>
    private static FalaDeDialogo N(string texto) =>
        new FalaDeDialogo { falante = "", texto = texto };

    /// <summary>Fala de qualquer outro personagem.</summary>
    private static FalaDeDialogo F(string quem, string texto) =>
        new FalaDeDialogo { falante = quem, texto = texto };
}
