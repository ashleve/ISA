using UnityEngine;

public class coinRotater : MonoBehaviour
{
    [SerializeField]
    private int speedOfRotation = 100;
    [SerializeField]
    private float floatingFrequency;
    [SerializeField]
    private float floatingAmplitude;

    private Vector3 position;

    // Update is called once per frame
    private void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        transform.Rotate(0, 0, speedOfRotation * Time.deltaTime);
        transform.position = position + new Vector3(0,  floatingAmplitude * Mathf.Sin(Time.time * floatingFrequency), 0);
    }
}