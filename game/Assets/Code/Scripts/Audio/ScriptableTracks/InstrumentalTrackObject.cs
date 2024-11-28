using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InstrumentalTrackObject", menuName = "ScriptableContainer/InstrumentalTrackObject", order = 0)]
public class InstrumentalTrackObject : ScriptableObject
{
    public MusicInstrumentalTrack instrumentalTrack;
}

[Serializable]
public enum MusicInstrumentalTrack
{
    Track01=0,
    Track02=1,
}