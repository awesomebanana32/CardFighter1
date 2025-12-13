using UnityEngine;
using System.Collections;

public class FogOfWarSource : MonoBehaviour
{
    [Header("Vision")]
    public float stealthRadius = 5f;

    private bool registered = false;

    void OnEnable()
    {
        StartCoroutine(TryRegister());
    }

    IEnumerator TryRegister()
    {
        while (!registered)
        {
            if (FogOfWar.Instance != null)
            {
                FogOfWar.Instance.RegisterVisionSource(this);
                registered = true;
                yield break;
            }
            yield return null;
        }
    }

    void OnDisable()
    {
        if (registered && FogOfWar.Instance != null)
        {
            FogOfWar.Instance.UnRegisterVisionSource(this);
            registered = false;
        }
    }
}
