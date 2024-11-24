using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverDuration = .1f;
    [SerializeField] private float hoverFrequency = 1f;
    [SerializeField] private float hoverAmplitude = .1f;
    [SerializeField] private Ease hoverEase = Ease.InOutSine;
    public UnityEvent onClick;
    
    private RectTransform _rt;
    private bool _hovering;
    private float _hoverTime;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        OnPointerExit(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rt.DOScale(Vector3.one * hoverScale, hoverDuration).SetEase(hoverEase);
        _hovering = true;
        _hoverTime = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rt.DOScale(Vector3.one, hoverDuration).From(_rt.localScale).SetEase(hoverEase);
        _hovering = false;
    }

    private void Update()
    {
        if (!_hovering)
        {
            if(_rt.localScale == Vector3.one*hoverScale) OnPointerExit(null);
            return;
        }
        _hoverTime += Time.deltaTime;
        
        if(_hoverTime < hoverDuration) return;
        var scale = Mathf.Sin(_hoverTime * hoverFrequency) * hoverAmplitude + hoverScale;
        _rt.localScale = Vector3.one * scale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}
