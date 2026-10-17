# CasaLip — implementação e validação de 17/10/2026

**Estado:** implementação aplicada; encerramento do plano ainda pendente dos casos indicados abaixo. **Ambiente:** Unity 6000.5.8f1, Windows, branch `docs/consolidar-contexto`, base `027952041d40e7957875a66641727684586455c1` com alterações locais, sem novo commit. Datas desta revisão em America/Sao_Paulo; o log técnico registra UTC.

Plano seguido: [correção da CasaLip](../planos-implementacao/2026-10-16-correcao-casalip.md). Diagnóstico original: [apresentação](../problemas/casalip-apresentacao-dialogo.md) e [retorno](../problemas/casalip-retorno-porta-trancada.md).

## Atualizacao posterior

### Teste visual do usuário e reversão do aumento local do terminal — 17/10/2026

**Resultado: rejeitado pelo usuário.** Após testar a alteração, o responsável relatou que os tamanhos pioraram: menu minúsculo, diálogo quase ilegível e terminal ilegível. Este retorno supera o aceite visual anterior como evidência da apresentação atual. Não há comprovação das resoluções renderizadas nos três testes nem de todos os percursos de input.

Capturas fornecidas pelo usuário, preservadas no repositório:

- [Menu minúsculo](evidencias/casalip-correcao-2026-10-17/rejeicao-escala-menu.png).
- [Diálogo com texto quase ilegível](evidencias/casalip-correcao-2026-10-17/rejeicao-escala-dialogo.png).
- [Terminal ilegível](evidencias/casalip-correcao-2026-10-17/rejeicao-escala-terminal.png).

**O que foi alterado nesta tentativa:** somente `m_LocalScale.x` e `m_LocalScale.y` do `RectTransform` de `Canvas/PainelTerminal`, de 1 para 1,5, por overrides na instância de `Assets/Scenes/CasaLip.unity`; Z permaneceu 1. O alvo no prefab é `fileID: 8899243622573378381`, GUID `69e38ec182414fb4f8b024d51a9fb2a5`. Canvas/Scaler, menu, diálogo e scripts não foram alterados funcionalmente nesta tentativa. O Unity normalizou espaços em campos YAML vazios ao salvar.

**Por que a decisão falhou:** o agente escolheu ampliar uniformemente em 50% para preservar proporções e inferiu, pelas dimensões dos filhos, que o conteúdo caberia em 1280×720. Confirmou apenas alvo, escala, persistência e diff; deixou a validação visual com o usuário, conforme solicitado. Isso demonstrava que a edição foi aplicada, mas não que ela corrigia a legibilidade. A consulta do Canvas já mostrava `ConstantPixelSize`, fator 1 e retângulo 3840×2160, divergindo da referência histórica de 1280×720. O agente não investigou essa divergência nem relacionou resolução renderizada, tamanho efetivo dos textos e apresentação da Game View antes de escolher o ajuste local.

**O que a evidência permite afirmar:** a escala numérica foi aumentada, não diminuída. Entretanto, a tentativa falhou no teste real, e as três capturas mostram UI ilegível. As capturas de menu e terminal exibem `Scale 0.22x` na Game View; essa redução de visualização é uma pista para o diagnóstico, não prova da causa nem motivo para invalidar o relato. Como a edição local não alterou menu/diálogo, a causa da piora conjunta permanece sem diagnóstico; não atribuí-la automaticamente ao terminal ou somente à Game View.

**Reversão:** projeto e CasaLip confirmados; Editor estava em Play. Foi encerrado Play e foram revertidos somente os overrides X/Y com `PrefabUtility.RevertPropertyOverride`; consulta confirmou `(1, 1, 1)` e cena salva. O salvamento também serializou tamanho de `PainelDeEscolhas` e posição do conteúdo do scroll, fora do escopo. A cena foi descarregada abrindo MainMenu e esses dois valores foram repostos ao conteúdo em disco anterior à reversão. Diff final ignorando espaços no fim das linhas ficou vazio para CasaLip e Canvas.prefab, comprovando ausência de mudanças funcionais remanescentes desta tentativa. Editor deixado em MainMenu, fora de Play. Não houve novo teste nem restauração de save; a incerteza anterior sobre o save ativo permanece, inclusive após os testes realizados pelo usuário. Backups anteriores não foram acessados.

**Pendente:** investigar a cadeia de escala/resolução e as instâncias persistentes de UI antes de propor outra alteração. A reversão recupera os valores anteriores à tentativa, mas não é evidência de que a legibilidade voltou ou de que o problema anterior foi resolvido.

Retificação de 17/10/2026, preservando abaixo o registro da primeira rodada. O [plano da tarefa](../planos-implementacao/2026-10-16-correcao-casalip.md#retomada-vigente) é a fonte de retomada e aceite vigente.

O usuário aprovou porta e diálogo, rejeitou o tamanho dos menus/terminal e apontou que Apollo não vira para o jogador. A captura 1280 aprovada pelo agente não substitui esse retorno.

O log local `fluxo-validacao.txt` contém verificações posteriores:

| UTC em 17/10 | Evidência por API |
|---|---|
| 16:35:40 | C06: nova sessão Play e load; conclusão/porta/terminal preservados, UI fechada e contador zero |
| 16:35:43 | C08: fala longa com quatro páginas e fonte 24; não é validação visual final |
| 16:36:30 | C08: narração sem nome, contador zero |
| 16:36:32–33 | C02/C04: while finito pelo menu, espera por conversa ocupada, callback preservado; catálogo de quatro falas concluído, contador zero |

As imagens `fala-longa-1280.png` e `fala-longa-1920.png` capturaram buffer antigo de City e foram descartadas como prova visual da casa. Input físico e integração de Status/Bag/Droid continuam não comprovados.

**Save:** a restauração com hash confirmado descrita abaixo ocorreu antes dessa última rodada. Os testes posteriores gravaram novamente; a restauração final não foi confirmada. Backup anterior à última rodada: `Logs/CorrecaoCasaLip/save-antes-encerramento.json`. Conferir arquivo atual e preservar progresso novo antes de restaurar. Esta atualização é documental e não opera o Unity ou o save.

## Registro da primeira rodada

## Implementação aplicada

- Janela inferior com Canvas Scaler 1280×720/Match 0.5, âncoras coerentes, escala um, moldura importada no retângulo visível 585×183, áreas de retrato/texto e fonte 24 com paginação.
- Apollo usa o quadro frontal parado `idle_4` de `Characters/Companion/Apollo/animations/idle.png`, escolhido conforme o pedido do responsável. A etiqueta é World Space. O mesmo asset substitui o retrato de mago pela lista manual. A arte continua sendo o placeholder LPC disponível.
- Lip usa o atlas LPC, oito clips e `LipCasaLip.controller` em CasaLip e City. `PlayerMovement.usarEspelhamentoHorizontal=false` evita inverter novamente os quadros de esquerda/direita. GUIDs dos assets, transform, tag, física e componentes do Player foram preservados.
- Aberturas de terminal e diálogo inicializam uma vez; abrir terminal repetido mantém Runner/contador. O menu guarda a origem do painel e libera apenas seu bloqueio. O contexto da casa apresenta a água e conserva rascunhos.
- `ControlePuzzleAgua` é local à CasaLip, com VerificadorDePuzzle e ControladorPuzzleAguaCasa. O checker legado foi removido da instância; o arquivo original permanece. LocalizadorDeTerminal recupera a UI persistente, inclusive inativa.
- A introdução grava `ato1_falou_apollo_casa`; uma resolução nova grava `ato1_casa_resolvida`, salva e apresenta `casa_porta_apollo`. Restaurar a conclusão apenas reaplica a porta.
- DTO v6 e migração em memória antes da restauração de flags.

## Decisão sobre saves antigos

O save real v5 continha somente `ato1_casa_resolvida=true`. O código antigo gravava essa flag ao terminar a introdução; o checker da água não tinha outro marcador persistente. Portanto o arquivo não permite descobrir se a água também foi resolvida.

Para conservar o progresso informado pelo responsável, a migração **mantém a conclusão antiga verdadeira** e acrescenta a introdução verdadeira. Isso substitui a proposta inicial de apagar a conclusão. Consequência explícita: um save antigo de quem só conversou também recebe a passagem liberada. Saves novos distinguem os dois eventos. Inventário, atributos, técnicas, Gold, contadores e outras flags são preservados; migrar novamente é idempotente.

## Evidências e resultados

O agente principal executou os percursos por APIs dos componentes usando MCP. Essas chamadas não comprovam por si só as teclas físicas E/Esc nem deslocamento manual. A revisão independente de runtime não confirmou defeitos novos no recorte, condicionando o encerramento aos testes de integração restantes.

| Caso | Resultado observado | Estado |
|---|---|---|
| C01 | MainMenu.NovoJogo abriu CasaLip; concluir introdução gravou somente a introdução e manteve a porta trancada | Aprovado por API |
| C02 | Variável ausente, texto e 90 não concluíram; primeira abertura apresentou o código inicial da água; atribuição 100 concluiu | Aprovado nesses caminhos; integração com while ainda pendente |
| C03 | Fixture real de terminal preservou Runner/variável Lua em abertura repetida e balanceou fechamento/callback | Aprovado por API; digitação física com E e Esc pendente |
| C04 | Resolução nova pelo terminal aberto por API fechou terminal/menu, abriu quatro falas do catálogo, manteve contador 1 durante o diálogo e 0 no final; evento ocorreu uma vez | Aprovado na origem mundo/API; menu/while e diálogo ocupado pendentes |
| C05 | CasaLip → City → CasaLip repetido **duas vezes**; porta destrancada, terminal vivo, um checker local, nenhum legado, introdução/sucesso sem repetição; hash do save e contadores não mudaram | Aprovado por API |
| C06 | Save v6 resolvido preservado como fixture; houve saída de Play, mas o carregamento em sessão nova não foi concluído | Pendente |
| C07 | Migração v5 com flag verdadeira, v5 sem flag, v6 e idempotência aprovados, conservando dados alheios à casa | Aprovado em memória; load completo ainda pendente |
| C08 | Captura final 1280×720 inspecionada: moldura, nome, retrato e avanço legíveis | Parcial; 1920×1080, narração e fala longa finais pendentes |
| C09 | Quatro direções e quatro diagonais por API usaram atlas LPC sem flip duplicado; cenário de flip antigo foi comparado com a opção corrigida | Aprovado por API; percurso manual/câmera e inspeção em City pendentes |
| C10 | Fixtures aprovaram primeiro diálogo inativo e origem do terminal/menu | Parcial; conferência integrada de Status/Bag/Droid pendente |

Seis checks do harness passaram na execução final: `ValidarAberturaRepetidaDoTerminal`, `ValidarPrimeiroDialogoInativo`, `ValidarLeituraLuaFinita`, `ValidarContextoDoTerminal`, `ValidarOrigemDoTerminal`, `ValidarMigracaoCasaLip`. O harness também aprovou estado inicial, estado resolvido e cada uma das duas revisitas durante o percurso.

Na primeira chamada agregada da suíte imediatamente após carregar a cena houve uma TargetInvocationException, sem causa detalhada no retorno. Ela não foi contada como sucesso. Os seis checks foram depois executados individualmente com estado inicializado e painel real fechado; todos retornaram OK. O registro final preserva essa distinção.

Antes da correção, chamadas diretas demonstraram Runner recriado, contador 1→2→1, singleton sem encontrar diálogo inativo e porta recriada trancada com checker persistente resolvido. O harness foi escrito antes das correções, mas não estava importado na execução desses diagnósticos RED; não se atribui a ele aquela reprodução.

## Arquivos de evidência e save

- [Percurso executado](../../Logs/CorrecaoCasaLip/fluxo-validacao.txt), [harness final](../../Logs/CorrecaoCasaLip/green-harness-final.txt), [revisão de runtime](../../Logs/CorrecaoCasaLip/runtime-review.md).
- [Captura final 1280×720](evidencias/casalip-correcao-2026-10-17/sucesso-1280.png).
- Scripts explícitos de reprodução: [harness Editor](../../Assets/Editor/ValidacaoCasaLip.cs) e [comandos do percurso](../../Logs/CorrecaoCasaLip/FluxoCasaLip.cs). O segundo fica em Logs, não é importado nem executa automaticamente.

Os arquivos em `Logs` são locais e ignorados pelo Git. Os resultados relevantes estão resumidos nesta revisão; não dependem de publicar o save do responsável.

O save do responsável foi copiado antes dos testes e **restaurado ao final**, com SHA-256 idêntico `F9018A19EFE54652D201B02BF161E7C808D3B4DC1F2FCFAF025B29F74322ABC7`. Caminho ativo: `Application.persistentDataPath/save.json`, nesta máquina `C:/Users/silve/AppData/LocalLow/DefaultCompany/DroidsCode/save.json`. Backups locais: `Logs/CorrecaoCasaLip/save-original.json` e `save-antes-validacao-2026-10-17.json`. A fixture resolvida v6 está em `save-v6-resolvido.json`.

## Interrupção e retomada

O servidor MCP local estava desligado e foi iniciado pelo binário oficial já instalado. Um aviso de atualização do plugin foi dispensado; as recompilações/recargas do Unity interromperam a conexão durante a tentativa de nova sessão. Essas falhas de ferramenta e os erros de scripts temporários de diagnóstico não equivalem a erro de compilação dos fontes finais. O runtime corrigido carregou e os testes acima foram executados.

O responsável interrompeu Computer Use pela tecla física Esc. Nenhuma nova entrada de computador foi enviada depois disso. O processo não é declarado encerrado.

Para retomar, seguir o bloco vigente do plano vinculado acima. A lista desta primeira rodada foi superada pelos testes posteriores e pelo retorno do usuário.

TER-001 e conteúdo narrativo geral de City/Ato 1 permanecem fora deste encerramento. O load preexistente via SinalizadorDeRetorno da Bag foi apontado na revisão como outro caminho a tratar no ciclo global de UI; não foi alterado neste recorte.
