using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private MusicClass musicPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;

        musicPlayer = GameObject.Find("MusicPlayer").GetComponent<MusicClass>();
        
    }

    private void Start()
    {
        //SetMusicVolume(GameStatus.MusicVolume);
        //SetSoundVolume(GameStatus.SoundVolume);
        
        EnableSounds();
        PlayMusic();
    }

    public void PitchSounds(float value)
    {
        audioMixer.SetFloat("SoundsPitch", value);
    }
    
    
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume)*20);
        //GameStatus.ChangeMusicVolume(volume);
    }
    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("Sounds FX", Mathf.Log10(volume)*20);
        //GameStatus.ChangeSoundVolume(volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        //GameStatus.ChangeMusicVolume(volume);
    }

    public void DisableSounds()
    {
        audioMixer.SetFloat("Sounds FX", -80f);
    }
    public void EnableSounds()
    {
        //SetSoundVolume(GameStatus.SoundVolume);
    }

    public void PlayMusic()
    {
        musicPlayer.PlayMusic();
    }
    public void StopMusic()
    {
        musicPlayer.StopMusic();
    }
}
