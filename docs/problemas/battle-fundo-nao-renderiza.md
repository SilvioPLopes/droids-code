# Battle: fundo n?o renderiza como SpriteRenderer

**ID:** VIS-001.  
**Status:** contornado conforme relato hist?rico; causa raiz n?o identificada.  
**Revis?o documental:** 16/10/2026. N?o houve nova reprodu??o nesta consolida??o.  
**Cena:** [Battle.unity](../../Assets/Scenes/Battle.unity).

## Comportamento relatado

Durante Play, a UI aparecia, mas o fundo `battle_0` n?o era desenhado como SpriteRenderer. A investiga??o incluiu posi??o, escala, pivot, material, c?mera, culling e pipeline. Um SpriteRenderer novo tamb?m n?o apareceu naquela sess?o. As evid?ncias e ressalvas completas est?o no [relato original preservado](../historico/2026-10-16-contexto-anterior/bug-battle-sprite-background.md).

A data original dessa investiga??o n?o est? informada no arquivo; 16/10 ? a data desta revis?o, n?o uma nova valida??o visual.

## Contorno aplicado

O respons?vel usou uma UI.Image no Canvas para o mapa e ajustou a ordem visual com o inimigo. O resultado foi relatado como funcional. O levantamento posterior da cena tamb?m descreve um objeto Mapa na UI.

Esse contorno usa RectTransform e ordem da hierarquia do Canvas. N?o demonstra que a causa do problema de SpriteRenderer foi corrigida, nem que qualquer outra cena esteja afetada.

## Hip?teses ainda abertas

A investiga??o original termina com compara??o de Frame Debugger entre cenas e verifica??o de poss?veis mudan?as em c?mera/componentes durante execu??o. Alguns trechos das hip?teses foram parcialmente descartados no pr?prio hist?rico; n?o devem ser tratados como diagn?stico fechado de Render Graph ou Renderer Feature.

## Pr?xima valida??o

Executar VAL-11 do [roteiro](../guias/validacao-jogo.md): confirmar fundo/inimigo vis?veis, ordem e escala em resolu??es-alvo. Se o contorno atende ? apresenta??o aprovada, mant?-lo documentado. Reabrir investiga??o de causa raiz quando houver necessidade real de SpriteRenderer nessa cena ou regress?o observada.

**Encerramento:** registrar se a apresenta??o foi aceita com o contorno ou se uma causa raiz foi demonstrada e corrigida. Apar?ncia funcional e causa resolvida s?o resultados distintos.
