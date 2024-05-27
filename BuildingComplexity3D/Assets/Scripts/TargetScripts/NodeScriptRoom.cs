using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>NodeScriptRoom</c> is attached to all <c>RoomNode</c> target <c>GameObjects</c>.
/// Inherets from <c>NodeScript</c> and has <c>weight</c> property.
/// </summary>
public class NodeScriptRoom : NodeScript
{
    /// <value>
    /// Property <c>weight</c> represents the capacity of the room.
    /// </value>
    public int weight;
}
