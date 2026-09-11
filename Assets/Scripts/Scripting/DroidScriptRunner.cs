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
    }

    public class ResultadoExecucao
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
    }
}
