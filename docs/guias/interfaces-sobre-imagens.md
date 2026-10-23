# Interfaces sobre imagens — método a partir do terminal

Procedimento para transformar uma referência visual em uma interface utilizável no Unity, mantendo campos, botões e dados reais. Aplicável a terminal, inventário, loja, status e painéis semelhantes. O resultado deve funcionar e ser legível no tamanho em que a pessoa joga.

O caso de referência é o terminal polido em 23/10/2026. [Plano e evidências](../planos-implementacao/2026-10-22-terminal-ilustrado.md); [contrato técnico vigente](../contextos/sistemas/interface.md#terminal-ilustrado). Esses documentos guardam os resultados e parâmetros atuais; este guia guarda a lógica para repetir o trabalho.

## 1. O que aprendemos no terminal

| Decisão | Motivo | Regra que pode ser reaproveitada |
|---|---|---|
| Primeiro reaproveitar os controles Unity sobre a arte | A imagem já sugeria espaços de código, saída e botões | Aparência nova deve reutilizar as funções existentes sempre que possível |
| Retirar números e erro de demonstração impressos | Confundiam decoração com estado real | Conteúdo variável pertence à UI, não ao bitmap |
| Tentar recorte não destrutivo nas margens | Parte da tela era consumida por preto/base decorativa | Recorte serve quando o problema é margem; não resolve sozinho proporção e tipografia inadequadas |
| Reavaliar após a captura rejeitada | Um render grande parecia aceitável, mas a janela real mostrava letras pequenas | Aprovação de propriedades ou teste por API não comprova legibilidade |
| Usar a segunda captura como referência de foco | O usuário queria o monitor, com pouca distração em volta | Identificar o que a referência pede em termos de área útil e hierarquia |
| Criar arte horizontal, sem base e sem letras | A composição quadrada limitava a área útil; letras desenhadas não se ajustavam | Projetar o fundo para acomodar o conteúdo, com tipografia real e independente |
| Aumentar fontes, reorganizar áreas e ocultar o cenário enquanto aberto | O problema envolvia leitura e competição visual | Foco é uma decisão de composição; ampliar a imagem não substitui dimensionar texto |
| Verificar também em janela pequena | A condição de uso era menor que o render de origem | A menor condição relevante deve fazer parte do aceite |

A versão horizontal usa 1536×1024 como referência. Esse valor é uma escolha deste painel, não um padrão obrigatório para outras interfaces. O ganho decisivo veio da combinação de composição, tipografia e controles reais.

## 2. Separar três responsabilidades

| Parte | Responsabilidade | Exemplos no terminal |
|---|---|---|
| Arte | Material, moldura, textura, identidade visual e áreas de apoio | Metal, tela verde, papel, robô decorativo |
| Apresentação | Proporção, posições, fontes, contraste, máscaras, rolagem e foco | Campos sobre o desenho, letras maiores, fundo opaco do painel |
| Comportamento e dados | O que a ação executa e de onde vem a informação | Lua, objetivo atual, pontos, saída, limpar/fechar, callbacks |

Texto de botão, título, número, mensagem, estado de execução, checkbox e indicador de rolagem devem ser elementos reais quando precisarem mudar, escalar ou responder ao usuário. Um controle pode ter fundo transparente; texto, cursor, seleção e indicação de foco precisam continuar perceptíveis.

No refoco do terminal, a execução Lua e as fontes dos dados foram preservadas. Uma mudança de aparência não autoriza reescrever lógica, salvar progresso, mudar flags ou corrigir sistemas vizinhos.

## 3. Levantar a referência e a condição de uso

Antes de editar, registrar no plano da tarefa:

1. Imagem original, versão desejada e comentário do usuário sobre o que importa nela.
2. Painel/objetos/arquivos permitidos; cenas e prefabs que os consomem.
3. Resolução renderizada, tamanho da janela exibida e zoom da Game View quando conhecido.
4. Canvas/Scaler efetivo, escalas herdadas, tamanho dos campos e fontes atuais.
5. Conteúdo real mais longo que precisa caber ou ser acessível por rolagem.
6. Backup, valores anteriores, reversão e resultado visível que servirá de aceite.

No terminal, a captura mostrava Game View a 0.22×. A fonte nominal renderizada de aproximadamente 45 px chegava à prévia com cerca de 10 px; textos laterais chegavam com cerca de 8 px. Esse dado sustentou a troca de composição e tipografia. Não mudamos o zoom para fazer a entrega parecer melhor.

Para estimar o tamanho nominal visto:

```text
tamanho na prévia ≈ tamanho nominal renderizado × fator de exibição da prévia
```

O tamanho nominal não é a altura exata de cada glifo. Em Canvas com câmera ou World Space, a projeção também importa. A estimativa orienta o diagnóstico; a comparação visual decide se a leitura funciona.

## 4. Escolher o tipo de alteração

| Evidência | Caminho adequado |
|---|---|
| Só existem margens decorativas excessivas; conteúdo já legível | Recorte de exibição, máscara ou janela útil, preservando a imagem |
| Áreas estão bem distribuídas, mas letras são pequenas | Ajustar fontes reais e conferir quebra/rolagem, sem reduzir fonte automaticamente para caber |
| Proporção da arte limita leitura e espaço dos controles | Criar uma composição adequada à área disponível |
| Muitas palavras/estados estão desenhados no fundo | Produzir fundo limpo e transferir esses elementos para a UI |
| Cenário compete com um painel que exige concentração | Usar fundo de foco exclusivo desse painel, se fizer sentido para o fluxo |
| Arte é boa, mas clique/cursor não acompanha o desenho | Corrigir mapeamento, camadas e regiões interativas; não refazer a imagem por isso |

Se o usuário rejeitar uma versão, preservar a captura e revisar a causa antes de outro ajuste. Não continuar aumentando percentuais por suposição. As tentativas anteriores não viram regras universais.

## 5. Preparar a arte para receber a interface

- Preservar o original e gerar uma versão irmã. Não substituir arquivo, GUID ou `.meta` existente sem autorização.
- Definir proporção e regiões úteis antes de gerar: edição, informação secundária, saída, ações e estado.
- Manter textura discreta atrás da leitura e contraste apropriado para texto claro/escuro.
- Pedir espaços vazios para conteúdo; evitar letras, números, código de exemplo, erro fictício, cursores, luzes de estado e handles de scrollbar impressos.
- Conservar a identidade que o usuário quer: material, cores, moldura e elementos decorativos úteis. Detalhes que consomem área de leitura precisam justificar seu espaço.
- Inspecionar a imagem gerada. Coordenadas solicitadas ao gerador são intenção; a montagem deve usar as posições realmente produzidas.
- Salvar o asset final dentro do projeto. Arquivo existente somente no diretório do gerador não é entrega integrada.

### Modelo de prompt

```text
Referência: [imagem e características a preservar].
Destino: fundo para interface Unity com controles reais sobrepostos.
Proporção/resolução pretendida: [valores desta interface].
Enquadramento: [somente painel, margens, decoração permitida].
Regiões: [função, tamanho relativo ou coordenadas desejadas de cada área].
Leitura: fundo discreto, contraste, espaço para fontes dimensionadas para [janela mínima].
Não desenhar: textos, números, exemplos de estado, cursor ou controles variáveis.
Preservar: [identidade visual]. Não acrescentar: [elementos indesejados].
```

No terminal, o image_gen integrado produziu um fundo separado, horizontal, verde/latão, só do monitor. Os títulos e botões foram montados depois como TMP. Usar esse procedimento não obriga gerar outra imagem quando um ajuste local já satisfaz o aceite.

## 6. Mapear e montar sobre a imagem

Montar uma tabela por painel:

| Área da arte | Elemento Unity | Dados/ação existente | Texto/foco/rolagem necessários |
|---|---|---|---|
| [região] | [InputField/TMP/Button/ScrollRect/Toggle] | [referência/método] | [requisitos] |

Escolher uma referência virtual `Rw × Rh` igual à composição usada. Para coordenadas medidas a partir do canto superior esquerdo, uma convenção prática é:

```text
anchorMin = anchorMax = (0, 1)
pivot = (0, 1)
anchoredPosition = (x, -y)
sizeDelta = (largura, altura)
```

Reaproveitar componentes, referências, listeners e callbacks. Tornar transparentes apenas fundos/legendas duplicadas; manter superfícies de clique válidas, cursor, seleção, foco e estados perceptíveis. Reservar espaço para scrollbars, sem cobrir o texto. Testar conteúdo vazio, curto, longo e com erro.

Dimensionar o conjunto proporcionalmente dentro da área disponível:

```text
fator de encaixe = min(largura disponível / Rw, altura disponível / Rh) × margem
```

Se houver recorte autorizado, usar a janela útil no cálculo. Compensar a escala herdada do painel para não multiplicar um aumento já aplicado pelo Canvas. Não distorcer uma imagem para preencher a tela nem modificar o Scaler global como atalho.

No terminal, [ApresentacaoTerminalIlustrado](../../Assets/Scripts/Menus/ApresentacaoTerminalIlustrado.cs) faz essa apresentação local. [RefocoTerminalIlustrado](../../Assets/Editor/RefocoTerminalIlustrado.cs) contém o mapeamento concreto. São exemplos a consultar; não anexar o componente a outro painel nem repetir a migração sem adaptar escopo, referências e aceite.

## 7. Validar no tamanho que importa

| Verificação | O que comprova |
|---|---|
| Compilação e referências | A montagem é carregável e aponta para os componentes esperados |
| Render grande e pequeno | Enquadramento, contraste, hierarquia e tamanho nominal no cenário escolhido |
| Digitação, clique e rolagem por API | Controles reais recebem eventos e executam o caminho esperado |
| Abrir/fechar e retorno à origem | Foco, callbacks e bloqueios de menu permanecem consistentes |
| Input físico e avaliação do usuário | Experiência real e aceite visual, que as verificações anteriores não substituem |

No caso do terminal, foram usados 1920×1080, 1280×720 e 844×475. O limite nominal de 16 px para código e 14 px para corpo lateral foi definido para esse caso. Não copiar esses números como garantia universal: fonte, contraste, distância e conteúdo também afetam a leitura.

Comparar antes/depois nas mesmas condições. Não aprovar uma versão só porque os campos ficaram dentro da tela. Se o texto longo não couber, garantir rolagem perceptível ou rever a composição; não escondê-lo ou achatá-lo para passar no teste.

Conferir consumidores compartilhados e diff: outros painéis, Canvas/Scaler, cenas, backend e arquivos originais devem permanecer preservados conforme o escopo. Testes que gravem saves exigem backup e restauração verificada. Ao montar por ferramenta, não repetir mutação após timeout sem consultar o estado.

## 8. Ficha reutilizável para a próxima imagem

Copiar para o plano da tarefa, sem criar um processo paralelo:

```text
Painel e objetivo:
Referência visual / características a preservar:
Problema observado e evidência:
Resolução, janela exibida e zoom conhecido:
Escopo permitido / consumidores / itens preservados:
Decisão: reaproveitar, recortar, reorganizar ou criar outra arte — motivo:
Arte original / nova versão / referência virtual:
Tabela de regiões → controles → dados/ações:
Tipografia e contraste na menor condição relevante:
Conteúdo longo / rolagem / foco / retorno ao fechar:
Backup / valores anteriores / reversão:
Aceite visível e comportamento esperado:
Resultado: editado / salvo / testado por API / input físico / aprovado pelo usuário:
Pendências, estado Unity, backups e próximo passo:
```

Guardar arquitetura no contrato do painel, procedimento neste guia e evidências/estado no plano da tarefa. Depois de uma etapa comprovada, retomar do próximo ponto pendente; não gerar a arte nem remontar controles de novo apenas para conferir o resultado.
