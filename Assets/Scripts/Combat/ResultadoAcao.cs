namespace DroidsCode.Combat
{
    // Versão mínima e propositalmente incompleta (ver ESPECIFICACAO_TECNICA.md, secao 3).
    // Nao adicionar campos (status, efeito visual, combo) sem Brainstorm no PLANO_IMPLEMENTACAO.md.
    public class ResultadoAcao
    {
        public bool Sucesso { get; set; }
        public int DanoCausado { get; set; }
        public string Mensagem { get; set; }
    }
}
