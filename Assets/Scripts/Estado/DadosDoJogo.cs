using System.Collections.Generic;

// DTOs planos, sem Dictionary (JsonUtility nao serializa Dictionary).
// Separado do Droid/GerenciadorDeEstado de proposito -- esses sao os
// objetos de dominio reais, isso aqui e so o "retrato" que vai pro disco.

[System.Serializable]
public class TecnicaSalva
{
    public string nome;
    public int nivelDeDano;
}

[System.Serializable]
public class FlagDeHistoria
{
    public string chave;
    public bool valor;
}

[System.Serializable]
public class DroidSalvo
{
    public string nome;
    public int hp;
    public int hpMax;
    public int statsFor;
    public int statsAgi;
    public int statsVit;
    public int statsInt;
    public int statsDex;
    public int statsLuk;
    public int pontosDisponiveis;
    public int nivel;
    public int experiencia;
    public List<TecnicaSalva> tecnicas = new List<TecnicaSalva>();
}

[System.Serializable]
public class DadosDoJogo
{
    public int versao = 1; // pra migrar formato no futuro sem quebrar saves antigos
    public DroidSalvo droid;
    public float posicaoX;
    public float posicaoY;
    public float posicaoZ;
    public string cena;

    // Escalavel: qualquer flag de historia futura (bosses derrotados, fases
    // completas, dialogos vistos) entra aqui sem mudar a estrutura.
    public List<FlagDeHistoria> flagsDeHistoria = new List<FlagDeHistoria>();
}
