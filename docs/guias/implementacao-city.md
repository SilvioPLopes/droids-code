# Implementação de City — NPCs e eventos do Ato 1

## a. Objetivo, fontes e limites

- [I] Objetivo: contexto de montagem para sessões futuras; identificar locais, personagens, diálogos, dependências e sprites, sem resolver lacunas de design.
- [I] Escopo desta entrega: somente este guia; nenhuma cena, código, catálogo, save ou outro documento pode ser alterado nesta tarefa. Reversão: remover apenas este arquivo novo. Validação mínima: cobertura dos IDs `city_`, fontes, caminhos e limite de linhas.
- [I] Premissa fornecida pelo usuário, não auditada: Homem da fila, Dara, Tácio e Apollo são **teste existente, a reposicionar**. Não presumir posições, componentes ou aparência aprovados. Não foi feita auditoria de City.unity.
- [I] Legenda: **[D]** diálogo; **[C]** catálogo/dados de assets; **[E]** enredo; **[I]** inferência, sugestão ou lacuna. Rotinas e posições são **dado atual, sujeito a mudança**.
- [I] Hierarquia desta tarefa: **D > C > E > sprites**. Havendo conflito, usar o conteúdo de D como base operacional deste guia e preservar as versões lado a lado em g; não editar ou conciliar as fontes por conta própria.
- [D] Fonte principal lida: `Assets/Scripts/Catalogos/CatalogoDeDialogos.cs` (doravante **D**), especialmente linhas 190–340 e 638–805: 35 blocos `city_`. Nenhuma fala foi copiada.
- [C] Fontes lidas: `Assets/Scripts/Catalogos/CatalogoDeNpcs.cs` (**N**), `CatalogoDeLocais.cs` (**L**) e `CatalogoDeQuests.cs` (**Q**), todos nesse mesmo diretório; identidade, local padrão, rotina, quests e flags.
- [E] Complementos lidos: `docs/enredo/TCC_-_ENREDO_v2.md` (**E**), geografia, sessões e notas de montagem; `docs/enredo/TCC_-_ENREDO_v2_ADENDO.md` (**A**), pistas e correção de cânone. Não substituem D nesta tarefa.
- [C] Assets conferidos: `Assets/sprites/Entities/Characters/` (**S**); `Documentation/character-catalog.md`, `sprite-mapping.md` e `animation-coverage.md` como referência auxiliar de produção, sem substituir os catálogos.
- [I] Os caminhos abreviados acima são relativos à raiz do projeto; citações `D:190`, `N:127`, etc. significam arquivo:linha. Não são evidência de comportamento em runtime.
- [D] D declara somente conteúdo e IDs, sem condições/flags de gatilho: **ND = não declarado**. ND não significa configuração vazia autorizada. Condição de quest não equivale a condição do diálogo nem de presença do objeto.
- [I] Modos sugeridos abaixo não são decisões de montagem; nenhuma flag impeditiva ou encadeamento novo foi inventado como contrato. Antes de implementar, resolver as lacunas necessárias ao recorte autorizado.

## b. Resumo do elenco de City

[D] Contagem = blocos `city_` em que há fala do personagem; não conta mera menção, narração ou diálogo de outra cena. [C] Caminho = relativo a S, com `sprite.png`; o diretório também contém `spritesheet.png` quando indicado por existir o sprite.

| NPC | ID [C], salvo indicação | Local atual | Nº [D] | Sprite [C], relativo a S | Status |
|---|---|---|---:|---|---|
| Apollo | `apollo` | [D] acompanha Lip; mirante, portão, soleira | 10 | `Companion/Apollo/sprite.png` | [I] teste existente, a reposicionar; [C] arte placeholder |
| Lip | [C] sem entrada em N | [D] percurso de Apollo; cozinha na mochila | 9 | `Protagonist/sprite.png`; variante `Protagonist/variants/com_mochila/sprite.png` | [I] ator do jogador; não é NPC de serviço |
| Dara | `dara` | [C] `loc_poco`; [D] praça e portão leste na S7 | 6 | `NPCs/Dara/sprite.png` | [I] teste existente, a reposicionar |
| Homem da fila | `homem_fila` | [C] `loc_poco` | 1 | `NPCs/Homem_da_Fila/sprite.png` | [I] teste existente, a reposicionar |
| Senhor Tácio | `tacio` | [C] `loc_mercado`; [E] posto de vigia no sino | 3 | `NPCs/Tacio/sprite.png` | [I] teste existente, a reposicionar |
| Mestra Bruna | `mestra_guilda` | [C] `loc_guilda`; [D] assembleia | 4 | `NPCs/Mestra_Bruna/sprite.png` | [I] montagem não verificada |
| Nilce | `estalajadeira` | [C] `loc_estalagem`; [D] porta na assembleia | 1 | `NPCs/Nilce/sprite.png` | [I] montagem não verificada |
| Quirino | `ferreiro` | [C] `loc_oficina`; [D] oficina no reparo | 2 | `NPCs/Quirino/sprite.png` | [I] montagem não verificada |
| Sula | `vendedora_pecas` | [C] `loc_barraca_sula`; [D] assembleia | 1 | `NPCs/Sula/sprite.png` | [I] montagem não verificada |
| Pipo | `crianca_rua` | [C] `loc_beco_carcacas`; [D] bica e chegada | 2 | `NPCs/Pipo/sprite.png` | [I] montagem não verificada |
| Seu Onofre | `velho_viaduto` | [C] `loc_mercado`, junto ao pilar | 3 | `NPCs/Seu_Onofre/sprite.png` | [I] montagem não verificada |
| Nadir | `nadir` | [D] portão e praça; [C] `loc_praca` | 5 | `Antagonists/Nadir_Quintela/sprite.png` | [I] montagem não verificada |
| Rasha | `rasha` | [D] batente da Guilda na assembleia Ferro | 1 | `NPCs/Rasha/sprite.png` | [I] aparição de rota, não moradora permanente |
| Teodoro | `teodoro` | [D] assembleia Lótus | 1 | `NPCs/Teodoro_Sampaio/sprite.png` | [I] aparição de rota, não morador permanente |
| Ivo | `ivo` | [D] casa vazia/foto; relato de visita na rota Lótus | 0 | `Antagonists/Ivo_Rangel/sprite.png` | [I] sem encontro presencial `city_` escrito |
| Célia | [C] sem entrada em N | [D] foto da casa de Ivo | 0 | `NPCs/Celia_Ferraz_Rangel/sprite.png` | [C] referência visual de foto; não NPC presencial |
| Carregadores (2 figurantes) | [C] sem entradas em N | [D] comitiva no portão | 0 | `Antagonists/Carregador_Apex/sprite.png` | [I] um modelo disponível; duas instâncias não verificadas |
| Droid de escolta | [C] sem entrada em N | [D] comitiva no portão, desligado | 0 | [C] sem conjunto identificado em S | [I] visual e objeto pendentes |
| Benta | `benta` | [C] `loc_posto_benta` | 0 | [C] sem conjunto nominal em S | [I] integração narrativa em aberto |
| Zeca | `zeca_cerca` | [C] `loc_portao_norte` | 0 | [C] sem conjunto nominal em S | [I] integração e divergências em aberto |

## c. Mapa de locais narrativos — não são nomes de cenas Unity

| Local / ID atual | NPCs e diálogos |
|---|---|
| [D] Alto do container; [C] candidato `loc_mirante` | [D] Apollo/Lip: `city_s1_mirante`; [I] equivalência com tanque não resolvida (g) |
| [C] Poço `loc_poco` / praça `loc_praca` | [D] `city_s2_dara`, `city_s2_homem_fila`; `city_s7_agua`; `city_s7_nadir_oferta`, `city_s7_assembleia`, `city_s7_dara_fala`, variantes `city_s7_nadir_*`; [I] posição exata da oferta não dita em D |
| [C] Mercado `loc_mercado`, pilar do viaduto | [D] `city_s2_tacio`, `city_s2_onofre`, `city_s7_onofre`; [C] Tácio/Onofre; [E] Onofre permanece no pilar ao ouvir a água |
| [C] Barraca `loc_barraca_sula` | [D] `city_s2_sula`; [C] Sula |
| [C] Oficina `loc_oficina` | [D] `city_s2_quirino`, `city_s7_quirino_perna`; Quirino, Apollo e Lip no reparo |
| [C] Guilda `loc_guilda` | [D] `city_s2_bruna`, `city_s2_bruna_drone`; `city_s7_nadir_ferro` começa com Rasha no batente |
| [C] Estalagem `loc_estalagem` | [D] `city_s2_nilce`; Nilce na porta durante `city_s7_assembleia` |
| [D] Casa de Ivo; [C] sem ID próprio em L | [D] `city_s2_casa_ivo_cadernos`, `city_s2_casa_ivo_foto`, `city_s2_casa_ivo_bilhete`; Apollo acompanha investigação; Ivo ausente |
| [D] Portão leste; [C] catálogo contém `loc_portao_norte` | [D] `city_s2_apollo_conclusao`, `city_s2_apollo_portao`, `city_s7_dara_cantil`; [I] chegada/partida nesse mesmo acesso; [E] recusa antes do portão |
| [E] Posto de vigia e saída de casa | [D] `city_s3_sino`; [E] Tácio no posto, gatilho na saída de casa; não confundir local do falante com local do trigger |
| [D] Soleira/cozinha de Lip; [C] `loc_casa_lip` | [D] `city_s7_apollo_revelacao`, `city_s7_mochila`; [I] representação em City vs transição para CasaLip não determinada pelo prefixo |
| [D] Entrada da praça na rota Ferro | [D] `city_s7_dara_ferro`; [I] extensão às variantes Lótus/Desgarrado é sugestão, não local expresso |
| [C] Beco `loc_beco_carcacas`, casa `loc_casa_dara`, posto `loc_posto_benta`, ferro-velho `loc_ferro_velho` | [C] locais de rotina/quests; [I] sem novos IDs `city_` específicos para suas atividades |

## d. Matriz dos 35 diálogos e fichas por NPC

[D] Em todas as linhas, requisitos, impedimentos e concessões são ND **em D**. As células acrescentam apenas complementos identificados. [I] “Por script” e “automático” são sugestões de orquestração, não afirmações sobre componentes existentes.
[I] Não transformar dependência de sessão em condição universal para todo diálogo, nem esconder um NPC porque uma fala foi concluída. Não conceder a mesma recompensa/flag por duas fontes sem definir o responsável.

| ID [D] / fonte | Falantes [D] (N = narração) | Necessárias: complemento a ND | Impeditivas | Flag concedida: complemento a ND | Local citado em D ou complemento | Gatilho sugerido |
|---|---|---|---|---|---|---|
| `city_s1_mirante` D:190 | Apollo, Lip, N | [E] `ato1_casa_resolvida` E:637 | [D] ND | [E] `ato1_viu_torre` E:635 | [D] alto do container | [E] ao entrar no trigger E:635 |
| `city_s2_dara` D:206 | Dara, Lip, N | [C] `ato1_casa_resolvida` abre Q principal, não trava esta fala | [D] ND | [E] `ato1_falou_dara` E:811 | [D] balde; [C] fila do poço | [E] ao interagir E:811 |
| `city_s2_homem_fila` D:213 | Homem da fila, N | [C] contexto Q principal como acima | [D] ND | [E] `ato1_falou_fila` E:811 | [D] fila no ID; [C] poço | [E] ao interagir E:811 |
| `city_s2_tacio` D:219 | Tácio, N | [C] contexto Q principal como acima | [D] ND | [E] `ato1_falou_tacio` E:812 | [D] observa fila; [C] mercado | [E] por script via interação de loja E:812 |
| `city_s2_bruna` D:226 | Bruna | [D] ND | [D] ND | [D] ND | [D] Guilda citada; [C] galpão | [I] ao interagir |
| `city_s2_bruna_drone` D:230 | Bruna, N | [D] ND | [D] ND | [E] `ato1_falou_bruna` E:812 | [D] trabalho com metal; [C] galpão | [E] ao interagir E:812 |
| `city_s2_pipo` D:238 | Pipo | [D] ND; [C] quest própria exige `ato1_sessao2`, não esta fala | [D] ND | [D] ND | [D] sem posição; [C] beco das carcaças | [I] ao interagir |
| `city_s2_onofre` D:243 | Onofre | [D] ND | [D] ND | [D] ND | [D] cita casa de Ivo como destino, não sua posição; [C] pilar | [I] ao interagir |
| `city_s2_nilce` D:249 | Nilce | [D] ND | [D] ND | [D] ND | [D] hospedagem; [C] estalagem | [I] ao interagir |
| `city_s2_quirino` D:253 | Quirino | [D] ND | [D] ND | [D] ND | [D] sem posição; [C] oficina | [I] ao interagir |
| `city_s2_sula` D:259 | Sula, N | [D] ND | [D] ND | [D] ND | [D] praça citada em aviso; [C] barraca | [I] ao interagir |
| `city_s2_casa_ivo_cadernos` D:266 | Apollo, N | [D] exploração opcional; flags ND | [D] ND | [E] `ato1_viu_casa_ivo` no conjunto E:813; produtor ND | [D] casa de Ivo pelo ID/contexto | [I] ao interagir com cadernos |
| `city_s2_casa_ivo_foto` D:273 | N | [D] exploração opcional; flags ND | [D] ND | [E] mesma flag do conjunto, produtor ND | [D] parede da casa de Ivo | [I] ao interagir com foto |
| `city_s2_casa_ivo_bilhete` D:279 | Apollo, N | [D] exploração opcional; flags ND | [D] ND | [E] mesma flag do conjunto, produtor ND | [D] caderno sobre a cama | [I] ao interagir com bilhete |
| `city_s2_apollo_conclusao` D:287 | Apollo, Lip, N | [C] Q:77–86 exige conversar com Dara, fila e Tácio para concluir quest; vínculo ao gatilho ND | [D] ND | [E] `ato1_sessao2` E:814; [C] também saída de Q principal | [D] perto do portão leste, fim da tarde | [E] ao entrar no trigger E:814 |
| `city_s2_apollo_portao` D:301 | Apollo, Lip | [D] fala de técnica e DEX, sem limiar numérico; [E] gate em e | [D] ND | [E] nenhuma indicada E:815 | [D] portão no ID/texto; [E] leste | [I] por script, ao tentar sair bloqueado |
| `city_s3_sino` D:316 | Tácio, Apollo, Lip, N | [I] S2 concluída antes da S3; flag exata ND | [D] ND | [E] nenhuma indicada E:931 | [D] sino/cidade; [E] posto de vigia | [E] ao entrar no trigger na saída de casa E:931 |
| `city_s3_recusa_apollo` D:325 | Apollo, Lip, N | [I] depois do sino; flag de encadeamento ND | [D] ND | [E] `ato1_apollo_recusou` E:932 | [D] ameaça a leste; [E] antes do portão | [E] ao entrar no trigger, uma vez E:932 |
| `city_s7_agua` D:638 | Onofre, N | [E] `ato1_sessao6` E:1654 | [D] ND | [E] `ato1_agua_voltou` E:1654 | [D] rua, praça e bica | [E] trigger na praça E:1654 |
| `city_s7_quirino_perna` D:646 | Quirino, Apollo, Lip, N | [C] Q:231–239: S6 + servo + 3 sucatas + conversa; são requisitos da quest | [D] ND | [C] `quest_apollo_perna_ok` na entrega Q:246; [E] ligação ao diálogo E:1655 | [D] oficina | [E] por script na entrega da quest E:1655 |
| `city_s7_nadir_chegada` D:660 | Pipo, Nadir, N | [D] estação resolvida no contexto; [I] após retorno; flag exata ND | [D] ND | [E] `ato1_nadir` para conjunto chegada/oferta/assembleia E:1656; produtor ND | [D] lado de fora do portão, direção não dita | [E] ao entrar no trigger no portão E:1656 |
| `city_s7_nadir_oferta` D:668 | Nadir, N | [I] após chegada; flag ND | [D] ND | [E] conjunto de `ato1_nadir`, produtor ND | [D] leitura pública sem posição; [C] praça | [I] por script na sequência |
| `city_s7_assembleia` D:683 | Tácio, Bruna, N | [I] após oferta; flag ND | [D] ND | [E] conjunto de `ato1_nadir`, produtor ND | [D] ao redor do poço; Nilce na porta da estalagem | [I] por script na sequência |
| `city_s7_dara_fala` D:691 | Dara, N | [I] durante assembleia, antes da resposta; flag ND | [D] ND | [E] nenhuma indicada E:1658 | [D] praça | [E] gatilho na assembleia E:1658; [I] por script para ordenar |
| `city_s7_nadir_ferro` D:703 | Rasha, Nadir, N | [E] `ato1_faccao_ferro` E:1657/1823; [I] após fala de Dara | [E] outras rotas exclusivas E:1823–1825; impedimentos técnicos ND | [E] `ato1_cidade_respondeu` E:1657 | [D] batente da Guilda e assembleia | [I] por script, ramificação exclusiva |
| `city_s7_nadir_lotus` D:714 | Teodoro, Nadir, N | [E] `ato1_faccao_lotus`; [I] após fala de Dara | [E] outras rotas exclusivas; implementação ND | [E] `ato1_cidade_respondeu` E:1657 | [D] assembleia | [I] por script, ramificação exclusiva |
| `city_s7_nadir_desgarrado` D:724 | Bruna, Nadir, N | [E] `ato1_faccao_desgarrado` | [E] outras rotas exclusivas; implementação ND | [E] `ato1_cidade_respondeu` E:1657 | [D] praça, sem intervenção externa | [I] por script, ramificação exclusiva |
| `city_s7_dara_ferro` D:731 | Dara, N | [I] retorno da S6 + rota Ferro; origem do bloqueio ND | [D] ND | [D] ND | [D] entrada da praça | [I] ao interagir |
| `city_s7_dara_lotus` D:737 | Dara, N | [I] retorno da S6 + rota Lótus | [D] ND | [D] ND | [D] sem posição; [I] entrada da praça como variante | [I] ao interagir |
| `city_s7_dara_desgarrado` D:742 | Dara | [I] retorno da S6 + rota Desgarrado | [D] ND | [D] ND | [D] sem posição; [I] entrada da praça como variante | [I] ao interagir |
| `city_s7_apollo_revelacao` D:747 | Apollo, Lip, N | [I] depois do contrato; flags ND | [D] ND | [E] nenhuma indicada E:1659 | [D] soleira de casa, gerador | [E] ao entrar no trigger na porta E:1659 |
| `city_s7_mochila` D:769 | N | [I] após revelação; flags ND | [D] ND | [E] nenhuma indicada E:1660 | [D] mesa da cozinha, madrugada | [E] painel de itens + gatilho E:1660; [I] por script |
| `city_s7_dara_cantil` D:779 | Dara, N | [I] etapa da partida; flags ND | [D] ND | [E] conjunto da saída → `ato1_concluido` E:1661; produtor ND | [D] portão leste | [E] trigger no portão E:1661 |
| `city_s7_partida` D:784 | Lip, Apollo, N | [I] após mochila/cantil; flags ND | [D] ND | [E] conjunto da saída → `ato1_concluido`; [C] Q:130 também concede essa flag | [D] saída de Ferrovale ao amanhecer; direção ND | [I] por script na sequência de saída |
| `city_s7_onofre` D:798 | Onofre | [D] retorno da Cratera no contexto; [I] S6 como condição, não declarada | [D] ND | [D] ND | [D] sem posição; [C] pilar do viaduto | [I] ao interagir |

### Fichas de montagem — ler junto com a matriz

[C] Animações abreviadas: **A6** = `walk,hurt,shoot,slash,spellcast,thrust`; **A7** = A6 + `idle`; **A15** = A6 + `idle,backslash,climb,combat_idle,emote,halfslash,jump,run,sit`; **A14** = A15 sem `climb`. Arquivos `.png` em `animations/`, não Animator validado. Sprite completo = S + caminho da seção b. [I] Ordens sem encadeamento declarado continuam sugestões.

#### Apollo / Lip
- [C] Função: Apollo mentor/companheiro; [D] Lip é interlocutor e protagonista. [D] Locais e dez/nove falas conforme b/matriz.
- [I] Ordem: mirante → investigação opcional → conclusão S2 → gate → sino → recusa → reparo S7 → revelação → partida. [D] Mochila tem apenas narração. [I] Condições/flags: usar as distinções da matriz, sem atribuir flags a todos os eventos.
- [C] Rotina Apollo, N:119–123: madrugada em casa; manhã em casa acompanhando Lip; períodos sem entrada caem no local padrão. Lip não tem rotina em N.
- [C] Sprites: `Companion/Apollo/sprite.png` A15; `Protagonist/sprite.png` A15; variante com mochila A6. [I] O catálogo de rotina não é sistema de follow.
- [I] Montar: presença de Apollo nos eventos, posições de Lip e pontos de investigação; representação de soleira/cozinha sem escolher transição de cena; visual de reparo pendente.

#### Dara / `dara`
- [C] Função: moradora e ofertante de quests; local padrão `loc_poco` N:127–142. [D] S7: entrada da praça na Ferro, praça no discurso, portão leste no cantil.
- [I] Ordem: `city_s2_dara` → uma variante `city_s7_dara_{ferro,lotus,desgarrado}` → `city_s7_dara_fala` → `city_s7_dara_cantil`; posição das variantes antes/depois da assembleia ND. Condições/flags na matriz.
- [C] Rotina: madrugada/manhã poço; tarde mercado; noite casa. Quests: `q_agua_dara` exige `ato1_falou_dara` → `quest_dara_filtro_ok`; `q_dara_terceira` exige esta última → `quest_dara_terceira_ok` (Q:141–180).
- [C] Sprite `NPCs/Dara/sprite.png`, A15. [I] Montar: reposicionar teste, interações, balde vazio/cheio, posições do discurso e cantil; não transformar quest lateral em requisito do ato.

#### Homem da fila / `homem_fila`
- [C] Função genérica; poço N:165–180. [D] Único diálogo `city_s2_homem_fila`; condições/flag na matriz.
- [C] Rotina: manhã poço; tarde praça; demais períodos usam local padrão. Sprite `NPCs/Homem_da_Fila/sprite.png`, A15.
- [I] Montar: reposicionar teste na fila e vincular interação; pós-S6 não tem novo ID `city_` próprio, apesar da biografia.

#### Tácio / `tacio`
- [C] Loja no mercado N:145–162; [E] posto de vigia na S3 E:833. [I] Ordem: `city_s2_tacio` → `city_s3_sino` → `city_s7_assembleia`; requisitos/flags na matriz.
- [C] Rotina: manhã/tarde mercado; noite oficina com Quirino. Quests `q_cobre_tacio`: `ato1_falou_tacio` → `quest_tacio_cobre_ok`; `q_tacio_linha`: esta última → `quest_tacio_linha_ok` (Q:186–222).
- [C] Sprite `NPCs/Tacio/sprite.png`, A6. [I] Montar: reposicionar teste, conversa/serviço, posição no sino e assembleia; recepção pessoal de Nadir só em E:1495, não fala própria em D.

#### Bruna / `mestra_guilda`
- [C] Guilda, `loc_guilda` N:246–264. [I] Ordem: `city_s2_bruna` → `city_s2_bruna_drone` → `city_s7_assembleia` → `city_s7_nadir_desgarrado` somente nessa rota; ordem entre as duas primeiras ND.
- [C] Rotina: madrugada/manhã/noite Guilda; tarde ferro-velho. Quests: `q_limpeza_borda` exige S3; cadeia da Guilda começa em S2, gera `guilda_membro` → `guilda_cacada_ok` → `guilda_alfa_ok` → `guilda_verdade_ok` (Q:252–265,386–475).
- [C] Sprite `NPCs/Mestra_Bruna/sprite.png`, A6. [I] Montar: balcão/interação de Guilda e posição de assembleia; `PapelDeNpc.Guilda` cadastrado não comprova serviço configurado.

#### Nilce / `estalajadeira`
- [C] Estalagem N:267–285. [D] `city_s2_nilce`; presença silenciosa em `city_s7_assembleia` na porta, não novo diálogo de Nilce.
- [C] Rotina: manhã/noite estalagem; tarde mercado; madrugada corredor do segundo andar. `q_nilce_quarto` exige `ato1_sessao3` → `quest_nilce_ok` (Q:315–331).
- [C] Sprite `NPCs/Nilce/sprite.png`, A6. [I] Montar: interação de hospedagem e posição silenciosa visível da praça; não presumir que a quest tem fala escrita.

#### Quirino / `ferreiro`
- [C] Ferreiro, oficina N:288–305. [I] Ordem: `city_s2_quirino` → `city_s7_quirino_perna`; condições/flag na matriz e Q:228–246.
- [C] Rotina: manhã/tarde bancada/atendimento; noite dominó com Tácio; tudo na oficina. Sprite `NPCs/Quirino/sprite.png`, A6.
- [I] Montar: oficina/interação, entrega da quest e evento com Apollo; distinguir entrega/recompensa de simples conversa para não concluir reparo cedo.

#### Sula / `vendedora_pecas`
- [C] Loja, barraca N:308–324. [D] `city_s2_sula`; presença silenciosa em `city_s7_assembleia`.
- [C] Rotina: manhã/tarde barraca; noite beco. `q_sula_divida` exige `ato1_nadir` → `quest_sula_ok` (Q:356–373); diálogo específico da revelação da quest não existe em D.
- [C] Sprite `NPCs/Sula/sprite.png`, A6. [I] Montar: comércio/interação e presença na assembleia; não reaproveitar fala inicial como confissão sem decisão posterior.

#### Pipo / `crianca_rua`
- [C] Genérico, beco N:327–343. [I] Ordem: `city_s2_pipo` → presença na água → `city_s7_nadir_chegada`; condições na matriz.
- [C] Rotina: manhã beco; tarde mercado; noite barraca de Sula. `q_pipo_peca` exige `ato1_sessao2` → `quest_pipo_ok` (Q:271–287).
- [C] Sprite `NPCs/Pipo/sprite.png`, só `walk`. [I] Montar: interação, posição na bica e anúncio da comitiva; não exigir quest própria para o anúncio sem fonte.

#### Onofre / `velho_viaduto`
- [C] Informante no mercado/pilar N:346–363. [I] Ordem: `city_s2_onofre` → `city_s7_agua` → `city_s7_onofre`; ordem das duas últimas não contratada.
- [C] Rotina: manhã pilar leste; tarde pilar central; noite permanece/dorme no mercado. `q_onofre_provas` exige `ato1_faccao_escolhida`, objetivo `ato1_cratera_verdade` → `quest_onofre_ok` (Q:293–309).
- [C] Sprite `NPCs/Seu_Onofre/sprite.png`, A7. [I] Montar: caixote/pilar e falas de retorno; ouvir água não obriga deslocá-lo até a bica.

#### Nadir / `nadir`
- [D] Portão na chegada; leitura pública e assembleia; [C] informante, local padrão praça N:370–387.
- [I] Ordem: `city_s7_nadir_chegada` → `city_s7_nadir_oferta` → `city_s7_assembleia` → `city_s7_dara_fala` → exatamente uma variante `city_s7_nadir_{ferro,lotus,desgarrado}`. Condições/flags na matriz.
- [C] Rotina: tarde/noite praça; `FlagQueFazAparecer = ato1_nadir`. [I] Falta definir relação entre aparição da comitiva e flag concedida no conjunto da chegada; não assumir autoativação.
- [C] Sprite `Antagonists/Nadir_Quintela/sprite.png`, A6. [I] Montar: entrada, espera, posição de leitura, comitiva e encadeamento sem sobreposição de diálogos.

#### Rasha / Teodoro
- [D] Função: intervenientes de rota; `city_s7_nadir_ferro` começa com Rasha no batente da Guilda; `city_s7_nadir_lotus` traz Teodoro à assembleia.
- [C] IDs `rasha`/`teodoro`; local padrão borda da Cratera. Rotina Rasha: manhã borda, noite acampamento; Teodoro: manhã/noite acampamento, tarde borda (N:203–237). [I] Não importar essas rotinas como posições permanentes de City.
- [C] Sprites `NPCs/Rasha/sprite.png` A7 e `NPCs/Teodoro_Sampaio/sprite.png` A6. [I] Montar: aparições exclusivas conforme matriz; nenhum deles intervém na rota Desgarrado.

#### Ivo / Célia / figurantes
- [D] Ivo não fala nos blocos City; sua casa está vazia; `city_s7_dara_lotus` relata visita anterior dele, sem escrever encontro com Lip. [C] Ivo: `ivo`, rotina na estação/borda (N:183–200), sprite `Antagonists/Ivo_Rangel/sprite.png` A14.
- [D] `city_s2_casa_ivo_foto` é narração; [C] Célia não tem ID em N; sprite `NPCs/Celia_Ferraz_Rangel/sprite.png` A15 é referência para foto, não autorização de NPC vivo.
- [D] Chegada inclui dois carregadores e um droid desligado. [C] Sem IDs/rotinas em N; modelo `Antagonists/Carregador_Apex/sprite.png` A6; sem sprite de escolta identificado em S.
- [I] Montar: três examináveis da casa, foto composta pendente e comitiva; não escolher aparência individual do segundo carregador ou corpo da escolta neste guia.

#### Benta / Zeca
- [C] `benta`: loja/posto ao lado da estalagem; manhã/noite posto, tarde ferro-velho N:390–407. `zeca_cerca`: vigia norte; madrugada/tarde/noite portão, manhã estalagem N:410–427.
- [C] Sem sprite nominal identificado em S; sem flag de presença preenchida em N. [D] Zero diálogos `city_` próprios; [C] ambos têm texto de boas-vindas, que não é ID de D.
- [I] Montagem narrativa e visual em aberto; não criar falas, flags ou substituir Pipo/Tácio por eles. Divergências em g.

## e. Eventos de cena

### Portão e gate — S2.6
- [D] `city_s2_apollo_conclusao` perto do acesso leste; `city_s2_apollo_portao` explica preparação antes da travessia. [E] E:803 indica `ato1_sessao2`, técnica configurada e mínimo de duas ações no gate.
- [C] `q_main_agua` exige `ato1_casa_resolvida` e conversas com Dara, fila e Tácio; concede `ato1_sessao2` (Q:72–86). [E] E:814 também atribui essa flag ao comentário de Apollo. [I] Responsável pela concessão continua em aberto.
- [D] Participam Apollo/Lip; D não determina DEX mínima numérica. [I] Tipo sugerido: tentativa bloqueada chama fala por script; retorno à interação e liberação precisam de contrato posterior.

### Sino — S3.1 e recusa
- [D] `city_s3_sino`: Tácio, Apollo, Lip, moradores em reação; drone como ameaça narrada. [E] Tácio toca sino no posto; Pipo é recolhido da rua (E:833–839).
- [E] Saída de casa dispara sino; antes do portão dispara `city_s3_recusa_apollo`, grava `ato1_apollo_recusou` (E:931–932). [I] Não há flag própria para “sino concluído” nas fontes consultadas; encadeamento não deve ser inventado silenciosamente.
- [E] Continuidade S3.3: `game_s3_apollo_golpes` na chegada a Game, fora do inventário `city_` (E:933). [I] Local da prosa e fronteira técnica permanecem divergência em g.

### Retorno, chegada de Nadir e assembleia — S7
- [D] Água: Dara na bica, Pipo bebe, Onofre fala; reparo: Quirino/Apollo/Lip. [E] Água exige `ato1_sessao6` → `ato1_agua_voltou`; reparo é entrega da quest (E:1654–1655).
- [D] Chegada: Pipo anuncia; Nadir espera fora do portão com dois carregadores e escolta desligada. Oferta → assembleia ao redor do poço; Tácio/Bruna falam, Nilce observa da porta e Sula está presente.
- [D] Dara discursa e sai; Rasha intervém da Guilda na Ferro, Teodoro chega na Lótus, Bruna/Nadir encerram no Desgarrado. [E] Rotas exclusivas geram `ato1_cidade_respondeu` (E:1657). [I] Momento de gravar `ato1_nadir` dentro do grupo de chegada é ND.
- [C] `q_main_partida` exige S6, água de volta, conversa com Nadir e resposta da cidade; concede `ato1_concluido` (Q:112–130). [E] E:1661 coloca conclusão na saída; não antecipar nem escolher entre fontes sem registrar o conflito.
- [D] Depois há revelação na soleira, mochila na cozinha, cantil no portão leste e partida. [I] Ordem técnica das variantes de Dara, sincronismo do reparo e momento de transição final seguem abertos.

## f. Ordem de montagem sugerida — dependências, não decisões novas

1. [I] Delimitar futuro recorte autorizado; resolver apenas divergências bloqueadoras. Reposicionar testes existentes sem inferir posições a partir deles. Não alterar Canvas/Scaler ou outras cenas por consequência deste guia.
2. [C] Preparar locais/identidades e sprites da seção b; [I] preservar IDs e referências, tratar rotinas como dado atual, não implementar relógio/movimentação sem escopo próprio.
3. [D] S1: mirante; [E] depende da casa resolvida e marca visita à torre. [I] Não inventar versões pré-crise de `city_s2_*` (g).
4. [D] S2: conversas, três examináveis da casa de Ivo, conclusão e gate. [C] Vincular objetivos da quest aos IDs dos NPCs é requisito distinto de reproduzir a fala; produtor de S2 está em aberto.
5. [D] S3: sino → recusa; [E] conexão com explicação em Game. [I] Só configurar passagem quando encadeamento e condições de repetição estiverem definidos; não criar flags por conveniência.
6. [C] Entrada externa da S7: `ato1_sessao6`; [D] água e reparo antes do desfecho afetivo de Apollo. [I] Condições de entrega e disponibilidade de materiais precisam permanecer consistentes com Q.
7. [D] Chegada → oferta → assembleia → fala de Dara → variante política exclusiva; [E] registrar resposta da cidade. [I] Definir produtor de `ato1_nadir` antes de ligar aparição e evento para evitar dependência circular.
8. [D] Revelação → mochila → cantil/partida; [I] não finalizar ato pela quest antes da sequência sem decisão posterior. Conversas de Dara/Onofre têm janela exata ND.
9. [I] Validar futuramente o recorte com input e comparação visual: acessibilidade, legibilidade, ordem, ausência de disparos simultâneos e persistência. Este guia não registra esses testes como executados.

## g. Divergências — versões lado a lado, sem conciliação das fontes

[I] A coluna esquerda registra a base aplicável pela hierarquia solicitada; isso não é veredito editorial nem autorização para corrigir a coluna direita. Fontes completas e abreviações em a.

| Assunto | Diálogo / dado de maior prioridade | Outra versão / lacuna |
|---|---|---|
| Portão | [D] leste em D:288 e D:780 | [C] norte em L:304/376 e N:414; [I] não há correspondência aprovada entre IDs/objetos |
| Descoberta de Nadir | [D] Pipo anuncia D:661 | [C] rotina de Zeca atribui descoberta a ele N:427; [E] primeiro observador é Pipo E:1489 |
| S3.3 | [D] ID `game_s3_apollo_golpes` D:75; bloco D:341 | [E] prosa no caminho ao portão E:883–901, nota manda chegada em Game E:933; D não fixa coordenada |
| Revelação de Apollo | [D] rastreamento por seu número específico D:747–755 | [E] qualquer um dos 431 núcleos A:29–35; [I] manter ID/conteúdo de D como base deste guia, sem reescrever |
| Mirante | [D] alto do container e luz pulsante D:190–200 | [C] tanque elevado, luz que não pisca L:343–345; [E] três momentos E:245, enquanto L:356 lista quatro sessões |
| S1 pré-crise | [D] `city_s2_dara` já trata corte da cota D:206–211 | [E] E:634 pede variantes pré-crise `city_s2_*`; [I] não existem IDs separados para essas versões |
| Conclusão S2 | [C] Q:81–86 pede Dara + fila + Tácio e concede `ato1_sessao2` | [E] E:814 atribui flag ao comentário; [D] D:287 não define objetivos nem flag; [I] produtor não conciliado |
| Final do ato | [C] Q:117–130 conclui com água, Nadir e resposta; concede `ato1_concluido` | [E] saída/cantil/partida em E:1661; [D] ainda há revelação/mochila/saída D:747–797 |
| Nadir / props | [D] caderneta D:662/669 | [C] tablet e cabelo preso N:378–379; [C] visual selecionado com bob em S/Documentation/sprite-mapping.md:29 |
| Dara / props | [D] um balde nas duas mãos D:207; cheio no discurso D:692 | [C] dois galões, cabelo curto N:132–133; visual selecionado com rabo de cavalo em S/Documentation/sprite-mapping.md:35 |
| Oficina na S7 | [D] reparo e oficina D:646–658 | [C] sessões de importância da oficina listam 2,3,5 L:184; não incluem o reparo S7 |
| Casa na sequência City | [D] soleira/cozinha em IDs `city_s7_*` D:748/770 | [C] `loc_casa_lip` L:90 não é nome de cena; [E] sequência em City E:1856; [I] representação/transição não decidida |

## h. Lacunas e limites para a próxima sessão

- [D] D não fornece flags impeditivas, coordenadas, condições de repetição ou escolha de componentes. [I] A matriz não autoriza substituir ND por valores arbitrários.
- [C] N:449–470 só consulta rotina/local; `FlagQueFazAparecer`/`FlagQueFazSumir` são campos N:578–581, não prova de consumidor. [I] Aparição de Nadir e intervenientes exige contrato de montagem próprio.
- [C] Benta/Zeca têm local, biografia e boas-vindas; [D] sem bloco próprio. [I] Não inventar função no sino/chegada para preencher essa falta.
- [C] Quests laterais de Dara, Tácio, Bruna, Nilce, Pipo, Onofre e Sula existem; [D] faltam IDs de diálogos específicos para várias aceitações/entregas/revelações. [I] Não confundir boas-vindas com essas conversas.
- [D] `city_s7_dara_lotus` relata que Ivo veio; não escreve encontro presencial com Lip. [I] Não adicionar Ivo permanente em City por esse relato.
- [C] L não tem local próprio para a casa de Ivo; [D] três examináveis escritos. [I] Sala/acesso, item do bilhete e flag de entrada precisam de vínculo definido, sem novo ID de local nesta entrega.
- [D] Pipo e figurantes não falam em todos os eventos em que aparecem. [I] Presença física, narração e fala são registros diferentes; contagem da seção b não mede aparições.
- [C] Personagens possuem PNGs, mas isso não comprova recorte/importação/Animator/objeto. Props e emblemas constam pendentes em S/Documentation/character-catalog.md:74–77; Apollo é placeholder. [I] Aprovação visual não realizada.
- [I] Último resultado desta tarefa: guia documental; Unity não consultado, estado Play/cena/dirty desconhecido; nenhum backup, save ou asset foi acessado para mutação. Próximo passo: escolher recorte futuro e resolver suas lacunas bloqueadoras.

### Sugestões de atualização de outros documentos — não aplicadas

- [I] `docs/indice-trabalho.md`: vincular este guia e atualizar o caminho antigo do catálogo visual.
- [I] `docs/guias/montagem-ato-1.md`: referenciar a matriz, separar presença/diálogo/quest e registrar responsáveis ainda indefinidos pelas flags de S2/S7.
- [I] `docs/plano-desenvolvimento.md`: registrar recortes de montagem, props/robôs pendentes e contratos de encadeamento, sem converter sugestões em implementação concluída.
- [I] `S/Documentation/character-catalog.md`: corrigir caminhos antigos `Assets/Characters/...` e esclarecer que “não montados” se refere à produção visual, não à auditoria de cenas.
- [I] `N`, `L`, `Q`, `D`, enredo/adendo: registrar futura conciliação das divergências de g somente em tarefa autorizada; nenhuma alteração é proposta como automática por este guia.
