using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Sem cenas, progresso ou saves: valida recursos e resolucao real do painel.
public static class ValidacaoRetratosDialogo
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly string[] NomesPrincipais = { "Apollo", "Lip", "Dara", "Senhor Tácio", "Homem da fila" };
    static readonly string[] IdsPrincipais = { "apollo", "lip", "dara", "tacio", "homem_fila" };
    sealed class SemIO : ISistemaDeSalvamento
    {
        public void Salvar() { throw new InvalidOperationException("Save proibido na fixture de retratos."); }
        public void Carregar() { throw new InvalidOperationException("Load proibido na fixture de retratos."); }
        public bool ExisteSave() { throw new InvalidOperationException("Consulta de save proibida na fixture de retratos."); }
    }
    static void Exigir(bool condicao, string erro)
    {
        if (!condicao) throw new InvalidOperationException(erro);
    }

    public static string ExecutarLogoVisual()
    {
        Exigir(!EditorApplication.isPlaying, "Exige fora de Play.");
        return RenderizarLogo(true) + "\n" + RenderizarLogo(false) +
            "\nPreview 1920x1080; sem cliques, saves ou alteracoes na cena persistente. Aprovacao visual pendente.";
    }

    static string RenderizarLogo(bool antes)
    {
        const int largura = 1920, altura = 1080;
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/MainMenu.unity");
        RenderTexture rt = null;
        Camera cam = null;
        try
        {
            var raizes = cena.GetRootGameObjects();
            var canvas = raizes.SelectMany(g => g.GetComponentsInChildren<Canvas>(true)).Single(c => c.name == "Canvas");
            var titulo = canvas.transform.Find("DroidsCode") as RectTransform;
            Exigir(titulo != null, "Canvas/DroidsCode ausente.");
            var logo = titulo.Find("Logo");
            Exigir(logo != null && logo.GetComponent<Image>().sprite != null, "Logo nao vinculada.");
            if (antes)
            {
                titulo.GetComponent<TextMeshProUGUI>().enabled = true;
                titulo.sizeDelta = new Vector2(200f, 50f);
                titulo.anchoredPosition = new Vector2(0f, 100f);
                logo.gameObject.SetActive(false);
            }
            var cameras = raizes.SelectMany(g => g.GetComponentsInChildren<Camera>(true)).ToArray();
            cam = cameras.FirstOrDefault(c => c.name == "Main Camera") ?? cameras.First();
            foreach (var camera in cameras) camera.enabled = false;
            cam.scene = cena;
            cam.aspect = (float)largura / altura;
            rt = new RenderTexture(largura, altura, 24);
            cam.targetTexture = rt;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 10f;
            Canvas.ForceUpdateCanvases();
            foreach (var texto in canvas.GetComponentsInChildren<TextMeshProUGUI>()) texto.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
            cam.Render();
            string arquivo = antes ? "logo-antes-reconstituido-preview" : "logo-depois-preview";
            var rtAnterior = RenderTexture.active;
            Texture2D imagem = null;
            try
            {
                RenderTexture.active = rt;
                imagem = new Texture2D(largura, altura, TextureFormat.RGB24, false);
                imagem.ReadPixels(new Rect(0, 0, largura, altura), 0, 0);
                imagem.Apply();
                Directory.CreateDirectory("output/portrait-sources/validacao");
                File.WriteAllBytes("output/portrait-sources/validacao/" + arquivo + ".png", imagem.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = rtAnterior;
                if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem);
            }
            Rect RetanguloNaTela(RectTransform alvo)
            {
                var cantos = new Vector3[4]; alvo.GetWorldCorners(cantos);
                var pixels = cantos.Select(p => cam.WorldToScreenPoint(p)).ToArray();
                return Rect.MinMaxRect(pixels.Min(p => p.x), pixels.Min(p => p.y), pixels.Max(p => p.x), pixels.Max(p => p.y));
            }
            var area = RetanguloNaTela(antes ? titulo : (RectTransform)logo);
            var erros = new List<string>();
            if (area.xMin < 0 || area.yMin < 0 || area.xMax > largura || area.yMax > altura)
                erros.Add("titulo/logo fora do enquadramento: " + area);
            foreach (var botao in canvas.GetComponentsInChildren<Button>())
                if (botao.gameObject.activeInHierarchy && area.Overlaps(RetanguloNaTela((RectTransform)botao.transform)))
                    erros.Add("titulo/logo sobrepoe botao " + botao.name);
            var scaler = canvas.GetComponent<CanvasScaler>();
            string config = scaler == null ? "sem Scaler" : scaler.uiScaleMode + ", referencia=" + scaler.referenceResolution + ", match=" + scaler.matchWidthOrHeight;
            return (erros.Count == 0 ? "PASS " : "FAIL ") + arquivo + ": " + config + ", escala=" + canvas.scaleFactor + ", area=" + area +
                (erros.Count == 0 ? "; enquadrado e sem sobreposicao aos botoes ativos." : "; " + string.Join("; ", erros));
        }
        finally
        {
            if (cam != null) cam.targetTexture = null;
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena);
        }
    }

    public static string ExecutarVisual()
    {
        Exigir(!EditorApplication.isPlaying, "Exige fora de Play.");
        var resultados = new List<string>();
        resultados.Add(Renderizar("Nilce", "estalajadeira", "nilce-antes-reconstituido", true));
        resultados.Add(Renderizar("Nilce", "estalajadeira", "nilce-depois", false));
        resultados.Add(Renderizar("Crianca", "fv_crianca_sula_02", "crianca-02-depois", false));
        resultados.Add(Renderizar("Crianca", "fv_crianca_sula_03", "crianca-03-depois", false));
        return string.Join("\n", resultados) + "\nCliques testados por Button.onClick; input real e aprovacao visual pendentes. Antes reconstituido apenas na copia de preview, sem saves/cenas persistentes.";
    }

    public static string ExecutarPrincipaisVisual()
    {
        Exigir(!EditorApplication.isPlaying, "Exige fora de Play.");
        var resultados = new List<string>();
        for (int i = 0; i < NomesPrincipais.Length; i++)
        {
            resultados.Add(Renderizar(NomesPrincipais[i], IdsPrincipais[i], "principais-" + IdsPrincipais[i] + "-antes", false, true));
            resultados.Add(Renderizar(NomesPrincipais[i], IdsPrincipais[i], "principais-" + IdsPrincipais[i] + "-depois", false));
        }
        return string.Join("\n", resultados) + "\nAntes usa override legado real da City, ou recurso original quando nao ha override. Somente preview; sem saves, input real/aprovacao visual pendentes.";
    }

    public static string VerificarPrincipaisNasCenas()
    {
        Exigir(!EditorApplication.isPlaying, "Exige fora de Play.");
        var resultados = new List<string>();
        var resolver = typeof(DialogoUIManager).GetMethod("ObterRetrato", Privado);
        foreach (var caminho in new[] { "Assets/Scenes/CasaLip.unity", "Assets/Scenes/City.unity", "Assets/Scenes/Game.unity" })
        {
            var cena = EditorSceneManager.OpenPreviewScene(caminho);
            try
            {
                var paineis = cena.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<DialogoUIManager>(true)).ToArray();
                Exigir(paineis.Length > 0, "Sem DialogoUIManager em " + caminho);
                foreach (var ui in paineis)
                    for (int i = 0; i < 2; i++)
                    {
                        var esperado = Resources.Load<Sprite>("Retratos/Dialogo/" + NomesPrincipais[i]);
                        Exigir(esperado != null, "Dedicado ausente: " + NomesPrincipais[i]);
                        foreach (var id in new[] { "", IdsPrincipais[i] })
                            Exigir((Sprite)resolver.Invoke(ui, new object[] { NomesPrincipais[i], id }) == esperado,
                                "Override antigo prevaleceu: " + caminho + "/" + NomesPrincipais[i] + " id=" + id);
                    }
                resultados.Add("PASS " + caminho + ": Apollo/Lip por nome e ID usam dedicados acima dos overrides legados.");
            }
            finally { EditorSceneManager.ClosePreviewScene(cena); }
        }
        return string.Join("\n", resultados) + "\nConsulta em previews; cenas persistentes intactas.";
    }

    static string Renderizar(string nome, string id, string arquivo, bool antes, bool retratoLegado = false)
    {
        const int largura = 1920, altura = 1080;
        var singletonEstado = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var singletonDialogo = typeof(DialogoUIManager).GetField("_instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var estadoAnterior = singletonEstado.GetValue(null);
        var dialogoAnterior = singletonDialogo.GetValue(null);
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/City.unity");
        RenderTexture rt = null;
        Camera cam = null;
        try
        {
            var raizes = cena.GetRootGameObjects();
            var ui = raizes.SelectMany(g => g.GetComponentsInChildren<DialogoUIManager>(true)).Single();
            var canvas = ui.GetComponentInParent<Canvas>(true);
            Exigir(canvas != null && ui.imagemRetrato != null, "Canvas/retrato real nao encontrado na City.");
            var scaler = canvas.GetComponent<CanvasScaler>();
            string config = scaler == null ? "sem Scaler" : scaler.uiScaleMode + ", referencia=" + scaler.referenceResolution + ", match=" + scaler.matchWidthOrHeight;

            // A preview e uma copia da cena; somente a arvore do dialogo fica visivel.
            foreach (var raiz in raizes) raiz.SetActive(raiz == canvas.transform.root.gameObject);
            Transform ramo = ui.transform;
            while (ramo != canvas.transform)
            {
                ramo.gameObject.SetActive(true);
                foreach (Transform irmao in ramo.parent)
                    if (irmao != ramo) irmao.gameObject.SetActive(false);
                ramo = ramo.parent;
            }
            canvas.gameObject.SetActive(true);
            foreach (Transform tr in canvas.GetComponentsInChildren<Transform>(true)) tr.gameObject.layer = 30;

            var estadoGo = new GameObject("EstadoFixtureRetratos", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(estadoGo, cena);
            var estado = estadoGo.GetComponent<GerenciadorDeEstado>();
            singletonEstado.SetValue(null, estado);
            singletonDialogo.SetValue(null, ui);
            typeof(DialogoUIManager).GetField("_salvamento", Privado).SetValue(ui, new SemIO());

            var cameraGo = new GameObject("CameraFixtureRetratos", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraGo, cena);
            cam = cameraGo.GetComponent<Camera>();
            cam.scene = cena;
            cam.enabled = false;
            cam.orthographic = true;
            cam.orthographicSize = altura / 2f;
            cam.aspect = (float)largura / altura;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.12f, .13f, .12f);
            cam.cullingMask = 1 << 30;
            rt = new RenderTexture(largura, altura, 24);
            cam.targetTexture = rt;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 10f;
            Canvas.ForceUpdateCanvases();
            foreach (var escala in canvas.GetComponentsInChildren<EscalaLocalDeDialogo>(true)) escala.AtualizarEscala();
            Canvas.ForceUpdateCanvases();

            var conversar = new EscolhaDeDialogo { texto = "Conversar" };
            var voltar = new EscolhaDeDialogo { texto = "Até mais" };
            EscolhaDeDialogo recebida = null;
            bool abriu = ui.Mostrar(new[] { new FalaDeDialogo { falante = nome, idRetrato = id, texto = "O que você precisa?" } },
                new[] { conversar, voltar }, e => recebida = e);
            Exigir(abriu && ui.EstaAberto && estado.MenuAberto, "Dialogo nao abriu no estado isolado.");
            ui.Avancar();
            var botoes = ui.painelDeEscolhas.GetComponentsInChildren<Button>();
            Exigir(botoes.Length == 2, "Esperava duas escolhas.");
            Exigir(ui.imagemRetrato.sprite != null, "Retrato ausente para " + id);
            if (retratoLegado)
            {
                var manual = ui.retratosConhecidos.FirstOrDefault(r => string.Equals(r.nomeDoPersonagem, nome, StringComparison.OrdinalIgnoreCase));
                ui.imagemRetrato.sprite = manual != null ? manual.retrato : Resources.Load<Sprite>("Retratos/" + nome);
                Exigir(ui.imagemRetrato.sprite != null, "Retrato legado ausente para " + nome);
                ui.imagemRetrato.gameObject.SetActive(true);
            }
            if (antes)
            {
                // Estado anterior comprovado: Nilce sem recurso e escolhas com estilo do template.
                ui.imagemRetrato.gameObject.SetActive(false);
                foreach (var botao in botoes)
                {
                    var fundo = botao.GetComponent<Image>();
                    var original = ui.prefabBotaoEscolha.GetComponent<Image>();
                    fundo.sprite = original.sprite; fundo.type = original.type; fundo.color = original.color;
                    botao.colors = ui.prefabBotaoEscolha.colors;
                    var borda = botao.GetComponent<Outline>();
                    if (borda != null) borda.enabled = false;
                    foreach (Transform filho in botao.transform)
                        if (filho.name.StartsWith("BordaEscolha", StringComparison.Ordinal)) filho.gameObject.SetActive(false);
                    var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
                    texto.enableAutoSizing = false; texto.fontSize = 44f;
                    texto.color = new Color(.08f, .06f, .04f, 1f);
                    texto.margin = ui.prefabBotaoEscolha.GetComponentInChildren<TextMeshProUGUI>(true).margin;
                }
            }
            foreach (Transform tr in canvas.GetComponentsInChildren<Transform>(true)) tr.gameObject.layer = 30;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)ui.painelDeEscolhas);
            foreach (var texto in ui.painel.GetComponentsInChildren<TextMeshProUGUI>()) texto.ForceMeshUpdate();
            foreach (var botao in botoes)
            {
                Exigir(!botao.GetComponentInChildren<TextMeshProUGUI>().isTextTruncated, "Escolha cortada.");
                var cantos = new Vector3[4];
                ((RectTransform)botao.transform).GetWorldCorners(cantos);
                foreach (var canto in cantos)
                {
                    var p = cam.WorldToScreenPoint(canto);
                    Exigir(p.x >= 0 && p.y >= 0 && p.x <= largura && p.y <= altura, "Escolha fora da tela.");
                }
            }
            cam.Render();
            var rtAnterior = RenderTexture.active;
            Texture2D imagem = null;
            try
            {
                RenderTexture.active = rt;
                imagem = new Texture2D(largura, altura, TextureFormat.RGB24, false);
                imagem.ReadPixels(new Rect(0, 0, largura, altura), 0, 0);
                imagem.Apply();
                Directory.CreateDirectory("output/portrait-sources/validacao");
                File.WriteAllBytes("output/portrait-sources/validacao/" + arquivo + ".png", imagem.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = rtAnterior;
                if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem);
            }
            // Retira os clones do container antes do callback: Fechar usa Destroy de runtime.
            // A preview fecha e descarta estes objetos sem tocar assets ou progresso.
            foreach (var botao in botoes) botao.transform.SetParent(canvas.transform, false);
            botoes[0].onClick.Invoke();
            Exigir(recebida == conversar && !ui.EstaAberto && !estado.MenuAberto, "Callback/fechamento da escolha falhou.");
            return "PASS " + arquivo + ": 1920x1080, " + config + ", escala efetiva=" + canvas.scaleFactor + ", callback sem consequencias/saves.";
        }
        finally
        {
            if (cam != null) cam.targetTexture = null;
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena);
            singletonEstado.SetValue(null, estadoAnterior);
            singletonDialogo.SetValue(null, dialogoAnterior);
        }
    }

    public static string Executar()
    {
        var erros = new List<string>();
        foreach (var npc in CatalogoDeNpcs.Todos.Values)
            if (Resources.Load<Sprite>("Retratos/" + npc.Nome) == null)
                erros.Add("Retrato ausente: " + npc.Nome);
        var ids = new[] { "fv_cientista_01", "fv_crianca_sula_02", "fv_crianca_sula_03", "fv_guarda_01", "fv_hacker_01", "fv_idoso_01", "fv_mecanico_01", "fv_operario_01", "fv_vendedor_01", "fv_vizinha_dara_01" };
        foreach (var id in ids)
            if (Resources.Load<Sprite>("Retratos/" + id) == null) erros.Add("Retrato ausente: " + id);
        if (Resources.Load<Sprite>("Retratos/Lip") == null) erros.Add("Lip ausente");
        foreach (var nome in NomesPrincipais)
            if (Resources.Load<Sprite>("Retratos/Dialogo/" + nome) == null) erros.Add("Retrato dedicado ausente: " + nome);
        var campo = typeof(FalaDeDialogo).GetField("idRetrato");
        if (campo == null) erros.Add("Fala nao transporta identidade: duas Crianca continuam ambiguas");
        if (erros.Count > 0) return "FAIL\n" + string.Join("\n", erros);

        var go = new GameObject("FixtureRetratos");
        go.SetActive(false); // nao registra singleton via Awake.
        try
        {
            var ui = go.AddComponent<DialogoUIManager>();
            var resolver = typeof(DialogoUIManager).GetMethod("ObterRetrato", BindingFlags.Instance | BindingFlags.NonPublic);
            Sprite Resolver(string nome, string id) => (Sprite)resolver.Invoke(ui, new object[] { nome, id });
            var a = Resolver("Crianca", ids[1]);
            var b = Resolver("Crianca", ids[2]);
            if (a == null || b == null || a == b) erros.Add("Criancas nao possuem retratos distintos");
            if (Resolver("Nilce", "estalajadeira") != Resources.Load<Sprite>("Retratos/Nilce")) erros.Add("Fallback por nome falhou");
            for (int i = 0; i < NomesPrincipais.Length; i++)
            {
                string nome = NomesPrincipais[i];
                var esperado = Resources.Load<Sprite>("Retratos/Dialogo/" + nome);
                // Simula explicitamente o override antigo ainda serializado nas cenas.
                ui.retratosConhecidos.Add(new RetratoPersonagem { nomeDoPersonagem = nome, retrato = Resources.Load<Sprite>("Retratos/Lip") });
                if (Resolver(nome, "") != esperado) erros.Add("Override antigo prevaleceu sobre dedicado: " + nome);
                if (Resolver("Nilce", IdsPrincipais[i]) != esperado) erros.Add("ID principal perdeu prioridade: " + nome);
            }
            foreach (var alias in new[] { "Senhor Tacio", "Tácio", "Tacio" })
                if (Resolver(alias, "") != Resources.Load<Sprite>("Retratos/Dialogo/Senhor Tácio")) erros.Add("Alias de Tacio falhou: " + alias);
            if (Resolver("Apollo", ids[1]) != a) erros.Add("ID de crianca foi substituido por nome principal");
            if (Resolver("Apollo", "id_inexistente") == Resources.Load<Sprite>("Retratos/Dialogo/Apollo")) erros.Add("ID desconhecido promoveu dedicado pelo nome");
            if (Resolver("Nadir", "") != Resources.Load<Sprite>("Retratos/Nadir Quintela")) erros.Add("Alias narrativo de Nadir nao resolve a identidade completa");
            if (Resolver("", "") != null) erros.Add("Narracao ganhou personagem");
            if (Resolver("NaoExiste", "id_inexistente") != null) erros.Add("ID desconhecido ganhou personagem");
            var morador = go.AddComponent<MoradorInterativo>();
            morador.idNpc = ids[1]; morador.nome = "Crianca";
            var falas = (List<FalaDeDialogo>)typeof(MoradorInterativo).GetMethod("Fala", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(morador, new object[] { "Teste" });
            if ((string)campo.GetValue(falas[0]) != ids[1] || falas[0].falante != "Crianca") erros.Add("Morador perdeu ID ou alterou nome");
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
        return erros.Count == 0 ? "PASS: 27 identidades, cinco dedicados, precedencia sobre overrides, IDs distintos, aliases, fallback, narracao e Morador; sem saves" : "FAIL\n" + string.Join("\n", erros);
    }
}
