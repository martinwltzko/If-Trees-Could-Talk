using UnityEngine;
using UnityEngine.Events;

public class FenceGate : MonoBehaviour
{
    [SerializeField] private bool multiplePlayable;
    private bool _played;
    
    [SerializeField] private UnityEvent onPlay;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if (_played && !multiplePlayable) return;
        _played = true;
        
        Debug.Log("Playing fence gate animation");
        onPlay.Invoke();
    }
}
