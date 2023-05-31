using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   TargetScript is attached to all target objects           *
*   (Exits, Doors, Nodes, etc.)                              *
*   script mainly takes care of calling necessary methods    *
*   within other classes when an Agent triggers a target     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public class TargetScript : MonoBehaviour
{
    [SerializeField] GameObject manager;

    // initializes manager GameObject
    void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    // calls relevant method when an agent triggers target
    private void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;

        if (gameObject.name.Contains("exit")) {
            ExitReached(agent);
        } else if (gameObject.name.Contains("door")) {
            DoorReached(agent);
        } else if (gameObject.name.Contains("node")) {
            NodeReached(agent);
        }
    }

    // calls relevant method if agent remains triggering target for too long
    // (this prevents agents from getting stuck at deadends or starting positions if they spawn overlapping a target)
    private void OnTriggerStay(Collider col)
    {
        GameObject agent = col.gameObject;
        if(agent.GetComponent<AgentBehaviorSmart>().reachedCooldown || agent.GetComponent<AgentBehaviorSmart>().targetObject.GetInstanceID() != gameObject.GetInstanceID()) {
            return;
        }

        if (gameObject.name.Contains("exit")) {
            return;
            // ExitReached(agent);
        } else if (gameObject.name.Contains("door")) {
            DoorReached(agent);
        } else if (gameObject.name.Contains("node")) {
            NodeReached(agent);
        }
    }

    // calls method to log Exit from FileManager, Destroys agent, and checks if there are any more agents in scene
    private void ExitReached(GameObject agent)
    {
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringExit(agent, gameObject);
        Destroy(agent);

        GameObject[] smartAgents = GameObject.FindGameObjectsWithTag("Smart Agent");
        if (smartAgents.Length - 1 == 0) {
            file.CalculateResults();
        }
    }

    // calls method to log Door from FileManager, calls method for agent to know it has reached its current target
    private void DoorReached(GameObject agent) {
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringDoor(agent, gameObject);
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }

    // calls method to log Node from FileManager, calls method for agent to know it has reached its current target
    private void NodeReached(GameObject agent) {
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringNode(agent, gameObject);
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }
}