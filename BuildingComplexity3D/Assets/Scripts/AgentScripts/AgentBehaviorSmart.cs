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
        targets.AddRange(GameObject.FindGameObjectsWithTag("IntersectionNode"));
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
            TargetsEnum closestEnum = TargetsEnum.UNKNOWN;

            foreach (Collider check in rangeChecks) {
                Vector3 target = check.transform.position;
                Vector3 direction = (target - transform.position).normalized;
                float distance = Vector3.Distance(target, transform.position);
                TargetsEnum checkEnum = GetTargetsEnum(check.gameObject);

                if (!Physics.Raycast(transform.position, direction, distance, obstructionMask)) {
                    bool truth = checkEnum < closestEnum;
                    TargetsEnum ch = checkEnum, cl = closestEnum;
                    
                    if (closest == null || checkEnum < closestEnum) { 
                        closest = check.gameObject;
                        targetBound = true;
                        closestDistance = distance;
                        closestEnum = GetTargetsEnum(closest);
                    } else if (checkEnum == closestEnum) {
                        bool lessVisited = visitedTargets[check.gameObject] < visitedTargets[closest];
                        bool closerEqualVisited = visitedTargets[check.gameObject] == visitedTargets[closest] && distance < closestDistance;
                        if (lessVisited || closerEqualVisited) {
                            closest = check.gameObject;
                            targetBound = true;
                            closestDistance = distance;
                            closestEnum = GetTargetsEnum(closest);
                        }
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
            // Debug.Log("Agent Target Location: " + navMeshAgent.destination + "\ntarget: " + targetObject + "\ntag: " + targetObject.tag);
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

    public TargetsEnum GetTargetsEnum(GameObject target)
    {
        switch(target.layer)
        {
            case var value when value == LayerMask.NameToLayer("Exits"):
                return TargetsEnum.EXIT;
                break;
            case var value when value == LayerMask.NameToLayer("Stairs"):
                return TargetsEnum.STAIR;
                break;
            case var value when value == LayerMask.NameToLayer("Doors"):
                return TargetsEnum.DOOR;
                break;
            case var value when value == LayerMask.NameToLayer("Nodes"):
                if (target.tag == "IntersectionNode") {
                    return TargetsEnum.INTERSECTION;
                }
                return TargetsEnum.NODE;
                break;
            default:
                return TargetsEnum.UNKNOWN;
        }
    }
}