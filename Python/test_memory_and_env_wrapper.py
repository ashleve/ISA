from mlagents_envs.environment import UnityEnvironment
from utils import EnvWrapper, Memory
import time


# This is a non-blocking call that only loads the environment.
env = UnityEnvironment()
env = EnvWrapper(env)

memory = Memory(env.agent_ids)

start = time.time()

decision_steps = env.reset()
step = 0
last_states = {}
last_actions = {}
last_log_probs = {}
while step < 10000:

    for id in decision_steps.agent_id:
        last_states[id] = decision_steps[id].obs[0]

    # states = decision_steps.obs[0]
    # if states.shape[0] == 0:
    #     actions = env.get_random_actions()
    # else:
    #     actions = model(states)

    # Generate an action for all agents
    actions = env.get_random_actions()

    for i, id in enumerate(decision_steps.agent_id):
        last_actions[id] = actions[i]
        # last_log_probs[id] = log_probs[i]

    # Move the simulation forward
    new_decision_steps, terminal_steps = env.step(actions)

    for id in terminal_steps.agent_id:
        memory.states[id].append(last_states[id])
        memory.actions[id].append(last_actions[id])
        memory.rewards[id].append(terminal_steps[id].reward)
        memory.dones[id].append(True)
        # memory.log_probs[id].append(last_log_probs[id])

    for id in new_decision_steps.agent_id:
        if id in terminal_steps:
            continue
        memory.states[id].append(last_states[id])
        memory.actions[id].append(last_actions[id])
        memory.rewards[id].append(new_decision_steps[id].reward)
        memory.dones[id].append(False)
        # memory.log_probs[id].append(last_log_probs[id])

        step += 1

    decision_steps = new_decision_steps


end = time.time()
print("Execution time:", end - start)
print("Memory size:", memory.size())


print(len(memory.states[0]))
print(len(memory.actions[0]))
print(len(memory.rewards[0]))
print(len(memory.dones[0]))

env.close()
