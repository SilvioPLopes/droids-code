# Auditoria de scripts C# — BLOCO 07: Consolidação

**Escopo:** consolidação dos seis relatórios da auditoria estática por blocos. Não executei testes, compilei, rodei o jogo, abri Unity ou usei MCP. Nenhum código, cena, prefab ou documento existente foi alterado. Este arquivo é a consolidação solicitada.

## 1. Resumo executivo

1. A leitura integral foi concluída para **79 scripts C#** nos escopos dos BLOCOS 01–06; os inventários individuais identificam cada arquivo e o grau de leitura.
2. Foram identificados **achados importantes** em custos de técnica (overflow), aplicação de save inválido, progresso de quests, callbacks de diálogo e configuração de encontros; alguns dependem de configuração ou da regra narrativa desejada.
3. O preço de venda incorreto, já registrado na auditoria geral, permanece confirmado por leitura do fluxo da loja.
4. Também há riscos menores em validações de puzzle/inventário, dicas de puzzle, scrollbar e fronteiras de sorteio.
5. Não foi confirmado achado crítico nem perda de save em execução. Isso não certifica ausência: nenhuma execução, teste ou validação de cena foi feita.
6. As telas não são responsivas por reflow; os YAMLs mostram coordenadas de referência fixas e layouts automáticos localizados. O mapeamento do BLOCO 06 é parcial frente à exigência de tabular todos os valores numéricos de scaler, transforms e câmera, e não substitui comparação visual em resoluções reais.
7. A câmera de Game serializa fundo azul sólido; City e Battle também têm a mesma cor serializada, mas clear flags distintos, então o azul percebido em cada caso precisa de validação visual.
8. Permanecem pendentes perguntas de design e inspeção de cenas/Inspector, especialmente para o Vale, Apollo, quests automáticas, persistência de efeitos e aparência do Game Over.

## 2. Cobertura consolidada

### Relatórios de bloco

- [BLOCO 01 — Combate](2026-10-31-scripts-BLOCO-01-combate.md): nove scripts de `Assets/Scripts/Combat/`, leitura integral.
- [BLOCO 02 — Droid e progressão](2026-10-31-scripts-BLOCO-02-droid-e-progressao.md): oito scripts de Droid e 17 de Scripting, leitura integral.
- [BLOCO 03 — Estado e save](2026-10-31-scripts-BLOCO-03-estado-e-save.md): quatro scripts de Estado, incluindo `SalvamentoJson.cs`, leitura integral.
- [BLOCO 04 — História e Catálogos](2026-10-31-scripts-BLOCO-04-historia-e-catalogos.md): 20 scripts, leitura integral.
- [BLOCO 05 — Menus, interiores, casa, câmera e verificadores](2026-10-31-scripts-BLOCO-05-menus-interiores-camera.md): 15 scripts escopados, leitura integral; sete outros managers da pasta `Menus` não estavam no escopo especificado.
- [BLOCO 06 — Construção de UI e responsividade](2026-10-31-scripts-BLOCO-06-ui-responsividade.md): cinco scripts de montagem e `RefocoTerminalIlustrado.cs` lidos integralmente; prefabs/cenas foram consultados em trechos YAML dirigidos. O mapeamento é parcial para os requisitos de valores exatos por Canvas/câmera, conforme limitação registrada no relatório.

Os 79 scripts dos escopos são 9 + 25 + 4 + 20 + 15 + 6. Os documentos relacionados foram consultados seletivamente, não lidos todos integralmente. A auditoria geral de 30/10 permanece parcial quanto ao inventário completo de documentação e ao restante do código; esta consolidação não converte suas áreas não lidas em cobertura confirmada.

## 3. Estado por sistema

### Combate

O fluxo normal documentado de checkpoint, vitória/fuga e Game Over está alinhado em geral com o código lido. `BattleManager.FinalizarBatalha` não chama diretamente o registro de progresso de quest por derrota/vitória; isso confirma a lacuna prévia, mas não exclui um produtor indireto fora do trecho. O caminho de encontro por tabela rejeita chefes/únicos; o caminho de ID único legado não faz a mesma validação. O ciclo do boss e sua única ação por turno são coerentes com o contrato documentado.

### Droid, técnicas, pontos e Lua

O Droid reúne estado e participação em combate; o runner Lua aplica limites de fonte/retomada/saída e a API expõe mutações de progressão/equipamento. Foi encontrado um caminho de overflow em custo de técnica que, estaticamente, pode transformar gasto em crédito de pontos. A semântica de comando Lua recusado e a reversão parcial após erro/cancelamento são limitações/propostas já registradas, não novos defeitos confirmados nesta consolidação.

### Estado e save

O formato observado é v7; migrações e checkpoint de exploração seguem o contrato consultado. A carga pode, contudo, falhar depois de já mutar parte do estado se o DTO tiver coleções/elementos nulos não cobertos pela validação. A hipótese depende de save malformado; não foi demonstrado que haja um arquivo desses no uso.

### História, quests, catálogos e puzzles

As definições de quests e diálogos são dados, não prova de montagem em cena. A flag da quest de Tácio pode desbloquear a etapa seguinte após uma entrega apesar de o texto indicar três. A verificação do puzzle “Calibragem do braço” não confirma a relação de dobro. A falha de configuração de diálogo pode chamar callback de conclusão apesar de a UI retornar que não abriu. A integração de quests por flags/conversas varia por caminho e não está comprovada globalmente.

### Menus, interiores, casa e verificadores

Terminal, verificadores e controladores locais comunicam por eventos e dependem de configuração de cena. O evento `AoExecutarCodigo` é emitido somente em execução bem-sucedida; erros não contam para a dica por tentativas conectada ao verificador. A pega da scrollbar da Bag tem tamanho fixo. O preço de venda incorreto já estava registrado e continua presente.

### UI, cenas e responsividade

As telas usam composições de referência e medidas absolutas em vários pontos; há escala uniforme e anchors/layouts locais, mas não reflow sistêmico. Canvas compartilhado é usado por 34 cenas e depende de câmera/overrides da cena host. A câmera Game tem clear color azul com clear flags de cor; City/Battle serializam a mesma cor, mas os campos de clear flags são diferentes. Não há demonstração visual de quais áreas efetivamente revelam o fundo em cada proporção.

## 4. Achados consolidados por prioridade

Severidade considera o risco potencial e a evidência estática. “Importante condicionado” significa que a falha depende de configuração inválida ou de uma decisão narrativa que ainda não foi fechada. Nenhum item abaixo foi corrigido.

### 🟡 Importante

#### P1 — Quest de derrota sem produtor de progresso localizado

- **Arquivo/linhas:** [CatalogoDeQuests.cs:103, 261, 419, 443](../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs); [BattleManager.cs:724–741](../../Assets/Scripts/Combat/BattleManager.cs); [GerenciadorDeQuests.cs:16–21](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs).
- **Evidência:** há objetivos `Derrotar`; o encerramento de batalha lido não chama diretamente `RegistrarProgresso`, embora o comentário do gerenciador atribua a BattleManager esse registro. A auditoria geral já registrava a lacuna.
- **Causa provável:** integração de progresso de quest por combate incompleta ou removida sem atualizar comentário/documentação.
- **Limite:** não foi localizado produtor indireto nem foi comprovado qual objetivo está acessível no build.
- **Sugestão:** definir os critérios de contagem e registrar exatamente uma vitória válida; persistir no checkpoint salvável após retorno, prevenindo duplicação de recompensa. Referências: [BLOCO 01](2026-10-31-scripts-BLOCO-01-combate.md), [auditoria geral](2026-10-30-auditoria-geral-projeto.md).

#### P2 — Overflow em custo de técnica pode conceder grande saldo de pontos

- **Arquivo/linhas:** [TerminalDroidApi.cs:166–169, 246–247](../../Assets/Scripts/Scripting/TerminalDroidApi.cs); [TabelaDeCustos.cs:16–17](../../Assets/Scripts/Scripting/TabelaDeCustos.cs); [TecnicaComposta.cs:66–74](../../Assets/Scripts/Scripting/TecnicaComposta.cs); [PontosDeProgressao.cs:15–23](../../Assets/Scripts/Scripting/PontosDeProgressao.cs).
- **Evidência/cause:** duração sem teto entra em multiplicação de custo `int`; um custo negativo é aceito por `TentarGastar` e subtraído do saldo. O BLOCO 02 descreve uma entrada concreta que pode resultar em `2.147.483.647` pontos.
- **Sugestão:** limitar parâmetros, fazer cálculo verificado e recusar custo inválido/negativo antes de mutar técnica ou saldo. O plano de progressão já propõe falha sem mutação, mas não implementa essa salvaguarda.
- Referência: [BLOCO 02](2026-10-31-scripts-BLOCO-02-droid-e-progressao.md).

#### P3 — Save inválido pode deixar o estado parcialmente aplicado

- **Arquivo/linhas:** [SalvamentoJson.cs:225–236, 265–309, 337–367](../../Assets/Scripts/Estado/SalvamentoJson.cs).
- **Evidência/cause:** listas/elementos não são todos validados antes da aplicação; a aplicação já limpa efeitos e altera Droid antes de percorrer as coleções restantes. Null em entrada pode provocar falha nesse percurso.
- **Limite:** condicionado a save/DTO malformado; não há evidência de arquivo defeituoso usado em jogo. EST-02 está proposto no plano.
- **Sugestão:** validar o DTO inteiro e montar estado candidato antes de substituir a sessão ativa.
- Referência: [BLOCO 03](2026-10-31-scripts-BLOCO-03-estado-e-save.md).

#### P4 — Callback pode concluir diálogo que não abriu (condicionado à configuração)

- **Arquivo/linhas:** [DialogoUIManager.cs:215–219](../../Assets/Scripts/Historia/DialogoUIManager.cs); [GatilhoDeDialogo.cs:252–263, 280–296](../../Assets/Scripts/Historia/GatilhoDeDialogo.cs).
- **Evidência/cause:** falta de painel/texto aciona `aoConcluir`, e só depois retorna `false`; o callback do gatilho grava flag/save e pode desativá-lo.
- **Sugestão:** falha de abertura não deve reutilizar callback de conclusão; separar resultados da operação.
- **Limite:** cenas/Inspector não foram inspecionados para determinar se algum diálogo real está nessa condição.
- Referência: [BLOCO 04](2026-10-31-scripts-BLOCO-04-historia-e-catalogos.md).

#### P5 — Venda usa preço de compra, não preço real de venda

- **Arquivo/linhas:** [CatalogoDeItens.cs:77–78](../../Assets/Scripts/Catalogos/CatalogoDeItens.cs); [LojaUIManager.cs:191, 196, 235](../../Assets/Scripts/Menus/LojaUIManager.cs); [ApresentacaoLojaIlustrada.cs:62](../../Assets/Scripts/Menus/ApresentacaoLojaIlustrada.cs).
- **Evidência:** catálogo define `PrecoDeVendaReal`, enquanto exibição e crédito da venda usam `item.Preco`.
- **Causa provável:** fluxo de venda usa propriedade de compra e ignora valor contratado no catálogo.
- **Sugestão:** usar o mesmo valor de venda do catálogo na UI e no crédito e validar zero/preço específico/estoque.
- **Histórico:** já registrado como I-02 na auditoria geral, confirmado no BLOCO 05, não um achado novo.

#### P6 — Modo legado de EncounterZone não filtra chefes/únicos (condicionado à configuração)

- **Arquivo/linhas:** [EncounterZone.cs:124–128, 135–149](../../Assets/Scripts/Combat/EncounterZone.cs); [CatalogoDeInimigos.cs:201–202](../../Assets/Scripts/Catalogos/CatalogoDeInimigos.cs).
- **Evidência:** o modo tabela rejeita chefes/únicos; sem tabela, o ID único é retornado sem consultar esses atributos. O boss Droid do Ivo está marcado chefe/único.
- **Limite:** nenhum valor de zona foi lido, e o plano recomenda manter o boss fora da EncounterZone. Não há prova de configuração atual indevida.
- **Sugestão:** decidir se o campo único representa encontro roteirizado ou aleatório; aplicar a regra apropriada e documentá-la.
- Referência: [BLOCO 01](2026-10-31-scripts-BLOCO-01-combate.md).

#### P7 — Quest de Tácio pode ser desbloqueada antes da terceira entrega

- **Arquivo/linhas:** [CatalogoDeQuests.cs:184–209](../../Assets/Scripts/Catalogos/CatalogoDeQuests.cs); [GerenciadorDeQuests.cs:269–276](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs).
- **Evidência/cause:** a flag necessária à quest subsequente é marcada na primeira conclusão da quest repetível, mas o resumo diz “Depois da terceira entrega”.
- **Decisão em aberto:** confirmar se a terceira entrega é regra de design ou apenas texto. Se for regra, usar contagem/marco; se não, corrigir narrativa.
- Referência: [BLOCO 04](2026-10-31-scripts-BLOCO-04-historia-e-catalogos.md).

#### P8 — Responsividade é dívida confirmada de composição, não um caso isolado

- **Evidência representativa:** medidas absolutas em [MontagemTerminalIlustrado.cs:15–22](../../Assets/Editor/MontagemTerminalIlustrado.cs), [MontagemLojaIlustrada.cs:13–16, 86–124](../../Assets/Editor/MontagemLojaIlustrada.cs), [MontagemBattleIlustrada.cs:40–45, 124–169](../../Assets/Editor/MontagemBattleIlustrada.cs), [MontagemGameOver.cs:50–61, 76–94](../../Assets/Editor/MontagemGameOver.cs) e [RefocoTerminalIlustrado.cs:55–83](../../Assets/Editor/RefocoTerminalIlustrado.cs).
- **Escopo:** Terminal, Loja, Bag, Droid, Status, Menu do Mundo, Battle, Game Over e Diálogo; detalhes por tela estão no [BLOCO 06](2026-10-31-scripts-BLOCO-06-ui-responsividade.md).
- **Causa comum:** composição dimensionada para referências fixas; escalas e `LayoutGroup` locais não fornecem reflow geral nem safe area.
- **Sugestão:** planejar regras de reflow específicas por painel e considerar suas cenas hospedeiras. Não presumir que alterar um CanvasScaler compartilhado resolva todos os consumidores.
- **Validação ausente:** não houve comparação visual antes/depois; faltam resolução/aspecto, modo de tela e host exatos desejados.
- **Lacuna específica do BLOCO 06:** não foram tabulados `screenMatchMode` para cada Canvas, todos os anchors/pivôs e dimensões/posições de cada elemento, nem valores numéricos de `orthographicSize`/aspect de cada câmera. A tabela por tela usa evidências representativas; esses campos exigem leitura adicional dirigida dos YAMLs antes de afirmar um inventário completo.

### 🟢 Baixa prioridade

#### B1 — Verificação de puzzle não valida a relação matemática solicitada

- **Arquivo/linhas:** [CatalogoDePuzzles.cs:156–168](../../Assets/Scripts/Catalogos/CatalogoDePuzzles.cs).
- **Evidência:** “Calibragem do braço” pede armazenar o dobro de Força; as condições verificam somente positividade.
- **Sugestão:** validar a relação `previsao == forca_lida * 2` e, se possível, a leitura do atributo real; caso contrário, alinhar o enunciado ao que pode ser provado.

#### B2 — Remoção com quantidade negativa pode criar item

- **Arquivo/linhas:** [GerenciadorDeEstado.cs:116–130](../../Assets/Scripts/Estado/GerenciadorDeEstado.cs).
- **Evidência:** `TentarRemoverItem` subtrai quantidade sem rejeitar valor negativo. Depende de um chamador inválido.
- **Sugestão:** recusar quantidades `<= 0`.

#### B3 — Tentativa Lua com erro não conta para a dica de puzzle

- **Arquivo/linhas:** [TerminalUIManager.cs:70–73, 283–312](../../Assets/Scripts/Menus/TerminalUIManager.cs); [VerificadorDePuzzle.cs:16–17, 69–71, 140–147, 209–214](../../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs).
- **Evidência:** evento de execução só é emitido no sucesso; o contador de tentativas está associado ao evento.
- **Sugestão:** separar “execução terminou” de “execução teve sucesso” e contar tentativas segundo o contrato do puzzle.

#### B4 — Scrollbar da Bag tem pega fixa

- **Arquivo/linhas:** [ApresentacaoBagIlustrada.cs:108–116](../../Assets/Scripts/Menus/ApresentacaoBagIlustrada.cs); comparação com [ApresentacaoStatusIlustrado.cs:84–91](../../Assets/Scripts/Menus/ApresentacaoStatusIlustrado.cs).
- **Evidência:** pega criada com tamanho `12×60`, sem normalização proporcional feita pela configuração equivalente do Status.
- **Sugestão:** reutilizar a configuração proporcional do Status e conferir visualmente. Registrado nos BLOCOS 05 e 06, contado uma vez aqui.

#### B5 — Comentário de recompensa diz que salva imediatamente, mas o save é consolidado depois

- **Arquivo/linhas:** [BattleManager.cs:565–600, 734–745](../../Assets/Scripts/Combat/BattleManager.cs).
- **Evidência:** `AplicarRecompensas` altera estado em memória; checkpoint ocorre na finalização. O fluxo normal documentado permanece coerente.
- **Sugestão:** corrigir comentário se houver autorização futura; não implica perda comprovada no fluxo normal.

#### B6 — Chance de drop zero tem comportamento de fronteira

- **Arquivo/linhas:** [BattleManager.cs:585–589](../../Assets/Scripts/Combat/BattleManager.cs).
- **Evidência:** com filtro `Random.value > drop.chance`, uma chance zero pode aceitar o caso em que o valor aleatório é exatamente zero.
- **Limite:** efeito raro e dependente da fronteira do RNG; não reproduzido.
- **Sugestão:** tratar chance zero explicitamente e testar limites em fixture determinística, quando testes forem autorizados.

## 5. Hipóteses e sintomas preexistentes ainda não resolvidos

Os itens abaixo não são afirmados como bugs confirmados apenas com esta auditoria:

- **Quest automática por mudanças de flag/cena (I-03):** chamadas pontuais de atualização foram encontradas em diário, conversa e entrega; integração global não confirmada. Contrato desejado precisa ser decidido.
- **Efeitos temporários entre batalhas (H-01):** a carga limpa efeitos; o comportamento de encerramento/vitória/fuga e a regra de design não foram fechados.
- **Duplicação de recompensa em reentrada (H-04):** hipótese anterior, sem reprodução nesta cobertura.
- **Game Over, fundo azul, diálogo inesperado com Apollo no Vale e sprite genérico do personagem:** sintomas relatados, sem reprodução. BLOCO 06 localizou configurações de câmera/UI, mas não identificou qual objeto da cena do Vale inicia diálogo nem validou a arte em runtime.
- **Boss via EncounterZone:** apenas risco condicionado; não foram inspecionadas configurações serializadas de zona.
- **Referências de Inspector e ordem de inicialização:** classes têm dependências obrigatórias, mas a auditoria não prova referência quebrada ou falha efetiva em cena.

## 6. Divergências e documentação desatualizada

- Comentário de recompensa em `BattleManager` fala em salvamento imediato; código consolida depois.
- `docs/contextos/sistemas/interface.md` afirma preservação de sessão bem-sucedida ao reabrir terminal; `TerminalUIManager` cria novo runner em cada abertura, e comentário do verificador descreve reset. A regra desejada deve ser confirmada.
- `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md` tem texto que diz que `AoExecutarCodigo` ocorre após erro; o código atual só emite em sucesso, e proposta posterior descreve mudança ainda planejada.
- Comentário do catálogo de puzzles diz que MoonSharp não protege contra laço infinito, mas `DroidScriptRunner` implementa limite de instruções/retomadas.
- Catálogo/wiki divergem na cronologia Ivo/Célia: “vinte dias” versus datas separadas por onze dias, se estiverem falando do mesmo evento.
- Texto da quest de Tácio indica terceira entrega, mas a flag destrava na primeira.
- Textos antigos sobre Lua sem limites e exposição de comandos de teste são divergências já anotadas no relatório geral; não foram tratados como novos.

## 7. Intenções que não precisam de correção por si sós (⚪)

- Inimigo padrão com uma ação; o plano descreve rotinas de inimigo como trabalho separado.
- Dependências obrigatórias de UI/Inspector declaradas no código não provam referência quebrada sem examinar cena.
- Recusa local de venda para itens não vendáveis e Bag orientando equipamento pelo terminal são comportamentos intencionais documentados.
- Placeholder de itens, XP, pontos por nível e tabelas de balanceamento não pedem rebalanceamento sem requisito.
- Save de combate é checkpoint de exploração e não retomada do turno; contexto de inimigo/flag de vitória não persistido foi descrito como intencional.
- O rótulo “Lua — Em construção” na wiki descreve documentação, não ausência do runner/API.
- A implementação de um verificador genérico e outro legado para Casa não prova coexistência indevida sem inspeção da montagem.

## 8. Plano de ação sugerido

1. **Definir contratos antes das correções condicionais:** progressão por derrota, atualização de quests automáticas, terceira entrega de Tácio, persistência de efeitos, comportamento de sessão Lua ao reabrir, e semântica do modo de ID único em encontros.
2. **Priorizar integridade de estado:** corrigir custo negativo/overflow de técnicas e garantir que save inválido não aplique estado parcial.
3. **Fechar progressão de gameplay:** ligar derrota a quest uma vez, decidir se a venda usa `PrecoDeVendaReal` e alinhar a flag/contador de Tácio ao texto aprovado; avaliar callbacks de diálogo para não concluir em falha.
4. **Fechar validações educativas e de UX:** verificação do dobro no puzzle; eventos de tentativas com erro conforme o contrato; validação de quantidade positiva e pega proporcional da Bag.
5. **Mapear cenas afetadas por configuração:** inspecionar, em escopo separado, valores de zona, referências de diálogo, sprites do Vale, hosts do Canvas e referências obrigatórias de Inspector. Não inferir defeito serializado a partir de scripts isolados.
6. **Planejar responsividade por painel:** definir resoluções/aspectos e critérios visuais; tratar Terminal, Loja, Battle e telas compartilhadas sem alterar Canvas/Scaler comum por suposição. Só depois comparar antes/depois.
7. **Revisar documentação após decidir e implementar:** corrigir comentários de save, contrato de reabertura do terminal, cronologia Ivo/Célia e texto de Tácio; diferenciar sempre planejado, implementado, testado por API/input e aprovado visualmente.
8. **Validar as mudanças em etapa autorizada:** testes de comportamento para lógica, teste de save com dados malformados controlados e comparação visual em resoluções aprovadas. Esta auditoria não executou essas validações.

## 9. Perguntas em aberto

1. Objetivos `Derrotar` devem contar qualquer inimigo, um ID específico ou somente o boss? Em qual cena salvável deve ser persistido o progresso/recompensa?
2. Quests automáticas devem atualizar por qualquer flag, troca de cena, conversa, abertura do diário ou conjunto explicitamente definido?
3. A flag de Tácio deve exigir três entregas, ou o texto “terceira entrega” está impreciso?
4. Efeitos temporários terminam ao sair de uma batalha, persistem por turnos globais ou são limpos em outro marco?
5. Reabrir o terminal deve preservar variáveis/sessão Lua ou iniciar uma sessão limpa?
6. Para o fundo azul e Game Over, qual resolução/aspecto, modo de tela, cena hospedeira e referência visual aprovada? O sintoma azul foi observado em Game, City, Battle ou em outra cena?
7. “Vario” refere-se ao Vale? Qual cena, ponto de entrada e NPC/objeto inicia o diálogo inesperado com Apollo?
8. Qual sprite/arte do jogador deve aparecer no Vale e qual personagem/cena deve ser usada como referência?
9. O modo de ID único do `EncounterZone` representa encontro aleatório ou gatilho roteirizado? Alguma zona atual está configurada com chefe/único?
10. “bugs de desenvolvimento” designa uma pasta esperada em outro caminho/nome? A auditoria geral encontrou `docs/problemas/`, mas não confirmou aquela pasta raiz.

## 10. Arquivos lidos e não lidos

### Leitura integral confirmada nesta auditoria por blocos

Todos os arquivos de código dos escopos específicos dos BLOCOS 01–05 foram lidos integralmente; suas listas, linhas e resumos estão nos relatórios vinculados na seção 2. O BLOCO 06 leu integralmente seus seis scripts de montagem/refoco e apenas trechos YAML das cenas/prefabs listados abaixo.

### Leitura parcial/dirigida de cenas e prefabs

`Canvas.prefab`, `PainelBag.prefab`, `PainelStatus.prefab`, `City.unity`, `Battle.unity`, `Game.unity`, `GameOver.unity` e trechos de `CasaLip.unity` foram consultados em partes, não integralmente. `BotaoItemTemplate.prefab` foi lido integralmente como YAML. Os hosts restantes do Canvas compartilhado e câmeras não foram mapeados individualmente.

### Documentação

Os documentos de sistema, wiki e planos foram consultados por relevância, não todos integralmente. A auditoria geral de 30/10 lista 104 Markdown inventariados e explicita quais tiveram leitura integral, parcial ou não confirmada. Não se declara leitura integral de todos os documentos do repositório.

### Não lidos integralmente

Além dos arquivos fora do escopo de cada bloco, permanecem não lidos integralmente: os sete managers de `Assets/Scripts/Menus/` excluídos do BLOCO 05; as cenas/prefabs consultados somente em trechos; demais hosts do Canvas; e documentos cujo estado de leitura não está confirmado na auditoria geral. A lista de scripts fora da cobertura integral global deve ser consultada na seção 10 do [relatório geral de 30/10](2026-10-30-auditoria-geral-projeto.md); esta auditoria por blocos não afirma cobrir todo `Assets/`.
