using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class Request
{
    public static UniTask<string> Get(string url, string json="{}", string auth=default)
    {
        return SendRequest(url, json, auth, UnityWebRequest.kHttpVerbGET);
    }
    
    public static UniTask<string> Post(string url, string json="{}", string auth=default)
    {
        return SendRequest(url, json, auth, UnityWebRequest.kHttpVerbPOST);
    }
    
    public static UniTask<string> Put(string url, string json="{}", string auth=default)
    {
        return SendRequest(url, json, auth, UnityWebRequest.kHttpVerbPUT);
    }
    
    public static UniTask<string> Delete(string url, string json="{}", string auth=default)
    {
        return SendRequest(url, json, auth, UnityWebRequest.kHttpVerbDELETE);
    }

    private static async UniTask<string> SendRequest(string url, string json, string auth, string method)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(auth))
            {
                webRequest.SetRequestHeader("Authorization", auth);
            }

            webRequest.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(json))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"{method} RESPONSE {webRequest.responseCode} | {webRequest.downloadHandler.text}");
                return webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"{method} ERROR {webRequest.responseCode} | {webRequest.error}");
                return null;
            }
        }
    }
}