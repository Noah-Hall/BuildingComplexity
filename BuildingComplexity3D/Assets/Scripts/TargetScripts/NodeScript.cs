using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   NodeScript is attached to all Node target objects        *
*   script calls necessary methods when an                   *
*   Agent triggers a Node                                    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class NodeScript : TargetScript
{
    // initializes manager GameObject
    public override void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    // calls relevant method when an agent triggers target
    public override void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;
        TargetReached(agent);
    }

    // calls relevant method if agent remains triggering target for too long
    // (this prevents agents from getting stuck at deadends or starting positions if they spawn overlapping a target)
    public override void OnTriggerStay(Collider col)
    {
        GameObject agent = col.gameObject;
        if(agent.GetComponent<AgentBehaviorSmart>().reachedCooldown || agent.GetComponent<AgentBehaviorSmart>().targetObject.GetInstanceID() != gameObject.GetInstanceID()) {
            return;
        }
        
        TargetReached(agent);
    }

    // calls method to log Target from FileManager
    // calls method for agent to know it has reached its current target
    public override void TargetReached(GameObject agent)
    {
        Debug.Log(agent);
        Debug.Log(gameObject);
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringLogFile(agent, gameObject);
        
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }
}
