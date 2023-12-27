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
            previewSystem.StartShowingPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
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

        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, GetNewPlacementPosition(gridPosition), isRotated);

        GridData selectedData = GetGridData();
        selectedData.AddObjectAt(gridPosition, 
                                database.objectsData[selectedObjectIndex].Size, 
                                database.objectsData[selectedObjectIndex].ID,
                                selectedOrientation, 
                                index);
        previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), false, database.objectsData[selectedObjectIndex].color);
    }

    public void OnRotate(Vector3Int gridPosition)
    {
        if (selectedOrientation == PlacementOrientation.NONE) { return; }
        if (selectedOrientation == PlacementOrientation.SIDE) 
        { 
            selectedOrientation = PlacementOrientation.BOTTOM;
            isRotated = !isRotated;
            previewSystem.RotatePreview();
            bool placementValidity = CheckPlacementValidity(gridPosition);
            previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
        } else if (selectedOrientation == PlacementOrientation.BOTTOM) 
        { 
            selectedOrientation = PlacementOrientation.SIDE;
            isRotated = !isRotated;
            previewSystem.RotatePreview();
            bool placementValidity = CheckPlacementValidity(gridPosition);
            previewSystem.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        GridData selectedData = GetGridData();
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size); 
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
                break;
            case PlacementOrientation.SIDE:
                return sideWallData;
                break;
            case PlacementOrientation.BOTTOM:
                return bottomWallData;
                break;
        }
        return null;
    }

    private Vector3 GetNewPlacementPosition(Vector3Int gridPosition)
    {
        Vector3 returnVal = new Vector3();
        switch (selectedOrientation)
        {
            case PlacementOrientation.NONE:
            case PlacementOrientation.CENTER:
                returnVal = new Vector3(grid.CellToWorld(gridPosition).x + 0.5f, grid.CellToWorld(gridPosition).y, grid.CellToWorld(gridPosition).z + 0.5f);
                break;
            case PlacementOrientation.SIDE:
                returnVal = new Vector3(grid.CellToWorld(gridPosition).x, grid.CellToWorld(gridPosition).y, grid.CellToWorld(gridPosition).z + 0.5f);
                break;
            case PlacementOrientation.BOTTOM:
                returnVal = new Vector3(grid.CellToWorld(gridPosition).x + 0.5f, grid.CellToWorld(gridPosition).y, grid.CellToWorld(gridPosition).z);
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
