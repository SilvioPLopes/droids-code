using System.Collections.Generic;

namespace DroidsCode.DroidCore
{
    public class TecnicaComposta
    {
        public string Nome { get; set; }
        public int NivelDeDano { get; set; }
        public List<EfeitoDeAtributo> EfeitosDeAtributo { get; set; } = new List<EfeitoDeAtributo>();
        public List<EfeitoDeDanoPorTurno> EfeitosDeDanoPorTurno { get; set; } = new List<EfeitoDeDanoPorTurno>();

        public int CustoTotal()
        {
            int custo = NivelDeDano * TabelaDeCustos.CustoDanoPorNivel;

            foreach (EfeitoDeAtributo efeito in EfeitosDeAtributo)
            {
                custo += efeito.CalcularCusto();
            }

            foreach (EfeitoDeDanoPorTurno efeito in EfeitosDeDanoPorTurno)
            {
                custo += efeito.CalcularCusto();
            }

            return custo;
        }
    }

    // Equivalente ao ActiveBuff do projeto de referencia (Ragnarok Core).
    // -1 turnos == permanente (nao usado nesta fase, mantido por consistencia).
    public class EfeitoDeAtributo
    {
        public string NomeExibicao { get; set; } // ex: "Stun"
        public TipoAtributo Atributo { get; set; }
        public int Valor { get; set; }
        public int DuracaoEmTurnos { get; set; }

        public int CalcularCusto()
        {
            if (!TabelaDeCustos.CustoPorTurnoDeEfeito.TryGetValue(NomeExibicao, out int custoPorTurno))
            {
                return 0; // TODO: decidir se efeito desconhecido na tabela deveria ser erro
            }

            return custoPorTurno * DuracaoEmTurnos;
        }
    }

    // Dano continuo por turno (ex: Envenenamento) — divergencia consciente do
    // ActiveBuff de referencia, modelado como tipo proprio.
    public class EfeitoDeDanoPorTurno
    {
        public string NomeExibicao { get; set; } // ex: "Envenenamento"
        public int DanoPorTurno { get; set; }
        public int DuracaoEmTurnos { get; set; }

        public int CalcularCusto()
        {
            if (!TabelaDeCustos.CustoPorTurnoDeEfeito.TryGetValue(NomeExibicao, out int custoPorTurno))
            {
                return 0;
            }

            // TODO: nao confirmado se Envenenamento escala por turno como Stun.
            return custoPorTurno * DuracaoEmTurnos;
        }
    }
}
