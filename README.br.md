# Sistema Biomecânico Baseado em Realidade Virtual

## Visão Geral

### Descrição Breve

Este projeto visa utilizar sensores de EMG para analisar e aproveitar dados de sinais musculares para aprimorar tratamentos para indivíduos com deficiências parciais nos membros, como aqueles com extremidades superiores ou inferiores incompletas, com o suporte de um ambiente de realidade virtual.

### Como Funciona

Dois sensores musculares são fixados no braço do paciente, um no bíceps e o outro no tríceps. Após o paciente colocar os óculos de realidade virtual e estar posicionado corretamente, inicia-se a cena de calibração. O paciente realiza **contração** e **extensão** máximas para obter parâmetros precisos da posição do braço. A calibração é feita em duas etapas: **extensão** e **contração**, cada uma com duração de 25 segundos. O usuário inicia e finaliza a calibração usando o controle.

Os parâmetros são calculados automaticamente, e os mapas de contração e extensão são exibidos na cena do Unity. A equipe avalia a qualidade da calibração e, se necessário, a calibração pode ser repetida. Durante esse processo, o usuário pode ver os valores normalizados dos sensores no gráfico.

Após a calibração, a cena principal começa, e o paciente pode ver o braço mecânico se movendo em resposta à ativação muscular.

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

## Software

Para as especificações de configuração do software, foram utilizados alguns aplicativos chave:

- Unity *Build 2022.3.51F*
- Scripts em Python *3.11* 
- Código Arduino

> Mais informações sobre os scripts de Arduino e Python podem ser encontradas [neste repositório](https://github.com/fiorotticaio/Hardware-and-auxiliary-codes-for-the-biomechanical-system).

## Mídia

Este projeto foi apresentado na [Exposição de Computação e Tecnologia](https://computacao-ufes.github.io/mostra/pic2_EC_20241.html) na [Universidade Federal do Espírito Santo](ufes.br).

Neste [vídeo](https://youtu.be/uEduPgnbO7c) mais detalhes são apresentados mostrando como o sistema funciona.

## Autores

- [@fiorotticaio](https://github.com/fiorotticaio)
- [@matheusschreiber](https://github.com/matheusschreiber)
- [@viniciuscole](https://github.com/viniciuscole)
