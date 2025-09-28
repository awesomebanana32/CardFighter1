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

        float distance = Vector3.Distance(transform.position, nearestEnemy.position);

        if (distance <= attackRange)
        {
            agent.ResetPath();
            attackState.SetTarget(nearestEnemy);
            return attackState;
        }
        else
        {
            agent.SetDestination(nearestEnemy.position);
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
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        nearestEnemy = closestEnemy;
    }
}
