using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class <c>ObjectPlacer</c> handles the logic for placing and removing objects in <c>PlacementSystem</c>.
/// </summary>
public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedObjects;
    private List<PlacerObject> placerObjects;

    private void Start () 
    { 
        placedObjects = new List<GameObject>();
        placerObjects = new List<PlacerObject>();
    }

    /// <summary>
    /// Method handles the logic for placing objects in <c>PlacementSystem</c>.
    /// </summary>
    /// <returns>
    /// Number of placed objects as <c>int</c>.
    /// </returns>
    /// <param name="placerObject">The object to be placed</param>
    public int PlaceObject(PlacerObject placerObject)
    {
        if (placerObject == null || placerObject.realObj == null) 
        { 
            Debug.LogError("placerObject or prefab null: " + placerObject.ToString());
            return -1;
        }
        GameObject newObject = Instantiate(placerObject.realObj);
        newObject.transform.position = placerObject.pos;
        if (placerObject.rotate) { newObject.transform.Rotate(0, 90, 0); }

        Vector3 newScale = newObject.transform.localScale;
        newScale.x = placerObject.scale.x > 1 ? placerObject.scale.x : newScale.x;
        newScale.z = placerObject.scale.y > 1 ? placerObject.scale.y : newScale.z;
        newObject.transform.localScale = newScale;
        
        if (placerObject.ID == 5) {
            StairScript stairScript = newObject.GetComponent<StairScript>();
            stairScript._floor = placerObject.stairInfo.floorNum;
            stairScript._stairwell = placerObject.stairInfo.stairwellNum;
            stairScript._isExitFloor = placerObject.stairInfo.isExitFloor;
        }

        if (placerObject.ID == 6) {
            NodeScriptRoom nodeScript = newObject.GetComponent<NodeScriptRoom>();
            nodeScript.weight = placerObject.weight;
        }
        
        placedObjects.Add(newObject);
        placerObjects.Add(placerObject);
        return placedObjects.Count - 1;
    }

    /// <summary>
    /// Method handles the logic for removing objects in <c>PlacementSystem</c>.
    /// </summary>
    /// <param name="index">The index of the object to be removed</param>
    public void RemoveObjectAt(int index)
    {
        if (placedObjects.Count <= index || placedObjects[index] == null) { return; }
        Destroy(placedObjects[index]);
        placedObjects[index] = null;
        placerObjects[index] = null;
    }

    /// <summary>
    /// Method gets every placed <c>PlacerObject</c>.
    /// </summary>
    /// <returns>
    /// <c>List</c> of every placed <c>PlacerObject</c>.
    /// </returns>
    public List<PlacerObject> GetPlacerObjects()
    {
        for(int i = placerObjects.Count - 1; i >= 0; i--)
        {
            if (placerObjects[i] == null) { 
                placerObjects.RemoveAt(i);
            }
        }
        return placerObjects;
    }
}