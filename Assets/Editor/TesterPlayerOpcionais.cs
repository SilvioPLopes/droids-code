using System;
using System.Linq;
using System.Threading.Tasks;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Extensao manual: conserva a historia encerrada e completa cada quest do catalogo uma vez.
public static partial class TesterPlayer
{
    public static bool TodasAsQuestsEntregues(Func<string,bool> concluida)
    {
        if(concluida==null)throw new ArgumentNullException(nameof(concluida));
        return CatalogoDeQuests.Todos.Keys.All(concluida);
    }

    [MenuItem("Droids Code/Testes/TesterPlayer/Concluir todas as opcionais do save")]
    public static void PrepararTodasOpcionais()
    {
        Exigir(!EditorApplication.isPlaying,"Saia de Play para retomar o save.");
        for(int i=0;i<SceneManager.sceneCount;i++)Exigir(!SceneManager.GetSceneAt(i).isDirty,"Cena nao salva.");
        Exigir(new SalvamentoJson().ExisteSave(),"Save nao encontrado.");
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        SessionState.SetString(Modo,"opcionais"); SessionState.SetBool(Armado,true);
        EditorApplication.isPlaying=true;
    }
    static bool Concluida(string id)=>GerenciadorDeQuests.EstaConcluida(id) && !GerenciadorDeQuests.EstaAtiva(id);
    static async Task Opcional(string id,Func<Task> acao)
    {
        await Etapa(CatalogoDeQuests.Obter(id).Titulo,async()=>
        {
            if(!Concluida(id))await acao();
            Exigir(Concluida(id),"Quest opcional nao entregue: "+id);
        });
    }
    static async Task PrepararCacada()
    {
        var d=G.DroidDoJogador;
        int gasto=Math.Min(d.Pontos.PontosDisponiveis,Math.Max(0,20-d.StatsBase.For));
        if(gasto>0)
        {
            int anterior=d.StatsBase.For;
            await Codigo("droid.subirAtributo(\"For\","+gasto+")");
            Exigir(d.StatsBase.For==anterior+gasto,"Upgrade de FOR recusado.");
        }
        await CurarFora();
    }
    static async Task Cacar(string encontro)
    {
        await GarantirGame(); await PrepararCacada();
        try { ValidacaoPercursoApresentacao.PreferirAtaqueBasico=true; await Encontro(encontro); }
        finally { ValidacaoPercursoApresentacao.PreferirAtaqueBasico=false; }
    }
    static void ProgressoColeta(string etapa,string id,int alvo,int batalhas)
    {
        relatorio.etapa=etapa+": "+id+" "+G.ObterQuantidadeDeItem(id)+"/"+alvo+", encontros="+batalhas;
        Gravar(); Debug.Log("[TesterPlayer] "+relatorio.etapa);
    }
    static async Task Coletar(string id,int quantidade,string encontro,int limite=40)
    {
        string etapa=relatorio.etapa;
        for(int i=0;G.ObterQuantidadeDeItem(id)<quantidade && i<limite;i++)
        {
            await Cacar(encontro); ProgressoColeta(etapa,id,quantidade,i+1);
        }
        Exigir(G.ObterQuantidadeDeItem(id)>=quantidade,"Coleta atingiu limite sem estoque suficiente: "+id);
        relatorio.etapa=etapa; Gravar();
    }
    static async Task Bruna()
    {
        if(SceneManager.GetActiveScene().name=="Interior_E09_Terreo")return;
        await GarantirCity(); await Transitar("Interior_E09_Terreo");
    }
    static async Task ExecutarOpcionais()
    {
        try
        {
            await Opcional("q_pipo_peca",async()=>
            {
                await GarantirCity(); await Transitar("Interior_E05_Principal","Porta_Casa02");
                await Missao("crianca_rua","q_pipo_peca"); await Missao("crianca_rua","q_pipo_peca",true);
            });
            await Opcional("q_sula_divida",async()=>
            {
                if(SceneManager.GetActiveScene().name!="Interior_E05_Principal")
                { await GarantirCity(); await Transitar("Interior_E05_Principal","Porta_Casa02"); }
                await Missao("vendedora_pecas","q_sula_divida"); await Missao("vendedora_pecas","q_sula_divida",true);
            });
            await Opcional("q_dara_terceira",async()=>
            {
                await GarantirCity(); await Missao("Dara","q_dara_terceira");
                await Coletar(CatalogoDeItens.IdSucataLimpa,4+(Concluida("q_guilda_1_iniciacao")?0:5),"Sucata da borda");
                await GarantirCity(); await Missao("Dara","q_dara_terceira",true);
            });
            await Opcional("q_guilda_1_iniciacao",async()=>
            {
                await Coletar(CatalogoDeItens.IdSucataLimpa,5,"Sucata da borda");
                await Bruna(); await Missao("mestra_guilda","q_guilda_1_iniciacao");
                await Missao("mestra_guilda","q_guilda_1_iniciacao",true);
            });
            await Opcional("q_guilda_2_cacada",async()=>
            {
                await Bruna(); await Missao("mestra_guilda","q_guilda_2_cacada");
                await GarantirCity(); await Missao("Tácio","q_cobre_tacio");
                var q=CatalogoDeQuests.Obter("q_guilda_2_cacada");
                int reserva=3+(Concluida("q_cobre_tacio")?0:5)+(Concluida("q_tacio_linha")?0:10);
                string etapa=relatorio.etapa;
                for(int i=0;(G.ObterQuantidadeDeItem(CatalogoDeItens.IdFioDeCobre)<reserva || !GerenciadorDeQuests.ObjetivoCompleto(q,0)) && i<80;i++)
                {
                    await Cacar("Droid Selvagem"); ProgressoColeta(etapa,CatalogoDeItens.IdFioDeCobre,reserva,i+1);
                }
                Exigir(GerenciadorDeQuests.ObjetivoCompleto(q,0),"Caçada nao registrou cinco vitorias.");
                Exigir(G.ObterQuantidadeDeItem(CatalogoDeItens.IdFioDeCobre)>=reserva,"Reserva de cobre incompleta.");
                relatorio.etapa=etapa; await Bruna(); await Missao("mestra_guilda","q_guilda_2_cacada",true);
            });
            await Opcional("q_guilda_3_alfa",async()=>
            {
                await Bruna(); await Missao("mestra_guilda","q_guilda_3_alfa");
                await GarantirGame(); await PrepararCacada();
                if(!Flag("ato1_alfa_derrotado"))await Encontro("Selvagem Alfa");
                Exigir(G.ObterQuantidadeDeItem(CatalogoDeItens.IdSensorOptico)>0,"Sensor do Alfa nao esta na Bag.");
                await Bruna(); await Missao("mestra_guilda","q_guilda_3_alfa",true);
            });
            await Opcional("q_guilda_4_verdade",async()=>
            {
                await Bruna(); await Missao("mestra_guilda","q_guilda_4_verdade");
                await Missao("mestra_guilda","q_guilda_4_verdade",true);
            });
            await Opcional("q_cobre_tacio",async()=>
            {
                await Coletar(CatalogoDeItens.IdFioDeCobre,5,"Droid Selvagem",80);
                await GarantirCity(); await Missao("Tácio","q_cobre_tacio"); await Missao("Tácio","q_cobre_tacio",true);
            });
            await Opcional("q_tacio_linha",async()=>
            {
                await GarantirCity(); await Missao("Tácio","q_tacio_linha");
                await Coletar(CatalogoDeItens.IdFioDeCobre,10,"Droid Selvagem",80);
                await GarantirCity(); await Transitar("Interior_E11_Casa05","Porta_Casa05");
                var q=CatalogoDeQuests.Obter("q_tacio_linha");
                if(!GerenciadorDeQuests.ObjetivoCompleto(q,1))
                    await Puzzle("RoloDeCobre",CatalogoDePuzzles.Obter("p_contador_sucata").Solucao,"fv_p_contador_sucata_ok");
                Exigir(GerenciadorDeQuests.ObjetivoCompleto(q,1),"Contador nao registrou objetivo apos aceite.");
                await GarantirCity(); await Missao("Tácio","q_tacio_linha",true);
            });
            await Opcional("q_nilce_quarto",async()=>
            {
                await GarantirCity(); await Transitar("Interior_E08_Principal","Porta_PredioPlaca_B");
                await Missao("estalajadeira","q_nilce_quarto");
                if(!GerenciadorDeQuests.ObjetivoCompleto(CatalogoDeQuests.Obter("q_nilce_quarto"),0))
                    await Puzzle("BombaDeReserva",CatalogoDePuzzles.Obter("p_bomba_reserva").Solucao,"fv_p_bomba_reserva_ok");
                await Missao("estalajadeira","q_nilce_quarto",true);
            });
            await Opcional("q_ivo_terca",async()=>
            {
                await GarantirGame(); await Missao("Ivo","q_ivo_terca");
                if(!GerenciadorDeQuests.ObjetivoCompleto(CatalogoDeQuests.Obter("q_ivo_terca"),0))
                    await Puzzle("Calibragem da bomba dois",CatalogoDePuzzles.Obter("p_calibragem_braco").Solucao,"ato1_calibragem_feita");
                await Missao("Ivo","q_ivo_terca",true); await GarantirCity();
            });
            Exigir(Flag("ato1_concluido"),"Encerramento do Ato1 foi perdido.");
            Exigir(TodasAsQuestsEntregues(Concluida),"Catalogo ainda tem quests nao entregues: "+string.Join(",",CatalogoDeQuests.Todos.Keys.Where(id=>!Concluida(id))));
            Exigir(GerenciadorDeQuests.Ativas().Count==0,"Missao repetivel ficou reaceita ou ativa.");
            relatorio.resultado="PASSOU: todas as "+CatalogoDeQuests.Todos.Count+" quests entregues e Ato1 preservado";
            ValidacaoPercursoApresentacao.Capturar("tester-todas-quests");
        }
        catch(OperationCanceledException){relatorio.resultado="CANCELADO";}
        catch(Exception e)
        {
            relatorio.resultado="BLOQUEADO"; relatorio.erro=e.ToString();
            if(EditorApplication.isPlaying)ValidacaoPercursoApresentacao.Capturar("tester-opcionais-bloqueio");
            Debug.LogError("[TesterPlayer] "+relatorio.etapa+"\n"+e);
        }
        finally {relatorio.executando=false;relatorio.fim=DateTime.UtcNow.ToString("O");Gravar();}
    }
}
