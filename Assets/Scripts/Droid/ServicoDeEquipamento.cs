using System;

namespace DroidsCode.DroidCore
{
    /// <summary>Operações de equipamento em memória. Não grava arquivo nem controla interface.</summary>
    public static class ServicoDeEquipamento
    {
        public static DroidPart CriarPeca(TipoDePeca slot, string id, string nome, Raridade raridade,
            TipoAtributo? atributo, int bonus, int principal = 0) => slot switch
        {
            TipoDePeca.Braco => new Braco(nome, principal, raridade, atributo, bonus, id),
            TipoDePeca.Perna => new Perna(nome, principal, raridade, atributo, bonus, id),
            TipoDePeca.Tronco => new Tronco(nome, principal, raridade, atributo, bonus, id),
            TipoDePeca.Cabeca => new Cabeca(nome, principal, raridade, atributo, bonus, id),
            _ => throw new ArgumentOutOfRangeException(nameof(slot))
        };

        static bool PodeDevolver(GerenciadorDeEstado estado, DroidPart peca, out string mensagem)
        {
            mensagem = "";
            if (peca == null) return true;
            if (string.IsNullOrEmpty(peca.IdItem) || CatalogoDeItens.Obter(peca.IdItem) == null)
            { mensagem = "A peça instalada não tem ID de catálogo válido. A operação foi cancelada para preservar a peça."; return false; }
            if (estado.ObterQuantidadeDeItem(peca.IdItem) == int.MaxValue)
            { mensagem = "Não há espaço numérico para devolver a peça ao inventário."; return false; }
            return true;
        }

        public static bool TentarEquipar(GerenciadorDeEstado estado, string id, out string mensagem)
        {
            mensagem = "Estado do jogador indisponível.";
            if (estado == null || estado.DroidDoJogador == null) return false;
            var item = string.IsNullOrWhiteSpace(id) ? null : CatalogoDeItens.Obter(id);
            if (item == null || item.Tipo != TipoDeItem.Equipavel || !item.SlotDePeca.HasValue)
            { mensagem = "ID inexistente ou item não equipável: " + id; return false; }
            var droid = estado.DroidDoJogador; var slot = item.SlotDePeca.Value;
            var anterior = droid.ObterPeca(slot);
            if (anterior != null && anterior.IdItem == id)
            { mensagem = item.Nome + " já está equipado. Nenhuma unidade consumida."; return true; }
            if (estado.ObterQuantidadeDeItem(id) <= 0)
            { mensagem = "Você não possui " + item.Nome + "."; return false; }
            if (!PodeDevolver(estado, anterior, out mensagem)) return false;
            var nova = CriarPeca(slot, id, item.Nome, item.Raridade, item.AtributoBonificado, item.ValorDoBonus);
            int hpAntes = droid.Hp;
            if (!estado.TentarRemoverItem(id, 1)) { mensagem = "Estoque insuficiente."; return false; }
            droid.EquiparPeca(slot, nova);
            if (anterior != null) estado.AdicionarItem(anterior.IdItem, 1);
            droid.Hp = Math.Min(hpAntes, droid.HpMax);
            mensagem = item.Nome + " equipado em " + slot + "."; return true;
        }

        public static bool TentarDesequipar(GerenciadorDeEstado estado, TipoDePeca slot, out string mensagem)
        {
            mensagem = "Estado ou slot indisponível.";
            if (estado == null || estado.DroidDoJogador == null || !Enum.IsDefined(typeof(TipoDePeca), slot)) return false;
            var droid = estado.DroidDoJogador; var anterior = droid.ObterPeca(slot);
            if (anterior == null) { mensagem = slot + " já está vazio."; return true; }
            if (!PodeDevolver(estado, anterior, out mensagem)) return false;
            int hpAntes = droid.Hp;
            droid.EquiparPeca(slot, null); estado.AdicionarItem(anterior.IdItem, 1);
            droid.Hp = Math.Min(hpAntes, droid.HpMax);
            mensagem = anterior.Nome + " devolvido ao inventário."; return true;
        }
    }
}
