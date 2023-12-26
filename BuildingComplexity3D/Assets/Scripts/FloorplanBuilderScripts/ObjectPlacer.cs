using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedObjects;

    private void Start () 
    { 
        placedObjects = new List<GameObject>(); 
    }

    public int PlaceObject(GameObject prefab, Vector3 pos)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = pos;
        placedObjects.Add(newObject);
        return placedObjects.Count - 1;
    }

    public void RemoveObjectAt(int index)
    {
        if (placedObjects.Count <= index || placedObjects[index] == null) { return; }
        Destroy(placedObjects[index]);
        placedObjects[index] = null;
    }
}
