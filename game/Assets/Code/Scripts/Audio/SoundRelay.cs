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
}
