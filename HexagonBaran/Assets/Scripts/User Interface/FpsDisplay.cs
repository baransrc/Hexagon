using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FpsDisplay : MonoBehaviour
{
    private float _timeElapsedSinceLastUpdate = 0f;
    private float _timeNeededToUpdateMinFPSCounter = 5f;
    private int _minFPS = 144;
    private int _maxFPS = 0;
    private bool _firstFrame = true;
    
    private TextMeshProUGUI _fpsTextUI;

    [SerializeField] private float refreshInterval;
    [SerializeField] private bool showMinMax;

    private void Awake()
    {
        _fpsTextUI = GetComponent<TextMeshProUGUI>();        
    }

    private void UpdateFPSCounter()
    {
        var currentFPS = (int)(1.0f / Time.unscaledDeltaTime);

        _minFPS = (currentFPS < _minFPS) && (_timeNeededToUpdateMinFPSCounter <= 0f) ? currentFPS : _minFPS;
        _maxFPS = (currentFPS > _maxFPS) && (_timeNeededToUpdateMinFPSCounter <= 0f) ? currentFPS : _maxFPS;

        var fpsString = "FPS: " + currentFPS + "\n";
        
        var minMaxString = (_timeNeededToUpdateMinFPSCounter <= 0f) ? "MIN: " + _minFPS + "\n" + "MAX: " + _maxFPS
                                                                    : "MIN: ???" + "\n" + "MAX: ???";

        _fpsTextUI.text = (showMinMax) ? fpsString + minMaxString : fpsString;
    }


    private void Update()
    {
        _timeElapsedSinceLastUpdate += Time.unscaledDeltaTime;

        if (_timeNeededToUpdateMinFPSCounter > 0) _timeNeededToUpdateMinFPSCounter -= Time.unscaledDeltaTime;

        if (!(_timeElapsedSinceLastUpdate >= refreshInterval) && !_firstFrame)
        {
            return;
        }
        
        UpdateFPSCounter();
            
        _timeElapsedSinceLastUpdate = 0.0f;
            
        _firstFrame = false;
    }
}