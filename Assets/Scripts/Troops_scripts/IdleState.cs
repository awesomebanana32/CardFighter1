using UnityEngine;

public class IdleState : State
{
    public ChaseState chaseState;
    public float visionRange = 10f; // Add this
    public string enemyTag = ""; // Set per prefab, like in ChaseState

    private bool canSeeTheEnemy; // Existing

    public override State RunCurrentState()
    {
        canSeeTheEnemy = CheckForEnemies(); // New check
        if (canSeeTheEnemy)
        {
            return chaseState;
        }
        else
        {
            return this;
        }
    }

    private bool CheckForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= visionRange)
            {
                return true;
            }
        }
        return false;
    }
}