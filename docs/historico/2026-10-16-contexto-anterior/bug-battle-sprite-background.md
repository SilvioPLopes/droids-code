# Bug: `battle_0` (background) não renderiza em `Battle.unity`

**STATUS: contornado (workaround aplicado e funcional). Causa raiz NÃO identificada — ver seção "Solução aplicada" e hipóteses 18/19 em aberto.**

**Sintoma:** no Game View, durante o Play, só aparecem a cor de clear da câmera (azul) e os elementos de UI (Canvas Screen Space Overlay). O sprite de fundo `battle_0` nunca aparece, sem nenhum erro no Console.

**Cena:** `Battle.unity` — objeto `battle_0`, `SpriteRenderer`, material `Sprite-Lit-Default`, sprite fonte `battle.jpg`.

---

## Hipóteses descartadas (testadas com evidência)

### ❌ 1. Sprite fora do campo de visão da câmera (posição/escala erradas)
- **Teste:** posição e escala do `battle_0` conferidas no Inspector parado e em Play pausado.
- **Evidência:** `Position (0.85, 0.96)` / `Scale (1.5, 1.5)` idênticos nos dois momentos (Imagem 1 vs Imagem 2). Cálculo de overlap câmera×sprite mostra interseção real (~15-20% da tela).
- **Conclusão:** não é script movendo o objeto em runtime, e ele está dentro do frustum.
- **Atualização:** pivot do sprite (`battle_jpg.meta`) estava em `{x:0, y:0}` (canto inferior esquerdo) em vez de centralizado — bug real, corrigido para Center. Mesmo assim, com Position `(0,0,0)` e pivot corrigido, o bug persistiu. Confirma que pivot/posição não é a causa raiz (era um problema real, mas secundário).

### ❌ 1b. Escala zerada no Transform (achado nesta sessão, ainda não fecha o caso)
- **Teste:** Inspector do `battle_0` após zerar a Position para testar a hipótese 1.
- **Evidência:** `Scale: (0, 0, 0)` — objeto com tamanho zero, não gera geometria/área pra desenhar. Bate exatamente com "zero Draw Opaque/Transparent" no Frame Debugger (não é falha de renderização, é ausência de geometria).
- **Correção aplicada:** Scale alterado para `(1, 1, 1)`.
- **Resultado:** **bug persiste mesmo com Scale (1,1,1) e Position (0,0,0)**. Confirma que a escala zerada foi um problema real introduzido durante os testes, mas não é a causa raiz original do bug — a causa raiz ainda está em aberto.

### ❌ 2. Painéis de UI cobrindo a tela inteira
- **Teste:** inspeção direta do `Canvas` em `Battle.unity`.
- **Evidência:** `PainelListaDeItens` e `PainelListaDeAtaques` estão com `m_IsActive: 0`. O background do slider cobre só a faixa 25%-75%, não a tela toda.
- **Conclusão:** nenhuma UI está tampando o mundo.

### ❌ 3. `BattleManager` desativando/movendo o objeto
- **Teste:** busca por referências ao `battle_0` nos campos serializados do `BattleManager` na cena.
- **Evidência:** nenhum campo serializado do `BattleManager` referencia `battle_0`.
- **Conclusão:** descarta o script como causa direta (não cobre lógica interna não serializada, mas não há vínculo de dados).

### ❌ 4. Segunda câmera (vinda do `DontDestroyOnLoad`) limpando o frame por cima
- **Teste:** Frame Debugger em Play mode (Window > Analysis > Frame Debugger).
- **Evidência:** só existe um nó de câmera (`RenderSingleCameraInternal`, 9 eventos: `BlitFinalToBackBuffer` + `DrawScreenSpaceUI`/Canvas). Nenhum segundo passe de câmera aparece.
- **Conclusão:** só há uma câmera renderizando; não há sobrescrita de frame.

### ❌ 5. Transform do `battle_0` diferente em runtime
- Mesmo teste da hipótese 1 (Imagem 1 vs Imagem 2): posição/escala idênticas parado e pausado.
- **Conclusão:** descartada.

### ❌ 6. GameObject ou `SpriteRenderer` desativado em runtime
- **Teste:** checkbox do GameObject e do componente `Sprite Renderer` no Inspector, parado e pausado.
- **Evidência:** ambos marcados (ativo/habilitado) nos dois momentos (Imagem 1 vs Imagem 2).
- **Conclusão:** o objeto e o componente estão ativos o tempo todo.

### ❌ 7. Culling Mask da câmera excluindo o layer `Default`
- **Teste:** leitura direta do YAML de `Battle.unity` (componente `Camera`, GameObject "Main Camera").
- **Evidência:** `m_CullingMask: m_Bits: 4294967295` (todos os 32 bits ligados).
- **Conclusão:** a câmera enxerga todos os layers, inclusive `Default` (layer do `battle_0`).

### ❌ 8. Referência de sprite quebrada (GUID/fileID divergente)
- **Teste:** comparação direta entre `m_Sprite` no `SpriteRenderer` de `battle_0` e o `internalID` gerado em `battle_jpg.meta`.
- **Evidência:** `fileID: 852928365244996250, guid: 8cc24ccf5f7bc36439795641a2bbe359` bate exatamente nos dois arquivos.
- **Conclusão:** a referência ao sprite é válida, não está quebrada/perdida.

### ❌ 9. Câmera/Canvas/SpriteRenderer duplicados na cena
- **Teste:** contagem de blocos `!u!20` (Camera), `!u!223` (Canvas) e `!u!212` (SpriteRenderer) no YAML de `Battle.unity`.
- **Evidência:** exatamente 1 de cada.
- **Conclusão:** não há objeto duplicado conflitando.

### ❌ 10. Câmera renderizando para uma Render Texture (não pra tela)
- **Teste:** campo `m_TargetTexture` do componente Camera.
- **Evidência:** `{fileID: 0}` — nenhuma textura alvo, renderiza direto pro back buffer (bate com a cor de clear azul aparecendo exatamente na tela).
- **Conclusão:** descartada.

### ❌ 11. Câmera do tipo Overlay sem Base Camera no stack
- **Teste:** campo `m_CameraType` na `Universal Additional Camera Data`.
- **Evidência:** `m_CameraType: 0` (Base).
- **Conclusão:** câmera é Base normal, renderiza sozinha sem depender de stack.

### ❌ 12. GameObject solto ("GameObject") na hierarquia sendo algo relevante
- **Teste:** inspeção dos componentes desse objeto no YAML.
- **Evidência:** só tem `Transform`, nenhum outro componente.
- **Conclusão:** objeto órfão, irrelevante para o bug.

---

### ❌ 13. Renderer Data do URP não compatível ou filtrando o layer `Default`
- **Teste 1 — Graphics Settings:** `Project Settings → Graphics`. `Default Render Pipeline` aparece como `None`, mas isso é só o fallback global — não reflete o pipeline realmente ativo (esse é definido por Quality Level).
- **Teste 2 — Quality Settings:** `Project Settings → Quality`. `Current Active Quality Level: Very Low` → `Render Pipeline Asset: UniversalRP (Universal Render Pipeline Asset)`. Pipeline URP está de fato ativo, não é Built-in.
- **Teste 3 — Inspector do asset `UniversalRP`:** `Renderer List → 0 → Renderer2D (Renderer 2D Data)`. Tipo de renderer correto (não é Universal Renderer genérico) — compatível com `Sprite-Lit-Default`.
- **Teste 4 — Inspector do `Renderer2D (Renderer 2D Data)`:** `Filtering → Layer Mask: Everything`. Inclui o layer `Default` do `battle_0`. Também confirma seção `Light Blend Styles` presente (características exclusivas do 2D Renderer Data).
- **Conclusão:** pipeline, renderer type e layer mask estão todos corretos. Hipótese descartada — não é isso.

### ❌ 14. (Consequência do achado 1b) Zero draw calls é causado por posição/pivot fora de área
- **Teste:** Frame Debugger rodado novamente **depois** de já ter corrigido pivot e zerado Position para `(0,0,0)`.
- **Evidência:** árvore de eventos continua mostrando só `BlitFinalToBackBuffer` + `DrawScreenSpaceUI` (Canvas, 7x Draw Mesh). **Nenhum** `Draw Opaque`/`Draw Transparent`/passe de mundo aparece, mesmo com o objeto centralizado e (na ocasião) com Scale zerada.
- **Conclusão parcial:** a ausência de draw calls de mundo bateu com Scale `(0,0,0)` (achado 1b) nesse momento específico — mas depois de corrigir a Scale para `(1,1,1)` o bug **persistiu** (relatado pelo usuário, Frame Debugger pós-fix ainda não reconferido). Isso indica que pode haver mais de uma causa simultânea, ou uma causa ainda não identificada além da escala.

---

### ❌ 16. Escala do sprite battle_0 insuficiente para cobrir a área visível
- **Teste:** Scale do `battle_0` alterada para `(1000, 1000, 1)` — tamanho absurdamente maior que qualquer frustum de câmera possível.
- **Evidência:** Frame Debugger no mesmo estado continua mostrando **zero** passes de mundo (`BlitFinalToBackBuffer` + `DrawScreenSpaceUI` apenas). Print da Game View confirma: tela seguia 100% azul mesmo com Scale 1000x1000.
- **Conclusão:** elimina definitivamente qualquer hipótese geométrica (posição, pivot, escala, enquadramento/frustum). Não é isso.

### ❌ 17. Problema específico do objeto/sprite/material `battle_0`
- **Teste decisivo:** criado um `SpriteRenderer` novo do zero na cena (`GameObject → 2D Object → Sprites → Square`, sprite quadrado branco padrão da própria Unity, sem material customizado, sem histórico de edição) — testado dentro do Canvas e fora do Canvas, em Position (0,0,0) e em tamanho avantajado.
- **Evidência:** o quadrado branco **não aparece em nenhum dos casos**, mesmo sendo um objeto 100% novo e padrão da engine.
- **Conclusão:** o problema **não é do objeto `battle_0` especificamente** (não é sprite corrompido, não é o material `Sprite-Lit-Default`, não é o import settings, não é o GUID) — é algo na **cena/câmera/pipeline** que impede *qualquer* SpriteRenderer de ser renderizado. Isso muda o escopo da investigação: parar de olhar o `battle_0` e focar exclusivamente na câmera / Renderer Features / Render Graph dessa cena específica.

---

## Estado confirmado até agora (checklist do que JÁ NÃO é a causa)

- ❌ Posição do sprite (testado 0,0,0 e posições variadas)
- ❌ Pivot do sprite (corrigido de (0,0) para Center)
- ❌ Escala do sprite (testado 0, 1, e 1000 — nenhuma resolve)
- ❌ Cor/Alpha do SpriteRenderer (branco opaco, confirmado)
- ❌ GameObject ou componente desativado (ambos ativos, confirmado)
- ❌ Culling Mask da câmera (`Everything`, inclui o layer `Default`)
- ❌ Render Pipeline Asset ausente (URP `UniversalRP` confirmado ativo no Quality Level em uso)
- ❌ Tipo de Renderer incompatível (`Renderer2D (Renderer 2D Data)` confirmado, com Light Blend Styles)
- ❌ Layer Mask do Renderer2D (`Everything`, inclui `Default`)
- ❌ Câmera duplicada/conflitante (apenas 1 câmera confirmada na cena)
- ❌ Scale da câmera zerada (era (0,0,1) em um teste, corrigida para (1,1,1) — bug persiste)
- ❌ Problema exclusivo do objeto/sprite/material `battle_0` (SpriteRenderer novo e genérico também não aparece)
- ❌ Render Texture como destino da câmera (`m_TargetTexture: {fileID: 0}`)
- ❌ Câmera tipo Overlay sem stack (`m_CameraType: 0` = Base)

**O que segue confirmado funcionando:** Canvas Screen Space - Overlay renderiza normalmente (UI.Image, Sliders, Botões, Textos — tudo aparece). Apenas objetos com `SpriteRenderer` (mundo 2D) nunca são desenhados, universalmente, para qualquer objeto desse tipo na cena.

---

## ✅ SOLUÇÃO APLICADA (workaround funcional, não é a correção da causa raiz)

**Estratégia usada pelo usuário:** já que `SpriteInimigo` e `SpritePlayer` (UI.Image dentro do Canvas) sempre renderizaram normalmente, e `battle_0` (SpriteRenderer, fora do Canvas) nunca renderizava — a lógica foi "clonar o que funciona e reaproveitar a configuração que já é válida" em vez de seguir caçando a causa raiz no pipeline.

**Passo a passo do que foi feito:**
1. Duplicado o GameObject `SpriteInimigo` (um `UI.Image` dentro do Canvas, que já renderizava normalmente).
2. Trocado o campo `Source Image` do clone para a imagem do mapa (`battle_0`/`battle.jpg`).
3. O mapa passou a aparecer — confirma que o Canvas (`UI.Image`) nunca teve o bug; o bug era exclusivo de objetos `SpriteRenderer` no mundo.
4. Problema novo: a duplicata (o clone) renderizava **por cima** do `SpriteInimigo` original, cobrindo o inimigo (efeito colateral da ordem dos objetos dentro do Canvas — em Canvas, a ordem na Hierarchy define a ordem de desenho; um objeto duplicado é inserido logo abaixo do original na Hierarchy, ou seja, desenhado depois → aparece por cima).
5. Solução do efeito colateral: em vez de reordenar manualmente, o usuário duplicou o **objeto do mapa** (que já estava por cima) e trocou a imagem *dessa nova duplicata* de volta para a sprite do inimigo — usando a mesma regra (duplicata sempre fica por cima) a favor, garantindo que o inimigo ficasse na camada correta de novo.
6. Resultado: mapa de fundo e inimigo aparecendo corretamente, cada um na ordem visual certa, ambos como `UI.Image` dentro do Canvas.

**Por que isso funciona tecnicamente:** o bug raiz (documentado nas hipóteses 1-19 abaixo) afeta exclusivamente o pipeline de `SpriteRenderer` (objetos de mundo, fora do Canvas) — nenhuma causa encontrada explica por que esse pipeline não desenha nada, mas ficou provado que **o Canvas Screen Space - Overlay nunca foi afetado**. Ao mover o sprite do mapa para dentro do Canvas como `UI.Image`, ele passa a usar o mesmo pipeline que já funcionava (Canvas.RenderOverlays → Draw Mesh), contornando o bug em vez de resolvê-lo.

**Limitação conhecida do workaround:** `UI.Image` não é `SpriteRenderer` — não tem Sorting Layer/Order in Layer do mundo 2D, a escala e posicionamento seguem lógica de `RectTransform` (pixels/anchors) em vez de unidades de mundo, e não interage com luzes 2D (`Sprite-Lit-Default` deixa de fazer sentido nesse contexto). Para essa cena de batalha (fundo estático + UI), isso não é um problema prático. Se o projeto precisar de SpriteRenderers funcionais no mundo em outras cenas no futuro (parallax, sprites com luz 2D, Sorting Layers complexos), a causa raiz (hipóteses 18/19) ainda precisa ser encontrada.

---

### ⏳ 18. Renderer Feature customizada ou etapa do Render Graph pulando Draw Opaque/Transparent
- **Por que suspeitar:** o Frame Debugger mostra a `ExecuteRenderGraph` executando apenas 2 sub-passes (`BlitFinalToBackBuffer` e `DrawScreenSpaceUI`) — nenhum passe nomeado tipo `DrawOpaqueObjects`/`DrawTransparentObjects`/`Render Opaques` aparece em nenhum momento, em nenhum teste. Isso é atípico até para uma cena vazia — normalmente esses passes aparecem mesmo sem nada pra desenhar (só ficam com contagem 0 de draw calls, mas o nó existe).
- **Teste pendente:** conferir se existe alguma **Renderer Feature** custom na aba `Renderer Features` do asset `Renderer2D (Renderer 2D Data)` que possa estar interferindo (já vimos "No Renderer Features added" — então não é isso) — **na verdade já descartado, ver Image 3 da mensagem anterior sobre o Renderer2D**.
- **Teste pendente real:** comparar o Frame Debugger da cena **City** (que funciona) lado a lado com o da **Battle**, prestando atenção à presença/ausência do nó de Draw Opaque/Transparent Objects — se a City também não tiver esse nó nomeado mas ainda assim funcionar, o formato do nome do passe no Render Graph desta versão da Unity pode ser diferente do que estamos assumindo, e o verdadeiro problema está em outro lugar.

### ⏳ 19. Prefab Override ou componente extra desabilitando renderização silenciosamente em toda a cena Battle
- **Por que suspeitar:** o comportamento é uniforme para todo e qualquer SpriteRenderer criado na cena, o que sugere algo em nível de cena (não de objeto) — ex: um script rodando em `Awake`/`OnEnable` de algum objeto global (`BattleManager`, `EventSystem`, ou até um script anexado à própria `Main Camera` que não apareceu nos componentes vistos até agora) que desliga a renderização de sprites (ex: `Camera.cullingMask = 0` sobrescrito em runtime, ou uma Renderer Feature/Volume desabilitando opacos via script).
- **Teste pendente:** abrir o Inspector da Main Camera e do BattleManager rolando até o fim (pode haver mais scripts/componentes abaixo da área visível nos prints atuais) — confirmar se há algum script além do já visto.
- **Teste pendente:** comparar diretamente o Culling Mask e demais campos da câmera **em runtime** (Play, pausado) vs. **em edição** (Play parado) — se algo estiver diferente, é sinal de um script alterando isso via código no primeiro frame.
