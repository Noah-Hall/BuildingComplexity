using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;
    private Vector3 lastPosition;
    private Vector3 mousePosition;
    public event Action OnClicked, OnExit;

    private void Start()
    {
        mousePosition = Input.mousePosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { OnClicked?.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { OnExit?.Invoke(); }
        if (mousePosition != Input.mousePosition) 
        {
            Vector3 distance = mousePosition - Input.mousePosition;
            Vector3 camPos = sceneCamera.gameObject.transform.position;
            sceneCamera.gameObject.transform.position = new Vector3(camPos.x - distance.x, camPos.y, camPos.z - distance.z);
        }
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

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
