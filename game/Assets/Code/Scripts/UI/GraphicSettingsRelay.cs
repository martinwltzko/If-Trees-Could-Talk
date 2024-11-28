using System;
using Code.Scripts.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GraphicSettingsRelay : MonoBehaviour
{
    [SerializeField] private UnityEvent forceCloseEvent;
    [SerializeField] private UnityEvent applySettingsDialog;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button applyButton;
    
    private int _currentQualityLevel;
    private int _selectedQualityLevel;
    private bool _fullscreen;

    private bool ApplyNecessary => _selectedQualityLevel!=_currentQualityLevel || _fullscreen!=Screen.fullScreen;

    private void Awake()
    {
        ReloadSettings();
        ApplySettings();
    }

    private void Start()
    {
        qualityText.text = ((GraphicsQuality)_currentQualityLevel).ToString();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void ReloadSettings()
    {
        _currentQualityLevel = (int)SaveSystem.GetFloat(SaveSystem.SaveVariable.Quality);
        _selectedQualityLevel = (int)SaveSystem.GetFloat(SaveSystem.SaveVariable.Quality);
        _fullscreen = Screen.fullScreen;
        
        Debug.Log("Quality Level: " + _currentQualityLevel);
        Debug.Log("Fullscreen: " + Screen.fullScreen);
    }
    
    public void TraverseQuality()
    {
        _selectedQualityLevel = (_selectedQualityLevel + 1) % QualitySettings.names.Length;
        qualityText.text = ((GraphicsQuality)_selectedQualityLevel).ToString(); ;
        
        applyButton.gameObject.SetActive(ApplyNecessary);
    }
    
    public void ToggleFullscreen()
    {
        _fullscreen = !_fullscreen;
        fullscreenToggle.isOn = _fullscreen;
        
        applyButton.gameObject.SetActive(ApplyNecessary);
    }

    public void TryClose()
    {
        if (ApplyNecessary)
        {
            fullscreenToggle.isOn = _fullscreen;
            qualityText.text = QualitySettings.names[_currentQualityLevel];
            applySettingsDialog.Invoke();
        }
        else forceCloseEvent.Invoke();
    }
    
    public void ForceClose()
    {
        _fullscreen = Screen.fullScreen;
        _selectedQualityLevel = _currentQualityLevel;
        
        qualityText.text = QualitySettings.names[_selectedQualityLevel];
        fullscreenToggle.isOn = _fullscreen;
        
        forceCloseEvent.Invoke();
    }
    
    public void ApplySettings()
    {
        _currentQualityLevel = _selectedQualityLevel;

        Screen.fullScreen = _fullscreen;
        GameSettings.Quality = (GraphicsQuality)_currentQualityLevel;
        QualitySettings.SetQualityLevel((int)GameSettings.Quality);
        applyButton.gameObject.SetActive(ApplyNecessary);
    }
}