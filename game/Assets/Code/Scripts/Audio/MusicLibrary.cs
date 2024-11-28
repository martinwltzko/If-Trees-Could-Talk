using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioLibrary/MusicLibrary", fileName = "MusicLibrary")]
public class MusicLibrary : ScriptableObject
{
    public List<MusicMap> music;
    
    public MusicMap Match(MusicType type)
    {
        MusicMap foundMap = music.Find(x => x.musicType == type);

        if(foundMap == null)
        {
            Debug.LogError("No match found with music id: " + type);
        }

        return foundMap;
    }
    
    [Serializable]
    public class MusicMap
    {
        public MusicType musicType;
        public EventReference musicEvent;
    }
}