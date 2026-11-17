# DroidsCode — plano de testes de usabilidade e gestão

Data: 14/11/2026. Documento de planejamento baseado no código e no roteiro da versão de banca disponíveis no repositório.

## Entrega e preservação

Esta ampliação acrescenta um catálogo complementar de **60 tarefas** e **14 entregas de gestão**. A matriz anterior deve ser mantida. O original não pôde ser lido nesta sessão: a consolidação e a eliminação de duplicidades dependem da disponibilização local desse arquivo. Não se afirma que as 60 tarefas sejam inéditas em relação ao original.

- [Planilha editável](Matriz_Ampliada_DroidsCode.xlsx): abas Matriz ampliada e Gestão, com filtros e listas de resultados.
- [Catálogo em CSV](Catalogo_Usabilidade_DroidsCode.csv).
- [Gestão em CSV](Gestao_DroidsCode.csv). CSVs usam UTF-8 e separador ponto e vírgula; não são promessa de importação nativa no GitHub Projects.

## Objetivo e método

Avaliar se o participante encontra ações, compreende orientações e interpreta o retorno do jogo. A presença de uma classe C# não comprova acesso pela interface. O catálogo foi limitado às ações descritas no [roteiro atual](../guias/roteiro-100-por-cento.md) e aos controladores indicados em cada tarefa. Diários de missões sem botão ligado e Opções ainda não implementadas ficam fora das tarefas de funcionalidade pronta.

Esta rodada não realizou testes com participantes. Todas as novas tarefas começam como **Não executado**; resultados automatizados anteriores não foram reaproveitados como aprovação de usabilidade. Conteúdo disponível pode depender da etapa da história.

## Cobertura

| Frente | Tarefas |
|---|---:|
| Menu inicial | 5 |
| Exploração | 8 |
| Diálogos | 6 |
| Missões | 8 |
| Terminal Lua | 7 |
| Inventário e equipamentos | 6 |
| Loja | 6 |
| Combate | 7 |
| Salvamento e retomada | 4 |
| Encerramento | 3 |
| **Total** | **60** |

## Aplicação com participantes

1. Definir público, nome do moderador e participantes; registrar perfil e familiaridade com jogos/programação.
2. Separar roteiros por frente. Cada participante recebe apenas um subconjunto viável, sem obrigação de completar as 60 tarefas.
3. Fixar commit/build, sistema operacional, resolução renderizada e modalidade de controle. Se usar Editor, registrar escala da Game View e Canvas/Scaler efetivos. Não misturar Editor com executável no mesmo resultado.
4. Preparar pré-condições por gameplay ou checkpoints de teste claramente identificados. Proteger save pessoal com backup e hash antes de sessões que gravam; restaurar e conferir o hash sem sobrescrever progresso novo do usuário.
5. Ler a tarefa ao participante sem revelar o caminho. Iniciar cronômetro ao terminar a instrução; parar no sucesso, abandono ou interrupção registrada. Tempo não tem limite inventado neste documento.
6. Registrar tempo, ajuda, erros observados, satisfação de 1 a 5, identificação/data e evidência na linha. Sucesso sem ajuda, Sucesso com ajuda, Falha e Bloqueado são resultados diferentes. Pré-condição ausente é bloqueio, não falha do participante.
7. Para vários participantes, duplicar a aba por participante/rodada e manter os mesmos IDs; não contar execuções repetidas como requisitos novos.
8. Consolidar somente sessões realizadas. Taxa de sucesso = sucessos com e sem ajuda / sessões com sucesso ou falha; não executados e bloqueados não entram no denominador. Informar também sucesso sem ajuda separadamente e tempos por tarefa.

## Critérios de prioridade

Alta: impede avanço, entendimento central, combate, integridade de progresso ou economia. Média: dificulta descoberta, consulta ou ações secundárias. Baixa: fluxo auxiliar sem bloqueio do percurso principal. São prioridades propostas de teste, não severidades de defeitos já comprovados. Responsáveis são papéis sugeridos; nomes e prazos ainda não foram fornecidos.

## Catálogo de tarefas

### US-001 — Menu inicial

**Tarefa ao participante:** Comece uma nova partida pelo menu inicial.

**Pré-condição:** Menu inicial aberto; save pessoal previamente protegido.

**Conclusão observável:** Participante encontra Novo Jogo e chega à casa inicial sem orientação.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [MainMenuManager.cs](../../Assets/Scripts/Menus/MainMenuManager.cs).

### US-002 — Menu inicial

**Tarefa ao participante:** Encontre as configurações de som.

**Pré-condição:** Menu inicial aberto.

**Conclusão observável:** Participante abre o painel de configurações e identifica o controle de volume.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [MainMenuManager.cs](../../Assets/Scripts/Menus/MainMenuManager.cs).

### US-003 — Menu inicial

**Tarefa ao participante:** Reduza e depois aumente o volume do jogo.

**Pré-condição:** Configurações abertas e áudio disponível.

**Conclusão observável:** Participante manipula o slider e percebe mudança audível.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [MainMenuManager.cs](../../Assets/Scripts/Menus/MainMenuManager.cs).

### US-004 — Menu inicial

**Tarefa ao participante:** Volte das configurações para o menu inicial.

**Pré-condição:** Configurações abertas.

**Conclusão observável:** Participante retorna ao painel inicial sem iniciar uma partida.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [MainMenuManager.cs](../../Assets/Scripts/Menus/MainMenuManager.cs).

### US-005 — Menu inicial

**Tarefa ao participante:** Encerre o jogo pelo menu inicial.

**Pré-condição:** Executável aberto; executar ao fim do roteiro desse participante.

**Conclusão observável:** Participante encontra Sair e o executável encerra; não usar resultado do Editor como substituto.

**Prioridade:** Baixa. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [MainMenuManager.cs](../../Assets/Scripts/Menus/MainMenuManager.cs).

### US-006 — Exploração

**Tarefa ao participante:** Caminhe da posição inicial até Apollo.

**Pré-condição:** CasaLip; introdução sem bloqueio de movimento.

**Conclusão observável:** Participante descobre os controles e se aproxima do personagem.

**Prioridade:** Alta. **Dependência:** US-001 **Resultado:** Não executado.

**Fonte:** [PlayerMovement.cs](../../Assets/Scripts/Scripting/PlayerMovement.cs).

### US-007 — Exploração

**Tarefa ao participante:** Saia de CasaLip e chegue à cidade.

**Pré-condição:** Saída liberada conforme a introdução.

**Conclusão observável:** Participante identifica a porta e aparece no destino sem ficar preso.

**Prioridade:** Alta. **Dependência:** US-006 **Resultado:** Não executado.

**Fonte:** [TransicaoDeCena.cs](../../Assets/Scripts/Scripting/TransicaoDeCena.cs).

### US-008 — Exploração

**Tarefa ao participante:** Encontre Dara na praça.

**Pré-condição:** City acessível.

**Conclusão observável:** Participante localiza Dara e consegue se aproximar para conversar. 

**Prioridade:** Alta. **Dependência:** US-007 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-009 — Exploração

**Tarefa ao participante:** Entre na casa/oficina de Quirino.

**Pré-condição:** City acessível; consultar mapa somente se houver dificuldade registrada.

**Conclusão observável:** Participante identifica a entrada correta e alcança o interior E02 Principal.

**Prioridade:** Média. **Dependência:** US-007 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-010 — Exploração

**Tarefa ao participante:** Saia da oficina e retorne à cidade.

**Pré-condição:** Interior E02 Principal aberto.

**Conclusão observável:** Participante encontra a saída e reconhece o retorno à City.

**Prioridade:** Média. **Dependência:** US-009 **Resultado:** Não executado.

**Fonte:** [TransicaoDeCena.cs](../../Assets/Scripts/Scripting/TransicaoDeCena.cs).

### US-011 — Exploração

**Tarefa ao participante:** Suba e depois desça a escada do prédio E03.

**Pré-condição:** Participante posicionado no térreo E03 por percurso normal.

**Conclusão observável:** Participante reconhece os acessos e faz ida e volta entre pavimentos sem aprisionamento.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-012 — Exploração

**Tarefa ao participante:** Contorne um móvel e alcance a porta da sala.

**Pré-condição:** Sala E03 com mobiliário e colisores ativos.

**Conclusão observável:** Participante percebe o obstáculo e encontra passagem; registrar colisões inesperadas e tentativas.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [PlayerMovement.cs](../../Assets/Scripts/Scripting/PlayerMovement.cs).

### US-013 — Exploração

**Tarefa ao participante:** Encontre e atravesse a saída da cidade para o vale.

**Pré-condição:** Pré-requisitos do portão cumpridos; City aberta.

**Conclusão observável:** Participante localiza a saída e reconhece a chegada ao vale.

**Prioridade:** Alta. **Dependência:** US-007 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-014 — Diálogos

**Tarefa ao participante:** Inicie uma conversa com Dara.

**Pré-condição:** Perto de Dara; nenhum painel ou diálogo aberto.

**Conclusão observável:** Participante descobre a interação e abre a conversa com o NPC pretendido.

**Prioridade:** Alta. **Dependência:** US-008 **Resultado:** Não executado.

**Fonte:** [DialogoUIManager.cs](../../Assets/Scripts/Historia/DialogoUIManager.cs).

### US-015 — Diálogos

**Tarefa ao participante:** Avance uma conversa até a próxima fala.

**Pré-condição:** Conversa aberta com mais de uma fala.

**Conclusão observável:** Participante encontra uma tecla válida e percebe a progressão do texto.

**Prioridade:** Alta. **Dependência:** US-014 **Resultado:** Não executado.

**Fonte:** [DialogoUIManager.cs](../../Assets/Scripts/Historia/DialogoUIManager.cs).

### US-016 — Diálogos

**Tarefa ao participante:** Escolha uma resposta apresentada na conversa.

**Pré-condição:** Diálogo com opções disponível.

**Conclusão observável:** Participante distingue opções do texto narrativo e seleciona a desejada.

**Prioridade:** Alta. **Dependência:** US-014 **Resultado:** Não executado.

**Fonte:** [DialogoUIManager.cs](../../Assets/Scripts/Historia/DialogoUIManager.cs).

### US-017 — Diálogos

**Tarefa ao participante:** Conclua a conversa e volte a caminhar.

**Pré-condição:** Conversa em andamento.

**Conclusão observável:** Participante encerra o diálogo e retoma o movimento sem ajuda.

**Prioridade:** Alta. **Dependência:** US-014 **Resultado:** Não executado.

**Fonte:** [DialogoUIManager.cs](../../Assets/Scripts/Historia/DialogoUIManager.cs).

### US-018 — Diálogos

**Tarefa ao participante:** Converse novamente com o mesmo personagem.

**Pré-condição:** Conversa anterior encerrada; participante próximo ao NPC.

**Conclusão observável:** Participante inicia nova interação e percebe a fala ou opção disponível no estado atual.

**Prioridade:** Média. **Dependência:** US-017 **Resultado:** Não executado.

**Fonte:** [DialogoUIManager.cs](../../Assets/Scripts/Historia/DialogoUIManager.cs).

### US-019 — Diálogos

**Tarefa ao participante:** Leia todas as opções e selecione uma em uma conversa longa.

**Pré-condição:** Conversa com escolhas longas disponível no percurso.

**Conclusão observável:** Texto e opções são compreendidos e selecionáveis sem cortes ou sobreposição; registrar resolução e escala.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-020 — Missões

**Tarefa ao participante:** Aceite a missão de água oferecida por Dara.

**Pré-condição:** Oferta disponível; missão ainda não aceita.

**Conclusão observável:** Participante identifica a oferta e confirma o aceite.

**Prioridade:** Alta. **Dependência:** US-014 **Resultado:** Não executado.

**Fonte:** [GerenciadorDeQuests.cs](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs).

### US-021 — Missões

**Tarefa ao participante:** Explique o que precisa fazer para ajudar Dara.

**Pré-condição:** Missão aceita; orientação da conversa disponível.

**Conclusão observável:** Participante descreve objetivo e destino coerentes sem o moderador revelar a resposta.

**Prioridade:** Alta. **Dependência:** US-020 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-022 — Missões

**Tarefa ao participante:** Localize a bancada do filtro na oficina de Quirino.

**Pré-condição:** Etapa do filtro disponível; missão correspondente aceita.

**Conclusão observável:** Participante chega à bancada correta a partir da orientação recebida.

**Prioridade:** Alta. **Dependência:** US-021 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-023 — Missões

**Tarefa ao participante:** Depois de cumprir o objetivo de Dara, volte à responsável.

**Pré-condição:** Objetivo da missão cumprido por gameplay normal.

**Conclusão observável:** Participante identifica que deve retornar a Dara para entregar a missão.

**Prioridade:** Alta. **Dependência:** US-020 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-024 — Missões

**Tarefa ao participante:** Entregue a missão de Dara e confirme a conclusão.

**Pré-condição:** Objetivo completo; conversa com Dara disponível.

**Conclusão observável:** Participante encontra Entregar e confirma Concluir entrega; percebe o encerramento.

**Prioridade:** Alta. **Dependência:** US-023 **Resultado:** Não executado.

**Fonte:** [GerenciadorDeQuests.cs](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs).

### US-025 — Missões

**Tarefa ao participante:** Encontre uma missão opcional oferecida por Tácio.

**Pré-condição:** Oferta de Tácio disponível no estado da partida.

**Conclusão observável:** Participante encontra e aceita uma missão opcional disponível sem confundi-la com a loja.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-026 — Missões

**Tarefa ao participante:** Aceite a missão de iniciação da guilda.

**Pré-condição:** Guilda E09 acessível; oferta de Mestra Bruna disponível.

**Conclusão observável:** Participante encontra Bruna e compreende o aceite da iniciação.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-027 — Missões

**Tarefa ao participante:** Identifique a recompensa recebida por uma missão entregue.

**Pré-condição:** Missão com recompensa entregue; anotar estado anterior.

**Conclusão observável:** Participante reconhece a recompensa pelo retorno do jogo e pela consulta ao estado correspondente.

**Prioridade:** Média. **Dependência:** US-024 **Resultado:** Não executado.

**Fonte:** [GerenciadorDeQuests.cs](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs).

### US-028 — Terminal Lua

**Tarefa ao participante:** Abra o terminal de programação pelo menu do mundo.

**Pré-condição:** Exploração livre; nenhum diálogo ou combate.

**Conclusão observável:** Participante encontra Terminal no menu e identifica entrada, saída e Executar.

**Prioridade:** Alta. **Dependência:** US-001 **Resultado:** Não executado.

**Fonte:** [TerminalUIManager.cs](../../Assets/Scripts/Menus/TerminalUIManager.cs).

### US-029 — Terminal Lua

**Tarefa ao participante:** Abra o terminal do gerador de CasaLip.

**Pré-condição:** Gerador acessível no estágio inicial da história.

**Conclusão observável:** Participante interage com o objeto certo e identifica o contexto do puzzle.

**Prioridade:** Alta. **Dependência:** US-006 **Resultado:** Não executado.

**Fonte:** [TerminalUIManager.cs](../../Assets/Scripts/Menus/TerminalUIManager.cs).

### US-030 — Terminal Lua

**Tarefa ao participante:** Execute uma solução válida para o gerador de água.

**Pré-condição:** Terminal do gerador aberto; fornecer solução do guia apenas se o roteiro prever.

**Conclusão observável:** Participante executa o código e reconhece a mensagem ou mudança que indica a solução.

**Prioridade:** Alta. **Dependência:** US-029 **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

### US-031 — Terminal Lua

**Tarefa ao participante:** Execute o código incompleto local x = e explique o retorno.

**Pré-condição:** Terminal aberto em sessão de teste protegida; código anterior anotado.

**Conclusão observável:** Participante percebe o erro de sintaxe e entende que precisa corrigir a entrada; não executar laços infinitos.

**Prioridade:** Alta. **Dependência:** US-028 **Resultado:** Não executado.

**Fonte:** [DroidScriptRunner.cs](../../Assets/Scripts/Scripting/DroidScriptRunner.cs).

### US-032 — Terminal Lua

**Tarefa ao participante:** Corrija o código anterior para print("teste") e execute.

**Pré-condição:** Erro de sintaxe da tarefa anterior exibido.

**Conclusão observável:** Participante edita a entrada e reconhece a saída teste após executar.

**Prioridade:** Alta. **Dependência:** US-031 **Resultado:** Não executado.

**Fonte:** [DroidScriptRunner.cs](../../Assets/Scripts/Scripting/DroidScriptRunner.cs).

### US-033 — Terminal Lua

**Tarefa ao participante:** Limpe o código do terminal.

**Pré-condição:** Entrada contém texto de teste descartável.

**Conclusão observável:** Participante encontra Limpar e percebe a remoção do conteúdo da entrada.

**Prioridade:** Baixa. **Dependência:** US-032 **Resultado:** Não executado.

**Fonte:** [TerminalUIManager.cs](../../Assets/Scripts/Menus/TerminalUIManager.cs).

### US-034 — Terminal Lua

**Tarefa ao participante:** Consulte a ajuda e aprenda uma técnica disponível.

**Pré-condição:** Pontos suficientes e técnica liberada; save de teste protegido.

**Conclusão observável:** Participante usa a ajuda para montar o comando e reconhece técnica aprendida e custo aplicado.

**Prioridade:** Alta. **Dependência:** US-028 **Resultado:** Não executado.

**Fonte:** [TerminalDroidApi.cs](../../Assets/Scripts/Scripting/TerminalDroidApi.cs).

### US-035 — Inventário e equipamentos

**Tarefa ao participante:** Abra a Bag e encontre um item do inventário.

**Pré-condição:** Exploração livre; pelo menos um item obtido normalmente.

**Conclusão observável:** Participante abre a Bag e seleciona o item pretendido.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [BagUIManager.cs](../../Assets/Scripts/Menus/BagUIManager.cs).

### US-036 — Inventário e equipamentos

**Tarefa ao participante:** Consulte a descrição e a quantidade de um item.

**Pré-condição:** Bag aberta com item selecionável.

**Conclusão observável:** Participante identifica efeito e quantidade disponível antes de usar.

**Prioridade:** Média. **Dependência:** US-035 **Resultado:** Não executado.

**Fonte:** [BagUIManager.cs](../../Assets/Scripts/Menus/BagUIManager.cs).

### US-037 — Inventário e equipamentos

**Tarefa ao participante:** Encontre um item em outra página da Bag.

**Pré-condição:** Inventário de teste com itens suficientes para mais de uma página.

**Conclusão observável:** Participante identifica a paginação e encontra o item fora da página inicial.

**Prioridade:** Média. **Dependência:** US-035 **Resultado:** Não executado.

**Fonte:** [BagUIManager.cs](../../Assets/Scripts/Menus/BagUIManager.cs).

### US-038 — Inventário e equipamentos

**Tarefa ao participante:** Use uma cura durante a exploração.

**Pré-condição:** HP abaixo do máximo; cura válida disponível; sem combate.

**Conclusão observável:** Participante confirma o uso e percebe alteração de HP e redução da quantidade.

**Prioridade:** Alta. **Dependência:** US-036 **Resultado:** Não executado.

**Fonte:** [BagUIManager.cs](../../Assets/Scripts/Menus/BagUIManager.cs).

### US-039 — Inventário e equipamentos

**Tarefa ao participante:** Descubra como instalar um equipamento no Droid.

**Pré-condição:** Equipamento compatível no estoque; painel Droid acessível.

**Conclusão observável:** Participante entende a orientação para o terminal e instala pelo comando disponível na ajuda.

**Prioridade:** Alta. **Dependência:** US-028 **Resultado:** Não executado.

**Fonte:** [TelaDeDroidManager.cs](../../Assets/Scripts/Menus/TelaDeDroidManager.cs).

### US-040 — Inventário e equipamentos

**Tarefa ao participante:** Consulte HP, energia e atributos de Apollo.

**Pré-condição:** Status acessível pelo menu do mundo.

**Conclusão observável:** Participante abre Status e encontra os valores solicitados sem confundir consulta com distribuição de pontos.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [TelaDeStatusManager.cs](../../Assets/Scripts/Menus/TelaDeStatusManager.cs).

### US-041 — Loja

**Tarefa ao participante:** Abra a loja de Senhor Tácio.

**Pré-condição:** Tácio acessível; opção de loja disponível.

**Conclusão observável:** Participante distingue loja e conversa e entra no painel de compra.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [LojaUIManager.cs](../../Assets/Scripts/Menus/LojaUIManager.cs).

### US-042 — Loja

**Tarefa ao participante:** Compare o preço de um produto com seu saldo.

**Pré-condição:** Loja aberta; ao menos um produto ofertado.

**Conclusão observável:** Participante encontra preço e saldo e informa se consegue comprar.

**Prioridade:** Alta. **Dependência:** US-041 **Resultado:** Não executado.

**Fonte:** [LojaUIManager.cs](../../Assets/Scripts/Menus/LojaUIManager.cs).

### US-043 — Loja

**Tarefa ao participante:** Compre uma unidade de um produto.

**Pré-condição:** Saldo suficiente; anotar saldo e quantidade anteriores.

**Conclusão observável:** Participante seleciona e confirma; reconhece uma unidade adicionada e desconto correspondente.

**Prioridade:** Alta. **Dependência:** US-042 **Resultado:** Não executado.

**Fonte:** [LojaUIManager.cs](../../Assets/Scripts/Menus/LojaUIManager.cs).

### US-044 — Loja

**Tarefa ao participante:** Tente comprar um produto acima do seu saldo.

**Pré-condição:** Sessão de teste com saldo menor que o preço de um produto.

**Conclusão observável:** Participante entende a recusa e identifica saldo insuficiente como motivo; sem compra realizada.

**Prioridade:** Alta. **Dependência:** US-041 **Resultado:** Não executado.

**Fonte:** [LojaUIManager.cs](../../Assets/Scripts/Menus/LojaUIManager.cs).

### US-045 — Loja

**Tarefa ao participante:** Venda uma unidade de um item permitido.

**Pré-condição:** Item vendável no estoque; loja aberta.

**Conclusão observável:** Participante encontra Vender, confirma o item e percebe a atualização do estoque e do saldo.

**Prioridade:** Média. **Dependência:** US-041 **Resultado:** Não executado.

**Fonte:** [LojaUIManager.cs](../../Assets/Scripts/Menus/LojaUIManager.cs).

### US-046 — Loja

**Tarefa ao participante:** Tente vender um item protegido da história.

**Pré-condição:** Item protegido disponível na sessão; registrar se a interface o oferece ou o oculta.

**Conclusão observável:** Participante entende a proteção pela ausência da oferta ou pela mensagem de recusa, sem perder o item.

**Prioridade:** Alta. **Dependência:** US-041 **Resultado:** Não executado.

**Fonte:** [LojaUIManager.cs](../../Assets/Scripts/Menus/LojaUIManager.cs).

### US-047 — Combate

**Tarefa ao participante:** Inicie um encontro liberado no vale.

**Pré-condição:** Encontro acessível conforme os pré-requisitos da história.

**Conclusão observável:** Participante inicia o encontro e reconhece sua vez e as opções de ação.

**Prioridade:** Alta. **Dependência:** US-013 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-048 — Combate

**Tarefa ao participante:** Ataque o inimigo com uma ação disponível.

**Pré-condição:** Turno do jogador; golpe válido disponível.

**Conclusão observável:** Participante executa um ataque e identifica retorno de dano/estado e mudança de turno.

**Prioridade:** Alta. **Dependência:** US-047 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-049 — Combate

**Tarefa ao participante:** Escolha uma técnica entre várias ações disponíveis.

**Pré-condição:** Turno do jogador; mais de uma ação aprendida.

**Conclusão observável:** Participante abre a lista, identifica a técnica desejada e confirma o golpe.

**Prioridade:** Alta. **Dependência:** US-047 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-050 — Combate

**Tarefa ao participante:** Abra a lista de golpes e volte sem atacar.

**Pré-condição:** Mais de uma ação disponível; turno do jogador.

**Conclusão observável:** Participante cancela o submenu e retorna às opções de ação sem executar golpe.

**Prioridade:** Média. **Dependência:** US-049 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-051 — Combate

**Tarefa ao participante:** Use uma cura durante seu turno.

**Pré-condição:** HP reduzido; item permitido em combate no estoque.

**Conclusão observável:** Participante seleciona e confirma a cura, percebe o consumo e o efeito no turno.

**Prioridade:** Alta. **Dependência:** US-047 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-052 — Combate

**Tarefa ao participante:** Consulte Status e retorne ao combate.

**Pré-condição:** Batalha ativa; opção Status disponível.

**Conclusão observável:** Participante encontra os dados de Apollo e fecha o painel para continuar lutando.

**Prioridade:** Média. **Dependência:** US-047 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-053 — Combate

**Tarefa ao participante:** Tente fugir de um encontro que permite fuga.

**Pré-condição:** Encontro com fuga permitida; turno do jogador.

**Conclusão observável:** Participante entende a opção e distingue sucesso de falha pelo retorno; uma tentativa pode falhar.

**Prioridade:** Média. **Dependência:** US-047 **Resultado:** Não executado.

**Fonte:** [BattleManager.cs](../../Assets/Scripts/Combat/BattleManager.cs).

### US-054 — Salvamento e retomada

**Tarefa ao participante:** Salve o progresso pelo menu do mundo.

**Pré-condição:** Save pessoal copiado e hash registrado; exploração livre.

**Conclusão observável:** Participante encontra Salvar e entende a confirmação; guardar evidência da mensagem.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [MenuMundoManager.cs](../../Assets/Scripts/Menus/MenuMundoManager.cs).

### US-055 — Salvamento e retomada

**Tarefa ao participante:** Carregue o progresso salvo.

**Pré-condição:** Save de teste válido; registrar antes cena, posição e estado esperado.

**Conclusão observável:** Participante encontra Carregar e reconhece o retorno ao progresso salvo.

**Prioridade:** Alta. **Dependência:** US-054 **Resultado:** Não executado.

**Fonte:** [MenuMundoManager.cs](../../Assets/Scripts/Menus/MenuMundoManager.cs).

### US-056 — Salvamento e retomada

**Tarefa ao participante:** Confira os itens e o dinheiro após carregar.

**Pré-condição:** Valores do momento do save anotados.

**Conclusão observável:** Participante localiza Bag/Status/Loja e confirma os mesmos valores do save.

**Prioridade:** Alta. **Dependência:** US-055 **Resultado:** Não executado.

**Fonte:** [SalvamentoJson.cs](../../Assets/Scripts/Estado/SalvamentoJson.cs).

### US-057 — Salvamento e retomada

**Tarefa ao participante:** Retome uma missão que estava ativa ao salvar.

**Pré-condição:** Save de teste com missão aceita e ainda não entregue.

**Conclusão observável:** Participante reconhece a etapa preservada pelas conversas/objetivos e consegue prosseguir sem novo aceite.

**Prioridade:** Alta. **Dependência:** US-055 **Resultado:** Não executado.

**Fonte:** [GerenciadorDeQuests.cs](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs).

### US-058 — Encerramento

**Tarefa ao participante:** Depois de uma derrota, retome pelo botão Continuar.

**Pré-condição:** GameOver alcançado em sessão de teste; save/checkpoint válido disponível.

**Conclusão observável:** Participante encontra Continuar e reconhece a retomada com HP recuperável.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [GameOverManager.cs](../../Assets/Scripts/Menus/GameOverManager.cs).

### US-059 — Encerramento

**Tarefa ao participante:** Volte ao menu inicial pela tela de derrota.

**Pré-condição:** GameOver aberto; save de teste protegido.

**Conclusão observável:** Participante entende Sair nessa tela como retorno ao menu inicial e conclui o retorno.

**Prioridade:** Média. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [GameOverManager.cs](../../Assets/Scripts/Menus/GameOverManager.cs).

### US-060 — Encerramento

**Tarefa ao participante:** Conclua as falas após vencer o boss e explique o próximo passo.

**Pré-condição:** Boss vencido por percurso normal; conversas finais disponíveis.

**Conclusão observável:** Participante reconhece o encerramento do Ato 1 e distingue missões opcionais ainda pendentes.

**Prioridade:** Alta. **Dependência:** Sem dependência de tarefa; conferir pré-condição. **Resultado:** Não executado.

**Fonte:** [roteiro-100-por-cento.md](../../docs/guias/roteiro-100-por-cento.md).

## Entregas de gestão

| ID | Entrega | IDs relacionados | Prioridade | Papel sugerido | Dependências | Aceite | Situação |
|---|---|---|---|---|---|---|---|
| GT-001 | Conferir matriz anterior e preservar tarefas | Todas | Alta | Coordenação | — | Matriz anterior preservada e duplicidades revisadas sem excluir objetivos diferentes. | A fazer |
| GT-002 | Revisar cobertura de requisitos | US-001 a US-060 | Alta | Coordenação | GT-001 | Cada tarefa aponta para comportamento e fonte reais; funcionalidades indisponíveis não são oferecidas como prontas. | A fazer |
| GT-003 | Definir participantes e distribuir roteiros | US-001 a US-060 | Alta | Coordenação | GT-002 | Participantes registrados e roteiros curtos distribuídos por perfil, sem exigir 60 tarefas de cada pessoa. | A fazer |
| GT-004 | Fixar versão, ambiente e referência visual | US-001 a US-060 | Alta | Responsável por testes | GT-002 | Build/commit, resolução renderizada, escala da Game View se usada e configuração Canvas/Scaler registrados. | A fazer |
| GT-005 | Preparar sessões e proteger saves | US-001 a US-060 | Alta | Responsável por testes | GT-004 | Pré-condições satisfeitas; backup do save com hash e restauração planejada; nenhum progresso pessoal sobrescrito. | A fazer |
| GT-006 | Aplicar roteiro de entrada e exploração | US-001 a US-019 | Alta | Moderador | GT-003; GT-005 | Execuções registram tempo, ajuda, erros e evidência, incluindo dificuldade de interação/legibilidade. | A fazer |
| GT-007 | Aplicar roteiro de missões e Lua | US-020 a US-034 | Alta | Moderador | GT-003; GT-005 | Participantes executam tarefas atribuídas e explicam objetivos e mensagens; laços infinitos não são usados. | A fazer |
| GT-008 | Aplicar roteiro de inventário e loja | US-035 a US-046 | Média | Moderador | GT-003; GT-005 | Registrar compreensão de itens, compra, venda e recusas nas sessões previstas. | A fazer |
| GT-009 | Aplicar roteiro de combate e persistência | US-047 a US-060 | Alta | Moderador | GT-003; GT-005 | Registrar decisões de turno, retomada e encerramento; restaurar save pessoal e conferir hash ao fim. | A fazer |
| GT-010 | Consolidar indicadores da rodada | US-001 a US-060 | Alta | Analista de testes | GT-006; GT-007; GT-008; GT-009 | Consolidar sucesso sem/com ajuda, falhas, tempos e satisfação; excluir não executados do denominador. | A fazer |
| GT-011 | Priorizar problemas observados | IDs com dificuldade ou falha | Alta | Coordenação | GT-010 | Cada problema tem reprodução, impacto, evidência, responsável a designar e prioridade justificada. | A fazer |
| GT-012 | Corrigir problemas aprovados e retestar | IDs afetados por correção | Alta | Desenvolvimento e testes | GT-011 | Somente correções com escopo aprovado; reteste dos consumidores afetados com evidência comparável. | A fazer |
| GT-013 | Organizar cartões no GitHub Projects | GT-001 a GT-014 | Média | Coordenação | GT-002 | Quadro vinculado ao repositório; cartões com IDs, prioridade, aceite e evidência; migração só após acesso confirmado. | A fazer |
| GT-014 | Consolidar e entregar relatório final | US-001 a US-060 | Alta | Coordenação | GT-010; GT-011 | Documentos versionados, resultados reais separados de planejamento e pendências explicitadas. | A fazer |

## Organização proposta no GitHub Projects

Quadro sugerido: **DroidsCode — Validação de usabilidade**. Usar os estados A fazer, Em andamento, Em revisão e Concluído; itens bloqueados devem ter o impedimento explícito. Campos: ID, frente, prioridade, responsável, prazo e vínculo para a matriz.

Criar primeiro 14 cartões de gestão, com os respectivos IDs US no corpo. Se a avaliação exigir rastreamento individual, criar cartões também para as tarefas US; isso não altera a quantidade de requisitos. Cada cartão deve conter pré-condição, aceite e link de evidência. Um cartão de teste só passa para Concluído com a sessão registrada; a simples redação da tarefa não equivale a execução. Correções de código precisam de escopo próprio aprovado.

**Estado nesta entrega:** arquivos publicados na main; 29 issues criadas (12 entregas de implementação encerradas e 17 itens de backlog abertos). O quadro visual Projects ainda não foi criado; consulte o índice em README.md. A conexão e a escrita no repositório GitHub foram confirmadas; a organização no Projects depende de acesso específico ao quadro. Não se alteraram permissões ou autenticação.
