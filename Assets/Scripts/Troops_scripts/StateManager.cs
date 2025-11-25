using UnityEngine;
using UnityEngine.AI;

public class StateManager : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private MoveState moveState;
    [SerializeField] private ChaseState chaseState;
    public Team team;

    private State currentState;
    private bool isDead = false;

    private void Start()
    {
        currentState = startingState;
        currentState?.OnEnterState();
    }

    private void Update()
    {
        if (isDead || currentState == null)
            return;

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
        if (isDead || moveState == null) return;

        currentState?.OnExitState();

        moveState.SetTargetPosition(position);
        currentState = moveState;
        currentState.OnEnterState();
    }

    public void CommandChase()
    {
        if (isDead || chaseState == null) return;

        currentState?.OnExitState();

        currentState = chaseState;
        currentState.OnEnterState();
    }

    public static void CommandAllToChase(string enemyTag)
    {
        StateManager[] allTroops = Object.FindObjectsOfType<StateManager>();
        foreach (StateManager troop in allTroops)
        {
            if (troop != null && troop.chaseState != null)
                troop.CommandChase();
        }
    }

    public void CleanupBeforeDeath()
    {
        if (isDead) return;
        isDead = true;

        currentState?.OnExitState();
        currentState = null;

        var agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    public void Die()
    {
        CleanupBeforeDeath();
        Destroy(gameObject);
    }
}
