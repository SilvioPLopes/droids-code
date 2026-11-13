# Auditoria de scripts C# — BLOCO 06: Construção de UI e responsividade

**Somente leitura.** Não rodei testes, jogo, compilação ou Unity/MCP e não alterei nem criei arquivos existentes. Premissa do usuário adotada: as telas não foram construídas de forma responsiva. Escalas uniformes e alguns anchors adaptáveis não equivalem a reflow/reorganização para proporções distintas.

## 1. Escopo e arquivos necessários

A busca encontrou cinco `Montagem*.cs` em `Assets/Editor` e `RefocoTerminalIlustrado.cs`. Os alvos diretos das montagens são:

- [Canvas.prefab](../../Assets/Prefabs/Canvas.prefab) — Terminal, Menu do Mundo e telas compartilhadas de Status, Droid e Bag.
- [City.unity](../../Assets/Scenes/City.unity) — Loja e Diálogo.
- [Battle.unity](../../Assets/Scenes/Battle.unity) — Battle; `MontagemBattleIlustrada` também carrega `PainelBag.prefab`.
- [GameOver.unity](../../Assets/Scenes/GameOver.unity) — alvo declarado para Game Over; a cena existe.
- [PainelBag.prefab](../../Assets/Prefabs/PainelBag.prefab) e [PainelStatus.prefab](../../Assets/Prefabs/PainelStatus.prefab) — prefabs de UI usados pelas telas.
- [BotaoItemTemplate.prefab](../../Assets/Prefabs/BotaoItemTemplate.prefab) — template referenciado por Bag e opções de diálogo.

Referências nos scripts de montagem: [MontagemTerminalIlustrado.cs:11, 102](../../Assets/Editor/MontagemTerminalIlustrado.cs), [MontagemMenuMundoIlustrado.cs:10, 25](../../Assets/Editor/MontagemMenuMundoIlustrado.cs), [MontagemLojaIlustrada.cs:68](../../Assets/Editor/MontagemLojaIlustrada.cs), [MontagemBattleIlustrada.cs:20–24, 116](../../Assets/Editor/MontagemBattleIlustrada.cs), [MontagemGameOver.cs:14–16](../../Assets/Editor/MontagemGameOver.cs) e [RefocoTerminalIlustrado.cs:11](../../Assets/Editor/RefocoTerminalIlustrado.cs).

O prefab compartilhado `Canvas.prefab` é usado em 34 cenas, incluindo City, Game, CasaLip e interiores. Portanto, o Canvas não tem uma câmera única: usa a câmera da cena hospedeira. A configuração varia entre cenas; os YAMLs dirigidos não estabelecem o resultado visual para cada host.

## 2. Padrão geral das montagens

O padrão recorrente cria caixas com âncora e pivô em ponto único, `anchoredPosition` e `sizeDelta` em pixels: exemplos em [MontagemTerminalIlustrado.cs:15–22](../../Assets/Editor/MontagemTerminalIlustrado.cs), [MontagemLojaIlustrada.cs:13–16](../../Assets/Editor/MontagemLojaIlustrada.cs), [MontagemBattleIlustrada.cs:40–45](../../Assets/Editor/MontagemBattleIlustrada.cs), [MontagemGameOver.cs:59–61](../../Assets/Editor/MontagemGameOver.cs) e [RefocoTerminalIlustrado.cs:17–18](../../Assets/Editor/RefocoTerminalIlustrado.cs).

As exceções mais frequentes são fundos e áreas de rolagem esticados. Terminal, Menu do Mundo, Loja e o ramo visual de Battle escalam quadros de referência, mas mantêm composição/coordenadas fixas. Não foi encontrado reflow geral, quebra adaptativa para telas estreitas ou tratamento geral de safe area. `LayoutGroup`/`ContentSizeFitter` aparecem em listas/descrições, não como solução global da composição.

Existe também divergência compartilhada no CanvasScaler: o prefab-base usa `ConstantPixelSize`, mas instâncias em Game e CasaLip sobrescrevem para `ScaleWithScreenSize`, referência 1920×1080 e match 0,5 ([Game.unity:4823–4836](../../Assets/Scenes/Game.unity), [CasaLip.unity:2545–2558](../../Assets/Scenes/CasaLip.unity)). No prefab-base, o modo é `0`, referência 800×600 e match `0` ([Canvas.prefab:4369–4374](../../Assets/Prefabs/Canvas.prefab)). A escala global da UI pode, portanto, variar com a cena hospedeira; nenhum desses modos implementa por si só reflow dos elementos.

## 3. Fundo azul e câmeras

A cor azul está configurada como cor de fundo da câmera: RGB `{0.19215687, 0.3019608, 0.4745098}`, aproximadamente `#314D79`.

- Em Game, a câmera usa `m_ClearFlags: 2` e viewport completo (`x=0, y=0, width=1, height=1`); o azul é a cor de limpeza sólida serializada ([Game.unity:895–928](../../Assets/Scenes/Game.unity)).
- City e Battle também serializam a mesma cor; nelas `m_ClearFlags: 1` e `RenderSettings.m_SkyboxMaterial` é nulo ([City.unity:29, 8652–8684](../../Assets/Scenes/City.unity), [Battle.unity:29, 4797–4835](../../Assets/Scenes/Battle.unity)).
- Game Over, por contraste, limpa em preto ([GameOver.unity:514–530](../../Assets/Scenes/GameOver.unity)).
- Os Canvas compartilhados são `ScreenSpaceOverlay` ([Canvas.prefab:4302–4350](../../Assets/Prefabs/Canvas.prefab)); pixels descobertos ou transparentes deixam aparecer a cena/câmera por trás.

Assim, a origem configurada do azul é a câmera, não uma imagem azul de fundo da UI. A configuração pode explicar o azul que aparece em áreas sem cobertura do Canvas, mas a leitura estática **não comprova quais pixels ficam azuis numa execução específica**. Não há letterbox configurado na viewport inspecionada; conteúdo com proporção fixa pode deixar áreas descobertas. Aspecto de execução não está fixado nesses YAMLs.

## 4. Mapeamento de responsividade por tela

| Tela | CanvasScaler, layout e fundos | Câmera | Problema de responsividade e esforço qualitativo |
|---|---|---|---|
| **Terminal** | Canvas base é overlay e `ConstantPixelSize` ([Canvas.prefab:4302–4374](../../Assets/Prefabs/Canvas.prefab)). `PainelTerminal` centralizado em 1254×1254 ([Canvas.prefab:941–1034](../../Assets/Prefabs/Canvas.prefab)); refoco define referência 1536×1024 e repõe elementos em coordenadas/tamanhos absolutos ([RefocoTerminalIlustrado.cs:55–83](../../Assets/Editor/RefocoTerminalIlustrado.cs)). Arte no painel e fundo escuro de foco separado ([Canvas.prefab:1400–1475, 1521–1528](../../Assets/Prefabs/Canvas.prefab)). Fitters em áreas roláveis, não composição global ([Canvas.prefab:926, 5974](../../Assets/Prefabs/Canvas.prefab)). | Herdada da cena host; em Game é azul sólida e viewport full, em City viewport full com clear flags 1 e skybox nulo. | Escala uniforme não reorganiza campo, painéis, rótulos e botões. **Médio-alto**: adaptar painel e controles para proporções estreitas preservando arte e rolagem. |
| **Loja** | Canvas associado ao ramo da Loja em City usa modo 0, referência 800×600 e match 0 ([City.unity:6046–6112](../../Assets/Scenes/City.unity)). Raiz mede 1536×1024 e usa escala local; arte e controles usam pixels ([City.unity:849–949](../../Assets/Scenes/City.unity), [MontagemLojaIlustrada.cs:15–16, 86–124](../../Assets/Editor/MontagemLojaIlustrada.cs)). Arte não preserva aspecto; `FundoLoja` é separado ([City.unity:9894–9940, 10704–10749](../../Assets/Scenes/City.unity)). Grid de duas colunas, células de 400×154 e fitter da descrição ([City.unity:11564–11584](../../Assets/Scenes/City.unity), [MontagemLojaIlustrada.cs:104–116](../../Assets/Editor/MontagemLojaIlustrada.cs)). | City, ortográfica, viewport full; cor azul configurada ([City.unity:8652–8684](../../Assets/Scenes/City.unity)). | Grade e detalhes pressupõem composição fixa. **Alto**: prever reflow da grade/detalhes e tratar proporção da arte. |
| **Bag** | `PainelBag.prefab` tem raiz central, posição `(-31.93, -50.72)`, tamanho 220×100 ([PainelBag.prefab:446–473](../../Assets/Prefabs/PainelBag.prefab)); contém `VerticalLayoutGroup` e `ContentSizeFitter` vertical ([PainelBag.prefab:485–512](../../Assets/Prefabs/PainelBag.prefab)). Template tem largura fixa 260×30 ([BotaoItemTemplate.prefab:20–45](../../Assets/Prefabs/BotaoItemTemplate.prefab)). Montagem de Battle instancia o prefab ([MontagemBattleIlustrada.cs:20–35](../../Assets/Editor/MontagemBattleIlustrada.cs)); scaler Battle modo 0, referência 800×600, escala serializada 3.3 ([Battle.unity:5083–5090](../../Assets/Scenes/Battle.unity)). | Em Battle, viewport full, ortográfica, azul serializado ([Battle.unity:4797–4835](../../Assets/Scenes/Battle.unity)); na Bag exploratória, depende da cena hospedeira. | Layout automático limita-se à lista; raiz/elementos não refluem. **Médio**. A pega fixa da scrollbar já foi registrada no BLOCO 05 e não é duplicada como novo achado. |
| **Droid** | Manager no Canvas compartilhado ([Canvas.prefab:8621–8689](../../Assets/Prefabs/Canvas.prefab)); ramo usa prefab-base de Status com alterações de manager/nome ([Canvas.prefab:8520–8630](../../Assets/Prefabs/Canvas.prefab)). Raiz do prefab de Status estica e tem fundo preto translúcido, mas campos internos usam âncoras pontuais e dimensões fixas ([PainelStatus.prefab:1096–1157, 1222–1226](../../Assets/Prefabs/PainelStatus.prefab)). | Herdada da cena host; scaler também pode herdar overrides de Game/CasaLip. | Fundo stretch não compensa campos de equipamento fixos. **Médio-alto**. |
| **Status** | Manager no Canvas compartilhado ([Canvas.prefab:8021–8060](../../Assets/Prefabs/Canvas.prefab)). Raiz do `PainelStatus.prefab` é stretch 0–1 com fundo escuro translúcido; textos usam âncora central, offset e tamanho fixos ([PainelStatus.prefab:1096–1157, 1222–1226](../../Assets/Prefabs/PainelStatus.prefab)). Não foi encontrado `LayoutGroup`/`AspectRatioFitter` na raiz. | Da cena host; fundo translúcido não oculta a câmera. | Seções não se reorganizam nem mudam dimensões; escala global não resolve. **Médio-alto**. |
| **Menu do Mundo** | `PainelMenuMundo` estica até a Canvas ([Canvas.prefab:4646–4681](../../Assets/Prefabs/Canvas.prefab)), mas arte 1254×1254 ([Canvas.prefab:7697–7750](../../Assets/Prefabs/Canvas.prefab)); montagem define arte/título com tamanho fixo ([MontagemMenuMundoIlustrado.cs:29–41](../../Assets/Editor/MontagemMenuMundoIlustrado.cs)). Apresentador escala e centraliza com margem sem reflow dos botões ([ApresentacaoMenuMundoIlustrado.cs:65–85](../../Assets/Scripts/Menus/ApresentacaoMenuMundoIlustrado.cs)). Fundo escuro de foco ([Canvas.prefab:2329–2382](../../Assets/Prefabs/Canvas.prefab)). | Cena que hospeda o Canvas; painel não tem câmera própria. | Escala adapta quadro, não arranjo quadrado de arte e botões. **Médio**. |
| **Battle** | CanvasScaler modo 0, referência 800×600, match 0 e escala 3.3 ([Battle.unity:5083–5090](../../Assets/Scenes/Battle.unity)). Montagem cria quadro 1672×941 e posiciona mapa, HUD e botões em coordenadas fixas; mapa 1672×775 ([MontagemBattleIlustrada.cs:43–45, 124–169](../../Assets/Editor/MontagemBattleIlustrada.cs)). Layouts em listas, não no quadro principal ([Battle.unity:2512–2537, 4641–4667](../../Assets/Scenes/Battle.unity)). | Ortográfica, viewport completo, clear flags 1, cor azul ([Battle.unity:4797–4835](../../Assets/Scenes/Battle.unity)). | Apresentador já escala uniformemente o ramo local, registrado no BLOCO 01, mas não rearranja HUD, mapa, log e botões. **Alto**. |
| **Game Over** | CanvasScaler modo 1, referência 1920×1080, match 0,5 ([GameOver.unity:1129–1134](../../Assets/Scenes/GameOver.unity)). Fundo stretch 0–1, offsets zero e imagem sem preservação de aspecto ([GameOver.unity:994–1050](../../Assets/Scenes/GameOver.unity)). Montagem ancora título, botões e aviso no canto superior esquerdo com posições/tamanhos fixos ([MontagemGameOver.cs:50–61, 76–94](../../Assets/Editor/MontagemGameOver.cs)). | Ortográfica, viewport full, clear sólido preto ([GameOver.unity:499–530](../../Assets/Scenes/GameOver.unity)). | CanvasScaler escala globalmente; conteúdo segue fixo na referência e o fundo pode distorcer em proporções distintas. **Médio**. |
| **Diálogo** | Em City, `PainelDialogo` fica entre anchors horizontais 10%–90%, no rodapé, com `AspectRatioFitter` razão 3,1967 ([City.unity:6150–6191](../../Assets/Scenes/City.unity)). Texto 902×240,5, fonte 36, âncora central; escolhas têm layout vertical e fitter ([City.unity:6741–6765](../../Assets/Scenes/City.unity)). Scalers dos Canvas da cena em modo 0, referência 800×600, match 0 ([City.unity:2605–2611, 5362–5368, 5681–5687, 6100–6106](../../Assets/Scenes/City.unity)). | City, viewport full e azul configurado ([City.unity:8652–8684](../../Assets/Scenes/City.unity)). | A largura acompanha anchors, mas razão fixa e texto de dimensões fixas não fazem reflow; existe risco estrutural de encaixe com falas longas. **Médio**. |

As referências acima dão evidência de configuração serializada; não são comparações visuais entre proporções nem provam cortes efetivamente observados.

## 5. Padrões compartilhados, baseline e limitações

- **Causa comum sustentada:** composição baseada em coordenadas de referência fixas, por vezes escalada uniformemente, com `LayoutGroup`/fitters restritos a conteúdos. A correção sistêmica exigiria planejar regras responsivas por tela; não se conclui que um único script compartilhado corrija todos os painéis.
- **Azul:** a origem configurada é comum em algumas câmeras, mas é distinta dos problemas de layout. O relatório geral já registrava o azul como relato sem reprodução ([auditoria geral:228, 270](2026-10-30-auditoria-geral-projeto.md)); este mapeamento identifica a cor serializada, sem validar o sintoma visual em execução.
- **Battle:** o ramo de escala local já foi coberto no BLOCO 01 ([BLOCO 01](2026-10-31-scripts-BLOCO-01-combate.md)); não é repetido como novo achado.
- **Bag:** a pega fixa da scrollbar já foi registrada no BLOCO 05 ([BLOCO 05](2026-10-31-scripts-BLOCO-05-menus-interiores-camera.md)); não é repetida como novo achado.
- PNGs/assets verificados para as montagens (Terminal, Menu, Loja, Game Over, Bag e Battle), os prefabs e cenas listados, e a fonte LiberationSans citada pela montagem de Game Over foram encontrados. Isso não substitui auditoria completa de referências.
- Para decidir cortes, legibilidade e aparência final ainda faltam a resolução/aspecto e o modo de tela desejados, a cena hospedeira exata e validação visual; não foram feitos testes visuais.

## 6. Inventário de leitura

### Completude dos requisitos específicos de responsividade

Este mapeamento é **parcial** em relação à lista numérica solicitada. Os trechos YAML consultados identificam modo/referência/match de alguns CanvasScaler e padrões de âncoras/tamanhos, mas não tabulam `screenMatchMode` para cada Canvas, todos os anchors/pivôs e `sizeDelta`/`anchoredPosition` de cada elemento, nem valores numéricos de `orthographicSize`/aspect de cada câmera. A tabela por tela é evidência representativa e não uma inspeção exaustiva de todos os componentes serializados. O aspecto efetivo depende também da resolução/modo de execução. Nenhuma conclusão visual sobre cortes, sobreposições ou pixels realmente azuis foi validada.

### Scripts lidos integralmente

- [MontagemTerminalIlustrado.cs](../../Assets/Editor/MontagemTerminalIlustrado.cs) — 187 linhas.
- [MontagemMenuMundoIlustrado.cs](../../Assets/Editor/MontagemMenuMundoIlustrado.cs) — 46 linhas.
- [MontagemLojaIlustrada.cs](../../Assets/Editor/MontagemLojaIlustrada.cs) — 128 linhas.
- [MontagemGameOver.cs](../../Assets/Editor/MontagemGameOver.cs) — 109 linhas.
- [MontagemBattleIlustrada.cs](../../Assets/Editor/MontagemBattleIlustrada.cs) — 178 linhas.
- [RefocoTerminalIlustrado.cs](../../Assets/Editor/RefocoTerminalIlustrado.cs) — 105 linhas.

### Cena/prefab lidos como YAML

- Leitura integral: [BotaoItemTemplate.prefab](../../Assets/Prefabs/BotaoItemTemplate.prefab).
- Leitura de trechos dirigidos, não integral: `Canvas.prefab`, `PainelBag.prefab`, `PainelStatus.prefab`, `City.unity`, `Battle.unity`, `Game.unity`, `GameOver.unity` e trechos de `CasaLip.unity`.
- Outros hosts que contêm `Canvas.prefab` não foram lidos integralmente nem tiveram câmeras mapeadas individualmente. Não houve arquivo necessário que não pudesse ser aberto; essa limitação é de escopo, não de acesso.

## 7. Próximo passo

BLOCO 06 concluído. Próximo e último: BLOCO 07 — consolidação dos seis relatórios, sem acrescentar alterações de código e sem afirmar validação em runtime.
