# Contexto do projeto

**Revisão:** 17/10/2026, America/Sao_Paulo.

**Base Git:** `027952041d40e7957875a66641727684586455c1`, com alterações locais de documentação, runtime, cenas e assets da CasaLip.

**Branch da consolidação:** `docs/consolidar-contexto`.

Este é o documento principal do estado atual. Ele reúne informações de produto, arquitetura e integração. O [plano](plano-desenvolvimento.md) é responsável pelas tarefas; os [guias](guias/montagem-ato-1.md) descrevem procedimentos; o [histórico](historico/README.md) preserva versões anteriores.

**Retomada incremental:** o responsável definiu uma demo desenvolvida por partes, começando pela CasaLip. O [índice de trabalho](indice-trabalho.md) permite localizar assuntos sem reler todos os documentos; a [revisão da casa](revisoes/casalip.md) registra a inspeção atual, base Git e validações ainda pendentes.

## Como interpretar as evidências

| Evidência | O que permite afirmar |
|---|---|
| Código lido | O comportamento está implementado no arquivo indicado; a execução ainda pode depender da montagem. |
| Cena/prefab lido | Valores e referências estão serializados; instâncias, overrides e inicialização podem alterar o estado efetivo. |
| Observação MCP/API | O estado foi observado na sessão/data registrada; a matriz de 17/10 distingue chamadas por API de input real e não representa todas as partidas/builds. |
| Relato histórico do responsável | Houve uma validação anterior, com o escopo e a data preservados no registro. |
| Planejado ou não verificado | Não há evidência suficiente para declarar funcionamento atual. |

As correções da CasaLip estão implementadas, com validação parcial na [revisão de 17/10](revisoes/casalip-validacao-2026-10-17.md). Por API passaram introdução sem conclusão, rejeição de ausente/texto/90, sucesso por 100 na origem mundo com quatro falas e sem repetição, e duas revisitas com referências/porta corretas e save inalterado. Seis fixtures do harness passaram novamente. Captura 1280 legível e oito direções por API foram conferidas. Permanecem reinício/load, sucesso integrado por menu/laço/diálogo ocupado, 1920/narração/fala longa e input real; o usuário interrompeu Computer Use com Escape. Builds exigem validação própria. Travamento permanente do Player continua não reproduzido.

## 1. Visão e escopo

Droids Code é um RPG educativo, com exploração top-down em 8 direções, batalhas por turnos e programação em Lua fora de combate. Lip e Apollo atravessam o Ato 1 em Ferrovale; o terminal relaciona lógica de programação a puzzles, atributos e técnicas.

A direção narrativa vigente está no [Enredo v2](enredo/TCC_-_ENREDO_v2.md) e no [adendo](enredo/TCC_-_ENREDO_v2_ADENDO.md), lidos integralmente nesta retomada; o adendo prevalece nas divergências. O tema central é quem escreve quem, com erro como informação no Ato 1. Artemis executa uma especificação alterada; as rotas Flor de Ferro, Lótus Branca e Desgarrado produzem consequências políticas. Apollo é um entre 431 núcleos; sua individualidade nasce da convivência com Lip. A montagem deve preservar a ambiguidade das pistas antes da revelação posterior.

A proposta pedagógica introduz variáveis e laços na CasaLip, configuração de técnicas antes de sair da cidade e uso de condicionais em puzzles posteriores. A verificação atual lê valores finais de variáveis Lua: atingir `nivel_agua >= 100` não comprova, por si só, que o jogador usou um laço. A avaliação do aprendizado precisa considerar essa limitação.

| Momento pretendido | Conceito | Evidência mecânica disponível |
|---|---|---|
| Água na CasaLip | Variável e while | Valor final de nivel_agua; conclusão persistente/controlador local implementados, validação integrada em andamento. |
| Portão de City | Método, argumentos e custo | Quantidade de ações configuradas; orçamento inicial precisa ser validado. |
| Combate | Sequência e planejamento | Uso das técnicas preparadas fora da luta. |
| Energia na Cratera | Condicional e laço | Valores finais de energia e porta_aberta; montagem posterior ainda parcial. |

O conteúdo narrativo tem catálogos próprios. Os dois documentos de enredo agora estão disponíveis em `docs/enredo`; a indisponibilidade registrada na consolidação anterior deixou de se aplicar a eles. A implementação precisa ser conferida com essas fontes: conteúdo no catálogo não comprova montagem, e o guia ainda contém etapas herdadas, como entregar módulos de facção na Sessão 4 em vez da Sessão 5. Os Atos 2 e 3 orientam a campanha; o ponto final da demo ainda não foi definido.

## 2. Ambiente e execução

| Item | Evidência atual |
|---|---|
| Unity | `6000.5.8f1`, em [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt). |
| Renderização | URP 2D; configurações em [Assets/Settings](../Assets/Settings). |
| UI | uGUI e TextMeshPro. |
| Terminal | MoonSharp; dependência Git `upm/beta/v3.0` no [manifest](../Packages/manifest.json). |
| Input | `activeInputHandler: 2`; Player usa o Input legado, e EventSystem usa módulo do Input System. |
| MCP | `com.ivanmurzak.unity.mcp` consta no manifest. Erros históricos de conexão devem ser separados de erros de gameplay. |
| Plataformas pretendidas | Windows e WebGL. Build e compatibilidade não foram validados nesta consolidação. |
| Backend/API | Há valores serializados por destino em [ProjectSettings.asset](../ProjectSettings/ProjectSettings.asset). A antiga afirmação genérica de Mono não foi mantida como confirmação para todos os destinos. |
| Testes | Harness manual Editor em `Assets/Editor/ValidacaoCasaLip.cs`, sem NUnit/asmdef nem execução automática; resultados recentes em `green-harness-final.txt` e percurso por API em `fluxo-validacao.txt`, ambos em `Logs/CorrecaoCasaLip`. |

O código é C#. A documentação antiga adotava C# 9 como limite de sintaxe; a versão efetiva do compilador não foi medida. Abrir a cena pelo Editor permite inspecionar montagem, mas a validação completa deve partir também de `MainMenu`.

## 3. Estrutura real dos scripts

São **61 arquivos `.cs`** em `Assets/Scripts`, excluindo exemplos de TextMeshPro e o harness em `Assets/Editor`. A árvore abaixo descreve a estrutura existente, sem aplicar as reorganizações propostas na análise.

```text
Assets/Scripts/
├── Camera/ (1)
│   └── CameraFollow.cs
├── Casa/ (3)
│   ├── ControladorPuzzleAguaCasa.cs
│   ├── InteragirComTerminalCasa.cs
│   └── PuzzleAguaCasaChecker.cs
├── Catalogos/ (8)
│   ├── CatalogoDeDialogos.cs
│   ├── CatalogoDeInimigos.cs
│   ├── CatalogoDeItens.cs
│   ├── CatalogoDeLicoes.cs
│   ├── CatalogoDeLocais.cs
│   ├── CatalogoDeNpcs.cs
│   ├── CatalogoDePuzzles.cs
│   └── CatalogoDeQuests.cs
├── Combat/ (6)
│   ├── BattleManager.cs
│   ├── CombatEngine.cs
│   ├── EncounterZone.cs
│   ├── IParticipanteDeCombate.cs
│   ├── InimigoFixo.cs
│   └── ResultadoAcao.cs
├── Droid/ (7)
│   ├── Braco.cs
│   ├── Cabeca.cs
│   ├── Droid.cs
│   ├── DroidPart.cs
│   ├── DroidStats.cs
│   ├── Perna.cs
│   └── Tronco.cs
├── Estado/ (4)
│   ├── DadosDoJogo.cs
│   ├── GerenciadorDeEstado.cs
│   ├── ISistemaDeSalvamento.cs
│   └── SalvamentoJson.cs
├── Historia/ (9)
│   ├── ApolloDica.cs
│   ├── CondicaoDeHistoria.cs
│   ├── DialogoUIManager.cs
│   ├── GatilhoDeBatalha.cs
│   ├── GatilhoDeDialogo.cs
│   ├── GerenciadorDeQuests.cs
│   ├── NpcInterativo.cs
│   ├── QuestUIManager.cs
│   └── TrancaPorHistoria.cs
├── Menus/ (7)
│   ├── BagUIManager.cs
│   ├── LojaUIManager.cs
│   ├── MainMenuManager.cs
│   ├── MenuMundoManager.cs
│   ├── TelaDeDroidManager.cs
│   ├── TelaDeStatusManager.cs
│   └── TerminalUIManager.cs
├── Scripting/ (15)
│   ├── DroidScriptRunner.cs
│   ├── EfeitosTemporariosUtil.cs
│   ├── ItemConsumivel.cs
│   ├── LocalizadorDeTerminal.cs
│   ├── PlayerMovement.cs
│   ├── PontosDeProgressao.cs
│   ├── ProgressaoDeNivel.cs
│   ├── RestaurarPosicao.cs
│   ├── SistemaDeProgressao.cs
│   ├── TabelaDeCombate.cs
│   ├── TabelaDeCustos.cs
│   ├── TecnicaComposta.cs
│   ├── TerminalDroidApi.cs
│   ├── TipoAtributo.cs
│   └── TransicaoDeCena.cs
└── Verificadores/ (1)
    └── VerificadorDePuzzle.cs
```

`PlayerAnimatorController.controller` também está em `Assets/Scripts`; as animações novas da CasaLip ficam em `Assets/Animations`. A pasta `_Teste` está vazia de `.cs`. Os scripts usam tanto tipos globais quanto os namespaces `DroidsCode.DroidCore`, `DroidsCode.Combat`, `DroidsCode.Scripting` e `DroidsCode.Casa`. Nome de pasta e namespace não precisam coincidir; a padronização é uma tarefa futura.

`GatilhoDeQuest.cs`, `ControladorDeRoteiro.cs` e `_Teste/InimigoTeste.cs` não foram encontrados. `ItemConsumivel.cs` permanece como legado; a busca encontrou referências aos tipos antigos apenas no próprio arquivo e em comentários, enquanto BattleManager consome o catálogo e inventário atuais.

## 4. Cenas e navegação

As cinco cenas estão habilitadas em [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset). A ordem de build começa em MainMenu; o destino de Novo Jogo é um campo serializado.

```text
MainMenu → CasaLip ↔ City ↔ Game → Battle → Game
```

O retorno de Battle está atualmente fixado em `Game` pelo campo `nomeCenaMundo`. Um encontro iniciado futuramente em outra cena precisará de contexto de retorno próprio.

| Cena | Montagem lida | Limites da validação |
|---|---|---|
| [MainMenu](../Assets/Scenes/MainMenu.unity) | `nomeCenaJogo: CasaLip`; o default no script é Game. | Novo Jogo apenas carrega a cena; reset explícito de uma sessão existente precisa de revisão. |
| [CasaLip](../Assets/Scenes/CasaLip.unity) | Terminal, verificador/controlador locais, porta para City, Apollo e UI ajustada. Player mantém componentes/tag/colisores com arte/animações associadas. | Correções implementadas; matriz final em andamento. Travamento permanente não reproduzido. |
| [City](../Assets/Scenes/City.unity) | Dara, homem da fila, Tácio, comentário do Apollo, Loja, entrada da casa e portão para Game. Portão exige `ato1_sessao2` e técnica extra. | `ControleDeHistoria.regras` está vazio. Montagem de outras fases/NPCs não está concluída. |
| [Game](../Assets/Scenes/Game.unity) | Zona aleatória com `idDoInimigo` vazio, entrada de City e objeto `falaqualquer` usando `casa_intro_apollo`. | Sequência roteirizada das sessões posteriores não está montada integralmente. |
| [Battle](../Assets/Scenes/Battle.unity) | BattleManager, UI, listas, barras e valores de inimigo/recompensas no Inspector. | Restrições do catálogo e política de autosave precisam de integração; fundo tem registro de contorno histórico. |

Prefabs de UI existentes em [Assets/Prefabs](../Assets/Prefabs): `Canvas`, `PainelBag`, `PainelStatus`, `BotaoItemTemplate` e `BotaoAtaquePrefab`. O mapa `MetaValle.prefab` está em `Assets/Tilemaps`.

## 5. Sistemas e responsabilidades

### Droid, progressão e combate

- [Droid](../Assets/Scripts/Droid/Droid.cs) mantém atributos, HP, peças, técnicas e efeitos. Técnicas usam `Dictionary<string, TecnicaComposta>`; peças contribuem para atributos totais.
- [DroidPart](../Assets/Scripts/Droid/DroidPart.cs), Braco, Perna, Tronco e Cabeca representam dados de equipamento. Trocar uma peça substitui o slot; equipar pela Bag consome o item. Não há desequipar.
- PontosDeProgressao controla orçamento; ProgressaoDeNivel controla XP/nível; SistemaDeProgressao concede pontos após subir de nível.
- [CombatEngine](../Assets/Scripts/Combat/CombatEngine.cs) aplica dano por turno no atacante, verifica morte/Stun, delega a ação e decrementa efeitos do atacante. Usa [IParticipanteDeCombate](../Assets/Scripts/Combat/IParticipanteDeCombate.cs), implementado por Droid e InimigoFixo.
- [BattleManager](../Assets/Scripts/Combat/BattleManager.cs) reúne UI, corrotinas de turno, consumo de itens, montagem de inimigo, recompensas e transição. Já verifica a derrota dos dois participantes após ataques; a pendência antiga de detecção atrasada não descreve o código atual.
- InimigoFixo possui uma ação fixa em C#. O catálogo varia valores e drops; variedade de comportamento ainda é limitada.

HIT/FLEE, crítico por LUK e resistência por INT estão implementados. O domínio usa `UnityEngine.Random` em Droid; portanto, não é inteiramente independente da engine. Listas de efeitos são mutáveis e não há limpeza centralizada entre batalhas nem regra final de empilhamento.

### Terminal e puzzles

[DroidScriptRunner](../Assets/Scripts/Scripting/DroidScriptRunner.cs) cria a VM MoonSharp com `Preset_SoftSandbox` e expõe somente [TerminalDroidApi](../Assets/Scripts/Scripting/TerminalDroidApi.cs). Uma nova sessão após fechar/reabrir o terminal cria nova VM; chamar `Abrir` enquanto ele já está aberto preserva a VM/rascunho e não incrementa novamente o contador. Variáveis Lua não substituem o estado persistente do puzzle. A execução é síncrona e não possui limite contra laços infinitos (`TER-001`).

Comandos de jogador presentes: `subirAtributo`, `aprenderTecnica`, variantes com Veneno/Stun, `melhorarTecnica`, `esquecerTecnica`, `obterAtributo`, `listarTecnicas`, `obterPontosDisponiveis` e `testeAdicionarPontos`. O último é um comando de teste exposto, sem o custo normal. `Explicar`, `Ajuda` e `Resumo` não existem na API, embora CatalogoDeLicoes já tenha conteúdo.

[TerminalUIManager](../Assets/Scripts/Menus/TerminalUIManager.cs) controla campo, log, histórico, pontos e evento `AoExecutarCodigo`. `EscreverNoLog` existe; abertura ativa o painel antes do log e a rolagem não inicia coroutine em componente inativo. O [relato de coroutine](problemas/terminal-log-painel-inativo.md) guarda o diagnóstico anterior; a confirmação final de mensagem/rolagem por ambas as origens permanece na matriz. `DefinirContextoPuzzle` apresenta enunciado/código inicial quando cabível; limpar o contexto correspondente restaura ajuda geral mantendo rascunho global.

[VerificadorDePuzzle](../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs) lê um Id do catálogo, suporta múltiplas condições, dicas após tentativas e progresso de quest, mantendo fallback manual. `AssociarTerminal` remove a inscrição anterior e assina a nova uma vez; `LocalizadorDeTerminal.Encontrar` inclui inativos e prefere UI persistente válida. A referência perdida pode ser recuperada. `flagAoResolver` grava conclusão; `flagQueJaResolve` restaura efeitos sem save/progresso/evento. `AoResolverPelaPrimeiraVez` é emitido após persistir resolução nova; `mostrarFalaAoResolver` controla a fala direta.

CasaLip usa um verificador local `p_agua_casa` em `ControlePuzzleAgua` e [ControladorPuzzleAguaCasa](../Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs). O controlador aplica o contexto, fecha terminal/origem do menu e apresenta `casa_porta_apollo` na primeira resolução; aguarda uma conversa ocupada sem substituir seu callback. O checker legado foi removido da cena, mas seu script permanece no repositório. O [relato de retorno](problemas/casalip-retorno-porta-trancada.md) preserva a causa anterior.

### UI e interação

[MenuMundoManager](../Assets/Scripts/Menus/MenuMundoManager.cs) persiste o Canvas raiz, remove duplicatas e abre Status/Terminal/Bag/Droid. Opções é placeholder. Battle e MainMenu estão bloqueadas para abertura do Menu do Mundo. O botão de missões ainda não existe.

Os painéis usam callbacks `AoFechar`, mas o registro de menus não é uniforme: Terminal e Diálogo registram a própria abertura; a Loja usa NpcInterativo; Status/Bag/Droid dependem do menu que os abriu. O menu agora distingue propriedade lógica de visibilidade e só retorna de um filho que abriu; fechar terminal originado no mundo não revela o menu. `FecharParaDialogo` limpa essa origem antes do fechamento do terminal. Os demais painéis e caminhos de load exigem seus próprios testes.

A [coleta visual de 16/10](problemas/casalip-apresentacao-dialogo.md) confirmou âncoras invertidas, texto reduzido e assets anteriores. Em 17/10, a montagem refez o layout escalável, habilitou nome, encaixou retrato e associou a arte disponível de Lip/Apollo; Apollo usa idle LPC provisório, com etiqueta no mundo. A captura final 1280 foi considerada legível pelo agente principal; quatro direções/quatro diagonais e flip LPC/legado passaram por API. Faltam 1920/narração/fala longa e input real/física para encerrar `UI-003/VIS-001`. `casa_porta_apollo` passou pelo mundo/API; `HIS-002` permanece parcial.

[PlayerMovement](../Assets/Scripts/Scripting/PlayerMovement.cs) move por velocidade do Rigidbody2D, atualiza Animator e bloqueia input quando `MenuAberto`. A opção `usarEspelhamentoHorizontal` permite desativar flip quando o controlador tem artes próprias por direção. A abertura repetida defeituosa foi reproduzida e corrigida, sem comprovar que fosse a causa do travamento permanente relatado.

## 6. Estado persistente e objetos de cena

| Duração | Responsáveis | Regra de integração |
|---|---|---|
| Sessão inteira | GerenciadorDeEstado: Droid, inventário, Gold, flags, contadores e contexto transitório. | Estado semântico deve sobreviver à troca de cena. |
| Sessão inteira | Canvas mantido por MenuMundoManager, incluindo terminal e painéis. | Não depender indefinidamente de referências para objetos locais destruídos. |
| Cena | Player, portas, NPCs, triggers, geometria e controladores locais. | Recuperar o estado persistente e localizar dependências válidas ao carregar. |
| Abertura do terminal | VM Lua e API da sessão. | O valor de uma variável Lua não substitui a flag de conclusão persistente. |
| Arquivo salvo | DadosDoJogo/SalvamentoJson. | Só campos serializados são restaurados após reiniciar a aplicação. |

Fluxos atuais:

1. **Transição:** TransicaoDeCena grava posição de chegada e nome de destino → carrega cena → RestaurarPosicao usa coordenadas de mundo se o destino corresponde → limpa posição pendente.
2. **Puzzle:** execução Lua → evento → verificação → efeitos de vitória → flag/progresso → save → evento de primeira resolução. A CasaLip usa esse fluxo; restauração pela flag reaplica efeitos sem repetir persistência/evento.
3. **Diálogo:** interação/trigger → condições → falas do Inspector ou catálogo → painel → escolha/flag → save. Falas do Inspector têm prioridade; escolhas ainda são configuradas no gatilho.
4. **Batalha:** encontro/gatilho grava inimigo e posição de origem → Battle monta inimigo → turnos → XP/Gold/itens e flag → retorno. Fuga e destino de retorno ainda têm diferenças em relação ao fluxo de finalização.

Na observação MCP de 15–16/10, o checker persistente estava resolvido e sem porta válida, e TerminalCasa estava sem referência ao terminal. O Player estava habilitado, contador de menus em zero e `Time.timeScale = 1`. Não foram encontrados scripts ausentes nos objetos carregados daquela sessão; isso não equivale a verificar todos os assets.

## 7. Salvamento

[SalvamentoJson](../Assets/Scripts/Estado/SalvamentoJson.cs) grava `Application.persistentDataPath/save.json` com JsonUtility. [DadosDoJogo](../Assets/Scripts/Estado/DadosDoJogo.cs) usa versão **6** e listas planas para dados que em runtime são dicionários.

`MigrarDados` roda em memória antes de restaurar flags: versões anteriores a 6 com `ato1_casa_resolvida=true` preservam a conclusão e recebem `ato1_falou_apollo_casa=true`. A flag antiga era ambígua; conservar progresso também conserva a conclusão de legados que só conversaram. v6+ não sofre essa conversão e os demais dados ficam intactos. A fixture em memória passou; isso não substitui reinício/load do save real.

Ao encerrar a validação parcial de 17/10, o agente principal restaurou o save original via MCP e confirmou SHA-256 igual ao backup `Logs/CorrecaoCasaLip/save-original.json`: `F9018A19EFE54652D201B02BF161E7C808D3B4DC1F2FCFAF025B29F74322ABC7`.

Inclui atributos, HP/HP base, pontos, nível/XP, técnicas com efeitos configurados, flags, contadores de missão, inventário, Gold, cena e posição. Carregar modifica o Droid existente, restaura estado, agenda posição, zera o contador de menus e carrega a cena salva.

Lacunas verificáveis no código:

- Slots de peças equipadas não estão em DroidSalvo; equipamento consumido pela Bag pode ser perdido ao reiniciar e carregar.
- Autosave em Battle grava a cena Battle e posição zero se não há objeto com tag Player, sem serializar todo o combate ou o retorno.
- Comprar/vender na Loja altera memória, sem chamar Salvar nesses métodos.
- Zerar o contador durante load não sincroniza automaticamente todas as flags internas dos painéis persistentes.
- Há migração específica da CasaLip para v6; os demais problemas de esquema/persistência não foram resolvidos por essa conversão.

## 8. Conteúdo, NPCs e progressão narrativa

| Catálogo | Conteúdo contado no código | Integração atual |
|---|---|---|
| [Quests](../Assets/Scripts/Catalogos/CatalogoDeQuests.cs) | 18 missões | Motor e diário existem; falta oferta/entrega via NPC, botão do menu e ligação de eventos. |
| [NPCs](../Assets/Scripts/Catalogos/CatalogoDeNpcs.cs) | 16 definições de personagens | Locais e rotinas são dados. Não há controlador executando horários/deslocamentos. |
| [Locais](../Assets/Scripts/Catalogos/CatalogoDeLocais.cs) | 19 definições | Descrição e conexões não significam 19 cenas montadas. |
| [Inimigos](../Assets/Scripts/Catalogos/CatalogoDeInimigos.cs) | 8 definições | Battle usa valores/drops; não consulta PermiteFugir/EhUnico/EhChefe para todas as regras. |
| [Puzzles](../Assets/Scripts/Catalogos/CatalogoDePuzzles.cs) | 10 definições | Verificador v2 existe; montagem efetiva dos puzzles extras não foi confirmada. |
| [Lições](../Assets/Scripts/Catalogos/CatalogoDeLicoes.cs) | 9 tópicos | Explicar/ListarTopicos existem no catálogo, sem métodos correspondentes na API Lua. |
| [Diálogos](../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs) | 63 entradas | Cobre conteúdo do Ato 1; só parte dos gatilhos está montada nas cenas. |
| [Itens](../Assets/Scripts/Catalogos/CatalogoDeItens.cs) | 6 categorias | Cura, Buff, Debuff, Equipável, ForaDeBatalha e Chave; UI não consome todos os metadados. |

[NpcInterativo](../Assets/Scripts/Historia/NpcInterativo.cs) tem tipos Generico, Loja e Dialogo. Papéis como Estalagem/Ferreiro no catálogo não correspondem a painéis implementados. Para NPC com NpcInterativo de diálogo/loja, o gatilho deve ter uma única origem de disparo; a configuração recomendada é `ApenasPorScript` quando ele for chamado pelo componente de interação.

[GerenciadorDeQuests](../Assets/Scripts/Historia/GerenciadorDeQuests.cs) oferece aceitar, atualizar automáticas, registrar progresso e entregar. Conversa e vitória são mencionadas nos comentários, mas GatilhoDeDialogo e BattleManager não chamam RegistrarProgresso. VerificadorDePuzzle chama para resolução. A atualização automática aparece na abertura do diário e após entrega; o controlador de roteiro citado nos comentários não existe.

Flags-chave de montagem: `ato1_falou_apollo_casa`, `ato1_casa_resolvida`, `ato1_falou_dara`, `ato1_falou_fila`, `ato1_sessao2`, `ato1_apollo_explicou`, `ato1_sessao3`, `ato1_faccao_escolhida`, as três flags de facção, `ato1_puzzle_energia`, `ato1_ivo_confessou`, `ato1_sessao6` e `ato1_concluido`. São case-sensitive. Na CasaLip, a introdução grava e é impedida por `ato1_falou_apollo_casa`; o verificador grava `ato1_casa_resolvida`. Saves legados seguem a política v6 descrita acima.

## 9. Fórmulas e decisões vigentes

As fórmulas abaixo foram reconciliadas com Droid e as tabelas locais. Os valores são de desenvolvimento, sujeitos a playtesting.

| Regra | Implementação atual |
|---|---|
| Dano do Droid | `max(1, FOR_total * 2 + NivelDeDano * 5 - Defesa_alvo)`; crítico multiplica por 2 após acerto. |
| Defesa | VIT total, incluindo peças e efeitos. |
| HP máximo | `HpMaxBase + floor(HpMaxBase * VIT_total * 0.01)`. O método de bônus fixo de HP de peças retorna zero; peças de VIT afetam o percentual. |
| Acerto | `Clamp(50 + HIT_atacante - FLEE_alvo, 5, 95)`; HIT deriva de DEX, FLEE de AGI. |
| Crítico | 1% por LUK, rolado depois do acerto. Não garante acerto de um ataque que errou. |
| INT | 1% de resistência a efeito e 1% de bônus de cura por ponto. |
| XP e pontos | Próximo nível exige `Nivel * 100`; subir concede 5 pontos. |
| Custos | Atributo: 1/unidade; dano de técnica: 1/nível; Stun: 3/turno; veneno: 2/turno. O custo do veneno não multiplica pelo dano por turno. |

Fontes: [TabelaDeCombate](../Assets/Scripts/Scripting/TabelaDeCombate.cs), [TabelaDeCustos](../Assets/Scripts/Scripting/TabelaDeCustos.cs) e [TecnicaComposta](../Assets/Scripts/Scripting/TecnicaComposta.cs). `CustoResistenciaPorNivel` continua declarado sem uso encontrado.

Decisões preservadas das entregas anteriores:

- Terminal Lua fora de batalha; cálculo de combate em C#.
- Acesso do jogador ao Droid por métodos validados da fachada.
- Menu fixo de combate; técnicas configuradas são dados para a lista de ações.
- CombatEngine depende do contrato do participante, sem ramificação por tipo concreto.
- Peças por composição; herança com Cartuchos de Código é design futuro.
- Identificadores de catálogo e chaves de save precisam de migração se forem alterados.
- Regra de empilhamento de efeitos ainda depende de decisão de design.
- Modo Puzzle separado, ScriptableObjects/Factory e desequipar permanecem propostas sem implementação aprovada nesta revisão.

## 10. Manutenção da documentação

Esta seção descreve o processo de manutenção; as seções anteriores descrevem o projeto. Pedidos atuais do responsável definem o escopo do trabalho. Procedimentos de importação de pacotes antigos ficam como histórico. Preferências de colaboração são preservadas abaixo; a migração documental não redefine decisões de gameplay.

- Após mudança relevante, atualizar a seção responsável pelo comportamento, a tarefa correspondente no plano e o guia afetado. Os outros arquivos usam links, sem repetir o conteúdo inteiro.
- Registrar data e fonte de verificação; usar método/campo além do caminho quando uma conclusão depender de código específico.
- Separar decisão aprovada, implementação existente, hipótese e proposta de refatoração.
- Acesso local e MCP substituem a antiga premissa de trabalhar apenas com arquivos colados no chat. Ler os arquivos reais antes de afirmar consistência.
- Preservar as preferências registradas: ler integralmente os arquivos fornecidos para a tarefa, agrupar dúvidas necessárias, descrever configuração de Unity com campos e passos concretos e distinguir hipóteses de resultados. O histórico solicita ZIP com caminhos reais em entregas de código; esta tarefa é documental e não produz um pacote de scripts.
- Convenções de código registradas: português para novos nomes de domínio, PascalCase para tipos/métodos públicos e camelCase para campos privados. Nomes e namespaces existentes só mudam em refatoração específica, com verificação de referências.
- Toda alteração de código/cena exige validação adequada; ausência de erro no Console não comprova todo o percurso.
- Nomes de documentação: minúsculas, sem espaços/acentos, separados por hífen; exceção `README.md`. Links relativos com `/` e capitalização exata dos alvos.
- Histórico é material de consulta, preservado sem reconciliação retroativa. O [índice histórico](historico/README.md) registra origem e divergências superadas.

As prioridades e critérios de conclusão estão exclusivamente no [plano de desenvolvimento](plano-desenvolvimento.md).
