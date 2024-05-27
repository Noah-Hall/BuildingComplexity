using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>ExitScript</c> is attached to all <c>Exit</c> target <c>GameObjects</c>.
/// Mainly deals with calling relevant methods within <c>AgentBrehaviorSmart</c> and <c>FileManager</c>.
/// </summary>
public class ExitScript : TargetScript
{
    public override void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    /// <summary>
    /// Calls relevant methods when an agent triggers <c>Exit</c>
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public override void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;
        TargetReached(agent);
    }

    /// <summary>
    /// Calls relevant method if agent remains triggering <c>Exit</c> for too long
    /// (this prevents agents from getting stuck at deadends).
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public override void OnTriggerStay(Collider col)
    {
        return;
    }
    
    /// <summary>
    /// Calls methods to log <c>Exit</c> and agent interaction
    /// </summary>
    /// <param name="agent">The <c>GameObject</c> of the agent.</param>
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
