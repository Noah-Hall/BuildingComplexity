using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>StairScript</c> is attached to all <c>Stair</c> target <c>GameObjects</c>.
/// Mainly deals with calling relevant methods within <c>AgentBrehaviorSmart</c> and <c>FileManager</c>.
/// </summary>
public class StairScript : TargetScript
{
    /// <value>
    /// Property <c>_floor</c> is floor # which the <c>Stair</c> is on.
    /// </value>
    public int _floor;

    /// <value>
    /// Property <c>_stairwell</c> is stairwell # which the <c>Stair</c> is attached to.
    /// </value>
    public int _stairwell;

    /// <value>
    /// Property <c>_isExitFloor</c> should be true if there is an <c>Exit</c> on the same floor as the <c>Stair</c>.
    /// </value>
    public bool _isExitFloor;

    public override void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    /// <summary>
    /// Calls relevant methods when an agent triggers <c>Stair</c>
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
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

    /// <summary>
    /// Calls relevant method if agent remains triggering <c>Stair</c> for too long
    /// (this prevents agents from getting stuck at deadends).
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public override void OnTriggerStay(Collider col)
    {
        GameObject agent = col.gameObject;
        TargetReached(agent);
    }

    /// <summary>
    /// Calls methods to log <c>Stair</c> and agent interaction
    /// </summary>
    /// <param name="agent">The <c>GameObject</c> of the agent.</param>
    public override void TargetReached(GameObject agent)
    {
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringLogFile(agent, gameObject);
        GameObject temp = agent.GetComponent<AgentBehaviorSmart>().StairReached(gameObject);
        file.WriteStringLogFile(agent, temp);
    }
}
