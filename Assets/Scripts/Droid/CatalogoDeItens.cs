using System.Collections.Generic;

namespace DroidsCode.DroidCore
{
    // Estagio 2 (13/09/2026) — Sistema de Item real, primeira fatia.
    // Substitui ItemConsumivel.cs/ItensDeBatalha (lista fixa sem quantidade).
    // Escopo desta fase: SO itens de Cura fixos. Buff/Debuff/Equipavel/Fora-
    // de-batalha ficam mapeados em CHECKLIST_DE_DESENVOLVIMENTO.md como
    // proxima fase -- nao implementados aqui de proposito (ver secao "Sistema
    // de Item / Inventario real" do checklist).
    //
    // Cada item tem um ID estavel (string) que NUNCA muda depois de definido
    // -- e a chave usada no save (Inventario/ItemSalvo) e no inventario em
    // runtime. Mudar o texto de "Nome" e seguro; mudar "Id" quebra saves
    // existentes.
    public enum TipoDeItem
    {
        Cura
        // Buff, Debuff, Equipavel, ForaDeBatalha -- proxima fase (ver checklist)
    }

    public class DefinicaoDeItem
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public TipoDeItem Tipo { get; set; }
        public int CuraHp { get; set; }
        public bool UsavelEmBatalha { get; set; } = true;
        public bool UsavelForaDeBatalha { get; set; } = true;
    }

    // Catalogo estatico -- mesmo espirito de TabelaDeCombate/TabelaDeCustos
    // (dados de config isolados da logica). O INVENTARIO (quantidade que o
    // jogador possui) fica em GerenciadorDeEstado, nao aqui -- isto aqui e
    // só o "cardapio" de itens que existem no jogo.
    public static class CatalogoDeItens
    {
        public const string IdPocaoPequena = "pocao_pequena";
        public const string IdPocaoMedia = "pocao_media";
        public const string IdPocaoGrande = "pocao_grande";
        public const string IdReparoCompleto = "reparo_completo";

        public static readonly Dictionary<string, DefinicaoDeItem> Todos = new Dictionary<string, DefinicaoDeItem>
        {
            [IdPocaoPequena] = new DefinicaoDeItem { Id = IdPocaoPequena, Nome = "Poção Pequena", Tipo = TipoDeItem.Cura, CuraHp = 20 },
            [IdPocaoMedia] = new DefinicaoDeItem { Id = IdPocaoMedia, Nome = "Poção Média", Tipo = TipoDeItem.Cura, CuraHp = 50 },
            [IdPocaoGrande] = new DefinicaoDeItem { Id = IdPocaoGrande, Nome = "Poção Grande", Tipo = TipoDeItem.Cura, CuraHp = 80 },
            // 9999 == "cura tudo", igual o placeholder antigo em ItemConsumivel.cs
            // (UsarItem sempre limita ao HpMax, entao o numero exato nao importa
            // desde que seja >= qualquer HpMax possivel do jogo).
            [IdReparoCompleto] = new DefinicaoDeItem { Id = IdReparoCompleto, Nome = "Reparo Completo", Tipo = TipoDeItem.Cura, CuraHp = 9999 },
        };

        public static DefinicaoDeItem Obter(string id)
        {
            return Todos.TryGetValue(id, out var def) ? def : null;
        }
    }

    // --- DTOs de save (mesmo padrao de TecnicaSalva/DadosDoJogo — JsonUtility
    // nao serializa Dictionary, entao o inventario vira List<ItemSalvo> pra
    // persistir, e volta a virar Dictionary em runtime). ---
    [System.Serializable]
    public class ItemSalvo
    {
        public string id;
        public int quantidade;
    }
}
