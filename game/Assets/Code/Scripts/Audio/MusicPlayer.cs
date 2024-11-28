using System;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private MusicLibrary _musicLibrary;
    [SerializeField] private EventInstance _musicEventInstance;
    
    [SerializeField] private bool playOnStart;
    [SerializeField, EnableIf("playOnStart")] private MusicType trackType;
    [SerializeField, EnableIf("playOnStart")] private MusicVocalTrack vocalStartTrack;
    [SerializeField, EnableIf("playOnStart")] private MusicInstrumentalTrack instrumentalStartTrack;

    private void Start()
    {
        if (!playOnStart) return;
        
        PlayMusic(trackType);
        switch (trackType)
        {
            case MusicType.Instrumental:
                SetInstrumentalMusicParameter(instrumentalStartTrack);
                break;
            case MusicType.Vocal:
                SetVocalMusicParameter(vocalStartTrack);
                break;
            default:
                Debug.LogError("Unknown music type", this);
                break;
        }
    }

    private void OnDestroy()
    {
        StopMusic();
    }
    
    public void PlayMusic(MusicTypeObject typeObject) => PlayMusic(typeObject.musicType);
    public void SetVocalMusicParameter(VocalTrackObject trackObject) => SetVocalMusicParameter(trackObject.vocalTrack);
    public void SetInstrumentalMusicParameter(InstrumentalTrackObject trackObject) => SetInstrumentalMusicParameter(trackObject.instrumentalTrack);
    
    public void PlayMusic(MusicType type)
    {
        var music = _musicLibrary.Match(type);
        if (music == null)
        {
            Debug.LogWarning($"Music with type {type} not found in MusicLibrary");
            return;
        }

        _musicEventInstance = AudioManager.Instance.CreateInstance(music.musicEvent);
        _musicEventInstance.start();
    }
    
    public void SetVocalMusicParameter(MusicVocalTrack vocalTrack)
    {
        _musicEventInstance.setParameterByName("track", (float)vocalTrack);
    }
    public void SetInstrumentalMusicParameter(MusicInstrumentalTrack trackObject)
    {
        _musicEventInstance.setParameterByName("track", (float)trackObject);
    }
    
    
    public void StopMusic()
    {
        _musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}