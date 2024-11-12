using System;
using System.Collections.Generic;
using AdvancedController;
using Code.Scripts.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class SelectionCircle : MonoBehaviour
{
    [SerializeField] private int idleScaling = 20;
    [SerializeField] private int activeScaling = 100;
    [SerializeField] private float activationSmoothness = 10f;
    
    [SerializeField, Range(0,1f)] private float sensitivity = .75f;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private float clampDistance = 1f;
    [SerializeField] private float selectionThreshold;
    
    [SerializeField] private float optionRadius = 100f;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private OptionText optionPrefab;
    
    private Vector2 _mouseDelta;
    private bool _pressed;

    private readonly Dictionary<OptionProvider.Option, OptionText> _optionsMapping = new();
    private OptionProvider.Option _selectedOption;
    private OptionProvider _optionProvider;

    private RectTransform _rt;

    private void Start()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void SetOptions(OptionProvider optionProvider)
    {
        if(_optionProvider == optionProvider) return;
        _optionProvider = optionProvider;
        
        UpdateOptions(optionProvider);
    }

    public void UpdateSelection(Vector2 mouseDelta)
    {
        _mouseDelta += mouseDelta * sensitivity;
        _mouseDelta = Vector2.ClampMagnitude(_mouseDelta, clampDistance);
        
        cursor.anchoredPosition = _mouseDelta;
        CheckSelection();
    }

    public void OnPrimary(bool down, out OptionProvider.Option selectedOption)
    {
        selectedOption = null;
        
        if (down)
        {
            _pressed = true;
            _mouseDelta = Vector2.zero;
            
            _rt.DOSizeDelta(new Vector2(activeScaling, activeScaling), activationSmoothness);
            foreach (var option in _optionsMapping) {
                option.Value.gameObject.SetActive(true);
            }
        }
        if (!down && _pressed) {
            _pressed = false;
            cursor.anchoredPosition = Vector2.zero;
            
            if (_selectedOption!=null && _optionsMapping.TryGetValue(_selectedOption, out var selectedOptionTransform)) {
                selectedOptionTransform.GetComponent<TextMeshProUGUI>().color = Color.gray;
                selectedOption = _selectedOption;
            }

            _rt.DOSizeDelta(new Vector2(idleScaling, idleScaling), activationSmoothness);
            foreach (var option in _optionsMapping) {
                option.Value.gameObject.SetActive(false);
            }
        }
        
        cursor.gameObject.SetActive(down);
    }
    
    private void UpdateOptions(OptionProvider optionProvider)
    {
        foreach (Transform child in optionsContainer) {
            if(Application.isEditor) DestroyImmediate(child.gameObject);
            else Destroy(child.gameObject);
        }
        
        _optionsMapping.Clear();
        if(optionProvider==null) return;

        for (var i = 0; i < optionProvider.Options.Count; i++)
        {
            var option = optionProvider.Options[i];
            var optionTransform = Instantiate(optionPrefab, optionsContainer);
            var angle = 360f / optionProvider.Options.Count * i;
            
            var optionPosition = new Vector2(
                Mathf.Sin(angle * Mathf.Deg2Rad) * optionRadius,
                Mathf.Cos(angle * Mathf.Deg2Rad) * optionRadius);
            
            optionTransform.Set(option.OptionName, optionPosition, Color.gray);
            optionTransform.Hide(true);
            
            _optionsMapping.Add(option, optionTransform);
        }
    }

    private void CheckSelection()
    {
        if(_optionProvider==null) return;
        
        var optionSelected = false;
        foreach (var option in _optionProvider.Options)
        {
            var tr = _optionsMapping[option];
            tr.SetColor(Color.gray);
            
            var distance = Vector2.Distance(tr.Position, _mouseDelta);
            if (distance < selectionThreshold)
            {
                Debug.Log($"Selected: {option.OptionName}");
                _selectedOption = option;
                optionSelected = true;
                
                tr.SetColor(Color.white);
            }
        }
        if (!optionSelected && _selectedOption!=null && _optionsMapping.TryGetValue(_selectedOption, out var value)) {
            value.SetColor(Color.gray);
            _selectedOption = null;
        }
    }
}
