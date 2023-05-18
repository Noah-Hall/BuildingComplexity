using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    [SerializeField] Camera cam;
    float targetZoom;
    
    // Start is called before the first frame update
    void Awake()
    {
        Camera.main.orthographicSize = 16;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        
        for(int i = 1; i < doors.Length + 1; i++) {
            doors[i - 1].name = "door " + i;
        }

        for(int i = 1; i < exits.Length + 1; i++) {
            exits[i - 1].name = "exit " + i;
        }
    }
}