using UnityEngine;
using DroidsCode.DroidCore;
using DroidsCode.Combat;
using DroidsCode.Scripting;

// SCRIPT DESCARTAVEL — nao faz parte do jogo final.
// Testa Bloco 2: terminal Lua configurando o Droid fora de batalha, depois
// CombatEngine resolvendo um turno sem chamar Lua nenhuma vez.
namespace DroidsCode._Teste
{
    public class TesteBloco2 : MonoBehaviour
    {
        private void Start()
        {
            var droid = new Droid("Apollo-01", hpMax: 100)
            {
                StatsBase = new DroidStats { For = 5, Agi = 4, Vit = 6, Int = 3, Dex = 4, Luk = 2 }
            };

            var pontos = new PontosDeProgressao();
            pontos.GanharPontosPorNivel(3); // 15 pontos

            var api = new TerminalDroidApi(droid, pontos);
            var runner = new DroidScriptRunner(api);

            // Fase de preparacao — terminal, fora de batalha.
            string codigoLua = @"
                droid.subirAtributo(""For"", 2)
                droid.aprenderTecnica(""Soco de Sobrecarga"", 3)
            ";

            ResultadoExecucao execucao = runner.ExecutarCodigo(codigoLua);
            Debug.Log(execucao.Sucesso
                ? $"Codigo executado. Pontos restantes: {api.ObterPontosDisponiveis()}"
                : $"Erro no Lua: {execucao.MensagemErro}");

            Debug.Log($"FOR apos upgrade: {droid.ObterTotal(TipoAtributo.For)}");

            // Fase de combate — menu fixo, sem Lua.
            var inimigo = new InimigoTeste("Sucata-01", hpMax: 50, defesa: 3);
            var engine = new CombatEngine();

            ResultadoAcao resultado = engine.ExecutarTurno(droid, inimigo, "Soco de Sobrecarga");
            Debug.Log(resultado.Mensagem);
            Debug.Log($"HP do inimigo apos o turno: {inimigo.Hp}/{inimigo.HpMax}");
            Debug.Log($"Inimigo derrotado? {engine.VerificarDerrota(inimigo)}");
        }
    }
}
