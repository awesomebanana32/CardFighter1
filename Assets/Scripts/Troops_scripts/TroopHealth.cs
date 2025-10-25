using UnityEngine;

public class TroopHealth : MonoBehaviour
{
    [SerializeField] private FloatingHealthbar healthbar;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int populationCost = 1;

    private float currentHealth;
    private PlacementSystem placementSystem;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        if (placementSystem == null)
            placementSystem = Object.FindFirstObjectByType<PlacementSystem>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        // Flash the troop/building
        if (FlashManager.Instance != null)
            FlashManager.Instance.FlashObject(gameObject);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (gameObject.CompareTag("TeamGreen") && placementSystem != null)
            placementSystem.AddToPopulation(-populationCost);

        Destroy(gameObject);
    }
}
