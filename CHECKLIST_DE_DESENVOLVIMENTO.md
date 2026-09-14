# Droids Code — Checklist e Roteiro de Desenvolvimento

> **Este documento faz parte de um trio tratado como uma única fonte de
> verdade** — junto com `readme.md` e `DOCUMENTACAO_TECNICA.md`. Marcar algo
> como concluído aqui **exige** checar se os outros dois também precisam de
> atualização na mesma sessão — ver a regra completa em
> `DOCUMENTACAO_TECNICA.md` §0.2.
>
> **Regra de manutenção:** edite marcando itens como concluídos e movendo
> entre seções — não acumule histórico aqui. Se algo foi corrigido/decidido,
> resuma em 1-2 linhas; detalhe extenso fica pro commit, não pro documento.
>
> ⚠️ **Nota desta entrega:** este checklist reconcilia duas frentes que
> rodaram em paralelo sem uma saber da outra — Painel do Droid/Equipável com
> bônus real de atributo (um lado) e Cidade/Loja/Gold (outro lado). Histórico
> de cada frente foi condensado; nada foi descartado, só resumido.

---

## ✅ Gold no Status + Menu persistente entre cenas

- **`TelaDeStatusManager.cs`**: novo campo opcional `textoGold`, mostra `Gold: {GerenciadorDeEstado.Instancia.Gold}` — Status nunca tinha sido atualizado quando o Gold foi criado.
- **`MenuMundoManager.cs`**: ganhou `Awake()` com o mesmo padrão singleton + `DontDestroyOnLoad` de `GerenciadorDeEstado`. Antes, o Canvas do menu (e todos os painéis Status/Terminal/Bag/Droid) só existia na cena onde foi colocado (`Game`) — ESC na Cidade não abria nada. Agora o Canvas persiste entre cenas; **trabalho de Editor:** remover qualquer Canvas/Menu duplicado colocado manualmente na cena `City`, só deve existir um.

---

## 🔴 Bugs abertos (fazer primeiro, nesta ordem)

1. [ ] **Efeitos ativos do Droid nunca são zerados entre batalhas.** Sem efeito visível hoje, mas é risco pra inimigo especial ou uso repetido de Debuff.
2. [ ] **Empilhamento sem limite de Veneno/Stun/Corrosivo/Disruptor EMP.** Aplicar o mesmo efeito várias vezes soma o dano por turno de todas as instâncias; Stun repetido não estende a duração. **Confirmado em teste real** (Disruptor EMP 2x = 2 instâncias, não renova). **Decisão de comportamento pendente de resposta** — opções em aberto: renovar duração (sem empilhar), permitir empilhar até um cap, ou bloquear reaplicação enquanto ativo. Não implementar sem fechar isso primeiro.

---

## ✅ Painel do Droid (visualização de peças)

`TelaDeDroidManager.cs` (novo) + patch em `MenuMundoManager.cs`: botão "Droid" do Menu do Mundo abre painel somente-leitura com os 4 slots (Braço/Perna/Tronco/Cabeça), nome + bônus de atributo (ou "Vazio"). Código pronto, Prefab `PainelDroid` já configurado no Editor e testado pelo responsável do projeto. Só visualização — equipar continua pela Bag; sem "desequipar" nesta leva.

**Pendência:** testar se equipar a mesma peça 2x duplica o bônus, ou se `Droid.EquiparPeca` de fato substitui (comportamento esperado) — **já testado e confirmado: não duplica, substitui corretamente.**

---

## ✅ Sistema de Item completo (Cura/Buff/Debuff/Equipável/ForaDeBatalha) + Gold

Reconciliação: esse trabalho já existia em código antes de aparecer documentado — os `.md` não refletiam o estado real. Resumo do que está implementado:

- `CatalogoDeItens.cs`: `TipoDeItem` com as 5 categorias, cada uma com `DefinicaoDeItem` (id, nome, tipo, efeito, usável em/fora de batalha, e agora `Preco`).
- Buff/Debuff reaproveitam o motor de efeito de técnica (`EfeitoDeAtributo`/`EfeitoDeDanoPorTurno`) — por isso herdam os 2 bugs abertos acima.
- Equipável dá bônus real via `DroidPart.AtributoBonificado`/`ValorDoBonus`, somado em `Droid.ObterBonusDePecas`. Equipar consome o item, sem desequipar.
- Fora de Batalha: Sinalizador de Retorno (recarrega save), Kit de Acampamento (cura total + salva), Chave de Acesso (flag genérica).
- Inventário real e persistido (`GerenciadorDeEstado`, save v4), Bag funcional fora de batalha.
- `Gold`: `GerenciadorDeEstado.Gold` + `AdicionarGold`/`TentarGastarGold`, persistido. Fontes: kit inicial, drop configurável por `BattleManager` (`goldMinimo`/`goldMaximo`/`tabelaDeDrops`, preenchido manualmente por cena de batalha), e venda na Loja.

**Pendência aberta:** confirmar que `BattleManager.tabelaDeDrops` sobreviveu às mudanças recentes da Loja (foram mexidos em sessões diferentes, sem visibilidade cruzada) — sem isso, "Gold: 0" pode nunca sair do zero em jogo normal fora da Loja.

---

## ✅ Cidade, Loja (compra/venda) e NPC

- **Cena `City`** + `TransicaoDeCena` (mundo ↔ Cidade) — **corrigido nesta sessão:** campo de destino trocado de `Transform` pra `Vector2` (coordenadas), porque Unity não permite referência cross-scene de `Transform`.
- **`NpcInterativo.cs`**: enum `TipoDeNpc` (`Generico`/`Loja`) + campo `lojaUIManager`. `Generico` mantém placeholder (`Debug.Log`); `Loja` abre o painel e registra/libera "menu aberto" do mesmo jeito que Status/Terminal/Bag/Droid já fazem.
- **`LojaUIManager.cs`** (novo): painel com abas Comprar/Vender, mesmo contrato estrutural do `BagUIManager` (`AoFechar`, `LayoutRebuilder.ForceRebuildLayoutImmediate` antes de popular a lista). Comprar gasta Gold e soma item; Vender remove item e credita Gold.
- **`CatalogoDeItens.Preco`** (novo campo) + **`ItensDaLoja`** (expõe o catálogo inteiro como vendável) — todo item vale 1 Gold (`PrecoPadrao`), decisão deliberada, não balanceado.
- **Bugs de Editor já resolvidos ao vivo nesta sessão:** `CatalogoDeItens.cs` duplicado fora da pasta certa (corrigido); `lojaUIManager` do NPC não estava linkado (corrigido); `BotaoItemTemplate` aparecendo sozinho na tela — precisa ficar desativado, é só molde (corrigido); itens da lista sobrepostos — faltava Vertical Layout Group + Content Size Fitter (corrigido, lista já aparece certinha nos prints).
- **Em andamento:** adicionar `Scroll Rect` numa `ScrollView` nova na Loja, e fixar tamanho de fonte no template (hoje varia por item).

---

## 🔧 Trabalho de Editor pendente

- [ ] **Loja:** finalizar `Scroll Rect` + fixar fonte no template de botão (em andamento).
- [ ] **Battle:** o mesmo bug de sobreposição de botões (texto cortado/empilhado) que existia na Bag e já foi corrigido lá — confirmar se ainda falta nos painéis `painelListaDeAtaques`/`painelListaDeItens` dentro do `BattleManager`, cena Battle.
- [ ] Preencher `goldMinimo`/`goldMaximo`/`tabelaDeDrops` no Inspector de cada `BattleManager` que deveria dropar algo — hoje ficam zerados/vazios até serem configurados manualmente (ver pendência de sobrevivência ao merge, acima).

---

## 🎯 Próximos passos imediatos (curto prazo, em ordem sugerida)

1. Fechar a decisão de comportamento de empilhamento de efeito e corrigir os 2 bugs abertos (motor de efeitos) — bloqueante pra qualquer Debuff/técnica de status novo.
2. Terminar o polish visual da Loja (Scroll Rect + fonte fixa) — já em andamento.
3. Confirmar que `BattleManager.tabelaDeDrops` sobreviveu ao trabalho da Loja, e configurar nos `BattleManager`s que faltarem — sem isso o jogo normal não gera Gold fora da Loja.
4. Balancear `Preço` por item (hoje todos valem 1 Gold) — só depois do passo 3, senão não há como testar significado de preço.
5. Decidir e remover (ou usar) `TabelaDeCustos.CustoResistenciaPorNivel` — declarada e nunca referenciada.
6. Decidir se/quando retomar o Diálogo simples de NPC genérico (pulado por decisão do responsável, não descartado).

---

## 🗺️ Amadurecimento do jogo (o "quadro geral")

Não são tarefas urgentes — cardápio de ideias pra depois que o essencial estiver redondo:

### Sensação de jogo
- [ ] Feedback ao acertar/errar (shake de câmera, flash, som de impacto)
- [ ] Som ambiente e efeitos sonoros básicos
- [ ] Transição de tela mundo ↔ batalha (hoje é corte seco)

### Conteúdo e variedade
- [ ] Mais de um tipo de inimigo com comportamento diferente (hoje só `InimigoFixo`) — cada um já pode ter sua própria `tabelaDeDrops`/Gold, falta variar o comportamento
- [ ] Curva de dificuldade dos inimigos
- [ ] Ícones e descrição longa nos itens/Loja (hoje só nome + stat resumido)

### Ensino/UX (crítico pro objetivo pedagógico do TCC)
- [ ] Onboarding do terminal — testar com alguém que nunca viu o jogo
- [ ] Apollo Debugger: existe em código de fato, ou é só descrição de design ainda? Se só design, é lacuna entre o pitch pedagógico e o que o jogador vê hoje

### Solidez técnica
- [ ] Testes automatizados mínimos (dano, custo de técnica, efeito de item)
- [ ] Múltiplos slots de save / confirmação antes de sobrescrever
- [ ] Playtesting real dos valores de `TabelaDeCustos`/`TabelaDeCombate`/itens/preços — tudo hoje é placeholder

---

## 🚫 Fora do escopo por enquanto (não deixar a IA empurrar isso)

Modo Puzzle do terminal, `DroidDataSO`/Factory, herança real de peça (Cartuchos de Código sobrescrevendo `DroidBase`), sistema de "desequipar". Se um agente sugerir atacar algum desses sem você ter puxado o assunto, redirecione pra cá.
