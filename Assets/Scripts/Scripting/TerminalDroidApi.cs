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
            return TentarAprender(tecnica);
        }

        // Ex. Lua: droid.aprenderTecnicaComVeneno("Golpe Toxico", 2, 3, 2)
        // nivelDeDano = dano direto do golpe; danoPorTurno/duracaoEmTurnos = o Envenenamento aplicado no alvo.
        public bool AprenderTecnicaComVeneno(string nome, int nivelDeDano, int danoPorTurno, int duracaoEmTurnos)
        {
            if (string.IsNullOrWhiteSpace(nome) || nivelDeDano <= 0 || danoPorTurno <= 0 || duracaoEmTurnos <= 0)
            {
                Registrar("Falha: nome, nível de dano, dano por turno ou duração inválidos.");
                return false;
            }

            var tecnica = new TecnicaComposta { Nome = nome, NivelDeDano = nivelDeDano };
            tecnica.EfeitosDeDanoPorTurno.Add(new EfeitoDeDanoPorTurno
            {
                NomeExibicao = "Envenenamento",
                DanoPorTurno = danoPorTurno,
                DuracaoEmTurnos = duracaoEmTurnos
            });

            return TentarAprender(tecnica);
        }

        // Ex. Lua: droid.aprenderTecnicaComStun("Choque", 1, 1)
        // nivelDeDano = dano direto do golpe; duracaoEmTurnos = por quantos turnos o alvo perde a ação.
        public bool AprenderTecnicaComStun(string nome, int nivelDeDano, int duracaoEmTurnos)
        {
            if (string.IsNullOrWhiteSpace(nome) || nivelDeDano <= 0 || duracaoEmTurnos <= 0)
            {
                Registrar("Falha: nome, nível de dano ou duração inválidos.");
                return false;
            }

            var tecnica = new TecnicaComposta { Nome = nome, NivelDeDano = nivelDeDano };
            tecnica.EfeitosDeAtributo.Add(new EfeitoDeAtributo
            {
                // NomeExibicao "Stun" e o unico campo que importa aqui -- e uma
                // flag checada por nome em CombatEngine.EstaAtordoado(), nao
                // um buff/debuff de atributo de verdade. Atributo/Valor ficam
                // no default (For/0) de proposito, pra nao afetar ObterTotal.
                NomeExibicao = "Stun",
                Atributo = TipoAtributo.For,
                Valor = 0,
                DuracaoEmTurnos = duracaoEmTurnos
            });

            return TentarAprender(tecnica);
        }

        // Cobra o custo total da tecnica (dano + efeitos, ver TecnicaComposta.CustoTotal)
        // e registra, se houver pontos suficientes. Compartilhado pelas 3 variantes acima.
        private bool TentarAprender(TecnicaComposta tecnica)
        {
            int custo = tecnica.CustoTotal();
            if (!_droid.Pontos.TentarGastar(custo))
            {
                Registrar($"Falha: '{tecnica.Nome}' custaria {custo} pontos, você só tem {_droid.Pontos.PontosDisponiveis}.");
                return false;
            }

            _droid.TecnicasConfiguradas[tecnica.Nome] = tecnica;
            Registrar($"Sucesso: técnica '{tecnica.Nome}' aprendida (custou {custo} pontos).");
            return true;
        }

        public int ObterPontosDisponiveis()
        {
            return _droid.Pontos.PontosDisponiveis;
        }

        // Ex. Lua: droid.obterAtributo("For") -- leitura pura, nao gasta pontos
        public int ObterAtributo(string nomeAtributo)
        {
            if (!System.Enum.TryParse(nomeAtributo, ignoreCase: true, out TipoAtributo atributo))
            {
                Registrar($"Falha: atributo '{nomeAtributo}' não existe.");
                return 0;
            }

            return _droid.ObterTotal(atributo);
        }

        // Ex. Lua: droid.listarTecnicas()
        public string ListarTecnicas()
        {
            if (_droid.TecnicasConfiguradas.Count == 0)
            {
                return "Nenhuma técnica configurada.";
            }

            return string.Join(", ", _droid.TecnicasConfiguradas.Keys);
        }

        // Ex. Lua: droid.esquecerTecnica("Soco de Sobrecarga")
        // Devolve os pontos gastos na técnica (via CustoTotal()) ao esquecer.
        // O Ataque Básico nunca pode ser esquecido (é a ação padrão da Droid.cs).
        public bool EsquecerTecnica(string nome)
        {
            if (nome == Droid.NomeAtaqueBasico)
            {
                Registrar("Falha: o Ataque Básico não pode ser esquecido.");
                return false;
            }

            if (!_droid.TecnicasConfiguradas.TryGetValue(nome, out TecnicaComposta tecnica))
            {
                Registrar($"Falha: técnica '{nome}' não existe.");
                return false;
            }

            int reembolso = tecnica.CustoTotal();
            _droid.TecnicasConfiguradas.Remove(nome);
            _droid.Pontos.DefinirPontos(_droid.Pontos.PontosDisponiveis + reembolso);
            Registrar($"Sucesso: técnica '{nome}' esquecida ({reembolso} pontos devolvidos).");
            return true;
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