using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fala local da lojista; caixa e rabicho são uma única malha sem bloquear cliques.</summary>
[DisallowMultipleComponent]
public sealed class BalaoDaLoja : MaskableGraphic
{
    TextMeshProUGUI nome, fala;

    public void Preparar(TMP_FontAsset fonte)
    {
        raycastTarget = false;
        color = new Color(.10f, .11f, .10f, .99f);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(85, -840);
        rectTransform.sizeDelta = new Vector2(730, 145);
        var moldura = gameObject.AddComponent<Outline>();
        moldura.effectColor = new Color(.70f, .55f, .29f);
        moldura.effectDistance = new Vector2(2, -2);
        nome = CriarTexto("Falante", fonte, 28, 12, 38);
        nome.fontStyle = FontStyles.Bold;
        fala = CriarTexto("Fala", fonte, 34, 55, 78);
    }

    TextMeshProUGUI CriarTexto(string identificador, TMP_FontAsset fonte, float tamanho, float y, float altura)
    {
        var objeto = new GameObject(identificador, typeof(RectTransform), typeof(TextMeshProUGUI));
        objeto.layer = gameObject.layer;
        objeto.transform.SetParent(transform, false);
        var texto = objeto.GetComponent<TextMeshProUGUI>();
        texto.font = fonte;
        texto.fontSize = tamanho;
        texto.enableAutoSizing = false;
        texto.textWrappingMode = TextWrappingModes.Normal;
        texto.color = new Color(.94f, .87f, .68f);
        texto.raycastTarget = false;
        texto.rectTransform.anchorMin = texto.rectTransform.anchorMax = texto.rectTransform.pivot = new Vector2(0, 1);
        texto.rectTransform.anchoredPosition = new Vector2(20, -y);
        texto.rectTransform.sizeDelta = new Vector2(690, altura);
        return texto;
    }

    public void Mostrar(string falante, string texto)
    {
        nome.text = falante;
        fala.text = texto;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        var r = rectTransform.rect;
        // Corpo retangular e triângulo apontando para baixo à esquerda.
        vh.AddVert(new Vector3(r.xMin, r.yMin), color, Vector2.zero);
        vh.AddVert(new Vector3(r.xMin, r.yMax), color, Vector2.zero);
        vh.AddVert(new Vector3(r.xMax, r.yMax), color, Vector2.zero);
        vh.AddVert(new Vector3(r.xMax, r.yMin), color, Vector2.zero);
        vh.AddTriangle(0, 1, 2); vh.AddTriangle(2, 3, 0);
        vh.AddVert(new Vector3(r.xMin + 45, r.yMin), color, Vector2.zero);
        vh.AddVert(new Vector3(r.xMin + 45, r.yMin - 18), color, Vector2.zero);
        vh.AddVert(new Vector3(r.xMin + 75, r.yMin), color, Vector2.zero);
        vh.AddTriangle(4, 5, 6);
    }
}
