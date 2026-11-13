# Auditoria geral do projeto — 30/10/2026

## 1. Resumo executivo

1. Auditoria **estática e parcial**: nenhum teste, build, jogo, Unity ou MCP foi executado.
2. O inventário encontrou 104 Markdown em `docs/`, 80 scripts em `Assets/Scripts/` e 25 scripts próprios em `Assets/Editor/`.
3. A pasta de raiz `bugs de desenvolvimento/` não foi encontrada; há quatro registros em `docs/problemas/`.
4. A cobertura de leitura não atingiu todos os documentos nem todos os scripts; a matriz abaixo distingue leitura integral confirmada de cobertura não confirmada.
5. Achado funcional de maior confiança: objetivos de quest `Derrotar` existem, mas a vitória não registra progresso em `GerenciadorDeQuests`.
6. Outro provável defeito: venda usa `item.Preco` em vez de `PrecoDeVendaReal`, apesar do cálculo definido no catálogo.
7. Atualização de quests automáticas não foi encontrada ligada a toda mudança de flag/cena; a necessidade exata depende do contrato desejado.
8. Não foi demonstrada perda de save, quebra de fluxo do Ato 1, null reference ou race condition por leitura estática nesta cobertura.
9. Os relatos de tela cheia/Game Over, diálogo inesperado no Vale e sprite genérico permanecem sintomas sem causa reproduzida.
10. Este relatório não certifica comportamento em execução nem aprovação visual; recomenda uma rodada posterior de leitura/reprodução delimitada.

## 2. Inventário de documentos e sistemas

O inventário abaixo contém todos os 104 caminhos `.md` enumerados em `docs/`. “Leitura integral confirmada” significa que a leitura integral foi explicitamente registrada durante a auditoria; os demais itens foram enumerados, mas sua leitura integral não pôde ser confirmada. A breve descrição é baseada no título/caminho e, quando lido, no conteúdo.

### Documentos de contexto e arquitetura

| Arquivo | Assunto | Leitura |
|---|---|---|
| `docs/contexto-projeto.md` | Arquitetura e visão técnica geral | Integral |
| `docs/indice-trabalho.md` | Navegação por planos e contratos | Integral |
| `docs/plano-desenvolvimento.md` | Prioridades e backlog de desenvolvimento | Parcial |
| `docs/contextos/analises/2026-10-18-auditoria-city-casalip-ddol.md` | Auditoria histórica de City, CasaLip e DontDestroyOnLoad | Integral |
| `docs/contextos/cenas/_modelo-cena.md` | Modelo para contratos de cena | Integral |
| `docs/contextos/cenas/casalip.md` | Contrato e estados da CasaLip | Integral |
| `docs/contextos/cenas/city.md` | Contrato da City | Não confirmada |
| `docs/contextos/sistemas/combate.md` | Regras e contrato de combate | Integral |
| `docs/contextos/sistemas/estado-e-save.md` | Estado persistente, checkpoints e save | Integral |
| `docs/contextos/sistemas/interface.md` | Contrato da interface compartilhada | Parcial |

### Enredo

| Arquivo | Assunto | Leitura |
|---|---|---|
| `docs/enredo/TCC_-_ENREDO_v2.md` | Documento-base do enredo | Não confirmada |
| `docs/enredo/TCC_-_ENREDO_v2_ADENDO.md` | Adendo/cânone que prevalece sobre a versão-base | Não confirmada |

### Guias

| Arquivo | Assunto | Leitura |
|---|---|---|
| `docs/guias/criacao-sprites-personagens-lpc.md` | Criação e integração de sprites LPC | Integral |
| `docs/guias/direcao-arte-robos-e-inimigos.md` | Direção visual de robôs/inimigos | Não confirmada |
| `docs/guias/equipamentos-apollo.md` | Equipamentos, slots, comandos e validação | Integral |
| `docs/guias/icones-de-itens.md` | Ícones e cadastro visual de itens | Integral |
| `docs/guias/implementacao-city.md` | Montagem e rastreio de personagens/objetos da City | Não confirmada |
| `docs/guias/interfaces-sobre-imagens.md` | Método de composição de UI sobre arte | Não confirmada |
| `docs/guias/montagem-ato-1.md` | Procedimento de montagem/validação do Ato 1 | Não confirmada |
| `docs/guias/operacao-mcp.md` | Procedimento operacional do conector MCP | Integral |
| `docs/guias/validacao-jogo.md` | Casos e roteiro de validação do jogo | Não confirmada |

### Histórico

| Arquivo | Assunto | Leitura |
|---|---|---|
| `docs/historico/2026-09-14-bugs-casalip.md` | Bugs antigos da CasaLip | Não confirmada |
| `docs/historico/2026-09-15-entrega-ato-1.md` | Registro histórico de entrega do Ato 1 | Não confirmada |
| `docs/historico/2026-10-14-analise-projeto-unity.md` | Análise antiga do projeto Unity | Não confirmada |
| `docs/historico/2026-10-16-contexto-anterior/bug-battle-sprite-background.md` | Bug histórico do fundo de Battle | Não confirmada |
| `docs/historico/2026-10-16-contexto-anterior/checklist-desenvolvimento.md` | Checklist histórico de desenvolvimento | Não confirmada |
| `docs/historico/2026-10-16-contexto-anterior/documentacao-tecnica.md` | Documentação técnica anterior | Não confirmada |
| `docs/historico/2026-10-16-contexto-anterior/guia-montagem-ato-1.md` | Guia histórico de montagem do Ato 1 | Não confirmada |
| `docs/historico/2026-10-16-contexto-anterior/readme.md` | README histórico | Não confirmada |
| `docs/historico/2026-10-17-contexto-antes-hierarquia.md` | Estado histórico anterior à hierarquia de cenas | Não confirmada |
| `docs/historico/2026-10-22-cidade-hd.md` | Registro histórico de arte da cidade | Não confirmada |
| `docs/historico/2026-10-23-aproveitamento-artes-menus.md` | Reaproveitamento de artes de menus | Não confirmada |
| `docs/historico/2026-10-29-montagem-cidade-interiores-npcs.md` | Registro da montagem de City/interiores/NPCs | Não confirmada |
| `docs/historico/README.md` | Índice do histórico | Não confirmada |

### Planos de implementação e melhorias

| Arquivo | Assunto | Leitura |
|---|---|---|
| `docs/planos-implementacao/_modelo-tarefa.md` | Modelo de plano de tarefa | Integral |
| `docs/planos-implementacao/2026-10-16-correcao-casalip.md` | Correção e validação da CasaLip | Não confirmada |
| `docs/planos-implementacao/2026-10-17-auditoria-cenas.md` | Auditoria de cenas/prefabs | Não confirmada |
| `docs/planos-implementacao/2026-10-22-diagnostico-avisos-e-carregamento.md` | Avisos e carregamento | Não confirmada |
| `docs/planos-implementacao/2026-10-22-orientacao-npcs-city.md` | Orientação de NPCs na City | Não confirmada |
| `docs/planos-implementacao/2026-10-22-terminal-ilustrado.md` | Montagem e validação da UI do terminal | Parcial |
| `docs/planos-implementacao/2026-10-23-bag-ilustrada.md` | UI da Bag | Não confirmada |
| `docs/planos-implementacao/2026-10-23-droid-ilustrado.md` | UI/equipamentos do Droid | Não confirmada |
| `docs/planos-implementacao/2026-10-23-loja-ilustrada.md` | UI e comportamento da loja | Parcial |
| `docs/planos-implementacao/2026-10-23-menu-mundo-ilustrado.md` | UI do Menu do Mundo | Não confirmada |
| `docs/planos-implementacao/2026-10-23-sprites-unicas-itens.md` | Sprites únicas de itens | Não confirmada |
| `docs/planos-implementacao/2026-10-23-status-ilustrado.md` | UI de Status | Não confirmada |
| `docs/planos-implementacao/2026-10-24-apresentacao-telas.md` | Apresentação de telas e mapas | Não confirmada |
| `docs/planos-implementacao/2026-10-28-concepcao-interiores-ferrovale.md` | Concepção dos interiores | Não confirmada |
| `docs/planos-implementacao/2026-10-28-moradores-e-rotinas-ferrovale.md` | Moradores e rotinas | Não confirmada |
| `docs/planos-implementacao/2026-10-28-portas-interiores-e-mapas.md` | Portas, interiores e mapas | Não confirmada |
| `docs/planos-implementacao/2026-10-29-battle-artes-menus.md` | Arte e menus de Battle | Não confirmada |
| `docs/planos-implementacao/2026-10-29-direcao-visual-inimigos-ato1.md` | Direção visual dos inimigos do Ato 1 | Não confirmada |
| `docs/planos-implementacao/2026-10-29-evento-boss-vale.md` | Boss/evento do Vale | Não confirmada |
| `docs/planos-implementacao/2026-10-29-readme-e-wiki.md` | README e wiki | Não confirmada |
| `docs/planos-implementacao/2026-10-29-refatoracao-documentacao.md` | Refatoração/risco documental | Não confirmada |
| `docs/planos-implementacao/2026-10-29-refatoracao-nomenclatura-ferrovale.md` | Nomes, casas e identidade dos NPCs | Não confirmada |
| `docs/planos-implementacao/2026-10-29-revisao-interiores-dialogos-colisoes.md` | Diálogos, colisões e circulação | Não confirmada |
| `docs/planos-implementacao/2026-10-30-estabilidade-lua-loja-game-over.md` | Lua, loja e Game Over | Parcial |
| `docs/planos-implementacao/2026-10-30-revisao-geral-projeto-e-repositorio.md` | Revisão geral interna e do repositório | Parcial |
| `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md` | Melhorias de terminal/puzzles | Parcial |
| `docs/planos-implementacao/melhorias/02-estado-inventario-e-economia.md` | Estado, inventário e economia | Não confirmada |
| `docs/planos-implementacao/melhorias/03-combate-e-progressao.md` | Combate e progressão | Não confirmada |
| `docs/planos-implementacao/melhorias/04-missoes-mundo-e-narrativa.md` | Missões, mundo e narrativa | Não confirmada |
| `docs/planos-implementacao/melhorias/05-mecanicas-e-demo.md` | Mecânicas e demo | Não confirmada |
| `docs/planos-implementacao/melhorias/06-validacao-e-entrega.md` | Validação e entrega | Não confirmada |
| `docs/planos-implementacao/melhorias/README.md` | Índice de melhorias | Não confirmada |

### Problemas e revisões

| Arquivo | Assunto | Leitura |
|---|---|---|
| `docs/problemas/battle-fundo-nao-renderiza.md` | Registro do fundo de Battle | Integral |
| `docs/problemas/casalip-apresentacao-dialogo.md` | Apresentação de diálogo da CasaLip | Não confirmada |
| `docs/problemas/casalip-retorno-porta-trancada.md` | Retorno/porta trancada na CasaLip | Integral |
| `docs/problemas/terminal-log-painel-inativo.md` | Histórico do problema de log/painel do terminal | Integral |
| `docs/revisoes/auditoria-cenas-2026-10-17.md` | Revisão histórica das cenas | Não confirmada |
| `docs/revisoes/casalip-validacao-2026-10-17.md` | Validação histórica da CasaLip | Parcial |
| `docs/revisoes/casalip.md` | Revisão da CasaLip | Não confirmada |

### Wiki do projeto

Os 31 Markdown da wiki foram lidos integralmente, mas essa leitura não equivale a uma auditoria completa de todas as implementações descritas.

| Arquivo | Assunto |
|---|---|
| `docs/wiki/_Footer.md` | Rodapé da wiki |
| `docs/wiki/_Sidebar.md` | Navegação da wiki |
| `docs/wiki/Apollo.md` | Personagem Apollo |
| `docs/wiki/Artemis.md` | Personagem Artemis |
| `docs/wiki/Bestiario.md` | Inimigos/bestiário |
| `docs/wiki/Boss-Droid-do-Ivo.md` | Boss do Droid do Ivo |
| `docs/wiki/Campanha-e-finais.md` | Campanha e finais planejados |
| `docs/wiki/Celia.md` | Personagem Célia |
| `docs/wiki/Dara.md` | Personagem Dara |
| `docs/wiki/Desafios.md` | Desafios |
| `docs/wiki/Estado-do-projeto.md` | Estado publicado do projeto |
| `docs/wiki/Faccoes.md` | Facções |
| `docs/wiki/Fontes-e-criterios.md` | Fontes/critério editorial |
| `docs/wiki/Galeria.md` | Galeria |
| `docs/wiki/Guia-do-professor.md` | Guia do professor |
| `docs/wiki/Guilda-dos-Sucateiros.md` | Guilda |
| `docs/wiki/Historia.md` | História |
| `docs/wiki/Home.md` | Página inicial |
| `docs/wiki/Itens.md` | Itens |
| `docs/wiki/Ivo.md` | Personagem Ivo |
| `docs/wiki/Lip.md` | Personagem Lip |
| `docs/wiki/Lua.md` | Introdução a Lua |
| `docs/wiki/Missoes.md` | Missões |
| `docs/wiki/Moradores-de-Ferrovale.md` | Moradores de Ferrovale |
| `docs/wiki/Mundo-e-locais.md` | Mundo e locais |
| `docs/wiki/Nadir.md` | Personagem Nadir |
| `docs/wiki/Personagens.md` | Índice de personagens |
| `docs/wiki/Publicar-a-wiki.md` | Publicação da wiki |
| `docs/wiki/Sistemas-do-jogo.md` | Sistemas de jogo |
| `docs/wiki/Sobre-o-projeto.md` | Sobre o projeto |
| `docs/wiki/Visao-tecnica.md` | Visão técnica |

**Pasta de bugs:** não foi encontrada `C:\Users\silve\DroidsCode\bugs de desenvolvimento\`; a busca inicial encontrou `docs/problemas/` acima. Não se pode concluir que outra pasta com nome variante/interno inexista sem nova listagem dirigida.

**Arquivos de imagem em `docs/`:** foram enumeradas 10 imagens de evidência em `docs/revisoes/evidencias/` e 11 imagens em `docs/wiki/images/`; não foram inspecionadas visualmente nesta auditoria.

## 3. Estado por sistema

### Combate e progressão

- **Documentado:** combate por turnos, energia, técnicas e efeitos temporários estão especificados em `docs/contextos/sistemas/combate.md`; `docs/planos-implementacao/melhorias/03-combate-e-progressao.md` separa propostas de balanceamento de contratos implementados.
- **Código consultado parcialmente:** `Assets/Scripts/Combat/BattleManager.cs` encerra combate e conduz recompensas; `Assets/Scripts/Combat/CombatEngine.cs` aplica danos/efeitos. Não houve leitura integral de todos os scripts de combate.
- **Achado:** não encontrei chamada a `GerenciadorDeQuests.RegistrarProgresso` para derrota de inimigo no encerramento da vitória.
- **Efeito temporário:** limpeza após restaurar save aparece em `SalvamentoJson`, mas o encerramento após vitória/fuga não foi comprovado como limpando efeitos. A regra de design de persistência entre combates não está resolvida aqui.
- **Progressão:** o contrato declara pontos por nível e as técnicas gastam pontos; nenhum defeito de cálculo pôde ser confirmado com a cobertura parcial.

### Missões e história

- **Documentado:** quests cobrem conversa, coleta, puzzle, derrota e condições narrativas; os documentos avisam que entradas de catálogo não comprovam gatilhos montados.
- **Código consultado parcialmente:** `GerenciadorDeQuests.cs`, `CatalogoDeQuests.cs`, `QuestUIManager.cs`, `MoradorInterativo.cs`, `VerificadorDePuzzle.cs`.
- **Achados:** objetivos `Derrotar` constam no catálogo, mas não encontrei produtor de progresso na vitória. A atualização de quests automáticas está em eventos pontuais e não foi encontrada ligada a toda mudança de flag/cena.
- **Limite:** não percorri todas as cenas/prefabs nem confirmei quais quests estão acessíveis no build; não atribuo estado jogável às entradas de catálogo.

### Estado, save e Game Over

- **Documentado:** `docs/contextos/sistemas/estado-e-save.md` estabelece o estado persistente; os planos mais recentes registram save v7, checkpoint pré-batalha e Game Over.
- **Código consultado parcialmente:** `Assets/Scripts/Estado/SalvamentoJson.cs`, `Assets/Scripts/Estado/GerenciadorDeEstado.cs`, `Assets/Scripts/Menus/GameOverManager.cs`.
- **Evidência:** o caminho de carga limpa efeitos temporários antes de restaurar HP; `SalvamentoJson.cs:208–210` recusa salvar destinos Battle/GameOver, de modo que registrar quest enquanto ainda está nessas cenas exige consolidar progresso no retorno/checkpoint.
- **Limite:** não foram executados save/load, migração, recuperação pós-derrota nem cenários de arquivo inválido. Nenhuma perda de progresso foi demonstrada.

### Interface, terminal e loja

- **Documentado:** contratos da UI e planos específicos distinguem render/API de input real e aprovação visual.
- **Terminal:** `DroidScriptRunner` tem execução cooperativa com limite/retomada/cancelamento; o histórico `docs/problemas/terminal-log-painel-inativo.md` descreve uma ordem antiga que não condiz com a ativação do painel antes da mensagem inicial na implementação atual consultada. A correção visual/por input não foi revalidada.
- **Loja:** `LojaUIManager` consulta `PodeSerVendido` antes de consumir o item, conforme o trecho lido; a guarda de item protegido está implementada no código consultado. Contudo, a exibição/pagamento usa preço de compra em vez do preço de venda do catálogo.
- **Game Over/full screen:** os relatos de botões/tela fora da identidade visual esperada e área azul ao redimensionar estão registrados no plano de estabilidade. São relatos de usuário, sem reprodução nesta auditoria.

### Cenas, personagens e arte

- **Documentado:** contratos da City/CasaLip, revisão de interiores e matriz de identidade fornecem referências históricas/atuais diferentes; planos distinguem sprite de menu de sprite de mundo.
- **Relatos atuais:** diálogo indevido com Apollo no Vale e sprite genérico são sintomas informados pelo usuário; não foi possível localizar objeto/cena nem provar causa com o código parcialmente lido.
- **Limite:** não foram inspecionadas todas as cenas/prefabs/recursos serializados. A auditoria não determina se referências no Inspector estão quebradas.

## 4. Achados classificados

Classificação baseada apenas na evidência estática disponível. **Não foi observado achado 🔴 CRÍTICO confirmado**; isso é limite de cobertura, não garantia de ausência.

### 🟡 IMPORTANTE

| ID | Arquivo(s)/linhas | Evidência | Causa provável | Sugestão (não aplicada) |
|---|---|---|---|---|
| I-01 | `Assets/Scripts/Catalogos/CatalogoDeQuests.cs:103,261,419,443`; `Assets/Scripts/Combat/BattleManager.cs:724–741`; `Assets/Scripts/Historia/GerenciadorDeQuests.cs:16–21`; `docs/plano-desenvolvimento.md:48` | Existem objetivos `Derrotar`; o comentário do gerenciador atribui registro a BattleManager, mas o trecho de vitória não chama progresso. A busca de chamadas em `Assets/Scripts/` encontrou registros de puzzle/conversa, não de combate. O backlog mantém `QUEST-001` aberto. | Integração de progresso de quest por batalha não foi concluída ou foi removida sem atualizar comentário. | No fluxo aprovado de vitória, registrar derrota uma vez e persistir após retornar a uma cena salvável; evitar gravação na cena Battle/GameOver. Validar objetivo incompleto/concluído e recompensa sem duplicação. |
| I-02 | `Assets/Scripts/Catalogos/CatalogoDeItens.cs:77`; `Assets/Scripts/Menus/LojaUIManager.cs:196,235`; `Assets/Scripts/Menus/ApresentacaoLojaIlustrada.cs:62` | `PrecoDeVendaReal` calcula preço específico/metade do preço; a loja apresenta e paga `item.Preco`. | O fluxo de venda usa propriedade de compra e ignora o valor de venda contratado no catálogo. | Usar o preço de venda do catálogo tanto no rótulo quanto no crédito e validar zero/preço específico/estoque. |
| I-03 | `Assets/Scripts/Historia/GerenciadorDeQuests.cs:160–164,276`; `Assets/Scripts/Historia/QuestUIManager.cs:71`; `Assets/Scripts/Interiores/MoradorInterativo.cs:77`; `docs/planos-implementacao/melhorias/04-missoes-mundo-e-narrativa.md` (leitura integral pendente) | O comentário documenta atualização por troca de cena e mudança de flags; as chamadas localizadas estão no diário, conversa e entrega. A busca não encontrou `ControladorDeRoteiro`. | A integração de atualização global pode estar ausente ou delegada a um mecanismo não localizado. | Decidir o contrato de atualização e ligar a eventos reais de flag/cena, sem tornar abertura do diário requisito oculto. Reproduzir uma quest automática antes de classificar como bloqueio de percurso. |

### 🟢 BAIXA PRIORIDADE

| ID | Arquivo(s)/linhas | Evidência | Causa provável | Sugestão (não aplicada) |
|---|---|---|---|---|
| B-01 | `docs/contexto-projeto.md:12`; `docs/plano-desenvolvimento.md:37–41`; `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md:9,179`; `Assets/Scripts/Scripting/DroidScriptRunner.cs:35,61,69,80,88` | Documentos antigos ainda descrevem Lua sem limite/TER-001 pendente; runner implementa início, retomadas limitadas e cancelamento. O registro de TER-01 também mantém um próximo passo incompatível com a implementação posterior. | Consolidação documental incompleta após a mudança de código. | Atualizar somente após revisão integral dos planos/validações, preservando input físico e aceites visuais como pendentes quando não demonstrados. |
| B-02 | `docs/problemas/terminal-log-painel-inativo.md:4,23`; `Assets/Scripts/Menus/TerminalUIManager.cs:179,211,213,415` | O problema descreve ordem antiga de inicialização; no código consultado `Abrir()` ativa o painel antes de adicionar a primeira linha. | Registro histórico não marcado claramente como superado. | Acrescentar estado de “corrigido por código; comportamento/input não revalidado” em uma tarefa documental autorizada. |
| B-03 | `docs/planos-implementacao/2026-10-30-estabilidade-lua-loja-game-over.md:5,16,19,64,169` | O plano reúne status de momentos diferentes: resumo de pausa, implementação posterior, diagnósticos/RED antigos e checkpoints GREEN posteriores. | Cronologia de retomadas acumulada no mesmo arquivo sem um sumário final único. | Consolidar um status vigente datado e manter os registros históricos rotulados como superseded, sem apagar evidência. |
| B-04 | `docs/plano-desenvolvimento.md:5` (referência apontada pela auditoria) | A frente atual ainda aponta para 28/10 embora haja planos de 30/10. | Backlog/prioridade não foi atualizado. | Reordenar e apontar para plano vigente após decisão do usuário. |

### 🟢 BAIXA PRIORIDADE — hipóteses que precisam de reprodução

| ID | Arquivo(s)/linhas | Evidência/limite | Hipótese de causa | Próxima verificação |
|---|---|---|---|---|
| H-01 | `Assets/Scripts/Combat/BattleManager.cs:724–768`; `Assets/Scripts/Combat/CombatEngine.cs:18–56`; `Assets/Scripts/Estado/SalvamentoJson.cs:275–276` | A carga limpa efeitos; o trecho de encerramento de batalha não mostrou limpeza equivalente. Não está decidida a regra de duração entre combates. | Efeito temporário pode sobreviver a vitória/fuga no objeto Droid persistente. | Definir semântica e inspecionar estado antes/depois de combate em fixture isolada, sem save pessoal. |
| H-02 | `docs/planos-implementacao/2026-10-30-estabilidade-lua-loja-game-over.md:193,197–200` | Usuário relata botões sem identidade visual esperada e fundo azul em tela cheia; não houve captura/inspeção na condição relatada. | Pode envolver arte, fundo/câmera, cobertura de UI ou configuração por resolução; nada disso foi confirmado. | Reproduzir em resolução/modo registrados e inspecionar câmera, fundo, Canvas/Scaler e área exposta antes de qualquer ajuste. |
| H-03 | `docs/planos-implementacao/2026-10-30-revisao-geral-projeto-e-repositorio.md:58–62` | Usuário relata diálogo de Apollo sem interação e sprite genérico no Vale (“Vario” também foi transcrito); local/objeto/ator não identificados. | Trigger, referência de diálogo ou prefab/renderer inadequado; ainda são hipóteses. | Confirmar a cena/local exatos e reproduzir o objeto de interação; rastrear referência/ID sem substituir arte por tentativa. |
| H-04 | `Assets/Scripts/Combat/BattleManager.cs:724–756`; `Assets/Scripts/Estado/SalvamentoJson.cs:208–210` | Recompensas e salvamento no trecho de vitória foram consultados; não houve percurso nem teste de repetição. | Recompensa/registro pode duplicar se o fluxo de conclusão puder ser reentrado. | Analisar guardas de término e testar finitamente entrada/saída/retorno em fixture sem save do jogador. |

### ⚪ NÃO PRECISA CORRIGIR (com o escopo/evidência atuais)

| Caso | Justificativa |
|---|---|
| `docs/bugs de desenvolvimento/` não existe, mas há `docs/problemas/` | Não duplicar/mover registros só para satisfazer um nome de pasta; primeiro confirmar se o usuário se refere a outro diretório. A ausência da pasta raiz foi observada por uma consulta delimitada. |
| Compatibilidade `DroidScriptRunner.ExecutarCodigo()` permanece | O método é compatibilidade síncrona usada por fixtures/consumidores síncronos; não foi encontrada chamada de runtime na busca. Não remover sem localizar consumidores e contrato. |
| Alterações de atributo/técnica podem ser parciais em erro Lua | `DroidScriptRunner.cs:105–111` avisa explicitamente que nem toda mutação é transacional. O plano TER-02 trata transação integral como proposta separada; ausência de rollback total não é defeito comprovado até essa decisão ser aprovada. |
| NPC de ensino e “Livro de Códigos” não encontrados no código consultado | São funcionalidades pedidas como proposta recente, não parte da implementação declarada como pronta. Planejar como recurso novo e definir o modelo de persistência antes de classificá-lo como regressão. |
| Não há confirmação visual atual do Game Over/UI | “Arte no jogo” é requisito de aceitação visual, não se deduz da existência de uma sprite/cena nem se corrige aumentando escalas sem reprodução. |

## 5. Divergências entre documentação e código

1. **Quests de derrota:** `GerenciadorDeQuests.cs:16–21` comenta que BattleManager registra `Derrotar`, mas a vitória em `BattleManager.cs:724–741` não mostra a chamada; `QUEST-001` segue pendente em `docs/plano-desenvolvimento.md:48`. A divergência favorece a leitura de que a integração falta, embora a cobertura incompleta impeça afirmar que não exista outro produtor indireto.
2. **Preço de venda:** `CatalogoDeItens.cs:77` define `PrecoDeVendaReal`; `LojaUIManager.cs:196,235` exibe/paga `Preco`. O código da loja está divergente do contrato do catálogo.
3. **TER-001:** documentos ainda indicam execução Lua síncrona/sem limite, mas `DroidScriptRunner.cs` implementa coroutine com limites configurados. Os registros mais recentes do plano de estabilidade prevalecem sobre estados antigos, sem equivaler a teste real.
4. **TER-002:** o problema descreve painel/log inativo; a ordem atual observada ativa o painel antes de inserir texto. A correção aparece como implementada em outras partes do histórico, mas o documento de problema não foi encerrado.
5. **Status do plano de estabilidade:** o mesmo arquivo mantém resumo inicial “pausado/implementação salva”, pendências e RED antigos e checkpoints posteriores com testes/montagem executados. Usar a cronologia mais recente; não repetir helper de montagem nem assumir aprovação visual.
6. **Contratos de efeitos temporários:** combate descreve duração por turno; save limpa efeitos em carga. Não foi encontrado contrato explícito suficiente sobre limpeza após vitória/fuga, portanto a diferença não pode ser declarada incorreta até decisão de design.
7. **Histórico vs estado atual:** análise de 18/10, revisão de 17/10 e documentação anterior à hierarquia descrevem configurações/contagens antigas. São proveniência, não prova do estado serializado atual.
8. **Wiki vs código:** a leitura integral da wiki foi feita, mas os scripts não foram integralmente lidos; não é possível determinar sistematicamente todas as divergências da wiki nesta auditoria parcial.

## 6. Documentos desatualizados ou contraditórios

- `docs/contexto-projeto.md` e `docs/plano-desenvolvimento.md`: referências a TER-001 não refletem o runner limitado mais recente. Atualizar status sem apagar pendências de input real.
- `docs/problemas/terminal-log-painel-inativo.md`: descrição antiga permanece sem um fechamento que distinga implementação da confirmação em jogo.
- `docs/planos-implementacao/2026-10-30-estabilidade-lua-loja-game-over.md`: cronologia de estados precisa de um resumo vigente único; ver B-03.
- `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md`: implementação e próximos passos parecem coexistir sem status resolvido (B-01).
- `docs/planos-implementacao/2026-10-30-revisao-geral-projeto-e-repositorio.md`: contém achados/relatos posteriores que são sintomas e encaminhamentos; não são diagnósticos de causa.
- **Limite:** não fiz leitura integral dos 104 documentos. A lista acima é o conjunto de contradições comprovadas pelas referências lidas, não uma afirmação de que os demais documentos estejam atualizados.

## 7. Bugs prováveis ordenados por prioridade

| Ordem | Problema/hipótese | Evidência | Confiança e próximo passo |
|---|---|---|---|
| 1 | Objetivo de quest por derrota pode nunca progredir | Objetivos no catálogo e ausência de registro no trecho de vitória; comentário e plano contradizem o código | Alta confiança na ausência da chamada direta no método inspecionado; confirmar outros produtores/rota e testar idempotência antes de corrigir |
| 2 | Venda paga valor de compra, não valor de venda | Propriedade específica do catálogo vs `item.Preco` na apresentação/pagamento | Alta confiança estática; validar itens com preço especial/zero e fluxo de confirmação |
| 3 | Quest automática pode atualizar só em interação/UI e não ao mudar flag/cena | Chamadas localizadas e documentação do gerenciador; controlador mencionado não localizado | Média; confirmar intenção e dependências no percurso |
| 4 | Efeito temporário pode sobreviver entre batalhas | Não vista limpeza no encerramento, embora haja limpeza no load | Baixa/média; depende da regra de design e do estado mantido entre cenas |
| 5 | Tela cheia expõe fundo azul ou Game Over destoa da arte | Relato do usuário sem condição de render capturada | Não classificável como causa; reproduzir antes de mexer |
| 6 | NPC/trigger inicia fala errada e sprite genérico no Vale | Relato do usuário, sem identificação da cena/objeto | Hipótese; localizar e inspecionar referências no ativo real |
| 7 | Progresso de quest/recompensas poderia duplicar em reentrada | Não foi possível confirmar todas as guardas/eventos | Hipótese de baixa confiança; testar somente após análise de fluxo |

## 8. Plano de ação sugerido

1. **Completar esta auditoria de leitura:** ler integralmente os documentos ainda não confirmados e os scripts não lidos; criar matriz path→estado de leitura e reconciliar os achados antes de chamar este documento de completo.
2. **Confirmar requisitos de quest:** decidir produtor/ordem de salvamento para objetivos `Derrotar` e atualização de condições automáticas; então adicionar testes finitos, sem gravar save pessoal.
3. **Corrigir contrato de venda, se confirmado:** exibir e pagar `PrecoDeVendaReal`; testar preço padrão, exceção, estoque e item protegido em fixture.
4. **Reconciliar documentação de Lua:** separar implementação da validação por input real, fechar TER-001/TER-002 apenas com evidência correspondente.
5. **Reproduzir os relatos recentes:** registrar cena, resolução, modo de tela, Game View/zoom e objeto/ator exatos para os problemas de Game Over, fundo azul, diálogo e sprite do Vale.
6. **Decidir persistência do Livro de Códigos:** conteúdo a ensinar, quem ensina, slots/edição, escopo do save e segurança contra execução automática; isso é feature nova, não correção presumida.
7. **Inspecionar os efeitos entre combates** somente depois de estabelecer a regra esperada para vitória, fuga e Game Over.
8. Não alterar Canvas/Scaler compartilhado, GUIDs/cenas ou estado de save com base apenas em hipóteses desta auditoria.

## 9. Perguntas em aberto para o usuário

1. “Vario” refere-se ao Vale? Qual cena exata, ponto de entrada e objeto inicia Apollo?
2. Qual imagem/tela/botões devem servir como referência visual aprovada para Game Over? Em que resolução e modo de tela aparece o azul?
3. Objetivos de quest `Derrotar` devem contar qualquer inimigo, um ID específico ou somente o boss? A recompensa deve persistir imediatamente após a batalha?
4. Quests automáticas devem atualizar ao alterar qualquer flag, na troca de cena, ao abrir o diário, ou em uma combinação definida?
5. Efeitos temporários devem terminar ao sair de uma batalha, persistir por uma contagem global de turnos, ou ser limpos apenas em descanso/carga?
6. O Livro de Códigos é um conjunto de trechos locais ao jogador, um item/inventário diegético ou dados no save da campanha? Códigos armazenados devem ser apenas texto até o usuário apertar Executar?
7. “bugs de desenvolvimento” é uma pasta esperada em outro caminho/nome? A consulta delimitada só encontrou `docs/problemas/`.

## 10. Cobertura de leitura e arquivos não confirmados

### Documentos

- **Integral confirmada (46):** os 31 arquivos de `docs/wiki/`; `docs/contexto-projeto.md`; `docs/indice-trabalho.md`; `docs/contextos/analises/2026-10-18-auditoria-city-casalip-ddol.md`; `docs/contextos/cenas/_modelo-cena.md`; `docs/contextos/cenas/casalip.md`; `docs/contextos/sistemas/combate.md`; `docs/contextos/sistemas/estado-e-save.md`; `docs/planos-implementacao/_modelo-tarefa.md`; `docs/guias/criacao-sprites-personagens-lpc.md`; `docs/guias/equipamentos-apollo.md`; `docs/guias/icones-de-itens.md`; `docs/guias/operacao-mcp.md`; `docs/problemas/battle-fundo-nao-renderiza.md`; `docs/problemas/casalip-retorno-porta-trancada.md`; `docs/problemas/terminal-log-painel-inativo.md`.
- **Parcial confirmada (8):** `docs/plano-desenvolvimento.md`; `docs/contextos/sistemas/interface.md`; `docs/planos-implementacao/2026-10-22-terminal-ilustrado.md`; `docs/planos-implementacao/2026-10-23-loja-ilustrada.md`; `docs/planos-implementacao/2026-10-30-estabilidade-lua-loja-game-over.md`; `docs/planos-implementacao/2026-10-30-revisao-geral-projeto-e-repositorio.md`; `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md`; `docs/revisoes/casalip-validacao-2026-10-17.md`.
- **Leitura integral não confirmada:** os outros 50 Markdown da lista de inventário. A auditoria não conseguiu separar todos entre “nenhuma leitura” e “consulta parcial”; não os considero lidos.

### Código C# próprio relevante

Foram enumerados 80 arquivos em `Assets/Scripts/` e 25 em `Assets/Editor/` (helpers/fixtures próprios). “Integral confirmada” = arquivo explicitamente reportado como integral pelo processo de auditoria; “parcial” = trechos lidos; todos os demais têm leitura integral não confirmada.

**Integral confirmada — `Assets/Scripts/` (10):**

- `Assets/Scripts/Scripting/DroidScriptRunner.cs`
- `Assets/Scripts/Scripting/LimitesDeExecucao.cs`
- `Assets/Scripts/Menus/GameOverManager.cs`
- `Assets/Scripts/Menus/BalaoDaLoja.cs`
- `Assets/Scripts/Menus/BagUIManager.cs`
- `Assets/Scripts/Menus/MenuMundoManager.cs`
- `Assets/Scripts/Menus/MainMenuManager.cs`
- `Assets/Scripts/Menus/TelaDeDroidManager.cs`
- `Assets/Scripts/Menus/TelaDeStatusManager.cs`
- `Assets/Scripts/Historia/GerenciadorDeQuests.cs`

**Parcial confirmada — `Assets/Scripts/` (11):**

- `Assets/Scripts/Combat/BattleManager.cs`
- `Assets/Scripts/Estado/SalvamentoJson.cs`
- `Assets/Scripts/Menus/LojaUIManager.cs`
- `Assets/Scripts/Menus/TerminalUIManager.cs`
- `Assets/Scripts/Scripting/TerminalDroidApi.cs`
- `Assets/Scripts/Catalogos/CatalogoDeItens.cs`
- `Assets/Scripts/Catalogos/CatalogoDeQuests.cs`
- `Assets/Scripts/Historia/QuestUIManager.cs`
- `Assets/Scripts/Interiores/MoradorInterativo.cs`
- `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs`
- `Assets/Scripts/Menus/ApresentacaoLojaIlustrada.cs`

**Integral confirmada — `Assets/Editor/` (7):**

- `Assets/Editor/ValidacaoCombate.cs`
- `Assets/Editor/ValidacaoEncontrosAB.cs`
- `Assets/Editor/ValidacaoSalvarCarregar.cs`
- `Assets/Editor/ValidacaoBagBatalha.cs`
- `Assets/Editor/ValidacaoBagIlustrada.cs`
- `Assets/Editor/ValidacaoBossVale.cs`
- `Assets/Editor/ValidacaoBattleIlustrada.cs`

**Parcial confirmada — `Assets/Editor/` (2):**

- `Assets/Editor/ValidacaoLimiteLua.cs`
- `Assets/Editor/ValidacaoEquipamentosApollo.cs`

**Leitura integral não confirmada — 59 scripts em `Assets/Scripts/`:**

- `Assets/Scripts/Camera/CameraFollow.cs`
- `Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs`, `Assets/Scripts/Casa/InteragirComTerminalCasa.cs`, `Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs`
- `Assets/Scripts/Catalogos/CatalogoDeDialogos.cs`, `Assets/Scripts/Catalogos/CatalogoDeInimigos.cs`, `Assets/Scripts/Catalogos/CatalogoDeLicoes.cs`, `Assets/Scripts/Catalogos/CatalogoDeLocais.cs`, `Assets/Scripts/Catalogos/CatalogoDeNpcs.cs`, `Assets/Scripts/Catalogos/CatalogoDePuzzles.cs`, `Assets/Scripts/Catalogos/CatalogoVisualDeItens.cs`
- `Assets/Scripts/Combat/ApresentacaoBattleIlustrada.cs`, `Assets/Scripts/Combat/ApresentacaoInimigoBattle.cs`, `Assets/Scripts/Combat/CombatEngine.cs`, `Assets/Scripts/Combat/DroidDoIvo.cs`, `Assets/Scripts/Combat/EncounterZone.cs`, `Assets/Scripts/Combat/IParticipanteDeCombate.cs`, `Assets/Scripts/Combat/InimigoFixo.cs`, `Assets/Scripts/Combat/ResultadoAcao.cs`
- `Assets/Scripts/Droid/Braco.cs`, `Assets/Scripts/Droid/Cabeca.cs`, `Assets/Scripts/Droid/Droid.cs`, `Assets/Scripts/Droid/DroidPart.cs`, `Assets/Scripts/Droid/DroidStats.cs`, `Assets/Scripts/Droid/Perna.cs`, `Assets/Scripts/Droid/ServicoDeEquipamento.cs`, `Assets/Scripts/Droid/Tronco.cs`
- `Assets/Scripts/Estado/DadosDoJogo.cs`, `Assets/Scripts/Estado/GerenciadorDeEstado.cs`, `Assets/Scripts/Estado/ISistemaDeSalvamento.cs`
- `Assets/Scripts/Historia/ApolloDica.cs`, `Assets/Scripts/Historia/CondicaoDeHistoria.cs`, `Assets/Scripts/Historia/DialogoUIManager.cs`, `Assets/Scripts/Historia/EscalaLocalDeDialogo.cs`, `Assets/Scripts/Historia/GatilhoDeBatalha.cs`, `Assets/Scripts/Historia/GatilhoDeDialogo.cs`, `Assets/Scripts/Historia/NpcInterativo.cs`, `Assets/Scripts/Historia/OrientacaoNpc.cs`, `Assets/Scripts/Historia/TrancaPorHistoria.cs`
- `Assets/Scripts/Interiores/TerminalDoInterior.cs`
- `Assets/Scripts/Menus/ApresentacaoBagIlustrada.cs`, `Assets/Scripts/Menus/ApresentacaoDroidIlustrado.cs`, `Assets/Scripts/Menus/ApresentacaoMenuMundoIlustrado.cs`, `Assets/Scripts/Menus/ApresentacaoStatusIlustrado.cs`, `Assets/Scripts/Menus/ApresentacaoTerminalIlustrado.cs`
- `Assets/Scripts/Scripting/EfeitosTemporariosUtil.cs`, `Assets/Scripts/Scripting/ItemConsumivel.cs`, `Assets/Scripts/Scripting/LocalizadorDeTerminal.cs`, `Assets/Scripts/Scripting/PlayerMovement.cs`, `Assets/Scripts/Scripting/PontosDeProgressao.cs`, `Assets/Scripts/Scripting/ProgressaoDeNivel.cs`, `Assets/Scripts/Scripting/RestaurarPosicao.cs`, `Assets/Scripts/Scripting/SistemaDeProgressao.cs`, `Assets/Scripts/Scripting/TabelaDeCombate.cs`, `Assets/Scripts/Scripting/TabelaDeCustos.cs`, `Assets/Scripts/Scripting/TabelaDeEnergia.cs`, `Assets/Scripts/Scripting/TecnicaComposta.cs`, `Assets/Scripts/Scripting/TipoAtributo.cs`, `Assets/Scripts/Scripting/TransicaoDeCena.cs`

**Leitura integral não confirmada — 16 scripts em `Assets/Editor/`:**

- `Assets/Editor/ArteInimigosBattle.cs`, `Assets/Editor/MontagemBattleIlustrada.cs`, `Assets/Editor/MontagemGameOver.cs`, `Assets/Editor/MontagemLojaIlustrada.cs`, `Assets/Editor/MontagemMenuMundoIlustrado.cs`, `Assets/Editor/MontagemTerminalIlustrado.cs`, `Assets/Editor/OrganizacaoVisualDeItens.cs`, `Assets/Editor/RefocoTerminalIlustrado.cs`, `Assets/Editor/ValidacaoCasaLip.cs`, `Assets/Editor/ValidacaoDialogoGenerico.cs`, `Assets/Editor/ValidacaoEnergiaApollo.cs`, `Assets/Editor/ValidacaoGameOver.cs`, `Assets/Editor/ValidacaoLojaIlustrada.cs`, `Assets/Editor/ValidacaoMenuMundoIlustrado.cs`, `Assets/Editor/ValidacaoStatusIlustrado.cs`, `Assets/Editor/ValidacaoTerminalIlustrado.cs`.

**Contagem reconciliada:** 17 integral confirmadas, 13 parciais e 75 sem leitura integral confirmada. `ApresentacaoLojaIlustrada.cs` teve status divergente entre os registros da auditoria; por conservadorismo, consta como parcial. A contagem não equivale a uma cobertura completa dos 105 scripts.

### C# de terceiros não tratado como código do jogo

`Assets/TextMesh Pro/Examples & Extras/Scripts/` contém 34 scripts de exemplos do pacote TextMesh Pro. Foram enumerados, mas não lidos; não os considero relevantes ao comportamento próprio do jogo. Nenhuma inspeção de consumo/referências foi feita nesta auditoria parcial.

## Validação desta auditoria

- Não executei comandos de teste/build, não iniciei o jogo nem Unity, e não usei MCP.
- Não alterei arquivos existentes, código, cenas, prefabs, saves ou configurações. O único arquivo que esta tarefa pretende criar é este relatório.
- O estado do workspace já continha alterações não relacionadas antes da elaboração; elas foram preservadas.
- Não foi feita validação funcional; não há teste de comportamento nesta tarefa.
- A conclusão é **relatório parcial, não auditoria completa**. O passo 1 do plano sugerido é resolver a cobertura documental/código antes de transformar hipóteses em defeitos confirmados.
