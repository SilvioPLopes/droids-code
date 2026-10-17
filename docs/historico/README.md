# Histórico e migração da documentação

**Consolidação:** 16/10/2026. O estado vigente está no [contexto](../contexto-projeto.md). Estes registros preservam o que foi escrito em cada entrega; valores, caminhos, status e instruções podem estar superados.

## Registros preservados

| Arquivo | Origem e uso |
|---|---|
| [Bugs da CasaLip, 14/09](2026-09-14-bugs-casalip.md) | Original de `bugs de desenvolvimento/BUG_PUZZLE_AGUA_CASALIP.md`. Mantém duas resoluções relatadas e uma pendência secundária do log. O problema atual de retorno tem relato próprio. |
| [Entrega do Ato 1, 15/09](2026-09-15-entrega-ato-1.md) | Original de LEIA_ISTO_PRIMEIRO.md, com instruções daquela entrega. |
| [Análise factual, 14/10](2026-10-14-analise-projeto-unity.md) | Original de ANALISE_PROJETO_UNITY.md. Fotografia de leitura estática, com limitações de resolução de prefabs e sem compilação executada. |
| [README anterior](2026-10-16-contexto-anterior/readme.md) | Cópia integral anterior à simplificação da entrada do projeto. |
| [Documentação técnica anterior](2026-10-16-contexto-anterior/documentacao-tecnica.md) | Arquitetura, instruções de sessões antigas e notas de reconciliação originais. |
| [Checklist anterior](2026-10-16-contexto-anterior/checklist-desenvolvimento.md) | Planejamento anterior, preservando status e decisões daquelas sessões. |
| [Guia anterior](2026-10-16-contexto-anterior/guia-montagem-ato-1.md) | Montagem da versão anterior, preservada para consulta. |
| [Investigação original do fundo de Battle](2026-10-16-contexto-anterior/bug-battle-sprite-background.md) | Relato integral antes da criação do resumo ativo do problema. |

O corpo desses oito arquivos foi preservado sem reescrita. A pasta `2026-10-16-contexto-anterior` indica a data de arquivamento, não a data de criação ou validação de todas as suas afirmações. Referências textuais antigas foram mantidas como parte do registro e podem apontar para arquivos ausentes.

## Mapa da documentação ativa

| Nome anterior | Referência atual |
|---|---|
| readme.md | [README.md](../../README.md) |
| DOCUMENTACAO_TECNICA.md | [contexto-projeto.md](../contexto-projeto.md) |
| CHECKLIST_DE_DESENVOLVIMENTO.md | [plano-desenvolvimento.md](../plano-desenvolvimento.md) |
| GUIA_DE_MONTAGEM_ATO1.md | [montagem-ato-1.md](../guias/montagem-ato-1.md) |
| Bugs da casa em um único arquivo | [Retorno à casa](../problemas/casalip-retorno-porta-trancada.md) e [log do terminal](../problemas/terminal-log-painel-inativo.md), com o relato resolvido em setembro arquivado acima. |
| bug_battle_sprite_background.md | [Fundo de Battle](../problemas/battle-fundo-nao-renderiza.md) |

## Divergências reconciliadas

- Caminhos atuais de Scripts foram conferidos; pastas históricas World/UI/Salvamento não foram apresentadas como existentes.
- HIT/FLEE/crítico e as constantes de combate passaram a refletir os arquivos locais.
- EscreverNoLog já existe no TerminalUIManager; a antiga ausência foi retirada das tarefas abertas.
- O catálogo atual tem 18 quests, 16 personagens e 19 locais; catálogos prontos não foram equiparados a cenas prontas.
- VerificadorDePuzzle v2 foi documentado com fallback manual; a casa continua usando o checker antigo.
- ItemConsumivel foi classificado como legado; equipamentos não serializados e autosave em Battle foram registrados como lacunas atuais.
- Os itens de facção já existem no catálogo; as antigas instruções de colar um pacote não são pré-requisito vigente.
- Rotinas de NPC foram identificadas como dados sem controlador de movimentação.
- O travamento do Player foi mantido como não reproduzido. A observação de porta/referências perdidas recebeu registro separado.
- O arquivo LICENSE não foi localizado; a declaração MIT anterior ficou pendente de confirmação.

## Referências indisponíveis

Os registros citam `PARA_COLAR_CatalogoDeItens.md`, `LEIA_ISTO_OS_10_ARQUIVOS_NOVOS.md`, documentos `TCC_-_ENREDO*`/GDD e roteiros anteriores. Eles não foram encontrados no inventário Markdown desta consolidação. Os guias ativos não dependem de baixar ou colar esses arquivos. A revisão de narrativa permanece pendente de uma fonte de cânone disponível.

## Limites desta migração

A refatoração documental não corrigiu gameplay nem alterou scripts, cenas, prefabs ou configuração. As alterações locais anteriores em CasaLip e configurações foram preservadas na nova branch. A validação documental confere caminhos, links, inventário e integridade dos arquivos protegidos; os testes de jogo permanecem registrados no [roteiro](../guias/validacao-jogo.md).

## Referencia anterior a hierarquia de 17/10

[Contexto amplo preservado](2026-10-17-contexto-antes-hierarquia.md): inventario de scripts, formulas e evidencias datadas. Para trabalho atual, seguir o [indice](../indice-trabalho.md) e o plano indicado nele.
