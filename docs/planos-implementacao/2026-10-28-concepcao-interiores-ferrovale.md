# Concepção — interiores de Ferrovale por edifício

**Estado documental:** referência arquitetônica consolidada em 29/10/2026. Preferências e correspondências abaixo não constituem nova aprovação visual ou inspeção de cenas.
**Objetivo:** conservar o vínculo fachada→tipo de ambiente e as decisões de arte, sem repetir a cronologia da produção/montagem.

## Fontes para continuar

| Necessidade | Fonte |
|---|---|
| Pessoas, nomes e ambientes atuais | [Matriz de identidade](2026-10-29-refatoracao-nomenclatura-ferrovale.md) |
| Coordenação de mapas e portas | [Plano principal](2026-10-28-portas-interiores-e-mapas.md) |
| Lote instalado e limites do teste | [Montagem de 29/10](../historico/2026-10-29-montagem-cidade-interiores-npcs.md) |
| Colisões, escadas, andares incompletos e diálogos | [Revisão vigente](2026-10-29-revisao-interiores-dialogos-colisoes.md) |
| Artes e proveniência | [Catálogo visual](../../output/interiores-ferrovale/2026-10-28/catalogo-interiores.html), [manifesto de produção](../../output/interiores-ferrovale/2026-10-28/manifesto-producao.json), [mapa de integração](../../output/interiores-ferrovale/2026-10-28/mapa-integracao.json) |
| Cronologia e valores anteriores | [Cópia integral anterior](../../output/backups/2026-10-29-refatoracao-documentacao/lote-02/antes/docs/planos-implementacao/2026-10-28-concepcao-interiores-ferrovale.md) |

O mapa de integração conserva campos de proposta e instalação de épocas diferentes: distinguir sua finalidade antes de consumir ou alterar. Este documento não atualiza JSONs nem substitui evidência datada de montagem.

## Preferências duráveis do usuário

- Pequenas alterações devem criar identidade entre ambientes: distribuição dos móveis, materiais, textura, reparos, conservação e detalhes de uso. Evitar apartamentos integralmente idênticos.
- Variar pisos de azulejo/cerâmica, cimento, metal/chapas e madeira/tacos. Madeira é uma possibilidade, não o padrão obrigatório. Aumentar presença de metal em paredes, estruturas, móveis e pisos conforme fachada.
- Há diferenças de recursos mesmo no pós-apocalipse: moradias mais pobres podem ter remendos coerentes e materiais reaproveitados; as de mais recursos podem ter acabamentos mais completos e conservados, sem luxo deslocado do mundo. Pobreza não significa piso intransitável, lixo gratuito ou ruína sem função.
- Tacos, juntas, retângulos e direções de textura precisam de alinhamento consistente; transições de material devem fechar com intenção. Evitar remendos cuja geometria pareça erro de geração. O usuário cogitou descartar o primeiro apartamento por desalinhamento: A v2 fica fora da seleção de produção, preservada como histórico; B/C/D continuam bases utilizáveis. Não apagar o arquivo A.
- Materiais/acabamento das bases aprovados; câmera top-down de RPG e orientação dos móveis seguindo seu uso. Sem cama/cozinha de kitnet como fórmula, sem pendente de teto, sem personagem ou UI incorporados. Porta única em apartamentos, entrada curta e discreta. Exceções com múltiplas entradas exigem fachada/acessos compatíveis.
- Casa, prédio, mansão, porão e terraço têm necessidades próprias. Usar apenas espaços úteis, sem simular residência funcional completa. Escadas precisam de patamares e leitura de subida/descida; terraços apenas quando propostos explicitamente, sem fingir que fachada comprova um acesso não observado.
- Prefeitura é um DOMO METÁLICO especial: sem madeira, planta ampla curva/radial, piso contínuo de metal/cimento com juntas radiais ou grandes placas curvas, sem grade de quadradinhos ou reaproveitamento literal da sala doméstica retangular. Criar arte exclusiva.

## Identificação e contagem

A coleta original identificou 17 contornos `GameObject...`: E00…E16, sendo 16 PolygonCollider2D e um EdgeCollider2D. Quinze polígonos correspondiam a edifícios; E07 era mancha/resíduo sem prédio e E15 o perímetro. E00 corresponde ao objeto sem número; os demais ao sufixo numérico. Esses IDs não renomeiam objetos automaticamente.

A leitura por faixas encontrou quatro volumes no alto, seis no meio e cinco embaixo. São **15 conjuntos construídos**, não 15 casas, andares ou pessoas. As Casas 1–10 são nomes de coleta. Geminadas e acessos múltiplos não justificam fundir moradias.

A coleta inicial registrou 22 entradas de edifícios, incluindo CasaLip; o lote posterior registrou 23 após acrescentar o segundo acesso de E05. Duas saídas da cidade são contadas separadamente. Contagem histórica não confirma o estado atual do Editor. Coordenadas e método de projeção ficam na [coleta](2026-10-17-auditoria-cenas.md#coleta-de-entradas-da-city--28102026); não ordenar por nome nem reaproveitar pixels de um mapa com outra resolução/Transform/PPU.

## Referência por fachada

Ocupação e nomes dos moradores pertencem à matriz de identidade. Esta tabela guarda somente a leitura arquitetônica; quantidades de pisos adicionais continuam decisões de design.

| ID / coleta | Fachada e arquitetura a preservar |
|---|---|
| E06 / Casa1 | Torre compacta de metal/painéis quentes e anexo vermelho; térreo, superior e anexo. Não inventar segunda porta externa ambígua |
| E05 / Casa2 | Casa baixa alaranjada/azul, duas entradas da mesma moradia; cerâmica recuperada e chapa |
| E10 / Casas3/4 | Geminado laranja/azul; duas moradias independentes com pisos e arranjos distintos |
| E12 / CasaLip | Torre metálica azul/cinza; manter cena, imagem e puzzle existentes; proposta de arte não autoriza substituição |
| E11 / Casas5/6 | Geminadas vermelhas; duas moradias independentes, acabamentos diferentes |
| E09 / Casa maior | Construção em L, madeira/metal; térreo e superior com terraço. Acesso elevado lateral era candidato sem XY confirmado |
| E08 / Prédio com placa | Dois volumes unidos, duas entradas para o principal e ligação superior; não inferir seis quartos funcionais pela fachada |
| E16 / Prefeitura | Domo metálico; salão amplo radial próprio, sem madeira ou grade doméstica |
| E00 / Casa7 | Torre estreita alaranjada; hall, patamar e apartamento. Não presumir outros pavimentos acessíveis |
| E01 / Casa8 | Torre azul/metálica; hall próprio, patamar e apartamento com mais metal |
| E02 / Casa9 e garagem | Casa e garagem com acessos externos independentes; não foi prevista porta de ligação interna |
| E03 / Mansão/casa-chave | Volume largo com janelas em duas faixas; térreo e superior próprios, metal conservado e peças de piso alinhadas |
| E04 / Geminado azul | Dois módulos e acessos independentes; matrizes C/D, sem criar unidade adicional por motivo narrativo |
| E13 / Casa10 | Volumes azul e ocre; hall, patamar e apartamento. Faixa frontal esquerda não foi comprovada como porta |
| E14 / Casa com porão | Duas unidades e acesso central baixo; porão comum proposto, sem pressupor conexão interna entre todas as unidades |

## Artes, reutilização e limites

A produção de 28/10 registrou **24 artes novas** e **seis matrizes em oito usos**. Arquivos, prompts e seleções ficam no catálogo/manifesto, sem segunda lista de produção aqui.

Reutilizações declaradas: patamar genérico em E00/E01/E13; matriz de superior em E08; apartamento B em E00; apartamentos C/D em E04; sala de piso cuidado na unidade direita E14. Reutilizar imagem não torna o ambiente ou morador a mesma entidade.

As revisões selecionadas foram v2 nos superiores E03/E06/E09. As v1 foram preservadas; o lote de montagem registra a quarentena. E09 conserva diferença de posição do vão entre pisos; E06 tem incoerência visual de direção de escada na revisão vigente. Não declarar alinhamento/circulação corrigidos pelo acabamento da imagem.

Bases aprovadas e novas propostas têm aceites distintos. A matriz superior aprovada de E08 foi mantida; o corredor novo com seis portas permaneceu candidato fora de Assets. Quarto3 e andares reservados não significam quartos/pavimentos completos montados. Confirmar arte, chegada, escada e corredor por ambiente antes de expandir.

## Arquitetura de integração

A solução estudada usa uma cena leve por ambiente visitável, prefab base e montagem compartilhada. Prefab e cena são complementares: fundo, limites, obstáculos e portais são configurados por sala; Player/câmera e referências precisam ser coerentes. Não duplicar gerenciadores persistentes nem usar o contorno inteiro da imagem como substituto de paredes/móveis.

Uma mesma matriz pode atender cenas distintas mantendo a identidade de cada prédio. Duas portas da mesma casa podem chegar ao mesmo ambiente com chegadas/retornos próprios. Piso superior é acessado por escada, sem porta de rua artificial. O anexo E06 foi oferecido por interação na montagem; não tratar isso como segunda entrada externa comprovada.

O estudo contou 32 usos de ambientes, 30 artes distintas e dez pares internos propostos. O lote posterior registra cenas e ligações salvas; não repetir as instruções antigas de criar o piloto/lote. O fluxo de transição e save por nome de cena exige compatibilidade antes de renomear cenas, conforme o plano de identidade. Não houve medição de desempenho que sustente promessa de otimização.

### Piloto autorizado da Casa 3 — execução

O piloto E10/Casa03 foi instalado e o usuário confirmou movimento por teclado; ajuste visual da porta ficou reservado a ele. Isso não valida os outros ambientes nem comprova a circulação da montagem posterior. Valores, GUIDs, testes e backups originais estão na cópia integral e em `output/backups/2026-10-28-piloto-casa03/`.

## Pendências e retomada

- Resolver circulação/escadas e alinhamentos por alvo conforme revisão vigente; não criar andares para completar aparência externa.
- Definir quantidade real de unidades/pavimentos das torres e ampliar somente com escopo e aceite próprios.
- Preservar vínculos E02 independentes, E05 compartilhada e geminadas E10/E11 separadas.
- Para alteração visual: registrar resolução, zoom, Canvas/Scaler efetivos, antes/depois e aceite visível; nenhuma propriedade isolada comprova circulação.
- Próximo passo: usar a revisão vigente para o alvo escolhido; usar esta referência ao discutir arquitetura/arte.
- Consolidação documental: nenhuma cena, imagem ou JSON alterado; Unity não consultado, nenhum save acessado. Cópia anterior vinculada; backups de gameplay existentes preservados, sem restauração ou limpeza nova.
