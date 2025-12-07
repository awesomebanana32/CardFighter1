using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    [Header("Damage Settings")]
    public float baseDamage = 5f;
    public string targetTag = "TeamRed";
    public float duration = 5f;      // How long the fire lasts
    public float tickRate = 0.3f;    // Damage interval

    [Header("Spell Caster")]
    public GameObject caster;         // The unit that cast the fire

    // Track coroutines per enemy
    private Dictionary<TroopHealth, Coroutine> activeCoroutines = new Dictionary<TroopHealth, Coroutine>();

    private void Start()
    {
        Destroy(gameObject, duration); // Auto cleanup
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null && !activeCoroutines.ContainsKey(health))
            {
                Coroutine c = StartCoroutine(ApplyDamageOverTime(health));
                activeCoroutines.Add(health, c);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null && activeCoroutines.ContainsKey(health))
            {
                StopCoroutine(activeCoroutines[health]);
                activeCoroutines.Remove(health);
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(TroopHealth health)
    {
        while (health != null)
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

            yield return new WaitForSeconds(tickRate);
        }
    }
}
