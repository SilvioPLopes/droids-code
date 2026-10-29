# Revisão dos interiores: diálogos, colisões e circulação

## Ordem autorizada e escopo

Pedido do usuário: validar TerminalDroidApi, salvar e fazer commit em nova branch **antes** da análise. Depois registrar os defeitos relatados e separar correções imediatas de trabalho futuro. Não corrigir colisores/portais por tentativa; o usuário fará ajustes pontuais. Não gerar imagens novas.

Checkpoint permitido: assets/cenas/prefabs de jogo e documentação da montagem atual, incluindo mapas já presentes. Preservar alterações locais do conector, Packages e ProjectSettings globais fora do commit. Excluir ferramentas baixadas, backups, anexos e candidatos não aprovados. Referências/GUIDs existentes preservados. Branch de origem: `arte/city-unificada-4k`; HEAD inicial: `a4a25fb`.

Validação prévia: projeto C:/Users/silve/DroidsCode/Assets, City limpa, fora de Play, sem compilação em curso/falha. TerminalDroidApi sem diff contra HEAD; implementação de equipamentos já no commit bf84290. Validar somente comandos equipar/desequipar e rollback em fixture temporária, sem IO de save e sem percorrer o jogo. Salvar assets pendentes; cena limpa não exige resserialização.

Reversão: checkpoint Git preserva a montagem anterior à análise. Nesta etapa só este documento será acrescentado; diagnóstico posterior será documental/read-only até existir causa confirmada e escopo de correção delimitado. Não reverter trabalho alheio.

## Sintomas relatados pelo usuário

- Retrato do NPC ausente na caixa de conversa.
- Opções de resposta com botões brancos pequenos e apresentação inadequada.
- Colisores sobre áreas que deveriam permitir circulação; casas com soft lock.
- Portais fora dos pontos corretos.
- Prédios acessam apenas o primeiro andar; escadas com direção funcional contrária ao desenho.

Capturas do usuário são evidência visual do diálogo sem retrato/opções e do posicionamento do Player junto ao sofá no hall. Cena exata, causa geométrica e resolução efetiva ainda precisam de identificação; posição do personagem sozinha não comprova qual collider bloqueia. O relato de soft lock invalida qualquer presunção de circulação concluída baseada na checagem estrutural anterior.

## Aceite e validação mínima

Documento deve identificar causa confirmada versus hipótese, alvos por arquivo/cena, risco e ordem de correção. Retrato precisa corresponder ao interlocutor; escolhas devem ter leitura/hierarquia e enquadramento adequados, sem mudar o Canvas/Scaler global. Circulação exige caminho chegada → interação → saída com volume dos pés do Player considerado; portais coerentes com o desenho das escadas e chegada fora de triggers/bloqueios. Testes estruturais não equivalem a input real/aprovação visual. Sem Play ou gravação de progresso nesta rodada.

## Estado

Validação atual de TerminalDroidApi passou por API no Editor: equipar/listar via Lua, erro Lua desfaz inventário/peça, desequipar devolve estoque, ID/slot inválidos recusados. Fixture temporária encerrada e singleton restaurado em finally; zero gravações de save, sem Play. AssetDatabase.SaveAssets executado; City já estava limpa. Não houve alteração nova no fonte TerminalDroidApi.

Checkpoint concluído **antes da análise**, commit `49857b4`, branch `revisao/interiores-dialogos-colisoes-2026-10-29`. Assets salvos; City limpa, fora de Play. Um index.lock antigo de 24/10 foi preservado em `output/backups/2026-10-29-checkpoint-git/index.lock-antigo`, depois de confirmar ausência de processo Git ativo. Não houve limpeza de objetos Git. Alterações do conector, Packages e ProjectSettings globais ficaram fora do commit.

## Diagnóstico posterior ao checkpoint

### Retratos: causa confirmada

`MoradorInterativo.Fala` informa somente nome/texto. `DialogoUIManager.MostrarFalaAtual` resolve nome em `retratosConhecidos` ou `Resources/Retratos/<nome>` e esconde a Image quando não encontra sprite nem retrato padrão. A sprite usada pelo NPC no mapa não é automaticamente um retrato.

Nos 22 prefabs de Ferrovale, a consulta atual encontrou arquivos correspondentes somente para **Dara, Senhor Tácio e Homem da fila**. Outros 19 não têm correspondência; inclui Idoso da captura. Há cinco retratos em Resources ao todo, contando Lip/Apollo. City tem `imagemRetrato` preenchida, retrato padrão vazio e duas exceções manuais. Portanto, não é necessário criar uma nova ilustração para corrigir a ausência: o caminho imediato é fornecer a sprite existente do interlocutor como alternativa, preservando os retratos já disponíveis.

**Escopo proposto de correção:** `MoradorInterativo` + contrato de retrato explícito por fala em `DialogoUIManager/FalaDeDialogo`, se adotado. Identificar por ator/ID, não cadastrar globalmente por nome genérico: as duas crianças compartilham "Crianca" mas têm sprites diferentes. O fallback deve valer durante conversa, oferta/andamento/entrega e falas narrativas do mesmo ator; não deixar retrato do NPC anterior após fechar ou trocar o falante. Consumidores do manager compartilhado: gatilhos narrativos, CasaLip/Apollo, terminais/avisos, diálogos de quest. Não mostrar rosto em narração sem falante. Validação mínima: caso com retrato existente, sem retrato, crianças distintas e troca para narração. Ainda não implementado nesta revisão.

### Escolhas: causa de apresentação confirmada

`DialogoUIManager.MostrarEscolhas` instancia `prefabBotaoEscolha` sem aplicar apresentação própria. Na City ele aponta para **BotaoItemTemplate**, 260×30 unidades locais, texto nominal26.85. PainelDeEscolhas é filho de PainelDialogo; retângulo consultado fora de Play10×10 antes do layout dinâmico. Esse retângulo sozinho não comprova o tamanho em runtime. A captura comprova os botões brancos estreitos e separados visualmente da moldura.

Estado consultado: GameView3840×2160; Canvas ScreenSpaceOverlay, scaleFactor1, Scaler ConstantPixelSize; escolhas com escala herdada2. Zoom de visualização da captura não foi medido e não foi assumido como causa. Nenhum Canvas/Scaler foi alterado.

**Correção imediata proposta:** template exclusivo das escolhas, fundo escuro e borda metálica compatíveis com a moldura, texto claro, espaçamento/margem, altura para rótulos longos e estados de foco/seleção visíveis. Posicionar o conjunto como parte do diálogo, sem cobrir texto/retrato; permitir rolagem se houver muitas quests. Usar UI real com assets existentes, sem gerar imagem. Não editar BotaoItemTemplate compartilhado da Bag/loja para resolver diálogo. Antes de aplicar: identificar as instâncias locais de DialogoUIManager nas cenas (o manager consultado não veio do Canvas.prefab); a propagação precisa alcançar City/CasaLip e os interiores, sem replicar uma alteração manual em cada painel. Comparar render pequeno e3840×2160, escolha curta/longa/múltiplas opções, teclado e mouse. Ainda não implementado nesta revisão.

### Colisões/soft lock: mansão identificada e causa sustentada

A segunda captura corresponde à arte **e03-mansao-terreo.png**, cena `Interior_E03_Terreo`, prefab `Assets/Prefabs/Interiores/E03_Terreo.prefab`.

Os três BoxCollider2D salvos são blocos: balcão `[500,260,940,480]`, conjunto de estar esquerdo `[190,520,720,830]`, conjunto direito `[780,520,1260,825]`, em pixels da imagem1448×1086. Os dois últimos cobrem tapetes/piso entre móveis, não apenas sofá, mesa e cadeiras. Entre eles sobra corredor de **60px**; o círculo do Player tem diâmetro **120px** nessa escala (raio0.3 mundo,200PPU). Entre balcão e bloco de estar sobra40px. Assim, uma posição pode estar livre pontualmente e mesmo assim isolada da saída.

O helper anterior `Free()` escolhe um ponto sem sobreposição, próximo do desejado. **Não verifica conectividade com saída nem correspondência do collider com o móvel desenhado.** Isso explica por que a checagem estrutural anterior passou sem comprovar circulação.

Inspeção estática atual exportou os colliders dos31 prefabs e as22 chegadas da City para os novos interiores. A [análise de conectividade](../../output/revisao-interiores/2026-10-29/analise-conectividade.json), grade10px com círculo dos pés, identifica a chegada da mansão em pixel **(425,435)**, numa componente sem portal alcançável. O retorno do andar superior chega em (1250,435), na componente da escada, mas também sem caminho à saída da rua. O resultado sustenta o soft lock relatado, sem exigir percorrer o jogo.

**Não basta mudar a chegada:** a saída inferior continua separada pelos blocos. Correção prioritária é redesenhar os obstáculos da mansão por móvel, preservando o corredor central e o acesso inferior, e só então posicionar chegada/portais. Sem reduzir o collider do Player para mascarar o problema.

**Limites:** diagnóstico geométrico dos prefabs, sem simular física nem corpos dos NPCs, sem ler overrides de cada instância de cena. Componentes pequenas isoladas em outros ambientes indicam regiões suspeitas, não comprovam soft lock em todas as chegadas. Casa03 usa configuração anterior sem esse EdgeCollider e exige inspeção própria. Não há liberação de circulação global. Dados: [geometria atual](../../output/revisao-interiores/2026-10-29/geometria-prefabs.json), [chegadas](../../output/revisao-interiores/2026-10-29/chegadas-city.json), [script reproduzível](../../output/revisao-interiores/2026-10-29/auditar_geometria.py). Esses arquivos só registram diagnóstico; não alteram jogo/save.

### Prédios e sentido das escadas

| Alvo | Estado confirmado | Encaminhamento |
|---|---|---|
| E00/E01/E13 | Hall → um Patamar → Apartamento01. O Patamar só tem retorno ao hall e porta do apartamento. `EscadaSuperiorReservada` mostra texto de degraus instáveis. | É montagem parcial, não acesso completo aos andares da fachada. Quantidade de andares/apartamentos precisa ser definida; criar conexões sem reutilizar o mesmo nome de cena para todos os níveis. |
| Matriz do patamar | Desenho possui escada de subida à esquerda e descida à direita; portal salvo da direita em pixel(1135,390) retorna ao hall. Esquerda não tem transição superior. | Preservar a distinção visual; colocar cada trigger na boca correta e chegada fora dele. Não inverter os dois destinos em lote. |
| E06_Superior | Arte aprovada representa escada com aparência de subida, mas único portal `escada_E06_Terreo` retorna ao térreo. | Incoerência visual real; simples troca de nome/destino não transforma desenho de subida em descida. Definir retorno coerente/ajuste visual pontual, sem gerar/substituir arte agora. |
| E03_Superior | Desenho tem abertura escura da escada de descida; destino salvo é E03_Terreo. | Direção topológica recíproca existe; trigger e aproximação precisam ser alinhados na boca do desenho, após resolver os blocos do térreo. |
| Demais pares, inclusive E08/E09 | Referências recíprocas não validam a orientação desenhada nem a acessibilidade. | Marcar subida/descida por imagem e revisar por par. Não presumir que todos têm a mesma inversão. |

Não remapear todos os prédios para corrigir um caso. Identificar em cada imagem: boca da escada, direção visual, portal, ponto seguro de saída, andar de origem/destino. Chegada deve permitir afastar-se e voltar sem disparar imediatamente o portal contrário.

## O que aplicar primeiro e o que pode esperar

| Prioridade | Trabalho | Pode usar os assets atuais? | Condição de conclusão |
|---|---|---|---|
| P0 — próximo ajuste | Mansão E03: separar colliders por móvel e validar corredor/saída; conferir chegadas nos dois sentidos. | Sim; não requer arte nova. | Geometria conectada + teste manual do usuário de entrar, circular, sair e voltar da escada. |
| P0 — outros relatos de bloqueio | Triar os demais ambientes por chegada→saída, volume dos pés e NPCs. Registrar cena/objeto exatos antes de editar. | Sim. | Nenhuma chegada isolada; ajuste pontual, sem reconstruir toda a montagem. |
| P1 — apresentação atual | Retrato do interlocutor usando sprite existente e template exclusivo de escolhas. | Sim; retratos ilustrados novos são opcionais futuros. | Mesmo interlocutor/retrato correto, texto legível, escolhas integradas à moldura, input preservado; aprovação visual do usuário. |
| P1 — circulação vertical existente | Alinhar portais dos pares já montados às bocas e sentidos visuais; conferir saída/retorno. | Em parte; E06 exige decisão sobre incoerência do desenho. | Um par validado por vez, sem trocar destinos por padrão global. |
| P2 — expansão dos prédios | Definir número de andares/unidades e ampliar grafo de ambientes, moradores e interiores necessários. | Pode reutilizar matrizes com variações; novos espaços exigem escopo próprio. | Todos os andares planejados realmente acessíveis. Não declarar prédio completo com uma mensagem de reserva. |
| Depois da circulação/UI | Retratos inéditos, decoração especializada, quartos extras e rotinas/horários dos NPCs. | Trabalho adicional. | Não bloquear correções funcionais com geração de arte. |

## Entrega desta rodada e próximo passo

**Editado/salvo:** checkpoint anterior à análise e este diagnóstico com evidências. **Validado por API:** TerminalDroidApi/equipamentos e rollback; nenhuma mudança nova nesse código. **Inspecionado:** retratos, configuração das escolhas, geometria dos prefabs, chegadas da City e artes citadas. **Não aplicado:** alterações de UI, collider, portal, escada ou imagem depois do relato de regressão. Não há novo aceite visual nem teste de caminhada/Play nesta rodada.

Próxima intervenção delimitada: **E03_Terreo/Colisoes/Obstaculo_1..3 e respectivos pontos de chegada**, com snapshot e comparação com a imagem antes de tocar nos demais interiores. Em seguida, apresentação de diálogo. O checkpoint permite comparar com a montagem recebida; não usar checagem estrutural verde para encerrar a reclamação de soft lock.
