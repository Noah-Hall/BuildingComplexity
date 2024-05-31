using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class <c>InputManager</c> is responsible for handling user input within <c>PlacementSystem</c>.
/// </summary>
public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;
    private Vector3 lastPosition;
    public event Action OnClicked, OnExit;
    private float minZoom = 60f, maxZoom = 1f, sensitivity = 5f;
    private Vector3 cameraOrigin, difference;
    private bool drag = false;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { OnClicked?.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { OnExit?.Invoke(); }
        UpdateZoom();
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(2)) 
        {
            difference = sceneCamera.ScreenToWorldPoint(Input.mousePosition) - sceneCamera.transform.position;
            if (drag == false) 
            {
                drag = true;
                cameraOrigin = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        } else {
            drag = false; 
        }

        if (drag)
        {
            sceneCamera.transform.position = cameraOrigin - difference;
        }
    }

    private void UpdateZoom()
    {
        float changeZoom = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        if (Camera.main.orthographicSize - changeZoom < maxZoom) { return; }
        if (Camera.main.orthographicSize - changeZoom > minZoom) { return; }
        Camera.main.orthographicSize -= changeZoom;
    }

    /// <summary>
    /// Method returns true if mouse is over UI within <c>Floorplan_Editor</c> scene.
    /// </summary>
    /// <returns><c>bool</c></returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// Method returns the grid square the user is hovering over in <c>Floorplan_Editor</c> scene.
    /// </summary>
    /// <returns><c>Vector3</c></returns>
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask)) 
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

}
