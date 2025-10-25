using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    public int damage = 5;
    public string targetTag = "TeamRed";
    public float duration = 5f; // how long the fire stays before disappearing
    public float tickRate = 0.3f; // how often damage is applied

    // Keep track of coroutines for each enemy
    private Dictionary<TroopHealth, Coroutine> activeCoroutines = new Dictionary<TroopHealth, Coroutine>();

    private void Start()
    {
        // Destroy the fire after a few seconds
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            TroopHealth health = other.GetComponent<TroopHealth>();
            if (health != null && !activeCoroutines.ContainsKey(health))
            {
                // Start damage-over-time coroutine
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
                // Stop the coroutine when they leave the fire
                StopCoroutine(activeCoroutines[health]);
                activeCoroutines.Remove(health);
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(TroopHealth health)
    {
        while (health != null)
        {
            health.TakeDamage(damage);
            yield return new WaitForSeconds(tickRate);
        }
    }
}
