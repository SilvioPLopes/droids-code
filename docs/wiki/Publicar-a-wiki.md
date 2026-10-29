# Publicar a wiki

Esta pasta contém páginas prontas em Markdown. A aba **Wiki** do GitHub utiliza um repositório Git separado; enviar estes arquivos ao repositório principal não publica a wiki automaticamente.

## 1. Inicializar no GitHub

Com uma conta que tenha permissão de escrita no projeto:

1. Abra [a wiki do Droids Code](https://github.com/SilvioPLopes/droids-code/wiki).
2. Caso apareça a opção de criar a primeira página, crie a página **Home**.
3. Cole o conteúdo de `Home.md` desta pasta e clique em **Save Page**.
4. Se a aba Wiki estiver desabilitada, um administrador deve habilitar **Wikis** nas configurações do repositório, quando disponível para esse repositório e plano.

Se a wiki já tiver conteúdo, preserve as páginas existentes e compare antes de substituir arquivos com o mesmo nome.

## 2. Publicar as demais páginas pela interface

Para uma publicação sem terminal, use **New Page**, crie cada título abaixo e cole o conteúdo do arquivo correspondente:

| Título | Arquivo nesta pasta |
|---|---|
| Home | `Home.md` |
| Guia-do-professor | `Guia-do-professor.md` |
| Visao-tecnica | `Visao-tecnica.md` |
| Publicar-a-wiki | `Publicar-a-wiki.md` |

Salve cada página. Para a navegação lateral personalizada, use a opção de adicionar uma barra lateral e cole `_Sidebar.md`.

## 3. Alternativa: publicar por Git

Depois de existir a primeira página, execute na raiz do projeto:

```powershell
git clone https://github.com/SilvioPLopes/droids-code.wiki.git output/wiki-publicacao
```

Se `output/wiki-publicacao` já existir, confira seu estado antes de reutilizar; não execute o clone sobre uma pasta existente.

Antes de copiar, compare as páginas que já estiverem na wiki. Em uma wiki recém-inicializada, copie as páginas preparadas:

```powershell
Copy-Item -Path docs/wiki/*.md -Destination output/wiki-publicacao/
git -C output/wiki-publicacao status --short
git -C output/wiki-publicacao diff
```

Revise o conteúdo. Depois faça o commit somente destas páginas e publique:

```powershell
git -C output/wiki-publicacao add -- Home.md Guia-do-professor.md Visao-tecnica.md Publicar-a-wiki.md _Sidebar.md
git -C output/wiki-publicacao commit -m "Documenta execução e avaliação inicial do Droids Code"
git -C output/wiki-publicacao push origin HEAD
```

Use a branch padrão da wiki, obtida pelo clone normal. O GitHub publica as páginas enviadas à branch padrão.

## 4. Conferir a publicação

Abra novamente a aba **Wiki**, confira as páginas, os links e a barra lateral. Só então altere o status de publicação no README e na Home. O README do repositório principal também precisa ser enviado na branch usada para a entrega.

**Diagnóstico desta preparação:** a consulta Git retornou `Repository not found`. Essa mensagem não distingue uma wiki ainda não inicializada de um problema de acesso; não houve publicação remota nesta etapa.

Referência: [documentação oficial do GitHub sobre páginas da wiki](https://docs.github.com/en/communities/documenting-your-project-with-wikis/adding-or-editing-wiki-pages).
