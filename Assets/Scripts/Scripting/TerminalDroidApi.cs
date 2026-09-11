using System.Collections.Generic;
using DroidsCode.DroidCore;

namespace DroidsCode.Scripting
{
    // Fachada exposta ao Lua no terminal (fora de batalha). O MoonSharp NUNCA
    // registra Droid diretamente — so esta classe, que valida pontos antes de
    // aplicar qualquer mudanca. Ver CORRECAO_ARQUITETURA_TERMINAL_MENU.md.
    public class TerminalDroidApi
    {
        private readonly Droid _droid;
        private readonly List<string> _logDaSessao = new List<string>();

        public TerminalDroidApi(Droid droid)
        {
            _droid = droid;
        }

        // Ex. Lua: droid.subirAtributo("For", 1)
        public bool SubirAtributo(string nomeAtributo, int quantidade)
        {
            if (quantidade <= 0)
            {
                Registrar("Falha: quantidade deve ser maior que zero.");
                return false;
            }

            if (!System.Enum.TryParse(nomeAtributo, ignoreCase: true, out TipoAtributo atributo))
            {
                Registrar($"Falha: atributo '{nomeAtributo}' não existe.");
                return false; // nome de atributo invalido — nao lanca excecao, so recusa
            }

            int custo = quantidade * TabelaDeCustos.CustoUpgradeAtributo;
            if (!_droid.Pontos.TentarGastar(custo))
            {
                Registrar($"Falha: '{nomeAtributo}' custaria {custo} pontos, você só tem {_droid.Pontos.PontosDisponiveis}.");
                return false; // pontos insuficientes
            }

            AplicarUpgrade(atributo, quantidade);
            Registrar($"Sucesso: {nomeAtributo} +{quantidade} (custou {custo} pontos).");
            return true;
        }

        // Ex. Lua: droid.aprenderTecnica("Soco de Sobrecarga", 3)
        public bool AprenderTecnica(string nome, int nivelDeDano)
        {
            if (string.IsNullOrWhiteSpace(nome) || nivelDeDano <= 0)
            {
                Registrar("Falha: nome ou nível de dano inválido.");
                return false;
            }

            var tecnica = new TecnicaComposta { Nome = nome, NivelDeDano = nivelDeDano };

            if (!_droid.Pontos.TentarGastar(tecnica.CustoTotal()))
            {
                Registrar($"Falha: '{nome}' custaria {tecnica.CustoTotal()} pontos, você só tem {_droid.Pontos.PontosDisponiveis}.");
                return false;
            }

            _droid.TecnicasConfiguradas[nome] = tecnica;
            Registrar($"Sucesso: técnica '{nome}' aprendida.");
            return true;
        }

        public int ObterPontosDisponiveis()
        {
            return _droid.Pontos.PontosDisponiveis;
        }

        // --- Adicionados para o terminal com histórico (TerminalUIManager) ---

        public IReadOnlyList<string> ObterLogDaSessao() => _logDaSessao;

        public void LimparLog() => _logDaSessao.Clear();

        private void Registrar(string mensagem)
        {
            _logDaSessao.Add(mensagem);
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