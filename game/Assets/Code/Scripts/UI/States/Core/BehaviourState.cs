using Code.Scripts.Common.Extensions;
using UnityEngine;
using UnityUtils.StateMachine;

namespace Code.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BehaviourState : MonoBehaviour, IStateBehaviour<UIController>
    {
        protected UIController Controller;
        [SerializeField] protected CanvasGroup CanvasGroup;

        // private void Awake()
        // {
        //     CanvasGroup = GetComponent<CanvasGroup>();
        // }
        
        public void Initialize(UIController controller)
        {
            Controller = controller;
        }
        
        public virtual void OnEnter() {
            CanvasGroup.SwitchCanvasGroupState(true);
        }

        public virtual void OnExit() {
            CanvasGroup.SwitchCanvasGroupState(false);
        }
    }
}