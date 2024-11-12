using System;
using UnityEngine;

namespace AdvancedController.Utilities
{
    public class MouseLock : MonoBehaviour
    {
        public CursorLockMode lockMode = CursorLockMode.Locked;
    
        private void OnValidate()
        {
            Cursor.lockState = lockMode;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                lockMode = Cursor.lockState==CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.lockState = lockMode;
            }
        }
    }
}

