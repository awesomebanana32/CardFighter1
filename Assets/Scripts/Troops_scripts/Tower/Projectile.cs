using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;      // Max time before self-destruct
    [SerializeField] private string targetTag = "PlayerTroop"; // Target collision tag

    [Header("Projectile Stats")]
    public int baseDamage = 10;   // Base damage, set by tower/unit

    [Header("Optional Caster")]
    public GameObject caster;     // Unit that fired this projectile

    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Self-destruct after lifetime
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null)
            {
                float scaledDamage = baseDamage;

                // Scale damage with caster level if available
                if (caster != null)
                {
                    LevelSystem lvl = caster.GetComponent<LevelSystem>();
                    if (lvl != null)
                        scaledDamage *= 1 + lvl.level * 0.1f; // +10% per level
                }

                // Deal damage and pass caster for XP
                health.TakeDamage(scaledDamage, caster);
            }

            // Destroy projectile
            Destroy(gameObject);
        }
    }
}
