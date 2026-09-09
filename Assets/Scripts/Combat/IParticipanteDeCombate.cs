using System.Collections.Generic;

namespace DroidsCode.Combat
{
    public interface IParticipanteDeCombate
    {
        string Nome { get; }
        int Hp { get; set; }
        int HpMax { get; }

        // Adicionado para viabilizar a formula de dano (ver BattleEngine.java
        // de referencia: dano = max(1, totalAtk - defesaAlvo)). Sem isso o
        // CombatEngine nao tem como saber a defesa do alvo.
        int Defesa { get; }

        IEnumerable<string> ObterAcoesDisponiveis();

        ResultadoAcao ExecutarAcao(string nomeAcao, IParticipanteDeCombate alvo);
    }
}
