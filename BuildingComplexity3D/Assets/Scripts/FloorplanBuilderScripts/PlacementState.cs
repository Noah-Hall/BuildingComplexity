using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    public PlacementOrientation selectedOrientation { get; private set; } 
    public int ID { get; private set; }
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData, sideWallData, bottomWallData, centerData;
    private ObjectPlacer objectPlacer;
    private ObjectsDatabaseSO database;
    private bool isRotated = false;
    private Vector2Int objectSize;

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

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

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

    public void OnScaleXY(int x, int y, Vector3Int gridPosition)
    {
        objectSize.x = x < 1 ? objectSize.x : x;
        objectSize.y = y < 1 ? objectSize.y : y;

        previewSystem.ScalePreview(objectSize, objectSize);
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
    }

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
    
    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
    }
}
