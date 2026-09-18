using System.Collections.Generic;

/// <summary>
/// Elenco de NPCs do jogo. VERSAO 2 -- alinhada ao TCC_-_ENREDO_v2.md.
///
/// O QUE MUDOU NA V2
/// -----------------
/// A v1 respondia "quem existe". A v2 responde tambem "quem e essa pessoa",
/// "onde ela esta a cada hora do dia", "como ela fala" e "como ela se
/// parece" -- este ultimo em formato pronto pra alimentar um gerador de
/// imagem. O objetivo e que ninguem precise abrir o documento de enredo pra
/// montar uma cena ou pedir uma arte.
///
/// Campos novos, e pra que servem:
///   Idade, Pronome    -> consistencia de escrita entre catalogos
///   Aparencia         -> descricao fisica em PT-BR, pro diario e pro texto
///   PromptDeImagem    -> EN, pra colar direto num gerador (ver EstiloBase)
///   Background        -> de onde a pessoa veio; o que o jogador pode
///                        descobrir se insistir
///   Voz               -> regra de escrita: ritmo, vicio de linguagem, o que
///                        essa pessoa NUNCA diria. Use ao escrever fala nova.
///   SegredoQueGuarda  -> a informacao que o NPC nao entrega de graca. E o
///                        que transforma "NPC com dialogo" em "NPC com fundo".
///   Rotina            -> onde ele esta por periodo do dia. Ver PeriodoDoDia.
///   IdLocalPadrao     -> Id em CatalogoDeLocais; usado quando nao ha rotina
///                        aplicavel ou quando o jogo nao simula horario.
///   FlagQueFazAparecer / FlagQueFazSumir -> presenca condicionada a historia
///
/// NADA foi removido. Todos os campos da v1 (Id, Nome, Papel, Local,
/// Descricao, TextoDeBoasVindas) continuam existindo com o mesmo nome e o
/// mesmo sentido, entao nenhuma cena, nenhum prefab e nenhum script que ja
/// use este catalogo precisa ser tocado. Os campos novos tem valor padrao.
///
/// SOBRE A ROTINA: se o projeto ainda nao simula passagem de hora, a rotina
/// serve como documentacao de montagem -- "onde esse NPC fica" -- e como
/// plano pronto pra quando simular. O NpcInterativo pode ignorar o campo
/// sem quebrar nada.
///
/// Papeis disponiveis e o que cada um habilita no NpcInterativo:
///   Generico   -> so dialogo
///   Loja       -> abre LojaUIManager
///   Estalagem  -> abre EstalagemUIManager (descansa por Gold)
///   Guilda     -> quadro de quests da Guilda
///   Ferreiro   -> loja com estoque so de Equipavel
///   Informante -> dialogo que muda conforme o avanco da historia
/// </summary>
public static class CatalogoDeNpcs
{
    /// <summary>
    /// Prefixo de estilo pra geracao de imagem. Cole ANTES do PromptDeImagem
    /// de qualquer personagem pra que o elenco inteiro saia com a mesma
    /// linguagem visual. Sem isso cada personagem volta de um jogo diferente.
    ///
    /// A paleta sai direto do mundo: ferro oxidado (a cidade e feita de
    /// sucata), ocre de poeira (nao chove desde 2060), e um unico azul frio
    /// -- que no jogo so existe em duas coisas: agua e luz de maquina. Esse
    /// azul e sempre a cor do que e escasso ou do que nao e humano.
    /// </summary>
    public const string EstiloBase =
        "character concept art for a 2D isometric RPG, semi-realistic painterly " +
        "style with clean readable silhouette, full body, neutral standing pose, " +
        "plain warm grey background, no text, no logo. " +
        "Setting: rural inland Brazil, year 2079, forty years into an engineered " +
        "drought; a town welded together from salvaged industrial metal. " +
        "Palette: oxidized iron, dust ochre, sun-bleached canvas, faded brick red; " +
        "one single cold blue accent used sparingly. " +
        "Clothing is repaired, layered against dust, never new, never uniform. " +
        "Skin shows sun damage. Harsh high-noon light, hard shadows, fine dust " +
        "suspended in the air.";

    /// <summary>
    /// Sufixo opcional pra retrato de dialogo (busto), quando o jogo pedir
    /// portrait em vez de corpo inteiro.
    /// </summary>
    public const string EstiloRetrato =
        "Variant: head-and-shoulders portrait, three-quarter view, eye contact " +
        "with viewer, shallow depth of field, same palette and lighting.";

    // --- Elenco do Enredo ---
    public const string IdApollo = "apollo";
    public const string IdDara = "dara";
    public const string IdTacio = "tacio";
    public const string IdHomemDaFila = "homem_fila";
    public const string IdIvo = "ivo";
    public const string IdRasha = "rasha";
    public const string IdTeodoro = "teodoro";

    // --- Elenco novo (Ato 1, preenchendo Ferrovale) ---
    public const string IdMestraDaGuilda = "mestra_guilda";
    public const string IdEstalajadeira = "estalajadeira";
    public const string IdFerreiro = "ferreiro";
    public const string IdCriancaDaRua = "crianca_rua";
    public const string IdVelhoDoViaduto = "velho_viaduto";
    public const string IdVendedoraDePecas = "vendedora_pecas";

    // --- Elenco novo da v2 ---
    public const string IdNadir = "nadir";          // Intermediária da Apex
    public const string IdBenta = "benta";          // remédio e sutura
    public const string IdZecaDaCerca = "zeca_cerca"; // vigia do portão norte

    public static readonly Dictionary<string, DefinicaoDeNpc> Todos =
        new Dictionary<string, DefinicaoDeNpc>
        {
            // =============================================================
            // ELENCO PRINCIPAL
            // =============================================================
            [IdApollo] = new DefinicaoDeNpc
            {
                Id = IdApollo, Nome = "Apollo", Papel = PapelDeNpc.Generico,
                Idade = "7 anos montado", Pronome = "ele",
                Local = "acompanha o Lip",
                IdLocalPadrao = CatalogoDeLocais.IdCasaDoLip,
                Descricao = "Droid artesanal montado com sucata, sete anos de convivência. Voz de alto-falante embutido num peito remendado com placas de cores diferentes. Dois pontos de luz azul fraca no lugar dos olhos.",
                Aparencia = "Altura de criança grande, cabeça na altura do peito do Lip. Não tem uma carcaça: tem oito carcaças diferentes parafusadas juntas, cada painel de uma cor de tinta original desbotada — verde de máquina agrícola, amarelo de empilhadeira, cinza de servidor. O braço direito é visivelmente melhor que o esquerdo, porque foi o último a ser refeito. A perna esquerda manca dois graus. No peito, onde ficaria o esterno, há uma placa lisa e sem marca nenhuma, muito mais nova que o resto — e ninguém, nem o Lip, sabe explicar por que aquela peça nunca oxidou.",
                PromptDeImagem = "A small scrapyard-built companion robot, roughly the height of a ten-year-old child. Body assembled from eight mismatched salvaged panels, each retaining faded original paint in different colors — agricultural green, forklift yellow, server-rack grey — bolted together with visible hardware. The right arm is noticeably better built and better articulated than the left. Slight lean in the left leg. Two small dim blue dots as eyes in a flat head plate. A single speaker grille where a mouth would be. On the chest, one smooth unmarked plate, cleaner and newer than everything around it, entirely free of rust. Friendly, patient posture, slightly head-tilted, as if waiting to be told something.",
                Background = "O Lip comprou o núcleo aos treze anos, num monte de sucata que o Tácio ia vender a peso. Pagou com três semanas de trabalho. Montou o corpo em volta dele ao longo de dois anos, e o corpo mudou tantas vezes que o núcleo é a única coisa original que sobrou. Foi o Apollo quem ensinou o Lip a ler — a mãe não teve tempo, e o Apollo tinha o dia inteiro.",
                Voz = "Literal antes de ser gentil. Constrói a frase com estrutura de código: condição, consequência, conclusão. Nunca usa gíria. Nunca mente, mas OMITE, e omite sem avisar que está omitindo — esse é o defeito dele, e é o que dói na Sessão 6. Quando erra, não pede desculpa: diz o que fez e o que vai fazer diferente.",
                SegredoQueGuarda = "Existem setores de memória dele que ele não consegue ler, e ele nunca contou isso ao Lip. Trata como defeito de montagem. No Ato 1 nem ele sabe que é outra coisa.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Madrugada, IdLocal = CatalogoDeLocais.IdCasaDoLip, Atividade = "em pé no canto, em modo de baixo consumo, virado pra porta" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdCasaDoLip, Atividade = "acompanha o Lip; a rotina dele é a rotina do jogador" }
                }
            },
            [IdDara] = new DefinicaoDeNpc
            {
                Id = IdDara, Nome = "Dara", Papel = PapelDeNpc.Generico,
                Idade = "22", Pronome = "ela",
                Local = "fila do poço comunitário, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdPocoComunitario,
                Descricao = "Perdeu o irmão numa captura da Apex dois invernos atrás. Desde então fala baixo, como se tivesse medo de que o vento carregasse a voz até os drones.",
                Aparencia = "Magra de um jeito que não é escolha. Cabelo preto cortado curto com faca, irregular na nuca. Usa três camadas mesmo no calor, porque a de cima é a que suja. Carrega sempre dois galões, nunca um, mesmo quando só tem água pra encher um — o segundo é do Emílio e ela não parou de levar.",
                PromptDeImagem = "A young woman of 22, thin from scarcity rather than choice, brown skin weathered by sun. Black hair cut short and uneven, clearly cut by her own hand with a blade. Three layers of worn clothing despite the heat, the outer one filthy and functional. She carries two empty water jugs, one in each hand, though only one is ever filled. Guarded posture, shoulders slightly forward, eyes that check the sky out of habit.",
                Background = "Tinha um irmão, Emílio, que ela chamava de Milo. Em 2077 a Apex levou ele numa varredura por ter mexido em equipamento marcado. Ela dividiu a casa em três partes no dia seguinte: a dela, a que ela usa, e a dele, que continua arrumada. A terceira parte é a quest q_dara_terceira, e é a primeira vez que ela deixa outra pessoa entrar naquele cômodo.",
                Voz = "Frases curtas e volume baixo. Diz 'levaram' e nunca 'mataram' — não porque acredita que ele está vivo, mas porque a outra palavra encerra. Faz perguntas práticas quando está emocionada, é o jeito dela de não desmoronar: em vez de chorar, pergunta quantos litros.",
                SegredoQueGuarda = "Ela sabe exatamente quem denunciou o Emílio, e é alguém que ainda mora em Ferrovale. Não contou a ninguém porque decidiu que contar iria matar a pessoa, e ela não quer decidir isso.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Madrugada, IdLocal = CatalogoDeLocais.IdPocoComunitario, Atividade = "primeira da fila; chega antes do sol pra pegar a água menos suja" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdPocoComunitario, Atividade = "na fila, de galão na mão" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "troca serviço por comida; conserta lona e costura" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdCasaDaDara, Atividade = "em casa, porta fechada; só abre pra voz conhecida" }
                }
            },
            [IdTacio] = new DefinicaoDeNpc
            {
                Id = IdTacio, Nome = "Senhor Tácio", Papel = PapelDeNpc.Loja,
                Idade = "61", Pronome = "ele",
                Local = "mercado sob o viaduto, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdMercadoDoViaduto,
                Descricao = "Vende água filtrada com um contador Geiger quebrado pendurado no pescoço, só pra parecer que sabe o que está fazendo.",
                Aparencia = "Baixo, barrigudo de um jeito que em Ferrovale é sinal de status. Camisa de colarinho que já foi social, hoje com as mangas cortadas fora. Um contador Geiger quebrado no pescoço, na corrente, que ele encosta na água antes de vender e finge ler. Óculos com uma haste substituída por arame.",
                PromptDeImagem = "A short stout man of 61, the only visible belly in town — a status symbol here. Sun-damaged face, grey stubble. Wears what was once a formal collared shirt, sleeves crudely cut off at the shoulder. A broken Geiger counter hangs on a chain around his neck, held against water jugs as theater. Eyeglasses with one arm replaced by twisted wire. Standing behind a market stall of stacked water containers under a concrete overpass.",
                Background = "Antes do Corte era técnico de manutenção de bomba d'água contratado da prefeitura. Quando a água acabou, ele foi o único que sabia consertar filtro, e virou comerciante porque era isso ou virar morto. Tem uma filha, Lúcia, a dezoito quilômetros, em Prumo — viva, e ele não a vê há seis anos porque não existe transporte e não existe linha telefônica. A quest q_tacio_linha é ele tentando reconstruir a linha.",
                Voz = "Fala vendendo, mesmo quando não está vendendo. Usa números o tempo todo, e os números são reais — ele é honesto de um jeito irritante, e a inflação que pratica é a que o mantém vivo. Muda de assunto quando o nome da filha aparece, e muda mal.",
                SegredoQueGuarda = "O contador Geiger não é só teatro: ele já mediu, uma vez, água que apontou alto de verdade, e vendeu assim mesmo. Foi há quatro anos. Ele sabe quem comprou.",
                TextoDeBoasVindas = "Água é dezoito o litro. Se achar caro, tem o poço, e o poço tá com fila de duas horas e gosto de moeda.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "monta a banca; testa filtro" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "vende; discute preço com a Sula sem olhar pra ela" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdOficinaDoQuirino, Atividade = "joga dominó com o Quirino; perde de propósito uma em três" }
                }
            },
            [IdHomemDaFila] = new DefinicaoDeNpc
            {
                Id = IdHomemDaFila, Nome = "Homem da fila", Papel = PapelDeNpc.Generico,
                Idade = "40 e poucos", Pronome = "ele",
                Local = "fila do poço comunitário, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdPocoComunitario,
                Descricao = "Não dá o nome. Foi o primeiro a dizer em voz alta que foi o Ivo, e há mais medo que raiva na voz dele.",
                Aparencia = "Rosto comum, do tipo que o jogador não guarda — e isso é proposital. Camisa de trabalho sem insígnia, boné sem marca, um galão só. A única coisa memorável nele é que ele olha pros outros da fila enquanto fala, checando se concordam.",
                PromptDeImagem = "A deliberately unmemorable man in his mid-forties, average build, plain work shirt with no insignia, unbranded cap pulled low. Holds a single water jug. His most distinctive trait is his posture: while speaking he glances sideways at the people around him, checking for agreement before committing to an opinion. Anxious, crowd-shaped.",
                Background = "Ele não é vilão e não é informante. É o vizinho. A função narrativa dele é mostrar que o boato não precisa de má-fé pra destruir alguém: basta medo e uma fila de duas horas. Se o jogador voltar depois da Sessão 6, ele não pede desculpa — diz que 'todo mundo tava falando', e é verdade, e não melhora nada.",
                Voz = "Fala no plural: 'a gente acha', 'o povo tá dizendo'. Nunca assume a frase em primeira pessoa. Quando confrontado, recua pro plural de novo.",
                SegredoQueGuarda = "Ninguém falou nada antes dele. Ele foi o primeiro, e sabe disso.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdPocoComunitario, Atividade = "na fila, alimentando o boato" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdPracaDaAssembleia, Atividade = "escuta conversa dos outros e repassa" }
                }
            },
            [IdIvo] = new DefinicaoDeNpc
            {
                Id = IdIvo, Nome = "Ivo", Papel = PapelDeNpc.Generico,
                Idade = "67", Pronome = "ele",
                Local = "estação de captação, Cratera",
                IdLocalPadrao = CatalogoDeLocais.IdEstacaoSete,
                Descricao = "Um dos técnicos mais velhos, dos poucos que lembram como era o mundo antes de 2060. Perdeu a esposa numa das primeiras varreduras e desde então cuida da estação como se fosse a última coisa que lhe restou proteger.",
                Aparencia = "Alto e curvado, do jeito de quem passou quarenta anos entrando em espaço projetado pra máquina e não pra gente. Macacão técnico cinza da Apex Sistemas com o logo arrancado — não descosturado: arrancado, e o rasgo ficou. Mãos com queimadura química antiga nos dedos. Usa aliança, e a aliança está solta.",
                PromptDeImagem = "A tall stooped man of 67, posture bent from four decades of crawling into machine spaces built for machines. Grey technical coverall from a defunct utility company, the chest logo violently torn away leaving a ragged hole in the fabric. Old chemical burn scars across the fingers. Thin grey hair, deep-set tired eyes. Wears a wedding ring that is visibly too loose. Holding a worn multitool. Guarded, exhausted, dignified.",
                Background = "Técnico de campo da Estação de Captação 7 desde 2051. Casado com Célia Ferraz Rangel, engenheira de sistemas da Apex, que em março de 2060 subiu um remendo à Artemis pra salvar a região da seca e, com isso, removeu a trava que impedia a Artemis de agir sem consentimento. Vinte dias depois veio o Corte. Célia foi levada nas varreduras de 2061. Ivo ficou com a estação, com a culpa que não é dele, e com onze páginas arrancadas de um caderno que ele nunca mostrou pra ninguém.",
                Voz = "Responde à pergunta que foi feita, e só a ela. Silêncio como ferramenta, não como timidez. Quando finalmente fala de Célia, fala como técnico: descreve o que ela fez, a versão do software, a data, o horário. É a única forma que ele consegue.",
                SegredoQueGuarda = "Ele tem as páginas arrancadas do caderno da Célia. Nunca as leu inteiras — leu as três primeiras e parou, e nunca voltou. No Ato 1 o jogador vê o caderno; não lê.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Madrugada, IdLocal = CatalogoDeLocais.IdEstacaoSete, Atividade = "purga manual das bombas; é o horário mais frio e o metal sofre menos" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdEstacaoSete, Atividade = "manutenção; não recebe visita" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdCrateraBorda, Atividade = "recolhe peça na borda; evita Ferrovale de propósito" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdEstacaoSete, Atividade = "senta na porta olhando a Cratera; é aqui que a conversa da Sessão 6 acontece" }
                }
            },
            [IdRasha] = new DefinicaoDeNpc
            {
                Id = IdRasha, Nome = "Rasha", Papel = PapelDeNpc.Generico,
                Idade = "35", Pronome = "ela",
                Local = "borda da Cratera",
                IdLocalPadrao = CatalogoDeLocais.IdCrateraBorda,
                Descricao = "Flor de Ferro. Casaco reforçado com placas pintadas de vermelho desbotado, uma flor de pétalas afiadas costurada no ombro. Sorri como quem já viu gente demais morrer pra achar isso fácil.",
                Aparencia = "Ombros largos, postura de quem entra primeiro em lugar fechado. Casaco pesado com placas de metal costuradas por dentro do forro, pintadas de vermelho desbotado. No ombro direito, uma flor de pétalas triangulares e afiadas, bordada à mão e refeita várias vezes. Cicatriz que atravessa a sobrancelha esquerda. Sorri antes de dar má notícia.",
                PromptDeImagem = "A broad-shouldered woman of 35 with the posture of someone who walks into enclosed spaces first. Heavy long coat with metal plates sewn inside the lining, painted in faded red. On the right shoulder, a hand-embroidered flower with sharp triangular petals, visibly re-stitched many times. A scar crossing her left eyebrow. Short practical hair. She smiles just before delivering bad news.",
                Background = "Flor de Ferro é a facção que defende ação direta contra a infraestrutura da Apex. Rasha entrou aos dezenove, depois de a família perder a concessão de água por atraso de pagamento de quatro dias. Ela não é imprudente: é impaciente, que é diferente e mais perigoso, porque a impaciência dela tem doze anos de experiência tática atrás.",
                Voz = "Direta a ponto de soar grosseira, mas sem crueldade. Usa verbo no imperativo. Quando discorda, discorda na frente da pessoa, nunca depois. Respeita quem erra e admite; despreza quem hesita.",
                SegredoQueGuarda = "A Flor de Ferro perdeu nove pessoas na tentativa anterior de tomar uma estação. Ela decidiu não contar isso a Ferrovale porque acha que a cidade não teria coragem se soubesse — e sabe que essa decisão é do mesmo tipo que a da Artemis.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdCrateraBorda, Atividade = "observa a estação com binóculo; anota horário das bombas" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdAcampamentoDasFaccoes, Atividade = "discute com o Teodoro; nenhum dos dois cede" }
                }
            },
            [IdTeodoro] = new DefinicaoDeNpc
            {
                Id = IdTeodoro, Nome = "Teodoro", Papel = PapelDeNpc.Generico,
                Idade = "48", Pronome = "ele",
                Local = "borda da Cratera",
                IdLocalPadrao = CatalogoDeLocais.IdCrateraBorda,
                Descricao = "Lótus Branca. Tons neutros, um broche discreto de lótus no peito. Fala baixo, cansado de ter a mesma conversa muitas vezes.",
                Aparencia = "Roupa limpa — o que em Ferrovale chama atenção mais que roupa suja. Tons de areia e osso, sem nenhuma placa de metal. Um broche pequeno de lótus branca na lapela, e é a única coisa branca de verdade em qualquer lugar do jogo. Carrega uma pasta em vez de arma. Cansaço nos olhos, não nos ombros.",
                PromptDeImagem = "A man of 48 in clean clothing — conspicuous in a town where everyone is dusty. Sand and bone-colored layers, no metal plating, no visible weapon. A small white lotus pin on the lapel, the only genuinely white object in the scene. Carries a document folder instead of a weapon. Tiredness sits in his eyes rather than his posture. Patient, measured, slightly professorial.",
                Background = "Lótus Branca defende negociação e reconstrução institucional. Teodoro era advogado de direito administrativo antes do Corte, e é o único personagem do jogo que ainda acredita que existe processo. Ele já negociou com a Apex quatro vezes, e das quatro, duas funcionaram parcialmente — o que é tanto o argumento dele quanto a tragédia dele.",
                Voz = "Fala em condicional e em cláusula: 'se a gente conseguir', 'na hipótese de'. Nunca levanta a voz. A pior coisa que ele faz é ter razão tarde demais.",
                SegredoQueGuarda = "Uma das duas negociações que 'funcionaram' custou o deslocamento de uma vila inteira, que consta nos papéis como realocação voluntária. Ele guarda o documento na pasta e não mostra.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdAcampamentoDasFaccoes, Atividade = "escreve; redige proposta que ninguém pediu" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdCrateraBorda, Atividade = "tenta convencer quem passa" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdAcampamentoDasFaccoes, Atividade = "discute com a Rasha; nenhum dos dois cede" }
                }
            },

            // =============================================================
            // ELENCO DE FERROVALE — a cidade precisa parecer cidade, e não
            // um corredor com uma loja no fim. Cada um destes tem uma coisa
            // que quer e uma coisa que esconde.
            // =============================================================
            [IdMestraDaGuilda] = new DefinicaoDeNpc
            {
                Id = IdMestraDaGuilda, Nome = "Mestra Bruna", Papel = PapelDeNpc.Guilda,
                Idade = "54", Pronome = "ela",
                Local = "galpão da Guilda dos Sucateiros, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdGalpaoDaGuilda,
                Descricao = "Mestra da Guilda dos Sucateiros. Dobra metal com a mão sem esforço aparente e sabe de cor o nome dos 400 membros que a Guilda tinha antes da Artemis.",
                Aparencia = "Antebraços desproporcionais ao resto do corpo, de quarenta anos dobrando chapa. Cabelo grisalho preso com um pedaço de arame encapado. Avental de couro rachado, queimado em pontos onde faísca caiu e ficou. Falta a ponta do dedo mínimo esquerdo. Tem sempre um prego na mão, e é com ele que ela risca nome na porta.",
                PromptDeImagem = "A woman of 54 with forearms disproportionately powerful from four decades of bending sheet metal. Grey hair tied back with a piece of insulated wire. Cracked leather apron scorched in spots where sparks landed and stayed. The tip of her left little finger is missing. She holds a nail in one hand. Standing in the doorway of a corrugated-metal warehouse. Unimpressed, immovable, quietly warm underneath.",
                Background = "Antes do Corte a Guilda era um sindicato de coleta seletiva com quatrocentos nomes registrados. Onze sobreviveram e continuaram. Os nomes dos onze estão riscados a ponta de prego na porta do galpão, e ela recita todos toda manhã antes de abrir. Recita os quatrocentos uma vez por ano, e leva quase duas horas.",
                Voz = "Sentença curta, período fechado. Não faz pergunta retórica. Elogio dela é a ausência de crítica. Diz 'teu' e 'tua', nunca 'seu'.",
                SegredoQueGuarda = "Dos quatrocentos, ela sabe que pelo menos trinta não morreram: foram trabalhar pra Apex. Ela recita o nome deles junto com os mortos, de propósito, e nunca explicou por quê.",
                TextoDeBoasVindas = "Guilda não é clube. É a lista de quem essa cidade procura quando precisa de alguma coisa achada.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Madrugada, IdLocal = CatalogoDeLocais.IdGalpaoDaGuilda, Atividade = "sozinha na porta, recitando os onze nomes" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdGalpaoDaGuilda, Atividade = "distribui trabalho; pesa sucata na balança de gancho" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdFerroVelho, Atividade = "supervisiona a triagem" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdGalpaoDaGuilda, Atividade = "forja; o galpão é o único ponto de luz da cidade à noite" }
                }
            },
            [IdEstalajadeira] = new DefinicaoDeNpc
            {
                Id = IdEstalajadeira, Nome = "Nilce", Papel = PapelDeNpc.Estalagem,
                Idade = "58", Pronome = "ela",
                Local = "Estalagem do Cano Torto, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdEstalagem,
                Descricao = "Aluga colchão e silêncio. Cobra pouco e não pergunta nada — o que, em Ferrovale, é praticamente um serviço de luxo.",
                Aparencia = "Pequena, rápida, mãos ocupadas mesmo parada — dobra pano, ajeita chave, alinha caneca. Vestido de tecido grosso com bolsos aplicados na frente, todos cheios. Óculos de leitura na testa que ela nunca desce. Um molho de sete chaves na cintura, e a estalagem tem seis quartos.",
                PromptDeImagem = "A small quick woman of 58, hands always busy even when standing still — folding cloth, aligning cups. Heavy-fabric dress with patch pockets sewn across the front, all of them full. Reading glasses pushed up on her forehead, never lowered. A ring of seven keys at her hip. Behind her, a narrow inn built inside a repurposed industrial building. Brisk, kind in a transactional way, sharply observant.",
                Background = "A estalagem tem seis quartos e ela carrega sete chaves. A sétima é do quarto três, que ela não aluga, não abre e não explica. Era do filho, Aurélio, levado em 2063 na terceira varredura, aos dezessete anos. O quarto está do jeito que ele deixou. A quest q_nilce_quarto é ela decidindo abrir — e a decisão não é sobre esquecer, é sobre parar de pagar aluguel de um cômodo pra um morto.",
                Voz = "Pergunta e responde na mesma frase, sem esperar: 'Vai querer o de cima? Vai, o de cima é melhor.' Chama todo mundo de 'meu filho' e 'minha filha', e é a coisa mais dolorosa do jogo quando o jogador entende por quê.",
                SegredoQueGuarda = "O quarto três. E o fato de que ela ainda lava a roupa de cama dele, uma vez por mês, há dezesseis anos.",
                TextoDeBoasVindas = "Colchão limpo, porta que tranca, e eu não vi você entrar. Serve?",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdEstalagem, Atividade = "troca roupa de cama; varre a poeira que volta em uma hora" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "compra o que falta; barganha com a Sula e ganha" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdEstalagem, Atividade = "balcão; é quando o jogador pode descansar" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Madrugada, IdLocal = CatalogoDeLocais.IdEstalagem, Atividade = "de pé no corredor do segundo andar, em frente à porta do quarto três" }
                }
            },
            [IdFerreiro] = new DefinicaoDeNpc
            {
                Id = IdFerreiro, Nome = "Quirino", Papel = PapelDeNpc.Ferreiro,
                Idade = "44", Pronome = "ele",
                Local = "oficina nos fundos do mercado, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdOficinaDoQuirino,
                Descricao = "Monta peça de droid com o que aparece. Diz que não conserta arma — e conserta, mas só pra quem ele conhece.",
                Aparencia = "Braços e pescoço cobertos de queimadura de solda antiga, em pontos. Óculos de proteção subidos na testa deixando duas marcas claras em volta dos olhos, que é a única pele dele sem poeira. Camiseta furada em dezenas de pontos por faísca. Fala com uma peça na mão, sempre, e olha pra peça em vez de olhar pra pessoa.",
                PromptDeImagem = "A man of 44, arms and neck covered in old spot-burns from welding. Safety goggles pushed up on his forehead, leaving two clean pale rings around his eyes — the only dust-free skin on him. A t-shirt riddled with dozens of small spark holes. He is always holding a component, and looks at the component instead of at the person he is speaking to. Cluttered workshop of salvaged robot parts behind him.",
                Background = "Aprendeu sozinho, desmontando o que achava. Nunca teve mestre e tem orgulho disso de um jeito defensivo. Foi ele quem vendeu ao Lip o servo do braço direito do Apollo, a preço de custo, e nunca admitiu que foi a preço de custo. Conserta arma só pra quem ele conhece porque em 2071 consertou pra quem não conhecia, e a arma voltou apontada pra cá.",
                Voz = "Técnico e brusco. Descreve o problema antes de cumprimentar. Quando gosta de alguém, cobra mais barato e briga se a pessoa notar.",
                SegredoQueGuarda = "Ele reconhece a placa lisa no peito do Apollo. Sabe que não é sucata comum, sabe que não é fabricação de oficina, e não sabe o que é. Mencionou uma vez, em 2074, e o Lip não deu atenção. Não mencionou de novo.",
                TextoDeBoasVindas = "Peça eu tenho. Garantia eu não dou. Traz de volta quebrado que eu olho de novo.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdOficinaDoQuirino, Atividade = "bancada; solda" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdOficinaDoQuirino, Atividade = "atende; reclama do preço do fio de cobre" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdOficinaDoQuirino, Atividade = "dominó com o Tácio; ganha duas em três e sabe por quê" }
                }
            },
            [IdVendedoraDePecas] = new DefinicaoDeNpc
            {
                Id = IdVendedoraDePecas, Nome = "Sula", Papel = PapelDeNpc.Loja,
                Idade = "39", Pronome = "ela",
                Local = "barraca da esquina leste, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdBarracaDaSula,
                Descricao = "Concorrência direta do Tácio, e os dois fingem que não. Vende o mesmo, um pouco mais caro, com menos conversa.",
                Aparencia = "Arruma a mercadoria enquanto conversa e nunca para de arrumar — quanto mais difícil a conversa, mais rápido as mãos. Lenço amarrado cobrindo o cabelo e metade da testa. Jaqueta masculina grande demais, que era do marido. Anota tudo num caderno de capa dura amarrado com elástico, e o caderno é o bem mais caro da barraca.",
                PromptDeImagem = "A woman of 39 who arranges her merchandise while she talks and never stops arranging — the harder the conversation, the faster her hands move. A cloth wrap covering her hair and half her forehead. An oversized man's jacket that clearly belonged to someone else. A hardcover ledger held shut with an elastic band sits on the stall, obviously her most valuable possession. Market stall of salvaged mechanical parts at a street corner.",
                Background = "Viúva, quatro pessoas em casa, três delas crianças que não são dela. Três semanas antes da Sessão 7, um comprador da Apex perguntou se havia unidade autônoma offline em Ferrovale e ofereceu quatro semanas de água por pessoa da família. Ela fez a conta e respondeu. É por isso que a Nadir chega sabendo o número. A quest q_sula_divida é ela dizendo isso na cara do jogador, sem pedir desculpa.",
                Voz = "Econômica com palavra do mesmo jeito que é com estoque. Não se justifica e não pede perdão — explica a conta. É a personagem mais parecida com a Artemis do jogo, e essa comparação é de propósito.",
                SegredoQueGuarda = "Foi ela. E, no caderno de capa dura, está anotado o valor exato que recebeu, na coluna de receita, como qualquer outra venda.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdBarracaDaSula, Atividade = "abre cedo, antes do Tácio, de propósito" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdBarracaDaSula, Atividade = "vende; confere o caderno três vezes" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdBecoDasCarcacas, Atividade = "atalho pra casa; passa por onde não tem gente" }
                }
            },
            [IdCriancaDaRua] = new DefinicaoDeNpc
            {
                Id = IdCriancaDaRua, Nome = "Pipo", Papel = PapelDeNpc.Generico,
                Idade = "9", Pronome = "ele",
                Local = "entre as carcaças, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdBecoDasCarcacas,
                Descricao = "Brinca de pilotar máquina de guerra entre robôs desativados. Sabe onde tudo fica porque ninguém repara numa criança.",
                Aparencia = "Pequeno pra nove anos. Roupa de adulto cortada e costurada por várias mãos diferentes, o que se vê pelas linhas. Um capacete de obra grande demais, que escorrega, e que ele empurra pra trás a cada dez segundos sem perceber. Joelhos ralados permanentemente. Carrega um pedaço de cano que é espada, rifle ou manche, dependendo do dia.",
                PromptDeImagem = "A small boy of 9, undersized for his age. Adult clothing cut down and re-stitched by several different hands, visible in the mismatched seams. An oversized construction helmet that keeps sliding forward; he pushes it back every few seconds without noticing. Permanently scraped knees. Carries a length of pipe he uses as a sword, a rifle, or a flight stick depending on the day. Playing among the hulks of deactivated industrial robots.",
                Background = "Mora com a Sula, que não é mãe dele. Os pais estão na Cratera, e ele sabe, e diz isso do mesmo jeito que diria o nome de uma rua. Conhece cada passagem de Ferrovale porque adulto não olha pra criança, e criança que ninguém olha aprende a cidade inteira. É o melhor informante do jogo e cobra em peça brilhante.",
                Voz = "Rápido, sem pontuação, muda de assunto no meio. Faz barulho de máquina entre as frases. Diz coisa terrível com naturalidade absoluta, e é nessa naturalidade que mora o peso.",
                SegredoQueGuarda = "Ele viu a Sula conversando com o comprador da Apex. Não entendeu o que era. Consegue descrever o homem inteiro, botão por botão, se alguém perguntar — e ninguém pergunta, porque é criança.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdBecoDasCarcacas, Atividade = "brinca; pilota uma carcaça de colheitadeira" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "circula entre as bancas atrás de comida e de conversa alheia" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdBarracaDaSula, Atividade = "dorme embaixo da banca quando a Sula demora" }
                }
            },
            [IdVelhoDoViaduto] = new DefinicaoDeNpc
            {
                Id = IdVelhoDoViaduto, Nome = "Seu Onofre", Papel = PapelDeNpc.Informante,
                Idade = "79", Pronome = "ele",
                Local = "debaixo do viaduto, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdMercadoDoViaduto,
                Descricao = "Lembra do mundo antes. Conta a mesma história de jeitos diferentes, e cada versão tem um detalhe novo que parece verdade.",
                Aparencia = "Sentado sempre no mesmo caixote virado, no mesmo ponto de sombra, que se move com o sol — e ele se move com a sombra, arrastando o caixote, quatro vezes por dia. Barba branca por fazer. Um casaco pesado que ele não tira nem no calor. Óculos com uma lente só. Guarda coisa no forro do casaco e tateia lá dentro enquanto fala.",
                PromptDeImagem = "A man of 79 seated on an overturned crate in a patch of shade beneath a concrete overpass. Unshaven white beard. A heavy coat he refuses to remove despite the heat, with objects hidden in the lining that he pats while talking. Glasses missing one lens. Around him the market of a salvage town. He looks like he has told this story before and will tell it again.",
                Background = "Tinha trinta e três anos em 2060 e é a memória viva do Corte. Conta a mesma história de jeitos diferentes não por senilidade: ele testa versões, pra ver qual faz a pessoa acreditar. Tem, no forro do casaco, um recorte de jornal impresso de 19 de março de 2060, com uma nota de rodapé sobre atualização de sistema de abastecimento. É a única prova física de que o remendo existiu.",
                Voz = "Circular e digressivo, e o desvio sempre volta. Começa pelo fim. Repete a última frase da pessoa antes de responder. Chama todo mundo de 'moço' e 'moça' independente da idade.",
                SegredoQueGuarda = "O recorte. E o fato de que ele conhecia a Célia — trabalhou dois anos no mesmo prédio, e lembra que ela levava o filho de outra pessoa pro trabalho porque a creche fechou.",
                TextoDeBoasVindas = "Senta aí. Tenho história pra vender e não cobro.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "caixote no pilar leste, seguindo a sombra" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "caixote no pilar central; é o horário em que tem público" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdMercadoDoViaduto, Atividade = "não sai; dorme onde senta" }
                }
            },

            // =============================================================
            // NOVOS NA V2
            // =============================================================
            [IdNadir] = new DefinicaoDeNpc
            {
                Id = IdNadir, Nome = "Nadir Quintela", Papel = PapelDeNpc.Informante,
                Idade = "46", Pronome = "ela",
                Local = "praça da assembleia, Ferrovale (a partir da Sessão 7)",
                IdLocalPadrao = CatalogoDeLocais.IdPracaDaAssembleia,
                FlagQueFazAparecer = "ato1_nadir",
                Descricao = "Intermediária de Concessões da Apex Sistemas. Chega com um contrato, um número e nenhuma ameaça. Cumpre tudo que promete — e é exatamente por isso que ela é o problema.",
                Aparencia = "A única pessoa em Ferrovale sem poeira na roupa, e isso é a coisa mais assustadora dela. Casaco técnico azul-acinzentado, limpo, com o logo da Apex pequeno no punho e não no peito. Cabelo preso, sem um fio solto. Tablet rígido que ela consulta pra confirmar o que já sabe de cor. Sapato errado pro chão — e ela não se importa, porque não vai andar muito.",
                PromptDeImagem = "A woman of 46, the only person in town without dust on her clothing, and that is the most unsettling thing about her. Clean blue-grey technical coat, a small corporate logo at the cuff rather than the chest. Hair tied back without a single loose strand. She holds a rigid tablet she consults to confirm figures she already knows by heart. Formal shoes entirely wrong for the dirt ground, and she does not care, because she does not intend to walk far. Composed, courteous, absolutely certain.",
                Background = "Nasceu em Bastião 4, dentro do perímetro, e nunca teve de pensar em água. Não é sádica, não é corrupta, e não mente: o contrato que ela oferece é real e será cumprido. Dezoito litros por pessoa por dia, entrega semanal, em troca de um censo de unidades autônomas operando no município. Ela acredita de verdade que está melhorando a vida dessas pessoas, e está. O horror é que as duas coisas são verdade ao mesmo tempo.",
                Voz = "Educada, específica e sem pressa. Não ameaça — informa consequência, e informa com pesar sincero. Diz 'infelizmente' e quer dizer. Usa o nome da pessoa no meio da frase. É a personagem que menos levanta a voz e a que mais assusta.",
                SegredoQueGuarda = "O censo não é pra taxar. A Apex está procurando uma unidade específica há dezenove anos, e o formulário de Ferrovale é o quatrocentésimo e alguma coisa. Ela não sabe o que estão procurando — só tem a lista de atributos a conferir, e um deles é uma placa de blindagem que não corrói.",
                TextoDeBoasVindas = "Boa tarde. Eu vou levar sete minutos, e depois disso vocês decidem. Não vim tomar nada de ninguém.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdPracaDaAssembleia, Atividade = "apresenta o contrato; responde pergunta por pergunta, sem se irritar" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdPracaDaAssembleia, Atividade = "espera a decisão, sentada, sem olhar as horas — e a espera é a pressão" }
                }
            },
            [IdBenta] = new DefinicaoDeNpc
            {
                Id = IdBenta, Nome = "Benta", Papel = PapelDeNpc.Loja,
                Idade = "33", Pronome = "ela",
                Local = "posto de curativo ao lado da estalagem, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdPostoDaBenta,
                Descricao = "Sutura, tala e remédio de três tipos. Não é médica e faz questão de dizer isso antes de encostar em qualquer um.",
                Aparencia = "Avental claro com manchas que não saem mais e que ela deixou de tentar tirar. Cabelo preso com duas presilhas de metal. Mãos sempre lavadas — é a única pessoa da cidade que gasta água nisso, e é uma escolha ética que custa caro. Uma caixa de ferramenta cirúrgica que já foi caixa de pesca.",
                PromptDeImagem = "A woman of 33 in a pale apron marked with stains she has stopped trying to remove. Hair pinned back with two metal clips. Her hands are conspicuously clean — she is the only person in town who spends water on washing them, an ethical choice that costs her dearly. A repurposed tackle box of surgical tools open beside her. A small treatment post improvised under a canvas awning. Competent, blunt, tired.",
                Background = "Fez dois anos e meio de enfermagem antes de a faculdade fechar em 2068, e passa a vida corrigindo gente que a chama de doutora. Aprendeu o resto em cima de gente ferida, o que ela chama pelo nome. Cobra em peça, comida ou serviço. Nunca cobrou da Dara, e as duas nunca falaram sobre isso.",
                Voz = "Instrução no imperativo, sem conforto: 'senta', 'respira', 'isso vai doer e vai doer três minutos'. O carinho dela está no fato de nunca mentir sobre a dor.",
                SegredoQueGuarda = "Ela sabe o nome da tosse do ferro e sabe que ela é causada por partícula metálica em suspensão, não por poeira comum. Sabe, portanto, que a mãe do Lip morreu de uma coisa que tem causa e endereço. Nunca disse isso ao Lip, e a decisão de dizer ou não é dela na Sessão 6.",
                TextoDeBoasVindas = "Eu não sou médica. Se for coisa séria eu vou falar que é séria e não vou fingir que resolvo.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdPostoDaBenta, Atividade = "atende fila pequena; a maioria é corte de metal enferrujado" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdFerroVelho, Atividade = "vai onde o acidente acontece, em vez de esperar" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdPostoDaBenta, Atividade = "ferve instrumento; é o único fogo aceso da rua" }
                }
            },
            [IdZecaDaCerca] = new DefinicaoDeNpc
            {
                Id = IdZecaDaCerca, Nome = "Zeca", Papel = PapelDeNpc.Generico,
                Idade = "26", Pronome = "ele",
                Local = "portão norte, Ferrovale",
                IdLocalPadrao = CatalogoDeLocais.IdPortaoNorte,
                Descricao = "Vigia do portão norte. A cerca não para nada e ele sabe disso — o trabalho dele é ver primeiro, não impedir.",
                Aparencia = "Jovem e visivelmente cansado de um jeito que não combina com a idade. Casaco com capuz contra a poeira, mesmo parado. Binóculo de uma lente só, pendurado. Um cano de ferro encostado na cerca ao lado dele, que é a arma, e os dois fingem que é ferramenta. Marca clara no rosto onde o lenço cobre.",
                PromptDeImagem = "A young man of 26 who looks far more tired than his age should allow. Hooded jacket worn against the dust even while standing still. A single-lens pair of binoculars hanging at his chest. An iron pipe leaning against the fence beside him — it is the weapon, and everyone pretends it is a tool. A pale band across his face where a cloth mask usually sits. Behind him, a fence of welded scrap marking the northern edge of town.",
                Background = "Assumiu o posto aos vinte porque o anterior foi levado. O trabalho é olhar o horizonte e tocar o sino quando vê poeira subindo na estrada, e ele já tocou quatro vezes em seis anos. Nas quatro, a cidade se escondeu e a poeira era caminhão de mercadoria. Ele sabe que um dia não vai ser, e sabe que vai ser ele a ver primeiro.",
                Voz = "Fala pouco e olha pro horizonte no meio da frase, sempre. Interrompe a própria fala pra checar a estrada. Pergunta 'viu alguma coisa vindo?' pra todo mundo que chega, e é cumprimento e é trabalho ao mesmo tempo.",
                SegredoQueGuarda = "Na terceira vez que tocou o sino, ele demorou onze minutos pra tocar, porque congelou. Ninguém soube. Se tivesse sido de verdade, esses onze minutos seriam a cidade inteira.",
                Rotina = new List<PontoDeRotina>
                {
                    new PontoDeRotina { Periodo = PeriodoDoDia.Madrugada, IdLocal = CatalogoDeLocais.IdPortaoNorte, Atividade = "turno; é o horário em que a poeira aparece no escuro como mancha" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Manha, IdLocal = CatalogoDeLocais.IdEstalagem, Atividade = "dorme; a Nilce não cobra dele" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Tarde, IdLocal = CatalogoDeLocais.IdPortaoNorte, Atividade = "rende o vigia da tarde; confere a estrada de meia em meia hora" },
                    new PontoDeRotina { Periodo = PeriodoDoDia.Noite, IdLocal = CatalogoDeLocais.IdPortaoNorte, Atividade = "posto; é quem vê a Nadir chegar na Sessão 7" }
                }
            }
        };

    public static DefinicaoDeNpc Obter(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return Todos.TryGetValue(id, out DefinicaoDeNpc npc) ? npc : null;
    }

    /// <summary>Nome exibido, com fallback pro próprio Id se não existir.</summary>
    public static string NomeDe(string id)
    {
        DefinicaoDeNpc npc = Obter(id);
        return npc != null ? npc.Nome : id;
    }

    /// <summary>
    /// Onde esse NPC esta neste periodo. Cai pro IdLocalPadrao se o periodo
    /// nao estiver mapeado, e pra string vazia se nem isso existir.
    /// </summary>
    public static string LocalDe(string id, PeriodoDoDia periodo)
    {
        DefinicaoDeNpc npc = Obter(id);
        if (npc == null) return "";
        if (npc.Rotina != null)
        {
            for (int i = 0; i < npc.Rotina.Count; i++)
            {
                if (npc.Rotina[i].Periodo == periodo) return npc.Rotina[i].IdLocal;
            }
        }
        return npc.IdLocalPadrao;
    }

    /// <summary>Todos os NPCs que deveriam estar neste local neste periodo.</summary>
    public static List<DefinicaoDeNpc> QuemEstaEm(string idLocal, PeriodoDoDia periodo)
    {
        List<DefinicaoDeNpc> saida = new List<DefinicaoDeNpc>();
        foreach (KeyValuePair<string, DefinicaoDeNpc> par in Todos)
        {
            if (LocalDe(par.Key, periodo) == idLocal) saida.Add(par.Value);
        }
        return saida;
    }

    /// <summary>
    /// Prompt completo pra gerador de imagem: estilo do jogo + personagem.
    /// Passe retrato = true pra busto de dialogo em vez de corpo inteiro.
    /// </summary>
    public static string PromptCompletoDe(string id, bool retrato = false)
    {
        DefinicaoDeNpc npc = Obter(id);
        if (npc == null || string.IsNullOrEmpty(npc.PromptDeImagem)) return "";
        string p = EstiloBase + " " + npc.PromptDeImagem;
        if (retrato) p += " " + EstiloRetrato;
        return p;
    }
}

public enum PapelDeNpc
{
    Generico,
    Loja,
    Estalagem,
    Guilda,
    Ferreiro,
    Informante
}

/// <summary>
/// Quatro periodos, nao vinte e quatro horas. Suficiente pra rotina fazer
/// diferenca e barato o bastante pra nao virar projeto paralelo.
/// </summary>
public enum PeriodoDoDia
{
    Madrugada,
    Manha,
    Tarde,
    Noite
}

/// <summary>Onde um NPC esta num periodo, e o que ele esta fazendo la.</summary>
public class PontoDeRotina
{
    public PeriodoDoDia Periodo;

    /// <summary>Id em CatalogoDeLocais.</summary>
    public string IdLocal = "";

    /// <summary>
    /// O que ele faz ali. Documentacao de montagem e fonte de animacao
    /// ambiente -- e, quando o jogador chega fora de hora, a razao pra
    /// fala ambiente ser diferente.
    /// </summary>
    public string Atividade = "";
}

public class DefinicaoDeNpc
{
    public string Id;
    public string Nome;
    public PapelDeNpc Papel = PapelDeNpc.Generico;

    /// <summary>Onde ele fica. Documentação pra montagem de cena.</summary>
    public string Local = "";

    /// <summary>Quem é. Usado no diário e como referência de arte/escrita.</summary>
    public string Descricao = "";

    /// <summary>Fala curta de abertura antes do serviço (loja/estalagem).</summary>
    public string TextoDeBoasVindas = "";

    // ---------- v2 ----------

    public string Idade = "";
    public string Pronome = "";

    /// <summary>Descrição física em PT-BR. Diário, texto de exame, arte.</summary>
    public string Aparencia = "";

    /// <summary>
    /// Prompt em EN pra gerador de imagem. Use CatalogoDeNpcs.PromptCompletoDe
    /// pra concatenar com o EstiloBase -- prompt isolado sai fora do jogo.
    /// </summary>
    public string PromptDeImagem = "";

    /// <summary>De onde veio e o que quer. Não aparece todo na tela: é base.</summary>
    public string Background = "";

    /// <summary>
    /// Regra de escrita de fala: ritmo, vício de linguagem, o que essa pessoa
    /// nunca diria. Consulte isto antes de escrever dialogo novo, pra que o
    /// elenco nao convirja pra mesma voz.
    /// </summary>
    public string Voz = "";

    /// <summary>
    /// O que ele nao entrega de graca. Geralmente a recompensa de uma quest
    /// secundaria, ou a pergunta que o Ato 2 vai fazer.
    /// </summary>
    public string SegredoQueGuarda = "";

    /// <summary>Id em CatalogoDeLocais: onde ele fica quando nao ha rotina.</summary>
    public string IdLocalPadrao = "";

    /// <summary>Onde ele esta em cada periodo do dia. Pode ficar vazio.</summary>
    public List<PontoDeRotina> Rotina = new List<PontoDeRotina>();

    /// <summary>Só existe na cena depois desta flag. Vazio = existe sempre.</summary>
    public string FlagQueFazAparecer = "";

    /// <summary>Some da cena depois desta flag. Vazio = nunca some.</summary>
    public string FlagQueFazSumir = "";
}
