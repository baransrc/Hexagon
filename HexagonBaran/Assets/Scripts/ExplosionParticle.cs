using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplosionParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionParticle;
    
    public void Play(Color color, Vector3 position)
    {
        transform.position = position;
        
        var main = explosionParticle.main;

        main.startColor = color;

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
