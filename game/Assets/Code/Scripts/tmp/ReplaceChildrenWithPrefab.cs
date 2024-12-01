using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ReplaceChildrenWithPrefab : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    
    #if UNITY_EDITOR
    [Button]
    public async void Replace()
    {
        List<GameObject> instances = new List<GameObject>();
        foreach (Transform child in transform)
        {
            var instance = Instantiate(prefab);
            instance.transform.position = child.position;
            instance.transform.rotation = child.rotation;
            instance.transform.localScale = child.localScale;
            instances.Add(instance);
            
            DestroyImmediate(child.gameObject);
            await UniTask.Delay(100);
        }
        
        foreach (var instance in instances) {
            instance.transform.SetParent(transform);
        }
    }
    #endif
}
