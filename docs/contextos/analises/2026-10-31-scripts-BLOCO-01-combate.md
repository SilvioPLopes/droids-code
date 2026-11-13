# Auditoria de scripts C# — BLOCO 01: Combate

**Escopo:** leitura estática dos nove arquivos `Assets/Scripts/Combat/*.cs`, mais confronto dirigido com o contrato de combate e os registros relativos a encontros e ao boss. Não executei testes, compilei, abri Unity, rodei o jogo ou usei MCP. Nenhum arquivo existente foi alterado.

**Baseline:** li primeiro o relatório [Auditoria geral do projeto — 30/10](2026-10-30-auditoria-geral-projeto.md). O achado já registrado sobre progresso de quest por derrota não é repetido como um novo item abaixo; a leitura integral do fluxo `BattleManager.FinalizarBatalha` confirma que ele não chama `RegistrarProgresso` diretamente. Ver achado I-01 no relatório anterior.

## 1. Resumo por arquivo

| Arquivo | Leitura integral | O que faz |
|---|---:|---|
| `Assets/Scripts/Combat/ApresentacaoBattleIlustrada.cs` | Sim | Aplica escala local à apresentação, sincroniza HUD/listas/log e adapta botões dinâmicos ao quadro ilustrado. |
| `Assets/Scripts/Combat/ApresentacaoInimigoBattle.cs` | Sim | Resolve o ID do inimigo e seleciona sprite/quadro visual, com fallback para A/B/legado. |
| `Assets/Scripts/Combat/BattleManager.cs` | Sim | Inicializa combate, ações, itens, turnos, recompensas, checkpoint, fuga e Game Over. |
| `Assets/Scripts/Combat/CombatEngine.cs` | Sim | Resolve dano por turno, stun, ação, dano direto e decremento de efeitos. |
| `Assets/Scripts/Combat/DroidDoIvo.cs` | Sim | Implementa a máquina de estados/ciclo de ações do boss e efeitos do golpe de contenção. |
| `Assets/Scripts/Combat/EncounterZone.cs` | Sim | Mede deslocamento em trigger, sorteia encontros e prepara checkpoint/transição para Battle. |
| `Assets/Scripts/Combat/InimigoFixo.cs` | Sim | Implementa o participante inimigo padrão, ação única, cálculo de dano e efeitos. |
| `Assets/Scripts/Combat/IParticipanteDeCombate.cs` | Sim | Contrato comum para Droid e inimigo no motor de combate. |
| `Assets/Scripts/Combat/ResultadoAcao.cs` | Sim | Estrutura mínima de resultado de uma ação. |

## 2. Estado do sistema e relação com a documentação

### Fluxo de batalha

`BattleManager.Start()` obtém o Droid persistente, captura o retorno, resolve o inimigo pelo ID/contexto, constrói o participante e liga os botões ([BattleManager.cs:154–205](../../Assets/Scripts/Combat/BattleManager.cs)). O contexto prioriza `ProximoInimigoId`, depois `idDoInimigoPadrao`, e finalmente os valores do Inspector ([BattleManager.cs:609–652](../../Assets/Scripts/Combat/BattleManager.cs)).

O motor aplica dano por turno no início da vez de quem está afetado, impede a ação se HP chegou a zero ou houver Stun, executa a ação e desconta efeitos no final ([CombatEngine.cs:8–61](../../Assets/Scripts/Combat/CombatEngine.cs)). Vitória/fuga fecham listas, consolidam o checkpoint para a origem quando existe contexto e retornam ao mundo; derrota não grava o Droid morto e carrega `GameOver` ([BattleManager.cs:724–777](../../Assets/Scripts/Combat/BattleManager.cs)).

O fluxo corresponde ao contrato atual de combate em [combate.md:24–32](../sistemas/combate.md), inclusive: consumo e HP em memória durante a luta; checkpoint antes/depois; fuga sem prêmio; derrota em Game Over; e ausência de retomada do turno de combate. A fixture ou aprovação visual descrita em documentos não foi repetida nem validada nesta tarefa.

### Ações, efeitos e inimigos

- `InimigoFixo` oferece uma ação (`Atacar`) e usa os cálculos de acerto/crítico compartilhados ([InimigoFixo.cs:15–27, 51–92](../../Assets/Scripts/Combat/InimigoFixo.cs)). O plano de Battle declara explicitamente que novas rotinas de inimigo são trabalho separado ([combate.md:57](../sistemas/combate.md)); ação única não é defeito por si só.
- `DroidDoIvo` anuncia uma ação por vez, tem quatro etapas e só avança o ciclo após uma ação válida ([DroidDoIvo.cs:9–57](../../Assets/Scripts/Combat/DroidDoIvo.cs)). O motor não chama a ação de participante atordoado/morto, portanto o ciclo não progride nesses casos. Isso corresponde ao contrato ([combate.md:7](../sistemas/combate.md)).
- `EncounterZone` usa uma tabela de probabilidades absolutas se `encontros` estiver preenchido; rejeita tabela malformada e impede entradas desconhecidas, chefes e únicos nessa modalidade ([EncounterZone.cs:135–153](../../Assets/Scripts/Combat/EncounterZone.cs)). O contrato documenta o sorteio A/B e a distinção entre tabela e campo único ([combate.md:38–41](../sistemas/combate.md)).
- `IParticipanteDeCombate` expõe as listas mutáveis de efeitos e os métodos de ação/decremento ([IParticipanteDeCombate.cs:6–38](../../Assets/Scripts/Combat/IParticipanteDeCombate.cs)); o contrato depende de implementadores manterem invariantes compatíveis.

### Apresentação de Battle

`ApresentacaoBattleIlustrada` contém escala local por `min(largura/referência, altura/referência)` com ramo centralizado ([ApresentacaoBattleIlustrada.cs:33–50](../../Assets/Scripts/Combat/ApresentacaoBattleIlustrada.cs)). É uma adaptação de escala do ramo da apresentação, não prova de responsividade integral da tela: os objetos internos são ajustados parcialmente por código e o relatório de responsividade de todas as telas pertence ao BLOCO 06. O plano documenta o ramo local de referência 1672×941 e preservação do Canvas/Scaler ([combate.md:49–53](../sistemas/combate.md)).

`ApresentacaoInimigoBattle` busca um `BattleManager` na própria cena se não foi serializado, usa `ProximoInimigoId` antes do ID padrão e prioriza sprite explicitamente cadastrada sobre o recurso do boss ([ApresentacaoInimigoBattle.cs:21–68](../../Assets/Scripts/Combat/ApresentacaoInimigoBattle.cs)). O plano de boss confirma que a arte do boss é recurso e que a aprovação visual/enquadramento continuam pendentes ([2026-10-29-evento-boss-vale.md](../../planos-implementacao/2026-10-29-evento-boss-vale.md)).

## 3. Achados deste bloco

Não reclassifico como novos achados o problema de progresso de quest por derrota, a incerteza de efeitos entre batalhas ou os relatos visuais já listados na auditoria anterior.

### 🟡 IMPORTANTE — validação de chefe/único inconsistente entre os dois modos de zona

- **Arquivos/linhas:** [EncounterZone.cs:124–128, 135–149](../../Assets/Scripts/Combat/EncounterZone.cs); [CatalogoDeInimigos.cs:201–202](../../Assets/Scripts/Catalogos/CatalogoDeInimigos.cs); [2026-10-29-evento-boss-vale.md:13](../../planos-implementacao/2026-10-29-evento-boss-vale.md); [combate.md:39–40](../sistemas/combate.md).
- **Evidência:** no modo de tabela, `TentarSelecionarInimigo` rejeita `EhChefe` e `EhUnico` (linhas 146–148). Sem tabela, retorna `idDoInimigo` diretamente (linhas 124–128), sem consultar essas propriedades. O Droid do Ivo está marcado simultaneamente `EhChefe=true` e `EhUnico=true`. O plano orienta manter o boss fora da EncounterZone e usar o gatilho roteirizado.
- **Causa provável:** a regra de exclusão foi implementada somente no caminho novo de tabela; o fallback legado aceita qualquer ID cadastrado.
- **Impacto/certeza:** configuração-dependente. Uma zona sem tabela, com chance positiva e `idDoInimigo="droid_do_ivo"` (ou outro único/chefe) pode selecionar o encontro; o código então prepara Battle. Não inspecionei os valores serializados das zonas nesta etapa. A tabela A/B documentada para `Game/ZonaBatalha` rejeita chefes/únicos e não é evidência de que uma zona atual esteja configurada incorretamente.
- **Sugestão (não aplicada):** definir explicitamente se o modo de ID único representa um encontro roteirizado ou aleatório. Se for aleatório, aplicar a mesma validação de elegibilidade do modo de tabela; se deve admitir encontro fixo único, renomear/documentar o contrato para não confundir esse modo com spawn aleatório.

### 🟢 BAIXA PRIORIDADE — comentário de recompensa afirma salvamento imediato, enquanto a persistência é consolidada depois

- **Arquivo/linhas:** [BattleManager.cs:565–570, 571–600, 734–745](../../Assets/Scripts/Combat/BattleManager.cs); [combate.md:27](../sistemas/combate.md).
- **Evidência:** o comentário antes de `AplicarRecompensas()` diz que Gold/itens são creditados e “salva na hora”; o método só chama `AdicionarGold`/`AdicionarItem`. O `SalvarCheckpoint` ocorre depois, em `FinalizarBatalha`, agregando XP, flag e recompensas no estado final.
- **Causa provável:** comentário herdado do desenho anterior de persistência imediata, não atualizado após consolidar o checkpoint de batalha.
- **Impacto:** não é evidência de falha no fluxo normal com retorno válido: o contrato atual especifica persistência final única e o código tenta salvar depois de aplicar tudo. Sem checkpoint/contexto válido, não há essa gravação automática; o contrato descreve Battle direta como fallback sem checkpoint de encerramento ([combate.md:29](../sistemas/combate.md)).
- **Sugestão (não aplicada):** corrigir o comentário quando houver autorização para editar código/documentação, mantendo explícito que `AplicarRecompensas` apenas modifica estado em memória e que `FinalizarBatalha` consolida.

### 🟢 BAIXA PRIORIDADE — probabilidade zero depende de valor aleatório exatamente igual a zero

- **Arquivo/linha:** [BattleManager.cs:585–589](../../Assets/Scripts/Combat/BattleManager.cs).
- **Evidência:** o filtro é `if (Random.value > drop.chance) continue;`. Para `chance == 0`, o drop continua se `Random.value == 0`, pois a comparação é falsa.
- **Causa provável:** comparação estrita em vez de tratar o limite inferior como chance zero.
- **Impacto/certeza:** efeito raro e dependente do contrato inclusivo de `UnityEngine.Random.value`; não foi executado nem reproduzido. O atributo `[Range]` limita a edição normal no Inspector, mas não garante a condição de fronteira do sorteio.
- **Sugestão (não aplicada):** fazer o caminho de chance zero recusar explicitamente antes do sorteio (e validar faixas serializadas); cobrir os extremos 0 e 1 em uma fixture determinística.

### ⚪ NÃO PRECISA CORRIGIR — dependências explícitas do Inspector e ações únicas

- `BattleManager.Start()` exige `botaoAtacar`, `botaoItem`, `botaoFugir`, sliders e referências de texto configurados ([BattleManager.cs:60–82, 177–204](../../Assets/Scripts/Combat/BattleManager.cs)); `AtualizarBarras()` e `MostrarMensagem()` usam sliders/texto sem null-check ([BattleManager.cs:783–814](../../Assets/Scripts/Combat/BattleManager.cs)). A própria classe descreve a montagem manual e o arraste pelo Inspector ([BattleManager.cs:10–13](../../Assets/Scripts/Combat/BattleManager.cs)). Isso é uma dependência de montagem conhecida, não evidência de referência quebrada. A cena/prefab não foi inspecionada neste bloco.
- `InimigoFixo` expõe somente `Atacar` e `CombatEngine.VerificarDerrota()` mantém TODO sobre derrota por ausência de método essencial ([CombatEngine.cs:64–73](../../Assets/Scripts/Combat/CombatEngine.cs)). A documentação diz que padrões de inimigos são entrega futura ([combate.md:57](../sistemas/combate.md)); não ampliar sem decisão de design.

## 4. Fragilidade e acoplamento

1. **Ordem de inicialização e singleton:** `BattleManager.Start()` pressupõe que `GerenciadorDeEstado.Instancia` e seu Droid estejam prontos ([BattleManager.cs:154–165](../../Assets/Scripts/Combat/BattleManager.cs)). As apresentações também consultam o singleton em `LateUpdate`/`Start` ([ApresentacaoBattleIlustrada.cs:21–29](../../Assets/Scripts/Combat/ApresentacaoBattleIlustrada.cs), [ApresentacaoInimigoBattle.cs:21–37](../../Assets/Scripts/Combat/ApresentacaoInimigoBattle.cs)). O código assume que o gerenciador persiste e inicializa antes; não há prova de falha de ordem aqui, pois não li as cenas nem o script de estado integralmente neste bloco.
2. **Contrato das listas de ação:** `TurnoDoInimigo()` indexa a primeira ação retornada ([BattleManager.cs:698–700](../../Assets/Scripts/Combat/BattleManager.cs)). Todos os participantes lidos retornam uma ação, incluindo o boss; se uma implementação futura retornar lista vazia, ocorre `ArgumentOutOfRangeException`. O contrato de interface aceita `IEnumerable<string>` sem declarar cardinalidade ([IParticipanteDeCombate.cs:30–38](../../Assets/Scripts/Combat/IParticipanteDeCombate.cs)). Fragilidade para implementadores futuros, não bug atual demonstrado.
3. **Referências de UI:** vários campos de Inspector são obrigatórios por uso incondicional; se a montagem serializada divergir, o erro aparece no primeiro `Start`, atualização de HUD ou log. A validação deve examinar a cena no bloco autorizado, não inferir referências quebradas pelo script.
4. **Atualização contínua da apresentação:** `LateUpdate()` chama layout e sincronização de conteúdo a cada frame; quando listas estão abertas, `AtualizarLista()` reescreve estilo e força reconstrução do layout ([ApresentacaoBattleIlustrada.cs:21–30, 63–108](../../Assets/Scripts/Combat/ApresentacaoBattleIlustrada.cs)). É trabalho repetido potencialmente evitável, mas não há medida que demonstre impacto de desempenho no número atual de itens.
5. **Eventos/coroutines:** o BattleManager registra listeners em botões da cena e cria coroutines de turnos/encerramento ([BattleManager.cs:177–188, 295–299, 416–419, 326–333, 690–777](../../Assets/Scripts/Combat/BattleManager.cs)). Não encontrei assinatura em eventos estáticos ou publisher persistente; os listeners observados pertencem aos botões da própria cena. A guarda `batalhaEncerrada` impede iniciar finalização duplicada e a coroutine do inimigo verifica essa guarda depois da espera. Não há evidência estática de race condition entre cenas nesse trecho.
6. **Checkpoint e dependência de salvamento:** `EncounterZone` grava/prepara contexto antes de carregar Battle ([EncounterZone.cs:155–171](../../Assets/Scripts/Combat/EncounterZone.cs)); a finalização persiste ao destino salvo, depois limpa contexto ([BattleManager.cs:743–764](../../Assets/Scripts/Combat/BattleManager.cs)). Esse ordenamento coincide com o contrato de checkpoint. Outros gatilhos/consumidores não foram auditados neste bloco.

## 5. Divergências e decisões

| Tema | Código | Documento | Conclusão |
|---|---|---|---|
| Encontro da tabela | Filtra desconhecidos, chefes e únicos | [combate.md:39](../sistemas/combate.md) descreve a mesma regra para tabela não vazia | Consistente nesse modo. |
| ID único legado em zona | Não filtra `EhChefe`/`EhUnico` antes de retornar ID | [combate.md:39](../sistemas/combate.md) diz que sem tabela permanece modo anterior; [plano do boss](../../planos-implementacao/2026-10-29-evento-boss-vale.md) orienta boss fora da zona | Ambiguidade de contrato/configuração; ver achado 🟡, não presumir zona atual mal configurada. |
| Vitória, fuga e Game Over | Consolida checkpoint final em vitória/fuga; derrota carrega Game Over | [combate.md:27–30](../sistemas/combate.md) | Consistente para o fluxo com contexto válido; falha de checkpoint mantém estado em memória e avisa. |
| Comentário de persistência das recompensas | Método não salva; o checkpoint vem após o método | Comentário de [BattleManager.cs:565–570](../../Assets/Scripts/Combat/BattleManager.cs) afirma “salva na hora” | Comentário desatualizado; o código atual se alinha ao checkpoint único documentado. |
| Responsividade de Battle | `ApresentacaoBattleIlustrada` escala um ramo local por fator mínimo e centraliza | Plano registra ramo 1672×941, Canvas/Scaler preservado ([combate.md:49–53](../sistemas/combate.md)) | Há uma estratégia local de escala, mas não prova tela responsiva como um todo; análise comparativa fica para BLOCO 06. |
| Progresso de quest por derrota | A leitura integral de `FinalizarBatalha` confirmou que o método não registra progresso de quest diretamente | Achado anterior I-01 em [auditoria geral](2026-10-30-auditoria-geral-projeto.md) | Achado pré-existente confirmado, não repetido. Investigar produtor indireto/contrato no BLOCO 04. |

## 6. Arquivos lidos e limites

### Leitura integral concluída neste bloco

- `Assets/Scripts/Combat/ApresentacaoBattleIlustrada.cs`
- `Assets/Scripts/Combat/ApresentacaoInimigoBattle.cs`
- `Assets/Scripts/Combat/BattleManager.cs` (815 linhas, lido em seis intervalos contínuos)
- `Assets/Scripts/Combat/CombatEngine.cs`
- `Assets/Scripts/Combat/DroidDoIvo.cs`
- `Assets/Scripts/Combat/EncounterZone.cs`
- `Assets/Scripts/Combat/InimigoFixo.cs`
- `Assets/Scripts/Combat/IParticipanteDeCombate.cs`
- `Assets/Scripts/Combat/ResultadoAcao.cs`

### Consultas complementares, não integrais neste bloco

- `docs/contextos/sistemas/combate.md` — trechos pertinentes sobre boss, checkpoint, encontros, Battle ilustrada e status (linhas 1–42 consultadas; documento completo tem conteúdo posterior não usado).
- `docs/planos-implementacao/2026-10-29-evento-boss-vale.md` — seção de comportamento/posicionamento (não lido integralmente).
- `Assets/Scripts/Catalogos/CatalogoDeInimigos.cs` — declaração/campos de `EhChefe` e `EhUnico` e registro do boss (trechos, não integral).
- `docs/wiki/Sistemas-do-jogo.md` — leitura integral consultada como referência geral.
- `docs/wiki/Boss-Droid-do-Ivo.md` — somente busca dirigida.
- `docs/contextos/analises/2026-10-30-auditoria-geral-projeto.md` — lido integralmente antes do bloco.

### Não lidos integralmente neste bloco

Nenhum dos nove scripts de `Assets/Scripts/Combat/` ficou sem leitura integral. As dependências citadas acima fora da pasta Combat foram consultadas apenas nos trechos identificados e não são declaradas como integralmente auditadas aqui. Cenas, prefabs, configurações de Inspector e execução do jogo não foram lidos/inspecionados neste bloco.

**Próximo passo:** salvar este bloco concluído antes de iniciar BLOCO 02. Não aplicar nenhuma correção sem tarefa/autorização separada.
