using UnityEngine;

public abstract class State : MonoBehaviour
{
    // Called once when the state becomes active
    public virtual void OnEnterState() { }

    // Called once when leaving the state
    public virtual void OnExitState() { }

    // Called every frame while the state is active
    public abstract State RunCurrentState();
}
