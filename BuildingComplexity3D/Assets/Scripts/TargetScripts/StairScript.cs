using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   StairScript is attached to all Stair target objects      *
*   script calls necessary methods when an                   *
*   Agent triggers a Stair                                   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class StairScript : TargetScript
{
    public int _floor;
    public int _stairwell;
    public bool _isExitFloor;

    // initializes manager GameObject
    public override void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    // calls relevant method when an agent triggers target
    public override void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;
        if(agent.name.Contains("smart agent")) {
            if(agent.GetComponent<AgentBehaviorSmart>().reachedCooldown || agent.GetComponent<AgentBehaviorSmart>().targetObject.GetInstanceID() != gameObject.GetInstanceID()) {
                return;
            }

            TargetReached(agent);
        } else if (agent.name.Contains("test agent")) {
            Debug.Log("test agent");
            agent.GetComponent<AgentTestingScript>().StairReached(gameObject);
        }
    }

    // calls relevant method if agent remains triggering target for too long
    // (this prevents agents from getting stuck at deadends or starting positions if they spawn overlapping a target)
    public override void OnTriggerStay(Collider col)
    {
        GameObject agent = col.gameObject;
        TargetReached(agent);
    }

    // calls method to log Target from FileManager
    // if Target == Door || Target == Node-> calls method for agent to know it has reached its current target
    // if Target == Exit-> Destroys agent, and checks if there are any more agents in scene
    public override void TargetReached(GameObject agent)
    {
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringLogFile(agent, gameObject);
        GameObject temp = agent.GetComponent<AgentBehaviorSmart>().StairReached(gameObject);
        file.WriteStringLogFile(agent, temp);
    }
}
