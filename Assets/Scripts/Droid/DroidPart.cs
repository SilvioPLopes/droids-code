namespace DroidsCode.DroidCore
{
    public enum Raridade
    {
        Comum,
        Raro,
        Lendario
    }

    public abstract class DroidPart
    {
        public string Nome { get; set; }
        public int AtributoPrincipal { get; set; }
        public Raridade Raridade { get; set; }

        protected DroidPart(string nome, int atributoPrincipal, Raridade raridade)
        {
            Nome = nome;
            AtributoPrincipal = atributoPrincipal;
            Raridade = raridade;
        }
    }
}
