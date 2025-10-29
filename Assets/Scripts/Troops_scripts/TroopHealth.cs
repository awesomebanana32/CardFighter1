using UnityEngine;

public class TroopHealth : MonoBehaviour
{
    [SerializeField] private FloatingHealthbar healthbar;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int populationCost = 1;

    private float currentHealth;
    private PlacementSystem placementSystem;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        if (placementSystem == null)
            placementSystem = PlacementSystem.Instance; // Singleton reference
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        if (FlashManager.Instance != null)
            FlashManager.Instance.FlashObject(gameObject);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (gameObject.CompareTag("TeamGreen") && placementSystem != null)
            placementSystem.AddToPopulation(-populationCost);

        Destroy(gameObject);
    }
}
