using System.Collections.Generic;
using DroidsCode.DroidCore;

namespace DroidsCode.Combat
{
    /// <summary>Máquina de contenção: carga anunciada, corte pesado e janela de descompressão.</summary>
    public sealed class DroidDoIvo : InimigoFixo
    {
        public const string Prensa = "Prensa Hidráulica";
        public const string Carga = "Pressurizar";
        public const string Corte = "Corte de Contenção";
        public const string Descanso = "Descompressão";
        static readonly string[] Ciclo = { Prensa, Carga, Corte, Descanso };
        readonly int ataque;
        int etapa;

        public string ProximaAcao => Ciclo[etapa];

        public DroidDoIvo(DefinicaoDeInimigo definicao)
            : base(definicao.Nome, definicao.HpMax, definicao.Ataque, definicao.Defesa, definicao.RecompensaXp)
        {
            ataque = definicao.Ataque;
            Hit = definicao.Hit;
            Flee = definicao.Flee;
            ChanceCritica = definicao.ChanceCritica;
            ChanceDeResistirEfeito = definicao.ChanceDeResistirEfeito;
        }

        // BattleManager escolhe a primeira ação. Só a intenção atual está disponível.
        public override IEnumerable<string> ObterAcoesDisponiveis()
        {
            yield return ProximaAcao;
        }

        public override ResultadoAcao ExecutarAcao(string nomeAcao, IParticipanteDeCombate alvo)
        {
            if (nomeAcao != ProximaAcao || alvo == null)
                return new ResultadoAcao { Sucesso = false, Mensagem = "Ação do boss indisponível." };

            ResultadoAcao resultado;
            if (nomeAcao == Carga || nomeAcao == Descanso)
            {
                resultado = new ResultadoAcao
                {
                    Sucesso = true,
                    DanoCausado = 0,
                    Mensagem = nomeAcao == Carga
                        ? "O Droid pressuriza os braços: o próximo corte será pesado!"
                        : "O Droid descomprime os pistões e fica sem atacar neste turno."
                };
            }
            else
            {
                resultado = Golpear(nomeAcao, alvo);
            }

            // CombatEngine não chama ExecutarAcao sob stun/morte: não consome a carga.
            // Um golpe que erra consome a ação normalmente, preservando a janela seguinte.
            etapa = (etapa + 1) % Ciclo.Length;
            resultado.Mensagem += " Próxima ação: " + ProximaAcao + ".";
            return resultado;
        }

        ResultadoAcao Golpear(string nomeAcao, IParticipanteDeCombate alvo)
        {
            if (!Droid.RolarAcerto(Hit, alvo.Flee))
                return new ResultadoAcao { Sucesso = false, Mensagem = Nome + " usou " + nomeAcao + ", mas errou!" };

            int dano = System.Math.Max(1, ataque + (nomeAcao == Corte ? 12 : 0) - alvo.Defesa);
            bool critico = Droid.RolarCritico(ChanceCritica);
            if (critico) dano = (int)System.Math.Round(dano * TabelaDeCombate.MultiplicadorDano_Critico);
            string efeito = "";
            if (nomeAcao == Corte)
            {
                if (Droid.RolarResistencia(alvo.ChanceDeResistirEfeito))
                    efeito = " O alvo resistiu ao vazamento.";
                else
                {
                    alvo.EfeitosDeDanoAtivos.Add(new EfeitoDeDanoPorTurno
                    {
                        NomeExibicao = "Vazamento", DanoPorTurno = 2, DuracaoEmTurnos = 2
                    });
                    efeito = " Vazamento: 2 de dano por 2 turnos.";
                }
            }
            return new ResultadoAcao
            {
                Sucesso = true, DanoCausado = dano,
                Mensagem = Nome + " usou " + nomeAcao + (critico ? "! CRÍTICO!" : "!") + " Causou " + dano + " de dano." + efeito
            };
        }
    }
}
