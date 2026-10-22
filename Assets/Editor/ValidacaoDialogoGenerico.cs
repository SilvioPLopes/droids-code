using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Fixtures em cena de preview; singletons restaurados e salvamento substituído.
/// Não abre cenas do jogo, não inicia Play e não lê/grava progresso pessoal.</summary>
public static class ValidacaoDialogoGenerico
{
    const BindingFlags Campos = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static readonly FieldInfo EstadoSingleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
    static readonly FieldInfo PainelSingleton = typeof(DialogoUIManager).GetField("_instancia", BindingFlags.Static | BindingFlags.NonPublic);

    [MenuItem("Droids Code/Validacao/Dialogos genericos City")]
    public static void PeloMenu() => Debug.Log(Executar());

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlayingOrWillChangePlaymode, "Execute fora de Play.");
        var resultados = new List<string>();
        Rodar(resultados, "catalogo dos tres NPCs", f =>
        {
            foreach (var id in new[] { "city_generico_dara", "city_generico_homem_fila", "city_generico_tacio" })
            {
                var falas = CatalogoDeDialogos.Obter(id);
                Exigir(falas != null && falas.Count > 0 && !string.IsNullOrWhiteSpace(falas[0].texto), "Fala ausente: " + id);
            }
        });
        Rodar(resultados, "cache nulo nao interrompe retrato nome e fala", f =>
        {
            f.Painel.imagemRetrato = f.Objeto("Miniatura").AddComponent<UnityEngine.UI.Image>();
            f.Painel.textoFalante = f.Objeto("Nome").AddComponent<TextMeshProUGUI>();
            foreach (var nome in new[] { "Dara", "Homem da fila", "Senhor Tácio" })
            {
                Definir(f.Painel, "_cacheDeRetratosPorConvencao", null);
                f.Painel.textoFala.text = "Fala";
                Exigir(f.Painel.Mostrar(new[] { new FalaDeDialogo { falante = nome, texto = "Conversa de teste" } }), "Painel nao abriu.");
                Exigir(f.Painel.imagemRetrato.gameObject.activeSelf && f.Painel.imagemRetrato.sprite == Resources.Load<Sprite>("Retratos/" + nome)
                    && f.Painel.imagemRetrato.sprite != null, "Miniatura ausente: " + nome);
                Exigir(f.Painel.textoFalante.text == nome && f.Painel.textoFala.text == "Conversa de teste", "Nome/fala ficaram incompletos: " + nome);
                f.Painel.Fechar();
                Exigir(!f.Estado.MenuAberto, "Menu ficou preso.");
            }
        });
        Rodar(resultados, "flag bloqueada permite fala generica repetivel sem progresso", f =>
        {
            f.Gatilho.flagAoTerminar = "fixture_nao_gravar";
            f.Gatilho.desativarAposConcluir = true;
            f.Gatilho.AoConcluirDialogoExterno = () => throw new Exception("Callback principal executado no generico.");
            Exigir(!f.Gatilho.PodeDisparar(), "Principal deveria estar bloqueada.");
            for (int i = 0; i < 2; i++)
            {
                Exigir((bool)Chamar(f.Gatilho, "PodeDispararGenerico"), "Fallback indisponivel.");
                f.Gatilho.TentarInteragir();
                Exigir(f.Painel.EstaAberto && f.Painel.textoFala.text == f.TextoGenerico, "Interacao nao abriu generico.");
                Chamar(f.Painel, "Concluir", new object[] { null });
                Exigir(!f.Estado.MenuAberto && !f.Estado.ObterFlag("fixture_nao_gravar"), "Fallback alterou progresso/menu.");
                Exigir(f.Save.Chamadas == 0 && !(bool)Campo(f.Gatilho, "_jaDisparouNestaSessao"), "Fallback consumiu principal ou salvou.");
                // O harness e sincrono; simula a proxima leitura de input em outro frame.
                var frame = typeof(GatilhoDeDialogo).GetField("_frameConclusaoDialogo", Campos);
                if (frame != null) frame.SetValue(f.Gatilho, Time.frameCount - 1);
            }
        });
        Rodar(resultados, "tecla que fecha generico nao reabre no mesmo frame", f =>
        {
            f.Gatilho.TentarInteragir();
            Chamar(f.Painel, "Concluir", new object[] { null });
            f.Gatilho.TentarInteragir();
            Exigir(!f.Painel.EstaAberto && !f.Estado.MenuAberto, "Conversa reabriu com input do mesmo frame.");
        });
        Rodar(resultados, "principal tem prioridade e generico aparece depois", f =>
        {
            f.Estado.DefinirFlag("fixture_liberada", true);
            Exigir(f.Gatilho.PodeDisparar() && !(bool)Chamar(f.Gatilho, "PodeDispararGenerico"), "Prioridade incorreta.");
            f.Gatilho.TentarInteragir();
            Exigir(f.Painel.textoFala.text == "Fala principal de teste", "Generico substituiu principal.");
            Chamar(f.Painel, "Concluir", new object[] { null });
            Exigir(!f.Gatilho.PodeDisparar() && (bool)Chamar(f.Gatilho, "PodeDispararGenerico"), "Principal vista nao liberou fallback.");
            f.Gatilho.TentarInteragir();
            Exigir(!f.Painel.EstaAberto, "Fechar principal abriu generico no mesmo frame.");
            var frameConclusao = typeof(GatilhoDeDialogo).GetField("_frameConclusaoDialogo", Campos);
            if (frameConclusao != null) frameConclusao.SetValue(f.Gatilho, Time.frameCount - 1);
            f.Gatilho.TentarInteragir();
            Exigir(f.Painel.textoFala.text == f.TextoGenerico, "Fallback apos principal incorreto.");
            Chamar(f.Painel, "Concluir", new object[] { null });
        });
        Rodar(resultados, "flag impeditiva usa generico", f =>
        {
            f.Estado.DefinirFlag("fixture_liberada", true);
            f.Estado.DefinirFlag("fixture_impede", true);
            f.Gatilho.flagsQueImpedem = new[] { "fixture_impede" };
            Exigir(!f.Gatilho.PodeDisparar() && (bool)Chamar(f.Gatilho, "PodeDispararGenerico"), "Flag impeditiva ignorada.");
        });
        Rodar(resultados, "cutscene nao dispara generico", f =>
        {
            f.Gatilho.modo = GatilhoDeDialogo.ModoDeDisparo.AoEntrarNoTrigger;
            Exigir(!(bool)Chamar(f.Gatilho, "PodeDispararGenerico"), "Fallback habilitado em cutscene.");
        });
        Rodar(resultados, "NpcInterativo de dialogo usa fallback", f =>
        {
            var npc = f.Gatilho.gameObject.AddComponent<NpcInterativo>();
            npc.tipo = NpcInterativo.TipoDeNpc.Dialogo;
            npc.gatilhoDeDialogo = f.Gatilho;
            Chamar(npc, "Interagir");
            Exigir(f.Painel.EstaAberto && f.Painel.textoFala.text == f.TextoGenerico, "Despacho de dialogo nao usou fallback.");
            Chamar(f.Painel, "Concluir", new object[] { null });
            Exigir(!f.Estado.MenuAberto, "Contador de menu ficou preso.");
        });
        Rodar(resultados, "sem configuracao ou ID invalido preserva silencio", f =>
        {
            foreach (var id in new[] { "", "fixture_id_inexistente" })
            {
                Definir(f.Gatilho, "idDoDialogoGenerico", id);
                Exigir(!(bool)Chamar(f.Gatilho, "PodeDispararGenerico"), "Fallback invalido considerado disponivel.");
                f.Gatilho.TentarInteragir();
                Exigir(!f.Painel.EstaAberto && !f.Estado.MenuAberto, "ID vazio/invalido abriu UI.");
            }
        });
        Rodar(resultados, "loja abre somente depois da fala generica e fecha menu", f =>
        {
            var npc = f.Gatilho.gameObject.AddComponent<NpcInterativo>();
            npc.tipo = NpcInterativo.TipoDeNpc.Loja;
            npc.gatilhoDeDialogo = f.Gatilho;
            npc.lojaUIManager = f.Objeto("Loja").AddComponent<LojaUIManager>();
            npc.lojaUIManager.painel = f.Objeto("Vitrine");
            Chamar(npc, "Interagir");
            Exigir(f.Painel.EstaAberto && !npc.lojaUIManager.painel.activeSelf, "Loja abriu antes da fala.");
            Chamar(f.Painel, "Concluir", new object[] { null });
            Exigir(npc.lojaUIManager.painel.activeSelf && f.Estado.MenuAberto && f.Save.Chamadas == 0, "Loja nao abriu depois ou salvou progresso.");
            npc.lojaUIManager.Fechar();
            Exigir(!f.Estado.MenuAberto, "Menu ficou preso depois da loja.");
        });
        Rodar(resultados, "painel ocupado nao troca fala nem abre loja por baixo", f =>
        {
            f.Painel.Mostrar(new[] { new FalaDeDialogo { texto = "Outra conversa" } });
            Exigir(!(bool)Chamar(f.Gatilho, "DispararGenerico", new object[] { null }), "Fallback aceitou sobrescrever painel.");
            Exigir(f.Painel.textoFala.text == "Outra conversa", "Outra conversa sobrescrita.");
            var npc = f.Gatilho.gameObject.AddComponent<NpcInterativo>();
            npc.gatilhoDeDialogo = f.Gatilho;
            npc.lojaUIManager = f.Objeto("LojaOcupada").AddComponent<LojaUIManager>();
            npc.lojaUIManager.painel = f.Objeto("VitrineOcupada");
            Chamar(npc, "AbrirLoja");
            Exigir(!npc.lojaUIManager.painel.activeSelf, "Loja abriu por baixo de outro dialogo.");
            f.Painel.Fechar();
        });
        return string.Join("\n", resultados);
    }

    static void Rodar(List<string> linhas, string nome, Action<Fixture> teste)
    {
        try { using (var f = new Fixture()) teste(f); linhas.Add("OK: " + nome); }
        catch (Exception e) { while (e.InnerException != null) e = e.InnerException; linhas.Add("FALHOU: " + nome + " — " + e.Message); }
    }

    sealed class SaveBloqueado : ISistemaDeSalvamento
    {
        public int Chamadas;
        public void Salvar() { Chamadas++; throw new Exception("Teste tentou salvar; progresso pessoal protegido."); }
        public void Carregar() => throw new Exception("Teste tentou carregar save.");
        public bool ExisteSave() => false;
    }

    sealed class Fixture : IDisposable
    {
        readonly Scene cena;
        readonly object estadoAnterior;
        readonly object painelAnterior;
        public readonly GerenciadorDeEstado Estado;
        public readonly DialogoUIManager Painel;
        public readonly GatilhoDeDialogo Gatilho;
        public readonly SaveBloqueado Save = new SaveBloqueado();
        public string TextoGenerico => CatalogoDeDialogos.Obter("city_generico_dara")[0].texto;

        public Fixture()
        {
            estadoAnterior = EstadoSingleton.GetValue(null);
            painelAnterior = PainelSingleton.GetValue(null);
            cena = EditorSceneManager.NewPreviewScene();
            try
            {
                Estado = Objeto("Estado").AddComponent<GerenciadorDeEstado>();
                EstadoSingleton.SetValue(null, Estado);
                Painel = Objeto("Dialogo").AddComponent<DialogoUIManager>();
                PainelSingleton.SetValue(null, Painel);
                Painel.painel = Objeto("Painel");
                Painel.textoFala = Objeto("Fala").AddComponent<TextMeshProUGUI>();
                var npc = Objeto("NPC");
                npc.AddComponent<BoxCollider2D>().isTrigger = true;
                Gatilho = npc.AddComponent<GatilhoDeDialogo>();
                Definir(Gatilho, "_salvamento", Save);
                Gatilho.falas.Add(new FalaDeDialogo { texto = "Fala principal de teste" });
                Gatilho.flagsNecessarias = new[] { "fixture_liberada" };
                Gatilho.apenasUmaVez = true;
                // Reflexao permite executar a fase vermelha antes de existir o campo novo.
                var campo = typeof(GatilhoDeDialogo).GetField("idDoDialogoGenerico");
                if (campo != null) campo.SetValue(Gatilho, "city_generico_dara");
            }
            catch { Dispose(); throw; }
        }

        public GameObject Objeto(string nome)
        {
            var go = new GameObject("Fixture-Generico-" + nome, typeof(RectTransform));
            go.SetActive(false);
            SceneManager.MoveGameObjectToScene(go, cena);
            return go;
        }
        public void Dispose()
        {
            EstadoSingleton.SetValue(null, estadoAnterior);
            PainelSingleton.SetValue(null, painelAnterior);
            EditorSceneManager.ClosePreviewScene(cena);
        }
    }

    static object Campo(object alvo, string nome) => alvo.GetType().GetField(nome, Campos).GetValue(alvo);
    static void Definir(object alvo, string nome, object valor)
    {
        var campo = alvo.GetType().GetField(nome, Campos);
        Exigir(campo != null, "Campo ausente: " + nome);
        campo.SetValue(alvo, valor);
    }
    static object Chamar(object alvo, string nome, params object[] argumentos)
    {
        var metodo = alvo.GetType().GetMethod(nome, Campos);
        Exigir(metodo != null, "Metodo ausente: " + nome);
        return metodo.Invoke(alvo, argumentos);
    }
    static void Exigir(bool condicao, string mensagem) { if (!condicao) throw new Exception(mensagem); }
}
