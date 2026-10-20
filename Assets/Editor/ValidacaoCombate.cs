using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DroidsCode.Combat;
using DroidsCode.DroidCore;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Harness manual: fixtures de preview, arquivo temporário e singleton restaurado em finally.
/// Não inicia Play, não abre cenas do jogo e não lê/grava o save pessoal.</summary>
public static class ValidacaoCombate
{
    const BindingFlags Campos = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static readonly FieldInfo Singleton = typeof(GerenciadorDeEstado).GetField("instancia", BindingFlags.Static | BindingFlags.NonPublic);

    [MenuItem("Droids Code/Validacao/Checkpoint e encerramento de combate")]
    public static void ExecutarPeloMenu() => Debug.Log(Executar());

    public static string Executar()
    {
        Exigir(!EditorApplication.isPlaying, "Execute fora de Play.");
        var resultados = new List<string>();
        Rodar(resultados, "checkpoint independente da cena ativa", Checkpoint);
        Rodar(resultados, "Battle recusada sem sobrescrever checkpoint", RecusarBattle);
        Rodar(resultados, "legado Battle recusado antes de aplicar estado", Legado);
        Rodar(resultados, "falha de gravacao nao prepara entrada", FalhaDeEntrada);
        Rodar(resultados, "fuga bloqueia comandos e consolida uma vez", Fuga);
        Rodar(resultados, "vitoria concede uma recompensa e salva origem", Vitoria);
        Rodar(resultados, "derrota preserva checkpoint anterior", Derrota);
        Rodar(resultados, "Battle sem origem nao grava", SemOrigem);
        Rodar(resultados, "falha no save final nao duplica recompensa", FalhaFinal);
        return string.Join("\n", resultados);
    }

    static void Rodar(List<string> resultados, string nome, Action<Fixture> teste)
    {
        try { using (var f = new Fixture()) teste(f); resultados.Add("OK: " + nome); }
        catch (Exception e) { resultados.Add("FALHOU: " + nome + " — " + Causa(e)); }
    }

    static void Checkpoint(Fixture f)
    {
        f.Estado.DefinirFlag("teste_concluido", true);
        f.Estado.AdicionarItem(CatalogoDeItens.IdPocaoPequena, 3);
        f.Salvar("City", new Vector3(8.89f, -9, 0));
        var dados = f.Ler();
        Exigir(dados.cena == "City" && dados.posicaoX == 8.89f && dados.posicaoY == -9, "Cena/posicao explicita perdida.");
        Exigir(dados.flagsDeHistoria.Exists(x => x.chave == "teste_concluido" && x.valor), "Flag nao salva.");
        f.Estado.TentarRemoverItem(CatalogoDeItens.IdPocaoPequena);
        Exigir(f.Ler().itens.Find(x => x.id == CatalogoDeItens.IdPocaoPequena).quantidade == 3, "Mudanca em memoria alterou checkpoint.");
    }

    static void RecusarBattle(Fixture f)
    {
        f.Salvar("Game", new Vector3(-18, 6, 0));
        string antes = File.ReadAllText(f.Arquivo);
        ExigirFalha(() => f.Salvar("Battle", Vector3.zero));
        Exigir(File.ReadAllText(f.Arquivo) == antes, "Battle sobrescreveu checkpoint.");
    }

    static void Legado(Fixture f)
    {
        string legado = JsonUtility.ToJson(new DadosDoJogo { cena = "Battle", droid = new DroidSalvo { nome = "Legado" } });
        File.WriteAllText(f.Arquivo, legado);
        string nome = f.Estado.DroidDoJogador.Nome;
        ExigirFalha(() => f.Ler());
        Exigir(File.ReadAllText(f.Arquivo) == legado && f.Estado.DroidDoJogador.Nome == nome, "Legado alterou arquivo/estado.");
    }

    static void FalhaDeEntrada(Fixture f)
    {
        f.Estado.ProximoInimigoId = "anterior";
        var invalido = f.NovoSalvamento(f.Pasta); // destino é diretório: falha de IO real.
        Exigir(!f.Preparar(invalido), "Entrada aceita apesar de falha de gravacao.");
        Exigir(!(bool)Propriedade(f.Estado, "TemRetornoDeBatalha") && f.Estado.ProximoInimigoId == "anterior", "Falha alterou contexto.");
    }

    static void Fuga(Fixture f)
    {
        Exigir(f.Preparar(f.Salvamento), "Entrada recusada.");
        f.Estado.AdicionarItem(CatalogoDeItens.IdPocaoPequena, 2);
        f.Estado.DroidDoJogador.Hp = 11;
        var b = f.Batalha();
        var fim = (IEnumerator)Chamar(b, "FinalizarBatalha", false, true);
        Exigir(fim.MoveNext(), "Nao iniciou espera de retorno.");
        Exigir((bool)LerCampo(b, "batalhaEncerrada") && !(bool)LerCampo(b, "turnoDoJogador"), "Fuga deixou comandos abertos.");
        Exigir(!b.botaoAtacar.interactable && !b.botaoItem.interactable && !b.botaoFugir.interactable, "Botoes ativos apos fuga.");
        Exigir(!b.painelListaDeAtaques.gameObject.activeSelf && !b.painelListaDeItens.gameObject.activeSelf, "Lista permaneceu aberta.");
        Chamar(b, "ExecutarAtaqueDoJogador", Droid.NomeAtaqueBasico);
        Chamar(b, "UsarItem", CatalogoDeItens.Obter(CatalogoDeItens.IdPocaoPequena));
        Exigir(f.Estado.ObterQuantidadeDeItem(CatalogoDeItens.IdPocaoPequena) == 2, "Callback tardio consumiu item.");
        Exigir(!f.Estado.ObterFlag("teste_vitoria") && f.Estado.Gold == 0, "Fuga concedeu premio.");
        Exigir(f.Ler().cena == "Game" && f.Ler().droid.hp == 11, "Fuga nao consolidou estado na origem.");
        Exigir(f.Estado.ProximoInimigoId == null && !(bool)Propriedade(f.Estado, "TemRetornoDeBatalha"), "Contexto de batalha vazou.");
        Exigir(!((IEnumerator)Chamar(b, "FinalizarBatalha", false, true)).MoveNext(), "Segunda saida agendada.");
    }

    static void Vitoria(Fixture f)
    {
        Exigir(f.Preparar(f.Salvamento), "Entrada recusada.");
        var b = f.Batalha();
        Exigir(((IEnumerator)Chamar(b, "FinalizarBatalha", true, false)).MoveNext(), "Vitoria nao iniciou saida.");
        Exigir(f.Estado.Gold == 10 && f.Estado.ObterFlag("teste_vitoria"), "Premio/flag incorretos.");
        var dados = f.Ler();
        Exigir(dados.cena == "Game" && dados.posicaoX == -18 && dados.gold == 10, "Resultado nao salvo na origem.");
        Exigir(!((IEnumerator)Chamar(b, "FinalizarBatalha", true, false)).MoveNext() && f.Estado.Gold == 10, "Premio repetido.");
    }

    static void Derrota(Fixture f)
    {
        Exigir(f.Preparar(f.Salvamento), "Entrada recusada.");
        string antes = File.ReadAllText(f.Arquivo);
        f.Estado.DroidDoJogador.Hp = 0;
        ((IEnumerator)Chamar(f.Batalha(), "FinalizarBatalha", false, false)).MoveNext();
        Exigir(File.ReadAllText(f.Arquivo) == antes, "Derrota sobrescreveu checkpoint.");
    }

    static void SemOrigem(Fixture f)
    {
        f.Salvar("City", Vector3.one);
        string antes = File.ReadAllText(f.Arquivo);
        ((IEnumerator)Chamar(f.Batalha(), "FinalizarBatalha", false, true)).MoveNext();
        Exigir(File.ReadAllText(f.Arquivo) == antes, "Battle isolada sobrescreveu save.");
    }

    static void FalhaFinal(Fixture f)
    {
        Exigir(f.Preparar(f.Salvamento), "Entrada recusada.");
        string antes = File.ReadAllText(f.Arquivo);
        var b = f.Batalha();
        Campo(b, "_salvamento", f.NovoSalvamento(f.Pasta));
        ((IEnumerator)Chamar(b, "FinalizarBatalha", true, false)).MoveNext();
        Exigir(f.Estado.Gold == 10 && File.ReadAllText(f.Arquivo) == antes, "Falha final perdeu checkpoint/premio.");
        ((IEnumerator)Chamar(b, "FinalizarBatalha", true, false)).MoveNext();
        Exigir(f.Estado.Gold == 10, "Repetiu premio apos erro de IO.");
    }

    sealed class Fixture : IDisposable
    {
        readonly Scene cena;
        readonly object singletonAnterior;
        public readonly string Pasta;
        public string Arquivo => Path.Combine(Pasta, "checkpoint.json");
        public readonly GerenciadorDeEstado Estado;
        public readonly object Salvamento;

        public Fixture()
        {
            singletonAnterior = Singleton.GetValue(null);
            cena = EditorSceneManager.NewPreviewScene();
            Pasta = Path.Combine(Path.GetTempPath(), "DroidsCode-Validacao-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Pasta);
            Estado = Objeto("Estado").AddComponent<GerenciadorDeEstado>();
            Campo(Estado, "<DroidDoJogador>k__BackingField", new Droid("Fixture", hpMax: 30));
            Singleton.SetValue(null, Estado);
            // Construído sob demanda dentro do using para preservar a limpeza em caso de falha.
            Salvamento = null;
        }

        public object NovoSalvamento(string caminho)
        {
            var ctor = typeof(SalvamentoJson).GetConstructor(new[] { typeof(string) });
            Exigir(ctor != null, "SalvamentoJson ainda nao aceita destino isolado.");
            return ctor.Invoke(new object[] { caminho });
        }
        object Sistema => Salvamento ?? NovoSalvamento(Arquivo);
        public void Salvar(string nome, Vector3 pos) => Chamar(Sistema, "SalvarCheckpoint", Estado, nome, pos);
        public DadosDoJogo Ler() => (DadosDoJogo)Chamar(Sistema, "LerDados");
        public bool Preparar(object sistema)
        {
            var args = new object[] { "Game", new Vector3(-18, 6, 0), "sucata_captura", "teste_vitoria", sistema ?? Sistema, null };
            return (bool)Chamar(Estado, "TentarPrepararBatalha", args);
        }
        public BattleManager Batalha()
        {
            var b = Objeto("Batalha").AddComponent<BattleManager>();
            b.botaoAtacar = Objeto("Atacar").AddComponent<Button>();
            b.botaoItem = Objeto("Item").AddComponent<Button>();
            b.botaoFugir = Objeto("Fugir").AddComponent<Button>();
            b.painelListaDeAtaques = Objeto("Ataques").transform;
            b.painelListaDeItens = Objeto("Itens").transform;
            b.painelListaDeAtaques.gameObject.SetActive(true);
            b.painelListaDeItens.gameObject.SetActive(true);
            b.textoMensagem = Objeto("Mensagem").AddComponent<TextMeshProUGUI>();
            b.goldMinimo = b.goldMaximo = 10;
            Campo(b, "droid", Estado.DroidDoJogador);
            Campo(b, "inimigo", new InimigoFixo("Fixture", 20, 5, 1, 0));
            // Inicialização explícita para a fixture inativa de preview.
            Campo(b, "_logDeBatalha", new List<string>());
            Campo(b, "_salvamento", Sistema);
            Chamar(b, "CapturarRetorno");
            return b;
        }
        GameObject Objeto(string nome)
        {
            var go = new GameObject("Fixture-" + nome, typeof(RectTransform));
            go.SetActive(false);
            SceneManager.MoveGameObjectToScene(go, cena);
            return go;
        }
        public void Dispose()
        {
            Singleton.SetValue(null, singletonAnterior);
            EditorSceneManager.ClosePreviewScene(cena);
            Directory.Delete(Pasta, true);
        }
    }

    static object Chamar(object alvo, string nome, params object[] args)
    {
        var metodo = alvo.GetType().GetMethod(nome, Campos);
        Exigir(metodo != null, "Metodo ausente: " + nome);
        return metodo.Invoke(alvo, args);
    }
    static object LerCampo(object alvo, string nome) => alvo.GetType().GetField(nome, Campos).GetValue(alvo);
    static void Campo(object alvo, string nome, object valor)
    {
        var campo = alvo.GetType().GetField(nome, Campos);
        Exigir(campo != null, "Campo ausente: " + nome);
        campo.SetValue(alvo, valor);
    }
    static object Propriedade(object alvo, string nome)
    {
        var p = alvo.GetType().GetProperty(nome, Campos);
        Exigir(p != null, "Propriedade ausente: " + nome);
        return p.GetValue(alvo);
    }
    static Exception Causa(Exception e) => e is TargetInvocationException && e.InnerException != null ? Causa(e.InnerException) : e;
    static void ExigirFalha(Action acao)
    {
        try { acao(); } catch (Exception e) when (Causa(e) is InvalidOperationException) { return; }
        throw new Exception("Operacao invalida deveria ter sido recusada.");
    }
    static void Exigir(bool ok, string mensagem) { if (!ok) throw new Exception(mensagem); }
}
