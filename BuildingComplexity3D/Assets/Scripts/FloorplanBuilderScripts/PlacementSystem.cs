using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] 
    private GameObject mouseIndicator;
    [SerializeField] 
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDatabaseSO database;
    // private int selectedObjectIndex = -1;
    // private PlacementOrientation slectedOrientation = PlacementOrientation.NONE;

    [SerializeField]
    private GameObject gridVisualization;
    private GridData floorData, sideWallData, bottomWallData, centerData;
    [SerializeField]
    private PreviewSystem preview;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField]
    private ObjectPlacer objectPlacer;
    private IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        floorData = new GridData();
        sideWallData = new GridData();
        bottomWallData = new GridData();
        centerData = new GridData();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, preview, floorData, sideWallData, bottomWallData, centerData, objectPlacer, database);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new RemovingState(grid, preview, floorData, sideWallData, bottomWallData, centerData, objectPlacer);
        
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        if (buildingState == null) { return; }
        gridVisualization.SetActive(false);

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
}
