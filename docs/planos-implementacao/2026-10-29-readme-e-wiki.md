# README para avaliação e wiki

**Estado/data:** README e documentos na main; páginas e sidebar da Wiki publicadas por Git, 29/10/2026. Aprovação visual remota não presumida.
**Objetivo:** adaptar o modelo anexado para o professor abrir o projeto Unity e jogar o início; preparar a wiki e verificar a possibilidade de publicação.
**Arquivos permitidos:** `README.md`, este plano e novas páginas em `docs/wiki/`. Não criar outro `readme.md`, pois o repositório já usa `README.md`.
**Preservar:** todas as alterações existentes; código, cenas, pacotes, saves e documentação de outras tarefas. Não iniciar Unity/Play nem gerar builds.
**Dependências/consumidores:** README público para avaliação; wiki complementar. Links para contratos existentes, sem duplicar andamento técnico.
**Fontes necessárias:** modelo anexado; `ProjectSettings/ProjectVersion.txt`; `EditorBuildSettings.asset`; MainMenu/City; contratos CasaLip/City; catálogo de puzzles e comandos do terminal; roteiro de montagem para identificar pendências.
**Aceite:** instruções em português para importar, abrir MainMenu, dar Play, controlar Lip, resolver água e explorar City; distinguir continuação condicional de percurso comprovado. Estrutura do modelo adaptada a Unity, sem tecnologias ou integrantes inventados. Wiki navegável e estado da publicação explícito.
**Validação mínima:** conferir comandos/nomes contra arquivos atuais, links locais e diff; não executar testes de gameplay nem afirmar aprovação visual.
**Valores anteriores/reversão:** README existente sem alterações locais no início, com apresentação/resumo e links técnicos, vários caracteres já corrompidos. Baseline disponível em `git show HEAD:README.md`. Reversão somente dos hunks desta tarefa, preservando alterações posteriores; novos documentos removíveis individualmente se solicitado.

## Execução e retomada

- Repositório: `https://github.com/SilvioPLopes/droids-code`.
- GitHub CLI não localizado; nenhuma ferramenta conectada de publicação GitHub identificada.
- Consulta Git da wiki: primeira tentativa bloqueada por rede; repetição autorizada fora do sandbox retornou `Repository not found`. Não comprova ausência da wiki: também pode ser falta de acesso. Nenhuma mutação remota realizada.
- Decisão: preparar páginas Markdown locais e instruções de inicialização/publicação; não declarar wiki publicada.
- A saída atual de City exige `ato1_sessao2` e ao menos duas ações (incluindo ataque básico). Fonte de pontos pela rota normal aparece como pendência no guia anterior; não tratar o registro antigo como teste atual.
- Usuário confirmou que o professor vai rodar no Unity. Não há executável a documentar. Nomes/responsabilidades da equipe continuam sem informação; seção sinalizada para preenchimento pelos autores.
- Unity não consultado nem alterado nesta tarefa; saves intocados. Nenhum backup de save necessário.
- Alterações salvas: README adaptado ao modelo com importação, Play, controles, solução finita da água, exploração de City, técnica condicional, troubleshooting, build e links. Cinco arquivos de wiki: Home, guia, visão técnica, publicação e sidebar.
- Verificação documental: `git diff --check -- README.md` sem erros; conferidos os destinos locais e o fechamento de blocos de código. Cena MainMenu aponta para CasaLip; City exige história e técnica; controles conferidos nos scripts. Sem teste de input, partida completa, build ou aprovação visual. Links remotos para conteúdo local novo dependem do envio da versão correspondente.
- Navegação da wiki usa URLs das páginas sem extensão; fontes técnicas permanecem no repositório principal. Referência de publicação consultada na documentação oficial do GitHub.
- Próximo passo: usuário abrir a aba Wiki com acesso de escrita e criar Home se ainda não houver; depois publicar as páginas preparadas seguindo `docs/wiki/Publicar-a-wiki.md`. Se já existir, resolver o acesso antes de substituir conteúdo. Conferir renderização remota e atualizar status só após publicação confirmada. Envio/commit do README não realizado nesta tarefa.

## Publicação solicitada

- Usuário autorizou publicar após confirmar que o README pertence à raiz do repositório.
- Branch local: `revisao/interiores-dialogos-colisoes-2026-10-29`, com upstream correspondente. Índice Git vazio antes desta etapa; alterações de outras tarefas permanecem fora do commit.
- Consulta remota confirmou `main` como padrão, no commit `30e44d8990e1fc7979842f52f51de760cca0322e`. Essa versão não contém CasaLip nem `docs/`; a branch atual tem 71 commits posteriores. Não publicar o guia na main sem apontar para a versão compatível; não integrar mudanças do jogo implicitamente.
- Destino solicitado ao usuário: branch atual com versão recente ou somente documentação na main apontando para essa versão. Preparação do commit limitada ao README, páginas da wiki e este plano.

## Retomada após integração na main — 29/10/2026

- Usuário informou ter atualizado a main e autorizou continuar a publicação nela.
- Estado local observado: branch `main`, HEAD `26ebf0af36bf9af54f8f93adb7425a6cd803ae3a`, sem alterações rastreadas. O commit `22e32ff` com README e cinco arquivos de wiki já faz parte desse histórico, seguido da integração `e603233`.
- `git ls-remote origin refs/heads/main` confirmou exatamente `26ebf0af36bf9af54f8f93adb7425a6cd803ae3a` no GitHub. Assim, README e fontes em `docs/wiki/` já estão publicados na main; a divergência de base anterior foi resolvida pelo usuário.
- Nova consulta ao repositório separado `droids-code.wiki.git` ainda retornou `Repository not found`. Não comprova se falta inicialização, habilitação ou acesso.
- Verificação pela interface não pôde prosseguir: ferramenta de navegador retornou `Local browser use is disabled in Work settings`. Não alterar configurações nem contornar essa restrição por outro meio.
- Próximo passo que depende do usuário: abrir `https://github.com/SilvioPLopes/droids-code/wiki` e criar a página Home usando `docs/wiki/Home.md`, caso ofereça criação inicial. Se a aba não existir, verificar Wikis nas configurações do repositório. Após inicialização e acesso disponível, publicar as demais páginas e sidebar seguindo o guia existente. Publicação na aba Wiki continua não confirmada.
- Nenhuma alteração no jogo, Unity, saves ou backups nesta retomada. Este registro é a única alteração documental desta etapa.

## Publicação após criação da Home pelo usuário

- Usuário confirmou criação da Home e autorizou concluir as demais páginas.
- Clone autorizado disponível em `output/wiki-publicacao`; branch padrão `master`, inicialmente apenas `Home.md`, sem alterações locais. Home remota contém o texto preparado, sem conteúdo adicional do usuário.
- Escopo desta rodada: páginas já permitidas em `docs/wiki/`, README (status da publicação), este plano e cópia da wiki em `output/wiki-publicacao`. Preservar código, cenas, saves e a primeira revisão da Home no histórico Git.
- Antes de mudar, valores anteriores: README/Home indicavam publicação pendente; guia de publicação registrava o bloqueio anterior. Reversão por hunks ou commit inverso somente se solicitada, mantendo edições posteriores. O clone preserva o commit inicial remoto, sem exclusão de arquivos.
- Validação mínima: diff somente documental, links internos/externos e correspondência do commit enviado com a referência remota. Não exige abrir navegador nem executar Unity.
- Próximo passo: copiar guia, visão técnica, publicação e sidebar; ajustar avisos de status; publicar na branch padrão da wiki e verificar o remoto antes de atualizar a main.

### Resultado da publicação

- Publicação da Wiki concluída: `628d1084ac04124be9b514e4d44aa5c0430d4bec` em `master`. Push avançou `998f8a5..628d108`; `ls-remote` confirmou o hash remoto e clone ficou limpo.
- Home preservada com duas atualizações de status; adicionados guia do professor, visão técnica, procedimento de manutenção e sidebar. Histórico inicial do usuário preservado.
- README passa a apontar para a Wiki sem aviso de publicação pendente; fontes de Home/procedimento também foram atualizadas para manutenção futura.
- Verificação: diff documental sem erros; commit remoto confirmado. Consulta web para renderização retornou `Cache miss`, portanto não há aprovação visual por navegador. Não repetir essa consulta sem evidência nova.
- Estado do Unity/saves inalterado. Clone de manutenção mantido em `output/wiki-publicacao`; nenhuma limpeza ou backup removido.
- Próximo passo: conferir visualmente a Wiki pelo navegador e preencher integrantes/responsabilidades da equipe se necessário. Atualizações futuras exigem sincronizar fontes em `docs/wiki/` e repositório separado da Wiki.
