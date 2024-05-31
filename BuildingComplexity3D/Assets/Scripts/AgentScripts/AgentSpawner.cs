using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class <c>AgentSpawner</c> handles logic to automatically spawn <c>SmartAgents</c> around the floorplan based on <c>RoomNodes</c>.
/// </summary>
public class AgentSpawner : MonoBehaviour
{
    /// <value>
    /// Property <c>smartAgentPrefab</c> The <c>GameObject</c> prefab to spawn.
    /// </value>
    public GameObject smartAgentPrefab;
    private GameObject tempAgent;
    // private int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // smartAgentPrefab = Resources.Load("Agents/SmartAgent3D", GameObject) as GameObject;
    }

    /// <summary>
    /// Method <c>Instantiates</c> one <c>smartAgentPrefab</c>.
    /// </summary>
    /// <returns>
    /// The <c>AgentBehaviorSmart</c> script attached to <c>smartAgentPrefab</c>.
    /// </returns>
    /// <param name="position">The <c>Vector3</c> position at which to <c>Instantiate</c> the <c>smartAgentPrefab</c>.</param>
    public AgentBehaviorSmart Spawn(Vector3 position) {
        tempAgent = Instantiate(smartAgentPrefab, position, Quaternion.identity);
        // tempAgent.name = "smart agent " + count++;
        return tempAgent.GetComponent<AgentBehaviorSmart>();
    }
}
