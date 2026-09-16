using System.Collections.Generic;

/// <summary>
/// Puzzles de logica prontos. Task 8: "lista de puzzles novos que podem ser
/// facilmente criados, com a mesma mecanica ou mecanicas um pouco
/// diferentes, ja prontos em algum script".
///
/// COMO FUNCIONA A MECANICA (a mesma do puzzle da agua, generalizada):
/// o jogador escreve Lua no terminal; o VerificadorDePuzzle le o valor FINAL
/// de uma ou mais variaveis globais via DroidScriptRunner.ObterVariavelNumerica
/// e compara com o esperado. Nao ha parser, nao ha analise de codigo -- o que
/// e avaliado e o RESULTADO, o que e pedagogicamente correto: existe mais de
/// um caminho certo, e todos passam.
///
/// TRES FORMATOS SUPORTADOS, em ordem crescente de exigencia:
///   A) UMA VARIAVEL -- "faca nivel_agua chegar a 100". Ensina laco.
///   B) VARIAS VARIAVEIS -- "some tudo em total E conte em quantos".
///      Ensina que o programa tem estado, nao uma resposta so.
///   C) VARIAVEL-SENTINELA -- "so marque porta_aberta = 1 SE energia >= 50".
///      Ensina condicional: o jogador tem que provar que testou.
///
/// COMO ADICIONAR UM PUZZLE NOVO: copie uma entrada, troque os textos e as
/// verificacoes. Nenhum script de cena muda -- o VerificadorDePuzzle so
/// precisa do Id no Inspector.
/// </summary>
public static class CatalogoDePuzzles
{
    // Ids estaveis -- usados em Inspector e em objetivo de quest.
    public const string IdAguaDaCasa = "p_agua_casa";
    public const string IdPainelDeEnergia = "p_painel_energia";
    public const string IdContadorDeSucata = "p_contador_sucata";
    public const string IdFiltroDeAgua = "p_filtro_agua";
    public const string IdSequenciaDaPorta = "p_sequencia_porta";
    public const string IdCalibragemDoBraco = "p_calibragem_braco";
    public const string IdRotaDePatrulha = "p_rota_patrulha";
    public const string IdBombaDeReserva = "p_bomba_reserva";
    public const string IdTriagemDePecas = "p_triagem_pecas";
    public const string IdAntenaDaGuilda = "p_antena_guilda";

    public static readonly Dictionary<string, DefinicaoDePuzzle> Todos =
        new Dictionary<string, DefinicaoDePuzzle>
        {
            // =============================================================
            // 1. ÁGUA DA CASA — Sessão 1. JÁ IMPLEMENTADO E TESTADO.
            // Formato A. Conceito: variável + laço.
            // =============================================================
            [IdAguaDaCasa] = new DefinicaoDePuzzle
            {
                Id = IdAguaDaCasa,
                Titulo = "O gerador de água",
                Conceito = "Variável e laço (while)",
                Enunciado = "O valor não muda sozinho. Faz ele subir de dez em dez até encher — cem.",
                CodigoInicial = "nivel_agua = 0\n-- seu código aqui\n",
                Dica = "Uma linha que soma roda uma vez. Um while roda até a condição deixar de ser verdade.",
                Solucao = "nivel_agua = 0\nwhile nivel_agua < 100 do\n  nivel_agua = nivel_agua + 10\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "nivel_agua", Comparacao = TipoDeComparacao.MaiorOuIgual, Valor = 100 }
                },
                FalaDeVitoria = "Viu? Não foi tão difícil quanto você fingiu que ia ser.",
                Curiosidade = "Geradores de água de Ferrovale não param quando enchem. Quem programou o original em 2041 confiava que sempre haveria alguém olhando."
            },

            // =============================================================
            // 2. PAINEL DE ENERGIA — Sessão 5. Formato C.
            // Conceito: condicional composta com laço.
            // =============================================================
            [IdPainelDeEnergia] = new DefinicaoDePuzzle
            {
                Id = IdPainelDeEnergia,
                Titulo = "Painel de energia da Cratera",
                Conceito = "Condicional (if) + laço",
                Enunciado = "Carrega a energia até cinquenta e SÓ ENTÃO marca porta_aberta = 1. Se marcar antes, o painel rejeita.",
                CodigoInicial = "energia = 0\nporta_aberta = 0\n-- seu código aqui\n",
                Dica = "Duas coisas: primeiro um while que carrega, depois um if que testa antes de marcar.",
                Solucao = "energia = 0\nporta_aberta = 0\nwhile energia < 50 do\n  energia = energia + 10\nend\nif energia >= 50 then\n  porta_aberta = 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "energia", Comparacao = TipoDeComparacao.MaiorOuIgual, Valor = 50 },
                    new VerificacaoDeVariavel { Variavel = "porta_aberta", Comparacao = TipoDeComparacao.IgualA, Valor = 1 }
                },
                FalaDeVitoria = "Passagem liberada. Você não só carregou — você conferiu antes de agir. É diferente.",
                Curiosidade = "O painel foi instalado pra impedir que a estação fosse ligada com carga parcial. Queimou três bombas antes de alguém pensar nisso."
            },

            // =============================================================
            // 3. CONTADOR DE SUCATA — quest da Guilda. Formato B.
            // Conceito: duas variáveis mudando juntas (soma + contagem).
            // =============================================================
            [IdContadorDeSucata] = new DefinicaoDePuzzle
            {
                Id = IdContadorDeSucata,
                Titulo = "Balança da Guilda",
                Conceito = "Duas variáveis no mesmo laço (acumulador e contador)",
                Enunciado = "Cada peça pesa 7. Empilha até o peso total passar de 70, e me diz em quantas peças deu.",
                CodigoInicial = "total = 0\nquantas = 0\n-- seu código aqui\n",
                Dica = "Dentro do mesmo while, duas linhas: uma soma o peso, outra soma um na contagem.",
                Solucao = "total = 0\nquantas = 0\nwhile total <= 70 do\n  total = total + 7\n  quantas = quantas + 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "total", Comparacao = TipoDeComparacao.MaiorQue, Valor = 70 },
                    new VerificacaoDeVariavel { Variavel = "quantas", Comparacao = TipoDeComparacao.IgualA, Valor = 11 }
                },
                FalaDeVitoria = "Onze. A balança confere. Duas contas ao mesmo tempo, no mesmo laço — isso é programa de verdade.",
                Curiosidade = "A Guilda pesa tudo em múltiplos de sete desde que a única balança que sobrou quebrou e travou nesse incremento."
            },

            // =============================================================
            // 4. FILTRO DE ÁGUA — quest da Dara. Formato C com subtração.
            // Conceito: laço decrescente + condição de parada.
            // =============================================================
            [IdFiltroDeAgua] = new DefinicaoDePuzzle
            {
                Id = IdFiltroDeAgua,
                Titulo = "Purga do filtro",
                Conceito = "Laço decrescente e condição de parada",
                Enunciado = "O filtro tem 60 de resíduo. Cada purga tira 8. Purga até sobrar menos que 8 e marca pronto = 1.",
                CodigoInicial = "residuo = 60\npronto = 0\n-- seu código aqui\n",
                Dica = "Subtrair é igual a somar, só com sinal trocado. Para quando não der mais pra tirar 8 inteiros.",
                Solucao = "residuo = 60\npronto = 0\nwhile residuo >= 8 do\n  residuo = residuo - 8\nend\nif residuo < 8 then\n  pronto = 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "residuo", Comparacao = TipoDeComparacao.Menor, Valor = 8 },
                    new VerificacaoDeVariavel { Variavel = "pronto", Comparacao = TipoDeComparacao.IgualA, Valor = 1 }
                },
                FalaDeVitoria = "Sobrou quatro. Não dá pra tirar mais — e você soube parar. Saber parar é metade do laço.",
                Curiosidade = "Filtro de carvão não limpa água. Ele segura o que é grande demais pra passar. O resto continua lá."
            },

            // =============================================================
            // 5. SEQUÊNCIA DA PORTA — Formato B. Conceito: ordem importa.
            // =============================================================
            [IdSequenciaDaPorta] = new DefinicaoDePuzzle
            {
                Id = IdSequenciaDaPorta,
                Titulo = "Tranca de três estágios",
                Conceito = "Sequência: a ordem das operações muda o resultado",
                Enunciado = "A tranca tem três estágios. Cada um multiplica o código por 2, depois soma 3. Começa em 1 e roda os três. O resultado tem que ser 29.",
                CodigoInicial = "codigo = 1\netapas = 0\n-- seu código aqui\n",
                Dica = "Multiplica PRIMEIRO, soma DEPOIS. Se inverter, dá outro número — e a tranca não abre.",
                Solucao = "codigo = 1\netapas = 0\nwhile etapas < 3 do\n  codigo = codigo * 2\n  codigo = codigo + 3\n  etapas = etapas + 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "codigo", Comparacao = TipoDeComparacao.IgualA, Valor = 29 },
                    new VerificacaoDeVariavel { Variavel = "etapas", Comparacao = TipoDeComparacao.IgualA, Valor = 3 }
                },
                FalaDeVitoria = "Vinte e nove. Se você tivesse somado antes de multiplicar, daria trinta e dois e a tranca ficava fechada. Ordem importa.",
                Curiosidade = "Trancas de três estágios eram padrão em depósito militar. Esta aqui é cópia pirata, feita por alguém que só viu o manual."
            },

            // =============================================================
            // 6. CALIBRAGEM DO BRAÇO — usa a API do próprio droid.
            // MECÂNICA DIFERENTE: o puzzle não é matemático, é fazer o
            // jogador USAR os comandos reais do jogo dentro de um laço.
            // =============================================================
            [IdCalibragemDoBraco] = new DefinicaoDePuzzle
            {
                Id = IdCalibragemDoBraco,
                Titulo = "Calibragem do braço",
                Conceito = "Chamar função e usar o valor que ela devolve",
                Enunciado = "Lê minha força com droid.obterAtributo(\"For\") e guarda em forca_lida. Depois guarda o dobro dela em previsao.",
                CodigoInicial = "forca_lida = 0\nprevisao = 0\n-- seu código aqui\n",
                Dica = "droid.obterAtributo(\"For\") DEVOLVE um número. Você pode guardar esse número numa variável e usar depois.",
                Solucao = "forca_lida = droid.obterAtributo(\"For\")\nprevisao = forca_lida * 2",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "forca_lida", Comparacao = TipoDeComparacao.MaiorQue, Valor = 0 },
                    new VerificacaoDeVariavel { Variavel = "previsao", Comparacao = TipoDeComparacao.MaiorQue, Valor = 0 }
                },
                // Este puzzle nao pode fixar valor exato: FOR muda conforme o
                // jogador evolui. As verificacoes checam "leu e derivou", que
                // e o que importa pedagogicamente.
                FalaDeVitoria = "Você leu meu estado e calculou em cima dele. É assim que um programa reage ao mundo em vez de só repetir.",
                Curiosidade = "Meu ataque base é força vezes dois. Você acabou de calcular sozinho o que eu calculo todo turno."
            },

            // =============================================================
            // 7. ROTA DE PATRULHA — Formato C com duas condições.
            // Conceito: condição composta.
            // =============================================================
            [IdRotaDePatrulha] = new DefinicaoDePuzzle
            {
                Id = IdRotaDePatrulha,
                Titulo = "Janela da patrulha",
                Conceito = "Condição composta (and) e decisão",
                Enunciado = "A patrulha passa a cada 12 minutos e leva 4 pra sumir de vista. Avança o relógio até dar 24 e, se o relógio for múltiplo de 12 E o tempo de espera for pelo menos 4, marca seguro = 1.",
                CodigoInicial = "relogio = 0\nespera = 4\nseguro = 0\n-- seu código aqui\n",
                Dica = "Em Lua, \"e\" se escreve and. Múltiplo de 12 dá resto zero: relogio % 12 == 0.",
                Solucao = "relogio = 0\nespera = 4\nseguro = 0\nwhile relogio < 24 do\n  relogio = relogio + 12\nend\nif relogio % 12 == 0 and espera >= 4 then\n  seguro = 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "relogio", Comparacao = TipoDeComparacao.IgualA, Valor = 24 },
                    new VerificacaoDeVariavel { Variavel = "seguro", Comparacao = TipoDeComparacao.IgualA, Valor = 1 }
                },
                FalaDeVitoria = "Duas condições ao mesmo tempo. Você não adivinhou a janela — você provou que ela existe.",
                Curiosidade = "Rotas de patrulha da Apex são regulares por economia de energia, não por descuido. Regularidade é o único furo que elas têm."
            },

            // =============================================================
            // 8. BOMBA DE RESERVA — Formato C, com armadilha proposital.
            // Conceito: condição de parada errada = laço infinito.
            // ⚠️ MoonSharp NÃO tem proteção contra laço infinito (pendência
            // conhecida). Este puzzle ensina o conceito de propósito, mas a
            // solução pedida NUNCA trava — ver a Dica.
            // =============================================================
            [IdBombaDeReserva] = new DefinicaoDePuzzle
            {
                Id = IdBombaDeReserva,
                Titulo = "Bomba de reserva",
                Conceito = "Condição de parada: por que um laço termina",
                Enunciado = "A bomba enche o tanque de 15 em 15 até 90. Mas ela só liga se pressao for exatamente 90 — nem mais, nem menos.",
                CodigoInicial = "pressao = 0\n-- seu código aqui\n",
                Dica = "90 dividido por 15 dá 6 exato, então parar em \"< 90\" chega certinho. Cuidado: se o incremento não fechar exato no alvo, o laço passa do ponto ou não para nunca.",
                Solucao = "pressao = 0\nwhile pressao < 90 do\n  pressao = pressao + 15\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "pressao", Comparacao = TipoDeComparacao.IgualA, Valor = 90 }
                },
                FalaDeVitoria = "Exato. Se o passo não fecha no alvo, o laço passa direto — ou nunca para. Esse é o erro que mais trava máquina nessa cidade.",
                Curiosidade = "O bilhete do Ivo dizia que a bomba de reserva ficava embaixo do piso da sala leste. Ele deixou escrito antes de fechar a estação."
            },

            // =============================================================
            // 9. TRIAGEM DE PEÇAS — Formato B com decisão dentro do laço.
            // Conceito: if DENTRO do while (classificar enquanto percorre).
            // =============================================================
            [IdTriagemDePecas] = new DefinicaoDePuzzle
            {
                Id = IdTriagemDePecas,
                Titulo = "Triagem do ferro-velho",
                Conceito = "Decisão dentro do laço (classificar enquanto percorre)",
                Enunciado = "Vinte peças numeradas de 1 a 20. Peça de número par vai pra reaproveitar, ímpar vai pra fundir. Conta quantas de cada.",
                CodigoInicial = "peca = 1\nreaproveitar = 0\nfundir = 0\n-- seu código aqui\n",
                Dica = "Par é resto zero na divisão por 2: peca % 2 == 0. O if fica DENTRO do while, junto com o incremento.",
                Solucao = "peca = 1\nreaproveitar = 0\nfundir = 0\nwhile peca <= 20 do\n  if peca % 2 == 0 then\n    reaproveitar = reaproveitar + 1\n  else\n    fundir = fundir + 1\n  end\n  peca = peca + 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "reaproveitar", Comparacao = TipoDeComparacao.IgualA, Valor = 10 },
                    new VerificacaoDeVariavel { Variavel = "fundir", Comparacao = TipoDeComparacao.IgualA, Valor = 10 },
                    new VerificacaoDeVariavel { Variavel = "peca", Comparacao = TipoDeComparacao.MaiorQue, Valor = 20 }
                },
                FalaDeVitoria = "Dez e dez. Você decidiu vinte vezes seguidas sem escrever vinte decisões. É pra isso que o laço serve.",
                Curiosidade = "A Guilda separa par e ímpar porque as peças vinham numeradas de fábrica em pares complementares. Ninguém lembra mais disso — só continuam separando."
            },

            // =============================================================
            // 10. ANTENA DA GUILDA — Formato B, o mais difícil da lista.
            // Conceito: dois laços encadeados (um dentro do outro é o passo
            // seguinte; aqui são dois em sequência, preparando o terreno).
            // =============================================================
            [IdAntenaDaGuilda] = new DefinicaoDePuzzle
            {
                Id = IdAntenaDaGuilda,
                Titulo = "Sintonia da antena",
                Conceito = "Dois laços em sequência (subir e depois afinar)",
                Enunciado = "Sobe a frequência de 100 em 100 até passar de 800. Depois desce de 25 em 25 até ficar em 800 ou menos. E conta quantos ajustes ao todo.",
                CodigoInicial = "freq = 0\najustes = 0\n-- seu código aqui\n",
                Dica = "Dois while, um depois do outro. O contador de ajustes é o mesmo nos dois — ele não zera no meio.",
                Solucao = "freq = 0\najustes = 0\nwhile freq <= 800 do\n  freq = freq + 100\n  ajustes = ajustes + 1\nend\nwhile freq > 800 do\n  freq = freq - 25\n  ajustes = ajustes + 1\nend",
                Verificacoes = new List<VerificacaoDeVariavel>
                {
                    new VerificacaoDeVariavel { Variavel = "freq", Comparacao = TipoDeComparacao.IgualA, Valor = 800 },
                    new VerificacaoDeVariavel { Variavel = "ajustes", Comparacao = TipoDeComparacao.IgualA, Valor = 13 }
                },
                FalaDeVitoria = "Treze ajustes. Nove pra subir demais, quatro pra voltar. Às vezes o caminho certo passa do ponto antes de acertar.",
                Curiosidade = "A antena da Guilda não transmite. Só escuta. Bruna diz que é mais seguro assim, e ela nunca explicou por quê."
            }
        };

    public static DefinicaoDePuzzle Obter(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return Todos.TryGetValue(id, out DefinicaoDePuzzle p) ? p : null;
    }
}

public enum TipoDeComparacao
{
    MaiorOuIgual,
    MaiorQue,
    IgualA,
    Menor,
    MenorOuIgual,
    Diferente
}

public class VerificacaoDeVariavel
{
    public string Variavel;
    public TipoDeComparacao Comparacao = TipoDeComparacao.MaiorOuIgual;
    public double Valor;

    public bool Satisfeita(double lido)
    {
        const double Epsilon = 0.0001;
        switch (Comparacao)
        {
            case TipoDeComparacao.MaiorQue: return lido > Valor;
            case TipoDeComparacao.IgualA: return System.Math.Abs(lido - Valor) < Epsilon;
            case TipoDeComparacao.Menor: return lido < Valor;
            case TipoDeComparacao.MenorOuIgual: return lido <= Valor;
            case TipoDeComparacao.Diferente: return System.Math.Abs(lido - Valor) >= Epsilon;
            default: return lido >= Valor;
        }
    }

    public string Descrever() => $"{Variavel} {Simbolo()} {Valor}";

    private string Simbolo()
    {
        switch (Comparacao)
        {
            case TipoDeComparacao.MaiorQue: return ">";
            case TipoDeComparacao.IgualA: return "=";
            case TipoDeComparacao.Menor: return "<";
            case TipoDeComparacao.MenorOuIgual: return "<=";
            case TipoDeComparacao.Diferente: return "~=";
            default: return ">=";
        }
    }
}

public class DefinicaoDePuzzle
{
    public string Id;
    public string Titulo;
    public string Conceito = "";
    public string Enunciado = "";

    /// <summary>Pré-preenchido no campo do terminal ao abrir o puzzle.</summary>
    public string CodigoInicial = "";

    /// <summary>Mostrada pelo Apollo depois de algumas tentativas erradas.</summary>
    public string Dica = "";

    /// <summary>
    /// Solução de referência. NUNCA mostrada ao jogador pelo código do jogo —
    /// existe pra documentação, teste automatizado e pra você conferir na
    /// hora de montar a cena.
    /// </summary>
    public string Solucao = "";

    public List<VerificacaoDeVariavel> Verificacoes = new List<VerificacaoDeVariavel>();

    public string FalaDeVitoria = "";
    public string Curiosidade = "";
}
