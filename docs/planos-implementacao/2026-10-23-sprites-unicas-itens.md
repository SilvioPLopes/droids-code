# Sprites únicas por item

**Objetivo:** substituir seleção de ícone por categoria por referência explícita por ID, com imagens distintas para cada item e troca simples. Poções pequena/média/grande formam família visual com volumes/silhuetas diferentes. Usuário autorizou melhorar imagens; não ampliar o estoque da loja.

**Causa observada:** ApresentacaoLojaIlustrada.Icone escolhe entre seis sprites por TipoDeItem; a exceção é Núcleo de Energia. Catálogo possui 27 IDs e a loja atual 18. A aba Vender pode representar itens dos demais IDs; preparar referência visual dos 27 sem alterar regras de venda.

**Escopo permitido:** novo `CatalogoVisualDeItens.cs` em Scripts/Catalogos e asset editável em Resources/Catalogos; imagens novas organizadas em `Assets/sprites/Itens` com recortes nomeados pelo ID; novo helper Editor de importação/validação desses assets; alterar somente resolução visual em ApresentacaoLojaIlustrada.cs e teste ValidacaoLojaIlustrada.cs; este plano, guia de referência e índice/contrato. Preservar campo serializado `icones` e GUIDs antigos por compatibilidade, sem usá-lo como fallback repetido.

**Preservar:** City e demais cenas, prefabs, originais em Atualizados/novos, Canvas/Scaler, layout, preços, disponibilidade, lógica de compra/venda/feedback e saves. Não mover/renomear assets antigos nem excluir duplicados. Componentes compartilhados: catálogo novo sem consumidores anteriores; somente loja passa a usá-lo nesta rodada.

**Estrutura decidida:** três folhas com nove ícones cada (Consumo, Tecnologia, Especiais), preservadas inteiras, com recortes Sprite `.asset` individuais nomeados pelo ID em `Recortes`. Recortar por Sprite.Create reaproveita o mecanismo já usado pela loja, sem instalar pacote nem repintar o bitmap. Um ScriptableObject contém 27 entradas ID/nome/sprite, editável no Inspector; referências individuais podem ser substituídas por sprites de qualquer PNG importado. Nenhum número de índice de categoria deve definir a imagem no runtime. Origens e troca documentadas no guia.

**Assets pretendidos:** Consumo: poções P/M/G, reparo, escudo, óleo, injetor, granada e corrosivo. Tecnologia: EMP, núcleo, servo, sensor, blindagem, giroscópio, sinalizador, acampamento e treinamento. Especiais: chave, núcleo de sobrecarga, ressonância, cobre, filtro, sucata, emblema, bilhete, servo de missão. As duas imagens 3 enviadas são duplicadas já verificadas; usar uma referência, não tratá-las como conjuntos diferentes.

**Aceite:** 27 IDs conhecidos com referências não nulas e distintas; 18 produtos atuais da loja permanecem os mesmos; lista e detalhe usam a mesma referência por ID; poções visualmente diferenciáveis; transparência e enquadramento sem invasão das células; troca de sprite pelo Inspector, sem editar loja ou cena. Nenhuma mudança econômica.

**Backup/reversão:** scripts anteriores e hashes dos arquivos preservados em `output/itens-visuais/backup/`. Reverter apenas hunks próprios; novos assets podem ser desreferenciados sem apagar originais. Testes isolados não gravam saves. Confirmar Unity antes de importar: último levantamento DroidsCode/City, fora de Play, sem compilação em andamento; consultar novamente antes da mutação de assets.

## Etapas e retomada

- [x] Confirmar repetição atual por leitura/reflexão e acrescentar teste de referências únicas.
- [x] Gerar/inspecionar folhas usando referências 3/17, preservar versões originais e registrar prompts.
- [x] Importar sprites por ID, montar catálogo visual e trocar somente resolução de ícone.
- [x] Validar cobertura, ausência de IDs/sprites repetidos, coerência lista/detalhe e renders pequenos.
- [x] Documentar como localizar/trocar/adicionar uma imagem; registrar estado e limites do teste.

## Resultado e evidências

**Editado/salvo:** três folhas novas e 27 sprites `.asset` por ID em `Assets/sprites/Itens`; catálogo editável `Assets/Resources/Catalogos/VisuaisDeItens.asset`; resolução de ícone da apresentação alterada para ID. Campo antigo `icones` mantido serializado e oculto por compatibilidade; não usado como fallback. Menu Editor **DroidsCode → Itens → Abrir catálogo visual** facilita localizar as referências. [Guia de organização e troca](../guias/icones-de-itens.md), com mapa dos 27 itens; README também na pasta de sprites.

**Geração:** ferramenta image_gen integrada, referências locais 3/17; folhas com exatamente nove objetos em grade 3×3, transparência conferida no arquivo e inspeção visual. Resolução pedida 1536×1536, entregue 1254×1254 nos três arquivos. Prompts completos em `output/itens-visuais/prompts.json`: silhuetas próprias, metal/latão, fundo alpha e sem texto/moldura/magenta; poções vermelhas com uma/duas/três faixas e volumes diferentes. Resultados originais no diretório `.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86`: Consumo `exec-8e822bb2-96e1-4fed-9795-75a7282efde5.png`, Tecnologia `exec-2e9d5712-6be8-47a4-8c8d-b59fd66e8807.png`, Especiais `exec-0a64f8a4-f5da-4e22-9735-7c160b838b51.png`. Cópias de projeto ficam nas pastas nomeadas acima.

**Recortes:** medidos pelo alpha real, agrupando componentes conectados pela região semântica da grade e adicionando margem. Assim bicos/antenas que ultrapassam a célula nominal não são cortados. Apenas leitura dos pixels; bitmaps não repintados nem redimensionados. Primeiras três sprites compartilham janela quadrada do maior frasco para preservar tamanhos relativos. Campo `Origem` guarda folha, linha/coluna e retângulo efetivo. Importação única com guarda contra sobrescrita das escolhas futuras do Inspector; `Validar()` não altera assets.

**Teste antes:** reflexão sobre a apresentação existente confirmou 27 itens usando só seis sprites; teste de unicidade retornou falso. Após implementação, `OrganizacaoVisualDeItens.Validar()` retornou OK: 27 IDs com referências/recortes distintos, 18 produtos da loja preservados. Compilador rejeitou uso de GetInstanceID obsoleto no novo helper; verificação foi alterada para comparar diretamente textura/retângulo. Sem atualização de pacotes ou APIs de outros módulos.

**Teste depois:** `ValidacaoLojaIlustrada.Executar(844,475)` e `(1280,720)` passaram, incluindo correspondência de todos os ícones da vitrine com seus IDs e do detalhe com a seleção. Também preservados os testes anteriores de seleção, compra/venda repetida, aviso local, insuficiente, vazio, conversa/callback, rolagem e clique. Testes por API, não input físico; não gravam saves.

**Inspeção visual:** prévias atualizadas em `output/loja-ilustrada/preview-844x475.png` e `preview-1280x720.png`; pequena inspecionada com poções claramente diferentes, reparo/escudo/óleo próprios e terminal portátil no treinamento. Galeria `output/itens-visuais/catalogo-27.png` mostra todos os recortes com nomes e foi inspecionada: objetos correspondentes, distintos e sem partes principais cortadas. Aprovação visual do usuário pendente.

**Preservação e retomada:** alterações anteriores de feedback local mantidas; arquivos antigos não movidos/apagados. Backups dos scripts desta rodada e hashes dos arquivos preservados em `output/itens-visuais/backup/`. Nenhuma cena, regra de item, controlador econômico ou save editados. Próximo passo: usuário avaliar os novos ícones no jogo ou trocar uma referência no catálogo; não reexecutar importação inicial.

**Último estado verificado:** City limpa e ativa, fora de Play, compilação parada e sem falha, zero objetos temporários da loja/galeria; catálogo visual selecionado no Inspector. Hashes de City, CatalogoDeItens e LojaUIManager iguais ao início desta rodada. Diff sem erros de whitespace. Sem commit/push.

Referências: [análise das artes](2026-10-23-aproveitamento-artes-menus.md), [loja](2026-10-23-loja-ilustrada.md). Aceite visual do novo conjunto ainda não recebido.
