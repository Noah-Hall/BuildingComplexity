using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>FloorplanRunner</c> handles logic for <c>Floorplan_Runner</c> scene.
/// </summary>
public class FloorplanRunner : MonoBehaviour
{
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI speedSliderText;
    [SerializeField] private ManagerScript manager;

    /// <summary>
    /// Method handles logic for user selecting simulation speed.
    /// </summary>
    public void UpdateSpeedSlider() {
        speedSliderText.SetText(speedSlider.value.ToString() + "X");
    }

    /// <summary>
    /// Method handles logic for user running simulation.
    /// </summary>
    public void StartRunner() {
        manager.simulationSpeed = (int)speedSlider.value;
        manager.StartRun();
    }
}
