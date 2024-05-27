using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract Class <c>TargetScript</c> is the base Class from which all target scripts inheret.
/// Mainly deals with calling relevant methods within <c>AgentBrehaviorSmart</c> and <c>FileManager</c>.
/// </summary>
public abstract class TargetScript : MonoBehaviour
{
    /// <value>
    /// Property <c>manager</c> is the Manager <c>GameObject</c> of the scene.
    /// </value>
    public GameObject manager;

    /// <summary>
    /// Initializes <c>manager</c>.
    /// </summary>
    public abstract void Awake();

    /// <summary>
    /// Calls relevant methods when an agent triggers target
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public abstract void OnTriggerEnter(Collider col);

    /// <summary>
    /// Calls relevant method if agent remains triggering target for too long
    /// (this prevents agents from getting stuck at deadends or starting positions if they spawn overlapping a target).
    /// </summary>
    /// <param name="col">The <c>Collider</c> of the agent.</param>
    public abstract void OnTriggerStay(Collider col);

    /// <summary>
    /// Calls method to log <c>Target</c> from <c>FileManager</c>
    /// </summary>
    /// <param name="agent">The <c>GameObject</c> of the agent.</param>
    public abstract void TargetReached(GameObject agent);
}