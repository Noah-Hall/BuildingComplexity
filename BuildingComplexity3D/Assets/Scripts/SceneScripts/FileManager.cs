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
*   - Floorplan_Specs.txt generates general specs                *
*     about the floorplan                                        *
*   - Logfile.csv logs everytime an Agent reaches a target       *
*   - Results.csc lists all objects                              *
*     (with first, last, and total # of visits)                  *
*   - Summary.csv summarizes relevant information from above     *
*     (mainly, lowest, largest, and average values)              *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
public class FileManager: MonoBehaviour
{
    public string ID;
    public System.DateTime dateTime = System.DateTime.UtcNow.ToLocalTime();
    private string FolderName;
    private string LogFile;
    private string SummaryRecordFile;
    private string ResultsFile;
    private string FloorplanFile;
    private Scene scene;
    List<TargetInfo> nodeResultsList = new List<TargetInfo>();
    List<TargetInfo> doorResultsList = new List<TargetInfo>();
    List<TargetInfo> exitResultsList = new List<TargetInfo>();

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

        LogFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Logfile.csv");

        TextWriter wt = new StreamWriter(LogFile, false);
        wt.WriteLine("Object Name, Agent Name, Position-X, Position-Z, Time (Seconds)");
        wt.Close();

        SummaryRecordFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Summary.csv");

        wt = new StreamWriter(SummaryRecordFile, false);
        wt.WriteLine("Category, Minimun, Min Count, Maximum, Max Count, Average");
        wt.Close();

        ResultsFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Results.csv");

        wt = new StreamWriter(ResultsFile, false);
        wt.WriteLine("Object Name, First Visit, Last Visit, Total # of Visits");
        wt.Close();

        FloorplanFile = Application.dataPath + ("/LogFiles/" + FolderName + "/Floorplan_Specs.txt");

        SaveFloorplan();
    }

    private void Start()
    {
        GameObject[] moduleNodes = GameObject.FindGameObjectsWithTag("ModuleNode");
        foreach(GameObject obj in moduleNodes) {
            nodeResultsList.Add(new TargetInfo(obj.name));
        }
        GameObject[] roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        foreach(GameObject obj in roomNodes) {
            nodeResultsList.Add(new TargetInfo(obj.name));
        }

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach(GameObject obj in doors) {
            doorResultsList.Add(new TargetInfo(obj.name));
        }

        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        foreach(GameObject obj in exits) {
            exitResultsList.Add(new TargetInfo(obj.name));
        }
    }

    public void SaveFloorplan() {
        ScreenCapture.CaptureScreenshot(Application.dataPath + ("/LogFiles/" + FolderName + "/_Floorplan.png"));
    }

    public void GenerateFloorplanData(int nodes, int doors, int exits, int stairwells, int floors) {
        TextWriter wt = new StreamWriter(FloorplanFile, false);
        wt.WriteLine("Number of Nodes: " + nodes);
        wt.WriteLine("Number of Doors: " + doors);
        wt.WriteLine("Number of Exits: " + exits);
        wt.WriteLine("Number of Stairwells: " + stairwells);
        wt.WriteLine("Number of Floors: " + floors);
        wt.Close();
    }
    
    // writes a formatted line to LogFile.csv
    public void WriteStringLogFile(GameObject agent, GameObject obj)
    {
        float tempTime = (Mathf.Round(Time.time * 100f) / 100f);
        string tempTimeS = tempTime.ToString("0.00");
        string tempX = (Mathf.Round(obj.transform.position.x * 100f) / 100f).ToString("0.00");
        string tempZ = (Mathf.Round(obj.transform.position.z * 100f) / 100f).ToString("0.00");
        string tempWrite = string.Format("{0}, {1}, {2:F2}, {3:F2}, {4}", obj.name, agent.name, tempX, tempZ, tempTimeS);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(LogFile, true);
        writer.WriteLine(tempWrite);
        writer.Close();
        StreamReader reader = new StreamReader(LogFile);
        //Print the text from the file
        // Debug.Log(reader.ReadToEnd());
        reader.Close();
        if (obj.tag == "ModuleNode" || obj.tag == "RoomNode") {
            TargetInfo tempNode = new TargetInfo("");
            int index = -1;
            for (int i = 0; i < nodeResultsList.Count(); i ++) {
                if (nodeResultsList[i].name == obj.name) { 
                    tempNode = nodeResultsList[i];
                    index = i;
                    break;
                }
            }

            if (!tempNode.firstSet) {
                tempNode.first = tempTime;
                tempNode.firstSet = true;
                tempNode.last = tempTime;
                tempNode.lastSet = true;
                tempNode.count++;
            } else {
                tempNode.last = tempTime;
                tempNode.lastSet = true;
                tempNode.count++;
            }

            nodeResultsList[index] = tempNode;
        }

        if (obj.tag == "Door") {
            TargetInfo tempNode = new TargetInfo("");
            int index = -1;
            for (int i = 0; i < doorResultsList.Count(); i ++) {
                if (doorResultsList[i].name == obj.name) { 
                    tempNode = doorResultsList[i];
                    index = i;
                    break;
                }
            }

            if (!tempNode.firstSet) {
                tempNode.first = tempTime;
                tempNode.firstSet = true;
                tempNode.last = tempTime;
                tempNode.lastSet = true;
                tempNode.count++;
            } else {
                tempNode.last = tempTime;
                tempNode.lastSet = true;
                tempNode.count++;
            }

            doorResultsList[index] = tempNode;
        }

        if (obj.tag == "Exit") {
            TargetInfo tempNode = new TargetInfo("");
            int index = -1;
            for (int i = 0; i < exitResultsList.Count(); i ++) {
                if (exitResultsList[i].name == obj.name) { 
                    tempNode = exitResultsList[i];
                    index = i;
                    break;
                }
            }

            if (!tempNode.firstSet) {
                tempNode.first = tempTime;
                tempNode.firstSet = true;
                tempNode.last = tempTime;
                tempNode.lastSet = true;
                tempNode.count++;
            } else {
                tempNode.last = tempTime;
                tempNode.lastSet = true;
                tempNode.count++;
            }

            exitResultsList[index] = tempNode;
        }
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
        GenerateResultsFile();
        
        StreamWriter writer = new StreamWriter(SummaryRecordFile, true);
        foreach(string line in results) {
            writer.WriteLine(line);
        }
        writer.Close();
        Debug.Log("Summary Calculated");
    }

    private void GenerateResultsFile()
    {
        StreamWriter writer = new StreamWriter(ResultsFile, true);

        foreach(TargetInfo ti in nodeResultsList) {
            string tempWrite = string.Format("{0}, {1:F2}, {2:F2}, {3}", ti.name, ti.first, ti.last, ti.count);
            writer.WriteLine(tempWrite);
        }
        foreach(TargetInfo ti in doorResultsList) {
            string tempWrite = string.Format("{0}, {1:F2}, {2:F2}, {3}", ti.name, ti.first, ti.last, ti.count);
            writer.WriteLine(tempWrite);
        }
        foreach(TargetInfo ti in exitResultsList) {
            string tempWrite = string.Format("{0}, {1:F2}, {2:F2}, {3}", ti.name, ti.first, ti.last, ti.count);
            writer.WriteLine(tempWrite);
        }

        writer.Close();
        Debug.Log("Results Calculated");
    }

    // calculates Node related results 
    private List<string> CalculateNodes()
    {
        List<string> nodeResults = new List<string>();
        try {
            StreamReader sr = new StreamReader(LogFile);
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
                if (node.activeInHierarchy) {
                    nodeDict.Add(node.name, 0);
                }
            }
            foreach(GameObject node in roomNodes) {
                if (node.activeInHierarchy) {
                    nodeDict.Add(node.name, 0);
                }
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
                if (elem.Value == leastNode.Value) {
                    nodeVals.minCount++;
                }
                if (elem.Value == mostNode.Value) {
                    nodeVals.maxCount++;
                }
            }
            nodeVals.min = leastNode.Key + " : " + leastNode.Value;
            nodeVals.max = mostNode.Key + " : " + mostNode.Value;
            nodeVals.ave = nodeVals.ave / nodeDict.Count;

            var leastAgent = agentDict.Aggregate((x, y) => x.Value < y.Value ? x : y);
            var mostAgent = agentDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            foreach(KeyValuePair<string, int> elem in agentDict) {
                agentVals.ave += elem.Value;
                if (elem.Value == leastAgent.Value) {
                    agentVals.minCount++;
                }
                if (elem.Value == mostAgent.Value) {
                    agentVals.maxCount++;
                }
            }
            agentVals.min = leastAgent.Key + " : " + leastAgent.Value;
            agentVals.max = mostAgent.Key + " : " + mostAgent.Value;
            agentVals.ave = agentVals.ave / agentDict.Count;

            nodeResults.Add(string.Format("Node Records, {0:F2} agents, {1} nodes, {2:F2} agents, {3} nodes, {4:F2} agents", nodeVals.min, nodeVals.minCount, nodeVals.max, nodeVals.maxCount, nodeVals.ave.ToString("0.00")));
            nodeResults.Add(string.Format("Node Visits, {0:F2} nodes, {1} agents, {2:F2} nodes, {3} agents, {4:F2} nodes", agentVals.min, agentVals.minCount, agentVals.max, agentVals.maxCount, agentVals.ave.ToString("0.00")));
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
            StreamReader sr = new StreamReader(LogFile);
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
                if (door.activeInHierarchy) {
                    doorDict.Add(door.name, 0);
                }
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
                if (elem.Value == leastDoor.Value) {
                    doorVals.minCount++;
                }
                if (elem.Value == mostDoor.Value) {
                    doorVals.maxCount++;
                }
            }
            doorVals.min = leastDoor.Key + " : " + leastDoor.Value;
            doorVals.max = mostDoor.Key + " : " + mostDoor.Value;
            doorVals.ave = doorVals.ave / doorDict.Count;

            var leastAgent = agentDict.Aggregate((x, y) => x.Value < y.Value ? x : y);
            var mostAgent = agentDict.Aggregate((x, y) => x.Value > y.Value ? x : y);
            foreach(KeyValuePair<string, int> elem in agentDict) {
                agentVals.ave += elem.Value;
                if (elem.Value == leastAgent.Value) {
                    agentVals.minCount++;
                }
                if (elem.Value == mostAgent.Value) {
                    agentVals.maxCount++;
                }
            }
            agentVals.min = leastAgent.Key + " : " + leastAgent.Value;
            agentVals.max = mostAgent.Key + " : " + mostAgent.Value;
            agentVals.ave = agentVals.ave / agentDict.Count;

            doorResults.Add(string.Format("Door Records, {0} agents, {1} doors, {2} agents, {3} doors, {4} agents", doorVals.min, doorVals.minCount, doorVals.max, doorVals.maxCount, doorVals.ave.ToString("0.00")));
            doorResults.Add(string.Format("Door Visits, {0} doors, {1} agents, {2} doors, {3} agents, {4} doors", agentVals.min, agentVals.minCount, agentVals.max, agentVals.maxCount, agentVals.ave.ToString("0.00")));
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
            StreamReader sr = new StreamReader(LogFile);
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
                if (timeVals.min > temp) { 
                    timeVals.min = temp; 
                    timeVals.minCount = 1;
                }
                if (timeVals.max < temp) { 
                    timeVals.max = temp;
                    timeVals.maxCount = 1;
                }
                if (timeVals.min == temp) { timeVals.minCount++; }
                if (timeVals.max == temp) { timeVals.maxCount++; }
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
                if (elem.Value == leastExited.Value) {
                    exitedVals.minCount++;
                }
                if (elem.Value == mostExited.Value) {
                    exitedVals.maxCount++;
                }
            }
            exitedVals.min = leastExited.Key + " : " + leastExited.Value;
            exitedVals.max = mostExited.Key + " : " + mostExited.Value;
            exitedVals.ave = exitedVals.ave / exitDict.Count;

            exitResults.Add(string.Format("Exit Records, {0} agents, {1} exits, {2} agents, {3} exits, {4} agents", exitedVals.min, exitedVals.minCount, exitedVals.max, exitedVals.maxCount, exitedVals.ave.ToString("0.00")));
            exitResults.Add(string.Format("Exit Time, {0} seconds, {1}, {2} seconds, {3}, {4} seconds", timeVals.min.ToString("0.00"), timeVals.minCount, timeVals.max.ToString("0.00"), timeVals.maxCount, timeVals.ave.ToString("0.00")));
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
    public int minCount;
    public int maxCount;

    public MinMaxAve(T min, U max, V ave) 
    {
        this.min = min;
        this.max = max;
        this.ave = ave;
        minCount = 0;
        maxCount = 0;
    }
}

public struct TargetInfo
{
    public string name;
    public float first;
    public float last;
    public int count;
    public bool firstSet;
    public bool lastSet;

    // public TargetInfo()
    // {
    //     name = "";
    //     first = last = 0f;
    //     count = 0;
    //     firstSet = lastSet = false;
    // }
    public TargetInfo(string name) 
    {
        this.name = name;
        first = last = 0f;
        count = 0;
        firstSet = lastSet = false;
    }
    public TargetInfo(string name, float first)
    {
        this.name = name;
        this.first = first;
        last = 0f;
        count = 0;
        firstSet = true;
        lastSet = false;
    }
    public TargetInfo(string name, float first, float last, int count)
    {
        this.name = name;
        this.first = first;
        this.last = last;
        this.count = count;
        firstSet = lastSet = true;
    }
}