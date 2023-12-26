using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    private PlacementOrientation selectedOrientation = PlacementOrientation.NONE;
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData, sideWallData, bottomWallData, centerData;
    private ObjectPlacer objectPlacer;

    public RemovingState(Grid grid, PreviewSystem previewSystem, GridData floorData, GridData sideWallData, GridData bottomWallData, GridData centerData, ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.sideWallData = sideWallData;
        this.bottomWallData = bottomWallData;
        this.centerData = centerData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition) 
    {
        GridData selectedData = GetData(gridPosition);
        if (selectedData != null) 
        {
            gameObjectIndex = selectedData.GetObjectIndex(gridPosition);
            if (gameObjectIndex == -1) { return; }
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), IsCellEmpty(gridPosition));
    }

    private GridData GetData(Vector3Int gridPosition)
    {
        if (!sideWallData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            return sideWallData;
        } else if (!bottomWallData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            return bottomWallData;
        } else if (!centerData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            return centerData;
        } else if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            return floorData;
        }
        return null;
    }

    private bool IsCellEmpty(Vector3Int gridPosition)
    {
        if (GetData(gridPosition) == null) { return false; }
        return true;
    }

    public void UpdateState(Vector3Int gridPosition) 
    {
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), IsCellEmpty(gridPosition));
    }
}
