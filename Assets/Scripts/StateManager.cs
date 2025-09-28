using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private State startingState;
    private State currentState;

    void Start()
    {
        currentState = startingState;
    }

    void Update()
    {
        if (currentState != null)
        {
            State nextState = currentState.RunCurrentState();
            if (nextState != currentState && nextState != null)
            {
                currentState = nextState;
            }
        }
    }
}
