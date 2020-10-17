using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreObject : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshPro scoreText;
    
    [Header("Color")]
    [SerializeField] private Color initialColor;
    [SerializeField] private Color endColor;
    [SerializeField] private Color disabledColor;

    [Header("Position")]
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Vector3 endScale;

    [Header("Duration")]
    [SerializeField] private float positionLerpMultiplier;
    [SerializeField] private float colorLerpMultiplier;
    [SerializeField] private float scaleLerpMultiplier;
    [SerializeField] private float duration;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    
    
    public void ShowScore(int score)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = new Vector3(1f, 1f, 1f);
        
        scoreText.text = "+" + score;
        
        StopAllCoroutines();
        StartCoroutine(ShowScoreCoroutine());
    }
    
    private IEnumerator ShowScoreCoroutine()
    {
        var step = 0f;

        endPosition.x = Random.Range(-3f, 3f);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        var playedAudio = false;

        while (step < 1f)
        {
            step += Time.deltaTime / duration;

            if (step >= 0.1f && !playedAudio)
            {
                audioSource.Play();
                playedAudio = true;
            }
            
            scoreText.color = Color.Lerp(initialColor, endColor, step * colorLerpMultiplier);

            transform.localPosition = Vector3.Lerp(transform.localPosition,
                                                   endPosition, 
                                                   step * positionLerpMultiplier);
            
            transform.localScale = Vector3.Lerp(transform.localScale, 
                                                endScale, 
                                                step * scaleLerpMultiplier);
            
            yield return null;
        }
        
        Recycle();
    }
    
    private void Recycle()
    {
        scoreText.text = "";
        
        scoreText.color = disabledColor;
        
        transform.localPosition = Vector3.zero;
        
        gameObject.SetActive(false);
    }
}
