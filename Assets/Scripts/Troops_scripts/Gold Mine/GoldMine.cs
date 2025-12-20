using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [Header("Gold Mine Settings")]
    [SerializeField] private int goldPerTick = 25;
    [SerializeField] private float tickInterval = 3f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            timer -= tickInterval;
            GenerateGold();
        }
    }

    void GenerateGold()
    {
        if (GoldManager.Instance == null)
            return;

        GoldManager.Instance.AddGold(goldPerTick);
    }
}
