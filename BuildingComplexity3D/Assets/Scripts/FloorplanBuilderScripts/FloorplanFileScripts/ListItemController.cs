using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
