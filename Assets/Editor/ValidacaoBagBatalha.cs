using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Contrato de contexto antes da montagem, sem consumir itens ou acessar saves.</summary>
public static class ValidacaoBagBatalha
{
    const BindingFlags Privado = BindingFlags.Instance | BindingFlags.NonPublic;
    sealed class SemIO : ISistemaDeSalvamento
    {
        public int Chamadas;
        public void Salvar() { Chamadas++; throw new InvalidOperationException("Save indevido na Bag de combate."); }
        public void Carregar() { Chamadas++; throw new InvalidOperationException("Load indevido na Bag de combate."); }
        public bool ExisteSave() { Chamadas++; throw new InvalidOperationException("Consulta indevida de save."); }
    }
    static void Exigir(bool ok, string mensagem) { if (!ok) throw new InvalidOperationException(mensagem); }
    public static string VerificarContrato()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Exige fora de Play.");
        if (typeof(BagUIManager).GetProperty("EmBatalha") == null ||
            typeof(BagUIManager).GetMethod("MostrarEmBatalha") == null ||
            typeof(BattleManager).GetMethod("TentarUsarItemDaBag") == null)
            throw new InvalidOperationException("Flag/abertura/confirmacao de Bag em batalha ainda ausentes.");
        return "OK: contrato de contexto da Bag disponível.";
    }

    public static string Executar(int largura, int altura)
    {
        VerificarContrato();
        var singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var anterior = singleton.GetValue(null);
        var cena = EditorSceneManager.OpenPreviewScene("Assets/Scenes/Battle.unity");
        RenderTexture rt = null; BattleManager b = null;
        try
        {
            var raizes = cena.GetRootGameObjects();
            b = raizes.SelectMany(g => g.GetComponentsInChildren<BattleManager>(true)).Single();
            var bag = b.bagDeBatalha; Exigir(bag != null && bag.arteBag != null, "Bag local/arte não montadas.");
            var io = new SemIO(); typeof(BagUIManager).GetField("_salvamento", Privado).SetValue(bag, io);
            var go = new GameObject("EstadoFixtureBagBatalha", typeof(GerenciadorDeEstado));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena);
            var estado = go.GetComponent<GerenciadorDeEstado>();
            var d = new Droid("Heroi", 30); d.Hp = 10;
            typeof(GerenciadorDeEstado).GetField("<DroidDoJogador>k__BackingField", Privado).SetValue(estado, d);
            singleton.SetValue(null, estado);
            var inventario = CatalogoDeItens.Todos.Values.ToList();
            estado.CarregarInventario(inventario.ToDictionary(i => i.Id, i => 3));
            var snapshot = estado.Inventario.ToDictionary(x => x.Key, x => x.Value);
            b.hpMaxInimigo = 9999; b.nomeCenaMundo = "";
            typeof(BattleManager).GetMethod("Start", Privado).Invoke(b, null);
            var turno = typeof(BattleManager).GetField("turnoDoJogador", Privado);
            // Estabelecer a geometria da câmera/Canvas antes de criar o ScrollRect da Bag.
            var cam = raizes.SelectMany(g => g.GetComponentsInChildren<Camera>(true)).First();
            cam.scene = cena; cam.enabled = false; cam.orthographic = true;
            cam.orthographicSize = altura / 2f; cam.aspect = (float)largura / altura;
            cam.transform.position = new Vector3(0, 0, -10);
            rt = new RenderTexture(largura, altura, 24); cam.targetTexture = rt;
            var canvas = b.botaoItem.GetComponentInParent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = cam; canvas.planeDistance = 10;
            Canvas.ForceUpdateCanvases();
            b.botaoItem.onClick.Invoke();
            Exigir(bag.EmBatalha && bag.EstaAberta, "Flag não ativada pela batalha.");
            var a = bag.painel.GetComponentInChildren<ApresentacaoBagIlustrada>();
            Exigir(a != null, "Interface ilustrada não reutilizada.");
            // Todos os itens conhecidos com estoque, inclusive buff/debuff e proibidos, acessíveis por paginação.
            for (int n = 0; n < inventario.Count; n++)
            {
                a.MudarPagina(n / 12 - a.Pagina); a.Selecionar(n % 12);
                Exigir(a.ItemSelecionado == inventario[n].Id, "Item oculto/inacessível: " + inventario[n].Id);
            }
            Exigir(snapshot.All(x => estado.ObterQuantidadeDeItem(x.Key) == x.Value), "Seleção gastou itens.");
            Exigir((bool)turno.GetValue(b) && d.Hp == 10, "Seleção alterou HP/turno.");
            int indice = inventario.FindIndex(i => !i.UsavelEmBatalha && i.Tipo == TipoDeItem.Equipavel);
            Exigir(indice >= 0, "Sem equipamento proibido para validar.");
            a.MudarPagina(indice / 12 - a.Pagina); a.Selecionar(indice % 12);
            var acao = a.transform.Find("Acao").GetComponent<Button>();
            Exigir(acao.interactable, "Item proibido não permite consultar o aviso por Usar.");
            acao.onClick.Invoke();
            Exigir(a.Mensagem.text == "Este item não pode ser usado durante uma batalha.", "Aviso incorreto.");
            Exigir(snapshot.All(x => estado.ObterQuantidadeDeItem(x.Key) == x.Value) && d.Hp == 10 &&
                (bool)turno.GetValue(b) && bag.EstaAberta && io.Chamadas == 0, "Proibição teve efeito colateral.");
            Canvas.ForceUpdateCanvases(); a.AtualizarEscala(); Canvas.ForceUpdateCanvases();
            a.Mensagem.ForceMeshUpdate(); Exigir(!a.Mensagem.isTextTruncated, "Aviso cortado.");
            var descricao = a.GetComponentsInChildren<TMPro.TextMeshProUGUI>().First(t => t.name == "Descricao");
            descricao.ForceMeshUpdate();
            Exigir(descricao.textInfo.characterCount > 0 && Mathf.Abs(descricao.rectTransform.anchoredPosition.y) < 1,
                "Descrição inicial fora da viewport; conferir ordem de configuração da câmera/Canvas.");
            var cantos = new Vector3[4]; ((RectTransform)a.transform).GetWorldCorners(cantos);
            foreach (var canto in cantos)
            {
                var p = cam.WorldToScreenPoint(canto);
                Exigir(p.x >= -1 && p.y >= -1 && p.x <= largura + 1 && p.y <= altura + 1, "Bag fora da tela.");
            }
            cam.Render(); Salvar(rt, $"output/bag-batalha/preview-{largura}x{altura}.png");
            a.transform.Find("Voltar").GetComponent<Button>().onClick.Invoke();
            Exigir(!bag.EstaAberta && !bag.EmBatalha && (bool)turno.GetValue(b), "Voltar não limpou flag ou gastou turno.");
            b.botaoItem.onClick.Invoke();
            bag.ConfirmarItemDaBag("pocao_pequena");
            Exigir(d.Hp > 10 && estado.ObterQuantidadeDeItem("pocao_pequena") == 2 && !bag.EstaAberta,
                "Item permitido não aplicou efeito/consumo/fechamento.");
            Exigir(!(bool)turno.GetValue(b), "Uso permitido não passou o turno.");
            Exigir(!b.TentarUsarItemDaBag("pocao_pequena") && estado.ObterQuantidadeDeItem("pocao_pequena") == 2,
                "Confirmação repetida consumiu novamente.");
            b.StopAllCoroutines();
            bag.MostrarEmBatalha(b); bag.ConfirmarItemDaBag("pocao_pequena");
            Exigir(estado.ObterQuantidadeDeItem("pocao_pequena") == 2 && io.Chamadas == 0,
                "Uso fora do turno foi aceito ou acessou save.");
            bag.Fechar();
            // A mesma instância volta ao modo exploração sem vazar a flag nem habilitar consumíveis exclusivos de combate.
            bag.Mostrar();
            Exigir(!bag.EmBatalha && bag.ItemVisivel(CatalogoDeItens.Obter("pocao_pequena")), "Modo exploração não restaurado.");
            var exclusivo = inventario.First(i => !i.UsavelForaDeBatalha && i.UsavelEmBatalha);
            Exigir(!bag.ItemVisivel(exclusivo), "Item exclusivo de batalha vazou no filtro do mundo.");
            bag.Fechar();
            Exigir(io.Chamadas == 0, "Bag de combate tentou acessar saves.");
            return $"OK {largura}x{altura}: flag, {inventario.Count} itens/páginas, proibido sem efeito/turno, permitido com consumo único/turno, bloqueio fora do turno, Voltar e modo exploração; zero IO de save. Consumo apenas em estado sintético.";
        }
        finally
        {
            if (b != null) b.StopAllCoroutines();
            if (rt != null) { rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            EditorSceneManager.ClosePreviewScene(cena); singleton.SetValue(null, anterior);
        }
    }

    static void Salvar(RenderTexture rt, string caminho)
    {
        var anterior = RenderTexture.active; Texture2D imagem = null;
        try
        {
            RenderTexture.active = rt; imagem = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            imagem.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); imagem.Apply();
            Directory.CreateDirectory("output/bag-batalha"); File.WriteAllBytes(caminho, imagem.EncodeToPNG());
        }
        finally { RenderTexture.active = anterior; if (imagem != null) UnityEngine.Object.DestroyImmediate(imagem); }
    }
}
