using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


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

public class SteeringWheelSimCarMovement : Movement
{

    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float turnSensitivity = 1.0f;
    [SerializeField]
    private float maxSteerAngle = 45.0f;
    [SerializeField]
    private Vector3 _centerOfMass;
    [SerializeField]
    private List<Wheel> wheels;

    private float inputX, inputY;

    private Rigidbody _rb;

    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centerOfMass;
    }

    
    private void FixedUpdate()
    {
        AnimateWheels();
        //GetInputs();
        SetTorque();
        Turn();
    }

    private void GetInputs()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
    }

    private void SetTorque()
    {
        foreach (var wheel in wheels)
        {
            wheel.collider.brakeTorque = 0f;
            wheel.collider.motorTorque = inputY * maxAcceleration * 500f * Time.fixedDeltaTime;
            // Debug.Log(wheel.collider.motorTorque);
        }
    }

    private void Turn()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = inputX * turnSensitivity * maxSteerAngle;
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, _steerAngle, 0.5f);
            }
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
            wheel.model.transform.rotation = _rot;
        }
    }

    override public void Move(float steering, float force)
    {
        inputX = steering;
        inputY = force;
    }

    public override void ResetCar()
    {
        // Debug.Log("Reset wheels");
        // Reset wheel torque
        foreach (var wheel in wheels)
        {
            wheel.collider.motorTorque = 0f;
            wheel.collider.brakeTorque = 100000f;
        }
        // Reset wheel steering
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                wheel.collider.steerAngle = 0f;
            }
        }
    }
}
