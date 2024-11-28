using System;
using AdvancedController;
using Code.Scripts.UI;
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
    
    private AimingHandler AimingHandler => player.AimingHandler;
    private PlayerInteractions PlayerInteractions => player.PlayerInteractions;
    private UIController UIController => player.GetUIController();
    
    [SerializeField] private OptionProvider postMessageOptionProvider;
    [SerializeField] private OptionProvider stashMessageOptionProvider;
    

    public void CreateMessage()
    {
        var noteDisplay = UIController.NoteDisplay;
        if (noteDisplay.NoteText.Trim() == string.Empty)
        {
            Debug.Log("No message to post");
            return;
        }
        
        _messageCreated = true; 
        _message = Instantiate(messagePrefab);
        _message.GetComponent<Collider>().enabled = false;
        _message.SetMessage(noteDisplay.NoteText);
        noteDisplay.Note.ClearCachedText();
    }
    
    public void PostMessage()
    {
        if(!_messageCreated) return;
        
        WebHandler.Instance.GenerateMessage(_message.message, _message.transform.position, _message.transform.forward);
        SaveSystem.SaveFloat(SaveSystem.SaveVariable.MessageAmount, SaveSystem.GetFloat(SaveSystem.SaveVariable.MessageAmount)+1);
        
        PlayerInteractions.enabled = true;
        _messageCreated = false;
        _message.GetComponent<Collider>().enabled = true;
        UIController.SetOptionProvider(null);
    }
    
    public void StashMessage()
    {
        if(!_messageCreated) return;
        
        UIController.NoteDisplay.Note.SetNoteText(_message.message);
        UIController.SetOptionProvider(null);
        _messageCreated = false;
        Destroy(_message.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(_messageCreated)
        {
            //Note: SetOptionsProvider returns early if optionProvider didn't change, so it's safe to call it every frame
            UIController.SetOptionProvider(!AimingHandler.IsAiming
                ? stashMessageOptionProvider
                : postMessageOptionProvider);

            var position = AimingHandler.AimingPoint;
            var normal = AimingHandler.AimingNormal;
            
            _message.transform.position = position + normal * messageDistance;
            _message.transform.rotation = Quaternion.LookRotation(-normal);
        }
    }
}
