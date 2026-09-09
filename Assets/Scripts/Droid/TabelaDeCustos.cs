using System.Collections.Generic;

namespace DroidsCode.DroidCore
{
    // PLACEHOLDER — valores de rascunho, nao balanceados. Nao ajustar por
    // conta propria (decisao de game design via playtesting).
    public static class TabelaDeCustos
    {
        public const int CustoUpgradeAtributo = 1; // por ponto de FOR/AGI/VIT/INT/DEX/LUK

        public const int CustoDanoPorNivel = 1;
        public const int CustoResistenciaPorNivel = 1;

        public static readonly Dictionary<string, int> CustoPorTurnoDeEfeito = new Dictionary<string, int>
        {
            { "Stun", 3 },          // por turno: 1 turno = 3, 2 turnos = 6
            { "Envenenamento", 2 }  // TODO: confirmar se escala por turno ou e fixo
        };
    }
}
