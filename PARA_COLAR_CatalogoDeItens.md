# Para colar em `CatalogoDeItens.cs` — itens de facção do Ato 1

**Por que isto não veio como arquivo pronto:** `CatalogoDeItens.cs` não estava
entre os 20 `.cs` enviados. Reescrevê-lo de memória, a partir só de como ele é
*usado* em `BagUIManager`/`BattleManager`/`LojaUIManager`, apagaria tudo que eu
não vi — todas as poções, os itens de Fora de Batalha, os `Id*` constantes.
A regra §0 item 4 da documentação técnica existe exatamente pra isso.

Então aqui vai o trecho pra colar, com os nomes de campo **inferidos do uso
real**. Se algum nome não bater com o seu arquivo, é ajuste de 1 linha.

---

## 1. As duas recompensas de facção

As três recompensas do Enredo mapeiam, uma a uma, em categorias que o sistema
de item **já suporta**. Nenhuma mecânica nova:

| Facção | Recompensa do Enredo | Vira, no código |
| :--- | :--- | :--- |
| Flor de Ferro | Núcleo de Sobrecarga — "multiplica o dano do golpe" | Item `Equipavel`, slot Braço, `+6 For` |
| Lótus Branca | Módulo de Ressonância — "desativa a agressividade, uma vez por combate" | Item `Debuff` que aplica **Stun** de 3 turnos, quantidade 1 (a escassez é a regra de "uma vez") |
| Desgarrado | Nenhum bônus | A ausência das duas entradas acima. Já funciona |

Cole dentro do dicionário `Todos`, no mesmo formato das entradas que já existem:

```csharp
// ----------------------------------------------------------------
// ATO 1 — recompensas de facção (Sessão 4 do Enredo).
// Entregues por EscolhaDeDialogo.idItemConcedido, nunca comprados.
// ----------------------------------------------------------------
{
    IdNucleoDeSobrecarga,
    new DefinicaoDeItem
    {
        Id = IdNucleoDeSobrecarga,
        Nome = "Núcleo de Sobrecarga",
        Tipo = TipoDeItem.Equipavel,
        SlotDePeca = TipoDePeca.Braco,
        AtributoBonificado = TipoAtributo.For,
        ValorDoBonus = 6,
        UsavelEmBatalha = false,
        UsavelForaDeBatalha = true,
        Preco = PrecoPadrao,
        VendavelNaLoja = false   // ver seção 3 abaixo
    }
},
{
    IdModuloDeRessonancia,
    new DefinicaoDeItem
    {
        Id = IdModuloDeRessonancia,
        Nome = "Módulo de Ressonância",
        Tipo = TipoDeItem.Debuff,
        UsavelEmBatalha = true,
        UsavelForaDeBatalha = false,
        Preco = PrecoPadrao,
        VendavelNaLoja = false,
        // Reaproveita o motor de efeito de técnica, igual os outros
        // Buff/Debuff já fazem. "Stun" é flag checada por NOME em
        // CombatEngine.EstaAtordoado — Atributo/Valor ficam no default
        // de propósito, pra não mexer em ObterTotal.
        EfeitoDeAtributoAplicado = new EfeitoDeAtributo
        {
            NomeExibicao = "Stun",
            Atributo = TipoAtributo.For,
            Valor = 0,
            DuracaoEmTurnos = 3
        }
    }
},
```

E as constantes de Id, junto das que já existem (`IdPocaoPequena` etc.):

```csharp
public const string IdNucleoDeSobrecarga = "nucleo_sobrecarga";
public const string IdModuloDeRessonancia = "modulo_ressonancia";
```

---

## 2. Os Ids que o `CatalogoDeInimigos` espera

`Combat/CatalogoDeInimigos.cs` já referencia dois Ids nas tabelas de drop:
`"pocao_pequena"` e `"pocao_media"`. Se no seu catálogo eles têm outro texto,
troque lá — ou troque aqui. Drop com Id inválido é ignorado em silêncio
(`BattleManager.AplicarRecompensas` já protege), então o sintoma seria "o
inimigo nunca dropa nada", sem erro no Console.

---

## 3. ⚠️ Um detalhe que estraga a escolha de facção se passar batido

`CatalogoDeItens.ItensDaLoja` expõe **`Todos.Values` inteiro**, sem filtro — e
todo item vale `PrecoPadrao` = 1 Gold.

Com os dois itens acima no catálogo e nada mais, o jogador entra na Loja do
Tácio e compra **as duas recompensas de facção por 1 Gold cada**, antes mesmo
de conhecer Rasha e Teodoro. A decisão mais importante do Ato 1 vira uma
compra de padaria.

O conserto é pequeno. Adicione o campo em `DefinicaoDeItem`:

```csharp
// Ato 1: itens de história/recompensa não entram na vitrine da Loja.
// Default true pra que todo item que já existe continue vendável
// exatamente como antes.
public bool VendavelNaLoja = true;
```

E filtre em `ItensDaLoja`:

```csharp
public static IEnumerable<DefinicaoDeItem> ItensDaLoja
{
    get
    {
        foreach (DefinicaoDeItem item in Todos.Values)
        {
            if (item.VendavelNaLoja) yield return item;
        }
    }
}
```

`LojaUIManager.PopularComprar` faz `new List<DefinicaoDeItem>(CatalogoDeItens.ItensDaLoja)`,
então `IEnumerable` funciona sem tocar nele.

**Sobre a aba Vender:** ela lê o inventário direto, não `ItensDaLoja` — então o
jogador ainda consegue *vender* o Núcleo por 1 Gold. Se isso incomodar, a mesma
flag resolve com um `if (!item.VendavelNaLoja) continue;` em `PopularVender`.
Deixei de fora de propósito: vender a própria recompensa é uma burrice que o
jogador tem direito de cometer, e bloquear é decisão de design sua, não minha.
