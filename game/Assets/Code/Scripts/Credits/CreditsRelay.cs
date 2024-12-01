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
    [SerializeField] private Image creditsLogoPrefab;
    [SerializeField] private VerticalLayoutGroup creditsParent;
    [SerializeField] private RectTransform scrollContentRect;
    [SerializeField] private HorizontalLayoutGroup logoParent;

    [SerializeField] private CreditsCreator creditsCreator;

    private int _blocks = 0;
    private int _amountMessages;
    [SerializeField] private float initialHeight = 500;

    private void Start() {
        _amountMessages = (int)SaveSystem.GetFloat(SaveSystem.SaveVariable.MessageAmount);

        #if UNITY_EDITOR
        if (loadFromResources) {
            var json = System.IO.File.ReadAllText(Application.dataPath + $"/{localPath}");
            GenerateCredits(json, creditsCreator.Logos);
            return;
        }
        #endif
        
        GenerateCredits(creditsCreator.GenerateJson(save:false), creditsCreator.Logos);
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
        GenerateCredits(creditsCreator.GenerateJson(), creditsCreator.Logos);
    }
    private void GenerateCredits(string json, List<Sprite> logos)
    {
        var creditsDict = JsonConvert.DeserializeObject<Dictionary<CreditsCreator.Roles, List<string>>>(json);
        var preferredHeight = initialHeight;
        
        // Clear existing credits
        foreach (Transform child in creditsParent.transform) {
            if(Application.isEditor) DestroyImmediate(child.gameObject);
            else Destroy(child.gameObject);
        }
        
        // Credits
        foreach (var (role, text) in creditsDict)
        {
            if(role==CreditsCreator.Roles.SpecialThanks) continue;
            
            var creditText = Instantiate(creditsTextPrefab, creditsParent.transform);
            StringBuilder sb = new StringBuilder();
            foreach (var line in text) {
                sb.Append(line);
                sb.Append("\n");
            }
            creditText.text = $"{role}:\n{sb}";
            AdjustHeight(creditText, creditText.rectTransform);
            initialHeight += creditText.preferredHeight;
            _blocks += 1;
        }
        
        // Messages / Stats
        if (_amountMessages > 0)
        {
            var narrativeCredit = Instantiate(creditsTextPrefab, creditsParent.transform);
            narrativeCredit.text = $"Narrative:\n Messages (x{_amountMessages}) - You";
            preferredHeight += narrativeCredit.preferredHeight;
            _blocks += 1;
        }
        
        // Special Thanks
        var specialThanks = Instantiate(creditsTextPrefab, creditsParent.transform);
        var builder = new StringBuilder();
        foreach (var line in creditsDict[CreditsCreator.Roles.SpecialThanks]) {
            builder.Append(line);
            builder.Append("\n");
        }
        specialThanks.text = $"{builder}";
        AdjustHeight(specialThanks, specialThanks.rectTransform);
        preferredHeight += specialThanks.preferredHeight;
        _blocks += 1;
    
        // Logos
        int imageSizeY = 0;
        int imageSizeX = 0;
        foreach (var logo in logos) {
            var image = Instantiate(creditsLogoPrefab, logoParent.transform);
            image.sprite = logo;
            image.SetNativeSize();
            if(image.rectTransform.sizeDelta.y > imageSizeY)
                imageSizeY = (int)image.rectTransform.sizeDelta.y;
            imageSizeX += (int)image.rectTransform.sizeDelta.x;
        }
        logoParent.transform.parent = null;
        ((RectTransform)logoParent.transform).sizeDelta = new Vector2(imageSizeX, imageSizeY);
        logoParent.transform.parent = creditsParent.transform;
        
        // Adjust the height of the scroll content
        AdjustHeight(preferredHeight + _blocks*creditsParent.spacing + imageSizeY, scrollContentRect);
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