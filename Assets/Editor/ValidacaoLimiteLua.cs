using System;
using System.Reflection;
using DroidsCode.Scripting;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fixture manual, somente Lua finito, sem cenas, saves ou execução ao importar.</summary>
public static class ValidacaoLimiteLua
{
    static void Exigir(bool valor, string mensagem)
    {
        if (!valor) throw new InvalidOperationException(mensagem);
    }

    public static string Executar()
    {
        var runner = new DroidScriptRunner(new TerminalDroidApi(new Droid("FixtureLua", 30)));
        Exigir(runner.ExecutarCodigo("resposta = 6 * 7").Sucesso && runner.ObterVariavelNumerica("resposta") == 42,
            "Programa curto não preservou o cálculo.");
        Type limitesTipo = typeof(DroidScriptRunner).Assembly.GetType("DroidsCode.Scripting.LimitesDeExecucao");
        Exigir(limitesTipo != null, "RED: motor ainda não oferece limites configuráveis.");
        object limites = Activator.CreateInstance(limitesTipo);
        limitesTipo.GetProperty("InstrucoesPorRetomada").SetValue(limites, 100);
        limitesTipo.GetProperty("MaximoDeRetomadas").SetValue(limites, 1);
        var iniciar = typeof(DroidScriptRunner).GetMethod("IniciarExecucao");
        var avancar = typeof(DroidScriptRunner).GetMethod("AvancarExecucao");
        Exigir(iniciar != null && avancar != null, "RED: falta ciclo cooperativo.");
        Exigir((bool)iniciar.Invoke(runner, new[] { (object)"nivel_agua=10; for i=1,10000 do nivel_agua=nivel_agua+1 end", limites }), "Não iniciou programa finito.");
        string estado = "Executando";
        for (int passo = 0; passo < 5 && estado == "Executando"; passo++)
            estado = avancar.Invoke(runner, null).ToString();
        Exigir(estado == "LimiteExcedido", "Loop finito não foi interrompido pelo orçamento pequeno: " + estado);
        Exigir(runner.ObterVariavelNumerica("nivel_agua") == null, "Variável parcial pode aprovar puzzle após interrupção.");
        Exigir(runner.ExecutarCodigo("resposta=21*2").Sucesso && runner.ObterVariavelNumerica("resposta") == 42,
            "Próxima execução curta não recuperou a sessão.");
        return ExecutarContratos() + " RED original confirmado separadamente; sem saves/Play.";
    }

    static string ExecutarContratos()
    {
        var droid = new Droid("FixtureLuaContratos", 30);
        var api = new TerminalDroidApi(droid);
        var runner = new DroidScriptRunner(api);
        Exigir(runner.ExecutarCodigo("contador=3").Sucesso, "Primeira execução curta.");
        Exigir(runner.ExecutarCodigo("contador=contador+4").Sucesso && runner.ObterVariavelNumerica("contador") == 7,
            "Globais não persistiram entre sucessos.");
        Exigir(runner.IniciarExecucao("nivel_agua=10; for i=1,10000 do nivel_agua=nivel_agua+1 end",
            new LimitesDeExecucao { InstrucoesPorRetomada = 100, MaximoDeRetomadas = 1 }), "Início quota pequena.");
        Exigir(runner.ObterVariavelNumerica("contador") == null, "Leitura de vitória durante execução.");
        Exigir(runner.AvancarExecucao() == EstadoExecucao.LimiteExcedido && !runner.UltimoResultado.Sucesso,
            "Quota pequena não interrompeu loop finito.");
        Exigir(runner.UltimoResultado.MensagemErro == DroidScriptRunner.MensagemLimiteInstrucoes, "Feedback de limite divergente.");
        Exigir(runner.ObterVariavelNumerica("nivel_agua") == null && runner.ObterVariavelNumerica("contador") == null,
            "Sessão parcial não descartada.");
        Exigir(runner.IniciarExecucao("nivel_agua=10; for i=1,10000 do nivel_agua=nivel_agua+1 end", new LimitesDeExecucao()), "Início quota suficiente.");
        Exigir(Drenar(runner) == EstadoExecucao.Concluida && runner.ObterVariavelNumerica("nivel_agua") == 10010,
            "Mesmo loop finito não concluiu com orçamento suficiente.");
        Exigir(runner.ExecutarCodigo("nivel_agua=0; while nivel_agua < 10 do nivel_agua=nivel_agua+1 end").Sucesso &&
            runner.ObterVariavelNumerica("nivel_agua") == 10, "Exemplo while atual.");
        Exigir(runner.ExecutarCodigo("function fat(n) if n<=1 then return 1 end return n*fat(n-1) end; recursao=fat(8)").Sucesso &&
            runner.ObterVariavelNumerica("recursao") == 40320, "Recursão finita.");
        Exigir(!runner.ExecutarCodigo("if then").Sucesso && runner.UltimoResultado.ErroDeLua, "Erro de sintaxe não tratado.");
        Exigir(runner.ObterVariavelNumerica("nivel_agua") == null, "Sintaxe inválida preservou VM anterior.");
        Exigir(!runner.ExecutarCodigo("parcial=10; error('fixture')").Sucesso && runner.UltimoResultado.ErroDeLua &&
            runner.ObterVariavelNumerica("parcial") == null, "Erro runtime deixou parcial legível.");
        Exigir(runner.IniciarExecucao("cancelado=9; for i=1,10000 do cancelado=cancelado+1 end", new LimitesDeExecucao()), "Início cancelamento.");
        Exigir(!runner.IniciarExecucao("substituto=1", new LimitesDeExecucao()), "Duplo início substituiu job.");
        Exigir(!runner.ExecutarCodigo("substituto=2").Sucesso && runner.EstadoAtual == EstadoExecucao.Executando,
            "Adaptador síncrono substituiu job.");
        Exigir(runner.AvancarExecucao() == EstadoExecucao.Executando, "Loop finito deveria ceder execução antes de terminar.");
        runner.CancelarExecucao(); runner.CancelarExecucao();
        Exigir(runner.EstadoAtual == EstadoExecucao.Cancelada && runner.ObterVariavelNumerica("cancelado") == null,
            "Cancelamento não descartou sessão ou não foi idempotente.");
        Exigir(runner.ExecutarCodigo("reaberto=42").Sucesso && runner.ObterVariavelNumerica("reaberto") == 42,
            "Execução após cancelamento não recuperou VM.");
        Exigir(runner.ExecutarCodigo("assert(coroutine==nil and string==nil and table==nil and pcall==nil and xpcall==nil and load==nil and require==nil and io==nil and os==nil and debug==nil and setmetatable==nil); assert(math.floor(3.8)==3 and bit32.band(3,1)==1); soma=0; for _,v in ipairs({2,3}) do soma=soma+v end").Sucesso &&
            runner.ObterVariavelNumerica("soma") == 5, "Módulos removidos disponíveis ou módulos permitidos quebrados.");
        Exigir(runner.ExecutarCodigo("for i=1,120 do print(i); droid.listarTecnicas() end").Sucesso, "Saída finita.");
        Exigir(api.ObterLogDaSessao().Count == 100, "Quota de linhas não respeitada.");
        int avisos = 0;
        foreach (string linha in api.ObterLogDaSessao())
        {
            Exigir(linha.Length <= 512, "Quota de caracteres de saída.");
            if (linha.Contains("Saída truncada")) avisos++;
        }
        Exigir(avisos == 1, "Aviso de truncamento não foi único.");
        Exigir(runner.ExecutarCodigo("print('" + new string('x', 600) + "')").Sucesso, "Linha longa finita.");
        Exigir(api.ObterLogDaSessao().Count == 2 && api.ObterLogDaSessao()[0].Length == 512,
            "Linha longa não truncada no coletor compartilhado.");
        Exigir(runner.ExecutarCodigo("final=42").Sucesso && api.ObterLogDaSessao().Count == 0,
            "Saída não foi reiniciada para a próxima execução.");
        Exigir(runner.IniciarExecucao("--" + new string('x', 16384), new LimitesDeExecucao()) &&
            runner.EstadoAtual == EstadoExecucao.LimiteExcedido, "Fonte longa não recusada antes da compilação.");
        Exigir(runner.IniciarExecucao("MoonSharp_dump_b64::AAAA", new LimitesDeExecucao()) &&
            runner.EstadoAtual == EstadoExecucao.Falhou && !runner.UltimoResultado.ErroDeLua, "Bytecode não recusado como entrada conhecida.");
        int pontos = droid.Pontos.PontosDisponiveis;
        Exigir(!runner.ExecutarCodigo("droid.testeAdicionarPontos(1); error('apos comando')").Sucesso &&
            droid.Pontos.PontosDisponiveis == pontos + 1 && runner.UltimoResultado.MensagemErro.Contains("efeitos parciais"),
            "Limite de atomicidade existente não foi informado.");
        Exigir(runner.ExecutarCodigo("ok=42").Sucesso, "Recuperação final.");
        return "PASS: quotas finitas, cálculo/while/recursão, persistência, erros, cancelamento, concorrência, módulos, saída, fonte e efeitos parciais.";
    }

    static EstadoExecucao Drenar(DroidScriptRunner runner)
    {
        for (int passo = 0; passo < 250 && runner.EstadoAtual == EstadoExecucao.Executando; passo++) runner.AvancarExecucao();
        Exigir(runner.EstadoAtual != EstadoExecucao.Executando, "Fixture excedeu seu número finito de passos.");
        return runner.EstadoAtual;
    }

    /// <summary>API da UI em preview isolada, sem input físico, render, puzzles reais ou saves.</summary>
    public static string ExecutarUI()
    {
        Exigir(!EditorApplication.isPlaying, "Executar fora de Play.");
        const BindingFlags privado = BindingFlags.Instance | BindingFlags.NonPublic;
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        object anterior = singleton.GetValue(null);
        var cena = EditorSceneManager.NewPreviewScene();
        GameObject raiz = null;
        try
        {
            raiz = new GameObject("FixtureTerminalLua", typeof(RectTransform));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(raiz, cena);
            var estado = raiz.AddComponent<GerenciadorDeEstado>();
            singleton.SetValue(null, estado);
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", privado)
                .SetValue(estado, new Droid("FixtureTerminal", 30));
            var terminal = raiz.AddComponent<TerminalUIManager>();
            terminal.painelTerminal = new GameObject("Painel", typeof(RectTransform));
            terminal.painelTerminal.transform.SetParent(raiz.transform, false);
            var campoGo = new GameObject("Campo", typeof(RectTransform), typeof(TMP_InputField));
            campoGo.transform.SetParent(terminal.painelTerminal.transform, false);
            var campo = campoGo.GetComponent<TMP_InputField>();
            var executarGo = new GameObject("Executar", typeof(RectTransform), typeof(Button));
            executarGo.transform.SetParent(terminal.painelTerminal.transform, false);
            terminal.botaoExecutar = executarGo.GetComponent<Button>();
            terminal.mostrarDicasDoApollo = false;
            int eventos = 0;
            terminal.AoExecutarCodigo += () => eventos++;
            void AbrirSemFoco()
            {
                terminal.campoDeCodigo = null;
                terminal.Abrir();
                terminal.campoDeCodigo = campo;
            }
            void Executar(string codigo)
            {
                campo.text = codigo;
                terminal.botaoExecutar.onClick.Invoke();
            }
            void Frame() => typeof(TerminalUIManager).GetMethod("Update", privado).Invoke(terminal, null);
            AbrirSemFoco();
            Executar("fixture_lua=42");
            Exigir(terminal.Runner.EstadoAtual == EstadoExecucao.Executando && !terminal.botaoExecutar.interactable && eventos == 0,
                "UI executou no clique ou publicou evento antes de concluir.");
            Frame();
            Exigir(eventos == 1 && terminal.Runner.ObterVariavelNumerica("fixture_lua") == 42 && terminal.botaoExecutar.interactable,
                "UI não publicou exatamente um evento no sucesso.");
            Executar("fixture_lua=7; error('fixture')"); Frame();
            Exigir(eventos == 1 && terminal.Runner.ObterVariavelNumerica("fixture_lua") == null && campo.text.Contains("error"),
                "UI publicou vitória em erro ou apagou código.");
            const string finito = "fixture_lua=0; for i=1,100000 do fixture_lua=fixture_lua+1 end";
            Executar(finito);
            DroidScriptRunner emCurso = terminal.Runner;
            terminal.botaoExecutar.onClick.Invoke(); Frame();
            Exigir(emCurso.EstadoAtual == EstadoExecucao.Executando && ReferenceEquals(emCurso, terminal.Runner),
                "Duplo clique substituiu job ou UI drenou todas retomadas no frame.");
            terminal.Fechar();
            Exigir(emCurso.EstadoAtual == EstadoExecucao.Cancelada && !terminal.EstaAberto && !estado.MenuAberto && eventos == 1 && campo.text == finito,
                "Fechar não cancelou/preservou código/contador.");
            AbrirSemFoco(); Executar("fixture_lua=42"); Frame();
            Exigir(eventos == 2 && terminal.Runner.ObterVariavelNumerica("fixture_lua") == 42,
                "Reabrir não recuperou execução curta.");
            Executar(finito); Frame(); emCurso = terminal.Runner;
            terminal.DefinirContextoPuzzle(CatalogoDePuzzles.IdAguaDaCasa);
            Exigir(emCurso.EstadoAtual == EstadoExecucao.Cancelada && !ReferenceEquals(emCurso, terminal.Runner) && eventos == 2,
                "Troca de contexto não descartou job antigo.");
            Executar(finito); Frame(); emCurso = terminal.Runner;
            terminal.LimparContextoPuzzle(CatalogoDePuzzles.IdAguaDaCasa);
            Exigir(emCurso.EstadoAtual == EstadoExecucao.Cancelada && eventos == 2, "Limpar contexto não cancelou.");
            Executar(finito); Frame(); emCurso = terminal.Runner;
            raiz.SetActive(false);
            // Em Edit Mode, MonoBehaviour não depende do ciclo de callbacks de Play.
            typeof(TerminalUIManager).GetMethod("OnDisable", privado).Invoke(terminal, null);
            Exigir(emCurso.EstadoAtual == EstadoExecucao.Cancelada && !terminal.EstaAberto && !estado.MenuAberto && eventos == 2,
                "OnDisable não cancelou ou deixou menu aberto.");
            raiz.SetActive(true); AbrirSemFoco(); Executar(finito);
            for (int frame = 0; frame < 210 && terminal.Runner.EstadoAtual == EstadoExecucao.Executando; frame++) Frame();
            Exigir(terminal.Runner.EstadoAtual == EstadoExecucao.LimiteExcedido && eventos == 2 && campo.text == finito,
                "UI publicou vitória em limite ou apagou código.");
            Executar("fixture_lua=42"); Frame();
            Exigir(eventos == 3 && terminal.botaoExecutar.interactable, "UI não recuperou após limite.");
            terminal.Fechar();
            return "PASS UI/API: um Resume/frame, sucesso único, erro/limite sem evento, duplo clique, fechar/reabrir, contexto e OnDisable. Input físico e visual pendentes.";
        }
        finally
        {
            if (raiz != null) UnityEngine.Object.DestroyImmediate(raiz);
            singleton.SetValue(null, anterior);
            EditorSceneManager.ClosePreviewScene(cena);
        }
    }
}
