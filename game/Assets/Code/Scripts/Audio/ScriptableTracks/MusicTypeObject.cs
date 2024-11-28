using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTypeObject", menuName = "ScriptableContainer/MusicTypeObject", order = 0)]
public class MusicTypeObject : ScriptableObject
{
    public MusicType musicType;
}

[Serializable]
public enum MusicType
{
    Vocal,
    Instrumental
}