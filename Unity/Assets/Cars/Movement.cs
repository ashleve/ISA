using UnityEngine;

public abstract class Movement : MonoBehaviour {
    public abstract void move(float steering, float force);

    public abstract void reset();
}
