using System;
using System.Linq;
using System.Threading.Tasks;
using AdvancedController;
using AdvancedController.Utilities;
using Code.Scripts.Input;
using Code.Scripts.UI;
using EventHandling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI.Extensions;
using UnityUtils.StateMachine;

//TODO: Separate this class into a gameUI and main menu controller, also scan for all
// states in hierarchy automatically and add them to the state machine
public class UIController : MonoBehaviour
{
    [FormerlySerializedAs("uiInputReader")]
    [Header("General")] 
    [SerializeField] private UIInputReader inputReader;
    [SerializeField] private UIRaycaster uiRaycaster;
    [SerializeField] private MouseLock mouseLock;
    [SerializeField] private bool isMainMenu;

    [Header("UI Containers")]
    [SerializeField, HideIf("isMainMenu")] private GameObject ingameUIContainer;
    [SerializeField, HideIf("isMainMenu")] private GameObject pauseMenuContainer;
    [SerializeField, ShowIf("isMainMenu")] private GameObject mainMenuContainer;

    [Header("UI Elements")]
    [SerializeField, HideIf("isMainMenu")] private NoteDisplay noteDisplay;
    [SerializeField, HideIf("isMainMenu")] private SelectionCircle selectionCircle;

    [Header("UI States")] 
    [SerializeField, ShowIf("isMainMenu")] private MainMenuState mainMenuState;
    [SerializeField, HideIf("isMainMenu")] private PauseMenuState pauseMenuState;
    [SerializeField, HideIf("isMainMenu")] private InGameUIState inGameUIState;
    [SerializeField, HideIf("isMainMenu")] private NoteEditingState noteEditingState;
    [SerializeField, ShowIf("isMainMenu")] private CreditsState creditsState;
    [SerializeField, ShowIf("isMainMenu")] private NoNetworkState noNetworkState;
    [SerializeField] private OptionsMenuState optionsMenuState;
    [SerializeField] private AudioSettingsState audioSettingsState;
    [SerializeField] private VideoSettingsState videoSettingsState;
    [SerializeField] private GameSettingsState gameSettingsState;
    [SerializeField] private ApplyVideoSettingsState applyVideoSettingsState;
    
    
    [Header("Debug")]
    [SerializeField, Sirenix.OdinInspector.ReadOnly] private BehaviourState currentState;
    [SerializeField, Sirenix.OdinInspector.ReadOnly] private PlayerInstance player;
    
    private CameraController CameraController => player?.CameraController;
    public SelectionCircle SelectionCircle => selectionCircle;
    public NoteDisplay NoteDisplay => noteDisplay;

    private StateMachine _stateMachine;
    
    private bool _cancelPressedThisFrame;
    private bool _openNote;
    private bool _editingNote;
    
    public Action<OptionProvider.Option> OnOptionPressed = delegate { };

    private void Awake()
    {
        inputReader.Cancel += OnCancel;
        inputReader.Primary += OnClicking;
        inputReader.Secondary += OnCancel;
        
        WebHandler.OnAuthenticateFailed += OnNotConnected;
    }

    private void Start()
    {
        SetupStateMachine();
        if(!WebHandler.Authenticated) OnNotConnected();
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

    private void OnDestroy()
    {
        WebHandler.OnAuthenticateFailed -= OnNotConnected;
    }

    private void Update()
    {
        //RaycastMousePoint(out _mouseOverIgnores);
        _stateMachine.Update();
        if (_cancelPressedThisFrame) _cancelPressedThisFrame = false;
        if (selectionCircle) selectionCircle.UpdateSelection(inputReader.MouseDelta); 
    }

    private void FixedUpdate() => _stateMachine.FixedUpdate();
    
    private void OnUILoaded(PlayerInstance playerInstance) {
        player = playerInstance;
    }

    private void OnNotConnected()
    {
        ChangeState(noNetworkState);
    }

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
        creditsState?.Initialize(this);
        applyVideoSettingsState?.Initialize(this);
        noNetworkState?.Initialize(this);
        
        At(inGameUIState, pauseMenuState, () => _cancelPressedThisFrame);
        At(audioSettingsState, optionsMenuState, () => _cancelPressedThisFrame);
        At(videoSettingsState, optionsMenuState, () => _cancelPressedThisFrame);
        At(gameSettingsState, optionsMenuState, () => _cancelPressedThisFrame);
        
        At(optionsMenuState, mainMenuState, () => _cancelPressedThisFrame && isMainMenu);
        At(optionsMenuState, pauseMenuState, () => _cancelPressedThisFrame && !isMainMenu);
        At(pauseMenuState, inGameUIState, () => _cancelPressedThisFrame);
        
        At(applyVideoSettingsState, videoSettingsState, () => _cancelPressedThisFrame);

        Any(noteEditingState, () => _openNote);
        At(noteEditingState, inGameUIState, () => _cancelPressedThisFrame);
        
        At(creditsState, mainMenuState, () => _cancelPressedThisFrame && isMainMenu);
        At(noNetworkState, mainMenuState, () => _cancelPressedThisFrame && isMainMenu);
        
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
    
    public void ChangeState(BehaviourState state)
    {
        currentState = state;
        _stateMachine.ChangeState(state);
    }

    private void OnCancel(bool isPressed)
    {
        Debug.Log("Cancel pressed: " + isPressed);
        _cancelPressedThisFrame = isPressed;
    }
    
    private void OnClicking(bool isPressed)
    {
        Debug.Log("Click pressed: " + isPressed);
        if(selectionCircle == null) return;
        if (uiRaycaster.IsPointOver<IMouseOverIgnores>(inputReader.MousePosition)) return;
        if(_editingNote) mouseLock.SetMouseLock(CursorLockMode.Confined, !isPressed);
        
        CameraController.enabled = !isPressed;
        selectionCircle.OnPrimary(isPressed, inputReader.MousePosition, out var option);
        if (option != null) OnOptionPressed(option);

    }
    
    public void OnPauseMenuEnter()
    {
        if(ingameUIContainer) ingameUIContainer.SetActive(false);
        if(mainMenuContainer) mainMenuContainer.SetActive(false);
        if(pauseMenuContainer) pauseMenuContainer.SetActive(true);
        
        mouseLock.SetMouseLock(CursorLockMode.None, true);
        player?.InputReader.DisablePlayerActions();
    }
    
    public void OnMainMenuEnter()
    {
        if(ingameUIContainer) ingameUIContainer.SetActive(false);
        if(mainMenuContainer) mainMenuContainer.SetActive(true);
        if(pauseMenuContainer) pauseMenuContainer.SetActive(false);
        
        mouseLock.SetMouseLock(CursorLockMode.None, true);
        player?.InputReader.DisablePlayerActions();
    }
    
    public void OnInGameUIEnter()
    {
        if(ingameUIContainer) ingameUIContainer.SetActive(true);
        if(mainMenuContainer) mainMenuContainer.SetActive(false);
        if(pauseMenuContainer) pauseMenuContainer.SetActive(false);
        
        mouseLock.SetMouseLock(CursorLockMode.Locked, false);
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
            player.InputReader.DisablePlayerActions();
            mouseLock.SetMouseLock(CursorLockMode.Confined, true);
            CameraController.enabled = false;
            _editingNote = true;
        }
        else {
            player.InputReader.EnablePlayerActions();
            player.PlayerInteractions.ClearCurrentOptionProvider();
            noteDisplay.Note.gameObject.SetActive(false);
            mouseLock.SetMouseLock(CursorLockMode.Locked, false);
            CameraController.enabled = true;
            _editingNote = false;
        }
    }
    
    public void LoadSceneGroup(int index)
    {
        Bootstrapper.Instance.LoadSceneGroup(index);
    }
    
    public void QuitGame()
    {
        Bootstrapper.Instance.QuitGame();
    }
}