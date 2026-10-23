# Plano de implementação — Estado, inventário e economia

> Execução futura: seguir [AGENTS.md](../../../AGENTS.md) e `superpowers:executing-plans` em recortes pequenos. Este documento não autoriza alterar saves pessoais, iniciar Play ou executar testes agora.

**Objetivo:** preservar aquisições e progresso entre sessões, tornar transações previsíveis e permitir experimentar equipamentos sem perdas acidentais.
**Arquitetura:** manter o estado central existente; separar serialização/restauração de transições de cena e regras de inventário da apresentação da Bag/Loja.
**Tecnologias:** C#, JsonUtility, armazenamento atual e harness Editor existente; sem atualização de pacotes.
**Especificação:** [plano principal](README.md) e [contrato de estado/save](../../contextos/sistemas/estado-e-save.md).
**Estado atualizado23/10:** EST-01 e recorte funcional de INV-01 implementados/testados por fixtures na [entrega do Droid](../../planos-implementacao/2026-10-23-droid-ilustrado.md). Salvar/Carregar e mensagens locais também corrigidos; EST-02 integral, ECO-01 e INV-02 continuam propostas. Os checkboxes abaixo preservam o planejamento original, não uma lista de trabalho toda por executar; usar a atualização e fonte vinculada antes de repetir implementação.

## Regras globais deste eixo

- O formato vigente é v7, com equipamentos. Cada nova capacidade usa a próxima versão disponível; não reservar versões concorrentes para programas e missões.
- A migração v6 da casa e a recusa de save legado em `Battle` devem ser preservadas. Informação ausente não pode ser reconstruída por suposição.
- Checkpoint de entrada de combate permanece na exploração; vitória/fuga consolidam na origem. Nunca chamar `Salvar()` com cena `Battle` para corrigir outra integração.
- Atualização de direção: derrota retorna ao último checkpoint **da Casa**, escolhido pelo usuário. O autosave corrente de exploração e o snapshot de retorno da Casa são dados distintos; a tabela B1/R1 e a proposta de descanso/rollback estão no plano 03. Nenhuma mudança de narrativa nesta rodada.
- Falha de leitura/validação não pode aplicar metade do save, fechar o menu ou trocar a cena.
- Contratos abaixo são propostas salvo os recortes atualizados. Já existem DroidPart.IdItem, PecaSalva, DroidSalvo.pecas, ServicoDeEquipamento e ValidacaoEquipamentosApollo; não criar classes paralelas só para seguir os nomes inicialmente sugeridos.
- Fazer cópia profunda do estado necessário, sem copiar a hierarquia Unity ou clonar objetos de cena.

## EST-01 — Identidade de peças e evolução do save

**Entregue em23/10 com validação isolada:** payload/slots/raridade/bônus, migração legada, recusa de inválidos/futuro e restauração antes do HP. Harness real é `Assets/Editor/ValidacaoEquipamentosApollo.cs`, não o ValidacaoPersistencia sugerido abaixo. Percurso por input/reinício do jogo continua pendente. [Guia vigente](../../guias/equipamentos-apollo.md).

**Prioridade:** P1 / SAVE-001. **Dependências:** nenhuma. **Decisão D-EST-01:** salvar ID estável e atributos da peça equipada; slots vazios permanecem vazios. Identidade não é o nome exibido.

**Arquivos:**

- Alterar `Assets/Scripts/Droid/DroidPart.cs`, `Braco.cs`, `Perna.cs`, `Tronco.cs`, `Cabeca.cs` e `Droid.cs`.
- Alterar `Assets/Scripts/Estado/DadosDoJogo.cs`, `SalvamentoJson.cs`.
- Alterar `Assets/Scripts/Menus/BagUIManager.cs` apenas construção/identidade necessária.
- Criar `Assets/Editor/ValidacaoPersistencia.cs`.

**Consumidores:** Bag, TelaDeDroid, Status, cálculo de atributos/HP, combate, terminal que consulta atributos e carga de checkpoint.

**Contrato proposto:** `DroidPart.IdItem` identifica o catálogo; `PecaSalva` contém `slot`, `idItem`, `nome`, `raridade`, `atributoPrincipal`, `temBonus`, `atributoBonificado`, `valorDoBonus`. `DroidSalvo.pecas` é uma lista de no máximo quatro slots únicos. Guardar atributos efetivos evita reprecificar/rebalancear silenciosamente equipamentos antigos durante load.

- [ ] Criar fixture v6 sem peças e fixture com quatro peças distintas; demonstrar a lacuna de round-trip atual usando arquivo temporário.
- [ ] Preservar os construtores existentes ou migrar todos seus consumidores no mesmo recorte, incluindo harnesses; não quebrar montagem via renomeação.
- [ ] Serializar slots, identidade, raridade e bônus; validar tipo compatível com o slot.
- [ ] Migrar save anterior para lista vazia; se o inventário antigo já perdeu a peça, registrar a limitação, sem conceder equipamento imaginado.
- [ ] Validar listas nulas, slot duplicado, enum inválido, quantidade de slots e atributos fora de limites antes de mutar sessão.
- [ ] Para ID ausente do catálogo: preservar payload na leitura e recusar aplicação com mensagem específica; não descartar aquisição. Recuperação por mapeamento de IDs exige tarefa explícita.
- [ ] Restaurar peças antes de calcular/clamp de HP; testar unidade em nova sessão, não apenas sobre o mesmo Droid que já está equipado.
- [ ] Salvar e reler fixture migrada; garantir flag da casa, técnicas, inventário, Gold, contadores e checkpoint intactos.

**Testes:** peça rara mantém raridade; tronco +6 VIT mantém bônus; slot vazio substitui peça que estava em memória; item equipado continua fora da Bag; migração não repete recompensa; save inválido não muda a sessão.

**Aceite:** equipar → salvar → encerrar/reabrir → carregar preserva slots e atributos, sem duplicação de quantidade.

**Reversão:** backup da versão anterior do arquivo e do save de teste. Downgrade de código não deve ler um save futuro e ignorar campos; bloquear versão superior à suportada. Restaurar backup pessoal somente nas condições de VAL-01.

## EST-02 — Restauração integral, ciclo de sessão e falhas de IO

**Prioridade:** P1. **Dependências:** EST-01 para incluir equipamento no snapshot; D-EST-02 abaixo.

**Arquivos:** alterar `GerenciadorDeEstado.cs`, `SalvamentoJson.cs`, `ISistemaDeSalvamento.cs` apenas se necessário à interface comum, `MainMenuManager.cs`, `MenuMundoManager.cs`; consumidores de autosave `BagUIManager`, `DialogoUIManager`, `GatilhoDeDialogo`, `VerificadorDePuzzle`, `GerenciadorDeQuests` e `BattleManager` entram somente para padronizar resultado de gravação no recorte que os usa.

**Novos propostos para a Casa:** `Assets/Scripts/Estado/CheckpointCasaSalvo.cs`, com DTO de conteúdo não recursivo, e `Assets/Scripts/Casa/PontoDeCheckpointCasa.cs`, responsável apenas pela interação de descanso/registro e por chamar o wrapper de estado. Montagem em um único ponto seguro da CasaLip, com ID, posição e dono do input registrados antes da edição; não assumir referência ou coordenada existente nem modificar o puzzle de água.

**Decisão D-EST-02:** “Novo jogo” cria um estado novo em memória; não exclui o save anterior implicitamente. O primeiro salvamento da nova campanha precisa seguir uma política visível de substituição. Manter um slot e confirmação de substituição é a recomendação mínima; múltiplos slots ficam fora da demo.

**Contratos propostos:**

- `EstadoRestaurado SalvamentoJson.PrepararRestauracao(DadosDoJogo dados)` valida/migra e constrói estado sem aplicar nem carregar cena.
- `void GerenciadorDeEstado.AplicarEstado(EstadoRestaurado estado)` substitui o estado da sessão em um ponto único.
- `void GerenciadorDeEstado.IniciarNovoJogo()` restaura defaults explicitamente; não depende de `Awake` rodar outra vez.
- `ResultadoSalvamento SalvamentoJson.TentarSalvarCheckpoint(GerenciadorDeEstado estado, string cena, Vector3 posicao)` contém `Sucesso` e `Mensagem`; preservar wrapper atual durante migração dos consumidores.
- `EstadoRestaurado` inclui Droid, flags, contadores, inventário, Gold e destino; cresce quando CAD-01/MIS-01 acrescentarem dados persistentes. Não armazena callbacks/VM/UI.
- Proposta de extensão: `DadosDoJogo.CheckpointCasa` contém `CheckpointCasaSalvo`, com ID estável, cena, posição e conteúdo integral do estado naquele ponto. Extrair um DTO de conteúdo sem o próprio campo `CheckpointCasa`, evitando snapshots recursivos ou crescimento a cada autosave. Preservar compatibilidade dos campos correntes na migração, coordenada com EST-01/COM-03/PRO-02.
- `bool GerenciadorDeEstado.TentarRegistrarCheckpointCasa(out string erro)` valida estar no ponto seguro autorizado da Casa, prepara cópia com HP máximo e temporários limpos, grava estado corrente + snapshot da Casa na mesma transação e só então aplica. Ação explícita de descanso; salvar por compra/vitória/conversa fora da Casa não atualiza esse snapshot.
- `bool GerenciadorDeEstado.TentarRetornarAoCheckpointCasa(out string erro)` extrai/valida snapshot, prepara estado completo pela restauração comum, grava atomicamente um save corrente com esse estado e o mesmo checkpoint da Casa, aplica e carrega a Casa. Falha anterior à confirmação não altera memória, arquivo nem tela de derrota; repetir não reaplica prêmio.
- `bool GerenciadorDeEstado.TentarConfirmarRedistribuicaoCasa(ResumoDeRedistribuicao resumo, out string erro)` é o wrapper de PRO-02: revalida a prévia, prepara a redistribuição numa cópia e usa a mesma transação de registro da Casa, sem dois saves independentes.
- Migração sem snapshot da Casa mantém o campo ausente; não inventar estado antigo, posição ou descanso. Orientar o primeiro registro seguro. Campanha nova recebe registro inicial válido antes do primeiro encontro; objeto/posição seguros serão inventariados na implementação, sem presumir coordenadas.

- [ ] Provar com fixtures que aplicar save sobre sessão existente limpa slots/efeitos/dados ausentes, em vez de herdar a sessão anterior.
- [ ] Validar o DTO inteiro antes de chamar `AplicarEstado`: cena, posição finita, versão, listas, enums, IDs e valores; permitir legado válido pelas migrações.
- [ ] Preparar o novo estado sem alterar singleton; ao falhar, manter memória, arquivo e painel atuais.
- [ ] Na aplicação bem-sucedida, cancelar execução Lua, fechar painéis pelas APIs de seus donos, limpar contexto de batalha e efeitos transitórios.
- [ ] Coordenar contador global e flags internas dos painéis; `ZerarMenusAbertos()` sozinho não prova que `_aberto`/origens foram limpos.
- [ ] Carregar a cena só após aplicar o estado completo; reaplicar posição por `RestaurarPosicao`, preservando seu contrato.
- [ ] Implementar NovoJogo sem carregar progresso anterior do singleton; não apagar o arquivo existente.
- [ ] Padronizar aviso de falha de gravação: progresso em memória identificado como não salvo; opção de repetir sem repetir a ação que concedeu prêmio.
- [ ] Preservar gravação em temporário/substituição; qualificar `File.Replace` e persistência no navegador em VAL-03 antes de alegar suporte WebGL.
- [ ] Acrescentar snapshot da Casa em migração sequencial; carregar/salvar normalmente conserva esse conteúdo e não cria uma cópia dentro de outra.
- [ ] Testar registro explícito na Casa e autosave posterior em City: ambos existem, e somente o segundo altera o estado corrente. Recusa de registro fora da Casa não cura HP nem muda dados.
- [ ] Testar retorno após derrota: salvar corrente restaurado antes da transição, sem sobrescrever por estado derrotado. Se gravar falhar, conservar o arquivo anterior e não anunciar retorno concluído.
- [ ] Testar progresso obtido e perdido após o último registro: inventário, XP/pontos, compras, recompensas, fatos e marcadores de ocorrência voltam juntos; jamais misturar flags novas com recursos antigos.

**Testes:** save malformado/versão futura não altera sessão; checkpoint correto remove debuff da sessão anterior conforme COM-02; carregar não deixa menu travando movimento; NovoJogo após load não herda Gold/flags/técnicas; erro de disco mantém arquivo anterior; repetir salvar não repete XP/itens.

**Aceite:** carregar e recomeçar têm comportamento previsível; o jogador sabe quando uma gravação falhou.

**Coordenar:** COM-06 usa o wrapper de retorno da Casa, que reaproveita `PrepararRestauracao`/`AplicarEstado`; não implementar restauração alternativa só para a luta. O checkpoint de entrada da batalha permanece como proteção de retomada da aplicação, sem tomar o lugar do snapshot da Casa na derrota. PRO-02 registra o checkpoint junto da redistribuição confirmada.

## INV-01 — Equipar, trocar e inspecionar com segurança

**Atualização23/10:** serviço e comandos `droid.equipar`, `droid.desequipar`, `droid.listarPecas` implementados. Decisão posterior do usuário: instalar pelo terminal, sem botão de equipamento na Bag/Droid. Inventário devolvido, mesmoID sem consumo, ícone por ID e comparação na ficha; testes no plano do Droid. Propostas de botão/consumo sem devolução abaixo não são direção vigente. Camadas de roupa/armadura no corpo ainda são trabalho futuro, separado de ícones.

**Prioridade:** P1. **Dependências:** EST-01; gravação consistente de EST-02.

**Arquivos:** alterar `BagUIManager.cs`, `TelaDeDroidManager.cs`, `TelaDeStatusManager.cs`, `Droid.cs`, `CatalogoDeItens.cs`; criar `Assets/Scripts/Droid/ServicoDeEquipamento.cs`; ampliar `ValidacaoPersistencia.cs`.

**Decisão D-INV-01:** trocar devolve a peça anterior ao inventário; desequipar é permitido; peça lendária conserva raridade. Catálogo atual não possui instâncias aleatórias, portanto inventário por ID continua suficiente; afixos e peças procedurais exigirão modelo próprio no futuro.

**Interface:** `ResultadoEquipamento ServicoDeEquipamento.TentarEquipar(GerenciadorDeEstado estado, string idItem)` e `TentarDesequipar(GerenciadorDeEstado estado, TipoDePeca slot)`. Resultado contém sucesso, mensagem e item anterior. Serviço não salva nem manipula UI.

- [ ] Validar estoque, tipo e slot antes de remover item; construir peça com identidade e raridade corretas.
- [ ] Trocar em uma operação: remover nova unidade, equipar e devolver anterior; se inválida, nenhum passo fica aplicado.
- [ ] Equipar o mesmo ID já equipado não consome outra unidade nem aumenta HP.
- [ ] Recalcular HP após troca; proposta: manter HP atual e limitar ao novo máximo ao reduzir VIT, sem curar ao alternar peças.
- [ ] Mostrar todos os itens, inclusive chaves e exclusivos de combate; uso desabilitado com motivo quando não permitido.
- [ ] Exibir descrição, raridade, slot, equipado atual e comparação de atributos antes da troca; não depender só do nome no botão.
- [ ] Persistir operação completa uma vez; se gravação falhar, conservar estado coerente em memória e informar pendência.
- [ ] Validar UI local sem mudar Canvas/Scaler global; comparar Bag e Droid e conferir Status após a operação.

**Testes:** trocar A por B devolve A; não ter B não remove A; usar mesmo item não duplica; retirar VIT não deixa HP maior que máximo; item chave visível mas não equipável; load preserva troca; falha de IO não duplica ao tentar salvar novamente.

**Aceite:** jogador consegue experimentar e voltar ao equipamento anterior sem perda ou cura explorável.

## ECO-01 — Compra e venda consistentes

**Prioridade:** P1 / LOJA-001. **Dependências:** EST-02; INV-01 para apresentação de todos os itens.

**Arquivos:** alterar `LojaUIManager.cs`, `CatalogoDeItens.cs`, `GerenciadorDeEstado.cs`; criar `Assets/Scripts/Estado/ServicoDeEconomia.cs` e `Assets/Editor/ValidacaoEconomia.cs`.

**Contrato:** `ResultadoTransacao ServicoDeEconomia.Comprar(GerenciadorDeEstado estado, string idItem, int quantidade)` e `Vender(...)`; validar operação inteira antes de alterar Gold/inventário. Serviço não faz IO. UI usa o mesmo cálculo de preço do serviço e coordena uma gravação.

- [ ] Reproduzir em fixture venda de item protegido e preço incorreto sem usar inventário real.
- [ ] Usar `CatalogoDeItens.PodeSerVendido` e `PrecoDeVendaReal` tanto na apresentação quanto na execução; ocultar ou desabilitar venda protegida.
- [ ] Comprar apenas item ofertado, com quantidade positiva e saldo suficiente; validar overflow da multiplicação e soma de estoque.
- [ ] Corrigir guardas das operações compartilhadas: `TentarRemoverItem` e `TentarGastar` não aceitam quantidade negativa para aumentar saldo.
- [ ] Validar ID antes de qualquer alteração; nenhum item desconhecido é criado silenciosamente pela loja.
- [ ] Aplicar débito e crédito juntos e bloquear clique duplicado enquanto a mesma operação está sendo finalizada.
- [ ] Salvar após transação completa em exploração; falha informa “compra aplicada, ainda não salva” e repetir gravação não recompra.
- [ ] Renovar lista e saldo; validar que preço recebido coincide com o anunciado.

**Casos:** poção pequena compra por 8 e vende por 4 conforme catálogo atual; Gold 7 recusa compra de 8; fio de cobre protegido não sai do inventário; quantidade -1/0 recusa; estoque 1 não vende 2; preço zero protegido não destrói item; gravação posterior conserva transação.

**Aceite:** não há perda de item de missão pela loja; preços exibidos e aplicados coincidem; reiniciar após save mantém compra/venda.

**Fora deste pacote:** estoque por comerciante, inflação, reputação de preço e crafting. Esses itens não devem entrar como dependência de corrigir preço e restrição.

## INV-02 — Consumíveis fora de batalha e retorno

**Prioridade:** P2. **Dependências:** EST-02, INV-01; decisão D-INV-02.

**Evidência:** Sinalizador de Retorno chama `Carregar`, desfaz ações posteriores ao save e não remove sua unidade; a descrição menciona essa reversão, mas a apresentação da Bag não mostra descrição completa. Isso é uma regra existente a esclarecer, não um teleporte já implementado.

**Decisão D-INV-02 recomendada:** na demo, apresentar explicitamente o sinalizador como retorno ao checkpoint da Casa, com confirmação de desfazer o progresso posterior ao seu registro e caráter reutilizável; usar o mesmo wrapper de EST-02. Esta extensão do sinalizador é proposta, não escolha expressa do usuário. Não cobrar um consumível que o próprio load faria reaparecer. Transformar em teleporte consumível exige novo contrato de destino e fica para expansão.

**Arquivos:** alterar `BagUIManager.cs`, `CatalogoDeItens.cs`; criar `Assets/Editor/ValidacaoInventario.cs` se casos não couberem no harness de persistência.

- [ ] Separar validação de ação e consumo; item de ação nula/desconhecida não é removido.
- [ ] Exibir efeito real antes do uso, inclusive reset ao checkpoint; tratar ausência/invalidade de save.
- [ ] Não chamar load apenas para descobrir depois que o arquivo era inválido; usar pré-validação de EST-02.
- [ ] Centralizar cura fora/batalha em regra comum só se isso for necessário ao recorte, preservando tratamento de turno de COM-02.
- [ ] Decidir explicitamente se cura com HP cheio é recusada em batalha; fora de batalha já é recusada. Não consumir sem feedback.
- [ ] Garantir uso único de item de XP, kit e chave; nova tentativa de salvar não repete efeito.

**Testes:** cancelar retorno mantém sessão; arquivo inexistente não consome; item sem ação não consome; módulo dá 100 XP uma vez; kit respeita HP máximo; chave não entrega duplicação por recarregar.

## Casos de revisão obrigatória

| Risco | Dono | Verificação mínima |
|---|---|---|
| Save mais novo aberto em versão antiga | EST-01 | Recusa antes de mutar memória/arquivo |
| Load sobre sessão já equipada e com efeito | EST-02 | Estado final depende só do save e da política de transitórios |
| ID removido de catálogo | EST-01 | Mensagem com ID; arquivo preservado; sem descarte silencioso |
| Clique duplicado ou falha após aplicar compra | ECO-01 | Operação única, repetição de IO não repete compra |
| Ganho de HP alternando VIT | INV-01 | Trocas não curam; HP sempre dentro do máximo |

## Retomada

- **Editado e salvo nesta tarefa:** documentação.
- **Comportamento/API/input/visual:** não testados nesta tarefa.
- **Próximo passo:** EST-01 em fixture temporária, quando a implementação e os testes do recorte forem autorizados.
- **Unity/save/backups:** não acessados. Não existe backup novo porque não houve edição de jogo; preservação e restauração futura seguem [VAL-01](06-validacao-e-entrega.md).
