namespace DroidsCode.DroidCore
{
    public class Cabeca : DroidPart
    {
        public Cabeca(string nome, int atributoPrincipal, Raridade raridade,
            TipoAtributo? atributoBonificado = null, int valorDoBonus = 0)
            : base(nome, atributoPrincipal, raridade, atributoBonificado, valorDoBonus) { }
    }
}
