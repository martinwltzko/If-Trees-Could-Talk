using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

[System.Serializable]
public class SoundRelay
{
    public SoundLibrary soundLibrary;
    public Transform soundSource;

    public void PlaySoundOneShot(string soundId)
    {
        SoundLibrary.SoundMap map = soundLibrary.Match(soundId);
        AudioManager.Instance.PlayOneShot(map.soundEvent, soundSource.position);
    }

    public void PlaySoundWithParameter(string soundId, string parameterName, float parameter)
    {
        var map = soundLibrary.Match(soundId);
        var instance = AudioManager.Instance.CreateTemporaryInstance(map.soundEvent);
        instance.setParameterByName(parameterName, parameter);
        instance.start();
    }
}
