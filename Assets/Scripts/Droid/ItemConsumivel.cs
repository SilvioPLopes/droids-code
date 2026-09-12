namespace DroidsCode.DroidCore
{
    // PLACEHOLDER minimo pra dar funcao real ao botao "Item" (ver
    // CHECKLIST_DE_DESENVOLVIMENTO.md — hoje ele cura sozinho, sem menu).
    // NAO e um sistema de inventario real: sem quantidade, sem persistencia
    // em DadosDoJogo, sem drop. E uma lista fixa disponivel em qualquer
    // batalha. Evoluir pra biblioteca externa de itens (ver contexto do
    // repositorio ragnarok-core) fica pra depois, fora de escopo agora.
    public class ItemConsumivel
    {
        public string Nome { get; set; }
        public int CuraHp { get; set; }
    }

    public static class ItensDeBatalha
    {
        public static readonly ItemConsumivel[] Disponiveis = new[]
        {
            new ItemConsumivel { Nome = "Poção Pequena", CuraHp = 2 },
            new ItemConsumivel { Nome = "Poção Média", CuraHp = 5 },
            new ItemConsumivel { Nome = "Poção Grande", CuraHp = 8 },
            new ItemConsumivel { Nome = "Reparo Completo", CuraHp = 9999 } // valor alto o suficiente pra sempre encher o HpMax; UsarItem() limita ao HpMax
        };
    }
}
