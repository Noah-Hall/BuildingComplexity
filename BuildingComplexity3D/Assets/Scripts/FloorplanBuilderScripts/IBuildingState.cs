// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public interface IBuildingState
{
    public void EndState();
    public void OnAction(Vector3Int gridPosition);
    public void UpdateState(Vector3Int gridPosition);
}
