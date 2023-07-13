using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public GameObject smartAgentPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        // smartAgentPrefab = Resources.Load("Agents/SmartAgent3D", GameObject) as GameObject;
    }

    public void Spawn(Vector3 position) {
        Instantiate(smartAgentPrefab, position, Quaternion.identity);
    }
}
