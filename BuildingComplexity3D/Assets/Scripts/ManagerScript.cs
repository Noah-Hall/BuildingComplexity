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
    private float targetZoom;
    public AgentSpawner spawner;
    public bool spawnAgents;

    // Sets camera
    // names all important objects in scene for coherant LogFiles
    void Awake()
    {
        // spawnAgents = true;
        if (spawnAgents) {
            spawner = GameObject.Find("Smart Agent Spawner").GetComponent<AgentSpawner>();
        }
        Camera.main.orthographicSize = 16;
        GameObject[] moduleNodes = GameObject.FindGameObjectsWithTag("ModuleNode");
        GameObject[] roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        GameObject[] smartAgents = GameObject.FindGameObjectsWithTag("Smart Agent");
        
        for(int i = 1; i < roomNodes.Length + 1; i++) {
            if (roomNodes[i - 1].activeInHierarchy) {
                if (spawnAgents) {
                    spawner.Spawn(roomNodes[i - 1].transform.position);
                }
                roomNodes[i - 1].name = "room node " + i;
            }
        }

        for(int i = 1; i < moduleNodes.Length + 1; i++) {
            if (moduleNodes[i - 1].activeInHierarchy) {
                moduleNodes[i - 1].name = "module node " + i;
            }
        }

        for(int i = 1; i < doors.Length + 1; i++) {
            if (doors[i - 1].activeInHierarchy) {
                doors[i - 1].name = "door " + i;
            }
        }

        for(int i = 1; i < exits.Length + 1; i++) {
            exits[i - 1].name = "exit " + i;
        }

        for(int i = 1; i < smartAgents.Length + 1; i++) {
            smartAgents[i - 1].name = "smart agent " + i;
        }
    }
}