# Equipamentos do Apollo

## Usar no jogo

Adquira a peça na loja ou por recompensa e execute no terminal:

```lua
droid.listarPecas()
droid.equipar("sensor_optico")
droid.desequipar("cabeca")
```

Slots aceitos: `cabeca`, `tronco`, `braco`, `perna` (também aceita ç e maiúsculas). Métodos retornam verdadeiro/falso e explicam o resultado no log do terminal. Menu Droid e Bag apenas inspecionam equipamentos; não há botão para instalar.

Instalar retira uma unidade do inventário; trocar devolve a anterior. Instalar novamente o mesmo ID não consome outra unidade. Desequipar devolve a peça ao inventário; slot vazio não faz nada. Falta de estoque/ID inválido cancela a operação. Alterar Vitalidade por equipamento preserva HP atual e limita ao novo máximo, sem curar.

Use **Salvar** depois de programar a montagem. Os checkpoints existentes também a gravam; não há autosave por comando. Erro Lua desfaz alterações de equipamentos/inventário daquela execução. Isso não torna transacionais os comandos antigos de atributos/técnicas nem protege contra loops infinitos.

## IDs atuais

O catálogo lógico é `Assets/Scripts/Catalogos/CatalogoDeItens.cs`: 27 IDs, seis equipamentos. Os nomes podem mudar sem mudar o ID. Não renomear IDs existentes sem migração dos saves.

| ID | Item | Slot | Bônus |
|---|---|---|---|
| `nucleo_de_energia` | Núcleo de Energia | Tronco | +5 Força |
| `servo_motor` | Servo-Motor | Perna | +5 Agilidade |
| `sensor_optico` | Sensor Óptico | Cabeça | +6 Destreza |
| `placa_de_blindagem` | Placa de Blindagem | Tronco | +6 Vitalidade |
| `giroscopio` | Giroscópio | Braço | +6 Sorte |
| `nucleo_sobrecarga` | Núcleo de Sobrecarga | Braço | +10 Força |

Domínio: `ServicoDeEquipamento.TentarEquipar` / `TentarDesequipar` recebem estado explicitamente e não fazem IO. `Droid.ObterPeca` consulta o slot. O save v7 guarda slot, ID, nome, raridade, atributo principal e bônus; restaura peças antes de limitar HP. IDs/slots incompatíveis, slots duplicados e versões futuras são recusados antes da aplicação dos equipamentos.

Saves anteriores não continham as peças: carregam com slots vazios. Peças que já desapareceram de um save antigo não podem ser recuperadas automaticamente. Peça legada ainda em memória sem ID pode ser preservada no novo save, mas sua troca/devolução é recusada para evitar inventar um item pelo nome.

## Imagens e troca de sprites

- Artes finais dos menus: `Assets/UI/`, separadas em `LojaIlustrada`, `MenuMundoIlustrado`, `BagIlustrada`, `StatusIlustrado`, `TerminalIlustrado` e `DroidIlustrado`.
- Tela atual do Droid: `Assets/UI/DroidIlustrado/droid-conceito-v2.png`; primeira proposta preservada como `droid-conceito.png`.
- Referências originais enviadas: `Assets/sprites/Atualizados/novos/`, incluindo as folhas 3 e 17.
- Sprites individuais: `Assets/sprites/Itens/Recortes/<id>.asset`; folhas em `Assets/sprites/Itens/Consumo`, `Tecnologia` e `Especiais`.
- Relação ID → sprite: `Assets/Resources/Catalogos/VisuaisDeItens.asset`, resolvida por `CatalogoVisualDeItens.Obter(id)`. [Guia de troca de ícones](icones-de-itens.md).

Tela Droid usa a sprite do ID realmente instalado. Corpo atual e ícones são componentes separados. `CompositorApollo` reserva `CorpoBase` e camadas Perna/Tronco/Braco/Cabeca no mesmo retângulo, desativadas enquanto não houver arte vestível compatível. Ícones de item não são sobrepostos no corpo. Para a futura aparência será necessário desenhar base/peças no mesmo gabarito e cadastrar a relação por ID; esta entrega prepara a hierarquia, sem prometer camadas vestíveis já existentes.

## Verificação

`Assets/Editor/ValidacaoEquipamentosApollo.cs` verifica serviço, Lua real, save isolado e restauração em outra instância; `Visual(largura, altura)` renderiza o painel real em cena de prévia, verifica referências/consulta/paginação/retorno. Evidências e limites ficam no [plano da tarefa](../planos-implementacao/2026-10-23-droid-ilustrado.md). Fixtures não gravam o save pessoal, não carregam cenas de jogo nem equivalem a input real/aprovação visual.
