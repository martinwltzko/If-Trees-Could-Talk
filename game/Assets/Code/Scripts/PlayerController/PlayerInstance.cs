using System;
using AdvancedController;
using Code.Scripts.UI;
using Cysharp.Threading.Tasks;
using EventHandling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI.Extensions;

public class PlayerInstance : MonoBehaviour
{
    [field: SerializeField] public AimingHandler AimingHandler { get; private set; }
    [field: SerializeField] public PlayerController PlayerController { get; private set; }
    [field: SerializeField] public CameraController CameraController { get; private set; }
    [field: SerializeField] public PlayerInteractions PlayerInteractions { get; private set; }
    [field: SerializeField] public PlayerStats PlayerStats { get; private set; }
    [field: SerializeField] public InputReader InputReader { get; private set; }
    
    private EventBinding<UILoadedEvent> _uiLoadedEventBinding;
    public event Action<UIController> OnUiLoaded = delegate { };
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private UIController uiController;
    public UIController GetUIController()
    {
        if(uiController==null) Debug.LogError("UIController is null. Cant get it!");
        return uiController;
    }

    private void OnEnable()
    {
        _uiLoadedEventBinding = new EventBinding<UILoadedEvent>(UILoadedCallback);
        EventBus<UILoadedEvent>.Register(_uiLoadedEventBinding);
        EventBus<PlayerLoadedEvent>.Ping(new PlayerLoadedEvent(this, true));
    }

    private void OnDisable()
    {
        EventBus<PlayerLoadedEvent>.Ping(new PlayerLoadedEvent(this, false));
    }
    
    private void Start() 
    {
        InputReader.EnablePlayerActions();
    }
    
    private void OnDestroy() 
    {
        
        EventBus<UILoadedEvent>.Unregister(_uiLoadedEventBinding);
    }

    private async void UILoadedCallback(UILoadedEvent e)
    {
        if (!e.IsLoaded) return;
        uiController = e.UIController;
        e.Callback.Invoke(this);
        
        // Wait for the next frame to ensure everything is loaded, probably not necessary
        await UniTask.WaitForFixedUpdate();
        OnUiLoaded.Invoke(uiController);
    }
    
    public void DisablePlayerControl()
    {
        CameraController.gameObject.SetActive(false);
        PlayerController.gameObject.SetActive(false);
        AimingHandler.gameObject.SetActive(false);
    }
    
    public void EnablePlayerControl()
    {
        CameraController.gameObject.SetActive(true);
        PlayerController.gameObject.SetActive(true);
        AimingHandler.gameObject.SetActive(true);
        CameraController.CinemachineCamera.Prioritize();
        PlayerInteractions.ClearCurrentOptionProvider();
    }
}
