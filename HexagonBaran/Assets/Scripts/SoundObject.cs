using UnityEngine;

[System.SerializableAttribute]
public class SoundObject
{
    [SerializeField] private Sounds soundType;
    [SerializeField] private AudioSource audioSource;

    public Sounds SoundType
    {
        get
        {
            return soundType;
        }

        private set
        {
            
        }
    }
    
    public void Play()
    {
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}