using UnityEngine;

public abstract class Movement : MonoBehaviour {
    public abstract void move(CarAgent carAgent, float right, float forward);
}
