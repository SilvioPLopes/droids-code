# Terminal ilustrado — montagem sobre a arte

## Documentação do método — 23/10/2026

**Pedido:** documentar como o terminal foi transformado na versão polida e extrair uma lógica reutilizável para outras imagens/interfaces.

**Escopo desta rodada:** criar docs/guias/interfaces-sobre-imagens.md; acrescentar links no índice e no contrato de interface; atualizar somente este registro. Nenhum script, imagem, prefab, cena, estado Unity ou save será alterado.

**Aceite:** explicar critérios e decisões que levaram ao resultado, incluindo por que o recorte não bastou; separar arte de conteúdo/interação; fornecer procedimento, ficha, prompt e validação reutilizáveis, distinguindo parâmetros do terminal de regras gerais. Evidências datadas continuam neste plano, sem copiar toda a cronologia para o guia.

**Preservação/validação:** alterações documentais aditivas; reversão somente dos acréscimos próprios. Conferir caminhos locais, consistência dos exemplos com o contrato e diff dos documentos. Sem testes Unity para esta tarefa documental.

**Resultado:** [guia reutilizável](../guias/interfaces-sobre-imagens.md) criado e ligado pelo índice/contrato. Explica escolhas e limites do caso, separação arte/apresentação/comportamento, levantamento da janela real, decisão entre recorte e nova composição, prompt, mapeamento de coordenadas, ajuste proporcional, testes e ficha para o próximo painel. Evidências completas continuam neste plano. Código e estado Unity não acessados/modificados nesta rodada; usuário se refere à versão atual como polida, sem inferir aceite de fluxos não testados por ele.

**Verificação documental:** links do novo guia, do contrato e deste plano conferidos; diff sem erros de whitespace. O índice mantém um link antigo ausente para o catálogo de personagens, fora deste escopo. Próximo passo: usar a ficha do guia na próxima interface; nenhum novo teste Unity necessário nesta rodada.

## Retomada vigente — reprovação de legibilidade e foco (23/10/2026)

**Relato:** usuário rejeitou o resultado, texto ilegível; a segunda captura é a referência de foco no monitor, sem cenário/base ocupando área. Autorizou alterar a imagem se necessário. Não continuar o recorte por fatores tentativos; a entrega anterior não tem aceite visual.

**Diagnóstico comprovado:** CasaLip em Play, terminal aberto, render 3840×2160. Código fonte virtual 24 × escala efetiva 1.875598 = 45.01 px renderizados; saída/objetivo fonte 20 = 37.51 px. Captura do usuário mostra Game View a 0.22×, logo tamanhos exibidos aproximados 9.9 e 8.25 px. A área útil estreita e tipografia definida para a arte quadrada não atende a leitura na janela mostrada. Zoom é condição de comparação, não algo a alterar para declarar correção.

**Direção revisada apresentada ao usuário:** manter estilo verde/metal e foco da segunda imagem, produzir outra arte horizontal só do monitor, borda curta, sem base/pés, áreas de código/objetivo/saída maiores. Textos e botões terão legendas reais escaláveis, sem palavras/números de demonstração impressos. Fundo opaco exclusivo do terminal elimina competição do cenário. Preservar a arte atual em arquivo separado.

**Escopo:** novo asset irmão em Assets/UI/TerminalIlustrado; ApresentacaoTerminalIlustrado.cs e helper Editor de remontagem delimitada; prefab Canvas somente ramo terminal; teste de layout/legibilidade em 1920×1080, 1280×720 e 844×475 (aproximação da Game View da captura); documentação deste plano/contrato. Não alterar Canvas/Scaler global, cenas/mundo/NPCs, backend Lua, save/flags ou diálogo. Inspecionar e preservar trabalho recente de recorte antes da mudança.

**Aceite visível:** foco no monitor como segunda imagem; sem base/cenário visíveis ao abrir; texto do código de aproximadamente 16 px ou mais em 844×475 e corpo lateral pelo menos 14 px; campos e botões sem cortes/sobreposição; objetivo/log longos roláveis; controles continuam alinhados e funcionais. Comparar também na resolução pequena, não apenas em render grande. Nova imagem em separado, atual intacta.

**Backup/reversão:** guardar prefab/scripts/renders atuais e código/saída da sessão em output/terminal-ilustrado/foco-2026-10-23/ antes de aplicar. Autorização anterior para encerrar Play/aplicar terminal já recebida; guardar sessão antes de encerrá-la, sem nova confirmação. Reverter só hunks desta rodada se solicitado, não presumir recuperação visual.

**Resultado aplicado, aceite do usuário pendente:** nova arte `Assets/UI/TerminalIlustrado/terminal-foco-horizontal.png`, 1536×1024; fontes reais código/números 40, saída 34, objetivo/pontos 32; títulos e botões agora TMP, sem palavras impressas no bitmap. Fundo exclusivo opaco ocupa tela enquanto terminal aberto; moldura sem base/pés, cenário oculto. Referência/cortes do perfil atual 1536×1024, 0/0, margem 0.98. Script compensa escala herdada sem mudar Canvas/Scaler.

**Evidência antes/depois:** teste novo em 844×475 falhou no perfil anterior com “Texto de código pequeno na janela de referência”, antes de aplicar a migração. Após mudança, passou com mínimo nominal código >=16 px e corpo lateral >=14 px; valores aproximados atuais 18.18/15.46/14.55. Render pequeno inspecionado: textos e controles identificáveis, sem cenário competindo. Objetivo longo continua rolável (não cabe inteiro em uma página), agora com indicador visível; depuração possui caixa desenhada na UI.

**Verificação:** bateria isolada Unity passou em 844×475, 1280×720 e 1920×1080: abertura menu/contexto, digitação por evento, Lua finito, sucesso/erro, limpar, depuração, rolagem e regiões de clique, retorno/fechamento. Renders em output/terminal-ilustrado/preview-*.png. São testes por API e renders de fixture, não input físico/aprovação do usuário.

**Preservação:** backup atualizado em foco-2026-10-23/backup, inclusive código/saída da sessão. Play encerrado com autorização anterior para aplicação do terminal; sem partida de teste ou gravação de save. Novo helper RefocoTerminalIlustrado aplica uma migração com guarda; não foi repetida após timeout. Helpers e runtime compilados sem erro. Nas cenas CasaLip/City/Game foi confirmado um terminal com novo perfil/arte/fontes/fundo, fechando as preview scenes sem salvar. Nenhuma cena foi editada nesta rodada.

**Diff:** comparação do prefab com backup desta rodada: zero registros alterados ou novos fora do terminal, nenhum registro removido. Serialização automática do MenuMundoManager foi reposta ao registro anterior. Hash da arte antiga permanece CF678021B5474D1A1147E83B21BC9BC13AFAA199A328343CB57CE380C08A3699. Backend TerminalUIManager/Lua, diálogos e demais painéis não editados nesta rodada. Tratar testes anteriores como históricos, sem ignorar nova rejeição.

**Geração:** ferramenta image_gen integrada, referência visual da segunda captura, saída original em `.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-3da58258-e80a-4fc4-aa70-7cebaee0d241.png`; cópia final dentro de Assets. Prompt: monitor horizontal 1536×1024 verde/latão, moldura justa sem base/cenário, painéis vazios maiores e robô pequeno, sem letras/números/scrollbars impressos; controles/tipografia reais sobrepostos. Imagem anterior intacta.

**Estado final consultado:** DroidsCode/MainMenu, fora de Play, limpa, compilação parada e sem falha; nenhum EstadoIsoladoTerminal remanescente. Diff de scripts/documentação sem erros de whitespace. Não houve commit/push nesta rodada.

**Próximo passo:** apresentar prévia pequena e usuário avaliar no jogo. Se reprovar, preservar evidência e reavaliar composição/legibilidade antes de novo ajuste; não aumentar fatores automaticamente. Registros abaixo são históricos e não comprovam aceite desta versão.

## Retomada vigente — recorte de exibição em 23/10/2026

**Pedido novo:** ampliar o terminal e aceitar cerca de 5% de corte no topo/base, sem alterar o arquivo original. É ajuste autorizado do enquadramento anterior, não tentativa de corrigir outra regressão.

**Causa/valores observados:** script encaixa o quadrado completo de 1254 com margem 0.98; margens escuras da arte consomem área útil. MainMenu fora de Play e limpa. Proposta: enquadrar a altura útil de 90% (1128.6), preservando escala uniforme, limite horizontal e coordenadas dos controles. Com margem 0.98, borda física da tela corta aproximadamente 4.08% de cada lado vertical do quadrado; janela útil configurada é 5% por lado. Ganho relativo em telas largas: 1/0.9 = 11.11%.

**Arquivos permitidos:** ApresentacaoTerminalIlustrado.cs (campos de corte e cálculo de escala/centro); ValidacaoTerminalIlustrado.cs (aceite da janela visível e controles no lugar do quadrado completo); contrato de interface e este plano. Sem alterar prefab/cenas/Canvas/Scaler, funcionamento Lua, imagem/metas ou saves. Consumidores continuam CasaLip/City/Game.

**Aceite:** arte proporcional, maior, excesso vertical cortado só nas margens; campo/objetivo/saída/botões/depuração inteiros e alinhados. Comparação visual 1920×1080 e 1280×720; propriedades de controles dentro da tela; hash da imagem igual ao inicial CF678021B5474D1A1147E83B21BC9BC13AFAA199A328343CB57CE380C08A3699. Zoom da Game View desconhecido, não alterado; previews com resolução controlada.

**Backup/reversão:** scripts e renders anteriores copiados para output/terminal-ilustrado/recorte-2026-10-23/. Reversão do enquadramento: cortes superior/inferior em zero recuperam o cálculo anterior; ou restaurar apenas os hunks próprios dos scripts. Não restaurar arquivos inteiros sobre mudanças posteriores.

**Resultado:** cálculo aplicado; cortes configuráveis em 0.05/0.05, escala uniforme e centro útil ajustado também para cortes assimétricos. Teste atualizado verifica janela útil, proporção e limites dos controles; não exige mais que margens decorativas fiquem inteiras. Compilação Unity sem falha; bateria isolada passou em 1920×1080 e 1280×720. Prévia das duas resoluções inspecionada: maior, margens cortadas, controles e moldura útil preservados. Arquivo PNG com hash idêntico ao inicial; nenhum prefab/cena foi editado por esta rodada. Interface documentada.

**Estado/limites:** durante recarga o Editor apareceu brevemente em Play; consultas posteriores o encontraram novamente em MainMenu fora de Play. Não foi enviado comando para entrar/sair de Play nem executada partida pelo agente. Testes isolados só rodaram após confirmação de fora de Play. Duas falhas transitórias de conector durante recarga superadas por consulta de estado, sem repetir mutação de montagem. Backups nesta pasta incluem scripts/renders antes do recorte; nenhum save modificado. Input físico e aprovação visual do usuário permanecem pendentes.

**Próximo passo:** usuário avaliar o novo enquadramento. Ajustes futuros usam os dois campos de corte locais; não refazer arte ou mudar Canvas global. Registros abaixo descrevem a entrega anterior.

**Estado atual:** montagem aplicada e salva fora de Play; verificação por API e renders concluída em 1920×1080 e 1280×720. Usuário autorizou encerrar Play/aplicar; retomada encontrou DroidsCode/MainMenu já fora de Play e limpa. Arte e controles implementados no prefab compartilhado; contratos globais preservados. Input físico/aprovação visual do usuário ainda não recebidos. Histórico abaixo não representa bloqueio vigente.

## Retomada vigente

**Objetivo do usuário:** abrir a imagem de terminal enviada como interface real, com campos editáveis, dados e áreas clicáveis transparentes sobre os espaços desenhados; preservar execução Lua, limpar, fechar e informações do terminal atual.

**Referências:** anexos `6801fb9a-2178-40ee-a9a5-d13caccded09/image-1.jpeg` (arte 1254×1254) e `image-2.png` (terminal atual). Arte contém números e erro de exemplo que não podem se passar por dados reais. Trabalhar numa cópia, sem sobrescrever o anexo.

**Escopo permitido:** novo asset de fundo em Assets/UI/TerminalIlustrado, script de apresentação exclusivo do terminal e apoio Editor para montar/verificar esse ramo, Assets/Prefabs/Canvas.prefab somente PainelTerminal e descendentes, overrides desse ramo em CasaLip/City/Game se comprovadamente necessários, referências do TerminalUIManager preservadas. Mudanças mínimas em TerminalUIManager somente para expor estado visual de execução/depuração, se necessárias. Este plano, contrato de interface e índice.

**Preservar:** Canvas/Scaler global, diálogo, outros menus, cena/mundo/NPCs, save e regras de puzzle/progressão. Consumidores do prefab: CasaLip, City e Game; Canvas pode persistir entre cenas. Não usar ajuste global de escala para alinhar arte.

**Aceite:** ao abrir o terminal, arte proporcional sem corte; código digitado no espaço esquerdo; objetivo e pontos reais legíveis; saída real com rolagem; Limpar/Executar/Fechar clicáveis sobre o desenho; depuração/estado sem dados fictícios impressos. Texto longo permanece acessível. Testar aberturas pelo menu e pelo contexto do puzzle sem mudar regras. Demais painéis preservados.

**Montagem proposta:** painel virtual quadrado de 1254 unidades ajustado proporcionalmente à área de tela, com escala local compensando escala herdada do Canvas; controles posicionados por coordenadas da arte. Fundos/labels duplicados do Unity ficam transparentes; texto, cursor, seleção e rolagem continuam reais. Arte sem números/erro de demonstração permite numeração e status dinâmicos.

**Estado inicial comprovado:** DroidsCode/City em Play, limpa, render 3840×2160; terminal persistente aberto. PainelTerminal 100×100 escala local (2,2,1), controles atuais 500×100, ajuda/pontos ocultos. Git status inicialmente sem alterações listadas. Usuário consultado sobre encerrar Play; não encerrar até resposta.

**Backup/reversão:** antes de qualquer alteração de montagem, copiar Canvas.prefab e cenas afetadas para output/terminal-ilustrado/backup, com hashes; guardar código/saída da sessão sem executar Lua ou salvar jogo. Reverter somente arquivos/hunks próprios, preservando posteriores. Não restaurar save: não será modificado.

**Validação mínima:** compilar; conferir referências e consumidores; render real do ramo antes/depois em 1920×1080 e 1280×720; verificar limites/campos/rolagem e regiões de clique. Testar Lua finito sem alterações de progresso e preservar menu/diálogo. Separar testes por API, input real e aprovação visual. Não executar while infinito nem testes que gravem saves sem backup.

**Andamento — primeira rodada:** arte limpa gerada pelo image_gen integrado, inspecionada e copiada para `output/terminal-ilustrado/preparo/terminal-fundo.png`; original preservado. Pedido da edição: preservar composição/labels/geometria, remover somente números 1–5, seta, frase de erro de demonstração e “Desligada” impressos. Saída original do gerador: `C:/Users/silve/.codex/generated_images/01a1650a-b48a-74a2-8177-73e5a4b06a86/exec-4d59ee3f-fe94-4c7a-a8dd-2c7db93a64a8.png`.

- Preparados em `output/terminal-ilustrado/preparo/`, ainda fora de Assets: ApresentacaoTerminalIlustrado.cs (escala local, números, luzes, checkbox); MontagemTerminalIlustrado.cs (montagem do ramo do prefab); TerminalUIManager.cs.novo (mudança mínima de estado visual/log de depuração); ValidacaoTerminalIlustrado.cs (prévia e teste isolado sem saves). Ainda não importados/compilados/executados. Não declarar integração pronta.
- Backup de Canvas.prefab e CasaLip/City/Game com hashes em `output/terminal-ilustrado/backup/`; código e saída reais preservados em `codigo-sessao.txt` e `saida-sessao.txt`, sem executar Lua. Copiar também TerminalUIManager.cs antes de aplicar a alteração de fonte.
- Inspeção delimitada encontrou overrides antigos em transforms/scrollbars do ramo terminal nas três cenas, inclusive escala local 2/4. Será preciso retirar somente overrides de apresentação desse ramo, preservando diálogo e todos os demais overrides. Confirmar alvos e diff antes de salvar.
- Última consulta: City em Play, limpa. Pedido de permissão para encerrar sessão está pendente. Nenhuma mudança de modo/cena/progresso ou importação de script foi feita; recompile em Play poderia perder estado, por isso o preparo permanece fora de Assets.
- Verificação desta rodada: arte vista, arquivos de preparo presentes e backups criados. Compilação, render da UI montada e testes de comportamento ainda pendentes. Progresso do objetivo: arte e implementação preparatória concretas; objetivo completo ainda não atingido.

**Próximo passo:** aguardar resposta ao encerramento do Play (ou constatar que usuário já saiu); confirmar projeto/cena/dirty, encerrar somente se autorizado; copiar fontes/arte preparados para Assets, compilar, executar montagem uma vez, resolver overrides do terminal por alvo delimitado, renderizar/validar e corrigir somente falhas demonstradas. Não repetir montagem após timeout sem consultar se já existe componente no prefab. Fontes de preparo podem conter erros ainda não vistos pelo compilador.

### Continuação — compilação isolada do preparo

- Rodada anterior classificada como progresso: arte, backups e fontes preparatórias concretos. Revalidação atual: DroidsCode/City ainda em Play e limpa; resposta ao encerramento continua pendente. Não houve importação/mutação de produto.
- Compilação externa com Roslyn do SDK local e referências atuais Unity, sem acionar AssetDatabase/recarga do Editor. Fontes copiadas para `output/terminal-ilustrado/compilacao-isolada/`; classe do manager renomeada somente nessa cópia para não conflitar com Assembly-CSharp. Essa checagem verifica tipos/sintaxe, não integração no prefab ou funcionamento no Editor.
- Primeiro problema de referências duplicadas resolvido por deduplicação; compilador então demonstrou erro real no helper: campoDeCodigo.textComponent é TMP_Text, e Formatar exigia TextMeshProUGUI. Assinatura corrigida no preparo para TMP_Text, sem mudar comportamento.
- Nova execução do compilador terminou exit 0; único aviso CS1701 relacionado às referências netstandard do processo isolado. Gerado PreparoTerminal.dll fora de Assets; não carregado no Unity. Compilação pelo Unity, montagem, render e testes reais continuam pendentes.
- Próximo passo permanece condicionado a sair de Play sem descartar a sessão por conta própria. Não repetir pedido de autorização nem iniciar testes de jogo enquanto a resposta estiver pendente.

### Bloqueio de execução confirmado

Na terceira rodada consecutiva com a mesma pendência, consulta atual confirmou DroidsCode/City em Play e limpa. Ainda não chegou resposta ao pedido para encerrar a sessão. Preparo e compilação isolada estão preservados; não há outra etapa de integração/validação executável sem importação ou alteração do estado do Editor. Objetivo bloqueado aguardando resposta ou saída de Play pelo usuário, não concluído. Retomar pelo próximo passo acima, sem gerar novamente a arte ou repetir a preparação já verificada.

### Execução após autorização

- Fontes e arte importadas; compilação Unity finalizada sem falha. Montagem do prefab executada uma vez. Preservados IDs/referências dos controles originais; acrescentados fundo, gutter, objetivo rolável, luzes e checkbox. Alteração do manager limitada a estado visual e log opcional de depuração.
- Removidos somente overrides de apresentação previamente inventariados: CasaLip 18, City 16, Game 19. Arquivos ainda coincidiam byte a byte com os backups antes da remoção. Cenas não foram abertas/salvas pelo Editor nessa edição de YAML.
- Comparação por registros YAML do prefab identificou serialização automática de cenasBloqueadas no MenuMundoManager, fora do ramo; reposto o registro original. Conferência posterior: nenhum registro preexistente alterado fora do terminal. Único registro antigo removido é a Mask substituída por RectMask2D da saída; IDs dos controles mantidos.
- Testes revelaram perda de acentos somente nas strings novas geradas no preparo via pipe PowerShell/Python; corrigidas em UTF-8 no fonte. Falha inicial de raycast na fixture era depth=-1 porque câmera ainda não renderizava a preview scene: câmera vinculada à cena isolada e render antes de verificar hits. Não houve ajuste dos botões por tentativa.
- Comparação visual revelou handle da scrollbar herdando 20×20 e anchors antigos; corrigido para ocupar largura do trilho (14) com tamanho proporcional ao conteúdo. Fitter/rebuild testados: saída longa se desloca entre início/fim, indicador com size entre 0 e 1.
- Testes por API passaram em 1920×1080 e 1280×720: abertura via menu, retorno ao menu, contexto do puzzle, Lua finito atingindo 100, erro Lua exibido, depuração, limpar, numeração, rolagem, regiões dos três botões e fechar sem contador residual. Isolamento com Droid/estado temporários, singleton anterior restaurado em finally; sem save.
- Preview scenes de CasaLip/City/Game conferidas e fechadas sem salvar: um terminal por cena, novo componente e todas as referências presentes; painel quadrado dentro do Canvas com escala herdada compensada. MainMenu do usuário preservada.
- Renders `output/terminal-ilustrado/preview-1920x1080.png` e `preview-1280x720.png` inspecionados: arte proporcional, código/objetivo/saída/pontos nas áreas próprias, controles sobre desenho. São renders de fixture Unity, não captura de input físico na partida. Resolução declarada nos nomes; zoom da Game View do usuário não foi alterado nem usado como causa de layout.
- Documentação de contrato/índice atualizada. Backups permanecem em output/terminal-ilustrado/backup; nenhum save lido/restaurado/gravado nesta execução. Dados do terminal da sessão anterior preservados nos arquivos iniciais; na retomada já não havia terminal vivo.

### Verificação final e entrega

- Digitação por `TMP_InputField.ProcessEvent` escreveu o caractere esperado; raycast alcançou o campo e os três botões sobre a arte. Não equivale a teclado/mouse físicos. Mesma bateria passou novamente nas duas resoluções, incluindo rolagem efetiva início/fim, size proporcional da scrollbar e ciclo menu→terminal→menu sem contador residual.
- Conferência de escopo em `output/terminal-ilustrado/conferencia-escopo.json`: zero registros preexistentes alterados fora do terminal, zero registros novos fora desse ramo; única remoção é Mask antiga da saída. Os 53 overrides de cena retirados são exclusivamente os inventariados. Botões/campo/saída/pontos/ajuda preservam referências.
- Whitespace novo de serialização normalizado somente no ramo permitido. Dois `m_Name: ` herdados em registros externos idênticos ao backup podem aparecer como linhas adicionadas em `git diff --check` por realocação de documentos YAML; preservados para não editar outro painel. Diff de scripts/documentação sem erros de whitespace. Não declarar check global do prefab inteiramente verde por isso.
- Final Unity: DroidsCode/MainMenu, fora de Play, limpa, sem compilação em andamento e sem falha de compilação. Nenhum EstadoIsoladoTerminal remanescente. A única preview scene ainda existente é CustomLightsScene da Scene View (SceneLight), pertencente ao Editor; cenas de teste fechadas, não removê-la.
- Sem gravação de save nem transição de jogo durante a validação. Backups e código/saída anteriores permanecem em output/terminal-ilustrado/backup. Arquivos finais usados pelo projeto estão em Assets; preparo antigo em output é histórico e não deve sobrescrever as correções posteriores.

**Auditoria do objetivo:** imagem usada como fundo persistente no terminal das três cenas; campos e áreas clicáveis transparentes alinhados ao desenho; código editável e numeração; execução/limpeza/fechamento funcionam; objetivo/ajuda, saída e pontos usam dados reais; rolagem mantém texto longo acessível; arte preserva proporção em duas resoluções; outros ramos do Canvas e regras/saves preservados. Evidência: fontes/prefab/cenas atuais, consultas dos consumidores, teste isolado Unity e renders inspecionados. Não foi exigida aprovação visual prévia para aplicar a montagem; avaliação do usuário no jogo permanece uma etapa de uso/aceite, sem alegação de já recebida.

**Próximo passo vigente:** usuário abrir terminal no jogo e avaliar o resultado visual. Se houver rejeição/regressão, preservar evidência e diagnosticar antes de outro ajuste. Não refazer arte/montagem nem repetir baterias aprovadas sem mudança relevante. Não há implementação pendente do escopo deste terminal; bugs de Salvar/Carregar/avisos continuam no plano próprio.
