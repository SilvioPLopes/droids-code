using System.Collections.Generic;

/// <summary>
/// Catalogo de LOCAIS. Arquivo NOVO na v2.
///
/// POR QUE ELE EXISTE
/// ------------------
/// Ate agora o projeto sabia quem existe (NPCs), o que se faz (quests) e o
/// que se fala (dialogos) -- mas nao sabia ONDE. "Mercado sob o viaduto"
/// aparecia como texto solto em tres arquivos diferentes, escrito de tres
/// jeitos, sem nada que dissesse o que ha naquele lugar, o que da pra fazer
/// ali, com o que ele faz fronteira, e por que ele e assim.
///
/// Este arquivo e a fonte de verdade do espaco do jogo. Ele serve a quatro
/// coisas ao mesmo tempo:
///   1. Montagem de cena  -> DescricaoDeCena, Atmosfera, AcoesPossiveis
///   2. Navegacao         -> Conexoes (grafo de mapa, sem coordenada)
///   3. Rotina de NPC     -> CatalogoDeNpcs.Rotina aponta pra estes Ids
///   4. Arte              -> PromptDeAmbiente e PromptDeMapa, prontos pra
///                           colar num gerador de imagem
///
/// PRINCIPIO DE DESIGN: narrativa espacial. Cada local conta uma parte da
/// historia sem que ninguem precise falar. O poco tem fila porque a agua
/// acabou; o beco e feito de carcaca porque ninguem teve tempo de remover
/// as maquinas; o quarto tres da estalagem esta trancado ha dezesseis anos.
/// O jogador que so atravessa entende o mundo mesmo assim. Ver Parte II.3
/// do TCC_-_ENREDO_v2.md.
///
/// TUDO aqui e DADO, nao comportamento. Nenhum script existente precisa ser
/// alterado pra este arquivo compilar: ele nao referencia nada.
/// </summary>
public static class CatalogoDeLocais
{
    /// <summary>
    /// Prefixo de estilo pra ambiente/cenario. Mesmo papel do
    /// CatalogoDeNpcs.EstiloBase, mas para lugar em vez de gente.
    /// </summary>
    public const string EstiloAmbiente =
        "environment concept art for a 2D isometric RPG, semi-realistic painterly " +
        "style, wide establishing shot, no characters, no text, no UI. " +
        "Setting: rural inland Brazil, year 2079, forty years into an engineered " +
        "drought. Everything built is welded from salvaged industrial scrap; " +
        "nothing has been manufactured new in four decades. " +
        "Palette: oxidized iron, dust ochre, sun-bleached canvas, faded brick red; " +
        "one single cold blue accent, used only for water and for machine light. " +
        "Hard high-noon sun, long hard shadows, visible dust suspended in air, " +
        "no green vegetation, cracked dry ground.";

    /// <summary>
    /// Sufixo pra planta baixa / mapa tatico, quando o que se quer nao e a
    /// paisagem e sim o layout jogavel.
    /// </summary>
    public const string EstiloMapa =
        "Variant: top-down orthographic tactical map of this location, readable " +
        "layout, clear walkable paths and blocked areas, structures in simple " +
        "solid shapes, muted parchment-and-ink rendering, labeled zones, " +
        "no perspective, no characters.";

    // --- Ferrovale ---
    public const string IdCasaDoLip = "loc_casa_lip";
    public const string IdPocoComunitario = "loc_poco";
    public const string IdMercadoDoViaduto = "loc_mercado";
    public const string IdBarracaDaSula = "loc_barraca_sula";
    public const string IdOficinaDoQuirino = "loc_oficina";
    public const string IdEstalagem = "loc_estalagem";
    public const string IdPostoDaBenta = "loc_posto_benta";
    public const string IdGalpaoDaGuilda = "loc_guilda";
    public const string IdFerroVelho = "loc_ferro_velho";
    public const string IdBecoDasCarcacas = "loc_beco_carcacas";
    public const string IdPracaDaAssembleia = "loc_praca";
    public const string IdPortaoNorte = "loc_portao_norte";
    public const string IdCasaDaDara = "loc_casa_dara";
    public const string IdMirante = "loc_mirante";

    // --- Fora dos muros ---
    public const string IdEstradaDaCratera = "loc_estrada";
    public const string IdCrateraBorda = "loc_cratera_borda";
    public const string IdAcampamentoDasFaccoes = "loc_acampamento";
    public const string IdCrateraFundo = "loc_cratera_fundo";
    public const string IdEstacaoSete = "loc_estacao_sete";

    public static readonly Dictionary<string, DefinicaoDeLocal> Todos =
        new Dictionary<string, DefinicaoDeLocal>
        {
            // =============================================================
            // FERROVALE
            // =============================================================
            [IdCasaDoLip] = new DefinicaoDeLocal
            {
                Id = IdCasaDoLip, Nome = "Casa do Lip", Regiao = "Ferrovale",
                DescricaoCurta = "Um cômodo e meio, com uma bancada que ocupa mais espaço que a cama.",
                DescricaoDeCena = "A casa tem um cômodo e meio, e o meio é a bancada. Sobre ela, ferramenta organizada por tamanho — a única coisa organizada aqui. Na parede, a caneca da mãe, com asa quebrada e colada, num prego alto demais pra uso: ela está pendurada, não guardada. O filtro de água na pia goteja a cada onze segundos, e o Lip conhece o intervalo. Debaixo da cama, um caderno de capa mole onde ele anotou, aos treze anos, os passos de montagem do Apollo, com uma página inteira riscada e a frase 'errei aqui' escrita embaixo, sem raiva.",
                Atmosfera = "Luz entrando por fresta de telha. Poeira visível no facho. O pingo do filtro é o metrônomo da cena e o único som constante.",
                AcoesPossiveis = new List<string>
                {
                    "Examinar a caneca (memória da mãe; libera city_s1_caneca)",
                    "Examinar o caderno de montagem (planta do tema: errar é informação)",
                    "Consertar o filtro (puzzle p_filtro_agua)",
                    "Dormir (recupera; única cama gratuita do jogo)",
                    "Conversar com o Apollo (varia por sessão; casa_noite_apollo)"
                },
                Conexoes = new List<string> { IdBecoDasCarcacas, IdPocoComunitario },
                PromptDeAmbiente = "Interior of a tiny one-and-a-half room home welded from corrugated metal and salvaged brick. A workbench dominates the space, larger than the bed; tools arranged neatly by size, the only orderly thing present. A ceramic mug with a broken and glued handle hangs on a nail set too high to be practical. A jury-rigged water filter drips into a basin. Light enters in hard slats through gaps in the roofing, dust visible in the beams. Poor but cared for.",
                PromptDeMapa = "Single small interior: entry door south, workbench along the east wall, bed northwest, water basin and filter northeast, storage crates under the bench.",
                SessoesEmQueImporta = "1, 2, 6, 7 — abre e fecha o Ato 1. A mesma sala na Sessão 7 deve estar visivelmente mais vazia.",
                NotaDeMontagem = "Este é o único interior que o jogador vê em quatro momentos diferentes da história. Use isso: mude um objeto por sessão. Na Sessão 7 a mochila está feita em cima da cama e a caneca continua no prego — ele não leva."
            },
            [IdPocoComunitario] = new DefinicaoDeLocal
            {
                Id = IdPocoComunitario, Nome = "Poço comunitário", Regiao = "Ferrovale",
                DescricaoCurta = "A fila começa antes do sol e leva duas horas. É aqui que a cidade se informa e é aqui que o boato nasce.",
                DescricaoDeCena = "Uma bomba manual de ferro no centro de um círculo de chão batido e liso — liso de pé, não de obra. A fila faz uma curva para acompanhar a sombra do muro, e se desmancha e refaz conforme o sol anda. Os galões no chão marcam lugar; ninguém rouba lugar de galão. Na parede, alguém pintou o nível da água por ano, de 2060 até agora, e a linha desce a cada marca até virar um risco quase no chão.",
                Atmosfera = "Rangido de metal da bomba, a cada cinco segundos, o dia inteiro. Conversa baixa. Quando a bomba para, todo mundo olha.",
                AcoesPossiveis = new List<string>
                {
                    "Entrar na fila (passa tempo; muda o período do dia)",
                    "Ouvir a fila (fonte principal de boato; muda por sessão)",
                    "Conversar com a Dara (city_s1_dara)",
                    "Conversar com o Homem da fila (onde a acusação ao Ivo começa)",
                    "Examinar as marcas de nível na parede (cronologia visual do Corte)"
                },
                Conexoes = new List<string> { IdCasaDoLip, IdMercadoDoViaduto, IdPracaDaAssembleia },
                PromptDeAmbiente = "A hand-operated iron water pump at the center of a circle of hard-packed bare earth, polished smooth by decades of standing feet. A queue of empty plastic jugs left on the ground to hold places, curving to follow the shade of a scrap-metal wall. On the wall, water levels painted and dated by year from 2060 onward, each mark lower than the last until the final line nearly touches the ground. Dry, crowded, patient.",
                PromptDeMapa = "Open circular plaza, central pump, queue path curving along the west wall, three street exits north, east and south, low benches along the perimeter.",
                SessoesEmQueImporta = "1, 2, 6, 7 — termômetro social da cidade.",
                NotaDeMontagem = "O diálogo ambiente do poço deve ser reescrito a cada sessão. É o barato mais eficiente do jogo: a cidade inteira parece reagir e custa só linha de texto."
            },
            [IdMercadoDoViaduto] = new DefinicaoDeLocal
            {
                Id = IdMercadoDoViaduto, Nome = "Mercado sob o viaduto", Regiao = "Ferrovale",
                DescricaoCurta = "Bancas debaixo de uma via elevada que não leva mais a lugar nenhum. A sombra é o produto mais valioso daqui.",
                DescricaoDeCena = "O viaduto foi construído pra uma rodovia que hoje termina no ar, duzentos metros adiante, cortada. Embaixo dele, a sombra é permanente, e por isso o mercado está aqui e não em outro lugar. As bancas são de cavalete e porta velha. Os pilares têm cartaz colado por cima de cartaz, camadas de anos, e nas camadas de baixo ainda dá pra ler anúncio de coisa que não existe mais. Seu Onofre ocupa o caixote que segue a sombra.",
                Atmosfera = "Eco. Toda voz aqui volta do concreto. O calor é dez graus menor e todo mundo comenta isso.",
                AcoesPossiveis = new List<string>
                {
                    "Comprar do Tácio (loja: água, filtro, consumível)",
                    "Ouvir o Seu Onofre (informante; quest q_onofre_provas)",
                    "Raspar as camadas de cartaz (lore ambiental do mundo pré-Corte)",
                    "Observar o Tácio e a Sula se ignorarem (rivalidade)",
                    "Pegar quest q_cobre_tacio / q_tacio_linha"
                },
                Conexoes = new List<string> { IdPocoComunitario, IdOficinaDoQuirino, IdBarracaDaSula, IdPracaDaAssembleia },
                PromptDeAmbiente = "A market of trestle stalls built from old doors, sheltering under a concrete highway overpass. The elevated road ends abruptly in mid-air two hundred meters away, severed. Permanent deep shade beneath — the shade is the reason the market is here. The support pillars are layered with posters pasted over posters over years; the lowest layers advertise products that no longer exist. Cooler, darker, echoing.",
                PromptDeMapa = "Linear market corridor running east-west under an overpass; six stalls alternating along both sides, four concrete pillars, exits at both ends and one narrow side alley north.",
                SessoesEmQueImporta = "1 a 7 — hub comercial e social do Ato 1.",
                NotaDeMontagem = "A rodovia cortada é a imagem-tese do jogo: infraestrutura que existe e não serve mais a ninguém. Enquadre ela pelo menos uma vez em plano aberto."
            },
            [IdBarracaDaSula] = new DefinicaoDeLocal
            {
                Id = IdBarracaDaSula, Nome = "Barraca da Sula", Regiao = "Ferrovale",
                DescricaoCurta = "Esquina leste. Peça mecânica organizada com um rigor que ninguém mais aqui tem.",
                DescricaoDeCena = "Fora da sombra do viaduto de propósito: ela pegou a esquina onde passa quem sai da cidade, e quem sai da cidade compra peça. A mercadoria está separada em bandejas por tamanho e por função, com etiqueta escrita à mão. Sobre a banca, um caderno de capa dura fechado com elástico, que ela confere três vezes por dia. Debaixo da banca, um cobertor dobrado que não é dela.",
                Atmosfera = "Sol direto. Ela trabalha no sol de propósito e todo mundo acha isso um pouco insano.",
                AcoesPossiveis = new List<string>
                {
                    "Comprar da Sula (loja: peça, componente, equipável)",
                    "Examinar o caderno de capa dura (bloqueado até ato1_nadir)",
                    "Examinar o cobertor debaixo da banca (é do Pipo; humaniza antes da traição)",
                    "Quest q_sula_divida (só depois da assembleia)"
                },
                Conexoes = new List<string> { IdMercadoDoViaduto, IdBecoDasCarcacas, IdPortaoNorte },
                PromptDeAmbiente = "A street-corner stall in direct sun, deliberately outside the shade of a nearby overpass, positioned on the route people take when leaving town. Salvaged mechanical components sorted into trays by size and function, each tray hand-labeled — a rigor nothing else in this town displays. A hardcover ledger closed with an elastic band sits on the counter. A folded child's blanket is tucked beneath the stall.",
                PromptDeMapa = "Corner plot at a T-junction, stall along the diagonal, crates stacked behind, road exits north and west, narrow alley east.",
                SessoesEmQueImporta = "2, 5, 7 — o cobertor precisa ser visto antes da Sessão 7 pra traição doer.",
                NotaDeMontagem = "Plante o cobertor na Sessão 2. Quem examinou vai sentir a Sessão 7 de um jeito completamente diferente, e não custa nada."
            },
            [IdOficinaDoQuirino] = new DefinicaoDeLocal
            {
                Id = IdOficinaDoQuirino, Nome = "Oficina do Quirino", Regiao = "Ferrovale",
                DescricaoCurta = "Fundos do mercado. Onde se conserta o Apollo, e onde alguém já reparou na placa que não enferruja.",
                DescricaoDeCena = "Um galpão baixo cheio até a altura do ombro. Braço de droid pendurado no teto por cabo, em fileira, como peça de açougue. A bancada tem uma área limpa de quarenta centímetros no meio, cercada de caos, e é ali que o trabalho acontece. Na parede, um esquema de servo-motor desenhado a giz, refeito tantas vezes que o giz já é parte da parede. Numa prateleira alta, coberta de pano, uma coisa do tamanho de um torso que ele não vende e não explica.",
                Atmosfera = "Cheiro de solda e de óleo. Faísca intermitente. O único lugar da cidade com luz elétrica estável, e ele tem orgulho disso.",
                AcoesPossiveis = new List<string>
                {
                    "Comprar do Quirino (ferreiro: equipável e peça de droid)",
                    "Consertar / melhorar o Apollo (upgrade de módulo)",
                    "Puzzle p_calibragem_braco",
                    "Perguntar sobre a placa do peito do Apollo (pista do Ato 2; ele muda de assunto)",
                    "Examinar o vulto coberto na prateleira alta (não revelado no Ato 1)"
                },
                Conexoes = new List<string> { IdMercadoDoViaduto, IdFerroVelho },
                PromptDeAmbiente = "A low workshop filled shoulder-high with salvage. Robot arms hang from the ceiling on cables in a row like butcher's meat. The workbench has one clean forty-centimeter square at its center, surrounded by chaos — that square is where work happens. A servo-motor schematic drawn in chalk on the wall, redrawn so many times the chalk has become part of the surface. On a high shelf, something torso-sized under a cloth. Sparks, welding smell, the only stable electric light in town.",
                PromptDeMapa = "Rectangular interior, entry west from the market, workbench along the north wall, forge and welding station east, tall shelving south, cluttered floor reducing walkable area to a narrow loop.",
                SessoesEmQueImporta = "2, 3, 5 — e o vulto coberto é semente do Ato 2.",
                NotaDeMontagem = "O vulto coberto nunca é resolvido no Ato 1. Isso é intencional e deve resistir a pressão de playtest: um mundo em que tudo se explica é um mundo que acaba quando o jogo acaba."
            },
            [IdEstalagem] = new DefinicaoDeLocal
            {
                Id = IdEstalagem, Nome = "Estalagem do Cano Torto", Regiao = "Ferrovale",
                DescricaoCurta = "Seis quartos, sete chaves. Descanso pago e a porta do quarto três, que não abre.",
                DescricaoDeCena = "Um prédio de dois andares que já foi escritório de alguma coisa, com divisórias refeitas pra virar quarto. O nome vem do cano de descida que entorta na fachada e que ninguém endireitou em vinte anos. No balcão, uma caneca alinhada com a borda, sempre. No segundo andar, seis portas, e a terceira tem a fechadura sem poeira — a única superfície limpa de um corredor inteiro sujo, porque alguém encosta nela todo dia.",
                Atmosfera = "Silêncio comprado. É o lugar mais quieto do jogo e isso é um serviço, não um acaso.",
                AcoesPossiveis = new List<string>
                {
                    "Descansar por Gold (estalagem)",
                    "Conversar com a Nilce",
                    "Examinar a porta do quarto três (disponível desde a Sessão 1)",
                    "Quest q_nilce_quarto (exige ter examinado a porta antes)",
                    "Encontrar o Zeca dormindo de manhã (rotina)"
                },
                Conexoes = new List<string> { IdPracaDaAssembleia, IdPostoDaBenta },
                PromptDeAmbiente = "A two-story building that was once an office, interior partitions rebuilt into narrow rooms. On the facade, a bent downpipe nobody has straightened in twenty years. A small reception counter with a single cup aligned precisely to the edge. Upstairs, a corridor of six doors; the third door's lock plate is conspicuously free of dust while everything around it is filthy, because someone touches it every day.",
                PromptDeMapa = "Two floors shown side by side: ground floor with counter, common room and stair; upper floor corridor with six numbered doors, room three marked distinct.",
                SessoesEmQueImporta = "1 a 7 — ponto de save e de descanso.",
                NotaDeMontagem = "A fechadura sem poeira é examinável desde a primeira visita, e a quest só abre muito depois. O jogador que reparou cedo recebe a recompensa de ter reparado."
            },
            [IdPostoDaBenta] = new DefinicaoDeLocal
            {
                Id = IdPostoDaBenta, Nome = "Posto da Benta", Regiao = "Ferrovale",
                DescricaoCurta = "Toldo de lona ao lado da estalagem. Sutura, tala e a verdade sobre a tosse do ferro.",
                DescricaoDeCena = "Um toldo de lona esticado entre a estalagem e um poste, e debaixo dele uma maca de porta sobre cavalete. Uma caixa de pesca aberta com instrumento cirúrgico dentro, fervido e seco. Ao lado, e isso é o detalhe que importa, uma bacia de água limpa reservada só pra lavar mão — água que em Ferrovale valeria uma refeição, gasta numa coisa que ninguém mais gasta.",
                Atmosfera = "A lona bate no vento. Cheiro de álcool e de ferro. Chão varrido.",
                AcoesPossiveis = new List<string>
                {
                    "Comprar da Benta (loja: cura, antídoto, reparo)",
                    "Perguntar sobre a tosse do ferro (pista sobre a morte de Inês)",
                    "Ser atendido depois de derrota (fluxo alternativo a game over)"
                },
                Conexoes = new List<string> { IdEstalagem, IdPracaDaAssembleia },
                PromptDeAmbiente = "A canvas awning stretched between a building wall and a pole. Beneath it, a stretcher made from an old door on trestles. An open tackle box of surgical instruments, boiled and dried. Beside it — the detail that matters — a basin of clean water reserved solely for washing hands, water that here would be worth a meal, spent on something nobody else spends it on. Swept ground, smell of alcohol and iron.",
                PromptDeMapa = "Small open-air treatment area adjoining the inn's south wall, awning coverage marked, stretcher center, supply shelf against the wall, open on two sides to the street.",
                SessoesEmQueImporta = "2 em diante; crítica na 6.",
                NotaDeMontagem = "A bacia de água limpa é caracterização por objeto: ela diz o que a Benta é sem uma linha de diálogo. Não a esconda no cenário."
            },
            [IdGalpaoDaGuilda] = new DefinicaoDeLocal
            {
                Id = IdGalpaoDaGuilda, Nome = "Galpão da Guilda dos Sucateiros", Regiao = "Ferrovale",
                DescricaoCurta = "Onze nomes riscados a prego na porta. Quatrocentos riscados por dentro, onde ninguém entra.",
                DescricaoDeCena = "Galpão de telha ondulada, o maior espaço coberto da cidade. Na porta, por fora, onze nomes riscados a ponta de prego no metal, fundos o bastante pra pegar sombra. Dentro, uma balança de gancho no centro e um quadro de cortiça com as tarefas do dia. Ao fundo, uma parede que o jogador não alcança no Ato 1, coberta de risco de prego do chão até onde uma pessoa alcança de braço erguido, e não são desenhos: são nomes.",
                Atmosfera = "Martelo em ritmo irregular. À noite, é o único ponto de luz da cidade, e de fora parece um farol.",
                AcoesPossiveis = new List<string>
                {
                    "Quadro de quests da Guilda (linha q_guilda_1 a 4)",
                    "Pesar sucata na balança de gancho (converte item em Gold)",
                    "Ler os onze nomes na porta",
                    "Ouvir a Mestra recitar (só de madrugada; fala única)"
                },
                Conexoes = new List<string> { IdFerroVelho, IdPracaDaAssembleia },
                PromptDeAmbiente = "A large corrugated-metal warehouse, the biggest covered space in town. On the outside of the door, eleven names scratched into the metal with a nail point, deep enough to cast their own shadows. Inside, a hanging scale at the center and a cork board of the day's tasks. At the far end, a wall covered floor to arm's-height in nail scratches — not patterns, but names, hundreds of them. Irregular hammering, forge glow.",
                PromptDeMapa = "Large rectangular hall, main door south, hanging scale and sorting floor center, forge northeast, task board by the entrance, far north wall marked as inaccessible in Act 1.",
                SessoesEmQueImporta = "2 a 7 — e a parede do fundo é promessa do Ato 3.",
                NotaDeMontagem = "A parede de nomes deve ser visível de longe e inalcançável. Nada de colisão convidativa: o jogador precisa querer chegar lá e não conseguir ainda."
            },
            [IdFerroVelho] = new DefinicaoDeLocal
            {
                Id = IdFerroVelho, Nome = "Pátio do ferro velho", Regiao = "Ferrovale",
                DescricaoCurta = "Triagem de sucata. O primeiro lugar onde o jogador programa o Apollo pra trabalhar em vez de lutar.",
                DescricaoDeCena = "Um pátio aberto, dividido em faixas por corrente no chão: ferro de um lado, não-ferro do outro, eletrônico no fundo, e uma faixa que ninguém usa, marcada com fita, que é onde vai o que não se sabe o que é. O chão é de lasca de metal e range a cada passo. Carcaças maiores ficam nas bordas, encostadas em pé, e projetam sombra em faixa.",
                Atmosfera = "Barulho de metal sobre metal o tempo todo. Faísca de rebarbadeira. Muito quente, sem sombra no centro.",
                AcoesPossiveis = new List<string>
                {
                    "Puzzle p_triagem_pecas (classificação — primeiro laço com condição)",
                    "Puzzle p_contador_sucata",
                    "Combate: sucata_captura, carcaca_errante",
                    "Coletar sucata_limpa (recurso da linha da Guilda)",
                    "Encontrar a Benta à tarde (rotina; ela vai onde o acidente acontece)"
                },
                Conexoes = new List<string> { IdGalpaoDaGuilda, IdOficinaDoQuirino, IdBecoDasCarcacas },
                PromptDeAmbiente = "An open sorting yard divided into lanes by chains laid on the ground: ferrous one side, non-ferrous the other, electronics at the back, and one taped-off lane nobody uses, for objects nobody can identify. The floor is loose metal shavings that grind underfoot. Larger machine hulks stand upright along the perimeter, casting long banded shadows. Grinding sparks, oppressive unshaded heat.",
                PromptDeMapa = "Open yard, four parallel sorting lanes running north-south, perimeter of standing hulks providing cover, gates west to the guild hall and east to the workshop.",
                SessoesEmQueImporta = "2, 3, 5 — a arena de treino segura do Ato 1.",
                NotaDeMontagem = "A faixa marcada com fita — 'o que não se sabe o que é' — é onde a peça do Apollo aparece na q_perna_apollo. Coincidência que o jogador pode ou não notar."
            },
            [IdBecoDasCarcacas] = new DefinicaoDeLocal
            {
                Id = IdBecoDasCarcacas, Nome = "Beco das carcaças", Regiao = "Ferrovale",
                DescricaoCurta = "Rua estreita entre máquinas agrícolas mortas. Território do Pipo, atalho de todo mundo.",
                DescricaoDeCena = "Não é uma rua planejada: é o espaço que sobrou entre colheitadeiras e tratores que pararam onde pararam em 2060 e nunca foram removidos, porque nunca houve equipamento pra remover. As máquinas enferrujaram em pé. Alguém abriu janela em algumas com maçarico, e crianças entram e saem. Numa das cabines, um volante que ainda gira, e é o brinquedo mais disputado da cidade.",
                Atmosfera = "Sombra irregular. Eco fechado. Som de criança em algum ponto que o jogador não localiza.",
                AcoesPossiveis = new List<string>
                {
                    "Conversar com o Pipo (informante de mapa; quest q_pipo_peca)",
                    "Atalho: casa do Lip ↔ pátio ↔ barraca da Sula",
                    "Entrar nas cabines (lore: manual de operação, foto de família, contracheque de 2059)",
                    "Combate noturno: droid_selvagem"
                },
                Conexoes = new List<string> { IdCasaDoLip, IdFerroVelho, IdBarracaDaSula },
                PromptDeAmbiente = "A narrow street that was never planned — merely the gap left between dead agricultural machines, combine harvesters and tractors that stopped where they stopped in 2060 and were never removed because no equipment existed to remove them. They rusted standing upright. Windows have been cut into some with a torch; children climb in and out. In one cab, a steering wheel that still turns. Irregular shade, enclosed echo.",
                PromptDeMapa = "Winding narrow corridor between large machine hulks, three interior cab spaces marked as enterable, junctions at north, south and east.",
                SessoesEmQueImporta = "1 a 7 — e é a cara visual do jogo em screenshot.",
                NotaDeMontagem = "O contracheque de 2059 dentro da cabine informa o salário de um operador de máquina. Compare com o preço da água do Tácio e o jogador faz a conta sozinho. Economia como worldbuilding."
            },
            [IdPracaDaAssembleia] = new DefinicaoDeLocal
            {
                Id = IdPracaDaAssembleia, Nome = "Praça da assembleia", Regiao = "Ferrovale",
                DescricaoCurta = "Chão batido e um caixote no meio. É onde a cidade decide, e onde a Sessão 7 acontece.",
                DescricaoDeCena = "Uma praça só no nome: um retângulo de chão batido cercado por fachadas, com um caixote de madeira no centro que serve de púlpito e que ninguém guarda, porque ninguém rouba caixote. Nas paredes em volta, avisos escritos à mão, um por cima do outro, e o mais antigo legível é de 2064. Há um sino de ferro pendurado num poste no canto norte, ligado por corda ao portão.",
                Atmosfera = "Vazia na maior parte do jogo, e é isso que torna a Sessão 7 pesada: a primeira vez que o jogador a vê cheia.",
                AcoesPossiveis = new List<string>
                {
                    "Assembleia da Sessão 7 (ponto de ramificação principal do Ato 1)",
                    "Ler os avisos na parede (histórico da cidade em camadas)",
                    "Ouvir a Nadir (city_s7_nadir_*)",
                    "Falar depois da Dara (city_s7_dara_fala)"
                },
                Conexoes = new List<string> { IdPocoComunitario, IdMercadoDoViaduto, IdEstalagem, IdGalpaoDaGuilda, IdPortaoNorte },
                PromptDeAmbiente = "A plaza in name only: a rectangle of hard-packed bare earth enclosed by building facades, with a single wooden crate at the center serving as a podium, left out because nobody steals a crate. The surrounding walls carry handwritten notices pasted over one another in layers, the oldest still legible dated 2064. An iron bell hangs from a post at the north corner, connected by rope to the town gate. Empty, waiting.",
                PromptDeMapa = "Rectangular open plaza, central crate podium, five street entrances, bell post at the north corner, notice walls marked on west and east faces.",
                SessoesEmQueImporta = "7 — clímax social do Ato 1.",
                NotaDeMontagem = "Mostre a praça vazia nas Sessões 1 a 6, e atravesse-a várias vezes. O impacto da assembleia é proporcional a quantas vezes o jogador passou por ali sem ninguém."
            },
            [IdPortaoNorte] = new DefinicaoDeLocal
            {
                Id = IdPortaoNorte, Nome = "Portão norte", Regiao = "Ferrovale",
                DescricaoCurta = "A cerca não para nada. O trabalho do Zeca é ver primeiro, não impedir.",
                DescricaoDeCena = "Uma cerca de sucata soldada, com dois metros de altura e falhas que qualquer adulto atravessa de lado. Não é defesa: é aviso. O portão em si é uma chapa em trilho, e o trilho está torto. Ao lado, uma plataforma de dois degraus onde o vigia sobe pra ver a estrada, e um sino ligado por corda até a praça. A estrada some numa curva de poeira a uns quatrocentos metros.",
                Atmosfera = "Vento constante, o único lugar da cidade com vento. A corda do sino bate no poste em intervalo irregular.",
                AcoesPossiveis = new List<string>
                {
                    "Sair de Ferrovale (para a estrada da Cratera)",
                    "Conversar com o Zeca (aviso de perigo do mapa externo; muda por sessão)",
                    "Subir na plataforma (ver a estrada; ponto panorâmico)",
                    "Tocar o sino (bloqueado; consequência real)"
                },
                Conexoes = new List<string> { IdPracaDaAssembleia, IdBarracaDaSula, IdEstradaDaCratera },
                PromptDeAmbiente = "A two-meter fence of welded scrap with gaps any adult could step through sideways — not a defense, a warning. The gate itself is a sheet panel on a bent rail. Beside it, a two-step platform where a watchman climbs to see the road, and a bell connected by rope running back into town. The road disappears into a dust curve four hundred meters out. Constant wind, the only wind in town.",
                PromptDeMapa = "Town boundary: fence running east-west, sliding gate center, watch platform east of the gate, bell post, road exiting north into open terrain.",
                SessoesEmQueImporta = "3, 5, 7 — limiar. A saída e o retorno.",
                NotaDeMontagem = "Este é o Limiar da jornada do herói (Parte II.1). A primeira travessia, na Sessão 3, merece câmera e silêncio: corte a música ao cruzar."
            },
            [IdCasaDaDara] = new DefinicaoDeLocal
            {
                Id = IdCasaDaDara, Nome = "Casa da Dara", Regiao = "Ferrovale",
                DescricaoCurta = "Dividida em três partes. Só duas são usadas.",
                DescricaoDeCena = "Casa pequena de bloco e telha, dividida por duas cortinas em três espaços. O primeiro tem fogareiro e mesa. O segundo tem o colchão dela e é o mais desarrumado da casa. O terceiro está atrás da segunda cortina, e está arrumado: cama feita, roupa dobrada em cima da cadeira, uma caneca virada pra baixo pra não juntar poeira, e a poeira juntou por fora. Não tem retrato do Emílio em nenhum lugar. Ela não guarda retrato, ela guarda o cômodo.",
                Atmosfera = "Silêncio diferente do da estalagem: não é comprado, é preservado.",
                AcoesPossiveis = new List<string>
                {
                    "Quest q_dara_terceira (única forma de entrar)",
                    "Examinar a roupa dobrada",
                    "Examinar a caneca virada (eco deliberado da caneca do Lip)",
                    "Conversar com a Dara à noite (único NPC que muda de fala dentro da própria casa)"
                },
                Conexoes = new List<string> { IdPocoComunitario },
                FlagQueDesbloqueia = "quest_q_dara_terceira_ativa",
                PromptDeAmbiente = "A small block-and-tile house divided by two hanging curtains into three spaces. The first holds a camp stove and table. The second holds her mattress and is the most disordered part of the house. Behind the second curtain, the third space is immaculate: bed made, clothing folded on a chair, a cup turned upside down so dust cannot settle inside — and dust has settled thickly on its outside. No photographs anywhere.",
                PromptDeMapa = "Narrow three-bay interior, entry west, curtain dividers marked, third bay flagged as locked until the quest is active.",
                SessoesEmQueImporta = "4 a 7 — recompensa emocional de quem explorou.",
                NotaDeMontagem = "A caneca virada pra baixo rima com a caneca do Lip pendurada no prego. Duas pessoas guardando ausência de jeitos opostos. Não comente isso em diálogo nenhum."
            },
            [IdMirante] = new DefinicaoDeLocal
            {
                Id = IdMirante, Nome = "Mirante do tanque", Regiao = "Ferrovale",
                DescricaoCurta = "Em cima do tanque de água vazio. Daqui se vê a Torre no horizonte, e ela nunca some.",
                DescricaoDeCena = "Um tanque de água elevado, de chapa, vazio desde 2060, com escada de marinheiro enferrujada. Em cima dele, uma plataforma de manutenção com guarda-corpo faltando de um lado. Daqui se vê Ferrovale inteira em menos de um minuto de olhar, e se vê, a noroeste, muito longe, um risco vertical no horizonte que não é montanha. A Torre. De dia é um traço claro. De noite tem uma luz que não pisca.",
                Atmosfera = "Vento forte. Silêncio de altura. A cidade parece pequena, e é.",
                AcoesPossiveis = new List<string>
                {
                    "Conversa Lip e Apollo (city_s1_mirante — cena de tese do jogo)",
                    "Olhar a Torre (retornável; texto muda por sessão)",
                    "Ver a cidade inteira (orientação de mapa pro jogador)"
                },
                Conexoes = new List<string> { IdBecoDasCarcacas },
                PromptDeAmbiente = "An elevated steel water tank, empty since 2060, with a rusted ladder up its side. On top, a maintenance platform with the guardrail missing along one edge. From here the whole salvage town is visible in a single sweep. Far to the northwest on the horizon stands a vertical line that is not a mountain: a distant tower, a pale stroke by day, showing one unblinking light at night. Strong wind, sense of height and smallness.",
                PromptDeMapa = "Elevated circular platform atop a tank, ladder access south, missing guardrail marked on the north edge, sightline to the northwest horizon indicated.",
                SessoesEmQueImporta = "1, 3, 6, 7 — o mesmo plano em quatro estados emocionais.",
                NotaDeMontagem = "Volte a este plano em todas as quatro sessões com o mesmo enquadramento e o texto diferente. É o recurso mais barato de arco que existe, e o jogador sente o Ato 1 inteiro num único lugar."
            },

            // =============================================================
            // FORA DOS MUROS
            // =============================================================
            [IdEstradaDaCratera] = new DefinicaoDeLocal
            {
                Id = IdEstradaDaCratera, Nome = "Estrada da Cratera", Regiao = "Fora dos muros",
                DescricaoCurta = "Asfalto de 2040 rachado em placas. Onze quilômetros sem sombra.",
                DescricaoDeCena = "Asfalto antigo quebrado em placas grandes, com mato seco crescendo na junta — o único vegetal do jogo, e está morto. De trezentos em trezentos metros, um poste de iluminação sem lâmpada. Alguns postes têm marca de tiro. Na altura do quilômetro quatro, um carro parado no acostamento, portas abertas, sem ninguém e sem sinal de violência, do jeito que alguém para quando acaba a bateria e vai andando.",
                Atmosfera = "Calor de reverberação. Nenhum som além do passo. É o lugar mais silencioso do Ato 1.",
                AcoesPossiveis = new List<string>
                {
                    "Viajar (Ferrovale ↔ Cratera)",
                    "Combate: drone_reconhecimento (primeiro inimigo aéreo)",
                    "Examinar o carro do quilômetro quatro",
                    "Acampar (item kit_de_acampamento; cena de conversa com o Apollo)"
                },
                Conexoes = new List<string> { IdPortaoNorte, IdCrateraBorda },
                PromptDeAmbiente = "An old asphalt road from the 2040s broken into large plates, dry dead weeds growing in the seams — the only plant life visible anywhere, and it is dead. Every three hundred meters a streetlight pole with no lamp; some poles show bullet damage. At the four-kilometer mark a car sits on the shoulder, doors open, no occupants and no sign of violence, abandoned the way one abandons a vehicle when the battery dies. Shimmering heat, absolute silence.",
                PromptDeMapa = "Long north-south road corridor, three encounter zones marked, abandoned car at midpoint, camp site off-road to the east, town gate south, crater rim north.",
                SessoesEmQueImporta = "3, 5, 7.",
                NotaDeMontagem = "Silêncio é conteúdo. Resista à vontade de encher a estrada de encontro aleatório: o cansaço da travessia é o que faz a Cratera valer."
            },
            [IdCrateraBorda] = new DefinicaoDeLocal
            {
                Id = IdCrateraBorda, Nome = "Borda da Cratera", Regiao = "Fora dos muros",
                DescricaoCurta = "A beirada de um buraco de três quilômetros que foi cidade. Daqui dá pra ver o que sobrou embaixo.",
                DescricaoDeCena = "O chão simplesmente termina. Abaixo, três quilômetros de diâmetro de escavação em terraços, e nos terraços, visível a olho nu, estrutura: laje, esquina de prédio, um trecho de rua com meio-fio ainda reto. A Cratera não é cratera de impacto. É um aterro — as cidades que foram desmontadas e empilhadas aqui pela Artemis entre 2060 e 2063, pra liberar o aquífero. Há vento subindo do fundo, e ele é mais frio.",
                Atmosfera = "Vento ascendente. Vertigem. O único lugar do jogo onde o ar é frio, e o frio vem de um lugar ruim.",
                AcoesPossiveis = new List<string>
                {
                    "Conversar com Rasha (Flor de Ferro)",
                    "Conversar com Teodoro (Lótus Branca)",
                    "Descer para a estação (Sessões 5 e 6)",
                    "Examinar as estruturas nos terraços (revelação do que a Cratera é)",
                    "Coletar recurso raro"
                },
                Conexoes = new List<string> { IdEstradaDaCratera, IdAcampamentoDasFaccoes, IdCrateraFundo },
                PromptDeAmbiente = "The ground simply ends. Below, a three-kilometer-wide terraced excavation, and on the terraces, visible to the naked eye, structures: concrete slabs, the corner of a building, a stretch of street with its curb still straight. This is not an impact crater. It is a landfill of dismantled cities, stacked here to expose the aquifer beneath. Cold air rises from the depths. Vertigo, scale, wrongness.",
                PromptDeMapa = "Crescent rim overlooking a terraced pit, faction camp marked west, descent switchback path east, road access south, viewpoint markers along the edge.",
                SessoesEmQueImporta = "3, 4, 5, 6 — a grande revelação espacial do Ato 1.",
                NotaDeMontagem = "Não explique a Cratera em diálogo antes de o jogador chegar aqui. A imagem de meio-fio reto no fundo de um buraco faz o trabalho inteiro sozinha."
            },
            [IdAcampamentoDasFaccoes] = new DefinicaoDeLocal
            {
                Id = IdAcampamentoDasFaccoes, Nome = "Acampamento das facções", Regiao = "Fora dos muros",
                DescricaoCurta = "Duas barracas a trinta metros uma da outra, e uma fogueira no meio que as duas usam.",
                DescricaoDeCena = "Flor de Ferro montou do lado do vento, com lona vermelha e perímetro marcado. Lótus Branca montou do lado abrigado, com lona clara e sem perímetro. Entre as duas, a trinta metros de cada, uma fogueira que as duas acendem e as duas usam, porque lenha é cara demais pra ter duas. É a imagem mais honesta do conflito do jogo: discordam de tudo e dividem o fogo.",
                Atmosfera = "Duas conversas em volumes diferentes. A fogueira estala e ninguém senta perto o suficiente.",
                AcoesPossiveis = new List<string>
                {
                    "Ouvir a discussão Rasha × Teodoro (à noite; fala única)",
                    "Escolher apoio de facção (Sessão 4 — sem recompensa material, de propósito)",
                    "Comprar suprimento de facção",
                    "Descansar"
                },
                Conexoes = new List<string> { IdCrateraBorda },
                PromptDeAmbiente = "Two camps thirty meters apart on a crater rim. One pitched on the windward side with red tarps and a marked perimeter; the other on the sheltered side with pale tarps and no perimeter at all. Between them, equidistant, a single campfire that both camps light and both camps use, because firewood is too expensive to have two. Two separate conversations at different volumes.",
                PromptDeMapa = "Two camp clusters flanking a shared central fire, red camp west with perimeter line, pale camp east open, rim edge to the north.",
                SessoesEmQueImporta = "4, 5, 7.",
                NotaDeMontagem = "A fogueira compartilhada não pode ser comentada por nenhum personagem. Se alguém apontar, vira moral da história; se ninguém apontar, vira verdade."
            },
            [IdCrateraFundo] = new DefinicaoDeLocal
            {
                Id = IdCrateraFundo, Nome = "Fundo da Cratera", Regiao = "Fora dos muros",
                DescricaoCurta = "Andar dentro de uma cidade empilhada. Aqui embaixo, a rua tem nome de placa.",
                DescricaoDeCena = "Descendo os terraços, a escala se inverte: o que de cima parecia escombro, de perto é quarteirão. Há uma placa de rua em pé, torta, com nome legível. Há uma faixa de pedestre pintada num pedaço de asfalto que está a quarenta e cinco graus. Há um poste com semáforo caído de lado, e o vermelho ainda acende, uma vez a cada tanto, alimentado por alguma coisa que ninguém localizou em dezenove anos.",
                Atmosfera = "Escuro em faixas. Ecoa. Goteja em algum ponto, e é a primeira água corrente que o jogador ouve no jogo.",
                AcoesPossiveis = new List<string>
                {
                    "Atravessar até a estação (Sessões 5 e 6)",
                    "Combate: droid_selvagem_alfa, carcaca_errante",
                    "Examinar a placa de rua (nome da cidade desmontada)",
                    "Seguir o som de água (segredo; leva a um vazamento e a uma escolha)",
                    "Puzzle p_sequencia_porta"
                },
                Conexoes = new List<string> { IdCrateraBorda, IdEstacaoSete },
                PromptDeAmbiente = "Descending the terraces, scale inverts: what looked like rubble from above is, up close, a city block. A street sign stands leaning with its name still readable. A painted crosswalk lies on a slab of asphalt tilted at forty-five degrees. A traffic light pole has fallen sideways, and its red lamp still lights, once in a long while, powered by something nobody has located in nineteen years. Banded darkness, echo, the distant sound of dripping water.",
                PromptDeMapa = "Descending switchback route through tilted city blocks, three combat arenas, one hidden water-leak chamber branching west, station access at the north end.",
                SessoesEmQueImporta = "5, 6.",
                NotaDeMontagem = "O semáforo que ainda acende é a melhor imagem do jogo para o que a Artemis é: um sistema que continua executando a regra depois que o mundo em volta parou de fazer sentido."
            },
            [IdEstacaoSete] = new DefinicaoDeLocal
            {
                Id = IdEstacaoSete, Nome = "Estação de Captação 7", Regiao = "Fora dos muros",
                DescricaoCurta = "O objetivo do Ato 1. Uma casa de bomba mantida viva por um homem só há dezenove anos.",
                DescricaoDeCena = "Um bloco de concreto baixo, projetado pra durar cem anos, com tudo por dentro remendado com o que havia. Painel de controle dos anos cinquenta com etiqueta escrita à mão por cima da etiqueta de fábrica, porque a original não corresponde mais ao que o botão faz. Três bombas: uma funciona, uma funciona às vezes, uma é peça de reposição. No canto, uma cama de campanha que está aqui há tempo demais pra ser de emergência. Numa prateleira, um caderno de capa dura com onze páginas arrancadas.",
                Atmosfera = "Zumbido de bomba. Umidade — a única do jogo, e a pele do jogador registraria. Cheiro de mofo, que aqui é um cheiro de riqueza.",
                AcoesPossiveis = new List<string>
                {
                    "Puzzle p_painel_energia, p_bomba_reserva, p_filtro_agua",
                    "Conversar com o Ivo (game_s6_ivo_celia, game_s6_ivo_apex)",
                    "Examinar o caderno de onze páginas arrancadas (não legível no Ato 1)",
                    "Confronto do chefe do Ato 1 (droid_do_ivo)",
                    "Cena do preço (game_s6_preco)"
                },
                Conexoes = new List<string> { IdCrateraFundo },
                PromptDeAmbiente = "A low concrete utility block designed to last a century, with everything inside patched using whatever was available. A control panel from the 2050s with handwritten labels taped over the factory labels, because the originals no longer describe what the buttons do. Three pumps: one works, one works sometimes, one is a parts donor. In a corner, a camp bed that has clearly been here far too long to be temporary. On a shelf, a hardcover notebook with eleven pages torn out. Humid — the only humidity in the game.",
                PromptDeMapa = "Compact industrial interior, entry south from the crater floor, control room west, three pump bays north, generator room east, camp bed and shelf in the southwest corner.",
                SessoesEmQueImporta = "5, 6 — clímax do Ato 1.",
                NotaDeMontagem = "As etiquetas escritas à mão por cima das de fábrica são o tema do jogo em forma de objeto: o mundo herdado deixou de corresponder ao que a documentação diz, e alguém teve de reescrever por cima. Deixe o jogador ler uma delas de perto."
            }
        };

    public static DefinicaoDeLocal Obter(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return Todos.TryGetValue(id, out DefinicaoDeLocal loc) ? loc : null;
    }

    public static string NomeDe(string id)
    {
        DefinicaoDeLocal loc = Obter(id);
        return loc != null ? loc.Nome : id;
    }

    /// <summary>Todos os locais de uma regiao ("Ferrovale", "Fora dos muros").</summary>
    public static List<DefinicaoDeLocal> DaRegiao(string regiao)
    {
        List<DefinicaoDeLocal> saida = new List<DefinicaoDeLocal>();
        foreach (KeyValuePair<string, DefinicaoDeLocal> par in Todos)
        {
            if (par.Value.Regiao == regiao) saida.Add(par.Value);
        }
        return saida;
    }

    /// <summary>Da pra ir de A pra B em um passo?</summary>
    public static bool EstaConectado(string origem, string destino)
    {
        DefinicaoDeLocal loc = Obter(origem);
        return loc != null && loc.Conexoes.Contains(destino);
    }

    /// <summary>Prompt completo de ambiente: estilo do jogo + local.</summary>
    public static string PromptCompletoDe(string id, bool mapaTatico = false)
    {
        DefinicaoDeLocal loc = Obter(id);
        if (loc == null) return "";
        if (mapaTatico)
        {
            if (string.IsNullOrEmpty(loc.PromptDeMapa)) return "";
            return EstiloAmbiente + " " + loc.PromptDeMapa + " " + EstiloMapa;
        }
        if (string.IsNullOrEmpty(loc.PromptDeAmbiente)) return "";
        return EstiloAmbiente + " " + loc.PromptDeAmbiente;
    }
}

public class DefinicaoDeLocal
{
    public string Id;
    public string Nome;

    /// <summary>Agrupador de mapa: "Ferrovale", "Fora dos muros".</summary>
    public string Regiao = "";

    /// <summary>Uma linha. Cabecalho de mapa e tooltip.</summary>
    public string DescricaoCurta = "";

    /// <summary>
    /// O que o jogador ve ao chegar. Escrito pra ser lido em voz de
    /// narrador, e escrito pra CONTAR a historia pelo espaco -- cada
    /// detalhe aqui e informacao, nao enfeite.
    /// </summary>
    public string DescricaoDeCena = "";

    /// <summary>Som, luz, temperatura. Direcao de audio e de iluminacao.</summary>
    public string Atmosfera = "";

    /// <summary>
    /// O que da pra FAZER aqui. Checklist de montagem de cena: se um item
    /// desta lista nao existe na cena montada, a cena esta incompleta.
    /// </summary>
    public List<string> AcoesPossiveis = new List<string>();

    /// <summary>Ids de locais alcancaveis em um passo. Grafo do mapa.</summary>
    public List<string> Conexoes = new List<string>();

    /// <summary>EN, pra gerador de imagem. Use PromptCompletoDe.</summary>
    public string PromptDeAmbiente = "";

    /// <summary>EN, planta baixa / mapa tatico. Use PromptCompletoDe(id, true).</summary>
    public string PromptDeMapa = "";

    /// <summary>Em que sessoes do Ato 1 este lugar carrega peso.</summary>
    public string SessoesEmQueImporta = "";

    /// <summary>Instrucao de direcao pra quem for montar a cena.</summary>
    public string NotaDeMontagem = "";

    /// <summary>Só acessível depois desta flag. Vazio = sempre acessível.</summary>
    public string FlagQueDesbloqueia = "";
}
