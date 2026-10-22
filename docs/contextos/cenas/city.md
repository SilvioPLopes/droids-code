# Contexto da City

Contrato específico da cena. Andamento, testes e aceite ficam no [plano da tarefa](../../planos-implementacao/2026-10-22-orientacao-npcs-city.md). Para outras cenas, consultar o contexto correspondente; não transferir os valores da City automaticamente.

## Identidade e limites

- Cena: [City.unity](../../../Assets/Scenes/City.unity). Atores e nomes no mundo pertencem à cena; a UI do MenuMundoManager pode persistir nas transições.
- Escala local padrão definida pelo usuário para os NPCs da City: **X = 1.33, Y = 1.33**. Z dos três atores existentes permanece 2. Não redimensionar o protagonista para compensar diferenças visuais.
- Preservar GUIDs, sprites, posições e configuração local dos colliders/gatilhos ao ajustar apresentação. A escala do ator também afeta os filhos e o alcance físico dos colliders; não alterar os valores locais destes como efeito secundário.

## Atores e conteúdo

| Objeto na cena | Nome no mundo | Folha de poses | Fala principal / genérica | Retrato em Resources/Retratos |
|---|---|---|---|---|
| Dara | Dara | Characters/NPCs/Dara/animations/idle.png | city_s2_dara / city_generico_dara | Dara.png |
| homem da fila | Homem da fila | Characters/NPCs/Generic/morador_01/spritesheet_idle.png | city_s2_homem_fila / city_generico_homem_fila | Homem da fila.png |
| Tácio | Tácio | Characters/Antagonists/Ivo_Rangel/animations/idle.png | city_s2_tacio / city_generico_tacio | Senhor Tácio.png |

As folhas estão em `Assets/sprites/Entities/`. A arte de Tácio continua usando a folha de Ivo; não trocar a identidade visual sem escopo próprio. Os três têm OrientacaoNpc no mesmo objeto do gatilho: poses cima/esquerda/baixo/direita são os quadros 0/2/4/6. A pose frontal é a usada no retrato.

## Nomes no mundo

- Hierarquia padrão: `NPC/NomeNPC/Nome`. NomeNPC é um Canvas **WorldSpace**, filho do NPC, e não um Canvas Overlay preso à tela. O TMP existente é preservado para manter as referências de interação.
- Canvas local: área 250×32, escala uniforme 0.004, rotação identidade, centralizado em X. Sorting layer do SpriteRenderer, ordem do sprite + 10.
- TMP: fonte existente, tamanho 24, centralizado, uma linha sem quebra, sem raycast e com área esticada no Canvas. Não editar o material compartilhado da fonte para estilizar um único nome.
- Posicionamento: abaixo da maior altura das quatro poses, com folga local 0.04 entre os pés e o topo da área de nome. Centro Y local do contêiner: Dara -0.354; homem da fila -0.374; Tácio -0.349.
- Visibilidade por proximidade/disponibilidade preservada: Dara/fila usam `GatilhoDeDialogo.indicadorDeInteracao`; Tácio usa `NpcInterativo.indicadorDeInteracao`. A referência aponta ao TMP Nome, não ao Canvas dos menus.
- **Loja de Tácio:** continua em `Tácio/Canvas/Loja`, com Canvas ScreenSpaceOverlay separado. O nome fica no novo `Tácio/NomeNPC`. Não converter o Canvas da loja para WorldSpace nem herdar a configuração do nome.

## Diálogo e interface

- Montagem de referência: diálogo da CasaLip, com moldura `Assets/sprites/Atualizados/Tela_de_dialogo-removebg-preview.png`, miniatura frontal do falante, nome e fala ao lado. Não alterar a CasaLip para ajustar a City.
- Ramo local: `Canvas/AreaDialogo/PainelDialogo`. EscalaLocalDeDialogo mantém coordenadas virtuais 1920×1080, Match 0.5; a adaptação é exclusiva do diálogo. Painel com escala local 1, fonte 48, retrato com preserveAspect e AspectRatioFitter da referência.
- Canvas compartilhado da City permanece ConstantPixelSize. Não mudar seu Scaler como forma de corrigir diálogo ou nome de NPC; isso afeta os outros painéis.
- Retratos seguem o nome exato do **falante do catálogo**, que pode diferir do nome do objeto (Tácio fala como “Senhor Tácio”). DialogoUIManager consulta a lista manual e depois `Resources/Retratos/<Falante>`. O cache deve se inicializar sob demanda; ausência de cache não pode interromper miniatura/fala.
- Prioridade: fala principal disponível → fala genérica repetível quando principal indisponível/já vista. A genérica não grava flags/save nem consome a principal. Em Tácio, a loja abre depois da fala. O frame de fechamento não reutiliza a mesma tecla para abrir outra conversa.
- Dara/fila usam GatilhoDeDialogo AoInteragir e NpcInterativo desabilitado. Tácio usa NpcInterativo Loja e gatilho ApenasPorScript. Não criar duas origens para a mesma tecla.

## Mapa e transições

- Fundo: `Assets/sprites/Mapas/metavalle.jpg`, textura 1920×1080. Sprite metavalle_0: rect (15,0,1890,1080), PPU 140.625, preservando área 13.44×7.68 em unidades e a transformação do objeto do mapa.
- Fluxo: CasaLip ↔ City ↔ Game. Condições narrativas e configuração dos gatilhos/portão estão na [montagem do Ato 1, seção City](../../guias/montagem-ato-1.md#4-city-conversas-e-portão).
- `SaidaCidade/Canvas/desativado` fica inativo e desvinculado de TransicaoDeCena.textoDeBloqueio. Não reativar o aviso antigo como etiqueta do personagem.

## Fontes e validação

- Contratos compartilhados: [interface](../sistemas/interface.md), [estado e save](../sistemas/estado-e-save.md).
- Alterações visuais exigem registrar resolução/zoom e observar enquadramento, legibilidade e ausência de sobreposição. Propriedade salva não equivale a input real nem à aprovação visual do usuário; resultados pertencem ao plano.
- Pesquisa restrita: objetos Dara, homem da fila, Tácio e seus filhos em City.unity; GatilhoDeDialogo, NpcInterativo, OrientacaoNpc, DialogoUIManager e EscalaLocalDeDialogo somente se a tarefa envolver esses contratos.
