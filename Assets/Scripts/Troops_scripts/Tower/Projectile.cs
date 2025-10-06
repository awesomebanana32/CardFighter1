using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f; // Max time before self-destruct if no hit
    [SerializeField] private string targetTag = "PlayerTroop"; // Same as tower's targetTag, for collision check

    public int damage; // Set by the tower when instantiating

    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // Move forward in the direction it's facing
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Self-destruct after lifetime
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
{
    Debug.Log($"Projectile hit: {other.name} with tag '{other.tag}'"); // This will spam in Console—check if it fires and what tag it sees

    if (other.CompareTag(targetTag))
    {
        Debug.Log("Tag match! Applying damage."); // Confirm this logs
        TroopHealth health = other.GetComponent<TroopHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"{gameObject.name} hit {other.name} for {damage} damage.");
        }
        else
        {
            Debug.LogWarning("TroopHealth component missing on hit target!");
        }
        Destroy(gameObject);
    }
}
}