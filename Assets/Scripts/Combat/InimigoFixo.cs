using System.Collections.Generic;
using DroidsCode.DroidCore;

namespace DroidsCode.Combat
{
    public class InimigoFixo : IParticipanteDeCombate
    {
        private const string NomeAcaoUnica = "Atacar";
        private readonly int _ataque;

        public string Nome { get; }
        public int Hp { get; set; }
        public int HpMax { get; }
        public int Defesa { get; }

        // Nao vai na interface IParticipanteDeCombate de proposito — so o
        // BattleManager referencia InimigoFixo diretamente, nao precisa
        // vazar isso pro contrato generico.
        public int RecompensaXp { get; }

        // Motor de efeitos (Stun/Envenenamento) — mesmo contrato do Droid,
        // mas sem buff de atributo: InimigoFixo nao tem atributos
        // configuraveis, so stats fixos definidos no construtor.
        public List<EfeitoDeAtributo> EfeitosAtivos { get; } = new List<EfeitoDeAtributo>();
        public List<EfeitoDeDanoPorTurno> EfeitosDeDanoAtivos { get; } = new List<EfeitoDeDanoPorTurno>();

        public InimigoFixo(string nome, int hpMax, int ataque, int defesa, int recompensaXp)
        {
            Nome = nome;
            HpMax = hpMax;
            Hp = hpMax;
            _ataque = ataque;
            Defesa = defesa;
            RecompensaXp = recompensaXp;
        }

        public IEnumerable<string> ObterAcoesDisponiveis()
        {
            yield return NomeAcaoUnica;
        }

        public ResultadoAcao ExecutarAcao(string nomeAcao, IParticipanteDeCombate alvo)
        {
            int dano = System.Math.Max(1, _ataque - alvo.Defesa);
            return new ResultadoAcao
            {
                Sucesso = true,
                DanoCausado = dano,
                Mensagem = $"{Nome} atacou! Causou {dano} de dano."
            };
        }

        public void DecrementarEfeitosAtivos()
        {
            EfeitosTemporariosUtil.Decrementar(EfeitosAtivos);
            EfeitosTemporariosUtil.Decrementar(EfeitosDeDanoAtivos);
        }
    }
}
