# Droids Code — Checklist e Roteiro de Desenvolvimento

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

## 🔴 Bugs abertos (fazer primeiro, nesta ordem)

Levantados numa varredura dedicada em 12/09/2026, após o motor de efeitos (Veneno/Stun) entrar em combate. Detalhe técnico de cada um em `DOCUMENTACAO_TECNICA.md` §4.2/§4.3/§9.

1. [ ] **Vitória/derrota não detectada na hora quando dano por turno mata quem ia agir.** `BattleManager.TurnoDoInimigo()` só checa `VerificarDerrota(droid)`; `ExecutarAtaqueDoJogador()` só checa `VerificarDerrota(inimigo)`. Se o veneno matar o próprio atacante da vez no início do turno dele, a batalha só percebe no turno seguinte.
2. [ ] **Salvar/Carregar descarta Veneno/Stun de uma técnica.** `TecnicaSalva` só grava nome/nível de dano — os efeitos da técnica somem ao carregar o save.
3. [ ] **Ataque Básico pode ser sobrescrito.** `AprenderTecnica`/`AprenderTecnicaComVeneno`/`AprenderTecnicaComStun` não bloqueiam o nome `Droid.NomeAtaqueBasico` (só `EsquecerTecnica` bloqueia).
4. [ ] **Reaprender uma técnica existente não reembolsa a versão antiga.** `TentarAprender` cobra o custo total de novo sem checar se o nome já estava configurado — os pontos da versão anterior somem.
5. [ ] **Terminal — seta ↓ do histórico sem proteção.** Diferente da seta ↑ (só navega se o campo estiver vazio), ↓ troca o texto do campo mesmo no meio de uma digitação nova.
6. [ ] **Terminal — não dá pra sair do histórico de volta pro campo vazio só com ↓.** `NavegarHistorico` trava no último comando do histórico como teto.
7. [ ] **Efeitos ativos do Droid nunca são zerados entre batalhas.** Sem efeito visível hoje (só inimigos com técnica poderiam causar isso, e não existem ainda), mas é risco pra quando houver inimigo especial.
8. [ ] **Empilhamento sem limite de Veneno/Stun.** Envenenar o mesmo alvo várias vezes soma o dano por turno de todas as instâncias; Stun repetido não estende a duração de fato. Confirmar se é intencional antes de decidir se é bug.

---

## 🔧 Trabalho de Editor pendente (não é código, é configuração no Inspector)

- [ ] `BattleManager`: criar `painelListaDeItens` (painel vazio, inativo) e
      `prefabBotaoItem` (pode reutilizar o mesmo prefab de `prefabBotaoAtaque`),
      arrastar as referências no Inspector — sem isso o botão "Item" cai no
      fallback (usa o primeiro item direto, sem mostrar a lista).

---

## 🎯 Próximos passos imediatos (curto prazo, em ordem sugerida)

1. Corrigir os bugs abertos acima.
2. Decidir e remover (ou usar) `TabelaDeCustos.CustoResistenciaPorNivel` — está
   declarada e não é referenciada em lugar nenhum.

> ✅ Concluído nesta sessão (12/09/2026): visor de experiência/XP na Tela de Status,
> e aplicação de fato dos efeitos (Stun/Envenenamento) de `TecnicaComposta` em combate.

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
`DOCUMENTACAO_TECNICA.md` §8): sistema de acerto/erro (HIT/FLEE), Modo Puzzle do
terminal, `DroidDataSO`/Factory, Fase 2 de peças (herança real), inventário completo.
Se um agente sugerir atacar algum desses sem você ter puxado o assunto, é sinal de
que ele não leu esta seção — redirecione pra cá.
