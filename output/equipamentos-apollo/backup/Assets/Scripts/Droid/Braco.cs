namespace DroidsCode.DroidCore
{
    public class Braco : DroidPart
    {
        // Estagio 3: parametros novos sao opcionais (default null/0) -- nao
        // quebra nenhuma chamada existente que so passava os 3 primeiros.
        public Braco(string nome, int atributoPrincipal, Raridade raridade,
            TipoAtributo? atributoBonificado = null, int valorDoBonus = 0)
            : base(nome, atributoPrincipal, raridade, atributoBonificado, valorDoBonus) { }
    }
}
