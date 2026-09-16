using System.Collections.Generic;

namespace DroidsCode.DroidCore
{
    // Estagio 3 (fase 2 do sistema de item) -- expande o catalogo de Cura
    // (estagio 2, 13/09/2026) para as 4 categorias que faltavam.
    //
    // ATO 1 (15/09/2026) -- quatro mudancas, todas ADITIVAS (nenhum item
    // existente mudou de comportamento):
    //   1. DefinicaoDeItem ganhou Descricao (texto longo, mostrado na Bag e
    //      na Loja) e Raridade -- a UI antes so tinha nome + stat resumido,
    //      item aberto no CHECKLIST > Amadurecimento ("Icones e descricao
    //      longa nos itens/Loja").
    //   2. Ganhou VendavelNaLoja e PrecoDeVenda. ItensDaLoja deixou de ser
    //      "Todos.Values" cru -- ver o comentario grande em ItensDaLoja.
    //   3. Precos deixaram de ser todos 1 Gold. Continuam placeholder, mas
    //      agora tem ORDEM DE GRANDEZA coerente entre si, que e o minimo pra
    //      Gold significar alguma coisa.
    //   4. Itens novos: recompensas de faccao, itens de quest, e equipaveis
    //      que dao o que fazer com Gold (inclusive um de DEX -- ver a nota
    //      de balanceamento no Oleo de Precisao).
    public enum TipoDeItem
    {
        Cura,
        Buff,
        Debuff,
        Equipavel,
        ForaDeBatalha,
        // NOVO (Ato 1): item que so existe pra ser entregue numa quest ou
        // pra ler uma curiosidade. Nunca usavel, nunca vendavel.
        Chave
    }

    // Estagio 3: identifica QUAL logica de uso-fora-de-batalha rodar (ver
    // BagUIManager.UsarAcaoForaDeBatalha).
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

        public int Preco { get; set; } = PrecoPadrao;

        // NOVO (Ato 1): texto longo mostrado na Bag/Loja. Escrito no tom do
        // mundo (sucata, Ferrovale, gambiarra) de proposito -- descricao de
        // item e um dos lugares mais baratos de contar mundo num RPG.
        public string Descricao { get; set; } = "";

        // NOVO (Ato 1): so afeta apresentacao (ordenacao e cor na lista).
        public Raridade Raridade { get; set; } = Raridade.Comum;

        // NOVO (Ato 1): controla se o item aparece na aba Comprar da Loja.
        // Default TRUE pra que todo item que ja existia continue vendavel
        // exatamente como antes desta mudanca.
        public bool VendavelNaLoja { get; set; } = true;

        // NOVO (Ato 1): quanto a Loja PAGA ao jogador. 0 = cai no calculo
        // padrao (metade do Preco, minimo 1) -- ver PrecoDeVendaReal.
        public int PrecoDeVenda { get; set; } = 0;

        /// <summary>
        /// Quanto a Loja paga por este item. Spread comprador/vendedor de
        /// 50% -- sem isso o jogador compra e revende pelo mesmo preco, e
        /// "vender" vira um botao sem consequencia nenhuma.
        /// </summary>
        public int PrecoDeVendaReal =>
            PrecoDeVenda > 0 ? PrecoDeVenda : (Preco / 2 < 1 ? 1 : Preco / 2);

        // --- Cura (Estagio 2) ---
        public int CuraHp { get; set; }

        // --- Buff / Debuff (Estagio 3) ---
        public EfeitoDeAtributo EfeitoDeAtributoAplicado { get; set; }
        public EfeitoDeDanoPorTurno EfeitoDeDanoAplicado { get; set; }

        // --- Equipavel (Estagio 3) ---
        public TipoDePeca? SlotDePeca { get; set; }
        public TipoAtributo? AtributoBonificado { get; set; }
        public int ValorDoBonus { get; set; }

        // --- ForaDeBatalha (Estagio 3) ---
        public AcaoForaDeBatalha? AcaoForaDeBatalha { get; set; }
        public string ChaveDeFlag { get; set; }

        public const int PrecoPadrao = 10;

        /// <summary>
        /// Resumo de uma linha usado no BOTAO da lista (a Descricao longa vai
        /// no painel de detalhe). Centralizado aqui em vez de duplicado em
        /// BagUIManager e BattleManager, que tinham dois metodos
        /// DescreverItem quase iguais -- e ja divergentes entre si.
        /// </summary>
        public string ResumoCurto()
        {
            switch (Tipo)
            {
                case TipoDeItem.Cura:
                    return $"+{CuraHp} HP";
                case TipoDeItem.Equipavel:
                    return AtributoBonificado != null
                        ? $"+{ValorDoBonus} {AtributoBonificado} ({SlotDePeca})"
                        : "Peça";
                case TipoDeItem.Buff:
                case TipoDeItem.Debuff:
                    if (EfeitoDeAtributoAplicado != null)
                        return $"{EfeitoDeAtributoAplicado.NomeExibicao} ({EfeitoDeAtributoAplicado.DuracaoEmTurnos}t)";
                    if (EfeitoDeDanoAplicado != null)
                        return $"{EfeitoDeDanoAplicado.NomeExibicao} {EfeitoDeDanoAplicado.DanoPorTurno}/turno";
                    return "";
                case TipoDeItem.Chave:
                    return "Item de missão";
                default:
                    return "";
            }
        }
    }

    // Catalogo estatico -- mesmo espirito de TabelaDeCombate/TabelaDeCustos.
    // O INVENTARIO (quantidade que o jogador possui) fica em
    // GerenciadorDeEstado, nao aqui.
    public static class CatalogoDeItens
    {
        // --- Cura ---
        public const string IdPocaoPequena = "pocao_pequena";
        public const string IdPocaoMedia = "pocao_media";
        public const string IdPocaoGrande = "pocao_grande";
        public const string IdReparoCompleto = "reparo_completo";

        // --- Buff ---
        public const string IdEscudoDeEmergencia = "escudo_de_emergencia";
        public const string IdOleoDePrecisao = "oleo_de_precisao";      // NOVO (Ato 1)
        public const string IdSobrecargaTemporaria = "sobrecarga_temp"; // NOVO (Ato 1)

        // --- Debuff ---
        public const string IdGranadaDeAcido = "granada_de_acido";
        public const string IdDisruptorEmp = "disruptor_emp";
        public const string IdCorrosivo = "corrosivo";

        // --- Equipavel ---
        public const string IdNucleoDeEnergia = "nucleo_de_energia";
        public const string IdServoMotor = "servo_motor";
        public const string IdPlacaDeBlindagem = "placa_de_blindagem"; // NOVO
        public const string IdSensorOptico = "sensor_optico";          // NOVO
        public const string IdGiroscopio = "giroscopio";               // NOVO

        // --- ForaDeBatalha ---
        public const string IdSinalizadorDeRetorno = "sinalizador_de_retorno";
        public const string IdKitDeAcampamento = "kit_de_acampamento";
        public const string IdChaveDeAcesso = "chave_de_acesso";

        // --- ATO 1: recompensas de faccao (Sessao 4 do Enredo) ---
        public const string IdNucleoDeSobrecarga = "nucleo_sobrecarga";
        public const string IdModuloDeRessonancia = "modulo_ressonancia";

        // --- ATO 1: itens de quest ---
        public const string IdFioDeCobre = "fio_de_cobre";
        public const string IdFiltroDeAgua = "filtro_de_agua";
        public const string IdSucataLimpa = "sucata_limpa";
        public const string IdEmblemaDaGuilda = "emblema_da_guilda";
        public const string IdBilheteDoIvo = "bilhete_do_ivo";
        public const string IdPecaDaPernaDoApollo = "peca_perna_apollo";

        public static readonly Dictionary<string, DefinicaoDeItem> Todos = new Dictionary<string, DefinicaoDeItem>
        {
            // =============================================================
            // CURA
            // =============================================================
            [IdPocaoPequena] = new DefinicaoDeItem
            {
                Id = IdPocaoPequena, Nome = "Poção Pequena", Tipo = TipoDeItem.Cura, CuraHp = 20, Preco = 8,
                Descricao = "Um tubo de gel reparador raspado de kits médicos pré-2060. Sabe a metal, mas fecha junta."
            },
            [IdPocaoMedia] = new DefinicaoDeItem
            {
                Id = IdPocaoMedia, Nome = "Poção Média", Tipo = TipoDeItem.Cura, CuraHp = 50, Preco = 22,
                Raridade = Raridade.Raro,
                Descricao = "Dose dupla, selada a quente. O Tácio jura que é do estoque antigo do hospital. Ninguém checa."
            },
            [IdPocaoGrande] = new DefinicaoDeItem
            {
                Id = IdPocaoGrande, Nome = "Poção Grande", Tipo = TipoDeItem.Cura, CuraHp = 80, Preco = 55,
                Raridade = Raridade.Raro,
                Descricao = "Cartucho industrial. Pesado demais pra carregar muitos, bom demais pra deixar pra trás."
            },
            [IdReparoCompleto] = new DefinicaoDeItem
            {
                Id = IdReparoCompleto, Nome = "Reparo Completo", Tipo = TipoDeItem.Cura, CuraHp = 9999, Preco = 150,
                Raridade = Raridade.Lendario,
                Descricao = "Nanorreparo de verdade, dos que a Apex usa nas próprias unidades. Ninguém em Ferrovale sabe como um foi parar aqui."
            },

            // =============================================================
            // BUFF (só em batalha)
            // =============================================================
            [IdEscudoDeEmergencia] = new DefinicaoDeItem
            {
                Id = IdEscudoDeEmergencia, Nome = "Escudo de Emergência", Tipo = TipoDeItem.Buff,
                UsavelForaDeBatalha = false, Preco = 30,
                Descricao = "Campo de contenção improvisado. Aguenta três turnos antes de derreter o próprio emissor. +10 VIT.",
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                { NomeExibicao = "Escudo", Atributo = TipoAtributo.Vit, Valor = 10, DuracaoEmTurnos = 3 }
            },
            // NOVO (Ato 1). Existe por um motivo de BALANCEAMENTO, não de
            // variedade: o Droid inicial tem DEX 0, o que dá 40% de chance de
            // acerto contra os primeiros inimigos (ver a nota completa em
            // CatalogoDeInimigos). Este item é a rede de segurança pra quem
            // ainda não entendeu que precisa subir DEX no terminal.
            [IdOleoDePrecisao] = new DefinicaoDeItem
            {
                Id = IdOleoDePrecisao, Nome = "Óleo de Precisão", Tipo = TipoDeItem.Buff,
                UsavelForaDeBatalha = false, Preco = 18,
                Descricao = "Lubrifica os servos de mira do braço. +15 DEX por 4 turnos — na prática, você para de errar tanto.",
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                { NomeExibicao = "Precisão", Atributo = TipoAtributo.Dex, Valor = 15, DuracaoEmTurnos = 4 }
            },
            [IdSobrecargaTemporaria] = new DefinicaoDeItem
            {
                Id = IdSobrecargaTemporaria, Nome = "Injetor de Sobrecarga", Tipo = TipoDeItem.Buff,
                UsavelForaDeBatalha = false, Preco = 35, Raridade = Raridade.Raro,
                Descricao = "Despeja energia direto no atuador. +8 FOR por 3 turnos. O Apollo reclama depois.",
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                { NomeExibicao = "Sobrecarga", Atributo = TipoAtributo.For, Valor = 8, DuracaoEmTurnos = 3 }
            },

            // =============================================================
            // DEBUFF (só em batalha)
            // =============================================================
            [IdGranadaDeAcido] = new DefinicaoDeItem
            {
                Id = IdGranadaDeAcido, Nome = "Granada de Ácido", Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false, Preco = 25,
                Descricao = "Come a blindagem do alvo. -5 VIT por 3 turnos, o que significa mais dano em todo golpe seu.",
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                { NomeExibicao = "Corrosão", Atributo = TipoAtributo.Vit, Valor = -5, DuracaoEmTurnos = 3 }
            },
            [IdDisruptorEmp] = new DefinicaoDeItem
            {
                Id = IdDisruptorEmp, Nome = "Disruptor EMP", Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false, Preco = 40, Raridade = Raridade.Raro,
                Descricao = "Pulso curto. O alvo perde um turno inteiro se reorganizando.",
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                { NomeExibicao = "Stun", Atributo = TipoAtributo.For, Valor = 0, DuracaoEmTurnos = 1 }
            },
            [IdCorrosivo] = new DefinicaoDeItem
            {
                Id = IdCorrosivo, Nome = "Corrosivo", Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false, Preco = 28,
                Descricao = "Frasco de resíduo da Cratera. 5 de dano por turno, por 3 turnos.",
                EfeitoDeDanoAplicado = new EfeitoDeDanoPorTurno
                { NomeExibicao = "Corrosivo", DanoPorTurno = 5, DuracaoEmTurnos = 3 }
            },

            // =============================================================
            // EQUIPÁVEL
            // =============================================================
            [IdNucleoDeEnergia] = new DefinicaoDeItem
            {
                Id = IdNucleoDeEnergia, Nome = "Núcleo de Energia", Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false, Preco = 60,
                SlotDePeca = TipoDePeca.Tronco, AtributoBonificado = TipoAtributo.For, ValorDoBonus = 5,
                Descricao = "Célula de carga reaproveitada. Instala no tronco e alimenta os atuadores com mais folga. +5 FOR."
            },
            [IdServoMotor] = new DefinicaoDeItem
            {
                Id = IdServoMotor, Nome = "Servo-Motor", Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false, Preco = 60,
                SlotDePeca = TipoDePeca.Perna, AtributoBonificado = TipoAtributo.Agi, ValorDoBonus = 5,
                Descricao = "Resolve o mancar do Apollo — pelo menos por um tempo. +5 AGI, ou seja, esquiva mais."
            },
            // NOVO (Ato 1): faltavam peças de Cabeça e um equipável de DEX,
            // justamente o atributo mais crítico pro jogador iniciante.
            [IdSensorOptico] = new DefinicaoDeItem
            {
                Id = IdSensorOptico, Nome = "Sensor Óptico", Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false, Preco = 70, Raridade = Raridade.Raro,
                SlotDePeca = TipoDePeca.Cabeca, AtributoBonificado = TipoAtributo.Dex, ValorDoBonus = 6,
                Descricao = "Lente de drone de reconhecimento, com a marca da Apex lixada por cima. +6 DEX — você erra muito menos."
            },
            [IdPlacaDeBlindagem] = new DefinicaoDeItem
            {
                Id = IdPlacaDeBlindagem, Nome = "Placa de Blindagem", Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false, Preco = 65,
                SlotDePeca = TipoDePeca.Tronco, AtributoBonificado = TipoAtributo.Vit, ValorDoBonus = 6,
                Descricao = "Chapa de container cortada na medida. Feia, pesada, funciona. +6 VIT (defesa e HP máximo)."
            },
            [IdGiroscopio] = new DefinicaoDeItem
            {
                Id = IdGiroscopio, Nome = "Giroscópio", Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false, Preco = 75, Raridade = Raridade.Raro,
                SlotDePeca = TipoDePeca.Braco, AtributoBonificado = TipoAtributo.Luk, ValorDoBonus = 6,
                Descricao = "Estabiliza o braço no instante do impacto. +6 LUK — mais chance de crítico."
            },

            // =============================================================
            // FORA DE BATALHA
            // =============================================================
            [IdSinalizadorDeRetorno] = new DefinicaoDeItem
            {
                Id = IdSinalizadorDeRetorno, Nome = "Sinalizador de Retorno", Tipo = TipoDeItem.ForaDeBatalha,
                UsavelEmBatalha = false, Preco = 45,
                Descricao = "Queima e chama um carrinho de sucata. Na prática: volta pro último ponto salvo — inclusive desfazendo o que você fez desde então.",
                AcaoForaDeBatalha = DroidsCode.DroidCore.AcaoForaDeBatalha.SinalizadorDeRetorno
            },
            [IdKitDeAcampamento] = new DefinicaoDeItem
            {
                Id = IdKitDeAcampamento, Nome = "Kit de Acampamento", Tipo = TipoDeItem.ForaDeBatalha,
                UsavelEmBatalha = false, Preco = 50,
                Descricao = "Lona, soldador e um pouco de paciência. Recupera todo o HP e salva o jogo na hora.",
                AcaoForaDeBatalha = DroidsCode.DroidCore.AcaoForaDeBatalha.KitDeAcampamento
            },
            [IdChaveDeAcesso] = new DefinicaoDeItem
            {
                Id = IdChaveDeAcesso, Nome = "Chave de Acesso", Tipo = TipoDeItem.ForaDeBatalha,
                UsavelEmBatalha = false, Preco = 90, Raridade = Raridade.Raro, VendavelNaLoja = false,
                Descricao = "Cartão de manutenção antigo. Abre portas que não deveriam mais abrir.",
                AcaoForaDeBatalha = DroidsCode.DroidCore.AcaoForaDeBatalha.ChaveDeAcesso,
                ChaveDeFlag = "chaveDeAcessoUsada"
            },

            // =============================================================
            // ATO 1 — RECOMPENSAS DE FACÇÃO (Sessão 4)
            // Nunca compráveis, nunca vendáveis: a escolha é a única forma
            // de obtê-las, e vender por engano seria irreversível.
            // =============================================================
            [IdNucleoDeSobrecarga] = new DefinicaoDeItem
            {
                Id = IdNucleoDeSobrecarga, Nome = "Núcleo de Sobrecarga", Tipo = TipoDeItem.Equipavel,
                UsavelEmBatalha = false, VendavelNaLoja = false, Preco = 0,
                Raridade = Raridade.Lendario,
                SlotDePeca = TipoDePeca.Braco, AtributoBonificado = TipoAtributo.For, ValorDoBonus = 10,
                Descricao = "Da Rasha. Bordas ásperas, núcleo pulsando vermelho por baixo do metal riscado. +10 FOR. \"Ferro não é sobre economizar força. É sobre gastar tudo quando importa.\""
            },
            [IdModuloDeRessonancia] = new DefinicaoDeItem
            {
                Id = IdModuloDeRessonancia, Nome = "Módulo de Ressonância", Tipo = TipoDeItem.Debuff,
                UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Raridade = Raridade.Lendario,
                Descricao = "Do Teodoro. Disco liso, luz branca pulsando devagar. Reescreve o protocolo de combate do alvo em vez de danificar o hardware — ele para por 3 turnos. Você só tem um.",
                EfeitoDeAtributoAplicado = new EfeitoDeAtributo
                { NomeExibicao = "Stun", Atributo = TipoAtributo.For, Valor = 0, DuracaoEmTurnos = 3 }
            },

            // =============================================================
            // ATO 1 — ITENS DE QUEST (nunca usáveis, nunca vendáveis)
            // =============================================================
            [IdFioDeCobre] = new DefinicaoDeItem
            {
                Id = IdFioDeCobre, Nome = "Fio de Cobre", Tipo = TipoDeItem.Chave,
                UsavelEmBatalha = false, UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Descricao = "Em Ferrovale, cobre é moeda. Antes de virar moeda, era o que levava energia pros lugares."
            },
            [IdFiltroDeAgua] = new DefinicaoDeItem
            {
                Id = IdFiltroDeAgua, Nome = "Filtro de Água", Tipo = TipoDeItem.Chave,
                UsavelEmBatalha = false, UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Descricao = "Cartucho de carvão prensado. Não deixa a água boa, só deixa ela possível."
            },
            [IdSucataLimpa] = new DefinicaoDeItem
            {
                Id = IdSucataLimpa, Nome = "Sucata Limpa", Tipo = TipoDeItem.Chave,
                UsavelEmBatalha = false, UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Descricao = "Metal sem corrosão, raro depois da Cratera. Vale mais que comida em certos dias."
            },
            [IdEmblemaDaGuilda] = new DefinicaoDeItem
            {
                Id = IdEmblemaDaGuilda, Nome = "Emblema da Guilda", Tipo = TipoDeItem.Chave,
                UsavelEmBatalha = false, UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Raridade = Raridade.Raro,
                Descricao = "Uma engrenagem torta com três dentes limados. Quem carrega isso em Ferrovale é reconhecido — pra bem e pra mal."
            },
            [IdBilheteDoIvo] = new DefinicaoDeItem
            {
                Id = IdBilheteDoIvo, Nome = "Bilhete do Ivo", Tipo = TipoDeItem.Chave,
                UsavelEmBatalha = false, UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Descricao = "Papel dobrado quatro vezes, letra tremida: \"Se eu não voltar, a bomba de reserva fica embaixo do piso da sala leste. Não deixem a cidade sem água por minha causa.\""
            },
            [IdPecaDaPernaDoApollo] = new DefinicaoDeItem
            {
                Id = IdPecaDaPernaDoApollo, Nome = "Servo da Perna do Apollo", Tipo = TipoDeItem.Chave,
                UsavelEmBatalha = false, UsavelForaDeBatalha = false, VendavelNaLoja = false, Preco = 0,
                Raridade = Raridade.Raro,
                Descricao = "O servo emperrado que faz o Apollo mancar desde sempre. Sete anos esperando uma peça de reposição."
            },
        };

        public static DefinicaoDeItem Obter(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return Todos.TryGetValue(id, out var def) ? def : null;
        }

        // CORRECAO IMPORTANTE (15/09/2026). A versao anterior era
        // "Todos.Values" cru, sem filtro, com a nota "se no futuro alguns
        // itens nao deverem ser vendaveis, filtrar aqui". Esse futuro chegou
        // junto com o Ato 1: sem este filtro, o jogador compraria as DUAS
        // recompensas de faccao na loja do Tacio antes de conhecer Rasha e
        // Teodoro -- a decisao mais importante do Ato viraria compra de
        // padaria. Itens de quest tambem apareceriam na vitrine.
        public static IEnumerable<DefinicaoDeItem> ItensDaLoja
        {
            get
            {
                foreach (DefinicaoDeItem item in Todos.Values)
                {
                    if (item.VendavelNaLoja && item.Preco > 0) yield return item;
                }
            }
        }

        /// <summary>
        /// Se o jogador pode vender este item. Itens de quest e recompensas
        /// de facção ficam de fora -- vender a própria escolha de facção por
        /// engano seria irreversível e o jogo não teria como avisar.
        /// </summary>
        public static bool PodeSerVendido(DefinicaoDeItem item)
        {
            return item != null && item.VendavelNaLoja && item.Tipo != TipoDeItem.Chave;
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
