using UnityEngine;

public class coinRotater : MonoBehaviour
{
    public int speedOfRotation = 100;

    private Vector3 position;

    public float floatingFrequency = 1.2f;

    public float floatingAmplitude = 0.2f;
    
    // Update is called once per frame
    private void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        transform.Rotate(0,0 ,speedOfRotation * Time.deltaTime);
        transform.position = position + new Vector3(0 ,  floatingAmplitude * Mathf.Sin(Time.time * floatingFrequency), 0 );
    }
}