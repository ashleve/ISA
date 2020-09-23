using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class WheelSimCarMovement : Movement
{

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject model;
        public WheelCollider collider;
        public Axel axel;
    }


    [SerializeField]
    private float torque = 8f;
    [SerializeField]
    private Vector3 _centerOfMass;
    [SerializeField]
    private List<Wheel> wheels;

    private Rigidbody _rb;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centerOfMass;
    }

    private void FixedUpdate()
    {
        AnimateWheels();
    }

    public override void Move(float steering, float force)
    {
        foreach(var wheel in wheels)
            wheel.collider.brakeTorque = 0f;

        steering = (steering + 1) / 2;

        wheels[0].collider.motorTorque = torque * steering * force;
        wheels[2].collider.motorTorque = torque * steering * force;

        wheels[1].collider.motorTorque = torque * (1 - steering) * force;
        wheels[3].collider.motorTorque = torque * (1 - steering) * force;
    }

    public override void ResetCar()
    {
        // Reset wheel torque
        foreach (var wheel in wheels)
        {
            wheel.collider.motorTorque = 0f;
            wheel.collider.brakeTorque = 100000f;
        }
    }

    private void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion _rot;
            Vector3 _pos;
            wheel.collider.GetWorldPose(out _pos, out _rot);
            wheel.model.transform.position = _pos;

            Vector3 currentRot = wheel.model.transform.eulerAngles;
            wheel.model.transform.eulerAngles = new Vector3(_rot.eulerAngles.x, _rot.eulerAngles.y, _rot.eulerAngles.z - 90);
        }
    }
}
