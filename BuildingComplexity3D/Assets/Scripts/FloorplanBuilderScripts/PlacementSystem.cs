using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] 
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField] 
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;
    private PlacementOrientation slectedOrientation;

    [SerializeField]
    private GameObject gridVisualization;

    private void Start()
    {
        StopPlacement();
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
        cellIndicator.SetActive(true);
        SetOnClicked(true);
        inputManager.OnExit += StopPlacement;
    }

    private void SetOnClicked(bool set)
    {
        switch(slectedOrientation)
        {
            case PlacementOrientation.CENTER:
                if (set) { inputManager.OnClicked += PlaceStructureCenter; }
                else { inputManager.OnClicked -= PlaceStructureCenter; }
                break;
            case PlacementOrientation.SIDE:
                if (set) { inputManager.OnClicked += PlaceStructureSide; }
                else { inputManager.OnClicked -= PlaceStructureSide; }
                break;
            case PlacementOrientation.BOTTOM:
                if (set) { inputManager.OnClicked += PlaceStructureBottom; }
                else { inputManager.OnClicked -= PlaceStructureBottom; }
                break;
        }
    }

    private void PlaceStructureCenter()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = new Vector3(grid.CellToWorld(gridPosition).x + 0.5f, grid.CellToWorld(gridPosition).y, grid.CellToWorld(gridPosition).z + 0.5f);
    }

    private void PlaceStructureSide()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = new Vector3(grid.CellToWorld(gridPosition).x, grid.CellToWorld(gridPosition).y, grid.CellToWorld(gridPosition).z + 0.5f);
    }

    private void PlaceStructureBottom()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = new Vector3(grid.CellToWorld(gridPosition).x + 0.5f, grid.CellToWorld(gridPosition).y, grid.CellToWorld(gridPosition).z);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        SetOnClicked(false);
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
