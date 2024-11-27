using System;
using AdvancedController;
using UnityEngine;

public class MessagePoster : MonoBehaviour
{
    [SerializeField] private PlayerInstance player;

    [SerializeField] private Message messagePrefab;
    [SerializeField] private float messageDistance;

    private string _messageText;
    private Message _message;
    private bool _messageCreated;
    private bool _messagePosted;
    
    private InputReader Input => player.InputReader;
    private AimingHandler AimingHandler => player.AimingHandler;
    private PlayerInteractions PlayerInteractions => player.PlayerInteractions;
    //private NoteDisplay NoteDisplay => player.UIController.NoteDisplay;
    
    private void Start()
    {
        Input.Primary += OnPrimary;
    }

    private void OnPrimary(bool down)
    {
        if (!down) return;
        if (!AimingHandler.IsAiming) return;
        if(_messageCreated) PostMessage();
    }

    public void CreateMessage()
    {
        var noteDisplay = player.GetUIController().NoteDisplay;
        if (noteDisplay.NoteText.Trim() == string.Empty)
        {
            Debug.Log("No message to post");
            return;
        }
        
        _messageCreated = true;
        PlayerInteractions.enabled = false;
        
        _message = Instantiate(messagePrefab);
        _message.GetComponent<Collider>().enabled = false;
        _message.SetMessage(noteDisplay.NoteText);
        noteDisplay.Note.ClearCachedText();
    }
    
    public void PostMessage()
    {
        if(!_messageCreated) return;
        
        WebHandler.Instance.GenerateMessage(_message.message, _message.transform.position, _message.transform.forward);
        
        PlayerInteractions.enabled = true;
        _messageCreated = false;
        _message.GetComponent<Collider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_messageCreated)
        {
            var position = AimingHandler.AimingPoint;
            var normal = AimingHandler.AimingNormal;
            
            _message.transform.position = position + normal * messageDistance;
            _message.transform.rotation = Quaternion.LookRotation(-normal);
        }
    }
}
