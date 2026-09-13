using System.Collections.Generic;
using DroidsCode.Combat;

namespace DroidsCode.DroidCore
{
    public class Droid : IParticipanteDeCombate
    {
        public string Nome { get; set; }
        public int Hp { get; set; }

        // REFATORACAO (Estagio 1 — 12/09/2026): HpMax deixou de ser um campo
        // solto e virou CALCULADO a partir de HpMaxBase + VIT (ver
        // TabelaDeCombate.PercentualHpPorVit) + bonus fixo de pecas (Fase 2,
        // ainda 0 — ver ObterBonusDePecasHpFixo). HpMaxBase e o unico valor
        // realmente "setado" (construtor / save); HpMax nunca e atribuido
        // direto de novo em lugar nenhum do codebase.
        public int HpMaxBase { get; set; }

        public int HpMax => HpMaxBase + ObterBonusDePecasHpFixo() + CalcularBonusHpPorVit();

        private int CalcularBonusHpPorVit()
        {
            // Arredondado pra baixo, conforme decisao de design (12/09/2026):
            // 1 VIT = 1% do HpMaxBase. Ex: HpMaxBase 30, VIT 30 => +9 (30*30*0.01=9).
            float bonus = HpMaxBase * ObterTotal(TipoAtributo.Vit) * TabelaDeCombate.PercentualHpPorVit;
            return (int)System.Math.Floor(bonus);
        }

        private int ObterBonusDePecasHpFixo()
        {
            // Placeholder pra Fase 2 (customizacao de pecas): pecas poderao
            // dar HP fixo adicional aqui. Hoje sempre 0 -- DroidPart ainda so
            // tem AtributoPrincipal generico (ver ObterBonusDePecas).
            return 0;
        }

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

        // Alinhado com ESPECIFICACAO_TECNICA.md secao 4 (versao atualizada).
        // O proprio Droid guarda seus pontos — antes isso era passado por
        // fora (TerminalDroidApi recebia um PontosDeProgressao separado).
        public PontosDeProgressao Pontos { get; } = new PontosDeProgressao();

        // Nivel/XP ganhos em combate — separado de Pontos (SRP).
        public ProgressaoDeNivel Progressao { get; } = new ProgressaoDeNivel();

        // Defesa = VIT total, mesmo padrao do Player.java de referencia
        // (getTotalDef() = getTotalVit() + armadura). Droids Code ainda nao
        // tem armadura/equipamento com bonus de defesa — so VIT por ora.
        // NOTA (Estagio 1): VIT agora tem DOIS efeitos (Defesa E %HP) por
        // decisao explicita de design — nao e bug, e intencional.
        public int Defesa => ObterTotal(TipoAtributo.Vit);

        // --- Estagio 1 (12/09/2026): atributos derivados HIT/FLEE/Critico/Resistencia ---
        // DEX -> HIT, AGI -> FLEE, LUK -> % critico, INT -> % resistencia a
        // efeito + % bonus de cura recebida. Ver TabelaDeCombate para as
        // constantes/formulas.
        public int Hit => ObterTotal(TipoAtributo.Dex);
        public int Flee => ObterTotal(TipoAtributo.Agi);
        public float ChanceCritica => ObterTotal(TipoAtributo.Luk) * TabelaDeCombate.PercentualCriticoPorLuk;
        public float ChanceDeResistirEfeito => ObterTotal(TipoAtributo.Int) * TabelaDeCombate.PercentualResistenciaPorInt;
        public float BonusPercentualDeCura => ObterTotal(TipoAtributo.Int) * TabelaDeCombate.PercentualBonusCuraPorInt;

        // Secao 13 — efeitos ativos (buffs/debuffs), so o Droid do jogador tem
        public List<EfeitoDeAtributo> EfeitosAtivos { get; } = new List<EfeitoDeAtributo>();

        // Dano por turno ativo (ex: Envenenamento) -- separado de EfeitosAtivos
        // porque EfeitoDeDanoPorTurno e um tipo proprio (ver TecnicaComposta.cs).
        public List<EfeitoDeDanoPorTurno> EfeitosDeDanoAtivos { get; } = new List<EfeitoDeDanoPorTurno>();

        // Sempre disponivel, independente do que o jogador programou no
        // terminal — e o que aparece se "Lutar" for clicado sem nenhuma
        // tecnica configurada.
        public const string NomeAtaqueBasico = "Ataque Basico";

        public Droid(string nome, int hpMax)
        {
            Nome = nome;
            HpMaxBase = hpMax;
            Hp = HpMax; // usa o calculado (VIT base ainda nao setada aqui, mas ja funciona se setar depois)
            TecnicasConfiguradas[NomeAtaqueBasico] = new TecnicaComposta { Nome = NomeAtaqueBasico, NivelDeDano = 1 };
        }

        public IEnumerable<string> ObterAcoesDisponiveis()
        {
            return TecnicasConfiguradas.Keys;
        }

        // Chamado pelo TerminalDroidApi.AplicarUpgrade quando o atributo
        // upado for VIT: precisa recalcular HpMax e dar a DIFERENCA de HP
        // atual (nao deixa o jogador "devendo" cura, mas tambem nao cura de
        // graca alem do que o novo teto abriu). Chamar SEMPRE depois de
        // alterar StatsBase.Vit.
        public void RecalcularHpAposMudancaDeVit(int hpMaxAntes)
        {
            int diferenca = HpMax - hpMaxAntes;
            if (diferenca > 0)
            {
                Hp += diferenca;
            }
            else if (Hp > HpMax)
            {
                // Seguranca: se algum dia VIT puder cair (efeito negativo,
                // desequipar peca), nao deixar Hp > HpMax.
                Hp = HpMax;
            }
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

            // --- Estagio 1: roll de acerto (HIT vs FLEE) ---
            // CORRECAO_DE_ESCOPO (12/09/2026): reintroduz o sistema HIT/FLEE
            // que estava listado como "fora de escopo" — decisao explicita
            // do responsavel do projeto nesta sessao, substitui a marcacao
            // antiga em CHECKLIST_DE_DESENVOLVIMENTO.md/readme.md.
            if (!RolarAcerto(Hit, alvo.Flee))
            {
                return new ResultadoAcao
                {
                    Sucesso = false,
                    DanoCausado = 0,
                    Mensagem = $"{Nome} usou '{nomeAcao}', mas {alvo.Nome} esquivou!"
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

            // --- Estagio 1: roll de critico (LUK), independente do roll de acerto ---
            bool critico = RolarCritico(ChanceCritica);
            if (critico)
            {
                dano = (int)System.Math.Round(dano * TabelaDeCombate.MultiplicadorDano_Critico);
            }

            AplicarEfeitosDaTecnica(tecnica, alvo);

            string mensagem = critico
                ? $"{Nome} usou '{nomeAcao}'! CRÍTICO! Causou {dano} de dano."
                : $"{Nome} usou '{nomeAcao}'! Causou {dano} de dano.";

            return new ResultadoAcao
            {
                Sucesso = true,
                DanoCausado = dano,
                Mensagem = mensagem
            };
        }

        // Rolls isolados em metodos estaticos pra ficarem faceis de testar /
        // reusar (InimigoFixo usa os mesmos).
        public static bool RolarAcerto(int hitAtacante, int fleeAlvo)
        {
            int chance = TabelaDeCombate.ChanceDeAcertoBase + (hitAtacante - fleeAlvo);
            chance = System.Math.Clamp(chance, TabelaDeCombate.ChanceDeAcertoMinima, TabelaDeCombate.ChanceDeAcertoMaxima);
            return UnityEngine.Random.Range(0, 100) < chance;
        }

        public static bool RolarCritico(float chanceCritica)
        {
            return UnityEngine.Random.Range(0f, 100f) < chanceCritica;
        }

        // Copia cada efeito da tecnica pro alvo -- copia, nao referencia direta,
        // pra nao compartilhar o mesmo objeto (e a mesma DuracaoEmTurnos) entre
        // usos diferentes da tecnica.
        // Estagio 1: agora consulta alvo.ChanceDeResistirEfeito (INT) antes de
        // aplicar CADA efeito -- resistencia e rolada por efeito, nao uma vez
        // so pra tecnica inteira (uma tecnica com Stun+Veneno pode resistir um
        // e sofrer o outro).
        private static void AplicarEfeitosDaTecnica(TecnicaComposta tecnica, IParticipanteDeCombate alvo)
        {
            foreach (EfeitoDeAtributo efeito in tecnica.EfeitosDeAtributo)
            {
                if (RolarResistencia(alvo.ChanceDeResistirEfeito)) continue;

                alvo.EfeitosAtivos.Add(new EfeitoDeAtributo
                {
                    NomeExibicao = efeito.NomeExibicao,
                    Atributo = efeito.Atributo,
                    Valor = efeito.Valor,
                    DuracaoEmTurnos = efeito.DuracaoEmTurnos
                });
            }

            foreach (EfeitoDeDanoPorTurno efeito in tecnica.EfeitosDeDanoPorTurno)
            {
                if (RolarResistencia(alvo.ChanceDeResistirEfeito)) continue;

                alvo.EfeitosDeDanoAtivos.Add(new EfeitoDeDanoPorTurno
                {
                    NomeExibicao = efeito.NomeExibicao,
                    DanoPorTurno = efeito.DanoPorTurno,
                    DuracaoEmTurnos = efeito.DuracaoEmTurnos
                });
            }
        }

        private static bool RolarResistencia(float chanceDeResistir)
        {
            return UnityEngine.Random.Range(0f, 100f) < chanceDeResistir;
        }

        public int ObterTotal(TipoAtributo atributo)
        {
            int baseValor = ObterValorBase(atributo);
            int bonusPeca = ObterBonusDePecas(atributo);
            int bonusEfeito = ObterBonusDeEfeitos(atributo);
            return baseValor + bonusPeca + bonusEfeito;
        }

        private int ObterBonusDeEfeitos(TipoAtributo atributo)
        {
            int soma = 0;
            foreach (EfeitoDeAtributo efeito in EfeitosAtivos)
            {
                if (efeito.Atributo == atributo)
                {
                    soma += efeito.Valor;
                }
            }
            return soma;
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
            EfeitosTemporariosUtil.Decrementar(EfeitosAtivos);
            EfeitosTemporariosUtil.Decrementar(EfeitosDeDanoAtivos);
        }
    }
}
