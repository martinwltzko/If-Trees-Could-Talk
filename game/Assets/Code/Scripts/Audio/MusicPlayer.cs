using FMOD.Studio;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private SoundLibrary _soundLibrary;
    private EventInstance _musicEventInstance;
    
    public void PlayMusic(string musicName)
    {
        var music = _soundLibrary.Match(musicName);
        if (music == null)
        {
            Debug.LogWarning($"Music with name {musicName} not found in SoundLibrary");
            return;
        }

        _musicEventInstance = AudioManager.Instance.CreateInstance(music.soundEvent);
        _musicEventInstance.start();
    }
    
    public void SetMusicParameter(MusicTrackObject trackObject)
    {
        _musicEventInstance.setParameterByName("track", (float)trackObject.track);
    }
    
    public void StopMusic()
    {
        _musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}