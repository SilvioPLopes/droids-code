using System.Collections.Generic;
using DroidsCode.Combat;

namespace DroidsCode.DroidCore
{
    public class Droid : IParticipanteDeCombate
    {
        public string Nome { get; set; }
        public int Hp { get; set; }
        public int HpMax { get; set; }

        public Braco Braco { get; set; }
        public Perna Perna { get; set; }
        public Tronco Tronco { get; set; }
        public Cabeca Cabeca { get; set; }

        // Substitui o antigo Dictionary<string, Closure>. O Lua so edita este
        // dado no terminal (fora de batalha) — nunca e chamado durante o turno.
        // Ver CORRECAO_ARQUITETURA_TERMINAL_MENU.md.
        public Dictionary<string, TecnicaComposta> TecnicasConfiguradas { get; } = new Dictionary<string, TecnicaComposta>();

        // Secao 9 — atributos base + calculo de total
        public DroidStats StatsBase { get; set; } = new DroidStats();

        // Defesa = VIT total, mesmo padrao do Player.java de referencia
        // (getTotalDef() = getTotalVit() + armadura). Droids Code ainda nao
        // tem armadura/equipamento com bonus de defesa — so VIT por ora.
        public int Defesa => ObterTotal(TipoAtributo.Vit);

        // Secao 13 — efeitos ativos (buffs/debuffs), so o Droid do jogador tem
        public List<EfeitoDeAtributo> EfeitosAtivos { get; } = new List<EfeitoDeAtributo>();

        // Sempre disponivel, independente do que o jogador programou no
        // terminal — e o que aparece se "Lutar" for clicado sem nenhuma
        // tecnica configurada.
        public const string NomeAtaqueBasico = "Ataque Basico";

        public Droid(string nome, int hpMax)
        {
            Nome = nome;
            HpMax = hpMax;
            Hp = hpMax;
            TecnicasConfiguradas[NomeAtaqueBasico] = new TecnicaComposta { Nome = NomeAtaqueBasico, NivelDeDano = 1 };
        }

        public IEnumerable<string> ObterAcoesDisponiveis()
        {
            return TecnicasConfiguradas.Keys;
        }

        public ResultadoAcao ExecutarAcao(string nomeAcao, IParticipanteDeCombate alvo)
        {
            if (!TecnicasConfiguradas.TryGetValue(nomeAcao, out TecnicaComposta tecnica))
            {
                return new ResultadoAcao
                {
                    Sucesso = false,
                    DanoCausado = 0,
                    Mensagem = $"Tecnica '{nomeAcao}' nao esta configurada neste Droid."
                };
            }

            // Adaptado de BattleEngine.calculateDamage (Ragnarok Core, testado
            // 20x em BattleEngineTest): dano = max(1, totalAtk - defesaAlvo).
            // statusAtk = FOR*2 (igual ao totalStr*2 do Java).
            // "weaponAtk" do Java vira NivelDeDano*DanoPorNivelDeTecnica aqui —
            // a tecnica programada e a "arma" do Droid.
            int statusAtk = ObterTotal(TipoAtributo.For) * 2;
            int bonusTecnica = tecnica.NivelDeDano * TabelaDeCombate.DanoPorNivelDeTecnica;
            int totalAtk = statusAtk + bonusTecnica;

            int dano = System.Math.Max(1, totalAtk - alvo.Defesa);

            // TODO: EfeitosDeAtributo/EfeitosDeDanoPorTurno da tecnica ainda nao
            // sao aplicados aqui (nao existe fluxo de buff/DoT em combate ainda).
            // Ver CombatEngine — quem chama DecrementarEfeitosAtivos.

            return new ResultadoAcao
            {
                Sucesso = true,
                DanoCausado = dano,
                Mensagem = $"{Nome} usou '{nomeAcao}'! Causou {dano} de dano."
            };
        }

        public int ObterTotal(TipoAtributo atributo)
        {
            int baseValor = ObterValorBase(atributo);
            int bonusPeca = ObterBonusDePecas(atributo);
            // Bonus de buff fora do escopo — nao existe sistema de buff/duracao aplicado ainda.
            return baseValor + bonusPeca;
        }

        private int ObterValorBase(TipoAtributo atributo)
        {
            return atributo switch
            {
                TipoAtributo.For => StatsBase.For,
                TipoAtributo.Agi => StatsBase.Agi,
                TipoAtributo.Vit => StatsBase.Vit,
                TipoAtributo.Int => StatsBase.Int,
                TipoAtributo.Dex => StatsBase.Dex,
                TipoAtributo.Luk => StatsBase.Luk,
                _ => 0
            };
        }

        private int ObterBonusDePecas(TipoAtributo atributo)
        {
            // Placeholder: DroidPart so tem AtributoPrincipal generico nesta fase.
            return 0;
        }

        // Chamado pelo CombatEngine em um unico ponto do fluxo de turno.
        public void DecrementarEfeitosAtivos()
        {
            for (int i = EfeitosAtivos.Count - 1; i >= 0; i--)
            {
                if (EfeitosAtivos[i].DuracaoEmTurnos < 0)
                {
                    continue; // permanente
                }

                EfeitosAtivos[i].DuracaoEmTurnos -= 1;

                if (EfeitosAtivos[i].DuracaoEmTurnos <= 0)
                {
                    EfeitosAtivos.RemoveAt(i);
                }
            }
        }
    }
}
