using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private RectTransform _statusBar;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private ScaleType _scaleType;
    
    public float FillAmount
    {
        get
        {
            switch (_scaleType)
            {
                case ScaleType.Horizontal:
                    return _statusBar.localScale.x;
                case ScaleType.Vertical:
                    return _statusBar.localScale.y;
                default:
                    return 0f;
            }
        }
    }

    public void UpdateStatusBar(float value, float maxValue)
    {
        if(value>maxValue) value = maxValue;
        var percentage = Mathf.Clamp01(value / maxValue);

        switch (_scaleType)
        {
            case ScaleType.Horizontal:
                _statusBar.localScale = new Vector3(percentage, 1, 1);
                break;
            case ScaleType.Vertical:
                _statusBar.localScale = new Vector3(1, percentage, 1);
                break;
        }
        
        if(_statusText) _statusText.text = $"{value}/{maxValue}";
    }
    
    private enum ScaleType
    {
        Horizontal,
        Vertical
    }
}