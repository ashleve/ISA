Simple environment from this tutorial: <br>
https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Create-New.md

## How to run
1. Create new conda environment, name it for example "mlagents"
2. In the new environment install this package <br>
`pip install mlagents`
3. In terminal activate conda environment <br>
`conda activate mlagents`
4. Navigate to the main folder and run <br>
`mlagents-learn config/rollerball_config.yaml --run-id=RollerBall`
5. Open Unity project in editor and press play (you should see training now)
6. To see tensorboard results, open another terminal, navigate to the main folder, activate conda environment and run <br>
`tensorboard --logdir=results --port=6006`
