using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button pauseButton;
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private float pauseMenuFadeDuration;

    private void Awake()
    {
        pauseMenu.alpha = 0f;
        pauseMenu.gameObject.SetActive(false);
    }

    public void OnClickPauseButton()
    {
        AudioManager.Instance.PlaySound(Sounds.Button);
        gameManager.Paused = true;
        StartCoroutine(EnablePauseMenu());
    }

    private IEnumerator EnablePauseMenu()
    {
        pauseButton.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        pauseMenu.alpha = 0f;
        
        while (pauseMenu.alpha < 1f)
        {
            var alpha = pauseMenu.alpha;
            alpha += Time.unscaledDeltaTime / pauseMenuFadeDuration;
            alpha = alpha > 1f ? 1f : alpha;
            
            pauseMenu.alpha = alpha;

            yield return null;
        }
    }

    private IEnumerator DisablePauseMenu()
    {
        pauseMenu.alpha = 1f;
        
        while (pauseMenu.alpha > 0f)
        {
            var alpha = pauseMenu.alpha;
            alpha -= Time.unscaledDeltaTime / pauseMenuFadeDuration;
            alpha = alpha < 0f ? 0f : alpha;
            
            pauseMenu.alpha = alpha;

            yield return null;
        }
        
        pauseButton.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        gameManager.Paused = false;
    }
    
    public void OnClickContinue()
    {
        AudioManager.Instance.PlaySound(Sounds.Button);
        StartCoroutine(DisablePauseMenu());
    }

    public void OnClickExit()
    {
        AudioManager.Instance.PlaySound(Sounds.Button);
        gameManager.EndGame();
    }
}
