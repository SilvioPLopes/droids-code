# Revisão da CasaLip

> Revisão datada. Para estado e aceite vigentes, usar a [retomada do plano](../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente); não executar pendências antigas sem conferir esse bloco.

## Estado em 17/10/2026

**Implementado; validação parcial registrada.** Base `027952041d40e7957875a66641727684586455c1`, branch `docs/consolidar-contexto`, com alterações locais de runtime, cenas e assets. Resultados e pendências estão na [validação de 17/10](casalip-validacao-2026-10-17.md); a coleta de 16/10 abaixo permanece como diagnóstico histórico.

- `ControlePuzzleAgua`, local e ativo, reúne `VerificadorDePuzzle` para `p_agua_casa` e `ControladorPuzzleAguaCasa`. O checker legado foi removido da cena; `TextoApollo` de vitória foi desativado.
- O verificador associa o terminal válido, incluindo UI inativa/persistente, grava `ato1_casa_resolvida` na primeira resolução e restaura a porta local pela flag, sem repetir save, progresso ou evento. A introdução usa `ato1_falou_apollo_casa` e impede repetição por essa mesma chave.
- O controlador aplica o contexto de água ao terminal e apresenta `casa_porta_apollo` pelo catálogo depois de fechar terminal e origem do menu. Restauração não dispara a conversa de sucesso.
- `Abrir` quando o terminal já está aberto preserva Runner/rascunho e não acumula contador. Fechar e inicializar terminal/diálogo são idempotentes. O menu controla a origem de abertura e o diálogo pode ser localizado inativo.
- A montagem do diálogo foi refeita com âncoras coerentes, escalas unitárias, nome habilitado, retrato com proporção preservada e Canvas escalável. Lip recebeu a arte disponível de protagonista; Apollo usa o idle LPC provisório e retrato associado, com etiqueta no mundo. A captura final `depois-1280.png` foi inspecionada pelo agente principal e está legível. Quatro direções e quatro diagonais foram verificadas por API, sem flip na arte LPC e com comparação ao flip legado; isso não substitui input real/física nem aprovação a 1920.
- O save passou a v6. A migração preserva conclusão legada verdadeira e acrescenta introdução verdadeira; o dado antigo não distingue conversa de puzzle, portanto um save legado de quem só conversou também mantém a conclusão. Saves novos separam as duas flags.

`Logs/CorrecaoCasaLip/green-harness-final.txt` registra seis OK recentes: abertura repetida, primeiro diálogo inativo, leitura Lua finita, contexto, origem do menu e migração. `fluxo-validacao.txt` registra o percurso por API: MainMenu → CasaLip com introdução verdadeira/conclusão falsa/porta trancada; variável ausente, texto e 90 rejeitados; atribuição direta 100 pela origem mundo concluiu, salvou, destrancou e abriu quatro falas com um menu/evento. Ao terminar, contador zero; reexecução não repetiu save/evento/diálogo.

**C05 passou por API em duas revisitas CasaLip → City → CasaLip:** harness confirmou nova porta aberta, terminal/referências vivos e verificador local único, sem repetir introdução/sucesso; hash do save permaneceu igual. Essa evidência não equivale ao reinício de Play com load (`C06`), ainda pendente. Também faltam sucesso integrado pelo menu/laço finito/diálogo ocupado, imagem 1920 com narração/fala longa e input real. O usuário interrompeu Computer Use com Escape; não há aprovação de input a partir dessa tentativa.

O save original foi restaurado pelo agente principal via MCP, com SHA-256 igual ao backup: `F9018A19EFE54652D201B02BF161E7C808D3B4DC1F2FCFAF025B29F74322ABC7`. A revisão estática em `runtime-review.md` não encontrou defeito novo no pacote revisado e explicitou seus limites.

Permanecem fora deste recorte a proteção de execução Lua (`TER-001`), conteúdo narrativo opcional da casa, orçamento/progressão de City e validação do Ato 1 inteiro. O critério continua `nivel_agua >= 100`, aceitando atribuição direta.

## Registro histórico — inspeção de 16/10/2026

As descrições de implementação e pendências a seguir correspondem ao estado anterior às correções de 17/10.

**Data:** 16/10/2026, America/Sao_Paulo. **Base:** `027952041d40e7957875a66641727684586455c1`, branch `docs/consolidar-contexto`.

**Escopo:** revisão incremental da primeira cena da demo, solicitada pelo responsável. A primeira inspeção leu código e consultou o MCP fora de Play Mode, no Unity `6000.5.8f1`, projeto `C:/Users/silve/DroidsCode`. Depois, as seis capturas e a sessão em Play do responsável permitiram inspecionar o estado resultante da revisita. Nenhum script, cena, prefab ou save foi alterado pelo agente. O agente não executou automaticamente o percurso; o teste de reinício/load permanece pendente.

Retomada: [índice de trabalho](../indice-trabalho.md). Tarefas e critérios completos: [plano existente](../plano-desenvolvimento.md). Direção narrativa: [Sessão 1 do enredo](../enredo/TCC_-_ENREDO_v2.md) e [adendo](../enredo/TCC_-_ENREDO_v2_ADENDO.md).

**Coleta mais recente:** [análise das capturas e do layout](../problemas/casalip-apresentacao-dialogo.md). Ela identifica `ae69544` como o commit que inverteu as âncoras da janela e reduziu o texto, confirma as duas artes de mago do Apollo e o Canvas de tela do nome, e registra a referência destruída de terminal/porta após revisita. O pedido atual inclui usar `casa_porta_apollo` para o sucesso do puzzle. Tarefas visuais: `UI-003` e `VIS-001`.

## O que já existe

| Parte | Evidência atual | Situação |
|---|---|---|
| Player | `Actors/Player` ativo, tag `Player`, CircleCollider2D sólido, PlayerMovement e Animator existentes | Montado; movimento depende da verificação em Play |
| Terminal físico | `TerminalCasa` ativo, BoxCollider2D com `Is Trigger`, interação por `E`, referência preenchida para PainelTerminal | A configuração que impedia E em setembro está corrigida; há validação histórica |
| Terminal Lua | Campo, Executar, Fechar, log, scroll e pontos associados; dicas do Apollo habilitadas | Implementado e montado; abertura e primeira resolução têm validação histórica |
| Verificação da água | Um `PuzzleAguaCasaChecker` no `PainelTerminal`, variável `nivel_agua`, alvo `100`, porta e TextoApollo associados | Implementação vigente; nenhum `VerificadorDePuzzle` anexado foi encontrado pela consulta MCP |
| Porta | `PortaDeSaidaCasa` ativa, TransicaoDeCena habilitado, BoxCollider2D trigger, `trancada = true`, mensagem de bloqueio associada | Estado inicial configurado corretamente |
| Destino | Porta aponta para `City`, chegada `(8.89, -9)` | Referência e coordenadas configuradas; posição efetiva precisa de percurso |
| Apollo | Um GatilhoDeDialogo, modo AoInteragir, Id `casa_intro_apollo`, falas manuais e escolhas vazias | Usa o catálogo; consequência da conversa precisa de revisão |
| UI persistente | MenuMundoManager mantém o Canvas raiz por DontDestroyOnLoad | Implementado; associações com objetos locais precisam sobreviver corretamente às revisitas |

A [validação de 14/09](../historico/2026-09-14-bugs-casalip.md) registrou a resolução do problema inicial de leitura de `nivel_agua` e do collider que impedia a interação por E. Isso não encerra o [problema posterior de revisita](../problemas/casalip-retorno-porta-trancada.md).

## Como a porta funciona hoje

1. A cena carrega a porta com `TransicaoDeCena.trancada = true`.
2. Perto de TerminalCasa, E chama `TerminalUIManager.Abrir()`. Esc → Terminal usa a mesma instância.
3. Abrir cria uma nova VM Lua. Executar roda o texto, atualiza o log e invoca `AoExecutarCodigo`, inclusive quando a execução retorna erro.
4. `PuzzleAguaCasaChecker.VerificarVitoria()` lê `Runner.ObterVariavelNumerica("nivel_agua")`.
5. Variável ausente/não numérica ou valor menor que 100 mantém a porta trancada. Valor igual ou maior que 100 marca `_puzzleResolvido`, mostra a mensagem do Apollo e desmarca `trancada` na porta referenciada.
6. Ao entrar novamente no trigger da porta com a tag Player e `trancada = false`, TransicaoDeCena agenda a posição de chegada e carrega City.

O checker **não consulta nem grava uma flag de história**, não chama Salvar e não concede pontos. A conclusão acima pertence à instância do checker. A porta só reage a OnTriggerEnter2D: destrancar enquanto o Player já está sobre o trigger não executa uma nova transição por si só.

O critério atual aceita também `nivel_agua = 100` diretamente. Ele verifica o resultado numérico; não comprova que o jogador escreveu um `while`. Nenhuma alteração de avaliação pedagógica foi decidida nesta revisão.

## Flags e estados da casa

| Nome | Quem altera hoje | O que significa atualmente | Tratamento pendente |
|---|---|---|---|
| `ato1_casa_resolvida` | GatilhoDeDialogo do Apollo, em AoConcluirDialogo; chama Salvar | Conversa introdutória terminada, mesmo sem resolver a água | `HIS-001`: reservar a conclusão para o puzzle e separar a conversa |
| `_puzzleResolvido` | PuzzleAguaCasaChecker, após valor atingir o alvo | Booleano privado da instância; impede repetir os efeitos | `INT-001`: conclusão persistente e reaplicação na nova porta |
| `trancada` | Valor inicial da porta e atribuição feita pelo checker | Estado local da passagem, não é flag de história | Derivar da conclusão correta em cada entrada |
| `ato1_viu_caderno` | Nenhum gatilho correspondente encontrado nesta cena | Flag opcional prevista no enredo | Montar junto da interação do caderno |
| `ato1_viu_rotina` | Nenhum gatilho correspondente encontrado nesta cena | Flag opcional da cena das 3h12 | Definir o disparo noturno e montar quando chegarmos a esse trecho |

O gatilho do Apollo está com `apenasUmaVez = true`, mas `flagsNecessarias` e `flagsQueImpedem` estão vazios. A restrição local vale para aquela instância; a flag gravada não impede automaticamente outra conversa após recriar a cena.

`ato1_viu_torre` pertence ao mirante em City, não a uma interação dentro de CasaLip.

## O que explica a porta fechada após voltar

O [relato INT-001](../problemas/casalip-retorno-porta-trancada.md) guarda a reprodução relatada e o estado observado em execução. A leitura atual e a cena ainda exibem a estrutura descrita ali:

- O checker é filho do Canvas persistente. Quando resolvido, guarda `_puzzleResolvido = true` e deixa de avaliar outras execuções.
- A porta pertence à cena e é destruída ao sair. A porta recriada nasce trancada.
- O checker antigo não reassocia a nova porta nem reaplica a conclusão; o novo Canvas é descartado como duplicata.
- TerminalCasa também guarda uma referência serializada para o terminal da nova cena, sem mecanismo de recuperação da instância persistente.

Essa é a causa sustentada pelo código e pelas observações para a **revisita**. Na coleta mais recente do responsável, o MCP confirmou `_puzzleResolvido = true`, porta antiga sem referência viva, TerminalCasa com terminal destruído e porta nova trancada. Se o sintoma acontecer já na primeira resolução, verificar valor lido, evento, referência da porta e reentrada no trigger antes de atribuir a mesma causa. O percurso foi executado pelo responsável; o agente inspecionou seu resultado.

## Conteúdo da casa a completar depois da base

No Editor foi encontrado somente um GatilhoDeDialogo, o do Apollo. As falas `casa_filtro`, `casa_caneca`, `casa_caderno`, `casa_noite_apollo`, `casa_puzzle_dica` e `casa_porta_apollo` existem no catálogo, mas não têm gatilhos próprios montados na cena inspecionada.

O terminal ainda apresenta o exemplo geral `droid.subirAtributo("For", 1)`, enquanto o puzzle da casa prevê `nivel_agua = 0` e orientação sobre `while`. Conferir essa apresentação ao finalizar o tutorial.

Os sprites novos ainda não substituíram os dois personagens desta cena: Apollo usa `Assets/sprites/Entities/Npc's/Wizzard/Death/Death-Sheet.png`, sem Animator; Player usa `Assets/sprites/Entities/Characters/Body_A/Animations/Walk_Base/Walk_Down.aseprite`, com `Assets/Scripts/PlayerAnimatorController.controller`. A [documentação visual](../../Assets/sprites/Entities/Npc's/Characters/Documentation/sprite-mapping.md) identifica o Apollo novo como placeholder e enumera gestos/props pendentes. Integrar os assets preservando componentes e referências do Player.

## Ordem para encerrar este trecho

1. Reexecutar `VAL-01/02` para registrar o funcionamento inicial, mantendo as validações antigas como evidência histórica.
2. Resolver `INT-001/HIS-001` juntos: autoria da conclusão, controlador local, associação ao terminal persistente e restauração da porta. O guia já descreve a migração; a busca do VerificadorDePuzzle por terminal inativo precisa ser revisada.
3. Validar `VAL-03`: duas revisitas e reinício com load. Confirmar que conversar sem resolver não libera a progressão de City.
4. Rever `UI-001/TER-002` pelo `VAL-04`: abertura pelo mundo e menu, digitação com E, fechamento, contador e log. O código ainda permite chamar Abrir repetidamente e inicia a rolagem antes de ativar o painel.
5. Finalizar a apresentação da casa: código inicial, falas e objetos narrativos, sprites e feedback do gerador/água. Para o tutorial de laços, tratar também a execução limitada de `TER-001`; não testar laço infinito antes da proteção.

Esses passos detalham as tarefas existentes para CasaLip. Nenhuma delas foi marcada como concluída por esta auditoria. Resultado esperado para avançar: resolver, sair, voltar e carregar preserva a conclusão; UI libera o Player; o jogador entende o primeiro puzzle e conhece Apollo.

## Chaves para retomar

`CasaLip`, `INT-001`, `HIS-001`, `UI-001`, `UI-003`, `VIS-001`, `TER-002`, `TER-001`, `VAL-02`, `VAL-03`, `VAL-04`, `ato1_casa_resolvida`, `nivel_agua`, `_puzzleResolvido`, `PortaDeSaidaCasa`, `TerminalCasa`, `PainelTerminal`, `AoExecutarCodigo`, `casa_porta_apollo`, `ae69544`.
