using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Assign a child transform in the editor for the firing position
    [SerializeField] private string targetTag = "PlayerTroop"; // Set this to the tag of the player's troops (e.g., "TeamRed" or whatever your player's team tag is)

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
        {
            return;
        }

        // Calculate direction and rotation towards the target
        Vector3 direction = (currentTarget.position - firePoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Instantiate the projectile
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, rotation);

        // Set damage on the projectile
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.damage = damage;
        }
    }
}