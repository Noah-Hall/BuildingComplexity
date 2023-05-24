using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBehaviorSmart : MonoBehaviour
{
    // [SerializeField] private Transform movePositionTransform;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float radius = 50;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;
    private List<GameObject> visitedTargets = new List<GameObject>();
    public bool targetBound;
    public bool isWandering;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetMask = LayerMask.GetMask("Exits", "Doors", "Nodes");
        obstructionMask = LayerMask.GetMask("Walls");
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        if (transform.position == navMeshAgent.destination) {
            isWandering = false;
        }
    }

    private IEnumerator FOVRoutine() 
    {
        while(true) {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Debug.Log("FOV Check");
        targetBound = false;
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length > 0) {
            Debug.Log("FOV Check Length > 0");
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (Collider check in rangeChecks) {
                Vector3 target = check.transform.position;
                Vector3 direction = (target - transform.position).normalized;
                float distance = Vector3.Distance(target, transform.position);
                // Debug.Log(check.gameObject.name);

                if (!Physics.Raycast(transform.position, direction, distance, obstructionMask)) {
                    if (closest == null) { 
                        closest = check.gameObject;
                        targetBound = true;
                        closestDistance = distance;
                        // Debug.Log("Layer: " + closest.layer);
                    }
                    switch(check.gameObject.layer) {
                        case var value when value == LayerMask.NameToLayer("Exits"):
                            // Debug.Log("FOV Check Exit Found");
                            if (closest.layer != LayerMask.NameToLayer("Exits") || distance < closestDistance) {
                                targetBound = true;
                                closest = check.gameObject;
                                closestDistance = distance;
                            }
                            break;
                        case var value when value == LayerMask.NameToLayer("Doors"):
                            // Debug.Log("FOV Check Door Found");
                            switch (closest.layer) {
                                case var val when val == LayerMask.NameToLayer("Exits"):
                                    break;
                                case var val when val == LayerMask.NameToLayer("Doors"):
                                    if (distance < closestDistance) {
                                        if (!visitedTargets.Contains(check.gameObject)) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        } else if (visitedTargets.Contains(closest)) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        }
                                    } else if (!visitedTargets.Contains(check.gameObject) && visitedTargets.Contains(closest)) {
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
                                    Debug.Log("Default Reached");
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
                                        if (!visitedTargets.Contains(check.gameObject)) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        } else if (visitedTargets.Contains(closest)) {
                                            targetBound = true;
                                            closest = check.gameObject;
                                            closestDistance = distance;
                                        }
                                    } else if (!visitedTargets.Contains(check.gameObject) && visitedTargets.Contains(closest)) {
                                        targetBound = true;
                                        closest = check.gameObject;
                                        closestDistance = distance;
                                    }
                                    break;
                                default:
                                    Debug.Log("Default Reached");
                                    break;
                            }
                            break;
                        default:
                            Debug.Log("Default Reached");
                            break;
                    }
                }
            }

            if (targetBound) {
                Debug.Log("FOV Check targetBound");
                navMeshAgent.destination = closest.transform.position;
            } else {
                Debug.Log("FOV Check targetBound else");
            }
        } else {
            Debug.Log("FOV Check else");
        }
    }

    public void TargetReached(GameObject door)
    {
        Debug.Log("TargetReached");
        visitedTargets.Add(door);
        Vector3 move = transform.forward;
        navMeshAgent.Move(move);
    }
}