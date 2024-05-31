using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Class <c>MenuManager</c> is responsible for handling user navigation with UI.
/// (Some methods are empty/contain commented out code and thus are depricated)
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;
    private string filenameString = null;
    private Text filenameText;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Method sets file to be opened for <c>Run_Selector</c> and <c>Build_Selector</c> scenes.
    /// </summary>
    /// <param name="newFilename">Filename to be loaded.</param>
    public void SetFileName(string newFilename)
    {
        filenameString = newFilename;
        filenameText = GameObject.Find("FilenameText").GetComponent<Text>();
        button = GameObject.Find("LoadButton").GetComponent<Button>();
        filenameText.text = filenameString;
        button.interactable = true;
    }

    /*
     *  Functions: 5
     *  Scene: Floorplan_Editor
     *  Panel: SettingsPanel
     */
    /// <summary>
    /// Method handles logic for showing settings panel when user clicks settings gear.
    /// </summary>
    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Method handles logic for unshowing settings panel when user clicks "x" in settings panel.
    /// </summary>
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);   
    }

    public void SaveButton()
    {

    }

    /// <summary>
    /// Method handles logic for returning to main menu.
    /// </summary>
    public void QuitButton()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    /// <summary>
    /// Method handles logic for saving and returning to main menu.
    /// </summary>
    public void SaveAndQuitButton()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    /*
     *  Functions: 3 (borrows show/close settings panel functions)
     *  Scene: Main_Menu
     *  Panel: Background
     */
    /// <summary>
    /// Method handles logic for navigating to <c>Build_Selector</c> scene.
    /// </summary>
    public void FloorplanEditorButton()
    {
        SceneManager.LoadScene("Build_Selector");
    }

    /// <summary>
    /// Method handles logic for navigating to <c>Run_Selector</c> scene.
    /// </summary>
    public void RunFloorplanButton()
    {
        SceneManager.LoadScene("Run_Selector");
    }

    /// <summary>
    /// Method handles logic for closing EASE application.
    /// </summary>
    public void CloseEaseButton()
    {
        Application.Quit();
    }

    /*
     *  Functions: 1
     *  Scene: Build_Selector
     *  Panel: Background
     */

     public void LoadButton()
     {
        // SceneManager.LoadScene("Floorplan_Editor");
     }

     /*
     *  Functions: 1
     *  Scene: Run_Selector
     *  Panel: Background
     */

     public void RunButton()
     {
        // SceneManager.LoadScene("Floorplan_Runner");
     }
}
