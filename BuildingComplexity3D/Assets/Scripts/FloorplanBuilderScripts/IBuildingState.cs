// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface <c>IBuildingState</c> defines the structure for building states used by PlacementSystem.
/// </summary>
public interface IBuildingState
{
    /// <summary>
    /// Method for transitioning out of given state.
    /// </summary>
    public void EndState();

    /// <summary>
    /// Method for enacting given state action.
    /// </summary>
    public void OnAction(Vector3Int gridPosition);

    /// <summary>
    /// Method for updating within given state.
    /// </summary>
    public void UpdateState(Vector3Int gridPosition);
}
