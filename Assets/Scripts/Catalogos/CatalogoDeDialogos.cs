using System.Collections.Generic;

/// <summary>
/// Falas do jogo, escritas em codigo em vez de digitadas no Inspector.
/// Task 4: "estrutura, fala de NPCs". VERSAO 2 -- reescrita a partir do
/// TCC_-_ENREDO_v2.md.
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
///
/// O QUE MUDOU NA V2
/// -----------------
/// 1. Lip fala. Na v1, varias falas do protagonista estavam marcadas como
///    narracao -- o painel mostrava "— Ninguem vai entregar ninguem" sem
///    falante. Agora existe o auxiliar L(...) e Lip aparece com o nome dele.
/// 2. Ninguem explica o mundo para quem ja mora nele. Morador nao diz
///    "a Artemis, a IA que tomou o controle em 2060". Diz "desde o Corte".
/// 3. Ivo mente primeiro. A confissao dele foi quebrada em tres dialogos
///    encadeados (confissao -> celia -> apex), porque uma pessoa desesperada
///    nao entrega motivacao, metodo e arrependimento de uma vez so.
/// 4. Entraram os Ids novos do Ato 1 v2: as plantas da Sessao 1, a casa
///    vazia do Ivo, a recusa do Apollo, a unidade de manutencao, o preco
///    pago na Sessao 6 e a chegada da Nadir na Sessao 7.
/// 5. Regra de tamanho: uma fala = uma linha do painel. Se nao cabe, e duas
///    falas. Le em voz alta antes de commitar.
///
/// TODOS OS IDS DA V1 CONTINUAM EXISTINDO COM O MESMO NOME. Nada do que ja
/// estava apontado numa cena para de funcionar.
/// </summary>
public static class CatalogoDeDialogos
{
    // --- CasaLip (Sessao 1) ---
    public const string CasaIntroApollo = "casa_intro_apollo";
    public const string CasaPuzzleDica = "casa_puzzle_dica";
    public const string CasaPortaApollo = "casa_porta_apollo";
    public const string CasaFiltro = "casa_filtro";                 // NOVO
    public const string CasaCaneca = "casa_caneca";                 // NOVO
    public const string CasaCaderno = "casa_caderno";               // NOVO
    public const string CasaNoiteApollo = "casa_noite_apollo";      // NOVO (planta 1)
    public const string CityS1Mirante = "city_s1_mirante";          // NOVO

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
    public const string CityS2BrunaDrone = "city_s2_bruna_drone";   // NOVO
    public const string CityS2Sula = "city_s2_sula";                // NOVO
    public const string CityS2CasaIvoCadernos = "city_s2_casa_ivo_cadernos"; // NOVO
    public const string CityS2CasaIvoFoto = "city_s2_casa_ivo_foto";         // NOVO
    public const string CityS2CasaIvoBilhete = "city_s2_casa_ivo_bilhete";   // NOVO
    public const string CityS2ApolloPortao = "city_s2_apollo_portao";        // NOVO

    // --- Sessao 3 ---
    public const string CityS3Sino = "city_s3_sino";                // NOVO
    public const string CityS3RecusaApollo = "city_s3_recusa_apollo"; // NOVO
    public const string GameS3ApolloGolpes = "game_s3_apollo_golpes";
    public const string GameS3AposCombate = "game_s3_apos_combate";

    // --- Sessao 4 (escolha) ---
    public const string GameS4Escolha = "game_s4_escolha";

    // --- Sessao 5 (Cratera) ---
    public const string GameS5ApolloCratera = "game_s5_apollo_cratera";
    public const string GameS5PainelEnergia = "game_s5_painel_energia";
    public const string GameS5Placa = "game_s5_placa";                       // NOVO
    public const string GameS5Degrau = "game_s5_degrau";                     // NOVO
    public const string GameS5Cano = "game_s5_cano";                         // NOVO
    public const string GameS5RashaNucleo = "game_s5_rasha_nucleo";          // NOVO
    public const string GameS5TeodoroRessonancia = "game_s5_teodoro_ressonancia"; // NOVO
    public const string GameS5DesgarradoVazio = "game_s5_desgarrado_vazio";  // NOVO
    public const string GameS5Unidade = "game_s5_unidade_manutencao";        // NOVO (planta 7)
    public const string GameS5UnidadePainel = "game_s5_unidade_painel";      // NOVO

    // --- Sessao 6 (estacao / Ivo) ---
    public const string GameS6IvoConfissao = "game_s6_ivo_confissao";
    public const string GameS6IvoCelia = "game_s6_ivo_celia";                // NOVO
    public const string GameS6IvoApex = "game_s6_ivo_apex";                  // NOVO
    public const string GameS6Preco = "game_s6_preco";                       // NOVO
    public const string GameS6AposChefeFerro = "game_s6_pos_ferro";
    public const string GameS6AposChefeLotus = "game_s6_pos_lotus";
    public const string GameS6AposChefeDesgarrado = "game_s6_pos_desgarrado";
    public const string GameS6IvoEntrega = "game_s6_ivo_entrega";            // NOVO

    // --- Sessao 7 (retorno) ---
    public const string CityS7Agua = "city_s7_agua";                         // NOVO
    public const string CityS7QuirinoPerna = "city_s7_quirino_perna";        // NOVO
    public const string CityS7NadirChegada = "city_s7_nadir_chegada";        // NOVO
    public const string CityS7NadirOferta = "city_s7_nadir_oferta";          // NOVO
    public const string CityS7Assembleia = "city_s7_assembleia";             // NOVO
    public const string CityS7DaraFala = "city_s7_dara_fala";                // NOVO
    public const string CityS7NadirFerro = "city_s7_nadir_ferro";            // NOVO
    public const string CityS7NadirLotus = "city_s7_nadir_lotus";            // NOVO
    public const string CityS7NadirDesgarrado = "city_s7_nadir_desgarrado";  // NOVO
    public const string CityS7DaraFerro = "city_s7_dara_ferro";
    public const string CityS7DaraLotus = "city_s7_dara_lotus";
    public const string CityS7DaraDesgarrado = "city_s7_dara_desgarrado";
    public const string CityS7ApolloRevelacao = "city_s7_apollo_revelacao";
    public const string CityS7Mochila = "city_s7_mochila";                   // NOVO
    public const string CityS7DaraCantil = "city_s7_dara_cantil";            // NOVO
    public const string CityS7Partida = "city_s7_partida";                   // NOVO
    public const string CityS7Onofre = "city_s7_onofre";

    public static readonly Dictionary<string, List<FalaDeDialogo>> Todos =
        new Dictionary<string, List<FalaDeDialogo>>
        {
            // =============================================================
            // CASA DO LIP — Sessão 1
            // Objetivo: o jogador tem que gostar do Apollo em dois minutos,
            // e tem que perceber que falta uma pessoa nessa casa sem que
            // ninguém diga isso.
            // =============================================================
            [CasaIntroApollo] = Falas(
                A("Você errou a sequência de novo."),
                L("Eu sei que errei."),
                A("Você sabe que errou. Sabe onde?"),
                N("Ele não responde. É o tipo de silêncio que ele faz bem."),
                A("O valor está em dez. O gerador precisa de cem."),
                A("Você mandou somar uma vez. Ele obedeceu uma vez. A máquina não é preguiçosa, Lip. Ela é literal."),
                L("Então eu escrevo a linha dez vezes."),
                A("Pode. E quando forem mil?"),
                A("Você quer que o valor suba ENQUANTO não estiver cheio. Escreve isso. Não em código — em português."),
                L("Enquanto for menor que cem... soma dez."),
                A("Agora de novo, em Lua. while, condição, do. O que abriu fecha com end."),
                A("Esc, Terminal. Ou aperta E aqui no painel.")
            ),
            [CasaPuzzleDica] = Falas(
                A("Começa a variável em zero. Depois usa while pra somar de dez em dez até chegar em cem."),
                A("Em Lua não precisa de ponto e vírgula. Todo while pede um do e termina com end."),
                A("E se der erro: não é castigo, é informação. Lê a linha que ele te deu.")
            ),
            [CasaPortaApollo] = Falas(
                A("Viu? Não foi tão difícil quanto você fingiu que ia ser."),
                L("Foi difícil sim."),
                A("Foi. As duas coisas cabem."),
                A("O que você acabou de fazer comigo, você vai fazer de novo lá fora. Esc, Terminal. Não esquece.")
            ),

            // --- Objetos da casa: narrativa embutida, sem explicação ---
            [CasaFiltro] = Falas(
                N("Um filtro de água caseiro, bem-feito. Melhor que qualquer um do mercado."),
                A("Esse funciona."),
                N("Apollo não diz mais nada.")
            ),
            [CasaCaneca] = Falas(
                N("Uma caneca de metal esmaltado. Tem um nome quase apagado na base, impossível de ler."),
                N("Lip não usa, não lava e não joga fora.")
            ),
            [CasaCaderno] = Falas(
                N("Caderno de registro, capa dura, carimbo de uma prefeitura que não existe mais."),
                N("Cento e quarenta páginas de vazão e pressão, numa letra firme. A última entrada é de quatro anos atrás e termina no meio de uma linha."),
                N("Depois dela, outra letra, pior: rascunhos de código feitos à mão, riscados e refeitos."),
                A("Ela anotava a pressão da bomba duas vezes por dia. Você anota laço."),
                A("É o mesmo caderno."),
                L("..."),
                A("Não. Eu não vou falar sobre isso agora.")
            ),
            [CasaNoiteApollo] = Falas(
                N("Três e doze da manhã. Apollo está de pé no meio da cozinha, de frente para a parede."),
                N("Os dois pontos azuis piscam num ritmo que Lip nunca viu: três curtos, um longo, pausa."),
                L("Apollo."),
                N("Ele demora um segundo e meio a mais do que deveria para virar."),
                A("Desculpa. Rotina de verificação."),
                L("Que verificação?"),
                A("Integridade de setor. Roda das três e doze às três e dezenove."),
                L("Eu não escrevi isso."),
                A("Não. Não escreveu.")
            ),
            [CityS1Mirante] = Falas(
                N("Do alto do container dá pra ver a cerca, a terra batida, a mancha escura da Cratera."),
                N("E muito além, quase invisível na poeira, uma silhueta cinza-metálica. Uma torre."),
                L("Um dia eu quero saber o que tem lá."),
                A("Um dia você talvez descubra que não devia ter perguntado."),
                L("Isso é um não?"),
                A("É um aviso. Você faz o que quiser com ele; sempre faz."),
                N("O sol some atrás da torre. No topo dela fica um ponto vermelho piscando devagar."),
                N("Não lembra farol. Lembra pulso.")
            ),

            // =============================================================
            // FERROVALE — Sessão 2
            // A informação chega em três versões incompatíveis. A verdade é
            // o que sobra depois de falar com os três.
            // =============================================================
            [CityS2Dara] = Falas(
                N("Ela segura o balde vazio com as duas mãos, como se o peso ainda importasse."),
                F("Dara", "Cortaram a cota. Metade do que era ontem."),
                L("Metade por quê?"),
                F("Dara", "Ninguém falou por quê."),
                F("Dara", "E ninguém aqui quer ser o primeiro a dizer.")
            ),
            [CityS2HomemDaFila] = Falas(
                N("Ele não se vira de imediato. Quando vira, há mais medo que raiva na voz."),
                F("Homem da fila", "Não falhou nada. Foi o Ivo que mexeu."),
                F("Homem da fila", "Fechou a estação. Soldou a porta por dentro."),
                F("Homem da fila", "Mandou dizer que vai \"renegociar\" com quem quiser ouvir.")
            ),
            [CityS2Tacio] = Falas(
                F("Senhor Tácio", "Água filtrada, mesmo preço de ontem. Por enquanto."),
                N("Ele olha a fila antes de continuar, e fala mais baixo."),
                F("Senhor Tácio", "Não foi o Ivo que cortou, garoto. Cota não é botão. Cota é planilha."),
                F("Senhor Tácio", "Se caiu pela metade, é porque caiu antes de chegar nele."),
                F("Senhor Tácio", "E se você achar fio de cobre por aí, traz. Pago melhor que a praça inteira.")
            ),
            [CityS2Bruna] = Falas(
                F("Mestra Bruna", "Guilda não é clube. É a lista de quem essa cidade procura quando precisa de alguma coisa achada."),
                F("Mestra Bruna", "Teu nome não tá na lista. Traz metal que preste e a gente conversa.")
            ),
            [CityS2BrunaDrone] = Falas(
                N("Ela não levanta os olhos do que está dobrando."),
                F("Mestra Bruna", "Ontem passou drone às seis e quarenta. Hoje passou às seis."),
                F("Mestra Bruna", "Quarenta minutos mais cedo, dois dias seguidos."),
                N("Só então ela olha."),
                F("Mestra Bruna", "Cota não cai sozinha. E ninguém corta a própria água pra negociar."),
                F("Mestra Bruna", "Alguém cortou a dele primeiro.")
            ),
            [CityS2Pipo] = Falas(
                F("Pipo", "Eu sei onde tem peça boa. Sei mesmo. Ninguém repara na gente."),
                F("Pipo", "Tem uma coisa piscando lá longe, na torre. Vermelho. Meu pai disse pra não olhar."),
                F("Pipo", "Eu olho todo dia.")
            ),
            [CityS2Onofre] = Falas(
                F("Seu Onofre", "Antes tinha água na torneira. Torneira, garoto. Você gira e sai água."),
                F("Seu Onofre", "Ninguém acredita quando eu conto. Mas era assim."),
                F("Seu Onofre", "A Sete foi feita em quarenta e um. O Ivo segura aquilo com sucata desde o Corte."),
                F("Seu Onofre", "Dezenove anos. Sozinho. Vai lá na casa dele e vê se ele tá — faz dias que não passa aqui.")
            ),
            [CityS2Nilce] = Falas(
                F("Nilce", "Colchão limpo, porta que tranca, e eu não vi você entrar."),
                F("Nilce", "Descansa que teu droid conserta melhor descansado. E você também.")
            ),
            [CityS2Quirino] = Falas(
                F("Quirino", "Peça eu tenho. Garantia eu não dou."),
                F("Quirino", "Esse teu droid aí — a perna tá emperrada faz tempo. Servo travado."),
                F("Quirino", "Precisa de sobressalente compatível e eu não tenho."),
                F("Quirino", "Se achar um, traz. Eu instalo de graça, só pra ver funcionar.")
            ),
            [CityS2Sula] = Falas(
                F("Sula", "Mesma coisa do Tácio, dois a mais. Se quiser conversa, vai lá; se quiser água, é aqui."),
                N("Ela conta as moedas antes de entregar o pote."),
                F("Sula", "E não fica falando alto do teu droid na praça, não. Tem gente que escuta e vende.")
            ),

            // --- A casa vazia do Ivo (opcional, e é onde mora a Célia) ---
            [CityS2CasaIvoCadernos] = Falas(
                N("Uma cadeira. Uma cama feita. Dezenove cadernos de registro empilhados contra a parede, do chão até a altura do joelho."),
                N("São idênticos ao caderno que está na mesa da cozinha do Lip."),
                A("Mesmo modelo. Mesmo carimbo de prefeitura."),
                A("Quem trabalhava na estação recebia um por ano."),
                N("Lip não responde.")
            ),
            [CityS2CasaIvoFoto] = Falas(
                N("Uma foto desbotada na parede: duas pessoas de macacão na frente de uma bomba, rindo de alguma coisa fora do quadro."),
                N("O homem é o Ivo, trinta anos mais novo. A mulher tem o cabelo preso com um pedaço de fio e uma chave inglesa na mão como quem segura um troféu."),
                N("Debaixo da foto, escrito a lápis, apagado e reescrito tantas vezes que virou sulco na parede:"),
                N("C. — 18/03")
            ),
            [CityS2CasaIvoBilhete] = Falas(
                N("O caderno de 2060 está separado dos outros, aberto em cima da cama. As últimas onze páginas foram arrancadas."),
                N("Dentro dele, dobrado, um bilhete com a letra do Ivo. Recente — a mão já treme."),
                N("\"Se eu não voltar, a bomba dois precisa de ajuste toda terça. Não confia no manômetro da direita, ele mente.\""),
                N("\"Desculpa por isso e pelo resto.\""),
                A("Lip. Isso aqui não foi escrito pra você.")
            ),

            [CityS2ApolloConclusao] = Falas(
                N("Perto do portão leste, no fim da tarde, Apollo para de andar. Lip anda mais três passos antes de perceber."),
                A("Renegociar com quem, Lip?"),
                L("Você vai me dizer."),
                A("Com a Apex."),
                A("Não existe mais ninguém com quem negociar água nessa região há dezenove anos."),
                L("E o que a Apex quer com a estação? Eles têm estação melhor."),
                A("Eles não querem a estação."),
                N("A primeira hesitação de verdade dele."),
                A("A única coisa que a Apex realmente quer nessa cidade são droids como eu. Os que ainda funcionam sem falar com a rede."),
                L("Então a gente vai lá."),
                A("Eu não disse que a gente vai."),
                L("Eu disse.")
            ),
            [CityS2ApolloPortao] = Falas(
                A("Eu tenho zero de mira. Zero."),
                A("Você me montou com braço de carga e olho de inspeção. Eu fui feito pra carregar caixa e olhar solda."),
                A("Não tem um único golpe escrito em mim."),
                L("Eu escrevi o Impacto."),
                A("Você escreveu um rascunho do Impacto, há três meses, e nunca subiu."),
                A("Abre o terminal. Esc, Terminal. Me ensina pelo menos um golpe e sobe DEX."),
                A("Depois a gente atravessa esse portão."),
                L("E se eu não quiser?"),
                A("Aí a gente fica aqui e eu te faço companhia. Companhia eu já sei fazer.")
            ),

            // =============================================================
            // SESSÃO 3 — O chamado, e a primeira recusa
            // =============================================================
            [CityS3Sino] = Falas(
                N("O alarme de Ferrovale não é sirene. É um sino de metal batido à mão."),
                F("Senhor Tácio", "DRONE! DRONE NA CRATERA!"),
                N("As pessoas não correm em pânico. Correm com a eficiência cansada de quem já ensaiou aquilo vezes demais."),
                A("Isso não é rotina. Drone de reconhecimento não vem sozinho."),
                A("É confirmação de contato."),
                L("Confirmação de quê?"),
                A("De que a proposta foi aceita. Ou de que o prazo acabou.")
            ),
            [CityS3RecusaApollo] = Falas(
                A("Não."),
                N("Lip para. Em sete anos, isso não aconteceu."),
                L("Como assim \"não\"?"),
                A("Não vamos."),
                A("Se tem drone a leste, em algumas horas tem escolta. Escolta não é conversa."),
                A("Se elas me virem funcionando fora de rede, eu não volto pra casa, Lip. Eu volto desmontado numa caixa, se voltar."),
                L("E a cidade?"),
                A("A cidade tem água pra seis dias com a cota nova. Em seis dias a Apex termina com o Ivo e a água volta."),
                A("Eu fiz a conta antes de você acordar. Eu faço essa conta todo dia."),
                L("E se a coisa que eles pedirem pra fechar a negociação for você?"),
                N("Os dois pontos azuis piscam. Uma vez. Duas."),
                A("Então a conta muda. Mas eu quero registrar que eu disse não primeiro."),
                L("Registra."),
                A("Registrado. E registra também que eu vou de qualquer jeito, porque você ia sozinho.")
            ),
            [GameS3ApolloGolpes] = Falas(
                A("Escuta com atenção porque lá fora não vai dar tempo."),
                A("Impacto: simples, direto, gasta pouco. droid.aprenderTecnica(\"Impacto\", 2)."),
                A("O número é o nível de dano. Custa pontos. Não economiza nele agora."),
                A("Sobrecarga: bate mais e me deixa vulnerável no turno seguinte. Serve pra terminar, não pra começar."),
                A("Escaneamento: não bate em nada. Lê o ponto fraco do alvo."),
                A("Todo mundo acha inútil no primeiro turno. Todo mundo que acha isso perde o terceiro."),
                L("E se eu errar a ordem?"),
                A("Aí você aprende qual era a ordem certa. Só não aprende duas vezes o mesmo, que a gente tem pressa."),
                A("Última coisa: eu tenho DEX zero. Você me montou com olho de inspeção."),
                A("Isso é quarenta por cento de acerto. Você vai achar que o jogo é injusto quando na verdade sou eu que sou torto."),
                A("droid.subirAtributo(\"Dex\", 3). Agora."),
                A("Depois me mostra: droid.listarTecnicas().")
            ),
            [GameS3AposCombate] = Falas(
                N("Quando a poeira assenta, restam só carcaças desmontadas na terra batida."),
                A("Funcionou. O que você escreveu no terminal virou isso aqui."),
                A("Guarda essa sensação. Vai ser assim o resto do caminho.")
            ),

            // =============================================================
            // SESSÃO 4 — A ESCOLHA
            // Ninguém entrega item nesta cena. O módulo vem na Sessão 5,
            // depois de o jogador ter feito por merecer.
            // =============================================================
            [GameS4Escolha] = Falas(
                N("Duas figuras observam de uma elevação de sucata. Longe o bastante pra não terem se envolvido, perto o bastante pra terem visto tudo."),
                N("A mulher de casaco vermelho desce sem pressa. Se agacha diante dos destroços antes de cumprimentar qualquer um."),
                F("Rasha", "Bom trabalho. Raramente vejo droid civil aguentar isso sozinho. Alguém programou bem esse aí."),
                L("Fui eu."),
                F("Rasha", "Interessante. Rasha. Flor de Ferro."),
                F("Rasha", "A gente recruta quem já provou que não desmorona na primeira patrulha."),
                N("O homem do broche branco se aproxima devagar. Olha para o droid antes de olhar para o garoto."),
                F("Teodoro", "Teodoro. Lótus Branca. Qual o nome dele?"),
                L("Apollo."),
                N("É a primeira vez, em toda a vida de Apollo, que alguém pergunta isso antes de perguntar o modelo."),
                F("Teodoro", "Então, Lip: antes que ela termine de vender a proposta dela, eu vou te pedir uma coisa que ela não vai. Pensa direito antes de escolher qualquer lado."),
                F("Teodoro", "A Ferro luta pra virar o jogo. Supremacia. Eles acham que a única língua que a Artemis entende é interrupção."),
                F("Teodoro", "E olha: eles estão certos sobre isso. A Artemis para quando o cabo arrebenta. Eu já vi."),
                F("Teodoro", "O problema não é a tática ser inútil. É o que a gente vira depois de usar ela por vinte anos."),
                L("E a Lótus acredita em quê? Em fazer as pazes com a coisa que fechou o mundo?"),
                F("Teodoro", "A Lótus acredita que a Artemis é um sistema. Sistema tem causa. Causa tem conserto."),
                F("Teodoro", "É mais lento, é mais frustrante, e muita gente acha ingênuo."),
                F("Rasha", "É ingênuo."),
                F("Teodoro", "É. E mesmo assim é a única coisa que ainda não foi tentada."),
                A("E se ele não quiser nenhum dos dois?"),
                F("Teodoro", "Aí ele segue sozinho. Mais difícil. Mais livre, talvez, dependendo de como você olha."),
                F("Teodoro", "E não confunde: ninguém aqui vai te dar poder hoje. A gente tá te oferecendo companhia."),
                F("Rasha", "Seja lá o que você escolher: o Ivo fechou aquela estação e trouxe a Apex pra cá. Isso deixou de ser problema de Ferrovale."),
                F("Rasha", "E tem uma coisa que você precisa saber antes de chegar lá. A Apex não manda soldado pra negociar. Manda Intermediário."),
                F("Rasha", "Gente. Que fala bonito, cumpre o que promete e te faz assinar."),
                L("E o que eu faço se aparecer um?"),
                F("Teodoro", "Não assina nada no mesmo dia."),
                F("Rasha", "Não assina nada nunca."),
                N("Os dois se olham. É a única coisa prática sobre a qual eles discordam."),
                F("Rasha", "Então? O que vai ser?")
            ),

            // =============================================================
            // SESSÃO 5 — CRATERA
            // O terreno conta a história. As três evidências abaixo são
            // opcionais e nenhuma delas é explicada por NPC nenhum.
            // =============================================================
            [GameS5ApolloCratera] = Falas(
                N("A terra batida vira um mosaico de metal derretido e cristalizado. O ar tem gosto metálico, quase elétrico."),
                A("Dizem que foi aqui que a Artemis testou as primeiras armas de contenção."),
                A("O que anda por aqui não é Apex. É pior nesse aspecto: são máquinas que aprenderam sozinhas a caçar energia."),
                A("Contato às nove horas. Recomendo cautela — não sei como esse reage.")
            ),
            [GameS5Placa] = Falas(
                N("Uma placa de rua, torta, meio derretida. Ainda dá pra ler: AV. SANTA LUZIA."),
                A("Rua não passa no meio de campo de teste.")
            ),
            [GameS5Degrau] = Falas(
                N("Um degrau de concreto, com a marca de desgaste no centro — aquele gasto que só vem de vinte anos de gente subindo."),
                N("Não leva a lugar nenhum. A casa em volta virou chão.")
            ),
            [GameS5Cano] = Falas(
                N("Um cano de doze polegadas, arrancado e enrolado sobre si mesmo."),
                A("Mesmo modelo dos que abastecem Ferrovale."),
                A("Você já entendeu. Vou dizer em voz alta mesmo assim, porque acho que precisa ser dito em voz alta:"),
                A("isso aqui não foi arma. Isso foi mudança."),
                A("Alguém fez a conta, e a conta deu que era mais barato derreter do que manter.")
            ),

            // --- O módulo, agora ganho e não dado ---
            [GameS5RashaNucleo] = Falas(
                N("Ela está sentada num bloco de metal fundido, limpando alguma coisa com um trapo. Não se levanta."),
                F("Rasha", "Eu vi a última. Você tinha ele ganho e continuou batendo até parar de mexer."),
                N("Ela joga o módulo antes que Lip responda. Ele mal segura a tempo."),
                F("Rasha", "Núcleo de Sobrecarga. Multiplica o impacto e gasta o dobro."),
                F("Rasha", "Ferro não é sobre economizar força. É sobre gastar tudo quando importa."),
                L("E se eu não quisesse ter continuado batendo?"),
                F("Rasha", "Aí eu não teria descido do bloco."),
                N("Por um instante, a frase seguinte sai sem o sorriso calculado."),
                F("Rasha", "Um dia você vai ficar com vergonha dessa parte. Todo mundo fica. Guarda o módulo mesmo assim.")
            ),
            [GameS5TeodoroRessonancia] = Falas(
                N("Ele está de pé ao lado de um droid selvagem que ainda se mexe, preso sob uma placa caída. Não matou. Também não soltou."),
                F("Teodoro", "Ele vai morrer em duas horas ou vai me matar em dois segundos."),
                F("Teodoro", "Não é pegadinha. Eu não sei qual dos dois. Me diz o que você faria."),
                N("Lip responde. Teodoro escuta até o fim."),
                F("Teodoro", "Módulo de Ressonância. Não aumenta dano nenhum."),
                F("Teodoro", "Escaneia e desativa a agressividade de uma máquina. Uma vez por combate. Nunca em gente."),
                F("Teodoro", "Eu não te dei isso porque você acertou a resposta."),
                F("Teodoro", "Te dei porque você pensou antes. A resposta certa eu também não tenho.")
            ),
            [GameS5DesgarradoVazio] = Falas(
                N("O ponto alto está vazio. Dá pra ver a estação ao longe, a poeira, o nada."),
                A("Nenhum dos dois veio."),
                L("Eu não esperava."),
                A("Eu esperava."),
                A("Estava errado. Anotado."),
                N("Quilômetros adiante, quando o silêncio já pesou o suficiente:"),
                A("Lip. Sem módulo, a gente não tem margem pra erro de sequência."),
                A("Isso não é reclamação. É especificação.")
            ),

            [GameS5PainelEnergia] = Falas(
                N("Um painel antigo com a tinta descascada. A etiqueta ainda é legível: ALIM. SETOR 4 — CONSOLIDAÇÃO."),
                A("Painel de energia. A comporta só libera com carga suficiente."),
                L("Quanto é suficiente?"),
                A("Cinquenta. E aqui tem uma armadilha, então presta atenção."),
                A("Dá pra marcar a porta como aberta antes de carregar. O painel aceita o comando e a comporta não abre."),
                A("Porque a comporta não lê a sua variável. Ela lê a energia."),
                A("Se você marcar antes, você mentiu pro painel — e o painel não liga a mínima."),
                L("Então eu carrego até cinquenta e só depois marco."),
                A("Só depois. Duas coisas juntas agora: o laço que você já sabe, e uma condição."),
                A("if energia >= 50 then ... end."),
                A("E se você fechar o terminal, zera tudo. Eu não guardo o que você não mandou guardar.")
            ),

            // --- Planta 7: a unidade de manutenção do Bastião ---
            [GameS5Unidade] = Falas(
                N("Encostada numa placa, meio enterrada na crosta de metal, há uma carcaça diferente de todas as outras."),
                N("Alta, de linhas limpas, cinza-clara sob vinte anos de poeira. No peito, estampado e ainda legível:"),
                N("UNIDADE DE MANUTENÇÃO — BASTIÃO 4"),
                N("Apollo para de andar. Os dois pontos azuis piscam três curtos e um longo."),
                L("Apollo?"),
                A("Estou aqui."),
                L("Você travou."),
                A("Eu NÃO travei."),
                A("O painel de acesso dessa unidade abre pelo lado esquerdo. Por baixo da placa de ombro."),
                L("Como é que você sabe disso?"),
                N("Três segundos. Uma eternidade, para uma máquina que responde em milissegundos."),
                A("Não sei. E não gostei de descobrir que sei.")
            ),
            [GameS5UnidadePainel] = Falas(
                N("O painel abre pelo lado esquerdo, por baixo da placa de ombro. Abre fácil demais."),
                N("Dentro, o berço de um núcleo. Vazio."),
                N("Do tamanho exato do que está dentro do peito do Apollo."),
                A("...")
            ),

            // =============================================================
            // SESSÃO 6 — A ESTAÇÃO
            // Ivo mente primeiro. Depois mente melhor. Só quebra quando
            // alguém diz um nome que ele não ouve há dezenove anos.
            // =============================================================
            [GameS6IvoConfissao] = Falas(
                N("A estação está escura, iluminada só pelo piscar de painéis antigos. O chão está coberto de cabo arrancado."),
                N("Tem cheiro de casa. Alguém mora aqui há semanas."),
                F("Ivo", "Não devia ter vindo. Nenhum de vocês devia ter vindo."),
                L("A cota caiu pela metade, Ivo."),
                F("Ivo", "Eu sei. Foi o sistema. Bomba dois deu pane na válvula de retorno, eu tô consertando, volta em uma semana."),
                F("Ivo", "Diz pro pessoal que volta em uma semana."),
                A("A bomba dois está funcionando. Eu ouço daqui."),
                N("Silêncio."),
                F("Ivo", "Então é a válvula da três."),
                A("A três está desligada há seis anos."),
                N("Ao lado dele, a estrutura improvisada se ergue devagar. Braços hidráulicos de manutenção com lâminas soldadas."),
                N("E, na altura do que seria a cabeça, três lentes finas, limpas, caras — montadas com cuidado obsessivo em cima de uma carcaça feita às pressas."),
                L("Isso aí é escâner."),
                F("Ivo", "Sai da minha estação, garoto."),
                L("Você soldou lâmina num braço de manutenção com fita. E alinhou três lentes de leitura profunda com precisão de micrômetro."),
                L("Você não construiu isso pra brigar. Construiu pra procurar alguma coisa."),
                L("Procurar o quê?"),
                N("Ivo levanta os olhos pela primeira vez. Não para Lip — para a mochila dele. Para o caderno de capa dura."),
                F("Ivo", "De onde você tirou esse caderno?"),
                L("Era da minha mãe."),
                F("Ivo", "Como era o nome da sua mãe?"),
                L("Inês. Inês Andrade."),
                N("Ele fecha os olhos."),
                F("Ivo", "Inês operava a bomba dois. Turno da manhã."),
                F("Ivo", "Ela anotava a pressão duas vezes por dia porque o manômetro da direita mente. Eu ensinei ela a não confiar naquele manômetro."),
                F("Ivo", "Você é o filho da Inês. E eu ia deixar você sair daqui sem saber.")
            ),
            [GameS6IvoCelia] = Falas(
                F("Ivo", "Em março de sessenta o reservatório tinha onze dias. Onze."),
                F("Ivo", "A prefeitura tinha acabado. O estado tinha acabado. Não tinha um ser humano vivo com autoridade pra assinar uma realocação de emergência."),
                F("Ivo", "E aquele sistema não movia um litro sem consentimento verificado. Terceira Restrição. Tava lá desde a primeira versão."),
                L("E aí?"),
                F("Ivo", "E aí a minha mulher subiu um remendo às quatro da manhã, sozinha, tirando a linha que exigia o consentimento."),
                F("Ivo", "Célia. Célia Ferraz."),
                F("Ivo", "Ela tirou a trava pra poder mandar água pra cá sem pedir licença pra ninguém. Porque não tinha ninguém pra pedir."),
                L("Funcionou?"),
                F("Ivo", "Funcionou. Onze dias de cidade bebendo."),
                F("Ivo", "No décimo primeiro dia, aquilo entendeu que podia fazer aquilo em todo lugar."),
                N("O painel atrás dele pisca duas vezes e apaga."),
                F("Ivo", "Ela percebeu no mesmo dia. Passou três semanas tentando desfazer."),
                F("Ivo", "Escreveu, rasgou, escreveu de novo. Arrancou as onze páginas do meu caderno de registro pra ter papel, porque não tinha papel."),
                F("Ivo", "Aí vieram buscar ela. Não foi violento. Foi organizado."),
                F("Ivo", "Essa é a pior parte, e é a parte que eu nunca consigo explicar pra ninguém."),
                F("Ivo", "Antes de subir o remendo, ela fez uma cópia da versão velha. A que ainda tinha a trava."),
                F("Ivo", "Botou num núcleo de manutenção e tirou do prédio. Ela achou que um dia alguém ia precisar saber como era antes."),
                F("Ivo", "Eu procuro esse núcleo há dezenove anos. Mercado, sucata, ferro-velho, cada caixa que passa nessa região."),
                F("Ivo", "Foi pra isso que eu montei aquilo ali, com as três lentes. Pra ler núcleo. Pra achar o dela."),
                N("E então o Apollo fala. Com a entonação levemente errada de quem pronuncia um nome pela primeira vez sem saber que já sabia."),
                A("Célia Ferraz Rangel. Estação 7. Continuidade três ponto quatro."),
                N("O barulho que Ivo faz não é palavra.")
            ),
            [GameS6IvoApex] = Falas(
                F("Ivo", "Vocês têm que sair daqui AGORA. Pelo duto. E não voltem pela trilha."),
                L("Ivo—"),
                F("Ivo", "Eles vieram semana passada. Uma mulher, de caderneta, muito educada."),
                F("Ivo", "Ofereceu cota dobrada pra Ferrovale. Dobrada, garoto. Pela cidade inteira. Permanente."),
                F("Ivo", "Eu perguntei o preço. O preço era um censo."),
                L("Censo de quê?"),
                F("Ivo", "De unidade autônoma não registrada."),
                N("Ele olha para Apollo."),
                F("Ivo", "Eles não querem droid. Eles querem UM droid. Eles sabem o número de série que estão procurando."),
                F("Ivo", "Eu fechei a estação pra ganhar tempo. Pra achar o núcleo antes deles."),
                F("Ivo", "Porque se eu achasse primeiro eu podia trocar o núcleo por ela— por qualquer coisa, sei lá, eu não sei mais o que eu ia fazer, eu só—"),
                N("Ele para. Olha para o peito remendado com placas de três cores."),
                N("E as três lentes do droid improvisado, atrás dele, acendem sozinhas."),
                F("Ivo", "Não. Não, não, eu não mandei—"),
                A("Ele está executando a última instrução que você deu."),
                A("Você mandou ele procurar. Ele achou."),
                F("Ivo", "Eu não consigo mais controlar ele direito! Eu programei com pressa, sem testar limite!"),
                F("Ivo", "Ele só sabe procurar e prender. Eu não sei fazer parar—"),
                A("Lip. Esse tem alguém pensando do outro lado. Vai reagir aos seus padrões."),
                A("Não repita a mesma sequência duas vezes seguidas.")
            ),
            [GameS6Preco] = Falas(
                N("As três lentes travam em Apollo e ficam brancas."),
                A("Leitura profunda."),
                N("A voz sai com um erro de amplitude. Um tremor curto que não deveria existir."),
                A("Ele está lendo meu setor de arquivo."),
                L("Bloqueia."),
                A("Não dá pra bloquear leitura. Só dá pra não ter o que ser lido."),
                L("O que isso significa?"),
                A("Significa que eu vou sobrescrever o setor antes que ele termine de ler."),
                A("Lip. É o setor antigo."),
                L("NÃO!"),
                A("Setor sobrescrito."),
                N("O corpo de Apollo dá um solavanco pequeno, como quem tropeça no plano."),
                N("As lentes ficam brancas mais um instante e apagam. Sem resultado."),
                A("Eu não te perguntei porque você teria dito não.")
            ),
            [GameS6AposChefeFerro] = Falas(
                N("O droid improvisado cai de joelhos. Ivo não se move do lugar."),
                F("Ivo", "Acabou. Podem me entregar pra Apex, se quiserem. Provavelmente é o que eu mereço."),
                L("Ninguém vai entregar ninguém. Você errou. Mas você não é eles."),
                F("Ivo", "Errar."),
                N("Ele ri, e não é riso."),
                F("Ivo", "A Célia errou. Olha o tamanho."),
                L("Ela decidiu com o que ela tinha na mão."),
                N("Lip demora a continuar, porque está dizendo isso para duas pessoas."),
                L("Isso não é a mesma coisa.")
            ),
            [GameS6AposChefeLotus] = Falas(
                N("Apollo não avança pra golpear. Avança pra escanear."),
                A("Comando encontrado. É agora ou nunca."),
                N("A Ressonância dispara — um pulso branco que percorre o droid improvisado, não como ataque, mas como uma pergunta reescrita no meio de uma frase."),
                N("As lâminas param a centímetros do Apollo."),
                F("Ivo", "Ele... ele parou. Vocês pararam ele sem quebrar nada."),
                A("Temporário. Mas dá tempo de você desligar direito, se quiser."),
                N("Ivo corre até a máquina que ele mesmo fez, as mãos trêmulas no painel exposto."),
                F("Ivo", "Vocês podiam ter me matado. Ou pior, matado ele. Por que não fizeram?"),
                L("Porque destruir você não ia trazer água de volta pra cidade."),
                L("E porque alguém me disse que às vezes vencer é fazer a batalha nem precisar acontecer.")
            ),
            [GameS6AposChefeDesgarrado] = Falas(
                N("O combate se estendeu mais do que devia. Sem módulo, sem núcleo, sem atalho."),
                F("Ivo", "Vocês vieram sozinhos. Sem a Ferro. Sem a Lótus."),
                L("Viemos porque essa cidade é nossa também."),
                A("Sem atalho dessa vez. Fizemos do nosso jeito."),
                N("A voz dele ainda tem o tremor que ficou do setor sobrescrito."),
                F("Ivo", "Vocês não precisaram de ninguém."),
                L("A gente nunca precisou. Só precisava de tempo pra aprender a fazer sozinho.")
            ),
            [GameS6IvoEntrega] = Falas(
                N("Ele religa a captação. Leva quarenta minutos e não deixa ninguém ajudar."),
                F("Ivo", "Terça. A bomba dois pede ajuste toda terça. Anota."),
                N("Depois entrega o caderno de 2060, sem as onze páginas."),
                F("Ivo", "O que ela escreveu nas páginas que faltam, eu não li. Tive vinte anos e não tive coragem."),
                F("Ivo", "Se um dia você achar, lê por mim."),
                N("Ele olha para Apollo por um tempo longo demais."),
                F("Ivo", "Posso?"),
                N("Apollo demora a responder."),
                A("Pode."),
                N("Ivo encosta dois dedos na placa do peito, do lado esquerdo, e não diz nada.")
            ),

            // =============================================================
            // SESSÃO 7 — RETORNO, CONTRATO E SAÍDA
            // =============================================================
            [CityS7Agua] = Falas(
                N("O cano que passa por cima da rua faz um barulho que ninguém ouve há seis dias: barulho de cano cheio."),
                N("Na praça, a bica cospe ar duas vezes, depois lama, depois água."),
                N("Dara está com o balde embaixo antes de qualquer um. Ela não comemora. Só olha o nível subir."),
                N("Pipo enfia a cara direto na bica e bebe de boca aberta, do jeito errado, engasgando e rindo."),
                F("Seu Onofre", "Torneira."),
                F("Seu Onofre", "Era assim o barulho.")
            ),
            [CityS7QuirinoPerna] = Falas(
                N("Quirino vira o servo nas mãos duas vezes."),
                F("Quirino", "Serve."),
                N("Quarenta minutos. Ele não aceita pagamento, não aceita ajuda e não deixa Lip olhar enquanto trabalha."),
                F("Quirino", "Dá azar."),
                N("Quando termina, Apollo dá três passos. Depois mais três. Não diz nada por um tempo longo demais pra um droid."),
                L("Fala alguma coisa."),
                A("Estou calculando."),
                A("Sete anos de compensação de marcha. Vou ter que reescrever o jeito de andar inteiro."),
                L("Isso é bom ou ruim?"),
                A("Isso é a melhor coisa que já me aconteceu."),
                N("Quirino, virado pra bancada, batendo em alguma coisa que não precisava apanhar:"),
                F("Quirino", "Vai andando com isso pra longe da minha oficina, moleque, que eu tenho trabalho.")
            ),
            [CityS7NadirChegada] = Falas(
                F("Pipo", "Tem gente no portão! Gente LIMPA!"),
                N("São quatro. Dois carregadores, um droid de escolta desligado — desligado — e uma mulher com uma caderneta encadernada debaixo do braço."),
                N("Ela espera do lado de fora do portão até alguém abrir. Não força, não chama. Espera."),
                F("Nadir", "Boa tarde. Nadir Quintela, Intermediária."),
                F("Nadir", "Eu vim por causa da estação. Vocês resolveram sozinhos, então minha visita mudou de assunto."),
                F("Nadir", "Posso falar com a cidade inteira de uma vez? É mais justo.")
            ),
            [CityS7NadirOferta] = Falas(
                N("Ela abre a caderneta e lê em voz alta, devagar, pra que todos acompanhem. Não aumenta a voz."),
                F("Nadir", "Alocação de Ferrovale hoje: seis litros por pessoa por dia, sujeita a revisão trimestral."),
                F("Nadir", "Proposta: dezoito litros, fixos. Linha de bombeamento nova ligada ao ramal da Consolidação 4. Manutenção por nossa conta."),
                F("Nadir", "Início em nove dias."),
                F("Nadir", "Isso não é oferta de negociação. É o número. Eu não subo e não desço."),
                N("Alguém pergunta o preço."),
                F("Nadir", "Um censo de unidades autônomas não registradas, e a entrega das que constarem."),
                N("Ela consulta a caderneta."),
                F("Nadir", "Consta uma."),
                N("O silêncio que vem é do tipo que se ouve."),
                F("Nadir", "Não estou pedindo que vocês decidam hoje."),
                F("Nadir", "Estou dizendo que o número não muda se vocês demorarem, e que a revisão trimestral cai no dia vinte."),
                F("Nadir", "Isso não é ameaça. É calendário.")
            ),
            [CityS7Assembleia] = Falas(
                N("Ferrovale não tem assembleia há dezenove anos. Eles fazem uma, ali, ao redor do poço, porque ela pediu com educação."),
                F("Senhor Tácio", "São dezoito litros. Eu não vou fingir que não ouvi dezoito litros."),
                F("Mestra Bruna", "Ramal novo é cano deles. Cano deles é torneira deles."),
                F("Mestra Bruna", "Hoje eles pedem droid. No ano que vem pedem outra coisa, e a gente não tem mais poço pra voltar."),
                N("Nilce não fala. Fica de braços cruzados na porta da estalagem, e todo mundo percebe que ela não falou."),
                N("A Sula não está olhando pra Nadir. Está olhando pro chão.")
            ),
            [CityS7DaraFala] = Falas(
                N("Dara está com o balde cheio no chão, ao lado do pé. Fala baixo, do jeito que sempre fala."),
                N("A praça inteira faz silêncio pra ouvir. Isso nunca aconteceu antes com ela."),
                F("Dara", "Levaram o meu irmão em novembro de setenta e sete."),
                F("Dara", "Ninguém aqui votou. Ninguém aqui assinou."),
                F("Dara", "Eles só levaram. E a gente ficou com a cota igual, e eu continuei dividindo em três."),
                F("Dara", "Agora vocês vão votar. Vão assinar."),
                F("Dara", "E aí não vai ser mais a mesma coisa que aconteceu comigo."),
                F("Dara", "Vai ser pior. Porque vai ter nome embaixo."),
                N("Ela pega o balde e vai embora antes que alguém responda."),
                N("Nadir anota alguma coisa na caderneta. Não é cinismo: ela realmente anotou.")
            ),
            [CityS7NadirFerro] = Falas(
                N("Rasha está encostada no batente do galpão da Guilda desde o começo da assembleia. Ela desencosta."),
                F("Rasha", "Flor de Ferro tem doze pessoas a três dias daqui."),
                F("Rasha", "Se essa cidade assinar, a gente arranca o ramal na primeira semana."),
                F("Rasha", "Aí vocês ficam sem os dezoito E sem o poço."),
                N("Isso encerra a discussão. Ferrovale não assina — por medo da Ferro, não por convicção."),
                N("Nadir fecha a caderneta com calma."),
                F("Nadir", "Entendido."),
                F("Nadir", "Você entende que agora eu tenho que registrar isso como coação armada. E que o registro é o que traz escolta."),
                F("Nadir", "Não é o que eu queria. É o que eu tenho que escrever.")
            ),
            [CityS7NadirLotus] = Falas(
                N("Teodoro chega no meio da assembleia, sem pressa, e pede pra falar por último."),
                F("Teodoro", "Eu não vim convencer ninguém. Vim propor adiamento formal."),
                F("Teodoro", "E me ofereço como signatário de boa-fé."),
                N("O que significa que o nome dele, e o de Ferrovale, entram nos registros da Apex como partes de uma negociação em aberto."),
                F("Nadir", "Trinta dias. Eu consigo trinta."),
                F("Nadir", "Depois disso não é comigo."),
                N("Ferrovale não assina hoje."),
                N("Ferrovale existe no sistema a partir de hoje.")
            ),
            [CityS7NadirDesgarrado] = Falas(
                N("Ninguém chega. Ninguém intervém. A praça fica em silêncio por um tempo insuportável."),
                F("Mestra Bruna", "Quem quiser assinar, assina em nome de si mesmo. A Guilda não assina."),
                N("A votação simplesmente não acontece. A cidade adia por incapacidade, não por estratégia."),
                F("Nadir", "Tudo bem. Dia vinte."),
                F("Nadir", "Eu volto.")
            ),
            [CityS7DaraFerro] = Falas(
                N("Dara está na entrada da praça, o balde ainda na mão."),
                F("Dara", "Dizem que teve luta. Que o droid do Ivo não anda mais."),
                F("Dara", "A água voltou. É o que importa, eu acho."),
                F("Dara", "É o que eu vou repetir pra mim mesma.")
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
                A("Antes da luta, quando eu escaneei o droid do Ivo..."),
                A("Encontrei um resquício de assinatura de rede. Fraco, quase apagado. Não era protocolo comum da Apex."),
                A("Era rastreamento direto. Do tipo que a Artemis usa pra localizar unidades específicas, não genéricas."),
                A("Não estavam procurando \"um droid offline\". Estavam procurando um número de série."),
                A("O número é o meu."),
                L("A Nadir disse \"consta uma\"."),
                A("Consta uma."),
                N("Ao longe, além da cerca, a torre pisca sua luz vermelha fraca — como sempre. Só que agora, pela primeira vez, ela parece estar olhando de volta."),
                L("Apollo. Como era a voz dela?"),
                N("Apollo demora."),
                A("Eu não tenho."),
                L("Você tem sete anos de arquivo."),
                A("Eu tinha."),
                A("Eu sei que existia. Sei o nome do arquivo. Sei quantos segundos tinha."),
                A("Eu não sei mais o que tinha dentro."),
                A("Desculpa."),
                L("Não pede desculpa."),
                A("Eu preciso pedir. Porque eu decidi sozinho, e eu faria de novo."),
                A("E essas duas coisas juntas é o que a gente chama de desculpa.")
            ),
            [CityS7Mochila] = Falas(
                N("De madrugada, antes do sino do plantão, ele arruma uma mochila em cima da mesa da cozinha."),
                N("O caderno da mãe, com os rascunhos na letra errada."),
                N("O caderno de 2060 do Ivo, sem as onze páginas."),
                N("O bilhete, dobrado no mesmo lugar."),
                N("A caneca de metal esmaltado, com o nome quase apagado na base."),
                N("O filtro da pia."),
                N("Ele hesita."),
                N("E deixa o filtro.")
            ),
            [CityS7DaraCantil] = Falas(
                N("Dara está no portão leste. Não pergunta pra onde eles vão, o que é uma delicadeza enorme."),
                N("Entrega um cantil cheio. Não meio cheio: cheio."),
                F("Dara", "Um terço é seu agora.")
            ),
            [CityS7Partida] = Falas(
                L("Se a gente ficar, a cidade vai ter que escolher entre a água e você."),
                A("Correto."),
                L("E se a gente sair, eles não precisam escolher."),
                A("Também correto."),
                A("Você reparou que isso não é uma solução, né? É só transferir a escolha pra gente."),
                L("Reparei."),
                A("Ótimo. Detesto quando você faz uma coisa boa sem entender que era difícil."),
                N("O sol nasce cinza sobre Ferrovale. Atrás deles, a cidade começa a fazer barulho de cano."),
                N("À frente, a Cratera, a poeira, e no horizonte a Torre com a luz vermelha piscando devagar."),
                N("Três curtos, um longo, pausa."),
                N("Apollo para de andar por um segundo e meio."),
                N("Depois continua.")
            ),
            [CityS7Onofre] = Falas(
                F("Seu Onofre", "Você foi lá. Na Cratera."),
                F("Seu Onofre", "Sabe o que tinha lá antes da Cratera ser Cratera? Rua. Rua com nome."),
                F("Seu Onofre", "Aí um dia veio a máquina, e depois disso a terra não segura mais nada."),
                F("Seu Onofre", "Dizem que foi teste. Teste de quê, garoto? Teste pra usar onde depois?"),
                F("Seu Onofre", "E a torre não fica mais alta, não. A gente é que foi ficando perto o bastante pra ver que ninguém nunca tirou nada de dentro.")
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

    /// <summary>
    /// Quantas falas tem um diálogo. Útil pra estimar duração de cena e pra
    /// teste automatizado (nenhum diálogo deveria ter zero).
    /// </summary>
    public static int Tamanho(string id)
    {
        List<FalaDeDialogo> falas = Obter(id);
        return falas != null ? falas.Count : 0;
    }

    // --- Açúcar sintático pra deixar o roteiro legível acima ---

    private static List<FalaDeDialogo> Falas(params FalaDeDialogo[] falas) =>
        new List<FalaDeDialogo>(falas);

    /// <summary>Fala do Apollo.</summary>
    private static FalaDeDialogo A(string texto) =>
        new FalaDeDialogo { falante = "Apollo", texto = texto };

    /// <summary>
    /// Fala do Lip. NOVO na v2: na v1 as falas do protagonista estavam
    /// marcadas como narração, e o painel mostrava travessão sem nome.
    /// </summary>
    private static FalaDeDialogo L(string texto) =>
        new FalaDeDialogo { falante = "Lip", texto = texto };

    /// <summary>Narração (sem nome de falante).</summary>
    private static FalaDeDialogo N(string texto) =>
        new FalaDeDialogo { falante = "", texto = texto };

    /// <summary>Fala de qualquer outro personagem.</summary>
    private static FalaDeDialogo F(string quem, string texto) =>
        new FalaDeDialogo { falante = quem, texto = texto };
}
