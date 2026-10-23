using MoonSharp.Interpreter;

namespace DroidsCode.Scripting
{
    public class DroidScriptRunner
    {
        private readonly Script _vm;

        public DroidScriptRunner(TerminalDroidApi api)
        {
            // Sandbox: remove io, os (exceto tempo), load, require, dofile.
            // O Lua digitado pelo jogador não deveria ter acesso a sistema de
            // arquivos/SO — sem isso, new Script() carrega todos os módulos
            // padrão, incluindo esses, o que é uma porta aberta desnecessária
            // num terminal exposto ao jogador dentro do jogo.
            _vm = new Script(CoreModules.Preset_SoftSandbox);

            UserData.RegisterType<TerminalDroidApi>();
            // So a fachada e exposta — Droid.cs nunca e registrado diretamente.
            _vm.Globals["droid"] = api;
        }

        public ResultadoExecucao ExecutarCodigo(string codigoLua)
        {
            // TODO: sem protecao contra loop infinito ainda (ex: "while true do end"
            // travaria o jogo). Pendência conhecida, fora do escopo desta correção.
            try
            {
                _vm.DoString(codigoLua);
                return new ResultadoExecucao { Sucesso = true, MensagemErro = null };
            }
            catch (ScriptRuntimeException ex)
            {
                return new ResultadoExecucao { Sucesso = false, MensagemErro = ex.DecoratedMessage };
            }
            catch (SyntaxErrorException ex)
            {
                return new ResultadoExecucao { Sucesso = false, MensagemErro = ex.DecoratedMessage };
            }
        }

        // ADICIONADO (cena de abertura — puzzle de casa, 14/09/2026): leitura
        // pura de uma variável global Lua depois da execução, usada pra
        // avaliar puzzles que não passam por droid.* nenhum (ex: "nivel_agua"
        // da Sessão 1 do Enredo). Retorna null se a variável não existir
        // ainda ou não for numérica — quem chama decide o que fazer com isso
        // (ver VerificadorDePuzzle.VerificarVitoria / PuzzleAguaCasaChecker).
        public double? ObterVariavelNumerica(string nomeDaVariavel)
        {
            DynValue valor = _vm.Globals.Get(nomeDaVariavel);

            // REMOVIDO (15/09/2026): o Debug.Log temporario que imprimia todas
            // as chaves globais do Lua a cada leitura. O bug que motivou ele
            // (puzzle da agua) esta confirmado corrigido e testado desde
            // 14/09; o log so poluia o Console a cada execucao de codigo --
            // item 7 dos "Proximos passos imediatos" do CHECKLIST.
            // Quem precisar de depuracao de puzzle agora tem o campo
            // "logDeDepuracao" no VerificadorDePuzzle, que e ligavel por
            // instancia no Inspector em vez de global e sempre ligado.

            if (valor.Type != DataType.Number)
            {
                return null;
            }
            return valor.Number;
        }
    }

    public class ResultadoExecucao
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
    }
}
