using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClass : MonoBehaviour
{
    private static MusicClass _instance;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) 
            return;
        else
            _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}
