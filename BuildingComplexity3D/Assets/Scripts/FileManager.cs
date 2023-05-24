using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;

public class FileManager: MonoBehaviour
{
    public string ID;
    public System.DateTime dateTime = System.DateTime.UtcNow.ToLocalTime();
    private string FolderName;
    private string NodeRecordFile;
    private string DoorRecordFile;
    private string ExitRecordFile;
    private Scene scene;

    private void Awake()
    {
        Debug.Log("Awake:" + SceneManager.GetActiveScene().name);
        scene = SceneManager.GetActiveScene();
        ID = scene.name;
        string dateTimeStr = "Date_" + dateTime.ToString("MM") + "-" + dateTime.ToString("dd") + "-" + dateTime.ToString("yyyy") + "_Time_" + dateTime.ToString("HH") + "-" + dateTime.ToString("mm") + "-" + dateTime.ToString("ss");
        Debug.Log(dateTime);
        // theDate = System.DateTime.Now.ToString("MM-dd");
        FolderName = ID + "_" + dateTimeStr;
        AssetDatabase.CreateFolder("Assets/LogFiles", FolderName);

        NodeRecordFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Nodes.csv");

        TextWriter wt = new StreamWriter(NodeRecordFile, false);
        wt.WriteLine("Agent Name, Node Name, Position-X, Position-Z, Time (Seconds)");
        wt.Close();

        DoorRecordFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Doors.csv");

        wt = new StreamWriter(DoorRecordFile, false);
        wt.WriteLine("Agent Name, Door Name, Position-X, Position-Z, Time (Seconds)");
        wt.Close();

        ExitRecordFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Exits.csv");

        wt = new StreamWriter(ExitRecordFile, false);
        wt.WriteLine("Agent Name, Exit Name, Position-X, Position-Z, Exit Time (Seconds)");
        wt.Close();
    }
    
    public void WriteStringNode(GameObject agent, GameObject node)
    {
        string tempTime = (Mathf.Round(Time.time * 100f) / 100f).ToString();
        string tempWrite = string.Format("{0}, {1}, {2}, {3}, {4}", agent.name, node.name, Mathf.Round(node.transform.position.x * 100f) / 100f, Mathf.Round(node.transform.position.z * 100f) / 100f, tempTime);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(NodeRecordFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(NodeRecordFile);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    public void WriteStringDoor(GameObject agent, GameObject door)
    {
        string tempTime = (Mathf.Round(Time.time * 100f) / 100f).ToString();
        string tempWrite = string.Format("{0}, {1}, {2}, {3}, {4}", agent.name, door.name, Mathf.Round(door.transform.position.x * 100f) / 100f, Mathf.Round(door.transform.position.z * 100f) / 100f, tempTime);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(DoorRecordFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(DoorRecordFile);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    public void WriteStringExit(GameObject agent, GameObject exit)
    {
        string tempTime = (Mathf.Round(Time.time * 100f) / 100f).ToString();
        string tempWrite = string.Format("{0}, {1}, {2}, {3}, {4}", agent.name, exit.name, Mathf.Round(exit.transform.position.x * 100f) / 100f, Mathf.Round(exit.transform.position.z * 100f) / 100f, tempTime);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(ExitRecordFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(ExitRecordFile);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}