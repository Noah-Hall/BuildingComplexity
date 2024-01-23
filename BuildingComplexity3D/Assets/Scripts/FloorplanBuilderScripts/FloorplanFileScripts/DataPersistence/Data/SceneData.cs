using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneData
{
    public List<GameObject> objectsList;
    public FileData fileData;
    public SceneData()
    {
        this.objectsList = new List<GameObject>();
    }

    public void SetFileData(FileData fileData)
    {
        this.fileData = fileData;
    }
}
