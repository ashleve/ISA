# ISA
skool project


## How to run
1. Create new conda environment "mlagents"
2. In terminal activate conda environment <br>
`conda activate mlagents`
3. Navigate to the main folder and run <br>
`mlagents-learn config/rollerball_config.yaml --run-id=RollerBall`
4. Open Unity project in editor and press play (you should see training now)
5. To see tensorboard results, open another terminal, navigate to the main folder, activate conda environment and run <br>
`tensorboard --logdir=results --port=6006`
