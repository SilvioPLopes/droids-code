# Ícones de itens — localizar e trocar

O catálogo lógico continua em `Assets/Scripts/Catalogos/CatalogoDeItens.cs`: ID, nome, efeito, preço e disponibilidade. A aparência fica separada no [catálogo visual](../../Assets/Resources/Catalogos/VisuaisDeItens.asset). A loja consulta a sprite pelo **ID exato**, não pela categoria. Trocar a imagem não altera o funcionamento do item.

## Trocar uma imagem

1. No Unity, abrir **DroidsCode → Itens → Abrir catálogo visual** ou selecionar `Assets/Resources/Catalogos/VisuaisDeItens.asset` no Project.
2. Expandir `Itens` no Inspector e localizar a entrada pelo ID/nome. O nome de consulta serve somente para facilitar a procura.
3. Importar a nova imagem como **Sprite (2D and UI)**, com transparência e sem cortar o objeto. Arrastar a sprite para o campo `Sprite` dessa entrada. Se vier de uma folha, usar seu recorte, não a folha inteira.
4. Atualizar `Origem` com o arquivo/referência usado. Manter o ID. Cada item deve apontar para uma sprite distinta.
5. Abrir a loja e conferir ícone da lista e do detalhe, especialmente no tamanho pequeno. `OrganizacaoVisualDeItens.Validar()` verifica cobertura e referências repetidas; a avaliação visual continua necessária.

Não precisa editar código da loja, abrir a cena para salvar referência ou recriar o catálogo. Não executar `Aplicar()` novamente: ele é o importador inicial e recusa sobrescrever escolhas existentes.

## Organização dos arquivos

| Local | Conteúdo |
|---|---|
| `Assets/Resources/Catalogos/VisuaisDeItens.asset` | Relação editável ID → sprite; fonte de referência usada pelo jogo |
| `Assets/sprites/Itens/Consumo/consumo.png` | Folha de curas e consumíveis |
| `Assets/sprites/Itens/Tecnologia/tecnologia.png` | Folha de peças e equipamentos utilitários |
| `Assets/sprites/Itens/Especiais/especiais.png` | Folha de missão, facção e itens fora da vitrine atual |
| `Assets/sprites/Itens/Recortes/<id>.asset` | Sprite individual nomeada pelo ID e referenciando a região da folha |
| `Assets/sprites/Atualizados/novos/3-removebg-preview.png` e `17-removebg-preview.png` | Referências originais preservadas; não são substituídas pelas novas folhas |
| `Assets/UI/LojaIlustrada/loja-icones.png` e `icone-0..5.asset` | Conjunto antigo por categoria, preservado por compatibilidade; não define mais o ícone do item |

As folhas novas foram geradas com image_gen usando 3/17 como referências de estilo, com metal/latão, cores e silhuetas próprias. Cada folha possui nove objetos em três linhas/colunas. A resolução efetivamente entregue foi **1254×1254**, embora o prompt pedisse 1536×1536. Os recortes consideram o alpha real, pois bicos/antenas podem passar da célula nominal; não reutilizar coordenadas fixas de outra resolução. As três poções usam uma janela de tamanho comum para preservar a diferença visual de volume, além da silhueta e das faixas.

## Mapa inicial de referências

Linha/coluna são contadas a partir do canto superior esquerdo. O campo `Origem` de cada entrada guarda também o retângulo efetivo, nas coordenadas de textura do Unity (origem embaixo à esquerda). Este mapa documenta a importação inicial; se houver troca manual posterior, o asset e sua origem atualizada prevalecem.

| ID | Nome | Folha | Linha/coluna |
|---|---|---|---|
| `pocao_pequena` | Poção Pequena | Consumo | 1/1 |
| `pocao_media` | Poção Média | Consumo | 1/2 |
| `pocao_grande` | Poção Grande | Consumo | 1/3 |
| `reparo_completo` | Reparo Completo | Consumo | 2/1 |
| `escudo_de_emergencia` | Escudo de Emergência | Consumo | 2/2 |
| `oleo_de_precisao` | Óleo de Precisão | Consumo | 2/3 |
| `sobrecarga_temp` | Injetor de Sobrecarga | Consumo | 3/1 |
| `granada_de_acido` | Granada de Ácido | Consumo | 3/2 |
| `corrosivo` | Corrosivo | Consumo | 3/3 |
| `disruptor_emp` | Disruptor EMP | Tecnologia | 1/1 |
| `nucleo_de_energia` | Núcleo de Energia | Tecnologia | 1/2 |
| `servo_motor` | Servo-Motor | Tecnologia | 1/3 |
| `sensor_optico` | Sensor Óptico | Tecnologia | 2/1 |
| `placa_de_blindagem` | Placa de Blindagem | Tecnologia | 2/2 |
| `giroscopio` | Giroscópio | Tecnologia | 2/3 |
| `sinalizador_de_retorno` | Sinalizador de Retorno | Tecnologia | 3/1 |
| `kit_de_acampamento` | Kit de Acampamento | Tecnologia | 3/2 |
| `modulo_de_treinamento` | Módulo de Treinamento | Tecnologia | 3/3 |
| `chave_de_acesso` | Chave de Acesso | Especiais | 1/1 |
| `nucleo_sobrecarga` | Núcleo de Sobrecarga | Especiais | 1/2 |
| `modulo_ressonancia` | Módulo de Ressonância | Especiais | 1/3 |
| `fio_de_cobre` | Fio de Cobre | Especiais | 2/1 |
| `filtro_de_agua` | Filtro de Água | Especiais | 2/2 |
| `sucata_limpa` | Sucata Limpa | Especiais | 2/3 |
| `emblema_da_guilda` | Emblema da Guilda | Especiais | 3/1 |
| `bilhete_do_ivo` | Bilhete do Ivo | Especiais | 3/2 |
| `peca_perna_apollo` | Servo da Perna do Apollo | Especiais | 3/3 |

## Quando entrar outro item

Adicionar o item lógico somente na tarefa que autorizar essa funcionalidade. Depois, acrescentar uma entrada no catálogo visual com o mesmo ID e uma sprite exclusiva. Pode ser um PNG separado ou recorte de outra folha; não precisa reorganizar as folhas existentes. Não habilitar venda por estar presente no catálogo visual.

Nesta etapa, só a loja passou a consumir esse catálogo. Bag, combate e outros menus não foram alterados. Execução, validação, backups e pendências no [plano de sprites únicas](../planos-implementacao/2026-10-23-sprites-unicas-itens.md).
