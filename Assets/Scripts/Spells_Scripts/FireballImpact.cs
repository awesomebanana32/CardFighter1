using UnityEngine;

public class FireballImpact : MonoBehaviour
{
    [Header("Damage Settings")]
    public float baseDamage = 20f;
    public string targetTag = "TeamRed";

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float explosionDuration = 2f;

    [Header("Fire Settings")]
    public GameObject firePrefab;
    public float fireOffsetY = 0.1f;

    [Header("Physics Settings")]
    public LayerMask groundLayer;

    [Header("Spell Caster")]
    public GameObject caster; // the unit that cast this fireball

    private bool hasCollided = false;

    private void Update()
    {
        // Safety: detect ground hit if collision missed
        if (!hasCollided)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.6f, groundLayer))
            {
                HandleImpact(hit.point, Quaternion.LookRotation(hit.normal), hit.collider);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        Vector3 pos = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
        Quaternion rot = collision.contactCount > 0 ? Quaternion.LookRotation(collision.GetContact(0).normal) : Quaternion.identity;

        HandleImpact(pos, rot, collision.collider);
    }

    private void HandleImpact(Vector3 explosionPos, Quaternion explosionRot, Collider hitCollider)
    {
        // Damage enemy troops
        if (hitCollider.CompareTag(targetTag))
        {
            TroopHealth health = hitCollider.GetComponent<TroopHealth>();
            if (health != null)
            {
                float scaledDamage = baseDamage;

                // Scale with caster level if available
                if (caster != null)
                {
                    LevelSystem lvl = caster.GetComponent<LevelSystem>();
                    if (lvl != null)
                        scaledDamage *= 1 + lvl.level * 0.1f; // +10% per level
                }

                // Deal damage and pass caster for XP
                health.TakeDamage(scaledDamage, caster);
            }
        }

        // Spawn explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, explosionPos, explosionRot);
            Destroy(explosion, explosionDuration);
        }

        // Spawn fire prefab on ground
        if (firePrefab != null)
        {
            Vector3 firePos = explosionPos;

            // Raycast downward to find ground
            if (Physics.Raycast(explosionPos, Vector3.down, out RaycastHit hit, 5f, groundLayer))
            {
                firePos = hit.point + Vector3.up * fireOffsetY;
            }

            Instantiate(firePrefab, firePos, Quaternion.identity);
        }

        Destroy(gameObject, 0.02f);
    }
}
