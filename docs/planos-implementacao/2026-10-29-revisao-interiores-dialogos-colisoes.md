# Revisão dos interiores: diálogos, colisões e circulação

## Ordem autorizada e escopo

Pedido do usuário: validar TerminalDroidApi, salvar e fazer commit em nova branch **antes** da análise. Depois registrar os defeitos relatados e separar correções imediatas de trabalho futuro. Não corrigir colisores/portais por tentativa; o usuário fará ajustes pontuais. Não gerar imagens novas.

Checkpoint permitido: assets/cenas/prefabs de jogo e documentação da montagem atual, incluindo mapas já presentes. Preservar alterações locais do conector, Packages e ProjectSettings globais fora do commit. Excluir ferramentas baixadas, backups, anexos e candidatos não aprovados. Referências/GUIDs existentes preservados. Branch de origem: `arte/city-unificada-4k`; HEAD inicial: `a4a25fb`.

Validação prévia: projeto C:/Users/silve/DroidsCode/Assets, City limpa, fora de Play, sem compilação em curso/falha. TerminalDroidApi sem diff contra HEAD; implementação de equipamentos já no commit bf84290. Validar somente comandos equipar/desequipar e rollback em fixture temporária, sem IO de save e sem percorrer o jogo. Salvar assets pendentes; cena limpa não exige resserialização.

Reversão: checkpoint Git preserva a montagem anterior à análise. Nesta etapa só este documento será acrescentado; diagnóstico posterior será documental/read-only até existir causa confirmada e escopo de correção delimitado. Não reverter trabalho alheio.

## Sintomas relatados pelo usuário

- Retrato do NPC ausente na caixa de conversa.
- Opções de resposta com botões brancos pequenos e apresentação inadequada.
- Colisores sobre áreas que deveriam permitir circulação; casas com soft lock.
- Portais fora dos pontos corretos.
- Prédios acessam apenas o primeiro andar; escadas com direção funcional contrária ao desenho.

Capturas do usuário são evidência visual do diálogo sem retrato/opções e do posicionamento do Player junto ao sofá no hall. Cena exata, causa geométrica e resolução efetiva ainda precisam de identificação; posição do personagem sozinha não comprova qual collider bloqueia. O relato de soft lock invalida qualquer presunção de circulação concluída baseada na checagem estrutural anterior.

## Aceite e validação mínima

Documento deve identificar causa confirmada versus hipótese, alvos por arquivo/cena, risco e ordem de correção. Retrato precisa corresponder ao interlocutor; escolhas devem ter leitura/hierarquia e enquadramento adequados, sem mudar o Canvas/Scaler global. Circulação exige caminho chegada → interação → saída com volume dos pés do Player considerado; portais coerentes com o desenho das escadas e chegada fora de triggers/bloqueios. Testes estruturais não equivalem a input real/aprovação visual. Sem Play ou gravação de progresso nesta rodada.

## Estado

Validação atual de TerminalDroidApi passou por API no Editor: equipar/listar via Lua, erro Lua desfaz inventário/peça, desequipar devolve estoque, ID/slot inválidos recusados. Fixture temporária encerrada e singleton restaurado em finally; zero gravações de save, sem Play. AssetDatabase.SaveAssets executado; City já estava limpa. Não houve alteração nova no fonte TerminalDroidApi.

Checkpoint em preparação na branch `revisao/interiores-dialogos-colisoes-2026-10-29`. Análise dos interiores começa somente após concluir o commit. Os sintomas relatados continuam pendentes.
