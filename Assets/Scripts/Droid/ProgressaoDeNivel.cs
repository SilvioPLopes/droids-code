namespace DroidsCode.DroidCore
{
    // Separado de PontosDeProgressao de proposito (SRP): aquela e orcamento
    // de pontos gasto no terminal; esta e nivel/XP ganhos em combate.
    public class ProgressaoDeNivel
    {
        public int Nivel { get; private set; } = 1;
        public int ExperienciaAtual { get; private set; } = 0;

        // [DEFAULT] mesma curva do LevelingService.java de referencia
        // (Nivel * 100). Placeholder de balanceamento, nao final.
        public int ExperienciaNecessariaProximoNivel => Nivel * 100;

        // Retorna quantos niveis subiram nessa chamada (0 se nenhum).
        public int GanharExperiencia(int xpGanho)
        {
            ExperienciaAtual += xpGanho;
            int niveisGanhos = 0;

            while (ExperienciaAtual >= ExperienciaNecessariaProximoNivel)
            {
                ExperienciaAtual -= ExperienciaNecessariaProximoNivel;
                Nivel++;
                niveisGanhos++;
            }

            return niveisGanhos;
        }

        // Uso exclusivo do sistema de salvamento, pra restaurar valor exato.
        public void Definir(int nivel, int experiencia)
        {
            Nivel = nivel < 1 ? 1 : nivel;
            ExperienciaAtual = experiencia < 0 ? 0 : experiencia;
        }
    }
}
