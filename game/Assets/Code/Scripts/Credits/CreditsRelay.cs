using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsRelay : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField] private bool loadFromResources = false;
    [SerializeField, ShowIf("loadFromResources")] private string localPath = "Resources/Credits/credit.json";
    [SerializeField, ReadOnly, ShowIf("loadFromResources")] private string path;
    #endif
    
    [SerializeField] private TextMeshProUGUI creditsTextPrefab;
    [SerializeField] private VerticalLayoutGroup creditsParent;
    [SerializeField] private RectTransform scrollContentRect;

    [SerializeField] private CreditsCreator creditsCreator;

    private int _blocks = 0;
    private int _amountMessages;
    private float _preferredHeight = 200;

    private void Start() {
        _amountMessages = (int)SaveSystem.GetFloat(SaveSystem.SaveVariable.MessageAmount);

        #if UNITY_EDITOR
        if (loadFromResources) {
            var json = System.IO.File.ReadAllText(Application.dataPath + $"/{localPath}");
            GenerateCredits(json);
            return;
        }
        #endif
        
        GenerateCredits(creditsCreator.GenerateJson(save:false));
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        path = Application.dataPath + $"/{localPath}";
    }
    #endif

    [Button]
    private void GenerateCredits()
    {
        GenerateCredits(creditsCreator.GenerateJson());
    }
    private void GenerateCredits(string json)
    {
        var creditsDict = JsonConvert.DeserializeObject<Dictionary<CreditsCreator.Roles, List<string>>>(json);

        foreach (Transform child in creditsParent.transform) {
            if(Application.isEditor) DestroyImmediate(child.gameObject);
            else Destroy(child.gameObject);
        }
        foreach (var (role, text) in creditsDict)
        {
            var creditText = Instantiate(creditsTextPrefab, creditsParent.transform);
            StringBuilder sb = new StringBuilder();
            foreach (var line in text) {
                sb.Append(line);
                sb.Append("\n");
            }
            creditText.text = $"{role}:\n{sb}";
            AdjustHeight(creditText, creditText.rectTransform);
            _preferredHeight += creditText.preferredHeight;
            _blocks += 1;
        }
        if (_amountMessages > 0)
        {
            var narrativeCredit = Instantiate(creditsTextPrefab, creditsParent.transform);
            narrativeCredit.text = $"Narrative:\n Messages (x{_amountMessages}) - You";
            _preferredHeight += narrativeCredit.preferredHeight;
            _blocks += 1;
        }
        
        AdjustHeight(_preferredHeight + _blocks*creditsParent.spacing, scrollContentRect);
    }
    
    public void AdjustHeight(float preferredHeight, RectTransform rectTransform)
    {
        // Adjust the RectTransform's height
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = preferredHeight;
        rectTransform.sizeDelta = sizeDelta;
    }
    
    public void AdjustHeight(TextMeshProUGUI textMeshPro, RectTransform rectTransform)
    {

        // Force TextMeshPro to update its layout
        textMeshPro.ForceMeshUpdate();

        // Get the preferred height of the text content
        float preferredHeight = textMeshPro.preferredHeight;

        // Adjust the RectTransform's height
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = preferredHeight;
        rectTransform.sizeDelta = sizeDelta;
    }
}