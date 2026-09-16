using System.Collections.Generic;

/// <summary>
/// Elenco de NPCs do jogo. Task 6: "uma grade melhor de NPCs para o jogo,
/// como normal em RPGs -- estalagem, outras lojas".
///
/// O que este catalogo resolve: hoje um NPC e um GameObject com um
/// NpcInterativo e nada mais. Nao existe lugar onde esteja escrito QUEM
/// existe em Ferrovale, qual a funcao de cada um, e qual dialogo e de quem.
/// Sem isso, adicionar personagem vira arqueologia de cena.
///
/// Este arquivo e a fonte de verdade do elenco. Cada NPC na cena aponta pro
/// seu Id aqui (campo idDoNpc em NpcInterativo/GatilhoDeDialogo), e a partir
/// do Id o jogo sabe: nome exibido, funcao (loja/estalagem/guilda/...),
/// quais quests ele oferece (CatalogoDeQuests.DoNpc) e quais falas tem
/// (CatalogoDeDialogos).
///
/// Papeis disponiveis e o que cada um habilita no NpcInterativo:
///   Generico   -> so dialogo
///   Loja       -> abre LojaUIManager
///   Estalagem  -> abre EstalagemUIManager (descansa por Gold)
///   Guilda     -> quadro de quests da Guilda
///   Ferreiro   -> loja com estoque so de Equipavel
///   Informante -> dialogo que muda conforme o avanco da historia
/// </summary>
public static class CatalogoDeNpcs
{
    // --- Elenco do Enredo ---
    public const string IdApollo = "apollo";
    public const string IdDara = "dara";
    public const string IdTacio = "tacio";
    public const string IdHomemDaFila = "homem_fila";
    public const string IdIvo = "ivo";
    public const string IdRasha = "rasha";
    public const string IdTeodoro = "teodoro";

    // --- Elenco novo (Ato 1, preenchendo Ferrovale) ---
    public const string IdMestraDaGuilda = "mestra_guilda";
    public const string IdEstalajadeira = "estalajadeira";
    public const string IdFerreiro = "ferreiro";
    public const string IdCriancaDaRua = "crianca_rua";
    public const string IdVelhoDoViaduto = "velho_viaduto";
    public const string IdVendedoraDePecas = "vendedora_pecas";

    public static readonly Dictionary<string, DefinicaoDeNpc> Todos =
        new Dictionary<string, DefinicaoDeNpc>
        {
            // =============================================================
            // ELENCO PRINCIPAL (vindo do TCC_-_ENREDO.md)
            // =============================================================
            [IdApollo] = new DefinicaoDeNpc
            {
                Id = IdApollo, Nome = "Apollo", Papel = PapelDeNpc.Generico,
                Local = "acompanha o Lip",
                Descricao = "Droid artesanal montado com sucata, sete anos de convivência. Voz de alto-falante embutido num peito remendado com placas de cores diferentes. Dois pontos de luz azul fraca no lugar dos olhos."
            },
            [IdDara] = new DefinicaoDeNpc
            {
                Id = IdDara, Nome = "Dara", Papel = PapelDeNpc.Generico,
                Local = "fila do poço comunitário, Ferrovale",
                Descricao = "Perdeu o irmão numa captura da Apex dois invernos atrás. Desde então fala baixo, como se tivesse medo de que o vento carregasse a voz até os drones."
            },
            [IdTacio] = new DefinicaoDeNpc
            {
                Id = IdTacio, Nome = "Senhor Tácio", Papel = PapelDeNpc.Loja,
                Local = "mercado sob o viaduto, Ferrovale",
                Descricao = "Vende água filtrada com um contador Geiger quebrado pendurado no pescoço, só pra parecer que sabe o que está fazendo."
            },
            [IdHomemDaFila] = new DefinicaoDeNpc
            {
                Id = IdHomemDaFila, Nome = "Homem da fila", Papel = PapelDeNpc.Generico,
                Local = "fila do poço comunitário, Ferrovale",
                Descricao = "Não dá o nome. Foi o primeiro a dizer em voz alta que foi o Ivo, e há mais medo que raiva na voz dele."
            },
            [IdIvo] = new DefinicaoDeNpc
            {
                Id = IdIvo, Nome = "Ivo", Papel = PapelDeNpc.Generico,
                Local = "estação de captação, Cratera",
                Descricao = "Um dos técnicos mais velhos, dos poucos que lembram como era o mundo antes de 2060. Perdeu a esposa numa das primeiras varreduras e desde então cuida da estação como se fosse a última coisa que lhe restou proteger."
            },
            [IdRasha] = new DefinicaoDeNpc
            {
                Id = IdRasha, Nome = "Rasha", Papel = PapelDeNpc.Generico,
                Local = "borda da Cratera",
                Descricao = "Flor de Ferro. Casaco reforçado com placas pintadas de vermelho desbotado, uma flor de pétalas afiadas costurada no ombro. Sorri como quem já viu gente demais morrer pra achar isso fácil."
            },
            [IdTeodoro] = new DefinicaoDeNpc
            {
                Id = IdTeodoro, Nome = "Teodoro", Papel = PapelDeNpc.Generico,
                Local = "borda da Cratera",
                Descricao = "Lótus Branca. Tons neutros, um broche discreto de lótus no peito. Fala baixo, cansado de ter a mesma conversa muitas vezes."
            },

            // =============================================================
            // ELENCO NOVO — Ferrovale precisa parecer uma cidade, não um
            // corredor com uma loja no fim.
            // =============================================================
            [IdMestraDaGuilda] = new DefinicaoDeNpc
            {
                Id = IdMestraDaGuilda, Nome = "Mestra Bruna", Papel = PapelDeNpc.Guilda,
                Local = "galpão da Guilda dos Sucateiros, Ferrovale",
                Descricao = "Mestra da Guilda dos Sucateiros. Dobra metal com a mão sem esforço aparente e sabe de cor o nome dos 400 membros que a Guilda tinha antes da Artemis.",
                TextoDeBoasVindas = "Guilda não é clube. É a lista de quem essa cidade procura quando precisa de alguma coisa achada."
            },
            [IdEstalajadeira] = new DefinicaoDeNpc
            {
                Id = IdEstalajadeira, Nome = "Nilce", Papel = PapelDeNpc.Estalagem,
                Local = "Estalagem do Cano Torto, Ferrovale",
                Descricao = "Aluga colchão e silêncio. Cobra pouco e não pergunta nada — o que, em Ferrovale, é praticamente um serviço de luxo.",
                TextoDeBoasVindas = "Colchão limpo, porta que tranca, e eu não vi você entrar. Serve?"
            },
            [IdFerreiro] = new DefinicaoDeNpc
            {
                Id = IdFerreiro, Nome = "Quirino", Papel = PapelDeNpc.Ferreiro,
                Local = "oficina nos fundos do mercado, Ferrovale",
                Descricao = "Monta peça de droid com o que aparece. Diz que não conserta arma — e conserta, mas só pra quem ele conhece.",
                TextoDeBoasVindas = "Peça eu tenho. Garantia eu não dou. Traz de volta quebrado que eu olho de novo."
            },
            [IdVendedoraDePecas] = new DefinicaoDeNpc
            {
                Id = IdVendedoraDePecas, Nome = "Sula", Papel = PapelDeNpc.Loja,
                Local = "barraca da esquina leste, Ferrovale",
                Descricao = "Concorrência direta do Tácio, e os dois fingem que não. Vende o mesmo, um pouco mais caro, com menos conversa."
            },
            [IdCriancaDaRua] = new DefinicaoDeNpc
            {
                Id = IdCriancaDaRua, Nome = "Pipo", Papel = PapelDeNpc.Generico,
                Local = "entre as carcaças, Ferrovale",
                Descricao = "Brinca de pilotar máquina de guerra entre robôs desativados. Sabe onde tudo fica porque ninguém repara numa criança."
            },
            [IdVelhoDoViaduto] = new DefinicaoDeNpc
            {
                Id = IdVelhoDoViaduto, Nome = "Seu Onofre", Papel = PapelDeNpc.Informante,
                Local = "debaixo do viaduto, Ferrovale",
                Descricao = "Lembra do mundo antes. Conta a mesma história de jeitos diferentes, e cada versão tem um detalhe novo que parece verdade.",
                TextoDeBoasVindas = "Senta aí. Tenho história pra vender e não cobro."
            }
        };

    public static DefinicaoDeNpc Obter(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return Todos.TryGetValue(id, out DefinicaoDeNpc npc) ? npc : null;
    }

    /// <summary>Nome exibido, com fallback pro próprio Id se não existir.</summary>
    public static string NomeDe(string id)
    {
        DefinicaoDeNpc npc = Obter(id);
        return npc != null ? npc.Nome : id;
    }
}

public enum PapelDeNpc
{
    Generico,
    Loja,
    Estalagem,
    Guilda,
    Ferreiro,
    Informante
}

public class DefinicaoDeNpc
{
    public string Id;
    public string Nome;
    public PapelDeNpc Papel = PapelDeNpc.Generico;

    /// <summary>Onde ele fica. Documentação pra montagem de cena.</summary>
    public string Local = "";

    /// <summary>Quem é. Usado no diário e como referência de arte/escrita.</summary>
    public string Descricao = "";

    /// <summary>Fala curta de abertura antes do serviço (loja/estalagem).</summary>
    public string TextoDeBoasVindas = "";
}
