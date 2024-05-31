using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>SceneData</c> represents the data for a floorplan scene and file.
/// </summary>
[System.Serializable]
public class SceneData
{
    /// <value>
    /// Property <c>objectList</c> holds the <c>List</c> of <c>PlacerObjects</c> in the floorplan.
    /// </value>
    public List<PlacerObject> objectsList;

    /// <value>
    /// Property <c>fileData</c> holds the <c>FileData</c> of the floorplan.
    /// </value>
    public FileData fileData;

    public SceneData()
    {
        this.objectsList = new List<PlacerObject>();
    }

    /// <summary>
    /// Method handles logic for setting <c>fileData</c>.
    /// </summary>
    /// <param name="fileData">The <c>FileData</c> to be set.</param>
    public void SetFileData(FileData fileData)
    {
        this.fileData = fileData;
    }
}
