using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager instance { get; private set; }
    private SceneData sceneData;
    private FileDataHandler dataHandler;
    private string currentFileName;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one PersistenceManager in the scene.");
        }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler();
    }

    public void SetFileName(string fileName)
    {
        currentFileName = fileName;
    }

    public void NewFloorplan(string fileName)
    {
        this.sceneData = new SceneData();

        // set sceneData.fileData
        string theDate = System.DateTime.Now.ToString("MM/dd/yyyy") + "_" + System.DateTime.Now.ToString("hh:mm:ss");
        FileData newFileData = new FileData(fileName, theDate, "N/A");
        sceneData.SetFileData(newFileData);
    }

    public void LoadFloorplan(string fileName)
    {
        this.sceneData = dataHandler.Load(fileName);

        // if no data can be loaded, initialize to a new game
        if (this.sceneData == null)
        {
            Debug.Log("No data was found.");
        } else {
            // set sceneData.fileData
            string theDate = System.DateTime.Now.ToString("MM/dd/yyyy") + "_" + System.DateTime.Now.ToString("hh:mm:ss");
            FileData newFileData = new FileData(fileName, theDate, "N/A");
            sceneData.SetFileData(newFileData);
        }
    }

    public void SaveFloorplan()
    {
        // Add all relevant GameObjects to sceneData.objectsList
        GameObject[] moduleNodes = GameObject.FindGameObjectsWithTag("ModuleNode");
        sceneData.objectsList.AddRange(moduleNodes);
        GameObject[] roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        sceneData.objectsList.AddRange(roomNodes);
        GameObject[] intersectionNodes = GameObject.FindGameObjectsWithTag("IntersectionNode");
        sceneData.objectsList.AddRange(intersectionNodes);
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        sceneData.objectsList.AddRange(doors);
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        sceneData.objectsList.AddRange(exits);
        GameObject[] stairs = GameObject.FindGameObjectsWithTag("Stair");
        sceneData.objectsList.AddRange(stairs);
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        sceneData.objectsList.AddRange(floors);

        // set sceneData.fileData
        string theDate = System.DateTime.Now.ToString("MM/dd/yyyy") + "_" + System.DateTime.Now.ToString("hh:mm:ss");
        FileData newFileData = new FileData(currentFileName, theDate, "N/A");
        sceneData.SetFileData(newFileData);

        // save sceneData as a file
        dataHandler.Save(sceneData);
    }
}
