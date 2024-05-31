using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>PlacerObject</c> defines an object that can be placed in <c>PlacementSystem</c>.
/// </summary>
[System.Serializable]
public class PlacerObject
{
    /// <value>
    /// Property <c>realObj</c> holds the prefab <c>GameObject</c>.
    /// </value>
    public GameObject realObj;

    /// <value>
    /// Property <c>prefabName</c> holds the name of <c>realObj</c>.
    /// </value>
    public string prefabName;

    /// <value>
    /// Property <c>pos</c> holds the <c>Vector3</c> position of the object.
    /// </value>
    public Vector3 pos;

    /// <value>
    /// Property <c>rotate</c> holds true when rotated.
    /// </value>
    public bool rotate;

    /// <value>
    /// Property <c>scale</c> holds the <c>Vector2Int</c> scale of the object.
    /// </value>
    public Vector2Int scale;

    /// <value>
    /// Property <c>gridPosition</c> holds the <c>Vector3Int</c> grid position of the object.
    /// </value>
    public Vector3Int gridPosition;

    /// <value>
    /// Property <c>ID</c> holds the object id.
    /// </value>
    public int ID;

    /// <value>
    /// Property <c>orientation</c> holds the <c>PlacementOrientation</c> of the object.
    /// </value>
    public PlacementOrientation orientation;

    /// <value>
    /// Property <c>stairInfo</c> holds the <c>StairInfo</c> of the object (this should only be filled if the object is a stair).
    /// </value>
    public StairInfo stairInfo;

    /// <value>
    /// Property <c>weight</c> holds the <c>int</c> weight of the object (this should only be filled if the object is a room node).
    /// </value>
    public int weight;

    /// <summary>
    /// <c>PlacerObject</c> constructor for most objects.
    /// </summary>
    public PlacerObject(GameObject realObj, Vector3 pos, bool rotate, Vector2Int scale, Vector3Int gridPosition, int ID, PlacementOrientation orientation)
    {
        this.realObj = realObj;
        this.pos = pos;
        this.rotate = rotate;
        this.scale = scale;
        this.gridPosition = gridPosition;
        this.ID = ID;
        this.orientation = orientation;

        this.prefabName = realObj.name;
    }

    /// <summary>
    /// <c>PlacerObject</c> constructor for stairs.
    /// </summary>
    public PlacerObject(GameObject realObj, Vector3 pos, bool rotate, Vector2Int scale, Vector3Int gridPosition, int ID, PlacementOrientation orientation, StairInfo stairInfo)
    {
        this.realObj = realObj;
        this.pos = pos;
        this.rotate = rotate;
        this.scale = scale;
        this.gridPosition = gridPosition;
        this.ID = ID;
        this.orientation = orientation;

        this.prefabName = realObj.name;
        this.stairInfo = stairInfo;
    }

    /// <summary>
    /// <c>PlacerObject</c> constructor for room nodes.
    /// </summary>
    public PlacerObject(GameObject realObj, Vector3 pos, bool rotate, Vector2Int scale, Vector3Int gridPosition, int ID, PlacementOrientation orientation, int weight)
    {
        this.realObj = realObj;
        this.pos = pos;
        this.rotate = rotate;
        this.scale = scale;
        this.gridPosition = gridPosition;
        this.ID = ID;
        this.orientation = orientation;

        this.prefabName = realObj.name;
        this.weight = weight;
    }

    /// <summary>
    /// Method gets relevant data about <c>PlacerObject</c>.
    /// </summary>
    /// <returns>
    /// <c>string</c>
    /// </returns>
    public override string ToString()
    {
        return "realObj: " + realObj.ToString() + ", Pos: " + pos.ToString() + ", Rotate: " + rotate + ", Scale: " + scale.ToString() + ", Grid Position: " + gridPosition.ToString() + ", ID: " + ID + ", Orientation:" + orientation.ToString();
    }  
}

/// <summary>
/// Struct <c>StairInfo</c> holds relevant data about a <c>PlacerObject</c> <c>Stair</c>.
/// </summary>
[System.Serializable]
public struct StairInfo {
    /// <value>
    /// Property <c>floorNum</c> holds the integer value of which floor the <c>Stair</c> object is on.
    /// </value>
    public int floorNum;

    /// <value>
    /// Property <c>stairwellNum</c> holds the integer value of which stairwell the <c>Stair</c> object is connected to.
    /// </value>
    public int stairwellNum;

    /// <value>
    /// Property <c>isExitFloor</c> holds the boolean value of whether the <c>Stair</c> object is on the same floor as an <c>Exit</c> object.
    /// </value>
    public bool isExitFloor;
}