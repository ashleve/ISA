using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class BoxCollectingAgent : Agent
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
            this.transform.localPosition = new Vector3(0, 1.2f, 0);
            this.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            this.movement.ResetCar();
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(
            Random.value * 8.5f - 4,
            0.5f,
            Random.value * 8.5f - 4
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition.x);
        sensor.AddObservation(Target.localPosition.z);
        sensor.AddObservation(this.transform.localPosition.x);
        sensor.AddObservation(this.transform.localPosition.z);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        // Agent angular
        sensor.AddObservation(rBody.angularVelocity.x);
        sensor.AddObservation(rBody.angularVelocity.y);
        sensor.AddObservation(rBody.angularVelocity.z);

        // Agent direction
        sensor.AddObservation(this.transform.forward.x);
        sensor.AddObservation(this.transform.forward.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Debug.DrawRay(transform.position, this.transform.forward * 4, Color.green);

        SetReward(-0.003f);

        float steering = vectorAction[0];
        float force = vectorAction[1];
        movement.Move(steering, force);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.4f)
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
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    private bool IsFlipped(Transform transform) {
        return Mathf.Abs(Vector3.Dot(transform.up, Vector3.down)) < 0.985f;
    }
}
