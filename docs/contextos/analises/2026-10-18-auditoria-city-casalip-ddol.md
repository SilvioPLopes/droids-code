# Auditoria — City, CasaLip e DontDestroyOnLoad

**Data:** 18/10/2026  
**Escopo:** leitura de `City`, do YAML salvo de `CasaLip`, dos prefabs e scripts consumidores.  
**Limites:** nenhuma cena, objeto, componente ou script foi alterado. `City` foi consultada fora de Play e salva; `DontDestroyOnLoad` estava vazio na consulta, portanto sua descrição abaixo deriva do código, não de uma sessão em execução.

## 1. Mapa da hierarquia

```text
City
├─ Main Camera                         CameraFollow → Player
├─ Grid
│  └─ Tilemap                          Tilemap + TilemapRenderer
├─ metavalle_0                         SpriteRenderer: desenho da cidade
├─ GameObject e GameObject (1)…(16)     PolygonCollider2D: obstáculos
├─ Global Light 2D
│  └─ Player                           movimento, física e RestaurarPosicao
├─ Tácio                               interação de loja + gatilho de diálogo
│  └─ Canvas
│     ├─ Text (TMP)                    indicador
│     └─ Loja                          LojaUIManager
├─ Dara
│  └─ Canvas/Text (TMP)                indicador
├─ homem da fila
│  └─ Canvas/Text (TMP)                indicador
├─ comentário_Apollo                   diálogo automático de conclusão
├─ SaidaCidade                         transição + TrancaPorHistoria
│  └─ Canvas/desativado                texto de bloqueio
├─ Entrada_CasaLip                     transição para CasaLip
├─ PontoDeChegada_City                  marcador
├─ PontoDeChegada_CasaLip               marcador
├─ ControleDeHistoria                  CondicaoDeHistoria; regras vazias
├─ EventSystem                         InputSystemUIInputModule
└─ Canvas                              UI compartilhada
   ├─ PainelMenuMundo
   ├─ PainelStatus
   ├─ PainelDroid
   ├─ PainelTerminal
   ├─ PainelBag
   ├─ GerenciadorMenuMundo             MenuMundoManager
   └─ PainelDialogo
      ├─ TextoFalante / TextoFala
      ├─ BotaoAvancar
      └─ PainelDeEscolhas
```

```text
CasaLip — montagem salva
├─ Main Camera
├─ Global Light 2D
├─ LipCasa_0                           desenho da casa
├─ Actors
│  └─ Player                           movimento, física e RestaurarPosicao
├─ Apollo                              dois GatilhoDeDialogo + OrientacaoNpc
│  └─ Canvas/Text (TMP)                identificação no mundo
├─ TerminalCasa                        InteragirComTerminalCasa + trigger
├─ PortaDeSaidaCasa                    TransicaoDeCena + trigger
├─ ControlePuzzleAgua                  controlador + VerificadorDePuzzle
├─ GameObject / GameObject (1)/(2)      PolygonCollider2D
├─ GameObject (3)                      EdgeCollider2D
├─ EventSystem                         InputSystemUIInputModule
└─ Canvas                              instância de Canvas.prefab
   ├─ painéis compartilhados e GerenciadorMenuMundo
   ├─ preciso ir no terminal           aviso da porta
   ├─ PainelTerminal/TextoApollo
   └─ PainelDialogo
      ├─ AreaRetrato/ImagemRetrato
      ├─ AreaTexto
      │  ├─ TextoFalante / TextoFala
      │  └─ BotaoAvancar/Text (TMP)
      └─ PainelDeEscolhas
```

Estrutura persistente prevista:

```text
DontDestroyOnLoad
├─ Canvas                              primeiro Canvas preservado
│  └─ painéis e gestores que já estavam dentro dele
└─ GerenciadorDeEstado                 singleton criado sob demanda
```

`MenuMundoManager.Awake()` preserva `transform.root.gameObject`. O `EventSystem`, os NPCs e o Player não são preservados por esse mecanismo.

## 2. Transforms fora do padrão

| Cena/objeto | Scale local | Conteúdo afetado |
|---|---|---|
| City — `metavalle_0` | `3,3,1` | Apenas `SpriteRenderer`; não possui filhos |
| City — `Tácio`, `Dara`, `homem da fila` | `2,2,2` | Sprite, trigger e Canvas filho |
| City — Canvas desses três NPCs | `0.5,0.5,0.5` | Compensação da escala do pai |
| City — `SaidaCidade` | `5,1,1` | Trigger de saída e Canvas |
| City — `SaidaCidade/Canvas` | `0.2,1,1` | Compensação horizontal |
| City — `Global Light 2D/Player` | `2,2,0` | Sprite e `CircleCollider2D` |
| City — `comentário_Apollo` | `4.438096,4.003865,1.4596` | Trigger de diálogo automático |
| City — `Canvas/PainelDialogo` | `1.035770,0.517880,1.035770` | Textos, escolhas e avanço; compressão vertical |
| CasaLip — `LipCasa_0` | `1.6,1.5,1` | Desenho da casa, sem filhos |
| CasaLip — `Apollo` | `3,3,1` | Sprite, trigger e identificação |
| CasaLip — `Apollo/Canvas` | `0.0033333332,0.0033333332,1` | Identificação em World Space |
| CasaLip — `Actors/Player` | `4,4,0` | Sprite e collider |

A cidade não está inteira dentro de um objeto escalado. `metavalle_0`, colisores, NPCs, triggers e marcadores são raízes independentes. Os marcadores `PontoDeChegada_City` e `PontoDeChegada_CasaLip` têm escala unitária.

## 3. Fluxo de diálogo

Os NPCs usam sobreposição de `Collider2D` e tecla `E`, não distância calculada.

| Objeto | Disparador | Área do trigger no mundo | Condições |
|---|---|---|---|
| `Tácio` | `NpcInterativo`, tipo `Loja`; chama `GatilhoDeDialogo` em `ApenasPorScript` | `2×2`, centro `(0,4)` | `city_s2_tacio`; uma vez por instância, sem flags exigidas |
| `Dara` | `GatilhoDeDialogo`, `AoInteragir` | `2×2`, centro `(-5.39,2.93)` | Exige `ato1_casa_resolvida`; bloqueado por `ato1_sessao2` |
| `homem da fila` | `GatilhoDeDialogo`, `AoInteragir` | `2×2`, centro `(-4.48,1.38)` | Mesmas condições de Dara |
| `comentário_Apollo` | `GatilhoDeDialogo`, `AoEntrarNoTrigger` | Aproximadamente `4.438×4.004`, centro `(-4.64,2.2)` | Exige `ato1_falou_dara` e `ato1_falou_fila`; bloqueado por `ato1_sessao2` |
| CasaLip — `Apollo`, introdução | `GatilhoDeDialogo`, `AoInteragir` | `0.81×0.96`, centro `(3.39,0.31)` | `casa_intro_apollo`; bloqueado por `ato1_falou_apollo_casa` |
| CasaLip — `Apollo`, conversa | Segundo `GatilhoDeDialogo`, `AoInteragir` | Mesmo collider | `casa_apollo_conversa`; exige `ato1_falou_apollo_casa`; repetível |

`NpcInterativo` está desabilitado em `Dara`, `homem da fila` e `Apollo`, mas os respectivos `GatilhoDeDialogo` estão habilitados. O Player de City tem tag `Player`, `Rigidbody2D` dinâmico, simulação habilitada e gravidade zero.

Referências de `DialogoUIManager`:

| Campo | City | CasaLip |
|---|---|---|
| `painel` | `Canvas/PainelDialogo` | `Canvas/PainelDialogo` |
| `textoFala` | `PainelDialogo/TextoFala` | `PainelDialogo/AreaTexto/TextoFala` |
| `textoFalante` | `PainelDialogo/TextoFalante` | `PainelDialogo/AreaTexto/TextoFalante` |
| `botaoAvancar` | `PainelDialogo/BotaoAvancar` | `PainelDialogo/AreaTexto/BotaoAvancar` |
| `painelDeEscolhas` | `PainelDialogo/PainelDeEscolhas` | Mesmo nome |
| `prefabBotaoEscolha` | `BotaoItemTemplate` | `BotaoItemTemplate.prefab` |

Nenhuma referência obrigatória está vazia ou aponta para outro objeto identificado. Em City, `imagemRetrato` e `retratoPadrao` estão vazios, mas são opcionais. `Tácio/NpcInterativo.gatilhoDeDialogo` está vazio e é resolvido com `GetComponent<GatilhoDeDialogo>()`.

## 4. Botões

Todos os 20 `Button` de City têm zero eventos persistentes no Inspector. Os listeners são registrados por código em `Start()` ou ao instanciar botões.

| Botão | Alvo previsto | Método/ação |
|---|---|---|
| `PainelMenuMundo/BotaoStatus` | `GerenciadorMenuMundo` | `AoClicarStatus()` |
| `BotaoTerminal` | `GerenciadorMenuMundo` | `AoClicarTerminal()` |
| `BotaoBag` | `GerenciadorMenuMundo` | `AoClicarBag()` |
| `BotaoDroid` | `GerenciadorMenuMundo` | `AoClicarDroid()` |
| `BotaoOpcoes` | `GerenciadorMenuMundo` | `MostrarAviso("Opções")` |
| `BotaoSalvar` | `GerenciadorMenuMundo` | `AoClicarSalvar()` |
| `BotaoCarregar` | `GerenciadorMenuMundo` | `AoClicarCarregar()` |
| `BotaoVoltar` | `GerenciadorMenuMundo` | `Fechar()` |
| `PainelStatus/BotaoVoltarStatus` | `PainelStatus` | `Fechar()` |
| `PainelDroid/BotaoVoltarDroid` | `PainelDroid` | `Fechar()` |
| `PainelBag/BotaoVoltarBag` | `PainelBag` | `Fechar()` |
| `PainelTerminal/BotaoExecutar` | `PainelTerminal` | `AoClicarExecutar()` |
| `PainelTerminal/BotaoFechar` | `PainelTerminal` | `Fechar()` |
| `PainelTerminal/BotaoLimpar` | `PainelTerminal` | `AoClicarLimpar()` |
| `PainelDialogo/.../BotaoAvancar` | `PainelDialogo` | `Avancar()` |
| `Tácio/Canvas/Loja/Comprar` | `Loja` | `MudarAba(Aba.Comprar)` |
| `Tácio/Canvas/Loja/Vender` | `Loja` | `MudarAba(Aba.Vender)` |
| `Tácio/Canvas/Loja/Voltar` | `Loja` | `Fechar()` |
| `Tácio/Canvas/Loja/BotaoItemTemplate` | Modelo inativo | Cópias recebem `Comprar(item)` ou `Vender(item)` |
| `Tácio/Canvas/Loja/ScrollView/lista` | Sem ação encontrada | É um container que também possui `Button` |

Não foi encontrado evento persistente com alvo perdido ou método inexistente. O registro efetivo em Play não foi testado.

## 5. Canvas

| Canvas | Render Mode | Canvas Scaler | Sort Order |
|---|---|---|---:|
| City — `Canvas` | Screen Space Overlay | Constant Pixel Size, fator `1` | 0 |
| City — `Tácio/Canvas` | Screen Space Overlay | Constant Pixel Size, fator `1` | 0 |
| City — `Dara/Canvas` | Screen Space Overlay | Constant Pixel Size, fator `1` | 0 |
| City — `homem da fila/Canvas` | Screen Space Overlay | Constant Pixel Size, fator `1` | 0 |
| City — `SaidaCidade/Canvas` | Screen Space Overlay | Constant Pixel Size, fator `1` | 0 |
| CasaLip — `Canvas` | Screen Space Overlay | Scale With Screen Size, `1920×1080`, Match `0.5` | 0 |
| CasaLip — `Apollo/Canvas` | World Space, câmera `Main Camera` | Scaler desabilitado | 11 |

A UI principal está no `Canvas` raiz. A loja está em outro Canvas, filho de Tácio. City usa pixels constantes; CasaLip salva referência `1920×1080`. Como o primeiro Canvas é preservado entre cenas, a UI pode variar conforme o percurso de entrada.

Em City, com Canvas de `3840×2160`, o `BotaoAvancar` ocupa aproximadamente `x=3763.15` até `3870.22`; cerca de 30 pixels ficam além da borda direita. `PainelDialogo` também excede as laterais. Os três indicadores dos NPCs ocupam o mesmo retângulo central e têm `raycastTarget=true`.

## 6. Acoplamentos frágeis

- `MenuMundoManager.Awake()` preserva o primeiro Canvas e descarta duplicatas; City e CasaLip têm layouts de diálogo diferentes.
- `DialogoUIManager.Instancia` usa `FindAnyObjectByType`, incluindo objetos inativos.
- `LocalizadorDeTerminal` pesquisa globalmente e prefere o `TerminalUIManager` persistente.
- O contador global `GerenciadorDeEstado.MenuAberto` bloqueia os dois scripts de interação enquanto é positivo.
- `comentário_Apollo` só testa condições em `OnTriggerEnter2D`; não reavalia enquanto o Player permanece dentro.
- `TransicaoDeCena` usa coordenadas manuais: `Entrada_CasaLip` envia para `(0,-4)` e `PortaDeSaidaCasa` para `(8.89,-9)`. Não há vínculo automático aos marcadores.
- `PortaDeSaidaCasa.textoDeBloqueio` aponta para uma UI local que pode ser descartada como Canvas duplicado.
- Não foram encontrados `GameObject.Find("nome")` ou distâncias numéricas para diálogo; há buscas por tipo, tag, nome de cena e IDs de narrativa.

## 7. Hipóteses ordenadas

1. **Condições narrativas bloqueiam a conversa.** Dara e o homem da fila exigem `ato1_casa_resolvida` e são bloqueados por `ato1_sessao2`. A sessão com falha não foi observada.
2. **Área clicável deslocada ou encoberta.** O botão de avanço está parcialmente fora da tela e há indicadores de tela coincidentes, com raycast habilitado.
3. **Canvas persistente diferente do esperado.** City e CasaLip têm Canvas e hierarquias de diálogo diferentes, enquanto `MenuMundoManager` preserva o primeiro Canvas.
4. **Gatilho automático não é reavaliado após conversas.** A área de `comentário_Apollo` sobrepõe Dara/fila, mas suas condições são avaliadas apenas quando há entrada no trigger.
5. **`MenuAberto` permanece positivo.** Os dois scripts de interação retornam antes de reagir se o contador global estiver aberto. Não foi observado contador preso; a confirmação exige inspeção em Play.

## Estado final

O Unity permaneceu em `City`, fora de Play, com a cena salva. Não houve teste por input real ou aprovação visual.
