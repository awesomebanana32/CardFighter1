using UnityEngine;

public class TroopHealth : MonoBehaviour
{
    [SerializeField] private FloatingHealthbar healthbar; // Reference to your healthbar
    [SerializeField] private float maxHealth = 100f; // Maximum health
    [SerializeField] private float damageAmount = 10f; // How much damage each key press does
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.UpdateHealthBar(currentHealth, maxHealth);

    }

    void Update()
    {
        // Check if the 1 key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        // Optional: Add death logic here
        
        Destroy(gameObject);
    }
}