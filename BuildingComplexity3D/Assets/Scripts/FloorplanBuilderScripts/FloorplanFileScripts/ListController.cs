using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

/// <summary>
/// Class <c>ListController</c> handles logic for displaying the list of floorplan files.
/// </summary>
public class ListController : MonoBehaviour
{
    // public Sprite [] AnimalImages;
	/// <value>
    /// Property <c>ContentPanel</c> holds the <c>GameObject</c> panel which files need to be added to.
    /// </value>
	public GameObject ContentPanel;

	/// <value>
    /// Property <c>ListItemPrefab</c> holds the <c>GameObject</c> prefab which represents a file.
    /// </value>
	public GameObject ListItemPrefab;

	ArrayList FileDatas;

	void Start () {

		// 1. Get the data to be displayed
        FileDatas = new ArrayList();
        var info = new DirectoryInfo(Application.dataPath + "/Scene_Files/Floorplan_Builds");
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo) {
            if (!file.Name.Contains(".meta") ) {
                FileDatas.Add(new FileData(file.Name, file.LastWriteTime.ToString(), file.Length.ToString()));
            }
        }

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
