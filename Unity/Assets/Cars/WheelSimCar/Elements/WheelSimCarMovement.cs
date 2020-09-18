using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSimCarMovement : Movement
{
    private WheelCollider[] wheelColliders = new WheelCollider[4];

    public float torque = 8f;

    override public void move(float right, float forward) {
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

    public override void reset()
    {
        // Nothing to reset
    }
}
