# Plano de implementação — Novas mecânicas e demo

**Objetivo:** transformar programação, preparação do Droid e ajuda aos moradores em decisões com respostas perceptíveis. As ideias abaixo são propostas, não recursos existentes nem mudanças autorizadas nesta sessão.

**Arquitetura:** aproveitar execução limitada, avaliação de puzzles, eventos de missão, catálogo de inimigos e flags. Cada novidade entra como um consumidor desses contratos; não criar outro motor de quests, outra VM ou uma segunda persistência.

**Atualização de escopo:** recarga e retorno à Casa escolhidos pelo usuário; B1/R1, prévia de custos, redistribuição na Casa (PRO-02), proteção contra stun contínuo e treino seguro (COM-07) acrescentados no plano 03. Treino é P3 opcional. Narrativa adiada nesta rodada; o conteúdo narrativo já proposto abaixo não foi revisado nem aprovado por esta atualização.

**Fontes:** [plano principal](README.md), [terminal e puzzles](01-terminal-e-puzzles.md), [combate](03-combate-e-progressao.md), [missões e mundo](04-missoes-mundo-e-narrativa.md) e [adendo narrativo](../../enredo/TCC_-_ENREDO_v2_ADENDO.md). Execução futura em recortes, conforme [AGENTS.md](../../../AGENTS.md).

## Escolhas de produto

| Decisão | Recomendação proposta | Limite |
|---|---|---|
| D-MEC-01 — resposta à programação | Começar por um puzzle de depuração com três entradas e resposta visual simples | Animação de estados do simulador não equivale a um depurador de cada linha Lua |
| D-MEC-02 — escopo da demo | Uma missão secundária com consequência, um puzzle de depuração e um encontro com intenção legível | Ato 3, rotinas completas e fabricação de peças ficam fora deste recorte |
| D-MEC-03 — ajuda pedagógica | Dicas graduais solicitadas pelo jogador; acesso à solução separado e explícito | Não punir uso de ajuda nem afirmar aprendizado só pelo resultado numérico |
| D-MEC-04 — repetição | Cada nova rodada tem identidade e entradas próprias; recompensa exige regra de disponibilidade | Não reabrir puzzles únicos ou zerar flags para gerar tarefas infinitas |

Não lançar todas as mecânicas de uma vez. A demo usa MEC-01/MEC-02, os padrões de COM-05 e uma consequência de MUN-01. Scanner aprofundado, peças com capacidades, distribuição de recursos e rodadas repetíveis são extensões posteriores.

## MEC-01 — Mostrar o resultado do programa no mundo

**Prioridade:** P2. **Dependências:** PUZ-02 e TER-03. **Arquivos candidatos:** `Assets/Scripts/Catalogos/CatalogoDePuzzles.cs`, integração local em `VerificadorDePuzzle.cs` e `TerminalUIManager.cs`. Novos opcionais: `Assets/Scripts/Scripting/SimulacaoDePuzzle.cs` e `Assets/Scripts/Menus/VisualizacaoDePuzzle.cs`. Montagem limitada ao painel do novo puzzle, com alvo registrado antes da execução.

**Contrato proposto:** `ResultadoSimulacao` contém `idPuzzle`, `idCaso`, `sucesso`, `mensagem` e uma lista limitada de `EstadoDaSimulacao`. Cada estado tem índice, valores permitidos e uma descrição curta. O avaliador produz os estados; a UI apenas os representa. Não expor a tabela global inteira da VM.

- [ ] Escolher um sistema concreto: pequena bomba/filtro com entrada, processamento e saída. Identificar unidades e resultado esperado no enunciado.
- [ ] Definir no máximo 100 estados visuais por caso como limite inicial configurável. Acima disso, resumir a apresentação sem alterar a avaliação e sem consumir memória sem limite.
- [ ] Para transformação de valores, mostrar entrada e saída. Para comandos controlados de simulação, registrar somente operações expostas pela API do puzzle; não apresentar isso como inspeção de cada instrução Lua.
- [ ] Reproduzir a visualização após uma execução limitada, mantendo “entrada”, “resultado obtido” e “resultado esperado” legíveis. Falha mostra o caso divergente e não concede sucesso.
- [ ] Permitir pular animação ou reduzir movimento, sem atraso obrigatório para tentar novamente. Fechar o painel cancela reprodução e libera o bloqueio adquirido.
- [ ] Vincular uma consequência de mundo somente ao evento de conclusão aceito por PUZ-01, nunca ao término da animação.

**Verificação mínima futura:** ampliar o harness de puzzles de PUZ-02 com resultado correto, divergente, vazio, cancelado e lista acima do limite. Por input real, repetir executar/fechar/reabrir sem animações duplicadas ou mudança indevida de flag.

**Aceite:** o jogador consegue identificar qual entrada falhou e comparar o resultado; o feedback permanece compreensível sem animação ou apenas pela cor. O painel não cobre código, botões ou mensagens essenciais nas resoluções de VAL-02.

**Reversão:** desativar apenas a visualização nova e restaurar seus vínculos registrados; preservar avaliação, soluções e progresso já salvo.

## MEC-02 — Resolver programas com defeitos e receber dicas graduais

**Prioridade:** P2. **Dependências:** PUZ-02, TER-03; MEC-01 para a apresentação escolhida da demo. **Arquivos:** `CatalogoDePuzzles.cs`, `CatalogoDeLicoes.cs`, `ApolloDica.cs` e pontos mínimos do terminal. IDs e objetos novos serão registrados na entrega; não substituir o puzzle da CasaLip.

**Recorte proposto:** um desafio optativo apresenta código finito que calcula uma saída errada, com entradas de teste visíveis. Exemplo pedagógico inicial: multiplicação esperada por dois; a versão defeituosa soma dois, passando por coincidência para uma entrada e falhando nas demais. Definir o enunciado narrativo antes de associar item ou recompensa.

**Contrato:** cada definição oferece código inicial, casos e três dicas: observar a divergência, relacionar conceito, sugerir correção. A solução completa exige ação separada. Resultado guarda se dicas/solução foram consultadas, somente para feedback local; isso não altera recompensa de forma escondida.

- [ ] Validar código inicial com casos 0, 5 e 9, saídas esperadas 0, 10 e 18. A variante `entrada + 2` não pode passar no conjunto.
- [ ] Ligar dicas ao tópico existente da aula por ID estável; tópico ausente apresenta fallback explicativo sem exceção.
- [ ] Preservar rascunho ao pedir dica ou executar. “Recomeçar exercício” deve ser ação distinta de limpar log e exigir decisão quando descartar edição.
- [ ] Registrar tentativa, conclusão e uso de ajuda no caderno quando CAD-01 estiver disponível; a avaliação não pode depender da abertura do caderno.
- [ ] Incluir uma pergunta de previsão antes de executar, opcional: “qual será a saída para 5?”. Comparar a previsão com a execução sem transformá-la em bloqueio da campanha.
- [ ] Fazer leitura editorial: o texto ensina uma ideia por exercício e não exige conhecer um comando ainda não apresentado.

**Verificação:** código inicial falha, solução correta passa todos os casos, valor constante passa no máximo os casos coincidentes, erro/cancelamento não conclui. A entrega de recompensa é responsabilidade de MIS-02/DEM-01, não das dicas.

**Aceite:** novo jogador consegue editar, executar, entender uma divergência e tentar novamente sem perder código. Observar compreensão em playtest autorizado; aprovação automática do exercício não comprova aprendizagem.

**Reversão:** retirar a oferta do desafio e suas entradas novas; conservar dados de conclusão já existentes e o comportamento anterior da casa.

## MEC-03 — Scanner tático e informação útil

**Prioridade:** P3, após a demo. **Dependências:** COM-02, COM-04 e COM-05. **Arquivos:** `BattleManager.cs`, `ResultadoAcao.cs`, `CatalogoDeInimigos.cs` e catálogo de peças caso a obtenção seja equipamento. Possível novo `InformacaoDeAnalise.cs`, somente se dados já existentes não bastarem.

**Recorte:** a intenção básica do próximo turno continua visível por COM-05. O scanner revela resistência, uma característica do padrão e a duração de efeito pertinente. Não esconder o anúncio indispensável atrás de uma ação obrigatória.

- [ ] Definir obtenção, duração da informação e preço de uso em uma tabela antes de implementar. Recomendação: ação própria que consome um turno, com informação mantida até o fim daquele encontro.
- [ ] Estender o tipo de ação de COM-02 com `Analisar`; ela sofre dano por turno e stun pelo mesmo caminho que ataque/item/fuga.
- [ ] Fixar a intenção anunciada para a decisão corrente; consultar informação não sorteia nem recalcula um resultado mais favorável. Se o estado exigir cancelá-la, explicar a interrupção.
- [ ] Mostrar uma utilidade concreta: descobrir resistência que torne uma segunda técnica vantajosa. Não prometer adaptação autônoma do chefe se só existe sequência estática.
- [ ] Conservar ataque básico acessível, permitir consulta novamente sem multiplicar bônus e limpar informação no encerramento do encontro.

**Verificação/aceite:** analisar sob stun não revela indevidamente; análise válida consome exatamente um turno; informação corresponde aos atributos efetivos de COM-01; a próxima ação é a anunciada ou sua interrupção explicada.

**Reversão:** retirar a opção de análise e restaurar catálogo/painel local; preservar padrões básicos e regras compartilhadas de turno.

## MEC-04 — Peças que oferecem capacidades

**Prioridade:** P3. **Dependências:** EST-01, INV-01 e PUZ-02; MIS-02 quando houver obtenção por quest. **Arquivos:** `CatalogoDeItens.cs`, `DroidPart.cs`, `Droid.cs`, `BagUIManager.cs` e o verificador do desafio optativo escolhido.

**Contrato proposto:** conjunto derivado `CapacidadesAtivas` calculado pelos IDs das peças equipadas; não persistir outra cópia contraditória do mesmo conjunto. Primeira capacidade sugerida: sensor que disponibiliza uma leitura adicional em puzzle optativo. Estatísticas numéricas continuam usando o contrato de equipamento existente.

- [ ] Definir uma única peça, fonte verificável de obtenção, slot, capacidade e descrição antes de ampliar o catálogo.
- [ ] Mostrar na comparação da Bag o atributo e a capacidade ganhos/perdidos.
- [ ] Ao abrir desafio, mostrar requisito e rota de obtenção. Se o jogador puder vender/perder a peça, manter recompra ou alternativa acessível; não bloquear missão principal por equipamento removível.
- [ ] Congelar capacidades usadas na sessão de simulação ou invalidá-la explicitamente ao trocar peça, evitando avaliação com dois estados diferentes.
- [ ] Restaurar equipamento por EST-01 e recalcular capacidades; não usar nome ou raridade como chave.

**Verificação/aceite:** equipar habilita, desequipar retira, carregar restaura, peça desconhecida não concede capacidade. Nenhuma combinação remove a única saída da campanha. Validar uma rota optativa completa antes de criar outras capacidades.

**Reversão:** desabilitar o requisito do desafio novo e retirar sua oferta quando necessário; preservar a peça e seu valor numérico em saves existentes até haver migração explícita.

## MEC-05 — Escolhas técnicas sobre recursos da comunidade

**Prioridade:** P3. **Dependências:** PUZ-02, MIS-02, MUN-01 e NAR-01. **Arquivos:** catálogos de puzzles/quests/diálogos e controlador local da consequência. Nova simulação somente se MEC-01 não representar o problema.

**Recorte proposto:** distribuir capacidade limitada entre dois consumidores, com limites mínimos claros e duas soluções viáveis. Recomendação inicial: exercício isolado com orçamento 10 e consumidores que precisam de pelo menos 3 cada; valores são de protótipo, não balanceamento aprovado da campanha.

- [ ] Definir variáveis, unidades, restrições e consequências antes da interface. Rejeitar negativos, soma acima do orçamento e overflow.
- [ ] Permitir prévia sem gravar escolhas; confirmar aplica uma flag de resultado e uma consequência compreensível, uma única vez.
- [ ] Apresentar benefícios e custos de ambas as opções. Evitar “resposta moral correta” oculta ou promessas de mudança global sem montagem correspondente.
- [ ] Manter variante narrativa coerente com o adendo e sem antecipar solução do problema central da cidade.
- [ ] Separar validade técnica de escolha narrativa: várias configurações válidas podem continuar a campanha.

**Verificação/aceite:** ao menos duas distribuições válidas; inválida não muta mundo; confirmar/recarregar não duplica recompensa; falas e objetos correspondem à opção escolhida.

**Reversão:** retirar a nova oferta e restaurar objetos alterados; preservar a flag em saves e documentar fallback de leitura para campanhas que já escolheram.

## MEC-06 — Rodadas repetíveis de puzzles

**Prioridade:** P3. **Dependências:** PUZ-02, CAD-01, MIS-01/MIS-02 e EST-02. **Arquivos:** `VerificadorDePuzzle.cs`, `CatalogoDePuzzles.cs`, `CatalogoDeQuests.cs`, `GerenciadorDeQuests.cs` e DTO/migração de save sob coordenação de EST-01.

**Problema de integração:** a missão `q_ivo_terca` é repetível, mas a conclusão de um puzzle único não fornece uma nova rodada jogável. Até esta entrega, MIS-01 deve impedir um segundo aceite que não tenha um objetivo executável.

**Contrato proposto:** `RodadaDePuzzle` contém ID do puzzle, contador monotônico da rodada, parâmetros/seed, estado e resultado. Persistir somente rodada atual e resumo agregado limitado; não manter histórico ilimitado de GUIDs, código ou tentativas. `idOcorrencia` combina produtor estável e contador para deduplicação de MIS-01.

- [ ] Aprovar disponibilidade: por marco da história ou intervalo de jogo controlado; não usar relógio do computador como garantia antifraude. Definir recompensa e limite antes de permitir farm.
- [ ] Gerar e persistir rodada no aceite, com entradas determinísticas reproduzíveis. Recarregar mantém a rodada e não gera prêmio novo.
- [ ] Reabrir o verificador para a rodada corrente sem apagar conclusão histórica, flag da casa ou solução do primeiro sucesso.
- [ ] Registrar tentativa por rodada; estender CAD-01 com migração própria na próxima versão disponível. Conservar apenas primeira solução histórica e última solução/rascunho conforme limites definidos.
- [ ] Entrega consome a conclusão daquela rodada; a próxima só nasce por novo aceite elegível. Reenvio de evento/entrega não paga novamente.

**Verificação/aceite:** primeira e segunda rodadas exigem execuções diferentes identificadas; recarga no meio mantém entradas; evento antigo não conclui rodada nova; não há crescimento ilimitado do save.

**Reversão:** interromper novos aceites e preservar a rodada já aceita com caminho de conclusão ou cancelamento explícito; não apagar contadores para contornar incompatibilidade.

## DEM-01 — Montar uma fatia completa e verificável

**Prioridade:** P2. **Dependências obrigatórias:** PRO-01, MIS-02, UI-01, MUN-01, NAR-01, COM-05/COM-06, MEC-01/MEC-02, CAD-01, INV-01/ECO-01 e os contratos P1 de que dependem. **Critério de saída:** VAL-04. Mecânicas P3 não bloqueiam esta entrega.

**Arquivos candidatos:** somente catálogos e objetos que compõem o percurso aprovado em `CasaLip`, `City` e `Battle`; nenhum mapa novo é necessário por princípio. Antes de editar, registrar caminhos exatos dos objetos, componentes, referências, valores anteriores e autoria das alterações locais existentes.

**Percurso proposto:**

1. Jogador resolve a casa pelo contrato vigente e chega a City. Pendências históricas de retomada/input/visual permanecem critérios reais, não são encerradas pela existência deste plano.
2. A missão principal recebe as conversas e entrega previstas; 120 XP habilitam os cinco pontos usados na preparação inicial de PRO-01.
3. Jogador aceita a missão existente `q_agua_dara`. O novo desafio optativo de depuração oferece uma fonte alcançável de `filtro_de_agua`; essa ligação é proposta e precisa ser montada, pois o catálogo sozinho não comprova obtenção no mundo.
4. Concluir o desafio concede o filtro uma vez pelo produtor definido no recorte. MIS-02 consome o filtro ao entregar a quest, aplica `quest_dara_filtro_ok` e concede somente a recompensa do catálogo. O puzzle não entrega também a recompensa da quest.
5. MUN-01 mostra uma alteração local no abastecimento de Dara e uma fala correspondente. Não declarar que toda a cidade recuperou a água. O adendo prevalece sobre uma interpretação improvisada.
6. Jogador encontra um inimigo de padrão legível, testa duas alternativas viáveis, vence/foge ou retorna ao último checkpoint da Casa após derrota. Autosave feito fora da Casa não muda esse destino. Os atributos do catálogo e as regras de turno/recarga realmente participam do resultado.
7. Equipamento, missão, caderno e estado de mundo sobrevivem a salvar, encerrar e carregar em ambiente de validação autorizado.

**Passos:**

- [ ] Fechar tabela de IDs, produtores de recompensas, pré-requisitos e objetos da fatia; validar referências e disponibilidade em NAR-01.
- [ ] Confirmar que a entrega de Dara consome exatamente o item exigido e não depende de venda/compra de item protegido. Definir tratamento se o filtro já foi obtido antes de aceitar.
- [ ] Registrar percurso de campanha nova e retomada intermediária, incluindo ordem alternativa de conversas e puzzle anterior ao aceite.
- [ ] Demonstrar três respostas: saída do programa, reação do morador e intenção do adversário.
- [ ] Executar a matriz mínima de VAL-02 e revisão de entrega VAL-04; coletar medidas locais, sem telemetria de terceiros.
- [ ] Registrar bloqueios reais e encaminhar novas ideias ao backlog; não acrescentar expansão durante a validação da fatia.

**Aceite:** percurso possível sem comando de teste, item inacessível, recompensa duplicada, perda de equipamento ou reinício forçado. Jogador entende por que programar ajuda alguém e como preparação muda sua decisão de combate. Tempo, tentativas, dicas e compreensão são observações de playtest, não metas numéricas já aprovadas.

**Reversão:** desativar oferta/gatilho novos de forma localizada, preservar progresso já gravado e restaurar montagem pelo diff autorizado. Restaurar save somente em fixture ou mediante autorização e verificação de que não contém progresso novo.

## Extensões adiadas e retomada

Rotinas por períodos estão em MUN-02; aprendizado persistente está em CAD-01. Fabricação/reparo com economia própria, combate controlado por programa autônomo, múltiplos slots, cooperação, geração procedural e Ato 3 completo exigem planos próprios. Migração para ScriptableObjects, reorganização de pastas e remoção de legado só devem ocorrer por necessidade demonstrada, preservando GUIDs e consumidores.

**Estado da sessão:** somente planejamento. Nenhuma dessas mecânicas foi implementada ou testada, e não houve edição de objetos, execução de Lua ou acesso a save. Próximo passo futuro: selecionar a fatia, fechar D-MEC-01 a D-MEC-04 conforme os itens escolhidos e seguir a ordem do [plano principal](README.md). Unity não consultado; backups de gameplay não foram necessários nesta sessão.
