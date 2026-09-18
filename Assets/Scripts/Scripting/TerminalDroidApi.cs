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

            if (custo > _droid.Pontos.PontosDisponiveis)
            {
                Registrar($"Falha: '{nomeAtributo}' custaria {custo} pontos, você só tem {_droid.Pontos.PontosDisponiveis}.");
                return false; // pontos insuficientes
            }

            // SALVAGUARDA (17/09/2026 — softlock da primeira tecnica): enquanto
            // o Droid ainda nao tem NENHUMA tecnica alem do Ataque Basico, nao
            // deixa o saldo cair abaixo do custo da tecnica mais barata
            // possivel (NivelDeDano 1, sem efeitos = TabelaDeCustos.
            // CustoDanoPorNivel). Sem isso, um jogador com so 1 ponto (ex:
            // concedido por historia antes da primeira batalha) podia gastar
            // tudo em atributo e ficar sem nenhuma forma de passar por um
            // portao com TrancaPorHistoria.exigirTecnicaConfigurada — e sem
            // reembolso possivel, ja que so tecnica tem "desfazer"
            // (EsquecerTecnica), nao atributo. Deixa de se aplicar sozinha
            // assim que a primeira tecnica extra e aprendida.
            bool aindaSemTecnicaExtra = _droid.TecnicasConfiguradas.Count <= 1;
            if (aindaSemTecnicaExtra)
            {
                int saldoAposGasto = _droid.Pontos.PontosDisponiveis - custo;
                if (saldoAposGasto < TabelaDeCustos.CustoDanoPorNivel)
                {
                    Registrar($"Falha: gastar {custo} ponto(s) em '{nomeAtributo}' deixaria só {saldoAposGasto} — não dá pra aprender nenhuma técnica depois (a mais barata custa {TabelaDeCustos.CustoDanoPorNivel}). Aprenda sua primeira técnica antes de investir tudo em atributo.");
                    return false;
                }
            }

            _droid.Pontos.TentarGastar(custo); // saldo ja validado acima — sempre bem-sucedido aqui
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

            // CORRECAO (12/09/2026 — Bug 3): mesma protecao que ja existia em
            // EsquecerTecnica, replicada aqui -- sem isso dava pra
            // sobrescrever o Ataque Basico chamando aprenderTecnica com o
            // mesmo nome (e depois nao tinha como desfazer).
            if (nome == Droid.NomeAtaqueBasico)
            {
                Registrar("Falha: o Ataque Básico não pode ser sobrescrito.");
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

            // CORRECAO (12/09/2026 — Bug 3): mesma protecao do Ataque Basico
            // replicada aqui (ver AprenderTecnica).
            if (nome == Droid.NomeAtaqueBasico)
            {
                Registrar("Falha: o Ataque Básico não pode ser sobrescrito.");
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

            // CORRECAO (12/09/2026 — Bug 3): mesma protecao do Ataque Basico
            // replicada aqui (ver AprenderTecnica).
            if (nome == Droid.NomeAtaqueBasico)
            {
                Registrar("Falha: o Ataque Básico não pode ser sobrescrito.");
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
        //
        // CORRECAO (12/09/2026 — Bug 4): antes, reaprender um nome ja existente
        // sobrescrevia a tecnica antiga cobrando o custo total de novo -- os
        // pontos investidos na versao anterior sumiam sem reembolso ("perder e
        // recomecar do zero" a cada ajuste). Decisao tomada: BLOQUEAR
        // reaprendizado do mesmo nome aqui, e direcionar pro fluxo correto
        // (MelhorarTecnica, upgrade incremental que cobra so a diferenca).
        // A mensagem de log ja indica ao jogador qual comando usar em vez
        // disso -- serve de dica de uso dentro do proprio terminal.
        private bool TentarAprender(TecnicaComposta tecnica)
        {
            if (_droid.TecnicasConfiguradas.ContainsKey(tecnica.Nome))
            {
                Registrar($"Falha: a técnica '{tecnica.Nome}' já existe. Use droid.melhorarTecnica(\"{tecnica.Nome}\", nivelDeDanoAdicional) para evoluí-la, ou droid.esquecerTecnica(\"{tecnica.Nome}\") antes de reaprendê-la do zero.");
                return false;
            }

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

        // Ex. Lua: droid.melhorarTecnica("Soco de Sobrecarga", 2)
        // CORRECAO (12/09/2026 — Bug 4): upgrade incremental de uma tecnica ja
        // aprendida -- soma nivelDeDanoAdicional ao NivelDeDano existente e
        // cobra so a DIFERENCA de custo (CustoTotal novo - CustoTotal antigo),
        // nunca o custo cheio de novo. Segue o padrao mais elogiado em ARPGs
        // de referencia (releitar uma gema no PoE e upar, nao apagar e
        // recriar): menos punitivo, sem "sobrescrita silenciosa", e sem
        // reembolso+recompra manual. Efeitos (Veneno/Stun) da tecnica NAO sao
        // alterados por este metodo -- so o nivel de dano direto. Melhorar
        // efeitos fica fora de escopo por ora (nao pedido).
        public bool MelhorarTecnica(string nome, int nivelDeDanoAdicional)
        {
            if (string.IsNullOrWhiteSpace(nome) || nivelDeDanoAdicional <= 0)
            {
                Registrar("Falha: nome ou nível de dano adicional inválido.");
                return false;
            }

            if (nome == Droid.NomeAtaqueBasico)
            {
                Registrar("Falha: o Ataque Básico não pode ser melhorado.");
                return false;
            }

            if (!_droid.TecnicasConfiguradas.TryGetValue(nome, out TecnicaComposta tecnica))
            {
                Registrar($"Falha: a técnica '{nome}' não existe. Use droid.aprenderTecnica(\"{nome}\", nivelDeDano) primeiro.");
                return false;
            }

            int custoAntigo = tecnica.CustoTotal();
            int nivelAntigo = tecnica.NivelDeDano;
            tecnica.NivelDeDano += nivelDeDanoAdicional; // aplicado direto no objeto ja em TecnicasConfiguradas
            int custoNovo = tecnica.CustoTotal();
            int diferenca = custoNovo - custoAntigo;

            if (!_droid.Pontos.TentarGastar(diferenca))
            {
                // Reverte o aumento de nivel -- sem pontos suficientes, a
                // tecnica nao pode ficar num estado "melhorada mas nao paga".
                tecnica.NivelDeDano = nivelAntigo;
                Registrar($"Falha: melhorar '{nome}' de nível {nivelAntigo} para {nivelAntigo + nivelDeDanoAdicional} custaria {diferenca} pontos (diferença), você só tem {_droid.Pontos.PontosDisponiveis}.");
                return false;
            }

            Registrar($"Sucesso: técnica '{nome}' melhorada de nível {nivelAntigo} para {tecnica.NivelDeDano} (custou {diferenca} pontos de diferença).");
            return true;
        }

        public int ObterPontosDisponiveis()
        {
            return _droid.Pontos.PontosDisponiveis;
        }

        // Ex. Lua: droid.testeAdicionarPontos(500)
        // SOMENTE PARA TESTES -- ignora completamente a economia normal do
        // jogo (nivel/XP). Serve pra você testar builds de técnica/atributo
        // sem precisar farmar batalha toda vez. Considere remover ou esconder
        // isso antes de qualquer build final/demo pra terceiros.
        public void TesteAdicionarPontos(int quantidade)
        {
            _droid.Pontos.DefinirPontos(_droid.Pontos.PontosDisponiveis + quantidade);
            Registrar($"[TESTE] +{quantidade} pontos (total agora: {_droid.Pontos.PontosDisponiveis}).");
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

        // CORRECAO/REFATORACAO (Estagio 1 — 12/09/2026): HpMax virou calculado
        // a partir de VIT (ver Droid.HpMax). Subir VIT muda o teto de HP na
        // hora -- precisa capturar o HpMax ANTES da mudanca e chamar
        // RecalcularHpAposMudancaDeVit depois, ou o Hp atual nao acompanha o
        // novo teto (jogador ficaria "devendo" cura pra sempre).
        private void AplicarUpgrade(TipoAtributo atributo, int quantidade)
        {
            int hpMaxAntes = _droid.HpMax;

            switch (atributo)
            {
                case TipoAtributo.For: _droid.StatsBase.For += quantidade; break;
                case TipoAtributo.Agi: _droid.StatsBase.Agi += quantidade; break;
                case TipoAtributo.Vit: _droid.StatsBase.Vit += quantidade; break;
                case TipoAtributo.Int: _droid.StatsBase.Int += quantidade; break;
                case TipoAtributo.Dex: _droid.StatsBase.Dex += quantidade; break;
                case TipoAtributo.Luk: _droid.StatsBase.Luk += quantidade; break;
            }

            if (atributo == TipoAtributo.Vit)
            {
                _droid.RecalcularHpAposMudancaDeVit(hpMaxAntes);
            }
        }
    }
}