using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlipbookImageAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float animationSpeed = 0.1f;
    [SerializeField] private bool playOnAwake = true;

    private bool _playing;
    private int _currentSpriteIndex;

    private void Awake() 
    {
        if (playOnAwake) Play();
    }

    private void OnDestroy() {
        _playing = false;
    }

    public async void Play()
    {
        if (_playing) {
            _currentSpriteIndex = 0;
            return;
        }
        Debug.Log("<color=green>Playing flipbook animation</color>");
        await PlayAsync();
    }
    
    private async UniTask PlayAsync()
    {
        _currentSpriteIndex = 0;
        _playing = true;
        while (_playing) {
            image.sprite = sprites[_currentSpriteIndex];
            _currentSpriteIndex = (_currentSpriteIndex + 1) % sprites.Count;
            await UniTask.Delay(TimeSpan.FromSeconds(animationSpeed));
        }
    }
}
