using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>ListItemController</c> handles logic for an item in <ListController>.
/// </summary>
public class ListItemController : MonoBehaviour
{
    public Text _name, _date, _size;
    public MenuManager menuManager;

    public void Start()
    {
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
    }

    public void ItemButtonSelected()
    {
        menuManager.SetFileName(_name.text);
    }
}
