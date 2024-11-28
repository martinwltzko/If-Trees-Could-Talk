using System;
using Unity.Collections;
using UnityEngine;

namespace AdvancedController.Utilities
{
    public class MouseLock : MonoBehaviour
    {
        [SerializeField, ReadOnly] private CursorLockMode currentLockMode;
        
        public void SetMouseLock(CursorLockMode lockMode, bool visible)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = visible;
        }

        private void Update()
        {
            currentLockMode = Cursor.lockState;
        }
    }
}

