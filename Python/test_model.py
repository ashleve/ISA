from mlagents_envs.environment import UnityEnvironment
from utils import EnvWrapper, Memory
from models import Actor, Critic
import numpy as np
import torch
import wandb
import yaml


print("Downloading model...")
api = wandb.Api()
# run = api.run("rl-cars/ISA_mlagents/2y77vzvy")  # steeringWheelSimCar best model
run = api.run("rl-cars/ISA_mlagents/12jz73au")  # steeringWheelSimCar best model
# run = api.run("rl-cars/ISA_mlagents/2a7vi8uo")  # wheelSimCar best model
run.file("actor_model.h5").download(replace=True)
run.file("critic_model.h5").download(replace=True)
run.file("config.yaml").download(replace=True)
print("Model downloaded.")


with open('config.yaml') as file:
    config = yaml.load(file, Loader=yaml.FullLoader)
# print(config)


actor = Actor(
    obs_space=config["obs_space"]["value"],
    action_space=config["action_space"]["value"],
    hidden_size=config["hidden_size"]["value"]
)

actor.load_state_dict(torch.load('actor_model.h5'))
print("Neural network initialized.")


print("Press play in Unity.")
env = UnityEnvironment()
env = EnvWrapper(env)


decision_steps = env.reset()
while True:

    states = decision_steps.obs[0]

    if states.shape[0] != 0:
        states = torch.as_tensor(states, dtype=torch.float32)
        actions, _, _ = actor(states)
        actions = np.clip(actions.cpu().numpy(), -1, 1)
    else:
        actions = env.get_random_actions()

    decision_steps, terminal_steps = env.step(actions)
