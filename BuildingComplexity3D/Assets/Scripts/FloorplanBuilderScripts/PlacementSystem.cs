using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] 
    private GameObject mouseIndicator, chooseNodeUI, scaleUI, scaleXYUI, scaleSize, scaleSizeX, scaleSizeY, stairUI, stairFloorNum, stairStairwellNum, stairExitFloor;
    private StairInfo stairInfo;
    [SerializeField] 
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDatabaseSO database;
    

    [SerializeField]
    private GameObject gridVisualization;
    private GridData floorData, sideWallData, bottomWallData, centerData;
    [SerializeField]
    private PreviewSystem preview;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField]
    private ObjectPlacer objectPlacer;
    private IBuildingState buildingState;
    [SerializeField] 
    private Camera sceneCamera;

    private void Start()
    {
        StopPlacement();
        floorData = new GridData();
        sideWallData = new GridData();
        bottomWallData = new GridData();
        centerData = new GridData();
        SetScaleUIs();
        stairInfo.floorNum = 1;
        stairInfo.stairwellNum = 1;
        stairInfo.isExitFloor = true;
    }

    public void StartNode()
    {
        StartPlacement(chooseNodeUI.GetComponent<Dropdown>().value + 6);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, preview, floorData, sideWallData, bottomWallData, centerData, objectPlacer, database);
        SetScaleUIs();

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new RemovingState(grid, preview, floorData, sideWallData, bottomWallData, centerData, objectPlacer);
        SetScaleUIs();
        
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        PlacementState placementState = ((PlacementState)buildingState);
        if (placementState.ID == 5) {
            placementState.OnAction(gridPosition, stairInfo);
        } else {
            buildingState.OnAction(gridPosition);
        }
    }

    public void RotateStructure()
    {
        if (!(buildingState is PlacementState)) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        ((PlacementState)buildingState).OnRotate(gridPosition);
    }

    public void ScaleStructure()
    {
        if (!(buildingState is PlacementState)) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (scaleUI.activeSelf) {
            int scaleTo = int.Parse(scaleSize.GetComponent<Text>().text);
            ((PlacementState)buildingState).OnScale(scaleTo, gridPosition);
        } else if (scaleXYUI.activeSelf) {
            int scaleToX = int.Parse(scaleSizeX.GetComponent<Text>().text);
            int scaleToY = int.Parse(scaleSizeY.GetComponent<Text>().text);
            ((PlacementState)buildingState).OnScaleXY(scaleToX, scaleToY, gridPosition);
        }
    }

    public void SetStairInfo() {
        if (!(buildingState is PlacementState)) { return; }
        stairInfo.floorNum = int.Parse(stairFloorNum.GetComponent<Text>().text);
        stairInfo.stairwellNum = int.Parse(stairStairwellNum.GetComponent<Text>().text);
        stairInfo.isExitFloor = stairExitFloor.GetComponent<Toggle>().isOn;
    }

    private void StopPlacement()
    {
        if (buildingState == null) { return; }
        gridVisualization.SetActive(false);
        scaleUI.SetActive(false);
        scaleXYUI.SetActive(false);

        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    private void Update()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;
        
        if (buildingState == null) { return; }
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition == gridPosition) { return; }
        buildingState.UpdateState(gridPosition);
        lastDetectedPosition = gridPosition;
    }

    private void SetScaleUIs()
    {
        if (buildingState == null || buildingState is RemovingState)
        {
            scaleUI.SetActive(false);
            scaleXYUI.SetActive(false);
            chooseNodeUI.SetActive(false);
            stairUI.SetActive(false);
            return; 
        }
        PlacementState placementState = ((PlacementState)buildingState);
        scaleUI.SetActive(false);
        scaleXYUI.SetActive(false);
        chooseNodeUI.SetActive(false);
        stairUI.SetActive(false);

        switch (placementState.selectedOrientation)
        {
            case PlacementOrientation.CENTER:
                if (placementState.ID == 1) { scaleXYUI.SetActive(true); }
                if (placementState.ID == 5) { stairUI.SetActive(true); }
                if (placementState.ID > 5) { chooseNodeUI.SetActive(true); }
                break;
            case PlacementOrientation.SIDE:
            case PlacementOrientation.BOTTOM:
                scaleUI.SetActive(true);
                break;
            case PlacementOrientation.NONE:
                break;
        }
    }

    public void LoadFromSceneData(SceneData data)
    {
        foreach(PlacerObject placerObject in data.objectsList)
        {
            if (placerObject == null) { continue; }
            int index = objectPlacer.PlaceObject(placerObject);

            GridData selectedData = GetGridData(placerObject.orientation, placerObject.ID);
            Vector2Int placedObjectSize = placerObject.rotate ? new Vector2Int(placerObject.scale.y, placerObject.scale.x) : placerObject.scale;
            selectedData.AddObjectAt(placerObject.gridPosition, 
                                    placedObjectSize,
                                    placerObject.ID,
                                    placerObject.orientation, 
                                    index);
        }
    }

    private GridData GetGridData(PlacementOrientation orientation, int ID)
    {
        switch(orientation)
        {
            case PlacementOrientation.NONE:
            case PlacementOrientation.CENTER:
                if (ID < 2)
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
}
