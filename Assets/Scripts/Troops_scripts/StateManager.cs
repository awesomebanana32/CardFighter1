using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private MoveState moveState;
    [SerializeField] private ChaseState chaseState; // Reference to ChaseState
    [SerializeField] private string enemyTag = "TeamGreen"; // Enemy tag for this troop
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

    // Method to switch this troop to chase state
    public void CommandChase(string targetEnemyTag)
    {
        if (chaseState == null)
        {
            Debug.LogWarning($"{gameObject.name} has no ChaseState assigned!");
            return;
        }

        // Set enemy tag for chase state
        chaseState.enemyTag = targetEnemyTag;

        // Exit current state
        if (currentState != null)
        {
            currentState.OnExitState();
        }

        // Switch to chase state
        currentState = chaseState;
        currentState.OnEnterState();
    }

    // Static method to command all troops to chase
    public static void CommandAllToChase(string enemyTag)
    {
        StateManager[] allTroops = FindObjectsOfType<StateManager>();
        int troopsCommanded = 0;

        foreach (StateManager troop in allTroops)
        {
            if (troop.chaseState != null)
            {
                troop.CommandChase(enemyTag);
                troopsCommanded++;
            }
        }

        Debug.Log($"Chase command sent to {troopsCommanded} troops targeting {enemyTag}");
    }
}