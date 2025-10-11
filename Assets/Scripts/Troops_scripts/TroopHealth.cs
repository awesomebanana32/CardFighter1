using UnityEngine;

public class TroopHealth : MonoBehaviour
{
    [SerializeField] private FloatingHealthbar healthbar; // Reference to your healthbar
    [SerializeField] private float maxHealth = 100f;      // Maximum health
    [SerializeField] private int populationCost = 1;      // Population cost of this troop
    private float currentHealth;

    private PlacementSystem placementSystem;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        // Automatically find the PlacementSystem in the scene if not assigned
        if (placementSystem == null)
        {
            placementSystem = Object.FindFirstObjectByType<PlacementSystem>();
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // Reduce population if the troop is tagged as TeamGreen
        if (gameObject.CompareTag("TeamGreen") && placementSystem != null)
        {
            placementSystem.AddToPopulation(-populationCost);
        }

        Destroy(gameObject);
    }
}
