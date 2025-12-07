using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [Header("Level Stats")]
    public int level = 1;                // Current level

    [Header("Scaling")]
    public float hpPerLevel = 0.10f;     // 10% per level
    public float damagePerLevel = 0.10f; // 10% per level

    [Header("Runtime")]
    public float currentDamage;

    private TroopHealth health;

    private void Awake()
    {
        health = GetComponent<TroopHealth>();
        currentDamage = health.GetBaseDamage();
    }

    /// <summary>
    /// Call this when the unit kills an enemy
    /// </summary>
    public void OnKill()
    {
        level++;

        // Increase Max HP
        float newMax = health.BaseMaxHealth * (1 + hpPerLevel * (level - 1));
        float oldPercent = health.CurrentHealth / health.MaxHealth;
        health.SetMaxHealth(newMax, oldPercent);

        // Increase damage
        currentDamage = health.GetBaseDamage() * (1 + damagePerLevel * (level - 1));

        // Update Level UI
        if (health.levelUI != null)
            health.levelUI.UpdateLevelNumber(level);

        // Optional: debug
        // Debug.Log($"{gameObject.name} leveled up to {level}");
    }
}
