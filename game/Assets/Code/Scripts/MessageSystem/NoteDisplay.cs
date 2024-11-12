using AdvancedController;
using Code.Scripts.UI;
using UnityEngine;

public class NoteDisplay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PlayerInteractions playerInteractions;
    
    [SerializeField] private OptionProvider writeNoteProvider;
    [SerializeField] private OptionProvider readNoteProvider;
    [SerializeField] private Note note;
    
    public string NoteText => note.NoteText;
    
    public void WriteNote()
    {
        playerController.enabled = false;
        cameraController.enabled = false;
        playerInteractions.OverrideOptionProvider(writeNoteProvider);
        
        note.LoadCachedText();
        note.gameObject.SetActive(true);
    }
    
    public void ReadNote(Message message)
    {
        playerController.enabled = false;
        cameraController.enabled = false;
        playerInteractions.OverrideOptionProvider(readNoteProvider);
        
        note.SetNoteText(message.message);
        note.gameObject.SetActive(true);
    }
    
    public void CloseNote()
    {
        note.gameObject.SetActive(false);
        playerInteractions.ClearOptionProviderOverride();
        
        playerController.enabled = true;
        cameraController.enabled = true;
    }
    
    public void ClearNote()
    {
        note.ClearCachedText();
    }
}
