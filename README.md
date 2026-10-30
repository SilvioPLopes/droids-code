# Droids Code

RPG educativo em 2D que ensina lógica de programação por meio de exploração, diálogos, desafios em Lua e combates por turnos. O jogador controla Lip e programa seu Droid, Apollo, para avançar pela história de Ferrovale.

Projeto de TCC do curso de Sistemas para Internet da FATEC Baixada Santista — Rubens Lara. Este guia foi preparado para o professor executar o jogo **dentro do Unity Editor** e conhecer as etapas iniciais.

## Funcionalidades

- Exploração de mapas em 2D e interação com personagens.
- Terminal com execução de código Lua e desafios de lógica.
- Configuração de atributos e técnicas do Apollo por programação.
- Combate por turnos, inventário, equipamentos e loja.
- Sistemas de missões e salvamento de progresso.

O projeto está em desenvolvimento. A presença de um sistema não significa que todo o percurso da história esteja finalizado; consulte as condições de continuação no roteiro abaixo.

## Tecnologias utilizadas

- **Engine:** Unity **6000.5.8f1**, conforme [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).
- **Linguagens:** C# nos sistemas do jogo e Lua no terminal do jogador.
- **Renderização:** Universal Render Pipeline (URP), com recursos 2D.
- **Interface:** Unity UI (uGUI) e TextMeshPro.
- **Interpretador Lua:** MoonSharp.
- **Versionamento:** Git e GitHub.

As dependências estão em [Packages/manifest.json](Packages/manifest.json).

## Pré-requisitos

- Unity Hub instalado.
- Unity Editor **6000.5.8f1** instalado pelo Hub.
- Git, caso opte por clonar o repositório.
- Internet para baixar o projeto e resolver os pacotes na primeira abertura.

Para jogar pelo Editor, não é necessário gerar um executável nem instalar módulos de WebGL.

## Instalação e configuração

### 1. Obter o código-fonte

No terminal, execute:

```bash
git clone https://github.com/SilvioPLopes/droids-code.git
cd droids-code
```

Outra opção é usar **Code → Download ZIP** no GitHub e extrair o arquivo antes de abrir. Em ambos os casos, localize a pasta que contém `Assets`, `Packages` e `ProjectSettings`.

### 2. Abrir pelo Unity Hub

1. Abra o Unity Hub.
2. Na aba **Projects**, use a opção de adicionar um projeto existente no disco.
3. Selecione a pasta que contém `Assets`, `Packages` e `ProjectSettings`.
4. Abra com a versão **6000.5.8f1**.
5. Aguarde a importação dos assets, a resolução dos pacotes e a compilação dos scripts. A primeira abertura pode demorar mais.

Os pacotes são resolvidos pelo Unity. Não é necessário cadastrar credenciais ou variáveis de ambiente para seguir este roteiro.

## Execução em ambiente de desenvolvimento

### Dar Play no jogo

1. No painel **Project**, abra `Assets → Scenes`.
2. Dê dois cliques em **MainMenu.unity**.
3. Clique no botão **▶ Play**, na barra superior do Unity.
4. Na aba **Game**, clique em **Novo Jogo**. A cena inicial configurada é **CasaLip**.
5. Clique na área da aba **Game** para que ela receba os comandos do teclado.

Para encerrar, clique novamente em **▶ Play**. Comece pelo `MainMenu` para acompanhar o fluxo inicial previsto.

### Controles

| Ação | Comando |
|---|---|
| Mover Lip | **W, A, S, D** ou **setas** |
| Interagir com personagem ou terminal próximo | **E** |
| Avançar diálogo | **E**, **Espaço** ou **Enter** |
| Abrir o Menu do Mundo durante a exploração | **Esc** |
| Escolher botões e opções | **Clique do mouse** |
| Executar o código no terminal | Botão **Executar** ou **Ctrl + Enter** |

Durante diálogos e menus, o movimento pode ficar bloqueado. Termine a conversa ou feche o painel para voltar a andar. Ao escrever no terminal, clique primeiro no campo de código.

## Como jogar as etapas iniciais

### 1. Casa de Lip — o gerador de água

**Objetivo:** fazer o nível de água chegar a 100 para liberar a saída da casa.

1. Leia a conversa inicial com Apollo e avance as falas.
2. Aproxime Lip do terminal da casa e pressione **E**.
3. Leia o enunciado e a dica. O código utiliza a variável `nivel_agua`.
4. Escreva um programa que aumente esse valor de dez em dez até chegar a 100.
5. Clique em **Executar** ou pressione **Ctrl + Enter**.
6. Conclua a conversa que acompanha o sucesso e siga pela saída para chegar à cidade.

<details>
<summary>Solução de apoio para o professor</summary>

```lua
nivel_agua = 0
while nivel_agua < 100 do
  nivel_agua = nivel_agua + 10
end
```

O programa começa em zero. Enquanto o valor é menor que 100, soma dez. Ao chegar a 100, a condição deixa de ser verdadeira e o laço termina.

O verificador considera o valor final da variável; ele não exige uma forma específica de escrever a solução. O exemplo demonstra variável, repetição e condição de parada.

</details>

**Resultado esperado:** a porta fica liberada e Lip pode sair. Apenas conversar com Apollo não resolve o desafio.

### 2. Ferrovale — explorar e conversar

**Objetivo:** conhecer o problema da cidade e reunir as informações iniciais.

1. Ao sair da casa, explore a cidade com WASD ou setas.
2. Aproxime-se de **Dara** e pressione **E**. Leia a conversa até o fim.
3. Faça o mesmo com o **Homem da fila**.
4. Depois das duas conversas, procure Apollo e aproxime-se para acionar seu comentário de conclusão.
5. Visite o **Senhor Tácio** para conhecer a interação de loja.
6. Use **Esc** para conhecer o Menu do Mundo e acessar os painéis, como Terminal, Bag, Status e Droid.

**Resultado esperado:** conhecer o contexto da falta de água, ouvir o comentário de Apollo e explorar os sistemas disponíveis na cidade.

### 3. Preparar Apollo e tentar seguir para o vale

A saída da cidade exige concluir as conversas iniciais e configurar pelo menos **uma técnica além do Ataque Básico**.

1. Abra **Esc → Terminal** e consulte a ajuda e o saldo de pontos de programação.
2. Se houver pontos suficientes, um exemplo de comando para aprender uma técnica é:

   ```lua
   droid.aprenderTecnica("Golpe Inicial", 1)
   ```

3. Execute e confira a mensagem do terminal. A técnica só é aprendida se os requisitos e o custo forem atendidos.
4. Feche o painel e procure a saída da cidade para o vale, correspondente à cena `Game`.
5. Caso a passagem permaneça bloqueada, leia a mensagem: ela distingue a falta de conversas da falta de técnica.

**Limite deste roteiro:** a disponibilidade de pontos pela progressão normal antes dessa saída ainda precisa ser confirmada em uma partida completa. Se não houver pontos para aprender a técnica, registre esse bloqueio e encerre a avaliação inicial na cidade. Esse caso não deve ser contornado com comandos de teste durante a avaliação do percurso normal.

Se conseguir avançar e entrar em um combate, escolha as ações pelos botões da tela. O combate ocorre por turnos; técnicas consomem energia, e o Ataque Básico permite recuperá-la. A continuação além da cidade depende do conteúdo montado na versão entregue.

## Se algo não funcionar

| Situação | O que conferir |
|---|---|
| As teclas não respondem | Clique na aba **Game** e termine diálogos ou feche painéis abertos. |
| O jogo começa em outro mapa | Pare o Play, abra `Assets/Scenes/MainMenu.unity` e inicie novamente. |
| A porta da casa continua trancada | Execute o código e confira se `nivel_agua` chegou a pelo menos 100. |
| A saída da cidade está bloqueada | Termine as conversas e confira se Apollo aprendeu uma técnica extra. |
| O Unity apresenta erro de compilação | Confira a versão do Editor e aguarde a resolução de pacotes. Se persistir, envie ao responsável a mensagem da janela **Console**. |

O jogo possui salvamento de progresso. Não exclua saves para tentar resolver problemas. O botão **Novo Jogo** carrega a cena inicial; este guia não pressupõe que ele apague um progresso anterior.

## Build para produção

A avaliação descrita acima usa o Unity Editor. Para gerar uma distribuição separada:

1. Fora do Play, abra **File → Build Profiles**.
2. Selecione a plataforma desejada e confira se o módulo correspondente está instalado pelo Unity Hub.
3. Confira a lista de cenas, mantendo `Assets/Scenes/MainMenu.unity` como primeira cena habilitada.
4. Use **Build** e escolha uma pasta de saída fora de `Assets`.
5. Teste a distribuição gerada antes de entregá-la. O funcionamento no Editor não comprova o funcionamento do build.

Windows e WebGL são plataformas pretendidas pelo projeto; este documento não certifica builds nessas plataformas.

## Aplicação publicada

Para esta entrega, a execução será local pelo **Unity Editor**. Não foi informado um endereço público para jogar no navegador nem um download de executável.

## Estrutura do projeto

```text
droids-code/
├── Assets/
│   ├── Scenes/          # MainMenu, CasaLip, City, Game, Battle e interiores
│   ├── Scripts/         # Sistemas em C# e integração com Lua
│   ├── Prefabs/         # Objetos reutilizáveis
│   └── Resources/       # Recursos carregados pelo jogo
├── Packages/            # Manifesto e dependências do Unity
├── ProjectSettings/     # Versão e configurações do projeto
├── docs/                # Documentação e planos de trabalho
│   └── wiki/            # Páginas preparadas para a wiki
└── README.md            # Guia de execução e etapas iniciais
```

## Equipe

Os nomes dos integrantes e suas responsabilidades serão incluídos pelos autores antes da entrega acadêmica.

## Documentação adicional

- [Wiki do projeto no GitHub](https://github.com/SilvioPLopes/droids-code/wiki).
- Enciclopédia do jogo: [história](https://github.com/SilvioPLopes/droids-code/wiki/Historia), [personagens](https://github.com/SilvioPLopes/droids-code/wiki/Personagens), [missões](https://github.com/SilvioPLopes/droids-code/wiki/Missoes), [itens](https://github.com/SilvioPLopes/droids-code/wiki/Itens) e [bestiário](https://github.com/SilvioPLopes/droids-code/wiki/Bestiario).
- [Fontes das páginas da wiki neste repositório](docs/wiki/Home.md).
- [Índice da documentação técnica](docs/indice-trabalho.md).
- [Contexto e arquitetura do projeto](docs/contexto-projeto.md).
- [Roteiro técnico de validação](docs/guias/validacao-jogo.md).
- [Enredo](docs/enredo/TCC_-_ENREDO_v2.md) e [adendo](docs/enredo/TCC_-_ENREDO_v2_ADENDO.md).
