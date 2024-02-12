using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedObjects;
    private List<PlacerObject> placerObjects;

    private void Start () 
    { 
        placedObjects = new List<GameObject>();
        placerObjects = new List<PlacerObject>();
    }

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
        
        placedObjects.Add(newObject);
        placerObjects.Add(placerObject);
        return placedObjects.Count - 1;
    }

    public void RemoveObjectAt(int index)
    {
        if (placedObjects.Count <= index || placedObjects[index] == null) { return; }
        Destroy(placedObjects[index]);
        placedObjects[index] = null;
        placerObjects[index] = null;
    }

    public List<PlacerObject> GetPlacerObjects()
    {
        for(int i = 0; i < placerObjects.Count; i++)
        {
            if (placerObjects[i] != null) { continue; }
            placerObjects.RemoveAt(i);
        }
        return placerObjects;
    }
}