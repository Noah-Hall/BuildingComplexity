using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class FileManager: MonoBehaviour
{
    public string theTime;
    public string theDate;
    public string ID;
    string DoorRecordFile;
    string ExitRecordFile;
    Scene scene;

    void Awake()
    {
        Debug.Log("Awake:" + SceneManager.GetActiveScene().name);
        scene = SceneManager.GetActiveScene();
        ID = scene.name;
        theTime = System.DateTime.Now.ToString("HH.mm");
        theDate = System.DateTime.Now.ToString("MM-dd");

        DoorRecordFile = Application.dataPath + ("/LogFiles/DoorLogs/" + ID + "_" +
        "AgentData" + "_" + theDate + "_" + theTime + "_doors.csv");

        TextWriter wt = new StreamWriter(DoorRecordFile, false);
        wt.WriteLine("Agent Name, Door Name, Position-X, Position-Z, Time (Seconds)");
        wt.Close();

        ExitRecordFile = Application.dataPath + ("/LogFiles/ExitLogs/" + ID + "_" +
        "AgentData" + "_" + theDate + "_" + theTime + "_exits.csv");

        wt = new StreamWriter(ExitRecordFile, false);
        wt.WriteLine("Agent Name, Exit Name, Position-X, Position-Z, Exit Time (Seconds)");
        wt.Close();
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

    // public void ReadString()
    // {
    //     //Read the text from directly from the test.txt file
    //     StreamReader reader = new StreamReader(RecordFile);
    //     Debug.Log(reader.ReadToEnd());
    //     reader.Close();
    // }

    // theTime = System.DateTime.Now.ToString("HH.mm");
    //     theDate = System.DateTime.Now.ToString("MM-dd");
    //     /*PositionFile = Application.dataPath + ("/" + ID +
    //     "_" + "PositionAndCollision" + "_" + theDate + "_" + theTime +
    //    ".csv");*/
    //     TimerCollisionFile = Application.dataPath + ("/" + ID + "_" +
    //    "TrialTime" + "_" + theDate + "_" + theTime + ".csv");
    //     DeltasFile = Application.dataPath + ("/" + ID + "_" +
    //    "ControllerInputs" + "_" + theDate + "_" + theTime + ".csv");
        
    //     //CollisionTimer.Start();
    //     //Headers and repeated call for Position Mapping
    //     /*InvokeRepeating("WritePosition", 3, 0.5f);
    //     TextWriter tw = new StreamWriter(PositionFile, false);
    //     tw.WriteLine("PositionX,PositionY,PositionZ,Pitch,Yaw,Roll,Time");
    //     tw.Close();*/
    //     //Headers and repeated call for Input Mapping
    //     TextWriter wt = new StreamWriter(DeltasFile, false);
    //     wt.WriteLine("H Change, V Change, NHV Change, Time");
    //     wt.Close();

}

// public void CollisionStopwatchEndAndRecord()
//     {
//         TextWriter tw = new StreamWriter(TimerCollisionFile, true);
//         tw.WriteLine("Collision " + count + " (ms):" + "," +
//        (CollisionTimer.ElapsedMilliseconds - elapsed_time) +
//         "," + "Time stamp(ms)" + "," +
//        CollisionTimer.ElapsedMilliseconds);
//         elapsed_time = CollisionTimer.ElapsedMilliseconds;
//         tw.Close();
//     }
//     public void TotalTime()
//     {
//         TextWriter tw = new StreamWriter(TimerCollisionFile, true);
//         tw.WriteLine("Total Time (ms)" + "," +
//        CollisionTimer.ElapsedMilliseconds);
//         tw.WriteLine("Total Collisions" + "," + count);
//         tw.Close();
//     }
//    /* public void WritePosition()
//     {
//         TextWriter tw = new StreamWriter(PositionFile, true);
//         tw.WriteLine(transform.position.x + "," + transform.position.y + "," +
//         transform.position.z + "," + transform.rotation.x + ","
//         + transform.rotation.y + "," + transform.rotation.z + "," +
//        CollisionTimer.ElapsedMilliseconds);
//         tw.Close();
//     }*/

//     public void ControllerDeltas()
//     {
//         TextWriter tw = new StreamWriter(DeltasFile, true);
//         tw.WriteLine((Input.GetAxis("Horizontal") - InputHorizontal) + "," +
//        (Input.GetAxis("Vertical") - InputVertical) + ","
//         + (Input.GetAxis("NHV") - InputNHV) + "," +
//        CollisionTimer.ElapsedMilliseconds);
//         InputHorizontal = Input.GetAxis("Horizontal");
//         InputVertical = Input.GetAxis("Vertical");
//         InputNHV = Input.GetAxis("NHV");
//         tw.Close();
//     }