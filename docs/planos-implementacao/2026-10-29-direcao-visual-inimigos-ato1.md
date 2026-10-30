# Direção visual dos inimigos do Ato 1

**Estado e data:** 29/10/2026; v1 rejeitado como chefe e preservado como Droid C. Conceito pesado v2 gerado por pedido explícito, entregue para avaliação visual; sem integração.

**Continuação funcional autorizada:** [evento do boss no vale](2026-10-29-evento-boss-vale.md) prepara combate e recurso da arte v2 sem posicionar evento ou editar cenas. O plano vinculado guarda o estado dessa implementação; a geração aqui não implica aprovação visual em jogo.
**Objetivo:** confrontar enredo/adendo, assets e processo existente; propor o elenco visual do chefe e inimigos menores e deixar um guia reutilizável para gerar suas imagens.
**Arquivos permitidos:** este plano novo, `docs/guias/direcao-arte-robos-e-inimigos.md` novo e um link de navegação em `docs/indice-trabalho.md`.
**Preservar:** enredo/adendo, Assets, cenas, prefabs, scripts, GUIDs, trabalho local existente, ferramentas de produção e saves. Nenhuma operação no Unity.
**Dependências e consumidores:** catálogo de inimigos define IDs; `ApresentacaoInimigoBattle` e `VisualInimigoBattle.prefab` selecionam imagens da Battle. Futuras sprites do mundo terão consumidores/pivôs próprios. Guia não muda esses contratos.
**Fontes necessárias:** enredo VI.3–VI.6 e regras III.9; adendo A e C; catálogo e apresentação reais; guia LPC; guia de interfaces; imagens citadas no guia novo. Consulta dirigida, sem revisão literária integral ou reescrita do enredo.
**Aceite:** identificar chefe, linguagem visual, cobertura existente e faltante; separar cânone de sugestões; oferecer ficha e prompts reproduzíveis, sem apresentar conceito como asset jogável.
**Validação mínima:** abrir referências visuais reais; conferir IDs e associações serializadas em disco; revisar links e diff documental. Sem Play, input real ou teste de save.

## Escopo narrativo e proposta

O chefe do Ato 1 é `droid_do_ivo`, nome de catálogo **Droid Improvisado**, na Estação Sete (sessão 6). A descrição exige corpo industrial remendado com placas da estação, dois braços hidráulicos de manutenção com lâminas adaptadas e três lentes de leitura precisas e limpas. É um escâner tornado perigoso por uma instrução sem limites. Ivo permanece personagem humano; não vira monstro. O adendo prevalece: Apollo não é um escolhido único; Artemis não recebe corpo nem luta física; Nadir não vira chefe.

**Limite da palavra demo:** o pedido atual menciona um boss da demo. Este plano prepara Ivo como chefe narrativo do Ato 1. O percurso proposto em [DEM-01](melhorias/05-mecanicas-e-demo.md#dem-01--montar-uma-fatia-completa-e-verificável) é uma fatia menor com Dara e um encontro, sem exigir chegar à Estação Sete. Incluir Ivo nessa fatia é uma decisão de escopo ainda não tomada; a revisão não altera DEM-01.

**Minha proposta de design, ainda não aprovada:** começar por um conceito estático do chefe em três quartos, voltado à esquerda, que contraste corpo improvisado e óptica de precisão. Preferir postura alta, irregular, com braços de manutenção longos; evitar virar uma Sucata Reforçada maior. Depois produzir Droid Selvagem e Drone de Reconhecimento. Sucatas A/B já disponíveis compõem a força de captura; selvagens compõem a fauna independente da Cratera. Esses menores não são automaticamente lacaios do Ivo.

Não há convocação de minions na luta da sessão 6 descrita no trecho consultado. Acrescentar ajudantes ao encontro mudaria roteiro e combate. A opção mais econômica é usar inimigos menores no percurso e preservar o duelo contra o escâner. Alfa e Carcaça podem completar o bestiário em lote posterior, sem criar IDs novos.

## Cobertura observada em disco

| ID | Evidência atual | Falta para a proposta |
|---|---|---|
| `sucata_captura` | PNG v1 e associação na Battle e prefab | Animação e representação própria no mundo não verificadas nesta revisão |
| `sucata_reforcada` | PNGs v1/v2; associação aponta para v2 em ambos | Visual v2 no Editor depende da retomada anterior; não usar captura antiga da v1 para atestá-lo |
| `droid_do_ivo` | Catálogo, atributos, chefe/único e fuga proibida | Arte própria não encontrada na pasta ativa de inimigos; sem associação na lista visual consultada |
| `droid_selvagem` | Catálogo e papel narrativo na Cratera | Arte própria e vínculo visual não encontrados nos mesmos alvos |
| `drone_reconhecimento` | Catálogo e papel narrativo de vigilância | Arte própria e vínculo visual não encontrados nos mesmos alvos |
| `droid_selvagem_alfa`, `carcaca_errante` | Catálogo | Produção visual posterior; mesma limitação de busca |

Não foi feita varredura de todos os arquivos do repositório para provar inexistência absoluta. A apresentação atual usa `legado` para IDs sem arte mapeada; cadastro não garante representação correta. Neste projeto os entregáveis observados são sprites 2D, não modelos 3D.

## Lacunas que a arte não resolve

`BattleManager` cria `InimigoFixo`; `InimigoFixo.ObterAcoesDisponiveis()` oferece uma ação. A consulta a esses arquivos e `CombatEngine` não encontrou a resistência crescente ao último golpe descrita na sessão 6. Não considerar a adaptação implementada por existir `EhChefe`. Também não foi comprovada a encenação da leitura profunda/perda de memória no meio da luta. São tarefas futuras de comportamento, fora desta revisão.

O render histórico de Battle aberto nesta rodada mostra o Apollo provisório em linguagem LPC e inimigo com pixelização muito mais detalhada. Preservar os sinais atuais de materiais/paleta não obriga perpetuar essa diferença de acabamento: comparar tudo no tamanho real antes de aprovar uma nova família. O prompt antigo solicita grade lógica 64×64, mas as imagens abertas não demonstram essa grade. Não documentar a intenção como propriedade técnica comprovada.

## Execução e retomada

### Conceito pesado v2 — nova geração autorizada

Usuário pediu explicitamente gerar a imagem após a proposta da máquina pesada de contenção e reforçou que deve ser assustador. Permitidos: novos `boss-conceito-v2.png`, `prompt-v2.txt`, `character-config-v2.md` e `verificacao-v2.json` na pasta de produção de `droid_do_ivo`, mais atualização deste plano. Preservar v1/Droid C, A/B, Assets, cenas, prefabs, atributos e saves. Arquivos v2 não existiam antes; reversão por remover somente novos arquivos desta etapa, se solicitada. Consumidores atuais não mudam; sem integração/Unity.

Hipótese autorizada: silhueta pesada de contenção, tronco hidráulico largo, cabeça recuada, braços maciços com lâminas improvisadas, pernas curtas e pés largos, materiais escuros e três lentes de precisão. Referência B/v2 para acabamento e peso mínimo a superar; C apenas como contraste de silhueta a evitar. Aceite visual: ameaça e identidade de boss perceptíveis sem depender de escala, mantendo três lentes, dois braços e corpo completo voltado à esquerda. Inspeção da imagem e alpha amostrado constituem a validação desta etapa; medo/carismo exigem avaliação do usuário, sem declaração antecipada de sucesso.

**Entregue nesta etapa:** [boss-conceito-v2.png](../../output/inimigos-ato1/2026-10-29/droid_do_ivo/boss-conceito-v2.png), gerado uma vez por ImageGen integrado, copiado sem edição; [ficha e proveniência](../../output/inimigos-ato1/2026-10-29/droid_do_ivo/character-config-v2.md) e prompt exato preservados. PNG 1312×1199 ARGB, cantos alpha zero, SHA256 CB7DD87BE26BB89F204AF749047C719047A5491F8AD0F1DA10A944223C92F46F. Inspeção visual da saída confirma volume maior de tronco/braços, cabeça recuada e três lentes; figura completa, margens estreitas nas extremidades. Medo/carismo e escolha final pendentes do usuário. Próximo passo vigente: avaliação visual da v2, antes de integrar ou gerar poses. Unity não operado, sem mudança em Assets/stats/saves; originais e Droid C preservados, sem limpeza pendente.

### Avaliação do v1 e reaproveitamento como Droid C

**Evidência do usuário:** arte muito parecida com Droid A, magra e sem firmeza; até Droid B transmite mais medo. Gostou da imagem como inimigo comum, mas rejeitou sua identidade de primeiro boss. Aumentar o tamanho não resolve a falta de robustez, carisma e ameaça. Deseja chefe mais parrudo, assustador e sombrio. Isso supera a proposta anterior de corpo estreito/alto como solução de chefe.

**Diagnóstico:** a geração cumpriu os elementos anatômicos, mas preservou a linguagem corporal estreita de A; tratar três lentes e lâminas como identidade suficiente foi uma escolha inadequada para o aceite visual. Hipótese para o redesenho: massa estrutural, centro de gravidade baixo, postura de contenção e óptica protegida devem comunicar ameaça mesmo em miniatura. Ainda não comprovada por imagem; não gerar segunda tentativa por suposição nesta rodada.

**Escopo autorizado antes da edição:** preservar conceito v1, prompt e proveniência; copiar a mesma imagem como novo `Assets/sprites/Entities/Enemies/Battle/DroidC-v1.png` e criar `.meta` próprio a partir das configurações de importação da Sucata A, sem reutilizar GUID; ficha nova em `output/inimigos-ato1/2026-10-29/droid_c/`; atualizar este plano, guia e classificação da ficha original. `Droid C` é rótulo de produção, não ID novo do catálogo. Atributos ficam para o usuário; não alterar catálogo, lista visual, cenas ou prefabs. Consumidor novo ainda não associado. PNG/meta de C não existiam antes; reversão por remover somente esses arquivos novos e desfazer os trechos desta rodada, se solicitado. Preservar todas as outras alterações locais.

**Aceite mínimo:** cópia C idêntica ao v1 por SHA256, origem e avaliação rastreáveis, meta com GUID próprio; nenhuma atribuição a `droid_do_ivo` introduzida. Importação e aparência no Unity permanecem pendentes. Próximo boss deve ser distinto de A/C e transmitir mais peso e ameaça que B sem depender de escala.

**Ajuste da preparação antes da cópia:** meta de A contém registros antigos de recortes apesar de Sprite Single. Usar como gabarito o meta limpo de B/v2 (Sprite Single, listas de recortes vazias), gerando GUID e spriteID próprios para C. Não alterar o meta de A ou B.

**Proposta de identidade para discussão:** máquina pesada de contenção da Estação Sete. Tronco como bloco de potência hidráulica, ombros integrados à estrutura, pescoço curto e cabeça recuada; três lentes em um nicho óptico limpo e protegido. Dois braços de manutenção maciços com lâminas de chapa improvisadas, juntas grossas, pernas curtas e pés muito largos. Massa de aço escuro e desgaste concentrado nas bordas; evitar tubos finos dominando a silhueta. As três lentes preservam o cânone; forma geral comunica uma máquina que pode esmagar/prender Apollo. Desenho ainda não gerado ou aprovado.

**Entregue e verificado nesta reclassificação:** PNG de C copiado para Assets com SHA256 idêntico ao original; meta novo com GUID `b102bdea7a614538b47200427e8196c2`, Sprite Single e recortes vazios, derivado do gabarito B/v2. [Ficha de C](../../output/inimigos-ato1/2026-10-29/droid_c/character-config-v1.md) e verificação de cópia preservam proveniência. Guia e ficha original corrigidos para não indicar aprovação como chefe. Sem nova geração, alteração de pixels, estatísticas, código, cenas ou prefabs. Unity não operado; importação e apresentação em jogo pendentes. Cadastro e atributos de C não concluídos, conforme reserva do usuário para ajustar status. Nenhum save/backup existente alterado ou limpeza criada.

### Produção autorizada do conceito do chefe — 29/10/2026

Usuário aprovou seguir com a criação da arte do boss. Escopo desta etapa: uma imagem estática de corpo inteiro, transparente, conforme a ficha aprovada, sem integração. Arquivos permitidos adicionais: arquivos novos em `output/inimigos-ato1/2026-10-29/droid_do_ivo/` (PNG, prompt e ficha/proveniência). Consumidores atuais permanecem intactos. Aceite mínimo: exatamente três lentes visíveis, dois braços hidráulicos com lâminas improvisadas, corpo industrial remendado distinto das Sucatas, figura completa voltada à esquerda e fundo transparente. Comparar com Sucata A/v1 e B/v2 como referências de material/acabamento. Aprovação visual final pertence ao usuário. Antes da mutação: pasta de produção nova, sem valores existentes a substituir; reversão por remoção somente dos arquivos novos desta etapa, se solicitada. Nenhum backup de cena/save necessário; Unity não será operado.

**Resultado desta etapa:** conceito v1 produzido por ImageGen integrado e copiado sem edição para [boss-conceito-v1.png](../../output/inimigos-ato1/2026-10-29/droid_do_ivo/boss-conceito-v1.png). Prompt exato, referências e origem em [ficha de produção](../../output/inimigos-ato1/2026-10-29/droid_do_ivo/character-config-v1.md). PNG 1312×1199 ARGB, cantos transparentes amostrados, hash registrado. Inspeção visual confirma três lentes triangulares, braços hidráulicos com lâminas e corpo completo orientado à esquerda. Margens nas extremidades são estreitas; não houve avaliação no enquadramento da Battle. Arte estática entregue para avaliação do usuário; sem importação/integração, Play, animações ou alteração de saves. Unity continua não operado; original do gerador preservado, sem limpeza pendente. Próximo passo desta etapa: avaliação visual do conceito v1 pelo usuário antes de preparar integração ou poses.

- **Antes da edição:** plano e guia não existiam; índice já continha alterações alheias. Reversão: remover apenas os dois arquivos novos e a linha de navegação desta tarefa, se solicitado, preservando todas as demais mudanças. Não foi criado backup de Assets porque nenhum asset será alterado.
- **Aplicado:** guia de direção visual, ficha de produção e prompts de proposta; plano com inventário e limites; link no índice.
- **Último resultado:** leitura de código/YAML e inspeção visual direta dos PNGs em 29/10/2026. Referências específicas constam no guia. Isso não é teste em execução.
- **Verificação documental:** todos os destinos locais dos links dos dois documentos novos existem; nenhum espaço em fim de linha; `git diff --check -- docs/indice-trabalho.md` sem erro. Índice já modificado antes da tarefa recebeu somente uma linha adicional. Não houve teste de comportamento ou nova captura de Unity.
- **Aprovação do usuário:** v1 rejeitado como chefe e aceito para reaproveitamento como terceiro inimigo comum. Nenhuma aprovação de visual em cena.
- **Próximo passo:** usuário avaliar o conceito pesado v2 entregue acima. Cadastro/atributos do inimigo C e associação em jogo permanecem pendentes; atributos reservados ao usuário. Manter IDs existentes.
- **Unity/save/backups:** Unity não consultado ou operado nesta tarefa; projeto/cena/Play atuais não confirmados. Saves e backups anteriores intocados; nenhuma limpeza pendente criada.
