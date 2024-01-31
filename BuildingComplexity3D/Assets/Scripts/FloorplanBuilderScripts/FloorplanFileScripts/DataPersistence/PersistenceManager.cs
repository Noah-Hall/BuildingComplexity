using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager instance { get; private set; }
    private SceneData sceneData;
    private FileDataHandler dataHandler;
    private string currentFileName;
    [SerializeField]
    private InputField inputField;

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
        if (SceneManager.GetActiveScene().name == "Floorplan_Editor")
        {
            SetupFloorplan();
        }
    }

    public void SetFileName(string filename)
    {
        currentFileName = filename;
    }

    public void NewFloorplan(string filename)
    {
        this.sceneData = new SceneData();

        // set sceneData.fileData
        string theDate = System.DateTime.Now.ToString("MM/dd/yyyy") + "_" + System.DateTime.Now.ToString("hh:mm:ss");
        FileData newFileData = new FileData(filename, theDate, "N/A");
        sceneData.SetFileData(newFileData);
    }

    public void LoadFloorplan()
    {
        Text filename = GameObject.Find("FilenameText").GetComponent<Text>();
        dataHandler.Load(filename.text);
    }

    private void SetupFloorplan()
    {
        this.sceneData = dataHandler.UnpackSceneToLoad();

        // if no data can be loaded, initialize to a new game
        if (this.sceneData == null)
        {
            Debug.Log("No data was found.");
        } else {
            // set sceneData.fileData
            string theDate = System.DateTime.Now.ToString("MM/dd/yyyy") + "_" + System.DateTime.Now.ToString("hh:mm:ss");
            FileData newFileData = new FileData(sceneData.fileData._name, theDate, sceneData.fileData._size);
            sceneData.SetFileData(newFileData);
            GameObject.Find("PlacementSystem").GetComponent<PlacementSystem>().LoadFromSceneData(sceneData);
        }
    }

    public void SaveFloorplan()
    {
        SceneData oldSceneData = dataHandler.GetCurrentSceneData();

        // Set list of PlacerObjects from ObjectPlacer GameObject in Floorplan_Editor
        List<PlacerObject> placerObjects = GameObject.Find("ObjectPlacer").GetComponent<ObjectPlacer>().GetPlacerObjects();

        // Add all relevant GameObjects to sceneData.objectsList
        // PROBLEM NullReferenceException: Object reference not set to an instance of an object
        sceneData.objectsList = placerObjects;

        // set sceneData.fileData
        string theDate = System.DateTime.Now.ToString("MM/dd/yyyy") + "_" + System.DateTime.Now.ToString("hh:mm:ss");
        FileData newFileData = new FileData(oldSceneData.fileData._name, theDate, oldSceneData.fileData._size);
        sceneData.SetFileData(newFileData);

        // save sceneData as a file
        dataHandler.Save(sceneData);
    }

    public void CreateFloorplan()
    {
        dataHandler.Create(inputField.text);
    }
}
