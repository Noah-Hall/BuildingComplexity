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

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetMask = LayerMask.GetMask("Doors");
        obstructionMask = LayerMask.GetMask("Walls");
        StartCoroutine(FOVRoutine());
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

    private void FieldOfViewCheck()
    {
        Debug.Log("FOV Check");
        doorBound = false;
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length > 0) {
            Debug.Log("FOV Check Length > 0");
            Transform closest = null;
            float closestDistance = float.MaxValue;
            bool exitFound = false;
            foreach (Collider check in rangeChecks) {
                if (!visitedDoors.Contains(check.gameObject)) {
                    Transform door = check.transform;
                    Vector3 direction = (door.position - transform.position).normalized;
                    float distance = Vector3.Distance(door.position, transform.position);
                    if (!Physics.Raycast(transform.position, direction, distance, obstructionMask)) {
                        if (exitFound) {
                            if (check.gameObject.tag == "Exit") {
                                if (distance < closestDistance) {
                                    doorBound = true;
                                    closest = door;
                                    closestDistance = distance;
                                }
                            }
                        } else {
                            if (check.gameObject.tag == "Exit") {
                                Debug.Log("FOV Check Exit Found");
                                doorBound = true;
                                closest = door;
                                closestDistance = distance;
                                exitFound = true;
                                // break;
                            }
                            if (closest == null) {
                                doorBound = true;
                                closest = door;
                                closestDistance = distance;
                            } else if (distance < closestDistance) {
                                doorBound = true;
                                closest = door;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }
            if (doorBound) {
                Debug.Log("FOV Check doorBound");
                navMeshAgent.destination = closest.position;
            }
        } else {
            Debug.Log("FOV Check else");
            //look for door
            SearchForDoor();
        }
    }

    private void SearchForDoor()
    {

    }

    public void DoorReached(GameObject door)
    {
        StartCoroutine(DoorReachedRoutine(door));
    }
}