using UnityEngine;
using UnityUtils.StateMachine;

namespace Code.Scripts.UI
{
    public sealed class PauseMenuState : BehaviourState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Controller.OnPauseMenuEnter();
        }
    }
}