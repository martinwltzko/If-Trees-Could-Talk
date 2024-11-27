using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace GameSystems.InputSystem
{
    public class InputComposer : MonoBehaviour
    {
        public static InputComposer Instance { get; private set; }
        public enum InputMode { Single, Additive }
    
        private static IInputReceiver[] _inputReceivers = new IInputReceiver[MAX_INPUT_RECEIVERS];
        public static IInputReceiver[] InputReceivers => _inputReceivers;
        public static int InputReceiverCount => _inputReceivers.Count(x => x!=null);
        public const int MAX_INPUT_RECEIVERS = 32;
    
        private static bool _overrideInput;
        private static IInputReceiver _overrideReceiver;
    
        private static CancellationToken _cancellationToken;
        
        private PlayerInput _playerInput;
    
        [RuntimeInitializeOnLoadMethod]
        private void Intialize() {
            Debug.Log("Input Composer cleanup");
            _inputReceivers = new IInputReceiver[MAX_INPUT_RECEIVERS];
        }

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    
        // Update is called once per frame
        void Update()
        {
            var input = _playerInput.FrameInput;
    
            if (_overrideInput) {
                if (_cancellationToken.IsCancellationRequested) {
                    _overrideReceiver = null;
                    _overrideInput = false;
                } else {
                    _overrideReceiver.UpdateInput(input);
                    return;
                }
            }
            foreach (var receiver in _inputReceivers) {
                if(receiver==null) continue;
                receiver.UpdateInput(input);
            }
        }
    
        public static void RegisterInput(IInputReceiver sender)
        {
            if (!_inputReceivers.Contains(sender))
            {
                if (InputReceiverCount + 1 >= MAX_INPUT_RECEIVERS) 
                    throw new Exception("Input receivers limit reached. Think about increasing MAX_INPUT_RECEIVERS.");
    
                _inputReceivers[InputReceiverCount] = sender;
            }
        }
    
        public static void OverrideInput(IInputReceiver sender, out CancellationTokenSource session)
        {
            session = new CancellationTokenSource();
            _cancellationToken = session.Token;
            _overrideReceiver = sender;
            _overrideInput = true;
        }
    
        public static void ReleaseInput(IInputReceiver sender)
        {
            if (_overrideReceiver == sender) {
                _overrideInput = false;
                _overrideReceiver = null;
                return;
            }
    
            if (_inputReceivers.Contains(sender))
            {
                var index = Array.IndexOf(_inputReceivers, sender);
                _inputReceivers[index] = null;
            }
        }
    }
}


