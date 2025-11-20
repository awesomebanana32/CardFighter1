using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private ObjectDatabaseSO database;
    [SerializeField] private int maxPopulation = 100;

    [Header("UI")]
    [SerializeField] private GameObject selectCityPopup;
    [SerializeField] private float popupDuration = 2f;

    public static PlacementSystem Instance { get; private set; }

    private int currentPopulation = 0;
    public int CurrentPopulation => currentPopulation;
    public int MaxPopulation => maxPopulation;

    // Queue entry: each entry is for a specific troop type *and* a specific city
    private class QueueEntry
    {
        public int troopID;
        public City spawnCity;
        public int count;
        public float lastSpawnTime;    // time when cooldown for this entry started / reset
        public Image cooldownImage;    // optional UI image to show cooldown
    }

    private readonly List<QueueEntry> spawnQueue = new List<QueueEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        // Process queue entries (iterate backwards so we can remove)
        for (int i = spawnQueue.Count - 1; i >= 0; i--)
        {
            QueueEntry entry = spawnQueue[i];

            float cooldown = database.GetCooldownByID(entry.troopID);
            // Safety: avoid divide by zero
            if (cooldown <= 0f) cooldown = 0.0001f;

            float elapsed = Time.time - entry.lastSpawnTime;
            float normalized = Mathf.Clamp01(elapsed / cooldown);

            // Fill amount: we use countdown (full -> empty). When lastSpawnTime set at Time.time, fillAmount = 1 - 0 = 1
            if (entry.cooldownImage != null)
                entry.cooldownImage.fillAmount = 1f - normalized;

            if (elapsed < cooldown)
                continue; // still cooling down

            // Enough time = spawn one unit from this entry
            TrySpawnFromEntry(entry);

            // Reset lastSpawnTime for the next spawn in this entry (if any)
            entry.lastSpawnTime = Time.time;

            // reduce queue count and update UI (now update total across cities)
            entry.count--;
            RefreshButtonQueueDisplay(entry.troopID);

            if (entry.count <= 0)
            {
                // clear overlay and remove entry
                if (entry.cooldownImage != null)
                    entry.cooldownImage.fillAmount = 0f;
                spawnQueue.RemoveAt(i);
            }
            else
            {
                // keep entry in list, next spawn will wait cooldown from this moment
                if (entry.cooldownImage != null)
                    entry.cooldownImage.fillAmount = 1f; // reset overlay to full at start of next cooldown
            }
        }
    }

    /// <summary>
    /// Called by TroopButton when clicked. Deducts gold/pop immediately and adds to queue for that (troopID, city).
    /// </summary>
    public void QueueSpawn(int troopID, Image cooldownImage)
    {
        City selectedCity = CitySelectionManager.SelectedCity;
        if (selectedCity == null)
        {
            ShowSelectCityPopup();
            return;
        }

        GameObject prefab = database.GetPrefabByID(troopID);
        if (prefab == null)
        {
            Debug.LogWarning($"PlacementSystem.QueueSpawn: prefab missing for ID {troopID}");
            return;
        }

        int popCost = database.GetPopulationCostByID(troopID);
        int goldCost = database.GetGoldCostByID(troopID);
        float cooldown = database.GetCooldownByID(troopID);
        if (cooldown <= 0f) cooldown = 0.0001f;

        // GOLD: attempt to spend immediately
        if (!GoldManager.Instance.SpendGold(goldCost))
        {
            // not enough gold
            return;
        }

        // POP: verify there's space; if not, refund and exit
        if (currentPopulation + popCost > maxPopulation)
        {
            GoldManager.Instance.AddGold(goldCost); // refund
            // optionally show UI here
            return;
        }

        // Reserve population immediately (prevents over-queueing beyond pop cap)
        AddToPopulation(popCost);

        // Find existing queue entry for same troopID and same city
        QueueEntry existing = spawnQueue.Find(e => e.troopID == troopID && e.spawnCity == selectedCity);

        if (existing != null)
        {
            // increment count; cooldown already running for this entry
            existing.count++;
            RefreshButtonQueueDisplay(troopID);
            return;
        }

        // Create a new queue entry for this city & troop
        QueueEntry entry = new QueueEntry()
        {
            troopID = troopID,
            spawnCity = selectedCity,
            count = 1,
            cooldownImage = cooldownImage,
            lastSpawnTime = Time.time // start cooldown immediately
        };

        spawnQueue.Add(entry);

        // initialize overlay to full (countdown starts)
        if (entry.cooldownImage != null)
            entry.cooldownImage.fillAmount = 1f;

        RefreshButtonQueueDisplay(troopID);
    }

    // Attempt to spawn one unit from the provided entry (assumes population was reserved earlier)
    private void TrySpawnFromEntry(QueueEntry entry)
    {
        if (entry.spawnCity == null)
        {
            // City was destroyed/unassigned — refund population? (we already reserved pop)
            // Safer to not spawn and refund population + gold isn't tracked here because gold already spent.
            // We'll release reserved population back:
            AddToPopulation(-database.GetPopulationCostByID(entry.troopID));
            return;
        }

        GameObject prefab = database.GetPrefabByID(entry.troopID);
        if (prefab == null)
        {
            Debug.LogWarning($"PlacementSystem.TrySpawnFromEntry: prefab missing for ID {entry.troopID}");
            // restore pop reservation for this spawn
            AddToPopulation(-database.GetPopulationCostByID(entry.troopID));
            return;
        }

        // Final population check before instantiate (in case something changed)
        int popCost = database.GetPopulationCostByID(entry.troopID);
        if (currentPopulation - popCost < 0)
        {
            // This shouldn't normally happen because we reserved on queue, but guard anyway.
            return;
        }

        // Instantiate at the stored city's spawn point
        Transform spawnPoint = entry.spawnCity.GetSpawnPoint();
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Population already reserved earlier — no need to AddToPopulation here.
        // We kept reserved population count as actual currentPopulation, so nothing to change now.
        // (Note: when reserving we added pop; when failed we remove it).
    }

    // reserve / unreserve population (AddToPopulation used as reservation and consumption)
    public void AddToPopulation(int amount)
    {
        currentPopulation += amount;
        currentPopulation = Mathf.Clamp(currentPopulation, 0, maxPopulation);
    }

    // UI popup if no city is selected
    private void ShowSelectCityPopup()
    {
        if (selectCityPopup == null) return;
        selectCityPopup.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HidePopupAfterDelay(popupDuration));
    }

    private IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (selectCityPopup != null) selectCityPopup.SetActive(false);
    }

    // Find all TroopButton components and update the count for those matching troopID
    // THIS NOW DISPLAYS THE TOTAL QUEUE COUNT ACROSS ALL CITIES for that troopID
    private void RefreshButtonQueueDisplay(int troopID)
    {
        int total = 0;
        for (int i = 0; i < spawnQueue.Count; i++)
        {
            if (spawnQueue[i].troopID == troopID)
                total += spawnQueue[i].count;
        }

        TroopButton[] buttons = FindObjectsOfType<TroopButton>();
        foreach (var b in buttons)
        {
            if (b.troopID == troopID)
                b.UpdateQueueCount(total);
        }
    }
}
