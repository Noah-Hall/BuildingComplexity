using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour
{
    // public Sprite [] AnimalImages;
	public GameObject ContentPanel;
	public GameObject ListItemPrefab;

	ArrayList FileDatas;

	void Start () {

		// 1. Get the data to be displayed
		FileDatas = new ArrayList (){
			new FileData("name1", "date1", "size1"),
			new FileData("name2", "date2", "size2"),
            new FileData("name3", "date3", "size3"),
            new FileData("name4", "date4", "size4"),
			new FileData("name5", "date5", "size5"),
            new FileData("name6", "date6", "size6"),
            new FileData("name1", "date1", "size1"),
			new FileData("name2", "date2", "size2"),
            new FileData("name3", "date3", "size3"),
            new FileData("name4", "date4", "size4"),
			new FileData("name5", "date5", "size5"),
            new FileData("name6", "date6", "size6"),
		};

		// 2. Iterate through the data, 
		//	  instantiate prefab, 
		//	  set the data, 
		//	  add it to panel
		foreach(FileData data in FileDatas) {
			GameObject newFileData = Instantiate(ListItemPrefab) as GameObject;
            ListItemController controller = newFileData.GetComponent<ListItemController>();
			controller._name.text = data._name;
			controller._date.text = data._date;
			controller._size.text = data._size;
			newFileData.transform.SetParent(ContentPanel.transform);
			newFileData.transform.localScale = Vector3.one;
		}
	}	
}
