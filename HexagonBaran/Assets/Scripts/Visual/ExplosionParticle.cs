using System.Collections;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private AudioSource audioSource;
    
    public void Play(Color color, Vector3 position)
    {
        transform.position = position;
        
        var main = explosionParticle.main;
        main.startColor = color;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        audioSource.Play();

        StopAllCoroutines();
        
        StartCoroutine(PlayParticleCoroutine());
    }

    private IEnumerator PlayParticleCoroutine()
    {
        if (explosionParticle.isPlaying)
        {
            explosionParticle.Stop();
        }
        
        explosionParticle.Play();

        while (explosionParticle.isPlaying)
        {
            yield return null;
        }
        
        Recycle();
    }

    private void Recycle()
    {
        transform.position = Pool.SharedInstance.ItemSpawnLocation;
        
        gameObject.SetActive(false);
    }
}
