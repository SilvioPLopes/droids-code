using System;
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
            if (!TentarCalcularCusto(out int custo))
                throw new InvalidOperationException("Não foi possível calcular um custo válido para a técnica.");
            return custo;
        }

        public bool TentarCalcularCusto(out int custo) =>
            TentarCalcularCusto(NivelDeDano, out custo);

        public bool TentarCalcularCusto(int nivelDeDano, out int custo)
        {
            custo = 0;
            if (nivelDeDano < 0 || EfeitosDeAtributo == null || EfeitosDeDanoPorTurno == null)
                return false;

            try
            {
                long total = checked((long)nivelDeDano * TabelaDeCustos.CustoDanoPorNivel);

                foreach (EfeitoDeAtributo efeito in EfeitosDeAtributo)
                {
                    if (efeito == null || string.IsNullOrEmpty(efeito.NomeExibicao) ||
                        efeito.DuracaoEmTurnos <= 0 ||
                        !TabelaDeCustos.CustoPorTurnoDeEfeito.TryGetValue(efeito.NomeExibicao, out int custoPorTurno) ||
                        custoPorTurno < 0)
                        return false;

                    total = checked(total + checked((long)custoPorTurno * efeito.DuracaoEmTurnos));
                }

                foreach (EfeitoDeDanoPorTurno efeito in EfeitosDeDanoPorTurno)
                {
                    if (efeito == null || string.IsNullOrEmpty(efeito.NomeExibicao) ||
                        efeito.DuracaoEmTurnos <= 0 || efeito.DanoPorTurno <= 0 ||
                        !TabelaDeCustos.CustoPorTurnoDeEfeito.TryGetValue(efeito.NomeExibicao, out int custoPorTurno) ||
                        custoPorTurno < 0)
                        return false;

                    total = checked(total + checked((long)custoPorTurno * efeito.DuracaoEmTurnos));
                }

                if (total < 0 || total > int.MaxValue)
                    return false;

                custo = (int)total;
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }

    // Contrato minimo compartilhado pelos dois tipos de efeito ativo, so pra
    // permitir um unico metodo generico de decremento/expiracao (ver
    // Droid.DecrementarEfeitosAtivos / InimigoFixo.DecrementarEfeitosAtivos).
    public interface IEfeitoTemporario
    {
        int DuracaoEmTurnos { get; set; }
    }

    // Equivalente ao ActiveBuff do projeto de referencia (Ragnarok Core).
    // -1 turnos == permanente (nao usado nesta fase, mantido por consistencia).
    public class EfeitoDeAtributo : IEfeitoTemporario
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

            return checked(custoPorTurno * DuracaoEmTurnos);
        }
    }

    // Dano continuo por turno (ex: Envenenamento) — divergencia consciente do
    // ActiveBuff de referencia, modelado como tipo proprio.
    public class EfeitoDeDanoPorTurno : IEfeitoTemporario
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
            return checked(custoPorTurno * DuracaoEmTurnos);
        }
    }
}
