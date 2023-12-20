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
    private int selectedObjectIndex = -1;
    private PlacementOrientation slectedOrientation = PlacementOrientation.NONE;

    [SerializeField]
    private GameObject gridVisualization;
    private GridData floorData, sideWallData, bottomWallData, centerData;
    private List<GameObject> placedObjects;
    [SerializeField]
    private PreviewSystem preview;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private void Start()
    {
        StopPlacement();
        floorData = new GridData();
        sideWallData = new GridData();
        bottomWallData = new GridData();
        centerData = new GridData();
        placedObjects = new List<GameObject>();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        slectedOrientation = database.objectsData[selectedObjectIndex].orientation;
        if (selectedObjectIndex < 0) 
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity) { return; }

        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = GetNewPlacementPosition(gridPosition);
        placedObjects.Add(newObject);
        GridData selectedData = GetGridData();
        selectedData.AddObjectAt(gridPosition, 
                                database.objectsData[selectedObjectIndex].Size, 
                                database.objectsData[selectedObjectIndex].ID,
                                slectedOrientation, 
                                placedObjects.Count - 1);
        preview.UpdatePreview(newObject.transform.position, grid.CellToWorld(gridPosition), false, database.objectsData[selectedObjectIndex].color);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        slectedOrientation = PlacementOrientation.NONE;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private Vector3 GetNewPlacementPosition(Vector3Int gridPosition)
    {
        Vector3 returnVal = new Vector3();
        switch (slectedOrientation)
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

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        GridData selectedData = GetGridData();
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size); 
    }

    private GridData GetGridData()
    {
        switch(slectedOrientation)
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

    private void Update()
    {
        if (selectedObjectIndex < 0) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;

        if (lastDetectedPosition == gridPosition) { return; }
        bool placementValidity = CheckPlacementValidity(gridPosition);
        preview.UpdatePreview(GetNewPlacementPosition(gridPosition), grid.CellToWorld(gridPosition), placementValidity, database.objectsData[selectedObjectIndex].color);
        lastDetectedPosition = gridPosition;
    }
}
