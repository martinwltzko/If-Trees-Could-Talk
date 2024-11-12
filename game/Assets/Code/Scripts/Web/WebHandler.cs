using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class WebHandler : MonoBehaviour
{
    public static WebHandler Instance { get; private set; }
    
    [SerializeField] private string uri = "http://127.0.0.1:8000"; // TODO: Change this to your server's IP
    [SerializeField] private string token;
    [SerializeField] private string playerId;
    
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    private async void Start()
    {
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
    public async void GenerateMessage(string message)
    {
        var auth = $"Token {token}";
        var dataDict = new Dictionary<string, string> { {"playerId", playerId}, {"message", message } };
        await Request.Post($"{uri}/api/messages/", auth:auth, json:JsonConvert.SerializeObject(dataDict));
    }
    
    [Button]
    public async void UpdateMessage(string id, string message)
    {
        var auth = $"Token {token}";
        var dataDict = new Dictionary<string, string> { {"playerId", playerId}, {"message", message } };
        await Request.Put($"{uri}/api/messages/{id}/", auth:auth, json:JsonConvert.SerializeObject(dataDict));
    }
    
    [Button]
    public async UniTask<string[]> GetMessages(int amount=50, int offset=0)
    {
        var messages = new List<string>();
        Queue<string> queries = new Queue<string>();
        queries.Enqueue($"{uri}/api/messages/?page=1");

        int pages = 1;
        while (queries.Count > 0)
        {
            var query = queries.Dequeue();
            var msg = await Request.Get(query);
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            var results = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json["results"].ToString());
            
            messages.AddRange(ParseMessages(results));
            
            if (json.ContainsKey("next") && amount > results.Count*pages) {
                if(json["next"]==null) break;
                queries.Enqueue(json["next"].ToString());
                pages++;
            }
        }
        
        return messages.ToArray();
    }

    private List<string> ParseMessages(List<Dictionary<string, string>> results)
    {
        var messages = new List<string>();
        foreach (var dict in results)
        {
            // Debug.Log("Id:" + dict["id"]);
            // Debug.Log("PlayerId:" + dict["owner"]);
            messages.Add(dict["message"]);
            //Debug.Log("Message:" + dict["message"]);
        }

        return messages;
    }
}
