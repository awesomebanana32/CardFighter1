using UnityEngine;

public class TroopHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private FloatingHealthbar healthbar;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int populationCost = 1;

    [Header("Level UI")]
    public LevelNumberUI levelUI;  // public so LevelSystem can access it

    private float currentHealth;
    private PlacementSystem placementSystem;
    private bool isDead = false;

    // Public getters
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float BaseMaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        if (placementSystem == null)
            placementSystem = PlacementSystem.Instance; // Singleton reference
    }

    public void TakeDamage(float damage, GameObject attacker = null)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);

        if (FlashManager.Instance != null)
            FlashManager.Instance.FlashObject(gameObject);

        if (currentHealth <= 0)
            Die(attacker);
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);
    }

    /// <summary>
    /// Safely set max health while preserving current health percentage
    /// </summary>
    public void SetMaxHealth(float newMax, float currentPercent)
    {
        maxHealth = newMax;
        currentHealth = maxHealth * currentPercent;
        healthbar?.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void Die(GameObject killer = null)
    {
        if (isDead) return;
        isDead = true;

        // Award level up to killer
        if (killer != null)
        {
            LevelSystem ls = killer.GetComponent<LevelSystem>();
            if (ls != null)
                ls.OnKill();
        }

        if (gameObject.CompareTag("TeamGreen") && placementSystem != null)
            placementSystem.AddToPopulation(-populationCost);

        Destroy(gameObject);
    }

    public float GetBaseDamage()
    {
        AttackState attack = GetComponent<AttackState>();
        return attack != null ? attack.damage : 1f;
    }
}
