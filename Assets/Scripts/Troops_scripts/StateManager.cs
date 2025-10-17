using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private MoveState moveState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private string enemyTag = "TeamGreen";
    private State currentState;
    private bool isDead = false;

    void Start()
    {
        currentState = startingState;

        if (currentState != null)
            currentState.OnEnterState();
    }

    void Update()
    {
        // Skip logic if dead or destroyed
        if (isDead || currentState == null)
            return;

        // Safely run state logic
        State nextState = currentState.RunCurrentState();

        if (nextState != null && nextState != currentState)
        {
            currentState.OnExitState();
            currentState = nextState;
            currentState.OnEnterState();
        }
    }

    public void CommandMove(Vector3 position)
    {
        if (isDead) return;

        if (currentState != null)
            currentState.OnExitState();

        if (moveState != null)
        {
            moveState.SetTargetPosition(position);
            currentState = moveState;
            currentState.OnEnterState();
        }
    }

    public void CommandChase(string targetEnemyTag)
    {
        if (isDead) return;

        if (chaseState == null)
        {
            Debug.LogWarning($"{gameObject.name} has no ChaseState assigned!");
            return;
        }

        chaseState.enemyTag = targetEnemyTag;

        if (currentState != null)
            currentState.OnExitState();

        currentState = chaseState;
        currentState.OnEnterState();
    }

    public static void CommandAllToChase(string enemyTag)
    {
   
        StateManager[] allTroops = Object.FindObjectsByType<StateManager>(FindObjectsSortMode.None);
        int troopsCommanded = 0;

        foreach (StateManager troop in allTroops)
        {
            if (troop != null && troop.chaseState != null)
            {
                troop.CommandChase(enemyTag);
                troopsCommanded++;
            }
        }

        Debug.Log($"Chase command sent to {troopsCommanded} troops targeting {enemyTag}");
    }

    public void CleanupBeforeDeath()
    {
        if (isDead) return;
        isDead = true;

        if (currentState != null)
        {
            currentState.OnExitState();
            currentState = null;
        }

        // Disable NavMeshAgent or any movement system
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        // Optional: disable all colliders or visuals
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    public void Die()
    {
        CleanupBeforeDeath();
        Destroy(gameObject);
    }
}
