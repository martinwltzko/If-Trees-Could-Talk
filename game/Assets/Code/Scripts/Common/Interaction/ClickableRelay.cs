using UnityEngine;
using UnityEngine.Events;

namespace Interaction
{
    public class ClickableRelay : MonoBehaviour, IClickable
    {
        [SerializeField] private UnityEvent _onClick;
    
        public void OnClick()
        {
            Debug.Log("Clicked!", this);
            _onClick.Invoke();
        }
    }
}