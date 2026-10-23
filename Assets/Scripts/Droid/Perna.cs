namespace DroidsCode.DroidCore
{
    public class Perna : DroidPart
    {
        public Perna(string nome, int atributoPrincipal, Raridade raridade,
            TipoAtributo? atributoBonificado = null, int valorDoBonus = 0, string idItem = null)
            : base(nome, atributoPrincipal, raridade, atributoBonificado, valorDoBonus, idItem) { }
    }
}
