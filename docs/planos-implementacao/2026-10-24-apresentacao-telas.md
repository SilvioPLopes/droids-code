# Apresentação visual das telas

**Estado e data:** apresentação criada e validada, 24/10/2026; avaliação do usuário pendente.
**Objetivo:** PowerPoint visual, com pouco texto, mostrando telas e menus do DroidsCode. Usuário delegou a composição; usar identidade do jogo.
**Escopo permitido:** este plano e novos arquivos em `output/apresentacao-telas/`; link deste plano no índice. Fontes são imagens existentes, somente leitura. Não editar Assets, cenas, scripts, outros planos ou saves, nem operar o Unity.
**Dependências:** capturas de menu, Bag, Loja, Status, equipamentos Apollo e terminal em `output/`, documentadas nos planos específicos. São prévias de montagem; não representam captura nova de uma sessão do jogador. Preservar essa distinção nas notas da apresentação.
**Aceite:** imagens grandes, sem distorção ou cortes de controles, títulos curtos, pouco texto e identidade visual coerente. Cobrir menus disponíveis e terminal; não apresentar artes vazias como telas implementadas.
**Validação mínima:** verificar fontes de imagem, renderizar e inspecionar todos os slides, validar pacote PPTX e geometria. Sem testes de lógica do jogo.
**Antes e reversão:** nenhum arquivo existente será substituído, exceto este plano e adição de link ao índice; remover apenas os novos arquivos próprios se solicitado. Não há necessidade de backup de produto.

## Execução e retomada

- Decisão: PowerPoint 16:9, paleta escura/cobre, uma tela por slide e legendas curtas.
- Fontes encontradas: previews 1280×720 de menu, loja, Bag, Status, Apollo e terminal. Inspeção visual pendente.
- Unity: não consultado nem operado; saves e backups existentes preservados.
- Próximo passo: inspecionar imagens, criar e validar apresentação.
- Inspeção das seis fontes e seis slides realizada. Primeira validação bloqueou a entrega por fonte Aptos no pacote, apesar da intenção Segoe UI. Causa localizada na propriedade `fontFamily` do autor: a API de texto exige `typeface`. Correção delimitada a essa propriedade; antes `fontFamily:font`, depois `typeface:font`; reversão por esse trecho. Reexportar e validar novamente, preservando composição e fontes das imagens.

## Encerramento

- Criado/salvo: `output/apresentacao-telas/entrega/DroidsCode-telas-e-menus.pptx`, seis slides 16:9 com Menu, Bag, Loja, Status, Apollo e Terminal. Títulos e legendas editáveis; capturas incorporadas, sem distorcer proporção. Notas identificam fontes e dados de fixture.
- Verificado: seis renders finais inspecionados individualmente em 1280×720, sem sobreposição ou corte adicional dos controles. Validação do pacote, geometria/fontes e reimportação Artifact Tool aprovadas, zero achados. Evidência em `output/apresentacao-telas/build/validation.json`; prévias em `build/previews/`. Não aberto/testado no PowerPoint nativo.
- Recuperação adicional: finalizador precisava de `RUNTIME_NODE_MODULES`; caminho do runtime fornecido ao processo, sem instalar ou alterar dependências. Validação final concluída.
- Aprovação visual do usuário pendente. Próximo passo: usuário abrir o arquivo e avaliar a apresentação; nenhum ajuste adicional presumido.
- Unity não consultado/operado nesta tarefa. Saves, cenas, Assets e backups existentes preservados. Fontes e intermediários próprios em `output/apresentacao-telas/build/`; sem backup de produto necessário e sem limpeza pendente de dados de jogador. Sem commit/push.

## Ampliação — mapas e NPCs, 24/10

**Pedido:** imagens dos mapas e NPCs, mantendo apresentação visual e pouco texto. Ampliar a apresentação em novo arquivo e fornecer imagens separadas em ZIP.
**Escopo:** novos arquivos próprios em `output/apresentacao-telas/`, este plano. Ler mapas `metavalle.jpg`, `LipCasa.jpg`, `ferrovale.jpg`, `battle.jpg`, três retratos de NPCs em Resources e captura existente de Tácio. Preservar PPTX anterior e todos os assets originais; não operar Unity.
**Evidência:** GUIDs confirmam LipCasa em CasaLip e ferrovale em Game; contrato City identifica metavalle e três NPCs (Dara, Homem da fila, Tácio). Mapas são artes de fundo, sem atores/UI. Captura de Tácio é prévia anterior. Não misturar retratos com elenco futuro não montado.
**Aceite:** quatro cenários em imagem inteira, três NPCs identificados e imagem de Tácio no mapa; 12 slides totais com os seis anteriores preservados. Imagens disponíveis separadamente, distinguindo artes e prévia do jogo. Validar apenas slides novos, pacote final e ZIP.
**Antes/reversão:** adicionar seis slides em cópia do arquivo original; apagar apenas novos arquivos próprios se solicitado. Retratos pequenos ampliados por vizinho mais próximo para apresentação, sem redesenho.
**Próximo passo:** gerar versão ampliada, conferir slides novos e entregar arquivos.

**Resultado:** criada `output/apresentacao-telas/entrega/DroidsCode-telas-mapas-e-npcs.pptx`, 12 slides. Quatro cenários, retratos de três NPCs e Tácio no mapa adicionados. Os seis renders anteriores têm hashes idênticos aos da nova composição. Seis slides novos renderizados a partir do PPTX final e inspecionados em 1280×720; imagens inteiras, nomes legíveis, sem sobreposições. Pacote, geometria/fontes e reimportação aprovados com zero achados (`build/v2/validation.json`).

**Imagens separadas:** `entrega/DroidsCode-imagens-mapas-npcs.zip`, oito imagens e LEIA-ME; entradas e tamanhos conferidos. Quatro mapas copiados dos assets originais, três retratos ampliados sem redesenho, captura anterior de Tácio preservada. Fontes intermediárias em `imagens-mapas-npcs/`; PPTX anterior preservado.

**Retomada:** arquivos prontos para avaliação do usuário; nenhuma aprovação visual presumida, nem teste no PowerPoint nativo. Unity não consultado/operado; sem gravação em cenas, Assets ou saves. Sem backup novo de produto necessário, sem limpeza de jogador pendente e sem commit/push. Próximo passo: usuário abrir as imagens/apresentação e avaliar.

## Apresentação do enredo com imagens — 24/10

**Pedido:** criar apresentação baseada nas imagens e no texto do enredo, mantendo preferência já dada por visual e pouco texto.
**Escopo:** novo PPTX e intermediários em `output/apresentacao-telas/`, este plano; enredo/adendo/assets/previews somente leitura. Preservar os dois PPTX anteriores, todos os Assets, cenas, scripts e saves. Não operar Unity.
**Fontes:** `docs/enredo/TCC_-_ENREDO_v2.md`, recortes Mundo, Personagens, Temas e roteiro do Ato 1; `TCC_-_ENREDO_v2_ADENDO.md`, cânone e arquitetura dos três atos. Adendo prevalece: Apollo é um entre 431 núcleos; Apex faz busca ampla, sem origem única ou escolhido. Não usar a afirmação contraditória antiga de perseguição exclusiva.
**Decisão de composição:** cerca de 15 slides 16:9, sequência narrativa do mundo/dupla → crise → investigação/travessia → Ivo/consequências → Apex → três atos → programação e preparação → tema final. Pouco texto nos slides, contexto e fontes nas notas. Incluir acontecimentos do roteiro sem alegar implementação completa. Artes atuais e sprites do projeto como referências visuais; retrato antigo de Apollo em Resources é um mago e será preterido pela sprite de droid atual em Characters/Companion.
**Aceite:** história compreensível, imagem grande e texto breve, nomes/cânone corretos, atos futuros explicitamente apresentados como arco planejado. Imagens preservam proporções; não inventar cenas implementadas nem trocar nomes de assets por prova de função narrativa.
**Validação mínima:** conferir texto contra fontes, renderizar/inspecionar cada slide, validar pacote/fontes/geometria e reimportação. Sem testes do jogo ou pesquisas externas para resumir materiais fornecidos.
**Antes/reversão:** novos arquivos próprios, sem substituir entregas. Remover só esses arquivos se solicitado. Nenhum backup de produto necessário.
**Próximo passo:** produzir composição narrativa e validar apresentação final.

**Resultado:** criado/salvo `output/apresentacao-telas/entrega/DroidsCode-enredo-ilustrado.pptx`, 15 slides com mundo, Lip/Apollo, crise da água, moradores, Cratera, facções, Ivo, consequências, Nadir, três atos e relação com programação/equipamentos. Texto curto editável, imagens incorporadas e notas com contexto/fontes. Revelações do Ato 1 incluídas; atos futuros identificados como arco planejado. Seguido adendo sobre Apollo e busca ampla da Apex.

**Validação:** 15 renders inspecionados em 1280×720; após revisão de redação em 10/12/15, somente esses três foram renderizados e inspecionados novamente. Sem cortes/sobreposições observados. Finalizador aprovou pacote, geometria/fontes e reimportação com 15 slides e zero achados (`build/enredo/validation-final.json`). SHA256 final `AAE93F32F3BDF2D3E966108EFE0CBA257738C3627044864643681D0657267E6C`. Não aberto no PowerPoint nativo.

**Retomada:** avaliação do usuário pendente. Próximo passo é abrir e avaliar a versão narrativa; nenhuma edição do jogo foi necessária. Unity não consultado/operado, saves e backups preservados. Intermediários em `build/enredo/`; exportação preliminar `entrega/DroidsCode-enredo-e-universo.pptx` não é a entrega escolhida. Apresentações anteriores de telas/mapas preservadas. Sem limpeza de dados do jogador pendente, commit ou push.

**Entrega externa solicitada:** usuário não conseguiu baixar pelo link local e autorizou subir ou enviar por e-mail. Nenhuma ferramenta de e-mail/armazenamento conectada encontrada; navegador disponível sem abas/sessões de serviço abertas. Outlook Email encontrado no catálogo, ainda não instalado/conectado; integração sugerida. Nenhum upload ou e-mail realizado. Próximo passo: após conexão, conferir se suporta envio com anexo e enviar ao destinatário autorizado, ou usar destino de armazenamento disponibilizado pelo usuário. Arquivo local pronto permanece em `entrega/DroidsCode-enredo-ilustrado.pptx`; pode ser aberto diretamente no Windows sem download. Unity e saves não operados.

## Download no celular — entrega por hospedagem privada

**Esclarecimento:** usuário está no celular e precisa baixar o arquivo salvo no computador; caminho local não resolve. Pedido anterior de subir o arquivo permanece autorizado.
**Escopo antes de editar:** novos arquivos em `output/apresentacao-telas/download-mobile/`, cópia do PPTX final já validado para hospedagem, este plano. Usar Sites privado do proprietário, sem publicar todo o repositório ou dados pessoais. Página mínima com link de download. Preservar entregas anteriores, Assets, cenas e saves.
**Dependências:** serviço Sites e Git exclusivo da página; arquivo PPTX de 14,8 MB, abaixo do limite usual de asset estático. Não alterar autenticação/rede do projeto Unity.
**Aceite:** URL HTTPS implantada com botão/link para baixar PPTX, acesso privado na conta do proprietário. Conferir cópia por hash e publicação pelo retorno do serviço; diferenciar publicação de teste no celular.
**Antes/reversão:** novo diretório e Site privado, nenhuma substituição do arquivo original. Despublicação/remoção do Site somente se solicitado. Sem backup de produto necessário.
**Próximo passo:** preparar cópia de download, criar Site, sincronizar fonte e publicar privadamente.
