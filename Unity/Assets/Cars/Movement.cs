using UnityEngine;

public abstract class Movement : MonoBehaviour {
    public abstract void move(Rigidbody rBody, int right, int forward);
}
