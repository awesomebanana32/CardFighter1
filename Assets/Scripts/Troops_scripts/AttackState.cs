using UnityEngine;

public class AttackState : State
{
    public float attackCooldown = 1f;
    public float attackRange = 2f;
    public int damage = 10;

    private float lastAttackTime = 0f;
    private Transform target;

    public ChaseState chaseState;

    private LevelSystem levelSystem;

    private void Awake()
    {
        levelSystem = GetComponent<LevelSystem>();
    }

    public override State RunCurrentState()
    {
        if (target == null)
            return chaseState;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.forward = dir;

        Collider col = target.GetComponent<Collider>();
        if (col == null)
            return chaseState;

        float distanceSqr = (col.ClosestPoint(transform.position) - transform.position).sqrMagnitude;

        if (distanceSqr <= attackRange * attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            TroopHealth health = target.GetComponent<TroopHealth>();

            if (health != null)
            {
                float dmg = (levelSystem != null) ? levelSystem.currentDamage : damage;
                health.TakeDamage(dmg, this.gameObject); // PASS ATTACKER
            }

            lastAttackTime = Time.time;
        }

        if (distanceSqr > attackRange * attackRange)
            return chaseState;

        return this;
    }

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }
}
