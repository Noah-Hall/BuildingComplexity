using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorplanRunner : MonoBehaviour
{
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI speedSliderText;
    [SerializeField] private ManagerScript manager;

    public void UpdateSpeedSlider() {
        speedSliderText.SetText(speedSlider.value.ToString() + "X");
    }

    public void StartRunner() {
        manager.simulationSpeed = (int)speedSlider.value;
        manager.StartRun();
    }
}
