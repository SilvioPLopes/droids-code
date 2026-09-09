using UnityEngine;
using DroidsCode.DroidCore;

// SCRIPT DESCARTAVEL — nao faz parte do jogo final.
// Anexe num GameObject vazio numa cena qualquer e rode o Play para validar
// Bloco 1 (dados puros) antes de avancar para CombatEngine/MoonSharp.
namespace DroidsCode._Teste
{
    public class TesteBloco1 : MonoBehaviour
    {
        private void Start()
        {
            var droid = new Droid("Apollo-01", hpMax: 100)
            {
                Braco = new Braco("Braco padrao", 5, Raridade.Comum),
                Perna = new Perna("Perna padrao", 3, Raridade.Comum),
                Tronco = new Tronco("Tronco padrao", 10, Raridade.Comum),
                Cabeca = new Cabeca("Cabeca padrao", 2, Raridade.Comum),
                StatsBase = new DroidStats { For = 5, Agi = 4, Vit = 6, Int = 3, Dex = 4, Luk = 2 }
            };

            Debug.Log($"Droid criado: {droid.Nome} | HP {droid.Hp}/{droid.HpMax}");
            Debug.Log($"FOR total: {droid.ObterTotal(TipoAtributo.For)}");

            var pontos = new PontosDeProgressao();
            pontos.GanharPontosPorNivel(2);
            Debug.Log($"Pontos disponiveis: {pontos.PontosDisponiveis}");

            var tecnica = new TecnicaComposta
            {
                Nome = "Golpe Perfurante",
                NivelDeDano = 3,
                EfeitosDeAtributo = { new EfeitoDeAtributo { NomeExibicao = "Stun", Atributo = TipoAtributo.Agi, Valor = -1, DuracaoEmTurnos = 1 } }
            };
            Debug.Log($"Custo da tecnica '{tecnica.Nome}': {tecnica.CustoTotal()} pontos");

            bool conseguiu = pontos.TentarGastar(tecnica.CustoTotal());
            Debug.Log(conseguiu
                ? $"Tecnica aprendida. Pontos restantes: {pontos.PontosDisponiveis}"
                : "Pontos insuficientes para aprender a tecnica.");
        }
    }
}
