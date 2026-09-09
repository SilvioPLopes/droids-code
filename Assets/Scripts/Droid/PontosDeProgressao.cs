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
    }
}
