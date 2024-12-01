using System;
using System.Threading;
using Code.Scripts.Common.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameSystems.Common.Singletons;
using TMPro;
using UnityEngine;

public class Tooltip : RegulatorSingleton<Tooltip>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI tooltipLabel;
    [SerializeField] private RectTransform rt;
    
    private UniTask _showTask;
    private bool _enabled;
    
    
    public async void ShowTooltip(string text)
    {
        if (_showTask.Status == UniTaskStatus.Pending) {
            await _showTask;
        }

        _showTask = ShowTooltipTask(text);
        canvasGroup.SwitchCanvasGroupState(true);
    }
    
    public void HideTooltip()
    {
        canvasGroup.SwitchCanvasGroupState(false);
    }
    
    private async UniTask ShowTooltipTask(string text)
    {
        Debug.Log($"Showing tooltip: {text}");
        tooltipLabel.SetText(text);
        var width = tooltipLabel.preferredWidth;
        var height = tooltipLabel.preferredHeight;
        rt.sizeDelta = new Vector2(width, height);
    }
}