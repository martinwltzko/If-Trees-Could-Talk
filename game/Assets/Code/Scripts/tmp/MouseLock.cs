using System;
using Unity.Collections;
using UnityEngine;

namespace AdvancedController.Utilities
{
    public class MouseLock : MonoBehaviour
    {
        [SerializeField, ReadOnly] private CursorLockMode currentLockMode;
        
        public void SetMouseLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void Update()
        {
            currentLockMode = Cursor.lockState;
        }
    }
}

