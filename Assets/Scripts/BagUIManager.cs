using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DroidsCode.DroidCore;

/// <summary>
/// ESTAGIO 3 (fase 2 do sistema de item, sem alinhamento extra):
///   - UsarItemForaDeBatalha virou um switch por TipoDeItem em vez de
///     hard-coded pra cura. Cura mantem o comportamento antigo; Equipavel
///     e ForaDeBatalha sao novos (ver EquiparItem/UsarAcaoForaDeBatalha).
///   - Buff/Debuff nao aparecem aqui (UsavelForaDeBatalha=false no
///     catalogo) -- so existem em batalha (ver BattleManager).
/// </summary>
public class BagUIManager : MonoBehaviour
{
    [Header("Painel raiz")]
    public GameObject painel;

    [Header("Lista de itens")]
    public Transform painelListaDeItens;
    public Button prefabBotaoItem;
    public Button botaoVoltar;

    [Header("Feedback")]
    public TextMeshProUGUI textoMensagem;

    public System.Action AoFechar;

    private readonly ISistemaDeSalvamento _salvamento = new SalvamentoJson();

    void Start()
    {
        if (botaoVoltar != null) botaoVoltar.onClick.AddListener(Fechar);
        if (painel != null) painel.SetActive(false);
    }

    // CORRECAO (13/09/2026 — Bag nao aparecia ao clicar): a ordem antiga era
    // AtualizarLista() (que instancia botoes DENTRO de um painel ainda
    // INATIVO) e só depois painel.SetActive(true). Instanciar filhos num
    // GameObject inativo, com VerticalLayoutGroup + ContentSizeFitter no
    // pai, faz o Unity nao recalcular a altura/largura do painel (ele so
    // recalcula layout em objetos ATIVOS) -- o painel ativa com Content
    // Size Fitter travado em 0x0, entao fica "ligado" mas invisivel.
    // Fix: ativar o painel PRIMEIRO, so DEPOIS popular a lista, e forcar um
    // rebuild de layout no frame seguinte.
    public void Mostrar()
    {
        if (painel != null) painel.SetActive(true);
        AtualizarLista();

        if (painelListaDeItens != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeItens.GetComponent<RectTransform>());
        }
        if (painel != null)
        {
            var painelRect = painel.GetComponent<RectTransform>();
            if (painelRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(painelRect);
        }
    }

    public void Fechar()
    {
        if (painel != null) painel.SetActive(false);
        AoFechar?.Invoke();
    }

    void AtualizarLista()
    {
        if (painelListaDeItens == null || prefabBotaoItem == null)
        {
            Debug.LogWarning("BagUIManager: painelListaDeItens/prefabBotaoItem não configurados no Inspector.");
            return;
        }

        foreach (Transform filho in painelListaDeItens)
            Destroy(filho.gameObject);

        var gerenciador = GerenciadorDeEstado.Instancia;
        var itensComEstoque = new List<DefinicaoDeItem>();

        foreach (var kv in gerenciador.Inventario)
        {
            if (kv.Value <= 0) continue;
            var def = CatalogoDeItens.Obter(kv.Key);
            if (def == null || !def.UsavelForaDeBatalha) continue;
            itensComEstoque.Add(def);
        }

        if (itensComEstoque.Count == 0)
        {
            MostrarMensagem("Sua bag está vazia.");
        }

        foreach (DefinicaoDeItem item in itensComEstoque)
        {
            int quantidade = gerenciador.ObterQuantidadeDeItem(item.Id);

            Button botao = Instantiate(prefabBotaoItem, painelListaDeItens);
            botao.gameObject.SetActive(true);

            var texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            // Estagio 3: descricao generica (antes era hard-coded pra cura).
            if (texto != null) texto.text = DescreverItem(item, quantidade);

            DefinicaoDeItem itemCapturado = item;
            botao.onClick.AddListener(() => UsarItemForaDeBatalha(itemCapturado));
        }
    }

    // NOVO (Estagio 3): texto do botao, generico por Tipo. Equipavel mostra
    // o bonus (ex: "+5 For"); Cura mostra a cura (comportamento antigo);
    // ForaDeBatalha nao tem stat pra mostrar, so o nome.
    string DescreverItem(DefinicaoDeItem item, int quantidade)
    {
        string detalhe = item.Tipo switch
        {
            TipoDeItem.Cura => $"(+{item.CuraHp} HP)",
            TipoDeItem.Equipavel => item.AtributoBonificado != null ? $"(+{item.ValorDoBonus} {item.AtributoBonificado})" : "",
            _ => ""
        };
        return string.IsNullOrEmpty(detalhe) ? $"{item.Nome} x{quantidade}" : $"{item.Nome} {detalhe} x{quantidade}";
    }

    // REFATORACAO (Estagio 3): antes era hard-coded pra cura. Agora
    // despacha por TipoDeItem -- Cura mantem o comportamento antigo
    // (extraido pra UsarCuraForaDeBatalha), Equipavel/ForaDeBatalha sao
    // novos.
    void UsarItemForaDeBatalha(DefinicaoDeItem item)
    {
        var gerenciador = GerenciadorDeEstado.Instancia;
        Droid droid = gerenciador.DroidDoJogador;

        switch (item.Tipo)
        {
            case TipoDeItem.Cura:
                UsarCuraForaDeBatalha(item, gerenciador, droid);
                break;

            case TipoDeItem.Equipavel:
                EquiparItem(item, gerenciador, droid);
                break;

            case TipoDeItem.ForaDeBatalha:
                UsarAcaoForaDeBatalha(item, gerenciador, droid);
                break;

            default:
                // Buff/Debuff tem UsavelForaDeBatalha=false no catalogo --
                // nao deveria chegar aqui, mas nao consome silenciosamente
                // se acontecer.
                MostrarMensagem($"{item.Nome} não pode ser usado fora de batalha.");
                return;
        }
    }

    void UsarCuraForaDeBatalha(DefinicaoDeItem item, GerenciadorDeEstado gerenciador, Droid droid)
    {
        if (droid.Hp >= droid.HpMax)
        {
            MostrarMensagem($"{droid.Nome} já está com HP cheio.");
            return;
        }

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            MostrarMensagem($"Você não tem mais {item.Nome}.");
            return;
        }

        float multiplicador = 1f + (droid.BonusPercentualDeCura / 100f);
        int curaBase = Mathf.FloorToInt(item.CuraHp * multiplicador);
        int curaAplicada = Mathf.Min(curaBase, droid.HpMax - droid.Hp);
        droid.Hp = Mathf.Min(droid.HpMax, droid.Hp + curaBase);

        MostrarMensagem($"{droid.Nome} usou {item.Nome} e recuperou {curaAplicada} de HP!");
        _salvamento.Salvar();
        AtualizarListaComRebuild();
    }

    // NOVO (Estagio 3): equipar CONSOME o item -- cria a peca a partir dos
    // dados do catalogo (nome/slot/bonus) e instala via Droid.EquiparPeca,
    // substituindo a peca anterior daquele slot se houver (sem
    // desequipar/devolver nesta fase, ver comentario em Droid.EquiparPeca).
    void EquiparItem(DefinicaoDeItem item, GerenciadorDeEstado gerenciador, Droid droid)
    {
        if (item.SlotDePeca == null)
        {
            Debug.LogWarning($"BagUIManager: item Equipavel '{item.Id}' sem SlotDePeca definido no catalogo.");
            return;
        }

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            MostrarMensagem($"Você não tem mais {item.Nome}.");
            return;
        }

        DroidPart novaPeca = item.SlotDePeca switch
        {
            TipoDePeca.Braco => new Braco(item.Nome, 0, Raridade.Comum, item.AtributoBonificado, item.ValorDoBonus),
            TipoDePeca.Perna => new Perna(item.Nome, 0, Raridade.Comum, item.AtributoBonificado, item.ValorDoBonus),
            TipoDePeca.Tronco => new Tronco(item.Nome, 0, Raridade.Comum, item.AtributoBonificado, item.ValorDoBonus),
            TipoDePeca.Cabeca => new Cabeca(item.Nome, 0, Raridade.Comum, item.AtributoBonificado, item.ValorDoBonus),
            _ => null
        };

        if (novaPeca == null) return;

        droid.EquiparPeca(item.SlotDePeca.Value, novaPeca);
        MostrarMensagem($"{item.Nome} equipado!");
        _salvamento.Salvar();
        AtualizarListaComRebuild();
    }

    // NOVO (Estagio 3): itens de uso unico fora de batalha que nao sao cura
    // nem equipamento. Cada acao tem a logica minima decidida sem
    // alinhamento extra (ver conversa) -- facil de trocar depois.
    void UsarAcaoForaDeBatalha(DefinicaoDeItem item, GerenciadorDeEstado gerenciador, Droid droid)
    {
        // Sinalizador de Retorno e um caso especial: "retorna ao ultimo
        // save" reaproveitando SalvamentoJson.Carregar() inteiro (o mesmo
        // Load do botao "Carregar" do Menu do Mundo) em vez de um sistema
        // de teleporte proprio. Por isso NAO consome o item aqui -- o load
        // ja restaura o inventario para o estado salvo (incluindo o
        // proprio Sinalizador, se ele ja estava la antes). Efeito colateral
        // aceito: tambem desfaz progresso desde o ultimo save, igual um
        // "Carregar" manual faria.
        if (item.AcaoForaDeBatalha == AcaoForaDeBatalha.SinalizadorDeRetorno)
        {
            MostrarMensagem($"{droid.Nome} usou {item.Nome}!");
            _salvamento.Carregar();
            return;
        }

        if (!gerenciador.TentarRemoverItem(item.Id, 1))
        {
            MostrarMensagem($"Você não tem mais {item.Nome}.");
            return;
        }

        switch (item.AcaoForaDeBatalha)
        {
            case AcaoForaDeBatalha.KitDeAcampamento:
                droid.Hp = droid.HpMax;
                MostrarMensagem($"{droid.Nome} acampou e recuperou todo o HP!");
                _salvamento.Salvar();
                break;

            case AcaoForaDeBatalha.ChaveDeAcesso:
                if (!string.IsNullOrEmpty(item.ChaveDeFlag))
                {
                    gerenciador.DefinirFlag(item.ChaveDeFlag, true);
                }
                MostrarMensagem($"{item.Nome} usada.");
                _salvamento.Salvar();
                break;
        }

        AtualizarListaComRebuild();
    }

    void AtualizarListaComRebuild()
    {
        AtualizarLista();
        if (painelListaDeItens != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(painelListaDeItens.GetComponent<RectTransform>());
    }

    void MostrarMensagem(string mensagem)
    {
        if (textoMensagem != null) textoMensagem.text = mensagem;
        Debug.Log($"[Bag] {mensagem}");
    }
}
