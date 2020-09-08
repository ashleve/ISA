from collections import deque

import torch
import torch.nn as nn
from torch.utils.data import SubsetRandomSampler, BatchSampler
from torch.optim import Adam
import wandb
import numpy as np
from mlagents_envs.environment import UnityEnvironment
from utils import EnvWrapper, Memory
from models import Actor, Critic
import time


wandb.init(project="PPO_continuous2", group='roller-agent', job_type="eval")

config = wandb.config
config.env_name = 'roller-agent'


# This is a non-blocking call that only loads the environment.
env = UnityEnvironment()
env = EnvWrapper(env)


config.gamma = 0.99
config.lamb = 0.95
config.batch_size = 10
config.memory_size = 100
config.hidden_size = 128
config.actor_lr = 0.0003
config.critic_lr = 0.0003
config.ppo_multiple_epochs = 3
config.eps = 0.2
config.grad_clip_norm = 0.5
config.entropy_weight = 0.0
config.max_steps = 50_000
config.num_of_agents = len(env.agent_ids)

config.obs_space = env.state_size
config.action_space = env.action_size
config.action_high = 1
config.action_low = -1


# device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
device = torch.device('cpu')
config.device = device.type
print(device)


actor = Actor(config).to(device)
critic = Critic(config).to(device)

wandb.watch(actor)
wandb.watch(critic)

optimizer_actor = Adam(actor.parameters(), lr=config.actor_lr)
optimizer_critic = Adam(critic.parameters(), lr=config.critic_lr)
memory = Memory(env.agent_ids)


def compute_GAE(rewards, state_values, done, gamma, lamb):
    """
        Computes Generalized Advantage Estimations.
    """
    returns = [rewards[-1] + state_values[-1]]
    running_sum = rewards[-1] - state_values[-1]
    for i in reversed(range(len(rewards) - 1)):
        mask = 0 if done[i+1] else 1
        delta = rewards[i] + gamma * state_values[i+1] * mask - state_values[i]
        running_sum = delta + gamma * lamb * running_sum * mask
        returns.insert(0, running_sum + state_values[i])

    returns = torch.as_tensor(returns, dtype=torch.float32)
    return returns


def compute_r2g(rewards, done, gamma):
    """
        Computes discounted rewards-to-go.
    """
    rewards2go = []
    running_sum = 0
    for r, is_terminal in zip(reversed(rewards), reversed(done)):
        running_sum = r + gamma * running_sum * (1 - is_terminal)
        rewards2go.insert(0, running_sum)

    rewards2go = torch.as_tensor(rewards2go, dtype=torch.float32)
    return rewards2go


def compute_loss(states, actions, rewards_to_go, adv, old_log_probs):

    # adv = (adv - adv.mean()) / (adv.std() + 1e-5)
    # rewards_to_go = (rewards_to_go - rewards_to_go.mean()) / (rewards_to_go.std() + 1e-5)

    # compute critic loss
    v = critic(states)
    critic_loss = (rewards_to_go - v).pow(2)

    # compute actor loss
    _, _, pi = actor(states)
    log_probs = pi.log_prob(actions).sum(1)
    ratio = torch.exp(log_probs - old_log_probs)  # exp(log_prob - old_log_prob) = (prob / old_prob)
    clip = torch.clamp(ratio, 1 - config.eps, 1 + config.eps)
    actor_loss = -torch.min(ratio * adv, clip * adv)

    # compute entropy
    entropy = pi.entropy().sum(1)
    actor_loss -= config.entropy_weight * entropy

    return actor_loss.mean(), critic_loss.mean(), entropy.mean(), ratio.mean()


def update():
    start = time.time()

    # compute rewards-to-go
    rewards_to_go = [compute_r2g(memory.rewards[id], memory.dones[id], gamma=config.gamma) for id in env.agent_ids]
    rewards_to_go = torch.cat(rewards_to_go).detach().to(device)

    # prepare data
    states = [torch.squeeze(torch.stack(memory.states[id]), 1) for id in env.agent_ids]
    states = torch.cat(states).detach().to(device)

    actions = [torch.squeeze(torch.stack(memory.actions[id]), 1) for id in env.agent_ids]
    actions = torch.cat(actions).detach().to(device)

    old_log_probs = [torch.squeeze(torch.stack(memory.log_probs[id]), 1) for id in env.agent_ids]
    old_log_probs = torch.cat(old_log_probs).detach().to(device)

    # state_values = torch.squeeze(critic(states).detach().to(device))

    # compute state values
    for id in env.agent_ids:
        memory.state_values[id] = critic(torch.stack(memory.states[id]))
    state_values = torch.squeeze(torch.cat([memory.state_values[id] for id in env.agent_ids])).detach().to(device)

    # # compute GAE
    # gae_returns = [compute_GAE(
    #     rewards=memory.rewards[id],
    #     state_values=memory.state_values[id],
    #     done=memory.dones[id],
    #     gamma=config.gamma,
    #     lamb=config.lamb
    # ) for id in env.agent_ids]
    # gae_returns = torch.cat(gae_returns).detach().to(device)
    # adv = (gae_returns - state_values).detach().to(device)

    # normalize rewards-to-go
    # rewards_to_go = (rewards_to_go - rewards_to_go.mean()) / (rewards_to_go.std() + 1e-5)

    # compute advantage estimations
    adv = (rewards_to_go - state_values)

    # adv = (adv - adv.mean()) / (adv.std() + 1e-5)

    # rewards_to_go = gae_returns

    # learn
    for _ in range(config.ppo_multiple_epochs):

        # create sampler
        sampler = SubsetRandomSampler(range(memory.size()))
        batch_sampler = BatchSampler(sampler, batch_size=config.batch_size, drop_last=False)

        # execute epoch
        for indices in batch_sampler:

            batch_states = states[indices]
            batch_actions = actions[indices]
            batch_rewards_to_go = rewards_to_go[indices]
            batch_adv = adv[indices]
            batch_old_log_probs = old_log_probs[indices]

            actor_loss, critic_loss, _, _ = compute_loss(
                batch_states,
                batch_actions,
                batch_rewards_to_go,
                batch_adv,
                batch_old_log_probs
            )

            # update critic
            optimizer_critic.zero_grad()
            critic_loss.backward()
            nn.utils.clip_grad_norm_(critic.parameters(), config.grad_clip_norm)
            optimizer_critic.step()

            # update actor
            optimizer_actor.zero_grad()
            actor_loss.backward()
            nn.utils.clip_grad_norm_(actor.parameters(), config.grad_clip_norm)
            optimizer_actor.step()

    # log stats
    actor_loss, critic_loss, entropy, ratio = compute_loss(states, actions, rewards_to_go, adv, old_log_probs)
    end = time.time()
    wandb.log({
        "actor loss": actor_loss,
        "critic loss": critic_loss,
        "ppo prob ratio": ratio,
        "entropy": entropy,
        "loss computation time": end - start
    })


def train(max_steps=1_000_000):
    ep_rewards = deque(maxlen=150)
    ep_lengths = deque(maxlen=150)

    decision_steps = env.reset()

    step = 0
    total_num_of_games = 0
    last_states = {}
    last_actions = {}
    last_log_probs = {}
    ep_rewards_log = {id: [] for id in decision_steps.agent_id}
    ep_length_log = {id: 0 for id in decision_steps.agent_id}
    while step < max_steps:

        for id in decision_steps.agent_id:
            last_states[id] = torch.tensor(decision_steps[id].obs[0])

        # Generate an action for all agents
        states = decision_steps.obs[0]
        if states.shape[0] == 0:
            actions = env.get_random_actions()
            actions = torch.FloatTensor(actions)
        else:
            states = torch.as_tensor(states, dtype=torch.float32).to(device)
            actions, log_probs, pi = actor(states)

        for i, id in enumerate(decision_steps.agent_id):
            last_actions[id] = actions[i]
            last_log_probs[id] = log_probs[i]

        # Move the simulation forward
        new_decision_steps, terminal_steps = env.step(np.clip(actions.cpu().numpy(), -1, 1))

        for id in terminal_steps.agent_id:
            memory.states[id].append(last_states[id])
            memory.actions[id].append(last_actions[id])
            memory.rewards[id].append(terminal_steps[id].reward)
            memory.dones[id].append(True)
            memory.log_probs[id].append(last_log_probs[id])

            ep_rewards_log[id].append(terminal_steps[id].reward)
            ep_rewards.append(sum(ep_rewards_log[id]))
            ep_rewards_log[id].clear()
            ep_length_log[id] += 1
            ep_lengths.append(ep_length_log[id])
            ep_length_log[id] = 0
            total_num_of_games += 1

        for id in new_decision_steps.agent_id:
            if id in terminal_steps:
                continue
            memory.states[id].append(last_states[id])
            memory.actions[id].append(last_actions[id])
            memory.rewards[id].append(new_decision_steps[id].reward)
            memory.dones[id].append(False)
            memory.log_probs[id].append(last_log_probs[id])

            ep_rewards_log[id].append(new_decision_steps[id].reward)
            ep_length_log[id] += 1

            step += 1

        decision_steps = new_decision_steps

        if memory.size() >= config.memory_size:
            update()
            memory.reset()

            print(f"Step: {step}, Avg reward: {sum(ep_rewards) / len(ep_rewards):.2f}")

            wandb.log({
                "total number of steps": step,
                "total number of games": total_num_of_games,
                "avg reward": sum(ep_rewards) / len(ep_rewards),
                "avg episode length": sum(ep_lengths) / len(ep_lengths)
            }, step=step)


def main():
    train(max_steps=config.max_steps)

    # torch.save(actor_critic.state_dict(), "model.h5")
    # wandb.save('model.h5')


if __name__ == "__main__":
    main()
    env.close()
