using UnityEngine;

public class AmbientPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.InitializeAmbientSound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
