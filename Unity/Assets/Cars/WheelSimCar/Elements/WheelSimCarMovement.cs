using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSimCarMovement : Movement
{
    private WheelCollider[] wheelColliders = new WheelCollider[4];

    public float torque = 8f;

    override public void move(CarAgent carAgent, float right, float forward) {
        // Rigidbody rBody = carAgent.GetComponent<Rigidbody>();
        // rBody.AddRelativeForce(new Vector3(0, 0, forward * forceMultiplier));
        // rBody.AddRelativeTorque(0, right * torqueMultiplier, 0);
        // WheelCollider collider = carAgent.GetComponents<WheelCollider>();
        wheelColliders = this.GetComponentsInChildren<WheelCollider>();

        foreach(WheelCollider wheel in wheelColliders) {
            wheel.motorTorque = torque * forward;
        }

        if(right != 0 || forward == 0) {
            wheelColliders[0].motorTorque = torque * right * 1f;
            wheelColliders[2].motorTorque = torque * right * 1f;

            wheelColliders[1].motorTorque = torque * right * -1f;
            wheelColliders[3].motorTorque = torque * right * -1f;
        }
        
    }
}
