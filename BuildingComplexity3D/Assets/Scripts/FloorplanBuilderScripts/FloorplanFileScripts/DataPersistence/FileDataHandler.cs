using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>FileDataHandler</c> handles logic for using floorplan files.
/// </summary>
public class FileDataHandler
{
    private string pathSceneToLoad = Application.dataPath + "/Scene_Files/Floorplan_Stasher/Scene_To_Load.mngr";
    private string pathCurrentScene = Application.dataPath + "/Scene_Files/Floorplan_Stasher/Current_Scene.mngr";
    private string pathBuilds = Application.dataPath + "/Scene_Files/Floorplan_Builds/";

    /// <summary>
    /// Method handles logic for loading files to either <c>Floorplan_Runner</c> or <c>Floorplan_Editor</c> scene.
    /// </summary>
    /// <param name="filename">The file name <c>string</c></param>
    public void Load(string filename)
    {
        string fullpath = Path.Combine(pathBuilds, filename);
        if (File.Exists(fullpath))
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fullpath);
                fileInfo.CopyTo(pathSceneToLoad, true);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullpath + "\n" + e);
            }
        }
        if (SceneManager.GetActiveScene().name == "Build_Selector") {
            SceneManager.LoadScene("Floorplan_Editor");
        }
        if (SceneManager.GetActiveScene().name == "Run_Selector") {
            SceneManager.LoadScene("Floorplan_Runner");
        }
    }

    /// <summary>
    /// Method gets <c>SceneData</c> from loaded file.
    /// </summary>
    /// <returns>
    /// <c>SceneData</c>
    /// </returns>
    public SceneData UnpackSceneToLoad()
    {
        string fullpath = pathSceneToLoad;
        FileInfo fileToCopy = new FileInfo(fullpath);
        fileToCopy.CopyTo(pathCurrentScene, true);
        SceneData loadedData = null;
        if (File.Exists(fullpath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<SceneData>(dataToLoad);
                File.WriteAllText(pathSceneToLoad, "");
            }
            catch (Exception e)
            {
                File.WriteAllText(pathSceneToLoad, "");
                Debug.LogError("Error occured when trying to load data from file: " + fullpath + "\n" + e);
            }
        }
        return loadedData;
    }

    /// <summary>
    /// Method handles logic for saving files.
    /// </summary>
    /// <param name="sceneData">The <c>SceneData</c> to save in the file.</param>
    public void Save(SceneData sceneData)
    {
        string fullPath = Path.Combine(pathBuilds, sceneData.fileData._name);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(sceneData, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    /// <summary>
    /// Method handles logic for creating files.
    /// </summary>
    /// <param name="filename">The file name <c>string</c></param>
    public void Create(string filename)
    {
        string fullpath = pathBuilds + filename;
        File.WriteAllText(fullpath, "");
        FileInfo file = new FileInfo(fullpath);
        FileData fileData = new FileData(file.Name, file.LastWriteTime.ToString(), file.Length.ToString());
        SceneData newSceneData = new SceneData();
        newSceneData.SetFileData(fileData);
        Save(newSceneData);
        Load(filename);
    }

    /// <summary>
    /// Method handles logic for exiting without saving.
    /// </summary>
    public void Exit()
    {
        File.WriteAllText(pathSceneToLoad, "");
        File.WriteAllText(pathCurrentScene, "");
    }

    /// <summary>
    /// Method gets current <c>SceneData</c>.
    /// </summary>
    /// <returns>
    /// <c>SceneData</c>
    /// </returns>
    public SceneData GetCurrentSceneData()
    {
        SceneData to_return = null;
        try
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(pathCurrentScene, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
        
            to_return = JsonUtility.FromJson<SceneData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to load data from file: " + pathCurrentScene + "\n" + e);
        }
        return to_return;
    }
}
