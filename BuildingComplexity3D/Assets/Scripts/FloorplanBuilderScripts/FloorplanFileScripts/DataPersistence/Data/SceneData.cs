using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneData
{
    public List<PlacerObject> objectsList;
    public FileData fileData;
    public SceneData()
    {
        this.objectsList = new List<PlacerObject>();
    }

    public void SetFileData(FileData fileData)
    {
        this.fileData = fileData;
    }
}
