using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   ExitScript is attached to all Exit target objects        *
*   script calls necessary methods when an                   *
*   Agent triggers an Exit                                   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class ExitScript : TargetScript
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

    // simply returns since agent should be destroyed after on OnTriggerEnter
    public override void OnTriggerStay(Collider col)
    {
        return;
    }

    // calls method to log Target from FileManager
    // Destroys agent, and checks if there are any more agents in scene
    public override void TargetReached(GameObject agent)
    {
        agent.GetComponent<AgentBehaviorSmart>().FingerPrint(transform.position);

        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringLogFile(agent, gameObject);

        file.WriteAgentData(agent);
        Destroy(agent);
        
        GameObject[] smartAgents = GameObject.FindGameObjectsWithTag("Smart Agent");
        if (smartAgents.Length - 1 == 0) {
            file.GenerateFiles();
        }
    }
}
