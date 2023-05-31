using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Linq;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*   FileManager handles creating Assers/LogFiles                 *
*   Four files are grouped into a folder                         *
*   - Nodes.csv logs everytime an agent reaches a target Node    *
*   - Doors.csv logs everytime an agent reaches a target Door    *
*   - Exits.csv logs everytime an agent reaches an Exit          *
*   - Results.csv summarizes relevant information from above     *
*     (mainly, lowest, largest, and average values)              *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public class FileManager: MonoBehaviour
{
    public string ID;
    public System.DateTime dateTime = System.DateTime.UtcNow.ToLocalTime();
    private string FolderName;
    private string NodeRecordFile;
    private string DoorRecordFile;
    private string ExitRecordFile;
    private string ResultRecordFile;
    private Scene scene;

    // initializes filepath variables and writes first line of files
    private void Awake()
    {
        // Debug.Log("Awake:" + SceneManager.GetActiveScene().name);
        scene = SceneManager.GetActiveScene();
        ID = scene.name;
        string dateTimeStr = "Date_" + dateTime.ToString("MM") + "-" + dateTime.ToString("dd") + "-" + dateTime.ToString("yyyy") + "_Time_" + dateTime.ToString("HH") + "-" + dateTime.ToString("mm") + "-" + dateTime.ToString("ss");
        // Debug.Log(dateTime);
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

        ResultRecordFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Results.csv");

        wt = new StreamWriter(ResultRecordFile, false);
        wt.WriteLine("Category, Minimun, Maximum, Average");
        wt.Close();
    }
    
    // writes a formatted line to Node.csv
    public void WriteStringNode(GameObject agent, GameObject node)
    {
        string tempTime = (Mathf.Round(Time.time * 100f) / 100f).ToString();
        string tempWrite = string.Format("{0}, {1}, {2}-X, {3}-Z, {4} seconds", agent.name, node.name, Mathf.Round(node.transform.position.x * 100f) / 100f, Mathf.Round(node.transform.position.z * 100f) / 100f, tempTime);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(NodeRecordFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(NodeRecordFile);
        //Print the text from the file
        // Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    // writes a formatted line to Door.csv
    public void WriteStringDoor(GameObject agent, GameObject door)
    {
        string tempTime = (Mathf.Round(Time.time * 100f) / 100f).ToString();
        string tempWrite = string.Format("{0}, {1}, {2}-X, {3}-Z, {4} seconds", agent.name, door.name, Mathf.Round(door.transform.position.x * 100f) / 100f, Mathf.Round(door.transform.position.z * 100f) / 100f, tempTime);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(DoorRecordFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(DoorRecordFile);
        //Print the text from the file
        // Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    // writes a formatted line to Exit.csv
    public void WriteStringExit(GameObject agent, GameObject exit)
    {
        string tempTime = (Mathf.Round(Time.time * 100f) / 100f).ToString();
        string tempWrite = string.Format("{0}, {1}, {2}-X, {3}-Z, {4} seconds", agent.name, exit.name, Mathf.Round(exit.transform.position.x * 100f) / 100f, Mathf.Round(exit.transform.position.z * 100f) / 100f, tempTime);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(ExitRecordFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(ExitRecordFile);
        //Print the text from the file
        // Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    // calculates results and writes all lines to Results.csv
    public void CalculateResults()
    {
        List<string> results = new List<string>();
        List<string> temp = new List<string>();
        Debug.Log("Calculating results");
        results = CalculateNodes();
        temp = CalculateDoors();
        results.AddRange(temp);
        temp = CalculateExits();
        results.AddRange(temp);
        
        StreamWriter writer = new StreamWriter(ResultRecordFile, true);
        foreach(string line in results) {
            writer.WriteLine(line);
        }
        writer.Close();
        Debug.Log("Results Calculated");
    }

    // calculates Node related results 
    private List<string> CalculateNodes()
    {
        List<string> nodeResults = new List<string>();
        try {
            StreamReader sr = new StreamReader(NodeRecordFile);
            // Format: <node name, #agents>
            IDictionary<string, int> nodeDict = new Dictionary<string, int>();
            // Format: <agent name, #nodes>
            IDictionary<string, int> agentDict = new Dictionary<string, int>();
            MinMaxAve<string, string, float> nodeVals = new MinMaxAve<string, string, float>("", "", 0f);
            MinMaxAve<string, string, float> agentVals = new MinMaxAve<string, string, float>("", "", 0f);
            string[] separatingStrings = {", ", "-X, ", "-Z, ", " seconds"};
            string line;

            GameObject[] moduleNodes = GameObject.FindGameObjectsWithTag("ModuleNode");
            GameObject[] roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
            foreach(GameObject node in moduleNodes) {
                nodeDict.Add(node.name, 0);
            }
            foreach(GameObject node in roomNodes) {
                nodeDict.Add(node.name, 0);
            }
            GameObject[] agentList = GameObject.FindGameObjectsWithTag("Smart Agent");
            foreach(GameObject agent in agentList) {
                agentDict.Add(agent.name, 0);
            }

            sr.ReadLine();
            while((line = sr.ReadLine()) != null) {
                string[] segments = line.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                if (!nodeDict.ContainsKey(segments[1])) {
                    nodeDict.Add(segments[1], 1);
                } else {
                    nodeDict[segments[1]]++;
                }
                if (!agentDict.ContainsKey(segments[0])) {
                    agentDict.Add(segments[0], 1);
                } else {
                    agentDict[segments[0]]++;
                }
            }
            sr.Close();

            var leastNode = nodeDict.Aggregate((x, y) => x.Value < y.Value ? x : y);
            var mostNode = nodeDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            foreach(KeyValuePair<string, int> elem in nodeDict) {
                nodeVals.ave += elem.Value;
            }
            nodeVals.min = leastNode.Key + " : " + leastNode.Value;
            nodeVals.max = mostNode.Key + " : " + mostNode.Value;
            nodeVals.ave = nodeVals.ave / nodeDict.Count;

            var leastAgent = agentDict.Aggregate((x, y) => x.Value < y.Value ? x : y);
            var mostAgent = agentDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            foreach(KeyValuePair<string, int> elem in agentDict) {
                agentVals.ave += elem.Value;
            }
            agentVals.min = leastAgent.Key + " : " + leastAgent.Value;
            agentVals.max = mostAgent.Key + " : " + mostAgent.Value;
            agentVals.ave = agentVals.ave / agentDict.Count;

            nodeResults.Add(string.Format("Node Records, {0} agents, {1} agents, {2} agents", nodeVals.min, nodeVals.max, nodeVals.ave.ToString("0.00")));
            nodeResults.Add(string.Format("Node Visits, {0} nodes, {1} nodes, {2} nodes", agentVals.min, agentVals.max, agentVals.ave.ToString("0.00")));
        } catch (Exception e) {
                // Let the user know what went wrong.
                Debug.Log("The Node file could not be read:");
                Debug.Log(e.Message);
        }
        return nodeResults;
    }

    // calculates Door related results 
    private List<string> CalculateDoors()
    {
        List<string> doorResults = new List<string>();
        try {
            StreamReader sr = new StreamReader(DoorRecordFile);
            // Format: <door name, #agents>
            IDictionary<string, int> doorDict = new Dictionary<string, int>();
            // Format: <agent name, #doors>
            IDictionary<string, int> agentDict = new Dictionary<string, int>();
            MinMaxAve<string, string, float> doorVals = new MinMaxAve<string, string, float>("", "", 0f);
            MinMaxAve<string, string, float> agentVals = new MinMaxAve<string, string, float>("", "", 0f);
            string[] separatingStrings = {", ", "-X, ", "-Z, ", " seconds"};
            string line;

            GameObject[] doorList = GameObject.FindGameObjectsWithTag("Door");
            foreach(GameObject door in doorList) {
                doorDict.Add(door.name, 0);
            }
            GameObject[] agentList = GameObject.FindGameObjectsWithTag("Smart Agent");
            foreach(GameObject agent in agentList) {
                agentDict.Add(agent.name, 0);
            }

            sr.ReadLine();
            while((line = sr.ReadLine()) != null) {
                string[] segments = line.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                if (!doorDict.ContainsKey(segments[1])) {
                    doorDict.Add(segments[1], 1);
                } else {
                    doorDict[segments[1]]++;
                }
                if (!agentDict.ContainsKey(segments[0])) {
                    agentDict.Add(segments[0], 1);
                } else {
                    agentDict[segments[0]]++;
                }
            }
            sr.Close();

            var leastDoor = doorDict.Aggregate((x, y) => x.Value < y.Value ? x : y);
            var mostDoor = doorDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            foreach(KeyValuePair<string, int> elem in doorDict) {
                doorVals.ave += elem.Value;
            }
            doorVals.min = leastDoor.Key + " : " + leastDoor.Value;
            doorVals.max = mostDoor.Key + " : " + mostDoor.Value;
            doorVals.ave = doorVals.ave / doorDict.Count;

            var leastAgent = agentDict.Aggregate((x, y) => x.Value < y.Value ? x : y);
            var mostAgent = agentDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            foreach(KeyValuePair<string, int> elem in agentDict) {
                agentVals.ave += elem.Value;
            }
            agentVals.min = leastAgent.Key + " : " + leastAgent.Value;
            agentVals.max = mostAgent.Key + " : " + mostAgent.Value;
            agentVals.ave = agentVals.ave / agentDict.Count;

            doorResults.Add(string.Format("Door Records, {0} agents, {1} agents, {2} agents", doorVals.min, doorVals.max, doorVals.ave.ToString("0.00")));
            doorResults.Add(string.Format("Door Visits, {0} doors, {1} doors, {2} doors", agentVals.min, agentVals.max, agentVals.ave.ToString("0.00")));
        } catch (Exception e) {
                // Let the user know what went wrong.
                Debug.Log("The Door file could not be read:");
                Debug.Log(e.Message);
        }
        return doorResults;
    }

    // calculates Exit related results 
    private List<string> CalculateExits()
    {
        List<string> exitResults = new List<string>();
        try {
            StreamReader sr = new StreamReader(ExitRecordFile);
            // Format: <exit name, #agents>
            IDictionary<string, int> exitDict = new Dictionary<string, int>();
            MinMaxAve<float, float, float> timeVals = new MinMaxAve<float, float, float>(float.MaxValue, 0f, 0f);
            string[] separatingStrings = {", ", "-X, ", "-Z, ", " seconds"};
            string line;
            float count = 0f;

            GameObject[] exitList = GameObject.FindGameObjectsWithTag("Exit");
            foreach(GameObject exit in exitList) {
                exitDict.Add(exit.name, 0);
            }

            sr.ReadLine();
            while((line = sr.ReadLine()) != null) {
                string[] segments = line.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                if (!exitDict.ContainsKey(segments[1])) {
                    exitDict.Add(segments[1], 1);
                } else {
                    exitDict[segments[1]]++;
                }
                float temp = float.Parse(segments[4]);
                if (timeVals.min > temp) { timeVals.min = temp; }
                if (timeVals.max < temp) { timeVals.max = temp; }
                timeVals.ave += temp;
                count++;
            }
            sr.Close();
            timeVals.ave = timeVals.ave / count;

            MinMaxAve<string, string, float> exitedVals = new MinMaxAve<string, string, float>("", "", 0f);
            var mostExited = exitDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            var leastExited = exitDict.Aggregate((x, y) => x.Value < y.Value ? x : y);

            foreach(KeyValuePair<string, int> elem in exitDict) {
                exitedVals.ave += elem.Value;
            }
            exitedVals.min = leastExited.Key + " : " + leastExited.Value;
            exitedVals.max = mostExited.Key + " : " + mostExited.Value;
            exitedVals.ave = exitedVals.ave / exitDict.Count;

            exitResults.Add(string.Format("Exit Records, {0} agents, {1} agents, {2} agents", exitedVals.min, exitedVals.max, exitedVals.ave.ToString("0.00")));
            exitResults.Add(string.Format("Exit Time, {0} seconds, {1} seconds, {2} seconds", timeVals.min.ToString("0.00"), timeVals.max.ToString("0.00"), timeVals.ave.ToString("0.00")));
        } catch (Exception e) {
            // Let the user know what went wrong.
            Debug.Log("The Exit file could not be read:");
            Debug.Log(e.Message);
        }
        return exitResults;
    }
}

// Structure to hold the minimum, maximum, and average value
public struct MinMaxAve<T, U, V>
{
    public T min;
    public U max;
    public V ave;

    public MinMaxAve(T min, U max, V ave) 
    {
        this.min = min;
        this.max = max;
        this.ave = ave;
    }
}