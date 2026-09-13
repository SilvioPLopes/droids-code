# Droids Code — Checklist e Roteiro de Desenvolvimento

> **Este documento faz parte de um trio tratado como uma única fonte de
> verdade** — junto com `readme.md` e `DOCUMENTACAO_TECNICA.md`. Marcar algo
> como concluído aqui **exige** checar se `DOCUMENTACAO_TECNICA.md` (mapa de
> classes/árvore de pastas) e `readme.md` (status/roadmap) também precisam de
> atualização na mesma sessão — ver a regra completa e a tabela de
> correspondências em `DOCUMENTACAO_TECNICA.md` §0.2.
>
> ⚠️ **Nota desta entrega:** `DOCUMENTACAO_TECNICA.md` não foi atualizado
> junto nesta sessão (conteúdo completo não enviado) — pendência aberta pra
> quando ele for enviado de novo.

> Este documento existe pra responder uma pergunta específica: **"o que eu faço agora?"**
> Ele é o terceiro pé da documentação, ao lado do `readme.md` (o que o jogo é) e do
> `DOCUMENTACAO_TECNICA.md` (como o código está organizado hoje). Se você (dev solo,
> sem experiência prévia em jogos) está em dúvida sobre prioridade, comece aqui.
>
> **Não inclui GDD/Enredo.** História e design narrativo de longo prazo não entram
> nesta lista — eles distorcem prioridade (empurram itens como "Puzzle 1" antes da
> hora). Este documento só lida com o que é tecnicamente acionável agora.
>
> **Regra de manutenção:** edite marcando itens como concluídos e movendo entre
> seções — não acumule histórico aqui. Se um bug foi corrigido, apague a linha (o
> "aconteceu" fica registrado no commit/no jogo, não precisa duplicar aqui).

---

## ✅ Concluído nesta sessão (Estágio 3 — Sistema de Item, fase 2/2)

Fecha o "🚨 Mecânica essencial faltante" abaixo. Implementado sem alinhamento
de design adicional (decisões tomadas e documentadas inline no código,
fáceis de ajustar depois se não servirem):

- **`TipoDeItem` expandido**: `Cura, Buff, Debuff, Equipavel, ForaDeBatalha`
  (os 4 últimos eram só comentário no enum desde a fase 1). Novo enum
  `AcaoForaDeBatalha` (`SinalizadorDeRetorno, KitDeAcampamento,
  ChaveDeAcesso`) identifica qual lógica cada item fora-de-batalha roda.
- **Buff/Debuff reaproveitam o motor de efeito já existente**
  (`EfeitoDeAtributo`/`EfeitoDeDanoPorTurno`, o mesmo de técnica) em vez de
  criar uma estrutura nova. Buff aplica no próprio Droid (`BattleManager.
  AplicarEfeitoDeItem`); Debuff aplica no inimigo e rola resistência (INT),
  igual efeito de técnica. Exemplos no catálogo: Escudo de Emergência
  (buff de VIT), Granada de Ácido (debuff de VIT), Disruptor EMP (reaproveita
  a flag "Stun"), Corrosivo (dano por turno).
  **Atenção:** Corrosivo herda os 2 bugs abertos do motor de efeitos (ver
  seção de bugs abaixo) — não corrigido nesta entrega de propósito.
- **Equipável dá bônus real de atributo**: `DroidPart` ganhou
  `AtributoBonificado`/`ValorDoBonus` (novo, opcional — não mexe no
  `AtributoPrincipal` int genérico que já existia). `Droid.
  ObterBonusDePecas` deixou de ser placeholder fixo em 0 e agora soma o
  bônus das 4 peças de verdade. Equipar (via Bag) cria a peça a partir do
  catálogo e instala com `Droid.EquiparPeca` — **consome o item, sem
  desequipar/trocar nesta fase** (decisão explícita, documentada no código;
  fácil de evoluir se pedido depois).
- **Fora de batalha (Bag)**: Sinalizador de Retorno (reaproveita
  `SalvamentoJson.Carregar()` inteiro — "volta pro último save", não
  inventa teleporte novo; por isso não consome o item, o load já restaura
  o inventário), Kit de Acampamento (cura total + salva), Chave de Acesso
  (seta uma flag genérica em `GerenciadorDeEstado`, sem destravar conteúdo
  de história específico — isso fica fora do escopo técnico).
- **Gold novo**: `GerenciadorDeEstado.Gold` + `AdicionarGold`/
  `TentarGastarGold` (este último ainda sem uso — não há loja). Persistido
  em `DadosDoJogo` **v4**. Saves v1-v3 carregam Gold = 0 (compatibilidade,
  não quebra o load).
- **Drop de inimigo**: `BattleManager` ganhou `goldMinimo`/`goldMaximo` +
  `tabelaDeDrops` (lista configurável no Inspector, cada entrada com
  id do item + chance independente + quantidade min/max). Rolado em
  `AplicarRecompensas()`, chamado nos dois pontos onde a vitória contra o
  inimigo já era detectada. Salva o jogo na hora (mesmo padrão de "persistir
  imediatamente" que o consumo de item já usa).

**Trabalho de Editor necessário para isso funcionar:**
- Nenhum objeto novo na cena é necessário — Buff/Debuff aparecem
  automaticamente na lista de itens de batalha já existente; Equipável e os
  3 itens Fora de Batalha aparecem automaticamente na Bag já existente.
- Único ajuste manual: preencher `goldMinimo`/`goldMaximo`/`tabelaDeDrops`
  no Inspector de cada `BattleManager` (uma por inimigo/cena de batalha) —
  deixados zerados/vazios por padrão, então nenhum inimigo droppa nada até
  você configurar isso.

**Pendências abertas geradas por este trabalho:**
- Sem loja/compra ainda — só kit inicial + drop. `TentarGastarGold` existe
  mas não é chamado em lugar nenhum ainda.
- Sem "desequipar" — trocar de peça Equipável descarta a anterior (ela não
  volta pro inventário).
- Itens Equipáveis/de exemplo (`Núcleo de Energia`, `Servo-Motor`) e os 3
  Debuffs/1 Buff não têm balanceamento real — valores placeholder, mesma
  situação de `TabelaDeCombate`/`TabelaDeCustos`.
- `DOCUMENTACAO_TECNICA.md` não foi atualizado (ver nota no topo deste
  documento).

---

## 🔴 Bugs abertos (fazer primeiro, nesta ordem)

Detalhe técnico em `DOCUMENTACAO_TECNICA.md` §9.

1. [ ] **Efeitos ativos do Droid nunca são zerados entre batalhas.** Sem efeito visível hoje (só inimigos com técnica ou item Debuff poderiam causar isso), mas é risco pra quando houver inimigo especial ou uso repetido de Debuff.
2. [ ] **Empilhamento sem limite de Veneno/Stun (e agora Corrosivo/Disruptor EMP, itens Debuff).** Aplicar o mesmo efeito várias vezes soma o dano por turno de todas as instâncias; Stun repetido não estende a duração de fato. **CONFIRMADO em teste real (sessão atual):** usar Disruptor EMP (item Debuff que aplica a flag "Stun") duas vezes empilha duas instâncias, em vez de renovar a duração de uma só. Não é um bug novo do sistema de item — é o mesmo bug de técnica, só que agora com uma segunda fonte que aciona ele. **Decisão de comportamento ainda pendente** (cap de instâncias? renovar duração em vez de somar?) — não implementar sem definir isso primeiro.

---

## ❓ Pendência de teste (não confirmado ainda)

- **Equipável (Núcleo de Energia/Servo-Motor) duplicando bônus de atributo ao usar 2x.** Levantado nesta sessão, mas o teste real usou Disruptor EMP (Debuff), não um item Equipável — a suspeita de duplicação **não foi confirmada nem descartada**. Pelo código, `Droid.EquiparPeca` deveria SUBSTITUIR a peça do slot, não somar (ver Estágio 3 abaixo) — mas isso precisa ser testado de fato com 2x Núcleo de Energia (mesmo slot) antes de assumir que está correto.
- **Item equipado não aparece em lugar nenhum da UI.** Nem a Bag, nem a Tela de Status mostram qual peça está em qual slot hoje. Lacuna real, não é bug — falta feature. Ver seção de amadurecimento abaixo.

---

## ✅ Concluído em sessão anterior (13/09/2026 — Estágio 2: Sistema de Item, fase 1/2)

Primeira fatia do sistema de item — cobriu só itens de Cura, com
quantidade real e persistência real (`CatalogoDeItens.cs`, inventário em
`GerenciadorDeEstado`, save v3, `BattleManager`/`BagUIManager` lendo o
inventário real). Ver Estágio 3 acima para a fase 2 (Buff/Debuff/Equipável/
ForaDeBatalha + drop), que fecha o que ficava pendente aqui.

---

## 🚨 Mecânica essencial faltante

> ✅ **Fechada nesta sessão (Estágio 3)** — ver "Concluído nesta sessão" no
> topo. O que restava (drop de inimigo, categorias além de Cura, UI mais
> rica o suficiente pra mostrar cada tipo) foi implementado. O que **ainda
> não existe** (loja/compra, ícones/descrição longa, filtro por categoria
> na UI) fica listado nas pendências acima e no roadmap de amadurecimento
> abaixo — não é mais bloqueante de lançamento, é melhoria.

---

## 🔧 Trabalho de Editor pendente (não é código, é configuração no Inspector)

- [x] ~~`BattleManager`: `painelListaDeItens`/`prefabBotaoItem` — apontado como "ainda mal otimizado"~~ — **corrigido nesta sessão** na Bag (`ListaDeItensBag`), ver "✅ Concluído" abaixo. Confirmar se o mesmo ajuste ainda falta no `BattleManager` (ver item logo abaixo).
- [ ] **NOVO (sessão atual):** o mesmo bug de sobreposição de botões (texto cortado/empilhado no meio da tela) que existia na Bag também aparece nos painéis `painelListaDeAtaques`/`painelListaDeItens` **dentro do `BattleManager`, na cena Battle** — confirmado por print do usuário. Aplicar a MESMA correção (Vertical Layout Group + Content Size Fitter no painel, largura maior no prefab de botão de batalha, Auto Size no texto) nesses dois painéis. Ainda não feito.
- [ ] preencher `goldMinimo`/`goldMaximo`/`tabelaDeDrops` no Inspector de cada `BattleManager` que deveria dropar algo — hoje todos ficam zerados/vazios por padrão (sem drop nenhum) até isso ser configurado manualmente.

### ✅ Concluído nesta sessão — layout da lista de itens da Bag

O texto dos botões de item (ex: "Núcleo de Energia (+5 For) x1") ficava
sobreposto/cortado no meio da tela porque `ListaDeItensBag` estava com
tamanho fixo (100x100) e sem componentes de layout automático. Corrigido
via Editor (sem mudança de código — `BagUIManager.cs` já esperava essa
configuração, só faltava no Inspector):

- `ListaDeItensBag` ganhou **Vertical Layout Group** (Spacing 5–10, Child
  Alignment Upper Center, sem Force Expand) + **Content Size Fitter**
  (Horizontal/Vertical Fit = Preferred Size) — painel cresce sozinho
  conforme o número de itens.
- Prefab `BotaoItemTemplate`: largura aumentada de 160 para 260.
- Filho `Text (TMP)` do prefab: **Auto Size** ativado, Overflow ajustado —
  texto encolhe em vez de vazar em casos extremos.
- Scroll Rect adicionado no `PainelBag` pelo usuário, antecipando a lista
  crescer mais no futuro (decisão do usuário, não pedida por mim).

**Resultado confirmado por print do usuário:** lista aparece corretamente
separada, uma linha por item.

---

## 🎯 Próximos passos imediatos (curto prazo, em ordem sugerida)

1. Corrigir os 2 bugs abertos acima (motor de efeitos).
2. Configurar `goldMinimo`/`goldMaximo`/`tabelaDeDrops` nos `BattleManager`s existentes — sem isso a fase 2 do item está implementada mas nenhum inimigo droppa nada na prática.
3. Decidir e remover (ou usar) `TabelaDeCustos.CustoResistenciaPorNivel` — está declarada e não é referenciada em lugar nenhum.
4. Decidir se/quando implementar loja (usaria `GerenciadorDeEstado.TentarGastarGold`, já pronto e sem uso).

---

## 🗺️ Amadurecimento do jogo (o "quadro geral")

Isso é o tipo de checklist que qualquer RPG por turno pequeno costuma precisar pra
sair de "mecânica funcionando" para "jogo que dá vontade de jogar de novo". Não são
tarefas urgentes — são a resposta pra "depois que eu conserto o que está quebrado, o
que falta pro jogo ficar redondo":

### Sensação de jogo (o mais barato, maior retorno percebido)
- [ ] Feedback ao acertar/errar um ataque: um leve shake de câmera, flash no sprite
      atingido, ou um som de impacto — hoje o combate é só texto + slider
- [ ] Som ambiente e efeitos sonoros básicos (passo, ataque, vitória, derrota)
- [ ] Transição de tela entre mundo ↔ batalha (hoje é corte seco de cena)

### Conteúdo e variedade
- [ ] Mais de um tipo de inimigo com comportamento diferente (hoje só existe
      `InimigoFixo`, sempre a mesma ação) — agora cada um já pode ter sua própria
      `tabelaDeDrops`/Gold, só falta variar o comportamento em combate
- [ ] Curva de dificuldade: os inimigos escalam com o progresso do jogador, ou são
      todos fixos manualmente?
- [ ] Loja/NPC vendedor (usaria o Gold e `TentarGastarGold`, já existentes)
- [ ] Ícones e descrição longa nos itens (hoje só nome + stat resumido no texto do botão)

### Ensino/UX (crítico pro objetivo pedagógico do TCC)
- [ ] Onboarding do terminal: o jogador entende, na primeira vez que abre, o que
      pode digitar e por quê? Hoje existe um texto de ajuda estático — vale testar
      com alguém que nunca viu o jogo
- [ ] Feedback de erro do Lua: quando o Apollo Debugger (mencionado no readme) vai
      de fato existir em código, ou é só descrição de design ainda? Se só design,
      isso é uma lacuna grande entre o pitch pedagógico e o que o jogador realmente
      vê hoje no terminal

### Solidez técnica
- [ ] Testes automatizados mínimos pra regras de combate (fórmula de dano, custo de
      técnica, e agora efeito de item) — hoje qualquer mudança precisa ser validada
      manualmente jogando
- [ ] Múltiplos slots de save, ou pelo menos confirmação antes de sobrescrever o
      save existente (fica mais relevante agora que o Sinalizador de Retorno chama
      `Carregar()` — um save único significa "voltar" sempre pro mesmo ponto)
- [ ] Playtesting real dos valores de `TabelaDeCustos`/`TabelaDeCombate`/itens novos —
      todos os números hoje são placeholder, nenhum foi calibrado com gente jogando

Marque o que fizer sentido perseguir e ignore o resto — esta seção é um cardápio de
ideias, não uma exigência.

---

## 🚫 Fora do escopo por enquanto (não deixar a IA empurrar isso)

Itens já decididos como "não agora" (ver roadmap completo em
`DOCUMENTACAO_TECNICA.md` §8): Modo Puzzle do terminal, `DroidDataSO`/Factory,
Fase de herança real de peças (Cartuchos de Código sobrescrevendo `DroidBase`).
Se um agente sugerir atacar algum desses sem você ter puxado o assunto, é sinal de
que ele não leu esta seção — redirecione pra cá.

**Exceções (removidas desta lista, ver seções correspondentes):**
- Inventário/sistema de item saiu em 12/09/2026 — **fases 1 e 2 concluídas**
  (ver "Concluído" acima). Loja/compra continua fora de escopo por ora.
- **Sistema de acerto/erro (HIT/FLEE) saiu em 12/09/2026** — já implementado.
- **Equipável deu bônus real de atributo** (Estágio 3) — a peça em si ainda
  não tem comportamento de código (isso continua fora de escopo, é a "Fase
  de herança real" acima).
