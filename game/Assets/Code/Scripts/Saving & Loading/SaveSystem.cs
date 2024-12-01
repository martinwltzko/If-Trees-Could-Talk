using UnityEngine;

public static class SaveSystem
{
    private const string MasterVolume = "MasterVolume";
    private const string MusicVolume = "MusicVolume";   
    private const string SfxVolume = "SfxVolume";
    private const string AmbienceVolume = "AmbienceVolume";
    private const string Quality = "Quality";
    private const string Sensitivity = "Sensitivity";
    private const string InvertY = "InvertY";
    private const string InvertX = "InvertX";
    private const string ToggleBackground = "ToggleBackground";
    private const string MessageAmount = "MessageAmount";
    private const string TutorialPlayed = "TutorialPlayed";
    
    public enum SaveVariable
    {
        MasterVolume,
        MusicVolume,
        SfxVolume,
        AmbienceVolume,
        Quality,
        Sensitivity,
        InvertY,
        InvertX,
        TextBackground,
        MessageAmount,
        TutorialPlayed,
    }
    
    public static void SaveFloat(SaveVariable variable, float value)
    {
        switch (variable)
        {
            case SaveVariable.MasterVolume:
                PlayerPrefs.SetFloat(MasterVolume, value);
                break;
            case SaveVariable.MusicVolume:
                PlayerPrefs.SetFloat(MusicVolume, value);
                break;
            case SaveVariable.SfxVolume:
                PlayerPrefs.SetFloat(SfxVolume, value);
                break;
            case SaveVariable.AmbienceVolume:
                PlayerPrefs.SetFloat(AmbienceVolume, value);
                break;
            case SaveVariable.Quality:
                PlayerPrefs.SetFloat(Quality, value);
                break;
            case SaveVariable.Sensitivity:
                PlayerPrefs.SetFloat(Sensitivity, value);
                break;
            case SaveVariable.InvertX:
                PlayerPrefs.SetFloat(InvertX, value);
                break;
            case SaveVariable.InvertY:
                PlayerPrefs.SetFloat(InvertY, value);
                break;
            case SaveVariable.TextBackground:
                PlayerPrefs.SetFloat(ToggleBackground, value);
                break;
            case SaveVariable.MessageAmount:
                PlayerPrefs.SetFloat(MessageAmount, value);
                break;
            case SaveVariable.TutorialPlayed:
                PlayerPrefs.SetFloat(TutorialPlayed, value);
                break;
            default:
                Debug.LogWarning($"(SaveSystem) Variable not found {variable}");
                break;
                
        }
        Debug.Log($"Saved variable {variable} with value {value}");
    }

    public static float GetFloat(SaveVariable variable)
    {
        var value = 0f;
        switch (variable)
        {
            case SaveVariable.MasterVolume:
                value = PlayerPrefs.GetFloat(MasterVolume, 1f);
                break;
            case SaveVariable.MusicVolume:
                value =  PlayerPrefs.GetFloat(MusicVolume, 1f);
                break;
            case SaveVariable.SfxVolume:
                value = PlayerPrefs.GetFloat(SfxVolume, 1f);
                break;
            case SaveVariable.AmbienceVolume:
                value = PlayerPrefs.GetFloat(AmbienceVolume, 1f);
                break;
            case SaveVariable.Quality:
                value =  PlayerPrefs.GetFloat(Quality, 2f);
                break;
            case SaveVariable.Sensitivity:
                value = PlayerPrefs.GetFloat(Sensitivity, .5f);
                break;
            case SaveVariable.InvertX:
                value = PlayerPrefs.GetFloat(InvertX, 0f);
                break;
            case SaveVariable.InvertY:
                value = PlayerPrefs.GetFloat(InvertY, 0f);
                break;
            case SaveVariable.TextBackground:
                value = PlayerPrefs.GetFloat(ToggleBackground, 0f);
                break;
            case SaveVariable.MessageAmount:
                value = PlayerPrefs.GetFloat(MessageAmount, 0f);
                break;
            case SaveVariable.TutorialPlayed:
                value = PlayerPrefs.GetFloat(TutorialPlayed, 0f);
                break;
            default:
                Debug.LogWarning($"(SaveSystem) Variable not found {variable}");
                break;
        }
        Debug.Log($"Loaded {variable} with value {value}");
        return value;
    }
}