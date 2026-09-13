using System.Collections.Generic;
using DroidsCode.DroidCore; // necessario p/ ItemSalvo (definido em CatalogoDeItens.cs)

// DTOs planos, sem Dictionary (JsonUtility nao serializa Dictionary).
// Separado do Droid/GerenciadorDeEstado de proposito -- esses sao os
// objetos de dominio reais, isso aqui e so o "retrato" que vai pro disco.

// CORRECAO (12/09/2026): TecnicaSalva so guardava nome/nivelDeDano -- os
// efeitos (Stun/Envenenamento) da tecnica eram perdidos ao salvar/carregar.
// EfeitoDeAtributoSalvo/EfeitoDeDanoPorTurnoSalvo sao DTOs planos, mesma
// logica das outras classes desta pagina (JsonUtility nao serializa
// Dictionary, mas List<T> de tipos [Serializable] funciona normalmente).
[System.Serializable]
public class EfeitoDeAtributoSalvo
{
    public string nomeExibicao;
    public int atributo; // enum TipoAtributo salvo como int (JsonUtility serializa enum como int)
    public int valor;
    public int duracaoEmTurnos;
}

[System.Serializable]
public class EfeitoDeDanoPorTurnoSalvo
{
    public string nomeExibicao;
    public int danoPorTurno;
    public int duracaoEmTurnos;
}

[System.Serializable]
public class TecnicaSalva
{
    public string nome;
    public int nivelDeDano;
    public List<EfeitoDeAtributoSalvo> efeitosDeAtributo = new List<EfeitoDeAtributoSalvo>();
    public List<EfeitoDeDanoPorTurnoSalvo> efeitosDeDanoPorTurno = new List<EfeitoDeDanoPorTurnoSalvo>();
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
    // REFATORACAO (Estagio 1 — 12/09/2026): salvava "hpMax" direto; agora
    // HpMax e CALCULADO (HpMaxBase + %VIT, ver Droid.cs), entao o que
    // precisa ser persistido e a BASE, nao o total. Salvar o total antigo
    // faria o bonus de VIT se acumular a cada save/load. Saves antigos (sem
    // este campo) carregam hpMaxBase=0 -- ver nota em SalvamentoJson.Carregar.
    public int hpMaxBase;
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
    public int versao = 3; // v3 (13/09/2026): + itens (Inventario, ver ItemSalvo)
    public DroidSalvo droid;
    public float posicaoX;
    public float posicaoY;
    public float posicaoZ;
    public string cena;

    // Escalavel: qualquer flag de historia futura (bosses derrotados, fases
    // completas, dialogos vistos) entra aqui sem mudar a estrutura.
    public List<FlagDeHistoria> flagsDeHistoria = new List<FlagDeHistoria>();

    // Estagio 2 (13/09/2026): inventario real, DTO plano mesmo padrao de
    // FlagDeHistoria/TecnicaSalva (JsonUtility nao serializa Dictionary).
    // Saves v1/v2 (sem este campo) carregam lista vazia -- ver
    // SalvamentoJson.Carregar.
    public List<ItemSalvo> itens = new List<ItemSalvo>();
}
