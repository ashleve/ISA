using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class DrivingCarOn2WheelsAgent : Agent
{
    protected Rigidbody rBody;
    protected Movement movement;

    private Vector3 spawnPos;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        spawnPos = this.transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            EndEpisode();
        }

        // print(transform.localEulerAngles);
    }

    private void FixedUpdate()
    {
        // reward for rotation up to 50 degrees
        float forwardVelocity = rBody.transform.InverseTransformDirection(rBody.velocity).z;
        float rotationZ = transform.localEulerAngles.z;
        if (rotationZ < 45) // for right side
        {
            SetReward(forwardVelocity * Mathf.Clamp(rotationZ, 0, 25) / 25);
        }
        else if (rotationZ > 315) // for left side
        {
            rotationZ = 360f - rotationZ;
            SetReward(forwardVelocity * Mathf.Clamp(rotationZ, 0, 25) / 25);
        }
        /*else if(rotationZ > 90 && rotationZ < 270)
        {
            SetReward(-1);
        }*/
    }

    public override void OnEpisodeBegin()
    {
        // zero agent's momentum
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        // this.transform.localPosition = new Vector3(0, 1.2f, 0);
        this.transform.localPosition = spawnPos;
        // this.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        this.transform.rotation = Quaternion.identity;
        this.movement.ResetCar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent positions
        sensor.AddObservation(this.transform.localPosition.x);
        sensor.AddObservation(this.transform.localPosition.z);

        // Agent rotation
        sensor.AddObservation(this.transform.localEulerAngles.x);
        sensor.AddObservation(this.transform.localEulerAngles.y);
        sensor.AddObservation(this.transform.localEulerAngles.z);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        // Agent angular velocity
        sensor.AddObservation(rBody.angularVelocity.x);
        sensor.AddObservation(rBody.angularVelocity.y);
        sensor.AddObservation(rBody.angularVelocity.z);

        // Agent direction
        sensor.AddObservation(this.transform.forward.x);
        sensor.AddObservation(this.transform.forward.y);
        sensor.AddObservation(this.transform.forward.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float steering = vectorAction[0];
        float force = vectorAction[1];
        movement.Move(steering, force);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            // wall collision penalty
            // Debug.Log(collision.relativeVelocity.magnitude);
            SetReward(-4 * collision.relativeVelocity.magnitude);
        }
    }
}
