using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBehaviorSmart : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float radius = 50;
    [SerializeField] private LayerMask targetMask, obstructionMask;
    IDictionary<GameObject, int> visitedTargets = new Dictionary<GameObject, int>();
    public bool targetBound, reachedCooldown;

    //debugging stuff
    [SerializeField] private GameObject dest;
    public bool debuggingOn = false;

    private void Awake()
    {
        if (debuggingOn) {
            dest = GameObject.Find("Destination");
        }

        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("ModuleNode"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("RoomNode"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Door"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Exit"));

        foreach(GameObject target in targets) {
            visitedTargets.Add(target, 0);
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        targetMask = LayerMask.GetMask("Exits", "Doors", "Nodes");
        obstructionMask = LayerMask.GetMask("Walls");
        StartCoroutine(FOVRoutine());
        // StartCoroutine(Logging());
    }

    private void Update()
    {
        if (transform.position == navMeshAgent.destination) {
            targetBound = false;
        }
    }

    private IEnumerator Logging() {
        while(true) {
            Debug.Log("Destination: " + navMeshAgent.destination);
            foreach(KeyValuePair<GameObject, int> visited in visitedTargets) {
                if (visited.Value > 0) {
                    Debug.Log(visited.Key.name + " : " + visited.Value + " : " + visited.Key.transform.position);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator FOVRoutine() 
    {
        while(true) {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    private IEnumerator Cooldown()
    {
        reachedCooldown = true;
        yield return new WaitForSeconds(1f);
        reachedCooldown = false;
    }

    private void FieldOfViewCheck()
    {
        // Debug.Log("FOV Check");
        targetBound = false;
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length > 0) {
            // Debug.Log("FOV Check Length > 0");
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (Collider check in rangeChecks) {
                Vector3 target = check.transform.position;
                Vector3 direction = (target - transform.position).normalized;
                float distance = Vector3.Distance(target, transform.position);
                // // Debug.Log(check.gameObject.name);

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
                // Debug.Log("FOV Check targetBound");
                navMeshAgent.destination = closest.transform.position;
                if (debuggingOn) {
                    dest.transform.position = closest.transform.position;
                }
            } else {
                // Debug.Log("FOV Check targetBound else");
            }
        } else {
            // Debug.Log("FOV Check else");
        }
    }

    public void TargetReached(GameObject target)
    {
        StartCoroutine(Cooldown());
        // Debug.Log("TargetReached");
        visitedTargets[target] = visitedTargets[target] + 1;
        if (target.layer == LayerMask.NameToLayer("Doors")) {
            Vector3 move = transform.forward;
            navMeshAgent.Move(move);
        }
    }
}