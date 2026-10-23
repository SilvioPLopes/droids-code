using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Apresentação local do terminal; não altera o Canvas nem executa código Lua.</summary>
[ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
public sealed class ApresentacaoTerminalIlustrado : MonoBehaviour
{
    public TerminalUIManager terminal;
    public TextMeshProUGUI numerosDeLinha;
    public TextMeshProUGUI estadoDaDepuracao;
    public Toggle depuracao;
    public Image luzPronto;
    public Image luzExecutando;
    public Image luzErro;
    public float margemDaTela = 0.98f;
    [Header("Recorte de exibição (não modifica a imagem)")]
    [Range(0f, 0.2f)] public float corteSuperior = 0.05f;
    [Range(0f, 0.2f)] public float corteInferior = 0.05f;
    const float Tamanho = 1254f;
    bool ajustando;
    string ultimoCodigo;

    void OnEnable()
    {
        AtualizarEscala();
        AtualizarDados();
        if (depuracao != null)
        {
            depuracao.onValueChanged.RemoveListener(AlternarDepuracao);
            depuracao.onValueChanged.AddListener(AlternarDepuracao);
        }
    }

    void OnDisable()
    {
        if (depuracao != null) depuracao.onValueChanged.RemoveListener(AlternarDepuracao);
    }

    void LateUpdate() { AtualizarEscala(); AtualizarDados(); }
    void OnRectTransformDimensionsChange() => AtualizarEscala();

    public void AtualizarEscala()
    {
        if (ajustando) return;
        var area = transform as RectTransform;
        var pai = transform.parent as RectTransform;
        var canvas = GetComponentInParent<Canvas>();
        if (area == null || pai == null || canvas == null) return;
        var raiz = canvas.rootCanvas.transform as RectTransform;
        if (raiz == null || raiz.rect.width <= 0 || raiz.rect.height <= 0) return;
        Vector3 herdada = pai.lossyScale;
        if (Mathf.Abs(herdada.x) < 0.00001f || Mathf.Abs(herdada.y) < 0.00001f) return;
        float topo = Mathf.Clamp(corteSuperior, 0f, 0.2f);
        float baseDaArte = Mathf.Clamp(corteInferior, 0f, 0.2f);
        float alturaUtil = Tamanho * (1f - topo - baseDaArte);
        float fator = Mathf.Min(raiz.rect.width * Mathf.Abs(raiz.lossyScale.x) / Tamanho,
            raiz.rect.height * Mathf.Abs(raiz.lossyScale.y) / alturaUtil) * margemDaTela;
        float lado = fator * Tamanho;
        var escala = new Vector3(lado / Tamanho / Mathf.Abs(herdada.x),
            lado / Tamanho / Mathf.Abs(herdada.y), 1f);
        ajustando = true;
        try
        {
            area.anchorMin = area.anchorMax = area.pivot = Vector2.one * 0.5f;
            area.sizeDelta = Vector2.one * Tamanho;
            if (area.localScale != escala) area.localScale = escala;
            float centroUtil = (baseDaArte - topo) * Tamanho * 0.5f;
            area.position = raiz.TransformPoint(raiz.rect.center) - area.TransformVector(new Vector3(0f, centroUtil, 0f));
        }
        finally { ajustando = false; }
    }

    void AlternarDepuracao(bool valor)
    {
        if (terminal != null) terminal.depuracaoAtiva = valor;
        AtualizarDados();
    }

    public void AtualizarDados()
    {
        if (terminal == null) return;
        if (estadoDaDepuracao != null)
            estadoDaDepuracao.text = terminal.depuracaoAtiva ? "Ligada" : "Desligada";
        if (depuracao != null && depuracao.isOn != terminal.depuracaoAtiva)
            depuracao.SetIsOnWithoutNotify(terminal.depuracaoAtiva);
        CorDaLuz(luzPronto, terminal.EstadoVisual == TerminalUIManager.EstadoDoTerminal.Pronto, new Color(0.2f, 1f, 0.25f));
        CorDaLuz(luzExecutando, terminal.EstadoVisual == TerminalUIManager.EstadoDoTerminal.Executando, new Color(1f, 0.65f, 0.05f));
        CorDaLuz(luzErro, terminal.EstadoVisual == TerminalUIManager.EstadoDoTerminal.Erro, new Color(1f, 0.12f, 0.08f));
        if (numerosDeLinha == null || terminal.campoDeCodigo == null) return;
        string codigo = terminal.campoDeCodigo.text ?? "";
        if (ultimoCodigo != codigo || string.IsNullOrEmpty(numerosDeLinha.text))
        {
            ultimoCodigo = codigo;
            int linhas = 1;
            foreach (char c in codigo) if (c == '\n') linhas++;
            var texto = new System.Text.StringBuilder();
            for (int i = 1; i <= linhas; i++) texto.Append(i).Append('\n');
            numerosDeLinha.text = texto.ToString();
        }
        // TMP desloca o texto quando o cursor sai da área; o gutter acompanha o mesmo deslocamento vertical.
        var pos = numerosDeLinha.rectTransform.anchoredPosition;
        float y = terminal.campoDeCodigo.textComponent.rectTransform.anchoredPosition.y;
        if (!Mathf.Approximately(pos.y, y)) numerosDeLinha.rectTransform.anchoredPosition = new Vector2(pos.x, y);
    }

    static void CorDaLuz(Image luz, bool acesa, Color cor)
    {
        if (luz == null) return;
        luz.color = acesa ? cor : new Color(0.015f, 0.025f, 0.015f, 0.94f);
    }
}
