using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Stairwell</c> maintains the logic of a physical stairwell.
/// It is used by <c>ManagerScript</c> to fill relevant info and "connect" relevant <c>Stairs</c>.
/// </summary>
public class Stairwell
{
    private List<GameObject> _stairwell = new List<GameObject>();
    private bool _exitFound;
    private GameObject _exitFloor;

    /// <value>
    /// Property <c>_num</c> functions as the <c>Stairwell</c> ID. It is related to <c>Stair._stairwell</c>.
    /// </value>
    public int _num;

    /// <value>
    /// Property <c>_count</c> is the number of <c>Stairs</c> in the <c>Stairwell</c>.
    /// </value>
    public int _count;

    // sorts the _stairwell list to keep floors in numerical order
    private void Order()
    {
        _stairwell.Sort(CompareStairs);
    }

    // compares two stair GameObjects
    private static int CompareStairs(GameObject x_object, GameObject y_object)
    {
        int x = x_object.GetComponent<StairScript>()._floor;
        int y = y_object.GetComponent<StairScript>()._floor;

        return x - y;
    }

    /// <summary>
    /// Adds <paramref name="newStair"/> to <c>_stairwell</c>.
    /// </summary>
    /// <returns>
    /// True if <paramref name="newStair"/> was added sucessfully.
    /// </returns>
    /// <param name="newStair">The <c>Stair</c> to be added.</param>
    public bool Add(GameObject newStair)
    {
        if (_num == newStair.GetComponent<StairScript>()._stairwell) {
            _stairwell.Add(newStair);
            Order();
            _count = _stairwell.Count;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Gets <c>Stair</c> by floor #.
    /// </summary>
    /// <returns>
    /// Stair <c>GameObject</c>.
    /// </returns>
    /// <param name="floor">The floor to get the <c>Stair</c> from.</param>
    public GameObject Get(int floor)
    {
        if (floor <= _stairwell.Count) {
            return _stairwell[floor - 1];
        }
        return null;
    }

    /// <summary>
    /// Gets <c>Stair</c> on same floor as an <c>Exit</c>.
    /// </summary>
    /// <returns>
    /// Stair <c>GameObject</c>.
    /// </returns>
    /// <param name="floor">The floor to get the <c>Stair</c> from.</param>
    public GameObject GetExitFloor()
    {
        if (_exitFound) {
            return _exitFloor;
        }

        foreach (GameObject stair in _stairwell) {
            if (stair.GetComponent<StairScript>()._isExitFloor) {
                _exitFound = true;
                _exitFloor = stair;
                return _exitFloor;
            }
        }
        
        return null;
    }

    /// <summary>
    /// Constructor initializes <c>_num</c> and <c>_count</c>.
    /// </summary>
    /// <param name="stairwellNum">Initializes <c>_num</c>.</param>
    public Stairwell(int stairwellNum)
    {
        _num = stairwellNum;
        _count = _stairwell.Count;
    }

    /// <summary>
    /// Constructor initializes <c>_num</c>, <c>_count</c>, and <c>_stairwell</c>.
    /// </summary>
    /// <param name="stairwellNum">Initializes <c>_num</c>.</param>
    /// <param name="stairs">Initializes <c>_stairwell</c>.</param>
    public Stairwell(int stairwellNum, List<GameObject> stairs)
    {
        _num = stairwellNum;
        _stairwell = stairs;
        _count = _stairwell.Count;
        Order();
    }
}