
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFlipbook : MonoBehaviour
{
    [SerializeField] private List<Sprite> flipbookCursors;
    [SerializeField] private float flipbookSpeed = 0.1f;
    
    private List<Texture2D> _flipbookTextures;
    [SerializeField] private bool _enabled = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _flipbookTextures = GenerateTextures();
        StartCoroutine(Flipbook());
    }
    
    private List<Texture2D> GenerateTextures()
    {
        _flipbookTextures = new List<Texture2D>();
        for (var i = 0; i < flipbookCursors.Count; i++)
        {
            var sprite = flipbookCursors[i];
            var texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            texture.SetPixels(sprite.texture.GetPixels(i*128, 0, 128, 128));
            texture.Apply();
            _flipbookTextures.Add(texture);
        }

        return _flipbookTextures;
    }

    private IEnumerator Flipbook()
    {
        int i = 0;
        while (_enabled) 
        {
            Cursor.SetCursor(_flipbookTextures[i], Vector2.zero, CursorMode.Auto);
            yield return new WaitForSeconds(flipbookSpeed);

            i = (i + 1) % _flipbookTextures.Count;
        }
    }
}
