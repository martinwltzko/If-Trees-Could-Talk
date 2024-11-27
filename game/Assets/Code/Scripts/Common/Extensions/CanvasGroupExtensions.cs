using UnityEngine;

namespace Code.Scripts.Common.Extensions
{
    public static class CanvasGroupExtensions
    {
        public static void SwitchCanvasGroupState(this CanvasGroup canvasGroup, bool state)
        {
            canvasGroup.alpha = state ? 1 : 0;
            canvasGroup.blocksRaycasts = state;
            canvasGroup.interactable = state;
        }
    }
}