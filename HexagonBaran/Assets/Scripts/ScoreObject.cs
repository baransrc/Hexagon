using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private Color initialColor;
    [SerializeField] private Color endColor;
    [SerializeField] private Color disabledColor;

    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Vector3 endScale;

    [SerializeField] private float positionLerpMultiplier;
    [SerializeField] private float colorLerpMultiplier;
    [SerializeField] private float scaleLerpMultiplier;
    
    [SerializeField] private float duration;
    
    private Vector3 initialPosition;
    
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
        
        while (step < 1f)
        {
            step += Time.deltaTime / duration;

            scoreText.color = Color.Lerp(initialColor,
                endColor,
                step * colorLerpMultiplier);

            transform.localPosition = Vector3.Lerp(transform.localPosition,
                endPosition,
                step * positionLerpMultiplier);
            
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, 
                step * scaleLerpMultiplier);
            
            yield return 0;
        }

        Recycle();
    }
    
    private void Recycle()
    {
        scoreText.text = "";
        scoreText.color = disabledColor;
        transform.localPosition = initialPosition;
        
        gameObject.SetActive(false);
    }
}
