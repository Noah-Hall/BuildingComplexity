using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*   Stairwell is a class that maintains the logic of a   *
*   physical stairwell. It is used by the ManagerScript  *
*   to fill relevant info and "connect" relevant stairs  *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class Stairwell
{
    private List<GameObject> _stairwell = new List<GameObject>();
    public int _num;
    public int _count;
    private bool _exitFound;
    private GameObject _exitFloor;

    private void Order()
    {
        _stairwell.Sort(CompareStairs);
    }

    private static int CompareStairs(GameObject x_object, GameObject y_object)
    {
        int x = x_object.GetComponent<StairScript>()._floor;
        int y = y_object.GetComponent<StairScript>()._floor;

        return x - y;
    }

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

    public GameObject Get(int floor)
    {
        if (floor <= _stairwell.Count) {
            return _stairwell[floor - 1];
        }
        return null;
    }

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

    public Stairwell(int stairwellNum)
    {
        _num = stairwellNum;
        _count = _stairwell.Count;
    }

    public Stairwell(int stairwellNum, List<GameObject> stairs)
    {
        _num = stairwellNum;
        _stairwell = stairs;
        _count = _stairwell.Count;
        Order();
    }
}