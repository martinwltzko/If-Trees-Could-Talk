using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlipbookImageAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float animationSpeed = 0.1f;

    private Image _image;
    private Coroutine _flipbookCoroutine;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _flipbookCoroutine = StartCoroutine(Flipbook());
    }

    private void OnDisable()
    {
        if(_flipbookCoroutine == null) return;
        StopCoroutine(_flipbookCoroutine);
        _flipbookCoroutine = null;
    }
    
    private IEnumerator Flipbook()
    {
        int i = 0;
        while (true) 
        {
            // Set the sprite to the current frame
            yield return new WaitForSeconds(animationSpeed);
            i = (i + 1) % sprites.Count;
            _image.sprite = sprites[i];
        }
    }
}
