using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileData
{
    public string _name, _date, _size;

	public FileData(string name, string date, string size){
		_name = name;
        _date = date;
        _size = size;
	}
}
