using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*   ManagerScript manages given scene and objects        *
*   Mainly, names all important objects within scene     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public class ManagerScript : MonoBehaviour
{
    public List<Stairwell> _stairwells = new List<Stairwell>();
    private int _stairwellNum = 0;
    private int _floorNum = 0;
    [SerializeField] Camera cam;
    private float targetZoom;
    public AgentSpawner spawner;
    public bool spawnAgents;

    // Sets camera
    // names all important objects in scene for coherant LogFiles
    void Awake()
    {
        if (spawnAgents) {
            spawner = GameObject.Find("Smart Agent Spawner").GetComponent<AgentSpawner>();
        }
        Camera.main.orthographicSize = 16;
        GameObject[] moduleNodes = GameObject.FindGameObjectsWithTag("ModuleNode");
        GameObject[] roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        GameObject[] intersectionNodes = GameObject.FindGameObjectsWithTag("IntersectionNode");
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        GameObject[] stairs = GameObject.FindGameObjectsWithTag("Stair");
        GameObject[] navMeshes = GameObject.FindGameObjectsWithTag("NavMesh");
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        
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

        for(int i = 1; i < intersectionNodes.Length + 1; i++) {
            if (intersectionNodes[i - 1].activeInHierarchy) {
                intersectionNodes[i - 1].name = "intersection node " + i;
            }
        }

        for(int i = 1; i < doors.Length + 1; i++) {
            if (doors[i - 1].activeInHierarchy) {
                doors[i - 1].name = "door " + i;
            }
        }

        for(int i = 1; i < exits.Length + 1; i++) {
            if (exits[i - 1].activeInHierarchy) {
                exits[i - 1].name = "exit " + i;
            }
        }

        int stairNum = 0;
        for (int i = 1; i < stairs.Length + 1; i++) {
            if (stairs[i - 1].activeInHierarchy) {
                int tempF = stairs[i - 1].GetComponent<StairScript>()._floor;
                int tempN = stairs[i - 1].GetComponent<StairScript>()._stairwell;
                stairs[i - 1].name = ("stair " + tempN + ":F" + tempF);
                if (tempN > stairNum) { stairNum = tempN; }
            }
        }

        FillStairwells();

        //calculate floor area
        // float floorAreaC = 0; // for collider calculation (removed)
        float floorAreaS = 0;
        foreach(GameObject floor in floors) {
            // Vector3 size = floor.GetComponent<Collider>().bounds.size;
            // floorAreaC += size.x * size.z;
            float tempArea = floor.transform.localScale.x * floor.transform.localScale.z;
            floorAreaS += tempArea;
        }
        Debug.Log("Square Meters Colliders: " + floorAreaC + "\nSquare Meters Local Scale: " + floorAreaS);

        GameObject[] smartAgents = GameObject.FindGameObjectsWithTag("Smart Agent");
        
        for(int i = 1; i < smartAgents.Length + 1; i++) {
            smartAgents[i - 1].name = "smart agent " + i;
        }

        GameObject[] testAgents = GameObject.FindGameObjectsWithTag("Test Agent");
        
        for(int i = 1; i < testAgents.Length + 1; i++) {
            testAgents[i - 1].name = "test agent " + i;
        }

        gameObject.GetComponent<FileManager>().GenerateFloorplanData(floorAreaS, moduleNodes.Length + roomNodes.Length, doors.Length, exits.Length, _stairwellNum, navMeshes.Length);
    }

    public Stairwell GetStairwell(int num)
    {
        foreach (Stairwell stairwell in _stairwells) {
            if (stairwell._num == num) {
                return stairwell;
            }
        }
        return null;
    }

    private void FillStairwells()
    {
        GameObject[] stairs = GameObject.FindGameObjectsWithTag("Stair");

        foreach (GameObject stair in stairs) {
            int floor = stair.GetComponent<StairScript>()._floor;
            int num = stair.GetComponent<StairScript>()._stairwell;
            bool found = false;

            for (int i = 0; i < _stairwells.Count; i++) {
                if (_stairwells[i]._num == num) {
                    found = true;
                    _stairwells[i].Add(stair);
                    break;
                }
            }

            if (!found) {
                _stairwellNum++;
                Stairwell newStairwell = new Stairwell(num);
                newStairwell.Add(stair);
                _stairwells.Add(newStairwell);
            }
        }

        _stairwells.Sort(CompareStairNum);
    }

    private static int CompareStairNum(Stairwell x_object, Stairwell y_object)
    {
        int x = x_object._num;
        int y = y_object._num;

        return x - y;
    }
}