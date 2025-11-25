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
            return idleState;

        if (!agent.hasPath || agent.destination != targetPosition)
            agent.SetDestination(targetPosition);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            hasTarget = false;
            agent.ResetPath();
            return idleState;
        }

        return this;
    }
}
