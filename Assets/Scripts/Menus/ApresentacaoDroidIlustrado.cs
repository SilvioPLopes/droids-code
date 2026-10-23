using System;
using System.Collections.Generic;
using DroidsCode.DroidCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Consulta de peças e técnicas; instalar/remover pertence ao terminal.</summary>
public sealed class ApresentacaoDroidIlustrado : MonoBehaviour
{
    static readonly TipoDePeca[] Slots = { TipoDePeca.Cabeca, TipoDePeca.Braco, TipoDePeca.Tronco, TipoDePeca.Perna };
    static readonly string[] Rotulos = { "Cabeça", "Braço", "Tronco", "Perna" };
    static readonly TipoAtributo[] Atributos = { TipoAtributo.For, TipoAtributo.Agi, TipoAtributo.Vit, TipoAtributo.Int, TipoAtributo.Dex, TipoAtributo.Luk };
    static readonly string[] Nomes = { "Força", "Agilidade", "Vitalidade", "Inteligência", "Destreza", "Sorte" };
    TelaDeDroidManager dono; TMP_FontAsset fonte; Droid droid; RectTransform foco;
    readonly Image[] iconesEquipados = new Image[4], iconesEstoque = new Image[8];
    readonly TextMeshProUGUI[] vazios = new TextMeshProUGUI[4], valores = new TextMeshProUGUI[6], quantidades = new TextMeshProUGUI[8], nomesTecnicas = new TextMeshProUGUI[8];
    readonly Button[] botoesEstoque = new Button[8], botoesTecnicas = new Button[8];
    readonly List<DefinicaoDeItem> estoque = new List<DefinicaoDeItem>();
    readonly List<string> tecnicas = new List<string>();
    TextMeshProUGUI detalhe, saldo, paginaEstoque, paginaTecnicas; ScrollRect scrollDetalhe;
    int paginaPecas, paginaAcoes;

    public static ApresentacaoDroidIlustrado Criar(TelaDeDroidManager controlador)
    {
        var pai = controlador.painel.transform;
        foreach (Transform filho in pai) filho.gameObject.SetActive(false);
        foreach (var b in pai.GetComponents<Behaviour>())
            if (b is Image || b is LayoutGroup || b is ContentSizeFitter || b is ScrollRect || b is Mask || b is RectMask2D) b.enabled = false;
        var go = new GameObject("DroidIlustrado", typeof(RectTransform), typeof(ApresentacaoDroidIlustrado)); go.transform.SetParent(pai, false);
        var a = go.GetComponent<ApresentacaoDroidIlustrado>(); a.dono = controlador;
        a.fonte = controlador.textoBraco == null ? TMP_Settings.defaultFontAsset : controlador.textoBraco.font;
        a.Montar(); a.AtualizarEscala(); return a;
    }
    RectTransform Caixa(Transform pai, string nome, float x, float y, float w, float h)
    {
        var r = new GameObject(nome, typeof(RectTransform)).GetComponent<RectTransform>(); r.SetParent(pai, false);
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1); r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
    }
    Image Imagem(Transform pai, string nome, float x, float y, float w, float h, Sprite sprite = null)
    {
        var i = Caixa(pai, nome, x, y, w, h).gameObject.AddComponent<Image>(); i.sprite = sprite;
        i.preserveAspect = sprite != null; i.raycastTarget = false; return i;
    }
    TextMeshProUGUI Texto(Transform pai, string nome, float x, float y, float w, float h, string valor, float tamanho = 30, bool papel = false)
    {
        var t = Caixa(pai, nome, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>(); t.font = fonte;
        t.text = valor; t.fontSize = tamanho; t.enableAutoSizing = false; t.raycastTarget = false;
        t.color = papel ? new Color(.18f, .14f, .09f) : new Color(.96f, .92f, .77f);
        t.alignment = TextAlignmentOptions.MidlineLeft; t.textWrappingMode = TextWrappingModes.Normal; return t;
    }
    Button Botao(string nome, float x, float y, float w, float h, string rotulo = "")
    {
        var i = Imagem(transform, nome, x, y, w, h); i.raycastTarget = true; i.canvasRenderer.cullTransparentMesh = false;
        var b = i.gameObject.AddComponent<Button>(); b.targetGraphic = i; var c = b.colors;
        c.normalColor = Color.clear; c.highlightedColor = new Color(1, .8f, .4f, .16f); c.selectedColor = c.highlightedColor;
        c.pressedColor = new Color(1, .8f, .4f, .3f); b.colors = c;
        if (rotulo.Length > 0) Texto(b.transform, "Rotulo", 0, 0, w, h, rotulo).alignment = TextAlignmentOptions.Center;
        return b;
    }
    void Montar()
    {
        foco = Imagem(transform, "FundoFoco", 0, 0, 1536, 1024).rectTransform;
        foco.GetComponent<Image>().color = new Color(.035f, .04f, .035f, .98f); foco.GetComponent<Image>().raycastTarget = true;
        Imagem(transform, "ArteV2", 0, 0, 1536, 1024, dono.arteDroid);
        Texto(transform, "Titulo", 595, 25, 350, 53, "APOLLO · DROID", 40).alignment = TextAlignmentOptions.Center;
        var corpo = Caixa(transform, "CompositorApollo", 323, 215, 273, 278);
        Imagem(corpo, "CorpoBase", 0, 0, 273, 278, dono.corpoApollo);
        // Gabarito comum reservado; ícones do inventário não são sprites vestíveis.
        foreach (var camada in new[] { "Perna", "Tronco", "Braco", "Cabeca" })
            Imagem(corpo, "Camada" + camada, 0, 0, 273, 278).enabled = false;
        var posicoes = new[] { new Vector2(404, 113), new Vector2(192, 248), new Vector2(619, 248), new Vector2(619, 383) };
        for (int n = 0; n < 4; n++)
        {
            int indice = n; var p = posicoes[n]; var b = Botao("Equipado" + Slots[n], p.x, p.y, 96, 94);
            iconesEquipados[n] = Imagem(b.transform, "Icone", 14, 5, 68, 60);
            vazios[n] = Texto(b.transform, "Vazio", 0, 17, 96, 40, "Vazio", 25); vazios[n].alignment = TextAlignmentOptions.Center;
            Texto(b.transform, "Slot", 0, 67, 96, 24, Rotulos[n], 22).alignment = TextAlignmentOptions.Center;
            b.onClick.AddListener(() => InspecionarSlot(indice));
        }
        float[] ys = { 115, 208, 271, 333, 395, 457 };
        for (int n = 0; n < 6; n++)
        {
            Texto(transform, "Atributo" + n, 834, ys[n], 410, 47, Nomes[n], 34);
            valores[n] = Texto(transform, "Total" + n, 1286, ys[n], 102, 47, "", 34); valores[n].alignment = TextAlignmentOptions.Right;
        }
        paginaEstoque = Texto(transform, "PaginaPecas", 184, 577, 465, 35, "", 27); paginaEstoque.alignment = TextAlignmentOptions.Center;
        paginaTecnicas = Texto(transform, "PaginaTecnicas", 879, 577, 455, 35, "", 27); paginaTecnicas.alignment = TextAlignmentOptions.Center;
        Botao("PecasAnterior", 110, 577, 45, 35, "‹").onClick.AddListener(() => MudarPaginaPecas(-1));
        Botao("PecasProxima", 663, 577, 45, 35, "›").onClick.AddListener(() => MudarPaginaPecas(1));
        Botao("TecnicasAnterior", 811, 577, 45, 35, "‹").onClick.AddListener(() => MudarPaginaTecnicas(-1));
        Botao("TecnicasProxima", 1360, 577, 45, 35, "›").onClick.AddListener(() => MudarPaginaTecnicas(1));
        for (int n = 0; n < 8; n++)
        {
            int indice = n; float y = 639 + n / 4 * 108;
            var b = Botao("PecaDisponivel" + n, 110 + n % 4 * 163, y, 124, 76); botoesEstoque[n] = b;
            iconesEstoque[n] = Imagem(b.transform, "Icone", 12, 0, 100, 64);
            quantidades[n] = Texto(b.transform, "Quantidade", 72, 52, 50, 24, "", 23); quantidades[n].alignment = TextAlignmentOptions.Right;
            b.onClick.AddListener(() => InspecionarDisponivel(indice));
            var t = Botao("Tecnica" + n, 809 + n % 4 * 163, y, 124, 76); botoesTecnicas[n] = t;
            nomesTecnicas[n] = Texto(t.transform, "Nome", 5, 0, 114, 76, "", 24);
            nomesTecnicas[n].alignment = TextAlignmentOptions.Center;
            t.onClick.AddListener(() => InspecionarTecnica(indice));
        }
        var viewport = Caixa(transform, "Inspecao", 106, 891, 830, 66); viewport.gameObject.AddComponent<RectMask2D>();
        var captura = viewport.gameObject.AddComponent<Image>(); captura.color = new Color(0, 0, 0, .01f);
        scrollDetalhe = viewport.gameObject.AddComponent<ScrollRect>(); scrollDetalhe.horizontal = false;
        scrollDetalhe.movementType = ScrollRect.MovementType.Clamped; scrollDetalhe.scrollSensitivity = 30;
        detalhe = Texto(viewport, "Texto", 0, 0, 814, 66, "", 28, true); detalhe.alignment = TextAlignmentOptions.TopLeft;
        scrollDetalhe.viewport = viewport; scrollDetalhe.content = detalhe.rectTransform;
        saldo = Texto(transform, "Gold", 1005, 898, 167, 48, "", 29);
        Botao("Voltar", 1230, 898, 199, 53, "Voltar").onClick.AddListener(dono.Fechar);
    }
    void MostrarDetalhe(string texto)
    {
        detalhe.text = texto; detalhe.rectTransform.sizeDelta = new Vector2(814, Mathf.Max(66, detalhe.GetPreferredValues(texto, 814, Mathf.Infinity).y));
        scrollDetalhe.verticalNormalizedPosition = 1;
    }
    public void Atualizar()
    {
        var estado = GerenciadorDeEstado.Instancia; droid = estado.DroidDoJogador;
        for (int n = 0; n < 4; n++)
        {
            var p = droid.ObterPeca(Slots[n]); var sprite = p == null ? null : CatalogoVisualDeItens.Obter(p.IdItem);
            iconesEquipados[n].sprite = sprite; iconesEquipados[n].enabled = sprite != null;
            vazios[n].text = p == null ? "Vazio" : "Sem ícone"; vazios[n].enabled = sprite == null;
        }
        for (int n = 0; n < 6; n++) valores[n].text = droid.ObterTotal(Atributos[n]).ToString();
        estoque.Clear(); foreach (var kv in estado.Inventario)
        {
            var item = CatalogoDeItens.Obter(kv.Key); if (kv.Value > 0 && item != null && item.Tipo == TipoDeItem.Equipavel) estoque.Add(item);
        }
        tecnicas.Clear(); tecnicas.AddRange(droid.ObterAcoesDisponiveis());
        saldo.text = "Gold: " + estado.Gold;
        MudarPaginaPecas(0); MudarPaginaTecnicas(0);
        MostrarDetalhe("Selecione uma peça para consultar. Instale/remova pelo terminal com droid.equipar(\"id\") e droid.desequipar(\"slot\").");
    }
    public void MudarPaginaPecas(int delta)
    {
        paginaPecas = Mathf.Clamp(paginaPecas + delta, 0, Mathf.Max(0, (estoque.Count - 1) / 8));
        paginaEstoque.text = $"PEÇAS DISPONÍVEIS · {paginaPecas + 1}/{Mathf.Max(1, (estoque.Count + 7) / 8)}";
        for (int n = 0; n < 8; n++)
        {
            int i = paginaPecas * 8 + n; bool tem = i < estoque.Count; botoesEstoque[n].gameObject.SetActive(tem); if (!tem) continue;
            var item = estoque[i]; iconesEstoque[n].sprite = CatalogoVisualDeItens.Obter(item.Id); iconesEstoque[n].enabled = iconesEstoque[n].sprite != null;
            quantidades[n].text = "×" + GerenciadorDeEstado.Instancia.ObterQuantidadeDeItem(item.Id);
        }
    }
    public void MudarPaginaTecnicas(int delta)
    {
        paginaAcoes = Mathf.Clamp(paginaAcoes + delta, 0, Mathf.Max(0, (tecnicas.Count - 1) / 8));
        paginaTecnicas.text = $"TÉCNICAS · {paginaAcoes + 1}/{Mathf.Max(1, (tecnicas.Count + 7) / 8)}";
        for (int n = 0; n < 8; n++)
        {
            int i = paginaAcoes * 8 + n; bool tem = i < tecnicas.Count; botoesTecnicas[n].gameObject.SetActive(tem); if (!tem) continue;
            string nome = tecnicas[i]; nomesTecnicas[n].text = nome + "\n<size=80%>" + (nome == Droid.NomeAtaqueBasico ? "+10 energia" : droid.ObterCustoDeEnergia(nome) + " energia") + "</size>";
        }
    }
    void InspecionarSlot(int indice)
    {
        var p = droid.ObterPeca(Slots[indice]);
        MostrarDetalhe(p == null ? Rotulos[indice] + ": Vazio." : $"{Rotulos[indice]}: {p.Nome}\nID: {p.IdItem ?? "legado sem ID"} · {p.Raridade} · +{p.ValorDoBonus} {p.AtributoBonificado}\nRemover: droid.desequipar(\"{Slots[indice].ToString().ToLowerInvariant()}\")");
    }
    void InspecionarDisponivel(int indice)
    {
        int i = paginaPecas * 8 + indice; if (i >= estoque.Count) return; var item = estoque[i]; var atual = droid.ObterPeca(item.SlotDePeca.Value);
        MostrarDetalhe($"{item.Nome} · {item.Raridade} · {item.SlotDePeca}\nAtual: {(atual == null ? "Vazio" : atual.Nome + " +" + atual.ValorDoBonus + " " + atual.AtributoBonificado)} → candidato: +{item.ValorDoBonus} {item.AtributoBonificado}\n{item.Descricao}\ndroid.equipar(\"{item.Id}\")");
    }
    void InspecionarTecnica(int indice)
    {
        int i = paginaAcoes * 8 + indice; if (i >= tecnicas.Count) return; string nome = tecnicas[i];
        MostrarDetalhe(nome + "\n" + (nome == Droid.NomeAtaqueBasico ? "Ataque gratuito: recupera 10 de energia." : "Custo de uso: " + droid.ObterCustoDeEnergia(nome) + " de energia.") + " Técnicas são configuradas no terminal.");
    }
    void LateUpdate() => AtualizarEscala();
    public void AtualizarEscala()
    {
        var canvas = GetComponentInParent<Canvas>(); if (canvas == null || foco == null) return;
        var raiz = (RectTransform)canvas.rootCanvas.transform; var herdada = transform.parent.lossyScale;
        if (Mathf.Abs(herdada.x) < .00001f || Mathf.Abs(herdada.y) < .00001f) return;
        float fator = Mathf.Min(raiz.rect.width * Mathf.Abs(raiz.lossyScale.x) / 1536, raiz.rect.height * Mathf.Abs(raiz.lossyScale.y) / 1024) * .98f;
        if (fator <= 0) return;
        var r = (RectTransform)transform; r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f; r.sizeDelta = new Vector2(1536, 1024);
        r.localScale = new Vector3(fator / Mathf.Abs(herdada.x), fator / Mathf.Abs(herdada.y), 1); r.position = raiz.TransformPoint(raiz.rect.center);
        foco.anchorMin = foco.anchorMax = foco.pivot = Vector2.one * .5f; foco.anchoredPosition = Vector2.zero;
        foco.sizeDelta = new Vector2(raiz.rect.width * Mathf.Abs(raiz.lossyScale.x) / fator, raiz.rect.height * Mathf.Abs(raiz.lossyScale.y) / fator);
    }
}
