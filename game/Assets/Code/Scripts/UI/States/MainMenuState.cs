using System;
using Code.Scripts.Common.Extensions;
using UnityEngine;
using UnityUtils.StateMachine;

namespace Code.Scripts.UI
{
    public sealed class MainMenuState : BehaviourState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Controller.OnMainMenuEnter();
        }
    }
}