using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    public AttackState attackState;
    public float attackRange = 2f;
    public float visionRange = 10f;
    public string enemyTag = "";

    private NavMeshAgent agent;
    private Transform nearestEnemy;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnEnterState()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null && agent.isOnNavMesh)
            agent.ResetPath();

        nearestEnemy = null;
        //Debug.Log($"{gameObject.name} entered ChaseState targeting {enemyTag}");
    }

    public override State RunCurrentState()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return this;

        FindNearestEnemy();

        if (nearestEnemy == null)
        {
            agent.ResetPath();
            return this;
        }

        Collider enemyCollider = nearestEnemy.GetComponent<Collider>();
        if (enemyCollider == null)
            return this;

        Vector3 closestPoint = enemyCollider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(transform.position, closestPoint);

        // If target destroyed mid-run
        if (!nearestEnemy.gameObject.activeInHierarchy)
        {
            nearestEnemy = null;
            agent.ResetPath();
            return this;
        }

        if (distance <= attackRange)
        {
            agent.ResetPath();
            attackState.SetTarget(nearestEnemy);
            return attackState;
        }

        // Avoid crash if SetDestination fails
        try
        {
            if (agent.isOnNavMesh)
                agent.SetDestination(closestPoint);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"{gameObject.name} failed to SetDestination: {e.Message}");
        }

        return this;
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider == null) continue;

            Vector3 closestPoint = enemyCollider.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position, closestPoint);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        nearestEnemy = closestEnemy;
    }
}
