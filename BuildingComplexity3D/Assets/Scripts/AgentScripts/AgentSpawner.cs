using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*   AgentSpawner handles logic to automatically spawn    *
*   SmartAgents around the floorplan based on RoomNodes  *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class AgentSpawner : MonoBehaviour
{
    public GameObject smartAgentPrefab;
    private GameObject tempAgent;
    private int count = 1;

    // Start is called before the first frame update
    void Awake()
    {
        // smartAgentPrefab = Resources.Load("Agents/SmartAgent3D", GameObject) as GameObject;
    }

    public void Spawn(Vector3 position) {
        tempAgent = Instantiate(smartAgentPrefab, position, Quaternion.identity);
        // tempAgent.name = "smart agent " + count++;
    }
}
