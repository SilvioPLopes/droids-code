using System;

namespace DroidsCode.Scripting
{
    public enum EstadoExecucao { Executando, Concluida, Falhou, Cancelada, LimiteExcedido }

    // Orçamentos iniciais; não são uma medição de performance nem instruções úteis exatas.
    public sealed class LimitesDeExecucao
    {
        public int MaximoDeCaracteres { get; set; } = 16384;
        public int InstrucoesPorRetomada { get; set; } = 1000;
        public int MaximoDeRetomadas { get; set; } = 200;
        public int MaximoDeLinhasDeSaida { get; set; } = 100;
        public int MaximoDeCaracteresPorLinha { get; set; } = 512;

        internal LimitesDeExecucao CopiarValidado()
        {
            if (MaximoDeCaracteres <= 0 || InstrucoesPorRetomada <= 0 || MaximoDeRetomadas <= 0 ||
                MaximoDeLinhasDeSaida <= 0 || MaximoDeCaracteresPorLinha <= 0)
                throw new ArgumentOutOfRangeException(nameof(LimitesDeExecucao), "Todos os limites devem ser positivos.");
            return (LimitesDeExecucao)MemberwiseClone();
        }
    }
}
