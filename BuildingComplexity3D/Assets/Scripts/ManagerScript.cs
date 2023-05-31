using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*   ManagerScript manages given scene and objects        *
*   Mainly, names all important objects within scene     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public class ManagerScript : MonoBehaviour
{
    [SerializeField] Camera cam;
    float targetZoom;
    
    // Sets camera
    // names all important objects in scene for coherant LogFiles
    void Awake()
    {
        Camera.main.orthographicSize = 16;
        GameObject[] moduleNodes = GameObject.FindGameObjectsWithTag("ModuleNode");
        GameObject[] roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        GameObject[] smartAgents = GameObject.FindGameObjectsWithTag("Smart Agent");
        
        for(int i = 1; i < doors.Length + 1; i++) {
            doors[i - 1].name = "door " + i;
        }

        for(int i = 1; i < exits.Length + 1; i++) {
            exits[i - 1].name = "exit " + i;
        }

        for(int i = 1; i < moduleNodes.Length + 1; i++) {
            moduleNodes[i - 1].name = "module node " + i;
        }

        for(int i = 1; i < roomNodes.Length + 1; i++) {
            roomNodes[i - 1].name = "room node " + i;
        }

        for(int i = 1; i < smartAgents.Length + 1; i++) {
            smartAgents[i - 1].name = "smart agent " + i;
        }
    }
}