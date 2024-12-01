using System;
using FlameArthur.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class DirectorStarter : MonoBehaviour
{
    private PlayableDirector _director;
    [SerializeField] private float endTime;
    [SerializeField] private UnityEvent onDirectorEnd;

    private bool _ended;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        _director.Play();
        _ended = false;
    }
    
    private void Update()
    {
        if (!_ended && (float)_director.time >= endTime) {
            onDirectorEnd.Invoke();
            _ended = true;
        }
    }
}