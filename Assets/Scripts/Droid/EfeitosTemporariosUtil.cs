using System.Collections.Generic;

namespace DroidsCode.DroidCore
{
    // Logica de decremento/expiracao compartilhada entre Droid.EfeitosAtivos e
    // InimigoFixo.EfeitosAtivos -- evita duplicar o mesmo loop nos dois
    // participantes de combate. Puro C#, sem UnityEngine (mesma regra das
    // outras classes de dominio, ver secao 7 da documentacao tecnica).
    public static class EfeitosTemporariosUtil
    {
        public static void Decrementar<T>(List<T> lista) where T : IEfeitoTemporario
        {
            for (int i = lista.Count - 1; i >= 0; i--)
            {
                if (lista[i].DuracaoEmTurnos < 0)
                {
                    continue; // permanente
                }

                lista[i].DuracaoEmTurnos -= 1;

                if (lista[i].DuracaoEmTurnos <= 0)
                {
                    lista.RemoveAt(i);
                }
            }
        }
    }
}
