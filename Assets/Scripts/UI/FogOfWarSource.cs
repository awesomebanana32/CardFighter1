using UnityEngine;
using System.Collections;

public class FogOfWarSource : MonoBehaviour
{
    [Header("Vision")]
    public float stealthRadius = 5f;

    private bool registeredVisible = false;
    private bool registeredDiscovered = false;

    void OnEnable()
    {
        StartCoroutine(TryRegister());
    }

    IEnumerator TryRegister()
    {
        while (!registeredVisible || !registeredDiscovered)
        {
            // Register with FogOfWarVisible
            if (!registeredVisible && FogOfWarVisible.Instance != null)
            {
                FogOfWarVisible.Instance.RegisterVisionSource(this);
                registeredVisible = true;
            }

            // Register with FogOfWarDiscovered
            if (!registeredDiscovered && FogOfWarDiscovered.Instance != null)
            {
                FogOfWarDiscovered.Instance.RegisterVisionSource(this);
                registeredDiscovered = true;
            }

            yield return null;
        }
    }

    void OnDisable()
    {
        if (registeredVisible && FogOfWarVisible.Instance != null)
        {
            FogOfWarVisible.Instance.UnRegisterVisionSource(this);
            registeredVisible = false;
        }

        if (registeredDiscovered && FogOfWarDiscovered.Instance != null)
        {
            FogOfWarDiscovered.Instance.UnRegisterVisionSource(this);
            registeredDiscovered = false;
        }
    }
}
