using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   AgentTestingScript used for testing new features before  *
*   migrating them to SmartAgents                            *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class AgentTestingScript : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float radius = 50;
    [SerializeField] private LayerMask targetMask, obstructionMask;
    private IDictionary<GameObject, int> visitedTargets = new Dictionary<GameObject, int>();
    public bool targetBound, reachedCooldown;
    public GameObject targetObject;
    public int _currentFloor;

    // general initialization
    private void Awake()
    {
        _currentFloor = 2;
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("Stair"));

        foreach(GameObject target in targets) {
            visitedTargets.Add(target, 0);
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        targetMask = LayerMask.GetMask("Stairs");
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
                    } else if (distance < closestDistance) {
                        targetBound = true;
                        closest = check.gameObject;
                        closestDistance = distance;
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

    public void StairReached(GameObject target)
    {
        ManagerScript manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
        int temp = target.GetComponent<StairScript>()._stairwell;
        Stairwell stair = manager.GetStairwell(temp);
        // if (stair is not null) {
        //     Debug.Log("not null");
        // } else {
        //     Debug.Log("null");
        // }
        int count = stair._count;
        GameObject loc = stair.GetExitFloor();
        navMeshAgent.Warp(loc.transform.position);
    }
}
