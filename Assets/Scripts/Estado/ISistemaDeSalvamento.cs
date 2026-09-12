// Interface ja prevista em LEIA_PRIMEIRO.md ("candidato futuro a uma
// interface ISistemaDeSalvamento"). Existir como interface permite trocar
// a implementacao (json local -> cloud save, por exemplo) sem mudar quem
// chama.
public interface ISistemaDeSalvamento
{
    void Salvar();
    void Carregar();
    bool ExisteSave();
}
