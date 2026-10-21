# Combate e progressão — plano de implementação

> Para execução futura: seguir `superpowers:executing-plans`, tarefa por tarefa, com um agente por padrão. Os checkboxes representam trabalho proposto; este documento não autoriza implementação.

**Objetivo:** tornar catálogo, ações, efeitos, recompensas e progressão coerentes, permitindo concluir a introdução sem comandos de teste.

**Arquitetura proposta:** preservar `BattleManager` como coordenador da apresentação e das transições, `CombatEngine` como responsável pelo turno e os participantes como responsáveis pelos atributos e ações. Introduzir somente regras pequenas de técnicas e padrões de inimigos; não executar Lua durante a batalha.

**Tecnologias:** Unity, C#, UGUI/TextMeshPro, catálogos existentes e harnesses manuais de Editor.

**Especificação e estado:** [plano geral](README.md), análise estática dos 62 scripts realizada em 20/10/2026 e [contrato de combate](../../contextos/sistemas/combate.md). Documento proposto; nenhum item implementado ou testado nesta tarefa.

## Limites e fontes

- Autorização atual: somente este arquivo; não alterar scripts, cenas, prefabs, saves, pacotes ou configuração.
- Na execução futura, registrar antes de cada edição os arquivos autorizados, valores anteriores, consumidores e reversão. Um arquivo candidato abaixo não está autorizado agora.
- Preservar GUIDs, `.meta`, IDs de catálogo, técnicas e recompensas existentes. Não reorganizar namespaces.
- Não mudar Canvas/Scaler. Qualquer montagem de UI exige delimitar cena, objeto e propriedades em etapa própria.
- As alterações numéricas de combate exigem decisão de design e playtest autorizado; não interpretar um teste lógico como balanceamento aprovado.
- Validar lógica por fixtures isoladas; validar fluxo por input real; validar apresentação por comparação visual. Registrar cada evidência separadamente.
- Testes que gravem dados devem usar diretório temporário e restaurar estado global em `finally`; testes com save pessoal exigem backup e restauração verificada.
- Não assumir `Assets/Tests` ou infraestrutura nova. Reutilizar o padrão de [ValidacaoCombate.cs](../../../Assets/Editor/ValidacaoCombate.cs).

| Fonte existente | Evidência relevante, sem afirmar validação de gameplay |
|---|---|
| [BattleManager.cs](../../../Assets/Scripts/Combat/BattleManager.cs) | `AplicarInimigoDoCatalogo` transfere atributos básicos/recompensas; não transfere todos os atributos derivados nem aplica as três restrições do catálogo. Itens e fuga contornam o processamento completo do turno. |
| [InimigoFixo.cs](../../../Assets/Scripts/Combat/InimigoFixo.cs) | Oferece somente `Atacar`; defesa fixa não incorpora efeitos de atributo, tornando inefetiva a redução de defesa prometida pela Granada de Ácido. |
| [CombatEngine.cs](../../../Assets/Scripts/Combat/CombatEngine.cs) | Processa dano por turno, stun e decremento quando chamado para ataque; não define o ciclo completo de todos os tipos de ação. |
| [TecnicaComposta.cs](../../../Assets/Scripts/Scripting/TecnicaComposta.cs) e [TerminalDroidApi.cs](../../../Assets/Scripts/Scripting/TerminalDroidApi.cs) | O custo de veneno ignora `DanoPorTurno`; valores positivos não têm teto de domínio; não existe limite de execução por energia/cooldown. |
| [Catálogo de quests](../../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs) e [diálogos](../../../Assets/Scripts/Catalogos/CatalogoDeDialogos.cs) | A missão principal inicial prevê 120 XP; tutorial solicita técnica de custo 2 e DEX +3; o estado inicial tem zero pontos e a integração da rota precisa ser concluída. |
| [Contexto de combate](../../contextos/sistemas/combate.md) | Checkpoint anterior é preservado na derrota; não há política completa de recuperação. Efeitos temporários não têm limpeza completa de encerramento. |

## Decisões e ordem

| Decisão e estado | Direção registrada | Consequência |
|---|---|---|
| D-COM-01 — recarga escolhida | Usuário escolheu recarga por técnica. Contar nos turnos do dono; ataque básico permanece disponível. | COM-04 usa a tabela B1 abaixo; energia sai das alternativas desta rodada. |
| D-COM-02 — ciclo dos efeitos | Duração por turno do dono; dano por turno antes da ação; todas as ações na mesma pipeline; renovar sem empilhar o mesmo efeito/origem; temporários restritos à batalha. | COM-02 depende de aprovação dessas regras, inclusive stun sobre item/fuga. |
| D-COM-03 — retorno escolhido | Usuário definiu último checkpoint na Casa. Não usar automaticamente o autosave anterior à luta como ponto de retorno por derrota. | COM-06/EST-02 precisam distinguir estado corrente e snapshot da Casa. A política detalhada de rollback é proposta abaixo. |
| D-COM-04 — rebalanceamento solicitado | Tabela inicial B1 com custos, limites e exemplos; redistribuição em PRO-02. | Valores são proposta para implementação futura e playtest, sem resultado de balanceamento observado. |
| D-PRO-01 — primeira preparação | Ligar `q_main_agua` → 120 XP → nível 2/5 pontos → técnica nível 2 e DEX +3 antes da exigência do tutorial. | PRO-01 depende da integração de missão; não distribuir pontos gratuitos em paralelo. |

## Tabela B1 — Proposta inicial de pontos, custos e limites

**Status:** números propostos pelo planejamento após a direção do usuário. Não aplicados ao jogo. O rebalanceamento começa pela relação entre orçamento e custo, mantendo inicialmente os cinco pontos por nível e a curva de XP existente para não alterar simultaneamente todas as variáveis.

| Componente | Custo B1 | Limite inicial proposto |
|---|---|---|
| Pontos ao subir de nível | +5 por nível efetivamente ganho | Sem bônus por recarregar, redistribuir ou repetir recompensa |
| FOR/AGI/VIT/INT/DEX/LUK | 1 ponto por incremento permanente comprado | Até +10 comprados por atributo, acima da base da campanha; equipamento/buff não entra nesse contador |
| Dano direto de técnica | 1 ponto por nível de dano | Nível de dano de 1 a 10; esse nível não equivale ao dano final em HP |
| Envenenamento | 1 ponto × dano por turno × duração | Dano por turno de 1 a 3; duração de 1 a 3 turnos |
| Atordoamento (`Stun`) | 3 pontos × duração | Duração de 1 turno; protege contra bloqueio contínuo conforme COM-02 |
| Efeitos por técnica | Soma dos componentes | Até 2: no máximo um veneno e um stun, sem duplicatas ou nomes desconhecidos |
| Técnicas configuráveis | Sem taxa adicional por slot | Até 4 aprendidas, além do ataque básico protegido |
| Melhorar técnica | Custo B1 do candidato menos custo B1 da técnica atual | Cobrar apenas diferença positiva, com validação antes de mutar |
| Esquecer/redistribuir | Reembolso do investimento efetivamente registrado | Sem taxa na proposta; não calcular devolução pelo preço de catálogo novo |

**Fórmula:** `CustoB1 = NivelDeDano + (DanoVeneno × TurnosVeneno) + (3 × TurnosStun)`, omitindo os efeitos ausentes. Nomes não definem preço ou recarga. Alterar nome ou criar duplicata não deve mudar as regras do efeito.

**Exemplos verificáveis de custo e recarga:**

| Exemplo de configuração, sem criar entrada de catálogo agora | Dano direto | Veneno | Stun | Custo B1 | Recarga |
|---|---:|---|---|---:|---:|
| Ataque básico existente | Preservar definição | — | — | 0 | 0 |
| Impacto | 2 | — | — | 2 | 1 |
| Pulso tóxico | 1 | 1 por 2 turnos | — | 3 | 2 |
| Veneno intenso | 2 | 3 por 2 turnos | — | 8 | 2 |
| Choque | 1 | — | 1 turno | 4 | 3 |
| Técnica combinada | 2 | 1 por 2 turnos | 1 turno | 7 | 3 |

Isso mantém `Impacto 2 + DEX +3 = 5` na preparação inicial. Uma técnica simples com veneno cabe nos primeiros cinco pontos; uma combinação de controle exige reservar pontos de mais de um nível. São hipóteses de ritmo, não comprovação de dificuldade.

**Legado e mudança de preços:** técnica antiga permanece utilizável até revisão explícita da migração. Investimento reconhecido usa fórmula histórica comprovada; exemplo v6 de dano 2 + veneno 3 por 2 turnos custou 6, enquanto B1 custaria 8. Esquecer devolve 6, não 8. Melhorar dano de 2 para 3 custa 1 e passa o investimento a 7. Caso um preço futuro seja menor, nunca gerar crédito automático por uma diferença negativa.

**Acima dos limites em saves antigos:** não truncar técnica, atributo ou slot silenciosamente. Carregar o dado compatível em estado de legado identificado, impedir novos aumentos que agravem o excesso e oferecer conversão/redistribuição pelo investimento comprovado. Ataque básico e acesso à Casa permanecem disponíveis. Falta de origem comprovada bloqueia somente a devolução estimada, não apaga a campanha.

## Tabela R1 — Recarga escolhida e proposta de intervalos

| Configuração | Oportunidades seguintes do dono bloqueadas |
|---|---:|
| Ataque básico | 0 |
| Dano direto nível 1 a 3, sem efeito | 1 |
| Dano direto nível 4 a 6, sem efeito | 2 |
| Dano direto nível 7 a 10, sem efeito | 3 |
| Técnica com veneno | Maior entre 2 e a faixa de dano direto |
| Técnica com stun, inclusive combinada com veneno | Maior entre 3 e a faixa de dano direto |

Proposta: técnicas compartilham **família de recarga** derivada de seu componente, com prioridade `Stun` → `Veneno` → `DanoDireto`. Ao usar uma técnica, atualizar a disponibilidade dela e de sua família pelo intervalo da técnica usada. Uma técnica só está disponível quando ambos zerarem. A família usa o maior prazo já existente, sem encurtá-lo ao receber outro uso. Assim, quatro cópias com nomes diferentes não contornam a espera; ataque básico fica fora das famílias. Exibir também o motivo “família em recarga”. O mecanismo é um único sistema de recarga, sem barra adicional de energia.

**Prévia de configuração:** antes de confirmar aprendizagem, melhoria ou redistribuição, exibir custo total, diferença a pagar, saldo restante, recarga/família e efeito esperado em linguagem simples. Simular não gasta pontos, não executa batalha, não grava save e não concede recompensa. Confirmação revalida o estado e aplica uma única vez.

**Sequência recomendada:** COM-01 → COM-02 → COM-03 → COM-04 → COM-05; COM-06 após COM-02 + EST-02; PRO-01 após MIS-01/MIS-02 e regras estáveis de técnicas.

PRO-02 entra após TER-02 + COM-03 + EST-01/EST-02; COM-07 fica depois da demo e não bloqueia a correção do combate. A Casa precisa de ponto de registro seguro por EST-02 antes de testar retorno por derrota.

**Coordenação externa:** [estado/economia](02-estado-inventario-e-economia.md) possui EST-01, schema incremental por capacidade, começando por equipamentos, e EST-02, restauração integral. [Missões/mundo](04-missoes-mundo-e-narrativa.md) possui MIS-01, progresso sem salvar dentro de Battle, e MIS-02, oferta/entrega/automáticas. [Terminal](01-terminal-e-puzzles.md) possui TER-02, transações de comandos, além da exposição Lua e do caderno; não duplicar esse trabalho. A rota inicial não depende das transações de compra/venda de ECO-01.

**Riscos de revisão:** morte por veneno ao escolher item (COM-02); comando de fuga em chefe por chamada direta (COM-01); overflow/reembolso após mudança de custo (COM-03); derrota com equipamentos e quest em memória (COM-06); missão concluída duas vezes concedendo pontos (PRO-01).

## COM-01 — Aplicar o catálogo e respeitar restrições

**Prioridade:** P1. **Dependências:** nenhuma implementação anterior; coordenar emissão de progresso com MIS-01.

**Escopo candidato existente:** `Assets/Scripts/Combat/BattleManager.cs`, `Assets/Scripts/Combat/InimigoFixo.cs`, `Assets/Scripts/Combat/EncounterZone.cs`, `Assets/Scripts/Historia/GatilhoDeBatalha.cs`, `Assets/Scripts/Catalogos/CatalogoDeInimigos.cs` e `Assets/Editor/ValidacaoCombate.cs`.

**Novo proposto:** `Assets/Scripts/Combat/RegrasDeEncontro.cs`; `Assets/Editor/ValidacaoRegrasCombate.cs`, compartilhado pelas tarefas COM-01 a COM-05.

**Consumidores:** entradas por zona/gatilho, montagem da Battle, ação de fuga, recompensas e flags de história. Não mudar valores de HP/ATK/DEF/XP para compensar a integração.

**Contratos propostos:**

- `InimigoFixo.Criar(DefinicaoDeInimigo definicao) : InimigoFixo` copia HP/ataque/defesa/XP, `Hit`, `Flee`, crítico e resistência; conservar construtor atual para fallback explícito/fixtures.
- `RegrasDeEncontro.PodeIniciar(DefinicaoDeInimigo definicao, GerenciadorDeEstado estado, out string motivo) : bool` centraliza a regra de inimigo único já vencido.
- `RegrasDeEncontro.ChaveVitoriaUnica(string idInimigo) : string` usa `inimigo_unico_<id>_derrotado`, sem substituir flags narrativas existentes.
- `PermiteFugir` governa botão e comando; `EhUnico` governa nova entrada; `EhChefe` identifica apresentação/padrão. Ser chefe não implica proibir fuga independentemente do campo explícito.
- Gravar flag de único somente na vitória consolidada uma vez; fuga/derrota não a gravam. Usar o mecanismo existente de flags, sem nova versão de schema só para esta regra.

- [ ] Registrar casos no harness: inimigo com `Hit=18/Flee=14` preserva valores; resistência/crítico do catálogo chegam ao participante.
- [ ] Confirmar falha desses casos no comportamento anterior, em fixture isolada, antes da edição.
- [ ] Centralizar criação e validar identidade do inimigo; ID não vazio desconhecido deve gerar diagnóstico e não iniciar silenciosamente outro inimigo; ID vazio mantém fallback documentado.
- [ ] Aplicar bloqueio de fuga na UI e na execução; rejeição não consome turno.
- [ ] Aplicar mesma verificação de único em zona e gatilho, antes de criar checkpoint; respeitar flag narrativa herdada quando a relação for conhecida.
- [ ] Testar vitória repetida sem duplicar flag/prêmio; derrota e fuga ainda permitem reencontro; inimigo comum continua repetível.
- [ ] Executar os casos afetados do harness existente e do novo; registrar resultado real, sem presumir aprovação visual.

**Aceite:** atributos da ficha correspondem aos usados no combate; chefe com fuga proibida informa o motivo; único vencido não inicia outra luta pelos dois caminhos.

**Reversão:** restaurar somente os trechos deste item e fixtures; guardar cópia das flags de teste. Não apagar flags reais de inimigos vencidos para forçar reexecução.

## COM-02 — Unificar turnos, efeitos e atributos temporários

**Prioridade:** P1. **Dependências:** COM-01; decisão D-COM-02.

**Existentes candidatos:** `Assets/Scripts/Combat/CombatEngine.cs`, `Assets/Scripts/Combat/BattleManager.cs`, `Assets/Scripts/Combat/InimigoFixo.cs`, `Assets/Scripts/Combat/IParticipanteDeCombate.cs`, `Assets/Scripts/Combat/ResultadoAcao.cs`, `Assets/Scripts/Droid/Droid.cs`, `Assets/Scripts/Scripting/EfeitosTemporariosUtil.cs`, `Assets/Scripts/Scripting/TecnicaComposta.cs` e `Assets/Editor/ValidacaoCombate.cs`; ampliar também `Assets/Editor/ValidacaoRegrasCombate.cs`, proposto em COM-01.

**Novo proposto:** `Assets/Scripts/Combat/AcaoDeCombate.cs`. Catálogo de itens é consumidor de regras; seus valores e descrições só mudam se a regra aprovada exigir, em diff separado.

**Contrato proposto:** sobrecarga `CombatEngine.ExecutarTurno(IParticipanteDeCombate atacante, IParticipanteDeCombate alvo, AcaoDeCombate acao) : ResultadoAcao`; manter a assinatura com nome como adaptador dos ataques.

- `AcaoDeCombate` carrega `TipoAcaoDeCombate` (`Ataque`, `Item`, `Fuga`) e `Func<ResultadoAcao> Executar`; o callback só é chamado depois de DoT/stun.
- Acrescentar `ResultadoAcao.ConsomeTurno` e `ResultadoAcao.EncerraBatalha`; erro de configuração/seleção é recusado antes de abrir turno; ação válida perdida por stun consome turno.
- Ordem aprovada deve ser única: validar escolha → DoT do ator → verificar morte → stun → executar ação → verificar desfecho → decrementar efeitos elegíveis → liberar próxima ação/encerrar.
- Item é debitado apenas quando seu callback executa; morte por DoT/stun não gasta estoque. Tentativa válida de fuga falha consome turno; fuga proibida é rejeitada antes da pipeline.
- Efeitos ganham `IdOrigem`; mesma combinação de origem + tipo + atributo renova duração para o maior prazo e substitui magnitude, sem somar instâncias. Origens distintas permanecem distintas.
- Aplicar cópias de efeitos, nunca as instâncias de catálogo/técnica; expirar um alvo não modifica a definição nem outro participante.
- Decrementar somente efeitos presentes no início e não renovados/criados no turno atual; buff usado sobre si começa sua contagem no próximo turno do dono.
- Temporários são removidos de ambos os participantes em vitória/fuga/derrota e ao descartar sessão; técnicas aprendidas, peças e atributos base permanecem.
- Proposta de proteção contra stun contínuo: depois de perder uma oportunidade por stun, o alvo fica imune a novos stuns até encerrar sua próxima oportunidade de ação. A imunidade é do alvo, compartilhada entre atacantes/técnicas, e não impede dano direto/veneno. Reaplicar stun durante imunidade não renova o bloqueio; a técnica efetivamente usada ainda consome turno e inicia recarga. A UI informa a imunidade. Limpar essa proteção junto dos efeitos no encerramento.
- `InimigoFixo` calcula defesa/ataque/acerto/esquiva a partir de base + efeitos compatíveis; mapear VIT→defesa, FOR→ataque, DEX→Hit, AGI→Flee, INT/LUK pelos coeficientes existentes, sem inventar nova fórmula.
- Para inimigo sem VIT base, debuff de VIT altera defesa, não HP máximo. Para Droid, preservar derivação existente de VIT/HP máximo, limitar HP ao teto ao expirar e nunca curar por reaplicar buff.

- [ ] Escrever casos: ator com HP 3 e veneno 3 escolhe poção/fuga/ataque → morre antes de agir, sem consumo de item ou contra-ataque extra.
- [ ] Escrever casos: stun de um turno bloqueia item e fuga uma vez; ação seguinte é liberada; seleção inválida não avança DoT nem duração.
- [ ] Escrever casos: ácido de catálogo reduz defesa 10 para 5 por três turnos do alvo; expiração restaura 10; mesma origem reaplicada não produz defesa 0.
- [ ] Implementar pipeline e retirar processamento duplicado das rotas de item/fuga; conservar bloqueio de cliques durante animação/encerramento.
- [ ] Implementar aplicação/renovação e decremento com snapshot dos efeitos elegíveis; testar buff de um turno usado sobre si e renovação no próprio turno.
- [ ] Implementar limpeza idempotente e testar fuga, vitória, derrota, início de outra luta e redução temporária do HP máximo.
- [ ] Testar duas técnicas com nomes distintos tentando manter stun: a segunda não impede a oportunidade protegida; depois dela a imunidade expira. Morte por DOT segue sendo verificada antes da ação.
- [ ] Executar testes de regras e regressões de encerramento; em sessão autorizada, conferir log, HP e consumo por input real.

**Aceite:** escolher item/fuga não evita veneno/stun; ácido modifica o dano recebido conforme defesa efetiva; nenhuma condição temporária vaza para exploração/nova luta.

**Reversão:** restaurar conjuntamente pipeline, adaptadores e aplicação de efeitos; nunca misturar callbacks novos com rotas antigas. Salvar fixtures anteriores para comparação.

## COM-03 — Validar técnicas e cobrar custo coerente

**Prioridade:** P1. **Dependências:** D-COM-04; coordenar transação de comandos com TER-02 e migração do investimento com EST-01. Definir/validar a regra não exige concluir EST-01 antes, mas entregar o novo reembolso exige incluir a migração correspondente no mesmo pacote.

**Existentes candidatos:** `Assets/Scripts/Scripting/TecnicaComposta.cs`, `Assets/Scripts/Scripting/TabelaDeCustos.cs`, `Assets/Scripts/Scripting/TerminalDroidApi.cs`, `Assets/Scripts/Scripting/PontosDeProgressao.cs` e `Assets/Scripts/Droid/Droid.cs`; ampliar `Assets/Editor/ValidacaoRegrasCombate.cs`, proposto em COM-01.

**Novos propostos:** `Assets/Scripts/Scripting/RegrasDeTecnica.cs` e `Assets/Scripts/Scripting/LimitesDeTecnica.cs`.

**Consumidores:** aprendizado, melhoria, esquecimento, carregamento de técnicas e catálogo de lições. A camada Lua deve delegar à regra C# compartilhada.

**Contratos propostos:**

- `RegrasDeTecnica.TentarValidarECustear(TecnicaComposta tecnica, out int custo, out string erro) : bool` não altera pontos/técnica e rejeita nome inválido, efeito desconhecido e parâmetros fora dos limites.
- `LimitesDeTecnica` representa B1, incluindo dano, duração, efeitos, slots e contadores de atributo comprado; separar tetos de configuração de estatísticas finais com equipamento. Registrar a versão de tabela usada na execução futura.
- Calcular em aritmética verificada; overflow falha sem mutação. Rejeitar zero/negativo quando a operação exige incremento; `PontosDeProgressao.TentarGastar` não pode conceder saldo para quantidade negativa.
- Proposta B1: dano direto mantém coeficiente 1; veneno passa a coeficiente 1 por unidade de dano por turno × duração; stun mantém 3 por turno e teto de um turno. Coeficiente legado de veneno 2 continua sendo usado apenas para reconhecer investimento antigo.
- Exemplo B1: dano direto nível 2 + veneno 3 por 2 turnos custa `2 + 3 × 2 = 8`; dano de veneno maior aumenta custo. Esta proposta substitui o exemplo anterior de 14 pontos.
- Aprender/melhorar valida candidato separado e só então debita/aplica; melhoria cobra a diferença entre candidato e técnica atual, ambos calculados pela mesma tabela vigente; falha deixa pontos/técnica idênticos.
- Política recomendada para legado: preservar técnica já comprada; em save v6 de origem comprovada, reconhecer investimento com a fórmula histórica conhecida: nível de dano × coeficiente legado de dano + soma do custo legado por turno × duração de cada efeito, inclusive veneno sem multiplicar seu dano. Nunca usar preços novos para esse cálculo; somar apenas novos débitos ao investimento reconhecido.
- Versão/origem sem fórmula comprovada exige decisão explícita de recuperação, sem inventar investimento ou habilitar reembolso pelo preço novo. EST-01 incorpora o campo/migração incremental ao pacote antes da entrega do reembolso; não fixar aqui uma versão futura do schema.

- [ ] Usar B1/R1 como base proposta do recorte; registrar qualquer ajuste posterior com motivo e versão, sem alterar números por tentativa durante correção de bug.
- [ ] Criar testes para cada teto e teto+1, zero/negativo, `int.MaxValue`, nome vazio/reservado, duplicata e efeito desconhecido.
- [ ] Testar técnica B1 de 8 pontos com saldo 7 → falha sem mudança; saldo 8 → sucesso e saldo zero; dano 10 aceita e 11 recusa; veneno 3/3 aceita e 4/3 ou 3/4 recusa; stun 2 recusa.
- [ ] Implementar regra central e substituir cálculo não validado nas entradas C#; não depender somente da validação do texto Lua.
- [ ] Testar melhoria que excede teto/saldo, ausência de alteração parcial e esquecimento com reembolso limitado ao investimento registrado.
- [ ] Coordenar fixtures com EST-01: técnica v6 de dano 2 + veneno 3 por 2 turnos reconhece custo histórico 6, embora B1 custe 8; melhoria de um nível acrescenta somente 1 ao investimento. Versão não comprovada não recebe reembolso estimado. Quinta técnica configurável e segundo efeito do mesmo tipo são recusados sem débito.
- [ ] Atualizar exemplos do terminal pelo plano responsável e executar harness de regras; não testar execução Lua infinita.

**Aceite:** nenhuma entrada inválida produz técnica/saldo parcial; dano por turno influencia o preço; aprender/melhorar/esquecer não cria pontos.

**Reversão:** guardar cópias de fixtures de técnicas/saldo; reverter regras e chamadores como unidade. Não abrir save migrado em versão antiga sem compatibilidade comprovada.

## COM-04 — Limitar repetição de técnicas durante a luta

**Prioridade:** P2. **Dependências:** COM-02/COM-03. Recarga escolhida pelo usuário; intervalos e famílias propostos em R1.

**Existentes candidatos:** `Assets/Scripts/Combat/BattleManager.cs`, `Assets/Scripts/Combat/CombatEngine.cs`, `Assets/Scripts/Combat/ResultadoAcao.cs` e `Assets/Scripts/Scripting/TecnicaComposta.cs`; ampliar `Assets/Editor/ValidacaoRegrasCombate.cs`, proposto em COM-01.

**Novo proposto:** `Assets/Scripts/Combat/ControleDeRecarga.cs`, contendo disponibilidade por técnica e família, sem sistema paralelo de energia.

**Consumidores:** lista de ataques, execução por comando, turnos do participante, encerramento e testes de IA. Cooldown é estado da sessão, não progresso permanente.

**Contrato proposto:** `ControleDeRecarga.PodeUsar(string idTecnica, out int turnosRestantes) : bool`, `RegistrarUso(string idTecnica, string familia, int intervalo) : void`, `EncerrarTurnoDoDono() : void`, `Limpar() : void`. Configurar o mapa técnica → família uma vez na entrada da luta, a partir de R1; `PodeUsar` retorna a maior espera entre ambas. Guardar a origem do bloqueio para a mensagem da UI.

- Identidade inicial pode usar o nome único existente da técnica, fixado na entrada da batalha; não permitir editar técnicas durante a luta.
- Uma técnica com intervalo 2 fica indisponível nas duas próximas oportunidades do dono e libera na terceira; o turno do uso não desconta a recarga recém-criada.
- Começar recarga quando a técnica é efetivamente executada, mesmo se errar; stun/morte antes de agir não inicia recarga. Item/fuga frustrada também avançam recargas anteriores.
- Ataque básico não entra em recarga. Usar os intervalos propostos de R1, sem apresentá-los como números já balanceados por playtest.

- [ ] Registrar R1 no recorte e validar as fronteiras de dano 3/4 e 6/7, família de veneno e prioridade de stun nas combinadas.
- [ ] Criar testes de intervalo 0 e 2, erro de ataque, stun, item, fuga frustrada e limpeza ao sair.
- [ ] Integrar consulta e marcação ao mesmo ponto de execução do turno, impedindo bypass por chamada direta.
- [ ] Mostrar “Disponível em N turnos” na lista; botão indisponível preserva leitura de nome/efeito e não aceita ação.
- [ ] Testar que sempre resta o ataque básico para participante vivo; ação bloqueada não consome turno.
- [ ] Testar cópias renomeadas: usar stun A bloqueia stun B pela família; veneno A bloqueia veneno B; ataque básico continua livre. Não descontar duas vezes quando técnica e família apontam para o mesmo intervalo.
- [ ] Conferir UI em resolução renderizada/zoom/Canvas registrados; validar recarga por input real somente em sessão autorizada.

**Aceite:** o jogador entende por que uma técnica está indisponível; contagem corresponde às ações realizadas; iniciar nova luta zera recarga.

**Reversão:** remover somente a limitação e apresentação adicionadas, mantendo pipeline COM-02 e técnicas compradas. Sem alterações permanentes no save.

## COM-05 — Dar padrões legíveis aos inimigos

**Prioridade:** P2 para os padrões da demo; padrões adicionais ficam em P3. **Dependências:** COM-01/COM-02 e mecanismo escolhido em COM-04 quando o padrão o utilizar.

**Existentes candidatos:** `Assets/Scripts/Combat/InimigoFixo.cs`, `Assets/Scripts/Combat/BattleManager.cs` e `Assets/Scripts/Catalogos/CatalogoDeInimigos.cs`; ampliar `Assets/Editor/ValidacaoRegrasCombate.cs`, proposto em COM-01.

**Novo proposto:** `Assets/Scripts/Combat/PadraoDeInimigo.cs`, contendo descrição simples de sequência e seleção; não criar árvore de comportamento ou novo sistema de scripts.

**Consumidores:** catálogo define o padrão, participante oferece ações e Battle exibe intenção. Manter `Atacar` como fallback para definições sem padrão.

**Contrato proposto:** `PadraoDeInimigo.EscolherAcao(int turnosExecutados, int hpAtual, int hpMax) : string`; seleção determinística baseada em sequência/limiar aprovado.

- Primeiro recorte recomendado: atacante básico; inimigo que alterna ataque e defesa temporária; chefe que anuncia ataque forte antes de executá-lo.
- Intensidades, duração, limiar de HP e identidades que receberão cada padrão exigem aprovação de design; não reaproveitar `EhChefe` para mudar dificuldade silenciosamente.
- Intenção é exibida antes da escolha do jogador; stun bloqueia execução e não avança a sequência; golpe anunciado que erra avança porque foi executado.
- Ação defensiva passa pelo mesmo turno/efeitos; não concede um turno extra escondido nem executa Lua.

- [ ] Aprovar uma tabela pequena de inimigo → sequência → mensagem de intenção; manter os demais no ataque básico.
- [ ] Escrever testes para ciclo completo, limiar exato, stun antes do golpe anunciado e padrão desconhecido.
- [ ] Implementar seleção sem aleatoriedade adicional e sem alterar fórmula de acerto/defesa.
- [ ] Integrar anúncio à UI existente; efeitos defensivos usam COM-02 e recarga, quando aplicável, usa COM-04.
- [ ] Testar sequência por fixture e, depois, por input real; verificar que o anúncio corresponde à ação executada ou explica sua interrupção.
- [ ] Medir turnos até vitória, itens usados, erros e compreensão do anúncio em playtest autorizado; propor ajustes em rodada posterior.

**Aceite:** ao menos dois padrões aprovados são distinguíveis por decisões observáveis; o jogador pode antecipar uma ação, e interrupções não contradizem o log.

**Reversão:** restaurar IDs/padrões alterados para `Atacar`; manter catálogo base integrado. Não redesenhar todos os inimigos para validar o primeiro padrão.

## COM-06 — Recuperar uma derrota de forma consistente

**Prioridade:** P1. **Dependências:** COM-02, EST-01 para capacidades persistidas, EST-02 para restauração integral e D-COM-03.

**Existentes candidatos:** `Assets/Scripts/Combat/BattleManager.cs`, `Assets/Scripts/Combat/EncounterZone.cs`, `Assets/Scripts/Historia/GatilhoDeBatalha.cs` e `Assets/Editor/ValidacaoCombate.cs`; `Assets/Scripts/Estado/SalvamentoJson.cs` e `Assets/Scripts/Estado/GerenciadorDeEstado.cs` sob propriedade de EST-02.

**Novo proposto:** `Assets/Scripts/Menus/TelaDeDerrotaManager.cs`, somente após aprovar UI local. Montagem candidata: painel da Battle; caminho exato de cena/objetos deve ser registrado antes de editar assets.

**Direção do usuário:** após derrota, retornar ao último checkpoint na Casa. O autosave anterior à luta continua protegendo retomada da aplicação, mas não substitui o snapshot de retorno da Casa.

**Detalhamento proposto:** registrar checkpoint por interação explícita “Descansar e registrar checkpoint” na CasaLip; restaurar HP máximo e consolidar o estado completo naquele ponto. Derrota restaura esse snapshot, sem penalidade extra de XP/gold: desfaz o progresso posterior ao registro, inclusive prêmios/consumos/flags, de forma coerente. Essa semântica de rollback e descanso é proposta do planejamento; o destino Casa é escolha expressa do usuário. A tela deve informar a consequência antes de confirmar.

**Contrato proposto:** `BattleManager.TentarRetomarAposDerrota(out string erro) : bool` chama o wrapper de retorno da Casa de EST-02, sem reaplicar XP/drops/flags de vitória nem gravar o estado derrotado.

- Consumir `GerenciadorDeEstado.TentarRetornarAoCheckpointCasa(out string erro) : bool`, proposto em EST-02. O wrapper prepara pelo mesmo `PrepararRestauracao`, grava atomicamente o estado corrente restaurado e só então aplica/transiciona. Battle não implementa um segundo restaurador. Falha de gravação deixa arquivo anterior e tela de derrota preservados.
- A restauração abrange HP, XP/pontos, técnicas, equipamentos, inventário, gold, flags, contadores e origem. Não duplicar uma restauração parcial dentro de Battle.
- “Voltar ao checkpoint da Casa” retorna à cena/posição confirmadas do snapshot da Casa, não à origem City/Game da última batalha. Não iniciar luta automaticamente.
- Gatilho sobreposto à posição restaurada só rearma após sair da área; zona aleatória reinicia distância acumulada e exige novo deslocamento. Estado de rearme é transitório, não altera checkpoint.
- Se leitura/validação/gravação falhar, manter tela de derrota com mensagem acionável; não carregar mundo com estado parcialmente restaurado.
- Battle aberta diretamente ou save legado sem checkpoint da Casa deve explicar a ausência; não fabricar posição/snapshot. Em campanha nova, registrar o primeiro ponto seguro antes de expor combates; em campanha legada, orientar registro na Casa. Como saída excepcional, oferecer carga normal do save disponível, claramente distinta de retorno à Casa.
- Desabilitar comandos de combate ao derrotar; limpar efeitos e recargas; repetição do clique não agenda múltiplas transições.

- [ ] Registrar fixture da Casa: HP máximo, dois consumíveis, peça equipada, pontos/quest; depois criar autosave em City, ganhar item/progredir quest e perder HP na luta.
- [ ] Escrever teste: derrota restaura todos os campos do snapshot da Casa, desfaz progresso posterior e mantém seu conteúdo inalterado. Save corrente passa a apontar para a Casa; encerrar/reabrir mantém esse estado, sem prêmio de vitória.
- [ ] Escrever testes de checkpoint ausente/corrompido e clique repetido, exigindo ausência de aplicação parcial.
- [ ] Testar falha na gravação do retorno: não aplicar snapshot em memória nem substituir o arquivo anterior; nova tentativa repete somente a transação de retorno, sem criar recompensa.
- [ ] Implementar integração com EST-02; manter o checkpoint anterior quando restauração falha.
- [ ] Montar painel somente após autorização de cena/objetos; registrar resolução, Game View, Canvas/Scaler e comparação antes/depois.
- [ ] Validar caminho por input real, incluindo posição dentro/próxima de zona de encontro, e confirmar ausência de nova batalha imediata.

**Aceite:** derrota oferece retorno à Casa no último checkpoint registrado, com HP/recursos/progresso coerentes com aquele momento; autosaves fora da Casa não deslocam o ponto seguro. Fechar/reabrir após retorno conserva a mesma situação.

**Reversão:** preservar checkpoint e backup autorizado; reverter UI/fluxo desta entrega sem substituir save do usuário. Reversão de código não comprova recuperação de progresso.

## PRO-01 — Tornar viável a preparação inicial

**Prioridade:** P1. **Dependências:** D-PRO-01 e MIS-01/MIS-02; COM-03 não pode alterar o custo de técnica simples 2 sem revisar este aceite. A recompensa narrativa e seus gastos não dependem da loja.

**Existentes candidatos:** `Assets/Scripts/Scripting/SistemaDeProgressao.cs`, `Assets/Scripts/Scripting/ProgressaoDeNivel.cs`, `Assets/Scripts/Scripting/PontosDeProgressao.cs`, `Assets/Scripts/Scripting/TerminalDroidApi.cs`, `Assets/Scripts/Catalogos/CatalogoDeQuests.cs`, `Assets/Scripts/Catalogos/CatalogoDeDialogos.cs`, `Assets/Scripts/Catalogos/CatalogoDeLicoes.cs` e `Assets/Scripts/Historia/TrancaPorHistoria.cs`; integração de `Assets/Scripts/Historia/GerenciadorDeQuests.cs` pertence a MIS-01/MIS-02.

**Novo proposto:** `Assets/Editor/ValidacaoProgressao.cs`. Estado inicial e recompensas não serão alterados para mascarar missão sem integração.

**Consumidores:** objetivo principal/HUD, oferta e entrega de missão, tutorial do terminal, portão/tranca de história, tela de status e save.

**Contrato existente preservado:** `SistemaDeProgressao.GanharExperiencia(Droid droid, int xpGanho)` centraliza níveis e pontos; missão entrega XP por esse caminho uma vez.

- Rota recomendada: oferta/ativação de `q_main_agua` → conversas previstas → conclusão/entrega segundo MIS-02 → recompensa 120 XP → orientação de técnica e DEX.
- Partindo de nível 1/XP 0/pontos 0, 120 XP devem produzir nível 2/XP restante 20/pontos 5; aprender técnica nível 2 custa 2 e DEX +3 custa 3.
- Exibir origem dos cinco pontos e feedback de gasto; atualizar diálogo/objetivo que hoje pede ação ainda impossível. Não exigir combate prévio para obter o orçamento do próprio tutorial.
- Preservar proteção contra gastar tudo antes de configurar técnica; oferecer orientação recuperável quando o jogador gastar fora da sequência, sem reset silencioso.
- Não elevar precisão, reduzir HP inimigo ou encurtar luta nesta entrega. Duração/precisão dependem de playtest autorizado após corrigir integração.

- [ ] Criar teste isolado da recompensa 120 XP e dos gastos 2+3; saldo final zero, técnica presente, DEX aumentado em 3.
- [ ] Criar testes: recompensa reaplicada não duplica XP/pontos; XP negativo e gasto negativo não aumentam saldo; ganho que cruza vários níveis concede cinco pontos por nível.
- [ ] Integrar a rota com MIS-01/MIS-02, respeitando conclusão automática versus entrega; evitar dois responsáveis concedendo a recompensa.
- [ ] Ajustar sequência textual/objetivo do tutorial e dependências da tranca dentro do escopo aprovado, preservando IDs.
- [ ] Testar partida nova, retomada antes/depois da recompensa e jogador que conversa com NPCs em ordem diferente.
- [ ] Testar saldo insuficiente e recuperação por orientação/esquecimento permitido; não usar `testeAdicionarPontos` no percurso de aceite.
- [ ] Em playtest autorizado, percorrer exploração → recompensa → terminal → saída; registrar tempo, clareza e tentativas, sem inferir balanceamento de uma execução.

**Aceite:** partida nova alcança os cinco pontos pela rota narrativa existente, configura técnica e DEX e prossegue sem comando de depuração, prêmio duplicado ou bloqueio sem explicação.

**Reversão:** reverter somente integração/textos deste item; preservar saves de teste separados e recompensas reais já concedidas. Mudanças de saldo pessoal exigem pedido específico.

## PRO-02 — Redistribuir pontos e comparar configurações na Casa

**Prioridade:** P2. **Dependências:** COM-03, TER-02, EST-01/EST-02. **Proposta nova:** redistribuição gratuita e integral dos investimentos comprovados, acessível na Casa fora de batalha. Rebalancear custos e permitir redistribuição são partes complementares desta proposta; o usuário solicitou o rebalanceamento, e esta é a solução sugerida para incentivar experimentação.

**Arquivos candidatos:** `PontosDeProgressao.cs`, `TerminalDroidApi.cs`, `Droid.cs`, `TabelaDeCustos.cs`, `DadosDoJogo.cs`, `SalvamentoJson.cs`, `TerminalUIManager.cs` e `TelaDeStatusManager.cs`. Novos propostos: `Assets/Scripts/Scripting/ServicoDeRedistribuicao.cs` e `ResumoDeRedistribuicao.cs`; ampliar `Assets/Editor/ValidacaoProgressao.cs`.

**Consumidores/contrato:** `ServicoDeRedistribuicao.Preparar(Droid droid) : ResumoDeRedistribuicao` produz base, compras, reembolso comprovado e saldo final sem mutar; `GerenciadorDeEstado.TentarConfirmarRedistribuicaoCasa(ResumoDeRedistribuicao resumo, out string erro) : bool` revalida, prepara cópia, registra checkpoint da Casa pela transação de EST-02 e só então aplica. Se as compras/saldo mudaram desde a prévia, recusar e mostrar resumo atualizado, sem usar números antigos.

- [ ] Persistir investimento por técnica e por atributo comprado, separado de atributos iniciais, peças e buffs. Coordenar migração na próxima versão disponível; não deduzir reembolso do atributo total da ficha.
- [ ] Exibir “saldo atual + devolução comprovada = saldo após redistribuir”, técnicas removidas e atributos de base preservados. Ataque básico, XP, nível, peças, inventário e flags não são redefinidos por redistribuir.
- [ ] Confirmar explicitamente; cancelar não muda nada. Redistribuição remove apenas técnicas customizadas e incrementos comprados identificados, devolvendo seu investimento uma única vez.
- [ ] Não conceder novamente cinco pontos por nível, não trocar a curva de XP e não zerar progresso de missão. Legado sem investimento comprovado informa a limitação; não inventar pontos.
- [ ] Preservar base da campanha e saldos legados reconhecidos. Investimento de peça, buff, recompensa de atributo sem custo ou alteração não identificada não é devolução de pontos comprados.
- [ ] Usar cópia e a mesma disciplina transacional de TER-02. Checkpoint atualizado junto da confirmação impede que morrer restaure compras antigas somadas ao reembolso novo; IO falho não confirma a redistribuição.
- [ ] Oferecer orientação para reconstruir técnica mínima e DEX se os requisitos do tutorial ainda forem relevantes, sem cobrar duas vezes ou prender o jogador na Casa.
- [ ] Na prévia comum de aprendizagem/melhoria, mostrar custo, diferença, saldo e recarga de B1/R1; aplicar a mesma regra de confirmação única, com mensagem de saldo insuficiente.

**Casos mínimos:** wallet 0 + técnica investida 2 + DEX comprada 3 → wallet 5 após redistribuir; segunda confirmação não devolve novamente; equipar peça com DEX +3 não acrescenta 3 ao reembolso; manter XP 20/nível 2/flags; cancelar preserva tudo; erro de gravação preserva configuração anterior; salvar/reabrir/derrotar mantém a configuração e o orçamento corretos.

**Aceite:** o jogador pode experimentar outra distribuição na Casa sem perder pontos comprovadamente comprados ou produzir pontos adicionais. Prévia informa o custo antes do gasto e a configuração confirmada sobrevive à retomada.

**Reversão:** retirar a opção nova sem apagar o ledger nem o checkpoint migrado. Não recalcular saves pelo preço anterior ou apagar investimentos para voltar a uma versão do código.

## COM-07 — Treino seguro e relatório de configuração

**Prioridade:** P3, proposta posterior à demo. **Dependências:** COM-01 a COM-04. **Objetivo:** permitir experimentar a tabela de custos/recarga em um adversário de treino na Casa sem arriscar progresso ou gerar farm.

**Arquivos candidatos:** consumidor local de `CombatEngine.cs`, `InimigoFixo.cs` e `ControleDeRecarga.cs`; novos `Assets/Scripts/Combat/SimuladorDeTreino.cs` e `Assets/Scripts/Menus/PainelDeTreino.cs`. Objetos da Casa delimitados antes de montagem; nenhuma mudança no puzzle de água ou diálogo existente é implícita.

**Contrato proposto:** `SimuladorDeTreino.Executar(ConfiguracaoDeTreino configuracao, int maximoDeTurnos) : ResultadoDeTreino`, com cópias dos participantes e limite de 5 turnos por simulação inicial. `ConfiguracaoDeTreino` inclui atributos/técnicas copiados e sequência finita de ações; `ResultadoDeTreino` contém dano direto, DOT, turnos bloqueados/recargas e HP final das cópias. Usar aleatoriedade local determinística nos testes, sem alterar a fonte global do jogo.

Os dois DTOs pertencem ao arquivo do simulador inicialmente; não criam dependência de UI/quest/save. Limite é de cinco oportunidades de ação processadas no total, não cinco frames ou um laço sem condição terminal.

- [ ] Criar duas configurações pequenas de alvo, uma neutra e uma resistente a veneno; valores declarados como dados de treino, não ajuste dos inimigos da campanha.
- [ ] Reutilizar as regras de turno e recarga reais em cópias profundas. Não abrir a batalha de campanha, não consumir inventário real e não emitir eventos de vitória/quest.
- [ ] Permitir comparar duas configurações pelo mesmo alvo/semente e pelo mesmo limite de oportunidades. Mostrar orçamento investido e turnos de espera, sem declarar automaticamente uma opção superior.
- [ ] Não conceder XP, gold, itens, flags ou registro de vitória. Fechar o painel descarta o resultado; não atualizar save ou checkpoint.
- [ ] Limitar sessões e logs; sem laço Lua, execução automática ilimitada ou simulação de toda a campanha.

**Verificação/aceite:** antes/depois de cinco turnos e de fechar/reabrir, hash do estado real/save e contadores permanecem idênticos; custos/recargas do relatório correspondem a B1/R1 e aos mesmos casos do harness de combate; resistência do alvo muda o resultado esperado. UI apresenta a natureza de treino e a ausência de recompensas.

**Reversão:** remover a entrada/painel de treino e suas referências locais; preservar motor de combate e dados reais. Resultado de treino não pode ter virado estado persistente.

## Validação, entrega e retomada

- [ ] Para cada entrega, registrar separadamente: editado, salvo, compilado, teste de regra/API, input real e aprovação visual.
- [ ] Executar somente casos afetados; `ValidacaoCombate.Executar()` e futuros `ValidacaoRegrasCombate.Executar()`/`ValidacaoProgressao.Executar()` devem retornar casos `OK`, sem `FALHOU`, fora de Play.
- [ ] Harnesses novos devem declarar menu, fixtures, limite de iterações, restauração de singletons/aleatoriedade e limpeza em `finally`; não depender de esperas indefinidas.
- [ ] Para UI, comparar resolução/zoom/Canvas iguais; aceite exige leitura de motivo, ausência de cortes/sobreposição e preservação dos demais painéis.
- [ ] Encaminhar integração final e percurso da demo para [validação/entrega](06-validacao-e-entrega.md); padrões adicionais pertencem à priorização de [mecânicas/demo](05-mecanicas-e-demo.md).

**Estado de saída em 21/10/2026:** plano redigido e revisado documentalmente; implementação, testes de comportamento, input real e aprovação visual pendentes. Unity não consultado, Play não iniciado, saves não acessados e backups não necessários nesta tarefa documental.

**Próximo passo:** revisar os números propostos B1/R1 e o detalhamento da Casa, delimitar a primeira entrega autorizada e registrar seus valores anteriores. Recarga e destino de derrota já foram escolhidos; narrativa está adiada. COM-01 pode ser escolhida primeiro sem aguardar o treino opcional.
