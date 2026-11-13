using System;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Saídas da derrota. Nenhuma ação desta tela grava progresso.</summary>
public class GameOverManager : MonoBehaviour
{
    public Button botaoContinuar;
    public Button botaoSair;
    public TextMeshProUGUI textoAviso;
    public GameObject eventosLocais;

    private SalvamentoJson _salvamento = new SalvamentoJson();
    private bool _emTransicao;

    void Start()
    {
        // O Canvas de exploração pode trazer um EventSystem persistente.
        if (eventosLocais != null)
            eventosLocais.SetActive(EventSystem.current == null || EventSystem.current.gameObject == eventosLocais);
        if (EventSystem.current != null && botaoContinuar != null)
            EventSystem.current.SetSelectedGameObject(botaoContinuar.gameObject);
    }

    /// <summary>Lê e restaura sem navegar nem escrever. Caminho explícito permite validação isolada.</summary>
    public static DadosDoJogo PrepararContinuacao(SalvamentoJson salvamento, GerenciadorDeEstado estado)
    {
        if (salvamento == null || !salvamento.ExisteSave())
            throw new InvalidOperationException("Nenhum save ou checkpoint disponível. Use Sair para voltar ao menu inicial.");
        var dados = salvamento.LerDados();
        if (dados.droid.hp <= 0)
            throw new InvalidOperationException("O último save não contém um Droid com HP recuperável. Use Sair para voltar ao menu inicial.");
        // Campos opcionais de versões antigas ficam vazios somente no DTO em memória.
        // Rejeitar entradas quebradas antes de aplicar evita restauração parcial.
        dados.flagsDeHistoria = dados.flagsDeHistoria ?? new List<FlagDeHistoria>();
        dados.itens = dados.itens ?? new List<DroidsCode.DroidCore.ItemSalvo>();
        dados.contadores = dados.contadores ?? new List<ContadorDeQuest>();
        dados.droid.tecnicas = dados.droid.tecnicas ?? new List<TecnicaSalva>();
        if (dados.flagsDeHistoria.Exists(f => f == null || f.chave == null)
            || dados.itens.Exists(i => i == null || i.id == null)
            || dados.contadores.Exists(c => c == null || c.chave == null)
            || dados.droid.tecnicas.Exists(t => t == null || t.nome == null))
            throw new InvalidOperationException("O save contém entradas de progresso inválidas. Arquivo preservado.");
        foreach (var tecnica in dados.droid.tecnicas)
        {
            tecnica.efeitosDeAtributo = tecnica.efeitosDeAtributo ?? new List<EfeitoDeAtributoSalvo>();
            tecnica.efeitosDeDanoPorTurno = tecnica.efeitosDeDanoPorTurno ?? new List<EfeitoDeDanoPorTurnoSalvo>();
            if (tecnica.efeitosDeAtributo.Exists(e => e == null) || tecnica.efeitosDeDanoPorTurno.Exists(e => e == null))
                throw new InvalidOperationException("O save contém uma técnica inválida. Arquivo preservado.");
        }
        SalvamentoJson.AplicarDadosSemTrocarCena(dados, estado);
        return dados;
    }

    public void Continuar()
    {
        if (_emTransicao) return;
        DefinirTransicao(true);
        try
        {
            var dados = PrepararContinuacao(_salvamento ?? (_salvamento = new SalvamentoJson()), GerenciadorDeEstado.Instancia);
            SceneManager.LoadScene(dados.cena);
        }
        catch (Exception e) when (FalhaRecuperavel(e))
        {
            MostrarFalha("Não foi possível continuar. " + e.Message);
        }
    }

    public void Sair()
    {
        if (_emTransicao) return;
        DefinirTransicao(true);
        try
        {
            GerenciadorDeEstado.Instancia.LimparBatalhaPendente();
            GerenciadorDeEstado.Instancia.ZerarMenusAbertos();
            SceneManager.LoadScene("MainMenu");
        }
        catch (Exception e) when (FalhaRecuperavel(e))
        {
            MostrarFalha("Não foi possível abrir o menu inicial. " + e.Message);
        }
    }

    static bool FalhaRecuperavel(Exception e) => e is IOException || e is UnauthorizedAccessException
        || e is InvalidOperationException || e is ArgumentException || e is NotSupportedException || e is UnityException;

    void MostrarFalha(string mensagem)
    {
        if (textoAviso != null) textoAviso.text = mensagem;
        DefinirTransicao(false);
    }

    void DefinirTransicao(bool ativa)
    {
        _emTransicao = ativa;
        if (botaoContinuar != null) botaoContinuar.interactable = !ativa;
        if (botaoSair != null) botaoSair.interactable = !ativa;
    }
}
