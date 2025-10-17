using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    public AttackState attackState;
    public float attackRange = 2f;
    public float visionRange = 10f;
    public string enemyTag = ""; // Set this per prefab or via CommandChase

    private NavMeshAgent agent;
    private Transform nearestEnemy;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnEnterState()
    {
        // Ensure agent is initialized
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Reset path to start fresh
        if (agent != null)
        {
            agent.ResetPath();
        }

        // Clear previous target
        nearestEnemy = null;

        // Log for debugging
        Debug.Log($"{gameObject.name} entered ChaseState targeting {enemyTag}");
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
            agent.ResetPath(); // Stop moving
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