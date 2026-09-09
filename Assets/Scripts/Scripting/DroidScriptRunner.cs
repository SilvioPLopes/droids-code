using MoonSharp.Interpreter;

namespace DroidsCode.Scripting
{
    public class DroidScriptRunner
    {
        private readonly Script _vm;

        public DroidScriptRunner(TerminalDroidApi api)
        {
            _vm = new Script();

            UserData.RegisterType<TerminalDroidApi>();
            // So a fachada e exposta — Droid.cs nunca e registrado diretamente.
            _vm.Globals["droid"] = api;
        }

        public ResultadoExecucao ExecutarCodigo(string codigoLua)
        {
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
    }

    public class ResultadoExecucao
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
    }
}
