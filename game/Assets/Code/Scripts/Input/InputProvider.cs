using System;
using Code.Scripts.Input;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private UIInputReader uiInput;
    public InputReader Input { get; private set; }
    public UIInputReader UIInput { get; private set; }

    private void Awake()
    {
        Input = ScriptableObject.CreateInstance<InputReader>();
        UIInput = ScriptableObject.CreateInstance<UIInputReader>();
        
        Input.EnablePlayerActions();
        UIInput.EnablePlayerActions();
    }
}