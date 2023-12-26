using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, PlacementOrientation orientation, int placedObjectIndex)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, orientation, placedObjectIndex);
        foreach(var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) { throw new Exception($"Dictionary already contains this cell position {pos}"); }
            placedObjects[pos] = data;
        }
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var position in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(position);
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();
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
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) { return false; }
        }
        return true;
    }

    public int GetObjectIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition)) { return -1; }
        return placedObjects[gridPosition].PlacedObjectIndex;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public PlacementOrientation objectOrientation { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int ID, PlacementOrientation objectOrientation, int PlacedObjectIndex) 
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.objectOrientation = objectOrientation;
        this.PlacedObjectIndex = PlacedObjectIndex;
    }
}
