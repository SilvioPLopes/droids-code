# Ampliação da matriz de usabilidade e organização da entrega

**Estado e data:** em execução, 14/11/2026. Estrutura aprovada pelo usuário nesta conversa; execução direta autorizada.
**Objetivo:** preservar a matriz anterior e acrescentar catálogo de 60 tarefas fundamentadas no jogo atual, com planejamento de gestão vinculado por IDs e documentação legível no GitHub.
**Arquivos permitidos:** este plano e arquivos novos em `docs/entrega/`. Fonte original que o usuário colocar nessa pasta será somente lida; nunca sobrescrita. Nenhum objeto Unity autorizado.
**Dependências somente de leitura:** controladores em Assets/Scripts, guia roteiro-100-por-cento.md, plano da apresentação jogável e arquivos de planilha fornecidos. Nenhum consumidor de gameplay será modificado.
**Preservar:** alteração preexistente em ProjectSettings/ProjectSettings.asset; todos os demais arquivos, saves e configurações. Nenhum teste de gameplay nesta tarefa documental.
**Aceite:** XLSX editável, catálogo com IDs únicos, instruções ao participante e resultado observável, aba de gestão com prioridade/dependências/aceite, documento Markdown e CSV de apoio. Não inventar execução, participantes, datas ou aprovação visual do usuário.
**Validação mínima:** conferir 60 IDs, contagem por frente, dependências e fontes existentes; recalcular/resgatar XLSX, conferir filtros e campos editáveis, renderizar as abas criadas quando suportado. Conferir diff e incluir no commit somente arquivos desta entrega.
**Valores anteriores e reversão:** arquivos novos não existiam antes desta rodada; remover apenas os novos arquivos desta entrega reverte a criação. Se a matriz original ficar disponível, gerar arquivo ampliado separado e comparar os valores originais antes/depois. Não reverter trabalho alheio.

## Decisões aprovadas

- Catálogo por dez frentes: menu 5, exploração 8, diálogos 6, missões 8, terminal 7, inventário 6, loja 6, combate 7, persistência 4, encerramento 3.
- Gestão em aba própria, vinculada aos IDs da matriz; campos de responsável e prazo ficam a definir quando não informados.
- Resultados dos novos testes começam como Não executado. Evidência histórica automatizada não equivale a teste de usabilidade com participantes.
- Usuário autorizou também subir os documentos ao repositório. Quadro GitHub Projects é etapa dependente de conexão e capacidade confirmadas; não inventar importação CSV automática.

## Execução e retomada

- Repositório local confirmado: origin https://github.com/SilvioPLopes/droids-code.git.
- Matriz conhecida: Matriz_Teste_Usabilidade_Droids_Code.xlsx; leitura remota não expôs células, transferência por terminal bloqueada por rede. Usuário orientado a salvar em docs/entrega. Não afirmar preservação dentro de arquivo ampliado até leitura/comparação efetivas.
- GitHub não estava instalado/conectado nas ferramentas disponíveis; integração sugerida. Nenhuma mudança remota confirmada.
- Próximo passo: produzir catálogo/gestão e integrar matriz original se disponibilizada; validar arquivos e publicar somente este escopo se a conexão permitir.
- Unity não acessado; nenhum save ou backup de jogo necessário.

## Resultado dos arquivos

- Criados em docs/entrega: Matriz_Ampliada_DroidsCode.xlsx (abas Matriz ampliada e Gestão), catálogo CSV, gestão CSV e Plano_Usabilidade_e_Gestao_DroidsCode.md.
- Verificação executada: 60 tarefas US e 14 entregas GT, IDs únicos, fontes presentes, dependências US válidas; CSVs relidos com contagens corretas, 64 links locais existentes e ZIP/XML do XLSX íntegros. Duas tabelas, filtros e validações de entrada exportados.
- Fórmulas recalculadas: 60 tarefas, zero executadas e Sem sessões. Alteração temporária para um sucesso atualizou contagem/taxa a 1/100%; estado Não executado restaurado antes da exportação. XLSX reimportado e 60 IDs recuperados.
- Prévias das duas abas renderizadas e inspecionadas: título, cabeçalho e primeiras linhas legíveis. Não houve abertura no Excel nativo nem aprovação visual do usuário. Evidência técnica e prévias em output/matriz-2026-11-14/; gerador e intermediários permanecem fora do lote Git.
- Limite da entrega: complemento pronto; matriz anterior ainda não disponibilizada localmente, portanto não foi incorporada nem comparada quanto a duplicidades. Próximo passo dessa parte é receber original em docs/entrega e gerar arquivo consolidado separado, preservando abas e valores anteriores.
- Publicação bloqueada: git add dos cinco arquivos específicos falhou com Permission denied ao criar .git/index.lock. Nenhum commit ou push foi realizado; não solicitar escalada indisponível nem contornar a restrição. GitHub ainda sem instalação/conexão confirmadas, nenhum quadro/cartão remoto criado. Próximo passo: concluir integração autorizada e verificar suas capacidades para publicar os arquivos e organizar Projects.
- Estado ao encerrar: arquivos da entrega salvos e verificados; matriz original e acesso remoto pendentes. Unity/save não acessados e alteração preexistente de ProjectSettings preservada.

## Retomada autorizada — publicação e backlog

Usuário confirmou instalação/autorização do GitHub e solicitou publicar documentos, fazer commit e organizar entregas desenvolvidas/backlog. Permissões de escrita no repositório confirmadas pelo conector. Publicar os arquivos por Git Data API preservando a árvore base, sem force e com verificação do SHA esperado; não alterar metadados Git locais bloqueados. Escopo adicional: novo resumo de entregas/backlog em docs/entrega e issues no repositório, identificadas por IDs estáveis. Encerrar como concluídas apenas entregas sustentadas pelos planos recentes, mantendo limitações de teste no corpo. Backlog antigo será organizado como proposta, não como defeito atual comprovado. Nenhuma alteração de gameplay/Unity/save. Quadro Projects depende das capacidades de navegador efetivamente disponíveis; sua eventual indisponibilidade não impede publicar os documentos e issues.

### Resultado confirmado da retomada

- Quatro documentos publicados na main no commit b5bc6e21be35dc8aba1cd940011954719f16f6f7; update de ref sem force e com SHA anterior esperado. Diretório remoto consultado e quatro SHAs conferidos. Nenhum arquivo existente foi removido ou substituído fora desse lote.
- Criadas e verificadas 29 issues: #1–#12 fechadas como entregas de implementação, com os limites dos testes nos corpos; #13–#29 abertas com prioridades P1/P2/P3, aceite e fonte documental. Nenhum responsável nominal, prazo ou teste humano inventado. Visão por entrega no novo docs/entrega/README.md.
- Conector disponível não expõe mutações de Projects. Navegador interno chegou à página Projects do repositório, mas estava sem login; nenhuma criação de quadro foi realizada. O Chrome não apareceu como navegador conectado no inventário, apesar da autorização relatada. Não repetir tentativas nativas que já falharam por verificação de URL.
- Próximo passo externo específico para o quadro: disponibilizar sessão autenticada no navegador controlável ou capacidade de escrita em Projects. Documentos e issues já estão publicados, independentemente desse bloqueio.
- Pendente separada: original da matriz ainda não disponível em docs/entrega; complemento publicado sem alegar consolidação. Unity, saves e alteração preexistente de ProjectSettings continuam preservados. Git remoto avançou por API; metadados Git locais não foram atualizados porque a pasta .git permanece sem escrita.
