using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private List<SoundObject> playableSounds;
    
    private static int _currentId = 0;

    private int _previousState;

    public static AudioManager Instance { get; private set; }
    private int ID { get; set; }

    private void Awake()
    {
        _currentId++;
        ID = _currentId;
        
        if (Instance  ==  null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSourceMusic.Play();
    }

    public void PlaySound(Sounds soundType)
    {
        if (playableSounds.Count == 0) return;

        var soundToPlay = playableSounds.Find(x => x.SoundType == soundType);

        if (soundToPlay.IsPlaying()) soundToPlay.Stop();

        soundToPlay.Play();
    }

    public void StopSound(Sounds soundType)
    {
        var soundToPlay = playableSounds.Find(x => x.SoundType == soundType);

        if (soundToPlay.IsPlaying())
        {
            soundToPlay.Stop();
        }
    }
}