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
    private List<GameObject> visitedDoors = new List<GameObject>();
    public bool doorBound;
    public bool isWandering;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetMask = LayerMask.GetMask("Doors");
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

    private IEnumerator DoorReachedRoutine(GameObject door)
    {
        yield return new WaitForSeconds(0.1f);
        visitedDoors.Add(door);
        Vector3 move = transform.forward;
        navMeshAgent.Move(move);
    }

    private IEnumerator SearchForDoor()
    {
        isWandering = true;
        yield return new WaitForSeconds(0.2f);
        Vector3 dest = RandomNavSphere(transform.position, 50f, LayerMask.NameToLayer("Floors"));
        navMeshAgent.destination = dest;
    }

    private void FieldOfViewCheck()
    {
        Debug.Log("FOV Check");
        doorBound = false;
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length > 0) {
            Debug.Log("FOV Check Length > 0");
            Transform closest = null;
            float closestDistance = float.MaxValue;
            bool exitFound = false, closestVisited = false;
            foreach (Collider check in rangeChecks) {
                Transform door = check.transform;
                Vector3 direction = (door.position - transform.position).normalized;
                float distance = Vector3.Distance(door.position, transform.position);
                if (!Physics.Raycast(transform.position, direction, distance, obstructionMask)) {
                    if (exitFound) {
                        if (check.gameObject.tag == "Exit" && distance < closestDistance) {
                            doorBound = true;
                            closest = door;
                            closestDistance = distance;
                        }
                    } else {
                        if (check.gameObject.tag == "Exit") {
                            Debug.Log("FOV Check Exit Found");
                            doorBound = true;
                            closest = door;
                            closestDistance = distance;
                            exitFound = true;
                        } else if (closest == null) {
                            doorBound = true;
                            closest = door;
                            closestDistance = distance;
                            if (visitedDoors.Contains(check.gameObject)) {
                                closestVisited = true;
                            }
                        } else if (distance < closestDistance) {
                            if (!visitedDoors.Contains(check.gameObject)) {
                                doorBound = true;
                                closest = door;
                                closestDistance = distance;
                                closestVisited = false;
                            } else if (closestVisited) {
                                doorBound = true;
                                closest = door;
                                closestDistance = distance;
                            }
                        } else if (!visitedDoors.Contains(check.gameObject) && closestVisited) {
                            doorBound = true;
                            closest = door;
                            closestDistance = distance;
                            closestVisited = false;
                        }
                    }
                }
            }
            if (doorBound) {
                Debug.Log("FOV Check doorBound");
                navMeshAgent.destination = closest.position;
            } else {
                Debug.Log("FOV Check else");
                //look for door
                if(!isWandering) {
                    StartCoroutine(SearchForDoor());
                }
            }
        } else {
            Debug.Log("FOV Check else");
            //look for door
            if(!isWandering) {
                StartCoroutine(SearchForDoor());
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask) 
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    public void DoorReached(GameObject door)
    {
        StartCoroutine(DoorReachedRoutine(door));
    }
}