using UnityEngine;

public class LightningDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float baseDamage = 50f;      // Base damage
    public float lifetime = 1f;         // How long lightning exists
    public bool destroyOnHit = true;

    [Header("Optional Effects")]
    public GameObject hitEffect;

    [Header("Spell Caster")]
    public GameObject caster;           // The unit that cast this spell

    private void Start()
    {
        Destroy(gameObject, lifetime); // Auto cleanup
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for enemy troop (adjust tag as needed)
        if (other.CompareTag("TeamRed"))
        {
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null)
            {
                float scaledDamage = baseDamage;

                // If caster has LevelSystem, scale damage
                if (caster != null)
                {
                    LevelSystem lvl = caster.GetComponent<LevelSystem>();
                    if (lvl != null)
                        scaledDamage *= 1 + lvl.level * 0.1f; // +10% per level
                }

                // Deal damage, XP goes to caster automatically
                health.TakeDamage(scaledDamage, caster);
            }

            // Visual effect
            if (hitEffect != null)
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);

            // Destroy lightning on hit if needed
            if (destroyOnHit)
                Destroy(gameObject);
        }
    }
}
