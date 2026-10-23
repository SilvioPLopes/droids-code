# Status ilustrado — análise e proposta

## Ajustes de leitura e alinhamento — etapa vigente

Captura do usuário: números em posições horizontais diferentes, siglas redundantes, Gold/pontos excedendo rodapé e custo de técnica quebrando “energia” sozinho. Causa comprovada: atributo e número concatenados com espaços; Gold e pontos no mesmo TMP de duas linhas dentro de um campo baixo; técnica/custo concatenados no mesmo tamanho. Arte aprovada permanece intacta.

Escopo antes de editar: somente `ApresentacaoStatusIlustrado.cs`, verificações da fixture existente `ValidacaoStatusIlustrado.cs`, este plano e contrato de interface se necessário. Separar nome e valor em componentes/colunas fixas; nomes completos sem siglas; XP/pontos lado a lado no campo esquerdo, Gold sozinho no central. Técnica com nome e custo em linhas intencionais/tamanhos distintos; título da ajuda identifica atributo selecionado e margem interna ligeiramente maior. Sem mudar domínio, energia, saves, cenas, prefab, Canvas/Scaler ou arte. Consumidores menu/batalha mantêm callbacks.

Backup dos dois scripts antes da rodada em `output/status-ilustrado/backup-alinhamento/`; reversão somente dos trechos próprios, preservando alterações posteriores. Aceite: números alinhados à direita na mesma coluna, nomes limpos, rodapé dentro das molduras, técnica/custo sem palavra isolada, explicação com margem. Comparar prévia844×475 e1280×720. Captura recebida é recorte794×413, sem resolução total/zoom do Editor conhecidos; perfil local1536×1024 e encaixe.98 preservados, Canvas global não consultado/alterado. Validação mínima: compilação e fixture/render; não retestar energia sem mudança de regras.

Estado anterior verificado agora: fixture de Status passou nas duas resoluções, raycast/retorno/reabertura incluídos; profundidade do botão antes/depois do primeiro render -1/37 confirma a falha anterior como ordem de preparação da prévia. Não era evidência de botão quebrado em jogo. Resultado em `output/status-ilustrado/validacao.txt`.

Inspeção adicional da prévia anterior844×475 revelou pegas das scrollbars invadindo o rodapé. Causa no apresentador: pega com sizeDelta/offset fixo60 além das âncoras proporcionais que Scrollbar atualiza. Correção local: offsets zerados antes de atribuir handleRect; scrollbar passa a dimensionar a pega somente pelas âncoras. Não alterar rolagem de outros painéis por inferência.

Aplicado/salvo: nomes completos sem siglas; campo numérico separado com largura140 e alinhamento à direita em todas as linhas. XP e “Pontos” (saldo dos pontos de evolução) em colunas no quadro inferior esquerdo; Gold sozinho no quadro central, todos na mesma linha-base. Nome da técnica e custo/recuperação em linhas próprias, custo a80% e tom secundário; título da ajuda mostra o atributo selecionado, com margem maior no texto. Regras, arte, prefab e cenas intactos nesta rodada. Fixture ampliada somente para geometria das colunas/rodapé e pegas contidas. Marcador solicita nova prévia; não repetir testes de energia.

Validação até esta atualização: compilação sem erros e inspeção da captura do usuário/prévia anterior; comparação renderizada da nova montagem aguarda importação/fixture no Unity. Resultado anterior não comprova os novos ajustes. Backup em `output/status-ilustrado/backup-alinhamento/`. Próximo passo: ler relatório e conferir novas capturas, depois aprovação visual do usuário. Unity não controlado/Play não interrompido, estado atual não consultado; sem save/commit/push.

## Energia de combate — etapa vigente

Usuário definiu gasto de energia por técnicas e preferiu custo crescente para técnicas mais fortes. Implementação passa a substituir o indicador indisponível. Parâmetros iniciais de design (revisáveis por playtest, sem alterar TabelaDeCustos de progressão): máximo100; técnica não básica custa10×máximo(1, nível de dano), mais5 por turno e por faixa de5 pontos de cada efeito de atributo (mínimo uma faixa), e dano×turnos para dano contínuo, teto100. Durações/níveis negativos não reduzem custo. Ataque Básico gratuito recupera10 (também quando erra); tentativa válida de técnica paga antes de rolar acerto. Atordoamento/morte impedem ação antes desse ponto e não cobram. Saldo insuficiente não causa dano/efeito nem consome energia; tentativa ainda segue o turno existente, com custo/saldo visíveis antes da escolha. Não alterar regras de turno para este recurso.

**Escopo antes de editar:** `Droid.cs`, novo `TabelaDeEnergia.cs`/meta, DTO `DadosDoJogo.cs` e gravação/restauração em `SalvamentoJson.cs`, rótulos/custos de ataques em `BattleManager.cs`, apresentador Status e suas fixtures; novo teste isolado de domínio/persistência em `Assets/Editor/ValidacaoEnergiaApollo.cs`/meta e contratos combate/save/interface. Consumidores: CombatEngine chama Droid.ExecutarAcao; terminal continua cobrando apenas pontos de programação; menu/batalha exibem Status; checkpoints existentes serializam o saldo. Inimigos/contrato IParticipanteDeCombate ficam intactos. Sem modificar cenas, Canvas, arte, itens ou fórmulas de dano.

Backup adicional de arquivos atuais em `output/status-ilustrado/backup-energia/`, preservando a montagem visual anterior. Reversão somente por diffs próprios, não substituir arquivos inteiros sobre edições posteriores. Save real não será tocado: testes de serialização/migração somente DTO/JSON em memória, sem Salvar/Carregar e sem troca de cena.

Aceite: saldo começa100, custo aumenta com dano/efeitos, não fica negativo, básico continua disponível e recarrega até máximo, atordoamento não cobra, Status mostra fração real e custos da lista; saves novos preservam saldo inclusive zero e antigos sem campos iniciam cheios. Compilar runtime/Editor e executar fixture quando possível; distinguir compilação de teste em Unity/input real. Consumo/recarga persistem em memória e no checkpoint existente; não recarregar abrindo painel, trocando cena ou gratuitamente no fim de combate.

### Resultado da energia e validação

Implementado saldo no Droid, cálculo independente em TabelaDeEnergia, cobrança antes do acerto, recarga pelo básico e guarda de insuficiência. BattleManager apresenta custo/saldo e desabilita escolhas sem saldo. Status exibe energia real atual/máxima e custos por técnica; ciano continua provisório/configurável, verde do HP preservado. Save recebe dois campos aditivos, sem mudar a versão6 nem a migração de flags: máximo ausente discrimina legado, saldo zero com máximo válido é preservado. Nenhuma alteração de inimigos, fórmulas de dano, XP, economia, itens ou cenas.

Compilação runtime/Editor passou sem erros, com dois avisos preexistentes em MenuMundoManager. Fixture nativa `ValidacaoEnergiaApollo` executou e registrou sucesso em `output/status-ilustrado/energia-validacao.txt`: reserva inicial, custo crescente/efeitos, cobrança única, insuficiência sem dano/efeito, básico+10 limitado, stun/morte/inválida sem gasto, migração DTO e roundtrip JsonUtility em memória preservando zero. Ampliação posterior da fixture verifica erro de ataque por semente determinística; novo marcador solicita reexecução após importação.

Teste independente .NET executou contra a DLL real compilada: custos10/30 e incrementos por duração/intensidade/veneno40/60/75, teto100, limites de restauração, zero, básico gratuito e migração DTO. Também chamou ExecutarAcao com saldo zero e comprovou recusa sem dano/efeitos. Evidência `output/status-ilustrado/dominio-validacao.txt`; fonte reproduzível em `output/status-ilustrado/testes-dominio/`. Esse teste não executa acerto aleatório nem JSON Unity; esses pontos pertencem à fixture nativa acima. Nenhum teste gravou arquivo de save, carregou cena real ou alterou progresso do usuário; Random.state é restaurado pela fixture nativa.

**Pendência visual descoberta:** fixture de Status parou em “Clique Voltar bloqueado” antes de gravar captura. Hipótese delimitada à preparação do teste: o raycast era feito antes do primeiro Camera.Render da cena de prévia. Somente a fixture foi ajustada para renderizar antes e registrar profundidade dos Graphics antes/depois e primeiro hit; nenhum valor de layout/click do painel alterado por suposição. Reexecução solicitada via `validar.pendente`; aguardar evidência antes de declarar causa ou interação aprovadas. Input real e aceite visual permanecem pendentes. Editor não foi controlado/Play não foi interrompido; não confundir execução automática da fixture fora de Play com teste manual da cena atual.

## Integração da v2 e análise de energia — etapa vigente

### Resultado salvo e retomada

Aplicada referência da v2/retrato/cor de energia somente no registro do controlador em PainelStatus.prefab (três linhas novas). Novo apresentador monta os controles ao abrir e oculta os antigos apenas na instância; referências legadas e callbacks preservados. HP verde proporcional ao valor real e atualizado enquanto aberto; energia vazia com “a definir”, sem valor inventado nem conversão de XP/pontos. Cor ciano provisória exposta no Inspector, ainda sem escolha final. Seis totais de atributos e ajuda, técnicas roláveis, XP, nível, pontos e Gold presentes. `Heroi` legado vira rótulo Apollo apenas na ficha; domínio/save não renomeados. V1 e v2 PNGs intactos. Inicialização idempotente evita fechar o painel se Mostrar for chamado antes de Start.

Verificação: asserção de vínculo falhou antes da montagem e passou depois, para arte e retrato corretos. Compilação runtime e Editor sem erros, dois avisos preexistentes de APIs obsoletas em MenuMundoManager. Um primeiro alvo de compilação incluiu Bag duas vezes (aviso de arquivo duplicado); alvo temporário corrigido sem alterar Bag. Diff de cenas vazio e Canvas.prefab mantém apenas a alteração anterior da Bag. Sem gravação de progresso.

Fixture `ValidacaoStatusIlustrado` preparada em cena de prévia, cobrindo HP0/50/100%, energia explicitamente indisponível, seis atributos, técnicas longas roláveis, enquadramento com escala herdada4, raycast/retorno e reabertura. Marcador `output/status-ilustrado/validar.pendente` solicita execução única na próxima importação do Editor, somente fora de Play/projeto correto. Relatório esperado `validacao.txt` e capturas844×475/1280×720; **não observados nesta sessão até o registro**. Não declarar comportamento/render testados com base em compilação.

Pendente: resposta sobre finalidade/consumo/recarga da energia, execução da fixture e teste visual por input real. Próximo passo: conferir relatório após importação e abrir Status pelo menu/batalha, validar tubos/retrato/textos/retorno; implementar energia de domínio somente com regra definida. Arte v2 aprovada; montagem visual ainda sem aprovação. Editor não foi operado: Play/cena/zoom desconhecidos, nenhuma cena salva ou interrompida. Backup em `output/status-ilustrado/backup/`; sem save, commit ou push. A pergunta sobre energia continua pendente; ausência de resposta não define uma mecânica.

Usuário aprovou v2 e solicitou tubos como vida (verde) e energia do Apollo (cor ainda aberta). Leitura de Droid, TecnicaComposta, TerminalDroidApi, CombatEngine e buscas delimitadas em scripts/contratos: não existe reserva atual/máxima de energia do Droid, custo por execução, recarga ou campo de energia persistido. `CustoTotal` de técnicas cobra pontos de progressão na programação; não é energia consumida na batalha. Puzzle da Cratera usa variável Lua própria; Núcleo de Energia concede bônus de FOR por equipamento. Nenhum desses valores deve alimentar uma falsa reserva do Apollo.

Pergunta enviada ao usuário sobre finalidade da energia: indicador preparado versus gasto em técnicas versus exploração/puzzles. Consumo/recarga pendentes de definição; avançar montagem visual e HP reais enquanto aguarda. Nunca substituir energia por XP ou pontos de evolução. Sem regra confirmada, segundo tubo vazio e explicitamente indisponível, sem inventar valor100/100. Cor provisória configurável no Inspector.

**Escopo antes de editar:** `TelaDeStatusManager.cs`, novo `ApresentacaoStatusIlustrado.cs` e meta, registro desse controlador no `Assets/Prefabs/PainelStatus.prefab`, metas da v2/pasta, fixture isolada `Assets/Editor/ValidacaoStatusIlustrado.cs`/meta e documentação deste plano/contrato. Preservar v1/v2 PNGs, cenas, Canvas/Scaler global, Bag/loja/terminal, técnicas/combate/progressão/saves. Consumidores do PainelStatus: Canvas compartilhado e referências em City/Game/Battle; preservar Fechar/AoFechar nos dois contextos. Nenhuma alteração de regras de domínio sem resposta sobre energia.

**Backup e reversão:** cópias anteriores de PainelStatus.prefab e TelaDeStatusManager.cs em `output/status-ilustrado/backup/`. Restaurar somente alterações próprias nesses arquivos; remover apresentador/referências próprios conferindo diff e preservando edições posteriores. Montagem dinâmica local por instância, como Bag, sem salvar novos objetos em cenas.

**Aceite:** arte v2 ao abrir, Apollo no retrato, HP atual/máximo verde dentro do tubo superior, segundo tubo reservado e identificado para energia, seis atributos com texto escuro no papel, todas as técnicas acessíveis por rolagem, nível/XP/pontos/Gold preservados fora dos tubos, Voltar funcional. Escala local1536×1024 a98% do Canvas; referências844×475 e1280×720. Resolução/zoom/Play atuais não consultados por falta de conector Unity nesta sessão; distinguir compilação, fixture e inspeção em jogo. Validação mínima: compilação runtime/Editor, vínculo dos assets/diff e fixture sem save quando Editor disponível. Aprovação da arte não comprova aprovação da montagem.

## Rodada de proposta artística

### Segunda opção solicitada

**Resultado da variante:** salva `Assets/UI/StatusIlustrado/status-conceito-v2.png`,1536×1024, canto externo transparente. V1 preservada. Geração original em `C:/Users/silve/.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-9d176520-85da-4929-ad03-28d2bf77b56a.png`. Inspeção: metal cinza esverdeado, tubos laterais e alojamentos cilíndricos HP/XP, seis linhas de ficha clara à esquerda, técnicas em quadro escuro e ajuda em papel à direita, dois campos inferiores e botão de retorno. Sem textos ou dados fictícios; pequeno instrumento circular apenas decorativo. Aproxima-se das referências4/8 em materiais e mecanismos. Pautas decorativas nos papéis deverão ser consideradas na tipografia futura; não comprovam legibilidade de dados ainda ausentes. Apresentada ao usuário, escolha entre variantes pendente. Nenhuma integração ou alteração de mecânica realizada.

Usuário considerou a disposição boa, pediu maior semelhança com referências4/8 e determinou preservar a primeira imagem. Escopo desta rodada: criar somente `Assets/UI/StatusIlustrado/status-conceito-v2.png` e este registro; não substituir v1 nem integrar no Unity. Direção: metal cinza envelhecido, tubos e mecanismos pequenos, detalhes cobre discretos e ficha de papel envelhecido, em vez de molduras douradas dominantes. Manter hierarquia horizontal, seis atributos, retrato/HP/XP, técnicas/ajuda e rodapé, sem dados pintados ou botões de evolução. Validar composição e relação com referências por inspeção visual; aprovação pendente do usuário. Reversão: remover apenas a nova variante se solicitado; originais e v1 intactos.

**Resultado:** criada e salva `Assets/UI/StatusIlustrado/status-conceito.png`,1536×1024,RGBA com alpha0 no canto externo. Original de geração preservado em `C:/Users/silve/.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-af0217c2-fb46-4ce9-b6ef-5bdd6d208240.png`. Ferramenta image_gen integrada, referências de acabamento Bag/loja e hierarquia10. Prompt exigiu painel horizontal vazio, retrato, nome/nível e dois alojamentos de barras acima, exatamente seis linhas de atributos à esquerda, técnicas/ajuda à direita, dois campos de informação e retorno no rodapé; sem letras, números, ícones fictícios ou botões de evolução.

Inspeção visual: composição e seis linhas presentes; acabamento coerente com Bag/loja, áreas escuras e molduras cobre/latão. Sexta linha ligeiramente mais baixa que as demais: na integração futura medir a região útil e usar fonte comum que caiba nela, sem assumir linhas idênticas. Campos de nome/nível também diferem das coordenadas sugeridas no prompt; medir a imagem entregue antes da montagem. Imagem apresentada ao usuário; aprovação desta arte pendente. Sem integração, tipografia real ou teste de legibilidade no Unity. Somente arte nova e este registro nesta rodada; cenas/scripts/saves preservados e estado Unity não consultado.

Usuário pediu visualizar a imagem proposta. Escopo autorizado nesta rodada: criar `Assets/UI/StatusIlustrado/status-conceito.png` e registrar resultado neste plano, preservando originais, scripts, cenas, prefab e saves. Sem integração no Unity. Reversão: retirar somente a nova arte se solicitado. Perfil pretendido1536×1024; referência de acabamento loja/Bag, estrutura de10/8. Retrato, rótulos e dados ficam vazios para receber componentes reais; não é necessário decidir identidade do personagem para criar a moldura. Aceite: ficha horizontal, seis linhas de atributos, cabeçalho de retrato/vida/XP, técnicas/ajuda à direita e rodapé de saldo/pontos/Voltar, sem botões de upgrade nem dados fictícios. Validação por inspeção visual da arte; tipografia em jogo e interação ficam para integração posterior.

**Estado e data:** análise em 23/10/2026; usuário enviou captura atual e referências10/4/8/9 para discutir a composição. Sem aprovação de arte ou integração nesta etapa.

**Objetivo:** identificar quais informações existem e têm função, organizar sua hierarquia e propor uma interface coerente com loja, menu e Bag aprovados. Não confundir valores zero com sistemas sem implementação.

**Escopo desta etapa, antes de editar:** somente este plano e seu link em `docs/indice-trabalho.md`. Nenhum script, sprite, meta, cena, prefab, save ou balanceamento será alterado. Reversão documental: retirar apenas os trechos próprios se solicitado, preservando trabalho anterior. Não gerar imagem ainda: pedido atual é analisar com o usuário.

**Fontes necessárias:** [análise das artes](2026-10-23-aproveitamento-artes-menus.md), [interface compartilhada](../contextos/sistemas/interface.md), `TelaDeStatusManager`, `TelaDeDroidManager`, `Droid`, `TabelaDeCombate`, `TerminalDroidApi`, `SistemaDeProgressao`, `ProgressaoDeNivel`, `PontosDeProgressao`, inicialização em `GerenciadorDeEstado` e chamadas em `CombatEngine`/`BattleManager`.

## Conteúdo atual e função demonstrada no código

O Status consulta `GerenciadorDeEstado.Instancia.DroidDoJogador`. Seu nome padrão é `Heroi`, HP base30, FOR5 e VIT3. Portanto o nome e a imagem atual podem sugerir protagonista, mas a fonte dos números é o Droid. Não renomear para Apollo ou Lip nem escolher retrato do humano por inferência. Para a proposta, recomenda-se ficha do Droid com representação correspondente; identidade visual exata precisa ser decidida com o usuário.

| Informação | Função atual | Tratamento proposto |
|---|---|---|
| Nome | Nome do Droid, não nome independente do protagonista | Cabeçalho, respeitando o dado real. |
| HP atual/máximo | Vida do participante em combate; máximo recebe contribuição de VIT | Destaque com barra e valores numéricos. |
| FOR — Força | Contribui para o ataque: FOR×2 mais contribuição da técnica, antes da defesa e crítico | Manter; explicar como dano físico, sem prometer dano fixo por ataque. |
| AGI — Agilidade | Fornece Flee/esquiva no cálculo de acerto do oponente | Manter; não chamar de velocidade/ordem de turno sem implementação correspondente. |
| VIT — Vitalidade | Defesa e acréscimo percentual ao HP base, arredondado para baixo | Manter; explicar defesa e vida máxima. |
| INT — Inteligência | Resistência a efeitos e bônus percentual de cura; Bag usa o bônus nas poções | Manter; não apresentar como mana ou dano mágico. |
| DEX — Destreza | Fornece Hit/precisão no cálculo de acerto | Manter; não mostrar DEX diretamente como porcentagem garantida de acerto. |
| LUK — Sorte | Chance de crítico usada após o acerto | Manter; não associar a drops ou preço sem evidência. |
| Nível e XP | XP aumenta nível e concede pontos; limiar atual é nível×100 | Agrupar com barra de experiência e números reais. |
| Pontos disponíveis | Orçamento de progressão, gasto pela API do terminal | Manter com nome mais explicativo, sem botões de distribuição direta nesta proposta. |
| Técnicas | Todas as chaves de `TecnicasConfiguradas`, incluindo Ataque Basico | Lista com espaço para vários nomes e rolagem; não um único campo de técnica fixa. |
| Gold | Saldo global do jogador em GerenciadorDeEstado, usado na loja | Manter em área secundária, separado dos atributos do Droid. |

Os totais dos seis atributos vêm de `Droid.ObterTotal`: base + bônus das peças + efeitos ativos. Uma futura interface não deve rotular esse total como “base”. Distinção visual de bônus seria uma melhoria adicional, não condição para preservar o conteúdo existente. Todos os seis atributos têm consumidores implementados; esta leitura não mede a frequência/utilidade de cada um nas missões atuais nem comprova balanceamento final. Constantes de combate/progressão possuem comentários de valores provisórios.

`TelaDeDroidManager` já apresenta as quatro peças equipadas, somente leitura. Recomenda-se manter Status como visão de desempenho/progressão e Droid como visão de peças, evitando duplicar a montagem completa de equipamentos agora.

## Análise das referências

| Referência | Aproveitar | Adaptação necessária |
|---|---|---|
| `10.jfif` | Hierarquia de retrato, nome/nível, vida/XP e seis atributos; descrição contextual do atributo | Composição vertical ocupa altura demais na tela horizontal. Remover textos técnicos, valores fictícios e todos os botões “+” da proposta. Sucata não substitui Gold sem decisão de economia. |
| `4-removebg-preview.png`, painel esquerdo | Organização por linhas e associação de ícones aos atributos | Reduzir pautas/decoração de pergaminho para manter coerência com os painéis escuros novos. Conteúdo impresso é referência, não dado real. |
| `8-removebg-preview.png`, painel esquerdo | Campos limpos para retrato, barras e linhas de informação | Adaptar para formato horizontal e seis atributos exatos, sem preencher espaços extras com dados inventados. |
| `9-removebg-preview - Copia.png` | Molduras, placas, barras e detalhes modulares | Escolher poucas peças consistentes com ferro escuro/cobre/latão dos menus atuais. Magenta/halos exigem avaliação se houver recorte; não supor transparência perfeita. |

A captura atual mostra ausência de agrupamento: técnicas junto do HP, pontos quebrados em várias linhas e valores espalhados sobre o cenário. Isso fundamenta uma mudança de hierarquia visual; a captura não identifica resolução efetiva, zoom da Game View nem configuração atual de Canvas/Scaler. Não atribuir esse resultado a escala específica sem consulta futura.

## Direção recomendada para discutir

Ficha horizontal, perfil de referência1536×1024, materiais e contraste da família loja/Bag/menu: ferro escuro, molduras de cobre/latão, engrenagens discretas e fontes claras. Arte sem rótulos, números, barras preenchidas ou retrato fictício; todos os dados e imagens de personagem serão componentes reais.

- Cabeçalho: retrato do titular dos dados, nome, nível, HP e experiência. Barras sempre acompanhadas de números; nenhum HP ou XP pintado no fundo.
- Corpo esquerdo: seis linhas generosas, com sigla, nome completo e total. Seleção de uma linha pode mostrar uma explicação curta do efeito já existente; somente consulta, sem gasto de pontos.
- Corpo direito: técnicas disponíveis em lista rolável e uma área de explicação contextual dos atributos. Não ampliar a ficha para listar todos os números derivados de combate nesta primeira montagem.
- Rodapé: Gold, pontos de evolução disponíveis e Voltar. Nenhuma informação atual é removida; nenhum botão de upgrade é sugerido pela arte.

Alternativas consideradas: copiar o painel vertical10 preserva sua hierarquia, mas reduz tamanho de leitura ao encaixar na tela horizontal; copiar a ficha de equipamentos mistura funções com o menu Droid. A ficha horizontal adapta10/8 usando molduras da9 e mantém espaço próprio para técnicas, que podem crescer.

**Decisões ainda abertas:** titular/retrato da ficha (o código aponta para Droid; a captura usa o nome genérico Heroi), aprovação dessa composição e eventual interesse futuro em distribuição de pontos fora do terminal. A última opção é mudança de mecânica, não requisito da reformulação visual; manter o fluxo atual como padrão. Não renomear dados salvos para resolver a inconsistência visual.

## Aceite e próxima etapa

Aceite desta análise: todas as informações atuais mapeadas, utilidade dos seis atributos demonstrada por consumidores reais e referências diferenciadas de mecânicas existentes. Isso foi atendido por leitura de código/inspeção das imagens, sem teste em jogo.

Após reação do usuário: criar proposta artística horizontal com as áreas acima, preservando originais; antes de integração, delimitar arquivos/objetos permitidos, backup e consumidores de Status. `BattleManager` também abre `TelaDeStatusManager`, além do menu do mundo: uma integração futura deve preservar ambos os retornos, sem mudar contador global ou Canvas/Scaler por consequência.

Validação futura: conferir resolução renderizada, zoom real e Canvas/Scaler; comparar enquadramento/legibilidade em844×475 e1280×720, nomes/técnicas longas, listas com várias técnicas, valores zero, HP parcial e níveis maiores. A lista inteira deve continuar acessível. Não alterar fórmulas nem gravar saves para testes visuais.

**Estado deixado:** somente documentação editada. Unity não operado; Play/cena/zoom não consultados. Sem imagem nova, integração, testes de comportamento, save ou commit. Aprovação visual pendente porque esta rodada entrega análise, não arte final.
