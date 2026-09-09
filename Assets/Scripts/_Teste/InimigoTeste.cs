using System.Collections.Generic;
using DroidsCode.Combat;

// FIXTURE DE TESTE — nao e o design final de inimigo (Sucata/Droid Selvagem/
// Droid Improvisado, ver MATRIZ_DESENVOLVIMENTO.md). So serve pra provar o
// CombatEngine ponta a ponta no Bloco 2.
namespace DroidsCode._Teste
{
    public class InimigoTeste : IParticipanteDeCombate
    {
        public string Nome { get; }
        public int Hp { get; set; }
        public int HpMax { get; }
        public int Defesa { get; }

        public InimigoTeste(string nome, int hpMax, int defesa)
        {
            Nome = nome;
            HpMax = hpMax;
            Hp = hpMax;
            Defesa = defesa;
        }

        public IEnumerable<string> ObterAcoesDisponiveis() => new List<string>();

        public ResultadoAcao ExecutarAcao(string nomeAcao, IParticipanteDeCombate alvo)
        {
            return new ResultadoAcao { Sucesso = false, DanoCausado = 0, Mensagem = "Inimigo de teste nao ataca." };
        }
    }
}
