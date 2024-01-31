using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

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
    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);   
    }

    public void SaveButton()
    {

    }

    public void QuitButton()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    public void SaveAndQuitButton()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    /*
     *  Functions: 3 (borrows show/close settings panel functions)
     *  Scene: Main_Menu
     *  Panel: Background
     */
    public void FloorplanEditorButton()
    {
        SceneManager.LoadScene("Build_Selector");
    }

    public void RunFloorplanButton()
    {
        SceneManager.LoadScene("Run_Selector");
    }

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
