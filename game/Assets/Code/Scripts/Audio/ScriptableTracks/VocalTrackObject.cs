using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "VocalTrackObject", menuName = "ScriptableContainer/VocalTrackObject", order = 0)]
public class VocalTrackObject : ScriptableObject
{
    public MusicVocalTrack vocalTrack;
}

[Serializable]
public enum MusicVocalTrack
{
    ARoadInTheCountry=0,
    CouldBe=1,
    TellTheStories=2,
}