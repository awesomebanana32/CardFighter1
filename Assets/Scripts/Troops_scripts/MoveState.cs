using UnityEngine;
using UnityEngine.AI;

public class MoveState : State
{
    public IdleState idleState;
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public override State RunCurrentState()
    {
        if (!hasTarget)
        {
            return idleState; // No target, go idle
        }

        agent.SetDestination(targetPosition);

        // Check if arrived (use agent's built-in check)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            hasTarget = false;
            agent.ResetPath();
            return idleState;
        }

        return this;
    }
}