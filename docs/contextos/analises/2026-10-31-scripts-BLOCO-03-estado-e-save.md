# Auditoria de scripts C# — BLOCO 03: Estado e save

**Escopo:** leitura estática integral dos scripts próprios de `Assets/Scripts/Estado/` e de `SalvamentoJson.cs`. A busca limitada em `Assets` encontrou um único `SalvamentoJson.cs`. Não executei testes, compilei, abri Unity, rodei o jogo ou usei MCP. Nenhum arquivo existente foi alterado.

**Baseline:** consultei os relatórios [Auditoria geral — 30/10](2026-10-30-auditoria-geral-projeto.md), [BLOCO 01 — Combate](2026-10-31-scripts-BLOCO-01-combate.md) e [BLOCO 02 — Droid e progressão](2026-10-31-scripts-BLOCO-02-droid-e-progressao.md). Achados que já constavam neles permanecem separados abaixo, sem duplicação como achados novos.

## 1. Resumo por arquivo

| Arquivo | Linhas | Leitura integral | Função |
|---|---:|---:|---|
| `Assets/Scripts/Estado/DadosDoJogo.cs` | 127 | Sim | DTOs serializáveis para Droid, peças, técnicas, flags, itens, contadores de quest e save v7. |
| `Assets/Scripts/Estado/GerenciadorDeEstado.cs` | 268 | Sim | Singleton e estado em memória: Droid, inventário, Gold, flags, contadores, posição e contexto temporário de batalha. |
| `Assets/Scripts/Estado/ISistemaDeSalvamento.cs` | 10 | Sim | Interface mínima para salvar, carregar e consultar existência de save. |
| `Assets/Scripts/Estado/SalvamentoJson.cs` | 410 | Sim | Serialização JSON, migração, validação, checkpoint, restauração e reconstrução de peças. |

## 2. Estado do sistema

O save atual é v7. A carga migra energia legada, limpa efeitos temporários, recusa destinos `Battle`/`GameOver` e restaura equipamento antes do HP, alinhado ao contrato de [estado-e-save.md](../sistemas/estado-e-save.md). O checkpoint de exploração é preparado antes de gravar o contexto pendente de batalha ([GerenciadorDeEstado.cs:190–210](../../Assets/Scripts/Estado/GerenciadorDeEstado.cs)); esse fluxo corresponde ao contrato de [combate.md](../sistemas/combate.md).

`GerenciadorDeEstado` integra estado do Droid, inventário, catálogos, persistência e fluxo de combate. `SalvamentoJson` depende de tipos do Droid/equipamento, `TabelaDeEnergia`, `SceneManager` e cenas incluídas no build. Alterações nesses contratos devem preservar a compatibilidade das migrações e a ordem de checkpoint.

## 3. Achados

### 🟡 Importante — carga inválida pode deixar o estado ativo parcialmente aplicado

- **Arquivo/linhas:** [SalvamentoJson.cs:225–236](../../Assets/Scripts/Estado/SalvamentoJson.cs), [SalvamentoJson.cs:265–309](../../Assets/Scripts/Estado/SalvamentoJson.cs) e [SalvamentoJson.cs:337–367](../../Assets/Scripts/Estado/SalvamentoJson.cs).
- **Evidência:** `ValidarDados` verifica o DTO, versão, peças e destino, mas não todas as listas e seus elementos. Em seguida, `AplicarDadosSemTrocarCena` já limpa efeitos e altera dados do Droid antes de percorrer as técnicas e outras coleções. Coleções nulas ou elementos nulos podem causar exceção durante a aplicação.
- **Causa provável:** validação incompleta antes de mutar o estado ativo.
- **Impacto/certeza:** risco condicionado a save/DTO malformado; a leitura pode falhar após alterar parte da sessão. A análise estática não estabelece que haja um arquivo malformado em uso.
- **Divergência:** o plano determina que falha de leitura/validação não aplique metade do save ([02-estado-inventario-e-economia.md:17](../../planos-implementacao/melhorias/02-estado-inventario-e-economia.md)); o próprio plano mantém EST-02 como proposta, não como requisito já concluído ([02-estado-inventario-e-economia.md:9](../../planos-implementacao/melhorias/02-estado-inventario-e-economia.md)).
- **Sugestão (não aplicada):** validar integralmente coleções e elementos antes de alterar o estado; construir um estado candidato e só aplicá-lo após validação completa, assegurando que a falha não muta parcialmente a sessão.

### 🟢 Baixa prioridade — remoção com quantidade negativa pode criar itens

- **Arquivo/linhas:** [GerenciadorDeEstado.cs:116–130](../../Assets/Scripts/Estado/GerenciadorDeEstado.cs).
- **Evidência:** `TentarRemoverItem` verifica `atual < quantidade` e depois subtrai a quantidade, sem rejeitar valores não positivos. Com estoque zero e quantidade `-1`, a guarda passa, o saldo torna-se 1 e a operação pode retornar sucesso.
- **Causa provável:** falta de validação de quantidade positiva.
- **Impacto/certeza:** depende de um chamador passar quantidade negativa. Os usos consultados normalmente passam `1`; a entrega de quest utiliza a quantidade do objetivo ([GerenciadorDeQuests.cs:242](../../Assets/Scripts/Historia/GerenciadorDeQuests.cs)).
- **Sugestão (não aplicada):** rejeitar `quantidade <= 0` antes de consultar ou alterar o inventário.

## 4. Achados prévios — não repetidos

- **Efeitos temporários entre batalhas (H-01):** hipótese anterior, sem regra de design fechada; o encerramento da batalha não foi reauditado neste bloco. A carga limpa as listas de efeitos antes de restaurar o HP ([SalvamentoJson.cs:275–276](../../Assets/Scripts/Estado/SalvamentoJson.cs)). Referências: [auditoria geral, linhas 227 e 249](2026-10-30-auditoria-geral-projeto.md), [BLOCO 02](2026-10-31-scripts-BLOCO-02-droid-e-progressao.md).
- **Progresso de quest por derrota (I-01):** achado anterior, confirmado no BLOCO 01; não reclassificado aqui. Referências: [auditoria geral, linha 210](2026-10-30-auditoria-geral-projeto.md) e [BLOCO 01](2026-10-31-scripts-BLOCO-01-combate.md).
- **Possível duplicação de recompensa em reentrada (H-04):** hipótese anterior do fluxo de batalha, fora do recorte reaberto. Referência: [auditoria geral, linha 230](2026-10-30-auditoria-geral-projeto.md).

## 5. Contratos, divergências e decisões

- O save v7, migração de energia legada, limpeza de efeitos na carga, recusa dos destinos `Battle`/`GameOver` e ordem de restauração dos equipamentos antes do HP são coerentes com [estado-e-save.md](../sistemas/estado-e-save.md).
- `TentarPrepararBatalha` salva checkpoint antes de definir o contexto pendente ([GerenciadorDeEstado.cs:190–210](../../Assets/Scripts/Estado/GerenciadorDeEstado.cs)); `SalvamentoJson` valida destinos e depende das cenas/build settings ([SalvamentoJson.cs:208–246](../../Assets/Scripts/Estado/SalvamentoJson.cs)). Isso coincide com o contrato de checkpoint em [combate.md:39–42](../sistemas/combate.md).
- O plano EST-02 descreve atomicidade de carga como trabalho proposto, portanto o comportamento observado diverge do objetivo planejado, não de uma implementação já aceita.

### Intenções ⚪ — não tratar como defeito por si só

- Trânsito de inimigo e flag de vitória não são persistidos deliberadamente; o save de combate é checkpoint de exploração, não retomada do turno.
- O singleton é lazy; há defaults de Droid/itens; `ISistemaDeSalvamento` é uma abstração para evolução futura. Referências: [GerenciadorDeEstado.cs:8–14](../../Assets/Scripts/Estado/GerenciadorDeEstado.cs), [GerenciadorDeEstado.cs:170–188](../../Assets/Scripts/Estado/GerenciadorDeEstado.cs) e [ISistemaDeSalvamento.cs:1–4](../../Assets/Scripts/Estado/ISistemaDeSalvamento.cs).
- A wiki distingue implementação de percurso validado ([Estado-do-projeto.md:3–25](../../wiki/Estado-do-projeto.md)); a leitura do código não comprova validação em execução.

## 6. Limitações

Não rodei testes, jogo, compilação, Unity ou MCP; não li nem alterei saves, cenas ou prefabs. As conclusões são estáticas: não confirmam comportamento em execução nem a ocorrência de save malformado em uso real. O fluxo completo de fim de batalha não foi reaberto neste bloco.

## 7. Próximo passo

BLOCO 03 concluído e registrado. Próximo na sequência: BLOCO 04 — História e catálogos, mantendo a auditoria somente leitura e sem repetir achados anteriores como novos.
