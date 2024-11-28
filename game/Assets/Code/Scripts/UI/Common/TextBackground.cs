using System;
using UnityEngine;
using UnityEngine.UI;

public class TextBackground : MonoBehaviour
{
    [SerializeField] private Image background;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background.raycastTarget = false;
        background.enabled = GameSettings.TextBackground;
    }

    private void OnEnable()
    {
        GameSettings.OnTextBackgroundChanged += OnTextBackgroundChanged;
    }

    private void OnDestroy()
    {
        GameSettings.OnTextBackgroundChanged -= OnTextBackgroundChanged;
    }

    private void OnTextBackgroundChanged(bool visible)
    {
        if(background == null) return;
        background.enabled = visible;
    }
}
