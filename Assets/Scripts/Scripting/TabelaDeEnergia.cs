using System;

namespace DroidsCode.DroidCore
{
    /// <summary>Balanceamento inicial de combate. Independente do custo de programação.</summary>
    public static class TabelaDeEnergia
    {
        public const int MaximaInicial = 100;
        public const int RecuperacaoAtaqueBasico = 10;

        public static int CalcularCusto(TecnicaComposta tecnica)
        {
            if (tecnica == null) return 0;
            // long evita overflow para valores extremos vindos de técnicas/saves.
            long custo = 10L * Math.Max(1, tecnica.NivelDeDano);
            if (custo >= MaximaInicial) return MaximaInicial;
            foreach (var efeito in tecnica.EfeitosDeAtributo)
            {
                long faixas = Math.Max(1, (Math.Abs((long)efeito.Valor) + 4) / 5);
                custo += 5L * faixas * Math.Max(1, efeito.DuracaoEmTurnos);
                if (custo >= MaximaInicial) return MaximaInicial;
            }
            foreach (var efeito in tecnica.EfeitosDeDanoPorTurno)
            {
                custo += (long)Math.Max(1, efeito.DanoPorTurno) * Math.Max(1, efeito.DuracaoEmTurnos);
                if (custo >= MaximaInicial) return MaximaInicial;
            }
            return (int)Math.Min(MaximaInicial, custo);
        }
    }
}
