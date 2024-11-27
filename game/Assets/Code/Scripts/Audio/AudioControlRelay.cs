using UnityEngine;

public class AudioControlRelay : MonoBehaviour
{
    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.masterVolume = volume;
    }
    
    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.musicVolume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.sfxVolume = volume;
    }
    
    public void SetAmbienceVolume(float volume)
    {
        AudioManager.Instance.ambienceVolume = volume;
    }
}