using System.Collections.Generic;
using DroidsCode.DroidCore;

namespace DroidsCode.Combat
{
    public class CombatEngine
    {
        public ResultadoAcao ExecutarTurno(
            IParticipanteDeCombate atacante,
            IParticipanteDeCombate alvo,
            string nomeAcao)
        {
            // Cada evento do turno vira uma linha de log -- juntas no final
            // em resultado.Mensagem, separadas por \n (quem exibe, ex.
            // BattleManager, decide se mostra como linhas separadas).
            var mensagens = new List<string>();

            // Veneno (ou qualquer EfeitoDeDanoPorTurno) causa dano no INICIO
            // do turno de quem esta afetado, antes de decidir se ele age.
            AplicarDanoPorTurno(atacante, mensagens);

            ResultadoAcao resultado;

            if (atacante.Hp <= 0)
            {
                // Morreu pelo proprio dano por turno (ex: veneno) antes de agir.
                resultado = new ResultadoAcao
                {
                    Sucesso = false,
                    DanoCausado = 0,
                    Mensagem = $"{atacante.Nome} sucumbiu a um efeito ativo antes de agir."
                };
            }
            else if (EstaAtordoado(atacante))
            {
                resultado = new ResultadoAcao
                {
                    Sucesso = false,
                    DanoCausado = 0,
                    Mensagem = $"{atacante.Nome} está atordoado e perde o turno!"
                };
            }
            else
            {
                // Delega para o participante. Nao chama Lua aqui — o combate so
                // le dado ja configurado no terminal (ver CORRECAO_ARQUITETURA_TERMINAL_MENU.md).
                resultado = atacante.ExecutarAcao(nomeAcao, alvo);

                if (resultado.Sucesso)
                {
                    alvo.Hp = System.Math.Max(0, alvo.Hp - resultado.DanoCausado);
                }
            }

            mensagens.Add(resultado.Mensagem);
            atacante.DecrementarEfeitosAtivos();

            resultado.Mensagem = string.Join("\n", mensagens);
            return resultado;
        }

        public bool VerificarDerrota(IParticipanteDeCombate participante)
        {
            if (participante.Hp <= 0)
            {
                return true;
            }

            // TODO: condicao de derrota por ausencia de metodo essencial ainda
            // pendente — quais acoes sao obrigatorias por fase. Nao decidido aqui.
            return false;
        }

        private static bool EstaAtordoado(IParticipanteDeCombate participante)
        {
            return participante.EfeitosAtivos.Exists(
                efeito => efeito.NomeExibicao == "Stun" && efeito.DuracaoEmTurnos > 0);
        }

        private static void AplicarDanoPorTurno(IParticipanteDeCombate participante, List<string> mensagens)
        {
            foreach (EfeitoDeDanoPorTurno efeito in participante.EfeitosDeDanoAtivos)
            {
                if (efeito.DuracaoEmTurnos > 0)
                {
                    participante.Hp = System.Math.Max(0, participante.Hp - efeito.DanoPorTurno);
                    mensagens.Add(
                        $"{participante.Nome} sofreu {efeito.DanoPorTurno} de dano de {efeito.NomeExibicao} " +
                        $"({efeito.DuracaoEmTurnos} turno(s) restante(s)).");
                }
            }
        }
    }
}
