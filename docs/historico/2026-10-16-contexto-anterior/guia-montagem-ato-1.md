# Guia de montagem do Ato 1 no Editor

Passo a passo completo, sem "provavelmente". Onde a resposta dependeria de algo
que eu não vi na sua cena, as duas variações estão descritas na mesma etapa.

**Ordem recomendada:** faça a Etapa 1 e teste. Ela sozinha já dá diálogo,
flags, portão e Apollo Dica funcionando — se algo estiver errado, você descobre
com 3 GameObjects na tela, não com 30.

---

## Etapa 0 — Importar

1. Copie `Assets/Scripts/Historia/` (7 arquivos) e
   `Assets/Scripts/Combat/CatalogoDeInimigos.cs` para o projeto.
2. Substitua os 6 arquivos alterados: `BattleManager.cs`, `NpcInterativo.cs`,
   `TerminalUIManager.cs`, `EncounterZone.cs`, `MenuMundoManager.cs`,
   `Estado/GerenciadorDeEstado.cs`, `Scripting/DroidScriptRunner.cs`.
3. Cole as entradas de `PARA_COLAR_CatalogoDeItens.md` no seu `CatalogoDeItens.cs`.
4. Espere a compilação. **Se aparecer erro**, é quase certo que seja um nome de
   campo do `DefinicaoDeItem` diferente do que inferi — o Console aponta a linha.
5. Adicione as cenas novas em **File → Build Settings → Scenes In Build**.

---

## Etapa 1 — O painel de diálogo (faça isto primeiro)

O painel mora dentro do **mesmo Canvas raiz que já tem o `MenuMundoManager`** —
aquele que recebe `DontDestroyOnLoad`. Assim ele viaja entre cenas de graça.

1. Nesse Canvas, crie `PainelDialogo` (UI → Panel). Ancore embaixo, ocupando
   ~25% da altura da tela. **Deixe desativado** (checkbox do topo do Inspector).
2. Dentro dele:
   - `TextoFalante` (TextMeshPro - Text), canto superior esquerdo, fonte menor.
   - `TextoFala` (TextMeshPro - Text), ocupando o resto.
   - `BotaoAvancar` (UI → Button). Pode ser um botão discreto com "▸" no canto
     inferior direito, **ou** um Button esticado sobre o painel inteiro com a
     imagem transparente (clicar em qualquer lugar avança). Qualquer um serve.
   - `PainelDeEscolhas` (UI → Panel), com **Vertical Layout Group** +
     **Content Size Fitter** (Vertical Fit = Preferred Size). Deixe **desativado**.
     *(Esse par de componentes é o mesmo que faltava e quebrou a lista da Loja —
     não pule.)*
3. No `PainelDialogo`, **Add Component → DialogoUIManager**. Preencha:
   - `Painel` → o próprio `PainelDialogo`
   - `Texto Falante` → `TextoFalante`
   - `Texto Fala` → `TextoFala`
   - `Botao Avancar` → `BotaoAvancar`
   - `Painel De Escolhas` → `PainelDeEscolhas`
   - `Prefab Botao Escolha` → **reuse o mesmo prefab de botão da Bag/Loja**
     (`BotaoItemTemplate`). Se ele estiver como objeto de cena e não Prefab,
     arraste-o para a pasta `Assets/Prefabs` primeiro e use o Prefab.

**Teste agora:** crie um GameObject vazio na `City`, Collider2D (Is Trigger),
`GatilhoDeDialogo`, modo `Ao Entrar No Trigger`, uma fala qualquer. Ande até
ele. Se o painel abre, o player congela, E avança a fala, a base está de pé.

---

## Etapa 2 — Ferrovale (cena `City`) — Sessão 2

> **Decisão de design assumida:** Ferrovale **é** a cena `City`. O mercado de
> trocas do Enredo é a Loja que você já construiu; o NPC lojista vira o Senhor
> Tácio. Se preferir separar em outra cena, nada aqui muda além dos nomes.

### 2.1 Objeto de controle

GameObject vazio na raiz, nome `ControleDeHistoria`. **Nunca desative ele** —
é ele que religa os outros quando as flags mudam. Add Component →
`CondicaoDeHistoria`.

### 2.2 Os três NPCs da fila

Para **Dara** e para o **homem da fila**: GameObject com sprite, Collider2D
(Is Trigger, um pouco maior que o sprite), e `GatilhoDeDialogo`:

| Campo | Dara | Homem da fila |
| :--- | :--- | :--- |
| Modo | Ao Interagir | Ao Interagir |
| Flags Necessarias | `ato1_casa_resolvida` | `ato1_casa_resolvida` |
| Flags Que Impedem | `ato1_sessao2` | `ato1_sessao2` |
| Flag Ao Terminar | `ato1_falou_dara` | `ato1_falou_fila` |
| Apenas Uma Vez | ✔ | ✔ |

Falas (rascunho, derivado do Enredo — reescreva à vontade):

- **Dara** — *"Cortaram a cota. Metade do que era ontem."* / *"E ninguém quer ser o primeiro a dizer por quê."*
- **Homem da fila** — *"Não falhou nada. Foi o Ivo que mexeu."* / *"Fechou a estação. Disse que vai 'renegociar' com quem quiser ouvir."*

Para o **Tácio**: use o NPC da Loja que já existe. Adicione um
`GatilhoDeDialogo` **no mesmo GameObject**, com **Modo = Apenas Por Script**
(importante: senão ele e o `NpcInterativo` brigam pela tecla E). Deixe
`Tipo = Loja` no `NpcInterativo`. O `NpcInterativo` patchado mostra a fala e só
então abre a vitrine.

- **Tácio** — *"Água filtrada, mesmo preço de ontem. Por enquanto."* / *"Se o poço secar de vez, nem esse preço vai existir."*

### 2.3 O comentário do Apollo que fecha a Sessão 2

GameObject vazio, Collider2D (Is Trigger) largo, perto do portão leste.
`GatilhoDeDialogo`:

- Modo: **Ao Entrar No Trigger**
- Flags Necessarias: `ato1_falou_dara`, `ato1_falou_fila`
- Flags Que Impedem: `ato1_sessao2`
- Flag Ao Terminar: **`ato1_sessao2`**
- Desativar Apos Concluir: ✔

Falas: *"Renegociar com quem, Lip?"* → *"Com a Apex."* → *"A única coisa que a Apex quer nessa cidade são Droids como eu. Os que ainda funcionam offline."*

### 2.4 O portão leste — o gate pedagógico

No GameObject da porta que leva à cena `Game`:

1. `TransicaoDeCena` (já deve existir, ou crie): `Nome Cena Destino` = `Game`,
   `Ponto De Chegada` = as coordenadas do ponto de entrada no mapa.
   **Marque `Trancada`.**
2. Crie um `TMP_Text` filho para o aviso, deixe **desativado**, e arraste em
   `Texto De Bloqueio`.
3. **Add Component → `TrancaPorHistoria`**:
   - `Flags Necessarias` → `ato1_sessao2`
   - `Exigir Tecnica Configurada` → **✔**
   - `Minimo De Acoes` → `2`
   - As duas mensagens já vêm preenchidas com texto útil; ajuste o tom.

É esta caixa de seleção que faz o TCC cumprir o que promete: sem abrir o
terminal e escrever `droid.aprenderTecnica("Impacto", 2)`, o jogador não sai
de Ferrovale.

---

## Etapa 3 — A saída leste e o primeiro combate (cena `Game`) — Sessão 3

1. **Fala do Apollo sobre os golpes.** `GatilhoDeDialogo`, Modo = Ao Entrar No
   Trigger, logo depois do ponto de chegada. Flag Ao Terminar:
   `ato1_apollo_explicou`. Desativar Apos Concluir ✔.
   Falas: o golpe de impacto (barato), o de sobrecarga (forte, deixa o braço
   vulnerável) e o escaneamento (revela o ponto fraco). Esta é a única vez no
   jogo em que os comandos do terminal são explicados em ficção — aproveite.
2. **Batalha roteirizada.** GameObject vazio, Collider2D (Is Trigger),
   `GatilhoDeBatalha`:
   - `Id Do Inimigo` → `sucata_captura`
   - `Nome Cena Batalha` → `Battle`
   - `Flags Necessarias` → `ato1_sessao2`
   - `Flag De Vitoria` → **`ato1_sessao3`**
   - `Exigir Tecla` → desmarcado (a luta vem até ele, como no Enredo)

---

## Etapa 4 — A escolha de facção (cena `Game`) — Sessão 4

1. GameObject `RashaETeodoro` com os dois sprites. **Deixe desativado.**
2. No `ControleDeHistoria` da cena `Game`, adicione uma Regra:
   - Descrição: `Rasha e Teodoro aparecem após o combate`
   - Alvos → `RashaETeodoro`
   - Flags Necessarias → `ato1_sessao3`
   - Flags Que Impedem → `ato1_faccao_escolhida`
3. No `RashaETeodoro`, Collider2D (Is Trigger) + `GatilhoDeDialogo`:
   - Modo: Ao Entrar No Trigger
   - Flag Ao Terminar: `ato1_faccao_escolhida`
   - Desativar Apos Concluir: ✔
   - **Falas**: a conversa dos dois (Rasha recrutando, Teodoro pedindo pra
     pensar, Apollo perguntando "e se ele não quiser nenhum dos dois?").
   - **Escolhas** (3 entradas):

| Texto | Flag Ao Escolher | Id Item Concedido | Resposta Apos Escolher |
| :--- | :--- | :--- | :--- |
| Flor de Ferro | `ato1_faccao_ferro` | `nucleo_sobrecarga` | *"Instala no braço do seu Droid. Ferro não é sobre economizar força."* |
| Lótus Branca | `ato1_faccao_lotus` | `modulo_ressonancia` | *"Às vezes vencer é fazer a batalha nem precisar acontecer."* |
| Nenhum dos dois | `ato1_faccao_desgarrado` | *(vazio)* | *"Sem vantagem extra, então. Vamos ter que confiar só no que já temos."* |

O item cai direto no inventário e é salvo na hora. O Núcleo é equipado pela
Bag; a Ressonância aparece no menu Itens **dentro** da batalha.

---

## Etapa 5 — A Cratera (cena `Game`) — Sessões 5 e 6

1. **Zona de encontro.** Colider2D grande cobrindo a região, `EncounterZone`:
   - `Id Do Inimigo` → `droid_selvagem`
   - `Flags Necessarias` → `ato1_faccao_escolhida`
   - `Chance De Encontro` → 0.12 pra começar
2. **Puzzle 2 — painel de energia.** GameObject vazio com `VerificadorDePuzzle`:
   - `Terminal` → deixe **vazio** (ele acha sozinho)
   - `Nome Da Variavel` → `porta_aberta`
   - `Comparacao` → `Igual A`
   - `Valor De Vitoria` → `1`
   - `Porta Para Destrancar` → a `TransicaoDeCena` da passagem (marque
     `Trancada` nela)
   - `Flag Ao Resolver` e `Flag Que Ja Resolve` → ambos `ato1_puzzle_energia`
   - `Log De Depuracao` → ligue enquanto monta, desligue depois

   Coloque perto um `GatilhoDeDialogo` com o enunciado na voz do Apollo:

   ```
   energia = 0
   porta_aberta = 0
   -- Carregue a energia até 50 e só então marque porta_aberta = 1
   ```

3. **O chefe.** `GatilhoDeBatalha`:
   - `Id Do Inimigo` → `droid_do_ivo`
   - `Flags Necessarias` → `ato1_puzzle_energia`
   - `Flag De Vitoria` → **`ato1_sessao6`**
   - `Exigir Tecla` → ✔ (a conversa com o Ivo vem antes; ele não emboscada)

   Antes dele, um `GatilhoDeDialogo` com a confissão do Ivo
   (Flag Ao Terminar: `ato1_ivo_confessou`).

---

## Etapa 6 — Retorno e fim do Ato (cena `City`) — Sessão 7

1. **Dara reage.** Três `GatilhoDeDialogo` distintos no mesmo ponto, cada um
   com `Flags Necessarias` = `ato1_sessao6` **+** a flag da facção
   (`ato1_faccao_ferro` / `_lotus` / `_desgarrado`). Só um vai disparar.
2. **A revelação do Apollo.** `GatilhoDeDialogo` na porta da casa do Lip,
   Flags Necessarias `ato1_sessao6`, Flag Ao Terminar `ato1_concluido`.
   O resquício de assinatura de rede da Artemis — o protocolo procurando por
   **ele**, não por um droid offline qualquer.
3. **Tela de fim de Ato.** Um Panel simples com "Fim do Ato 1", ligado por uma
   Regra de `CondicaoDeHistoria` com `Flags Necessarias` = `ato1_concluido`.

---

## Etapa 7 — Migrar a `CasaLip` (opcional, faça por último)

O `PuzzleAguaCasaChecker` original **continua funcionando** — não mexa nele
até o resto estar testado. Quando quiser migrar:

1. No GameObject do checker, adicione `VerificadorDePuzzle`:
   `Nome Da Variavel` = `nivel_agua`, `Comparacao` = `Maior Ou Igual`,
   `Valor De Vitoria` = `100`, `Porta Para Destrancar` = a porta,
   `Texto De Apollo` = o mesmo TMP, `Flag Ao Resolver` e `Flag Que Ja Resolve`
   = `ato1_casa_resolvida`.
2. **Remova** o `PuzzleAguaCasaChecker`.
3. Mova o novo componente para um GameObject da **própria cena `CasaLip`**, não
   para o painel do terminal — esse é o ponto da migração (ver
   `DOCUMENTACAO_TECNICA.md` §4.8).

⚠️ **Sem esta etapa**, a flag `ato1_casa_resolvida` nunca é gravada, e os
gatilhos da Etapa 2 que a exigem não disparam. Alternativa provisória: tire
`ato1_casa_resolvida` das `Flags Necessarias` da Etapa 2.

---

## Lista de flags do Ato 1

Use exatamente estes nomes — o `Dictionary` é case-sensitive.

```
ato1_casa_resolvida        puzzle da água (CasaLip)
ato1_falou_dara            Sessão 2
ato1_falou_fila            Sessão 2
ato1_sessao2               descobriu que foi o Ivo → destranca portão leste
ato1_apollo_explicou       fala sobre os golpes
ato1_sessao3               venceu as Sucatas de Captura
ato1_faccao_escolhida      passou pela cena de Rasha/Teodoro
ato1_faccao_ferro          \
ato1_faccao_lotus           > mutuamente exclusivas
ato1_faccao_desgarrado     /
ato1_puzzle_energia        Puzzle 2 da Cratera
ato1_ivo_confessou         diálogo antes do chefe
ato1_sessao6               venceu o Droid Improvisado
ato1_concluido             fim do Ato 1
```

---

## Se der errado

| Sintoma | Causa quase certa |
| :--- | :--- |
| Diálogo não abre | Painel não está dentro do Canvas persistente, ou `Painel`/`Texto Fala` vazios no `DialogoUIManager`. O Console avisa. |
| Player fica congelado depois do diálogo | Contador de menus desbalanceado. Confira que o `NpcInterativo` do NPC **não** está com `Tipo = Generico` chamando o diálogo por fora. |
| Fala pula sozinha | Dois componentes reagindo à mesma tecla. Se o GameObject tem `NpcInterativo` **e** `GatilhoDeDialogo`, o gatilho precisa estar em `Apenas Por Script`. |
| Escolhas não aparecem | `Painel De Escolhas` ou `Prefab Botao Escolha` vazios; ou o prefab de botão está ativo na cena em vez de ser molde. |
| Puzzle nunca resolve | Ligue `Log De Depuracao`. Quase sempre é nome de variável diferente, ou o jogador fechou e reabriu o terminal (isso **zera** as variáveis Lua — é esperado). |
| Inimigo errado na batalha | `Id Do Inimigo` com typo. O Console avisa e cai no Inspector da cena. |
| ESC abre o menu dentro da batalha | `Cenas Bloqueadas` do `MenuMundoManager` sem `Battle`. |
| Itens de facção à venda por 1 Gold | Falta o filtro `VendavelNaLoja` — ver seção 3 de `PARA_COLAR_CatalogoDeItens.md`. |
