using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    public AttackState attackState;
    public float attackRange = 2f;
    public float visionRange = 10f;
    public string enemyTag = ""; // set this per prefab (TeamRed targets TeamGreen, and vice versa)

    private NavMeshAgent agent;
    private Transform nearestEnemy;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override State RunCurrentState()
    {
        FindNearestEnemy();

        if (nearestEnemy == null)
        {
            agent.ResetPath();
            return this;
        }

        Collider enemyCollider = nearestEnemy.GetComponent<Collider>();
        if (enemyCollider == null)
        {
            return this;
        }

        // Use collider closest point instead of pivot
        Vector3 closestPoint = enemyCollider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(transform.position, closestPoint);

        // If within attack range, stop and switch to attack
        if (distance <= attackRange)
        {
            agent.ResetPath(); // stop moving
            attackState.SetTarget(nearestEnemy);
            return attackState;
        }
        else
        {
            // Move toward the closest point on the enemy collider
            agent.SetDestination(closestPoint);
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
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider == null) continue;

            // Use closest point to respect enemy size
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
