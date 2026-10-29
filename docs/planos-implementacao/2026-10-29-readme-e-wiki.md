# README para avaliação e wiki

**Estado/data:** edição local concluída; publicação da wiki pendente, 29/10/2026.
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
