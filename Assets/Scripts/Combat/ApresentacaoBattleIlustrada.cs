using DroidsCode.DroidCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Somente apresentação da Battle. Reutiliza os controles e o fluxo do BattleManager.</summary>
[ExecuteAlways, DisallowMultipleComponent]
public sealed class ApresentacaoBattleIlustrada : MonoBehaviour
{
    public BattleManager batalha;
    public Image energia;
    public TextMeshProUGUI nomePlayer, nomeInimigo, hpPlayer, hpInimigo, textoEnergia;
    public ScrollRect log, ataques, itens;
    public GameObject quadroAtaques, quadroItens;
    public Sprite molduraLista;
    public Vector2 referencia = new Vector2(1672, 941);
    bool ajustando;
    string ultimoLog;

    void OnEnable() => AtualizarLayout();
    void LateUpdate()
    {
        AtualizarLayout();
        if (Application.isPlaying && batalha != null)
        {
            var d = GerenciadorDeEstado.Instancia.DroidDoJogador;
            if (d != null) AtualizarHud(d, batalha.nomeInimigo,
                Mathf.RoundToInt(batalha.sliderInimigo.value * batalha.hpMaxInimigo), batalha.hpMaxInimigo);
        }
        AtualizarConteudo();
    }

    public void AtualizarLayout()
    {
        if (ajustando || transform.parent == null) return;
        var pai = transform.parent as RectTransform;
        var area = transform as RectTransform;
        if (pai == null || area == null || pai.rect.width <= 0 || pai.rect.height <= 0) return;
        ajustando = true;
        try
        {
            float fator = Mathf.Min(pai.rect.width / referencia.x, pai.rect.height / referencia.y);
            area.anchorMin = area.anchorMax = area.pivot = Vector2.one * .5f;
            area.anchoredPosition = Vector2.zero;
            area.sizeDelta = referencia;
            area.localScale = new Vector3(fator, fator, 1);
        }
        finally { ajustando = false; }
    }

    public void AtualizarHud(Droid droid, string inimigo, int hpAtual, int hpMax)
    {
        nomePlayer.text = string.IsNullOrWhiteSpace(droid.Nome) || droid.Nome == "Heroi" ? "APOLLO" : droid.Nome;
        nomeInimigo.text = inimigo;
        hpPlayer.text = $"HP  {droid.Hp}/{droid.HpMax}";
        hpInimigo.text = $"HP  {hpAtual}/{hpMax}";
        textoEnergia.text = $"ENERGIA  {droid.EnergiaAtual}/{droid.EnergiaMaxima}";
        var r = energia.rectTransform;
        r.anchorMax = new Vector2(droid.EnergiaMaxima > 0 ? Mathf.Clamp01((float)droid.EnergiaAtual / droid.EnergiaMaxima) : 0, 1);
        r.offsetMin = r.offsetMax = Vector2.zero;
    }

    public void AtualizarConteudo()
    {
        if (batalha == null) return;
        AtualizarLista(batalha.painelListaDeAtaques, quadroAtaques, ataques);
        AtualizarLista(batalha.painelListaDeItens, quadroItens, itens);
        if (log == null || batalha.textoMensagem == null) return;
        var t = batalha.textoMensagem;
        if (ultimoLog == t.text) return;
        ultimoLog = t.text;
        float h = t.GetPreferredValues(t.text, log.viewport.rect.width, Mathf.Infinity).y;
        t.rectTransform.sizeDelta = new Vector2(0, Mathf.Max(log.viewport.rect.height, h + 8));
        Canvas.ForceUpdateCanvases();
        log.verticalNormalizedPosition = 0;
    }

    void AtualizarLista(Transform conteudo, GameObject quadro, ScrollRect rolagem)
    {
        if (conteudo == null || quadro == null || rolagem == null) return;
        bool abrindo = conteudo.gameObject.activeSelf && !quadro.activeSelf;
        quadro.SetActive(conteudo.gameObject.activeSelf);
        if (!quadro.activeSelf) return;
        float largura = rolagem.viewport.rect.width - 20;
        foreach (Transform filho in conteudo)
        {
            var b = filho.GetComponent<Button>();
            if (b == null) continue;
            var t = b.GetComponentInChildren<TextMeshProUGUI>(true);
            if (t == null) continue;
            var imagem = b.GetComponent<Image>();
            if (imagem != null) { imagem.sprite = molduraLista; imagem.color = Color.white; imagem.type = Image.Type.Simple; }
            var cores = b.colors;
            cores.normalColor = Color.white; cores.highlightedColor = new Color(1, .86f, .62f);
            cores.pressedColor = new Color(.65f, .55f, .38f); cores.selectedColor = new Color(1, .86f, .62f);
            cores.disabledColor = new Color(.45f, .45f, .45f); cores.colorMultiplier = 1; b.colors = cores;
            t.fontSize = 32; t.enableAutoSizing = false; t.fontStyle = FontStyles.Normal;
            t.color = new Color(.96f, .91f, .76f); t.alignment = TextAlignmentOptions.MidlineLeft;
            t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Overflow; t.raycastTarget = false;
            t.rectTransform.anchorMin = Vector2.zero; t.rectTransform.anchorMax = Vector2.one;
            t.rectTransform.offsetMin = new Vector2(36, 16); t.rectTransform.offsetMax = new Vector2(-36, -16);
            var layout = b.GetComponent<LayoutElement>();
            if (layout == null) layout = b.gameObject.AddComponent<LayoutElement>();
            float altura = Mathf.Max(88, t.GetPreferredValues(t.text, largura - 72, Mathf.Infinity).y + 36);
            if (!Mathf.Approximately(layout.preferredHeight, altura)) layout.preferredHeight = altura;
            layout.flexibleHeight = 0;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)conteudo);
        if (abrindo) rolagem.verticalNormalizedPosition = 1;
    }
}
