using UnityEngine;

/// <summary>Mantém só o diálogo nas coordenadas da CasaLip, sem mudar o Canvas dos menus.</summary>
[ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
public sealed class EscalaLocalDeDialogo : MonoBehaviour
{
    public Vector2 referencia = new Vector2(1920f, 1080f);
    [Range(0f, 1f)] public float combinarLarguraAltura = 0.5f;
    bool ajustando;

    void OnEnable() => AtualizarEscala();
    void LateUpdate() => AtualizarEscala();
    void OnRectTransformDimensionsChange() => AtualizarEscala();

    public void AtualizarEscala()
    {
        if (ajustando) return;
        var area = transform as RectTransform;
        var pai = transform.parent as RectTransform;
        if (area == null || pai == null || pai.rect.width <= 0 || pai.rect.height <= 0
            || referencia.x <= 0 || referencia.y <= 0) return;

        float largura = Mathf.Log(pai.rect.width / referencia.x, 2f);
        float altura = Mathf.Log(pai.rect.height / referencia.y, 2f);
        float fator = Mathf.Pow(2f, Mathf.Lerp(largura, altura, combinarLarguraAltura));
        ajustando = true;
        try
        {
            area.anchorMin = area.anchorMax = area.pivot = new Vector2(0.5f, 0.5f);
            area.anchoredPosition = Vector2.zero;
            var escala = new Vector3(fator, fator, 1f);
            var tamanho = pai.rect.size / fator;
            if (area.localScale != escala) area.localScale = escala;
            if (area.sizeDelta != tamanho) area.sizeDelta = tamanho;
        }
        finally { ajustando = false; }
    }
}
