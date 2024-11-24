using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Sounds/SoundLibrary", fileName = "SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundMap> sounds;

    public SoundMap Match(string soundId)
    {
        SoundMap foundMap = sounds.Find(x => x.soundId == soundId);

        if(foundMap == null)
        {
            Debug.LogError("No mathc found with sound id: " + soundId);
        }

        return foundMap;
    }

    [Serializable]
    public class SoundMap
    {
        public string soundId;
        public List<AudioClip> soundAssets;

        public AudioClip RandomAudioClip()
        {
            if(soundAssets.Count==0)
            {
                Debug.LogError("No sounds in soundLibrary: " + this);
                return null;
            }

            int rnd = UnityEngine.Random.Range(0, soundAssets.Count);
            return soundAssets[rnd];
        }

    }
}
