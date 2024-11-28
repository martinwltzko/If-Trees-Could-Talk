using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioControlRelay : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambienceVolumeSlider;

    private void Start()
    {
        SetMasterVolume(SaveSystem.GetFloat(SaveSystem.SaveVariable.MasterVolume));
        SetMusicVolume(SaveSystem.GetFloat(SaveSystem.SaveVariable.MusicVolume));
        SetSFXVolume(SaveSystem.GetFloat(SaveSystem.SaveVariable.SfxVolume));
        SetAmbienceVolume(SaveSystem.GetFloat(SaveSystem.SaveVariable.AmbienceVolume));
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.masterVolume = volume;
        GameSettings.MasterVolume = volume;
        masterVolumeSlider.value = volume;
    }
    
    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.musicVolume = volume;
        GameSettings.MusicVolume = volume;
        musicVolumeSlider.value = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.sfxVolume = volume;
        GameSettings.SfxVolume = volume;
        sfxVolumeSlider.value = volume;
    }
    
    public void SetAmbienceVolume(float volume)
    {
        AudioManager.Instance.ambienceVolume = volume;
        GameSettings.AmbienceVolume = volume;
        ambienceVolumeSlider.value = volume;
    }
}