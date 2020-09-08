from mlagents_envs.environment import UnityEnvironment


env = UnityEnvironment()
env.reset()

behavior_name = list(env.behavior_specs)[0]
print(f"Name of the behavior : {behavior_name}")

spec = env.behavior_specs[behavior_name]
print("Action shape:", spec.action_shape)
print("Action size:", spec.action_size)
print("Action type:", spec.action_type)
print("State shape:", spec.observation_shapes[0])
print("State size:", spec.observation_shapes[0][0])


for episode in range(100):
    env.reset()

    decision_steps, terminal_steps = env.get_steps(behavior_name)
    tracked_agent = decision_steps.agent_id[0]
    done = False
    episode_rewards = 0

    while not done:

        # Generate an action for all agents
        action = spec.create_random_action(len(decision_steps))

        # Set the actions
        env.set_actions(behavior_name, action)

        # Move the simulation forward
        env.step()

        # Get the new simulation results
        decision_steps, terminal_steps = env.get_steps(behavior_name)

        if tracked_agent in decision_steps:  # The agent requested a decision
            episode_rewards += decision_steps[tracked_agent].reward

        if tracked_agent in terminal_steps:  # The agent terminated its episode
            episode_rewards += terminal_steps[tracked_agent].reward
            done = True

    print(f"Total rewards for episode {episode} is {episode_rewards}")


env.close()
