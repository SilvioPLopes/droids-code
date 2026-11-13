# Auditoria de scripts C# — BLOCO 05: Menus, interiores, casa, câmera e verificadores

**Escopo:** leitura estática integral dos 15 arquivos abaixo. Consultei a auditoria geral e os relatórios dos BLOCOS 01–04 como baseline. Não executei testes, jogo, compilação ou Unity/MCP; não alterei nem criei arquivos existentes. Este relatório é o único arquivo criado nesta etapa.

## 1. Resumo por arquivo — leitura integral

| Arquivo | Linhas | Leitura integral | Síntese |
|---|---:|---:|---|
| `Assets/Scripts/Menus/ApresentacaoTerminalIlustrado.cs` | 121 | Sim | Escala/layout local, estado visual, depuração e numeração de linhas. |
| `Assets/Scripts/Menus/ApresentacaoStatusIlustrado.cs` | 209 | Sim | Monta e atualiza ficha, atributos, energia, técnicas e escalas. |
| `Assets/Scripts/Menus/ApresentacaoMenuMundoIlustrado.cs` | 88 | Sim | Reutiliza botões existentes, aplica rótulos/layout e escala. |
| `Assets/Scripts/Menus/ApresentacaoLojaIlustrada.cs` | 120 | Sim | Layout/produtos, seleção, mensagens locais e recusa de venda. |
| `Assets/Scripts/Menus/ApresentacaoDroidIlustrado.cs` | 187 | Sim | Consulta equipamento, estoque e técnicas; não instala/remove peças pela UI. |
| `Assets/Scripts/Menus/ApresentacaoBagIlustrada.cs` | 196 | Sim | Grade paginada, descrição rolável, orientação para equipamento e ação. |
| `Assets/Scripts/Menus/LojaUIManager.cs` | 276 | Sim | Abas, compra/venda, validação de item, feedback e atualização da lista. |
| `Assets/Scripts/Menus/TerminalUIManager.cs` | 430 | Sim | Ciclo do terminal, runner, histórico, log, contexto de puzzle e evento de execução. |
| `Assets/Scripts/Interiores/TerminalDoInterior.cs` | 42 | Sim | Interação por proximidade/tecla e associação do puzzle ao terminal. |
| `Assets/Scripts/Interiores/MoradorInterativo.cs` | 170 | Sim | Seleção do morador próximo, diálogo, quests, loja e visita. |
| `Assets/Scripts/Casa/PuzzleAguaCasaChecker.cs` | 109 | Sim | Verifica variável Lua da Casa, atualiza fala e destranca porta. |
| `Assets/Scripts/Casa/InteragirComTerminalCasa.cs` | 75 | Sim | Trigger e tecla para abrir o terminal da Casa; bloqueio de reentrada no mesmo frame. |
| `Assets/Scripts/Casa/ControladorPuzzleAguaCasa.cs` | 87 | Sim | Associa verificador/contexto e apresenta diálogo de sucesso sem substituir conversa ativa. |
| `Assets/Scripts/Camera/CameraFollow.cs` | 65 | Sim | Segue alvo com suavização e limites ortográficos opcionais. |
| `Assets/Scripts/Verificadores/VerificadorDePuzzle.cs` | 299 | Sim | Assina o terminal, avalia condições, aplica efeitos, registra progresso e salva. |

Na pasta `Menus`, não foram incluídos os sete scripts fora do escopo expresso: `TelaDeStatusManager.cs`, `TelaDeDroidManager.cs`, `MenuMundoManager.cs`, `MainMenuManager.cs`, `GameOverManager.cs`, `BalaoDaLoja.cs` e `BagUIManager.cs`.

## 2. Estado por sistema

- **Terminal:** conecta UI, `DroidScriptRunner`, API Lua, catálogo de puzzles, verificadores e eventos. Cria novo runner em cada abertura.
- **Puzzle:** `VerificadorDePuzzle` consome o evento de execução do terminal, avalia condições, registra progresso e salva. Tentativas com erro de sintaxe/execução não chegam ao evento que o verificador assina.
- **Casa/interiores:** controladores locais conectam terminal, contexto/verificação do puzzle, diálogo, moradores e estado de cena. A coexistência dos verificadores depende da montagem não inspecionada.
- **Apresentadores:** scripts `Apresentacao*` aplicam layout e adaptam managers existentes. Leitura de scripts não comprova o resultado visual nem os valores serializados em cenas.
- **Câmera:** `CameraFollow` segue alvo e pode usar limites ortográficos, dependendo da configuração da cena, não lida nesta etapa.

## 3. Achados novos

### 🟢 Baixa — tentativas de puzzle que falham na execução não contam para a dica

- **Arquivos/linhas:** [TerminalUIManager.cs:70–73, 283–312](../../Assets/Scripts/Menus/TerminalUIManager.cs); [VerificadorDePuzzle.cs:16–17, 69–71, 140–147, 209–214](../../Assets/Scripts/Verificadores/VerificadorDePuzzle.cs).
- **Evidência:** `AoExecutarCodigo` é emitido somente quando `resultado.Sucesso`; o contador de dicas do verificador está associado ao evento e tentativas são incrementadas quando as condições avaliadas não foram satisfeitas.
- **Causa/impacto:** erros de sintaxe/execução não chegam ao verificador; uma dica prevista após N tentativas pode demorar ou nunca aparecer para quem está errando a execução. Execuções bem-sucedidas que não resolvem o puzzle contam normalmente.
- **Sugestão (não aplicada):** distinguir término de execução e sucesso; contar tentativas segundo o contrato e avaliar vitória somente quando a execução for bem-sucedida. A proposta permanece no plano `01-terminal-e-puzzles.md:106–108`.

### 🟢 Baixa — pega da scrollbar da descrição da Bag mantém tamanho fixo

- **Arquivos/linhas:** [ApresentacaoBagIlustrada.cs:108–116](../../Assets/Scripts/Menus/ApresentacaoBagIlustrada.cs); comparação com [ApresentacaoStatusIlustrado.cs:84–91](../../Assets/Scripts/Menus/ApresentacaoStatusIlustrado.cs).
- **Evidência:** a pega da Bag é criada com tamanho fixo `12×60` e atribuída à scrollbar sem zerar offsets nem preparar âncoras proporcionais. O Status faz essa normalização; o plano documenta que offsets fixos somavam tamanho à pega proporcional.
- **Causa/impacto:** em descrições longas, a pega da Bag pode não acompanhar corretamente o tamanho proporcional de rolagem ou exceder a trilha. A rolagem do texto ainda usa `ScrollRect`.
- **Sugestão (não aplicada):** aplicar à pega da Bag a configuração de âncoras/offsets já usada no Status; validar visualmente, sem alterar os demais painéis.

## 4. Achados prévios distintos — não repetidos como novos

- **I-02 — preço de venda:** permanece no código. A venda lista `item.Preco` ([LojaUIManager.cs:191, 196](../../Assets/Scripts/Menus/LojaUIManager.cs)) e credita `item.Preco` ([LojaUIManager.cs:235](../../Assets/Scripts/Menus/LojaUIManager.cs)), enquanto o catálogo define `PrecoDeVendaReal` ([CatalogoDeItens.cs:77–78](../../Assets/Scripts/Catalogos/CatalogoDeItens.cs)). Não duplicado como novo achado.
- **I-03 — atualização de quests automáticas:** permanece como pendência de integração ampla, já tratada no baseline e no BLOCO 04. O caminho de conversa de `MoradorInterativo.cs:73–78` atualiza quests, mas isso não prova integração global com toda mudança de flag/cena.
- **B-02 — registro histórico do log/painel do terminal:** a condição descrita como antiga não deve ser reapresentada como defeito atual; `TerminalUIManager.cs:179–220` ativa o painel antes do log de abertura. Input e aparência não foram revalidados.
- **I-01** (progresso por derrota) e hipóteses visuais **H-02/H-03** do baseline não foram reabertos nem resolvidos por este escopo.

## 5. Divergências com documentação

- `docs/contextos/sistemas/interface.md:4` diz que abertura repetida preserva sessão bem-sucedida. Porém `TerminalUIManager.cs:179–187` cria novo `DroidScriptRunner` a cada abertura; comentário de `VerificadorDePuzzle.cs:23–28` também descreve reset de variáveis ao fechar/reabrir. Parece divergência documental, não defeito confirmado; a intenção precisa ser esclarecida antes de alterar código ou contrato.
- `docs/planos-implementacao/melhorias/01-terminal-e-puzzles.md:17` afirma que `AoExecutarCodigo` ocorre após erro; o código atual só o invoca no sucesso. A proposta posterior PUZ-01 (`:103–108`) quer evento/contexto e contagem por execução. Não presumir que a proposta já foi implementada.
- O plano da loja prevê recusa local de venda proibida (`docs/planos-implementacao/2026-10-23-loja-ilustrada.md:3, 5–13`), comportamento coerente com o balão implementado. A divergência de preço é achado prévio.

## 6. Acoplamentos e intenções ⚪

- `VerificadorDePuzzle` e o legado `PuzzleAguaCasaChecker` são consumidores possíveis do evento; a proposta documental pede conferir coexistência e evitar efeitos duplicados. Sem cenas/prefabs, não se confirma que ambos estejam ativos na mesma montagem.
- Moradores conectam gatilhos narrativos, quests, loja, estado e `SceneManager`; apresentadores dependem de managers e referências da UI. `CameraFollow` depende do alvo/configuração da cena.
- ⚪ A Bag orientar equipamento para o terminal, em vez de equipar por botão, é intencional (`docs/contextos/sistemas/interface.md:28–38`).
- ⚪ A recusa local para itens não vendáveis é intencional; sua presença na lista, por si só, não é defeito.
- ⚪ O verificador da Casa e o genérico reutilizam o terminal existente, contrato descrito em `docs/contextos/cenas/casalip.md:5–10`.

## 7. Limitações

Análise estática apenas. Não inspecionei cenas, prefabs, valores do Inspector, configuração efetiva de colliders/câmera, nem executei input, Unity, testes ou compilação. Conclusões condicionadas à montagem não comprovam que referências/componentes estejam ativos em runtime.

## 8. Próximo passo

BLOCO 05 concluído. Próximo na sequência: BLOCO 06 — scripts `Assets/Editor/Montagem*.cs`, `RefocoTerminalIlustrado.cs` e inspeção textual dirigida de cenas/prefabs de UI para a análise de responsividade.
