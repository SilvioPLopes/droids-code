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
            // Delega para o participante. Nao chama Lua aqui — o combate so
            // le dado ja configurado no terminal (ver CORRECAO_ARQUITETURA_TERMINAL_MENU.md).
            ResultadoAcao resultado = atacante.ExecutarAcao(nomeAcao, alvo);

            if (resultado.Sucesso)
            {
                alvo.Hp = System.Math.Max(0, alvo.Hp - resultado.DanoCausado);
            }

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
    }
}
