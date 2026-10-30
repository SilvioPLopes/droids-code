# Boss — Droid Improvisado de Ivo

<img src="https://raw.githubusercontent.com/wiki/SilvioPLopes/droids-code/images/boss-ivo.png" alt="Arte do Droid Improvisado associado a Ivo" width="420">

**Nome no catálogo:** Droid Improvisado · **ID:** `droid_do_ivo` · **Classe de comportamento:** `DroidDoIvo`.

**Spoilers de combate e do conflito de Ivo.** A imagem é a arte existente do recurso do boss, não o retrato de Ivo humano.

## O que a construção revela

O chefe usa partes industriais antigas e placas reaproveitadas da estação. Não foi concebido como produto militar completo. O projeto apresenta a máquina como um resultado do desespero: alguém quer conter o que ameaça a estação e aceita construir sem as salvaguardas de um projeto estável.

O papel narrativo pertence ao encontro com Ivo na Estação 7. O desenvolvimento também preparou um evento para posicionamento no vale. Preparar um prefab e um comportamento não comprova que o confronto esteja colocado e acessível pela rota normal.

## Ficha atual do catálogo

| Atributo | Valor |
|---|---:|
| HP máximo | 200 |
| Ataque base | 18 |
| Defesa | 10 |
| Acerto (Hit) | 22 |
| Esquiva (Flee) | 4 |
| Chance de crítico | 5% |
| Resistência a efeitos | 25% |
| Recompensa | 300 XP |
| Gold | 20–40 |

O registro marca o inimigo como chefe, único e sem permissão de fuga. Esses campos são intenções/restrições do cadastro; o cumprimento de cada regra depende do fluxo de batalha. A configuração atual é diferente de propostas anteriores com 210 HP: esta ficha usa o arquivo vigente, com **200 HP**.

## Ciclo de ações

| Ordem | Ação | Comportamento |
|---|---|---|
| 1 | Prensa Hidráulica | Golpe com ataque base |
| 2 | Pressurizar | Não causa dano; anuncia o golpe pesado |
| 3 | Corte de Contenção | Ataque base +12 antes da defesa; pode aplicar Vazamento |
| 4 | Descompressão | Não ataca; abre uma janela de recuperação |

Depois da descompressão, o ciclo recomeça. A próxima ação é informada no resultado. Se o chefe estiver atordoado, o motor de combate não executa sua ação; a etapa não é consumida só pela passagem do turno bloqueado.

### Vazamento

Se o Corte acerta e o alvo não resiste ao efeito, aplica **2 de dano por turno durante 2 turnos**. O dano direto do golpe continua sujeito a acerto, defesa e crítico. O ataque pesado tem valor base 30 antes dessas etapas, não significa 30 de dano garantido no HP.

## Como ler o encontro

O comportamento anunciado permite antecipar o corte. O planejamento de HP, energia, técnica e consumíveis deve considerar as duas janelas em que o chefe não ataca. A descompressão é uma oportunidade prevista, não uma garantia de vitória com qualquer construção de Apollo.

Como a defesa do chefe é alta, apenas repetir uma ação fraca pode produzir um combate prolongado. Melhorar atributos, equipar peças e preparar técnicas faz parte da intenção do encontro. A disponibilidade desses recursos pela rota normal precisa ser avaliada separadamente do funcionamento isolado do boss.

## Recompensas e implementação

As recompensas de itens cadastradas podem ser consultadas na ficha do [Bestiário](https://github.com/SilvioPLopes/droids-code/wiki/Bestiario). A Wiki distingue quatro etapas: código de combate existente, prefab preparado, evento montado e encontro percorrido. Não considera as duas primeiras prova das duas últimas.

Fontes: [catálogo de inimigos](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Catalogos/CatalogoDeInimigos.cs), [DroidDoIvo.cs](https://github.com/SilvioPLopes/droids-code/blob/main/Assets/Scripts/Combat/DroidDoIvo.cs) e [plano do evento](https://github.com/SilvioPLopes/droids-code/blob/main/docs/planos-implementacao/2026-10-29-evento-boss-vale.md).
