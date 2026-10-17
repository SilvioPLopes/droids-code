# CasaLip — implementação e validação de 17/10/2026

**Estado:** implementação aplicada; encerramento do plano ainda pendente dos casos indicados abaixo. **Ambiente:** Unity 6000.5.8f1, Windows, branch `docs/consolidar-contexto`, base `027952041d40e7957875a66641727684586455c1` com alterações locais, sem novo commit. Datas desta revisão em America/Sao_Paulo; o log técnico registra UTC.

Plano seguido: [correção da CasaLip](../planos-implementacao/2026-10-16-correcao-casalip.md). Diagnóstico original: [apresentação](../problemas/casalip-apresentacao-dialogo.md) e [retorno](../problemas/casalip-retorno-porta-trancada.md).

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

Para retomar: confirmar MCP conectado e Play parado; usar a fixture v6 para C06 preservando novamente o save do responsável; depois executar uma sessão nova para while pelo menu e diálogo ocupado; completar C08 em duas resoluções, C10 nos painéis reais e a parte de input/movimento. Registrar observações antes de marcar os critérios restantes do plano. Não repetir os diagnósticos antigos nem reimplementar etapas já aplicadas.

TER-001 e conteúdo narrativo geral de City/Ato 1 permanecem fora deste encerramento. O load preexistente via SinalizadorDeRetorno da Bag foi apontado na revisão como outro caminho a tratar no ciclo global de UI; não foi alterado neste recorte.
