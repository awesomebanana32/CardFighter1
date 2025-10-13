using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private MoveState moveState;
    private State currentState;

    void Start()
    {
        // Initialize starting state
        currentState = startingState;

        if (currentState != null)
        {
            currentState.OnEnterState();
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            State nextState = currentState.RunCurrentState();

            if (nextState != null && nextState != currentState)
            {
                currentState.OnExitState();
                currentState = nextState;
                currentState.OnEnterState();
            }
        }
    }

    // Command troop to move — interrupts attack or other states
    public void CommandMove(Vector3 position)
    {
        // Exit current state if needed
        if (currentState != null)
        {
            currentState.OnExitState();
        }

        // Switch to move state
        if (moveState != null)
        {
            moveState.SetTargetPosition(position);
            currentState = moveState;
            currentState.OnEnterState();
        }
    }
}
