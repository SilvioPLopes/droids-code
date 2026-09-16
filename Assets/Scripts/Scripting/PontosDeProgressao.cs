namespace DroidsCode.DroidCore
{
    public class PontosDeProgressao
    {
        // PLACEHOLDER — valor de rascunho, nao balanceado.
        public const int PontosPorNivel = 5;

        public int PontosDisponiveis { get; private set; }

        public void GanharPontosPorNivel(int niveis = 1)
        {
            PontosDisponiveis += PontosPorNivel * niveis;
        }

        public bool TentarGastar(int quantidade)
        {
            if (quantidade > PontosDisponiveis)
            {
                return false;
            }

            PontosDisponiveis -= quantidade;
            return true;
        }

        // Usado exclusivamente pelo sistema de salvamento pra restaurar o
        // valor exato salvo — nao usar em fluxo normal de jogo (esse usa
        // GanharPontosPorNivel/TentarGastar).
        public void DefinirPontos(int quantidade)
        {
            PontosDisponiveis = quantidade < 0 ? 0 : quantidade;
        }
    }
}
