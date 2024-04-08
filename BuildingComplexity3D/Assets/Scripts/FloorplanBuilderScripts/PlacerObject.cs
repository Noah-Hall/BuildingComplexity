using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacerObject
{
    public GameObject realObj;
    public string prefabName;
    public Vector3 pos;
    public bool rotate;
    public Vector2Int scale;
    public Vector3Int gridPosition;
    public int ID;
    public PlacementOrientation orientation;
    public StairInfo stairInfo;

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


    public override string ToString()
    {
        return "realObj: " + realObj.ToString() + ", Pos: " + pos.ToString() + ", Rotate: " + rotate + ", Scale: " + scale.ToString() + ", Grid Position: " + gridPosition.ToString() + ", ID: " + ID + ", Orientation:" + orientation.ToString();
    }  
}

[System.Serializable]
public struct StairInfo {
    public int floorNum;
    public int stairwellNum;
    public bool isExitFloor;
}