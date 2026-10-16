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
> ⚠️ **Nota desta entrega (15/09/2026):** entrou o **sistema de história do
> Ato 1** (7 scripts novos + 6 patches) e o **pacote de catálogos de
> conteúdo** (9 de 10 arquivos confirmados por `.cs` real — ver seção
> própria abaixo). O trio foi conferido contra os `.cs` reais nas duas
> levas. Ver `GUIA_DE_MONTAGEM_ATO1.md` para o trabalho de Editor que falta.

---

## ✅ Reconciliado: pacote "catálogos de conteúdo" do Ato 1 (15/09/2026)

**Atualização da pendência anterior.** Dos 10 arquivos descritos em
`LEIA_ISTO_OS_10_ARQUIVOS_NOVOS.md`, **9 tiveram o `.cs` real anexado e
conferido nesta sessão**: `CatalogoDeItens` (reescrito), `CatalogoDeInimigos`,
`CatalogoDeQuests`, `CatalogoDeNpcs`, `CatalogoDeDialogos`, `CatalogoDePuzzles`,
`CatalogoDeLicoes`, `GerenciadorDeQuests`, `QuestUIManager`, e
`VerificadorDePuzzle` v2. Ver `DOCUMENTACAO_TECNICA.md` §8.1 para o
detalhamento completo do que foi confirmado em cada arquivo.

**Ainda em aberto (2 itens, não bloqueiam mais o restante do plano):**

1. **`GatilhoDeQuest.cs` não foi anexado.** É o componente de NPC que
   ofereceria/lembraria/entregaria quest ao interagir. Sem ele, o fluxo de
   "falar com NPC → pegar quest" provavelmente ainda depende de ligação
   manual (ex: chamar `GerenciadorDeQuests.Aceitar`/`Entregar` a partir de
   um `GatilhoDeDialogo` existente) até o arquivo ser colado ou escrito.
2. **`TerminalDroidApi.cs` com `Explicar`/`Ajuda`/`Resumo` não foi anexado.**
   `CatalogoDeLicoes.cs` está pronto e confirmado, mas a ponta que o
   exporia no terminal (`droid.explicar("while")`) segue sem confirmação —
   o arquivo `TerminalDroidApi.cs` em si nunca apareceu nesta sessão nem na
   anterior, só é referenciado por `TerminalUIManager`/`DroidScriptRunner`.

**🔴 Bug de integração novo, encontrado por leitura cruzada dos arquivos
recebidos (adicionar à seção de bugs abertos abaixo):** `VerificadorDePuzzle.cs`
v2 chama `terminal.EscreverNoLog(...)` quando não há `textoDeApollo`
configurado no Inspector. `TerminalUIManager.cs` **não tem esse método** —
só existe `AdicionarLinhaDeLog`, que é `private`. Qualquer puzzle montado
sem TMP de fala dedicado vai quebrar a compilação nesse ponto. Correção
sugerida: tornar `AdicionarLinhaDeLog` público, ou adicionar
`public void EscreverNoLog(string linha) => AdicionarLinhaDeLog(linha);`
em `TerminalUIManager`.

**Consequência prática:** o plano de implementação futuro pode tratar os 9
itens confirmados como "código pronto, falta Editor" (mesmo status do
sistema de história) — não mais como pendência de verificação. Os 2 itens
em aberto (`GatilhoDeQuest`, ponta do `TerminalDroidApi`) e o bug de
integração entram como itens normais de roadmap/bug, ver seções abaixo.

---

## ✅ Sistema de história do Ato 1 (código pronto, Editor pendente)

7 scripts novos em `Assets/Scripts/Historia/` + `Combat/CatalogoDeInimigos.cs`,
todos descritos em `DOCUMENTACAO_TECNICA.md` §4.8. Nenhum sistema de domínio
novo: tudo se apoia nas flags de história que já existiam no
`GerenciadorDeEstado`, já persistiam no save, e **não eram usadas por nada**.

- `DialogoUIManager` + `GatilhoDeDialogo` — falas e escolhas ramificadas por Inspector. Fecha o item "Diálogo simples de NPC genérico" que estava pulado.
- `CondicaoDeHistoria` — liga/desliga objetos por flag (Rasha e Teodoro só existem depois da Sessão 3, Dara troca de fala no retorno).
- `GatilhoDeBatalha` + `CatalogoDeInimigos` — batalha roteirizada com inimigo de catálogo. **Uma única cena `Battle`** serve todos os inimigos; some a necessidade de duplicar cena e de preencher `tabelaDeDrops` à mão em cada uma.
- `VerificadorDePuzzle` — **v2 (confirmado 15/09/2026):** lê o puzzle pelo `Id` no `CatalogoDePuzzles`, suporta múltiplas variáveis simultâneas, conta tentativas e mostra dica do catálogo após N erros, registra progresso de quest. Mantém o modo manual da v1 como fallback se `idDoPuzzle` ficar vazio — o `PuzzleAguaCasaChecker` original **não foi apagado**; os dois coexistem até a `CasaLip` ser migrada no Editor. **⚠️ Bug de integração:** chama `terminal.EscreverNoLog(...)`, método que não existe em `TerminalUIManager` — ver "🔴 Bugs abertos" abaixo.
- `TrancaPorHistoria` — portão de cena por flag **e por técnica configurada no terminal**. É o que torna o terminal caminho crítico: sem escrever código, o jogador não sai de Ferrovale.
- `ApolloDica` — tradução conceitual de erro do MoonSharp na voz do Apollo, ligada no `TerminalUIManager`.

**Patches:** `BattleManager` (inimigo de catálogo + flag de vitória + guarda de ESC), `NpcInterativo` (tipo `Dialogo`, e fala antes de abrir a Loja), `TerminalUIManager` (Apollo Dica), `EncounterZone` (inimigo por zona + condição de flag), `MenuMundoManager` (`cenasBloqueadas`), `GerenciadorDeEstado` (`ProximoInimigoId`/`FlagDeVitoriaPendente`/`LimparBatalhaPendente`).

**⚠️ Bug encontrado e corrigido junto:** desde que o `MenuMundoManager` ganhou
`DontDestroyOnLoad`, o Canvas do menu passou a existir **dentro da cena
`Battle`** — onde o `BattleManager` também escuta ESC. Um único ESC era
consumido pelos dois (fechava a lista de ataques *e* abria o menu de pausa por
cima da batalha). O comentário no próprio `BattleManager` previa esse dia
("confirmar se algum dia as duas rodarem sobrepostas"). Corrigido dos dois lados.

**Pendente (só Editor):** montar as cenas conforme `GUIA_DE_MONTAGEM_ATO1.md`, e colar os 2 itens de facção conforme `PARA_COLAR_CatalogoDeItens.md`.

---

## ✅ Catálogos de conteúdo do Ato 1 — Itens/Quests/NPCs/Diálogos/Puzzles/Lições (confirmado 15/09/2026)

9 dos 10 arquivos do pacote descrito em `LEIA_ISTO_OS_10_ARQUIVOS_NOVOS.md`
tiveram o `.cs` real conferido nesta sessão (ver `DOCUMENTACAO_TECNICA.md`
§8.1 para o detalhamento por arquivo). Resumo do que passou a existir:

- **`CatalogoDeItens.cs` reescrito:** 6ª categoria `Chave` (itens de missão, nunca usáveis/vendáveis), `Descricao` + `Raridade` em todo item, `VendavelNaLoja` (filtra a aba Comprar — sem isso os 2 itens de facção e os itens de quest apareceriam à venda), preços com escala real (8–150) e spread comprador/vendedor de 50% (`PrecoDeVendaReal`).
- **`CatalogoDeQuests.cs`:** 10 missões (2 principais com `AceitaAutomaticamente`, 4 de Guilda, 4 secundárias), objetivos tipados referenciando `CatalogoDeNpcs`/`CatalogoDeInimigos`/`CatalogoDePuzzles` por Id — integração cruzada real entre os catálogos, não só descrita em texto.
- **`CatalogoDeNpcs.cs`:** 13 NPCs (7 do Enredo + 6 novos), papel/local/descrição.
- **`CatalogoDeDialogos.cs`:** roteiro cobrindo Casa (Sessão 1) + City/Game das Sessões 2 a 7.
- **`CatalogoDePuzzles.cs`:** 10 puzzles por Id, incluindo os 2 já usados na matriz curricular do `readme.md` e mais 8 opcionais.
- **`CatalogoDeLicoes.cs`:** 9 conceitos pedagógicos (`variavel`/`while`/`if`/`comparacao`/`contador`/`funcao`/`erro`/`atributo`/`tecnica`) com normalização de apelido/acento.
- **`GerenciadorDeQuests.cs`:** motor estático — aceitar, registrar progresso automático, checar entrega, entregar (paga XP/Gold/item, grava flags). Progresso 100% em flags/contadores já persistidos.
- **`QuestUIManager.cs`:** diário de missões, mesmo contrato dos outros painéis do jogo.

**Ainda em aberto:**
- **`GatilhoDeQuest.cs`** — não anexado. Componente de NPC que ofereceria/entregaria quest ao interagir.
- **Ponta do `TerminalDroidApi`** (`Explicar`/`Ajuda`/`Resumo`) — não anexado. `CatalogoDeLicoes` está pronto, mas `droid.explicar("while")` não pode ser confirmado como funcional no terminal até esse arquivo ser colado.
- **Divergência de versão em `VerificadorDePuzzle`** já registrada acima e em `DOCUMENTACAO_TECNICA.md` §4.8/§8.1 — a v2 (por Id) é a vigente, mas o corpo da §4.8 ainda descreve a v1.

**Pendente (só Editor):** ligar `QuestUIManager` ao botão "Missões" do `MenuMundoManager` (ainda não patcheado — `MenuMundoManager.cs` conferido nesta sessão não tem botão/campo de Quest), montar os NPCs de `CatalogoDeNpcs` na cena `City`, e configurar os `VerificadorDePuzzle` (v2) das cenas com o `idDoPuzzle` correspondente.

---

## ✅ Cena `CasaLip` — puzzle de introdução (água) + Camera Follow

- **Cena nova `CasaLip`** (Sessão 1 do Enredo: `int nivel_agua = 0; while (nivel_agua < 100) { ... }`), primeiro contato do jogador com o terminal, antes da cena `Game`.
- **`Casa/InteragirComTerminalCasa.cs`** (novo): trigger de interação (tecla `E`) que abre o `TerminalUIManager` geral — não é um terminal próprio da casa, é o mesmo do resto do jogo. Não depende de `NpcInterativo` de propósito (decisão deliberada, ver `DOCUMENTACAO_TECNICA.md` §4.7).
- **`Casa/PuzzleAguaCasaChecker.cs`** (novo): mora no mesmo GameObject do `TerminalUIManager` da casa. Ouve `TerminalUIManager.AoExecutarCodigo` (evento novo), lê `nivel_agua` via `DroidScriptRunner.ObterVariavelNumerica` (método novo), e ao atingir 100 destranca a porta de saída + mostra fala do Apollo.
- **`TerminalUIManager.cs`**: ganhou `AoExecutarCodigo` (Action, dispara sempre ao final de cada execução) e `Runner` (getter só-leitura do `DroidScriptRunner`), pra permitir esse tipo de checker sem duplicar a UI do terminal.
- **`DroidScriptRunner.cs`**: ganhou `ObterVariavelNumerica(nome)`, leitura pura de variável global Lua após execução (retorna `null` se não existir/não for número).
- **✅ Testado (14/09/2026):** puzzle testado ponta a ponta pelo responsável do projeto, porta destranca corretamente ao `nivel_agua` chegar em 100.
- **Decisão de arquitetura:** porta usa campo `trancada` (bool) em `TransicaoDeCena`, marcado no Inspector, nunca desabilita o componente inteiro — ver `DOCUMENTACAO_TECNICA.md` §4.7 pro motivo (componente desabilitado não recebe trigger nem roda `Awake()` se nascer inativo).
- **`Camera/CameraFollow.cs`** (novo): segue o Lip suavemente (`Lerp`), com limites de mapa opcionais. Ainda não confirmado em qual(is) cena(s) está de fato anexado.

**✅ Pendência de limpeza resolvida (15/09/2026):** o `Debug.Log` temporário de `DroidScriptRunner.ObterVariavelNumerica` foi removido, junto dos `using` que só existiam por causa dele. Depuração de puzzle agora é o campo `logDeDepuracao` do `VerificadorDePuzzle`, ligável por instância no Inspector.

---

## ✅ Gold no Status + Menu persistente entre cenas

- **`TelaDeStatusManager.cs`**: novo campo opcional `textoGold`, mostra `Gold: {GerenciadorDeEstado.Instancia.Gold}` — Status nunca tinha sido atualizado quando o Gold foi criado.
- **`MenuMundoManager.cs`**: ganhou `Awake()` com o mesmo padrão singleton + `DontDestroyOnLoad` de `GerenciadorDeEstado`. Antes, o Canvas do menu (e todos os painéis Status/Terminal/Bag/Droid) só existia na cena onde foi colocado (`Game`) — ESC na Cidade não abria nada. Agora o Canvas persiste entre cenas; **trabalho de Editor:** remover qualquer Canvas/Menu duplicado colocado manualmente na cena `City`, só deve existir um.

---

## 🔴 Bugs abertos (fazer primeiro, nesta ordem)

1. [ ] **`VerificadorDePuzzle` v2 chama método inexistente em `TerminalUIManager`.** `terminal.EscreverNoLog(...)` é chamado quando não há `textoDeApollo` configurado, mas `TerminalUIManager` só tem `AdicionarLinhaDeLog` (privado). Quebra a compilação para qualquer puzzle sem TMP de fala dedicado. **Confirmado por leitura cruzada dos dois arquivos nesta sessão (15/09/2026).** Correção: tornar `AdicionarLinhaDeLog` público, ou adicionar um `public void EscreverNoLog(string linha) => AdicionarLinhaDeLog(linha);` em `TerminalUIManager`.
2. [ ] **Efeitos ativos do Droid nunca são zerados entre batalhas.** Sem efeito visível hoje, mas é risco pra inimigo especial ou uso repetido de Debuff.
3. [ ] **Empilhamento sem limite de Veneno/Stun/Corrosivo/Disruptor EMP.** Aplicar o mesmo efeito várias vezes soma o dano por turno de todas as instâncias; Stun repetido não estende a duração. **Confirmado em teste real** (Disruptor EMP 2x = 2 instâncias, não renova). **Decisão de comportamento pendente de resposta** — opções em aberto: renovar duração (sem empilhar), permitir empilhar até um cap, ou bloquear reaplicação enquanto ativo. Não implementar sem fechar isso primeiro.

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
2. **Corrigir o bug de integração `EscreverNoLog`** (`VerificadorDePuzzle` v2 × `TerminalUIManager`) — bloqueante pra qualquer puzzle montado sem `textoDeApollo`, ou seja, bloqueante pros 8 puzzles opcionais do `CatalogoDePuzzles`.
3. Terminar o polish visual da Loja (Scroll Rect + fonte fixa) — já em andamento.
4. Confirmar que `BattleManager.tabelaDeDrops` sobreviveu ao trabalho da Loja, e configurar nos `BattleManager`s que faltarem — sem isso o jogo normal não gera Gold fora da Loja.
5. Balancear `Preço` por item — o `CatalogoDeItens` reescrito já tem escala (8–150) e spread de venda, confirmados nesta sessão; falta só validar por playtesting real.
6. Decidir e remover (ou usar) `TabelaDeCustos.CustoResistenciaPorNivel` — declarada e nunca referenciada.
7. ✅ **Feito (15/09/2026):** Diálogo simples de NPC genérico deixou de ser "pulado" — coberto pelo sistema de história (`GatilhoDeDialogo`/`NpcInterativo` tipo `Dialogo`), confirmado por `.cs` real.
8. ✅ **Feito (15/09/2026):** `Debug.Log` temporário de `DroidScriptRunner.ObterVariavelNumerica` removido.
9. **Montar as cenas do Ato 1 no Editor** — o código confirmado está pronto; ver `GUIA_DE_MONTAGEM_ATO1.md`. Esta é a maior alavanca de resultado por esforço no projeto hoje.
10. ✅ **Feito (15/09/2026):** os 2 itens de facção (`Núcleo de Sobrecarga`, `Módulo de Ressonância`) já estão no `CatalogoDeItens.cs` reescrito, confirmado por `.cs` real — o item antigo "colar via `PARA_COLAR_CatalogoDeItens.md`" está superado.
11. **Ligar `QuestUIManager` ao Menu do Mundo** — botão "Missões" ainda não existe em `MenuMundoManager.cs` (confirmado por `.cs` real nesta sessão). Mesmo padrão de `botaoBag`/`botaoDroid`.
12. **Escrever ou colar `GatilhoDeQuest.cs`** e a ponta `Explicar`/`Ajuda`/`Resumo` do `TerminalDroidApi` — os 2 itens do pacote de conteúdo ainda sem `.cs` confirmado (ver `DOCUMENTACAO_TECNICA.md` §8.1).
13. **Confirmar HIT/FLEE e crítico (LUK)** colando `Droid.cs` e `CombatEngine.cs` — é a última divergência aberta entre o `readme` e a `DOCUMENTACAO_TECNICA` §8, e trava o balanceamento dos inimigos do Ato 1.

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
- [x] ~~Dica conceitual em erro de compilação/exceção~~ — **feito (15/09/2026)**, ver `ApolloDica` em `DOCUMENTACAO_TECNICA.md` §4.8.
- [ ] **Apollo Debugger — o que ainda falta:** destacar a linha da falha *dentro* do campo de código (hoje a linha só aparece como texto no log) e reagir enquanto o jogador digita, não só depois de Executar. A lacuna entre o pitch do readme e o produto encolheu, mas não fechou — continuar dizendo "parcial", nunca "implementado".

### Solidez técnica
- [ ] Testes automatizados mínimos (dano, custo de técnica, efeito de item)
- [ ] Múltiplos slots de save / confirmação antes de sobrescrever
- [ ] Playtesting real dos valores de `TabelaDeCustos`/`TabelaDeCombate`/itens/preços — tudo hoje é placeholder

---

## 🚫 Fora do escopo por enquanto (não deixar a IA empurrar isso)

`DroidDataSO`/Factory, herança real de peça (Cartuchos de Código sobrescrevendo `DroidBase`), sistema de "desequipar".

**Modo Puzzle genérico saiu desta lista por outro motivo (15/09/2026):** não é
"não agora", é provavelmente **desnecessário**. O `VerificadorDePuzzle` já
resolve os puzzles previstos do Ato 1 com configuração de Inspector e zero
código por puzzle. Só reabrir se aparecer um puzzle que ele comprovadamente não
consiga avaliar. Se um agente sugerir atacar algum desses sem você ter puxado o assunto, redirecione pra cá.