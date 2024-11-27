using Code.Scripts.Common.Extensions;
using UnityEngine;
using UnityUtils.StateMachine;

namespace Code.Scripts.UI
{
    public sealed class InGameUIState : BehaviourState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Controller.OnInGameUIEnter();
        }

        //TODO: This is a hack. I should perhaps adjust the state machine for sub states
        public void ShowCanvas(bool value)
        {
            CanvasGroup.SwitchCanvasGroupState(value);
        }
    }
}