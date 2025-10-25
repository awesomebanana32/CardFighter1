using UnityEngine;

public class LightningDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 50f;
    public float lifetime = 1f; // lightning lasts this long
    public bool destroyOnHit = true;

    [Header("Optional Effects")]
    public GameObject hitEffect; // sparks or explosion prefab

    private void Start()
    {
        // Auto-destroy after a short time (cleanup)
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it hit an enemy troop or building
        if (other.CompareTag("TeamRed"))
        {
            // Use your TroopHealth component
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Optional: create visual hit effect
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            }

            // Optionally destroy after hitting one target
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
