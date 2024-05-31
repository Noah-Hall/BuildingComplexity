using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>PlacementState</c> implements IBuildingState for placing objects in PlacementSystem.
/// </summary>
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData, sideWallData, bottomWallData, centerData;
    private ObjectPlacer objectPlacer;
    private ObjectsDatabaseSO database;
    private bool isRotated = false;
    private Vector2Int objectSize;

    /// <value>
    /// Property <c>selectedOrientation</c> holds the current orientation selected by user.
    /// </value>
    public PlacementOrientation selectedOrientation { get; private set; } 

    /// <value>
    /// Property <c>ID</c> holds the current object ID selected by user.
    /// </value>
    public int ID { get; private set; }

    public PlacementState(int ID, Grid grid, PreviewSystem previewSystem, GridData floorData, GridData sideWallData, GridData bottomWallData, GridData centerData, ObjectPlacer objectPlacer, ObjectsDatabaseSO database)
    {
        this.ID = ID;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.sideWallData = sideWallData;
        this.bottomWallData = bottomWallData;
        this.centerData = centerData;
        this.objectPlacer = objectPlacer;
        this.database = database;
        this.selectedOrientation = PlacementOrientation.NONE;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        selectedOrientation = database.objectsData[selectedObjectIndex].orientation;
        if (selectedObjectIndex > -1) 
        {
            objectSize = database.objectsData[selectedObjectIndex].Size;
            previewSystem.StartShowingPreview(database.objectsData[selectedObjectIndex].Prefab, objectSize);
        } else {
            throw new Exception($"No ID found {ID}");
        }
    }

    /// <summary>
    /// Method for transitioning out of <c>PlacementState</c>.
    /// </summary>
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// Method for placing most objects in <c>PlacementSystem</c>.
    /// </summary>
    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity) { return; }

        PlacerObject newPlacerObject = new PlacerObject(database.objectsData[selectedObjectIndex].Prefab, 
                                                        GetNewPlacementPosition(gridPosition), 
                                                        isRotated, 
                                                        objectSize, 
                                                        gridPosition, 
                                                        database.objectsData[selectedObjectIndex].ID, 
                                                        selectedOrientation);
        int index = objectPlacer.PlaceObject(newPlacerObject);
        if (index == -1) { return; }

        GridData selectedData = GetGridData();
        Vector2Int placedObjectSize = isRotated ? new Vector2Int(objectSize.y, objectSize.x) : objectSize;
        selectedData.AddObjectAt(gridPosition, 
                                placedObjectSize,
                                database.objectsData[selectedObjectIndex].ID,
                                selectedOrientation, 
                                index);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), false, database.objectsData[selectedObjectIndex].color);
    }

    /// <summary>
    /// Method for placing stair objects in <c>PlacementSystem</c>.
    /// </summary>
    public void OnAction(Vector3Int gridPosition, StairInfo stairInfo)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity) { return; }

        PlacerObject newPlacerObject = new PlacerObject(database.objectsData[selectedObjectIndex].Prefab, 
                                                        GetNewPlacementPosition(gridPosition), 
                                                        isRotated, 
                                                        objectSize, 
                                                        gridPosition, 
                                                        database.objectsData[selectedObjectIndex].ID, 
                                                        selectedOrientation,
                                                        stairInfo);
        int index = objectPlacer.PlaceObject(newPlacerObject);
        if (index == -1) { return; }

        GridData selectedData = GetGridData();
        Vector2Int placedObjectSize = isRotated ? new Vector2Int(objectSize.y, objectSize.x) : objectSize;
        selectedData.AddObjectAt(gridPosition, 
                                placedObjectSize,
                                database.objectsData[selectedObjectIndex].ID,
                                selectedOrientation, 
                                index);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), false, database.objectsData[selectedObjectIndex].color);
    }

    /// <summary>
    /// Method for placing room node objects in <c>PlacementSystem</c>.
    /// </summary>
    public void OnAction(Vector3Int gridPosition, int weight) 
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity) { return; }

        PlacerObject newPlacerObject = new PlacerObject(database.objectsData[selectedObjectIndex].Prefab, 
                                                        GetNewPlacementPosition(gridPosition), 
                                                        isRotated, 
                                                        objectSize, 
                                                        gridPosition, 
                                                        database.objectsData[selectedObjectIndex].ID, 
                                                        selectedOrientation,
                                                        weight);
        int index = objectPlacer.PlaceObject(newPlacerObject);
        if (index == -1) { return; }

        GridData selectedData = GetGridData();
        Vector2Int placedObjectSize = isRotated ? new Vector2Int(objectSize.y, objectSize.x) : objectSize;
        selectedData.AddObjectAt(gridPosition, 
                                placedObjectSize,
                                database.objectsData[selectedObjectIndex].ID,
                                selectedOrientation, 
                                index);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), false, database.objectsData[selectedObjectIndex].color);
    }

    /// <summary>
    /// Method for rotating objects in <c>PlacementSystem</c>.
    /// </summary>
    public void OnRotate(Vector3Int gridPosition)
    {
        if (selectedOrientation == PlacementOrientation.NONE) { return; }
        if (selectedOrientation == PlacementOrientation.SIDE || selectedOrientation == PlacementOrientation.BOTTOM) 
        {
            selectedOrientation = (selectedOrientation == PlacementOrientation.SIDE) ? PlacementOrientation.BOTTOM : PlacementOrientation.SIDE;
            isRotated = !isRotated;

            Vector2Int cursorSize = isRotated ? new Vector2Int(objectSize.y, objectSize.x) : objectSize;
            previewSystem.RotatePreview(cursorSize);

            bool placementValidity = CheckPlacementValidity(gridPosition);
            previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
        }
    }

    /// <summary>
    /// Method for scaling floor objects in <c>PlacementSystem</c>.
    /// </summary>
    public void OnScaleXY(int x, int y, Vector3Int gridPosition)
    {
        objectSize.x = x < 1 ? objectSize.x : x;
        objectSize.y = y < 1 ? objectSize.y : y;

        previewSystem.ScalePreview(objectSize, objectSize);
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
    }

    /// <summary>
    /// Method for scaling most objects in <c>PlacementSystem</c>.
    /// </summary>
    public void OnScale(int y, Vector3Int gridPosition)
    {
        objectSize.y = y < 1 ? objectSize.y : y;
        Vector2Int cursorSize = isRotated ? new Vector2Int(objectSize.y, objectSize.x) : objectSize;

        previewSystem.ScalePreview(cursorSize, objectSize);
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        GridData selectedData = GetGridData();
        Vector2Int cursorSize = isRotated ? new Vector2Int(objectSize.y, objectSize.x) : objectSize;
        return selectedData.CanPlaceObjectAt(gridPosition, cursorSize); 
    }

    private GridData GetGridData()
    {
        switch(selectedOrientation)
        {
            case PlacementOrientation.NONE:
            case PlacementOrientation.CENTER:
                if (database.objectsData[selectedObjectIndex].ID < 2)
                {
                    return floorData;
                }
                return centerData;
            case PlacementOrientation.SIDE:
                return sideWallData;
            case PlacementOrientation.BOTTOM:
                return bottomWallData;
        }
        return null;
    }

    private Vector3 GetNewPlacementPosition(Vector3Int gridPosition)
    {
        Vector3 returnVal = new Vector3();
        float addedHeight = database.objectsData[selectedObjectIndex].ID < 2 ? 0f : 0.55f;
        switch (selectedOrientation)
        {
            case PlacementOrientation.NONE:
            case PlacementOrientation.CENTER:
                returnVal = new Vector3(grid.CellToWorld(gridPosition).x + (objectSize.x / 2f), grid.CellToWorld(gridPosition).y + addedHeight, grid.CellToWorld(gridPosition).z + (objectSize.y / 2f));
                break;
            case PlacementOrientation.SIDE:
                returnVal = new Vector3(grid.CellToWorld(gridPosition).x, grid.CellToWorld(gridPosition).y + addedHeight, grid.CellToWorld(gridPosition).z + (objectSize.y / 2f));
                break;
            case PlacementOrientation.BOTTOM:
                returnVal = new Vector3(grid.CellToWorld(gridPosition).x + (objectSize.y / 2f), grid.CellToWorld(gridPosition).y + addedHeight, grid.CellToWorld(gridPosition).z);
                break;
        }
        return returnVal;
    }
    
    /// <summary>
    /// Method for updating <c>PlacementState</c>. Handles preview and placement validity.
    /// </summary>
    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
    }
}
