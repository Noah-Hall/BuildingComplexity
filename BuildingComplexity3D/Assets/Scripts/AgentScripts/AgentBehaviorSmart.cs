using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   AgentBehaviorSmart is attached to all Smart Agents   *
*   This script handles all Smart Agent behavior         *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public class AgentBehaviorSmart : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float radius = 50;
    [SerializeField] private LayerMask targetMask, obstructionMask;
    private IDictionary<GameObject, int> visitedTargets = new Dictionary<GameObject, int>();
    public bool targetBound, reachedCooldown;
    public GameObject targetObject;
    public int _currentFloor;
    public ManagerScript manager;

    // general initialization
    private void Awake()
    {
        manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("ModuleNode"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("RoomNode"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Door"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Exit"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Stair"));

        foreach(GameObject target in targets) {
            visitedTargets.Add(target, 0);
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        targetMask = LayerMask.GetMask("Exits", "Doors", "Nodes", "Stairs");
        obstructionMask = LayerMask.GetMask("Walls");
        StartCoroutine(FOVRoutine());
    }

    // sets targetBound to false if agent has reached target
    private void Update()
    {
        if (transform.position == navMeshAgent.destination) {
            targetBound = false;
        }
    }

    // calls FieldOfViewCheck 5 times a second (this is more optimal than every frame)
    private IEnumerator FOVRoutine() 
    {
        while(true) {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    // cooldown used for OnTriggerStay in TargetScript
    private IEnumerator Cooldown()
    {
        reachedCooldown = true;
        yield return new WaitForSeconds(2f);
        reachedCooldown = false;
    }

    // Looks for closest Exit, closest least-visited Door, or closest least-visited Node (in that order of priority)
    // Sets that "target" as agent's destination
    private void FieldOfViewCheck()
    {
        targetBound = false;
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length > 0) {
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (Collider check in rangeChecks) {
                Vector3 target = check.transform.position;
                Vector3 direction = (target - transform.position).normalized;
                float distance = Vector3.Distance(target, transform.position);

                if (!Physics.Raycast(transform.position, direction, distance, obstructionMask)) {
                    if (closest == null) { 
                        closest = check.gameObject;
                        targetBound = true;
                        closestDistance = distance;
                    }
                    switch(check.gameObject.layer) {
                        case var value when value == LayerMask.NameToLayer("Exits"):
                            if (closest.layer != LayerMask.NameToLayer("Exits") || distance < closestDistance) {
                                targetBound = true;
                                closest = check.gameObject;
                                closestDistance = distance;
                            }
                            break;
                        case var value when value == LayerMask.NameToLayer("Doors"):
                            switch (closest.layer) {
                                case var val when val == LayerMask.NameToLayer("Exits"):
                                    break;
                                case var val when val == LayerMask.NameToLayer("Doors"):
                                    if (distance < closestDistance) {
                                        if (visitedTargets[check.gameObject] <= visitedTargets[closest]) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        }
                                    } else if (visitedTargets[check.gameObject] < visitedTargets[closest]) {
                                        targetBound = true;
                                        closest = check.gameObject;
                                        closestDistance = distance;
                                    }
                                    break;
                                case var val when val == LayerMask.NameToLayer("Nodes"):
                                    targetBound = true;
                                    closest = check.gameObject;
                                    closestDistance = distance;
                                    break;
                                case var val when val == LayerMask.NameToLayer("Stairs"):
                                    if(closest.gameObject.GetComponent<StairScript>()._isExitFloor) {
                                        targetBound = true;
                                        closest = check.gameObject;
                                        closestDistance = distance;
                                        break;
                                    }
                                    break;
                                default:
                                    // Debug.Log("Default Reached");
                                    break;
                            }
                            break;
                        case var value when value == LayerMask.NameToLayer("Nodes"):
                            switch (closest.layer) {
                                case var val when val == LayerMask.NameToLayer("Exits"):
                                    break;
                                case var val when val == LayerMask.NameToLayer("Doors"):
                                    break;
                                case var val when val == LayerMask.NameToLayer("Nodes"):
                                    if (distance < closestDistance) {
                                        if (visitedTargets[check.gameObject] <= visitedTargets[closest]) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        }
                                    } else if (visitedTargets[check.gameObject] < visitedTargets[closest]) {
                                        targetBound = true;
                                        closest = check.gameObject;
                                        closestDistance = distance;
                                    }
                                    break;
                                case var val when val == LayerMask.NameToLayer("Stairs"):
                                    if(closest.gameObject.GetComponent<StairScript>()._isExitFloor) {
                                        targetBound = true;
                                        closest = check.gameObject;
                                        closestDistance = distance;
                                        break;
                                    }
                                    break;
                                default:
                                    // Debug.Log("Default Reached");
                                    break;
                            }
                            break;
                        case var value when value == LayerMask.NameToLayer("Stairs"):
                            switch (closest.layer) {
                                case var val when val == LayerMask.NameToLayer("Exits"):
                                    break;
                                case var val when val == LayerMask.NameToLayer("Doors"):
                                    if (check.gameObject.GetComponent<StairScript>()._isExitFloor) {
                                        break;
                                    }
                                    targetBound = true;
                                    closest = check.gameObject;
                                    closestDistance = distance;
                                    break;
                                case var val when val == LayerMask.NameToLayer("Nodes"):
                                    if (check.gameObject.GetComponent<StairScript>()._isExitFloor) {
                                        break;
                                    }
                                    targetBound = true;
                                    closest = check.gameObject;
                                    closestDistance = distance;
                                    break;
                                case var val when val == LayerMask.NameToLayer("Stairs"):
                                    if (distance < closestDistance) {
                                        if (visitedTargets[check.gameObject] <= visitedTargets[closest]) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        }
                                    } else if (visitedTargets[check.gameObject] < visitedTargets[closest]) {
                                        targetBound = true;
                                        closest = check.gameObject;
                                        closestDistance = distance;
                                    }
                                    break;
                                default:
                                    // Debug.Log("Default Reached");
                                    break;
                            }
                            break;
                        default:
                            // Debug.Log("Default Reached");
                            break;
                    }
                }
            }

            if (targetBound) {
                navMeshAgent.destination = closest.transform.position;
                targetObject = closest;
            } else {
                // Debug.Log("FOV Check targetBound else");
                Debug.Log(gameObject.name + " stranded. Try adding Nodes to scene to prevent \"dead zones\"");
            }
        } else {
            // Debug.Log("FOV Check else");
            Debug.Log(gameObject.name + " stranded. Try adding Nodes to scene to prevent \"dead zones\"");
        }
    }

    // Method called by TargetScript when agent triggers object
    public void TargetReached(GameObject target)
    {
        StartCoroutine(Cooldown());
        visitedTargets[target] = visitedTargets[target] + 1;
        if (target.layer == LayerMask.NameToLayer("Doors")) {
            Vector3 move = transform.forward;
            navMeshAgent.Move(move);
        }
    }

    public GameObject StairReached(GameObject target)
    {
        ManagerScript manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
        int temp = target.GetComponent<StairScript>()._stairwell;
        Stairwell stair = manager.GetStairwell(temp);
        int count = stair._count;
        GameObject loc = stair.GetExitFloor();
        navMeshAgent.Warp(loc.transform.position);

        StartCoroutine(Cooldown());
        visitedTargets[target] = visitedTargets[target] + 1;
        Vector3 move = transform.forward;
        navMeshAgent.Move(move);

        return loc;
    }
}