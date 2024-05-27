using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>DoorScript</c> is attached to all <c>Door</c> target <c>GameObjects</c>.
/// Mainly deals with calling relevant methods within <c>AgentBrehaviorSmart</c> and <c>FileManager</c>.
/// </summary>
public class DoorScript : TargetScript
{
    public override void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    /// <summary>
    /// Calls relevant methods when an agent triggers <c>Door</c>
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public override void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;
        TargetReached(agent);
    }

    /// <summary>
    /// Calls relevant method if agent remains triggering <c>Door</c> for too long
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
    /// Calls methods to log <c>Door</c> and agent interaction
    /// </summary>
    /// <param name="agent">The <c>GameObject</c> of the agent.</param>
    public override void TargetReached(GameObject agent)
    {
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringLogFile(agent, gameObject);
        
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }
}
