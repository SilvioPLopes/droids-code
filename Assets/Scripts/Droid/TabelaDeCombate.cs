namespace DroidsCode.DroidCore
{
    // PLACEHOLDER — valor de rascunho, nao balanceado. Separado da
    // TabelaDeCustos porque aquela e sobre CUSTO em pontos de progressao;
    // esta e sobre OUTPUT de dano/acerto/critico em combate — responsabilidades
    // diferentes.
    public static class TabelaDeCombate
    {
        // Equivalente ao "weaponAtk" do BattleEngine.java de referencia:
        // cada NivelDeDano da TecnicaComposta vale X de dano bruto.
        public const int DanoPorNivelDeTecnica = 5;

        // --- Estagio 1 (12/09/2026): atributos derivados ---

        // VIT -> HP: cada ponto de VIT da +1% do HpMaxBase, arredondado pra
        // baixo. Ex: HpMaxBase 30, VIT 30 => +30% => +9 HP (round down).
        public const float PercentualHpPorVit = 0.01f;

        // DEX -> HIT, AGI -> FLEE. Formula estilo Ragnarok simplificada:
        // chance = Base + (HIT do atacante - FLEE do alvo), com piso/teto pra
        // nunca ser garantido nem impossivel.
        public const int ChanceDeAcertoBase = 50;
        public const int ChanceDeAcertoMinima = 5;
        public const int ChanceDeAcertoMaxima = 95;

        // LUK -> critico. 1 LUK = 1% de chance. Critico dobra o dano e ignora
        // a chance de erro (rola depois do hit ja ter acertado).
        public const float PercentualCriticoPorLuk = 1f;
        public const float MultiplicadorDano_Critico = 2f;

        // INT -> resistencia a efeito (Stun/Veneno) e bonus de cura de item.
        // 1 INT = 1% de chance de resistir / 1% a mais de cura recebida.
        public const float PercentualResistenciaPorInt = 1f;
        public const float PercentualBonusCuraPorInt = 1f;

        // Valores fixos pro InimigoFixo (nao tem atributos configuraveis
        // ainda — ver InimigoFixo.cs). Serve so pra ele respeitar os mesmos
        // calculos que o Droid; substituir quando houver inimigos variados.
        public const int HitPadraoInimigo = 10;
        public const int FleePadraoInimigo = 10;
        public const float ChanceCriticoPadraoInimigo = 5f;
        public const float ResistenciaEfeitoPadraoInimigo = 0f;
    }
}
