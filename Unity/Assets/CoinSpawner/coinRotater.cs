using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinRotater : MonoBehaviour
{
    public int speedOfRotation = 100;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,speedOfRotation * Time.deltaTime);
    }
}
