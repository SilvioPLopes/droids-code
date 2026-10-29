# Visão técnica

## Tecnologias

Unity 6000.5.8f1 e C# compõem o jogo. O terminal executa Lua por meio de MoonSharp. A apresentação usa recursos 2D do Unity, URP, uGUI e TextMeshPro. Os pacotes estão declarados em `Packages/manifest.json`.

## Organização das cenas

| Cena | Papel |
|---|---|
| `MainMenu` | Entrada do jogo; Novo Jogo carrega CasaLip |
| `CasaLip` | Introdução com Apollo e desafio do gerador de água |
| `City` | Exploração de Ferrovale, conversas, comércio e acesso a interiores |
| `Game` | Mapa externo/vale |
| `Battle` | Interface e execução dos combates por turnos |
| `Interiores/` | Ambientes internos de Ferrovale |

Essa organização descreve os arquivos existentes; não certifica que todos os percursos narrativos estejam completos.

## Fontes de referência

Os documentos abaixo pertencem ao repositório principal. Decisões e pendências permanecem nas fontes técnicas, evitando cópias divergentes na wiki.

- [Contexto e arquitetura](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contexto-projeto.md).
- [Índice por assunto](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/indice-trabalho.md).
- [Interface](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contextos/sistemas/interface.md).
- [Estado e salvamento](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contextos/sistemas/estado-e-save.md).
- [Combate](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/contextos/sistemas/combate.md).
- [Enredo](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/enredo/TCC_-_ENREDO_v2.md) e [adendo](https://github.com/SilvioPLopes/droids-code/blob/HEAD/docs/enredo/TCC_-_ENREDO_v2_ADENDO.md).

Links para arquivos locais novos ou modificados só refletirão esta entrega no GitHub depois do envio da versão correspondente.
