using AdvancedController;
using UnityEngine;

public class MessagePoster : MonoBehaviour
{
    [SerializeField] private NoteDisplay noteDisplay;
    [SerializeField] private InputProvider inputProvider;
    [SerializeField] private PlayerInteractions playerInteractions;
    [SerializeField] private AimingHandler aimingHandler;
    [SerializeField] private Message messagePrefab;
    [SerializeField] private float messageDistance;

    private string _messageText;
    private Message _message;
    private bool _messageCreated;
    private bool _messagePosted;
    
    private InputReader Input => inputProvider.Input;
    
    private void Start()
    {
        Input.Primary += OnPrimary;
    }

    private void OnPrimary(bool down)
    {
        if (!down) return;
        if (!aimingHandler.IsAiming) return;
        if(_messageCreated) PostMessage();
    }

    public void CreateMessage()
    {
        _messageCreated = true;
        playerInteractions.enabled = false;
        
        _message = Instantiate(messagePrefab);
        _message.GetComponent<Collider>().enabled = false;
        _message.SetMessage(noteDisplay.NoteText);
    }
    
    public void PostMessage()
    {
        if(!_messageCreated) return;
        
        WebHandler.Instance.GenerateMessage(_message.message, _message.transform.position, _message.transform.forward);
        
        playerInteractions.enabled = true;
        _messageCreated = false;
        _message.GetComponent<Collider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_messageCreated)
        {
            var position = aimingHandler.AimingPoint;
            var normal = aimingHandler.AimingNormal;
            
            _message.transform.position = position + normal * messageDistance;
            _message.transform.rotation = Quaternion.LookRotation(-normal);
        }
    }
}
