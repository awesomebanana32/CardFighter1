using UnityEngine;

public class FireArea : MonoBehaviour
{
    public int damage = 10;
    public string targetTag = "TeamRed";
    public float duration = 5f; // how long fire stays before disappearing

    private void Start()
    {
        // Automatically destroy the fire after a few seconds
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log($" {other.name} took {damage} fire damage!");
            }
        }
    }
}
