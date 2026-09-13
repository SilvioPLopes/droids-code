# Droids Code — Checklist e Roteiro de Desenvolvimento

> **Este documento faz parte de um trio tratado como uma única fonte de
> verdade** — junto com `readme.md` e `DOCUMENTACAO_TECNICA.md`. Marcar algo
> como concluído aqui **exige** checar se `DOCUMENTACAO_TECNICA.md` (mapa de
> classes/árvore de pastas) e `readme.md` (status/roadmap) também precisam de
> atualização na mesma sessão — ver a regra completa e a tabela de
> correspondências em `DOCUMENTACAO_TECNICA.md` §0.2.

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

## ✅ Concluído nesta sessão (12/09/2026 — Estágio 1: Atributos Derivados)

Decisão de design fechada e implementada, revertendo a marcação anterior de
HIT/FLEE como "fora de escopo" (ver seção "🚫 Fora do escopo", atualizada):

- **HpMax virou calculado**, não mais campo solto: `Droid.HpMaxBase` (setado
  no construtor / save) + bônus de VIT (`+1% do HpMaxBase por ponto de VIT,
  arredondado pra baixo`) + espaço reservado pra bônus fixo de peças (Fase 2,
  hoje sempre 0). `TerminalDroidApi.AplicarUpgrade` recalcula o HP atual
  (`Droid.RecalcularHpAposMudancaDeVit`) sempre que VIT sobe, dando a
  diferença de teto sem "dever" cura.
- **VIT**: mantém Defesa (sem mudança) **e** agora também aumenta HpMax (%).
- **AGI → FLEE**, **DEX → HIT**: `IParticipanteDeCombate` ganhou `Hit`/`Flee`.
  Chance de acerto = `50 + (HIT atacante − FLEE alvo)`, clamp 5%–95% (ver
  `TabelaDeCombate`). Rolado em `Droid.ExecutarAcao`/`InimigoFixo.ExecutarAcao`
  antes do cálculo de dano; se errar, retorna `Sucesso=false` com mensagem de
  esquiva (sem novo campo em `ResultadoAcao`, mantendo a seção 3 congelada).
- **LUK → crítico**: `1 LUK = 1%` de chance, dobra o dano. Rolado
  separadamente do acerto (crítico não "salva" um ataque que errou).
- **INT → resistência a efeito** (`1 INT = 1%` de chance de resistir
  Stun/Veneno, rolado por efeito individual ao ser aplicado) **+ bônus de
  cura de item** (`1 INT = 1%` a mais de HP curado, `BattleManager.UsarItem`).
  Reaproveita a decisão de resistência; `TabelaDeCustos.CustoResistenciaPorLuk`
  segue não referenciada (não confundir com este cálculo, que é % direto por
  INT, sem custo em pontos).
- **InimigoFixo** ganhou `Hit`/`Flee`/`ChanceCritica`/`ChanceDeResistirEfeito`
  fixos (`TabelaDeCombate.*PadraoInimigo`) só para respeitar os mesmos
  cálculos do jogador — valores placeholder, sem gameplay real ainda (ver
  "Mais de um tipo de inimigo" abaixo, continua pendente).
- **Save (`DadosDoJogo`) versão 2**: `hpMax` → `hpMaxBase`. Compatibilidade
  com saves v1 tratada em `SalvamentoJson.Carregar` (se vier 0, mantém o
  HpMaxBase padrão do Droid em vez de zerar).

**Pendências abertas geradas por este trabalho** (não implementadas ainda):
- Bônus fixo de HP por peça (`Droid.ObterBonusDePecasHpFixo`) é placeholder
  retornando 0 — depende da Fase 2 de peças, fora de escopo.
- Nenhum teste automatizado cobrindo os novos rolls (ver "Testes automatizados
  mínimos" na seção de amadurecimento, abaixo).

---

## 🔴 Bugs abertos (fazer primeiro, nesta ordem)

Detalhe técnico em `DOCUMENTACAO_TECNICA.md` §9.

1. [ ] **Efeitos ativos do Droid nunca são zerados entre batalhas.** Sem efeito visível hoje (só inimigos com técnica poderiam causar isso, e não existem ainda), mas é risco pra quando houver inimigo especial.
2. [ ] **Empilhamento sem limite de Veneno/Stun.** Envenenar o mesmo alvo várias vezes soma o dano por turno de todas as instâncias; Stun repetido não estende a duração de fato. **Decisão de comportamento ainda pendente** (cap de instâncias? renovar duração em vez de somar?) — não implementar sem definir isso primeiro.

---

## ✅ Concluído nesta sessão (13/09/2026 — Estágio 2: Sistema de Item, fase 1/2)

Primeira fatia do "🚨 Mecânica essencial faltante" abaixo — cobre **só itens
de Cura**, com quantidade real e persistência. Buff/Debuff/Equipável/Fora-de-
batalha (exceto Bag, que já usa o sistema novo) ficam para a fase 2.

- **`CatalogoDeItens.cs` (novo)**: substitui `ItemConsumivel.cs`/
  `ItensDeBatalha`. `DefinicaoDeItem` = dado estático (id, nome, cura, tipo,
  usável em/fora de batalha); `TipoDeItem` enum com só `Cura` por ora — os
  outros tipos (Buff, Debuff, Equipável, ForaDeBatalha) ficam comentados no
  enum como lembrete, não implementados. IDs são estáveis (`pocao_pequena`
  etc.) — nunca renomear um Id existente, isso quebraria saves.
- **Inventário real** em `GerenciadorDeEstado` (`Dictionary<string,int>`,
  id → quantidade): `AdicionarItem`, `TentarRemoverItem` (falha se não tiver
  estoque), `ObterQuantidadeDeItem`. Droid novo começa com kit inicial (3
  Poção Pequena + 1 Poção Média) — placeholder, não há drop/loja ainda.
- **Persistência**: `DadosDoJogo` v3 ganhou `List<ItemSalvo>` (DTO plano,
  mesmo padrão de `TecnicaSalva`). Saves v1/v2 carregam inventário vazio
  (não quebra o load).
- **`BattleManager.AbrirListaDeItens`**: agora lê do inventário real, mostra
  quantidade (`Poção Pequena (+20 HP) x3`), e só lista itens com estoque > 0.
  `UsarItem` decrementa o estoque e **salva o jogo na hora** (decisão: evita
  que o jogador "perca de graça" o consumo se o jogo fechar no meio da
  batalha — o efeito já foi aplicado, o gasto precisa ficar gravado).
- **`BagUIManager.cs` (novo)** + botão "Bag" do `MenuMundoManager` real: cura
  fora de batalha, sem gastar turno (não existe turno fora de combate), mas
  ainda consome estoque e salva. Segue o mesmo padrão de painel + callback
  `AoFechar` que `TelaDeStatusManager`/`TerminalUIManager` já usam.

**Trabalho de Editor necessário para isso funcionar** (ver seção
"🔧 Trabalho de Editor pendente" abaixo): criar o Prefab `PainelBag` (mesma
estrutura do painel de itens da batalha) e arrastar as referências no
`MenuMundoManager`.

**Pendências abertas geradas por este trabalho:**
- Nenhuma forma de OBTER item ainda (drop de inimigo / loja) — só o kit
  inicial fixo. Continua bloqueante de lançamento (ver seção abaixo).
- Categorias Buff/Debuff/Equipável/uso-fora-de-batalha (Sinalizador, Kit de
  Acampamento, Chave de Acesso) ainda não têm `DefinicaoDeItem` nem lógica —
  ficam para a fase 2 do sistema de item.
- Sem limite de slots de inventário (lista livre) — decisão implícita desta
  fase, não revisitada.

---

## 🚨 Mecânica essencial faltante (bloqueante de lançamento)

> Diferente da seção de bugs acima, isso não é "código quebrado" — é sistema que
> ainda não existe. Levantado em 12/09/2026, a partir de revisão do estado atual
> do jogo. Mantido separado dos bugs porque exige decisão de design antes de
> qualquer código (ver "Decisão de design pendente" no fim desta seção).

### Sistema de Item / Inventário real

**Estado atual do código (atualizado 13/09/2026 — fase 1 concluída, ver seção
"✅ Concluído" no topo):** `CatalogoDeItens.cs` define itens de Cura com
inventário real persistido. O que falta pra fase 2:

- **Sem forma de obter item além do kit inicial.** Não existe drop de
  inimigo derrotado, não existe compra (não há loja/NPC vendedor), não
  existe achar item no mapa.
- **Sem diferenciação de tipo de item além de Cura.** Falta: item de efeito
  (buff/debuff, cura de status como Veneno/Stun), item equipável, item
  utilizável fora de batalha que não seja cura (Sinalizador de Retorno, Kit
  de Acampamento, Chave de Acesso).
- **UI de inventário é mínima** — mostra nome + cura + quantidade, sem ícone,
  sem descrição longa, sem categorização visual.

**O que falta, no mínimo, pra fase 2:**
1. **Pelo menos uma forma de obter item** — drop de inimigo derrotado e/ou
   compra em algum ponto do mundo (loja/NPC).
2. **Categorias Buff/Debuff/Equipável/ForaDeBatalha** em `TipoDeItem` — hoje
   só `Cura` existe; os outros valores foram deixados como comentário no
   enum como lembrete.
3. **UI mais rica** — ícone, descrição, filtro por categoria.

**Decisão de design pendente (não implementar sem fechar isso primeiro):**
drop, compra, ou os dois? Existe limite de slots de inventário ou é lista
livre? Itens equipáveis entram nesta mecânica ou ficam 100% dentro da tela de
peças do Droid (design ainda mais amplo, fora desta seção)?

---

## 🔧 Trabalho de Editor pendente (não é código, é configuração no Inspector)

- [ ] `BattleManager`: `painelListaDeItens`/`prefabBotaoItem` — apontado como "ainda mal otimizado" (12/09/2026); análise adiada para sessão futura, não mexer nisso sem revisão dedicada.
- [ ] **NOVO (13/09/2026):** criar o Prefab `PainelBag` na cena Game (mesma estrutura do painel de itens da batalha: painel raiz + lista + prefab de botão + botão Voltar) e arrastar as referências no `MenuMundoManager` (`bag`, e dentro do `BagUIManager`: `painel`, `painelListaDeItens`, `prefabBotaoItem`, `botaoVoltar`, `textoMensagem`). Sem isso o botão "Bag" cai no fallback de aviso "não implementado".

---

## 🎯 Próximos passos imediatos (curto prazo, em ordem sugerida)

1. Corrigir os bugs abertos acima.
2. Decidir e remover (ou usar) `TabelaDeCustos.CustoResistenciaPorNivel` — está
   declarada e não é referenciada em lugar nenhum.

> ✅ Concluído nesta sessão (13/09/2026): botão "Voltar" adicionado aos painéis
> de Ataque e Item da batalha (reaproveita o mesmo prefab de botão, só fecha o
> painel sem executar ação), ESC fecha o painel de Ataque/Item que estiver
> aberto, correção de um bug encontrado durante essa mudança (abrir Ataque e
> depois Item, sem fechar o primeiro, deixava os dois painéis abertos ao mesmo
> tempo — agora cada painel fecha o outro antes de abrir), e exclusividade
> entre os painéis de Status/Terminal e o Menu do Mundo (callback `AoFechar`
> em `TelaDeStatusManager`/`TerminalUIManager`, usado por `MenuMundoManager`
> pra esconder/reexibir seu próprio painel — com correção de um segundo bug:
> apertar ESC com o Terminal aberto reabria o Menu do Mundo por baixo dele).

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
      `InimigoFixo`, sempre a mesma ação)
- [ ] Curva de dificuldade: os inimigos escalam com o progresso do jogador, ou são
      todos fixos manualmente?

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
      técnica) — hoje qualquer mudança precisa ser validada manualmente jogando
- [ ] Múltiplos slots de save, ou pelo menos confirmação antes de sobrescrever o
      save existente
- [ ] Playtesting real dos valores de `TabelaDeCustos`/`TabelaDeCombate` — todos os
      números hoje são placeholder, nenhum foi calibrado com gente jogando

Marque o que fizer sentido perseguir e ignore o resto — esta seção é um cardápio de
ideias, não uma exigência.

---

## 🚫 Fora do escopo por enquanto (não deixar a IA empurrar isso)

Itens já decididos como "não agora" (ver roadmap completo em
`DOCUMENTACAO_TECNICA.md` §8): Modo Puzzle do terminal, `DroidDataSO`/Factory,
Fase 2 de peças (herança real).
Se um agente sugerir atacar algum desses sem você ter puxado o assunto, é sinal de
que ele não leu esta seção — redirecione pra cá.

**Exceções (removidas desta lista, ver seções correspondentes):**
- Inventário/sistema de item saiu em 12/09/2026 — ver "🚨 Mecânica essencial
  faltante", agora bloqueante de lançamento.
- **Sistema de acerto/erro (HIT/FLEE) saiu em 12/09/2026** — decisão explícita
  de reverter o "fora de escopo" anterior, ver "✅ Concluído nesta sessão
  (Estágio 1: Atributos Derivados)" no topo deste documento. Já implementado.
