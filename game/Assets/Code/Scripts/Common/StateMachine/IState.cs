using System;

namespace UnityUtils.StateMachine {
    public interface IState {
        void Update() { }
        void FixedUpdate() { }
        void OnEnter() { }
        void OnExit() { }
    }
    
    public interface IStateBehaviour<in T> : IState {
        void Initialize(T stateMachine);
    }
    
    public enum StateMoment {
        Update,
        FixedUpdate,
        OnEnter,
        OnExit
    }
}