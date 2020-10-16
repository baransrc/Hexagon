using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float defaultShakeMagnitude = 0.2f;
    [SerializeField] private float defaultDampingSpeed = 0.9f;
    private float _shakeMagnitude = 0.2f;
    private float _dampingSpeed = 0.5f;
    private float _shakeDuration = 0f;
    private Vector3 _initialPosition;

    private void OnEnable()
    {
        _initialPosition = transform.localPosition;
    }

    public void TriggerShake(float dampingSpeedMultiplier = 1f, float shakeMagnitudeMultiplier = 1f) 
    {
        if (dampingSpeedMultiplier <= 0f) return;
        
        _shakeDuration = 0.2f;
        _shakeMagnitude = defaultShakeMagnitude * shakeMagnitudeMultiplier; 
        _dampingSpeed = defaultDampingSpeed * dampingSpeedMultiplier;
    }

    private void Shake()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        
        if (_shakeDuration > 0)
        {
            transform.localPosition = _initialPosition + Random.insideUnitSphere * _shakeMagnitude;
   
            _shakeDuration -= Time.deltaTime * _dampingSpeed;
        }
        else
        {
            _shakeDuration = 0f;
            transform.localPosition = _initialPosition;
            _dampingSpeed = defaultDampingSpeed;
            _shakeMagnitude = defaultShakeMagnitude;
        }
    }
    
    private void Update()
    {
        Shake();
    }
}