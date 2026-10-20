# Cobertura de animações e verificações

Sheet padrão LPC: 832×3456 px, quadros 64×64, linhas up/left/down/right. Antes de tudo: nada foi comparado pixel a pixel com a exportação do site (inacessível aqui); a montagem usa um compositor próprio que replica o renderer do repositório.

| Personagem | walk | idle | hurt | slash | thrust | spellcast | shoot | run | combat_idle | extras | sheet | sprite | altura parado (px) | magenta restante | quadros vazios |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Felipe "Lip" Andrade | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | climb, jump, sit, emote, backslash, halfslash | 832×3456 | 64×64 | 49 | 0 | nenhum |
| Lip (variante com mochila) | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 49 | 0 | nenhum |
| Apollo (placeholder LPC) | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | climb, jump, sit, emote, backslash, halfslash | 832×3456 | 64×64 | 49 | 0 | nenhum |
| Ivo Rangel | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | jump, sit, emote, backslash, halfslash | 832×3456 | 64×64 | 47 | 0 | nenhum |
| Nadir Quintela | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 50 | 0 | nenhum |
| Carregador da Apex (figurante) | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 47 | 0 | nenhum |
| Dara | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | climb, jump, sit, emote, backslash, halfslash | 832×3456 | 64×64 | 48 | 0 | nenhum |
| Rasha | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 48 | 0 | nenhum |
| Teodoro Sampaio | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 48 | 0 | nenhum |
| Senhor Tácio | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 47 | 0 | nenhum |
| Mestra Bruna | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 49 | 0 | nenhum |
| Nilce | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 50 | 0 | nenhum |
| Quirino | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 47 | 0 | nenhum |
| Sula | ✔ | — | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 47 | 0 | nenhum |
| Pipo | ✔ | — | — | — | — | — | — | — | — | — | 832×3456 | 64×64 | 42 | 0 | nenhum |
| Seu Onofre | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | — | — | — | 832×3456 | 64×64 | 47 | 0 | nenhum |
| Homem da Fila (figurante) | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | climb, jump, sit, emote, backslash, halfslash | 832×3456 | 64×64 | 49 | 0 | nenhum |
| Célia Ferraz Rangel (referência de foto) | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | climb, jump, sit, emote, backslash, halfslash | 832×3456 | 64×64 | 48 | 0 | nenhum |

Não foi criada nenhuma animação nova; animações marcadas com — não existem para a combinação de peças escolhida. Onde falta `idle`, a pose parada é o quadro 0 de `walk`. Para combate por turnos use `combat_idle` quando houver, senão `idle`/`walk`.

Direções: apenas `hurt` e `climb` têm uma única direção.
