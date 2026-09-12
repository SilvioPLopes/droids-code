using System.Collections.Generic;
using DroidsCode.DroidCore;

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

        // Motor de efeitos por turno (Stun, Envenenamento, buffs/debuffs de
        // item ou tecnica). Listas mutaveis de proposito -- quem aplica um
        // efeito (ex: Droid.ExecutarAcao) adiciona direto em alvo.EfeitosAtivos.
        List<EfeitoDeAtributo> EfeitosAtivos { get; }
        List<EfeitoDeDanoPorTurno> EfeitosDeDanoAtivos { get; }
        void DecrementarEfeitosAtivos();

        IEnumerable<string> ObterAcoesDisponiveis();

        ResultadoAcao ExecutarAcao(string nomeAcao, IParticipanteDeCombate alvo);
    }
}
