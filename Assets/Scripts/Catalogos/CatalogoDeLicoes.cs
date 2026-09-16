using System.Collections.Generic;

/// <summary>
/// Licoes de programacao na voz do Apollo. Task 2: "indicacoes de melhora da
/// mecanica de codigo Lua, pensando no pedagogico".
///
/// O PROBLEMA QUE ISSO RESOLVE: o terminal so tinha uma lista de comandos
/// (TextoAjudaPadrao). Um aluno de Algoritmos travado em "por que meu while
/// nao para?" nao e ajudado por uma lista de assinaturas de metodo. Ele
/// precisa de explicacao de CONCEITO, com exemplo, no momento em que travou.
///
/// COMO O JOGADOR ACESSA: dentro do proprio terminal, sem sair do jogo:
///     droid.explicar("while")
///     droid.explicar()          -- lista os topicos
/// Isso e um comando Lua de verdade, entao o jogador pratica chamar funcao
/// com argumento so pra pedir ajuda. A ferramenta de aprender e ela mesma
/// um exercicio.
///
/// COMPLEMENTA (nao substitui) o ApolloDica, que age depois do ERRO. Aqui e
/// ativo: o jogador pergunta. La e reativo: o jogo responde ao tropeco.
/// </summary>
public static class CatalogoDeLicoes
{
    public class Licao
    {
        public string Topico;
        public string Titulo;
        public string Explicacao;
        public string Exemplo;
        public string ErroComum;
    }

    // Chave em minusculo, sem acento -- a busca normaliza a entrada do
    // jogador, entao "While", "WHILE" e "while" caem no mesmo lugar.
    public static readonly Dictionary<string, Licao> Todas = new Dictionary<string, Licao>
    {
        ["variavel"] = new Licao
        {
            Topico = "variavel",
            Titulo = "Variável — uma caixa com nome",
            Explicacao =
                "Uma variável guarda um valor e responde por um nome. Quando você escreve\n" +
                "nivel = 0, você não está perguntando nada: está MANDANDO o nivel valer zero.\n" +
                "Escrever de novo por cima troca o valor. O nome é seu, escolhe um que\n" +
                "diga o que é — daqui a uma semana você não lembra o que \"x\" era.",
            Exemplo = "nivel = 0\nnivel = nivel + 10   -- agora vale 10",
            ErroComum =
                "Usar um nome que nunca recebeu valor. Pra mim, 'nivel_agua' e 'nivelAgua'\n" +
                "são duas variáveis completamente diferentes. Maiúscula conta. Traço conta."
        },

        ["while"] = new Licao
        {
            Topico = "while",
            Titulo = "while — repetir enquanto for verdade",
            Explicacao =
                "Uma linha solta roda uma vez. O while roda o bloco DE NOVO E DE NOVO\n" +
                "enquanto a condição continuar verdadeira, e para no instante em que ela\n" +
                "deixa de ser. Ele testa a condição ANTES de cada repetição, então se ela\n" +
                "já começar falsa, o bloco não roda nenhuma vez.",
            Exemplo = "nivel = 0\nwhile nivel < 100 do\n  nivel = nivel + 10\nend",
            ErroComum =
                "Esquecer de mudar a variável dentro do bloco. Se nada muda, a condição\n" +
                "nunca fica falsa e o laço não para nunca — e aí o jogo trava de verdade.\n" +
                "Antes de rodar, pergunta: o que aqui dentro faz a condição virar falsa?"
        },

        ["if"] = new Licao
        {
            Topico = "if",
            Titulo = "if — decidir antes de agir",
            Explicacao =
                "O if executa o bloco só se a condição for verdadeira. É a diferença entre\n" +
                "fazer e fazer no momento certo. Com else, você diz o que fazer quando ela\n" +
                "for falsa. Repara que em Lua vem 'then' depois da condição, e o bloco\n" +
                "todo fecha com 'end'.",
            Exemplo = "if energia >= 50 then\n  porta_aberta = 1\nelse\n  porta_aberta = 0\nend",
            ErroComum =
                "Confundir = com ==. Um sinal de igual ATRIBUI (põe valor). Dois sinais\n" +
                "de igual COMPARAM (perguntam se é igual). Dentro de um if você quase\n" +
                "sempre quer dois."
        },

        ["comparacao"] = new Licao
        {
            Topico = "comparacao",
            Titulo = "Comparações — perguntar ao invés de mandar",
            Explicacao =
                "Uma comparação devolve verdadeiro ou falso, e é isso que o while e o if\n" +
                "consomem.\n" +
                "  ==  igual        ~=  diferente\n" +
                "  <   menor        <=  menor ou igual\n" +
                "  >   maior        >=  maior ou igual\n" +
                "Pra juntar duas condições: and (as duas) e or (qualquer uma).",
            Exemplo = "if vida > 0 and energia >= 10 then\n  pode_atacar = 1\nend",
            ErroComum =
                "Em Lua não se escreve && nem ||. É and e or, em palavra. E 'diferente'\n" +
                "é ~= , não !=."
        },

        ["contador"] = new Licao
        {
            Topico = "contador",
            Titulo = "Contador e acumulador — duas contas no mesmo laço",
            Explicacao =
                "Acumulador é a variável que SOMA quanto (total = total + peso).\n" +
                "Contador é a que soma QUANTAS VEZES (quantas = quantas + 1).\n" +
                "Os dois vivem no mesmo laço sem se atrapalhar, e juntos respondem duas\n" +
                "perguntas de uma vez: quanto deu, e em quantos passos.",
            Exemplo = "total = 0\nquantas = 0\nwhile total <= 70 do\n  total = total + 7\n  quantas = quantas + 1\nend",
            ErroComum =
                "Zerar o contador dentro do laço. Se você escrever quantas = 0 lá dentro,\n" +
                "ele volta pra zero a cada volta e no fim vale 1."
        },

        ["funcao"] = new Licao
        {
            Topico = "funcao",
            Titulo = "Chamar função — pedir alguma coisa e receber de volta",
            Explicacao =
                "Quando você escreve droid.obterAtributo(\"For\"), você está PEDINDO uma\n" +
                "coisa e recebendo um número em troca. Esse número não some — dá pra\n" +
                "guardar numa variável e usar depois. O que vai dentro dos parênteses são\n" +
                "os argumentos: texto entre aspas, número sem aspas.",
            Exemplo = "forca = droid.obterAtributo(\"For\")\nprevisao = forca * 2",
            ErroComum =
                "Chamar e jogar fora o resultado. droid.obterAtributo(\"For\") sozinho numa\n" +
                "linha lê e descarta. Se você quer o valor, guarda ele em algo."
        },

        ["erro"] = new Licao
        {
            Topico = "erro",
            Titulo = "Erro não é castigo, é informação",
            Explicacao =
                "Todo erro que aparece aqui tem uma linha e um motivo. Ler o erro é parte\n" +
                "do trabalho, não sinal de que você falhou — eu programo há sete anos e\n" +
                "leio erro todo dia.\n" +
                "Ordem pra investigar: (1) qual linha; (2) o nome está escrito igual em\n" +
                "todo lugar; (3) todo bloco que abriu fechou com end; (4) o valor é número\n" +
                "onde você esperava número.",
            Exemplo = "-- Execute assim mesmo e leia o que eu falo:\nnivel = nivel + 1",
            ErroComum =
                "Mudar cinco coisas de uma vez depois de um erro. Muda uma, roda, vê.\n" +
                "Senão você conserta e nem sabe o que consertou."
        },

        ["atributo"] = new Licao
        {
            Topico = "atributo",
            Titulo = "Os seis atributos — no que cada ponto vira",
            Explicacao =
                "FOR  — dano. Meu ataque é FOR x 2, mais o nível da técnica.\n" +
                "VIT  — defesa E HP máximo. Cada ponto tira dano recebido e sobe meu teto.\n" +
                "DEX  — pontaria. Chance de acerto é 50 + (meu DEX - esquiva do alvo).\n" +
                "AGI  — esquiva. Mesma conta, do outro lado.\n" +
                "LUK  — crítico. Cada ponto é 1% de chance de dobrar o dano.\n" +
                "INT  — resistir a Stun/Veneno, e curar mais com item.",
            Exemplo = "droid.subirAtributo(\"Dex\", 3)\ndroid.obterAtributo(\"Dex\")",
            ErroComum =
                "Colocar tudo em FOR. Dano alto não serve de nada se você erra metade dos\n" +
                "golpes — eu começo com DEX zero, e isso é 40% de acerto. Sobe DEX cedo."
        },

        ["tecnica"] = new Licao
        {
            Topico = "tecnica",
            Titulo = "Técnicas — o que eu sei fazer",
            Explicacao =
                "Uma técnica é um golpe que você me ensina e que passa a aparecer no menu\n" +
                "Lutar. O nível de dano custa pontos e some no dano final. Você pode\n" +
                "melhorar uma técnica já aprendida pagando só a diferença, ou esquecer e\n" +
                "receber os pontos de volta.",
            Exemplo =
                "droid.aprenderTecnica(\"Impacto\", 2)\n" +
                "droid.melhorarTecnica(\"Impacto\", 1)\n" +
                "droid.listarTecnicas()",
            ErroComum =
                "Tentar aprender de novo um nome que já existe. Isso falha de propósito —\n" +
                "use melhorarTecnica, que cobra só a diferença em vez do custo cheio."
        }
    };

    /// <summary>Lista de tópicos, pra quando o jogador chama sem argumento.</summary>
    public static string ListarTopicos()
    {
        var nomes = new List<string>(Todas.Keys);
        nomes.Sort();
        return "APOLLO: Posso explicar: " + string.Join(", ", nomes) +
               ".\n       Use droid.explicar(\"while\") — com aspas.";
    }

    /// <summary>
    /// Texto completo de um tópico, já formatado pro log do terminal.
    /// Tópico desconhecido devolve a lista, nunca uma resposta vazia.
    /// </summary>
    public static string Explicar(string topico)
    {
        if (string.IsNullOrWhiteSpace(topico)) return ListarTopicos();

        string chave = Normalizar(topico);
        if (!Todas.TryGetValue(chave, out Licao licao))
        {
            return $"APOLLO: Não tenho lição sobre \"{topico}\".\n{ListarTopicos()}";
        }

        return
            $"APOLLO: {licao.Titulo}\n" +
            $"{licao.Explicacao}\n" +
            $"--- exemplo ---\n{licao.Exemplo}\n" +
            $"--- onde todo mundo tropeça ---\n{licao.ErroComum}";
    }

    /// <summary>
    /// Minúsculas e sem acento, pra "condição" e "condicao" caírem no mesmo
    /// lugar. Sem isso, metade das tentativas do jogador brasileiro falha por
    /// um til.
    /// </summary>
    private static string Normalizar(string entrada)
    {
        string s = entrada.Trim().ToLowerInvariant();
        s = s.Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a");
        s = s.Replace("é", "e").Replace("ê", "e");
        s = s.Replace("í", "i");
        s = s.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o");
        s = s.Replace("ú", "u").Replace("ü", "u");
        s = s.Replace("ç", "c");

        // Apelidos: o jogador escreve o que faz sentido pra ele, não o nome
        // interno do tópico.
        switch (s)
        {
            case "laco":
            case "loop":
            case "repetir":
            case "enquanto": return "while";
            case "condicao":
            case "condicional":
            case "se": return "if";
            case "var":
            case "variaveis": return "variavel";
            case "comparar":
            case "operador":
            case "operadores": return "comparacao";
            case "acumulador":
            case "contar": return "contador";
            case "metodo":
            case "funcoes":
            case "chamar": return "funcao";
            case "erros":
            case "bug":
            case "debug": return "erro";
            case "atributos":
            case "stats":
            case "status": return "atributo";
            case "tecnicas":
            case "golpe":
            case "golpes": return "tecnica";
            default: return s;
        }
    }
}
