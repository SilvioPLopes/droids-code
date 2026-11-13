// Ferramenta Editor: somente leitura de cenas; IO limitado a evidencias da circulacao.
// Nao cria objetos, nao modifica componentes e nao salva cenas nem assets.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DroidsCode.Casa;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Inventario persistente da geometria, imagens e pontos de interacao, fora de Play.</summary>
public static class InventarioCirculacaoTester
{
    [Serializable]
    public sealed class InventarioDto
    {
        public int versao = 1;
        public string exportadoEmUtc;
        public bool somenteLeitura = true;
        public CenaDto[] cenas;
    }

    [Serializable]
    public sealed class CenaDto
    {
        public string nome;
        public string path;
        public FundoDto fundo;
        public PlayerDto[] players;
        public ColisorDto[] solidos;
        public ColisorDto[] triggers;
        public NpcDto[] npcs;
        public TerminalDto[] terminais;
        public TerminalCasaDto[] terminaisCasa;
        public PortalDto[] portais;
    }

    [Serializable]
    public sealed class PontoDto
    {
        public float x;
        public float y;
        public PontoDto(Vector3 ponto) { x = ponto.x; y = ponto.y; }
    }

    [Serializable]
    public sealed class CaminhoPontosDto
    {
        public int indice;
        public PontoDto[] pontos;
    }

    [Serializable]
    public sealed class FundoDto
    {
        public string caminho;
        public string prefabSourcePath;
        public string spriteAssetPath;
        public string textureAssetPath;
        public int textureWidth;
        public int textureHeight;
        public float rectXPixels;
        public float rectYPixels;
        public float rectWidthPixels;
        public float rectHeightPixels;
        public float pivotXPixels;
        public float pivotYPixels;
        public float pixelsPerUnit;
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;
        public bool flipX;
        public bool flipY;
        public bool enabled;
        public bool active;
        public string drawMode;
        public PontoDto[] cornersWorld;
    }

    [Serializable]
    public sealed class PlayerDto
    {
        public string caminho;
        public string prefabSourcePath;
        public float rootX;
        public float rootY;
        public float velocidade;
        public float escalaWorldX;
        public float escalaWorldY;
        public float escalaWorldZ;
        public bool enabled;
        public bool active;
        public bool temColliderCircular;
        public string colliderPath;
        public float colliderCenterOffsetX;
        public float colliderCenterOffsetY;
        public float colliderCenterWorldX;
        public float colliderCenterWorldY;
        public float raioWorld;
    }

    [Serializable]
    public sealed class ColisorDto
    {
        public string caminho;
        public string colliderPath;
        public string nome;
        public string prefabSourcePath;
        public string tipo;
        public bool enabled;
        public bool active;
        public bool activeSelf;
        public bool isTrigger;
        public bool eDoPlayer;
        public bool obstaculoNumerado;
        public int layer;
        public string layerName;
        public bool boundsDisponivel;
        public float x;
        public float y;
        public float width;
        public float height;
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;
        public float offsetLocalX;
        public float offsetLocalY;
        public float centroGeometriaWorldX;
        public float centroGeometriaWorldY;
        public float raioWorld;
        public float edgeRadiusWorld;
        public bool geometriaDetalhada;
        public PontoDto[] cornersWorld = Array.Empty<PontoDto>();
        public PontoDto[] pointsWorld = Array.Empty<PontoDto>();
        public CaminhoPontosDto[] pathsWorld = Array.Empty<CaminhoPontosDto>();
    }

    [Serializable]
    public sealed class NpcDto
    {
        public string caminho;
        public string prefabSourcePath;
        public string tipo;
        public string idNpc;
        public string nome;
        public string nomeDoNpc;
        public string[] nomes;
        public bool enabled;
        public bool active;
        public float x;
        public float y;
        public string[] triggerColliderPaths;
        public string destinoDeVisita;
        public float chegadaDaVisitaX;
        public float chegadaDaVisitaY;
    }

    [Serializable]
    public sealed class TerminalDto
    {
        public string caminho;
        public string prefabSourcePath;
        public string idPuzzle;
        public string flagNecessaria;
        public bool enabled;
        public bool active;
        public float x;
        public float y;
        public string[] triggerColliderPaths;
    }

    [Serializable]
    public sealed class TerminalCasaDto
    {
        public string caminho;
        public string prefabSourcePath;
        public string tipo = "InteragirComTerminalCasa";
        public bool enabled;
        public bool active;
        public float x;
        public float y;
        public string[] triggerColliderPaths;
    }

    [Serializable]
    public sealed class PortalDto
    {
        public string caminho;
        public string prefabSourcePath;
        public string nome;
        public string destino;
        public float pontoChegadaX;
        public float pontoChegadaY;
        public bool trancada;
        public bool enabled;
        public bool active;
        public string[] colliderPaths;
    }

    /// <summary>
    /// Exporta inventario.json em uma pasta de evidencias nova. Recusa Play, cenas sujas,
    /// cenas sem caminho salvo, destino externo e sobrescrita de inventario anterior.
    /// Restaura o conjunto original de cenas, inclusive ativa/carregadas, antes do IO.
    /// </summary>
    public static string Exportar(string pasta)
    {
        Exigir(!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode,
            "O inventario exige o Editor fora de Play e sem transicao para Play.");
        ExigirCenasLimpas();
        string destino = ResolverPasta(pasta);
        string arquivo = Path.Combine(destino, "inventario.json");
        Exigir(!File.Exists(arquivo), "Inventario existente: escolha outra pasta de evidencias.");

        string[] cenas = EditorBuildSettings.scenes
            .Where(c => c.enabled && CenaNoEscopo(c.path))
            .Select(c => c.path).Distinct(StringComparer.Ordinal).ToArray();
        Exigir(cenas.Length > 0, "Nenhuma cena habilitada no escopo City/CasaLip/Interiores.");
        SceneSetup[] anterior = EditorSceneManager.GetSceneManagerSetup();
        var inventario = new InventarioDto { exportadoEmUtc = DateTime.UtcNow.ToString("o") };
        var resultados = new List<CenaDto>();
        try
        {
            foreach (string path in cenas)
            {
                Scene cena = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Physics2D.SyncTransforms();
                resultados.Add(LerCena(cena));
                ExigirCenasLimpas();
            }
        }
        finally
        {
            EditorSceneManager.RestoreSceneManagerSetup(anterior);
            Physics2D.SyncTransforms();
        }

        inventario.cenas = resultados.ToArray();
        // Revalida o destino apos a leitura; nunca sobrescreve uma execucao anterior.
        ResolverPasta(destino);
        Directory.CreateDirectory(destino);
        using (var stream = new FileStream(arquivo, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            writer.Write(JsonUtility.ToJson(inventario, true));

        return "Inventario: " + resultados.Count + " cenas, " + resultados.Sum(c => c.solidos.Length)
            + " solidos, " + resultados.Sum(c => c.triggers.Length) + " triggers, "
            + resultados.Sum(c => c.npcs.Length) + " NPCs, "
            + resultados.Sum(c => c.terminais.Length + c.terminaisCasa.Length) + " terminais, "
            + resultados.Sum(c => c.portais.Length) + " portais. " + arquivo;
    }

    static CenaDto LerCena(Scene cena)
    {
        // Referencias novas a cada cena; nao consulta singleton nem objetos de outra cena.
        GameObject[] raizes = cena.GetRootGameObjects();
        PlayerMovement[] players = Componentes<PlayerMovement>(raizes);
        ColisorDto[] colisores = Componentes<Collider2D>(raizes).Select(LerColisor).ToArray();
        return new CenaDto
        {
            nome = cena.name,
            path = cena.path,
            fundo = LerFundo(Componentes<SpriteRenderer>(raizes)),
            players = players.Select(LerPlayer).ToArray(),
            solidos = colisores.Where(c => !c.isTrigger).ToArray(),
            triggers = colisores.Where(c => c.isTrigger).ToArray(),
            npcs = LerNpcs(raizes),
            terminais = Componentes<TerminalDoInterior>(raizes).Select(t => new TerminalDto
            {
                caminho = Caminho(t.transform), prefabSourcePath = PrefabPath(t),
                idPuzzle = t.idPuzzle, flagNecessaria = t.flagNecessaria,
                enabled = t.enabled, active = t.gameObject.activeInHierarchy,
                x = t.transform.position.x, y = t.transform.position.y,
                triggerColliderPaths = ColisoresDoComponente(t, true)
            }).ToArray(),
            terminaisCasa = Componentes<InteragirComTerminalCasa>(raizes).Select(t => new TerminalCasaDto
            {
                caminho = Caminho(t.transform), prefabSourcePath = PrefabPath(t),
                enabled = t.enabled, active = t.gameObject.activeInHierarchy,
                x = t.transform.position.x, y = t.transform.position.y,
                triggerColliderPaths = ColisoresDoComponente(t, true)
            }).ToArray(),
            portais = Componentes<TransicaoDeCena>(raizes).Select(p => new PortalDto
            {
                caminho = Caminho(p.transform), prefabSourcePath = PrefabPath(p), nome = p.name,
                destino = p.nomeCenaDestino, pontoChegadaX = p.pontoDeChegada.x,
                pontoChegadaY = p.pontoDeChegada.y, trancada = p.trancada,
                enabled = p.enabled, active = p.gameObject.activeInHierarchy,
                colliderPaths = ColisoresDoComponente(p, false)
            }).ToArray()
        };
    }

    static FundoDto LerFundo(SpriteRenderer[] renderers)
    {
        SpriteRenderer renderer = renderers.Where(r => r.sprite != null && r.sprite.texture != null
                && !DoPlayer(r.transform))
            .OrderByDescending(r => (double)r.bounds.size.x * r.bounds.size.y)
            .ThenBy(r => Caminho(r.transform), StringComparer.Ordinal).FirstOrDefault();
        if (renderer == null) return null;
        Sprite sprite = renderer.sprite;
        Rect rect = sprite.rect;
        Bounds bounds = renderer.bounds;
        Vector2 size = renderer.drawMode == SpriteDrawMode.Simple
            ? new Vector2(rect.width / sprite.pixelsPerUnit, rect.height / sprite.pixelsPerUnit)
            : renderer.size;
        Vector2 pivotNormalizado = new Vector2(sprite.pivot.x / rect.width, sprite.pivot.y / rect.height);
        Vector2 min = -Vector2.Scale(size, pivotNormalizado);
        Vector2 max = min + size;
        Vector2[] cantos = { min, new Vector2(max.x, min.y), max, new Vector2(min.x, max.y) };
        return new FundoDto
        {
            caminho = Caminho(renderer.transform), prefabSourcePath = PrefabPath(renderer),
            spriteAssetPath = AssetDatabase.GetAssetPath(sprite),
            textureAssetPath = AssetDatabase.GetAssetPath(sprite.texture),
            textureWidth = sprite.texture.width, textureHeight = sprite.texture.height,
            rectXPixels = rect.x, rectYPixels = rect.y, rectWidthPixels = rect.width,
            rectHeightPixels = rect.height, pivotXPixels = sprite.pivot.x, pivotYPixels = sprite.pivot.y,
            pixelsPerUnit = sprite.pixelsPerUnit,
            minX = bounds.min.x, minY = bounds.min.y, maxX = bounds.max.x, maxY = bounds.max.y,
            flipX = renderer.flipX, flipY = renderer.flipY, enabled = renderer.enabled,
            active = renderer.gameObject.activeInHierarchy, drawMode = renderer.drawMode.ToString(),
            cornersWorld = cantos.Select(c => new PontoDto(renderer.transform.TransformPoint(
                new Vector3(renderer.flipX ? -c.x : c.x, renderer.flipY ? -c.y : c.y, 0f)))).ToArray()
        };
    }

    static PlayerDto LerPlayer(PlayerMovement player)
    {
        CircleCollider2D[] circulos = player.GetComponents<CircleCollider2D>();
        Exigir(circulos.Length <= 1, "Mais de um CircleCollider2D no Player: " + Caminho(player.transform));
        CircleCollider2D circulo = circulos.FirstOrDefault();
        Vector3 origem = player.transform.position;
        Vector3 centro = circulo != null ? circulo.transform.TransformPoint(circulo.offset) : origem;
        Vector3 escala = player.transform.lossyScale;
        return new PlayerDto
        {
            caminho = Caminho(player.transform), prefabSourcePath = PrefabPath(player),
            rootX = origem.x, rootY = origem.y, velocidade = player.velocidade,
            escalaWorldX = escala.x, escalaWorldY = escala.y, escalaWorldZ = escala.z,
            enabled = player.enabled, active = player.gameObject.activeInHierarchy,
            temColliderCircular = circulo != null,
            colliderPath = circulo != null ? CaminhoColisor(circulo) : "",
            colliderCenterOffsetX = centro.x - origem.x, colliderCenterOffsetY = centro.y - origem.y,
            colliderCenterWorldX = centro.x, colliderCenterWorldY = centro.y,
            raioWorld = circulo != null ? circulo.radius * EscalaMaxima(circulo.transform) : 0f
        };
    }

    static ColisorDto LerColisor(Collider2D colisor)
    {
        Bounds bounds = colisor.bounds;
        Vector3 centro = colisor.transform.TransformPoint(colisor.offset);
        var dto = new ColisorDto
        {
            caminho = Caminho(colisor.transform), colliderPath = CaminhoColisor(colisor),
            nome = colisor.name, prefabSourcePath = PrefabPath(colisor), tipo = colisor.GetType().Name,
            enabled = colisor.enabled, active = colisor.gameObject.activeInHierarchy,
            activeSelf = colisor.gameObject.activeSelf, isTrigger = colisor.isTrigger,
            eDoPlayer = DoPlayer(colisor.transform), obstaculoNumerado = NomeObstaculo(colisor.name),
            layer = colisor.gameObject.layer, layerName = LayerMask.LayerToName(colisor.gameObject.layer),
            boundsDisponivel = colisor.enabled && colisor.gameObject.activeInHierarchy,
            x = bounds.center.x, y = bounds.center.y, width = bounds.size.x, height = bounds.size.y,
            minX = bounds.min.x, minY = bounds.min.y, maxX = bounds.max.x, maxY = bounds.max.y,
            offsetLocalX = colisor.offset.x, offsetLocalY = colisor.offset.y,
            centroGeometriaWorldX = centro.x, centroGeometriaWorldY = centro.y
        };
        if (colisor is BoxCollider2D box)
        {
            Vector2 metade = box.size * 0.5f;
            dto.cornersWorld = Mundo(box, new[] { -metade, new Vector2(metade.x, -metade.y),
                metade, new Vector2(-metade.x, metade.y) });
            dto.edgeRadiusWorld = box.edgeRadius * EscalaMaxima(box.transform);
            dto.geometriaDetalhada = true;
        }
        else if (colisor is CircleCollider2D circulo)
        {
            dto.raioWorld = circulo.radius * EscalaMaxima(circulo.transform);
            dto.geometriaDetalhada = true;
        }
        else if (colisor is EdgeCollider2D borda)
        {
            dto.pointsWorld = Mundo(borda, borda.points);
            dto.edgeRadiusWorld = borda.edgeRadius * EscalaMaxima(borda.transform);
            dto.geometriaDetalhada = true;
        }
        else if (colisor is PolygonCollider2D poligono)
        {
            dto.pathsWorld = Enumerable.Range(0, poligono.pathCount).Select(i => new CaminhoPontosDto
            {
                indice = i, pontos = Mundo(poligono, poligono.GetPath(i))
            }).ToArray();
            dto.geometriaDetalhada = true;
        }
        // Tipos nao detalhados permanecem identificados; bounds nao e substituto de forma exata.
        return dto;
    }

    static NpcDto[] LerNpcs(GameObject[] raizes)
    {
        Component[] npcs = Componentes<MoradorInterativo>(raizes).Cast<Component>()
            .Concat(Componentes<NpcInterativo>(raizes)).OrderBy(n => Caminho(n.transform), StringComparer.Ordinal)
            .ThenBy(n => n.GetType().Name, StringComparer.Ordinal).ToArray();
        return npcs.Select(n =>
        {
            MoradorInterativo morador = n as MoradorInterativo;
            NpcInterativo generico = n as NpcInterativo;
            string nome = morador != null ? morador.nome : "";
            string nomeDoNpc = generico != null ? generico.nomeDoNpc : "";
            return new NpcDto
            {
                caminho = Caminho(n.transform), prefabSourcePath = PrefabPath(n), tipo = n.GetType().Name,
                idNpc = morador != null ? morador.idNpc : "", nome = nome, nomeDoNpc = nomeDoNpc,
                nomes = new[] { n.name, nome, nomeDoNpc }.Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct(StringComparer.Ordinal).ToArray(),
                enabled = ((Behaviour)n).enabled, active = n.gameObject.activeInHierarchy,
                x = n.transform.position.x, y = n.transform.position.y,
                triggerColliderPaths = ColisoresDoComponente(n, true),
                destinoDeVisita = morador != null ? morador.destinoDeVisita : "",
                chegadaDaVisitaX = morador != null ? morador.chegadaDaVisita.x : 0f,
                chegadaDaVisitaY = morador != null ? morador.chegadaDaVisita.y : 0f
            };
        }).ToArray();
    }

    static T[] Componentes<T>(GameObject[] raizes) where T : Component
    {
        return raizes.SelectMany(r => r.GetComponentsInChildren<T>(true))
            .OrderBy(c => Caminho(c.transform), StringComparer.Ordinal).ToArray();
    }

    static string[] ColisoresDoComponente(Component componente, bool somenteTriggers)
    {
        // OnTrigger nos componentes inventariados e associado aos colisores do mesmo GameObject.
        return componente.GetComponents<Collider2D>().Where(c => !somenteTriggers || c.isTrigger)
            .Select(CaminhoColisor).ToArray();
    }

    static PontoDto[] Mundo(Collider2D colisor, Vector2[] pontos)
    {
        return pontos.Select(p => new PontoDto(colisor.transform.TransformPoint(p + colisor.offset))).ToArray();
    }

    static float EscalaMaxima(Transform transform)
    {
        Vector3 escala = transform.lossyScale;
        return Mathf.Max(Mathf.Abs(escala.x), Mathf.Abs(escala.y));
    }

    static bool DoPlayer(Transform transform)
    {
        for (Transform atual = transform; atual != null; atual = atual.parent)
            if (atual.GetComponent<PlayerMovement>() != null || atual.CompareTag("Player")) return true;
        return false;
    }

    static string Caminho(Transform transform)
    {
        var nomes = new List<string>();
        for (Transform atual = transform; atual != null; atual = atual.parent) nomes.Add(atual.name);
        nomes.Reverse();
        return string.Join("/", nomes);
    }

    static string CaminhoColisor(Collider2D colisor)
    {
        int indice = Array.IndexOf(colisor.GetComponents<Collider2D>(), colisor);
        return Caminho(colisor.transform) + "/" + colisor.GetType().Name + "[" + indice + "]";
    }

    static string PrefabPath(Component componente)
    {
        Component origem = PrefabUtility.GetCorrespondingObjectFromSource(componente);
        string path = origem != null ? AssetDatabase.GetAssetPath(origem) : "";
        return !string.IsNullOrEmpty(path) ? path
            : PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(componente.gameObject) ?? "";
    }

    static bool NomeObstaculo(string nome)
    {
        const string prefixo = "Obstaculo";
        if (!nome.StartsWith(prefixo, StringComparison.Ordinal)) return false;
        string numero = nome.Substring(prefixo.Length);
        if (numero.StartsWith("_", StringComparison.Ordinal)) numero = numero.Substring(1);
        return numero.Length > 0 && numero.All(c => c >= '0' && c <= '9');
    }

    static bool CenaNoEscopo(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        string normalizado = path.Replace('\\', '/');
        return normalizado == "Assets/Scenes/City.unity" || normalizado == "Assets/Scenes/CasaLip.unity"
            || normalizado.StartsWith("Assets/Scenes/Interiores/", StringComparison.Ordinal)
                && normalizado.EndsWith(".unity", StringComparison.OrdinalIgnoreCase);
    }

    static void ExigirCenasLimpas()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene cena = SceneManager.GetSceneAt(i);
            Exigir(!cena.isDirty, "Cena aberta com alteracoes nao salvas: " + cena.name);
            Exigir(!string.IsNullOrEmpty(cena.path), "Cena aberta sem caminho salvo: " + cena.name);
        }
    }

    static string ResolverPasta(string pasta)
    {
        Exigir(!string.IsNullOrWhiteSpace(pasta), "Informe a pasta de evidencias.");
        string projeto = Directory.GetParent(Application.dataPath).FullName;
        string raiz = Path.GetFullPath(Path.Combine(projeto, "output/apresentacao-jogavel/2026-11-12/circulacao"));
        string destino = Path.GetFullPath(Path.IsPathRooted(pasta) ? pasta : Path.Combine(projeto, pasta))
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        Exigir(string.Equals(destino, raiz, StringComparison.OrdinalIgnoreCase)
            || destino.StartsWith(raiz + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase),
            "O destino deve ficar em output/apresentacao-jogavel/2026-11-12/circulacao/.");
        // Nao permite que junction/symlink dentro da arvore de output redirecione o IO para fora dela.
        for (DirectoryInfo atual = new DirectoryInfo(destino); atual != null; atual = atual.Parent)
        {
            if (atual.Exists)
                Exigir((atual.Attributes & FileAttributes.ReparsePoint) == 0,
                    "Destino de evidencias contem redirecionamento de diretorio: " + atual.FullName);
            if (string.Equals(atual.FullName, projeto, StringComparison.OrdinalIgnoreCase)) break;
        }
        return destino;
    }

    static void Exigir(bool condicao, string mensagem)
    {
        if (!condicao) throw new InvalidOperationException("[InventarioCirculacao] " + mensagem);
    }
}
