using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class CollisionPunishment : MonoBehaviour
{

    protected Agent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponentInParent<Agent>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            // Debug.Log(collision.relativeVelocity);


            agent.SetReward(-collision.relativeVelocity.magnitude);
        }
    }

}
