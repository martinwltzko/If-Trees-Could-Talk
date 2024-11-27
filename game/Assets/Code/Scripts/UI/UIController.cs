using System;
using System.Threading.Tasks;
using AdvancedController.Utilities;
using Code.Scripts.Input;
using Code.Scripts.UI;
using EventHandling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.StateMachine;

public class UIController : MonoBehaviour
{
    [FormerlySerializedAs("uiInputReader")]
    [Header("General")] 
    [SerializeField] private UIInputReader inputReader;
    [SerializeField] private MouseLock mouseLock;
    [SerializeField] private bool isMainMenu;
    [SerializeField] private GameObject ingameUIContainer;
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject pauseMenuContainer;
    
    [Header("UI Elements")]
    [SerializeField] private NoteDisplay noteDisplay;
    [SerializeField] private SelectionCircle selectionCircle;
    public SelectionCircle SelectionCircle => selectionCircle;
    public NoteDisplay NoteDisplay => noteDisplay;
    
    [Header("UI States")] 
    [SerializeField] private MainMenuState mainMenuState;

    [SerializeField] private PauseMenuState pauseMenuState;
    [SerializeField] private OptionsMenuState optionsMenuState;
    [SerializeField] private InGameUIState inGameUIState;
    [SerializeField] private AudioSettingsState audioSettingsState;
    [SerializeField] private VideoSettingsState videoSettingsState;
    [SerializeField] private GameSettingsState gameSettingsState;
    [SerializeField] private NoteEditingState noteEditingState;
    [SerializeField] private CreditsState creditsState;
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private BehaviourState currentState;
    [SerializeField, ReadOnly] private PlayerInstance player;
    
    private StateMachine _stateMachine;
    private bool _cancelPressedThisFrame;
    private bool _primaryDown;
    private bool _openNote;
    
    public Action<OptionProvider.Option> OnOptionPressed = delegate { };

    private void Awake()
    {
        inputReader.Cancel += OnCancel;
        inputReader.Primary += OnClicking;
    }

    private void Start()
    {
        SetupStateMachine();
    }
    
    private void OnEnable() 
    {
        inputReader.EnablePlayerActions();
        EventBus<UILoadedEvent>.Ping(new UILoadedEvent(this, true, OnUILoaded));
    }

    private void OnDisable() {
        inputReader.DisablePlayerActions();
        EventBus<UILoadedEvent>.Ping(new UILoadedEvent(this, false, OnUILoaded));
    }

    private void OnUILoaded(PlayerInstance playerInstance) {
        player = playerInstance;
    }

    private void Update()
    {
        _stateMachine.Update();
        if (_cancelPressedThisFrame) _cancelPressedThisFrame = false;
        if (_primaryDown && selectionCircle) selectionCircle.UpdateSelection(inputReader.MouseDelta);
    }

    private void FixedUpdate() => _stateMachine.FixedUpdate();

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();
        
        mainMenuState?.Initialize(this);
        pauseMenuState?.Initialize(this);
        optionsMenuState?.Initialize(this);
        inGameUIState?.Initialize(this);
        audioSettingsState?.Initialize(this);
        videoSettingsState?.Initialize(this);
        gameSettingsState?.Initialize(this);
        noteEditingState?.Initialize(this);
        
        At(inGameUIState, pauseMenuState, () => _cancelPressedThisFrame);
        At(audioSettingsState, optionsMenuState, () => _cancelPressedThisFrame);
        At(videoSettingsState, optionsMenuState, () => _cancelPressedThisFrame);
        At(gameSettingsState, optionsMenuState, () => _cancelPressedThisFrame);
        
        At(optionsMenuState, mainMenuState, () => _cancelPressedThisFrame && isMainMenu);
        At(optionsMenuState, pauseMenuState, () => _cancelPressedThisFrame && !isMainMenu);
        At(pauseMenuState, inGameUIState, () => _cancelPressedThisFrame);

        Any(noteEditingState, () => _openNote);
        At(noteEditingState, inGameUIState, () => _cancelPressedThisFrame);
        
        At(creditsState, mainMenuState, () => _cancelPressedThisFrame && isMainMenu);
        
        _stateMachine.SetState(isMainMenu ? mainMenuState : inGameUIState);
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

    private void OnCancel(bool isPressed)
    {
        Debug.Log("Cancel pressed: " + isPressed);
        _cancelPressedThisFrame = isPressed;
    }
    
    private void OnClicking(bool isPressed)
    {
        Debug.Log("Click pressed: " + isPressed);
        _primaryDown = isPressed;
        if(selectionCircle == null) return;
        selectionCircle.OnPrimary(isPressed, out var option);
        if (option != null) OnOptionPressed(option);
    }

    public void LoadSceneGroup(int index)
    {
        Bootstrapper.Instance.LoadSceneGroup(index);
    }
    
    public void ChangeState(BehaviourState state)
    {
        currentState = state;
        _stateMachine.ChangeState(state);
    }
    
    public void OnPauseMenuEnter()
    {
        if(ingameUIContainer) ingameUIContainer.SetActive(false);
        if(mainMenuContainer) mainMenuContainer.SetActive(false);
        if(pauseMenuContainer) pauseMenuContainer.SetActive(true);
        
        mouseLock.SetMouseLock(false);
        player?.InputReader.DisablePlayerActions();
    }
    
    public void OnMainMenuEnter()
    {
        if(ingameUIContainer) ingameUIContainer.SetActive(false);
        if(mainMenuContainer) mainMenuContainer.SetActive(true);
        if(pauseMenuContainer) pauseMenuContainer.SetActive(false);
        
        mouseLock.SetMouseLock(false);
        player?.InputReader.DisablePlayerActions();
    }
    
    public void OnInGameUIEnter()
    {
        if(ingameUIContainer) ingameUIContainer.SetActive(true);
        if(mainMenuContainer) mainMenuContainer.SetActive(false);
        if(pauseMenuContainer) pauseMenuContainer.SetActive(false);
        
        mouseLock.SetMouseLock(true);
        player?.InputReader.EnablePlayerActions();
    }
    
    public void SetOptionProvider(OptionProvider optionProvider)
    {
        if(selectionCircle == null) return;
        selectionCircle.SetOptions(optionProvider);
    }

    public void OpenNote()
    {
        player.PlayerInteractions.SetOptionProvider(noteDisplay.WriteNoteOptions);
        noteDisplay.Note.LoadCachedText();
        noteDisplay.Note.gameObject.SetActive(true);
        ChangeState(noteEditingState);
        inGameUIState.ShowCanvas(true); //TODO: This is a hack to make sure the ingame GUI is not hidden when opening the note
    }

    public void OpenNote(Message message)
    {
        player.PlayerInteractions.SetOptionProvider(noteDisplay.ReadNoteOptions);
        noteDisplay.Note.SetNoteText(message.message);
        noteDisplay.Note.gameObject.SetActive(true);
        ChangeState(noteEditingState);
        inGameUIState.ShowCanvas(true); //TODO: This is a hack to make sure the ingame GUI is not hidden when opening the note
    }
    
    public void CloseNote()
    {
        ChangeState(inGameUIState);
    }
    
    public void OnNoteEditing(bool entering)
    {
        if(entering) {
            //PlayerInput.DisablePlayerActions();
            mouseLock.SetMouseLock(false);
        }
        else {
            //PlayerInput.EnablePlayerActions();
            player.PlayerInteractions.ClearCurrentOptionProvider();
            noteDisplay.Note.gameObject.SetActive(false);
            mouseLock.SetMouseLock(true);
        }
    }
}