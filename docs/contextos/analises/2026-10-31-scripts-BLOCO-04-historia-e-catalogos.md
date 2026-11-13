# Auditoria de scripts C# — BLOCO 04: História e Catálogos

**Escopo:** leitura estática integral de 20 scripts próprios de `Assets/Scripts/Historia/` e `Assets/Scripts/Catalogos/`, inclusive os catálogos extensos de diálogos, locais, NPCs e quests. Consultei documentação e relatórios anteriores nos trechos relevantes. Não executei testes, compilei, abri Unity, rodei o jogo ou usei MCP. Nenhum arquivo existente foi alterado.

**Baseline:** auditoria geral de 30/10 e relatórios dos BLOCOS 01–03. Achados prévios são listados separadamente, sem reapresentá-los como novos.

## 1. Inventário — leitura integral

| Arquivo | Linhas | Leitura integral | Função |
|---|---:|---:|---|
| `Assets/Scripts/Historia/ApolloDica.cs` | 126 | Sim | Dicas contextuais do Apollo. |
| `Assets/Scripts/Historia/CondicaoDeHistoria.cs` | 94 | Sim | Condições de ativação baseadas em flags. |
| `Assets/Scripts/Historia/DialogoUIManager.cs` | 560 | Sim | Exibe falas e escolhas, consequências, callbacks e fechamento da UI. |
| `Assets/Scripts/Historia/EscalaLocalDeDialogo.cs` | 38 | Sim | Configura escala local associada ao diálogo. |
| `Assets/Scripts/Historia/GatilhoDeBatalha.cs` | 143 | Sim | Gatilho de batalha condicionado por história. |
| `Assets/Scripts/Historia/GatilhoDeDialogo.cs` | 335 | Sim | Condições, exibição de diálogo, flags e callbacks. |
| `Assets/Scripts/Historia/GerenciadorDeQuests.cs` | 342 | Sim | Aceite, progresso, entrega e atualização de quests. |
| `Assets/Scripts/Historia/NpcInterativo.cs` | 204 | Sim | Roteamento de interações com NPCs. |
| `Assets/Scripts/Historia/OrientacaoNpc.cs` | 54 | Sim | Orientação de NPC durante interação. |
| `Assets/Scripts/Historia/QuestUIManager.cs` | 220 | Sim | Apresenta quests e objetivos na UI. |
| `Assets/Scripts/Historia/TrancaPorHistoria.cs` | 122 | Sim | Bloqueia/desbloqueia por condições de história. |
| `Assets/Scripts/Catalogos/CatalogoDeDialogos.cs` | 894 | Sim | Falas e escolhas narrativas indexadas por diálogo. |
| `Assets/Scripts/Catalogos/CatalogoDeInimigos.cs` | 303 | Sim | Dados e características dos inimigos. |
| `Assets/Scripts/Catalogos/CatalogoDeItens.cs` | 456 | Sim | Definições, preços e propriedades de itens. |
| `Assets/Scripts/Catalogos/CatalogoDeLicoes.cs` | 262 | Sim | Catálogo e normalização de conceitos de ensino. |
| `Assets/Scripts/Catalogos/CatalogoDeLocais.cs` | 552 | Sim | Dados, descrições e referências de locais. |
| `Assets/Scripts/Catalogos/CatalogoDeNpcs.cs` | 582 | Sim | Elenco, biografias, rotinas, referências e vozes de NPCs. |
| `Assets/Scripts/Catalogos/CatalogoDePuzzles.cs` | 347 | Sim | Enunciados, soluções e verificações de puzzles. |
| `Assets/Scripts/Catalogos/CatalogoDeQuests.cs` | 600 | Sim | Definições, objetivos, pré-requisitos e recompensas de quests. |
| `Assets/Scripts/Catalogos/CatalogoVisualDeItens.cs` | 42 | Sim | Associação visual de itens. |

## 2. Estado por sistema

- **Diálogos:** `DialogoUIManager` apresenta falas/escolhas e callbacks; `GatilhoDeDialogo` liga o resultado a flags, save e callbacks externos. A configuração efetiva de painéis, textos e falas em cena não foi examinada.
- **Quests:** `GerenciadorDeQuests` processa aceite, progresso e entrega com base em `CatalogoDeQuests` e em produtores de eventos. Nem todo caminho de diálogo registra conversa ou atualiza automaticamente as quests.
- **Puzzles e ensino:** `CatalogoDePuzzles` associa enunciados, verificações de variáveis e soluções ao runner Lua. A auditoria encontrou uma verificação que não comprova integralmente o resultado pedagógico esperado.
- **Catálogos:** NPCs, locais, itens, inimigos, lições, diálogos e quests fornecem dados. Sua existência não comprova que estejam montados ou usados em cenas.

## 3. Achados novos

### 🟡 Importante, condicionado à configuração — callback pode concluir diálogo que não abriu

- **Arquivos/linhas:** [DialogoUIManager.cs:215–219](../../Assets/Scripts/Historia/DialogoUIManager.cs); [GatilhoDeDialogo.cs:252–263](../../Assets/Scripts/Historia/GatilhoDeDialogo.cs); [GatilhoDeDialogo.cs:280–296](../../Assets/Scripts/Historia/GatilhoDeDialogo.cs).
- **Evidência:** quando painel ou texto obrigatório está ausente, `Mostrar` invoca `aoConcluir` e retorna `false`. O gatilho passa `AoConcluirDialogo` como callback, mas retorna ao receber `false`; o callback pode gravar `flagAoTerminar`, salvar e executar outro callback.
- **Impacto/certeza:** se o diálogo estiver mal configurado, ele pode não aparecer e ainda assim ser tratado como concluído, inclusive desativando o gatilho ou alterando a progressão. A montagem real das cenas não foi inspecionada.
- **Causa provável:** o caminho de falha de configuração usa o mesmo callback semântico de conclusão real.
- **Sugestão (não aplicada):** não invocar o callback de conclusão no caminho de falha; separar falha de abertura e conclusão efetiva da UI.

### 🟡 Importante — a quest de Tácio pode desbloquear antes da terceira entrega

- **Arquivos/linhas:** [CatalogoDeQuests.cs:184–209](../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs); [GerenciadorDeQuests.cs:269–276](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs); [Missões wiki:136–149](../../wiki/Missoes.md).
- **Evidência:** `q_cobre_tacio` é repetível e grava `quest_tacio_cobre_ok` ao concluir uma entrega. `q_tacio_linha` exige essa flag, embora seu resumo diga “Depois da terceira entrega”. A wiki registra o mesmo requisito narrativo e a disponibilidade técnica pela flag.
- **Impacto:** pelo código atual, a quest da linha pode ser liberada após uma única conclusão; não há contador de entregas impondo três.
- **Causa provável:** a dependência usa uma flag binária, não a contagem necessária à regra narrativa.
- **Sugestão (não aplicada):** se a terceira entrega é necessária, contar conclusões e liberar apenas ao atingir o marco; alternativamente, ajustar o texto narrativo para refletir o comportamento atual.

### 🟢 Baixa prioridade — verificação de “Calibragem do braço” não confirma o dobro

- **Arquivo/linhas:** [CatalogoDePuzzles.cs:156–168](../../Assets/Scripts/Catalogos/CatalogoDePuzzles.cs).
- **Evidência:** o enunciado pede ler Força e guardar o dobro em `previsao`; a solução de referência faz isso. As verificações exigem somente que `forca_lida` e `previsao` sejam positivos.
- **Impacto:** valores positivos arbitrários, sem relação de dobro, podem cumprir as verificações e exibir a fala de vitória.
- **Causa provável:** a condição valida presença/positividade, mas não a relação matemática solicitada.
- **Sugestão (não aplicada):** verificar `previsao == forca_lida * 2` e, se a API permitir, confirmar que `forca_lida` corresponde ao atributo atual. Se não for possível verificar a leitura da API, alinhar o objetivo pedagógico ao que o verificador pode provar.

## 4. Achados prévios e aprofundamentos — não reclassificados

- **Progresso de quests por derrota:** achado prévio. A auditoria geral registra objetivos `Derrotar` sem produtor localizado no encerramento de vitória; o BLOCO 01 confirmou que `FinalizarBatalha` não chama diretamente `RegistrarProgresso`. Referências: [auditoria geral](2026-10-30-auditoria-geral-projeto.md) e [BLOCO 01](2026-10-31-scripts-BLOCO-01-combate.md).
- **Conversas e atualização automática de quests:** o plano MIS-01 ainda lista integrações pendentes. A leitura mostra que `MoradorInterativo` registra conversa e atualiza quests ([MoradorInterativo.cs:73–78](../../Scripts/Interiores/MoradorInterativo.cs)), enquanto o callback de `GatilhoDeDialogo` não faz isso por si só ([GatilhoDeDialogo.cs:280–296](../../Assets/Scripts/Historia/GatilhoDeDialogo.cs)). O progresso depende do caminho de interação usado; sem montagem de cenas não foi possível determinar quais diálogos têm essa integração.
- Preço de venda, validação de chefes/únicos em `EncounterZone`, efeitos entre batalhas e possível duplicação de recompensas continuam nos termos dos relatórios prévios; os respectivos fluxos não foram reabertos neste bloco. Efeitos entre batalhas e duplicação de recompensas permanecem hipóteses, não defeitos confirmados.

## 5. Divergências documentais

- **Cronologia de Ivo/Célia:** o catálogo diz “Vinte dias depois” ([CatalogoDeNpcs.cs:192](../../Assets/Scripts/Catalogos/CatalogoDeNpcs.cs)); a wiki define remendo em 18/03/2060 e Corte em 29/03/2060, onze dias depois ([Historia wiki:19, 30–31](../../wiki/Historia.md)). Se o texto do catálogo se refere a esse mesmo remendo, a duração está divergente.
- **Proteção contra laço infinito:** comentário em [CatalogoDePuzzles.cs:202](../../Assets/Scripts/Catalogos/CatalogoDePuzzles.cs) afirma que MoonSharp não protege contra laço infinito, mas `DroidScriptRunner` implementa coroutine e limites de instruções/retomadas ([DroidScriptRunner.cs:17, 60–80](../../Assets/Scripts/Scripting/DroidScriptRunner.cs)). O comentário parece desatualizado; isso não demonstra ausência de proteção no runner atual.
- **Narrativa de Tácio:** divergência entre “terceira entrega” e flag liberada na primeira conclusão, detalhada no achado acima.

## 6. Dependências, acoplamentos e intenções ⚪

- Diálogos conectam `DialogoUIManager`, `GatilhoDeDialogo`, flags/save, callbacks externos e, em alguns caminhos, `MoradorInterativo`. A UI também depende de painel, texto e referências configuradas.
- Quests dependem de `CatalogoDeQuests`, estado/save, inventário, progressão e produtores de eventos. Atualização automática não deve ser presumida para qualquer mudança de flag.
- Puzzles conectam `CatalogoDePuzzles` ao runner Lua, às verificações de variáveis e ao registro de progresso.
- Catálogos de locais e NPCs descrevem dados, rotinas e sugestões, mas não comprovam montagem em cena nem quais falas são efetivamente usadas quando o Inspector fornece falas próprias.
- ⚪ O uso de um valor dinâmico de Força no puzzle “Calibragem do braço” é intencional; a lacuna está na verificação da relação calculada, não na ausência de um valor esperado fixo.

## 7. Limitações e próximo passo

Análise estática apenas: valores serializados em cenas, prefabs e Inspector não foram examinados; fluxos em runtime não foram confirmados. Os documentos relacionados foram consultados nos trechos relevantes, não integralmente. Próximo: BLOCO 05 — menus, interiores, casa, câmera e verificadores.
