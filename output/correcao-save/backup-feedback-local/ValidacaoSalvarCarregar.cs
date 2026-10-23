using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>IO somente em arquivo exclusivo de fixture. Não carrega cena ou grava save pessoal.</summary>
public static class ValidacaoSalvarCarregar
{
    const BindingFlags Privado = BindingFlags.NonPublic | BindingFlags.Instance;
    static void Exigir(bool ok, string erro) { if (!ok) throw new InvalidOperationException(erro); }
    static void Campo(object alvo, string nome, object valor) => alvo.GetType().GetField(nome, Privado).SetValue(alvo, valor);
    static object Ler(object alvo, string nome) => alvo.GetType().GetField(nome, Privado).GetValue(alvo);
    static void Chamar(object alvo, string nome) => alvo.GetType().GetMethod(nome, Privado).Invoke(alvo, null);
    static GameObject Objeto(UnityEngine.SceneManagement.Scene cena, string nome, params Type[] tipos)
    {
        var go = new GameObject(nome, tipos); UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, cena); return go;
    }
    [MenuItem("DroidsCode/Validação/Salvar e Carregar (arquivo isolado)")]
    public static void Menu() => Debug.Log(Executar());
    public static string Executar()
    {
        Exigir(!EditorApplication.isPlayingOrWillChangePlaymode, "Exige fora de Play.");
        Exigir(Application.dataPath.Replace('\\', '/') == "C:/Users/silve/DroidsCode/Assets", "Projeto inesperado.");
        Exigir(Task.Run(() => new SalvamentoJson()).GetAwaiter().GetResult() != null, "Construção acessou API Unity fora do thread principal.");
        var raizFixture = Path.GetFullPath("output/correcao-save/fixtures");
        var pasta = Path.GetFullPath(Path.Combine(raizFixture, Guid.NewGuid().ToString("N")));
        Exigir(pasta.StartsWith(raizFixture + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase), "Fixture fora do diretório autorizado.");
        Directory.CreateDirectory(pasta); var arquivo = Path.Combine(pasta, "save.json");
        var campoEstado = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var campoDialogo = typeof(DialogoUIManager).GetField("_instancia", BindingFlags.Static | BindingFlags.NonPublic);
        var estadoAnterior = campoEstado.GetValue(null); var dialogoAnterior = campoDialogo.GetValue(null);
        var cena = EditorSceneManager.NewPreviewScene();
        try
        {
            var estado = Objeto(cena, "EstadoFixture", typeof(GerenciadorDeEstado)).GetComponent<GerenciadorDeEstado>();
            campoEstado.SetValue(null, estado);
            var droid = new Droid("Fixture", 30);
            Campo(estado, "<DroidDoJogador>k__BackingField", droid);
            var servico = new SalvamentoJson(arquivo);
            Exigir(!servico.ExisteSave(), "Fixture já existia.");
            servico.SalvarCheckpoint(estado, "Game", new Vector3(1, 2, 0));
            Exigir(servico.ExisteSave() && servico.LerDados().droid.hp == 30, "Gravação/leitura falhou.");
            droid.Hp = 21; droid.RestaurarEnergia(25, 100);
            servico.SalvarCheckpoint(estado, "Game", new Vector3(3, 4, 0));
            var dados = servico.LerDados();
            Exigir(dados.droid.hp == 21 && dados.droid.energiaAtual == 25 && dados.posicaoX == 3, "Regravação perdeu dados.");
            Exigir(!File.Exists(arquivo + ".tmp"), "Arquivo temporário não foi limpo.");
            var menu = Objeto(cena, "MenuFixture", typeof(MenuMundoManager)).GetComponent<MenuMundoManager>();
            menu.painelMenu = Objeto(cena, "PainelMenu", typeof(RectTransform));
            Campo(menu, "_salvamento", null);
            Exigir(typeof(MenuMundoManager).GetProperty("Salvamento", Privado).GetValue(menu) != null, "Referência nula não recuperada.");
            Campo(menu, "_salvamento", new SalvamentoJson(Path.Combine(pasta, "ausente.json")));
            Chamar(menu, "AoClicarCarregar"); Exigir((string)Ler(menu, "_resultadoPersistenciaPendente") == "ausente", "Save ausente sem feedback.");
            Campo(menu, "_resultadoPersistenciaPendente", null);
            File.WriteAllText(arquivo, "{json quebrado"); Campo(menu, "_salvamento", servico);
            Chamar(menu, "AoClicarCarregar"); Exigir((string)Ler(menu, "_resultadoPersistenciaPendente") == "erro_carregar", "JSON inválido sem feedback.");
            Campo(menu, "_resultadoPersistenciaPendente", null);
            Campo(menu, "_salvamento", new SalvamentoJson(Path.Combine(pasta, "diretorio-inexistente", "save.json")));
            Chamar(menu, "AoClicarSalvar"); Exigir((string)Ler(menu, "_resultadoPersistenciaPendente") == "erro_salvar", "Falha de escrita anunciou sucesso.");
            Campo(menu, "_resultadoPersistenciaPendente", null); Campo(menu, "_salvamento", servico);
            Chamar(menu, "AoClicarSalvar"); Exigir((string)Ler(menu, "_resultadoPersistenciaPendente") == "salvo", "Handler de salvar falhou.");
            var dialogo = Objeto(cena, "DialogoFixture", typeof(DialogoUIManager)).GetComponent<DialogoUIManager>();
            campoDialogo.SetValue(null, dialogo);
            dialogo.painel = Objeto(cena, "PainelDialogo", typeof(RectTransform));
            dialogo.textoFala = Objeto(cena, "Fala", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            dialogo.textoFala.font = TMP_Settings.defaultFontAsset;
            bool callbackAnterior = false;
            dialogo.Mostrar(CatalogoDeDialogos.AvisoDePersistencia("ausente"), () => callbackAnterior = true);
            Chamar(menu, "TentarMostrarResultadoPersistencia");
            Exigir((string)Ler(menu, "_resultadoPersistenciaPendente") == "salvo" && !callbackAnterior, "Conversa/callback sobrescrito.");
            dialogo.Avancar(); Exigir(callbackAnterior, "Callback da conversa anterior perdido.");
            Chamar(menu, "TentarMostrarResultadoPersistencia");
            Exigir(dialogo.EstaAberto && dialogo.textoFala.text == "Jogo salvo." && Ler(menu, "_resultadoPersistenciaPendente") == null,
                "Confirmação não chegou ao diálogo.");
            Exigir(!menu.painelMenu.activeSelf, "Menu não fechou ao apresentar diálogo.");
            return "OK: construtor em thread secundário, referência nula recuperada, grava/regrava/lê fixture, energia/HP/posição preservados, ausente/JSON inválido/falha de IO com retorno correto, confirmação no diálogo e conversa/callback preservados. Sem CarregarDados/troca de cena/save pessoal.";
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(cena); campoEstado.SetValue(null, estadoAnterior); campoDialogo.SetValue(null, dialogoAnterior);
            // pasta foi resolvida e validada acima; só remove a fixture exclusiva criada nesta execução.
            if (Directory.Exists(pasta)) Directory.Delete(pasta, true);
        }
    }
}
