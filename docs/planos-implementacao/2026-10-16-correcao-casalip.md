# Plano de implementação — Correções da CasaLip

> **Para execução:** usar `superpowers:executing-plans` para implementar e verificar uma etapa por vez nesta conversa. `superpowers:subagent-driven-development` é alternativa somente se escolhida pelo responsável. As caixas abaixo acompanham a execução; nenhuma correção está concluída pela criação deste documento.

**Nome do plano:** Correções da CasaLip — Diálogos, Sprites e Persistência.

**Data:** 16/10/2026, America/Sao_Paulo.

**Estado em 17/10/2026:** implementação aplicada; validação final parcial. Veja [resultados e pendências observados](../revisoes/casalip-validacao-2026-10-17.md). O processo ainda não atende a todos os critérios de encerramento.

**Base de referência:** `027952041d40e7957875a66641727684586455c1`, branch `docs/consolidar-contexto`, com alterações documentais e configuração local já existentes.

**O que este plano tenta corrigir:** a janela de diálogo da CasaLip ficou pequena e deformada após alterações em âncoras e escalas; Apollo ainda usa uma sprite e um retrato de mago, com o nome distante do personagem; Lip usa o corpo-base sem as novas roupas; a vitória do puzzle aparece como texto isolado em vez de conversa do catálogo; e, ao retornar de City, a porta nasce trancada e o terminal perde sua referência. A flag `ato1_casa_resolvida` também é gravada pela conversa introdutória, misturando conversar com resolver o puzzle.

**Objetivo:** deixar a CasaLip legível e visualmente coerente, com sucesso apresentado pelo diálogo de Apollo e conclusão preservada em revisitas e carregamento.

**Arquitetura:** manter a porta e o controlador do puzzle locais à cena, recuperando a conclusão do GerenciadorDeEstado. Reutilizar VerificadorDePuzzle e CatalogoDeDialogos, associando-os à UI persistente válida. O terminal e o menu devem controlar suas próprias aberturas/fechamentos sem duplicar bloqueios de movimento.

**Tecnologias:** Unity `6000.5.8f1`, C#, uGUI, TextMeshPro, MoonSharp e MCP do Unity.

**Especificação e evidências:** [análise das seis capturas](../problemas/casalip-apresentacao-dialogo.md), [revisão da casa](../revisoes/casalip.md), [relato da revisita](../problemas/casalip-retorno-porta-trancada.md), [guia de montagem](../guias/montagem-ato-1.md) e [validação](../guias/validacao-jogo.md). Este plano detalha `UI-003`, `VIS-001`, `UI-001/TER-002`, `INT-001/HIS-001` e a parte de `HIS-002` relativa ao sucesso da água no [plano principal](../plano-desenvolvimento.md).

## Restrições gerais

- Trabalhar por partes na demo, começando pela CasaLip. City entra somente nas referências visuais de Lip e nos testes de ida/volta; seu conteúdo narrativo fica para outro trecho.
- Alterar cenas fora de Play Mode, pelo MCP, e salvar cada etapa. Preservar `.meta`, componentes, tag, colisores e referências do Player.
- Novos nomes de domínio em português; não reorganizar namespaces nem substituir sistemas fora deste recorte.
- Usar os assets disponíveis e manter créditos/licenças. O Apollo LPC é provisório; arte robótica definitiva e novas ilustrações não estão incluídas.
- Manter o critério numérico atual: `nivel_agua >= 100`. A atribuição direta continua aceita; este plano não implementa análise de uso de `while`.
- Não executar intencionalmente laços infinitos. A proteção `TER-001` continua necessária antes de uma validação aberta do tutorial com iniciantes.
- Preservar o save usado pelo responsável antes dos testes que gravam progresso. Registrar qual sessão e arquivo foram usados.
- Não marcar uma tarefa do plano principal como concluída apenas por completar sua parte na CasaLip.

## Decisões técnicas propostas

As escolhas abaixo foram aplicadas, com dois ajustes registrados durante a execução: Apollo parado usa o atlas idle indicado pelo responsável e a migração preserva o progresso legado.

| Assunto | Decisão |
|---|---|
| Porta | Recriar normalmente e reaplicar a conclusão; não usar DontDestroyOnLoad na porta |
| Flag do puzzle | Manter `ato1_casa_resolvida`, gravada somente pela resolução |
| Flag da introdução | Usar `ato1_falou_apollo_casa` ao concluir `casa_intro_apollo` |
| Sucesso | Usar `CatalogoDeDialogos.CasaPortaApollo`, Id `casa_porta_apollo`, sem copiar as falas para o checker |
| Apollo provisório | Usar `Characters/Companion/Apollo/animations/idle.png`, quadro frontal parado `idle_4`, identificado como placeholder LPC |
| Retratos provisórios | Associar as sprites novas de Apollo e Lip em `retratosConhecidos`, com proporção preservada; manter os PNGs antigos para histórico |
| Lip em movimento | Criar controlador próprio com as animações LPC e os mesmos parâmetros `MoveX`, `MoveY` e `Speed` |
| Interface | Referência de UI `1280 × 720`, Scale With Screen Size, Match `0.5`; diálogo na parte inferior |
| Saves antigos | Versão nova `6`; conservar `ato1_casa_resolvida=true` legado e acrescentar `ato1_falou_apollo_casa=true`, sem apagar progresso |

A proposta inicial de remover a conclusão legada foi substituída após conferir o save real e o pedido de preservar progresso. O formato v5 não distingue introdução de resolução: conservar a flag evita repetir o puzzle, mas também libera a passagem de saves antigos que só tinham a conversa. Inventário, atributos, técnicas, Gold, contadores e outras flags permanecem intactos. Saves novos separam introdução e resolução.

## Foco de revisão e casos que precisam de prova

1. Duas resoluções de tela: janela ampla, retrato encaixado e texto legível — etapas 1 e 6.
2. UI inicialmente inativa e Canvas duplicado na revisita: achar a instância válida e manter o primeiro diálogo aberto — etapas 3 e 4.
3. E durante a digitação, abertura repetida e fechamento por origens diferentes: preservar a VM e devolver o contador ao valor correto — etapa 3.
4. Save antigo com flag da conversa e load após resolução nova: distinguir introdução de conclusão sem apagar outros dados — etapas 4 e 6.
5. Evento repetido, revisita ou load: liberar a porta sem repetir recompensa, save de vitória ou conversa de sucesso — etapas 4, 5 e 6.

## Arquivos e responsabilidades

| Arquivo ou grupo | Responsabilidade |
|---|---|
| `Assets/Scenes/CasaLip.unity` | Layout, etiqueta, sprites, flags e controlador local |
| `Assets/Scenes/City.unity` | Referências visuais do Player para manter Lip coerente na saída e volta |
| `Assets/sprites/Atualizados/Tela_de_dialogo-removebg-preview.png.meta` | Retângulo importado da moldura, se necessário para remover margens transparentes; preservar o PNG |
| `Assets/Animations/CasaLip/Lip/` — novo | Controlador e oito clips de Lip |
| Importadores `.meta` de `Characters/Protagonist/sprite.png`, `Characters/Protagonist/animations/walk.png` e `Characters/Companion/Apollo/sprite.png`, dentro de `Assets/sprites/Entities/Npc's/` | Filtro, proporção e Pixels Per Unit consistentes entre pose inicial e animações |
| `Assets/Scripts/Scripting/PlayerMovement.cs` | Opção de espelhamento compatível com animações de esquerda/direita distintas |
| `Assets/Scripts/Menus/TerminalUIManager.cs` | Abertura única, inicialização, log e contexto do puzzle |
| `Assets/Scripts/Menus/MenuMundoManager.cs` | Origem do painel e liberação do bloqueio do menu |
| `Assets/Scripts/Historia/DialogoUIManager.cs` | Inicialização segura, acesso à UI inativa e estado de abertura |
| `Assets/Scripts/Scripting/LocalizadorDeTerminal.cs` — novo | Localizar o terminal vivo, inclusive inativo e persistente |
| `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs` | Associação, restauração e evento de resolução nova |
| `Assets/Scripts/Casa/InteragirComTerminalCasa.cs` | Recuperar referência e respeitar painel já aberto |
| `Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs` — novo | Coordenar contexto e conversa de sucesso da casa |
| `Assets/Scripts/Estado/DadosDoJogo.cs` e `SalvamentoJson.cs` | Versão 6 e migração específica da flag antiga |
| `Assets/Editor/ValidacaoCasaLip.cs` — novo | Asserções de regressão chamadas pelo MCP, sem dependência de NUnit ou nova asmdef |

O `PuzzleAguaCasaChecker` será removido da instância da casa durante a migração. Seu arquivo não será excluído neste recorte. O catálogo de diálogos já fornece as falas necessárias.

## Etapa 1 — Recuperar a janela de diálogo (`UI-003`)

**Arquivos:** CasaLip.unity; importação da moldura indicada acima. **Dependência:** nenhuma.

**Resultado:** moldura, nome, retrato, texto e avanço ocupam uma única janela legível.

- [ ] Registrar capturas de referência em `1280 × 720` e `1920 × 1080`, incluindo uma fala longa. Confirmar a falha atual antes de alterar valores.
- [ ] Configurar o Canvas principal com Scale With Screen Size, referência `1280 × 720` e Match `0.5`. Conferir também os outros painéis desse Canvas após a mudança.
- [x] Reconstruir PainelDialogo: âncoras inferiores `(0.1, 0)` a `(0.9, 0)`, pivot `(0.5, 0)`, posição `(0, 24)` e escala um. Determinar a altura pela proporção visível da moldura, sem esticar o desenho.
- [x] Ajustar o retângulo importado da moldura aos limites visíveis pelo Sprite Editor, se as margens transparentes impedirem o encaixe. Preservar o PNG e atualizar a referência da Image para o sprite importado correto.
- [x] Criar `AreaRetrato` e `AreaTexto` como filhos da janela; encaixar ImagemRetrato na janela desenhada da moldura. Preservar proporção e impedir que a silhueta de fundo fique exposta ao lado do retrato.
- [x] Habilitar TextoFalante, usar fonte `24` na referência de UI e escala um. TextoFala usa fonte `24`, escala um, Overflow `Page` e margens dentro de AreaTexto. Remover margens negativas e largura negativa herdadas.
- [ ] Restaurar escala um e área clicável do BotaoAvancar, dentro da janela. Manter escolhas no painel próprio e conferir a paginação.
- [ ] Repetir as capturas nas duas resoluções. Aprovar somente com texto sem corte, nome visível, retrato encaixado e botão acessível. Salvar a cena.

## Etapa 2 — Corrigir personagens e etiqueta (`VIS-001`)

**Arquivos:** CasaLip.unity, referências visuais do Player em City.unity, PlayerMovement.cs e assets novos em `Assets/Animations/CasaLip/Lip/`. **Dependência:** etapa 1 para validar retratos.

**Interfaces:** manter `MoveX`, `MoveY` e `Speed`; adicionar `public bool usarEspelhamentoHorizontal = true` a PlayerMovement. A opção conserva o comportamento dos controladores antigos; fica falsa nas instâncias que usam o novo controlador LPC.

- [ ] Registrar as referências e a altura visual de Player/Apollo antes da substituição. Conferir a importação das sprites novas com filtro Point e quadros de `64 × 64`.
- [x] Associar o quadro frontal parado `idle_4` de `Assets/sprites/Entities/Npc's/Characters/Companion/Apollo/animations/idle.png` ao Apollo da casa, preservando sua interação. Registrar o caráter provisório dessa arte.
- [x] Criar `LipCasaLip.controller` e os clips `ParadoCima.anim`, `ParadoBaixo.anim`, `ParadoEsquerda.anim`, `ParadoDireita.anim`, `AndarCima.anim`, `AndarBaixo.anim`, `AndarEsquerda.anim` e `AndarDireita.anim` na pasta de animação indicada.
- [x] Usar `Characters/Protagonist/animations/walk.png`: quatro linhas de nove quadros, ordem cima/esquerda/baixo/direita; quadro zero para parado, quadros 1–8 a `10 fps` para andar. Manter deslocamento em oito direções com orientação pelas quatro artes disponíveis.
- [ ] Associar o novo controlador e sprite inicial ao Player em CasaLip e City. Antes de mudar o script, registrar esquerda/direita com o controlador LPC e o flip antigo, para verificar o espelhamento duplicado.
- [ ] Condicionar o flipX em PlayerMovement à opção de espelhamento; com a opção falsa, manter flipX falso. Calibrar Pixels Per Unit de sprite.png e animations/walk.png de Lip de forma consistente, preservando a altura visual de referência, transform e física existentes.
- [x] Em DialogoUIManager.retratosConhecidos, associar `Apollo` e `Lip` às respectivas sprites novas, com Image.preserveAspect ligado. Confirmar que a prioridade manual substitui o retrato de mago carregado por Resources.
- [x] Converter `Apollo/Canvas` para World Space. Dimensionar a etiqueta para aproximadamente `1 × 0.25` unidades de mundo e posicioná-la `0.15` unidade acima dos bounds do SpriteRenderer. Manter a etiqueta como filha de Apollo, com nome centralizado.
- [ ] Validar parado, quatro direções e diagonais, ida/volta para City, etiqueta com movimento da câmera e retratos alternando entre Apollo/Lip. Confirmar componentes/referências preservados; salvar cenas e assets.

## Etapa 3 — Tornar o terminal e o fechamento de UI consistentes (`UI-001/TER-002`)

**Arquivos:** TerminalUIManager.cs, MenuMundoManager.cs, DialogoUIManager.cs e InteragirComTerminalCasa.cs. **Dependência:** independente da arte; necessária antes da conversa automática de sucesso.

**Interfaces produzidas:** `TerminalUIManager.EstaAberto` e `DialogoUIManager.EstaAberto`, propriedades bool somente leitura; `public void MenuMundoManager.FecharParaDialogo()`. Manter Abrir(), Fechar(), AoFechar e as duas sobrecargas existentes de DialogoUIManager.Mostrar.

- [ ] Reproduzir `VAL-04`: abrir pelo mundo, chamar Abrir novamente, fechar e conferir o contador; testar a origem Esc → Terminal e a primeira abertura do diálogo. Registrar as falhas antes da correção.
- [x] Tornar a inicialização de terminal/diálogo idempotente, chamada antes da primeira abertura. Os botões devem receber seus listeners uma vez; Start não pode fechar o painel recém-aberto. A busca de DialogoUIManager deve incluir objetos inativos.
- [x] Fazer Abrir retornar imediatamente quando o terminal já estiver logicamente aberto: preservar Runner, código e contador. Ativar o painel antes de escrever o log/iniciar a coroutine. Fechar libera o bloqueio uma vez e continua notificando AoFechar.
- [x] Guardar separadamente se o Menu do Mundo está logicamente aberto e se abriu o painel atual. AoFecharOutroPainel só reapresenta o menu quando foi a origem da abertura. Esc fecha o terminal aberto pelo mundo sem abrir um menu por trás.
- [x] Implementar FecharParaDialogo: esconder o menu, liberar apenas o bloqueio adquirido por ele e limpar sua origem/callback de painel atual. O terminal ainda fecha pelo próprio Fechar; não zerar globalmente o contador para esconder vazamentos.
- [x] Fazer a interação da casa respeitar MenuAberto/EstaAberto, evitando que E digitado no editor ou usado no diálogo reabra o terminal.
- [x] Em AoClicarCarregar, encerrar o menu logicamente antes de chamar Carregar. A UI persistente não pode permanecer visualmente aberta enquanto o load zera o contador; conferir o estado também após a cena salva carregar.
- [ ] Repetir `VAL-04` com digitação contendo `e`, aberturas repetidas, fechamento por botão/Esc e diálogo inicialmente inativo. Esperado: mesmo Runner na abertura repetida, primeira fala preservada, contador zero ao fechar tudo e nenhum erro novo de coroutine. Conferir Status/Bag/Droid por compartilharem o menu.

## Etapa 4 — Corrigir conclusão, associação e revisitas (`INT-001/HIS-001`)

**Arquivos:** LocalizadorDeTerminal.cs, VerificadorDePuzzle.cs, InteragirComTerminalCasa.cs, DadosDoJogo.cs, SalvamentoJson.cs, CasaLip.unity e ValidacaoCasaLip.cs. **Dependência:** etapa 3 para validar interação e contador.

**Interfaces produzidas:**

- `public static TerminalUIManager LocalizadorDeTerminal.Encontrar()` — inclui inativos, prefere a UI persistente válida e retorna null quando não há candidato válido.
- `public void VerificadorDePuzzle.AssociarTerminal(TerminalUIManager novoTerminal)` — remove a inscrição anterior, limpa seu estado e assina a nova uma vez.
- `public bool VerificadorDePuzzle.Resolvido { get; }` e `public event Action AoResolverPelaPrimeiraVez`.
- `public bool VerificadorDePuzzle.mostrarFalaAoResolver = true` — mantém o comportamento dos outros puzzles; a casa usa false.
- `public static void SalvamentoJson.MigrarDados(DadosDoJogo dados)` — migração em memória, sem acesso ao disco.
- Harness Editor: `public static string ValidacaoCasaLip.ValidarEstadoDaCasa(bool resolvidaEsperada)`, `ValidarMenus(int quantidadeEsperada)` e `ValidarMigracaoCasaLip()`; retornar `OK` apenas com todas as asserções satisfeitas, lançar InvalidOperationException em falha.

- [ ] Criar as asserções de regressão antes de corrigir a lógica. A primeira versão do harness usa tipos já existentes e Resources.FindObjectsOfTypeAll, sem depender das interfaces novas ainda ausentes. Depois do percurso antigo de revisita, `ValidarEstadoDaCasa(true)` deve falhar pela porta/referências, não por falta de compilação; registrar esse resultado. Inspecionar também DontDestroyOnLoad, sem criar estado por acidente.
- [x] Implementar o localizador compartilhado. Na interação e no verificador, recuperar a referência quando nula/destruída, inclusive após o descarte do Canvas duplicado. O controlador local permanece ativo enquanto aguarda associação; evitar inscrições duplicadas e limpar o estado de inscrição mesmo se a referência anterior foi destruída.
- [x] No VerificadorDePuzzle, reaplicar os efeitos a partir de flagQueJaResolve sem salvar, progredir quest ou emitir AoResolverPelaPrimeiraVez. Na resolução nova, gravar flag/progresso/save uma vez e só então emitir o evento. A fala direta respeita mostrarFalaAoResolver.
- [x] Criar `ControlePuzzleAgua` local e ativo em CasaLip, anexar VerificadorDePuzzle com `idDoPuzzle=p_agua_casa`, porta local, `flagAoResolver=ato1_casa_resolvida`, `flagQueJaResolve=ato1_casa_resolvida` e `mostrarFalaAoResolver=false`. Remover PuzzleAguaCasaChecker do PainelTerminal e desativar o TextoApollo de vitória antigo.
- [x] No gatilho introdutório de Apollo, usar `flagAoTerminar=ato1_falou_apollo_casa` e a mesma chave em flagsQueImpedem. Manter `idDoDialogo=casa_intro_apollo`, falas manuais vazias e apenasUmaVez ligado.
- [x] Alterar DadosDoJogo.versao de `5` para `6`. Antes de restaurar flags em Carregar, MigrarDados trata versões menores que 6: flag antiga verdadeira é preservada e acrescenta introdução verdadeira; outras flags e dados ficam intactos. Atualizar a versão em memória; versões 6 ou superiores não passam por essa conversão. Ajuste de política documentado na revisão de 17/10.
- [x] Validar a migração com DTOs em memória: v5 com flag antiga verdadeira e outro progresso; v5 sem a flag; v6 com puzzle resolvido; segunda migração do mesmo objeto. Esperado: conversão idempotente, preservação dos outros dados e conclusão v6 mantida.
- [ ] Executar `VAL-02/03`: conversa sozinha, variável ausente, variável não numérica, valor 90, valor 100, solução finita com while, duas revisitas e load. Usar sessões separadas para conferir a primeira conclusão por atribuição direta e por laço. Esperado: flag correta, nova porta destrancada, terminal vivo, um verificador local e nenhum checker antigo anexado.

Chamadas de verificação pelo MCP, usando `script-execute` em modo corpo:

```csharp
return ValidacaoCasaLip.ValidarEstadoDaCasa(true);
```

```csharp
return ValidacaoCasaLip.ValidarMigracaoCasaLip();
```

O primeiro comando depende de uma sessão em Play com o puzzle resolvido. A migração usa somente dados de teste em memória.

## Etapa 5 — Apresentar o sucesso pelo catálogo (`HIS-002`, recorte CasaLip)

**Arquivos:** novo ControladorPuzzleAguaCasa.cs, TerminalUIManager.cs e CasaLip.unity. **Dependências:** etapas 1, 3 e 4; etapa 2 fornece retratos coerentes.

**Interfaces:** o controlador recebe o VerificadorDePuzzle local e consome AoResolverPelaPrimeiraVez, AssociarTerminal, FecharParaDialogo e DialogoUIManager.Mostrar. Adicionar ao terminal `public void DefinirContextoPuzzle(string idDoPuzzle)` e `public void LimparContextoPuzzle(string idDoPuzzle)`, sem alterar Abrir/Fechar.

- [ ] Registrar a falha atual: resolver exibe somente TextoApollo e não apresenta as quatro falas de `casa_porta_apollo`.
- [x] Anexar ControladorPuzzleAguaCasa a ControlePuzzleAgua. Assinar/desassinar o evento local; aguardar a associação válida antes de configurar o contexto `p_agua_casa` no terminal.
- [x] Usar o contexto para apresentar o enunciado/código inicial do CatalogoDePuzzles. Na primeira abertura sem rascunho, o campo inicia com `nivel_agua = 0` e o comentário do catálogo; não sobrescrever código em digitação. Ao sair da casa, limpar somente esse contexto e restaurar a apresentação geral do terminal.
- [ ] Na resolução nova, executar FecharParaDialogo, fechar o terminal e chamar Mostrar com `CatalogoDeDialogos.Obter(CatalogoDeDialogos.CasaPortaApollo)`. Se já houver diálogo aberto, aguardar sua conclusão sem sobrescrever o callback existente. A coroutine termina se o controlador local for destruído.
- [x] Não duplicar as falas em campos do Inspector nem alterar o texto do catálogo para reproduzir a frase antiga. Restaurar uma casa já resolvida não dispara esse fluxo.
- [ ] Validar a sequência com abertura por E e pelo menu: terminal fecha, menu fica oculto, as quatro falas aparecem na ordem, o contador fica em um durante a conversa e volta a zero ao terminar. Reexecutar, revisitar e carregar não repetem o sucesso.

Fluxo esperado:

```mermaid
flowchart LR
    A[Executar Lua] --> B{nivel_agua >= 100}
    B -- Não --> C[Continuar no terminal]
    B -- Sim, primeira vez --> D[Gravar conclusão e liberar porta]
    D --> E[Fechar terminal e menu]
    E --> F[Diálogo casa_porta_apollo]
    G[Voltar ou carregar CasaLip] --> H{Conclusão salva}
    H -- Sim --> I[Restaurar porta sem repetir conversa]
    H -- Não --> J[Manter puzzle disponível]
```

## Etapa 6 — Verificar a entrega e atualizar os registros

**Arquivos:** [revisão da casa](../revisoes/casalip.md), [relatos](../problemas/casalip-apresentacao-dialogo.md), [contexto](../contexto-projeto.md), [guia](../guias/montagem-ato-1.md), [plano principal](../plano-desenvolvimento.md) e este documento. **Dependência:** etapas anteriores.

- [x] Aguardar a recompilação do Unity após as mudanças e separar erros de scripts dos avisos de conexão MCP. Nenhum erro novo de compilação pode permanecer.
- [ ] Executar a matriz abaixo, registrando ambiente, save, ações, esperado/observado, responsável e evidência. Distinguir chamada por API de componentes de input real do jogador.
- [ ] Conferir dados e referências após cada revisita e após reiniciar Play Mode/carregar. Recarregar apenas a cena não substitui o teste de reinício.
- [ ] Salvar cenas e assets, revisar diff e confirmar que os arquivos alterados pertencem a este plano. Executar `git diff --check -- Assets docs` sem erros de whitespace.
- [ ] Atualizar documentação com resultados observados e base Git. Fechar somente critérios demonstrados; falhas restantes ficam registradas com reprodução.

| Caso | Ação | Esperado |
|---|---|---|
| C01 | MainMenu → CasaLip, conversar sem resolver | Introdução verdadeira, conclusão falsa, porta trancada |
| C02 | Executar variável ausente/texto/90; depois solução finita até 100 | Somente o alvo numérico conclui; uma aplicação de efeitos |
| C03 | Abrir terminal repetidamente e digitar texto com E | Runner preservado na abertura repetida; nenhum bloqueio acumulado |
| C04 | Resolver pelas duas origens de abertura | Quatro falas do catálogo; menu/terminal ocultos; contador final zero |
| C05 | CasaLip → City → CasaLip duas vezes | Porta destrancada, terminal válido e introdução/sucesso sem repetição |
| C06 | Salvar resolvido, encerrar Play, reabrir e carregar | Conclusão preservada; porta restaurada sem repetir recompensa/fala |
| C07 | Migrar fixture v5 e carregar save v6 | Progresso legado conservado conforme política revisada; dados não relacionados preservados; saves novos distinguem conversa e resolução |
| C08 | Diálogos nas duas resoluções e com narração/fala longa | Sem deformação, retratos fora da moldura ou texto cortado |
| C09 | Lip parado/em movimento e ida/volta de City | Arte nova mantida, orientação correta, física/referências preservadas |
| C10 | Primeiro diálogo com UI inicialmente inativa; abrir/fechar outros painéis | Primeira fala permanece; menu e bloqueios seguem a origem correta |

## Critério de encerramento do plano

A CasaLip apresenta personagens e diálogos legíveis; o sucesso usa o catálogo; conversar não conclui o puzzle; a passagem e o terminal funcionam após duas revisitas e após reinício/load; nenhuma abertura deixa Lip bloqueado. Isso encerra este conjunto de correções, sem declarar todo o Ato 1 ou toda a demo concluídos.

## Registro de execução

| Etapa | Estado | Data, commit e evidência |
|---|---|---|
| 1 — Layout | Aplicado; visual final parcial | 17/10, base 0279520 + alterações locais; captura 1280 inspecionada; faltam 1920/longa/narração |
| 2 — Personagens | Aplicado; conferência parcial | 17/10, idle Apollo, oito clips Lip; 4 direções/4 diagonais por API aprovadas |
| 3 — Ciclo da UI | Aplicado; regressões por API aprovadas | 17/10, fixtures de abertura, diálogo inativo, contexto e origem OK; falta input real/painéis |
| 4 — Estado e revisitas | Aplicado; duas revisitas aprovadas | 17/10, flag/porta/referências/hash de save; migração em memória OK; falta load em sessão reiniciada |
| 5 — Sucesso do catálogo | Aplicado; origem mundo/API aprovada | 17/10, quatro falas, evento único, contador 1→0; falta menu/while e espera por diálogo ocupado |
| 6 — Validação final | Em andamento; interrompida | Registros na revisão de 17/10; save original restaurado; Computer Use interrompido por Esc |
