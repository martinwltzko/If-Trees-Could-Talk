using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIRaycaster : MonoBehaviour
{
    private readonly RaycastResult[] _raycastResults = new RaycastResult[16];
    [SerializeField, ReadOnly] private int raycastResultsCount;

    private void Update()
    {
        //TODO: Remove this debug code
        raycastResultsCount = RaycastMousePoint(_raycastResults);
    }
    
    public bool IsPointOver<T>(Vector2 point)
    {
        var results = RaycastAtPoint(point);
        return results.Exists(r => r.gameObject.GetComponent<T>() != null);
    }

    public List<RaycastResult> RaycastAtPoint(Vector2 point)
    {
        var pointerEventData = new PointerEventData(EventSystem.current) { position = point };
        var allRaycastResults = new List<RaycastResult>();

        // Find all canvases in the scene
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var canvas in canvases)
        {
            // Get the GraphicRaycaster for this canvas
            var graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
            {
                Debug.LogWarning($"Canvas {canvas.name} does not have a GraphicRaycaster.");
                continue;
            }

            // Perform the raycast
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, raycastResults);
            allRaycastResults.AddRange(raycastResults);
        }
        
        Debug.Log("UI Hits: " + allRaycastResults.Count);
        return allRaycastResults;
    }

    public int RaycastMousePoint(RaycastResult[] results)
    {
        results.ForEach(r => r.Clear());
        
        var mousePosition = Mouse.current.position.ReadValue();

        // Prepare a PointerEventData for the raycast
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) {
            position = mousePosition
        };

        // Store all raycast results across canvases
        List<RaycastResult> allRaycastResults = new List<RaycastResult>();

        // Find all canvases in the scene
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var canvas in canvases)
        {
            // Get the GraphicRaycaster for this canvas
            var graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
            {
                Debug.LogWarning($"Canvas {canvas.name} does not have a GraphicRaycaster.");
                continue;
            }

            // Perform the raycast
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, raycastResults);
            allRaycastResults.AddRange(raycastResults);
        }

        Debug.Log("UI Hits: " + allRaycastResults.Count);
        for(int i = 0; i < allRaycastResults.Count; i++)
        {
            if(i >= results.Length) break;
            results[i] = allRaycastResults[i];
        }
        return Mathf.Min(allRaycastResults.Count, results.Length);
    }
}