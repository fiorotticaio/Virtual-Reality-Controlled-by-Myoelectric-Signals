# Sistema Biomecânico Baseado em Realidade Virtual

## Visão Geral

### Descrição Breve

Este projeto visa utilizar sensores de EMG para analisar e aproveitar dados de sinais musculares para aprimorar tratamentos para indivíduos com deficiências parciais nos membros, como aqueles com extremidades superiores ou inferiores incompletas, com o suporte de um ambiente de realidade virtual.

### Como Funciona

Dois sensores musculares são fixados no braço do paciente, um no bíceps e o outro no tríceps. Após o paciente colocar os óculos de realidade virtual e estar posicionado corretamente, inicia-se a cena de calibração. O paciente realiza **contração** e **extensão** máximas para obter parâmetros precisos da posição do braço. A calibração é feita em duas etapas: **extensão** e **contração**, cada uma com duração de 25 segundos. O usuário inicia e finaliza a calibração usando o controle.

Os parâmetros são calculados automaticamente, e os mapas de contração e extensão são exibidos na cena do Unity. A equipe avalia a qualidade da calibração e, se necessário, a calibração pode ser repetida. Durante esse processo, o usuário pode ver os valores normalizados dos sensores no gráfico.

Após a calibração, a cena principal começa, e o paciente pode ver o braço mecânico se movendo em resposta à ativação muscular.

### Jogos

Existem alguns *mini-games* dentro da cena principal, que servem para estimular o paciente de uma maneira divertida. Com o controle do Quest 2, o usuário pode selecionar o jogo que quer testar.

O primeiro é um desafio para que o usuário tente manter a angulação do braço mecânico em um certo ângulo indicado na tela. Simples de entender, mas muito mais desafiador do que parece

O segundo é o jogo das cores. Movendo o braço mecânico, o paciente deve apontar para a cor correta, indicada na tela, e apertar o gatilho do controle.

Além disso, o usuário pode simular o movimento de pegar uma maçã, na opção "mover maçã".

## Hardware

Estes foram os principais materiais utilizados no projeto:

- Eletrodos de ECG descartáveis
- Fios de cobre (jumpers)
- Placa Arduino UNO
- Protoboard (ou Placa Perfurada)
- Placa MyoWare (Sensor Muscular)
- Óculos de Realidade Virtual - (Oculus Quest 2)
- Lâmina Descartável
- Álcool Esterilizante

### Sensores Myoware

<img src="media/myoware.png" height="300px"/>

### Esquemático do Projeto

<img src="media/schematic.png" width="500px"/>

### Placa Arduino

<img src="media/board.png" width="500px"/>

## Software

Para as especificações de configuração do software, foram utilizados alguns aplicativos chave:

- Unity *Build 2022.3.51F*
- Scripts em Python *3.11* 
- Código Arduino

> Mais informações sobre os scripts de Arduino e Python podem ser encontradas [neste repositório](https://github.com/fiorotticaio/Hardware-and-auxiliary-codes-for-the-biomechanical-system).

## Mídia

### Ambiente de Realidade Virtual - Calibração

<img src="media/calibration.png" width="500px"/>

### Ambiente de Realidade Virtual - Simulação

<img src="media/virtual-reality.png" width="500px"/>

### Mais informações

Neste [vídeo](https://youtu.be/uEduPgnbO7c) mais detalhes são apresentados mostrando como o sistema funciona.

## Autores

- [@fiorotticaio](https://github.com/fiorotticaio)
- [@matheusschreiber](https://github.com/matheusschreiber)
- [@viniciuscole](https://github.com/viniciuscole)

## Bonus

Este projeto foi apresentado na [Exposição de Computação e Tecnologia](https://computacao-ufes.github.io/mostra/pic2_EC_20241.html) na [Universidade Federal do Espírito Santo](ufes.br).