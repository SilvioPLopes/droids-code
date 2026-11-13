using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Montagem aditiva do percurso existente. Não altera Canvas, mapas ou prefabs de origem.</summary>
public static class MontagemDemoAto1
{
    const string Raiz = "DemoAto1";
    const string Apollo = "Assets/sprites/Entities/Characters/Companion/Apollo/sprite.png";
    const string Ivo = "Assets/sprites/Entities/Characters/Antagonists/Ivo_Rangel/sprite.png";
    static readonly List<string> Alteracoes = new List<string>();

    public static string PreviaGame() =>
        "Game/DemoAto1: Apollo (-18.75,6), encontro (-16,7), facções (-14,7), painel (-13.5,4), unidade (-10,3.5), Ivo (-8.5,4); " +
        "somente novos objetos/gatilhos e condições; preservar ZonaBatalha/Canvas/geometria.";

    public static string MontarGame()
    {
        Abrir("Game");
        Alteracoes.Clear();
        var root = new GameObject(Raiz);
        var apollo = Morador(root, "Apollo", "apollo", Apollo, new Vector2(-18.75f, 6));
        Historia(apollo, "game_s3_apollo_golpes", "ato1_apollo_explicou", "ato1_apollo_recusou");
        Historia(apollo, "game_s3_apos_combate", "ato1_apos_primeiro_combate", "ato1_sessao3");
        // O encontro inicial usa o mesmo callback e checkpoint das batalhas existentes.
        var encontro = Morador(root, "Sucata de Captura", "", "", new Vector2(-16, 7));
        Batalha(encontro, "sucata_captura", "ato1_sessao3", "ato1_apollo_explicou");

        var faccoes = Morador(root, "Rasha e Teodoro", "", "Assets/sprites/Entities/Characters/NPCs/Rasha/sprite.png", new Vector2(-14, 7));
        var escolha = Historia(faccoes, "game_s4_escolha", "ato1_faccao_escolhida", "ato1_apos_primeiro_combate");
        escolha.escolhas.Add(Opcao("Flor de Ferro", "ato1_faccao_ferro"));
        escolha.escolhas.Add(Opcao("Lótus Branca", "ato1_faccao_lotus"));
        escolha.escolhas.Add(Opcao("Seguir sem facção", "ato1_faccao_desgarrado"));
        Historia(faccoes, "game_s5_apollo_cratera", "ato1_chegou_cratera", "ato1_faccao_escolhida");
        var lotus = Historia(faccoes, "game_s5_teodoro_ressonancia", "ato1_modulo_recebido", "ato1_chegou_cratera", "ato1_faccao_lotus");
        foreach (var resposta in new[] { "Salvar quem está ferido", "Proteger quem ainda pode fugir", "Não escolher por eles" })
            lotus.escolhas.Add(Opcao(resposta, "ato1_modulo_recebido", "modulo_ressonancia"));
        Historia(faccoes, "game_s5_desgarrado_vazio", "ato1_modulo_recebido", "ato1_chegou_cratera", "ato1_faccao_desgarrado");
        // A condição comportamental da rota Ferro aguarda decisão do usuário; não conceder por suposição.

        var painel = Morador(root, "Apollo — painel de energia", "apollo", Apollo, new Vector2(-13.5f, 4));
        Historia(painel, "game_s5_painel_energia", "ato1_leu_painel", "ato1_modulo_recebido");
        Terminal(root, "Painel de energia", new Vector2(-14.75f, 4), "p_painel_energia", "ato1_leu_painel", "ato1_puzzle_energia");
        var unidade = Morador(root, "Unidade de manutenção", "", Apollo, new Vector2(-10, 3.5f));
        Historia(unidade, "game_s5_unidade_manutencao", "ato1_viu_unidade", "ato1_puzzle_energia");
        Historia(unidade, "game_s5_unidade_painel", "ato1_examinou_unidade", "ato1_viu_unidade");
        var ivo = Morador(root, "Ivo", "ivo", Ivo, new Vector2(-8.5f, 4));
        Historia(ivo, "game_s6_ivo_confissao", "ato1_ivo_reconheceu", "ato1_viu_unidade");
        Historia(ivo, "game_s6_ivo_celia", "ato1_ivo_celia", "ato1_ivo_reconheceu");
        Historia(ivo, "game_s6_ivo_apex", "ato1_ivo_confessou", "ato1_ivo_celia");
        Batalha(ivo, "droid_do_ivo", "ato1_sessao6", "ato1_ivo_confessou", "ato1_puzzle_energia");
        Historia(ivo, "game_s6_preco", "ato1_preco_pago", "ato1_sessao6").modo = GatilhoDeDialogo.ModoDeDisparo.AoEntrarNoTrigger;
        foreach (var rota in new[] { "ferro", "lotus", "desgarrado" })
            Historia(ivo, "game_s6_pos_" + rota, "ato1_ivo_resolvido", "ato1_preco_pago", "ato1_faccao_" + rota);
        Historia(ivo, "game_s6_ivo_entrega", "ato1_caderno_2060", "ato1_ivo_resolvido");
        Terminal(root, "Calibragem da bomba dois", new Vector2(-8.75f, 5.5f), "p_calibragem_braco", "quest_q_ivo_terca_ativa", "ato1_calibragem_feita");

        var sucata = Morador(root, "Sucata da borda", "", "", new Vector2(-13, 8.5f));
        Batalha(sucata, "sucata_captura", "", "ato1_sessao3");
        var selvagem = Morador(root, "Droid Selvagem", "", "", new Vector2(-9, 7));
        Batalha(selvagem, "droid_selvagem", "", "ato1_faccao_escolhida");
        var alfa = Morador(root, "Selvagem Alfa", "", "", new Vector2(-13.5f, 3.25f));
        Batalha(alfa, "droid_selvagem_alfa", "ato1_alfa_derrotado", "quest_q_guilda_3_alfa_ativa");
        Salvar();
        return string.Join("\n", Alteracoes);
    }

    public static string MontarCity()
    {
        Abrir("City");
        Alteracoes.Clear();
        var root = new GameObject(Raiz);
        var gate = UnityEngine.Object.FindObjectsByType<TransicaoDeCena>(FindObjectsSortMode.None).Single(t => t.nomeCenaDestino == "Game");
        var apollo = Morador(root, "Apollo — portão", "apollo", Apollo, new Vector2(-.909f, -9.5f));
        VincularConclusaoApollo(apollo);
        Historia(apollo, "city_s3_sino", "ato1_sino", "quest_q_main_agua_concluida", "ato1_apollo_conclusao");
        Historia(apollo, "city_s3_recusa_apollo", "ato1_apollo_recusou", "ato1_sino");
        var lockGate = gate.GetComponent<TrancaPorHistoria>();
        lockGate.flagsNecessarias = lockGate.flagsNecessarias.Concat(new[] { "ato1_apollo_recusou" }).Distinct().ToArray();
        Historia(apollo, "city_s7_agua", "ato1_agua_voltou", "ato1_caderno_2060");
        Historia(apollo, "city_s7_apollo_revelacao", "ato1_conversa_noturna", "ato1_cidade_respondeu");
        Historia(apollo, "city_s7_mochila", "ato1_mochila_pronta", "ato1_conversa_noturna");
        var dara = UnityEngine.Object.FindObjectsByType<MoradorInterativo>(FindObjectsSortMode.None).Single(m => m.idNpc == "dara");
        Historia(dara, "city_s7_dara_cantil", "ato1_cantil_recebido", "ato1_mochila_pronta");
        var partida = Historia(apollo, "city_s7_partida", "ato1_concluido", "ato1_cantil_recebido");
        partida.escolhas.Add(new EscolhaDeDialogo { texto = "Encerrar a demo", respostaAposEscolher = "Fim da demo — Ato 1", falanteDaResposta = "" });
        // Chegada, oferta, assembleia e reação usam Nadir já montada; preservar identidade e presença.
        var nadir = UnityEngine.Object.FindObjectsByType<MoradorInterativo>(FindObjectsInactive.Include, FindObjectsSortMode.None).Single(m => m.idNpc == "nadir");
        var oferta = nadir.historia.Single(g => g.idDoDialogo == "city_s7_nadir_oferta");
        oferta.flagsNecessarias = new[] { "ato1_nadir" };
        Historia(apollo, "city_s7_nadir_chegada", "ato1_nadir", "ato1_perna_reparada");
        Historia(nadir, "city_s7_assembleia", "ato1_assembleia", "ato1_ouviu_nadir");
        Historia(nadir, "city_s7_dara_fala", "ato1_dara_falou_assembleia", "ato1_assembleia");
        foreach (var rota in new[] { "ferro", "lotus", "desgarrado" })
            Historia(nadir, "city_s7_nadir_" + rota, "ato1_cidade_respondeu", "ato1_dara_falou_assembleia", "ato1_faccao_" + rota);
        Salvar();
        return string.Join("\n", Alteracoes);
    }

    public static string ComplementarGame()
    {
        Abrir("Game", false);
        Alteracoes.Clear();
        var root = GameObject.Find(Raiz);
        if (root == null || root.transform.Find("Calibragem da bomba dois") != null) throw new InvalidOperationException("Alvo inesperado; não repetir.");
        Terminal(root, "Calibragem da bomba dois", new Vector2(-8.75f, 5.5f), "p_calibragem_braco", "quest_q_ivo_terca_ativa", "ato1_calibragem_feita");
        Salvar();
        return string.Join("\n", Alteracoes);
    }

    public static string CorrigirConclusaoApollo()
    {
        Abrir("City", false);
        var apollo = GameObject.Find("DemoAto1/Apollo — portão").GetComponent<MoradorInterativo>();
        var conclusao = UnityEngine.Object.FindObjectsByType<GatilhoDeDialogo>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Single(g => g.idDoDialogo == "city_s2_apollo_conclusao");
        if (conclusao.flagAoTerminar != "ato1_sessao2") throw new InvalidOperationException("Alvo já alterado; não repetir.");
        VincularConclusaoApollo(apollo);
        apollo.historia.Single(g => g.idDoDialogo == "city_s3_sino").flagsNecessarias =
            new[] { "quest_q_main_agua_concluida", "ato1_apollo_conclusao" };
        Salvar();
        return "Conclusão Apollo usa flag própria, exige as três conversas e é compartilhada com o morador do portão antes do Sino; estados após sessão3 não repetem investigação.";
    }

    static void VincularConclusaoApollo(MoradorInterativo apollo)
    {
        var g = UnityEngine.Object.FindObjectsByType<GatilhoDeDialogo>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Single(x => x.idDoDialogo == "city_s2_apollo_conclusao");
        g.flagsNecessarias = new[] { "ato1_falou_dara", "ato1_falou_fila", "ato1_falou_tacio" };
        g.flagAoTerminar = "ato1_apollo_conclusao";
        g.flagsQueImpedem = new[] { "ato1_apollo_conclusao", "ato1_sessao3" };
        apollo.historia = new[] { g }.Concat(apollo.historia.Where(x => x != g)).ToArray();
    }

    public static string MontarOficina()
    {
        Abrir("Interiores/Interior_E02_Principal");
        Alteracoes.Clear();
        var root = new GameObject(Raiz);
        var quirino = UnityEngine.Object.FindObjectsByType<MoradorInterativo>(FindObjectsInactive.Include, FindObjectsSortMode.None).Single(m => m.idNpc == "ferreiro");
        Historia(quirino, "city_s7_quirino_perna", "ato1_perna_reparada", "quest_apollo_perna_ok", "ato1_agua_voltou");
        Terminal(root, "Bancada do filtro", new Vector2(-1.5f, -1), "p_filtro_agua", "quest_q_agua_dara_ativa", "ato1_filtro_pronto");
        var retirada = quirino.gameObject.AddComponent<GatilhoDeDialogo>();
        retirada.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript;
        retirada.falas = new List<FalaDeDialogo> { new FalaDeDialogo { falante = "Quirino", texto = "O filtro ficou pronto. Pode levar para a Dara." } };
        retirada.flagAoTerminar = "ato1_filtro_recolhido";
        retirada.flagsNecessarias = new[] { "ato1_filtro_pronto" }; retirada.flagsQueImpedem = new[] { "ato1_filtro_recolhido" };
        retirada.escolhas.Add(Opcao("Levar o filtro para Dara", "ato1_filtro_recolhido", "filtro_de_agua"));
        quirino.historia = quirino.historia.Concat(new[] { retirada }).ToArray();
        PrefabUtility.RecordPrefabInstancePropertyModifications(quirino);
        Salvar();
        return "E02/ferreiro: reparo após entrega + retirada única do filtro; bancada (-1.5,-1), sem mudar obstáculos ou arte.";
    }

    public static string ComplementarMarcas()
    {
        Abrir("Game", false);
        Alteracoes.Clear();
        var root = GameObject.Find(Raiz);
        if (root == null || root.transform.Find("Placa da rua") != null) throw new InvalidOperationException("Alvo inesperado; não repetir.");
        var placa = Morador(root, "Placa da rua", "", "", new Vector2(-13.5f, 8));
        Historia(placa, "game_s5_placa", "ato1_marca_placa", "ato1_faccao_escolhida");
        var degrau = Morador(root, "Degrau antigo", "", "", new Vector2(-13.5f, 3.5f));
        Historia(degrau, "game_s5_degrau", "ato1_marca_degrau", "ato1_marca_placa");
        var cano = Morador(root, "Canalização", "", "", new Vector2(-9, 6));
        Historia(cano, "game_s5_cano", "ato1_cratera_verdade", "ato1_marca_degrau");
        AtualizarIndicadores(root);
        Salvar();
        return string.Join("\n", Alteracoes);
    }

    public static string IndicadoresCity()
    {
        Abrir("City", false);
        AtualizarIndicadores(GameObject.Find(Raiz));
        Salvar();
        return "Somente indicador do Apollo novo no portão; Canvas preservado.";
    }

    public static string MontarFerro()
    {
        Abrir("Game", false);
        Alteracoes.Clear();
        var root = GameObject.Find(Raiz);
        if (root == null || root.transform.Find("Prova de Rasha") != null) throw new InvalidOperationException("Alvo inesperado; não repetir.");
        var encontro = Morador(root, "Prova de Rasha", "", "", new Vector2(-13, 5.8f));
        encontro.nome = "Sucata na trilha";
        encontro.transform.Find("Interação").GetComponent<TextMeshPro>().text = "E — Sucata na trilha";
        Batalha(encontro, "sucata_captura", "ato1_prova_rasha_vencida", "ato1_faccao_ferro", "ato1_chegou_cratera");
        var faccoes = root.transform.Find("Rasha e Teodoro").GetComponent<MoradorInterativo>();
        var decisao = faccoes.gameObject.AddComponent<GatilhoDeDialogo>();
        decisao.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript;
        decisao.apenasUmaVez = false;
        decisao.falas = new List<FalaDeDialogo> { new FalaDeDialogo { falante = "", texto = "A sucata já não consegue se levantar. A luta está vencida. Lip ainda pode continuar golpeando." } };
        decisao.flagsNecessarias = new[] { "ato1_prova_rasha_vencida", "ato1_faccao_ferro" };
        decisao.flagsQueImpedem = new[] { "ato1_continuou_luta" };
        var continuar = Opcao("Continuar a luta", "ato1_continuou_luta");
        continuar.respostaAposEscolher = "Lip continua batendo depois da vitória. Rasha observa sem interromper.";
        continuar.falanteDaResposta = "";
        decisao.escolhas.Add(continuar);
        decisao.escolhas.Add(new EscolhaDeDialogo { texto = "Parar por enquanto", respostaAposEscolher = "Lip abaixa o braço. A decisão pode ser retomada ao conversar aqui novamente.", falanteDaResposta = "" });
        faccoes.historia = faccoes.historia.Concat(new[] { decisao }).ToArray();
        var modulo = Historia(faccoes, "game_s5_rasha_nucleo", "ato1_modulo_recebido", "ato1_continuou_luta", "ato1_faccao_ferro");
        modulo.escolhas.Add(Opcao("Receber o Núcleo de Sobrecarga", "ato1_modulo_recebido", "nucleo_sobrecarga"));
        foreach (var m in root.GetComponentsInChildren<MoradorInterativo>(true))
        {
            if (m.batalha == null || m.transform.Find("Visual") != null) continue;
            string arquivo = m.batalha.idDoInimigo == "sucata_captura" ? "SucataCaptura-v1" : m.batalha.idDoInimigo == "droid_selvagem" ? "DroidC-v1" : "SucataReforcada-v2";
            var sprite = AssetDatabase.LoadAllAssetsAtPath("Assets/sprites/Entities/Enemies/Battle/" + arquivo + ".png").OfType<Sprite>().First();
            var visual = new GameObject("Visual"); visual.transform.SetParent(m.transform, false);
            var renderer = visual.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = 20;
            visual.transform.localScale = Vector3.one * (1.1f / sprite.bounds.size.y);
        }
        AtualizarIndicadores(root);
        Salvar();
        return "Prova de Rasha (-13,5.8): vitória não concede módulo; escolha explícita continuar → diálogo → um núcleo. Adiar permite retornar. Visuais existentes nos encontros novos.";
    }

    public static string ReaproveitarDialogoDaCasa()
    {
        Abrir("Game", false);
        var destino = UnityEngine.Object.FindObjectsByType<DialogoUIManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Single();
        if (destino.transform.Find("LegadoAntesDemo") != null) throw new InvalidOperationException("Diálogo já montado; não repetir.");
        var referencia = EditorSceneManager.OpenPreviewScene("Assets/Scenes/CasaLip.unity");
        try
        {
            var origem = referencia.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<DialogoUIManager>(true)).Single();
            if (!AssetDatabase.Contains(origem.prefabBotaoEscolha)) throw new InvalidOperationException("Template deve ser asset, sem referência entre cenas.");
            var antigos = destino.transform.Cast<Transform>().ToArray();
            var copia = UnityEngine.Object.Instantiate(origem.painel, destino.transform.parent, false);
            copia.name = "CopiaTemporariaDialogoDemo";
            var campos = copia.GetComponent<DialogoUIManager>();
            var arquivo = new GameObject("LegadoAntesDemo", typeof(RectTransform));
            arquivo.transform.SetParent(destino.transform, false); arquivo.SetActive(false);
            foreach (var filho in antigos) filho.SetParent(arquivo.transform, false);
            var de = origem.painel.GetComponent<RectTransform>();
            var para = destino.painel.GetComponent<RectTransform>();
            para.anchorMin = de.anchorMin; para.anchorMax = de.anchorMax; para.pivot = de.pivot;
            para.anchoredPosition = de.anchoredPosition; para.sizeDelta = de.sizeDelta;
            para.localScale = de.localScale; para.localRotation = de.localRotation;
            var imageDe = origem.painel.GetComponent<UnityEngine.UI.Image>();
            var imagePara = destino.painel.GetComponent<UnityEngine.UI.Image>();
            imagePara.sprite = imageDe.sprite; imagePara.type = imageDe.type; imagePara.color = imageDe.color;
            imagePara.preserveAspect = imageDe.preserveAspect; imagePara.pixelsPerUnitMultiplier = imageDe.pixelsPerUnitMultiplier;
            imagePara.raycastTarget = imageDe.raycastTarget;
            var proporcao = origem.painel.GetComponent<UnityEngine.UI.AspectRatioFitter>();
            var ajuste = destino.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();
            ajuste.aspectMode = proporcao.aspectMode; ajuste.aspectRatio = proporcao.aspectRatio;
            foreach (Transform filho in copia.transform.Cast<Transform>().ToArray()) filho.SetParent(destino.transform, false);
            destino.textoFalante = campos.textoFalante; destino.textoFala = campos.textoFala;
            destino.imagemRetrato = campos.imagemRetrato; destino.retratoPadrao = campos.retratoPadrao;
            destino.retratosConhecidos = new List<RetratoPersonagem>(campos.retratosConhecidos);
            destino.botaoAvancar = campos.botaoAvancar; destino.painelDeEscolhas = campos.painelDeEscolhas;
            destino.prefabBotaoEscolha = campos.prefabBotaoEscolha;
            destino.teclaAvancar = campos.teclaAvancar; destino.teclaAvancarAlternativa = campos.teclaAvancarAlternativa;
            // Só a cópia temporária criada acima é removida; os objetos antigos ficam arquivados.
            UnityEngine.Object.DestroyImmediate(copia);
            Salvar();
            return "Somente PainelDialogo local de Game recebeu apresentação/referências da CasaLip; gestor preservado e filhos antigos arquivados inativos. Canvas/Scaler/painéis irmãos intactos.";
        }
        finally { EditorSceneManager.ClosePreviewScene(referencia); }
    }

    static void AtualizarIndicadores(GameObject root)
    {
        foreach (var m in root.GetComponentsInChildren<MoradorInterativo>(true))
        {
            var label = m.transform.Find("Interação");
            if (label == null) continue;
            m.indicadorDeProximidade = label.gameObject; label.gameObject.SetActive(false);
        }
    }

    static void Abrir(string nome, bool novaRaiz = true)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Somente fora de Play.");
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Há cena suja; preservar antes de montar.");
        EditorSceneManager.OpenScene("Assets/Scenes/" + nome + ".unity");
        if (novaRaiz && GameObject.Find(Raiz) != null) throw new InvalidOperationException("Montagem já existe; não repetir.");
    }

    static MoradorInterativo Morador(GameObject root, string nome, string id, string spritePath, Vector2 pos)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(root.transform);
        go.transform.position = pos;
        var col = go.AddComponent<CircleCollider2D>(); col.radius = .85f; col.isTrigger = true;
        var sprite = string.IsNullOrEmpty(spritePath) ? null : AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().FirstOrDefault();
        if (sprite != null)
        {
            var visual = new GameObject("Visual"); visual.transform.SetParent(go.transform, false);
            var renderer = visual.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = 20;
            visual.transform.localScale = Vector3.one * (1.1f / sprite.bounds.size.y);
        }
        var label = new GameObject("Interação"); label.transform.SetParent(go.transform, false); label.transform.localPosition = new Vector3(0, 1.1f, 0);
        var text = label.AddComponent<TextMeshPro>(); text.text = "E — " + nome; text.fontSize = 2; text.alignment = TextAlignmentOptions.Center;
        text.GetComponent<MeshRenderer>().sortingOrder = 30; text.rectTransform.sizeDelta = new Vector2(3, .7f);
        var morador = go.AddComponent<MoradorInterativo>(); morador.idNpc = id; morador.nome = nome;
        morador.indicadorDeProximidade = label; label.SetActive(false);
        morador.saudacao = "Preciso terminar os preparativos antes de seguir.";
        Alteracoes.Add(go.name + " @ " + pos);
        return morador;
    }

    static GatilhoDeDialogo Historia(MoradorInterativo m, string id, string flag, params string[] requer)
    {
        if (CatalogoDeDialogos.Obter(id) == null) throw new InvalidOperationException("Diálogo inexistente: " + id);
        var g = m.gameObject.AddComponent<GatilhoDeDialogo>(); g.modo = GatilhoDeDialogo.ModoDeDisparo.ApenasPorScript;
        g.idDoDialogo = id; g.flagAoTerminar = flag; g.flagsNecessarias = requer; g.flagsQueImpedem = new[] { flag };
        m.historia = (m.historia ?? Array.Empty<GatilhoDeDialogo>()).Concat(new[] { g }).ToArray();
        if (PrefabUtility.IsPartOfPrefabInstance(m)) PrefabUtility.RecordPrefabInstancePropertyModifications(m);
        Alteracoes.Add(id + " → " + flag);
        return g;
    }

    static EscolhaDeDialogo Opcao(string texto, string flag, string item = "") =>
        new EscolhaDeDialogo { texto = texto, flagAoEscolher = flag, idItemConcedido = item, quantidadeDoItem = string.IsNullOrEmpty(item) ? 0 : 1 };

    static void Batalha(MoradorInterativo m, string id, string flag, params string[] requer)
    {
        var g = m.gameObject.AddComponent<GatilhoDeBatalha>(); g.enabled = false;
        g.idDoInimigo = id; g.flagDeVitoria = flag; g.flagsNecessarias = requer; g.nomeCenaBatalha = "Battle";
        m.batalha = g;
        Alteracoes.Add("Battle " + id + " → " + flag);
    }

    static void Terminal(GameObject root, string nome, Vector2 pos, string puzzle, string requer, string flag)
    {
        var go = new GameObject(nome); go.transform.SetParent(root.transform); go.transform.position = pos;
        var col = go.AddComponent<CircleCollider2D>(); col.radius = .7f; col.isTrigger = true;
        var verifier = go.AddComponent<VerificadorDePuzzle>(); verifier.idDoPuzzle = puzzle; verifier.flagAoResolver = flag; verifier.flagQueJaResolve = flag;
        verifier.enabled = false;
        var terminal = go.AddComponent<TerminalDoInterior>(); terminal.idPuzzle = puzzle; terminal.flagNecessaria = requer; terminal.verificador = verifier;
        var label = new GameObject("Interação"); label.transform.SetParent(go.transform, false); label.transform.localPosition = new Vector3(0, .8f, 0);
        var text = label.AddComponent<TextMeshPro>(); text.text = "E — " + nome; text.fontSize = 2; text.alignment = TextAlignmentOptions.Center;
        text.rectTransform.sizeDelta = new Vector2(3, .7f); text.GetComponent<MeshRenderer>().sortingOrder = 30;
        Alteracoes.Add(puzzle + " @ " + pos + " → " + flag);
    }

    static void Salvar()
    {
        var scene = SceneManager.GetActiveScene(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
    }
}
