using UnityEngine;

public abstract class State : MonoBehaviour
{
    public virtual void OnEnterState() { }
    public virtual void OnExitState() { }
    public abstract State RunCurrentState();
}
