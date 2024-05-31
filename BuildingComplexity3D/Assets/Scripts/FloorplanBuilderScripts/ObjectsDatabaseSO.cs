using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>ObjectDatabaseSO</c> holds a <c>List</c> of all object types for <c>PlacementSystem</c>.
/// </summary>
[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    /// <value>
    /// Property <c>objectsData</c> holds all <c>ObjectData</c> items as <c>List</c>.
    /// </value>
    public List<ObjectData> objectsData;
}

/// <summary>
/// Class <c>ObjectData</c> holds properties to describe an object in <c>PlacementSystem</c>.
/// </summary>
[Serializable]
public class ObjectData
{
    /// <value>
    /// Property <c>Name</c> holds the <c>string</c> name of <c>ObjectData</c>.
    /// </value>
    [field: SerializeField]
    public string Name { get; private set; }

    /// <value>
    /// Property <c>ID</c> holds the <c>int</c> id of <c>ObjectData</c>.
    /// </value>
    [field: SerializeField]
    public int ID { get; private set; }

    /// <value>
    /// Property <c>orientation</c> holds the <c>PlacementOrientation</c> value of <c>ObjectData</c>.
    /// </value>
    [field: SerializeField]
    public PlacementOrientation orientation { get; private set; }

    /// <value>
    /// Property <c>Size</c> holds the <c>Vector2Int</c> size of <c>ObjectData</c>.
    /// </value>
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    /// <value>
    /// Property <c>Prefab</c> holds the <c>GameObject</c> prefab of <c>ObjectData</c>.
    /// </value>
    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    /// <value>
    /// Property <c>color</c> holds the <c>Color</c> color of <c>ObjectData</c>.
    /// </value>
    [field: SerializeField]
    public Color color { get; private set; }
}

/// <summary>
/// Enum <c>PlacementOrientation</c> is used to describe an object's orientation in <c>PlacementSystem</c>.
/// </summary>
public enum PlacementOrientation
{
    CENTER,
    SIDE,
    BOTTOM,
    NONE
}
