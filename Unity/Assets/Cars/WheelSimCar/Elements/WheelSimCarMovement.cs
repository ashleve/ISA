using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSimCarMovement : Movement
{
    private WheelCollider[] wheelColliders;

    public float torque = 8f;


    private void Start()
    {
        wheelColliders = this.GetComponentsInChildren<WheelCollider>();
    }

    public override void Move(float steering, float force)
    {
        steering = (steering + 1) / 2;

        wheelColliders[0].motorTorque = torque * steering * force;
        wheelColliders[2].motorTorque = torque * steering * force;

        wheelColliders[1].motorTorque = torque * (1 - steering) * force;
        wheelColliders[3].motorTorque = torque * (1 - steering) * force;
    }

    public override void ResetCar()
    {
        // Nothing to reset
    }
}
