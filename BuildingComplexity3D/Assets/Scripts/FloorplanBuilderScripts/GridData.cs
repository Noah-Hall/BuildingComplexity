using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>GridData</c> handles logic with the grid in <c>PlacementSystem</c>.
/// </summary>
public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();

    /// <summary>
    /// Method handles logic for adding objects to the grid in <c>PlacementSystem</c>.
    /// </summary>
    /// <param name="gridPosition">The <c>Vector3Int</c> position within the grid.</param>
    /// <param name="objectSize">The <c>Vector2Int</c> size of the object.</param>
    /// <param name="ID">The <c>int</c> id of the object.</param>
    /// <param name="orientation">The <c>PlacementOrientation</c> of the object.</param>
    /// <param name="placedObjectIndex">The <c>int</c> index of the object.</param>
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

    /// <summary>
    /// Method handles logic for removing objects from the grid in <c>PlacementSystem</c>.
    /// </summary>
    /// <param name="gridPosition">The <c>Vector3Int</c> position within the grid.</param>
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

    /// <summary>
    /// Method handles checking for valid placement within the grid for <c>PlacementSystem</c>.
    /// </summary>
    /// <returns><c>True</c> iff object can be placed on <paramref name="gridPosition"/>.</returns>
    /// <param name="gridPosition">The <c>Vector3Int</c> position within the grid.</param>
    /// <param name="objectSize">The <c>Vector2Int</c> size of the object.</param>
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Method gets object index.
    /// </summary>
    /// <returns><c>int</c> index of object at <paramref name="gridPosition"/>.</returns>
    /// <param name="gridPosition">The <c>Vector3Int</c> position within the grid.</param>
    public int GetObjectIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition)) { return -1; }
        return placedObjects[gridPosition].PlacedObjectIndex;
    }
}

/// <summary>
/// Class <c>PlacementData</c> holds properties to describe an object in <c>GridData</c>.
/// </summary>
public class PlacementData
{
    /// <value>
    /// Property <c>occupiedPositions</c> holds the <c>List</c> of <c>Vector3Int</c> grid positions <c>PlacementData</c> occupies.
    /// </value>
    public List<Vector3Int> occupiedPositions;

    /// <value>
    /// Property <c>ID</c> holds the <c>int</c> id of the <c>PlacementData</c>.
    /// </value>
    public int ID { get; private set; }

    /// <value>
    /// Property <c>objectOrientation</c> holds the <c>PlacementOrientation</c> of the <c>PlacementData</c>.
    /// </value>
    public PlacementOrientation objectOrientation { get; private set; }

    /// <value>
    /// Property <c>PlacedObjectIndex</c> holds the <c>int</c> index of the <c>PlacementData</c>.
    /// </value>
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int ID, PlacementOrientation objectOrientation, int PlacedObjectIndex) 
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.objectOrientation = objectOrientation;
        this.PlacedObjectIndex = PlacedObjectIndex;
    }
}
