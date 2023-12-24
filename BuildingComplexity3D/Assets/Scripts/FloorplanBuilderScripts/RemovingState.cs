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
    private ObjectsDatabaseSO database;

    public RemovingState(Grid grid, PreviewSystem previewSystem, GridData floorData, GridData sideWallData, GridData bottomWallData, GridData centerData, ObjectPlacer objectPlacer, ObjectsDatabaseSO database)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.sideWallData = sideWallData;
        this.bottomWallData = bottomWallData;
        this.centerData = centerData;
        this.objectPlacer = objectPlacer;
        this.database = database;
    }

    public void EndState() {}
    public void OnAction(Vector3Int gridPosition) {}
    public void UpdateState(Vector3Int gridPosition) {}
}
