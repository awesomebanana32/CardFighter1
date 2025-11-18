using UnityEngine;

public class City : MonoBehaviour
{
    public GameObject rangeCircle;

    void Start()
    {
        if (rangeCircle != null)
            rangeCircle.SetActive(false);
    }

    public void ShowRange(bool show)
    {
        if (rangeCircle != null)
            rangeCircle.SetActive(show);
    }
}
