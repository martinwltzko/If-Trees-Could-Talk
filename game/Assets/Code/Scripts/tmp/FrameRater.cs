using System;
using UnityEngine;

namespace AdvancedController.Utilities
{
    public class FrameRater : MonoBehaviour
    {
        public int frameRate = 60;
        
        private void Awake()
        {
            Debug.Log($"Setting frame rate to {frameRate}");
            Application.targetFrameRate = frameRate;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Debug.Log($"Setting frame rate to {frameRate}");
                Application.targetFrameRate = frameRate;
            }
        }
    }
}