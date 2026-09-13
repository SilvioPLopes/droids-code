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

        // Estagio 1 (12/09/2026): InimigoFixo nao tem atributos configuraveis
        // (DEX/AGI/LUK/INT), entao usa valores FIXOS vindos de
        // TabelaDeCombate.*PadraoInimigo. Isso e proposital e temporario --
        // ver CHECKLIST_DE_DESENVOLVIMENTO.md ("Mais de um tipo de inimigo").
        // O objetivo aqui e so garantir que InimigoFixo respeita os MESMOS
        // calculos (Droid.RolarAcerto/RolarCritico) que o jogador, pra nao
        // precisar de logica de combate duplicada quando inimigos variados
        // chegarem.
        public int Hit { get; set; } = TabelaDeCombate.HitPadraoInimigo;
        public int Flee { get; set; } = TabelaDeCombate.FleePadraoInimigo;
        public float ChanceCritica { get; set; } = TabelaDeCombate.ChanceCriticoPadraoInimigo;
        public float ChanceDeResistirEfeito { get; set; } = TabelaDeCombate.ResistenciaEfeitoPadraoInimigo;
        public float BonusPercentualDeCura { get; set; } = 0f; // inimigo nao usa item

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
            // Estagio 1: mesmo roll de acerto que o Droid usa (Hit do inimigo
            // vs Flee do alvo) -- ver Droid.RolarAcerto.
            if (!DroidsCode.DroidCore.Droid.RolarAcerto(Hit, alvo.Flee))
            {
                return new ResultadoAcao
                {
                    Sucesso = false,
                    DanoCausado = 0,
                    Mensagem = $"{Nome} atacou, mas {alvo.Nome} esquivou!"
                };
            }

            int dano = System.Math.Max(1, _ataque - alvo.Defesa);

            bool critico = DroidsCode.DroidCore.Droid.RolarCritico(ChanceCritica);
            if (critico)
            {
                dano = (int)System.Math.Round(dano * TabelaDeCombate.MultiplicadorDano_Critico);
            }

            return new ResultadoAcao
            {
                Sucesso = true,
                DanoCausado = dano,
                Mensagem = critico
                    ? $"{Nome} atacou! CRÍTICO! Causou {dano} de dano."
                    : $"{Nome} atacou! Causou {dano} de dano."
            };
        }

        public void DecrementarEfeitosAtivos()
        {
            EfeitosTemporariosUtil.Decrementar(EfeitosAtivos);
            EfeitosTemporariosUtil.Decrementar(EfeitosDeDanoAtivos);
        }
    }
}
