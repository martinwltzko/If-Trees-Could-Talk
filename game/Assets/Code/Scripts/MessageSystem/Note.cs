using System;
using Code.Scripts.UI;
using TMPro;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private string _cachedText = string.Empty;
    public string NoteText => inputField.text;
    
    private void Start()
    {
        _cachedText = inputField.text;
        
        inputField.onValueChanged.AddListener((text) =>
        {
            Debug.Log("Text changed to: " + text);
            _cachedText = inputField.text;
        });
    }

    public void SetNoteText(string text)
    {
        //_cachedText = inputField.text;
        
        inputField.SetTextWithoutNotify(text);
        inputField.interactable = false;
    }
    
    public void ClearCachedText()
    {
        _cachedText = string.Empty;
        inputField.SetTextWithoutNotify(string.Empty);
        inputField.interactable = true;
    }

    public void LoadCachedText()
    {
        inputField.SetTextWithoutNotify(_cachedText);
        inputField.interactable = true;
    }
}
