# Análise factual do projeto Unity

- **Data/hora:** 14/10/2026 17:07 (UTC−03:00)
- **Pastas varridas:** raiz do repositório; `Assets/`; `Assets/Scenes/`; `Assets/Prefabs/`; `Assets/Tilemaps/`; `Assets/Scripts/` e subpastas; `Assets/Settings/`; `Assets/TextMesh Pro/Examples & Extras/`; `Packages/`; `ProjectSettings/`.
- **Critério de conteúdo:** `Assets/Scripts/` e as cinco cenas de `Assets/Scenes/` foram tratadas como conteúdo do projeto. `Assets/TextMesh Pro/Examples & Extras/` e `Assets/Settings/Scenes/URP2DSceneTemplate.unity` foram contabilizados separadamente como exemplos/template. Fonte: caminhos e arquivos enumerados em `Assets/`.
- **Serialização verificada antes da leitura de cenas e prefabs:** `m_SerializationMode: 2` em `ProjectSettings/EditorSettings.asset` (Force Text).

## 1. Ambiente

| Item | Valor lido | Arquivo(s) |
|---|---|---|
| Unity | `6000.5.8f1` | `ProjectSettings/ProjectVersion.txt` |
| Render pipeline | URP; o asset atribuído ao perfil de qualidade ativo aponta para `UniversalRP.asset`, que contém `Renderer2D` como renderer padrão | `Packages/manifest.json`; `ProjectSettings/QualitySettings.asset`; `Assets/Settings/UniversalRP.asset`; `Assets/Settings/Renderer2D.asset` |
| Scripting Backend | Android: valor serializado `1` em `scriptingBackend`. O README declara Mono, mas `ProjectSettings.asset` só define explicitamente Android nessa seção; backend de outros destinos não está especificado nesse trecho | `ProjectSettings/ProjectSettings.asset`; `readme.md` |
| API Compatibility Level | Valor serializado `6`; o README identifica o nível como .NET Standard 2.1 | `ProjectSettings/ProjectSettings.asset`; `readme.md` |
| Input | Ambos: `activeInputHandler: 2`; existe asset Input Actions e `InputManager.asset` contém eixos legados | `ProjectSettings/ProjectSettings.asset`; `ProjectSettings/InputManager.asset`; `Assets/Settings/InputSystem_Actions.inputactions` |
| UI | uGUI e TextMeshPro confirmados em scripts e cenas. Uso de UI Toolkit não encontrado nos scripts do projeto analisados | `Packages/manifest.json`; `Assets/Scripts/`; `Assets/Scenes/` |
| Serialização | Force Text (`m_SerializationMode: 2`) | `ProjectSettings/EditorSettings.asset` |
| `.gitignore` | Presente | `.gitignore` |
| Pasta `.git` | Presente | `.git/` |
| Pacotes | Lista completa abaixo | `Packages/manifest.json` |
| Pacotes de AI | `com.unity.ai.assistant` e `com.unity.ai.inference` | `Packages/manifest.json` |
| Pastas/arquivos relacionados a MCP | Nenhuma pasta ou arquivo com nome MCP foi encontrado nas pastas de conteúdo do projeto enumeradas (`Assets/`, `Packages/`, `ProjectSettings/`) | `Assets/`; `Packages/`; `ProjectSettings/` |
| Unity Cloud | `cloudProjectId` e `organizationId` vazios; `cloudEnabled: 0`; `m_Enabled: 0` | `ProjectSettings/ProjectSettings.asset`; `ProjectSettings/UnityConnectSettings.asset` |

### Pacotes de `Packages/manifest.json`

| Pacote | Versão |
|---|---|
| `org.moonsharp.moonsharp` | `https://github.com/moonsharp-devs/moonsharp.git?path=/interpreter#upm/beta/v3.0` |
| `com.unity.2d.animation` | `15.1.0` |
| `com.unity.2d.aseprite` | `5.0.3` |
| `com.unity.2d.psdimporter` | `14.0.3` |
| `com.unity.2d.sprite` | `1.0.0` |
| `com.unity.2d.spriteshape` | `15.0.3` |
| `com.unity.2d.tilemap` | `1.0.0` |
| `com.unity.2d.tilemap.extras` | `8.0.3` |
| `com.unity.2d.tooling` | `3.0.1` |
| `com.unity.ai.assistant` | `2.17.0-pre.1` |
| `com.unity.ai.inference` | `2.6.1` |
| `com.unity.collab-proxy` | `2.13.5` |
| `com.unity.ide.rider` | `3.0.38` |
| `com.unity.ide.visualstudio` | `2.0.26` |
| `com.unity.inputsystem` | `1.20.0` |
| `com.unity.multiplayer.center` | `1.0.1` |
| `com.unity.render-pipelines.universal` | `17.6.0` |
| `com.unity.test-framework` | `1.7.0` |
| `com.unity.timeline` | `1.8.12` |
| `com.unity.ugui` | `2.5.0` |
| `com.unity.visualscripting` | `1.9.12` |
| `com.unity.modules.accessibility` | `1.0.0` |
| `com.unity.modules.adaptiveperformance` | `1.0.0` |
| `com.unity.modules.ai` | `1.0.0` |
| `com.unity.modules.androidjni` | `1.0.0` |
| `com.unity.modules.animation` | `1.0.0` |
| `com.unity.modules.assetbundle` | `1.0.0` |
| `com.unity.modules.audio` | `1.0.0` |
| `com.unity.modules.cloth` | `1.0.0` |
| `com.unity.modules.director` | `1.0.0` |
| `com.unity.modules.imageconversion` | `1.0.0` |
| `com.unity.modules.imgui` | `1.0.0` |
| `com.unity.modules.jsonserialize` | `1.0.0` |
| `com.unity.modules.particlesystem` | `1.0.0` |
| `com.unity.modules.physics` | `1.0.0` |
| `com.unity.modules.physics2d` | `1.0.0` |
| `com.unity.modules.physicscore2d` | `1.0.0` |
| `com.unity.modules.screencapture` | `1.0.0` |
| `com.unity.modules.terrain` | `1.0.0` |
| `com.unity.modules.terrainphysics` | `1.0.0` |
| `com.unity.modules.tilemap` | `1.0.0` |
| `com.unity.modules.ui` | `1.0.0` |
| `com.unity.modules.uielements` | `1.0.0` |
| `com.unity.modules.umbra` | `1.0.0` |
| `com.unity.modules.unityanalytics` | `1.0.0` |
| `com.unity.modules.unitywebrequest` | `1.0.0` |
| `com.unity.modules.unitywebrequestassetbundle` | `1.0.0` |
| `com.unity.modules.unitywebrequestaudio` | `1.0.0` |
| `com.unity.modules.unitywebrequesttexture` | `1.0.0` |
| `com.unity.modules.unitywebrequestwww` | `1.0.0` |
| `com.unity.modules.vectorgraphics` | `1.0.0` |
| `com.unity.modules.vehicles` | `1.0.0` |
| `com.unity.modules.video` | `1.0.0` |
| `com.unity.modules.wind` | `1.0.0` |
| `com.unity.modules.xr` | `1.0.0` |

## 2. Cenas

### 2.1 `ProjectSettings/EditorBuildSettings.asset`

| Ordem | Caminho | Habilitada |
|---:|---|---|
| 1 | `Assets/Scenes/MainMenu.unity` | Sim |
| 2 | `Assets/Scenes/Game.unity` | Sim |
| 3 | `Assets/Scenes/Battle.unity` | Sim |
| 4 | `Assets/Scenes/City.unity` | Sim |
| 5 | `Assets/Scenes/CasaLip.unity` | Sim |

Fonte: `ProjectSettings/EditorBuildSettings.asset`.

### 2.2 Todas as cenas `.unity` encontradas em `Assets/`

| Categoria | Caminho | No Build |
|---|---|---|
| Jogo | `Assets/Scenes/MainMenu.unity` | Sim |
| Jogo | `Assets/Scenes/Game.unity` | Sim |
| Jogo | `Assets/Scenes/Battle.unity` | Sim |
| Jogo | `Assets/Scenes/City.unity` | Sim |
| Jogo | `Assets/Scenes/CasaLip.unity` | Sim |
| Template | `Assets/Settings/Scenes/URP2DSceneTemplate.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/01-  Single Line TextMesh Pro.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/02 - Multi-line TextMesh Pro.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/03 - Line Justification.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/04 - Word Wrapping.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/05 - Style Tags.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/06 - Extra Rich Text Examples.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/07 - Superscript & Subscript Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/08 - Improved Text Alignment.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/09 - Margin Tag Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/10 - Bullets & Numbered List Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/11 - The Style Tag.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/12 - Link Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/12a - Text Interactions.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/13 - Soft Hyphenation.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/14 - Multi Font & Sprites.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/15 - Inline Graphics & Sprites.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/16 - Linked text overflow mode example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/17 - Old Computer Terminal.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/18 - ScrollRect & Masking & Layout.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/19 - Masking Texture & Soft Mask.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/20 - Input Field with Scrollbar.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/21 - Script Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/22 - Basic Scripting Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/23 - Animating Vertex Attributes.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/24 - Surface Shader Example URP.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/24 - Surface Shader Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/25 - Sunny Days Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/26 - Dropdown Placeholder Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/27 - Double Pass Shader Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/28 - HDRP Shader Example.unity` | Não |
| Exemplo TextMesh Pro | `Assets/TextMesh Pro/Examples & Extras/Scenes/Benchmark (Floating Text).unity` | Não |

### 2.3 Cenas do jogo

Os campos listados como nulos são referências serializadas cujo `fileID` é zero. Referências internas para as quais o nome do alvo não foi resolvido são indicadas pelo `fileID` que aparece no arquivo. Os arquivos `.unity` possuem objetos provenientes de instâncias de prefab; a listagem abaixo registra os componentes de script do projeto que aparecem nos documentos YAML da cena.

#### `Assets/Scenes/MainMenu.unity`

| Propriedade | Valor |
|---|---|
| GameObjects raiz | `EventSystem`; `GerenciadorMenu`; `Main Camera`; `mainmenu_0` |
| Canvas até 3 níveis | `EventSystem` → `Canvas` → `DroidsCode`; `Configurações` → `Text (TMP)`; `PainelConfiguracoes` → `Slider`, `Voltar`; `PainelPrincipal`; `Novo Jogo` → `Text (TMP)`; `Sair` → `Text (TMP)` |
| Canvas Scaler | `Scale With Screen Size` (`m_UiScaleMode: 0`); referência `800 × 600` |
| Objetos inativos com scripts do projeto | Nenhum encontrado |

| GameObject | Script | Campos com referência nula no arquivo | Campos preenchidos (campo → alvo) |
|---|---|---|---|
| `GerenciadorMenu` | `Assets/Scripts/Menus/MainMenuManager.cs` | Nenhum detectado | `painelPrincipal` → `PainelPrincipal`; `painelConfiguracoes` → `PainelConfiguracoes` |

Fonte: `Assets/Scenes/MainMenu.unity`; campos de escala no mesmo arquivo.

#### `Assets/Scenes/Game.unity`

| Propriedade | Valor |
|---|---|
| GameObjects raiz | `PontoDeChegada_Game`; `EntradaCidade`; `ZonaBatalha`; objeto sem nome; `GameObject`; `falaqualquer`; `Grid`; `GameObject (3)`; `Main Camera`; `GameObject (1)`; `GameObject (2)`; `EventSystem`; `ferrovale_0`; `Global Light 2D`; `GameObject (5)`; `GameObject (4)` |
| Canvas até 3 níveis | `PainelDialogo` → `TextoFalante`; `TextoFala`; `BotaoAvancar`; `PainelDeEscolhas` |
| Canvas Scaler | A cena contém overrides de prefab para `m_UiScaleMode` e `m_ReferenceResolution`; o valor efetivo está no prefab instanciado. `Assets/Prefabs/Canvas.prefab` contém Canvas Scaler com modo `0` e referência `800 × 600` |
| Objetos inativos com scripts do projeto | Nenhum encontrado |

| GameObject | Script | Campos com referência nula no arquivo | Campos preenchidos (campo → alvo) |
|---|---|---|---|
| `falaqualquer` | `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | Nenhum detectado | `indicadorDeInteracao` → `IndicadorDeInteracao` |
| `ZonaBatalha` | `Assets/Scripts/Combat/EncounterZone.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| `Main Camera` | `Assets/Scripts/Camera/CameraFollow.cs` | Nenhum detectado | `target` → referência `fileID 896406761` |
| Objeto sem nome | `Assets/Scripts/Scripting/PlayerMovement.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| Objeto sem nome | `Assets/Scripts/Scripting/RestaurarPosicao.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| `PainelDialogo` | `Assets/Scripts/Historia/DialogoUIManager.cs` | Nenhum detectado | `painel` → `PainelDialogo`; `textoFalante` → `TextoFalante`; `textoFala` → `TextoFala`; `botaoAvancar` → `BotaoAvancar`; `painelDeEscolhas` → referência `fileID 488990099`; `prefabBotaoEscolha` → prefab referenciado pelo GUID `36cb0281c2ffd9d428385410c352f69a` |
| `EntradaCidade` | `Assets/Scripts/Scripting/TransicaoDeCena.cs` | `textoDeBloqueio` | Nenhum |

Fonte: `Assets/Scenes/Game.unity`; escala efetiva do Canvas em `Assets/Prefabs/Canvas.prefab`.

#### `Assets/Scenes/Battle.unity`

| Propriedade | Valor |
|---|---|
| GameObjects raiz | `Main Camera`; `GameObject`; `BattleManager`; `Canvas`; `EventSystem` |
| Canvas até 3 níveis | `Canvas` → `SliderInimigo` → `Fill Area` → `Fill`; `Handle Slide Area` → `Handle`, `Background`; `PainelListaDeAtaques`; `PainelListaDeItens`; `TextoMensagem`; `SpritePlayer`; `SliderPlayer`; `BotaoAtacar` → `Text (TMP)`; `Mapa`; `BotaoFugir` → `Text (TMP)`; `SpriteInimigo`; `BotaoItem` → `Text (TMP)`; `Inimigo`; `Status` → `Text (TMP)` |
| Canvas Scaler | `Scale With Screen Size` (`m_UiScaleMode: 0`); referência `800 × 600` |
| Objetos inativos com scripts do projeto | Nenhum encontrado |

| GameObject | Script | Campos com referência nula no arquivo | Campos preenchidos (campo → alvo) |
|---|---|---|---|
| `BattleManager` | `Assets/Scripts/Combat/BattleManager.cs` | Nenhum detectado | `sliderPlayer` → `SliderPlayer`; `sliderInimigo` → `SliderInimigo`; `textoMensagem` → `TextoMensagem`; `botaoAtacar` → `BotaoAtacar`; `botaoItem` → `BotaoItem`; `botaoFugir` → `BotaoFugir`; `botaoStatus` → `Status`; `telaDeStatus` → referência `fileID 770136228225574263`; `painelListaDeAtaques` → referência `fileID 1815127158`; `prefabBotaoAtaque` → prefab GUID `c96b74a9f79832a41b9acdb3401aee8c`; `painelListaDeItens` → referência `fileID 1090460036`; `prefabBotaoItem` → prefab GUID `36cb0281c2ffd9d428385410c352f69a` |
| Objeto de prefab sem nome | `Assets/Scripts/Menus/TelaDeStatusManager.cs` | Nenhum campo serializado próprio detectado | Componente presente em instância de prefab |

Fonte: `Assets/Scenes/Battle.unity`.

#### `Assets/Scenes/City.unity`

| Propriedade | Valor |
|---|---|
| GameObjects raiz | `GameObject (8)`; `PontoDeChegada_City`; `GameObject (5)`; `GameObject (9)`; `Dara`; `Global Light 2D`; `comentário_Apollo`; `Main Camera`; `GameObject (6)`; `GameObject (4)`; `homem da fila`; `GameObject (1)`; `GameObject (10)`; `GameObject (15)`; `EventSystem`; `GameObject (3)`; `Grid`; `GameObject (16)`; `metavalle_0`; `GameObject (12)`; `GameObject (2)`; `GameObject (11)`; `Entrada_CasaLip`; `ControleDeHistoria`; objeto sem nome; `SaidaCidade`; `Tácio`; `GameObject (14)`; `GameObject`; `PontoDeChegada_CasaLip`; `GameObject (13)`; `GameObject (7)` |
| Canvas até 3 níveis | `Dara` → `Canvas` → `Text (TMP)`; `Tácio` → `Canvas` → `Text (TMP)` e `Loja` → `Mensagem`, `Voltar`, `Comprar`, `ScrollView`, `Vender`, `Gold`; `homem da fila` → `Canvas` → `Text (TMP)`; `SaidaCidade` → `Canvas` → `desativado`; há quatro Canvas Scalers, cada um com modo `0` e referência `800 × 600` |
| Objetos inativos com scripts do projeto | Nenhum encontrado |

| GameObject | Script | Campos com referência nula no arquivo | Campos preenchidos (campo → alvo) |
|---|---|---|---|
| `Loja` | `Assets/Scripts/Menus/LojaUIManager.cs` | Nenhum detectado | `painel` → `Loja`; `botaoAbaComprar` → `Comprar`; `botaoAbaVender` → `Vender`; `painelListaDeItens` → referência `fileID 1199021419`; `prefabBotaoItem` → referência de prefab; `botaoVoltar` → `Voltar`; `textoMensagem` → `Mensagem`; `textoGold` → `Gold` |
| `SaidaCidade` | `Assets/Scripts/Scripting/TransicaoDeCena.cs` | Nenhum detectado | `textoDeBloqueio` → `desativado` |
| `SaidaCidade` | `Assets/Scripts/Historia/TrancaPorHistoria.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| Objeto sem nome | `Assets/Scripts/Scripting/RestaurarPosicao.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| Objeto sem nome | `Assets/Scripts/Scripting/PlayerMovement.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| `Entrada_CasaLip` | `Assets/Scripts/Scripting/TransicaoDeCena.cs` | `textoDeBloqueio` | Nenhum |
| `ControleDeHistoria` | `Assets/Scripts/Historia/CondicaoDeHistoria.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| `PainelDialogo` | `Assets/Scripts/Historia/DialogoUIManager.cs` | Nenhum detectado | `painel` → `PainelDialogo`; `textoFalante` → `TextoFalante`; `textoFala` → `TextoFala`; `botaoAvancar` → `BotaoAvancar`; `painelDeEscolhas` → referência `fileID 1354953970`; `prefabBotaoEscolha` → prefab GUID `36cb0281c2ffd9d428385410c352f69a` |
| `homem da fila` | `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | `indicadorDeInteracao` | Nenhum |
| `homem da fila` | `Assets/Scripts/Historia/NpcInterativo.cs` | `lojaUIManager`; `gatilhoDeDialogo` | `indicadorDeInteracao` → `Text (TMP)` |
| `Dara` | `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | `indicadorDeInteracao` | Nenhum |
| `Dara` | `Assets/Scripts/Historia/NpcInterativo.cs` | `lojaUIManager`; `gatilhoDeDialogo` | `indicadorDeInteracao` → `Text (TMP)` |
| `comentário_Apollo` | `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | `indicadorDeInteracao` | Nenhum |
| `Main Camera` | `Assets/Scripts/Camera/CameraFollow.cs` | Nenhum detectado | `target` → referência `fileID 978994979` |
| `Tácio` | `Assets/Scripts/Historia/NpcInterativo.cs` | `gatilhoDeDialogo` | `lojaUIManager` → `Loja`; `indicadorDeInteracao` → `Text (TMP)` |
| `Tácio` | `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | `indicadorDeInteracao` | Nenhum |

Fonte: `Assets/Scenes/City.unity`.

#### `Assets/Scenes/CasaLip.unity`

| Propriedade | Valor |
|---|---|
| GameObjects raiz | `EventSystem`; `GameObject (3)`; dois objetos sem nome; `GameObject (1)`; `LipCasa_0`; `Apollo`; `GameObject`; `Main Camera`; `Global Light 2D`; `TerminalCasa`; `PortaDeSaidaCasa`; `GameObject (2)` |
| Canvas até 3 níveis | `Apollo` → `Canvas` → `Text (TMP)`; `PainelDialogo` → `TextoFalante`, `TextoFala`, `ImagemRetrato`, `BotaoAvancar`, `PainelDeEscolhas` |
| Canvas Scaler | `Scale With Screen Size` (`m_UiScaleMode: 0`); referência `800 × 600` |
| Objetos inativos com scripts do projeto | Nenhum encontrado |

| GameObject | Script | Campos com referência nula no arquivo | Campos preenchidos (campo → alvo) |
|---|---|---|---|
| `TerminalCasa` | `Assets/Scripts/Casa/InteragirComTerminalCasa.cs` | Nenhum detectado | `terminal` → referência do componente de terminal |
| `Apollo` | `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | `indicadorDeInteracao` | Nenhum |
| `Apollo` | `Assets/Scripts/Historia/NpcInterativo.cs` | `lojaUIManager`; `gatilhoDeDialogo`; `indicadorDeInteracao` | Nenhum |
| `PortaDeSaidaCasa` | `Assets/Scripts/Scripting/TransicaoDeCena.cs` | Nenhum detectado | `textoDeBloqueio` → `preciso ir no terminal` |
| `PainelDialogo` | `Assets/Scripts/Historia/DialogoUIManager.cs` | `retratoPadrao` | `painel` → `PainelDialogo`; `textoFalante` → `TextoFalante`; `textoFala` → `TextoFala`; `imagemRetrato` → `ImagemRetrato`; `botaoAvancar` → `BotaoAvancar`; `painelDeEscolhas` → referência `fileID 288717857`; `prefabBotaoEscolha` → prefab GUID `36cb0281c2ffd9d428385410c352f69a` |
| Objeto sem nome | `Assets/Scripts/Scripting/RestaurarPosicao.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| Objeto sem nome | `Assets/Scripts/Scripting/PlayerMovement.cs` | Nenhum detectado | Nenhum campo de referência não nulo localizado |
| Objeto sem nome | `Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs` | Nenhum detectado | `transicaoDaPortaDeSaida` → `PortaDeSaidaCasa`; `textoDeApollo` → `TextoApollo` |
| Objeto de prefab sem nome | `Assets/Scripts/Menus/TerminalUIManager.cs` | Nenhum campo serializado próprio detectado | Componente presente em instância de prefab |

Fonte: `Assets/Scenes/CasaLip.unity`.

## 3. Prefabs do jogo

| Caminho | Scripts do projeto anexados | Campos com referência nula no arquivo | Variante |
|---|---|---|---|
| `Assets/Prefabs/BotaoAtaquePrefab.prefab` | Nenhum script do projeto detectado | Não se aplica | Não identificada como variante |
| `Assets/Prefabs/BotaoItemTemplate.prefab` | Nenhum script do projeto detectado | Não se aplica | Não identificada como variante |
| `Assets/Prefabs/Canvas.prefab` | `TerminalUIManager`; `MenuMundoManager`; `TelaDeStatusManager`; `BagUIManager`; `TelaDeDroidManager` | Nenhuma referência nula em campos próprios detectada | Não identificada como variante |
| `Assets/Prefabs/PainelBag.prefab` | `BagUIManager` | Nenhuma referência nula em campos próprios detectada | Não identificada como variante |
| `Assets/Prefabs/PainelStatus.prefab` | `TelaDeStatusManager` | Nenhuma referência nula em campos próprios detectada | Não identificada como variante |
| `Assets/Tilemaps/MetaValle.prefab` | Nenhum script do projeto detectado | Não se aplica | Não identificada como variante |

Fonte: arquivos `.prefab` e respectivas referências em `Assets/Prefabs/` e `Assets/Tilemaps/`. Instâncias aninhadas de prefab dentro de `Canvas.prefab` não foram classificadas como variantes.

## 4. Scripts

### 4.1 Contagem de arquivos `.cs`

| Pasta/categoria | Quantidade |
|---|---:|
| `Assets/Scripts/Camera/` | 1 |
| `Assets/Scripts/Casa/` | 2 |
| `Assets/Scripts/Catalogos/` | 8 |
| `Assets/Scripts/Combat/` | 6 |
| `Assets/Scripts/Droid/` | 7 |
| `Assets/Scripts/Estado/` | 4 |
| `Assets/Scripts/Historia/` | 9 |
| `Assets/Scripts/Menus/` | 7 |
| `Assets/Scripts/Scripting/` | 14 |
| `Assets/Scripts/Verificadores/` | 1 |
| **Scripts do projeto** | **59** |
| `Assets/TextMesh Pro/Examples & Extras/Scripts/` (terceiros) | 34 |

Fonte: enumeração recursiva de arquivos `*.cs` sob `Assets/`.

### 4.2 Classes do projeto derivadas de `MonoBehaviour` ou `ScriptableObject`

Os nomes de campos abaixo são os campos públicos ou anotados com `[SerializeField]` declarados nos scripts indicados. Namespace vazio significa que o arquivo não declara namespace. Os tipos são os tipos de declaração lidos no código.

| Caminho | Classe | Namespace | Campos public/[SerializeField] (tipo + nome) | `[RequireComponent]` |
|---|---|---|---|---|
| `Assets/Scripts/Camera/CameraFollow.cs` | `CameraFollow : MonoBehaviour` | Global | `Transform target`; `float smoothSpeed`; `Vector3 offset`; `bool usarLimites`; `Vector2 limiteMin`; `Vector2 limiteMax` | — |
| `Assets/Scripts/Casa/InteragirComTerminalCasa.cs` | `InteragirComTerminalCasa : MonoBehaviour` | `DroidsCode.Casa` | `TerminalUIManager terminal`; `KeyCode teclaDeInteracao`; `string tagDoPlayer` | `Collider2D` |
| `Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs` | `PuzzleAguaCasaChecker : MonoBehaviour` | `DroidsCode.Casa` | `string nomeDaVariavel`; `double valorDeVitoria`; `TransicaoDeCena transicaoDaPortaDeSaida`; `TMP_Text textoDeApollo`; `string mensagemDeVitoria` | `TerminalUIManager` |
| `Assets/Scripts/Combat/BattleManager.cs` | `BattleManager : MonoBehaviour` | Global | `Slider sliderPlayer`; `Slider sliderInimigo`; `TextMeshProUGUI textoMensagem`; `Button botaoAtacar`; `Button botaoItem`; `Button botaoFugir`; `Button botaoStatus`; `TelaDeStatusManager telaDeStatus`; `Transform painelListaDeAtaques`; `Button prefabBotaoAtaque`; `Transform painelListaDeItens`; `Button prefabBotaoItem`; `string nomeInimigo`; `int hpMaxInimigo`; `int atkInimigo`; `int defInimigo`; `int recompensaXpInimigo`; `string idItem`; `float chance`; `int quantidadeMinima`; `int quantidadeMaxima`; `int goldMinimo`; `int goldMaximo`; `List<DropDeItem> tabelaDeDrops`; `string idDoInimigoPadrao`; `string flagDeVitoria`; `string nomeCenaMundo`; `float chanceDeFugir` | — |
| `Assets/Scripts/Combat/EncounterZone.cs` | `EncounterZone : MonoBehaviour` | Global | `float chanceDeEncontro`; `float distanciaPorPasso`; `string nomeCenaBatalha`; `string idDoInimigo`; `string[] flagsNecessarias`; `string[] flagsQueImpedem` | `Collider2D` |
| `Assets/Scripts/Estado/GerenciadorDeEstado.cs` | `GerenciadorDeEstado : MonoBehaviour` | Global | `bool MenuAberto` | — |
| `Assets/Scripts/Historia/CondicaoDeHistoria.cs` | `CondicaoDeHistoria : MonoBehaviour` | Global | `string descricao`; `GameObject[] alvos`; `string[] flagsNecessarias`; `string[] flagsQueImpedem`; `bool inverter`; `Regra[] regras`; `bool reavaliarContinuamente`; `float intervaloDeReavaliacao` | — |
| `Assets/Scripts/Historia/DialogoUIManager.cs` | `DialogoUIManager : MonoBehaviour` | Global | `GameObject painel`; `TextMeshProUGUI textoFalante`; `TextMeshProUGUI textoFala`; `Image imagemRetrato`; `Sprite retratoPadrao`; `List<RetratoPersonagem> retratosConhecidos`; `Button botaoAvancar`; `KeyCode teclaAvancar`; `KeyCode teclaAvancarAlternativa`; `Transform painelDeEscolhas`; `Button prefabBotaoEscolha`; `Action AoFechar` | — |
| `Assets/Scripts/Historia/GatilhoDeBatalha.cs` | `GatilhoDeBatalha : MonoBehaviour` | Global | `string idDoInimigo`; `string nomeCenaBatalha`; `string[] flagsNecessarias`; `string[] flagsQueImpedem`; `string flagDeVitoria`; `string tagDoPlayer`; `bool exigirTecla`; `KeyCode teclaDeInteracao`; `GameObject indicadorDeInteracao` | `Collider2D` |
| `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | `GatilhoDeDialogo : MonoBehaviour` | Global | `System.Action AoConcluirDialogoExterno`; `ModoDeDisparo modo`; `KeyCode teclaDeInteracao`; `string tagDoPlayer`; `string[] flagsNecessarias`; `string[] flagsQueImpedem`; `string idDoDialogo`; `List<FalaDeDialogo> falas`; `List<EscolhaDeDialogo> escolhas`; `string flagAoTerminar`; `bool apenasUmaVez`; `bool desativarAposConcluir`; `GameObject indicadorDeInteracao` | `Collider2D` |
| `Assets/Scripts/Historia/NpcInterativo.cs` | `NpcInterativo : MonoBehaviour` | Global | `string nomeDoNpc`; `TipoDeNpc tipo`; `LojaUIManager lojaUIManager`; `GatilhoDeDialogo gatilhoDeDialogo`; `GameObject indicadorDeInteracao`; `KeyCode teclaDeInteracao` | `Collider2D` |
| `Assets/Scripts/Historia/QuestUIManager.cs` | `QuestUIManager : MonoBehaviour` | Global | `GameObject painel`; `Transform painelListaDeQuests`; `Button prefabBotaoQuest`; `Button botaoVoltar`; `TextMeshProUGUI textoTitulo`; `TextMeshProUGUI textoResumo`; `TextMeshProUGUI textoObjetivos`; `TextMeshProUGUI textoRecompensas`; `TextMeshProUGUI textoCuriosidade`; `System.Action AoFechar` | — |
| `Assets/Scripts/Historia/TrancaPorHistoria.cs` | `TrancaPorHistoria : MonoBehaviour` | Global | `string[] flagsNecessarias`; `string[] flagsQueImpedem`; `string mensagemQuandoFaltaFlag`; `bool exigirTecnicaConfigurada`; `int minimoDeAcoes`; `string mensagemQuandoFaltaTecnica`; `bool podeTrancarDeNovo` | `TransicaoDeCena` |
| `Assets/Scripts/Menus/BagUIManager.cs` | `BagUIManager : MonoBehaviour` | Global | `GameObject painel`; `Transform painelListaDeItens`; `Button prefabBotaoItem`; `Button botaoVoltar`; `TextMeshProUGUI textoMensagem`; `System.Action AoFechar` | — |
| `Assets/Scripts/Menus/LojaUIManager.cs` | `LojaUIManager : MonoBehaviour` | Global | `GameObject painel`; `Button botaoAbaComprar`; `Button botaoAbaVender`; `Transform painelListaDeItens`; `Button prefabBotaoItem`; `Button botaoVoltar`; `TextMeshProUGUI textoMensagem`; `TextMeshProUGUI textoGold`; `System.Action AoFechar` | — |
| `Assets/Scripts/Menus/MainMenuManager.cs` | `MainMenuManager : MonoBehaviour` | Global | `GameObject painelPrincipal`; `GameObject painelConfiguracoes`; `string nomeCenaJogo` | — |
| `Assets/Scripts/Menus/MenuMundoManager.cs` | `MenuMundoManager : MonoBehaviour` | Global | `GameObject painelMenu`; `KeyCode teclaDeAbertura`; `string[] cenasBloqueadas`; `Button botaoStatus`; `Button botaoTerminal`; `Button botaoBag`; `Button botaoDroid`; `Button botaoOpcoes`; `Button botaoSalvar`; `Button botaoCarregar`; `Button botaoVoltar`; `TelaDeStatusManager telaDeStatus`; `TerminalUIManager terminal`; `BagUIManager bag`; `TelaDeDroidManager telaDeDroid`; `TextMeshProUGUI textoAviso` | — |
| `Assets/Scripts/Menus/TelaDeDroidManager.cs` | `TelaDeDroidManager : MonoBehaviour` | Global | `GameObject painel`; `TextMeshProUGUI textoBraco`; `TextMeshProUGUI textoPerna`; `TextMeshProUGUI textoTronco`; `TextMeshProUGUI textoCabeca`; `Button botaoVoltar`; `System.Action AoFechar` | — |
| `Assets/Scripts/Menus/TelaDeStatusManager.cs` | `TelaDeStatusManager : MonoBehaviour` | Global | `GameObject painel`; `TextMeshProUGUI textoNome`; `TextMeshProUGUI textoHp`; `TextMeshProUGUI textoAtributos`; `TextMeshProUGUI textoTecnicas`; `TextMeshProUGUI textoPontos`; `TextMeshProUGUI textoExp`; `TextMeshProUGUI textoGold`; `Button botaoVoltar`; `System.Action AoFechar` | — |
| `Assets/Scripts/Menus/TerminalUIManager.cs` | `TerminalUIManager : MonoBehaviour` | Global | `GameObject painelTerminal`; `TMP_InputField campoDeCodigo`; `Button botaoExecutar`; `Button botaoFechar`; `Button botaoLimpar`; `TextMeshProUGUI textoSaida`; `ScrollRect scrollRectSaida`; `TextMeshProUGUI textoPontos`; `TextMeshProUGUI textoAjuda`; `string codigoDeExemplo`; `int maximoDeLinhasDeLog`; `bool mostrarDicasDoApollo`; `System.Action AoFechar`; `DroidScriptRunner Runner`; `System.Action AoExecutarCodigo` | — |
| `Assets/Scripts/Scripting/PlayerMovement.cs` | `PlayerMovement : MonoBehaviour` | Global | `float velocidade` | `Rigidbody2D`; `Animator`; `SpriteRenderer` |
| `Assets/Scripts/Scripting/RestaurarPosicao.cs` | `RestaurarPosicao : MonoBehaviour` | Global | Nenhum campo public/[SerializeField] detectado | — |
| `Assets/Scripts/Scripting/TransicaoDeCena.cs` | `TransicaoDeCena : MonoBehaviour` | Global | `string nomeCenaDestino`; `Vector2 pontoDeChegada`; `bool trancada`; `TMP_Text textoDeBloqueio`; `string mensagemDeBloqueio` | `Collider2D` |
| `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs` | `VerificadorDePuzzle : MonoBehaviour` | Global | `string idDoPuzzle`; `TerminalUIManager terminal`; `string nomeDaVariavel`; `TipoDeComparacao comparacao`; `double valorDeVitoria`; `TransicaoDeCena portaParaDestrancar`; `TMP_Text textoDeApollo`; `string falaAoResolverManual`; `GameObject[] objetosParaAtivar`; `GameObject[] objetosParaDesativar`; `int tentativasAteDica`; `string flagAoResolver`; `string flagQueJaResolve`; `bool logDeDepuracao` | — |

`DroidPart` e suas subclasses (`Braco`, `Cabeca`, `Perna` e `Tronco`) são classes C# comuns e não derivam de `MonoBehaviour` nem de `ScriptableObject`, conforme `Assets/Scripts/Droid/DroidPart.cs`, `Braco.cs`, `Cabeca.cs`, `Perna.cs` e `Tronco.cs`. Nenhuma classe própria declarada como derivada de `ScriptableObject` foi localizada em `Assets/Scripts/`.

### 4.3 Scripts de Editor

| Caminho | Evidência |
|---|---|
| `Assets/Scripts/Menus/MainMenuManager.cs` | Contém uso de `UnityEditor.EditorApplication.isPlaying` dentro de `#if UNITY_EDITOR` |
| Scripts sob pasta `Editor/` | Nenhum encontrado em `Assets/Scripts/` |

### 4.4 Arquivos `.asmdef`

Nenhum arquivo `.asmdef` encontrado sob `Assets/`.

### 4.5 ScriptableObjects em `.asset`

| Caminho | Tipo/estado |
|---|---|
| `Assets/Settings/DefaultVolumeProfile.asset` | `VolumeProfile` do URP |
| `Assets/Settings/Renderer2D.asset` | Dados de renderer 2D (`Renderer2D`) |
| `Assets/Settings/UniversalRP.asset` | Asset de configuração `UniversalRenderPipelineAsset` |
| `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset` | Configuração global do URP |
| `Assets/Settings/Build Profiles/Windows.asset` | Build Profile do Windows |

Os tipos acima correspondem aos assets de configuração Unity/URP referenciados pelos arquivos `Assets/Settings/*.asset`; nenhum asset `.asset` instanciado de uma classe `ScriptableObject` declarada em `Assets/Scripts/` foi encontrado.

## 5. Comparação com `DOCUMENTACAO_TECNICA.md` (§3 e §4)

### 5.1 Divergências de caminho

| O que o documento lista | O que existe no projeto | Arquivo/fonte |
|---|---|---|
| `Assets/Scripts/BattleManager.cs` | `Assets/Scripts/Combat/BattleManager.cs` | `DOCUMENTACAO_TECNICA.md` §3; arquivo atual |
| `Assets/Scripts/EncounterZone.cs` | `Assets/Scripts/Combat/EncounterZone.cs` | `DOCUMENTACAO_TECNICA.md` §3; arquivo atual |
| `Assets/Scripts/BagUIManager.cs`, `LojaUIManager.cs`, `MainMenuManager.cs`, `MenuMundoManager.cs`, `TelaDeDroidManager.cs`, `TelaDeStatusManager.cs`, `TerminalUIManager.cs` | Arquivos correspondentes sob `Assets/Scripts/Menus/` | `DOCUMENTACAO_TECNICA.md` §3; arquivos atuais |
| `Assets/Scripts/PlayerMovement.cs`, `RestaurarPosicao.cs`, `TransicaoDeCena.cs` | Arquivos correspondentes sob `Assets/Scripts/Scripting/` | `DOCUMENTACAO_TECNICA.md` §3; arquivos atuais |
| `Assets/Scripts/NpcInterativo.cs` | `Assets/Scripts/Historia/NpcInterativo.cs` | `DOCUMENTACAO_TECNICA.md` §3; arquivo atual |
| `Assets/Scripts/Historia/VerificadorDePuzzle.cs` | `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs` | `DOCUMENTACAO_TECNICA.md` §3; arquivo atual |
| `Assets/Scripts/Historia/CatalogoDeDialogos.cs`, `CatalogoDeLicoes.cs`, `CatalogoDeNpcs.cs`, `CatalogoDePuzzles.cs`, `CatalogoDeQuests.cs` | Arquivos correspondentes sob `Assets/Scripts/Catalogos/` | `DOCUMENTACAO_TECNICA.md` §3; arquivos atuais |
| `Assets/Scripts/Combat/CatalogoDeInimigos.cs` | `Assets/Scripts/Catalogos/CatalogoDeInimigos.cs` | `DOCUMENTACAO_TECNICA.md` §3; arquivo atual |
| `Assets/Scripts/Droid/CatalogoDeItens.cs`, `EfeitosTemporariosUtil.cs`, `ItemConsumivel.cs`, `PontosDeProgressao.cs`, `ProgressaoDeNivel.cs`, `SistemaDeProgressao.cs`, `TabelaDeCombate.cs`, `TabelaDeCustos.cs`, `TecnicaComposta.cs`, `TipoAtributo.cs` | Os arquivos correspondentes estão sob `Assets/Scripts/Catalogos/` (catálogo) ou `Assets/Scripts/Scripting/` (demais) | `DOCUMENTACAO_TECNICA.md` §3; arquivos atuais |
| `Assets/Scripts/Historia/GatilhoDeQuest.cs` | Não encontrado | `DOCUMENTACAO_TECNICA.md` §3; enumeração de `Assets/Scripts/` |
| `Assets/Scripts/_Teste/InimigoTeste.cs` | Não encontrado | `DOCUMENTACAO_TECNICA.md` §3; enumeração de `Assets/Scripts/` |
| Arquivos de `Assets/Scripts/Catalogos/` | Oito arquivos existem, mas os caminhos dessa pasta não estão listados na §3 | `Assets/Scripts/Catalogos/`; `DOCUMENTACAO_TECNICA.md` §3 |
| Classes `GerenciadorDeQuests.cs`, `QuestUIManager.cs`, `CatalogoDeInimigos.cs` e `CatalogoDeItens.cs` documentadas em caminhos de outras pastas | Arquivos existem nos caminhos reais enumerados em `Assets/Scripts/` | `DOCUMENTACAO_TECNICA.md` §3; arquivos atuais |

Não foram encontrados nomes-base `.cs` duplicados em pastas diferentes dentro de `Assets/Scripts/`. A comparação de caminhos mostra arquivos no projeto não descritos com o caminho atual na §3, incluindo os oito catálogos em `Assets/Scripts/Catalogos/`, além dos caminhos realocados listados na tabela.

### 5.2 Respostas solicitadas

| Pergunta | Resultado lido |
|---|---|
| a) `GatilhoDeQuest.cs` existe? Caminho e campos. | Não foi localizado em `Assets/Scripts/`; portanto, caminho e campos: **NÃO VERIFICÁVEL** (arquivo inexistente na enumeração atual). A documentação cita o caminho `Assets/Scripts/Historia/GatilhoDeQuest.cs`. |
| b) `TerminalUIManager` tem `EscreverNoLog`? Visibilidade de `AdicionarLinhaDeLog`? `VerificadorDePuzzle` chama `EscreverNoLog`? | Sim: `public void EscreverNoLog(string linha)` em `Assets/Scripts/Menus/TerminalUIManager.cs:296`; `AdicionarLinhaDeLog` não tem modificador de acesso na declaração e é privado por padrão (`Assets/Scripts/Menus/TerminalUIManager.cs:298`). `VerificadorDePuzzle` chama `terminal.EscreverNoLog(...)` em `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs:270`. |
| c) `TerminalDroidApi` tem `Explicar`, `Ajuda` ou `Resumo`? | Nenhum desses métodos foi encontrado em `Assets/Scripts/Scripting/TerminalDroidApi.cs`. O método `Explicar` encontrado está em `Assets/Scripts/Catalogos/CatalogoDeLicoes.cs`, não em `TerminalDroidApi`. |
| d) `TerminalDroidApi.TesteAdicionarPontos` ainda existe? | Sim; método `TesteAdicionarPontos(int quantidade)` em `Assets/Scripts/Scripting/TerminalDroidApi.cs:246`. |

## 6. Compilação

### 6.1 Riscos identificáveis por leitura

| Arquivo/linha | Leitura |
|---|---|
| `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs:270`; `Assets/Scripts/Menus/TerminalUIManager.cs:296` | A chamada `terminal.EscreverNoLog(...)` possui método público correspondente no tipo `TerminalUIManager`; a divergência registrada em `DOCUMENTACAO_TECNICA.md` §8.1/§9 não está presente nesses arquivos atuais. |

Não foi identificado, na verificação textual dessa chamada cruzada, método ausente ou visibilidade incompatível. Isso não representa resultado de compilação.

### 6.2 Resultado real

**NÃO VERIFICÁVEL** — não foi aberta uma sessão do Unity Editor nem executada compilação pelo Unity nesta análise.

## 7. Configurações

| Configuração | Valor lido | Arquivo |
|---|---|---|
| Tags | Lista vazia (`tags: []`) | `ProjectSettings/TagManager.asset` |
| Layers nomeadas | `0 Default`; `1 TransparentFX`; `2 Ignore Raycast`; `4 Water`; `5 UI` | `ProjectSettings/TagManager.asset` |
| Sorting Layers | `Default` (`uniqueID: 0`) | `ProjectSettings/TagManager.asset` |
| Input Actions | `Assets/Settings/InputSystem_Actions.inputactions` | `Assets/Settings/InputSystem_Actions.inputactions` |

## 8. Documentos de enredo e design

Os caminhos e contagens foram obtidos dos arquivos encontrados na raiz. Títulos abaixo reproduzem apenas as linhas de heading Markdown.

| Caminho | Linhas | Títulos |
|---|---:|---|
| `DOCUMENTACAO_TECNICA.md` | 851 | `Droids Code — Documentação Técnica`; `0. Como um agente de IA deve usar este documento`; `0.1 Regras obrigatórias de comportamento e entrega (13/09/2026)`; `0.2 Os três documentos são UMA entidade, não três documentos separados (13/09/2026)`; `1. Ambiente confirmado`; `2. Arquitetura`; `3. Estrutura de pastas (scripts)`; `4. Mapa de classes`; `4.1 DroidCore — Droid e progressão`; `4.2 Combat — combate`; `4.3 Scripting — terminal`; `4.4 Salvamento — persistência`; `4.5 World — mundo e estado entre cenas`; `4.6 UI — apresentação`; `4.7 Casa — cena CasaLip (puzzle de introdução, Sessão 1 do Enredo)`; `4.8 Historia — diálogo, flags e progressão do Ato 1 (15/09/2026)`; `5. Fórmulas e valores atuais`; `6. Decisões estruturais fechadas (não reabrir sem necessidade real)`; `7. Convenções de código`; `8. Roadmap (documentado, ainda não implementado)`; `8.1 Pacote "Ato 1 — catálogos de conteúdo" (✅ confirmado em 15/09/2026, sessão de reconciliação)`; `9. Pendências técnicas conhecidas` |
| `readme.md` | 346 | `Droids Code`; `Sobre o Projeto`; `Proposta Pedagógica`; `Mecânicas Principais e Design de Jogo`; `Terminal de Código`; `Combate por Turnos`; `Progressão`; `Itens, Equipamento e Gold`; `Missões (Quests)`; `Customização e Herança de Peças`; `Depuração Amigável (Apollo Debugger)`; `Salvamento`; `Mundo, Encontros e Cidade`; `Sinopse e Universo`; `Facções e Decisões Táticas (Ato 1)`; `Matriz Curricular & Progressão Pedagógica`; `Status de Desenvolvimento e Roteiro`; `Implementado e funcional`; `Roadmap (ainda não implementado)`; `Bugs conhecidos`; `Tecnologias`; `Licença` |
| `CHECKLIST_DE_DESENVOLVIMENTO.md` | 237 | `Droids Code — Checklist e Roteiro de Desenvolvimento`; `Reconciliado: pacote "catálogos de conteúdo" do Ato 1 (15/09/2026)`; `Sistema de história do Ato 1 (código pronto, Editor pendente)`; `Catálogos de conteúdo do Ato 1 — Itens/Quests/NPCs/Diálogos/Puzzles/Lições (confirmado 15/09/2026)`; `Cena CasaLip — puzzle de introdução (água) + Camera Follow`; `Gold no Status + Menu persistente entre cenas`; `Bugs abertos (fazer primeiro, nesta ordem)`; `Painel do Droid (visualização de peças)`; `Sistema de Item completo (Cura/Buff/Debuff/Equipável/ForaDeBatalha) + Gold`; `Cidade, Loja (compra/venda) e NPC`; `Trabalho de Editor pendente`; `Próximos passos imediatos (curto prazo, em ordem sugerida)`; `Amadurecimento do jogo (o "quadro geral")`; `Sensação de jogo`; `Conteúdo e variedade`; `Ensino/UX (crítico pro objetivo pedagógico do TCC)`; `Solidez técnica`; `Fora do escopo por enquanto (não deixar a IA empurrar isso)` |
| `LEIA_ISTO_PRIMEIRO.md` | 77 | `Entrega — Ato 1 jogável + trio de documentos reconciliado (15/09/2026)`; `O que tem aqui`; `Ordem de importação`; `Divergências do trio: 3 fechadas, 1 aberta`; `Dois bugs achados de brinde`; `O que não foi feito e por quê` |
| `GUIA_DE_MONTAGEM_ATO1.md` | 280 | `Guia de montagem do Ato 1 no Editor`; `Etapa 0 — Importar`; `Etapa 1 — O painel de diálogo (faça isto primeiro)`; `Etapa 2 — Ferrovale (cena City) — Sessão 2`; `2.1 Objeto de controle`; `2.2 Os três NPCs da fila`; `2.3 O comentário do Apollo que fecha a Sessão 2`; `2.4 O portão leste — o gate pedagógico`; `Etapa 3 — A saída leste e o primeiro combate (cena Game) — Sessão 3`; `Etapa 4 — A escolha de facção (cena Game) — Sessão 4`; `Etapa 5 — A Cratera (cena Game) — Sessões 5 e 6`; `Etapa 6 — Retorno e fim do Ato (cena City) — Sessão 7`; `Etapa 7 — Migrar a CasaLip (opcional, faça por último)`; `Lista de flags do Ato 1`; `Se der errado` |
| Arquivos com nome `TCC_-_ENREDO*`, `TCC_-_GDD*`, ou `GDD` | Não encontrados na enumeração recursiva do workspace | **NÃO VERIFICÁVEL** — nenhum arquivo com esses nomes foi encontrado para contar linhas ou listar títulos. |

## 9. Limitações da análise

| Item | Limitação |
|---|---|
| Resultado real de compilação | **NÃO VERIFICÁVEL** — requer abrir/executar o Unity Editor; não foi feito. |
| Backend fora do destino Android | **NÃO VERIFICÁVEL** — `ProjectSettings/ProjectSettings.asset` mostra apenas `Android: 1` em `scriptingBackend`; o README afirma Mono sem especificar essa configuração serializada por destino. |
| Unity Cloud | Os arquivos lidos têm identificadores de projeto e organização vazios e opções de vínculo desativadas; não foi consultado nenhum serviço externo. Fontes: `ProjectSettings/ProjectSettings.asset`; `ProjectSettings/UnityConnectSettings.asset`. |
| Referências de cena apontadas por fileID que não mapeou para GameObject visível | O ID foi registrado literalmente; o nome ou tipo específico do alvo não foi inferido. Fonte: respectivos arquivos `.unity`. |
| Papéis obrigatórios/opcionais de campos nulos | **NÃO VERIFICÁVEL** pelos YAMLs; os valores foram descritos apenas como referências nulas no arquivo, sem classificação de obrigatoriedade. |
| Tipos de assets cujo tipo Unity não aparece como texto no `.asset` | A classe do tipo pode depender de GUID e implementação de pacote externo; assets de configuração conhecidos foram nomeados com base nas referências de URP e nomes/tipos declarados nos arquivos. |
| Hierarquia serializada em instâncias de prefab e overrides | Algumas referências são armazenadas como overrides/instâncias e não como GameObjects YAML autônomos; nesses casos foi reportado o identificador de referência quando o nome do alvo não pôde ser associado. |
| Documentos de enredo ausentes | Nenhum `TCC_-_ENREDO`, `TCC_-_GDD` ou arquivo com nome `GDD` foi encontrado no workspace varrido. |
