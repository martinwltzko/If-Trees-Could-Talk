using System;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    [SerializeField] private InputReader input;
    public InputReader Input { get; private set; }

    private void Awake()
    {
        Input = ScriptableObject.CreateInstance<InputReader>();
        Input.EnablePlayerActions();
    }
}