using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class <c>ManagerScript</c> manages given scene and objects.
/// Mainly, names all important objects within scene.
/// </summary>
public class ManagerScript : MonoBehaviour
{
    private int _stairwellNum = 0;
    // private int _floorNum = 0;
    [SerializeField] Camera cam;
    private float targetZoom;
    [SerializeField] private FileManager fileManager;
    [SerializeField] private const float agentSpeed = 1.24f;

    /// <value>
    /// Property <c>_stairwells</c> is a List of all <c>Stairwell</c> objects.
    /// </value>
    public List<Stairwell> _stairwells = new List<Stairwell>();

    /// <value>
    /// Property <c>simulationSpeed</c> is the speed at which the simulation runs.
    /// </value>
    [SerializeField] public int simulationSpeed = 1;

    /// <value>
    /// Property <c>spawner</c> is object used to spawn agents.
    /// </value>
    public AgentSpawner spawner;

    /// <value>
    /// Property <c>spawnAgents</c> can be set to <c>False</c> to spawn agents manually.
    /// </value>
    public bool spawnAgents;

    /// <summary>
    /// Sets camera, names important objects in scene for coherant LogFiles, 
    /// and does some general initialization.
    /// </summary>
    public void StartRun()
    {
        Time.timeScale = simulationSpeed;
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
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
                    // spawn agent, check weight of current RoomNode, and then set Agent weight accordingly
                    AgentBehaviorSmart tempAgent = spawner.Spawn(roomNodes[i - 1].transform.position);
                    int tempInt = roomNodes[i - 1].GetComponent<NodeScriptRoom>().weight;
                    tempAgent.weight = tempInt < 1 ? 1 : tempInt;
                    tempAgent.GetComponent<NavMeshAgent>().speed = agentSpeed; // * simulationSpeed
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
                stairs[i - 1].name = "stair " + tempN + ":F" + tempF;
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
        // Debug.Log("Square Meters Colliders: " + floorAreaC + "\nSquare Meters Local Scale: " + floorAreaS);

        GameObject[] smartAgents = GameObject.FindGameObjectsWithTag("Smart Agent");
        
        for(int i = 1; i < smartAgents.Length + 1; i++) {
            smartAgents[i - 1].name = "smart agent " + i;
        }

        GameObject[] testAgents = GameObject.FindGameObjectsWithTag("Test Agent");
        
        for(int i = 1; i < testAgents.Length + 1; i++) {
            testAgents[i - 1].name = "test agent " + i;
        }

        fileManager.InitFileManager(simulationSpeed);
        gameObject.GetComponent<FileManager>().GenerateFloorplanData(floorAreaS, moduleNodes.Length + roomNodes.Length, doors.Length, exits.Length, _stairwellNum, navMeshes.Length);
    }

    /// <summary>
    /// Gets <c>Stairwell</c> with given passed <c>_num</c> value.
    /// </summary>
    /// <returns>
    /// <c>Stairwell</c> matching <paramref name="num"/>
    /// </returns>
    /// <param name="num"><c>Stairwell</c> # to get</param>
    public Stairwell GetStairwell(int num)
    {
        foreach (Stairwell stairwell in _stairwells) {
            if (stairwell._num == num) {
                return stairwell;
            }
        }
        return null;
    }

    // fills Stairwell objects with stair GameObjects
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

    // compares two Stairwell objects
    private static int CompareStairNum(Stairwell x_object, Stairwell y_object)
    {
        int x = x_object._num;
        int y = y_object._num;

        return x - y;
    }
}