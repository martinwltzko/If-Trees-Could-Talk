using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    [SerializeField] private Transform tree;
    [SerializeField] private float height;
    [SerializeField] private float radius;
    
    [SerializeField] private int messageCount;
    [SerializeField] private Message messagePrefab;
    [SerializeField] private float messageOffset;
    
    private readonly List<Message> _messages = new List<Message>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        var messages = await WebHandler.Instance.GetMessages();
        SpawnMessages(messages);
    }

    [Button]
    public void SpawnMessages(string[] messages)
    {
        foreach (var message in _messages)
        {
            if(Application.isEditor)
                DestroyImmediate(message.gameObject);
            else
                Destroy(message.gameObject);
        }
        
        _messages.Clear();
        for (int i = 0; i < messages.Length; i++)
        {
            var position = GetRandomPosition();
            var delta = Vector3.ProjectOnPlane(transform.position-position, Vector3.up);

            Debug.DrawRay(position, delta, Color.red, 10f);
            
            var hits = Physics.RaycastAll(position, delta, 1000f);
            if (hits.Length == 0) {
                continue;
            }
            
            foreach (var hit in hits)
            {
                if(hit.transform != tree) continue;
                
                var message = Instantiate(messagePrefab, hit.point + hit.normal*messageOffset, Quaternion.LookRotation(-hit.normal), transform);
                message.SetMessage(messages[i]);
                _messages.Add(message);
                break;
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Generate a normally distributed value for the angle
        float angle = Mathf.Clamp01((float)NormalDistribution()) * 2 * Mathf.PI;

        // Generate a random value for the height
        float y = UnityEngine.Random.Range(-height/2f, height/2f);
        float x = UnityEngine.Random.Range(-radius, radius);
        float z = UnityEngine.Random.Range(-radius, radius);
        
        // // Calculate the position on the cylinder mantle
        // float x = Mathf.Cos(angle) * radius;
        // float z = Mathf.Sin(angle) * radius;

        return new Vector3(x, y, z) + transform.position;
    }

    // Using Box-Muller transform to generate a normal distribution
    private double NormalDistribution()
    {
        double u1 = 1.0 - UnityEngine.Random.value; // uniform(0,1] random doubles
        double u2 = 1.0 - UnityEngine.Random.value;
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
        return randStdNormal;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(radius, height, radius));
    }
}