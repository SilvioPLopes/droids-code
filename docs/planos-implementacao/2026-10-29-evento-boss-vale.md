# Evento do boss no vale — lógica e combate

**Estado/data:** código e prefab preparados, compilados e validados por API em 29/10/2026; posicionamento, input real e visual em jogo pendentes. Pedido atual autoriza preparar evento sem posicionar, boss forte e técnicas que incentivem upgrades/compilação. Interpretação conservadora de “flash”: sem diálogos ou flags narrativas novas; esclarecimento apresentado ao usuário.
**Objetivo:** prefab de evento pronto para futura colocação perto do rio em `Game` (vale), usando o gatilho existente e o ID `droid_do_ivo`; combate próprio e arte v2 em runtime. Não altera o cânone da Estação Sete: localização de demonstração é montagem técnica futura, não reescrita do roteiro.
**Escopo permitido:** novos `Assets/Prefabs/EventoBossVale.prefab`/meta; `Assets/Scripts/Combat/DroidDoIvo.cs`/meta; `Assets/Editor/ValidacaoBossVale.cs`/meta; cópia nova da arte v2 em `Assets/Resources/Inimigos/BossIvo-v2.png` e metas necessárias; alterações locais em `InimigoFixo.cs` (métodos virtuais), `BattleManager.cs` (construção/introdução do boss), `ApresentacaoInimigoBattle.cs` (recurso do boss sem mudar cena), `CatalogoDeInimigos.cs` (somente stats do boss), contrato de combate e links neste plano/índice/plano de arte. Evidências e backups em `output/boss-vale/2026-10-29/`.
**Preservar:** cenas em disco e abertas, transform/posição, rio/mapa, Canvas/Scaler, arte A/B/C, diálogos, flags, recompensas, formatos de save, pacotes/infra e trabalho alheio. Não executar entrada real de batalha porque grava checkpoint. Não iniciar Play.
**Consumidores compartilhados:** BattleManager usa InimigoFixo; fixtures existentes fazem cast para esse tipo, por isso boss será subclasse compatível. CombatEngine mantém contrato/turnos/dano e efeitos, sem edição. ApresentacaoInimigoBattle é usada por Battle e prefab visual; recurso por ID permite boss nos dois sem sobrescrever cena dirty. GatilhoDeBatalha já atende encontros roteirizados; prefab o reutiliza sem alterar o componente compartilhado.
**Estado observado:** MCP confirmou projeto correto, Battle fora de Play e dirty. Não salvar, reabrir ou descartar a cena. Trabalhar no checkout atual para preservar montagem e alterações locais em andamento; nenhuma worktree solicitada. Compilação de scripts e fixtures sem saves são permitidas; não serializar a cena aberta.
**Valores anteriores/reversão:** boss HP95/ATK13/DEF5/Hit14/Flee6/crit8%/resist5%; recompensas e IDs preservados. Antes de editar, copiar arquivos existentes do escopo para pasta de baseline e registrar hashes. Reverter somente hunks desta tarefa se solicitado, comparando com baseline e mudanças posteriores; não substituir arquivo inteiro que tenha trabalho novo de terceiros. Novos arquivos podem ser removidos isoladamente. Saves não serão tocados, sem necessidade de backup de save.

## Desenho do comportamento

- Evento: prefab neutro na origem, trigger unitário, Player/E, destino Battle, `droid_do_ivo`; nenhum requisito, bloqueio ou flag de vitória. A entrada existente grava checkpoint/retorno. Sem flag, evento poderá ser repetido ao voltar e apertar E; não prometer unicidade persistida. Posicionamento e trava narrativa ficam explicitamente pendentes.
- Boss: balanceamento inicial HP200/ATK18/DEF10/Hit22/Flee4/crítico5%/resistência25%, sem fuga. Esquiva baixa evita transformar robustez em erros excessivos. A proposta inicial HP210 foi revisada pelo resultado descrito abaixo.
- Ciclo: **Prensa Hidráulica** (ATK18), **Pressurizar** (sem dano e anuncia corte), **Corte de Contenção** (ATK30, no acerto aplica Vazamento2 por2 turnos se alvo não resistir), **Descompressão** (sem dano, janela de recuperação). Próxima intenção aparece no log; ações bloqueadas por stun não avançam o ciclo. Não executa Lua durante o combate; compilação prepara técnicas antes da entrada.
- Sem resistência adaptativa nova nesta etapa; técnicas próprias/ciclo não equivalem à mecânica futura de adaptação do roteiro.
- Arte: usar a v2 já gerada por recurso carregado somente para esse ID, sem associar C ao boss. Seleção explícita de `visuais` tem prioridade se houver arte cadastrada. Enquadramento e medo continuam sujeitos a aprovação visual futura.

## Aceite e validação mínima

1. Prefab possui ID, trigger, E e cena corretos, vazio de flags e sem instância no mapa.
2. Boss percorre quatro ações, anuncia carga, respeita stun, dano/esquiva/defesa/resistência e aplica efeito somente quando acerta e alvo não resiste. Entrada inválida não avança ciclo.
3. Fixture com seeds fixos compara 200 lutas: Apollo inicial FOR5/VIT3/HP30/base versus preparado FOR20/VIT15/DEX20/AGI10 com técnica nível3. Preparado deve apresentar vantagem clara (meta >=70% de vitórias; inicial <=5%). É simulação de atributos/técnica real, não comprova disponibilidade dessas melhorias no percurso ou input do jogador.
4. Seleção da subclasse pelo ID na Battle e carregamento da imagem do boss; A/B mantêm tipo/atributos anteriores. Sem save, recompensas ou execução de gatilho real.
5. Diferenciar código editado, compilação, fixture por API e validação visual/input pendentes. Teste verde não certifica balanceamento final.

## Execução

**Primeira evidência de dificuldade:** ciclo, ações inválidas, stun/carga, dano e efeitos passaram; preparado venceu136/200, abaixo da meta140. Causa do limite identificada na fórmula: FOR20/nível3 contra DEF10 dá45 por técnica; básico dá35. Três técnicas e dois básicos somam205; HP210 força mais um acerto mesmo nesse conjunto. Única alteração desta rodada: HP200 para permitir esse conjunto de preparação, mantendo ataque/defesa/ciclo/chances. Verificar novamente sem alterar seeds ou meta de aceite. Não é balanceamento final por input real.

- [x] Registrar baseline; criar fixture e observar falha por ausência de `DroidDoIvo` no runtime antes da implementação.
- [x] Implementar subclasse e integração mínima; configurar prefab e recurso visual sem editar cenas.
- [x] Executar fixture isolada e regressão A/B pertinente; conferir diff e preservação da cena dirty.
- [x] Registrar resultados, limites e ponto de ligação perto do rio.

## Ponto de ligação futura — vale / perto do rio

**Artefato preparado:** [EventoBossVale.prefab](../../Assets/Prefabs/EventoBossVale.prefab). Não existe instância nova no mapa. Na montagem futura, instanciar esse prefab em `Game` junto ao acesso escolhido perto do rio e ajustar somente seu trigger; não há coordenada arbitrada nesta tarefa. Usa o fluxo existente `GatilhoDeBatalha` → tecla E dentro do trigger → `Disparar()` → validação da cena Battle → `TentarPrepararBatalha` com checkpoint/retorno → carregar Battle com `ProximoInimigoId=droid_do_ivo`.

Não colocar em `EncounterZone.encontros`: chefe permanece fora da tabela aleatória. Não chamar `Disparar()` no teste desta tarefa, pois gravaria checkpoint. Para eventual chamada por diálogo/UnityEvent, reavaliar guardas de menu/história do chamador; o fluxo preparado aqui é a interação E do gatilho existente.

Sem flag de vitória nova, a instância pode ser usada novamente após voltar. Definir unicidade/trava narrativa e ligação com Ivo é trabalho posterior. Não confundir `EhUnico` de catálogo com persistência implementada para este prefab.

## Resultado verificado e retomada

- **Editado/salvo em disco:** subclasse do chefe, seleção por ID e introdução na Battle; duas extensões virtuais na base; stats apenas do boss; seleção da arte v2 via Resources; prefab de evento sem posição; fixture manual de validação e metas próprios. Não mudou código do gatilho, motor, dano do jogador, energia, recompensas, diálogos, flags ou save.
- **Compilação:** recarga solicitada por API; primeira chamada de fixture ocorreu antes de a classe carregar e foi recusada. Consulta identificou compilação pendente. Janela Unity estava minimizada; solicitação explícita de `RequestScriptReload` resolveu sem salvar a cena. A classe carregou e fixture executou. Não alterar configurações/autenticação do conector para resolver.
- **Comportamento por API:** [fixture de combate](../../Assets/Editor/ValidacaoBossVale.cs) passou para ciclo, carga/descanso sem dano, intenção no log, stun preservando intenção/carga, ação inválida, acerto/erro, defesa, resistência e vazamento que expira. Na mesma execução, prefab carregou com trigger/E/ID/destino/flags vazias e sprite Resources carregou. [Evidência](../../output/boss-vale/2026-10-29/validacao-combate.txt).
- **Dificuldade:**200 seeds fixos, sem itens: inicial0/200; preparado152/200 (76%). Preparado tem FOR20/VIT15/DEX20/AGI10 e técnica nível3, respeitando energia e usando básico quando necessário. Não é promessa de chance fixa para qualquer build ou prova de economia/progressão da demo.
- **Lua real:** `DroidScriptRunner`/`TerminalDroidApi` em Droid isolado montaram a mesma build por `aprenderTecnica`/`subirAtributo`, consumindo60 pontos sintéticos (equivalentes a12 lotes de5). Nenhum ponto foi concedido ao jogador real. Disponibilidade de XP/peças/recursos para essa preparação no percurso não foi validada. [Evidência](../../output/boss-vale/2026-10-29/validacao-lua.txt).
- **Integração isolada:** cena de prévia da Battle + estado sintético confirmaram subclasse `DroidDoIvo`, sprite v2 e fuga bloqueada. Singleton restaurado e preview fechado em `finally`. [Evidência](../../output/boss-vale/2026-10-29/validacao-integracao.txt).
- **Regressão dirigida:** `ValidacaoEncontrosAB.Atributos()` confirmou atributos de A/B/chefe chegando à Battle e bloqueio de fuga, sem saves. [Evidência](../../output/boss-vale/2026-10-29/regressao-ab.txt).
- **Revisão:** uma revisão independente somente leitura do recorte não encontrou achados relevantes; não substitui testes de input/visual. Sem cadeia de revisores ou alterações fora do escopo.
- **Preservação:** hashes de Battle.unity, Game.unity e prefab VisualInimigoBattle permaneceram idênticos ao baseline desta tarefa; BattleManager contém somente os dois hunks previstos além do trabalho já existente. Não houve Play, início real da luta, IO de saves ou captura visual nova.
- **Estado deixado no Unity:** projeto correto, Battle aberta, fora de Play, compilação concluída e dirty=True como no início. Alterações não salvas do usuário preservadas; nenhuma chamada de SaveScene. Não restaurar baseline sobre esse estado.
- **Backups/limpeza:** fontes anteriores e hashes em `output/boss-vale/2026-10-29/baseline/` e `baseline-hashes.json`; fontes da arte v2/v1 e C preservadas. Nenhum backup de save necessário; nenhuma limpeza pendente criada.
- **Próximo passo:** escolher a posição perto do rio e ligar/avaliar a interação E quando o usuário pedir montagem; definir trava narrativa depois. Jogar a luta para aprovar dificuldade, legibilidade da intenção e enquadramento do boss. Não declarar o evento encontrado no percurso ou aprovado visualmente por estes testes.

## Integração Git autorizada

Usuário solicitou commit/push, depois merge com `main`, e confirmou incluir todas as alterações do projeto excluindo backups e ferramentas temporárias. Branch de origem: `revisao/interiores-dialogos-colisoes-2026-10-29`; destino remoto `origin` e base `main`. Incluir Assets/docs e entregáveis/evidências de output; excluir backups, baseline, ferramentas, node_modules, arquivos de preparação temporária, snapshots de serialização e anexos locais. Preservar esses arquivos no disco. A publicação não aprova visualmente o jogo nem autoriza salvar a cena dirty do Editor.

**Autorização posterior de salvar tudo:** usuário pediu explicitamente não deixar nada não salvo. MCP confirmou somente Battle aberta, dirty=True, fora de Play no projeto correto. Salvar assets e essa cena passa a estar autorizado. Antes do SaveScene, preservar arquivo de disco em `output/backups/2026-10-29-publicacao/Battle-antes-salvar.unity`; não desfazer o estado de edição do usuário. Esse novo salvamento supera a restrição de não salvar da implementação anterior; nenhum posicionamento/evento novo será criado por essa ação. Na preparação Git foi encontrado repositório de download sem commit em `output/apresentacao-telas/download-mobile/`; excluir como ferramenta temporária, sem apagar.

**Salvamento confirmado:** `AssetDatabase.SaveAssets` e `SaveOpenScenes` concluíram; Battle passou a dirty=False, fora de Play. Diff do salvamento inclui ajustes de apresentação que já estavam abertos no Editor (escala/posição do inimigo, posição de painel e altura de viewport) e normalização de scrollbar/ordem de campos; preservados por autorização de salvar o contexto atual. Não são ajustes novos de balanceamento nem validação visual. Preparação Git contém359 arquivos do projeto/entregáveis, sem backups, node_modules ou ferramentas excluídas. Verificação de diff sem marcadores de conflito; espaços finais convencionais de YAML Unity preservados, sem reformatação geral.
