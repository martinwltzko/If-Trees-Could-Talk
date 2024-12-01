using UnityEngine;
using UnityEngine.Diagnostics;
using UnityUtils.StateMachine;

public class TooltipRelay : MonoBehaviour
{
    public void ShowTooltip(string text)
    {
        Tooltip.Instance.ShowTooltip(text);
    }
    
    public void HideTooltip()
    {
        Tooltip.Instance.HideTooltip();
    }
}