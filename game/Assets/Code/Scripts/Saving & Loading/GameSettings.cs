using System;
using UnityEngine;



public static class GameSettings
{
    private static float _masterVolume = SaveSystem.GetFloat(SaveSystem.SaveVariable.MasterVolume);
    private static float _musicVolume = SaveSystem.GetFloat(SaveSystem.SaveVariable.MusicVolume);
    private static float _sfxVolume = SaveSystem.GetFloat(SaveSystem.SaveVariable.SfxVolume);
    private static float _ambienceVolume = SaveSystem.GetFloat(SaveSystem.SaveVariable.AmbienceVolume);
    
    private static GraphicsQuality _quality = (GraphicsQuality)SaveSystem.GetFloat(SaveSystem.SaveVariable.Quality);
    
    private static float _sensitivity = SaveSystem.GetFloat(SaveSystem.SaveVariable.Sensitivity);
    private static bool _invertY = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.InvertY), 1f);
    private static bool _invertX = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.InvertX), 1f);
    private static bool _textBackground = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.TextBackground), 1f);

    public static event Action<float> OnSensitivityChanged;
    public static event Action<bool> OnInvertYChanged;
    public static event Action<bool> OnInvertXChanged;
    public static event Action<bool> OnTextBackgroundChanged;
    
    public static float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.MasterVolume, value);
        }
    }
    
    public static float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.MusicVolume, value);
        }
    }
    
    public static float SfxVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.SfxVolume, value);
        }
    }
    
    public static float AmbienceVolume
    {
        get => _ambienceVolume;
        set
        {
            _ambienceVolume = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.AmbienceVolume, value);
        }
    }
    
    public static GraphicsQuality Quality
    {
        get => _quality;
        set
        {
            _quality = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.Quality, (float)value);
        }
    }
    
    public static float Sensitivity
    {
        get => _sensitivity;
        set
        {
            _sensitivity = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.Sensitivity, value);
            OnSensitivityChanged?.Invoke(value);
        }
    }
    
    public static bool InvertY
    {
        get => _invertY;
        set
        {
            _invertY = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.InvertY, value ? 1f : 0f);
            OnInvertYChanged?.Invoke(value);
        }
    }
    
    public static bool InvertX
    {
        get => _invertX;
        set
        {
            _invertX = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.InvertX, value ? 1f : 0f);
            OnInvertXChanged?.Invoke(value);
        }
    }
    
    public static bool TextBackground
    {
        get => _textBackground;
        set
        {
            _textBackground = value;
            SaveSystem.SaveFloat(SaveSystem.SaveVariable.TextBackground, value ? 1f : 0f);
            OnTextBackgroundChanged?.Invoke(value);
        }
    }
}