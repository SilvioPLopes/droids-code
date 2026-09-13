using System.Collections.Generic;

namespace DroidsCode.DroidCore
{
    // Estagio 3 (fase 2 do sistema de item, sem alinhamento extra -- ver
    // conversa) -- expande o catalogo de Cura (estagio 2, 13/09/2026) para
    // as 4 categorias que faltavam. Ver CHECKLIST_DE_DESENVOLVIMENTO.md,
    // secao "Sistema de Item / Inventario real".
    public enum TipoDeItem
    {
        Cura,
        Buff,
        Debuff,
        Equipavel,
        ForaDeBatalha
    }

    // Estagio 3: identifica QUAL logica de uso-fora-de-batalha rodar (ver
    // BagUIManager.UsarAcaoForaDeBatalha) -- o dado de cada acao (destino,
    // chave de flag) fica nos campos de DefinicaoDeItem abaixo.
    public enum AcaoForaDeBatalha
    {
        SinalizadorDeRetorno,
        KitDeAcampamento,
        ChaveDeAcesso
    }

    public class DefinicaoDeItem
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public TipoDeItem Tipo { get; set; }
        public bool UsavelEmBatalha { get; set; } = true;
        public bool UsavelForaDeBatalha { get; set; } = true;

        // --- Cura (Estagio 2) ---
        public int CuraHp { get; set; }

        // --- Buff / Debuff (Estagio 3) ---
        // Reaproveita as MESMAS classes de efeito que TecnicaComposta ja usa
        // (EfeitoDeAtributo / EfeitoDeDanoPorTurno) -- decisao tomada sem
        // alinhamento extra, em vez de criar uma estrutura de efeito nova
        // so pra item. Buff aplica no proprio Droid; Debuff aplica no
        // inimigo e rola resistencia (ver BattleManager.AplicarEfeitoDeItem).
        // Um item pode preencher os dois campos (raro) ou so um -- null =
        // "nao usa esse tipo de efeito".
        public EfeitoDeAtributo EfeitoDeAtributoAplicado { get; set; }
        public EfeitoDeDanoPorTurno EfeitoDeDanoAplicado { get; set; }

        // --- Equipavel (Estagio 3) ---
        public TipoDePeca? SlotDePeca { get; set; }
        public TipoAtributo? AtributoBonificado { get; set; }
        public int ValorDoBonus { get; set; }

        // --- ForaDeBatalha (Estagio 3) ---
        public AcaoForaDeBatalha? AcaoForaDeBatalha { get; set; }
        // Usado so pela ChaveDeAcesso: chave de flag gravada em
        // GerenciadorDeEstado.DefinirFlag ao usar o item. Nao destranca
        // nada de mapa especifico ainda (isso cruzaria com conteudo de
        // historia, fora de escopo tecnico) -- so seta a flag generica.
        public string ChaveDeFlag { get; set; }
    }

    // Catalogo estatico -- mesmo espirito de TabelaDeCombate/TabelaDeCustos.
    // O INVENTARIO (quantidade que o jogador possui) fica em
    // GerenciadorDeEstado, nao aqui.
    public static class CatalogoDeItens
    {
        // --- Cura (Estagio 2) ---
        public const string IdPocaoPequena = "pocao_pequena";
        public const string IdPocaoMedia = "pocao_media";
        public const string IdPocaoGrande = "pocao_grande";
        public const string IdReparoCompleto = "reparo_completo";

        // --- Buff (Estagio 3) ---
        public const string IdEscudoDeEmergencia = "escudo_de_emergencia";

        // --- Debuff (Estagio 3) ---
        public const string IdGranadaDeAcido = "granada_de_acido";
        public const string IdDisruptorEmp = "disruptor_emp";
        public const string IdCorrosivo = "corrosivo";

        // --- Equipavel (Estagio 3) ---
        // Exemplos minimos pra validar o sistema -- balanceamento real e
        // mais itens ficam pra quando houver drop/loja calibrados.
        public const string IdNucleoDeEnergia = "nucleo_de_energia";
        public const string IdServoMotor = "servo_motor";

        // --- ForaDeBatalha (Estagio 3) ---
        public const string IdSinalizadorDeRetorno = "sinalizador_de_retorno";
        public const string IdKitDeAcampamento = "kit_de_acampamento";
        public const string IdChaveDeAcesso = "chave_de_acesso";

        public static readonly Dictionary<string, DefinicaoDeItem> Todos = new Dictionary<string, DefinicaoDeItem>
        {
            [IdPocaoPequena] = new DefinicaoDeItem { Id = IdPocaoPequena, Nome = "Poção Pequena", Tipo = TipoDeItem.Cura, CuraHp = 20 },
            [IdPocaoMedia] = new DefinicaoDeItem { Id = IdPocaoMedia, Nome = "Poção Média", Tipo = TipoDeItem.Cura, CuraHp = 50 },
            [IdPocaoGrande] = new DefinicaoDeItem { Id = IdPocaoGrande, Nome = "Poção Grande", Tipo = TipoDeItem.Cura, CuraHp = 80 },
            [IdReparoCompleto] = new DefinicaoDeItem { Id = IdReparoCompleto, Nome = "Reparo Completo", Tipo = TipoDeItem.Cura, CuraHp = 9999 },

            // Buff: +10 de VIT (Defesa, ver Droid.Defesa) por 3 turnos, no proprio Droid.
            [IdEscudoDeEmergencia] = new DefinicaoDeItem
            {
                Id = IdEscudoDeEmergencia,
                Nome = "Escudo de Emergência",
                Tipo = TipoDeItem.Buff,
                UsavelForaDeBatalha = false,
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                {
                    NomeExibicao = "Escudo",
                    Atributo = TipoAtributo.Vit,
                    Valor = 10,
                    DuracaoEmTurnos = 3
                }
            },

            // Debuff: -5 de VIT (Defesa) do inimigo por 3 turnos.
            [IdGranadaDeAcido] = new DefinicaoDeItem
            {
                Id = IdGranadaDeAcido,
                Nome = "Granada de Ácido",
                Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false,
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                {
                    NomeExibicao = "Corrosão",
                    Atributo = TipoAtributo.Vit,
                    Valor = -5,
                    DuracaoEmTurnos = 3
                }
            },

            // Debuff: reaproveita a flag "Stun" que CombatEngine.EstaAtordoado
            // ja checa por nome (mesmo mecanismo do Stun de tecnica) -- inimigo
            // perde 1 turno.
            [IdDisruptorEmp] = new DefinicaoDeItem
            {
                Id = IdDisruptorEmp,
                Nome = "Disruptor EMP",
                Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false,
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                {
                    NomeExibicao = "Stun",
                    Atributo = TipoAtributo.For,
                    Valor = 0,
                    DuracaoEmTurnos = 1
                }
            },

            // Debuff: dano continuo, mesmo padrao do Envenenamento de tecnica.
            // ATENCAO: herda os 2 bugs abertos do motor de efeitos (nao zera
            // entre batalhas / empilha sem limite) -- ver checklist, nao
            // corrigido aqui de proposito (exige decisao de design propria).
            [IdCorrosivo] = new DefinicaoDeItem
            {
                Id = IdCorrosivo,
                Nome = "Corrosivo",
                Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false,
                EfeitoDeDanoAplicado = new EfeitoDeDanoPorTurno
                {
                    NomeExibicao = "Corrosivo",
                    DanoPorTurno = 5,
                    DuracaoEmTurnos = 3
                }
            },

            // Equipavel: +5 FOR permanente no Tronco.
            [IdNucleoDeEnergia] = new DefinicaoDeItem
            {
                Id = IdNucleoDeEnergia,
                Nome = "Núcleo de Energia",
                Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false,
                SlotDePeca = TipoDePeca.Tronco,
                AtributoBonificado = TipoAtributo.For,
                ValorDoBonus = 5
            },

            // Equipavel: +5 AGI permanente na Perna.
            [IdServoMotor] = new DefinicaoDeItem
            {
                Id = IdServoMotor,
                Nome = "Servo-Motor",
                Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false,
                SlotDePeca = TipoDePeca.Perna,
                AtributoBonificado = TipoAtributo.Agi,
                ValorDoBonus = 5
            },

            // ForaDeBatalha: "retorna ao ultimo save" -- reaproveita
            // SalvamentoJson.Carregar() inteiro em vez de um teleporte
            // proprio (ver BagUIManager.UsarAcaoForaDeBatalha).
            [IdSinalizadorDeRetorno] = new DefinicaoDeItem
            {
                Id = IdSinalizadorDeRetorno,
                Nome = "Sinalizador de Retorno",
                Tipo = TipoDeItem.ForaDeBatalha,
                UsavelEmBatalha = false,
                AcaoForaDeBatalha = DroidsCode.DroidCore.AcaoForaDeBatalha.SinalizadorDeRetorno
            },

            // ForaDeBatalha: cura completa + salva na hora.
            [IdKitDeAcampamento] = new DefinicaoDeItem
            {
                Id = IdKitDeAcampamento,
                Nome = "Kit de Acampamento",
                Tipo = TipoDeItem.ForaDeBatalha,
                UsavelEmBatalha = false,
                AcaoForaDeBatalha = DroidsCode.DroidCore.AcaoForaDeBatalha.KitDeAcampamento
            },

            // ForaDeBatalha: seta uma flag de historia generica.
            [IdChaveDeAcesso] = new DefinicaoDeItem
            {
                Id = IdChaveDeAcesso,
                Nome = "Chave de Acesso",
                Tipo = TipoDeItem.ForaDeBatalha,
                UsavelEmBatalha = false,
                AcaoForaDeBatalha = DroidsCode.DroidCore.AcaoForaDeBatalha.ChaveDeAcesso,
                ChaveDeFlag = "chaveDeAcessoUsada"
            },
        };

        public static DefinicaoDeItem Obter(string id)
        {
            return Todos.TryGetValue(id, out var def) ? def : null;
        }
    }

    // --- DTO de save (JsonUtility nao serializa Dictionary) ---
    [System.Serializable]
    public class ItemSalvo
    {
        public string id;
        public int quantidade;
    }
}
