using UnityEngine;

public class AttackState : State
{
    public float attackCooldown = 1f;
    public float attackRange = 2f;  // consistent with ChaseState
    public int damage = 10;
    private float lastAttackTime = 0f;

    private Transform target;
    public ChaseState chaseState;

    public override State RunCurrentState()
    {
        if (target == null)
            return chaseState;

        // Face the target
        Vector3 dir = (target.position - transform.position).normalized;
        transform.forward = dir;

        float distance = Vector3.Distance(transform.position, target.position);

        // Attack if in range and cooldown passed
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            TroopHealth health = target.GetComponent<TroopHealth>();
            if (health != null)
                health.TakeDamage(damage);

            lastAttackTime = Time.time;
            Debug.Log($"{gameObject.name} attacked {target.name} for {damage} damage.");
        }

        // Return to chase if target moves away
        if (distance > attackRange)
            return chaseState;

        return this;
    }

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }
}
