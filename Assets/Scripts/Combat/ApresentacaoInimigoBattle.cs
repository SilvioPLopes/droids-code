using DroidsCode.Combat;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Seleciona apenas a arte do inimigo. Não altera estatísticas, encontros ou progresso.</summary>
[DisallowMultipleComponent, RequireComponent(typeof(Image))]
public sealed class ApresentacaoInimigoBattle : MonoBehaviour
{
    public BattleManager batalha;
    public Sprite droidA, droidB, legado;
    [System.Serializable]
    public class VisualPorId
    {
        public string id;
        public Sprite sprite;
    }
    [Tooltip("Artes por ID do catálogo. Campos A/B mantidos como compatibilidade para montagens anteriores.")]
    public VisualPorId[] visuais;
    public Rect quadroRobos = new Rect(1040, 215, 280, 280);
    public Rect quadroLegado = new Rect(1120, 239, 230, 230);
    public string IdVisualAtual { get; private set; }

    void Start() => AplicarContexto();

    public void AplicarContexto()
    {
        // Prefab não guarda referência a um gestor externo. Resolver somente na própria cena.
        if (batalha == null)
        {
            foreach (var raiz in gameObject.scene.GetRootGameObjects())
            {
                batalha = raiz.GetComponentInChildren<BattleManager>(true);
                if (batalha != null) break;
            }
        }
        var estado = GerenciadorDeEstado.Instancia;
        AplicarPorId(!string.IsNullOrEmpty(estado.ProximoInimigoId)
            ? estado.ProximoInimigoId : batalha != null ? batalha.idDoInimigoPadrao : "");
    }

    public void AplicarPorId(string id)
    {
        bool a = string.IsNullOrEmpty(id) || id == CatalogoDeInimigos.IdSucataDeCaptura;
        bool b = id == CatalogoDeInimigos.IdSucataReforcada;
        var sprite = a ? droidA : b ? droidB : legado;
        bool novo = (a || b) && sprite != null;
        string idVisual = string.IsNullOrEmpty(id) ? CatalogoDeInimigos.IdSucataDeCaptura : id;
        // Recurso do boss evita editar a cena/prefab compartilhados só para esta associação.
        // Um cadastro explícito em visuais, abaixo, continua tendo precedência.
        if (idVisual == CatalogoDeInimigos.IdDroidDoIvo)
        {
            var boss = Resources.Load<Sprite>("Inimigos/BossIvo-v2");
            if (boss != null) { sprite = boss; novo = true; }
        }
        if (visuais != null)
        {
            foreach (var visual in visuais)
            {
                if (visual == null || visual.id != idVisual || visual.sprite == null) continue;
                sprite = visual.sprite; novo = true;
                break;
            }
        }
        if (sprite == null) sprite = legado;
        var imagem = GetComponent<Image>();
        imagem.sprite = sprite; imagem.preserveAspect = true; imagem.raycastTarget = false;
        imagem.type = Image.Type.Simple;
        var r = imagem.rectTransform; var quadro = novo ? quadroRobos : quadroLegado;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(quadro.x, -quadro.y);
        r.sizeDelta = new Vector2(quadro.width, quadro.height); r.localScale = Vector3.one;
        IdVisualAtual = id ?? "";
    }
}
