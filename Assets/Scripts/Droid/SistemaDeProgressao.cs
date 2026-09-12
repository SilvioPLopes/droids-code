namespace DroidsCode.DroidCore
{
    // Servico de dominio (nao MonoBehaviour). Equivalente ao LevelingService.java
    // de referencia: recebe XP, decide se sobe nivel, concede pontos se sim.
    public static class SistemaDeProgressao
    {
        public static void GanharExperiencia(Droid droid, int xpGanho)
        {
            int niveisGanhos = droid.Progressao.GanharExperiencia(xpGanho);

            if (niveisGanhos > 0)
            {
                droid.Pontos.GanharPontosPorNivel(niveisGanhos);
            }
        }
    }
}
