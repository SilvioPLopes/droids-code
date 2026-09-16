# Entrega — Ato 1 jogável + trio de documentos reconciliado (15/09/2026)

## O que tem aqui

```
Assets/Scripts/
├── Historia/                    ← 7 arquivos NOVOS (sistema de história)
│   ├── DialogoUIManager.cs
│   ├── GatilhoDeDialogo.cs      (+ FalaDeDialogo, EscolhaDeDialogo, CondicoesDeHistoria)
│   ├── CondicaoDeHistoria.cs
│   ├── GatilhoDeBatalha.cs
│   ├── VerificadorDePuzzle.cs
│   ├── TrancaPorHistoria.cs
│   └── ApolloDica.cs
├── Combat/CatalogoDeInimigos.cs ← NOVO
├── BattleManager.cs             ← alterado
├── NpcInterativo.cs             ← alterado
├── TerminalUIManager.cs         ← alterado
├── EncounterZone.cs             ← alterado
├── MenuMundoManager.cs          ← alterado
├── Estado/GerenciadorDeEstado.cs   ← alterado
├── Scripting/DroidScriptRunner.cs  ← alterado
├── readme.md                       ← atualizado
├── DOCUMENTACAO_TECNICA.md         ← atualizado
└── CHECKLIST_DE_DESENVOLVIMENTO.md ← atualizado

GUIA_DE_MONTAGEM_ATO1.md          ← passo a passo de Editor
PARA_COLAR_CatalogoDeItens.md     ← trecho pros 2 itens de facção
```

Só sobrescreva os arquivos listados como "alterado". Nada mais foi tocado.

## Ordem de importação

1. Copie a pasta `Historia/` e o `CatalogoDeInimigos.cs`.
2. Sobrescreva os 7 alterados.
3. Cole o trecho de `PARA_COLAR_CatalogoDeItens.md`.
4. Siga `GUIA_DE_MONTAGEM_ATO1.md` — pare depois da **Etapa 1** e teste.

## Divergências do trio: 3 fechadas, 1 aberta

**Fechadas com o arquivo real em mãos:**
- Bônus de peça em atributo **é real** (`BagUIManager` constrói a peça com
  `AtributoBonificado`/`ValorDoBonus`). A §4.1 dizia "hoje sempre 0" e
  contradizia a própria §4.3 do mesmo documento.
- `HpMax` é **calculado** (`HpMaxBase` + %VIT + peça). A §5 não tinha fórmula.
- `IParticipanteDeCombate` expõe `EfeitosAtivos`, `EfeitosDeDanoAtivos` e
  `ChanceDeResistirEfeito`. A §4.2 listava só 6 membros.
- Bônus: `TransicaoDeCena.trancada`/`textoDeBloqueio` **são genéricos da
  classe** — a pergunta que a §4.7 e a §9 deixaram em aberto.

**Ainda aberta:** HIT/FLEE e crítico por LUK. O `readme` diz implementado; a
§8 diz planejado. Resistência por INT está **confirmada** (visível no
`BattleManager`). O resto exige `Droid.cs` e `CombatEngine.cs`, que não vieram.
Registrei isso explicitamente no fim da §5 em vez de escolher um dos lados.

## Dois bugs achados de brinde

1. **ESC duplo na batalha.** Desde que o `MenuMundoManager` ganhou
   `DontDestroyOnLoad`, o Canvas do menu passou a existir dentro da cena
   `Battle` — onde o `BattleManager` também escuta ESC. Um único ESC fechava a
   lista de ataques *e* abria o menu de pausa por cima da batalha. O comentário
   no próprio `BattleManager` previa esse dia. Corrigido dos dois lados.
2. **Itens de facção à venda por 1 Gold.** `ItensDaLoja` expõe o catálogo
   inteiro sem filtro. Com o Núcleo e a Ressonância no catálogo, o jogador
   compraria as duas recompensas na Loja antes de conhecer Rasha e Teodoro.
   O conserto (campo `VendavelNaLoja`) está em `PARA_COLAR_CatalogoDeItens.md`.

## O que não foi feito e por quê

- **Motor de efeitos** (empilhamento / não-zeragem entre batalhas): fora por
  instrução sua. Nada nesta entrega depende de corrigi-los.
- **`CatalogoDeItens.cs`**: não estava entre os 20 arquivos. Reescrevê-lo de
  memória apagaria todos os itens que eu não vi. Virou trecho pra colar.
- **`PuzzleAguaCasaChecker.cs`**: também não veio. O `VerificadorDePuzzle` é
  **aditivo** — o checker antigo continua funcionando e os dois coexistem até
  você migrar a `CasaLip` (Etapa 7 do guia, opcional e por último).
