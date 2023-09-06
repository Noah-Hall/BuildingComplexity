using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   TargetScript is attached to all target objects           *
*   (Exits, Doors, Nodes, etc.)                              *
*   script mainly takes care of calling necessary methods    *
*   within other classes when an Agent triggers a target     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public abstract class TargetScript : MonoBehaviour
{
    public GameObject manager;

    // initializes manager GameObject
    public abstract void Awake();

    // calls relevant method when an agent triggers target
    public abstract void OnTriggerEnter(Collider col);

    // calls relevant method if agent remains triggering target for too long
    // (this prevents agents from getting stuck at deadends or starting positions if they spawn overlapping a target)
    public abstract void OnTriggerStay(Collider col);

    // calls method to log Target from FileManager
    // if Target == Door || Target == Node-> calls method for agent to know it has reached its current target
    // if Target == Exit-> Destroys agent, and checks if there are any more agents in scene
    public abstract void TargetReached(GameObject agent);
}