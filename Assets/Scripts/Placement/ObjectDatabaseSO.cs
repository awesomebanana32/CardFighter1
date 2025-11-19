using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDatabase", menuName = "Database/ObjectDatabaseSO")]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;

    // Get the prefab for a given ID
    public GameObject GetPrefabByID(int id)
    {
        ObjectData data = objectsData.Find(obj => obj.ID == id);
        return data != null ? data.Prefab : null;
    }

    // Get the population cost for a given ID
    public int GetPopulationCostByID(int id)
    {
        ObjectData data = objectsData.Find(obj => obj.ID == id);
        return data != null ? data.PopulationCost : 1;
    }
}

[Serializable]
public class ObjectData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int PopulationCost { get; private set; } = 1;
}
