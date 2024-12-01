using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Screenshotter : MonoBehaviour
{
    [SerializeField] private KeyCode screenshotKey;

    [SerializeField] private string path = "Docs/Screenshots/";
    [SerializeField, ReadOnly] private string fullPath;

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void OnValidate()
    {
        fullPath = Application.dataPath + $"/{path}";
    }


    private void HandleInput()
    {
        if (Input.GetKey(screenshotKey))
        {
            TakeScreenshot();
        }
    }

    [Button]
    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + $"/{path}" + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") +".png");
    } 
}
