using System.Collections.Generic;

/// <summary>
/// APOLLO DICA — versao minima e honesta do "Apollo Debugger" prometido no
/// readme.md.
///
/// O QUE E: traduz a mensagem crua de erro do MoonSharp numa explicacao
/// CONCEITUAL, na voz do Apollo, mostrada no proprio log do terminal.
///
/// O QUE NAO E (importante nao confundir, nem na monografia nem na banca):
/// isto NAO destaca a linha do erro dentro do editor de codigo, nao e
/// reativo em tempo real enquanto o jogador digita, e nao analisa o codigo —
/// so o texto do erro. O "Apollo Debugger" completo continua no
/// CHECKLIST → Amadurecimento. Isto e o primeiro degrau dele.
///
/// POR QUE E BARATO: o DroidScriptRunner JA captura SyntaxErrorException e
/// ScriptRuntimeException e devolve ex.DecoratedMessage (que ja inclui a
/// linha, no formato "chunk_N:(linha,coluna)"), e o TerminalUIManager JA tem
/// log de sessao. Nao havia nada a construir alem desta traducao.
///
/// COMO CRESCER: adicione pares em _dicas. A chave e um trecho em minusculo
/// que aparece na mensagem do MoonSharp; o valor e a fala do Apollo. A ordem
/// importa — a primeira chave encontrada ganha, entao deixe as mais
/// especificas em cima.
/// </summary>
public static class ApolloDica
{
    /// <summary>Prefixo usado no log do terminal, pra fala nao se confundir com saida de sistema.</summary>
    public const string Prefixo = "APOLLO:";

    private static readonly List<KeyValuePair<string, string>> _dicas =
        new List<KeyValuePair<string, string>>
        {
            // --- Erros de sintaxe (SyntaxErrorException) ---------------
            new KeyValuePair<string, string>("'end' expected",
                "Voce abriu um bloco e nao fechou. Todo 'while', todo 'if' e toda funcao precisa de um 'end' correspondente. Conta quantos voce abriu e quantos voce fechou."),

            new KeyValuePair<string, string>("'then' expected",
                "Um 'if' precisa de 'then' depois da condicao. Fica assim: if energia >= 50 then ... end."),

            new KeyValuePair<string, string>("'do' expected",
                "Um 'while' precisa de 'do' depois da condicao. Fica assim: while nivel < 100 do ... end."),

            new KeyValuePair<string, string>("unexpected symbol",
                "Tem um simbolo fora do lugar. Confere se voce fechou todo parentese e toda aspa que abriu — e lembra que aqui e Lua, nao precisa de ponto e virgula no fim da linha."),

            new KeyValuePair<string, string>("unfinished string",
                "Voce abriu aspas e nao fechou. Todo texto entre aspas precisa de aspas dos dois lados."),

            new KeyValuePair<string, string>("malformed number",
                "Esse numero esta escrito de um jeito que eu nao consigo ler. Usa ponto pra decimal, nao virgula."),

            new KeyValuePair<string, string>("<eof>",
                "O codigo acabou antes de fechar tudo que voce abriu. Normalmente e um 'end' faltando no fim."),

            // --- Erros de execucao (ScriptRuntimeException) ------------
            new KeyValuePair<string, string>("attempt to index",
                "Voce tentou usar alguma coisa que ainda nao existe. Confere se o nome esta escrito exatamente igual em todo lugar — 'nivel_agua' e 'nivelAgua' sao variaveis diferentes pra mim."),

            new KeyValuePair<string, string>("attempt to perform arithmetic",
                "Voce tentou fazer conta com algo que nao e numero. Isso quase sempre e uma variavel que nunca recebeu valor — cria ela com zero antes de somar."),

            new KeyValuePair<string, string>("attempt to compare",
                "Voce tentou comparar coisas de tipos diferentes (numero com texto, por exemplo). Confere o que tem dentro de cada lado do sinal."),

            new KeyValuePair<string, string>("attempt to call",
                "Voce chamou algo que nao e um comando. Confere o nome — os meus sao 'droid.subirAtributo', 'droid.aprenderTecnica', e por ai. Digita e clica Executar pra ver a lista de ajuda."),

            new KeyValuePair<string, string>("bad argument",
                "O comando existe, mas o que voce passou dentro dos parenteses nao serve. Texto vai entre aspas; numero vai sem."),

            new KeyValuePair<string, string>("cannot convert",
                "Voce passou um tipo de valor onde eu esperava outro. Numero sem aspas, texto com aspas."),

            new KeyValuePair<string, string>("stack overflow",
                "Alguma coisa esta se chamando sem parar. Confere se sua condicao de parada realmente chega a ser verdadeira em algum momento.")
        };

    /// <summary>
    /// Devolve a fala do Apollo pra uma mensagem de erro, ja com o prefixo.
    /// Nunca devolve null nem string vazia — erro desconhecido ganha uma
    /// resposta honesta, porque o pior resultado pedagogico seria o Apollo
    /// ficar mudo justamente quando o jogador travou.
    /// </summary>
    public static string Traduzir(string mensagemDeErro)
    {
        if (string.IsNullOrWhiteSpace(mensagemDeErro))
        {
            return $"{Prefixo} Alguma coisa falhou, mas nem eu entendi o que. Roda de novo devagar.";
        }

        string normalizada = mensagemDeErro.ToLowerInvariant();

        foreach (KeyValuePair<string, string> dica in _dicas)
        {
            if (normalizada.Contains(dica.Key))
            {
                return $"{Prefixo} {dica.Value}";
            }
        }

        return $"{Prefixo} Essa eu nao conheco de cabeca. Le a linha que apareceu no erro devagar, e confere nome por nome — quase sempre e um nome escrito diferente ou um bloco sem fechar.";
    }

    /// <summary>
    /// Extrai "linha N" da mensagem decorada do MoonSharp, que vem no
    /// formato "chunk_0:(3,12): mensagem". Devolve string vazia se nao
    /// conseguir — o chamador simplesmente nao mostra a linha nesse caso.
    /// </summary>
    public static string ExtrairLinha(string mensagemDeErro)
    {
        if (string.IsNullOrEmpty(mensagemDeErro)) return "";

        int abre = mensagemDeErro.IndexOf('(');
        if (abre < 0) return "";

        int virgula = mensagemDeErro.IndexOf(',', abre);
        if (virgula < 0) return "";

        string possivelLinha = mensagemDeErro.Substring(abre + 1, virgula - abre - 1).Trim();

        return int.TryParse(possivelLinha, out int linha) && linha > 0
            ? $"(linha {linha})"
            : "";
    }
}
