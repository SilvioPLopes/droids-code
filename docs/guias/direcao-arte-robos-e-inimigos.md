# Direção de arte e produção de robôs e inimigos

Guia inicial baseado no acervo inspecionado em 29/10/2026. **As observações descrevem as referências atuais; propostas e prompts abaixo ainda dependem de avaliação visual.** Estado da tarefa, cobertura dos inimigos e próximo passo ficam no [plano do Ato 1](../planos-implementacao/2026-10-29-direcao-visual-inimigos-ato1.md).

## Fontes e autoridade

1. Pedido atual e escolhas visuais do usuário.
2. [Enredo](../enredo/TCC_-_ENREDO_v2.md) e [adendo](../enredo/TCC_-_ENREDO_v2_ADENDO.md); adendo prevalece em divergências.
3. [Catálogo de inimigos](../../Assets/Scripts/Catalogos/CatalogoDeInimigos.cs): IDs e descrições existentes; não tomar números de combate como direção de arte.
4. Referências visuais efetivamente abertas e sua versão. Arquivo presente não significa aprovação do usuário.

O [guia LPC](criacao-sprites-personagens-lpc.md) já documenta personagens, receitas, créditos, limitações e reaproveitamento do processo para inimigos. O [guia de interfaces](interfaces-sobre-imagens.md) cobre fundos para controles reais. Este documento preenche a direção de robôs e imagens próprias; não substitui os dois processos.

## Referências visuais inspecionadas

| Referência | O que aproveitar | Limite |
|---|---|---|
| [Sucata A](../../Assets/sprites/Entities/Enemies/Battle/SucataCaptura-v1.png) | Corpo estreito, membros expostos, pinças, ferrugem, um sensor quente, pose à esquerda | PNG estático; não comprova grade lógica ou animação |
| [Sucata B v2](../../Assets/sprites/Entities/Enemies/Battle/SucataReforcada-v2.png) | Mesma família com massa, ombreiras, proteção e faixas de segurança | Referência de distinção de silhueta; não usar como corpo obrigatório de todo robô |
| [Render histórico da Battle](../../output/inimigos-battle/2026-10-29/battle-A-1280x720.png) | Escala aparente, direção do oponente, apoio dos pés, espaço para HUD/log | Captura anterior, não confirma cena atual ou v2 da B |
| [Painel Droid](../../Assets/UI/DroidIlustrado/droid-conceito-v2.png) | Aço escuro, latão/cobre, rebites, desgaste e construção industrial | É uma interface; não copiar seu nível de textura para uma sprite pequena |
| [Terminal](../../Assets/UI/TerminalIlustrado/terminal-foco-horizontal.png) | Tecnologia reparável, monitor verde, metal envelhecido | Verde do monitor não é uma cor obrigatória dos olhos de todos os droids |
| [Apollo LPC](../../Assets/sprites/Entities/Characters/Companion/Apollo/sprite.png) | Referência do personagem provisório e escala de detalhe dos humanos | Aparência humana/armadura não define o robô final |

## Linguagem visual a preservar

**Mundo material:** ficção científica de manutenção e escassez. Peças tiveram funções anteriores; cabos, parafusos, dobradiças e pistões precisam sugerir montagem plausível. Desgaste aparece em bordas, chapas e emendas. Um acessório deve ajudar a contar função ou procedência.

**Paleta observada:** carvão, aço gasto, cinzas sujos, ferrugem alaranjada, cobre/latão. Reservar luzes e cores vivas para sinais funcionais pequenos. Isso é leitura do acervo, não uma paleta numérica já aprovada. Não fixar códigos hexadecimais sem extrair e comparar as referências.

**Silhueta antes de textura:** reconhecer função pela forma mesmo reduzida. Captura = magro e pinças; reforçado = largo e protegido; selvagem = assimétrico e reparado; drone = reconhecimento aéreo; chefe = braços industriais e três lentes. As três últimas soluções de forma são propostas, limitadas pelas descrições narrativas.

**Retorno do usuário sobre o primeiro boss (29/10/2026):** três lentes e lâminas não bastam para a identidade. O conceito estreito produzido foi considerado semelhante a A, com menos ameaça que B; foi reaproveitado como [Droid C](../../Assets/sprites/Entities/Enemies/Battle/DroidC-v1.png), inimigo comum. Para o chefe, a direção desejada exige robustez, carisma, sombra e medo perceptíveis na própria forma, mesmo em tamanho reduzido. Não tentar resolver apenas ampliando a imagem ou acrescentando detalhes. A proposta anterior de corpo alto/estreito e o prompt histórico abaixo precisam ser revistos antes de nova produção; o [plano](../planos-implementacao/2026-10-29-direcao-visual-inimigos-ato1.md) guarda a hipótese pesada de contenção, ainda não gerada.

**Acabamento por destino:** sprites de combate usam contorno escuro, volumes em blocos e detalhes mecânicos legíveis. Humanos LPC, interfaces ilustradas e fundos não têm necessariamente a mesma densidade de pixels. Uma imagem grande com bordas pixelizadas não comprova uma grade consistente. Escolher a grade final numa etapa própria, comparando o robô ao Apollo e ao cenário na escala de uso.

Evitar neon espalhado, metal cromado impecável, armadura medieval, carne/ossos, rosto humano, armas militares gratuitas e engrenagens decorativas sem função. A óptica limpa do chefe é uma exceção narrativa intencional ao desgaste, não licença para deixar todo o corpo novo.

## Cânone e liberdade de desenho

| Personagem/família | Exigência das fontes | Decisão visual ainda aberta |
|---|---|---|
| Droid do Ivo | Corpo industrial antigo; placas da estação; dois braços hidráulicos com lâminas improvisadas; três lentes finas, limpas e caras | Base/pernas, arranjo das lentes, proporção, cor inicial dos sensores, grau de assimetria |
| Sucata de Captura | Rápida e frágil; imobiliza; garras | Usar A como referência já existente; não redesenhar sem motivo |
| Sucata Reforçada | Captura remendada pela Apex | B v2 é a variante existente; aprovação final em cena é distinta |
| Droid Selvagem | Offline; pernas tortas de sucata; azul instável nos olhos | Corpo próprio e postura irregular; evitar apenas recolorir A |
| Selvagem Alfa | Maior; peças de três droids; espera antes de atacar | Massas e ritmo visual distintos, sem virar chefe da estação |
| Carcaça Errante | Papel de inimigo lento/resistente no catálogo | Consultar descrição completa antes da produção; desenho ainda aberto |
| Drone de Reconhecimento | Vigilância e aviso de presença | Corpo compacto, óptica e propulsão são propostas; não existe anatomia fechada na consulta |
| Artemis / Nadir | Adendo: Artemis sem corpo; Nadir não é chefe | Não produzir avatar de combate para preencher a lista |

No chefe, as lentes ficam brancas durante a leitura profunda e apagam sem resultado (sessão 6.7). A cor branca desse momento não define o idle. Produzir futuramente a variação a partir do mesmo desenho, mantendo contagem, posição e corpo.

## Ficha antes de gerar

Registrar na pasta da produção, sem copiar o andamento do plano:

- ID estável, nome, destino: conceito, sprite Battle, sprite do mundo ou interface.
- Fonte de cada característica, separando cânone, referência existente e proposta.
- Silhueta, materiais, paleta, marcas de identidade e itens proibidos.
- Referências com caminho/versão e papel: estilo, identidade, escala ou enquadramento.
- Corpo completo, direção, câmera, linha dos pés e margens; altura aparente desejada no consumidor.
- Dimensões solicitadas e reais; grade lógica apenas se definida e verificada; alpha real para sprite isolada.
- Poses e direções pedidas e entregues; não inferir animação a partir de um quadro.
- Prompt exato, ferramenta, imagens de origem, versão, hash, créditos e limitações.

Destino proposto: `output/inimigos-ato1/<data>/<id>/`. Só a arte escolhida segue para `Assets/sprites/Entities/Enemies/Battle/`, em arquivo versionado. Preservar originais, `.meta` e GUIDs. Não apontar o projeto para uma imagem que só existe no diretório privado do gerador.

## Processo existente e sequência de produção

1. **Referência narrativa e visual:** consultar trechos pertinentes e abrir os PNGs reais. Para personagem novo montado pelo processo LPC, seguir também a leitura narrativa exigida pelo guia LPC antes da montagem.
2. **Escolher técnica:** humano compatível usa receita LPC com peças/créditos conferidos; robô próprio usa desenho/sprite dedicado. O Apollo LPC é aproximação, não um gabarito definitivo de robô. Não há pipeline de modelos 3D comprovado nesta consulta.
3. **Conceito único:** imagem estática de corpo inteiro para avaliar identidade, sem prometer rig/animação. Para geração raster, usar ImageGen integrado com as imagens de referência explícitas; guardar prompt e origem. Nas Sucatas existentes esse processo já deixou `prompt.txt`, `character-config.md` e verificações em `output/inimigos-battle/2026-10-29/`.
4. **Revisão na escala de uso:** conferir silhueta reduzida, contagem de lentes, função dos braços, pés, contraste e fidelidade à família. Uma imagem impressionante ampliada pode falhar no quadro pequeno da Battle. Usuário avalia a direção antes do lote de poses.
5. **Preparação técnica:** conferir alpha nas áreas vazias e contorno, dimensões, margens e importação. A configuração Point não transforma uma imagem em pixel art de grade regular. Reescalas/recortes devem preservar a versão de origem e constar da ficha.
6. **Representações separadas:** Battle precisa inicialmente de um quadro estático. Mundo pode precisar de quatro direções, idle/walk, pivot e collider. Estados de ataque, leitura, impacto e desativação são entregáveis futuros explícitos, não anexos presumidos do conceito.
7. **Integração delimitada:** confirmar projeto/cena/Play e alvo antes de operar Unity; registrar valores anteriores e reversão; associar ao ID existente. Descobrir consumidores da lista no prefab e na cena antes de editar; não alterar balanceamento junto com arte.
8. **Verificação:** associação por ID e fallback; enquadramento real sem cortar braços/lentes/pés ou cobrir UI. Registrar render, janela, zoom e Canvas/Scaler efetivos. Usar 1280×720 e 844×475 como condições históricas de comparação, não como prova do estado atual. Separar editado, salvo, API, input real e aprovação visual.

## Prompts iniciais de proposta

As fichas abaixo registram a proposta inicial. O chefe teve uma geração derivada, cujo prompt exato está na ficha de produção; o resultado foi rejeitado como boss e reaproveitado como Droid C. **Não reutilizar o prompt estreito do chefe como direção aprovada.** As fichas dos menores permanecem propostas não executadas nesta tarefa. Anexar referências locais abertas e identificar seus papéis. Dimensão final e grade dependem da preparação para o consumidor.

### Chefe — `droid_do_ivo`

```text
Use case: stylized-concept.
Asset type: proposta de sprite estática de inimigo para Battle de Droids Code.
Pedido: um único Droid Improvisado do Ivo, máquina industrial de manutenção
transformada às pressas em aparelho de captura e leitura de núcleos.
Referências: SucataCaptura-v1 e SucataReforcada-v2 apenas para materiais,
paleta e acabamento; não copiar corpo, cabeça de um olho nem pinças.
Sujeito: estrutura de droid industrial antigo, placas da Estação Sete
soldadas irregularmente, mangueiras e pistões expostos; exatamente dois
braços hidráulicos de manutenção com lâminas de chapa adaptadas.
Identidade obrigatória: exatamente três lentes finas, impecavelmente limpas,
de precisão, visíveis e distintas, contrastando com o corpo feito às pressas.
Proposta de forma: corpo alto e irregular, braços longos; aparência de máquina
de oficina perigosa, com a função original ainda reconhecível.
Estilo: arte 2D para RPG, contorno escuro, volumes em blocos, poucos detalhes
funcionais bem legíveis; aço gasto, carvão, ferrugem e cobre contido.
Composição: corpo inteiro em três quartos voltado à esquerda, ambos os pés
visíveis na mesma linha de apoio, membros separados por espaços legíveis,
margens transparentes em todos os lados. Sem perspectiva extrema.
Não incluir: humano Ivo, Apollo, cenário, base, sombra de chão, texto,
números, HUD, minions, arma de fogo, armadura militar ou pose extra.
Saída: uma imagem isolada com alpha realmente transparente.
```

### Inimigo menor — `droid_selvagem`

```text
Use case: stylized-concept.
Asset type: proposta de sprite estática de inimigo para Battle de Droids Code.
Um único droid offline da Cratera, pernas tortas remendadas com sucata,
peças de procedências diferentes e olhos de azul instável. Proposta:
postura irregular e assimetria evidente; distinguir da Sucata de Captura
pela silhueta, não só pela cor. Usar a Sucata A como referência apenas
de material/paleta/acabamento, preservando o azul narrativo dos olhos.
Arte 2D com contorno escuro e volumes em blocos; ferrugem e aço gasto.
Corpo inteiro em três quartos voltado à esquerda, pés na mesma linha,
margens livres, alpha transparente. Sem cenário, base, texto, HUD,
peças orgânicas, emblema de subordinado do Ivo ou personagens extras.
```

### Inimigo menor — `drone_reconhecimento`

```text
Use case: stylized-concept.
Asset type: proposta de sprite estática de inimigo para Battle de Droids Code.
Um único Drone de Reconhecimento da Apex. Proposta de anatomia ainda aberta:
corpo aéreo compacto, óptica de observação evidente, propulsão mecânica
legível e acabamento industrial coerente com as Sucatas A/B de referência.
Comunicar vigilância e coleta de informação. Não transformar em bombardeiro
ou em escâner de três lentes do Ivo. Arte 2D, contorno escuro, volumes em
blocos, aço gasto e ferrugem contida. Objeto inteiro, três quartos orientado
à esquerda, margens livres e alpha transparente. Sem cenário, sombra de chão,
linhas de movimento, texto, HUD, arma pesada ou personagens extras.
```

## Aceite visual da família

O jogador reconhece os inimigos pela forma no tamanho em que joga; o chefe mantém três lentes distinguíveis e braços de manutenção; o corpo completo cabe sem cortes ou sobreposição com HUD/log; materiais e contraste combinam com as referências. Estados especiais mantêm o mesmo personagem. A arte do mundo combina com portas/piso e o tamanho aparente do elenco. Nada disso é atestado apenas por PNG válido, compilação ou associação correta no Inspector.
