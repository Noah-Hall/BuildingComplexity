using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [SerializeField] GameObject manager;

    void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    private void OnTriggerEnter(Collider col)
    {
        GameObject agent = col.gameObject;
        if (gameObject.name.Contains("exit")) {
            ExitReached(agent);
        } else if (gameObject.name.Contains("door")) {
            DoorReached(agent);
        } else if (gameObject.name.Contains("node")) {
            NodeReached(agent);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        GameObject agent = col.gameObject;
        if(agent.GetComponent<AgentBehaviorSmart>().reachedCooldown) {
            return;
        }

        if (gameObject.name.Contains("exit")) {
            ExitReached(agent);
        } else if (gameObject.name.Contains("door")) {
            DoorReached(agent);
        } else if (gameObject.name.Contains("node")) {
            NodeReached(agent);
        }
    }

    private void ExitReached(GameObject agent)
    {
        // Debug.Log("Exit Reached");
        // Debug.Log(agent.name + " : " + gameObject.name + " : " + gameObject.transform.position + " : exited at : " + Time.time);
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringExit(agent, gameObject);
        Destroy(agent);
    }

    private void DoorReached(GameObject agent) {
        // Debug.Log("Door Reached");
        // Debug.Log(agent.name + " : " + gameObject.name + " : " + gameObject.transform.position + " : " + Time.time);
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringDoor(agent, gameObject);
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }

    private void NodeReached(GameObject agent) {
        // Debug.Log("Node Reached");
        // Debug.Log(agent.name + " : " + gameObject.name + " : " + gameObject.transform.position + " : " + Time.time);
        FileManager file = manager.GetComponent<FileManager>();
        file.WriteStringNode(agent, gameObject);
        agent.GetComponent<AgentBehaviorSmart>().TargetReached(gameObject);
    }
}