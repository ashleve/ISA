using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCarMovement : Movement
{
    public float forceMultiplier = 60f;
    public float torqueMultiplier = 33f;

    override public void move(Rigidbody rBody, int right, int forward) {
        rBody.AddRelativeForce(new Vector3(0, 0, forward * forceMultiplier));
        rBody.AddRelativeTorque(0, right * torqueMultiplier, 0);
    }
}
