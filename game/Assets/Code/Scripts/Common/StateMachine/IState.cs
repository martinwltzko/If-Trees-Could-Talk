using System;

namespace UnityUtils.StateMachine {
    public interface IState {
        void Update() { }
        void FixedUpdate() { }
        void OnEnter() { }
        void OnExit() { }
    }
    
    public enum StateMoment {
        Update,
        FixedUpdate,
        OnEnter,
        OnExit
    }
}