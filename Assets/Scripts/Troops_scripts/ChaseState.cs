using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    public AttackState attackState;
    public float attackRange = 2f;
    public Team team;

    private NavMeshAgent agent;
    private Transform nearestEnemy;
    private float checkInterval = 0.2f;
    private float nextCheckTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnEnterState()
    {
        nearestEnemy = null;
        agent?.ResetPath();
    }

    public override State RunCurrentState()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return this;

        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            FindNearestEnemy();
        }

        if (nearestEnemy == null)
        {
            agent.ResetPath();
            return this;
        }

        Collider enemyCollider = nearestEnemy.GetComponent<Collider>();
        if (enemyCollider == null)
            return this;

        Vector3 closestPoint = enemyCollider.ClosestPoint(transform.position);
        float distanceSqr = (closestPoint - transform.position).sqrMagnitude;

        if (!nearestEnemy.gameObject.activeInHierarchy)
        {
            nearestEnemy = null;
            agent.ResetPath();
            return this;
        }

        if (distanceSqr <= attackRange * attackRange)
        {
            agent.ResetPath();
            attackState.SetTarget(nearestEnemy);
            return attackState;
        }

        if (agent.isOnNavMesh)
            agent.SetDestination(closestPoint);

        return this;
    }

    private void FindNearestEnemy()
    {
        string[] enemyTags = TeamHelper.GetEnemies(team);
        float shortestDistanceSqr = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (string enemyTag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null) continue;
                Collider col = enemy.GetComponent<Collider>();
                if (col == null) continue;

                Vector3 closestPoint = col.ClosestPoint(transform.position);
                float distanceSqr = (closestPoint - transform.position).sqrMagnitude;

                if (distanceSqr < shortestDistanceSqr)
                {
                    shortestDistanceSqr = distanceSqr;
                    closestEnemy = enemy.transform;
                }
            }
        }

        nearestEnemy = closestEnemy;
    }
}
