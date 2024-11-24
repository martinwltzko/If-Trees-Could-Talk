using UnityEngine;

[System.Serializable]
public class SoundRelay
{
    public SoundLibrary soundLibrary;
    public AudioSource audioSource;

    public void PlaySoundOneshot(string soundId)
    {
        AudioClip clip = soundLibrary.Match(soundId).RandomAudioClip();
        audioSource.PlayOneShot(clip);
    }

    public void PlaySound(string soundId)
    {
        audioSource.clip = soundLibrary.Match(soundId).RandomAudioClip();
        audioSource.Play();
    }

    public void PlayLoopedSound(string soundId)
    {
        audioSource.clip = soundLibrary.Match(soundId).RandomAudioClip();
        audioSource.loop = true;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }


}
