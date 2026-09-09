using DroidsCode.DroidCore;

namespace DroidsCode.Scripting
{
    // Fachada exposta ao Lua no terminal (fora de batalha). O MoonSharp NUNCA
    // registra Droid diretamente — so esta classe, que valida pontos antes de
    // aplicar qualquer mudanca. Ver CORRECAO_ARQUITETURA_TERMINAL_MENU.md.
    public class TerminalDroidApi
    {
        private readonly Droid _droid;
        private readonly PontosDeProgressao _pontos;

        public TerminalDroidApi(Droid droid, PontosDeProgressao pontos)
        {
            _droid = droid;
            _pontos = pontos;
        }

        // Ex. Lua: droid.subirAtributo("For", 1)
        public bool SubirAtributo(string nomeAtributo, int quantidade)
        {
            if (quantidade <= 0)
            {
                return false;
            }

            if (!System.Enum.TryParse(nomeAtributo, ignoreCase: true, out TipoAtributo atributo))
            {
                return false; // nome de atributo invalido — nao lanca excecao, so recusa
            }

            int custo = quantidade * TabelaDeCustos.CustoUpgradeAtributo;
            if (!_pontos.TentarGastar(custo))
            {
                return false; // pontos insuficientes
            }

            AplicarUpgrade(atributo, quantidade);
            return true;
        }

        // Ex. Lua: droid.aprenderTecnica("Soco de Sobrecarga", 3)
        public bool AprenderTecnica(string nome, int nivelDeDano)
        {
            if (string.IsNullOrWhiteSpace(nome) || nivelDeDano <= 0)
            {
                return false;
            }

            var tecnica = new TecnicaComposta { Nome = nome, NivelDeDano = nivelDeDano };

            if (!_pontos.TentarGastar(tecnica.CustoTotal()))
            {
                return false;
            }

            _droid.TecnicasConfiguradas[nome] = tecnica;
            return true;
        }

        public int ObterPontosDisponiveis()
        {
            return _pontos.PontosDisponiveis;
        }

        private void AplicarUpgrade(TipoAtributo atributo, int quantidade)
        {
            switch (atributo)
            {
                case TipoAtributo.For: _droid.StatsBase.For += quantidade; break;
                case TipoAtributo.Agi: _droid.StatsBase.Agi += quantidade; break;
                case TipoAtributo.Vit: _droid.StatsBase.Vit += quantidade; break;
                case TipoAtributo.Int: _droid.StatsBase.Int += quantidade; break;
                case TipoAtributo.Dex: _droid.StatsBase.Dex += quantidade; break;
                case TipoAtributo.Luk: _droid.StatsBase.Luk += quantidade; break;
            }
        }
    }
}
