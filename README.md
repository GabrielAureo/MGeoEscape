# MGeoEscape
O MGeoEscape é um escape game em realidade aumentada feito em conjunto pelo Museu da Geodiversidade e o Lab3D da UFRJ.

## Funcionalidades

### Multiplayer em LAN
Afim de estimular o trabalho em equipe dos jogadores, as partidas foram pensadas em grupos de 3 jogadores sincronizados pela rede local do Museu da Geodiversidade. A API utilizada para o networking foi o Mirror Networking.

![Seleção de Jogador](/demo/mainmenu.gif)

### Realidade Aumentada baseada em reconhecimento de marcadores
Devido a falta de suporte ao ARCore nos aparelhos do MGeo, tomamos a decisão de utilizar marcadores físicos para a exprência de realidade aumentada do jogo. Para isso, utilizamos o pacote Vuforia.

![Uberabatitan em AR](/demo/dinoAR.jpg)

![Cofre em AR](/demo/safeAR.jpg)

### Movables e Sockets
A principal mecânica do jogo se dá pelo encaixe de objetos movíveis (chamados movables) em sockets. Os jogadores controlar os movables com o toque na tela do aparelho e ao soltá-lo em um objeto socket, ele se acomoda na posição.

![Gameplay do Dinossauro](/demo/dino_gameplay.gif)

![Gameplay do Dinossauro](/demo/safe_gameplay.gif)

