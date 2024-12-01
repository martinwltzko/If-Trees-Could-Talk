using UnityUtils.StateMachine;

namespace Code.Scripts.UI
{
    public class MovePlayerState : IState
    {
        private readonly Tutorial _tutorial;
    
        public MovePlayerState(Tutorial tutorial) {
            _tutorial = tutorial;
        }
        
        public void OnEnter()
        {
            _tutorial.OnMoveEntered();
        }
    }
    
    public class MoveCameraState : IState
    {
        private readonly Tutorial _tutorial;
    
        public MoveCameraState(Tutorial tutorial) {
            _tutorial = tutorial;
        }
        
        public void OnEnter()
        {
            _tutorial.OnCameraMoveEntered();
        }
    }
    
    public class InteractState : IState
    {
        private readonly Tutorial _tutorial;
    
        public InteractState(Tutorial tutorial) {
            _tutorial = tutorial;
        }
        
        public void OnEnter()
        {
            _tutorial.OnInteractEntered();
        }
    }
    
    public class TutorialFinishedState : IState
    {
        private readonly Tutorial _tutorial;
    
        public TutorialFinishedState(Tutorial tutorial) {
            _tutorial = tutorial;
        }
        
        public void OnEnter()
        {
            _tutorial.OnTutorialFinishedEntered();
        }
    }

}