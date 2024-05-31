using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>FileData</c> holds the basic data about a file.
/// </summary>
[System.Serializable]
public class FileData
{
    public string _name, _date, _size;

	public FileData(string name, string date, string size){
		_name = name;
        _date = date;
        _size = size;
	}
}
