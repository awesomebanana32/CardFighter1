using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Projectile Settings")]
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Firing position

    [Header("Target Settings")]
    [SerializeField] private string targetTag = "PlayerTroop"; // Enemy tag

    private float lastAttackTime = 0f;
    private Transform currentTarget;

    void Update()
    {
        FindNearestTargetInRange();

        if (currentTarget != null && Time.time - lastAttackTime >= attackCooldown)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    private void FindNearestTargetInRange()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);
        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject potential in potentialTargets)
        {
            float distance = Vector3.Distance(transform.position, potential.transform.position);
            if (distance <= attackRange && distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = potential.transform;
            }
        }

        currentTarget = nearest;
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null)
            return;

        // Direction and rotation
        Vector3 direction = (currentTarget.position - firePoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Instantiate projectile
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, rotation);

        // Set damage and caster
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.baseDamage = baseDamage;

            // Assign this tower/unit as the caster for XP and scaling
            projectileScript.caster = this.gameObject;
        }
    }
}
