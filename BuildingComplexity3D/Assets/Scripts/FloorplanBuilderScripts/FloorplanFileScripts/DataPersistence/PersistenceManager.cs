using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField]
    private List<GameObject> prefabs; 
    // border, floor, wall, door, exit, Stair, node room, node module, node intersection

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
            SetupFloorplanForEditor();
        }
        if (SceneManager.GetActiveScene().name == "Floorplan_Runner")
        {
            SetupFloorplanForRunner();
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

    private void SetupFloorplanForEditor()
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

    private void SetupFloorplanForRunner()
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

            foreach(PlacerObject placerObject in sceneData.objectsList)
            {
                if (placerObject == null) { continue; }
                GameObject prefabToInstantiate = getPrefab(placerObject.prefabName);
                if (prefabToInstantiate == null) { continue; }
                GameObject newObject = Instantiate(prefabToInstantiate);
                newObject.transform.position = placerObject.pos;
                if (placerObject.rotate) { newObject.transform.Rotate(0, 90, 0); }

                Vector3 newScale = newObject.transform.localScale;
                newScale.x = placerObject.scale.x > 1 ? placerObject.scale.x : newScale.x;
                newScale.z = placerObject.scale.y > 1 ? placerObject.scale.y : newScale.z;
                newObject.transform.localScale = newScale;
                newObject.transform.SetParent(GameObject.Find("Floorplan").transform);

                if (placerObject.ID == 5) {
                    StairScript stairScript = newObject.GetComponent<StairScript>();
                    stairScript._floor = placerObject.stairInfo.floorNum;
                    stairScript._stairwell = placerObject.stairInfo.stairwellNum;
                    stairScript._isExitFloor = placerObject.stairInfo.isExitFloor;
                }
            }
        }
    }

    private GameObject getPrefab(string objectName)
    {
        GameObject to_return = null;
        switch(objectName) {
            case var value when value.Contains("border"):
                to_return = prefabs[0];
                break;
            case var value when value.Contains("floor"):
                to_return = prefabs[1];
                break;
            case var value when value.Contains("wall"):
                to_return = prefabs[2];
                break;
            case var value when value.Contains("door"):
                to_return = prefabs[3];
                break;
            case var value when value.Contains("exit"):
                to_return = prefabs[4];
                break;
            case var value when value.Contains("stair"):
                to_return = prefabs[5];
                break;
            case var value when value.Contains("node room"):
                to_return = prefabs[6];
                break;
            case var value when value.Contains("node module"):
                to_return = prefabs[7];
                break;
            case var value when value.Contains("node intersection"):
                to_return = prefabs[8];
                break;
            // default:
            //     to_return = null;
        }
        return to_return;
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
