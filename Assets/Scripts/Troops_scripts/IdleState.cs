using UnityEngine;

public class IdleState : State
{
    public ChaseState chaseState;
    public float visionRange = 10f;
    public Team team;

    private bool canSeeEnemy;
    private float checkInterval = 0.2f;
    private float nextCheckTime = 0f;

    public override State RunCurrentState()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            canSeeEnemy = CheckForEnemies();
        }

        return canSeeEnemy ? chaseState : this;
    }

    private bool CheckForEnemies()
    {
        string[] enemyTags = TeamHelper.GetEnemies(team);

        foreach (string enemyTag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null) continue;
                float distanceSqr = (enemy.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr <= visionRange * visionRange)
                    return true;
            }
        }

        return false;
    }
}
