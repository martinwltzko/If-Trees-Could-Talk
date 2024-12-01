using System;
using Code.Scripts.UI;
using EventHandling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils.StateMachine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private bool forceTutorial;
    [SerializeField, ReadOnly] private PlayerInstance player;
    private StateMachine _stateMachine;
    private bool _tutorialFinished;

    private bool _playerMoved;
    private bool _cameraMoved;
    private bool _interacted;
    
    private EventBinding<PlayerLoadedEvent> _playerLoadedEventBinding;
    
    private IState _movePlayerState;
    private IState _moveCameraState;
    private IState _interactState;
    private IState _tutorialFinishedState;


    private void Awake()
    {
        _playerLoadedEventBinding = new EventBinding<PlayerLoadedEvent>((e) => {
            player = e.PlayerInstance;
            player.InputReader.Look += (_, _) => _cameraMoved = true;
            player.InputReader.Move += (_) => _playerMoved = true;
            player.GetUIController().OnOptionPressed += (_) => _interacted = true;
        });
        
        EventBus<PlayerLoadedEvent>.Register(_playerLoadedEventBinding);
    }

    private void OnDestroy()
    {
        EventBus<PlayerLoadedEvent>.Unregister(_playerLoadedEventBinding);
    }

    private void Start()
    {
        _tutorialFinished = Mathf.Approximately(SaveSystem.GetFloat(SaveSystem.SaveVariable.TutorialPlayed), 1f);
        SetupStateMachine();
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine?.FixedUpdate();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();

        _movePlayerState = new MovePlayerState(this);
        _moveCameraState = new MoveCameraState(this);
        _interactState = new InteractState(this);
        _tutorialFinishedState = new TutorialFinishedState(this);

        At(_movePlayerState, _moveCameraState, () => _playerMoved);
        At(_moveCameraState, _interactState, () => _cameraMoved);
        At(_interactState, _tutorialFinishedState, () => _interacted);
        Any(_tutorialFinishedState, () => _interacted);
    }
    
    private void At(IState from, IState to, Func<bool> condition)
    {
        if(from==null || to==null) return;
        _stateMachine.AddTransition(from, to, condition);
    }

    private void Any(IState to, Func<bool> condition)
    {
        if(to==null) return;
        _stateMachine.AddAnyTransition(to, condition);
    }

    public void StartTutorial()
    {
        Debug.Log("Starting tutorial");
        var state = _tutorialFinished ? _tutorialFinishedState : _movePlayerState;
        _stateMachine.SetState(forceTutorial ? _movePlayerState : state);
    }
    
    public void OnMoveEntered()
    {
        Tooltip.Instance.ShowTooltip("Move the player with WASD");
    }
    public void OnCameraMoveEntered()
    {
        Tooltip.Instance.ShowTooltip("Move the camera with the mouse");
    }
    public void OnInteractEntered()
    {
        Tooltip.Instance.ShowTooltip("Hold left mouse button to open interaction menu");
    }
    public void OnTutorialFinishedEntered()
    {
        _tutorialFinished = true;
        Tooltip.Instance.HideTooltip();
        SaveSystem.SaveFloat(SaveSystem.SaveVariable.TutorialPlayed, 1f);
    }
}