using AdvancedController;
using Code.Scripts.UI;
using UnityEngine;

public class NoteDisplay : MonoBehaviour
{
    [SerializeField] private OptionProvider writeNoteProvider;
    [SerializeField] private OptionProvider readNoteProvider;
    [SerializeField] private Note note;

    public OptionProvider WriteNoteOptions => writeNoteProvider;
    public OptionProvider ReadNoteOptions => readNoteProvider;
    public Note Note => note;
    
    public string NoteText => note.NoteText;
    
    // public void WriteNote()
    // {
    //     PlayerInteractions.SetOptionProvider(writeNoteProvider);
    //     
    //     note.LoadCachedText();
    //     note.gameObject.SetActive(true);
    // }
    //
    // public void ReadNote(Message message)
    // {
    //     PlayerInteractions.SetOptionProvider(readNoteProvider);
    //     
    //     note.SetNoteText(message.message);
    //     note.gameObject.SetActive(true);
    // }
    //
    // public void CloseNote()
    // {
    //     note.gameObject.SetActive(false);
    //     PlayerInteractions.ClearCurrentOptionProvider();
    // }
    
    // public void ClearNote()
    // {
    //     note.ClearCachedText();
    // }
}
