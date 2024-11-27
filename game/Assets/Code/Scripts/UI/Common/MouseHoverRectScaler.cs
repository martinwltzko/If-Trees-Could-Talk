using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHoverRectScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverDuration = .1f;
    [SerializeField] private float hoverFrequency = 1f;
    [SerializeField] private float hoverAmplitude = .1f;
    [SerializeField] private Ease hoverEase = Ease.InOutSine;
    
    private RectTransform _rt;
    private bool _hovering;
    private float _hoverTime;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        _rt.localScale = Vector3.one;
        _hovering = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rt.DOScale(Vector3.one * hoverScale, hoverDuration).SetEase(hoverEase);
        _hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rt.DOScale(Vector3.one, hoverDuration).From(_rt.localScale).SetEase(hoverEase);
        _hovering = false;
    }
    
    private void Update()
    {
        if(!_hovering) return;
        // if (!_hovering)
        // {
        //     if(_rt.localScale == Vector3.one*hoverScale) OnPointerExit(null);
        //     return;
        // }
        _hoverTime += Time.deltaTime;
        
        if(_hoverTime < hoverDuration) return;
        var scale = Mathf.Sin(_hoverTime * hoverFrequency) * hoverAmplitude + hoverScale;
        _rt.localScale = Vector3.one * scale;
    }
}