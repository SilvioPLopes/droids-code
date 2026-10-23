using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using DroidsCode.Casa;
using DroidsCode.Scripting;
using DroidsCode.DroidCore;
using TMPro;

/// <summary>
/// Harness manual de Editor. Nada executa ao compilar/importar.
/// Inspecoes nao criam singletons, carregam cenas nem acessam o save.
/// Cada metodo retorna OK somente depois de todas as assercoes.
/// </summary>
public static class ValidacaoCasaLip
{
    private const string Conclusao = "ato1_casa_resolvida";
    private const string Introducao = "ato1_falou_apollo_casa";
    private const BindingFlags Campos = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static string ValidarEstadoDaCasa(bool resolvidaEsperada)
    {
        Exigir(Application.isPlaying, "Estado da casa exige Play Mode.");
        Scene casa = SceneManager.GetSceneByName("CasaLip");
        Exigir(casa.IsValid() && casa.isLoaded, "CasaLip nao esta carregada.");
        GerenciadorDeEstado estado = EstadoExistente();
        Exigir(estado.ObterFlag(Conclusao) == resolvidaEsperada, "Flag de conclusao diverge do esperado: " + resolvidaEsperada);

        var portas = Vivos<TransicaoDeCena>().Where(p => p.gameObject.scene == casa && p.nomeCenaDestino == "City").ToArray();
        Exigir(portas.Length == 1, "Esperada uma porta CasaLip -> City; encontradas " + portas.Length);
        Exigir(portas[0].enabled, "Componente da porta foi desabilitado.");
        Exigir(portas[0].trancada == !resolvidaEsperada, "Porta CasaLip -> City: trancada=" + portas[0].trancada + "; esperado=" + !resolvidaEsperada);

        var terminais = Vivos<TerminalUIManager>().Where(TerminalValido).ToArray();
        Exigir(terminais.Length == 1, "Esperada uma UI terminal viva/valida (inclui DontDestroyOnLoad); encontradas " + terminais.Length);
        var interacoes = Vivos<InteragirComTerminalCasa>().Where(i => i.gameObject.scene == casa).ToArray();
        Exigir(interacoes.Length == 1, "Esperada uma interacao local com terminal; encontradas " + interacoes.Length);
        Exigir(ReferenceEquals(LerCampo(interacoes[0], "terminal"), terminais[0]), "Interacao da casa perdeu referencia ao terminal vivo apos revisita.");

        var verificadores = Vivos<VerificadorDePuzzle>().Where(v => v.gameObject.scene == casa && v.idDoPuzzle == "p_agua_casa").ToArray();
        Exigir(verificadores.Length == 1, "Esperado um VerificadorDePuzzle local p_agua_casa; encontrados " + verificadores.Length);
        var verificador = verificadores[0];
        Exigir(verificador.isActiveAndEnabled, "Verificador local deve permanecer ativo com terminal fechado.");
        Exigir(verificador.terminal == terminais[0], "Verificador perdeu a associacao com terminal vivo.");
        Exigir(verificador.portaParaDestrancar == portas[0], "Verificador referencia porta de outra cena ou destruida.");
        Exigir(verificador.flagAoResolver == Conclusao && verificador.flagQueJaResolve == Conclusao, "Flags do verificador nao correspondem a conclusao da casa.");
        Exigir((bool)LerCampo(verificador, "_resolvido") == resolvidaEsperada, "Estado interno do verificador diverge da conclusao persistida.");
        Exigir(!Vivos<PuzzleAguaCasaChecker>().Any(), "Checker legado PuzzleAguaCasaChecker ainda esta anexado (inclusive inativo/persistente).");
        return "OK";
    }

    public static string ValidarMenus(int quantidadeEsperada)
    {
        GerenciadorDeEstado estado = EstadoExistente();
        int quantidade = (int)LerCampo(estado, "_contadorMenusAbertos");
        Exigir(quantidade == quantidadeEsperada, "Menus abertos=" + quantidade + "; esperado=" + quantidadeEsperada);
        Exigir(estado.MenuAberto == (quantidadeEsperada > 0), "MenuAberto diverge do contador.");
        return "OK";
    }

    public static string ValidarMigracaoCasaLip()
    {
        MethodInfo migrar = typeof(SalvamentoJson).GetMethod("MigrarDados", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(DadosDoJogo) }, null);
        Exigir(migrar != null, "Migracao em memoria ainda nao implementada: SalvamentoJson.MigrarDados(DadosDoJogo).");
        var antigo = Fixture(5);
        antigo.flagsDeHistoria.Add(new FlagDeHistoria { chave = Conclusao, valor = true });
        migrar.Invoke(null, new object[] { antigo });
        Exigir(antigo.versao == 7, "Migracao v5 nao atualizou versao para 7.");
        Exigir(Flag(antigo, Introducao), "Conversa antiga deve migrar para introducao.");
        Exigir(Flag(antigo, Conclusao), "Conclusao legada verdadeira deve ser preservada para nao apagar progresso do jogador.");
        ValidarProgressoPreservado(antigo);
        string primeira = JsonUtility.ToJson(antigo);
        migrar.Invoke(null, new object[] { antigo });
        Exigir(JsonUtility.ToJson(antigo) == primeira, "Migracao nao e idempotente.");

        var semFlag = Fixture(5);
        migrar.Invoke(null, new object[] { semFlag });
        Exigir(semFlag.versao == 7 && !Flag(semFlag, Introducao) && !Flag(semFlag, Conclusao), "v5 sem flag inventou progresso de historia.");
        ValidarProgressoPreservado(semFlag);

        var moderno = Fixture(6);
        moderno.droid.energiaAtual = 25;
        moderno.droid.energiaMaxima = 100;
        moderno.flagsDeHistoria.Add(new FlagDeHistoria { chave = Conclusao, valor = true });
        // A migração v7 só adiciona equipamentos/versão; conserva todos os dados v6.
        moderno.versao = 7;
        string antes = JsonUtility.ToJson(moderno);
        moderno.versao = 6;
        migrar.Invoke(null, new object[] { moderno });
        Exigir(JsonUtility.ToJson(moderno) == antes, "Migração v6→v7 alterou dados anteriores.");
        return "OK";
    }

    /// <summary>
    /// Play Mode: fixture isolada, usa o estado ja existente; nao executa Lua
    /// no terminal da cena. Restaura contador e destroi somente a fixture.
    /// Detecta Runner recriado, registro duplicado e fechamento duplicado.
    /// </summary>
    public static string ValidarAberturaRepetidaDoTerminal()
    {
        Exigir(Application.isPlaying, "Teste de terminal exige Play Mode.");
        var estado = EstadoExistente();
        int anterior = (int)LerCampo(estado, "_contadorMenusAbertos");
        var raiz = new GameObject("FixtureCasaLipTerminal");
        try
        {
            var terminal = raiz.AddComponent<TerminalUIManager>();
            terminal.painelTerminal = new GameObject("PainelFixture");
            terminal.painelTerminal.transform.SetParent(raiz.transform);
            terminal.painelTerminal.SetActive(false);
            Invocar(terminal, "Start");
            int fechamentos = 0;
            terminal.AoFechar = () => fechamentos++;
            terminal.Abrir();
            var runner = terminal.Runner;
            Exigir(runner != null, "Primeira abertura nao criou Runner.");
            Exigir(runner.ExecutarCodigo("marcador_fixture = 37").Sucesso, "Fixture Lua finita falhou.");
            terminal.Abrir();
            var falhas = new List<string>();
            if (!ReferenceEquals(terminal.Runner, runner)) falhas.Add("Abrir duas vezes substituiu Runner");
            if (terminal.Runner.ObterVariavelNumerica("marcador_fixture") != 37) falhas.Add("Abrir duas vezes perdeu variavel da sessao");
            if ((int)LerCampo(estado, "_contadorMenusAbertos") != anterior + 1) falhas.Add("Abrir duas vezes acumulou bloqueio de menu");
            terminal.Fechar();
            terminal.Fechar();
            if ((int)LerCampo(estado, "_contadorMenusAbertos") != anterior) falhas.Add("Fechar deixou bloqueio acumulado");
            if (fechamentos != 1) falhas.Add("Callback de fechar deve ocorrer uma vez");
            Exigir(falhas.Count == 0, string.Join("; ", falhas));
            return "OK";
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(raiz);
            typeof(GerenciadorDeEstado).GetField("_contadorMenusAbertos", Campos).SetValue(estado, anterior);
        }
    }

    /// <summary>
    /// Reproduz Mostrar antes do primeiro Start com painel originalmente
    /// inativo. Start e invocado explicitamente para manter teste sincrono.
    /// Nao executa callback narrativo, progresso nem save.
    /// </summary>
    public static string ValidarPrimeiroDialogoInativo()
    {
        Exigir(Application.isPlaying, "Teste de dialogo exige Play Mode.");
        var estado = EstadoExistente();
        int anterior = (int)LerCampo(estado, "_contadorMenusAbertos");
        FieldInfo singleton = typeof(DialogoUIManager).GetField("_instancia", BindingFlags.Static | BindingFlags.NonPublic);
        object instanciaAnterior = singleton.GetValue(null);
        var raiz = new GameObject("FixtureCasaLipDialogo");
        raiz.SetActive(false);
        try
        {
            var dialogo = raiz.AddComponent<DialogoUIManager>();
            dialogo.painel = raiz;
            var texto = new GameObject("FalaFixture", typeof(RectTransform), typeof(CanvasRenderer));
            texto.transform.SetParent(raiz.transform);
            dialogo.textoFala = texto.AddComponent<TextMeshProUGUI>();
            Exigir(dialogo.Mostrar(new[] { new FalaDeDialogo { falante = "", texto = "Primeira fala preservada." } }), "Primeiro Mostrar recusado.");
            Invocar(dialogo, "Start");
            Exigir(raiz.activeSelf && (bool)LerCampo(dialogo, "_aberto"), "Start ocultou a primeira fala aberta quando UI estava inativa.");
            Exigir(dialogo.textoFala.text == "Primeira fala preservada.", "Primeira fala foi perdida/substituida.");
            Exigir((int)LerCampo(estado, "_contadorMenusAbertos") == anterior + 1, "Dialogo nao adquiriu exatamente um bloqueio.");
            dialogo.Fechar();
            dialogo.Fechar();
            Exigir((int)LerCampo(estado, "_contadorMenusAbertos") == anterior, "Fechar dialogo nao equilibrou contador.");
            return "OK";
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(raiz);
            singleton.SetValue(null, instanciaAnterior);
            typeof(GerenciadorDeEstado).GetField("_contadorMenusAbertos", Campos).SetValue(estado, anterior);
        }
    }

    /// <summary>Somente Runner isolado e codigo finito; nao prova efeitos do puzzle na cena.</summary>
    public static string ValidarLeituraLuaFinita()
    {
        var runner = new DroidScriptRunner(new TerminalDroidApi(new Droid("Fixture", 100)));
        Exigir(runner.ObterVariavelNumerica("nivel_agua") == null, "Variavel ausente deve retornar null.");
        Exigir(runner.ExecutarCodigo("nivel_agua = 'texto'").Sucesso && runner.ObterVariavelNumerica("nivel_agua") == null, "Texto nao deve ser interpretado como numero.");
        Exigir(runner.ExecutarCodigo("nivel_agua = 90").Sucesso && runner.ObterVariavelNumerica("nivel_agua") == 90, "Valor 90 lido incorretamente.");
        Exigir(runner.ExecutarCodigo("nivel_agua = 100").Sucesso && runner.ObterVariavelNumerica("nivel_agua") == 100, "Atribuicao direta 100 lida incorretamente.");
        Exigir(runner.ExecutarCodigo("nivel_agua = 0; while nivel_agua < 100 do nivel_agua = nivel_agua + 10 end").Sucesso && runner.ObterVariavelNumerica("nivel_agua") == 100, "Laco finito nao atingiu 100.");
        return "OK";
    }

    /// <summary>Fixture de UI: contexto nao pode apagar rascunho ou reiniciar VM.</summary>
    public static string ValidarContextoDoTerminal()
    {
        Exigir(Application.isPlaying, "Teste de contexto exige Play Mode.");
        var estado = EstadoExistente();
        int anterior = (int)LerCampo(estado, "_contadorMenusAbertos");
        var raiz = new GameObject("FixtureCasaLipContexto");
        try
        {
            var terminal = raiz.AddComponent<TerminalUIManager>();
            var painel = new GameObject("PainelFixture", typeof(RectTransform));
            painel.transform.SetParent(raiz.transform);
            terminal.painelTerminal = painel;
            var entrada = new GameObject("CodigoFixture", typeof(RectTransform));
            entrada.transform.SetParent(painel.transform);
            terminal.campoDeCodigo = entrada.AddComponent<TMP_InputField>();
            var ajuda = new GameObject("AjudaFixture", typeof(RectTransform));
            ajuda.transform.SetParent(painel.transform);
            terminal.textoAjuda = ajuda.AddComponent<TextMeshProUGUI>();
            Invocar(terminal, "Start");
            string ajudaGeral = terminal.textoAjuda.text;
            MethodInfo definir = typeof(TerminalUIManager).GetMethod("DefinirContextoPuzzle", new[] { typeof(string) });
            MethodInfo limpar = typeof(TerminalUIManager).GetMethod("LimparContextoPuzzle", new[] { typeof(string) });
            Exigir(definir != null && limpar != null, "API de contexto ainda nao implementada.");
            definir.Invoke(terminal, new object[] { "p_agua_casa" });
            terminal.Abrir();
            Exigir(terminal.campoDeCodigo.text == "nivel_agua = 0\n-- seu código aqui\n", "Contexto inicial nao apresentou CodigoInicial da agua.");
            Exigir(terminal.textoAjuda.text.Contains("O valor não muda sozinho."), "Ajuda do contexto nao apresentou enunciado da agua.");
            var runner = terminal.Runner;
            terminal.campoDeCodigo.text = "-- meu rascunho com E\nnivel_agua = 90";
            string ajudaPuzzle = terminal.textoAjuda.text;
            definir.Invoke(terminal, new object[] { "p_agua_casa" });
            Exigir(terminal.campoDeCodigo.text == "-- meu rascunho com E\nnivel_agua = 90" && ReferenceEquals(runner, terminal.Runner), "Reaplicar contexto apagou rascunho/Runner.");
            limpar.Invoke(terminal, new object[] { "outro_puzzle" });
            Exigir(terminal.textoAjuda.text == ajudaPuzzle, "Limpar contexto de outro id alterou a ajuda.");
            limpar.Invoke(terminal, new object[] { "p_agua_casa" });
            Exigir(terminal.textoAjuda.text == ajudaGeral, "Limpar contexto nao restaurou ajuda geral.");
            Exigir(terminal.campoDeCodigo.text == "-- meu rascunho com E\nnivel_agua = 90" && ReferenceEquals(runner, terminal.Runner), "Limpar contexto apagou rascunho/Runner.");
            terminal.Fechar();
            Exigir((int)LerCampo(estado, "_contadorMenusAbertos") == anterior, "Teste de contexto deixou bloqueio de menu.");
            return "OK";
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(raiz);
            typeof(GerenciadorDeEstado).GetField("_contadorMenusAbertos", Campos).SetValue(estado, anterior);
        }
    }

    /// <summary>
    /// Fixture raiz inativa evita Awake de MenuMundoManager/duplicata DDOL.
    /// Exercita APIs reais de menu/terminal, sem input ou componentes da cena.
    /// </summary>
    public static string ValidarOrigemDoTerminal()
    {
        Exigir(Application.isPlaying, "Teste de origem exige Play Mode.");
        var estado = EstadoExistente();
        int anterior = (int)LerCampo(estado, "_contadorMenusAbertos");
        var raiz = new GameObject("FixtureCasaLipOrigem");
        raiz.SetActive(false);
        try
        {
            var menu = raiz.AddComponent<MenuMundoManager>();
            var terminal = raiz.AddComponent<TerminalUIManager>();
            menu.painelMenu = new GameObject("MenuFixture");
            menu.painelMenu.transform.SetParent(raiz.transform);
            terminal.painelTerminal = new GameObject("TerminalFixture");
            terminal.painelTerminal.transform.SetParent(raiz.transform);
            menu.terminal = terminal;
            Invocar(terminal, "Start");
            Invocar(menu, "Start");

            // Abrir pelo mundo nao adquire nem exibe o menu do mundo ao fechar.
            terminal.Abrir();
            Exigir((int)LerCampo(estado, "_contadorMenusAbertos") == anterior + 1 && !menu.painelMenu.activeSelf, "Terminal do mundo abriu menu ou registrou bloqueio incorreto.");
            terminal.Fechar();
            Exigir(!menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior, "Fechar terminal do mundo revelou menu ou deixou bloqueio.");

            // Abrir pelo menu preserva sua propriedade enquanto ele fica oculto.
            Invocar(menu, "Alternar");
            Exigir(menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior + 1, "Menu nao adquiriu exatamente um bloqueio.");
            Invocar(menu, "AoClicarTerminal");
            Exigir(!menu.painelMenu.activeSelf && terminal.painelTerminal.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior + 2, "Origem menu deve ocultar painel mantendo dois bloqueios pertencentes aos paineis.");
            terminal.Fechar();
            Exigir(menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior + 1, "Fechar terminal originado no menu nao devolveu menu/bloqueio correto.");
            Invocar(menu, "Fechar");
            Exigir(!menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior, "Fechar menu deixou bloqueio.");

            // Transicao para dialogo apaga origem antes do callback do terminal.
            Invocar(menu, "Alternar");
            Invocar(menu, "AoClicarTerminal");
            Invocar(menu, "FecharParaDialogo");
            Exigir(!menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior + 1, "FecharParaDialogo nao liberou somente o bloqueio do menu oculto.");
            terminal.Fechar();
            Exigir(!menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior, "Callback terminal reabriu menu depois de preparar dialogo.");
            terminal.Abrir();
            terminal.Fechar();
            Exigir(!menu.painelMenu.activeSelf && (int)LerCampo(estado, "_contadorMenusAbertos") == anterior, "Origem de menu vazou para abertura posterior pelo mundo.");
            return "OK";
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(raiz);
            typeof(GerenciadorDeEstado).GetField("_contadorMenusAbertos", Campos).SetValue(estado, anterior);
        }
    }

    private static void Invocar(object alvo, string nome)
    {
        MethodInfo metodo = alvo.GetType().GetMethod(nome, Campos);
        Exigir(metodo != null, "Metodo de fixture nao encontrado: " + nome);
        try { metodo.Invoke(alvo, null); }
        catch (TargetInvocationException erro) { throw erro.InnerException ?? erro; }
    }

    private static DadosDoJogo Fixture(int versao)
    {
        var dados = new DadosDoJogo { versao = versao, cena = "City", gold = 73, posicaoX = 12.5f, droid = new DroidSalvo { nome = "Fixture", hp = 31, nivel = 4 } };
        dados.flagsDeHistoria.Add(new FlagDeHistoria { chave = "outra_flag", valor = true });
        dados.contadores.Add(new ContadorDeQuest { chave = "outra_quest", valor = 3 });
        return dados;
    }

    private static void ValidarProgressoPreservado(DadosDoJogo dados)
    {
        Exigir(Flag(dados, "outra_flag") && dados.contadores.Count == 1 && dados.contadores[0].valor == 3 && dados.contadores[0].chave == "outra_quest", "Migracao alterou outras flags/contadores.");
        Exigir(dados.cena == "City" && dados.gold == 73 && dados.posicaoX == 12.5f && dados.droid.nome == "Fixture" && dados.droid.hp == 31 && dados.droid.nivel == 4, "Migracao alterou dados nao relacionados.");
    }

    private static bool Flag(DadosDoJogo dados, string chave) { return dados.flagsDeHistoria.Any(f => f.chave == chave && f.valor); }
    private static bool TerminalValido(TerminalUIManager terminal) { return terminal.painelTerminal != null && terminal.campoDeCodigo != null; }
    private static T[] Vivos<T>() where T : Component { return Resources.FindObjectsOfTypeAll<T>().Where(c => c != null && c.gameObject.scene.IsValid() && c.gameObject.scene.isLoaded).ToArray(); }
    private static GerenciadorDeEstado EstadoExistente()
    {
        var estados = Vivos<GerenciadorDeEstado>();
        Exigir(estados.Length == 1, "Esperado um GerenciadorDeEstado ja existente; encontrados " + estados.Length + ". A validacao nao cria singleton.");
        return estados[0];
    }
    private static object LerCampo(object alvo, string nome)
    {
        FieldInfo campo = alvo.GetType().GetField(nome, Campos);
        Exigir(campo != null, "Campo de inspecao nao encontrado: " + nome);
        return campo.GetValue(alvo);
    }
    private static void Exigir(bool condicao, string mensagem) { if (!condicao) throw new InvalidOperationException("[CasaLip] " + mensagem); }
}
