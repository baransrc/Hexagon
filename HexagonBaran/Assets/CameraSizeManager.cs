using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Camera))]
public class CameraSizeManager : MonoBehaviour
{
   [SerializeField] private float cameraSize;
   private Camera _camera;
   private const float Aspect9X16 = 9f/16f;
   private void Start() 
   {
      _camera = GetComponent<Camera>();
      AdjustSize();
   }

   private void AdjustSize()
   {
      if (_camera.aspect > Aspect9X16)
      {
         return;
      }
      
      var unitsPerPixel = cameraSize / Screen.width;

      var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

      _camera.orthographicSize = desiredHalfHeight;
   }
}
