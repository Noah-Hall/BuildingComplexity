using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class <c>AgentBehaviorSmart</c> handles all Smart Agent behavior.
/// </summary>
public class AgentBehaviorSmart : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float radius = 50;
    [SerializeField] private LayerMask targetMask, obstructionMask;
    private IDictionary<GameObject, int> visitedTargets = new Dictionary<GameObject, int>();
    private Vector3 startPosition;
    private Vector3 prevPosition;
    private bool initialized = false;

    public bool targetBound, reachedCooldown;
    public ManagerScript manager;

    /// <value>
    /// Property <c>targetObject</c> the target GameObject the agent is currently moving towards.
    /// </value>
    public GameObject targetObject;

    /// <value>
    /// Property <c>_currentFloor</c> what floor the agent is currently on.
    /// </value>
    public int _currentFloor;

    /// <value>
    /// Property <c>numVisitedTargets</c> how many targets the agent has visited.
    /// </value>
    public int numVisitedTargets = 0;

    /// <value>
    /// Property <c>lineToExit</c> the distance between agent start position and exit position (meters).
    /// </value>
    public float lineToExit;

    /// <value>
    /// Property <c>totalDistanceTraveled</c> the distance the agent has currently traveled (meters).
    /// </value>
    public float totalDistanceTraveled = 0;

    /// <value>
    /// Property <c>weight</c> how many "people" the agent represents.
    /// </value>
    public int weight;


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
        startPosition = prevPosition = transform.position;
        // StartCoroutine(FOVRoutine());
        initialized = true;
    }

    // updates totalDistanceTraveled
    private void Update()
    {
        totalDistanceTraveled += Vector3.Distance(transform.position, prevPosition);
        prevPosition = transform.position;
    }

    // calls FieldOfViewCheck 5 times a second (this is more optimal than every frame)
    private IEnumerator FOVRoutine() 
    {
        while(true) {
            yield return new WaitForSeconds(0.2f);
            // FieldOfViewCheck();
        }
    }

    // cooldown used for OnTriggerStay in TargetScript
    private IEnumerator Cooldown()
    {
        reachedCooldown = true;
        yield return new WaitForSeconds(0.2f);
        reachedCooldown = false;
    }

    // Looks for closest Exit, closest least-visited Door, or closest least-visited Node (in that order of priority)
    // Sets that "target" as agent's destination
    private void FixedUpdate()
    {
        if (!initialized) { return; }
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
                transform.LookAt(navMeshAgent.destination);
            } else {
                Debug.Log(gameObject.name + " stranded. Try adding Nodes to scene to prevent \"dead zones\"");
            }
        } else {
            Debug.Log(gameObject.name + " stranded. Try adding Nodes to scene to prevent \"dead zones\"");
        }
    }

    /// <summary>
    /// Method called by <c>TargetScript</c> when agent triggers target object.
    /// </summary>
    /// <param name="target">The <c>GameObject</c> of the triggered target.</param>
    public void TargetReached(GameObject target)
    {
        numVisitedTargets++;
        targetBound = false;
        StartCoroutine(Cooldown());
        visitedTargets[target] = visitedTargets[target] + 1;
        if (target.layer == LayerMask.NameToLayer("Doors")) {
            Vector3 move = navMeshAgent.destination;
            // use y rotation of transform to find direction of agent and use direction to find vector of other side of door
            // (1.111111111 * (y-rotation % 90)) / 100 = x
            // z = 1.0 - x
            int switch_direction = (int)(transform.eulerAngles.y) / 90;
            float x, z;
            switch (switch_direction) {
                // down
                case 0:
                    x = (1.111111111f * (transform.eulerAngles.y % 90f)) / 100f;
                    z = 1f - x;
                    move = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
                    break;
                // left
                case 1:
                    x = (1.111111111f * (transform.eulerAngles.y % 90f)) / 100f;
                    z = 1f - x;
                    move = new Vector3(transform.position.x + x, transform.position.y, transform.position.z - z);
                    break;
                // up
                case 2:
                    x = (1.111111111f * (transform.eulerAngles.y % 90f)) / 100f;
                    z = 1f - x;
                    move = new Vector3(transform.position.x - x, transform.position.y, transform.position.z - z);
                    break;
                // right
                default:
                    x = (1.111111111f * (transform.eulerAngles.y % 90f)) / 100f;
                    z = 1f - x;
                    move = new Vector3(transform.position.x - x, transform.position.y, transform.position.z + z);
                    break;

            }
            // Debug.Log("---TargetReached---\nx: " + x + ", z: " + z + "\nY-rotation: " + transform.eulerAngles.y + "\nswitch_dir: " + switch_direction);
            navMeshAgent.destination = move;
            transform.LookAt(navMeshAgent.destination);
        }
    }

    /// <summary>
    /// Method called by <c>StairScript</c> when agent triggers stair object.
    /// </summary>
    /// <param name="target">The <c>GameObject</c> of the triggered stair.</param>
    public GameObject StairReached(GameObject target)
    {
        numVisitedTargets++;
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

    /// <summary>
    /// Method called by <c>ExitScript</c> to calculate the agent's beginning and ending positions.
    /// </summary>
    /// <param name="target">The <c>GameObject</c> of the triggered exit.</param>
    public void FingerPrint(Vector3 exitPosition)
    {
        lineToExit = Vector3.Distance(exitPosition, startPosition);
    }

    /// <summary>
    /// Helper method for <c>FieldOfViewCheck</c>
    /// </summary>
    /// <returns>
    /// The <c>TargetsEnum</c> value of a given target <c>GameObject</c>.
    /// </returns>
    /// <param name="target">The <c>GameObject</c> of the given target.</param>
    public TargetsEnum GetTargetsEnum(GameObject target)
    {
        switch(target.layer)
        {
            case var value when value == LayerMask.NameToLayer("Exits"):
                return TargetsEnum.EXIT;
            case var value when value == LayerMask.NameToLayer("Stairs"):
                return TargetsEnum.STAIR;
            case var value when value == LayerMask.NameToLayer("Doors"):
                if (visitedTargets[target] > 1) {
                    return TargetsEnum.VISITED_DOOR;
                }
                return TargetsEnum.DOOR;
            case var value when value == LayerMask.NameToLayer("Nodes"):
                if (target.tag == "IntersectionNode" && visitedTargets[target] < 1) {
                    return TargetsEnum.INTERSECTION;
                }
                if (visitedTargets[target] >= 1) {
                    return TargetsEnum.VISITED_NODE;
                }
                return TargetsEnum.NODE;
            default:
                return TargetsEnum.UNKNOWN;
        }
    }
}