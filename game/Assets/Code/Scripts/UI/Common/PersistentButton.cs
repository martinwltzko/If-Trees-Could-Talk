using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PersistentButton : MonoBehaviour
{
    private Button _button;
    private bool _highlighted;
    private Image _highlightImage;
    
    private Action _onClick = delegate { };
    
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _highlightImage = transform.Find("Highlighting")?.GetComponent<Image>();
        
        _button.onClick.AddListener(() => SetHighlight(! _highlighted));
        _button.onClick.AddListener(() => _onClick.Invoke());
    }

    public void Click()
    {
        _onClick.Invoke();
    }
    
    public void SetOnClickAction(Action onClick) {
        _onClick = onClick;
    }

    public void SetHighlight(bool value)
    {
        if (!_highlightImage) {
            Debug.LogError("No highlight image found!");
        };
        _highlightImage.enabled = value;
    }
}
