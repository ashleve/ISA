using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCarMovement : Movement
{
    public float forceMultiplier = 60f;
    public float torqueMultiplier = 33f;

    override public void Move(float steering, float forward) 
    {
        Rigidbody rBody = this.GetComponent<Rigidbody>();
        rBody.AddRelativeForce(new Vector3(0, 0, forward * forceMultiplier));
        rBody.AddRelativeTorque(0, steering * torqueMultiplier, 0);
    }

    override public void ResetCar()
    {
        // Nothing to reset
    }
}
