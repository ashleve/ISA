class EnvWrapper:
    def __init__(self, env):
        self.env = env
        self.env.reset()

        self.behavior_name = list(env.behavior_specs)[0]
        self._spec = env.behavior_specs[self.behavior_name]
        self.agent_ids = self.env.get_steps(self.behavior_name)[0].agent_id

        self.state_shape = self._spec.observation_shapes[0]
        self.state_size = self.state_shape[0]
        self.action_size = self._spec.action_size

    def step(self, actions):
        self.env.set_actions(self.behavior_name, actions)
        self.env.step()
        return self.env.get_steps(self.behavior_name)

    def reset(self):
        self.env.reset()
        decision_steps, _ = self.env.get_steps(self.behavior_name)
        return decision_steps

    def get_random_actions(self):
        decision_steps, _ = self.env.get_steps(self.behavior_name)
        return self._spec.create_random_action(len(decision_steps))

    def close(self):
        self.env.close()


class Memory:
    def __init__(self, agent_ids):
        self.agent_ids = agent_ids
        self.states = {}
        self.actions = {}
        self.rewards = {}
        self.dones = {}
        self.log_probs = {}
        self.state_values = {}
        self.reset()

    def reset(self):
        for id in self.agent_ids:
            self.states[id] = []
            self.actions[id] = []
            self.rewards[id] = []
            self.dones[id] = []
            self.log_probs[id] = []
            self.state_values[id] = []

    def size(self):
        return sum(len(self.states[id]) for id in self.agent_ids)

