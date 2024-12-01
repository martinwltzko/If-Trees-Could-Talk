using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Code.Scripts.MessageSystem;
using Cysharp.Threading.Tasks;
using GameSystems.Common.Singletons;
using Newtonsoft.Json;
using QFSW.QC;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class WebHandler : RegulatorSingleton<WebHandler>
{
    [SerializeField] private string uri; // TODO: Change this to your server's IP
    [SerializeField, EnableIf("offlineMode")] private string token;
    [SerializeField, ReadOnly, HideIf("offlineMode")] private string playerId;
    [SerializeField] private bool offlineMode;

    public Action OnGetFailed;
    public Action OnPostFailed;
    public Action OnPutFailed;
    
    public static Action OnAuthenticateFailed;
    public static UniTask<bool> AuthenticationTask;
    public static bool Authenticated { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        AuthenticationTask = AuthenticateAsync();
    }

    private async UniTask<bool> AuthenticateAsync()
    {
        if (offlineMode) return true;
        try {
            await UnityServices.InitializeAsync();
            await Authenticate();

            Debug.Log("<color=green>Authenticated</color>");
            Authenticated = true;
            return true;
        }
        catch (Exception e)
        {
            OnAuthenticateFailed?.Invoke();
            Debug.Log(e);

            Authenticated = false;
            return false;
        }
    }

    private async UniTask Authenticate()
    {
        AdjustStringForHttps(ref uri);
        try
        {
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
        catch (AuthenticationException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [Command("netstat")]
    public void NetStatus()
    {
        Debug.Log($"Authenticated: {Authenticated}");
        Debug.Log($"Offline Mode: {offlineMode}");
    }
    
    [Command("web-get")]
    public void GetMessagesCommand()
    {
        GetMessages();
    }
    
    [Button]
    public async void GenerateMessage(string message, Vector3 position, Vector3 normal)
    {
        AdjustStringForHttps(ref uri);
        var auth = $"Token {token}";
        var positionDict = new Dictionary<string, float>() { { "x", position.x }, { "y", position.y }, { "z", position.z } };
        var normalDict = new Dictionary<string, float>() { { "x", normal.x }, { "y", normal.y }, { "z", normal.z } };
        var dataDict = new Dictionary<string, object> { 
            {"message", message }, 
            {"position", positionDict},
            {"normal", normalDict}
        };
        
        try
        {
            await Request.Post($"{uri}/api/messages/", auth:auth, json:JsonConvert.SerializeObject(dataDict));
        }
        catch (Exception e) {
            OnPostFailed?.Invoke();
            Debug.LogError(e);
            throw;
        }
    }
    
    [Button]
    public async void UpdateMessage(string id, string message)
    {
        AdjustStringForHttps(ref uri);
        
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
            AdjustStringForHttps(ref query);
            Debug.Log("Query: " + query);
            
            try {
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
            catch (Exception e) {
                OnGetFailed?.Invoke();
                Debug.LogError(e);
                throw;
            }
        }
        
        return messages.ToArray();
    }
    
    //TODO: Could be at a better place, ran out of time
    private static void AdjustStringForHttps(ref string url)
    {
        if (url.Contains("http://"))
            url = url.Replace("http://", "https://");
    }
}
