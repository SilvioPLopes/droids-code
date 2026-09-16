using System.Collections.Generic;
using DroidsCode.DroidCore;

namespace DroidsCode.Combat
{
    /// <summary>
    /// Catalogo estatico de inimigos, mesmo padrao ja usado e aprovado em
    /// CatalogoDeItens (Dictionary<string, Definicao>, Id string estavel,
    /// Obter(id) devolvendo null em vez de lancar excecao).
    ///
    /// POR QUE ISSO EXISTE: antes, o inimigo de uma batalha era digitado no
    /// Inspector de cada cena Battle. Com o Ato 1 trazendo varios inimigos,
    /// isso viraria N cenas Battle duplicadas e N lugares pra esquecer de
    /// preencher tabelaDeDrops -- pendencia ja aberta no CHECKLIST. Com o
    /// catalogo existe UMA cena Battle: quem inicia a batalha escreve o Id em
    /// GerenciadorDeEstado.ProximoInimigoId e o BattleManager se monta.
    /// O Inspector continua valendo como fallback -- cena existente nao quebra.
    ///
    /// ===================================================================
    /// ⚠️ NOTA DE BALANCEAMENTO (15/09/2026) -- LER ANTES DE MEXER NOS NUMEROS
    /// ===================================================================
    /// Ao conferir a formula real de acerto (Droid.RolarAcerto +
    /// TabelaDeCombate) contra o Droid inicial de GerenciadorDeEstado
    /// (For=5, Vit=3, TODO O RESTO EM ZERO), aparece um problema serio:
    ///
    ///   Hit do jogador  = DEX total = 0
    ///   Flee do jogador = AGI total = 0
    ///   chance = clamp(50 + (Hit - FleeAlvo), 5, 95)
    ///
    /// Com os valores padrao de inimigo (HitPadraoInimigo = 10,
    /// FleePadraoInimigo = 10):
    ///   - jogador acerta o inimigo em 50 + (0 - 10) = **40%** das vezes
    ///   - inimigo acerta o jogador em 50 + (10 - 0) = **60%** das vezes
    ///
    /// Ou seja: o jogador comeca o jogo em desvantagem de acerto contra o
    /// primeiro inimigo da historia, e tem 0% de critico (LUK 0) e 0% de
    /// resistencia a efeito (INT 0). Isso nao e opiniao de dificuldade -- e
    /// um jogo que pune o jogador por nao ter aprendido ainda um sistema que
    /// o jogo nao ensinou.
    ///
    /// TRES CORRECOES FORAM APLICADAS, nesta ordem de preferencia:
    ///  1. (AQUI) Os inimigos do inicio do Ato 1 tem Flee BAIXO (2 a 4) em
    ///     vez do padrao 10. A Sucata de Captura, primeiro inimigo do jogo,
    ///     tem Flee 2 -- o jogador acerta 48% mesmo com DEX 0, e chega perto
    ///     de 80% com 3 pontos investidos em DEX.
    ///  2. (Enredo) A fala do Apollo antes do primeiro combate ensina
    ///     explicitamente droid.subirAtributo("Dex", 3) -- o atributo deixa
    ///     de ser adivinhacao e vira licao.
    ///  3. (Itens) "Oleo de Precisao" (+15 DEX por 4 turnos) e "Sensor
    ///     Optico" (+6 DEX permanente) existem como rede de seguranca.
    ///
    /// A correcao que NAO foi aplicada, e que talvez seja a certa: mudar os
    /// stats iniciais do Droid em GerenciadorDeEstado.Awake (ex: DEX 3,
    /// AGI 2, LUK 1). Nao fiz porque mexer nos stats iniciais muda todo o
    /// balanceamento que voce ja testou em batalha real -- e decisao sua,
    /// nao minha. Esta registrado no CHECKLIST como pendencia.
    /// </summary>
    public static class CatalogoDeInimigos
    {
        // Ids estaveis -- NUNCA renomear (vao pro Inspector e pros objetivos
        // de quest). Mesma regra de CatalogoDeItens.Id*.
        public const string IdSucataDeCaptura = "sucata_captura";
        public const string IdSucataReforcada = "sucata_reforcada";
        public const string IdDroidSelvagem = "droid_selvagem";
        public const string IdDroidSelvagemAlfa = "droid_selvagem_alfa";
        public const string IdCarcacaErrante = "carcaca_errante";
        public const string IdDroneDeReconhecimento = "drone_reconhecimento";
        public const string IdDroidDoIvo = "droid_do_ivo";
        public const string IdEsqueletoTeste = "esqueleto_teste";

        public static readonly Dictionary<string, DefinicaoDeInimigo> Todos =
            new Dictionary<string, DefinicaoDeInimigo>
            {
                // =========================================================
                // SESSAO 3 -- primeiro combate do jogo.
                // Fragil de proposito: o Enredo diz que sucatas de captura
                // sao "rapidas, mas frageis, construidas pra imobilizar
                // presas desarmadas". Serve pra ensinar o loop de turno,
                // nao pra ameacar. Flee 2 (nao 10) pela nota acima.
                // =========================================================
                [IdSucataDeCaptura] = new DefinicaoDeInimigo
                {
                    Id = IdSucataDeCaptura,
                    Nome = "Sucata de Captura",
                    HpMax = 24, Ataque = 4, Defesa = 1,
                    Hit = 6, Flee = 2, ChanceCritica = 2f, ChanceDeResistirEfeito = 0f,
                    RecompensaXp = 45, GoldMinimo = 3, GoldMaximo = 7,
                    Descricao = "Droid magro, rápido, com garras em vez de mãos. Projetado pra imobilizar, não pra matar. Isso, de alguma forma, é pior.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPocaoPequena, Chance = 0.5f },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdSucataLimpa, Chance = 0.6f }
                    }
                },

                // Versao mais dura, pra quest de caça e encontro tardio.
                [IdSucataReforcada] = new DefinicaoDeInimigo
                {
                    Id = IdSucataReforcada,
                    Nome = "Sucata Reforçada",
                    HpMax = 40, Ataque = 7, Defesa = 3,
                    Hit = 10, Flee = 5, ChanceCritica = 4f, ChanceDeResistirEfeito = 5f,
                    RecompensaXp = 80, GoldMinimo = 6, GoldMaximo = 12,
                    Descricao = "Uma sucata de captura que sobreviveu tempo suficiente pra ser remendada pela própria Apex.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdSucataLimpa, Chance = 0.8f, QuantidadeMinima = 1, QuantidadeMaxima = 2 },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdFioDeCobre, Chance = 0.4f }
                    }
                },

                // =========================================================
                // SESSAO 5 -- fauna da Cratera. E o inimigo de farm: aqui o
                // jogador junta XP/pontos pro terminal e Gold pra Loja antes
                // do chefe. Erratico = dano alto, defesa e esquiva baixas.
                // =========================================================
                [IdDroidSelvagem] = new DefinicaoDeInimigo
                {
                    Id = IdDroidSelvagem,
                    Nome = "Droid Selvagem",
                    HpMax = 38, Ataque = 8, Defesa = 2,
                    Hit = 12, Flee = 4, ChanceCritica = 6f, ChanceDeResistirEfeito = 0f,
                    RecompensaXp = 75, GoldMinimo = 5, GoldMaximo = 11,
                    Descricao = "Máquina desconectada de qualquer rede, que aprendeu sozinha a caçar energia pela Cratera. Os olhos brilham um azul instável, nada parecido com a calma dos do Apollo.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPocaoPequena, Chance = 0.45f, QuantidadeMinima = 1, QuantidadeMaxima = 2 },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdFioDeCobre, Chance = 0.5f },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPocaoMedia, Chance = 0.12f }
                    }
                },

                [IdDroidSelvagemAlfa] = new DefinicaoDeInimigo
                {
                    Id = IdDroidSelvagemAlfa,
                    Nome = "Selvagem Alfa",
                    HpMax = 60, Ataque = 12, Defesa = 4,
                    Hit = 16, Flee = 8, ChanceCritica = 10f, ChanceDeResistirEfeito = 10f,
                    RecompensaXp = 160, GoldMinimo = 15, GoldMaximo = 28,
                    EhUnico = true,
                    Descricao = "Maior que os outros, montado com pedaços de três droids diferentes. Alguma coisa nele aprendeu a esperar antes de atacar.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdSensorOptico, Chance = 1f },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPocaoMedia, Chance = 1f, QuantidadeMinima = 2, QuantidadeMaxima = 2 }
                    }
                },

                // Inimigo lento e duro: existe pra ensinar que atacar nem
                // sempre e a melhor acao (Debuff de VIT rende mais aqui).
                [IdCarcacaErrante] = new DefinicaoDeInimigo
                {
                    Id = IdCarcacaErrante,
                    Nome = "Carcaça Errante",
                    HpMax = 55, Ataque = 6, Defesa = 8,
                    Hit = 8, Flee = 1, ChanceCritica = 0f, ChanceDeResistirEfeito = 15f,
                    RecompensaXp = 90, GoldMinimo = 8, GoldMaximo = 14,
                    Descricao = "Um droid industrial que nunca parou de andar. Blindagem grossa, reflexos nenhum. Bater de frente é o jeito lento de vencer.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdSucataLimpa, Chance = 1f, QuantidadeMinima = 2, QuantidadeMaxima = 3 },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPlacaDeBlindagem, Chance = 0.15f }
                    }
                },

                // Rapido e evasivo: o contraponto da Carcaca. Sem DEX, o
                // jogador sente na pele por que o atributo existe.
                [IdDroneDeReconhecimento] = new DefinicaoDeInimigo
                {
                    Id = IdDroneDeReconhecimento,
                    Nome = "Drone de Reconhecimento",
                    HpMax = 26, Ataque = 9, Defesa = 1,
                    Hit = 18, Flee = 14, ChanceCritica = 8f, ChanceDeResistirEfeito = 0f,
                    RecompensaXp = 110, GoldMinimo = 10, GoldMaximo = 18,
                    Descricao = "Não vem sozinho, nunca. Se você está vendo um, alguém já sabe onde você está.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdSensorOptico, Chance = 0.25f },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdFioDeCobre, Chance = 0.7f, QuantidadeMinima = 1, QuantidadeMaxima = 2 }
                    }
                },

                // =========================================================
                // SESSAO 6 -- chefe do Ato 1.
                // O Enredo pede uma luta que "se estende mais que nas outras
                // rotas" na rota Desgarrado: HP alto, dano alto, vencivel com
                // o build que o jogador escreveu no terminal. Quem escolheu
                // Ferro entra com o Nucleo equipado (+10 FOR); quem escolheu
                // Lotus entra com a Ressonancia (Stun 3 turnos). Desgarrado
                // enfrenta isto cru -- por isso estes numeros sao o TETO do
                // balanceamento, nao a media.
                // =========================================================
                [IdDroidDoIvo] = new DefinicaoDeInimigo
                {
                    Id = IdDroidDoIvo,
                    Nome = "Droid Improvisado",
                    HpMax = 95, Ataque = 13, Defesa = 5,
                    Hit = 14, Flee = 6, ChanceCritica = 8f, ChanceDeResistirEfeito = 5f,
                    RecompensaXp = 300, GoldMinimo = 20, GoldMaximo = 40,
                    EhChefe = true, EhUnico = true, PermiteFugir = false,
                    Descricao = "Partes de um droid industrial antigo, soldadas com placas da própria estação. Não é um droid de guerra de verdade. É pior — é algo feito por desespero, sem nenhuma das salvaguardas que um projeto de combate teria.",
                    Drops = new List<DropDeInimigo>
                    {
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPocaoMedia, Chance = 1f, QuantidadeMinima = 2, QuantidadeMaxima = 2 },
                        new DropDeInimigo { IdItem = CatalogoDeItens.IdPecaDaPernaDoApollo, Chance = 1f }
                    }
                },

                // =========================================================
                // Compatibilidade: inimigo default que ja existia no
                // Inspector da cena Battle. Mantido pra nao quebrar testes.
                // =========================================================
                [IdEsqueletoTeste] = new DefinicaoDeInimigo
                {
                    Id = IdEsqueletoTeste,
                    Nome = "Esqueleto",
                    HpMax = 20, Ataque = 5, Defesa = 1,
                    Hit = TabelaDeCombate.HitPadraoInimigo,
                    Flee = TabelaDeCombate.FleePadraoInimigo,
                    ChanceCritica = TabelaDeCombate.ChanceCriticoPadraoInimigo,
                    ChanceDeResistirEfeito = TabelaDeCombate.ResistenciaEfeitoPadraoInimigo,
                    RecompensaXp = 10, GoldMinimo = 0, GoldMaximo = 2,
                    Descricao = "Inimigo de teste, anterior ao Ato 1.",
                    Drops = new List<DropDeInimigo>()
                }
            };

        /// <summary>
        /// Devolve null pra Id desconhecido (ex: erro de digitacao no
        /// Inspector) -- mesmo contrato de CatalogoDeItens.Obter, pra quem
        /// chama poder cair no fallback em vez de quebrar a cena.
        /// </summary>
        public static DefinicaoDeInimigo Obter(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return Todos.TryGetValue(id, out DefinicaoDeInimigo definicao) ? definicao : null;
        }
    }

    public class DefinicaoDeInimigo
    {
        public string Id;
        public string Nome;
        public string Descricao = "";

        public int HpMax;
        public int Ataque;
        public int Defesa;

        // NOVO (15/09/2026): atributos derivados por inimigo. InimigoFixo
        // expoe Hit/Flee/ChanceCritica/ChanceDeResistirEfeito como
        // { get; set; }, inicializados com TabelaDeCombate.*PadraoInimigo --
        // ou seja, TODO inimigo tinha exatamente a mesma pericia. Agora o
        // BattleManager sobrescreve depois de construir. Ver a nota de
        // balanceamento no topo da classe.
        public int Hit = TabelaDeCombate.HitPadraoInimigo;
        public int Flee = TabelaDeCombate.FleePadraoInimigo;
        public float ChanceCritica = TabelaDeCombate.ChanceCriticoPadraoInimigo;
        public float ChanceDeResistirEfeito = TabelaDeCombate.ResistenciaEfeitoPadraoInimigo;

        public int RecompensaXp;
        public int GoldMinimo;
        public int GoldMaximo;

        /// <summary>
        /// Chefe: dano e HP fora da curva, luta de historia. Usado pela UI
        /// (nome em destaque) e por EncounterZone, que nunca sorteia chefe.
        /// </summary>
        public bool EhChefe = false;

        /// <summary>
        /// Inimigo unico no jogo (nao spawna de novo). EncounterZone recusa
        /// sortear; GatilhoDeBatalha trava pela flag de vitoria. E a resposta
        /// a "lutas com personagens importantes nao podem ser spamaveis".
        /// </summary>
        public bool EhUnico = false;

        /// <summary>
        /// Se false, o botao Fugir fica desabilitado nesta batalha. Fugir de
        /// um chefe de historia deixaria o jogador num estado onde a flag de
        /// vitoria nunca e gravada e o roteiro trava sem explicacao.
        /// </summary>
        public bool PermiteFugir = true;

        public List<DropDeInimigo> Drops = new List<DropDeInimigo>();
    }

    /// <summary>
    /// Espelho de BattleManager.DropDeItem, mas em C# puro (sem
    /// UnityEngine), pra respeitar a convencao da secao 7 da
    /// DOCUMENTACAO_TECNICA: classe de dominio nao referencia MonoBehaviour.
    /// O BattleManager converte um no outro ao montar a batalha.
    /// </summary>
    public class DropDeInimigo
    {
        public string IdItem;
        public float Chance = 1f;
        public int QuantidadeMinima = 1;
        public int QuantidadeMaxima = 1;
    }
}
