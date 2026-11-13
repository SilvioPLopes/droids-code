using System;
using MoonSharp.Interpreter;

namespace DroidsCode.Scripting
{
    public class DroidScriptRunner
    {
        private Script _vm;
        private Coroutine _coroutine;
        private readonly TerminalDroidApi _api;
        private LimitesDeExecucao _limites;
        private int _retomadas;
        private bool _cicloEquipamentosAberto;

        public EstadoExecucao EstadoAtual { get; private set; } = EstadoExecucao.Concluida;
        public ResultadoExecucao UltimoResultado { get; private set; }
        public const string MensagemLimiteInstrucoes = "Execução interrompida: limite de instruções atingido. Confira a condição de parada e o incremento do laço. As variáveis desta sessão foram reiniciadas. Você pode editar e executar novamente.";

        public DroidScriptRunner(TerminalDroidApi api)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            UserData.RegisterType<TerminalDroidApi>();
        }

        private void CriarVm()
        {
            // Somente módulos sem execução reentrante de callbacks Lua sem yield.
            _vm = new Script(CoreModules.Basic | CoreModules.GlobalConsts |
                CoreModules.TableIterators | CoreModules.Math | CoreModules.Bit32);
            _vm.Options.DebugPrint = _api.RegistrarSaida;
            _vm.Globals["droid"] = _api;
        }

        // false significa concorrência recusada: o job e seu resultado são preservados.
        public bool IniciarExecucao(string codigo, LimitesDeExecucao limites)
        {
            if (EstadoAtual == EstadoExecucao.Executando) return false;
            _limites = (limites ?? new LimitesDeExecucao()).CopiarValidado();
            _api.ConfigurarSaida(_limites);
            _retomadas = 0;
            UltimoResultado = null;
            EstadoAtual = EstadoExecucao.Executando;
            codigo = codigo ?? "";
            if (codigo.Length > _limites.MaximoDeCaracteres)
            {
                Terminar(EstadoExecucao.LimiteExcedido, "Execução interrompida: limite de tamanho do código atingido. As variáveis desta sessão foram reiniciadas. Reduza o código e execute novamente.");
                return true;
            }
            if (codigo.StartsWith("MoonSharp_dump_b64::", StringComparison.Ordinal))
            {
                Terminar(EstadoExecucao.Falhou, "O terminal aceita somente código-fonte Lua, não bytecode.");
                return true;
            }
            try
            {
                if (_vm == null) CriarVm();
                _api.IniciarExecucaoEquipamentos();
                _cicloEquipamentosAberto = true;
                DynValue funcao = _vm.LoadString(codigo);
                _coroutine = _vm.CreateCoroutine(funcao).Coroutine;
                _coroutine.AutoYieldCounter = _limites.InstrucoesPorRetomada;
            }
            catch (InterpreterException ex) { Terminar(EstadoExecucao.Falhou, ex.DecoratedMessage, true); }
            catch (Exception ex) { Terminar(EstadoExecucao.Falhou, "Falha ao iniciar execução: " + ex.Message); }
            return true;
        }

        // Cada chamada realiza no máximo um Resume, sempre no thread do Unity.
        public EstadoExecucao AvancarExecucao()
        {
            if (EstadoAtual != EstadoExecucao.Executando) return EstadoAtual;
            try
            {
                _retomadas++;
                _coroutine.Resume();
                if (_coroutine.State == CoroutineState.Dead)
                    Terminar(EstadoExecucao.Concluida, null);
                else if (_coroutine.State != CoroutineState.ForceSuspended)
                    Terminar(EstadoExecucao.Falhou, "Execução suspensa em estado não permitido.");
                else if (_retomadas >= _limites.MaximoDeRetomadas)
                    Terminar(EstadoExecucao.LimiteExcedido, MensagemLimiteInstrucoes);
            }
            catch (InterpreterException ex) { Terminar(EstadoExecucao.Falhou, ex.DecoratedMessage, true); }
            catch (Exception ex) { Terminar(EstadoExecucao.Falhou, "Falha durante execução: " + ex.Message); }
            return EstadoAtual;
        }

        public void CancelarExecucao()
        {
            if (EstadoAtual == EstadoExecucao.Executando)
                Terminar(EstadoExecucao.Cancelada, "Execução cancelada. As variáveis desta sessão foram reiniciadas. Você pode editar e executar novamente.");
        }

        private void Terminar(EstadoExecucao estado, string mensagem, bool erroDeLua = false)
        {
            bool sucesso = estado == EstadoExecucao.Concluida;
            try
            {
                if (_cicloEquipamentosAberto)
                {
                    _cicloEquipamentosAberto = false;
                    _api.ConcluirExecucaoEquipamentos(sucesso);
                }
            }
            finally
            {
                _coroutine = null;
                if (!sucesso) _vm = null;
                EstadoAtual = estado;
                if (!sucesso && _api.HouveAlteracoesNoDroid)
                    mensagem += " Comandos já aplicados ao Droid podem ter efeitos parciais em atributos, pontos e técnicas; não há reversão integral dessas alterações.";
                UltimoResultado = new ResultadoExecucao
                {
                    Sucesso = sucesso, MensagemErro = mensagem, Estado = estado, ErroDeLua = erroDeLua
                };
            }
        }

        // Compatibilidade para fixtures/consumidores síncronos; usa as mesmas quotas.
        public ResultadoExecucao ExecutarCodigo(string codigoLua)
        {
            if (!IniciarExecucao(codigoLua, new LimitesDeExecucao()))
                return new ResultadoExecucao { Sucesso = false, Estado = EstadoExecucao.Executando, MensagemErro = "Já existe uma execução em andamento." };
            while (EstadoAtual == EstadoExecucao.Executando) AvancarExecucao();
            return UltimoResultado;
        }

        public double? ObterVariavelNumerica(string nomeDaVariavel)
        {
            // Não expor atribuições parciais aos verificadores de vitória.
            if (_vm == null || EstadoAtual != EstadoExecucao.Concluida) return null;
            DynValue valor = _vm.Globals.Get(nomeDaVariavel);
            return valor.Type == DataType.Number ? (double?)valor.Number : null;
        }
    }

    public class ResultadoExecucao
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
        public EstadoExecucao Estado { get; set; }
        public bool ErroDeLua { get; set; }
    }
}
