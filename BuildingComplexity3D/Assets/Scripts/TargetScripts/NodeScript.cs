using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>NodeScript</c> is attached to all <c>Node</c> target <c>GameObjects</c>.
/// Mainly deals with calling relevant methods within <c>AgentBrehaviorSmart</c> and <c>FileManager</c>.
/// </summary>
public class NodeScript : TargetScript
{
    public override void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    /// <summary>
    /// Calls relevant methods when an agent triggers <c>Node</c>
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public override void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;
        TargetReached(agent);
    }

    /// <summary>
    /// Calls relevant method if agent remains triggering <c>Node</c> for too long
    /// (this prevents agents from getting stuck at deadends).
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public override void OnTriggerStay(Collider col)
    {
        GameObject agent = col.gameObject;
        if(agent.GetComponent<AgentBehaviorSmart>().reachedCooldown || agent.GetComponent<AgentBehaviorSmart>().targetObject.GetInstanceID() != gameObject.GetInstanceID()) {
            return;
        }
        
        TargetReached(agent);
    }

    /// <summary>
    /// Calls methods to log <c>Node</c> and agent interaction
    /// </summary>
    /// <param name="agent">The <c>GameObject</c> of the agent.</param>
    public override void TargetReached(GameObject agent)
    {
        Debug.Log(agent);
        Debug.Log(gameObject);
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringLogFile(agent, gameObject);
        
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }
}
