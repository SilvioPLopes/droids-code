using UnityEngine;

/// <summary>Localiza a UI utilizável, inclusive com o painel fechado.</summary>
public static class LocalizadorDeTerminal
{
    public static TerminalUIManager Encontrar()
    {
        TerminalUIManager encontrado = null;
        foreach (var candidato in Resources.FindObjectsOfTypeAll<TerminalUIManager>())
        {
            if (candidato == null || !candidato.gameObject.scene.IsValid() ||
                !candidato.gameObject.scene.isLoaded || candidato.painelTerminal == null ||
                candidato.campoDeCodigo == null)
                continue;

            bool persistente = candidato.gameObject.scene.name == "DontDestroyOnLoad";
            bool encontradoPersistente = encontrado != null &&
                encontrado.gameObject.scene.name == "DontDestroyOnLoad";
            if (encontrado == null || (persistente && !encontradoPersistente) ||
                (persistente == encontradoPersistente && CompararHierarquia(candidato, encontrado) < 0))
                encontrado = candidato;
        }
        return encontrado;
    }

    private static int CompararHierarquia(TerminalUIManager primeiro, TerminalUIManager segundo)
    {
        int cena = string.CompareOrdinal(primeiro.gameObject.scene.path, segundo.gameObject.scene.path);
        if (cena != 0) return cena;
        return string.CompareOrdinal(CaminhoNaHierarquia(primeiro.transform), CaminhoNaHierarquia(segundo.transform));
    }

    private static string CaminhoNaHierarquia(Transform objeto)
    {
        string caminho = "";
        while (objeto != null)
        {
            caminho = objeto.GetSiblingIndex().ToString("D8") + "/" + caminho;
            objeto = objeto.parent;
        }
        return caminho;
    }
}
