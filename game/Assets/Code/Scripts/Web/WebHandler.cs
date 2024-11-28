using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Code.Scripts.MessageSystem;
using Cysharp.Threading.Tasks;
using GameSystems.Common.Singletons;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class WebHandler : RegulatorSingleton<WebHandler>
{
    [SerializeField] private string uri = "http://127.0.0.1:8000"; // TODO: Change this to your server's IP
    [SerializeField, EnableIf("offlineMode")] private string token;
    [SerializeField, ReadOnly, HideIf("offlineMode")] private string playerId;
    [SerializeField] private bool offlineMode;
    
    private async void Start()
    {
        if (offlineMode) return;
        await UnityServices.InitializeAsync();
        await Authenticate();
    }

    private async UniTask Authenticate()
    {
        if (AuthenticationService.Instance.IsSignedIn) return;
        try {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var accessToken = AuthenticationService.Instance.AccessToken;
            playerId = AuthenticationService.Instance.PlayerId;
            
            var msg = await Request.Post(
                $"{uri}/api/authenticate/", 
                auth: $"Token {accessToken}",
                json: JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "playerId", playerId }
                }));
            token = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg)["token"];
            playerId = AuthenticationService.Instance.PlayerId;
            
            Debug.Log($"PlayerId: {playerId}");
            Debug.Log($"Token: {token}");
        }
        catch (Exception e) {
            Debug.LogError(e);
        }
    }
    
    [Button]
    public async void GenerateMessage(string message, Vector3 position, Vector3 normal)
    {
        var auth = $"Token {token}";
        var positionDict = new Dictionary<string, float>() { { "x", position.x }, { "y", position.y }, { "z", position.z } };
        var normalDict = new Dictionary<string, float>() { { "x", normal.x }, { "y", normal.y }, { "z", normal.z } };
        var dataDict = new Dictionary<string, object> { 
            {"message", message }, 
            {"position", positionDict},
            {"normal", normalDict}
        };
        await Request.Post($"{uri}/api/messages/", auth:auth, json:JsonConvert.SerializeObject(dataDict));
    }
    
    [Button]
    public async void UpdateMessage(string id, string message)
    {
        var auth = $"Token {token}";
        var dataDict = new Dictionary<string, string> { {"message", message } };
        await Request.Put($"{uri}/api/messages/{id}/", auth:auth, json:JsonConvert.SerializeObject(dataDict));
    }
    
    [Button]
    public async UniTask<WebMessage[]> GetMessages(int amount=50, int offset=0)
    {
        var messages = new List<WebMessage>();
        
        Queue<string> queries = new Queue<string>();
        queries.Enqueue($"{uri}/api/messages/?page=1");

        int pages = 1;
        while (queries.Count > 0)
        {
            var query = queries.Dequeue();
            var msg = await Request.Get(query);

            // Deserialize the JSON response into a Dictionary and get the results part
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            var resultsJson = JsonConvert.SerializeObject(json["results"]);

            // Deserialize 'results' directly into a list of WebMessageWrapper
            var webMessages = JsonConvert.DeserializeObject<List<WebMessage>>(resultsJson);
            messages.AddRange(webMessages);

            if (json.ContainsKey("next") && amount > webMessages.Count*pages) {
                if(json["next"]==null) break;
                queries.Enqueue(json["next"].ToString());
                pages++;
            }
        }
        
        return messages.ToArray();
    }
}
