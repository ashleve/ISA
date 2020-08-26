using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    protected Rigidbody rBody;
    protected Movement movement;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
    }

    public Transform Target;
    
    
    public float resetHeight = 0.038f;
    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < resetHeight || IsFlipped(this.transform))
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.6f, 0);
            this.transform.localRotation = Quaternion.identity;
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.6f,
                                           Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        int right = 0, forward = 0;
        int direction = Mathf.FloorToInt(vectorAction[0]);
        if(direction == 1) {right = -1;}
        if(direction == 2) {right = 1;}
        if(direction == 3) {forward = -1;}
        if(direction == 4) {forward = 1;}

        movement.move(rBody, right, forward);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        if (this.transform.localPosition.y < resetHeight || IsFlipped(this.transform))
        {
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        // Possible directions
        int direction = 0;
        if(Input.GetKey("w")) direction = 4;
        if(Input.GetKey("a")) direction = 1;
        if(Input.GetKey("s")) direction = 3;
        if(Input.GetKey("d")) direction = 2;
        
        actionsOut[0] = direction;
    }

    private bool IsFlipped(Transform transform) {
        return Mathf.Abs(Vector3.Dot(transform.up, Vector3.down)) < 0.925f;
    }
}
