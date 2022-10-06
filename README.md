# Fun Autonomous Vehicle RL Environments in Unity

Investigating ways of simulating autonomous vehicles in Unity and fun experiments with Reinforcement Learning. Preparing environments for "Intelligent Autonomous Systems" university classes.

## Dependencies
- Unity 2020.1.3f1
- ML Agents 1.0.4
- Pytorch 1.8.0

## Algorithms

All agents were trained on Proximal Policy Optimization (PPO) algorithm, which was written from scratch in Pytorch. <br>
PPO available at [https://github.com/ashleve/ISA/tree/master/Python](https://github.com/ashleve/ISA/tree/master/Python).

## Agents

| DrivingOn2WheelsAgent       | CoinCollectingAgent         |
|-----------------------------|-----------------------------|
| <img src="https://github.com/hobogalaxy/ISA/blob/resources/gifs/drive2wheels.gif" width="390"> | <img src="https://github.com/hobogalaxy/ISA/blob/resources/imgs/CoinCollectingAgent.png" width="390"> |


<br>


| WheelSimCarAgent            | SteeringWheelSimCarAgent    |
|-----------------------------|-----------------------------|
| <img src="https://github.com/hobogalaxy/ISA/blob/resources/gifs/wheelSimAgent.gif" width="390"> | <img src="https://github.com/hobogalaxy/ISA/blob/resources/gifs/steeringWheelSimAgent.gif" width="390"> |

### WheelSimCarAgent vs SteeringWheelSimCarAgent
![](https://github.com/hobogalaxy/ISA/blob/resources/imgs/chart1.png)


<br>


![](https://github.com/hobogalaxy/ISA/blob/resources/gifs/raycasts.gif)
