using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

[CreateAssetMenu(menuName = "Sounds/SoundLibrary", fileName = "SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundMap> sounds;

    public SoundMap Match(string soundId)
    {
        SoundMap foundMap = sounds.Find(x => x.soundId == soundId);

        if(foundMap == null)
        {
            Debug.LogError("No match found with sound id: " + soundId);
        }

        return foundMap;
    }

    [Serializable]
    public class SoundMap
    {
        public string soundId;
        public EventReference soundEvent;
    }
}
