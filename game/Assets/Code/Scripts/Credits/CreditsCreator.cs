using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CreditsCreator", menuName = "Credits/CreditsCreator")]
public class CreditsCreator : ScriptableObject
{
    public string localPath = "Assets/Credits.json";
    [SerializeField, ReadOnly] private string path = Application.dataPath + $"/Assets/Credits.json";
    
    [Space(10)] public List<Credit> Credits;
    [Space(10)] public List<TrackCredit> MusicCredits;
    [Space(10)] public List<AssetCredit> AssetsCredits;
    [Space(10)] public List<Sprite> Logos;
    [Space(10)] public List<string> SpecialThanks;
    
    private readonly Dictionary<Roles, List<string>> _creditsDict = new(); 

    private void OnValidate()
    {
        path = Application.dataPath + $"/{localPath}";
    }
    
    [Button]
    public string GenerateJson(bool save=true)
    {
        _creditsDict.Clear();
        foreach (Roles role in Enum.GetValues(typeof(Roles)))
            _creditsDict[role] = new List<string>();
        
        foreach (var credit in Credits) 
            _creditsDict[credit.Role].AddRange(credit.Names);
        foreach (var credit in MusicCredits)
            _creditsDict[Roles.Music].Add(credit.ToString());
        foreach (var credit in AssetsCredits)
            _creditsDict[Roles.Assets].Add(credit.ToString());
        _creditsDict[Roles.SpecialThanks].AddRange(SpecialThanks);
        
        
        var json = JsonConvert.SerializeObject(_creditsDict, Formatting.Indented);
        Debug.Log(json);
        
        #if UNITY_EDITOR
        //Save to local path
        if (save) {
            var savePath = Application.dataPath + $"/{localPath}";
            System.IO.File.WriteAllText(savePath, json, System.Text.Encoding.UTF8);
        }
        
        #endif
        
        return json;
    }
    
    public List<Sprite> GetLogos() => Logos;
    
    [Serializable]
    public class Credit
    {
        public Roles Role;
        public List<string> Names;
    }
    
    [Serializable]
    public class AssetCredit
    {
        public string AssetName;
        public string CreatorName;
        
        public override string ToString() {
            return $"{AssetName} - {CreatorName}";
        }
    }
    
    [Serializable]
    public class TrackCredit
    {
        public string Title;
        public string Artist;
        public string Resource;
        public License License;
        
        public override string ToString() {
            return $"{Title} - {Artist}\n{Resource} under ({License})";
        }
    }

    public enum Roles
    {
        Programming,
        Art,
        Font,
        Music,
        Assets,
        SpecialThanks,
    }

    public enum License
    {
        CC_BY,
        CC_BY_SA,
        CC_BY_NC,
        CC_BY_NC_SA,
        CC0,
    }
}