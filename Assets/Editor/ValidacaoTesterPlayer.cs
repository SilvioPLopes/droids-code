using System;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class ValidacaoTesterPlayer
{
    public static string Executar()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Fixture exige fora de Play.");
        var tipo = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("TesterPlayer")).FirstOrDefault(t => t != null);
        if (tipo == null) throw new InvalidOperationException("TesterPlayer ainda nao implementado.");
        string cena = SceneManager.GetActiveScene().path;
        string resultado = (string)tipo.GetMethod("Iniciar").Invoke(null, null);
        if (!resultado.StartsWith("RECUSADO:")) throw new Exception("Iniciar fora de Play deveria ser recusado.");
        if (EditorApplication.isPlaying || SceneManager.GetActiveScene().path != cena)
            throw new Exception("Iniciar sem permissao mudou o estado do Editor.");
        tipo.GetMethod("Cancelar").Invoke(null, null);
        string estado = (string)tipo.GetMethod("Estado").Invoke(null, null);
        if (estado.Contains("\"executando\":true")) throw new Exception("Cancelamento deixou execucao ativa.");
        string movimento = ValidacaoPercursoApresentacao.Caminhar(0, 0).GetAwaiter().GetResult();
        if (movimento != "CANCELADO") throw new Exception("Movimento cancelado tentou acessar o player.");
        ValidacaoPercursoApresentacao.CancelamentoSolicitado = false;
        var seguro = typeof(ValidacaoPercursoApresentacao).GetMethod("VidaSeguraParaBoss");
        if (seguro == null) throw new Exception("Estrategia segura de corte ainda nao implementada.");
        int corte = (int)seguro.Invoke(null, new object[] { 18, 14, 0, DroidsCode.Combat.DroidDoIvo.Corte });
        if (corte != 32) throw new Exception("Corte critico deve prever32 de dano contra DEF14.");
        int reforcado = (int)seguro.Invoke(null, new object[] { 18, 19, 2, DroidsCode.Combat.DroidDoIvo.Corte });
        if (reforcado != 24) throw new Exception("Corte com DEF19 e dano de turno2 deve prever24.");
        var retomada = tipo.GetMethod("DeterminarEtapaDeRetomada");
        if (retomada == null) throw new Exception("Retomada de checkpoint ainda nao implementada.");
        Func<string, bool> bossVencido = f => f == "ato1_sessao6";
        if ((int)retomada.Invoke(null, new object[] { bossVencido }) != 9)
            throw new Exception("Boss vencido deve retomar epilogo sem repetir luta.");
        var todas=tipo.GetMethod("TodasAsQuestsEntregues");
        if(todas==null)throw new Exception("Aceite de todas as opcionais ainda nao implementado.");
        Func<string,bool> principais=id=>new[]{"q_main_agua","q_main_estacao","q_main_partida","q_agua_dara","q_perna_apollo","q_limpeza_borda","q_onofre_provas"}.Contains(id);
        if((bool)todas.Invoke(null,new object[]{principais}))throw new Exception("Sete quests nao podem aprovar todas as18.");
        Func<string,bool> completa=id=>true;
        if(!(bool)todas.Invoke(null,new object[]{completa}))throw new Exception("Todas concluídas deveriam aprovar.");
        Func<string,bool> faltaUltima=id=>id!="q_guilda_4_verdade";
        if((bool)todas.Invoke(null,new object[]{faltaUltima}))throw new Exception("GuildaIV pendente deve reprovar.");
        return "PASSOU: inicio explicito, recusa fora de Play e cancelamento ocioso seguros.";
    }
}
