# Droids Code

RPG educativo com explora??o em 2D, combate por turnos e um terminal Lua para ensinar l?gica de programa??o. O jogador acompanha Lip e seu Droid Apollo, resolve puzzles e configura atributos e t?cnicas por comandos.

Projeto de TCC do curso de Sistemas para Internet da FATEC Baixada Santista ? Rubens Lara. As plataformas pretendidas s?o Windows e WebGL; a compatibilidade de builds precisa de valida??o pr?pria.

## Come?ar pelo projeto

1. Abra a pasta do reposit?rio pelo Unity Hub com **Unity 6000.5.8f1**, vers?o registrada em [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).
2. Aguarde a resolu??o dos [pacotes](Packages/manifest.json) e a compila??o. O projeto usa URP 2D, uGUI/TextMeshPro e MoonSharp.
3. Abra [MainMenu.unity](Assets/Scenes/MainMenu.unity). O bot?o **Novo Jogo** est? configurado para carregar `CasaLip`.
4. Use WASD/setas para movimento, `E` para interagir e `Esc` para o Menu do Mundo. O terminal tamb?m possui Executar e Ctrl+Enter.
5. Antes de avaliar o jogo inteiro, siga o [roteiro de valida??o](docs/guias/validacao-jogo.md) e registre o resultado. A leitura de c?digo n?o equivale a um teste de funcionamento.

## Documenta??o

| Documento | Quando ler |
|---|---|
| [Índice de trabalho](docs/indice-trabalho.md) | Retomada por assunto, caminhos e chaves de pesquisa; foco atual da demo. |
| [Revisão da CasaLip](docs/revisoes/casalip.md) | Montagem atual, flags, mecanismo da porta e validações pendentes da primeira cena. |
| [Enredo v2](docs/enredo/TCC_-_ENREDO_v2.md) e [adendo](docs/enredo/TCC_-_ENREDO_v2_ADENDO.md) | Direção narrativa; o adendo prevalece nas divergências. |
| [Contexto do projeto](docs/contexto-projeto.md) | Refer?ncia principal do estado atual, arquitetura, cenas, sistemas e decis?es. |
| [Plano de desenvolvimento](docs/plano-desenvolvimento.md) | Prioridades, depend?ncias e crit?rios de conclus?o. |
| [Montagem do Ato 1](docs/guias/montagem-ato-1.md) | Configura??o no Editor, com pr?-requisitos e limita??es atuais. |
| [Valida??o do jogo](docs/guias/validacao-jogo.md) | Percursos de teste e registro de evid?ncias. |
| [Hist?rico e migra??o](docs/historico/README.md) | Entregas anteriores, investiga??es e documentos originais preservados. |

A ordem de leitura para retomar o desenvolvimento ? **README ? contexto ? tarefa no plano ? guia ou relato relacionado**. Descri??es completas de sistemas ficam no contexto; prioridades ficam no plano.

## Situa??o da revis?o

Em 16/10/2026, a documenta??o foi reconciliada com arquivos locais e observa??es MCP desta conversa. H? sistemas implementados, conte?do em cat?logos e montagem parcial das cenas do Ato 1. Os registros distinguem c?digo, configura??o e valida??o em execu??o.

Os detalhes dos problemas conhecidos est?o no plano e nos relatos vinculados. O travamento de movimento relatado na CasaLip permanece **n?o reproduzido**.

## Licen?a

O README anterior declarava MIT, mas o arquivo `LICENSE` n?o foi localizado nesta revis?o. A declara??o e o arquivo de licen?a precisam ser confirmados pelo respons?vel antes de redistribuir o projeto com essa identifica??o.
