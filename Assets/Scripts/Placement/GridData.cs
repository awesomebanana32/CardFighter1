// GridData.cs
using UnityEngine;
using System.Collections.Generic;
using System;

public class GridData 
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        if (!CanPlaceObjectAt(gridPosition, objectSize))
            throw new Exception("Cannot place object at the specified position: some cells are already occupied.");
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    public bool GetRepresentationIndex(Vector3Int gridPosition, out int index)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            index = -1;
            return false;
        }
        index = placedObjects[gridPosition].placedObjectIndex;
        return true;
    }

    public bool TryGetPlacementData(Vector3Int gridPosition, out PlacementData data)
    {
        return placedObjects.TryGetValue(gridPosition, out data);
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
            return;

        foreach (var pos in placedObjects[gridPosition].OccupiedPosition)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> OccupiedPosition;
    public int ID { get; private set; }
    public int placedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPosition, int ID, int placedObjectIndex)
    {
        this.OccupiedPosition = occupiedPosition;
        this.ID = ID;
        this.placedObjectIndex = placedObjectIndex;
    }
}