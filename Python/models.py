import torch
import torch.nn as nn
from torch.distributions import Normal


class Actor(nn.Module):
    def __init__(self, config):
        super(Actor, self).__init__()

        self.actor = nn.Sequential(
            nn.Linear(config.obs_space, config.hidden_size),
            nn.Tanh(),
            nn.Linear(config.hidden_size, 64),
            nn.Tanh(),
            nn.Linear(64, config.action_space),
            nn.Tanh()
        )

        self.log_std = nn.Parameter(torch.zeros(config.action_space), requires_grad=True)

    def forward(self, state):
        mean = self.actor(state)
        std = self.log_std.expand_as(mean).exp()
        pi = Normal(mean, std)
        a = pi.sample()
        return a, pi.log_prob(a).sum(1, keepdim=True), pi


class Critic(nn.Module):
    def __init__(self, config):
        super(Critic, self).__init__()

        self.critic = nn.Sequential(
            nn.Linear(config.obs_space, config.hidden_size),
            nn.Tanh(),
            nn.Linear(config.hidden_size, 64),
            nn.Tanh(),
            nn.Linear(64, 1)
        )

    def forward(self, state):
        v = self.critic(state)
        return v
