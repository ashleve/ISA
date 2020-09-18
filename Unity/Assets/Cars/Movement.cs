using UnityEngine;

public abstract class Movement : MonoBehaviour {
    public abstract void Move(float steering, float force);

    public abstract void ResetCar();
}
