# Menu do Droid — análise visual e composição de equipamentos

## Implementação autorizada de equipamentos — etapa vigente

Usuário solicitou criar agora a mecânica completa: peças identificadas, ícones no menu Apollo, equipar/desequipar por método e persistência. Reutilizar catálogo/ícones existentes; autorização para novas artes não obriga regenerar ícones já exclusivos. Originais em sprites/Atualizados/novos; artes finais de menus em Assets/UI/<menu>, ícones em sprites/Itens/Recortes e relação ID→sprite em Resources/Catalogos/VisuaisDeItens.asset. Não mover arquivos existentes/metas.

**Escopo antes de editar:** DroidPart e quatro subclasses (ID opcional), Droid (consulta de slot), novo ServicoDeEquipamento, DTO DadosDoJogo e SalvamentoJson (savev7 com peças e restauração testável sem troca de cena), TerminalDroidApi/DroidScriptRunner/TerminalUIManager (comandos e rollback de equipamentos em erro Lua), BagUIManager/ApresentacaoBagIlustrada (orientação para terminal), TelaDeDroidManager e novo ApresentacaoDroidIlustrado, somente registro do controlador no Canvas.prefab, meta da arte v2 para preservar quadro completo, fixture Editor ValidacaoEquipamentosApollo e assertivas de versão do consumidor ValidacaoCasaLip (6→7, sem mudar flags), guia de equipamentos e contratos/plano/índice. Preservar cenas, Canvas/Scaler global, outros painéis, economia, preços, itens/catálogos lógicos e sprites. Consumidores: cálculo de HP/atributos, Bag/terminal, checkpoints, Status e menu persistente.

Backup dos fontes/Canvas/meta atuais em `output/equipamentos-apollo/backup/`; copiar/hash do save pessoal antes de testes IO e verificar ao terminar. Testes usam diretório exclusivo de fixture, nunca save padrão; não restaurar sobre progresso novo. Reversão somente por diffs próprios, preservando edições posteriores/GUIDs. Sem commit/push solicitado.

**Contrato:** ID estável da peça é item.Id; serviço valida estoque/tipo/slot, troca devolve peça antiga, mesmo ID é no-op, desequipar devolve unidade. Serviço não grava nem depende de UI. HP atual é preservado e limitado ao novo máximo, sem cura por troca. Slots existentes Cabeça/Tronco/Braço/Perna; sem slot novo. Peça legada sem ID mantém payload no save, mas troca/retorno ao inventário deve recusar sem consumir ou perder a peça; não inferir identidade por nome. Save antigo migra para slots vazios sem inventar equipamentos; v7 preserva payload, raridade e bônus, valida slots/IDs/enums antes de aplicar e restaura peças antes do HP. Versão futura recusada. IDs desconhecidos preservam arquivo e impedem carga com explicação.

Terminal: `droid.equipar("id")`, `droid.desequipar("slot")`, `droid.listarPecas()`. Contexto explícito de inventário; sem acessar singleton silenciosamente em fixtures. Runner desfaz alterações de equipamento/inventário se houver erro Lua nessa execução; isso não promete transação dos atributos/técnicas legados nem proteção de Lua infinito. Estado confirmado é gravado pelo Salvar/checkpoints existentes, como a configuração do terminal; não introduzir autosave a cada comando. Bag orienta para o terminal e não instala por botão.

UI: integrar arte v2 preservada, Apollo atual independente, quatro ícones de peças instaladas, seis atributos, grades paginadas de estoque/técnicas e descrição/ID/comparação. Nenhum botão para equipar/desequipar. Camadas do corpo ficam separadas/preparadas, sem sobrepor ícones de inventário como falsa armadura. Não há novas sprites vestíveis compatíveis nesta entrega. Perfil1536×1024/encaixe local.98; validar844×475/1280×720, resolução/zoom atuais serão confirmados antes de operar Editor.

**Aceite/verificação:** testes de equipar/trocar/desequipar/mismoID/estoque insuficiente/raridade/HP sem cura; script Lua real com rollback em erro; save/reabrir em instância nova preserva quatro slots, bônus e inventário; legado esvazia slots e inválido não aplica estado parcial. Cobertura ID→ícone e menu mostram imagem da peça efetivamente instalada, navegação completa e retorno; comparação renderizada e input real/aprovação visual relatados separadamente. Etapas: testes/regressão → domínio/serviço → save → terminal/Bag → UI → compilação/fixtures/relatório/guia.

## Resultado da implementação de equipamentos — 23/10/2026

**Editado e salvo:** identidade nas peças, serviço de troca/devolução, comandos do terminal, rollback de equipamentos/inventário em erro Lua, save v7 e restauração das peças antes do HP; montagem v2 ligada ao Canvas.prefab. Bag orienta para o terminal. Quatro slots mostram sprite do ID instalado, seis totais em coluna fixa, grades paginadas e inspeção rolável. Ícones existentes foram suficientes; nenhuma arte nova gerada nesta implementação. [Comandos, seis IDs e localização das imagens](../guias/equipamentos-apollo.md).

**Testado no comportamento por API:** `output/equipamentos-apollo/validacao-final.txt` passou em Editor. Cobertura:27IDs/ícones exclusivos,6equipamentos, troca/devolução/idempotência/raridade/estoque/HP sem cura; Lua real e rollback; grava/lê/restaura quatro slots em instância nova, inventário/HP/energia e devolução da peça carregada; legado, ID/slot/duplicata/versão futura inválidos. Regressão de Salvar/Carregar confirmou feedback local sem conversa e preservação de callback/menu; migração CasaLip passou. Fixture CasaLip foi atualizada para admitir versão7 e fornecer energia moderna válida25/100 (máximo0 tem migração100/100 preexistente); nenhuma política de flags/energia alterada por causa do teste. Arquivos temporários exclusivos removidos.

**Comparação visual:** previews844×475 e1280×720 em `output/equipamentos-apollo/`, com `validacao-visual.txt`. Prefab real em PreviewScene/ScreenSpaceCamera/RenderTexture sem Scaler na fixture; encaixe local1536×1024/.98. Quatro ícones por ID,13técnicas paginadas, consulta sem consumo, enquadramento/raycast/retorno/reabertura passaram. Inspeção da prévia844 confirmou rótulos/valores dentro das molduras e corpo independente. **Input real e aprovação visual do usuário pendentes.** City consultada: Canvas ScreenSpaceOverlay/ConstantPixelSize; referenceResolution800×600 não controla escala nesse modo; GameView3840×2160, zoom aproximado.26. Links de arte/corpo confirmados na instância City. Nenhuma cena foi salva/alterada.

**Revisão/compilação:** skill requesting-code-review aplicada com revisor read-only delimitado; nenhum achado importante. Build runtime/Editor passou, dois avisos obsoletos preexistentes de MenuMundoManager em build completo; incremental final0erros/0avisos. Unity scriptCompilationFailed=false antes do import final; fixtures executaram depois do reload. Import provocou desconexão transitória do MCP; estado consultado antes de retomar, sem repetir mutação após timeout.

**Retomada/limites:** Unity mantido City fora de Play. Backups em `output/equipamentos-apollo/backup/`, incluindo fontes/Canvas/meta/save pessoal; hash pessoal D64DA031475BE9E6146145E5B5D31B8F5A3DAA0166C0326A3C4A30D18F3FB54F permaneceu igual, sem restauração sobre progresso. Canvas/Scaler global e cenas preservados. Camadas do corpo reservadas/desativadas; aparência vestida depende de futuras artes compatíveis e cadastro por ID. Saves antigos não recuperam peças nunca gravadas. Salvar/checkpoints persistem comandos; sem autosave por script. Próximo passo: usuário percorrer terminal/menu e aprovar a apresentação; ajustar apenas com evidência nova.

## Segunda proposta artística ampliada — etapa vigente

**Resultado:** criada `Assets/UI/DroidIlustrado/droid-conceito-v2.png`,1536×1024, canto externo transparente; primeira proposta preservada. Original selecionado em `C:/Users/silve/.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-04c1ed7c-6b18-41c0-8ed1-f69d03b9f682.png`. Rascunho anterior da mesma rodada preservado na pasta de geração como `exec-316c2827-e706-42a8-8a46-448d1cb23a28.png`. Geração integrada com referências2, Status-v2 e proposta anterior; segunda chamada buscou uniformizar as linhas do resumo.

Inspeção: janela do corpo vazia e quatro encaixes no alto à esquerda; duas grades4×2 na parte inferior, dezesseis células visíveis no total; resumo à direita; faixa de papel para inspeção e dois campos inferiores (saldo/retorno). Arte se aproxima da densidade/disposição da referência preferida, com materiais cinza/cobre da família atual. Sem personagem ou dados pintados. **Limite da geração:** manteve cinco linhas duplas e uma placa larga acima, apesar do pedido de seis linhas duplas. Há seis superfícies de leitura utilizáveis: futura montagem pode dedicar a placa superior ao primeiro atributo e as cinco linhas aos demais, com coluna numérica fixa em componentes nativos; não tratar a placa superior também como título nem perder um atributo. Se o usuário exigir seis linhas visualmente idênticas, corrigir esse detalhe na arte antes de integrar. Não declarar seis linhas duplas presentes.

Somente proposta visual apresentada, aprovação pendente. Oito células de cada grade não são limite de estoque/técnicas; futura navegação deve manter acesso ao restante. Espaço central recebe Apollo/camadas independentes depois; nenhum compositor, comando de equipamento ou migração de save implementado nesta rodada. Unity não operado; tipografia, encaixe do corpo e interação não testados. Próximo passo: reação do usuário à composição ampliada e decisão sobre uniformidade do resumo.

Usuário solicitou criar a versão melhorada para análise. Escopo: novo `Assets/UI/DroidIlustrado/droid-conceito-v2.png` e registro neste plano; preservar proposta anterior, originais, scripts, cenas, prefabs e saves. Não interpretar pedido da imagem como autorização para implementar os upgrades funcionais. Layout ampliado próximo da referência2: corpo independente à esquerda, quatro encaixes reais ao redor, seis campos de resumo à direita, duas grades inferiores para peças disponíveis e técnicas, área de inspeção/rodapé/retorno. Grades pagináveis na integração futura, sem criar capacidade máxima. Arte vazia, sem personagens definitivos, dados ou botões de equipar pintados. Identidade de metal cinza/cobre do Status-v2, perfil pretendido1536×1024, transparência externa. Aceite: áreas distinguíveis e úteis, espaços maiores que na referência pequena, quatro slots lógicos apenas; comparar composição visual sem alegar legibilidade de tipografia ainda ausente. Reversão: retirar somente a variante nova se solicitado. Sem operação Unity nesta rodada.

## Revisão de escopo e defesa do upgrade — etapa vigente

Usuário prefere a referência2, com mais campos e grades, à proposta reduzida aos quatro cartões. Solicitou releitura dos planos de melhorias e defesa antes de implementação. **Nesta rodada somente este plano é atualizado**; sem imagem nova, scripts, cenas, prefabs, saves ou testes de gameplay. Não interpretar preferência visual como aprovação de uma mecânica nova. A proposta artística anterior permanece preservada, mas não representa o layout preferido agora.

### Fontes de melhoria consultadas e confronto com o código

- [Plano de desenvolvimento](../plano-desenvolvimento.md): SAVE-001 pede identidade/persistência de peças; UI-002 prevê leitura/descrição; capacidades/cartuchos/desequipar não podem ser apresentados como implementados.
- [Plano de melhorias, EST-01 e INV-01](../planos-implemnentação/melhorias/02-estado-inventario-e-economia.md): identidade estável e payload da peça no save; troca/devolução/inspeção, raridade correta e comparação. Estado documental é proposta; código atual ainda não possui ID em DroidPart, não serializa peças e consome a nova peça sem devolver a anterior na Bag.
- [UI-01](../planos-implemnentação/melhorias/04-missoes-mundo-e-narrativa.md): usar campos reais, preservar origem/callback e validar UI local. Não exige duplicar todos os painéis numa única tela.
- [MEC-04](../planos-implemnentação/melhorias/05-mecanicas-e-demo.md): capacidades derivadas de IDs das peças (sensor que acrescenta leitura em puzzle optativo), dependente de EST-01, INV-01 e PUZ-02. ScannerMEC-03 depende também do ciclo de combate. São expansões possíveis, não uma entrega visual pequena.
- [README das melhorias](../planos-implemnentação/melhorias/README.md): escolher entregas pequenas, não implementar o lote completo; recarga e balanceamento ali são propostas históricas, sem sobrescrever a energia de combate definida posteriormente pelo usuário nesta sessão.

Confirmação atual delimitada: inventário por ID, seis definições de itens equipáveis no catálogo, técnicas configuradas, custos reais de energia, seis atributos e quatro slots equipados existem como dados. O controlador TelaDeDroidManager mostra só quatro textos; sua pobreza de apresentação não significa falta de dados no restante do modelo. Os cartões de peças podem usar o catálogo de ícones atual; a aplicação visual ao corpo ainda exige artes compatíveis e compositor.

### Direção nova do usuário: equipamento pelo terminal

O usuário determinou que **equipar será por código no terminal**, sem botão para equipar no menu. Isso substitui a recomendação anterior deste plano de manter a Bag como origem da troca. Fato atual: TerminalDroidApi recebe Droid e oferece atributos/técnicas; não possui comando de equipamento, inventário ou serviço de troca. Bag ainda equipa no código vigente. A migração não foi executada nesta análise.

Recorte funcional a aprovar: serviço único de equipamento que valide inventário/slot antes de mutar, mantenha identidade/raridade, devolva peça anterior e impeça consumo duplicado ao equipar o mesmo ID; fachada do terminal chama esse serviço. Nomes Lua ilustrativos `droid.equipar("id")`/`droid.desequipar("cabeca")` são **proposta**, não API existente. Leitura/inspeção no painel não executa esses comandos. Transferir somente a linha de consumo da Bag ao terminal não basta: é preciso definir confirmação/erro e relação com execução parcial do script; evitar uma mutação irreversível seguida de erro Lua. Coordenação com TER-02 deve ser delimitada antes de editar o terminal, sem ampliar automaticamente para toda a transação global prevista no backlog.

### Upgrade recomendado: ficha ampliada com dados reais

Recomenda-se evoluir a tela além dos quatro cartões, preservando a proporção e densidade da referência preferida, com fontes dimensionadas para uso real. Não copiar a quantidade de pequenos espaços cegamente: grades menores/paginadas mantêm acesso sem transformar informação em decoração ilegível.

| Região visual | Conteúdo e interação proposta | Origem e necessidade |
|---|---|---|
| Corpo à esquerda | Apollo em camadas; quatro encaixes reais próximos ao corpo, seleção apenas para consulta | Quatro slots de Droid; base/camadas independentes como já planejado. Não inventar oito posições equipáveis. |
| Painel direito superior | Resumo de desempenho: seis totais de atributos e contribuição dos equipamentos; nome/vida/energia compactos se couberem | Droid.ObterTotal, StatsBase, peças e energia reais. Ajuda a perceber resultado da montagem; cálculos detalhados continuam no Status. Não confundir bônus de equipamento com efeito temporário. |
| Grade inferior esquerda | Peças disponíveis no inventário, ícone/quantidade/slot; seleção apresenta diferença para a peça instalada | Inventário + CatalogoDeItens + CatalogoVisualDeItens; interação de inspeção, sem equipar. Grade com paginação ou rolagem, sem limite novo de inventário. |
| Grade inferior direita | Técnicas programadas, nome e custo de energia; descrição ao selecionar | TecnicasConfiguradas/ObterCustoDeEnergia reais. Grade de módulos/técnicas, não inventar cartuchos físicos nem limite de memória. Nomes/custos legíveis têm prioridade sobre ícones ainda inexistentes. |
| Detalhe/rodapé | Slot selecionado, bônus atual→candidato, ID de consulta e orientação do comando do terminal quando existir; Gold e Voltar | Contrato INV-01 de comparação, acrescido da nova direção terminal. Comparação somente prévia, sem mutar atributos/HP/energia ou antecipar resultado no corpo como se já estivesse equipado. |

As quatro posições instaladas e a grade de estoque são conceitos separados; raridade/capacidade não viram novos slots. Com inventário vazio, mostrar estado vazio real; não acrescentar equipamentos fictícios para encher a grade. Técnicas são configurações conhecidas, não consumíveis. Suas células não devem parecer botões de executar ataque fora da batalha.

### O que cabe agora e o que deve esperar

**Defesa recomendada:** um upgrade intermediário é justificável. A maior parte do ganho visual vem de expor dados existentes: estoque de peças, técnicas, efeitos numéricos e comparação. Identidade/save (EST-01) é fundação diretamente útil e resolve perda de montagem, não uma expansão decorativa. O recorte de serviço/terminal é necessário para cumprir a direção nova de equipar por código e deve ser tratado como mudança funcional, com testes próprios.

**Não recomendar nesta entrega:** capacidades especiais MEC-04, scanner, novas categorias de peças, cartuchos físicos, slots de memória, crafting/ferreiro ou buffs passivos sem fonte. MEC-04 agrega valor futuro, mas exige obtenção da peça e um puzzle optativo completo; fabricar esses dados só para preencher a tela produziria uma interface que promete o que o jogo não faz.

Alternativa mais simples possível: aplicar apenas as regiões de consulta usando dados atuais, sem persistência/serviço. Custa menos, mas deixa a limitação de perder equipamentos ao carregar e não atende a mudança de equipamento para terminal. Não é a recomendação para declarar a tela funcionalmente concluída; pode ser uma prévia visual antes da entrega funcional.

Sequência proposta, aguardando aprovação: (1) validar esta composição ampliada e criar segunda imagem, preservando a primeira; (2) fechar recorte de EST-01 e serviço/comandos do terminal, com migração e regras de troca explícitas; (3) integrar visual e leitura/inspeção/comparação; (4) compositor com base atual e contrato para a futura base, sem fingir camadas de equipamentos que ainda não existem. Não iniciar esses passos nesta rodada de defesa.

**Resultado e retomada:** proposta defendida com fontes/documentação e confirmação de lacunas atuais; nenhuma implementação autorizada por inferência. Próximo passo: reação do usuário ao upgrade intermediário e à composição próxima da referência2. Estado Unity/Play não consultado; sem backups de progresso necessários ou saves tocados. A aprovação anterior do Status não equivale à aprovação desta imagem ou escopo.

## Proposta artística para avaliação

**Resultado salvo:** `Assets/UI/DroidIlustrado/droid-conceito.png`,1536×1024, canto externo transparente. Original de geração em `C:/Users/silve/.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-93928a07-ed05-4fb7-92b1-2804a10c18b7.png`. Ferramenta image_gen integrada, referências Status-v2,2 e9. Prompt: estação de montagem com uma janela ampla vazia à esquerda, quatro cartões horizontais com ícone/texto à direita, papel para detalhes abaixo e rodapé de orientação/saldo/retorno; sem personagem, nomes, números ou dados pintados. Inspeção visual confirmou essa composição e acabamento cinza/cobre com tubos/engrenagens. Janela do Apollo ocupa aproximadamente x180–730,y130–770, plataforma abaixo; cartões ocupam aproximadamente y120–620; medir coordenadas finais antes da integração. Isso oferece área de desenho independente, mas não implementa o compositor por si só. Imagem apresentada ao usuário, aprovação pendente. Sem scripts/prefab/cenas/saves alterados ou operação do Unity nesta rodada.

Usuário pediu visualizar como montaríamos a imagem. Escopo desta rodada: criar `Assets/UI/DroidIlustrado/droid-conceito.png` e registrar resultado neste plano. Arte nova de estação de montagem horizontal, identidade da v2 do Status e referências2/9; janela grande vazia para corpo independente, exatamente quatro cartões à direita, detalhe e retorno. Não pintar um robô definitivo nem peças vestidas, não criar inventário extra. Preservar originais, scripts, cenas, prefab, saves e demais menus. Reversão: retirar somente o novo asset se solicitado. Aceite: organização visual clara, quatro slots reais e espaço útil para corpo/camadas, sem dados falsos ou tipografia pintada. Inspeção da arte não comprova integração nem legibilidade futura de fontes. Sem operação do Unity nesta etapa.

## Estado e escopo desta análise

Pedido de23/10/2026: reformular o próximo menu, usando referências2/9, mostrando peças equipadas e preparando a futura substituição da imagem do Apollo e sobreposição visual das peças. A figura humana/robótica impressa na referência2 não é o Apollo atual nem a escolha definitiva.

Antes de editar: esta etapa altera somente este plano e link em `docs/indice-trabalho.md`. Estuda implementação e define estrutura proposta; não troca sprite, gera arte, altera mecânicas, migra save ou aplica montagem ainda. Reversão documental: retirar trechos próprios preservando alterações posteriores. Fontes lidas: análise das artes, contrato interface/save, `TelaDeDroidManager`, `DroidPart`, `Droid`, `Braco`, criação de peças em Bag, `CatalogoVisualDeItens`, equipamentos em `CatalogoDeItens` e DTO/save. Nenhuma operação do Unity nesta rodada.

## O que existe e o que falta

| Evidência atual | Implicação |
|---|---|
| TelaDeDroidManager consulta cabeça/tronco/braço/perna e mostra nome/bônus ou Vazio | Preservar os quatro slots e os vazios. Menu é somente consulta; equipar ocorre pela Bag. |
| Droid.EquiparPeca substitui uma peça por slot; ObterTotal soma seus bônus | Equipamentos já têm efeito funcional. Esta reformulação não deve inventar novos slots nem alterar bônus. |
| DroidPart guarda nome, raridade, atributo principal, atributo bonificado e valor; não guarda ID de item | A aparência não pode ser associada de forma confiável pelo nome traduzido. A Bag conhece item.Id no momento da criação, mas perde essa referência na peça resultante. |
| Catálogo visual de itens já resolve ícones por ID | Reutilizar para os cartões quando a referência de origem estiver disponível. Ícone de inventário e camada vestível são assets diferentes. |
| Nenhum compositor visual em Droid/TelaDeDroidManager | A sobreposição proposta precisa de apresentação própria e catálogo das camadas. |
| DroidSalvo/SalvamentoJson não serializam peças equipadas | Reabrir o jogo não restaura a montagem de equipamentos. Persistência precisa entrar no escopo de integração da mecânica, não apenas na arte. |
| Controlador do menu atual não consulta Gold | O rótulo Gold visto na captura não demonstra saldo funcional; numa montagem futura, ligar ao saldo real ou discutir sua utilidade, sem manter campo falso. |

Não inferir sprites de peças pelo nome do item ou reutilizar automaticamente o ícone da loja como uma peça encaixada no corpo. Sprites existentes de itens são referências para desenho; sem pose/alinhamento compatíveis, uma sobreposição seria visualmente incorreta.

## Direção visual recomendada

Referência2 contribui com estação de montagem e corpo em destaque; referência9 com molduras/slots modulares. Usar materiais cinza envelhecido, cobre discreto e mecanismos da v2 do Status, mantendo a família visual. Não reproduzir o robô pintado na2 como fundo permanente.

Ficha horizontal1536×1024 como perfil inicial, dimensionada localmente ao Canvas, sem alterar Canvas/Scaler compartilhados. Proposta:

- Cabeçalho: Droid/Apollo e identificação, com espaço para informações reais; não duplicar a ficha completa de atributos/HP/energia do Status.
- Área esquerda: janela ampla de montagem, com base do Apollo como Image independente. Fundo de oficina discreto e espaço livre ao redor do corpo para as futuras peças.
- Área direita: exatamente quatro cartões — Cabeça, Tronco, Braço e Perna — com slot, ícone, nome e bônus em campos separados. Sem peça: Vazio. Não usar espaços em texto para alinhar números, repetindo o problema corrigido no Status.
- Parte inferior direita: detalhe da peça selecionada e origem/descrição quando disponível por ID. Cartão pode ser selecionado para consulta, sem equipar/consumir item automaticamente.
- Rodapé: orientação de que a troca ocorre pela Bag, saldo real se mantido e Voltar. Não copiar a grade de inventário da referência2: a Bag já tem essa função.

Aceite visual futuro: Apollo reconhecível e grande, quatro slots legíveis sem sobreposição, campos alinhados, ausência de espaços que pareçam slots funcionais inexistentes e nenhuma figura definitiva pintada no fundo. Validar844×475/1280×720 e nomes longos. Captura atual fornecida860×418; resolução total, zoom da Game View e configuração efetiva do Canvas não consultados. Não atribuir falha a escala global sem evidência.

## Estrutura proposta para a sobreposição

Compositor restrito ao retrato do menu nesta primeira etapa. Mudança do personagem no mapa/batalha animada precisaria de artes por direção/quadro, uma etapa separada. No menu, uma pose estática permite validar a mecânica de composição com custo menor.

```text
JanelaDeMontagem
  AparenciaApollo
    BaseDoCorpo
    CamadaPerna
    CamadaTronco
    CamadaBraco
    CamadaCabeca
```

Ordem acima é hipótese inicial de desenho, não regra universal de anatomia; deve ser validada com as artes. Catálogo poderá ordenar múltiplas camadas de um item (por exemplo braço atrás e à frente do tronco). O slot lógico Braço pode representar o conjunto de braços na mesma sprite transparente; não criar slots esquerdo/direito apenas porque o desenho tem dois braços. Mesmo princípio para pernas.

Cada camada é sprite com transparência, desenhada sobre o mesmo gabarito/pose da base. Mask/RectMask2D só recorta a janela de exibição; a montagem das peças vem da ordem de sobreposição e do alpha. Isso evita confundir máscara de recorte com equipamento visual.

### Contrato de arte para facilitar a troca futura

- Um perfil de corpo identificável (`apollo-frontal-v1`, por exemplo), com pose e proporções estáveis. Base atual pode aparecer temporariamente, mas não declarar peças novas compatíveis com ela por suposição.
- Gabarito comum, proposta512×768 por camada para corpo estático; dimensão final a confirmar com a futura arte. Mesma posição de cabeça, ombros, quadris e pés, mesma escala e ponto de origem em todos os arquivos. Transparência fora da peça, sem fundo ou outro corpo pintado.
- Importação preserva o quadro completo transparente. Não recortar automaticamente cada peça pelo conteúdo e depois usar preserveAspect de forma independente: isso altera sua posição/tamanho relativos. Todos os Images do compositor ocupam o mesmo retângulo, com âncoras e pivô comuns.
- Base separada da moldura. Trocar a base por outra pose exige perfil novo e artes de peças compatíveis; trocar somente cores/detalhes mantendo o gabarito pode reutilizar as camadas.
- Se uma peça substituir parte do corpo em vez de cobri-la, a base deve ser segmentada e permitir ocultar a região substituída. Sobreposição simples basta para blindagem/acessórios; membros substituíveis pedem esse contrato adicional para evitar dois braços ou silhueta duplicada.
- Ausência de arte compatível não apaga informação: mostrar nome/ícone/bônus no cartão e manter a representação-base, sem prometer transformação visual inexistente.

### Contrato de dados sugerido

1. Identidade estável: acrescentar `IdDoItemDeOrigem` opcional em DroidPart, preenchido a partir de item.Id pela Bag, sem quebrar construtores legados. Dados de combate continuam independentes de Sprite/Unity UI.
2. Catálogo visual de equipamentos separado do catálogo de ícones: chave itemId + perfil de corpo + slot; lista de sprites de camadas, ordem e, se necessário, regiões-base ocultadas. Um mesmo equipamento mantém seu ícone próprio e suas camadas vestíveis próprias.
3. Compositor de apresentação: recebe base/perfil e peças atuais; limpa camadas antigas antes de aplicar as novas; nenhuma mutação de bônus, inventário ou save para desenhar. Equipamento sem camada compatível mantém cartão funcional.
4. Persistência: guardar slot/ID e os dados necessários da peça (nome/raridade/atributos/bônus) para reconstruir o estado, evitando depender só de nomes ou perder peças legadas/customizadas. Não serializar Sprite, textura ou ordem de draw no save. Definir migração de peças ausentes como vazio; nunca consumir inventário para reconstruir equipamento carregado.
5. Atualização: ao abrir o menu, reconstruir a composição a partir do Droid atual. Se o equipamento puder ser trocado com o painel aberto no futuro, adicionar notificação específica então; não começar por consulta global a cada frame.

## Sequência recomendada de implementação

1. Criar proposta artística do painel com janela do corpo vazia, quatro cartões e área de detalhe; avaliar com o usuário antes de integrar. Manter o Apollo real como asset independente, não o robô da referência.
2. Integrar a ficha somente leitura, usando composição por camadas já preparada. Validar com base atual e slots vazios; não desenhar peças fictícias sobre ela.
3. Incorporar ID de origem e persistência das peças no escopo explícito, com migração/roundtrip em memória e testes de equipar/trocar/reabrir sem consumo adicional. Consumidores a considerar: Bag, DroidPart/construtores, Status/ObterTotal e checkpoint/save.
4. Quando a nova imagem-base chegar, produzir um conjunto piloto compatível (base + uma peça), validar encaixe, oclusão e remoção/troca. Só depois expandir aos quatro slots e múltiplas variantes.

A estrutura é viável com a UI de sprites usada no projeto; não requer introduzir um sistema de animação/esqueleto para a pose estática do menu. Nenhuma arte de equipamento compatível foi comprovada nesta análise. Uma base futura muito diferente não poderá reutilizar automaticamente camadas desenhadas para outra anatomia.

## Retomada e limites

Resultado desta rodada: inspeção do funcionamento atual e proposta de layout, dados e gabarito preservadas neste documento. Nenhuma imagem gerada ou script/prefab/cena alterado; não afirmar que o compositor já foi implementado. Sem operação de Unity, Play, save, testes de comportamento, backup de progresso, commit ou push. Aprovação da composição/gabarito pendente.

Próximo passo concreto: proposta artística da ficha com base independente e quatro cartões; a integração deve registrar arquivos/objetos, backups e aceite antes de mutar. O contrato de persistência continua sendo lacuna demonstrada, não defeito resolvido por esta documentação.
