using UnityEngine;

public abstract class Movement : MonoBehaviour {
    public abstract void move(CarAgent carAgent, int right, int forward);
}
