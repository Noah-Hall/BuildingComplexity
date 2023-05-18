using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] GameObject manager;
    void Awake()
    {
        manager = GameObject.Find("Manager");
    }

    private void OnTriggerEnter(Collider col)
    {
        if (gameObject.name.Contains("exit")) {
            Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + gameObject.transform.position + " : exited at : " + Time.time);
            GameObject agent = col.gameObject;
            FileManager file = manager.GetComponent<FileManager>();
            file.WriteStringExit(agent, gameObject);
            Destroy(agent);
        } else {
            Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + gameObject.transform.position + " : " + Time.time);
            GameObject agent = col.gameObject;
            FileManager file = manager.GetComponent<FileManager>();
            file.WriteStringDoor(agent, gameObject);
            agent.GetComponent<AgentBehaviorSmart>().DoorReached(gameObject);
        }
    }
}