using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameSettingRelay : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Toggle invertYToggle;
    [SerializeField] private Toggle invertXToggle;
    [SerializeField] private Toggle textBackgroundToggle;
    
    private float _sensitivity;
    private bool _invertY;
    private bool _invertX;
    private bool _textBackground;

    private void Awake()
    {
        _sensitivity = SaveSystem.GetFloat(SaveSystem.SaveVariable.Sensitivity);
        _invertY = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.InvertY), 1f);
        _invertX = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.InvertX), 1f);
        _textBackground = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.TextBackground), 1f);
        
        GameSettings.Sensitivity = _sensitivity;
        GameSettings.InvertY = _invertY;
        GameSettings.InvertX = _invertX;
        GameSettings.TextBackground = _textBackground;
    }

    private void Start()
    {
        sensitivitySlider.value = GameSettings.Sensitivity;
        invertYToggle.isOn = GameSettings.InvertY;
        invertXToggle.isOn = GameSettings.InvertX;
        textBackgroundToggle.isOn = GameSettings.TextBackground;
    }
    
    public void SetSensitivity(float sensitivity)
    {
        GameSettings.Sensitivity = sensitivity;
        sensitivitySlider.value = sensitivity;
    }
    
    public void ToggleInvertY()
    {
        var value = !GameSettings.InvertY;
        GameSettings.InvertY = value;
        invertYToggle.isOn = value;
    }
    
    public void ToggleInvertX()
    {
        var value = !GameSettings.InvertX;
        GameSettings.InvertX = value;
        invertXToggle.isOn = value;
    }
    
    public void ToggleTextBackground()
    {
        var value = !GameSettings.TextBackground;
        GameSettings.TextBackground = value;
        textBackgroundToggle.isOn = value;
    }
}