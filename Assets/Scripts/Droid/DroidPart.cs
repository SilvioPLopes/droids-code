namespace DroidsCode.DroidCore
{
    public enum Raridade
    {
        Comum,
        Raro,
        Lendario
    }

    // Estagio 3 (fase 2 do sistema de item) - Equipavel: identifica em qual
    // slot do Droid uma peca entra. Usado por DefinicaoDeItem (CatalogoDeItens)
    // e Droid.EquiparPeca pra saber qual propriedade (Braco/Perna/Tronco/
    // Cabeca) atualizar sem precisar de reflection.
    public enum TipoDePeca
    {
        Braco,
        Perna,
        Tronco,
        Cabeca
    }

    public abstract class DroidPart
    {
        public string Nome { get; set; }
        public int AtributoPrincipal { get; set; }
        public Raridade Raridade { get; set; }

        // Estagio 3 (fase 2 do sistema de item) - Equipavel: bonus real de
        // atributo que a peca da ao Droid quando equipada (ver
        // Droid.ObterBonusDePecas). Null = peca sem bonus (ex: pecas
        // criadas antes desta fase, ou pecas cosmeticas). Campo NOVO,
        // separado de AtributoPrincipal de proposito -- AtributoPrincipal
        // ja existia como int generico antes desta fase e nao foi mexido,
        // pra nao adivinhar o que ele ja representava em outro lugar do
        // codigo que nao vimos.
        public TipoAtributo? AtributoBonificado { get; set; }
        public int ValorDoBonus { get; set; }

        protected DroidPart(string nome, int atributoPrincipal, Raridade raridade,
            TipoAtributo? atributoBonificado = null, int valorDoBonus = 0)
        {
            Nome = nome;
            AtributoPrincipal = atributoPrincipal;
            Raridade = raridade;
            AtributoBonificado = atributoBonificado;
            ValorDoBonus = valorDoBonus;
        }
    }
}
